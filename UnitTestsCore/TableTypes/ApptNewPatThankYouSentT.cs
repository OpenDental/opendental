using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ApptNewPatThankYouSentT {
		public static ApptNewPatThankYouSent CreateOne(Patient pat,Appointment appt,string message="",AutoCommStatus status=AutoCommStatus.SendNotAttempted,CommType commType=CommType.Text) {
			ApptNewPatThankYouSent apptNewPat=new ApptNewPatThankYouSent {
				ApptNum=appt.AptNum,
				ApptDateTime=appt.AptDateTime,
				PatNum=pat.PatNum,
				ClinicNum=pat.ClinicNum,
				SendStatus=status,
				MessageType=commType,
				Message=message,
			};
			DataAction.RunPractice(() => ApptNewPatThankYouSents.InsertMany(new List<ApptNewPatThankYouSent> { apptNewPat }));
			return apptNewPat;
		}

		public static void ClearApptNewPatThankYouSentTable() {
			string command="DELETE FROM apptnewthankyousent WHERE ApptThankYouSentNum > 0";
			DataCore.NonQ(command);
		}

		///<summary></summary>
		public static List<ApptNewPatThankYouSent> GetAll() {
			string command="SELECT * FROM apptnewthankyousent";
			return DataAction.GetPractice(() => OpenDentBusiness.Crud.ApptNewPatThankYouSentCrud.SelectMany(command));
		}
	}
}
