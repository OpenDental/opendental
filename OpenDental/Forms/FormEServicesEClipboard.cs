using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.AutoComm;

namespace OpenDental {
	public partial class FormEServicesEClipboard:FormODBase {
		#region Fields - Private
		///<summary>Helper to manager prefs relating to eClipboard and getting them to/from the db.</summary>
		private ClinicPrefHelper _clinicPrefHelper=new ClinicPrefHelper(
			PrefName.EClipboardUseDefaults,
			PrefName.EClipboardAllowSelfCheckIn,
			PrefName.EClipboardAllowPaymentOnCheckin,
			PrefName.EClipboardAllowSelfPortraitOnCheckIn,
			PrefName.EClipboardPresentAvailableFormsOnCheckIn,
			PrefName.EClipboardCreateMissingFormsOnCheckIn,
			PrefName.EClipboardPopupKioskOnCheckIn,
			PrefName.EClipboardEnableByodSms,
			PrefName.EClipboardAppendByodToArrivalResponseSms,
			PrefName.EClipboardByodSmsTemplate,
			PrefName.EClipboardMessageComplete,
			PrefName.EClipboardDoTwoFactorAuth,
			PrefName.EClipboardImageCaptureDefs,
			PrefName.EClipboardHasMultiPageCheckIn,
			PrefName.EClipboardClinicalValidationFrequency,
			PrefName.EClipClinicalAutoLogin);
		private bool _doSetInvalidClinicPrefs=false;
		private bool _canEditEClipboard;
		///<summary>A list of all eclipboard sheet defs that are edited in this window. Synced with the database list on the ok click.</summary>
		private List<EClipboardSheetDef> _listEClipboardSheetDefs;
		private string _enabledColumnText="Enabled";
		///<summary>A list of all eclipboard image capture defs that are edited in this window. Synced with the database list on the ok click.</summary>
		private List<EClipboardImageCaptureDef> _listEClipboardImageCaptureDefs;
		#endregion Fields - Private

		///<summary>The current clinic num for this tab, handles whether or not the practice has clinics.</summary>
		private long GetClinicNumEClipboardTab() {
			if(!PrefC.HasClinicsEnabled) {
				return 0; //No clinics, HQ clinic
			}
			if(clinicPickerEClipboard==null) {
				return 0; //combobox hasn't loaded yet
			}
			return clinicPickerEClipboard.SelectedClinicNum;
		}

