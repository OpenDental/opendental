using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormAccountingLock : FormODBase {

		///<summary></summary>
		public FormAccountingLock()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountingLock_Load(object sender,EventArgs e) {
			if(PrefC.GetDate(PrefName.AccountingLockDate).Year>1880){
				textDate.Text=PrefC.GetDate(PrefName.AccountingLockDate).ToShortDateString();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix error first.");
				return;
			}
			if(Prefs.UpdateString(PrefName.AccountingLockDate,POut.Date(PIn.Date(textDate.Text),false))){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















