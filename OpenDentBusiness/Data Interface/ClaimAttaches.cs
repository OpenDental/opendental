using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimAttaches{
		public static long Insert(ClaimAttach attach) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				attach.ClaimAttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),attach);
				return attach.ClaimAttachNum;
			}
			return Crud.ClaimAttachCrud.Insert(attach);
		}
	}

	


}









