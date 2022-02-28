using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using OpenDentBusiness;

namespace OpenDentBusiness {
	public class Mounts {
		//command="SELECT MountNum,DocCategory,DateCreated,Description FROM mount WHERE PatNum='"+patNum+"'";
		//	raw=dcon.GetTable(command);
		///<summary>Only gets mounts that would be visible in the Chart module.</summary>
		public static List<Mount> GetForChart(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Mount>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<long> listDefNums=new List<long>();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<listDefs.Count;i++){
				if(listDefs[i].ItemValue.Contains("X")){
					listDefNums.Add(listDefs[i].DefNum);
				}
			}
			if(listDefNums.Count==0){
				return new List<Mount>();
			}
			string command="SELECT * FROM mount WHERE PatNum="+POut.Long(patNum)+" "
				+"AND DocCategory IN(";
			for(int i=0;i<listDefNums.Count;i++){
				if(i>0){
					command+=",";
				}
				command+=POut.Long(listDefNums[i]);
			}
			command+=")  ORDER BY DateCreated";
			DataTable table=Db.GetTable(command);
			return Crud.MountCrud.TableToList(table);
		}

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
			mount.DocCategory=docCategory;//this was already decided one level up
			mount.DateCreated=DateTime.Now;
			mount.Description=mountDef.Description;
			mount.Note="";
			mount.Width=mountDef.Width;
			mount.Height=mountDef.Height;
			mount.ColorBack=mountDef.ColorBack;
			mount.ColorFore=mountDef.ColorFore;
			mount.ColorTextBack=mountDef.ColorTextBack;
			//mount.ScaleValue=mountDef.ScaleValue;//see below
			mount.MountNum=Insert(mount);
			//mount.ListMountItems=new List<MountItem>();
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
				mountItem.TextShowing=listMountItemDefs[i].TextShowing;
				mountItem.FontSize=listMountItemDefs[i].FontSize;
				mountItem.MountItemNum=MountItems.Insert(mountItem);
				//mount.ListMountItems.Add(mountItem);
			}
			if(mountDef.ScaleValue!=""){
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.MountNum=mount.MountNum;
				imageDraw.DrawType=ImageDrawType.ScaleValue;
				imageDraw.DrawingSegment=mountDef.ScaleValue;
				ImageDraws.Insert(imageDraw);
			}
			return mount;
		}

		///<summary>Gets the thumbnail image for the given mount. Like documents, the thumbnail for every mount is in a subfolder named 'Thumbnails' within each patient's images folder.  For now, they will be 100x100, although we might change that. Thumbnail file names are "Mount"+MountNum+".jpg". Example: ..\SmithJohn425\Thumbnails\Mount382.jpg.</summary>
		public static Bitmap GetThumbnail(long mountNum,string patFolder){
			//No need to check RemotingRole; no call to db.
			//Create Thumbnails folder if it does not already exist for this patient folder.
			string pathThumbnails=Path.Combine(patFolder,"Thumbnails");
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(pathThumbnails)) {
				try {
					Directory.CreateDirectory(pathThumbnails);
				} 
				catch{
					return Documents.NoAvailablePhoto();
				}
			}
			string fileName="Mount"+mountNum.ToString()+".jpg";
			string fileNameFull=Path.Combine(patFolder,"Thumbnails",fileName);
			//Use the existing thumbnail if it already exists
			//(no way currently to check how old it is)
			Bitmap bitmap=null;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(fileNameFull)) {
				try {
					bitmap=(Bitmap)Bitmap.FromFile(fileNameFull);
				}
				catch{
					try {
						File.Delete(fileNameFull); //File may be invalid, corrupted, or unavailable. This was a bug in previous versions.
					} 
					catch {
						//we tried our best, and it just wasn't good enough
						return Documents.NoAvailablePhoto();
					}
				}
				//but that bitmap has a file lock that we need to release.
				Bitmap bitmap2=new Bitmap(bitmap);
				bitmap?.Dispose();
				return bitmap2;
			}
			//Unlike documents, this method never creates a missing thumbnail because that would cause delays.
			//Creation happens in FormImageFloat.CreateMountThumbnail().
			return Documents.NoAvailablePhoto();
		}

		///<summary>Replaces </summary>
		public static string ReplaceMount(string stringOriginal,Mount mount,bool isHtmlEmail=false) {
			if(mount==null) {
				return stringOriginal;
			}
			StringBuilder stringBuilder=new StringBuilder(stringOriginal);
			ReplaceTags.ReplaceOneTag(stringBuilder,"[MountDate]",mount.DateCreated.ToShortDateString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(stringBuilder,"[MountDescript]",mount.Description,isHtmlEmail);
			ReplaceTags.ReplaceOneTag(stringBuilder,"[MountProv]",Providers.GetFormalName(mount.ProvNum),isHtmlEmail);
			return stringBuilder.ToString();
		}


	}
}