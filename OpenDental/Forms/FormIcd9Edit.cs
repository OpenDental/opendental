using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>This window cannot even be accessed any longer.</summary>
	public partial class FormIcd9Edit:FormODBase {
		private ICD9 _iCD9;
		public bool IsNew;

		public FormIcd9Edit(ICD9 icd9) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_iCD9=icd9;
		}

		private void FormIcd9Edit_Load(object sender,EventArgs e) {
			if(!IsNew) {
				textCode.Enabled=false;
			}
			textCode.Text=_iCD9.ICD9Code;
			textDescription.Text=_iCD9.Description;
		}

		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try {
				ICD9s.Delete(_iCD9.ICD9Num);
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			_iCD9.ICD9Code=textCode.Text;
			_iCD9.Description=textDescription.Text;
			if(IsNew) {//Used the "+Add" button to open this form.
				if(ICD9s.CodeExists(_iCD9.ICD9Code)) {//Must enter a unique code.
					MsgBox.Show(this,"You must choose a unique code.");
					return;
				}
				ICD9s.Insert(_iCD9);
			}
			else {
				ICD9s.Update(_iCD9);
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}