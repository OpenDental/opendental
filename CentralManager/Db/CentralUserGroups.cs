using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentralManager {
	public class CentralUserGroups {

		///<summary>Syncs the database to reflect what is contained within listNew.  Returns a list of UserGroups that need to be deleted as Users need to be moved out of the group before they can be deleted.  Compares entries based on UserGroupNumCEMT rather than primary keys.</summary>
		public static List<UserGroup> Sync(List<UserGroup> listNew,List<UserGroup> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<UserGroup> listIns    =new List<UserGroup>();
			List<UserGroup> listUpdNew =new List<UserGroup>();
			List<UserGroup> listUpdDB  =new List<UserGroup>();
			List<UserGroup> listDel    =new List<UserGroup>();
			listNew.Sort((UserGroup x,UserGroup y) => { return x.UserGroupNumCEMT.CompareTo(y.UserGroupNumCEMT); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((UserGroup x,UserGroup y) => { return x.UserGroupNumCEMT.CompareTo(y.UserGroupNumCEMT); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			UserGroup fieldNew;
			UserGroup fieldDB;
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
				else if(fieldNew.UserGroupNumCEMT<fieldDB.UserGroupNumCEMT) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.UserGroupNumCEMT>fieldDB.UserGroupNumCEMT) {//dbPK less than newPK, dbItem is 'next'
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
				UserGroups.InsertNoCache(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				UserGroups.UpdateCEMTNoCache(listUpdNew[i]);//Doesn't use cache
			}
			//for(int i=0;i<listDel.Count;i++) {
			//	UserGroups.Delete(listDel[i]);
			//}
			return listDel;
		}

	}
}
