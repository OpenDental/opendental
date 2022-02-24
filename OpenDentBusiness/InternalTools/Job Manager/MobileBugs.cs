using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using OpenDental;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileBugs{
		#region Get Methods
		///<summary>Gets one MobileBug from the db.</summary>
		public static MobileBug GetOne(long mobileBugNum,bool useConnectionStore=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MobileBug>(MethodBase.GetCurrentMethod(),mobileBugNum,useConnectionStore);
			}
			return DataAction.GetBugsHQ(() => Crud.MobileBugCrud.SelectOne(mobileBugNum),useConnectionStore);
		}

		///<summary>Gets all bugs ordered by CreationDate DESC</summary>
		public static List<MobileBug> GetAll(bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileBug>>(MethodBase.GetCurrentMethod(),useConnectionStore);
			}
			string command="SELECT * FROM mobilebug ORDER BY DateTimeCreated DESC";
			return DataAction.GetBugsHQ(() => Crud.MobileBugCrud.SelectMany(command),useConnectionStore);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(MobileBug mobileBug,bool useConnectionStore=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				mobileBug.MobileBugNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileBug,useConnectionStore);
				return mobileBug.MobileBugNum;
			}
			return DataAction.GetBugsHQ(() => Crud.MobileBugCrud.Insert(mobileBug),useConnectionStore);
		}

		///<summary></summary>
		public static void Update(MobileBug mobileBug,bool useConnectionStore=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBug,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.MobileBugCrud.Update(mobileBug),useConnectionStore);
		}

		///<summary></summary>
		public static void Delete(long mobileBugNum,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileBugNum,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.MobileBugCrud.Delete(mobileBugNum),useConnectionStore);
		}
		#endregion Modification Methods

		#region Misc Methods
		///<Summary>Checks MobileBugNums in list for incompletes. Returns false if incomplete exists.</Summary>
		public static bool CheckForCompletion(List<long> listBugIDs,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listBugIDs,useConnectionStore);
			}
			int count=0;
			string command="SELECT COUNT(*) FROM mobilebug "
				+"WHERE BugStatus!='"+PIn.String(MobileBugStatus.Fixed.ToString())+"' "
				+"AND MobileBugNum IN ("+String.Join(",",listBugIDs)+")";
			DataAction.RunBugsHQ(() => count=PIn.Int(Db.GetCount(command)),useConnectionStore);
			return (count==0);
		}

		public static MobileBug GetNewBugForUser() {
			MobileBug mobileBug=new MobileBug();
			mobileBug.DateTimeCreated=DateTime.Now;
			mobileBug.BugStatus=MobileBugStatus.Found;
			mobileBug.Submitter=Bugs.GetBugSubmitter();
			return mobileBug;
		}
		#endregion Misc Methods
	}
}