using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using Newtonsoft.Json;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailMessageEdit : FormODBase {
		public bool IsNew;
		private bool _didTemplatesChange;
		///<summary>A copy of the message passed into the constructor.</summary>
		private EmailMessage _emailMessage;
		///<summary>If currently replying to a received message, the EmailMessage to which we are replying</summary>
		private EmailMessage _emailMessageReplyingTo;
		private bool _isDeleteAllowed=true;
		public bool DidEmailChange;
		///<summary>List of email messages to be considered for auto complete email address popup. When null will run query.</summary>
		private List<EmailMessage> _listEmailMessages=null;
		private List<EmailTemplate> _listEmailTemplates;
		private bool _isRawHtml;
		private long _clinicNum;
		private Patient _patient;

		///<summary>isDeleteAllowed defines whether the email is able to be deleted when a patient is attached. 
		///emailAddress corresponds to the account in Email Setup that will be used to send the email.
		///Currently, emails that are "Deleted" from the inbox are actually just hidden if they have a patient attached.</summary>
		public FormEmailMessageEdit(EmailMessage emailMessage,EmailAddress emailAddress=null,bool isDeleteAllowed=true
			,EmailMessage emailMessageReplyingTo=null,params List<EmailMessage>[] listEmailMessagesAll)
		{
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_isDeleteAllowed=isDeleteAllowed;
			_emailMessage=emailMessage.Copy();
			if(emailAddress==null) {
				_patient=EmailMessages.GetPatient(_emailMessage);
				emailPreview.LoadEmailAddresses(_patient.ClinicNum);
				FromAddressMatchResult formAddressMatchResult=emailPreview.TryGetFromEmailAddress(out emailAddress,_emailMessage.FromAddress);
				switch(formAddressMatchResult) {
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
			//Jordan It looks like this array of lists is flattened in the second step. I think doing that in the first step would be more elegant.
			List<List<EmailMessage>> listListsEmailMessagesHistory=listEmailMessagesAll.ToList().FindAll(x => x!=null); //Get every non-null email list (Inbox, Sent, etc)
			if(listListsEmailMessagesHistory.Count>0) {
				_listEmailMessages=listListsEmailMessagesHistory.SelectMany(x => x).Where(x => x!=null).ToList(); //Use SelectMany to get every non-null email message from those lists
			}
			_clinicNum=Clinics.ClinicNum;
			if(PrefC.HasClinicsEnabled && _clinicNum==0) {
				//Clinic0 cannot be directly signed up for Secure Email, so use the 'Default Clinic'
				_clinicNum=PrefC.GetLong(PrefName.EmailSecureDefaultClinic);
			}
			_emailMessageReplyingTo=emailMessageReplyingTo;
		}

		///<summary>Configures and adds the 'Send Email' button and 'Close' button to the UI.</summary>
		private void ConfigureSendButtons() {
			toolBarSend.Buttons.Clear();
			ODToolBarButton odToolBarButton=null;
			ContextMenu contextMenu=null;
			//Init the cancel button it will be part of the ODToolBarButton regardless.
			MenuItem menuItemCancel=new MenuItem();
			menuItemCancel.Text=Lan.g(this,"Cancel");
			menuItemCancel.Click+=new EventHandler(this.butCancel_Click);
			//Add send buttons if we are composing
			if(emailPreview.IsComposing) {
				contextMenu=GetSendMenu();
				contextMenu.MenuItems.Add(menuItemCancel);
				//Setting the Tag to the first option in the list results in first option click event on click.
				MenuItem menuItemSendDefault=contextMenu.MenuItems[0];
				odToolBarButton=new ODToolBarButton(menuItemSendDefault.Text,EnumIcons.Email,menuItemSendDefault.Text,menuItemSendDefault);
				odToolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				odToolBarButton.DropDownMenu=contextMenu;
			}
			//Add reply button if this is a received email
			else if(EmailMessages.IsReceived(_emailMessage.SentOrReceived)) {
				MenuItem menuItemReply=new MenuItem();
				menuItemReply.Text=Lan.g(this,"Reply");
				menuItemReply.Click+=new EventHandler(this.butSend_Click);//Opens new FormEmailMessageEdit window pre-filled with a reply to this email.
				contextMenu=new ContextMenu(new MenuItem[] { menuItemReply,menuItemCancel });
				odToolBarButton=new ODToolBarButton(menuItemReply.Text,EnumIcons.Email,"",menuItemReply);
				odToolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				odToolBarButton.DropDownMenu=contextMenu;
			}
			//Sent emails will only have Cancel
			else {
				odToolBarButton=new ODToolBarButton(menuItemCancel.Text,EnumIcons.None,"",menuItemCancel);
			}
			//ODToolBarButton does not have dimensions until after ToolBarOD's first OnPaint()...
			toolBarSend.Visible=true;
			toolBarSend.Paint+=ToolBarSend_Paint;
			toolBarSend.Buttons.Add(odToolBarButton);
			toolBarSend.Invalidate();
		}

		private void ToolBarSend_Paint(object sender,PaintEventArgs e) {
			//ODToolBarButton does not have dimensions until after ToolBarOD's first OnPaint()...
			int width=toolBarSend.Buttons[0]?.Bounds.Width??0;
			if(width!=0 && width!=toolBarSend.Width) {
				//Resize and relocate toolBarSend so it displays properly in the form.
				LayoutManager.MoveWidth(toolBarSend,width);
				LayoutManager.MoveLocation(toolBarSend,new Point(emailPreview.Location.X+emailPreview.Width-toolBarSend.Width,toolBarSend.Location.Y));
			}
			toolBarSend.Paint-=ToolBarSend_Paint;//Dimensions are set.  No need to keep doing this.
		}

		///<summary>Configures the dropdown/contextmenu for the 'Send Email' button.</summary>
		private ContextMenu GetSendMenu() {
			if(!Enum.TryParse(ClinicPrefs.GetPrefValue(PrefName.EmailDefaultSendPlatform,_clinicNum),out EmailPlatform emailPlatform)
				|| (emailPlatform==EmailPlatform.Secure && !Clinics.IsSecureEmailEnabled(_clinicNum))
				|| (emailPlatform==EmailPlatform.Direct && !IsDirectMessagingEnabled()))
			{
				//If pref is invalid, or set to a platform that is not signed up, default to Insecure email.
				emailPlatform=EmailPlatform.Unsecure;
			}
			ContextMenu contextMenu=new ContextMenu();
			MenuItem menuItemSend=new MenuItem();
			menuItemSend.Text=Lan.g(this,"Send");
			menuItemSend.Click+=new EventHandler(this.butSend_Click);
			MenuItem menuItemSendSecure=new MenuItem();
			menuItemSendSecure.Text=Lan.g(this,"Send Secure");
			menuItemSendSecure.Click+=new EventHandler(this.butSendSecure_Click);
			MenuItem menuItemSendDirect=new MenuItem();
			menuItemSendDirect.Text=Lan.g(this,"Direct Messaging");//For EHR and very rare
			menuItemSendDirect.Click+=new EventHandler(this.butDirectMessage_Click);
			MenuItem getDefaultMenuItem() {
				return emailPlatform switch {
					EmailPlatform.Secure => menuItemSendSecure,
					EmailPlatform.Direct => menuItemSendDirect,
					EmailPlatform.Unsecure => menuItemSend,
					_ => menuItemSend,//If the preference is invalid, just use Insecure as the default.
				};
			}
			List<MenuItem> listMenuItems=new List<MenuItem>() {
				menuItemSend,//always an option
			};
			if(EmailSecures.IsSecureEmailReleased()) {
				listMenuItems.Add(menuItemSendSecure);
			}
			if(IsDirectMessagingEnabled()) {
				//Only include Direct Messaging if enabled.
				listMenuItems.Add(menuItemSendDirect);
			}
			//Default first, then SendInsecure
			listMenuItems=listMenuItems.OrderByDescending(x => x==getDefaultMenuItem()).ThenByDescending(x => x==menuItemSend).ToList();
			for(int i=0;i<listMenuItems.Count;i++) {
				listMenuItems[i].Index=i;
			}
			//If we are replying to a secure email, the only option should be to reply with a secure email.
			//Users should start a new email if they want to send a different type.
			if(IsReplyingToSecureEmail()) {
				listMenuItems=new List<MenuItem>() { menuItemSendSecure };
			}
			contextMenu.MenuItems.AddRange(listMenuItems.ToArray());
			return contextMenu;
		}

		private void toolBarSend_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag is MenuItem menuItemSend) {
				menuItemSend.PerformClick();
			}
		}

		private bool IsDirectMessagingEnabled() {			
			return PrefC.GetBool(PrefName.ShowFeatureEhr);
		}

		private void FormEmailMessageEdit_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup,suppressMessage:true)) {
				butAddTemplate.Enabled=false;
				butDeleteTemplate.Enabled=false;
			}
			if(_emailMessage.PatNum==0) {
				butSave.Enabled=false;
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
			emailPreview.LoadEmailMessage(_emailMessage,_listEmailMessages,IsReplyingToSecureEmail());
			if(!emailPreview.IsComposing) {
				panelTemplates.Visible=false;
				panelAutographs.Visible=false;
				//When opening an email from FormEmailInbox, the email status will change to read automatically,
				//and changing the text on the cancel button helps convey that to the user.
				butEditText.Visible=false;
				butEditHtml.Visible=false;
			}
			FillTemplates();
			FillAutographs();
			butRefresh.Visible=false;
			//For all email received types, we disable most of the controls and put the form into a mostly read-only state.
			if(EmailMessages.IsReceived(_emailMessage.SentOrReceived)) {
				butRefresh.Visible=true;
				butRawMessage.Visible=true;
			}
			ConfigureSendButtons();
			labelDecrypt.Visible=false;
			butDecrypt.Visible=false;
			if(_emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedEncrypted) {
				labelDecrypt.Visible=true;
				butDecrypt.Visible=true;
				butRefresh.Visible=false;
			}
		}

		///<summary>Returns true if this message is a reply to a Secure Email.</summary>
		private bool IsReplyingToSecureEmail() {
			return _emailMessageReplyingTo!=null && EmailMessages.IsSecureEmail(_emailMessageReplyingTo.SentOrReceived);
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
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormEmailTemplateEdit formEmailTemplateEdit=new FormEmailTemplateEdit();
			formEmailTemplateEdit.EmailTemplateCur=_listEmailTemplates[listTemplates.SelectedIndex];
			formEmailTemplateEdit.ShowDialog();
			if(formEmailTemplateEdit.DialogResult!=DialogResult.OK){
				return;
			}
			EmailTemplates.RefreshCache();
			_didTemplatesChange=true;
			FillTemplates();
		}

		private void butAddTemplate_Click(object sender, System.EventArgs e) {
			using FormEmailTemplateEdit formEmailTemplateEdit=new FormEmailTemplateEdit();
			formEmailTemplateEdit.IsNew=true;
			formEmailTemplateEdit.EmailTemplateCur=new EmailTemplate();
			formEmailTemplateEdit.ShowDialog();
			if(formEmailTemplateEdit.DialogResult!=DialogResult.OK){
				return;
			}
			EmailTemplates.RefreshCache();
			_didTemplatesChange=true;
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
			_didTemplatesChange=true;
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
			List<EmailAttach> listEmailAttaches=EmailAttaches.GetForTemplate(listEmailTemplates[listTemplates.SelectedIndex].EmailTemplateNum);
			listEmailAttaches.ForEach(x => x.EmailTemplateNum=0); //Unattach the emailattachments from the email template.
			EmailTemplate emailTemplateSelected=listEmailTemplates[listTemplates.SelectedIndex];
			if(EmailMessages.IsHtmlEmail(emailTemplateSelected.TemplateType)) {
				bool isRawHtml=emailTemplateSelected.TemplateType==EmailType.RawHtml;
				if(!HasValidInlineAttachments(emailTemplateSelected.BodyText,isRawHtml)) { //Need to check inline images if they are valid
					return;
				}
				//this is an html template. Translate and then set the view to html.
				emailPreview.LoadTemplate(listEmailTemplates[listTemplates.SelectedIndex].Subject,emailTemplateSelected.BodyText,listEmailAttaches);
				emailPreview.HtmlText=emailPreview.BodyText;
				emailPreview.BodyText=emailPreview.BodyText;
				_isRawHtml=isRawHtml;
				ChangeViewToHtml(_isRawHtml);
			}
			else {
				//regular template. 
				emailPreview.LoadTemplate(emailTemplateSelected.Subject,emailTemplateSelected.BodyText,listEmailAttaches);
			}
		}

		private void butEditTemplate_Click(object sender,EventArgs e) {
			if(listTemplates.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormEmailTemplateEdit formEmailTemplateEdit=new FormEmailTemplateEdit();
			formEmailTemplateEdit.EmailTemplateCur=_listEmailTemplates[listTemplates.SelectedIndex];
			formEmailTemplateEdit.ShowDialog();
			if(formEmailTemplateEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			EmailTemplates.RefreshCache();
			_didTemplatesChange=true;
			FillTemplates();
		}

		private void listTemplates_MouseMove(object sender,MouseEventArgs e) {
			int idxItem=listTemplates.IndexFromPoint(e.Location);
			if(idxItem==-1) {
				toolTipMessage.Hide(listTemplates);
				return;
			}
			string description=listTemplates.Items[idxItem].ToString();
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

		/// <summary>Searches the list of EmailAutographs and returns the first match, otherwise null</summary>
		private EmailAutograph FindDefaultEmailAutograph() {
			if(emailPreview.TryGetFromEmailAddress(out EmailAddress emailAddressOutgoing)==FromAddressMatchResult.Failed) {
				return null;
			}
			string emailUsername=EmailMessages.GetAddressSimple(emailAddressOutgoing.EmailUsername);
			string emailSender=EmailMessages.GetAddressSimple(emailAddressOutgoing.SenderAddress);
			string autographEmail;
			for(int i=0;i<listAutographs.Items.Count;i++) {
				autographEmail=EmailMessages.GetAddressSimple(((EmailAutograph)listAutographs.Items.GetObjectAt(i)).EmailAddress.Trim());
				//Use Contains() because an autograph can theoretically have multiple email addresses associated with it.
				if((!string.IsNullOrWhiteSpace(emailUsername) && autographEmail.Contains(emailUsername)) 
					|| (!string.IsNullOrWhiteSpace(emailSender) && autographEmail.Contains(emailSender)))
				{
					return (EmailAutograph)listAutographs.Items.GetObjectAt(i);
				}
			}
			return null;
		}

		///<summary>Sets the default autograph that shows in the message body. 
		///The default autograph is determined to be the first autograph with an email that matches the email address of the sender.</summary>
		private void SetDefaultAutograph() {
			if(!emailPreview.IsComposing || !IsNew) {
				return;
			}
			EmailAutograph emailAutograph=FindDefaultEmailAutograph();
			if(emailAutograph==null) {
				return;
			}
			if(!IsAutographHTML(emailAutograph)) {//HTML autographs will be appended automatically on send.
				InsertAutograph(emailAutograph);
			}
		}
		
		///<summary>Currently just appends an autograph to the bottom of the email message.  When the functionality to reply to emails is implemented, 
		///this will need to be modified so that it inserts the autograph text at the bottom of the new message being composed, but above the message
		///history.</summary>
		private void InsertAutograph(EmailAutograph emailAutograph) {
			emailPreview.BodyText+="\r\n\r\n"+emailAutograph.AutographText;
		}

		/// <summary>Returns true if the given EmailAutograph contains HTML tags, false otherwise. </summary>
		private bool IsAutographHTML(EmailAutograph emailAutograph) {
			string allTagsPattern="<(?:\"[^\"]*\"|'[^']*'|[^'\">])+>"; //Matches all HTML tags including inline css with double or single quotations.
			bool doesContainHtml=Regex.IsMatch(emailAutograph.AutographText,allTagsPattern);
			//An autograph without HTML tags may still have an OD-style image link (which requires HTML to insert correctly), so check for that as well
			//This regex looks for any tags of the syntax "[[img:FILE.EXT]]" where "FILE" is any valid file name and "EXT" is a file extension that matches one of the image formats specified below
			//We check for a valid file extension to prevent matching against non-image links, e.g. "[[How To Check A Patient In]]"
			string imageTagPattern="\\[\\[img:.*(bmp|gif|jpeg|jpg|png|svg)\\]\\]";
			doesContainHtml|=Regex.IsMatch(emailAutograph.AutographText,imageTagPattern,RegexOptions.IgnoreCase);
			return doesContainHtml;
		}

		private void listAutographs_DoubleClick(object sender,EventArgs e) { //edit an autograph
			if(listAutographs.SelectedIndex==-1) {
				return;
			}
			using FormEmailAutographEdit formEmailAutographEdit=new FormEmailAutographEdit(listAutographs.GetSelected<EmailAutograph>());
			formEmailAutographEdit.ShowDialog();
			if(formEmailAutographEdit.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Email);
				FillAutographs();
			}
		}

		private void butAddAutograph_Click(object sender,EventArgs e) { //add a new autograph
			EmailAutograph emailAutograph=new EmailAutograph();
			using FormEmailAutographEdit formEmailAutographEdit=new FormEmailAutographEdit(emailAutograph);
			formEmailAutographEdit.IsNew=true;
			formEmailAutographEdit.ShowDialog();
			if(formEmailAutographEdit.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Email);
				FillAutographs();
			}
		}

		private void butInsertAutograph_Click(object sender,EventArgs e) {
			if(listAutographs.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select an autograph before inserting."));
				return;
			}
			if(emailPreview.IsHtml || IsAutographHTML(listAutographs.GetSelected<EmailAutograph>())) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Autographs with images or HTML tags must be inserted from the Edit HTML window using the Autograph dropdown in the toolbar."
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
			DataValid.SetInvalid(InvalidType.Email);
			FillAutographs();
		}

		private void butEditAutograph_Click(object sender,EventArgs e) {
			if(listAutographs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			using FormEmailAutographEdit formEmailAutographEdit=new FormEmailAutographEdit(listAutographs.GetSelected<EmailAutograph>());
			formEmailAutographEdit.ShowDialog();
			if(formEmailAutographEdit.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Email);
				FillAutographs();
			}
		}

		#endregion

		private void butDecrypt_Click(object sender,EventArgs e) {
			if(EmailMessages.GetReceiverUntrustedCount(_emailMessage.FromAddress) >= 0) {//Not trusted yet.
				string msgTrust=Lan.g(this,"The sender address must be added to your trusted addresses before you can decrypt the email")
					+". "+Lan.g(this,"Add")+" "+_emailMessage.FromAddress+" "+Lan.g(this,"to trusted addresses")+"?";
				if(MessageBox.Show(msgTrust,"",MessageBoxButtons.OKCancel)==DialogResult.OK) {
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
				_emailMessage=EmailMessages.ProcessRawEmailMessageIn(_emailMessage.BodyText,_emailMessage.EmailMessageNum,emailAddress,isAck:true,_emailMessage.SentOrReceived);//Does not change read status of email regardless of success.
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Decryption failed.")+"\r\n"+ex.Message);
				//Error=InvalidEncryption: means that someone used the wrong certificate when sending the email to this inbox, and we tried to decrypt with a different certificate.
				//Error=NoTrustedRecipients: means the sender is not added to the trust anchors in mmc.
				Cursor=Cursors.Default;
				return;
			}
			RefreshAll();
			DidEmailChange=true;	
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
							.Where(x => !x.In(HideInFlags.None,HideInFlags.AccountCommLog,HideInFlags.AccountProgNotes,HideInFlags.ChartProgNotes)).ToList()
							.Sum(x=>(int)x); 
						EmailMessages.Update(_emailMessage);
						DidEmailChange=true;
						DialogResult=DialogResult.OK;
						Close();
					}
				}
				else{
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this email?")){
						EmailMessages.Delete(_emailMessage);
						DidEmailChange=true;
						DialogResult=DialogResult.OK;
						Close();
					}
				}
			}
		}

		private void butRawMessage_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(_emailMessage.RawEmailIn);
			msgBoxCopyPaste.ShowDialog();
		}		

		private void butSave_Click(object sender,EventArgs e) {
			//this will not be available if already sent.
			SaveMsg();
			DialogResult=DialogResult.OK;
			Close(); //this form can be opened modelessly.
		}

		private void SaveMsg(){
			//allowed to save message with invalid fields, so no validation here.  Only validate when sending.
			if(emailPreview.IsComposing) {
				_emailMessage.BodyText=emailPreview.BodyText;//markup text
				_emailMessage.HtmlText=emailPreview.HtmlText;
				if(emailPreview.IsHtml) {
					_emailMessage.HtmlType=EmailType.Html;
				}
				if(_isRawHtml) {
					_emailMessage.HtmlType=EmailType.RawHtml;
				}
			}
			emailPreview.SaveMsg(_emailMessage);
			if(IsNew) {
				EmailMessages.Insert(_emailMessage);
				IsNew=false;//As soon as the message is saved to the database, it is no longer new because it has a primary key.  Prevents new email from being duplicated if saved multiple times.
			}
			else {
				EmailMessages.Update(_emailMessage);
			}
			DidEmailChange=true;
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
				_emailMessage=EmailMessages.ProcessRawEmailMessageIn(_emailMessage.RawEmailIn,_emailMessage.EmailMessageNum,emailAddress,isAck:false,_emailMessage.SentOrReceived);//Does not change read status of email regardless of success.
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Refreshing failed.")+"\r\n"+ex.Message);
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			RefreshAll();
			DidEmailChange=true;
		}

		private void butEditHtml_Click(object sender,EventArgs e) {
			OpenEditHtmlWindow();
		}

		private void OpenEditHtmlWindow() {
			//get the most current version of the "plain" text from the emailPreview text box.
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			//todo, decide how images will be uploaded and written to the raw email for secure email
			formEmailEdit.MarkupText=emailPreview.BodyText;//Copy existing text in case user decided to compose HTML after starting their email.
			formEmailEdit.IsRaw=_isRawHtml;
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			emailPreview.BodyText=formEmailEdit.MarkupText;
			emailPreview.HtmlText=formEmailEdit.HtmlText;
			_emailMessage.AreImagesDownloaded=formEmailEdit.AreImagesDownloaded;
			_isRawHtml=formEmailEdit.IsRaw;
			ChangeViewToHtml(_isRawHtml);
		}

		private void butEditText_Click(object sender,EventArgs e) {
			ChangeViewToPlainText();
		}

		private void ChangeViewToHtml(bool isRaw) {
			emailPreview.RefreshView(isHtml:true,isRaw);
		}

		private void ChangeViewToPlainText() {
			emailPreview.RefreshView(isHtml:false,isRawHtml:false);
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
			_emailMessage.SentOrReceived=EmailSentOrReceived.SentDirect;
			try {
				EmailMessages.SendEmail(_emailMessage,emailAddressFrom,useDirect:true);
			}
			catch(Exception ex) {
				//Revert this emailmessage because it didn't get saved to the database.
				_emailMessage.SentOrReceived=emailMessageOld;
				Cursor=Cursors.Default;
				string errMsg=Lan.g(this,"Failed to send email.")+"\r\n"+Lan.g(this, "Click Details to see the error message from the Email Client.");
				FriendlyException.Show(errMsg, ex);
				return;
			}
			MsgBox.Show(this,"Sent");
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
			Close();//this form can be opened modelessly.
		}

		///<summary>Returns true if all of the inline images exist. Returns false otherwise. If the body text isRawHtml then will just return true and not check.</summary>
		private bool HasValidInlineAttachments(string bodyText,bool isRawHtml) {
			if(isRawHtml) {
				return true;//Ignore checking inline images
			}
			string imageOpeningTag="[[img:";
			string imageClosingTag="]]";
			string bodyTextRemaining=bodyText;
			while(bodyTextRemaining.Contains(imageOpeningTag)) {
				int index=bodyTextRemaining.IndexOf(imageOpeningTag);
				if(index==-1) {
					break;//No more inline images
				}
				if(!bodyTextRemaining.Contains(imageClosingTag)) {
					MessageBox.Show(Lan.g(this,"One or more image tags do not close within this template."));
					return false;
				}
				index+=imageOpeningTag.Length;
				bodyTextRemaining=bodyTextRemaining.Substring(index,bodyTextRemaining.Length-index);
				string imgName=bodyTextRemaining.Substring(0,bodyTextRemaining.IndexOf(imageClosingTag));
				string fullPath;
				try {
					fullPath=ImageStore.GetEmailImagePath();
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,ex.Message));
					return false;
				}
				fullPath=FileAtoZ.CombinePaths(fullPath,POut.String(imgName));
				if(!FileAtoZ.Exists(fullPath)) {
					MessageBox.Show(Lan.g(this,$"{imgName} could not be found."));
					return false;
				}
			}
			return true;
		}

		///<summary>Checks to make sure there's a value in the From and To address text boxes. If isSecureEmail is true, will also check if there's a value in the Subject line</summary>
		private bool ValidateFieldsForSend(bool isSecureEmail=false) {
			StringBuilder error=new StringBuilder();
			if(emailPreview.FromAddress=="") {
				error.AppendLine(Lan.g(this,"Sender address is required."));
			}
			if(emailPreview.ToAddress=="" && emailPreview.CcAddress=="" && emailPreview.BccAddress=="") {
				error.AppendLine(Lan.g(this,"At least one recipient is required."));
			}
			if(isSecureEmail && string.IsNullOrWhiteSpace(emailPreview.Subject)) {
				error.AppendLine(Lan.g(this,"Subject line is required."));
			}
			string errorText=PrefC.GetFirstShortURL(emailPreview.BodyText);
			if(!string.IsNullOrWhiteSpace(errorText)) {
				error.AppendLine(Lan.g(this,"Message cannot contain the URL")+" "+errorText+" "+Lan.g(this,"as this is only allowed for eServices."));
			}
			string errorMsg=error.ToString();
			if(!string.IsNullOrWhiteSpace(errorMsg)) {
				MessageBox.Show(this,Lan.g(this,"The following error(s) need to be addressed before you can send your email")+$":\n{errorMsg}");
				return false;
			}
			return true;
		}

		///<summary>Becomes the reply button if the email was received.</summary>
		private void butSend_Click(object sender, System.EventArgs e) {
			toolBarSend.Enabled=false;
			if(EmailMessages.IsReceived(_emailMessage.SentOrReceived)) {
				if(!Security.IsAuthorized(EnumPermType.EmailSend)) {
					toolBarSend.Enabled=true;
					return;
				}
				using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(EmailMessages.CreateReply(_emailMessage,emailPreview.EmailAddressPreview)
					,emailPreview.EmailAddressPreview,isDeleteAllowed:true,emailMessageReplyingTo:_emailMessage);
				formEmailMessageEdit.IsNew=true;
				formEmailMessageEdit.ShowDialog();
				if(formEmailMessageEdit.DialogResult==DialogResult.OK) {
					DidEmailChange=true;
					SaveMsg();
					DialogResult=DialogResult.OK;
					Close();//this form can be opened modelessly.
				}
				toolBarSend.Enabled=true;
				return;
			}
			//this will not be available if already sent.
			if(!ValidateFieldsForSend()) {
				toolBarSend.Enabled=true;
				return;
			}
			if(EhrCCD.HasCcdEmailAttachment(_emailMessage)) {
				MsgBox.Show(this,"The email has a summary of care attachment which may contain sensitive patient data.  Use the Direct Message button instead.");
				toolBarSend.Enabled=true;
				return;
			}
			EmailAddress emailAddress=GetOutgoingEmailForSending();
			if(emailAddress==null) {
				toolBarSend.Enabled=true;
				return;
			}
			if(emailAddress.SMTPserver==""){
				MsgBox.Show(this,"The email address in email setup must have an SMTP server.");
				toolBarSend.Enabled=true;
				return;
			}
			if(emailAddress.AccessToken.IsNullOrEmpty() && emailAddress.AuthenticationType==OAuthType.Microsoft) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This email address needs to be re-authenticated. Sign into this email through Microsoft?")) {
					MicrosoftTokenHelper microsoftToken=new MicrosoftTokenHelper();
					if(ODBuild.IsWeb()) {
						if(!CloudClientL.IsCloudClientRunning()) {
							toolBarSend.Enabled=true;
							return; 
						}
						string strMicrosoftAuthCodesJSON=ODCloudClient.GetMicrosoftAccessToken(emailAddress.EmailUsername,"");
						if(!strMicrosoftAuthCodesJSON.IsNullOrEmpty())
						microsoftToken=JsonConvert.DeserializeObject<MicrosoftTokenHelper>(strMicrosoftAuthCodesJSON);
					}
					else {
						microsoftToken=System.Threading.Tasks.Task.Run(async()=>await MicrosoftApiConnector.GetAccessToken(emailAddress.EmailUsername,"")).GetAwaiter().GetResult();
					}
					if(microsoftToken.ErrorMessage!="") {
						MsgBox.Show("Error: "+microsoftToken.ErrorMessage);
						toolBarSend.Enabled=true;
						return;
					}
					if(microsoftToken.AccessToken=="") {
						toolBarSend.Enabled=true;
						return; //Authentication was cancelled so do nothing.
					}
					if(microsoftToken.EmailAddress!=emailAddress.EmailUsername) {
						MsgBox.Show("Please sign in with the same email address.");
						toolBarSend.Enabled=true;
						return;
					}
					emailAddress.AccessToken=microsoftToken.AccessToken;
					emailAddress.RefreshToken=microsoftToken.AccountInfo;
					emailAddress.Pop3ServerIncoming="pop.outlook.com"; //This field was also cleared and needs to be populated.
					EmailAddresses.Update(emailAddress);
					DataValid.SetInvalid(InvalidType.Email);
				}
				else {
					toolBarSend.Enabled=true;
					return;//User wants to manually sign into the email in the setup window.
				}
			}
			//If default autograph is HTML send the whole email as HTML.
			EmailAutograph emailAutograph=FindDefaultEmailAutograph();
			if(emailAutograph!=null && IsAutographHTML(emailAutograph) && _emailMessage.HtmlType==EmailType.Regular) {
				bool hasValidHTML=false;
				try {
					string markupText=emailPreview.BodyText+"\r\n\r\n"+emailAutograph.AutographText;
					emailPreview.HtmlText=MarkupEdit.TranslateToXhtml(markupText,true,isEmail:true);
					hasValidHTML=true;
				}
				catch(Exception ex) {
					ex.DoNothing();
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"There was a problem automatically appending the HTML autograph. Continue without an autograph?")) {
						toolBarSend.Enabled=true;
						return;//User wants to manually fix the HTML autograph and try again.
					}
				}
				if(hasValidHTML) {
					InsertAutograph(emailAutograph);
					ChangeViewToHtml(isRaw:false);//can't be raw HTML since EmailType is regular above
				}
			}
			Cursor=Cursors.WaitCursor;
			SaveMsg();
			System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate2=null;
			//By this point, we are confident we have selected the correct EmailAddress object.  Use the appropriate cert/sig for this address.
			if(emailPreview.IsSigned) { 
				x509Certificate2=EmailMessages.GetCertFromPrivateStore(emailAddress.EmailUsername);
			}
			_emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
			try{
				EmailMessages.SendEmail(_emailMessage,emailAddress,x509Certificate2);
			}
			catch(Exception ex){
				Cursor=Cursors.Default;
				if(ODBuild.IsWeb() && ex.InnerException!=null) {
					if(ex.InnerException.Message.Contains("InvalidAuthenticationToken")) {
						emailAddress.AccessToken="";
						EmailAddresses.Update(emailAddress);
						DataValid.SetInvalid(InvalidType.Email);
						butSend_Click(sender,e);
					toolBarSend.Enabled=true;
					return;
					}
				}
				string errMsg=Lan.g(this,"Failed to send email.")+"\r\n"+Lan.g(this, "Click Details to see the error message from the Email Client.");
				FriendlyException.Show(errMsg, ex);
				toolBarSend.Enabled=true;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Sent");
			//MessageCur.MsgDateTime=DateTime.Now;
			DialogResult=DialogResult.OK;
			Close();//this form can be opened modelessly.
		}

		///<summary>Sends the email as a Secure Email via the EmailHosting API.</summary>
		private void butSendSecure_Click(object sender, System.EventArgs e) {
			if(Clinics.IsSecureEmailEnabled(_clinicNum)) {
				SendSecure();
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Secure Email must be signed up and enabled before sending emails.  Go to setup?")) {
				return;
			}
			using FormSecureEmailSetup formSecureEmailSetup=new FormSecureEmailSetup();
			formSecureEmailSetup.ShowDialog();
			//Now we need to update the default clinic num.
			if(PrefC.HasClinicsEnabled && _clinicNum==0) {
				//Clinic0 cannot be directly signed up for Secure Email, so use the 'Default Clinic'
				_clinicNum=PrefC.GetLong(PrefName.EmailSecureDefaultClinic);
			}
			ConfigureSendButtons();
		}

		private void SendSecure() {
			if(!ValidateFieldsForSend(isSecureEmail:true)) {
				return;
			}
			if(!IsValidSecureEmail()) {
				return;
			}
			EmailAddress emailAddressSender=GetOutgoingEmailForSending();
			if(emailAddressSender==null) {
				return;
			}
			SaveMsg();//wires UI into _emailMessage and inserts/updates db.
			string toAddress=emailPreview.ToAddress;
			ProgressOD progressOD=new ProgressOD();
			//Send the Email
			progressOD.ActionMain=() => {
				_patient??=EmailMessages.GetPatient(_emailMessage);
				EmailSecures.SendSecureEmail(_emailMessage,emailAddressSender,toAddress,_clinicNum,_emailMessageReplyingTo,_patient);
			};
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Failed to send secure email.")+"\r\n"+ex.Message,ex);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			MsgBox.Show(this,"Sent");
			DialogResult=DialogResult.OK;
			Close();//this form can be opened modelessly.
		}

		#region Secure Email Validation
		///<summary>Returns true if fields are valid for sending a Secure Email. CC and BCC fields must be blank.</summary>
		private bool IsValidSecureEmail() {
			//Secure emails only have From and To addresses.
			if((emailPreview.CcAddress==string.Empty && emailPreview.BccAddress==string.Empty)
				|| MsgBox.Show(this,MsgBoxButtons.YesNo,"CC and BCC fields must be blank for Secure Emails." +
			" Would you like to move the addresses in those fields to the To field and continue sending?"))
			{
				MoveCCandBCCIntoToField();
				return true;
			}
			return false;
		}

		///<summary>Moves the addresses in the CC and BCC fields into the To field.</summary>
		private void MoveCCandBCCIntoToField() {
			List<string> listAddressesToMove=emailPreview.CcAddress.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
			listAddressesToMove.AddRange(emailPreview.BccAddress.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList());
			for(int i=0;i<listAddressesToMove.Count;i++) {
				if(emailPreview.ToAddress!="") {
					emailPreview.ToAddress+=",";
				}
				emailPreview.ToAddress+=listAddressesToMove[i];
			}
			emailPreview.CcAddress="";
			emailPreview.BccAddress="";
		}
		#endregion Secure Email Validation

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormEmailMessageEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_didTemplatesChange){
				DataValid.SetInvalid(InvalidType.Email);
			}
			EmailSaveEvent.Fired-=EmailSaveEvent_Fired;
			if(DidEmailChange){
				Signalods.SetInvalid(InvalidType.EmailMessages);
				return;
			}
			if(IsNew){
				EmailMessages.Delete(_emailMessage);
			}
		}
	}
}