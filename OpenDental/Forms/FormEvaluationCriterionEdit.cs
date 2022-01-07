using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEvaluationCriterionEdit:FormODBase {
		//This window is currently not in use. All of the functions this window provides are now available directly from the EvaluationEdit window.
		//If there is ever more functionality added to EvaluationCriterion then this window may once again be necessary, but as of 06/24/2014 this window has been deprecated.
		private EvaluationCriterion _evalCritCur;
		private List<GradingScaleItem> _listGradeItems;
		private GradingScale _gradingScale;

		public FormEvaluationCriterionEdit(EvaluationCriterion evalCritCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evalCritCur=evalCritCur;
		}

		private void FormEvaluationCriterionEdit_Load(object sender,EventArgs e) {
			//There is always going to be an EvaluationCriterion when coming into this window.
			_gradingScale=GradingScales.GetOne(_evalCritCur.GradingScaleNum);
			textCriterionDescript.Text=_evalCritCur.CriterionDescript;
			textGradingScale.Text=_gradingScale.Description;
			textNote.Text=_evalCritCur.Notes;
			_listGradeItems=GradingScaleItems.Refresh(_evalCritCur.GradingScaleNum);
			//if(!_gradingScale.IsPercentage) {
			//	textGradeNumber.ReadOnly=true;
			//	textGradeShowingPercent.Visible=false;
			//	for(int i=0;i<_listGradeItems.Count;i++) {
			//		comboGradeShowing.Items.Add(_listGradeItems[i].GradeShowing);
			//		if(_listGradeItems[i].GradeShowing==_evalCritCur.GradeShowing) {
			//			comboGradeShowing.SelectedIndex=i;
			//			textGradeNumber.Text=_evalCritCur.GradeNumber.ToString();
			//		}
			//	}
			//}
			//else {
			//	comboGradeShowing.Visible=false;
			//	textGradeNumber.Text=_evalCritCur.GradeNumber.ToString();
			//	textGradeShowingPercent.Text=_evalCritCur.GradeShowing;
			//}
		}

		private void comboGradeNumber_SelectionChangeCommitted(object sender,EventArgs e) {
			_evalCritCur.GradeNumber=_listGradeItems[comboGradeShowing.SelectedIndex].GradeNumber;
			_evalCritCur.GradeShowing=_listGradeItems[comboGradeShowing.SelectedIndex].GradeShowing;
			textGradeNumber.Text=_evalCritCur.GradeNumber.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//float result;
			//if(_gradingScale.IsPercentage) {
			//	if(!float.TryParse(textGradeNumber.Text,out result)) {
			//		MsgBox.Show(this,"Grade number must be a valid percentage. Do not include '%' in the value.");
			//		return;
			//	}
			//	_evalCritCur.GradeNumber=result;
			//	_evalCritCur.GradeShowing=textGradeShowingPercent.Text;
			//}
			_evalCritCur.Notes=textNote.Text;
			EvaluationCriterions.Update(_evalCritCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}