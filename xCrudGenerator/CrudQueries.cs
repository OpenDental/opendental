using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace xCrudGenerator {
	public class CrudQueries {
		private const string rn="\r\n";
		private const string t4="\t\t\t\t";

		///<summary>Writes any necessary queries to the end of the ConvertDatabase file.  Usually zero or one.  The convertDbFile could also be the one in the Mobile folder.</summary>
		///<param name="logFileName">If this is blank, will display the results in a MessageBox, otherwise writes the results to the file.</param>
		public static void Write(string convertDbFile,Type typeClass,string dbName,bool isMobile,bool doRunQueries,bool doAppendToConvertDbFile,
			StringBuilder results) 
		{
			StringBuilder strb;
			FieldInfo[] fields=typeClass.GetFields();//We can't assume they are in the correct order.
			FieldInfo priKey=null;
			FieldInfo priKey1=null;
			FieldInfo priKey2=null;
			if(isMobile) {
				priKey1=CrudGenHelper.GetPriKeyMobile1(fields,typeClass.Name);
				priKey2=CrudGenHelper.GetPriKeyMobile2(fields,typeClass.Name);
			}
			else {
				priKey=CrudGenHelper.GetPriKey(fields,typeClass.Name);
			}
			string tablename=CrudGenHelper.GetTableName(typeClass);//in lowercase now.
			string priKeyParam=null;
			string priKeyParam1=null;
			string priKeyParam2=null;
			if(isMobile) {
				priKeyParam1=priKey1.Name.Substring(0,1).ToLower()+priKey1.Name.Substring(1);//lowercase initial letter.  Example customerNum
				priKeyParam2=priKey2.Name.Substring(0,1).ToLower()+priKey2.Name.Substring(1);//lowercase initial letter.  Example patNum
			}
			else {
				priKeyParam=priKey.Name.Substring(0,1).ToLower()+priKey.Name.Substring(1);//lowercase initial letter.  Example patNum
			}
			string obj=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);//lowercase initial letter.  Example feeSched or feeSchedm
			List<FieldInfo> fieldsExceptPri=null;
			if(isMobile) {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey2);//for mobile, only excludes PK2
			}
			else {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey);
			}
			CrudSpecialColType specialType;
			string command="SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '"+dbName+"' AND table_name = '"+tablename+"'";
			if(DataCore.GetScalar(command)!="1") {
				if(!CrudGenHelper.IsMissingInGeneral(typeClass)) {
					strb=new StringBuilder();
					strb.Append("This table was not found in the database:"
						+rn+tablename);
					if(doAppendToConvertDbFile) {
						strb.Append(rn+"Query will be found at the end of "+Path.GetFileName(convertDbFile));
					}
					if(doRunQueries) {
						strb.Append(rn+"Table will be added to database.");
					}
					results.AppendLine(strb.ToString());
					strb=new StringBuilder();
					strb.Append(rn+rn+t4+"/*");
					List<DbSchemaCol> cols=null;
					if(isMobile) {
						cols=CrudQueries.GetListColumns(priKey1.Name,priKey2.Name,fieldsExceptPri,true);
					}
					else {
						cols=CrudQueries.GetListColumns(priKey.Name,null,fieldsExceptPri,false);
					}
					strb.Append("\r\n"+CrudSchemaRaw.AddTable(tablename,cols,4,isMobile,doRunQueries));
					strb.Append(rn+t4+"*/");
					if(doAppendToConvertDbFile) {
						File.AppendAllText(convertDbFile,strb.ToString());
					}
				}
			}
			List<FieldInfo> newColumns=CrudGenHelper.GetNewFields(fields,typeClass,dbName);
			if(newColumns.Count>0) {
				strb=new StringBuilder();
				strb.Append("The following columns were not found in the database.");
				for(int f=0;f<newColumns.Count;f++) {
					strb.Append(rn+tablename+"."+newColumns[f].Name);
				}
				if(doAppendToConvertDbFile) {
					strb.Append(rn+"Query will be found at the end of "+Path.GetFileName(convertDbFile));
				}
				if(doRunQueries) {
					strb.Append(rn+"Column will be added to table.");
				}
				results.AppendLine(strb.ToString());
				strb=new StringBuilder();
				strb.Append(rn+rn+t4+"/*");
				for(int f=0;f<newColumns.Count;f++) {
					specialType=CrudGenHelper.GetSpecialType(newColumns[f]);
					OdDbType odtype=GetOdDbTypeFromColType(newColumns[f].FieldType,specialType);
					TextSizeMySqlOracle textsize=TextSizeMySqlOracle.Small;
					if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
						textsize=TextSizeMySqlOracle.Medium;
					}
					DbSchemaCol col=new DbSchemaCol(newColumns[f].Name,odtype,textsize);
					strb.Append(CrudSchemaRaw.AddColumnEnd(tablename,col,4,doRunQueries,typeClass));
				}
				strb.Append(rn+t4+"*/");
				if(doAppendToConvertDbFile) {
					File.AppendAllText(convertDbFile,strb.ToString());
				}
			}
		}

		public static OdDbType GetOdDbTypeFromColType(Type fieldType,CrudSpecialColType specialType) {
			if(specialType.HasFlag(CrudSpecialColType.DateEntry)
						|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) 
			{
				return OdDbType.Date;
			}
			if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
				return OdDbType.DateTimeStamp;
			}
			if(specialType.HasFlag(CrudSpecialColType.DateT)
						|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
						|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) 
			{
				return OdDbType.DateTime;
			}
			if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
				return OdDbType.VarChar255;
			}
			if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
				return OdDbType.TimeSpan;
			}
			if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
				return OdDbType.Long;
			}
			if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
				return OdDbType.Text;
			}
			if(fieldType.IsEnum) {
				return OdDbType.Enum;
			}
			switch(fieldType.Name) {
				default:
					throw new ApplicationException("Type not yet supported: "+fieldType.Name);
				case "Bitmap":
					return OdDbType.Text;
				case "Boolean":
					return OdDbType.Bool;
				case "Byte":
					return OdDbType.Byte;
				case "Color":
					return OdDbType.Int;
				case "DateTime"://This is only for date, not dateT
					return OdDbType.Date;
				case "Double":
					return OdDbType.Currency;
				case "Interval":
					return OdDbType.Int;
				case "Int64":
					return OdDbType.Long;
				case "Int32":
					return OdDbType.Int;
				case "Single":
					return OdDbType.Float;
				case "String":
					return OdDbType.VarChar255;//or text
				case "TimeSpan":
					return OdDbType.TimeOfDay;
			}
		}

		///<summary>priKeyName2=null for not mobile.</summary>
		public static List<DbSchemaCol> GetListColumns(string priKeyName1,string priKeyName2,List<FieldInfo> fieldsExceptPri,bool isMobile) {
			List<DbSchemaCol> retVal=new List<DbSchemaCol>();
			//DbSchemaCol col;
			retVal.Add(new DbSchemaCol(priKeyName1,OdDbType.Long));
			if(isMobile) {
				retVal.Add(new DbSchemaCol(priKeyName2,OdDbType.Long));
			}
			CrudSpecialColType specialType;
			for(int f=0;f<fieldsExceptPri.Count;f++) {
				if(isMobile && fieldsExceptPri[f].Name==priKeyName1) {//2 already skipped
					continue;
				}
				specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
				TextSizeMySqlOracle textsize=TextSizeMySqlOracle.Small;
				if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
					textsize=TextSizeMySqlOracle.Medium;
				}
				retVal.Add(new DbSchemaCol(fieldsExceptPri[f].Name,GetOdDbTypeFromColType(fieldsExceptPri[f].FieldType,specialType),textsize));
			}
			return retVal;
		}
		
	}
}
