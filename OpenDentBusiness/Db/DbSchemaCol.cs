//This is outdated, but will remain here for reference for a while.
//Functionality was moved to the CrudGenerator project.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	public class DbSchemaCol {
		public string ColumnName;
		public OdDbType DataType;
		//<summary>Specify Indexed true if column should be indexed.</summary>
		//public bool Indexed;
		///<summary>Specify textSize if there's any chance of it being greater than 4000 char.</summary>
		public TextSizeMySqlOracle TextSize;
		///<summary>If specifying an int, it uses int by default.  Set this to true to instead use smallint.</summary>
		public bool IntUseSmallInt;

		/*
		/// <summary>Takes DbSchemaCol and makes a new instance of it. </summary>
		public DbSchemaCol(DbSchemaCol newCol) {
			ColumnName=newCol.ColumnName;
			DataType=newCol.DataType;
			//Indexed=newCol.Indexed;
			IntUseSmallInt=newCol.IntUseSmallInt;
			TextSize=newCol.TextSize;
		}*/

		public DbSchemaCol(string columnName,OdDbType dataType) {
			ColumnName=columnName;
			DataType=dataType;
			//Indexed=false;
		}

		public DbSchemaCol(string columnName,OdDbType dataType,TextSizeMySqlOracle textSize) {
			ColumnName=columnName;
			DataType=dataType;
			//Indexed=indexed;
			TextSize=textSize;
		}

		public DbSchemaCol(string columnName,OdDbType dataType,TextSizeMySqlOracle textSize,bool intUseSmallInt) {
			ColumnName=columnName;
			DataType=dataType;
			//Indexed=indexed;
			TextSize=textSize;
			IntUseSmallInt=intUseSmallInt;
		}

		//public DbSchemaCol(string columnName,OdDbType dataType) {
		//	ColumnName=columnName;
		//	DataType=dataType;
			//Indexed=indexed;
		//}

		/// <summary>Creates a new instance of this object with identical variable values.</summary>
		public DbSchemaCol Copy() {
			return (DbSchemaCol)this.MemberwiseClone();
		}

	}

	public enum TextSizeMySqlOracle {
		///<summary>255-4k, MySql: text, Oracle: varchar2</summary>
		Small,
		///<summary>4k-65k, MySql: text, Oracle: clob</summary>
		Medium,
		///<summary>65k+, MySql: mediumtext, Oracle: clob</summary>
		Large
	}


}
