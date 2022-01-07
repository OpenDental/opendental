//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class SupplyNeededCrud {
		///<summary>Gets one SupplyNeeded object from the database using the primary key.  Returns null if not found.</summary>
		public static SupplyNeeded SelectOne(long supplyNeededNum) {
			string command="SELECT * FROM supplyneeded "
				+"WHERE SupplyNeededNum = "+POut.Long(supplyNeededNum);
			List<SupplyNeeded> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one SupplyNeeded object from the database using a query.</summary>
		public static SupplyNeeded SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<SupplyNeeded> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of SupplyNeeded objects from the database using a query.</summary>
		public static List<SupplyNeeded> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<SupplyNeeded> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<SupplyNeeded> TableToList(DataTable table) {
			List<SupplyNeeded> retVal=new List<SupplyNeeded>();
			SupplyNeeded supplyNeeded;
			foreach(DataRow row in table.Rows) {
				supplyNeeded=new SupplyNeeded();
				supplyNeeded.SupplyNeededNum= PIn.Long  (row["SupplyNeededNum"].ToString());
				supplyNeeded.Description    = PIn.String(row["Description"].ToString());
				supplyNeeded.DateAdded      = PIn.Date  (row["DateAdded"].ToString());
				retVal.Add(supplyNeeded);
			}
			return retVal;
		}

		///<summary>Converts a list of SupplyNeeded into a DataTable.</summary>
		public static DataTable ListToTable(List<SupplyNeeded> listSupplyNeededs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="SupplyNeeded";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("SupplyNeededNum");
			table.Columns.Add("Description");
			table.Columns.Add("DateAdded");
			foreach(SupplyNeeded supplyNeeded in listSupplyNeededs) {
				table.Rows.Add(new object[] {
					POut.Long  (supplyNeeded.SupplyNeededNum),
					            supplyNeeded.Description,
					POut.DateT (supplyNeeded.DateAdded,false),
				});
			}
			return table;
		}

		///<summary>Inserts one SupplyNeeded into the database.  Returns the new priKey.</summary>
		public static long Insert(SupplyNeeded supplyNeeded) {
			return Insert(supplyNeeded,false);
		}

		///<summary>Inserts one SupplyNeeded into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(SupplyNeeded supplyNeeded,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				supplyNeeded.SupplyNeededNum=ReplicationServers.GetKey("supplyneeded","SupplyNeededNum");
			}
			string command="INSERT INTO supplyneeded (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="SupplyNeededNum,";
			}
			command+="Description,DateAdded) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(supplyNeeded.SupplyNeededNum)+",";
			}
			command+=
				     DbHelper.ParamChar+"paramDescription,"
				+    POut.Date  (supplyNeeded.DateAdded)+")";
			if(supplyNeeded.Description==null) {
				supplyNeeded.Description="";
			}
			OdSqlParameter paramDescription=new OdSqlParameter("paramDescription",OdDbType.Text,POut.StringParam(supplyNeeded.Description));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramDescription);
			}
			else {
				supplyNeeded.SupplyNeededNum=Db.NonQ(command,true,"SupplyNeededNum","supplyNeeded",paramDescription);
			}
			return supplyNeeded.SupplyNeededNum;
		}

		///<summary>Inserts one SupplyNeeded into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(SupplyNeeded supplyNeeded) {
			return InsertNoCache(supplyNeeded,false);
		}

		///<summary>Inserts one SupplyNeeded into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(SupplyNeeded supplyNeeded,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO supplyneeded (";
			if(!useExistingPK && isRandomKeys) {
				supplyNeeded.SupplyNeededNum=ReplicationServers.GetKeyNoCache("supplyneeded","SupplyNeededNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="SupplyNeededNum,";
			}
			command+="Description,DateAdded) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(supplyNeeded.SupplyNeededNum)+",";
			}
			command+=
				     DbHelper.ParamChar+"paramDescription,"
				+    POut.Date  (supplyNeeded.DateAdded)+")";
			if(supplyNeeded.Description==null) {
				supplyNeeded.Description="";
			}
			OdSqlParameter paramDescription=new OdSqlParameter("paramDescription",OdDbType.Text,POut.StringParam(supplyNeeded.Description));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramDescription);
			}
			else {
				supplyNeeded.SupplyNeededNum=Db.NonQ(command,true,"SupplyNeededNum","supplyNeeded",paramDescription);
			}
			return supplyNeeded.SupplyNeededNum;
		}

		///<summary>Updates one SupplyNeeded in the database.</summary>
		public static void Update(SupplyNeeded supplyNeeded) {
			string command="UPDATE supplyneeded SET "
				+"Description    =  "+DbHelper.ParamChar+"paramDescription, "
				+"DateAdded      =  "+POut.Date  (supplyNeeded.DateAdded)+" "
				+"WHERE SupplyNeededNum = "+POut.Long(supplyNeeded.SupplyNeededNum);
			if(supplyNeeded.Description==null) {
				supplyNeeded.Description="";
			}
			OdSqlParameter paramDescription=new OdSqlParameter("paramDescription",OdDbType.Text,POut.StringParam(supplyNeeded.Description));
			Db.NonQ(command,paramDescription);
		}

		///<summary>Updates one SupplyNeeded in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(SupplyNeeded supplyNeeded,SupplyNeeded oldSupplyNeeded) {
			string command="";
			if(supplyNeeded.Description != oldSupplyNeeded.Description) {
				if(command!="") { command+=",";}
				command+="Description = "+DbHelper.ParamChar+"paramDescription";
			}
			if(supplyNeeded.DateAdded.Date != oldSupplyNeeded.DateAdded.Date) {
				if(command!="") { command+=",";}
				command+="DateAdded = "+POut.Date(supplyNeeded.DateAdded)+"";
			}
			if(command=="") {
				return false;
			}
			if(supplyNeeded.Description==null) {
				supplyNeeded.Description="";
			}
			OdSqlParameter paramDescription=new OdSqlParameter("paramDescription",OdDbType.Text,POut.StringParam(supplyNeeded.Description));
			command="UPDATE supplyneeded SET "+command
				+" WHERE SupplyNeededNum = "+POut.Long(supplyNeeded.SupplyNeededNum);
			Db.NonQ(command,paramDescription);
			return true;
		}

		///<summary>Returns true if Update(SupplyNeeded,SupplyNeeded) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(SupplyNeeded supplyNeeded,SupplyNeeded oldSupplyNeeded) {
			if(supplyNeeded.Description != oldSupplyNeeded.Description) {
				return true;
			}
			if(supplyNeeded.DateAdded.Date != oldSupplyNeeded.DateAdded.Date) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one SupplyNeeded from the database.</summary>
		public static void Delete(long supplyNeededNum) {
			string command="DELETE FROM supplyneeded "
				+"WHERE SupplyNeededNum = "+POut.Long(supplyNeededNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many SupplyNeededs from the database.</summary>
		public static void DeleteMany(List<long> listSupplyNeededNums) {
			if(listSupplyNeededNums==null || listSupplyNeededNums.Count==0) {
				return;
			}
			string command="DELETE FROM supplyneeded "
				+"WHERE SupplyNeededNum IN("+string.Join(",",listSupplyNeededNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}