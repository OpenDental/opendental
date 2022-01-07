using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTxtMsgEdit:FormODBase {
		///<summary>PatNum of the patient selected.  Required if sending message without loading form or loading with patient selected.</summary>
		public long PatNum=0;
		///<summary>WirelessPhone of the patient selected.  Required if sending message without loading form.  
		///Once form is loaded uses textWirelessPhone.Text instead.</summary>
		public string WirelessPhone;
		///<summary>Message text to be sent.  Required if sending message without loading form.  Once form is loaded uses textMessage.Text instead.</summary>
		public string Message;
		///<summary>TxtMsgOk status of the patient.  Required if sending message without loading form or loading with patient selected.</summary>
		public YN TxtMsgOk;
		
		public FormTxtMsgEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTxtMsgEdit_Load(object sender,EventArgs e) {
			textWirelessPhone.Text=WirelessPhone;
			textMessage.Text=Message;
			SetMessageCounts();
			if(PatNum==0) {
				radioPatient.Checked=false;
				radioOther.Checked=true;
				butPatFind.Enabled=false;
				textPatient.Enabled=false;
				textWirelessPhone.ReadOnly=false;
				textPatient.Text="";
				textWirelessPhone.Text="";
			}
			else {
				radioPatient.Checked=true;
				radioOther.Checked=false;
				butPatFind.Enabled=true;
				textPatient.Enabled=true;
				textWirelessPhone.ReadOnly=true;
				textPatient.Text=Patients.GetPat(PatNum).GetNameLF();
				textWirelessPhone.Text=WirelessPhone;
			}
			SetFilterControlsAndAction(() => SetMessageCounts(),0,textMessage);
		}

		private void SetMessageCounts() {
			textCharCount.Text=textMessage.TextLength.ToString();
			textMsgCount.Text=SmsPhones.CalculateMessagePartsNumber(textMessage.Text).ToString();
		}

		private void FormTxtMsgEdit_Shown(object sender,EventArgs e) {
			textMessage.Focus();
			textMessage.Select(textMessage.Text.Length,0);
		}
		/// <summary>May be called from other parts of the program without showing this form. You must still create an instance of this form though. 
		/// Checks CallFire bridge, if it is OK to send a text, etc. (Buttons to load this form are usually disabled if it is not OK, 
		/// but this is needed for Confirmations, Recalls, etc.). CanIncreaseLimit will prompt the user to increase the spending limit if sending the 
		/// text would exceed that limit. Should only be true when this method is called from this form. </summary>
		public bool SendText(long patNum,string wirelessPhone,string message,YN txtMsgOk,long clinicNum,SmsMessageSource smsMessageSource,bool canIncreaseLimit=false) {
			if(Plugins.HookMethod(this,"FormTxtMsgEdit.SendText_Start",patNum,wirelessPhone,message,txtMsgOk)) {
				return false;
			}
			if(Plugins.HookMethod(this,"FormTxtMsgEdit.SendText_Start2",patNum,wirelessPhone,message,txtMsgOk)) {
				return true;
			}
			if(wirelessPhone=="") {
				MsgBox.Show(this,"Please enter a phone number.");
				return false;
			}
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				if(!PrefC.HasClinicsEnabled && PrefC.GetDateT(PrefName.SmsContractDate).Year<1880) { //Checking for practice (clinics turned off).
					MsgBox.Show(this,"Integrated Texting has not been enabled.");
					return false;
				}
				else if(PrefC.HasClinicsEnabled && !Clinics.IsTextingEnabled(clinicNum)) { //Checking for specific clinic.
					//This is likely to happen a few times per office until they setup texting properly.
					if(clinicNum!=0) {
						MessageBox.Show(Lans.g(this,"Integrated Texting has not been enabled for the following clinic")+":\r\n"+Clinics.GetClinic(clinicNum).Description+".");
					}
					else {
						//Should never happen. This message is precautionary.
						MsgBox.Show(this,"The default texting clinic has not been set.");
					}
					return false;
				}
			}
			else if(!Programs.IsEnabled(ProgramName.CallFire)) {
				MsgBox.Show(this,"CallFire Program Link must be enabled.");
				return false;
			}
			if(patNum!=0 && txtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)){
				MsgBox.Show(this,"It is not OK to text this patient.");
				return false;
			}
			if(patNum!=0 && txtMsgOk==YN.No) {
				MsgBox.Show(this,"It is not OK to text this patient.");
				return false;
			}
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				try {
					SmsToMobiles.SendSmsSingle(patNum,wirelessPhone,message,clinicNum,smsMessageSource,user:Security.CurUser);  //Can pass in 0 as PatNum if no patient selected.
					return true;
				}
				catch(Exception ex) {
					//ProcessSendSmsException handles the spending limit has been reached error, or returns false if the exception is different.
					if(!canIncreaseLimit || !FormEServicesSetup.ProcessSendSmsException(ex)) { 
						MsgBox.Show(this,ex.Message);
					}
					return false;
				}
			}
			else {
			    if(message.Length>160) {//only a limitation for CallFire
				    MsgBox.Show(this,"Text length must be less than 160 characters.");
				    return false;
			    }
				return SendCallFire(patNum,wirelessPhone,message);  //Can pass in 0 as PatNum if no patient selected.
			}
		}

		///<summary>Sends text message to callfire.  If patNum=0 will not create commlog entry.</summary>
		private bool SendCallFire(long patNum,string wirelessPhone,string message) {
			string key=ProgramProperties.GetPropVal(ProgramName.CallFire,"Key From CallFire");
			string msg=wirelessPhone+","+message.Replace(",","");//ph#,msg Commas in msg cause error.
			try {
				CallFireService.SMSService callFire=new CallFireService.SMSService();
				callFire.sendSMSCampaign(
					key,
					new string[] { msg },
					"Open Dental");
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Error sending text message.\r\n\r\n"+ex.Message);
				return false;
			}
			if(patNum==0) {  //No patient selected, do not make commlog.
				return true;
			}
			Commlog commlog=new Commlog();
			commlog.CommDateTime=DateTime.Now;
			commlog.DateTStamp=DateTime.Now;
			commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT);
			commlog.Mode_=CommItemMode.Text;
			commlog.Note=msg;//phone,note
			commlog.PatNum=patNum;
			commlog.SentOrReceived=CommSentOrReceived.Sent;
			commlog.UserNum=Security.CurUser.UserNum;
			commlog.DateTimeEnd=DateTime.Now;
			Commlogs.Insert(commlog);
			SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,commlog.PatNum,"Insert Text Message");
			return true;
		}

		private void butPatFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formP=new FormPatientSelect();
			formP.ShowDialog();
			if(formP.DialogResult!=DialogResult.OK) {
				return;
			}
			Patient patCur=Patients.GetPat(formP.SelectedPatNum);
			PatNum=patCur.PatNum;
			TxtMsgOk=patCur.TxtMsgOk;
			textPatient.Text=patCur.GetNameLF();
			textWirelessPhone.Text=patCur.WirelessPhone;
		}

		private void radioOther_Click(object sender,EventArgs e) {
			if(!textWirelessPhone.ReadOnly) {//The user clicked the "Another Person" radio button multiple times consecutively.
				return;//Leave so that the phone number is not wiped out unnecessarily.
			}
			butPatFind.Enabled=false;
			textPatient.Enabled=false;
			textWirelessPhone.ReadOnly=false;
			textPatient.Text="";
			textWirelessPhone.Text="";
			PatNum=0;
			TxtMsgOk=YN.Unknown;
		}

		private void radioPatient_Click(object sender,EventArgs e) {
			if(textWirelessPhone.ReadOnly) {//The user clicked the "Patient" radio button multiple times consecutively.
				return;//Leave so that the phone number is not wiped out unnecessarily.
			}
			butPatFind.Enabled=true;
			textPatient.Enabled=true;
			textWirelessPhone.ReadOnly=true;
			textPatient.Text="";
			textWirelessPhone.Text="";
			PatNum=0;
			TxtMsgOk=YN.Unknown;
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(textMessage.Text=="") {
				MsgBox.Show(this,"Please enter a message first.");
				return;
			}
			if(radioOther.Checked) {  //No patient selected
				if(textWirelessPhone.Text=="") {
					MsgBox.Show(this,"Please enter a phone number first.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You do not have a patient selected.  If you are sending a message to an existing "
						+"patient you should choose the Patient option and use the find button.  If you proceed no commlog entry "
						+"will be created and any replies to this message will not be automatically associated with any patient.  Continue?")) {
					return;
				}
				long clinicNum= Clinics.ClinicNum;
				if(clinicNum==0) {
					clinicNum=PrefC.GetLong(PrefName.TextingDefaultClinicNum);
				}
				if(!SendText(0,textWirelessPhone.Text,textMessage.Text,YN.Unknown,clinicNum,SmsMessageSource.DirectSms,true)) {  //0 as PatNum to denote no pat specified
					return;//Allow the user to try again.  A message was already shown to the user inside SendText().
				}
			}
			else {  //Patient selected
				if(PatNum==0) {
					MsgBox.Show(this,"You must first select a patient with the find button, or use the Another Person option.");
					return;
				}
				if(textWirelessPhone.Text=="") {
					MsgBox.Show(this,"This patient has no wireless phone entered.  You must add a wireless phone number to their patient account first before "
						+"you can send a text message.");
					return;
				}
				if(!SendText(PatNum,textWirelessPhone.Text,textMessage.Text,TxtMsgOk,SmsPhones.GetClinicNumForTexting(PatNum),SmsMessageSource.DirectSms,true)) {
					return;//Allow the user to try again.  A message was already shown to the user inside SendText().
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}