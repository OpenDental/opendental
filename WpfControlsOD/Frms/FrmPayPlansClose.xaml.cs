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
	public partial class FrmPayPlansClose:FrmODBase {

		public FrmPayPlansClose() {
			InitializeComponent();
			Lang.F(this);
			PreviewKeyDown+=FrmPayPlansClose_PreviewKeyDown;
		}

		private void FrmPayPlansClose_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			long plansClosed=PayPlans.AutoClose(checkOldPaymentPlans.Checked==true,checkInsPaymentPlans.Checked==true);
			string msgText;
			if(plansClosed>0) {
				msgText=Lans.g(this,"Success.")+"  "+plansClosed+" "+Lans.g(this,"plan(s) closed.");
			}
			else {
				msgText=Lans.g(this,"There were no plans to close.");
			}
			MsgBox.Show(msgText);
			IsDialogOK=true;
		}

	}
}