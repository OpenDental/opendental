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
			//Lan.F(this);
		}

		private void butSave_Click(object sender,EventArgs e) {
			long plansClosed=PayPlans.AutoClose(canIncludeOldPaymentPlans:(bool)checkOldPaymentPlans.Checked);
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