using OpenDentBusiness;
using OpenDentBusiness.AutoComm;
using OpenDentBusiness.WebTypes.AutoComm;

namespace UnitTestsCore {
	public class CalendarIcsInfoT {
		public static CalendarIcsInfo CreateCalenarIcsInfo(Appointment appt) {
			CalendarIcsInfo calIcs=new CalendarIcsInfo() {
				Title=appt.AptNum.ToString(),
				Location=appt.AptNum.ToString(),
				PatNum=appt.PatNum,
				AptNum=appt.AptNum,
				DateStart=appt.AptDateTime,
				DateEnd=appt.AptDateTime,
				OfficeEmail=appt.AptNum.ToString(),
				Method=CalMethod.Request,
			};
			return calIcs;
		}
	}
}
