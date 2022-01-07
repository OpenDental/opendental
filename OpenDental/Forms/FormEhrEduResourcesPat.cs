using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrEduResourcesPat:FormODBase {
		public Patient patCur;
		private List<EduResource> eduResourceList = new List<EduResource>();
		private List<EhrMeasureEvent> eduMeasureProvidedList = new List<EhrMeasureEvent>();

		public FormEhrEduResourcesPat() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEduResourcesPat_Load(object sender,EventArgs e) {
			FillGridEdu();
			FillGridProvided();
		}

		private void FillGridEdu() {
			gridEdu.BeginUpdate();
			gridEdu.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Criteria",300);
			gridEdu.ListGridColumns.Add(col);
			col=new GridColumn("Link",100);
			gridEdu.ListGridColumns.Add(col);
			eduResourceList=EduResources.GenerateForPatient(patCur.PatNum);
			gridEdu.ListGridRows.Clear();
			GridRow row;
			foreach(EduResource eduResCur in eduResourceList) {
				row=new GridRow();
				if(eduResCur.DiseaseDefNum!=0) {
					row.Cells.Add("Problem: "+DiseaseDefs.GetItem(eduResCur.DiseaseDefNum).DiseaseName);
					//row.Cells.Add("ICD9: "+DiseaseDefs.GetItem(eduResCur.DiseaseDefNum).ICD9Code);
				}
				else if(eduResCur.MedicationNum!=0) {
					row.Cells.Add("Medication: "+Medications.GetDescription(eduResCur.MedicationNum));
				}
				else if(eduResCur.SmokingSnoMed!="") {
					Snomed sCur=Snomeds.GetByCode(eduResCur.SmokingSnoMed);
					string criteriaCur="Tobacco Use Assessment: ";
					if(sCur!=null) {
						criteriaCur+=sCur.Description;
					}
					row.Cells.Add(criteriaCur);
				}
				else {
					row.Cells.Add("Lab Results: "+eduResCur.LabResultName);
				}
				row.Cells.Add(eduResCur.ResourceUrl);
				gridEdu.ListGridRows.Add(row);
			}
			gridEdu.EndUpdate();
		}

		private void gridEdu_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col!=1) {
				return;
			}
			bool didPrint = false;
			try {
				using FormEhrEduBrowser FormEDUB = new FormEhrEduBrowser(eduResourceList[e.Row].ResourceUrl);
				FormEDUB.ShowDialog();
				didPrint = FormEDUB.DidPrint;
				//System.Diagnostics.Process.Start(eduResourceList[e.Row].ResourceUrl);
			}
			catch {
				MessageBox.Show("Link not found.");
				return;
			}
			if(didPrint) {
				EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
				newMeasureEvent.DateTEvent=DateTime.Now;
				newMeasureEvent.EventType=EhrMeasureEventType.EducationProvided;
				newMeasureEvent.PatNum=patCur.PatNum;
				newMeasureEvent.MoreInfo=eduResourceList[e.Row].ResourceUrl;
				EhrMeasureEvents.Insert(newMeasureEvent);
				FillGridProvided();
			}
		}

		private void FillGridProvided() {
			gridProvided.BeginUpdate();
			gridProvided.ListGridColumns.Clear();
			GridColumn col=new GridColumn("DateTime",140);
			gridProvided.ListGridColumns.Add(col);
			col=new GridColumn("Details",600);
			gridProvided.ListGridColumns.Add(col);
			eduMeasureProvidedList=EhrMeasureEvents.RefreshByType(patCur.PatNum,EhrMeasureEventType.EducationProvided);
			gridProvided.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<eduMeasureProvidedList.Count;i++) {
				row=new GridRow();
				row.Cells.Add(eduMeasureProvidedList[i].DateTEvent.ToString());
				row.Cells.Add(eduMeasureProvidedList[i].MoreInfo);
				gridProvided.ListGridRows.Add(row);
			}
			gridProvided.EndUpdate();
		}
		
		private void butDelete_Click(object sender,EventArgs e) {
			if(gridProvided.SelectedIndices.Length<1) {
				MessageBox.Show("Please select at least one record to delete.");
				return;
			}
			for(int i=0;i<gridProvided.SelectedIndices.Length;i++) {
				EhrMeasureEvents.Delete(eduMeasureProvidedList[gridProvided.SelectedIndices[i]].EhrMeasureEventNum);
			}
			FillGridProvided();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	}
}
