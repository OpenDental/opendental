using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes.Shared.XWeb {
	///<summary>All the fields from an Edge Express API response.</summary>
	public class EdgeExpressResponse {
		public string ALIAS;
		public decimal APPROVEDAMOUNT;
		public string APPROVALCODE;
		public decimal AUTHORIZEDAMOUNT;
		public int BATCHNO;
		public decimal CAPTUREDAMOUNT;
		public string CARDBRAND;
		public string CARDCODERESPONSE;
		public string CARDTYPE;
		public int EXPMONTH;
		public int EXPYEAR;
		public string INDUSTRY;
		public string MASKEDCARDNUMBER;
		public string ORDERID;
		public decimal ORIGINALAUTHORIZEDAMOUNT;
		public string ORIGINALPROCESSORRESPONSE;
		public decimal ORIGINALREQUESTEDAMOUNT;
		public string ORIGINALRESPONSECODE;
		public string ORIGINALRESPONSEDESCRIPTION;
		public long ORIGINALTRANSACTIONDATETIME;
		public string ORIGINALTRANSACTIONTYPE;
		public string PROCESSORRESPONSE;
		public int RECEIPTID;
		public int RESPONSECODE;
		public string RESPONSEDESCRIPTION;
		public string RETURNID;
		public string STATE;
		public long TRANSACTIONDATETIME;
		public string TRANSACTIONID;
		public string TRANSACTIONTYPE;
	}

}
