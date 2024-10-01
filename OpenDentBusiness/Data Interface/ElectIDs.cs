using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace OpenDentBusiness{

	public class ElectIDs{
		#region CachePattern

		private class ElectIDCache : CacheListAbs<ElectID> {
			protected override List<ElectID> GetCacheFromDb() {
				string command="SELECT * from electid ORDER BY CarrierName";
				return Crud.ElectIDCrud.SelectMany(command);
			}
			protected override List<ElectID> TableToList(DataTable table) {
				return Crud.ElectIDCrud.TableToList(table);
			}
			protected override ElectID Copy(ElectID electID) {
				return electID.Copy();
			}
			protected override DataTable ListToTable(List<ElectID> listElectIDs) {
				return Crud.ElectIDCrud.ListToTable(listElectIDs,"ElectID");
			}
			protected override void FillCacheIfNeeded() {
				ElectIDs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ElectIDCache _electIDCache=new ElectIDCache();

		public static List<ElectID> GetDeepCopy(bool isShort=false) {
			return _electIDCache.GetDeepCopy(isShort);
		}

		private static ElectID GetFirstOrDefault(Func<ElectID,bool> match,bool isShort=false) {
			return _electIDCache.GetFirstOrDefault(match,isShort);
		}

		public static List<ElectID> GetWhere(Predicate<ElectID> match,bool isShort=false) {
			return _electIDCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_electIDCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_electIDCache.FillCacheFromTable(table);
				return table;
			}
			return _electIDCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_electIDCache.ClearCache();
		}
		#endregion

		public static long Insert(ElectID electID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				electID.ElectIDNum=Meth.GetLong(MethodBase.GetCurrentMethod(),electID);
				return electID.ElectIDNum;
			}
			return Crud.ElectIDCrud.Insert(electID);
		}

		public static void Update(ElectID electID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),electID);
				return;
			}
			Crud.ElectIDCrud.Update(electID);
		}

		public static bool Update(ElectID electIDNew,ElectID electIDOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),electIDNew,electIDOld);
			}
			return Crud.ElectIDCrud.Update(electIDNew,electIDOld);
		}

		///<summary>Takes a list of PayorIDs from DxC's getPayerListService API method. Inserts/updates new or existing electids.</summary>
		public static void UpsertFromDentalXChange(List<Dentalxchange2016.supportedTransPayer> listSupportedTransPayers) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSupportedTransPayers);
				return;
			}
			bool hasChanged=false;
			for(int i=0;i<listSupportedTransPayers.Count;i++) {
				Dentalxchange2016.supportedTransPayer supportedTransPayer=listSupportedTransPayers[i];
				ElectID electID=GetFirstOrDefault(x=>x.PayorID==supportedTransPayer.PayerIDCode && x.CarrierName==supportedTransPayer.Name && x.CommBridge==EclaimsCommBridge.ClaimConnect);
				if(electID is null) {
					electID=new ElectID();
					electID.CarrierName=supportedTransPayer.Name;
					electID.PayorID=supportedTransPayer.PayerIDCode;
					electID.CommBridge=EclaimsCommBridge.ClaimConnect;
					electID.Attributes=String.Join(",",Eclaims.ClaimConnect.GetAttributes(supportedTransPayer).Select(x => (int)x));
					Insert(electID);
					hasChanged=true;
					continue;
				}
				ElectID electIDOld=electID.Copy();
				electID.Attributes=String.Join(",",Eclaims.ClaimConnect.GetAttributes(supportedTransPayer).Select(x => (int)x));
				hasChanged|=Update(electID,electIDOld);
			}
			if(hasChanged) {
				Signalods.SetInvalid(InvalidType.ElectIDs);
			}
		}
		
		///<summary>Takes a list of PayorIDs from EDS's List_Payers API method. Inserts/updates new or existing electids.</summary>
		public static void UpsertFromEDS(List<IdNameAttributes> listIdNameAttributess) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listIdNameAttributess);
				return;
			}
			bool hasChanged=false;
			for(int i=0;i<listIdNameAttributess.Count;i++) {
				IdNameAttributes idNameAttributes=listIdNameAttributess[i];
				string payorID=idNameAttributes.ID;
				string name=idNameAttributes.Name;
				string attributes=idNameAttributes.Attributes;
				if(payorID=="NULL") { //EDS may send over an empty electronic id with "NULL" as the payer id.
					continue;
				}
				ElectID electID=GetFirstOrDefault(x=>x.PayorID==payorID && x.CarrierName==name && x.CommBridge==EclaimsCommBridge.EDS);
				if(electID is null) {
					electID=new ElectID();
					electID.PayorID=payorID;
					electID.CarrierName=name;
					electID.CommBridge=EclaimsCommBridge.EDS;
					electID.Attributes=attributes;
					Insert(electID);
					hasChanged=true;
					continue;
				}
				ElectID electIDOld=electID.Copy();
				electID.CarrierName=name;
				electID.PayorID=payorID;
				electID.Attributes=attributes;
				hasChanged|=Update(electID,electIDOld);
			}
			if(hasChanged) {
				Signalods.SetInvalid(InvalidType.ElectIDs);
			}
		}

		///<summary></summary>
		public static List<ProviderSupplementalID> GetRequiredIdents(string payorID){
			Meth.NoCheckMiddleTierRole();
			ElectID electID=GetID(payorID);
			if(electID==null){
				return new List<ProviderSupplementalID>();
			}
			if(electID.ProviderTypes==""){
				return new List<ProviderSupplementalID>();
			}
			List<string> listProvTypes=electID.ProviderTypes.Split(',').ToList();
			if(listProvTypes.Count==0){
				return new List<ProviderSupplementalID>();
			}
			List<ProviderSupplementalID> listProviderSupplementalIDsRet=new List<ProviderSupplementalID>();
			for(int i=0;i<listProvTypes.Count;i++){
				listProviderSupplementalIDsRet[i]=(ProviderSupplementalID)(Convert.ToInt32(listProvTypes[i]));
			}
			/*
			if(electID=="SB601"){//BCBS of GA
				retVal=new ProviderSupplementalID[2];
				retVal[0]=ProviderSupplementalID.BlueShield;
				retVal[1]=ProviderSupplementalID.SiteNumber;
			}*/
			return listProviderSupplementalIDsRet;
		}

		///<summary>Gets ONE ElectID that uses the supplied payorID. Even if there are multiple payors using that ID.  So use this carefully.</summary>
		public static ElectID GetID(string payorID){
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.PayorID==payorID);
		}

		///<summary>Gets an arrayList of ElectID objects based on a supplied payorID. If no matches found, then returns array of 0 length. Used to display payors in FormInsPlan and also to get required idents.  This means that all payors with the same ID should have the same required idents and notes.</summary>
		public static List<ElectID> GetIDs(string payorID) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.PayorID==payorID);
		}

		///<summary>Gets the names of the payors to display based on the payorID.  Since carriers sometimes share payorIDs, there will often be multiple payor names returned.</summary>
		public static List<string> GetDescripts(string payorID){
			Meth.NoCheckMiddleTierRole();
			if(payorID==""){
				return new List<string>();
			}
			return GetIDs(payorID).Select(x => x.CarrierName).ToList();
		}

		public static bool IsMedicaid(string payorID) {
			Meth.NoCheckMiddleTierRole();
			ElectID electID=GetID(payorID);
			if(electID==null) {
				return false;
			}
			return electID.IsMedicaid;
		}
	}
	
	[Serializable]
	public class IdNameAttributes {
		public string ID;
		public string Name;
		public string Attributes;
	}

}










