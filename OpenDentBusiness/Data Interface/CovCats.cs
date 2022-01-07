using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class CovCats {
		#region Get Methods
		///<summary>Returns a distinct list of valid ProcCodes for the given eBenefitCat.</summary>
		public static List<string> GetValidCodesForEbenCat(EbenefitCategory eBenefitCat) {
			//No need to check RemotingRole; no call to db.
			List<string> listValidStrings=new List<string>();
			List<CovCat> listCovCats=CovCats.GetWhere(x => x.EbenefitCat==eBenefitCat,true);
			foreach(CovCat covCat in listCovCats) {
				CovSpan[] arrayCovSpans=CovSpans.GetForCat(covCat.CovCatNum);
				listValidStrings.AddRange(
					ProcedureCodes.GetWhere(x => CovSpans.IsCodeInSpans(x.ProcCode,arrayCovSpans),true).Select(x => x.ProcCode).ToList()
				);
			}
			return listValidStrings.Distinct().ToList();
		}
		
		///<summary>Pass in list of procedures and covCat, return the sum of all CanadaTimeUnits of the procedures in that covCat as a double.</summary>
		public static double GetAmtUsedForCat(List<Procedure> listProcs,CovCat covCat) {
			List<ProcedureCode> listProcCodes=new List<ProcedureCode>();
			for(int i=0;i<listProcs.Count;i++) {
				listProcCodes.Add(ProcedureCodes.GetProcCode(listProcs[i].CodeNum));	//turn list of procedures into list of procedurecodes.
			}
			double total=0;//CanadaTimeUnits can be decimal numbers, like 0.5.
			for(int i=0;i<listProcCodes.Count;i++) { //for every procedurecode
				List<CovCat> listCovCatsForProc=GetCovCats(CovSpans.GetCats(listProcCodes[i].ProcCode));
				if(listCovCatsForProc.Any(x => x.CovCatNum==covCat.CovCatNum)) {
					total+=listProcCodes[i].CanadaTimeUnits; //add the Canada time units to the total.
				}
			}
			return total;
		}
		#endregion

		#region Delete
		public static void Delete(CovCat covCat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covCat);
				return;
			}
			string command="DELETE FROM covcat "
				+"WHERE CovCatNum = '"+POut.Long(covCat.CovCatNum)+"'";
			Db.NonQ(command);
		}
		#endregion

		#region CachePattern

		private class CovCatCache : CacheListAbs<CovCat> {
			protected override List<CovCat> GetCacheFromDb() {
				string command="SELECT * FROM covcat ORDER BY CovOrder";
				return Crud.CovCatCrud.SelectMany(command);
			}
			protected override List<CovCat> TableToList(DataTable table) {
				return Crud.CovCatCrud.TableToList(table);
			}
			protected override CovCat Copy(CovCat covCat) {
				return covCat.Copy();
			}
			protected override DataTable ListToTable(List<CovCat> listCovCats) {
				return Crud.CovCatCrud.ListToTable(listCovCats,"CovCat");
			}
			protected override void FillCacheIfNeeded() {
				CovCats.GetTableFromCache(false);
			}
			protected override bool IsInListShort(CovCat covCat) {
				return !covCat.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CovCatCache _CovCatsache=new CovCatCache();

		public static List<CovCat> GetDeepCopy(bool isShort=false) {
			return _CovCatsache.GetDeepCopy(isShort);
		}

		public static int GetFindIndex(Predicate<CovCat> match,bool isShort=false) {
			return _CovCatsache.GetFindIndex(match,isShort);
		}

		public static CovCat GetFirst(bool isShort=false) {
			return _CovCatsache.GetFirst(isShort);
		}

		public static CovCat GetFirstOrDefault(Func<CovCat,bool> match,bool isShort=false) {
			return _CovCatsache.GetFirstOrDefault(match,isShort);
		}

		public static CovCat GetLastOrDefault(Func<CovCat,bool> match,bool isShort=false) {
			return _CovCatsache.GetLastOrDefault(match,isShort);
		}

		public static List<CovCat> GetWhere(Predicate<CovCat> match,bool isShort=false) {
			return _CovCatsache.GetWhere(match,isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _CovCatsache.GetCount(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_CovCatsache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_CovCatsache.FillCacheFromTable(table);
				return table;
			}
			return _CovCatsache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(CovCat covcat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covcat);
				return;
			}
			Crud.CovCatCrud.Update(covcat);
		}

		///<summary></summary>
		public static long Insert(CovCat covcat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				covcat.CovCatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),covcat);
				return covcat.CovCatNum;
			}
			return Crud.CovCatCrud.Insert(covcat);
		}

		///<summary>Does not update the cache.  The cache must be manually refreshed after using this method beccause it only updates the database.</summary>
		public static void MoveUp(CovCat covcat) {
			//No need to check RemotingRole; no call to db.
			List<CovCat> listCovCats=CovCats.GetDeepCopy();
			int oldOrder=listCovCats.FindIndex(x => x.CovCatNum==covcat.CovCatNum);
			if(oldOrder==0 || oldOrder==-1) {
				return;
			}
			SetOrder(listCovCats[oldOrder],(byte)(oldOrder-1));
			SetOrder(listCovCats[oldOrder-1],(byte)oldOrder);
		}

		///<summary>Does not update the cache.  The cache must be manually refreshed after using this method beccause it only updates the database.</summary>
		public static void MoveDown(CovCat covcat) {
			//No need to check RemotingRole; no call to db.
			List<CovCat> listCovCats=CovCats.GetDeepCopy();
			int oldOrder=listCovCats.FindIndex(x => x.CovCatNum==covcat.CovCatNum);
			if(oldOrder==listCovCats.Count-1 || oldOrder==-1) {
				return;
			}
			SetOrder(listCovCats[oldOrder],(byte)(oldOrder+1));
			SetOrder(listCovCats[oldOrder+1],(byte)oldOrder);
		}

		///<summary></summary>
		private static void SetOrder(CovCat covcat, byte newOrder) {
			//No need to check RemotingRole; no call to db.
			covcat.CovOrder=newOrder;
			Update(covcat);
		}

		///<summary></summary>
		public static CovCat GetCovCat(long covCatNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.CovCatNum==covCatNum);
		}
		
		///<summary></summary>
		public static List<CovCat> GetCovCats(List<long> listCovCatNum) {
			return GetWhere(x => ListTools.In(x.CovCatNum,listCovCatNum));
		}

		///<summary></summary>
		public static double GetDefaultPercent(long myCovCatNum) {
			//No need to check RemotingRole; no call to db.
			CovCat covCat=GetFirstOrDefault(x => x.CovCatNum==myCovCatNum);
			return (covCat==null ? 0 : (double)covCat.DefaultPercent);
		}

		///<summary></summary>
		public static string GetDesc(long covCatNum) {
			//No need to check RemotingRole; no call to db.
			CovCat covCat=GetLastOrDefault(x => x.CovCatNum==covCatNum);
			return (covCat==null ? "" : covCat.Description);
		}

		///<summary></summary>
		public static long GetCovCatNum(int orderShort) {
			//No need to check RemotingRole; no call to db.
			CovCat covCat=GetLastOrDefault(x => x.CovOrder==orderShort,true);
			return (covCat==null ? 0 : covCat.CovCatNum);
		}

		///<summary>Returns -1 if not in ListShort.</summary>
		public static int GetOrderShort(long CovCatNum) {
			//No need to check RemotingRole; no call to db.
			return GetFindIndex(x => x.CovCatNum==CovCatNum,true);
		}

		///<summary>Returns -1 if not in the provided list.</summary>
		public static int GetOrderShort(long CovCatNum,List<CovCat> listCovCats) {
			//No need to check RemotingRole; no call to db.
			int retVal=-1;
			for(int i=0;i<listCovCats.Count;i++) {
				if(CovCatNum==listCovCats[i].CovCatNum) {
					retVal=i;
				}
			}
			return retVal;
		}

		///<summary>Gets a matching benefit category from the short list.  Returns null if not found, which should be tested for.</summary>
		public static CovCat GetForEbenCat(EbenefitCategory eben){
			//No need to check RemotingRole; no call to db.
			return CovCats.GetFirstOrDefault(x => x.EbenefitCat==eben,true);
		}

		///<summary>If none assigned, it will return None.</summary>
		public static EbenefitCategory GetEbenCat(long covCatNum) {
			//No need to check RemotingRole; no call to db.
			CovCat covCat=CovCats.GetFirstOrDefault(x => x.CovCatNum==covCatNum,true);
			return (covCat==null ? EbenefitCategory.None : covCat.EbenefitCat);
		}

		public static int CountForEbenCat(EbenefitCategory eben) {
			//No need to check RemotingRole; no call to db.
			return CovCats.GetWhere(x => x.EbenefitCat==eben,true).Count;
		}

		public static void SetOrdersToDefault() {
			//This can only be run if the validation checks have been run first.
			//No need to check RemotingRole; no call to db.
			SetOrder(GetForEbenCat(EbenefitCategory.General),0);
			SetOrder(GetForEbenCat(EbenefitCategory.Diagnostic),1);
			SetOrder(GetForEbenCat(EbenefitCategory.DiagnosticXRay),2);
			SetOrder(GetForEbenCat(EbenefitCategory.RoutinePreventive),3);
			SetOrder(GetForEbenCat(EbenefitCategory.Restorative),4);
			SetOrder(GetForEbenCat(EbenefitCategory.Endodontics),5);
			SetOrder(GetForEbenCat(EbenefitCategory.Periodontics),6);
			SetOrder(GetForEbenCat(EbenefitCategory.OralSurgery),7);
			SetOrder(GetForEbenCat(EbenefitCategory.Crowns),8);
			SetOrder(GetForEbenCat(EbenefitCategory.Prosthodontics),9);
			SetOrder(GetForEbenCat(EbenefitCategory.MaxillofacialProsth),10);
			SetOrder(GetForEbenCat(EbenefitCategory.Accident),11);
			SetOrder(GetForEbenCat(EbenefitCategory.Orthodontics),12);
			SetOrder(GetForEbenCat(EbenefitCategory.Adjunctive),13);
			//now set the remaining categories to come after the ebens.
			byte idx=14;
			List<CovCat> listCovCatsShort=CovCats.GetWhere(x => x.EbenefitCat!=EbenefitCategory.None,true);
			for(int i=0;i<listCovCatsShort.Count;i++) {
				SetOrder(listCovCatsShort[i],idx);
				idx++;
			}
			//finally, the hidden categories
			List<CovCat> listCovCats=CovCats.GetWhere(x => !x.IsHidden);
			for(int i=0;i<listCovCats.Count;i++) {
				SetOrder(listCovCats[i],idx);
				idx++;
			}
		}

		public static void SetSpansToDefault() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				SetSpansToDefaultCanada();
			}
			else {
				SetSpansToDefaultUsa();
			}
		}

		public static void SetSpansToDefaultUsa() {
			//This can only be run if the validation checks have been run first.
			//No need to check RemotingRole; no call to db.
			long covCatNum;
			CovSpan span;
			covCatNum=GetForEbenCat(EbenefitCategory.General).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D0000";
			span.ToCode="D7999";
			CovSpans.Insert(span);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D9000";
			span.ToCode="D9999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D0000";
			span.ToCode="D0999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D0200";
			span.ToCode="D0399";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D1000";
			span.ToCode="D1999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D2000";
			span.ToCode="D2699";
			CovSpans.Insert(span);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D2800";
			span.ToCode="D2999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D3000";
			span.ToCode="D3999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D4000";
			span.ToCode="D4999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D7000";
			span.ToCode="D7999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D2700";
			span.ToCode="D2799";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D5000";
			span.ToCode="D5899";
			CovSpans.Insert(span);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D6200";
			span.ToCode="D6899";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D5900";
			span.ToCode="D5999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Accident).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covCatNum=GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D8000";
			span.ToCode="D8999";
			CovSpans.Insert(span);
			covCatNum=GetForEbenCat(EbenefitCategory.Adjunctive).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			span=new CovSpan();
			span.CovCatNum=covCatNum;
			span.FromCode="D9000";
			span.ToCode="D9999";
			CovSpans.Insert(span);
		}

		public static void SetSpansToDefaultCanada() {
			//This can only be run if the validation checks have been run first.
			//No need to check RemotingRole; no call to db.
			RecreateSpansForCategory(EbenefitCategory.General,"00000-99999");
			RecreateSpansForCategory(EbenefitCategory.Diagnostic,"01000-09999");
			RecreateSpansForCategory(EbenefitCategory.DiagnosticXRay,"02000-02999");
			RecreateSpansForCategory(EbenefitCategory.RoutinePreventive,"10000-19999");
			RecreateSpansForCategory(EbenefitCategory.Restorative,"20000-26999","28000-29999");
			RecreateSpansForCategory(EbenefitCategory.Crowns,"27000-27999");
			RecreateSpansForCategory(EbenefitCategory.Endodontics,"30000-39999");
			RecreateSpansForCategory(EbenefitCategory.Periodontics,"40000-49999");
			RecreateSpansForCategory(EbenefitCategory.Prosthodontics,"50000-56999","58000-69999");
			RecreateSpansForCategory(EbenefitCategory.MaxillofacialProsth,"57000-57999");
			RecreateSpansForCategory(EbenefitCategory.OralSurgery,"70000-79999");
			RecreateSpansForCategory(EbenefitCategory.Orthodontics,"80000-89999");
			RecreateSpansForCategory(EbenefitCategory.Adjunctive,"90000-99999");
			RecreateSpansForCategory(EbenefitCategory.Accident);
		}

		///<summary>Deletes the current CovSpans for the given eBenefitCategory, then creates new code ranges from the ranges specified in arrayCodeRanges.  The values in arrayCodeRanges can be a single code such as "D0120" or a code range such as "D9000-D9999".</summary>
		private static void RecreateSpansForCategory(EbenefitCategory eBenefitCategory,params string[] arrayCodeRanges) {
			long covCatNum=GetForEbenCat(eBenefitCategory).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			for(int i=0;i<arrayCodeRanges.Length;i++) {
				string codeRange=arrayCodeRanges[i];
				CovSpan span=new CovSpan();
				span.CovCatNum=covCatNum;
				if(codeRange.Contains("-")) {//Code range
					span.FromCode=codeRange.Substring(0,codeRange.IndexOf("-"));
					span.ToCode=codeRange.Substring(span.FromCode.Length+1);
				}
				else {//Single code
					span.FromCode=codeRange;
					span.ToCode=codeRange;
				}
				CovSpans.Insert(span);
			}
		}
	}

	



}









