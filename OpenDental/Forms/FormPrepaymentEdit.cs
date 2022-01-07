using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPrepaymentEdit:FormODBase {
		public int CountCur;

		public FormPrepaymentEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPrepaymentEdit_Load(object sender,EventArgs e) {
			this.ActiveControl=textBox1;
			textBox1.Text=CountCur.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			CountCur=PIn.Int(textBox1.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}