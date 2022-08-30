using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Important columns from the RemoteSupport.PeerInfo table that represents a client or host connected to a session.</summary>
	[Serializable]
	public class PeerInfo {
		///<summary>The display name chosen by the peer.  For technicians this is always their full email address.</summary>
		public string UserName="";
		///<summary>The server date and time (UTC) when the peer originally joined the session. Does not update when peer rejoins.</summary>
		public DateTime DateTimeJoined=DateTime.MinValue;

		///<summary>FK to employee. This is the employee that has an email address that exactly matches the UserName field.
		///This is a helper field that needs to be programmatically set by the entity that cares about PeerInfos that are linked to employees.</summary>
		[XmlIgnore,JsonIgnore]
		public long EmployeeNum;
		///<summary>Localizes DateTimeJoined and then returns a TimeSpan that represents how long the user has been in the current session.</summary>
		[XmlIgnore,JsonIgnore]
		public TimeSpan SessionTime => (DateTime.Now-DateTimeJoined.ToLocalTime());
	}
}
