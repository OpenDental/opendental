using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormTrojanCollect : FormODBase {
		public Patient PatientCur;
		private Patient _patientGuar;
		private Employer _employer;

		///<summary></summary>
		public FormTrojanCollect() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrojanCollect_Load(object sender,EventArgs e) {
			if(PatientCur==null) {
				MsgBox.Show(this,"Please select a patient first.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_patientGuar=Patients.GetPat(PatientCur.Guarantor);
			if(_patientGuar.EmployerNum>0) {
				_employer=Employers.GetEmployer(_patientGuar.EmployerNum);
			}
			if(_patientGuar.LName.Length==0){
				MsgBox.Show(this,"Missing guarantor last name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patientGuar.FName.Length==0) {
				MsgBox.Show(this,"Missing guarantor first name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Regex.IsMatch(_patientGuar.SSN,@"^\d{9}$")) {
				MsgBox.Show(this,"Guarantor SSN must be exactly 9 digits.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patientGuar.Address.Length==0) {
				MsgBox.Show(this,"Missing guarantor address.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patientGuar.City.Length==0) {
				MsgBox.Show(this,"Missing guarantor city.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patientGuar.State.Length!=2) {
				MsgBox.Show(this,"Guarantor state must be 2 characters.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patientGuar.Zip.Length<5) {
				MsgBox.Show(this,"Invalid guarantor zip.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			labelGuarantor.Text=_patientGuar.GetNameFL();
			labelAddress.Text=_patientGuar.Address;
			if(!string.IsNullOrEmpty(_patientGuar.Address2)){
				labelAddress.Text+=", "+_patientGuar.Address2;
			}
			labelCityStZip.Text=_patientGuar.City+", "+_patientGuar.State+" "+_patientGuar.Zip;
			labelSSN.Text=_patientGuar.SSN.Substring(0,3)+"-"+_patientGuar.SSN.Substring(3,2)+"-"+_patientGuar.SSN.Substring(5,4);
			labelDOB.Text=_patientGuar.Birthdate.Year<1880?"":_patientGuar.Birthdate.ToShortDateString();
			labelPhone.Text=Clip(_patientGuar.HmPhone,13);
			labelEmployer.Text=_employer?.EmpName??"";
			labelEmpPhone.Text=_employer?.Phone??"";
			labelPatient.Text=PatientCur.GetNameFL();
			DateTime dateLastProcedure=TrojanQueries.GetMaxProcedureDate(_patientGuar.PatNum);
			DateTime dateLastPay=TrojanQueries.GetMaxPaymentDate(_patientGuar.PatNum);
			textDate.Text=dateLastProcedure.ToShortDateString();
			if (dateLastPay>dateLastProcedure) {
				textDate.Text=dateLastPay.ToShortDateString();
			}
			textAmount.Text=_patientGuar.BalTotal.ToString("F2");
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormTrojanCollectSetup formTrojanCollectSetup=new FormTrojanCollectSetup();
			if(formTrojanCollectSetup.ShowDialog()==DialogResult.Cancel) {
				return;
			}
			if(!Programs.IsEnabled(ProgramName.TrojanExpressCollect)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>Clips the input string to the specified length.  Also strips out any *, tabs, newlines, etc.  If inputstr is null or empty returns empty string.</summary>
		private string Clip(string inputstr,int length){
			if(string.IsNullOrEmpty(inputstr)) {
				return "";
			}
			string retval=inputstr.Replace("*","").Replace("\r","").Replace("\n","").Replace("\t","");
			if(retval.Length>length){
				retval=retval.Substring(0,length);
			}
			return retval;
		}

		private void butHelp_Click(object sender,EventArgs e) {
			using FormTrojanHelp formTrojanHelp=new FormTrojanHelp();
			formTrojanHelp.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textAmount.IsValid()) {
				MsgBox.Show(this,"Please fix debt amount.");
				return;
			}
			double amtDebt=PIn.Double(textAmount.Text);
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Date is not valid.");
				return;
			}
			DateTime dateDelinquency=PIn.Date(textDate.Text);
			if(dateDelinquency.Year<1950) {
				MessageBox.Show("Date is not valid.");
				return;
			}
			if(dateDelinquency>DateTime.Today) {
				MsgBox.Show(this,"Date cannot be a future date.");
				return;
			}
			long programNum=Programs.GetProgramNum(ProgramName.TrojanExpressCollect);
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(programNum,"Password"));
			if(!Regex.IsMatch(password,@"^[A-Z]{2}\d{4}$")) {
				MsgBox.Show(this,"Password is not in correct format. Must be like this: AB1234");
				return;
			}
			string folderPath=ProgramProperties.GetPropVal(programNum,"FolderPath");
			if(string.IsNullOrEmpty(folderPath)){
				MsgBox.Show(this,"Export folder has not been setup yet.  Please go to Setup at the top of this window.");
				return;
			}
			long billingType=PIn.Long(ProgramProperties.GetPropVal(programNum,"BillingType"));
			if(billingType==0) {
				MsgBox.Show(this,"Billing type has not been setup yet.  Please go to Setup at the top of this window.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!File.Exists(ODFileUtils.CombinePaths(folderPath,"TROBEN.HB"))){
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"The Trojan Communicator is not installed or is not configured for the folder")+": "
					+folderPath+".  "+Lan.g(this,"Please contact Trojan Software Support at 800-451-9723 x1 or x2"));
				return;
			}
			try {
				File.Delete(ODFileUtils.CombinePaths(folderPath,"TROBEN.HB"));
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"There was an error attempting to delete a file from the export folder path.  Check folder permissions and/or try running as administrator.");
				return;
			}
			using FileSystemWatcher fileSystemWatcher=new FileSystemWatcher(folderPath,"TROBEN.HB");
			if(fileSystemWatcher.WaitForChanged(WatcherChangeTypes.Created,10000).TimedOut) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"The Trojan Communicator is not running. Please check it.");
				return;
			}
			StringBuilder stringBuilder_=new StringBuilder();
			if(radioDiplomatic.Checked){
				stringBuilder_.Append("D*");
			}
			else if(radioFirm.Checked) {
				stringBuilder_.Append("F*");
			}
			else if(radioSkip.Checked) {
				stringBuilder_.Append("S*");
			}
			stringBuilder_.Append(Clip(PatientCur.LName,18)+"*");
			stringBuilder_.Append(Clip(PatientCur.FName,18)+"*");
			stringBuilder_.Append(Clip(PatientCur.MiddleI,1)+"*");
			stringBuilder_.Append(Clip(_patientGuar.LName,18)+"*");//validated
			stringBuilder_.Append(Clip(_patientGuar.FName,18)+"*");//validated
			stringBuilder_.Append(Clip(_patientGuar.MiddleI,1)+"*");
			stringBuilder_.Append(_patientGuar.SSN.Substring(0,3)+"-"+_patientGuar.SSN.Substring(3,2)+"-"+_patientGuar.SSN.Substring(5,4)+"*");//validated
			if(_patientGuar.Birthdate.Year>=1880) {
				stringBuilder_.Append(_patientGuar.Birthdate.ToShortDateString());
			}
			stringBuilder_.Append("*");
			stringBuilder_.Append(Clip(_patientGuar.HmPhone,13)+"*");
			stringBuilder_.Append(Clip(_employer?.EmpName,35)+"*");
			stringBuilder_.Append(Clip(_employer?.Phone,13)+"*");
			string address=_patientGuar.Address;//validated
			if(!string.IsNullOrEmpty(_patientGuar.Address2)){
				address+=", "+_patientGuar.Address2;
			}
			stringBuilder_.Append(Clip(address,30)+"*");
			stringBuilder_.Append(Clip(_patientGuar.City,20)+"*");//validated
			stringBuilder_.Append(Clip(_patientGuar.State,2)+"*");//validated
			stringBuilder_.Append(Clip(_patientGuar.Zip,5)+"*");//validated
			stringBuilder_.Append(amtDebt.ToString("F2")+"*");//validated
			stringBuilder_.Append(dateDelinquency.ToShortDateString()+"*");//validated
			stringBuilder_.Append(password+"*");//validated
			stringBuilder_.AppendLine(Clip(Security.CurUser.UserName,25));//There is always a logged in user
			int newFileNum=TrojanQueries.GetUniqueFileNum();
			string outputFile="CT"+newFileNum.ToString().PadLeft(6,'0')+".TRO";
			try {
				File.AppendAllText(ODFileUtils.CombinePaths(folderPath,outputFile),stringBuilder_.ToString());
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"There was an error writing to the export file.  Check folder permissions and/or try running as administrator.");
				return;
			}
			using FileSystemWatcher fileSystemWatcher2=new FileSystemWatcher(folderPath,outputFile);
			if(fileSystemWatcher2.WaitForChanged(WatcherChangeTypes.Deleted,10000).TimedOut) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Warning!! Request was not sent to Trojan within the 10 second limit.");
				return;
			}
			Patients.UpdateFamilyBillingType(billingType,PatientCur.Guarantor);
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
		}
	}
}