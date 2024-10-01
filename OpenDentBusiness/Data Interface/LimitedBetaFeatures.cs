using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.WebTypes;

namespace OpenDentBusiness{
	///<summary>LimitedBetaFeatures show which beta eServices clinics are allowed to use. </summary>
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

		public static void ClearCache() {
			_limitedBetaFeatureCache.ClearCache();
		}
		#endregion Cache Pattern

		///<summary>Gets one LimitedBetaFeature from the db.</summary>
		public static LimitedBetaFeature GetOne(long limitedBetaFeatureNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<LimitedBetaFeature>(MethodBase.GetCurrentMethod(),limitedBetaFeatureNum);
			}
			return Crud.LimitedBetaFeatureCrud.SelectOne(limitedBetaFeatureNum);
		}

		///<summary>Returns true if the clinic is signed up for limited beta feature, or the feature is marked finished at hq. 
		///This does not guarantee that the clinic should have access to the feature. Only that the limited beta restriction is met for this clinic.
		///If a given feature typically requires further validation (usually via prefs or HQ validation) then you must perform that validation after this check.
		/// clinicNum of -1 indicates a clinic independent feature. 
		/// The 'feature' parameter is the long value of the EServiceFeatureInfoEnum. 
		public static bool IsAllowed(EServiceFeatureInfoEnum eServiceFeatureInfoEnum,long clinicNum=-1) {
			Meth.NoCheckMiddleTierRole();
			#region Completed Override
			//If this eServiceFeatureInfoEnum has a completed attribute, return true. This acts as an override once features are marked complete.
			EServiceFeatureStatusAttribute eServiceFeatureStatusAttribute=EnumTools.GetAttributeOrDefault<EServiceFeatureStatusAttribute>((EServiceFeatureInfoEnum)eServiceFeatureInfoEnum);
			if(eServiceFeatureStatusAttribute.IsFinished){
				return true; //Overrides any feature status checking.
			}
			#endregion
			LimitedBetaFeature limitedBetaFeature=GetFirstOrDefault(x => x.LimitedBetaFeatureTypeNum==(long)eServiceFeatureInfoEnum && (x.ClinicNum==clinicNum || x.ClinicNum==-1));
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
			List<LimitedBetaFeature> listLimitedBetaFeaturesToInsert=new List<LimitedBetaFeature>();
			for(int i=0;i<listLimitedBetaFeaturesHQ.Count;i++) {
				LimitedBetaFeature limitedBetaFeatureOld=listLimitedBetaFeaturesDB
					.FirstOrDefault(x=>x.ClinicNum==listLimitedBetaFeaturesHQ[i].ClinicNum && x.LimitedBetaFeatureTypeNum==listLimitedBetaFeaturesHQ[i].LimitedBetaFeatureTypeNum);
				if(limitedBetaFeatureOld==null) {
					//Insert if one does not exist
					listLimitedBetaFeaturesToInsert.Add(listLimitedBetaFeaturesHQ[i]);
					isCacheInvalid=true;
				}
				else { 
					//Update if the DB has an entry for the existing feature / clinic combo
					//Set the local PK for the listLimitedBetaFeaturesHq.
					listLimitedBetaFeaturesHQ[i].LimitedBetaFeatureNum=limitedBetaFeatureOld.LimitedBetaFeatureNum;
					isCacheInvalid|=Crud.LimitedBetaFeatureCrud.Update(listLimitedBetaFeaturesHQ[i],limitedBetaFeatureOld);
				}
			}
			IEnumerable<long> listLimitedBetaFeatureNumsDb=listLimitedBetaFeaturesDB.Select(x => x.LimitedBetaFeatureNum);
			IEnumerable<long> listLimitedBetaFeatureNumsHq=listLimitedBetaFeaturesHQ.Select(x => x.LimitedBetaFeatureNum);
			List<long> listLimitedBetaFeatureNumsToDelete=listLimitedBetaFeatureNumsDb.Except(listLimitedBetaFeatureNumsHq).ToList();
			isCacheInvalid|=listLimitedBetaFeatureNumsToDelete.Count>0;
			//Perform the bulk inserts and deletes.
			Crud.LimitedBetaFeatureCrud.DeleteMany(listLimitedBetaFeatureNumsToDelete);
			Crud.LimitedBetaFeatureCrud.InsertMany(listLimitedBetaFeaturesToInsert);
			if(isCacheInvalid) {
				Signalods.SetInvalid(InvalidType.LimitedBetaFeature);
				RefreshCache();
			}
		}



	}
}