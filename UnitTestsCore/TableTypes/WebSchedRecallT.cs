using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class WebSchedRecallT {

		///<summary>Gets all WebSchedRecalls for a PatNum.</summary>
		public static List<WebSchedRecall> GetWebSchedRecalls(long patNum) {
			return GetWebSchedRecalls(new List<long> { patNum });
		}

		///<summary>Gets all WebSchedRecalls for a list of PatNums.</summary>
		public static List<WebSchedRecall> GetWebSchedRecalls(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<WebSchedRecall>();
			}
			string command="SELECT * FROM webschedrecall WHERE PatNum IN("+string.Join(",",listPatNums)+")";
			return OpenDentBusiness.Crud.WebSchedRecallCrud.SelectMany(command);
		}

		public static WebSchedRecall CreateWebSchedRecall(long patNum,long recallNum,long clinicNum) {
			WebSchedRecall wsRecall=new WebSchedRecall {
				ClinicNum=clinicNum,
				EmailPat="patient@opendental.com",
				EmailSendStatus=AutoCommStatus.SendNotAttempted,
				IsForEmail=true,
				IsForSms=true,
				PatNum=patNum,
				PhonePat="503-363-5432",
				RecallNum=recallNum,
				SmsSendStatus=AutoCommStatus.SendNotAttempted,
			};
			WebSchedRecalls.Insert(wsRecall);
			return wsRecall;
		}

		public static void UpdateDateTEntry(WebSchedRecall wsRecall,DateTime dateTEntry) {
			string command=$"UPDATE webschedrecall SET DateTimeEntry={POut.DateT(dateTEntry)} WHERE WebSchedRecallNum={wsRecall.WebSchedRecallNum}";
			DataCore.NonQ(command);
		}

		///<summary>Clears the table. Does not truncate so that primary keys are not re-used.</summary>
		public static void ClearWebSchedRecallTable() {
			string command="DELETE FROM webschedrecall WHERE WebSchedRecallNum > 0";
			DataCore.NonQ(command);
		}

	}
}
