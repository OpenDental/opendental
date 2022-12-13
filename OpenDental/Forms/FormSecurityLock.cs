using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Text;

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

		private string CreateSecurityLog(string textDateOld,string textDaysOld,int days,bool includesAdminOld) {
			StringBuilder stringBuilder=new StringBuilder();
			string textDateOldCopy=textDateOld;
			if(textDateOld=="01/01/0001") {
				textDateOldCopy="";
			}
			if(textDateOldCopy!=textDate.Text) {
				stringBuilder.AppendLine($"Global Lock Date changed from '{textDateOldCopy}' to '{textDate.Text}'");
			}
			if(textDaysOld!=days.ToString()) {
				stringBuilder.AppendLine($"Global Lock Days changed from '{textDaysOld}' to '{days}'");
			}
			if(includesAdminOld!=checkAdmin.Checked) {
				stringBuilder.AppendLine($"Lock includes administrators changed from '{includesAdminOld}' to '{checkAdmin.Checked}'");
			}
			return stringBuilder.ToString();
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
			//Get currently stored values for audit log.
			string textDateOld=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			string textDaysOld=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			bool includesAdminOld=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
			if(Prefs.UpdateString(PrefName.SecurityLockDate,POut.Date(date,false))
				| Prefs.UpdateInt(PrefName.SecurityLockDays,days)
				| Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkAdmin.Checked)  )
			{
				DataValid.SetInvalid(InvalidType.Prefs);
				string log=CreateSecurityLog(textDateOld,textDaysOld,days,includesAdminOld);
				SecurityLogs.MakeLogEntry(Permissions.SecurityGlobal,0,log.ToString());
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















