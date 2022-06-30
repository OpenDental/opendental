using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ConfirmationRequestT {
		///<summary>Deletes all entries.</summary>
		public static void ClearConfirmationRequestTable() {
			string command="DELETE FROM confirmationrequest WHERE ConfirmationRequestNum > 0";
			DataCore.NonQ(command);
		}

		///<summary>Create a confirmation request. If isEmail is false, it will be marked as an SMS</summary>
		public static ConfirmationRequest CreateConfirmationRequest(Appointment appt,Patient pat,long msgFk=0) {
			bool isEmail=pat.PreferContactMethod==ContactMethod.Email;
			ConfirmationRequest request=new ConfirmationRequest {
				ApptNum=appt.AptNum,
				ApptDateTime=appt.AptDateTime,
				ClinicNum=appt.ClinicNum, //use appt clinic; not prov clinic, pat clinic, or operatory clinic.
				DateTimeConfirmExpire=DateTime.Now.AddDays(1),
				MessageType= isEmail ? CommType.Email : CommType.Text,
				MessageFk=msgFk,
				MsgTextToMobileTemplate="",
				MsgTextToMobile="",
				GuidMessageToMobile=Guid.NewGuid().ToString(),
				EmailSubjTemplate="",
				EmailSubj="",
				EmailTextTemplate="",
				EmailText="",
				SecondsFromEntryToExpire=0,
				PatNum=appt.PatNum,
				PhonePat=pat.HmPhone,
				RSVPStatus=RSVPStatusCodes.PendingRsvp
			};
			ConfirmationRequests.Insert(request);
			return request;
		}
	}
}
