using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>One item is needed for each field on a claimform.</summary>
	[Serializable()]
	public class ClaimFormItem:TableBase {
		///<summary>Primary key.</summary>
		//[XmlIgnore]
		[CrudColumn(IsPriKey=true)]
		public long ClaimFormItemNum;
		///<summary>FK to claimform.ClaimFormNum</summary>
		//[XmlIgnore]
		public long ClaimFormNum;
		///<summary>If this item is an image.  Usually only one per claimform.  eg ADA2002.emf.  Otherwise it MUST be left blank, or it will trigger an error that the image cannot be found.</summary>
		public string ImageFileName;
		///<summary>Must be one of the hardcoded available fieldnames for claims.</summary>
		public string FieldName;
		///<summary>For dates, the format string. ie MM/dd/yyyy or M d y among many other possibilities.</summary>
		public string FormatString;
		///<summary>The x position of the item on the claim form.  In pixels. 100 pixels per inch.</summary>
		public float XPos;
		///<summary>The y position of the item.</summary>
		public float YPos;
		///<summary>Limits the printable area of the item. Set to zero to not limit.</summary>
		public float Width;
		///<summary>Limits the printable area of the item. Set to zero to not limit.</summary>
		public float Height;

		///<summary>Returns a copy of the claimformitem.</summary>
    public ClaimFormItem Copy(){
			return (ClaimFormItem)this.MemberwiseClone(); 
		}

	}

	

	

}









