//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class AutoCodeCondCrud {
		///<summary>Gets one AutoCodeCond object from the database using the primary key.  Returns null if not found.</summary>
		public static AutoCodeCond SelectOne(long autoCodeCondNum) {
			string command="SELECT * FROM autocodecond "
				+"WHERE AutoCodeCondNum = "+POut.Long(autoCodeCondNum);
			List<AutoCodeCond> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one AutoCodeCond object from the database using a query.</summary>
		public static AutoCodeCond SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoCodeCond> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of AutoCodeCond objects from the database using a query.</summary>
		public static List<AutoCodeCond> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoCodeCond> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<AutoCodeCond> TableToList(DataTable table) {
			List<AutoCodeCond> retVal=new List<AutoCodeCond>();
			AutoCodeCond autoCodeCond;
			foreach(DataRow row in table.Rows) {
				autoCodeCond=new AutoCodeCond();
				autoCodeCond.AutoCodeCondNum= PIn.Long  (row["AutoCodeCondNum"].ToString());
				autoCodeCond.AutoCodeItemNum= PIn.Long  (row["AutoCodeItemNum"].ToString());
				autoCodeCond.Cond           = (OpenDentBusiness.AutoCondition)PIn.Int(row["Cond"].ToString());
				retVal.Add(autoCodeCond);
			}
			return retVal;
		}

		///<summary>Converts a list of AutoCodeCond into a DataTable.</summary>
		public static DataTable ListToTable(List<AutoCodeCond> listAutoCodeConds,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="AutoCodeCond";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("AutoCodeCondNum");
			table.Columns.Add("AutoCodeItemNum");
			table.Columns.Add("Cond");
			foreach(AutoCodeCond autoCodeCond in listAutoCodeConds) {
				table.Rows.Add(new object[] {
					POut.Long  (autoCodeCond.AutoCodeCondNum),
					POut.Long  (autoCodeCond.AutoCodeItemNum),
					POut.Int   ((int)autoCodeCond.Cond),
				});
			}
			return table;
		}

		///<summary>Inserts one AutoCodeCond into the database.  Returns the new priKey.</summary>
		public static long Insert(AutoCodeCond autoCodeCond) {
			return Insert(autoCodeCond,false);
		}

		///<summary>Inserts one AutoCodeCond into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(AutoCodeCond autoCodeCond,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				autoCodeCond.AutoCodeCondNum=ReplicationServers.GetKey("autocodecond","AutoCodeCondNum");
			}
			string command="INSERT INTO autocodecond (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="AutoCodeCondNum,";
			}
			command+="AutoCodeItemNum,Cond) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(autoCodeCond.AutoCodeCondNum)+",";
			}
			command+=
				     POut.Long  (autoCodeCond.AutoCodeItemNum)+","
				+    POut.Int   ((int)autoCodeCond.Cond)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				autoCodeCond.AutoCodeCondNum=Db.NonQ(command,true,"AutoCodeCondNum","autoCodeCond");
			}
			return autoCodeCond.AutoCodeCondNum;
		}

		///<summary>Inserts one AutoCodeCond into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoCodeCond autoCodeCond) {
			return InsertNoCache(autoCodeCond,false);
		}

		///<summary>Inserts one AutoCodeCond into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoCodeCond autoCodeCond,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO autocodecond (";
			if(!useExistingPK && isRandomKeys) {
				autoCodeCond.AutoCodeCondNum=ReplicationServers.GetKeyNoCache("autocodecond","AutoCodeCondNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="AutoCodeCondNum,";
			}
			command+="AutoCodeItemNum,Cond) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(autoCodeCond.AutoCodeCondNum)+",";
			}
			command+=
				     POut.Long  (autoCodeCond.AutoCodeItemNum)+","
				+    POut.Int   ((int)autoCodeCond.Cond)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				autoCodeCond.AutoCodeCondNum=Db.NonQ(command,true,"AutoCodeCondNum","autoCodeCond");
			}
			return autoCodeCond.AutoCodeCondNum;
		}

		///<summary>Updates one AutoCodeCond in the database.</summary>
		public static void Update(AutoCodeCond autoCodeCond) {
			string command="UPDATE autocodecond SET "
				+"AutoCodeItemNum=  "+POut.Long  (autoCodeCond.AutoCodeItemNum)+", "
				+"Cond           =  "+POut.Int   ((int)autoCodeCond.Cond)+" "
				+"WHERE AutoCodeCondNum = "+POut.Long(autoCodeCond.AutoCodeCondNum);
			Db.NonQ(command);
		}

		///<summary>Updates one AutoCodeCond in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(AutoCodeCond autoCodeCond,AutoCodeCond oldAutoCodeCond) {
			string command="";
			if(autoCodeCond.AutoCodeItemNum != oldAutoCodeCond.AutoCodeItemNum) {
				if(command!="") { command+=",";}
				command+="AutoCodeItemNum = "+POut.Long(autoCodeCond.AutoCodeItemNum)+"";
			}
			if(autoCodeCond.Cond != oldAutoCodeCond.Cond) {
				if(command!="") { command+=",";}
				command+="Cond = "+POut.Int   ((int)autoCodeCond.Cond)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE autocodecond SET "+command
				+" WHERE AutoCodeCondNum = "+POut.Long(autoCodeCond.AutoCodeCondNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(AutoCodeCond,AutoCodeCond) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(AutoCodeCond autoCodeCond,AutoCodeCond oldAutoCodeCond) {
			if(autoCodeCond.AutoCodeItemNum != oldAutoCodeCond.AutoCodeItemNum) {
				return true;
			}
			if(autoCodeCond.Cond != oldAutoCodeCond.Cond) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one AutoCodeCond from the database.</summary>
		public static void Delete(long autoCodeCondNum) {
			string command="DELETE FROM autocodecond "
				+"WHERE AutoCodeCondNum = "+POut.Long(autoCodeCondNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many AutoCodeConds from the database.</summary>
		public static void DeleteMany(List<long> listAutoCodeCondNums) {
			if(listAutoCodeCondNums==null || listAutoCodeCondNums.Count==0) {
				return;
			}
			string command="DELETE FROM autocodecond "
				+"WHERE AutoCodeCondNum IN("+string.Join(",",listAutoCodeCondNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}