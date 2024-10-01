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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<FHIRSubscription>(MethodBase.GetCurrentMethod(),fHIRSubscriptionNum);
			}
			return Crud.FHIRSubscriptionCrud.SelectOne(fHIRSubscriptionNum);
		}

		///<summary></summary>
		public static long Insert(FHIRSubscription fHIRSubscription) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				fHIRSubscription.FHIRSubscriptionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),fHIRSubscription);
				return fHIRSubscription.FHIRSubscriptionNum;
			}
			long fHIRSubscriptionNum=Crud.FHIRSubscriptionCrud.Insert(fHIRSubscription);
			for(int i=0;i<fHIRSubscription.ListContactPoints.Count;i++) {
				fHIRSubscription.ListContactPoints[i].FHIRSubscriptionNum=fHIRSubscriptionNum;
				FHIRContactPoints.Insert(fHIRSubscription.ListContactPoints[i]);
			}
			return fHIRSubscriptionNum;
		}

		///<summary></summary>
		public static void Update(FHIRSubscription fHIRSubscription) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscription);
				return;
			}
			string command="SELECT * FROM fhircontactpoint WHERE FHIRSubscriptionNum="+POut.Long(fHIRSubscription.FHIRSubscriptionNum);
			List<FHIRContactPoint> listFHIRContactPoints=Crud.FHIRContactPointCrud.SelectMany(command);
			List<FHIRContactPoint> listFHIRContactPointsFromFHIRSub=fHIRSubscription.ListContactPoints;
			for(int i=0;i<listFHIRContactPointsFromFHIRSub.Count;i++) {
				listFHIRContactPointsFromFHIRSub[i].FHIRSubscriptionNum=fHIRSubscription.FHIRSubscriptionNum;
				if(listFHIRContactPoints.Any(x => x.FHIRContactPointNum==listFHIRContactPointsFromFHIRSub[i].FHIRContactPointNum)) {
					//Update any FHIRContactPoint that already exists in the db
					FHIRContactPoints.Update(listFHIRContactPointsFromFHIRSub[i]);
				}
				else {
					//Insert any FHIRContactPoint that does not exist in the db
					FHIRContactPoints.Insert(listFHIRContactPointsFromFHIRSub[i]);
				}
			}
			//Delete any FHIRContactPoint that exists in the db but not in the new list
			listFHIRContactPoints=listFHIRContactPoints.FindAll(x => !fHIRSubscription.ListContactPoints.Any(y => y.FHIRContactPointNum==x.FHIRContactPointNum));
			for(int i=0;i<listFHIRContactPoints.Count;i++) {
				FHIRContactPoints.Delete(listFHIRContactPoints[i].FHIRContactPointNum);
			}
			Crud.FHIRSubscriptionCrud.Update(fHIRSubscription);
		}

		///<summary></summary>
		public static void Update(FHIRSubscription fHIRSubscription,FHIRSubscription fHIRSubscriptionOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscription,fHIRSubscriptionOld);
				return;
			}
			Crud.FHIRSubscriptionCrud.Update(fHIRSubscription,fHIRSubscriptionOld);
		}

			///<summary></summary>
		public static void Delete(long fHIRSubscriptionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fHIRSubscriptionNum);
				return;
			}
			Crud.FHIRSubscriptionCrud.Delete(fHIRSubscriptionNum);
			string command="DELETE FROM fhircontactpoint WHERE FHIRSubscriptionNum="+POut.Long(fHIRSubscriptionNum);
			Db.NonQ(command);
		}
	}
}