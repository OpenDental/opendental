using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using OpenDentBusiness;

namespace OpenDentBusiness {
	public class Mounts {
		public static long Insert(Mount mount){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				mount.MountNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mount);
				return mount.MountNum;
			}
			return Crud.MountCrud.Insert(mount);
		}

		public static void Update(Mount mount){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mount);
				return;
			}
			Crud.MountCrud.Update(mount);
		}


		///<summary>Should already have checked that no images are attached.</summary>
		public static void Delete(Mount mount){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mount);
				return;
			}
			string command="DELETE FROM mount WHERE MountNum='"+POut.Long(mount.MountNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM mountitem WHERE MountNum='"+POut.Long(mount.MountNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Returns a single mount object corresponding to the given mount number key.</summary>
		public static Mount GetByNum(long mountNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Mount>(MethodBase.GetCurrentMethod(),mountNum);
			}
			Mount mount= Crud.MountCrud.SelectOne(mountNum);
			if(mount==null){
				return new Mount();
			}
			return mount;
		}

		///<summary>MountItems are included.  Everything has been inserted into the database.</summary>
		public static Mount CreateMountFromDef(MountDef mountDef, long patNum, long docCategory){
			//No need to check RemotingRole; no call to db.
			Mount mount=new Mount();
			mount.PatNum=patNum;
			mount.DocCategory=docCategory;
			mount.DateCreated=DateTime.Now;
			mount.Description=mountDef.Description;
			mount.Note="";
			mount.Width=mountDef.Width;
			mount.Height=mountDef.Height;
			mount.ColorBack=mountDef.ColorBack;
			mount.MountNum=Insert(mount);
			mount.ListMountItems=new List<MountItem>();
			List<MountItemDef> listMountItemDefs=MountItemDefs.GetForMountDef(mountDef.MountDefNum);
			for(int i=0;i<listMountItemDefs.Count;i++){
				MountItem mountItem=new MountItem();
				mountItem.MountNum=mount.MountNum;
				mountItem.Xpos=listMountItemDefs[i].Xpos;
				mountItem.Ypos=listMountItemDefs[i].Ypos;
				mountItem.ItemOrder=listMountItemDefs[i].ItemOrder;
				mountItem.Width=listMountItemDefs[i].Width;
				mountItem.Height=listMountItemDefs[i].Height;
				mountItem.RotateOnAcquire=listMountItemDefs[i].RotateOnAcquire;
				mountItem.ToothNumbers=listMountItemDefs[i].ToothNumbers;
				mountItem.MountItemNum=MountItems.Insert(mountItem);
				mount.ListMountItems.Add(mountItem);
			}
			return mount;
		}
	}
}