		#region Constructor
		public FormEServicesEClipboard() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - FormEServices Boilerplate	
		private void FormEServicesEClipboard_Load(object sender,EventArgs e) {
			//Fill the list of sheets
			_listEClipboardSheetDefs=EClipboardSheetDefs.Refresh();
			//Fill the list of eclipboard image capture defs from the db.
			_listEClipboardImageCaptureDefs=EClipboardImageCaptureDefs.Refresh();
			//Set Clinics or No
			if(!PrefC.HasClinicsEnabled) {
				checkEClipboardUseDefaults.Visible=false;
			}
			else {
				clinicPickerEClipboard.SelectionChangeCommitted += new System.EventHandler(this.ClinicPickerEClipboard_SelectionChangeCommitted);
			}
			EClipboardSetControlsForClinicPrefs();
			//Subscribe to eclipboard events to fill grid if needed
			EClipboardEvent.Fired+=eClipboardChangedEvent_Fired;
			_canEditEClipboard=Security.IsAuthorized(Permissions.EServicesSetup,true);
			FillGridMobileAppDevices();
			FillGridEClipboardSheetInUse();
			FillImageCaptureFrequencyUI();
			SetUIEClipboardEnabled();
			textByodSmsTemplate.MouseHover+=new EventHandler((sender,e) => {
				string availableTags=Lans.g(this,"Available Tags:\n")+OpenDentBusiness.AutoComm.ByodTagReplacer.BYOD_TAG+Lans.g(this,"(required)\n")
					+string.Join(",",ApptReminderRules.GetAvailableTags(ApptReminderType.Undefined));
				ToolTip toolTip=new ToolTip();
				toolTip.SetToolTip(textByodSmsTemplate,availableTags);
			});
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			EClipboardPushPrefs();
			EClipboardSheetDefs.Sync(_listEClipboardSheetDefs,EClipboardSheetDefs.Refresh());
			EClipboardImageCaptureDefs.Sync(_listEClipboardImageCaptureDefs,EClipboardImageCaptureDefs.Refresh());
			EClipboardEvent.Fired-=eClipboardChangedEvent_Fired;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion Methods - FormEServices Boilerplate

		#region Methods - Event Handlers Main
		private void butConfStatuses_Click(object sender,EventArgs e) {
			FormEServicesAutoMsgingAdvanced formEServicesAutoMsgingAdvanced=new FormEServicesAutoMsgingAdvanced();
			formEServicesAutoMsgingAdvanced.ShowDialog();
		}

		private void ButEClipboardAddSheets_Click(object sender,EventArgs e) {
			using FormSheetDefs formSheetDefs=new FormSheetDefs(SheetTypeEnum.PatientForm,SheetTypeEnum.MedicalHistory,SheetTypeEnum.Consent);
			formSheetDefs.ShowDialog();
			FillGridEClipboardSheetInUse();
		}
		
		private void ButEClipboardRight_Click(object sender,EventArgs e) {
			if(listEClipboardSheetsAvailable.SelectedIndices.Count==0) {
				return;
			}
			List<SheetDef> listSheetDefs=listEClipboardSheetsAvailable.GetListSelected<SheetDef>();
			for(int i=0;i<listSheetDefs.Count;i++) {
				EClipboardSheetDef eClipboardSheetDef=_listEClipboardSheetDefs.FirstOrDefault(x => x.ClinicNum==GetClinicNumEClipboardTab() && x.SheetDefNum==listSheetDefs[i].SheetDefNum);
				if(eClipboardSheetDef==null) {
					eClipboardSheetDef=new EClipboardSheetDef();
					eClipboardSheetDef.SheetDefNum=listSheetDefs[i].SheetDefNum;
					eClipboardSheetDef.ClinicNum=GetClinicNumEClipboardTab();
					eClipboardSheetDef.ResubmitInterval=TimeSpan.FromDays(30);
					eClipboardSheetDef.MinAge=-1;
					eClipboardSheetDef.MaxAge=-1;
					_listEClipboardSheetDefs.Add(eClipboardSheetDef);
				}
			}
			FillGridEClipboardSheetInUse();
			SetEClipboardSheetOrder();
		}

		private void ButEClipboardLeft_Click(object sender,EventArgs e) {
			if(gridEClipboardSheetsInUse.SelectedIndices.Count()==0) {
				return;
			}
			//grid selection mode is One so there should be exactly one row here to remove
			_listEClipboardSheetDefs.Remove((EClipboardSheetDef)gridEClipboardSheetsInUse.SelectedGridRows.First().Tag);
			FillGridEClipboardSheetInUse();
			SetEClipboardSheetOrder();
		}

		private void ButEClipboardUp_Click(object sender,EventArgs e) {
			SwapSheets(doMoveUp:true);
		}

		private void ButEClipboardDown_Click(object sender,EventArgs e) {
			SwapSheets(doMoveUp:false);
		}

		private void ClinicPickerEClipboard_SelectionChangeCommitted(object sender,EventArgs e) {
			EClipboardSetControlsForClinicPrefs();
			FillGridMobileAppDevices();
			FillGridEClipboardSheetInUse();
			FillImageCaptureFrequencyUI();
			SetUIEClipboardEnabled();
		}

		public void eClipboardChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.eClipboard || this.IsDisposed) {
				return;
			}
			FillGridMobileAppDevices();
		}

