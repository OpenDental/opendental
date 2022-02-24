using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebSchedASAPSend:FormODBase {
		private readonly long _clinicNum;
		private readonly long _opNum;
		private readonly DateTime _dtSlotStart;
		private readonly DateTime _dtSlotEnd;
		private readonly List<Appointment> _listAppts;
		private readonly List<Recall> _listRecalls;
		private List<PatComm> _listPatComms;
		private AsapComms.AsapListSender _asapListSender;
		private string _emailText="";
		private bool _isTemplateRawHtml;

		public FormWebSchedASAPSend(long clinicNum,long opNum,DateTime dtSlotStart,DateTime dtSlotEnd,List<Appointment> listAppts,List<Recall> listRecalls) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
			_opNum=opNum;
			_dtSlotStart=dtSlotStart;
			_dtSlotEnd=dtSlotEnd;
			_listAppts=listAppts;
			_listRecalls=listRecalls;
		}

		private void FormWebSchedASAPSend_Load(object sender,EventArgs e) {
			Clinic curClinic=Clinics.GetClinic(_clinicNum)??Clinics.GetDefaultForTexting()??Clinics.GetPracticeAsClinicZero();
			List<long> listPatNums=(_listAppts.Select(x => x.PatNum).Union(_listRecalls.Select(x => x.PatNum))).Distinct().ToList();
			_listPatComms=Patients.GetPatComms(listPatNums,curClinic,isGetFamily: false);
			string textTemplate=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapTextTemplate,_clinicNum);
			string emailTemplate=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapEmailTemplate,_clinicNum);
			string emailSubject=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapEmailSubj,_clinicNum);
			textTextTemplate.Text=AsapComms.ReplacesTemplateTags(textTemplate,_clinicNum,_dtSlotStart);
			_emailText=AsapComms.ReplacesTemplateTags(emailTemplate,_clinicNum,_dtSlotStart,isHtmlEmail:true);
			RefreshEmail();
			textEmailSubject.Text=AsapComms.ReplacesTemplateTags(emailSubject,_clinicNum,_dtSlotStart);
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
			_asapListSender=AsapComms.CreateSendList(_listAppts,_listRecalls,_listPatComms,GetSendMode(),textTextTemplate.Text,_emailText,
				textEmailSubject.Text,_dtSlotStart,DateTime.Now,_clinicNum,_isTemplateRawHtml);
			int countTexts=_asapListSender.CountTextsToSend;
			int countEmails=_asapListSender.CountEmailsToSend;
			gridSendDetails.BeginUpdate();
			gridSendDetails.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),120);
			gridSendDetails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Sending Text"),100,HorizontalAlignment.Center);
			gridSendDetails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Sending Email"),100,HorizontalAlignment.Center);
			gridSendDetails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),150);
			gridSendDetails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),300);
			gridSendDetails.ListGridColumns.Add(col);
			gridSendDetails.ListGridRows.Clear();
			foreach(AsapComm comm in _asapListSender.ListAsapComms) {
				GridRow row=new GridRow();
				AsapComms.AsapListSender.PatientDetail patDetail=_asapListSender.ListDetails.First(x => x.PatNum==comm.PatNum);
				row.Cells.Add(patDetail.PatName);
				row.Cells.Add(patDetail.IsSendingText ? "X" : "");
				row.Cells.Add(patDetail.IsSendingEmail ? "X" : "");
				row.Cells.Add(Lan.g(this,Enum.GetName(typeof(AsapCommFKeyType),comm.FKeyType)));
				row.Cells.Add(patDetail.Note);
				row.Tag=patDetail;
				gridSendDetails.ListGridRows.Add(row);
			}
			gridSendDetails.SortForced(0,false);
			gridSendDetails.EndUpdate();
			if(countTexts==1) {
				labelAnticipated.Text+=countTexts+" "+Lan.g(this,"text will be sent at")+" "
					+_asapListSender.DtStartSendText.ToShortTimeString()+".\r\n";
			}
			else if(countTexts > 1) {
				int minutesBetweenTexts=_asapListSender.MinutesBetweenTexts;
				labelAnticipated.Text+=countTexts+" "+Lan.g(this,"texts will be sent starting at")+" "
					+_asapListSender.DtStartSendText.ToShortTimeString()+" "+Lan.g(this,"with")+" "+minutesBetweenTexts+" "
					+Lan.g(this,"minute"+(minutesBetweenTexts==1 ? "" : "s")+" between each text")+".\r\n";
			}
			if(GetSendMode()!=AsapComms.SendMode.Email && _asapListSender.IsOutsideSendWindow) {
				labelAnticipated.Text+=Lan.g(this,"Because it is currently outside the automatic send window, texts will not start sending until")+" "
					+_asapListSender.DtStartSendText.ToString()+".\r\n";
			}
			int countTextToSendAtEndTime=_asapListSender.ListAsapComms.Count(x => x.DateTimeSmsScheduled==_asapListSender.DtTextSendEnd);
			if(PrefC.DoRestrictAutoSendWindow && countTextToSendAtEndTime > 1) {
				labelAnticipated.Text+=Lan.g(this,"In order to not send texts outside the automatic send window,")+" "+countTextToSendAtEndTime
					+" "+Lan.g(this,"texts will be sent at")+" "+_asapListSender.DtTextSendEnd.ToString()+".\r\n";
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
			else if(radioText.Checked) {
				return AsapComms.SendMode.Text;
			}
			else if(radioEmail.Checked) {
				return AsapComms.SendMode.Email;
			}
			else {
				return AsapComms.SendMode.PreferredContact;
			}
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

		private void butOK_Click(object sender,EventArgs e) {
			FillSendDetails();
			AsapComms.InsertForSending(_asapListSender.ListAsapComms,_dtSlotStart,_dtSlotEnd,_opNum);
			string message=_asapListSender.CountTextsToSend+" "+Lan.g(this,"text"+(_asapListSender.CountTextsToSend==1?"":"s")+" and")+" "
				+_asapListSender.CountEmailsToSend+" "+Lan.g(this,"email"+(_asapListSender.CountEmailsToSend==1?"":"s")+" have been entered to be sent.");
			PopupFade.Show(this,message);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormWebSchedASAPSend_FormClosing(object sender,FormClosingEventArgs e) {
			timerUpdateDetails.Stop();
		}

	}
}