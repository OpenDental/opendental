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
	/// <summary></summary>
	public partial class FrmFont : FrmODBase {
		///<summary>0 represents third mixed state. Valid values are between 50 and 300. If we leave it 0, then we are indicating that we don't want to change it.</summary>
		public int FontScale=0;
		public bool IsEmpty;
		public Color ColorText;
		public Color? ColorBack;

		///<summary></summary>
		public FrmFont() {
			InitializeComponent();
			Load+=Frm_Load;
			checkNone.Click+=CheckNone_Click;
		}

		private void Frm_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(!IsEmpty){
				labelWarning.Visible=false;
			}
			textVIntFontScale.Value=FontScale;
			panelColor.ColorBack=ColorText;
			if(ColorBack is null){
				panelColorBack.ColorBack=Colors.Transparent;
				checkNone.Checked=true;
			}
			else{
				panelColorBack.ColorBack=ColorBack.Value;
			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			FrmColorDialog colorDialog=new FrmColorDialog();
			colorDialog.Color=panelColor.ColorBack;
			colorDialog.ShowDialog();
			panelColor.ColorBack=colorDialog.Color;
		}

		private void butColorBack_Click(object sender,EventArgs e) {
			FrmColorDialog colorDialog=new FrmColorDialog();
			colorDialog.Color=panelColorBack.ColorBack;
			colorDialog.ShowDialog();
			if(colorDialog.IsDialogCancel){
				return;
			}
			panelColorBack.ColorBack=colorDialog.Color;
			if(colorDialog.Color==Colors.Transparent) {//not sure if this is even possible
				checkNone.Checked=true;
			}
			else{
				checkNone.Checked=false;
			}
		}

		private void CheckNone_Click(object sender,EventArgs e) {
			if(checkNone.Checked==true){
				panelColorBack.ColorBack=Colors.Transparent;
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntFontScale.IsValid()){
				MsgBox.Show("Please fix invalid values first.");
				return;
			}
			if(textVIntFontScale.Value !=0 && textVIntFontScale.Value<50){
				MsgBox.Show("Font scale cannot be less than 50.");
			}
			FontScale=textVIntFontScale.Value;
			if(FontScale==0){
				FontScale=100;
			}
			ColorText=panelColor.ColorBack;
			if(checkNone.Checked==true){
				ColorBack=null;
			}
			else{
				ColorBack=panelColorBack.ColorBack;
			}
			IsDialogOK=true;
		}
	}
}