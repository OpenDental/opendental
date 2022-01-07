using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class DiseaseDefs {
		#region CachePattern

		private class DiseaseDefCache : CacheListAbs<DiseaseDef> {
			protected override List<DiseaseDef> GetCacheFromDb() {
				string command="SELECT * FROM diseasedef ORDER BY ItemOrder";
				return Crud.DiseaseDefCrud.SelectMany(command);
			}
			protected override List<DiseaseDef> TableToList(DataTable table) {
				return Crud.DiseaseDefCrud.TableToList(table);
			}
			protected override DiseaseDef Copy(DiseaseDef diseaseDef) {
				return diseaseDef.Copy();
			}
			protected override DataTable ListToTable(List<DiseaseDef> listDiseaseDefs) {
				return Crud.DiseaseDefCrud.ListToTable(listDiseaseDefs,"DiseaseDef");
			}
			protected override void FillCacheIfNeeded() {
				DiseaseDefs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(DiseaseDef diseaseDef) {
				return !diseaseDef.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DiseaseDefCache _diseaseDefCache=new DiseaseDefCache();

		public static int GetCount(bool isShort=false) {
			return _diseaseDefCache.GetCount(isShort);
		}

		public static List<DiseaseDef> GetDeepCopy(bool isShort=false) {
			return _diseaseDefCache.GetDeepCopy(isShort);
		}

		public static List<DiseaseDef> GetWhere(Predicate<DiseaseDef> match,bool isShort=false) {
			return _diseaseDefCache.GetWhere(match,isShort);
		}

		public static DiseaseDef GetFirstOrDefault(Func<DiseaseDef,bool> match,bool isShort=false) {
			return _diseaseDefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_diseaseDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_diseaseDefCache.FillCacheFromTable(table);
				return table;
			}
			return _diseaseDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Fixes item orders in DB if needed. Returns true if changes were made.</summary>
		public static bool FixItemOrders() {
			bool retVal=false;
			List<DiseaseDef> listDD=GetDeepCopy();
			listDD.Sort(DiseaseDefs.SortItemOrder);
			for(int i=0;i<listDD.Count;i++) {
				if(listDD[i].ItemOrder==i) {
					continue;
				}
				listDD[i].ItemOrder=i;
				DiseaseDefs.Update(listDD[i]);
				retVal=true;
			}
			if(retVal) {
				DiseaseDefs.RefreshCache();
			}
			return retVal;
		}

		///<summary></summary>
		public static void Update(DiseaseDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.DiseaseDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(DiseaseDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				def.DiseaseDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.DiseaseDefNum;
			}
			long retVal=Crud.DiseaseDefCrud.Insert(def);
			return retVal;
		}

		///<summary>Returns a list of valid diseasedefnums to delete from the passed in list.</summary>
		public static List<long> ValidateDeleteList(List<long> listDiseaseDefNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listDiseaseDefNums);
			}
			List<long> listDiseaseDefNumsNotDeletable=new List<long>();
			if(listDiseaseDefNums==null || listDiseaseDefNums.Count < 1) {
				return listDiseaseDefNumsNotDeletable;
			}
			//In use by preference
			if(listDiseaseDefNums.Contains(PrefC.GetLong(PrefName.ProblemsIndicateNone))) {
				listDiseaseDefNumsNotDeletable.Add(PrefC.GetLong(PrefName.ProblemsIndicateNone));
			}
			//Validate patient attached
			string command = "SELECT DISTINCT disease.DiseaseDefNum "
				+"FROM patient,disease "
				+"WHERE patient.PatNum=disease.PatNum "
				+"AND disease.DiseaseDefNum IN ("+String.Join(",",listDiseaseDefNums)+") ";
			try {
				listDiseaseDefNumsNotDeletable.AddRange(Db.GetListLong(command));
			}
			catch {
				//Do Nothing
			}
			//Validate edu resource attached
			command="SELECT DISTINCT eduresource.DiseaseDefNum FROM eduresource WHERE eduresource.DiseaseDefNum IN ("+String.Join(",",listDiseaseDefNums)+") ";
			try {
				listDiseaseDefNumsNotDeletable.AddRange(Db.GetListLong(command));
			}
			catch {
				//Do Nothing
			}
			//Validate family health history attached
			command="SELECT DISTINCT familyhealth.DiseaseDefNum FROM patient,familyhealth "
				+"WHERE patient.PatNum=familyhealth.PatNum "
				+"AND familyhealth.DiseaseDefNum IN ("+String.Join(",",listDiseaseDefNums)+") ";
			try {
				listDiseaseDefNumsNotDeletable.AddRange(Db.GetListLong(command));
			}
			catch {
				//Do Nothing
			}
			return listDiseaseDefNumsNotDeletable;
		}

		///<summary>Returns the name of the disease, whether hidden or not.</summary>
		public static string GetName(long diseaseDefNum) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.DiseaseDefNum==diseaseDefNum);
			return (diseaseDef==null ? "" : diseaseDef.DiseaseName);
		}

		///<summary>Returns the name of the disease based on SNOMEDCode, then if no match tries ICD9Code, then if no match returns empty string. Used in EHR Patient Lists.</summary>
		public static string GetNameByCode(string SNOMEDorICD9Code) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.SnomedCode==SNOMEDorICD9Code);
			if(diseaseDef!=null) {
				return diseaseDef.DiseaseName;
			}
			diseaseDef=GetFirstOrDefault(x => x.ICD9Code==SNOMEDorICD9Code);
			return (diseaseDef==null ? "" : diseaseDef.DiseaseName);
		}

		///<summary>Returns the DiseaseDefNum based on SNOMEDCode, then if no match tries ICD9Code, then if no match tries ICD10Code, then if no match returns 0. Used in EHR Patient Lists and when automatically inserting pregnancy Dx from FormVitalsignEdit2014.  Will match hidden diseases.</summary>
		public static long GetNumFromCode(string CodeValue) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.SnomedCode==CodeValue);
			if(diseaseDef!=null) {
				return diseaseDef.DiseaseDefNum;
			}
			diseaseDef=GetFirstOrDefault(x => x.ICD9Code==CodeValue);
			if(diseaseDef!=null) {
				return diseaseDef.DiseaseDefNum;
			}
			diseaseDef=GetFirstOrDefault(x => x.Icd10Code==CodeValue);
			return (diseaseDef==null ? 0 : diseaseDef.DiseaseDefNum);
		}

		///<summary>Returns the DiseaseDefNum based on SNOMEDCode.  If no match or if SnomedCode is an empty string returns 0.  Only matches SNOMEDCode, not ICD9 or ICD10.</summary>
		public static long GetNumFromSnomed(string SnomedCode) {
			//No need to check RemotingRole; no call to db.
			if(SnomedCode=="") {
				return 0;
			}
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.SnomedCode==SnomedCode);
			return (diseaseDef==null ? 0 : diseaseDef.DiseaseDefNum);
		}

		///<summary>Returns the diseaseDef with the specified num.  Will match hidden diseasedefs.</summary>
		public static DiseaseDef GetItem(long diseaseDefNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.DiseaseDefNum==diseaseDefNum);
		}

		///<summary>Returns the diseaseDefNum that exactly matches the specified string.  Used in import functions when you only have the name to work with.  Can return 0 if no match.  Does not match hidden diseases.</summary>
		public static long GetNumFromName(string diseaseName){
			//No need to check RemotingRole; no call to db.
			return GetNumFromName(diseaseName,false);
		}

		///<summary>Returns the diseaseDefNum that exactly matches the specified string.  Will return 0 if no match.
		///Set matchHidden to true to match hidden diseasedefs as well.</summary>
		public static long GetNumFromName(string diseaseName,bool matchHidden) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=_diseaseDefCache.GetFirstOrDefault(x => x.DiseaseName==diseaseName,!matchHidden);
			return (diseaseDef==null) ? 0 : diseaseDef.DiseaseDefNum;
		}

		///<summary>Returns the diseasedef that has a name exactly matching the specified string. Returns null if no match.  Does not match hidden diseases.</summary>
		public static DiseaseDef GetFromName(string diseaseName) {
			//No need to check RemotingRole; no call to db.
			return _diseaseDefCache.GetFirstOrDefault(x => x.DiseaseName==diseaseName,true);
		}

		public static List<long> GetChangedSinceDiseaseDefNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT DiseaseDefNum FROM diseasedef WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> diseaseDefNums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				diseaseDefNums.Add(PIn.Long(dt.Rows[i]["DiseaseDefNum"].ToString()));
			}
			return diseaseDefNums;
		}

		///<summary>Used along with GetChangedSinceDiseaseDefNums</summary>
		public static List<DiseaseDef> GetMultDiseaseDefs(List<long> diseaseDefNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DiseaseDef>>(MethodBase.GetCurrentMethod(),diseaseDefNums);
			}
			string strDiseaseDefNums="";
			DataTable table;
			if(diseaseDefNums.Count>0) {
				for(int i=0;i<diseaseDefNums.Count;i++) {
					if(i>0) {
						strDiseaseDefNums+="OR ";
					}
					strDiseaseDefNums+="DiseaseDefNum='"+diseaseDefNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM diseasedef WHERE "+strDiseaseDefNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			DiseaseDef[] multDiseaseDefs=Crud.DiseaseDefCrud.TableToList(table).ToArray();
			List<DiseaseDef> diseaseDefList=new List<DiseaseDef>(multDiseaseDefs);
			return diseaseDefList;
		}

		public static bool ContainsSnomed(string snomedCode, long excludeDefNum) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.SnomedCode==snomedCode && x.DiseaseDefNum!=excludeDefNum);
			return (diseaseDef!=null);
		}

		public static bool ContainsICD9(string icd9Code,long excludeDefNum) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.ICD9Code==icd9Code && x.DiseaseDefNum!=excludeDefNum);
			return (diseaseDef!=null);
		}

		public static bool ContainsIcd10(string icd10Code,long excludeDefNum) {
			//No need to check RemotingRole; no call to db.
			DiseaseDef diseaseDef=GetFirstOrDefault(x => x.Icd10Code==icd10Code && x.DiseaseDefNum!=excludeDefNum);
			return (diseaseDef!=null);
		}

		///<summary>Sync pattern, must sync entire table. Probably only to be used in the master problem list window.</summary>
		public static void Sync(List<DiseaseDef> listDefs,List<DiseaseDef> listDefsOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDefs,listDefsOld);
				return;
			}
			Crud.DiseaseDefCrud.Sync(listDefs,listDefsOld);
		}

		///<summary>Get all diseasedefs that have a pregnancy code that applies to the three CQM measures with pregnancy as an exclusion condition.</summary>
		public static List<DiseaseDef> GetAllPregDiseaseDefs() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DiseaseDef>>(MethodBase.GetCurrentMethod());
			}
			Dictionary<string,string> listAllPregCodesForCQMs=EhrCodes.GetCodesExistingInAllSets(new List<string> { "2.16.840.1.113883.3.600.1.1623","2.16.840.1.113883.3.526.3.378" });
			List<DiseaseDef> retval=new List<DiseaseDef>();
			List<DiseaseDef> listDiseaseDefs=GetDeepCopy();
			for(int i=0;i<listDiseaseDefs.Count;i++) {
				if(listAllPregCodesForCQMs.ContainsKey(listDiseaseDefs[i].ICD9Code) && listAllPregCodesForCQMs[listDiseaseDefs[i].ICD9Code]=="ICD9CM") {
					retval.Add(listDiseaseDefs[i]);
					continue;
				}
				if(listAllPregCodesForCQMs.ContainsKey(listDiseaseDefs[i].Icd10Code) && listAllPregCodesForCQMs[listDiseaseDefs[i].Icd10Code]=="ICD10CM") {
					retval.Add(listDiseaseDefs[i]);
					continue;
				}
				if(listAllPregCodesForCQMs.ContainsKey(listDiseaseDefs[i].SnomedCode) && listAllPregCodesForCQMs[listDiseaseDefs[i].SnomedCode]=="SNOMEDCT") {
					retval.Add(listDiseaseDefs[i]);
					continue;
				}
			}
			return retval;
		}

		///<summary>Sorts alphabetically by DiseaseName, then by PK.</summary>
		public static int SortAlphabetically(DiseaseDef x,DiseaseDef y) {
			if(x.DiseaseName!=y.DiseaseName) {
				return x.DiseaseName.CompareTo(y.DiseaseName);
			}
			return x.DiseaseDefNum.CompareTo(y.DiseaseDefNum);
		}

		public static int SortItemOrder(DiseaseDef x,DiseaseDef y) {
			if(x.ItemOrder!=y.ItemOrder) {
				return x.ItemOrder.CompareTo(y.ItemOrder);
			}
			return x.DiseaseDefNum.CompareTo(y.DiseaseDefNum);
		}

	/*public static DiseaseDef GetByICD9Code(string ICD9Code) {///<summary>Returns the diseasedef that has a name exactly matching the specified string. Returns null if no match.  Does not match hidden diseases.</summary>
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<List.Length;i++) {
				if(ICD9Code==List[i].ICD9Code) {
					return List[i];
				}
			}
			return null;
		}*/
	}

		



		
	

	

	


}










