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
		public LabCase CaseCur;
		private List<Laboratory> ListLabs;
		private List<LabTurnaround> turnaroundList;
		///<summary>The lab slip, if one exists.</summary>
		private Sheet sheet;
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
			if(Plugins.HookMethod(this,"FormLabCaseEdit.Load_start",CaseCur,IsNew)) {
				return;
			}
			if(CaseCur==null) {
				MsgBox.Show(this,"Lab case no longer exists.");
				DialogResult=DialogResult.Abort;//Results in form closing logic
				return;
			}
			Patient pat=Patients.GetPat(CaseCur.PatNum);
			//If pat is null, this can trickle down and allow the user to create a lab sheet with a FK to an invalid Patient.
			if(pat==null) {
				MsgBox.Show(this,"There is no valid Patient attached to this Labcase.");
				DialogResult=DialogResult.Abort;//Results in form closing logic
				return;
			}
			textPatient.Text=pat.GetNameFL();
			ListLabs=Laboratories.Refresh();
			//Include the current lab, even if it is hidden.
			ListLabs=ListLabs.Where(x => x.LaboratoryNum==CaseCur.LaboratoryNum || !x.IsHidden).ToList();
			for(int i=0;i<ListLabs.Count;i++){
				listLab.Items.Add(ListLabs[i].Description+" "+ListLabs[i].Phone);
				if(ListLabs[i].LaboratoryNum==CaseCur.LaboratoryNum){
					listLab.SelectedIndex=i;
				}
			}
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++){
				comboProv.Items.Add(_listProviders[i].Abbr);
				if(_listProviders[i].ProvNum==CaseCur.ProvNum){
					comboProv.SelectedIndex=i;
				}
			}
			Appointment apt=Appointments.GetOneApt(CaseCur.AptNum);
			if(apt!=null){
				if(apt.AptStatus==ApptStatus.UnschedList){
					textAppointment.Text=Lan.g(this,"Unscheduled");
				}
				else{
					textAppointment.Text=apt.AptDateTime.ToShortDateString()+" "+apt.AptDateTime.ToShortTimeString();
				}
				textAppointment.Text+=", "+apt.ProcDescript;
			}
			apt=Appointments.GetOneApt(CaseCur.PlannedAptNum);
			if(apt!=null){
				textPlanned.Text=apt.ProcDescript;
				if(textPlanned.Text==""){
					textPlanned.Text=Lan.g(this,"Attached");
				}
			}
			if(CaseCur.DateTimeCreated.Year>1880){
				textDateCreated.Text=CaseCur.DateTimeCreated.ToString();
			}
			if(CaseCur.DateTimeSent.Year>1880) {
				textDateSent.Text=CaseCur.DateTimeSent.ToString();
			}
			if(CaseCur.DateTimeRecd.Year>1880) {
				textDateRecd.Text=CaseCur.DateTimeRecd.ToString();
			}
			if(CaseCur.DateTimeChecked.Year>1880) {
				textDateChecked.Text=CaseCur.DateTimeChecked.ToString();
			}
			if(CaseCur.DateTimeDue.Year>1880) {
				textDateDue.Text=CaseCur.DateTimeDue.ToShortDateString()+" "+CaseCur.DateTimeDue.ToShortTimeString();
			}
			textInstructions.Text=CaseCur.Instructions;
			textLabFee.Text=CaseCur.LabFee.ToString("n");
			sheet=Sheets.GetLabSlip(CaseCur.PatNum,CaseCur.LabCaseNum);
			if(sheet==null) {
				butSlip.Text=Lan.g(this,"New Slip");
			}
			else {
				butSlip.Text=Lan.g(this,"Edit Slip");
			}
			textInvoiceNumber.Text=CaseCur.InvoiceNum;
			Plugins.HookAddCode(this,"FormLabCaseEdit.Load_end",CaseCur,IsNew);
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
			CaseCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDetachPlanned_Click(object sender,EventArgs e) {
			CaseCur.PlannedAptNum=0;
			textPlanned.Text="";
		}

		private void listLab_SelectedIndexChanged(object sender,EventArgs e) {
			if(listLab.SelectedIndex==-1){
				return;
			}
			turnaroundList=LabTurnarounds.GetForLab(ListLabs[listLab.SelectedIndex].LaboratoryNum);
			listTurnaround.Items.Clear();
			for(int i=0;i<turnaroundList.Count;i++){
				listTurnaround.Items.Add(turnaroundList[i].Description+", "+turnaroundList[i].DaysActual.ToString());
			}
		}

		private void listTurnaround_Click(object sender,EventArgs e) {
			if(listTurnaround.SelectedIndex==-1){
				return;
			}
			DateTime duedate=LabTurnarounds.ComputeDueDate
				(MiscData.GetNowDateTime().Date,turnaroundList[listTurnaround.SelectedIndex].DaysActual);
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
			if(sheet==null) {//create new
				if(!SaveToDb()) {
					return;
				}
				Laboratory lab=ListLabs[listLab.SelectedIndex];
				SheetDef sheetDef;
				if(lab.Slip==0){
					sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabSlip);
				}
				else{
					sheetDef=SheetDefs.GetSheetDef(lab.Slip);
				}
				sheet=SheetUtil.CreateSheet(sheetDef,CaseCur.PatNum);
				SheetParameter.SetParameter(sheet,"PatNum",CaseCur.PatNum);
				SheetParameter.SetParameter(sheet,"LabCaseNum",CaseCur.LabCaseNum);
				if(ListProcCodeNums!=null) {
					StaticTextData data=new StaticTextData();
					StaticTextFieldDependency dependencies=StaticTextData.GetStaticTextDependencies(sheet.SheetFields);
					if(ListProcCodeNums.Count()>0) {
						dependencies|=StaticTextFieldDependency.ListSelectedTpProcs;
					}
					Family _famCur=Patients.GetFamily(CaseCur.PatNum);
					Patient _patCur=Patients.GetPat(CaseCur.PatNum);
					data=StaticTextData.GetStaticTextData(dependencies,_patCur,_famCur,ListProcCodeNums);
					SheetFiller.FillFields(sheet,staticTextData: data);
				}
				else {
					SheetFiller.FillFields(sheet);
				}
				SheetUtil.CalculateHeights(sheet);
				using FormSheetFillEdit FormS=new FormSheetFillEdit(sheet);
				FormS.ShowDialog();
			}
			else {//edit existing
				SheetFields.GetFieldsAndParameters(sheet);
				using FormSheetFillEdit FormS=new FormSheetFillEdit(sheet);
				FormS.ShowDialog();
			}
			//refresh
			sheet=Sheets.GetLabSlip(CaseCur.PatNum,CaseCur.LabCaseNum);
			if(sheet==null) {
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
				LabCases.Delete(CaseCur.LabCaseNum);
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
			CaseCur.LaboratoryNum=ListLabs[listLab.SelectedIndex].LaboratoryNum;
			//AptNum
			//PlannedAptNum
			CaseCur.ProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			if(textDateCreated.Text==""){
				CaseCur.DateTimeCreated=DateTime.MinValue;
			}
			else{
				CaseCur.DateTimeCreated=DateTime.Parse(textDateCreated.Text);
			}
			if(textDateSent.Text==""){
				CaseCur.DateTimeSent=DateTime.MinValue;
			}
			else{
				CaseCur.DateTimeSent=DateTime.Parse(textDateSent.Text);
			}
			if(textDateRecd.Text==""){
				CaseCur.DateTimeRecd=DateTime.MinValue;
			}
			else{
				CaseCur.DateTimeRecd=DateTime.Parse(textDateRecd.Text);
			}
			if(textDateChecked.Text==""){
				CaseCur.DateTimeChecked=DateTime.MinValue;
			}
			else{
				CaseCur.DateTimeChecked=DateTime.Parse(textDateChecked.Text);
			}
			if(textDateDue.Text==""){
				CaseCur.DateTimeDue=DateTime.MinValue;
			}
			else{
				CaseCur.DateTimeDue=DateTime.Parse(textDateDue.Text);
			}
			CaseCur.Instructions=textInstructions.Text;
			CaseCur.LabFee=PIn.Double(textLabFee.Text);
			CaseCur.InvoiceNum=textInvoiceNumber.Text;
			object[] parameters= { true };
			Plugins.HookAddCode(this,"FormLabCaseEdit.SaveToDb_update",parameters);
			if(!(bool)parameters[0]) {
				return false;
			}
			try{
				//if(IsNew){//No.  Always created ahead of time
				LabCases.Update(CaseCur);
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
				if(sheet==null) {
					LabCases.Delete(CaseCur.LabCaseNum);
				}
				else {//user created and possibly printed a lab slip.  We can't let them delete this lab case
					//lab cases are always created ahead of time, so no need to save here
				}
			}
		}

		

		

		

		

		

		

		

		

		

		

		

		

		


	}
}





















