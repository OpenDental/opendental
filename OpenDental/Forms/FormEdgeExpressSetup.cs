using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using EdgeExpressProps=OpenDentBusiness.ProgramProperties.PropertyDescs.EdgeExpress;

namespace OpenDental {
	public partial class FormEdgeExpressSetup:FormODBase {
		private Program _progCur;
		///<summary>Used to revert the clinic drop down if the user tries to change clinics and the payment type hasn't been set.</summary>
		private long _clinicNumRevert;
		private List<Def> _listPayTypeDefs;
		///<summary>List of EdgeExpress prog props for all clinics.  Includes props with ClinicNum=0 for headquarters/props unassigned to a clinic.</summary>
		private List<ProgramProperty> _listProgProps;
		private List<ProgramProperty> _listWebPayProgProps=new List<ProgramProperty>();

		public FormEdgeExpressSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEdgeExpressSetup_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.EdgeExpress);
			if(_progCur==null) {
				return;//should never happen
			}
			if(PrefC.HasClinicsEnabled) {
				groupPaySettings.Text=Lan.g(this,"Clinic Payment Settings");
				if(Security.CurUser.ClinicIsRestricted) {
					//if program link is enabled, disable the enable check box so the restricted user cannot disable for all clinics
					checkEnabled.Enabled=!_progCur.Enabled;
				}
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				_clinicNumRevert=Clinics.ClinicNum;
			}
			else {//clinics not enabled
				checkEnabled.Text=Lan.g(this,"Enabled");
				labelClinicEnable.Visible=false;
				groupPaySettings.Text=Lan.g(this,"Payment Settings");
			}
			checkEnabled.Checked=_progCur.Enabled;
			_listProgProps=ProgramProperties.GetForProgram(_progCur.ProgramNum);
			FillFields();
		}

		///<summary>Fills all but comboClinic, checkEnabled which are filled on load.</summary>
		private void FillFields() {
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum>-1) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			//AuthKey had previously been stored as obfuscated text (prior to 16.2). 
			//The XWeb feature was not publicly available for any of these versions so it safe to remove that restriction.
			//It was determined that storing in plain-text is good enough as the obfuscation wasn't really making the key any more secure.
			textAuthKey.Text=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.AuthKey,clinicNum);
			//PaymentType ComboBox
			string payTypeDefNum=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PaymentType,clinicNum);
			comboPaymentType.Items.Clear();
			_listPayTypeDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			_listPayTypeDefs.ForEach(x => comboPaymentType.Items.Add(x.ItemName));
			comboPaymentType.SelectedIndex=_listPayTypeDefs.FindIndex(x => x.DefNum.ToString()==payTypeDefNum);
			//Other text boxes and check boxes
			textXWebID.Text=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.XWebID,clinicNum);
			textTerminalID.Text=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.TerminalID,clinicNum);
			checkPromptSig.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PromptSignature,clinicNum));
			checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PrintReceipt,clinicNum));
			checkForceDuplicate.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.ForceRecurringCharge,clinicNum));
			checkPreventSavingNewCC.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PreventSavingNewCC,clinicNum));
			checkWebPayEnabled.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.IsOnlinePaymentsEnabled,clinicNum));
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedClinicNum==_clinicNumRevert) {//didn't change the selected clinic
				return;
			}
			//EdgeExpress will be enabled for the clinic if the enabled checkbox is checked and the 3 XWeb fields are not blank. Don't let them switch 
			//clinics or close the form with only 1 or 2 of the three fields filled in.  If they fill in 1, they must fill in the other 2. 
			bool isXWebEnabled=checkEnabled.Checked && (textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0 
				|| textTerminalID.Text.Trim().Length>0);
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
			UpdateAllProperties(_clinicNumRevert);
			_clinicNumRevert=comboClinic.SelectedClinicNum;//now that we've updated the values for the clinic we're switching from, update _clinicNumRevert
			FillFields();
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {			
			Process.Start("https://opendental.com/resources/redirects/redirectopenedge.html");
		}

		///<summary>Validate XWebID, AuthKey, and TerminalID.  XWebID and TerminalID must be numbers only, 12 digits and 8 digits long respectively.
		///AuthKey must be alphanumeric, 32 digits long.</summary>
		private bool ValidateXWeb(bool suppressMessage = false) {
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
			return true;
		}

		///<summary>If isValidateAllClinics is true, validates the PaymentType for all clinics with EdgeExpress enabled.
		///<para>If the current user is restricted to a clinic or if clinics are not enabled and the enabled checkbox is checked,
		///or if isValidateAllClinics is false, only comboPaymentType.SelectedIndex will be validated.</para>
		///<para>If clinics are enabled and isValidateAllClinics is true and the current user is not restricted to a clinic, the PaymentType for any
		///clinic with Username and Password set or with any of the XWeb settings set will be validated.</para></summary>
		private bool ValidatePaymentTypes(bool isAllClinics) {
			//if not enabled, don't worry about invalid payment type
			if(!checkEnabled.Checked) {
				return true;
			}
			bool isClientEnabled=!PrefC.HasClinicsEnabled || textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0  || 
				textTerminalID.Text.Trim().Length>0;
			if(isClientEnabled && comboPaymentType.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a payment type first.");
				return false;
			}
			if(!isAllClinics || !PrefC.HasClinicsEnabled || Security.CurUser.ClinicIsRestricted) {
				return true;
			}
			//only validate payment types for all clinics if isAllClinics==true and clinics are enabled and the current user is not restricted to a clinic
			string payTypeCur="";
			List<long> listUserClinicNums=comboClinic.ListClinics.Select(x => x.ClinicNum).ToList();
			//make sure all clinics with EdgeExpress enabled also have a payment type selected
			for(int i=0;i<listUserClinicNums.Count;i++) {
				payTypeCur=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PaymentType,listUserClinicNums[i]);
				//isClientEnabled will be true if both username and password are set for this clinic
				isClientEnabled=ProgramProperties.GetPropValFromList(_listProgProps,"Username",listUserClinicNums[i]).Length>0
					&& ProgramProperties.GetPropValFromList(_listProgProps,"Password",listUserClinicNums[i]).Length>0;
				if(isClientEnabled && !_listPayTypeDefs.Any(x => x.DefNum.ToString()==payTypeCur)) {
					MsgBox.Show(this,"Please select the payment type for all clinics with EdgeExpress enabled.");
					return false;
				}
			}
			return true;
		}

		///<summary>Updates the values in the local list of program properties for each clinic.
		///Only modifies other clinics if _listUserClinicNums[_indexClinicRevert]=0, meaning user just modified the HQ values.
		///If the clinic EdgeExpress client Username and Password are the same as HQ, the clinic values will be updated with the values entered.
		///If the clinic XWeb values are the same as HQ, the clinic XWeb values will be updated with the values entered.
		///If both the EdgeExpress client values and the XWeb values are the same as HQ, the payment type will be updated.
		///The values in the local list for HQ, or for the clinic modified if it was not HQ, have to be updated after calling this method.</summary>
		private void SyncWithHQ() {
			if(!PrefC.HasClinicsEnabled || _clinicNumRevert>0) {
				return;
			}
			string hqXWebID=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.XWebID,0);//HQ XWebID before updating to value in textbox
			string hqAuthKey=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.AuthKey,0);//HQ AuthKey before updating to value in textbox
			string hqTerminalID=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.TerminalID,0);//HQ TerminalID before updating to value in textbox
			string hqPayType=ProgramProperties.GetPropValFromList(_listProgProps,EdgeExpressProps.PaymentType,0);//HQ PaymentType before updating to combo box selection
			string payTypeCur="";
			if(comboPaymentType.SelectedIndex>-1) {
				payTypeCur=_listPayTypeDefs[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			//for each distinct ClinicNum in the prog property list for EdgeExpress except HQ
			foreach(long clinicNum in _listProgProps.Select(x => x.ClinicNum).Where(x => x>0).Distinct()) {
				//Updates the PaymentType in both if checks, in case the other isn't met so the payment type will be synched if either condition is true.
				//only if all three XWeb HQ values are not blank
				bool isXWebSynch=_listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc==EdgeExpressProps.XWebID && x.PropertyValue==hqXWebID)
					&& _listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc==EdgeExpressProps.AuthKey && x.PropertyValue==hqAuthKey)
					&& _listProgProps.Exists(x => x.ClinicNum==clinicNum && x.PropertyDesc==EdgeExpressProps.TerminalID && x.PropertyValue==hqTerminalID);
				if(!isXWebSynch) {
					continue;
				}
				UpdateProperty(clinicNum,EdgeExpressProps.XWebID,textXWebID.Text.Trim());
				UpdateProperty(clinicNum,EdgeExpressProps.AuthKey,textAuthKey.Text.Trim());
				UpdateProperty(clinicNum,EdgeExpressProps.TerminalID,textTerminalID.Text.Trim());
				//only synch payment type if both client and XWeb values are the same as HQ and the payment type is valid
				if(!string.IsNullOrEmpty(payTypeCur)) {
					_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==EdgeExpressProps.PaymentType && x.PropertyValue==hqPayType)
						.ForEach(x => x.PropertyValue=payTypeCur);//always 1 item; null safe
				}
			}
		}

		///<summary>Updates the program property for this clinic. Creates and adds to the list if it cannot be found.</summary>
		void UpdateProperty(long clinicNum,string propDesc,string propVal) {
			ProgramProperty prop=_listProgProps.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PropertyDesc==propDesc);
			if(prop==null) {
				prop=new ProgramProperty {
					ProgramNum=_progCur.ProgramNum,
					ClinicNum=clinicNum,
					PropertyDesc=propDesc,
				};
				_listProgProps.Add(prop);
			}
			prop.PropertyValue=propVal;
		}

		private void UpdateAllProperties(long clinicNum) {
			string payTypeCur="";
			if(comboPaymentType.SelectedIndex>-1) {
				payTypeCur=_listPayTypeDefs[comboPaymentType.SelectedIndex].DefNum.ToString();
			}
			UpdateProperty(clinicNum,EdgeExpressProps.PromptSignature,POut.Bool(checkPromptSig.Checked));
			UpdateProperty(clinicNum,EdgeExpressProps.PrintReceipt,POut.Bool(checkPrintReceipt.Checked));
			UpdateProperty(clinicNum,EdgeExpressProps.XWebID,textXWebID.Text.Trim());
			UpdateProperty(clinicNum,EdgeExpressProps.AuthKey,textAuthKey.Text.Trim());
			UpdateProperty(clinicNum,EdgeExpressProps.TerminalID,textTerminalID.Text.Trim());
			UpdateProperty(clinicNum,EdgeExpressProps.PaymentType,payTypeCur);
			UpdateProperty(clinicNum,EdgeExpressProps.ForceRecurringCharge,POut.Bool(checkForceDuplicate.Checked));
			UpdateProperty(clinicNum,EdgeExpressProps.PreventSavingNewCC,POut.Bool(checkPreventSavingNewCC.Checked));
			UpdateProperty(clinicNum,EdgeExpressProps.IsOnlinePaymentsEnabled,POut.Bool(checkWebPayEnabled.Checked));
		}

		private void checkWebPayEnabled_Click(object sender,EventArgs e) {
			if(!checkWebPayEnabled.Checked) {
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			ProgramProperty programPropertyWebPayEnabled=ProgramProperties.GetOnlinePaymentsEnabledForClinic(clinicNum,ProgramName.EdgeExpress);
			string msg=Lan.g(this,"Online payments is already enabled for another processor and must be disabled in order to use EdgeExpress online payments. "
				+"Would you like to disable the other processor for online payments?");
			if(programPropertyWebPayEnabled!=null) {
				if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					checkWebPayEnabled.Checked=false;
					return;
				}
				//User wants to set as new processor for online payments, add to the list for the current session to remove currently enabled program.
				_listWebPayProgProps.Add(programPropertyWebPayEnabled);
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(_progCur==null) {//should never happen
				MsgBox.Show(this,"EdgeExpress entry is missing from the database.");
				return;
			}
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.EdgeExpress,out string err)) {
				MsgBox.Show(err);
				return;
			}
			//EdgeExpress will be enabled for the clinic if the enabled checkbox is checked and the 3 XWeb fields are not blank. Don't let them switch 
			//clinics or close the form with only 1 or 2 of the three fields filled in.  If they fill in 1, they must fill in the other 2. 
			bool isXWebEnabled=checkEnabled.Checked && (textXWebID.Text.Trim().Length>0 || textAuthKey.Text.Trim().Length>0
				|| textTerminalID.Text.Trim().Length>0);
			if(isXWebEnabled && !ValidateXWeb()) {//error message box displayed in ValidateXWeb()
				return;
			}
			//if the user just modified the HQ credentials, change any credentials that were the same as HQ to keep them synched
			SyncWithHQ();
			//get selected ClinicNum (if enabled), PaymentType, encrypted Password, and encrypted AuthKey
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			UpdateAllProperties(clinicNum);
			//validate the payment type set for all clinics with EdgeExpress enabled
			if(!ValidatePaymentTypes(true)) {//if validation fails, message box will already have been shown
				return;
			}
			if(_progCur.Enabled!=checkEnabled.Checked) {
				_progCur.Enabled=checkEnabled.Checked;
				Programs.Update(_progCur);
			}
			ProgramProperties.Sync(_listProgProps,_progCur.ProgramNum);
			string strEdgeExpressOnlinePaymentsEnabled=EdgeExpressProps.IsOnlinePaymentsEnabled;
			//Find all clinics that have EdgeExpress online payments enabled
			List<ProgramProperty> listEdgeExpressOnlinePayments=_listProgProps.FindAll(x => x.PropertyDesc==strEdgeExpressOnlinePaymentsEnabled &&
				PIn.Bool(x.PropertyValue));
			for(int i=0;i < listEdgeExpressOnlinePayments.Count;i++) {
				//Find all online payment enabled program properties that we saved in this session. Only clinics that have changes will have an 
				//IsOnlinePaymentsEnabled property in memory. This is needed to ensure that we don't disable other processors if someone
				//checks to use EdgeExpress online payments and then decides to keep it disabled during the same session.
				ProgramProperty webOnlinePayments=_listWebPayProgProps.FirstOrDefault(y => y.ClinicNum==listEdgeExpressOnlinePayments[i].ClinicNum);
				if(webOnlinePayments!=null) {
					ProgramProperties.UpdateProgramPropertyWithValue(webOnlinePayments,POut.Bool(false));
				}
			}
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
