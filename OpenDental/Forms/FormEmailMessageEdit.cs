using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	public partial class FormEmailMessageEdit : FormODBase {
		public bool IsNew;
    private bool _hasTemplatesChanged;
		///<summary>A copy of the message passed into the constructor.</summary>
		private EmailMessage _emailMessage;
    private bool _isDeleteAllowed=true;
    public bool HasEmailChanged;
		///<summary>List of email messages to be considered for auto complete email address popup. When null will run query.</summary>
		private List<EmailMessage> _listAllEmailMessages=null;
		private List<EmailTemplate> _listEmailTemplates;
		private bool _isRawHtml;

		///<summary>isDeleteAllowed defines whether the email is able to be deleted when a patient is attached. 
		///emailAddress corresponds to the account in Email Setup that will be used to send the email.
		///Currently, emails that are "Deleted" from the inbox are actually just hidden if they have a patient attached.</summary>
		public FormEmailMessageEdit(EmailMessage emailMessage,EmailAddress emailAddress=null,bool isDeleteAllowed=true
			,params List<EmailMessage>[] listAllEmailMessages)
		{
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
      _isDeleteAllowed=isDeleteAllowed;
      _emailMessage=emailMessage.Copy();
			if(emailAddress==null) {
				Patient pat=Patients.GetPat(_emailMessage.PatNum);
				emailPreview.LoadEmailAddresses(pat.ClinicNum);
				FromAddressMatchResult result=emailPreview.TryGetFromEmailAddress(out emailAddress,_emailMessage.FromAddress);
				switch(result) {
					case FromAddressMatchResult.Failed:
						//emailAddress is still null. User will be prompted to make a valid emailAddress selection when trying to send email.
						break;
					case FromAddressMatchResult.Success:
						//emailAddress set succesfully
						break;
					case FromAddressMatchResult.Multi:
						//TryGetFromEmailAddress sets emailAddress to first match, but we don't want to lock this in until Sending the email, where we will 
						//confirm via user prompt. Form can still load with emailAddress set to null.
						emailAddress=null;
						break;
				}
			}
			emailPreview.EmailAddressPreview=emailAddress;
			List<List<EmailMessage>> listAllHistoric=listAllEmailMessages.ToList().FindAll(x => x!=null);
			if(listAllHistoric.Count>0) {
				_listAllEmailMessages=listAllHistoric.SelectMany(x => x).Where(x => x!=null).ToList();
			}
		}

		private void FormEmailMessageEdit_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)) {
				butAddTemplate.Enabled=false;
				butDeleteTemplate.Enabled=false;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				butDirectMessage.Visible=true;
			}
			_isRawHtml=_emailMessage.HtmlType==EmailType.RawHtml;
			Cursor=Cursors.WaitCursor;
			RefreshAll();
			SetDefaultAutograph();
			EmailSaveEvent.Fired+=EmailSaveEvent_Fired;
			Cursor=Cursors.Default;
			Plugins.HookAddCode(this,"FormEmailMessageEdit_Load_end",_emailMessage,emailPreview);
		}

		private void EmailSaveEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.EmailSave) {
				return;
			}
			//save email
			SaveMsg();//I think this is all we need
		}

		private void RefreshAll() {
			_emailMessage.IsNew=IsNew;
			emailPreview.LoadEmailMessage(_emailMessage,_listAllEmailMessages);
			if(!emailPreview.IsComposing) {
				panelTemplates.Visible=false;
				panelAutographs.Visible=false;
				butDirectMessage.Enabled=false;//not allowed to send again.
				butSend.Visible=false;//not allowed to send again.
				butSave.Visible=false;//not allowed to save changes.
				//When opening an email from FormEmailInbox, the email status will change to read automatically,
				//and changing the text on the cancel button helps convey that to the user.
				butCancel.Text="Close";
				butEditText.Visible=false;
				butEditHtml.Visible=false;
			}
			FillTemplates();
			FillAutographs();
			butRefresh.Visible=false;
			//For all email received types, we disable most of the controls and put the form into a mostly read-only state.
			if(_emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedEncrypted ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedDirect ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.ReadDirect ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.Received ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.Read ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.WebMailReceived ||
				_emailMessage.SentOrReceived==EmailSentOrReceived.WebMailRecdRead)
			{
				butRefresh.Visible=true;
				butRawMessage.Visible=true;
        butSend.Visible=true;
        butSend.Text=Lan.g(this,"Reply");
      }
			labelDecrypt.Visible=false;
			butDecrypt.Visible=false;
			if(_emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedEncrypted) {
				labelDecrypt.Visible=true;
				butDecrypt.Visible=true;
				butRefresh.Visible=false;
			}
		}

		#region Templates

		private void FillTemplates() {
			listTemplates.Items.Clear();
			_listEmailTemplates=EmailTemplates.GetDeepCopy();
			for(int i=0;i<_listEmailTemplates.Count;i++) {
				listTemplates.Items.Add(_listEmailTemplates[i].Description);
			}
		}

		private void listTemplates_DoubleClick(object sender, System.EventArgs e) {
			if(listTemplates.SelectedIndex==-1){
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormEmailTemplateEdit FormE=new FormEmailTemplateEdit();
			FormE.ETcur=_listEmailTemplates[listTemplates.SelectedIndex];
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			EmailTemplates.RefreshCache();
			_hasTemplatesChanged=true;
			FillTemplates();
		}

		private void butAddTemplate_Click(object sender, System.EventArgs e) {
			using FormEmailTemplateEdit FormE=new FormEmailTemplateEdit();
			FormE.IsNew=true;
			FormE.ETcur=new EmailTemplate();
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			EmailTemplates.RefreshCache();
			_hasTemplatesChanged=true;
			FillTemplates();
		}

		private void butDeleteTemplate_Click(object sender, System.EventArgs e) {
			if(listTemplates.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete e-mail template?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			EmailTemplates.Delete(_listEmailTemplates[listTemplates.SelectedIndex]);
			EmailTemplates.RefreshCache();
			_hasTemplatesChanged=true;
			FillTemplates();
		}

		private void butInsertTemplate_Click(object sender, System.EventArgs e) {
			if(listTemplates.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(emailPreview.BodyText!="" || emailPreview.Subject!="" || emailPreview.HasAttachments){
				if(MessageBox.Show(Lan.g(this,"Replace existing e-mail text with text from the template?  Existing attachments will not be deleted.")
					,"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			ChangeViewToPlainText();//reset the view to plaintext, to show that body text was replaced.
			List<EmailTemplate> listEmailTemplates=_listEmailTemplates;
			List<EmailAttach> listAttachments=EmailAttaches.GetForTemplate(listEmailTemplates[listTemplates.SelectedIndex].EmailTemplateNum);
			listAttachments.ForEach(x => x.EmailTemplateNum=0); //Unattach the emailattachments from the email template.
			EmailTemplate templateCur=listEmailTemplates[listTemplates.SelectedIndex];
			if(EmailMessages.IsHtmlEmail(templateCur.TemplateType)) {
				//this is an html template. Translate and then set the view to html.
				emailPreview.LoadTemplate(listEmailTemplates[listTemplates.SelectedIndex].Subject,templateCur.BodyText,listAttachments);
				emailPreview.HtmlText=emailPreview.BodyText;
				emailPreview.BodyText=emailPreview.BodyText;
				_isRawHtml=templateCur.TemplateType==EmailType.RawHtml;
				ChangeViewToHtml(_isRawHtml);
			}
			else {
				//regular template. 
				emailPreview.LoadTemplate(templateCur.Subject,templateCur.BodyText,listAttachments);
			}
		}

		private void butEditTemplate_Click(object sender,EventArgs e) {
			if(listTemplates.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormEmailTemplateEdit FormE=new FormEmailTemplateEdit();
			FormE.ETcur=_listEmailTemplates[listTemplates.SelectedIndex];
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK) {
				return;
			}
			EmailTemplates.RefreshCache();
			_hasTemplatesChanged=true;
			FillTemplates();
		}

		private void listTemplates_MouseMove(object sender,MouseEventArgs e) {
			int itemIdx=listTemplates.IndexFromPoint(e.Location);
			if(itemIdx==-1) {
				toolTipMessage.Hide(listTemplates);
				return;
			}
			string description=listTemplates.Items[itemIdx].ToString();
			if(toolTipMessage.GetToolTip(listTemplates).Contains(description)) {
				return;
			}
			toolTipMessage.SetToolTip(listTemplates,description);
		}

		#endregion Templates

		#region Autographs
		/// <summary>Fills the autographs picklist.</summary>
		private void FillAutographs() {
			listAutographs.Items.Clear();
			listAutographs.Items.AddList(EmailAutographs.GetDeepCopy(),x => x.Description);
		}

		///<summary>Sets the default autograph that shows in the message body. 
		///The default autograph is determined to be the first autograph with an email that matches the email address of the sender.</summary>
		private void SetDefaultAutograph() {
			if(!emailPreview.IsComposing || !IsNew) {
				return;
			}
			EmailAddress emailOutgoing=null;
			if(emailPreview.TryGetFromEmailAddress(out emailOutgoing)==FromAddressMatchResult.Failed) {
				return;
			}
			string emailUserName=EmailMessages.GetAddressSimple(emailOutgoing.EmailUsername);
			string emailSender=EmailMessages.GetAddressSimple(emailOutgoing.SenderAddress);
			string autographEmail;
			for(int i=0;i<listAutographs.Items.Count;i++) {
				autographEmail=EmailMessages.GetAddressSimple(((EmailAutograph)listAutographs.Items.GetObjectAt(i)).EmailAddress.Trim());
				//Use Contains() because an autograph can theoretically have multiple email addresses associated with it.
				if((!string.IsNullOrWhiteSpace(emailUserName) && autographEmail.Contains(emailUserName)) 
					|| (!string.IsNullOrWhiteSpace(emailSender) && autographEmail.Contains(emailSender)))
				{
					InsertAutograph((EmailAutograph)listAutographs.Items.GetObjectAt(i));
					break;
				}
			}
		}
		
		///<summary>Currently just appends an autograph to the bottom of the email message.  When the functionality to reply to emails is implemented, 
		///this will need to be modified so that it inserts the autograph text at the bottom of the new message being composed, but above the message
		///history.</summary>
		private void InsertAutograph(EmailAutograph emailAutograph) {
			emailPreview.BodyText+="\r\n\r\n"+emailAutograph.AutographText;
		}
		
		private void listAutographs_DoubleClick(object sender,EventArgs e) { //edit an autograph
			if(listAutographs.SelectedIndex==-1) {
				return;
			}
			using FormEmailAutographEdit FormEAE=new FormEmailAutographEdit(listAutographs.GetSelected<EmailAutograph>());
			FormEAE.ShowDialog();
			if(FormEAE.DialogResult==DialogResult.OK) {
				EmailAutographs.RefreshCache();
				FillAutographs();
			}
		}

		private void butAddAutograph_Click(object sender,EventArgs e) { //add a new autograph
			EmailAutograph emailAutograph=new EmailAutograph();
			using FormEmailAutographEdit FormEAE=new FormEmailAutographEdit(emailAutograph);
			FormEAE.IsNew=true;
			FormEAE.ShowDialog();
			if(FormEAE.DialogResult==DialogResult.OK) {
				EmailAutographs.RefreshCache();
				FillAutographs();
			}
		}

		private void butInsertAutograph_Click(object sender,EventArgs e) {
			if(listAutographs.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select an autograph before inserting."));
				return;
			}
			if(emailPreview.IsHtml) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Autographs must be inserted from the Edit HTML window using the autograph dropdown in the toolbar."
					+"\r\n\r\nWould you like to open the Edit HTML window?")) 
				{
					OpenEditHtmlWindow();
				}
				return;
			}
			ChangeViewToPlainText();//reset the view to the plain text so the autograph can be appended.
			InsertAutograph(listAutographs.GetSelected<EmailAutograph>());
		}
		
		private void butDeleteAutograph_Click(object sender,EventArgs e) {
			if(listAutographs.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete autograph?"),"",MessageBoxButtons.OKCancel) != DialogResult.OK) {
				return;
			}
			EmailAutographs.Delete(listAutographs.GetSelected<EmailAutograph>().EmailAutographNum);
			EmailAutographs.RefreshCache();
			FillAutographs();
		}

		private void butEditAutograph_Click(object sender,EventArgs e) {
			if(listAutographs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			using FormEmailAutographEdit FormEAE=new FormEmailAutographEdit(listAutographs.GetSelected<EmailAutograph>());
			FormEAE.ShowDialog();
			if(FormEAE.DialogResult==DialogResult.OK) {
				EmailAutographs.RefreshCache();
				FillAutographs();
			}
		}

		#endregion

		private void butDecrypt_Click(object sender,EventArgs e) {
			if(EmailMessages.GetReceiverUntrustedCount(_emailMessage.FromAddress) >= 0) {//Not trusted yet.
				string strTrustMessage=Lan.g(this,"The sender address must be added to your trusted addresses before you can decrypt the email")
					+". "+Lan.g(this,"Add")+" "+_emailMessage.FromAddress+" "+Lan.g(this,"to trusted addresses")+"?";
				if(MessageBox.Show(strTrustMessage,"",MessageBoxButtons.OKCancel)==DialogResult.OK) {
					Cursor=Cursors.WaitCursor;
					EmailMessages.TryAddTrustDirect(_emailMessage.FromAddress);
					Cursor=Cursors.Default;
					if(EmailMessages.GetReceiverUntrustedCount(_emailMessage.FromAddress) >= 0) {
						MsgBox.Show(this,"Failed to trust sender because a valid certificate could not be located.");
						return;
					}
				}
				else {
					MsgBox.Show(this,"Cannot decrypt message from untrusted sender.");
					return;
				}
			}
			Cursor=Cursors.WaitCursor;
			EmailAddress emailAddress=emailPreview.EmailAddressPreview;
			try {
				_emailMessage=EmailMessages.ProcessRawEmailMessageIn(_emailMessage.BodyText,_emailMessage.EmailMessageNum,emailAddress,true);//If decryption is successful, sets status to ReceivedDirect.
				//The Direct message was decrypted.
				EmailMessages.UpdateSentOrReceivedRead(_emailMessage);//Mark read, because we are already viewing the message within the current window.					
				RefreshAll();
        HasEmailChanged=true;
      }
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Decryption failed.")+"\r\n"+ex.Message);
				//Error=InvalidEncryption: means that someone used the wrong certificate when sending the email to this inbox, and we tried to decrypt with a different certificate.
				//Error=NoTrustedRecipients: means the sender is not added to the trust anchors in mmc.
			}
			Cursor=Cursors.Default;
		}

		private void butDelete_Click(object sender,EventArgs e) {
      if(IsNew){
        DialogResult=DialogResult.Cancel;
        Close();
        //It will be deleted in the FormClosing() Event.
      }
      else{
        if(_emailMessage.PatNum!=0 && !_isDeleteAllowed) {
          if(MsgBox.Show(this, MsgBoxButtons.YesNo,"Hide this email from the inbox?")) {
						//Forward compatible if we add new HideInFlags.
						_emailMessage.HideIn=(HideInFlags)Enum.GetValues(typeof(HideInFlags)).OfType<HideInFlags>()
							.Where(x => !ListTools.In(x,HideInFlags.None,HideInFlags.AccountCommLog,HideInFlags.AccountProgNotes,HideInFlags.ChartProgNotes)).ToList()
							.Sum(x=>(int)x); 
            EmailMessages.Update(_emailMessage);
            HasEmailChanged=true;
            DialogResult=DialogResult.OK;
            Close();
          }
        }
        else{
          if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this email?")){
            EmailMessages.Delete(_emailMessage);
            HasEmailChanged=true;
            DialogResult=DialogResult.OK;
            Close();
          }
        }
      }
		}

		private void butRawMessage_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(_emailMessage.RawEmailIn);
			msgbox.ShowDialog();
		}		

		private void butSave_Click(object sender,EventArgs e) {
			//this will not be available if already sent.
			SaveMsg();
			DialogResult=DialogResult.OK;
      Close(); //this form can be opened modelessly.
    }

		private void SaveMsg(){
			//allowed to save message with invalid fields, so no validation here.  Only validate when sending.
			_emailMessage.BodyText=emailPreview.BodyText;//markup text
			_emailMessage.HtmlText=emailPreview.HtmlText;
			if(emailPreview.IsHtml) {
				_emailMessage.HtmlType=EmailType.Html;
			}
			if(_isRawHtml) {
				_emailMessage.HtmlType=EmailType.RawHtml;
			}
			emailPreview.SaveMsg(_emailMessage);
			if(IsNew) {
				EmailMessages.Insert(_emailMessage);
				IsNew=false;//As soon as the message is saved to the database, it is no longer new because it has a primary key.  Prevents new email from being duplicated if saved multiple times.
			}
			else {
				EmailMessages.Update(_emailMessage);
			}
      HasEmailChanged=true;
    }

		private void butRefresh_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(_emailMessage.RawEmailIn)) {
				MsgBox.Show(this,"Email message no longer contains original raw message.");
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.WaitCursor;
			EmailAddress emailAddress=emailPreview.EmailAddressPreview;
			try {
				_emailMessage=EmailMessages.ProcessRawEmailMessageIn(_emailMessage.RawEmailIn,_emailMessage.EmailMessageNum,emailAddress,false);
				EmailMessages.UpdateSentOrReceivedRead(_emailMessage);//Mark read, because we are already viewing the message within the current window.
				RefreshAll();
        HasEmailChanged=true;
      }
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Refreshing failed.")+"\r\n"+ex.Message);
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
    }

		private void butEditHtml_Click(object sender,EventArgs e) {
			OpenEditHtmlWindow();
		}

		private void OpenEditHtmlWindow() {
			//get the most current version of the "plain" text from the emailPreview text box.
			using FormEmailEdit formEE=new FormEmailEdit();
			formEE.MarkupText=emailPreview.BodyText;//Copy existing text in case user decided to compose HTML after starting their email.
			formEE.IsRaw=_isRawHtml;
			formEE.ShowDialog();
			if(formEE.DialogResult!=DialogResult.OK) {
				return;
			}
			emailPreview.BodyText=formEE.MarkupText;
			emailPreview.HtmlText=formEE.HtmlText;
			_emailMessage.AreImagesDownloaded=formEE.AreImagesDownloaded;
			_isRawHtml=formEE.IsRaw;
			ChangeViewToHtml(_isRawHtml);
		}

		private void butEditText_Click(object sender,EventArgs e) {
			ChangeViewToPlainText();
		}

		private void ChangeViewToHtml(bool isRaw) {
			emailPreview.RefreshView(true,isRaw);
		}

		private void ChangeViewToPlainText() {
			emailPreview.RefreshView(false,false);
		}

		///<summary>Gets the outgoing email account (EmailAddress object) from emailPreview. Prompts user when there are problems matching the textbox 
		///displaying the sending address with an account in Email Setup.
		///Returns null if failed to match, or if matched to multiple and user canceled out of selection window.</summary>
		private EmailAddress GetOutgoingEmailForSending() {
			EmailAddress emailAddress=null;
			FromAddressMatchResult result=emailPreview.TryGetFromEmailAddress(out emailAddress);
			switch(result) {
				case FromAddressMatchResult.Failed:
					MessageBox.Show(Lan.g(this,"No email account found in Email Setup for")+": "+emailPreview.FromAddress);
					break;
				case FromAddressMatchResult.Success:
					//emailAddress set succesfully
					break;
				case FromAddressMatchResult.Multi:
					if(MessageBox.Show(Lan.g(this,"Multiple email accounts matching")+" "+emailPreview.FromAddress+"\r\n"
						+Lan.g(this,"Send using")+":\r\n"
						+Lan.g(this,"Username")+": "+emailAddress.EmailUsername+"\r\n"
						+Lan.g(this,"Sending Address")+": "+emailAddress.GetFrom()+"?","Email Address",MessageBoxButtons.YesNo)
						==DialogResult.No)
					{
						emailAddress=emailPreview.PickEmailAccount();
					}
					else {
						//emailAddress set to first matched emailAddress in email setup (isChooseFirstOnDuplicate).
					}
					break;
			}
			return emailAddress;
		}

    private void butDirectMessage_Click(object sender,EventArgs e) {
			//this will not be available if already sent.
			if(emailPreview.FromAddress=="" || emailPreview.ToAddress=="") {
				MessageBox.Show("Addresses not allowed to be blank.");
				return;
			}
			EmailAddress emailAddressFrom=GetOutgoingEmailForSending();
			if(emailAddressFrom==null) {
				return;
			}
			if(emailPreview.FromAddress!=emailAddressFrom.EmailUsername) {
				//Without this block, encryption would fail with an obscure error message, because the from address would not match the digital signature of the sender.
				MessageBox.Show(Lan.g(this,"From address must match email address username in email setup.")+"\r\n"+Lan.g(this,"From address must be exactly")+" "+emailAddressFrom.EmailUsername);
				return;
			}
			if(emailAddressFrom.SMTPserver=="") {
				MsgBox.Show(this,"The email address in email setup must have an SMTP server.");
				return;
			}
			if(emailPreview.ToAddress.Contains(",")) {
				MsgBox.Show(this,"Multiple recipient addresses not allowed for direct messaging.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(EmailMessages.GetReceiverUntrustedCount(emailPreview.ToAddress) >= 0) {//Not trusted yet.
				EmailMessages.TryAddTrustDirect(emailPreview.ToAddress);
			}
			SaveMsg();
			EmailSentOrReceived emailMessageOld=_emailMessage.SentOrReceived;
			try {
				_emailMessage.SentOrReceived=EmailSentOrReceived.SentDirect;
				EmailMessages.SendEmail(_emailMessage,emailAddressFrom,useDirect:true);
				MsgBox.Show(this,"Sent");
			}
			catch(Exception ex) {
				//Revert this emailmessage because it didn't get saved to the database.
				_emailMessage.SentOrReceived=emailMessageOld;
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(ex.Message);
				msgBox.ShowDialog();
				return;
			}
			Cursor=Cursors.Default;
      DialogResult=DialogResult.OK;
      Close();//this form can be opened modelessly.
    }

		///<summary>Becomes the reply button if the email was received.</summary>
		private void butSend_Click(object sender, System.EventArgs e) {
      if(_emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedEncrypted
         || _emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedDirect
         || _emailMessage.SentOrReceived==EmailSentOrReceived.ReadDirect
         || _emailMessage.SentOrReceived==EmailSentOrReceived.Received
         || _emailMessage.SentOrReceived==EmailSentOrReceived.Read
         || _emailMessage.SentOrReceived==EmailSentOrReceived.WebMailReceived
         || _emailMessage.SentOrReceived==EmailSentOrReceived.WebMailRecdRead)
      {
        if(!Security.IsAuthorized(Permissions.EmailSend)) {
          return;
        }
        using FormEmailMessageEdit FormE=new FormEmailMessageEdit(EmailMessages.CreateReply(_emailMessage,emailPreview.EmailAddressPreview),emailPreview.EmailAddressPreview,true,_listAllEmailMessages);
        FormE.IsNew=true;
        FormE.ShowDialog();
        if(FormE.DialogResult==DialogResult.OK) {
          HasEmailChanged=true;
					SaveMsg();
          DialogResult=DialogResult.OK;
          Close();//this form can be opened modelessly.
        }
        return;
      }
      //this will not be available if already sent.
      if(emailPreview.FromAddress==""){ 
				MsgBox.Show(this,"Please enter a sender address.");
				return;
			}
			if(emailPreview.ToAddress=="" && emailPreview.CcAddress=="" && emailPreview.BccAddress=="") {
				MsgBox.Show(this,"Please enter at least one recipient.");
				return;
			}
			if(EhrCCD.HasCcdEmailAttachment(_emailMessage)) {
				MsgBox.Show(this,"The email has a summary of care attachment which may contain sensitive patient data.  Use the Direct Message button instead.");
				return;
			}
			EmailAddress emailAddress=GetOutgoingEmailForSending();
			if(emailAddress==null) {
				return;
			}
			if(emailAddress.SMTPserver==""){
				MsgBox.Show(this,"The email address in email setup must have an SMTP server.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			SaveMsg();
			try{
				//By this point, we are confident we have selected the correct EmailAddress object.  Use the appropriate cert/sig for this address.
				System.Security.Cryptography.X509Certificates.X509Certificate2 cert=null;
				if(emailPreview.IsSigned) { 
					cert=EmailMessages.GetCertFromPrivateStore(emailAddress.EmailUsername);
				}
				_emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
				EmailMessages.SendEmail(_emailMessage,emailAddress,cert);
				MsgBox.Show(this,"Sent");
			}
			catch(Exception ex){
				Cursor=Cursors.Default;
				string message=Lan.g(this,"Failed to send email.")+"\r\n\r\n"+Lan.g(this,"Error message from the email client was")+":\r\n  "+ex.Message;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(message);
				msgBox.ShowDialog();
				return;
			}
			Cursor=Cursors.Default;
      //MessageCur.MsgDateTime=DateTime.Now;
      DialogResult=DialogResult.OK;
      Close();//this form can be opened modelessly.
    }

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(!emailPreview.IsComposing) {//Use clicked the 'Close' button.  This is a 'read' email, so only changeable property is HideInFlags.
				SaveMsg();
				DialogResult=DialogResult.OK;//Triggers a refresh in calling views.
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
      Close();
		}

		private void FormEmailMessageEdit_FormClosing(object sender,FormClosingEventArgs e) {
      if(_hasTemplatesChanged){
        DataValid.SetInvalid(InvalidType.Email);
      }
      EmailSaveEvent.Fired-=EmailSaveEvent_Fired;
			if(HasEmailChanged){
        Signalods.SetInvalid(InvalidType.EmailMessages);
        return;
			}
			if(IsNew){
				EmailMessages.Delete(_emailMessage);
			}
		}

	}
}