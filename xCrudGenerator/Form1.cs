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
		public static string convertDbFile=@"..\..\..\OpenDentBusiness\Misc\ConvertDatabases8.cs";
		private const string rn="\r\n";
		private const string t="\t";
		private const string t2="\t\t";
		private const string t3="\t\t\t";
		private const string t4="\t\t\t\t";
		private const string t5="\t\t\t\t\t";
		private const string t6="\t\t\t\t\t\t";
		private const string t7="\t\t\t\t\t\t\t";
		private List<Type> _listTypes;
		public static List<string> ListFilesToAddToODBiz=new List<string>();

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender,EventArgs e) {
			if(Environment.MachineName.ToLower()!="jordans") {
				butPd.Visible=false;
			}
			if(!Directory.Exists(crudDir)) {
				MessageBox.Show(crudDir+" is an invalid path.");
				Application.Exit();
			}
			_listTypes=new List<Type>();
			Type typeTableBase=typeof(TableBase);
			Assembly assembly=Assembly.GetAssembly(typeTableBase);
			foreach(Type typeClass in assembly.GetTypes()) {
				if(typeClass.Name.StartsWith("WebForms_")) {
					continue;
				}
				if(typeClass.IsSubclassOf(typeTableBase) && !typeClass.IsAbstract) {
					_listTypes.Add(typeClass);
				}
			}
			_listTypes.Sort(CompareTypesByName);
			for(int i=0;i<_listTypes.Count;i++) {
				listClass.Items.Add(_listTypes[i].Name);
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
				try {
					CrudGenHelper.ConnectToDatabase(textDb.Text,server:server,password:textPassword.Text);
				}
				catch (Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return;
				}
				for(int i=0;i<_listTypes.Count;i++) {
					string crudDirectory=crudDir;
					className=_listTypes[i].Name+"Crud";
					strb=new StringBuilder();
					if(checkAddToConvertScript.Checked) {
						CrudGenHelper.ValidateTypes(_listTypes[i],textDb.Text);
					}
					WriteAll(strb,className,_listTypes[i],false);
					string crudDirOverride=CrudGenHelper.GetCrudLocationOverride(_listTypes[i]);
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
					CrudQueries.Write(convertDbFile,_listTypes[i],textDb.Text,false,checkRunQueries.Checked,checkAddToConvertScript.Checked,results);
					CrudGenDataInterface.Create(convertDbFile,_listTypes[i],textDb.Text,false);
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
		private void WriteAll(StringBuilder stringBuilder,string className,Type typeClass,bool isMobile) {
			#region initialize variables
			FieldInfo[] fieldInfoArray=typeClass.GetFields();//We can't assume they are in the correct order.
			FieldInfo fieldInfoPriKey=null;
			FieldInfo priKey1=null;
			FieldInfo priKey2=null;
			if(isMobile) {
				priKey1=CrudGenHelper.GetPriKeyMobile1(fieldInfoArray,typeClass.Name);
				priKey2=CrudGenHelper.GetPriKeyMobile2(fieldInfoArray,typeClass.Name);
			}
			else {
				fieldInfoPriKey=CrudGenHelper.GetPriKey(fieldInfoArray,typeClass.Name);
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
				priKeyParam=fieldInfoPriKey.Name.Substring(0,1).ToLower()+fieldInfoPriKey.Name.Substring(1);//lowercase initial letter.  Example patNum
			}
			List<EnumPermType> listAuditTrailPerms=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeClass));
			string obj=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);//lowercase initial letter.  Example feeSched
			string oldObj="old"+typeClass.Name;//used in the second update overload.  Example oldFeeSched
			string customNamespace=CrudGenHelper.GetNamespaceOverride(typeClass);
			string nameListParam="list"+priKeyParam.Substring(0,1).ToUpper()+priKeyParam.Substring(1)+"s";//prepend 'list', upper case priKey first char, append 's'. Example listPatNums
			#endregion initialize variables
			#region class header
			stringBuilder.Append("//This file is automatically generated."+rn
				+"//Do not attempt to make changes to this file because the changes will be erased and overwritten."+rn
				+@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;"+rn);
			if(CrudGenHelper.HasBatchWriteMethods(typeClass)) {
				stringBuilder.Append("using System.Text;"+rn);
			}
			if(className.StartsWith("EhrLab")) {
				stringBuilder.Append("using EhrLaboratories;"+rn);
			}
			if(isMobile) {
				stringBuilder.Append(rn+"namespace OpenDentBusiness.Mobile.Crud{");
			}
			else if(!string.IsNullOrEmpty(customNamespace)) {
				stringBuilder.Append(rn+"namespace "+customNamespace+"{");
			}
			else { 
				stringBuilder.Append(rn+"namespace OpenDentBusiness.Crud{");
			}
			stringBuilder.Append(rn+t+"public class "+className+" {");
			#endregion class header
			#region SelectOne
			//SelectOne------------------------------------------------------------------------------------------
			if(isMobile) {
				stringBuilder.Append(rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using primaryKey1(CustomerNum) and primaryKey2.  Returns null if not found.</summary>");
				stringBuilder.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(long "+priKeyParam1+",long "+priKeyParam2+") {");
				stringBuilder.Append(rn+t3+"string command=\"SELECT * FROM "+tablename+" \"");
				stringBuilder.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+priKeyParam1+")+\" AND "+priKey2.Name+" = \"+POut.Long("+priKeyParam2+");");
			}
			else {
				stringBuilder.Append(rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using the primary key.  Returns null if not found.</summary>");
				stringBuilder.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(long "+priKeyParam+") {");
				stringBuilder.Append(rn+t3+"string command=\"SELECT * FROM "+tablename+" \"");
				stringBuilder.Append(rn+t4+"+\"WHERE "+fieldInfoPriKey.Name+" = \"+POut.Long("+priKeyParam+");");
			}
			stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			stringBuilder.Append(rn+t3+"if(list.Count==0) {");
			stringBuilder.Append(rn+t4+"return null;");
			stringBuilder.Append(rn+t3+"}");
			stringBuilder.Append(rn+t3+"return list[0];");
			stringBuilder.Append(rn+t2+"}");
			#endregion SelectOne
			#region SelectOne(command)
			//SelectOne(string command)--------------------------------------------------------------------------
			stringBuilder.Append(rn+rn+t2+"///<summary>Gets one "+typeClass.Name+" object from the database using a query.</summary>");
			stringBuilder.Append(rn+t2+"public static "+typeClass.Name+" SelectOne(string command) {");
			stringBuilder.Append(rn+t3+@"if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException(""Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n""+command);
			}");
			stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			stringBuilder.Append(rn+t3+"if(list.Count==0) {");
			stringBuilder.Append(rn+t4+"return null;");
			stringBuilder.Append(rn+t3+"}");
			stringBuilder.Append(rn+t3+"return list[0];");
			stringBuilder.Append(rn+t2+"}");
			#endregion SelectOne(command)
			#region SelectMany
			//SelectMany-----------------------------------------------------------------------------------------
			stringBuilder.Append(rn+rn+t2+"///<summary>Gets a list of "+typeClass.Name+" objects from the database using a query.</summary>");
			stringBuilder.Append(rn+t2+"public static List<"+typeClass.Name+"> SelectMany(string command) {");
			stringBuilder.Append(rn+t3+@"if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException(""Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n""+command);
			}");
			stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> list=TableToList(Db.GetTable(command));");
			stringBuilder.Append(rn+t3+"return list;");
			stringBuilder.Append(rn+t2+"}");
			#endregion SelectMany
			#region TableToList
			//TableToList----------------------------------------------------------------------------------------
			stringBuilder.Append(rn+rn+t2+"///<summary>Converts a DataTable to a list of objects.</summary>");
			stringBuilder.Append(rn+t2+"public static List<"+typeClass.Name+"> TableToList(DataTable table) {");
			stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> retVal=new List<"+typeClass.Name+">();");
			stringBuilder.Append(rn+t3+typeClass.Name+" "+obj+";");
			stringBuilder.Append(rn+t3+"foreach(DataRow row in table.Rows) {");
			stringBuilder.Append(rn+t4+obj+"=new "+typeClass.Name+"();");
			List<FieldInfo> listFieldInfos=CrudGenHelper.GetFieldsExceptNotDb(fieldInfoArray);
			stringBuilder.Append(RowToObj(listFieldInfos,typeClass,obj));
			stringBuilder.Append(rn+t4+"retVal.Add("+obj+");");
			stringBuilder.Append(rn+t3+"}");
			stringBuilder.Append(rn+t3+"return retVal;");
			stringBuilder.Append(rn+t2+"}");
			#endregion TableToList
			#region RowToObj
			//RowToObj------------------------------------------------------------------------------------------
			if(CrudGenHelper.UsesDataReader(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Converts an IDataRecord to a "+typeClass.Name+" object.</summary>");
				stringBuilder.Append(rn+t2+"public static "+typeClass.Name+" RowToObj(IDataRecord row) {");
				stringBuilder.Append(rn+t3+"return new "+typeClass.Name+" {");
				stringBuilder.Append(RowToObj(listFieldInfos,typeClass));
				stringBuilder.Append(rn+t3+"};");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion RowToObj
			#region ListToTable
			//ListToTable----------------------------------------------------------------------------------------
			stringBuilder.Append(rn+rn+t2+"///<summary>Converts a list of "+typeClass.Name+" into a DataTable.</summary>");
			stringBuilder.Append(rn+t2+"public static DataTable ListToTable(List<"+typeClass.Name+"> list"+typeClass.Name+"s,string tableName=\"\") {");
			stringBuilder.Append(rn+t3+"if(string.IsNullOrEmpty(tableName)) {");
			stringBuilder.Append(rn+t4+"tableName=\""+typeClass.Name+"\";");
			stringBuilder.Append(rn+t3+"}");
			stringBuilder.Append(rn+t3+"DataTable table=new DataTable(tableName);");
			//Now add columns.
			foreach(FieldInfo field in listFieldInfos) {
				stringBuilder.Append(rn+t3+"table.Columns.Add(\""+field.Name+"\");");
			}
			string classLowerCase=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);
			stringBuilder.Append(rn+t3+"foreach("+typeClass.Name+" "+classLowerCase+" in list"+typeClass.Name+"s) {");
			stringBuilder.Append(rn+t4+"table.Rows.Add(new object[] {");
			CrudSpecialColType specialType;
			foreach(FieldInfo field in listFieldInfos) {
				stringBuilder.Append(rn+t5);
				//Fields are not guaranteed to be in any particular order.
				specialType=CrudGenHelper.GetSpecialType(field);
				if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					stringBuilder.Append("POut.Time  (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append("POut.Long (");
				}
				else if(field.FieldType.IsEnum) {
					stringBuilder.Append("POut.Int   ((int)");
				}
				else switch(field.FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+field.FieldType.Name);
						case "Bitmap":
							stringBuilder.Append("POut.Bitmap(");
							break;
						case "Boolean":
							stringBuilder.Append("POut.Bool  (");
							break;
						case "Byte":
							stringBuilder.Append("POut.Byte  (");
							break;
						case "DateTime":
							stringBuilder.Append("POut.DateT (");
							break;
						case "Double":
							stringBuilder.Append("POut.Double(");
							break;
						case "Int64":
							stringBuilder.Append("POut.Long  (");
							break;
						case "Color":
						case "Int32":
						case "Interval":
							stringBuilder.Append("POut.Int   (");
							break;
						case "Single":
							stringBuilder.Append("POut.Float (");
							break;
						case "String":
							stringBuilder.Append("            ");
							break;
						case "TimeSpan":
							stringBuilder.Append("POut.Time  (");
							break;
					}
				if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append(classLowerCase+"."+field.Name+".Ticks),");
				}
				else if(specialType.HasFlag(CrudSpecialColType.Double))	{	//Add cases / method calls for # rounding places
					int decimalPlaces=(int)field.CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
					stringBuilder.Append(classLowerCase+"."+field.Name+", decimalPlaces:"+decimalPlaces+"),");
				}
				else {
					stringBuilder.Append(classLowerCase+"."+field.Name
						+(field.FieldType.Name=="Color" ? ".ToArgb()" : "")
						+(field.FieldType.Name=="Interval" ? ".ToInt()" : "")
						+(field.FieldType.Name=="DateTime" ? ",false" : "")
						+(field.FieldType.Name=="TimeSpan" ? ",false" : "")
						+(field.FieldType.Name=="String" ? "" : ")")
						+",");
				}
			}
			stringBuilder.Append(rn+t4+"});");
			stringBuilder.Append(rn+t3+"}");
			stringBuilder.Append(rn+t3+"return table;");
			stringBuilder.Append(rn+t2+"}");
			#endregion ListToTable
			#region Insert
			//Insert---------------------------------------------------------------------------------------------
			List<FieldInfo> fieldInfosExceptPri=null; 
			if(isMobile) {
				fieldInfosExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fieldInfoArray,priKey2);
				//first override not used for mobile.
				//second override
				stringBuilder.Append(rn+rn+t2+"///<summary>Usually set useExistingPK=true.  Inserts one "+typeClass.Name+" into the database.</summary>");
				stringBuilder.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				stringBuilder.Append(rn+t3+"if(!useExistingPK) {");// && PrefC.RandomKeys) {");PrefC.RandomKeys is always true for mobile, since autoincr is just not possible.
//Todo: ReplicationServers.GetKey() needs to work for mobile.  Not needed until we start inserting records from mobile.
				stringBuilder.Append(rn+t4+obj+"."+priKey2.Name+"=ReplicationServers.GetKey(\""+tablename+"\",\""+priKey2.Name+"\");");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				//strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");//PrefC.RandomKeys is always true
				stringBuilder.Append(rn+t3+"command+=\""+priKey2.Name+",\";");
				//strb.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"command+=\"");
				for(int f=0;f<fieldInfosExceptPri.Count;f++) {//all fields except PK2.
					if(CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]).HasFlag(CrudSpecialColType.TimeStamp)) {
						continue;
					}
					if(f>0) {
						stringBuilder.Append(",");
					}
					stringBuilder.Append(fieldInfosExceptPri[f].Name);
				}
				stringBuilder.Append(") VALUES(\";");
				//strb.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");//PrefC.RandomKeys is always true
				stringBuilder.Append(rn+t3+"command+=POut.Long("+obj+"."+priKey2.Name+")+\",\";");
				//strb.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"command+=");
			}
			else {
				fieldInfosExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fieldInfoArray,fieldInfoPriKey);
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Returns the new priKey.</summary>");
				stringBuilder.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+") {");
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
				stringBuilder.Append(rn+t3+"return Insert("+obj+",false);");
				stringBuilder.Append(rn+t2+"}");
				//second override
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Provides option to use the existing priKey.</summary>");
				stringBuilder.Append(rn+t2+"public static long Insert("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(!useExistingPK && PrefC.RandomKeys) {");
					stringBuilder.Append(rn+t4+obj+"."+fieldInfoPriKey.Name+"=ReplicationServers.GetKey(\""+tablename+"\",\""+fieldInfoPriKey.Name+"\");");
					stringBuilder.Append(rn+t3+"}");
				}
				stringBuilder.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					stringBuilder.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				stringBuilder.Append(rn+t4+"command+=\""+fieldInfoPriKey.Name+",\";");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"command+=\"");
				for(int f=0;f<fieldInfosExceptPri.Count;f++) {
					specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						continue;
					}
					if(f>0) {
						stringBuilder.Append(",");
					}
					stringBuilder.Append(fieldInfosExceptPri[f].Name);
				}
				stringBuilder.Append(") VALUES(\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					stringBuilder.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				stringBuilder.Append(rn+t4+"command+=POut.Long("+obj+"."+fieldInfoPriKey.Name+")+\",\";");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"command+=");
			}
			//a quick and dirty temporary list that just helps keep track of which columns take parameters
			List<OdSqlParameter> paramList=new List<OdSqlParameter>();
			for(int f=0;f<fieldInfosExceptPri.Count;f++) {
				stringBuilder.Append(rn+t4);
				specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
				if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					stringBuilder.Append("//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
					continue;
				}
				if(f==0) {
					stringBuilder.Append(" ");
				}
				else {
					stringBuilder.Append("+");
				}
				if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
					stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) 
				{
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f])) {
						//not a hist table, or is a hist table and field is not inherited from other table
						stringBuilder.Append("    DbHelper.Now()+\"");
					}
					//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append("    POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
					stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
					stringBuilder.Append("\"'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+".ToString())+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					stringBuilder.Append("\"'\"+POut.TSpan ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append("\"'\"+POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+".Ticks)+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
					stringBuilder.Append("    DbHelper.ParamChar+\"param"+fieldInfosExceptPri[f].Name);
					paramList.Add(new OdSqlParameter(fieldInfosExceptPri[f].Name,OdDbType.Text,specialType));
				}
				else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
					stringBuilder.Append("    POut.Int   ((int)"+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else switch(fieldInfosExceptPri[f].FieldType.Name) {
					default:
						throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
					case "Bitmap":
						stringBuilder.Append("    POut.Bitmap("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Boolean":
						stringBuilder.Append("    POut.Bool  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Byte":
						stringBuilder.Append("    POut.Byte  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Color":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToArgb())+\"");
						break;
					case "DateTime"://This is only for date, not dateT.
						stringBuilder.Append("    POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Double":
						if(specialType.HasFlag(CrudSpecialColType.Double)) {
							int decimalPlaces=(int)fieldInfosExceptPri[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
							stringBuilder.Append("		 POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+", decimalPlaces:"+decimalPlaces+")+\"");
						}
						else {
							stringBuilder.Append("		 POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						}
						break;
					case "Interval":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToInt())+\"");
						break;
					case "Int64":
						stringBuilder.Append("    POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Int32":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Single":
						stringBuilder.Append("    POut.Float ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "String":
						stringBuilder.Append("\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
							+obj+"."+fieldInfosExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
						break;
					case "TimeSpan":
						stringBuilder.Append("    POut.Time  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
				}
				if(f==fieldInfosExceptPri.Count-2
					&& CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
					&& !CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f+1])) 
				{
					//in case the last field is a timestamp that is not a hist table column inherited from another table
					stringBuilder.Append(")\";");
				}
				else if(f<fieldInfosExceptPri.Count-1) {
					stringBuilder.Append(",\"");
				}
				else {
					stringBuilder.Append(")\";");
				}
			}
			for(int i=0;i<paramList.Count;i++) {
				stringBuilder.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
				stringBuilder.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
				stringBuilder.Append(rn+t3+"}");
				//example: OdSqlParameter paramNote=new OdSqlParameter("paramNote",
				//           OdDbType.Text,procNote.Note);
				stringBuilder.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
					+"OdDbType.Text,");
				if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
					stringBuilder.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
				}
				else {
					stringBuilder.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
				}
			}
			string paramsString="";//example: ,paramNote,paramAltNote
			for(int i=0;i<paramList.Count;i++) {
				paramsString+=",param"+paramList[i].ParameterName;
			}	
			if(isMobile) {
				stringBuilder.Append(rn+t3+"Db.NonQ(command"+paramsString+");//There is no autoincrement in the mobile server.");
				stringBuilder.Append(rn+t3+"return "+obj+"."+priKey2.Name+";");
				stringBuilder.Append(rn+t2+"}");
			}
			else {
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					stringBuilder.Append(rn+t3+"if(useExistingPK || PrefC.RandomKeys) {");
				}
				stringBuilder.Append(rn+t4+"Db.NonQ(command"+paramsString+");");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"else {");
				stringBuilder.Append(rn+t4+obj+"."+fieldInfoPriKey.Name+"=Db.NonQ(command,true,\""+fieldInfoPriKey.Name+"\",\""+obj+"\""+paramsString+");");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"return "+obj+"."+fieldInfoPriKey.Name+";");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Insert
			#region InsertMany
			if(CrudGenHelper.HasBatchWriteMethods(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts many "+typeClass.Name+"s into the database.</summary>");
				stringBuilder.Append(rn+t2+"public static void InsertMany(List<"+typeClass.Name+"> list"+typeClass.Name+"s) {");
				stringBuilder.Append(rn+t3+"InsertMany(list"+typeClass.Name+"s,false);");
				stringBuilder.Append(rn+t2+"}");
				//second override
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts many "+typeClass.Name+"s into the database.  Provides option to use the existing priKey.</summary>");
				stringBuilder.Append(rn+t2+"public static void InsertMany(List<"+typeClass.Name+"> list"+typeClass.Name+"s,bool useExistingPK) {");
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
					stringBuilder.Append(rn+t3+"if(!useExistingPK && PrefC.RandomKeys) {");
						stringBuilder.Append(rn+t4+"foreach("+typeClass.Name+" "+obj+" in list"+typeClass.Name+"s) {");
							stringBuilder.Append(rn+t5+"Insert("+obj+");");
						stringBuilder.Append(rn+t4+"}");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"else {");
				}
				#region MySQL and not using Random Primary Keys.
						stringBuilder.Append(rn+t4+"StringBuilder sbCommands=null;");
						stringBuilder.Append(rn+t4+"int index=0;");
						stringBuilder.Append(rn+t4+"int countRows=0;");
						stringBuilder.Append(rn+t4+"while(index < list"+typeClass.Name+"s.Count) {");
							stringBuilder.Append(rn+t5+typeClass.Name+" "+obj+"=list"+typeClass.Name+"s[index];");
							stringBuilder.Append(rn+t5+"StringBuilder sbRow=new StringBuilder(\"(\");");
							stringBuilder.Append(rn+t5+"bool hasComma=false;");
							stringBuilder.Append(rn+t5+"if(sbCommands==null) {");
								stringBuilder.Append(rn+t6+"sbCommands=new StringBuilder();");
								stringBuilder.Append(rn+t6+"sbCommands.Append(\"INSERT INTO "+tablename+" (\");");
								stringBuilder.Append(rn+t6+"if(useExistingPK) {");
									stringBuilder.Append(rn+t7+"sbCommands.Append(\""+fieldInfoPriKey.Name+",\");");
								stringBuilder.Append(rn+t6+"}");
								stringBuilder.Append(rn+t6+"sbCommands.Append(\"");
								for(int f=0;f<fieldInfosExceptPri.Count;f++) {
									specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
									if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
										continue;
									}
									if(f>0) {
										stringBuilder.Append(",");
									}
									stringBuilder.Append(fieldInfosExceptPri[f].Name);
								}
								stringBuilder.Append(") VALUES \");");
								stringBuilder.Append(rn+t6+"countRows=0;");
							stringBuilder.Append(rn+t5+"}");//end if
							stringBuilder.Append(rn+t5+"else {");//This is not the first row 
								stringBuilder.Append(rn+t6+"hasComma=true;");
							stringBuilder.Append(rn+t5+"}");//end else
							stringBuilder.Append(rn+t5+"if(useExistingPK) {");
								stringBuilder.Append(rn+t6+"sbRow.Append(POut.Long("+obj+"."+fieldInfoPriKey.Name+")); sbRow.Append(\",\");");
							stringBuilder.Append(rn+t5+"}");
							for(int f=0;f<fieldInfosExceptPri.Count;f++) {
								stringBuilder.Append(rn+t5);
								specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
								if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
									stringBuilder.Append("//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
									continue;
								}
								if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
									//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
									stringBuilder.Append("sbRow.Append(POut.DateT("+obj+"."+fieldInfosExceptPri[f].Name+"));");
								}
								else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
									|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
									|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
									|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
								{
									if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f])) {
										//not a hist table, or is a hist table and field is not inherited from other table
										stringBuilder.Append("sbRow.Append(DbHelper.Now());");
									}
									//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
									else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
										stringBuilder.Append("sbRow.Append(POut.Date("+obj+"."+fieldInfosExceptPri[f].Name+"));");
									}
									else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
										stringBuilder.Append("sbRow.Append(POut.DateT("+obj+"."+fieldInfosExceptPri[f].Name+"));");
									}
								}
								else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
									stringBuilder.Append("sbRow.Append(POut.DateT("+obj+"."+fieldInfosExceptPri[f].Name+"));");
								}
								else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
									stringBuilder.Append("sbRow.Append(\"'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+".ToString())+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
									stringBuilder.Append("sbRow.Append(\"'\"+POut.TSpan ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
									stringBuilder.Append("sbRow.Append(\"'\"+POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+".Ticks)+\"'\");");
								}
								else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
									stringBuilder.Append("sbRow.Append(\"'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'\");");
								}
								else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
									stringBuilder.Append("sbRow.Append(POut.Int((int)"+obj+"."+fieldInfosExceptPri[f].Name+"));");
								}
								else switch(fieldInfosExceptPri[f].FieldType.Name) {
									default:
										throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
									case "Bitmap":
										stringBuilder.Append("sbRow.Append(POut.Bitmap("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Boolean":
										stringBuilder.Append("sbRow.Append(POut.Bool("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Byte":
										stringBuilder.Append("sbRow.Append(POut.Byte("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Color":
										stringBuilder.Append("sbRow.Append(POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+".ToArgb()));");
										break;
									case "DateTime"://This is only for date, not dateT.
										stringBuilder.Append("sbRow.Append(POut.Date("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Double":
										if(specialType.HasFlag(CrudSpecialColType.Double)) {
											int decimalPlaces=(int)fieldInfosExceptPri[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
											stringBuilder.Append("sbRow.Append(POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+", decimalPlaces:"+decimalPlaces+"));");
										}
										else {
											stringBuilder.Append("sbRow.Append(POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										}
										break;
									case "Interval":
										stringBuilder.Append("sbRow.Append(POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+".ToInt()));");
										break;
									case "Int64":
										stringBuilder.Append("sbRow.Append(POut.Long("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Int32":
										stringBuilder.Append("sbRow.Append(POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "Single":
										stringBuilder.Append("sbRow.Append(POut.Float("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
									case "String":
										stringBuilder.Append("sbRow.Append(\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
											+obj+"."+fieldInfosExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'\");");
										break;
									case "TimeSpan":
										stringBuilder.Append("sbRow.Append(POut.Time("+obj+"."+fieldInfosExceptPri[f].Name+"));");
										break;
								}
								if(f==fieldInfosExceptPri.Count-2
									&& CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
									&& !CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f+1])) 
								{
									//in case the last field is a timestamp that is not a hist table column inherited from another table
									stringBuilder.Append(" sbRow.Append(\")\");");
								}
								else if(f<fieldInfosExceptPri.Count-1) {
									stringBuilder.Append(" sbRow.Append(\",\");");
								}
								else {
									stringBuilder.Append(" sbRow.Append(\")\");");
								}
							}
							stringBuilder.Append(rn+t5+"if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {");//plus 1 for comma
								stringBuilder.Append(rn+t6+"Db.NonQ(sbCommands.ToString());");
								//There has not been a need yet for returning the new primary keys after a batch insert.
								//We need to do some testing before we can implement this feature in the future.
								//http://stackoverflow.com/questions/7333524/how-can-i-insert-many-rows-into-a-mysql-table-and-return-the-new-ids
								//We can possbily call LAST_INSERT_ID() to get the ID of the last row, and then loop backwards decrementing the key values,
								//assuming the values would be sequential even if another user inserts a new row while our batch insert is running.
								stringBuilder.Append(rn+t6+"sbCommands=null;");
							stringBuilder.Append(rn+t5+"}");//end if
							stringBuilder.Append(rn+t5+"else {");
								stringBuilder.Append(rn+t6+"if(hasComma) {");
									stringBuilder.Append(rn+t7+"sbCommands.Append(\",\");");
								stringBuilder.Append(rn+t6+"}");
								stringBuilder.Append(rn+t6+"sbCommands.Append(sbRow.ToString());");
								stringBuilder.Append(rn+t6+"countRows++;");
								stringBuilder.Append(rn+t6+"if(index==list"+typeClass.Name+"s.Count-1) {");
									stringBuilder.Append(rn+t7+"Db.NonQ(sbCommands.ToString());");
								stringBuilder.Append(rn+t6+"}");//end else
								stringBuilder.Append(rn+t6+"index++;");
							stringBuilder.Append(rn+t5+"}");//end else
						stringBuilder.Append(rn+t4+"}");//end while
				#endregion MySQL and not using Random Primary Keys.
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {//Don't exclude PrefC check.
					stringBuilder.Append(rn+t3+"}");
				}
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion InsertMany
			#region InsertNoCache
			//InsertNoCache---------------------------------------------------------------------------------------------
			fieldInfosExceptPri=null; 
			if(isMobile) {
				//Mobile sync does not use cache already.
			}
			else {
				fieldInfosExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fieldInfoArray,fieldInfoPriKey);
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Returns the new priKey.  Doesn't use the cache.</summary>");
				stringBuilder.Append(rn+t2+"public static long InsertNoCache("+typeClass.Name+" "+obj+") {");
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
				stringBuilder.Append(rn+t3+"return InsertNoCache("+obj+",false);");
				stringBuilder.Append(rn+t2+"}");
				//second override
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts one "+typeClass.Name+" into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>");
				stringBuilder.Append(rn+t2+"public static long InsertNoCache("+typeClass.Name+" "+obj+",bool useExistingPK) {");
				if(!CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);");
				}
					stringBuilder.Append(rn+t3+"string command=\"INSERT INTO "+tablename+" (\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
						stringBuilder.Append(rn+t4+"command+=\""+fieldInfoPriKey.Name+",\";");
					stringBuilder.Append(rn+t3+"}");
				}
				else {
					stringBuilder.Append(rn+t3+"if(!useExistingPK && isRandomKeys) {");
						stringBuilder.Append(rn+t4+obj+"."+fieldInfoPriKey.Name+"=ReplicationServers.GetKeyNoCache(\""+tablename+"\",\""+fieldInfoPriKey.Name+"\");");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"if(isRandomKeys || useExistingPK) {");
						stringBuilder.Append(rn+t4+"command+=\""+fieldInfoPriKey.Name+",\";");
					stringBuilder.Append(rn+t3+"}");
				}
					stringBuilder.Append(rn+t3+"command+=\"");
					for(int f=0;f<fieldInfosExceptPri.Count;f++) {
					specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
							continue;
						}
						if(f>0) {
							stringBuilder.Append(",");
						}
						stringBuilder.Append(fieldInfosExceptPri[f].Name);
					}
					stringBuilder.Append(") VALUES(\";");
				if(CrudGenHelper.GetCrudExcludePrefC(typeClass)) {
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					stringBuilder.Append(rn+t3+"if(isRandomKeys || useExistingPK) {");
				}
						stringBuilder.Append(rn+t4+"command+=POut.Long("+obj+"."+fieldInfoPriKey.Name+")+\",\";");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"command+=");
			}
			//a quick and dirty temporary list that just helps keep track of which columns take parameters
			paramList=new List<OdSqlParameter>();
			#region FieldsExceptPri
			for(int f=0;f<fieldInfosExceptPri.Count;f++) {
				stringBuilder.Append(rn+t4);
				specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
				if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f]) && specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					stringBuilder.Append("//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
					continue;
				}
				if(f==0) {
					stringBuilder.Append(" ");
				}
				else {
					stringBuilder.Append("+");
				}
				if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
					//TimeStamp columns are skipped unless this is a hist table and the column is inherited from the other table, so retain object field value
					stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateEntryEditable)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
				{
					if(!CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f])) {
						//not a hist table, or is a hist table and field is not inherited from other table
						stringBuilder.Append("    DbHelper.Now()+\"");
					}
					//must be a hist table and field is inherited from other table, so insert Date/DateTime values from the object
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateEntry,CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append("    POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(EnumTools.HasAnyFlag(specialType,CrudSpecialColType.DateTEntry,CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
				}
				else if(specialType.HasFlag(CrudSpecialColType.DateT)) {
					stringBuilder.Append("    POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
					stringBuilder.Append("\"'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+".ToString())+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					stringBuilder.Append("\"'\"+POut.TSpan ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append("\"'\"+POut.Long("+obj+"."+fieldInfosExceptPri[f].Name+".Ticks)+\"'");
				}
				else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
					stringBuilder.Append("    DbHelper.ParamChar+\"param"+fieldInfosExceptPri[f].Name);
					paramList.Add(new OdSqlParameter(fieldInfosExceptPri[f].Name,OdDbType.Text,specialType));
				}
				else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
					stringBuilder.Append("    POut.Int   ((int)"+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
				}
				else switch(fieldInfosExceptPri[f].FieldType.Name) {
					default:
						throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
					case "Bitmap":
						stringBuilder.Append("    POut.Bitmap("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Boolean":
						stringBuilder.Append("    POut.Bool  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Byte":
						stringBuilder.Append("    POut.Byte  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Color":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToArgb())+\"");
						break;
					case "DateTime"://This is only for date, not dateT.
						stringBuilder.Append("    POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Double":
						if(specialType.HasFlag(CrudSpecialColType.Double)) {											
							int decimalPlaces=(int)fieldInfosExceptPri[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
							stringBuilder.Append("	   POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+", decimalPlaces:"+decimalPlaces+")+\""); 
						}
						else {
							stringBuilder.Append("	   POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+")+\""); 
						}
						break;
					case "Interval":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToInt())+\"");
						break;
					case "Int64":
						stringBuilder.Append("    POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Int32":
						stringBuilder.Append("    POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "Single":
						stringBuilder.Append("    POut.Float ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
					case "String":
						stringBuilder.Append("\"'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
							+obj+"."+fieldInfosExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
						break;
					case "TimeSpan":
						stringBuilder.Append("    POut.Time  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
						break;
				}
				if(f==fieldInfosExceptPri.Count-2
					&& CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f+1]).HasFlag(CrudSpecialColType.TimeStamp)
					&& !CrudGenHelper.IsFieldInherited(typeClass,fieldInfosExceptPri[f+1])) 
				{
					//in case the last field is a timestamp that is not a hist table column inherited from another table
					stringBuilder.Append(")\";");
				}
				else if(f<fieldInfosExceptPri.Count-1) {
					stringBuilder.Append(",\"");
				}
				else {
					stringBuilder.Append(")\";");
				}
			}
			#endregion FieldsExceptPri
			for(int i=0;i<paramList.Count;i++) {
					stringBuilder.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
						stringBuilder.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					stringBuilder.Append(rn+t3+"}");
					//example: OdSqlParameter paramNote=new OdSqlParameter("paramNote",
					//           OdDbType.Text,procNote.Note);
					stringBuilder.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
				if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
					stringBuilder.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
				}
				else {
					stringBuilder.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
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
					stringBuilder.Append(rn+t3+"if(useExistingPK) {");
				}
				else {
					stringBuilder.Append(rn+t3+"if(useExistingPK || isRandomKeys) {");
				}
						stringBuilder.Append(rn+t4+"Db.NonQ(command"+paramsString+");");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"else {");
						stringBuilder.Append(rn+t4+obj+"."+fieldInfoPriKey.Name+"=Db.NonQ(command,true,\""+fieldInfoPriKey.Name+"\",\""+obj+"\""+paramsString+");");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"return "+obj+"."+fieldInfoPriKey.Name+";");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion InsertNoCache
			#region Update
			//Update---------------------------------------------------------------------------------------------
			//get the longest fieldname for alignment purposes
			int longestField=listFieldInfos.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
			if(!CrudGenHelper.IsTableHist(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Updates one "+typeClass.Name+" in the database.</summary>");
				stringBuilder.Append(rn+t2+"public static void Update("+typeClass.Name+" "+obj+") {");
				stringBuilder.Append(rn+t3+"string command=\"UPDATE "+tablename+" SET \"");
				for(int f=0;f<fieldInfosExceptPri.Count;f++) {
					if(isMobile && fieldInfosExceptPri[f]==priKey1) {//2 already skipped
						continue;
					}
					specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						stringBuilder.Append(rn+t4+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						stringBuilder.Append(rn+t4+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						stringBuilder.Append(rn+t4+"//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						stringBuilder.Append(rn+t4+"//"+fieldInfosExceptPri[f].Name+" excluded from update");
						continue;
					}
					stringBuilder.Append(rn+t4+"+\""+fieldInfosExceptPri[f].Name.PadRight(longestField,' ')+"= ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						stringBuilder.Append(" \"+POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append(" \"+POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append(" \"+POut.DateT ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						stringBuilder.Append("'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						stringBuilder.Append("'\"+POut.TSpan ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						stringBuilder.Append(" \"+POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+".Ticks)+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
						stringBuilder.Append(" \"+DbHelper.ParamChar+\"param"+fieldInfosExceptPri[f].Name);
						//paramList is already set above
					}
					else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
						stringBuilder.Append(" \"+POut.Int   ((int)"+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else switch(fieldInfosExceptPri[f].FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
						case "Bitmap":
							stringBuilder.Append(" \"+POut.Bitmap("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Boolean":
							stringBuilder.Append(" \"+POut.Bool  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Byte":
							stringBuilder.Append(" \"+POut.Byte  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Color":
							stringBuilder.Append(" \"+POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToArgb())+\"");
							break;
						case "DateTime"://This is only for date, not dateT
							stringBuilder.Append(" \"+POut.Date  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Double":
							if(specialType.HasFlag(CrudSpecialColType.Double)) {
								int decimalPlaces=(int)fieldInfosExceptPri[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
								stringBuilder.Append(" \"+POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+", decimalPlaces:"+decimalPlaces+")+\""); 
							}
							else {
								stringBuilder.Append(" \"+POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+")+\""); 
							}
							break;
						case "Interval":
							stringBuilder.Append(" \"+POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+".ToInt())+\"");
							break;
						case "Int64":
							stringBuilder.Append(" \"+POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Int32":
							stringBuilder.Append(" \"+POut.Int   ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "Single":
							stringBuilder.Append(" \"+POut.Float ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
						case "String":
							stringBuilder.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
								+obj+"."+fieldInfosExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
							break;
						case "TimeSpan":
							stringBuilder.Append(" \"+POut.Time  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
							break;
					}
					//If ALL the rest of the fields should be skipped, don't add comma
					if(!fieldInfosExceptPri.Skip(f+1).All(x => EnumTools.HasAnyFlag(CrudGenHelper.GetSpecialType(x),
						CrudSpecialColType.TimeStamp,
						CrudSpecialColType.DateEntry,
						CrudSpecialColType.DateTEntry,
						CrudSpecialColType.ExcludeFromUpdate))) 
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(" \"");
				}
				if(isMobile) {
					stringBuilder.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+obj+"."+priKey1.Name+")+\" AND "+priKey2.Name+" = \"+POut.Long("+obj+"."+priKey2.Name+");");
				}
				else {
					stringBuilder.Append(rn+t4+"+\"WHERE "+fieldInfoPriKey.Name+" = \"+POut.Long("+obj+"."+fieldInfoPriKey.Name+");");
				}
				for(int i=0;i<paramList.Count;i++) {
					stringBuilder.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					stringBuilder.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						stringBuilder.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						stringBuilder.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				stringBuilder.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Update
			#region Update 2nd override
			//Update, 2nd override-------------------------------------------------------------------------------
			//NOTE: If any changes are made to Update 2nd override, they need to be reflected in UpdateComparison as well!!!
			if(!isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Updates one "+typeClass.Name+" in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>");
				stringBuilder.Append(rn+t2+"public static bool Update("+typeClass.Name+" "+obj+","+typeClass.Name+" "+oldObj+") {");
				stringBuilder.Append(rn+t3+"string command=\"\";");
				for(int f=0;f<fieldInfosExceptPri.Count;f++) {
					//if(isMobile && fieldsExceptPri[f]==priKey1) {//2 already skipped
					//	continue;
					//}
					specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" excluded from update");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)//DateEntryEditable special type is date data type in the db
						|| (!specialType.HasFlag(CrudSpecialColType.DateT)//if not special type DateT
							&& !specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)//and not DateTEntryEditable
							&& fieldInfosExceptPri[f].FieldType.Name=="DateTime"))//and field name is DateTime, then it is a date data type in the db
					{
						stringBuilder.Append(rn+t3+"if("+obj+"."+fieldInfosExceptPri[f].Name+".Date != "+oldObj+"."+fieldInfosExceptPri[f].Name+".Date) {");
					}
					else {
						stringBuilder.Append(rn+t3+"if("+obj+"."+fieldInfosExceptPri[f].Name+" != "+oldObj+"."+fieldInfosExceptPri[f].Name+") {");
					}
					stringBuilder.Append(rn+t4+"if(command!=\"\") { command+=\",\";}");
					stringBuilder.Append(rn+t4+"command+=\""+fieldInfosExceptPri[f].Name+" = ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						stringBuilder.Append("\"+POut.DateT("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append("\"+POut.Date("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append("\"+POut.DateT("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						stringBuilder.Append("'\"+POut.String("+obj+"."+fieldInfosExceptPri[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						stringBuilder.Append("'\"+POut.TSpan ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						stringBuilder.Append("'\"+POut.Long  ("+obj+"."+fieldInfosExceptPri[f].Name+".Ticks)+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
						stringBuilder.Append("\"+DbHelper.ParamChar+\"param"+fieldInfosExceptPri[f].Name);
						//paramList is already set above
					}
					else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
						stringBuilder.Append("\"+POut.Int   ((int)"+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
					}
					else switch(fieldInfosExceptPri[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
							case "Boolean":
								stringBuilder.Append("\"+POut.Bool("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Bitmap":
								stringBuilder.Append("\"+POut.Bitmap("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Byte":
								stringBuilder.Append("\"+POut.Byte("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Color":
								stringBuilder.Append("\"+POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+".ToArgb())+\"");
								break;
							case "DateTime"://This is only for date, not dateT.
								stringBuilder.Append("\"+POut.Date("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Double":
								if(specialType.HasFlag(CrudSpecialColType.Double)) {
									int decimalPlaces=(int)fieldInfosExceptPri[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
									stringBuilder.Append("\"+POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+", decimalPlaces:"+decimalPlaces+")+\""); 
								}
								else {
									stringBuilder.Append("\"+POut.Double("+obj+"."+fieldInfosExceptPri[f].Name+")+\""); 
								}
								break;
							case "Interval":
								stringBuilder.Append("\"+POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+".ToInt())+\"");
								break;
							case "Int64":
								stringBuilder.Append("\"+POut.Long("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Int32":
								stringBuilder.Append("\"+POut.Int("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "Single":
								stringBuilder.Append("\"+POut.Float("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
							case "String":
								stringBuilder.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
									+obj+"."+fieldInfosExceptPri[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
								break;
							case "TimeSpan":
								stringBuilder.Append("\"+POut.Time  ("+obj+"."+fieldInfosExceptPri[f].Name+")+\"");
								break;
						}
					stringBuilder.Append("\";");
					stringBuilder.Append(rn+t3+"}");
				}
				stringBuilder.Append(rn+t3+"if(command==\"\") {");
				stringBuilder.Append(rn+t4+"return false;");
				stringBuilder.Append(rn+t3+"}");
				for(int i=0;i<paramList.Count;i++) {
					stringBuilder.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					stringBuilder.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
					+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						stringBuilder.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						stringBuilder.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				stringBuilder.Append(rn+t3+"command=\"UPDATE "+tablename+" SET \"+command");
				stringBuilder.Append(rn+t4+"+\" WHERE "+fieldInfoPriKey.Name+" = \"+POut.Long("+obj+"."+fieldInfoPriKey.Name+");");
				stringBuilder.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				stringBuilder.Append(rn+t3+"return true;");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Update 2nd override
			#region UpdateComparison
			//UpdateComparison-------------------------------------------------------------------------------
			if(!isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Returns true if Update("+typeClass.Name+","+typeClass.Name+") would make changes to the database."
					+rn+t2+"///Does not make any changes to the database and can be called before remoting role is checked.</summary>");
				stringBuilder.Append(rn+t2+"public static bool UpdateComparison("+typeClass.Name+" "+obj+","+typeClass.Name+" "+oldObj+") {");
				for(int f = 0;f<fieldInfosExceptPri.Count;f++) {
					//if(isMobile && fieldsExceptPri[f]==priKey1) {//2 already skipped
					//	continue;
					//}
					specialType=CrudGenHelper.GetSpecialType(fieldInfosExceptPri[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						stringBuilder.Append(rn+t3+"//"+fieldInfosExceptPri[f].Name+" excluded from update");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)//DateEntryEditable special type is date data type in the db
						|| (!specialType.HasFlag(CrudSpecialColType.DateT)//if not special type DateT
							&& !specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)//and not DateTEntryEditable
							&& fieldInfosExceptPri[f].FieldType.Name=="DateTime"))//and field name is DateTime, then it is a date data type in the db
					{
						stringBuilder.Append(rn+t3+"if("+obj+"."+fieldInfosExceptPri[f].Name+".Date != "+oldObj+"."+fieldInfosExceptPri[f].Name+".Date) {");
					}
					else {
						stringBuilder.Append(rn+t3+"if("+obj+"."+fieldInfosExceptPri[f].Name+" != "+oldObj+"."+fieldInfosExceptPri[f].Name+") {");
					}
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
						stringBuilder.Append(rn+t4+"return true");
						//paramList is already set above
					}
					else if(fieldInfosExceptPri[f].FieldType.IsEnum) {
						stringBuilder.Append(rn+t4+"return true");
					}
					else switch(fieldInfosExceptPri[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+fieldInfosExceptPri[f].FieldType.Name);
							case "Boolean":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Bitmap":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Byte":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Color":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "DateTime"://This is only for date, not dateT.
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Double":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Interval":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Int64":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Int32":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "Single":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "String":
								stringBuilder.Append(rn+t4+"return true");
								break;
							case "TimeSpan":
								stringBuilder.Append(rn+t4+"return true");
								break;
						}
					stringBuilder.Append(";");
					stringBuilder.Append(rn+t3+"}");
				}
				stringBuilder.Append(rn+t3+"return false;");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion UpdateComparison
			#region UpdateCemt
			//UpdateCemt---------------------------------------------------------------------------------------------
			//get the longest fieldname for alignment purposes
			if(CrudGenHelper.IsTableSkipCemt(typeClass) && !isMobile && !CrudGenHelper.IsTableHist(typeClass)) {
				FieldInfo cemtSyncKey=CrudGenHelper.GetCemtSyncKey(fieldInfoArray,typeClass.Name);
				List<FieldInfo> listFieldsExceptCEMT=CrudGenHelper.GetFieldsExceptNotCemt(fieldInfosExceptPri.ToArray());
				longestField=listFieldsExceptCEMT.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
				stringBuilder.Append(rn+rn+t2+"///<summary>Updates columns that do not have the '"+nameof(CrudColumnAttribute.IsNotCemtColumn)+"' attribute. Uses the '"+cemtSyncKey.Name+"' column instead of the PK column.</summary>");
				stringBuilder.Append(rn+t2+"public static void UpdateCemt("+typeClass.Name+" "+obj+") {");
				stringBuilder.Append(rn+t3+"string command=\"UPDATE "+tablename+" SET \"");
				for(int f = 0;f < listFieldsExceptCEMT.Count;f++) {
					if(isMobile && listFieldsExceptCEMT[f]==priKey1) {//2 already skipped
						continue;
					}
					specialType=CrudGenHelper.GetSpecialType(listFieldsExceptCEMT[f]);
					if(specialType.HasFlag(CrudSpecialColType.DateEntry)) {
						stringBuilder.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.DateTEntry)) {
						stringBuilder.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" not allowed to change");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.TimeStamp)) {
						stringBuilder.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" can only be set by MySQL");
						continue;
					}
					if(specialType.HasFlag(CrudSpecialColType.ExcludeFromUpdate)) {
						stringBuilder.Append(rn+t4+"//"+listFieldsExceptCEMT[f].Name+" excluded from update");
						continue;
					}
					stringBuilder.Append(rn+t4+"+\""+listFieldsExceptCEMT[f].Name.PadRight(longestField,' ')+"= ");
					if(specialType.HasFlag(CrudSpecialColType.DateT)) {
						stringBuilder.Append(" \"+POut.DateT ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateEntryEditable)) {
						stringBuilder.Append(" \"+POut.Date  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.DateTEntryEditable)) {
						stringBuilder.Append(" \"+POut.DateT ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {
						stringBuilder.Append("'\"+POut.String("+obj+"."+listFieldsExceptCEMT[f].Name+".ToString())+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
						stringBuilder.Append("'\"+POut.TSpan ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"'");
					}
					else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
						stringBuilder.Append(" \"+POut.Long  ("+obj+"."+listFieldsExceptCEMT[f].Name+".Ticks)+\"");
					}
					else if(specialType.HasFlag(CrudSpecialColType.IsText)) {
						stringBuilder.Append(" \"+DbHelper.ParamChar+\"param"+listFieldsExceptCEMT[f].Name);
						//paramList is already set above
					}
					else if(listFieldsExceptCEMT[f].FieldType.IsEnum) {
						stringBuilder.Append(" \"+POut.Int   ((int)"+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
					}
					else switch(listFieldsExceptCEMT[f].FieldType.Name) {
							default:
								throw new ApplicationException("Type not yet supported: "+listFieldsExceptCEMT[f].FieldType.Name);
							case "Bitmap":
								stringBuilder.Append(" \"+POut.Bitmap("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Boolean":
								stringBuilder.Append(" \"+POut.Bool  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Byte":
								stringBuilder.Append(" \"+POut.Byte  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Color":
								stringBuilder.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+".ToArgb())+\"");
								break;
							case "DateTime"://This is only for date, not dateT
								stringBuilder.Append(" \"+POut.Date  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Double":
								if(specialType.HasFlag(CrudSpecialColType.Double)) {
									int decimalPlaces=(int)listFieldsExceptCEMT[f].CustomAttributes.First().NamedArguments.FirstOrDefault(x=>x.MemberName=="DecimalPlaces").TypedValue.Value;
									stringBuilder.Append("\"+POut.Double("+obj+"."+listFieldsExceptCEMT[f].Name+", decimalPlaces:"+decimalPlaces+")+\""); 
								}
								else {
									stringBuilder.Append("\"+POut.Double("+obj+"."+listFieldsExceptCEMT[f].Name+")+\""); 
								}
								break;
							case "Interval":
								stringBuilder.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+".ToInt())+\"");
								break;
							case "Int64":
								stringBuilder.Append(" \"+POut.Long  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Int32":
								stringBuilder.Append(" \"+POut.Int   ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "Single":
								stringBuilder.Append(" \"+POut.Float ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
							case "String":
								stringBuilder.Append("'\"+POut.String"+(specialType.HasFlag(CrudSpecialColType.CleanText) ? "Note" : "")+"("
									+obj+"."+listFieldsExceptCEMT[f].Name+(specialType.HasFlag(CrudSpecialColType.CleanText) ? ",true" : "")+")+\"'");
								break;
							case "TimeSpan":
								stringBuilder.Append(" \"+POut.Time  ("+obj+"."+listFieldsExceptCEMT[f].Name+")+\"");
								break;
						}
					//If ALL the rest of the fields should be skipped, don't add comma
					if(!listFieldsExceptCEMT.Skip(f+1).All(x => EnumTools.HasAnyFlag(CrudGenHelper.GetSpecialType(x),
						CrudSpecialColType.TimeStamp,
						CrudSpecialColType.DateEntry,
						CrudSpecialColType.DateTEntry,
						CrudSpecialColType.ExcludeFromUpdate))) {
						stringBuilder.Append(",");
					}
					stringBuilder.Append(" \"");
				}
				if(isMobile) {
					stringBuilder.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+obj+"."+priKey1.Name+")+\" AND "+priKey2.Name+" = \"+POut.Long("+obj+"."+priKey2.Name+");");
				}
				else {
					stringBuilder.Append(rn+t4+"+\"WHERE "+cemtSyncKey.Name+" = \"+POut.Long("+obj+"."+cemtSyncKey.Name+");");
				}
				for(int i = 0;i<paramList.Count;i++) {
					stringBuilder.Append(rn+t3+"if("+obj+"."+paramList[i].ParameterName+"==null) {");
					stringBuilder.Append(rn+t4+""+obj+"."+paramList[i].ParameterName+"=\"\";");
					stringBuilder.Append(rn+t3+"}");
					stringBuilder.Append(rn+t3+"OdSqlParameter param"+paramList[i].ParameterName+"=new OdSqlParameter(\"param"+paramList[i].ParameterName+"\","
						+"OdDbType.Text,");
					if(((CrudSpecialColType)paramList[i].Value).HasFlag(CrudSpecialColType.CleanText)) {
						stringBuilder.Append("POut.StringNote("+obj+"."+paramList[i].ParameterName+"));");//This is where large amounts of consecutive newlines are stripped away.
					}
					else {
						stringBuilder.Append("POut.StringParam("+obj+"."+paramList[i].ParameterName+"));");
					}
				}
				stringBuilder.Append(rn+t3+"Db.NonQ(command"+paramsString+");");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion UpdateCemt
			#region Delete
			//Delete---------------------------------------------------------------------------------------------
			if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"//Delete not allowed for this table");
				stringBuilder.Append(rn+t2+"//public static void Delete(long "+priKeyParam+") {");
				stringBuilder.Append(rn+t2+"//");
				stringBuilder.Append(rn+t2+"//}");
			}
			else {
				stringBuilder.Append(rn+rn+t2+"///<summary>Deletes one "+typeClass.Name+" from the database.</summary>");
				if(isMobile) {
					stringBuilder.Append(rn+t2+"public static void Delete(long "+priKeyParam1+",long "+priKeyParam2+") {");
					stringBuilder.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
					stringBuilder.Append(rn+t4+"+\"WHERE "+priKey1.Name+" = \"+POut.Long("+priKeyParam1+")+\" AND "+priKey2.Name+" = \"+POut.Long("+priKeyParam2+");");
				}
				else {
					stringBuilder.Append(rn+t2+"public static void Delete(long "+priKeyParam+") {");
					if(!listAuditTrailPerms.IsNullOrEmpty()) {
						stringBuilder.Append(rn+t3+"ClearFkey("+priKeyParam+");");
					}
					stringBuilder.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
					stringBuilder.Append(rn+t4+"+\"WHERE "+fieldInfoPriKey.Name+" = \"+POut.Long("+priKeyParam+");");
				}
				stringBuilder.Append(rn+t3+"Db.NonQ(command);");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Delete
			#region Delete Many
			//Delete Many---------------------------------------------------------------------------------------------
			if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"//Delete not allowed for this table");
				stringBuilder.Append(rn+t2+"//public static void DeleteMany(List<long> "+nameListParam+") {");
				stringBuilder.Append(rn+t2+"//");
				stringBuilder.Append(rn+t2+"//}");
			}
			else {
				stringBuilder.Append(rn+rn+t2+"///<summary>Deletes many "+typeClass.Name+"s from the database.</summary>");
				stringBuilder.Append(rn+t2+"public static void DeleteMany(List<long> "+nameListParam+") {");
				stringBuilder.Append(rn+t3+"if("+nameListParam+"==null || "+nameListParam+".Count==0) {");
				stringBuilder.Append(rn+t4+"return;");
				stringBuilder.Append(rn+t3+"}");
				if(!listAuditTrailPerms.IsNullOrEmpty()) {
					stringBuilder.Append(rn+t3+"ClearFkey("+nameListParam+");");
				}
				stringBuilder.Append(rn+t3+"string command=\"DELETE FROM "+tablename+" \"");
				stringBuilder.Append(rn+t4+"+\"WHERE "+fieldInfoPriKey.Name+" IN(\"+string.Join(\",\","+nameListParam+".Select(x => POut.Long(x)))+\")\";");
				stringBuilder.Append(rn+t3+"Db.NonQ(command);");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Delete Many
			#region Sync
			//Synch-----------------------------------------------------------------------------------------
			if(CrudGenHelper.IsSynchable(typeClass) || CrudGenHelper.IsSynchableBatchWriteMethods(typeClass)) {
				stringBuilder.Append(rn+rn+t2+"///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.");
				if(CrudGenHelper.IsSynchableBatchWriteMethods(typeClass) && CrudGenHelper.HasBatchWriteMethods(typeClass)) {
					stringBuilder.Append(rn+t2+"///Caution: Uses InsertMany for inserts, so the objects in listNew will not have primary keys set.");
				}
				if(CrudGenHelper.IsSecurityStamped(typeClass)) {//sec tables that are synchable must have userNum passed in.
					stringBuilder.Append(rn+t2+"///Supply Security.CurUser.UserNum, used to set the SecUserNumEntry field for Inserts.</summary>");
					stringBuilder.Append(rn+t2+"public static bool Sync(List<"+typeClass.Name+"> listNew,List<"+typeClass.Name+"> listDB,long userNum) {");
				}
				else {
					stringBuilder.Append("</summary>");
					stringBuilder.Append(rn+t2+"public static bool Sync(List<"+typeClass.Name+"> listNew,List<"+typeClass.Name+"> listDB) {");
				}
				stringBuilder.Append(rn+t3+"//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.");
				stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> listIns    =new List<"+typeClass.Name+">();");
				stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> listUpdNew =new List<"+typeClass.Name+">();");
				stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> listUpdDB  =new List<"+typeClass.Name+">();");
				stringBuilder.Append(rn+t3+"List<"+typeClass.Name+"> listDel    =new List<"+typeClass.Name+">();");
				stringBuilder.Append(rn+t3+"listNew.Sort(("+typeClass.Name+" x,"+typeClass.Name+" y) => { return x."+fieldInfoPriKey.Name+".CompareTo(y."+fieldInfoPriKey.Name+"); });");
				stringBuilder.Append(rn+t3+"listDB.Sort(("+typeClass.Name+" x,"+typeClass.Name+" y) => { return x."+fieldInfoPriKey.Name+".CompareTo(y."+fieldInfoPriKey.Name+"); });");
				stringBuilder.Append(rn+t3+"int idxNew=0;");
				stringBuilder.Append(rn+t3+"int idxDB=0;");
				stringBuilder.Append(rn+t3+"int rowsUpdatedCount=0;");
				stringBuilder.Append(rn+t3+""+typeClass.Name+" fieldNew;");
				stringBuilder.Append(rn+t3+""+typeClass.Name+" fieldDB;");
				stringBuilder.Append(rn+t3+"//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.");
				stringBuilder.Append(rn+t3+"//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.");
				stringBuilder.Append(rn+t3+"while(idxNew<listNew.Count || idxDB<listDB.Count) {");
				stringBuilder.Append(rn+t4+"fieldNew=null;");
				stringBuilder.Append(rn+t4+"if(idxNew<listNew.Count) {");
				stringBuilder.Append(rn+t5+"fieldNew=listNew[idxNew];");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"fieldDB=null;");
				stringBuilder.Append(rn+t4+"if(idxDB<listDB.Count) {");
				stringBuilder.Append(rn+t5+"fieldDB=listDB[idxDB];");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"//begin compare");
				stringBuilder.Append(rn+t4+"if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.");
				stringBuilder.Append(rn+t5+"listIns.Add(fieldNew);");
				stringBuilder.Append(rn+t5+"idxNew++;");
				stringBuilder.Append(rn+t5+"continue;");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.");
				stringBuilder.Append(rn+t5+"listDel.Add(fieldDB);");
				stringBuilder.Append(rn+t5+"idxDB++;");
				stringBuilder.Append(rn+t5+"continue;");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"else if(fieldNew."+fieldInfoPriKey.Name+"<fieldDB."+fieldInfoPriKey.Name+") {//newPK less than dbPK, newItem is 'next'");
				stringBuilder.Append(rn+t5+"listIns.Add(fieldNew);");
				stringBuilder.Append(rn+t5+"idxNew++;");
				stringBuilder.Append(rn+t5+"continue;");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"else if(fieldNew."+fieldInfoPriKey.Name+">fieldDB."+fieldInfoPriKey.Name+") {//dbPK less than newPK, dbItem is 'next'");
				stringBuilder.Append(rn+t5+"listDel.Add(fieldDB);");
				stringBuilder.Append(rn+t5+"idxDB++;");
				stringBuilder.Append(rn+t5+"continue;");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t4+"//Both lists contain the 'next' item, update required");
				stringBuilder.Append(rn+t4+"listUpdNew.Add(fieldNew);");
				stringBuilder.Append(rn+t4+"listUpdDB.Add(fieldDB);");
				stringBuilder.Append(rn+t4+"idxNew++;");
				stringBuilder.Append(rn+t4+"idxDB++;");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"//Commit changes to DB");
				if(CrudGenHelper.IsSynchableBatchWriteMethods(typeClass) && CrudGenHelper.HasBatchWriteMethods(typeClass)) {
					if(CrudGenHelper.IsSecurityStamped(typeClass)) {
						//if this table IsSecurityStamped, there is a SecUserNumEntry field that needs to be set to the userNum passed in for inserts
						stringBuilder.Append(rn+t3+"listIns.ForEach(x => x.SecUserNumEntry=userNum);");
					}
					stringBuilder.Append(rn+t3+"InsertMany(listIns);");
				}
				else {
					stringBuilder.Append(rn+t3+"for(int i=0;i<listIns.Count;i++) {");
					if(CrudGenHelper.IsSecurityStamped(typeClass)) {
						//if this table IsSecurityStamped, there is a SecUserNumEntry field that needs to be set to the userNum passed in for inserts
						stringBuilder.Append(rn+t4+"listIns[i].SecUserNumEntry=userNum;");
					}
					stringBuilder.Append(rn+t4+"Insert(listIns[i]);");
					stringBuilder.Append(rn+t3+"}");
				}
				stringBuilder.Append(rn+t3+"for(int i=0;i<listUpdNew.Count;i++) {");
				stringBuilder.Append(rn+t4+"if(Update(listUpdNew[i],listUpdDB[i])) {");
				stringBuilder.Append(rn+t5+"rowsUpdatedCount++;");
				stringBuilder.Append(rn+t4+"}");
				stringBuilder.Append(rn+t3+"}");
				if(CrudGenHelper.IsDeleteForbidden(typeClass)) {
					//When Crud.Delete() is forbidden, the only way we could possibly delete a row is if the S class has a specific Delete() function defined.
					//There are very few classes which are both Synchable and where Crud.Delete() is forbidden.
					stringBuilder.Append(rn+t3+"for(int i=0;i<listDel.Count;i++) {");
					stringBuilder.Append(rn+t4+typeClass.Name+"s.Delete(listDel[i]."+fieldInfoPriKey.Name+");");
					stringBuilder.Append(rn+t3+"}");
				}
				else {
					stringBuilder.Append(rn+t3+"DeleteMany(listDel.Select(x => x."+fieldInfoPriKey.Name+").ToList());");
				}
				stringBuilder.Append(rn+t3+"if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {");
				stringBuilder.Append(rn+t4+"return true;");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"return false;");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion Sync
			#region ConvertToM
			if(isMobile) {
				//ConvertToM------------------------------------------------------------------------------------------
				Type typeClassReg=CrudGenHelper.GetTypeFromMType(typeClass.Name,_listTypes);//gets the non-mobile type
				if(typeClassReg==null) {
					stringBuilder.Append(rn+rn+t2+"//ConvertToM not applicable.");
				}
				else{
					string tablenameReg=CrudGenHelper.GetTableName(typeClassReg);//in lowercase now.
					string objReg=typeClassReg.Name.Substring(0,1).ToLower()+typeClassReg.Name.Substring(1);//lowercase initial letter.  Example feeSched
					FieldInfo[] fieldsReg=typeClassReg.GetFields();//We can't assume they are in the correct order.
					List<FieldInfo> fieldsInDbReg=CrudGenHelper.GetFieldsExceptNotDb(fieldsReg);
					stringBuilder.Append(rn+rn+t2+"///<summary>Converts one "+typeClassReg.Name+" object to its mobile equivalent.  Warning! CustomerNum will always be 0.</summary>");
					stringBuilder.Append(rn+t2+"public static "+typeClass.Name+" ConvertToM("+typeClassReg.Name+" "+objReg+") {");
					stringBuilder.Append(rn+t3+typeClass.Name+" "+obj+"=new "+typeClass.Name+"();");
					for(int f=0;f<listFieldInfos.Count;f++) {
						if(listFieldInfos[f].Name=="CustomerNum") {
							stringBuilder.Append(rn+t3+"//CustomerNum cannot be set.  Remains 0.");
							continue;
						}
						bool matchfound=false;
						for(int r=0;r<fieldsInDbReg.Count;r++) {
							if(listFieldInfos[f].Name==fieldsInDbReg[r].Name) {
								stringBuilder.Append(rn+t3+obj+"."+listFieldInfos[f].Name.PadRight(longestField,' ')+"="+objReg+"."+fieldsInDbReg[r].Name+";");
								matchfound=true;
							}
						}
						if(!matchfound) {
							throw new ApplicationException("Match not found.");
						}
					}
					stringBuilder.Append(rn+t3+"return "+obj+";");
					stringBuilder.Append(rn+t2+"}");
				}
			}
			#endregion ConvertToM
			#region ClearFkey(long)
			if(!listAuditTrailPerms.IsNullOrEmpty()) {  //If there are any AuditPerms set for this table
				stringBuilder.Append(rn+rn+t2+"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+" as FKey and are related to "+typeClass.Name+".");
				stringBuilder.Append(rn+t2+"///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClass.Name+@" table type.</summary>");
				stringBuilder.Append(rn+t2+"public static void ClearFkey(long "+priKeyParam+") {");
				List<string> listPermTypes=new List<string>();
				for(int i=0;i<listAuditTrailPerms.Count;i++) {
					listPermTypes.Add(((int)listAuditTrailPerms[i]).ToString());
				}
				stringBuilder.Append(rn+t3+"if("+priKeyParam+"==0) {");
				stringBuilder.Append(rn+t4+"return;");
				stringBuilder.Append(rn+t3+"}");
				stringBuilder.Append(rn+t3+"string command=\"UPDATE securitylog SET FKey=0 WHERE FKey=\"+POut.Long("+priKeyParam+")+"
					+"\" AND PermType IN ("+String.Join(",",listPermTypes)+")\";");  
				//If we wanted to make this more readable we could put a comment into the crud file here of what the listPermTypes mean.
				stringBuilder.Append(rn+t3+"Db.NonQ(command);");
				stringBuilder.Append(rn+t2+"}");
			}
			#endregion ClearFkey(long)
			#region ClearFkey(List<long>)
			if(!listAuditTrailPerms.IsNullOrEmpty()) {  //If there are any AuditPerms set for this table
				stringBuilder.Append(rn+rn+t2+"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+"s as FKey and are related to "+typeClass.Name+".");
				stringBuilder.Append(rn+t2+"///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClass.Name+@" table type.</summary>");
				stringBuilder.Append(rn+t2+"public static void ClearFkey(List<long> "+nameListParam+") {");
				stringBuilder.Append(rn+t3+"if("+nameListParam+"==null || "+nameListParam+".FindAll(x => x != 0).Count==0) {");
				stringBuilder.Append(rn+t4+"return;");
				stringBuilder.Append(rn+t3+"}");
				List<string> listPermTypes=new List<string>();
				for(int i=0;i<listAuditTrailPerms.Count;i++) {
					listPermTypes.Add(((int)listAuditTrailPerms[i]).ToString());
				}
				stringBuilder.Append(rn+t3+"string command=\"UPDATE securitylog SET FKey=0 WHERE FKey IN(\"+String.Join(\",\","+nameListParam
					+".FindAll(x => x != 0))+\") AND PermType IN ("+String.Join(",",listPermTypes)+")\";");
				//If we wanted to make this more readable we could put a comment into the crud file here of what the listPermTypes mean.
				stringBuilder.Append(rn+t3+"Db.NonQ(command);");
				stringBuilder.Append(rn+t2+"}");
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
			stringBuilder.Append(rn);
			stringBuilder.Append(@"
	}
}");
		}

		private static string RowToObj(List<FieldInfo> listFieldInfos,Type typeClass,string obj=null) {
			string objStr=obj??"";
			if(!string.IsNullOrEmpty(obj)) {
				objStr+=".";
			}
			//get the longest fieldname for alignment purposes
			int longestField=listFieldInfos.Select(x => x.Name.Length).DefaultIfEmpty(0).Max();
			StringBuilder stringBuilder=new StringBuilder();
			CrudSpecialColType specialType;
			for(int f = 0;f<listFieldInfos.Count;f++) {
				//Fields are not guaranteed to be in any particular order.
				specialType=CrudGenHelper.GetSpecialType(listFieldInfos[f]);
				if(specialType.HasFlag(CrudSpecialColType.EnumAsString) && !CrudGenHelper.UsesDataReader(typeClass)) {
					string fieldLower=listFieldInfos[f].Name.Substring(0,1).ToLower()+listFieldInfos[f].Name.Substring(1);//lowercase initial letter.  Example clockStatus
					stringBuilder.Append(rn+t4+"string "+fieldLower+"=row[\""+listFieldInfos[f].Name+"\"].ToString();");
					stringBuilder.Append(rn+t4+"if("+fieldLower+"==\"\") {");
					stringBuilder.Append(rn+t5+objStr+listFieldInfos[f].Name.PadRight(longestField-2,' ')+"="
						+"("+listFieldInfos[f].FieldType.ToString()+")0;");
					stringBuilder.Append(rn+t4+"}");
					stringBuilder.Append(rn+t4+"else try{");
					stringBuilder.Append(rn+t5+objStr+listFieldInfos[f].Name.PadRight(longestField-2,' ')+"=("+listFieldInfos[f].FieldType.ToString()+")Enum.Parse(typeof("
						+listFieldInfos[f].FieldType.ToString()+"),"+fieldLower+");");
					stringBuilder.Append(rn+t4+"}");
					stringBuilder.Append(rn+t4+"catch{");
					stringBuilder.Append(rn+t5+objStr+listFieldInfos[f].Name.PadRight(longestField-2,' ')+"=("+listFieldInfos[f].FieldType.ToString()+")0;");
					stringBuilder.Append(rn+t4+"}");
					continue;
				}
				stringBuilder.Append(rn+t4+objStr+listFieldInfos[f].Name.PadRight(longestField,' ')+"= ");
				if(specialType.HasFlag(CrudSpecialColType.DateT)
					|| specialType.HasFlag(CrudSpecialColType.TimeStamp)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntry)
					|| specialType.HasFlag(CrudSpecialColType.DateTEntryEditable))
				{
					//specialTypes.DateEntry and DateEntryEditable is handled fine by the normal DateTime (date) below.
					stringBuilder.Append("PIn.DateT (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanNeg)) {
					stringBuilder.Append("PIn.TSpan (");
				}
				else if(specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append("TimeSpan.FromTicks(PIn.Long(");
				}
				else if(specialType.HasFlag(CrudSpecialColType.EnumAsString)) {//already made sure to only use this pattern if UsesDataReader=true
					stringBuilder.Append("PIn.Enum<"+listFieldInfos[f].FieldType.ToString()+">(");//.ToString() instead of .Name to get fully qualified name
				}
				//no special treatment for specialType clob
				else if(listFieldInfos[f].FieldType.IsEnum) {
					stringBuilder.Append("("+listFieldInfos[f].FieldType.ToString()+")PIn.Int(");//.ToString() instead of .Name to get fully qualified name
				}
				else switch(listFieldInfos[f].FieldType.Name) {
						default:
							throw new ApplicationException("Type not yet supported: "+listFieldInfos[f].FieldType.Name);
						case "Bitmap":
							stringBuilder.Append("PIn.Bitmap(");
							break;
						case "Boolean":
							stringBuilder.Append("PIn.Bool  (");
							break;
						case "Byte":
							stringBuilder.Append("PIn.Byte  (");
							break;
						case "Color":
							stringBuilder.Append("Color.FromArgb(PIn.Int(");
							break;
						case "DateTime"://This ONLY handles date, not dateT which is a special type.
							stringBuilder.Append("PIn.Date  (");
							break;
						case "Double":
							stringBuilder.Append("PIn.Double(");
							break;
						case "Interval":
							stringBuilder.Append("new Interval(PIn.Int(");
							break;
						case "Int64":
							stringBuilder.Append("PIn.Long  (");
							break;
						case "Int32":
							stringBuilder.Append("PIn.Int   (");
							break;
						case "Single":
							stringBuilder.Append("PIn.Float (");
							break;
						case "String":
							stringBuilder.Append("PIn.String(");
							break;
						case "TimeSpan":
							stringBuilder.Append("PIn.Time(");
							break;
				}
				stringBuilder.Append("row[\""+listFieldInfos[f].Name+"\"].ToString()"+(specialType.HasFlag(CrudSpecialColType.EnumAsString)?",true":"")+")");
				if(listFieldInfos[f].FieldType.Name=="Color" || listFieldInfos[f].FieldType.Name=="Interval" || specialType.HasFlag(CrudSpecialColType.TimeSpanLong)) {
					stringBuilder.Append(")");
				}
				stringBuilder.Append(string.IsNullOrEmpty(objStr)?",":";");
			}
			return stringBuilder.ToString();
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
			Type typeClass=_listTypes[listClass.SelectedIndex];
			SnippetType snipType=(SnippetType)comboType.SelectedIndex;
			bool isMobile=false;
			string snippet=CrudGenDataInterface.GetSnippet(typeClass,snipType,isMobile);
			textSnippet.Text=snippet;
			Clipboard.SetText(snippet);
		}

		private void butPd_Click(object sender,EventArgs e) {
			PatientDataGenerator.Generate();
			//MessageBox.Show("Done");
			Application.Exit();
		}

		private void butAddPrefs_Click(object sender,EventArgs e) {
			string server="localhost"+(string.IsNullOrEmpty(textPortNum.Text) ? "" : $":{textPortNum.Text}");
			try {
					CrudGenHelper.ConnectToDatabase(textDb.Text,server:server,password:textPassword.Text);
				}
				catch (Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return;
				}
			CrudGenHelper.AddMissingPreferences(convertDbFile);
		}

		private void Form1_FormClosing(object sender,FormClosingEventArgs e) {
			CrudGenHelper.AddToOpenDentBusiness(ListFilesToAddToODBiz);
		}

		
	}
}
