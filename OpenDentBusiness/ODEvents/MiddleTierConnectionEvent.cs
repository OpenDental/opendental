using CodeBase;
using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Specific ConnectionEvent for when communication to the Middle Tier is unavailable.</summary>
	public class MiddleTierConnectionEvent {
		///<summary>This event will get fired whenever communication to the MiddleTier is attempted and fails.</summary>
		public static event MiddleTierConnectionEventHandler Fired;
		
		///<summary>Call this method only when communication to the database is not possible.</summary>
		public static void Fire(MiddleTierConnectionEventArgs e) {
			if(Fired!=null) {
				Fired(e);
			}
		}
	}

	public class MiddleTierConnectionEventArgs:ODEventArgs {
		///<summary>This will be set to true once the connection to the database or MiddleTier has been restored.</summary>
		public bool IsConnectionRestored;
		///<summary>Exception associated to the connection event, if any.</summary>
		public Exception Exception;

		public MiddleTierConnectionEventArgs(bool isConnectionRestored) 
			: base(ODEventType.MiddleTierConnection,MiddleTierConnectionEventType.MiddleTierConnectionLost.GetDescription()) 
		{
			IsConnectionRestored=isConnectionRestored;
		}
	}

	///<summary>A list of the types of mysql errors handled through FormConnectionLost</summary>
	public enum MiddleTierConnectionEventType {
		///<summary>Occurs when the connection is lost with the Middle Tier server.</summary>
		[Description("Connection to the Middle Tier server has been lost.  Connectivity will be retried periodically.  Click Retry to attempt to "
			+"connect manually or Exit Program to close the program.")]
		MiddleTierConnectionLost,
	}

	///<summary></summary>
	public delegate void MiddleTierConnectionEventHandler(MiddleTierConnectionEventArgs e);

}
