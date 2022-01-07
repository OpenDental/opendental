using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>An individual pharmacy store.</summary>
	[Serializable()]
	public class Pharmacy : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PharmacyNum;
		///<summary>NCPDPID assigned by NCPDP.  Not used yet.</summary>
		public string PharmID;
		///<summary>For now, it can just be a common description.  Later, it might have to be an official designation.</summary>
		public string StoreName;
		///<summary>Includes all punctuation.</summary>
		public string Phone;
		///<summary>Includes all punctuation.</summary>
		public string Fax;
		///<summary>.</summary>
		public string Address;
		///<summary>Optional.</summary>
		public string Address2;
		///<summary>.</summary>
		public string City;
		///<summary>Two char, uppercase.</summary>
		public string State;
		///<summary>.</summary>
		public string Zip;
		///<summary>A freeform note for any info that is needed about the pharmacy, such as hours.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The last date and time this row was altered.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		
		public Pharmacy Copy(){
			return (Pharmacy)this.MemberwiseClone();
		}	
	}
}

