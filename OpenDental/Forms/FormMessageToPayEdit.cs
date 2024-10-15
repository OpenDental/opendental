using CodeBase;
using Newtonsoft.Json;
using OpenDental.Thinfinity;
using OpenDentBusiness;
using OpenDentBusiness.AutoComm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMessageToPayEdit:FormODBase {
		private MsgToPayTagReplacer _msgToPayTagReplacer;
		private Patient _patient;
		private Clinic _clinic;
		private PatComm _patComm;
		#region Message Template
		private EmailType _emailTypeCur=EmailType.Html;
		private string _msgToPayTextTemplate;
		private MsgToPayEmailTemplate _msgToPayEmailTemplate;
		private string _msgToPayEmailHtmlText;
		private string _emailSubject;
		#endregion Message Template

		public FormMessageToPayEdit(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_msgToPayTagReplacer=new MsgToPayTagReplacer();
			Text=Text+$" - {_patient.LName}, {_patient.FName}";//Title becomes 'Message-to-Pay - Doe, John'
		}

		#region Event Handlers
		private void FormMessageToPayEdit_Load(object sender,EventArgs e) {
			LoadMessageTemplates();
			_clinic=Clinics.GetClinicOrSmsDefaultOrPracticeClinic(_patient.ClinicNum);
			_patComm=Patients.GetPatComms(ListTools.FromSingle(_patient.PatNum),_clinic).FirstOrDefault();
			checkText.Enabled=_patComm.IsSmsAnOption && SmsPhones.IsIntegratedTextingEnabled();
			checkEmail.Enabled=_patComm.IsEmailAnOption;
			radioFamily.Checked=true;
			SetEnabledMessageTemplateControls();
		}

		private void FormMessageToPayEdit_Shown(object sender,EventArgs e) {
			RefreshEmailBrowser();
		}

		private void butEditEmail_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.MarkupText=_msgToPayEmailTemplate.Template;
			formEmailEdit.IsRawAllowed=true;
			formEmailEdit.IsRaw=_msgToPayEmailTemplate.EmailType==EmailType.RawHtml;
			formEmailEdit.DoCheckForDisclaimer=false;
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_msgToPayEmailTemplate.EmailType=EmailType.Html;
			if(formEmailEdit.IsRaw) {
				_msgToPayEmailTemplate.EmailType=EmailType.RawHtml;
			}
			_msgToPayEmailTemplate.Template=formEmailEdit.MarkupText;
			_msgToPayEmailHtmlText=formEmailEdit.HtmlText;
			RefreshEmailBrowser();
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!PerformValidation()) {
				return;
			}
			CommOptOut commOptOut=CommOptOuts.GetForPat(_patient.PatNum);
			bool isOptedOutEmail=false;
			bool isOptedOutText=false;
			if(commOptOut!=null) {
				isOptedOutEmail=checkEmail.Checked && commOptOut.IsOptedOut(CommOptOutMode.Email,CommOptOutType.MsgToPay);
				isOptedOutText=checkText.Checked && commOptOut.IsOptedOut(CommOptOutMode.Text,CommOptOutType.MsgToPay);
			}
			if(isOptedOutEmail || isOptedOutText) {
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"Patient has opted out of receiving automated messages. Send anyway?")) {
					return;
				}
			}
			//Single Patient or Family
			//Create a regular Statement here.
			Statement statement=GetRegularStatement();
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			List<Def> listDefsForImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			DataSet dataSetAccount=AccountModules.GetAccount(_patient.PatNum,statement);
			Documents.CreateAndSaveStatementPDF(statement,sheetDef,isLimitedCustom:false,showLName:false,excludeTxfr:false,listDefsForImageCats,dataSet:dataSetAccount);
			bool emailSent=false;
			bool textSent=false;
			string filePath=CreateStatementPDF(statement);
			if(checkText.Checked) {
				textSent|=SendText(statement);
			}
			if(checkEmail.Checked) {
				emailSent|=SendEmail(statement,filePath);
			}
			if(!emailSent && !textSent) {//Nothing sent, don't insert EServiceShortGuid.
				//Error messages will happen within each send method
				return;
			}
			statement=Statements.GetStatement(statement.StatementNum);//Refresh just in case
			EServiceShortGuids.CreateAndInsertMsgToPayShortGuid(_patient.PatNum,statement.ShortGUID);
			List<string> listSendModes=new List<string>();
			string message=Lan.g(this,"Message sent via:")+" ";
			if(emailSent) {
				listSendModes.Add(Lan.g(this,"Email"));
			}
			if(textSent) {
				listSendModes.Add(Lan.g(this,"Text"));
			}
			message=message+string.Join(", ",listSendModes);
			MessageBox.Show(message);
			DialogResult=DialogResult.OK;
		}

		private void butPreview_Click(object sender,EventArgs e) {
			Statement statement=GetRegularStatement();
			if(statement==null) {
				return;
			}
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			DataSet dataSetStatement=AccountModules.GetAccount(_patient.PatNum,statement,isComputeAging:false,doIncludePatLName:true);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
			SheetFiller.FillFields(sheet,dataSetStatement,statement);
			SheetUtil.CalculateHeights(sheet,dataSetStatement,statement,true);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
			string filePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,filePath,statement,dataSet:dataSetStatement);
			try {
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(filePath);
				}
				else {
					Process.Start(filePath);
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unable to open the file."),ex);
			}
			try {
				Statements.DeleteStatements(ListTools.FromSingle(statement),true);
			}
			catch(Exception ex) {
				//This shouldn't happen
			}
		}

		private void checkEmail_Click(object sender,EventArgs e) {
			SetEnabledMessageTemplateControls();
		}

		private void checkText_Click(object sender,EventArgs e) {
			SetEnabledMessageTemplateControls();
		}
		#endregion Event Handlers

		#region Helper Methods
		///<summary>Enables or disables controls depending on the check state of checkEmail and checkText.</summary>
		private void SetEnabledMessageTemplateControls() {
			//Email controls
			bool isEmailChecked=checkEmail.Checked;
			groupEmail.Enabled=isEmailChecked;
			textSubject.Enabled=isEmailChecked;
			butEditEmail.Enabled=isEmailChecked;
			//SMS controls
			textMessage.Enabled=checkText.Checked;
		}

		///<summary>Performs message sending validation.</summary>
		private bool PerformValidation() {
			//One option must be selected
			if(!checkText.Checked && !checkEmail.Checked) {//Neither send modes checked
				MsgBox.Show(this,"A message type must be selected.");
				return false;
			}
			if(checkText.Checked) {
				//Message must contain Text to pay tag.
				if(!textMessage.Text.Contains(MsgToPayTagReplacer.MSG_TO_PAY_TAG)) {
					MessageBox.Show(Lan.g(this,"SMS Message Text must contain")+" '"+MsgToPayTagReplacer.MSG_TO_PAY_TAG+"'.");
					return false;
				}
			}
			if(checkEmail.Checked) {//Not an else if because we can send for both, in that case validate for both.
				if(!browserEmail.DocumentText.Contains(MsgToPayTagReplacer.MSG_TO_PAY_TAG)) {
					MessageBox.Show(Lan.g(this,"Email Message Text must contain")+" '"+MsgToPayTagReplacer.MSG_TO_PAY_TAG+"'.");
					return false;
				}
				if(string.IsNullOrWhiteSpace(textSubject.Text)) {
					MsgBox.Show(this,"Email Subject cannot be empty.");
					return false;
				}
			}
			//One option must be selected
			if(!radioFamily.Checked && !radioPatient.Checked) {
				MsgBox.Show(this,"A statement type must be selected.");
				return false;
			}
			//SMS
			if(checkText.Checked && !PatientL.CheckPatientTextingAllowed(_patient,this)) {//Texting is selected and Patient not allowed to be texted, PatientL.CheckPatientTextingAllowed() displays message when false
				return false;
			}
			if(checkText.Checked && (_patComm==null || !_patComm.IsSmsAnOption)) {
				MsgBox.Show(this,"SMS is not an option for this patient.");
				return false;
			}
			//Email
			if(checkEmail.Checked && (_patComm==null || !_patComm.IsEmailAnOption)) {//Email is selected and we either have no PatComm or patient can't be emailed
				MsgBox.Show(this,"Email is not an option for this patient.");
				return false;
			}
			return true;
		}

		///<summary>Loads message templates from preferences.</summary>
		private void LoadMessageTemplates() {
			MsgToPayLite msgToPayLite=new MsgToPayLite(_patient);//Used only for tag replacements
			_emailSubject=_msgToPayTagReplacer.ReplaceTags(PrefC.GetString(PrefName.PaymentPortalMsgToPaySubjectTemplate),msgToPayLite,_clinic,false);
			_msgToPayEmailTemplate=JsonConvert.DeserializeObject<MsgToPayEmailTemplate>(PrefC.GetString(PrefName.PaymentPortalMsgToPayEmailMessageTemplate));
			if(_msgToPayEmailTemplate==null) {
				_msgToPayEmailTemplate=new MsgToPayEmailTemplate();
				_msgToPayEmailTemplate.Template="";
			}
			_msgToPayEmailHtmlText=_msgToPayEmailTemplate.Template;
			_msgToPayTextTemplate=PrefC.GetString(PrefName.PaymentPortalMsgToPayTextMessageTemplate);
			_msgToPayTextTemplate=_msgToPayTagReplacer.ReplaceTags(_msgToPayTextTemplate,msgToPayLite,_clinic,false);
			textSubject.Text=_emailSubject;
			textMessage.Text=_msgToPayTextTemplate;
			browserEmail.DocumentText=_msgToPayEmailHtmlText;
		}

		private Statement SetStatementSendModes(Statement statement) {
			if(checkText.Checked) {
				statement.SmsSendStatus=AutoCommStatus.SendNotAttempted;
			}
			else {
				statement.SmsSendStatus=AutoCommStatus.DoNotSend;
			}
			//Mode_ is insignificant unless we are sending via Email.
			//Patient will never actually receive this statement in this case since the statement only exists to generate a balance for M2P.
			statement.Mode_=StatementMode.Electronic;
			if(checkEmail.Checked) {
				statement.Mode_=StatementMode.Email;
			}
			return statement;
		}

		private Statement GetRegularStatement() {
			Statement statement=Statements.GenerateStatement(_patient,DateTime.MinValue,DateTime.Today,StatementMode.Electronic,isSinglePatient:radioPatient.Checked);
			statement=SetStatementSendModes(statement);
			statement.StatementNum=Statements.Insert(statement);
			return statement;
		}

		private bool SendText(Statement statement) {
			SmsToMobile smsToMobile=new SmsToMobile();
			smsToMobile.ClinicNum=_patient.ClinicNum;
			smsToMobile.GuidMessage=Guid.NewGuid().ToString();
			smsToMobile.GuidBatch=smsToMobile.GuidMessage;
			smsToMobile.IsTimeSensitive=false;
			smsToMobile.MobilePhoneNumber=_patComm.SmsPhone;
			smsToMobile.PatNum=statement.PatNum;
			smsToMobile.MsgType=SmsMessageSource.MsgToPay;
			smsToMobile.MsgText=_msgToPayTagReplacer.ReplaceTagsForStatement(textMessage.Text,_patient,statement,_clinic,false);
			statement.TagOD=smsToMobile.GuidMessage;
			bool sendSuccessful=false;
			try {
				List<SmsToMobile> listSmsToMobiles=SmsToMobiles.SendSmsMany(ListTools.FromSingle(smsToMobile),userod:Security.CurUser);
				sendSuccessful=Statements.HandleSmsSent(listSmsToMobiles,ListTools.FromSingle(statement)).IsNullOrEmpty();//If empty, nothing failed
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"An error occurred while attempting to send your message: ")+ex.Message);
				return false;
			}
			if(!sendSuccessful) {
				MsgBox.Show(this,"Text message failed to send.");
			}
			return sendSuccessful;
		}

		private bool SendEmail(Statement statement,string savedPdfPath) {
			Document documentStatement=Documents.GetByNum(statement.DocNum);
			string attachPath=EmailAttaches.GetAttachPath();
			string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+ODRandom.Next(1000).ToString()+".pdf";
			string filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				ImageStore.Export(filePathAndName,documentStatement,_patient);
			}
			else {
				FileAtoZ.Copy(savedPdfPath,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ);
			}
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.DisplayedFileName="Statement.pdf";
			emailAttach.ActualFileName=fileName;
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.BodyText=_msgToPayTagReplacer.ReplaceTagsForStatement(_msgToPayEmailTemplate.Template,_patient,statement,_clinic,false,true);
			emailMessage.HtmlText=_msgToPayTagReplacer.ReplaceTagsForStatement(_msgToPayEmailHtmlText,_patient,statement,_clinic,false,true);
			EmailAddress emailAddress=EmailAddresses.GetNewEmailDefault(Security.CurUser.UserNum,_patient.ClinicNum);
			emailMessage.FromAddress=emailAddress.GetFrom();
			emailMessage.MsgDateTime=DateTime_.Now;
			emailMessage.PatNum=_patient.Guarantor;
			emailMessage.PatNumSubj=_patient.PatNum;
			emailMessage.RecipientAddress=_patComm.Email;
			emailMessage.Subject=_msgToPayTagReplacer.ReplaceTagsForStatement(textSubject.Text,_patient,statement,_clinic,false,true);
			emailMessage.ToAddress=_patComm.Email;
			emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
			emailMessage.HtmlType=_emailTypeCur;
			emailMessage.MsgType=EmailMessageSource.MsgToPay;
			emailMessage.Attachments=ListTools.FromSingle(emailAttach);
			try {
				EmailMessages.SendEmail(emailMessage,emailAddress);
			}
			catch (Exception ex) {
				MessageBox.Show(Lan.g(this,"Email failed to send: ")+ex.Message);
				return false;
			}
			return true;
		}

		///<summary>Returns filepath.</summary>
		private string CreateStatementPDF(Statement statement) {
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			DataSet dataSetStatement=AccountModules.GetAccount(_patient.PatNum,statement,isComputeAging:false,doIncludePatLName:true);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
			SheetFiller.FillFields(sheet,dataSetStatement,statement);
			SheetUtil.CalculateHeights(sheet,dataSetStatement,statement,true);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
			string filePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,filePath,statement,dataSet:dataSetStatement);
			return filePath;
		}

		private void RefreshEmailBrowser() {
			_emailTypeCur=_msgToPayEmailTemplate.EmailType;
			MsgToPayLite msgToPayLite=new MsgToPayLite(_patient);
			//Replace tags now, any tag that requires a statement will not be replaced until send (MsgToPay and Statement Urls/Statement balance)
			string templateMessage=_msgToPayTagReplacer.ReplaceTags(_msgToPayEmailTemplate.Template,msgToPayLite,_clinic,true); 
			if(_emailTypeCur==EmailType.RawHtml) {
				browserEmail.DocumentText=templateMessage;
			}
			else {
				try {
					browserEmail.DocumentText=MarkupEdit.TranslateToXhtml(templateMessage,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true,scale:LayoutManager.ScaleMyFont());
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}
		#endregion Helper Methods
	}
}