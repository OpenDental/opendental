using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileReleases{
		#region Get Methods
		///<summary>Gets one MobileRelease from the db.</summary>
		public static MobileRelease GetOne(long mobileReleaseNum,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MobileRelease>(MethodBase.GetCurrentMethod(),mobileReleaseNum,useConnectionStore);
			}
			string command="SELECT * FROM mobilerelease WHERE MobileReleaseNum="+POut.Long(mobileReleaseNum);
			return DataAction.GetBugsHQ(() => Crud.MobileReleaseCrud.SelectOne(command),useConnectionStore);
		}

		///<summary>Gets the latest release for an app type.</summary>
		public static MobileRelease GetLastReleaseForApp(AppType appType,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MobileRelease>(MethodBase.GetCurrentMethod(),appType,useConnectionStore);
			}
			string command="SELECT * FROM mobilerelease "
				+"WHERE AppType='"+POut.String(appType.ToString())+"' "
				+"ORDER BY MajorNum DESC, MinorNum DESC, BuildNum DESC ";
			return DataAction.GetBugsHQ(() => Crud.MobileReleaseCrud.SelectOne(command),useConnectionStore);
		}

		///<summary>Gets what is predicted to be the next release based on the release pattern in JordansRemoteManagementTool.</summary>
		public static string GetNextVersionForApp(AppType appType,bool useConnectionStore=false) {
			//No need for remoting role, no call to db.
			MobileRelease lastRelease=GetLastReleaseForApp(appType,useConnectionStore);
			int versionAsNumber=int.Parse(lastRelease.MajorNum.ToString()+lastRelease.MinorNum.ToString()+lastRelease.BuildNum.ToString());
			versionAsNumber++;
			string versionStr=versionAsNumber.ToString();
			return versionStr.Substring(0,versionStr.Length-2)+"."+versionStr[versionStr.Length-2].ToString()+"."+versionStr[versionStr.Length-1].ToString();
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(MobileRelease mobileRelease,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				mobileRelease.MobileReleaseNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileRelease,useConnectionStore);
				return mobileRelease.MobileReleaseNum;
			}
			return DataAction.GetBugsHQ(() => Crud.MobileReleaseCrud.Insert(mobileRelease),useConnectionStore);
		}
		#endregion Modification Methods
		/*
		#region Get Methods
		///<summary></summary>
		public static List<MobileRelease> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileRelease>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM mobilerelease WHERE PatNum = "+POut.Long(patNum);
			return Crud.MobileReleaseCrud.SelectMany(command);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Update(MobileRelease mobileRelease){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileRelease);
				return;
			}
			Crud.MobileReleaseCrud.Update(mobileRelease);
		}
		///<summary></summary>
		public static void Delete(long mobileReleaseNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileReleaseNum);
				return;
			}
			Crud.MobileReleaseCrud.Delete(mobileReleaseNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/
	}
}