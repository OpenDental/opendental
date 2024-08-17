using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormXchargeSetup : FormODBase {
		private Program _program;
		///<summary>List of X-Charge prog props for all clinics.  Includes props with ClinicNum=0 for headquarters/props unassigned to a clinic.</summary>
		private List<ProgramProperty> _listProgramProperties;
		private List<ProgramProperty> _listProgramPropertiesWebPay=new List<ProgramProperty>();
		///<summary>Used to revert the clinic drop down if the user tries to change clinics and the payment type hasn't been set.</summary>
		private long _clinicNumRevert;
		private bool _isLoading = false;
		private List<Def> _listDefsPayTypes;

		///<summary></summary>
		public FormXchargeSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormXchargeSetup_Load(object sender,EventArgs e) {
			_isLoading=true;
			_program=Programs.GetCur(ProgramName.Xcharge);
			if(_program==null) {
				return;//should never happen
			}
			if(ODBuild.IsWeb()) {
				linkLabel1.Text+="\r\n"+Lans.g(this,"X-Charge is not supported in web mode. Use EdgeExpress instead.");
			}
			if(PrefC.HasClinicsEnabled) {
				groupPaySettings.Text=Lan.g(this,"Clinic Payment Settings");
				if(Security.CurUser.ClinicIsRestricted) {
					//if program link is enabled, disable the enable check box so the restricted user cannot disable for all clinics
					checkEnabled.Enabled=!_program.Enabled;
				}
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				_clinicNumRevert=Clinics.ClinicNum;
			}
			else {//clinics not enabled
				checkEnabled.Text=Lan.g(this,"Enabled");
				labelClinicEnable.Visible=false;
				groupPaySettings.Text=Lan.g(this,"Payment Settings");
			}
			checkEnabled.Checked=_program.Enabled;
			textPath.Text=_program.Path;
			textOverride.Text=ProgramProperties.GetLocalPathOverrideForProgram(_program.ProgramNum);
			_listProgramProperties=ProgramProperties.GetForProgram(_program.ProgramNum);
			for(int i=0;i<_listProgramProperties.Count;i++) {
				if(_listProgramProperties[i].IsHighSecurity) {
					_listProgramProperties[i].TagOD=_listProgramProperties[i].PropertyValue;
				}
			}
			if(!ProgramProperties.CanEditProperties(_listProgramProperties)) {
				textPassword.ReadOnly=true;
			}
			FillFields();
			_isLoading=false;
		}

		///<summary>Fills all but comboClinic, checkEnabled, textPath, and textOverride which are filled on load.</summary>
		private void FillFields() {
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum>-1) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			//Password
			string password=ProgramProperties.GetPropValFromList(_listProgramProperties,"Password",clinicNum);
			if(password.Length>0) {
				password=CodeBase.MiscUtils.Decrypt(password);
				textPassword.UseSystemPasswordChar=true;
			}
			textPassword.Text=password;
			//AuthKey had previously been stored as obfuscated text (prior to 16.2). 
			//The XWeb feature was not publicly available for any of these versions so it safe to remove that restriction.
			//It was determined that storing in plain-text is good enough as the obfuscation wasn't really making the key any more secure.
			textAuthKey.Text=ProgramProperties.GetPropValFromList(_listProgramProperties,"AuthKey",clinicNum);
			//PaymentType ComboBox
			string payTypeDefNum=ProgramProperties.GetPropValFromList(_listProgramProperties,"PaymentType",clinicNum);
			comboPaymentType.Items.Clear();
			_listDefsPayTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			_listDefsPayTypes.ForEach(x => comboPaymentType.Items.Add(x.ItemName));
			comboPaymentType.SelectedIndex=_listDefsPayTypes.FindIndex(x => x.DefNum.ToString()==payTypeDefNum);
			//Other text boxes and check boxes
			textUsername.Text=ProgramProperties.GetPropValFromList(_listProgramProperties,"Username",clinicNum);
			textXWebID.Text=ProgramProperties.GetPropValFromList(_listProgramProperties,"XWebID",clinicNum);
			textTerminalID.Text=ProgramProperties.GetPropValFromList(_listProgramProperties,"TerminalID",clinicNum);
			checkWebPayEnabled.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgramProperties,"IsOnlinePaymentsEnabled",clinicNum));
			checkPromptSig.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgramProperties,"PromptSignature",clinicNum));
			checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgramProperties,"PrintReceipt",clinicNum));
			checkForceDuplicate.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.XCharge.XChargeForceRecurringCharge,clinicNum));
			checkPreventSavingNewCC.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC,clinicNum));
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedClinicNum==_clinicNumRevert) {//didn't change the selected clinic
				return;
			}
			//XWeb will be enabled for the clinic if the enabled checkbox is checked and the 3 XWeb fields are not blank.  Don't let them switch clinics or
			//close the form with only 1 or 2 of the three fields filled in.  If they fill in 1, they must fill in the other 2.  Per JasonS - 10/12/2015
			bool isXWebEnabled=checkEnabled.Checked && (textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0  || textTerminalID.Text.Trim().Length>0);
			if(isXWebEnabled && !ValidateXWeb()) {//error message box displayed in ValidateXWeb()
				comboClinic.SelectedClinicNum=_clinicNumRevert;//validation didn't pass, revert clinic choice so they have to fix it
				return;//if any of the X-Web fields do not pass validation, return
			}
			//if the payment type currently set is not valid and X-Charge is enabled, revert the clinic and return, message box shown in ValidatePaymentTypes
			if(!ValidatePaymentTypes(false)) {
				comboClinic.SelectedClinicNum=_clinicNumRevert;//revert clinic selection, X-Charge is enabled and the payment type is not valid
				return;
			}
			SyncWithHQ();//if the user just modified the HQ credentials, change any credentials that were the same as HQ to keep them synched
			string passwordEncrypted="";
			if(textPassword.Text.Trim().Length>0) {
				passwordEncrypted=CodeBase.MiscUtils.Encrypt(textPassword.Text.Trim());
			}			
			string strDefNumPayType="";
			if(comboPaymentType.SelectedIndex>-1) {
				strDefNumPayType=_listDefsPayTypes[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			List<ProgramProperty> listProgramPropertiesPropertyDesc;
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="Username");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textUsername.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="Password");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=passwordEncrypted;//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="PromptSignature");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPromptSig.Checked);//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="PrintReceipt");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPrintReceipt.Checked);//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="XWebID");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textXWebID.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="AuthKey");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textAuthKey.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="TerminalID");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textTerminalID.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="IsOnlinePaymentsEnabled");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkWebPayEnabled.Checked);//always 1 item, null safe
			}
			//payment type already validated
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && x.PropertyDesc=="PaymentType");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=strDefNumPayType;//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert && 
				x.PropertyDesc==ProgramProperties.PropertyDescs.XCharge.XChargeForceRecurringCharge);
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkForceDuplicate.Checked);
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==_clinicNumRevert &&
					x.PropertyDesc==ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC);
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPreventSavingNewCC.Checked);
			}
			_clinicNumRevert=comboClinic.SelectedClinicNum;//now that we've updated the values for the clinic we're switching from, update _clinicNumRevert
			textPassword.UseSystemPasswordChar=false;//FillFields will set this to true if the clinic being selected has a password set
			textAuthKey.UseSystemPasswordChar=false;//FillFields will set this to true if the clinic being selected has an AuthKey entered
			FillFields();
		}

		//This method will be used if we want to start allowing use of Usernames and Passwords in lieu of the Auth Key.  Called from validate in
		//butOK_Click, but for now we don't allow users to enter Username and Password in liew of Auth Key, so not in use.
		/////<summary>Call to validate that the password typed in meets the X-Web password strength requirements.  Passwords must be between 8 and 15
		/////characters in length, and must contain at least one letter, one number, and one of these special characters: $%^&+=</summary>
		//private bool IsPasswordXWebValid() {
		//	string password=textPassword.Text.Trim();
		//	if(password.Length < 8 || password.Length > 15) {//between 8 - 15 chars
		//		return false;
		//	}
		//	if(!Regex.IsMatch(password,"[A-Za-z]+")) {//must contain at least one letter
		//		return false;
		//	}
		//	if(!Regex.IsMatch(password,"[0-9]+")) {//must contain at least one number
		//		return false;
		//	}
		//	if(!Regex.IsMatch(password,"[$%^&+=]+")) {//must contain at least one special character
		//		return false;
		//	}
		//	return true;
		//}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			//Let the users see what they are typing if they clear out the password field completely
			if(textPassword.Text.Trim().Length==0) {
				textPassword.UseSystemPasswordChar=false;
			}
		}

		private void textAuthKey_TextChanged(object sender,EventArgs e) {
			//We want to let users see what they are typing in if they cleared out the AuthKey field completely or are typing in for the first time.
			//X-Charge does this in their server settings window.  We shall do the same.
			if(textAuthKey.Text.Trim().Length==0) {
				textAuthKey.UseSystemPasswordChar=false;
			}
		}

		private void checkWebPayEnabled_Click(object sender,EventArgs e) {
			if(!checkWebPayEnabled.Checked) {
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			ProgramProperty programPropertyWebPayEnabled=ProgramProperties.GetOnlinePaymentsEnabledForClinic(clinicNum,ProgramName.Xcharge);
			string msg=Lan.g(this,"Online payments is already enabled for another processor and must be disabled in order to use Xcharge online payments. "
				+"Would you like to disable the other processor online payments?");
			if(programPropertyWebPayEnabled!=null) {
				if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					checkWebPayEnabled.Checked=false;
					return;
				}
				//User wants to set as new processor for online payments, add to the list for the current session to remove currently enabled program.
				_listProgramPropertiesWebPay.Add(programPropertyWebPayEnabled);
			}
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://opendental.com/resources/redirects/redirectopenedge.html");
		}

		///<summary>Validate XWebID, AuthKey, and TerminalID.  XWebID and TerminalID must be numbers only, 12 digits and 8 digits long respectively.
		///AuthKey must be alphanumeric, 32 digits long.</summary>
		private bool ValidateXWeb(bool suppressMessage=false) {
			//Validate ALL XWebID, AuthKey, and TerminalID.  Each is required for X-Web to work.
			if(!Regex.IsMatch(textXWebID.Text.Trim(),"^[0-9]{12}$")) {
				if(!suppressMessage) {
					MsgBox.Show(this,"XWeb ID must be 12 digits.");
				}
				return false;
			}
			if(!Regex.IsMatch(textAuthKey.Text.Trim(),"^[A-Za-z0-9]{32}$")) {
				if(!suppressMessage) {
					MsgBox.Show(this,"Auth Key must be 32 alphanumeric characters.");
				}
				return false;
			}
			if(!Regex.IsMatch(textTerminalID.Text.Trim(),"^[0-9]{8}$")) {
				if(!suppressMessage) {
					MsgBox.Show(this,"Terminal ID must be 8 digits.");
				}
				return false;
			}
			////We are not going to give the option for users to use their Username and Password.
			////The following password strength requirement would need to be enforced if we want to start allowing use of
			////Usernames and Passwords in lieu of the Auth Key.
			////XWebID and TerminalID are valid.  Make sure the password meets the required complexity for XWeb.
			//if(!IsPasswordXWebValid()) {
			//	MessageBox.Show(this,Lan.g(this,"Passwords must be between 8 and 15 characters in length and must contain at least one letter, "
			//		+"one number, and one of these special characters")+": $%^&+=");
			//	return;
			//}
			return true;
		}

		///<summary>If isValidateAllClinics is true, validates the PaymentType for all clinics with X-Charge enabled.
		///<para>If the current user is restricted to a clinic or if clinics are not enabled and the enabled checkbox is checked,
		///or if isValidateAllClinics is false, only comboPaymentType.SelectedIndex will be validated.</para>
		///<para>If clinics are enabled and isValidateAllClinics is true and the current user is not restricted to a clinic, the PaymentType for any
		///clinic with Username and Password set or with any of the XWeb settings set will be validated.</para></summary>
		private bool ValidatePaymentTypes(bool isAllClinics) {
			//if not enabled, don't worry about invalid payment type
			if(!checkEnabled.Checked) {
				return true;
			}
			//XWeb will be enabled for the clinic if the XWeb enabled checkbox is checked and the 3 XWeb fields are not blank.  Don't let them switch clinics or
			//close the form with only 1 or 2 of the three fields filled in.  If they fill in 1, they must fill in the other 2.  Per JasonS - 10/12/2015
			bool isXWebEnabled=checkWebPayEnabled.Checked 
				&& (textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0  || textTerminalID.Text.Trim().Length>0);
			//X-Charge will be enabled if the enabled checkbox is checked and either clinics are disabled OR both Username and Password are set
			bool isClientEnabled=!PrefC.HasClinicsEnabled || (textUsername.Text.Trim().Length>0 && textPassword.Text.Trim().Length>0);
			if((isClientEnabled || isXWebEnabled) && comboPaymentType.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a payment type first.");
				return false;
			}
			if(!isAllClinics || !PrefC.HasClinicsEnabled || Security.CurUser.ClinicIsRestricted) {
				return true;
			}
			//only validate payment types for all clinics if isAllClinics==true and clinics are enabled and the current user is not restricted to a clinic
			List<long> listClinicNumsUser=comboClinic.ListClinics.Select(x => x.ClinicNum).ToList();
			//make sure all clinics with X-Charge enabled also have a payment type selected
			for(int i=0;i<listClinicNumsUser.Count;i++) {
				string strPayType=ProgramProperties.GetPropValFromList(_listProgramProperties,"PaymentType",listClinicNumsUser[i]);
				//isClientEnabled will be true if both username and password are set for this clinic
				isClientEnabled=ProgramProperties.GetPropValFromList(_listProgramProperties,"Username",listClinicNumsUser[i]).Length>0
					&& ProgramProperties.GetPropValFromList(_listProgramProperties,"Password",listClinicNumsUser[i]).Length>0;
				//isXWebEnabled will be true if any of the XWeb values are set
				isXWebEnabled=checkWebPayEnabled.Checked
					&& (ProgramProperties.GetPropValFromList(_listProgramProperties,"XWebID",listClinicNumsUser[i]).Length>0
					|| ProgramProperties.GetPropValFromList(_listProgramProperties,"AuthKey",listClinicNumsUser[i]).Length>0
					|| ProgramProperties.GetPropValFromList(_listProgramProperties,"TerminalID",listClinicNumsUser[i]).Length>0);
				//if the program is enabled and the username and password fields are not blank for client, or XWebID, AuthKey, and TerminalID are not blank
				//for XWeb, then X-Charge is enabled for this clinic so make sure the payment type is also set
				if((isClientEnabled || isXWebEnabled)	&& !_listDefsPayTypes.Any(x => x.DefNum.ToString()==strPayType)) {
					MsgBox.Show(this,"Please select the payment type for all clinics with X-Charge enabled.");
					return false;
				}
			}
			return true;
		}

		///<summary>Updates the values in the local list of program properties for each clinic.
		///Only modifies other clinics if _listUserClinicNums[_indexClinicRevert]=0, meaning user just modified the HQ values.
		///If the clinic X-Charge client Username and Password are the same as HQ, the clinic values will be updated with the values entered.
		///If the clinic XWeb values are the same as HQ, the clinic XWeb values will be updated with the values entered.
		///If both the X-Charge client values and the XWeb values are the same as HQ, the payment type will be updated.
		///The values in the local list for HQ, or for the clinic modified if it was not HQ, have to be updated after calling this method.</summary>
		private void SyncWithHQ() {
			if(!PrefC.HasClinicsEnabled || _clinicNumRevert>0) {
				return;
			}
			string hqUsername=ProgramProperties.GetPropValFromList(_listProgramProperties,"Username",0);//HQ Username before updating to value in textbox
			string hqPassword=ProgramProperties.GetPropValFromList(_listProgramProperties,"Password",0);//HQ Password before updating to value in textbox
			string hqXWebID=ProgramProperties.GetPropValFromList(_listProgramProperties,"XWebID",0);//HQ XWebID before updating to value in textbox
			string hqAuthKey=ProgramProperties.GetPropValFromList(_listProgramProperties,"AuthKey",0);//HQ AuthKey before updating to value in textbox
			string hqTerminalID=ProgramProperties.GetPropValFromList(_listProgramProperties,"TerminalID",0);//HQ TerminalID before updating to value in textbox
			string hqPayType=ProgramProperties.GetPropValFromList(_listProgramProperties,"PaymentType",0);//HQ PaymentType before updating to combo box selection
			//IsOnlinePaymentsEnabled will not be synced with HQ so specific clinics can be disabled for patient portal payments.
			string strDefNumPayType="";
			if(comboPaymentType.SelectedIndex>-1) {
				strDefNumPayType=_listDefsPayTypes[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			string passwordEncrypted="";
			if(textPassword.Text.Trim().Length>0) {
				passwordEncrypted=CodeBase.MiscUtils.Encrypt(textPassword.Text.Trim());
			}
			//for each distinct ClinicNum in the prog property list for X-Charge except HQ
			List<long> listClinicNums=_listProgramProperties.Select(x => x.ClinicNum).Where(x => x>0).Distinct().ToList();
			for(int i = 0;i<listClinicNums.Count;i++) { 
				//Updates the PaymentType in both if checks, in case the other isn't met so the payment type will be synched if either condition is true.
				//if this clinic has the same Username and Password, update them
				bool isClientSynch=_listProgramProperties.Exists(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="Username" && x.PropertyValue==hqUsername)
					&& _listProgramProperties.Exists(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="Password" && x.PropertyValue==hqPassword);
				//only if all three XWeb HQ values are not blank
				bool isXWebSynch=_listProgramProperties.Exists(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="XWebID" && x.PropertyValue==hqXWebID)
					&& _listProgramProperties.Exists(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="AuthKey" && x.PropertyValue==hqAuthKey)
					&& _listProgramProperties.Exists(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="TerminalID" && x.PropertyValue==hqTerminalID);
				if(!isClientSynch && !isXWebSynch) {
					continue;
				}
				List<ProgramProperty> listProgramPropertiesPropertyDesc;
				if(isClientSynch) {
					//update the username and password to keep it synched with HQ
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="Username");
					for(int j=0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=textUsername.Text.Trim();//always 1 item; null safe
					}
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="Password");
					for(int j=0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=passwordEncrypted;
					}
				}
				if(isXWebSynch) {
					//update the XWebID, AuthKey, and TerminalID to keep it synched with HQ
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="XWebID");
					for(int j=0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=textXWebID.Text.Trim();//always 1 item; null safe
					}
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="AuthKey");
					for(int j=0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=textAuthKey.Text.Trim();//always 1 item; null safe
					}
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="TerminalID");
					for(int j=0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=textTerminalID.Text.Trim();//always 1 item; null safe
					}
				}
				//only synch payment type if both client and XWeb values are the same as HQ and the payment type is valid
				if(isClientSynch && isXWebSynch && !string.IsNullOrEmpty(strDefNumPayType)) {
					listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i] && x.PropertyDesc=="PaymentType" && x.PropertyValue==hqPayType);
					for(int j = 0;j<listProgramPropertiesPropertyDesc.Count;j++) {
						listProgramPropertiesPropertyDesc[j].PropertyValue=strDefNumPayType;
					}
				}
			}
		}

		private void checkEnabled_CheckedChanged(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				bool isDisabledForWeb=Programs.GetListDisabledForWeb().Select(x => x.ToString()).Contains(_program.ProgName);
				if(checkEnabled.Checked && isDisabledForWeb) {
					checkEnabled.Checked=false;
					if(!_isLoading){
						MsgBox.Show(this,"Web users cannot currently enable this bridge");
					}
				}
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			#region Validation and Update Local List
			if(_program==null) {//should never happen
				MsgBox.Show(this,"X-Charge entry is missing from the database.");
				return;
			}
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.Xcharge,out string err)) {
				MessageBox.Show(err);
				return;
			}
			#region Validate Path and Local Path Override
			//Check for the path override first.
			if(checkEnabled.Checked && textOverride.Text.Trim().Length>0) {
				if(!File.Exists(textOverride.Text.Trim())) {
					MsgBox.Show(this,"Local Path Override is not valid.");
					return;
				}
			}
			//Check the global path if no override was entered.
			else if(checkEnabled.Checked && textOverride.Text.Trim().Length==0) {
				//No override was entered so validate the global path which is required at this point.
				if((textPath.Text.Trim().Length==0 || !File.Exists(textPath.Text.Trim()))
					&& !(checkWebPayEnabled.Checked && ValidateXWeb(true) && textPath.Text.Trim().Length==0)) 
				{
					MsgBox.Show(this,"Program Path is not valid.");
					return;
				}
			}
			#endregion Validate Path and Local Path Override
			#region Validate Username and Password
			//If clinics are not enabled and the X-Charge program link is enabled, make sure there is a username and password set.
			//If clinics are enabled, the program link can be enabled with blank username and/or password fields for some clinics.
			//X-Charge will be disabled for any clinic with a blank username or password.
			if(checkEnabled.Checked && !PrefC.HasClinicsEnabled && (textUsername.Text.Trim().Length==0 || textPassword.Text.Trim().Length==0)) {
				MsgBox.Show(this,"Please enter a username and password first.");
				return;
			}
			#endregion Validate Username and Password
			#region Validate X-Web WebID, AuthKey, and TerminalID
			//Check to see if ANY X-Web settings have been set, and if so validate them
			//XWeb will be enabled for the clinic if the enabled checkbox is checked and the 3 XWeb fields are not blank.  Don't let them switch clinics or
			//close the form with only 1 or 2 of the three fields filled in.  If they fill in 1, they must fill in the other 2.  Per JasonS - 10/12/2015
			if(checkEnabled.Checked
				&& (textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0  || textTerminalID.Text.Trim().Length>0)
				&& !ValidateXWeb()) //error message box displayed in ValidateXWeb()
			{
				return;
			}
			#endregion Validate X-Web WebID, AuthKey, and TerminalID
			#region Update Local List of Program Properties
			//if the user just modified the HQ credentials, change any credentials that were the same as HQ to keep them synched
			SyncWithHQ();
			//get selected ClinicNum (if enabled), PaymentType, encrypted Password, and encrypted AuthKey
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			string passwordEncrypted="";
			if(textPassword.Text.Trim().Length>0) {
				passwordEncrypted=CodeBase.MiscUtils.Encrypt(textPassword.Text.Trim());
			}			
			string strDefNumPayType="";
			if(comboPaymentType.SelectedIndex>-1) {
				strDefNumPayType=_listDefsPayTypes[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			List<ProgramProperty> listProgramPropertiesPropertyDesc;
			//set the values in the list for this clinic
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Username");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textUsername.Text.Trim();
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="Password");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=passwordEncrypted;
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="PromptSignature");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPromptSig.Checked);//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="PrintReceipt");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPrintReceipt.Checked);//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="XWebID");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textXWebID.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="AuthKey");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textAuthKey.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="TerminalID");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=textTerminalID.Text.Trim();//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="IsOnlinePaymentsEnabled");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkWebPayEnabled.Checked);//always 1 item, null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc=="PaymentType");
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=strDefNumPayType;//always 1 item; null safe
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.XCharge.XChargeForceRecurringCharge);
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkForceDuplicate.Checked);
			}
			listProgramPropertiesPropertyDesc=_listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC);
			for(int i = 0;i<listProgramPropertiesPropertyDesc.Count;i++) {
				listProgramPropertiesPropertyDesc[i].PropertyValue=POut.Bool(checkPreventSavingNewCC.Checked);
			}
			#endregion Update Local List of Program Properties
			#region Validate PaymentTypes For All Clinics
			//validate the payment type set for all clinics with X-Charge enabled
			if(!ValidatePaymentTypes(true)) {//if validation fails, message box will already have been shown
				return;
			}
			#endregion Validate PaymentTypes For All Clinics
			#endregion Validation and Update Local List
			#region Save
			if(_program.Enabled!=checkEnabled.Checked || _program.Path!=textPath.Text.Trim()) {//update the program if the IsEnabled flag or Path has changed
				_program.Enabled=checkEnabled.Checked;
				_program.Path=textPath.Text.Trim();
				Programs.Update(_program);
			}
			if(ProgramProperties.GetLocalPathOverrideForProgram(_program.ProgramNum)!=textOverride.Text.Trim()) {
				ProgramProperties.InsertOrUpdateLocalOverridePath(_program.ProgramNum,textOverride.Text.Trim());
			}
			ProgramProperties.Sync(_listProgramProperties,_program.ProgramNum);
			//Find all clinics that have Xcharge online payments enabled
			List<ProgramProperty> listProgramPropertyOnlinePayments=_listProgramProperties.FindAll(x => x.PropertyDesc=="IsOnlinePaymentsEnabled" &&
				PIn.Bool(x.PropertyValue));
			for(int i=0;i < listProgramPropertyOnlinePayments.Count;i++) {
				//Find all online payment enabled program properties that we saved in this session. Only clinics that have changes will have an 
				//IsOnlinePaymentsEnabled property in memory. This is needed to ensure that we don't disable other processors if someone
				//checks to use Xcharge online payments and then decides to keep it disabled during the same session.
				ProgramProperty programPropertyWebPay=_listProgramPropertiesWebPay.FirstOrDefault(y => y.ClinicNum==listProgramPropertyOnlinePayments[i].ClinicNum);
				if(programPropertyWebPay!=null) {
					ProgramProperties.UpdateProgramPropertyWithValue(programPropertyWebPay,POut.Bool(false));
				}
			}
			#endregion Save
			for(int i=0;i<_listProgramProperties.Count;i++) {
				if(_listProgramProperties[i].IsHighSecurity && _listProgramProperties[i].PropertyValue!=_listProgramProperties[i].TagOD.ToString()){
					StringBuilder stringBuilderLogText=new StringBuilder();
					stringBuilderLogText.Append(_program.ProgDesc+"'s "+_listProgramProperties[i].PropertyDesc);
					if(PrefC.HasClinicsEnabled) {
						if(_listProgramProperties[i].ClinicNum==0) {
							stringBuilderLogText.Append(" for Headquarters");
						}
						else {
							stringBuilderLogText.Append(" for clinic "+_listProgramProperties[i].ClinicNum);
						}					
					}
					stringBuilderLogText.Append(" was altered.");
					SecurityLogs.MakeLogEntry(Permissions.ManageHighSecurityProgProperties,0,stringBuilderLogText.ToString(),_program.ProgramNum,DateTime.Now);
				}
			}
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















