using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using OpenDentBusiness;
using CodeBase;

namespace OpenDentBusiness {
	public class MountItems {
		public static long Insert(MountItem mountItem) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				mountItem.MountItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mountItem);
				return mountItem.MountItemNum;
			}
			return Crud.MountItemCrud.Insert(mountItem);
		}

		/*
		public static void Update(MountItem mountItem) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountItem);
				return;
			}
			Crud.MountItemCrud.Update(mountItem);
		}*/

		public static void Delete(MountItem mountItem) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountItem);
				return;
			}
			string command="DELETE FROM mountitem WHERE MountItemNum='"+POut.Long(mountItem.MountItemNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Returns the list of mount items associated with the given mount key. In order by ItemOrder, which is 1-indexed.</summary>
		public static List<MountItem> GetItemsForMount(long mountNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <MountItem>>(MethodBase.GetCurrentMethod(),mountNum);
			}
			string command="SELECT * FROM mountitem WHERE MountNum='"+POut.Long(mountNum)+"' ORDER BY ItemOrder";
			return Crud.MountItemCrud.SelectMany(command);
		}
	}
}