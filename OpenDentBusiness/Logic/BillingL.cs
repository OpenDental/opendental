using CodeBase;
using OpenDentBusiness.FileIO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using MySqlConnector;
using OpenDentBusiness.AutoComm;

namespace OpenDentBusiness {
	public class BillingL {

		///<summary>ODService has the option to run statements. This workflow has no UI so all decisions will be made here as if the user made the decision.
		///This method is implemented in BillingL so it can be unit tested. Leaving it in OD Service would prevent it from being unit tested.</summary>
		public static void RunFromODService(LogWriter logWriter,Action<SendStatementsIO> actionMock) {
			SendStatementsIO sendStatementsIO=new SendStatementsIO();
			sendStatementsIO.Source="OpenDentalService";
			sendStatementsIO.LogWriter=logWriter;
			sendStatementsIO.AllowXmlFileSelection=false;
			sendStatementsIO.FuncComputeAging=(dateTimeToday) => {
				try {
					Ledgers.ComputeAging(0,dateTimeToday);
					return true;
				}
				catch(Exception ex) {
					if(ex is MySqlException mySqlEx&&mySqlEx.Number==1213) {
						//Deadlock error, don't re-throw.
						sendStatementsIO.LogWrite(Lans.g("FormBilling","Deadlock error detected in aging transaction and rolled back. Try again later."),LogLevel.Error);
						return false;
					}
					else {
						//Not a deadlock error, just re-throw.
						sendStatementsIO.LogWrite(Lans.g("FormBilling","Unknown Aging error:\r\n")+ex.Message,LogLevel.Error);
						throw ex;
					}
				}
			};
			sendStatementsIO.FuncGetIsHistoryStartMinDate=() => { return true; };
			sendStatementsIO.FuncAskQuestion=(question) => { return true; };
			sendStatementsIO.ActionPrompt=(prompt,useCopyPasteDialog) => { sendStatementsIO.LogWrite(prompt,LogLevel.Error); };
			sendStatementsIO.FuncChooseSaveFile=(initialSaveDirectory) => { throw new Exception("OpenDentalService should not be prompted to select an XML file."); };
			sendStatementsIO.ActionProgressEvent=(odEventArgs) => { };
			sendStatementsIO.FuncGetBillingDataTable=() => { throw new Exception("OpenDentalService should not be allowed to pause/resume."); };
			sendStatementsIO.FunctGetIsPaused=() => { return false; };
			sendStatementsIO.FuncGetIsCancelled=() => { return false; };
			sendStatementsIO.ActionSleepDuringPause=() => { };
			sendStatementsIO.FuncGetSenderEmailAddress=(clinicNum) => { 
				EmailAddress emailAddress=EmailAddresses.GetByClinic(clinicNum);
				//Use clinic's Email Sender Address Override, if present.
				emailAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,clinicNum);
				return emailAddress;
			};
			sendStatementsIO.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => {
				if(useSecureEmail) {
					EmailSecures.InsertMessageThenSend(emailMessage,emailAddress,emailMessage.ToAddress,clinicNumPat);
				}
				else {
					//If IsCloudStorage==true, then we will end up downloading the file again in EmailMessages.SendEmailUnsecure.
					EmailMessages.SendEmail(emailMessage,emailAddress);
				}
			};
			sendStatementsIO.FuncGetPatientPdfPath=(tempPdfFile,filePath) => {
				if(!CloudStorage.IsCloudStorage) {
					//savedPdfPath is just the filename when using DataStorageType.InDatabase
					return filePath;
				}
				//Using cloud.
				if(tempPdfFile!="") {
					//To save time by not having to download it.
					return tempPdfFile;
				}
				//We have not yet downloaded the pdf.
				string savedPdfPath=PrefC.GetRandomTempFile("pdf");
				FileAtoZ.Copy(filePath,savedPdfPath,FileAtoZSourceDestination.AtoZToLocal);
				return savedPdfPath;
			};
			sendStatementsIO.FuncGetPdfDocument=(rawBase64,savedPdfPath) => {
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					byte[] rawData=Convert.FromBase64String(rawBase64);
					using(Stream stream = new MemoryStream(rawData)) {
						return PdfReader.Open(stream,PdfDocumentOpenMode.Import);
					}
				}
				else {
					return PdfReader.Open(savedPdfPath,PdfDocumentOpenMode.Import);
				}
			};
			sendStatementsIO.FuncGetEmailAttachment=(savedPdfPath,documentStatement,patient) => {
				string attachPath=EmailAttaches.GetAttachPath();
				string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+ODRandom.Next(1000).ToString()+".pdf";
				string filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					ImageStore.Export(filePathAndName,documentStatement,patient);
				}
				else {
					FileAtoZ.Copy(savedPdfPath,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ);
				}
				EmailAttach emailAttach=new EmailAttach();
				emailAttach.DisplayedFileName="Statement.pdf";
				emailAttach.ActualFileName=fileName;
				return emailAttach;
			};
			sendStatementsIO.ActionDeleteTempPdfFile=(tempPdfFile) => {
				File.Delete(tempPdfFile);
			};
			sendStatementsIO.FuncSendEhgStatement=(xmlData,clincNum) => {
				Bridges.EHG_statements.Send(xmlData,sendStatementsIO.CurStatementBatch.ClinicNum,out string alertMsg);
				return alertMsg;
			};
			//Give unit test one last chance to mock any callbacks of sendStatementsIO instance. This gives unit test a chance to short-circuit things like email send, pdf gen, etc.
			actionMock(sendStatementsIO);
			List<long> listUnsentStatementNums=Statements.GetUnsentStatements(StatementMode.Electronic,StatementMode.Email);
			if(!listUnsentStatementNums.IsNullOrEmpty()) {
				//Delete unsent statements before we create new ones
				sendStatementsIO.LogWrite($"{listUnsentStatementNums.Count} "+Lans.g("FormBilling","unsent statements deleted."),LogLevel.Verbose);
				Statements.DeleteAll(listUnsentStatementNums);
				//This is not an accurate permission type.
				SecurityLogs.MakeLogEntry(EnumPermType.Billing,0,"Automated Statement Generator: Unsent statements were deleted.");
			}
			//Aging required before statements can be generated.
			if(!RunAgingEnterprise(sendStatementsIO)) {
				sendStatementsIO.LogWrite(Lans.g("FormBilling","Aging failed"),LogLevel.Error);
				return;
			}
			List<long> listClinicNumsAll=new List<long>() { -1 };
			if(PrefC.HasClinicsEnabled) {
				listClinicNumsAll=Clinics.GetDeepCopy(isShort: true).Select(x => x.ClinicNum).ToList();
				listClinicNumsAll.Add(0);//Include HQ/Unassigned clinic. FormBilling.cs includes HQ when 'All' is selected.
			}
			List<Statement> listStatementsCreatedAll=new List<Statement>();
			foreach(long clinicNum in listClinicNumsAll) {
				List<Statement> listStatementsCreatedByClinic=CreateStatementHelper(clinicNum,sendStatementsIO);//clinicNumCur will be -1 if clinics are not enabled.
				if(listStatementsCreatedByClinic.IsNullOrEmpty()) {
					sendStatementsIO.LogWrite(Lans.g("FormBilling","No statements generated for ClinicNum")+" "+clinicNum.ToString(),LogLevel.Error);
					continue;
				}
				sendStatementsIO.LogWrite($"Generated {listStatementsCreatedByClinic.Count} statements for ClinicNum {clinicNum}",LogLevel.Information);
				listStatementsCreatedAll.AddRange(listStatementsCreatedByClinic);
			}
			sendStatementsIO.ListStatementNumsToSend=listStatementsCreatedAll
				//OD Service will generate Mail, Email, Electronic statements in CreateStatementHelper. But we only want to handle send for Email, Electronic.
				//This behavior was retained during OD Service billing refactor job I15824.
				.Where(x => x.Mode_.In(StatementMode.Email,StatementMode.Electronic))
				.Select(x => x.StatementNum).ToList();
			SendStatements(sendStatementsIO);
		}

		///<summary>No FillGrid needed in here or in any called method because that always happens one level up. 
		///Returns false if billing is interrupted or has a critical failure.</summary>
		public static bool SendStatements(SendStatementsIO sendStatementsIO) {
			//Run aging for all patients. May be skipped if aging has already run today.
			if(!RunAgingEnterprise(sendStatementsIO)) {
				return false;//if aging fails, don't generate and print statements
			}
			//Statements will be ordered in GetStatements.
			List<Statement> listStatements=Statements.GetStatements(sendStatementsIO.ListStatementNumsToSend);
			Statement popUpCheck=listStatements.FirstOrDefault(x => x.Mode_==StatementMode.Electronic);
			//In case the user didn't come directly from FormBillingOptions check the DateRangeFrom on an electronic statement to see if we need to
			//display the warning message. Spot checking to save time. 
			if(popUpCheck!=null && (sendStatementsIO.FuncGetIsHistoryStartMinDate() || popUpCheck.DateRangeFrom.Year<1880)) {
				if(!sendStatementsIO.FuncAskQuestion(Lans.g("FormBilling","Sending statements electronically for all account history could result in many pages. Continue?"))) {
					return false;
				}
				SecurityLogs.MakeLogEntry(EnumPermType.Billing,0,"User proceeded with electronic billing for all dates.");
				sendStatementsIO.LogWrite(Lans.g("FormBilling","User proceeded with electronic billing for all dates."),LogLevel.Information);
			}
			sendStatementsIO.DictionaryFamilies=Statements.GetFamiliesForStatements(listStatements);
			//Installment plans don't get added to db. They get retrieved from db and appended to the appropriate statement passed in.
			//We will use installments later in Statements.CreateStatementPdfSheets.
			Statements.AddInstallmentPlansToStatements(listStatements,sendStatementsIO.DictionaryFamilies);
			List<Patient> listPatients=sendStatementsIO.DictionaryFamilies.Values.SelectMany(x => x.ListPats).DistinctBy(x => x.PatNum).ToList();
			sendStatementsIO.ListStatementBatches=Statements.GetBatchesForStatements(listStatements,listPatients);
			for(int i=0; i<sendStatementsIO.ListStatementBatches.Count; i++) {
				sendStatementsIO.CurStatementBatch=sendStatementsIO.ListStatementBatches[i];
				if(!BillingProgressPause(sendStatementsIO)) {
					return false;
				}
				//Fire progress event before we start.
				sendStatementsIO.FireOverallProgress();
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Preparing Batch")+" "+sendStatementsIO.CurStatementBatch.BatchNum);
				//Now to print, send eBills, and text messages.
				//If any return false, the user canceled during execution OR other catastrophic failure occurred.
				bool isBatchValid=PrintBatch(sendStatementsIO);
				if(isBatchValid) { //Only send eBills if batch ok.
					isBatchValid=SendEBills(sendStatementsIO);
				}
				if(isBatchValid) { //Only send texts if batch and eBills ok.
					isBatchValid=SendTextMessages(sendStatementsIO);
				}
				//Always sync back to StatementProd.
				Statements.SyncStatementProdsForMultipleStatements(sendStatementsIO.CurStatementBatch.ListStatementDatas);
				//There was an issue with this batch don't continue.
				if(!isBatchValid) {
					return false;
				}
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Batch Completed")+"...");
			}
			//Pass back all temp files to FormBilling for file delete.
			for(int i=0;i<sendStatementsIO.ListTempPdfFiles.Count;i++) {
				//Any failures will most likely get cleaned up when the user closes OD.
				ODException.SwallowAnyException(() => sendStatementsIO.ActionDeleteTempPdfFile(sendStatementsIO.ListTempPdfFiles[i]));
			}
			//Reporting on billing results.
			string message="";
			int count=sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.BadEmailAddress);
			if(count>0) {
				message+=Lans.g("FormBilling","Skipped due to missing or bad email address:")+" "+count.ToString()+"\r\n";
			}
			count=sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.BadMailingAddress);
			if(count>0) {
				message+=Lans.g("FormBilling","Skipped due to missing or bad mailing address:")+" "+count.ToString()+"\r\n";
			}
			count=sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.BadSmsSetup);
			if(count>0) {
				message+=Lans.g("FormBilling","No text message sent due to SMS setup issue:")+" "+count.ToString()+"\r\n";
			}
			if(sendStatementsIO.CountStatementsSkippedForDeletion>0) {
				message+=Lans.g("FormBilling","Skipped due to being deleted by another user:")+" "+sendStatementsIO.CountStatementsSkippedForDeletion.ToString()+"\r\n";
			}
			count=sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc);
			if(count>0) {
				message+=Lans.g("FormBilling","Skipped due to miscellaneous error")+": "+count.ToString()+"\r\n";
			}
			message+=Lans.g("FormBilling","Printed:")+" "+sendStatementsIO.CountStatementsPrinted.ToString()+"\r\n"
				+Lans.g("FormBilling","Emailed:")+" "+sendStatementsIO.CountStatementsEmailed.ToString()+"\r\n"
				+Lans.g("FormBilling","SentElect:")+" "+sendStatementsIO.CountStatementsSentElectronic.ToString()+"\r\n"
				+Lans.g("FormBilling","Texted:")+" "+sendStatementsIO.CountStatmentsSentPayPortalText.ToString();
			sendStatementsIO.LogWrite(message,LogLevel.Error);
			if(sendStatementsIO.ListSkippedPatients.Count>0) {
				//Modify original box to have yes/no buttons to see if they want to see who errored out
				message+="\r\n\r\n"+Lans.g("FormBilling","Would you like to see skipped patnums?");
				string skippedPatNums=Lans.g("FormBilling","Skipped Patients...")+"\r\n"+string.Join("\r\n",sendStatementsIO.ListSkippedPatients
					.OrderBy(x => (int)x.Reason)
					.Select(x => $"{Lans.g("FormBilling","PatNum:")} {x.PatNum} ({x.Reason}) {x.Error}"));
				if(sendStatementsIO.FuncAskQuestion(message)) {
					sendStatementsIO.ActionPrompt(skippedPatNums,true);
				}
			}
			else {
				//If there were no errors, we simply show this.
				sendStatementsIO.ActionPrompt(message,false);
			}
			return true;
		}

		///<summary>Concat all the pdf's together to create one print job. Returns false if the printing was canceled. 
		///Changes to this method will also need to be made to OpenDentalService.ProcessStatements.SendEmails().</summary>
		private static bool PrintBatch(SendStatementsIO sendStatementsIO) {
			EmailMessage emailMessage;
			EmailAttach emailAttach;
			EmailAddress emailAddress;
			Patient patient;
			string patFolder;
			PdfDocument pdfDocumentInput;
			PdfPage pdfPage;
			string savedPdfPath;
			DataSet dataSet;
			List<EmailAutograph> listEmailAutographs=EmailAutographs.GetDeepCopy();
			BillingUseElectronicEnum electronicBillingType=PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic);
			//From Saul/Derek attempted fix B31268.
			//If we don't send emails first and there are a lot of e-bills and email address is set to implicit ssl then the emails fail to send sometimes.
			//This ordering will ensure that each batch will process emails before electronic.
			sendStatementsIO.CurStatementBatch.ListStatements=sendStatementsIO.CurStatementBatch.ListStatements.OrderBy(x => x.Mode_).ToList();
			for(int i=0; i<sendStatementsIO.CurStatementBatch.ListStatements.Count; i++) {
				Statement statement=sendStatementsIO.CurStatementBatch.ListStatements[i];
				if(!BillingProgressPause(sendStatementsIO)) {
					return false;
				}
				//Fire progress event before we iterate so we deal with counts before this loop iteration. That is most accurate.
				sendStatementsIO.FireOverallProgress();
				sendStatementsIO.CurStatementIdx++;
				//Pre-increment CountStatementsProcessed.
				//We will only decrement at the very end in the case where we determine this is an eBill and should be processed later as an eBill.
				sendStatementsIO.CurStatementBatch.CountStatementsProcessed++;
				if(statement==null) {//The statement was probably deleted by another user.
					sendStatementsIO.CountStatementsSkippedForDeletion++;
					continue;
				}
				if(sendStatementsIO.ListStatementNumsToSkipAfterPause.Contains(statement.StatementNum) || statement.IsSent) {
					//The statement was deleted or marked sent elsewhere.
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.Misc,Lans.g("FormBilling","Statement was adjusted elsewhere."));
					continue;
				}
				sendStatementsIO.FireStatementProgress(5);
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Generating Single PDFs")+"...");
				//We need the family of this patient so we can use the guarantor address later.
				if(!sendStatementsIO.DictionaryFamilies.TryGetValue(statement.PatNum,out Family family)) {
					family=Patients.GetFamily(statement.PatNum);
				}
				patient=family.GetPatient(statement.PatNum);
				patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				dataSet=AccountModules.GetStatementDataSet(statement,isComputeAging: false,doIncludePatLName: PrefC.IsODHQ);
				//Verify send email before trying to loop through any statements. If it's bad, we won't continue.
				emailAddress=sendStatementsIO.FuncGetSenderEmailAddress(patient.ClinicNum);
				sendStatementsIO.FireStatementProgress(10);
				if(statement.Mode_==StatementMode.Email) {
					//Bad sender email is a show stopper, don't bother to continue.
					if(string.IsNullOrEmpty(emailAddress?.SMTPserver)) {
						sendStatementsIO.ActionPrompt(Lans.g("FormBilling","You need to enter an SMTP server name in e-mail setup before you can send e-mail."),false);
						return false;
					}
					//Bad patient email, just log it an move to next statement.
					if(patient.Email=="") {
						sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadEmailAddress,Lans.g("FormBilling","Empty patient Email"));
						continue;
					}
				}
				sendStatementsIO.FireStatementProgress(15);
				statement.IsSent=true;
				statement.DateSent=DateTime.Today;
				#region Print PDFs
				string tempPdfFile="";
				if(statement.Mode_==StatementMode.Electronic && electronicBillingType.In(BillingUseElectronicEnum.EHG,BillingUseElectronicEnum.ClaimX) && !PrefC.GetBool(PrefName.BillingElectCreatePDF)) {
					//Do not create a pdf
					//Detach the previously created document for the statement if one exists because it may not match what is sent to the patient,
					//and the Statement.DocNum will need to match the StatementProd.DocNum if late charges are going to be created.
					Statements.DetachDocFromStatements(statement.DocNum);
					//DocNum is set to zero for StatementProds when no pdf is created for electronic statements.
					sendStatementsIO.CurStatementBatch.ListStatementDatas.Add(new StatementData(dataSet,0));
				}
				else {
					try {
						tempPdfFile=Statements.CreateStatementPdfSheets(statement,patient,family,dataSet);
						//The above method throws an exception or returns an empty string if unable to create a PDF.
						if(tempPdfFile=="") {
							throw new Exception("Error creating statement PDF");
						}
						//Add to temp file list so we can delete it when we are done.
						sendStatementsIO.ListTempPdfFiles.Add(tempPdfFile);
					}
					catch(Exception ex) {
						sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.Misc,Lans.g("FormBilling","Error creating PDF")+": "+ex.ToString());
						continue;
					}
					sendStatementsIO.FireStatementProgress(100);
					sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","PDF Created")+"...");
					if(statement.DocNum==0) {
						sendStatementsIO.ActionPrompt(Lans.g("FormBilling","Failed to save PDF.  In Setup, DataPaths, please make sure the top radio button is checked."),false);
						return false;
					}
					sendStatementsIO.CurStatementBatch.ListStatementDatas.Add(new StatementData(dataSet,statement.DocNum));
				}
				//imageStore = OpenDental.Imaging.ImageStore.GetImageStore(pat);
				//If stmt.DocNum==0, savedPdfPath will be "".  A blank savedPdfPath is fine for electronic statements.
				Document documentStatement=Documents.GetByNum(statement.DocNum);
				savedPdfPath=sendStatementsIO.FuncGetPatientPdfPath(tempPdfFile,ImageStore.GetFilePath(documentStatement,patFolder));
				if(statement.Mode_==StatementMode.InPerson || statement.Mode_==StatementMode.Mail) {
					//Will be null by default to indicate no printing necessary.
					sendStatementsIO.PdfMasterDocument=sendStatementsIO.PdfMasterDocument??new PdfDocument();
					pdfDocumentInput=sendStatementsIO.FuncGetPdfDocument(documentStatement.RawBase64,savedPdfPath);
					for(int idx = 0; idx<pdfDocumentInput.PageCount; idx++) {
						pdfPage=pdfDocumentInput.Pages[idx];
						sendStatementsIO.PdfMasterDocument.AddPage(pdfPage);
						sendStatementsIO.FirePdfProgress(idx,pdfDocumentInput.PageCount);
						sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","PDF Added to Print List")+"...");
					}
					sendStatementsIO.CountStatementsPrinted++;
					sendStatementsIO.ListStatementNumsSent.Add(statement.StatementNum);
					Statements.MarkSent(statement.StatementNum,statement.DateSent);
				}
				#endregion
				#region Preparing Email
				if(statement.Mode_==StatementMode.Email) {
					sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Preparing Email")+"...");
					sendStatementsIO.FireStatementProgress(40);
					try {
						emailMessage=Statements.GetEmailMessageForStatement(statement,patient,emailAddress);
						emailAttach=sendStatementsIO.FuncGetEmailAttachment(savedPdfPath,documentStatement,patient);
						sendStatementsIO.FireStatementProgress(70);
						emailMessage.Attachments.Add(emailAttach);
						emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
						emailMessage.MsgDateTime=DateTime.Now;
						if(PrefC.GetBool(PrefName.BillingEmailIncludeAutograph)) {
							EmailAutograph emailAutograph=EmailAutographs.GetForOutgoing(listEmailAutographs,emailAddress);
							if(emailAutograph!=null) {
								//Always set the BodyText, we will additionally set HtmlText below if necessary (mimics FormEmailMessageEdit).
								emailMessage.BodyText=EmailMessages.InsertAutograph(emailMessage.BodyText,emailAutograph);
								if(MarkupEdit.ContainsOdHtmlTags(emailAutograph.AutographText)) {
									//Attempt to convert entire message to html to accomodate for html autograph.
									ODException.SwallowAnyException(() => {
										//This will format the entire body at HTML, not just the autograph. This now becomes an undocumented loophole to deploy an html statement email.
										string markup=MarkupEdit.TranslateToXhtml(EmailMessages.InsertAutograph(emailMessage.BodyText,emailAutograph),false,isEmail:true);
										//We got this far so change the message body and html type.
										emailMessage.HtmlText=markup;
										emailMessage.HtmlType=EmailType.Html;
									});
								}
							}
						}
						long clinicNumPat=0;
						if(PrefC.HasClinicsEnabled) {
							clinicNumPat=patient.ClinicNum;
							if(clinicNumPat==0) { //set 0 clinic to use default clinic settings
								clinicNumPat=PrefC.GetLong(PrefName.EmailSecureDefaultClinic);
							}
						}
						bool useSecureEmail=
							Enum.TryParse(ClinicPrefs.GetPrefValue(PrefName.EmailStatementsSecure,clinicNumPat),out EmailPlatform emailPlatform)
							&& emailPlatform==EmailPlatform.Secure
							&& Clinics.IsSecureEmailEnabled(clinicNumPat);
						if(useSecureEmail) {
							emailMessage.SentOrReceived=EmailSentOrReceived.SecureEmailSent;
						}
						sendStatementsIO.ActionSendEmail(clinicNumPat,emailMessage,emailAddress,useSecureEmail);
						sendStatementsIO.FireStatementProgress(90);
						sendStatementsIO.CountStatementsEmailed++;
						sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Email Sent")+"...");
					}
					catch(Exception ex) {
						sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.Misc,Lans.g("FormBilling","Error sending email")+": "+ex.ToString());
						sendStatementsIO.FireStatementProgress(100);
						continue;
					}
					sendStatementsIO.ListStatementNumsSent.Add(statement.StatementNum);
					Statements.MarkSent(statement.StatementNum,statement.DateSent);
				}
				#endregion
				#region Preparing E-Bills
				if(statement.Mode_==StatementMode.Electronic) {
					sendStatementsIO.FireStatementProgress(65);
					sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Preparing E-Bills")+"...");
					Patient guarantor = family.ListPats[0];
					if(guarantor.Address.Trim()==""||guarantor.City.Trim()==""||guarantor.State.Trim()==""||guarantor.Zip.Trim()=="") {
						sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadMailingAddress,Lans.g("FormBilling","Error with patient address"));
						continue;
					}
					//Eventually will not use Statement.IsRecipt or Statement.IsInvoice but rather StmtType.Invoice and StmtType.Receipt.
					if(statement.StatementType==StmtType.LimitedStatement || statement.IsReceipt || statement.IsInvoice) {
						sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.Misc,Lans.g("FormBilling","Limited statements, Receipts, and Invoices cannot be sent electronically."));
						continue;
					}
					EbillStatement ebillStatement=new EbillStatement();
					ebillStatement.Family=family;
					ebillStatement.Statement=statement;
					long clinicNum = 0;//If clinics are disabled, then all bills will go into the same "bucket"
					if(PrefC.HasClinicsEnabled) {
						clinicNum=family.Guarantor.ClinicNum;
					}
					if(electronicBillingType==BillingUseElectronicEnum.EHG) {
						List<string> listElectErrors=Bridges.EHG_statements.Validate(clinicNum);
						if(!listElectErrors.IsNullOrEmpty()) {
							sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.Misc,listElectErrors.Last());
							continue;//skip the current statement, since there are errors.
						}						
					}
					//We made it this far so this statement will be processed by SendEBills.
					//Let's decrement here since we already incremented at the top of this loop.
					//We will re-increment in SendEBills when we truly process this statement.
					sendStatementsIO.CurStatementBatch.CountStatementsProcessed--;
					sendStatementsIO.CurStatementBatch.ListEbillStatements.Add(ebillStatement);
					sendStatementsIO.FireStatementProgress(70);
					sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","E-Bill Added To Send List")+"...");
				}
				#endregion
			}
			//Fire progress event one last time after our last loop iteration.
			sendStatementsIO.FireOverallProgress();
			return true;
		}

		///<summary>Attempt to send electronic bills if needed. Returns false if the sending was canceled.
		///RunAgingEnterprise must run and pass before calling this method.</summary>
		private static bool SendEBills(SendStatementsIO sendStatementsIO) {
			sendStatementsIO.FireStatementProgress(80);
			sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Sending E-Bills")+"...");
			if(!BillingProgressPause(sendStatementsIO)) {
				return false;
			}
			if(sendStatementsIO.CurStatementBatch.ListEbillStatements.Count==0) {//All statements have been sent for the current batch.  Nothing more to do.
				sendStatementsIO.LogWrite(Lans.g("FormBilling","No ebills need to be sent."),LogLevel.Information);
				return true;
			}
			sendStatementsIO.LogWrite(Lans.g("FormBilling","Sending ebills for ClinicNum")+" "+sendStatementsIO.CurStatementBatch.ClinicNum.ToString(),LogLevel.Information);
			BillingUseElectronicEnum electronicBillingType=PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="   ";
			//Holds all statements. Will be sent to 1 of 4 electronic billing vendors.
			//Each vendor will perform 3 phases on this string.
			//1) Append general practice info
			//2) Append each individual statement's info to an xml writer
			//3) Send xlm string to vendor
			StringBuilder strBuildElect=new StringBuilder();
			XmlWriter xmlWriterElect=XmlWriter.Create(strBuildElect,xmlWriterSettings);
			#region 1) Append general practice info
			if(electronicBillingType==BillingUseElectronicEnum.EHG) {
				Bridges.EHG_statements.GeneratePracticeInfo(xmlWriterElect,sendStatementsIO.CurStatementBatch.ClinicNum);
			}
			else if(electronicBillingType==BillingUseElectronicEnum.POS) {
				Bridges.POS_statements.GeneratePracticeInfo(xmlWriterElect,sendStatementsIO.CurStatementBatch.ClinicNum);
			}
			else if(electronicBillingType==BillingUseElectronicEnum.ClaimX) {
				Bridges.ClaimX_Statements.GeneratePracticeInfo(xmlWriterElect,sendStatementsIO.CurStatementBatch.ClinicNum);
			}
			else if(electronicBillingType==BillingUseElectronicEnum.EDS) {
				Bridges.EDS_Statements.GeneratePracticeInfo(xmlWriterElect,sendStatementsIO.CurStatementBatch.ClinicNum);
			}
			else {
				sendStatementsIO.LogWrite(Lans.g("FormBilling","\'No billing electronic\' is currently selected in Billing Defaults."),LogLevel.Error);
			}
			#endregion 1) Append general practice info
			#region 2) Append each individual statement's info to an xml writer
			Family family;
			Patient patient;
			DataSet dataSet;
			List<long> listElectStmtNums=new List<long>();
			sendStatementsIO.FireStatementProgress(85);
			//This loop has iterated backwards since day 1. Many years ago, we would batch and remove from the end of this list instead of just making a new list.
			//Batching was solved in a different way sometime along the way so iterating backwards is no longer important. Leaving backwards iteration in-tact just in case.
			for(int i=0; i<sendStatementsIO.CurStatementBatch.ListEbillStatements.Count;i++) {
				if(!BillingProgressPause(sendStatementsIO)) {
					return false;
				}
				//Fire progress event before we iterate so we deal with counts before this loop iteration. That is most accurate.
				sendStatementsIO.FireOverallProgress();
				//It is finally time to increment for this statement.
				sendStatementsIO.CurStatementBatch.CountStatementsProcessed++;
				Statement statementCur=sendStatementsIO.CurStatementBatch.ListEbillStatements[i].Statement;
				if(statementCur==null) {//The statement was probably deleted by another user.
					sendStatementsIO.CountStatementsSkippedForDeletion++;
					continue;
				}
				if(sendStatementsIO.ListStatementNumsToSkipAfterPause.Contains(statementCur.StatementNum)) {
					//The statement was deleted or marked sent elsewhere while this billing session was paused and subsequently resumed.
					sendStatementsIO.AddSkippedPatient(statementCur.PatNum,SkipReason.Misc,Lans.g("FormBilling","Statement was adjusted elsewhere."));
					continue;
				}
				family=sendStatementsIO.CurStatementBatch.ListEbillStatements[i].Family;
				patient=family.GetPatient(statementCur.PatNum);
				dataSet=AccountModules.GetStatementDataSet(statementCur,isComputeAging: false,doIncludePatLName: PrefC.IsODHQ);
				try {
					//Write the statement into a temporary string builder, so that if the statement fails to generate (due to exception),
					//then the partially generated statement will not be added to the strBuildElect.
					StringBuilder strBuildStatement = new StringBuilder();
					using(XmlWriter xmlWriterStatement = XmlWriter.Create(strBuildStatement,xmlWriterElect.Settings)) {
						if(electronicBillingType==BillingUseElectronicEnum.None) {
							throw new Exception(Lans.g("FormBilling","\'No billing electronic\' is currently selected in Billing Defaults."));
						}
						else if(electronicBillingType==BillingUseElectronicEnum.EHG) {
							Bridges.EHG_statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(electronicBillingType==BillingUseElectronicEnum.POS) {
							Bridges.POS_statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(electronicBillingType==BillingUseElectronicEnum.ClaimX) {
							Bridges.ClaimX_Statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(electronicBillingType==BillingUseElectronicEnum.EDS) {
							Bridges.EDS_Statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
					}
					//Write this statement's XML to the XML document with all the statements.
					using(XmlReader readerStatement = XmlReader.Create(new StringReader(strBuildStatement.ToString()))) {
						xmlWriterElect.WriteNode(readerStatement,true);
					}
					//We made it this far for this statement so it was generated successfully.
					listElectStmtNums.Add(statementCur.StatementNum);
				}
				catch(Exception ex) {
					sendStatementsIO.AddSkippedPatient(patient.PatNum,SkipReason.Misc,Lans.g("FormBilling","Error sending statement")+": "+ex.ToString());
				}
			}
			xmlWriterElect.Close();
			if(listElectStmtNums.Count==0) {//All statements for this batch were either deleted or had an exception thrown while generating.
				return true;//Go on to next batch
			}
			sendStatementsIO.FireStatementProgress(90);
			#endregion 2) Append each individual statement's info
			#region 3) Send xlm string to vendor
			//Each vendor uses initial directory and xml pref slightly different.
			string xmlFilePathFromPref="";
			string initialSaveDirectory="";
			bool doMarkSent=false;
			if(electronicBillingType==BillingUseElectronicEnum.EHG) {
				//This is a web call to DentalXChange so we will try 3 times before we consider it failed.
				for(int attempts = 0;attempts<3;attempts++) {
					string alertMsg=null;
					try {
						alertMsg=sendStatementsIO.FuncSendEhgStatement(strBuildElect.ToString(),sendStatementsIO.CurStatementBatch.ClinicNum);
						if(!string.IsNullOrEmpty(alertMsg)) {
							sendStatementsIO.ActionPrompt(alertMsg,false);
						}
						//We made it this far so batch succeeded.
						doMarkSent=true;
						break;
					}
					catch(Exception ex) {
						if(attempts<2) {//Don't indicate the error unless it failed on the last attempt.
							continue;//The only thing skipped besides the error message is evaluating if the statement was written, which it wasn't.
						}
						//This batch was not sent
						if(ex.Message.Contains("(404) Not Found")) {//Custom 404 error message
							sendStatementsIO.AppendMiscSystemError(Lans.g("FormBilling","The connection to the server could not be established or was lost, or the upload timed out.  "
							+"Ensure your internet connection is working and that your firewall is not blocking this application.  "
							+"If the upload timed out after 10 minutes, try sending 25 statements or less in each batch to reduce upload time."));
						}
						else {//Document any other errors to make troubleshooting much easier.
							sendStatementsIO.AppendMiscSystemError(Lans.g("FormBilling",ex.Message));
						}
						//An API exception will return true below, which will allow subsequent batches to continue to run.
					}
				}
			}
			else if(electronicBillingType==BillingUseElectronicEnum.POS) {
				xmlFilePathFromPref=PrefC.GetString(PrefName.BillingElectStmtOutputPathPos);
				initialSaveDirectory="";
			}
			else if(electronicBillingType==BillingUseElectronicEnum.ClaimX) {
				xmlFilePathFromPref="";
				//Clint from ExtraDent requested this default path.
				initialSaveDirectory=@"C:\StatementX\";
			}
			else if(electronicBillingType==BillingUseElectronicEnum.EDS) {
				xmlFilePathFromPref=PrefC.GetString(PrefName.BillingElectStmtOutputPathEds);
				initialSaveDirectory="";
			}
			if(electronicBillingType!=BillingUseElectronicEnum.EHG) {
				//DentalXChange does not write to a file. All others do.
				if(!sendStatementsIO.SetXmlFilePath(xmlFilePathFromPref,initialSaveDirectory)) {
					if(!sendStatementsIO.AllowXmlFileSelection) {
						sendStatementsIO.AppendMiscSystemError(Lans.g("FormBilling",$"The preference for {electronicBillingType} does not have a valid path."));
					}
					//User elected to cancel when prompted for a file path on current or previous iteration.
					//Dont make them cancel again, just cancel for all batches and move on to next batch.
					return true;
				}
				//Convert base path to clinic specific path.
				string xmlFilePathClinic=Statements.GetEbillFilePathForClinic(sendStatementsIO.XmlFilePath,sendStatementsIO.CurStatementBatch.ClinicNum);
				File.WriteAllText(xmlFilePathClinic,strBuildElect.ToString());
				doMarkSent=true;
			}
			if(doMarkSent) {
				//Loop through all statements in the batch and mark them sent.
				for(int i=0;i<listElectStmtNums.Count;i++) {
					//Adding here assures that only IsSent=true statements will be attempted in SendTextMessages.
					sendStatementsIO.ListStatementNumsSent.Add(listElectStmtNums[i]);
					Statements.MarkSent(listElectStmtNums[i],DateTime.Today);
					sendStatementsIO.CountStatementsSentElectronic++;
					sendStatementsIO.FireStatementProgress(100);
				}
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","E-Bills Sent")+"...");
			}
			else {
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","E-Bills Not Sent")+"...");
			}
			#endregion 3) Send to vendor
			//Fire progress event one last time after our last loop iteration.
			sendStatementsIO.FireOverallProgress();
			return true;
		}

		///<summary>Sends text messages to the current batch of statements. Changes to this method will also need to be made to OpenDentalService.ScheduledProcessThread.SendTextMessages().
		///In order to receive a text, the clinic of the patient whom the statement belongs to must be signed up for texting, the patient must meet the criteria of PatComm.IsSmsAnOption, and patient must not be opted out.
		///Most importantly, the BillingOptions for this statement Mode must indicate that this Mode is to receive a text message. Statement.SmsSendStatus is used to manage this flag.</summary>
		private static bool SendTextMessages(SendStatementsIO sendStatementsIO) {
			List<SmsToMobile> listTextsToSend=new List<SmsToMobile>();
			List<Patient> listPatients=sendStatementsIO.DictionaryFamilies.Values.SelectMany(x => x.ListPats).DistinctBy(x => x.PatNum).ToList();
			List<PatComm> listPatComms=Patients.GetPatComms(listPatients);
			string guidBatch=null;
			for(int i=0; i<sendStatementsIO.CurStatementBatch.ListStatements.Count; i++) {
				Statement statement=sendStatementsIO.CurStatementBatch.ListStatements[i];
				if(!BillingProgressPause(sendStatementsIO)) {
					return false;
				}
				if(sendStatementsIO.ListSkippedPatients.Any(x => x.PatNum==statement.PatNum)) {
					//Already skipped this patient for some other reason. Don't bother sending a text.
					//Skipping here is new behavior for 24.2 Billing refactor. Previously would send statement text for skipped patients (bug).
					continue;
				}
				if(!sendStatementsIO.ListStatementNumsSent.Contains(statement.StatementNum)) {
					//This statement was not sent for whatever reason so don't bother sending a text message.
					continue;
				}
				//Statements are generated in FormBillingOptions.
				//This form will link certain statement modes (Email, Electronic, InPerson, etc) to receive a corresponding text message.
				//If the Mode of this statement is not set to send a text message, then we will skip the text message here.
				if(statement.SmsSendStatus!=AutoCommStatus.SendNotAttempted) {
					continue;
				}
				if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.BillingDefaultsSmsTemplate))) {
					//User has opted to allow billing to run despite not having a valide sms template.
					//This combination would previously cause this method to return false, which would cause billing to halt after already having sent first batch of statements.
					//2/29/24 - SamO decided that this is not a haltable offense since user opted to allow it to happen. Instead we will just add this failed comm to the error list.
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadSmsSetup,Lans.g("FormBilling","SMS Statements template not setup"));
					continue;
				}
				PatComm patComm=listPatComms.Find(x => x.PatNum==statement.PatNum);
				if(patComm==null) {
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadSmsSetup,Lans.g("FormBilling","Unable to find patient communication method"));
					continue;
				}
				if(!patComm.IsSmsAnOption) {
					continue;
				}
				if(patComm.CommOptOut.IsOptedOut(CommOptOutMode.Text,CommOptOutType.Statements)) {
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadSmsSetup,Lans.g("FormBilling","Patient is opted out of automated messaging."));
					continue;
				}
				Patient patient=listPatients.Find(x => x.PatNum==statement.PatNum)??Patients.GetPat(statement.PatNum);
				if(patient==null) {
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadSmsSetup,Lans.g("FormBilling","Unable to find patient"));
					continue;
				}
				SmsToMobile textToSend=new SmsToMobile();
				textToSend.ClinicNum=patient.ClinicNum;
				textToSend.GuidMessage=Guid.NewGuid().ToString();
				textToSend.IsTimeSensitive=false;
				textToSend.MobilePhoneNumber=patComm.SmsPhone;
				textToSend.PatNum=statement.PatNum;
				try {
					textToSend.MsgText=new MsgToPayTagReplacer().ReplaceTagsForStatement(PrefC.GetString(PrefName.BillingDefaultsSmsTemplate),patient,statement);
				}
				catch(Exception e) {
					sendStatementsIO.AddSkippedPatient(statement.PatNum,SkipReason.BadSmsSetup,Lans.g("FormBilling","Failed to format text message correctly")+ ": "+e.Message);
					continue;
				}
				textToSend.MsgType=SmsMessageSource.Statements;
				//First message guid is the batch guid.
				guidBatch=guidBatch??textToSend.GuidMessage;
				textToSend.GuidBatch=guidBatch;
				listTextsToSend.Add(textToSend);
				//Store the guid here so we can reference it below and link this statement back to it's message.
				statement.TagOD=textToSend.GuidMessage;
			}
			if(!BillingProgressPause(sendStatementsIO)) {
				return false;
			}
			if(listTextsToSend.Count==0) {
				return true;
			}
			try {
				sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Sending text messages")+"...");
				List<SmsToMobile> listSmsToMobiles=SmsToMobiles.SendSmsMany(listTextsToSend,userod:Security.CurUser);
				List<SmsToMobile> listSmsToMobilesFails=Statements.HandleSmsSent(listSmsToMobiles,sendStatementsIO.CurStatementBatch.ListStatements);
				for(int i = 0;i<listSmsToMobilesFails.Count;++i) {
					sendStatementsIO.AddSkippedPatient(listSmsToMobilesFails[i].PatNum,SkipReason.BadSmsSetup,Lans.g("Statements","Error Sending text messages")+": "+listSmsToMobilesFails[i].CustErrorText);
				}
				sendStatementsIO.CountStatmentsSentPayPortalText+=listSmsToMobiles.Where(x => x.SmsStatus!=SmsDeliveryStatus.FailNoCharge).Count();
			}
			catch(Exception ex) {
				sendStatementsIO.AppendMiscSystemError(Lans.g("FormBilling","Error Sending text messages")+": "+ex.ToString());
				List<long> listFailedStatementNums=sendStatementsIO.CurStatementBatch.ListStatements
					//Statement.TagOD was set to SmsToMobile.GuidMessage above. Match all of those back here so we can fail them all.
					.FindAll(x => x.TagOD is string guidMessage && listTextsToSend.Any(y => y.GuidMessage==guidMessage))
					.Select(x => x.StatementNum).ToList();
				Statements.UpdateSmsSendStatus(listFailedStatementNums,AutoCommStatus.SendFailed);
			}
			return true;
		}

		///<summary>Check to see if the user wants to pause the sending of statements.  If so, wait until they decide to resume.
		///Returns true unless the user clicks cancel in the progress window.
		///The method will wait infinitely if paused from the progress window.
		///If pause event is resumed, billing will restart where it left off and exclude any statements that may have been deleted while paused.</summary>
		public static bool BillingProgressPause(SendStatementsIO sendStatementsIO) {
			sendStatementsIO.ListStatementNumsToSkipAfterPause=new List<long>();
			bool hasEventFired=false;
			//Pause until resume.
			while(sendStatementsIO.FunctGetIsPaused()) {
				if(!hasEventFired) {//Don't fire this event more than once.
					sendStatementsIO.FireTextMsgProgress(Lans.g("FormBilling","Warning"),isWarningOffEvent:true);
					hasEventFired=true;
				}
				sendStatementsIO.ActionSleepDuringPause();
				//Resume?
				if(!sendStatementsIO.FunctGetIsPaused()) { 
					//Get remaining statements given original constraints from UI.
					DataTable table=sendStatementsIO.FuncGetBillingDataTable();
					List<long> listStatementNumsFromDb=table.Select().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
					//Get statement nums yet to be sent from original run.
					List<long> listStatementNumsUnsent=sendStatementsIO.ListStatementNumsToSend.Except(sendStatementsIO.ListStatementNumsSent).ToList();
					//Capture any statements that were deleted while this billing progress was paused.
					sendStatementsIO.ListStatementNumsToSkipAfterPause=listStatementNumsUnsent.Except(listStatementNumsFromDb).ToList();
				}
				if(sendStatementsIO.FuncGetIsCancelled()) { //State changed from paused to cancelled.
					return false;
				}
			}
			//Check to see if the user wants to stop sending statements.
			if(sendStatementsIO.FuncGetIsCancelled()) {
				return false;
			}
			return true;
		}

		///<summary>Runs enterprise aging. Returns false if there are any errors while running aging.</summary>
		public static bool RunAgingEnterprise(SendStatementsIO sendStatementsIO) {
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			DateTime dateTimeToday=dateTimeNow.Date;
			DateTime dateTimeLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateTimeLastAging.Date==dateTimeToday) {
				return true;//already ran aging for this date, just move on
			}
			Prefs.RefreshCache();
			if(!PrefC.IsAgingAllowedToStart()) {
				string prompt=Lans.g("FormBilling","In order to print or send statments, aging must be re-calculated, but you cannot run aging until it has "
					+"finished the current calculations which began on")+" "+PrefC.GetDateT(PrefName.AgingBeginDateTime).ToString()+".\r\n"+Lans.g("FormBilling","If you believe the current "
					+"aging process has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Preferences | Account - General "
					+"and pressing the 'Clear' button.");
				sendStatementsIO.ActionPrompt(prompt,false);
				return false;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.AgingRan,0,"Starting Aging - "+sendStatementsIO.Source);
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dateTimeNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			sendStatementsIO.LogWrite(Lans.g("FormBilling","Calculating enterprise aging for all patients as of")+" "+dateTimeToday.ToShortDateString()+"...",LogLevel.Information);
			bool ret=sendStatementsIO.FuncComputeAging(dateTimeToday);
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			if(ret) {
				//Only move aging date forward if success.
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateTimeToday,false));
				SecurityLogs.MakeLogEntry(EnumPermType.AgingRan,0,"Aging complete - "+sendStatementsIO.Source);
			}
			Signalods.SetInvalid(InvalidType.Prefs);
			return ret;
		}

		///<summary>Creates statements for a single clinic.  Returns statements created, or empty list.
		///Uses ClinicPrefs preference billing options when generating statements. Otherwise, uses the practice default preference values. 
		///It will also use the default practice preference values for ClinicPrefs that are missing for the Clinic. 
		///This method closely mimics OpenDental.FormBillingOptions.CreateHelper(). Changes to this method will also need to be considered there.</summary>
		///<param name="clinicNum">The clinic that billing is being generated for. Passing in a value of -1 indicates clinics not being enabled.</param>
		public static List<Statement> CreateStatementHelper(long clinicNum,SendStatementsIO sendStatementsIO) {
			//If clinics are enabled, the list will be filled with the passed in clinicNum. Otherwise, it will be empty.
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled&&clinicNum>=0) {
				listClinicNums.Add(clinicNum);
			}
			ClinicBillingOptions clinicBillingOptions=new ClinicBillingOptions(clinicNum);
			//Practice specific preferences.
			List<StatementMode> listStatementModesToSendSMS=new List<StatementMode>();
			if(SmsPhones.IsIntegratedTextingEnabled()) {
				//Each statement mode can be coupled with a text message to the payment portal. This link is made in FormBillingOptions and saved to this pref.
				listStatementModesToSendSMS=EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(PrefC.GetString(PrefName.BillingDefaultsModesToText));
			}
			string strLastStatementDate=DateTime.Today.AddMonths(-1).ToShortDateString();
			string strDateStart=DateTime.Today.AddDays(-PrefC.GetLong(PrefName.BillingDefaultsLastDays)).ToShortDateString();
			string strDateEnd=DateTime.Today.ToShortDateString();
			bool doBillingDefaultsIntermingled=PrefC.GetBool(PrefName.BillingDefaultsIntermingle);
			bool doBillingDefaultSinglePatient=PrefC.GetBool(PrefName.BillingDefaultsSinglePatient);
			//Clinic specific preferences
			bool doBillingIncludeChanged=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingIncludeChanged));
			bool doBillingExcludeInsPending=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeInsPending));
			bool doBillingExcludeIfUnsentProcs=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeIfUnsentProcs));
			string strBillingAgeOfAccount=clinicBillingOptions.GetPrefValue(PrefName.BillingAgeOfAccount);
			string strbillingSelectBillingTypes=clinicBillingOptions.GetPrefValue(PrefName.BillingSelectBillingTypes);
			string strBillingExcludeLessThan=clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeLessThan);
			bool doBillingExcludeBadAddresses=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeBadAddresses));
			bool doBillingShowNegative=!PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeNegative));
			bool doBillingExcludeInactive=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingExcludeInactive));
			bool doBillingIgnoreInPerson=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingIgnoreInPerson));
			string strBillingDefaultNote=clinicBillingOptions.GetPrefValue(PrefName.BillingDefaultsNote);
			bool doBillingShowTransSinceBalZero=PIn.Bool(clinicBillingOptions.GetPrefValue(PrefName.BillingShowTransSinceBalZero));
			bool isSuperFam=false;//Defaults to false.
			DateTime dateLastStatement=PIn.Date(strLastStatementDate);
			if(strLastStatementDate=="") {
				dateLastStatement=DateTime.Today;
			}
			List<long> listBillingDefNums=new List<long>();
			if(string.IsNullOrWhiteSpace(strbillingSelectBillingTypes)) {//All billing types
				listBillingDefNums=Defs.GetDefsForCategory(DefCat.BillingTypes,true).Select(x => x.DefNum).ToList();
			}
			else {
				listBillingDefNums=strbillingSelectBillingTypes.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
			}
			List<PatAging> listPatAgings=new List<PatAging>();
			Dictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(doBillingDefaultSinglePatient,doBillingIncludeChanged,doBillingExcludeInsPending,
					doBillingExcludeIfUnsentProcs,isSuperFam,listClinicNums);
			//This method was frequently causing a Middle Tier error. Grab all information, filtered by clinic if clinics enabled, and use the
			//PatAgingData dictionary to create a list of PatNums with unsent procs, a list of PatNums with pending ins, and a dictionary of PatNum key to
			//last trans date value and send only that data as it's all that GetAgingList needs.
			List<long> listPendingInsPatNums=new List<long>();
			List<long> listUnsentPatNums=new List<long>();
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long,List<PatAgingTransaction>>();
			if(doBillingExcludeInsPending||doBillingExcludeIfUnsentProcs||doBillingIncludeChanged) {
				foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
					if(PrefC.HasClinicsEnabled&&!listClinicNums.IsNullOrEmpty()&&!listClinicNums.Contains(kvp.Value.ClinicNum)) {
						continue;
					}
					if(doBillingExcludeInsPending&&kvp.Value.HasPendingIns) {//don't fill list if not excluding if pending ins
						listPendingInsPatNums.Add(kvp.Key);
					}
					if(doBillingExcludeIfUnsentProcs&&kvp.Value.HasUnsentProcs) {//don't fill list if not excluding if unsent procs
						listUnsentPatNums.Add(kvp.Key);
					}
					if(doBillingIncludeChanged) {//don't fill dictionary if not including if changed
						dictPatAgingTransactions[kvp.Key]=kvp.Value.ListPatAgingTransactions;
					}
				}
			}
			try {
				listPatAgings=Patients.GetAgingList(strBillingAgeOfAccount,dateLastStatement,listBillingDefNums,doBillingExcludeBadAddresses,!doBillingShowNegative,
					PIn.Double(strBillingExcludeLessThan),doBillingExcludeInactive,doBillingIgnoreInPerson,listClinicNums,isSuperFam,
					doBillingDefaultSinglePatient,listPendingInsPatNums,listUnsentPatNums,dictPatAgingTransactions);
			}
			catch(Exception ex) {
				string strErrorMsg=Lans.g("FormBilling","Error getting list:")+" "+ex.Message+"\r\n\n\n"+ex.StackTrace;
				if(ex.InnerException!=null) {
					strErrorMsg+="\r\n\r\nInner Exception: "+ex.InnerException.Message+"\r\n\r\n"+ex.InnerException.StackTrace;
				};
				sendStatementsIO.LogWrite(strErrorMsg,LogLevel.Error);
				return new List<Statement>();
			}
			List<Patient> listPatientsSuperHeads=new List<Patient>();
			//If making a super family bill, we need to manipulate the agingList to only contain super family head members.
			//It can also contain regular family members, but should not contain any individual super family members other than the head.
			if(isSuperFam) {
				List<PatAging> listPatAgingsSuperFamilies=new List<PatAging>();
				for(int i=listPatAgings.Count-1;i>=0;i--) {//Go through each PatAging in the retrieved list
					if(listPatAgings[i].SuperFamily==0||!listPatAgings[i].HasSuperBilling) {
						continue;//It is okay to leave non-super billing PatAgings in the list.
					}
					Patient patientSuperFamilyHead=listPatientsSuperHeads.FirstOrDefault(x => x.PatNum==listPatAgings[i].SuperFamily);
					if(patientSuperFamilyHead==null) {
						patientSuperFamilyHead=Patients.GetPat(listPatAgings[i].SuperFamily);
						listPatientsSuperHeads.Add(patientSuperFamilyHead);
					}
					if(!patientSuperFamilyHead.HasSuperBilling) {
						listPatAgings[i].HasSuperBilling=false;//Family guarantor has super billing but superhead doesn't, so no super bill.  Mark statement as no superbill.
						continue;
					}
					//If the guar has super billing enabled and the superhead has superbilling, this entry needs to be merged to superbill.
					if(listPatAgings[i].HasSuperBilling&&patientSuperFamilyHead.HasSuperBilling) {
						PatAging patAging=listPatAgingsSuperFamilies.FirstOrDefault(x => x.PatNum==patientSuperFamilyHead.PatNum);//Attempt to find an existing PatAging for the superhead.
						if(patAging==null) {
							//Create new PatAging object using SuperHead "credentials" but the guarantor's balance information.
							patAging=new PatAging();
							patAging.AmountDue=listPatAgings[i].AmountDue;
							patAging.BalTotal=listPatAgings[i].BalTotal;
							patAging.Bal_0_30=listPatAgings[i].Bal_0_30;
							patAging.Bal_31_60=listPatAgings[i].Bal_31_60;
							patAging.Bal_61_90=listPatAgings[i].Bal_61_90;
							patAging.BalOver90=listPatAgings[i].BalOver90;
							patAging.InsEst=listPatAgings[i].InsEst;
							patAging.PatName=patientSuperFamilyHead.GetNameLF();
							patAging.DateLastStatement=listPatAgings[i].DateLastStatement;
							patAging.BillingType=patientSuperFamilyHead.BillingType;
							patAging.PayPlanDue=listPatAgings[i].PayPlanDue;
							patAging.PatNum=patientSuperFamilyHead.PatNum;
							patAging.HasSuperBilling=listPatAgings[i].HasSuperBilling;//true
							patAging.SuperFamily=listPatAgings[i].SuperFamily;//Same as superHead.PatNum
							patAging.ClinicNum=listPatAgings[i].ClinicNum;
							listPatAgingsSuperFamilies.Add(patAging);
						}
						else {
							//Sum the information together for all guarantors of the superfamily.
							patAging.AmountDue+=listPatAgings[i].AmountDue;
							patAging.BalTotal+=listPatAgings[i].BalTotal;
							patAging.Bal_0_30+=listPatAgings[i].Bal_0_30;
							patAging.Bal_31_60+=listPatAgings[i].Bal_31_60;
							patAging.Bal_61_90+=listPatAgings[i].Bal_61_90;
							patAging.BalOver90+=listPatAgings[i].BalOver90;
							patAging.InsEst+=listPatAgings[i].InsEst;
							patAging.PayPlanDue+=listPatAgings[i].PayPlanDue;
						}
						listPatAgings.RemoveAt(i);//Remove the individual guarantor statement from the aging list since it's been combined with a superstatement.
					}
				}
				listPatAgings.AddRange(listPatAgingsSuperFamilies);
			}
			#region Message Construction
			if(listPatAgings.Count==0) {
				sendStatementsIO.LogWrite(Lans.g("FormBilling","List of created bills is empty."),LogLevel.Verbose);
				return new List<Statement>();
			}
			else {
				sendStatementsIO.LogWrite(Lans.g("FormBilling","Statements created")+": "+POut.Int(listPatAgings.Count),LogLevel.Verbose);
			}
			#endregion
			bool isHistoryStartMinDate=string.IsNullOrWhiteSpace(strDateStart);
			DateTime dateRangeFrom=PIn.Date(strDateStart);
			DateTime dateRangeTo=DateTime.Today;//Needed for payplan accuracy.//new DateTime(2200,1,1);
			if(strDateEnd!="") {
				dateRangeTo=PIn.Date(strDateEnd);
			}
			Statement statement;
			List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidUrls=new List<WebServiceMainHQProxy.ShortGuidResult>();
			SheetDef sheetDefStatement=SheetUtil.GetStatementSheetDef();
			if(//They are going to send texts
				listStatementModesToSendSMS.Count>0
				//Or the email body has a statement URL
				||PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains(MsgToPayTagReplacer.STATEMENT_URL_TAG.ToLower())
				||PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains(MsgToPayTagReplacer.STATEMENT_SHORT_TAG.ToLower())
				||PrefC.GetString(PrefName.BillingEmailBodyText).ToLower().Contains(MsgToPayTagReplacer.MSG_TO_PAY_TAG.ToLower())
				//Or the statement sheet has a URL field
				||(sheetDefStatement!=null&&sheetDefStatement.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.OutputText
							&&(x.FieldValue.ToLower().Contains(MsgToPayTagReplacer.STATEMENT_URL_TAG.ToLower())||x.FieldValue.ToLower().Contains(MsgToPayTagReplacer.STATEMENT_SHORT_TAG.ToLower()))))) {
				//Then get some short GUIDs and URLs from OD HQ.
				try {
					int countForSms=listPatAgings.Count(x => Statements.DoSendSms(x,dictPatAgingData,listStatementModesToSendSMS));
					//Previously we were reserving more guids then neccessary. Would just dump extra GUIDS.
					listShortGuidUrls=WebServiceMainHQProxy.GetShortGUIDs(countForSms,countForSms,clinicNum,eServiceCode.PatientPortalViewStatement);
				}
				catch(Exception ex) {
					sendStatementsIO.LogWrite(Lans.g("FormBilling","Unable to create a unique URL for each statement. The Patient Portal URL will be used instead.\r\n")+ex.Message,LogLevel.Error);
					listShortGuidUrls=listPatAgings.Select(x => new WebServiceMainHQProxy.ShortGuidResult {
						MediumURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortURL=PrefC.GetString(PrefName.PatientPortalURL),
						ShortGuid=""
					}).ToList();
				}
			}
			Dictionary<long,InstallmentPlan> dictInstallmentPlans=InstallmentPlans.GetForFams(listPatAgings.Select(x => x.PatNum).ToList());
			List<Statement> listStatementsForInsert=new List<Statement>();
			DateTime dateBalBeganCur;
			for(int i = 0;i<listPatAgings.Count;++i) {
				statement=new Statement();
				statement.DateRangeFrom=dateRangeFrom;
				statement.DateRangeTo=dateRangeTo;
				statement.DateSent=DateTime.Today;
				statement.DocNum=0;
				statement.HidePayment=false;
				statement.Intermingled=doBillingDefaultsIntermingled;
				statement.IsSent=false;
				statement.Mode_=Statements.GetStatementMode(listPatAgings[i]);
				if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
					statement.Intermingled=true;
				}
				bool doSendSms=Statements.DoSendSms(listPatAgings[i],dictPatAgingData,listStatementModesToSendSMS);
				statement.SmsSendStatus=(doSendSms ? AutoCommStatus.SendNotAttempted : AutoCommStatus.DoNotSend);
				statement.ShortGUID="";
				statement.StatementURL="";
				statement.StatementShortURL="";
				WebServiceMainHQProxy.ShortGuidResult shortGuidUrl=listShortGuidUrls.FirstOrDefault(x => x.IsForSms==doSendSms);
				if(shortGuidUrl!=null) {
					statement.ShortGUID=shortGuidUrl.ShortGuid;
					statement.StatementURL=shortGuidUrl.MediumURL;
					statement.StatementShortURL=shortGuidUrl.ShortURL;
					listShortGuidUrls.Remove(shortGuidUrl);
				}
				statement.Note=strBillingDefaultNote;
				InstallmentPlan installmentPlan;
				if(dictInstallmentPlans.TryGetValue(listPatAgings[i].PatNum,out installmentPlan)&&installmentPlan!=null) {
					statement.Note=strBillingDefaultNote.Replace("[InstallmentPlanTerms]","Installment Plan\r\n"
						+"Date First Payment: "+installmentPlan.DateFirstPayment.ToShortDateString()+"\r\n"
						+"Monthly Payment: "+installmentPlan.MonthlyPayment.ToString("c")+"\r\n"
						+"APR: "+(installmentPlan.APR/100).ToString("P")+"\r\n"
						+"Note: "+installmentPlan.Note);
				}
				PatAgingData patAgingData;
				dictPatAgingData.TryGetValue(listPatAgings[i].PatNum,out patAgingData);
				//appointment reminders are not handled here since it would be too slow.
				DateTime dateBalZeroCur=patAgingData?.DateBalZero??DateTime.MinValue;//dateBalZeroCur will be DateTime.MinValue if PatNum isn't in dict
				if(doBillingShowTransSinceBalZero&&dateBalZeroCur.Year>1880) {
					statement.DateRangeFrom=dateBalZeroCur;
				}
				//dateBalBegan is first transaction date for a charge that consumed the last of the credits for the account, so first transaction that isn't
				//fully paid for based on oldest paid first logic
				dateBalBeganCur=patAgingData?.DateBalBegan??DateTime.MinValue;//dateBalBeganCur will be DateTime.MinValue if PatNum isn't in dict
				int ageAccount=0;
				//ageAccount is number of days between the day the account first started to have a positive bal and as of today's date
				if(dateBalBeganCur>DateTime.MinValue) {
					ageAccount=(DateTime.Today-dateBalBeganCur).Days;
				}
				List<Dunning> listAllDunnings=Dunnings.Refresh(listClinicNums);
				List<Dunning> listDunningsForPat=listAllDunnings.FindAll(x => (x.BillingType==0 || x.BillingType==listPatAgings[i].BillingType) //same billing type
					&& x.ClinicNum==listPatAgings[i].ClinicNum
					&& ageAccount>=x.AgeAccount-x.DaysInAdvance //old enough to qualify for this dunning message, taking into account DaysInAdvance
					&& (x.InsIsPending==YN.Unknown || x.InsIsPending==(listPatAgings[i].InsEst>0?YN.Yes:YN.No)));//dunning msg ins pending=unkown or matches this acct
				Dunning dunning=listDunningsForPat.LastOrDefault(x => !x.IsSuperFamily);
				if(listPatAgings[i].HasSuperBilling&&listPatAgings[i].PatNum==listPatAgings[i].SuperFamily&&isSuperFam) {
					dunning=listDunningsForPat.LastOrDefault(x => x.IsSuperFamily);
					statement.SuperFamily=listPatAgings[i].SuperFamily;//If this bill is for the superhead and has superbill enabled, it's a superbill.  Flag it as such.
				}
				if(dunning!=null) {
					if(statement.Note!="") {
						statement.Note+="\r\n\r\n";//leave one empty line
					}
					statement.Note+=dunning.DunMessage;
					statement.NoteBold=dunning.MessageBold;
					statement.EmailSubject=dunning.EmailSubject;
					statement.EmailBody=dunning.EmailBody;
				}
				statement.PatNum=listPatAgings[i].PatNum;
				statement.SinglePatient=doBillingDefaultSinglePatient;
				statement.IsBalValid=true;
				statement.BalTotal=listPatAgings[i].BalTotal;
				statement.InsEst=listPatAgings[i].InsEst;
				listStatementsForInsert.Add(statement);
			}
			if(listStatementsForInsert.Count>0) {
				sendStatementsIO.LogWrite(Lans.g("FormBilling","Inserting Statements to database")+"... "+listStatementsForInsert.Count,LogLevel.Verbose);
				//Previously used InsertMany here. Had to switch and insert each row individually so we could retain the new PK on each statement object.
				//This will be used as input for BillingL.SendStatements.
				for(int i=0; i<listStatementsForInsert.Count; i++) {
					Statements.Insert(listStatementsForInsert[i]);
				}
			}
			return listStatementsForInsert;
		}
	}

	///<summary>Define reasons that a single statement might be skipped in billing.</summary>
	public enum SkipReason {
		///<summary>0</summary>
		BadEmailAddress,
		///<summary>1</summary>
		BadMailingAddress,
		///<summary>2</summary>
		BadSmsSetup,
		///<summary>3</summary>
		Misc,
	}

	public class SendStatementsSkipped {
		public long PatNum;
		public string Error;
		public SkipReason Reason;
	}

	public class SendStatementsIO {
		#region Inputs
		///<summary>Statements attempting to be sent. Generated from raw DataTable which is a reflection of grid selections in FormBilling.
		///Will be converted to list of Statements as needed inside SendStatements.</summary>
		public List<long> ListStatementNumsToSend=new List<long>();
		///<summary>Use for logging.</summary>
		public LogWriter LogWriter=null;
		///<summary>ODService will not allow user to select xml file for e-bill generation. Default folder for e-billing type must exist or billing will fail.
		///True by default.</summary>
		public bool AllowXmlFileSelection=true;
		///<summary>Used for aging security logs. This Source will print to SecurityLog.LogText.</summary>
		public string Source="Undefined";
		#endregion
		#region Outputs
		///<summary>Any error that causes a bill to not be properly generated for a given patient is added to this list.
		///This had previously been stored as a list of dictionaries, making it the most hideous use of dictionary ever recorded in Open Dental lore.
		///Nothing prevents a given patient from having multiple entries in this list.
		///The only duplicated prevented in this list is for PatNum 0 for SkipReason Misc. These are system errors and will all be appended to a single entry.</summary>
		public List<SendStatementsSkipped> ListSkippedPatients=new List<SendStatementsSkipped>();
		///<summary>Running tally of statements that were successfully emailed. Statement.Mode must be Email for this count to increment.
		///Failed email results in no increment and and entry in ListSkippedPatients.</summary>
		public int CountStatementsEmailed;
		///<summary>Running tally of statements that were successfully added to the PdfMasterDocument. 
		///Statement.Mode must be InPerson or Mail for this count to increment.</summary>
		public int CountStatementsPrinted;
		///<summary>Running tally of statements that were successfully sent electroincally. 
		///Statement.Mode must be Electronic for this count to increment.
		///There are 4 electronic billing providers, which provider is used by practice is stored in PrefName.BillingUseElectronic.
		///Statements are sent in batches to electronic billing providers so if the operation fails, then all statements are failed and CountStatementsSentElectronic will be decremented accordingly.</summary>
		public int CountStatementsSentElectronic;
		///<summary>Pdf that is the sum off all statements' pages for this billing run which had a statement mode of InPerson or Mail.
		///Will be null if no statements required print. Check for null before using!
		///Each statement's document will be opened and each page of that document will be added to the PdfMasterDocument.
		///This is used to show a single printable pdf on screen when the billing run has completed. All statements needing print can then be printed with one user print command.</summary>
		public PdfDocument PdfMasterDocument=null;
		///<summary>Running tally of statements that were not able to be sent due to statement no longer existing.
		///This usually means that statement was deleted by another user will billing was running.
		///These statements will be counted as processed by no bill will be produced.</summary>
		public int CountStatementsSkippedForDeletion;
		///<summary>Running tally of statements that had a text message generated and succesfully sent to a patient.
		///In order to receive a text, clinic must be signed up for texting, the patient must meet the criteria of PatComm.IsSmsAnOption, and patient must not be opted out.
		///This tally only increments if all those criteria are met and then a text is sent to the patient and that text does not fail to send for any other reason.</summary>
		public int CountStatmentsSentPayPortalText;
		///<summary>Current index of overall progress of all statement. Used for progress updates only.</summary>
		public int CurStatementIdx=0;
		///<summary>Statements will be processed in batches. 
		///If clinics on then each batch will be clinic num of statements' patient's guarantor. 
		///If clinics off then batch will be set arbitrarily and limited to size determined by PrefName.BillingElectBatchMax.</summary>
		public List<StatementBatch> ListStatementBatches=new List<StatementBatch>();
		///<summary>Managed by BillingL.SendStatements as it iterates through ListStatementBatches.</summary>
		public StatementBatch CurStatementBatch=new StatementBatch();
		///<summary>Statements actually sent. Used when Billing is paused and resumed to determine where we left off.</summary>
		public List<long> ListStatementNumsSent=new List<long>();
		///<summary>If progress is paused and then resumed, this list will be filled with statement nums that may have been deleted in the meantime and should now be skipped.</summary>
		public List<long> ListStatementNumsToSkipAfterPause=new List<long>();
		///<summary>The families that are selected when the user hits "Send". The key is the PatNum and the value is its Family.
		///Dictionary has deep roots in the supporting code for Billing so this anti-pattern (using dictionary) must persist in this case.</summary>
		public Dictionary<long,Family> DictionaryFamilies=new Dictionary<long, Family>();
		///<summary>List of all temp files that were created for a given Billing run. These will be passed back to FormBilling once billing has completed so they can be safely deleted.</summary>
		public List<string> ListTempPdfFiles=new List<string>();
		///<summary>User may be prompted for an eBilling xml file path. If they elect to cancel that prompt, hold onto their choice so we can auto cancel for all batches.</summary>
		public bool DidUserCancelXmlFileSelection=false;
		///<summary>Holds the path that either came from an individual eBilling vendor pref or (if that path does not exist) user is prompted for a path and a manual path will be set.
		///Retain this path so we only have to prompt the user once on first batch iteration.</summary>
		public string XmlFilePath;
		#endregion
		#region Callbacks
		///<summary>User has the option to run billing for all time (no StartDate specified). 
		///If this option is selected and at least one statement is billed electronically, user will be prompted that this may take an extremely long time to complete. 
		///User will then be given option to cancel.</summary>
		public Func<bool> FuncGetIsHistoryStartMinDate;
		///<summary>Fired when any progress event occurs that requires UI update.</summary>
		public Action<ODEventArgs> ActionProgressEvent;
		///<summary>Passes a question from billing logic back to UI (or unit test). Expects yes/no (true/false) answer.</summary>
		public Func<string,bool> FuncAskQuestion;
		///<summary>Passes a prompt from billing logic back to UI (or unit test). Bool is used to indicate MsgCopyPaste dialog should be used in place of MsgBox.</summary>
		public Action<string,bool> ActionPrompt;
		///<summary>Passes an initial directory that should be shown in a directory/file chooser dialog. Returns true if OK and path exists. Returns false if Cancel. Also returns file name selected by user (not including full path).</summary>
		public Func<string,ChooseSaveFile> FuncChooseSaveFile;
		///<summary>Asks UI if user has paused billing.</summary>
		public Func<bool> FunctGetIsPaused;
		///<summary>Asks UI if user has cancelled billing.</summary>
		public Func<bool> FuncGetIsCancelled;
		///<summary>Gives UI a chance to sleep while billing is paused. Unit test can use this as a callback to perform operations during pause.</summary>
		public Action ActionSleepDuringPause;
		///<summary>Typically a reflection of Statements.GetBilling. Use that method to mock columns/rows if desired in unit test.</summary>
		public Func<DataTable> FuncGetBillingDataTable;
		///<summary>Input: Patient's ClinicNum, Output: Sender Email. Will typically use the email belonging to the clinic of the patient attached to the statement.</summary>
		public Func<long,EmailAddress> FuncGetSenderEmailAddress;
		///<summary>Input: Patient's ClinicNum, EmailMessage to send, Send EmailAddress, Use Secure Email. Invoker must send the email via given method, and insert email into db.</summary>
		public Action<long,EmailMessage,EmailAddress,bool> ActionSendEmail;
		///<summary>Input: tempPdfFile, filePath. Output: savedPdfPath.</summary>
		public Func<string,string,string> FuncGetPatientPdfPath;
		///<summary>Input: RawBase64 (only used when AtoZ is in database), savedPdfPath (was set by output of FuncGetPatientPdfPath). Output: pdf document opened using given inputs.
		///Invoker must evaulate the input args and open the PdfDocument. Leave PdfDocument open so Billing can read its pages.</summary>
		public Func<string,string,PdfDocument> FuncGetPdfDocument;
		///<summary>Input: savedPdfPath (was set by output of FuncGetPatientPdfPath). Statement document, Patient belonging to statement. Output: email attachment.
		///Invoker must evaulate the input args and create the EmailAttach to be attached to the satement email.</summary>
		public Func<string,Document,Patient,EmailAttach> FuncGetEmailAttachment;
		///<summary>Input: Temp pdf file name to be deleted.</summary>
		public Action<string> ActionDeleteTempPdfFile;
		///<summary>Input: The inputs of Bridges.EHG_statements.Send(). Output: Any alert message returned by Bridges.EHG_statements.Send().
		///This callback exists so unit tests can mock the DentalXChange api call.</summary>
		public Func<string,long,string> FuncSendEhgStatement;
		///<summary>Input: Today's date, taken from MySql host. Output: true if Ledgers.ComputeAging succeeded. If return false, billing is cancelled.</summary>
		public Func<DateTime,bool> FuncComputeAging;
		#endregion
		#region Helper Methods
		///<summary>Tries to set XmlFilePath to be used on future batch iterations. If xmlFilePathBaseFromPref is no good then user will be prompted for local directory.
		///If user cancels or has cancelled on previous iteration then returns false and you can assume no xml file should be generated for this batch.
		///If XmlFilePath has been properly set, then returns true and you can continue with the batch.</summary>
		public bool SetXmlFilePath(string xmlFilePathBaseFromPref,string initialSaveDirectory) {
			if(DidUserCancelXmlFileSelection) {
				//User elected to cancel when prompted for a file path on previous iteration.
				//Dont make them cancel again, just cancel for all batches and move on to next batch.
				return false;
			}
			if(!string.IsNullOrEmpty(XmlFilePath)) {
				//Path was already set on a previous iteration.
				return true;
			}
			//Path not set yet, let's try.
			if(!string.IsNullOrEmpty(xmlFilePathBaseFromPref) && Directory.Exists(xmlFilePathBaseFromPref)) {
				//Pref is a valid path, set it.
				XmlFilePath=ODFileUtils.CombinePaths(xmlFilePathBaseFromPref,"Statements.xml");
				return true;
			}
			if(!AllowXmlFileSelection) {
				//ODService has no UI so xmlFilePathBaseFromPref directory must exist or we will have to return here and allow e-billing to fail.
				return false;
			}
			//Try to create initial save directory if one was provided.
			if(!string.IsNullOrEmpty(initialSaveDirectory)) {
				ODException.SwallowAnyException(() => { Directory.CreateDirectory(initialSaveDirectory); });
			}
			//Directory did not exist, force user to choose a valid path.
			ChooseSaveFile chooseSaveFile=FuncChooseSaveFile(initialSaveDirectory);
			if(!chooseSaveFile.IsSelectionOk) {
				//To remember that the user canceled the first time through.  User only needs to cancel once to cancel all batches.
				DidUserCancelXmlFileSelection=true;
				//Go on to next batch
				return false;
			}			
			//User chose a valid path, set it.
			XmlFilePath=chooseSaveFile.FileName;
			return true;
		}

		///<summary>Adds a patient and reason to ListSkippedPatients when a patient statement cannot be processed.</summary>
		public void AddSkippedPatient(long patNum,SkipReason skipReason,string error) {
			SendStatementsSkipped sendStatementsSkipped = new SendStatementsSkipped();
			sendStatementsSkipped.PatNum=patNum;
			sendStatementsSkipped.Reason=skipReason;
			sendStatementsSkipped.Error=error;
			ListSkippedPatients.Add(sendStatementsSkipped);
		}

		///<summary>There can be only 1 single misc system error. Usually because of a catastrophic failure like an outer loop exception. Always linked to PatNum 0.
		///This method will append to that error or create it where necessary.</summary>
		public void AppendMiscSystemError(string error) {
			SendStatementsSkipped sendStatementsSkipped=ListSkippedPatients.Find(x => x.PatNum==0 && x.Reason==SkipReason.Misc);
			if(sendStatementsSkipped==null) {
				//Insert new item.
				sendStatementsSkipped=new SendStatementsSkipped();
				sendStatementsSkipped.PatNum=0;
				sendStatementsSkipped.Reason=SkipReason.Misc;
				sendStatementsSkipped.Error=error;
				ListSkippedPatients.Add(sendStatementsSkipped);
			}
			else if(!sendStatementsSkipped.Error.Contains(error)) {
				//Update existing item.
				sendStatementsSkipped.Error+="; "+error;
			}
		}

		///<summary>Update overall progress of all statement processing. Also fires current batch progress.</summary>
		public void FireOverallProgress() {
			ActionProgressEvent?.Invoke(new ODEventArgs(ODEventType.Billing,
				new ProgressBarHelper(
					labelValue: Lans.g("FormBilling","Overall"),
					percentValue: Math.Ceiling(((double)CurStatementIdx/ListStatementNumsToSend.Count)*100)+"%",
					blockValue: CurStatementIdx,
					blockMax: ListStatementNumsToSend.Count,
					progressStyle: ProgBarStyle.Blocks,
					tagString: "1")));
			ActionProgressEvent?.Invoke(new ODEventArgs(ODEventType.Billing,
				new ProgressBarHelper(
					labelValue: Lans.g(this,"Batch")+"\r\n"+CurStatementBatch.BatchNum+" / "+ListStatementBatches.Count,
					percentValue: Math.Ceiling(((double)CurStatementBatch.CountStatementsProcessed/CurStatementBatch.ListStatements.Count)*100)+"%",
					blockValue: CurStatementBatch.CountStatementsProcessed,
					blockMax: CurStatementBatch.ListStatements.Count,
					progressStyle: ProgBarStyle.Blocks,
					tagString: "2")));
		}

		///<summary>Just a rough estimate of percentage complete the current statment happens to be. 
		///Blame Allen Job 819 : Progress bar for statements, 9/1/2016.</summary>
		public void FireStatementProgress(int fakePercentage) {
			ActionProgressEvent?.Invoke(new ODEventArgs(ODEventType.Billing,
			new ProgressBarHelper(
					labelValue: Lans.g("FormBilling","Statement")+"\r\n"+CurStatementIdx+" / "+ListStatementNumsToSend.Count,
					percentValue: Math.Ceiling(((double)fakePercentage/100)*100)+"%",
					blockValue: fakePercentage,
					blockMax: 100,
					progressStyle: ProgBarStyle.Blocks,
					tagString: "3")));
		}

		///<summary>Statement progress gets hijacked to indicate pdf page creation.</summary>
		public void FirePdfProgress(int pageIndex,int totalPageCount) {
			//Start with 15% base and add percentage of pages complete.
			int percentComplete=((pageIndex/totalPageCount)*85)+15;
			ActionProgressEvent?.Invoke(new ODEventArgs(ODEventType.Billing,
				new ProgressBarHelper(
					labelValue: Lans.g("FormBilling","Statement")+"\r\n"+CurStatementIdx+" / "+ListStatementNumsToSend.Count,
					percentValue: percentComplete+"%",
					blockValue: percentComplete,
					blockMax: 100,
					progressStyle: ProgBarStyle.Blocks,
					tagString: "3")));
		}

		///<summary>Update progress bar text only. Does not update progress blocks.</summary>
		public void FireTextMsgProgress(string labelValue,bool isWarningOffEvent=false) {
			ActionProgressEvent?.Invoke(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(labelValue,progressBarEventType: isWarningOffEvent ? ProgBarEventType.WarningOff : ProgBarEventType.TextMsg)));
		}

		///<summary>Helper method to log message to logger file for Statement action type.</summary>
		public void LogWrite(string logMsg,LogLevel logLevel) {
			 LogWriter?.WriteLine(logMsg,logLevel,"Statements");
		}

		#endregion
	}

	///<summary>Used for billing to group statements by guarantor ClinicNum. BatchNum is 1-based and insignificant.</summary>
	public class StatementBatch {
		///<summary>1-based batch num for displaying in UI. Typically assigned by Statements.GetBatchesForStatements.</summary>
		public int BatchNum;
		///<summary>When clincs turned off, always 0. When clinics turned on, the clinicnum of the guarantor of the statements' patients.</summary>
		public long ClinicNum;
		///<summary>All statements for this batch. Comes from a variety of patient, but all patients' guarantor will share same ClinicNum.</summary>
		public List<Statement> ListStatements=new List<Statement>();
		///<summary>Tracks statements that have been processed in this batch and events progress back to UI. Processed does not mean success, just means dealt with.</summary>
		public int CountStatementsProcessed=0;
		///<summary>Statement data that is retained throughout each batch. Will be used to sync statement data back to StatementProd table once batch has completed.</summary>
		public List<StatementData> ListStatementDatas=new List<StatementData>();
		///<summary>Statements with StatementMode.Electronic are added to this list as statements are added to print list. 
		///These electronic statements will then be sent electronically via the proper eBilling vendor.</summary>
		public List<EbillStatement> ListEbillStatements=new List<EbillStatement>();
	}

	///<summary>Used in place of tuple transport save file selection back from UI to BillingL.</summary>
	public class ChooseSaveFile {
		public string FileName;
		public bool IsSelectionOk;
	}

	///<summary>Helper class that sets all of the default billing preferences for a specific clinic.</summary>
	public class ClinicBillingOptions {
		public long ClinicNum;
		public List<StatementPref> ListStatementPrefs;

		///<summary>Sets the all of the billing preferences for the clinic passed in. The preferences used for this method are the same ones used in OpenDental.FormBillingOptions.cs. Changes to the preference list will also need to be modified in OpenDental.FormBillingOptions.cs.</summary>
		public ClinicBillingOptions(long clinicNum) {
			ClinicNum=clinicNum;
			List<StatementPref> listStatementPrefs=new List<StatementPref>();
			List<PrefName> listBillingPrefs=new List<PrefName>() {
					PrefName.BillingIncludeChanged,
					PrefName.BillingSelectBillingTypes,
					PrefName.BillingAgeOfAccount,
					PrefName.BillingExcludeBadAddresses,
					PrefName.BillingExcludeInactive,
					PrefName.BillingExcludeNegative,
					PrefName.BillingExcludeInsPending,
					PrefName.BillingExcludeIfUnsentProcs,
					PrefName.BillingExcludeLessThan,
					PrefName.BillingIgnoreInPerson,
					PrefName.BillingShowTransSinceBalZero,
					PrefName.BillingDefaultsNote
				};
			if(clinicNum<=0) {//Use Default settings
				foreach(PrefName pref in listBillingPrefs) {
					listStatementPrefs.Add(new StatementPref(pref,PrefC.GetString(pref)));
				}
			}
			else {//Use ClinicPref Values
				List<ClinicPref> listClinicPrefsCur=ClinicPrefs.GetWhere(x => x.ClinicNum==clinicNum && listBillingPrefs.Contains(x.PrefName));
				foreach(ClinicPref clinicPref in listClinicPrefsCur) {
					listStatementPrefs.Add(new StatementPref(clinicPref.PrefName,clinicPref.ValueString));
				}
				List<PrefName> listExistingClinicPrefs=listClinicPrefsCur.Select(x => x.PrefName).ToList();
				List<PrefName> listMissingClincPrefs=listBillingPrefs.FindAll(x => !listExistingClinicPrefs.Contains(x));
				foreach(PrefName prefNameMissing in listMissingClincPrefs) {
					//Set missing pref equal to the default HQ value.
					listStatementPrefs.Add(new StatementPref(prefNameMissing,PrefC.GetString(prefNameMissing)));
				}
			}
			ListStatementPrefs=listStatementPrefs;
		}

		public string GetPrefValue(PrefName prefName) {
			return ListStatementPrefs.FirstOrDefault(x => x.PrefName==prefName)?.ValueString??"";
		}
	}

	public class StatementPref {
		public PrefName PrefName;
		public string ValueString;

		public StatementPref(PrefName prefName,string valueString) {
			PrefName=prefName;
			ValueString=valueString;
		}
	}
}
