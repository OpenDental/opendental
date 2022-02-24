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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<RxDef[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM rxdef ORDER BY Drug";
			return Crud.RxDefCrud.SelectMany(command).ToArray();
		}

		public static RxDef GetOne(long rxDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<RxDef>(MethodBase.GetCurrentMethod(),rxDefNum);
			}
			return Crud.RxDefCrud.SelectOne(rxDefNum);
		}

		///<summary></summary>
		public static void Update(RxDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.RxDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(RxDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				def.RxDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.RxDefNum;
			}
			return Crud.RxDefCrud.Insert(def);
		}

		///<summary></summary>
		public static List<RxDef> TableToList(DataTable table) {
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			return Crud.RxDefCrud.TableToList(table);
		}

		///<summary>Also deletes all RxAlerts that were attached.</summary>
		public static void Delete(RxDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command="DELETE FROM rxalert WHERE RxDefNum="+POut.Long(def.RxDefNum);
			Db.NonQ(command);
			command= "DELETE FROM rxdef WHERE RxDefNum = "+POut.Long(def.RxDefNum);
			Db.NonQ(command);
		}	
	}

}













