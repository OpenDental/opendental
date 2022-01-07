using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OpenDentBusiness;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness.Eclaims {
	public class Dutch {

		///<summary>Called from Eclaims and includes multiple claims.  This should return the text of the file that was sent so that it can be saved in 
		///the db.  If this returns an empty result, then claims won't be marked as sent.  The problem is that we create multiple files here.</summary>
		public static string SendBatch(Clearinghouse clearinghouseClin,List<ClaimSendQueueItem> queueItems,int batchNum) {
			//We assume for now one file per claim.
			for(int i=0;i<queueItems.Count;i++) {
				if(!CreateClaim(clearinghouseClin,queueItems[i],batchNum)) {
					return "";
				}
			}
			return "Sent";//no need to translate.  User will not see.
		}

		///<summary>Called once for each claim to be created.  For claims with a lot of procedures, this may actually create multiple claims.  
		///Normally returns empty string unless something went wrong.</summary>
		public static bool CreateClaim(Clearinghouse clearinghouseClin,ClaimSendQueueItem queueItem,int batchNum) {
			StringBuilder strb=new StringBuilder();
			string t="\t";
			strb.Append("110\t111\t112\t118\t203/403\tF108/204/404\t205/405\t206\t207\t208\t209\t210\t211\t212\t215\t217\t219\t406\t408\t409\t410\t411\t413\t414\t415\t416\t418\t419\t420\t421\t422\t423\t424\t425\t426\t428\t429\t430\t432\t433\r\n");
			Claim claim=Claims.GetClaim(queueItem.ClaimNum);
			Provider provBill=Providers.GetProv(claim.ProvBill);
			Patient pat=Patients.GetPat(claim.PatNum);
			InsPlan insplan=InsPlans.GetPlan(claim.PlanNum,new List<InsPlan>());
			InsSub insSub=InsSubs.GetSub(claim.InsSubNum,new List<InsSub>());
			Carrier carrier=Carriers.GetCarrier(insplan.CarrierNum);
			List<ClaimProc> claimProcList=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcsForClaim=ClaimProcs.GetForSendClaim(claimProcList,claim.ClaimNum);
			List<Procedure> procList=Procedures.Refresh(claim.PatNum);
			Procedure proc;
			ProcedureCode procCode;
			//ProcedureCode procCode;
			for(int i=0;i<claimProcsForClaim.Count;i++) {
				//claimProcsForClaim already excludes any claimprocs with ProcNum=0, so no payments etc.
				proc=Procedures.GetProcFromList(procList,claimProcsForClaim[i].ProcNum);
				//procCode=Pro
				strb.Append(provBill.SSN+t);//110
				strb.Append(provBill.MedicaidID+t);//111
				strb.Append(t);//112
				strb.Append(t);//118
				strb.Append(pat.SSN+t);//203/403
				strb.Append(carrier.CarrierName+t);//carrier name?
				strb.Append(insSub.SubscriberID+t);
				strb.Append(pat.PatNum.ToString()+t);
				strb.Append(pat.Birthdate.ToString("dd-MM-yyyy")+t);
				if(pat.Gender==PatientGender.Female) {
					strb.Append("2"+t);//"V"+t);
				}
				else {
					strb.Append("1"+t);//M"+t);
				}
				strb.Append("1"+t);
				strb.Append(DutchLName(pat.LName)+t);//last name without prefix
				strb.Append(DutchLNamePrefix(pat.LName)+t);//prefix
				strb.Append("2"+t);
				strb.Append(DutchInitials(pat)+t);//215. initials
				strb.Append(pat.Zip+t);
				strb.Append(DutchAddressNumber(pat.Address)+t);//219 house number.  Already validated.
				strb.Append(t);
				strb.Append(proc.ProcDate.ToString("dd-MM-yyyy")+t);//procDate
				procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				string strProcCode=procCode.ProcCode;
				if(strProcCode.EndsWith("00")) {//ending with 00 indicates it's a lab code.
					strb.Append("02"+t);
				}
				else {
					strb.Append("01"+t);//409. Procedure code (01) or lab costs (02)
				}
				strb.Append(t);
				strb.Append(t);
				strb.Append(strProcCode+t);
				strb.Append(GetUL(proc,procCode)+t);//414. U/L.
				strb.Append(Tooth.ToInternat(proc.ToothNum)+t);
				strb.Append(Tooth.SurfTidyForClaims(proc.Surf,proc.ToothNum)+t);//needs validation
				strb.Append(t);
				if(claim.AccidentRelated=="") {//not accident
					strb.Append("N"+t);
				}
				else {
					strb.Append("J"+t);
				}
				strb.Append(pat.SSN+t);
				strb.Append(t);
				strb.Append(t);
				strb.Append(t);
				strb.Append(proc.ProcFee.ToString("F")+t);
				strb.Append("1"+t);
				strb.Append(proc.ProcFee.ToString("F")+t);
				strb.Append(t);
				strb.Append(t);
				strb.Append(proc.ProcFee.ToString("F")+t);
				strb.Append(t);
				strb.Append(t);
				strb.Append("\r\n");
			}
			string saveFolder=clearinghouseClin.ExportPath;
			if(!Directory.Exists(saveFolder)) {
				MessageBox.Show(saveFolder+" "+Lans.g("Dutch","not found."));
				return false;
			}
			string saveFile=ODFileUtils.CombinePaths(saveFolder,"claims"+claim.ClaimNum.ToString()+".txt");
			File.WriteAllText(saveFile,strb.ToString());
			//MessageBox.Show(strb.ToString());
			return true;
		}

		///<summary>Returns only the portion of the LName not including the prefix</summary>
		public static string DutchLName(string fullLName) {
			//eg. Berg, van der
			if(!fullLName.Contains(",")) {
				return fullLName;
			}
			return fullLName.Substring(0,fullLName.IndexOf(","));
		}

		///<summary>Returns only the prefix of the LName</summary>
		public static string DutchLNamePrefix(string fullLName) {
			//eg. Berg, van der
			if(!fullLName.Contains(",")) {
				return "";
			}
			if(fullLName.EndsWith(",")) {
				return "";
			}
			string retVal=fullLName.Substring(fullLName.IndexOf(",")+1);// van der
			retVal.TrimStart(' ');
			return retVal;
		}

		public static string DutchInitials(Patient pat) {
			string[] arrayFirstNames=pat.FName.Split(new char[]{'-'},StringSplitOptions.RemoveEmptyEntries);
			string retVal="";
			for(int i=0;i<arrayFirstNames.Length;i++) {
				retVal+=arrayFirstNames[i].Substring(0,1).ToUpper()+".";
			}
			if(pat.MiddleI!="") {
				retVal+=pat.MiddleI.Substring(0,1).ToUpper()+".";
			}
			return retVal;
		}

		///<summary>Returns only the house number portion of the address.  Expects a number.  Already validated that some text came before the number.</summary>
		public static string DutchAddressNumber(string address) {
			Match match=Regex.Match(address,"[0-9]+");//find the first group of numbers
			return match.Value;
		}

		/// <summary>Returns either 0,1,or 2</summary>
		public static string GetUL(Procedure proc,ProcedureCode procCode) {
			if(procCode.TreatArea==TreatmentArea.Arch) {
				if(proc.Surf=="U") {
					return "1";
				}
				if(proc.Surf=="L") {
					return "2";
				}
				return "0";//should never happen
			}
			else {
				return "0";
			}
		}

		///<summary>Returns a string describing all missing data on this claim.  Claim will not be allowed to be sent electronically unless this string comes back empty.  There is also an out parameter containing any warnings.  Warnings will not block sending.</summary>
		public static void GetMissingData(ClaimSendQueueItem queueItem){//,out string warning) {
			StringBuilder strb=new StringBuilder();
			string warning="";
			Claim claim=Claims.GetClaim(queueItem.ClaimNum);
			Patient pat=Patients.GetPat(claim.PatNum);
			if(!Regex.IsMatch(pat.Address,@"^[a-zA-Z ]+[0-9]+")) {//format must be streetname, then some numbers, then anything else.
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Patient address format");
			}
			//return strb.ToString();
			queueItem.MissingData=strb.ToString();
			queueItem.Warnings=warning;
		}

	}
}
