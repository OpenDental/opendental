using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>When some objects are deleted, we sometimes need a way to track them for synching purposes.  Other objects already have fields for IsHidden or PatStatus which track deletions just fine.  Those types of objects will not use this table.</summary>
	[Serializable(),CrudTable(HasBatchWriteMethods=true)]
	public class DeletedObject:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DeletedObjectNum;
		///<summary>Foreign key to a number of different tables, depending on which type it is.</summary>
		public long ObjectNum;
		///<summary>Enum:DeletedObjectType </summary>
		public DeletedObjectType ObjectType;
		///<summary>Updated any time the row is altered in any way.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		
		public DeletedObject Clone(){
			return (DeletedObject)this.MemberwiseClone();
		}	
	}
}




	










