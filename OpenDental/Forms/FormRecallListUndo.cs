using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRecallListUndo:FormODBase {
		public FormRecallListUndo() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecallListUndo_Load(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
		}

		private void textDate_TextChanged(object sender,EventArgs e) {
			if(textDate.IsValid()) {
				int count=Commlogs.GetRecallUndoCount(PIn.Date(textDate.Text));
				labelCount.Text=count.ToString();
			}
			else {
				labelCount.Text="";
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date");
				return;
			}
			DateTime date=PIn.Date(textDate.Text);
			if(date < DateTime.Today.AddDays(-7)){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Date is from more than one week ago.  Continue anyway?")){
					return;
				}
			}
			if(MessageBox.Show("Delete all "+labelCount.Text+" commlog entries?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			Commlogs.RecallUndo(date);
			SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,0,"Recall list undo tool ran");
			MsgBox.Show(this,"Done");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}