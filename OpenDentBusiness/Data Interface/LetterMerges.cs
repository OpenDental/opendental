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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_letterMergeCache.FillCacheFromTable(table);
				return table;
			}
			return _letterMergeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

#if !DISABLE_MICROSOFT_OFFICE
		private static Word.Application wordApp;

		///<summary>This is a static reference to a word application.  That way, we can reuse it instead of having to reopen Word each time.</summary>
		public static Word.Application WordApp {
			//No need to check RemotingRole; no call to db.
			get {
				if(wordApp==null) {
					wordApp=new Word.Application();
					wordApp.Visible=true;
				}
				try {
					wordApp.Activate();
				}
				catch {
					wordApp=new Word.Application();
					wordApp.Visible=true;
					wordApp.Activate();
				}
				return wordApp;
			}
		}
#endif

		///<summary>Inserts this lettermerge into database.</summary>
		public static long Insert(LetterMerge merge) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				merge.LetterMergeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),merge);
				return merge.LetterMergeNum;
			}
			return Crud.LetterMergeCrud.Insert(merge);
		}

		///<summary></summary>
		public static void Update(LetterMerge merge){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),merge);
				return;
			}
			Crud.LetterMergeCrud.Update(merge);
		}

		///<summary></summary>
		public static void Delete(LetterMerge merge){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),merge);
				return;
			}
			string command="DELETE FROM lettermerge "
				+"WHERE LetterMergeNum = "+POut.Long(merge.LetterMergeNum);
			Db.NonQ(command);
		}

		///<summary>Supply the index of the cat within Defs.Short.</summary>
		public static List<LetterMerge> GetListForCat(int catIndex){
			//No need to check RemotingRole; no call to db.
			long defNum=Defs.GetDefsForCategory(DefCat.LetterMergeCats,true)[catIndex].DefNum;
			return GetWhere(x => x.Category==defNum);
		}
	}

	



}









