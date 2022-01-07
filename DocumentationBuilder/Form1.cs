//How to format comments to trigger links:
//Inherits from task. This is triggered by "Inherits from ".  It then looks for ".".  So anything can follow after.
//and:
//FK to definition.DefNum is triggered by "FK to ".  It then looks for ".".  So anything can follow after.
//and:
//"Enum:" Then, the enum name must follow.  It must then be followed by a space or by nothing at all.  NO PERIOD allowed.
//ExitCodes: 103=Missing required arguments from command line, 110=Missing tables, 111=Missing Enums, 112=Could not build

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using UnitTests.Documentation;
using System.Text.RegularExpressions;

namespace DocumentationBuilder {
	public partial class Form1:Form {
		private DataConnection dcon;
		private XPathNavigator NavigatorODBusiness;
		private XPathNavigator NavigatorCodeBase;
		private List<string> MissingTables;
		private List<string> _listTableNames;
		private StringBuilder _errorMessage;
		private Version _versionCur;
		private Version _versionPrevious;
		private bool _isSilentMode;
		private string _pathPreviousDocumentationFile;

		public Form1(string[] cla) {
			InitializeComponent();
			_errorMessage=new StringBuilder();
			string version="";
			string serverName="";
			string database="";
			string mysqlUser="";
			string mysqlPass="";
			string versionPrevious="";
			if(cla.Length!=0) {
				foreach(string arg in cla) {
					if(arg.Contains("Version=")) {
						version=arg.Substring("Version=".Length).Trim('"');
					}
					if(arg.Contains("ServerName=")) {
						serverName=arg.Substring("ServerName=".Length).Trim('"');
					}
					if(arg.Contains("Database=")) {
						database=arg.Substring("Database=".Length).Trim('"');
					}
					if(arg.Contains("MySQLUser=")) {
						mysqlUser=arg.Substring("MySQLUser=".Length).Trim('"');
					}
					if(arg.Contains("MySQLPass=")) {
						mysqlPass=arg.Substring("MySQLPass=".Length).Trim('"');
					}
					if(arg.Contains("VersionPrev=")) {
						versionPrevious=arg.Substring("VersionPrev=".Length).Trim('"');
					}
					if(arg.Contains("PathPreviousDocumenationFile=")) {
						_pathPreviousDocumentationFile=arg.Substring("PathPreviousDocumenationFile=".Length).Trim('"');
					}
				}
				if(string.IsNullOrEmpty(version)
					||string.IsNullOrEmpty(serverName)
					|| string.IsNullOrEmpty(database)
					|| string.IsNullOrEmpty(mysqlUser)
					|| string.IsNullOrEmpty(mysqlPass))
				{
					Environment.Exit(103);//Missing required arguments from command line.
					return;
				}
				_isSilentMode=true;
			}
			_listTableNames=GetTableNames();
			if(_isSilentMode) {
				dcon=new DataConnection(serverName,database,mysqlUser,mysqlPass);
				textVersion.Text=version;
				_versionCur=new Version(version);
				if(string.IsNullOrEmpty(versionPrevious)) {
					_versionPrevious=new Version(_versionCur.Major,_versionCur.Minor-1,1);//Could be incorrect if the current version is XX.1.1.  We would have to manually fix it
				}
				else {
					_versionPrevious=new Version(versionPrevious);
				}
				try {
					Build();
					SchemaChanges();
				}
				catch(Exception ex) {
					ex.DoNothing();
					Environment.Exit(112);//Could not build.
				}
				Environment.Exit(0);
				return;
			}
			dcon=new DataConnection();
		}

		private void Form1_Load(object sender,EventArgs e) {
			textConnStr.Text=dcon.ConnStr;
			string command="SELECT ValueString FROM preference WHERE PrefName='DatabaseVersion'";
			textVersion.Text=dcon.GetCount(command);
			_versionCur=new Version(textVersion.Text);
			_versionPrevious=new Version(_versionCur.Major,_versionCur.Minor-1,1);//Could be incorrect if the current version is XX.1.1.  We would have to manually fix it
			textPrevVersion.Text=_versionPrevious.ToString();
		}

		private void butBuild_Click(object sender,EventArgs e) {
			if(textVersion.Text=="") {
				MessageBox.Show("Please enter the database version.");
				return;
			}
			if(textPrevVersion.Text=="") {
				MessageBox.Show("Please enter the previous version.");
				return;
			}
			_versionCur=new Version(textVersion.Text);
			_versionPrevious=new Version(textPrevVersion.Text);
			Cursor=Cursors.WaitCursor;
			Build();
			SchemaChanges();
			Cursor=Cursors.Default;
		}

