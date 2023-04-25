using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	///<summary>DialogResult will be Abort if message was unable to be read. 
	///If message is read successfully (Ok or Cancel), then caller is responsible for updating SentOrReceived to read (where applicable).</summary>
	public partial class FormWebMailMessageEdit:FormODBase {
		private EmailMessage _emailMessageSecure;
		private EmailMessage _emailMessageInsecure;
		private EmailAddress _emailAddressSender;
		///<summary>Will be false if it is not possible to send an insecure email message to the patient.</summary>
		private bool _allowSendNotificationMessage=true;
		///<summary>If viewing existing message this list will only contain the "regarding" patient.
		///If composing, this will contain all family members of which the patient is eligible to view given PHI constraints.</summary>
		private List<Patient> _listPatients=null;
		///<summary>This is the email message that was passed into the constructor for an existing message being viewed or replied to.
		///Will be null if composing a new message.</summary>
		private EmailMessage _emailMessage;
		///<summary>Attachment objects will be set right before inserting _secureMessage into db. Until then they will be held separate.  If viewing an
		///existing message, this will hold the list of attachments sent with the message.</summary>
		private List<EmailAttach> _listEmailAttaches=new List<EmailAttach>();
		///<summary>On load, the form will be Compose mode if no email message is passed in (null).
		///Otherwise, the form will be View mode, meaning we are viewing an existing message.
		///Once the user presses the reply button, the form will reload into Reply mode.</summary>
		private WebMailMode _webMailMode;
		///<summary>List of family members of the patient who are eligible to view given PHI constraints.</summary>
		private List<Patient> _listPatientsForPHI;
		///<summary>Will contain a valid patient that this web mail should to be sent to to view in their patient portal.</summary>
		private Patient _patient;
		///<summary>The patient of which this specific Web Mail will be sent on behalf of.
		///This will typically be the same as _patCur but can differ if a "care taker" or someone else is communicating on their behalf.</summary>
		private Patient _patientRegarding;
		///<summary>The provider that this Web Mail will be sent from.  User can change this at any time.
		///If the user currently logged in is not associated to this provider then they will be prompted to enter credentials.</summary>
		private Provider _provider;
		///<summary>Set to the provider that is associated to the user currently logged in.  Null if no provider associated.</summary>
		private Provider _providerCurUser;
		///<summary>A list of all providers in the cache that have a user associated to them.</summary>
		private List<Provider> _listProviders;

		///<summary>Method returns the Web Mail body text preference.  Also replaces all replaceable variables.</summary>
		private string GetBodyTextInsecure(){
			string bodyTextInsecure=PrefC.GetString(PrefName.PatientPortalNotifyBody);
			bodyTextInsecure=bodyTextInsecure.Replace("[URL]",PrefC.GetString(PrefName.PatientPortalURL));
			return bodyTextInsecure;
		}

		///<summary>Default constructor. This implies that we are composing a new message, NOT replying to an existing message.</summary>
		public FormWebMailMessageEdit(long patNum) : this(patNum,null) { }

		///<summary>Use this constructor when viewing or replying to an existing message.</summary>
		public FormWebMailMessageEdit(long patNum,EmailMessage emailMessage) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailMessage=emailMessage;
			_patient=Patients.GetPat(patNum);
		}

		private void FormWebMailMessageEdit_Load(object sender,EventArgs e) {
			_webMailMode=WebMailMode.View;
			if(_emailMessage==null) {
				_webMailMode=WebMailMode.Compose;
			}
			string error="";
			if(_patient==null) {
				error+="Cannot send Web Mail to an invalid patient. ";
			}
			else {
				_listPatientsForPHI=Patients.GetPatientsForPhi(_patient.PatNum);
				if(_listPatientsForPHI.Count==0) {//Every patient should have at least one guarantor.
					error+="Patient family not setup properly.  Make sure guarantor is valid. ";
				}
			}
			//Webmail notification email address.  One notification email per database (not clinic specific).
			_emailAddressSender=EmailAddresses.GetOne(PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			if(_emailAddressSender==null
				|| _emailAddressSender.EmailAddressNum==0
				|| _emailAddressSender.EmailUsername=="") 
			{
				//No valid "Notify" email setup for this practice yet.
				error+="Invalid Web Mail Notify email.  Configure a Web Mail Notify email address in E-mail Setup. ";
			}
			_emailAddressSender=EmailAddresses.OverrideSenderAddressClinical(_emailAddressSender,_patient.ClinicNum);
			List<Userod> listUserods=Userods.GetUsersWithProviders();
			if(listUserods.Count<1) {
				error+="Cannot send Web Mail until there is at least one User associated to a Provider. ";
			}
			if(error!="") {
				MsgBox.Show(this,error);
				DialogResult=DialogResult.Abort;
				return;
			}
			if(_emailMessage!=null) {
				_patientRegarding=Patients.GetLim(_emailMessage.PatNumSubj);
			}
			if(Security.CurUser!=null) {
				_providerCurUser=Providers.GetProv(Security.CurUser.ProvNum);
			}
			List<long> listProvNums=listUserods.Select(x => x.ProvNum).Distinct().ToList();
			_listProviders=Providers.GetProvsByProvNums(listProvNums);
			LayoutMenu();
			FillFields();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		///<summary>Only called on load and on Send Click if in View mode so that the form gets put into Reply mode correctly.
		///Calling this method in more than the two places mentioned above will result in the user losing information that they have typed in.</summary>
		private void FillFields() {
			_listEmailAttaches=new List<EmailAttach>();
			_listPatients=new List<Patient>();
			comboRegardingPatient.Items.Clear();
			if(_emailMessage==null) {
				for(int i=0;i<_listPatientsForPHI.Count;i++) {
					Patient patient=_listPatientsForPHI[i];
					_listPatients.Add(patient);
					comboRegardingPatient.Items.Add(patient.GetNameFL());
					if(patient.PatNum==_patient.PatNum) {
						comboRegardingPatient.SelectedIndex=i;
					}
				}
				textTo.Text=_patient.GetNameFL();
				Provider provider=Providers.GetProv(_patient.PriProv);
				//Check to see if the patients primary provider has a user associated to them.
				if(_listProviders.Any(x => x.ProvNum==provider.ProvNum)) {
					//The patients primary provider has at least one user associated to them.
					_provider=provider;
				}
				//Now check to see if there is a provider associated to the user currently logged in.
				if(_providerCurUser!=null) {
					_provider=_providerCurUser;//Always prefer the provider that is associated to the user currently logged in.
				}
				textFrom.Text=(_provider==null) ? "" : _provider.GetFormalName();
			}
			else {//An existing email has been passed in.
				_provider=Providers.GetProv(_emailMessage.ProvNumWebMail);
				_listPatients.Add(_patientRegarding);
				comboRegardingPatient.Items.Add(_patientRegarding.GetNameFL());
				comboRegardingPatient.SelectedIndex=0;
				textSubject.Text=_emailMessage.Subject;
				textBody.Text=_emailMessage.BodyText;
				if(_webMailMode==WebMailMode.Reply) {
					//Make the "new" subject look like a reply by tacking on the RE: abbreviation.
					if(_emailMessage.Subject.IndexOf("RE:")!=0) {
						textSubject.Text="RE: "+textSubject.Text;
					}
					//Preserve the "conversation" by making a pseudo copy of the "original" message within the body of the "new".
					textBody.Text="\r\n\r\n-----"+Lan.g(this,"Original Message")+"-----\r\n"
						+(_patientRegarding==null ? "" : (Lan.g(this,"Regarding Patient")+": "+_patientRegarding.GetNameFL()+"\r\n"))
						+Lan.g(this,"From")+": "+_emailMessage.FromAddress+"\r\n"
						+Lan.g(this,"Sent")+": "+_emailMessage.MsgDateTime.ToShortDateString()+" "+_emailMessage.MsgDateTime.ToShortTimeString()+"\r\n"
						+Lan.g(this,"To")+": "+_emailMessage.ToAddress+"\r\n"
						+Lan.g(this,"Subject")+": "+_emailMessage.Subject
						+"\r\n\r\n"+_emailMessage.BodyText;
					//Since this email message was originally from the patient to the provider, we now need to swap the to and from address around.
					//This way, the "new" web mail looks like an actual reply to the patient instead of the patient talking back to themselves.
					string toAddressOld=_emailMessage.ToAddress;
					string fromAddressOld=_emailMessage.FromAddress;
					_emailMessage.ToAddress=fromAddressOld;
					_emailMessage.FromAddress=toAddressOld;
				}
				textTo.Text=_emailMessage.ToAddress;
				textFrom.Text=_emailMessage.FromAddress;
				_listEmailAttaches=_emailMessage.Attachments;
			}
			FillAttachments();
			SetEnabledHelper();
			VerifyInputs();
		}

		///<summary>Sets enabled statuses for fields based on whether we are viewing an existing message or replying to/composing a message.</summary>
		private void SetEnabledHelper() {
			if(_webMailMode==WebMailMode.View) {
				comboRegardingPatient.Enabled=false;
				textSubject.ReadOnly=true;
				textBody.ReadOnly=true;
				textBody.BackColor=SystemColors.Control;
				butAttach.Enabled=false;
				butPreview.Enabled=false;
				butSend.Text=Lan.g(this,"&Reply");
				listAttachments.ContextMenu=new ContextMenu(new[] { menuItemAttachmentPreview });
				labelNotification.Text="";
				butProvPick.Enabled=false;
			}
			else {
				comboRegardingPatient.Enabled=true;
				textSubject.ReadOnly=false;
				textBody.ReadOnly=false;
				textBody.BackColor=SystemColors.Window;
				butAttach.Enabled=true;
				butPreview.Enabled=true;
				butSend.Text=Lan.g(this,"&Send");
				listAttachments.ContextMenu=contextMenuAttachments;//contains a remove and open
				//labelNotification.Text will be set in VerifyInputs based on input values
				butProvPick.Enabled=true;
			}
		}

		private void FillAttachments() {
			listAttachments.Items.Clear();
			listAttachments.Items.AddList(_listEmailAttaches,x => x.DisplayedFileName);
			if(_listEmailAttaches.Count>0) {
				listAttachments.SelectedIndex=0;
			}
		}

		///<summary>Disables the send and preview buttons and shows a red warning label with the "reason" passed in.  Translates the reason.</summary>
		private void BlockSendNotificationMessage(string reason) {
			_allowSendNotificationMessage=false;
			butSend.Enabled=false;
			butPreview.Enabled=false;
			labelNotification.Text=Lan.g(this,"Warning")+": "+Lan.g(this,"Notification email send prevented")+" - "+Lan.g(this,reason);
			labelNotification.ForeColor=Color.Red;
		}

		///<summary>Enables the send and preview buttons so that web mail can be sent.</summary>
		private void AllowSendMessages() {
			_allowSendNotificationMessage=true;
			butSend.Enabled=true;
			butPreview.Enabled=true;
			labelNotification.ForeColor=SystemColors.ControlText;
		}

		///<summary></summary>
		private void VerifyInputs() {
			if(_webMailMode!=WebMailMode.View) {//If in view mode, do not enable preview and send buttons.
				AllowSendMessages();
			}
			if(_patient.Email=="") {
				BlockSendNotificationMessage("Missing patient email. Setup patient email using Family module.");
			}
			if(!Patients.HasPatientPortalAccess(_patient.PatNum)) {
				BlockSendNotificationMessage("Patient has not been given online access. Setup patient online access using Chart module.");
			}
			if(_emailMessage!=null) {
				if(_patientRegarding.PatNum==0) {
					BlockSendNotificationMessage("Patient who sent this message cannot access PHI for regarding patient.");
				}
			}
			if(PrefC.GetString(PrefName.PatientPortalNotifySubject)=="") {
				BlockSendNotificationMessage("Missing notification email subject. Create a subject in Setup.");
			}
			if(PrefC.GetString(PrefName.PatientPortalNotifyBody)=="") {
				BlockSendNotificationMessage("Missing notification email body. Create a body in Setup.");
			}
			if(_allowSendNotificationMessage && _webMailMode!=WebMailMode.View) {//If in view mode, do not include notification email information.
				labelNotification.Text=Lan.g(this,"Notification email will be sent to patient")+": "+_patient.Email;
			}
		}

		///<summary></summary>
		private bool VerifyOutputs() {
			if(textSubject.Text=="") {
				MsgBox.Show(this,"Enter a subject");
				textSubject.Focus();
				return false;
			}
			if(textBody.Text=="") {
				MsgBox.Show(this,"Email body is empty");
				textBody.Focus();
				return false;
			}
			if(GetPatNumSubj()<=0) {
				MsgBox.Show(this,"Select a valid patient");
				comboRegardingPatient.Focus();
				return false;
			}
			return true;
		}

		///<summary>Returns true if the From provider is associated to the user currently logged in.
		///If the user has chosen a different provider as the From provider this will prompt them to enter a password for any user associated to them.
		///Loops through all users associated to the From provider until the credentials typed in match.</summary>
		private bool VerifyFromProvider() {
			if(_provider==null) {
				MsgBox.Show(this,"Invalid From provider.");
				return false;
			}
			//Don't require validating credentials if the user currently logged in is associated to the selected provider.
			if(_providerCurUser!=null && _providerCurUser.ProvNum==_provider.ProvNum) {
				return true;
			}
			List<Userod> listUserods=Userods.GetUsersByProvNum(_provider.ProvNum);//Get all potential users for this provider.
			while(true) {
				//Get the password for a user that is associated to the provider chosen.
				using InputBox inputBox=new InputBox(Lan.g(this,"Input a password for a User that is associated to provider:")+"\r\n"+_provider.GetFormalName());
				inputBox.textResult.PasswordChar='*';
				inputBox.ShowDialog();
				if(inputBox.DialogResult==DialogResult.OK) {
					//Validate the password typed in against all the users associated to the selected provider.
					for(int i=0;i<listUserods.Count();i++) {
						if(Authentication.CheckPassword(listUserods[i],inputBox.textResult.Text)) {
							return true;
						}
					}
					MsgBox.Show(this,"Invalid password.  Please try again or Cancel.");
				}
				else {//User canceled
					return false;
				}
			}
		}

		private long GetPatNumSubj() {
			if(_listPatients==null) {
				return 0;
			}
			return _listPatients[comboRegardingPatient.SelectedIndex].PatNum;
		}

		private void listAttachments_MouseDown(object sender,MouseEventArgs e) {
			//A right click also needs to select an items so that the context menu will work properly.
			if(e.Button==MouseButtons.Right) {
				int idxClicked=listAttachments.IndexFromPoint(e.X,e.Y);
				if(idxClicked!=-1) {
					listAttachments.SelectedIndex=idxClicked;
				}
			}
		}

		private void listAttachments_DoubleClick(object sender,EventArgs e) {
			if(listAttachments.SelectedIndex==-1) {
				return;
			}
			EmailAttach emailAttach=_listEmailAttaches[listAttachments.SelectedIndex];
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName),emailAttach.DisplayedFileName);		
		}

		private void menuItemAttachmentPreview_Click(object sender,EventArgs e) {
			listAttachments_DoubleClick(sender,e);
		}

		private void menuItemAttachmentRemove_Click(object sender,EventArgs e) {
			try {
				if(listAttachments.SelectedIndex==-1) {
					return;
				}
				FileAtoZ.Delete(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_listEmailAttaches[listAttachments.SelectedIndex].ActualFileName));				
				_listEmailAttaches.RemoveAt(listAttachments.SelectedIndex);
				FillAttachments();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormEServicesPatientPortal formEServicesPatientPortal=new FormEServicesPatientPortal();
			formEServicesPatientPortal.ShowDialog();
			if(formEServicesPatientPortal.DialogResult==DialogResult.OK) {
				VerifyInputs();//Validates preferences that are necessary to sending notification emails.
			}
		}

		private void butProvPick_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(_listProviders);
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult==DialogResult.OK) {
				_provider=_listProviders.First(x => x.ProvNum==formProviderPick.ProvNumSelected);
				textFrom.Text=Providers.GetFormalName(_provider.ProvNum);
			}
		}

		private void butPreview_Click(object sender,EventArgs e) {
			if(!VerifyOutputs()) {
				return;
			}
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine("------ "+Lan.g(this,"Notification email that will be sent to the patient's email address:"));
			if(_allowSendNotificationMessage) {
				stringBuilder.Append(Lan.g(this,"Subject")+": ");
				stringBuilder.AppendLine(PrefC.GetString(PrefName.PatientPortalNotifySubject));
				stringBuilder.AppendLine(Lan.g(this,"Body")+": "+GetBodyTextInsecure());
			}
			else {
				stringBuilder.AppendLine(Lan.g(this,"------ "+Lan.g(this,"Notification email settings are not set up.  Click Setup from the web mail message edit window"
					+" to set up notification emails")+" ------"));
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("------ "+Lan.g(this,"Secure web mail message that will be sent to the patient's portal:"));
			stringBuilder.AppendLine(Lan.g(this,"Subject")+": "+textSubject.Text);
			stringBuilder.AppendLine(Lan.g(this,"Body")+": "+textBody.Text.Replace("\n","\r\n"));
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilder.ToString());
			msgBoxCopyPaste.ShowDialog();
		}

		private void butAttach_Click(object sender,EventArgs e) {
			_listEmailAttaches.AddRange(EmailAttachL.PickAttachments(_patient));
			FillAttachments();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_emailMessage==null) {
				DialogResult=DialogResult.Abort;//Nothing to do the message doesn't exist.
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this webmail?")) {
				return;
			}
			EmailMessages.Delete(_emailMessage);
			string logText="";
			logText+="\r\n"+Lan.g(this,"From")+": "+_emailMessage.FromAddress+". ";
			logText+="\r\n"+Lan.g(this,"To")+": "+_emailMessage.ToAddress+". ";
			if(!String.IsNullOrEmpty(_emailMessage.Subject)) {
				logText+="\r\n"+Lan.g(this,"Subject")+": "+_emailMessage.Subject+". ";
			}
			if(!String.IsNullOrEmpty(_emailMessage.BodyText)) {
				if(_emailMessage.BodyText.Length > 50) {
					logText+="\r\n"+Lan.g(this,"Body Text")+": "+_emailMessage.BodyText.Substring(0,49)+"... ";
				}
				else {
					logText+="\r\n"+Lan.g(this,"Body Text")+": "+_emailMessage.BodyText;
				}
			}
			if(_emailMessage.MsgDateTime != DateTime.MinValue) {
				logText+="\r\n"+Lan.g(this,"Date")+": "+_emailMessage.MsgDateTime.ToShortDateString()+". ";
			}
			SecurityLogs.MakeLogEntry(Permissions.WebMailDelete,_emailMessage.PatNum,Lan.g(this,"Web Mail deleted.")+" "+logText);
			DialogResult=DialogResult.Abort;//We want to abort here to avoid using the email in parent windows when it's been deleted.
		}

		///<summary>When viewing an existing message, the "Send" button text will be "Reply" and _webMailMode will be View.  Pressing the button will
		///reload this form as a reply message.
		///When composing a new message or replying to an existing message, the button text will be "Send" and _webMailMode will be
		///either Compose or Reply.  Pressing the button will cause an attempt to send the secure and insecure message if applicable.</summary>
		private void butSend_Click(object sender,EventArgs e) {
			if(_webMailMode==WebMailMode.View) {
				_webMailMode=WebMailMode.Reply;
				FillFields();
				return;
			}
			if(!Security.IsAuthorized(Permissions.WebMailSend)) {
				return;
			}
			VerifyInputs();
			if(!VerifyOutputs()) {
				return;
			}
			if(!VerifyFromProvider()) {
				return;
			}
			butSend.Enabled=false;
			EmailType emailType=PrefC.GetEnum<EmailType>(PrefName.PortalWebEmailTemplateType);
			//Insert the message. The patient will not see this as an actual email.
			//Rather, they must login to the patient portal (secured) and view the message that way.
			//This is how we get around sending the patient a secure message, which would be a hassle for all involved.
			_emailMessageSecure=new EmailMessage();
			_emailMessageSecure.FromAddress=textFrom.Text;
			_emailMessageSecure.ToAddress=textTo.Text;
			_emailMessageSecure.PatNum=_patient.PatNum;
			_emailMessageSecure.SentOrReceived=EmailSentOrReceived.WebMailSent;  //this is secure so mark as webmail sent
			_emailMessageSecure.ProvNumWebMail=_provider.ProvNum;
			_emailMessageSecure.Subject=textSubject.Text;
			_emailMessageSecure.BodyText=textBody.Text;
			_emailMessageSecure.MsgDateTime=DateTime.Now;
			_emailMessageSecure.PatNumSubj=GetPatNumSubj();
			_emailMessageSecure.MsgType=EmailMessageSource.WebMail;
			if(_allowSendNotificationMessage) {
				_emailMessageInsecure=new EmailMessage();
				_emailMessageInsecure.HtmlType=emailType;
				if(string.IsNullOrEmpty(_emailAddressSender.SenderAddress)) {
					_emailMessageInsecure.FromAddress=_emailAddressSender.EmailUsername;
				}
				else {
					_emailMessageInsecure.FromAddress=_emailAddressSender.SenderAddress;
				}
				_emailMessageInsecure.ToAddress=_patient.Email;
				_emailMessageInsecure.PatNum=_patient.PatNum;
				_emailMessageInsecure.Subject=PrefC.GetString(PrefName.PatientPortalNotifySubject);
				Clinic clinic=Clinics.GetClinic(_patient.ClinicNum)??Clinics.GetPracticeAsClinicZero();
				_emailMessageInsecure.BodyText=Clinics.ReplaceOffice(GetBodyTextInsecure(),clinic,isHtmlEmail:true,doReplaceDisclaimer:true);
				_emailMessageInsecure.SentOrReceived=EmailSentOrReceived.Sent; //this is not secure so just mark as regular sent
				//Send an insecure notification email to the patient.
				_emailMessageInsecure.MsgDateTime=DateTime.Now;
				_emailMessageInsecure.PatNumSubj=GetPatNumSubj();
				_emailMessageInsecure.MsgType=EmailMessageSource.WebMail;
				try {
					EmailMessages.PrepHtmlEmail(_emailMessageInsecure);
					EmailMessages.SendEmail(_emailMessageInsecure,_emailAddressSender);
				}
				catch(Exception ex) {
					MessageBox.Show(this,"An error occurred sending the message. Please try again later or contact support.");
					Logger.openlog.LogMB(this,System.Reflection.MethodBase.GetCurrentMethod().Name,ex.Message,Logger.Severity.ERROR);
					butSend.Enabled=true;
					return;
				}
			}
			_emailMessageSecure.Attachments=_listEmailAttaches;
			EmailMessages.Insert(_emailMessageSecure);
			SecurityLogs.MakeLogEntry(Permissions.WebMailSend,0,Lan.g(this,"Web Mail sent"));
			MsgBox.Show(this,"Message Sent");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


		private enum WebMailMode {
			Compose,
			View,
			Reply
		}
	}
}