using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ImageDraws{
		#region Methods - Get
		///<summary>All drawings for a single document.</summary>
		public static List<ImageDraw> RefreshForDoc(long docNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ImageDraw>>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="SELECT * FROM imagedraw WHERE DocNum = "+POut.Long(docNum);
			return Crud.ImageDrawCrud.SelectMany(command);
		}

		///<summary>All drawings for a single mount.</summary>
		public static List<ImageDraw> RefreshForMount(long mountNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ImageDraw>>(MethodBase.GetCurrentMethod(),mountNum);
			}
			string command="SELECT * FROM imagedraw WHERE MountNum = "+POut.Long(mountNum);
			return Crud.ImageDrawCrud.SelectMany(command);
		}
		/*
		///<summary>Gets one ImageDraw from the db.</summary>
		public static ImageDraw GetOne(long imageDrawNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ImageDraw>(MethodBase.GetCurrentMethod(),imageDrawNum);
			}
			return Crud.ImageDrawCrud.SelectOne(imageDrawNum);
		}*/
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ImageDraw imageDraw){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				imageDraw.ImageDrawNum=Meth.GetLong(MethodBase.GetCurrentMethod(),imageDraw);
				return imageDraw.ImageDrawNum;
			}
			return Crud.ImageDrawCrud.Insert(imageDraw);
		}
		///<summary></summary>
		public static void Update(ImageDraw imageDraw){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),imageDraw);
				return;
			}
			Crud.ImageDrawCrud.Update(imageDraw);
		}
		///<summary></summary>
		public static void Delete(long imageDrawNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),imageDrawNum);
				return;
			}
			Crud.ImageDrawCrud.Delete(imageDrawNum);
		}
		#endregion Methods - Modify
		



	}
}