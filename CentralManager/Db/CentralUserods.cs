using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentralManager {
	public class CentralUserods {

		///<summary>Syncs the database to reflect what is contained within listNew.  Compares entries based on UserNumCEMT rather than primary keys.</summary>
		public static void Sync(List<Userod> listNew,ref List<Userod> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<Userod> listIns    =new List<Userod>();
			List<Userod> listUpdNew =new List<Userod>();
			List<Userod> listUpdDB  =new List<Userod>();
			List<Userod> listDel    =new List<Userod>();
			listNew.Sort((Userod x,Userod y) => { return x.UserNumCEMT.CompareTo(y.UserNumCEMT); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((Userod x,Userod y) => { return x.UserNumCEMT.CompareTo(y.UserNumCEMT); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			Userod fieldNew;
			Userod fieldDB;
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
				else if(fieldNew.UserNumCEMT<fieldDB.UserNumCEMT) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.UserNumCEMT>fieldDB.UserNumCEMT) {//dbPK less than newPK, dbItem is 'next'
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
				Userods.InsertNoCache(listIns[i]);
				listDB.Add(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				Userods.UpdateCEMT(listUpdNew[i]);//listUpdNew gets populated by the list coming from the CEMT. Userods.Update uses primary key to update. Won't work.  Need to make new update.
			}
			//for(int i=0;i<listDel.Count;i++) {//Userods can never be deleted.
			//	Userods.DeleteCEMT(listDel[i].SuperUserNum);
			//}
		}
		
	}
}
