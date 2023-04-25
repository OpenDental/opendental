using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormImageScale:FormODBase {
		public float Pixels;
		public string StringUnits;
		public float ScaleVal;
		public int Decimals;

		public FormImageScale() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageScale_Load(object sender,EventArgs e) {
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

		private void textUnits_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
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

		private void butOK_Click(object sender,EventArgs e) {
			if(!textScale.IsValid()
				|| !textDecimals.IsValid())
			{
				MsgBox.Show(this,"Please fix errors first.");
				return;
			}
			ScaleVal=(float)textScale.Value;
			//if(ScaleVal<=0){
			//	MsgBox.Show(this,"Please enter a scale first.");
			//	return;
			//}
			StringUnits=textUnits.Text;
			Decimals=textDecimals.Value;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}