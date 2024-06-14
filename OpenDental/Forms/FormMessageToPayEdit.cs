using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness;
using OpenDentBusiness.AutoComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMessageToPayEdit:FormODBase {
		private Patient _patient;
		private Clinic _clinic;
		private MsgToPayEmailTemplate _msgToPayEmailTemplate;
		private MsgToPayTagReplacer _msgToPayTagReplacer;

		public FormMessageToPayEdit(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=Patients.GetGuarForPat(patNum);
			_clinic=Clinics.GetClinic(_patient.ClinicNum);
			//Do not replace the Statement Balance tag yet. Tag is replaced by the most recent statement which gets generated after we insert this message into the database.
			_msgToPayTagReplacer=new MsgToPayTagReplacer(replaceStatmentBalance:false);
		}

		private void FormMessageToPayEdit_Load(object sender,EventArgs e) {
			bool isTextingEnabled=SmsPhones.IsIntegratedTextingEnabled();
			radioText.Enabled=isTextingEnabled;
			labelTextingSignup.Visible=!isTextingEnabled;
			textPatient.Text=_patient.LName+", "+_patient.FName;
			radioText.Checked=isTextingEnabled;
			radioEmail.Checked=!isTextingEnabled;
			LoadMessageTemplates();
		}

		private void RefreshEmail() {
			if(_msgToPayEmailTemplate.EmailType==EmailType.RawHtml) {
				browserEmail.DocumentText=_msgToPayEmailTemplate.Template;
			}
			else {
				try {
					string text=MarkupEdit.TranslateToXhtml(_msgToPayEmailTemplate.Template,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true,scale:LayoutManager.ScaleMyFont());
					MsgToPayLite msgToPayLite=new MsgToPayLite(_patient);
					text=_msgToPayTagReplacer.ReplaceTags(text,msgToPayLite,_clinic,false);
					browserEmail.DocumentText=text;
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}

		private void LoadMessageTemplates() {
			textSubject.Text=PrefC.GetString(PrefName.PaymentPortalMsgToPaySubjectTemplate);
			_msgToPayEmailTemplate=JsonConvert.DeserializeObject<MsgToPayEmailTemplate>(PrefC.GetString(PrefName.PaymentPortalMsgToPayEmailMessageTemplate));
			if(_msgToPayEmailTemplate==null) {
				_msgToPayEmailTemplate=new MsgToPayEmailTemplate();
				_msgToPayEmailTemplate.Template="";
			}
			string template=PrefC.GetString(PrefName.PaymentPortalMsgToPayTextMessageTemplate);
			MsgToPayLite msgToPayLite=new MsgToPayLite(_patient);
			template=_msgToPayTagReplacer.ReplaceTags(template,msgToPayLite,_clinic,false);
			textMessage.Text=template;
			RefreshEmail();
		}

		private bool ValidateSend() {
			List<string> listErrors=new List<string>();
			//One option must be selected
			if(!radioEmail.Checked && !radioText.Checked) {
				listErrors.Add(Lan.g(this,"A message type must be selected."));
			}
			if(radioText.Checked) {
				listErrors.AddRange(ValidateSMS());
			}
			else {
				listErrors.AddRange(ValidateEmail());
			}
			//Display errors if any
			if(!listErrors.IsNullOrEmpty()) {
				MessageBox.Show(Lan.g(this,"Please fix the following before continuing:")+"\r\n- "+string.Join("\r\n- ",listErrors));
			}
			return listErrors.IsNullOrEmpty();
		}

		private List<string> ValidateEmail() {
			List<string> listErrors=new List<string>();
			if(!browserEmail.DocumentText.Contains(MsgToPaySents.MSG_TO_PAY_TAG)) {
				listErrors.Add(Lan.g(this,"Email Message Text must contain")+" '"+MsgToPaySents.MSG_TO_PAY_TAG+"'.");
			}
			if(string.IsNullOrWhiteSpace(textSubject.Text)) {
				listErrors.Add(Lan.g(this,"Email Subject cannot be empty."));
			}
			return listErrors;
		}

		private List<string> ValidateSMS() {
			List<string> listErrors=new List<string>();
			//Message must contain Text to pay tag.
			if(!textMessage.Text.Contains(MsgToPaySents.MSG_TO_PAY_TAG)) {
				listErrors.Add(Lan.g(this,"SMS Message Text must contain")+" '"+MsgToPaySents.MSG_TO_PAY_TAG+"'.");
			}
			return listErrors;
		}

		private bool CanSendSms() {
			return PatientL.CheckPatientTextingAllowed(_patient,this);
		}

		private bool CanSendEmail() {
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(_patient.PatNum),_clinic).FirstOrDefault();
			return patComm?.IsEmailAnOption??false;
		}

		private void radioMessageType_CheckedChanged(object sender,EventArgs e) {
			CheckChangedControls();
		}

		private void CheckChangedControls() {
			bool isEmail=radioEmail.Checked;
			textSubject.Enabled=isEmail;
			browserEmail.Visible=isEmail;
			butEdit.Visible=isEmail;
			textMessage.Visible=!isEmail;
		}

		private void butEdit_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.MarkupText=_msgToPayEmailTemplate.Template;
			formEmailEdit.IsRawAllowed=true;
			formEmailEdit.IsRaw=_msgToPayEmailTemplate.EmailType==EmailType.RawHtml;
			formEmailEdit.DoCheckForDisclaimer=false;
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			EmailType emailType=EmailType.Regular;
			if(formEmailEdit.IsRaw) {
				emailType=EmailType.RawHtml;
			}
			_msgToPayEmailTemplate.EmailType=emailType;
			_msgToPayEmailTemplate.Template=formEmailEdit.MarkupText;
			RefreshEmail();
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!ValidateSend()) {
				return;
			}
			CommType commType=CommType.Text;
			if(radioEmail.Checked) {
				commType=CommType.Email;
			}
			//SMS
			if(commType==CommType.Text) {
				if(!CanSendSms()) {
					return;
				}
			}
			//Email
			else {
				if(!CanSendEmail()) {
					MsgBox.Show(this,"Email is not an option for this patient.");
					return;
				}
			}
			//Create a regular Statement here. We're passing in MinVal for date start and today for date end on purpose because there is no UI
			Statement statement=Statements.GenerateStatement(_patient,DateTime.MinValue,DateTime.Today,StatementMode.Electronic);
			Statements.Insert(statement);
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			List<Def> listDefsForImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			Documents.CreateAndSaveStatementPDF(statement,sheetDef,isLimitedCustom:false,showLName:false,excludeTxfr:false,listDefsForImageCats);
			//Create text to pay here
			MsgToPaySent msgToPaySent=MsgToPaySents.CreateFromPat(_patient);
			msgToPaySent.MessageType=commType;
			msgToPaySent.Message=textMessage.Text;
			msgToPaySent.Subject="";
			if(radioEmail.Checked) {
				msgToPaySent.Message=_msgToPayEmailTemplate.Template;
				msgToPaySent.EmailType=_msgToPayEmailTemplate.EmailType;
				msgToPaySent.Subject=textSubject.Text;
			}
			MsgToPaySents.Insert(msgToPaySent);
			MessageBox.Show(Lan.g(this,"Your")+$" {commType} "+Lan.g(this,"has been sent"));
			DialogResult=DialogResult.OK;
		}
	}
}