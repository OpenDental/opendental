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
		public List<ReminderRule> ListReminderRules;
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
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Reminder Criterion",200);
			gridMain.Columns.Add(col);
			col=new GridColumn("Message",200);
			gridMain.Columns.Add(col);
			ListReminderRules=ReminderRules.SelectAll();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListReminderRules.Count;i++) {
				row=new GridRow();
				switch(ListReminderRules[i].ReminderCriterion) {
					case EhrCriterion.Problem:
						DiseaseDef diseaseDef=DiseaseDefs.GetItem(ListReminderRules[i].CriterionFK);
						row.Cells.Add("Problem ="+diseaseDef.ICD9Code+" "+diseaseDef.DiseaseName);
						break;
					case EhrCriterion.Medication:
						Medication medication = Medications.GetMedication(ListReminderRules[i].CriterionFK);
						if(medication.MedicationNum==medication.GenericNum) {//handle generic medication names.
							row.Cells.Add("Medication = "+medication.MedName);
						}
						else {
							row.Cells.Add("Medication = "+medication.MedName+" / "+Medications.GetGenericName(medication.GenericNum));
						}
						break;
					case EhrCriterion.Allergy:
						row.Cells.Add("Allergy = "+AllergyDefs.GetOne(ListReminderRules[i].CriterionFK).Description);
						break;
					case EhrCriterion.Age:
						row.Cells.Add("Age "+ListReminderRules[i].CriterionValue);
						break;
					case EhrCriterion.Gender:
						row.Cells.Add("Gender is "+ListReminderRules[i].CriterionValue);
						break;
					case EhrCriterion.LabResult:
						row.Cells.Add("LabResult "+ListReminderRules[i].CriterionValue);
						break;
					//case EhrCriterion.ICD9:
					//  row.Cells.Add("ICD9 "+ICD9s.GetDescription(listReminders[i].CriterionFK));
					//  break;
				}
				row.Cells.Add(ListReminderRules[i].Message);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReminderRuleEdit formReminderRuleEdit=new FormReminderRuleEdit();
			formReminderRuleEdit.ReminderRuleCur = ListReminderRules[e.Row];
			formReminderRuleEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormReminderRuleEdit formReminderRuleEdit=new FormReminderRuleEdit();
			formReminderRuleEdit.IsNew=true;
			formReminderRuleEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}
