using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>For a given subscriber, this list all their plans.  User then selects one plan from the list or creates a blank plan.</summary>
	public partial class FrmInsSelectSubscr : FrmODBase {
		private long _subscriber;
		private List <InsSub> _listInsSubs;
		private long _patNum;
		///<summary>When dialogResult=OK, this will contain the InsSubNum of the selected plan.  If this is 0, then user has selected the 'New' option.</summary>
		public long SelectedInsSubNum;

		///<summary></summary>
		public FrmInsSelectSubscr(long subscriber, long patNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			_subscriber=subscriber;
			_patNum=patNum;
		}

		private void FrmInsSelectSubscr_Loaded(object sender,RoutedEventArgs e) {
			_listInsSubs=InsSubs.GetListForSubscriber(_subscriber);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			//PatPlan[] patPlanArray;
			string str;
			InsPlan insPlan;
			if(!InsSubs.ValidatePlanNumForList(_listInsSubs.Select(x => x.InsSubNum).ToList())) {//If !isValid, any links should have been fixed and we now need to update our list.
				_listInsSubs=InsSubs.GetListForSubscriber(_subscriber);
			}
			for(int i=0;i<_listInsSubs.Count;i++) {
				insPlan=InsPlans.GetPlan(_listInsSubs[i].PlanNum,listInsPlans);
				str=InsPlans.GetCarrierName(_listInsSubs[i].PlanNum,listInsPlans);
				if(insPlan.GroupNum!="") {
					str+=Lans.g(this," group:")+insPlan.GroupNum;
				}
				int countPatPlans=PatPlans.GetCountBySubNum(_listInsSubs[i].InsSubNum);
				if(countPatPlans==0) {
					str+=" "+Lans.g(this,"(not in use)");
				}
				listPlans.Items.Add(str);
			}
		}

		/// <summary>Checks if the insurnace plan selected still exists. Returns true if it does, false if it doesn't. If the plan does not exist a message box is popped up
		/// and the selected index is removed from SubList and listPlans </summary>
		private bool VerifyInsPlanExists() {
			if(InsPlans.GetPlan(_listInsSubs[listPlans.SelectedIndex].PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Insurance plan selected no longer exists.");
				_listInsSubs.RemoveAt(listPlans.SelectedIndex);
				listPlans.Items.RemoveAt(listPlans.SelectedIndex);
				return false;
			}
			return true;
		} 

		private void listPlans_DoubleClick(object sender, MouseButtonEventArgs e) {
			if(listPlans.SelectedIndex==-1){
				return;
			}
			if(PatPlans.GetCountForPatAndInsSub(_listInsSubs[listPlans.SelectedIndex].InsSubNum,_patNum)!=0) {
				MsgBox.Show(this,"This patient already has this plan attached.  If you would like to add a new plan for the same subscriber and insurance carrier, click new plan.");
				return;
			}
			if(!VerifyInsPlanExists()) {
				return;
			}
			SelectedInsSubNum=_listInsSubs[listPlans.SelectedIndex].InsSubNum;
			IsDialogOK=true;
		}

		private void butNew_Click(object sender, System.EventArgs e) {
			SelectedInsSubNum=0;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPlans.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a plan first.");
				return;
			}
			if(PatPlans.GetCountForPatAndInsSub(_listInsSubs[listPlans.SelectedIndex].InsSubNum,_patNum)!=0) {
				MsgBox.Show(this,"This patient already has this plan attached.  If you would like to add a new plan for the same subscriber and insurance carrier, click new plan.");
				return;
			}
			if(!VerifyInsPlanExists()) {
				return;
			}
			SelectedInsSubNum=_listInsSubs[listPlans.SelectedIndex].InsSubNum;
			IsDialogOK=true;
		}

		

		

		


	}
}





















