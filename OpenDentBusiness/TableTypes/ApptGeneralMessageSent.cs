using System;
using OpenDentBusiness.WebTypes.AutoComm;

namespace OpenDentBusiness {
	///<summary>When a general message is sent for an appointment a record of that send is stored here. This is used to prevent re-sends of the same message.</summary>
	[Serializable, CrudTable(HasBatchWriteMethods = true)]
	public class ApptGeneralMessageSent:AutoCommAppt {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptGeneralMessageSentNum;
	}
}
