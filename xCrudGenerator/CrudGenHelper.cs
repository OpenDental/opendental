using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using Microsoft.Build.Evaluation;
using OpenDentBusiness;

namespace xCrudGenerator {
	public class CrudGenHelper {
		///<summary>Will throw exception if no primary key attribute defined.</summary>
		public static FieldInfo GetPriKey(FieldInfo[] fields,string tableName){
			for(int i=0;i<fields.Length;i++) {
				object[] attributes = fields[i].GetCustomAttributes(typeof(CrudColumnAttribute),true);
				if(attributes.Length!=1) {
					continue;
				}
				if(((CrudColumnAttribute)attributes[0]).IsPriKey) {
					return fields[i];
				}
			}
			throw new ApplicationException("No primary key defined for "+tableName);
		}

		///<summary>Will throw exception if no primary key attribute defined.</summary>
		public static FieldInfo GetPriKeyMobile1(FieldInfo[] fields,string tableName) {
			for(int i=0;i<fields.Length;i++) {
				object[] attributes = fields[i].GetCustomAttributes(typeof(CrudColumnAttribute),true);
				if(attributes.Length!=1) {
					continue;
				}
				if(((CrudColumnAttribute)attributes[0]).IsPriKeyMobile1) {
					return fields[i];
				}
			}
			throw new ApplicationException("No primary key 1 defined for "+tableName);
		}

		///<summary>Will throw exception if no primary key attribute defined.</summary>
		public static FieldInfo GetPriKeyMobile2(FieldInfo[] fields,string tableName) {
			for(int i=0;i<fields.Length;i++) {
				object[] attributes = fields[i].GetCustomAttributes(typeof(CrudColumnAttribute),true);
				if(attributes.Length!=1) {
					continue;
				}
				if(((CrudColumnAttribute)attributes[0]).IsPriKeyMobile2) {
					return fields[i];
				}
			}
			throw new ApplicationException("No primary key 2 defined for "+tableName);
		}

		///<summary>The name of the table in the database.  By default, the lowercase name of the class type.</summary>
		public static string GetTableName(Type typeClass) {
			return CrudTableAttribute.GetTableName(typeClass);
		}

