using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDentHL7 {

	public partial class FormDebug:Form {
		private ServiceHL7 _service=new ServiceHL7();

		public FormDebug(string serviceName="") {
			_service.ServiceName=serviceName;
			InitializeComponent();
		}

		private void FormDebug_Load(object sender,EventArgs e) {
			try {
				textHL7Name.Text=_service.ServiceName;
				ChooseDatabaseInfo chooseDatabaseInfo=CentralConnections.GetChooseDatabaseConnectionSettings();
				textServerName.Text=chooseDatabaseInfo.CentralConnectionCur.ServerName;
				textDatabaseName.Text=chooseDatabaseInfo.CentralConnectionCur.DatabaseName;
			}
			catch(Exception ex) {
				textOutput.Text=ex.ToString()+"\r\n";
			}
		}

		private void butStart_Click(object sender,EventArgs e) {
			if(butStart.Text=="Start") {
				try {
					_service.ServiceName=textHL7Name.Text;
					_service.StartManually();
					butStart.Text="Stop";
					textHL7Name.ReadOnly=true;
					textOutput.Text=_service.ServiceName+" Running...\r\n";
				}
				catch(Exception ex) {
					butStart.Text="Start";
					textHL7Name.ReadOnly=false;
					textOutput.Text=ex.ToString()+"\r\n";
				}
			}
			else {
				_service.Stop();
				butStart.Text="Start";
				textHL7Name.ReadOnly=false;
				textOutput.AppendText(_service.ServiceName+" Stopped\r\n");
			}
		}

	}
}
