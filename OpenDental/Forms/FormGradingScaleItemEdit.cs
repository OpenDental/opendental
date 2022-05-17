using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormGradingScaleItemEdit:FormODBase {
		private GradingScaleItem _gradingScaleItem;

		public FormGradingScaleItemEdit(GradingScaleItem gradingScaleItem) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_gradingScaleItem=gradingScaleItem;
		}

		private void FormGradingScaleItemEdit_Load(object sender,EventArgs e) {
			textGradeShowing.Text=_gradingScaleItem.GradeShowing;
			textGradeNumber.Text=_gradingScaleItem.GradeNumber.ToString();
			textDescription.Text=_gradingScaleItem.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_gradingScaleItem.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			GradingScaleItems.Delete(_gradingScaleItem.GradingScaleItemNum);
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
			_gradingScaleItem.GradeNumber=gradeNumber;
			_gradingScaleItem.GradeShowing=textGradeShowing.Text;
			if(textGradeShowing.Text=="") {
				_gradingScaleItem.GradeShowing=gradeNumber.ToString();
			}
			_gradingScaleItem.Description=textDescription.Text;
			if(_gradingScaleItem.IsNew) {
				GradingScaleItems.Insert(_gradingScaleItem);
			}
			else {
				GradingScaleItems.Update(_gradingScaleItem);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}