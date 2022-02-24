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
			protected override DataTable DictToTable(Dictionary<string,LanguageForeign> dictLanguageForeigns) {
				return Crud.LanguageForeignCrud.ListToTable(dictLanguageForeigns.Values.Cast<LanguageForeign>().ToList(),"LanguageForeign");
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

			protected override DataTable ListToTable(List<LanguageForeign> listAllItems) {
				return Crud.LanguageForeignCrud.ListToTable(listAllItems);
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_languageForeignCache.FillCacheFromTable(table);
				return table;
			}
			return _languageForeignCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(LanguageForeign lanf){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				lanf.LanguageForeignNum=Meth.GetLong(MethodBase.GetCurrentMethod(),lanf);
				return lanf.LanguageForeignNum;
			}
			return Crud.LanguageForeignCrud.Insert(lanf);
		}

		///<summary></summary>
		public static void Update(LanguageForeign lanf) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),lanf);
				return;
			}
			Crud.LanguageForeignCrud.Update(lanf);
		}

		///<summary></summary>
		public static void Delete(LanguageForeign lanf){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),lanf);
				return;
			}
			Crud.LanguageForeignCrud.Delete(lanf.LanguageForeignNum);
		}

		///<summary>Only used during export to get a list of all translations for specified culture only.</summary>
		public static LanguageForeign[] GetListForCurrentCulture(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LanguageForeign[]>(MethodBase.GetCurrentMethod());
			}
			string command=
				"SELECT * FROM languageforeign "
				+"WHERE Culture='"+CultureInfo.CurrentCulture.Name+"'";
			return Crud.LanguageForeignCrud.SelectMany(command).ToArray();
		}

		///<summary>Used in FormTranslation to get all translations for all cultures for one classtype</summary>
		public static LanguageForeign[] GetListForType(string classType){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LanguageForeign[]>(MethodBase.GetCurrentMethod(),classType);
			}
			string command=
				"SELECT * FROM languageforeign "
				+"WHERE ClassType='"+POut.String(classType)+"'";
			return Crud.LanguageForeignCrud.SelectMany(command).ToArray();
		}
		
		///<summary>Used in FormTranslation to get a single entry for the specified culture.  The culture match must be extact.  If no translation entries, then it returns null.</summary>
		public static LanguageForeign GetForCulture(LanguageForeign[] listForType,string english,string cultureName){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<listForType.Length;i++){
				if(english!=listForType[i].English){
					continue;
				}
				if(cultureName!=listForType[i].Culture){
					continue;
				}
				return listForType[i];
			}
			return null;
		}

		///<summary>Used in FormTranslation to get a single entry with the same language as the specified culture, but only for a different culture.  For instance, if culture is es-PR (Spanish-PuertoRico), then it will return any spanish translation that is NOT from Puerto Rico.  If no other translation entries, then it returns null.</summary>
		public static LanguageForeign GetOther(LanguageForeign[] listForType,string english,string cultureName){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<listForType.Length;i++){
				if(english!=listForType[i].English){
					continue;
				}
				if(cultureName==listForType[i].Culture){
					continue;
				}
				if(cultureName.Substring(0,2)!=listForType[i].Culture.Substring(0,2)){
					continue;
				}
				return listForType[i];
			}
			return null;
		}
	}

	

	

}













