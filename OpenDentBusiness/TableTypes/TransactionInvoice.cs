using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Used in the accounting section of the program.  Each row contains a document that is attached to a transaction.</summary>
	[Serializable]
	public class TransactionInvoice:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TransactionInvoiceNum;
		///<summary>File name including the extension.</summary>
		public string FileName;
		///<summary>The raw file data converted to base64.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string InvoiceData;

		///<summary></summary>
		public TransactionInvoice Copy() {
			return (TransactionInvoice)MemberwiseClone();
		}
	}
}
