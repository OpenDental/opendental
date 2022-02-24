using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEduResourceSetup:FormODBase {
		private List<EduResource> eduResourceList;

		public FormEduResourceSetup() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEduResourceSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridEdu.BeginUpdate();
			gridEdu.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Criteria",300);
			gridEdu.ListGridColumns.Add(col);
			col=new GridColumn("Link",700);
			gridEdu.ListGridColumns.Add(col);
			eduResourceList=EduResources.SelectAll();
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
					row.Cells.Add("Lab Results: "+eduResCur.LabResultName+" "+eduResCur.LabResultCompare);
				}
				row.Cells.Add(eduResCur.ResourceUrl);
				gridEdu.ListGridRows.Add(row);
			}
			gridEdu.EndUpdate();
		}

		private void gridEdu_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEduResourceEdit FormERE = new FormEduResourceEdit();
			FormERE.EduResourceCur=eduResourceList[e.Row];
			FormERE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEduResourceEdit FormERE = new FormEduResourceEdit();
			FormERE.IsNew=true;
			FormERE.EduResourceCur=new EduResource();
			FormERE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	


	}
}
