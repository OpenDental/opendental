using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
		public class PayorTypeT {
		public static PayorType CreatePayorType(long patNum,string note="Test") {
			PayorType payorType=new PayorType {
				DateStart=DateTime.Now,
				Note=note,
				PatNum=patNum,
			};
			long payorTypeNum=PayorTypes.Insert(payorType);
			payorType.PayorTypeNum=payorTypeNum;
			PayorTypes.Update(payorType);
			return payorType;
		}
	}
}
