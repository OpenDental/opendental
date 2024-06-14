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
		public static long GetOrInsert(string campaignName="",string mediumInfo="",string sourceInfo="") {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),campaignName,mediumInfo,sourceInfo);
			}
			if(string.IsNullOrEmpty(campaignName) 
				&& string.IsNullOrEmpty(mediumInfo)
				&& string.IsNullOrEmpty(sourceInfo))
			{
				throw new ODException(Lans.g("Utm","Must have at least one string parameter that is not empty."));
			}
			string command="SELECT UtmNum FROM utm" +
				" WHERE CampaignName='"+POut.String(campaignName)+"'"+
				" AND MediumInfo='"+POut.String(mediumInfo)+"'"+
				" AND SourceInfo='"+POut.String(sourceInfo)+"'";
			long utmNum=Db.GetLong(command);
			if(utmNum!=0) {
				return utmNum;
			}
			Utm utm=new Utm();
			utm.CampaignName=campaignName;
			utm.MediumInfo=mediumInfo;
			utm.SourceInfo=sourceInfo;
			return Crud.UtmCrud.Insert(utm);
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