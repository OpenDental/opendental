using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {

	///<summary>This class is used to dynamically construct WHERE clauses for SQL queries.</summary>
	[Serializable]
	public class SQLWhere {
		///<summary>The clause that can be appended to the query. Public so that it can be serialized for middle tier.</summary>
		public string _whereClause;		

		///<summary>Creates a SQLParam that evaluates the specified column with regards to the specified value.</summary>
		public static SQLWhere Create<T>(string columnName,ComparisonOperator comparison,T value,bool doTreatDtAsDate=false,string tableName="") {
			if(!string.IsNullOrEmpty(tableName)) {
				columnName=tableName.ToLower()+"."+columnName;
			}
			SQLWhere sqlParam=new SQLWhere();
			sqlParam._whereClause=(doTreatDtAsDate ? DbHelper.DtimeToDate(columnName) : columnName)
				+comparison.GetDescription()+POutObj(value,doTreatDtAsDate);
			return sqlParam;
		}

		///<summary>Creates an IN clause using the specified column and the specified values.</summary>
		public static SQLWhere CreateIn<T>(string columnName,List<T> listValues,bool doTreatDtAsDate=false,string tableName="") {
			if(!string.IsNullOrEmpty(tableName)) {
				columnName=tableName.ToLower()+"."+columnName;
			}
			SQLWhere sqlParam=new SQLWhere();
			if(listValues.Count==0) {
				sqlParam._whereClause=" FALSE ";
			}
			else {
				sqlParam._whereClause=(doTreatDtAsDate ? DbHelper.DtimeToDate(columnName) : columnName)
					+" IN ("+string.Join(",",listValues.Select(x => POutObj(x,doTreatDtAsDate)))+")";
			}
			return sqlParam;
		}

		///<summary>Creates a NOT IN clause using the specified column and the specified values.</summary>
		public static SQLWhere CreateNotIn<T>(string columnName,List<T> listValues,bool doTreatDtAsDate=false,string tableName="") {
			if(!string.IsNullOrEmpty(tableName)) {
				columnName=tableName.ToLower()+"."+columnName;
			}
			SQLWhere sqlParam=new SQLWhere();
			if(listValues.Count==0) {
				sqlParam._whereClause=" TRUE ";
			}
			else {
				sqlParam._whereClause=(doTreatDtAsDate ? DbHelper.DtimeToDate(columnName) : columnName)
					+" NOT IN ("+string.Join(",",listValues.Select(x => POutObj(x,doTreatDtAsDate)))+")";
			}
			return sqlParam;
		}

		///<summary>Creates a BETWEEN clause with the specified column and the specified values.</summary>
		public static SQLWhere CreateBetween<T>(string columnName,T valueLower,T valueHigher,bool doTreatDtAsDate=false,string tableName="") {
			if(!string.IsNullOrEmpty(tableName)) {
				columnName=tableName.ToLower()+"."+columnName;
			}
			SQLWhere sqlParam=new SQLWhere();
			sqlParam._whereClause=(doTreatDtAsDate ? DbHelper.DtimeToDate(columnName) : columnName)
				+" BETWEEN "+POutObj(valueLower,doTreatDtAsDate)+" AND "+POutObj(valueHigher,doTreatDtAsDate)+"";
			return sqlParam;
		}

		///<summary>Creates a NOT BETWEEN clause with the specified column and the specified values.</summary>
		public static SQLWhere CreateNotBetween<T>(string columnName,T valueLower,T valueHigher,bool doTreatDtAsDate=false,string tableName="") {
			if(!string.IsNullOrEmpty(tableName)) {
				columnName=tableName.ToLower()+"."+columnName;
			}
			SQLWhere sqlParam=new SQLWhere();
			sqlParam._whereClause=(doTreatDtAsDate ? DbHelper.DtimeToDate(columnName) : columnName)
				+" NOT BETWEEN "+POutObj(valueLower,doTreatDtAsDate)+" AND "+POutObj(valueHigher,doTreatDtAsDate)+"";
			return sqlParam;
		}

		///<summary>Returns the SQLParam as a WHERE clause. Do not modify this method. 
		///Many implementers will rely on exactly this ToString() implementation. EG, string.Join().</summary>
		public override string ToString() {
			return _whereClause;
		}

		///<summary>POuts the passed in value in the appropriate manner.</summary>
		private static string POutObj(object value,bool doTreatDtAsDate) {
			if(value is bool) {
				return POut.Bool((bool)value);
			}
			else if(value is int) {
				return POut.Int((int)value);
			}
			else if(value is long) {
				return POut.Long((long)value);
			}
			else if(value is DateTime) {
				if(doTreatDtAsDate) {
					return POut.Date((DateTime)value);
				}
				else {
					return POut.DateT((DateTime)value);
				}
			}
			else if(value is string) {
				return "'"+POut.String((string)value)+"'";
			}
			else if(value is double) {
				return POut.Double((double)value);
			}
			else if(value is decimal) {
				return POut.Decimal((decimal)value);
			}
			else if(value is byte) {
				return POut.Byte((byte)value);
			}
			else if(value is float) {
				return POut.Float((float)value);
			}
			else if(value is TimeSpan) {
				return "'"+POut.TSpan((TimeSpan)value)+"'";
			}
			else if(value.GetType().IsEnum) {
				return POut.Int((int)value);
			}
			else {
				throw new NotImplementedException(value.GetType().Name+" has not been implemented in SQLWhere");
			}
		}
	}

	///<summary>A binary comparison operator.</summary>
	public enum ComparisonOperator {
		[Description("=")]
		Equals,
		[Description("!=")]
		NotEquals,
		[Description(">")]
		GreaterThan,
		[Description(">=")]
		GreaterThanOrEqual,
		[Description("<")]
		LessThan,
		[Description("<=")]
		LessThanOrEqual,
	}
}
