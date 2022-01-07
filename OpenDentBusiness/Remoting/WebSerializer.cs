using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;

//This file is used in conjunction with and must match WebServiceCustomerUpdates\WebSerialize.cs.
namespace WebServiceSerializer {
	///<summary>Used to serialize primitives for WebServiceCustUpdates I/O.</summary>
	public static class WebSerializer {

		///<summary>If the delimiter character is found in a given cell, then the cell's value will be updated to include the place holder value in lieu of the delimiter. This ensures that the delimiter is reserved for only delimiting cells. The place holder will be replaced by the delimiter value on the other end once the cells have been properly delimited.</summary>
		private const string _cellDelimiterPlaceHolder="zzzzzzzzzz";
		///<summary>This value is reserved strictly for delimiting cells in a serialized data row.</summary>
		private const string _cellDelimiter="~";
		///<summary>Format necessary for C#/Java date/time.</summary>
		private const string DotNetDateTimeFormat="yyyy-MM-dd HH:mm:ss";
		///<summary>Format necessary for MySql date/time.</summary>
		private const string MySqlDateTimeFormat="%Y-%m-%d %H:%i:%s";

		///<summary>Format is hard-coded to: %Y-%m-%d %H:%i:%s which is the MySQL version of the C#/Java serializing of a date/time.</summary>
		private static string DateFormatColumnForMySql(string colName) {
			return "DATE_FORMAT("+colName+",'"+MySqlDateTimeFormat+"')";
		}

		///<summary>Returns our most commonly used XmlWriterSettings.</summary>
		public static XmlWriterSettings CreateXmlWriterSettings(bool omitXmlDeclaration) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			settings.OmitXmlDeclaration=omitXmlDeclaration;
			return settings;
		}

		///<summary>Escapes common characters used in XML from the passed in String.</summary>
		private static string EscapeForXml(string myString) {
			if(string.IsNullOrEmpty(myString)) {
				return "";
			}
			StringBuilder strBuild=new StringBuilder();
			int length=myString.Length;
			for(int i=0;i<length;i++) {
				String character=myString.Substring(i,1);
				if(character.Equals("<")) {
					strBuild.Append("&lt;");
					continue;
				}
				else if(character.Equals(">")) {
					strBuild.Append("&gt;");
					continue;
				}
				else if(character.Equals("\"")) {
					strBuild.Append("&quot;");
					continue;
				}
				else if(character.Equals("\'")) {
					strBuild.Append("&#039;");
					continue;
				}
				else if(character.Equals("&")) {
					strBuild.Append("&amp;");
					continue;
				}
				strBuild.Append(character);
			}
			return strBuild.ToString();
		}

		private static string EscapeForXmlCustom(String myString) {
			if(string.IsNullOrEmpty(myString)) {
				return "";
			}
			StringBuilder strBuild=new StringBuilder();
			int length=myString.Length;
			for(int i=0;i<length;i++) {
				string character=myString.Substring(i,1);
				if(character=="<") {
					strBuild.Append("[[60]]");
					continue;
				}
				else if(character==">") {
					strBuild.Append("[[62]]");
					continue;
				}
				else if(character=="\"") {
					strBuild.Append("[[34]]");
					continue;
				}
				else if(character=="\'") {
					strBuild.Append("[[39]]");
					continue;
				}
				else if(character=="&") {
					strBuild.Append("[[38]]");
					continue;
				}
				strBuild.Append(character);
			}
			return strBuild.ToString();
		}

		private static string EscapeForURL(String myString) {
			if(string.IsNullOrEmpty(myString)) {
				return "";
			}
			StringBuilder strBuild=new StringBuilder();
			int length=myString.Length;
			for(int i=0;i<length;i++) {
				String character=myString.Substring(i,i+1);
				if(character.Equals("<")) {
					strBuild.Append("%3C");
					continue;
				}
				else if(character.Equals(">")) {
					strBuild.Append("%3E");
					continue;
				}
				else if(character.Equals("&")) {
					strBuild.Append("%26");
					continue;
				}
				strBuild.Append(character);
			}
			return strBuild.ToString();
		}

