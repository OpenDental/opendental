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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEvaluationSetup","Course"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluationSetup","Evaluation Title"),180);
			gridMain.ListGridColumns.Add(col);
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
			EvaluationDef evalDef=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			using FormEvaluationDefEdit FormEDE=new FormEvaluationDefEdit(evalDef);
			FormEDE.ShowDialog();
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
			EvaluationDef evalDefOld=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			EvaluationDef evalDefNew=evalDefOld.Copy();
			evalDefNew.EvalTitle+="-copy";
			evalDefNew.EvaluationDefNum=EvaluationDefs.Insert(evalDefNew);
			List<EvaluationCriterionDef> listCritDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(evalDefOld.EvaluationDefNum);
			for(int i=0;i<listCritDefs.Count;i++) {
				EvaluationCriterionDef critDefCopy=listCritDefs[i].Copy();
				critDefCopy.EvaluationDefNum=evalDefNew.EvaluationDefNum;
				EvaluationCriterionDefs.Insert(critDefCopy);
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EvaluationDef evalDef=new EvaluationDef();
			evalDef.IsNew=true;
			evalDef.EvaluationDefNum=EvaluationDefs.Insert(evalDef);
			using FormEvaluationDefEdit FormEDE=new FormEvaluationDefEdit(evalDef);
			FormEDE.ShowDialog();
			if(FormEDE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		/// <summary>The selected Def from the grid will be copied into a brand new Evaluation and saved to the DB. This includes all EvaluationCriterion as well. Used when creating a new Evaluation.</summary>
		private void CopyDefToEvaluation() {
			EvaluationDef evalDef=EvaluationDefs.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString()));
			Evaluation evalNew=new Evaluation();
			evalNew.DateEval=DateTime.Today;
			evalNew.EvalTitle=evalDef.EvalTitle;
			evalNew.GradingScaleNum=evalDef.GradingScaleNum;
			evalNew.InstructNum=Security.CurUser.ProvNum;
			evalNew.SchoolCourseNum=evalDef.SchoolCourseNum;
			evalNew.EvaluationNum=Evaluations.Insert(evalNew);
			List<EvaluationCriterionDef> evalCritDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(evalDef.EvaluationDefNum);
			EvaluationCriterion evalCrit;
			for(int i=0;i<evalCritDefs.Count;i++) {
				evalCrit=new EvaluationCriterion();
				evalCrit.CriterionDescript=evalCritDefs[i].CriterionDescript;
				evalCrit.EvaluationNum=evalNew.EvaluationNum;
				evalCrit.GradingScaleNum=evalCritDefs[i].GradingScaleNum;
				evalCrit.IsCategoryName=evalCritDefs[i].IsCategoryName;
				evalCrit.ItemOrder=evalCritDefs[i].ItemOrder;
				evalCrit.MaxPointsPoss=evalCritDefs[i].MaxPointsPoss;
				EvaluationCriterions.Insert(evalCrit);
			}
			evalNew.IsNew=true;
			using FormEvaluationEdit FormEE=new FormEvaluationEdit(evalNew);
			FormEE.ShowDialog();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}




	}
}