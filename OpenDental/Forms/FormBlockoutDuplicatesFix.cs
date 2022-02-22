using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBlockoutDuplicatesFix:FormODBase {
		public FormBlockoutDuplicatesFix() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBlockoutDuplicatesFix_Load(object sender,EventArgs e) {
			FillLabels();
			Cursor=Cursors.Default;
		}

		private void FillLabels() {
			labelCount.Text=Schedules.GetDuplicateBlockoutCount().ToString();
			if(labelCount.Text=="0") {
				labelInstructions.Text="";
			}
			else {
				labelInstructions.Text=Lan.g(this,"Click the Clear button to fix the duplicates.");
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(labelCount.Text=="0") {
				MsgBox.Show(this,"There are no duplicates to clear.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all duplicates?")){
				return;
			}
			Cursor=Cursors.WaitCursor;
			Schedules.ClearDuplicates();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Clear duplicate blockouts.");
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
			FillLabels();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}