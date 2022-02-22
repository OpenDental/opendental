using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class SmsToMobileT {

		public static SmsToMobile CreateSmsToMobile(Patient pat,string guidMessage,SmsMessageSource source,long clinicNum=0) {
			string guidBatch=Guid.NewGuid().ToString();
			SmsToMobile smsToMobile=new SmsToMobile() {
				GuidBatch=guidBatch,
				GuidMessage=guidMessage,
				MsgType=source,
				PatNum=pat.PatNum,
				ClinicNum=clinicNum,
				MobilePhoneNumber=pat.WirelessPhone
			};
			smsToMobile.SmsToMobileNum=SmsToMobiles.Insert(smsToMobile);
			return smsToMobile;
		}

		public static List<SmsToMobile> GetForPats(List<long> listPatNums) {
			if(listPatNums.IsNullOrEmpty()) {
				return new List<SmsToMobile>();
			}
			string command="SELECT * FROM smstomobile WHERE PatNum IN("+string.Join(",",listPatNums)+")";
			return OpenDentBusiness.Crud.SmsToMobileCrud.SelectMany(command);
		}

		///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearSmsToMobileTable() {
			string command="DELETE FROM smstomobile WHERE SmsToMobileNum > 0";
			DataCore.NonQ(command);
		}
	}
}
