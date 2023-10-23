using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WikiLists{				
		/*We will probably add this back in later. (TranslateToHTML)
		/// <summary>Returns an html formatted table generated from a "SELECT * FROM wikiList_&lt;tablename&gt;" query.</summary>
		public static string TranslateToHTML(string tableName) {
			string command = "SELECT * FROM wikiList_"+tableName;//TODO: userOD table used just for testing purposes.
			DataTable Table = Db.GetTable(command);
			StringBuilder TableBuilder = new StringBuilder();
			TableBuilder.AppendLine("\t<table>");
					TableBuilder.AppendLine("\t\t<tr>");
					TableBuilder.AppendLine("\t\t\t<td align=\"center\" colspan=\""+Table.Columns.Count+"\">");
					TableBuilder.AppendLine("\t\t\t<h3>List : "+tableName+"</h3>");
					TableBuilder.AppendLine("\t\t\t</td>");
					TableBuilder.AppendLine("\t\t</tr>");
				TableBuilder.AppendLine("\t\t<tr>");
				foreach(DataColumn col in Table.Columns){
					TableBuilder.AppendLine("\t\t\t<th>");
					TableBuilder.AppendLine("\t\t\t\t<b>"+col.ColumnName+"</b>");
					TableBuilder.AppendLine("\t\t\t</th>");
				}
				TableBuilder.AppendLine("\t\t</tr>");
			//TODO: table headers
			foreach(DataRow row in Table.Rows) {
				TableBuilder.AppendLine("\t\t<tr>");
				foreach(object cell in row.ItemArray) {
					TableBuilder.AppendLine("\t\t\t<td>");
					TableBuilder.AppendLine("\t\t\t\t"+cell.ToString());
					TableBuilder.AppendLine("\t\t\t</td>");
				}
				TableBuilder.AppendLine("\t\t</tr>");
			}
			TableBuilder.AppendLine("\t</table>");
			return TableBuilder.ToString();
		}*/

		public static bool CheckExists(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listName);
			}
			string command = "SHOW TABLES LIKE 'wikilist\\_"+POut.String(listName)+"'";
			if(Db.GetTable(command).Rows.Count==1) {
				//found exacty one table with that name
				return true;
			}
			//no table found with that name
			return false;
		}

		public static DataTable GetByName(string listName) {
			return GetByName(listName,"");
		}

		public static DataTable GetByName(string listName, string orderBy) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listName, orderBy);
			}
			string command="SELECT * FROM wikilist_"+POut.String(listName);
			if(!string.IsNullOrEmpty(orderBy)) {
				command+=" ORDER BY "+POut.String(orderBy);//Manual ovverride of Order By
			}
			else using(DataTable tableDescript=Db.GetTable("DESCRIBE wikilist_"+POut.String(listName))) {
				if(tableDescript.Rows.Count==1) {
					command+=" ORDER BY "+tableDescript.Rows[0]["Field"];//order by PK
				}
				else if(tableDescript.Rows.Count>1) {
					command+=" ORDER BY "+tableDescript.Rows[1]["Field"];//order by the second column, even though we show the primary key
				}
			}
			return Db.GetTable(command);
		}

		/// <summary>Creates empty table with a column for PK and optionally the columns in listHeaders. List name must be formatted correctly before
		/// being passed here, i.e. no spaces, all lowercase.  If listHeaders is not null or empty, they will be inserted into the db.
		/// If dropTableIfExists==true, the table with name wikilist_listName will be dropped if it exists and any wikilistheaderwidth rows for the table
		/// will be deleted before creating the new table and inserting any new wikilistheaderwidth rows.</summary>
		public static void CreateNewWikiList(string listName,List<WikiListHeaderWidth> listWikiListHeaderWidths=null,bool dropTable=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,listWikiListHeaderWidths,dropTable);
				return;
			}
			List<string> listStringColDefs=new List<string>();
			if(listWikiListHeaderWidths.IsNullOrEmpty()) {
				WikiListHeaderWidth wikiListHeaderWidth=new WikiListHeaderWidth();
				wikiListHeaderWidth.ListName=listName;
				wikiListHeaderWidth.ColName=listName+"Num";
				wikiListHeaderWidth.ColWidth=100;
				listWikiListHeaderWidths=new List<WikiListHeaderWidth>() { wikiListHeaderWidth };
			}
			listStringColDefs.Add($"{POut.String(listWikiListHeaderWidths[0].ColName)} bigint NOT NULL auto_increment PRIMARY KEY");//listHeaders guaranteed to not be null or empty
			listStringColDefs.AddRange(listWikiListHeaderWidths.Skip(1).Select(x => $"{POut.String(x.ColName)} TEXT NOT NULL"));//first in listHeaders added as PK already
			string command="";
			if(dropTable) {
				command+=$"DROP TABLE IF EXISTS wikilist_{POut.String(listName)}; ";
				WikiListHeaderWidths.DeleteForList(listName);
			}
			command+=$@"CREATE TABLE wikilist_{POut.String(listName)} (
					{string.Join(@",
					",listStringColDefs)}
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			WikiListHeaderWidths.InsertMany(listWikiListHeaderWidths);
		}

		///<summary>Column is automatically named "Column#" where # is the number of columns+1.</summary>
		public static void AddColumn(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName);
				return;
			}
			//Find Valid column name-----------------------------------------------------------------------------------------
			DataTable tableColumnNames = Db.GetTable("DESCRIBE wikilist_"+POut.String(listName));
			string newColumnName="Column1";//default in case table has no columns. Should never happen.
			for(int i=0;i<tableColumnNames.Rows.Count+1;i++) {//+1 to guarantee we can find a valid name.
				newColumnName="Column"+(1+i);//ie. Column1, Column2, Column3...
				for(int j=0;j<tableColumnNames.Rows.Count;j++) {
					if(newColumnName==tableColumnNames.Rows[j]["Field"].ToString()) {
						newColumnName="";
						break;
					}
				}
				if(newColumnName!="") {
					break;//found a valid name.
				}
			}
			if(newColumnName=="") {
				//should never happen.
				throw new ApplicationException("Could not create valid column name.");
			}
			//Add new column name--------------------------------------------------------------------------------------------
			string command = "ALTER TABLE wikilist_"+POut.String(listName)+" ADD COLUMN "+POut.String(newColumnName)+" TEXT NOT NULL";
			Db.NonQ(command);
			//Add column widths to wikiListHeaderWidth Table-----------------------------------------------------------------
			WikiListHeaderWidth wikiListHeaderWidth = new WikiListHeaderWidth();
			wikiListHeaderWidth.ColName=newColumnName;
			wikiListHeaderWidth.ListName=listName;
			wikiListHeaderWidth.ColWidth=100;
			WikiListHeaderWidths.InsertNew(wikiListHeaderWidth);
		}

		///<summary>Check to see if column can be deleted, returns true is the column contains only nulls.</summary>
		public static bool CheckColumnEmpty(string listName,string colName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listName,colName);
			}
			string command = "SELECT COUNT(*) FROM wikilist_"+POut.String(listName)+" WHERE "+POut.String(colName)+"!=''";
			return Db.GetCount(command).Equals("0");
		}

		///<summary>Check to see if column can be deleted, returns true is the column contains only nulls.</summary>
		public static void DeleteColumn(string listName,string colName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,colName);
				return;
			}
			string command = "ALTER TABLE wikilist_"+POut.String(listName)+" DROP "+POut.String(colName);
			Db.NonQ(command);
			WikiListHeaderWidths.Delete(listName,colName);
		}

		/// <summary>Shifts the column to the left, does nothing if trying to shift leftmost two columns.</summary>
		public static void ShiftColumnLeft(string listName,string colName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,colName);
				return;
			}
			DataTable tableColumnNames=Db.GetTable("DESCRIBE wikilist_"+POut.String(listName));
			if(tableColumnNames.Rows.Count<3) {
				return;//not enough columns to reorder.
			}
			int index=tableColumnNames.Select().ToList().FindIndex(x => x[0].ToString()==colName);
			if(index>1 && index<tableColumnNames.Rows.Count) {
				string command=$@"ALTER TABLE wikilist_{POut.String(listName)}
					MODIFY {POut.String(colName)} TEXT NOT NULL AFTER {POut.String(tableColumnNames.Rows[index-2][0].ToString())}";
				Db.NonQ(command);
			}
		}

		/// <summary>Shifts the column to the right, does nothing if trying to shift the rightmost column.</summary>
		public static void ShiftColumnRight(string listName,string colName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,colName);
				return;
			}
			DataTable tableColumnNames=Db.GetTable("DESCRIBE wikilist_"+POut.String(listName));
			if(tableColumnNames.Rows.Count<3) {
				return;//not enough columns to reorder.
			}
			int index=tableColumnNames.Select().ToList().FindIndex(x => x[0].ToString()==colName);
			if(index>0 && index<tableColumnNames.Rows.Count-1) {
				string command=$@"ALTER TABLE wikilist_{POut.String(listName)}
					MODIFY {POut.String(colName)} TEXT NOT NULL AFTER {POut.String(tableColumnNames.Rows[index+1][0].ToString())}";
				Db.NonQ(command);
			}
		}

		///<summary>Adds one item to wiki list and returns the new PK.</summary>
		public static long AddItem(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),listName);
			}
			string command = "INSERT INTO wikilist_"+POut.String(listName)+" VALUES ()";//inserts empty row with auto generated PK.
			return Db.NonQ(command,true);
		}

		/// <summary></summary>
		/// <param name="tableItems">Should be a DataTable object with a single DataRow containing the item.</param>
		public static void UpdateItem(string listName,DataTable tableItems) {
			if(tableItems.Columns.Count<2) {
				//if the table contains only a PK column.
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,tableItems);
				return;
			}
			List<string> listStringRowSets=tableItems.Columns.OfType<DataColumn>().Skip(1)//skip 1 because we do not need to update the PK
				.Select(x => POut.String(x.ColumnName)+$"='{POut.String(tableItems.Rows[0][x].ToString())}'").ToList();
			string command=$@"UPDATE wikilist_{POut.String(listName)} SET {string.Join(@",
				",listStringRowSets)}
				WHERE {POut.String(tableItems.Columns[0].ColumnName)}={POut.Long(PIn.Long(tableItems.Rows[0][0].ToString()))}";
			Db.NonQ(command);
		}

		public static DataTable GetItem(string listName,long itemNum,string colName=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listName,itemNum,colName);
			}
			colName=POut.String(string.IsNullOrEmpty(colName)?(listName+"Num"):colName);
			string command=$"SELECT * FROM wikilist_{POut.String(listName)} WHERE {colName}={POut.Long(itemNum)}";
			return Db.GetTable(command);
		}

		public static void DeleteItem(string listName,long itemNum,string colName=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName,itemNum,colName);
				return;
			}
			colName=POut.String(string.IsNullOrEmpty(colName)?(listName+"Num"):colName);
			string command=$@"DELETE FROM wikilist_{POut.String(listName)} WHERE {colName}={POut.Long(itemNum)}";
			Db.NonQ(command);
		}

		public static void DeleteList(string listName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listName);
				return;
			}
			string command = "DROP TABLE wikilist_"+POut.String(listName);
			Db.NonQ(command);
			WikiListHeaderWidths.DeleteForList(listName);
		}

		public static List<string> GetAllLists() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> listStringWikiList = new List<string>();
			string command = "SHOW TABLES LIKE 'wikilist\\_%'";//must escape _ (underscore) otherwise it is interpreted as a wildcard character.
			DataTable table = Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				listStringWikiList.Add(table.Rows[i][0].ToString());
			}
			return listStringWikiList;
		}

		///<summary><para>Surround with try catch.  Safely renames list by creating new list, selecting existing list into new list, then deleting existing list.</para>
		///<para>This code could be used to either copy or backup lists in the future. (With minor modifications).</para></summary>
		public static void Rename(string nameOriginal,string nameNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),nameOriginal,nameNew);
				return;
			}
			//Name should already have been validated and available.
			string command="CREATE TABLE wikilist_"+POut.String(nameNew)+" AS SELECT * FROM wikilist_"+POut.String(nameOriginal);
			Db.NonQ(command);
			//Validate content before altering and deleting things
			DataTable tableNewNames=GetByName(nameNew);
			DataTable tableOldNames=GetByName(nameOriginal);
			if(tableNewNames.Rows.Count!=tableOldNames.Rows.Count) {
				command="DROP TABLE wikilist_"+POut.String(nameNew);
				Db.NonQ(command);
				throw new Exception("Error occurred renaming list.  Mismatch found in row count. No changes made.");
			}
			if(tableNewNames.Columns.Count!=tableOldNames.Columns.Count) {
				command="DROP TABLE wikilist_"+POut.String(nameNew);
				Db.NonQ(command);
				throw new Exception("Error occurred renaming list.  Mismatch found in column count. No changes made.");
			}
			for(int r1=0;r1<tableNewNames.Rows.Count;r1++) {
				for(int r2=0;r2<tableOldNames.Rows.Count;r2++) {
					if(tableNewNames.Rows[r1][0]!=tableOldNames.Rows[r2][0]) {
						continue;//pk does not match
					}
					for(int c=0;c<tableNewNames.Columns.Count;c++) {//both lists have same number of columns
						if(tableNewNames.Rows[r1][c]==tableOldNames.Rows[r2][c]) {
							continue;//contents match
						}
						throw new Exception("Error occurred renaming list.  Mismatch Error found in row data. No changes made.");
					}//end columns
				}//end tableOld
			}//end tableNew
			//Alter table names----------------------------------------------------------------------------
			string priKeyColNameOrig=POut.String(nameOriginal)+"Num";
			if(!tableNewNames.Columns.Contains(priKeyColNameOrig)) {//if new table doesn't contain a PK based on the old table name, make the first column the nameNew+"Num" PK column
				priKeyColNameOrig=POut.String(tableNewNames.Columns[0].ColumnName);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE wikilist_"+POut.String(nameNew)+" CHANGE "+priKeyColNameOrig+" "+POut.String(nameNew)+"Num bigint NOT NULL auto_increment PRIMARY KEY";
			}
			else {
				command="RENAME COLUMN wikilist_"+POut.String(nameNew)+"."+priKeyColNameOrig+" TO "+POut.String(nameNew)+"Num"; 
			}
			Db.NonQ(command);
			command="UPDATE wikilistheaderwidth SET ListName='"+POut.String(nameNew)+"' WHERE ListName='"+POut.String(nameOriginal)+"'";
			Db.NonQ(command);
			command=$@"UPDATE wikilistheaderwidth SET ColName='{POut.String(nameNew)}Num'
				WHERE ListName='{POut.String(nameNew)}' AND ColName='{priKeyColNameOrig}'";
			Db.NonQ(command);
			//drop old table---------------------
			command="DROP TABLE wikilist_"+POut.String(nameOriginal);
			Db.NonQ(command);
			WikiListHeaderWidths.RefreshCache();
		}
	}
}