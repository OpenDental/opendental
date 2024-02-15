using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using OpenDental;
using System.Linq;

namespace UnitTestsCore {

	public class ClaimTrackingT {

		///<summary>Clear out the table for future test runs.</summary>
		public static void ClearClaimTrackingTable() {
			string command="DELETE FROM claimtracking";
			DataCore.NonQ(command);
		}

		///<summary>Creates and inserts a new ClaimTracking for a passed in claim.</summary>
		public static ClaimTracking CreateClaimTracking(Claim claim,OpenDentBusiness.ClaimTrackingType claimTrackingType=OpenDentBusiness.ClaimTrackingType.StatusHistory,
			long userNum=0,string note="",long trackingDefNum=0,long trackingErrorDefNum=0)
		{
			OpenDentBusiness.ClaimTracking claimTracking=new OpenDentBusiness.ClaimTracking();
			claimTracking.ClaimNum=claim.ClaimNum;
			claimTracking.TrackingType=claimTrackingType;
			claimTracking.UserNum=userNum;
			claimTracking.Note=note;
			claimTracking.TrackingDefNum=trackingDefNum;
			claimTracking.TrackingErrorDefNum=trackingErrorDefNum;
			claimTracking.ClaimTrackingNum=OpenDentBusiness.ClaimTrackings.Insert(claimTracking);
			return claimTracking;
		}

		///<summary>DateTimeEntry cannot normally be set manually. For testing purposes only.</summary>
		public static void UpdateClaimTrackingDateTimeEntry(ClaimTracking claimTracking,DateTime dateTime) {
			string command="UPDATE claimtracking SET DateTimeEntry="+POut.DateT(dateTime)+" "
				+"WHERE ClaimTrackingNum = "+POut.Long(claimTracking.ClaimTrackingNum);
			DataCore.NonQ(command);
		}

	}
}
