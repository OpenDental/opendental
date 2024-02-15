using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class FrmDocumentSize:FrmODBase {
		public Document DocumentCur;
		public System.Drawing.Size SizeRaw;

		public FrmDocumentSize() {
			InitializeComponent();
			Load+=FrmDocumentSize_Load;
			PreviewKeyDown+=FrmDocumentSize_PreviewKeyDown;
		}

		private void FrmDocumentSize_Load(object sender,EventArgs e) {
			Lang.F(this);
			textRawSize.Text=SizeRaw.Width.ToString()+" x "+SizeRaw.Height.ToString();
			checkIsFlipped.Checked=DocumentCur.IsFlipped;
			textDegreesRotated.Text=DocumentCur.DegreesRotated.ToString();
			if(DocumentCur.CropW>0){
				labelCropInfo.Text="(remove the existing crop)";
			}
			else{
				butCropReset.IsEnabled=false;
			}
			if(DocumentCur.IsFlipped || DocumentCur.DegreesRotated>0 || DocumentCur.CropW>0){
				labelResetAll.Text="(reset crop, flip, and rotation)";
			}
			else{
				butResetAll.IsEnabled=false;
			}
		}

		private void checkIsFlipped_Click(object sender,EventArgs e) {
			if(checkIsFlipped.Checked==true) {
				butResetAll.IsEnabled=true;
				labelResetAll.Text="(reset crop, flip, and rotation)";
			}
			else {
				butResetAll.IsEnabled=false;
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
			IsDialogOK=true;
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
			IsDialogOK=true;
		}

		private void FrmDocumentSize_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
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
				DocumentCur.IsFlipped=(checkIsFlipped.Checked==true);
				DocumentCur.DegreesRotated=degreesRotated;
				Documents.Update(DocumentCur,documentOld);
				IsDialogOK=true;
			}
			else {
				IsDialogOK=false;
			}
		}

	}
}