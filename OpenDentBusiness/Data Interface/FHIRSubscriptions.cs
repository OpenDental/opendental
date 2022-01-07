using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FHIRSubscriptions{
		///<summary>Gets one FHIRSubscription from the db.</summary>
		public static FHIRSubscription GetOne(long fHIRSubscriptionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<FHIRSubscription>(MethodBase.GetCurrentMethod(),fHIRSubscriptionNum);
			}
			return Crud.FHIRSubscriptionCrud.SelectOne(fHIRSubscriptionNum);
		}

		///<summary></summary>
		public static long Insert(FHIRSubscription fHIRSubscription) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				fHIRSubscription.FHIRSubscriptionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),fHIRSubscription);
				return fHIRSubscription.FHIRSubscriptionNum;
			}
			long fHIRSubscriptionNum=Crud.FHIRSubscriptionCrud.Insert(fHIRSubscription);
			foreach(FHIRContactPoint fHIRContactPoint in fHIRSubscription.ListContactPoints) {
				fHIRContactPoint.FHIRSubscriptionNum=fHIRSubscriptionNum;
				FHIRContactPoints.Insert(fHIRContactPoint);
			}
			return fHIRSubscriptionNum;
		}

		///<summary></summary>
		public static void Update(FHIRSubscription fHIRSubscription) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscription);
				return;
			}
			string command="SELECT * FROM fhircontactpoint WHERE FHIRSubscriptionNum="+POut.Long(fHIRSubscription.FHIRSubscriptionNum);
			List<FHIRContactPoint> listDbOld=Crud.FHIRContactPointCrud.SelectMany(command);
			foreach(FHIRContactPoint contactPoint in fHIRSubscription.ListContactPoints) {
				contactPoint.FHIRSubscriptionNum=fHIRSubscription.FHIRSubscriptionNum;
				if(listDbOld.Any(x => x.FHIRContactPointNum==contactPoint.FHIRContactPointNum)) {
					//Update any FHIRContactPoint that already exists in the db
					FHIRContactPoints.Update(contactPoint);
				}
				else {
					//Insert any FHIRContactPoint that does not exist in the db
					FHIRContactPoints.Insert(contactPoint);
				}
			}			
			//Delete any FHIRContactPoint that exists in the db but not in the new list
			listDbOld.FindAll(x => !fHIRSubscription.ListContactPoints.Any(y => y.FHIRContactPointNum==x.FHIRContactPointNum))
				.ForEach(x => FHIRContactPoints.Delete(x.FHIRContactPointNum));
			Crud.FHIRSubscriptionCrud.Update(fHIRSubscription);
		}

		///<summary></summary>
		public static void Update(FHIRSubscription fHIRSubscription,FHIRSubscription fHIRSubscriptionOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscription,fHIRSubscriptionOld);
				return;
			}
			Crud.FHIRSubscriptionCrud.Update(fHIRSubscription,fHIRSubscriptionOld);
		}

			///<summary></summary>
		public static void Delete(long fHIRSubscriptionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscriptionNum);
				return;
			}
			Crud.FHIRSubscriptionCrud.Delete(fHIRSubscriptionNum);
			string command="DELETE FROM fhircontactpoint WHERE FHIRSubscriptionNum="+POut.Long(fHIRSubscriptionNum);
			Db.NonQ(command);
		}
	}
}