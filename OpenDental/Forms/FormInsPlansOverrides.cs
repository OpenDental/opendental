using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormInsPlansOverrides:FormODBase {
		private long _codeNum;
		private DataTable _table;
		private bool _isTrojanEnabled;

		///<summary></summary>
		public FormInsPlansOverrides(long codeNum){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_codeNum=codeNum;
		}

		private void FormInsTemplates_Load(object sender, System.EventArgs e) {
			string planOrGroupNum="Group Num";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
				planOrGroupNum="Plan Number";
			}
			labelGroupNum.Text=Lan.g(this,planOrGroupNum);
			Program program=Programs.GetCur(ProgramName.Trojan);
			if(program!=null && program.Enabled) { //If trojan bridge exists and is enabled, allow user to filter upon TrojanID.
				_isTrojanEnabled=true;
				FillGrid();
				return;
			}
			labelTrojanID.Visible=false;
			textTrojanID.Visible=false;
			FillGrid();
		}

		private void FillGrid(bool isGetAll=false){
			Cursor=Cursors.WaitCursor;
			_table=InsPlans.GetBigList(radioOrderEmp.Checked,textEmployer.Text,textCarrier.Text,
				textGroupName.Text,textGroupNum.Text,textPlanNum.Text,textTrojanID.Text,checkShowHidden.Checked,isGetAll);
			List<long> listPlanNums=new List<long>();
			for(int i=0;i<_table.Rows.Count;i++) {
				listPlanNums.Add(PIn.Long(_table.Rows[i]["PlanNum"].ToString()));
			}
			string groupNum="Group#";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
				groupNum="Plan#";
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lans.g("TableInsurancePlans","Employer"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Carrier"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans",groupNum),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Group Name"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Subs"),40);
			gridMain.Columns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				col=new GridColumn(Lan.g("TableCarriers","CDAnet"),50);
				gridMain.Columns.Add(col);
			}
			if(_isTrojanEnabled){ // If trojan bridge exists and is enabled, show TrojanID column in grid
				col=new GridColumn(Lans.g("TableInsurancePlans","TrojanID"),60);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lans.g("TableInsurancePlans","NoBillIns Override"),60);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Get NoBillIns overrides for every plan currently in our grid.
			List<InsPlanPreference> listInsPlanPreferencesNoBillIns=InsPlanPreferences.GetManyForPlanNums(_codeNum,InsPlanPrefFKeyType.ProcCodeNoBillIns,listPlanNums);
			for(int i=0;i<_table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["EmpName"].ToString());
				row.Cells.Add(_table.Rows[i]["CarrierName"].ToString());
				row.Cells.Add(_table.Rows[i]["GroupNum"].ToString());
				row.Cells.Add(_table.Rows[i]["GroupName"].ToString());
				row.Cells.Add(_table.Rows[i]["subscribers"].ToString());
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					row.Cells.Add((_table.Rows[i]["IsCDA"].ToString()=="0")?"":"X");
				}
				if(_isTrojanEnabled) { // If trojan bridge exists and is enabled, show TrojanID column in grid
					row.Cells.Add(_table.Rows[i]["TrojanID"].ToString());
				}
				string noBillInsOverride="";
				InsPlanPreference insPlanPreference=listInsPlanPreferencesNoBillIns.Find(x => x.PlanNum==PIn.Long(_table.Rows[i]["PlanNum"].ToString()));
				if(insPlanPreference==null) {
					noBillInsOverride=""; // If the current insurance plan does not have a NoBillInsOverride, leave the cell blank.
				}
				else {
					noBillInsOverride=PIn.Enum<NoBillInsOverride>(insPlanPreference.ValueString).GetDescription(); // Otherwise, populate with the description of the override
				}
				row.Cells.Add(noBillInsOverride);
				row.Tag=PIn.Long(_table.Rows[i]["PlanNum"].ToString());//Tag the row with the PlanNum to easily get list of selected PlanNums later on
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void radioOrderEmp_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void radioOrderCarrier_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void textEmployer_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textCarrier_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textGroupName_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textGroupNum_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textPlanNum_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textTrojanID_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butBillIns_Click(object sender,EventArgs e) {
			UpsertNoBillInsOverrides(NoBillInsOverride.BillToIns); 
		}

		private void ButDontBill_Click(object sender,EventArgs e) {
			UpsertNoBillInsOverrides(NoBillInsOverride.DoNotUsuallyBillToIns);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			List<long> listPlanNums=gridMain.SelectedTags<long>();//Get list of all the selected insurance PlanNums
			int numSelectedPlans=listPlanNums.Count;
			if(numSelectedPlans==0) { 
				MsgBox.Show(this,"Please select insurance plan(s) first.");
				return;
			}
			string message=$"{Lan.g(this,"You are about to delete the 'Do not usually bill to Ins' override for")} {numSelectedPlans} {Lan.g(this,"insurance plan(s). Are you sure?")}";
			if(!MsgBox.Show(MsgBoxButtons.YesNo,message)) {
				return;
			}
			InsPlanPreferences.DeleteMany(_codeNum,InsPlanPrefFKeyType.ProcCodeNoBillIns,listPlanNums);//Delete all NoBillIns overrides for the selected PlanNums from the db
			FillGrid();
			MsgBox.Show(this,"Done");
		}

		private void UpsertNoBillInsOverrides(NoBillInsOverride noBillInsOverride) { 
			List<long> listPlanNums=gridMain.SelectedTags<long>();//Get list of all the selected insurance PlanNums
			int numSelectedPlans=listPlanNums.Count;
			if(numSelectedPlans==0) {
				MsgBox.Show(this,"Please select insurance plan(s) first.");
				return;
			}
			string message=Lan.g(this,$"You are about to set the 'Do not usually bill to Ins' override to '{noBillInsOverride.GetDescription()}' for")+
				$" {numSelectedPlans} {Lan.g(this,"insurance plan(s). Are you sure?")}";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
				return;
			}
			//Inserts or updates the NoBillIns override for the selected PlanNums. Does not delete. User must click butDelete to delete any overrides. 
			InsPlanPreferences.UpsertMany(_codeNum,InsPlanPrefFKeyType.ProcCodeNoBillIns,listPlanNums,POut.Enum<NoBillInsOverride>(noBillInsOverride));
			FillGrid();
			MsgBox.Show(this,"Done");
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);//Selects all the rows in the grid
		}

		private void butGetAll_Click(object sender,EventArgs e) {
			FillGrid(true);//By default, FillGrid limits number of DataTable rows retrieved to 200, this will retrieve every single DataTable row and display in grid.
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}


















