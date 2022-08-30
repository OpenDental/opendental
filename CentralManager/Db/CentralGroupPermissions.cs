using OpenDentBusiness;
using System.Collections.Generic;

namespace CentralManager {
	public class CentralGroupPermissions {

		///<summary>Syncs the database to reflect what is contained within listNew. Deletes all GroupPermissions from the remote UserGroup and inserts what the CEMT UserGroup has.</summary>
		public static void Sync(List<GroupPermission> listNew,List<GroupPermission> listDB) {
			for(int i=0;i < listDB.Count;i++) {
				GroupPermissions.DeleteNoCache(listDB[i]);
			}
			//Commit changes to DB
			for(int i=0;i < listNew.Count;i++) {
				GroupPermissions.InsertNoCache(listNew[i]);
			}
		}
	}
}
