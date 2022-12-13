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
		///<summary>This can be quite long.</summary>
		public string MessageToShow;
		///<summary>So that it shows a different link.</summary>
		public bool IsPlugin;

		public FormDatabaseIntegrity() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDatabaseIntegrity_Load(object sender,EventArgs e) {
			labelMessage.Text=MessageToShow;
			if(IsPlugin){
				linkLabel.Text="https://www.opendental.com/site/plugins.html";
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void linkLabel1_Click(object sender,EventArgs e) {
			if(IsPlugin){
				Process.Start("https://www.opendental.com/site/plugins.html");
			}
			else{
				Process.Start("https://www.opendental.com/site/integrity.html");
			}
		}
	}
}