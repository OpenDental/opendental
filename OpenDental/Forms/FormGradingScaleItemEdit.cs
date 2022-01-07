using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormGradingScaleItemEdit:FormODBase {
		private GradingScaleItem _gradingScaleItemCur;

		public FormGradingScaleItemEdit(GradingScaleItem gradingScaleItemCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_gradingScaleItemCur=gradingScaleItemCur;
		}

		private void FormGradingScaleItemEdit_Load(object sender,EventArgs e) {
			textGradeShowing.Text=_gradingScaleItemCur.GradeShowing;
			textGradeNumber.Text=_gradingScaleItemCur.GradeNumber.ToString();
			textDescription.Text=_gradingScaleItemCur.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_gradingScaleItemCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			GradingScaleItems.Delete(_gradingScaleItemCur.GradingScaleItemNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textGradeNumber.Text=="") {
				MsgBox.Show(this,"Grade Number is a required field and cannot be empty.");
				return;
			}
			float gradeNumber=0;//Just a placeholder
			if(!float.TryParse(textGradeNumber.Text,out gradeNumber)) {//Fills gradeNumber
				MsgBox.Show(this,"Grade Number is not in a valid format. Please type in a number.");
				return;
			}
			
			_gradingScaleItemCur.GradeNumber=gradeNumber;
			_gradingScaleItemCur.GradeShowing=textGradeShowing.Text;
			if(textGradeShowing.Text=="") {
				_gradingScaleItemCur.GradeShowing=gradeNumber.ToString();
			}
			_gradingScaleItemCur.Description=textDescription.Text;
			if(_gradingScaleItemCur.IsNew) {
				GradingScaleItems.Insert(_gradingScaleItemCur);
			}
			else {
				GradingScaleItems.Update(_gradingScaleItemCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}