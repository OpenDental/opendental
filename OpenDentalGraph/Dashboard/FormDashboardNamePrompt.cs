using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph {
	public partial class FormDashboardNamePrompt:Form {
		public delegate bool ValidateTabNameArgs(string tabName);
		private ValidateTabNameArgs _validateTabNameArgs;
		public string TabName {
			get { return textBoxTabName.Text; }
		}

		public FormDashboardNamePrompt(string tabName,ValidateTabNameArgs validateTabNameArgs) {
			InitializeComponent();
			textBoxTabName.Text=tabName;
			_validateTabNameArgs=validateTabNameArgs;
		}
		
		private void FormDashboardNamePrompt_FormClosing(object sender,FormClosingEventArgs e) {
			if(this.DialogResult!=DialogResult.OK) {
				return;
			}
			if(string.IsNullOrEmpty(TabName)) {
				MessageBox.Show("Tab Name is empty.");
				e.Cancel=true;
			}
			if(!_validateTabNameArgs(TabName)) {
				e.Cancel=true;
			}
		}
	}
}
