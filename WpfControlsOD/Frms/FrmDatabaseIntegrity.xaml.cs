using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary>Message box shown if the database was altered by a third party outside of Open Dental.
	///Set the TextWarningMessage to describe which entries are unsafe. </summary>
	public partial class FrmDatabaseIntegrity:FrmODBase {
		///<summary>This can be quite long.</summary>
		public string MessageToShow;
		///<summary>So that it shows a different link.</summary>
		public bool IsPlugin;

		public FrmDatabaseIntegrity() {
			InitializeComponent();
			Load+=FormDatabaseIntegrity_Load;
		}

		private void FormDatabaseIntegrity_Load(object sender,EventArgs e) {
			Lang.F(this);
			labelMessage.Text=MessageToShow;
			if(IsPlugin){
				linkLabel.Text="https://www.opendental.com/site/plugins.html";
			}
		}

		private void linkLabel_LinkClicked(object sender,EventArgs e) {
			if(IsPlugin){
				Process.Start("https://www.opendental.com/site/plugins.html");
			}
			else{
				Process.Start("https://www.opendental.com/site/integrity.html");
			}
		}

	}
}