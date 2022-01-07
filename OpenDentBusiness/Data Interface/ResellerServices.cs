using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class ResellerServices {
		#region Get Methods

		///<summary></summary>
		public static List<ResellerService> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ResellerService>>(MethodBase.GetCurrentMethod());
			}
			return Crud.ResellerServiceCrud.SelectMany("SELECT * FROM resellerservice");
		}

		///<summary></summary>
		public static List<ResellerService> GetServicesForReseller(long resellerNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ResellerService>>(MethodBase.GetCurrentMethod(),resellerNum);
			}
			string command="SELECT * FROM resellerservice WHERE ResellerNum = "+POut.Long(resellerNum);
			return Crud.ResellerServiceCrud.SelectMany(command);
		}

		///<summary>Gets one ResellerService from the db.</summary>
		public static ResellerService GetOne(long resellerServiceNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ResellerService>(MethodBase.GetCurrentMethod(),resellerServiceNum);
			}
			return Crud.ResellerServiceCrud.SelectOne(resellerServiceNum);
		}

		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(ResellerService resellerService) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				resellerService.ResellerServiceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),resellerService);
				return resellerService.ResellerServiceNum;
			}
			return Crud.ResellerServiceCrud.Insert(resellerService);
		}

		#endregion

		#region Update

		///<summary></summary>
		public static void Update(ResellerService resellerService) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),resellerService);
				return;
			}
			Crud.ResellerServiceCrud.Update(resellerService);
		}

		#endregion

		#region Delete

		///<summary></summary>
		public static void Delete(long resellerServiceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),resellerServiceNum);
				return;
			}
			string command= "DELETE FROM resellerservice WHERE ResellerServiceNum = "+POut.Long(resellerServiceNum);
			Db.NonQ(command);
		}

		#endregion
	}
}