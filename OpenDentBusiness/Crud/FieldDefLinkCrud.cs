//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class FieldDefLinkCrud {
		///<summary>Gets one FieldDefLink object from the database using the primary key.  Returns null if not found.</summary>
		public static FieldDefLink SelectOne(long fieldDefLinkNum) {
			string command="SELECT * FROM fielddeflink "
				+"WHERE FieldDefLinkNum = "+POut.Long(fieldDefLinkNum);
			List<FieldDefLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one FieldDefLink object from the database using a query.</summary>
		public static FieldDefLink SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FieldDefLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of FieldDefLink objects from the database using a query.</summary>
		public static List<FieldDefLink> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FieldDefLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<FieldDefLink> TableToList(DataTable table) {
			List<FieldDefLink> retVal=new List<FieldDefLink>();
			FieldDefLink fieldDefLink;
			foreach(DataRow row in table.Rows) {
				fieldDefLink=new FieldDefLink();
				fieldDefLink.FieldDefLinkNum= PIn.Long  (row["FieldDefLinkNum"].ToString());
				fieldDefLink.FieldDefNum    = PIn.Long  (row["FieldDefNum"].ToString());
				fieldDefLink.FieldDefType   = (OpenDentBusiness.FieldDefTypes)PIn.Int(row["FieldDefType"].ToString());
				fieldDefLink.FieldLocation  = (OpenDentBusiness.FieldLocations)PIn.Int(row["FieldLocation"].ToString());
				retVal.Add(fieldDefLink);
			}
			return retVal;
		}

		///<summary>Converts a list of FieldDefLink into a DataTable.</summary>
		public static DataTable ListToTable(List<FieldDefLink> listFieldDefLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="FieldDefLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("FieldDefLinkNum");
			table.Columns.Add("FieldDefNum");
			table.Columns.Add("FieldDefType");
			table.Columns.Add("FieldLocation");
			foreach(FieldDefLink fieldDefLink in listFieldDefLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (fieldDefLink.FieldDefLinkNum),
					POut.Long  (fieldDefLink.FieldDefNum),
					POut.Int   ((int)fieldDefLink.FieldDefType),
					POut.Int   ((int)fieldDefLink.FieldLocation),
				});
			}
			return table;
		}

		///<summary>Inserts one FieldDefLink into the database.  Returns the new priKey.</summary>
		public static long Insert(FieldDefLink fieldDefLink) {
			return Insert(fieldDefLink,false);
		}

		///<summary>Inserts one FieldDefLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(FieldDefLink fieldDefLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				fieldDefLink.FieldDefLinkNum=ReplicationServers.GetKey("fielddeflink","FieldDefLinkNum");
			}
			string command="INSERT INTO fielddeflink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FieldDefLinkNum,";
			}
			command+="FieldDefNum,FieldDefType,FieldLocation) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(fieldDefLink.FieldDefLinkNum)+",";
			}
			command+=
				     POut.Long  (fieldDefLink.FieldDefNum)+","
				+    POut.Int   ((int)fieldDefLink.FieldDefType)+","
				+    POut.Int   ((int)fieldDefLink.FieldLocation)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				fieldDefLink.FieldDefLinkNum=Db.NonQ(command,true,"FieldDefLinkNum","fieldDefLink");
			}
			return fieldDefLink.FieldDefLinkNum;
		}

		///<summary>Inserts one FieldDefLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FieldDefLink fieldDefLink) {
			return InsertNoCache(fieldDefLink,false);
		}

		///<summary>Inserts one FieldDefLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FieldDefLink fieldDefLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO fielddeflink (";
			if(!useExistingPK && isRandomKeys) {
				fieldDefLink.FieldDefLinkNum=ReplicationServers.GetKeyNoCache("fielddeflink","FieldDefLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="FieldDefLinkNum,";
			}
			command+="FieldDefNum,FieldDefType,FieldLocation) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(fieldDefLink.FieldDefLinkNum)+",";
			}
			command+=
				     POut.Long  (fieldDefLink.FieldDefNum)+","
				+    POut.Int   ((int)fieldDefLink.FieldDefType)+","
				+    POut.Int   ((int)fieldDefLink.FieldLocation)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				fieldDefLink.FieldDefLinkNum=Db.NonQ(command,true,"FieldDefLinkNum","fieldDefLink");
			}
			return fieldDefLink.FieldDefLinkNum;
		}

		///<summary>Updates one FieldDefLink in the database.</summary>
		public static void Update(FieldDefLink fieldDefLink) {
			string command="UPDATE fielddeflink SET "
				+"FieldDefNum    =  "+POut.Long  (fieldDefLink.FieldDefNum)+", "
				+"FieldDefType   =  "+POut.Int   ((int)fieldDefLink.FieldDefType)+", "
				+"FieldLocation  =  "+POut.Int   ((int)fieldDefLink.FieldLocation)+" "
				+"WHERE FieldDefLinkNum = "+POut.Long(fieldDefLink.FieldDefLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one FieldDefLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(FieldDefLink fieldDefLink,FieldDefLink oldFieldDefLink) {
			string command="";
			if(fieldDefLink.FieldDefNum != oldFieldDefLink.FieldDefNum) {
				if(command!="") { command+=",";}
				command+="FieldDefNum = "+POut.Long(fieldDefLink.FieldDefNum)+"";
			}
			if(fieldDefLink.FieldDefType != oldFieldDefLink.FieldDefType) {
				if(command!="") { command+=",";}
				command+="FieldDefType = "+POut.Int   ((int)fieldDefLink.FieldDefType)+"";
			}
			if(fieldDefLink.FieldLocation != oldFieldDefLink.FieldLocation) {
				if(command!="") { command+=",";}
				command+="FieldLocation = "+POut.Int   ((int)fieldDefLink.FieldLocation)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE fielddeflink SET "+command
				+" WHERE FieldDefLinkNum = "+POut.Long(fieldDefLink.FieldDefLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(FieldDefLink,FieldDefLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(FieldDefLink fieldDefLink,FieldDefLink oldFieldDefLink) {
			if(fieldDefLink.FieldDefNum != oldFieldDefLink.FieldDefNum) {
				return true;
			}
			if(fieldDefLink.FieldDefType != oldFieldDefLink.FieldDefType) {
				return true;
			}
			if(fieldDefLink.FieldLocation != oldFieldDefLink.FieldLocation) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one FieldDefLink from the database.</summary>
		public static void Delete(long fieldDefLinkNum) {
			string command="DELETE FROM fielddeflink "
				+"WHERE FieldDefLinkNum = "+POut.Long(fieldDefLinkNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many FieldDefLinks from the database.</summary>
		public static void DeleteMany(List<long> listFieldDefLinkNums) {
			if(listFieldDefLinkNums==null || listFieldDefLinkNums.Count==0) {
				return;
			}
			string command="DELETE FROM fielddeflink "
				+"WHERE FieldDefLinkNum IN("+string.Join(",",listFieldDefLinkNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<FieldDefLink> listNew,List<FieldDefLink> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<FieldDefLink> listIns    =new List<FieldDefLink>();
			List<FieldDefLink> listUpdNew =new List<FieldDefLink>();
			List<FieldDefLink> listUpdDB  =new List<FieldDefLink>();
			List<FieldDefLink> listDel    =new List<FieldDefLink>();
			listNew.Sort((FieldDefLink x,FieldDefLink y) => { return x.FieldDefLinkNum.CompareTo(y.FieldDefLinkNum); });
			listDB.Sort((FieldDefLink x,FieldDefLink y) => { return x.FieldDefLinkNum.CompareTo(y.FieldDefLinkNum); });
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			FieldDefLink fieldNew;
			FieldDefLink fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.FieldDefLinkNum<fieldDB.FieldDefLinkNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.FieldDefLinkNum>fieldDB.FieldDefLinkNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			DeleteMany(listDel.Select(x => x.FieldDefLinkNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}