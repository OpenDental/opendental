using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SupplyNeededs {
		///<summary>Gets all SupplyNeededs.</summary>
		public static List<SupplyNeeded> CreateObjects() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SupplyNeeded>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM supplyneeded ORDER BY DateAdded";
			return Crud.SupplyNeededCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(SupplyNeeded supp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				supp.SupplyNeededNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supp);
				return supp.SupplyNeededNum;
			}
			return Crud.SupplyNeededCrud.Insert(supp);
		}

		///<summary></summary>
		public static void Update(SupplyNeeded supp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supp);
				return;
			}
			Crud.SupplyNeededCrud.Update(supp);
		}

		///<summary></summary>
		public static void DeleteObject(SupplyNeeded supp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supp);
				return;
			}
			Crud.SupplyNeededCrud.Delete(supp.SupplyNeededNum);
		}
	}

	


	


}









