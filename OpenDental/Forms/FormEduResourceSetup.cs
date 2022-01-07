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
		private List<EduResource> _listEduResources;

		public FormEduResourceSetup() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEduResourceSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridEdu.BeginUpdate();
			gridEdu.Columns.Clear();
			GridColumn col=new GridColumn("Criteria",300);
			gridEdu.Columns.Add(col);
			col=new GridColumn("Link",700);
			gridEdu.Columns.Add(col);
			_listEduResources=EduResources.SelectAll();
			gridEdu.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEduResources.Count;i++) {
				row=new GridRow();
				if(_listEduResources[i].DiseaseDefNum!=0) {
					row.Cells.Add("Problem: "+DiseaseDefs.GetItem(_listEduResources[i].DiseaseDefNum).DiseaseName);
					//row.Cells.Add("ICD9: "+DiseaseDefs.GetItem(_listEduResources[i].DiseaseDefNum).ICD9Code);
				}
				else if(_listEduResources[i].MedicationNum!=0) {
					row.Cells.Add("Medication: "+Medications.GetDescription(_listEduResources[i].MedicationNum));
				}
				else if(_listEduResources[i].SmokingSnoMed!="") {
					Snomed snomed=Snomeds.GetByCode(_listEduResources[i].SmokingSnoMed);
					string criteria="Tobacco Use Assessment: ";
					if(snomed!=null) {
						criteria+=snomed.Description;
					}
					row.Cells.Add(criteria);
				}
				else {
					row.Cells.Add("Lab Results: "+_listEduResources[i].LabResultName+" "+_listEduResources[i].LabResultCompare);
				}
				row.Cells.Add(_listEduResources[i].ResourceUrl);
				gridEdu.ListGridRows.Add(row);
			}
			gridEdu.EndUpdate();
		}

		private void gridEdu_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEduResourceEdit formEduResourceEdit = new FormEduResourceEdit();
			formEduResourceEdit.EduResourceCur=_listEduResources[e.Row];
			formEduResourceEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEduResourceEdit formEduResourceEdit = new FormEduResourceEdit();
			formEduResourceEdit.IsNew=true;
			formEduResourceEdit.EduResourceCur=new EduResource();
			formEduResourceEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	


	}
}
