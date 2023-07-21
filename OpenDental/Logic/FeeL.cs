using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	///<summary></summary>
	public class FeeL {

		///<summary></summary>
		public static void ImportFees(string fileName,long feeSchedNum,long clinicNum,long provNum) {
			List<Fee> listFees=Fees.GetListExact(feeSchedNum,clinicNum,provNum);
			List<Fee> listFeesAdded=new List<Fee>();
			string[] fields;
			int counter=0;
			int lineCount=File.ReadAllLines(fileName).Length;//quick and dirty
			using StreamReader streamReader=new StreamReader(fileName);
			string line=streamReader.ReadLine();
			while(line!=null) {
				fields=line.Split(new string[1] {"\t"},StringSplitOptions.None);
				if(fields.Length<2){// && fields[1]!=""){//we no longer skip blank fees
					line=streamReader.ReadLine();
					continue;
				}
				long codeNum = ProcedureCodes.GetCodeNum(fields[0]);
				if(codeNum==0){
					line=streamReader.ReadLine();
					continue;
				}
				Fee feeFound=listFeesAdded.Find(x => x.CodeNum==codeNum);
				if(feeFound!=null) {//if we have added this fee from the file before we will skip it
					line=streamReader.ReadLine();
					continue;
				}
				Fee fee=Fees.GetFee(codeNum,feeSchedNum,clinicNum,provNum,listFees);
				string feeOldStr="";
				DateTime datePrevious=DateTime.MinValue;
				if(fee!=null) {
					listFeesAdded.Add(fee);
					feeOldStr="Old Fee: "+fee.Amount.ToString("c")+", ";
					datePrevious=fee.SecDateTEdit;
				}
				if(fields[1]=="") {//an empty entry will delete an existing fee, but not insert a blank override
					if(fee==null){//nothing to do
						//counter++;
						//line=sr.ReadLine();
						//continue;
					}
					else{
						//doesn't matter if the existing fee is an override or not.
						Fees.Delete(fee);
						SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,"Procedure: "+fields[0]+", "
							+feeOldStr
							//+", Deleted Fee: "+fee.Amount.ToString("c")+", "
							+"Fee Schedule: "+FeeScheds.GetDescription(feeSchedNum)+". "
							+"Fee deleted using the Import button in the Fee Tools window.",codeNum,
							DateTime.MinValue);
						SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,"Fee deleted",fee.FeeNum,datePrevious);
					}
				}
				else {//value found
					if(fee==null){//no current fee
						fee=new Fee();
						fee.Amount=PIn.Double(fields[1]);
						fee.FeeSched=feeSchedNum;
						fee.CodeNum=codeNum;
						fee.ClinicNum=clinicNum;//Either 0 because you're importing on an HQ schedule or local clinic because the feesched is localizable.
						fee.ProvNum=provNum;
						listFeesAdded.Add(fee);
						Fees.Insert(fee);
					}
					else{
						fee.Amount=PIn.Double(fields[1]);
						Fees.Update(fee);
					}
					SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,"Procedure: "+fields[0]+", "
						+feeOldStr
						+", New Fee: "+fee.Amount.ToString("c")+", "
						+"Fee Schedule: "+FeeScheds.GetDescription(feeSchedNum)+". "
						+"Fee changed using the Import button in the Fee Tools window.",codeNum,
						DateTime.MinValue);
					SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,"Fee changed",fee.FeeNum,datePrevious);
				}
				double percent=(double)counter*100d/(double)lineCount;
				counter++;
				ProgressBarEvent.Fire(ODEventType.ProgressBar,new ProgressBarHelper(
					"Importing fees...",((int)percent).ToString(),blockValue:(int)percent,progressStyle:ProgBarStyle.Blocks));
				line=streamReader.ReadLine();
			}
		}

		///<summary>Returns true if current user can edit the given feeSched, otherwise false.
		///Shows a MessageBox if user is not allowed to edit.</summary>
		public static bool CanEditFee(FeeSched feeSched,long provNum,long clinicNum) {
			//User doesn't have permission
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit)) {
				return false;
			}
			//Check if a provider fee schedule is selected and if the current user has permissions to edit provider fees.
			if(provNum!=0 && !Security.IsAuthorized(Permissions.ProviderFeeEdit)) {
				return false;
			}
			//Make sure the user has permission to edit the clinic of the fee schedule being edited.
			if(!Security.CurUser.ClinicIsRestricted || clinicNum==Clinics.ClinicNum) {
				return true;
			}
			if(clinicNum==0 && feeSched!=null && feeSched.IsGlobal) {
				//Allow restricted users to edit the default Fee when the FeeSched is global.
				//Intentionally blank so logic is more readable.
				return true;
			}
			MessageBox.Show(Lans.g("Fee","User is clinic restricted and")+" "+feeSched.Description+" "+Lans.g("Fee","is not global."));
			return false;
		}

		///<summary>The list of ClaimProcs passed in must all be for the same insurance plan.
		///Updates fee schedules with the AllowedOverrides from the list of ClaimProcs.</summary>
		public static void SaveAllowedFeesFromClaimPayment(List<ClaimProc> listClaimProcs,List<InsPlan> listInsPlans) {
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit,true) && !Security.IsAuthorized(Permissions.AllowFeeEditWhileReceivingClaim,true)) {
				return;
			}
			if(listClaimProcs.IsNullOrEmpty()) {
				return;
			}
			//if no allowed fee schedule for plan, then nothing to do
			InsPlan insPlan=InsPlans.GetPlan(listClaimProcs[0].PlanNum,listInsPlans);
			long allowedFeeSchedNum=insPlan.AllowedFeeSched;
			if(allowedFeeSchedNum==0) {//no allowed fee sched
				return;
			}
			//if no allowed fees entered, then nothing to do 
			bool allowedFeesEntered=listClaimProcs.Any(x=> x.AllowedOverride!=-1);
			if(!allowedFeesEntered) {
				return;
			}
			//ask user if they want to save the fees
			if(!MsgBox.Show("FormClaimPayTotal",MsgBoxButtons.YesNo,"Save the allowed amounts to the allowed fee schedule?")) {
				return;
			}
			//select the feeSchedule
			if(FeeScheds.GetIsHidden(allowedFeeSchedNum)) {
				MsgBox.Show("FormClaimPayTotal","Allowed fee schedule is hidden, so no changes can be made.");
				return;
			}
			List<Procedure> listProcedures=Procedures.GetProcsFromClaimProcs(listClaimProcs);
			for(int i=0;i<listClaimProcs.Count;i++) {
				if(listClaimProcs[i].AllowedOverride==-1) {
					continue;
				}
				Procedure procedure=Procedures.GetProcFromList(listProcedures,listClaimProcs[i].ProcNum);
				long codeNum=procedure.CodeNum;
				//ProcNum not found. ClaimProc may be an as total payment.
				if(codeNum==0) {
					continue;
				}
				Fee fee=Fees.GetFee(codeNum,allowedFeeSchedNum,procedure.ClinicNum,procedure.ProvNum);
				DateTime datePrevious=DateTime.MinValue;
				if(fee==null) {
					fee=new Fee();
					fee.FeeSched=allowedFeeSchedNum;
					fee.CodeNum=codeNum;
					fee.ClinicNum=(FeeScheds.GetFirst(x => x.FeeSchedNum==allowedFeeSchedNum).IsGlobal) ? 0 : procedure.ClinicNum;
					fee.ProvNum=(FeeScheds.GetFirst(x => x.FeeSchedNum==allowedFeeSchedNum).IsGlobal) ? 0 : procedure.ProvNum;
					fee.Amount=listClaimProcs[i].AllowedOverride;
					Fees.Insert(fee);
				}
				else {
					fee.Amount=listClaimProcs[i].AllowedOverride;
					datePrevious=fee.SecDateTEdit;
					Fees.Update(fee);
				}
				SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,Lan.g("FormClaimPayTotal","Procedure")+": "+ProcedureCodes.GetStringProcCode(fee.CodeNum)
					+", "+Lan.g("FormClaimPayTotal","Fee")+": "+fee.Amount.ToString("c")+", "+Lan.g("FormClaimPayTotal","Fee Schedule")+" "+FeeScheds.GetDescription(fee.FeeSched)
					+". "+Lan.g("FormClaimPayTotal","Automatic change to allowed fee in Enter Payment window.  Confirmed by user."),fee.CodeNum,DateTime.MinValue);
				SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,Lan.g("FormClaimPayTotal","Fee Updated"),fee.FeeNum,datePrevious);
			}
		}

	}
}