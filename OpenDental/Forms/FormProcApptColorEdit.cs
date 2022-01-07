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
		public ApptViewItem ApptVItem;

		///<summary></summary>
		public FormProcApptColorEdit()
		{
			//
			// Required for Windows Form Designer support
			//
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
			using ColorDialog colorDlg=new ColorDialog();
			colorDlg.AllowFullOpen=true;
			colorDlg.AnyColor=true;
			colorDlg.SolidColorOnly=false;
			colorDlg.Color=panelColor.BackColor;
			if(colorDlg.ShowDialog()==DialogResult.OK) {
				panelColor.BackColor=colorDlg.Color;
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
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
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





















