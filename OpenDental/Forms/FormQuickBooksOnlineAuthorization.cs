using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormQuickBooksOnlineAuthorization:FormODBase {
		///<summary>The authorization code entered by the user.</summary>
		public string AuthCode="";
		///<summary>The realm Id entered by the user.</summary>
		public string RealmId="";

		public FormQuickBooksOnlineAuthorization() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender,EventArgs e) {
			AuthCode=textAuthCode.Text;
			RealmId=textRealmId.Text;
			DialogResult=DialogResult.OK;
		}

	}
}