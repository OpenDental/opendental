using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class TreatmentPlanModules {

		///<summary>Gets a good chunk of the data used in the TP Module.</summary>
		public static TPModuleData GetModuleData(long patNum,bool doMakeSecLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<TPModuleData>(MethodBase.GetCurrentMethod(),patNum,doMakeSecLog);
			}
			TPModuleData tpData=new TPModuleData();
			tpData.Fam=Patients.GetFamily(patNum);
			tpData.Pat=tpData.Fam.GetPatient(patNum);
			tpData.PatPlanList=PatPlans.Refresh(patNum);
			if(!PatPlans.IsPatPlanListValid(tpData.PatPlanList)) {
				//PatPlans had invalid references and need to be refreshed.
				tpData.PatPlanList=PatPlans.Refresh(patNum);
			}
			tpData.SubList=InsSubs.RefreshForFam(tpData.Fam);
			tpData.InsPlanList=InsPlans.RefreshForSubList(tpData.SubList);
			tpData.BenefitList=Benefits.Refresh(tpData.PatPlanList,tpData.SubList);
			tpData.ClaimList=Claims.Refresh(tpData.Pat.PatNum);
			tpData.HistList=ClaimProcs.GetHistList(tpData.Pat.PatNum,tpData.BenefitList,tpData.PatPlanList,tpData.InsPlanList,DateTime.Today,
				tpData.SubList);
			tpData.ListSubstLinks=SubstitutionLinks.GetAllForPlans(tpData.InsPlanList);
			tpData.DiscountPlanSub=DiscountPlanSubs.GetSubForPat(patNum);
			tpData.DiscountPlan=DiscountPlans.GetForPats(new List<long>{ patNum }).FirstOrDefault();
			TreatPlanType tpTypeCur=(tpData.DiscountPlanSub==null?TreatPlanType.Insurance:TreatPlanType.Discount);
			TreatPlans.AuditPlans(patNum,tpTypeCur);
			tpData.ListProcedures=Procedures.Refresh(patNum);
			tpData.ListTreatPlans=TreatPlans.GetAllForPat(patNum);
			tpData.ArrProcTPs=ProcTPs.Refresh(patNum);
			if(doMakeSecLog) {
				SecurityLogs.MakeLogEntry(Permissions.TPModule,patNum,"");
			}
			return tpData;
		}

		///<summary>Gets most of the data needed to load the active treatment plan.</summary>
		///<param name="doFillHistList">If false, then LoadActiveTPData.HistList will be null.</param>
		public static LoadActiveTPData GetLoadActiveTpData(Patient pat,long treatPlanNum,List<Benefit> listBenefits,List<PatPlan> listPatPlans,
			List<InsPlan> listInsPlans,DateTime dateTimeTP,List<InsSub> listInsSubs,bool doFillHistList,bool isTreatPlanSortByTooth,
			List<SubstitutionLink> listSubstLinks) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<LoadActiveTPData>(MethodBase.GetCurrentMethod(),pat,treatPlanNum,listBenefits,listPatPlans,listInsPlans,dateTimeTP,
					listInsSubs,doFillHistList,isTreatPlanSortByTooth,listSubstLinks);
			}
			LoadActiveTPData data=new LoadActiveTPData();
			data.ListTreatPlanAttaches=TreatPlanAttaches.GetAllForTreatPlan(treatPlanNum);
			List<Procedure> listProcs=Procedures.GetManyProc(data.ListTreatPlanAttaches.Select(x=>x.ProcNum).ToList(),false);
			data.listProcForTP=Procedures.SortListByTreatPlanPriority(listProcs.FindAll(x => x.ProcStatus==ProcStat.TP || x.ProcStatus==ProcStat.TPi)
				,isTreatPlanSortByTooth,data.ListTreatPlanAttaches).ToList();
			//One thing to watch out for here is that we must be absolutely sure to include all claimprocs for the procedures listed,
			//regardless of status.  Needed for Procedures.ComputeEstimates.  This should be fine.
			data.ClaimProcList=ClaimProcs.RefreshForTP(pat.PatNum);
			if(doFillHistList) {
				data.HistList=ClaimProcs.GetHistList(pat.PatNum,listBenefits,listPatPlans,listInsPlans,-1,dateTimeTP,listInsSubs);
			}
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			foreach(Procedure procedure in listProcs) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(procedure.CodeNum));
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum);
			data.ListFees=Fees.GetListFromObjects(listProcedureCodes,listProcs.Select(x=>x.MedicalCode).ToList(),listProcs.Select(x=>x.ProvNum).ToList(),
				pat.PriProv,pat.SecProv,pat.FeeSched,listInsPlans,listProcs.Select(x=>x.ClinicNum).ToList(),null,//appts can be null because provs already set
				listSubstLinks,discountPlanNum);
			data.BlueBookEstimateData=new BlueBookEstimateData(listInsPlans,listInsSubs,listPatPlans,listProcs,listSubstLinks);
			return data;
		}
	}

	[Serializable]
	public class TPModuleData {
		public Family Fam;
		public Patient Pat;
		public List<InsSub> SubList;
		public List<InsPlan> InsPlanList;
		public List<PatPlan> PatPlanList;
		public List<Benefit> BenefitList;
		public List<Claim> ClaimList;
		public List<ClaimProcHist> HistList;
		public List<SubstitutionLink> ListSubstLinks;
		public List<Procedure> ListProcedures;
		public List<TreatPlan> ListTreatPlans;
		public ProcTP[]	ArrProcTPs;
		public DiscountPlan DiscountPlan;
		public DiscountPlanSub DiscountPlanSub;
	}

	[Serializable]
	public class LoadActiveTPData {
		public List<TreatPlanAttach> ListTreatPlanAttaches;
		public List<Procedure> listProcForTP;
		public List<ClaimProc> ClaimProcList;
		public List<ClaimProcHist> HistList;
		//public List<SubstitutionLink> ListSubstLinks;//already handled above, in TPModuleData
		///<summary>Includes fees for all procedurecodes on this plan. Includes fee schedules for providers on this plan, insurance, etc.  Very concise list.</summary>
		public List<Fee> ListFees;
		public BlueBookEstimateData BlueBookEstimateData;
	}

	public class TpRow {
		public string Done;
		public string Priority;
		public string Tth;
		public string Surf;
		public string Code;
		public string Description;
		public string Prognosis;
		public string Dx;
		public string ProcAbbr;
		public decimal Fee;
		public decimal PriIns;
		public decimal SecIns;
		public decimal Discount;
		public decimal Pat;
		public decimal FeeAllowed;
		public System.Drawing.Color ColorText;
		public System.Drawing.Color ColorLborder;
		public bool Bold;
		public object Tag;
		public decimal TaxEst;
		public long ProvNum;
		public DateTime DateTP;
		public long ClinicNum;
		public string Appt;
		public decimal CatPercUCR;
	}
}
