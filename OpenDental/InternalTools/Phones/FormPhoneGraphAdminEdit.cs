using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPhoneGraphAdminEdit:FormODBase {
		public PhoneGraph PhoneGraphCur;

		public FormPhoneGraphAdminEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormPhoneGraphAdmin_Load(object sender,System.EventArgs e) {
			textDateEntry.Text=PhoneGraphCur.DateEntry.ToShortDateString();
			textNote.Text=PhoneGraphCur.Note;
			textMax.Text=PhoneGraphCur.DailyLimit.ToString();
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(PhoneGraphCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			PhoneGraphs.Delete(PhoneGraphCur.PhoneGraphNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			//Date can't be changed
			PhoneGraphCur.Note=textNote.Text;
			int dailyLimit=0;
			try{
				dailyLimit=PIn.Int(textMax.Text);
			}
			catch{
				MsgBox.Show("Please fix entry first.");
				return;
			}
			PhoneGraphCur.DailyLimit=dailyLimit;
			PhoneGraphs.InsertOrUpdate(PhoneGraphCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			this.DialogResult=DialogResult.Cancel;
		}

	
	}
}
