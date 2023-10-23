using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace OpenDentBusiness{

	///<summary>Quick paste categories are used by the quick paste notes feature.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class QuickPasteCat:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long QuickPasteCatNum;
		///<summary>.</summary>
		public string Description;
		///<summary>The order of this category within the list. 0-based.</summary>
		public int ItemOrder;
		///<summary>Enum:EnumQuickPasteType  Each Category can be set to be the default category for multiple types of notes. Stored as integers separated by commas.</summary>
		public string DefaultForTypes;

		///<summary>Helper property for a list of DefaultForTypes as an actual enumeration list.</summary>
		[XmlIgnore,JsonIgnore]
		public List<EnumQuickPasteType> ListDefaultForTypes {
			get {
				if(string.IsNullOrEmpty(DefaultForTypes)) {
					return new List<EnumQuickPasteType>();
				}
				return DefaultForTypes.Split(',').Select(x => PIn.Enum<EnumQuickPasteType>(x)).ToList();
			}
		}
		
		public QuickPasteCat Copy() {
			return (QuickPasteCat)this.MemberwiseClone();
		}
		


	}

	


}









