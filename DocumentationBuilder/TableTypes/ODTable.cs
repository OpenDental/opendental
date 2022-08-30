using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationBuilder {
	public class ODTable {
		///<summary>The name of the table.</summary>
		public string Name;
		public List<ODColumn> ListColumns;

		///<summary>Returns a copy of this Table.</summary>
		public ODTable Copy() {
			return (ODTable)this.MemberwiseClone();
		}

		public ODTable(string name,List<ODColumn> listColumns) {
			Name=name;
			ListColumns=listColumns;
		}
	}

	public class ODColumn {
		///<summary>The name of the column.</summary>
		public string Name;
		///<summary>The column's datatype.</summary>
		public string DataType;
		///<summary>The column's ItemOrder.</summary>
		public string ItemOrder;
		///<summary>True if the column is deleted. Used for schema changes documentation.</summary>
		public bool IsDeleted;
		public List<ODEnum> ListODEnums;

		///<summary>Returns a copy of this Attribute.</summary>
		public ODColumn Copy() {
			return (ODColumn)this.MemberwiseClone();
		}
		
		public ODColumn(string name,string dataType,string itemOrder,List<ODEnum> listODEnums,bool isDeleted=false) {
			Name=name;
			DataType=dataType;
			ItemOrder=itemOrder;
			IsDeleted=isDeleted;
			ListODEnums=listODEnums;
		}
	}

	public class ODEnum {
		public string Name;
		public string Value;
		public ODEnumStatus Status=ODEnumStatus.Existing;
	}
	public enum ODEnumStatus {
		///<summary>0</summary>
		Existing,
		///<summary>1</summary>
		New,
		///<summary>2</summary>
		Deleted,
	}
}
