using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RxDefs {
		///<summary></summary>
		public static RxDef[] Refresh() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<RxDef[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM rxdef ORDER BY Drug";
			return Crud.RxDefCrud.SelectMany(command).ToArray();
		}

		public static RxDef GetOne(long rxDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<RxDef>(MethodBase.GetCurrentMethod(),rxDefNum);
			}
			return Crud.RxDefCrud.SelectOne(rxDefNum);
		}

		///<summary></summary>
		public static void Update(RxDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.RxDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(RxDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				def.RxDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.RxDefNum;
			}
			return Crud.RxDefCrud.Insert(def);
		}

		///<summary></summary>
		public static List<RxDef> TableToList(DataTable table) {
			//No need to check MiddleTierRole; Calls GetTableRemotelyIfNeeded().
			return Crud.RxDefCrud.TableToList(table);
		}

		///<summary>Also deletes all RxAlerts that were attached.</summary>
		public static void Delete(RxDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command="DELETE FROM rxalert WHERE RxDefNum="+POut.Long(def.RxDefNum);
			Db.NonQ(command);
			command= "DELETE FROM rxdef WHERE RxDefNum = "+POut.Long(def.RxDefNum);
			Db.NonQ(command);
		}
		
		///<summary>Used to combine prescriptions by first adjusting the FKs on any necessary tables, then removing the prescription from the rxDef table.</summary>
		public static void Combine(List<long> listRxNums,long pickedRxNum) {
			if(listRxNums.Count<=1) {
				return;//nothing to do
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRxNums,pickedRxNum);
				return;
			}
			for(int i=0;i<listRxNums.Count;i++) {
				if(listRxNums[i]==pickedRxNum) {
					continue;
				}
				string command="UPDATE rxalert SET RxDefNum="+POut.Long(pickedRxNum)+" "
					+"WHERE RxDefNum="+POut.Long(listRxNums[i]);
				Db.NonQ(command);
				command="DELETE FROM rxdef "
					+"WHERE RxDefNum="+POut.Long(listRxNums[i]);
				Db.NonQ(command);
			}
		}
	}

}













