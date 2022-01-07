using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>If a number is entered in this table, then any incoming text message will not be entered into the database.</summary>
	[Serializable]
	public class SmsBlockPhone:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SmsBlockPhoneNum;
		///<summary>The phone number to be blocked.</summary>
		public string BlockWirelessNumber;

		///<summary></summary>
		public SmsBlockPhone Copy() {
			return (SmsBlockPhone)this.MemberwiseClone();
		}
	}
	
}