		public static FieldInfo GetCemtSyncKey(FieldInfo[] fields,string tableName) {
			for(int i = 0;i<fields.Length;i++) {
				object[] attributes = fields[i].GetCustomAttributes(typeof(CrudColumnAttribute),true);
				if(attributes.Length!=1) {
					continue;
				}
				if(((CrudColumnAttribute)attributes[0]).IsCemtSyncKey) {
					return fields[i];
				}
			}
			throw new ApplicationException($"The TableType {tableName} does not contain a field with the {nameof(CrudColumnAttribute.IsCemtSyncKey)} attribute");
		}
		///<summary></summary>
		public static bool IsDeleteForbidden(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).IsDeleteForbidden) {
					return true;
				}
			}
			//couldn't find any.
			return false;
		}

		///<summary></summary>
		public static bool IsMissingInGeneral(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).IsMissingInGeneral) {
					return true;
				}
			}
			//couldn't find any.
			return false;
		}

		///<summary>Returns the custom crud class location for given typeClass, or blank if not set.</summary>
		public static string GetCrudLocationOverride(Type typeClass) {
			object[] attributes=typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(!string.IsNullOrEmpty(((CrudTableAttribute)attributes[i]).CrudLocationOverride)) {
					return ((CrudTableAttribute)attributes[i]).CrudLocationOverride;
				}
			}
			return "";
		}
		
		///<summary>Returns the custom namespace for given typeClass, or blank if not set.</summary>
		public static string GetNamespaceOverride(Type typeClass) {
			object[] attributes=typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(!string.IsNullOrEmpty(((CrudTableAttribute)attributes[i]).NamespaceOverride)) {
					return ((CrudTableAttribute)attributes[i]).NamespaceOverride;
				}
			}
			return "";
		}
		
		///<summary>Returns the given typeClass excludes PrefC in its CRUD file.</summary>
		public static bool GetCrudExcludePrefC(Type typeClass) {
			object[] attributes=typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				return ((CrudTableAttribute)attributes[i]).CrudExcludePrefC;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsMobile(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).IsMobile) {
					return true;
				}
			}
			//couldn't find any.
			return false;
		}

		///<summary></summary>
		public static bool IsSynchable(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).IsSynchable) {
					return true;
				}
			}
			//couldn't find any.
			return false;
		}

		public static bool IsSynchableBatchWriteMethods(Type typeClass) {
			return typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true).Any(x => ((CrudTableAttribute)x).IsSynchableBatchWriteMethods);
		}

		///<summary></summary>
		public static bool IsSecurityStamped(Type typeClass) {
			return typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true).Any(x => ((CrudTableAttribute)x).IsSecurityStamped);
		}

		///<summary></summary>
		public static bool HasBatchWriteMethods(Type typeClass) {
			object[] attributes=typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).HasBatchWriteMethods) {
					return true;
				}
			}
			//couldn't find any.
			return false;
		}

		///<summary></summary>
		public static bool UsesDataReader(Type typeClass) {
			return typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true).OfType<CrudTableAttribute>().Any(x => x.UsesDataReader);
		}

		///<summary>True if the table has the IsTableHist attribute set to true.</summary>
		public static bool IsTableHist(Type typeClass) {
			return typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true).OfType<CrudTableAttribute>().Any(x => x.IsTableHist);
		}

		///<summary>True if the table contains any IsNotCemtColumn attributes set to true.</summary>
		public static bool IsTableSkipCemt(Type typeClass) {
			FieldInfo[] tableFields = typeClass.GetFields();
			return ContainsFieldsNotCemtColumn(tableFields);
		}

		///<summary>True if table has the IsTableHist attribute set to true and the field is inherited from the other table</summary>
		public static bool IsFieldInherited(Type typeClass,FieldInfo field) {
			return IsTableHist(typeClass) && field.DeclaringType.Name!=typeClass.Name;
		}
		
		///<summary>For Mobile, this only excludes PK2; result includes PK1, the CustomerNum.  Always excludes fields that are not in the database, like patient.Age.</summary>
		public static List<FieldInfo> GetFieldsExceptPriKey(FieldInfo[] fields,FieldInfo priKey) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(fields[i].Name==priKey.Name) {
					continue;
				}
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>For Mobile, this only excludes PK2; result includes PK1, the CustomerNum.  Includes IsNotDbColumn.</summary>
		public static List<FieldInfo> GetAllFieldsExceptPriKey(FieldInfo[] fields,FieldInfo priKey) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(fields[i].Name==priKey.Name) {
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>This only excludes fields that are not in the database, like patient.Age.</summary>
		public static List<FieldInfo> GetFieldsExceptNotDb(FieldInfo[] fields) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>Returns all fields that should be included with CEMT related CRUD methods.</summary>
		public static List<FieldInfo> GetFieldsExceptNotCemt(FieldInfo[] fields) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i = 0;i<fields.Length;i++) {
				if(IsNotCemtSkipColumn(fields[i])) {
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>This only excludes fields that do not sync to CEMT remote databases.</summary>
		public static bool ContainsFieldsNotCemtColumn(FieldInfo[] fields) {
			for(int i = 0;i<fields.Length;i++) {
				if(IsNotCemtSkipColumn(fields[i])) {
					return true;
				}
			}
			return false;
		}
		///<summary>This gets all new fields which are found in the table definition but not in the database.  Result will be empty if the table itself is not in the database.</summary>
		public static List<FieldInfo> GetNewFields(FieldInfo[] fields,Type typeClass,string dbName) {
			string tablename=GetTableName(typeClass);
			string command="SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '"+dbName+"' AND table_name = '"+tablename+"'";
			if(DataCore.GetScalar(command)!="1") {
				return new List<FieldInfo>();
			}
			command="SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS "
				+"WHERE table_name = '"+tablename+"' AND table_schema = '"+dbName+"'";
			DataTable table=DataCore.GetTable(command);
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(IsNotDbColumn(fields[i])) {
					continue;
				}
				bool found=false; ;
				for(int t=0;t<table.Rows.Count;t++) {
					if(table.Rows[t]["COLUMN_NAME"].ToString().ToLower()==fields[i].Name.ToLower()) {
						found=true;
					}
				}
				if(!found) {
					retVal.Add(fields[i]);
				}
			}
			return retVal;
		}

		///<summary>Pass in fields processed by GetFieldsExceptPriKey.  This quick method returns the bigint fields so that indexes can possibly be added.  For mobile, pass in the priKeyName2 so that it can be excluded.  If not mobile, then set it to null.</summary>
		public static List<FieldInfo> GetBigIntFields(List<FieldInfo> fieldsExceptPri,string priKeyName2) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fieldsExceptPri.Count;i++) {
				if(priKeyName2 != null) {//mobile
					if(fieldsExceptPri[i].Name==priKeyName2) {
						continue;
					}
				}
				if(fieldsExceptPri[i].FieldType.Name=="Int64") {
					retVal.Add(fieldsExceptPri[i]);
				}
			}
			return retVal;
		}

		public static CrudSpecialColType GetSpecialType(FieldInfo field) {
			object[] attributes = field.GetCustomAttributes(typeof(CrudColumnAttribute),true);
			if(attributes.Length==0) {
				return CrudSpecialColType.None;
			}
			return ((CrudColumnAttribute)attributes[0]).SpecialType;
		}

		///<summary>Normally false</summary>
		public static bool IsNotDbColumn(FieldInfo field) {
			object[] attributes = field.GetCustomAttributes(typeof(CrudColumnAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			return ((CrudColumnAttribute)attributes[0]).IsNotDbColumn;
		}

		///<summary>Normally false.</summary>
		public static bool IsNotCemtSkipColumn(FieldInfo field) {
			object[] attributes = field.GetCustomAttributes(typeof(CrudColumnAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			return ((CrudColumnAttribute)attributes[0]).IsNotCemtColumn;
		}

		public static void ConnectToDatabase(string dbName,string server="localhost",string user= "root",string password=""){
			if(server.ToLower().Contains("server") && dbName.ToLower()=="customers") {
				throw new ApplicationException("Not allowed to connect to the 'customers' database.");
			}
			DataConnection dcon=new DataConnection();
			dcon.SetDb(server,dbName,user,password,"","",DatabaseType.MySql);
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
		}

		public static void ConnectToDatabaseM(string dbName) {
			DataConnection dcon=new DataConnection();
			dcon.SetDb("10.10.1.196",dbName,"root","","","",DatabaseType.MySql);
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
		}

		///<summary>Gets the regular non-mobile type by stripping the m off the end of the mobile type.  Quicker than formalizing the type with an attribute on the m table.</summary>
		public static Type GetTypeFromMType(string typeNameMobile,List<Type> typesReg) {
			if(typeNameMobile=="Userm") {
				return null;
			}
			string typeNameReg=typeNameMobile.Substring(0,typeNameMobile.Length-1);
			for(int i=0;i<typesReg.Count;i++) {
				if(typesReg[i].Name==typeNameReg) {
					return typesReg[i];
				}
			}
			throw new ApplicationException("Type not found.");
		}

		///<summary>Makes sure the tablename is valid.  Goes through each column and makes sure that the column is present and that the type in the database is a supported type for this C# data type.  Throws exception if it fails.</summary>
		public static void ValidateTypes(Type typeClass,string dbName) {
			string tablename=GetTableName(typeClass);
			string command="SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '"+dbName+"' AND table_name = '"+tablename+"'";
			if(DataCore.GetScalar(command)!="1"){
				return;//can't validate
			}
			command="SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS "
				+"WHERE table_name = '"+tablename+"' AND table_schema = '"+dbName+"'";
			DataTable table=DataCore.GetTable(command);
			//We also are going to check to make sure there are not any extra columns in the database that are not in the code.
			HashSet<string> setDbColumns=new HashSet<string>(table.Select().Select(x => x["COLUMN_NAME"].ToString()));
			FieldInfo[] fields=typeClass.GetFields();
			for(int i=0;i<fields.Length;i++){
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				ValidateColumn(dbName,tablename,fields[i],table);
				setDbColumns.Remove(fields[i].Name);
			}
			if(setDbColumns.Count > 0) {
				throw new Exception("Table "+tablename+" has columns that are not a part of its corresponding TableType class: "
					+string.Join(", ",setDbColumns));
			}
		}

		public static void ValidateColumn(string dbName,string tablename,FieldInfo field,DataTable table) {
			#region Oracle Removed
			//if(!tablename.In(//The ehrlab tables have already been released with long column names.  We might fix later.
			//	"ehrlab","ehrlabresult","ehrlabresultscopyto","ehrlabspecimencondition","ehrlabspecimenrejectreason",
			//	"triagemetric"))//Hq only table 
			//{				
			//	if(field.Name.Length>30) {
			//		throw new ApplicationException("Column name longer than 30 characters.  Invalid for Oracle.  Shorten the column name.  See "+tablename+"."+field.Name);
			//	}
			//}
			#endregion
			//make sure the column exists
			string dataTypeInDb="";
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["COLUMN_NAME"].ToString().ToLower()==field.Name.ToLower()){
					dataTypeInDb=table.Rows[i]["DATA_TYPE"].ToString();
				}
			}
			if(dataTypeInDb==""){
				return;//can't validate
			}
			CrudSpecialColType specialColType=GetSpecialType(field);
			string dataTypeExpected="";
			string dataTypeExpected2="";//if an alternate datatype is allowed
			string dataTypeExpected3="";
			if(specialColType.HasFlag(CrudSpecialColType.TimeStamp)) {
				dataTypeExpected="timestamp";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
				dataTypeExpected="bigint";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.DateEntry)) {
				dataTypeExpected="date";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
				dataTypeExpected="date";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.DateT)) {
				dataTypeExpected="datetime";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.DateTEntry)) {
				dataTypeExpected="datetime";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
				dataTypeExpected="datetime";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.TinyIntSigned)) {
				dataTypeExpected="tinyint";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.EnumAsString)) {
				dataTypeExpected="varchar";
			}
			else if(specialColType.HasFlag(CrudSpecialColType.TextIsClob)) {
				dataTypeExpected="text";
				dataTypeExpected2="mediumtext";
				dataTypeExpected3="longtext";
			}
			else if(field.FieldType.IsEnum) {
				dataTypeExpected="tinyint";
				dataTypeExpected2="int";
				dataTypeExpected3="smallint";
			}
			else switch(field.FieldType.Name) {
				default:
					throw new ApplicationException("Type not yet supported: "+field.FieldType.Name);
				case "Bitmap":
					dataTypeExpected="mediumtext";
					dataTypeExpected2="text";//only for very small images
					break;
				case "Boolean":
					dataTypeExpected="tinyint";
					break;
				case "Byte":
					dataTypeExpected="tinyint";
					break;
				case "Color":
					dataTypeExpected="int";
					break;
				case "DateTime":
					dataTypeExpected="date";//If the mysql field is datetime, then the C# field should have an [attribute] describing the type. 
					break;
				case "Double":
					dataTypeExpected="double";
					break;
				case "Interval":
					dataTypeExpected="int";
					break;
				case "Int64":
					dataTypeExpected="bigint";
					break;
				case "Int32":
					//use C# int for ItemOrder style fields.  We know they will not use random keys.
					dataTypeExpected="int";
					dataTypeExpected2="smallint";//ok as long as the coding is careful.  Less than ideal.
					//tinyint not allowed.  Possibly change C# type to byte if values can be between 0 and 255 with no negatives.
					//We might some day use SByte for values that can be -127 to 127.  Example, perio depths, percentages that allow -1, etc.  For now, those are smallint.
					break;
				case "Single":
					dataTypeExpected="float";//not 1:1, but we never use the full range anyway.
					dataTypeExpected2="float unsigned";
					break;
				case "String":
					dataTypeExpected="varchar";
					dataTypeExpected3="char";
					//text, mediumtext, or longtext should be marked TextIsClob
					break;
				case "TimeSpan":
					dataTypeExpected="time";
					break;
			}
			if(!ListTools.In(dataTypeInDb,dataTypeExpected,dataTypeExpected2,dataTypeExpected3)) {
				throw new Exception(tablename+"."+field.Name+" type mismatch.  Look in the lines of code above for case \""+field.FieldType.Name
					+"\".  The types listed are what is allowed in the mysql database.  "+dataTypeInDb+" is not one of the allowed mysql types.");
			}
		}

		///<summary>Scans the convert scripts for preferences that are not present in the database and addes them.</summary>
		///<param name="logFileName">If this is blank, will display the results in a MessageBox, otherwise writes the results to the file.</param>
		public static void AddMissingPreferences(string convertDbFileName,string logFileName="") {
			//This method is not exhaustive. I didn't bother making this script work for preferences that were added before version 14.
			List<PrefName> listPrefs=Enum.GetValues(typeof(PrefName)).OfType<PrefName>().Where(x => !ListTools.In(x,
				//HQ only prefs
				PrefName.AsteriskServerHeartbeat,
				PrefName.AsteriskServerIp,
				PrefName.CustListenerConnectionRequestTimeoutMS,
				PrefName.CustListenerHeartbeatFrequencyMinutes,
				PrefName.CustListenerPort,
				PrefName.CustListenerSocketTimeoutMS,
				PrefName.CustListenerTransmissionTimeoutMS,
				PrefName.HQTriageCoordinator,
				PrefName.VoiceMailMonitorHeartBeat,
				PrefName.VoiceMailDeleteAfterDays,
				PrefName.WebCamFrequencyMS,
				PrefName.StopWebCamSnapshot,
				PrefName.VoiceMailArchivePath,
				PrefName.VoiceMailCreatePath,
				PrefName.VoiceMailOriginationPath,
				PrefName.VoiceMailSMB2UserName,
				PrefName.VoiceMailSMB2Password,
				PrefName.VoiceMailSMB2Enabled,
				PrefName.CustomersHQDatabase,
				PrefName.CustomersHQMySqlPassHash,
				PrefName.CustomersHQMySqlUser,
				PrefName.CustomersHQServer,
				PrefName.TriageCalls,
				PrefName.TriageRedCalls,
				PrefName.TriageRedTime,
				PrefName.TriageTime,
				PrefName.TriageTimeWarning,
				PrefName.VoicemailCalls,
				PrefName.VoicemailTime,
				PrefName.IntrospectionItems,
				PrefName.AsteriskConferenceApplication,
				PrefName.AsteriskHighVolumeMode,
				PrefName.ServicesHqDatabase,
				PrefName.ServicesHqDoNotConnect,
				PrefName.ServicesHqMySqlUser,
				PrefName.ServicesHqMySqpPasswordObf,
				PrefName.ServicesHqServer,
				//Deprecated prefs
				PrefName.ScannerCompression,
				PrefName.RequiredFieldColor,
				//Secret prefs
				PrefName.CloudSessionLimitCap,
				PrefName.UpdateAlterLargeTablesDirectly,
				PrefName.UpdateStreamLinePassword,
				//Clinicpref only prefs
				PrefName.PatientPortalInviteUseDefaults,
				//Not really used
				PrefName.CentralManagerPassSalt,
				PrefName.NotApplicable
				)).ToList();
			List<string> listLines=null;
			List<PrefName> listPrefsAdded=new List<PrefName>();
			List<PrefName> listPrefsUnableToAdd=new List<PrefName>();
			foreach(PrefName pref in listPrefs) {
				if(Prefs.GetContainsKey(pref.ToString())) {
					continue;
				}
				if(listLines==null) {
					listLines=GetConvertScriptLines(convertDbFileName);
				}
				Regex insertRegex=new Regex(@"INSERT INTO preference(\s*)\(PrefName,(\s*)ValueString\) (VALUES(\s*)\(|SELECT(\s*))'"+pref.ToString()+"'");
				int lineIndex=listLines.FindIndex(x => insertRegex.IsMatch(x)
					|| x.Contains("UPDATE preference SET PrefName='"+pref.ToString()));
				if(lineIndex==-1) {
					listPrefsUnableToAdd.Add(pref);
					continue;
				}
				string prefCommandLine=listLines[lineIndex];
				while(!prefCommandLine.Contains(";")) {
					prefCommandLine+=listLines[++lineIndex];
				}
				string prefCommand=prefCommandLine.Trim();
				if(prefCommand.StartsWith("string")){
					prefCommand=prefCommand.Substring("string command=".Length).TrimStart('"','@','$');
				}
				else {
					prefCommand=prefCommand.Substring("command=".Length).TrimStart('"','@','$');
				}
				int endOfCommand=prefCommand.IndexOf("')\";")+2;
				if(endOfCommand==1) {
					endOfCommand=prefCommand.IndexOf(")\";")+1;//E.g. command="INSERT INTO preference(PrefName,ValueString) VALUES('RadiologyDateStartedUsing154',"+POut.DateT(DateTime.Now.Date)+")";
					if(endOfCommand==0) {
						endOfCommand=prefCommand.IndexOf("'\";")+1;//E.g. command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistExamCodes',ValueString FROM preference WHERE PrefName='InsBenExamCodes'";
					}
					if(endOfCommand==0) {
						endOfCommand=prefCommand.IndexOf(";\";");//E.g. command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmExcludeEThankYou','');";
					}
				}
				if(prefCommand.StartsWith("UPDATE preference SET PrefName='")) {
					endOfCommand=prefCommand.IndexOf(";")-1;//Eg. command="UPDATE preference SET PrefName='InsChecksFrequency' WHERE PrefName='InsTpChecksFrequency'";
				}
				prefCommand=prefCommand.Substring(0,endOfCommand);
				DataCore.NonQ(prefCommand);
				listPrefsAdded.Add(pref);
			}
			string unableToAdd=string.Join("\r\n",listPrefsUnableToAdd.Select(x => x.ToString()));
			string prefsAdded=string.Join("\r\n",listPrefsAdded.Select(x => x.ToString()));
			string message="";
			if(unableToAdd != "") {
				message+="Unable to add these preferences:\r\n"+unableToAdd+"\r\n\r\n";
			}
			if(prefsAdded != "") {
				message+="Preferences added:\r\n"+prefsAdded+"\r\n";
			}
			if(message=="") {
				message+="No preferences added.\r\n";
			}
			message+="Done";
			if(string.IsNullOrEmpty(logFileName)) {
				MessageBox.Show(message);
			}
			else {
				File.AppendAllText(logFileName,message+"\r\n");
			}
			Prefs.RefreshCache();
		}

		private static List<string> GetConvertScriptLines(string convertDbFileName) {
			List<string> listLines=new List<string>();
			listLines.AddRange(File.ReadAllLines(convertDbFileName));
			int convertScriptNum=PIn.Int(string.Join("",Path.GetFileName(convertDbFileName).Where(x => char.IsDigit(x))),false);
			while(--convertScriptNum > 0) {
				convertDbFileName=Path.GetDirectoryName(convertDbFileName)+Path.DirectorySeparatorChar+"ConvertDatabases"+convertScriptNum+".cs";
				listLines.AddRange(File.ReadAllLines(convertDbFileName));
			}
			return listLines;
		}

		public static void UpdateDatabase(string database,string server,string user,string password) {
			string convertDbFile=Path.Combine(Application.StartupPath,Form1.convertDbFile);
			string logFileName=Path.Combine(Application.StartupPath,"CrudGeneratorLog.txt");
			try {
				ConnectToDatabase(database,server,user,password);
				File.AppendAllText(logFileName,"-------------------Updating database '"+database+"' on "+DateTime.Now.ToString()+"-----------------------------"
					+"--\r\n");
				List<Type> tableTypes=new List<Type>();
				Type typeTableBase=typeof(TableBase);
				Assembly assembly=Assembly.GetAssembly(typeTableBase);
				StringBuilder results=new StringBuilder();
				foreach(Type typeClass in assembly.GetTypes()) {
					if(typeClass.IsSubclassOf(typeTableBase) && !IsMobile(typeClass) && !typeClass.Name.StartsWith("WebForms_")) {
						CrudQueries.Write(convertDbFile,typeClass,database,false,true,false,results);
					}
				}
				File.AppendAllText(logFileName,results.ToString());
				AddMissingPreferences(convertDbFile,logFileName);
			}
			catch(Exception ex) {
				File.AppendAllText(logFileName,"Error: "+ex.Message+"\r\nStack Trace:\r\n"+ex.StackTrace+"\r\n\r\nTime of Occurrence: "+DateTime.Now+"\r\n");
			}
		}

		///<summary>Adds the file to Subversion.</summary>
		public static void AddToSubversion(string fileName) {
			string command="/C svn add \""+fileName+"\"";
			ProcessStartInfo startInfo=new ProcessStartInfo("cmd.exe",command);
			startInfo.WindowStyle=ProcessWindowStyle.Hidden;
			Process process=new Process();
			process.StartInfo=startInfo;
			process.Start();
			process.WaitForExit();
		}

		///<summary>Adds these files to the OpenDentBusiness project.  If the file is already a part of OpenDentBusiness, no harm, no foul.</summary>
		public static void AddToOpenDentBusiness(List<string> listFileNames) {
			if(listFileNames.Count==0) {
				return;
			}
			Project proj=new Project(Path.Combine(Form1.crudDir,"..","OpenDentBusiness.csproj"));
			foreach(string file in listFileNames) {
				if(proj.Items.FirstOrDefault(x => x.EvaluatedInclude==file)==null) {//Check to make sure the file doesn't already exist.
					proj.AddItem("Compile",file);
				}
			}
			proj.Save();
		}

	}
}
