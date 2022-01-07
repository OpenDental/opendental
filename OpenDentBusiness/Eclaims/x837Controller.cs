using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness.Eclaims
{
	/// <summary></summary>
	public partial class x837Controller {

		private static string RECSFileExistsMsg => Lans.g("FormClaimsSend","You must send your existing claims from the RECS program before you can "
			+"create another batch.");

		static x837Controller() {
			//Need to initiliaze this here because ODCloudClient doesn't have access to OpenDentBusiness.Lans.
			_lans_g=Lans.g;
		}

		///<summary></summary>
		public x837Controller()
		{
			
		}

		///<summary>Gets the filename for this batch. Used when saving.</summary>
		private static string GetFileName(Clearinghouse clearinghouseClin,int batchNum,bool isAutomatic) { //called from this.SendBatch. Clinic-level clearinghouse passed in.
			string saveFolder=clearinghouseClin.ExportPath;
			if(!ODBuild.IsWeb() && !Directory.Exists(saveFolder)) {
				if(!isAutomatic) {
					MessageBox.Show(saveFolder+" not found.");
				}
				return "";
			}
			if(clearinghouseClin.CommBridge==EclaimsCommBridge.RECS){
				if(!ODBuild.IsWeb() && File.Exists(ODFileUtils.CombinePaths(saveFolder,"ecs.txt"))){
					if(!isAutomatic) {
						MessageBox.Show(RECSFileExistsMsg);
					}
					return "";//prevents overwriting an existing ecs.txt.
				}
				return ODFileUtils.CombinePaths(saveFolder,"ecs.txt");
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {
				//creating workstation specific claim upload folders for claimconnect
				string filePath = ODFileUtils.CombinePaths(saveFolder, ODEnvironment.MachineName);
				if(!Directory.Exists(filePath)) {
					try {
						Directory.CreateDirectory(filePath);
					}
					catch (Exception e) {
						MessageBox.Show(e.Message);
					}
				}
				saveFolder = filePath;
				return ODFileUtils.CombinePaths(saveFolder,"claims"+batchNum.ToString()+".txt");
			}
			else{
				return ODFileUtils.CombinePaths(saveFolder,"claims"+batchNum.ToString()+".txt");
			}
		}

		///<summary>If file creation was successful but communications failed, then this deletes the X12 file.  This is not used in the Tesia bridge because of the unique filenaming.</summary>
		public static void Rollback(Clearinghouse clearinghouseClin,int batchNum) {//called from various eclaims classes. Clinic-level clearinghouse passed in.
			if(clearinghouseClin.CommBridge==EclaimsCommBridge.RECS) {
				//A RECS rollback never deletes the file, because there is only one
			}
			else {
				//This is a Windows extension, so we do not need to worry about Unix path separator characters.
				File.Delete(ODFileUtils.CombinePaths(clearinghouseClin.ExportPath,"claims"+batchNum.ToString()+".txt"));
			}
		}

		///<summary>Called from Eclaims and includes multiple claims.  Returns the string that was sent.  
		///The string needs to be parsed to determine the transaction numbers used for each claim.</summary>
		public static string SendBatch(Clearinghouse clearinghouseClin,List<ClaimSendQueueItem> queueItems,int batchNum,EnumClaimMedType medType,
			bool isAutomatic) 
		{
			//each batch is already guaranteed to be specific to one clearinghouse, one clinic, and one EnumClaimMedType
			//Clearinghouse clearhouse=ClearinghouseL.GetClearinghouse(queueItems[0].ClearinghouseNum);
			string saveFile="";
			if(clearinghouseClin.IsClaimExportAllowed) {
				saveFile=GetFileName(clearinghouseClin,batchNum,isAutomatic);
				if(saveFile==""){
					return "";
				}
			}
			string messageText;
			try{
				messageText=GenerateBatch(clearinghouseClin,queueItems,batchNum,medType);
			}
			catch(ODException odex) {
				MessageBox.Show(odex.Message,"x837");
				return "";
			}
			if(clearinghouseClin.IsClaimExportAllowed) {
				if(clearinghouseClin.CommBridge==EclaimsCommBridge.PostnTrack) {
					//need to clear out all CRLF from entire file
					messageText=messageText.Replace("\r","");
					messageText=messageText.Replace("\n","");
				}
				if(ODBuild.IsWeb() && DoSendBatchToCloudClient(clearinghouseClin)) {
					try {
						ODCloudClient.ExportClaim(saveFile,messageText,doOverwriteFile:clearinghouseClin.CommBridge!=EclaimsCommBridge.RECS);
					}
					catch(ODException odEx) {
						if(odEx.ErrorCodeAsEnum==ODException.ErrorCodes.FileExists && clearinghouseClin.CommBridge==EclaimsCommBridge.RECS) {
							MessageBox.Show(RECSFileExistsMsg,"x837");
						}
						else {
							MessageBox.Show(odEx.Message,"x837");
						}
						if(odEx.ErrorCodeAsEnum!=ODException.ErrorCodes.ClaimArchiveFailed) {//If archiving failed, we can continue with sending.
							return "";
						}
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message,"x837");
						return "";
					}
				}
				else {
					File.WriteAllText(saveFile,messageText,Encoding.ASCII);
					CopyToArchive(saveFile);
				}
			}
			return messageText;
		}

		///<summary>True if the x837 batch file should be created on the Cloud user's client machine. False it it should be created on the server.</summary>
		public static bool DoSendBatchToCloudClient(Clearinghouse clearinghouse) {
			if(clearinghouse.CommBridge==EclaimsCommBridge.ClaimConnect) {//ClaimConnect does everything without a client program.
				return false;
			}
			return true;
		}

		///<summary>Helper method for SendBatch that generates the X837 messagetext. This method is public as it is used in ClaimConnect.cs</summary>
		public static string GenerateBatch(Clearinghouse clearinghouseClin,List<ClaimSendQueueItem> queueItems,int batchNum,EnumClaimMedType medType) {
			string messageText="";
			using(MemoryStream ms=new MemoryStream()) {
				using(StreamWriter sw=new StreamWriter(ms)) {
					if(clearinghouseClin.Eformat==ElectronicClaimFormat.x837D_4010) {
						X837_4010.GenerateMessageText(sw,clearinghouseClin,batchNum,queueItems);
					}
					else {//Any of the 3 kinds of 5010
						X837_5010.GenerateMessageText(sw,clearinghouseClin,batchNum,queueItems,medType);
					}
					sw.Flush();//Write contents of sw into ms.
					ms.Position=0;//Reset position to 0, or else the StreamReader below will not read anything.
					using(StreamReader sr=new StreamReader(ms,Encoding.ASCII)) {
						messageText=sr.ReadToEnd();
					}
				}
			}
			return messageText;
		}

		/// <summary>True if all listClaimProcs point to a procNum in listAllProcs, otherwise false.
		/// Called when sending a claim to validate that all claimProcs for a claim exists in the list of procNums.</summary>
		/// <param name="listClaimProcs">List of claimProcs for a specific claim.</param>
		/// <param name="listAllProcs">List of all procs for a patient or all procs for a specific claim.</param>
		/// <param name="claim">The claim associated to given list of claimProcs.</param>
		/// <param name="error">An out param that describes the error when returning false, otherwise blank.</param>
		/// <returns>True if all claimProcs point to a procNum in given list, otherwise false.</returns>
		public static bool HasValidProcNums(List<ClaimProc> listClaimProcs, List<Procedure> listAllProcs,Claim claim,out string error) {
			error="";
			List<long> listProcNums=listAllProcs.Select(y => y.ProcNum).ToList();
			if(listClaimProcs.Any(x => !ListTools.In(x.ProcNum,listProcNums))) {//claimProcs does not contain total payment rows or Canadian lab estimates.
				//Eventually we loop through claimProcs, calling Procedures.GetProcFromList(), which returns a "blank" procedure if no procedure in 
				//procList exists that matches the claimproc.  This results in a Procedure with null properties (ex: Procedure.CodeMod1), which cannot be
				//written to the functional group.  A solution is for the user to delete the claim, then run DBM, which will remove the orphaned claimproc.
				error=Lans.g("x837","The following claim is linked to an invalid estimate:\r\nPatNum: "+claim.PatNum
					+"\r\nDate: "+claim.DateService+"\r\nDelete the claim, run Database Maintenance, and recreate the claim.");
			}
			return error.IsNullOrEmpty();
		}

		

		
	



		

	}
}
