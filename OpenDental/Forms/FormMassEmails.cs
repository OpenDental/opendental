using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormMassEmails:FormODBase {
		private const string _activateMassEmailMessage="Mass Email is based on usage. By activating and enabling this feature you are agreeing to the charges. Continue?";
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signup;		

		public FormMassEmails(WebServiceMainHQProxy.EServiceSetup.SignupOut signup=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signup=signup;
		}

		private void FormMassEmailSetup_Load(object sender,EventArgs e) {
			if(_signup is null) {
				_signup=FormEServicesSetup.GetSignupOut();
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

		private void FillClinics() {
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true,"Headquarters");
			if(!PrefC.HasClinicsEnabled) {
				listClinics.RemoveAll(x => x.ClinicNum!=0);
			}
			gridClinics.BeginUpdate();
			gridClinics.ListGridColumns.Clear();
			GridColumn col;	
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),250);
				gridClinics.ListGridColumns.Add(col);		
			}
			col=new GridColumn("Mass Email Enabled",65,HorizontalAlignment.Center) {
				Tag=new OnGridCellClick((clinicNum) => ToggleEnabled(clinicNum,PrefName.MassEmailStatus)),
			};
			gridClinics.ListGridColumns.Add(col);
			//Add this line back in when the Secure Email front end and web client are complete.
			//col=new GridColumn("Secure Email Enabled",65,HorizontalAlignment.Center) {
			//	Tag=new OnGridCellClick((clinicNum) => ToggleEnabled(clinicNum,PrefName.EmailSecureStatus)),
			//};
			//gridClinics.ListGridColumns.Add(col);				
			col=new GridColumn("",65);
			gridClinics.ListGridColumns.Add(col);
			gridClinics.ListGridRows.Clear();
			foreach(Clinic clinic in listClinics) {
				GridRow row=new GridRow();
				bool hasCredentials=Clinics.HasEmailHostingCredentials(clinic.ClinicNum);
				bool isMassEmailActivated=Clinics.IsMassEmailSignedUp(clinic.ClinicNum);
				bool isMassEmailEnabled=Clinics.IsMassEmailEnabled(clinic.ClinicNum);
				bool isSecureEmailActivated=Clinics.IsSecureEmailSignedUp(clinic.ClinicNum);
				bool isSecureEmailEnabled=Clinics.IsSecureEmailEnabled(clinic.ClinicNum);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(clinic.ClinicNum,listClinics));
				}
				AddSignupCell(row,hasCredentials,isMassEmailActivated,isMassEmailEnabled,new EventHandler((o,e) => SignupMassEmail(clinic.ClinicNum)));
				//Add this line back in when the Secure Email front end and web client are complete.
				//AddSignupCell(row,hasCredentials,isSecureEmailActivated,isSecureEmailEnabled,new EventHandler((o,e) => SignupSecureEmail(clinic.ClinicNum)));
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
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			gridClinics.Enabled=allowEdit;
			bool areAnyClinicsActivated=gridClinics.GetTags<Clinic>().Any(x => Clinics.IsMassEmailSignedUp(x.ClinicNum) || Clinics.IsSecureEmailSignedUp(x.ClinicNum));
			bool areAnyClinicsActivatedMass=gridClinics.GetTags<Clinic>().Any(x => Clinics.IsMassEmailSignedUp(x.ClinicNum));
			groupSetup.Enabled=allowEdit && areAnyClinicsActivated;
			groupMassEmail.Enabled=areAnyClinicsActivatedMass;
		}

		private void gridClinics_CellClick(object sender,ODGridClickEventArgs e) {
			Clinic clinic=gridClinics.SelectedTag<Clinic>();
			GridColumn col=gridClinics.ListGridColumns[e.Col];
			if(clinic!=null && col.Tag is OnGridCellClick onClick) {
				onClick(clinic.ClinicNum);
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

		private void SignupSecureEmail(long clinicNum) {
			if(ODBuild.IsDebug()) {
				new WebServiceMainHQMockDemo().SetEServiceCodeEnabled(eServiceCode.EmailSecureAccess,clinicNum,true);
			}
			else {
				//Signup may or may not be added in this window.
				using FormEServicesSignup formSignup=new FormEServicesSignup(_signup);
				formSignup.ShowDialog();
			}
			//Syncs signup prefs with HQ.  Creates credentials if necessary.
			_signup=FormEServicesSetup.GetSignupOut(_signup);
			if(!Clinics.IsSecureEmailEnabled(clinicNum)) {
				ToggleEnabled(clinicNum,PrefName.EmailSecureStatus);
			}
			RefreshView();
		}

		///<summary>Makes a web call to HQ to signup for Mass Email and generate Hosted Email credentials if necessary.</summary>
		private void SignupMassEmail(long clinicNum) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,_activateMassEmailMessage))	{
				return;
			}
			IWebServiceMainHQ webServiceMainHQ=null;
			if(ODBuild.IsDebug()) {
				webServiceMainHQ=new WebServiceMainHQMockDemo();
			}
			ProgressOD progressOD=new ProgressOD();
			//Creates credentials (if necessary) and adds the signup.
			progressOD.ActionMain=() => WebServiceMainHQProxy.CreateHostedEmailCredentials(clinicNum,true,false,webServiceMainHQ);
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
			try {
				_signup=FormEServicesSetup.GetSignupOut();			
			}
			catch(Exception ex) {
				FriendlyException.Show("An error occurred while signing up for Mass Email.",ex);
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

		private void butManualMassEmail_Click(object sender,EventArgs e) {
			using FormMassEmailList formMassEmailList=new FormMassEmailList();
			formMassEmailList.ShowDialog();
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