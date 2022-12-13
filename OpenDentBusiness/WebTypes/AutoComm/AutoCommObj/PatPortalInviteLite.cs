using System.Data;

namespace OpenDentBusiness.AutoComm {
	///<summary>An object to store data used to send a PatientPortalInvite.</summary>
	public class PatPortalInviteLite:ApptLite {
		///<summary>The UserWeb and password info generated for this PatientPortalInvite.</summary>
		public PatientPortalCredential PatientPortalCredential;
		///<summary>FK to emailmessage.EmailMessageNum. The email that was sent for this invite.</summary>
		public long EmailMessageNum;

		public PatPortalInviteLite(DataRow row) : base(row) {	}
	}
}
