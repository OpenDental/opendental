using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentralManager {
	public class CentralGroupPermissions {

		///<summary>Syncs the database to reflect what is contained within listNew.</summary>
		public static void Sync(List<GroupPermission> listNew,List<GroupPermission> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<GroupPermission> listIns    =new List<GroupPermission>();
			List<GroupPermission> listUpdNew =new List<GroupPermission>();
			List<GroupPermission> listUpdDB  =new List<GroupPermission>();
			List<GroupPermission> listDel    =new List<GroupPermission>();
			listNew.Sort((GroupPermission x,GroupPermission y) => { return x.GroupPermNum.CompareTo(y.GroupPermNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((GroupPermission x,GroupPermission y) => { return x.GroupPermNum.CompareTo(y.GroupPermNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			GroupPermission fieldNew;
			GroupPermission fieldDB;
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
				else if(fieldNew.GroupPermNum<fieldDB.GroupPermNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.GroupPermNum>fieldDB.GroupPermNum) {//dbPK less than newPK, dbItem is 'next'
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
				GroupPermissions.InsertNoCache(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				GroupPermissions.UpdateNoCache(listUpdNew[i]);
			}
			for(int i=0;i<listDel.Count;i++) {
				GroupPermissions.DeleteNoCache(listDel[i]);
			}
		}



	}
}