		private static string ReplaceEscapes(string myString) {
			if(string.IsNullOrEmpty(myString)) {
				return "";
			}
			StringBuilder processedXml=new StringBuilder();
			for(int i=0;i<myString.Length;i++) {
				//if at any point this char is not a match then ONLY append the start char, then continue
				//every continue should be accompanied by processedXml.Append(startChar)
				//search for consecutive [[ to open the special char indicator
				string startChar=myString.Substring(i,1);
				if(startChar!="[") {
					processedXml.Append(startChar);
					continue;
				}
				string nextChar=myString.Substring(i+1,1);
				if(nextChar!="[") {
					processedXml.Append(startChar);
					continue;
				}
				//search for the consecutive ]] to close the special char indicator
				string remaining=myString.Substring(i,myString.Length-i);
				int endsAt=remaining.IndexOf("]]");
				if(endsAt<0) { //make sure the special char is closed before the end of this xml tag
					processedXml.Append(startChar);
					continue;
				}
				//we have a good special char to translate it, append it, and set the new index location
				//get the guts of the special char
				string specialChar=remaining.Substring(2,remaining.IndexOf("]]")-2);
				//convert to asci
				int asciiAsInt;
				if(!int.TryParse(specialChar,out asciiAsInt)) { //not a valid ascii value
					processedXml.Append(startChar);
					continue;
				}
				//append the ascii char as a string
				processedXml.Append(Char.ConvertFromUtf32(asciiAsInt));
				//set the new index location, we have skipped a good chunk... [[123]]
				i+=(endsAt+1);
			}
			return processedXml.ToString();
		}

		/// <summary>Works in conjunction with DeserializePrimitive. Typically used to pass single primitives back and forth between web services.</summary>
		public static string SerializePrimitive<T>(T obj) {
			return SerializeForCSharp(typeof(T).ToString(),obj);
		}

		///<summary>Goes through all the possible types of objects and returns the object serialized for Java.  objectType must be fully qualified.  Ex: System.Int32.  For DataTables, set objectType to "DataTable".  Returns an empty node if the object is null.  Throws exceptions.</summary>
		public static string SerializeForCSharp(string objectType,Object obj) {
			if(obj==null) {
				return "<"+objectType+"/>";//Return an empty node?
			}
			if(obj.GetType().IsEnum) { //Serialize value as int.
				return "<"+objectType+">"+POut.PInt((int)obj)+"</"+objectType+">";
			}
			//Primitives--------------------------------------------------------------------
			if(objectType=="System.Int32" || objectType=="int") {
				return "<int>"+POut.PInt((int)obj)+"</int>";
			}
			if(objectType=="System.Int64" || objectType=="long") {
				return "<long>"+Convert.ToInt64(((long)obj)).ToString()+"</long>";
			}
			if(objectType=="System.Boolean" || objectType=="bool") {
				return "<bool>"+POut.PBool((bool)obj)+"</bool>";
			}
			if(objectType=="System.String" || objectType=="string") {
				return "<string>"+EscapeForXml((string)obj)+"</string>";
			}
			if(objectType=="System.Char" || objectType=="char") {
				return "<char>"+Convert.ToChar((char)obj).ToString()+"</char>";
			}
			if(objectType=="System.Single" || objectType=="Single") {
				return "<float>"+POut.PFloat((float)obj)+"</float>";
			}
			if(objectType=="System.Byte" || objectType=="byte") {
				return "<byte>"+POut.PByte((byte)obj)+"</byte>";
			}
			if(objectType=="System.Double" || objectType=="double") {
				return "<double>"+POut.PDouble((double)obj)+"</double>";
			}
			if(objectType.StartsWith("List")) {//Lists.
				return SerializeList(objectType,obj);
			}
			if(objectType.Contains("[")) {//Arrays.
				return SerializeArray(objectType,obj);
			}
			//DateTime----------------------------------------------------------------------
			if(objectType=="DateTime") {
				return "<DateTime>"+((DateTime)obj).ToString(DotNetDateTimeFormat)+"</DateTime>";
			}
			//DataTable---------------------------------------------------------------------
			if(objectType=="DataTable") {
				return SerializeDataTable((DataTable)obj);
			}
			//DataSet-----------------------------------------------------------------------
			if(objectType=="DataSet") {
				return SerializeDataSet((DataSet)obj);
			}
			throw new NotSupportedException("SerializeForCSharp, unsupported class type: "+objectType);
		}

