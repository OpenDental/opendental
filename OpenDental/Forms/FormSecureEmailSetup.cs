using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using static OpenDentBusiness.WebServiceMainHQProxy.EServiceSetup;

namespace OpenDental {
	public partial class FormSecureEmailSetup:FormODBase {
		private SignupOut _signupOut;
		private List<EmailPlatform> _listEmailPlatforms=new List<EmailPlatform>();

		public FormSecureEmailSetup(SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
			_listEmailPlatforms.Add(EmailPlatform.Unsecure);
			_listEmailPlatforms.Add(EmailPlatform.Secure);
		}

		private void FormMassEmailSetup_Load(object sender,EventArgs e) {
			if(_signupOut is null) {
				_signupOut=DownloadSignups(_signupOut);
				if(_signupOut is null) {
					Close();
					return;
				}
			}
			string secureEmailUrl=_signupOut?.EServices.FirstOrDefault(x => x.EService==eServiceCode.EmailSecureAccess)?.HostedUrl;
			if(string.IsNullOrWhiteSpace(secureEmailUrl)) {
				webBrowser.Visible=false;
			}
			else {
				webBrowser.Navigate(secureEmailUrl);
			}
			InitDefaultClinic();
			RefreshView();
		}

		private SignupOut DownloadSignups(SignupOut signupOld) {
			SignupOut signupOut=null;
			try {
				signupOut=FormEServicesSetup.GetSignupOut(signupOld);
			}
			catch(WebException we) {
				FriendlyException.Show(Lan.g(this,"Could not reach HQ.  Please make sure you have an internet connection and try again or call support."),we);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem loading settings.  Please try again or call support."),ex);
			}
			return signupOut;
		}

		private void FillClinics() {
			if(PrefC.HasClinicsEnabled) {
				FillClinicsGrid();
			}
			else {
				FillNoClinics();
			}
		}

