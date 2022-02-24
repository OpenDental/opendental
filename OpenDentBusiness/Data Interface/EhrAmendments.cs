using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrAmendments{
		///<summary>Gets list of all amendments for a specific patient and orders them by DateTRequest</summary>
		public static List<EhrAmendment> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrAmendment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehramendment WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateTRequest";
			return Crud.EhrAmendmentCrud.SelectMany(command);
		}

		///<summary>Gets one EhrAmendment from the db.</summary>
		public static EhrAmendment GetOne(long ehrAmendmentNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrAmendment>(MethodBase.GetCurrentMethod(),ehrAmendmentNum);
			}
			return Crud.EhrAmendmentCrud.SelectOne(ehrAmendmentNum);
		}

		///<summary></summary>
		public static long Insert(EhrAmendment ehrAmendment){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrAmendment.EhrAmendmentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrAmendment);
				return ehrAmendment.EhrAmendmentNum;
			}
			return Crud.EhrAmendmentCrud.Insert(ehrAmendment);
		}

		///<summary></summary>
		public static void Update(EhrAmendment ehrAmendment){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrAmendment);
				return;
			}
			Crud.EhrAmendmentCrud.Update(ehrAmendment);
		}

		///<summary></summary>
		public static void Delete(long ehrAmendmentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrAmendmentNum);
				return;
			}
			string command= "DELETE FROM ehramendment WHERE EhrAmendmentNum = "+POut.Long(ehrAmendmentNum);
			Db.NonQ(command);
		}
	}
}