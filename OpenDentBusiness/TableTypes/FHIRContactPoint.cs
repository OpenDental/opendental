using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Details of a Technology mediated contact point (phone, fax, email, etc.). https://www.hl7.org/fhir/datatypes.html#contactpoint 
	///.</summary>
	[Serializable]
	public class FHIRContactPoint:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FHIRContactPointNum;
		///<summary>FK to fhirsubscription.FHIRSubscriptionNum.</summary>
		public long FHIRSubscriptionNum;
		///<summary>Enum:ContactPointSystem </summary>
		public ContactPointSystem ContactSystem;
		///<summary>The actual contact point details.</summary>
		public string ContactValue;
		///<summary>Enum:ContactPointUse </summary>
		public ContactPointUse ContactUse;
		///<summary>Specify preferred order of use (1 = highest)</summary>
		public int ItemOrder;
		///<summary>Time when the contact point started to be in use.</summary>
		public DateTime DateStart;
		///<summary>Timewhen the contact point stopped being used.</summary>
		public DateTime DateEnd;

		///<summary></summary>
		public FHIRContactPoint Copy() {
			return (FHIRContactPoint)this.MemberwiseClone();
		}
	}

	public enum ContactPointSystem {
		///<summary>The value is a telephone number used for voice calls. Use of full international numbers starting with + is recommended to enable
		///automatic dialing support but not required.</summary>
		Phone,
		///<summary>The value is a fax machine. Use of full international numbers starting with + is recommended to enable automatic dialing support 
		///but not required.</summary>
		Fax,
		///<summary>The value is an email address.</summary>
		Email,
		///<summary>The value is a pager number. These may be local pager numbers that are only usable on a particular pager system.</summary>
		Pager,
		///<summary>A contact that is not a phone, fax, or email address. The format of the value SHOULD be a URL. This is intended for various personal 
		///contacts including blogs, Twitter, Facebook, etc. Do not use for email addresses. If this is not a URL, then it will require human 
		///interpretation.</summary>
		Other
	}

	public enum ContactPointUse {
		///<summary>A communication contact point at a home; attempted contacts for business purposes might intrude privacy and chances are one will 
		///contact family or other household members instead of the person one wishes to call. Typically used with urgent cases, or if no other contacts 
		///are available.</summary>
		Home,
		///<summary>An office contact point. First choice for business related contacts during business hours.</summary>
		Work,
		///<summary>A temporary contact point. The period can provide more detailed information.</summary>
		Temp,
		///<summary>This contact point is no longer in use (or was never correct, but retained for records).</summary>
		Old,
		///<summary>A telecommunication device that moves and stays with its owner. May have characteristics of all other use codes, suitable for urgent 
		///matters, not the first choice for routine business.</summary>
		Mobile
	}
}
