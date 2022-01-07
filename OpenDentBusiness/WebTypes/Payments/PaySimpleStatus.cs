using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes.Payments {
	public class PaySimpleStatus:WebBase {
		///<summary>external id - paysimple's id for payment.</summary>
		public long PaymentId;
		///<summary>status of the paysimple payment.</summary>
		public string PaymentStatus;
	}
}
