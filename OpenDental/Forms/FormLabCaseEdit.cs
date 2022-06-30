using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLabCaseEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public LabCase LabCaseCur;
		private List<Laboratory> _listLaboratories;
		private List<LabTurnaround> _listLabTurnarounds;
		///<summary>The lab slip, if one exists.</summary>
		private Sheet _sheet;
		private List<Provider> _listProviders;
		public List<long> ListProcCodeNums;

		///<summary></summary>
		public FormLabCaseEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLabCaseEdit_Load(object sender, System.EventArgs e) {
			if(Plugins.HookMethod(this,"FormLabCaseEdit.Load_start",LabCaseCur,IsNew)) {
				return;
			}
			if(LabCaseCur==null) {
				MsgBox.Show(this,"Lab case no longer exists.");
				DialogResult=DialogResult.Abort;//Results in form closing logic
				return;
			}
			Patient patient=Patients.GetPat(LabCaseCur.PatNum);
			//If pat is null, this can trickle down and allow the user to create a lab sheet with a FK to an invalid Patient. Only allow user to delete or view lab case.
			if(patient==null) {
				MsgBox.Show(this,"There is no valid Patient attached to this Labcase. Labcase can only be viewed or deleted.");
				DisableAllExcept(butDelete,butCancel,textInstructions);
				textInstructions.ReadOnly=true;//Allows user to scroll and see entire instructions instead of disabling the control which doesn't allow user to scroll.
			}
			else {
				textPatient.Text=patient.GetNameFL();
			}
			_listLaboratories=Laboratories.Refresh();
			//Include the current lab, even if it is hidden.
			_listLaboratories=_listLaboratories.Where(x => x.LaboratoryNum==LabCaseCur.LaboratoryNum || !x.IsHidden).ToList();
			for(int i=0;i<_listLaboratories.Count;i++){
				listLab.Items.Add(_listLaboratories[i].Description+" "+_listLaboratories[i].Phone);
				if(_listLaboratories[i].LaboratoryNum==LabCaseCur.LaboratoryNum){
					listLab.SelectedIndex=i;
				}
			}
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++){
				comboProv.Items.Add(_listProviders[i].Abbr);
				if(_listProviders[i].ProvNum==LabCaseCur.ProvNum){
					comboProv.SelectedIndex=i;
				}
			}
			Appointment appointment=Appointments.GetOneApt(LabCaseCur.AptNum);
			if(appointment!=null){
				if(appointment.AptStatus==ApptStatus.UnschedList){
					textAppointment.Text=Lan.g(this,"Unscheduled");
				}
				else{
					textAppointment.Text=appointment.AptDateTime.ToShortDateString()+" "+appointment.AptDateTime.ToShortTimeString();
				}
				textAppointment.Text+=", "+appointment.ProcDescript;
			}
			appointment=Appointments.GetOneApt(LabCaseCur.PlannedAptNum);
			if(appointment!=null){
				textPlanned.Text=appointment.ProcDescript;
				if(textPlanned.Text==""){
					textPlanned.Text=Lan.g(this,"Attached");
				}
			}
			if(LabCaseCur.DateTimeCreated.Year>1880){
				textDateCreated.Text=LabCaseCur.DateTimeCreated.ToString();
			}
			if(LabCaseCur.DateTimeSent.Year>1880) {
				textDateSent.Text=LabCaseCur.DateTimeSent.ToString();
			}
			if(LabCaseCur.DateTimeRecd.Year>1880) {
				textDateRecd.Text=LabCaseCur.DateTimeRecd.ToString();
			}
			if(LabCaseCur.DateTimeChecked.Year>1880) {
				textDateChecked.Text=LabCaseCur.DateTimeChecked.ToString();
			}
			if(LabCaseCur.DateTimeDue.Year>1880) {
				textDateDue.Text=LabCaseCur.DateTimeDue.ToShortDateString()+" "+LabCaseCur.DateTimeDue.ToShortTimeString();
			}
			textInstructions.Text=LabCaseCur.Instructions;
			textLabFee.Text=LabCaseCur.LabFee.ToString("n");
			_sheet=Sheets.GetLabSlip(LabCaseCur.PatNum,LabCaseCur.LabCaseNum);
			if(_sheet==null) {
				butSlip.Text=Lan.g(this,"New Slip");
			}
			else {
				butSlip.Text=Lan.g(this,"Edit Slip");
			}
			textInvoiceNumber.Text=LabCaseCur.InvoiceNum;
			Plugins.HookAddCode(this,"FormLabCaseEdit.Load_end",LabCaseCur,IsNew);
		}

		private void textDateDue_TextChanged(object sender,EventArgs e) {
			try{
				DateTime date=DateTime.Parse(textDateDue.Text);
				textWeekday.Text=date.ToString("ddd");
			}
			catch{
				textWeekday.Text="";
			}
		}

		private void butDetach_Click(object sender,EventArgs e) {
			LabCaseCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDetachPlanned_Click(object sender,EventArgs e) {
			LabCaseCur.PlannedAptNum=0;
			textPlanned.Text="";
		}

		private void listLab_SelectedIndexChanged(object sender,EventArgs e) {
			if(listLab.SelectedIndex==-1){
				return;
			}
			_listLabTurnarounds=LabTurnarounds.GetForLab(_listLaboratories[listLab.SelectedIndex].LaboratoryNum);
			listTurnaround.Items.Clear();
			for(int i=0;i<_listLabTurnarounds.Count;i++){
				listTurnaround.Items.Add(_listLabTurnarounds[i].Description+", "+_listLabTurnarounds[i].DaysActual.ToString());
			}
		}

		private void listTurnaround_Click(object sender,EventArgs e) {
			if(listTurnaround.SelectedIndex==-1){
				return;
			}
			DateTime duedate=LabTurnarounds.ComputeDueDate
				(MiscData.GetNowDateTime().Date,_listLabTurnarounds[listTurnaround.SelectedIndex].DaysActual);
			textDateDue.Text=duedate.ToShortDateString()+" "+duedate.ToShortTimeString();
			listTurnaround.SelectedIndex=-1;
		}

		private void butCreatedNow_Click(object sender,EventArgs e) {
			textDateCreated.Text=MiscData.GetNowDateTime().ToString();
		}

		private void butSentNow_Click(object sender,EventArgs e) {
			textDateSent.Text=MiscData.GetNowDateTime().ToString();
		}

		private void butRecdNow_Click(object sender,EventArgs e) {
			textDateRecd.Text=MiscData.GetNowDateTime().ToString();
		}

		private void butCheckedNow_Click(object sender,EventArgs e) {
			textDateChecked.Text=MiscData.GetNowDateTime().ToString();
		}

		/*private void buttonEmail_Click(object sender,EventArgs e) {
			int CurPatNum=CaseCur.PatNum;
			EmailMessage message=new EmailMessage();
			message.PatNum=CurPatNum;
			Patient pat=Patients.GetPat(CurPatNum);
			message.ToAddress="";//pat.Email;
			message.FromAddress=PrefC.GetString(PrefName.EmailSenderAddress");
			message.Subject=Lan.g(this,"RE: ")+pat.GetNameFL();
			using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message);
			FormE.IsNew=true;
			FormE.ShowDialog();

		}*/

		private void butSlip_Click(object sender,EventArgs e) {
			if(_sheet==null) {//create new
				if(!SaveToDb()) {
					return;
				}
				Laboratory laboratory=_listLaboratories[listLab.SelectedIndex];
				SheetDef sheetDef;
				if(laboratory.Slip==0){
					sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabSlip);
				}
				else{
					sheetDef=SheetDefs.GetSheetDef(laboratory.Slip);
				}
				_sheet=SheetUtil.CreateSheet(sheetDef,LabCaseCur.PatNum);
				SheetParameter.SetParameter(_sheet,"PatNum",LabCaseCur.PatNum);
				SheetParameter.SetParameter(_sheet,"LabCaseNum",LabCaseCur.LabCaseNum);
				if(ListProcCodeNums!=null) {
					StaticTextData staticTextData=new StaticTextData();
					StaticTextFieldDependency staticTextFieldDependency=StaticTextData.GetStaticTextDependencies(_sheet.SheetFields);
					if(ListProcCodeNums.Count()>0) {
						staticTextFieldDependency|=StaticTextFieldDependency.ListSelectedTpProcs;
					}
					Family family=Patients.GetFamily(LabCaseCur.PatNum);
					Patient patient=Patients.GetPat(LabCaseCur.PatNum);
					staticTextData=StaticTextData.GetStaticTextData(staticTextFieldDependency,patient,family,ListProcCodeNums);
					SheetFiller.FillFields(_sheet,staticTextData: staticTextData);
				}
				else {
					SheetFiller.FillFields(_sheet);
				}
				SheetUtil.CalculateHeights(_sheet);
				using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(_sheet);
				formSheetFillEdit.ShowDialog();
			}
			else {//edit existing
				SheetFields.GetFieldsAndParameters(_sheet);
				using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(_sheet);
				formSheetFillEdit.ShowDialog();
			}
			//refresh
			_sheet=Sheets.GetLabSlip(LabCaseCur.PatNum,LabCaseCur.LabCaseNum);
			if(_sheet==null) {
				butSlip.Text=Lan.g(this,"New Slip");
			}
			else {
				butSlip.Text=Lan.g(this,"Edit Slip");
				butCancel.Enabled=false;//user can still click X to close window, but we do handle that as well.
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//whether new or not
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Lab Case?")){
				return;
			}
			try{
				LabCases.Delete(LabCaseCur.LabCaseNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>Returns false if not able to save.</summary>
		private bool SaveToDb() {
			if(listLab.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a lab first.");
				return false;
			}
			if(comboProv.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a provider first.");
				return false;
			}
			if(textDateCreated.Text!=""){
				try{
					DateTime.Parse(textDateCreated.Text);
				}
				catch{
					MsgBox.Show(this,"Date Time Created is invalid.");
					return false;
				}
			}
			if(textDateSent.Text!="") {
				try {
					DateTime.Parse(textDateSent.Text);
				}
				catch {
					MsgBox.Show(this,"Date Time Sent is invalid.");
					return false;
				}
			}
			if(textDateRecd.Text!="") {
				try {
					DateTime.Parse(textDateRecd.Text);
				}
				catch {
					MsgBox.Show(this,"Date Time Received is invalid.");
					return false;
				}
			}
			if(textDateChecked.Text!="") {
				try {
					DateTime.Parse(textDateChecked.Text);
				}
				catch {
					MsgBox.Show(this,"Date Time Checked is invalid.");
					return false;
				}
			}
			if(textDateDue.Text!="") {
				try {
					DateTime.Parse(textDateDue.Text);
				}
				catch {
					MsgBox.Show(this,"Date Time Due is invalid.");
					return false;
				}
			}
			if(!textLabFee.IsValid()) {
				MsgBox.Show(this,"Lab fee amount is invalid.");
				return false;
			}
			LabCaseCur.LaboratoryNum=_listLaboratories[listLab.SelectedIndex].LaboratoryNum;
			//AptNum
			//PlannedAptNum
			LabCaseCur.ProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			if(textDateCreated.Text==""){
				LabCaseCur.DateTimeCreated=DateTime.MinValue;
			}
			else{
				LabCaseCur.DateTimeCreated=DateTime.Parse(textDateCreated.Text);
			}
			if(textDateSent.Text==""){
				LabCaseCur.DateTimeSent=DateTime.MinValue;
			}
			else{
				LabCaseCur.DateTimeSent=DateTime.Parse(textDateSent.Text);
			}
			if(textDateRecd.Text==""){
				LabCaseCur.DateTimeRecd=DateTime.MinValue;
			}
			else{
				LabCaseCur.DateTimeRecd=DateTime.Parse(textDateRecd.Text);
			}
			if(textDateChecked.Text==""){
				LabCaseCur.DateTimeChecked=DateTime.MinValue;
			}
			else{
				LabCaseCur.DateTimeChecked=DateTime.Parse(textDateChecked.Text);
			}
			if(textDateDue.Text==""){
				LabCaseCur.DateTimeDue=DateTime.MinValue;
			}
			else{
				LabCaseCur.DateTimeDue=DateTime.Parse(textDateDue.Text);
			}
			LabCaseCur.Instructions=textInstructions.Text;
			LabCaseCur.LabFee=PIn.Double(textLabFee.Text);
			LabCaseCur.InvoiceNum=textInvoiceNumber.Text;
			object[] objectArrayParameters= { true };
			Plugins.HookAddCode(this,"FormLabCaseEdit.SaveToDb_update",objectArrayParameters);
			if(!(bool)objectArrayParameters[0]) {
				return false;
			}
			try{
				//if(IsNew){//No.  Always created ahead of time
				LabCases.Update(LabCaseCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!SaveToDb()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormLabCaseEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew) {
				if(_sheet==null) {
					LabCases.Delete(LabCaseCur.LabCaseNum);
				}
				else {//user created and possibly printed a lab slip.  We can't let them delete this lab case
					//lab cases are always created ahead of time, so no need to save here
				}
			}
		}

		

		

		

		

		

		

		

		

		

		

		

		

		


	}
}





















