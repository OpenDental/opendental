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
		public static List<string> GetValidCodesForEbenCat(EbenefitCategory ebenefitCategory) {
			Meth.NoCheckMiddleTierRole();
			List<string> listStringsValid=new List<string>();
			List<CovCat> listCovCats=CovCats.GetWhere(x => x.EbenefitCat==ebenefitCategory,isShort:true);
			for(int i=0;i<listCovCats.Count;i++){
				List<CovSpan> listCovSpans=CovSpans.GetForCat(listCovCats[i].CovCatNum);
				listStringsValid.AddRange(
					ProcedureCodes.GetWhere(x => CovSpans.IsCodeInSpans(x.ProcCode,listCovSpans),isShort:true).Select(x => x.ProcCode).ToList()
				);
			}
			return listStringsValid.Distinct().ToList();
		}
		
		///<summary>Pass in list of procedures and covCat, return the sum of all CanadaTimeUnits of the procedures in that covCat as a double.</summary>
		public static double GetAmtUsedForCat(List<Procedure> listProcedures,CovCat covCat) {
			Meth.NoCheckMiddleTierRole();
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			for(int i=0;i<listProcedures.Count;i++) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(listProcedures[i].CodeNum));	//turn list of procedures into list of procedurecodes.
			}
			double total=0;//CanadaTimeUnits can be decimal numbers, like 0.5.
			for(int i=0;i<listProcedureCodes.Count;i++) { //for every procedurecode
				List<CovCat> listCovCatsForProc=GetCovCats(CovSpans.GetCats(listProcedureCodes[i].ProcCode));
				if(listCovCatsForProc.Any(x => x.CovCatNum==covCat.CovCatNum)) {
					total+=listProcedureCodes[i].CanadaTimeUnits; //add the Canada time units to the total.
				}
			}
			return total;
		}
		#endregion

		#region Delete
		public static void Delete(CovCat covCat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			return GetTableFromCache(doRefreshCache:true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_CovCatsache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_CovCatsache.FillCacheFromTable(table);
				return table;
			}
			return _CovCatsache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_CovCatsache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(CovCat covcat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),covcat);
				return;
			}
			Crud.CovCatCrud.Update(covcat);
		}

		///<summary></summary>
		public static long Insert(CovCat covcat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				covcat.CovCatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),covcat);
				return covcat.CovCatNum;
			}
			return Crud.CovCatCrud.Insert(covcat);
		}

		///<summary>Does not update the cache.  The cache must be manually refreshed after using this method beccause it only updates the database.</summary>
		public static void MoveUp(CovCat covcat) {
			Meth.NoCheckMiddleTierRole();
			List<CovCat> listCovCats=CovCats.GetDeepCopy();
			int oldOrder=listCovCats.FindIndex(x => x.CovCatNum==covcat.CovCatNum);
			if(oldOrder==0 || oldOrder==-1) {
				return;
			}
			SetOrder(listCovCats[oldOrder],oldOrder-1);
			SetOrder(listCovCats[oldOrder-1],oldOrder);
		}

		///<summary>Does not update the cache.  The cache must be manually refreshed after using this method beccause it only updates the database.</summary>
		public static void MoveDown(CovCat covcat) {
			Meth.NoCheckMiddleTierRole();
			List<CovCat> listCovCats=CovCats.GetDeepCopy();
			int oldOrder=listCovCats.FindIndex(x => x.CovCatNum==covcat.CovCatNum);
			if(oldOrder==listCovCats.Count-1 || oldOrder==-1) {
				return;
			}
			SetOrder(listCovCats[oldOrder],oldOrder+1);
			SetOrder(listCovCats[oldOrder+1],oldOrder);
		}

		///<summary></summary>
		private static void SetOrder(CovCat covcat, int newOrder) {
			Meth.NoCheckMiddleTierRole();
			covcat.CovOrder=newOrder;
			Update(covcat);
		}

		///<summary></summary>
		public static CovCat GetCovCat(long covCatNum) {
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.CovCatNum==covCatNum);
		}
		
		///<summary></summary>
		public static List<CovCat> GetCovCats(List<long> listCovCatNums) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => listCovCatNums.Contains(x.CovCatNum));
		}

		///<summary></summary>
		public static double GetDefaultPercent(long covCatNum) {
			Meth.NoCheckMiddleTierRole();
			CovCat covCat=GetFirstOrDefault(x => x.CovCatNum==covCatNum);
			if(covCat==null){
				return 0;
			}
			return covCat.DefaultPercent;
		}

		///<summary></summary>
		public static string GetDesc(long covCatNum) {
			Meth.NoCheckMiddleTierRole();
			CovCat covCat=GetLastOrDefault(x => x.CovCatNum==covCatNum);
			if(covCat==null){
				return "";
			}
			return covCat.Description;
		}

		///<summary></summary>
		public static long GetCovCatNum(int orderShort) {
			Meth.NoCheckMiddleTierRole();
			CovCat covCat=GetLastOrDefault(x => x.CovOrder==orderShort,isShort:true);
			if(covCat==null){
				return 0;
			}
			return covCat.CovCatNum;
		}

		///<summary>Returns -1 if not in ListShort.</summary>
		public static int GetOrderShort(long covCatNum) {
			Meth.NoCheckMiddleTierRole();
			return GetFindIndex(x => x.CovCatNum==covCatNum,isShort:true);
		}

		///<summary>Returns -1 if not in the provided list.</summary>
		public static int GetOrderShort(long covCatNum,List<CovCat> listCovCats) {
			Meth.NoCheckMiddleTierRole();
			int retVal=-1;
			for(int i=0;i<listCovCats.Count;i++) {
				if(covCatNum==listCovCats[i].CovCatNum) {
					retVal=i;
				}
			}
			return retVal;
		}

		///<summary>Gets a matching benefit category from the short list.  Returns null if not found, which should be tested for.</summary>
		public static CovCat GetForEbenCat(EbenefitCategory ebenefitCategory){
			Meth.NoCheckMiddleTierRole();
			return CovCats.GetFirstOrDefault(x => x.EbenefitCat==ebenefitCategory,isShort:true);
		}

		public static CovCat GetForDesc(string description) {
			Meth.NoCheckMiddleTierRole();
			return CovCats.GetFirstOrDefault(x => x.Description==description,isShort:true);
		}

		///<summary>If none assigned, it will return None.</summary>
		public static EbenefitCategory GetEbenCat(long covCatNum) {
			Meth.NoCheckMiddleTierRole();
			CovCat covCat=CovCats.GetFirstOrDefault(x => x.CovCatNum==covCatNum,isShort:true);
			if(covCat==null){
				return EbenefitCategory.None;
			}
			return covCat.EbenefitCat;
		}

		public static int CountForEbenCat(EbenefitCategory ebenefitCategory) {
			Meth.NoCheckMiddleTierRole();
			return CovCats.GetWhere(x => x.EbenefitCat==ebenefitCategory,isShort:true).Count;
		}

		public static void SetOrdersToDefault() {
			//This can only be run if the validation checks have been run first.
			Meth.NoCheckMiddleTierRole();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				SetOrder(GetForEbenCat(EbenefitCategory.General),0);
				SetOrder(GetForEbenCat(EbenefitCategory.Diagnostic),1);
				SetOrder(GetForEbenCat(EbenefitCategory.DiagnosticXRay),2);
				SetOrder(GetForEbenCat(EbenefitCategory.RoutinePreventive),3);
				SetOrder(GetForEbenCat(EbenefitCategory.Restorative),4);
				SetOrder(GetForEbenCat(EbenefitCategory.Crowns),5);
				SetOrder(GetForEbenCat(EbenefitCategory.Endodontics),6);
				SetOrder(GetForEbenCat(EbenefitCategory.Periodontics),7);
				SetOrder(GetForEbenCat(EbenefitCategory.Prosthodontics),8);
				SetOrder(GetForEbenCat(EbenefitCategory.MaxillofacialProsth),9);
				SetOrder(GetForEbenCat(EbenefitCategory.OralSurgery),10);
				SetOrder(GetForEbenCat(EbenefitCategory.Orthodontics),11);
				SetOrder(GetForEbenCat(EbenefitCategory.Adjunctive),12);
				SetOrder(GetForDesc("Implants"),13);
				SetOrder(GetForEbenCat(EbenefitCategory.Accident),14);
				SetOrder(GetForDesc("SC/RP"),15);
			}
			else {
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
			}
			//now set the remaining categories to come after the ebens.
			int idx=14;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				idx=16;
			}
			List<CovCat> listCovCatsShort=CovCats.GetWhere(x => x.EbenefitCat==EbenefitCategory.None,isShort:true);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				listCovCatsShort.RemoveAll(x => x.Description=="Implants" || x.Description=="SC/RP");
			}
			for(int i=0;i<listCovCatsShort.Count;i++) {
				SetOrder(listCovCatsShort[i],idx);
				idx++;
			}
			//finally, the hidden categories
			List<CovCat> listCovCats=CovCats.GetWhere(x => x.EbenefitCat==EbenefitCategory.None && x.IsHidden);
			for(int i=0;i<listCovCats.Count;i++) {
				SetOrder(listCovCats[i],idx);
				idx++;
			}
		}

		public static void SetSpansToDefault() {
			Meth.NoCheckMiddleTierRole();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				SetSpansToDefaultCanada();
			}
			else {
				SetSpansToDefaultUsa();
			}
		}

		public static void SetSpansToDefaultUsa() {
			//This can only be run if the validation checks have been run first.
			Meth.NoCheckMiddleTierRole();
			long covCatNum;
			CovSpan covSpan;
			covCatNum=GetForEbenCat(EbenefitCategory.General).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D0000";
			covSpan.ToCode="D7999";
			CovSpans.Insert(covSpan);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D9000";
			covSpan.ToCode="D9999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D0000";
			covSpan.ToCode="D0999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D0200";
			covSpan.ToCode="D0399";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D1000";
			covSpan.ToCode="D1999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D2000";
			covSpan.ToCode="D2699";
			CovSpans.Insert(covSpan);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D2800";
			covSpan.ToCode="D2999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D3000";
			covSpan.ToCode="D3999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D4000";
			covSpan.ToCode="D4999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D7000";
			covSpan.ToCode="D7999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D2700";
			covSpan.ToCode="D2799";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D5000";
			covSpan.ToCode="D5899";
			CovSpans.Insert(covSpan);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D6200";
			covSpan.ToCode="D6899";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D5900";
			covSpan.ToCode="D5999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Accident).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covCatNum=GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D8000";
			covSpan.ToCode="D8999";
			CovSpans.Insert(covSpan);
			covCatNum=GetForEbenCat(EbenefitCategory.Adjunctive).CovCatNum;
			CovSpans.DeleteForCat(covCatNum);
			covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode="D9000";
			covSpan.ToCode="D9999";
			CovSpans.Insert(covSpan);
		}

		public static void SetSpansToDefaultCanada() {
			//This can only be run if the validation checks have been run first.
			Meth.NoCheckMiddleTierRole();
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
			RecreateSpansForCategory(EbenefitCategory.Orthodontics,"01901-01901","80000-89999","93330-93349");
			RecreateSpansForCategory(EbenefitCategory.Adjunctive,"90000-93329","93350-99999");
			RecreateSpansForCategoryCanada("Implants","79900-79999");
			RecreateSpansForCategory(EbenefitCategory.Accident);
			RecreateSpansForCategoryCanada("SC/RP","11111-11119","43421-43429");
		}

		private static void SetSpansForCovCatNum(long covCatNum,params string[] stringArrayCodeRanges) {
			Meth.NoCheckMiddleTierRole();
			CovSpans.DeleteForCat(covCatNum);
			for(int i=0;i<stringArrayCodeRanges.Length;i++) {
				string codeRange=stringArrayCodeRanges[i];
				CovSpan covSpan=new CovSpan();
				covSpan.CovCatNum=covCatNum;
				if(codeRange.Contains("-")) {//Code range
					covSpan.FromCode=codeRange.Substring(0,codeRange.IndexOf("-"));
					covSpan.ToCode=codeRange.Substring(covSpan.FromCode.Length+1);
				}
				else {//Single code
					covSpan.FromCode=codeRange;
					covSpan.ToCode=codeRange;
				}
				CovSpans.Insert(covSpan);
			}
		}

		///<summary>Deletes the current CovSpans for the given eBenefitCategory, then creates new code ranges from the ranges specified in arrayCodeRanges.  The values in arrayCodeRanges can be a single code such as "D0120" or a code range such as "D9000-D9999".</summary>
		private static void RecreateSpansForCategory(EbenefitCategory eBenefitCategory,params string[] stringArrayCodeRanges) {
			Meth.NoCheckMiddleTierRole();
			SetSpansForCovCatNum(GetForEbenCat(eBenefitCategory).CovCatNum,stringArrayCodeRanges);
		}

		private static void RecreateSpansForCategoryCanada(string categoryName,params string[] stringArrayCodeRanges) {
			Meth.NoCheckMiddleTierRole();
			CovCat covCat=GetForDesc(categoryName);
			if(covCat==null) {
				covCat=new CovCat();
				covCat.Description=categoryName;
				covCat.EbenefitCat=EbenefitCategory.None;
				covCat.DefaultPercent=-1;
				covCat.CovOrder=CovCats.GetDeepCopy().Count;
				covCat.IsHidden=false;
				CovCats.Insert(covCat);
				CovCats.RefreshCache();
			}
			SetSpansForCovCatNum(covCat.CovCatNum,stringArrayCodeRanges);
		}
	}

	



}









