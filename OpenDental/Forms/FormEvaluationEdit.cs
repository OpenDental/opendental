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
	public partial class FormEvaluationEdit:FormODBase {
		private Evaluation _evaluation;
		private Provider _providerInstructor;
		private Provider _providerStudent;
		private List<EvaluationCriterion> _listEvaluationCriterions;
		private GradingScale _gradingScale;
		private List<GradingScaleItem> _listGradingScaleItems;
		private List<GradingScale> _listGradingScales=new List<GradingScale>();
		private Dictionary<long,List<GradingScaleItem>> _dictionaryListsGradingScaleItems=new Dictionary<long,List<GradingScaleItem>>();
		private long _gradingNum=0;


		public FormEvaluationEdit(Evaluation evaluation) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evaluation=evaluation;
		}

		private void FormEvaluationEdit_Load(object sender,EventArgs e) {
			//This window fills all necessary data on load. This eliminates the need for multiple calls to the database.
			textDate.Text=_evaluation.DateEval.ToShortDateString();
			textTitle.Text=_evaluation.EvalTitle;
			_gradingScale=GradingScales.GetOne(_evaluation.GradingScaleNum);
			_listGradingScaleItems=GradingScaleItems.Refresh(_evaluation.GradingScaleNum);
			textGradeScaleName.Text=_gradingScale.Description;
			_providerInstructor=Providers.GetProv(_evaluation.InstructNum);
			textInstructor.Text=_providerInstructor.GetLongDesc();
			_providerStudent=Providers.GetProv(_evaluation.StudentNum);
			if(_providerStudent!=null) {
				textStudent.Text=_providerStudent.GetLongDesc();
			}
			textCourse.Text=SchoolCourses.GetDescript(_evaluation.SchoolCourseNum);
			textGradeNumberOverride.Text=_evaluation.OverallGradeNumber.ToString();
			textGradeShowingOverride.Text=_evaluation.OverallGradeShowing;
			_listEvaluationCriterions=EvaluationCriterions.Refresh(_evaluation.EvaluationNum);
			for(int i=0;i<_listEvaluationCriterions.Count;i++) {
				GradingScale gradingScaleNew=GradingScales.GetOne(_listEvaluationCriterions[i].GradingScaleNum);
				GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==gradingScaleNew.GradingScaleNum);
				if(gradingScale is null) { //New grading scale isn't a duplicate, so add it to our list of grading scales.
					_listGradingScales.Add(gradingScaleNew);
				}
				if(!_dictionaryListsGradingScaleItems.ContainsKey(gradingScaleNew.GradingScaleNum)) {
					_dictionaryListsGradingScaleItems.Add(gradingScaleNew.GradingScaleNum,GradingScaleItems.Refresh(gradingScaleNew.GradingScaleNum));
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
			gridCriterion.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Description"),150);
			gridCriterion.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Grading Scale"),90);
			gridCriterion.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Showing"),60);
			gridCriterion.Columns.Add(col);
			//This column is directly referenced later.  If the order of the columns changes then the direct reference must also change.
			col=new GridColumn(Lan.g("FormEvaluationEdit","Number"),60);
			col.IsEditable=true;
			gridCriterion.Columns.Add(col);
			//This column is directly referenced later.  If the order of the columns changes then the direct reference must also change.
			col=new GridColumn(Lan.g("FormEvaluationEdit","Note"),120);
			col.IsEditable=true;
			gridCriterion.Columns.Add(col);
			gridCriterion.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEvaluationCriterions.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEvaluationCriterions[i].CriterionDescript);
				if(_listEvaluationCriterions[i].IsCategoryName) {//Categories do not get filled
					row.Bold=true;
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterions[i].GradingScaleNum);
					if(gradingScale is null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(gradingScale.Description);
					}
					row.Cells.Add(_listEvaluationCriterions[i].GradeShowing);
					row.Cells.Add(_listEvaluationCriterions[i].GradeNumber.ToString());
					row.Cells.Add(_listEvaluationCriterions[i].Notes);
				}
				gridCriterion.ListGridRows.Add(row);
			}
			gridCriterion.EndUpdate();
		}

		private void FillGridGrading(long gradingScaleNum) {
			GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==gradingScaleNum);
			if(gradingScale is null) {
				FillGridGradingWithPickList(gradingScaleNum);
				return;
			}
			if(gradingScale.ScaleType==EnumScaleType.Weighted) {
				FillGridGradingWithWeighted(gradingScaleNum);
			}
			else if(gradingScale.ScaleType==EnumScaleType.Percentage) {
				FillGridGradingWithPercentage(gradingScaleNum);
			}
			else {//PickList ScaleType
				FillGridGradingWithPickList(gradingScaleNum);
			}
		}

		private void FillGridGradingWithWeighted(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Max Point Value"),100,HorizontalAlignment.Center);
			gridGrades.Columns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			row.Cells.Add(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].MaxPointsPoss.ToString());
			gridGrades.ListGridRows.Add(row);
			gridGrades.EndUpdate();
		}

		private void FillGridGradingWithPercentage(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Percentage"),100,HorizontalAlignment.Center);
			gridGrades.Columns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			row.Cells.Add(Lan.g("FormEvaluationEdit","0 to 100"));
			gridGrades.ListGridRows.Add(row);
			gridGrades.EndUpdate();
		}

		private void FillGridGradingWithPickList(long gradingScaleNum) {
			gridGrades.BeginUpdate();
			gridGrades.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationEdit","Number"),60);
			gridGrades.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Showing"),60);
			gridGrades.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationEdit","Description"),150);
			gridGrades.Columns.Add(col);
			gridGrades.ListGridRows.Clear();
			GridRow row=new GridRow();
			if(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].IsCategoryName) {//Refill the grid with nothing if a category is selected.
				gridGrades.EndUpdate();
				return;
			}
			for(int i=0;i<_dictionaryListsGradingScaleItems[gradingScaleNum].Count;i++) {
				row=new GridRow();
				row.Cells.Add(_dictionaryListsGradingScaleItems[gradingScaleNum][i].GradeNumber.ToString());
				row.Cells.Add(_dictionaryListsGradingScaleItems[gradingScaleNum][i].GradeShowing);
				row.Cells.Add(_dictionaryListsGradingScaleItems[gradingScaleNum][i].Description);
				gridGrades.ListGridRows.Add(row);
				gridGrades.EndUpdate();
			}
		}

		private void gridCriterion_CellEnter(object sender,ODGridClickEventArgs e) {
			long gradingScaleNumSelected=_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum;
			if(_gradingNum!=gradingScaleNumSelected) {
				FillGridGrading(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum);
			}
			gridGrades.SetAll(false);
		}

		private void gridCriterion_CellClick(object sender,ODGridClickEventArgs e) {
			long gradingScaleNumSelected=_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum;
			if(_gradingNum!=gradingScaleNumSelected) {
				FillGridGrading(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum);
			}
			gridGrades.SetAll(false);
		}

		private void gridCriterion_CellLeave(object sender,ODGridClickEventArgs e) {
			if(gridCriterion.SelectedCell.X==-1 || gridCriterion.SelectedCell.Y==-1) {
				return;
			}
			//If the user somehow gets to a Y value that is greater than or equal to the count (Y is 0 based), return.
			if(gridCriterion.SelectedCell.Y>=_listEvaluationCriterions.Count) {
				return;
			}
			//Make sure that the user didn't get to an invalid X (0 based).
			if(gridCriterion.SelectedCell.X>=gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells.Count) {
				return;
			}
			if(_listEvaluationCriterions[gridCriterion.SelectedCell.Y].IsCategoryName) {//This must happen first so categories are always set to blank columns
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[gridCriterion.SelectedCell.X].Text="";
				return;
			}
			if(gridCriterion.SelectedCell.X==4) {//Cell 4 is the Notes Column
				_listEvaluationCriterions[gridCriterion.SelectedCell.Y].Notes=gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[4].Text;
				return;
			}
			//Everything below this point assumes changes may have been made to the GradeNumber column.
			float number=0;
			if(!float.TryParse(gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text,out number)) {// Cell 3 is the GradeNumber column
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text=_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeNumber.ToString();
				return;
			}
			GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum);
			if(gradingScale!=null && gradingScale.ScaleType==EnumScaleType.PickList) {
				//If using a picklist the value entered must equal a value that exists in the list. You cannot type in a value that does not have a corresponding grade item.
				bool isValid=false;
				for(int i=0;i<_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum].Count;i++) {
					if(number==_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeNumber) {
						_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeShowing=_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeShowing;
						gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[2].Text=_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum][i].GradeShowing;
						isValid=true;
						gridCriterion.Invalidate();//Needed to redraw grid without unselecting the row.
						break;
					}
				}
				if(!isValid) {
					gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[3].Text=_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeNumber.ToString();
					gridCriterion.Invalidate();//Needed to redraw grid without unselecting the row.
				}
			}
			else {
				_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeShowing=number.ToString();
				gridCriterion.ListGridRows[gridCriterion.SelectedCell.Y].Cells[2].Text=number.ToString();
			}
			_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeNumber=number;
			RecalculateGrades();
		}

		private void gridGrades_CellClick(object sender,ODGridClickEventArgs e) {
			GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum);
			if(gradingScale is null) {
				return;
			}
			if(gradingScale.ScaleType==EnumScaleType.Weighted || gradingScale.ScaleType==EnumScaleType.Percentage)
			{
				return;
			}
			_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeNumber=_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum][gridGrades.GetSelectedIndex()].GradeNumber;
			_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradeShowing=_dictionaryListsGradingScaleItems[_listEvaluationCriterions[gridCriterion.SelectedCell.Y].GradingScaleNum][gridGrades.GetSelectedIndex()].GradeShowing;
			Point point=gridCriterion.SelectedCell;
			FillGridCriterion();
			gridCriterion.SetSelected(point);
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
			for(int i=0;i<_listEvaluationCriterions.Count;i++) {
				GradingScale gradingScale=_listGradingScales.Find(x=>x.GradingScaleNum==_listEvaluationCriterions[i].GradingScaleNum);
				if(gradingScale!=null && _gradingScale.ScaleType==gradingScale.ScaleType) {
					critCount++;
					gradeSum+=_listEvaluationCriterions[i].GradeNumber;
					maxPoints+=_listEvaluationCriterions[i].MaxPointsPoss;
				}
			}
			if(_gradingScale.ScaleType==EnumScaleType.PickList || _gradingScale.ScaleType==EnumScaleType.Percentage) {
				//Grades here are calculated as an average. All criterion in these modes will be worth the same "weight".
				//This could be improved later by adding weights to questions. This would mean they get 'averaged' into the grade
				//multiple times dependent on the weight. i.e. Criterion 1 has a weight of 3 and it gets graded an A.
				//The calculation will have 3 A's instead of just one for that criterion.
				if(critCount!=0) {
					gradeNumberDisplay=gradeSum/critCount;
				}
			}
			if(_gradingScale.ScaleType==EnumScaleType.Weighted) {
				//This grade is calculated as a sum of all criterion grade numbers over the maximum points possible all the criterion.
				gradeNumberDisplay=gradeSum;
			}
			if(_gradingScale.ScaleType==EnumScaleType.Percentage) {
				textGradeNumber.Text=gradeNumberDisplay.ToString();
				textGradeShowing.Text=gradeNumberDisplay.ToString();
			}
			if(_gradingScale.ScaleType==EnumScaleType.PickList){
				float dif=float.MaxValue;
				float closestNumber=0;
				string closestShowing="";
				for(int i=0;i<_listGradingScaleItems.Count;i++) {
					if(Math.Abs(_listGradingScaleItems[i].GradeNumber-gradeNumberDisplay) < dif) {
						dif=Math.Abs(_listGradingScaleItems[i].GradeNumber-gradeNumberDisplay);
						closestNumber=_listGradingScaleItems[i].GradeNumber;
						closestShowing=_listGradingScaleItems[i].GradeShowing;
					}
				}
				textGradeNumber.Text=closestNumber.ToString();
				textGradeShowing.Text=closestShowing;
			}
			if(_gradingScale.ScaleType==EnumScaleType.Weighted) {
				textGradeNumber.Text=gradeNumberDisplay.ToString();
				textGradeShowing.Text=gradeNumberDisplay+"/"+maxPoints;
			}
		}

		private void butStudentPicker_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick();
			formProviderPick.IsStudentPicker=true;
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult==DialogResult.OK) {
				_providerStudent=Providers.GetProv(formProviderPick.SelectedProvNum);
				_evaluation.StudentNum=_providerStudent.ProvNum;
				textStudent.Text=_providerStudent.GetLongDesc();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evaluation.IsNew || MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the evaluation.  Continue?")) {
				Evaluations.Delete(_evaluation.EvaluationNum);
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
			if(_providerStudent==null) {
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
				_evaluation.OverallGradeNumber=parsed;
			}
			for(int i=0;i<_listEvaluationCriterions.Count;i++) {
				_listEvaluationCriterions[i].Notes=gridCriterion.ListGridRows[i].Cells[4].Text;//Cell 4 is the notes column
				EvaluationCriterions.Update(_listEvaluationCriterions[i]);
			}
			_evaluation.DateEval=DateTime.Parse(textDate.Text);
			_evaluation.StudentNum=_providerStudent.ProvNum;
			_evaluation.OverallGradeShowing=textGradeShowing.Text;
			_evaluation.OverallGradeNumber=PIn.Float(textGradeNumber.Text);
			if(!String.IsNullOrWhiteSpace(textGradeShowingOverride.Text)) {
				_evaluation.OverallGradeShowing=textGradeShowingOverride.Text;
			}
			Evaluations.Update(_evaluation);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}