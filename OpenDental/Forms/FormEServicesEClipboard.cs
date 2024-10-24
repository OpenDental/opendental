using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
			PrefName.EClipboardPresentAvailableFormsOnCheckIn,
			PrefName.EClipboardCreateMissingFormsOnCheckIn,
			PrefName.EClipboardPopupKioskOnCheckIn,
			PrefName.EClipboardEnableByodSms,
			PrefName.EClipboardAppendByodToArrivalResponseSms,
			PrefName.EClipboardByodSmsTemplate,
			PrefName.EClipboardMessageComplete,
			PrefName.EClipboardDoTwoFactorAuth,
			PrefName.EClipboardImageCaptureDefs,
			PrefName.EClipboardHasMultiPageCheckIn);
		private bool _doSetInvalidClinicPrefs=false;
		private bool _canEditEClipboard;
		///<summary>A list of all eclipboard sheet defs that are edited in this window. Synced with the database list on the ok click.</summary>
		private List<EClipboardSheetDef> _listEClipboardSheetDefs;
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
			return clinicPickerEClipboard.ClinicNumSelected;
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
			if(!ODBuild.IsDebug() && !PrefC.IsODHQ){//if release and not HQ
				butEFormAdd.Visible=false;
			}
			//Disable all controls if user does not have EServicesSetup permission.
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true)) {
				DisableAllExcept();
			}
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
			ODEvent.Fired+=eClipboardChangedEvent_Fired;
			_canEditEClipboard=Security.IsAuthorized(EnumPermType.EServicesSetup,true);
			FillGridImages();
			FillGridForms();
			SetUIEClipboardEnabled();
			textByodSmsTemplate.MouseHover+=new EventHandler((sender,e) => {
				string availableTags=Lans.g(this,"Available Tags:\n")+OpenDentBusiness.AutoComm.ByodTagReplacer.BYOD_TAG+Lans.g(this,"(required)\n")
					+string.Join(",",ApptReminderRules.GetAvailableTags(ApptReminderType.Undefined));
				ToolTip toolTip=new ToolTip();
				toolTip.SetToolTip(textByodSmsTemplate,availableTags);
			});
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			//Find clinics where pref changed.
			List<long> listClinicsChanged=_clinicPrefHelper.GetClinicsWithChanges(PrefName.EClipboardUseDefaults).FindAll(x=>x!=0);
			EClipboardPushPrefs();
			EClipboardSheetDefs.Sync(_listEClipboardSheetDefs,EClipboardSheetDefs.Refresh());
			EClipboardImageCaptureDefs.Sync(_listEClipboardImageCaptureDefs,EClipboardImageCaptureDefs.Refresh());
			MobileBrandingProfiles.SynchMobileBrandingProfileClinicDefaults(listClinicsChanged);
			ODEvent.Fired-=eClipboardChangedEvent_Fired;
			DialogResult=DialogResult.OK;
		}

		#endregion Methods - FormEServices Boilerplate

		#region Methods - Event Handlers Main
		private void butConfStatuses_Click(object sender,EventArgs e) {
			FormEServicesAutoMsgingAdvanced formEServicesAutoMsgingAdvanced=new FormEServicesAutoMsgingAdvanced();
			formEServicesAutoMsgingAdvanced.ShowDialog();
		}

		private void ButEClipboardUp_Click(object sender,EventArgs e) {
			SwapSheets(doMoveUp:true);
		}

		private void ButEClipboardDown_Click(object sender,EventArgs e) {
			SwapSheets(doMoveUp:false);
		}

		private void ClinicPickerEClipboard_SelectionChangeCommitted(object sender,EventArgs e) {
			EClipboardSetControlsForClinicPrefs();
			FillGridImages();
			FillGridForms();
			SetUIEClipboardEnabled();
		}

		public void eClipboardChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.eClipboard || this.IsDisposed) {
				return;
			}
		}

		private void gridImages_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EClipboardImageCaptureDef eClipboardImageCaptureDefSelected=gridImages.SelectedTag<EClipboardImageCaptureDef>();
			if(eClipboardImageCaptureDefSelected==null) {
				return;
			}
			using FormEClipboardImageCaptureDefEdit formEClipboardImageCaptureDefEdit=new FormEClipboardImageCaptureDefEdit();
			formEClipboardImageCaptureDefEdit.EClipboardImageCaptureDefCur=eClipboardImageCaptureDefSelected;
			formEClipboardImageCaptureDefEdit.ListEClipboardImageCaptureDefs=_listEClipboardImageCaptureDefs.Select(x => x.Copy()).ToList();
			formEClipboardImageCaptureDefEdit.ShowDialog();
			if(formEClipboardImageCaptureDefEdit.IsDeleted){
				_listEClipboardImageCaptureDefs.Remove(eClipboardImageCaptureDefSelected);
			}
			FillGridImages();
		}

		private void gridForms_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEClipboardSheetRule formEClipboardSheetRule=new FormEClipboardSheetRule();
			formEClipboardSheetRule.EClipboardSheetDefCur=(EClipboardSheetDef)gridForms.ListGridRows[e.Row].Tag;
			formEClipboardSheetRule.ListEClipboardSheetDefs=gridForms.ListGridRows.Select(x=>(EClipboardSheetDef)x.Tag).ToList();
			formEClipboardSheetRule.ShowDialog();
			if(formEClipboardSheetRule.IsDeleted) {
				EClipboardSheetDef eClipboardSheetDef=(EClipboardSheetDef)gridForms.ListGridRows[e.Row].Tag;
				_listEClipboardSheetDefs.Remove(eClipboardSheetDef);
			}
			FillGridForms();
		}

		private void butImageAdd_Click(object sender,EventArgs e){
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			using FormEClipboardImagePicker formEClipboardImagePicker=new FormEClipboardImagePicker();
			formEClipboardImagePicker.ClinicNum=clinicNum;
			formEClipboardImagePicker.ListEClipboardImageCaptureDefs=_listEClipboardImageCaptureDefs;//no longer need to send in copies, child form can't make edits to eClipboardImageCaptureDefs already in this list.
			formEClipboardImagePicker.ShowDialog();
			if(formEClipboardImagePicker.DialogResult!=DialogResult.OK) {
				return;
			}
			//On OK click, update _listEClipboardImageCaptureDefs to reflect any newly added eClipboardImageCaptureDefs
			for(int i=0;i<formEClipboardImagePicker.ListEClipboardImageCaptureDefsSelected.Count;i++){
				EClipboardImageCaptureDef eClipboardImageCaptureDef=formEClipboardImagePicker.ListEClipboardImageCaptureDefsSelected[i];
				_listEClipboardImageCaptureDefs.Add(eClipboardImageCaptureDef);
			}
			//Update the UI to reflect the new changes
			FillGridImages();
		}

		private void butSheetAdd_Click(object sender,EventArgs e) {
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			FrmSheetPicker frmSheetPicker=new FrmSheetPicker();
			List<SheetDef> listSheetDefs=new List<SheetDef>();
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.PatientForm));
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.MedicalHistory));
			listSheetDefs.AddRange(SheetDefs.GetCustomForType(SheetTypeEnum.Consent));
			//Clear any custom sheet defs that don't have a mobile layout
			listSheetDefs.RemoveAll(x => !x.HasMobileLayout);
			for(int i=0;i<gridForms.ListGridRows.Count;i++){
				//Remove any current sheets in use from the list of sheet defs available to add.
				EClipboardSheetDef eClipboardSheetDef=(EClipboardSheetDef)gridForms.ListGridRows[i].Tag;
				if(eClipboardSheetDef.SheetDefNum==0 || eClipboardSheetDef is null){//If SheetDefNum==0, this is an eForm.
					continue;
				}
				if(listSheetDefs.Select(x=>x.SheetDefNum).Contains(eClipboardSheetDef.SheetDefNum)){
					listSheetDefs.RemoveAll(x=>x.SheetDefNum==eClipboardSheetDef.SheetDefNum);
				}
			}
			frmSheetPicker.ListSheetDefs=listSheetDefs;
			frmSheetPicker.ShowDialog();
			if(frmSheetPicker.IsDialogOK){
				List<SheetDef> listSheetDefsSelected=frmSheetPicker.ListSheetDefsSelected;
				List<EClipboardSheetDef> listEClipboardSheetDefs=new List<EClipboardSheetDef>();
				for(int i=0;i<listSheetDefsSelected.Count;i++){
					EClipboardSheetDef eClipboardSheetDef=new EClipboardSheetDef();
					eClipboardSheetDef.SheetDefNum=listSheetDefsSelected[i].SheetDefNum;
					eClipboardSheetDef.ClinicNum=clinicNum;
					eClipboardSheetDef.ResubmitInterval=TimeSpan.FromDays(30);
					eClipboardSheetDef.MinAge=-1;
					eClipboardSheetDef.MaxAge=-1;
					eClipboardSheetDef.ItemOrder=gridForms.ListGridRows.Count;
					_listEClipboardSheetDefs.Add(eClipboardSheetDef);
				}
				FillGridForms();
				SetEClipboardSheetOrder();
			}
		}
		
		private void butEFormAdd_Click(object sender,EventArgs e) {
			InsertInternalEFormsIfNeeded();
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			FrmEFormPicker frmEFormPicker=new FrmEFormPicker();
			frmEFormPicker.ShowDialog();
			if(!frmEFormPicker.IsDialogOK){
				return;
			}
			EFormDef eFormDef=frmEFormPicker.EFormDefSelected;
			//attach the added eForm to a new eClipboardSheetDef
			EClipboardSheetDef eClipboardSheetDef=new EClipboardSheetDef();
			eClipboardSheetDef.EFormDefNum=eFormDef.EFormDefNum;
			eClipboardSheetDef.ClinicNum=clinicNum;
			eClipboardSheetDef.ResubmitInterval=TimeSpan.FromDays(30);
			eClipboardSheetDef.MinAge=-1;
			eClipboardSheetDef.MaxAge=-1;
			eClipboardSheetDef.ItemOrder=gridForms.ListGridRows.Count;
			_listEClipboardSheetDefs.Add(eClipboardSheetDef);
			FillGridForms();
			SetEClipboardSheetOrder();
		}

		private void butBrandingProfile_Click(object sender,EventArgs e) {
			using FormMobileBrandingProfileEdit formMobileBrandingProfileEdit= new FormMobileBrandingProfileEdit();
			formMobileBrandingProfileEdit.ClinicNum=clinicPickerEClipboard.ClinicNumSelected;
			formMobileBrandingProfileEdit.ShowDialog();			
		}
		#endregion Methods - Event Handlers Main

		#region Methods - Event Handlers Prefs Section
		private void CheckEClipboardUseDefaults_Click(object sender, EventArgs e){
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardUseDefaults,clinicPickerEClipboard.ClinicNumSelected,
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
			FillGridImages();
			FillGridForms();
			SetUIEClipboardEnabled();
		}

		private void CheckEClipboardAllowCheckIn_Click(object sender, EventArgs e){
			string strAllowCheckIn=POut.Bool(checkEClipboardAllowCheckIn.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.ClinicNumSelected,strAllowCheckIn);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowSelfCheckIn,strAllowCheckIn);
		}

		private void CheckAllowPaymentCheckIn_Click(object sender,EventArgs e) {
			string strAllowPayments=POut.Bool(checkEClipboardAllowPaymentCheckIn.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowPaymentOnCheckin,clinicPickerEClipboard.ClinicNumSelected,strAllowPayments);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAllowPaymentOnCheckin,strAllowPayments);
		}

		private void CheckEClipboardAllowSheets_Click(object sender, EventArgs e){
			string strAllowSheets=POut.Bool(checkEClipboardAllowSheets.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,strAllowSheets);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPresentAvailableFormsOnCheckIn,strAllowSheets);
		}

		private void CheckEClipboardCreateMissingForms_Click(object sender, EventArgs e){
			SetUIEClipboardEnabled();
			string strCanCreateMissingForms=POut.Bool(checkEClipboardCreateMissingForms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,strCanCreateMissingForms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardCreateMissingFormsOnCheckIn,strCanCreateMissingForms);
		}

		private void CheckEClipboardPopupKiosk_Click(object sender, EventArgs e){
			string strHasPopupKiosk=POut.Bool(checkEClipboardPopupKiosk.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,strHasPopupKiosk);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardPopupKioskOnCheckIn,strHasPopupKiosk);
		}
		private void checkEnableByodSms_Click(object sender,EventArgs e) {
			SetUIEClipboardEnabled();
			string strEnableByodSms=POut.Bool(checkEnableByodSms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.ClinicNumSelected,strEnableByodSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardEnableByodSms,strEnableByodSms);
		}

		private void checkAppendByodToArrivalResponseSms_Click(object sender,EventArgs e) {
			string strByodForResponseSms=POut.Bool(checkAppendByodToArrivalResponseSms.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.ClinicNumSelected,strByodForResponseSms);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardAppendByodToArrivalResponseSms,strByodForResponseSms);
		}

		private void checkRequire2FA_Click(object sender, EventArgs e) {
			string strRequire2FA=POut.Bool(checkRequire2FA.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.ClinicNumSelected,strRequire2FA);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardDoTwoFactorAuth,strRequire2FA);
		}

		private void checkDisplayIndividually_Click(object sender, EventArgs e) {
			string strChecked=POut.Bool(checkDisplayIndividually.Checked);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardHasMultiPageCheckIn,clinicPickerEClipboard.ClinicNumSelected,strChecked);
			UpdateEClipboardDefaultsIfNeeded(PrefName.EClipboardHasMultiPageCheckIn,strChecked);
		}

		private void TextByodSmsTemplate_TextChanged(object sender, EventArgs e){
			if(!textByodSmsTemplate.Text.Contains(ByodTagReplacer.BYOD_TAG)) {
				return;//Do not track these changes while the byod tag is not present.
			}
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.ClinicNumSelected,textByodSmsTemplate.Text);
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
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.ClinicNumSelected,textEClipboardMessage.Text);
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
			//Create a mobile notification to update all of the preference values on the mobile devices for the clinic nums that have new preference changes.
			List<long> listClinicNumsDistinct=listClinicNumsChanged.Distinct().ToList();
			for(int i=0;i<listClinicNumsDistinct.Count;i++) {
				MobileNotifications.CI_NewEClipboardPrefs(listClinicNumsDistinct[i]);
			}
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
			checkRequire2FA.Checked=_clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardDoTwoFactorAuth);
			textByodSmsTemplate.Text=_clinicPrefHelper.GetDefaultStringVal(PrefName.EClipboardByodSmsTemplate);
			textEClipboardMessage.Text=_clinicPrefHelper.GetDefaultStringVal(PrefName.EClipboardMessageComplete);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEClipboardAllowCheckIn.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowPaymentOnCheckin,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEClipboardAllowPaymentCheckIn.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEClipboardAllowSheets.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardCreateMissingFormsOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEClipboardCreateMissingForms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardPopupKioskOnCheckIn,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEClipboardPopupKiosk.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardEnableByodSms,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkEnableByodSms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkAppendByodToArrivalResponseSms.Checked && checkEnableByodSms.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardByodSmsTemplate,clinicPickerEClipboard.ClinicNumSelected,
				textByodSmsTemplate.Text);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardMessageComplete,clinicPickerEClipboard.ClinicNumSelected,
				textEClipboardMessage.Text);
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardDoTwoFactorAuth,clinicPickerEClipboard.ClinicNumSelected,
				POut.Bool(checkRequire2FA.Checked));
			_clinicPrefHelper.ValChangedByUser(PrefName.EClipboardHasMultiPageCheckIn,clinicPickerEClipboard.ClinicNumSelected,POut.Bool(checkDisplayIndividually.Checked));
		}

		///<summary>Sets the Defaults checkbox itself.  Then sets the 5 other checkboxes and the textbox that are involved in prefs.  Sets them based on the values in the local clinicpref list.  Does not change any of those values.  Called only on startup.</summary>
		private void EClipboardSetControlsForClinicPrefs(){
			long clinicNum=clinicPickerEClipboard.ClinicNumSelected;
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
				textByodSmsTemplate.Text=_clinicPrefHelper.GetStringVal(PrefName.EClipboardByodSmsTemplate,clinicNum);
				textEClipboardMessage.Text=_clinicPrefHelper.GetStringVal(PrefName.EClipboardMessageComplete,clinicNum);
			}
		}

		private void FillGridImages() {
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			//Separate the EClipboard Images defcat capturedefs from the self portrait capturedef and store each in their respective variables.
			List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefs=_listEClipboardImageCaptureDefs.FindAll(x => !x.IsSelfPortrait && x.ClinicNum==clinicNum);
			EClipboardImageCaptureDef eClipboardImageCaptureDefSelfPortrait=_listEClipboardImageCaptureDefs.Find(x => x.IsSelfPortrait && x.ClinicNum==clinicNum);
			gridImages.ListGridRows.Clear();
			gridImages.Columns.Clear();
			gridImages.BeginUpdate();
			GridColumn col=new GridColumn("Definition",120);
			gridImages.Columns.Add(col);
			col=new GridColumn("Item Value",180);
			gridImages.Columns.Add(col);
			if(eClipboardImageCaptureDefSelfPortrait!=null){//First add self portait if not null
				GridRow row=new GridRow();
				row.Cells.Add("Self Portrait");//Col: Definition
				row.Cells.Add("Allows patient to submit a self-portrait upon checkin");//Col: Item Value
				row.Cells.Add(eClipboardImageCaptureDefSelfPortrait.FrequencyDays.ToString());//Col: Frequency
				gridImages.ListGridRows.Add(row);
				row.Tag=eClipboardImageCaptureDefSelfPortrait;
			}
			for(int i=0;i<listEClipboardImageCaptureDefs.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(Defs.GetDef(DefCat.EClipboardImageCapture,listEClipboardImageCaptureDefs[i].DefNum).ItemName);//Col: Definition
				row.Cells.Add(Defs.GetDef(DefCat.EClipboardImageCapture,listEClipboardImageCaptureDefs[i].DefNum).ItemValue);//Col: Item Value
				row.Cells.Add(listEClipboardImageCaptureDefs[i].FrequencyDays.ToString());//Col: Frequency
				row.Tag=listEClipboardImageCaptureDefs[i];
				gridImages.ListGridRows.Add(row);
			}
			col=new GridColumn("Frequency (Days)",100,HorizontalAlignment.Center);
			gridImages.Columns.Add(col);
			gridImages.EndUpdate();
		}

		///<summary>Fills gridForms with the Sheets and eForms in use.</summary>
		private void FillGridForms() {
			//Get the list of in-memory eclipboard sheets
			long clinicNum=GetClinicNumEClipboardTab();
			if(checkEClipboardUseDefaults.Checked) {
				clinicNum=0;
			}
			List<EClipboardSheetDef> listEClipboardSheetDefs=_listEClipboardSheetDefs.FindAll(x => x.ClinicNum==clinicNum);
			//Put the sheets that are in use into the grid of sheets in use
			gridForms.ListGridRows.Clear();
			gridForms.Columns.Clear();
			gridForms.BeginUpdate();
			GridColumn col=new GridColumn("",50);//Can either be "Sheet" or "eForm". Does not correspond to 'SheetType' column in SheetDef or 'FormType' column in eFormDef. May need to rename this column to reduce ambiguity.
			gridForms.Columns.Add(col);
			col=new GridColumn("Form Name",180);
			gridForms.Columns.Add(col);
			col=new GridColumn("Behavior",180);
			gridForms.Columns.Add(col);
			col=new GridColumn("Min Age",40,HorizontalAlignment.Center);
			gridForms.Columns.Add(col);
			col=new GridColumn("Max Age",40,HorizontalAlignment.Center);
			gridForms.Columns.Add(col);
			string addOn="";
			List<EClipboardSheetDef> listEClipboardSheetDefsOrdered=listEClipboardSheetDefs.OrderBy(x => x.ItemOrder).ToList();
			string gridCellValue;
			for(int i=0;i<listEClipboardSheetDefsOrdered.Count;i++){
				GridRow row=new GridRow();
				if(listEClipboardSheetDefsOrdered[i].SheetDefNum!=0){
					row.Cells.Add("Sheet");
					row.Cells.Add(SheetDefs.GetDescription(listEClipboardSheetDefsOrdered[i].SheetDefNum));
				}
				else if(listEClipboardSheetDefsOrdered[i].EFormDefNum!=0){
					row.Cells.Add("eForm");
					row.Cells.Add(EFormDefs.GetFirstOrDefault(x=>x.EFormDefNum==listEClipboardSheetDefsOrdered[i].EFormDefNum)?.Description);
				}
				else{
					continue;
				}
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
				gridForms.ListGridRows.Add(row);
			}
			col=new GridColumn("Frequency (Days)",100,HorizontalAlignment.Center);
			gridForms.Columns.Add(col);
			gridForms.EndUpdate();
		}

		private void SetEClipboardSheetOrder() {
			if(gridForms.ListGridRows.Count<1) {
				return;
			}
			int idx=0;
			List<EClipboardSheetDef> listEClipboardSheetDefs=gridForms.ListGridRows.Select(x => (EClipboardSheetDef)x.Tag).ToList();
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
			if(ODBuild.IsDebug() && Environment.MachineName.ToLower()=="jordanhome"){
				isClinicSignedUp=true;
			}
			bool doUseDefaults=GetClinicNumEClipboardTab()!=0 && checkEClipboardUseDefaults.Checked;
			bool enableSheets=checkEClipboardCreateMissingForms.Checked;
			checkEClipboardUseDefaults.Enabled=GetClinicNumEClipboardTab()!=0 && isClinicSignedUp && _canEditEClipboard;
			checkAppendByodToArrivalResponseSms.Enabled=checkEnableByodSms.Checked;
			textByodSmsTemplate.Enabled=checkEnableByodSms.Checked;
			groupEClipboardRules.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults;
			gridForms.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults && enableSheets;
			gridImages.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults;
			butEFormAdd.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults && enableSheets;
			butSheetAdd.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults && enableSheets;
			butImageAdd.Enabled=isClinicSignedUp && _canEditEClipboard && !doUseDefaults;
			labelEClipboardNotSignedUp.Visible=!isClinicSignedUp;
			//Enabled when allowed and either use defaults is unchecked or we are at default clinic.
			if((isClinicSignedUp && _canEditEClipboard) && (!checkEClipboardUseDefaults.Checked || GetClinicNumEClipboardTab()==0)) {
				butBrandingProfile.Enabled=true;
			}
			else {
				butBrandingProfile.Enabled=false;
			}
		}

		private void SwapSheets(bool doMoveUp) {
			if(gridForms.SelectedIndices.Count()==0) {
				return;
			}
			int selectedIdx=gridForms.GetSelectedIndex();
			if(doMoveUp && selectedIdx==0) {
				return;
			}
			if(!doMoveUp && selectedIdx==gridForms.ListGridRows.Count-1) {
				return;
			}
			int swapIdx=selectedIdx;
			if(doMoveUp) {
				swapIdx-=1;
			}
			else {
				swapIdx+=1;
			}
			EClipboardSheetDef eClipboardSheetDefSelected=(EClipboardSheetDef)gridForms.ListGridRows[selectedIdx].Tag;
			EClipboardSheetDef eClipboardSheetDefSwap=(EClipboardSheetDef)gridForms.ListGridRows[swapIdx].Tag;
			int selectedItemOrder=eClipboardSheetDefSelected.ItemOrder;
			eClipboardSheetDefSelected.ItemOrder=eClipboardSheetDefSwap.ItemOrder;
			eClipboardSheetDefSwap.ItemOrder=selectedItemOrder;
			FillGridForms();
			gridForms.SetSelected(swapIdx,setValue:true);
		}

		private void UpdateEClipboardDefaultsIfNeeded(PrefName prefName,string newVal) {
			if(GetClinicNumEClipboardTab()==0) { //we are making changes to the default
				List<ClinicPref> listClinicPrefs=_clinicPrefHelper.GetWhere(PrefName.EClipboardUseDefaults,"1").ToList();
				for(int i=0;i<listClinicPrefs.Count;i++) {
					_clinicPrefHelper.ValChangedByUser(prefName,listClinicPrefs[i].ClinicNum,newVal);
				}
			}
		}

		///<summary>If there are no EFormDefs in the db, then this will copy all of the internal forms into the db. This method allows eForms to work right out of the box in the event a user decides to add an eForm to the eClipboard without first opening and saving the eForm, although this behavior is not expected to be common. Also used in FrmEFormDefs.</summary>
		private void InsertInternalEFormsIfNeeded(){
			bool didInsertInternal=EForms.InsertInternalToDb();
			if(!didInsertInternal){
				return;//custom eForms existed in the db already.
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			DataValid.SetInvalid(InvalidType.Sheets);
		}

		#endregion Methods - Private
	}
}