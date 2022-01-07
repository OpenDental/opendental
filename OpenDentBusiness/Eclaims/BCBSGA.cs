using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDentBusiness.Eclaims {
	public class BCBSGA {

		public static string ErrorMessage="";

		public BCBSGA() {

		}

		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created X12 file. The batchnum is supplied for the possible rollback.</summary>
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum,ITerminalConnector terminalConnector) { 
			bool retVal=true;
			try {
				terminalConnector.ShowForm();
				terminalConnector.OpenConnection(clearinghouseClin.ModemPort);
				//1. Dial
				terminalConnector.Dial("17065713158");
				//2. Wait for connect, then pause 3 seconds
				terminalConnector.WaitFor("CONNECT 9600",50000);
				terminalConnector.Pause(3000);
				terminalConnector.ClearRxBuff();
				//3. Send Submitter login record
				string submitterLogin=
					//position,length indicated for each
					"/SLRON"//1,6 /SLRON=Submitter login
					+terminalConnector.Sout(clearinghouseClin.LoginID,12,12)//7,12 Submitter ID
					+terminalConnector.Sout(clearinghouseClin.Password,8,8)//19,8 submitter password
					+"NAT"//27,3 use NAT
						//30,8 suggested 8-BYTE CRC of the file for unique ID. No spaces.
						//But I used the batch number instead
					+batchNum.ToString().PadLeft(8,'0')
					+"ANSI837D  1    "//38,15 "ANSI837D  1    "=Dental claims
					+"X"//53,1 X=Xmodem, or Y for transmission protocol
					+"ANSI"//54,4 use ANSI
					+"BCS"//58,3 BCS=BlueCrossBlueShield
					+"00";//61,2 use 00 for filler
				terminalConnector.Send(submitterLogin);
				//4. Receive Y, indicating login accepted
				if(terminalConnector.WaitFor("Y","N",20000)=="Y") {
					//5. Wait 1 second.
					terminalConnector.Pause(1000);
				}
				else {
					//6. If login rejected, receive an N,
					//followed by Transmission acknowledgement explaining
					throw new Exception(terminalConnector.Receive(5000));
				}
				//7. Send file using X-modem or Z-modem
				//slash not handled properly if missing:
				terminalConnector.UploadXmodem(clearinghouseClin.ExportPath+"claims"+batchNum.ToString()+".txt");
				//8. After transmitting, pause for 1 second.
				terminalConnector.Pause(1000);
				terminalConnector.ClearRxBuff();
				//9. Send submitter logout record
				string submitterLogout=
					"/SLROFF"//1,7 /SLROFF=Submitter logout
					+terminalConnector.Sout(clearinghouseClin.LoginID,12,12)//8,12 Submitter ID
					+batchNum.ToString().PadLeft(8,'0')//20,8 matches field in submitter Login
					+"!"//28,1 use ! to retrieve transmission acknowledgement record
					+"\r\n";
				terminalConnector.Send(submitterLogout);
				//10. Prepare to receive the Transmission acknowledgement.  This is a receipt.
				terminalConnector.Receive(5000);//this is displayed in the progress box so user can see.
			}
			catch(Exception ex) {
				ErrorMessage=ex.Message;
				x837Controller.Rollback(clearinghouseClin,batchNum);
				retVal=false;
			}
			finally {
				terminalConnector.CloseConnection();
			}
			return retVal;
		}

		///<summary>Retrieves any waiting reports from this clearinghouse. Returns true if the communications were successful, and false if they failed.</summary>
		public static bool Retrieve(Clearinghouse clearinghouseClin,bool isAutomatic,ITerminalConnector terminalConnector,IODProgressExtended progress=null) {
			progress=progress??new ODProgressExtendedNull();
			bool retVal=true;
			try {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server and downloading reports"),"reports","17%",17);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				terminalConnector.ShowForm();
				terminalConnector.OpenConnection(clearinghouseClin.ModemPort);
				terminalConnector.Dial("17065713158");
				//2. Wait for connect, then pause 3 seconds
				terminalConnector.WaitFor("CONNECT 9600",50000);
				terminalConnector.Pause(3000);
				terminalConnector.ClearRxBuff();
				//1. Send submitter login record
				string submitterLogin=
					"/SLRON"//1,6 /SLRON=Submitter login
					+terminalConnector.Sout(clearinghouseClin.LoginID,12,12)//7,12 Submitter ID
					+terminalConnector.Sout(clearinghouseClin.Password,8,8)//19,8 submitter password
					+"   "//27,3 use 3 spaces
	//Possible issue with Trans ID
					+"12345678"//30,8. they did not explain this field very well in documentation
					+"*              "//38,15 "    *          "=All available. spacing ok?
					+"X"//53,1 X=Xmodem, or Y for transmission protocol
					+"MDD "//54,4 use 'MDD '
					+"VND"//58,3 Vendor ID is yet to be assigned by BCBS
					+"00";//61,2 Software version not important
				byte response=(byte)'Y';
				string retrieveFile="";
				progress.UpdateProgress(Lans.g(progress.LanThis,"Web server contact successful."));
				progress.UpdateProgress(Lans.g(progress.LanThis,"Downloading files"),"reports","33%",33);
				if(progress.IsPauseOrCancel()) {
					return false;
				}
				while(response==(byte)'Y') {
					terminalConnector.ClearRxBuff();
					terminalConnector.Send(submitterLogin);
					response=0;
					while(response!=(byte)'N'	&& response!=(byte)'Y' && response!=(byte)'Z') {
						response=terminalConnector.GetOneByte(20000);
						terminalConnector.ClearRxBuff();
						Application.DoEvents();
					}
					//2. If not accepted, N is returned
					//3. and must receive transmission acknowledgement
					if(response==(byte)'N') {
						progress.UpdateProgress(terminalConnector.Receive(10000));
						break;
					}
					//4. If login accepted, but no records, Z is returned. Hang up.
					if(response==(byte)'Z') {
						progress.UpdateProgress(Lans.g(progress.LanThis,"No reports to retrieve."));
						break;
					}
					//5. If record(s) available, Y is returned, followed by dos filename and 32 char subj.
					//less than one second since all text is supposed to immediately follow the Y
					retrieveFile=terminalConnector.Receive(800).Substring(0,12);//12 char in dos filename
					terminalConnector.ClearRxBuff();
					//6. Pause for 200 ms. (already mostly handled);
					terminalConnector.Pause(200);
					//7. Receive file using Xmodem
					//path must include trailing slash for now.
					terminalConnector.DownloadXmodem(clearinghouseClin.ResponsePath+retrieveFile);
					//8. Pause for 5 seconds.
					terminalConnector.Pause(5000);
					//9. Repeat all steps including login until a Z is returned.
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"Closing connection to web server"),"reports","50%",50);
				if(progress.IsPauseOrCancel()) {
					return false;
				}
			}
			catch(Exception ex) {
				ErrorMessage=ex.Message;
				retVal=false;
			}
			finally {
				terminalConnector.CloseConnection();
			}
			return retVal;
		}

	}
}
