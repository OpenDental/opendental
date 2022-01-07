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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_covSpanCache.FillCacheFromTable(table);
				return table;
			}
			return _covSpanCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(CovSpan span) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),span);
				return;
			}
			Validate(span);
			Crud.CovSpanCrud.Update(span);
			return;
		}

		///<summary></summary>
		public static long Insert(CovSpan span) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				span.CovSpanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),span);
				return span.CovSpanNum;
			}
			Validate(span);
			return Crud.CovSpanCrud.Insert(span);
		}

		///<summary></summary>
		private static void Validate(CovSpan span){
			//No need to check RemotingRole; no call to db.
			if(span.FromCode=="" || span.ToCode=="") {
				throw new ApplicationException(Lans.g("FormInsSpanEdit","Codes not allowed to be blank."));
			}
			if(String.Compare(span.ToCode,span.FromCode)<0){
				throw new ApplicationException(Lans.g("FormInsSpanEdit","From Code must be less than To Code.  Remember that the comparison is alphabetical, not numeric.  For instance, 100 would come before 2, but after 02."));
			}
		}

		///<summary></summary>
		public static void Delete(CovSpan span) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),span);
				return;
			}
			string command="DELETE FROM covspan"
				+" WHERE CovSpanNum = '"+POut.Long(span.CovSpanNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForCat(long covCatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covCatNum);
				return;
			}
			string command="DELETE FROM covspan WHERE CovCatNum = "+POut.Long(covCatNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long GetCat(string myCode){
			//No need to check RemotingRole; no call to db.
			CovSpan covSpan=GetLastOrDefault(x => String.Compare(myCode,x.FromCode)>=0
					&& String.Compare(myCode,x.ToCode)<=0);
			return (covSpan==null ? 0 : covSpan.CovCatNum);
		}

		public static List<long> GetCats(string myCode) {
			return GetWhere(x => String.Compare(myCode,x.FromCode)>=0
				&& String.Compare(myCode,x.ToCode)<=0).Select(x => x.CovCatNum).ToList();
		}

		///<summary></summary>
		public static CovSpan[] GetForCat(long catNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.CovCatNum==catNum).ToArray();
		}

		///<summary>If the supplied code falls within any of the supplied spans, then returns true.</summary>
		public static bool IsCodeInSpans(string strProcCode,CovSpan[] covSpanArray) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<covSpanArray.Length;i++) {
				if(String.Compare(strProcCode,covSpanArray[i].FromCode)>=0
					&& String.Compare(strProcCode,covSpanArray[i].ToCode)<=0) {
					return true;
				}
			}
			return false;
		}
	}

	


}









