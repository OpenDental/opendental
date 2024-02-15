using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmShortCodeOptIn:FrmODBase {
		public Patient PatientCur;

		public FrmShortCodeOptIn() {
			InitializeComponent();
			Load+=FrmShortCodeOptIn_Load;
			FormClosing+=FrmShortCodeOptIn_FormClosing;
		}

		private void FrmShortCodeOptIn_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(string.IsNullOrWhiteSpace(PatientCur?.WirelessPhone??"")) {
				MsgBox.Show(this,"Patient must have a Wireless Phone.");
				IsDialogOK=false;
				return;
			}
			string script=PrefC.GetString(PrefName.ShortCodeOptInScript);
			if(PatientCur.ShortCodeOptIn==YN.No) {
				PrefC.GetString(PrefName.ShortCodeOptedOutScript);
			}
			if(!string.IsNullOrWhiteSpace(script)) {
				label1.Text=FillInTextTemplate(script,PatientCur);
			}
			//Defaulting to true when IsShortCodeSmsAnOption is YN.Unknown will likely result in higher participation.
			checkOptIn.Checked= PatientCur.ShortCodeOptIn.In(YN.Yes,YN.Unknown);
			//Dentist is only allowed to change the value when already set to YN.Yes or YN.Unknown.  In other words, disabled when patient has opted out.
			checkOptIn.Visible= PatientCur.ShortCodeOptIn.In(YN.Yes,YN.Unknown);
			LayoutMenu();
		}

		public static string FillInTextTemplate(string template,Patient patient) {
			if(!template.Contains("[Practice Name]")) {
				return template
				.Replace("[FName]",patient.GetNameFirstOrPreferred())
				.Replace("[WirelessPhone]",patient.WirelessPhone);
			}
			//fill in the [Practice Name] 
			//Default to practice title.
			string clinicName="";
			if(PrefC.HasClinicsEnabled) {
				//Try to get clinic title speficially set for this clinic's opt-in.
				clinicName=ClinicPrefs.GetPrefValue(PrefName.ShortCodeOptInClinicTitle,patient.ClinicNum);
				if(string.IsNullOrEmpty(clinicName)) { //No explicit title found so use clinic.Desc.
					clinicName=Clinics.GetDesc(patient.ClinicNum);
				}
			}
			if(string.IsNullOrEmpty(clinicName)) {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			//We could not find a suitable title so use a generic title.
			if(string.IsNullOrEmpty(clinicName)) {
				clinicName="your dentist";
			}
			return template
				.Replace("[FName]",patient.GetNameFirstOrPreferred())
				.Replace("[WirelessPhone]",patient.WirelessPhone)
				.Replace("[Practice Name]",clinicName);
		}

		private void LayoutMenu() {
			menuMain.Add(new MenuItem("Setup",settingsToolStripMenuItem_Click));
		}

		///<summary>Returns true if this form needs to be shown to the user based on practice and patient settings.  False otherwise.</summary>
		public static void PromptIfNecessary(Patient patient,long clinicNum) {
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(patient)).FirstOrDefault();
			if(patComm is null) {
				return;
			}
			bool isClinicPrompting=ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,clinicNum);
			if(!isClinicPrompting//Auto prompt is disabled (practice pref if not found for clinic)
				&& patComm.IsPatientShortCodeEligible(clinicNum)//The patient might be able to receive short code sms.
				&& patient.ShortCodeOptIn==YN.Unknown)//And the patient has not explicitly opted in or out yet.
			{
				//Provider has acknowledged they will have the Appt Text opt-in conversation with the patient in absence of this prompt, and that this will
				//cause "Unknown" patients to be automatically opted-in.  HQ has a preference to disable auto Unknown->Yes if necessary.
				TrySendToHq(patient.WirelessPhone,YN.Yes,patient.PatNum,clinicNum,isSilent:true);
				return;
			}
			bool isPromptNecessary=isClinicPrompting//Auto prompt is enabled (practice pref if not found for clinic)
				&& patComm.IsPatientShortCodeEligible(clinicNum)
				&& patient.ShortCodeOptIn==YN.Unknown; //Patient has not opted in or out for short codes yet.
			if(!isPromptNecessary) {
				return;
			}
			FrmShortCodeOptIn frmShortCodeOptIn=new FrmShortCodeOptIn();
			frmShortCodeOptIn.PatientCur=patient;
			frmShortCodeOptIn.ShowDialog();
		}
		
		///<summary>Sends a request to HQ to update the Short Code Opt In status of this patient.  Can be sent silently without any feedback in the UI.
		///</summary>
		private static bool TrySendToHq(string wirelessPhone,YN optIn,long patNum,long clinicNum,bool isSilent=false) {
			List<PayloadItem> listPayloadItems=new List<PayloadItem>(){
				new PayloadItem(wirelessPhone,"PhonePat"),
				new PayloadItem((int)optIn,"ShortCodeOptInInt"),
				new PayloadItem(patNum,"PatNum"),
				new PayloadItem(clinicNum,"ClinicNum"),
				new PayloadItem(false,"IsWebSchedNewPat"),
				new PayloadItem(isSilent,"IsSilentUpdate"),
			};
			string result="";
			if(isSilent) {
				ODThread threadSilent=new ODThread((o) => WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.SetSmsPatientPhoneOptIn(PayloadHelper.CreatePayload(listPayloadItems,eServiceCode.IntegratedTexting)));
				threadSilent.AddExceptionHandler((ex) => ex.DoNothing());
				threadSilent.Name="ShortCodeOptIn";
				threadSilent.Start();
				return true;
			}
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.SetSmsPatientPhoneOptIn(PayloadHelper.CreatePayload(listPayloadItems,eServiceCode.IntegratedTexting));
			};
			progressOD.ShowDialog();
			if(progressOD.IsCancelled){
				return false;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			XmlNode xmlNode=xmlDocument.SelectSingleNode("//ListSmsToMobiles");
			if(xmlNode is null) {
				xmlNode=xmlDocument.SelectSingleNode("//Error");
				if(!(xmlNode is null)) {
					MessageBox.Show(Lang.g("ShortCodes","An error occurred: ")+xmlNode.InnerText);
				}
				return false;
			}
			List<SmsToMobile> listSmsToMobiles;
			XmlReader xmlReader=XmlReader.Create(new System.IO.StringReader(xmlNode.InnerXml));
			System.Xml.Serialization.XmlSerializer xmlSerializerListSmsToMobile=new System.Xml.Serialization.XmlSerializer(typeof(List<SmsToMobile>));
			listSmsToMobiles=(List<SmsToMobile>)xmlSerializerListSmsToMobile.Deserialize(xmlReader);
			if(listSmsToMobiles==null) { //List should always be there even if it's empty.
				MessageBox.Show(Lang.g("ShortCodes","An error occurred: ")+xmlNode.InnerText);
				return false;
			}
			//Should only be 0 or 1.
			if(listSmsToMobiles.Count>0) {
				listSmsToMobiles.ForEach(x => x.DateTimeSent=DateTime.Now);
				SmsToMobiles.InsertMany(listSmsToMobiles);
				string message=$"{wirelessPhone} will shortly receive the following message{(listSmsToMobiles.Count>1 ? "s" : "")}:\n"
					+string.Join("\n",listSmsToMobiles.Select((x,i) => $"{i+1}) {x.MsgText}"));
				MessageBox.Show(message,"Appointment Texts");
			}
			//Local OptIn status for this patient will be updated by a Transmission from HQ, resulting from this call to SetSmsPatientPhoneOptIn().
			return true;
		}

		private void settingsToolStripMenuItem_Click(object sender,EventArgs e) {
			FormLauncher formLauncher=new FormLauncher(EnumFormName.FormEServicesTexting);
			formLauncher.ShowDialog();
		}

		private void FrmShortCodeOptIn_FormClosing(object sender,EventArgs e) {
			if(IsDialogOK) {
				return;
			}
			if(string.IsNullOrWhiteSpace(PatientCur?.WirelessPhone??"")) {//there is a DialogResult.Cancel in the Load caused by this scenario and is already addressed
				return;
			}
			//Closed form by X Click
			string msg;
			if(PatientCur.ShortCodeOptIn==YN.Unknown) {
				//Dentist is only allowed to explicitly opt in the patient the first time.  After that, the patient must text START.
				if(checkOptIn.Checked==true) {//opting in will result in an OptInReply being sent.
					msg=Lang.g(this,"An SMS confirming this selection will be sent to ")+PatientCur.WirelessPhone+". Continue?";
				}
				else {
					msg=Lang.g(this,"Are you sure ")+PatientCur.GetNameFirstOrPreferred()+Lang.g(this," does NOT want to receive appointment reminders?");
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msg)) {
					return;
				}
				if(TrySendToHq(PatientCur.WirelessPhone,checkOptIn.Checked==true ? YN.Yes : YN.No,PatientCur.PatNum,Clinics.ClinicNum)) {
					//This value will actually be set by a transmission from HQ, but for the sake of the UI, set it here.
					PatientCur.ShortCodeOptIn=checkOptIn.Checked==true ? YN.Yes : YN.No;
					IsDialogOK=true;
					return;
				}
			}
			else if(PatientCur.ShortCodeOptIn==YN.Yes && checkOptIn.Checked==false) {
				msg=Lang.g(this,"Are you sure ")+PatientCur.GetNameFirstOrPreferred()+Lang.g(this," does NOT want to receive appointment reminders?");
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msg)) {
					return;
				}
				//Dentist is only allowed to opt-out a patient who was previously set to opted-in.
				if(TrySendToHq(PatientCur.WirelessPhone,checkOptIn.Checked==true ? YN.Yes : YN.No,PatientCur.PatNum,Clinics.ClinicNum)) {
					//This value will actually be set by a transmission from HQ, but for the sake of the UI, set it here.
					PatientCur.ShortCodeOptIn=checkOptIn.Checked==true ? YN.Yes : YN.No;
					IsDialogOK=true;
					return;
				}
			}
		}

	}
}