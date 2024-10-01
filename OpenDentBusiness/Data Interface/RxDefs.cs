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
		public static void Update(RxDef rxDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxDef);
				return;
			}
			Crud.RxDefCrud.Update(rxDef);
		}

		///<summary></summary>
		public static long Insert(RxDef rxDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				rxDef.RxDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),rxDef);
				return rxDef.RxDefNum;
			}
			return Crud.RxDefCrud.Insert(rxDef);
		}

		///<summary></summary>
		public static List<RxDef> TableToList(DataTable table) {
			Meth.NoCheckMiddleTierRole();//Calls GetTableRemotelyIfNeeded().
			return Crud.RxDefCrud.TableToList(table);
		}

		///<summary>Also deletes all RxAlerts that were attached.</summary>
		public static void Delete(RxDef rxDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxDef);
				return;
			}
			string command="DELETE FROM rxalert WHERE RxDefNum="+POut.Long(rxDef.RxDefNum);
			Db.NonQ(command);
			command= "DELETE FROM rxdef WHERE RxDefNum = "+POut.Long(rxDef.RxDefNum);
			Db.NonQ(command);
		}
		
		///<summary>Used to combine prescriptions by first adjusting the FKs on any necessary tables, then removing the prescription from the rxDef table.</summary>
		public static void Combine(List<long> listRxDefNums,long rxDefNumPicked) {
			if(listRxDefNums.Count<=1) {
				return;//nothing to do
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRxDefNums,rxDefNumPicked);
				return;
			}
			for(int i=0;i<listRxDefNums.Count;i++) {
				if(listRxDefNums[i]==rxDefNumPicked) {
					continue;
				}
				string command="UPDATE rxalert SET RxDefNum="+POut.Long(rxDefNumPicked)+" "
					+"WHERE RxDefNum="+POut.Long(listRxDefNums[i]);
				Db.NonQ(command);
				command="DELETE FROM rxdef "
					+"WHERE RxDefNum="+POut.Long(listRxDefNums[i]);
				Db.NonQ(command);
			}
		}
	}

}













