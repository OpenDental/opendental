using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrCarePlans:FormODBase {

		private Patient _patCur;
		private List<EhrCarePlan> _listCarePlans;

		public FormEhrCarePlans(Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
		}

		private void FormEhrCarePlans_Load(object sender,EventArgs e) {
			FillCarePlans();
		}

		private void FormEhrCarePlans_Resize(object sender,EventArgs e) {
			FillCarePlans();//So the columns will resize.
		}

		private void FillCarePlans() {
			gridCarePlans.BeginUpdate();
			gridCarePlans.ListGridColumns.Clear();
			int colDatePixCount=66;
			int variablePixCount=gridCarePlans.Width-10-colDatePixCount;
			int colGoalPixCount=variablePixCount/2;
			int colInstructionsPixCount=variablePixCount-colGoalPixCount;
			gridCarePlans.ListGridColumns.Add(new UI.GridColumn("Date",colDatePixCount));
			gridCarePlans.ListGridColumns.Add(new UI.GridColumn("Goal",colGoalPixCount));
			gridCarePlans.ListGridColumns.Add(new UI.GridColumn("Instructions",colInstructionsPixCount));
			gridCarePlans.EndUpdate();
			gridCarePlans.BeginUpdate();
			gridCarePlans.ListGridRows.Clear();
			_listCarePlans=EhrCarePlans.Refresh(_patCur.PatNum);
			for(int i=0;i<_listCarePlans.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_listCarePlans[i].DatePlanned.ToShortDateString());//Date
				Snomed snomedEducation=Snomeds.GetByCode(_listCarePlans[i].SnomedEducation);
				if(snomedEducation==null) {
					row.Cells.Add("");//We allow blank or "NullFlavor" SNOMEDCT codes when exporting CCDAs, so we allow them to be blank when displaying here as well.
				}
				else {
					row.Cells.Add(snomedEducation.Description);//GoalDescript
				}
				row.Cells.Add(_listCarePlans[i].Instructions);//Instructions
				gridCarePlans.ListGridRows.Add(row);
			}
			gridCarePlans.EndUpdate();
		}

		private void gridCarePlans_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormEhrCarePlanEdit formEdit=new FormEhrCarePlanEdit(_listCarePlans[e.Row]);
			if(formEdit.ShowDialog()==DialogResult.OK) {
				FillCarePlans();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EhrCarePlan ehrCarePlan=new EhrCarePlan();
			ehrCarePlan.IsNew=true;
			ehrCarePlan.PatNum=_patCur.PatNum;
			ehrCarePlan.DatePlanned=DateTime.Today;
			using FormEhrCarePlanEdit formEdit=new FormEhrCarePlanEdit(ehrCarePlan);
			if(formEdit.ShowDialog()==DialogResult.OK) {
				FillCarePlans();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}