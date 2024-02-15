using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CanadianNetworks{
		#region Cache Pattern

		private class CanadianNetworkCache : CacheListAbs<CanadianNetwork> {
			protected override List<CanadianNetwork> GetCacheFromDb() {
				string command="SELECT * FROM canadiannetwork ORDER BY Descript";
				return Crud.CanadianNetworkCrud.SelectMany(command);
			}
			protected override List<CanadianNetwork> TableToList(DataTable table) {
				return Crud.CanadianNetworkCrud.TableToList(table);
			}
			protected override CanadianNetwork Copy(CanadianNetwork canadianNetwork) {
				return canadianNetwork.Copy();
			}
			protected override DataTable ListToTable(List<CanadianNetwork> listCanadianNetworks) {
				return Crud.CanadianNetworkCrud.ListToTable(listCanadianNetworks,"CanadianNetwork");
			}
			protected override void FillCacheIfNeeded() {
				CanadianNetworks.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CanadianNetworkCache _canadianNetworkCache=new CanadianNetworkCache();

		public static List<CanadianNetwork> GetDeepCopy(bool isShort=false) {
			return _canadianNetworkCache.GetDeepCopy(isShort);
		}

		public static CanadianNetwork GetFirstOrDefault(Func<CanadianNetwork,bool> match,bool isShort=false) {
			return _canadianNetworkCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_canadianNetworkCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_canadianNetworkCache.FillCacheFromTable(table);
				return table;
			}
			return _canadianNetworkCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(CanadianNetwork canadianNetwork) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				canadianNetwork.CanadianNetworkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),canadianNetwork);
				return canadianNetwork.CanadianNetworkNum;
			}
			return Crud.CanadianNetworkCrud.Insert(canadianNetwork);
		}

		///<summary></summary>
		public static void Update(CanadianNetwork canadianNetwork){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),canadianNetwork);
				return;
			}
			Crud.CanadianNetworkCrud.Update(canadianNetwork);
		}

		///<summary>Uses cache. Give a non-null claim if the transaction is for a claim.</summary>
		public static CanadianNetwork GetNetwork(long networkNum,Clearinghouse clearinghouseClin,Claim claim=null) {
			//No need to check MiddleTierRole; no call to db.
			CanadianNetwork canadianNetwork=GetFirstOrDefault(x => x.CanadianNetworkNum==networkNum);
			//CSI is the previous name for the network now known as INSTREAM.
			//According to Telus 05/18/2023: "TELUS has signed a contract with Instream Canada so that TELUS can send Denturists and Hygienists claims to Instream Canada
			//for carriers defined as Instream, and also, Instream Canada can send Denturists and Hygienists claims to TELUS for carriers defined as TELUS"
			//Dentist claims must not be redirected.
			if(clearinghouseClin.CommBridge==EclaimsCommBridge.Claimstream && canadianNetwork.Abbrev=="CSI" && claim!=null) {
				Provider providerTreat=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat);
				if(providerTreat.NationalProvID.StartsWith("202") || providerTreat.NationalProvID.StartsWith("8")) {//Hygienist or Denturist.
					//Network redirect only allowed for Hygienists or Denturists.
					canadianNetwork=GetFirstOrDefault(x => x.Abbrev=="TELUS B");
				}
			}
			return canadianNetwork;
		}
	}
}













