using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ConnGroupAttaches{
		///<summary>Gets all conn group attaches from the database.</summary>
		public static List<ConnGroupAttach> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ConnGroupAttach>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM conngroupattach";
			return Crud.ConnGroupAttachCrud.SelectMany(command);
		}

		///<summary>Gets all ConnGroupAttaches for a given ConnectionGroupNum.</summary>
		public static List<ConnGroupAttach> GetForGroup(long connectionGroupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<ConnGroupAttach>>(MethodBase.GetCurrentMethod(),connectionGroupNum);
			}
			string command="SELECT * FROM conngroupattach WHERE ConnectionGroupNum="+POut.Long(connectionGroupNum);
			return Crud.ConnGroupAttachCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Attach(long centralConnectionNum,long connectionGroupNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),centralConnectionNum,connectionGroupNum);
				return;
			}
			ConnGroupAttach connGroupAttach=new ConnGroupAttach();
			connGroupAttach.CentralConnectionNum=centralConnectionNum;
			connGroupAttach.ConnectionGroupNum=connectionGroupNum;
			Crud.ConnGroupAttachCrud.Insert(connGroupAttach);
		}

		///<summary></summary>
		public static void Detach(long centralConnectionNum,long connectionGroupNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),centralConnectionNum,connectionGroupNum);
				return;
			}
			string command="DELETE FROM conngroupattach WHERE "
				+"CentralConnectionNum="+POut.Long(centralConnectionNum)
				+" AND ConnectionGroupNum="+POut.Long(connectionGroupNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForGroup(long connectionGroupNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),connectionGroupNum);
				return;
			}
			string command="DELETE FROM conngroupattach WHERE "
				+"ConnectionGroupNum="+POut.Long(connectionGroupNum);
			Db.NonQ(command);
		}
	}
}