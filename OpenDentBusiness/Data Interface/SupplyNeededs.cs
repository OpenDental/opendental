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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SupplyNeeded>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM supplyneeded ORDER BY DateAdded";
			return Crud.SupplyNeededCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(SupplyNeeded supplyNeeded) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				supplyNeeded.SupplyNeededNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supplyNeeded);
				return supplyNeeded.SupplyNeededNum;
			}
			return Crud.SupplyNeededCrud.Insert(supplyNeeded);
		}

		///<summary></summary>
		public static void Update(SupplyNeeded supplyNeeded) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyNeeded);
				return;
			}
			Crud.SupplyNeededCrud.Update(supplyNeeded);
		}

		///<summary></summary>
		public static void DeleteObject(SupplyNeeded supplyNeeded){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyNeeded);
				return;
			}
			Crud.SupplyNeededCrud.Delete(supplyNeeded.SupplyNeededNum);
		}
	}

	


	


}