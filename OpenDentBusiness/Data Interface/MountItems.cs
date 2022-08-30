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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				mountItem.MountItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mountItem);
				return mountItem.MountItemNum;
			}
			return Crud.MountItemCrud.Insert(mountItem);
		}

		public static void Update(MountItem mountItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountItem);
				return;
			}
			Crud.MountItemCrud.Update(mountItem);
		}

		///<summary>Will throw an exception if any document(image) attached to this mountitem.</summary>
		public static void Delete(MountItem mountItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountItem);
				return;
			}
			string command="SELECT COUNT(*) FROM document WHERE MountItemNum="+POut.Long(mountItem.MountItemNum);
			string count=Db.GetCount(command);
			if(count!="0"){
				throw new ApplicationException(Lans.g("MountItems","Not allowed to delete a MountItem that has an attached image."));
			}
			command="DELETE FROM mountitem WHERE MountItemNum='"+POut.Long(mountItem.MountItemNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Returns the list of mount items associated with the given mount key. In order by ItemOrder, which is 1-indexed.</summary>
		public static List<MountItem> GetItemsForMount(long mountNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List <MountItem>>(MethodBase.GetCurrentMethod(),mountNum);
			}
			string command="SELECT * FROM mountitem WHERE MountNum='"+POut.Long(mountNum)+"' ORDER BY ItemOrder";
			return Crud.MountItemCrud.SelectMany(command);
		}

		/*
		///<summary>Returns the list of MountItems containing the passed list of MountItemNums. Ordered by MountItemNum.</summary>
		public static List<MountItem> GetMountItems(List<long> listMountItemNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MountItem>>(MethodBase.GetCurrentMethod(),listMountItemNums);
			}
			string command="SELECT * FROM mountitem WHERE MountItemNum IN("+string.Join(",",listMountItemNums)+") ORDER BY MountItemNum";
			return Crud.MountItemCrud.SelectMany(command);
		}*/
	}
}