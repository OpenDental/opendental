using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormShortCodeOptIn:FormODBase {
		private Patient _pat;

		public FormShortCodeOptIn(Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_pat=pat;
		}

		private void FormShortCodeOptIn_Load(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(_pat?.WirelessPhone??"")) {
				MsgBox.Show(this,"Patient must have a Wireless Phone.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			string script=_pat.ShortCodeOptIn==YN.No ? PrefC.GetString(PrefName.ShortCodeOptedOutScript) : PrefC.GetString(PrefName.ShortCodeOptInScript);
			if(!string.IsNullOrWhiteSpace(script)) {
				label1.Text=FillInTextTemplate(script,_pat);
			}
			//Defaulting to true when IsShortCodeSmsAnOption is YN.Unknown will likely result in higher participation.
			checkOptIn.Checked=ListTools.In(_pat.ShortCodeOptIn,YN.Yes,YN.Unknown);
			//Dentist is only allowed to change the value when already set to YN.Yes or YN.Unknown.  In other words, disabled when patient has opted out.
			checkOptIn.Visible=ListTools.In(_pat.ShortCodeOptIn,YN.Yes,YN.Unknown);
			LayoutMenu();
		}

		public static string FillInTextTemplate(string template,Patient pat) {
			return template
				.Replace("[FName]",pat.GetNameFirstOrPreferred())
				.Replace("[WirelessPhone]",pat.WirelessPhone);
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",settingsToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>Returns true if this form needs to be shown to the user based on practice and patient settings.  False otherwise.</summary>
		public static void PromptIfNecessary(Patient pat,long clinicNum) {
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(pat)).FirstOrDefault();
			if(patComm is null) {
				return;
			}
			bool isClinicPrompting=ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,clinicNum);
			if(!isClinicPrompting//Auto prompt is disabled (practice pref if not found for clinic)
				&& patComm.IsPatientShortCodeEligible(clinicNum)//The patient might be able to receive short code sms.
				&& pat.ShortCodeOptIn==YN.Unknown)//And the patient has not explicitly opted in or out yet.
			{
				//Provider has acknowledged they will have the Appt Text opt-in conversation with the patient in absence of this prompt, and that this will
				//cause "Unknown" patients to be automatically opted-in.  HQ has a preference to disable auto Unknown->Yes if necessary.
				TrySendToHq(pat.WirelessPhone,YN.Yes,pat.PatNum,clinicNum,isSilent:true);
				return;
			}
			bool isPromptNecessary=isClinicPrompting//Auto prompt is enabled (practice pref if not found for clinic)
				&& patComm.IsPatientShortCodeEligible(clinicNum)
				&& pat.ShortCodeOptIn==YN.Unknown; //Patient has not opted in or out for short codes yet.
			if(!isPromptNecessary) {
				return;
			}
			using FormShortCodeOptIn FormShortCode=new FormShortCodeOptIn(pat);
			FormShortCode.ShowDialog();
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
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.SetSmsPatientPhoneOptIn(PayloadHelper.CreatePayload(listPayloadItems,eServiceCode.IntegratedTexting));
			};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return false;
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XmlNode node=doc.SelectSingleNode("//ListSmsToMobiles");
			if(node is null) {
				node=doc.SelectSingleNode("//Error");
				if(!(node is null)) {
					MessageBox.Show(Lan.g("ShortCodes","An error occurred: ")+node.InnerText);
				}
				return false;
			}
			List<SmsToMobile> listSmsToMobiles;
			using(XmlReader reader=XmlReader.Create(new System.IO.StringReader(node.InnerXml))) {
				System.Xml.Serialization.XmlSerializer xmlListSmsToMobileSerializer=new System.Xml.Serialization.XmlSerializer(typeof(List<SmsToMobile>));
				listSmsToMobiles=(List<SmsToMobile>)xmlListSmsToMobileSerializer.Deserialize(reader);
			}
			if(listSmsToMobiles==null) { //List should always be there even if it's empty.
				MessageBox.Show(Lan.g("ShortCodes","An error occurred: ")+node.InnerText);
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
			using FormEServicesTexting formESTexting=new FormEServicesTexting();
			formESTexting.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			string msg;
			if(_pat.ShortCodeOptIn==YN.Unknown) {
				//Dentist is only allowed to explicitly opt in the patient the first time.  After that, the patient must text START.
				if(checkOptIn.Checked) {//opting in will result in an OptInReply being sent.
					msg=Lan.g(this,"An SMS confirming this selection will be sent to ")+_pat.WirelessPhone;
				}
				else {
					msg=Lan.g(this,"Are you sure ")+_pat.GetNameFirstOrPreferred()+Lan.g(this," does NOT want to receive appointment reminders?");
				}
				if(MessageBox.Show(this,msg,Lan.g(this,"Continue?"),MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
				if(TrySendToHq(_pat.WirelessPhone,checkOptIn.Checked ? YN.Yes : YN.No,_pat.PatNum,Clinics.ClinicNum)) {
					//This value will actually be set by a transmission from HQ, but for the sake of the UI, set it here.
					_pat.ShortCodeOptIn=checkOptIn.Checked ? YN.Yes : YN.No;
					DialogResult=DialogResult.OK;
					return;
				}
			}
			else if(_pat.ShortCodeOptIn==YN.Yes && !checkOptIn.Checked) {
				msg=Lan.g(this,"Are you sure ")+_pat.GetNameFirstOrPreferred()+Lan.g(this," does NOT want to receive appointment reminders?");
				if(MessageBox.Show(this,msg,Lan.g(this,"Continue?"),MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
				//Dentist is only allowed to opt-out a patient who was previously set to opted-in.
				if(TrySendToHq(_pat.WirelessPhone,checkOptIn.Checked ? YN.Yes : YN.No,_pat.PatNum,Clinics.ClinicNum)) {
					//This value will actually be set by a transmission from HQ, but for the sake of the UI, set it here.
					_pat.ShortCodeOptIn=checkOptIn.Checked ? YN.Yes : YN.No;
					DialogResult=DialogResult.OK;
					return;
				}
			}
			DialogResult=DialogResult.Cancel;			
		}
	}
}