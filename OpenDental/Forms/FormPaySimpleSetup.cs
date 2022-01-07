using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using System.Diagnostics;
using CodeBase;

namespace OpenDental {
	public partial class FormPaySimpleSetup:FormODBase {

		private Program _progCur;
		///<summary>Local cache of all of the ClinicNums the current user has permission to access at the time the form loads.  Filled at the same time
		///as comboClinic and is used to set programproperty.ClinicNum when saving.</summary>
		private List<long> _listUserClinicNums;
		///<summary>List of PaySimple program properties for all clinics.
		///Includes properties with ClinicNum=0, the headquarters props or props not assigned to a clinic.</summary>
		private List<ProgramProperty> _listProgProps;

		///<summary>Used to revert the slected index in the clinic drop down box if the user tries to change clinics
		///and the payment type has not been set.</summary>
		private int _indexClinicRevert;
		private List<Def> _listPaymentTypeDefs;

		public FormPaySimpleSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPaySimpleSetup_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.PaySimple);
			if(_progCur==null) {
				MsgBox.Show(this,"The PaySimple entry is missing from the database.");//should never happen
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
				//if PaySimple is enabled and the user is restricted to a clinic, don't allow the user to disable for all clinics
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
			if(PrefC.HasClinicsEnabled) {
				foreach(Clinic clinicCur in Clinics.GetForUserod(Security.CurUser)) {
					AddNeededProgramProperties(clinicCur.ClinicNum);
				}
			}
			FillFields();
		}

