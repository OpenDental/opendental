using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CommlogT {

		public static Commlog CreateCommlog(long patNum,string text="",DateTime commDateTime=default(DateTime),
			CommSentOrReceived sentOrReceived=CommSentOrReceived.Sent,CommItemMode itemMode=CommItemMode.None,long userNum=0,string signature="",long commType=0) 
		{
			Commlog commlog=new Commlog {
				CommDateTime=commDateTime.Year > 1880 ? commDateTime : DateTime.Now,
				Mode_=itemMode,
				Note=text,
				PatNum=patNum,
				SentOrReceived=sentOrReceived,
				UserNum=userNum,
				Signature=signature,
				CommType=commType
			};
			Commlogs.Insert(commlog);
			return commlog;
		}

		public static Commlog CreateCommlogReferral(long referralNum,string note="",CommItemMode commItemMode=CommItemMode.None,
			CommSentOrReceived commSentOrReceived=CommSentOrReceived.Neither,long userNum=0, EnumCommReferralBehavior enumCommReferralBehavior=EnumCommReferralBehavior.None) 
		{
			Commlog commlog=new Commlog {
				PatNum=0,//Always 0 for referral commlogs.
				CommDateTime=DateTime.Now,
				Note=note,
				Mode_=commItemMode,
				SentOrReceived=commSentOrReceived,
				UserNum=userNum,
				ReferralNum=referralNum,
				CommReferralBehavior=enumCommReferralBehavior,
			};
			Commlogs.Insert(commlog);
			return commlog;
		}

		public static void ClearCommLogTable() {
			string command="DELETE FROM commlog";
			DataCore.NonQ(command);
		}

		public static List<Commlog> GetCommlogsForPat(long patNum) {
			string command=$"SELECT * FROM commlog where PatNum={patNum}";
			return OpenDentBusiness.Crud.CommlogCrud.SelectMany(command);
		}

		public static List<Commlog> GetAll() {
			string command=$"SELECT * FROM commlog";
			return OpenDentBusiness.Crud.CommlogCrud.SelectMany(command);
		}
	}
}
