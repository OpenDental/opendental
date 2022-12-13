using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EClipboardImageCaptures{
		#region Methods - Get
		///<summary>Returns all the image capture records of images a patient has submitted via eClipboard.</summary>
		public static List<EClipboardImageCapture> GetManyByPatNum(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EClipboardImageCapture>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eclipboardimagecapture WHERE PatNum = "+POut.Long(patNum);
			return Crud.EClipboardImageCaptureCrud.SelectMany(command);
		}

		///<summary>Returns image captures with the following docNum.</summary>
		/*public static List<EClipboardImageCapture> GetManyByDocNum(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EClipboardImageCapture>>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="SELECT * FROM eclipboardimagecapture WHERE DocNum = "+POut.Long(docNum);
			return Crud.EClipboardImageCaptureCrud.SelectMany(command);
		}
		
		///<summary>Gets one EClipboardImageCapture from the db.</summary>
		public static EClipboardImageCapture GetOne(long EClipboardImageCaptureNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EClipboardImageCapture>(MethodBase.GetCurrentMethod(),EClipboardImageCaptureNum);
			}
			return Crud.EClipboardImageCaptureCrud.SelectOne(EClipboardImageCaptureNum);
		}
		*/
		#endregion Methods - Get
		
		#region Methods - Modify
		///<summary>Insert a new EClipboardImageCapture object into the db.</summary>
		public static long Insert(EClipboardImageCapture eClipboardImageCapture){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eClipboardImageCapture.EClipboardImageCaptureNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eClipboardImageCapture);
				return eClipboardImageCapture.EClipboardImageCaptureNum;
			}
			return Crud.EClipboardImageCaptureCrud.Insert(eClipboardImageCapture);
		}

		///<summary>Updates an existing EClipboardImageCapture record in the db.</summary>
		public static void Update(EClipboardImageCapture eClipboardImageCapture){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eClipboardImageCapture);
				return;
			}
			Crud.EClipboardImageCaptureCrud.Update(eClipboardImageCapture);
		}

		public static void Upsert(EClipboardImageCapture eClipboardImageCapture) {
			//No need to call remoting check, not going to the database (yet)
			if(eClipboardImageCapture.EClipboardImageCaptureNum>0) {
				Update(eClipboardImageCapture);
			}
			else {
				Insert(eClipboardImageCapture);
			}
		}

		/*
		///<summary></summary>
		public static void Delete(long EClipboardImageCaptureNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),EClipboardImageCaptureNum);
				return;
			}
			Crud.EClipboardImageCaptureCrud.Delete(EClipboardImageCaptureNum);
		}
		*/

		///<summary></summary>
		/*public static void DeleteMany(List<long> listEClipboardImageCaptureNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEClipboardImageCaptureNums);
				return;
			}
			Crud.EClipboardImageCaptureCrud.DeleteMany(listEClipboardImageCaptureNums);
		}*/

		///<summary>Deletes every EClipboardImageCapture record in the DB that references the passed in DocNum.</summary>
		public static void DeleteByDocNum(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docNum);
				return;
			}
			string command=$"DELETE FROM eclipboardimagecapture WHERE DocNum = "+POut.Long(docNum);
			Db.NonQ(command);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		#endregion Methods - Misc
	}
}