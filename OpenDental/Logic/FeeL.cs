using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public class FeeL {

		///<summary></summary>
		public static void ImportFees(string fileName,long feeSchedNum,long clinicNum,long provNum) {
			List<Fee> listFees=Fees.GetListExact(feeSchedNum,clinicNum,provNum);
			string[] fields;
			int counter=0;
			int lineCount=File.ReadAllLines(fileName).Length;//quick and dirty
			using(StreamReader sr=new StreamReader(fileName)) {
				string line=sr.ReadLine();
				while(line!=null) {
					fields=line.Split(new string[1] {"\t"},StringSplitOptions.None);
					if(fields.Length<2){// && fields[1]!=""){//we no longer skip blank fees
						line=sr.ReadLine();
						continue;
					}
					long codeNum = ProcedureCodes.GetCodeNum(fields[0]);
					if(codeNum==0){
						line=sr.ReadLine();
						continue;
					}
					Fee fee=Fees.GetFee(codeNum,feeSchedNum,clinicNum,provNum,listFees);
					string feeOldStr="";
					DateTime datePrevious=DateTime.MinValue;
					if(fee!=null) {
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
					line=sr.ReadLine();
				}
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
			if(Security.CurUser.ClinicIsRestricted && clinicNum!=Clinics.ClinicNum) {
				if(clinicNum==0 && feeSched!=null && feeSched.IsGlobal) {
					//Allow restricted users to edit the default Fee when the FeeSched is global.
					//Intentionally blank so logic in more readable, will return true below.
				}
				else {
					MessageBox.Show(Lans.g("Fee","User is clinic restricted and")+" "+feeSched.Description+" "+Lans.g("Fee","is not global."));
					return false;
				}
			}
			return true;
		}

	}
}