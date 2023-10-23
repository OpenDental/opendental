using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfControls.UI {
//This doesn't work, but I'm leaving it here anyway

	public partial class FormLongStringEditor:Form {
		public FormLongStringEditor() {
			InitializeComponent();
		}

		public string Value{get;set; }

		private void FormLongStringEditor_Load(object sender,EventArgs e) {
			textBox.Text=Value;
		}

		private void butSave_Click(object sender,EventArgs e) {
			Value=textBox.Text;
			DialogResult=DialogResult.OK;
			Close();
		}
	}
}
