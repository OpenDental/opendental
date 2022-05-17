using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>These are the definitions for the custom patient fields added and managed by the user.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class ApptFieldDef:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptFieldDefNum;
		///<summary>The name of the field that the user will be allowed to fill in the appt edit window.  Duplicates are prevented.</summary>
		public string FieldName;
		///<summary>Enum:ApptFieldType Text=0,PickList=1</summary>
		public ApptFieldType FieldType;
		///<summary>The text that contains pick list values, each separated by \r\n.  Length 4000.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string PickList;
		///<summary>Zero based ordering for the items.</summary>
		public int ItemOrder;
	
		///<summary></summary>
		public ApptFieldDef Clone() {
			return (ApptFieldDef)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum ApptFieldType {
		///<summary>0</summary>
		Text,
		///<summary>1</summary>
		PickList
	}

	
}




