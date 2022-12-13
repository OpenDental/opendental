using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Utms{
		#region Methods - Get
		///<summary></summary>
		//public static List<Utm> Refresh(long patNum){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetObject<List<Utm>>(MethodBase.GetCurrentMethod(),patNum);
		//	}
		//	string command="SELECT * FROM utm WHERE PatNum = "+POut.Long(patNum);
		//	return Crud.UtmCrud.SelectMany(command);
		//}

		//<summary>Gets one Utm from the db.</summary>
		//public static Utm GetOne(long utmNum){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		return Meth.GetObject<Utm>(MethodBase.GetCurrentMethod(),utmNum);
		//	}
		//	return Crud.UtmCrud.SelectOne(utmNum);
		//}

		///<summary>Gets one Utm from the db or inserts a row if it doesn't exist.</summary>
		public static long GetOrInsert(string CampaignName="",string MediumInfo="",string SourceInfo="") {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),CampaignName,MediumInfo,SourceInfo);
			}
			if(string.IsNullOrEmpty(CampaignName) 
				&& string.IsNullOrEmpty(MediumInfo)
				&& string.IsNullOrEmpty(SourceInfo))
			{
				throw new ODException(Lans.g("Utm","Must have at least one string parameter that is not empty."));
			}
			string command="SELECT UtmNum FROM utm" +
				" WHERE CampaignName='"+POut.String(CampaignName)+"'"+
				" AND MediumInfo='"+POut.String(MediumInfo)+"'"+
				" AND SourceInfo='"+POut.String(SourceInfo)+"'";
			long primaryKey=Db.GetLong(command);
			if(primaryKey==0) {
				Utm utm=new Utm();
				utm.CampaignName=CampaignName;
				utm.MediumInfo=MediumInfo;
				utm.SourceInfo=SourceInfo;
				return Crud.UtmCrud.Insert(utm);
			}
			else {
				return primaryKey;
			}
		}
		#endregion Methods - Get
		#region Methods - Modify
		/////<summary></summary>
		//public static long Insert(Utm utm){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		utm.UtmNum=Meth.GetLong(MethodBase.GetCurrentMethod(),utm);
		//		return utm.UtmNum;
		//	}
		//	return Crud.UtmCrud.Insert(utm);
		//}

		/////<summary></summary>
		//public static void Update(Utm utm){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),utm);
		//		return;
		//	}
		//	Crud.UtmCrud.Update(utm);
		//}

		/////<summary></summary>
		//public static void Delete(long utmNum) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),utmNum);
		//		return;
		//	}
		//	Crud.UtmCrud.Delete(utmNum);
		//}
		#endregion Methods - Modify
		#region Methods - Misc
		#endregion Methods - Misc




	}
}