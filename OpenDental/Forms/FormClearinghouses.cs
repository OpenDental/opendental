using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Net;
using System.Collections.Generic;
using OpenDental.UI;
using System.Globalization;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClearinghouses:FormODBase {
		private bool _listHasChanged;
		private List<Clearinghouse> _listClearinghousesHq;
		/// <summary>List of all clinic-level clearinghouses for the current clinic.</summary>
		private List<Clearinghouse> _listClearinghousesClinics;
		///<summary>List of all clinic-level clearinghouses.</summary>
		private List<Clearinghouse> _listClearinghousesClinicsAll;

		///<summary></summary>
		public FormClearinghouses()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.C(this, new System.Windows.Forms.Control[]
			{
				labelGuide
			});
			Lan.F(this);
		}

		private void FormClearinghouses_Load(object sender, System.EventArgs e) {
			textReportComputerName.Text=PrefC.GetString(PrefName.ClaimReportComputerName);
			int claimReportReceiveInterval=PrefC.GetInt(PrefName.ClaimReportReceiveInterval);
			checkReceiveReportsService.Checked=PrefC.GetBool(PrefName.ClaimReportReceivedByService);
			_listClearinghousesHq=Clearinghouses.GetDeepCopy(true);
			_listClearinghousesClinicsAll=Clearinghouses.GetAllNonHq();
			_listClearinghousesClinics=new List<Clearinghouse>();
			comboClinic.ClinicNumSelected=Clinics.ClinicNum;
			FillGrid();
			if(claimReportReceiveInterval==0) {
				radioTime.Checked=true;
				DateTime fullDateTime=PrefC.GetDateT(PrefName.ClaimReportReceiveTime);
				textReportCheckTime.Text=fullDateTime.ToShortTimeString();
			}
			else {
				textReportCheckInterval.Text=POut.Int(claimReportReceiveInterval);
				radioInterval.Checked=true;
			}
		}

		private void FillGrid(){
			_listClearinghousesClinics.Clear();
			for(int i=0;i<_listClearinghousesClinicsAll.Count;i++) {
				if(_listClearinghousesClinicsAll[i].ClinicNum==comboClinic.ClinicNumSelected) {
					_listClearinghousesClinics.Add(_listClearinghousesClinicsAll[i]);
				}
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Export Path"),230);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Format"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Is Default"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Payors"),80){ IsWidthDynamic=true };//310
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listClearinghousesHq.Count;i++) {
				Clearinghouse[] clearinghouseArrayTags=new Clearinghouse[3];//[0]=clearinghouseHq, [1]=clearinghouseClinic, [2]=clearinghouseCur per ODGridRow
				clearinghouseArrayTags[0]=_listClearinghousesHq[i].Copy(); //clearinghousehq.
				clearinghouseArrayTags[2]=_listClearinghousesHq[i].Copy();//clearinghouseCur. will be clearinghouseHq if clearinghouseClinic doesn't exist.
				for(int j=0;j<_listClearinghousesClinics.Count;j++) {
					if(_listClearinghousesClinics[j].HqClearinghouseNum==_listClearinghousesHq[i].ClearinghouseNum) {
						clearinghouseArrayTags[1]=_listClearinghousesClinics[j];//clearinghouseClin
						clearinghouseArrayTags[2]=Clearinghouses.OverrideFields(_listClearinghousesHq[i],_listClearinghousesClinics[j]);
						break;
					}
				}
				Clearinghouse clearinghouse=clearinghouseArrayTags[2];
				row=new GridRow();
				row.Tag=clearinghouseArrayTags;
				row.Cells.Add(clearinghouse.Description);
				row.Cells.Add(clearinghouse.ExportPath);
				row.Cells.Add(clearinghouse.Eformat.ToString());
				string s="";
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==_listClearinghousesHq[i].ClearinghouseNum) {
					s+="Dent";
				}
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==_listClearinghousesHq[i].ClearinghouseNum) {
					if(s!="") {
						s+=",";
					}
					s+="Med";
				}
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==_listClearinghousesHq[i].ClearinghouseNum) {
					if(s!="") {
						s+=",";
					}
					s+="Elig";
				}
				row.Cells.Add(s);
				row.Cells.Add(clearinghouse.Payors);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Clearinghouse clearinghouseHq=((Clearinghouse[])(gridMain.ListGridRows[e.Row].Tag))[0].Copy();//cannot be null
			using FormClearinghouseEdit formClearinghouseEdit=new FormClearinghouseEdit();
			formClearinghouseEdit.ClearinghouseHq=clearinghouseHq;
			formClearinghouseEdit.ClearinghouseHqOld=clearinghouseHq.Copy(); //cannot be null
			formClearinghouseEdit.ClinicNum=comboClinic.ClinicNumSelected;//_selectedClinicNum;
			formClearinghouseEdit.ListClearinghousesClin=new List<Clearinghouse>();
			formClearinghouseEdit.ListClearinghousesClinOld=new List<Clearinghouse>();
			for(int i=0;i<_listClearinghousesClinicsAll.Count;i++) {
				if(_listClearinghousesClinicsAll[i].HqClearinghouseNum==clearinghouseHq.ClearinghouseNum) {
					formClearinghouseEdit.ListClearinghousesClin.Add(_listClearinghousesClinicsAll[i].Copy());
					formClearinghouseEdit.ListClearinghousesClinOld.Add(_listClearinghousesClinicsAll[i].Copy());
				}
			}
			formClearinghouseEdit.ShowDialog();
			if(formClearinghouseEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formClearinghouseEdit.ClearinghouseCur==null) {//Clearinghouse was deleted.  Can only be deleted when HQ selected.
				_listClearinghousesHq.RemoveAt(e.Row); //no need to update the nonHq list.
			}
			else { //Not deleted.  Both the non-HQ and HQ lists need to be updated.
				_listClearinghousesHq[e.Row]=formClearinghouseEdit.ClearinghouseHq; //update Hq Clearinghouse.
				//Update the clinical clearinghouse list by deleting all of the entries for the selected clearinghouse,
				_listClearinghousesClinicsAll.RemoveAll(x => x.HqClearinghouseNum==clearinghouseHq.ClearinghouseNum);
				//then adding the updated versions back to the list.
				_listClearinghousesClinicsAll.AddRange(formClearinghouseEdit.ListClearinghousesClin);
			}
			_listHasChanged=true;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormClearinghouseEdit formClearingHouseEdit=new FormClearinghouseEdit();
			formClearingHouseEdit.ClearinghouseHq=new Clearinghouse();
			formClearingHouseEdit.ClearinghouseHqOld=new Clearinghouse();
			formClearingHouseEdit.ClinicNum=0;
			formClearingHouseEdit.ListClearinghousesClin=new List<Clearinghouse>();
			formClearingHouseEdit.ListClearinghousesClinOld=new List<Clearinghouse>();
			formClearingHouseEdit.IsNew=true;
			formClearingHouseEdit.ShowDialog();
			if(formClearingHouseEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formClearingHouseEdit.ClearinghouseCur!=null) { //clearinghouse was not deleted
				_listClearinghousesHq.Add(formClearingHouseEdit.ClearinghouseHq.Copy());
				_listClearinghousesClinicsAll.AddRange(formClearingHouseEdit.ListClearinghousesClin);
			}
			_listHasChanged=true;
			FillGrid();
		}

		private void butDefaultDental_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			Clearinghouse ch=_listClearinghousesHq[gridMain.GetSelectedIndex()];
			if(ch.Eformat==ElectronicClaimFormat.x837_5010_med_inst){//med/inst clearinghouse
				MsgBox.Show(this,"The selected clearinghouse must first be set to a dental e-claim format.");
				return;
			}
			bool isInvalid=false;
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==0
				&& Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,ch.ClearinghouseNum)) 
			{
				isInvalid=true;
			}
			if(Prefs.UpdateLong(PrefName.ClearinghouseDefaultDent,ch.ClearinghouseNum)) {
				isInvalid=true;
			}
			if(isInvalid) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butDefaultMedical_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			Clearinghouse clearinghouse=_listClearinghousesHq[gridMain.GetSelectedIndex()];
			if(clearinghouse.Eformat!=ElectronicClaimFormat.x837_5010_med_inst){//anything except the med/inst format
				MsgBox.Show(this,"The selected clearinghouse must first be set to the med/inst e-claim format.");
				return;
			}
			bool isInvalid=false;
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==0
				&& Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,clearinghouse.ClearinghouseNum)) 
			{
				isInvalid=true;
			}
			if(Prefs.UpdateLong(PrefName.ClearinghouseDefaultMed,clearinghouse.ClearinghouseNum)) {
				isInvalid=true;
			}
			if(isInvalid) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butEligibility_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
					MsgBox.Show(this,"Please select a row first.");
					return;
			}
			Clearinghouse clearinghouse=_listClearinghousesHq[gridMain.GetSelectedIndex()];
			if(Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,clearinghouse.ClearinghouseNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void checkReceiveReportsService_CheckedChanged(object sender,EventArgs e) {
			if(checkReceiveReportsService.Checked) {
				textReportComputerName.Enabled=false;
				butThisComputer.Enabled=false;
			}
			else {
				textReportComputerName.Enabled=true;
				butThisComputer.Enabled=true;
			}
		}

		private void radioInterval_CheckedChanged(object sender,EventArgs e) {
			if(radioInterval.Checked) {
				labelReportheckUnits.Enabled=true;
				textReportCheckInterval.Enabled=true;
				textReportCheckTime.Text="";
				textReportCheckTime.Enabled=false;
				textReportCheckTime.ClearError();
				return;
			}
			labelReportheckUnits.Enabled=false;
			textReportCheckInterval.Text="";
			textReportCheckInterval.Enabled=false;
			textReportCheckTime.Enabled=true;
		}

		private void butThisComputer_Click(object sender,EventArgs e) {
			textReportComputerName.Text=Dns.GetHostName();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textReportComputerName.Text.Trim().ToLower()=="localhost" || textReportComputerName.Text.Trim()=="127.0.0.1") {
				MsgBox.Show(this,"Computer name to fetch new reports from cannot be localhost or 127.0.0.1 or any other loopback address.");
				return;
			}
			int reportCheckIntervalMinuteCount=0;
			try {
				reportCheckIntervalMinuteCount=PIn.Int(textReportCheckInterval.Text);//blank=0
			}
			catch {
				MsgBox.Show(this,"Please fix the check interval field.");
				return;
			}
			if(textReportCheckInterval.Enabled && (reportCheckIntervalMinuteCount<5 || reportCheckIntervalMinuteCount>60)) {
				MsgBox.Show(this,"Report check interval must be between 5 and 60 inclusive.");
				return;
			}
			if(radioTime.Checked && (textReportCheckTime.Text=="" || !textReportCheckTime.IsValid())) {
				MsgBox.Show(this,"Please enter a time to receive reports.");
				return;
			}
			bool doRestartToShowChanges=false;
			bool doInvalidateCache=false;
			if(Prefs.UpdateString(PrefName.ClaimReportComputerName,textReportComputerName.Text)) {
				doRestartToShowChanges=true;
				//No point in invalidating prefs since this only affects a workstation on startup.
			}
			if(Prefs.UpdateInt(PrefName.ClaimReportReceiveInterval,reportCheckIntervalMinuteCount)) {
				doInvalidateCache=true;
			}
			if(radioTime.Checked) {
				if(Prefs.UpdateDateT(PrefName.ClaimReportReceiveTime,PIn.DateT(textReportCheckTime.Text))) {
					doInvalidateCache=true;
				}
			}
			else if(textReportCheckTime.Text=="" && Prefs.UpdateDateT(PrefName.ClaimReportReceiveTime,DateTime.MinValue)) {
				doInvalidateCache=true;
			}
			if(Prefs.UpdateBool(PrefName.ClaimReportReceivedByService,checkReceiveReportsService.Checked)) {
				if(checkReceiveReportsService.Checked) {
					doInvalidateCache=true;
				}
				else {
					doRestartToShowChanges=true;
				}
			}
			if(doRestartToShowChanges) {
				MsgBox.Show(this,"You will need to restart the program for changes to take effect.");
			}
			if(doInvalidateCache) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Close();
		}

		private void FormClearinghouses_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==0){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A default clearinghouse should be set. Continue anyway?")){
					e.Cancel=true;
					return;
				}
			}
			if(_listHasChanged) {
				//update all computers including this one:
				DataValid.SetInvalid(InvalidType.ClearHouses);  //This needs to be done before the cache calls below.
			}
			//validate that the default dental clearinghouse is not type mismatched.
			Clearinghouse clearinghouseDent=Clearinghouses.GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
			if(clearinghouseDent!=null) {
				if(clearinghouseDent.Eformat==ElectronicClaimFormat.x837_5010_med_inst) {//mismatch
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The default dental clearinghouse should be set to a dental e-claim format.  Continue anyway?")) {
						e.Cancel=true;
						return;
					}
				}
			}
			//validate medical clearinghouse
			Clearinghouse clearinghouseMed=Clearinghouses.GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultMed));
			if(clearinghouseMed!=null) {
				if(clearinghouseMed.Eformat!=ElectronicClaimFormat.x837_5010_med_inst) {//mismatch
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The default medical clearinghouse should be set to a med/inst e-claim format.  Continue anyway?")) {
						e.Cancel=true;
						return;
					}
				}
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

	}
}