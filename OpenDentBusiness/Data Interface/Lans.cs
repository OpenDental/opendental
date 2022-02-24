using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Handles database commands for the language table in the database.</summary>
	public class Lans {
		#region CachePattern

		///<summary>Utilizes the NonPkAbs version of CacheDict because it uses a custom Key instead of the PK LanguageNum.</summary>
		private class LanguageCache : CacheDictNonPkAbs<Language,string,Language> {
			protected override List<Language> GetCacheFromDb() {
				string command="SELECT * FROM language";
				return Crud.LanguageCrud.SelectMany(command);
			}
			protected override List<Language> TableToList(DataTable table) {
				return Crud.LanguageCrud.TableToList(table);
			}
			protected override Language Copy(Language language) {
				return language.Copy();
			}
			protected override DataTable DictToTable(Dictionary<string,Language> dictLanguages) {
				return Crud.LanguageCrud.ListToTable(dictLanguages.Values.ToList(),"Language");
			}
			protected override void FillCacheIfNeeded() {
				Lans.GetTableFromCache(false);
			}
			protected override string GetDictKey(Language language) {
				return language.ClassType+language.English;
			}
			protected override Language GetDictValue(Language language) {
				return language;
			}
			protected override Language CopyDictValue(Language language) {
				return language.Copy();
			}

			protected override DataTable ListToTable(List<Language> listAllItems) {
				return Crud.LanguageCrud.ListToTable(listAllItems);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LanguageCache _languageCache=new LanguageCache();

		public static bool DictIsNull() {
			return _languageCache.DictIsNull();
		}

		///<summary>Does not do anything if the current region is US.
		///Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return null;
			}
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return;
			}
			_languageCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_languageCache.FillCacheFromTable(table);
				return table;
			}
			return _languageCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Converts a string to the current language.</summary>
		public static string g(string classType,string text) {
			//No need to check RemotingRole; no call to db.
			string retVal=Lans.ConvertString(classType,text);
			//if(ItemInserted) {
			//	RefreshCache();
			//}
			return retVal;
		}

		///<summary>Converts a string to the current language.</summary>
		public static string g(System.Object sender,string text) {
			//No need to check RemotingRole; no call to db.
			string retVal=Lans.ConvertString(sender.GetType().Name,text);
			//if(ItemInserted) {
			//	RefreshCache();
			//}
			return retVal;
		}

		///<summary>This is where all the action happens.  This method is used by all the others.
		///This is always run on the client rather than the server, unless, of course, it's being called from the server.
		///If it inserts an item into the db table, it will also add it to the local cache, but will not trigger a refresh on both ends.</summary>
		public static string ConvertString(string classType,string text) {
			//No need to check RemotingRole; no call to db.
			if(classType==null || text==null) {
				return "";
			}
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return text;
			}
			if(text.Trim()=="") {
				return "";
			}
			if(DictIsNull()) {//Use this just in case this check is happening before the db connection is established (e.g. Splash Screen)
				return text;
			}
			Language mylan=new Language();
			mylan.ClassType=classType;
			mylan.English=text;
			if(_languageCache.AddValueForKey(classType+text,mylan)) {
				Insert(mylan);
				return text;
			}
			if(LanguageForeigns.GetContainsKey(classType+text)) {
				if(((LanguageForeign)LanguageForeigns.GetOne(classType+text)).Translation=="") {
					//if translation is empty
					return text;//return the English version
				}
				return ((LanguageForeign)LanguageForeigns.GetOne(classType+text)).Translation;
			}
			else {
				return text;
			}
		}

		///<summary></summary>
		public static long Insert(Language language) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				language.LanguageNum=Meth.GetLong(MethodBase.GetCurrentMethod(),language);
				return language.LanguageNum;
			}
			return Crud.LanguageCrud.Insert(language);
		}

		/*
		///<summary>not used to update the english version of text.  Create new instead.</summary>
		public static void UpdateCur(){
			string command="UPDATE language SET "
				+"EnglishComments = '" +POut.PString(Cur.EnglishComments)+"'"
				+",IsObsolete = '"     +POut.PBool  (Cur.IsObsolete)+"'"
				+" WHERE ClassType = BINARY '"+POut.PString(Cur.ClassType)+"'"
				+" AND English = BINARY '"     +POut.PString(Cur.English)+"'";
			NonQ(false);
		}*/

		///<summary>No need to refresh after this.</summary>
		public static void DeleteItems(string classType,List<string> englishList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),classType,englishList);
				return;
			}
			string command="DELETE FROM language WHERE ClassType='"+POut.String(classType)+"' AND (";
			for(int i=0;i<englishList.Count;i++) {
				if(i>0) {
					command+="OR ";
				}
				command+="English='"+POut.String(englishList[i])+"' ";
				_languageCache.RemoveKey(classType+englishList[i]);
			}
			command+=")";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static string[] GetListCat() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT Distinct ClassType FROM language ORDER BY ClassType ";
			DataTable table=Db.GetTable(command);
			string[] ListCat=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++) {
				ListCat[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return ListCat;
		}

		///<summary>Only used in translation tool to get list for one category</summary>
		public static Language[] GetListForCat(string classType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Language[]>(MethodBase.GetCurrentMethod(),classType);
			}
			string command="SELECT * FROM language "
				+"WHERE ClassType = BINARY '"+POut.String(classType)+"' ORDER BY English";
			return Crud.LanguageCrud.SelectMany(command).ToArray();
		}

		///<summary>This had to be added because SilverLight does not allow globally setting the current culture format.</summary>
		public static string GetShortDateTimeFormat() {
			//No need to check RemotingRole; no call to db.
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				//DateTimeFormatInfo formatinfo=(DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
				//formatinfo.ShortDatePattern="MM/dd/yyyy";
				//return formatinfo;
				return "MM/dd/yyyy";
			}
			else {
				return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			}
		}

		///<summary>Gets a short time format for displaying in appt and schedule along the sides. Pass in a clone of the current culture; it will get altered. Returns a string format.</summary>
		public static string GetShortTimeFormat(CultureInfo ci) {
			//No need to check RemotingRole; no call to db.
			string hFormat="";
			ci.DateTimeFormat.AMDesignator=ci.DateTimeFormat.AMDesignator.ToLower();
			ci.DateTimeFormat.PMDesignator=ci.DateTimeFormat.PMDesignator.ToLower();
			string shortTimePattern=ci.DateTimeFormat.ShortTimePattern;
			if(shortTimePattern.IndexOf("hh")!=-1) {//if hour is 01-12
				hFormat+="hh";
			}
			else if(shortTimePattern.IndexOf("h")!=-1) {//or if hour is 1-12
				hFormat+="h";
			}
			else if(shortTimePattern.IndexOf("HH")!=-1) {//or if hour is 00-23
				hFormat+="HH";
			}
			else {//hour is 0-23
				hFormat+="H";
			}
			if(shortTimePattern.IndexOf("t")!=-1) {//if there is an am/pm designator
				hFormat+="tt";
			}
			else {//if no am/pm designator, then use :00
				hFormat+=":00";//time separator will actually change according to region
			}
			return hFormat;
		}

		///<summary>This is one rare situation where queries can be passed.  But it will always fail for client web and server web.</summary>
		public static void LoadTranslationsFromTextFile(string content) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				throw new ApplicationException("Not allowed.");
			}
			Db.NonQ(content);
		}

		public static string[] TranslateArray(string classType,string[] strings){
			string[] retVal=new string[strings.Length];
			for(int i=0;i<strings.Length;i++){
				retVal[i]=g(classType,strings[i]);
			}
			return retVal;
		}		
	}

	//<summary>Implements the ITranslate interface by calling Lans.g().</summary>
	//public class LansTranslate:ITranslate {
		//<summary>Calls Lans.g().</summary>
		//public string Translate(object sender,string text) {
		//	return Lans.g(sender,text);
		//}
	//}
}
