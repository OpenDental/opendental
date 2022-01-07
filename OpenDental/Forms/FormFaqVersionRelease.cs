using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFaqVersionRelease:FormODBase {

		public FormFaqVersionRelease() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textManualVersion.Text.Contains(".")) {
				MessageBox.Show(this,"Please enter a valid manual version. For example, 19.1 should be entered as '191'.");
				return;
			}
			int newVersion=PIn.Int(textManualVersion.Text);
			if(!Faqs.PageForVersionExists(newVersion)) {
				MessageBox.Show($"There are no manual pages for version {newVersion}. Please run this tool after the manual publisher has released version {newVersion}");
				return;
			}
			if(MessageBox.Show($"You are about to create FAQ's for {newVersion}. Continue?","",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			Faqs.CreateFaqsForNewVersion(newVersion);
			DialogResult=DialogResult.OK;
		}


		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