		///<summary>Returns the primitive or general object deserialized.  Throws exception.</summary>
		public static object Deserialize(string typeName,string xml) {
			//Handle enums special.
			Type type=null;
			try { type=Type.GetType(typeName); }
			catch { /* We couldn't get the typeIn from the typeName so it won't be an enum. Swallow this error. */}
			if(type!=null) { //Our input type is an enum so deserialize it according to what type of enum and if it was serialized as a string or an int.
				if(type.IsEnum) {
					using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
						if(reader.Read()) { //Value was serialized by int.
							return Enum.ToObject(type,PIn.PInt(reader.ReadString()));
						}
					}
				}
				else if(type.IsGenericType && type.GetGenericTypeDefinition()==typeof(List<>)) {
					Type listItemType=type.GetGenericArguments()[0];
					return DeserializeList(xml);
				}
			}
			try {
				using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
					while(reader.Read()) {//In a loop just in case there is whitespace and it needs to read more than once.  Shouldn't need to loop more than once.
						//Primitives--------------------------------------------------------------------
						switch(reader.Name.ToLower()) { //Comes from xWebAppsCodeGenerator\GenerateTableTypes.GetSerializableTypeName().
							case "int":
							case "int32":
								return PIn.PInt(reader.ReadString());
							case "long":
							case "int64":
								return PIn.PLong(reader.ReadString());
							case "bool":
							case "boolean":
								return PIn.PBool(reader.ReadString());
							case "string":
								return PIn.PString(reader.ReadString());
							case "char":
								return Convert.ToChar(reader.ReadString());
							case "float":
								return PIn.PFloat(reader.ReadString());
							case "byte":
								return PIn.PByte(reader.ReadString());
							case "double":
								return PIn.PDouble(reader.ReadString());
							case "datetime": //Format matters here. Java put it in this format in Serializing.getSerializedObject().
								return DateTime.ParseExact(reader.ReadString(),DotNetDateTimeFormat,null);
							case "datatable":
								return DeserializeDataTable(reader.ReadOuterXml());
							case "dataset":
								return DeserializeDataSet(reader.ReadOuterXml());
						}
						//Arrays------------------------------------------------------------------------
						if(typeName.Contains("[")) {
							//TODO: This will need to be enhanced to handle simple and possibly multidimensional arrays.
							throw new Exception("Multidimensional arrays not supported");
						}
					}
				}
			}
			catch(Exception e) {
				//Deserializing known type failed.
				string context="Deserialize, error deserializing primitive or general type: "+typeName+"\r\n"+xml;
				throw e;
			}
			//Type must not be supported yet.
			Exception ex=new Exception("Deserialize, unsupported primitive or general type: "+typeName);
			throw ex;
		}

		/// <summary>Works in conjunction with SerializePrimitive. Typically used to pass single primitives back and forth between web services.</summary>
		private static List<T> DeserializePrimitiveList<T>(string xml) {
			List<object> ret=(List<object>)Deserialize(typeof(List<T>).ToString(),xml);
			return ret.Cast<T>().ToList();
		}

		/// <summary>Works in conjunction with SerializePrimitive. Typically used to pass single primitives back and forth between web services.</summary>
		public static T DeserializePrimitive<T>(string xml) {
			return (T)Deserialize(typeof(T).ToString(),xml);
		}
		
		///<summary>Searches through resultXml for the given tagName and returns a deserialized object.
		///Specifically looks for an Error node and throws an exception with the InnerText of said node if found.</summary>
		public static T DeserializeTag<T>(string resultXml,string tagName) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(resultXml);
			//Validate output.
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				throw new Exception(node.InnerText);
			}
			node=doc.SelectSingleNode("//"+tagName);
			if(node==null) {
				throw new Exception("tagName node not found: "+tagName);
			}
			T retVal;
			using(XmlReader reader=XmlReader.Create(new StringReader(node.InnerXml))) {
				XmlSerializer serializer=new XmlSerializer(typeof(T));
				retVal=(T)serializer.Deserialize(reader);
			}
			if(retVal==null) {
				throw new Exception("tagName node invalid: "+tagName);
			}
			return retVal;
		}

		///<summary>Returns the inner text for the node with the given nodeName.</summary>
		public static string DeserializeNode(string xml,string nodeName,bool doThrowIfNotFound=true) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xml);
			XmlNode node=doc.SelectSingleNode("//"+nodeName);
			if(node==null) {
				if(!doThrowIfNotFound) {
					return "";
				}
				throw new ODException("Node not found: "+nodeName);
			}
			return node.InnerText;
		}

		///<summary>Parse the xml and look for a node called 'Error'. If found then throw the node's InnerText.</summary>
		private static void ParseErrorAndThrow(string xml) {
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				reader.MoveToContent();
				while(reader.Read()) {
					//Only detect start elements.
					if(!reader.IsStartElement()) {
						continue;
					}
					//save field name and move to the value
					string fieldName=reader.Name;
					reader.Read();
					switch(fieldName) {
						case "Error":
							throw new Exception(ReplaceEscapes(reader.ReadContentAsString()));
					}
				}
			}
		}

		/// <summary>Works in conjunction with SerializePrimitive. Typically used to pass single primitives back and forth between web services.</summary>
		public static List<T> DeserializePrimitiveListOrThrow<T>(string xml) {
			ParseErrorAndThrow(xml);
			return DeserializePrimitiveList<T>(xml);
		}

		/// <summary>Works in conjunction with SerializePrimitive. Typically used to pass single primitives back and forth between web services.</summary>
		public static T DeserializePrimitiveOrThrow<T>(string xml) {
			ParseErrorAndThrow(xml);
			return (T)Deserialize(typeof(T).ToString(),xml);
		}

		private static DataTable DeserializeDataTable(string xml) {
			DataTable dataTable=new DataTable();
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				if(!reader.ReadToFollowing("Name")) {
					throw new Exception("Name tag not found");
				}
				dataTable.TableName=ReplaceEscapes(reader.ReadString());
				while(reader.Read()) {
					if(!reader.IsStartElement()) {
						continue;
					}
					if(reader.Name=="") {
						continue;
					}
					if(reader.Name=="Cols") {
						continue;
					}
					if(reader.Name=="Col") { //new column header
						dataTable.Columns.Add(ReplaceEscapes(reader.ReadString()));
						continue;
					}
					if(reader.Name=="Cells") { //starting rows
						continue;
					}
					if(reader.Name=="y") { //new row						
						DataRow row=dataTable.NewRow();
						string pipedRow=reader.ReadString();
						string[] cells=pipedRow.Split(new string[1] { _cellDelimiter },StringSplitOptions.None);
						if(cells.Length==dataTable.Columns.Count) {
							for(int i=0;i<cells.Length;i++) {
								cells[i]=ReplaceEscapes(cells[i].Replace(_cellDelimiterPlaceHolder,_cellDelimiter));
							}
							row.ItemArray=cells;
							dataTable.Rows.Add(row);
						}
						continue;
					}
				}
			}
			return dataTable;
		}

		///<summary>Helper function that will serialize a data table by looping through the rows and columns.</summary>
		private static string SerializeDataTable(DataTable table) {
			StringBuilder result=new StringBuilder();
			result.Append("<DataTable>");
			//Table name.
			result.Append("<Name>").Append(table.TableName).Append("</Name>");
			//Column names.
			result.Append("<Cols>");
			for(int i=0;i<table.Columns.Count;i++) {
				result.Append("<Col>").Append(table.Columns[i].ColumnName).Append("</Col>");
			}
			result.Append("</Cols>");
			//Set each cell by looping through each column row by row.
			result.Append("<Cells>");
			for(int i=0;i<table.Rows.Count;i++) {//Row loop.
				result.Append("<y>");
				for(int j=0;j<table.Columns.Count;j++) {//Column loop.
					string cellValue=table.Rows[i][j].ToString();
					if(table.Columns[j].DataType.Name=="DateTime") { //DateTime requires special formatting so it can be deserialized by java in DataTable.getCellDateFromFormatString().
						DateTime dt;
						if(!DateTime.TryParse(table.Rows[i][j].ToString(),out dt)) { //Shouldn't get here but just in case, give it a default value.
							dt=new DateTime(1,1,1);
						}
						cellValue=dt.ToString(DotNetDateTimeFormat);
					}
					//Add the formatted string.
					result.Append(EscapeForXml(cellValue).Replace(_cellDelimiter,_cellDelimiterPlaceHolder));
					if(j<table.Columns.Count-1) {
						result.Append(_cellDelimiter);
					}
				}
				result.Append("</y>");
			}
			result.Append("</Cells>");
			result.Append("</DataTable>");
			return result.ToString();
		}

		private static DataSet DeserializeDataSet(string xml) {
			DataSet dataSet=new DataSet();
			string typeName="";
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				reader.MoveToContent();//Moves to root node, <List>.
				reader.Read();//Moves to first objects node, this will be the type of the object list.
				typeName=reader.Name;//Should be "DataTable"
				if(typeName=="DataSet") {//Can happen if passed an empty data set.
					return dataSet;
				}
				do {
					dataSet.Tables.Add(DeserializeDataTable(reader.ReadOuterXml()));
				} while(reader.Name==typeName);
			}
			return dataSet;
		}

		///<summary>Helper function that will serialize a data set.</summary>
		private static string SerializeDataSet(DataSet ds) {
			StringBuilder strb=new StringBuilder();
			strb.Append("<DataSet>");
			strb.Append("<DataTables>");
			for(int i=0;i<ds.Tables.Count;i++) {
				strb.Append(SerializeDataTable(ds.Tables[i]));
			}
			strb.Append("</DataTables>");
			strb.Append("</DataSet>");
			return strb.ToString();
		}

		///<summary>Pass in the type of list and the list object and this method will serialize it.  The object within the list must be fully qualified.</summary>
		public static string SerializeList<T>(List<T> list) {
			return SerializeList("List[[60]]"+typeof(T).Name+"[[62]]",list);
		}

		///<summary>Pass in the type of list and the list object and this method will serialize it.  The object within the list must be fully qualified.</summary>
		public static string SerializeList(string objectType,Object obj) {
			string listType="";
			//Strip out what kind of objects this list contains.
			Match m=Regex.Match(objectType,@"^List\[\[60\]\]([a-zA-Z0-9._%+-]*)\[\[62\]\]$");
			if(!m.Success) {
				throw new Exception("SerializeList, unknown object list: "+objectType);
			}
			listType=m.Result("$1");
			//Cast to a list of objects and loop through all the objects and call each objects corresponding serialize method.
			StringBuilder result=new StringBuilder();
			result.Append("<List>");
			IEnumerable enumerable=obj as IEnumerable;
			if(enumerable!=null) {
				foreach(object item in enumerable) {
					result.Append(SerializeForCSharp(listType,item));
				}
			}
			result.Append("</List>");
			return result.ToString();
		}

		///<summary>Pass in the type of list and the list object and this method will serialize it.  The object within the list must be fully qualified.</summary>
		public static List<object> DeserializeList(string xml) {
			List<object> listObject=new List<object>();
			string typeName="";
			//Find out what type of list this is.
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				reader.MoveToContent();//Moves to root node, <List>.
				reader.Read();//Moves to first objects node, this will be the type of the object list.
				typeName=reader.Name;
				if(typeName=="List") {//Can happen if passed an empty list.
					return listObject;
				}
				do {
					listObject.Add(Deserialize(typeName,reader.ReadOuterXml()));
				} while(reader.Name==typeName);
			}
			return listObject;
		}

		///<summary>Helper function that will serialize any array.</summary>
		public static string SerializeArray(string typeName,Object obj) {
			//TODO Enhance to handle serializing arrays.
			throw new Exception("SerializeArray, arrays not supported yet.");
		}

		private static string GetNodeNameFromType(Type t) {
			return t.Name+"Xml";
		}

		public static T ReadXml<T>(string xml) where T : new() {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xml);
			XmlNode node=doc.SelectSingleNode("//"+GetNodeNameFromType(typeof(T)));
			if(node==null) {
				throw new ApplicationException(GetNodeNameFromType(typeof(T))+" node not present.");
			}
			T ret=default(T); XmlSerializer serializer=new XmlSerializer(typeof(T));
			using(XmlReader reader = XmlReader.Create(new System.IO.StringReader(node.InnerXml))) {
				ret=(T)serializer.Deserialize(reader);
			}
			if(ret==null) {
				ret=new T();
			}
			return ret;
		}

		public static string WriteXml<T>(T input) {
			XmlSerializer serializer=new XmlSerializer(typeof(T)); StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer = XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(true))) {
				writer.WriteStartElement(GetNodeNameFromType(typeof(T)));
				serializer.Serialize(writer,input);
				writer.WriteEndElement();
			}
			return strbuild.ToString();
		}
	}

	///<summary>Converts various datatypes into strings formatted correctly for MySQL. "P" was originally short for Parameter because this class was written specifically to replace parameters in the mysql queries. Using strings instead of parameters is much easier to debug.  This will later be rewritten as a System.IConvertible interface on custom mysql types.  I would rather not ever depend on the mysql connector for this so that this program remains very db independent.
	///Marked internal so it doesn't get mistaken or misused in place of OpenDentBusiness.POut.</summary>
	internal class POut {

		///<summary></summary>
		public static string PBool(bool myBool) {
			if(myBool==true) {
				return "1";
			}
			else {
				return "0";
			}
		}

		///<summary></summary>
		public static string PByte(byte myByte) {
			return myByte.ToString();
		}

		///<summary>Always encapsulates the result, depending on the current database connection.</summary>
		public static string PDateT(DateTime myDateT) {
			return PDateT(myDateT,true);
		}

		///<summary></summary>
		public static string PDateT(DateTime myDateT,bool encapsulate) {
			if(myDateT.Year<1880) {
				myDateT=DateTime.MinValue;
			}
			try {
				string outDate=myDateT.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture);//new DateTimeFormatInfo());
				string frontCap="'";
				string backCap="'";
				//if(DataConnection.DBtype==DatabaseType.Oracle) {
				//	frontCap="TO_DATE('";
				//	backCap="','YYYY-MM-DD HH24:MI:SS')";
				//}
				if(encapsulate) {
					outDate=frontCap+outDate+backCap;
				}
				return outDate;
			}
			catch {
				return "";//this saves zero's to the database
			}
		}

		///<summary>Converts a date to yyyy-MM-dd format which is the format required by MySQL. myDate is the date you want to convert. encapsulate is true for the first overload, making the result look like this: 'yyyy-MM-dd' for MySQL.</summary>
		public static string PDate(DateTime myDate) {
			return PDate(myDate,true);
		}

		public static string PDate(DateTime myDate,bool encapsulate) {
			if(myDate.Year<1880) {
				myDate=DateTime.MinValue;
			}
			try {
				//the new DTFormatInfo is to prevent changes in year for Korea
				string outDate=myDate.ToString("yyyy-MM-dd",new DateTimeFormatInfo());
				string frontCap="'";
				string backCap="'";
				//if(DataConnection.DBtype==DatabaseType.Oracle){
				//	frontCap="TO_DATE('";
				//	backCap="','YYYY-MM-DD')";
				//}
				if(encapsulate) {
					outDate=frontCap+outDate+backCap;
				}
				return outDate;
			}
			catch {
				//return "0000-00-00";
				return "";//this saves zeros to the database
			}
		}

		public static string PTimeSpan(TimeSpan myTimeSpan) {
			return PTimeSpan(myTimeSpan,true);
		}

		public static string PTimeSpan(TimeSpan myTimeSpan,bool encapsulate) {
			try {
				string outTimeSpan = myTimeSpan.ToString();
				string frontCap = "'";
				string backCap = "'";
				if(encapsulate) {
					outTimeSpan = frontCap + outTimeSpan + backCap;
				}
				return outTimeSpan;
			}
			catch {
				return "";//this saves zero's to the database
			}
		}

		///<summary></summary>
		public static string PDouble(double myDouble) {
			try {
				//because decimal is a comma in Europe, this sends it to db with period instead 
				return myDouble.ToString("f",new NumberFormatInfo());
			}
			catch {
				return "0";
			}
		}

		///<summary></summary>
		public static string PInt(int myInt) {
			return myInt.ToString();
		}

		///<summary></summary>
		public static string PLong(long myLong) {
			return myLong.ToString();
		}

		public static string PShort(short myShort) {
			return myShort.ToString();
		}

		///<summary></summary>
		public static string PFloat(float myFloat) {
			return myFloat.ToString();
		}

		///<summary></summary>
		public static string PString(string myString) {
			if(myString==null) {
				return "";
			}
			/*if(DataConnection.DBtype!=DatabaseType.MySql){
				if(myString.Contains(";")){
					myString=myString.Replace(";","");
				}
				if(myString.Contains("'")) {
					myString=myString.Replace("'","");
				}
				if(myString==null) {
					return "";
				}
			}*/
			StringBuilder strBuild=new StringBuilder();
			for(int i=0;i<myString.Length;i++) {
				switch(myString.Substring(i,1)) {
					//note. When using binary data, must escape ',",\, and nul(? haven't done nul)
					case "'": strBuild.Append(@"\'"); break;// ' replaced by \'
					case "\"": strBuild.Append("\\\""); break;// " replaced by \"
					case @"\": strBuild.Append(@"\\"); break;//single \ replaced by \\
					case "\r": strBuild.Append(@"\r"); break;//carriage return(usually followed by new line)
					case "\n": strBuild.Append(@"\n"); break;//new line
					case "\t": strBuild.Append(@"\t"); break;//tab
					default: strBuild.Append(myString.Substring(i,1)); break;
				}
			}
			//The old slow way of doing it:
			/*string newString="";
			for(int i=0;i<myString.Length;i++){
				switch (myString.Substring(i,1)){
					case "'": newString+=@"\'"; break;
					case @"\": newString+=@"\\"; break;//single \ replaced by \\
					case "\r": newString+=@"\r"; break;//carriage return(usually followed by new line)
					case "\n": newString+=@"\n"; break;//new line
					case "\t": newString+=@"\t"; break;//tab
						//case "%": newString+="\\%"; break;//causes errors because only ambiguous in LIKE clause
						//case "_": newString+="\\_"; break;//see above
					default : newString+=myString.Substring(i,1); break;
				}//end switch
			}//end for*/
			//MessageBox.Show(strBuild.ToString());
			return strBuild.ToString();
		}

		//<summary></summary>
		//public static string PTimee (string myTime){
		//	return DateTime.Parse(myTime).ToString("HH:mm:ss");
		//}

		/*
		///<summary></summary>
		public static string PBitmap(Bitmap bitmap) {
			if(bitmap==null){
				return "";
			}
			MemoryStream stream=new MemoryStream();
			bitmap.Save(stream,ImageFormat.Bmp);
			byte[] rawData=stream.ToArray();
			return Convert.ToBase64String(rawData);
		}*/

		///<summary></summary>
		public static string PBitmap(Bitmap bitmap) {
			if(bitmap==null) {
				return "";
			}
			MemoryStream stream=new MemoryStream();
			bitmap.Save(stream,ImageFormat.Bmp);
			byte[] rawData=stream.ToArray();
			return Convert.ToBase64String(rawData);
		}

		///<summary>Converts the specified wav file into a string representation.  The timing of this is a little different than with the other "P" functions and is only used by the import button in FormSigElementDefEdit.  After that, the wav spends the rest of it's life as a string until "played" or exported.</summary>
		public static string PSound(string filename) {
			if(!File.Exists(filename)) {
				throw new ApplicationException("File does not exist.");
			}
			if(!filename.EndsWith(".wav")) {
				throw new ApplicationException("Filename must end with .wav");
			}
			FileStream stream=new FileStream(filename,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
			byte[] rawData=new byte[stream.Length];
			stream.Read(rawData,0,(int)stream.Length);
			return Convert.ToBase64String(rawData);
		}

		///<summary>The supplied string should already be in safe base64 format, and should not need any special escaping.  The purpose of this function is to enforce that the supplied string meets these requirements.  This is done quickly.</summary>
		public static string Base64(string myString) {
			if(myString==null) {
				return "";
			}
			if(!Regex.IsMatch(myString,"[A-Z0-9]*")) {
				throw new ApplicationException("Characters found that do not match base64 format.");
			}
			return myString;
		}

	}

	/*=========================================================================================
=================================== class PIn ===========================================*/
	///<summary>Converts strings coming in from the database into the appropriate type. "P" was originally short for Parameter because this class was written specifically to replace parameters in the mysql queries. Using strings instead of parameters is much easier to debug.  This will later be rewritten as a System.IConvertible interface on custom mysql types.  I would rather not ever depend on the mysql connector for this so that this program remains very db independent.
	///Marked internal so it doesn't get mistaken or misused in place of OpenDentBusiness.PIn.</summary>
	internal class PIn {
		///<summary></summary>
		public static bool PBool(string myString) {
			return myString=="1";
		}

		///<summary></summary>
		public static byte PByte(string myString) {
			if(myString=="") {
				return 0;
			}
			else {
				return System.Convert.ToByte(myString);
			}
		}

		///<summary></summary>
		public static DateTime PDate(string myString) {
			if(myString=="")
				return DateTime.MinValue;
			try {
				return (DateTime.Parse(myString));
				//return DateTime.Parse(myString,CultureInfo.InvariantCulture);
			}
			catch {
				return DateTime.MinValue;
			}
		}

		///<summary></summary>
		public static DateTime PDateT(string myString) {
			if(myString=="")
				return DateTime.MinValue;
			//if(myString=="0000-00-00 00:00:00")//useless
			//	return DateTime.MinValue;
			try {
				return (DateTime.Parse(myString));
			}
			catch {
				return DateTime.MinValue;
			}
		}

		public static TimeSpan PTimeSpan(string myString) {
			if(string.IsNullOrEmpty(myString)) {
				return TimeSpan.MinValue;
			}
			try {
				return (TimeSpan.Parse(myString));
			}
			catch {
				return TimeSpan.MinValue;
			}
		}

		///<summary>If blank or invalid, returns 0. Otherwise, parses.</summary>
		public static double PDouble(string myString) {
			if(myString=="") {
				return 0;
			}
			else {
				try {
					return System.Convert.ToDouble(myString);
				}
				catch {
					//MessageBox.Show("Error converting "+myString+" to double");
					return 0;
				}
			}

		}

		///<summary></summary>
		public static int PInt(string myString) {
			if(myString=="") {
				return 0;
			}
			else {
				return System.Convert.ToInt32(myString);
			}
		}

		///<summary></summary>
		public static long PLong(string myString) {
			if(myString=="") {
				return 0;
			}
			else {
				return System.Convert.ToInt64(myString);
			}
		}

		///<summary></summary>
		public static short PShort(string myString) {
			if(myString == "") {
				return 0;
			}
			else {
				return System.Convert.ToInt16(myString);
			}
		}

		///<summary></summary>
		public static float PFloat(string myString) {
			if(myString=="") {
				return 0;
			}
			//try{
			return System.Convert.ToSingle(myString);
			//}
			//catch{
			//	return 0;
			//}
		}

		///<summary>Currently does nothing.</summary>
		public static string PString(string myString) {
			return myString;
		}

		//<summary></summary>
		//public static string PTime (string myTime){
		//	return DateTime.Parse(myTime).ToString("HH:mm:ss");
		//}

		//<summary></summary>
		//public static string PBytes (byte[] myBytes){
		//	return Convert.ToBase64String(myBytes);
		//}

		/*
		///<summary></summary>
		public static Bitmap PBitmap(string myString){
			if(myString==""){
				return null;
			}
			byte[] rawData=Convert.FromBase64String(myString);
			MemoryStream stream=new MemoryStream(rawData);
			Bitmap image=new Bitmap(stream);
			return image;
		}*/

		///<summary></summary>
		public static Bitmap PBitmap(string myString) {
			if(myString==null || myString.Length<0x32) {//Bitmaps require a minimum length for header info.
				return null;
			}
			byte[] rawData=Convert.FromBase64String(myString);
			MemoryStream stream=new MemoryStream(rawData);
			Bitmap image=new Bitmap(stream);
			return image;
		}

		///<summary>Saves the string representation of a sound into a .wav file.  The timing of this is different than with the other "P" functions, and is only used by the export button in FormSigElementDefEdit</summary>
		public static void PSound(string sound,string filename) {
			if(!filename.EndsWith(".wav")) {
				throw new ApplicationException("Filename must end with .wav");
			}
			byte[] rawData=Convert.FromBase64String(sound);
			FileStream stream=new FileStream(filename,FileMode.Create,FileAccess.Write);
			stream.Write(rawData,0,rawData.Length);
		}
	}
}