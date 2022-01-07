using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {

	public partial class FormEServicesMassEmail:FormODBase {
		private bool _doSetInvalidClinicPrefs=false;
		private bool _doSetInvalidPrefs=false;
		private string _activateMessage;
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

		//==================== Promotion Variables ====================
		private Clinic GetClinicPromoCur() {
			if(comboClinicMassEmail.IsUnassignedSelected) {
				return Clinics.GetPracticeAsClinicZero();//Get the default/HQ clinic
			}
			return comboClinicMassEmail.GetSelectedClinic();
		}

		public FormEServicesMassEmail(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesMassEmail_Load(object sender,EventArgs e) {
			FillComboClinicsMassEmail();
			FillPromotionForClinic();
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			_activateMessage="Mass Email is based on usage. By activating and enabling this feature you are agreeing to the charges. Continue?";
			checkIsMassEmailEnabled.Enabled=allowEdit;
			checkIsSecureEnabled.Enabled=allowEdit;
			butActivate.Enabled=allowEdit;
			WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService signupOutMassEmail=_signupOut.EServices
				.FirstOrDefault(x => x.EService==eServiceCode.EmailMassUsage);
			if(signupOutMassEmail!=null) {
				webBrowser1.Navigate(signupOutMassEmail.HostedUrl);
			}
			else {
				webBrowser1.Visible=false;
			}
		}

		private void FillComboClinicsMassEmail() {
			comboClinicMassEmail.IsUnassignedSelected=true;
		}

		///<summary>Fills all necessary data for the clinic.</summary>
		private void FillPromotionForClinic() {
			long clinicNum=GetClinicPromoCur().ClinicNum;
			bool hasCredentials;
			if(clinicNum==0) {
				hasCredentials=!string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.MassEmailGuid))
					&& !string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.MassEmailSecret));
			}
			else {
				hasCredentials=!string.IsNullOrWhiteSpace(ClinicPrefs.GetPref(PrefName.MassEmailGuid,clinicNum)?.ValueString)
					&& !string.IsNullOrWhiteSpace(ClinicPrefs.GetPref(PrefName.MassEmailSecret,clinicNum)?.ValueString);
			} 
			butActivate.Visible=!hasCredentials;
			checkIsMassEmailEnabled.Visible=hasCredentials && Clinics.IsMassEmailSignedUp(clinicNum);
			checkIsMassEmailEnabled.Checked=Clinics.IsMassEmailEnabled(clinicNum);
			checkIsSecureEnabled.Visible=hasCredentials && Clinics.IsSecureEmailSignedUp(clinicNum);
			checkIsSecureEnabled.Checked=Clinics.IsSecureEmailEnabled(clinicNum);
		}

		private void comboClinicPromotion_SelectionChangeCommitted(object sender,EventArgs e) {
			FillPromotionForClinic();
		}

		private void butActivate_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,_activateMessage))	{
				return;
			}
			IWebServiceMainHQ iWebServiceMainHQ=WebServiceMainHQProxy.GetWebServiceMainHQInstance();
			string guid="";
			string secret="";
			long clinicNum=GetClinicPromoCur().ClinicNum;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				List<PayloadItem> listPayloadItems=new List<PayloadItem>{new PayloadItem(clinicNum,"ClinicNum")};
				string officeData=PayloadHelper.CreatePayload(listPayloadItems,eServiceCode.Undefined);
				string result=iWebServiceMainHQ.EmailHostingSignup(officeData);
				guid=WebSerializer.DeserializeTag<string>(result,"AccountGUID");
				ODException.SwallowAnyException(() => {
					//Only sent the first time EmailHostingSignup is called.
					secret=WebSerializer.DeserializeTag<string>(result,"AccountSecret");
				});
			};
			progressOD.StartingMessage="Activating promotions...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				FriendlyException.Show("An error occurred while activating Mass Email: "+ex.Message,ex);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			//Rather than upserting for clinnicpref 0, needs to be preference
			if(!PrefC.HasClinicsEnabled || clinicNum==0) {
				Prefs.UpdateInt(PrefName.MassEmailStatus,(int)(HostedEmailStatus.SignedUp | HostedEmailStatus.Enabled));
				if(!string.IsNullOrWhiteSpace(guid)) {
					Prefs.UpdateString(PrefName.MassEmailGuid,guid);
				}
				if(!string.IsNullOrWhiteSpace(secret)) {
					Prefs.UpdateString(PrefName.MassEmailSecret,secret);
				}
				Prefs.RefreshCache();
				_doSetInvalidPrefs=true;
			}
			else {
				//If we made it this far, we have activated that account successfully.
				ClinicPrefs.Upsert(PrefName.MassEmailStatus,clinicNum,((int)(HostedEmailStatus.SignedUp | HostedEmailStatus.Enabled)).ToString());
				if(!string.IsNullOrWhiteSpace(guid)) {
					ClinicPrefs.Upsert(PrefName.MassEmailGuid,clinicNum,guid);
				}
				if(!string.IsNullOrWhiteSpace(secret)) {
					ClinicPrefs.Upsert(PrefName.MassEmailSecret,clinicNum,secret);
				}
				ClinicPrefs.RefreshCache();
				_doSetInvalidClinicPrefs=true;
			}
			FillPromotionForClinic();
		}

		private void checkIsMassEmailEnabled_Click(object sender,EventArgs e) {
			ChangeEnabled(PrefName.MassEmailStatus,checkIsMassEmailEnabled.Checked);
		}

		private void checkIsSecureEmailEnabled_Click(object sender,EventArgs e) {
			ChangeEnabled(PrefName.EmailSecureStatus,checkIsSecureEnabled.Checked);
		}

		private void ChangeEnabled(PrefName prefName,bool isEnabling) {
			if(prefName==PrefName.MassEmailStatus && isEnabling && !MsgBox.Show(this,MsgBoxButtons.YesNo,_activateMessage))	{//box is being checked
				checkIsMassEmailEnabled.Checked=false;
				return;
			}
			long clinicNum=GetClinicPromoCur().ClinicNum;
			HostedEmailStatus emailStatus=HostedEmailStatus.NotActivated;
			if(clinicNum==0) {
				emailStatus=PrefC.GetEnum<HostedEmailStatus>(prefName);
				if(isEnabling){
					emailStatus=EnumTools.AddFlag(emailStatus,HostedEmailStatus.Enabled);
				}
				else {
					emailStatus=EnumTools.RemoveFlag(emailStatus,HostedEmailStatus.Enabled);
				}
				Prefs.UpdateInt(prefName,(int)emailStatus);
				Prefs.RefreshCache();
				_doSetInvalidPrefs=true;
			}
			else {
				emailStatus=PIn.Enum<HostedEmailStatus>(ClinicPrefs.GetInt(prefName,clinicNum));
				if(isEnabling) {
					emailStatus=EnumTools.AddFlag(emailStatus,HostedEmailStatus.Enabled);
				}
				else {
					emailStatus=EnumTools.RemoveFlag(emailStatus,HostedEmailStatus.Enabled);
				}
				ClinicPrefs.Upsert(prefName,clinicNum,((int)emailStatus).ToString());
				ClinicPrefs.RefreshCache();
				_doSetInvalidClinicPrefs=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			if(_doSetInvalidPrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void button1_Click(object sender,EventArgs e) {
			long clinicNum=comboClinicMassEmail.SelectedClinicNum;
			if(clinicNum==0) {
				HostedEmailStatus emailStatus=PrefC.GetEnum<HostedEmailStatus>(PrefName.EmailSecureStatus);
				emailStatus=emailStatus ^ HostedEmailStatus.SignedUp;
				Prefs.UpdateInt(PrefName.EmailSecureStatus,(int)emailStatus);
				Prefs.RefreshCache();
			}
			else  {
				HostedEmailStatus emailStatus=(HostedEmailStatus)ClinicPrefs.GetInt(PrefName.EmailSecureStatus,clinicNum);
				emailStatus=emailStatus ^ HostedEmailStatus.SignedUp;
				ClinicPrefs.Upsert(PrefName.EmailSecureStatus,clinicNum,$"{(long)emailStatus}");
				ClinicPrefs.RefreshCache();
			}
			FillPromotionForClinic();
		}

		private void button2_Click(object sender,EventArgs e) {
			long clinicNum=comboClinicMassEmail.SelectedClinicNum;
			if(clinicNum==0) {
				Prefs.UpdateString(PrefName.MassEmailGuid,"");
				Prefs.UpdateString(PrefName.MassEmailSecret,"");
				Prefs.UpdateInt(PrefName.MassEmailStatus,0);
				Prefs.UpdateInt(PrefName.EmailSecureStatus,0);
				Prefs.RefreshCache();
			}
			else  {
				ClinicPrefs.Upsert(PrefName.MassEmailGuid,clinicNum,"");
				ClinicPrefs.Upsert(PrefName.MassEmailSecret,clinicNum,"");
				ClinicPrefs.Upsert(PrefName.MassEmailStatus,clinicNum,"0");
				ClinicPrefs.Upsert(PrefName.EmailSecureStatus,clinicNum,"0");
				ClinicPrefs.Upsert(PrefName.EmailSecureStatus,clinicNum,$"0");
				ClinicPrefs.RefreshCache();
			}
			FillPromotionForClinic();
		}
	}
}