using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEvaluationDefEdit:FormODBase {
		private EvaluationDef _evalDefCur;
		private List<EvaluationCriterionDef> _criterionDefsForEval;
		private Dictionary<long,GradingScale> _gradingScales;
		private List<long> _itemOrder;

		public FormEvaluationDefEdit(EvaluationDef evalDefCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evalDefCur=evalDefCur;
		}

		private void FormEvaluationDefEdit_Load(object sender,EventArgs e) {
			if(!_evalDefCur.IsNew) {
				textTitle.Text=_evalDefCur.EvalTitle;
				textCourse.Text=SchoolCourses.GetDescript(_evalDefCur.SchoolCourseNum);
				textGradeScaleName.Text=GradingScales.GetOne(_evalDefCur.GradingScaleNum).Description;
			}
			_criterionDefsForEval=EvaluationCriterionDefs.GetAllForEvaluationDef(_evalDefCur.EvaluationDefNum);
			_itemOrder=new List<long>();
			for(int j=0;j<_criterionDefsForEval.Count;j++) {
				_itemOrder.Add(_criterionDefsForEval[j].EvaluationCriterionDefNum);
			}
			List<GradingScale> gradingScales=GradingScales.RefreshList();
			_gradingScales=new Dictionary<long,GradingScale>();
			for(int i=0;i<gradingScales.Count;i++) {
				_gradingScales.Add(gradingScales[i].GradingScaleNum,gradingScales[i]);
			}
			if(_gradingScales.Count!=0 
				&& !_evalDefCur.IsNew
				&& _gradingScales[_evalDefCur.GradingScaleNum].ScaleType!=EnumScaleType.Weighted) 
			{
				labelTotalPoint.Visible=false;
				textTotalPoints.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid() {//Also fills total points if necessary.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationDefEdit","Description"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationDefEdit","Grading Scale"),100);
			gridMain.ListGridColumns.Add(col);
			if(_evalDefCur.GradingScaleNum!=0
				&& _gradingScales[_evalDefCur.GradingScaleNum].ScaleType==EnumScaleType.Weighted) 
			{
				col=new GridColumn(Lan.g("FormEvaluationDefEdit","Max Points"),80);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			float points=0;
			for(int i=0;i<_criterionDefsForEval.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_criterionDefsForEval[i].CriterionDescript);
				if(_criterionDefsForEval[i].IsCategoryName) {
					row.Bold=true;
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_gradingScales[_criterionDefsForEval[i].GradingScaleNum].Description);
				}
				if(_evalDefCur.GradingScaleNum!=0 
					&& _gradingScales[_evalDefCur.GradingScaleNum].ScaleType==EnumScaleType.Weighted)
				{
					points+=_criterionDefsForEval[i].MaxPointsPoss;
					if(_criterionDefsForEval[i].IsCategoryName) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_criterionDefsForEval[i].MaxPointsPoss.ToString());
					}
				}
				gridMain.ListGridRows.Add(row);
				
			}
			gridMain.EndUpdate();
			textTotalPoints.Text=points.ToString();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEvaluationCriterionDefEdit FormECDE=new FormEvaluationCriterionDefEdit(_criterionDefsForEval[gridMain.GetSelectedIndex()]);
			FormECDE.ShowDialog();
			if(FormECDE.DialogResult==DialogResult.OK) {
				if(_criterionDefsForEval.Count!=_itemOrder.Count) {
					_itemOrder.Remove(_criterionDefsForEval[gridMain.GetSelectedIndex()].EvaluationCriterionDefNum);//Must be called before refreshing.
				}
				_criterionDefsForEval=EvaluationCriterionDefs.GetAllForEvaluationDef(_evalDefCur.EvaluationDefNum);
				SynchItemOrder();
				FillGrid();
			}
		}

		private void butCriterionAdd_Click(object sender,EventArgs e) {
			if(_evalDefCur.GradingScaleNum==0) {
				MsgBox.Show(this,"Please select a grading scale before adding criterion.");
				return;
			}
			EvaluationCriterionDef evalCritDef=new EvaluationCriterionDef();
			evalCritDef.EvaluationDefNum=_evalDefCur.EvaluationDefNum;
			evalCritDef.GradingScaleNum=_evalDefCur.GradingScaleNum;
			evalCritDef.IsNew=true;
			using FormEvaluationCriterionDefEdit FormECDE=new FormEvaluationCriterionDefEdit(evalCritDef);
			FormECDE.ShowDialog();
			if(FormECDE.DialogResult==DialogResult.OK) {
				_criterionDefsForEval=EvaluationCriterionDefs.GetAllForEvaluationDef(_evalDefCur.EvaluationDefNum);
				_itemOrder.Add(evalCritDef.EvaluationCriterionDefNum);//Must be called after refreshing
				SynchItemOrder();
				FillGrid();
			}
		}

		/// <summary>Used after adding or deleting an EvaluationCriterionDef.  Enables item order to persist.</summary>
		private void SynchItemOrder() {
			List<EvaluationCriterionDef> tempList=new List<EvaluationCriterionDef>();
			for(int i=0;i<_itemOrder.Count;i++) {
				for(int j=0;j<_criterionDefsForEval.Count;j++) {
					if(_itemOrder[i]==_criterionDefsForEval[j].EvaluationCriterionDefNum) {
						tempList.Add(_criterionDefsForEval[j]);
						break;
					}
				}
			}
			_criterionDefsForEval=tempList;
		}

		private void butGradingScale_Click(object sender,EventArgs e) {
			using FormGradingScales FormGS=new FormGradingScales();
			FormGS.IsSelectionMode=true;
			FormGS.ShowDialog();
			if(FormGS.DialogResult==DialogResult.OK) {
				textGradeScaleName.Text=FormGS.SelectedGradingScale.Description;
				_evalDefCur.GradingScaleNum=FormGS.SelectedGradingScale.GradingScaleNum;
				//If they added a new grading scale, add the scale to the dictionary.
				if(!_gradingScales.ContainsKey(FormGS.SelectedGradingScale.GradingScaleNum)) {
					_gradingScales.Add(FormGS.SelectedGradingScale.GradingScaleNum,GradingScales.GetOne(FormGS.SelectedGradingScale.GradingScaleNum));
				}
				if(_gradingScales[_evalDefCur.GradingScaleNum].ScaleType==EnumScaleType.Weighted) {
					labelTotalPoint.Visible=true;
					textTotalPoints.Visible=true;
				}
				else {
					labelTotalPoint.Visible=false;
					textTotalPoints.Visible=false;
				}
				FillGrid();//If they changed the grading scale type from weighted, this will remove the extra column.
			}
		}

		private void butCoursePicker_Click(object sender,EventArgs e) {
			using FormSchoolCourses FormSC=new FormSchoolCourses();
			FormSC.IsSelectionMode=true;
			FormSC.ShowDialog();
			if(FormSC.DialogResult==DialogResult.OK) {
				_evalDefCur.SchoolCourseNum=FormSC.CourseSelected.SchoolCourseNum;
				textCourse.Text=FormSC.CourseSelected.CourseID;
			}
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[0]==0) {
				return;
			}
			for(int i=0;i<selected.Length;i++) {
				_criterionDefsForEval.Reverse(selected[i]-1,2);
				_itemOrder.Reverse(selected[i]-1,2);
			}
			FillGrid();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]-1,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[selected.Length-1]==_criterionDefsForEval.Count-1) {
				return;
			}
			for(int i=selected.Length-1;i>=0;i--) {//go backwards
				_criterionDefsForEval.Reverse(selected[i],2);
				_itemOrder.Reverse(selected[i],2);
			}
			FillGrid();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]+1,true);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evalDefCur.IsNew || MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the evaluation def.  Continue?")) {
				EvaluationDefs.Delete(_evalDefCur.EvaluationDefNum);
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_evalDefCur.SchoolCourseNum==0) {
				MsgBox.Show(this,"A school course must be selected for this evaluation def before it can be saved.");
				return;
			}
			if(_evalDefCur.GradingScaleNum==0) {
				MsgBox.Show(this,"A grading scale must be selected for this evaluation def before it can be saved.");
				return;
			}
			if(!String.IsNullOrWhiteSpace(_evalDefCur.EvalTitle) 
				&& _evalDefCur.EvalTitle!=textTitle.Text 
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Changing the EvaluationDef titles during a term could interfere with grading reports.  Continue?")) 
			{
				return;
			}
			_evalDefCur.EvalTitle=textTitle.Text;
			EvaluationDefs.Update(_evalDefCur);
			for(int i=0;i<_criterionDefsForEval.Count;i++) {
				_criterionDefsForEval[i].ItemOrder=i;
				EvaluationCriterionDefs.Update(_criterionDefsForEval[i]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_evalDefCur.IsNew) {
				EvaluationDefs.Delete(_evalDefCur.EvaluationDefNum);
			}
			DialogResult=DialogResult.Cancel;
		}

		


	}
}