		private void GridEClipboardSheetsInUse_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormEClipboardSheetRules formEClipboardSheetRules=new FormEClipboardSheetRules((EClipboardSheetDef)gridEClipboardSheetsInUse.ListGridRows[e.Row].Tag,gridEClipboardSheetsInUse.ListGridRows.Select(x=>(EClipboardSheetDef)x.Tag).ToList());
			formEClipboardSheetRules.ShowDialog();
			FillGridEClipboardSheetInUse();
		}

		private void gridMobileAppDevices_CellClick(object sender,ODGridClickEventArgs e) {
			int idxEnabledColumn=gridMobileAppDevices.Columns.GetIndex(_enabledColumnText);
			if(e.Col!=idxEnabledColumn) {//They did not select the right column.
				return;
			}
			MobileAppDevice mobileAppDevice=gridMobileAppDevices.SelectedTag<MobileAppDevice>();
			//There is not a tag somehow.
			if(mobileAppDevice==null) {
				return;
			}
			if(mobileAppDevice.IsAllowed){
				if(mobileAppDevice.PatNum>0) {
					MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
						" or wait until the patient is no longer using the device.");
					return;
				}
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will immediately make the device unavailable to all other workstations. Continue?")) {
					return;
				}
			}
			mobileAppDevice.IsAllowed=!mobileAppDevice.IsAllowed;//Flip the bit.
			//Update the device because the signal processing of this form isn't friendly to keeping an in-memory list that syncs when the form closes
			MobileAppDevices.Update(mobileAppDevice);
			OpenDentBusiness.WebTypes.PushNotificationUtils.CI_IsAllowedChanged(mobileAppDevice.MobileAppDeviceNum,mobileAppDevice.IsAllowed);
			FillGridMobileAppDevices();	//Fill the grid to show the changes.
		}

		private void butImageOptions_Click(object sender,EventArgs e) {
			FormEClipboardImageCaptureDefs formEClipboardImageCaptureDefs=new FormEClipboardImageCaptureDefs(GetClinicNumEClipboardTab());
			//This creates a copy of the list, and a shallow copy of the objects in the list. This is needed to avoid bugs where changes made to 'ListEClipboardImageCaptureDefs'
			//inside of formEClipboardImageCaptureDefs are still kept even if the user clicks cancel in that form.
			formEClipboardImageCaptureDefs.ListEClipboardImageCaptureDefs=_listEClipboardImageCaptureDefs.Select(x => x.Copy()).ToList();
			formEClipboardImageCaptureDefs.ShowDialog();
			if(formEClipboardImageCaptureDefs.DialogResult==DialogResult.OK) {
				//On OK click, update the list of eclipboard image capture defs to reflect any changes the user made to which images user are allowed to take and their frequencies
				_listEClipboardImageCaptureDefs=formEClipboardImageCaptureDefs.ListEClipboardImageCaptureDefs.Select(x => x.Copy()).ToList();
				//Need to update the 'EClipboardAllowSelfPortraitOnCheckIn' pref in the clinicprefhelper since that pref is toggled in the previously opened form.
				UpdateAllowSelfPortraitPref();
				//Update the UI to reflect the new changes
				FillImageCaptureFrequencyUI();
			}
		}

		private void butSecurity_Click(object sender,EventArgs e) {
			//Permissions check presumably. Discuss with Sam.
			using FormEClipboardSecurityEdit formEClipSecEdit=new FormEClipboardSecurityEdit();
			int.TryParse(_clinicPrefHelper.GetStringVal(PrefName.EClipboardClinicalValidationFrequency,clinicPickerEClipboard.SelectedClinicNum),out int securityFreq);
			formEClipSecEdit.SecurityFrequency=securityFreq;
			if(formEClipSecEdit.ShowDialog()==DialogResult.OK) {
				_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardClinicalValidationFrequency,clinicPickerEClipboard.SelectedClinicNum,POut.Int(formEClipSecEdit.SecurityFrequency));
			}
		}
		#endregion Methods - Event Handlers Main

		#region Methods - Event Handlers Prefs Section
		private void CheckEClipboardUseDefaults_Click(object sender, EventArgs e){
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardUseDefaults,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardUseDefaults.Checked));
			if(checkEClipboardUseDefaults.Checked) {//If set to true, set the behavior rules and sheets to the default
				EClipboardSetControlsToPrefDefaults();
				_listEClipboardSheetDefs.RemoveAll(x => GetClinicNumEClipboardTab()!=0 && x.ClinicNum==GetClinicNumEClipboardTab());
				_listEClipboardImageCaptureDefs.RemoveAll(x => GetClinicNumEClipboardTab()!=0 && x.ClinicNum==GetClinicNumEClipboardTab());
			}
			else{
				//user unchecked the box, but nothing should change.  Other 5 checkboxes just stay with the old default values.  They can edit from here, which would override the defaults.
			}
			//disable the ability to edit rules if applicable
			FillGridEClipboardSheetInUse();
			FillImageCaptureFrequencyUI();
			SetUIEClipboardEnabled();
		}

		private void CheckEClipboardAllowCheckIn_Click(object sender, EventArgs e){
			string strAllowCheckIn=POut.Bool(checkEClipboardAllowCheckIn.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.SelectedClinicNum,strAllowCheckIn);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowSelfCheckIn,strAllowCheckIn);
		}

		private void CheckAllowPaymentCheckIn_Click(object sender,EventArgs e) {
			string strAllowPayments=POut.Bool(checkEClipboardAllowPaymentCheckIn.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowPaymentOnCheckin,clinicPickerEClipboard.SelectedClinicNum,strAllowPayments);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowPaymentOnCheckin,strAllowPayments);
		}

		private void CheckEClipboardAllowSheets_Click(object sender, EventArgs e){
			string strAllowSheets=POut.Bool(checkEClipboardAllowSheets.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strAllowSheets);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPresentAvailableFormsOnCheckIn,strAllowSheets);
		}

		private void CheckEClipboardCreateMissingForms_Click(object sender, EventArgs e){
			SetUIEClipboardEnabled();
			string strCanCreateMissingForms=POut.Bool(checkEClipboardCreateMissingForms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strCanCreateMissingForms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardCreateMissingFormsOnCheckIn,strCanCreateMissingForms);
		}

		private void CheckEClipboardPopupKiosk_Click(object sender, EventArgs e){
			string strHasPopupKiosk=POut.Bool(checkEClipboardPopupKiosk.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strHasPopupKiosk);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPopupKioskOnCheckIn,strHasPopupKiosk);
		}
		private void checkEnableByodSms_Click(object sender,EventArgs e) {
			SetUIEClipboardEnabled();
			string strEnableByodSms=POut.Bool(checkEnableByodSms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.SelectedClinicNum,strEnableByodSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardEnableByodSms,strEnableByodSms);
		}

		private void checkAppendByodToArrivalResponseSms_Click(object sender,EventArgs e) {
			string strByodForResponseSms=POut.Bool(checkAppendByodToArrivalResponseSms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.SelectedClinicNum,strByodForResponseSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAppendByodToArrivalResponseSms,strByodForResponseSms);
		}

		private void checkRequire2FA_Click(object sender, EventArgs e) {
			string strRequire2FA=POut.Bool(checkRequire2FA.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.SelectedClinicNum,strRequire2FA);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardDoTwoFactorAuth,strRequire2FA);
		}

		private void checkDisplayIndividually_Click(object sender, EventArgs e) {
			string strChecked=POut.Bool(checkDisplayIndividually.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardHasMultiPageCheckIn,clinicPickerEClipboard.SelectedClinicNum,strChecked);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardHasMultiPageCheckIn,strChecked);
		}

		private void checkClinicalAutoLogin_CheckedChanged(object sender,EventArgs e) {
			string strChecked=POut.Bool(checkClinicalAutoLogin.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipClinicalAutoLogin,clinicPickerEClipboard.SelectedClinicNum,strChecked);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipClinicalAutoLogin,strChecked);
		}

		private void TextByodSmsTemplate_TextChanged(object sender, EventArgs e){
			if(!textByodSmsTemplate.Text.Contains(ByodTagReplacer.BYOD_TAG)) {
				return;//Do not track these changes while the byod tag is not present.
			}
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.SelectedClinicNum,textByodSmsTemplate.Text);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardByodSmsTemplate,textByodSmsTemplate.Text);
		}
		
		private void textByodSmsTemplate_Leave(object sender,EventArgs e) {
			if(textByodSmsTemplate.Text.Contains(ByodTagReplacer.BYOD_TAG)) {
				return;
			}
			MsgBox.Show(this,$"eClipboard link template must contain {ByodTagReplacer.BYOD_TAG}");
			textByodSmsTemplate.Select();
		}

		private void TextEClipboardMessage_TextChanged(object sender, EventArgs e){
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.SelectedClinicNum,textEClipboardMessage.Text);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardMessageComplete,textEClipboardMessage.Text);
		}
		#endregion Methods - Event Handlers Prefs Section

		#region Methods - Private
		private void EClipboardPushPrefs() {
			//First get all of the clinic nums that have preference changes
			List<long> listClinicNumsChanged=_clinicPrefHelper.GetClinicsWithChanges();
			if(listClinicNumsChanged.Contains(0)) {
				//Default clinic has changed so find any clinics that used defaults. They have changed by association.
				listClinicNumsChanged.AddRange(
					//The default option only appears for users that don't have restricted clinic access, so start with the list of all non-hidden clinics
					Clinics.GetDeepCopy(isShort:true)
					//Filter to only those clinics that currently use defaults (they have no rules of their own).
					.Where(x => ClinicPrefs.GetBool(PrefName.EClipboardUseDefaults,x.ClinicNum))
					//Only need the clinicNums.
					.Select(x => x.ClinicNum)
				);
			}
			//todo: Validate ByodSmsTempalte has [ByodUrl]
			//Save to db.  Form's _doSetInvalidClinicPrefs will trigger signals.
			if(_clinicPrefHelper.SyncAllPrefs()) {
				_doSetInvalidClinicPrefs=true;
			}
			//Push all of the new preference values to the clinic nums that have preference changes
			List<long> listClinicNumsDistinct=listClinicNumsChanged.Distinct().ToList();
			for(int i=0;i<listClinicNumsDistinct.Count;i++) {
				OpenDentBusiness.WebTypes.PushNotificationUtils.CI_NewEClipboardPrefs(listClinicNumsDistinct[i]);
			}
		}

		///<summary>Updates the eclipboard image defs textbox to show which eclipboard images users are allowed to submit and the frequenicies at
		///which they should submit images.</summary>
		private void FillImageCaptureFrequencyUI() {
			long clinicNum=0;
			if(!checkEClipboardUseDefaults.Checked) { 
				clinicNum=GetClinicNumEClipboardTab();
			}
			//Separate the EClipboard Images defcat capturedefs from the self portrait capturedef and store each in their respective variables.
			List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefs=_listEClipboardImageCaptureDefs.FindAll(x => !x.IsSelfPortrait && x.ClinicNum==clinicNum);
			EClipboardImageCaptureDef eClipboardImageCaptureDefSelfPortrait=_listEClipboardImageCaptureDefs.Find(x => x.IsSelfPortrait && x.ClinicNum==clinicNum);
			List<string> listImageCaptureDefs=new List<string>();
			//If patient is allowed to submit self portrait, indicated by the self portrait having an eclipboardimagecapturedef record for this clinic, then we add it to
			//our list of strings that will be displayed in the UI.
			if(eClipboardImageCaptureDefSelfPortrait!=null) {
				listImageCaptureDefs.Add("Self Portrait ("+eClipboardImageCaptureDefSelfPortrait.FrequencyDays+")");
			}
			//From the list of EClipboardImageCaptureDef objects for the EClipboard Images patients are allowed to take, create a list of strings indicating the name of the
			//def (def in Eclipboard Images DefCat) and the frequency at which the patient will be prompted to submit the image
			for(int i=0;i<listEClipboardImageCaptureDefs.Count;i++) {
				listImageCaptureDefs.Add(Defs.GetDef(DefCat.EClipboardImageCapture,listEClipboardImageCaptureDefs[i].DefNum).ItemName+" ("+listEClipboardImageCaptureDefs[i].FrequencyDays+")");
			}
			textEclipboardImageDefs.Text=string.Join(", ",listImageCaptureDefs);
		}

		///<summary>Doesn't touch the Defaults checkbox itself.  Sets the 5 checkboxes and the textbox that are involved in prefs.  Importantly, it also sends those same default values to the list of clinicprefs in memory where they will later be synched.</summary>
		private void EClipboardSetControlsToPrefDefaults(){
			//EClipboardUseDefaults not included here
			checkEClipboardAllowCheckIn.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardAllowSelfCheckIn);
			checkEClipboardAllowPaymentCheckIn.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardAllowPaymentOnCheckin);
			checkEClipboardAllowSheets.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardPresentAvailableFormsOnCheckIn);
			checkEClipboardCreateMissingForms.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardCreateMissingFormsOnCheckIn);
			checkEClipboardPopupKiosk.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardPopupKioskOnCheckIn);
			checkEnableByodSms.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardEnableByodSms);
			checkAppendByodToArrivalResponseSms.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardAppendByodToArrivalResponseSms);
			checkDisplayIndividually.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardHasMultiPageCheckIn);
			checkClinicalAutoLogin.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipClinicalAutoLogin);
			checkRequire2FA.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardDoTwoFactorAuth);
			textByodSmsTemplate.Text=_clinicPrefHelper.GetDefaultStringVal(PrefName.EClipboardByodSmsTemplate);
			textEClipboardMessage.Text=_clinicPrefHelper.GetDefaultStringVal(PrefName.EClipboardMessageComplete);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowCheckIn.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowPaymentOnCheckin,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowPaymentCheckIn.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowSheets.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardCreateMissingForms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardPopupKiosk.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEnableByodSms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkAppendByodToArrivalResponseSms.Checked && checkEnableByodSms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.SelectedClinicNum,
				textByodSmsTemplate.Text);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.SelectedClinicNum,
				textEClipboardMessage.Text);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkRequire2FA.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardHasMultiPageCheckIn,clinicPickerEClipboard.SelectedClinicNum,POut.Bool(checkDisplayIndividually.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipClinicalAutoLogin,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkClinicalAutoLogin.Checked));
			UpdateAllowSelfPortraitPref();
		}

		///<summary>Sets the Defaults checkbox itself.  Then sets the 5 other checkboxes and the textbox that are involved in prefs.  Sets them based on the values in the local clinicpref list.  Does not change any of those values.  Called only on startup.</summary>
		private void EClipboardSetControlsForClinicPrefs(){
			long clinicNum=clinicPickerEClipboard.SelectedClinicNum;
			checkEClipboardUseDefaults.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardUseDefaults,clinicNum);
			if(checkEClipboardUseDefaults.Checked) {
				EClipboardSetControlsToPrefDefaults();
			}
			else {
				checkEClipboardAllowCheckIn.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardAllowSelfCheckIn,clinicNum);
				checkEClipboardAllowPaymentCheckIn.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardAllowPaymentOnCheckin,clinicNum);
				checkEClipboardAllowSheets.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicNum);
				checkEClipboardCreateMissingForms.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicNum);
				checkEClipboardPopupKiosk.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardPopupKioskOnCheckIn,clinicNum);
				checkEnableByodSms.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardEnableByodSms,clinicNum);
				checkAppendByodToArrivalResponseSms.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicNum)
					&& checkEnableByodSms.Checked;
				checkRequire2FA.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardDoTwoFactorAuth,clinicNum);
				checkDisplayIndividually.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipboardHasMultiPageCheckIn,clinicNum);
				checkClinicalAutoLogin.Checked=_clinicPrefHelper.GetBoolVal(PrefName.EClipClinicalAutoLogin,clinicNum);
				textByodSmsTemplate.Text=_clinicPrefHelper.GetStringVal(PrefName.EClipboardByodSmsTemplate,clinicNum);
				textEClipboardMessage.Text=_clinicPrefHelper.GetStringVal(PrefName.EClipboardMessageComplete,clinicNum);
			}
			if(!LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipClinical,clinicNum)) {
				checkClinicalAutoLogin.Enabled=false;
				checkClinicalAutoLogin.Visible=false;
				return;
			}
		}

		///<summary>Fills listEClipboardSheetsAvailable and small grid to its right.</summary>
		private void FillGridEClipboardSheetInUse() {
			//Fill the list of available sheets with the custom PatientForm and MedicalHist sheet defs
			List<SheetDef> listSheetDefs=new List<SheetDef>();
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.PatientForm));
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.MedicalHistory));
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.Consent));
			//Clear any custom sheet defs that don't have a mobile layout
			listSheetDefs.RemoveAll(x => !x.HasMobileLayout);
			//Get the list of in-memory eclipboard sheets
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			List<EClipboardSheetDef> listEClipboardSheetDefs=_listEClipboardSheetDefs.FindAll(x => x.ClinicNum==clinicNum);
			//Put the sheets that are in use into the grid of sheets in use
			gridEClipboardSheetsInUse.ListGridRows.Clear();
			gridEClipboardSheetsInUse.Columns.Clear();
			gridEClipboardSheetsInUse.BeginUpdate();
			GridColumn col=new GridColumn("Sheet Name",180);
			gridEClipboardSheetsInUse.Columns.Add(col);
			col=new GridColumn("Behavior",180);
			gridEClipboardSheetsInUse.Columns.Add(col);
			col=new GridColumn("Min Age",40,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.Columns.Add(col);
			col=new GridColumn("Max Age",40,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.Columns.Add(col);
			string addOn="";
			List<EClipboardSheetDef> listEClipboardSheetDefsOrdered=listEClipboardSheetDefs.OrderBy(x => x.ItemOrder).ToList();
			string gridCellValue;
			for(int i=0;i<listEClipboardSheetDefsOrdered.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(SheetDefs.GetDescription(listEClipboardSheetDefsOrdered[i].SheetDefNum));
				GridCell gridCell=new GridCell(Lan.g("enumPrefillCondition",listEClipboardSheetDefsOrdered[i].PrefillStatus.ToString()));
				row.Cells.Add(gridCell);
				gridCellValue="Not Set";
				if(listEClipboardSheetDefsOrdered[i].MinAge>0) {
					gridCellValue=listEClipboardSheetDefsOrdered[i].MinAge.ToString();
				}
				row.Cells.Add(gridCellValue);
				gridCellValue="Not Set";
				if(listEClipboardSheetDefsOrdered[i].MaxAge>0) {
					gridCellValue=listEClipboardSheetDefsOrdered[i].MaxAge.ToString();
				}
				row.Cells.Add(gridCellValue);
				row.Cells.Add(listEClipboardSheetDefsOrdered[i].ResubmitInterval.TotalDays.ToString()+addOn);
				row.Tag=listEClipboardSheetDefsOrdered[i];
				gridEClipboardSheetsInUse.ListGridRows.Add(row);
			}
			col=new GridColumn("Frequency (Days)",100,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.Columns.Add(col);
			gridEClipboardSheetsInUse.EndUpdate();
			//Put the sheets that are not in use to the list of available sheets
			listEClipboardSheetsAvailable.Items.Clear();
			listSheetDefs.RemoveAll(x => listEClipboardSheetDefs.Select(y => y.SheetDefNum).Contains(x.SheetDefNum));
			for(int i=0;i<listSheetDefs.Count;i++) {
				listEClipboardSheetsAvailable.Items.Add(listSheetDefs[i].Description,listSheetDefs[i]);
			}
		}

		///<summary>Fills the big main grid.</summary>
		private void FillGridMobileAppDevices() {
			gridMobileAppDevices.BeginUpdate();
			//Columns
			gridMobileAppDevices.Columns.Clear();
			GridColumn col=new GridColumn("Device Name",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last Attempt",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last Login",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn("Clinic",100){IsWidthDynamic=true };
				gridMobileAppDevices.Columns.Add(col);
			}
			col=new GridColumn("Device State",100) {IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn(_enabledColumnText,50,HorizontalAlignment.Center);
			gridMobileAppDevices.Columns.Add(col);
			if(_canEditEClipboard) {
				col=new GridColumn("Delete",45,HorizontalAlignment.Center);
				gridMobileAppDevices.Columns.Add(col);
			}
			//Rows
			gridMobileAppDevices.ListGridRows.Clear();
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForUser(Security.CurUser);
			if(GetClinicNumEClipboardTab()>0) {
				listMobileAppDevices.RemoveAll(x => x.ClinicNum!=GetClinicNumEClipboardTab());
			}
			for(int i=0;i<listMobileAppDevices.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listMobileAppDevices[i].DeviceName+"\r\n("+listMobileAppDevices[i].UniqueID+")");
				string rowValue="";
				if(listMobileAppDevices[i].LastAttempt.Year>1880) {
					rowValue=listMobileAppDevices[i].LastAttempt.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(listMobileAppDevices[i].LastLogin.Year>1880) {
					rowValue=listMobileAppDevices[i].LastLogin.ToString();
				}
				row.Cells.Add(rowValue);
				if(PrefC.HasClinicsEnabled) {
					if(listMobileAppDevices[i].ClinicNum==0) {
						rowValue=Clinics.GetPracticeAsClinicZero().Abbr;
					}
					else {
						rowValue=Clinics.GetClinic(listMobileAppDevices[i].ClinicNum).Abbr;
					}
					row.Cells.Add(rowValue);
				}
				row.Cells.Add(listMobileAppDevices[i].DevicePage.GetDescription());
				row.Cells.Add((listMobileAppDevices[i].IsAllowed ? "X" : ""));
				if(_canEditEClipboard) {
					#region Delete click handler
					void DeleteClick(object sender,EventArgs e) {
						//int i in the for loop doesn't actually get disposed once the loop ends, because it was being used here to access 
						//listMobileAppDevicesToShow. i was always i+1, no matter which row you clicked, so use selected index instead.
						if(listMobileAppDevices[gridMobileAppDevices.SelectedIndices[0]].PatNum>0) {
							MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
								" or wait until the patient is no longer using the device.");
							return;
						}
						if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will immediately remove the device from the database and all other workstations." +
							" Continue?")) {
							return;
						}
						MobileAppDevices.Delete(listMobileAppDevices[gridMobileAppDevices.SelectedIndices[0]].MobileAppDeviceNum);
						FillGridMobileAppDevices();
					}
					#endregion Delete click handler
					GridCell gridCell=new GridCell("Delete");
					gridCell.ColorBackG=Color.LightGray;
					gridCell.ClickEvent=DeleteClick;
					row.Cells.Add(gridCell);
				}
				row.Tag=listMobileAppDevices[i];
				gridMobileAppDevices.ListGridRows.Add(row);
			}
			gridMobileAppDevices.EndUpdate();
		}

		private void SetEClipboardSheetOrder() {
			if(gridEClipboardSheetsInUse.ListGridRows.Count<1) {
				return;
			}
			int idx=0;
			List<EClipboardSheetDef> listEClipboardSheetDefs=gridEClipboardSheetsInUse.ListGridRows.Select(x => (EClipboardSheetDef)x.Tag).ToList();
			for(int i=0;i<listEClipboardSheetDefs.Count;i++) {
				listEClipboardSheetDefs[i].ItemOrder=idx;
				idx++;
			}
		}

		///<summary>Called when user clicks on use defaults for clinic, AuthorizeTab, clinicPicker.SelectedIndexChanged, and CheckEClipboardCreateMissingForms_Click.  It sets various areas enabled or disabled.  Doesn't change the checked values.</summary>
		private void SetUIEClipboardEnabled() {
			//bool isClinicSignedUp=EClipboardDisplayAsEnabled();
			bool isClinicSignedUp=MobileAppDevices.IsClinicSignedUpForEClipboard(GetClinicNumEClipboardTab());
			if(PrefC.HasClinicsEnabled && GetClinicNumEClipboardTab()==0) {
				isClinicSignedUp=Clinics.GetForUserod(Security.CurUser).Any(x => MobileAppDevices.IsClinicSignedUpForEClipboard(x.ClinicNum));
			}
			bool notUsingDefaults=GetClinicNumEClipboardTab()==0 || !checkEClipboardUseDefaults.Checked;
			bool enableSheets=checkEClipboardCreateMissingForms.Checked;
			checkEClipboardUseDefaults.Enabled=GetClinicNumEClipboardTab()!=0 && isClinicSignedUp && _canEditEClipboard;
			checkAppendByodToArrivalResponseSms.Enabled=checkEnableByodSms.Checked;
			textByodSmsTemplate.Enabled=checkEnableByodSms.Checked;
			groupEClipboardRules.Enabled=isClinicSignedUp && _canEditEClipboard && notUsingDefaults;
			groupEClipboardSheets.Enabled=isClinicSignedUp && _canEditEClipboard && notUsingDefaults && enableSheets;
			gridMobileAppDevices.Enabled=isClinicSignedUp;
			labelEClipboardNotSignedUp.Visible=!isClinicSignedUp;
			butSecurity.Visible=LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipClinical,GetClinicNumEClipboardTab());
		}

		private void SwapSheets(bool doMoveUp) {
			if(gridEClipboardSheetsInUse.SelectedIndices.Count()==0) {
				return;
			}
			int selectedIdx=gridEClipboardSheetsInUse.GetSelectedIndex();
			if(doMoveUp && selectedIdx==0) {
				return;
			}
			if(!doMoveUp && selectedIdx==gridEClipboardSheetsInUse.ListGridRows.Count-1) {
				return;
			}
			int swapIdx=selectedIdx;
			if(doMoveUp) {
				swapIdx-=1;
			}
			else {
				swapIdx+=1;
			}
			EClipboardSheetDef eClipboardSheetDefSelected=(EClipboardSheetDef)gridEClipboardSheetsInUse.ListGridRows[selectedIdx].Tag;
			EClipboardSheetDef eClipboardSheetDefSwap=(EClipboardSheetDef)gridEClipboardSheetsInUse.ListGridRows[swapIdx].Tag;
			int selectedItemOrder=eClipboardSheetDefSelected.ItemOrder;
			eClipboardSheetDefSelected.ItemOrder=eClipboardSheetDefSwap.ItemOrder;
			eClipboardSheetDefSwap.ItemOrder=selectedItemOrder;
			FillGridEClipboardSheetInUse();
			gridEClipboardSheetsInUse.SetSelected(swapIdx,setValue:true);
		}

		private void UpdateEClipboardDefaultsIfNeeded(PrefName prefName,string newVal) {
			if(GetClinicNumEClipboardTab()==0) { //we are making changes to the default
				List<ClinicPref> listClinicPrefs=_clinicPrefHelper.GetWhere(PrefName.EClipboardUseDefaults,"1").ToList();
				for(int i=0;i<listClinicPrefs.Count;i++) {
					_clinicPrefHelper.ValChangedByUser(prefName,listClinicPrefs[i].ClinicNum,newVal);
				}
			}
		}

		private void UpdateAllowSelfPortraitPref() {
			long clinicNum=0;
			if(!checkEClipboardUseDefaults.Checked) {
				clinicNum=GetClinicNumEClipboardTab();
			}
			bool isSelfPortraitAllowed=_listEClipboardImageCaptureDefs.Any(x => x.IsSelfPortrait && x.ClinicNum==clinicNum);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,POut.Bool(isSelfPortraitAllowed));
		}
		#endregion Methods - Private
	}
}