using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CovSpans {
		#region CachePattern

		private class CovSpanCache : CacheListAbs<CovSpan> {
			protected override List<CovSpan> GetCacheFromDb() {
				string command="SELECT * FROM covspan ORDER BY FromCode";
				return Crud.CovSpanCrud.SelectMany(command);
			}
			protected override List<CovSpan> TableToList(DataTable table) {
				return Crud.CovSpanCrud.TableToList(table);
			}
			protected override CovSpan Copy(CovSpan covSpan) {
				return covSpan.Copy();
			}
			protected override DataTable ListToTable(List<CovSpan> listCovSpans) {
				return Crud.CovSpanCrud.ListToTable(listCovSpans,"CovSpan");
			}
			protected override void FillCacheIfNeeded() {
				CovSpans.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CovSpanCache _covSpanCache=new CovSpanCache();

		public static List<CovSpan> GetWhere(Predicate<CovSpan> match,bool isShort=false) {
			return _covSpanCache.GetWhere(match,isShort);
		}

		public static List<CovSpan> GetDeepCopy(bool isShort=false) {
			return _covSpanCache.GetDeepCopy(isShort);
		}

		public static CovSpan GetLastOrDefault(Func<CovSpan,bool> match,bool isShort=false) {
			return _covSpanCache.GetLastOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_covSpanCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_covSpanCache.FillCacheFromTable(table);
				return table;
			}
			return _covSpanCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_covSpanCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(CovSpan covSpan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covSpan);
				return;
			}
			Validate(covSpan);
			Crud.CovSpanCrud.Update(covSpan);
			return;
		}

		///<summary></summary>
		public static long Insert(CovSpan covSpan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				covSpan.CovSpanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),covSpan);
				return covSpan.CovSpanNum;
			}
			Validate(covSpan);
			return Crud.CovSpanCrud.Insert(covSpan);
		}

		///<summary></summary>
		private static void Validate(CovSpan covSpan){
			//No need to check MiddleTierRole; no call to db.
			if(covSpan.FromCode=="" || covSpan.ToCode=="") {
				throw new ApplicationException(Lans.g("FormInsSpanEdit","Codes not allowed to be blank."));
			}
			if(String.Compare(covSpan.ToCode,covSpan.FromCode)<0){
				throw new ApplicationException(Lans.g("FormInsSpanEdit","From Code must be less than To Code.  Remember that the comparison is alphabetical, not numeric.  For instance, 100 would come before 2, but after 02."));
			}
		}

		///<summary></summary>
		public static void Delete(CovSpan covSpan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covSpan);
				return;
			}
			string command="DELETE FROM covspan"
				+" WHERE CovSpanNum = '"+POut.Long(covSpan.CovSpanNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForCat(long covCatNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covCatNum);
				return;
			}
			string command="DELETE FROM covspan WHERE CovCatNum = "+POut.Long(covCatNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long GetCat(string myCode){
			//No need to check MiddleTierRole; no call to db.
			CovSpan covSpan=GetLastOrDefault(x => String.Compare(myCode,x.FromCode)>=0
					&& String.Compare(myCode,x.ToCode)<=0);
			if(covSpan==null){
				return 0;
			}
			return covSpan.CovCatNum;
		}

		public static List<long> GetCats(string myCode) {
			//No need to check MiddleTierRole; no call to db.
			List<long> listCovCatNums=GetWhere(x => String.Compare(myCode,x.FromCode)>=0
				&& String.Compare(myCode,x.ToCode)<=0).Select(x => x.CovCatNum).ToList();
			return listCovCatNums;
		}

		///<summary></summary>
		public static List<CovSpan> GetForCat(long covCatNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.CovCatNum==covCatNum);
		}

		///<summary>If the supplied code falls within any of the supplied spans, then returns true.</summary>
		public static bool IsCodeInSpans(string strProcCode,List<CovSpan> listCovSpans) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listCovSpans.Count;i++) {
				if(String.Compare(strProcCode,listCovSpans[i].FromCode)>=0
					&& String.Compare(strProcCode,listCovSpans[i].ToCode)<=0) 
				{
					return true;
				}
			}
			return false;
		}
	}

	


}









