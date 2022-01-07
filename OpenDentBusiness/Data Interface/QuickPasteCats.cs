using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class QuickPasteCats {
		#region CachePattern

		private class QuickPasteCatCache : CacheListAbs<QuickPasteCat> {
			protected override List<QuickPasteCat> GetCacheFromDb() {
				string command=
					"SELECT * from quickpastecat "
					+"ORDER BY ItemOrder";
				return Crud.QuickPasteCatCrud.SelectMany(command);
			}
			protected override List<QuickPasteCat> TableToList(DataTable table) {
				return Crud.QuickPasteCatCrud.TableToList(table);
			}
			protected override QuickPasteCat Copy(QuickPasteCat quickPasteCat) {
				return quickPasteCat.Copy();
			}
			protected override DataTable ListToTable(List<QuickPasteCat> listQuickPasteCats) {
				return Crud.QuickPasteCatCrud.ListToTable(listQuickPasteCats,"QuickPasteCat");
			}
			protected override void FillCacheIfNeeded() {
				QuickPasteCats.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static QuickPasteCatCache _quickPasteCatCache=new QuickPasteCatCache();

		public static List<QuickPasteCat> GetDeepCopy(bool isShort=false) {
			return _quickPasteCatCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _quickPasteCatCache.GetCount(isShort);
		}

		public static List<QuickPasteCat> GetWhere(Predicate<QuickPasteCat> match,bool isShort=false) {
			return _quickPasteCatCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_quickPasteCatCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_quickPasteCatCache.FillCacheFromTable(table);
				return table;
			}
			return _quickPasteCatCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(QuickPasteCat cat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				cat.QuickPasteCatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cat);
				return cat.QuickPasteCatNum;
			}
			return Crud.QuickPasteCatCrud.Insert(cat);
		}

		///<summary></summary>
		public static void Update(QuickPasteCat cat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cat);
				return;
			}
			Crud.QuickPasteCatCrud.Update(cat);
		}

		///<summary></summary>
		public static void Delete(QuickPasteCat cat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cat);
				return;
			}
			string command="DELETE from quickpastecat WHERE QuickPasteCatNum = '"
				+POut.Long(cat.QuickPasteCatNum)+"'";
 			Db.NonQ(command);
		}

		///<summary>Returns a list of all categories for a single type. Will return an empty list if no type is selected.</summary>
		public static List<QuickPasteCat> GetCategoriesForType(QuickPasteType type) {
			//No need to check RemotingRole; no call to db.
			List<QuickPasteCat> listQuickCats=GetWhere(x => ListTools.In(type,x.ListDefaultForTypes));
			if(listQuickCats.Count==0) {
				QuickPasteCat quickCat=GetDeepCopy().FirstOrDefault();//First category from db by item order
				if(quickCat!=null) {
					listQuickCats.Add(quickCat);
				}
			}
			return listQuickCats;
		}

		///<summary>Called from FormQuickPaste and from QuickPasteNotes.Substitute(). Returns the index of the default category for the specified type. If user has entered more than one, only one is returned.</summary>
		public static int GetDefaultType(QuickPasteType type){
			//No need to check RemotingRole; no call to db.
			if(GetCount()==0) {
				return -1;
			}
			if(type==QuickPasteType.None) {
				return 0;//default to first line
			}
			string[] types;
			List<QuickPasteCat> listQuickPasteCats=GetDeepCopy();
			for(int i=0;i<listQuickPasteCats.Count;i++){
				if(listQuickPasteCats[i].DefaultForTypes==""){
					types=new string[0];
				}
				else{
					types=listQuickPasteCats[i].DefaultForTypes.Split(',');
				}
				for(int j=0;j<types.Length;j++){
					if(((int)type).ToString()==types[j]){
						return i;
					}
				}
			}
			return 0;
		}

		///<summary>This should not be passing in two lists. Consider rewriting to only pass in one list and an identifier to get list from DB.</summary>
		public static bool Sync(List<QuickPasteCat> listNew,List<QuickPasteCat> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.QuickPasteCatCrud.Sync(listNew.Select(x=>x.Copy()).ToList(),listOld.Select(x=>x.Copy()).ToList());
		}
	}

}









