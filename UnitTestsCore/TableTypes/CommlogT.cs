using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CommlogT {

		public static Commlog CreateCommlog(long patNum,string text="",DateTime commDateTime=default(DateTime),
			CommSentOrReceived sentOrReceived=CommSentOrReceived.Sent,CommItemMode itemMode=CommItemMode.None) 
		{
			Commlog commlog=new Commlog {
				CommDateTime=commDateTime.Year > 1880 ? commDateTime : DateTime.Now,
				Mode_=itemMode,
				Note=text,
				PatNum=patNum,
				SentOrReceived=sentOrReceived,
			};
			Commlogs.Insert(commlog);
			return commlog;
		}
	}
}
