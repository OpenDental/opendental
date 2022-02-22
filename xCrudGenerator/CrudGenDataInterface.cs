using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;

namespace xCrudGenerator {
	public class CrudGenDataInterface {
		private const string rn="\r\n";
		private const string t="\t";
		private const string t2="\t\t";
		private const string t3="\t\t\t";
		private const string t4="\t\t\t\t";
		private const string t5="\t\t\t\t\t";

		public static string GetSnippet(Type typeClass,SnippetType snipType,bool isMobile){
			//bool isMobile=CrudGenHelper.IsMobile(typeClass);
			string Sname=GetSname(typeClass.Name);
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
			string obj=typeClass.Name.Substring(0,1).ToLower()+typeClass.Name.Substring(1);//lowercase initial letter.  Example feeSched
			string tablename=CrudGenHelper.GetTableName(typeClass);//in lowercase now.  Example feesched
			List<FieldInfo> fieldsExceptPri=null;
			if(isMobile) {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey2);//for mobile, only excludes PK2
			}
			else {
				fieldsExceptPri=CrudGenHelper.GetFieldsExceptPriKey(fields,priKey);
			}
			bool isTableHist=CrudGenHelper.IsTableHist(typeClass);
			List<DbSchemaCol> cols=CrudQueries.GetListColumns(priKey.Name,null,fieldsExceptPri,false);
			switch(snipType){
				default:
					return "snippet type not found.";
				case SnippetType.Insert:
					if(isMobile) {
						return GetInsert(typeClass.Name,obj,null,true);
					}
					else {
						return GetInsert(typeClass.Name,obj,priKey.Name,false);
					}
				case SnippetType.Update:
					return GetUpdate(typeClass.Name,obj,isMobile,isTableHist);
				case SnippetType.EntireSclass:
					if(isMobile) {
						return GetEntireSclassMobile(typeClass.Name,obj,priKey1.Name,priKey2.Name,Sname,tablename,priKeyParam1,priKeyParam2,isTableHist);
					}
					else {
						List<Permissions> listAuditTrailPerms=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeClass));
						return GetEntireSclass(typeClass.Name,obj,priKey.Name,Sname,tablename,priKeyParam,listAuditTrailPerms,isTableHist);
					}
				case SnippetType.AddTable:
					return CrudSchemaRaw.AddTable(tablename,cols,0,false,false);
				case SnippetType.AddColumnEnd:
					return string.Join("\r\n\r\n",cols.Select(x => CrudSchemaRaw.AddColumnEnd(tablename,x,0,false,typeClass).Trim()));
				case SnippetType.AddIndex:
					return string.Join("\r\n\r\n",cols.Select(x => CrudSchemaRaw.AddIndex(tablename,x.ColumnName,0).Trim()));
				case SnippetType.DropColumn:
					return string.Join("\r\n\r\n",cols.Select(x => CrudSchemaRaw.DropColumn(tablename,x.ColumnName,0).Trim()));
			}
		}

		private static string GetSname(string typeClassName){
			string Sname=typeClassName;
			if(typeClassName=="Etrans") {
				return "Etranss";
			}
			if(typeClassName=="SecurityLogHash") {
				return "SecurityLogHashes";
			}
			//if(typeClassName=="RegistrationKey") {
			//	return "RegistrationKeys";
			//}
			if(typeClassName=="Language") {
				return "Lans";
			}
			if(Sname.EndsWith("s")){
				Sname=Sname+"es";
			}
			else if(Sname.EndsWith("ch")){
				Sname=Sname+"es";
			}
			else if(Sname.EndsWith("ay")) {
				Sname=Sname+"s";
			}
			else if(Sname.EndsWith("ey")) {//eg key
				Sname=Sname+"s";
			}
			else if(Sname.EndsWith("y")) {
				Sname=Sname.TrimEnd('y')+"ies";
			}
			else {
				Sname=Sname+"s";
			}
			return Sname;
		}

		///<summary>Creates the Data Interface "s" classes for new tables, complete with typical stubs.  Asks user first.</summary>
		public static void Create(string convertDbFile,Type typeClass,string dbName,bool isMobile) {
			string Sname=GetSname(typeClass.Name);
			string fileName=null;
			if(isMobile) {
				fileName=@"..\..\..\OpenDentBusiness\Mobile\Data Interface\"+Sname+".cs";
			}
			else {
				fileName=@"..\..\..\OpenDentBusiness\Data Interface\"+Sname+".cs";
			}
			if(File.Exists(fileName)) {
				return;
			}
			if(CrudGenHelper.IsMissingInGeneral(typeClass)) {
				return;
			}
			if(MessageBox.Show("Create stub for "+fileName+"?","",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			string snippet=GetSnippet(typeClass,SnippetType.EntireSclass,isMobile);
			File.WriteAllText(fileName,snippet);
			CrudGenHelper.AddToSubversion(Path.Combine(Application.StartupPath,fileName));
			Form1.ListFilesToAddToODBiz.Add(Path.Combine("Data Interface",Path.GetFileName(fileName)));
			MessageBox.Show(fileName+" has been created.  It has been added to SVN and it will be added to the project on close.");
		}

		private static string GetCacheRegion(string typeClassName,string tablename,string obj,string SName) {
			string str=@"		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class "+typeClassName+"Cache : CacheListAbs<"+typeClassName+@"> {
			protected override List<"+typeClassName+@"> GetCacheFromDb() {
				string command=""SELECT * FROM "+tablename+@""";
				return Crud."+typeClassName+@"Crud.SelectMany(command);
			}
			protected override List<"+typeClassName+@"> TableToList(DataTable table) {
				return Crud."+typeClassName+@"Crud.TableToList(table);
			}
			protected override "+typeClassName+" Copy("+typeClassName+" "+obj+@") {
				return "+obj+@".Copy();
			}
			protected override DataTable ListToTable(List<"+typeClassName+"> list"+SName+@") {
				return Crud."+typeClassName+@"Crud.ListToTable(list"+GetSname(typeClassName)+",\""+typeClassName+@""");
			}
			protected override void FillCacheIfNeeded() {
				"+SName+@".GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static "+typeClassName+"Cache _"+obj+"Cache=new "+typeClassName+@"Cache();

		public static List<"+typeClassName+@"> GetDeepCopy(bool isShort=false) {
			return _"+obj+@"Cache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _"+obj+@"Cache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<"+typeClassName+@"> match,bool isShort=false) {
			return _"+obj+@"Cache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<"+typeClassName+@"> match,bool isShort=false) {
			return _"+obj+@"Cache.GetFindIndex(match,isShort);
		}

		public static "+typeClassName+@" GetFirst(bool isShort=false) {
			return _"+obj+@"Cache.GetFirst(isShort);
		}

		public static "+typeClassName+@" GetFirst(Func<"+typeClassName+@",bool> match,bool isShort=false) {
			return _"+obj+@"Cache.GetFirst(match,isShort);
		}

		public static "+typeClassName+@" GetFirstOrDefault(Func<"+typeClassName+@",bool> match,bool isShort=false) {
			return _"+obj+@"Cache.GetFirstOrDefault(match,isShort);
		}

		public static "+typeClassName+@" GetLast(bool isShort=false) {
			return _"+obj+@"Cache.GetLast(isShort);
		}

		public static "+typeClassName+@" GetLastOrDefault(Func<"+typeClassName+@",bool> match,bool isShort=false) {
			return _"+obj+@"Cache.GetLastOrDefault(match,isShort);
		}

		public static List<"+typeClassName+@"> GetWhere(Predicate<"+typeClassName+@"> match,bool isShort=false) {
			return _"+obj+@"Cache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_"+obj+@"Cache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name=""doRefreshCache"">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_"+obj+@"Cache.FillCacheFromTable(table);
				return table;
			}
			return _"+obj+@"Cache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/";
			return str;
		}

		/// <summary>priKeyName will be null if mobile.</summary>
		private static string GetInsert(string typeClassName,string obj,string priKeyName,bool isMobile){
			string retVal=null;
			if(isMobile) {
				retVal=@"		///<summary></summary>
		public static long Insert("+typeClassName+@" "+obj+@"){
			return Crud."+typeClassName+@"Crud.Insert("+obj+@",true);
		}";
			}
			else {
				retVal=@"		///<summary></summary>
		public static long Insert("+typeClassName+@" "+obj+@"){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				"+obj+@"."+priKeyName+@"=Meth.GetLong(MethodBase.GetCurrentMethod(),"+obj+@");
				return "+obj+@"."+priKeyName+@";
			}
			return Crud."+typeClassName+@"Crud.Insert("+obj+@");
		}";
			}
			return retVal;
		}

		private static string GetUpdate(string typeClassName,string obj,bool isMobile,bool isTableHist){
			string retVal=null;
			if(isTableHist) {
				retVal=@"		//Hist tables do not include Update methods";
			}
			else if(isMobile) {
				retVal=@"		///<summary></summary>
		public static void Update("+typeClassName+@" "+obj+@"){
			Crud."+typeClassName+@"Crud.Update("+obj+@");
		}";
			}
			else {
				retVal=@"		///<summary></summary>
		public static void Update("+typeClassName+@" "+obj+@"){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),"+obj+@");
				return;
			}
			Crud."+typeClassName+@"Crud.Update("+obj+@");
		}";
			}
			return retVal;
		}

		private static string GetClearFkey(List<Permissions> listPermissions,string typeClassName,string priKeyParam,string priKeyName) {
			string retVal="";
			if(listPermissions==null || listPermissions.Count==0){
				return retVal;
			}
			retVal=@"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+" as FKey and are related to "+typeClassName+@".
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClassName+@" table type.</summary>
		public static void ClearFkey(long "+priKeyParam+@") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),"+priKeyParam+@");
				return;
			}
			Crud."+typeClassName+@"Crud.ClearFkey("+priKeyParam+@");
		}";
			return retVal;
		}

		private static string GetClearFkeyList(List<Permissions> listPermissions,string typeClassName,string priKeyParam,string priKeyName) {
			string retVal="";
			if(listPermissions==null || listPermissions.Count==0) {
				return retVal;
			}
			string nameListPriKeyParam="list"+priKeyParam.Substring(0,1).ToUpper()+priKeyName.Substring(1)+"s";
			retVal=@"///<summary>Zeros securitylog FKey column for rows that are using the matching "+priKeyParam+"s as FKey and are related to "+typeClassName+@".
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the "+typeClassName+@" table type.</summary>
		public static void ClearFkey(List<long> "+nameListPriKeyParam+@") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),"+nameListPriKeyParam+@");
				return;
			}
			Crud."+typeClassName+@"Crud.ClearFkey("+nameListPriKeyParam+@");
		}";
			return retVal;
		}

		private static string GetEntireSclass(string typeClassName,string obj,string priKeyName,string Sname,string tablename,string priKeyParam,
			List<Permissions> listAuditTrailPerms,bool isTableHist)
		{
			string str=@"using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class "+Sname+@"{
"+GetCacheRegion(typeClassName,tablename,obj,Sname)+@"
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<"+typeClassName+@"> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<"+typeClassName+@">>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=""SELECT * FROM "+tablename+@" WHERE PatNum = ""+POut.Long(patNum);
			return Crud."+typeClassName+@"Crud.SelectMany(command);
		}
		
		///<summary>Gets one "+typeClassName+@" from the db.</summary>
		public static "+typeClassName+@" GetOne(long "+priKeyParam+@"){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<"+typeClassName+@">(MethodBase.GetCurrentMethod(),"+priKeyParam+@");
			}
			return Crud."+typeClassName+@"Crud.SelectOne("+priKeyParam+@");
		}
		#endregion Methods - Get
		#region Methods - Modify
"+GetInsert(typeClassName,obj,priKeyName,false)+@"
"+GetUpdate(typeClassName,obj,false,isTableHist)+@"
		///<summary></summary>
		public static void Delete(long "+priKeyParam+@") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),"+priKeyParam+@");
				return;
			}
			Crud."+typeClassName+@"Crud.Delete("+priKeyParam+@");
		}
		#endregion Methods - Modify
		#region Methods - Misc
		"+GetClearFkey(listAuditTrailPerms,typeClassName,priKeyParam,priKeyName)+@"

		"+GetClearFkeyList(listAuditTrailPerms,typeClassName,priKeyParam,priKeyName)+@"
		#endregion Methods - Misc
		*/



	}
}";
			return str;			
		}

		/// <summary>priKeyParam1 is CustomerNum for now.</summary>
		private static string GetEntireSclassMobile(string typeClassName,string obj,string priKeyName1,string priKeyName2,string Sname,string tablename,string priKeyParam1,string priKeyParam2,bool isTableHist) {
			string str=@"using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class "+Sname+@"{
		
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<"+typeClassName+@"> Refresh(long patNum){
			string command=""SELECT * FROM "+tablename+@" WHERE PatNum = ""+POut.Long(patNum);
			return Crud."+typeClassName+@"Crud.SelectMany(command);
		}

		///<summary>Gets one "+typeClassName+@" from the db.</summary>
		public static "+typeClassName+@" GetOne(long "+priKeyParam1+",long "+priKeyParam2+@"){
			return Crud."+typeClassName+@"Crud.SelectOne("+priKeyParam1+","+priKeyParam2+@");
		}

"+GetInsert(typeClassName,obj,null,true)+@"

"+GetUpdate(typeClassName,obj,true,isTableHist)+@"

		///<summary></summary>
		public static void Delete(long "+priKeyParam1+",long "+priKeyParam2+@") {
			string command= ""DELETE FROM "+tablename+@" WHERE "+priKeyName1+@" = ""+POut.Long("+priKeyParam1+")+\" AND "+priKeyName2+@" = ""+POut.Long("+priKeyParam2+@");
			Db.NonQ(command);
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<"+typeClassName+@"> ConvertListToM(List<"+typeClassName.Substring(0,typeClassName.Length-1)+@"> list) {
			List<"+typeClassName+@"> retVal=new List<"+typeClassName+@">();
			for(int i=0;i<list.Count;i++){
				retVal.Add(Crud."+typeClassName+@"Crud.ConvertToM(list[i]));
			}
			return retVal;
		}

		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<"+typeClassName+@"> list,long "+priKeyParam1+@") {
			for(int i=0;i<list.Count;i++){
				list[i].CustomerNum="+priKeyParam1+@";
				"+typeClassName+" "+obj+@"=Crud."+typeClassName+@"Crud.SelectOne("+priKeyParam1+",list[i]."+priKeyName2+@");
				if("+obj+@"==null){//not in db
					Crud."+typeClassName+@"Crud.Insert(list[i],true);
				}
				else{
					Crud."+typeClassName+@"Crud.Update(list[i]);
				}
			}
		}
		*/



	}
}";
			return str;
		}

		/*
		///<summary>priKeyName2 will be null if not mobile.</summary>
		private static string GetCreateTable(string tablename,string priKeyName1,string priKeyName2,List<FieldInfo> fieldsExceptPri) {
			StringBuilder strb=new StringBuilder();
			CrudQueries.GetCreateTable(strb,tablename,priKeyName1,priKeyName2,fieldsExceptPri);
			return strb.ToString();
		}*/








	}
}
