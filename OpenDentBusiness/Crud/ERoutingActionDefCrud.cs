//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class ERoutingActionDefCrud {
		///<summary>Gets one ERoutingActionDef object from the database using the primary key.  Returns null if not found.</summary>
		public static ERoutingActionDef SelectOne(long eRoutingActionDefNum) {
			string command="SELECT * FROM eroutingactiondef "
				+"WHERE ERoutingActionDefNum = "+POut.Long(eRoutingActionDefNum);
			List<ERoutingActionDef> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one ERoutingActionDef object from the database using a query.</summary>
		public static ERoutingActionDef SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ERoutingActionDef> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of ERoutingActionDef objects from the database using a query.</summary>
		public static List<ERoutingActionDef> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ERoutingActionDef> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<ERoutingActionDef> TableToList(DataTable table) {
			List<ERoutingActionDef> retVal=new List<ERoutingActionDef>();
			ERoutingActionDef eRoutingActionDef;
			foreach(DataRow row in table.Rows) {
				eRoutingActionDef=new ERoutingActionDef();
				eRoutingActionDef.ERoutingActionDefNum= PIn.Long  (row["ERoutingActionDefNum"].ToString());
				eRoutingActionDef.ERoutingDefNum      = PIn.Long  (row["ERoutingDefNum"].ToString());
				eRoutingActionDef.ERoutingActionType  = (OpenDentBusiness.EnumERoutingActionType)PIn.Int(row["ERoutingActionType"].ToString());
				eRoutingActionDef.ItemOrder           = PIn.Int   (row["ItemOrder"].ToString());
				eRoutingActionDef.SecDateTEntry       = PIn.DateT (row["SecDateTEntry"].ToString());
				eRoutingActionDef.DateTLastModified   = PIn.DateT (row["DateTLastModified"].ToString());
				eRoutingActionDef.ForeignKey          = PIn.Long  (row["ForeignKey"].ToString());
				eRoutingActionDef.ForeignKeyType      = (OpenDentBusiness.EnumERoutingDefFKType)PIn.Int(row["ForeignKeyType"].ToString());
				retVal.Add(eRoutingActionDef);
			}
			return retVal;
		}

		///<summary>Converts a list of ERoutingActionDef into a DataTable.</summary>
		public static DataTable ListToTable(List<ERoutingActionDef> listERoutingActionDefs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="ERoutingActionDef";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("ERoutingActionDefNum");
			table.Columns.Add("ERoutingDefNum");
			table.Columns.Add("ERoutingActionType");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("SecDateTEntry");
			table.Columns.Add("DateTLastModified");
			table.Columns.Add("ForeignKey");
			table.Columns.Add("ForeignKeyType");
			foreach(ERoutingActionDef eRoutingActionDef in listERoutingActionDefs) {
				table.Rows.Add(new object[] {
					POut.Long  (eRoutingActionDef.ERoutingActionDefNum),
					POut.Long  (eRoutingActionDef.ERoutingDefNum),
					POut.Int   ((int)eRoutingActionDef.ERoutingActionType),
					POut.Int   (eRoutingActionDef.ItemOrder),
					POut.DateT (eRoutingActionDef.SecDateTEntry,false),
					POut.DateT (eRoutingActionDef.DateTLastModified,false),
					POut.Long  (eRoutingActionDef.ForeignKey),
					POut.Int   ((int)eRoutingActionDef.ForeignKeyType),
				});
			}
			return table;
		}

		///<summary>Inserts one ERoutingActionDef into the database.  Returns the new priKey.</summary>
		public static long Insert(ERoutingActionDef eRoutingActionDef) {
			return Insert(eRoutingActionDef,false);
		}

		///<summary>Inserts one ERoutingActionDef into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(ERoutingActionDef eRoutingActionDef,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				eRoutingActionDef.ERoutingActionDefNum=ReplicationServers.GetKey("eroutingactiondef","ERoutingActionDefNum");
			}
			string command="INSERT INTO eroutingactiondef (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="ERoutingActionDefNum,";
			}
			command+="ERoutingDefNum,ERoutingActionType,ItemOrder,SecDateTEntry,DateTLastModified,ForeignKey,ForeignKeyType) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(eRoutingActionDef.ERoutingActionDefNum)+",";
			}
			command+=
				     POut.Long  (eRoutingActionDef.ERoutingDefNum)+","
				+    POut.Int   ((int)eRoutingActionDef.ERoutingActionType)+","
				+    POut.Int   (eRoutingActionDef.ItemOrder)+","
				+    DbHelper.Now()+","
				+    POut.DateT (eRoutingActionDef.DateTLastModified)+","
				+    POut.Long  (eRoutingActionDef.ForeignKey)+","
				+    POut.Int   ((int)eRoutingActionDef.ForeignKeyType)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				eRoutingActionDef.ERoutingActionDefNum=Db.NonQ(command,true,"ERoutingActionDefNum","eRoutingActionDef");
			}
			return eRoutingActionDef.ERoutingActionDefNum;
		}

		///<summary>Inserts one ERoutingActionDef into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ERoutingActionDef eRoutingActionDef) {
			return InsertNoCache(eRoutingActionDef,false);
		}

		///<summary>Inserts one ERoutingActionDef into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ERoutingActionDef eRoutingActionDef,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO eroutingactiondef (";
			if(!useExistingPK && isRandomKeys) {
				eRoutingActionDef.ERoutingActionDefNum=ReplicationServers.GetKeyNoCache("eroutingactiondef","ERoutingActionDefNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="ERoutingActionDefNum,";
			}
			command+="ERoutingDefNum,ERoutingActionType,ItemOrder,SecDateTEntry,DateTLastModified,ForeignKey,ForeignKeyType) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(eRoutingActionDef.ERoutingActionDefNum)+",";
			}
			command+=
				     POut.Long  (eRoutingActionDef.ERoutingDefNum)+","
				+    POut.Int   ((int)eRoutingActionDef.ERoutingActionType)+","
				+    POut.Int   (eRoutingActionDef.ItemOrder)+","
				+    DbHelper.Now()+","
				+    POut.DateT (eRoutingActionDef.DateTLastModified)+","
				+    POut.Long  (eRoutingActionDef.ForeignKey)+","
				+    POut.Int   ((int)eRoutingActionDef.ForeignKeyType)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				eRoutingActionDef.ERoutingActionDefNum=Db.NonQ(command,true,"ERoutingActionDefNum","eRoutingActionDef");
			}
			return eRoutingActionDef.ERoutingActionDefNum;
		}

		///<summary>Updates one ERoutingActionDef in the database.</summary>
		public static void Update(ERoutingActionDef eRoutingActionDef) {
			string command="UPDATE eroutingactiondef SET "
				+"ERoutingDefNum      =  "+POut.Long  (eRoutingActionDef.ERoutingDefNum)+", "
				+"ERoutingActionType  =  "+POut.Int   ((int)eRoutingActionDef.ERoutingActionType)+", "
				+"ItemOrder           =  "+POut.Int   (eRoutingActionDef.ItemOrder)+", "
				//SecDateTEntry not allowed to change
				+"DateTLastModified   =  "+POut.DateT (eRoutingActionDef.DateTLastModified)+", "
				+"ForeignKey          =  "+POut.Long  (eRoutingActionDef.ForeignKey)+", "
				+"ForeignKeyType      =  "+POut.Int   ((int)eRoutingActionDef.ForeignKeyType)+" "
				+"WHERE ERoutingActionDefNum = "+POut.Long(eRoutingActionDef.ERoutingActionDefNum);
			Db.NonQ(command);
		}

		///<summary>Updates one ERoutingActionDef in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(ERoutingActionDef eRoutingActionDef,ERoutingActionDef oldERoutingActionDef) {
			string command="";
			if(eRoutingActionDef.ERoutingDefNum != oldERoutingActionDef.ERoutingDefNum) {
				if(command!="") { command+=",";}
				command+="ERoutingDefNum = "+POut.Long(eRoutingActionDef.ERoutingDefNum)+"";
			}
			if(eRoutingActionDef.ERoutingActionType != oldERoutingActionDef.ERoutingActionType) {
				if(command!="") { command+=",";}
				command+="ERoutingActionType = "+POut.Int   ((int)eRoutingActionDef.ERoutingActionType)+"";
			}
			if(eRoutingActionDef.ItemOrder != oldERoutingActionDef.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(eRoutingActionDef.ItemOrder)+"";
			}
			//SecDateTEntry not allowed to change
			if(eRoutingActionDef.DateTLastModified != oldERoutingActionDef.DateTLastModified) {
				if(command!="") { command+=",";}
				command+="DateTLastModified = "+POut.DateT(eRoutingActionDef.DateTLastModified)+"";
			}
			if(eRoutingActionDef.ForeignKey != oldERoutingActionDef.ForeignKey) {
				if(command!="") { command+=",";}
				command+="ForeignKey = "+POut.Long(eRoutingActionDef.ForeignKey)+"";
			}
			if(eRoutingActionDef.ForeignKeyType != oldERoutingActionDef.ForeignKeyType) {
				if(command!="") { command+=",";}
				command+="ForeignKeyType = "+POut.Int   ((int)eRoutingActionDef.ForeignKeyType)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE eroutingactiondef SET "+command
				+" WHERE ERoutingActionDefNum = "+POut.Long(eRoutingActionDef.ERoutingActionDefNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(ERoutingActionDef,ERoutingActionDef) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(ERoutingActionDef eRoutingActionDef,ERoutingActionDef oldERoutingActionDef) {
			if(eRoutingActionDef.ERoutingDefNum != oldERoutingActionDef.ERoutingDefNum) {
				return true;
			}
			if(eRoutingActionDef.ERoutingActionType != oldERoutingActionDef.ERoutingActionType) {
				return true;
			}
			if(eRoutingActionDef.ItemOrder != oldERoutingActionDef.ItemOrder) {
				return true;
			}
			//SecDateTEntry not allowed to change
			if(eRoutingActionDef.DateTLastModified != oldERoutingActionDef.DateTLastModified) {
				return true;
			}
			if(eRoutingActionDef.ForeignKey != oldERoutingActionDef.ForeignKey) {
				return true;
			}
			if(eRoutingActionDef.ForeignKeyType != oldERoutingActionDef.ForeignKeyType) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one ERoutingActionDef from the database.</summary>
		public static void Delete(long eRoutingActionDefNum) {
			string command="DELETE FROM eroutingactiondef "
				+"WHERE ERoutingActionDefNum = "+POut.Long(eRoutingActionDefNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many ERoutingActionDefs from the database.</summary>
		public static void DeleteMany(List<long> listERoutingActionDefNums) {
			if(listERoutingActionDefNums==null || listERoutingActionDefNums.Count==0) {
				return;
			}
			string command="DELETE FROM eroutingactiondef "
				+"WHERE ERoutingActionDefNum IN("+string.Join(",",listERoutingActionDefNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}