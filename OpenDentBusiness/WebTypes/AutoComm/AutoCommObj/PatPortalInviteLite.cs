using System.Data;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	///<summary>An object to store data used to send a PatientPortalInvite.</summary>
	public class PatPortalInviteLite:ApptLite {
		///<summary>The UserWeb and password info generated for this PatientPortalInvite.</summary>
		public PatientPortalCredential PatientPortalCredential;
		///<summary>FK to emailmessage.EmailMessageNum. The email that was sent for this invite.</summary>
		public long EmailMessageNum;

		public PatPortalInviteLite(DataRow row) : base(row) {	}

		///<summary>For unit testing only! Do not use otherwise!!</summary>
		public PatPortalInviteLite() {
			if(!ODBuild.IsUnitTest) {
				throw new System.Exception("PatPortalInviteLite paramless ctor not allowed outside of unit tests.");
			}
		}
	}
}
