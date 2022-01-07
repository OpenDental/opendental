using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {

	///<summary>Deciding what a procedure fee should be requires quite a bit of data. This class serves as a convenient holder for all that data.
	///</summary>
	public class ProcFeeHelper {
		private long _patNum;
		public Patient Pat;
		public List<Fee> ListFees;
		public List<PatPlan> ListPatPlans;
		public List<InsSub> ListInsSubs;
		public List<InsPlan> ListInsPlans;
		public List<Benefit> ListBenefitsPrimary;

		///<summary>For serialization ONLY.</summary>
		public ProcFeeHelper() {
		}

		public ProcFeeHelper(long patNum) {
			_patNum=patNum;
		}

		public ProcFeeHelper(Patient pat,List<Fee> listFees,List<PatPlan> listPatPlans,List<InsSub> listInsSubs,List<InsPlan> listInsPlans,
			List<Benefit> listBenefits) : this(pat.PatNum) {
			Pat=pat;
			ListFees=listFees;
			ListPatPlans=listPatPlans;
			ListInsSubs=listInsSubs;
			ListInsPlans=listInsPlans;
			ListBenefitsPrimary=listBenefits;
		}

		///<summary>If necessary, fills the data needed for ProcFeeHelper. Does not fill ListFees.</summary>
		public void FillData() {
			if(Pat!=null && ListPatPlans!=null && ListInsSubs!=null && ListInsPlans!=null && ListBenefitsPrimary!=null) {
				return;//all data has already been filled.
			}
			ProcFeeHelper procFeeHelper=GetData(_patNum,this);
			Pat=procFeeHelper.Pat;
			ListPatPlans=procFeeHelper.ListPatPlans;
			ListInsSubs=procFeeHelper.ListInsSubs;
			ListInsPlans=procFeeHelper.ListInsPlans;
			ListBenefitsPrimary=procFeeHelper.ListBenefitsPrimary;
		}

		///<summary>Returns the data needed for ProcFeeHelper. Does not get ListFees.</summary>
		public static ProcFeeHelper GetData(long patNum,ProcFeeHelper procFeeHelper) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				//Not passing procFeeHelper because the null lists will get turned into empty lists which messes things up.
				return Meth.GetObject<ProcFeeHelper>(MethodBase.GetCurrentMethod(),patNum,null);
			}
			procFeeHelper=procFeeHelper??new ProcFeeHelper(patNum);
			procFeeHelper.Pat=procFeeHelper.Pat??Patients.GetPat(patNum);
			procFeeHelper.ListPatPlans=procFeeHelper.ListPatPlans??PatPlans.GetPatPlansForPat(patNum);
			procFeeHelper.ListInsSubs=procFeeHelper.ListInsSubs??InsSubs.GetMany(procFeeHelper.ListPatPlans.Select(x => x.InsSubNum).ToList());
			procFeeHelper.ListInsPlans=procFeeHelper.ListInsPlans??InsPlans.GetPlans(procFeeHelper.ListInsSubs.Select(x => x.PlanNum).ToList());
			if(procFeeHelper.ListPatPlans.Count>0) {
				PatPlan priPatPlan=procFeeHelper.ListPatPlans[0];
				InsSub priInsSub=InsSubs.GetSub(priPatPlan.InsSubNum,procFeeHelper.ListInsSubs);
				InsPlan priInsPlan=InsPlans.GetPlan(priInsSub.PlanNum,procFeeHelper.ListInsPlans);
				procFeeHelper.ListBenefitsPrimary=procFeeHelper.ListBenefitsPrimary??Benefits.RefreshForPlan(priInsPlan.PlanNum,priPatPlan.PatPlanNum);
			}
			else {
				procFeeHelper.ListBenefitsPrimary=new List<Benefit>();
			}
			return procFeeHelper;
		}
	}
}
