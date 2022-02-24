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
	public partial class FormReminderRules:FormODBase {
		public List<ReminderRule> listReminders;
		public FormReminderRules() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReminderRules_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Reminder Criterion",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Message",200);
			gridMain.ListGridColumns.Add(col);
			listReminders=ReminderRules.SelectAll();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listReminders.Count;i++) {
				row=new GridRow();
				switch(listReminders[i].ReminderCriterion) {
					case EhrCriterion.Problem:
						DiseaseDef def=DiseaseDefs.GetItem(listReminders[i].CriterionFK);
						row.Cells.Add("Problem ="+def.ICD9Code+" "+def.DiseaseName);
						break;
					case EhrCriterion.Medication:
						Medication tempMed = Medications.GetMedication(listReminders[i].CriterionFK);
						if(tempMed.MedicationNum==tempMed.GenericNum) {//handle generic medication names.
							row.Cells.Add("Medication = "+tempMed.MedName);
						}
						else {
							row.Cells.Add("Medication = "+tempMed.MedName+" / "+Medications.GetGenericName(tempMed.GenericNum));
						}
						break;
					case EhrCriterion.Allergy:
						row.Cells.Add("Allergy = "+AllergyDefs.GetOne(listReminders[i].CriterionFK).Description);
						break;
					case EhrCriterion.Age:
						row.Cells.Add("Age "+listReminders[i].CriterionValue);
						break;
					case EhrCriterion.Gender:
						row.Cells.Add("Gender is "+listReminders[i].CriterionValue);
						break;
					case EhrCriterion.LabResult:
						row.Cells.Add("LabResult "+listReminders[i].CriterionValue);
						break;
					//case EhrCriterion.ICD9:
					//  row.Cells.Add("ICD9 "+ICD9s.GetDescription(listReminders[i].CriterionFK));
					//  break;
				}
				row.Cells.Add(listReminders[i].Message);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReminderRuleEdit FormRRE=new FormReminderRuleEdit();
			FormRRE.RuleCur = listReminders[e.Row];
			FormRRE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormReminderRuleEdit FormRRE=new FormReminderRuleEdit();
			FormRRE.IsNew=true;
			FormRRE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}
