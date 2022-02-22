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
		public Size SizeRaw;

		public FormDocumentSize() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDocumentSize_Load(object sender,EventArgs e) {
			textRawSize.Text=SizeRaw.Width.ToString()+" x "+SizeRaw.Height.ToString();
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

		private void checkIsFlipped_CheckedChanged(object sender,EventArgs e) {
			if(checkIsFlipped.Checked) {
				butResetAll.Enabled=true;
				labelResetAll.Text="(reset crop, flip, and rotation)";
			}
			else {
				butResetAll.Enabled=false;
				labelResetAll.Text="(this image has no crop, flip, or rotate applied)";
			}
		}

		private void butCropReset_Click(object sender,EventArgs e) {
			Document documentOld=DocumentCur.Copy();
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.CropW=0;
			DocumentCur.CropH=0;
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

		private void butResetAll_Click(object sender,EventArgs e) {
			Document documentOld=DocumentCur.Copy();
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.CropW=0;
			DocumentCur.CropH=0;
			DocumentCur.DegreesRotated=0;
			DocumentCur.IsFlipped=false;
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			float degreesRotated=0;
			if(textDegreesRotated.Text!=""){
				try{
					degreesRotated=float.Parse(textDegreesRotated.Text);
				}
				catch{
					MsgBox.Show(this,"Invalid Degrees Rotated.");
					return;
				} 
			}
			if(DocumentCur.CropW>0
				&& DocumentCur.CropX!=0 && DocumentCur.CropY!=0)//but if cropped and centered,then we do allow rotate.
			{
				if(degreesRotated!=DocumentCur.DegreesRotated){
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
			}
			if(checkIsFlipped.Checked!=DocumentCur.IsFlipped 
				|| degreesRotated!=DocumentCur.DegreesRotated) 
			{
				Document documentOld=DocumentCur.Copy();
				DocumentCur.IsFlipped=checkIsFlipped.Checked;
				DocumentCur.DegreesRotated=degreesRotated;
				Documents.Update(DocumentCur,documentOld);
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