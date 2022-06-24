using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EClipboardImageCaptureDefs{
		
		#region Methods - Get
		///<summary>Retrievs all the eclipboard image capture defs from the db.</summary>
		public static List<EClipboardImageCaptureDef> Refresh(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EClipboardImageCaptureDef>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM eclipboardimagecapturedef";
			return Crud.EClipboardImageCaptureDefCrud.SelectMany(command);
		}
		/*
		///<summary>Gets one EClipboardImageCaptureDef from the db.</summary>
		public static EClipboardImageCaptureDef GetOne(long eClipboardImageCaptureDefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EClipboardImageCaptureDef>(MethodBase.GetCurrentMethod(),eClipboardImageCaptureDefNum);
			}
			return Crud.EClipboardImageCaptureDefCrud.SelectOne(eClipboardImageCaptureDefNum);
		}
		*/
		#endregion Methods - Get
		
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(EClipboardImageCaptureDef eClipboardImageCaptureDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				eClipboardImageCaptureDef.EClipboardImageCaptureDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eClipboardImageCaptureDef);
				return eClipboardImageCaptureDef.EClipboardImageCaptureDefNum;
			}
			return Crud.EClipboardImageCaptureDefCrud.Insert(eClipboardImageCaptureDef);
		}

		/*
		///<summary></summary>
		public static void Update(EClipboardImageCaptureDef eClipboardImageCaptureDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eClipboardImageCaptureDef);
				return;
			}
			Crud.EClipboardImageCaptureDefCrud.Update(eClipboardImageCaptureDef);
		}
		
		///<summary></summary>
		public static void Delete(long eClipboardImageCaptureDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eClipboardImageCaptureDefNum);
				return;
			}
			Crud.EClipboardImageCaptureDefCrud.Delete(eClipboardImageCaptureDefNum);
		}
		*/
		#endregion Methods - Modify
		

		#region Methods - Misc
		///<summary>Inserts, updates, or deletes db rows to match listNew</summary>
		public static bool Sync(List<EClipboardImageCaptureDef> listNew,List<EClipboardImageCaptureDef> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.EClipboardImageCaptureDefCrud.Sync(listNew,listOld);
		}

		///<summary>Returns true if eClipboard Image definition is currently being used.</summary>
		public static bool IsEClipboardImageDefInUse(long defNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),defNum);
			}
			string command=$"SELECT COUNT(*) FROM eclipboardimagecapturedef WHERE DefNum="+POut.Long(defNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}
		#endregion Methods - Misc
	}
}
