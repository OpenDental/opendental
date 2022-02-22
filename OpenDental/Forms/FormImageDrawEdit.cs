using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormImageDrawEdit:FormODBase {
		public ImageDraw ImageDrawCur;
		/// <summary>Or, for a mount, the size of the mount.</summary>
		public Size SizeBitmap;
		///<summary>Same as zoom slider from parent.  Typical value is 100.</summary>
		public float ZoomVal;

		public FormImageDrawEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageDrawEdit_Load(object sender, EventArgs e){
			textDrawText.Text=ImageDrawCur.GetTextString();
			Point pointLoc=ImageDrawCur.GetTextPoint();
			textLocX.Text=pointLoc.X.ToString();
			textLocY.Text=pointLoc.Y.ToString();
			textFontSize.Text=ImageDrawCur.FontSize.ToString("#.##");
			CalculateApparentFont();
			//butColor.BackColor=Color.White;//255,255,255,255
			//butColor.BackColor=Color.Black;//255,0,0,0
			//butColor.BackColor=Color.Empty;//0,0,0,0 , but this "null" actually makes the color inherit
			//butColor.BackColor=Color.Transparent;//0,255,255,255
			butColor.BackColor=ImageDrawCur.ColorDraw;
			if(ImageDrawCur.ColorBack.ToArgb()==Color.Transparent.ToArgb()){
				checkTransparent.Checked=true;
				butColorBack.BackColor=Color.White;
			}
			else{
				butColorBack.BackColor=ImageDrawCur.ColorBack;
			}
		}

		private void butCalculateApparent_Click(object sender,EventArgs e) {
			//this button doesn't actually do anything.  It gives the user a spot to click, but the real trigger is leaving the textbox and validating.
		}

		private void butCalculateActual_Click(object sender, EventArgs e){
			//this button doesn't actually do anything.  It gives the user a spot to click, but the real trigger is leaving the textbox and validating.
		}

		private void textFontSize_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
			CalculateApparentFont();
		}

		private void textFontApparent_Validating(object sender, System.ComponentModel.CancelEventArgs e){
			CalculateTrueFont();
		}

		private void CalculateTrueFont(){
			//This same math is used when creating a new text object outside this window.
			if(!textFontApparent.IsValid()){
				return;
			}
			if(textFontApparent.Text==""){
				return;
			}
			float fontSizeApparent=(float)textFontApparent.Value;
			//Example: ZoomVal=50%, so true fontSize will be bigger than apparent
			float fontSize=fontSizeApparent/ZoomVal*100f*LayoutManager.ScaleMy();
			textFontSize.Text=fontSize.ToString("#.##");
		}

		private void CalculateApparentFont(){
			if(!textFontSize.IsValid()){
				return;
			}
			if(textFontSize.Text==""){
				return;
			}
			float fontSize=(float)textFontSize.Value;
			float fontSizeApparent=fontSize*ZoomVal/100f/LayoutManager.ScaleMy();
			//Example: ZoomVal=50%, so true apparent font will be smaller than true
			textFontApparent.Text=fontSizeApparent.ToString("#.##");
		}

		private void butColor_Click(object sender, EventArgs e){
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butColorBack_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColorBack.BackColor;
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				//if None was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorBack.BackColor=colorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				butColorBack.BackColor=Color.Transparent;//looks like white. We could just as easily use white
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorBack.BackColor=Color.White;
			}
		}

		private void butDelete_Click(object sender, EventArgs e){
			if(ImageDrawCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			//Not necessary:
			//if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
			//	return;
			//}
			ImageDraws.Delete(ImageDrawCur.ImageDrawNum);
			//WasDeleted=true;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDrawText.Text==""){
				MsgBox.Show(this,"Text is required.");
				return;
			}
			if(!textLocX.IsValid() 
				|| !textLocY.IsValid() 
				|| !textFontSize.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
//todo: make sure x and y are within image bounds
//todo: test empty strings in x and y
			Point pointLoc=new Point(textLocX.Value,textLocY.Value);
			ImageDrawCur.SetLocAndText(pointLoc,textDrawText.Text);
			ImageDrawCur.FontSize=(float)textFontSize.Value;
			ImageDrawCur.ColorDraw=butColor.BackColor;
			if(checkTransparent.Checked){
				ImageDrawCur.ColorBack=Color.Transparent;
			}
			else{
				ImageDrawCur.ColorBack=butColorBack.BackColor;
			}
			if (ImageDrawCur.IsNew){
				ImageDraws.Insert(ImageDrawCur);
			}
			else{
				ImageDraws.Update(ImageDrawCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}