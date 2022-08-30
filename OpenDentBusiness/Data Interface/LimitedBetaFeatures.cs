using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.WebTypes;

namespace OpenDentBusiness{
	///<summary>LimitedBetaFeatures show which eServices in beta clinics are allowed to use. </summary>
	public class LimitedBetaFeatures {

		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class LimitedBetaFeatureCache : CacheListAbs<LimitedBetaFeature> {
			protected override List<LimitedBetaFeature> GetCacheFromDb() {
				string command="SELECT * FROM limitedbetafeature";
				return Crud.LimitedBetaFeatureCrud.SelectMany(command);
			}
			protected override List<LimitedBetaFeature> TableToList(DataTable table) {
				return Crud.LimitedBetaFeatureCrud.TableToList(table);
			}
			protected override LimitedBetaFeature Copy(LimitedBetaFeature limitedBetaFeature) {
				return limitedBetaFeature.Copy();
			}
			protected override DataTable ListToTable(List<LimitedBetaFeature> listLimitedBetaFeatures) {
				return Crud.LimitedBetaFeatureCrud.ListToTable(listLimitedBetaFeatures,"LimitedBetaFeature");
			}
			protected override void FillCacheIfNeeded() {
				LimitedBetaFeatures.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LimitedBetaFeatureCache _limitedBetaFeatureCache=new LimitedBetaFeatureCache();

		public static List<LimitedBetaFeature> GetDeepCopy(bool isShort=false) {
			return _limitedBetaFeatureCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _limitedBetaFeatureCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<LimitedBetaFeature> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<LimitedBetaFeature> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetFindIndex(match,isShort);
		}

		public static LimitedBetaFeature GetFirst(bool isShort=false) {
			return _limitedBetaFeatureCache.GetFirst(isShort);
		}

		public static LimitedBetaFeature GetFirst(Func<LimitedBetaFeature,bool> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetFirst(match,isShort);
		}

		public static LimitedBetaFeature GetFirstOrDefault(Func<LimitedBetaFeature,bool> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetFirstOrDefault(match,isShort);
		}

		public static LimitedBetaFeature GetLast(bool isShort=false) {
			return _limitedBetaFeatureCache.GetLast(isShort);
		}

		public static LimitedBetaFeature GetLastOrDefault(Func<LimitedBetaFeature,bool> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetLastOrDefault(match,isShort);
		}

		public static List<LimitedBetaFeature> GetWhere(Predicate<LimitedBetaFeature> match,bool isShort=false) {
			return _limitedBetaFeatureCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_limitedBetaFeatureCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_limitedBetaFeatureCache.FillCacheFromTable(table);
				return table;
			}
			return _limitedBetaFeatureCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		///<summary>Gets one LimitedBetaFeature from the db.</summary>
		public static LimitedBetaFeature GetOne(long limitedBetaFeatureNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<LimitedBetaFeature>(MethodBase.GetCurrentMethod(),limitedBetaFeatureNum);
			}
			return Crud.LimitedBetaFeatureCrud.SelectOne(limitedBetaFeatureNum);
		}

		///<summary>Returns true if the clinic is signed up for limited beta feature, or the feature is marked finished at hq. clinicNum of -1 indicates a clinic independat feature. The 'feature' parameter is the long value of the EServiceFeatureInfoEnum.</summary>
		public static bool IsAllowed(EServiceFeatureInfoEnum feature,long clinicNum=-1) {
			//No need to check MiddleTierRole; no call to db.
			#region Completed Override
			//If this eServiceFeatureInfoEnum has a completed attribute, return true. This acts as an override once features are marked complete.
			EServiceFeatureStatusAttribute attributeFinished=EnumTools.GetAttributeOrDefault<EServiceFeatureStatusAttribute>((EServiceFeatureInfoEnum)feature);
			if(attributeFinished.IsFinished){
				return true; //Overrides any feature status checking.
			}
			#endregion
			LimitedBetaFeature limitedBetaFeature=GetFirstOrDefault(x => x.LimitedBetaFeatureTypeNum==(long)feature && (x.ClinicNum==clinicNum || x.ClinicNum==-1));
			//Implicit that if a limitedBetaFeature is no longer in the list, it is finished and can be displayed / used. Its available to the general user base.
			//If an entry does exist, we need to check if the office is signed up for the feature
			return (limitedBetaFeature?.IsSignedUp??false);
		}

		///<summary>Syncs the loacal LimitedBetaFeature table with the list passed in. Ignores rows with undefined EServiceFeatureInfoEnums.</summary>
		public static void SyncFromHQ(List<LimitedBetaFeature> listLimitedBetaFeaturesHQ) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLimitedBetaFeaturesHQ);
				return;
			}
			bool isCacheInvalid=false;
			//Remove all unclassified features.
			listLimitedBetaFeaturesHQ.RemoveAll(x=>x.GetLimitedBetaFeatureEnum()==EServiceFeatureInfoEnum.None);
			List<LimitedBetaFeature> listLimitedBetaFeaturesDB=_limitedBetaFeatureCache.GetDeepCopy();
			List<LimitedBetaFeature> listToInsert=new List<LimitedBetaFeature>();
			for(int i=0;i<listLimitedBetaFeaturesHQ.Count;i++) {
				LimitedBetaFeature featureOld=listLimitedBetaFeaturesDB
					.FirstOrDefault(x=>x.ClinicNum==listLimitedBetaFeaturesHQ[i].ClinicNum && x.LimitedBetaFeatureTypeNum==listLimitedBetaFeaturesHQ[i].LimitedBetaFeatureTypeNum);
				if(featureOld==null) {
					//Insert if one does not exist
					listToInsert.Add(listLimitedBetaFeaturesHQ[i]);
					isCacheInvalid=true;
				}
				else { 
					//Update if the DB has an entry for the existing feature / clinic combo
					//Set the local PK for the listLimitedBetaFeaturesHq.
					listLimitedBetaFeaturesHQ[i].LimitedBetaFeatureNum=featureOld.LimitedBetaFeatureNum;
					isCacheInvalid|=Crud.LimitedBetaFeatureCrud.Update(listLimitedBetaFeaturesHQ[i],featureOld);
				}
			}
			IEnumerable<long> listDbNums=listLimitedBetaFeaturesDB.Select(x => x.LimitedBetaFeatureNum);
			IEnumerable<long> listHqNums=listLimitedBetaFeaturesHQ.Select(x => x.LimitedBetaFeatureNum);
			List<long> listToDelete=listDbNums.Except(listHqNums).ToList();
			isCacheInvalid|=listToDelete.Count>0;
			//Perform the bulk inserts and deletes.
			Crud.LimitedBetaFeatureCrud.DeleteMany(listToDelete);
			Crud.LimitedBetaFeatureCrud.InsertMany(listToInsert);
			if(isCacheInvalid) {
				Signalods.SetInvalid(InvalidType.LimitedBetaFeature);
				RefreshCache();
			}
		}



	}
}