using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Xml;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrSummaryOfCare:FormODBase {
		public Patient PatCur;
		private List<EhrMeasureEvent> _listHistorySent;
		private List<EhrSummaryCcd> _listCcdRec;

		public FormEhrSummaryOfCare() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormSummaryOfCare_Load(object sender,EventArgs e) {
			FillGridSent();
			FillGridRec();
		}

		private void FillGridSent() {
			List<RefAttach> listRefAttaches;
			gridSent.BeginUpdate();
			gridSent.ListGridColumns.Clear();
			GridColumn col=new GridColumn("DateTime",130,HorizontalAlignment.Center);
			gridSent.ListGridColumns.Add(col);
			col=new GridColumn("Meets",140,HorizontalAlignment.Center);
			gridSent.ListGridColumns.Add(col);
			_listHistorySent=EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.SummaryOfCareProvidedToDr);
			listRefAttaches=RefAttaches.GetRefAttachesForSummaryOfCareForPat(PatCur.PatNum);
			gridSent.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listHistorySent.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listHistorySent[i].DateTEvent.ToString());
				if(_listHistorySent[i].FKey==0) {
					row.Cells.Add("");
				}
				else {
					//Only add an X in the grid for the measure events that meet the summary of care measure so that users can see which ones meet.
					for(int j=0;j<listRefAttaches.Count;j++) {
						if(listRefAttaches[j].RefAttachNum==_listHistorySent[i].FKey) {
							row.Cells.Add("X");
							break;
						}
					}
				}
				gridSent.ListGridRows.Add(row);
			}
			gridSent.EndUpdate();
		}

		private void gridSent_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReferralsPatient FormRP=new FormReferralsPatient();
			FormRP.DefaultRefAttachNum=_listHistorySent[gridSent.GetSelectedIndex()].FKey;
			FormRP.PatNum=PatCur.PatNum;
			FormRP.IsSelectionMode=true;
			if(FormRP.ShowDialog()==DialogResult.Cancel) {
				return;
			}
			_listHistorySent[gridSent.GetSelectedIndex()].FKey=FormRP.RefAttachNum;
			EhrMeasureEvents.Update(_listHistorySent[gridSent.GetSelectedIndex()]);
			FillGridSent();
		}

		private void FillGridRec() {
			gridRec.BeginUpdate();
			gridRec.ListGridColumns.Clear();
			GridColumn col=new GridColumn("DateTime",140,HorizontalAlignment.Center);
			gridRec.ListGridColumns.Add(col);
			_listCcdRec=EhrSummaryCcds.Refresh(PatCur.PatNum);
			gridRec.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCcdRec.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listCcdRec[i].DateSummary.ToShortDateString());
				gridRec.ListGridRows.Add(row);
			}
			gridRec.EndUpdate();
		}

		private void gridRec_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			string xmltext=_listCcdRec[gridRec.GetSelectedIndex()].ContentSummary;
			DisplayCCD(xmltext,PatCur);
		}

		///<summary>Returns true if user performed a print job on the CCD.  Cannot be moved to OpenDentBusiness/Misc/EhrCCD.cs, because this function uses windows UI components.  
		///Strictly used for displaying CCD messages.  Will not allow user to reconcile meds, problems, or allergies into the patient's account.</summary>
		public static bool DisplayCCD(string strXmlCCD) {
			return DisplayCCD(strXmlCCD,null);
		}

		public static bool DisplayCCD(string strXmlCCD,Patient patCur) {
			return DisplayCCD(strXmlCCD,patCur,"");
		}

		///<summary>Returns true if user performed a print job on the CCD.  Cannot be moved to OpenDentBusiness/Misc/EhrCCD.cs, because this function uses windows UI components. 
		///Pass in a valid patient if the CCD is being displayed to reconcile (incoporate into patient record) medications and/or problems and/or allergies.
		///patCur can be null, or patCur.PatNum can be 0, to hide the three reconcile buttons. 
		///strXmlCCD is the actual text of the CCD. 
		///strAlterateFilPathXslCCD is a full file path to an alternative style sheet. 
		///This file will only be used in the case where the EHR dll version of the stylesheet couldn not be loaded. 
		///If neither method works for attaining the stylesheet then an excption will be thrown.</summary>
		public static bool DisplayCCD(string strXmlCCD,Patient patCur,string strAlterateFilPathXslCCD) {
			//string xmltext=GetSampleMissingStylesheet();
			XmlDocument doc=new XmlDocument();
			try {
				doc.LoadXml(strXmlCCD);
			}
			catch {
				throw new XmlException("Invalid XML");
			}
			string xmlFileName="";
			string xslFileName="";
			string xslContents="";
			if(doc.DocumentElement.Name.ToLower()=="clinicaldocument") {//CCD, CCDA, and C32.
				xmlFileName="ccd.xml";
				xslFileName="ccd.xsl";
				xslContents=FormEHR.GetEhrResource("CCD");
				if(xslContents=="") { //XSL load from EHR dll failed so see if caller provided an alternative
					if(strAlterateFilPathXslCCD!="") { //alternative XSL file was provided so use that for our stylesheet
						xslContents=FileAtoZ.ReadAllText(strAlterateFilPathXslCCD);
					}
				}
				if(xslContents=="") { //one last check to see if we succeeded in finding a stylesheet
					throw new Exception("No stylesheet found");
				}
			}
			else if(doc.DocumentElement.Name.ToLower()=="continuityofcarerecord" || doc.DocumentElement.Name.ToLower()=="ccr:continuityofcarerecord") {//CCR
				xmlFileName="ccr.xml";
				xslFileName="ccr.xsl";
				xslContents=FormEHR.GetEhrResource("CCR");
			}
			else {
				MessageBox.Show("This is not a valid CCD, CCDA, CCR, or C32 message.  Only the raw text will be shown");
				MessageBox.Show(strXmlCCD);
				return false;
			}
			XmlNode node=doc.SelectSingleNode("/processing-instruction(\"xml-stylesheet\")");
			if(node==null) {//document does not contain any stylesheet instruction, so add one
				XmlProcessingInstruction pi=doc.CreateProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\""+xslFileName+"\"");
				doc.InsertAfter(pi,doc.ChildNodes[0]);
			}
			else {//alter the existing instruction
				XmlProcessingInstruction pi=(XmlProcessingInstruction)node;
				pi.Value="type=\"text/xsl\" href=\""+xslFileName+"\"";
			}
			File.WriteAllText(Path.Combine(PrefC.GetTempFolderPath(),xmlFileName),doc.InnerXml.ToString());
			File.WriteAllText(Path.Combine(PrefC.GetTempFolderPath(),xslFileName),xslContents);
			using FormEhrSummaryCcdEdit formESCD=new FormEhrSummaryCcdEdit(ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),xmlFileName),patCur);
			formESCD.ShowDialog();
			string[] arrayFileNames={"ccd.xml","ccd.xsl","ccr.xml","ccr.xsl"};
			for(int i=0;i<arrayFileNames.Length;i++) {
				try {
					File.Delete(ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),arrayFileNames[i]));
				}
				catch {
					//Do nothing because the file could have been in use or there were not sufficient permissions.
					//This file will most likely get deleted next time a file is created.
				}
			}
			return formESCD.DidPrint;
		}

		private void butExport_Click(object sender,EventArgs e) {
			//Generate the CCD first so that any validation errors are apparent and up front.
			//It is better to not let the user waste their time creating a referral if there is a basic validation issue with the CCD generation.
			string ccd="";
			try {
				ccd=EhrCCD.GenerateSummaryOfCare(PatCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using FormReferralsPatient FormRP=new FormReferralsPatient();
			FormRP.PatNum=PatCur.PatNum;
			FormRP.IsSelectionMode=true;
			if(FormRP.ShowDialog()==DialogResult.Cancel) {
				MessageBox.Show("Summary of Care not exported.");
				return;
			}
			using FolderBrowserDialog dlg=new FolderBrowserDialog();
			dlg.SelectedPath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());//Default to patient image folder.
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK) {
				return;
			}
			if(File.Exists(Path.Combine(dlg.SelectedPath,"ccd.xml"))) {
				if(MessageBox.Show("Overwrite existing ccd.xml?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xml"),ccd);
			File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xsl"),FormEHR.GetEhrResource("CCD"));
			EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			newMeasureEvent.DateTEvent = DateTime.Now;
			newMeasureEvent.EventType = EhrMeasureEventType.SummaryOfCareProvidedToDr;
			newMeasureEvent.PatNum = PatCur.PatNum;
			newMeasureEvent.FKey=FormRP.RefAttachNum;//Can be 0 if user didn't pick a referral for some reason.
			long fkey=EhrMeasureEvents.Insert(newMeasureEvent);
			newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.FKey=fkey;
			newMeasureEvent.EventType=EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic;
			newMeasureEvent.PatNum=PatCur.PatNum;
			newMeasureEvent.FKey=FormRP.RefAttachNum;//Can be 0 if user didn't pick a referral for some reason.
			EhrMeasureEvents.Insert(newMeasureEvent);
			FillGridSent();
			MessageBox.Show("Exported");
		}

		private void butSendEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			//Generate the CCD first so that any validation errors are apparent and up front.
			//It is better to not let the user waste their time creating a referral if there is a basic validation issue with the CCD generation.
			string ccd="";
			try {
				ccd=EhrCCD.GenerateSummaryOfCare(PatCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using FormReferralsPatient FormRP=new FormReferralsPatient();
			FormRP.PatNum=PatCur.PatNum;
			FormRP.IsSelectionMode=true;
			if(FormRP.ShowDialog()==DialogResult.Cancel) {
				MessageBox.Show("Summary of Care not exported.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			EmailAddress emailAddressFrom=EmailAddresses.GetByClinic(0);//Default for clinic/practice.
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.PatNum=PatCur.PatNum;
			emailMessage.MsgDateTime=DateTime.Now;
			emailMessage.SentOrReceived=EmailSentOrReceived.Neither;//To force FormEmailMessageEdit into "compose" mode.
			emailMessage.FromAddress=emailAddressFrom.EmailUsername;//Cannot be emailAddressFrom.SenderAddress, because it would cause encryption to fail.
			emailMessage.ToAddress="";//User must set inside of FormEmailMessageEdit
			emailMessage.Subject="Summary of Care";
			emailMessage.BodyText="Summary of Care";
			emailMessage.MsgType=EmailMessageSource.EHR;
			try {
				emailMessage.Attachments.Add(EmailAttaches.CreateAttach("ccd.xml",Encoding.UTF8.GetBytes(ccd)));
				emailMessage.Attachments.Add(EmailAttaches.CreateAttach("ccd.xsl",Encoding.UTF8.GetBytes(FormEHR.GetEhrResource("CCD"))));
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			EmailMessages.Insert(emailMessage);
			using FormEmailMessageEdit formE=new FormEmailMessageEdit(emailMessage,emailAddressFrom);//Not "new" message because it already exists in db due to pre-insert.
			if(formE.ShowDialog()==DialogResult.OK) {
				EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
				newMeasureEvent.DateTEvent=DateTime.Now;
				newMeasureEvent.EventType=EhrMeasureEventType.SummaryOfCareProvidedToDr;
				newMeasureEvent.PatNum=PatCur.PatNum;
				newMeasureEvent.FKey=FormRP.RefAttachNum;//Can be 0 if user didn't pick a referral for some reason.
				EhrMeasureEvents.Insert(newMeasureEvent);
				newMeasureEvent=new EhrMeasureEvent();
				newMeasureEvent.DateTEvent=DateTime.Now;
				newMeasureEvent.EventType=EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic;
				newMeasureEvent.PatNum=PatCur.PatNum;
				newMeasureEvent.FKey=FormRP.RefAttachNum;//Can be 0 if user didn't pick a referral for some reason.
				EhrMeasureEvents.Insert(newMeasureEvent);
				FillGridSent();
			}
			Cursor=Cursors.Default;
		}

		private void butShowXhtml_Click(object sender,EventArgs e) {
			using FormReferralsPatient FormRP=new FormReferralsPatient();
			FormRP.PatNum=PatCur.PatNum;
			FormRP.IsSelectionMode=true;
			if(FormRP.ShowDialog()==DialogResult.Cancel) {
				MessageBox.Show("Summary of Care not shown.");
				return;
			}
			string ccd="";
			try {
				ccd=EhrCCD.GenerateSummaryOfCare(PatCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			bool didPrint=DisplayCCD(ccd);
			if(didPrint) {
				//we are printing a ccd so add new measure event.					
				EhrMeasureEvent measureEvent = new EhrMeasureEvent();
				measureEvent.DateTEvent = DateTime.Now;
				measureEvent.EventType = EhrMeasureEventType.SummaryOfCareProvidedToDr;
				measureEvent.FKey=FormRP.RefAttachNum;
				measureEvent.PatNum = PatCur.PatNum;
				EhrMeasureEvents.Insert(measureEvent);
				FillGridSent();
			}		
		}

		private void butShowXml_Click(object sender,EventArgs e) {
			string ccd="";
			try {
				ccd=EhrCCD.GenerateSummaryOfCare(PatCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(ccd);
			msgbox.ShowDialog();
		}

		private void butRecEmail_Click(object sender,EventArgs e) {
			using FormEmailInbox form=new FormEmailInbox();
			form.ShowDialog();
		}

		private void butRecFile_Click(object sender,EventArgs e) {
			using OpenFileDialog dlg=new OpenFileDialog();
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK){
				return;
			}
			string text=File.ReadAllText(dlg.FileName);
			EhrSummaryCcd ehrSummaryCcd=new EhrSummaryCcd();
			ehrSummaryCcd.ContentSummary=text;
			ehrSummaryCcd.DateSummary=DateTime.Today;
			ehrSummaryCcd.PatNum=PatCur.PatNum;
			EhrSummaryCcds.Insert(ehrSummaryCcd);
			FillGridRec();
			DisplayCCD(text,PatCur);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridSent.SelectedIndices.Length < 1) {
				MessageBox.Show("Please select at least one record to delete.");
				return;
			}
			for(int i=0;i<gridSent.SelectedIndices.Length;i++) {
				EhrMeasureEvents.Delete(_listHistorySent[gridSent.SelectedIndices[i]].EhrMeasureEventNum);
			}
			FillGridSent();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		

		
	

		

	

	


	}
}




#region OldCode
/*
		public static StringBuilder GenerateCCD(Patient pat) {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="   ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strBuilder,xmlSettings)){
				//Begin Clinical Document
				writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				writer.WriteStartElement("ClinicalDocument","urn:hl7-org:v3");
					// Asserting use of the HL7 Guide for CDA R2 - CCD
					writer.WriteStartElement("typeId");
						writer.WriteAttributeString("extension","POCD_HD0000040");
						writer.WriteAttributeString("root","2.16.840.1.113883.1.3");
				  writer.WriteEndElement();
				//Michael:Do I need this here?
					//writer.WriteStartElement("templateId");
					//  writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1");
					//writer.WriteEndElement();
					// Header---------------------------------------------------------------------------------------------
					// 2.1 CCR Unique Identifier
				  writer.WriteStartElement("id");
				  writer.WriteEndElement();
					writer.WriteStartElement("code");
						writer.WriteAttributeString("code","34133-9");
						writer.WriteAttributeString("codeSystemName","LOINC");
						writer.WriteAttributeString("codeSystem","2.16.840.1.113883.6.1");
						writer.WriteAttributeString("displayName","Summary of episode note");
				  writer.WriteEndElement();
					writer.WriteStartElement("documentationOf");
						writer.WriteStartElement("serviceEvent");
							writer.WriteAttributeString("classCode","PCPR");
							string effectiveTimeHigh=DateTime.Now.ToString("yyyyMMddHHmmsszzz").Replace(":","");
							string effectiveTimeLow=pat.Birthdate.ToString("yyyyMMddHHmmsszzz").Replace(":","");
							writer.WriteStartElement("effectiveTime");
								writer.WriteStartElement("high");
									writer.WriteAttributeString("value",effectiveTimeHigh);
								writer.WriteEndElement();//End high
								writer.WriteStartElement("low");
									writer.WriteAttributeString("value",effectiveTimeLow);
								writer.WriteEndElement();//End low
							writer.WriteEndElement();//End effectiveTime
						writer.WriteEndElement();
					writer.WriteEndElement();//End documentationOf
					// 2.2 Language
					writer.WriteStartElement("languageCode");
						writer.WriteAttributeString("value","en-US");
					writer.WriteEndElement();
					// 2.3 Version
				  writer.WriteStartElement("templateId");
				    writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1");
				  writer.WriteEndElement();
					// 2.4 CCR Creation Date/Time
					writer.WriteStartElement("effectiveTime");
						string ccrCreationDateTime=DateTime.Now.ToString("yyyyMMddHHmmsszzz").Replace(":","");
						writer.WriteAttributeString("value",ccrCreationDateTime);
					writer.WriteEndElement();
					// 2.5 Patient
					writer.WriteStartElement("recordTarget");
						writer.WriteStartElement("patientRole");
							writer.WriteStartElement("id");
								writer.WriteAttributeString("value",pat.PatNum.ToString());
							writer.WriteEndElement();
							writer.WriteStartElement("addr");
								writer.WriteAttributeString("use","HP");
								writer.WriteStartElement("streetAddressLine");
									writer.WriteString(pat.Address.ToString());
								writer.WriteEndElement();
								writer.WriteStartElement("streetAddressLine");
									writer.WriteString(pat.Address2.ToString());
								writer.WriteEndElement();
								writer.WriteStartElement("city");
									writer.WriteString(pat.City.ToString());
								writer.WriteEndElement();
								writer.WriteStartElement("state");
									writer.WriteString(pat.State.ToString());
								writer.WriteEndElement();
								writer.WriteStartElement("country");
									writer.WriteString("");
								writer.WriteEndElement();
							writer.WriteEndElement();//End addr
							writer.WriteStartElement("patient");
								writer.WriteStartElement("name");
									writer.WriteAttributeString("use","L");
									writer.WriteStartElement("given");
										writer.WriteString(pat.FName.ToString());
									writer.WriteEndElement();
									writer.WriteStartElement("given");
										writer.WriteString(pat.MiddleI.ToString());
									writer.WriteEndElement();
									writer.WriteStartElement("family");
										writer.WriteString(pat.LName.ToString());
									writer.WriteEndElement();
									writer.WriteStartElement("suffix");
										writer.WriteAttributeString("qualifier","TITLE");
										writer.WriteString(pat.Title.ToString());
									writer.WriteEndElement();//End suffix
								writer.WriteEndElement();//End name
							writer.WriteEndElement();//End patient
						writer.WriteEndElement();//End patientRole
						writer.WriteStartElement("text");//The following text will be parsed as html with a style sheet to be human readable.
							writer.WriteStartElement("table");
								writer.WriteAttributeString("width","100%");
								writer.WriteAttributeString("border","1");
								writer.WriteStartElement("thead");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("th");
										  writer.WriteString("Name");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Date of Birth");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Gender");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Identification Number");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Identification Number Type");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Address/Phone");
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End thead
								writer.WriteStartElement("tbody");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("td");//Name
											writer.WriteString(pat.LName+", "+pat.FName+" "+pat.MiddleI);
										writer.WriteEndElement();
										writer.WriteStartElement("td");//DoB
											writer.WriteString(pat.Birthdate.ToShortDateString());
										writer.WriteEndElement();
										writer.WriteStartElement("td");//Gender
											writer.WriteString(pat.Gender.ToString());
										writer.WriteEndElement();
										writer.WriteStartElement("td");//PatNum
											writer.WriteString(pat.PatNum.ToString());
										writer.WriteEndElement();
										writer.WriteStartElement("td");//"Open Dental PatNum"
											writer.WriteString("Open Dental PatNum");
										writer.WriteEndElement();
										writer.WriteStartElement("td");//Address/Phone
											writer.WriteString(pat.Address+" "+pat.Address2+"\r\n"+pat.City+", "
												+pat.State+"\r\n"+pat.Zip+"\r\n"+pat.HmPhone);
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End tbody
							writer.WriteEndElement();//End table
						writer.WriteEndElement();//End text
					writer.WriteEndElement();//End recordTarget
					// 2.6 From
					writer.WriteStartElement("author");
						writer.WriteStartElement("assignedAuthor");
							writer.WriteStartElement("assignedPerson");
								writer.WriteElementString("name","Auto Generated");
							writer.WriteEndElement();//End assignedPerson
						writer.WriteEndElement();//End assignedAuthor
					writer.WriteEndElement();//End author
					// Body--------------------------------------------------------------------------------------------
					// 3.5 Problems
					List<Disease> listProblem=Diseases.Refresh(pat.PatNum);
					int condID=1;//used to set the CondID-# in the problem list of the html table in the text section of this document.
					ICD9 icd9;
					writer.WriteStartElement("component");
						writer.WriteComment("Problems");
						writer.WriteStartElement("section");
							writer.WriteStartElement("templateId");
								writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1.11");
								writer.WriteAttributeString("assigningAuthorityName","HL7 CCD");
							writer.WriteEndElement();
							writer.WriteComment("Problems section template");
							writer.WriteStartElement("code");
								writer.WriteAttributeString("code","11450-4");
								writer.WriteAttributeString("codeSystemName","LOINC");
								writer.WriteAttributeString("codeSystem","2.16.840.1.113883.6.1");
								writer.WriteAttributeString("displayName","Problem list");
						writer.WriteEndElement();
						writer.WriteStartElement("title");
							writer.WriteString("Problems");
						writer.WriteEndElement();
						writer.WriteStartElement("text");//The following text will be parsed as html with a style sheet to be human readable.
							writer.WriteStartElement("table");
								writer.WriteAttributeString("width","100%");
								writer.WriteAttributeString("border","1");
								writer.WriteStartElement("thead");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("th");
										  writer.WriteString("ICD-9 Code");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Patient Problem");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Date Diagnosed");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Status");
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End thead
								writer.WriteStartElement("tbody");
									for(int i=0;i<listProblem.Count;i++){
										if(listProblem[i].ICD9Num!=0){
											icd9=ICD9s.GetOne(listProblem[i].ICD9Num);
											writer.WriteStartElement("tr");
												writer.WriteAttributeString("ID","CondID-"+condID);
												writer.WriteStartElement("td");
												  writer.WriteString(icd9.ICD9Code.Substring(0,3)+"."+icd9.ICD9Code.Substring(3));
												writer.WriteEndElement();
												writer.WriteStartElement("td");
													writer.WriteString(icd9.Description);
												writer.WriteEndElement();
												writer.WriteStartElement("td");
													writer.WriteString(listProblem[i].DateStart.ToShortDateString());
												writer.WriteEndElement();
												writer.WriteStartElement("td");
													writer.WriteString(listProblem[i].ProbStatus.ToString());
												writer.WriteEndElement();
											writer.WriteEndElement();//End tr
										}
									}
								writer.WriteEndElement();//End tbody
							writer.WriteEndElement();//End table
						writer.WriteEndElement();//End text
					writer.WriteEndElement();//End Problem component
					// 3.8 Alerts (for Allergies)
					List<Allergy> listAllergy=Allergies.Refresh(pat.PatNum);
					AllergyDef allergyDef;
					Medication med;
					writer.WriteStartElement("component");
						writer.WriteComment("Alerts");
						writer.WriteStartElement("section");
							writer.WriteStartElement("templateId");
								writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1.2");
								writer.WriteAttributeString("assigningAuthorityName","HL7 CCD");
							writer.WriteEndElement();
							writer.WriteComment("Alerts section template");
							writer.WriteStartElement("code");
								writer.WriteAttributeString("code","48765-2");
								writer.WriteAttributeString("codeSystemName","LOINC");
								writer.WriteAttributeString("codeSystem","2.16.840.1.113883.6.1");
								writer.WriteAttributeString("displayName","Allergies, adverse reactions, alerts");
						writer.WriteEndElement();
						writer.WriteStartElement("title");
							writer.WriteString("Allergies and Adverse Reactions");
						writer.WriteEndElement();
						writer.WriteStartElement("text");//The following text will be parsed as html with a style sheet to be human readable.
							writer.WriteStartElement("table");
								writer.WriteAttributeString("width","100%");
								writer.WriteAttributeString("border","1");
								writer.WriteStartElement("thead");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("th");
										  writer.WriteString("SNOMED Allergy Type Code");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Medication/Agent Allergy");
										writer.WriteEndElement();//
										writer.WriteStartElement("th");
											writer.WriteString("Reaction");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Adverse Event Date");
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End thead
								writer.WriteStartElement("tbody");
									for(int i=0;i<listAllergy.Count;i++){
										allergyDef=AllergyDefs.GetOne(listAllergy[i].AllergyDefNum);
										writer.WriteStartElement("tr");
											writer.WriteStartElement("td");
											  writer.WriteString(AllergyDefs.GetSnomedAllergyDesc(allergyDef.Snomed));
											writer.WriteEndElement();
											writer.WriteStartElement("td");
												med=Medications.GetMedication(allergyDef.MedicationNum);
											  writer.WriteString(med.RxCui.ToString()+" - "+med.MedName);
											writer.WriteEndElement();
											writer.WriteStartElement("td");
												writer.WriteString(listAllergy[i].Reaction);
											writer.WriteEndElement();
											writer.WriteStartElement("td");
												writer.WriteString(listAllergy[i].DateAdverseReaction.ToShortDateString());
											writer.WriteEndElement();
										writer.WriteEndElement();//End tr
									}
								writer.WriteEndElement();//End tbody
							writer.WriteEndElement();//End table
						writer.WriteEndElement();//End text
					writer.WriteEndElement();//End Alerts component
					// 3.9 Medications
					List<MedicationPat> listMedPat=MedicationPats.Refresh(pat.PatNum,true);
					writer.WriteStartElement("component");
						writer.WriteComment("Medications");
						writer.WriteStartElement("section");
							writer.WriteStartElement("templateId");
								writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1.8");
								writer.WriteAttributeString("assigningAuthorityName","HL7 CCD");
							writer.WriteEndElement();
							writer.WriteComment("Medications section template");
							writer.WriteStartElement("code");
								writer.WriteAttributeString("code","10160-0");
								writer.WriteAttributeString("codeSystemName","LOINC");
								writer.WriteAttributeString("codeSystem","2.16.840.1.113883.6.1");
								writer.WriteAttributeString("displayName","History of medication use");
						writer.WriteEndElement();
						writer.WriteStartElement("title");
							writer.WriteString("Medications");
						writer.WriteEndElement();
						writer.WriteStartElement("text");//The following text will be parsed as html with a style sheet to be human readable.
							writer.WriteStartElement("table");
								writer.WriteAttributeString("width","100%");
								writer.WriteAttributeString("border","1");
								writer.WriteStartElement("thead");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("th");
										  writer.WriteString("RxNorm Code");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
										  writer.WriteString("Product");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
										  writer.WriteString("Generic Name");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Brand Name");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Instructions");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Date Started");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Status");
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End thead
								writer.WriteStartElement("tbody");
									for(int i=0;i<listMedPat.Count;i++){
										med=Medications.GetMedication(listMedPat[i].MedicationNum);
										writer.WriteStartElement("tr");
											writer.WriteStartElement("td");
											  writer.WriteString(med.RxCui.ToString());//RxNorm Code
											writer.WriteEndElement();
											writer.WriteStartElement("td");
											  writer.WriteString("Medication");//Product
											writer.WriteEndElement();
											writer.WriteStartElement("td");
											  writer.WriteString(Medications.GetGenericName(med.GenericNum));//Generic Name
											writer.WriteEndElement();
											writer.WriteStartElement("td");
											  writer.WriteString(med.MedName);//Brand Name
											writer.WriteEndElement();
											writer.WriteStartElement("td");
												writer.WriteString(listMedPat[i].PatNote);//Instruction
											writer.WriteEndElement();
											writer.WriteStartElement("td");
												writer.WriteString(listMedPat[i].DateStart.ToShortDateString());//Date Started
											writer.WriteEndElement();
											writer.WriteStartElement("td");
										writer.WriteString(MedicationPat.IsMedActive(listMedPat[i])?"Active":"Inactive");//Status
											writer.WriteEndElement();
										writer.WriteEndElement();//End tr
									}
								writer.WriteEndElement();//End tbody
							writer.WriteEndElement();//End table
						writer.WriteEndElement();//End text
					writer.WriteEndElement();//End Medication component
					// 3.13 Results
					List<LabResult> listLabResult=LabResults.GetAllForPatient(pat.PatNum);
					writer.WriteStartElement("component");
						writer.WriteComment("Results");
						writer.WriteStartElement("section");
							writer.WriteStartElement("templateId");
								writer.WriteAttributeString("root","2.16.840.1.113883.10.20.1.14");
								writer.WriteAttributeString("assigningAuthorityName","HL7 CCD");
							writer.WriteEndElement();
							writer.WriteComment("Relevant diagnostic tests and/or labratory data");
							writer.WriteStartElement("code");
								writer.WriteAttributeString("code","30954-2");
								writer.WriteAttributeString("codeSystemName","LOINC");
								writer.WriteAttributeString("codeSystem","2.16.840.1.113883.6.1");
								writer.WriteAttributeString("displayName","Allergies, adverse reactions, alerts");
						writer.WriteEndElement();
						writer.WriteStartElement("title");
							writer.WriteString("Results");
						writer.WriteEndElement();
						writer.WriteStartElement("text");//The following text will be parsed as html with a style sheet to be human readable.
							writer.WriteStartElement("table");
								writer.WriteAttributeString("width","100%");
								writer.WriteAttributeString("border","1");
								writer.WriteStartElement("thead");
									writer.WriteStartElement("tr");
										writer.WriteStartElement("th");
										  writer.WriteString("LOINC Code");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Test");
										writer.WriteEndElement();//
										writer.WriteStartElement("th");
											writer.WriteString("Result");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Abnormal Flag");
										writer.WriteEndElement();
										writer.WriteStartElement("th");
											writer.WriteString("Date Performed");
										writer.WriteEndElement();
									writer.WriteEndElement();//End tr
								writer.WriteEndElement();//End thead
								writer.WriteStartElement("tbody");
									for(int i=0;i<listLabResult.Count;i++){
										writer.WriteStartElement("tr");
										writer.WriteStartElement("td");
										writer.WriteString(listLabResult[i].TestID.ToString());
										writer.WriteEndElement();
										writer.WriteStartElement("td");
										writer.WriteString(listLabResult[i].TestName);
										writer.WriteEndElement();
										writer.WriteStartElement("td");
										writer.WriteString(listLabResult[i].AbnormalFlag.ToString());
										writer.WriteEndElement();
										writer.WriteStartElement("td");
										writer.WriteString(listLabResult[i].DateTimeTest.ToShortDateString());
										writer.WriteEndElement();
										writer.WriteEndElement();//End tr
									}
								writer.WriteEndElement();//End tbody
							writer.WriteEndElement();//End table
						writer.WriteEndElement();//End text
					writer.WriteEndElement();//End Diagnostic Test Results component
				writer.WriteEndElement();
				return strBuilder;
			}
		}*/
#endregion OldCode

