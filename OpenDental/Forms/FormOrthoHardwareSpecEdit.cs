using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoHardwareSpecEdit:FormODBase {
		public OrthoHardwareSpec OrthoHardwareSpecCur;

		public FormOrthoHardwareSpecEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoHardwareSpecEdit_Load(object sender,EventArgs e) {
			textType.Text=OrthoHardwareSpecCur.OrthoHardwareType.ToString();
			textDescription.Text=OrthoHardwareSpecCur.Description;
			butColor.BackColor=OrthoHardwareSpecCur.ItemColor;
			checkHidden.Checked=OrthoHardwareSpecCur.IsHidden;
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			//colorDialog.AllowFullOpen=false;//this would block them from even picking specific RGB values.
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				return;
			}
			butColor.BackColor=colorDialog.Color;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(OrthoHardwareSpecCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			try{
				OrthoHardwareSpecs.Delete(OrthoHardwareSpecCur.OrthoHardwareSpecNum);
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			OrthoHardwareSpecCur.Description=textDescription.Text;
			OrthoHardwareSpecCur.ItemColor=butColor.BackColor;
			OrthoHardwareSpecCur.IsHidden=checkHidden.Checked;
			if(OrthoHardwareSpecCur.IsNew){
				OrthoHardwareSpecs.Insert(OrthoHardwareSpecCur);
			}
			else{
				OrthoHardwareSpecs.Update(OrthoHardwareSpecCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}