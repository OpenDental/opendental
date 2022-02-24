using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table represents a grouping of promotionlogs. When sending a waive of emails, this table links those promotion logs/emails together.</summary>
	[Serializable]
	public class Promotion:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PromotionNum;
		///<summary>The name of the promotion.</summary>
		public string PromotionName;
		///<summary>The time this promotion was sent out.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime DateTimeCreated;
		///<summary>FK to clinic.ClinicNum
		///The clinic this promotion was sent for.</summary>
		public long ClinicNum;
		///<summary>Enum:PromotionType - The type of promotion this is.</summary>
		public PromotionType TypePromotion;
	}

	public enum PromotionType {
		///<summary>0 - Signifies Manually Sent Promotions like from Mass Emails</summary>
		Manual,
		///<summary>1 - Signifies Birthday Greetings</summary>
		Birthday,
		///<summary>2 - Promotional Treatment</summary>
		Treatment,
		///<summary>3 - Special Promotions</summary>
		Special,
	}
}
