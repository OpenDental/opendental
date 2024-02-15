using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebSchedASAPSend:FormODBase {
		private long _clinicNum;
		private long _operatoryNum;
		private DateTime _dateTimeSlotStart;
		private DateTime _dateTimeSlotEnd;
		private List<Appointment> _listAppointments;
		private List<Recall> _listRecalls;
		private List<PatComm> _listPatComms;
		private AsapComms.AsapListSender _asapListSender;
		private string _emailText="";
		private bool _isTemplateRawHtml;

		public FormWebSchedASAPSend(long clinicNum,long operatoryNum,DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd,List<Appointment> listAppts,List<Recall> listRecalls) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
			_operatoryNum=operatoryNum;
			_dateTimeSlotStart=dateTimeSlotStart;
			_dateTimeSlotEnd=dateTimeSlotEnd;
			_listAppointments=listAppts;
			_listRecalls=listRecalls;
		}

		private void FormWebSchedASAPSend_Load(object sender,EventArgs e) {
			Clinic clinic=Clinics.GetClinic(_clinicNum)??Clinics.GetDefaultForTexting()??Clinics.GetPracticeAsClinicZero();
			List<long> listPatNums=(_listAppointments.Select(x => x.PatNum).Union(_listRecalls.Select(x => x.PatNum))).Distinct().ToList();
			_listPatComms=Patients.GetPatComms(listPatNums,clinic,isGetFamily: false);
			string textTemplate=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapTextTemplate,_clinicNum);
			string emailTemplate=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapEmailTemplate,_clinicNum);
			string emailSubject=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapEmailSubj,_clinicNum);
			textTextTemplate.Text=AsapComms.ReplacesTemplateTags(textTemplate,_clinicNum,_dateTimeSlotStart);
			_emailText=AsapComms.ReplacesTemplateTags(emailTemplate,_clinicNum,_dateTimeSlotStart,isHtmlEmail:true);
			RefreshEmail();
			textEmailSubject.Text=AsapComms.ReplacesTemplateTags(emailSubject,_clinicNum,_dateTimeSlotStart);
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				radioTextEmail.Checked=true;
			}
			else {
				radioEmail.Checked=true;
			}
			_isTemplateRawHtml=PIn.Enum<EmailType>(ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapEmailTemplateType,_clinicNum))==EmailType.RawHtml;
			FillSendDetails();
			timerUpdateDetails.Start();
		}

		private void RefreshEmail() {
			if(_isTemplateRawHtml) {
				browserEmailText.DocumentText=_emailText;
				return;
			}
			ODException.SwallowAnyException(() => {
				string text=MarkupEdit.TranslateToXhtml(_emailText,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true);
				browserEmailText.DocumentText=text;
			});
		}

		private void FillSendDetails() {
			labelAnticipated.Text="";
			_asapListSender=AsapComms.CreateSendList(_listAppointments,_listRecalls,_listPatComms,GetSendMode(),textTextTemplate.Text,_emailText,
				textEmailSubject.Text,_dateTimeSlotStart,DateTime.Now,_clinicNum,_isTemplateRawHtml);
			int countTexts=_asapListSender.CountTextsToSend;
			int countEmails=_asapListSender.CountEmailsToSend;
			gridSendDetails.BeginUpdate();
			gridSendDetails.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),120);
			gridSendDetails.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Sending Text"),100,HorizontalAlignment.Center);
			gridSendDetails.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Sending Email"),100,HorizontalAlignment.Center);
			gridSendDetails.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),150);
			gridSendDetails.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),300);
			gridSendDetails.Columns.Add(col);
			gridSendDetails.ListGridRows.Clear();
			for(int i=0;i<_asapListSender.ListAsapComms.Count;i++) { 
				GridRow row=new GridRow();
				AsapComms.AsapListSender.PatientDetail patDetail=_asapListSender.GetListPatientDetails().First(x => x.PatNum==_asapListSender.ListAsapComms[i].PatNum);
				row.Cells.Add(patDetail.PatName);
				row.Cells.Add(patDetail.IsSendingText ? "X" : "");
				row.Cells.Add(patDetail.IsSendingEmail ? "X" : "");
				row.Cells.Add(Lan.g(this,Enum.GetName(typeof(AsapCommFKeyType),_asapListSender.ListAsapComms[i].FKeyType)));
				row.Cells.Add(patDetail.Note);
				row.Tag=patDetail;
				gridSendDetails.ListGridRows.Add(row);
			}
			gridSendDetails.SortForced(0,false);
			gridSendDetails.EndUpdate();
			if(countTexts==1) {
				labelAnticipated.Text+=countTexts+" "+Lan.g(this,"text will be sent at")+" "
					+_asapListSender.DateTimeStartSendText.ToShortTimeString()+".\r\n";
			}
			else if(countTexts > 1) {
				int minutesBetweenTexts=_asapListSender.MinutesBetweenTexts;
				labelAnticipated.Text+=countTexts+" "+Lan.g(this,"texts will be sent starting at")+" "
					+_asapListSender.DateTimeStartSendText.ToShortTimeString()+" "+Lan.g(this,"with")+" "+minutesBetweenTexts+" "
					+Lan.g(this,"minute"+(minutesBetweenTexts==1 ? "" : "s")+" between each text")+".\r\n";
			}
			if(GetSendMode()!=AsapComms.SendMode.Email && _asapListSender.IsOutsideSendWindow) {
				labelAnticipated.Text+=Lan.g(this,"Because it is currently outside the automatic send window, texts will not start sending until")+" "
					+_asapListSender.DateTimeStartSendText.ToString()+".\r\n";
			}
			int countTextToSendAtEndTime=_asapListSender.ListAsapComms.Count(x => x.DateTimeSmsScheduled==_asapListSender.DateTimeTextSendEnd);
			if(PrefC.DoRestrictAutoSendWindow && countTextToSendAtEndTime > 1) {
				labelAnticipated.Text+=Lan.g(this,"In order to not send texts outside the automatic send window,")+" "+countTextToSendAtEndTime
					+" "+Lan.g(this,"texts will be sent at")+" "+_asapListSender.DateTimeTextSendEnd.ToString()+".\r\n";
			}
			if(countEmails > 0) {
				labelAnticipated.Text+=countEmails+" "+Lan.g(this,"email"+(countEmails==1 ? "" : "s")+" will be sent upon clicking Send.");
			}
			if(countTexts==0 && countEmails==0) {
				labelAnticipated.Text+=Lan.g(this,"No patients selected are able to receive communication using this send method.");
			}
		}

		private void timerUpdateDetails_Tick(object sender,EventArgs e) {
			FillSendDetails();
		}

		private void radio_CheckedChanged(object sender,EventArgs e) {
			textTextTemplate.Enabled=(!radioEmail.Checked);
			butEditEmail.Enabled=(!radioText.Checked);
			textEmailSubject.Enabled=(!radioText.Checked);
			FillSendDetails();
		}

		private AsapComms.SendMode GetSendMode() {
			if(radioTextEmail.Checked) {
				return AsapComms.SendMode.TextAndEmail;
			}
			if(radioText.Checked) {
				return AsapComms.SendMode.Text;
			}
			if(radioEmail.Checked) {
				return AsapComms.SendMode.Email;
			}
			return AsapComms.SendMode.PreferredContact;
		}

		private void butEditEmail_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.MarkupText=_emailText;
			formEmailEdit.DoCheckForDisclaimer=true;
			formEmailEdit.IsRawAllowed=true;
			formEmailEdit.IsRaw=_isTemplateRawHtml;
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_isTemplateRawHtml=formEmailEdit.IsRaw;
			_emailText=formEmailEdit.MarkupText;
			RefreshEmail();
		}

		private void butSend_Click(object sender,EventArgs e) {
			FillSendDetails();
			AsapComms.InsertForSending(_asapListSender.ListAsapComms,_dateTimeSlotStart,_dateTimeSlotEnd,_operatoryNum);
			string message=_asapListSender.CountTextsToSend+" "+Lan.g(this,"text"+(_asapListSender.CountTextsToSend==1?"":"s")+" and")+" "
				+_asapListSender.CountEmailsToSend+" "+Lan.g(this,"email"+(_asapListSender.CountEmailsToSend==1?"":"s")+" have been entered to be sent.");
			FormPopupFade formPopupFade=new FormPopupFade(message);
			formPopupFade.Show();
			DialogResult=DialogResult.OK;
		}

		private void FormWebSchedASAPSend_FormClosing(object sender,FormClosingEventArgs e) {
			timerUpdateDetails.Stop();
		}

	}
}