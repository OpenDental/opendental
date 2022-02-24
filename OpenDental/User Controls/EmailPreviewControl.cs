using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using Health.Direct.Common.Mime;

namespace OpenDental {
	public partial class EmailPreviewControl:UserControl {

		///<summary>TODO: Replace this flag with a new flag on the email address object.</summary>
		private bool _isSigningEnabled=true;
		private bool _isLoading=false;
		private bool _isComposing=false;
		private EmailMessage _emailMessage=null;		
		private X509Certificate2 _certSig=null;
		private List<EmailAttach> _listEmailAttachDisplayed=null;
		///<summary>Used when sending to get Clinic.</summary>
		private Patient _patCur=null;
		///<summary>If the message is an html email with images, then this list contains the raw image mime parts.  The user must give permission before converting these to images, for security purposes.  Gmail also does this with images, for example.</summary>
		private List<Health.Direct.Common.Mime.MimeEntity> _listImageParts=null;
		///<summary>If the message is an html email with attached applications (can be images also), then this list contains the raw image mime parts.  The user must give permission before converting these to images, for security purposes.  Gmail also does this with images, for example.</summary>
		private List<MimeEntity> _listAppParts=null;
    ///<summary>The list of email addresses in email setup. Primarly used for matching From address to internal EmailAddress.</summary>
		private List<EmailAddress> _listEmailAddresses;
		///<summary>Can be null. Should be set externally before showing this control to the user. Otherwise will attempt to match _emailMessage.FromAddress
		///against an EmailAddress object found in Email Setup.</summary>
		public EmailAddress EmailAddressPreview=null;
		///<summary>List of recommended emails to be show on email releated textboxes.
		///Usually history of all email messages regarding a specific inbox/outbox.
		///These email messages are "light" such that the do not include body text or raw email data.
		///These messages must be "light" in order to prevent from bloating memory.</summary>
		private List<string> _listHistoricContacts=new List<string>();
		///<summary>True if the thread has finished filling _listHistoricContacts.</summary>
		private bool _hasSetHistoricContacts;
		///<summary>List of all HideInFlags, except None.</summary>
		private List<HideInFlags> _listHideInFlags=new List<HideInFlags>();
		///<summary>True when the control is being used to preview an email.</summary>
		private bool _isPreview;
		///<summary>used specifically for checking if the message string in the body text has changed
		private bool _hasMessageTextChanged=false;
		///<summary>string to return the updated htmlText for a composing or sent email. webBrowser.DocumentText doesn't always work. </summary>
		public string HtmlText;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public bool IsComposing { get { return _isComposing; } }
		public string Subject { get { return textSubject.Text; } set { textSubject.Text=value; } }
		public string BodyText { get { return textBodyText.Text; } set { textBodyText.Text=value; } }
		///<summary>returns true if the current view is the HTML web browser, false if it is currently on the plain text box.</summary>
		public bool IsHtml { get {return webBrowser.Visible; } }
		public string FromAddress { get { return textFromAddress.Text; } }
		public string ToAddress { get { return textToAddress.Text; } set { textToAddress.Text=value; } }
		public string CcAddress { get { return textCcAddress.Text; } set { textCcAddress.Text=value; } }
		public string BccAddress { get { return textBccAddress.Text; } set { textBccAddress.Text=value; } }
		public bool IsSigned { get { return (_isSigningEnabled && _certSig!=null); } }
		public bool HasAttachments { get { return _emailMessage.Attachments.Count>0; } }
		public bool IsPreview { get { return this._isPreview; } set { this._isPreview=value; } }
		
		public long PatNum {
			get { 
				if(_patCur!=null) {
					return _patCur.PatNum;
				} 
				return 0;
			}
		}
		public long ClinicNum {
			get { 
				if(_patCur!=null) {
					return _patCur.ClinicNum;
				} 
				return 0;
			}
		}

		public X509Certificate2 Signature {
			get {
				if(IsSigned) {
					return _certSig;
				}
				return null;
			}
		}

