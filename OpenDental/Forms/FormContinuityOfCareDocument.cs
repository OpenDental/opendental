using System;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormContinuityOfCareDocument:FormODBase {
		public Patient PatCur;

		public FormContinuityOfCareDocument() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butShowAndPrint_Click(object sender,EventArgs e) {
			string continuityOfCareText="";
			try {
				continuityOfCareText=EhrCCD.GenerateClinicalSummary(PatCur,true,true,true,true,true,true,true,true,true,true,true,true,
					textInstructions.Text,DateTime.MinValue);
			} 
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			bool didPrint=FormEhrSummaryOfCare.DisplayCCD(continuityOfCareText);
			if(didPrint) {
				//we are printing a ccd so add new measure event.					
				EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
				ehrMeasureEvent.DateTEvent=DateTime.Now;
				ehrMeasureEvent.EventType=EhrMeasureEventType.ClinicalSummaryProvidedToPt;
				ehrMeasureEvent.PatNum=PatCur.PatNum;
				EhrMeasureEvents.Insert(ehrMeasureEvent);
			}
		}

		private void butExportToFile_Click(object sender,EventArgs e) {
			string continuityOfCareText="";
			try {
				continuityOfCareText=EhrCCD.GenerateClinicalSummary(PatCur,true,true,true,true,true,true,true,true,true,true,true,true,
					textInstructions.Text,DateTime.MinValue);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			using FolderBrowserDialog formBrowserDialog=new FolderBrowserDialog();
			formBrowserDialog.SelectedPath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());//Default to patient image folder.
			DialogResult dialogResult=formBrowserDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK) {
				return;
			}
			if(File.Exists(Path.Combine(formBrowserDialog.SelectedPath,"ccd.xml"))) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Overwrite existing ccd.xml file?")) {
					return;
				}
			}
			File.WriteAllText(Path.Combine(formBrowserDialog.SelectedPath,"ccd.xml"),continuityOfCareText);
			File.WriteAllText(Path.Combine(formBrowserDialog.SelectedPath,"ccd.xsl"),FormEHR.GetEhrResource("CCD"));
			EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=DateTime.Now;
			ehrMeasureEvent.EventType=EhrMeasureEventType.ClinicalSummaryProvidedToPt;
			ehrMeasureEvent.PatNum=PatCur.PatNum;
			EhrMeasureEvents.Insert(ehrMeasureEvent);
			MsgBox.Show(this,"File has been exported.");
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

	}
}