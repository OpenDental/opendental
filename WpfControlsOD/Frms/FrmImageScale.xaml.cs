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
	public partial class FrmImageScale:FrmODBase {
		public float Pixels;
		public string StringUnits;
		public float ScaleVal;
		public int Decimals;

		public FrmImageScale() {
			InitializeComponent();
			Load+=FrmImageScale_Load;
			textUnits.TextChanged+=TextUnits_TextChanged;
			PreviewKeyDown+=FrmImageScale_PreviewKeyDown;
		}

		private void FrmImageScale_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(Pixels==0 && ScaleVal==0){
				labelInstructions.Visible=true;
			}
			else{
				labelInstructions.Visible=false;
			}
			textUnits.Text=StringUnits;
			//known length is blank
			if(Pixels>0){
				textPixels.Text=Pixels.ToString();
			}
			if(ScaleVal>0){
				textScale.Text=ScaleVal.ToString();
			}
			textDecimals.Value=Decimals;
			FillUnits();
		}

		private void TextUnits_TextChanged(object sender,EventArgs e) {
			FillUnits();
		}

		private void FillUnits(){
			labelUnits.Text=textUnits.Text;
			labelScale.Text="Pixels per "+textUnits.Text;
		}

		private void butCalculate_Click(object sender,EventArgs e) {
			float lengthKnown=0;
			try{
				lengthKnown=PIn.Float(textKnownLength.Text);
			}
			catch{
				MsgBox.Show(this,"Please fix known length.");
				return;
			}
			if(Pixels==0){
				MsgBox.Show(this,"Please click on a line of known length first.");
				return;
			}
			if(lengthKnown==0) {
				MsgBox.Show("Please enter a known length greater than zero first.");
				return;
			}
			float scale=Pixels/lengthKnown;
			textScale.Text=scale.ToString();
		}

		private void FrmImageScale_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textScale.IsValid()
				|| !textDecimals.IsValid())
			{
				MsgBox.Show(this,"Please fix errors first.");
				return;
			}
			ScaleVal=(float)textScale.Value;
			StringUnits=textUnits.Text;
			Decimals=textDecimals.Value;
			IsDialogOK=true;
		}

	}
}