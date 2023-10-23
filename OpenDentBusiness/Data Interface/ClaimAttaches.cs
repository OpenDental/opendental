using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimAttaches{
		public static long Insert(ClaimAttach claimAttach) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				claimAttach.ClaimAttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),claimAttach);
				return claimAttach.ClaimAttachNum;
			}
			return Crud.ClaimAttachCrud.Insert(claimAttach);
		}
	}

	


}









