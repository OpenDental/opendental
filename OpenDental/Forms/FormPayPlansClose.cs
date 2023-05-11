using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.DirectoryServices;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormPayPlansClose:FormODBase {

		public FormPayPlansClose() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender,EventArgs e) {
			long plansClosed=PayPlans.AutoClose(checkDynamic.Checked);
			string msgText;
			if(plansClosed>0) {
				msgText=Lan.g(this,"Success.")+"  "+plansClosed+" "+Lan.g(this,"plan(s) closed.");
			}
			else {
				msgText=Lan.g(this,"There were no plans to close.");
			}
			MessageBox.Show(msgText);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}