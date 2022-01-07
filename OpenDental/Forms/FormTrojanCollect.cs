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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTrojanCollect : FormODBase {
		private Patient _patCur;
		private Patient _guarCur;
		private Employer _empCur;

		///<summary></summary>
		public FormTrojanCollect(Patient pat) {
			_patCur=pat;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrojanCollect_Load(object sender,EventArgs e) {
			if(_patCur==null) {
				MsgBox.Show(this,"Please select a patient first.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_guarCur=Patients.GetPat(_patCur.Guarantor);
			if(_guarCur.EmployerNum>0) {
				_empCur=Employers.GetEmployer(_guarCur.EmployerNum);
			}
			if(_guarCur.LName.Length==0){
				MsgBox.Show(this,"Missing guarantor last name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.FName.Length==0) {
				MsgBox.Show(this,"Missing guarantor first name.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Regex.IsMatch(_guarCur.SSN,@"^\d{9}$")) {
				MsgBox.Show(this,"Guarantor SSN must be exactly 9 digits.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.Address.Length==0) {
				MsgBox.Show(this,"Missing guarantor address.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.City.Length==0) {
				MsgBox.Show(this,"Missing guarantor city.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.State.Length!=2) {
				MsgBox.Show(this,"Guarantor state must be 2 characters.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_guarCur.Zip.Length<5) {
				MsgBox.Show(this,"Invalid guarantor zip.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			labelGuarantor.Text=_guarCur.GetNameFL();
			labelAddress.Text=_guarCur.Address;
			if(!string.IsNullOrEmpty(_guarCur.Address2)){
				labelAddress.Text+=", "+_guarCur.Address2;
			}
			labelCityStZip.Text=_guarCur.City+", "+_guarCur.State+" "+_guarCur.Zip;
			labelSSN.Text=_guarCur.SSN.Substring(0,3)+"-"+_guarCur.SSN.Substring(3,2)+"-"+_guarCur.SSN.Substring(5,4);
			labelDOB.Text=_guarCur.Birthdate.Year<1880?"":_guarCur.Birthdate.ToShortDateString();
			labelPhone.Text=Clip(_guarCur.HmPhone,13);
			labelEmployer.Text=_empCur?.EmpName??"";
			labelEmpPhone.Text=_empCur?.Phone??"";
			labelPatient.Text=_patCur.GetNameFL();
			DateTime lastProcDate=TrojanQueries.GetMaxProcedureDate(_guarCur.PatNum);
			DateTime lastPayDate=TrojanQueries.GetMaxPaymentDate(_guarCur.PatNum);
			textDate.Text=(lastPayDate>lastProcDate?lastPayDate:lastProcDate).ToShortDateString();
			textAmount.Text=_guarCur.BalTotal.ToString("F2");
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using(FormTrojanCollectSetup FormT=new FormTrojanCollectSetup()) {
				if(FormT.ShowDialog()==DialogResult.Cancel) {
					return;
				}
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
			using(FormTrojanHelp FormH=new FormTrojanHelp()) {
				FormH.ShowDialog();
			}
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
			using(FileSystemWatcher watcher=new FileSystemWatcher(folderPath,"TROBEN.HB")) {
				if(watcher.WaitForChanged(WatcherChangeTypes.Created,10000).TimedOut) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"The Trojan Communicator is not running. Please check it.");
					return;
				}
			}
			StringBuilder str=new StringBuilder();
			if(radioDiplomatic.Checked){
				str.Append("D*");
			}
			else if(radioFirm.Checked) {
				str.Append("F*");
			}
			else if(radioSkip.Checked) {
				str.Append("S*");
			}
			str.Append(Clip(_patCur.LName,18)+"*");
			str.Append(Clip(_patCur.FName,18)+"*");
			str.Append(Clip(_patCur.MiddleI,1)+"*");
			str.Append(Clip(_guarCur.LName,18)+"*");//validated
			str.Append(Clip(_guarCur.FName,18)+"*");//validated
			str.Append(Clip(_guarCur.MiddleI,1)+"*");
			str.Append(_guarCur.SSN.Substring(0,3)+"-"+_guarCur.SSN.Substring(3,2)+"-"+_guarCur.SSN.Substring(5,4)+"*");//validated
			if(_guarCur.Birthdate.Year>=1880) {
				str.Append(_guarCur.Birthdate.ToShortDateString());
			}
			str.Append("*");
			str.Append(Clip(_guarCur.HmPhone,13)+"*");
			str.Append(Clip(_empCur?.EmpName,35)+"*");
			str.Append(Clip(_empCur?.Phone,13)+"*");
			string address=_guarCur.Address;//validated
			if(!string.IsNullOrEmpty(_guarCur.Address2)){
				address+=", "+_guarCur.Address2;
			}
			str.Append(Clip(address,30)+"*");
			str.Append(Clip(_guarCur.City,20)+"*");//validated
			str.Append(Clip(_guarCur.State,2)+"*");//validated
			str.Append(Clip(_guarCur.Zip,5)+"*");//validated
			str.Append(amtDebt.ToString("F2")+"*");//validated
			str.Append(dateDelinquency.ToShortDateString()+"*");//validated
			str.Append(password+"*");//validated
			str.AppendLine(Clip(Security.CurUser.UserName,25));//There is always a logged in user
			int thisNum=TrojanQueries.GetUniqueFileNum();
			string outputFile="CT"+thisNum.ToString().PadLeft(6,'0')+".TRO";
			try {
				File.AppendAllText(ODFileUtils.CombinePaths(folderPath,outputFile),str.ToString());
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"There was an error writing to the export file.  Check folder permissions and/or try running as administrator.");
				return;
			}
			using(FileSystemWatcher watcher=new FileSystemWatcher(folderPath,outputFile)) {
				if(watcher.WaitForChanged(WatcherChangeTypes.Deleted,10000).TimedOut) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Warning!! Request was not sent to Trojan within the 10 second limit.");
					return;
				}
			}
			Patients.UpdateFamilyBillingType(billingType,_patCur.Guarantor);
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

	}
}





















