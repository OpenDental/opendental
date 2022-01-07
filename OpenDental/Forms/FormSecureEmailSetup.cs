using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSecureEmailSetup:FormODBase {
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signup;
		private List<EmailPlatform> _listEmailPlatforms=new List<EmailPlatform> { EmailPlatform.Unsecure, EmailPlatform.Secure };

		public FormSecureEmailSetup(WebServiceMainHQProxy.EServiceSetup.SignupOut signup=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signup=signup;
		}

		private void FormMassEmailSetup_Load(object sender,EventArgs e) {
			if(_signup is null) {
				_signup=DownloadSignups(_signup);
				if(_signup is null) {
					Close();
					return;
				}
			}
			string secureEmailUrl=_signup?.EServices.FirstOrDefault(x => x.EService==eServiceCode.EmailSecureAccess)?.HostedUrl;
			if(!string.IsNullOrWhiteSpace(secureEmailUrl)) {
				webBrowser.Navigate(secureEmailUrl);
			}
			else {
				webBrowser.Visible=false;
			}
			InitDefaultClinic();
			RefreshView();
		}

		private WebServiceMainHQProxy.EServiceSetup.SignupOut DownloadSignups(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOld) {
			WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null;
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
			checkEnabled.Checked=isSignedUp && isEnabled;
			comboPlatform.Items.Clear();
			if(isSignedUp) {
				comboPlatform.Items.AddListEnum(_listEmailPlatforms);
				comboPlatform.SelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailPlatformDefault);
			}
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			ToggleEnabled(0,PrefName.EmailSecureStatus);
		}

		private void comboPlatform_SelectionChangeCommitted(object sender,EventArgs e) {
			EmailPlatform emailPlatform=(EmailPlatform)comboPlatform.SelectedItem;
			Prefs.UpdateString(PrefName.EmailDefaultSendPlatform,emailPlatform.ToString());
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void FillClinicsGrid() {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);//Intentionally does not include clinic0
			gridClinics.BeginUpdate();
			gridClinics.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Clinic"),200);
			gridClinics.Columns.Add(col);
			col=new GridColumn("Secure Email Enabled",65,HorizontalAlignment.Center) {
				Tag=new OnGridCellClick((clinicNum) => ToggleEnabled(clinicNum,PrefName.EmailSecureStatus)),
			};
			gridClinics.Columns.Add(col);
			col=new GridColumn("Default Clinic",65,HorizontalAlignment.Center) {
				Tag=new OnGridCellClick((clinicNum) => SetDefaultSecureEmailClinic(clinicNum)),
			};
			gridClinics.Columns.Add(col);
			col=new GridColumn("Default Send",65,HorizontalAlignment.Center) {
				ListDisplayStrings=_listEmailPlatforms.Select(x => x.GetDescription(true)).ToList(),
				Tag=_listEmailPlatforms,
			};
			gridClinics.Columns.Add(col);
			col=new GridColumn("",65);
			gridClinics.Columns.Add(col);
			gridClinics.ListGridRows.Clear();
			long defaultClinicNum=PrefC.GetLong(PrefName.EmailSecureDefaultClinic);
			foreach(Clinic clinic in listClinics) {
				GridRow row=new GridRow();
				bool hasCredentials=Clinics.HasEmailHostingCredentials(clinic.ClinicNum);
				bool isSecureEmailActivated=Clinics.IsSecureEmailSignedUp(clinic.ClinicNum);
				bool isSecureEmailEnabled=Clinics.IsSecureEmailEnabled(clinic.ClinicNum);
				EmailPlatform emailPlatformDefault=PIn.Enum<EmailPlatform>(ClinicPrefs.GetPrefValue(PrefName.EmailDefaultSendPlatform,clinic.ClinicNum),true);
				row.Cells.Add(Clinics.GetAbbr(clinic.ClinicNum,listClinics));
				AddSignupCell(row,hasCredentials,isSecureEmailActivated,isSecureEmailEnabled,new EventHandler((o,e) => SignupSecureEmail(clinic.ClinicNum)));
				row.Cells.Add(new GridCell(defaultClinicNum==clinic.ClinicNum ? "X" : ""));
				row.Cells.Add(new GridCell($"{emailPlatformDefault.GetDescription(true)}") {
					ComboSelectedIndex=_listEmailPlatforms.FindIndex(x => x==emailPlatformDefault),
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
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			bool isSignedUpClinic0=Clinics.IsSecureEmailSignedUp(0);
			bool areAnyClinicsSignedUp=GetClinicNums().Any(x => Clinics.IsSecureEmailSignedUp(x));
			gridClinics.Visible=hasClinicsEnabled;
			gridClinics.Enabled=allowEdit;
			checkEnabled.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			labelPlatform.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			comboPlatform.Visible=!hasClinicsEnabled && isSignedUpClinic0;
			butSignup.Visible=!hasClinicsEnabled && !isSignedUpClinic0;
			butSignup.Enabled=allowEdit;
			checkEnabled.Enabled=allowEdit && isSignedUpClinic0;
			comboPlatform.Enabled=allowEdit && isSignedUpClinic0;
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
			long clinicNum=gridClinics.SelectedTag<Clinic>()?.ClinicNum??0;
			if(clinicNum==0) {
				Prefs.UpdateString(PrefName.EmailDefaultSendPlatform,emailPlatform.ToString());
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailDefaultSendPlatform,clinicNum,emailPlatform.ToString());
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void ToggleEnabled(long clinicNum,PrefName prefName) {
			if(clinicNum==0) {
				HostedEmailStatus status=PrefC.GetEnum<HostedEmailStatus>(prefName);
				status^=HostedEmailStatus.Enabled; 
				Prefs.UpdateInt(prefName,(int)status);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				HostedEmailStatus status=(HostedEmailStatus)ClinicPrefs.GetInt(prefName,clinicNum);
				status^=HostedEmailStatus.Enabled;
				ClinicPrefs.Upsert(prefName,clinicNum,$"{(int)status}");				
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			RefreshView();
		}

		private void InitDefaultClinic(long clinicNum=-1) {
			if(PrefC.GetLong(PrefName.EmailSecureDefaultClinic)==0) {
				if(clinicNum==-1) {
					clinicNum=Clinics.GetFirstOrDefault(x => Clinics.IsSecureEmailSignedUp(x.ClinicNum))?.ClinicNum??0;
				}
				//First clinic signed up for Secure.  Set as default clinic.
				SetDefaultSecureEmailClinic(clinicNum);
			}
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

		private void SignupSecureEmail(long clinicNum) {
			if(WebServiceMainHQProxy.GetWebServiceMainHQInstance() is WebServiceMainHQMockDemo webService) {
				//If we are not connecting to a real webservice, save this signup in memory.
				webService.SetEServiceCodeEnabled(eServiceCode.EmailSecureAccess,clinicNum,true);
			}
			else {
				//Signup may or may not be added in this window.
				using FormEServicesSignup formSignup=new FormEServicesSignup(_signup);
				formSignup.ShowDialog();
			}
			//Syncs signup prefs with HQ.  Creates credentials if necessary.
			_signup=DownloadSignups(_signup);
			if(_signup is null) {
				return;
			}
			ToggleEnabled(clinicNum,PrefName.EmailSecureStatus);
			if(Clinics.IsSecureEmailEnabled(clinicNum)) {
				InitDefaultClinic(clinicNum);
			}
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