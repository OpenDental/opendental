using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ReferralClinicLinks{
		#region Methods - Get
		public static List<ReferralClinicLink> GetAllForClinic(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReferralClinicLink>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM referralcliniclink WHERE ClinicNum="+POut.Long(clinicNum);
			return Crud.ReferralClinicLinkCrud.SelectMany(command);
		}

		public static List<ReferralClinicLink> GetAllForReferral(long referralNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReferralClinicLink>>(MethodBase.GetCurrentMethod(),referralNum);
			}
			string command="SELECT * FROM referralcliniclink WHERE ReferralNum="+POut.Long(referralNum);
			return Crud.ReferralClinicLinkCrud.SelectMany(command);
		}

		public static List<long> GetReferralNumsWithLinks() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT DISTINCT ReferralNum FROM referralcliniclink";
			return Db.GetListLong(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary>Creates new referral links for the clinics passed in. Optionally delete all existing links for the referral passed in.</summary>
		public static void InsertClinicLinksForReferral(long referralNum,List<long> listClinicNums,bool doDeleteOldLinks=false) {
			if(listClinicNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),referralNum,listClinicNums,doDeleteOldLinks);
				return;
			}
			if(doDeleteOldLinks) {
				string command="DELETE FROM referralcliniclink WHERE ReferralNum="+POut.Long(referralNum);
				Db.NonQ(command);
			}
			listClinicNums.RemoveAll(x => x==0);
			List<ReferralClinicLink> listRefClinicLinks=listClinicNums.Select(x => new ReferralClinicLink(){ClinicNum=x,ReferralNum=referralNum}).ToList();
			Crud.ReferralClinicLinkCrud.InsertMany(listRefClinicLinks);
		}

		///<summary></summary>
		//public static long Insert(ReferralClinicLink referralLink) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		referralLink.ReferralLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),referralLink);
		//		return referralLink.ReferralLinkNum;
		//	}
		//	return Crud.ReferralClinicLinkCrud.Insert(referralLink);
		//}

		/////<summary></summary>
		//public static void Update(ReferralClinicLink referralClinicLink){
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),referralClinicLink);
		//		return;
		//	}
		//	Crud.ReferralClinicLinkCrud.Update(referralClinicLink);
		//}

		/////<summary></summary>
		//public static void Delete(long referralClinicLinkNum) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),referralClinicLinkNum);
		//		return;
		//	}
		//	Crud.ReferralClinicLinkCrud.Delete(referralClinicLinkNum);
		//}

		/////<summary></summary>
		//public static void DeleteAllForReferral(List<long> listReferralClinicLinkNums) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),listReferralClinicLinkNums);
		//		return;
		//	}
		//	Crud.ReferralClinicLinkCrud.DeleteMany(listReferralClinicLinkNums);
		//}
		#endregion Methods - Modify
	}
}