		private void AddNeededProgramProperties(long clinicNum) {
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=PaySimple.PropertyDescs.PaySimpleApiUserName,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName && x.ClinicNum==0).PropertyValue,
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=PaySimple.PropertyDescs.PaySimpleApiKey,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey && x.ClinicNum==0).PropertyValue,
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeCC)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=PaySimple.PropertyDescs.PaySimplePayTypeCC,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeCC && x.ClinicNum==0).PropertyValue,
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeACH)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=PaySimple.PropertyDescs.PaySimplePayTypeACH,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeACH && x.ClinicNum==0).PropertyValue,
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePreventSavingNewCC && x.ClinicNum==0)?.PropertyValue??"0",
				});
			}
		}

		private void FillFields() {
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
			}
			textUsername.Text=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiUserName,clinicNum);
			textKey.Text=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiKey,clinicNum);
			string payTypeDefNumCC=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeCC,clinicNum);
			string payTypeDefNumACH=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeACH,clinicNum);
			checkPreventSavingNewCC.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,
				PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,clinicNum));
			_listPaymentTypeDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			comboPaymentTypeCC.Items.Clear();
			comboPaymentTypeCC.Items.AddDefs(_listPaymentTypeDefs);
			comboPaymentTypeCC.SetSelectedDefNum(PIn.Long(payTypeDefNumCC));
			comboPaymentTypeACH.Items.Clear();
			comboPaymentTypeACH.Items.AddDefs(_listPaymentTypeDefs);
			comboPaymentTypeACH.SetSelectedDefNum(PIn.Long(payTypeDefNumACH));
		}

		private string GetUsernameForClinic(long clinicNum) {
			return ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiUserName,clinicNum);
		}

		private string GetKeyForClinic(long clinicNum) {
			return ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiKey,clinicNum);
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedIndex==_indexClinicRevert) {//didn't change the selected clinic
				return;
			}
			//if PaySimple is enabled and the username and key are set for the current clinic,
			//make the user select a payment type before switching clinics
			if(checkEnabled.Checked && !IsClinicCurSetupDone()) {
				comboClinic.SelectedIndex=_indexClinicRevert;
				MsgBox.Show(this,"Please select a username, key, and/or payment type first.");
				return;
			}
			SynchWithHQ();//if the user just modified the HQ credentials, change any credentials that were the same as HQ to keep them synched
			//set the values in the list for the clinic we are switching from, at _clinicIndexRevert
			_listProgProps.FindAll(x => x.ClinicNum==_listUserClinicNums[_indexClinicRevert] && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName)
				.ForEach(x => x.PropertyValue=textUsername.Text);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==_listUserClinicNums[_indexClinicRevert] && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey)
				.ForEach(x => x.PropertyValue=textKey.Text);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==_listUserClinicNums[_indexClinicRevert] 
					&& x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeCC 
					&& comboPaymentTypeCC.SelectedIndex>-1)
				.ForEach(x => x.PropertyValue=comboPaymentTypeCC.GetSelected<Def>().DefNum.ToString());//always 1 item selected
			_listProgProps.FindAll(x => x.ClinicNum==_listUserClinicNums[_indexClinicRevert]
					&& x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeACH
					&& comboPaymentTypeACH.SelectedIndex>-1)
				.ForEach(x => x.PropertyValue=comboPaymentTypeACH.GetSelected<Def>().DefNum.ToString());//always 1 item selected
			_listProgProps.FindAll(x => x.ClinicNum==_listUserClinicNums[_indexClinicRevert]
				&& x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)
				.ForEach(x => x.PropertyValue=POut.Bool(checkPreventSavingNewCC.Checked));
			_indexClinicRevert=comboClinic.SelectedIndex;//now that we've updated the values for the clinic we're switching from, update _indexClinicRevert
			FillFields();
		}

		private bool IsClinicCurSetupDone() {
			//If nothing is set, they are OK to change clinics and save.
			if(string.IsNullOrWhiteSpace(textUsername.Text) && string.IsNullOrWhiteSpace(textKey.Text)){
				return true;
			}
			//If everything is set, they are OK to change clinics and save.
			if(!string.IsNullOrWhiteSpace(textUsername.Text) && !string.IsNullOrWhiteSpace(textKey.Text) 
				&& comboPaymentTypeCC.SelectedIndex>=0 && comboPaymentTypeACH.SelectedIndex>=0) 
			{
				return true;
			}
			return false;
		}

		///<summary>For each clinic, if the Username and Key are the same as the HQ (ClinicNum=0) Username and Key, update the clinic with the
		///values in the text boxes.  Only modifies other clinics if _indexClinicRevert=0, meaning user just modified the HQ clinic credentials.</summary>
		private void SynchWithHQ() {
			if(!PrefC.HasClinicsEnabled || _listUserClinicNums[_indexClinicRevert]>0) {//using clinics, and modifying the HQ clinic. otherwise return.
				return;
			}
			string hqUsername=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiUserName,0);//HQ Username before updating to value in textbox
			string hqKey=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiKey,0);//HQ Key before updating to value in textbox
			string hqPayTypeCC=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeCC,0);//HQ PaymentType before updating to combo box selection
			string payTypeCC="";
			if(comboPaymentTypeCC.SelectedIndex>-1) {
				payTypeCC=comboPaymentTypeCC.GetSelected<Def>().DefNum.ToString();
			}
			string hqPayTypeACH=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeACH,0);//HQ PaymentType before updating to combo box selection
			string payTypeACH="";
			if(comboPaymentTypeACH.SelectedIndex>-1) {
				payTypeACH=comboPaymentTypeACH.GetSelected<Def>().DefNum.ToString();
			}
			//for each distinct ClinicNum in the prog property list for PaySimple except HQ
			foreach(long clinicNum in _listProgProps.Select(x => x.ClinicNum).Where(x => x>0).Distinct()) {
				//if this clinic has a different username or key, skip it
				if(!_listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName && x.PropertyValue==hqUsername)
					|| !_listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey && x.PropertyValue==hqKey)) {
					continue;
				}
				//this clinic had a matching username and key, so update the username and key to keep it synched with HQ
				_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName)
					.ForEach(x => x.PropertyValue=textUsername.Text);//always 1 item; null safe
				_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey)
					.ForEach(x => x.PropertyValue=textKey.Text);//always 1 item; null safe
				if(!string.IsNullOrEmpty(payTypeCC)) {
					//update clinic payment type if it originally matched HQ's payment type and the selected payment type is valid
					_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeCC 
							&& x.PropertyValue==hqPayTypeCC)
						.ForEach(x => x.PropertyValue=payTypeCC);
				}
				if(!string.IsNullOrEmpty(payTypeACH)) {
					//update clinic payment type if it originally matched HQ's payment type and the selected payment type is valid
					_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeACH
							&& x.PropertyValue==hqPayTypeACH)
						.ForEach(x => x.PropertyValue=payTypeACH);
				}
			}
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://paysimple.com/partner/open-dental");
		}

		///<summary>Returns an error message if something went wrong. Otherwise, returns an empty string.</summary>
		private string WebhookHelper(string username,string key,long clinicNum) {
			string errorMessage="";
			if(username=="" || key=="") {
				return errorMessage;
			}
			try {
				Cursor=Cursors.WaitCursor;
				List<PaySimple.ApiWebhookResponse> listWebhooks=PaySimple.GetAchWebhooks(clinicNum);
				List<string> listHooksNeeded=new List<string>();
				if(listWebhooks.SelectMany(x => x.WebhookTypes.FindAll(y => y=="payment_failed")).Count()==0) {
					//no payment_failed webhook exists, created one. 
					listHooksNeeded.Add("payment_failed");
				}
				if(listWebhooks.SelectMany(x => x.WebhookTypes.FindAll(y => y=="payment_returned")).Count()==0) {
					//no payment_returned webhook exists, created one. 
					listHooksNeeded.Add("payment_returned");
				}
				if(listWebhooks.SelectMany(x => x.WebhookTypes.FindAll(y => y=="payment_settled")).Count()==0) {
					//no payment_settled webhook exists, created one. 
					listHooksNeeded.Add("payment_settled");
				}
				if(listHooksNeeded.Count > 0) {			
					string url=WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetPaySimpleWebHookUrl();		
					PaySimple.PostWebhook(clinicNum,listHooksNeeded.ToArray(),url);
				}
			}
			catch(Exception ex) {
				if(ex.Message.ToLower().Contains("invalid authorization token")) {
					errorMessage+=$"{Lan.g(this,"Authorization error. Username and/or Key is most likely incorrect.")}";
				}
				else {
					errorMessage+=$"{ex.Message}";
				}
			}
			finally {				
				Cursor=Cursors.Default;
			}
			return errorMessage;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			#region Validation
			//if program has been disabled at Hq and someone is trying to enable it from this form, block em
			//if clinics are not enabled and the PaySimple program link is enabled, make sure there is a username and key set
			//if clinics are enabled, the program link can be enabled with blank username and/or key fields for some clinics
			//clinics with blank username and/or key will essentially not have PaySimple enabled
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.PaySimple,out string err)) {
				MsgBox.Show(err);
				return;
			}
			if(checkEnabled.Checked && !IsClinicCurSetupDone()) {//Also works for offices not using clinics
				MsgBox.Show(this,"Please enter a username, key, and/or payment type first.");
				return;
			}
			SynchWithHQ();//if the user changes the HQ credentials, any clinic that had the same credentials will be kept in synch with HQ
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
			}
			string payTypeCCSelected="";
			if(comboPaymentTypeCC.SelectedIndex>-1) {
				payTypeCCSelected=comboPaymentTypeCC.GetSelected<Def>().DefNum.ToString();
			}
			string payTypeACHSelected="";
			if(comboPaymentTypeACH.SelectedIndex>-1) {
				payTypeACHSelected=comboPaymentTypeACH.GetSelected<Def>().DefNum.ToString();
			}
			//set the values in the list for this clinic
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiUserName)
				.ForEach(x => x.PropertyValue=textUsername.Text);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimpleApiKey)
				.ForEach(x => x.PropertyValue=textKey.Text);//always 1 item; null safe
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeCC)
				.ForEach(x => x.PropertyValue=payTypeCCSelected);//always 1 item
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePayTypeACH)
				.ForEach(x => x.PropertyValue=payTypeACHSelected);//always 1 item
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)
				.ForEach(x => x.PropertyValue=POut.Bool(checkPreventSavingNewCC.Checked));
			string payTypeCC;
			string payTypeACH;
			//make sure any other clinics with PaySimple enabled also have a payment type selected
			for(int i=0;i<_listUserClinicNums.Count;i++) {
				if(!checkEnabled.Checked) {//if program link is not enabled, do not bother checking the payment type selected
					break;
				}
				payTypeCC=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeCC,_listUserClinicNums[i]);
				payTypeACH=ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimplePayTypeACH,_listUserClinicNums[i]);
				//if the program is enabled and the username and key fields are not blank,
				//PaySimple is enabled for this clinic so make sure the payment type is also set
				if(ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiUserName,_listUserClinicNums[i])!="" //if username set
					&& ProgramProperties.GetPropValFromList(_listProgProps,PaySimple.PropertyDescs.PaySimpleApiKey,_listUserClinicNums[i])!=""//and key set
					//and either paytype is not a valid DefNum
					&& (!_listPaymentTypeDefs.Any(x => x.DefNum.ToString()==payTypeCC)
						|| !_listPaymentTypeDefs.Any(x => x.DefNum.ToString()==payTypeACH))) 
				{
					MsgBox.Show(this,"Please select payment types for all clinics with PaySimple username and key set.");
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
			#endregion Save
			DataValid.SetInvalid(InvalidType.Programs);
			//After programproperties are saved. User might want to save some information and then come back to finish up, so don't hit them with Webhooks validation.
			if(!checkEnabled.Checked) {
				DialogResult=DialogResult.OK;
				return;
			}
			//Webhooks calls validation. This code needs to be under the validation section so people enabling the program for the first time will 
			//be able to save their fields and the program properties will get filled correctly. 
			//get url for webhooks, then create the webhooks if not already present. Has to be done after validation so new user enables are able to save.
			//for each clinic that has a username and api key, make a call to paysimple's api to see what webhooks this api account has.
			string errorMessage="";
			if(PrefC.HasClinicsEnabled) {
				for(int i=0;i<_listUserClinicNums.Count;i++) {
					string errorMessageForClinic=WebhookHelper(GetUsernameForClinic(_listUserClinicNums[i]),GetKeyForClinic(_listUserClinicNums[i]),_listUserClinicNums[i]);
					if(!errorMessageForClinic.IsNullOrEmpty()) {
						if(!errorMessage.IsNullOrEmpty()) {
							errorMessage+="\r\n";
						}
						string clinicAbbr = _listUserClinicNums[i]==0 ? "Headquarters" : Clinics.GetAbbr(_listUserClinicNums[i]);
						errorMessage+=$"-{Lan.g(this,"Error for Clinic")} '{clinicAbbr}': {errorMessageForClinic}";
					}
				}
			}
			else {
				errorMessage=WebhookHelper(textUsername.Text,textKey.Text,0);
			}
			if(!string.IsNullOrEmpty(errorMessage)) {
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(errorMessage);
				msgBoxCopyPaste.ShowDialog();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}