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
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

		public FormMassEmailSetup(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormMassEmailSetup_Load(object sender,EventArgs e) {
			if(_signupOut is null) {
				_signupOut=DownloadSignups(_signupOut);
				if(_signupOut is null) {
					Close();
					return;
				}
			}
			string hostedEmailUrl=_signupOut?.EServices.FirstOrDefault(x => x.EService==eServiceCode.EmailMassUsage)?.HostedUrl;
			if(!string.IsNullOrWhiteSpace(hostedEmailUrl)) {
				webBrowser.Navigate(hostedEmailUrl);
			}
			else {
				webBrowser.Visible=false;
			}
			RefreshView();
		}

		private WebServiceMainHQProxy.EServiceSetup.SignupOut DownloadSignups(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOutOld) {
			WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null;
			try {
				signupOut=FormEServicesSetup.GetSignupOut(signupOutOld);
			}
			catch(WebException e) {
				FriendlyException.Show(Lan.g(this,"Could not reach HQ.  Please make sure you have an internet connection and try again or call support."),e);
			}
			catch(Exception e) {
				FriendlyException.Show(Lan.g(this,"There was a problem loading settings.  Please try again or call support."),e);
			}
			return signupOut;
		}

		private void FillClinics() {
			if(PrefC.HasClinicsEnabled) {
				FillClinicsGrid();
				return;
			}
			FillNoClinics();
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
			for(int i=0;i<listClinics.Count;i++){
				GridRow row=new GridRow();
				bool hasCredentials=Clinics.HasEmailHostingCredentials(listClinics[i].ClinicNum);
				bool isMassEmailActivated=Clinics.IsMassEmailSignedUp(listClinics[i].ClinicNum);
				bool isMassEmailEnabled=Clinics.IsMassEmailEnabled(listClinics[i].ClinicNum);
				row.Cells.Add(Clinics.GetAbbr(listClinics[i].ClinicNum,listClinics));
				AddSignupCell(row,hasCredentials,isMassEmailActivated,isMassEmailEnabled,new EventHandler((o,e) => SignupMassEmail(listClinics[i].ClinicNum)));
				row.Cells.Add("");
				row.Tag=listClinics[i];
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
				row.Cells.Add(gridCell);
				return;
			}
			//No Hosted Email credentials or eSerivceSignup.  Clinic must generate credentials and sign up.
			gridCell=new GridCell(Lan.g(this,"Sign Up")) {
				ClickEvent=onClickSignup,
			};
			row.Cells.Add(gridCell);
		}

		private void RefreshView() {
			FillClinics();
			UpdateControlVisibility();
		}

		private void UpdateControlVisibility() {
			bool hasClinicsEnabled=PrefC.HasClinicsEnabled;
			bool allowEdit=Security.IsAuthorized(EnumPermType.EServicesSetup,true);
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
			return new List<long> { 0 };
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
				HostedEmailStatus hostedEmailStatus=PrefC.GetEnum<HostedEmailStatus>(prefName);
				hostedEmailStatus^=HostedEmailStatus.Enabled; 
				Prefs.UpdateInt(prefName,(int)hostedEmailStatus);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				HostedEmailStatus hostedEmailStatus=(HostedEmailStatus)ClinicPrefs.GetInt(prefName,clinicNum);
				hostedEmailStatus^=HostedEmailStatus.Enabled;
				ClinicPrefs.Upsert(prefName,clinicNum,$"{(int)hostedEmailStatus}");				
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
			catch(Exception e){
				FriendlyException.Show("An error occurred while signing up for Mass Email: "+e.Message,e);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			_signupOut=DownloadSignups(_signupOut);
			if(_signupOut is null) {
				return;
			}
			if(!Clinics.IsMassEmailEnabled(clinicNum)) {
				ToggleEnabled(clinicNum,PrefName.MassEmailStatus);
			}
			RefreshView();
		}

		private void butAddresses_Click(object sender,EventArgs e) {
			using FormEmailHostingAddressVerification formEmailHostingAddressVerification=new FormEmailHostingAddressVerification();
			formEmailHostingAddressVerification.ShowDialog();
		}

		private void butSignatures_Click(object sender,EventArgs e) {
			using FormEmailHostingSignatures formEmailHostingSignatures=new FormEmailHostingSignatures();
			formEmailHostingSignatures.ShowDialog();
		}

		private void butTemplates_Click(object sender,EventArgs e) {			
			using FormMassEmailTemplates formMassEmailTemplates=new FormMassEmailTemplates();
			formMassEmailTemplates.ShowDialog();
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