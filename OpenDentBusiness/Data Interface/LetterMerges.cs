using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LetterMerges {
		#region CachePattern

		private class LetterMergeCache : CacheListAbs<LetterMerge> {
			protected override List<LetterMerge> GetCacheFromDb() {
				string command="SELECT * FROM lettermerge ORDER BY Description";
				List<LetterMerge> listLetterMerges=Crud.LetterMergeCrud.SelectMany(command);
				return listLetterMerges;
			}
			protected override List<LetterMerge> TableToList(DataTable table) {
				return Crud.LetterMergeCrud.TableToList(table);
			}
			protected override LetterMerge Copy(LetterMerge letterMerge) {
				return letterMerge.Copy();
			}
			protected override DataTable ListToTable(List<LetterMerge> listLetterMerges) {
				return Crud.LetterMergeCrud.ListToTable(listLetterMerges,"LetterMerge");
			}
			protected override void FillCacheIfNeeded() {
				LetterMerges.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LetterMergeCache _letterMergeCache=new LetterMergeCache();

		public static List<LetterMerge> GetWhere(Predicate<LetterMerge> match,bool isShort=false) {
			return _letterMergeCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_letterMergeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_letterMergeCache.FillCacheFromTable(table);
				return table;
			}
			return _letterMergeCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_letterMergeCache.ClearCache();
		}
		#endregion

#if !DISABLE_MICROSOFT_OFFICE
		private static Word.Application _word_Application;

		///<summary>This is a static reference to a word application. That way, we can reuse it instead of having to reopen Word each time. Surround with try/catch. Will throw exception if Word is not installed.</summary>
		public static Word.Application GetWordApp() {
			Meth.NoCheckMiddleTierRole();
			if(_word_Application==null) {
				_word_Application=new Word.Application();
				_word_Application.Visible=true;
			}
			try {
				_word_Application.Activate();
			}
			catch {
				_word_Application=new Word.Application();
				_word_Application.Visible=true;
				_word_Application.Activate();
			}
			return _word_Application;
		}
#endif

		///<summary>Inserts this lettermerge into database.</summary>
		public static long Insert(LetterMerge letterMerge) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				letterMerge.LetterMergeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),letterMerge);
				return letterMerge.LetterMergeNum;
			}
			return Crud.LetterMergeCrud.Insert(letterMerge);
		}

		///<summary></summary>
		public static void Update(LetterMerge letterMerge){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),letterMerge);
				return;
			}
			Crud.LetterMergeCrud.Update(letterMerge);
		}

		///<summary></summary>
		public static void Delete(LetterMerge letterMerge){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),letterMerge);
				return;
			}
			string command="DELETE FROM lettermerge "
				+"WHERE LetterMergeNum = "+POut.Long(letterMerge.LetterMergeNum);
			Db.NonQ(command);
		}

		///<summary>Supply the index of the cat within Defs.Short.</summary>
		public static List<LetterMerge> GetListForCat(int catIndex){
			Meth.NoCheckMiddleTierRole();
			long defNum=Defs.GetDefsForCategory(DefCat.LetterMergeCats,true)[catIndex].DefNum;
			return GetWhere(x => x.Category==defNum);
		}
	}

	



}









