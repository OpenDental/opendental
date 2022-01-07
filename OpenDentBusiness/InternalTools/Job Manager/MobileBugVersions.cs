using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileBugVersions {
		#region Get Methods
		///<summary>Returns a list of mobilebugversions for the given mobilebugnum.</summary>
		public static List<MobileBugVersion> GetVersionsForMobileBugNum(long mobileBugNum,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileBugVersion>>(MethodBase.GetCurrentMethod(),mobileBugNum,useConnectionStore);
			}
			List<MobileBugVersion> retVal=new List<MobileBugVersion>();
			string command="SELECT * FROM mobilebugversion WHERE MobileBugNum = '"+POut.Long(mobileBugNum)+"'";
			return DataAction.GetBugsHQ(() => Crud.MobileBugVersionCrud.SelectMany(command),useConnectionStore);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(MobileBugVersion mobileBugVersion,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				mobileBugVersion.MobileBugVersionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileBugVersion,useConnectionStore);
				return mobileBugVersion.MobileBugVersionNum;
			}
			return DataAction.GetBugsHQ(() => Crud.MobileBugVersionCrud.Insert(mobileBugVersion),useConnectionStore);
		}

		///<summary></summary>
		public static void Update(MobileBugVersion mobileBugVersion,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBugVersion,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.MobileBugVersionCrud.Update(mobileBugVersion),useConnectionStore);
		}

		///<summary></summary>
		public static void Delete(long mobileBugVersionNum,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBugVersionNum,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.MobileBugVersionCrud.Delete(mobileBugVersionNum),useConnectionStore);
		}
		#endregion Modification Methods

		#region Misc Methods
		#endregion Misc Methods

		/*
		///<summary></summary>
		public static List<MobileBugVersion> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileBugVersion>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM mobilebugversion WHERE PatNum = "+POut.Long(patNum);
			return Crud.MobileBugVersionCrud.SelectMany(command);
		}
		
		///<summary>Gets one MobileBugVersion from the db.</summary>
		public static MobileBugVersion GetOne(long mobileBugVersionNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MobileBugVersion>(MethodBase.GetCurrentMethod(),mobileBugVersionNum);
			}
			return Crud.MobileBugVersionCrud.SelectOne(mobileBugVersionNum);
		}
		#endregion Get Methods
		*/
	}
}