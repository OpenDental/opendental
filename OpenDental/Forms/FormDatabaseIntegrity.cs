using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Message box shown if the database was altered by a third party outside of Open Dental.
	///Set the TextWarningMessage to describe which entries are unsafe. </summary>
	public partial class FormDatabaseIntegrity:FormODBase {
		///<summary>Example: Patient</summary>
		public string ObjectDesc;

		public FormDatabaseIntegrity() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDatabaseIntegrity_Load(object sender,EventArgs e) {
			labelMessage.Text="This "+ObjectDesc+" was modified outside of Open Dental.";
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void linkLabel1_Click(object sender,EventArgs e) {
			Process.Start("https://www.opendental.com/site/integrity.html");
		}
	}
}