		///<summary>Attempts to retrive the EmailAddress corresponding to the address in textFromAddress.
		///Use fromAddressOverride when the controls textFromAddress.Text has not been loaded yet.</summary>
		public FromAddressMatchResult TryGetFromEmailAddress(out EmailAddress emailAddress,string fromAddressOverride="")
		{
			emailAddress=null;
			string fromAddressText=textFromAddress.Text;
			if(!string.IsNullOrEmpty(fromAddressOverride)) {
				fromAddressText=fromAddressOverride;
			}
			List<EmailAddress> listMatchingEmailAddresses=new List<EmailAddress>();
			#region EmailAddressPreview.EmailAddressNum based matching.
			if(EmailAddressPreview!=null
				//User may have manually modified the From address, force old matching logic when EmailAddressPreview.EmailAddressNum can't be trusted.
				&& EmailAddressPreview.GetFrom()==fromAddressText)
			{
				//Sending email address should be set before loading this control.  This is the best approach to matching email account.
				listMatchingEmailAddresses=_listEmailAddresses.FindAll(x => x.EmailAddressNum==EmailAddressPreview.EmailAddressNum);
			}
			#endregion
			#region Text based matching
			else {
				foreach(EmailAddress emailAccount in _listEmailAddresses) {
					//Less reliable, but works in most email account setups.
					string senderAddress=emailAccount.SenderAddress.Trim().ToLower();
					string emailUserName=emailAccount.EmailUsername.Trim().ToLower();
					string fromAddress=fromAddressText.Trim().ToLower();
					if(((senderAddress!="" && fromAddress.Contains(senderAddress))
						|| (emailUserName!="" && fromAddress.Contains(emailUserName))
						|| (fromAddress!="" && (emailUserName.Contains(fromAddress) || senderAddress.Contains(fromAddress))))
						&& (!listMatchingEmailAddresses.Contains(emailAccount))) 
					{
						listMatchingEmailAddresses.Add(emailAccount);//Found an emailaddress that is probably right.
					}
				}
			}
			#endregion
			FromAddressMatchResult result;
			switch(listMatchingEmailAddresses.Count) {
				case 0:
					emailAddress=null;
					result=FromAddressMatchResult.Failed;
					break;
				case 1:
					emailAddress=listMatchingEmailAddresses[0];
					result=FromAddressMatchResult.Success;
					break;
				default:
					emailAddress=listMatchingEmailAddresses[0];
					result=FromAddressMatchResult.Multi;
					break;
			}
			return result;
		}

		public EmailPreviewControl() {
			InitializeComponent();
			gridAttachments.ContextMenu=contextMenuAttachments;
		}

