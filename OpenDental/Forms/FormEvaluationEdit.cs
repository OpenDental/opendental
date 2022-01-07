using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEvaluationEdit:FormODBase {
		private Evaluation _evalCur;
		private Provider _provInstructor;
		private Provider _provStudent;
		private List<EvaluationCriterion> _listEvalCrits;
		private GradingScale _evalGradeScale;
		private List<GradingScaleItem> _listEvalGradeItems;
		private Dictionary<long,GradingScale> _dictCritGradeScales=new Dictionary<long,GradingScale>();
		private Dictionary<long,List<GradingScaleItem>> _dictCritGradeItems=new Dictionary<long,List<GradingScaleItem>>();
		private long _currentGradingNum=0;


		public FormEvaluationEdit(Evaluation evalCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evalCur=evalCur;
		}

		private void FormEvaluationEdit_Load(object sender,EventArgs e) {
			//This window fills all necessary data on load. This eliminates the need for multiple calls to the database.
			textDate.Text=_evalCur.DateEval.ToShortDateString();
			textTitle.Text=_evalCur.EvalTitle;
			_evalGradeScale=GradingScales.GetOne(_evalCur.GradingScaleNum);
			_listEvalGradeItems=GradingScaleItems.Refresh(_evalCur.GradingScaleNum);
			textGradeScaleName.Text=_evalGradeScale.Description;
			_provInstructor=Providers.GetProv(_evalCur.InstructNum);
			textInstructor.Text=_provInstructor.GetLongDesc();
			_provStudent=Providers.GetProv(_evalCur.StudentNum);
			if(_provStudent!=null) {
				textStudent.Text=_provStudent.GetLongDesc();
			}
			textCourse.Text=SchoolCourses.GetDescript(_evalCur.SchoolCourseNum);
			textGradeNumberOverride.Text=_evalCur.OverallGradeNumber.ToString();
			textGradeShowingOverride.Text=_evalCur.OverallGradeShowing;
			_listEvalCrits=EvaluationCriterions.Refresh(_evalCur.EvaluationNum);
			for(int i=0;i<_listEvalCrits.Count;i++) {
				GradingScale critGradeScale=GradingScales.GetOne(_listEvalCrits[i].GradingScaleNum);
				if(!_dictCritGradeScales.ContainsKey(critGradeScale.GradingScaleNum)) {
					_dictCritGradeScales.Add(critGradeScale.GradingScaleNum,critGradeScale);
				}
				if(!_dictCritGradeItems.ContainsKey(critGradeScale.GradingScaleNum)) {
					_dictCritGradeItems.Add(critGradeScale.GradingScaleNum,GradingScaleItems.Refresh(critGradeScale.GradingScaleNum));
				}
			}
			FillGridCriterion();
			RecalculateGrades();
			//Since there is no override column in the database, we check for equality of the calculated grade and the grade on the evaluation.
			//If they are different then the grade was overwritten at some point.
			if(textGradeNumber.Text==textGradeNumberOverride.Text) {
				textGradeNumberOverride.Text="";
			}
			if(textGradeShowing.Text==textGradeShowingOverride.Text) {
				textGradeShowingOverride.Text="";
			}
		}

		private void FillGridCriterion() {
			gridCriterion.BeginUpdate();
			gridCriterion.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Description"),150);
			gridCriterion.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Grading Scale"),90);
			gridCriterion.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Showing"),60);
			gridCriterion.ListGridColumns.Add(col);
			//This column is directly referenced later.  If the order of the columns changes then the direct reference must also change.
			col=new GridColumn(Lan.g("FormEvaluationEdit","Number"),60);
			col.IsEditable=true;
			gridCriterion.ListGridColumns.Add(col);
			//This column is directly referenced later.  If the order of the columns changes then the direct reference must also change.
			col=new GridColumn(Lan.g("FormEvaluationEdit","Note"),120);
			col.IsEditable=true;
			gridCriterion.ListGridColumns.Add(col);
			gridCriterion.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEvalCrits.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEvalCrits[i].CriterionDescript);
				if(_listEvalCrits[i].IsCategoryName) {//Categories do not get filled
					row.Bold=true;
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_dictCritGradeScales[_listEvalCrits[i].GradingScaleNum].Description);
					row.Cells.Add(_listEvalCrits[i].GradeShowing);
					row.Cells.Add(_listEvalCrits[i].GradeNumber.ToString());
					row.Cells.Add(_listEvalCrits[i].Notes);
				}
				gridCriterion.ListGridRows.Add(row);
			}
			gridCriterion.EndUpdate();
		}

		private void FillGridGrading(long gradingScaleNum) {
			if(_dictCritGradeScales[gradingScaleNum].ScaleType==EnumScaleType.Weighted) {
				FillGridGradingWithWeighted(gradingScaleNum);
			}
			else if(_dictCritGradeScales[gradingScaleNum].ScaleType==EnumScaleType.Percentage) {
				FillGridGradingWithPercentage(gradingScaleNum);
			}
			else {//PickList ScaleType
				FillGridGradingWithPickList(gradingScaleNum);
			}
		}

		private void FillGridGradingWithWeighted(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Max Point Value"),100,HorizontalAlignment.Center);
			gridGrades.ListGridColumns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvalCrits[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			row.Cells.Add(_listEvalCrits[gridCriterion.SelectedCell.Y].MaxPointsPoss.ToString());
			gridGrades.ListGridRows.Add(row);
			gridGrades.EndUpdate();
		}

		private void FillGridGradingWithPercentage(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Percentage"),100,HorizontalAlignment.Center);
			gridGrades.ListGridColumns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvalCrits[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			row.Cells.Add(Lan.g("FormEvaluationEdit","0 to 100"));
			gridGrades.ListGridRows.Add(row);
			gridGrades.EndUpdate();
		}

		private void FillGridGradingWithPickList(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Number"),60);
			gridGrades.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Showing"),60);
			gridGrades.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Description"),150);
			gridGrades.ListGridColumns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvalCrits[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			for(int i=0;i<_dictCritGradeItems[gradingScaleNum].Count;i++) {
				row=new GridRow();
				row.Cells.Add(_dictCritGradeItems[gradingScaleNum][i].GradeNumber.ToString());
				row.Cells.Add(_dictCritGradeItems[gradingScaleNum][i].GradeShowing);
				row.Cells.Add(_dictCritGradeItems[gradingScaleNum][i].Description);
				gridGrades.ListGridRows.Add(row);
				gridGrades.EndUpdate();
			}
		}

		private void gridCriterion_CellEnter(object sender,ODGridClickEventArgs e) {
			long selectedGradingScaleNum=_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum;
			if(_currentGradingNum!=selectedGradingScaleNum) {
				FillGridGrading(_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum);
			}
			gridGrades.SetAll(false);
		}

		private void gridCriterion_CellClick(object sender,ODGridClickEventArgs e) {
			long selectedGradingScaleNum=_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum;
			if(_currentGradingNum!=selectedGradingScaleNum) {
				FillGridGrading(_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum);
			}
			gridGrades.SetAll(false);
		}

		private void gridCriterion_CellLeave(object sender,ODGridClickEventArgs e) {
			if(gridCriterion.SelectedCell.X==-1 || gridCriterion.SelectedCell.Y==-1) {
				return;
			}
			//If the user somehow gets to a Y value that is greater than or equal to the count (Y is 0 based), return.
			if(gridCriterion.SelectedCell.Y>=_listEvalCrits.Count) {
				return;
			}
			//Make sure that the user didn't get to an invalid X (0 based).
			if(gridCriterion.SelectedCell.X>=gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells.Count) {
				return;
			}
			if(_listEvalCrits[gridCriterion.SelectedCell.Y].IsCategoryName) {//This must happen first so categories are always set to blank columns
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[gridCriterion.SelectedCell.X].Text="";
				return;
			}
			if(gridCriterion.SelectedCell.X==4) {//Cell 4 is the Notes Column
				_listEvalCrits[gridCriterion.SelectedCell.Y].Notes=gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[4].Text;
				return;
			}
			//Everything below this point assumes changes may have been made to the GradeNumber column.
			float number=0;
			if(!float.TryParse(gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text,out number)) {// Cell 3 is the GradeNumber column
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text=_listEvalCrits[gridCriterion.SelectedCell.Y].GradeNumber.ToString();
				return;
			}
			if(_dictCritGradeScales[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum].ScaleType==EnumScaleType.PickList) {
				//If using a picklist the value entered must equal a value that exists in the list. You cannot type in a value that does not have a corresponding grade item.
				bool isValid=false;
				for(int i=0;i<_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum].Count;i++) {
					if(number==_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeNumber) {
						_listEvalCrits[gridCriterion.SelectedCell.Y].GradeShowing=_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeShowing;
						gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[2].Text=_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeShowing;
						isValid=true;
						gridCriterion.Invalidate();//Needed to redraw grid without unselecting the row.
						break;
					}
				}
				if(!isValid) {
					gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text=_listEvalCrits[gridCriterion.SelectedCell.Y].GradeNumber.ToString();
					gridCriterion.Invalidate();//Needed to redraw grid without unselecting the row.
				}
			}
			else {
				_listEvalCrits[gridCriterion.SelectedCell.Y].GradeShowing=number.ToString();
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[2].Text=number.ToString();
			}
			_listEvalCrits[gridCriterion.SelectedCell.Y].GradeNumber=number;
			RecalculateGrades();
		}

		private void gridGrades_CellClick(object sender,ODGridClickEventArgs e) {
			if(_dictCritGradeScales[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum].ScaleType==EnumScaleType.Weighted
				|| _dictCritGradeScales[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum].ScaleType==EnumScaleType.Percentage) 
			{
					return;
			}
			_listEvalCrits[gridCriterion.SelectedCell.Y].GradeNumber=_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum][gridGrades.GetSelectedIndex()].GradeNumber;
			_listEvalCrits[gridCriterion.SelectedCell.Y].GradeShowing=_dictCritGradeItems[_listEvalCrits[gridCriterion.SelectedCell.Y].GradingScaleNum][gridGrades.GetSelectedIndex()].GradeShowing;
			Point oldSelectedCell=gridCriterion.SelectedCell;
			FillGridCriterion();
			gridCriterion.SetSelected(oldSelectedCell);
			RecalculateGrades();
		}

		private void RecalculateGrades() {
			//Grades are recalculated after every change to grade numbers. This includes choosing from the grade grid or typing it in manually.
			//Only Criterion that match the Evaluation grading scale will be considered when calculating grades.
			//All other criterion will need to be manually calculated using the override function.
			float gradeNumberDisplay=0;
			int critCount=0;
			float gradeSum=0;
			float maxPoints=0;
			for(int i=0;i<_listEvalCrits.Count;i++) {
				if(_evalGradeScale.ScaleType==_dictCritGradeScales[_listEvalCrits[i].GradingScaleNum].ScaleType) {
					critCount++;
					gradeSum+=_listEvalCrits[i].GradeNumber;
					maxPoints+=_listEvalCrits[i].MaxPointsPoss;
				}
			}
			if(_evalGradeScale.ScaleType==EnumScaleType.PickList || _evalGradeScale.ScaleType==EnumScaleType.Percentage) {
				//Grades here are calculated as an average. All criterion in these modes will be worth the same "weight".
				//This could be improved later by adding weights to questions. This would mean they get 'averaged' into the grade
				//multiple times dependent on the weight. i.e. Criterion 1 has a weight of 3 and it gets graded an A.
				//The calculation will have 3 A's instead of just one for that criterion.
				if(critCount!=0) {
					gradeNumberDisplay=gradeSum/critCount;
				}
			}
			if(_evalGradeScale.ScaleType==EnumScaleType.Weighted) {
				//This grade is calculated as a sum of all criterion grade numbers over the maximum points possible all the criterion.
				gradeNumberDisplay=gradeSum;
			}
			if(_evalGradeScale.ScaleType==EnumScaleType.Percentage) {
				textGradeNumber.Text=gradeNumberDisplay.ToString();
				textGradeShowing.Text=gradeNumberDisplay.ToString();
			}
			if(_evalGradeScale.ScaleType==EnumScaleType.PickList){
				float dif=float.MaxValue;
				float closestNumber=0;
				string closestShowing="";
				for(int i=0;i<_listEvalGradeItems.Count;i++) {
					if(Math.Abs(_listEvalGradeItems[i].GradeNumber-gradeNumberDisplay) < dif) {
						dif=Math.Abs(_listEvalGradeItems[i].GradeNumber-gradeNumberDisplay);
						closestNumber=_listEvalGradeItems[i].GradeNumber;
						closestShowing=_listEvalGradeItems[i].GradeShowing;
					}
				}
				textGradeNumber.Text=closestNumber.ToString();
				textGradeShowing.Text=closestShowing;
			}
			if(_evalGradeScale.ScaleType==EnumScaleType.Weighted) {
				textGradeNumber.Text=gradeNumberDisplay.ToString();
				textGradeShowing.Text=gradeNumberDisplay+"/"+maxPoints;
			}
		}

		private void butStudentPicker_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick();
			FormPP.IsStudentPicker=true;
			FormPP.ShowDialog();
			if(FormPP.DialogResult==DialogResult.OK) {
				_provStudent=Providers.GetProv(FormPP.SelectedProvNum);
				_evalCur.StudentNum=_provStudent.ProvNum;
				textStudent.Text=_provStudent.GetLongDesc();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evalCur.IsNew || MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the evaluation.  Continue?")) {
				Evaluations.Delete(_evalCur.EvaluationNum);
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(_provStudent==null) {
				MsgBox.Show(this,"Please attach a student to this evaluation.");
				return;
			}
			if(!String.IsNullOrWhiteSpace(textGradeNumberOverride.Text)) {
				//Overrides are not saved to the database. They are found by comparing calculated grades to grades found in the database.
				//If they are identical or blank then they have not been overwritten.
				//This will need to be taken into account when reporting since it is possible to override one grade column but not the other. i.e. A->B but number stays at 4.
				float parsed=0;
				if(!float.TryParse(textGradeNumberOverride.Text,out parsed)) {
					MsgBox.Show(this,"The override for Overall Grade Number is not a valid number.  Please input a valid number to save the evaluation.");
					return;
				}
				_evalCur.OverallGradeNumber=parsed;
			}
			for(int i=0;i<_listEvalCrits.Count;i++) {
				_listEvalCrits[i].Notes=gridCriterion.ListGridRows[i].Cells[4].Text;//Cell 4 is the notes column
				EvaluationCriterions.Update(_listEvalCrits[i]);
			}
			_evalCur.DateEval=DateTime.Parse(textDate.Text);
			_evalCur.StudentNum=_provStudent.ProvNum;
			_evalCur.OverallGradeShowing=textGradeShowing.Text;
			_evalCur.OverallGradeNumber=PIn.Float(textGradeNumber.Text);
			if(!String.IsNullOrWhiteSpace(textGradeShowingOverride.Text)) {
				_evalCur.OverallGradeShowing=textGradeShowingOverride.Text;
			}
			Evaluations.Update(_evalCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}