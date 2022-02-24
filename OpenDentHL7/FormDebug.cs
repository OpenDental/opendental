using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDentHL7 {

	public partial class FormDebug:Form {
		private string _serviceName;

		public FormDebug(string serviceName="") {
			_serviceName=serviceName;
			InitializeComponent();
		}

		private void FormDebug_Load(object sender,EventArgs e) {
			try {
				textHL7Name.Text=_serviceName;
				CentralConnections.GetChooseDatabaseConnectionSettings(out CentralConnection centralConnection,out string _,out YN _,
					out DataConnectionBase.DatabaseType _,out List<string> _,out bool _,out bool _);
				textServerName.Text=centralConnection.ServerName;
				textDatabaseName.Text=centralConnection.DatabaseName;
			}
			catch(Exception ex) {
				textOutput.Text=ex.ToString();
			}
		}

		private void butStart_Click(object sender,EventArgs e) {
			ServiceHL7 service=new ServiceHL7();
			service.ServiceName=_serviceName;
			try {
				service.StartManually();
			}
			catch(Exception ex) {
				textOutput.Text=ex.ToString();
			}
		}

	}
}
