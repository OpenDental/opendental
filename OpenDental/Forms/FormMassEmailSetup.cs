using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailSetup:FormODBase {
		private const string _activateMassEmailMessage="Mass Email is based on usage. By activating and enabling this feature you are agreeing to the charges. Continue?";
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signup;		

		public FormMassEmailSetup(WebServiceMainHQProxy.EServiceSetup.SignupOut signup=null) {
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
			string hostedEmailUrl=_signup?.EServices.FirstOrDefault(x => x.EService==eServiceCode.EmailMassUsage)?.HostedUrl;
			if(!string.IsNullOrWhiteSpace(hostedEmailUrl)) {
				webBrowser.Navigate(hostedEmailUrl);
			}
			else {
				webBrowser.Visible=false;
			}
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
			bool isSignedUp=Clinics.IsMassEmailSignedUp(0);
			bool isEnabled=Clinics.IsMassEmailEnabled(0);
			checkEnabled.Checked=isSignedUp && isEnabled;
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			ToggleEnabled(0,PrefName.MassEmailStatus);
		}

		private void FillClinicsGrid() {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true,"Headquarters");
			List<EmailPlatform> listEmailPlatforms=new List<EmailPlatform> { EmailPlatform.Unsecure, EmailPlatform.Secure };
			if(!PrefC.HasClinicsEnabled) {
				listClinics.RemoveAll(x => x.ClinicNum!=0);
			}
			gridClinics.BeginUpdate();
			gridClinics.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Clinic"),250);
			gridClinics.Columns.Add(col);		
			col=new GridColumn("Enabled",65,HorizontalAlignment.Center) {
				Tag=new OnGridCellClick((clinicNum) => ToggleEnabled(clinicNum,PrefName.MassEmailStatus)),
			};
			gridClinics.Columns.Add(col);
			col=new GridColumn("",65);
			gridClinics.Columns.Add(col);
			gridClinics.ListGridRows.Clear();
			foreach(Clinic clinic in listClinics) {
				GridRow row=new GridRow();
				bool hasCredentials=Clinics.HasEmailHostingCredentials(clinic.ClinicNum);
				bool isMassEmailActivated=Clinics.IsMassEmailSignedUp(clinic.ClinicNum);
				bool isMassEmailEnabled=Clinics.IsMassEmailEnabled(clinic.ClinicNum);
				row.Cells.Add(Clinics.GetAbbr(clinic.ClinicNum,listClinics));
				AddSignupCell(row,hasCredentials,isMassEmailActivated,isMassEmailEnabled,new EventHandler((o,e) => SignupMassEmail(clinic.ClinicNum)));
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
			bool isSignedUpClinic0=Clinics.IsMassEmailSignedUp(0);
			bool areAnyClinicsSignedUp=GetClinicNums().Any(x => Clinics.IsMassEmailSignedUp(x));
			gridClinics.Visible=hasClinicsEnabled;
			gridClinics.Enabled=allowEdit;
			checkEnabled.Visible=!hasClinicsEnabled;
			butSignup.Visible=!hasClinicsEnabled && !isSignedUpClinic0;
			butSignup.Enabled=allowEdit;
			checkEnabled.Enabled=allowEdit && isSignedUpClinic0;
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
			FillClinics();
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

		private void butSignup_Click(object sender,EventArgs e) {
			SignupMassEmail(0);
		}

		///<summary>Makes a web call to HQ to signup for Mass Email and generate Hosted Email credentials if necessary.</summary>
		private void SignupMassEmail(long clinicNum) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,_activateMassEmailMessage))	{
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			//Creates credentials (if necessary) and adds the signup.
			progressOD.ActionMain=() => WebServiceMainHQProxy.CreateHostedEmailCredentials(clinicNum,true,false);
			progressOD.StartingMessage="Signing up for Mass Email...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				FriendlyException.Show("An error occurred while signing up for Mass Email: "+ex.Message,ex);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			_signup=DownloadSignups(_signup);
			if(_signup is null) {
				return;
			}
			if(!Clinics.IsMassEmailEnabled(clinicNum)) {
				ToggleEnabled(clinicNum,PrefName.MassEmailStatus);
			}
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

		private void butTemplates_Click(object sender,EventArgs e) {			
			using FormMassEmailTemplates formTemplates=new FormMassEmailTemplates();
			formTemplates.ShowDialog();
		}

		private void butAnalytics_Click(object sender,EventArgs e) {
			using FormMassEmailAnalytics formMassEmailAnalytics=new FormMassEmailAnalytics();
			formMassEmailAnalytics.ShowDialog();
		}

		private void butBirthdays_Click(object sender,EventArgs e) {
			using FormMassEmailBirthdays formMassEmailBirthdays=new FormMassEmailBirthdays();
			formMassEmailBirthdays.ShowDialog();
		}

		private delegate void OnGridCellClick(long clinicNum);
	}
}