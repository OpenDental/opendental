using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Xml;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrClinicalSummary:FormODBase {
		public Patient PatCur;
		private List<EhrMeasureEvent> summariesSentList;

		public FormEhrClinicalSummary() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormClinicalSummary_Load(object sender,EventArgs e) {
			FillGridEHRMeasureEvents();
		}

		private void FillGridEHRMeasureEvents() {
			gridEHRMeasureEvents.BeginUpdate();
			gridEHRMeasureEvents.ListGridColumns.Clear();
			GridColumn col = new GridColumn("DateTime",140);
			gridEHRMeasureEvents.ListGridColumns.Add(col);
			//col = new ODGridColumn("Details",600);
			//gridEHRMeasureEvents.Columns.Add(col);
			summariesSentList = EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.ClinicalSummaryProvidedToPt);
			gridEHRMeasureEvents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<summariesSentList.Count;i++) {
				row = new GridRow();
				row.Cells.Add(summariesSentList[i].DateTEvent.ToString());
				//row.Cells.Add(summariesSentList[i].EventType.ToString());
				gridEHRMeasureEvents.ListGridRows.Add(row);
			}
			gridEHRMeasureEvents.EndUpdate();
		}

		private void butExport_Click(object sender,EventArgs e) {
			string ccd="";
			try {
				using FormEhrExportCCD FormEEC=new FormEhrExportCCD(PatCur);
				FormEEC.ShowDialog();
				if(FormEEC.DialogResult==DialogResult.OK) {
					ccd=FormEEC.CCD;
				}
				else {
					return;
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FolderBrowserDialog dlg=new FolderBrowserDialog();
			dlg.SelectedPath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());//Default to patient image folder.
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK) {
				return;
			}
			if(File.Exists(Path.Combine(dlg.SelectedPath,"ccd.xml"))){
				if(MessageBox.Show("Overwrite existing ccd.xml?","",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xml"),ccd);
			File.WriteAllText(Path.Combine(dlg.SelectedPath,"ccd.xsl"),FormEHR.GetEhrResource("CCD"));
			EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			newMeasureEvent.DateTEvent = DateTime.Now;
			newMeasureEvent.EventType = EhrMeasureEventType.ClinicalSummaryProvidedToPt;
			newMeasureEvent.PatNum = PatCur.PatNum;
			EhrMeasureEvents.Insert(newMeasureEvent);
			FillGridEHRMeasureEvents();
			MessageBox.Show("Exported");	
		}

		private void butSendToPortal_Click(object sender,EventArgs e) {
			//Validate
			string strCcdValidationErrors=EhrCCD.ValidateSettings();
			if(strCcdValidationErrors!="") {//Do not even try to export if global settings are invalid.
				MessageBox.Show(strCcdValidationErrors);//We do not want to use translations here, because the text is dynamic. The errors are generated in the business layer, and Lan.g() is not available there.
				return;
			}
			strCcdValidationErrors=EhrCCD.ValidatePatient(PatCur);//Patient cannot be null, because a patient must be selected before the EHR dashboard will open.
			if(strCcdValidationErrors!="") {
				MessageBox.Show(strCcdValidationErrors);//We do not want to use translations here, because the text is dynamic. The errors are generated in the business layer, and Lan.g() is not available there.
				return;
			}
			Provider prov=null;
			if(Security.CurUser.ProvNum!=0) {//If the current user is a provider.
				prov=Providers.GetProv(Security.CurUser.ProvNum);
			}
			else {
				prov=Providers.GetProv(PatCur.PriProv);//PriProv is not 0, because EhrCCD.ValidatePatient() will block if PriProv is 0.
			}
			try {
				//Create the Clinical Summary.
				using FormEhrExportCCD FormEEC=new FormEhrExportCCD(PatCur);
				FormEEC.ShowDialog();
				if(FormEEC.DialogResult!=DialogResult.OK) {//Canceled
					return;
				}
				//Save the clinical summary (ccd.xml) and style sheet (ccd.xsl) as webmail message attachments.
				//TODO: It would be more patient friendly if we instead generated a PDF file containing the Clinical Summary printout, or if we simply displayed the Clinical Summary in the portal.
				//The CMS definition does not prohibit sending human readable files, and sending a PDF to the portal mimics printing the Clinical Summary and handing to patient.
				Random rnd=new Random();
				string attachPath=EmailAttaches.GetAttachPath();
				List<EmailAttach> listAttachments=new List<EmailAttach>();
				EmailAttach attachCcd=new EmailAttach();//Save Clinical Summary to file in the email attachments folder.
				attachCcd.DisplayedFileName="ccd.xml";
				attachCcd.ActualFileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".xml";
				listAttachments.Add(attachCcd);
				FileAtoZ.WriteAllText(FileAtoZ.CombinePaths(attachPath,attachCcd.ActualFileName),FormEEC.CCD,"Uploading Attachment for Clinical Summary...");			
				EmailAttach attachSs=new EmailAttach();//Style sheet attachment.
				attachSs.DisplayedFileName="ccd.xsl";
				attachSs.ActualFileName=attachCcd.ActualFileName.Substring(0,attachCcd.ActualFileName.Length-4)+".xsl";//Same base name as the CCD.  The base names must match or the file will not display properly in internet browsers.
				listAttachments.Add(attachSs);
				FileAtoZ.WriteAllText(FileAtoZ.CombinePaths(attachPath,attachSs.ActualFileName),FormEHR.GetEhrResource("CCD"),
					"Uploading Attachment for Clinical Summary...");
				//Create and save the webmail message containing the attachments.
				EmailMessage msgWebMail=new EmailMessage();				
				msgWebMail.FromAddress=prov.GetFormalName();
				msgWebMail.ToAddress=PatCur.GetNameFL();
				msgWebMail.PatNum=PatCur.PatNum;
				msgWebMail.SentOrReceived=EmailSentOrReceived.WebMailSent;
				msgWebMail.ProvNumWebMail=prov.ProvNum;
				msgWebMail.Subject="Clinical Summary";
				msgWebMail.BodyText="To view the clinical summary:\r\n1) Download all attachments to the same folder.  Do not rename the files.\r\n2) Open the ccd.xml file in an internet browser.";
				msgWebMail.MsgDateTime=DateTime.Now;
				msgWebMail.PatNumSubj=PatCur.PatNum;
				msgWebMail.Attachments=listAttachments;
				msgWebMail.MsgType=EmailMessageSource.WebMail;
				EmailMessages.Insert(msgWebMail);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.EventType=EhrMeasureEventType.ClinicalSummaryProvidedToPt;
			newMeasureEvent.PatNum=PatCur.PatNum;
			EhrMeasureEvents.Insert(newMeasureEvent);
			FillGridEHRMeasureEvents();//This will cause the measure event to show in the grid below the popup message on the next line.  Reassures the user that the event was immediately recorded.
			MsgBox.Show(this,"Clinical Summary Sent");
		}

		private void butShowXhtml_Click(object sender,EventArgs e) {
			string ccd="";
			try {
				using FormEhrExportCCD FormEEC=new FormEhrExportCCD(PatCur);
				FormEEC.ShowDialog();
				if(FormEEC.DialogResult==DialogResult.OK) {
					ccd=FormEEC.CCD;
				}
				else {
					return;
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			bool didPrint=FormEhrSummaryOfCare.DisplayCCD(ccd);
			if(didPrint) {
				//we are printing a ccd so add new measure event.					
				EhrMeasureEvent measureEvent = new EhrMeasureEvent();
				measureEvent.DateTEvent = DateTime.Now;
				measureEvent.EventType = EhrMeasureEventType.ClinicalSummaryProvidedToPt;
				measureEvent.PatNum = PatCur.PatNum;
				EhrMeasureEvents.Insert(measureEvent);
				FillGridEHRMeasureEvents();
			}		
		}

		private void butShowXml_Click(object sender,EventArgs e) {
			string ccd="";
			try {
				using FormEhrExportCCD FormEEC=new FormEhrExportCCD(PatCur);
				FormEEC.ShowDialog();
				if(FormEEC.DialogResult==DialogResult.OK) {
					ccd=FormEEC.CCD;
				}
				else {
					return;
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(ccd);
			msgbox.ShowDialog();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridEHRMeasureEvents.SelectedIndices.Length < 1) {
				MessageBox.Show("Please select at least one record to delete.");
				return;
			}
			for(int i=0;i<gridEHRMeasureEvents.SelectedIndices.Length;i++) {
				EhrMeasureEvents.Delete(summariesSentList[gridEHRMeasureEvents.SelectedIndices[i]].EhrMeasureEventNum);
			}
			FillGridEHRMeasureEvents();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	

		











	}
}
