using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestsCore {
	public class ProcTpT {

		///<summary>Returns a list of ProcTPs that are created for a TreatPlan with a status of "Saved".</summary>
		public static List<ProcTP> CreateProcTpsForSavedTreatPlan(long treatPlanNum,List<Procedure> listProcedures) {
			for(int i=0;i<listProcedures.Count;i++) {
				ProcTP procTp=new ProcTP();
				procTp.TreatPlanNum=treatPlanNum;
				procTp.PatNum=listProcedures[i].PatNum;
				procTp.ProcNumOrig=listProcedures[i].ProcNum;
				procTp.ItemOrder=i;
				procTp.Priority=0;
				procTp.ToothNumTP=Tooth.Display(listProcedures[i].ToothNum);
				ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
				if(procCode.TreatArea==TreatmentArea.Surf) {
					procTp.Surf=Tooth.SurfTidyFromDbToDisplay(listProcedures[i].Surf,listProcedures[i].ToothNum);
				}
				else if(procCode.TreatArea==TreatmentArea.Sextant) {
					procTp.Surf=Tooth.GetSextant(listProcedures[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					procTp.Surf=listProcedures[i].Surf;//for UR, L, etc.
				}
				procTp.ProcCode=ProcedureCodes.GetStringProcCode(listProcedures[i].CodeNum);
				procTp.Descript=procCode.Descript;
				procTp.FeeAmt=listProcedures[i].ProcFee;
				procTp.PriInsAmt=80.00;
				procTp.SecInsAmt=20.00;
				procTp.Discount=listProcedures[i].Discount;
				procTp.PatAmt=15.75;
				procTp.Prognosis="Prognosis Text";
				procTp.Dx=listProcedures[i].Dx.ToString();
				procTp.ProcAbbr=procCode.AbbrDesc;
				procTp.FeeAllowed=80.00;
				procTp.TaxAmt=listProcedures[i].TaxAmt;
				procTp.ProvNum=listProcedures[i].ProvNum;
				procTp.DateTP=listProcedures[i].DateTP;
				procTp.ClinicNum=listProcedures[i].ClinicNum;
				procTp.CatPercUCR=50.00;
				ProcTPs.InsertOrUpdate(procTp,true);
			}
			return ProcTPs.RefreshForTP(treatPlanNum);
		}

		///<summary>Deletes everything from the ProcTP table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearProcTpTable() {
			string command="DELETE FROM proctp";
			DataCore.NonQ(command);
		}
	}
}
