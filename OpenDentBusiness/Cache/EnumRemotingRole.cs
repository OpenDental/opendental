using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public enum RemotingRole{
		///<summary>This dll is on a local workstation, and this workstation has successfully connected directly to the database with no 'server' layer.</summary>
		ClientDirect,
		///<summary>Workstation that is getting its data from a web service on the server.</summary>
		ClientWeb,
		///<summary>This dll is part of a web server that is providing data via web services.</summary>
		ServerWeb
	}
}
