using System;

namespace OpenDentBusiness {
	///<summary>Server that hosts patientviewer services (Patient Portal, Mobile Web, and Web Sched)</summary>
	public class PatientviewerServer {
		///<summary>The name of the patientviewer server.</summary>
		public string ServerName;
		///<summary>The IP address of the patientviewer server.</summary>
		public string ServerIP;
		///<summary>The base URL by which to check services on the server.</summary>
		public string ServiceUrl => $"http://{ServerIP}";
	}
}
