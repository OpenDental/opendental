using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormScreenEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public OpenDentBusiness.Screen ScreenCur;
		public ScreenGroup ScreenGroupCur;
		public ScreenPat ScreenPatCur;
		public bool IsDeleted;
		private bool _isValid;

		///<summary></summary>
		public FormScreenEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormScreenEdit_Load(object sender, System.EventArgs e) {
			textScreenGroupOrder.Text=ScreenCur.ScreenGroupOrder.ToString();
			switch(ScreenCur.Gender){
				case PatientGender.Unknown:
					radioUnknown.Checked=true;
					break;
				case PatientGender.Male:
					radioM.Checked=true;
					break;
				case PatientGender.Female:
					radioF.Checked=true;
					break;
			}
			listRace.Items.Clear();
			listRace.Items.AddEnums<PatientRaceOld>();
			listRace.SetSelectedEnum(ScreenCur.RaceOld);
			comboGradeLevel.Items.Clear();
			comboGradeLevel.Items.AddRange(Enum.GetNames(typeof(PatientGrade)));
			comboGradeLevel.SelectedIndex=(int)ScreenCur.GradeLevel;
			ArrayList items=new ArrayList();
			if(ScreenCur.Age==0)
				textAge.Text="";
			else
				textAge.Text=ScreenCur.Age.ToString();
			listUrgency.Items.Clear();
			listUrgency.Items.AddEnums<TreatmentUrgency>();
			listUrgency.SetSelectedEnum(ScreenCur.Urgency);
			SetCheckState(checkHasCaries,ScreenCur.HasCaries);
			SetCheckState(checkNeedsSealants,ScreenCur.NeedsSealants);
			SetCheckState(checkCariesExperience,ScreenCur.CariesExperience);
			SetCheckState(checkEarlyChildCaries,ScreenCur.EarlyChildCaries);
			SetCheckState(checkExistingSealants,ScreenCur.ExistingSealants);
			SetCheckState(checkMissingAllTeeth,ScreenCur.MissingAllTeeth);
			if(ScreenCur.Birthdate.Year<1880)
				textBirthdate.Text="";
			else
				textBirthdate.Text=ScreenCur.Birthdate.ToShortDateString();
			textComments.Text=ScreenCur.Comments;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkCariesExperience.Visible=false;
				checkEarlyChildCaries.Visible=false;
				checkExistingSealants.Visible=false;
				checkMissingAllTeeth.Visible=false;
				checkNeedsSealants.Visible=false;
				checkHasCaries.Visible=false;
				checkCariesExperience.CheckState=CheckState.Unchecked;
				checkEarlyChildCaries.CheckState=CheckState.Unchecked;
				checkExistingSealants.CheckState=CheckState.Unchecked;
				checkMissingAllTeeth.CheckState=CheckState.Unchecked;
				checkNeedsSealants.CheckState=CheckState.Unchecked;
				checkHasCaries.CheckState=CheckState.Unchecked;
			}
			if(ScreenPatCur==null) {
				textName.Text="Anonymous";
				ScreenCur.ScreenPatNum=0;
			}
			else {
				textName.Text=Patients.GetPat(ScreenPatCur.PatNum).GetNameLF();
				ScreenCur.ScreenPatNum=ScreenPatCur.ScreenPatNum;
			}
			_isValid=true;
		}

		private void SetCheckState(CheckBox checkBox,YN state){
			switch(state){
				case YN.Unknown:
					checkBox.CheckState=CheckState.Indeterminate;
					break;
				case YN.Yes:
					checkBox.CheckState=CheckState.Checked;
					break;
				case YN.No:
					checkBox.CheckState=CheckState.Unchecked;
					break;
			}
		}

		private void textScreenGroupOrder_Validating(object sender,System.ComponentModel.CancelEventArgs e){
			try{
				Convert.ToInt32(textScreenGroupOrder.Text);
			}
			catch{
				MessageBox.Show("Order invalid.");
				_isValid=false;
				e.Cancel=true;
			}
		}

		private void textAge_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(textAge.Text=="")
				return;
			if(textAge.Text=="0"){
				textAge.Text="";
				return;
			}
			try{
				Convert.ToInt32(textAge.Text);
			}
			catch{
				MessageBox.Show("Age invalid.");
				_isValid=false;
				e.Cancel=true;
			}
		}

		private void updownAgeArrows_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			//this is necessary because Microsoft's updown control is too buggy to be useful
			int currentValue=0;
			try{
				currentValue=PIn.Int(textAge.Text);
			}
			catch{
				return;
			}
			if(e.Y<8){//up
				textAge.Text=(currentValue+1).ToString();
			}
			else{//down
				if(textAge.Text=="")
					return;
				if(textAge.Text=="1"){
					textAge.Text="";
					return;
				}
				textAge.Text=(currentValue-1).ToString();
			}
		}

		private void textBirthdate_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(textBirthdate.Text=="")
				return;
			try{
				DateTime.Parse(textBirthdate.Text);
			}
			catch{
				MessageBox.Show("Birthdate invalid.");
				_isValid=false;
				e.Cancel=true;
			}
		}

		///<summary>Used by all 6 checkboxes to customize order of 3-state checking.</summary>
		private void checkBox_Click(object sender, System.EventArgs e) {
			switch(((CheckBox)sender).CheckState){
				case CheckState.Indeterminate:
					((CheckBox)sender).CheckState=CheckState.Checked;
					break;
				case CheckState.Checked:
					((CheckBox)sender).CheckState=CheckState.Unchecked;
					break;
				case CheckState.Unchecked:
					((CheckBox)sender).CheckState=CheckState.Indeterminate;
					break;
			}
		}

		private YN GetCheckState(CheckBox checkBox){
			switch(checkBox.CheckState){
				case CheckState.Indeterminate:
					return YN.Unknown;
				case CheckState.Checked:
					return YN.Yes;
				case CheckState.Unchecked:
					return YN.No;
			}
			return YN.Unknown;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				Screens.Delete(ScreenCur);
				DialogResult=DialogResult.Cancel;
				if(ScreenPatCur!=null) {
					ScreenPatCur.PatScreenPerm=PatScreenPerm.Unknown;
				}
				IsDeleted=true;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this screening?")){
				return;
			}
			if(ScreenPatCur!=null) {
				ScreenPatCur.PatScreenPerm=PatScreenPerm.Unknown;
			}
			IsDeleted=true;
			Screens.Delete(ScreenCur);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//the first 6 fields are handled when the ScreenGroup is saved.
			if(!_isValid) {//If validation failed and they still want to continue, do Cancel instead.
				DialogResult=DialogResult.Cancel;
				return;
			}
			ScreenCur.ScreenGroupOrder=PIn.Int(textScreenGroupOrder.Text);
			ScreenCur.ScreenGroupNum=ScreenGroupCur.ScreenGroupNum;
			if(radioUnknown.Checked) {
				ScreenCur.Gender=PatientGender.Unknown;
			}
			else if(radioM.Checked) {
				ScreenCur.Gender=PatientGender.Male;
			}
			else if(radioF.Checked) {
				ScreenCur.Gender=PatientGender.Female;
			}
			ScreenCur.RaceOld=listRace.GetSelected<PatientRaceOld>();
			ScreenCur.GradeLevel=(PatientGrade)comboGradeLevel.SelectedIndex;
			if(textBirthdate.Text!="" && textAge.Text=="") {//Birthdate is present but age isn't entered, calculate it.
				ScreenCur.Age=PIn.Byte(Patients.DateToAge(PIn.DateT(textBirthdate.Text)).ToString());
			}
			else if(textAge.Text!="") {//Age was manually entered, use it.
				ScreenCur.Age=PIn.Byte(textAge.Text);
			}
			else {//No age information was entered at all.
				ScreenCur.Age=0;
			}
			ScreenCur.Urgency=listUrgency.GetSelected<TreatmentUrgency>();
			ScreenCur.HasCaries=GetCheckState(checkHasCaries);
			ScreenCur.NeedsSealants=GetCheckState(checkNeedsSealants);
			ScreenCur.CariesExperience=GetCheckState(checkCariesExperience);
			ScreenCur.EarlyChildCaries=GetCheckState(checkEarlyChildCaries);
			ScreenCur.ExistingSealants=GetCheckState(checkExistingSealants);
			ScreenCur.MissingAllTeeth=GetCheckState(checkMissingAllTeeth);
			ScreenCur.Birthdate=PIn.Date(textBirthdate.Text);//"" is OK
			ScreenCur.Comments=textComments.Text;
			if(IsNew) {
				Screens.Insert(ScreenCur);
			}
			else {
				Screens.Update(ScreenCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		

		

		

		

		

		

		

		

		


		

		

		


	}
}





















