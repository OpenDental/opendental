using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	public class RpTreatPlanPresentationStatistics {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetTreatPlanPresentationStatistics(DateTime dateStart,DateTime dateEnd,bool isFirstPresented,bool hasAllClinics
			,bool hasClinicsEnabled,bool isPresenter,bool isGross,bool hasAllUsers,List<long> listUserNums,List<long> listClinicNums) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,isFirstPresented,hasAllClinics,hasClinicsEnabled,isPresenter,isGross
					,hasAllUsers,listUserNums,listClinicNums);
			}
			List<ProcTP> listProcTPsAll=ReportsComplex.RunFuncOnReportServer(() => ProcTPs.GetAllLim());
			List<TreatPlan> listSavedTreatPlans=ReportsComplex.RunFuncOnReportServer(() => TreatPlans.GetAllSavedLim());
			List<ProcTpTreatPlan> listProcTPTreatPlans=new List<ProcTpTreatPlan>();
			listProcTPsAll.ForEach(x =>
			{
				listProcTPTreatPlans.Add(new ProcTpTreatPlan()
				{
					TreatPlanCur = listSavedTreatPlans.First(y => y.TreatPlanNum == x.TreatPlanNum),
					ProcTPCur = x
				});
			});
			//get one entry per procedure with their first/last date of presentation based on radio buttons.
			if(isFirstPresented) {
				listProcTPTreatPlans = listProcTPTreatPlans
					.OrderBy(x => x.ProcTPCur.ProcNumOrig)
					.ThenBy(x => x.TreatPlanCur.DateTP)
					.ThenBy(x => x.TreatPlanCur.TreatPlanNum)
					.GroupBy(x => x.ProcTPCur.ProcNumOrig)
					.Select(x => x.First())
					.ToList();
			}
			else {
				listProcTPTreatPlans = listProcTPTreatPlans
					.OrderBy(x => x.ProcTPCur.ProcNumOrig)
					.ThenByDescending(x => x.TreatPlanCur.DateTP)
					.ThenBy(x => x.TreatPlanCur.TreatPlanNum)
					.GroupBy(x => x.ProcTPCur.ProcNumOrig)
					.Select(x => x.First())
					.ToList();
			}
			//get rid of any entries that are outside the range selected.
			listProcTPTreatPlans=listProcTPTreatPlans.Where(x => x.TreatPlanCur.DateTP.Date >= dateStart
				&& x.TreatPlanCur.DateTP.Date <= dateEnd).ToList();
			//Get the associated procedures, claimprocs, adjustments, users, appointments.
			List<Procedure> listProcsForTreatPlans = ReportsComplex.RunFuncOnReportServer(() => 
				Procedures.GetForProcTPs(listProcTPTreatPlans.Select(x => x.ProcTPCur).ToList(),ProcStat.C,ProcStat.TP));
			if(hasClinicsEnabled && !hasAllClinics) {
				listProcsForTreatPlans=
					listProcsForTreatPlans.FindAll(x => listClinicNums.Contains(x.ClinicNum));
			}
			List<ClaimProc> listClaimProcs=ReportsComplex.RunFuncOnReportServer(() => ClaimProcs.GetForProcsLimited(listProcsForTreatPlans.Select(x => x.ProcNum).ToList(),
				ClaimProcStatus.CapComplete,ClaimProcStatus.NotReceived,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.Estimate));
			List<Adjustment> listAdjustments=ReportsComplex.RunFuncOnReportServer(() => Adjustments.GetForProcs(listProcsForTreatPlans.Select(x => x.ProcNum).ToList()));
			List<Userod> listUserods=ReportsComplex.RunFuncOnReportServer(() => Userods.GetAll());
			List<TreatPlanPresenterEntry> listTreatPlanPresenterEntries=new List<TreatPlanPresenterEntry>();
			List<ProcedureCode> listProcCodes=ReportsComplex.RunFuncOnReportServer(() => ProcedureCodes.GetCodesForCodeNums(listProcsForTreatPlans.Select(x => x.CodeNum).ToList()));
			List<Appointment> listApts=ReportsComplex.RunFuncOnReportServer(() => Appointments.GetMultApts(listProcsForTreatPlans.Select(x => x.AptNum).ToList()));
			double amt=listProcsForTreatPlans.Sum(x => x.ProcFee);
			foreach(Procedure procCur in listProcsForTreatPlans) {
				double grossProd=procCur.ProcFeeTotal;
				double writeOffs=listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum)
						.Where(x => x.Status == ClaimProcStatus.CapComplete)
						.Sum(x => x.WriteOff);
				grossProd-=writeOffs;
				if(procCur.ProcStatus == ProcStat.C) {
					writeOffs += listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum)
						.Where(x => ListTools.In(x.Status,ClaimProcStatus.NotReceived,ClaimProcStatus.Received,ClaimProcStatus.Supplemental))
						.Sum(x => x.WriteOff);
				}
				else {
					foreach(ClaimProc claimProcCur in listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum).Where(x => x.Status == ClaimProcStatus.Estimate)) {
						if(claimProcCur.WriteOffEstOverride == -1) {
							if(claimProcCur.WriteOffEst!=-1) {
								writeOffs+=claimProcCur.WriteOffEst;
							}
						}
						else {
							writeOffs+=claimProcCur.WriteOffEstOverride;
						}
					}
					//writeOffs += listClaimProcs.Where(x => x.ProcNum == procCur.ProcNum)
					//	.Where(x => x.Status == ClaimProcStatus.Estimate)
					//	.Sum(x => x.WriteOffEstOverride == -1 ? (x.WriteOffEst == -1 ? 0 : x.WriteOffEst) : x.WriteOffEstOverride); //Allen won't let me commit this nested ternary :(
				}
				double adjustments=listAdjustments.Where(x => x.ProcNum == procCur.ProcNum).Sum(x => x.AdjAmt);
				double netProd=grossProd-writeOffs+adjustments;
				TreatPlan treatPlanCur=listProcTPTreatPlans.Where(x => x.ProcTPCur.ProcNumOrig == procCur.ProcNum).First().TreatPlanCur;
				Userod userPresenter;
				if(isPresenter) {
					userPresenter=listUserods.FirstOrDefault(x => x.UserNum == treatPlanCur.UserNumPresenter);
				}
				else { //radioEntryUser
					userPresenter=listUserods.FirstOrDefault(x => x.UserNum == treatPlanCur.SecUserNumEntry);
				}
				ProcedureCode procCode=listProcCodes.First(x => x.CodeNum == procCur.CodeNum);
				Appointment aptCur=listApts.FirstOrDefault(x => x.AptNum == procCur.AptNum);
				listTreatPlanPresenterEntries.Add(new TreatPlanPresenterEntry()
				{
					Presenter=userPresenter==null ? "" : userPresenter.UserName,
					DatePresented=treatPlanCur.DateTP,
					DateCompleted=procCur.ProcDate,
					ProcDescript=procCode.Descript,
					GrossProd=grossProd,
					Adjustments=adjustments,
					WriteOffs=writeOffs,
					NetProd=netProd,
					UserNumPresenter=userPresenter==null?0:userPresenter.UserNum,
					PresentedClinic=procCur.ClinicNum,
					ProcStatus=procCur.ProcStatus,
					TreatPlanNum=treatPlanCur.TreatPlanNum,
					AptNum=procCur.AptNum,
					AptStatus=aptCur==null?ApptStatus.None:aptCur.AptStatus
				});
			}
			DataTable table=new DataTable();
			table.Columns.Add("Presenter");
			table.Columns.Add("# of Plans");
			table.Columns.Add("# of Procs");
			table.Columns.Add("# of ProcsSched");
			table.Columns.Add("# of ProcsComp");
			if(isGross) {
				table.Columns.Add("GrossTPAmt",typeof(double));
				table.Columns.Add("GrossSchedAmt",typeof(double));
				table.Columns.Add("GrossCompAmt",typeof(double));
			}
			else {
				table.Columns.Add("NetTpAmt",typeof(double));
				table.Columns.Add("NetSchedAmt",typeof(double));
				table.Columns.Add("NetCompAmt",typeof(double));
			}
			if(!hasAllUsers) {
				listTreatPlanPresenterEntries=listTreatPlanPresenterEntries.Where(x => listUserNums.Contains(x.UserNumPresenter)).ToList();
			}
			listTreatPlanPresenterEntries=listTreatPlanPresenterEntries.OrderBy(x => x.Presenter).ToList();
			listTreatPlanPresenterEntries
				.GroupBy(x => x.Presenter).ToList().ForEach(x =>
				{
					DataRow row = table.NewRow();
					row["Presenter"] = x.First().Presenter=="" ? "None" : x.First().Presenter;
					row["# of Plans"] = x.GroupBy(y => y.TreatPlanNum).Count();
					row["# of Procs"] = x.Count();
					row["# of ProcsSched"] = x.Count(y => y.ProcStatus == ProcStat.TP && y.AptNum != 0 && y.AptStatus==ApptStatus.Scheduled);
					row["# of ProcsComp"] = x.Count(y => y.ProcStatus == ProcStat.C);
					if(isGross) {
						row["GrossTpAmt"] = x.Sum(y => y.GrossProd);
						row["GrossSchedAmt"] = x.Where(y => y.ProcStatus == ProcStat.TP && y.AptNum != 0 && y.AptStatus==ApptStatus.Scheduled).Sum(y => y.GrossProd);
						row["GrossCompAmt"] = x.Where(y => y.ProcStatus == ProcStat.C).Sum(y => y.GrossProd);
					}
					else {
						row["NetTpAmt"] = x.Sum(y => y.NetProd);
						row["NetSchedAmt"] = x.Where(y => y.ProcStatus == ProcStat.TP && y.AptNum != 0 && y.AptStatus==ApptStatus.Scheduled).Sum(y => y.NetProd);
						row["NetCompAmt"] = x.Where(y => y.ProcStatus == ProcStat.C).Sum(y => y.NetProd);
					}
					table.Rows.Add(row);
				});

			//DataTable table=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
			return table;
		}

		///<summary>Combines proctps and treatplans into one handy object.</summary>
		private class ProcTpTreatPlan {
			public ProcTP ProcTPCur;
			public TreatPlan TreatPlanCur;
			//public Procedure ProcCur;
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
			public ProcStat ProcStatus;
			public ApptStatus AptStatus;
			public long UserNumPresenter;
			public long PresentedClinic;
			public long TreatPlanNum;
			public long AptNum;
		}
	}
	

}
