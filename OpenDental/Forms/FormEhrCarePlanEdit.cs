using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrCarePlanEdit:FormODBase {

		private EhrCarePlan _ehrCarePlan;
		private Snomed _snomedGoal;

		public FormEhrCarePlanEdit(EhrCarePlan ehrCarePlan) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_ehrCarePlan=ehrCarePlan;
		}

		private void FormEhrCarePlanEdit_Load(object sender,EventArgs e) {
			textDate.Text=_ehrCarePlan.DatePlanned.ToShortDateString();
			_snomedGoal=null;
			if(!String.IsNullOrEmpty(_ehrCarePlan.SnomedEducation)) {//Blank if new
				_snomedGoal=Snomeds.GetByCode(_ehrCarePlan.SnomedEducation);
				textSnomedGoal.Text=_snomedGoal.Description;
			}
			textInstructions.Text=_ehrCarePlan.Instructions;
		}

		private void butSnomedGoalSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formS=new FormSnomeds();
			formS.IsSelectionMode=true;
			if(formS.ShowDialog()==DialogResult.OK) {
				_snomedGoal=formS.SelectedSnomed;
				textSnomedGoal.Text=_snomedGoal.Description;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_ehrCarePlan.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			EhrCarePlans.Delete(_ehrCarePlan.EhrCarePlanNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DateTime date;
			try {
				date=DateTime.Parse(textDate.Text);
			}
			catch {
				MsgBox.Show(this,"Date invalid");
				return;
			}
			if(_snomedGoal==null) {
				MsgBox.Show(this,"Missing SNOMED CT goal");
				return;
			}
			_ehrCarePlan.DatePlanned=date;
			_ehrCarePlan.SnomedEducation=_snomedGoal.SnomedCode;
			_ehrCarePlan.Instructions=textInstructions.Text;
			if(_ehrCarePlan.IsNew) {
				EhrCarePlans.Insert(_ehrCarePlan);
			}
			else {
				EhrCarePlans.Update(_ehrCarePlan);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}