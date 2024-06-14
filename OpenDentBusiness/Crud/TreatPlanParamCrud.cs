//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class TreatPlanParamCrud {
		///<summary>Gets one TreatPlanParam object from the database using the primary key.  Returns null if not found.</summary>
		public static TreatPlanParam SelectOne(long treatPlanParamNum) {
			string command="SELECT * FROM treatplanparam "
				+"WHERE TreatPlanParamNum = "+POut.Long(treatPlanParamNum);
			List<TreatPlanParam> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one TreatPlanParam object from the database using a query.</summary>
		public static TreatPlanParam SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TreatPlanParam> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of TreatPlanParam objects from the database using a query.</summary>
		public static List<TreatPlanParam> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TreatPlanParam> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<TreatPlanParam> TableToList(DataTable table) {
			List<TreatPlanParam> retVal=new List<TreatPlanParam>();
			TreatPlanParam treatPlanParam;
			foreach(DataRow row in table.Rows) {
				treatPlanParam=new TreatPlanParam();
				treatPlanParam.TreatPlanParamNum= PIn.Long  (row["TreatPlanParamNum"].ToString());
				treatPlanParam.PatNum           = PIn.Long  (row["PatNum"].ToString());
				treatPlanParam.TreatPlanNum     = PIn.Long  (row["TreatPlanNum"].ToString());
				treatPlanParam.ShowDiscount     = PIn.Bool  (row["ShowDiscount"].ToString());
				treatPlanParam.ShowMaxDed       = PIn.Bool  (row["ShowMaxDed"].ToString());
				treatPlanParam.ShowSubTotals    = PIn.Bool  (row["ShowSubTotals"].ToString());
				treatPlanParam.ShowTotals       = PIn.Bool  (row["ShowTotals"].ToString());
				treatPlanParam.ShowCompleted    = PIn.Bool  (row["ShowCompleted"].ToString());
				treatPlanParam.ShowFees         = PIn.Bool  (row["ShowFees"].ToString());
				treatPlanParam.ShowIns          = PIn.Bool  (row["ShowIns"].ToString());
				retVal.Add(treatPlanParam);
			}
			return retVal;
		}

		///<summary>Converts a list of TreatPlanParam into a DataTable.</summary>
		public static DataTable ListToTable(List<TreatPlanParam> listTreatPlanParams,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="TreatPlanParam";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("TreatPlanParamNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("TreatPlanNum");
			table.Columns.Add("ShowDiscount");
			table.Columns.Add("ShowMaxDed");
			table.Columns.Add("ShowSubTotals");
			table.Columns.Add("ShowTotals");
			table.Columns.Add("ShowCompleted");
			table.Columns.Add("ShowFees");
			table.Columns.Add("ShowIns");
			foreach(TreatPlanParam treatPlanParam in listTreatPlanParams) {
				table.Rows.Add(new object[] {
					POut.Long  (treatPlanParam.TreatPlanParamNum),
					POut.Long  (treatPlanParam.PatNum),
					POut.Long  (treatPlanParam.TreatPlanNum),
					POut.Bool  (treatPlanParam.ShowDiscount),
					POut.Bool  (treatPlanParam.ShowMaxDed),
					POut.Bool  (treatPlanParam.ShowSubTotals),
					POut.Bool  (treatPlanParam.ShowTotals),
					POut.Bool  (treatPlanParam.ShowCompleted),
					POut.Bool  (treatPlanParam.ShowFees),
					POut.Bool  (treatPlanParam.ShowIns),
				});
			}
			return table;
		}

		///<summary>Inserts one TreatPlanParam into the database.  Returns the new priKey.</summary>
		public static long Insert(TreatPlanParam treatPlanParam) {
			return Insert(treatPlanParam,false);
		}

		///<summary>Inserts one TreatPlanParam into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(TreatPlanParam treatPlanParam,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				treatPlanParam.TreatPlanParamNum=ReplicationServers.GetKey("treatplanparam","TreatPlanParamNum");
			}
			string command="INSERT INTO treatplanparam (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="TreatPlanParamNum,";
			}
			command+="PatNum,TreatPlanNum,ShowDiscount,ShowMaxDed,ShowSubTotals,ShowTotals,ShowCompleted,ShowFees,ShowIns) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(treatPlanParam.TreatPlanParamNum)+",";
			}
			command+=
				     POut.Long  (treatPlanParam.PatNum)+","
				+    POut.Long  (treatPlanParam.TreatPlanNum)+","
				+    POut.Bool  (treatPlanParam.ShowDiscount)+","
				+    POut.Bool  (treatPlanParam.ShowMaxDed)+","
				+    POut.Bool  (treatPlanParam.ShowSubTotals)+","
				+    POut.Bool  (treatPlanParam.ShowTotals)+","
				+    POut.Bool  (treatPlanParam.ShowCompleted)+","
				+    POut.Bool  (treatPlanParam.ShowFees)+","
				+    POut.Bool  (treatPlanParam.ShowIns)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				treatPlanParam.TreatPlanParamNum=Db.NonQ(command,true,"TreatPlanParamNum","treatPlanParam");
			}
			return treatPlanParam.TreatPlanParamNum;
		}

		///<summary>Inserts one TreatPlanParam into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TreatPlanParam treatPlanParam) {
			return InsertNoCache(treatPlanParam,false);
		}

		///<summary>Inserts one TreatPlanParam into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TreatPlanParam treatPlanParam,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO treatplanparam (";
			if(!useExistingPK && isRandomKeys) {
				treatPlanParam.TreatPlanParamNum=ReplicationServers.GetKeyNoCache("treatplanparam","TreatPlanParamNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="TreatPlanParamNum,";
			}
			command+="PatNum,TreatPlanNum,ShowDiscount,ShowMaxDed,ShowSubTotals,ShowTotals,ShowCompleted,ShowFees,ShowIns) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(treatPlanParam.TreatPlanParamNum)+",";
			}
			command+=
				     POut.Long  (treatPlanParam.PatNum)+","
				+    POut.Long  (treatPlanParam.TreatPlanNum)+","
				+    POut.Bool  (treatPlanParam.ShowDiscount)+","
				+    POut.Bool  (treatPlanParam.ShowMaxDed)+","
				+    POut.Bool  (treatPlanParam.ShowSubTotals)+","
				+    POut.Bool  (treatPlanParam.ShowTotals)+","
				+    POut.Bool  (treatPlanParam.ShowCompleted)+","
				+    POut.Bool  (treatPlanParam.ShowFees)+","
				+    POut.Bool  (treatPlanParam.ShowIns)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				treatPlanParam.TreatPlanParamNum=Db.NonQ(command,true,"TreatPlanParamNum","treatPlanParam");
			}
			return treatPlanParam.TreatPlanParamNum;
		}

		///<summary>Updates one TreatPlanParam in the database.</summary>
		public static void Update(TreatPlanParam treatPlanParam) {
			string command="UPDATE treatplanparam SET "
				+"PatNum           =  "+POut.Long  (treatPlanParam.PatNum)+", "
				+"TreatPlanNum     =  "+POut.Long  (treatPlanParam.TreatPlanNum)+", "
				+"ShowDiscount     =  "+POut.Bool  (treatPlanParam.ShowDiscount)+", "
				+"ShowMaxDed       =  "+POut.Bool  (treatPlanParam.ShowMaxDed)+", "
				+"ShowSubTotals    =  "+POut.Bool  (treatPlanParam.ShowSubTotals)+", "
				+"ShowTotals       =  "+POut.Bool  (treatPlanParam.ShowTotals)+", "
				+"ShowCompleted    =  "+POut.Bool  (treatPlanParam.ShowCompleted)+", "
				+"ShowFees         =  "+POut.Bool  (treatPlanParam.ShowFees)+", "
				+"ShowIns          =  "+POut.Bool  (treatPlanParam.ShowIns)+" "
				+"WHERE TreatPlanParamNum = "+POut.Long(treatPlanParam.TreatPlanParamNum);
			Db.NonQ(command);
		}

		///<summary>Updates one TreatPlanParam in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(TreatPlanParam treatPlanParam,TreatPlanParam oldTreatPlanParam) {
			string command="";
			if(treatPlanParam.PatNum != oldTreatPlanParam.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(treatPlanParam.PatNum)+"";
			}
			if(treatPlanParam.TreatPlanNum != oldTreatPlanParam.TreatPlanNum) {
				if(command!="") { command+=",";}
				command+="TreatPlanNum = "+POut.Long(treatPlanParam.TreatPlanNum)+"";
			}
			if(treatPlanParam.ShowDiscount != oldTreatPlanParam.ShowDiscount) {
				if(command!="") { command+=",";}
				command+="ShowDiscount = "+POut.Bool(treatPlanParam.ShowDiscount)+"";
			}
			if(treatPlanParam.ShowMaxDed != oldTreatPlanParam.ShowMaxDed) {
				if(command!="") { command+=",";}
				command+="ShowMaxDed = "+POut.Bool(treatPlanParam.ShowMaxDed)+"";
			}
			if(treatPlanParam.ShowSubTotals != oldTreatPlanParam.ShowSubTotals) {
				if(command!="") { command+=",";}
				command+="ShowSubTotals = "+POut.Bool(treatPlanParam.ShowSubTotals)+"";
			}
			if(treatPlanParam.ShowTotals != oldTreatPlanParam.ShowTotals) {
				if(command!="") { command+=",";}
				command+="ShowTotals = "+POut.Bool(treatPlanParam.ShowTotals)+"";
			}
			if(treatPlanParam.ShowCompleted != oldTreatPlanParam.ShowCompleted) {
				if(command!="") { command+=",";}
				command+="ShowCompleted = "+POut.Bool(treatPlanParam.ShowCompleted)+"";
			}
			if(treatPlanParam.ShowFees != oldTreatPlanParam.ShowFees) {
				if(command!="") { command+=",";}
				command+="ShowFees = "+POut.Bool(treatPlanParam.ShowFees)+"";
			}
			if(treatPlanParam.ShowIns != oldTreatPlanParam.ShowIns) {
				if(command!="") { command+=",";}
				command+="ShowIns = "+POut.Bool(treatPlanParam.ShowIns)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE treatplanparam SET "+command
				+" WHERE TreatPlanParamNum = "+POut.Long(treatPlanParam.TreatPlanParamNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(TreatPlanParam,TreatPlanParam) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(TreatPlanParam treatPlanParam,TreatPlanParam oldTreatPlanParam) {
			if(treatPlanParam.PatNum != oldTreatPlanParam.PatNum) {
				return true;
			}
			if(treatPlanParam.TreatPlanNum != oldTreatPlanParam.TreatPlanNum) {
				return true;
			}
			if(treatPlanParam.ShowDiscount != oldTreatPlanParam.ShowDiscount) {
				return true;
			}
			if(treatPlanParam.ShowMaxDed != oldTreatPlanParam.ShowMaxDed) {
				return true;
			}
			if(treatPlanParam.ShowSubTotals != oldTreatPlanParam.ShowSubTotals) {
				return true;
			}
			if(treatPlanParam.ShowTotals != oldTreatPlanParam.ShowTotals) {
				return true;
			}
			if(treatPlanParam.ShowCompleted != oldTreatPlanParam.ShowCompleted) {
				return true;
			}
			if(treatPlanParam.ShowFees != oldTreatPlanParam.ShowFees) {
				return true;
			}
			if(treatPlanParam.ShowIns != oldTreatPlanParam.ShowIns) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one TreatPlanParam from the database.</summary>
		public static void Delete(long treatPlanParamNum) {
			string command="DELETE FROM treatplanparam "
				+"WHERE TreatPlanParamNum = "+POut.Long(treatPlanParamNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many TreatPlanParams from the database.</summary>
		public static void DeleteMany(List<long> listTreatPlanParamNums) {
			if(listTreatPlanParamNums==null || listTreatPlanParamNums.Count==0) {
				return;
			}
			string command="DELETE FROM treatplanparam "
				+"WHERE TreatPlanParamNum IN("+string.Join(",",listTreatPlanParamNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}