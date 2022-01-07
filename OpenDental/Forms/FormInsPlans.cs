/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormInsPlans:FormODBase {
		//private InsTemplates InsTemplates;
		//<summary>Set to true if we are only using the list to select a template to link to rather than creating a new plan. If this is true, then IsSelectMode will be ignored.</summary>
		//public bool IsLinkMode;
		///<summary>Set to true when selecting a plan for a patient and we want SelectedPlan to be filled upon closing.</summary>
		public bool IsSelectMode;
		///<summary>After closing this form, if IsSelectMode, then this will contain the selected Plan.</summary>
		public InsPlan SelectedPlan;
		//private InsPlan[] ListAll;
		///<summary>Supply a string here to start off the search with filtered employers.</summary>
		public string empText;
		///<summary>Supply a string here to start off the search with filtered carriers.</summary>
		public string carrierText;
		private DataTable table;
		private bool trojan;
		///<summary>Supply a string here to start off the search with filtered group names.</summary>
		public string groupNameText;

		///<summary>Supply a string here to start off the search with filtered group nums.</summary>
		public string groupNumText;

		///<summary></summary>
		public FormInsPlans(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsTemplates_Load(object sender, System.EventArgs e) {
			labelGroupNum.Text=Lan.g(this,CultureInfo.CurrentCulture.Name.EndsWith("CA")?"Plan Number":"Group Num");//USA uses "Group Num", Canada uses "Plan Number"
			if(!IsSelectMode){
				butCancel.Text=Lan.g(this,"Close");
				butOK.Visible=false;
			}
			Program prog=Programs.GetCur(ProgramName.Trojan);
			if(prog!=null && prog.Enabled) {
				trojan=true;
			}
			else{
				labelTrojanID.Visible=false;
				textTrojanID.Visible=false;
			}
			textEmployer.Text=empText;
			textCarrier.Text=carrierText;
			textGroupName.Text=groupNameText;
			textGroupNum.Text=groupNumText;
			FillGrid();
		}

		private void FillGrid(bool isGetAll=false){
			Cursor=Cursors.WaitCursor;
			table=InsPlans.GetBigList(radioOrderEmp.Checked,textEmployer.Text,textCarrier.Text,
				textGroupName.Text,textGroupNum.Text,textPlanNum.Text,textTrojanID.Text,checkShowHidden.Checked,isGetAll);
			if(IsSelectMode){
				butBlank.Visible=true;
				butBlank.Enabled=Security.IsAuthorized(Permissions.InsPlanEdit,true);
			}
			int selectedRow;//preserves the selected row.
			if(gridMain.SelectedIndices.Length==1){
				selectedRow=gridMain.SelectedIndices[0];
			}
			else{
				selectedRow=-1;
			}
			string groupNum=CultureInfo.CurrentCulture.Name.EndsWith("CA")?"Plan#":"Group#";//USA uses Group Num, Canada uses Plan Num
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lans.g("TableInsurancePlans","Employer"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Carrier"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Phone"),82);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Address"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","City"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","ST"),25);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Zip"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans",groupNum),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Group Name"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","noE"),35);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","ElectID"),45);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Subs"),40);
			gridMain.ListGridColumns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				col=new GridColumn(Lan.g("TableCarriers","CDAnet"),50);
				gridMain.ListGridColumns.Add(col);
			}
			if(trojan){
				col=new GridColumn(Lans.g("TableInsurancePlans","TrojanID"),60);
				gridMain.ListGridColumns.Add(col);
			}
			//PlanNote not shown
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Carrier carrier;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["EmpName"].ToString());
				row.Cells.Add(table.Rows[i]["CarrierName"].ToString());
				row.Cells.Add(table.Rows[i]["Phone"].ToString());
				row.Cells.Add(table.Rows[i]["Address"].ToString());
				row.Cells.Add(table.Rows[i]["City"].ToString());
				row.Cells.Add(table.Rows[i]["State"].ToString());
				row.Cells.Add(table.Rows[i]["Zip"].ToString());
				row.Cells.Add(table.Rows[i]["GroupNum"].ToString());
				row.Cells.Add(table.Rows[i]["GroupName"].ToString());
				row.Cells.Add(table.Rows[i]["noSendElect"].ToString());
				row.Cells.Add(table.Rows[i]["ElectID"].ToString());
				row.Cells.Add(table.Rows[i]["subscribers"].ToString());
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					row.Cells.Add((table.Rows[i]["IsCDA"].ToString()=="0")?"":"X");
				}
				if(trojan){
					row.Cells.Add(table.Rows[i]["TrojanID"].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedRow,true);
			Cursor=Cursors.Default;
		}

		private bool InsPlanExists(InsPlan plan) {
			if(plan==null || plan.PlanNum==0) {
				MsgBox.Show(this,"Insurance plan selected no longer exists.");
				FillGrid();
				return false;
			}
			return true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e){
			InsPlan plan=InsPlans.GetPlan(PIn.Long(table.Rows[e.Row]["PlanNum"].ToString()),null);
			if(!InsPlanExists(plan)) {
				return;
			}
			if(IsSelectMode) {
				SelectedPlan=plan.Copy();
				DialogResult=DialogResult.OK;
				return;
			}
			using FormInsPlan FormIP=new FormInsPlan(plan,null,null);
			FormIP.ShowDialog();
			if(FormIP.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
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
		
		private void butGetAll_Click(object sender,EventArgs e) {
			FillGrid(true);
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanMerge)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<2) {
				MessageBox.Show(Lan.g(this,"Please select at least two items first."));
				return;
			}
			InsPlan[] arrayPlansSelected=new InsPlan[gridMain.SelectedIndices.Length];
			for(int i=0;i<arrayPlansSelected.Length;i++){
				arrayPlansSelected[i]=InsPlans.GetPlan(PIn.Long(table.Rows[gridMain.SelectedIndices[i]]["PlanNum"].ToString()),null);
				arrayPlansSelected[i].NumberSubscribers=PIn.Int(table.Rows[gridMain.SelectedIndices[i]]["subscribers"].ToString());
			}
			using FormInsPlansMerge FormI=new FormInsPlansMerge();
			FormI.ListAll=arrayPlansSelected;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK){
				return;
			}
			//If bluebook is on and the select plan to merge into is not bluebook eligible, we might need to warn the user if there are bluebook eligible plans
			//selected to merge that will lose their data.
			bool isBluebookOn=PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook;
			int planCount=GetInsBlueBookCount(FormI.PlanToMergeTo,arrayPlansSelected);
			if(isBluebookOn && planCount > 0) {//Make sure user wants to merge selected plans that are Category Percentage with no fee schedule attached, if applicable.
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,Lan.g(this,"Plans that have 'Use Blue Book' checked and a PlanType of Category Percentage that are merged into "
					+ "a plan that has a Fee Schedule or a different PlanType will no longer be eligible for Blue Book estimates and will have their Blue Book "
					+ $"data deleted. Blue Book data will be deleted for {planCount} of the selected plans. Would you like to continue?")))
				{
					return;
				}
			}
			//Do the merge.
			InsPlan planToMergeTo=FormI.PlanToMergeTo.Copy();
			//List<Benefit> benList=Benefits.RefreshForPlan(planToMergeTo,0);
			Cursor=Cursors.WaitCursor;
			bool didMerge=false;
			List<long> listMergedPlanNums=new List<long>();
			for(int i=0;i<arrayPlansSelected.Length;i++){//loop through each selected plan
				//skip the planToMergeTo, because it's already correct
				if(planToMergeTo.PlanNum==arrayPlansSelected[i].PlanNum){
					continue;
				}
				//==Michael - We are changing plans here, but not carriers, so this is not needed:
				//SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeCarrierName
				InsPlans.ChangeReferences(arrayPlansSelected[i].PlanNum,planToMergeTo);
				Benefits.DeleteForPlan(arrayPlansSelected[i].PlanNum);
				try {
					InsPlans.Delete(arrayPlansSelected[i],canDeleteInsSub:false);
				}
				catch (ApplicationException ex){
					MessageBox.Show(ex.Message);
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,0,
						Lan.g(this,"InsPlan Combine delete validation failed.  Plan was not deleted."),
						arrayPlansSelected[i].PlanNum,arrayPlansSelected[i].SecDateTEdit); //new plan, no date needed.
					//Since we already deleted/changed all of the other dependencies, 
					//we should continue in making the Securitylog entry and cleaning up. 
				}
				didMerge=true;
				listMergedPlanNums.Add(arrayPlansSelected[i].PlanNum);
				//for(int j=0;j<planNums.Count;j++) {
					//InsPlans.ComputeEstimatesForPlan(planNums[j]);
					//Eliminated in 5.0 for speed.
				//}
			}
			if(didMerge) {
				string logText=Lan.g(this,"Merged the following PlanNum(s): ")+string.Join(", ",listMergedPlanNums)+" "+Lan.g(this,"into")+" "+planToMergeTo.PlanNum;
				SecurityLogs.MakeLogEntry(Permissions.InsPlanMerge,0,logText);
			}
			FillGrid();
			//highlight the merged plan
			for(int i=0;i<table.Rows.Count;i++){
				for(int j=0;j<arrayPlansSelected.Length;j++){
					if(table.Rows[i]["PlanNum"].ToString()==arrayPlansSelected[j].PlanNum.ToString()){
						gridMain.SetSelected(i,true);
					}
				}
			}
			Cursor=Cursors.Default;
		}

		///<summary>Returns the number of plans for which insbluebook entries will be deleted.</summary>
		private int GetInsBlueBookCount(InsPlan planToMergeTo,InsPlan[] arrayPlansSelected) {
			int planCount=0;
			if(planToMergeTo.IsBlueBookEnabled && planToMergeTo.PlanType=="") {//Plan to merge to is blue book eligible, so no insbluebooks will be deleted.
				return planCount;
			}
			for(int i=0;i<arrayPlansSelected.Length;i++) {
				if(arrayPlansSelected[i].PlanType=="" && arrayPlansSelected[i].IsBlueBookEnabled) {//This will exclude the planToMergeTo which is in the array.
					planCount+=1;
				}
			}
			return planCount;
		}

		private void butBlank_Click(object sender, System.EventArgs e) {
			//this button is normally not visible.  It's only set visible when IsSelectMode.
			SelectedPlan=new InsPlan();
			DialogResult=DialogResult.OK;
		}

		private void butHide_Click(object sender,EventArgs e) {
			int unusedCount=InsPlans.UnusedGetCount();
			if(unusedCount==0) {
				MsgBox.Show(this,"All plans are in use.");
				return;
			}
			string msgText=unusedCount.ToString()+" "+Lan.g(this,"plans found that are not in use by any subscribers.  Hide all of them?");
			if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes){
				return;
			}
			InsPlans.UnusedHideAll();
			FillGrid();
			MsgBox.Show(this,"Done.");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//only visible if IsSelectMode
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(gridMain.SelectedIndices.Length>1) {
				MessageBox.Show(Lan.g(this,"Please select only one item first."));
				return;
			}
			InsPlan plan=InsPlans.GetPlan(PIn.Long(table.Rows[gridMain.SelectedIndices[0]]["PlanNum"].ToString()),null);
			if(!InsPlanExists(plan)) {
				return;
			}
			SelectedPlan=plan;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}


















