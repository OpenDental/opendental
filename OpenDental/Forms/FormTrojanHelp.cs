using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTrojanHelp : FormODBase {

		///<summary></summary>
		public FormTrojanHelp()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormTrojanHelp_Load(object sender,EventArgs e) {
			textMain.Select(0,31);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(323,20);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(571,5);//skip
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(933,18);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(1302,31);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(1473,10);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
		}


	}
}





