		private void butBuildUnitTest_Click(object sender,EventArgs e) {
			string pathOutputUnitTestsDocumentation=ODFileUtils.CombinePaths(new string[] {"..","..","UnitTestsDocumentation.xml"});
			string stringFileNameUnitTests=Path.GetFileNameWithoutExtension(pathOutputUnitTestsDocumentation);
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			using XmlWriter xmlWriter = XmlWriter.Create(pathOutputUnitTestsDocumentation,xmlWriterSettings);
			List<TypeInfo> listTypeInfos=Assembly.GetAssembly(typeof(UnitTests.TestBase)).DefinedTypes.ToList();
			List<(EnumTestNum TestNum,MethodInfo MethodInfo_)> listTuples=new List<(EnumTestNum,MethodInfo)>();
			//Iterate over all methods from each test class inheriting from TestBase, adding those defined in EnumTestNum to a list along with their EnumTestNum
			for(int i = 0;i<listTypeInfos.Count;i++) {
				List<MethodInfo> listMethodInfos=listTypeInfos[i].DeclaredMethods.ToList();
				for(int j = 0;j<listMethodInfos.Count;j++) {
					UnitTests.Documentation.Numbering numberingMethodCur
						=(UnitTests.Documentation.Numbering)Attribute.GetCustomAttribute(listMethodInfos[j],typeof(UnitTests.Documentation.Numbering));
					if(numberingMethodCur!=null) {
						(EnumTestNum,MethodInfo) tuple=(numberingMethodCur.TestNum,listMethodInfos[j]);
						listTuples.Add(tuple); 
					}
				}
			}
			listTuples=listTuples.OrderBy(x => (int)x.TestNum).ToList();
			//add  the listed items to the xml doc
			xmlWriter.WriteProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\""+stringFileNameUnitTests+".xsl\"");
			xmlWriter.WriteStartElement("members");
			xmlWriter.WriteAttributeString("version",textVersion.Text);
			for(int i = 0;i<listTuples.Count;i++) {
				xmlWriter.WriteStartElement("UnitTest");
				xmlWriter.WriteAttributeString("Name",value: listTuples[i].MethodInfo_.Name);
				if(listTuples[i].TestNum!=0) {
					xmlWriter.WriteStartElement("TestNum");
					xmlWriter.WriteString(((int)listTuples[i].TestNum).ToString());
					xmlWriter.WriteEndElement();
				}
				UnitTests.Documentation.VersionAdded versionAdded=(UnitTests.Documentation.VersionAdded)Attribute.GetCustomAttribute(listTuples[i].MethodInfo_,typeof(UnitTests.Documentation.VersionAdded));
				UnitTests.Documentation.Description description=(UnitTests.Documentation.Description)Attribute.GetCustomAttribute(listTuples[i].MethodInfo_,typeof(UnitTests.Documentation.Description));
				if(versionAdded!=null) {
					xmlWriter.WriteStartElement("VersionAdded");
					xmlWriter.WriteString(versionAdded.Version);
					xmlWriter.WriteEndElement();
				}
				if(description!=null) {
					string pattern=@"(?<!>)\r\n";//only match when not preceeded by > 
					description.Desc=Regex.Replace(description.Desc.ToString(),pattern,"<br/>\r\n");
					xmlWriter.WriteStartElement("Description");
					xmlWriter.WriteRaw(description.Desc);
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		private void Build() {
			MissingTables=new List<string>();
			//dcon=new DataConnection();
			List<ODTable> table=ODTables.GetODTables(dcon);
			string outputFile=ODFileUtils.CombinePaths(new string[] {"..","..","OpenDentalDocumentation.xml"});
			//input:
			string inputFileODBusiness=ODFileUtils.CombinePaths(new string[] {"..","..","..","OpenDentBusiness","bin","Release","OpenDentBusiness.xml"});
			string inputFileCodeBase=ODFileUtils.CombinePaths(new string[] {"..","..","..","CodeBase","bin","Release","CodeBase.xml"});
			NavigatorODBusiness=GetEnumNavigator(inputFileODBusiness);
			NavigatorCodeBase=GetEnumNavigator(inputFileCodeBase);
			WriteXml(outputFile,table);
			if(MissingTables.Count>0){
				string s="";
				for(int i=0;i<MissingTables.Count;i++){
					if(i>0){
						s+="\r\n";
					}
					s+=MissingTables[i];
				}
				MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(s);
				msgbox.ShowDialog();
				Environment.Exit(110);
				return;
			}
			if(!string.IsNullOrEmpty(_errorMessage.ToString())) {
				MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(_errorMessage.ToString());
				msgbox.ShowDialog();
				Environment.Exit(111);
				return;
			}
			//ProcessStartInfo startInfo=new ProcessStartInfo();
			//Process.Start("Notepad.exe",outputFile);
			//Process.Start("iexplore.exe",outputFile);
			if(!_isSilentMode) {
				Process.Start(outputFile);
			}
			Application.Exit();
		}

		///<summary>Generates XML for schema changes from previous version.</summary>
		private void SchemaChanges() {
			string outputFile=ODFileUtils.CombinePaths(new string[] {"..","..","OpenDentalDiffDocumentation.xml"});
			string pathDest=ODFileUtils.CombinePaths(new string[] {"..","..","OpenDentalDocumentation.xml"});
			List<ODTable> listTablesNewVersion=GetFromXML(pathDest);
			if(_pathPreviousDocumentationFile.IsNullOrEmpty()) {
				pathDest=ODFileUtils.CombinePaths(new string[] { "..","..","..","..","opendental"+_versionPrevious.Major+"."+_versionPrevious.Minor,"DocumentationBuilder","OpenDentalDocumentation.xml" });
			}
			else {
				pathDest=_pathPreviousDocumentationFile;
			}
			List<ODTable> listTablePrevVersion=GetFromXML(pathDest);
			List<ODTable> listTableChanges=GetSchemaChanges(listTablesNewVersion,listTablePrevVersion);
			WriteXml(outputFile,listTableChanges,includeSummary:false,isSchemaChange:true);
		}

		private XPathNavigator GetEnumNavigator(string inputFile) {
			XmlDocument document=new XmlDocument();
			document.Load(inputFile);
			return document.CreateNavigator();
		}

		///<summary>Contructs the .xml document with the the tables passed in.</summary>
		///<param name="outputFile">Path to the .xml document.</param>
		///<param name="table">Table with database table names. Usually null when writing the .xml file for schema changes.</param>
		///<param name="listTables">list of ODTable objects</param>
		///<param name="includeSummary">Optional param. Will be true if writing schema changes.</param>
		private void WriteXml(string outputFile,List<ODTable> listTables,bool includeSummary=true,bool isSchemaChange=false) {
			string fileName=Path.GetFileNameWithoutExtension(outputFile);
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			using(XmlWriter writer = XmlWriter.Create(outputFile,settings)) {
				writer.WriteProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\""+fileName+".xsl\"");
				writer.WriteStartElement("database");
				writer.WriteAttributeString("version",textVersion.Text);
				foreach(ODTable tbl in listTables) {
					WriteTable(writer,tbl.Name,tbl.ListColumns,includeSummary,isSchemaChange);
				}
				writer.WriteEndElement();
				writer.Flush();
			}
		}

		///<summary>Writes table to xml. listColumn is a list of Tuple(ColumnName,DataType,ItemOrder)</summary>
		private void WriteTable(XmlWriter writer,string tableName,List<ODColumn> listColumns,bool includeSummary,bool isSchemaChange){
			if(tableName=="anestheticdata"
				|| tableName=="anestheticrecord"
				|| tableName=="anesthmedsgiven"
				|| tableName=="anesthmedsintake"
				|| tableName=="anesthmedsinventory"
				|| tableName=="anesthmedsinventoryadj"
				|| tableName=="anesthmedsuppliers"
				|| tableName=="anesthscore"
				|| tableName=="anesthvsdata"
				|| tableName=="files"
				|| tableName=="instructor"
				|| tableName=="reseller"
				|| tableName=="resellerservice"
				|| tableName=="maparea"
				|| tableName=="") 
			{				
				return;
			}
			writer.WriteStartElement("table");
			writer.WriteAttributeString("name",tableName);
			//table summary
			string summary="";
			if(includeSummary) {//Include summary only for database documentation
				summary=GetSummary("T:OpenDentBusiness."+GetTableName(tableName));
			}
			List<string> ancestorTables=GetAncestorTables(summary);
			if(ancestorTables.Count>0) {
				writer.WriteAttributeString("base",ancestorTables[0]);
			}
			writer.WriteStartElement("summary");
			writer.WriteString(summary);
			writer.WriteEndElement();
			foreach(ODColumn col in listColumns) {
				int order=0;
				Int32.TryParse(col.ItemOrder,out order);
				List<string> listEnumChangesOnly=null;
				if(isSchemaChange) {
					listEnumChangesOnly=col.ListODEnums.FindAll(x => ListTools.In(x.Status,ODEnumStatus.Deleted,ODEnumStatus.New)).Select(x => x.Name).ToList();
					if(listEnumChangesOnly.IsNullOrEmpty()) {
						listEnumChangesOnly=null;//This is needed in the WriteColumn method
					}
				}
				WriteColumn(writer,order,tableName,col.Name,col.DataType,ancestorTables,isDeleted:col.IsDeleted,listEnumChangesOnly);
			}
			writer.WriteEndElement();
		}

		private void WriteColumn(XmlWriter writer,int order,string tableName,string colName,string sqlType,List<string> ancestorTables,bool isDeleted,
		List<string> listEnumChangesOnly=null) 
		{
			writer.WriteStartElement("column");
			writer.WriteAttributeString("order",order.ToString());
			writer.WriteAttributeString("name",colName);
			if(sqlType=="tinyint(3) unsigned") {
				sqlType="tinyint";
			}
			else if(sqlType=="tinyint(1) unsigned") {//not used very much
				sqlType="tinyint";
			}
			else if(sqlType=="smallint(5) unsigned") {
				sqlType="smallint";
			}
			else if(sqlType=="mediumint(8) unsigned") {
				sqlType="mediumint";
			}
			else if(sqlType.EndsWith(" unsigned")){
				sqlType=sqlType.Substring(0,sqlType.Length-9);
			}
			writer.WriteAttributeString("type",sqlType);
			if(isDeleted) {
				//GetSchemaChanges() will mark column as deleted if column/table was removed.
				writer.WriteAttributeString("deleted","true");
			}
			string summary=GetSummary("F:OpenDentBusiness."+GetTableName(tableName)+"."+colName);
			if(summary==""){
				//this deals with the situation where the new data access layer has public Properites instead of public Fields.
				summary=GetSummary("P:OpenDentBusiness."+GetTableName(tableName)+"."+colName);
			}
			int i=0;
			while(summary=="" && i<ancestorTables.Count) {//this deals with an inherited property
				summary=GetSummary("F:OpenDentBusiness."+GetTableName(ancestorTables[i])+"."+colName);
				if(summary=="Primary key.") {
					summary="FK to "+ancestorTables[i]+"."+colName;
				}
				i++;
			}
			if(summary.StartsWith("FK to ") || summary.StartsWith("FKey to ")) {//eg FK to definition.DefNum
				int indexDot=summary.IndexOf(".");
				if(indexDot!=-1){
					string fkTable=summary.Substring(6,indexDot-6).ToLower();
					writer.WriteAttributeString("fk",fkTable);
				}
			}
			//column summary
			writer.WriteStartElement("summary");
			writer.WriteString(summary);
			writer.WriteEndElement();
			if(summary.StartsWith("Enum:")){
				int indexSpace=summary.IndexOf(" ");//the space will be found after the name of the enum
				string enumName="";
				if(indexSpace==-1 && summary.Length>5){//Enum is listed, but no other comments.
					enumName=summary.Substring(5);
				}
				else if(indexSpace > 5){//This if statement just protects against a space right after the Enum:
					enumName=summary.Substring(5,indexSpace-5);
				}
				if(enumName!=""){
					WriteEnum(writer,enumName,listEnumChangesOnly);
				}
			}
			writer.WriteEndElement();
		}

		private void WriteEnum(XmlWriter writer,string enumName,List<string> listEnumChanges) {
			if(enumName.EndsWith(".")) {
				_errorMessage.AppendLine("ERROR! enum: "+enumName+" ends with \".\" and this causes the documentation to fail.\r\nCorrect the enum summary "+
					"in the table type and rebuild Open Dental in release mode to update the serialization file.");
				return;
			}
			string summary="";
			//get an ordered list from OpenDental.xml
			//T:OpenDental.AccountType
			//first the summary for the enum itsef
			XPathNavigator navEnum=GetEnumSummary(enumName);
			if(navEnum!=null) {//Enumerations that are not in the Enumerations.cs file will be null.
				summary=navEnum.Value;
			}
			writer.WriteStartElement("Enumeration");
			writer.WriteAttributeString("name",enumName);
			writer.WriteElementString("summary",summary);//No summary if the enum node was not found.  This is for enumerations that are not present in Enumerations.cs
			//now, the individual enumsItems
			//F:OpenDental.AccountType.Asset
			//*[starts-with(name(),'B')]
			XPathNodeIterator nodes=GetEnumItems(enumName);
			if(nodes.Count==0) {
				_errorMessage.AppendLine("ERROR! enum: "+enumName+" was not found.  Something is wrong with the serialized xml documentation.");
				return;
			}
				//("//member[@name='F:OpenDental."+enumName+".*']");
			string itemName;
			int lastDot;
			while(nodes.MoveNext()) {
				summary=nodes.Current.Value;
				//nodes.Current.MoveToAttribute("name",null);
				itemName=nodes.Current.GetAttribute("name","");
				lastDot=itemName.LastIndexOf(".");
				itemName=itemName.Substring(lastDot+1);
				if(listEnumChanges!=null && !ListTools.In(itemName,listEnumChanges)) {
					//enum only changes and the enum is not in the list of changes.
					continue;
				}
				writer.WriteStartElement("EnumValue");
				writer.WriteAttributeString("name",itemName);
				writer.WriteString(summary);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private XPathNodeIterator GetEnumItems(string enumName) {
			XPathNodeIterator retVal=NavigatorODBusiness.Select("//member[contains(@name,'F:OpenDentBusiness."+enumName+".')]");
			if(retVal.Count==0) {
				retVal=NavigatorCodeBase.Select("//member[contains(@name,'F:CodeBase."+enumName+".')]");
			}
			return retVal;
		}

		///<summary>Returns a list of Table objects that have changes when listTableNew is compared to listTablePrev.</summary>
		private List<ODTable> GetSchemaChanges(List<ODTable> listTableNew,List<ODTable> listTablePrev) 
		{
			List<ODTable> listRetVal=new List<ODTable>();
			//Get changes that were added
			GetSchemaChangesHelper(listTableNew,listTablePrev,ref listRetVal);
			//Get changes that were removed
			GetSchemaChangesHelper(listTablePrev,listTableNew,ref listRetVal,isDeleted:true);
			return listRetVal;
		}

		///<summary>References list of ODTables that contain the changes from listTablePrev to listTableNew.</summary>
		private void GetSchemaChangesHelper(List<ODTable> listTableNew,List<ODTable> listTablePrev,ref List<ODTable> listRetVal,bool isDeleted=false) 
		{
			//Loop through each table from the new version and compare it with the old version's tables and columns.
			foreach(ODTable table in listTableNew) {
				if(!listTablePrev.Select(x=>x.Name).Contains(table.Name)) {
					//table does not exist in listTablePrev. This is a new table to the new version.
					//Add to listRetVal since we don't need to check the difference in columns. All the columns are new.
					if(isDeleted) {
						table.ListColumns.ForEach(x => x.IsDeleted=true);
						table.ListColumns.ForEach(x => x.ListODEnums.ForEach(y => x.IsDeleted=true));
					}
					listRetVal.Add(table);
					continue;
				}
				if(table==null) {
					continue;//This shouldn't happen
				}
				//the table exist in both lists. Now look for changes to the columns. 
				List<ODColumn> listColumnChanges=new List<ODColumn>();
				//Looks through each column from the new version and check it against the old version.
				foreach(ODColumn col in table.ListColumns) {
					//Get the table that matches tableNew
					//Get the list of column names
					//Check if the columns exist
					if(!listTablePrev.FirstOrDefault(x=>x.Name==table.Name).ListColumns.Select(x=>x.Name).Contains(col.Name)) {
						//The old version does not have this column, add the column to listColumnChanges.
						if(isDeleted) {
							col.IsDeleted=true;
							col.ListODEnums.ForEach(x => x.Status=ODEnumStatus.Deleted);//change all enum statuses to deleted
						}
						else {
							col.ListODEnums.ForEach(x => x.Status=ODEnumStatus.New);//change all enum statuses to new
						}
						listColumnChanges.Add(col);
					}
					if(!listTablePrev.FirstOrDefault(x => x.Name==table.Name).ListColumns.Select(x => x.Name).Contains(col.Name)
						|| listColumnChanges.Any(x => x.Name==col.Name)) {
						continue;//We only care about columns that are in both or that have not been added already
					}
					//Now look for new/deleted enums. The table exists in both but enums are not in both
					List<ODColumn> listEnumChanges=new List<ODColumn>();
					foreach(ODEnum odEnum in col.ListODEnums) {
						if(listTablePrev.FirstOrDefault(x => x.Name==table.Name).ListColumns.SelectMany(x => x.ListODEnums).Select(x => x.Name).Contains(odEnum.Name)) {
							continue;//enums are in both
						}
						if(isDeleted) {
							odEnum.Status=ODEnumStatus.Deleted;
						}
						else {
							odEnum.Status=ODEnumStatus.New;
						}
						//Make sure the column hasn't been added to the listEnumChanges
						ODColumn odColumEnum=listEnumChanges.FirstOrDefault(x => x.Name==col.Name);
						if(odColumEnum==null) {
							//table has not been added
							odColumEnum=col.Copy();
							odColumEnum.ListODEnums=new List<ODEnum>();
							listEnumChanges.Add(odColumEnum);
						}
						//Only add the enums that are not in both
						odColumEnum.ListODEnums.Add(odEnum);
					}
					if(!listEnumChanges.IsNullOrEmpty()) {
						listColumnChanges.AddRange(listEnumChanges);
					}
				}
				if(listColumnChanges.Count>0) {
					//Add the changes to the listRetVal.
					listRetVal.Add(new ODTable(table.Name,listColumnChanges));
				}
			}
		}

		///<summary>Returns a list of ODTables. Creates ODTable objects from the xml passed in.</summary>
		private List<ODTable> GetFromXML(string filePathToXML) {
			XDocument doc=XDocument.Load(filePathToXML);
			List<ODTable> listTables=new List<ODTable>();
			//Get a list of all the columns from the OpenDentalDocumentation.xml passed in
			List<XElement> listXElements=doc.Descendants("table").SelectMany(x =>x.Elements("column")).ToList();
			//Loop through each XElement and contruct Table objects.
			foreach(XElement col in listXElements) {
				//The column's parents first attribute is the table name.
				string table=col.Parent.Attributes().ToList()[0].Value;
				//The column's attributes consist of Order,Name,Type
				string order=col.Attributes().ToList()[0].Value;
				string name=col.Attributes().ToList()[1].Value;
				string type=col.Attributes().ToList()[2].Value;
				//make sure the table name exist.
				if(!listTables.Select(x=>x.Name).ToList().Contains(table)) {
					//Add the new table to listTables.
					ODTable newTable=new ODTable(table,new List<ODColumn>());
					listTables.Add(newTable);
				}
				//Get Enum values
				List<XElement> listXElementsEnums=col.Descendants("EnumValue").ToList();
				List<ODEnum> listODEnums=new List<ODEnum>();
				for(int i=0;i<listXElementsEnums.Count;i++) {
					string nameEnum=listXElementsEnums[i].Attribute("name").Value;
					string valueEnum=listXElementsEnums[i].Value;
					listODEnums.Add(new ODEnum() { Name=nameEnum, Value=valueEnum});
				}
				//Get the table and add the column to the List of colums
				//listTables will contain the table name. We added it above
				listTables.FirstOrDefault(x => x.Name==table).ListColumns.Add(new ODColumn(name,type,order,listODEnums));
			}
			return listTables;
		}

		///<summary>Gets the tablename that's used in the program based on the database tablename.  They are usually the same, except for capitalization.</summary>
		private string GetTableName(string dbTable){
			switch(dbTable){
				//This section can be enabled temporarily to check for missing tables:
				/*
				default:
					if(!MissingTables.Contains(dbTable)){
						MissingTables.Add(dbTable); 
					}
					return "";*/
				//The only classes that need to be included below are those that have a capital letter in addition to the first one
				//or those which are obsolete.
				case "accountingautopay": return "AccountingAutoPay";
				case "alertcategory": return "AlertCategory";
				case "alertcategorylink": return "AlertCategoryLink";
				case "alertitem": return "AlertItem";
				case "alertread": return "AlertRead";
				case "alertsub": return "AlertSub";
				case "allergydef": return "AllergyDef";
				case "appointmentdeleted": return "AppointmentDeleted";
				case "appointmentrule": return "AppointmentRule";
				case "appointmenttype": return "AppointmentType";
				case "apptfield": return "ApptField";
				case "apptfielddef": return "ApptFieldDef";
				case "apptreminderrule": return "ApptReminderRule";
				case "apptremindersent": return "ApptReminderSent";
				case "apptcomm": return "ApptComm";
				case "apptview": return "ApptView";
				case "apptviewitem": return "ApptViewItem";
				case "asapcomm": return "AsapComm";
				case "autocode": return "AutoCode";
				case "autocodecond": return "AutoCodeCond";
				case "autocodeitem": return "AutoCodeItem";
				case "automationcondition": return "AutomationCondition";
				case "autonote": return "AutoNote";
				case "autonotecontrol": return "AutoNoteControl";
				case "canadianclaim": return "CanadianClaim";
				case "canadianextract": return "CanadianExtract";
				case "canadiannetwork": return "CanadianNetwork";
				case "cdspermission": return "CDSPermission";
				case "centralconnection": return "CentralConnection";
				case "chartview": return "ChartView";
				case "claimattach": return "ClaimAttach";
				case "claimcondcodelog": return "ClaimCondCodeLog";
				case "claimform": return "ClaimForm";
				case "claimformitem": return "ClaimFormItem";
				case "claimpayment": return "ClaimPayment";
				case "claimproc": return "ClaimProc";
				case "claimsnapshot": return "ClaimSnapshot";
				case "claimtracking": return "ClaimTracking";
				case "claimvalcodelog": return "ClaimValCodeLog";
				case "clinicpref": return "ClinicPref";
				case "clockevent": return "ClockEvent";
				case "codesystem": return "CodeSystem";
				case "computerpref": return "ComputerPref";
				case "confirmationrequest": return "ConfirmationRequest";
				case "connectiongroup": return "ConnectionGroup";
				case "conngroupattach": return "ConnGroupAttach";
				case "covcat": return "CovCat";
				case "covspan": return "CovSpan";
				case "creditcard": return "CreditCard";
				case "custrefentry": return "CustRefEntry";
				case "custreference": return "CustReference";
				case "dashboardar": return "DashboardAR";
				case "dashboardcell": return "DashboardCell";
				case "dashboardlayout": return "DashboardLayout";
				case "definition": return "Def";
				case "deflink": return "DefLink";
				case "deletedobject": return "DeletedObject";
				case "dictcustom": return "DictCustom";
				case "diseasedef": return "DiseaseDef";
				case "discountplan": return "DiscountPlan";
				case "displayfield": return "DisplayField";
				case "displayreport": return "DisplayReport";
				case "dispsupply": return "DispSupply";
				case "docattach": return "DocAttach";
				case "documentmisc": return "DocumentMisc";
				case "drugmanufacturer": return "DrugManufacturer";
				case "drugunit": return "DrugUnit";
				case "eduresource": return "EduResource";
				case "ehramendment": return "EhrAmendment";
				case "ehraptobs": return "EhrAptObs";
				case "ehrcareplan": return "EhrCarePlan";
				case "ehrlab": return "EhrLab";
				case "ehrlabclinicalinfo": return "EhrLabClinicalInfo";
				case "ehrlabimage": return "EhrLabImage";
				case "ehrlabnote": return "EhrLabNote";
				case "ehrlabresult": return "EhrLabResult";
				case "ehrlabresultscopyto": return "EhrLabResultsCopyTo";
				case "ehrlabspecimen": return "EhrLabSpecimen";
				case "ehrlabspecimencondition": return "EhrLabSpecimenCondition";
				case "ehrlabspecimenrejectreason": return "EhrLabSpecimenRejectReason";
				case "ehrmeasure": return "EhrMeasure";
				case "ehrmeasureevent": return "EhrMeasureEvent";
				case "ehrnotperformed": return "EhrNotPerformed";
				case "ehrpatient": return "EhrPatient";
				case "ehrprovkey": return "EhrProvKey";
				case "ehrquarterlykey": return "EhrQuarterlyKey";
				case "ehrsummaryccd": return "EhrSummaryCcd";
				case "ehrtrigger": return "EhrTrigger";
				case "electid": return "ElectID";
				case "emailaddress": return "EmailAddress";
				case "emailattach": return "EmailAttach";
				case "emailautograph": return "EmailAutograph";
				case "emailmessage": return "EmailMessage";
				case "emailmessageuid": return "EmailMessageUid";
				case "emailtemplate": return "EmailTemplate";
				case "eobattach": return "EobAttach";
				case "erxlog": return "ErxLog";
				case "eservicesignal": return "EServiceSignal";
				case "etransmessagetext": return "EtransMessageText";
				case "evaluationcriterion": return "EvaluationCriterion";
				case "evaluationcriteriondef": return "EvaluationCriterionDef";
				case "evaluationdef": return "EvaluationDef";
				case "famaging": return "FamAging";
				case "familyhealth": return "FamilyHealth";
				case "feesched": return "FeeSched";
				case "fhircontactpoint": return "FHIRContactPoint";
				case "fhirsubscription": return "FHIRSubscription";
				case "fielddeflink": return "FieldDefLink";
				case "formpat": return "FormPat";
				case "formularymed": return "FormularyMed";
				case "gradingscale": return "GradingScale";
				case "gradingscaleitem": return "GradingScaleItem";
				case "graphicassembly": return "GraphicAssembly Not Used";
				case "graphicelement": return "graphicelement Not Used";
				case "graphicpoint": return "graphicpoint Not Used";
				case "graphicshape": return "graphicshape Not Used";
				case "graphictype": return "graphictype Not Used";
				case "grouppermission": return "GroupPermission";
				case "histappointment": return "HistAppointment";
				case "hl7def": return "HL7Def";
				case "hl7deffield": return "HL7DefField";
				case "hl7defmessage": return "HL7DefMessage";
				case "hl7defsegment": return "HL7DefSegment";
				case "hl7msg": return "HL7Msg";
				case "hl7procattach": return "HL7ProcAttach";
				case "icd9": return "ICD9";
				case "inseditlog": return "InsEditLog";
				case "insfilingcode": return "InsFilingCode";
				case "insfilingcodesubtype": return "InsFilingCodeSubtype";
				case "insplan": return "InsPlan";
				case "inssub": return "InsSub";
				case "insverify": return "InsVerify";
				case "insverifyhist": return "InsVerifyHist";
				case "installmentplan": return "InstallmentPlan";
				case "journalentry": return "JournalEntry";
				case "labcase": return "LabCase";
				case "labpanel": return "LabPanel";
				case "labresult": return "LabResult";
				case "labturnaround": return "LabTurnaround";
				case "languageforeign": return "LanguageForeign";
				case "lettermerge": return "LetterMerge";
				case "lettermergefield": return "LetterMergeField";
				case "maparea": return "MapArea";
				case "medicalorder": return "MedicalOrder";
				case "medicationpat": return "MedicationPat";
				case "medlab": return "MedLab";
				case "medlabfacattach": return "MedLabFacAttach";
				case "medlabfacility": return "MedLabFacility";
				case "medlabresult": return "MedLabResult";
				case "medlabspecimen": return "MedLabSpecimen";
				case "mountdef": return "MountDef";
				case "mountitem": return "MountItem";
				case "mountitemdef": return "MountItemDef";
				case "oidexternal": return "OIDExternal";
				case "oidinternal": return "OIDInternal";
				case "orthochart": return "OrthoChart";
				case "orthocharttab": return "OrthoChartTabLink";
				case "orthocharttablink": return "OrthoChartTabLink";
				case "orionproc": return "OrionProc";
				case "patfield": return "PatField";
				case "patfielddef": return "PatFieldDef";
				case "patientlink": return "PatientLink";
				case "patientnote": return "PatientNote";
				case "patientportalinvite": return "PatientPortalInvite";
				case "patientrace": return "PatientRace";
				case "patplan": return "PatPlan";
				case "patrestriction": return "PatRestriction";
				case "payortype": return "PayorType";
				case "payperiod": return "PayPeriod";
				case "payplan": return "PayPlan";
				case "payplancharge": return "PayPlanCharge";
				case "paysplit": return "PaySplit";
				case "perioexam": return "PerioExam";
				case "periomeasure": return "PerioMeasure";
				case "phonenumber": return "PhoneNumber";
				case "plannedappt": return "PlannedAppt";
				case "preference": return "Pref";
				case "procapptcolor": return "ProcApptColor";
				case "procbutton": return "ProcButton";
				case "procbuttonitem": return "ProcButtonItem";
				case "procbuttonquick": return "ProcButtonQuick";
				case "proccodenote": return "ProcCodeNote";
				case "procedurecode": return "ProcedureCode";
				case "procedurelog": return "Procedure";
				case "procgroupitem": return "ProcGroupItem";
				case "proclicense": return "proclicense not used";
				case "procnote": return "ProcNote";
				case "proctp": return "ProcTP";
				case "programproperty": return "ProgramProperty";
				case "providererx": return "ProviderErx";
				case "providerident": return "ProviderIdent";
				case "questiondef": return "QuestionDef";
				case "quickpastecat": return "QuickPasteCat";
				case "quickpastenote": return "QuickPasteNote";
				case "refattach": return "RefAttach";
				case "registrationkey": return "RegistrationKey";
				case "recalltrigger": return "RecallTrigger";
				case "recalltype": return "RecallType";
				case "reminderrule": return "ReminderRule";
				case "repeatcharge": return "RepeatCharge";
				case "replicationserver": return "ReplicationServer";
				case "reqneeded": return "ReqNeeded";
				case "reqstudent": return "ReqStudent";
				case "requiredfield": return "RequiredField";
				case "requiredfieldcondition": return "RequiredFieldCondition";
				case "rxalert": return "RxAlert";
				case "rxdef": return "RxDef";
				case "rxnorm": return "RxNorm";
				case "rxpat": return "RxPat";
				case "scheddefault": return "SchedDefault";
				case "scheduleop": return "ScheduleOp";
				case "schoolclass": return "SchoolClass";
				case "schoolcourse": return "SchoolCourse";
				case "screengroup": return "ScreenGroup";
				case "screenpat": return "ScreenPat";
				case "securitylog": return "SecurityLog";
				case "securityloghash": return "SecurityLogHash";
				case "sheetdef": return "SheetDef";
				case "sheetfield": return "SheetField";
				case "sheetfielddef": return "SheetFieldDef";
				case "sigbutdef": return "SigButDef";
				case "sigbutdefelement": return "SigButDefElement";
				case "sigelement": return "SigElement";
				case "sigelementdef": return "SigElementDef";
				case "sigmessage": return "SigMessage";
				case "smsblockphone": return "SmsBlockPhone";
				case "smsfrommobile": return "SmsFromMobile";
				case "smsphone": return "SmsPhone";
				case "smstomobile": return "SmsToMobile";
				case "smsmo": return "SmsMO";
				case "smsmt": return "SmsMT";
				case "smsvln": return "SmsVln";
				case "stateabbr": return "StateAbbr";
				case "stmtadjattach": return "StmtAdjAttach";
				case "stmtpaysplitattach": return "StmtPaySplitAttach";
				case "stmtprocattach": return "StmtProcAttach";
				case "stmtlink": return "StmtLink";
				case "substitutionlink": return "SubstitutionLink";
				case "supplyneeded": return "SupplyNeeded";
				case "supplyorder": return "SupplyOrder";
				case "supplyorderitem": return "SupplyOrderItem";
				case "taskancestor": return "TaskAncestor";
				case "taskhist": return "TaskHist";
				case "tasklist": return "TaskList";
				case "tasknote": return "TaskNote";
				case "tasksubscription": return "TaskSubscription";
				case "taskunread": return "TaskUnread";
				case "terminalactive": return "TerminalActive";
				case "timeadjust": return "TimeAdjust";
				case "timecardrule": return "TimeCardRule";
				case "toolbutitem": return "ToolButItem";
				case "toothgridcell": return "ToothGridCell";
				case "toothgridcol": return "ToothGridCol";
				case "toothgriddef": return "ToothGridDef";
				case "toothinitial": return "ToothInitial";
				case "treatplan": return "TreatPlan";
				case "treatplanattach": return "TreatPlanAttach";
				case "tsitranslog": return "TsiTransLog";
				case "updatehistory": return "UpdateHistory";
				case "userclinic": return "UserClinic";
				case "usergroup": return "UserGroup";
				case "usergroupattach": return "UserGroupAttach";
				case "userodapptview": return "UserodApptView";
				case "userodpref": return "UserOdPref";
				case "userquery": return "UserQuery";
				case "userweb": return "UserWeb";
				case "vaccinedef": return "VaccineDef";
				case "vaccineobs": return "VaccineObs";
				case "vaccinepat": return "VaccinePat";
				case "webschedrecall": return "WebSchedRecall";
				case "wikilistheaderwidth": return "WikiListHeaderWidth";
				case "wikilisthist": return "WikiListHist";
				case "wikipage": return "WikiPage";
				case "wikipagehist": return "WikiPageHist";
				case "xchargetransaction": return "XChargeTransaction";
				case "xwebresponse": return "XWebResponse";
				case "zipcode": return "ZipCode";
			}
			/*single cap classes:
			account
			adjustment
			appointment
			benefit
			carrier
			claim
			clearinghouse
			clinic
			commlog
			computer
			contact
			county
			deposit
			disease
			document
			dunning
			employee
			employer
			etrans
			fee
			instructor
			laboratory
			language
			letter
			medication
			mount
			operatory
			patient
			payment
			preference
			printer
			program
			provider
			question
			recall
			reconcile
			referral
			schedule
			school
			screen
			signal
			task
			transaction
			userod
			 */
			string tbName=_listTableNames.FirstOrDefault(x => x.ToLower()==dbTable.ToLower());
			if(!string.IsNullOrEmpty(tbName)) {
				return tbName;
			}
			return dbTable.Substring(0,1).ToUpper()+dbTable.Substring(1);
		}

		private List<string> GetTableNames() {
			//"C:\development\OPEN DENTAL SUBVERSION\head\OpenDentBusiness\TableTypes\";
			string inputFile=ODFileUtils.CombinePaths(new string[] {"..","..","..","OpenDentBusiness","TableTypes"});
			return Directory.GetFiles(inputFile, "*.cs").Select(Path.GetFileNameWithoutExtension).ToList();
		}

		///<summary>Gets the summary from the xml file.  The full and correct member name must be supplied.</summary>
		private string GetSummary(string member){
			XPathNavigator navOne=NavigatorODBusiness.SelectSingleNode("//member[@name='"+member+"']");
			if(navOne==null){
				return "";
			}
			XPathNavigator nav=navOne.SelectSingleNode("summary");
			if(nav==null){
				return "";
			}
			return navOne.SelectSingleNode("summary").Value;
			//F:OpenDental.ReportParameter.DefaultValues']").Value;
		}

		private XPathNavigator GetEnumSummary(string enumName) {
			XPathNavigator retVal=NavigatorODBusiness.SelectSingleNode("//member[@name='T:OpenDentBusiness."+enumName+"']");
			if(retVal==null) {
				retVal=NavigatorCodeBase.SelectSingleNode("//member[@name='T:CodeBase."+enumName+"']");
			}
			return retVal;
		}

		/// <summary>Gets the names of all ancestors for this table</summary>
		private List<string> GetAncestorTables(string summary) {
			List<string> ancestors=new List<string>();
			string baseTable;
			bool keepSearching=true;
			while(keepSearching) {
				if(summary.StartsWith("Inherits from ")) {//inherited table
					int indexDot=summary.IndexOf(".");
					if(indexDot!=-1) {
						baseTable=summary.Substring(14,indexDot-14);
						ancestors.Add(baseTable);
						summary=GetSummary("T:OpenDentBusiness."+GetTableName(baseTable));
					}
				}
				else {
					keepSearching=false;
				}
			}
			return ancestors;
		}
	
	


	}
}