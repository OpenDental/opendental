using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormInsSpanEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private CovSpan CovSpanCur;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsSpanEditCanada";
			}
			return "FormInsSpanEdit";
		}

		///<summary></summary>
		public FormInsSpanEdit(CovSpan covSpanCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			CovSpanCur=covSpanCur.Copy();
			Lan.F(this);
		}

		private void FormInsSpanEdit_Load(object sender, System.EventArgs e) {
			textFrom.Text=CovSpanCur.FromCode;
			textTo.Text=CovSpanCur.ToCode;

		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US"){
				//if not match to D****
				if(!Regex.IsMatch(textFrom.Text,@"^D\w{4}$") || !Regex.IsMatch(textTo.Text,@"^D\w{4}$")){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"One of the codes is not a standard ADA code.  Use anyway?")){
						return;
					}
				}
			}
			CovSpanCur.FromCode=textFrom.Text;
			CovSpanCur.ToCode=textTo.Text;
			try{
				if(IsNew) {
					CovSpans.Insert(CovSpanCur);
				}
				else {
					CovSpans.Update(CovSpanCur);
				}
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				CovSpans.Delete(CovSpanCur);
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}
