using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProcApptColorEdit:FormODBase {
		public ProcApptColor ProcApptColorCur;
		public ApptViewItem ApptViewItemCur;

		///<summary></summary>
		public FormProcApptColorEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcApptColorEdit_Load(object sender,System.EventArgs e) {
			checkPrevDate.Checked=ProcApptColorCur.ShowPreviousDate;
			textCodeRange.Text=ProcApptColorCur.CodeRange;
			if(!ProcApptColorCur.IsNew) {
				panelColor.BackColor=ProcApptColorCur.ColorText;
			}
			else { 
				panelColor.BackColor=System.Drawing.Color.Black; 
			}
		}

		private void butChange_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.AllowFullOpen=true;
			colorDialog.AnyColor=true;
			colorDialog.SolidColorOnly=false;
			colorDialog.Color=panelColor.BackColor;
			if(colorDialog.ShowDialog()==DialogResult.OK) {
				panelColor.BackColor=colorDialog.Color;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ProcApptColorCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this procedure color range?")) {
				return;
			}
			try {
				ProcApptColors.Delete(ProcApptColorCur.ProcApptColorNum);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textCodeRange.Text.Trim()=="") {
				MessageBox.Show(Lan.g(this,"Code range cannot be blank."));
				return;
			}
			ProcApptColorCur.ColorText=panelColor.BackColor;
			ProcApptColorCur.CodeRange=textCodeRange.Text;
			ProcApptColorCur.ShowPreviousDate=checkPrevDate.Checked;
			try {
				if(ProcApptColorCur.IsNew) {
					ProcApptColors.Insert(ProcApptColorCur);
				}
				else {
					ProcApptColors.Update(ProcApptColorCur);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}
}