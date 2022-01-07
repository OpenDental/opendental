using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcLockTool:FormODBase {
		public FormProcLockTool() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcLockTool_Load(object sender,EventArgs e) {
			textDate1.Text=DateTime.Today.AddDays(-3).ToShortDateString();
			textDate2.Text=DateTime.Today.AddDays(-1).ToShortDateString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(!textDate1.IsValid() || !textDate2.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime date1=PIn.Date(textDate1.Text);
			DateTime date2=PIn.Date(textDate2.Text);
			if(date1>date2) {
				MsgBox.Show(this,"Date 1 cannot be greater than Date 2.");
				return;
			}
			if(date1.AddDays(7) < date2) {
				if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
					MsgBox.Show(this,"Admin permission is required for date spans greater than 7 days.");
					return;
				}
			}
			Procedures.Lock(date1,date2);
			if(date1.AddDays(7) < date2) {
				SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Proc Lock Tool "+date1.ToShortDateString()+" - "+date2.ToShortDateString());
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Proc Lock Tool "+date1.ToShortDateString()+" - "+date2.ToShortDateString());
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	}
}