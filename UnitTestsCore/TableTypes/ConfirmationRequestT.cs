using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ConfirmationRequestT {

		///<summary>Create a confirmation request. If isEmail is false, it will be marked as an SMS</summary>
		public static ConfirmationRequest CreateConfirmationRequest(Appointment appt,Patient pat) {
			bool isEmail=pat.PreferContactMethod==ContactMethod.Email;
			ConfirmationRequest request=new ConfirmationRequest {
				ApptNum=appt.AptNum,
				AptDateTimeOrig=appt.AptDateTime,
				ClinicNum=appt.ClinicNum, //use appt clinic; not prov clinic, pat clinic, or operatory clinic.
				DateTimeConfirmExpire=DateTime.Now.AddDays(1),
				IsForSms=!isEmail,
				IsForEmail=isEmail,
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
