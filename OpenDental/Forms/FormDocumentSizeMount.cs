using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDocumentSizeMount:FormODBase {
		public Document DocumentCur;
		public Size SizeRaw;
		public Size SizeMount;
		private double _zoomInitial;
		///<summary>This is the number calculated from what the user typed here.</summary>
		private int _degreesRotated;

		public FormDocumentSizeMount() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDocumentSizeMount_Load(object sender, EventArgs e){
			textRawSize.Text=SizeRaw.Width.ToString()+" x "+SizeRaw.Height.ToString();
			textMountSize.Text=SizeMount.Width.ToString()+" x "+SizeMount.Height.ToString();
			checkIsFlipped.Checked=DocumentCur.IsFlipped;
			textDegreesRotated.Text=DocumentCur.DegreesRotated.ToString();
			Size sizeCrop=new Size(DocumentCur.CropW,DocumentCur.CropH);
			_zoomInitial=UI.ImageTools.CalcZoomMount(SizeMount,SizeRaw,sizeCrop,DocumentCur.DegreesRotated);
			_zoomInitial=Math.Round(_zoomInitial,1);
			textZoomFit.Text=_zoomInitial.ToString();
			//double zoomOrig=(double)SizeMount.Width/SizeRaw.Width*100;
			double zoomOrig;
			//this math immediately switches to perspective of image, not mount.
			double widthMount=SizeMount.Width;
			double heightMount=SizeMount.Height;
			if(DocumentCur.DegreesRotated==90 || DocumentCur.DegreesRotated==270){
				widthMount=SizeMount.Height;
				heightMount=SizeMount.Width;
			}
			if(DocumentCur.CropW==0){	
				double ratioCropWtoH=widthMount/heightMount;
				bool isWide=false;
				if((double)SizeRaw.Width/SizeRaw.Height > ratioCropWtoH){
					isWide=true;
				}
				if(isWide){
					zoomOrig=widthMount/SizeRaw.Width*100;
				}
				else{
					zoomOrig=heightMount/SizeRaw.Height*100;
				}
			}
			else{
				zoomOrig=widthMount/DocumentCur.CropW*100;
				//we always assume CropH is wrong anyway, so no need to do additional checks for rotation.
			}
			zoomOrig=Math.Round(zoomOrig,1);
			textZoomOrig.Text=zoomOrig.ToString();
		}

		private void but100_Click(object sender, EventArgs e){
			if(!ValidateDegreesRotated()){
				return;
			}
			Document documentOld=DocumentCur.Copy();
			if(_degreesRotated==0 || _degreesRotated==180){
				DocumentCur.CropW=SizeMount.Width;
				DocumentCur.CropH=SizeMount.Height;
			}
			else{
				DocumentCur.CropW=SizeMount.Height;
				DocumentCur.CropH=SizeMount.Width;
			}
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.IsFlipped=checkIsFlipped.Checked;
			DocumentCur.DegreesRotated=_degreesRotated;
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

		private void butFit_Click(object sender, EventArgs e){
			if(!ValidateDegreesRotated()){
				return;
			}
			Document documentOld=DocumentCur.Copy();
			DocumentCur.CropW=0;
			DocumentCur.CropH=0;
			DocumentCur.CropX=0;
			DocumentCur.CropY=0;
			DocumentCur.IsFlipped=checkIsFlipped.Checked;
			DocumentCur.DegreesRotated=_degreesRotated;
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

		private void butExpandFill_Click(object sender, EventArgs e){
			if(!ValidateDegreesRotated()){
				return;
			}
			Rectangle rectangle=UI.ImageTools.CalcExpandToFill(SizeMount,SizeRaw,_degreesRotated);
			Document documentOld=DocumentCur.Copy();
			DocumentCur.CropW=rectangle.Width;
			DocumentCur.CropH=rectangle.Height;
			DocumentCur.CropX=rectangle.X;
			DocumentCur.CropY=rectangle.Y;
			DocumentCur.IsFlipped=checkIsFlipped.Checked;
			DocumentCur.DegreesRotated=_degreesRotated;
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

		///<summary>returns false if validation fails.</summary>
		private bool ValidateDegreesRotated(){
			_degreesRotated=0;
			if(textDegreesRotated.Text==""){
				return true;
			}
			try{
				_degreesRotated=int.Parse(textDegreesRotated.Text);
			}
			catch{
				MsgBox.Show(this,"Invalid Degrees Rotated.");
				return false;
			}
			return true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidateDegreesRotated()){
				return;
			}
			double zoom;
			try{
				zoom=double.Parse(textZoomFit.Text);
			}
			catch{
				MsgBox.Show(this,"Invalid Zoom.");
				return;
			}
			if(zoom<1){
			MsgBox.Show(this,"Zoom must be at least 1, and typically closer to 100.");
			return;
			}
			if(checkIsFlipped.Checked==DocumentCur.IsFlipped
				&& _degreesRotated==DocumentCur.DegreesRotated
				&& _zoomInitial==zoom)
			{
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(DocumentCur.CropW>0
				&& DocumentCur.CropX!=0 && DocumentCur.CropY!=0)//but if cropped and centered,then we do allow rotate.
			{
				if(_degreesRotated!=DocumentCur.DegreesRotated){
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
			}
			Document documentOld=DocumentCur.Copy();
			DocumentCur.IsFlipped=checkIsFlipped.Checked;
			if(_zoomInitial!=zoom || DocumentCur.DegreesRotated!=_degreesRotated){
				Size sizeNew=UI.ImageTools.CalcZoomToSize(SizeMount,SizeRaw,zoom,_degreesRotated);
				DocumentCur.CropW=sizeNew.Width;
				DocumentCur.CropH=sizeNew.Height;
			}
			DocumentCur.DegreesRotated=_degreesRotated;//has to come after the check above
			Documents.Update(DocumentCur,documentOld);
			DialogResult=DialogResult.OK;
		}

	}
}