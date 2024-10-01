using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LanguageForeigns {
		#region Cache Pattern

		///<summary>Utilizes the NonPkAbs version of CacheDict because it uses a custom Key instead of the PK LanguageForeignNum.</summary>
		private class LanguageForeignCache : CacheDictNonPkAbs<LanguageForeign,string,LanguageForeign> {
			protected override List<LanguageForeign> GetCacheFromDb() {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return new List<LanguageForeign>();//since DataTable is ignored anyway if on the client, this won't crash.
			}
			//load all translations for the current culture, using other culture of same language if no trans avail.
			string command=
				"SELECT * FROM languageforeign "
				+"WHERE Culture LIKE '"+CultureInfo.CurrentCulture.TwoLetterISOLanguageName+"%' "
				+"ORDER BY Culture";
				return Crud.LanguageForeignCrud.SelectMany(command);
			}
			protected override List<LanguageForeign> TableToList(DataTable table) {
				return Crud.LanguageForeignCrud.TableToList(table);
			}
			protected override LanguageForeign Copy(LanguageForeign languageForeign) {
				return languageForeign.Copy();
			}
			protected override DataTable DictToTable(Dictionary<string,LanguageForeign> dictionaryLanguageForeigns) {
				return Crud.LanguageForeignCrud.ListToTable(dictionaryLanguageForeigns.Values.Cast<LanguageForeign>().ToList(),"LanguageForeign");
			}
			protected override void FillCacheIfNeeded() {
				LanguageForeigns.GetTableFromCache(false);
			}
			protected override string GetDictKey(LanguageForeign languageForeign) {
				return languageForeign.ClassType+languageForeign.English;
			}
			protected override LanguageForeign GetDictValue(LanguageForeign languageForeign) {
				return languageForeign;
			}
			protected override LanguageForeign CopyDictValue(LanguageForeign languageForeign) {
				return languageForeign.Copy();
			}

			protected override DataTable ListToTable(List<LanguageForeign> listLanguageForeigns) {
				return Crud.LanguageForeignCrud.ListToTable(listLanguageForeigns);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LanguageForeignCache _languageForeignCache=new LanguageForeignCache();

		public static LanguageForeign GetOne(string classTypeEnglish) {
			return _languageForeignCache.GetOne(classTypeEnglish);
		}

		public static bool GetContainsKey(string classTypeEnglish) {
			return _languageForeignCache.GetContainsKey(classTypeEnglish);
		}

		///<summary>This will not do anything if the current region is US.
		///Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return null;
			}
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_languageForeignCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_languageForeignCache.FillCacheFromTable(table);
				return table;
			}
			return _languageForeignCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(LanguageForeign languageForeign){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				languageForeign.LanguageForeignNum=Meth.GetLong(MethodBase.GetCurrentMethod(),languageForeign);
				return languageForeign.LanguageForeignNum;
			}
			return Crud.LanguageForeignCrud.Insert(languageForeign);
		}

		///<summary></summary>
		public static void Update(LanguageForeign languageForeign) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),languageForeign);
				return;
			}
			Crud.LanguageForeignCrud.Update(languageForeign);
		}

		///<summary></summary>
		public static void Delete(LanguageForeign languageForeign){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),languageForeign);
				return;
			}
			Crud.LanguageForeignCrud.Delete(languageForeign.LanguageForeignNum);
		}

		///<summary>Only used during export to get a list of all translations for specified culture only.</summary>
		public static List<LanguageForeign> GetListForCurrentCulture(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LanguageForeign>>(MethodBase.GetCurrentMethod());
			}
			string command=
				"SELECT * FROM languageforeign "
				+"WHERE Culture='"+CultureInfo.CurrentCulture.Name+"'";
			return Crud.LanguageForeignCrud.SelectMany(command);
		}

		///<summary>Used in FormTranslation to get all translations for all cultures for one classtype</summary>
		public static List<LanguageForeign> GetListForType(string classType){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LanguageForeign>>(MethodBase.GetCurrentMethod(),classType);
			}
			string command=
				"SELECT * FROM languageforeign "
				+"WHERE ClassType='"+POut.String(classType)+"'";
			return Crud.LanguageForeignCrud.SelectMany(command);
		}
		
		///<summary>Used in FormTranslation to get a single entry for the specified culture.  The culture match must be extact.  If no translation entries, then it returns null.</summary>
		public static LanguageForeign GetForCulture(List<LanguageForeign> listLanguageForeignsForType,string english,string cultureName){
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listLanguageForeignsForType.Count;i++){
				if(english!=listLanguageForeignsForType[i].English){
					continue;
				}
				if(cultureName!=listLanguageForeignsForType[i].Culture){
					continue;
				}
				return listLanguageForeignsForType[i];
			}
			return null;
		}

		///<summary>Used in FormTranslation to get a single entry with the same language as the specified culture, but only for a different culture.  For instance, if culture is es-PR (Spanish-PuertoRico), then it will return any spanish translation that is NOT from Puerto Rico.  If no other translation entries, then it returns null.</summary>
		public static LanguageForeign GetOther(List<LanguageForeign> listLanguageForeignsForType,string english,string cultureName){
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listLanguageForeignsForType.Count;i++){
				if(english!=listLanguageForeignsForType[i].English){
					continue;
				}
				if(cultureName==listLanguageForeignsForType[i].Culture){
					continue;
				}
				if(cultureName.Substring(0,2)!=listLanguageForeignsForType[i].Culture.Substring(0,2)){
					continue;
				}
				return listLanguageForeignsForType[i];
			}
			return null;
		}
	}

	

	

}













