using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class MobileReleaseBuilds {
		#region Get Methods
		public static MobileReleaseBuild GetNewestBuildForAppInStore(AppType appType,AppStore appStore,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MobileReleaseBuild>(MethodBase.GetCurrentMethod(),appType,appStore,useConnectionStore);
			}
			string command="SELECT * FROM mobilereleasebuild"
				+" WHERE AppType='"+POut.String(appType.ToString())+"'"
				+" AND AppStore='"+POut.String(appStore.ToString())+"'";
			return DataAction.GetBugsHQ(() =>
				Crud.MobileReleaseBuildCrud.SelectMany(command).OrderByDescending(x => x.AppVersion).FirstOrDefault()
			,useConnectionStore);
		}

		///<summary>Returns the newest build with status "Released" from the table.</summary>
		public static MobileReleaseBuild GetNewestReleasedBuildForAppInStore(AppType appType,AppStore appStore,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MobileReleaseBuild>(MethodBase.GetCurrentMethod(),appType,appStore,useConnectionStore);
			}
			string command="SELECT * FROM mobilereleasebuild"
				+" WHERE AppType='"+POut.String(appType.ToString())+"'"
				+" AND AppStore='"+POut.String(appStore.ToString())+"'"
				+" AND StoreStatus='"+POut.String(StoreStatus.Released.ToString())+"'";
			return DataAction.GetBugsHQ(() =>
				Crud.MobileReleaseBuildCrud.SelectMany(command).OrderByDescending(x => x.AppVersion).FirstOrDefault()
			,useConnectionStore);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static void Update(MobileReleaseBuild mobileReleaseBuild,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileReleaseBuild,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.MobileReleaseBuildCrud.Update(mobileReleaseBuild),useConnectionStore);
		}

		///<summary></summary>
		public static long Insert(MobileReleaseBuild mobileReleaseBuild,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				mobileReleaseBuild.MobileReleaseBuildNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileReleaseBuild,useConnectionStore);
				return mobileReleaseBuild.MobileReleaseBuildNum;
			}
			return DataAction.GetBugsHQ(() => Crud.MobileReleaseBuildCrud.Insert(mobileReleaseBuild),useConnectionStore);
		}
		#endregion Modification Methods
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		/*
		///<summary></summary>
		public static List<MobileReleaseBuild> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileReleaseBuild>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM mobilereleasebuild WHERE PatNum = "+POut.Long(patNum);
			return Crud.MobileReleaseBuildCrud.SelectMany(command);
		}
		
		///<summary>Gets one MobileReleaseBuild from the db.</summary>
		public static MobileReleaseBuild GetOne(long mobileReleaseBuildNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MobileReleaseBuild>(MethodBase.GetCurrentMethod(),mobileReleaseBuildNum);
			}
			return Crud.MobileReleaseBuildCrud.SelectOne(mobileReleaseBuildNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Delete(long mobileReleaseBuildNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileReleaseBuildNum);
				return;
			}
			Crud.MobileReleaseBuildCrud.Delete(mobileReleaseBuildNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		#endregion Misc Methods
		*/
	}
}