using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEvaluationCriterionDefEdit:FormODBase {
		private EvaluationCriterionDef _evaluationCriterionDef;
		private GradingScale _gradingScale;

		public FormEvaluationCriterionDefEdit(EvaluationCriterionDef evaluationCriterionDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_evaluationCriterionDef=evaluationCriterionDef;
		}

		private void FormEvaluationCriterionDefEdit_Load(object sender,EventArgs e) {
			textDescript.Text=_evaluationCriterionDef.CriterionDescript;
			_gradingScale=GradingScales.GetOne(_evaluationCriterionDef.GradingScaleNum);
			textGradeScaleName.Text=_gradingScale.Description;
			checkIsCategoryName.Checked=_evaluationCriterionDef.IsCategoryName;
			if(_gradingScale.ScaleType==EnumScaleType.Weighted) {
				textPoints.Visible=true;
				labelPoints.Visible=true;
				textPoints.Text=_evaluationCriterionDef.MaxPointsPoss.ToString();
			}
		}

		private void butGradingScale_Click(object sender,EventArgs e) {
			//Although there can be multiple grading scales on the same evaluation, it is highly discouraged. 
			//The grades must be manually calculated since the only calculated grades are scales that match the evaluation.
			//This could be changed later by forcing all scales to have point values and giving each EvaluationCriterion a "rubrick" gradingscale.
			//This change would require that Evaluations be given a different kind of grading scale that allowed for percentage ranges.
			//This would then be calculated into a grade for the reports that could use a similar grading scale.
			//These changes may not be necessary if the customers prefers the current method.
			using FormGradingScales formGradingScales=new FormGradingScales();
			formGradingScales.IsSelectionMode=true;
			formGradingScales.ShowDialog();
			if(formGradingScales.DialogResult==DialogResult.OK) {
				textGradeScaleName.Text=formGradingScales.GradingScaleSelected.Description;
				_gradingScale=formGradingScales.GradingScaleSelected;
				_evaluationCriterionDef.GradingScaleNum=_gradingScale.GradingScaleNum;
				if(formGradingScales.GradingScaleSelected.ScaleType==EnumScaleType.Weighted) {
					textPoints.Visible=true;
					labelPoints.Visible=true;
					textPoints.Text=_evaluationCriterionDef.MaxPointsPoss.ToString();
				}
				else {
					textPoints.Visible=false;
					labelPoints.Visible=false;
					textPoints.Text="";
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_evaluationCriterionDef.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the criterion def.  Continue?")) {
				EvaluationCriterionDefs.Delete(_evaluationCriterionDef.EvaluationCriterionDefNum);
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescript.Text=="") {
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			float points=0;
			if(_gradingScale.ScaleType==EnumScaleType.Weighted && !float.TryParse(textPoints.Text,out points)) {
				MsgBox.Show(this,"The specified point value is not a valid number.  Please input a valid number to save the criterion.");
				return;
			}
			_evaluationCriterionDef.CriterionDescript=textDescript.Text;
			_evaluationCriterionDef.IsCategoryName=checkIsCategoryName.Checked;
			_evaluationCriterionDef.MaxPointsPoss=points;
			if(_evaluationCriterionDef.IsNew) {
				EvaluationCriterionDefs.Insert(_evaluationCriterionDef);
			}
			else {
				EvaluationCriterionDefs.Update(_evaluationCriterionDef);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}