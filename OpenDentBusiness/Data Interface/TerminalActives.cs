using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class TerminalActives {
		///<summary>Gets a list of all TerminalActives.  Used by FormTerminalManager.  Data is retrieved when FormTerminalManager initially loads or when
		///signalods of type Kiosk are processed by the form to refill the grid if necessary.</summary>
		public static List<TerminalActive> Refresh() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TerminalActive>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM terminalactive ORDER BY ComputerName,SessionName";
			return Crud.TerminalActiveCrud.SelectMany(command);
		}

		///<summary>DEPRECATED.  Use GetForCompAndSession.  Only kept for FormTerminalOld references, which is never displayed. Consider deleting.</summary>
		public static TerminalActive GetTerminal(string computerName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TerminalActive>(MethodBase.GetCurrentMethod(),computerName);
			}
			string command="SELECT * FROM terminalactive WHERE ComputerName ='"+POut.String(computerName)+"'";
			return Crud.TerminalActiveCrud.SelectOne(command);
		}

		///<summary>Get one TerminalActive from the db with ComputerName, SessionId, and ProcessId.  ComputerName is case-insensitive.
		///Will return null if not found.</summary>
		public static TerminalActive GetForCmptrSessionAndId(string computerName,int sessionId,int processId=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TerminalActive>(MethodBase.GetCurrentMethod(),computerName,sessionId,processId);
			}
			string command="SELECT * FROM terminalactive "
				+"WHERE ComputerName='"+POut.String(computerName)+"' "
				+"AND SessionId="+POut.Int(sessionId);
			if(processId>0) {
				command+=" AND ProcessId="+POut.Int(processId);
			}
			return Crud.TerminalActiveCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(TerminalActive te) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),te);
				return;
			}
			Crud.TerminalActiveCrud.Update(te);
		}

		///<summary>Used to set the patient for a kiosk, to either load a patient or pass in patNum==0 to clear a patient.  Does nothing if termNum is not
		///a valid TerminalActiveNum.</summary>
		public static void SetPatNum(long termNum,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),termNum,patNum);
				return;
			}
			if(termNum<1) {
				return;//invalid TerminalActiveNum, just return
			}
			string command="UPDATE terminalactive SET PatNum="+POut.Long(patNum)+" WHERE TerminalActiveNum="+POut.Long(termNum);
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary></summary>
		public static long Insert(TerminalActive te) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				te.TerminalActiveNum=Meth.GetLong(MethodBase.GetCurrentMethod(),te);
				return te.TerminalActiveNum;
			}
			return Crud.TerminalActiveCrud.Insert(te);
		}
		
		///<summary>DEPRECATED.  Use DeleteForCompAndSession.  Only kept for FormTerminalOld references, which is never displayed.</summary>
		public static void DeleteAllForComputer(string computerName){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName);
				return;
			}
			string command="DELETE FROM terminalactive WHERE ComputerName ='"+POut.String(computerName)+"'";
			Db.NonQ(command);
		}
		
		///<summary>This can be used to delete a specific terminalactive by computer name, session ID and process ID, e.g. when the terminal window closes
		///or when the delete button is pressed in the terminal manager window.  Also used to clear any left over terminalactives when starting a terminal
		///for a specific computer and session, in which case you can supply a process ID to exclude so the current terminal won't be deleted but all
		///others for the computer and session will be.  ComputerName is case-insensitive.</summary>
		public static void DeleteForCmptrSessionAndId(string computerName,int sessionId,int processId=0,int excludeId=0){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName,sessionId,processId,excludeId);
				return;
			}
			string command="DELETE FROM terminalactive WHERE ComputerName='"+POut.String(computerName)+"' AND SessionId="+POut.Int(sessionId);
			if(processId>0) {
				command+=" AND ProcessId="+POut.Int(processId);
			}
			if(excludeId>0) {
				command+=" AND ProcessId!="+POut.Int(excludeId);
			}
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary>Called whenever user wants to edit patient info.  Not allowed to if patient edit window is open at a terminal.  Once patient is done
		///at terminal, then staff allowed back into patient edit window.</summary>
		public static bool PatIsInUse(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM terminalactive WHERE PatNum="+POut.Long(patNum)
				+" AND (TerminalStatus="+POut.Long((int)TerminalStatusEnum.PatientInfo)
				+" OR TerminalStatus="+POut.Long((int)TerminalStatusEnum.UpdateOnly)+")";
			return Db.GetCount(command)!="0";
		}

		///<summary>Returns true if a terminal is already in the database with ComputerName=compName and ClientName=clientName or if either names are null
		///or whitespace.  Otherwise false.  Case-insensitive name comparison.  Allow the same ClientName for different computers.</summary>
		public static bool IsCompClientNameInUse(string compName,string clientName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),compName,clientName);
			}
			if(string.IsNullOrWhiteSpace(compName) || string.IsNullOrWhiteSpace(clientName)) {
				return true;//this will prevent them from using blank or null for the client name
			}
			string command="SELECT COUNT(*) FROM terminalactive "
				+"WHERE ComputerName='"+POut.String(compName)+"' "
				+"AND SessionName='"+POut.String(clientName)+"'";
			return Db.GetCount(command)!="0";
		}		
	}

		



		
	

	

	


}










