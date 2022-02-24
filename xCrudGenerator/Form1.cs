using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace xCrudGenerator {
	public partial class Form1:Form {
		public static string crudDir=@"..\..\..\OpenDentBusiness\Crud";
		public static string crudmDir=@"..\..\..\OpenDentBusiness\Mobile\Crud";
		public static string convertDbFile=@"..\..\..\OpenDentBusiness\Misc\ConvertDatabases6.cs";
		private const string rn="\r\n";
		private const string t="\t";
		private const string t2="\t\t";
		private const string t3="\t\t\t";
		private const string t4="\t\t\t\t";
		private const string t5="\t\t\t\t\t";
		private const string t6="\t\t\t\t\t\t";
		private const string t7="\t\t\t\t\t\t\t";
		private List<Type> tableTypes;
		private List<Type> tableTypesAll;
		private List<Type> tableTypesM;
		public static List<string> ListFilesToAddToODBiz=new List<string>();

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender,EventArgs e) {
			if(!Directory.Exists(crudDir)) {
				MessageBox.Show(crudDir+" is an invalid path.");
				Application.Exit();
			}
			if(!Directory.Exists(crudmDir)) {
				MessageBox.Show(crudmDir+" is an invalid path.");
				Application.Exit();
			}
			tableTypes=new List<Type>();
			tableTypesAll=new List<Type>();
			tableTypesM=new List<Type>();
			Type typeTableBase=typeof(TableBase);
			Assembly assembly=Assembly.GetAssembly(typeTableBase);
			foreach(Type typeClass in assembly.GetTypes()) {
				if(typeClass.Name.StartsWith("WebForms_")) {
					continue;
				}
				if(typeClass.IsSubclassOf(typeTableBase)) {
					if(CrudGenHelper.IsMobile(typeClass)) {
						tableTypesM.Add(typeClass);	
					}
					else{
						tableTypes.Add(typeClass);	
					}
					tableTypesAll.Add(typeClass);	
				}
			}
			tableTypesAll.Sort(CompareTypesByName);
			tableTypes.Sort(CompareTypesByName);
			tableTypesM.Sort(CompareTypesByName);
			for(int i=0;i<tableTypes.Count;i++) {
				listClass.Items.Add(tableTypesAll[i].Name);
			}
			for(int i=0;i<Enum.GetNames(typeof(SnippetType)).Length;i++) {
				comboType.Items.Add(Enum.GetNames(typeof(SnippetType))[i].ToString());
			}
			comboType.SelectedIndex=(int)SnippetType.EntireSclass;
		}

		private static int CompareTypesByName(Type x, Type y) {
			return x.Name.CompareTo(y.Name);
		}

		private void butRun_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			string[] files;
			StringBuilder strb;
			StringBuilder results=new StringBuilder();
			string className;
			if(checkRun.Checked) {
				string server="localhost"+(string.IsNullOrEmpty(textPortNum.Text) ? "" : $":{textPortNum.Text}");
				files=Directory.GetFiles(crudDir);
				CrudGenHelper.ConnectToDatabase(textDb.Text,server:server);
				for(int i=0;i<tableTypes.Count;i++) {
					string crudDirectory=crudDir;
					className=tableTypes[i].Name+"Crud";
					strb=new StringBuilder();
					if(checkAddToConvertScript.Checked) {
						CrudGenHelper.ValidateTypes(tableTypes[i],textDb.Text);
					}
					WriteAll(strb,className,tableTypes[i],false);
					string crudDirOverride=CrudGenHelper.GetCrudLocationOverride(tableTypes[i]);
					if(!string.IsNullOrEmpty(crudDirOverride)) {
						crudDirectory=crudDirOverride;
					}
					string crudFileName=Path.Combine(crudDirectory,className+".cs");
					bool isNewFile=!File.Exists(crudFileName);
					File.WriteAllText(crudFileName,strb.ToString());
					if(isNewFile) {
						CrudGenHelper.AddToSubversion(Path.Combine(Application.StartupPath,crudDirectory,className+".cs"));
						ListFilesToAddToODBiz.Add(Path.Combine("Crud",Path.GetFileName(crudFileName)));
					}
					CrudQueries.Write(convertDbFile,tableTypes[i],textDb.Text,false,checkRunQueries.Checked,checkAddToConvertScript.Checked,results);
					CrudGenDataInterface.Create(convertDbFile,tableTypes[i],textDb.Text,false);
					Application.DoEvents();
				}
			}
			if(checkRunSchema.Checked) {
				File.WriteAllText(@"..\..\..\OpenDentBusiness\Db\SchemaCrudTest.cs",CrudSchemaForUnitTest.Create());
			}
			Cursor=Cursors.Default;
			MessageBox.Show(results.Length > 0 ? results.ToString() : "Done");
		}

		///<summary>Example of className is 'AccountCrud' or 'PatientmCrud'.</summary>
		private void WriteAll(StringBuilder strb,string className,Type typeClass,bool isMobile) {
			#region initialize variables
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
			List<Permissions> listAuditTrailPerms=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeClass));
			string obj=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);//lowercase initial letter.  Example feeSched
			string oldObj="old"+typeClass.Name;//used in the second update overload.  Example oldFeeSched
			string customNamespace=CrudGenHelper.GetNamespaceOverride(typeClass);
			string nameListParam="list"+priKeyParam.Substring(0,1).ToUpper()+priKeyParam.Substring(1)+"s";//prepend 'list', upper case priKey first char, append 's'. Example listPatNums
			#endregion initialize variables
			#region class header
			strb.Append("//This file is automatically generated."+rn
				+"//Do not attempt to make changes to this file because the changes will be erased and overwritten."+rn
				+@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;"+rn);
			if(CrudGenHelper.HasBatchWriteMethods(typeClass)) {
				strb.Append("using System.Text;"+rn);
			}
			if(className.StartsWith("EhrLab")) {
				strb.Append("using EhrLaboratories;"+rn);
			}
			if(isMobile) {
				strb.Append(rn+"namespace OpenDentBusiness.Mobile.Crud{");
			}
			else if(!string.IsNullOrEmpty(customNamespace)) {
				strb.Append(rn+"namespace "+customNamespace+"{");
			}
			else { 
				strb.Append(rn+"namespace OpenDentBusiness.Crud{");
			}
			strb.Append(rn+t+"public class "+className+" {");
			#endregion class header
			#region SelectOne
			//SelectOne------------------------------------------------------------------------------------------
			if(isMobile) {
				strb.Append(rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using primaryKey1(CustomerNum) and primaryKey2.  Returns null if not found.</summary>");
				strb.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(long "+priKeyParam1+",long "+priKeyParam2+") {");
				strb.Append(rn+t3+"string command=\"SELECT * FROM "+tablename+" \"");
				strb.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+priKeyParam1+")+\" AND "+priKey2.Name+" = \"+POut.Long("+priKeyParam2+");");
			}
			else {
				strb.Append(rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using the primary key.  Returns null if not found.</summary>");
				strb.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(long "+priKeyParam+") {");
				strb.Append(rn+t3+"string command=\"SELECT * FROM "+tablename+" \"");
				strb.Append(rn+t4+"+\"WHERE "+priKey.Name+" = \"+POut.Long("+priKeyParam+");");
			}
			strb.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			strb.Append(rn+t3+"if(list.Count==0) {");
			strb.Append(rn+t4+"return null;");
			strb.Append(rn+t3+"}");
			strb.Append(rn+t3+"return list[0];");
			strb.Append(rn+t2+"}");
			#endregion SelectOne
			#region SelectOne(command)
			//SelectOne(string command)--------------------------------------------------------------------------
			strb.Append(rn+rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using a query.</summary>");
			strb.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(string command) {");
			strb.Append(rn+t3+@"if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException(""Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n""+command);
			}");
			strb.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			strb.Append(rn+t3+"if(list.Count==0) {");
			strb.Append(rn+t4+"return null;");
			strb.Append(rn+t3+"}");
			strb.Append(rn+t3+"return list[0];");
			strb.Append(rn+t2+"}");
			#endregion SelectOne(command)
			#region SelectMany
			//SelectMany-----------------------------------------------------------------------------------------
			strb.Append(rn+rn+t2+"///<summary>Gets a list of "+typeClass.Name+" objects from the database using a query.</summary>");
			strb.Append(rn+t2+"public static List<"+typeClass.Name+"> SelectMany(string command) {");
			strb.Append(rn+t3+@"if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException(""Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n""+command);
			}");
			strb.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			strb.Append(rn+t3+"return list;");
			strb.Append(rn+t2+"}");
			#endregion SelectMany
			#region TableToList
			//TableToList----------------------------------------------------------------------------------------
			strb.Append(rn+rn+t2+"///<summary>Converts a DataTable to a list of objects.</summary>");
			strb.Append(rn+t2+"public static List<"+typeClass.Name+"> TableToList(DataTable table) {");
			strb.Append(rn+t3+"List<"+typeClass.Name+"> retVal=new List<"+typeClass.Name+">();");
			strb.Append(rn+t3+typeClass.Name+" "+obj+";");
			strb.Append(rn+t3+"foreach(DataRow row in table.Rows) {");
			strb.Append(rn+t4+obj+"=new "+typeClass.Name+"();");
			List<FieldInfo> fieldsInDb=CrudGenHelper.GetFieldsExceptNotDb(fields);
			strb.Append(RowToObj(fieldsInDb,typeClass,obj));
			strb.Append(rn+t4+"retVal.Add("+obj+");");
			strb.Append(rn+t3+"}");
			strb.Append(rn+t3+"return retVal;");
			strb.Append(rn+t2+"}");
			#endregion TableToList
			#region RowToObj
			//RowToObj------------------------------------------------------------------------------------------
			if(CrudGenHelper.UsesDataReader(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Converts an IDataRecord to a "+typeClass.Name+" object.</summary>");
				strb.Append(rn+t2+"public static "+typeClass.Name+" RowToObj(IDataRecord row) {");
				strb.Append(rn+t3+"return new "+typeClass.Name+" {");
				strb.Append(RowToObj(fieldsInDb,typeClass));
				strb.Append(rn+t3+"};");
				strb.Append(rn+t2+"}");
			}
			#endregion RowToObj
			#region ListToTable
			//ListToTable----------------------------------------------------------------------------------------
			strb.Append(rn+rn+t2+"///<summary>Converts a list of "+typeClass.Name+" into a DataTable.</summary>");
			strb.Append(rn+t2+"public static DataTable ListToTable(List<"+typeClass.Name+"> list"+typeClass.Name+"s,string tableName=\"\") {");
			strb.Append(rn+t3+"if(string.IsNullOrEmpty(tableName)) {");
			strb.Append(rn+t4+"tableName=\""+typeClass.Name+"\";");
			strb.Append(rn+t3+"}");
			strb.Append(rn+t3+"DataTable table=new DataTable(tableName);");
			//Now add columns.
			foreach(FieldInfo field in fieldsInDb) {
				strb.Append(rn+t3+"table.Columns.Add(\""+field.Name+"\");");
			}
			string classLowerCase=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);
			strb.Append(rn+t3+"foreach("+typeClass.Name+" "+classLowerCase+" in list"+typeClass.Name+"s) {");
			strb.Append(rn+t4+"table.Rows.Add(new object[] {");
			CrudSpecialColType specialType;
			foreach(FieldInfo field in fieldsInDb) {
				strb.Append(rn+t5);
				//Fields are not guaranteed to be in any particular order.
				specialType=CrudGenHelper.GetSpecialType(field);
				if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					strb.Append("POut.Time  (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append("POut.Long (");
				}
				else if(field.FieldType.IsEnum) {
					strb.Append("POut.Int   ((int)");
				}
				else switch(field.FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+field.FieldType.Name);
						case "Bitmap":
							strb.Append("POut.Bitmap(");
							break;
						case "Boolean":
							strb.Append("POut.Bool  (");
							break;
						case "Byte":
							strb.Append("POut.Byte  (");
							break;
						case "DateTime":
							strb.Append("POut.DateT (");
							break;
						case "Double":
							strb.Append("POut.Double(");
							break;
						case "Int64":
							strb.Append("POut.Long  (");
							break;
						case "Color":
						case "Int32":
						case "Interval":
							strb.Append("POut.Int   (");
							break;
						case "Single":
							strb.Append("POut.Float (");
							break;
						case "String":
							strb.Append("            ");
							break;
						case "TimeSpan":
							strb.Append("POut.Time  (");
							break;
					}
				if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append(classLowerCase+"."+field.Name+".Ticks),");
				}
				else {
					strb.Append(classLowerCase+"."+field.Name
						+(field.FieldType.Name=="Color" ? ".ToArgb()" : "")
						+(field.FieldType.Name=="Interval" ? ".ToInt()" : "")
						+(field.FieldType.Name=="DateTime" ? ",false" : "")
						+(field.FieldType.Name=="TimeSpan" ? ",false" : "")
						+(field.FieldType.Name=="String" ? "" : ")")
						+",");
				}
			}
			strb.Append(rn+t4+"});");
			strb.Append(rn+t3+"}");
			strb.Append(rn+t3+"return table;");
			strb.Append(rn+t2+"}");
			#endregion ListToTable
			#region Insert
			//Insert---------------------------------------------------------------------------------------------
			List<FieldInfo> fieldsExceptPri=null; 
			if(isMobile) {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey2);
				//first override not used for mobile.
				//second override
				strb.Append(rn+rn+t2+"///<summary>Usually set useExistingPK=true.  Inserts one "+typeClass.Name+" into the database.</summary>");
				strb.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				strb.Append(rn+t3+"if(!useExistingPK) {");// && PrefC.RandomKeys) {");PrefC.RandomKeys is always true for mobile, since autoincr is just not possible.
//Todo: ReplicationServers.GetKey() needs to work for mobile.  Not needed until we start inserting records from mobile.
				strb.Append(rn+t4+obj+"."+priKey2.Name+"=ReplicationServers.GetKey(\""+tablename+"\",\""+priKey2.Name+"\");");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				//strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");//PrefC.RandomKeys is always true
				strb.Append(rn+t3+"command+=\""+priKey2.Name+",\";");
				//strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"command+=\"");
				for(int f=0;f<fieldsExceptPri.Count;f++) {//all fields except PK2.
					if(CrudGenHelper.GetSpecialType(fieldsExceptPri[f]).HasFlag(CrudSpecialColType.TimeStamp)) {
						continue;
					}
					if(f>0) {
						strb.Append(",");
					}
					strb.Append(fieldsExceptPri[f].Name);
				}
				strb.Append(") VALUES(\";");
				//strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");//PrefC.RandomKeys is always true
				strb.Append(rn+t3+"command+=POut.Long("+obj+"."+priKey2.Name+")+\",\";");
				//strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"command+=");
			}
			else {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey);
				strb.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Returns the new priKey.</summary>");
				strb.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+") {");
				#region Oracle Removed
				//strb.Append(rn+t3+"if(DataConnection.DBtype==DatabaseType.Oracle) {");
				//strb.Append(rn+t4+obj+"."+priKey.Name+"=DbHelper.GetNextOracleKey(\""+tablename+"\",\""+priKey.Name+"\");");
				//strb.Append(rn+t4+"int loopcount=0;");
				//strb.Append(rn+t4+"while(loopcount<100) {");
				//strb.Append(rn+t5+"try {");
				//strb.Append(rn+t5+t+"return Insert("+obj+",true);");
				//strb.Append(rn+t5+"}");
				//strb.Append(rn+t5+"catch(Oracle.ManagedDataAccess.Client.OracleException ex) {");
				//strb.Append(rn+t5+t+"if(ex.Number==1 && ex.Message.ToLower().Contains(\"unique constraint\") && ex.Message.ToLower().Contains(\"violated\")) {");
				//strb.Append(rn+t5+t2+obj+"."+priKey.Name+"++;");
				//strb.Append(rn+t5+t2+"loopcount++;");
				//strb.Append(rn+t5+t+"}");
				//strb.Append(rn+t5+t+"else{");
				//strb.Append(rn+t5+t2+"throw ex;");
				//strb.Append(rn+t5+t+"}");
				//strb.Append(rn+t5+"}");
				//strb.Append(rn+t4+"}");
				//strb.Append(rn+t4+"throw new ApplicationException(\"Insert failed.  Could not generate primary key.\");");
				//strb.Append(rn+t3+"}");
				//strb.Append(rn+t3+"else {");
				//strb.Append(rn+t4+"return Insert("+obj+",false);");
				//strb.Append(rn+t3+"}");
				#endregion Oracle Removed
				strb.Append(rn+t3+"return Insert("+obj+",false);");
				strb.Append(rn+t2+"}");
				//second override
				strb.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Provides option to use the existing priKey.</summary>");
				strb.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(!useExistingPK && PrefC.RandomKeys) {");
					strb.Append(rn+t4+obj+"."+priKey.Name+"=ReplicationServers.GetKey(\""+tablename+"\",\""+priKey.Name+"\");");
					strb.Append(rn+t3+"}");
				}
				strb.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				strb.Append(rn+t4+"command+=\""+priKey.Name+",\";");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"command+=\"");
				for(int f=0;f<fieldsExceptPri.Count;f++) {
					specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						continue;
					}
					if(f>0) {
						strb.Append(",");
					}
					strb.Append(fieldsExceptPri[f].Name);
				}
				strb.Append(") VALUES(\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				strb.Append(rn+t4+"command+=POut.Long("+obj+"."+priKey.Name+")+\",\";");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"command+=");
			}
			//a quick and dirty temporary list that just helps keep track of which columns take parameters
			List<OdSqlParameter> paramList=new List<OdSqlParameter>();
			for(int f=0;f<fieldsExceptPri.Count;f++) {
				strb.Append(rn+t4);
				specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
				if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					strb.Append("//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
					continue;
				}
				if(f==0) {
					strb.Append(" ");
				}
				else {
					strb.Append("+");
				}
				if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
					strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) 
				{
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f])) {
						//not a hist table, or is a hist table and field is not inherited from other table
						strb.Append("    DbHelper.Now()+\"");
					}
					//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
						strb.Append("    POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
						strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
					strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
					strb.Append("\"'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+".ToString())+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					strb.Append("\"'\"+POut.TSpan ("+obj+"."+fieldsExceptPri[f].Name+")+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append("\"'\"+POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+".Ticks)+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
					strb.Append("    DbHelper.ParamChar+\"param"+fieldsExceptPri[f].Name);
					paramList.Add(new OdSqlParameter(fieldsExceptPri[f].Name,OdDbType.Text,specialType));
				}
				else if(fieldsExceptPri[f].FieldType.IsEnum) {
					strb.Append("    POut.Int   ((int)"+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else switch(fieldsExceptPri[f].FieldType.Name) {
					default:
						throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
					case "Bitmap":
						strb.Append("    POut.Bitmap("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Boolean":
						strb.Append("    POut.Bool  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Byte":
						strb.Append("    POut.Byte  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Color":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToArgb())+\"");
						break;
					case "DateTime"://This is only for date, not dateT.
						strb.Append("    POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Double":
						strb.Append("		 POut.Double("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Interval":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToInt())+\"");
						break;
					case "Int64":
						strb.Append("    POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Int32":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Single":
						strb.Append("    POut.Float ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "String":
						strb.Append("\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
							+obj+"."+fieldsExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
						break;
					case "TimeSpan":
						strb.Append("    POut.Time  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
				}
				if(f==fieldsExceptPri.Count-2
					&& CrudGenHelper.GetSpecialType(fieldsExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
					&& !CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f+1])) 
				{
					//in case the last field is a timestamp that is not a hist table column inherited from another table
					strb.Append(")\";");
				}
				else if(f<fieldsExceptPri.Count-1) {
					strb.Append(",\"");
				}
				else {
					strb.Append(")\";");
				}
			}
			for(int i=0;i<paramList.Count;i++) {
				strb.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
				strb.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
				strb.Append(rn+t3+"}");
				//example: OdSqlParameter paramNote=new OdSqlParameter("paramNote",
				//           OdDbType.Text,procNote.Note);
				strb.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
					+"OdDbType.Text,");
				if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
					strb.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
				}
				else {
					strb.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
				}
			}
			string paramsString="";//example: ,paramNote,paramAltNote
			for(int i=0;i<paramList.Count;i++) {
				paramsString+=",param"+paramList[i].ParameterName;
			}	
			if(isMobile) {
				strb.Append(rn+t3+"Db.NonQ(command"+paramsString+");//There is no autoincrement in the mobile server.");
				strb.Append(rn+t3+"return "+obj+"."+priKey2.Name+";");
				strb.Append(rn+t2+"}");
			}
			else {
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				strb.Append(rn+t4+"Db.NonQ(command"+paramsString+");");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"else {");
				strb.Append(rn+t4+obj+"."+priKey.Name+"=Db.NonQ(command,true,\""+priKey.Name+"\",\""+obj+"\""+paramsString+");");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"return "+obj+"."+priKey.Name+";");
				strb.Append(rn+t2+"}");
			}
			#endregion Insert
			#region InsertMany
			if(CrudGenHelper.HasBatchWriteMethods(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Inserts many "+typeClass.Name+"s into the database.</summary>");
				strb.Append(rn+t2+"public static void InsertMany(List<"+typeClass.Name+"> list"+typeClass.Name+"s) {");
				strb.Append(rn+t3+"InsertMany(list"+typeClass.Name+"s,false);");
				strb.Append(rn+t2+"}");
				//second override
				strb.Append(rn+rn+t2+"///<summary>Inserts many "+typeClass.Name+"s into the database.  Provides option to use the existing priKey.</summary>");
				strb.Append(rn+t2+"public static void InsertMany(List<"+typeClass.Name+"> list"+typeClass.Name+"s,bool useExistingPK) {");
				#region Oracle Removed
				////The Oracle connector does not support batch commands, so we must loop through and run the inserts one at a time for Oracle.
				////When using Random Primary Keys, for each row inserted, a query has to be run to locate an available key,
				////which defeats the purpose of running batch commands.
				//if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
				//	strb.Append(rn+t3+"if(DataConnection.DBtype==DatabaseType.Oracle) {");
				//}
				//else {
				//	strb.Append(rn+t3+"if(DataConnection.DBtype==DatabaseType.Oracle || PrefC.RandomKeys) {");
				//}
				#endregion Oracle Removed
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {//Don't exclude PrefC check.
					strb.Append(rn+t3+"if(!useExistingPK && PrefC.RandomKeys) {");
						strb.Append(rn+t4+"foreach("+typeClass.Name+" "+obj+" in list"+typeClass.Name+"s) {");
							strb.Append(rn+t5+"Insert("+obj+");");
						strb.Append(rn+t4+"}");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"else {");
				}
				#region MySQL and not using Random Primary Keys.
						strb.Append(rn+t4+"StringBuilder sbCommands=null;");
						strb.Append(rn+t4+"int index=0;");
						strb.Append(rn+t4+"int countRows=0;");
						strb.Append(rn+t4+"while(index < list"+typeClass.Name+"s.Count) {");
							strb.Append(rn+t5+typeClass.Name+" "+obj+"=list"+typeClass.Name+"s[index];");
							strb.Append(rn+t5+"StringBuilder sbRow=new StringBuilder(\"(\");");
							strb.Append(rn+t5+"bool hasComma=false;");
							strb.Append(rn+t5+"if(sbCommands==null) {");
								strb.Append(rn+t6+"sbCommands=new StringBuilder();");
								strb.Append(rn+t6+"sbCommands.Append(\"INSERT INTO "+tablename+" (\");");
								strb.Append(rn+t6+"if(useExistingPK) {");
									strb.Append(rn+t7+"sbCommands.Append(\""+priKey.Name+",\");");
								strb.Append(rn+t6+"}");
								strb.Append(rn+t6+"sbCommands.Append(\"");
								for(int f=0;f<fieldsExceptPri.Count;f++) {
									specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
									if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
										continue;
									}
									if(f>0) {
										strb.Append(",");
									}
									strb.Append(fieldsExceptPri[f].Name);
								}
								strb.Append(") VALUES \");");
								strb.Append(rn+t6+"countRows=0;");
							strb.Append(rn+t5+"}");//end if
							strb.Append(rn+t5+"else {");//This is not the first row 
								strb.Append(rn+t6+"hasComma=true;");
							strb.Append(rn+t5+"}");//end else
							strb.Append(rn+t5+"if(useExistingPK) {");
								strb.Append(rn+t6+"sbRow.Append(POut.Long("+obj+"."+priKey.Name+")); sbRow.Append(\",\");");
							strb.Append(rn+t5+"}");
							for(int f=0;f<fieldsExceptPri.Count;f++) {
								strb.Append(rn+t5);
								specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
								if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
									strb.Append("//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
									continue;
								}
								if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
									//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
									strb.Append("sbRow.Append(POut.DateT("+obj+"."+fieldsExceptPri[f].Name+"));");
								}
								else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
									|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
									|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
									|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
								{
									if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f])) {
										//not a hist table, or is a hist table and field is not inherited from other table
										strb.Append("sbRow.Append(DbHelper.Now());");
									}
									//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
									else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
										strb.Append("sbRow.Append(POut.Date("+obj+"."+fieldsExceptPri[f].Name+"));");
									}
									else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
										strb.Append("sbRow.Append(POut.DateT("+obj+"."+fieldsExceptPri[f].Name+"));");
									}
								}
								else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
									strb.Append("sbRow.Append(POut.DateT("+obj+"."+fieldsExceptPri[f].Name+"));");
								}
								else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
									strb.Append("sbRow.Append(\"'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+".ToString())+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
									strb.Append("sbRow.Append(\"'\"+POut.TSpan ("+obj+"."+fieldsExceptPri[f].Name+")+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
									strb.Append("sbRow.Append(\"'\"+POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+".Ticks)+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
									strb.Append("sbRow.Append(\"'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+")+\"'\");");
								}
								else if(fieldsExceptPri[f].FieldType.IsEnum) {
									strb.Append("sbRow.Append(POut.Int((int)"+obj+"."+fieldsExceptPri[f].Name+"));");
								}
								else switch(fieldsExceptPri[f].FieldType.Name) {
									default:
										throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
									case "Bitmap":
										strb.Append("sbRow.Append(POut.Bitmap("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Boolean":
										strb.Append("sbRow.Append(POut.Bool("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Byte":
										strb.Append("sbRow.Append(POut.Byte("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Color":
										strb.Append("sbRow.Append(POut.Int("+obj+"."+fieldsExceptPri[f].Name+".ToArgb()));");
										break;
									case "DateTime"://This is only for date, not dateT.
										strb.Append("sbRow.Append(POut.Date("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Double":
										strb.Append("sbRow.Append(POut.Double("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Interval":
										strb.Append("sbRow.Append(POut.Int("+obj+"."+fieldsExceptPri[f].Name+".ToInt()));");
										break;
									case "Int64":
										strb.Append("sbRow.Append(POut.Long("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Int32":
										strb.Append("sbRow.Append(POut.Int("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "Single":
										strb.Append("sbRow.Append(POut.Float("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
									case "String":
										strb.Append("sbRow.Append(\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
											+obj+"."+fieldsExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'\");");
										break;
									case "TimeSpan":
										strb.Append("sbRow.Append(POut.Time("+obj+"."+fieldsExceptPri[f].Name+"));");
										break;
								}
								if(f==fieldsExceptPri.Count-2
									&& CrudGenHelper.GetSpecialType(fieldsExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
									&& !CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f+1])) 
								{
									//in case the last field is a timestamp that is not a hist table column inherited from another table
									strb.Append(" sbRow.Append(\")\");");
								}
								else if(f<fieldsExceptPri.Count-1) {
									strb.Append(" sbRow.Append(\",\");");
								}
								else {
									strb.Append(" sbRow.Append(\")\");");
								}
							}
							strb.Append(rn+t5+"if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {");//plus 1 for comma
								strb.Append(rn+t6+"Db.NonQ(sbCommands.ToString());");
								//There has not been a need yet for returning the new primary keys after a batch insert.
								//We need to do some testing before we can implement this feature in the future.
								//http://stackoverflow.com/questions/7333524/how-can-i-insert-many-rows-into-a-mysql-table-and-return-the-new-ids
								//We can possbily call LAST_INSERT_ID() to get the ID of the last row, and then loop backwards decrementing the key values,
								//assuming the values would be sequential even if another user inserts a new row while our batch insert is running.
								strb.Append(rn+t6+"sbCommands=null;");
							strb.Append(rn+t5+"}");//end if
							strb.Append(rn+t5+"else {");
								strb.Append(rn+t6+"if(hasComma) {");
									strb.Append(rn+t7+"sbCommands.Append(\",\");");
								strb.Append(rn+t6+"}");
								strb.Append(rn+t6+"sbCommands.Append(sbRow.ToString());");
								strb.Append(rn+t6+"countRows++;");
								strb.Append(rn+t6+"if(index==list"+typeClass.Name+"s.Count-1) {");
									strb.Append(rn+t7+"Db.NonQ(sbCommands.ToString());");
								strb.Append(rn+t6+"}");//end else
								strb.Append(rn+t6+"index++;");
							strb.Append(rn+t5+"}");//end else
						strb.Append(rn+t4+"}");//end while
				#endregion MySQL and not using Random Primary Keys.
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {//Don't exclude PrefC check.
					strb.Append(rn+t3+"}");
				}
				strb.Append(rn+t2+"}");
			}
			#endregion InsertMany
			#region InsertNoCache
			//InsertNoCache---------------------------------------------------------------------------------------------
			fieldsExceptPri=null; 
			if(isMobile) {
				//Mobile sync does not use cache already.
			}
			else {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey);
				strb.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Returns the new priKey.  Doesn't use the cache.</summary>");
				strb.Append(rn+t2+"public static long InsertNoCache("+typeClass.Name+" "+obj+") {");
				#region Oracle Removed
					//strb.Append(rn+t3+"if(DataConnection.DBtype==DatabaseType.MySql) {");
					//	strb.Append(rn+t4+"return InsertNoCache("+obj+",false);");
					//strb.Append(rn+t3+"}");
					//strb.Append(rn+t3+"else {");
					//	strb.Append(rn+t4+"if(DataConnection.DBtype==DatabaseType.Oracle) {");
					//		strb.Append(rn+t5+obj+"."+priKey.Name+"=DbHelper.GetNextOracleKey(\""+tablename+"\",\""+priKey.Name+"\"); //Cacheless method");
					//	strb.Append(rn+t4+"}");
					//	strb.Append(rn+t4+"return InsertNoCache("+obj+",true);");
					//strb.Append(rn+t3+"}");
				#endregion Oracle Removed
				strb.Append(rn+t3+"return InsertNoCache("+obj+",false);");
				strb.Append(rn+t2+"}");
				//second override
				strb.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>");
				strb.Append(rn+t2+"public static long InsertNoCache("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);");
				}
					strb.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
						strb.Append(rn+t4+"command+=\""+priKey.Name+",\";");
					strb.Append(rn+t3+"}");
				}
				else {
					strb.Append(rn+t3+"if(!useExistingPK && isRandomKeys) {");
						strb.Append(rn+t4+obj+"."+priKey.Name+"=ReplicationServers.GetKeyNoCache(\""+tablename+"\",\""+priKey.Name+"\");");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"if(isRandomKeys || useExistingPK) {");
						strb.Append(rn+t4+"command+=\""+priKey.Name+",\";");
					strb.Append(rn+t3+"}");
				}
					strb.Append(rn+t3+"command+=\"");
					for(int f=0;f<fieldsExceptPri.Count;f++) {
					specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
							continue;
						}
						if(f>0) {
							strb.Append(",");
						}
						strb.Append(fieldsExceptPri[f].Name);
					}
					strb.Append(") VALUES(\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					strb.Append(rn+t3+"if(isRandomKeys || useExistingPK) {");
				}
						strb.Append(rn+t4+"command+=POut.Long("+obj+"."+priKey.Name+")+\",\";");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"command+=");
			}
			//a quick and dirty temporary list that just helps keep track of which columns take parameters
			paramList=new List<OdSqlParameter>();
			#region FieldsExceptPri
			for(int f=0;f<fieldsExceptPri.Count;f++) {
				strb.Append(rn+t4);
				specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
				if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					strb.Append("//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
					continue;
				}
				if(f==0) {
					strb.Append(" ");
				}
				else {
					strb.Append("+");
				}
				if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
					strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
				{
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f])) {
						//not a hist table, or is a hist table and field is not inherited from other table
						strb.Append("    DbHelper.Now()+\"");
					}
					//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
						strb.Append("    POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
						strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
					strb.Append("    POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
					strb.Append("\"'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+".ToString())+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					strb.Append("\"'\"+POut.TSpan ("+obj+"."+fieldsExceptPri[f].Name+")+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append("\"'\"+POut.Long("+obj+"."+fieldsExceptPri[f].Name+".Ticks)+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
					strb.Append("    DbHelper.ParamChar+\"param"+fieldsExceptPri[f].Name);
					paramList.Add(new OdSqlParameter(fieldsExceptPri[f].Name,OdDbType.Text,specialType));
				}
				else if(fieldsExceptPri[f].FieldType.IsEnum) {
					strb.Append("    POut.Int   ((int)"+obj+"."+fieldsExceptPri[f].Name+")+\"");
				}
				else switch(fieldsExceptPri[f].FieldType.Name) {
					default:
						throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
					case "Bitmap":
						strb.Append("    POut.Bitmap("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Boolean":
						strb.Append("    POut.Bool  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Byte":
						strb.Append("    POut.Byte  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Color":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToArgb())+\"");
						break;
					case "DateTime"://This is only for date, not dateT.
						strb.Append("    POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Double":
						strb.Append("	   POut.Double("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Interval":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToInt())+\"");
						break;
					case "Int64":
						strb.Append("    POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Int32":
						strb.Append("    POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "Single":
						strb.Append("    POut.Float ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
					case "String":
						strb.Append("\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
							+obj+"."+fieldsExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
						break;
					case "TimeSpan":
						strb.Append("    POut.Time  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
						break;
				}
				if(f==fieldsExceptPri.Count-2
					&& CrudGenHelper.GetSpecialType(fieldsExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
					&& !CrudGenHelper.IsFieldInherited(typeClass,fieldsExceptPri[f+1])) 
				{
					//in case the last field is a timestamp that is not a hist table column inherited from another table
					strb.Append(")\";");
				}
				else if(f<fieldsExceptPri.Count-1) {
					strb.Append(",\"");
				}
				else {
					strb.Append(")\";");
				}
			}
			#endregion FieldsExceptPri
			for(int i=0;i<paramList.Count;i++) {
					strb.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
						strb.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					strb.Append(rn+t3+"}");
					//example: OdSqlParameter paramNote=new OdSqlParameter("paramNote",
					//           OdDbType.Text,procNote.Note);
					strb.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
				if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
					strb.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
				}
				else {
					strb.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
				}
			}
			paramsString="";//example: ,paramNote,paramAltNote
			for(int i=0;i<paramList.Count;i++) {
				paramsString+=",param"+paramList[i].ParameterName;
			}	
			if(isMobile) {
				//Not supported.
			}
			else {
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					strb.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					strb.Append(rn+t3+"if(useExistingPK || isRandomKeys) {");
				}
						strb.Append(rn+t4+"Db.NonQ(command"+paramsString+");");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"else {");
						strb.Append(rn+t4+obj+"."+priKey.Name+"=Db.NonQ(command,true,\""+priKey.Name+"\",\""+obj+"\""+paramsString+");");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"return "+obj+"."+priKey.Name+";");
				strb.Append(rn+t2+"}");
			}
			#endregion InsertNoCache
			#region Update
			//Update---------------------------------------------------------------------------------------------
			//get the longest fieldname for alignment purposes
			int longestField=fieldsInDb.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
			if(!CrudGenHelper.IsTableHist(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Updates one "+typeClass.Name+" in the database.</summary>");
				strb.Append(rn+t2+"public static void Update("+typeClass.Name+" "+obj+") {");
				strb.Append(rn+t3+"string command=\"UPDATE "+tablename+" SET \"");
				for(int f=0;f<fieldsExceptPri.Count;f++) {
					if(isMobile && fieldsExceptPri[f]==priKey1) {//2 already skipped
						continue;
					}
					specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						strb.Append(rn+t4+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						strb.Append(rn+t4+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						strb.Append(rn+t4+"//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						strb.Append(rn+t4+"//"+fieldsExceptPri[f].Name+" excluded from update");
						continue;
					}
					strb.Append(rn+t4+"+\""+fieldsExceptPri[f].Name.PadRight(longestField,' ')+"= ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						strb.Append(" \"+POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						strb.Append(" \"+POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						strb.Append(" \"+POut.DateT ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						strb.Append("'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						strb.Append("'\"+POut.TSpan ("+obj+"."+fieldsExceptPri[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						strb.Append(" \"+POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+".Ticks)+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
						strb.Append(" \"+DbHelper.ParamChar+\"param"+fieldsExceptPri[f].Name);
						//paramList is already set above
					}
					else if(fieldsExceptPri[f].FieldType.IsEnum) {
						strb.Append(" \"+POut.Int   ((int)"+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else switch(fieldsExceptPri[f].FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
						case "Bitmap":
							strb.Append(" \"+POut.Bitmap("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Boolean":
							strb.Append(" \"+POut.Bool  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Byte":
							strb.Append(" \"+POut.Byte  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Color":
							strb.Append(" \"+POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToArgb())+\"");
							break;
						case "DateTime"://This is only for date, not dateT
							strb.Append(" \"+POut.Date  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Double":
							strb.Append(" \"+POut.Double("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Interval":
							strb.Append(" \"+POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+".ToInt())+\"");
							break;
						case "Int64":
							strb.Append(" \"+POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Int32":
							strb.Append(" \"+POut.Int   ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "Single":
							strb.Append(" \"+POut.Float ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
						case "String":
							strb.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
								+obj+"."+fieldsExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
							break;
						case "TimeSpan":
							strb.Append(" \"+POut.Time  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
							break;
					}
					//If ALL the rest of the fields should be skipped, don't add comma
					if(!fieldsExceptPri.Skip(f+1).All(x => EnumTools.HasAnyFlag(CrudGenHelper.GetSpecialType(x),
						CrudSpecialColType.TimeStamp,
						CrudSpecialColType.DateEntry,
						CrudSpecialColType.DateTEntry,
						CrudSpecialColType.ExcludeFromUpdate))) 
					{
						strb.Append(",");
					}
					strb.Append(" \"");
				}
				if(isMobile) {
					strb.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+obj+"."+priKey1.Name+")+\" AND "+priKey2.Name+" = \"+POut.Long("+obj+"."+priKey2.Name+");");
				}
				else {
					strb.Append(rn+t4+"+\"WHERE "+priKey.Name+" = \"+POut.Long("+obj+"."+priKey.Name+");");
				}
				for(int i=0;i<paramList.Count;i++) {
					strb.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					strb.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						strb.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						strb.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				strb.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				strb.Append(rn+t2+"}");
			}
			#endregion Update
			#region Update 2nd override
			//Update, 2nd override-------------------------------------------------------------------------------
			//NOTE: If any changes are made to Update 2nd override, they need to be reflected in UpdateComparison as well!!!
			if(!isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Updates one "+typeClass.Name+" in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>");
				strb.Append(rn+t2+"public static bool Update("+typeClass.Name+" "+obj+","+typeClass.Name+" "+oldObj+") {");
				strb.Append(rn+t3+"string command=\"\";");
				for(int f=0;f<fieldsExceptPri.Count;f++) {
					//if(isMobile && fieldsExceptPri[f]==priKey1) {//2 already skipped
					//	continue;
					//}
					specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" excluded from update");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)//DateEntryEditable special type is date data type in the db
						|| (!specialType.HasFlag(CrudSpecialColType.DateT)//if not special type DateT
							&& !specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)//and not DateTEntryEditable
							&& fieldsExceptPri[f].FieldType.Name=="DateTime"))//and field name is DateTime, then it is a date data type in the db
					{
						strb.Append(rn+t3+"if("+obj+"."+fieldsExceptPri[f].Name+".Date != "+oldObj+"."+fieldsExceptPri[f].Name+".Date) {");
					}
					else {
						strb.Append(rn+t3+"if("+obj+"."+fieldsExceptPri[f].Name+" != "+oldObj+"."+fieldsExceptPri[f].Name+") {");
					}
					strb.Append(rn+t4+"if(command!=\"\") { command+=\",\";}");
					strb.Append(rn+t4+"command+=\""+fieldsExceptPri[f].Name+" = ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						strb.Append("\"+POut.DateT("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						strb.Append("\"+POut.Date("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						strb.Append("\"+POut.DateT("+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						strb.Append("'\"+POut.String("+obj+"."+fieldsExceptPri[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						strb.Append("'\"+POut.TSpan ("+obj+"."+fieldsExceptPri[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						strb.Append("'\"+POut.Long  ("+obj+"."+fieldsExceptPri[f].Name+".Ticks)+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
						strb.Append("\"+DbHelper.ParamChar+\"param"+fieldsExceptPri[f].Name);
						//paramList is already set above
					}
					else if(fieldsExceptPri[f].FieldType.IsEnum) {
						strb.Append("\"+POut.Int   ((int)"+obj+"."+fieldsExceptPri[f].Name+")+\"");
					}
					else switch(fieldsExceptPri[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
							case "Boolean":
								strb.Append("\"+POut.Bool("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Bitmap":
								strb.Append("\"+POut.Bitmap("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Byte":
								strb.Append("\"+POut.Byte("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Color":
								strb.Append("\"+POut.Int("+obj+"."+fieldsExceptPri[f].Name+".ToArgb())+\"");
								break;
							case "DateTime"://This is only for date, not dateT.
								strb.Append("\"+POut.Date("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Double":
								strb.Append("\"+POut.Double("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Interval":
								strb.Append("\"+POut.Int("+obj+"."+fieldsExceptPri[f].Name+".ToInt())+\"");
								break;
							case "Int64":
								strb.Append("\"+POut.Long("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Int32":
								strb.Append("\"+POut.Int("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "Single":
								strb.Append("\"+POut.Float("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
							case "String":
								strb.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
									+obj+"."+fieldsExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
								break;
							case "TimeSpan":
								strb.Append("\"+POut.Time  ("+obj+"."+fieldsExceptPri[f].Name+")+\"");
								break;
						}
					strb.Append("\";");
					strb.Append(rn+t3+"}");
				}
				strb.Append(rn+t3+"if(command==\"\") {");
				strb.Append(rn+t4+"return false;");
				strb.Append(rn+t3+"}");
				for(int i=0;i<paramList.Count;i++) {
					strb.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					strb.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
					+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						strb.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						strb.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				strb.Append(rn+t3+"command=\"UPDATE "+tablename+" SET \"+command");
				strb.Append(rn+t4+"+\" WHERE "+priKey.Name+" = \"+POut.Long("+obj+"."+priKey.Name+");");
				strb.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				strb.Append(rn+t3+"return true;");
				strb.Append(rn+t2+"}");
			}
			#endregion Update 2nd override
			#region UpdateComparison
			//UpdateComparison-------------------------------------------------------------------------------
			if(!isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Returns true if Update("+typeClass.Name+","+typeClass.Name+") would make changes to the database."
					+rn+t2+"///Does not make any changes to the database and can be called before remoting role is checked.</summary>");
				strb.Append(rn+t2+"public static bool UpdateComparison("+typeClass.Name+" "+obj+","+typeClass.Name+" "+oldObj+") {");
				for(int f = 0;f<fieldsExceptPri.Count;f++) {
					//if(isMobile && fieldsExceptPri[f]==priKey1) {//2 already skipped
					//	continue;
					//}
					specialType=CrudGenHelper.GetSpecialType(fieldsExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						strb.Append(rn+t3+"//"+fieldsExceptPri[f].Name+" excluded from update");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)//DateEntryEditable special type is date data type in the db
						|| (!specialType.HasFlag(CrudSpecialColType.DateT)//if not special type DateT
							&& !specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)//and not DateTEntryEditable
							&& fieldsExceptPri[f].FieldType.Name=="DateTime"))//and field name is DateTime, then it is a date data type in the db
					{
						strb.Append(rn+t3+"if("+obj+"."+fieldsExceptPri[f].Name+".Date != "+oldObj+"."+fieldsExceptPri[f].Name+".Date) {");
					}
					else {
						strb.Append(rn+t3+"if("+obj+"."+fieldsExceptPri[f].Name+" != "+oldObj+"."+fieldsExceptPri[f].Name+") {");
					}
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						strb.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
						strb.Append(rn+t4+"return true");
						//paramList is already set above
					}
					else if(fieldsExceptPri[f].FieldType.IsEnum) {
						strb.Append(rn+t4+"return true");
					}
					else switch(fieldsExceptPri[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+fieldsExceptPri[f].FieldType.Name);
							case "Boolean":
								strb.Append(rn+t4+"return true");
								break;
							case "Bitmap":
								strb.Append(rn+t4+"return true");
								break;
							case "Byte":
								strb.Append(rn+t4+"return true");
								break;
							case "Color":
								strb.Append(rn+t4+"return true");
								break;
							case "DateTime"://This is only for date, not dateT.
								strb.Append(rn+t4+"return true");
								break;
							case "Double":
								strb.Append(rn+t4+"return true");
								break;
							case "Interval":
								strb.Append(rn+t4+"return true");
								break;
							case "Int64":
								strb.Append(rn+t4+"return true");
								break;
							case "Int32":
								strb.Append(rn+t4+"return true");
								break;
							case "Single":
								strb.Append(rn+t4+"return true");
								break;
							case "String":
								strb.Append(rn+t4+"return true");
								break;
							case "TimeSpan":
								strb.Append(rn+t4+"return true");
								break;
						}
					strb.Append(";");
					strb.Append(rn+t3+"}");
				}
				strb.Append(rn+t3+"return false;");
				strb.Append(rn+t2+"}");
			}
			#endregion UpdateComparison
			#region UpdateCemt
			//UpdateCemt---------------------------------------------------------------------------------------------
			//get the longest fieldname for alignment purposes
			if(CrudGenHelper.IsTableSkipCemt(typeClass) && !isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				FieldInfo cemtSyncKey=CrudGenHelper.GetCemtSyncKey(fields,typeClass.Name);
				List<FieldInfo> listFieldsExceptCEMT=CrudGenHelper.GetFieldsExceptNotCemt(fieldsExceptPri.ToArray());
				longestField=listFieldsExceptCEMT.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
				strb.Append(rn+rn+t2+"///<summary>Updates columns that do not have the '"+nameof(CrudColumnAttribute.IsNotCemtColumn)+"' attribute. Uses the '"+cemtSyncKey.Name+"' column instead of the PK column.</summary>");
				strb.Append(rn+t2+"public static void UpdateCemt("+typeClass.Name+" "+obj+") {");
				strb.Append(rn+t3+"string command=\"UPDATE "+tablename+" SET \"");
				for(int f = 0;f < listFieldsExceptCEMT.Count;f++) {
					if(isMobile && listFieldsExceptCEMT[f]==priKey1) {//2 already skipped
						continue;
					}
					specialType=CrudGenHelper.GetSpecialType(listFieldsExceptCEMT[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						strb.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						strb.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						strb.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						strb.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" excluded from update");
						continue;
					}
					strb.Append(rn+t4+"+\""+listFieldsExceptCEMT[f].Name.PadRight(longestField,' ')+"= ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						strb.Append(" \"+POut.DateT ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						strb.Append(" \"+POut.Date  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						strb.Append(" \"+POut.DateT ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						strb.Append("'\"+POut.String("+obj+"."+listFieldsExceptCEMT[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						strb.Append("'\"+POut.TSpan ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						strb.Append(" \"+POut.Long  ("+obj+"."+listFieldsExceptCEMT[f].Name+".Ticks)+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TextIsClob)) {
						strb.Append(" \"+DbHelper.ParamChar+\"param"+listFieldsExceptCEMT[f].Name);
						//paramList is already set above
					}
					else if(listFieldsExceptCEMT[f].FieldType.IsEnum) {
						strb.Append(" \"+POut.Int   ((int)"+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else switch(listFieldsExceptCEMT[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+listFieldsExceptCEMT[f].FieldType.Name);
							case "Bitmap":
								strb.Append(" \"+POut.Bitmap("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Boolean":
								strb.Append(" \"+POut.Bool  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Byte":
								strb.Append(" \"+POut.Byte  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Color":
								strb.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+".ToArgb())+\"");
								break;
							case "DateTime"://This is only for date, not dateT
								strb.Append(" \"+POut.Date  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Double":
								strb.Append(" \"+POut.Double("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Interval":
								strb.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+".ToInt())+\"");
								break;
							case "Int64":
								strb.Append(" \"+POut.Long  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Int32":
								strb.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Single":
								strb.Append(" \"+POut.Float ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "String":
								strb.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
									+obj+"."+listFieldsExceptCEMT[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
								break;
							case "TimeSpan":
								strb.Append(" \"+POut.Time  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
						}
					//If ALL the rest of the fields should be skipped, don't add comma
					if(!listFieldsExceptCEMT.Skip(f+1).All(x => EnumTools.HasAnyFlag(CrudGenHelper.GetSpecialType(x),
						CrudSpecialColType.TimeStamp,
						CrudSpecialColType.DateEntry,
						CrudSpecialColType.DateTEntry,
						CrudSpecialColType.ExcludeFromUpdate))) {
						strb.Append(",");
					}
					strb.Append(" \"");
				}
				if(isMobile) {
					strb.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+obj+"."+priKey1.Name+")+\" AND "+priKey2.Name+" = \"+POut.Long("+obj+"."+priKey2.Name+");");
				}
				else {
					strb.Append(rn+t4+"+\"WHERE "+cemtSyncKey.Name+" = \"+POut.Long("+obj+"."+cemtSyncKey.Name+");");
				}
				for(int i = 0;i<paramList.Count;i++) {
					strb.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					strb.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					strb.Append(rn+t3+"}");
					strb.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						strb.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						strb.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				strb.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				strb.Append(rn+t2+"}");
			}
			#endregion UpdateCemt
			#region Delete
			//Delete---------------------------------------------------------------------------------------------
			if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
				strb.Append(rn+rn+t2+"//Delete not allowed for this table");
				strb.Append(rn+t2+"//public static void Delete(long "+priKeyParam+") {");
				strb.Append(rn+t2+"//");
				strb.Append(rn+t2+"//}");
			}
			else {
				strb.Append(rn+rn+t2+"///<summary>Deletes one "+typeClass.Name+" from the database.</summary>");
				if(isMobile) {
					strb.Append(rn+t2+"public static void Delete(long "+priKeyParam1+",long "+priKeyParam2+") {");
					strb.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
					strb.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+priKeyParam1+")+\" AND "+priKey2.Name+" = \"+POut.Long("+priKeyParam2+");");
				}
				else {
					strb.Append(rn+t2+"public static void Delete(long "+priKeyParam+") {");
					if(!listAuditTrailPerms.IsNullOrEmpty()) {
						strb.Append(rn+t3+"ClearFkey("+priKeyParam+");");
					}
					strb.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
					strb.Append(rn+t4+"+\"WHERE "+priKey.Name+" = \"+POut.Long("+priKeyParam+");");
				}
				strb.Append(rn+t3+"Db.NonQ(command);");
				strb.Append(rn+t2+"}");
			}
			#endregion Delete
			#region Delete Many
			//Delete Many---------------------------------------------------------------------------------------------
			if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
				strb.Append(rn+rn+t2+"//Delete not allowed for this table");
				strb.Append(rn+t2+"//public static void DeleteMany(List<long> "+nameListParam+") {");
				strb.Append(rn+t2+"//");
				strb.Append(rn+t2+"//}");
			}
			else {
				strb.Append(rn+rn+t2+"///<summary>Deletes many "+typeClass.Name+"s from the database.</summary>");
				strb.Append(rn+t2+"public static void DeleteMany(List<long> "+nameListParam+") {");
				strb.Append(rn+t3+"if("+nameListParam+"==null || "+nameListParam+".Count==0) {");
				strb.Append(rn+t4+"return;");
				strb.Append(rn+t3+"}");
				if(!listAuditTrailPerms.IsNullOrEmpty()) {
					strb.Append(rn+t3+"ClearFkey("+nameListParam+");");
				}
				strb.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
				strb.Append(rn+t4+"+\"WHERE "+priKey.Name+" IN(\"+string.Join(\",\","+nameListParam+".Select(x => POut.Long(x)))+\")\";");
				strb.Append(rn+t3+"Db.NonQ(command);");
				strb.Append(rn+t2+"}");
			}
			#endregion Delete Many
			#region Sync
			//Synch-----------------------------------------------------------------------------------------
			if(CrudGenHelper.IsSynchable(typeClass) || CrudGenHelper.IsSynchableBatchWriteMethods(typeClass)) {
				strb.Append(rn+rn+t2+"///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.");
				if(CrudGenHelper.IsSynchableBatchWriteMethods(typeClass) && CrudGenHelper.HasBatchWriteMethods(typeClass)) {
					strb.Append(rn+t2+"///Caution: Uses InsertMany for inserts, so the objects in listNew will not have primary keys set.");
				}
				if(CrudGenHelper.IsSecurityStamped(typeClass)) {//sec tables that are synchable must have userNum passed in.
					strb.Append(rn+t2+"///Supply Security.CurUser.UserNum, used to set the SecUserNumEntry field for Inserts.</summary>");
					strb.Append(rn+t2+"public static bool Sync(List<"+typeClass.Name+"> listNew,List<"+typeClass.Name+"> listDB,long userNum) {");
				}
				else {
					strb.Append("</summary>");
					strb.Append(rn+t2+"public static bool Sync(List<"+typeClass.Name+"> listNew,List<"+typeClass.Name+"> listDB) {");
				}
				strb.Append(rn+t3+"//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.");
				strb.Append(rn+t3+"List<"+typeClass.Name+"> listIns    =new List<"+typeClass.Name+">();");
				strb.Append(rn+t3+"List<"+typeClass.Name+"> listUpdNew =new List<"+typeClass.Name+">();");
				strb.Append(rn+t3+"List<"+typeClass.Name+"> listUpdDB  =new List<"+typeClass.Name+">();");
				strb.Append(rn+t3+"List<"+typeClass.Name+"> listDel    =new List<"+typeClass.Name+">();");
				strb.Append(rn+t3+"listNew.Sort(("+typeClass.Name+" x,"+typeClass.Name+" y) => { return x."+priKey.Name+".CompareTo(y."+priKey.Name+"); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.");
				strb.Append(rn+t3+"listDB.Sort(("+typeClass.Name+" x,"+typeClass.Name+" y) => { return x."+priKey.Name+".CompareTo(y."+priKey.Name+"); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.");
				strb.Append(rn+t3+"int idxNew=0;");
				strb.Append(rn+t3+"int idxDB=0;");
				strb.Append(rn+t3+"int rowsUpdatedCount=0;");
				strb.Append(rn+t3+""+typeClass.Name+" fieldNew;");
				strb.Append(rn+t3+""+typeClass.Name+" fieldDB;");
				strb.Append(rn+t3+"//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.");
				strb.Append(rn+t3+"//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.");
				strb.Append(rn+t3+"while(idxNew<listNew.Count || idxDB<listDB.Count) {");
				strb.Append(rn+t4+"fieldNew=null;");
				strb.Append(rn+t4+"if(idxNew<listNew.Count) {");
				strb.Append(rn+t5+"fieldNew=listNew[idxNew];");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"fieldDB=null;");
				strb.Append(rn+t4+"if(idxDB<listDB.Count) {");
				strb.Append(rn+t5+"fieldDB=listDB[idxDB];");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"//begin compare");
				strb.Append(rn+t4+"if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.");
				strb.Append(rn+t5+"listIns.Add(fieldNew);");
				strb.Append(rn+t5+"idxNew++;");
				strb.Append(rn+t5+"continue;");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.");
				strb.Append(rn+t5+"listDel.Add(fieldDB);");
				strb.Append(rn+t5+"idxDB++;");
				strb.Append(rn+t5+"continue;");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"else if(fieldNew."+priKey.Name+"<fieldDB."+priKey.Name+") {//newPK less than dbPK, newItem is 'next'");
				strb.Append(rn+t5+"listIns.Add(fieldNew);");
				strb.Append(rn+t5+"idxNew++;");
				strb.Append(rn+t5+"continue;");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"else if(fieldNew."+priKey.Name+">fieldDB."+priKey.Name+") {//dbPK less than newPK, dbItem is 'next'");
				strb.Append(rn+t5+"listDel.Add(fieldDB);");
				strb.Append(rn+t5+"idxDB++;");
				strb.Append(rn+t5+"continue;");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t4+"//Both lists contain the 'next' item, update required");
				strb.Append(rn+t4+"listUpdNew.Add(fieldNew);");
				strb.Append(rn+t4+"listUpdDB.Add(fieldDB);");
				strb.Append(rn+t4+"idxNew++;");
				strb.Append(rn+t4+"idxDB++;");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"//Commit changes to DB");
				if(CrudGenHelper.IsSynchableBatchWriteMethods(typeClass) && CrudGenHelper.HasBatchWriteMethods(typeClass)) {
					if(CrudGenHelper.IsSecurityStamped(typeClass)) {
						//if this table IsSecurityStamped, there is a SecUserNumEntry field that needs to be set to the userNum passed in for inserts
						strb.Append(rn+t3+"listIns.ForEach(x => x.SecUserNumEntry=userNum);");
					}
					strb.Append(rn+t3+"InsertMany(listIns);");
				}
				else {
					strb.Append(rn+t3+"for(int i=0;i<listIns.Count;i++) {");
					if(CrudGenHelper.IsSecurityStamped(typeClass)) {
						//if this table IsSecurityStamped, there is a SecUserNumEntry field that needs to be set to the userNum passed in for inserts
						strb.Append(rn+t4+"listIns[i].SecUserNumEntry=userNum;");
					}
					strb.Append(rn+t4+"Insert(listIns[i]);");
					strb.Append(rn+t3+"}");
				}
				strb.Append(rn+t3+"for(int i=0;i<listUpdNew.Count;i++) {");
				strb.Append(rn+t4+"if(Update(listUpdNew[i],listUpdDB[i])) {");
				strb.Append(rn+t5+"rowsUpdatedCount++;");
				strb.Append(rn+t4+"}");
				strb.Append(rn+t3+"}");
				if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
					//When Crud.Delete() is forbidden, the only way we could possibly delete a row is if the S class has a specific Delete() function defined.
					//There are very few classes which are both Synchable and where Crud.Delete() is forbidden.
					strb.Append(rn+t3+"for(int i=0;i<listDel.Count;i++) {");
					strb.Append(rn+t4+typeClass.Name+"s.Delete(listDel[i]."+priKey.Name+");");
					strb.Append(rn+t3+"}");
				}
				else {
					strb.Append(rn+t3+"DeleteMany(listDel.Select(x => x."+priKey.Name+").ToList());");
				}
				strb.Append(rn+t3+"if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {");
				strb.Append(rn+t4+"return true;");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"return false;");
				strb.Append(rn+t2+"}");
			}
			#endregion Sync
			#region ConvertToM
			if(isMobile) {
				//ConvertToM------------------------------------------------------------------------------------------
				Type typeClassReg=CrudGenHelper.GetTypeFromMType(typeClass.Name,tableTypes);//gets the non-mobile type
				if(typeClassReg==null) {
					strb.Append(rn+rn+t2+"//ConvertToM not applicable.");
				}
				else{
					string tablenameReg=CrudGenHelper.GetTableName(typeClassReg);//in lowercase now.
					string objReg=typeClassReg.Name.Substring(0,1).ToLower()+typeClassReg.Name.Substring(1);//lowercase initial letter.  Example feeSched
					FieldInfo[] fieldsReg=typeClassReg.GetFields();//We can't assume they are in the correct order.
					List<FieldInfo> fieldsInDbReg=CrudGenHelper.GetFieldsExceptNotDb(fieldsReg);
					strb.Append(rn+rn+t2+"///<summary>Converts one "+typeClassReg.Name+" object to its mobile equivalent.  Warning! CustomerNum will always be 0.</summary>");
					strb.Append(rn+t2+"public static "+typeClass.Name+" ConvertToM("+typeClassReg.Name+" "+objReg+") {");
					strb.Append(rn+t3+typeClass.Name+" "+obj+"=new "+typeClass.Name+"();");
					for(int f=0;f<fieldsInDb.Count;f++) {
						if(fieldsInDb[f].Name=="CustomerNum") {
							strb.Append(rn+t3+"//CustomerNum cannot be set.  Remains 0.");
							continue;
						}
						bool matchfound=false;
						for(int r=0;r<fieldsInDbReg.Count;r++) {
							if(fieldsInDb[f].Name==fieldsInDbReg[r].Name) {
								strb.Append(rn+t3+obj+"."+fieldsInDb[f].Name.PadRight(longestField,' ')+"="+objReg+"."+fieldsInDbReg[r].Name+";");
								matchfound=true;
							}
						}
						if(!matchfound) {
							throw new ApplicationException("Match not found.");
						}
					}
					strb.Append(rn+t3+"return "+obj+";");
					strb.Append(rn+t2+"}");
				}
			}
			#endregion ConvertToM
			#region ClearFkey(long)
			if(!listAuditTrailPerms.IsNullOrEmpty()) {  //If there are any AuditPerms set for this table
				strb.Append(rn+rn+t2+"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+" as FKey and are related to "+typeClass.Name+".");
				strb.Append(rn+t2+"///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClass.Name+@" table type.</summary>");
				strb.Append(rn+t2+"public static void ClearFkey(long "+priKeyParam+") {");
				List<string> listPermTypes=new List<string>();
				for(int i=0;i<listAuditTrailPerms.Count;i++) {
					listPermTypes.Add(((int)listAuditTrailPerms[i]).ToString());
				}
				strb.Append(rn+t3+"if("+priKeyParam+"==0) {");
				strb.Append(rn+t4+"return;");
				strb.Append(rn+t3+"}");
				strb.Append(rn+t3+"string command=\"UPDATE securitylog SET FKey=0 WHERE FKey=\"+POut.Long("+priKeyParam+")+"
					+"\" AND PermType IN ("+String.Join(",",listPermTypes)+")\";");  
				//If we wanted to make this more readable we could put a comment into the crud file here of what the listPermTypes mean.
				strb.Append(rn+t3+"Db.NonQ(command);");
				strb.Append(rn+t2+"}");
			}
			#endregion ClearFkey(long)
			#region ClearFkey(List<long>)
			if(!listAuditTrailPerms.IsNullOrEmpty()) {  //If there are any AuditPerms set for this table
				strb.Append(rn+rn+t2+"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+"s as FKey and are related to "+typeClass.Name+".");
				strb.Append(rn+t2+"///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClass.Name+@" table type.</summary>");
				strb.Append(rn+t2+"public static void ClearFkey(List<long> "+nameListParam+") {");
				strb.Append(rn+t3+"if("+nameListParam+"==null || "+nameListParam+".FindAll(x => x != 0).Count==0) {");
				strb.Append(rn+t4+"return;");
				strb.Append(rn+t3+"}");
				List<string> listPermTypes=new List<string>();
				for(int i=0;i<listAuditTrailPerms.Count;i++) {
					listPermTypes.Add(((int)listAuditTrailPerms[i]).ToString());
				}
				strb.Append(rn+t3+"string command=\"UPDATE securitylog SET FKey=0 WHERE FKey IN(\"+String.Join(\",\","+nameListParam
					+".FindAll(x => x != 0))+\") AND PermType IN ("+String.Join(",",listPermTypes)+")\";");
				//If we wanted to make this more readable we could put a comment into the crud file here of what the listPermTypes mean.
				strb.Append(rn+t3+"Db.NonQ(command);");
				strb.Append(rn+t2+"}");
			}
			#endregion ClearFkey(List<long>)
			//IsEqual is currently unfinished, but is here so that we can enhance it later to truly compare two objects. 
			//This will check all DB columns and all Non-DB columns for equality and return a boolean. 
			//The problem with implementing this at this time (3/4/2014) is that we don't have time to implement validating lists of objects.
			#region IsEqual
			//IsEqual-------------------------------------------------------------------------------
			//if(!isMobile) {
			//	if(typeClass.Name=="RxPat" || typeClass.Name=="Appointment" || typeClass.Name=="Claim" || typeClass.Name=="LetterMerge") {
			//		List<FieldInfo> allFields=CrudGenHelper.GetAllFieldsExceptPriKey(fields,priKey);
			//		strb.Append(rn+rn+t2+"///<summary>Checks two "+typeClass.Name+" objects for equality. Return true if equal and false if any variables are different. This includes special columns not included in the DB.</summary>");
			//		strb.Append(rn+t2+"public static bool IsEqual("+typeClass.Name+" "+obj+","+typeClass.Name+" "+oldObj+") {");
			//		for(int f=0;f<allFields.Count;f++) {
			//			if(CrudGenHelper.IsNotDbColumn(allFields[f])) {
			//				switch(allFields[f].FieldType.Name) {
			//					case "int":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "List<int>":
			//						strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//						strb.Append(rn+t4+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t5+"return false;");
			//						strb.Append(rn+t4+"}");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "string":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "List<string>":
			//						strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//						strb.Append(rn+t4+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t5+"return false;");
			//						strb.Append(rn+t4+"}");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "long":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "List<long>":
			//						strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//						strb.Append(rn+t4+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t5+"return false;");
			//						strb.Append(rn+t4+"}");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "Color":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+".ToArgb() != "+oldObj+"."+allFields[f].Name+".ToArgb()) {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "DateTime"://This is only for date, not dateT.
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "double":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "List<double>":
			//						strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//						strb.Append(rn+t4+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t5+"return false;");
			//						strb.Append(rn+t4+"}");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "Interval":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "bool":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					case "TimeSpan":
			//						strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+".Ticks != "+oldObj+"."+allFields[f].Name+".Ticks) {");
			//						strb.Append(rn+t4+"return false;");
			//						strb.Append(rn+t3+"}");
			//						break;
			//					default:
			//						//TODO: Add a sub-section for s-classes ending in H to have an ES added to them
			//						if(allFields[f].FieldType.Name.StartsWith("List<")) {
			//							strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//							string objName=allFields[f].FieldType.Name.Substring(5);
			//							objName=objName.Remove(objName.Length-1);
			//							string obj2=objName.Substring(0,1).ToLower()+objName.Substring(1);//lowercase initial letter.  Example feeSched
			//							string oldObj2="old"+objName;//Example oldFeeSched
			//							if(allFields[f].FieldType.Name.EndsWith("y")) {
			//								strb.Append(rn+t4+"if(!"+objName.Remove(objName.Length-1)+"ies.IsEqual("+objName+" "+obj2+","+objName+" "+oldObj2+") {");
			//								strb.Append(rn+t5+"return false;");
			//								strb.Append(rn+t4+"}");
			//							}
			//							else {
			//								strb.Append(rn+t4+"if(!"+objName+"s.IsEqual("+objName+" "+obj2+","+objName+" "+oldObj2+") {");
			//								strb.Append(rn+t5+"return false;");
			//								strb.Append(rn+t4+"}");
			//							}
			//							strb.Append(rn+t3+"}");
			//						}
			//						else if(allFields[f].FieldType.Name.EndsWith("[]")) {
			//							strb.Append(rn+t3+"for(int i=0;i<"+obj+"."+allFields[f].Name+".Count;i++) {");
			//							string objName=allFields[f].FieldType.Name.Remove(allFields[f].FieldType.Name.Length-2);
			//							string obj2=objName.Substring(0,1).ToLower()+objName.Substring(1);//lowercase initial letter.  Example feeSched
			//							string oldObj2="old"+objName;//Example oldFeeSched
			//							if(allFields[f].FieldType.Name.EndsWith("y")) {
			//								strb.Append(rn+t4+"if(!"+objName.Remove(objName.Length-1)+"ies.IsEqual("+objName+" "+obj2+","+objName+" "+oldObj2+") {");
			//								strb.Append(rn+t5+"return false;");
			//								strb.Append(rn+t4+"}");
			//							}
			//							else {
			//								strb.Append(rn+t4+"if(!"+objName+"s.IsEqual("+objName+" "+obj2+","+objName+" "+oldObj2+") {");
			//								strb.Append(rn+t5+"return false;");
			//								strb.Append(rn+t4+"}");
			//							}
			//							strb.Append(rn+t3+"}");
			//						}
			//						else {
			//							string obj2=allFields[f].FieldType.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);//lowercase initial letter.  Example feeSched
			//							string oldObj2="old"+allFields[f].FieldType.Name;//Example oldFeeSched
			//							if(allFields[f].FieldType.Name.EndsWith("y")) {
			//								strb.Append(rn+t3+"if(!"+allFields[f].FieldType.Name.Remove(allFields[f].FieldType.Name.Length-1)+"ies.IsEqual("+allFields[f].FieldType.Name+" "+obj2+","+allFields[f].FieldType.Name+oldObj2+") {");
			//								strb.Append(rn+t4+"return false;");
			//								strb.Append(rn+t3+"}");
			//							}
			//							else {
			//								strb.Append(rn+t3+"if(!"+allFields[f].FieldType.Name+"s.IsEqual("+allFields[f].FieldType.Name+" "+obj2+","+allFields[f].FieldType.Name+oldObj2+") {");
			//								strb.Append(rn+t4+"return false;");
			//								strb.Append(rn+t3+"}");
			//							}
			//						}
			//						break;
			//				}
			//			}
			//			//TODO: Check all special crud column types and hanlde their equals in their own way
			//			strb.Append(rn+t3+"if("+obj+"."+allFields[f].Name+" != "+oldObj+"."+allFields[f].Name+") {");
			//			strb.Append(rn+t4+"return false;");
			//			strb.Append(rn+t3+"}");
			//		}
			//		strb.Append(rn+t3+"return true;");
			//		strb.Append(rn+t2+"}");
			//	}
			//}
			#endregion IsEqual
			//Footer
			strb.Append(rn);
			strb.Append(@"
	}
}");
		}

		private static string RowToObj(List<FieldInfo> fieldsInDb,Type typeClass,string obj=null) {
			string objStr=obj??"";
			if(!string.IsNullOrEmpty(obj)) {
				objStr+=".";
			}
			//get the longest fieldname for alignment purposes
			int longestField=fieldsInDb.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
			StringBuilder strb=new StringBuilder();
			CrudSpecialColType specialType;
			for(int f = 0;f<fieldsInDb.Count;f++) {
				//Fields are not guaranteed to be in any particular order.
				specialType=CrudGenHelper.GetSpecialType(fieldsInDb[f]);
				if(specialType.HasFlag(CrudSpecialColType.EnumAsString) && !CrudGenHelper.UsesDataReader(typeClass)) {
					string fieldLower=fieldsInDb[f].Name.Substring(0,1).ToLower()+fieldsInDb[f].Name.Substring(1);//lowercase initial letter.  Example clockStatus
					strb.Append(rn+t4+"string "+fieldLower+"=row[\""+fieldsInDb[f].Name+"\"].ToString();");
					strb.Append(rn+t4+"if("+fieldLower+"==\"\") {");
					strb.Append(rn+t5+objStr+fieldsInDb[f].Name.PadRight(longestField-2,' ')+"="
						+"("+fieldsInDb[f].FieldType.ToString()+")0;");
					strb.Append(rn+t4+"}");
					strb.Append(rn+t4+"else try{");
					strb.Append(rn+t5+objStr+fieldsInDb[f].Name.PadRight(longestField-2,' ')+"=("+fieldsInDb[f].FieldType.ToString()+")Enum.Parse(typeof("
						+fieldsInDb[f].FieldType.ToString()+"),"+fieldLower+");");
					strb.Append(rn+t4+"}");
					strb.Append(rn+t4+"catch{");
					strb.Append(rn+t5+objStr+fieldsInDb[f].Name.PadRight(longestField-2,' ')+"=("+fieldsInDb[f].FieldType.ToString()+")0;");
					strb.Append(rn+t4+"}");
					continue;
				}
				strb.Append(rn+t4+objStr+fieldsInDb[f].Name.PadRight(longestField,' ')+"= ");
				if(specialType.HasFlag(CrudSpecialColType.DateT)
					|| specialType.HasFlag(CrudSpecialColType.TimeStamp)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
				{
					//specialTypes.DateEntry and DateEntryEditable is handled fine by the normal DateTime (date) below.
					strb.Append("PIn.DateT (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					strb.Append("PIn.TSpan (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append("TimeSpan.FromTicks(PIn.Long(");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {//already made sure to only use this pattern if UsesDataReader=true
					strb.Append("PIn.Enum<"+fieldsInDb[f].FieldType.ToString()+">(");//.ToString() instead of .Name to get fully qualified name
				}
				//no special treatment for specialType clob
				else if(fieldsInDb[f].FieldType.IsEnum) {
					strb.Append("("+fieldsInDb[f].FieldType.ToString()+")PIn.Int(");//.ToString() instead of .Name to get fully qualified name
				}
				else switch(fieldsInDb[f].FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+fieldsInDb[f].FieldType.Name);
						case "Bitmap":
							strb.Append("PIn.Bitmap(");
							break;
						case "Boolean":
							strb.Append("PIn.Bool  (");
							break;
						case "Byte":
							strb.Append("PIn.Byte  (");
							break;
						case "Color":
							strb.Append("Color.FromArgb(PIn.Int(");
							break;
						case "DateTime"://This ONLY handles date, not dateT which is a special type.
							strb.Append("PIn.Date  (");
							break;
						case "Double":
							strb.Append("PIn.Double(");
							break;
						case "Interval":
							strb.Append("new Interval(PIn.Int(");
							break;
						case "Int64":
							strb.Append("PIn.Long  (");
							break;
						case "Int32":
							strb.Append("PIn.Int   (");
							break;
						case "Single":
							strb.Append("PIn.Float (");
							break;
						case "String":
							strb.Append("PIn.String(");
							break;
						case "TimeSpan":
							strb.Append("PIn.Time(");
							break;
				}
				strb.Append("row[\""+fieldsInDb[f].Name+"\"].ToString()"+(specialType.HasFlag(CrudSpecialColType.EnumAsString)?",true":"")+")");
				if(fieldsInDb[f].FieldType.Name=="Color" || fieldsInDb[f].FieldType.Name=="Interval" || specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					strb.Append(")");
				}
				strb.Append(string.IsNullOrEmpty(objStr)?",":";");
			}
			return strb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		private void butSnippet_Click(object sender,EventArgs e) {
			if(listClass.SelectedIndex==-1) {
				MessageBox.Show("Please select a class.");
				return;
			}
			//if(comboType.SelectedIndex==-1) {
			//	MessageBox.Show("Please select a type.");
			//	return;
			//}
			Type typeClass=tableTypesAll[listClass.SelectedIndex];
			SnippetType snipType=(SnippetType)comboType.SelectedIndex;
			bool isMobile=CrudGenHelper.IsMobile(typeClass);
			string snippet=CrudGenDataInterface.GetSnippet(typeClass,snipType,isMobile);
			textSnippet.Text=snippet;
			Clipboard.SetText(snippet);
		}

		private void butAddPrefs_Click(object sender,EventArgs e) {
			string server="localhost"+(string.IsNullOrEmpty(textPortNum.Text) ? "" : $":{textPortNum.Text}");
			CrudGenHelper.ConnectToDatabase(textDb.Text,server:server);
			CrudGenHelper.AddMissingPreferences(convertDbFile);
		}

		private void Form1_FormClosing(object sender,FormClosingEventArgs e) {
			CrudGenHelper.AddToOpenDentBusiness(ListFilesToAddToODBiz);
		}
	}
}
