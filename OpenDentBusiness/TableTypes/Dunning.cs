using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>A message that will show on certain patient statements when printing bills.  Criteria must be met in order for the dunning message to show.</summary>
	[Serializable]
	public class Dunning:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DunningNum;
		///<summary>The actual dunning message that will go on the patient bill.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DunMessage;
		///<summary>FK to definition.DefNum.</summary>
		public long BillingType;
		///<summary>Program forces only 0,30,60,or 90.</summary>
		public byte AgeAccount;
		///<summary>Enum:YN Set Y to only show if insurance is pending.</summary>
		public YN InsIsPending;
		///<summary>A message that will be copied to the NoteBold field of the Statement.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MessageBold;
		///<summary>An override for the default email subject.</summary>
		public string EmailSubject;
		///<summary>An override for the default email body. Limit in db: 16M char.</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)] 
		public string EmailBody;
		///<summary>The number of days before an account reaches AgeAccount to include this dunning message on statements.
		///Example: If DaysInAdvance=3 and AgeAccount=90, an account that is 87 days old when bills are generated will include this message.</summary>
		public int DaysInAdvance;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>Boolean. Is true when the message is specifically created for super families.</summary>
		public bool IsSuperFamily;


		///<summary>Returns a copy of this Dunning.</summary>
		public Dunning Copy(){
			return (Dunning)this.MemberwiseClone();
		}


	}
	


}













