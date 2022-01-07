using System;

namespace OpenDentBusiness{

	///<summary>Each row is 1 computer, or if in RDP session, 1 connection from 1 computer, currently acting as a terminal for patient input.</summary>
	[Serializable]
	public class TerminalActive:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TerminalActiveNum;
		///<summary>The name of the computer where the terminal is active.</summary>
		public string ComputerName;
		///<summary>Enum:TerminalStatusEnum  No longer used.  Instead, the PatNum field is used.  Used to indicates at what point the patient was in the
		///sequence. 0=standby, 1=PatientInfo, 2=Medical, 3=UpdateOnly.  If status is 1, then nobody else on the network could open the patient edit
		///window for that patient.</summary>
		public TerminalStatusEnum TerminalStatus;
		///<summary>FK to patient.PatNum.  The patient currently showing in the terminal.  If 0, then terminal is in standby mode.</summary>
		public long PatNum;
		///<summary>The ID of the session from which this terminal instance was started.  The session ID is unique per computer login, so if this is a
		///terminal server every remote connection will have a unique session ID.  A kiosk is identified by ComputerName+SessionId+ProcessId.</summary>
		public int SessionId;
		///<summary>The ID of the process that initiated this kiosk instance.  This is unique per active computer process, so if a row exists with a
		///ProcessId that matches the instance we're about to start, we know it is safe to delete it, it must be left over and needs cleaned up.</summary>
		public int ProcessId;
		///<summary>The name of the computer used to make the remote connection to the app server when enabling kiosk mode. Could also be a name manually
		///entered by the user if there's already a connection to the app server from the same computer session. This serves as a human-readable name for
		///the ComputerName+SessionId+ProcessId to uniquely identify a kiosk. We will display the ComputerName and SessionName to the user in the kiosk
		///manager, but we will use the ComputerName+SessionId+ProcessId when the kiosk checks for available forms to display.</summary>
		public string SessionName;

		///<summary></summary>
		public TerminalActive Copy() {
			return (TerminalActive)this.MemberwiseClone();
		}

	}

		



		
	

	

	


}










