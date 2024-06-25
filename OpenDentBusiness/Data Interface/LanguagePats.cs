using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LanguagePats{
		#region Cache Pattern
		private class LanguagePatCache : CacheListAbs<LanguagePat> {
			protected override List<LanguagePat> GetCacheFromDb() {
				string command="SELECT * FROM languagepat";
				return Crud.LanguagePatCrud.SelectMany(command);
			}
			protected override List<LanguagePat> TableToList(DataTable table) {
				return Crud.LanguagePatCrud.TableToList(table);
			}
			protected override LanguagePat Copy(LanguagePat languagePat) {
				return languagePat.Copy();
			}
			protected override DataTable ListToTable(List<LanguagePat> listLanguagePats) {
				return Crud.LanguagePatCrud.ListToTable(listLanguagePats,"LanguagePat");
			}
			protected override void FillCacheIfNeeded() {
				LanguagePats.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LanguagePatCache _languagePatCache=new LanguagePatCache();

		public static void ClearCache() {
			_languagePatCache.ClearCache();
		}

		//public static List<LanguagePat> GetDeepCopy(bool isShort = false) {
		//	return _languagePatCache.GetDeepCopy(isShort);
		//}

		//public static int GetCount(bool isShort=false) {
		//	return _languagePatCache.GetCount(isShort);
		//}

		//public static bool GetExists(Predicate<LanguagePat> match,bool isShort=false) {
		//	return _languagePatCache.GetExists(match,isShort);
		//}

		//public static int GetFindIndex(Predicate<LanguagePat> match,bool isShort=false) {
		//	return _languagePatCache.GetFindIndex(match,isShort);
		//}

		//public static LanguagePat GetFirst(bool isShort=false) {
		//	return _languagePatCache.GetFirst(isShort);
		//}

		//public static LanguagePat GetFirst(Func<LanguagePat,bool> match,bool isShort=false) {
		//	return _languagePatCache.GetFirst(match,isShort);
		//}

		public static LanguagePat GetFirstOrDefault(Func<LanguagePat,bool> match,bool isShort=false) {
			return _languagePatCache.GetFirstOrDefault(match,isShort);
		}

		//public static LanguagePat GetLast(bool isShort=false) {
		//	return _languagePatCache.GetLast(isShort);
		//}

		//public static LanguagePat GetLastOrDefault(Func<LanguagePat,bool> match,bool isShort=false) {
		//	return _languagePatCache.GetLastOrDefault(match,isShort);
		//}

		//public static List<LanguagePat> GetWhere(Predicate<LanguagePat> match,bool isShort=false) {
		//	return _languagePatCache.GetWhere(match,isShort);
		//}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_languagePatCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_languagePatCache.FillCacheFromTable(table);
				return table;
			}
			return _languagePatCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary>Gets one LanguagePat from the db.</summary>
		//public static LanguagePat GetOne(long languagePatNum) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetObject<LanguagePat>(MethodBase.GetCurrentMethod(),languagePatNum);
		//	}
		//	return Crud.LanguagePatCrud.SelectOne(languagePatNum);
		//}

		///<summary>Gets a translation for a preference value from the cache. Returns the pref's value directly if language matches the practice's language/region. Otherwise returns an empty string.</summary>
		public static string GetPrefTranslation(PrefName prefName,string language,bool defaultToBlank=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<string>(MethodBase.GetCurrentMethod(),prefName,language,defaultToBlank);
			}
			LanguagePat languagePat=GetFirstOrDefault(x => x.Language==language && x.PrefName==prefName.ToString());
			if(languagePat==null || string.IsNullOrWhiteSpace(languagePat.Translation)) {
				if(defaultToBlank && language!=PrefC.GetLanguageAndRegion().ThreeLetterISOLanguageName) {
					return "";
				}
				return PrefC.GetString(prefName);//If default language, use pref value.
			}
			//Use translation from LanguagePat if it exists.
			return languagePat.Translation;
		}

		///<summary>Gets a list of all languages corresponding to a list of PrefNames from the db. Does not include default language. Used in OpenDentalWebApps to generate ApptReminderRules for WebSchedRecall.</summary>
		public static List<string> GetListLanguagesForPrefsFromDb(List<string> listPrefNames) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),listPrefNames);
			}
			if(listPrefNames.IsNullOrEmpty()) {
				return new List<string>();
			}
			string command="SELECT Language FROM languagepat "
				+"WHERE PrefName IN ("+string.Join(",",listPrefNames.Select(x=>$"'{POut.String(x)}'"))+") "
				+"GROUP BY language";
			return Db.GetListString(command);
		}

		///<summary>Gets a translation for a preference value from the db.</summary>
		public static LanguagePat GetPrefTranslationFromDb(PrefName prefName,string language) {
			return LanguagePats.GetListPrefTranslationsFromDb(ListTools.FromSingle(prefName.ToString()),ListTools.FromSingle(language)).FirstOrDefault();
		}

		///<summary>Gets a list of translations corresponding to a list of PrefNames and languages. Used in OpenDentalWebApps to check for WebSchedRecall URL tags.</summary>
		public static List<LanguagePat> GetListPrefTranslationsFromDb(List<string> listPrefNames,List<string> listLanguages) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LanguagePat>>(MethodBase.GetCurrentMethod(),listPrefNames,listLanguages);
			}
			if(listPrefNames.IsNullOrEmpty() || listLanguages.IsNullOrEmpty()) {
				return new List<LanguagePat>();
			}
			string command="SELECT * FROM languagepat "
				+"WHERE PrefName IN ("+string.Join(",",listPrefNames.Select(x=>$"'{POut.String(x)}'"))+") "
				+"AND Language IN ("+string.Join(",",listLanguages.Select(x=>$"'{POut.String(x)}'"))+") ";
			return Crud.LanguagePatCrud.SelectMany(command);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(LanguagePat languagePat){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				languagePat.LanguagePatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),languagePat);
				return languagePat.LanguagePatNum;
			}
			return Crud.LanguagePatCrud.Insert(languagePat);
		}

		///<summary></summary>
		public static void Update(LanguagePat languagePat){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),languagePat);
				return;
			}
			Crud.LanguagePatCrud.Update(languagePat);
		}
		///<summary>Updates a preference translation if it exists, otherwise inserts a new one. If language matches the practice's language, the preference value will be updated instead of a LanguagePat.</summary>
		public static void UpsertPrefTranslation(PrefName prefName,string language,string translation){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),prefName,language,translation);
				return;
			}
			if(language==PrefC.GetLanguageAndRegion().ThreeLetterISOLanguageName) {
				Prefs.UpdateString(prefName,translation);//If default language, update pref value directly.
				return;
			}
			LanguagePat languagePatNew=LanguagePats.GetPrefTranslationFromDb(prefName,language);
			if(languagePatNew==null) {
				languagePatNew=new LanguagePat();
				languagePatNew.Language=language;
				languagePatNew.PrefName=prefName.ToString();
				languagePatNew.Translation=translation;
				LanguagePats.Insert(languagePatNew);
				return;
			}
			LanguagePat languagePatOld=languagePatNew.Copy();
			languagePatNew.Translation=translation;
			Crud.LanguagePatCrud.Update(languagePatNew,languagePatOld);
		}
		///<summary></summary>
		//public static void Delete(long languagePatNum) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),languagePatNum);
		//		return;
		//	}
		//	Crud.LanguagePatCrud.Delete(languagePatNum);
		//}
		#endregion Methods - Modify
		#region Methods - Misc
		
		#endregion Methods - Misc



	}
}