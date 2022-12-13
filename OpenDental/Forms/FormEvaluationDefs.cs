using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEvaluationDefs:FormODBase {
		/// <summary>This mode is only used for picking an evaluationdef for a brand new evaluation.</summary>
		public bool IsSelectionMode=false;
		public int CourseIndex;
		private List<SchoolCourse> _listSchoolCourses;

		public FormEvaluationDefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEvaluationDefs_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butDuplicate.Visible=false;
				butAdd.Visible=false;
			}
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			comboCourse.Items.Add("All");
			for(int i=0;i<_listSchoolCourses.Count;i++) {
				comboCourse.Items.Add(_listSchoolCourses[i].CourseID);
			}
			comboCourse.SelectedIndex=CourseIndex;
			FillGrid();
		}

		private void FillGrid() {
			long courseNum=0;
			if(comboCourse.SelectedIndex!=0) {
				courseNum=_listSchoolCourses[comboCourse.SelectedIndex-1].SchoolCourseNum;
			}
			DataTable table=EvaluationDefs.GetAllByCourse(courseNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEvaluationSetup","Course"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluationSetup","Evaluation Title"),180);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["CourseID"].ToString());
				row.Cells.Add(table.Rows[i]["EvalTitle"].ToString());
				row.Tag=table.Rows[i]["EvaluationDefNum"].ToString();
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				CopyDefToEvaluation();
				DialogResult=DialogResult.OK;
				return;
			}
			EvaluationDef evaluationDef=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			using FormEvaluationDefEdit formEvaluationDefEdit=new FormEvaluationDefEdit(evaluationDef);
			formEvaluationDefEdit.ShowDialog();
			FillGrid();
		}

		private void comboCourse_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butDuplicate_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an evaluation to duplicate");
				return;
			}
			//Creates a full copy of the EvaluationDef including all EvaluationCriterionDefs.
			EvaluationDef evaluationDefOld=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			EvaluationDef evaluationDefNew=evaluationDefOld.Copy();
			evaluationDefNew.EvalTitle+="-copy";
			evaluationDefNew.EvaluationDefNum=EvaluationDefs.Insert(evaluationDefNew);
			List<EvaluationCriterionDef> listEvaluationCriterionDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(evaluationDefOld.EvaluationDefNum);
			for(int i=0;i<listEvaluationCriterionDefs.Count;i++) {
				EvaluationCriterionDef evaluationCriterionDef=listEvaluationCriterionDefs[i].Copy();
				evaluationCriterionDef.EvaluationDefNum=evaluationDefNew.EvaluationDefNum;
				EvaluationCriterionDefs.Insert(evaluationCriterionDef);
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EvaluationDef evaluationDef=new EvaluationDef();
			evaluationDef.IsNew=true;
			evaluationDef.EvaluationDefNum=EvaluationDefs.Insert(evaluationDef);
			using FormEvaluationDefEdit formEvaluationDefEdit=new FormEvaluationDefEdit(evaluationDef);
			formEvaluationDefEdit.ShowDialog();
			if(formEvaluationDefEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		/// <summary>The selected Def from the grid will be copied into a brand new Evaluation and saved to the DB. This includes all EvaluationCriterion as well. Used when creating a new Evaluation.</summary>
		private void CopyDefToEvaluation() {
			EvaluationDef evaluationDef=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			Evaluation evaluation=new Evaluation();
			evaluation.DateEval=DateTime.Today;
			evaluation.EvalTitle=evaluationDef.EvalTitle;
			evaluation.GradingScaleNum=evaluationDef.GradingScaleNum;
			evaluation.InstructNum=Security.CurUser.ProvNum;
			evaluation.SchoolCourseNum=evaluationDef.SchoolCourseNum;
			evaluation.EvaluationNum=Evaluations.Insert(evaluation);
			List<EvaluationCriterionDef> listEvaluationCriterionDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(evaluationDef.EvaluationDefNum);
			EvaluationCriterion evaluationCriterion;
			for(int i=0;i<listEvaluationCriterionDefs.Count;i++) {
				evaluationCriterion=new EvaluationCriterion();
				evaluationCriterion.CriterionDescript=listEvaluationCriterionDefs[i].CriterionDescript;
				evaluationCriterion.EvaluationNum=evaluation.EvaluationNum;
				evaluationCriterion.GradingScaleNum=listEvaluationCriterionDefs[i].GradingScaleNum;
				evaluationCriterion.IsCategoryName=listEvaluationCriterionDefs[i].IsCategoryName;
				evaluationCriterion.ItemOrder=listEvaluationCriterionDefs[i].ItemOrder;
				evaluationCriterion.MaxPointsPoss=listEvaluationCriterionDefs[i].MaxPointsPoss;
				EvaluationCriterions.Insert(evaluationCriterion);
			}
			evaluation.IsNew=true;
			using FormEvaluationEdit formEvaluationEdit=new FormEvaluationEdit(evaluation);
			formEvaluationEdit.ShowDialog();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}




	}
}