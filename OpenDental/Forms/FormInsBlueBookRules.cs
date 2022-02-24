using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;


namespace OpenDental {
	public partial class FormInsBlueBookRules:FormODBase {
		///<summary>Stores the original state of all rules from the insbluebookrule table when we firts get them from the db.</summary>
		private List<InsBlueBookRule> _listInsBlueBookRulesOld;

		public FormInsBlueBookRules() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsBlueBookRules_Load(object sender,EventArgs e) {
			//Get Data
			_listInsBlueBookRulesOld=InsBlueBookRules.GetAll().OrderBy(x => x.ItemOrder).ToList();
			//UCR Fee %
			textUcrFeePercent.Text=PrefC.GetInt(PrefName.InsBlueBookUcrFeePercent).ToString();
			//Check boxes
			checkEnableAnonFeeShare.Checked=false;
			if(PrefC.GetEnum<InsBlueBookAnonShareEnable>(PrefName.InsBlueBookAnonShareEnable)==InsBlueBookAnonShareEnable.On) {
				checkEnableAnonFeeShare.Checked=true;
			}
			checkUsePlanNumInHierarchy.Checked=PrefC.GetBool(PrefName.InsBlueBookUsePlanNumOverride);
			//List boxes
			listAllowedFeeMethod.Items.Clear();
			listAllowedFeeMethod.Items.AddEnums<InsBlueBookAllowedFeeMethod>();
			listAllowedFeeMethod.SelectedIndex=PrefC.GetInt(PrefName.InsBlueBookAllowedFeeMethod);
			listBlueBookFeature.Items.Clear();
			listBlueBookFeature.Items.AddEnums<AllowedFeeSchedsAutomate>();
			listBlueBookFeature.SelectedIndex=PrefC.GetInt(PrefName.AllowedFeeSchedsAutomate);
			//Grid
			FillRulesGrid();
		}

		private void FillRulesGrid() {
			FillRulesGridColumns();
			FillRulesGridRows();
		}

