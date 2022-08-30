using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPreferences:FormODBase {

		#region Fields - Private
		//Appointment 
		private UserControlApptGeneral userControlApptGeneral=new UserControlApptGeneral();
		private UserControlApptAppearance userControlApptAppearance=new UserControlApptAppearance();
		//Family
		private UserControlFamilyGeneral userControlFamilyGeneral=new UserControlFamilyGeneral();
		private UserControlFamilyInsurance userControlFamilyInsurance=new UserControlFamilyInsurance();
		//Account
		private UserControlAccountGeneral userControlAccountGeneral=new UserControlAccountGeneral();
		private UserControlAccountAdjustments userControlAccountAdjustments=new UserControlAccountAdjustments();
		private UserControlAccountInsurance userControlAccountInsurance=new UserControlAccountInsurance();
		private UserControlAccountPayments userControlAccountPayments=new UserControlAccountPayments();
		private UserControlAccountRecAndRepCharges userControlAccountRecAndRepCharges=new UserControlAccountRecAndRepCharges();
		//Treat Plan
		private UserControlTreatPlanGeneral userControlTreatPlanGeneral=new UserControlTreatPlanGeneral();
		private UserControlTreatPlanFreqLimit userControlTreatPlanFreqLimit=new UserControlTreatPlanFreqLimit();
		//Chart
		private UserControlChartGeneral userControlChartGeneral=new UserControlChartGeneral();
		private UserControlChartProcedures userControlChartProcedures=new UserControlChartProcedures();
		//Imaging
		private UserControlImagingGeneral userControlImagingGeneral=new UserControlImagingGeneral();
		//Manage
		private UserControlManageGeneral userControlManageGeneral=new UserControlManageGeneral();
		private UserControlManageBillingStatements userControlManageBillingStatements=new UserControlManageBillingStatements();
		#endregion Fields - Private

		#region Fields - Public
		public int SelectedNode=0;
		#endregion Fields - Public
		
		#region Constructors
		public FormPreferences() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void FormModulePrefs_Load(object sender,EventArgs e) {
			FillUserControls();
			if(ODBuild.IsDebug())	{
				LoadUserControls();
			}
			else {
				try {//try/catch used to prevent setup form from partially loading and filling controls.  Causes UEs, Example: TimeCardOvertimeFirstDayOfWeek set to -1 because UI control not filled properly.
					LoadUserControls();
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"An error has occurred while attempting to load preferences.  Run database maintenance and try again."),ex);
					DialogResult=DialogResult.Abort;
					return;
				}
			}
			if(SelectedNode==0) {
				treeMain.SelectedNode=treeMain.Nodes[0];
			}
			else {
				treeMain.SelectedNode=treeMain.Nodes[SelectedNode];
			}
			Plugins.HookAddCode(this,"FormModuleSetup.FormModuleSetup_Load_end");
		}

		private void treeMain_AfterSelect(object sender,TreeViewEventArgs e) {
			UserControl userControlSelected=treeMain.SelectedNode.Tag as UserControl;
			if(userControlSelected==null) {
				return;
			}
			userControlSelected.BringToFront();
		}

		private void treeMain_BeforeCollapse(object sender,TreeViewCancelEventArgs e) {
			e.Cancel=true;//Never allow the tree to collapse
		}

		private void butOK_Click(object sender,EventArgs e) {
			//validation is done within each save.
			//One save to db might succeed, and then a subsequent save can fail to validate.  That's ok.
			if(!userControlApptGeneral.SaveApptGeneral()
				|| !userControlApptAppearance.SaveApptAppearance()
				|| !userControlFamilyGeneral.SaveFamilyGeneral()
				|| !userControlFamilyInsurance.SaveFamilyInsurance()
				|| !userControlAccountAdjustments.SaveAccountAdjustments()
				|| !userControlAccountGeneral.SaveAccountGeneral()
				|| !userControlAccountInsurance.SaveAccountInsurance()
				|| !userControlAccountPayments.SaveAccountPayments()
				|| !userControlAccountRecAndRepCharges.SaveAccountRecAndRepCharges()
				|| !userControlTreatPlanGeneral.SaveTreatPlanGeneral()
				|| !userControlTreatPlanFreqLimit.SaveTreatPlanFreqLimit()
				|| !userControlChartGeneral.SaveChartGeneral()
				|| !userControlChartProcedures.SaveChartProcedures()
				|| !userControlManageGeneral.SaveManageGeneral()
				|| !userControlManageBillingStatements.SaveManageBillingStatements()
				//Out of order because we want to save all prefs before potentially closing program for image module swap.
				|| !userControlImagingGeneral.SaveImagingGeneral()
				)
			{
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPreferences_FormClosing(object sender,FormClosingEventArgs e) {
			if(userControlApptGeneral.Changed
				|| userControlApptAppearance.Changed
				|| userControlFamilyGeneral.Changed
				|| userControlFamilyInsurance.Changed
				|| userControlAccountAdjustments.Changed
				|| userControlAccountGeneral.Changed
				|| userControlAccountInsurance.Changed
				|| userControlAccountPayments.Changed
				|| userControlAccountRecAndRepCharges.Changed
				|| userControlTreatPlanGeneral.Changed
				|| userControlTreatPlanFreqLimit.Changed
				|| userControlChartGeneral.Changed
				|| userControlChartProcedures.Changed
				|| userControlManageGeneral.Changed
				|| userControlManageBillingStatements.Changed
				|| userControlImagingGeneral.Changed)
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Create one control per menu item using the LayoutManager.</summary>
		private void FillUserControls() {
			List<UserControl> listUserControls=new List<UserControl>();
			listUserControls.Add(userControlApptGeneral);
			listUserControls.Add(userControlApptAppearance);	
			listUserControls.Add(userControlFamilyGeneral);
			listUserControls.Add(userControlFamilyInsurance);
			listUserControls.Add(userControlAccountGeneral);
			listUserControls.Add(userControlAccountAdjustments);
			listUserControls.Add(userControlAccountInsurance);
			listUserControls.Add(userControlAccountPayments);
			listUserControls.Add(userControlAccountRecAndRepCharges);
			listUserControls.Add(userControlTreatPlanGeneral);
			listUserControls.Add(userControlTreatPlanFreqLimit);
			listUserControls.Add(userControlChartGeneral);
			listUserControls.Add(userControlChartProcedures);
			listUserControls.Add(userControlImagingGeneral);
			listUserControls.Add(userControlManageGeneral);
			listUserControls.Add(userControlManageBillingStatements);
			for(int i=0;i<listUserControls.Count;i++) {
				listUserControls[i].Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(listUserControls[i],panelMain);
			}
			LayoutManager.LayoutControlBoundsAndFonts(panelMain);
			FillTree();
		}

		///<summary>Fills the MenuTree, make sure that the Tag is the exact same name as the control.</summary>
		private void FillTree() {
			TreeNode treeNodeParent;
			TreeNode treeNodeChild;
			//Appointment
			treeNodeParent=new TreeNode("Appointment - General");
			treeNodeParent.Tag=userControlApptGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Appearance");
			treeNodeChild.Tag=userControlApptAppearance;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Family
			treeNodeParent=new TreeNode("Family - General");
			treeNodeParent.Tag=userControlFamilyGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Insurance");
			treeNodeChild.Tag=userControlFamilyInsurance;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Account
			treeNodeParent=new TreeNode("Account - General");
			treeNodeParent.Tag=userControlAccountGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Adjustments");
			treeNodeChild.Tag=userControlAccountAdjustments;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Insurance");
			treeNodeChild.Tag=userControlAccountInsurance;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Payments");
			treeNodeChild.Tag=userControlAccountPayments;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Recurring and Repeating Charges");
			treeNodeChild.Tag=userControlAccountRecAndRepCharges;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Treat Plan
			treeNodeParent=new TreeNode("Treat Plan - General");
			treeNodeParent.Tag=userControlTreatPlanGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Frequency Limitations");
			treeNodeChild.Tag=userControlTreatPlanFreqLimit;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Chart
			treeNodeParent=new TreeNode("Chart - General");
			treeNodeParent.Tag=userControlChartGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Procedures");
			treeNodeChild.Tag=userControlChartProcedures;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Imaging
			treeNodeParent=new TreeNode("Imaging - General");
			treeNodeParent.Tag=userControlImagingGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			//Manage
			treeNodeParent=new TreeNode("Manage - General");
			treeNodeParent.Tag=userControlManageGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Billing Statements");
			treeNodeChild.Tag=userControlManageBillingStatements;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeMain.ExpandAll();
		}

		///<summary>Load all of the preferences of each control.</summary>
		private void LoadUserControls() {
			userControlApptGeneral.FillApptGeneral();
			userControlApptAppearance.FillApptAppearance();
			userControlFamilyGeneral.FillFamilyGeneral();
			userControlFamilyInsurance.FillFamilyInsurance();
			userControlAccountAdjustments.FillAccountAdjustments();
			userControlAccountGeneral.FillAccountGeneral();
			userControlAccountInsurance.FillAccountInsurance();
			userControlAccountPayments.FillAccountPayments();
			userControlAccountRecAndRepCharges.FillAccountRecAndRepCharges();
			userControlTreatPlanGeneral.FillTreatPlanGeneral();
			userControlTreatPlanFreqLimit.FillTreatPlanFreqLimit();
			userControlChartGeneral.FillChartGeneral();
			userControlChartProcedures.FillChartProcedures();
			userControlImagingGeneral.FillImagingGeneral();
			userControlManageGeneral.FillManageGeneral();
			userControlManageBillingStatements.FillManageBillingStatements();
		}
		#endregion Methods - Private

		#region Methods - Other
		protected override string GetHelpOverride() {
			return treeMain.SelectedNode.Name;
		}
		#endregion Methods - Other
	}
}