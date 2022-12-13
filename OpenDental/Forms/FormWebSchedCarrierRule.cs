using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;


namespace OpenDental {
	public partial class FormWebSchedCarrierRule:FormODBase {
		private const string _DEFAULT_CARRIER_OTHER="Other (Default)";
		private	const string _DEFAULT_CARRIER_SELF_PAY="Self Pay/No Insurance (Default)";

		private long _clinicNum;
		//This is here to prevent the grid from loading when the user does not have a clinic selected or clicks out of the comboBox but does not actually have a clinic selected
		//This also disables clinic specific controls
		private bool _hasClinicSelected;
		private List<string> _listCarrierNames;
		private List<WebSchedCarrierRule> _listWebSchedCarrierRules;
		
		public FormWebSchedCarrierRule() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebSchedCarrierRule_Load(object sender,EventArgs e) {
			_hasClinicSelected=true;
			if(!PrefC.HasClinicsEnabled) {
				comboClinics.Visible=false;
				butCopyRules.Visible=false;
			}
			//Web Sched does not support ClinicNum 0 when clinics are enabled 
			if(PrefC.HasClinicsEnabled && comboClinics.SelectedClinicNum==-1) { 
				checkNewPatRequestIns.Enabled=false;
				checkExistingPatRequestIns.Enabled=false;
				_hasClinicSelected=false;
			}
			if(comboClinics.SelectedClinicNum<=0) {
				_clinicNum=0;
			}
			else {
				_clinicNum=comboClinics.SelectedClinicNum;
			}
			_listCarrierNames=Carriers.GetAllDistinctCarrierNames();
			if(_hasClinicSelected) {
				FillCheckBoxCarrierRules();
				FillGridWebSchedCarrierRules(true);
				FillListBoxCarrierNames();
			}
		}

		private void FillCheckBoxCarrierRules() {
			if(_clinicNum==0) {
				checkNewPatRequestIns.Checked=PrefC.GetBool(PrefName.WebSchedNewPatRequestInsurance);
				checkExistingPatRequestIns.Checked=PrefC.GetBool(PrefName.WebSchedExistingPatRequestInsurance);
			}
			else {
				checkNewPatRequestIns.Checked=ClinicPrefs.GetBool(PrefName.WebSchedNewPatRequestInsurance,_clinicNum);
				checkExistingPatRequestIns.Checked=ClinicPrefs.GetBool(PrefName.WebSchedExistingPatRequestInsurance,_clinicNum);
			}
		}

