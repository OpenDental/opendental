using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary>Stores the information for printing different types of claim forms.  Each claimform has many claimformitems attached to it, one for each field on the claimform.  This table has nothing to do with the actual claims.  It just describes how to print them.</summary>
	[Serializable()]
	public class ClaimForm:TableBase{
		///<summary>Primary key.</summary>
		//[XmlIgnore]
		[CrudColumn(IsPriKey=true)]
		public long ClaimFormNum;
		///<summary>eg. ADA2002 or CA Medicaid</summary>
		public string Description;
		///<summary>If true, then it will not be displayed in various claim form lists as a choice.</summary>
		//[XmlIgnore]
		public bool IsHidden;
		///<summary>Valid font name for all text on the form.</summary>
		public string FontName="";
		///<summary>Font size for all text on the form.</summary>
		public float FontSize;
		///<summary>Deprecated as of version 17.2. Internal claimforms have been moved over to XML files in OpenDentBusiness.Properties.Resources.</summary>
		public string UniqueID="";
		///<summary>Set to false to not print images.  This removes the background for printing on premade forms.</summary>
		public bool PrintImages;
		///<summary>Shifts all items by x/100th's of an inch to compensate for printer, typically less than 1/4 inch.</summary>
		//[XmlIgnore]
		public int OffsetX;
		///<summary>Shifts all items by y/100th's of an inch to compensate for printer, typically less than 1/4 inch.</summary>
		//[XmlIgnore]
		public int OffsetY;
		///<summary>The width of the claim form.</summary>
		public int Width;
		///<summary>The height of the claim form.</summary>
		public int Height;
		///<summary>This is not a database column.  It is list of all claimformItems that are attached to this ClaimForm.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<ClaimFormItem> Items=new List<ClaimFormItem>();
		///<summary>This is not a database column. If this claimform is internal, it cannot be edited.</summary>
		[CrudColumn(IsNotDbColumn = true)]
		public bool IsInternal;

		///<summary>Default constructor.</summary>
		public ClaimForm() {
			Width=850;
			Height=1100;
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("Items",typeof(ClaimFormItem[]))]
		public ClaimFormItem[] ItemsXml {
			get {
				if(Items==null) {
					return new ClaimFormItem[0];
				}
				return Items.ToArray();
			}
			set {
				Items=new List<ClaimFormItem>();
				for(int i = 0;i<value.Length;i++) {
					Items.Add(value[i]);
				}
			}
		}

		///<summary>Returns a copy of the claimform including the Items.</summary>
		public ClaimForm Copy() {
			ClaimForm cf = (ClaimForm)this.MemberwiseClone();
			List<ClaimFormItem> claimFormItemCopies = new List<ClaimFormItem>();
			for(int i = 0;i < cf.Items.Count;i++) {
				claimFormItemCopies.Add(cf.Items[i].Copy());
			}
			cf.Items=claimFormItemCopies;
			return cf;
		}


	}

	



}
