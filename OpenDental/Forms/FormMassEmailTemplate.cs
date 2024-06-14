using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailTemplate:FormODBase {
		///<summary>When a new template gets inserted in this form, this will be set to the primary key that the template cur receives.</summary>
		public long NewTemplateCurPriKey;
		///<summary>The template passed into this form. Shallow copy so anything that happens in this form will persist.</summary>
		private EmailHostingTemplate _emailHostingTemplate;
		///<summary>Enum to keep track of the changes that happened while editing in the html email editor.</summary>
		private EmailType _emailType;
		///<summary>To save the changes that were made to the html body while in the email editor.</summary>
		private string _htmlText;
		///<summary>Set to false if the template currently being viewed should not be able to be deleted. Templates with TemplateType Birthday should never be deleted</summary>
		private bool _canDeleteTemplate;

		public FormMassEmailTemplate(EmailHostingTemplate emailHostingTemplate,bool canDeleteTemplate=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailHostingTemplate=emailHostingTemplate;
			_canDeleteTemplate=canDeleteTemplate;
		}

		private void FormMassEmailTemplate_Load(object sender,EventArgs e) {
			bool isImportOrCopy=_emailHostingTemplate.IsNew && !string.IsNullOrEmpty(_emailHostingTemplate.BodyHTML);
			//True when a new template is being created off of a pre-existing file (import).
			if(isImportOrCopy && string.IsNullOrEmpty(_emailHostingTemplate.BodyPlainText)) {
				//copy's will already have plain text set. We only want to override if importing html.
				_emailHostingTemplate.BodyPlainText=MarkupEdit.ConvertToPlainText(_emailHostingTemplate.BodyHTML);
			}
			_htmlText=_emailHostingTemplate.BodyHTML??"";
			_emailType=_emailHostingTemplate.EmailTemplateType;
			textTemplateName.Text=_emailHostingTemplate.TemplateName;
			textSubject.Text=_emailHostingTemplate.Subject;
			textboxPlainText.Text=_emailHostingTemplate.BodyPlainText;
			butDeleteTemplate.Visible=CanDeleteTemplate();
		}

		private bool CanDeleteTemplate() {
			if(_emailHostingTemplate.IsNew) {
				return false;
			}
			return _canDeleteTemplate;
		}

		private void butBodyFieldsPlainText_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.User);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			FrmMessageReplacements frmMessageReplacements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplacements.IsSelectionMode=true;
			frmMessageReplacements.MessageReplacementSystemType=MessageReplacementSystemType.MassEmail;
			frmMessageReplacements.ShowDialog();
			if(frmMessageReplacements.IsDialogOK) {
				textboxPlainText.SelectedText=ReplaceReplacementTag(frmMessageReplacements.ReplacementTextSelected);
			}
		}

		private void butEditTemplate_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			if(string.IsNullOrEmpty(_htmlText)) {
				PlaintextToHTML();
			}
			formEmailEdit.IsMassEmail=true;
			formEmailEdit.MarkupText=_htmlText;
			formEmailEdit.IsRawAllowed=true;
			formEmailEdit.IsRaw=_emailType==EmailType.RawHtml;
			formEmailEdit.AreReplacementsAllowed=true;
			if(formEmailEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_htmlText=formEmailEdit.MarkupText;
			if(string.IsNullOrEmpty(_htmlText)) {
				_emailType=EmailType.Regular;
				return;
			}
			if(formEmailEdit.IsRaw) {
				_emailType=EmailType.RawHtml;
				return;
			}
			_emailType=EmailType.Html;//Our special wiki replcement that will turn into html.
		}

		private void butSubjectFields_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.User);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			FrmMessageReplacements frmMessageReplacements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplacements.IsSelectionMode=true;
			frmMessageReplacements.MessageReplacementSystemType=MessageReplacementSystemType.MassEmail;
			frmMessageReplacements.ShowDialog();
			if(frmMessageReplacements.IsDialogOK) {
				textSubject.SelectedText=ReplaceReplacementTag(frmMessageReplacements.ReplacementTextSelected);
				textSubject.Invalidate();
			}
		}

		private string ReplaceReplacementTag(string replacementStr) {
			return replacementStr
					.Replace("[","[{[{ ")
					.Replace("]"," }]}]");
		}

		private bool IsTemplateValid() {
			if(string.IsNullOrEmpty(textTemplateName.Text)) {
				MsgBox.Show(this,"Template must have a name.");
				return false;
			}
			if(string.IsNullOrEmpty(textSubject.Text)) {
				MsgBox.Show(this,"Template must have a subject.");
				return false;
			}
			if(string.IsNullOrEmpty(textboxPlainText.Text)) {
				MsgBox.Show(this,"Template must have plain text body.");
				return false;
			}
			return true;
		}

		private void PlaintextToHTML() {
			//This will make sure that the plaintext gets formatted correctly for the html template.
			_htmlText=textboxPlainText.Text;
			_emailType=EmailType.Html;
		}

		private void butDeleteTemplate_Click(object sender,EventArgs e) {
			//If IsNew, this won't get hit because the button is set to not be visible if tempCur.IsNew=true;
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this template? This cannot be undone.")) {
				return;
			}
			if(_emailHostingTemplate.TemplateId!=0) {
				//Create an API instance with the clinic num for this template.
				IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(_emailHostingTemplate.ClinicNum);
				DeleteTemplateRequest deleteTemplateRequest=new DeleteTemplateRequest();
				deleteTemplateRequest.TemplateNum=_emailHostingTemplate.TemplateId;
				try {
					iAccountApi.DeleteTemplate(deleteTemplateRequest);
				}
				catch(Exception ex) {
					FriendlyException.Show("Failed to delete template. Please try again.",ex);
					return;
				}
			}
			EmailHostingTemplates.Delete(_emailHostingTemplate.EmailHostingTemplateNum);
			DialogResult=DialogResult.OK;
		}

		private bool Save() {
			if(!IsTemplateValid()) {
				return false;
			}
			if(string.IsNullOrEmpty(_htmlText)) {
				PlaintextToHTML();
			}
			string xhtml;//api templates must have the full html text, even if only partial html. Database templates will store partial as plain text. 
			xhtml=_htmlText;
			if(_emailType==EmailType.Html && !string.IsNullOrEmpty(_htmlText)) {
				//This might not work for images, we should consider blocking them or warning them about sending if we detect images
				try {
					xhtml=MarkupEdit.TranslateToXhtml(_htmlText,true,false,true);
				}
				catch(Exception e) {
					e.DoNothing();
					if(!MsgBox.Show(MsgBoxButtons.YesNo,"There was an issue rendering your email.  If you use this template, you may send malformed emails to " +
						"every selected patient. Do you want to continue saving?")) 
					{
						return false;
					}
				}
			}
			List<string> listBodyReplacementsHTML=EmailHostingTemplates.GetListReplacements(xhtml).Distinct().ToList();
			List<string> listBodyReplacementsPlainText=EmailHostingTemplates.GetListReplacements(textboxPlainText.Text).Distinct().ToList();
			bool doBodyReplacementsMatch=listBodyReplacementsHTML.Count==listBodyReplacementsPlainText.Count && listBodyReplacementsHTML.All(x => listBodyReplacementsPlainText.Contains(x));
			//If the template has HTML, we must ensure that the replacements in both HTML and Plain Text exactly match.
			if(!string.IsNullOrEmpty(xhtml) && !doBodyReplacementsMatch) {
				MsgBox.Show("The replacements in the HTML template did not exactly match the replacements in the Plain Text template.");
				return false;
			}
			//Create an API instance with the clinic num for this template.
			IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(_emailHostingTemplate.ClinicNum);
			if(_emailHostingTemplate.IsNew || _emailHostingTemplate.TemplateId==0) {//templates inserted during convert didn't have their Id's set, we need to create them.
				Template templateTemp=new Template();
				templateTemp.TemplateName=textTemplateName.Text;
				templateTemp.TemplateBodyHtml=xhtml;
				templateTemp.TemplateBodyPlainText=textboxPlainText.Text;
				templateTemp.TemplateSubject=textSubject.Text;
				CreateTemplateRequest createTemplateRequest=new CreateTemplateRequest();
				createTemplateRequest.Template=templateTemp;
				try {
					CreateTemplateResponse createTemplateResponse=iAccountApi.CreateTemplate(createTemplateRequest);
					//This is how we can update the template later
					_emailHostingTemplate.TemplateId=createTemplateResponse.TemplateNum;
				}
				catch(Exception e) {
					FriendlyException.Show("Failed to create template. Please verify the Plain Text Body contains no HTML tags and try again.",e);
					return false;
				}
				_emailHostingTemplate.Subject=textSubject.Text;
				_emailHostingTemplate.TemplateName=textTemplateName.Text;
				_emailHostingTemplate.BodyPlainText=textboxPlainText.Text;
				//_htmlText might be wiki html, which is intended to be saved (vs raw html) in case the user wants to modify their template later.
				_emailHostingTemplate.BodyHTML=_htmlText;
				_emailHostingTemplate.EmailTemplateType=_emailType;
				if(_emailHostingTemplate.IsNew) {
					NewTemplateCurPriKey=EmailHostingTemplates.Insert(_emailHostingTemplate);
				}
				else {
					EmailHostingTemplates.Update(_emailHostingTemplate);
				}
				return true;
			}
			//We must update the template with the api before we can update our template.
			Template template=new Template();
			template.TemplateName=textTemplateName.Text;
			template.TemplateBodyHtml=xhtml;
			template.TemplateBodyPlainText=textboxPlainText.Text;
			template.TemplateSubject=textSubject.Text;
			UpdateTemplateRequest updateTemplateRequest=new UpdateTemplateRequest();
			updateTemplateRequest.Template=template;
			updateTemplateRequest.TemplateNum=_emailHostingTemplate.TemplateId;
			try {
				iAccountApi.UpdateTemplate(updateTemplateRequest);
			}
			catch(Exception e) {
				FriendlyException.Show("Failed to update template. Please try again.",e);
				return false;
			}
			_emailHostingTemplate.Subject=textSubject.Text;
			_emailHostingTemplate.TemplateName=textTemplateName.Text;
			_emailHostingTemplate.BodyPlainText=textboxPlainText.Text;
			_emailHostingTemplate.BodyHTML=_htmlText;
			_emailHostingTemplate.EmailTemplateType=_emailType;
			EmailHostingTemplates.Update(_emailHostingTemplate);
			return true;//save successful
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(Save()) {
				DialogResult=DialogResult.OK;
			}
		}

		private void FormMassEmailTemplate_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.Cancel) {
				return;
			}
			bool isImportOrCopy=_emailHostingTemplate.IsNew && !string.IsNullOrEmpty(_emailHostingTemplate.BodyHTML);
			//True when a new template is being created off of a pre-existing file (import).
			if(isImportOrCopy  &&
				!MsgBox.Show(this,MsgBoxButtons.YesNo,"Canceling will cause a loss of all work. Do you want to continue?")) 
			{
				return;
			}
		}

	}
}