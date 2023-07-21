using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormWebSchedCarrierRuleCopyToClinics:FormODBase {
		private Clinic _clinicCopyFrom;
		private List<Clinic> _listClinics;
		private List<WebSchedCarrierRule> _listWebSchedCarrierRules;

		public FormWebSchedCarrierRuleCopyToClinics() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebSchedCarrierRuleCopyToClinics_Load(object sender, EventArgs e) {
			_listClinics=Clinics.GetDeepCopy(true);
			comboClinicsFrom.Items.AddList(_listClinics,x => x.Abbr);
			listClinicsCopyTo.Items.AddList(_listClinics,x => x.Abbr);
			_clinicCopyFrom=comboClinicsFrom.GetSelected<Clinic>();
			if(_clinicCopyFrom!=null) {
				FillGridWebSchedCarrierRules(true);
			}
		}

		private void FillGridWebSchedCarrierRules(bool getRulesFromDb=false) {		
			if(getRulesFromDb) {
				_listWebSchedCarrierRules=WebSchedCarrierRules.GetWebSchedCarrierRulesForClinic(_clinicCopyFrom.ClinicNum);
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

		private void CopyRulesHelper(List<Clinic> listClinicsTo) {
			long clinicNumFrom=comboClinicsFrom.GetSelected<Clinic>().ClinicNum;
			string logMessage="";
			List<WebSchedCarrierRule> listWebSchedCarrierRulesFromClinic=gridWebSchedCarrierRules.SelectedTags<WebSchedCarrierRule>();
			List<WebSchedCarrierRule> listWebSchedCarrierRulesToClinics=WebSchedCarrierRules.GetWebSchedCarrierRulesForClinics(listClinicsTo.Select(x => x.ClinicNum).ToList());
			if(checkOverride.Checked) {
				WebSchedCarrierRules.DeleteMany(listWebSchedCarrierRulesToClinics.Select(x => x.WebSchedCarrierRuleNum).ToList());//Delete all rules for the selected clinics
				logMessage=Lan.g(this,"All rules for the selected clinics have been deleted and replaced with the selected carrier rules.");
			}
			else{
				//Get a list of rules that are being overridden and only replace those specific rules
				List<long> listWebSchedCarrierRulesToDelete=listWebSchedCarrierRulesToClinics
					.FindAll(x => listWebSchedCarrierRulesFromClinic.Any(y => y.CarrierName==x.CarrierName)) //Carrier Name casing should not be an issue because the query handles the casing
					.Select(x => x.WebSchedCarrierRuleNum).ToList();
				if(listWebSchedCarrierRulesToDelete.Count>0) {
					WebSchedCarrierRules.DeleteMany(listWebSchedCarrierRulesToDelete);//Delete the old rules as these will be replaced below
				}
				logMessage=Lan.g(this,"All selected rules have been copied and any existing rules for those carriers have been overridden.");
			}
			List<WebSchedCarrierRule> listWebSchedCarrierRulesToInsert=new List<WebSchedCarrierRule>();//Create a master insert list for one InsertMany statement below
			//Copy all rules of the from clinic to all selected clinics.
			for(int i=0;i<listClinicsTo.Count;i++) {
				//Make a deep copy of all rules for the from clinic and set the ClinicNum to the To clinic and add those rules to the list to be inserted.
				List<WebSchedCarrierRule> listWebSchedCarrierRulesTempCopy=listWebSchedCarrierRulesFromClinic.Select(x => x.Copy()).ToList();
				//NOTE - update the clinic num of the deep copy.
				for(int j=0;j<listWebSchedCarrierRulesTempCopy.Count;j++) {
					listWebSchedCarrierRulesTempCopy[j].ClinicNum=listClinicsTo[i].ClinicNum;
					listWebSchedCarrierRulesToInsert.Add(listWebSchedCarrierRulesTempCopy[j]);
				}
			}
			if(listWebSchedCarrierRulesToInsert.Count>0) {
				WebSchedCarrierRules.InsertMany(listWebSchedCarrierRulesToInsert);//Insert one main list for all necessary clinics
			}
			MsgBox.Show(this,logMessage);
		}

		private void comboClinicsFrom_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinicsFrom.GetSelected<Clinic>()==null) {
				return;
			}
			if(!listClinicsCopyTo.Items.Contains(_clinicCopyFrom)) {
				listClinicsCopyTo.Items.Clear();
				listClinicsCopyTo.Items.AddList(_listClinics,x => x.Abbr);
			}
			_clinicCopyFrom=comboClinicsFrom.GetSelected<Clinic>();
			listClinicsCopyTo.Items.RemoveAt(comboClinicsFrom.SelectedIndex);
			FillGridWebSchedCarrierRules(true);
		}

		private void butCopyRules_Click(object sender,EventArgs e) {
			if(comboClinicsFrom.GetSelected<Clinic>()==null) {
				MsgBox.Show(this,"Please select a clinic to copy rules from first.");
				return;
			}
			if(listClinicsCopyTo.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select one or more clinics to copy these rules to.");
				return;
			}
			if(gridWebSchedCarrierRules.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one or more Rules to copy.");
				return;
			}
			if(checkOverride.Checked) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete all existing rules for the selected clinics and replace them with the selected rules. Continue?")) {
					return;
				}
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will add or replace any existing rules for the selected clinics with the selected rules. Continue?")) {
					return;
				}
			}
			List<Clinic> listClinics=listClinicsCopyTo.GetListSelected<Clinic>();
			CopyRulesHelper(listClinics);
		}

		private void butCopyToAll_Click(object sender,EventArgs e) {
			if(comboClinicsFrom.GetSelected<Clinic>()==null) {
				MsgBox.Show(this,"Please select a clinic to copy rules from first.");
				return;
			}
			if(gridWebSchedCarrierRules.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one or more Rules to copy.");
				return;
			}
			if(checkOverride.Checked) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete all existing rules, for all other clinics, and replace them with the selected rules. Continue?")) {
					return;
				}
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will add or replace any existing, rules for all other clinics, with the selected rules. Continue?")) {
					return;
				}
			}
			List<Clinic> listClinics=listClinicsCopyTo.Items.GetAll<Clinic>();
			CopyRulesHelper(listClinics);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}
	}
}