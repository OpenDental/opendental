using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormBillingUndo : FormODBase {

		///<summary></summary>
		public FormBillingUndo()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBillingUndo_Load(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
		}

		private void butUndo_Click(object sender, System.EventArgs e) {
			//if(textDate.errorProvider1.GetError(textDate)!=""){
			//	MsgBox.Show(this,"Please fix data entry errors first.");
			//	return;
			//}
			//int rowsAffected=Commlogs.UndoStatements(PIn.PDate(textDate.Text));
			//MessageBox.Show(rowsAffected.ToString()+" "+Lan.g(this,"statement entries deleted."));
			//DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