		private void FillNoClinics() {
			bool isSignedUp=Clinics.IsSecureEmailSignedUp(0);
			bool isEnabled=Clinics.IsSecureEmailEnabled(0);
			EmailPlatform emailPlatformDefault=PIn.Enum<EmailPlatform>(ClinicPrefs.GetPrefValue(PrefName.EmailDefaultSendPlatform,0),true);
			EmailPlatform emailStatementsDefault=PIn.Enum<EmailPlatform>(ClinicPrefs.GetPrefValue(PrefName.EmailStatementsSecure,0),true);
			checkEnabled.Checked=isSignedUp && isEnabled;
			comboPlatform.Items.Clear();
			comboStatements.Items.Clear();
			if(isSignedUp) {
				comboPlatform.Items.AddListEnum(_listEmailPlatforms);
				comboPlatform.SelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailPlatformDefault);
				comboStatements.Items.AddListEnum(_listEmailPlatforms);
				comboStatements.SelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailStatementsDefault);
			}
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			HostedEmailStatus status=PrefC.GetEnum<HostedEmailStatus>(PrefName.EmailSecureStatus);
			status=EnumTools.ToggleFlag(status,HostedEmailStatus.Enabled);
			Prefs.UpdateInt(PrefName.EmailSecureStatus,(int)status);
			DataValid.SetInvalid(InvalidType.Prefs);
			RefreshView();
		}

		private void comboPlatform_SelectionChangeCommitted(object sender,EventArgs e) {
			EmailPlatform emailPlatform=(EmailPlatform)comboPlatform.SelectedItem;
			Prefs.UpdateString(PrefName.EmailDefaultSendPlatform,emailPlatform.ToString());
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void comboStatements_SelectionChangeCommitted(object sender,EventArgs e) {
			EmailPlatform emailPlatformStatements=(EmailPlatform)comboStatements.SelectedItem;
			Prefs.UpdateString(PrefName.EmailStatementsSecure,emailPlatformStatements.ToString());
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void FillClinicsGrid() {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);//Intentionally does not include clinic0
			gridClinics.BeginUpdate();
			gridClinics.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Clinic"),150);
			gridClinics.Columns.Add(col);
			col=new GridColumn("Secure Email Enabled",65,HorizontalAlignment.Center);
			col.Tag=new OnGridCellClick((clinicNum) => ToggleClinicEnabled(clinicNum));
			gridClinics.Columns.Add(col);
			col=new GridColumn("Default Clinic",65,HorizontalAlignment.Center);
			col.Tag=new OnGridCellClick((clinicNum) => SetDefaultSecureEmailClinic(clinicNum));
			gridClinics.Columns.Add(col);
			col=new GridColumn("Default Send",65,HorizontalAlignment.Center);
			col.ListDisplayStrings=_listEmailPlatforms.Select(x => x.GetDescription(true)).ToList();
			col.Tag=_listEmailPlatforms;
			gridClinics.Columns.Add(col);
			col=new GridColumn("Statement Send",65,HorizontalAlignment.Center);
			col.ListDisplayStrings=_listEmailPlatforms.Select(x => x.GetDescription(true)).ToList();
			col.Tag=_listEmailPlatforms;
			gridClinics.Columns.Add(col);
			col=new GridColumn("",65);
			gridClinics.Columns.Add(col);
			gridClinics.ListGridRows.Clear();
			long clinicNumDefault=PrefC.GetLong(PrefName.EmailSecureDefaultClinic);
			for(int i=0;i<listClinics.Count();i++) {
				GridRow row=new GridRow();
				Clinic clinic=listClinics[i];
				bool hasCredentials=Clinics.HasEmailHostingCredentials(clinic.ClinicNum);
				bool isSecureEmailActivated=Clinics.IsSecureEmailSignedUp(clinic.ClinicNum);
				bool isSecureEmailEnabled=Clinics.IsSecureEmailEnabled(clinic.ClinicNum);
				EmailPlatform emailPlatformDefault=PIn.Enum<EmailPlatform>(ClinicPrefs.GetPrefValue(PrefName.EmailDefaultSendPlatform,clinic.ClinicNum),true);
				EmailPlatform emailPlatformStatements=PIn.Enum<EmailPlatform>(ClinicPrefs.GetPrefValue(PrefName.EmailStatementsSecure,clinic.ClinicNum),true); //throws error invalid pref name
				row.Cells.Add(Clinics.GetAbbr(clinic.ClinicNum,listClinics));
				AddSignupCell(row,hasCredentials,isSecureEmailActivated,isSecureEmailEnabled,new EventHandler((o,e) => SignupSecureEmail(clinic.ClinicNum)));
				row.Cells.Add(new GridCell(clinicNumDefault==clinic.ClinicNum ? "X" : ""));
				row.Cells.Add(new GridCell($"{emailPlatformDefault.GetDescription(true)}") {
					ComboSelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailPlatformDefault),
				});
				row.Cells.Add(new GridCell($"{emailPlatformStatements.GetDescription(true)}") {
					ComboSelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailPlatformStatements),
				});
				row.Cells.Add("");
				row.Tag=clinic;
				gridClinics.ListGridRows.Add(row);
			}
			gridClinics.EndUpdate();
		}

		private void AddSignupCell(GridRow row,bool hasCredentials,bool isActivated,bool isEnabled,EventHandler onClickSignup) {
			GridCell gridCell;
			if(hasCredentials && isActivated) {
				//Hosted Email credentials already created, and eServiceSignup already created and active.  Clinic can enable and disabled feature locally.
				if(isEnabled) {
					gridCell=new GridCell("X");//Enabled.
				}
				else {
					gridCell=new GridCell("");//Disabled.
				}
			}
			else {
				//No Hosted Email credentials or eSerivceSignup.  Clinic must generate credentials and sign up.
				gridCell=new GridCell(Lan.g(this,"Sign Up")) {
					ClickEvent=onClickSignup,
				};
			}
			row.Cells.Add(gridCell);
		}

		private void RefreshView() {
			FillClinics();
			UpdateControlVisibility();
		}

		private void UpdateControlVisibility() {
			bool hasClinicsEnabled=PrefC.HasClinicsEnabled;
			bool allowEdit=Security.IsAuthorized(EnumPermType.EServicesSetup,true);
			bool isSignedUpClinic0=Clinics.IsSecureEmailSignedUp(0);
			bool areAnyClinicsSignedUp=GetClinicNums().Any(x => Clinics.IsSecureEmailSignedUp(x));
			gridClinics.Visible=hasClinicsEnabled;
			gridClinics.Enabled=allowEdit;
			checkEnabled.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			labelPlatform.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			comboPlatform.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			labelStatements.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			comboStatements.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			butSignup.Visible=!hasClinicsEnabled && !isSignedUpClinic0;
			butSignup.Enabled=allowEdit;
			checkEnabled.Enabled=allowEdit && isSignedUpClinic0;
			comboPlatform.Enabled=allowEdit && isSignedUpClinic0;
			comboStatements.Enabled=allowEdit && isSignedUpClinic0;
			groupSetup.Enabled=allowEdit && areAnyClinicsSignedUp;
		}

		private List<long> GetClinicNums() {
			if(PrefC.HasClinicsEnabled) {
				return gridClinics.GetTags<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			else {
				return new List<long> { 0 };
			}
		}

		private void gridClinics_CellClick(object sender,ODGridClickEventArgs e) {
			Clinic clinic=gridClinics.SelectedTag<Clinic>();
			GridColumn col=gridClinics.Columns[e.Col];
			if(clinic!=null && col.Tag is OnGridCellClick onClick) {
				onClick(clinic.ClinicNum);
			}
		}

		private void gridClinics_CellSelectionCommitted(object sender,ODGridClickEventArgs e) {
			if(!(gridClinics.Columns[e.Col].Tag is List<EmailPlatform> listEmailPlaforms)) {
				return;
			}
			int index=gridClinics.ListGridRows[e.Row].Cells[e.Col].ComboSelectedIndex;
			if(!index.Between(0,listEmailPlaforms.Count-1)) {
				return;
			}
			EmailPlatform emailPlatform=listEmailPlaforms[index];
			long clinicNum=0;
			if (gridClinics.SelectedTag<Clinic>()!=null) {
				clinicNum = gridClinics.SelectedTag<Clinic>().ClinicNum;
			}
			if(clinicNum==0) {
				Prefs.UpdateString(PrefName.EmailDefaultSendPlatform,emailPlatform.ToString());
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else if(gridClinics.Columns[e.Col].Heading=="Statement Send"){
				ClinicPrefs.Upsert(PrefName.EmailStatementsSecure,clinicNum,emailPlatform.ToString());
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailDefaultSendPlatform,clinicNum,emailPlatform.ToString());
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void ToggleClinicEnabled(long clinicNum) {
			HostedEmailStatus status=(HostedEmailStatus)ClinicPrefs.GetInt(PrefName.EmailSecureStatus,clinicNum);
			status=EnumTools.ToggleFlag(status,HostedEmailStatus.Enabled);
			ClinicPrefs.Upsert(PrefName.EmailSecureStatus,clinicNum,$"{(int)status}");
			DataValid.SetInvalid(InvalidType.ClinicPrefs);
			RefreshView();
		}

		private void InitDefaultClinic(long clinicNum=-1) {
			if(PrefC.GetLong(PrefName.EmailSecureDefaultClinic)!=0) {
				return;
			}
			if(clinicNum==-1) {
				clinicNum=0;
				if (Clinics.GetFirstOrDefault(x => Clinics.IsSecureEmailSignedUp(x.ClinicNum))!=null) {
					clinicNum = Clinics.GetFirstOrDefault(x => Clinics.IsSecureEmailSignedUp(x.ClinicNum)).ClinicNum;
				}
			}
			//First clinic signed up for Secure.  Set as default clinic.
			SetDefaultSecureEmailClinic(clinicNum);
		}

		private void SetDefaultSecureEmailClinic(long clinicNum) {
			if(!Clinics.IsSecureEmailSignedUp(clinicNum)) {
				return;
			}
			Prefs.UpdateLong(PrefName.EmailSecureDefaultClinic,clinicNum);
			DataValid.SetInvalid(InvalidType.Prefs);
			RefreshView();
		}

		private void butSignup_Click(object sender,EventArgs e) {
			SignupSecureEmail(0);
		}

		private void SignupSecureEmail(long clinicNumClicked) {
			// Get secure email signup status for all clinics before we get started (for later comparison).
			List<ClinicPref> listClinicPrefsOld=ClinicPrefs.GetPrefAllClinics(PrefName.EmailSecureStatus,true);
			if(WebServiceMainHQProxy.GetWebServiceMainHQInstance() is WebServiceMainHQMockDemo webService) {
				//If we are not connecting to a real webservice, save this signup in memory.
				webService.SetEServiceCodeEnabled(eServiceCode.EmailSecureAccess,clinicNumClicked,true);
			}
			else {
				//Signup may or may not be added in this window.
				using FormEServicesSignup formSignup=new FormEServicesSignup(_signupOut);
				formSignup.ShowDialog();
			}
			_signupOut=DownloadSignups(_signupOut); //Syncs signup prefs with HQ. Creates credentials if necessary.
			if(_signupOut==null) {
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				// Clinics are enabled => process everything except clinicNum zero.
				SignUpAllClinics(listClinicPrefsOld);
			}
			else {
				// Clinics not enabled => only process clinicNum zero.
				SignUpClinicZero(listClinicPrefsOld);
			}
		}

		private void SignUpClinicZero(List<ClinicPref> listClinicPrefsOld) {
			SignupOut.SignupOutEService signUpOutStatus=_signupOut?.EServices?.Find(x=>x.ClinicNum==0 && x.EServiceCodeInt==(int)eServiceCode.EmailSecureAccess);
			if(signUpOutStatus==null) {
				return;
			}
			string oldPref=(listClinicPrefsOld.Find(x => x.ClinicNum==0)?.ValueString)?.Trim()??"";
			bool oldStatusWasDisabled = oldPref=="0" || oldPref=="";
			if (signUpOutStatus.IsEnabled && oldStatusWasDisabled) {
				HostedEmailStatus status=PrefC.GetEnum<HostedEmailStatus>(PrefName.EmailSecureStatus);
				status=EnumTools.AddFlag(status,HostedEmailStatus.Enabled);
				Prefs.UpdateInt(PrefName.EmailSecureStatus,(int)status);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			RefreshView();
		}

		private void SignUpAllClinics(List<ClinicPref> listClinicPrefsOld) {
			List<SignupOut.SignupOutEService> listSignups=_signupOut?.EServices?.FindAll(x=>x.EServiceCodeInt==(int)eServiceCode.EmailSecureAccess);
			if((listSignups?.Count??0)==0) {
				return;
			}
			for(int i=0;i<listSignups.Count;i++) {
				if(listSignups[i].ClinicNum==0) {
					continue; // If ClinicNum is 0 skip;
				}
				string oldPref=(listClinicPrefsOld.Find(x => x.ClinicNum==listSignups[i].ClinicNum)?.ValueString)?.Trim()??"";
				bool oldStatusWasDisabled = oldPref=="0" || oldPref=="";
				if (listSignups[i].IsEnabled && oldStatusWasDisabled) {
					HostedEmailStatus status=(HostedEmailStatus)ClinicPrefs.GetInt(PrefName.EmailSecureStatus,listSignups[i].ClinicNum);
					status=EnumTools.AddFlag(status,HostedEmailStatus.Enabled);
					ClinicPrefs.Upsert(PrefName.EmailSecureStatus,listSignups[i].ClinicNum,$"{(int)status}");
					InitDefaultClinic(listSignups[i].ClinicNum);
				}
			}
			DataValid.SetInvalid(InvalidType.ClinicPrefs);
			RefreshView();
		}

		private void butAddresses_Click(object sender,EventArgs e) {
			using FormEmailHostingAddressVerification formVerification=new FormEmailHostingAddressVerification();
			formVerification.ShowDialog();
		}

		private void butSignatures_Click(object sender,EventArgs e) {
			using FormEmailHostingSignatures formSignatures=new FormEmailHostingSignatures();
			formSignatures.ShowDialog();
		}

		private delegate void OnGridCellClick(long clinicNum);
	}
}