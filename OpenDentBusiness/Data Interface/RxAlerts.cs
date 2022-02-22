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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxAlert>>(MethodBase.GetCurrentMethod(),rxDefNum);
			}
			string command="SELECT * FROM rxalert WHERE RxDefNum="+POut.Long(rxDefNum);
			return Crud.RxAlertCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<RxAlert> TableToList(DataTable table) {
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			return Crud.RxAlertCrud.TableToList(table);
		}

		///<summary></summary>
		public static void Update(RxAlert alert) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alert);
				return;
			}
			Crud.RxAlertCrud.Update(alert);
		}

		///<summary></summary>
		public static long Insert(RxAlert alert) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				alert.RxAlertNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alert);
				return alert.RxAlertNum;
			}
			return Crud.RxAlertCrud.Insert(alert);
		}

		///<summary></summary>
		public static void Delete(RxAlert alert) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alert);
				return;
			}
			string command="DELETE FROM rxalert WHERE RxAlertNum ="+POut.Long(alert.RxAlertNum);
			Db.NonQ(command);
		}
	}

}










