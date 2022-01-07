using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAmountEdit:FormODBase {
		public decimal Amount;
		private string _text;

		public FormAmountEdit(string text) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_text=text;
		}

		private void FormAmountEdit_Load(object sender,EventArgs e) {
			labelText.Text=_text;
			textAmount.Text=POut.Decimal(Amount);
			textAmount.SelectionStart=0;
			textAmount.SelectionLength=textAmount.Text.Length;
		}

		private void butOK_Click(object sender,EventArgs e) {
			Amount=PIn.Decimal(textAmount.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}