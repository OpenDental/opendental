using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Doesn't look like it's actually possible to use from dashboard.</summary>
	public partial class DashIndividualDiscount:UserControl,IDashWidgetField {
		///<summary>The subscriber for the discount plan. Can be an newly instantiated sub with no valid values if no discount plan.</summary>
		private DiscountPlanSub _discountPlanSub;
		///<summary>The discount plan for the patient. Can be an newly instantiated plan with no valid values if the patient does not have a discount plan.</summary>
		private DiscountPlan _discountPlan;
		private Patient _pat;
		///<summary>The total of all discount plan adjustments for the current date range segment for DateTime.Now.</summary>
		private double _discountAmtUsed;
		public LayoutManagerForms LayoutManager;

		public DashIndividualDiscount() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			_pat=pat;
			if(_pat!=null) {
				_discountPlanSub=DiscountPlanSubs.GetSubForPat(pat.PatNum);
			}
			_discountPlan=null;
			if(_discountPlanSub!=null) {
				_discountPlan=DiscountPlans.GetPlan(_discountPlanSub.DiscountPlanNum);
			}
			RefreshView();
		}

		public void RefreshView() {
			RefreshDiscountPlan(_pat,_discountPlanSub,_discountPlan);
		}

		public void RefreshDiscountPlan(Patient pat,DiscountPlanSub discountPlanSub,DiscountPlan discountPlan) {
			_pat=pat;
			_discountPlanSub=discountPlanSub;
			_discountPlan=discountPlan;
			_discountAmtUsed=0;
			groupBoxIndDiscount.Text=Lan.g(this,"Discount Plan");
			label11.Text=Lan.g(this,"Annual Max");
			textMaxAdj.Text="";
			label12.Text=Lan.g(this,"Adj Used");
			textUsedAdj.Text="";
			label18.Text=Lan.g(this,"Adj Remaining");
			textRemainingAdj.Text="";
			if(pat==null || _discountPlanSub==null || _discountPlan==null) {
				return;
			}
			if(!DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(_discountPlanSub,DateTime.Now,out DateTime startDate,out DateTime stopDate)) {
				return;
			}
			_discountAmtUsed=Adjustments.GetTotForPatByType(_discountPlanSub.PatNum,_discountPlan.DefNum,startDate,stopDate);
			if(_discountPlan.AnnualMax!=-1) {
				double adjMax=_discountPlan.AnnualMax;
				textMaxAdj.Text=adjMax.ToString("F");
				double adjRem=adjMax-_discountAmtUsed;
				textRemainingAdj.Text=adjRem.ToString("F");
			}
			textUsedAdj.Text=_discountAmtUsed.ToString("F");
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			ExtractData(data);
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.DiscountPlan==null || data.DiscountPlanSub==null) {
				return false;
			}
			return true;
		}

		private void ExtractData(PatientDashboardDataEventArgs data) {
			_pat=data.Pat;
			_discountPlan=data.DiscountPlan;
			_discountPlanSub=data.DiscountPlanSub;
		}
	}
}