		private void FillGridWebSchedCarrierRules(bool getRulesFromDb=false) {		
			if(getRulesFromDb) {
				_listWebSchedCarrierRules=WebSchedCarrierRules.GetWebSchedCarrierRulesForClinic(_clinicNum);
			}
			else {
				_listWebSchedCarrierRules=_listWebSchedCarrierRules.OrderBy(x => x.CarrierName).ToList();
			} 
			gridWebSchedCarrierRules.BeginUpdate();
			gridWebSchedCarrierRules.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("GridWebSchedCarrierRules","Insurance Carrier Name"),200);
			gridWebSchedCarrierRules.Columns.Add(col);
			col=new GridColumn(Lan.g("GridWebSchedCarrierRules","Display Name"),200);
			gridWebSchedCarrierRules.Columns.Add(col);
			col=new GridColumn(Lan.g("GridWebSchedCarrierRules","Rule"),110);
			gridWebSchedCarrierRules.Columns.Add(col);
			col=new GridColumn(Lan.g("GridWebSchedCarrierRules","Message"),0);
			gridWebSchedCarrierRules.Columns.Add(col);
			gridWebSchedCarrierRules.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listWebSchedCarrierRules.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listWebSchedCarrierRules[i].CarrierName);
				row.Cells.Add(_listWebSchedCarrierRules[i].DisplayName);
				row.Cells.Add(_listWebSchedCarrierRules[i].Rule.ToString());
				row.Cells.Add(_listWebSchedCarrierRules[i].Message);
				row.Tag=_listWebSchedCarrierRules[i];
				gridWebSchedCarrierRules.ListGridRows.Add(row);
			}
			gridWebSchedCarrierRules.EndUpdate();
		}

		private void FillListBoxCarrierNames() {
			listBoxCarriers.Items.Clear();
			List<string> listCarrierNamesFiltered=new List<string>();
			if(!_listWebSchedCarrierRules.Any(x => x.CarrierName==_DEFAULT_CARRIER_OTHER)) {
				listCarrierNamesFiltered.Add(_DEFAULT_CARRIER_OTHER);
			}
			if(!_listWebSchedCarrierRules.Any(x => x.CarrierName==_DEFAULT_CARRIER_SELF_PAY)) {
				listCarrierNamesFiltered.Add(_DEFAULT_CARRIER_SELF_PAY);
			}
			listCarrierNamesFiltered.AddRange(_listCarrierNames.Select(x => x));
			listCarrierNamesFiltered.RemoveAll(x => _listWebSchedCarrierRules.Any(y => y.CarrierName==x));
			listBoxCarriers.Items.AddList(listCarrierNamesFiltered,x => x);
		}

		private void checkNewPatRequestIns_Click(object sender,EventArgs e) {
			if(_clinicNum==0) {
				Prefs.UpdateBool(PrefName.WebSchedNewPatRequestInsurance,checkNewPatRequestIns.Checked);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.WebSchedNewPatRequestInsurance,_clinicNum,POut.Bool(checkNewPatRequestIns.Checked));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void checkExistingPatRequestIns_Click(object sender,EventArgs e) {
			if(_clinicNum==0) {
				Prefs.UpdateBool(PrefName.WebSchedExistingPatRequestInsurance,checkExistingPatRequestIns.Checked);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.WebSchedExistingPatRequestInsurance,_clinicNum,POut.Bool(checkExistingPatRequestIns.Checked));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void comboWebSchedClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinics.SelectedClinicNum>0) {
				if(!_hasClinicSelected) {
					checkNewPatRequestIns.Enabled=true;
					checkExistingPatRequestIns.Enabled=true;
					_hasClinicSelected=true;
				}			
				_clinicNum=comboClinics.SelectedClinicNum;
				FillCheckBoxCarrierRules();
				FillGridWebSchedCarrierRules(true);
				FillListBoxCarrierNames();
			}
		}

		private void gridWebSchedCarrierRules_DoubleClick(object sender,EventArgs e) {
			if(gridWebSchedCarrierRules.SelectedGridRows.Count>1) {
				MsgBox.Show(this,"Please select only one row to edit.");
				return;
			}
			WebSchedCarrierRule webSchedCarrierRule=gridWebSchedCarrierRules.SelectedTag<WebSchedCarrierRule>();
			if(webSchedCarrierRule==null) {
				MsgBox.Show(this,"Please select a rule to be edited.");
				return;
			}
			using FormWebSchedCarrierRuleEdit formWebSchedCarrierRuleEdit=new FormWebSchedCarrierRuleEdit();
			formWebSchedCarrierRuleEdit.WebSchedCarrierRule=webSchedCarrierRule;
			if(webSchedCarrierRule.CarrierName==_DEFAULT_CARRIER_OTHER) {
				formWebSchedCarrierRuleEdit.IsOtherDefaultCarrier=true;
			}
			formWebSchedCarrierRuleEdit.ShowDialog();
			if(formWebSchedCarrierRuleEdit.DialogResult==DialogResult.OK) {
				FillGridWebSchedCarrierRules();
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			List<long> listSelectedWebSchedCarrierRuleNums=gridWebSchedCarrierRules.SelectedTags<WebSchedCarrierRule>().Select(x => x.WebSchedCarrierRuleNum).ToList();
			if(listSelectedWebSchedCarrierRuleNums.Count==0) {
				MsgBox.Show(this,"Please select at least one Carrier.");
				return;
			}
			string deleteSelectedMsg=Lans.g(this,"This will delete all of the Carrier Rules selected. Continue?");
			if(!MsgBox.Show(MsgBoxButtons.YesNo,deleteSelectedMsg)) {
				return;
			}
			WebSchedCarrierRules.DeleteMany(listSelectedWebSchedCarrierRuleNums);
			_listWebSchedCarrierRules.RemoveAll(x => listSelectedWebSchedCarrierRuleNums.Any(y => y==x.WebSchedCarrierRuleNum));
			FillGridWebSchedCarrierRules();
			FillListBoxCarrierNames();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			List<string> listSelectedCarriers=listBoxCarriers.GetListSelected<string>();
			if(listSelectedCarriers.Count==0) {
				MsgBox.Show(this,"Please select at least one Carrier.");
				return;
			}
			List<WebSchedCarrierRule> listWebSchedCarrierRulesAdded=new List<WebSchedCarrierRule>();
			for(int i=0;i<listSelectedCarriers.Count;i++) {
				WebSchedCarrierRule webSchedCarrierRule=new WebSchedCarrierRule();
				webSchedCarrierRule.ClinicNum=_clinicNum;
				webSchedCarrierRule.CarrierName=listSelectedCarriers[i];
				webSchedCarrierRule.Rule=RuleType.Allow;
				if(listSelectedCarriers[i]==_DEFAULT_CARRIER_OTHER) {
					webSchedCarrierRule.DisplayName="Other";
					webSchedCarrierRule.Rule=RuleType.AllowWithInput;
				}
				else if(listSelectedCarriers[i]==_DEFAULT_CARRIER_SELF_PAY) {
					webSchedCarrierRule.DisplayName="Self Pay/No Insurance";
				}
				else {
					webSchedCarrierRule.DisplayName=listSelectedCarriers[i];
				}
				webSchedCarrierRule.Message="";
				listWebSchedCarrierRulesAdded.Add(webSchedCarrierRule);
			}
			WebSchedCarrierRules.InsertMany(listWebSchedCarrierRulesAdded);
			_listWebSchedCarrierRules.AddRange(listWebSchedCarrierRulesAdded);
			FillGridWebSchedCarrierRules(true);
			FillListBoxCarrierNames();
		}

		private void butCopyRules_Click(object sender,EventArgs e) {
			using FormWebSchedCarrierRuleCopyToClinics formWebSchedCarrierRuleCopyToClinics=new FormWebSchedCarrierRuleCopyToClinics();
			formWebSchedCarrierRuleCopyToClinics.ShowDialog();
			if(formWebSchedCarrierRuleCopyToClinics.DialogResult==DialogResult.OK) {
				if(_hasClinicSelected) {
					FillGridWebSchedCarrierRules(true);
					FillListBoxCarrierNames();
				}
			}
		}

		private void butSuggest_Click(object sender,EventArgs e) {
			if(!_hasClinicSelected) {
				MsgBox.Show(this,"Please select a clinic first.");
				return;
			}
			int numDays=60;
			List<string> listSuggestedCarrierNames=Claims.GetTopVolumeCarrierNamesForClinicAndPeriod(_clinicNum,numDays,10);
			if(listSuggestedCarrierNames.Count==0) {
				MsgBox.Show(Lans.g(this,"No suggestions can be made since there has been no claim activity for the last")+" "+numDays+" "+Lans.g(this,"days."));
				return;
			}
			List<string> listCarrierNamesToCompare=listBoxCarriers.Items.GetAll<string>();
			List<string> listCarrierNameRulesToAdd=new List<string>();
			string suggestedCarriers="";
			//Check to see which carriers currently do not already have rules, if not add to the list of rules to add
			for(int i=0;i<listSuggestedCarrierNames.Count;i++) {
				if(listCarrierNamesToCompare.Contains(listSuggestedCarrierNames[i])) {
					suggestedCarriers+=listSuggestedCarrierNames[i]+"\r\n";
					listCarrierNameRulesToAdd.Add(listSuggestedCarrierNames[i]);
				}
			}
			if(listCarrierNameRulesToAdd.Count==0) {
				MsgBox.Show(this,"No suggestions needed, you already have rules in place for Carriers with the highest amount of sent claims.");
				return;
			}
			string suggestedCarrierMsg=Lans.g(this,"Based on the highest number of claims sent, we recommend the following Carriers be added to your rule list:")
				+"\r\n\r\n"
				+suggestedCarriers
				+"\r\nContinue?";
			if(!MsgBox.Show(MsgBoxButtons.YesNo,suggestedCarrierMsg)) {
				return;
			}
			List<WebSchedCarrierRule> listWebSchedCarrierRulesAdded=new List<WebSchedCarrierRule>();
			for(int i=0;i<listCarrierNameRulesToAdd.Count;i++) { 
				WebSchedCarrierRule webSchedCarrierRule=new WebSchedCarrierRule();
				webSchedCarrierRule.ClinicNum=_clinicNum;
				webSchedCarrierRule.CarrierName=listCarrierNameRulesToAdd[i];				
				webSchedCarrierRule.DisplayName=listCarrierNameRulesToAdd[i];
				webSchedCarrierRule.Rule=RuleType.Allow;
				webSchedCarrierRule.Message="";
				listWebSchedCarrierRulesAdded.Add(webSchedCarrierRule);
			}
			WebSchedCarrierRules.InsertMany(listWebSchedCarrierRulesAdded);
			_listWebSchedCarrierRules.AddRange(listWebSchedCarrierRulesAdded);
			FillGridWebSchedCarrierRules(true);
			FillListBoxCarrierNames();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}