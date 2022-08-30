using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Hold parameter info in a database independent manner.</summary>
	public class OdSqlParameter {
		private string parameterName;
		private OdDbType dbType;
		private Object value;

		public OdDbType DbType {
			get { return dbType; }
			set { dbType = value; }
		}

		///<summary>parameterName should not include the leading character such as @ or : . And DbHelper.ParamChar() should be used to determine the char in the query itself.</summary>
		public string ParameterName {
			get { return parameterName; }
			set { parameterName = value; }
		}
	
		public Object Value {
			get { return this.value; }
			set { this.value = value; }
		}

		///<summary>parameterName should not include the leading character such as @ or : . And DbHelper.ParamChar() should be used to determine the char in the query itself.</summary>
		public OdSqlParameter(string parameterName,OdDbType dbType,Object value) {
			this.parameterName=parameterName;
			this.dbType=dbType;
			this.value=value;
		}

		public MySqlDbType GetMySqlDbType() {
			switch(this.dbType) {
				//case OdDbType.Blob:
				//	return MySqlDbType.MediumBlob;
				case OdDbType.Text:
					return MySqlDbType.MediumText;
				//none of these other types will use parameters.
					/*
				case OdDbType.Bool:
					return MySqlDbType.UByte;
				case OdDbType.Byte:
					return MySqlDbType.UByte;
				case OdDbType.Currency:
					return MySqlDbType.Double;
				case OdDbType.Date:
					return MySqlDbType.Date;
				case OdDbType.DateTime:
					return MySqlDbType.DateTime;
				case OdDbType.DateTimeStamp:
					return MySqlDbType.Timestamp;
				case OdDbType.Float:
					return MySqlDbType.Float;
				case OdDbType.Int:
					return MySqlDbType.Int32;
				case OdDbType.Long:
					return MySqlDbType.Int64;
				case OdDbType.Text:
					return MySqlDbType.MediumText;//hope this will work
				case OdDbType.TimeOfDay:
					return MySqlDbType.Time;
				case OdDbType.TimeSpan:
					return MySqlDbType.Time;
				case OdDbType.VarChar255:
					return MySqlDbType.VarChar;*/
				default:
					throw new ApplicationException("Type not found");
			}
		}

		public MySqlParameter GetMySqlParameter() {
			MySqlParameter param=new MySqlParameter();
			param.ParameterName=DbHelper.ParamChar+this.parameterName;
			param.Value=Value;
			param.MySqlDbType=GetMySqlDbType();
			return param;
		}

		//public OracleDbType GetOracleDbType() {
		//	switch(this.dbType) {
		//		//case OdDbType.Blob:
		//		//	return OracleDbType.Blob;
		//		case OdDbType.Text:
		//			return OracleDbType.Clob;
		//		//none of these other types will use parameters.
		//			/*
		//		case OdDbType.Bool:
		//			return OracleDbType.Byte;
		//		case OdDbType.Byte:
		//			return OracleDbType.Byte;
		//		case OdDbType.Currency:
		//			return OracleDbType.Decimal;
		//		case OdDbType.Date:
		//			return OracleDbType.Date;
		//		case OdDbType.DateTime:
		//			return OracleDbType.Date;
		//		case OdDbType.DateTimeStamp:
		//			return OracleDbType.Date;
		//		case OdDbType.Float:
		//			return OracleDbType.Double;
		//		case OdDbType.Int:
		//			return OracleDbType.Int32;
		//		case OdDbType.Long:
		//			return OracleDbType.Int64;
		//		case OdDbType.Text:
		//			return OracleDbType.Clob;
		//		case OdDbType.TimeOfDay:
		//			return OracleDbType.Date;
		//		case OdDbType.TimeSpan:
		//			return OracleDbType.Varchar2;
		//		case OdDbType.VarChar255:
		//			return OracleDbType.Varchar2;*/
		//		default:
		//			throw new ApplicationException("Type not found");
		//	}
		//}

		//public OracleParameter GetOracleParameter() {
		//	OracleParameter param=new OracleParameter();
		//	param.ParameterName=this.parameterName;
		//	param.OracleDbType=GetOracleDbType();
		//	return param;
		//}

	}


	

	
}
