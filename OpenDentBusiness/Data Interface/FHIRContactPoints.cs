using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FHIRContactPoints {
		public static List<FHIRContactPoint> GetContactPoints(long fHIRSubscriptionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FHIRContactPoint>>(MethodBase.GetCurrentMethod(),fHIRSubscriptionNum);
			}
			string command="SELECT * FROM fhircontactpoint WHERE FHIRSubscriptionNum="+POut.Long(fHIRSubscriptionNum);
			return FHIRContactPointCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(FHIRContactPoint fHIRContactPoint) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				fHIRContactPoint.FHIRContactPointNum=Meth.GetLong(MethodBase.GetCurrentMethod(),fHIRContactPoint);
				return fHIRContactPoint.FHIRContactPointNum;
			}
			return Crud.FHIRContactPointCrud.Insert(fHIRContactPoint);
		}

		///<summary></summary>
		public static void Update(FHIRContactPoint fHIRContactPoint) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRContactPoint);
				return;
			}
			Crud.FHIRContactPointCrud.Update(fHIRContactPoint);
		}

		///<summary></summary>
		public static void Delete(long fHIRContactPointNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRContactPointNum);
				return;
			}
			Crud.FHIRContactPointCrud.Delete(fHIRContactPointNum);
		}
	}
}