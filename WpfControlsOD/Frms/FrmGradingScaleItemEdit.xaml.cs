using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmGradingScaleItemEdit:FrmODBase {
		private GradingScaleItem _gradingScaleItem;

		public FrmGradingScaleItemEdit(GradingScaleItem gradingScaleItem) {
			InitializeComponent();
			//Lan.F(this);
			_gradingScaleItem=gradingScaleItem;
			Load+=FrmGradingScaleItemEdit_Load;
		}

		private void FrmGradingScaleItemEdit_Load(object sender,EventArgs e) {
			textGradeShowing.Text=_gradingScaleItem.GradeShowing;
			textGradeNumber.Text=_gradingScaleItem.GradeNumber.ToString();
			textDescription.Text=_gradingScaleItem.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_gradingScaleItem.IsNew) {
				IsDialogOK=false;
				return;
			}
			GradingScaleItems.Delete(_gradingScaleItem.GradingScaleItemNum);
			IsDialogOK=false;
		}

		private void butSave_Click(object sender,EventArgs e) {
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
			IsDialogOK=true;
		}



	}
}