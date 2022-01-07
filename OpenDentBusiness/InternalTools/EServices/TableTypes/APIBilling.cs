using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Only used internally by OpenDental, Inc.  Not used by anyone else. Aggregates customer charges for usage of FHIR API.</summary>
	[Serializable()]
	[CrudTable(IsMissingInGeneral=true,CrudLocationOverride = @"..\..\..\OpenDentBusiness\InternalTools\EServices\Crud")]
	public class APIBilling:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long APIBillingNum;
		///<summary>Should be unique in this table per DateOfBill.</summary>
		public long CustPatNum;
		///<summary>From patient.BillingCycleDay. The day of the month that this patient receives their bill.</summary>
		public int BillingCycleDay;
		///<summary>Timestamp when this row is entered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>Timestamp when this row was processed by RepeatCharge tool and procedures were posted. If MinVal then this row has not been processed yet.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeProceduresPosted;
		///<summary>Date only. Indicates the exact date on which the bill should post to the procedure log.</summary>
		public DateTime DateOfBill;
		///<summary>Date only. Indicates which calendar month this usage metric is for. Should be 1st of month at midnight. Example '2012-01-01 00:00:00'.
		///Will include charges from both the previous month's usage and the coming month's service activations.</summary>
		public DateTime MonthOfBill;
		///<summary>The produced output of the FHIR billing algorithm is a json serialized collection (List) of procedures. 
		///This will later be deserialized by OD proper and inserted into the procedurelog table.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProceduresJson;

		public APIBilling Copy() {
			return (APIBilling)MemberwiseClone();
		}
	}
}