		private void FillRulesGridColumns() {
			gridInsBlueBookRules.BeginUpdate();
			gridInsBlueBookRules.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"RuleType"),0);
			col.IsWidthDynamic=true;
			gridInsBlueBookRules.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Matching"),130);
			gridInsBlueBookRules.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"LimitType"),70,HorizontalAlignment.Center);
			gridInsBlueBookRules.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"LimitValue"),70,HorizontalAlignment.Center);
			gridInsBlueBookRules.ListGridColumns.Add(col);
			gridInsBlueBookRules.EndUpdate();
		}

		private void FillRulesGridRows() {
			gridInsBlueBookRules.BeginUpdate();
			gridInsBlueBookRules.ListGridRows.Clear();
			for(int i=0;i<_listInsBlueBookRulesOld.Count;i++) {
				InsBlueBookRule rule=_listInsBlueBookRulesOld[i].Copy();
				GridRow row=new GridRow();
				FillRuleGridRow(row,rule);
				gridInsBlueBookRules.ListGridRows.Add(row);
			}
			gridInsBlueBookRules.EndUpdate();
		}

		private void FillRuleGridRow(GridRow row,InsBlueBookRule rule) {
			row.Cells.Clear();
			if(InsBlueBookRules.IsDateLimitedType(rule)) {
				string ruleTypeCell="*Allowed fees from received claims";
				if(!checkUsePlanNumInHierarchy.Checked && rule.RuleType==InsBlueBookRuleType.InsurancePlan) {
					ruleTypeCell="Disabled";
				}
				row.Cells.Add(ruleTypeCell);
				row.Cells.Add(ODPrimitiveExtensions.GetDescription(rule.RuleType));
			}
			else if(rule.RuleType==InsBlueBookRuleType.ManualBlueBookSchedule) {
				row.Cells.Add("Manual blue book fee schedule");
				row.Cells.Add("Insurance Plan");
			}
			else if(rule.RuleType==InsBlueBookRuleType.UcrFee) {
				row.Cells.Add("UCR fee");
				row.Cells.Add("All");
			}
			row.Cells.Add(rule.LimitType.ToString());
			row.Cells.Add(rule.LimitValue.ToString());
			row.Tag=rule;
		}

		private void listBlueBookFeature_SelectedIndexChanged(object sender,EventArgs e) {
			if(listBlueBookFeature.GetSelected<AllowedFeeSchedsAutomate>()==AllowedFeeSchedsAutomate.BlueBook) {
				groupBlueBookSettings.Enabled=true;
			}
			else {
				groupBlueBookSettings.Enabled=false;
				checkEnableAnonFeeShare.Checked=false;
			}
		}

		private void GridInsBlueBookRules_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			InsBlueBookRule ruleSelected=(InsBlueBookRule)gridInsBlueBookRules.ListGridRows[e.Row].Tag;
			if(ruleSelected.RuleType==InsBlueBookRuleType.ManualBlueBookSchedule || ruleSelected.RuleType==InsBlueBookRuleType.UcrFee) {
				MsgBox.Show(this,"The rules "+ODPrimitiveExtensions.GetDescription(InsBlueBookRuleType.ManualBlueBookSchedule)+" and "
					+ODPrimitiveExtensions.GetDescription(InsBlueBookRuleType.UcrFee)+" cannot be edited.");
				return;
			}
			using FormInsBlueBookRuleEdit formInsBlueBookRuleEdit=new FormInsBlueBookRuleEdit(ruleSelected);
			formInsBlueBookRuleEdit.ShowDialog();
			ruleSelected=formInsBlueBookRuleEdit.RuleCur;
			gridInsBlueBookRules.BeginUpdate();
			FillRuleGridRow(gridInsBlueBookRules.ListGridRows[e.Row],ruleSelected);
			gridInsBlueBookRules.EndUpdate();

		}

		private void checkUsePlanNumInHierarchy_Click(object sender,EventArgs e) {
			for(int i=0;i<gridInsBlueBookRules.ListGridRows.Count;i++) {
				GridRow row=gridInsBlueBookRules.ListGridRows[i];
				InsBlueBookRule rule=(InsBlueBookRule)row.Tag;
				if(rule.RuleType==InsBlueBookRuleType.InsurancePlan) {
					gridInsBlueBookRules.BeginUpdate();
					FillRuleGridRow(row,(InsBlueBookRule)row.Tag);
					gridInsBlueBookRules.EndUpdate();
					return;
				}
			}
		}

		private void ButUp_Click(object sender,EventArgs e) {
			MoveSelectedRuleInGrid(true);
		}
	

		private void ButDown_Click(object sender,EventArgs e) {
			MoveSelectedRuleInGrid(false);
		}

		private void MoveSelectedRuleInGrid(bool isMovingUp) {
			int indexSelected=gridInsBlueBookRules.GetSelectedIndex();
			if(indexSelected==-1) {
				MsgBox.Show(this,"Please select a rule first.");
				return;
			}
			int indexToSwap=indexSelected+1;
			if(isMovingUp) {
				indexToSwap=indexSelected-1;
			}
			if(!gridInsBlueBookRules.SwapRows(indexSelected,indexToSwap)) {
				return;
			}
			gridInsBlueBookRules.SetSelected(indexToSwap,true);
		}

		///<summary>Updates prefs and cache as needed.</summary>
		private void UpdatePrefs() {
			InsBlueBookAnonShareEnable anonShareEnable;
			if(checkEnableAnonFeeShare.Checked) {
				anonShareEnable=InsBlueBookAnonShareEnable.On;
			}
			else if(listBlueBookFeature.GetSelected<AllowedFeeSchedsAutomate>()==AllowedFeeSchedsAutomate.BlueBook) {
				//The box is not checked, and blue book is selected, so we set the preference so that they don't get prompted to turn it on again.
				anonShareEnable=InsBlueBookAnonShareEnable.OffNoPrompt;
			}
			else {
				//The box is not checked and they are selecting a setting other than the blue book feature.
				//Sharing is turned off but if blue book is turned on again, they may be prompted to turn this pref on.
				anonShareEnable=InsBlueBookAnonShareEnable.Off;
			}
			bool changed=false;
			changed|=Prefs.UpdateInt(PrefName.InsBlueBookAnonShareEnable,(int)anonShareEnable);
			changed|=Prefs.UpdateBool(PrefName.InsBlueBookUsePlanNumOverride,checkUsePlanNumInHierarchy.Checked);
			changed|=Prefs.UpdateInt(PrefName.InsBlueBookAllowedFeeMethod,listAllowedFeeMethod.SelectedIndex);
			changed|=Prefs.UpdateInt(PrefName.AllowedFeeSchedsAutomate,listBlueBookFeature.SelectedIndex);
			changed|=Prefs.UpdateInt(PrefName.InsBlueBookUcrFeePercent,PIn.Int(textUcrFeePercent.Text,false));
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		///<summary>As of now, the old and new rule lists should always have one of each rule type because the type is not editable by the user and rules never get added or deleted.</summary>
		private void UpdateRules() {
			for(int i=0;i<gridInsBlueBookRules.ListGridRows.Count;i++) {
				InsBlueBookRule ruleCur=(InsBlueBookRule)gridInsBlueBookRules.ListGridRows[i].Tag;
				ruleCur.ItemOrder=i;
				InsBlueBookRule ruleOld=_listInsBlueBookRulesOld.FirstOrDefault(x => x.InsBlueBookRuleNum==ruleCur.InsBlueBookRuleNum);
				InsBlueBookRules.Update(ruleCur,ruleOld);
			}
		}

		///<summary>Creates Out of Network fee schedules for insurance plans when the Legacy Blue Book feature is turned on.<summary>
		private void GenerateAllowedFeeSchedules() {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Allowed fee schedules will now be set up for all insurance plans that do not already have one." +
				"\r\nThe name of each fee schedule will exactly match the name of the carrier." +
				"\r\nOnce created, allowed fee schedules can be easily managed from the fee schedules window.\r\nContinue?"))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			long schedsAdded=InsPlans.GenerateAllowedFeeSchedules();
			Cursor=Cursors.Default;
			MessageBox.Show(Lan.g(this,"Done.  Allowed fee schedules added: ")+schedsAdded.ToString());
			DataValid.SetInvalid(InvalidType.FeeScheds);
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(!textUcrFeePercent.IsValid() || textUcrFeePercent.Text=="") {
				MsgBox.Show(this,"Please enter a UCR Fee Percent between 0 and 100.");
				return;
			}
			if(listBlueBookFeature.GetSelected<AllowedFeeSchedsAutomate>()==AllowedFeeSchedsAutomate.LegacyBlueBook) {
				GenerateAllowedFeeSchedules();
			}
			if(listBlueBookFeature.GetSelected<AllowedFeeSchedsAutomate>()==AllowedFeeSchedsAutomate.BlueBook
				&& checkEnableAnonFeeShare.Checked==false
				&& PrefC.GetEnum<InsBlueBookAnonShareEnable>(PrefName.InsBlueBookAnonShareEnable)==InsBlueBookAnonShareEnable.Off)
			{
				checkEnableAnonFeeShare.Checked=MsgBox.Show(this,MsgBoxButtons.YesNo,
					"Would you like to anonymously share insurance payment data to help us improve the Blue Book feature?");
			}
			UpdatePrefs();
			UpdateRules();
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}