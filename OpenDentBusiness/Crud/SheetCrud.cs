//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class SheetCrud {
		///<summary>Gets one Sheet object from the database using the primary key.  Returns null if not found.</summary>
		public static Sheet SelectOne(long sheetNum) {
			string command="SELECT * FROM sheet "
				+"WHERE SheetNum = "+POut.Long(sheetNum);
			List<Sheet> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Sheet object from the database using a query.</summary>
		public static Sheet SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Sheet> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Sheet objects from the database using a query.</summary>
		public static List<Sheet> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Sheet> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Sheet> TableToList(DataTable table) {
			List<Sheet> retVal=new List<Sheet>();
			Sheet sheet;
			foreach(DataRow row in table.Rows) {
				sheet=new Sheet();
				sheet.SheetNum        = PIn.Long  (row["SheetNum"].ToString());
				sheet.SheetType       = (OpenDentBusiness.SheetTypeEnum)PIn.Int(row["SheetType"].ToString());
				sheet.PatNum          = PIn.Long  (row["PatNum"].ToString());
				sheet.DateTimeSheet   = PIn.DateT (row["DateTimeSheet"].ToString());
				sheet.FontSize        = PIn.Float (row["FontSize"].ToString());
				sheet.FontName        = PIn.String(row["FontName"].ToString());
				sheet.Width           = PIn.Int   (row["Width"].ToString());
				sheet.Height          = PIn.Int   (row["Height"].ToString());
				sheet.IsLandscape     = PIn.Bool  (row["IsLandscape"].ToString());
				sheet.InternalNote    = PIn.String(row["InternalNote"].ToString());
				sheet.Description     = PIn.String(row["Description"].ToString());
				sheet.ShowInTerminal  = PIn.Byte  (row["ShowInTerminal"].ToString());
				sheet.IsWebForm       = PIn.Bool  (row["IsWebForm"].ToString());
				sheet.IsMultiPage     = PIn.Bool  (row["IsMultiPage"].ToString());
				sheet.IsDeleted       = PIn.Bool  (row["IsDeleted"].ToString());
				sheet.SheetDefNum     = PIn.Long  (row["SheetDefNum"].ToString());
				sheet.DocNum          = PIn.Long  (row["DocNum"].ToString());
				sheet.ClinicNum       = PIn.Long  (row["ClinicNum"].ToString());
				sheet.DateTSheetEdited= PIn.DateT (row["DateTSheetEdited"].ToString());
				sheet.HasMobileLayout = PIn.Bool  (row["HasMobileLayout"].ToString());
				sheet.RevID           = PIn.Int   (row["RevID"].ToString());
				sheet.WebFormSheetID  = PIn.Long  (row["WebFormSheetID"].ToString());
				retVal.Add(sheet);
			}
			return retVal;
		}

		///<summary>Converts a list of Sheet into a DataTable.</summary>
		public static DataTable ListToTable(List<Sheet> listSheets,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Sheet";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("SheetNum");
			table.Columns.Add("SheetType");
			table.Columns.Add("PatNum");
			table.Columns.Add("DateTimeSheet");
			table.Columns.Add("FontSize");
			table.Columns.Add("FontName");
			table.Columns.Add("Width");
			table.Columns.Add("Height");
			table.Columns.Add("IsLandscape");
			table.Columns.Add("InternalNote");
			table.Columns.Add("Description");
			table.Columns.Add("ShowInTerminal");
			table.Columns.Add("IsWebForm");
			table.Columns.Add("IsMultiPage");
			table.Columns.Add("IsDeleted");
			table.Columns.Add("SheetDefNum");
			table.Columns.Add("DocNum");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("DateTSheetEdited");
			table.Columns.Add("HasMobileLayout");
			table.Columns.Add("RevID");
			table.Columns.Add("WebFormSheetID");
			foreach(Sheet sheet in listSheets) {
				table.Rows.Add(new object[] {
					POut.Long  (sheet.SheetNum),
					POut.Int   ((int)sheet.SheetType),
					POut.Long  (sheet.PatNum),
					POut.DateT (sheet.DateTimeSheet,false),
					POut.Float (sheet.FontSize),
					            sheet.FontName,
					POut.Int   (sheet.Width),
					POut.Int   (sheet.Height),
					POut.Bool  (sheet.IsLandscape),
					            sheet.InternalNote,
					            sheet.Description,
					POut.Byte  (sheet.ShowInTerminal),
					POut.Bool  (sheet.IsWebForm),
					POut.Bool  (sheet.IsMultiPage),
					POut.Bool  (sheet.IsDeleted),
					POut.Long  (sheet.SheetDefNum),
					POut.Long  (sheet.DocNum),
					POut.Long  (sheet.ClinicNum),
					POut.DateT (sheet.DateTSheetEdited,false),
					POut.Bool  (sheet.HasMobileLayout),
					POut.Int   (sheet.RevID),
					POut.Long  (sheet.WebFormSheetID),
				});
			}
			return table;
		}

		///<summary>Inserts one Sheet into the database.  Returns the new priKey.</summary>
		public static long Insert(Sheet sheet) {
			return Insert(sheet,false);
		}

		///<summary>Inserts one Sheet into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Sheet sheet,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				sheet.SheetNum=ReplicationServers.GetKey("sheet","SheetNum");
			}
			string command="INSERT INTO sheet (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="SheetNum,";
			}
			command+="SheetType,PatNum,DateTimeSheet,FontSize,FontName,Width,Height,IsLandscape,InternalNote,Description,ShowInTerminal,IsWebForm,IsMultiPage,IsDeleted,SheetDefNum,DocNum,ClinicNum,DateTSheetEdited,HasMobileLayout,RevID,WebFormSheetID) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(sheet.SheetNum)+",";
			}
			command+=
				     POut.Int   ((int)sheet.SheetType)+","
				+    POut.Long  (sheet.PatNum)+","
				+    POut.DateT (sheet.DateTimeSheet)+","
				+    POut.Float (sheet.FontSize)+","
				+"'"+POut.String(sheet.FontName)+"',"
				+    POut.Int   (sheet.Width)+","
				+    POut.Int   (sheet.Height)+","
				+    POut.Bool  (sheet.IsLandscape)+","
				+    DbHelper.ParamChar+"paramInternalNote,"
				+"'"+POut.String(sheet.Description)+"',"
				+    POut.Byte  (sheet.ShowInTerminal)+","
				+    POut.Bool  (sheet.IsWebForm)+","
				+    POut.Bool  (sheet.IsMultiPage)+","
				+    POut.Bool  (sheet.IsDeleted)+","
				+    POut.Long  (sheet.SheetDefNum)+","
				+    POut.Long  (sheet.DocNum)+","
				+    POut.Long  (sheet.ClinicNum)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (sheet.HasMobileLayout)+","
				+    POut.Int   (sheet.RevID)+","
				+    POut.Long  (sheet.WebFormSheetID)+")";
			if(sheet.InternalNote==null) {
				sheet.InternalNote="";
			}
			OdSqlParameter paramInternalNote=new OdSqlParameter("paramInternalNote",OdDbType.Text,POut.StringParam(sheet.InternalNote));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramInternalNote);
			}
			else {
				sheet.SheetNum=Db.NonQ(command,true,"SheetNum","sheet",paramInternalNote);
			}
			return sheet.SheetNum;
		}

		///<summary>Inserts one Sheet into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Sheet sheet) {
			return InsertNoCache(sheet,false);
		}

		///<summary>Inserts one Sheet into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Sheet sheet,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO sheet (";
			if(!useExistingPK && isRandomKeys) {
				sheet.SheetNum=ReplicationServers.GetKeyNoCache("sheet","SheetNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="SheetNum,";
			}
			command+="SheetType,PatNum,DateTimeSheet,FontSize,FontName,Width,Height,IsLandscape,InternalNote,Description,ShowInTerminal,IsWebForm,IsMultiPage,IsDeleted,SheetDefNum,DocNum,ClinicNum,DateTSheetEdited,HasMobileLayout,RevID,WebFormSheetID) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(sheet.SheetNum)+",";
			}
			command+=
				     POut.Int   ((int)sheet.SheetType)+","
				+    POut.Long  (sheet.PatNum)+","
				+    POut.DateT (sheet.DateTimeSheet)+","
				+    POut.Float (sheet.FontSize)+","
				+"'"+POut.String(sheet.FontName)+"',"
				+    POut.Int   (sheet.Width)+","
				+    POut.Int   (sheet.Height)+","
				+    POut.Bool  (sheet.IsLandscape)+","
				+    DbHelper.ParamChar+"paramInternalNote,"
				+"'"+POut.String(sheet.Description)+"',"
				+    POut.Byte  (sheet.ShowInTerminal)+","
				+    POut.Bool  (sheet.IsWebForm)+","
				+    POut.Bool  (sheet.IsMultiPage)+","
				+    POut.Bool  (sheet.IsDeleted)+","
				+    POut.Long  (sheet.SheetDefNum)+","
				+    POut.Long  (sheet.DocNum)+","
				+    POut.Long  (sheet.ClinicNum)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (sheet.HasMobileLayout)+","
				+    POut.Int   (sheet.RevID)+","
				+    POut.Long  (sheet.WebFormSheetID)+")";
			if(sheet.InternalNote==null) {
				sheet.InternalNote="";
			}
			OdSqlParameter paramInternalNote=new OdSqlParameter("paramInternalNote",OdDbType.Text,POut.StringParam(sheet.InternalNote));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramInternalNote);
			}
			else {
				sheet.SheetNum=Db.NonQ(command,true,"SheetNum","sheet",paramInternalNote);
			}
			return sheet.SheetNum;
		}

		///<summary>Updates one Sheet in the database.</summary>
		public static void Update(Sheet sheet) {
			string command="UPDATE sheet SET "
				+"SheetType       =  "+POut.Int   ((int)sheet.SheetType)+", "
				+"PatNum          =  "+POut.Long  (sheet.PatNum)+", "
				+"DateTimeSheet   =  "+POut.DateT (sheet.DateTimeSheet)+", "
				+"FontSize        =  "+POut.Float (sheet.FontSize)+", "
				+"FontName        = '"+POut.String(sheet.FontName)+"', "
				+"Width           =  "+POut.Int   (sheet.Width)+", "
				+"Height          =  "+POut.Int   (sheet.Height)+", "
				+"IsLandscape     =  "+POut.Bool  (sheet.IsLandscape)+", "
				+"InternalNote    =  "+DbHelper.ParamChar+"paramInternalNote, "
				+"Description     = '"+POut.String(sheet.Description)+"', "
				+"ShowInTerminal  =  "+POut.Byte  (sheet.ShowInTerminal)+", "
				+"IsWebForm       =  "+POut.Bool  (sheet.IsWebForm)+", "
				+"IsMultiPage     =  "+POut.Bool  (sheet.IsMultiPage)+", "
				+"IsDeleted       =  "+POut.Bool  (sheet.IsDeleted)+", "
				+"SheetDefNum     =  "+POut.Long  (sheet.SheetDefNum)+", "
				+"DocNum          =  "+POut.Long  (sheet.DocNum)+", "
				+"ClinicNum       =  "+POut.Long  (sheet.ClinicNum)+", "
				+"DateTSheetEdited=  "+POut.DateT (sheet.DateTSheetEdited)+", "
				+"HasMobileLayout =  "+POut.Bool  (sheet.HasMobileLayout)+", "
				+"RevID           =  "+POut.Int   (sheet.RevID)+", "
				+"WebFormSheetID  =  "+POut.Long  (sheet.WebFormSheetID)+" "
				+"WHERE SheetNum = "+POut.Long(sheet.SheetNum);
			if(sheet.InternalNote==null) {
				sheet.InternalNote="";
			}
			OdSqlParameter paramInternalNote=new OdSqlParameter("paramInternalNote",OdDbType.Text,POut.StringParam(sheet.InternalNote));
			Db.NonQ(command,paramInternalNote);
		}

		///<summary>Updates one Sheet in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Sheet sheet,Sheet oldSheet) {
			string command="";
			if(sheet.SheetType != oldSheet.SheetType) {
				if(command!="") { command+=",";}
				command+="SheetType = "+POut.Int   ((int)sheet.SheetType)+"";
			}
			if(sheet.PatNum != oldSheet.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(sheet.PatNum)+"";
			}
			if(sheet.DateTimeSheet != oldSheet.DateTimeSheet) {
				if(command!="") { command+=",";}
				command+="DateTimeSheet = "+POut.DateT(sheet.DateTimeSheet)+"";
			}
			if(sheet.FontSize != oldSheet.FontSize) {
				if(command!="") { command+=",";}
				command+="FontSize = "+POut.Float(sheet.FontSize)+"";
			}
			if(sheet.FontName != oldSheet.FontName) {
				if(command!="") { command+=",";}
				command+="FontName = '"+POut.String(sheet.FontName)+"'";
			}
			if(sheet.Width != oldSheet.Width) {
				if(command!="") { command+=",";}
				command+="Width = "+POut.Int(sheet.Width)+"";
			}
			if(sheet.Height != oldSheet.Height) {
				if(command!="") { command+=",";}
				command+="Height = "+POut.Int(sheet.Height)+"";
			}
			if(sheet.IsLandscape != oldSheet.IsLandscape) {
				if(command!="") { command+=",";}
				command+="IsLandscape = "+POut.Bool(sheet.IsLandscape)+"";
			}
			if(sheet.InternalNote != oldSheet.InternalNote) {
				if(command!="") { command+=",";}
				command+="InternalNote = "+DbHelper.ParamChar+"paramInternalNote";
			}
			if(sheet.Description != oldSheet.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(sheet.Description)+"'";
			}
			if(sheet.ShowInTerminal != oldSheet.ShowInTerminal) {
				if(command!="") { command+=",";}
				command+="ShowInTerminal = "+POut.Byte(sheet.ShowInTerminal)+"";
			}
			if(sheet.IsWebForm != oldSheet.IsWebForm) {
				if(command!="") { command+=",";}
				command+="IsWebForm = "+POut.Bool(sheet.IsWebForm)+"";
			}
			if(sheet.IsMultiPage != oldSheet.IsMultiPage) {
				if(command!="") { command+=",";}
				command+="IsMultiPage = "+POut.Bool(sheet.IsMultiPage)+"";
			}
			if(sheet.IsDeleted != oldSheet.IsDeleted) {
				if(command!="") { command+=",";}
				command+="IsDeleted = "+POut.Bool(sheet.IsDeleted)+"";
			}
			if(sheet.SheetDefNum != oldSheet.SheetDefNum) {
				if(command!="") { command+=",";}
				command+="SheetDefNum = "+POut.Long(sheet.SheetDefNum)+"";
			}
			if(sheet.DocNum != oldSheet.DocNum) {
				if(command!="") { command+=",";}
				command+="DocNum = "+POut.Long(sheet.DocNum)+"";
			}
			if(sheet.ClinicNum != oldSheet.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(sheet.ClinicNum)+"";
			}
			if(sheet.DateTSheetEdited != oldSheet.DateTSheetEdited) {
				if(command!="") { command+=",";}
				command+="DateTSheetEdited = "+POut.DateT(sheet.DateTSheetEdited)+"";
			}
			if(sheet.HasMobileLayout != oldSheet.HasMobileLayout) {
				if(command!="") { command+=",";}
				command+="HasMobileLayout = "+POut.Bool(sheet.HasMobileLayout)+"";
			}
			if(sheet.RevID != oldSheet.RevID) {
				if(command!="") { command+=",";}
				command+="RevID = "+POut.Int(sheet.RevID)+"";
			}
			if(sheet.WebFormSheetID != oldSheet.WebFormSheetID) {
				if(command!="") { command+=",";}
				command+="WebFormSheetID = "+POut.Long(sheet.WebFormSheetID)+"";
			}
			if(command=="") {
				return false;
			}
			if(sheet.InternalNote==null) {
				sheet.InternalNote="";
			}
			OdSqlParameter paramInternalNote=new OdSqlParameter("paramInternalNote",OdDbType.Text,POut.StringParam(sheet.InternalNote));
			command="UPDATE sheet SET "+command
				+" WHERE SheetNum = "+POut.Long(sheet.SheetNum);
			Db.NonQ(command,paramInternalNote);
			return true;
		}

		///<summary>Returns true if Update(Sheet,Sheet) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Sheet sheet,Sheet oldSheet) {
			if(sheet.SheetType != oldSheet.SheetType) {
				return true;
			}
			if(sheet.PatNum != oldSheet.PatNum) {
				return true;
			}
			if(sheet.DateTimeSheet != oldSheet.DateTimeSheet) {
				return true;
			}
			if(sheet.FontSize != oldSheet.FontSize) {
				return true;
			}
			if(sheet.FontName != oldSheet.FontName) {
				return true;
			}
			if(sheet.Width != oldSheet.Width) {
				return true;
			}
			if(sheet.Height != oldSheet.Height) {
				return true;
			}
			if(sheet.IsLandscape != oldSheet.IsLandscape) {
				return true;
			}
			if(sheet.InternalNote != oldSheet.InternalNote) {
				return true;
			}
			if(sheet.Description != oldSheet.Description) {
				return true;
			}
			if(sheet.ShowInTerminal != oldSheet.ShowInTerminal) {
				return true;
			}
			if(sheet.IsWebForm != oldSheet.IsWebForm) {
				return true;
			}
			if(sheet.IsMultiPage != oldSheet.IsMultiPage) {
				return true;
			}
			if(sheet.IsDeleted != oldSheet.IsDeleted) {
				return true;
			}
			if(sheet.SheetDefNum != oldSheet.SheetDefNum) {
				return true;
			}
			if(sheet.DocNum != oldSheet.DocNum) {
				return true;
			}
			if(sheet.ClinicNum != oldSheet.ClinicNum) {
				return true;
			}
			if(sheet.DateTSheetEdited != oldSheet.DateTSheetEdited) {
				return true;
			}
			if(sheet.HasMobileLayout != oldSheet.HasMobileLayout) {
				return true;
			}
			if(sheet.RevID != oldSheet.RevID) {
				return true;
			}
			if(sheet.WebFormSheetID != oldSheet.WebFormSheetID) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Sheet from the database.</summary>
		public static void Delete(long sheetNum) {
			string command="DELETE FROM sheet "
				+"WHERE SheetNum = "+POut.Long(sheetNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Sheets from the database.</summary>
		public static void DeleteMany(List<long> listSheetNums) {
			if(listSheetNums==null || listSheetNums.Count==0) {
				return;
			}
			string command="DELETE FROM sheet "
				+"WHERE SheetNum IN("+string.Join(",",listSheetNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}