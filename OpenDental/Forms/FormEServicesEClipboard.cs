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
		private ClinicPrefHelper _clinicPrefHelperEClipboard=new ClinicPrefHelper(
			PrefName.EClipboardUseDefaults,
			PrefName.EClipboardAllowSelfCheckIn,
			PrefName.EClipboardAllowSelfPortraitOnCheckIn,
			PrefName.EClipboardPresentAvailableFormsOnCheckIn,
			PrefName.EClipboardCreateMissingFormsOnCheckIn,
			PrefName.EClipboardPopupKioskOnCheckIn,
			PrefName.EClipboardEnableByodSms,
			PrefName.EClipboardAppendByodToArrivalResponseSms,
			PrefName.EClipboardByodSmsTemplate,
			PrefName.EClipboardMessageComplete,
			PrefName.EClipboardDoTwoFactorAuth,
			PrefName.EClipboardImageCaptureDefs);
		private bool _doSetInvalidClinicPrefs=false;
		private bool _eClipboardAllowEdit;
		///<summary>A list of all eclipboard sheet defs that are edited in this window. Synced with the database list on the ok click.</summary>
		private List<EClipboardSheetDef> _listEClipboardSheetDefs;
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
			_eClipboardAllowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			FillGridMobileAppDevices();
			FillGridEClipboardSheetInUse();
			SetUIEClipboardEnabled();
			textByodSmsTemplate.MouseHover+=new EventHandler((sender,e) => {
				string availableTags=Lans.g(this,"Available Tags:\n")+OpenDentBusiness.AutoComm.ByodTagReplacer.BYOD_TAG+Lans.g(this,"(required)\n")
					+string.Join(",",ApptReminderRules.GetAvailableTags(ApptReminderType.Undefined));
				ToolTip tip=new ToolTip();
				tip.SetToolTip(textByodSmsTemplate,availableTags);
			});
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			EClipboardPushPrefs();
			EClipboardSheetDefs.Sync(_listEClipboardSheetDefs,EClipboardSheetDefs.Refresh());
			EClipboardEvent.Fired-=eClipboardChangedEvent_Fired;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion Methods - FormEServices Boilerplate

		#region Methods - Event Handlers Main
		private void butConfStatuses_Click(object sender,EventArgs e) {
			FormEServicesAutoMsgingAdvanced formACS=new FormEServicesAutoMsgingAdvanced();
			formACS.ShowDialog();
		}

		private void ButEClipboardAddSheets_Click(object sender,EventArgs e) {
			using FormSheetDefs formSD=new FormSheetDefs(SheetTypeEnum.PatientForm,SheetTypeEnum.MedicalHistory,SheetTypeEnum.Consent);
			formSD.ShowDialog();
			FillGridEClipboardSheetInUse();
		}
		
		private void ButEClipboardRight_Click(object sender,EventArgs e) {
			if(listEClipboardSheetsAvailable.SelectedIndices.Count==0) {
				return;
			}
			List<SheetDef> listEClipboardSheetDefs=listEClipboardSheetsAvailable.GetListSelected<SheetDef>();
			for(int i=0;i<listEClipboardSheetDefs.Count;i++) {
				EClipboardSheetDef eClipboardSheetDef=_listEClipboardSheetDefs.FirstOrDefault(x => x.ClinicNum==GetClinicNumEClipboardTab() && x.SheetDefNum==listEClipboardSheetDefs[i].SheetDefNum);
				if(eClipboardSheetDef==null) {
					eClipboardSheetDef=new EClipboardSheetDef() {
						SheetDefNum=listEClipboardSheetDefs[i].SheetDefNum,
						ClinicNum=GetClinicNumEClipboardTab(),
						ResubmitInterval=TimeSpan.FromDays(30),
						MinAge=-1,
						MaxAge=-1
					};
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
			SwapSheets(moveUp:true);
		}

		private void ButEClipboardDown_Click(object sender,EventArgs e) {
			SwapSheets(moveUp:false);
		}

		private void ClinicPickerEClipboard_SelectionChangeCommitted(object sender,EventArgs e) {
			EClipboardSetControlsForClinicPrefs();
			FillGridMobileAppDevices();
			FillGridEClipboardSheetInUse();
			SetUIEClipboardEnabled();
		}

		public void eClipboardChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.eClipboard || this.IsDisposed) {
				return;
			}
			FillGridMobileAppDevices();
		}

		private void GridEClipboardSheetsInUse_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormEClipboardSheetRules formESR=new FormEClipboardSheetRules((EClipboardSheetDef)gridEClipboardSheetsInUse.ListGridRows[e.Row].Tag);
			formESR.ShowDialog();
			FillGridEClipboardSheetInUse();
		}

		private void gridMobileAppDevices_CellClick(object sender,ODGridClickEventArgs e) {
			int indexOfEnabledColumn;
			if(PrefC.HasClinicsEnabled){
				indexOfEnabledColumn=4;
			}
			else{
				indexOfEnabledColumn=3;
			}
			
			if(e.Col!=indexOfEnabledColumn) {//They did not select the right column.
				return;
			}
			MobileAppDevice mobileAppDevice=gridMobileAppDevices.SelectedTag<MobileAppDevice>();
			//There is not a tag somehow.
			if(mobileAppDevice==null) {
				return;
			}
			if(mobileAppDevice.IsAllowed){
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
			List<string> listImagePrefNames=textEclipboardImageDefs.Text.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).ToList();
			List<Def> listDefs=new List<Def>();
			for(int i=0;i<listImagePrefNames.Count();i++) {
				listDefs.Add(Defs.GetDefByExactName(DefCat.EClipboardImageCapture,listImagePrefNames[i]));
			}
			using FormDefinitionPicker formDefPicker=new FormDefinitionPicker(DefCat.EClipboardImageCapture,listDefs);
			formDefPicker.IsMultiSelectionMode=true;
			formDefPicker.ShowDialog();
			if(formDefPicker.DialogResult==DialogResult.OK) {
				string defsForPref=string.Join(",",formDefPicker.ListDefsSelected.Select(x=>x.DefNum));
				textEclipboardImageDefs.Text=EClipboardGetImageDefsFromPref(defsForPref);
				_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardImageCaptureDefs,clinicPickerEClipboard.SelectedClinicNum,defsForPref);
			}
		}
		#endregion Methods - Event Handlers Main

		#region Methods - Event Handlers Prefs Section
		private void CheckEClipboardUseDefaults_Click(object sender, EventArgs e){
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardUseDefaults,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardUseDefaults.Checked));
			if(checkEClipboardUseDefaults.Checked) {//If set to true, set the behavior rules and sheets to the default
				EClipboardSetControlsToPrefDefaults();
				_listEClipboardSheetDefs.RemoveAll(x => GetClinicNumEClipboardTab()!=0 && x.ClinicNum==GetClinicNumEClipboardTab());
			}
			else{
				//user unchecked the box, but nothing should change.  Other 5 checkboxes just stay with the old default values.  They can edit from here, which would override the defaults.
			}
			//disable the ability to edit rules if applicable
			FillGridEClipboardSheetInUse();
			SetUIEClipboardEnabled();
		}

		private void CheckEClipboardAllowCheckIn_Click(object sender, EventArgs e){
			string strAllowCheckIn=POut.Bool(checkEClipboardAllowCheckIn.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.SelectedClinicNum,strAllowCheckIn);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowSelfCheckIn,strAllowCheckIn);
		}

		private void CheckEClipboardAllowSelfPortrait_Click(object sender, EventArgs e){
			string strAllowSelfPortrait=POut.Bool(checkEClipboardAllowSelfPortrait.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strAllowSelfPortrait);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowSelfPortraitOnCheckIn,strAllowSelfPortrait);
		}

		private void CheckEClipboardAllowSheets_Click(object sender, EventArgs e){
			string strAllowSheets=POut.Bool(checkEClipboardAllowSheets.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strAllowSheets);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPresentAvailableFormsOnCheckIn,strAllowSheets);
		}

		private void CheckEClipboardCreateMissingForms_Click(object sender, EventArgs e){
			SetUIEClipboardEnabled();
			string strCanCreateMissingForms=POut.Bool(checkEClipboardCreateMissingForms.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strCanCreateMissingForms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardCreateMissingFormsOnCheckIn,strCanCreateMissingForms);
		}

		private void CheckEClipboardPopupKiosk_Click(object sender, EventArgs e){
			string strHasPopupKiosk=POut.Bool(checkEClipboardPopupKiosk.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,strHasPopupKiosk);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPopupKioskOnCheckIn,strHasPopupKiosk);
		}
		private void checkEnableByodSms_Click(object sender,EventArgs e) {
			SetUIEClipboardEnabled();
			string strEnableByodSms=POut.Bool(checkEnableByodSms.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.SelectedClinicNum,strEnableByodSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardEnableByodSms,strEnableByodSms);
		}

		private void checkAppendByodToArrivalResponseSms_Click(object sender,EventArgs e) {
			string strByodForResponseSms=POut.Bool(checkAppendByodToArrivalResponseSms.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.SelectedClinicNum,strByodForResponseSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAppendByodToArrivalResponseSms,strByodForResponseSms);
		}

		private void checkRequire2FA_Click(object sender, EventArgs e) {
			string strRequire2FA=POut.Bool(checkRequire2FA.Checked);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.SelectedClinicNum,strRequire2FA);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardDoTwoFactorAuth,strRequire2FA);
		}

		private void TextByodSmsTemplate_TextChanged(object sender, EventArgs e){
			if(!textByodSmsTemplate.Text.Contains(ByodTagReplacer.BYOD_TAG)) {
				return;//Do not track these changes while the byod tag is not present.
			}
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.SelectedClinicNum,textByodSmsTemplate.Text);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardByodSmsTemplate,textByodSmsTemplate.Text);
		}
		
		private void textByodSmsTemplate_Leave(object sender,EventArgs e) {
			if(!textByodSmsTemplate.Text.Contains(ByodTagReplacer.BYOD_TAG)) {
				MsgBox.Show(this,$"eClipboard link template must contain {ByodTagReplacer.BYOD_TAG}");
				textByodSmsTemplate.Select();
			}
		}

		private void TextEClipboardMessage_TextChanged(object sender, EventArgs e){
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.SelectedClinicNum,textEClipboardMessage.Text);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardMessageComplete,textEClipboardMessage.Text);
		}
		#endregion Methods - Event Handlers Prefs Section

		#region Methods - Private
		private void EClipboardPushPrefs() {
			//First get all of the clinic nums that have preference changes
			List<long> listClinicNumsWithChanges=_clinicPrefHelperEClipboard.GetClinicsWithChanges();
			if(listClinicNumsWithChanges.Contains(0)) {
				//Default clinic has changed so find any clinics that used defaults. They have changed by association.
				listClinicNumsWithChanges.AddRange(
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
			if(_clinicPrefHelperEClipboard.SyncAllPrefs()) {
				_doSetInvalidClinicPrefs=true;
			}
			//Push all of the new preference values to the clinic nums that have preference changes
			List<long> listClinicNumsDistinct=listClinicNumsWithChanges.Distinct().ToList();
			for(int i=0;i<listClinicNumsDistinct.Count;i++) {
				OpenDentBusiness.WebTypes.PushNotificationUtils.CI_NewEClipboardPrefs(listClinicNumsDistinct[i]);
			}
		}

		///<summary>This method expects a string that will need to be parsed into a human readable list of eClipboardImageCapture prefs.</summary>
		private string EClipboardGetImageDefsFromPref(string eClipboardImagePrefValueString) {
			if(string.IsNullOrWhiteSpace(eClipboardImagePrefValueString)) {
				return "";
			}
			List<string> listPrefNums=eClipboardImagePrefValueString.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).ToList();
			List<Def> listDefs=Defs.GetDefs(DefCat.EClipboardImageCapture,listPrefNums.Select(x=>PIn.Long(x)).ToList());
			return string.Join(",",listDefs.Select(x=>x.ItemName));
		}

		///<summary>Converts information in textEclipboardImageDefs.Text to string comma deliminated DefNums</summary>
		private string EClipboardImagePrefsFromText() {
			if(string.IsNullOrWhiteSpace(textEclipboardImageDefs.Text)) {
				return "";
			}
			List<string> listImagePrefNames=textEclipboardImageDefs.Text.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).ToList();
			List<long> listDefNums=new List<long>();
			for(int i = 0;i<listImagePrefNames.Count();i++) {
				listDefNums.Add(Defs.GetDefByExactName(DefCat.EClipboardImageCapture,listImagePrefNames[i]).DefNum);
			}
			return string.Join(",",listDefNums);
		}

		///<summary>Doesn't touch the Defaults checkbox itself.  Sets the 5 checkboxes and the textbox that are involved in prefs.  Importantly, it also sends those same default values to the list of clinicprefs in memory where they will later be synched.</summary>
		private void EClipboardSetControlsToPrefDefaults(){
			//EClipboardUseDefaults not included here
			checkEClipboardAllowCheckIn.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardAllowSelfCheckIn);
			checkEClipboardAllowSelfPortrait.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardAllowSelfPortraitOnCheckIn);
			checkEClipboardAllowSheets.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardPresentAvailableFormsOnCheckIn);
			checkEClipboardCreateMissingForms.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardCreateMissingFormsOnCheckIn);
			checkEClipboardPopupKiosk.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardPopupKioskOnCheckIn);
			checkEnableByodSms.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardEnableByodSms);
			checkAppendByodToArrivalResponseSms.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardAppendByodToArrivalResponseSms);
			checkRequire2FA.Checked=_clinicPrefHelperEClipboard.GetDefaultBoolVal(PrefName.EClipboardDoTwoFactorAuth);
			textByodSmsTemplate.Text=_clinicPrefHelperEClipboard.GetDefaultStringVal(PrefName.EClipboardByodSmsTemplate);
			textEClipboardMessage.Text=_clinicPrefHelperEClipboard.GetDefaultStringVal(PrefName.EClipboardMessageComplete);
			textEclipboardImageDefs.Text=EClipboardGetImageDefsFromPref(_clinicPrefHelperEClipboard.GetDefaultStringVal(PrefName.EClipboardImageCaptureDefs));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowCheckIn.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowSelfPortrait.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardAllowSheets.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardCreateMissingForms.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEClipboardPopupKiosk.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkEnableByodSms.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkAppendByodToArrivalResponseSms.Checked && checkEnableByodSms.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.SelectedClinicNum,
				textByodSmsTemplate.Text);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.SelectedClinicNum,
				textEClipboardMessage.Text);
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.SelectedClinicNum,
				POut.Bool(checkRequire2FA.Checked));
			_clinicPrefHelperEClipboard.ValChangedByUser(PrefName.EClipboardImageCaptureDefs,clinicPickerEClipboard.SelectedClinicNum,
				EClipboardImagePrefsFromText());
		}

		///<summary>Sets the Defaults checkbox itself.  Then sets the 5 other checkboxes and the textbox that are involved in prefs.  Sets them based on the values in the local clinicpref list.  Does not change any of those values.  Called only on startup.</summary>
		private void EClipboardSetControlsForClinicPrefs(){
			long clinicNumSelected=clinicPickerEClipboard.SelectedClinicNum;
			checkEClipboardUseDefaults.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardUseDefaults,clinicNumSelected);
			if(checkEClipboardUseDefaults.Checked) {
				EClipboardSetControlsToPrefDefaults();
			}
			else {
				checkEClipboardAllowCheckIn.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardAllowSelfCheckIn,clinicNumSelected);
				checkEClipboardAllowSelfPortrait.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicNumSelected);
				checkEClipboardAllowSheets.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicNumSelected);
				checkEClipboardCreateMissingForms.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicNumSelected);
				checkEClipboardPopupKiosk.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardPopupKioskOnCheckIn,clinicNumSelected);
				checkEnableByodSms.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardEnableByodSms,clinicNumSelected);
				checkAppendByodToArrivalResponseSms.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicNumSelected)
					&& checkEnableByodSms.Checked;
				checkRequire2FA.Checked=_clinicPrefHelperEClipboard.GetBoolVal(PrefName.EClipboardDoTwoFactorAuth,clinicNumSelected);
				textByodSmsTemplate.Text=_clinicPrefHelperEClipboard.GetStringVal(PrefName.EClipboardByodSmsTemplate,clinicNumSelected);
				textEClipboardMessage.Text=_clinicPrefHelperEClipboard.GetStringVal(PrefName.EClipboardMessageComplete,clinicNumSelected);
				textEclipboardImageDefs.Text=EClipboardGetImageDefsFromPref(_clinicPrefHelperEClipboard.GetStringVal(PrefName.EClipboardImageCaptureDefs,clinicNumSelected));
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
			gridEClipboardSheetsInUse.ListGridColumns.Clear();
			gridEClipboardSheetsInUse.BeginUpdate();
			GridColumn gridColumn=new GridColumn("Sheet Name",180);
			gridEClipboardSheetsInUse.ListGridColumns.Add(gridColumn);
			gridColumn=new GridColumn("Behavior",180);
			gridEClipboardSheetsInUse.ListGridColumns.Add(gridColumn);
			gridColumn=new GridColumn("Min Age",40,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.ListGridColumns.Add(gridColumn);
			gridColumn=new GridColumn("Max Age",40,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.ListGridColumns.Add(gridColumn);
			string addOn="";
			List<EClipboardSheetDef> listEClipboardSheetDefsOrdered=listEClipboardSheetDefs.OrderBy(x => x.ItemOrder).ToList();
			string gridCellValue;
			for(int i=0;i<listEClipboardSheetDefsOrdered.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(SheetDefs.GetDescription(listEClipboardSheetDefsOrdered[i].SheetDefNum));
				GridCell gridCell=new GridCell(Lan.g("enumPrefillCondition",listEClipboardSheetDefsOrdered[i].PrefillStatus.ToString()));
				gridRow.Cells.Add(gridCell);
				gridCellValue="Not Set";
				if(listEClipboardSheetDefsOrdered[i].MinAge>0) {
					gridCellValue=listEClipboardSheetDefsOrdered[i].MinAge.ToString();
				}
				gridRow.Cells.Add(gridCellValue);
				gridCellValue="Not Set";
				if(listEClipboardSheetDefsOrdered[i].MaxAge>0) {
					gridCellValue=listEClipboardSheetDefsOrdered[i].MaxAge.ToString();
				}
				gridRow.Cells.Add(gridCellValue);
				gridRow.Cells.Add(listEClipboardSheetDefsOrdered[i].ResubmitInterval.TotalDays.ToString()+addOn);
				gridRow.Tag=listEClipboardSheetDefsOrdered[i];
				gridEClipboardSheetsInUse.ListGridRows.Add(gridRow);
			}
			gridColumn=new GridColumn("Frequency (Days)",100,HorizontalAlignment.Center);
			gridEClipboardSheetsInUse.ListGridColumns.Add(gridColumn);
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
			gridMobileAppDevices.ListGridColumns.Clear();
			GridColumn gridColumn=new GridColumn("Device Name",100){IsWidthDynamic=true };
			gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			gridColumn=new GridColumn("Last Attempt",100){IsWidthDynamic=true };
			gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			gridColumn=new GridColumn("Last Login",100){IsWidthDynamic=true };
			gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			if(PrefC.HasClinicsEnabled) {
				gridColumn=new GridColumn("Clinic",100){IsWidthDynamic=true };
				gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			}
			gridColumn=new GridColumn("Enabled",50,HorizontalAlignment.Center);
			gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			if(_eClipboardAllowEdit) {
				gridColumn=new GridColumn("Delete",45,HorizontalAlignment.Center);
				gridMobileAppDevices.ListGridColumns.Add(gridColumn);
			}
			//Rows
			gridMobileAppDevices.ListGridRows.Clear();
			List<MobileAppDevice> listMobileAppDevicesToShow=MobileAppDevices.GetForUser(Security.CurUser);
			if(GetClinicNumEClipboardTab()>0) {
				listMobileAppDevicesToShow.RemoveAll(x => x.ClinicNum!=GetClinicNumEClipboardTab());
			}
			for(int i=0;i<listMobileAppDevicesToShow.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listMobileAppDevicesToShow[i].DeviceName+"\r\n("+listMobileAppDevicesToShow[i].UniqueID+")");
				string rowValue="";
				if(listMobileAppDevicesToShow[i].LastAttempt.Year>1880) {
					rowValue=listMobileAppDevicesToShow[i].LastAttempt.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(listMobileAppDevicesToShow[i].LastLogin.Year>1880) {
					rowValue=listMobileAppDevicesToShow[i].LastLogin.ToString();
				}
				row.Cells.Add(rowValue);
				if(PrefC.HasClinicsEnabled) {
					if(listMobileAppDevicesToShow[i].ClinicNum==0) {
						rowValue=Clinics.GetPracticeAsClinicZero().Abbr;
					}
					else {
						rowValue=Clinics.GetClinic(listMobileAppDevicesToShow[i].ClinicNum).Abbr;
					}
					row.Cells.Add(rowValue);
				}
				row.Cells.Add((listMobileAppDevicesToShow[i].IsAllowed ? "X" : ""));
				if(_eClipboardAllowEdit) {
					#region Delete click handler
					void DeleteClick(object sender,EventArgs e) {
						//int i in the for loop doesn't actually get disposed once the loop ends, because it was being used here to access 
						//listMobileAppDevicesToShow. i was always i+1, no matter which row you clicked, so use selected index instead.
						if(listMobileAppDevicesToShow[gridMobileAppDevices.SelectedIndices[0]].PatNum>0) {
							MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
								" or wait until the patient is no longer using the device.");
							return;
						}
						if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will immediately remove the device from the database and all other workstations." +
							" Continue?")) {
							return;
						}
						MobileAppDevices.Delete(listMobileAppDevicesToShow[gridMobileAppDevices.SelectedIndices[0]].MobileAppDeviceNum);
						FillGridMobileAppDevices();
					}
					#endregion Delete click handler
					GridCell cell=new GridCell("Delete");
					cell.ColorBackG=Color.LightGray;
					cell.ClickEvent=DeleteClick;
					row.Cells.Add(cell);
				}
				row.Tag=listMobileAppDevicesToShow[i];
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
			checkEClipboardUseDefaults.Enabled=GetClinicNumEClipboardTab()!=0 && isClinicSignedUp && _eClipboardAllowEdit;
			checkAppendByodToArrivalResponseSms.Enabled=checkEnableByodSms.Checked;
			textByodSmsTemplate.Enabled=checkEnableByodSms.Checked;
			groupEClipboardRules.Enabled=isClinicSignedUp && _eClipboardAllowEdit && notUsingDefaults;
			groupEClipboardSheets.Enabled=isClinicSignedUp && _eClipboardAllowEdit && notUsingDefaults && enableSheets;
			gridMobileAppDevices.Enabled=isClinicSignedUp;
			labelEClipboardNotSignedUp.Visible=!isClinicSignedUp;
		}

		private void SwapSheets(bool moveUp) {
			if(gridEClipboardSheetsInUse.SelectedIndices.Count()==0) {
				return;
			}
			int selectedIdx=gridEClipboardSheetsInUse.GetSelectedIndex();
			if(moveUp && selectedIdx==0) {
				return;
			}
			if(!moveUp && selectedIdx==gridEClipboardSheetsInUse.ListGridRows.Count-1) {
				return;
			}
			int swapIdx=selectedIdx+(moveUp?-1:1);
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
				List<ClinicPref> listClinicPrefs=_clinicPrefHelperEClipboard.GetWhere(PrefName.EClipboardUseDefaults,"1").ToList();
				for(int i=0;i<listClinicPrefs.Count;i++) {
					_clinicPrefHelperEClipboard.ValChangedByUser(prefName,listClinicPrefs[i].ClinicNum,newVal);
				}
			}
		}
		#endregion Methods - Private
	}
}