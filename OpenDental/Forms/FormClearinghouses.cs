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
		private bool listHasChanged;
		private List<Clearinghouse> _listClearinghousesHq;
		/// <summary>List of all clinic-level clearinghouses for the current clinic.</summary>
		private List<Clearinghouse> _listClearinghousesClinicCur;
		///<summary>List of all clinic-level clearinghouses.</summary>
		private List<Clearinghouse> _listClearinghousesClinicAll;

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
			_listClearinghousesClinicAll=Clearinghouses.GetAllNonHq();
			_listClearinghousesClinicCur=new List<Clearinghouse>();
			comboClinic.SelectedClinicNum=Clinics.ClinicNum;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				butEligibility.Visible=false;
			}
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
			_listClearinghousesClinicCur.Clear();
			for(int i=0;i<_listClearinghousesClinicAll.Count;i++) {
				if(_listClearinghousesClinicAll[i].ClinicNum==comboClinic.SelectedClinicNum) {
					_listClearinghousesClinicCur.Add(_listClearinghousesClinicAll[i]);
				}
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Export Path"),230);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Format"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Is Default"),60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Payors"),80){ IsWidthDynamic=true };//310
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listClearinghousesHq.Count;i++) {
				Clearinghouse[] listClearinghouseTag=new Clearinghouse[3];//[0]=clearinghouseHq, [1]=clearinghouseClinic, [2]=clearinghouseCur per ODGridRow
				listClearinghouseTag[0]=_listClearinghousesHq[i].Copy(); //clearinghousehq.
				listClearinghouseTag[2]=_listClearinghousesHq[i].Copy();//clearinghouseCur. will be clearinghouseHq if clearinghouseClinic doesn't exist.
				for(int j=0;j<_listClearinghousesClinicCur.Count;j++) {
					if(_listClearinghousesClinicCur[j].HqClearinghouseNum==_listClearinghousesHq[i].ClearinghouseNum) {
						listClearinghouseTag[1]=_listClearinghousesClinicCur[j];//clearinghouseClin
						listClearinghouseTag[2]=Clearinghouses.OverrideFields(_listClearinghousesHq[i],_listClearinghousesClinicCur[j]);
						break;
					}
				}
				Clearinghouse clearinghouseCur=listClearinghouseTag[2];
				row=new GridRow();
				row.Tag=listClearinghouseTag;
				row.Cells.Add(clearinghouseCur.Description);
				row.Cells.Add(clearinghouseCur.ExportPath);
				row.Cells.Add(clearinghouseCur.Eformat.ToString());
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
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==_listClearinghousesHq[i].ClearinghouseNum 
					&& !CultureInfo.CurrentCulture.Name.EndsWith("CA")) //Canadian. en-CA or fr-CA
				{
					if(s!="") {
						s+=",";
					}
					s+="Elig";
				}
				row.Cells.Add(s);
				row.Cells.Add(clearinghouseCur.Payors);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Clearinghouse clearinghouseHq=((Clearinghouse[])(gridMain.ListGridRows[e.Row].Tag))[0].Copy();//cannot be null
			using FormClearinghouseEdit FormCE=new FormClearinghouseEdit();
			FormCE.ClearinghouseHq=clearinghouseHq;
			FormCE.ClearinghouseHqOld=clearinghouseHq.Copy(); //cannot be null
			FormCE.ClinicNum=comboClinic.SelectedClinicNum;//_selectedClinicNum;
			FormCE.ListClearinghousesClinCur=new List<Clearinghouse>();
			FormCE.ListClearinghousesClinOld=new List<Clearinghouse>();
			for(int i=0;i<_listClearinghousesClinicAll.Count;i++) {
				if(_listClearinghousesClinicAll[i].HqClearinghouseNum==clearinghouseHq.ClearinghouseNum) {
					FormCE.ListClearinghousesClinCur.Add(_listClearinghousesClinicAll[i].Copy());
					FormCE.ListClearinghousesClinOld.Add(_listClearinghousesClinicAll[i].Copy());
				}
			}
			FormCE.ShowDialog();
			if(FormCE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormCE.ClearinghouseCur==null) {//Clearinghouse was deleted.  Can only be deleted when HQ selected.
				_listClearinghousesHq.RemoveAt(e.Row); //no need to update the nonHq list.
			}
			else { //Not deleted.  Both the non-HQ and HQ lists need to be updated.
				_listClearinghousesHq[e.Row]=FormCE.ClearinghouseHq; //update Hq Clearinghouse.
				//Update the clinical clearinghouse list by deleting all of the entries for the selected clearinghouse,
				_listClearinghousesClinicAll.RemoveAll(x => x.HqClearinghouseNum==clearinghouseHq.ClearinghouseNum);
				//then adding the updated versions back to the list.
				_listClearinghousesClinicAll.AddRange(FormCE.ListClearinghousesClinCur);
			}
			listHasChanged=true;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormClearinghouseEdit FormCE=new FormClearinghouseEdit();
			FormCE.ClearinghouseHq=new Clearinghouse();
			FormCE.ClearinghouseHqOld=new Clearinghouse();
			FormCE.ClinicNum=0;
			FormCE.ListClearinghousesClinCur=new List<Clearinghouse>();
			FormCE.ListClearinghousesClinOld=new List<Clearinghouse>();
			FormCE.IsNew=true;
			FormCE.ShowDialog();
			if(FormCE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormCE.ClearinghouseCur!=null) { //clearinghouse was not deleted
				_listClearinghousesHq.Add(FormCE.ClearinghouseHq.Copy());
				_listClearinghousesClinicAll.AddRange(FormCE.ListClearinghousesClinCur);
			}
			listHasChanged=true;
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
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") 
				&& PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==0
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
			Clearinghouse ch=_listClearinghousesHq[gridMain.GetSelectedIndex()];
			if(ch.Eformat!=ElectronicClaimFormat.x837_5010_med_inst){//anything except the med/inst format
				MsgBox.Show(this,"The selected clearinghouse must first be set to the med/inst e-claim format.");
				return;
			}
			bool isInvalid=false;
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") 
				&& PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility)==0
				&& Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,ch.ClearinghouseNum)) 
			{
				isInvalid=true;
			}
			if(Prefs.UpdateLong(PrefName.ClearinghouseDefaultMed,ch.ClearinghouseNum)) {
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
			Clearinghouse ch=_listClearinghousesHq[gridMain.GetSelectedIndex()];
			if(Prefs.UpdateLong(PrefName.ClearinghouseDefaultEligibility,ch.ClearinghouseNum)) {
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
			}
			else {
				labelReportheckUnits.Enabled=false;
				textReportCheckInterval.Text="";
				textReportCheckInterval.Enabled=false;
				textReportCheckTime.Enabled=true;
			}
		}

		private void butThisComputer_Click(object sender,EventArgs e) {
			textReportComputerName.Text=Dns.GetHostName();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			if(textReportComputerName.Text.Trim().ToLower()=="localhost" || textReportComputerName.Text.Trim()=="127.0.0.1") {
				MsgBox.Show(this,"Computer name to fetch new reports from cannot be localhost or 127.0.0.1 or any other loopback address.");
				return;
			}
			int reportCheckIntervalMinuteCount=0;
			try {
				reportCheckIntervalMinuteCount=PIn.Int(textReportCheckInterval.Text);
				if(textReportCheckInterval.Enabled && (reportCheckIntervalMinuteCount<5 || reportCheckIntervalMinuteCount>60)) {
					throw new ApplicationException("Invalid value.");//User never sees this message.
				}
			}
			catch {
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
			if(listHasChanged) {
				//update all computers including this one:
				DataValid.SetInvalid(InvalidType.ClearHouses);  //This needs to be done before the cache calls below.
			}
			//validate that the default dental clearinghouse is not type mismatched.
			Clearinghouse chDent=Clearinghouses.GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
			if(chDent!=null) {
				if(chDent.Eformat==ElectronicClaimFormat.x837_5010_med_inst) {//mismatch
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The default dental clearinghouse should be set to a dental e-claim format.  Continue anyway?")) {
						e.Cancel=true;
						return;
					}
				}
			}
			//validate medical clearinghouse
			Clearinghouse chMed=Clearinghouses.GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultMed));
			if(chMed!=null) {
				if(chMed.Eformat!=ElectronicClaimFormat.x837_5010_med_inst) {//mismatch
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





















