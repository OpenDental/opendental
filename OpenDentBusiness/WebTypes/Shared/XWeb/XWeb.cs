using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes.Shared.XWeb {
	///<summary></summary>
	[Serializable]
	public class XWeb : WebBase {
		public XWeb Copy() {
			return (XWeb)this.MemberwiseClone();
		}
	}

	public class WebPaymentProperties {
		public bool IsPaymentsAllowed;
		public string XWebID;
		public string AuthKey;
		public string TerminalID;
		public long PaymentTypeDefNum;
	}
}
