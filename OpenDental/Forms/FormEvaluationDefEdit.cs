using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormEvaluationDefEdit:FormODBase {
		private EvaluationDef _evaluationDef;
		private List<EvaluationCriterionDef> _listEvaluationCriterionDefs;
		private List<GradingScale> _listGradingScales;
		private List<long> _listItemOrders;

		public FormEvaluationDefEdit(EvaluationDef evaluationDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evaluationDef=evaluationDef;
		}

		private void FormEvaluationDefEdit_Load(object sender,EventArgs e) {
			if(!_evaluationDef.IsNew) {
				textTitle.Text=_evaluationDef.EvalTitle;
				textCourse.Text=SchoolCourses.GetDescript(_evaluationDef.SchoolCourseNum);
				textGradeScaleName.Text=GradingScales.GetOne(_evaluationDef.GradingScaleNum).Description;
			}
			_listEvaluationCriterionDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(_evaluationDef.EvaluationDefNum);
			_listItemOrders=new List<long>();
			for(int j=0;j<_listEvaluationCriterionDefs.Count;j++) {
				_listItemOrders.Add(_listEvaluationCriterionDefs[j].EvaluationCriterionDefNum);
			}
			_listGradingScales=GradingScales.RefreshList();
			if(_listGradingScales.Count!=0 && !_evaluationDef.IsNew) {
				GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_evaluationDef.GradingScaleNum);
				if(gradingScale!=null && gradingScale.ScaleType!=EnumScaleType.Weighted) {
					labelTotalPoint.Visible=false;
					textTotalPoints.Visible=false;
				}
			}
			FillGrid();
		}

		private void FillGrid() {//Also fills total points if necessary.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationDefEdit","Description"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationDefEdit","Grading Scale"),100);
			gridMain.Columns.Add(col);
			GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_evaluationDef.GradingScaleNum);
			if(gradingScale!=null 
				&& _evaluationDef.GradingScaleNum!=0 
				&& gradingScale.ScaleType==EnumScaleType.Weighted) 
			{
				col=new GridColumn(Lan.g("FormEvaluationDefEdit","Max Points"),80);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			float points=0;
			for(int i=0;i<_listEvaluationCriterionDefs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEvaluationCriterionDefs[i].CriterionDescript);
				if(_listEvaluationCriterionDefs[i].IsCategoryName) {
					row.Bold=true;
					row.Cells.Add("");
				}
				else {
					GradingScale gradingScaleEvaluationCriterion=_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterionDefs[i].GradingScaleNum);
					if(gradingScaleEvaluationCriterion!=null) {
						row.Cells.Add(_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterionDefs[i].GradingScaleNum).Description);
					}
				}
				if(gradingScale!=null 
					&& _evaluationDef.GradingScaleNum!=0 
					&& gradingScale.ScaleType==EnumScaleType.Weighted)
				{
					points+=_listEvaluationCriterionDefs[i].MaxPointsPoss;
					if(_listEvaluationCriterionDefs[i].IsCategoryName) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_listEvaluationCriterionDefs[i].MaxPointsPoss.ToString());
					}
				}
				gridMain.ListGridRows.Add(row);
				
			}
			gridMain.EndUpdate();
			textTotalPoints.Text=points.ToString();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEvaluationCriterionDefEdit formEvaluationCriterionDefEdit=new FormEvaluationCriterionDefEdit(_listEvaluationCriterionDefs[gridMain.GetSelectedIndex()]);
			formEvaluationCriterionDefEdit.ShowDialog();
			if(formEvaluationCriterionDefEdit.DialogResult==DialogResult.OK) {
				if(_listEvaluationCriterionDefs.Count!=_listItemOrders.Count) {
					_listItemOrders.Remove(_listEvaluationCriterionDefs[gridMain.GetSelectedIndex()].EvaluationCriterionDefNum);//Must be called before refreshing.
				}
				_listEvaluationCriterionDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(_evaluationDef.EvaluationDefNum);
				SynchItemOrder();
				FillGrid();
			}
		}

		private void butCriterionAdd_Click(object sender,EventArgs e) {
			if(_evaluationDef.GradingScaleNum==0) {
				MsgBox.Show(this,"Please select a grading scale before adding criterion.");
				return;
			}
			EvaluationCriterionDef evaluationCriterionDef=new EvaluationCriterionDef();
			evaluationCriterionDef.EvaluationDefNum=_evaluationDef.EvaluationDefNum;
			evaluationCriterionDef.GradingScaleNum=_evaluationDef.GradingScaleNum;
			evaluationCriterionDef.IsNew=true;
			using FormEvaluationCriterionDefEdit formEvaluationCriterionDefEdit=new FormEvaluationCriterionDefEdit(evaluationCriterionDef);
			formEvaluationCriterionDefEdit.ShowDialog();
			if(formEvaluationCriterionDefEdit.DialogResult==DialogResult.OK) {
				_listEvaluationCriterionDefs=EvaluationCriterionDefs.GetAllForEvaluationDef(_evaluationDef.EvaluationDefNum);
				_listItemOrders.Add(evaluationCriterionDef.EvaluationCriterionDefNum);//Must be called after refreshing
				SynchItemOrder();
				FillGrid();
			}
		}

		/// <summary>Used after adding or deleting an EvaluationCriterionDef.  Enables item order to persist.</summary>
		private void SynchItemOrder() {
			List<EvaluationCriterionDef> listEvaluationCriterionDefs=new List<EvaluationCriterionDef>();
			for(int i=0;i<_listItemOrders.Count;i++) {
				for(int j=0;j<_listEvaluationCriterionDefs.Count;j++) {
					if(_listItemOrders[i]==_listEvaluationCriterionDefs[j].EvaluationCriterionDefNum) {
						listEvaluationCriterionDefs.Add(_listEvaluationCriterionDefs[j]);
						break;
					}
				}
			}
			_listEvaluationCriterionDefs=listEvaluationCriterionDefs;
		}

		private void butGradingScale_Click(object sender,EventArgs e) {
			using FormGradingScales formGradingScales=new FormGradingScales();
			formGradingScales.IsSelectionMode=true;
			formGradingScales.ShowDialog();
			if(formGradingScales.DialogResult==DialogResult.OK) {
				textGradeScaleName.Text=formGradingScales.GradingScaleSelected.Description;
				_evaluationDef.GradingScaleNum=formGradingScales.GradingScaleSelected.GradingScaleNum;
				GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==formGradingScales.GradingScaleSelected.GradingScaleNum);
				if(gradingScale==null) { //Grading scale doesn't exist, so add a new one to the list
					gradingScale=GradingScales.GetOne(formGradingScales.GradingScaleSelected.GradingScaleNum);
					_listGradingScales.Add(gradingScale);
				}
				if(gradingScale.ScaleType==EnumScaleType.Weighted) {
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
			using FormSchoolCourses formSchoolCourses=new FormSchoolCourses();
			formSchoolCourses.IsSelectionMode=true;
			formSchoolCourses.ShowDialog();
			if(formSchoolCourses.DialogResult==DialogResult.OK) {
				_evaluationDef.SchoolCourseNum=formSchoolCourses.CourseSelected.SchoolCourseNum;
				textCourse.Text=formSchoolCourses.CourseSelected.CourseID;
			}
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] intArraySelectedIndices=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				intArraySelectedIndices[i]=gridMain.SelectedIndices[i];
			}
			if(intArraySelectedIndices[0]==0) {
				return;
			}
			for(int i=0;i<intArraySelectedIndices.Length;i++) {
				_listEvaluationCriterionDefs.Reverse(intArraySelectedIndices[i]-1,2);
				_listItemOrders.Reverse(intArraySelectedIndices[i]-1,2);
			}
			FillGrid();
			for(int i=0;i<intArraySelectedIndices.Length;i++) {
				gridMain.SetSelected(intArraySelectedIndices[i]-1,setValue:true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] intArraySelectedIndices=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				intArraySelectedIndices[i]=gridMain.SelectedIndices[i];
			}
			if(intArraySelectedIndices[intArraySelectedIndices.Length-1]==_listEvaluationCriterionDefs.Count-1) {
				return;
			}
			for(int i=intArraySelectedIndices.Length-1;i>=0;i--) {//go backwards
				_listEvaluationCriterionDefs.Reverse(intArraySelectedIndices[i],2);
				_listItemOrders.Reverse(intArraySelectedIndices[i],2);
			}
			FillGrid();
			for(int i=0;i<intArraySelectedIndices.Length;i++) {
				gridMain.SetSelected(intArraySelectedIndices[i]+1,setValue:true);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evaluationDef.IsNew || MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the evaluation def.  Continue?")) {
				EvaluationDefs.Delete(_evaluationDef.EvaluationDefNum);
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_evaluationDef.SchoolCourseNum==0) {
				MsgBox.Show(this,"A school course must be selected for this evaluation def before it can be saved.");
				return;
			}
			if(_evaluationDef.GradingScaleNum==0) {
				MsgBox.Show(this,"A grading scale must be selected for this evaluation def before it can be saved.");
				return;
			}
			if(!String.IsNullOrWhiteSpace(_evaluationDef.EvalTitle) 
				&& _evaluationDef.EvalTitle!=textTitle.Text 
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Changing the EvaluationDef titles during a term could interfere with grading reports.  Continue?")) 
			{
				return;
			}
			_evaluationDef.EvalTitle=textTitle.Text;
			EvaluationDefs.Update(_evaluationDef);
			for(int i=0;i<_listEvaluationCriterionDefs.Count;i++) {
				_listEvaluationCriterionDefs[i].ItemOrder=i;
				EvaluationCriterionDefs.Update(_listEvaluationCriterionDefs[i]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_evaluationDef.IsNew) {
				EvaluationDefs.Delete(_evaluationDef.EvaluationDefNum);
			}
			DialogResult=DialogResult.Cancel;
		}

		


	}
}