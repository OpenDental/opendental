using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDocumentSize:FormODBase {
		public Document DocumentCur;
		///<summary>This is the number calculated from what the user typed here and compare on OK_Click.</summary>
		private int _degreesRotated;
		///<summary>Check if the CheckIsFlipped has been changed on OK_Click.</summary>
		private bool _isFlipped;

		public FormDocumentSize() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDocumentSize_Load(object sender,EventArgs e) {
			_isFlipped=DocumentCur.IsFlipped;
			_degreesRotated=DocumentCur.DegreesRotated;
			checkIsFlipped.Checked=DocumentCur.IsFlipped;
			textDegreesRotated.Text=DocumentCur.DegreesRotated.ToString();
			if(DocumentCur.CropW>0){
				labelCropInfo.Text="(remove the existing crop)";
			}
			else{
				butCropReset.Enabled=false;
			}
			if(DocumentCur.IsFlipped || DocumentCur.DegreesRotated>0 || DocumentCur.CropW>0){
				labelResetAll.Text="(reset crop, flip, and rotation)";
			}
			else{
				butResetAll.Enabled=false;
			}
		}

		///<summary>Returns false if validation fails.</summary>
		private bool ValidateDegreesRotated(){
			_degreesRotated=0;
			if(textDegreesRotated.Text!=""){
				try{
					_degreesRotated=int.Parse(textDegreesRotated.Text);
					if(_degreesRotated!=0 && _degreesRotated!=90 && _degreesRotated!=180 && _degreesRotated!=270){
						MsgBox.Show(this,"Invalid Degrees Rotated.");
						return false;
					}
				}
				catch{
					MsgBox.Show(this,"Invalid Degrees Rotated.");
					return false;
				}
			}
			return true;
		}

		private void checkIsFlipped_CheckedChanged(object sender,EventArgs e) {
			if(checkIsFlipped.Checked==false) {
				butResetAll.Enabled=false;
				labelResetAll.Text="(this image has no crop, flip, or rotate applied)";
			}
			else {
				butResetAll.Enabled=true;
				labelResetAll.Text="(reset crop, flip, and rotation)";
			}
			_isFlipped=checkIsFlipped.Checked;
		}

		private void butCropReset_Click(object sender,EventArgs e) {
			Document docOld=DocumentCur.Copy();
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.CropW=0;
			DocumentCur.CropH=0;
			Documents.Update(DocumentCur,docOld);
			DialogResult=DialogResult.OK;
		}

		private void butResetAll_Click(object sender,EventArgs e) {
			Document docOld=DocumentCur.Copy();
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.CropW=0;
			DocumentCur.CropH=0;
			DocumentCur.DegreesRotated=0;
			DocumentCur.IsFlipped=false;
			Documents.Update(DocumentCur,docOld);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateDegreesRotated()) {
				return;
			}
			if(_isFlipped!=DocumentCur.IsFlipped 
				|| _degreesRotated!=DocumentCur.DegreesRotated) 
			{
				Document docOld=DocumentCur.Copy();
				DocumentCur.IsFlipped=checkIsFlipped.Checked;
				DocumentCur.DegreesRotated=PIn.Int(textDegreesRotated.Text);
				Documents.Update(DocumentCur,docOld);
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}