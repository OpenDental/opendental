using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using OpenDental;

namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizIntro:SetupWizControl {

		//public string CtrlName {
		//	set {
		//		labelTitle.Text+=" " +value+"...";
		//	}
		//}

		//public string CtrlDesc {
		//	set {
		//		labelDesc.Text = value;
		//	}
		//}

		public UserControlSetupWizIntro(string name, string descript) {
			InitializeComponent();
			labelTitle.Text+=" " +name+"...";
			labelDesc.Text = descript;
			labelDesc.Text += "\r\n\r\n"
				+Lan.g("FormSetupWizard","If you do not want to set up your")+" "+name+" "
				+Lan.g("FormSetupWizard","at this time, click 'Skip' below.");
		}

		private void UserControlSetupWizIntro_Load(object sender,EventArgs e) {
			IsDone=true;
			//OnControlDone?.Invoke(sender,e);
		}
	}
}
