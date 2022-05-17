using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public enum MiddleTierRole{
		///<summary>This dll is on a local workstation, and this workstation has successfully connected directly to the database with no Middle Tier layer.</summary>
		ClientDirect,
		///<summary>Workstation that is getting its data from the Middle Tier web service on the server.</summary>
		ClientMT,
		///<summary>This dll is part of the Middle Tier web server that is providing data via web services.</summary>
		ServerMT
	}
}
