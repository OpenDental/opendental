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
	public partial class FormSecurityLock:FormODBase {

		///<summary></summary>
		public FormSecurityLock()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSecurityLock_Load(object sender,EventArgs e) {
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880){
				textDate.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textDays.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			checkAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
		}

		private void textDate_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e) {
			textDays.Text="";
		}

		private void textDays_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix error first.");
				return;
			}
			int days = 0;
			if(!int.TryParse(textDays.Text,out days) && !string.IsNullOrEmpty(textDays.Text)) {
				MsgBox.Show(this,"Invalid number of days.");
				return;
			}
			if(days<0) {
				days=0;
			}
			if(days>3650) {
				switch(MessageBox.Show("Lock days set to greater than ten years, would you like to disable lock days instead?","",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Cancel:
						return;
					case DialogResult.OK:
					case DialogResult.Yes:
						days=0;//disable instead of using large value.
						break;
					default:
						break;
				}
			}
			DateTime date=PIn.Date(textDate.Text);
			if(Prefs.UpdateString(PrefName.SecurityLockDate,POut.Date(date,false))
				| Prefs.UpdateInt(PrefName.SecurityLockDays,days)
				| Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkAdmin.Checked)  )
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















