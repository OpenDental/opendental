using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class RxAlerts {
		///<summary>Gets a list of all RxAlerts for one RxDef.</summary>
		public static List<RxAlert> Refresh(long rxDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<RxAlert>>(MethodBase.GetCurrentMethod(),rxDefNum);
			}
			string command="SELECT * FROM rxalert WHERE RxDefNum="+POut.Long(rxDefNum);
			return Crud.RxAlertCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<RxAlert> TableToList(DataTable table) {
			Meth.NoCheckMiddleTierRole();//Calls GetTableRemotelyIfNeeded().
			return Crud.RxAlertCrud.TableToList(table);
		}

		///<summary></summary>
		public static void Update(RxAlert rxAlert) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxAlert);
				return;
			}
			Crud.RxAlertCrud.Update(rxAlert);
		}

		///<summary></summary>
		public static long Insert(RxAlert rxAlert) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				rxAlert.RxAlertNum=Meth.GetLong(MethodBase.GetCurrentMethod(),rxAlert);
				return rxAlert.RxAlertNum;
			}
			return Crud.RxAlertCrud.Insert(rxAlert);
		}

		///<summary></summary>
		public static void Delete(RxAlert rxAlert) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxAlert);
				return;
			}
			string command="DELETE FROM rxalert WHERE RxAlertNum ="+POut.Long(rxAlert.RxAlertNum);
			Db.NonQ(command);
		}
	}

}










