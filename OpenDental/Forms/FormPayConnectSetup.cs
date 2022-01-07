using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using Ionic.Zip;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPayConnectSetup : FormODBase {
		private Program _progCur;
		///<summary>Local cache of all of the clinic nums the current user has permission to access at the time the form loads.  Filled at the same time
		///as comboClinic and is used to set programproperty.ClinicNum when saving.</summary>
		private List<long> _listUserClinicNums;
		///<summary>List of PayConnect program properties for all clinics.
		///Includes properties with ClinicNum=0, the headquarters props or props not assigned to a clinic.</summary>
		private List<ProgramProperty> _listProgProps;
		private List<ProgramProperty> _listXWebWebPayProgProps=new List<ProgramProperty>();
		///<summary>Used to revert the slected index in the clinic drop down box if the user tries to change clinics
		///and the payment type has not been set.</summary>
		private int _indexClinicRevert;
		private List<Def> _listPaymentTypeDefs;

		///<summary></summary>
		public FormPayConnectSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayConnectSetup_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.PayConnect);
			if(_progCur==null) {
				MsgBox.Show(this,"The PayConnect entry is missing from the database.");//should never happen
				return;
			}
			checkEnabled.Checked=_progCur.Enabled;
			if(!PrefC.HasClinicsEnabled) {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				checkEnabled.Text=Lan.g(this,"Enabled");
				groupPaySettings.Text=Lan.g(this,"Payment Settings");
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				labelClinicEnable.Visible=false;
				_listUserClinicNums=new List<long>() { 0 };//if clinics are disabled, programproperty.ClinicNum will be set to 0
			}
			else {//Using clinics
				groupPaySettings.Text=Lan.g(this,"Clinic Payment Settings");
				_listUserClinicNums=new List<long>();
				comboClinic.Items.Clear();
				//if PayConnect is enabled and the user is restricted to a clinic, don't allow the user to disable for all clinics
				if(Security.CurUser.ClinicIsRestricted) {
					if(checkEnabled.Checked) {
						checkEnabled.Enabled=false;
					}
				}
				else {
					comboClinic.Items.Add(Lan.g(this,"Headquarters"));
					//this way both lists have the same number of items in it and if 'Headquarters' is selected the programproperty.ClinicNum will be set to 0
					_listUserClinicNums.Add(0);
					comboClinic.SelectedIndex=0;
				}
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
				for(int i=0;i<listClinics.Count;i++) {
					comboClinic.Items.Add(listClinics[i].Abbr);
					_listUserClinicNums.Add(listClinics[i].ClinicNum);
					if(Clinics.ClinicNum==listClinics[i].ClinicNum) {
						comboClinic.SelectedIndex=i;
						if(!Security.CurUser.ClinicIsRestricted) {
							comboClinic.SelectedIndex++;//increment the SelectedIndex to account for 'Headquarters' in the list at position 0 if the user is not restricted.
						}
					}
				}
				_indexClinicRevert=comboClinic.SelectedIndex;
			}
			_listProgProps=ProgramProperties.GetForProgram(_progCur.ProgramNum);
			FillFields();
		}

		private void FillFields() {
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
			}
			textUsername.Text=ProgramProperties.GetPropValFromList(_listProgProps,"Username",clinicNum);
			textPassword.Text=CDT.Class1.TryDecrypt(ProgramProperties.GetPropValFromList(_listProgProps,"Password",clinicNum));
			string payTypeDefNum=ProgramProperties.GetPropValFromList(_listProgProps,"PaymentType",clinicNum);
			string processingMethod=ProgramProperties.GetPropValFromList(_listProgProps,PayConnect.ProgramProperties.DefaultProcessingMethod,clinicNum);
			checkTerminal.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,"TerminalProcessingEnabled",clinicNum));
			checkForceRecurring.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,
				PayConnect.ProgramProperties.PayConnectForceRecurringCharge,clinicNum));
			checkPreventSavingNewCC.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,
				PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,clinicNum));
			checkPatientPortalPayEnabled.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,PayConnect.ProgramProperties.PatientPortalPaymentsEnabled,clinicNum));
			textToken.Text=PIn.String(ProgramProperties.GetPropValFromList(_listProgProps,PayConnect.ProgramProperties.PatientPortalPaymentsToken,clinicNum));
			comboPaymentType.Items.Clear();
			_listPaymentTypeDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<_listPaymentTypeDefs.Count;i++) {
				comboPaymentType.Items.Add(_listPaymentTypeDefs[i].ItemName);
				if(_listPaymentTypeDefs[i].DefNum.ToString()==payTypeDefNum) {
					comboPaymentType.SelectedIndex=i;
				}
			}
			comboDefaultProcessing.Items.Clear();
			comboDefaultProcessing.Items.Add(Lan.g(this,PayConnectProcessingMethod.WebService.GetDescription()));
			comboDefaultProcessing.Items.Add(Lan.g(this,PayConnectProcessingMethod.Terminal.GetDescription()));
			if(processingMethod=="0" || processingMethod=="1") {
				comboDefaultProcessing.SelectedIndex=PIn.Int(processingMethod);
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedIndex==_indexClinicRevert) {//didn't change the selected clinic
				return;
			}
			//if PayConnect is enabled and the username and password are set for the current clinic,
			//make the user select a payment type before switching clinics
			if(checkEnabled.Checked && textUsername.Text!="" && textPassword.Text!="" && comboPaymentType.SelectedIndex==-1) {
				comboClinic.SelectedIndex=_indexClinicRevert;
				MsgBox.Show(this,"Please select a payment type first.");
				return;
			}
			SynchWithHQ();//if the user just modified the HQ credentials, change any credentials that were the same as HQ to keep them synched
			UpdateListProgramPropertiesForClinic(_listUserClinicNums[_indexClinicRevert]);
			_indexClinicRevert=comboClinic.SelectedIndex;//now that we've updated the values for the clinic we're switching from, update _indexClinicRevert
			FillFields();
		}

		///<summary>For each clinic, if the Username and Password are the same as the HQ (ClinicNum=0) Username and Password, update the clinic with the
		///values in the text boxes.  Only modifies other clinics if _indexClinicRevert=0, meaning user just modified the HQ clinic credentials.</summary>
		private void SynchWithHQ() {
			if(!PrefC.HasClinicsEnabled || _listUserClinicNums[_indexClinicRevert]>0) {//using clinics, and modifying the HQ clinic. otherwise return.
				return;
			}
			string hqUsername=ProgramProperties.GetPropValFromList(_listProgProps,"Username",0);//HQ Username before updating to value in textbox
			string hqPassword=ProgramProperties.GetPropValFromList(_listProgProps,"Password",0);//HQ Password before updating to value in textbox
			string hqPayType=ProgramProperties.GetPropValFromList(_listProgProps,"PaymentType",0);//HQ PaymentType before updating to combo box selection
			//IsOnlinePaymentsEnabled will not be synced with HQ, since some clinics may need to disable patient portal payments
			//Token will not be synced with HQ, since some clinics may need to disable patient portal payments
			string payTypeCur="";
			if(comboPaymentType.SelectedIndex>-1) {
				payTypeCur=_listPaymentTypeDefs[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			//for each distinct ClinicNum in the prog property list for PayConnect except HQ
			foreach(long clinicNum in _listProgProps.Select(x => x.ClinicNum).Where(x => x>0).Distinct()) {
				//if this clinic has a different username or password, skip it
				if(!_listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Username" && x.PropertyValue==hqUsername)
					|| !_listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Password" && x.PropertyValue==hqPassword))
				{
					continue;
				}
				//this clinic had a matching username and password, so update the username and password to keep it synched with HQ
				_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Username")
					.ForEach(x => x.PropertyValue=textUsername.Text);//always 1 item; null safe
				string password=CDT.Class1.TryEncrypt(textPassword.Text);//Encrypt password before setting so it's not plaintext.
				_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Password")
					.ForEach(x => x.PropertyValue=password);//always 1 item; null safe
				if(string.IsNullOrEmpty(payTypeCur)) {
					continue;
				}
				//update clinic payment type if it originally matched HQ's payment type and the selected payment type is valid
				_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="PaymentType" && x.PropertyValue==hqPayType)
					.ForEach(x => x.PropertyValue=payTypeCur);//always 1 item; null safe
			}
		}

		private void UpdateListProgramPropertiesForClinic(long clinicNum) {
			string payTypeSelected="";
			if(comboPaymentType.SelectedIndex>-1) {
				payTypeSelected=_listPaymentTypeDefs[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			string processingMethodSelected=comboDefaultProcessing.SelectedIndex.ToString();
			//set the values in the list for this clinic
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Username")
				.ForEach(x => x.PropertyValue=textUsername.Text);//always 1 item; null safe
			string password=CDT.Class1.TryEncrypt(textPassword.Text);//Encrypt password before setting so it's not plaintext.
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Password")
				.ForEach(x => x.PropertyValue=password);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="PaymentType")
				.ForEach(x => x.PropertyValue=payTypeSelected);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PayConnect.ProgramProperties.DefaultProcessingMethod)
				.ForEach(x => x.PropertyValue=processingMethodSelected);
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="TerminalProcessingEnabled")
				.ForEach(x => x.PropertyValue=POut.Bool(checkTerminal.Checked));//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PayConnect.ProgramProperties.PayConnectForceRecurringCharge)
				.ForEach(x => x.PropertyValue=POut.Bool(checkForceRecurring.Checked));
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PayConnect.ProgramProperties.PayConnectPreventSavingNewCC)
				.ForEach(x => x.PropertyValue=POut.Bool(checkPreventSavingNewCC.Checked));
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PayConnect.ProgramProperties.PatientPortalPaymentsEnabled)
				.ForEach(x => x.PropertyValue=POut.Bool(checkPatientPortalPayEnabled.Checked));
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PayConnect.ProgramProperties.PatientPortalPaymentsToken)
				.ForEach(x => x.PropertyValue=textToken.Text);
		}

		private void checkPatientPortalPayEnabled_Click(object sender,EventArgs e) {
			if(!checkPatientPortalPayEnabled.Checked) {
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
			}
			ProgramProperty programPropertyWebPayEnabled=ProgramProperties.GetOnlinePaymentsEnabledForClinic(clinicNum,ProgramName.PayConnect);
			string msg=Lan.g(this,"Online payments are already enabled for another processor and must be disabled in order to use PayConnect online payments. "
				+"Would you like to disable the other processor for online payments?");
			if(programPropertyWebPayEnabled!=null) {
				if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					checkPatientPortalPayEnabled.Checked=false;
					return;
				}
				//User wants to set as new processor for online payments, add to the list for the current session to remove currently enabled program.
				_listXWebWebPayProgProps.Add(programPropertyWebPayEnabled);
			}
		}

		private void butGenerateToken_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textUsername.Text)) {
				MessageBox.Show("Username cannot be empty.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textPassword.Text)) {
				MessageBox.Show("Password cannot be empty.");
				return;
			}
			if(!string.IsNullOrWhiteSpace(textToken.Text) && MessageBox.Show("A token already exists.  Do you want to create a new one?")!=DialogResult.OK) {
				return;
			}
			try {
				textToken.Text=PayConnectREST.GetAccountToken(textUsername.Text,textPassword.Text);
			}
			catch(ODException ex) {
				MessageBox.Show(ex.Message);
			}
			catch(Exception ex) {
				MessageBox.Show("Error:\r\n"+ex.Message);
			}
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("http://www.opendental.com/resources/redirects/redirectpayconnect.html");
		}

		private void checkTerminal_CheckedChanged(object sender,EventArgs e) {
			butDownloadDriver.Visible=checkTerminal.Checked;
		}

		private void butDownloadDriver_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Terminal payments are not available while viewing through the web.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			WebClient client=new WebClient();
			//The VeriFone driver is necessary for PayConnect users to process payments on the VeriFone terminal.
			string zipFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),"VeriFoneUSBUARTDriver_Vx_1.0.0.52_B5.zip");
			try {
				client.DownloadFile(@"http://www.opendental.com/download/drivers/VeriFoneUSBUARTDriver_Vx_1.0.0.52_B5.zip",zipFileName);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Unable to download driver. Error message")+": "+ex.Message);
				return;
			}
			MemoryStream ms=new MemoryStream();
			string setupFileName="";
			using(ZipFile unzipped=ZipFile.Read(zipFileName)) {
				for(int unzipIndex=0;unzipIndex<unzipped.Count;unzipIndex++) {//Unzip/write all files to the temp directory
					ZipEntry ze=unzipped[unzipIndex];
					if(ze.FileName.ToLower()=="setup.exe") {
						setupFileName=ze.FileName;
					}
					ze.Extract(PrefC.GetTempFolderPath(),ExtractExistingFileAction.OverwriteSilently);
				}
			}
			Cursor=Cursors.Default;
			if(setupFileName=="") {
				MsgBox.Show(this,"Unable to install driver. Setup.exe file not found.");
				return;
			}
			//Run the setup.exe file
			Process.Start(ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),setupFileName));
			MessageBox.Show(Lans.g(this,"Download complete. Run the Setup.exe file in")+" "+PrefC.GetTempFolderPath()+" "
				+Lans.g(this,"if it does not start automatically."));
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			#region Validation
			//if this program has been disabled at HQ and someone is trying to enable it, block them
			//if clinics are not enabled and the PayConnect program link is enabled, make sure there is a username and password set
			//if clinics are enabled, the program link can be enabled with blank username and/or password fields for some clinics
			//clinics with blank username and/or password will essentially not have PayConnect enabled
			//if 'Enable terminal processing' is checked then the practice/clinic will not need a username and password.
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.PayConnect,out string err)) {
				MsgBox.Show(err);
				return;
			}
			if(checkEnabled.Checked && !checkTerminal.Checked && !PrefC.HasClinicsEnabled && 
				(textUsername.Text=="" || textPassword.Text=="")) 
			{
				MsgBox.Show(this,"Please enter a username and password first.");
				return;
			}
			if(checkEnabled.Checked //if PayConnect is enabled
				&& comboPaymentType.SelectedIndex<0 //and the payment type is not set
				&& (!PrefC.HasClinicsEnabled  //and either clinics are not enabled (meaning this is the only set of username, password, payment type values)
				|| (textUsername.Text!="" && textPassword.Text!=""))) //or clinics are enabled and this clinic's link has a username and password set
			{
				MsgBox.Show(this,"Please select a payment type first.");
				return;
			}
			if(checkEnabled.Checked //if PayConnect is enabled
				&& comboDefaultProcessing.SelectedIndex < 0)
			{
				MsgBox.Show(this,"Please select a default processing method type first.");
				return;
			}
			SynchWithHQ();//if the user changes the HQ credentials, any clinic that had the same credentials will be kept in synch with HQ
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
			}
			UpdateListProgramPropertiesForClinic(clinicNum);
			string payTypeCur;
			//make sure any other clinics with PayConnect enabled also have a payment type selected
			for(int i=0;i<_listUserClinicNums.Count;i++) {
				if(!checkEnabled.Checked) {//if program link is not enabled, do not bother checking the payment type selected
					break;
				}
				payTypeCur=ProgramProperties.GetPropValFromList(_listProgProps,"PaymentType",_listUserClinicNums[i]);
				//if the program is enabled and the username and password fields are not blank,
				//PayConnect is enabled for this clinic so make sure the payment type is also set
				//Decrypt password because empty string password is not empty string when encrypted.
				string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropValFromList(_listProgProps,"Password",_listUserClinicNums[i]));
				if(((ProgramProperties.GetPropValFromList(_listProgProps,"Username",_listUserClinicNums[i])!="" //if username set
					&& password!="") //and password set
					|| ProgramProperties.GetPropValFromList(_listProgProps,"TerminalProcessingEnabled",_listUserClinicNums[i])=="1")//or terminal enabled
					&& !_listPaymentTypeDefs.Any(x => x.DefNum.ToString()==payTypeCur)) //and paytype is not a valid DefNum
				{
					MsgBox.Show(this,"Please select the payment type for all clinics with PayConnect username and password set.");
					return;
				}
			}
			#endregion Validation
			#region Save
			if(_progCur.Enabled!=checkEnabled.Checked) {//only update the program if the IsEnabled flag has changed
				_progCur.Enabled=checkEnabled.Checked;
				Programs.Update(_progCur);
			}
			ProgramProperties.Sync(_listProgProps,_progCur.ProgramNum);
			//Find all clinics that have PayConnect online payments enabled
			string strPayConnectOnlinePaymentsEnabled=PayConnect.ProgramProperties.PatientPortalPaymentsEnabled;
			List<ProgramProperty> listPayConnectOnlinePayments=_listProgProps.FindAll(x => x.PropertyDesc==strPayConnectOnlinePaymentsEnabled &&
				PIn.Bool(x.PropertyValue));
			for(int i=0;i < listPayConnectOnlinePayments.Count;i++) {
				//Find all online payment enabled program properties that we saved in this session. Only clinics that have changes will have an 
				//IsOnlinePaymentsEnabled property in memory. This is needed to ensure that we don't disable other processors if someone
				//checks to use PayConnect online payments and then decides to keep it disabled during the same session.
				ProgramProperty webOnlinePayments=_listXWebWebPayProgProps.FirstOrDefault(y => y.ClinicNum==listPayConnectOnlinePayments[i].ClinicNum);
				if(webOnlinePayments!=null) {
					ProgramProperties.UpdateProgramPropertyWithValue(webOnlinePayments,POut.Bool(false));
				}
			}
			#endregion Save
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}