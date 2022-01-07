using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormToothGridDefEdit:FormODBase {
		public DisplayField FieldCur;
		private Font headerFont=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);

		///<summary></summary>
		public FormToothGridDefEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFieldEdit_Load(object sender,EventArgs e) {
			listCellType.Items.AddEnums<ToothGridCellType>();
			listProcStatus.Items.AddEnums<ProcStat>();
		}

		private void butOK_Click(object sender, System.EventArgs e) {

			//if(textWidth.errorProvider1.GetError(textWidth)!="") {
			//  MsgBox.Show(this,"Please fix data entry errors first.");
			//  return;
			//}
			//FieldCur.Description=textDescription.Text;
			//FieldCur.ColumnWidth=PIn.Int(textWidth.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















