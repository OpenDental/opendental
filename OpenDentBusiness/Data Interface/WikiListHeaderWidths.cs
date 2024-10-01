using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WikiListHeaderWidths{
		///<summary>Used temporarily.</summary>
		public static string DummyColName="Xrxzes";

		#region CachePattern

		private class WikiListHeaderWidthCache : CacheListAbs<WikiListHeaderWidth> {
			protected override List<WikiListHeaderWidth> GetCacheFromDb() {
				string command="SELECT * FROM wikilistheaderwidth";
				return Crud.WikiListHeaderWidthCrud.SelectMany(command);
			}
			protected override List<WikiListHeaderWidth> TableToList(DataTable table) {
				return Crud.WikiListHeaderWidthCrud.TableToList(table);
			}
			protected override WikiListHeaderWidth Copy(WikiListHeaderWidth wikiListHeaderWidth) {
				return wikiListHeaderWidth.Copy();
			}
			protected override DataTable ListToTable(List<WikiListHeaderWidth> listWikiListHeaderWidths) {
				return Crud.WikiListHeaderWidthCrud.ListToTable(listWikiListHeaderWidths,"WikiListHeaderWidth");
			}
			protected override void FillCacheIfNeeded() {
				WikiListHeaderWidths.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static WikiListHeaderWidthCache _wikiListHeaderWidthCache=new WikiListHeaderWidthCache();

		public static List<WikiListHeaderWidth> GetWhere(Predicate<WikiListHeaderWidth> match,bool isShort=false) {
			return _wikiListHeaderWidthCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_wikiListHeaderWidthCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_wikiListHeaderWidthCache.FillCacheFromTable(table);
				return table;
			}
			return _wikiListHeaderWidthCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_wikiListHeaderWidthCache.ClearCache();
		}
		#endregion

		///<summary>Returns header widths for list sorted in the same order as the columns appear in the DB. Can be more efficient than using cache.</summary>
		public static List<WikiListHeaderWidth> GetForListNoCache(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiListHeaderWidth>>(MethodBase.GetCurrentMethod(),listName);
			}
			List<WikiListHeaderWidth> listWikiListHeaderWidthsRet = new List<WikiListHeaderWidth>();
			List<WikiListHeaderWidth> listWikiListHeaderWidthsTemp = new List<WikiListHeaderWidth>();
			string command="DESCRIBE wikilist_"+POut.String(listName);
			DataTable tableDescriptions=Db.GetTable(command);
			command="SELECT * FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"'";
			listWikiListHeaderWidthsTemp=Crud.WikiListHeaderWidthCrud.SelectMany(command);
			for(int i = 0;i<tableDescriptions.Rows.Count;i++) {
				for(int j = 0;j<listWikiListHeaderWidthsTemp.Count;j++) {
					//Add WikiListHeaderWidth from listWikiListHeaderWidthsTemp to listWikiListHeaderWidthsRet if it is the next row in tableDescriptions.
					if(tableDescriptions.Rows[i][0].ToString()==listWikiListHeaderWidthsTemp[j].ColName) {
						listWikiListHeaderWidthsRet.Add(listWikiListHeaderWidthsTemp[j]);
						break;
					}
				}
				//next description row.
			}
			return listWikiListHeaderWidthsRet;
		}

		///<summary>Returns header widths for list sorted in the same order as the columns appear in the DB.  Uses cache.</summary>
		public static List<WikiListHeaderWidth> GetForList(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WikiListHeaderWidth>>(MethodBase.GetCurrentMethod(),listName);
			}
			List<WikiListHeaderWidth> listWikiListHeaderWidthsRet = new List<WikiListHeaderWidth>();
			string command = "DESCRIBE wikilist_"+POut.String(listName);
			DataTable tableDescripts = Db.GetTable(command);//Includes PK, and it's in the correct order.
			List<WikiListHeaderWidth> listWikiListHeaderWidths=GetWhere(x => x.ListName==listName);//also includes PK, but it can be out of order.
			for(int i = 0;i<tableDescripts.Rows.Count;i++) {//known to be in correct order
				WikiListHeaderWidth wikiListHeaderWidth = listWikiListHeaderWidths.Find(x => x.ColName == tableDescripts.Rows[i][0].ToString());
				//if it's null, it will crash. We always allow crashes when there's db corruption.
				listWikiListHeaderWidthsRet.Add(wikiListHeaderWidth);
			}
			return listWikiListHeaderWidthsRet;
		}

		///<summary>Also alters the db table for the list itself.  Throws exception if number of columns does not match.</summary>
		public static void UpdateNamesAndWidths(string listName,List<WikiListHeaderWidth> listWikiListHeaderWidths) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,listWikiListHeaderWidths);
				return;
			}
			string command="DESCRIBE wikilist_"+POut.String(listName);
			DataTable tableListDescriptions=Db.GetTable(command);
			if(tableListDescriptions.Rows.Count!=listWikiListHeaderWidths.Count) {
				throw new ApplicationException("List schema has been altered. Unable to save changes to list.");
			}
			List<string> listChanges=new List<string>();
			//rename Columns with dummy names in case user is renaming a new column with an old name.---------------------------------------------
			for(int i=1;i<tableListDescriptions.Rows.Count;i++) {//start with index 1 to skip PK col
				DataRow dataRow=tableListDescriptions.Rows[i];
				listChanges.Add($"CHANGE {POut.String(dataRow[0].ToString())} {POut.String(DummyColName+i)} TEXT NOT NULL");
				command=$@"UPDATE wikilistheaderwidth SET ColName='{POut.String(DummyColName+i)}'
					WHERE ListName='{POut.String(listName)}'
					AND ColName='{POut.String(dataRow[0].ToString())}'";
				Db.NonQ(command);
			}
			Db.NonQ($"ALTER TABLE wikilist_{POut.String(listName)} {string.Join(",",listChanges)}");
			listChanges=new List<string>();
			//rename columns names and widths-------------------------------------------------------------------------------------------------------
			for(int i=1;i<tableListDescriptions.Rows.Count;i++) {//start with index 1 to skip PK col
				WikiListHeaderWidth wikiListHeaderWidth=listWikiListHeaderWidths[i];
				listChanges.Add($"CHANGE {POut.String(DummyColName+i)} {POut.String(wikiListHeaderWidth.ColName)} TEXT NOT NULL");
				command=$@"UPDATE wikilistheaderwidth
					SET ColName='{POut.String(wikiListHeaderWidth.ColName)}',
						ColWidth='{POut.Int(wikiListHeaderWidth.ColWidth)}',
						PickList='{POut.String(wikiListHeaderWidth.PickList)}',
						IsHidden={POut.Bool(wikiListHeaderWidth.IsHidden)}
					WHERE ListName='{POut.String(listName)}'
					AND ColName='{POut.String(DummyColName+i)}'";
				Db.NonQ(command);
			}
			Db.NonQ($"ALTER TABLE wikilist_{POut.String(listName)} {string.Join(",",listChanges)}");
			//handle width of PK seperately because we do not rename the PK column, ever.
			command=$@"UPDATE wikilistheaderwidth
				SET ColWidth='{POut.Int(listWikiListHeaderWidths[0].ColWidth)}',
					PickList='{POut.String(listWikiListHeaderWidths[0].PickList)}',
					IsHidden={POut.Bool(listWikiListHeaderWidths[0].IsHidden)}
				WHERE ListName='{POut.String(listName)}'
				AND ColName='{POut.String(listWikiListHeaderWidths[0].ColName)}'";
			Db.NonQ(command);
			RefreshCache();
		}

		///<summary>No error checking. Only called from WikiLists.</summary>
		public static void InsertNew(WikiListHeaderWidth wikiListHeaderWidth) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),wikiListHeaderWidth);
				return;
			}
			Crud.WikiListHeaderWidthCrud.Insert(wikiListHeaderWidth);
			RefreshCache();
		}

		public static void InsertMany(List<WikiListHeaderWidth> listWikiListHeaderWidth) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listWikiListHeaderWidth);
				return;
			}
			Crud.WikiListHeaderWidthCrud.InsertMany(listWikiListHeaderWidth);
			RefreshCache();
		}

		///<summary>No error checking. Only called from WikiLists after the corresponding column has been dropped from its respective table.</summary>
		public static void Delete(string listName,string colName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,colName);
				return;
			}
			string command = "DELETE FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"' AND ColName='"+POut.String(colName)+"'";
			Db.NonQ(command);
			RefreshCache();
		}

		public static void DeleteForList(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName);
				return;
			}
			string command = "DELETE FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"'";
			Db.NonQ(command);
			RefreshCache();
		}

		public static List<WikiListHeaderWidth> GetFromListHist(WikiListHist wikiListHist) {
			Meth.NoCheckMiddleTierRole();
			List<WikiListHeaderWidth> listWikiListHeaderWidths=new List<WikiListHeaderWidth>();
			string[] arrayHeaders=wikiListHist.ListHeaders.Split(';');
			for(int i=0;i<arrayHeaders.Length;i++) {
				WikiListHeaderWidth wikiListHeaderWidth=new WikiListHeaderWidth();
				wikiListHeaderWidth.ListName=wikiListHist.ListName;
				wikiListHeaderWidth.ColName=arrayHeaders[i].Split(',')[0];
				wikiListHeaderWidth.ColWidth=PIn.Int(arrayHeaders[i].Split(',')[1]);
				listWikiListHeaderWidths.Add(wikiListHeaderWidth);
			}
			return listWikiListHeaderWidths;
		}
	}
}