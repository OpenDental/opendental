using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEvaluationCriterionDefEdit:FormODBase {
		EvaluationCriterionDef _evalCritDef;
		GradingScale _gradeScale;

		public FormEvaluationCriterionDefEdit(EvaluationCriterionDef evalCritDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evalCritDef=evalCritDef;
		}

		private void FormEvaluationCriterionDefEdit_Load(object sender,EventArgs e) {
			textDescript.Text=_evalCritDef.CriterionDescript;
			_gradeScale=GradingScales.GetOne(_evalCritDef.GradingScaleNum);
			textGradeScaleName.Text=_gradeScale.Description;
			checkIsCategoryName.Checked=_evalCritDef.IsCategoryName;
			if(_gradeScale.ScaleType==EnumScaleType.Weighted) {
				textPoints.Visible=true;
				labelPoints.Visible=true;
				textPoints.Text=_evalCritDef.MaxPointsPoss.ToString();
			}
		}

		private void butGradingScale_Click(object sender,EventArgs e) {
			//Although there can be multiple grading scales on the same evaluation, it is highly discouraged. 
			//The grades must be manually calculated since the only calculated grades are scales that match the evaluation.
			//This could be changed later by forcing all scales to have point values and giving each EvaluationCriterion a "rubrick" gradingscale.
			//This change would require that Evaluations be given a different kind of grading scale that allowed for percentage ranges.
			//This would then be calculated into a grade for the reports that could use a similar grading scale.
			//These changes may not be necessary if the customers prefers the current method.
			using FormGradingScales FormGS=new FormGradingScales();
			FormGS.IsSelectionMode=true;
			FormGS.ShowDialog();
			if(FormGS.DialogResult==DialogResult.OK) {
				textGradeScaleName.Text=FormGS.SelectedGradingScale.Description;
				_gradeScale=FormGS.SelectedGradingScale;
				_evalCritDef.GradingScaleNum=_gradeScale.GradingScaleNum;
				if(FormGS.SelectedGradingScale.ScaleType==EnumScaleType.Weighted) {
					textPoints.Visible=true;
					labelPoints.Visible=true;
					textPoints.Text=_evalCritDef.MaxPointsPoss.ToString();
				}
				else {
					textPoints.Visible=false;
					labelPoints.Visible=false;
					textPoints.Text="";
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evalCritDef.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the criterion def.  Continue?")) {
				EvaluationCriterionDefs.Delete(_evalCritDef.EvaluationCriterionDefNum);
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescript.Text=="") {
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			float points=0;
			if(_gradeScale.ScaleType==EnumScaleType.Weighted && !float.TryParse(textPoints.Text,out points)) {
				MsgBox.Show(this,"The specified point value is not a valid number.  Please input a valid number to save the criterion.");
				return;
			}
			_evalCritDef.CriterionDescript=textDescript.Text;
			_evalCritDef.IsCategoryName=checkIsCategoryName.Checked;
			_evalCritDef.MaxPointsPoss=points;
			if(_evalCritDef.IsNew) {
				EvaluationCriterionDefs.Insert(_evalCritDef);
			}
			else {
				EvaluationCriterionDefs.Update(_evalCritDef);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}