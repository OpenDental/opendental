using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	/// <summary>For a given subscriber, this list all their plans.  User then selects one plan from the list or creates a blank plan.</summary>
	public partial class FormInsSelectSubscr : FormODBase {
		private long Subscriber;
		private List <InsSub> SubList;
		private long _patNum;
		///<summary>When dialogResult=OK, this will contain the InsSubNum of the selected plan.  If this is 0, then user has selected the 'New' option.</summary>
		public long SelectedInsSubNum;

		///<summary></summary>
		public FormInsSelectSubscr(long subscriber, long patNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Subscriber=subscriber;
			_patNum=patNum;
		}

		private void FormInsSelectSubscr_Load(object sender, System.EventArgs e) {
			SubList=InsSubs.GetListForSubscriber(Subscriber);
			List<InsPlan> planList=InsPlans.RefreshForSubList(SubList);
			//PatPlan[] patPlanArray;
			string str;
			InsPlan plan;
			if(!InsSubs.ValidatePlanNumForList(SubList.Select(x => x.InsSubNum).ToList())) {//If !isValid, any links should have been fixed and we now need to update our list.
				SubList=InsSubs.GetListForSubscriber(Subscriber);
			}
			for(int i=0;i<SubList.Count;i++) {
				plan=InsPlans.GetPlan(SubList[i].PlanNum,planList);
				str=InsPlans.GetCarrierName(SubList[i].PlanNum,planList);
				if(plan.GroupNum!="") {
					str+=Lan.g(this," group:")+plan.GroupNum;
				}
				int countPatPlans=PatPlans.GetCountBySubNum(SubList[i].InsSubNum);
				if(countPatPlans==0) {
					str+=" "+Lan.g(this,"(not in use)");
				}
				listPlans.Items.Add(str);
			}
		}

		/// <summary>Checks if the insurnace plan selected still exists. Returns true if it does, false if it doesn't. If the plan does not exist a message box is popped up
		/// and the selected index is removed from SubList and listPlans </summary>
		private bool VerifyInsPlanExists() {
			if(InsPlans.GetPlan(SubList[listPlans.SelectedIndex].PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Insurance plan selected no longer exists.");
				SubList.RemoveAt(listPlans.SelectedIndex);
				listPlans.Items.RemoveAt(listPlans.SelectedIndex);
				return false;
			}
			return true;
		} 

		private void listPlans_DoubleClick(object sender, System.EventArgs e) {
			if(listPlans.SelectedIndex==-1){
				return;
			}
			if(PatPlans.GetCountForPatAndInsSub(SubList[listPlans.SelectedIndex].InsSubNum,_patNum)!=0) {
				MsgBox.Show(this,"This patient already has this plan attached.  If you would like to add a new plan for the same subscriber and insurance carrier, click new plan.");
				return;
			}
			if(!VerifyInsPlanExists()) {
				return;
			}
			SelectedInsSubNum=SubList[listPlans.SelectedIndex].InsSubNum;
			DialogResult=DialogResult.OK;
		}

		private void butNew_Click(object sender, System.EventArgs e) {
			SelectedInsSubNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPlans.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a plan first.");
				return;
			}
			if(PatPlans.GetCountForPatAndInsSub(SubList[listPlans.SelectedIndex].InsSubNum,_patNum)!=0) {
				MsgBox.Show(this,"This patient already has this plan attached.  If you would like to add a new plan for the same subscriber and insurance carrier, click new plan.");
				return;
			}
			if(!VerifyInsPlanExists()) {
				return;
			}
			SelectedInsSubNum=SubList[listPlans.SelectedIndex].InsSubNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















