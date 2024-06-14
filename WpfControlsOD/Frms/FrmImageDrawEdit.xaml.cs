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
	public partial class FrmImageDrawEdit:FrmODBase {
		public ImageDraw ImageDrawCur;
		/// <summary>Or, for a mount, the size of the mount.</summary>
		public Size SizeBitmap;
		///<summary>Same as zoom slider from parent.  Typical value is 100.</summary>
		public float ZoomVal;

		public FrmImageDrawEdit() {
			InitializeComponent();
			Load+=FrmImageDrawEdit_Load;
			textFontSize.LostFocus+=textFontSize_LostFocus;
			textFontApparent.LostFocus+=textFontApparent_LostFocus;
			PreviewKeyDown+=FrmImageDrawEdit_PreviewKeyDown;
		}

		private void FrmImageDrawEdit_Load(object sender, EventArgs e){
			Lang.F(this);
			textDrawText.Text=ImageDrawCur.GetTextString();
			Point pointLoc=new Point();
			pointLoc.X=ImageDrawCur.GetTextPoint().X;
			pointLoc.Y=ImageDrawCur.GetTextPoint().Y;
			textLocX.Text=pointLoc.X.ToString();
			textLocY.Text=pointLoc.Y.ToString();
			if(Programs.IsEnabled(ProgramName.Pearl)){
				groupBoxFont.IsEnabled=false;//show them the global pearl value, but don't let them change it.
			}
			textFontSize.Text=ImageDrawCur.GetFontSize().ToString("#.##");
			CalculateApparentFont();
			//ImageDrawCur.ColorBack can be transparent (0,255,255,255), which is transparent white.
			panelColor.ColorBack=ColorOD.ToWpf(ImageDrawCur.ColorDraw);
			if(ImageDrawCur.ColorBack.A==0){//transparent
				checkTransparent.Checked=true;
				panelColorBack.ColorBack=Colors.White;
			}
			else{
				panelColorBack.ColorBack=ColorOD.ToWpf(ImageDrawCur.ColorBack);
			}
		}

		private void butCalculateApparent_Click(object sender,EventArgs e) {
			//button in wpf doesn't steal focus, so LostFocus is not enough.
			CalculateApparentFont();
		}

		private void butCalculateActual_Click(object sender, EventArgs e){
			//button in wpf doesn't steal focus, so LostFocus is not enough.
			CalculateTrueFont();
		}

		private void textFontSize_LostFocus(object sender,RoutedEventArgs e) {
			CalculateApparentFont();
		}

		private void textFontApparent_LostFocus(object sender,RoutedEventArgs e) {
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
			float fontSize=fontSizeApparent/ZoomVal*100f*ScaleF(1);
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
			float fontSizeApparent=fontSize*ZoomVal/100f/ScaleF(1);
			//Example: ZoomVal=50%, so true apparent font will be smaller than true
			textFontApparent.Text=fontSizeApparent.ToString("#.##");
		}

		private void butColor_Click(object sender, EventArgs e){
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=panelColor.ColorBack;
			frmColorDialog.ShowDialog();
			panelColor.ColorBack=frmColorDialog.Color;
		}

		private void butColorBack_Click(object sender,EventArgs e) {
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=panelColorBack.ColorBack;
			frmColorDialog.ShowDialog();
			if(frmColorDialog.IsDialogCancel){
				//if Transparent was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			panelColorBack.ColorBack=frmColorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked==true){
				panelColorBack.ColorBack=Colors.White;//interpreted by user as transparent
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				//It would just remain white like it was before.
			}
		}

		private void butDelete_Click(object sender, EventArgs e){
			if(ImageDrawCur.IsNew){
				IsDialogCancel=true;
				return;
			}
			//Not necessary:
			//if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
			//	return;
			//}
			ImageDraws.Delete(ImageDrawCur.ImageDrawNum);
			//WasDeleted=true;
			IsDialogOK=true;
		}

		private void FrmImageDrawEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
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
			System.Drawing.Point point=new System.Drawing.Point();
			point.X=(int)pointLoc.X;
			point.Y=(int)pointLoc.Y;
			ImageDrawCur.SetLocAndText(point,textDrawText.Text);
			if(!Programs.IsEnabled(ProgramName.Pearl)){
				ImageDrawCur.FontSize=(float)textFontSize.Value;
			}
			ImageDrawCur.ColorDraw=ColorOD.FromWpf(panelColor.ColorBack);
			if(checkTransparent.Checked==true){
				ImageDrawCur.ColorBack=System.Drawing.Color.Transparent;
			}
			else{
				ImageDrawCur.ColorBack=ColorOD.FromWpf(panelColorBack.ColorBack);
			}
			if (ImageDrawCur.IsNew){
				ImageDraws.Insert(ImageDrawCur);
			}
			else{
				ImageDraws.Update(ImageDrawCur);
			}
			IsDialogOK=true;
		}

	}
}