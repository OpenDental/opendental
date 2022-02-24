using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness {
	public class RpPresentedTreatmentProduction {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetPresentedTreatmentProductionTable(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,
			bool hasAllClinics,bool hasClinicsEnabled,bool isPresenter,bool isFirstPresenter,List<long> listUserNums,bool isDetailed) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,hasAllClinics
					,hasClinicsEnabled,isPresenter,isFirstPresenter,listUserNums,isDetailed);
			}
			List<TreatPlanPresenterEntry> listTreatPlanPresenterEntries=new List<TreatPlanPresenterEntry>();
			listTreatPlanPresenterEntries=GetListTreatPlanPresenterEntries(listClinicNums,isFirstPresenter,isPresenter,dateStart,dateEnd);
			//-------------------------------------------------------Run Detailed
			if(isDetailed) {
				listTreatPlanPresenterEntries=listTreatPlanPresenterEntries
				.Where(x => listUserNums.Contains(x.UserNumPresenter))
				.OrderBy(x => x.Presenter).ThenBy(x => x.DatePresented).ToList();
				DataTable tableReport=new DataTable();
				tableReport.Columns.Add("Presenter");
				tableReport.Columns.Add("DatePresented",typeof(DateTime));
				tableReport.Columns.Add("DateCompleted",typeof(DateTime));
				tableReport.Columns.Add("Descript");
				tableReport.Columns.Add("GrossProd",typeof(double));
				tableReport.Columns.Add("WriteOffs",typeof(double));
				tableReport.Columns.Add("Adjustments",typeof(double));
				tableReport.Columns.Add("NetProduction",typeof(double));
				foreach(TreatPlanPresenterEntry presenterEntry in listTreatPlanPresenterEntries) {
					DataRow row = tableReport.NewRow();
					row["Presenter"] = presenterEntry.Presenter=="" ? "None" : presenterEntry.Presenter;
					row["DatePresented"] = presenterEntry.DatePresented;
					row["DateCompleted"] = presenterEntry.DateCompleted;
					row["Descript"] = presenterEntry.ProcDescript;
					row["GrossProd"] = presenterEntry.GrossProd;
					row["WriteOffs"] = presenterEntry.WriteOffs;
					row["Adjustments"] = presenterEntry.Adjustments;
					row["NetProduction"] = presenterEntry.NetProd;
					tableReport.Rows.Add(row);

				}
				return tableReport;
			}
			//------------------------------------------------------------

			//-----------------------------------------------------------Run Totals
			else {
				listTreatPlanPresenterEntries=listTreatPlanPresenterEntries
								.Where(x => listUserNums.Select(y => y).Contains(x.UserNumPresenter))
								.OrderBy(x => x.Presenter).ThenBy(x => x.DatePresented).ToList();
				DataTable table = new DataTable();
				table.Columns.Add("Presenter");
				table.Columns.Add("# of Procs");
				table.Columns.Add("GrossProd",typeof(double));
				table.Columns.Add("WriteOffs",typeof(double));
				table.Columns.Add("Adjustments",typeof(double));
				table.Columns.Add("NetProduction",typeof(double));
				listTreatPlanPresenterEntries.GroupBy(x => x.Presenter).ToList().ForEach(x =>
				{
					DataRow row = table.NewRow();
					row["Presenter"] = x.Select(y => y.Presenter).First() == "" ? "None" : x.Select(y => y.Presenter).First();
					row["# of Procs"] = x.Count();
					row["GrossProd"] = x.Sum(y => y.GrossProd);
					row["WriteOffs"] = x.Sum(y => y.WriteOffs);
					row["Adjustments"] = x.Sum(y => y.Adjustments);
					row["NetProduction"] = x.Sum(y => y.NetProd);
					table.Rows.Add(row);
				});
				return table;
			}

		}

		private static List<TreatPlanPresenterEntry> GetListTreatPlanPresenterEntries(List<long> listClinicNums, bool isFirstPresenter,bool isPresenter
			,DateTime dateStart,DateTime dateEnd) 
		{
			//No need to check RemotingRole; private method.
			List<Procedure> listProcsComplete=ReportsComplex.RunFuncOnReportServer(() => Procedures.GetCompletedForDateRangeLimited(dateStart,dateEnd,listClinicNums));
			List<ProcTP> listProcTPs=ReportsComplex.RunFuncOnReportServer(() => ProcTPs.GetForProcs(listProcsComplete.Select(x => x.ProcNum).ToList()));
			List<Procedure> listTreatPlanProcs=listProcsComplete.Where(x => listProcTPs.Select(y => y.ProcNumOrig).Contains(x.ProcNum)).ToList();
			List<TreatPlan> listSavedTreatPlans=ReportsComplex.RunFuncOnReportServer(() => TreatPlans.GetFromProcTPs(listProcTPs)); // attached proctps to treatment plans.
			List<ClaimProc> listClaimProcs=ReportsComplex.RunFuncOnReportServer(() => ClaimProcs.GetForProcsLimited(listTreatPlanProcs.Select(x => x.ProcNum).ToList(),
				ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapComplete,ClaimProcStatus.NotReceived));
			List<Adjustment> listAdjustments=ReportsComplex.RunFuncOnReportServer(() => Adjustments.GetForProcs(listTreatPlanProcs.Select(x => x.ProcNum).ToList()));
			List<Userod> listUserods=ReportsComplex.RunFuncOnReportServer(() => Userods.GetAll());
			List<TreatPlanPresenterEntry> listTreatPlanPresenterEntries=new List<TreatPlanPresenterEntry>();
			List<ProcedureCode> listProcCodes=ProcedureCodes.GetCodesForCodeNums(listTreatPlanProcs.Select(x => x.CodeNum).ToList());
			foreach(Procedure procCur in listTreatPlanProcs) {
				double grossProd=procCur.ProcFeeTotal;
				double writeOffs=listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum)
						.Where(x => x.Status==ClaimProcStatus.CapComplete)
						.Sum(x => x.WriteOff);
				grossProd-=writeOffs;
				writeOffs = listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum)
					.Where(x => x.Status == ClaimProcStatus.NotReceived
						|| x.Status == ClaimProcStatus.Received
						|| x.Status == ClaimProcStatus.Supplemental)
					.Sum(x => x.WriteOff);
				double adjustments = listAdjustments.Where(x => x.ProcNum == procCur.ProcNum).Sum(x => x.AdjAmt);
				double netProd = grossProd - writeOffs + adjustments;
				TreatPlan treatPlanCur;
				if(isFirstPresenter) {
					treatPlanCur = listSavedTreatPlans.Where(x => x.ListProcTPs.Any(y => y.ProcNumOrig==procCur.ProcNum)).OrderBy(x => x.DateTP).First();
				}
				else { //radioLastPresented
					treatPlanCur = listSavedTreatPlans.Where(x => x.ListProcTPs.Any(y => y.ProcNumOrig==procCur.ProcNum)).OrderByDescending(x => x.DateTP).First();
				}
				Userod userPresenter;
				if(isPresenter) {
					userPresenter = listUserods.FirstOrDefault(x => x.UserNum == treatPlanCur.UserNumPresenter);
				}
				else { //radioEntryUser
					userPresenter = listUserods.FirstOrDefault(x => x.UserNum == treatPlanCur.SecUserNumEntry);
				}
				ProcedureCode procCode = listProcCodes.First(x => x.CodeNum == procCur.CodeNum);
				listTreatPlanPresenterEntries.Add(new TreatPlanPresenterEntry()
				{
					Presenter=userPresenter==null?"":userPresenter.UserName,
					DatePresented=treatPlanCur.DateTP,
					DateCompleted=procCur.ProcDate,
					ProcDescript=procCode.Descript,
					GrossProd=grossProd,
					Adjustments=adjustments,
					WriteOffs=writeOffs,
					NetProd=netProd,
					UserNumPresenter=userPresenter==null?0:userPresenter.UserNum,
					PresentedClinic=procCur.ClinicNum
				});
			}
			return listTreatPlanPresenterEntries;
		}

		///<summary>Just a handy little container class to keep treat plan presenter entries.</summary>
		private class TreatPlanPresenterEntry {
			public string Presenter;
			public DateTime DatePresented;
			public DateTime DateCompleted;
			public string ProcDescript;
			public double GrossProd;
			public double WriteOffs;
			public double Adjustments;
			public double NetProd;
			public long UserNumPresenter;
			public long PresentedClinic;
		}


	}
}
	
