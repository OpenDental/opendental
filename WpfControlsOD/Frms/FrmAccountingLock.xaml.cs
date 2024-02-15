using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmAccountingLock : FrmODBase {

		///<summary></summary>
		public FrmAccountingLock()
		{
			InitializeComponent();
			Load+=FrmAccountingLock_Load;
			PreviewKeyDown+=FrmAccountingLock_PreviewKeyDown;
		}

		private void FrmAccountingLock_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(PrefC.GetDate(PrefName.AccountingLockDate).Year>1880){
				textVDate.Text=PrefC.GetDate(PrefName.AccountingLockDate).ToShortDateString();
			}
		}

		private void FrmAccountingLock_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textVDate.IsValid()) {
				MsgBox.Show(this,"Please fix error first.");
				return;
			}
			if(Prefs.UpdateString(PrefName.AccountingLockDate,POut.Date(PIn.Date(textVDate.Text),false))){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			IsDialogOK=true;
		}

	}
}