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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_wikiListHeaderWidthCache.FillCacheFromTable(table);
				return table;
			}
			return _wikiListHeaderWidthCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		/*///<summary>Returns header widths for list sorted in the same order as the columns appear in the DB. Can be more efficient than using cache.</summary>
		public static List<WikiListHeaderWidth> GetForListNoCache(string listName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiListHeaderWidth>>(MethodBase.GetCurrentMethod(),listName);
			}
			List<WikiListHeaderWidth> retVal = new List<WikiListHeaderWidth>();
			List<WikiListHeaderWidth> tempList = new List<WikiListHeaderWidth>();
			string command="DESCRIBE wikilist_"+POut.String(listName);
			DataTable listDescription=Db.GetTable(command);
			command="SELECT * FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"'";
			tempList=Crud.WikiListHeaderWidthCrud.SelectMany(command);
			for(int i=0;i<listDescription.Rows.Count;i++) {
				for(int j=0;j<tempList.Count;j++) {
					//Add WikiListHeaderWidth from tempList to retVal if it is the next row in listDescription.
					if(listDescription.Rows[i][0].ToString()==tempList[j].ColName) {
						retVal.Add(tempList[j]);
						break;
					}
				}
				//next description row.
			}
			return retVal;
		}*/

		///<summary>Returns header widths for list sorted in the same order as the columns appear in the DB.  Uses cache.</summary>
		public static List<WikiListHeaderWidth> GetForList(string listName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WikiListHeaderWidth>>(MethodBase.GetCurrentMethod(),listName);
			}
			List<WikiListHeaderWidth> retVal = new List<WikiListHeaderWidth>();
			string command = "DESCRIBE wikilist_"+POut.String(listName);//TODO: Oracle compatible?
			DataTable tableDescripts = Db.GetTable(command);
			List<WikiListHeaderWidth> listHeaderWidths=GetWhere(x => x.ListName==listName);
			for(int i = 0;i<tableDescripts.Rows.Count;i++) {
				WikiListHeaderWidth addWidth = listHeaderWidths.Where(x => x.ColName == tableDescripts.Rows[i][0].ToString()).FirstOrDefault();
				if(addWidth!=null) {
					retVal.Add(addWidth);
				}
			}
			return retVal;
		}

		///<summary>Also alters the db table for the list itself.  Throws exception if number of columns does not match.</summary>
		public static void UpdateNamesAndWidths(string listName,List<WikiListHeaderWidth> columnDefs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,columnDefs);
				return;
			}
			string command="DESCRIBE wikilist_"+POut.String(listName);
			DataTable tableListDescription=Db.GetTable(command);
			if(tableListDescription.Rows.Count!=columnDefs.Count) {
				throw new ApplicationException("List schema has been altered. Unable to save changes to list.");
			}
			List<string> listChanges=new List<string>();
			//rename Columns with dummy names in case user is renaming a new column with an old name.---------------------------------------------
			for(int i=1;i<tableListDescription.Rows.Count;i++) {//start with index 1 to skip PK col
				DataRow row=tableListDescription.Rows[i];
				listChanges.Add($"CHANGE {POut.String(row[0].ToString())} {POut.String(DummyColName+i)} TEXT NOT NULL");
				command=$@"UPDATE wikilistheaderwidth SET ColName='{POut.String(DummyColName+i)}'
					WHERE ListName='{POut.String(listName)}'
					AND ColName='{POut.String(row[0].ToString())}'";
				Db.NonQ(command);
			}
			Db.NonQ($"ALTER TABLE wikilist_{POut.String(listName)} {string.Join(",",listChanges)}");
			listChanges=new List<string>();
			//rename columns names and widths-------------------------------------------------------------------------------------------------------
			for(int i=1;i<tableListDescription.Rows.Count;i++) {//start with index 1 to skip PK col
				WikiListHeaderWidth wCur=columnDefs[i];
				listChanges.Add($"CHANGE {POut.String(DummyColName+i)} {POut.String(wCur.ColName)} TEXT NOT NULL");
				command=$@"UPDATE wikilistheaderwidth
					SET ColName='{POut.String(wCur.ColName)}',
						ColWidth='{POut.Int(wCur.ColWidth)}',
						PickList='{POut.String(wCur.PickList)}',
						IsHidden={POut.Bool(wCur.IsHidden)}
					WHERE ListName='{POut.String(listName)}'
					AND ColName='{POut.String(DummyColName+i)}'";
				Db.NonQ(command);
			}
			Db.NonQ($"ALTER TABLE wikilist_{POut.String(listName)} {string.Join(",",listChanges)}");
			//handle width of PK seperately because we do not rename the PK column, ever.
			command=$@"UPDATE wikilistheaderwidth
				SET ColWidth='{POut.Int(columnDefs[0].ColWidth)}',
					PickList='{POut.String(columnDefs[0].PickList)}',
					IsHidden={POut.Bool(columnDefs[0].IsHidden)}
				WHERE ListName='{POut.String(listName)}'
				AND ColName='{POut.String(columnDefs[0].ColName)}'";
			Db.NonQ(command);
			RefreshCache();
		}

		///<summary>No error checking. Only called from WikiLists.</summary>
		public static void InsertNew(WikiListHeaderWidth newWidth) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),newWidth);
				return;
			}
			Crud.WikiListHeaderWidthCrud.Insert(newWidth);
			RefreshCache();
		}

		public static void InsertMany(List<WikiListHeaderWidth> listNewWidths) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNewWidths);
				return;
			}
			Crud.WikiListHeaderWidthCrud.InsertMany(listNewWidths);
			RefreshCache();
		}

		///<summary>No error checking. Only called from WikiLists after the corresponding column has been dropped from its respective table.</summary>
		public static void Delete(string listName,string colName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,colName);
				return;
			}
			string command = "DELETE FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"' AND ColName='"+POut.String(colName)+"'";
			Db.NonQ(command);
			RefreshCache();
		}

		public static void DeleteForList(string listName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName);
				return;
			}
			string command = "DELETE FROM wikilistheaderwidth WHERE ListName='"+POut.String(listName)+"'";
			Db.NonQ(command);
			RefreshCache();
		}

		public static List<WikiListHeaderWidth> GetFromListHist(WikiListHist wikiListHist) {
			List<WikiListHeaderWidth> retVal=new List<WikiListHeaderWidth>();
			string[] arrayHeaders=wikiListHist.ListHeaders.Split(';');
			for(int i=0;i<arrayHeaders.Length;i++) {
				WikiListHeaderWidth hw=new WikiListHeaderWidth();
				hw.ListName=wikiListHist.ListName;
				hw.ColName=arrayHeaders[i].Split(',')[0];
				hw.ColWidth=PIn.Int(arrayHeaders[i].Split(',')[1]);
				retVal.Add(hw);
			}
			return retVal;
		}
	}
}