		///<summary>Loads the given emailMessage into the control.
		///Set listEmailMessages to messages to be considered for the auto complete contacts pop up.  When null will query.</summary>
    public void LoadEmailMessage(EmailMessage emailMessage,List<EmailMessage> listHistoricEmailMessages=null) {
			Cursor=Cursors.WaitCursor;
			_emailMessage=emailMessage;
			_patCur=Patients.GetPat(_emailMessage.PatNum);//we could just as easily pass this in.
			if(EmailMessages.IsUnsent(_emailMessage.SentOrReceived)) {//Composing a message
				_isComposing=true;
				if(_isSigningEnabled) {
					SetSig(EmailMessages.GetCertFromPrivateStore(_emailMessage.FromAddress));
				}
				_emailMessage.UserNum=Security.CurUser.UserNum;//UserNum is also updated when sent. Setting here to display when composing.
			}
			else {//sent or received (not composing)
				//For all email received or sent types, we disable most of the controls and put the window into a mostly read-only state.
				//There is no reason a user should ever edit a received message.
				//The user can copy the content and send a new email if needed (to mimic forwarding until we add the forwarding feature).
				_isComposing=false;
				textMsgDateTime.Text=_emailMessage.MsgDateTime.ToString();
				textMsgDateTime.ForeColor=Color.Black;
				gridAttachments.AddButtonEnabled=false;
				textFromAddress.ReadOnly=true;
				textToAddress.ReadOnly=true;
				textCcAddress.ReadOnly=true;
				textBccAddress.ReadOnly=true;
				textSubject.ReadOnly=true;
				textSubject.SpellCheckIsEnabled=false;//Prevents slowness resizing the window, because spell checker runs each time resize event is fired.
				textBodyText.ReadOnly=true;
				textBodyText.SpellCheckIsEnabled=false;//Prevents slowness resizing the window, because spell checker runs each time resize event is fired.
				butAccountPicker.Visible=false;
				LayoutManager.MoveWidth(textFromAddress,textCcAddress.Width);//Match the size of Cc Address.
				textFromAddress.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					| System.Windows.Forms.AnchorStyles.Right)));//Change the anchors to accommodate.
			}
			textSentOrReceived.Text=_emailMessage.SentOrReceived.ToString();
			textFromAddress.Text=_emailMessage.FromAddress;
			textToAddress.Text=_emailMessage.ToAddress;
			textCcAddress.Text=_emailMessage.CcAddress;
			textBccAddress.Text=_emailMessage.BccAddress; //if you send an email to yourself, you'll be able to see everyone in the bcc field.
			textSubject.Text=_emailMessage.Subject;
			textBodyText.Visible=true;
			webBrowser.Visible=false;
			if(EmailMessages.IsReceived(_emailMessage.SentOrReceived)) {
				List<List<MimeEntity>> listMimeParts=
					EmailMessages.GetMimePartsForMimeTypes(_emailMessage.RawEmailIn,EmailAddressPreview,
					"text/html","text/plain","image/","application/octet-stream");//Adding application/octet-stream as sometimes images are sent in this format
				List<MimeEntity> listHtmlParts=listMimeParts[0];//If RawEmailIn is blank, then this list will also be blank (ex Secure Web Mail messages).
				List<MimeEntity> listTextParts=listMimeParts[1];//If RawEmailIn is blank, then this list will also be blank (ex Secure Web Mail messages).
				_listImageParts=listMimeParts[2];//If RawEmailIn is blank, then this list will also be blank (ex Secure Web Mail messages).
				_listAppParts=listMimeParts[3];//If RawEmailIn is blank, then this list will also be blank (ex Secure Web Mail messages).
				if(listHtmlParts.Count>0) {//Html body found.
					textBodyText.Visible=false;
					_isLoading=true;
					try {
						webBrowser.DocumentText=EmailMessages.ProcessMimeTextPart(listHtmlParts[0]);
					}
					catch(ApplicationException ex) {
						webBrowser.DocumentText="Improperly formatted email. Error displaying email: "+ex.Message;
					}
					LayoutManager.MoveLocation(webBrowser,textBodyText.Location);
					LayoutManager.MoveSize(webBrowser,textBodyText.Size);
					webBrowser.Anchor=textBodyText.Anchor;
					webBrowser.Visible=true;
					if(!_listImageParts.IsNullOrEmpty() || !_listAppParts.IsNullOrEmpty()) {
						butShowImages.Visible=true;
					}
				}
				else if(listTextParts.Count>0) {//No html body found, however one specific mime part is for viewing in text only.					
					textBodyText.Text=EmailMessages.ProcessMimeTextPart(listTextParts[0]);
				}
				else {//No html body found and no text body found.  Last resort.  Show all mime parts which are not attachments (ugly).
					textBodyText.Text=_emailMessage.BodyText;//This version of the body text includes all non-attachment mime parts.
				}
				lableUserName.Visible=false;
				textUserName.Visible=false;
			}
			else {//Sent or Unsent/Saved.
				textBodyText.Text=_emailMessage.BodyText;//Show the body text exactly as typed by the user.
				if(EmailMessages.IsHtmlEmail(_emailMessage.HtmlType)) {
					try {
						if(_emailMessage.HtmlType==EmailType.RawHtml) {
							HtmlText=textBodyText.Text;
						}
						else {
							HtmlText=MarkupEdit.TranslateToXhtml(textBodyText.Text,false,false,true);//show the user's text in web browswer with HTML
						}
						InitializeHtmlEmail();//web browser needs to be set and made visible to show the translated text.
					}
					catch(Exception ex) {
						FriendlyException.Show("Error loading HTML.",ex);
					}
				}
				lableUserName.Visible=true;
				textUserName.Visible=true;
				textUserName.Text=(Userods.GetName(_emailMessage.UserNum));//Blank if 0.
			}
			FillAttachments();
			if(IsComposing) {
				LoadEmailAddresses(ClinicNum);
				LoadSig();
				if(PrefC.GetBool(PrefName.EnableEmailAddressAutoComplete)) {
					SetHistoricContacts(listHistoricEmailMessages);
				}
			}
			textBodyText.Select();
			Cursor=Cursors.Default;
			if(_isPreview) {
				tabAttachmentsShowEmail.TabPages.Remove(tabShowEmail); //Do not show Hide Email tab when in Email Preview mode
			}
			else {
				InitEmailShowInListBox();
				RefreshShowIn();
			}	
		}

		///<summary>Sets the email preview window to show the web browser for viewing HTML.</summary>
		public void InitializeHtmlEmail() {
			try {
				_isLoading=true;
				textBodyText.Visible=false;
				webBrowser.Visible=true;
				LayoutManager.MoveLocation(webBrowser,textBodyText.Location);
				LayoutManager.MoveSize(webBrowser,textBodyText.Size);
				webBrowser.Anchor=textBodyText.Anchor;
				webBrowser.DocumentText=HtmlText;
				webBrowser.BringToFront();
			}
			catch(Exception ex) {
				ex.DoNothing();
				//invalid preview
			}
		}

		///<summary>Refreshes the email preview pane to show the web browser when viewing HTML and the editable text if not.</summary>
		public void RefreshView(bool isHtml,bool isRawHtml) {
			if(_hasMessageTextChanged && isHtml) {//only translate if attempting to see an HTML view. Regular text emails allowed to have 'invalid' characters
				try {
					if(isRawHtml) {
						HtmlText=textBodyText.Text;
					}
					else {
						HtmlText=MarkupEdit.TranslateToXhtml(textBodyText.Text,false,false,true);//do not aggregate for RAW HTML
					}
					_hasMessageTextChanged=false;
				}
				catch(Exception ex) {
					FriendlyException.Show("Error loading HTML.",ex);
				}
			}
			if(isHtml) {
				InitializeHtmlEmail();
			}
			else {
				//load the plain text
				webBrowser.Visible=false;
				textBodyText.Visible=true;
				textBodyText.BringToFront();
			}
		}

		public void LoadEmailAddresses(long clinicNum) {
      //emails to include: 
      //1. Default Practice/Clinic
      //2. Me
      //3. All other email addresses not tied to a user
      _listEmailAddresses=new List<EmailAddress>();
      EmailAddress emailAddressDefault=EmailAddresses.GetByClinic(clinicNum);
      EmailAddress emailAddressMe=EmailAddresses.GetForUser(Security.CurUser.UserNum);
      if(emailAddressDefault!=null) {
        _listEmailAddresses.Add(emailAddressDefault);
      }
      if(emailAddressMe!=null) {
        _listEmailAddresses.Add(emailAddressMe);
      }
			foreach(EmailAddress emailCur in EmailAddresses.GetDeepCopy()) {
        if((emailAddressDefault!=null && emailCur.EmailUsername==emailAddressDefault.EmailUsername)
          || (emailAddressMe!=null && emailCur.EmailUsername==emailAddressMe.EmailUsername)) {
          continue;
        }
        _listEmailAddresses.Add(emailCur);
      }
		}
		
		private void LoadSig() {
			if(!_isComposing || !_isSigningEnabled) {
				return;
			}
			EmailAddress emailAddressDefault=EmailAddresses.GetByClinic(ClinicNum);
			if(emailAddressDefault!=null) {//Must have a default emailaddress to be allowed to set a signature/cert.  Presumably 
				EmailAddress emailAddressSelected=null;
				if(TryGetFromEmailAddress(out emailAddressSelected)==FromAddressMatchResult.Failed) {
					return;
				}
				SetSig(EmailMessages.GetCertFromPrivateStore(emailAddressSelected.EmailUsername));
			}
		}
		
		///<summary>Returns distinct list of email strings to be recommended to user.
		///Splits all email address fields into a large list of individual addresses into one large distinct list.
		///When given list is null, will run query.</summary>
		private void SetHistoricContacts(List<EmailMessage> listEmailMessages) {
			if(EmailAddressPreview==null) {
				//Only null when failed to match from address. If we do not know the from address then we can't load anything useful.
				return;
			}
			EmailAddress emailAddressPreview=EmailAddressPreview.Clone();
			ODThread thread=new ODThread(o => {
				List<string> listHistoricContacts;
				if(listEmailMessages==null) {
					listHistoricContacts=EmailMessages.GetHistoricalEmailAddresses(emailAddressPreview);
				}
				else {
					listHistoricContacts=EmailMessages.GetAddressesFromMessages(listEmailMessages);
				}
				this.InvokeIfRequired(() => {
					_listHistoricContacts=listHistoricContacts;
					_hasSetHistoricContacts=true;
				});
			});
			thread.Name="SetHistoricContacts";
			thread.Start();
		}

		#region HideInFlags
		///<summary>Loads HideInFlags descriptions into listShowIn</summary>
		private void InitEmailShowInListBox() {
			if(listShowIn.Items.Count!=0) {//Already initialized
				return;
			}
			_listHideInFlags=Enum.GetValues(typeof(HideInFlags)).Cast<HideInFlags>().Where(x => x!=HideInFlags.None).ToList();
			for(int i=0;i<_listHideInFlags.Count;i++) {
				listShowIn.Items.Add(Lan.g("enumHideInFlags",_listHideInFlags[i].GetDescription()));
			}
		}
		
		///<summary>Refreshes listShowIn according to current emailmessage selections in GridSent/GridInbox</summary>
		private void RefreshShowIn() {
			if(_isPreview) { //listbox not displayed in preview mode, no need to update
				return;
			}
			listShowIn.SetAll(true); //Reset to default(all selected)
			for(int i=0;i<_listHideInFlags.Count;i++) {
				if(_emailMessage.HideIn.HasFlag(_listHideInFlags[i])) {
					listShowIn.SetSelected(i,false); //clear selection
				}
			}
		}
		
		/// <summary>Builds a HideInFlags flag from listShowIn</summary>
		public HideInFlags GetHideInFlagFromShowIn() {
			HideInFlags flag=HideInFlags.None;
			for(int i=0;i<listShowIn.SelectedIndices.Count;i++) {
				flag|=_listHideInFlags[listShowIn.SelectedIndices[i]];
			}
			return (HideInFlags)((HideInFlags)_listHideInFlags.Sum(x => (int)x)-flag); //UI and backend logic are flipped (show vs hide).
		}
		#endregion

		#region Attachments

		public void FillAttachments() {
			_listEmailAttachDisplayed=new List<EmailAttach>();
			if(!_isComposing) {
				SetSig(null);
			}
			gridAttachments.BeginUpdate();
			gridAttachments.ListGridRows.Clear();
			gridAttachments.ListGridColumns.Clear();
			gridAttachments.ListGridColumns.Add(new OpenDental.UI.GridColumn("",90){ IsWidthDynamic=true });//No name column, since there is only one column.
			for(int i=0;i<_emailMessage.Attachments.Count;i++) {
				if(_emailMessage.Attachments[i].DisplayedFileName.ToLower()=="smime.p7s") {
					if(!_isComposing) {
						string smimeP7sFilePath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_emailMessage.Attachments[i].ActualFileName);
						string localFile=PrefC.GetRandomTempFile(".p7s");
						FileAtoZ.Copy(smimeP7sFilePath,localFile,FileAtoZSourceDestination.AtoZToLocal,doOverwrite:true);
						SetSig(EmailMessages.GetEmailSignatureFromSmimeP7sFile(localFile));
					}
					//Do not display email signatures in the attachment list, because "smime.p7s" has no meaning to a user
					//Also, Windows will install the smime.p7s into an useless place in the Windows certificate store.
					continue;
				}
				OpenDental.UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_emailMessage.Attachments[i].DisplayedFileName);
				gridAttachments.ListGridRows.Add(row);
				_listEmailAttachDisplayed.Add(_emailMessage.Attachments[i]);
			}
			gridAttachments.EndUpdate();
			if(gridAttachments.ListGridRows.Count>0) {
				gridAttachments.SetSelected(0,true);
			}
		}

		private void contextMenuAttachments_Popup(object sender,EventArgs e) {
			menuItemOpen.Enabled=false;
			menuItemRename.Enabled=false;
			menuItemRemove.Enabled=false;
			if(gridAttachments.SelectedIndices.Length>0) {
				menuItemOpen.Enabled=true;
			}
			if(gridAttachments.SelectedIndices.Length>0 && _isComposing) {
				menuItemRename.Enabled=true;
				menuItemRemove.Enabled=true;
			}
		}

		private void menuItemOpen_Click(object sender,EventArgs e) {
			OpenFile();
		}

		private void menuItemRename_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox(Lan.g(this,"Filename"));
			EmailAttach emailAttach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			input.textResult.Text=emailAttach.DisplayedFileName;
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			emailAttach.DisplayedFileName=input.textResult.Text;
			FillAttachments();
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			EmailAttach emailAttach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			_emailMessage.Attachments.Remove(emailAttach);
			FillAttachments();
		}

		private void gridAttachments_MouseDown(object sender,MouseEventArgs e) {
			//A right click also needs to select an items so that the context menu will work properly.
			if(e.Button==MouseButtons.Right) {
				int clickedIndex=gridAttachments.PointToRow(e.Y);
				if(clickedIndex!=-1) {
					gridAttachments.SetSelected(clickedIndex,true);
				}
			}
		}

		private void gridAttachments_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			OpenFile();
		}

		private void OpenFile() {
			EmailAttach emailAttach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			string strFilePathAttach=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName);
			try {
				if(EhrCCD.IsCcdEmailAttachment(emailAttach)) {
					string strTextXml=FileAtoZ.ReadAllText(strFilePathAttach);
					if(EhrCCD.IsCCD(strTextXml)) {
						Patient patEmail=null;//Will be null for most email messages.
						if(_emailMessage.SentOrReceived==EmailSentOrReceived.ReadDirect || _emailMessage.SentOrReceived==EmailSentOrReceived.ReceivedDirect) {
							patEmail=_patCur;//Only allow reconcile if received via Direct.
						}
						string strAlterateFilPathXslCCD="";
						//Try to find a corresponding stylesheet. This will only be used in the event that the default stylesheet cannot be loaded from the EHR dll.
						for(int i=0;i<_listEmailAttachDisplayed.Count;i++) {
							if(Path.GetExtension(_listEmailAttachDisplayed[i].ActualFileName).ToLower()==".xsl") {
								strAlterateFilPathXslCCD=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_listEmailAttachDisplayed[i].ActualFileName);
								break;
							}
						}
						FormEhrSummaryOfCare.DisplayCCD(strTextXml,patEmail,strAlterateFilPathXslCCD);
						return;
					}
				}
				else if(IsORU_R01message(strFilePathAttach)) {
					if(DataConnection.DBtype==DatabaseType.Oracle) {
						MsgBox.Show(this,"Labs not supported with Oracle.  Opening raw file instead.");
					}
					else {
						using FormEhrLabOrderImport FormELOI =new FormEhrLabOrderImport();
						FormELOI.Hl7LabMessage=FileAtoZ.ReadAllText(strFilePathAttach);
						FormELOI.ShowDialog();
						return;
					}
				}
				FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName),emailAttach.DisplayedFileName);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		
		private void ButAdd_Click(object sender, EventArgs e){
			if(!gridAttachments.AddButtonEnabled) {
				MsgBox.Show(this,"Attachments cannot be modified on historical email.");
				return;
			}
			_emailMessage.Attachments.AddRange(EmailAttachL.PickAttachments(_patCur));
			FillAttachments();
		}

		///<summary>Attempts to parse message and detects if it is an ORU_R01 HL7 message.  Returns false if it fails, or does not detect message type.</summary>
		private bool IsORU_R01message(string strFilePathAttach) {
			if(Path.GetExtension(strFilePathAttach) != "txt") {
				return false;
			}
			try {
				string[] ArrayMSHFields=FileAtoZ.ReadAllText(strFilePathAttach).Split(new string[] { "\r\n" },
					StringSplitOptions.RemoveEmptyEntries)[0].Split('|');
				if(ArrayMSHFields[8]!="ORU^R01^ORU_R01") {
					return false;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		#endregion Attachments

		#region Signature

		private void butSig_Click(object sender,EventArgs e) {
			using FormEmailDigitalSignature form=new FormEmailDigitalSignature(_certSig);
			if(form.ShowDialog()==DialogResult.OK) {
				//If the user just added trust, then refresh to pull the newly added certificate into the memory cache.
				EmailMessages.RefreshCertStoreExternal(EmailAddressPreview);
			}
		}

		private void SetSig(X509Certificate2 certSig) {
			_certSig=certSig;
			labelSignedBy.Visible=false;
			textSignedBy.Visible=false;
			textSignedBy.Text="";
			butSig.Visible=false;
			textFromAddress.ReadOnly=false;
			if(certSig!=null) {
				labelSignedBy.Visible=true;
				textSignedBy.Visible=true;
				textSignedBy.Text=EmailNameResolver.GetCertSubjectName(certSig);
				//Show the user that, if the message is signed, then the sender will always look like the address on the certificate,
				//even if they have a Sender Address setup.  Otherwise we would be misrepresenting how the Sender Address feature works.
				textFromAddress.Text=textSignedBy.Text;
				textFromAddress.ReadOnly=true;
				butSig.Visible=true;
			}
		}

		#endregion Signature

		#region Body

		public void LoadTemplate(string subject,string bodyText,List<EmailAttach> attachments) {
			List<Appointment> listApts=Appointments.GetFutureSchedApts(PatNum);
			Appointment aptNext=null;
			if(listApts.Count > 0){
				aptNext=listApts[0]; //next sched appt. If none, null.
			}
			Clinic clinic=Clinics.GetClinic(ClinicNum);
			Subject=ReplaceTemplateFields(subject,_patCur,aptNext,clinic);;
			BodyText=ReplaceTemplateFields(bodyText,_patCur,aptNext,clinic);
			_emailMessage.Attachments.AddRange(attachments);
			_hasMessageTextChanged=true;
			FillAttachments();
		}

		///<summary></summary>
		public static string ReplaceTemplateFields(string templateText,Patient pat,Appointment aptNext,Clinic clinic) {
			//patient information
			templateText=Patients.ReplacePatient(templateText,pat);
			//Guarantor Information
			templateText=Patients.ReplaceGuarantor(templateText,pat);
			//Family Information
			templateText=Family.ReplaceFamily(templateText,pat);
			//Next Scheduled Appointment Information
			templateText=Appointments.ReplaceAppointment(templateText,aptNext); //handles null nextApts.
			//Currently Logged in User Information
			templateText=ReplaceTags.ReplaceUser(templateText,Security.CurUser);
			//Clinic Information
			templateText=Clinics.ReplaceOffice(templateText,clinic);
			//Misc Information
			templateText=ReplaceTags.ReplaceMisc(templateText);
			//Referral Information
			templateText=Referrals.ReplaceRefProvider(templateText,pat);
			//Recall Information
			return Recalls.ReplaceRecall(templateText,pat);
		}

		///<summary>Accepts a list of MimeEntity and parses it for paths, htmls and other key factors. It will test that the resulting parts
		///have a valid image extension. If they do, they are saved to a temporary web page for presentation to the user. If they are not
		///then they are disreguarded an a message is sent to the user warning them of potentially malicious content.</summary>
		private string ParseAndSaveAttachement(string htmlFolderPath,string html,List<MimeEntity> listParts) {
				bool hasDangerousAttachment=false;
				foreach(MimeEntity entity in listParts) {
					string contentId=EmailMessages.GetMimeImageContentId(entity);
					string fileName=EmailMessages.GetMimeImageFileName(entity);
					//Only show image types.  Otherwise, prompt user that potentially dangerous code is attached to the email and will not be shown.
					if(!ImageStore.HasImageExtension(fileName)) {//Check file format against known image format extensions.
						hasDangerousAttachment=true;
						continue;
					}
					html=html.Replace("cid:"+contentId,fileName);
					EmailAttach attachment=_listEmailAttachDisplayed.FirstOrDefault(x => x.DisplayedFileName.ToLower().Trim()==fileName.ToLower().Trim());
					//The path and filename must be directly accessed from the EmailAttach object in question, otherwise subsequent code would have accessed
					//an empty bodied message and never shown an image.
					EmailMessages.SaveMimeImageToFile(entity,htmlFolderPath,attachment?.ActualFileName);
				}
				if(hasDangerousAttachment) {
					//Since the extension is not within the image formats it may contain mallware and we will not parse or present it.
					MsgBox.Show("This message contains some elements that may not be safe and will not be loaded.");
				}
				return html;
		}

		private void butShowImages_Click(object sender,EventArgs e) {
			try {
				//We need a folder in order to place the images beside the html file in order for the relative image paths to work correctly.
				string htmlFolderPath=ODFileUtils.CreateRandomFolder(PrefC.GetTempFolderPath());//Throws exceptions.
				string filePathHtml=ODFileUtils.CreateRandomFile(htmlFolderPath,".html");
				string html=webBrowser.DocumentText;
				List<MimeEntity> listMimeEntries=new List<MimeEntity>();
				listMimeEntries.AddRange(_listAppParts);
				listMimeEntries.AddRange(_listImageParts);
				html=ParseAndSaveAttachement(htmlFolderPath,html,listMimeEntries);
				File.WriteAllText(filePathHtml,html);
				_isLoading=true;
				webBrowser.Navigate(filePathHtml);
				butShowImages.Visible=false;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}

		private void textBodyText_KeyDown(object sender,KeyEventArgs e) {
			_hasMessageTextChanged=true;
		}

		private void webBrowser_Navigating(object sender,WebBrowserNavigatingEventArgs e) {
			if(_isLoading) {
				return;
			}
			e.Cancel=true;//Cancel browser navigation (for links clicked within the email message).
			if(e.Url.AbsoluteUri=="about:blank") {
				return;
			}
			//if user did not specify a valid url beginning with http:// then the event args would make the url start with "about:" 
			//ex: about:www.google.com and then would ask the user to get a separate app to open the link since it is unrecognized
			string url=e.Url.ToString();
			if(url.StartsWith("about")) {
				url=url.Replace("about:","http://");
			}
			Process.Start(url);//Instead launch the URL into a new default browser window.
		}

		private void webBrowser_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			_isLoading=false;
		}

		#endregion Body

		///<summary>Saves the UI input values into the emailMessage.  Allowed to save message with invalid fields, so no validation here.</summary>
		public void SaveMsg(EmailMessage emailMessage) {
			if(_isComposing) {
				emailMessage.FromAddress=textFromAddress.Text;
				emailMessage.ToAddress=textToAddress.Text;
				emailMessage.CcAddress=textCcAddress.Text;
				emailMessage.BccAddress=textBccAddress.Text;
				emailMessage.Subject=textSubject.Text;
				emailMessage.BodyText=textBodyText.Text;
				emailMessage.MsgDateTime=DateTime.Now;
				emailMessage.SentOrReceived=_emailMessage.SentOrReceived;//Status does not ever change.
				emailMessage.HtmlType=_emailMessage.HtmlType;
			}
			emailMessage.HideIn=GetHideInFlagFromShowIn();//User can edit hidden flags for all email messages, both incoming/outgoing.
		}
		
		private void butAccountPicker_Click(object sender,EventArgs e) {
			PickEmailAccount();
		}

		public EmailAddress PickEmailAccount() {
			using FormEmailAddresses formEA=new FormEmailAddresses();
			formEA.IsSelectionMode=true;
			formEA.ShowDialog();
			if(formEA.DialogResult==DialogResult.OK) {
				EmailAddress emailAccountSelected=_listEmailAddresses.Find(x => x.EmailAddressNum==formEA.EmailAddressNum);
				if(emailAccountSelected!=null) {
					EmailAddressPreview=emailAccountSelected;
				}
				else {
					MsgBox.Show(this,"Error selecting email account.");
					return null;
				}
				textFromAddress.Text=EmailAddressPreview.GetFrom();
				if(!_isComposing || !_isSigningEnabled) {
					return null;
				}
				SetSig(EmailMessages.GetCertFromPrivateStore(emailAccountSelected.EmailUsername));
			}
			else {
				EmailAddressPreview=null;
			}
			return EmailAddressPreview;
		}
		
		private void emailAddress_KeyDown(object sender,KeyEventArgs e) {
			if(System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift)) 
			{
				return;//Lets Ctrl+C and Ctrl+V etc... be processed by system.
			}
			if(char.IsLetterOrDigit((char)e.KeyCode) || ListTools.In(e.KeyCode,Keys.Enter,Keys.Up,Keys.Down,Keys.Escape)) {
				e.Handled=true;//We set e.Handled=true so that the key down event is not processed in the base class for any other purpose.
			}
		}

		private void emailAddress_KeyUp(object sender,KeyEventArgs e) {
			if(char.IsLetterOrDigit((char)e.KeyCode) || ListTools.In(e.KeyCode,Keys.Enter,Keys.Up,Keys.Down,Keys.Escape,Keys.Back)) {
				RecommendedEmailHelper(((ODtextBox)sender),e.KeyCode);
			}
		}

		///<summary>Creates a list box under given textBox filled with filtered list of recommended emails based on textBox.Text values.
		///Key is used to navigate list indirectly.</summary>
		private void RecommendedEmailHelper(ODtextBox textBox,Keys key) {
			if(!PrefC.GetBool(PrefName.EnableEmailAddressAutoComplete) || (_hasSetHistoricContacts && _listHistoricContacts.Count==0)) {//No recommendations to show.
				return;
			}
			//The passed in textBox's tag points to the grid of options.
			//The created grid's tag will point to the textBox.
			if(textBox.Tag==null) {
				textBox.Tag=new GridOD() {
					TranslationName="",
				};
			}
			GridOD gridContacts=(GridOD)textBox.Tag;
			//textBox.Text could contain multiple email addresses.
			//We only want to grab the last few characters as the filter string.
			//email@od.com,email2@od.com,emai => "emai" is the filter.
			//When there is no comma, will just use what is currently in the textbox.
			string emailFilter=textBox.Text.ToLower().Split(',').Last();
			if(emailFilter.Length<2) {//Require at least 2 characters for now.
				gridContacts.Hide();//Even if not showing .Hide() won't harm anything.
				textBox.Tag=null;//Reset tag so that initial logic runs again.
				return;
			}
			#region Key navigation and filtering
			switch(key) {
				case Keys.Enter://Select currently highlighted recommendation.
					if(gridContacts.ListGridRows.Count==0) {
						return;
					}
					CloseAndSetRecommendedContacts(gridContacts,true);
					return;
				case Keys.Up://Navigate the recommendations from the textBox indirectly.
					if(gridContacts.ListGridRows.Count==0) {
						return;
					}
					//gridContacts is multi select. We are navigating 1 row at a time so clear and set the selected index.
					int index=Math.Max(gridContacts.GetSelectedIndex()-1,0);
					gridContacts.SetAll(false);
					gridContacts.SetSelected(new int[] { index },true);
					gridContacts.ScrollToIndex(index);
					break;
				case Keys.Down://Navigate the recommendations from the textBox indirectly.
					if(gridContacts.ListGridRows.Count==0) {
						return;
					}
					//gridContacts is multi select. We are navigating 1 row at a time so clear and set the selected index.
					index=Math.Min(gridContacts.GetSelectedIndex()+1,gridContacts.ListGridRows.Count-1);
					gridContacts.SetAll(false);
					gridContacts.SetSelected(new int[] { index },true);
					gridContacts.ScrollToIndex(index);
					break;
				default:
					#region Filter recommendations
					List<string> listFilteredContacts;
					if(_hasSetHistoricContacts) {
						listFilteredContacts=_listHistoricContacts.FindAll(x => x.ToLower().Contains(emailFilter.ToLower()));
					}
					else {//The thread is still filling historic contacts.
						listFilteredContacts=new List<string> { Lans.g(this,"Loading contacts...") };
					}
					if(listFilteredContacts.Count==0) {
						gridContacts.Hide();//No options to show so make sure and hide the list box
						textBox.Tag=null;//Reset tag.
						return;
					}
					listFilteredContacts.Sort();
					gridContacts.BeginUpdate();
					if(gridContacts.ListGridColumns.Count==0) {//First time loading.
						gridContacts.ListGridColumns.Add(new GridColumn());
					}
					gridContacts.ListGridRows.Clear();
					foreach(string email in listFilteredContacts) {
						GridRow row=new GridRow(email);
						row.Tag=email;
						gridContacts.ListGridRows.Add(row);
					}
					gridContacts.EndUpdate();
					gridContacts.SetSelected(0,true);//Force a selection.
					#endregion
					break;
			}
			#endregion
			if(gridContacts.Tag!=null) {//Already initialized
				return;
			}
			//When the text box losses focus, we close/hide the grid.
			//TextBox_LostFocus event fires after the EmailAuto_Click event.
			textBox.Leave+=TextBox_LostFocus;
			#region Grid Init
			gridContacts.HeadersVisible=false;
			gridContacts.SelectionMode=GridSelectionMode.MultiExtended;
			gridContacts.MouseClick+=EmailAuto_Click;
			gridContacts.Tag=textBox;
			gridContacts.TitleVisible=false;
			LayoutManager.Add(gridContacts,this);
			gridContacts.BringToFront();
			Point menuPosition=textBox.Location;
			menuPosition.X+=10;
			menuPosition.Y+=textBox.Height-1;
			LayoutManager.MoveLocation(gridContacts,menuPosition);
			LayoutManager.MoveWidth(gridContacts,(int)(textBox.Width*0.75));
			gridContacts.SetSelected(0,true);
			#endregion
			gridContacts.Show();
		}

		///<summary>Fires after EmailAuto_Click()</summary>
		private void TextBox_LostFocus(object sender,EventArgs e) {
			ODtextBox textBox=((ODtextBox)sender);
			textBox.LostFocus-=TextBox_LostFocus;//Stops EventHandler from firing multiple times.
			if(textBox.Tag==null || this.ActiveControl==(GridOD)textBox.Tag) {//The contacts grid handles its own events.
				return;//Prevent from selecting email addresses twice.
			}
			CloseAndSetRecommendedContacts((GridOD)textBox.Tag,false);
		}

		///<summary>Fires before TextBox_LostFocus()</summary>
		private void EmailAuto_Click(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right//Let base ODGrid handle right clicks, do not hide.
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)
				|| System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift))
			{
				//ODGrid selection keys.
				//Set focus back to textbox so user can continue to type and navigate.
				((ODtextBox)((GridOD)sender).Tag).Focus();
				return;
			}
			CloseAndSetRecommendedContacts((GridOD)sender);
		}

		///<summary>Resets tags to null and hides the given grid.
		///If isSelectionMade is true, will set textBox.Text to selected item.</summary>
		private void CloseAndSetRecommendedContacts(GridOD grid,bool isSelectionMade=true) {
			ODtextBox textBox=((ODtextBox)grid?.Tag??null);
			if(textBox==null) {
				//Done for a bug from TextBox_LostFocus where textBox was null, could be cuase by the form closing and controls being disposed?
				return;
			}
			textBox.Tag=null;
			grid.Hide();
			if(isSelectionMade) {
				int index=textBox.Text.LastIndexOf(',');//-1 if not found.
				if(index==-1) {//The selected email is the first email being placed in our textbox.
					textBox.Text=string.Join(",",grid.SelectedGridRows.Select(x => ((string)x.Tag)).ToList());
				}
				else{//Adding multiple emails.
					textBox.Text=textBox.Text.Remove(index+1,textBox.Text.Length-index-1);//Remove filter characters
					textBox.Text+=string.Join(",",grid.SelectedGridRows.Select(x => ((string)x.Tag)).ToList());//Replace with selected email
				}
			}
			textBox.Focus();//Ensures that auto complete textbox maintains focus after auto complete.
			textBox.SelectionStart=textBox.Text.Length;//Moves cursor to end of the text in the textbox.
		}

	
	}

	public enum FromAddressMatchResult {
			Failed,
			Success,
			Multi
		}
}
