using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Word;
using System.Globalization;
using OpenDentBusiness.Eclaims;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcedureCodes {
		#region Get Methods

		public static List<ProcedureCode> GetProcCodeStartsWith(string codeStart) {
			//No need to check RemotingRole; no call to db.
			return _procedureCodeCache.GetWhereForKey(x => x.StartsWith(codeStart));
		}

		#endregion

		#region Cache Pattern

		///<summary>Utilizes the NonPkAbs version of CacheDict because it uses ProcCode as the Key instead of the PK CodeNum.</summary>
		private class ProcedureCodeCache : CacheDictNonPkAbs<ProcedureCode,string,ProcedureCode> {
			protected override List<ProcedureCode> GetCacheFromDb() {
				string command="SELECT * FROM procedurecode ORDER BY ProcCat,ProcCode";
				return Crud.ProcedureCodeCrud.SelectMany(command);
			}
			protected override List<ProcedureCode> TableToList(DataTable table) {
				return Crud.ProcedureCodeCrud.TableToList(table);
			}
			protected override ProcedureCode Copy(ProcedureCode procedureCode) {
				return procedureCode.Copy();
			}
			protected override DataTable DictToTable(Dictionary<string,ProcedureCode> dictProcedureCodes) {
				return Crud.ProcedureCodeCrud.ListToTable(dictProcedureCodes.Values.Cast<ProcedureCode>().ToList(),"ProcedureCode");
			}
			protected override void FillCacheIfNeeded() {
				ProcedureCodes.GetTableFromCache(false);
			}			
			protected override string GetDictKey(ProcedureCode procedureCode) {
				return procedureCode.ProcCode;
			}
			protected override ProcedureCode GetDictValue(ProcedureCode procedureCode) {
				return procedureCode;
			}
			protected override ProcedureCode CopyDictValue(ProcedureCode procedureCode) {
				return procedureCode.Copy();
			}

			protected override DataTable ListToTable(List<ProcedureCode> listAllItems) {
				return Crud.ProcedureCodeCrud.ListToTable(listAllItems);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcedureCodeCache _procedureCodeCache=new ProcedureCodeCache();

		public static List<ProcedureCode> GetListDeep() {
			return _procedureCodeCache.GetDeepCopyList();
		}

		public static int GetCount(bool isShort=false) {
			return _procedureCodeCache.GetCount(isShort);
		}

		public static ProcedureCode GetOne(string procCode) {
			return _procedureCodeCache.GetOne(procCode);
		}

		public static ProcedureCode GetFirstOrDefault(Func<ProcedureCode,bool> match,bool isShort=false) {
			return _procedureCodeCache.GetFirstOrDefault(match,isShort);
		}

		public static ProcedureCode GetFirstOrDefaultFromList(Func<ProcedureCode,bool> match,bool isShort=false) {
			return _procedureCodeCache.GetFirstOrDefaultFromList(match,isShort);
		}

		public static List<ProcedureCode> GetWhere(Func<ProcedureCode,bool> match,bool isShort=false) {
			return _procedureCodeCache.GetWhere(match,isShort);
		}

		public static List<ProcedureCode> GetWhereFromList(Predicate<ProcedureCode> match,bool isShort=false) {
			return _procedureCodeCache.GetWhereFromList(match,isShort);
		}

		public static bool GetContainsKey(string procCode) {
			return _procedureCodeCache.GetContainsKey(procCode);
		}

		#region Additional Lists

		///<summary>Returns a list of CodeNums for specific BW procedure codes.
		///There are several places in the program that need a BW group.  E.g. when computing limitations.</summary>
		public static List<long> ListBWCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenBWCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific Exam procedure codes.
		///There are several places in the program that need a Exam group.  E.g. when computing limitations.</summary>
		public static List<long> ListExamCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenExamCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific PanoFMX procedure codes.
		///There are several places in the program that need a PanoFMX group.  E.g. when computing limitations.</summary>
		public static List<long> ListPanoFMXCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenPanoCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific BW procedure codes.
		///There are several places in the program that need a BW group.  E.g. when computing limitations.</summary>
		public static List<long> ListCancerScreeningCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenCancerScreeningCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific Exam procedure codes.
		///There are several places in the program that need a Exam group.  E.g. when computing limitations.</summary>
		public static List<long> ListProphyCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenProphyCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific PanoFMX procedure codes.
		///There are several places in the program that need a PanoFMX group.  E.g. when computing limitations.</summary>
		public static List<long> ListFlourideCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenFlourideCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific BW procedure codes.
		///There are several places in the program that need a BW group.  E.g. when computing limitations.</summary>
		public static List<long> ListSealantCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenSealantCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific Exam procedure codes.
		///There are several places in the program that need a Exam group.  E.g. when computing limitations.</summary>
		public static List<long> ListCrownCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenCrownCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific PanoFMX procedure codes.
		///There are several places in the program that need a PanoFMX group.  E.g. when computing limitations.</summary>
		public static List<long> ListSRPCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenSRPCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific BW procedure codes.
		///There are several places in the program that need a BW group.  E.g. when computing limitations.</summary>
		public static List<long> ListFullDebridementCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenFullDebridementCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific Exam procedure codes.
		///There are several places in the program that need a Exam group.  E.g. when computing limitations.</summary>
		public static List<long> ListPerioMaintCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenPerioMaintCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific PanoFMX procedure codes.
		///There are several places in the program that need a PanoFMX group.  E.g. when computing limitations.</summary>
		public static List<long> ListDenturesCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenDenturesCodes);
			}
		}

		///<summary>Returns a list of CodeNums for specific BW procedure codes.
		///There are several places in the program that need a BW group.  E.g. when computing limitations.</summary>
		public static List<long> ListImplantCodeNums {
			get {
				return GetCodeNumsForPref(PrefName.InsBenImplantCodes);
			}
		}

		#region InsHist Preference Procedures

		///<summary>Returns the first procedure code for the InsHist preference passed in.</summary>
		public static ProcedureCode GetByInsHistPref(PrefName prefName) {
			return GetProcCode(GetCodeNumsForPref(prefName).FirstOrDefault());
		}

		#endregion InsHist Preference Procedures

		#endregion Additional Lists

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procedureCodeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procedureCodeCache.FillCacheFromTable(table);
				return table;
			}
			return _procedureCodeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		public const string GroupProcCode="~GRP~";
		///<summary>222 is the FeeSchedNum for the 'DO NOT EDIT' fee schedule in the customers database.</summary>
		public const long DoNotEditFeeSchedNum=222;

		public static string BitewingCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					return "02144";//4BW The same code for Quebec as well as the rest of Canada.
				}
				return "D0274";
			}
		}

		public static string PanoCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					if(Canadian.IsQuebec()) {//Quebec
						return "02600";
					}
					else {//The rest of Canada use the same procedure codes.
						return "02601";
					}
				}
				return "D0330";
			}
		}

		public static string CancerScreeningCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "01401";//Just a diagnostic code that doesn't look like it's very common.
				}
				return "D0431";
			}
		}

		public static string ProphyCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "11101";
				}
				return "D1110";
			}
		}

		public static string FlourideCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "12101";
				}
				return "D1206";
			}
		}

		public static string SealantCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "13401";
				}
				return "D1351";
			}
		}

		public static string CrownCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "27201";
				}
				return "D2740";
			}
		}

		public static string SRPCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "43421";
				}
				return "D4341";
			}
		}

		public static string FullDebridementCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "42121";
				}
				return "D4355";
			}
		}

		public static string PerioMaintCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "49101";
				}
				return "D4910";
			}
		}

		public static string DenturesCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "51711";
				}
				return "D5110";
			}
		}

		public static string ImplantCode {
			get {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					return "79921";
				}
				return "D6010";
			}
		}

		///<summary>Hard-coded dictionary of eService codes and their corresponding ProcCode within the database at HQ.</summary>
		private static readonly Dictionary<eServiceCode,string> _dictEServiceProcCodes=new Dictionary<eServiceCode,string>() {
			{ eServiceCode.Bundle,"042" },
			{ eServiceCode.ConfirmationOwn,"045" },
			{ eServiceCode.ConfirmationRequest,"040" },
			{ eServiceCode.EClipboard,"047" },
			{ eServiceCode.MobileWeb,"027" },
			{ eServiceCode.PatientPortal,"033" },
			{ eServiceCode.ResellerSoftwareOnly,"043" },
			{ eServiceCode.SoftwareOnly,"030" },
			{ eServiceCode.IntegratedTexting,"038" },
			{ eServiceCode.IntegratedTextingOwn,"046" },
			{ eServiceCode.IntegratedTextingUsage,"039" },
			{ eServiceCode.WebForms,"036" },
			{ eServiceCode.WebSched,"037" },
			{ eServiceCode.WebSchedNewPatAppt,"041" },
			{ eServiceCode.WebSchedASAP,"044" },
			{ eServiceCode.EmailMassUsage,"050" },
			{ eServiceCode.EmailSecureUsage,"051" },
			{ eServiceCode.EmailSecureAccess,"052" },
		};

		public static string GetProcCodeForEService(eServiceCode eService) {
			//No need to check RemotingRole; no call to db.
			return _dictEServiceProcCodes[eService];
		}

		public static eServiceCode GetEServiceForProcCode(string procCode) {
			//No need to check RemotingRole; no call to db.
			foreach(eServiceCode eService in _dictEServiceProcCodes.Keys) {
				if(_dictEServiceProcCodes[eService]==procCode) {
					return eService;
				}
			}
			throw new ODException("No corresponding eService found for the ProcCode provided.");
		}

		///<summary>Combine ProcedureCode and Fee into single class using HQ's DoNotEditFeeSchedNum. Used by signup portal.</summary>
		public static List<EServiceFee> GetAllEServiceFees() {
			List<string> listESerivceProcCodes=GetAllEServiceProcCodes();
			List<ProcedureCode> listProcCodes=GetWhere(x => listESerivceProcCodes.Contains(x.ProcCode));
			List<Fee> listFees=Fees.GetListForScheds(DoNotEditFeeSchedNum);
			return listProcCodes.Select(procCode => {
				Fee procFeeRet=listFees.FirstOrDefault(fee => fee.CodeNum==procCode.CodeNum);
				if(procFeeRet==null) {
					throw new Exception("OD customers DO NOT EDIT FeeSchedule does not include a single valid Fee for ProcCode: "+procCode.ProcCode);
				}
				return new EServiceFee() { ESvcProcCode=procCode,ESvcFee=procFeeRet };
			}).ToList();
		}

		public static List<string> GetAllEServiceProcCodes() {
			//No need to check RemotingRole; no call to db.
			return _dictEServiceProcCodes.Values.ToList();
		}

		public static List<ProcedureCode> GetChangedSince(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcedureCode>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT * FROM procedurecode WHERE DateTStamp > "+POut.DateT(changedSince);
			return Crud.ProcedureCodeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(ProcedureCode code) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				code.CodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),code);
				return code.CodeNum;
			}
			//must have already checked procCode for nonduplicate.
			return Crud.ProcedureCodeCrud.Insert(code);
		}

		///<summary></summary>
		public static void InsertMany(List<ProcedureCode> listCodes) {
			if(listCodes.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCodes);
				return;
			}
			//must have already checked procCode for nonduplicate.
			Crud.ProcedureCodeCrud.InsertMany(listCodes);
		}

		///<summary></summary>
		public static void Update(ProcedureCode code){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),code);
				return;
			}
			Crud.ProcedureCodeCrud.Update(code);
		}

		///<summary></summary>
		public static bool Update(ProcedureCode procCode,ProcedureCode procCodeOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),procCode,procCodeOld);
			}
			return Crud.ProcedureCodeCrud.Update(procCode,procCodeOld);
		}

		///<summary>Counts all procedure codes, including hidden codes.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM procedurecode";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Returns the ProcedureCode for the supplied procCode such as such as D####.
		///If no ProcedureCode is found, returns a new ProcedureCode.</summary>
		public static ProcedureCode GetProcCode(string myCode) {
			//No need to check RemotingRole; no call to db.
			ProcedureCode procedureCode=new ProcedureCode();
			if(IsValidCode(myCode)) {
				procedureCode=_procedureCodeCache.GetOne(myCode);
			}
			return procedureCode;
		}

		///<summary>Returns a list of ProcedureCodes for the supplied procCodes such as such as D####.  Returns an empty list if no matches.</summary>
		public static List<ProcedureCode> GetProcCodes(List<string> listCodes) {
			//No need to check RemotingRole; no call to db.
			if(listCodes==null || listCodes.Count < 1) {
				return new List<ProcedureCode>();
			}
			return _procedureCodeCache.GetWhereForKey(x => ListTools.In(x,listCodes));
		}

		///<summary>The new way of getting a procCode. Uses the primary key instead of string code. Returns new instance if not found.
		///Pass in a list of all codes to save from locking the cache if you are going to call this method repeatedly.</summary>
		public static ProcedureCode GetProcCode(long codeNum,List<ProcedureCode> listProcedureCodes=null) {
			//No need to check RemotingRole; no call to db.
			if(codeNum==0) {
				return new ProcedureCode();
			}
			if(listProcedureCodes==null) {
				return _procedureCodeCache.GetFirstOrDefaultFromList(x => x.CodeNum==codeNum)??new ProcedureCode();
			}
			for(int i=0;i<listProcedureCodes.Count;i++) {
				if(listProcedureCodes[i].CodeNum==codeNum) {
					return listProcedureCodes[i];
				}
			}
			return new ProcedureCode();
		}

		///<summary>Gets code from db to avoid having to constantly refresh in FormProcCodes</summary>
		public static ProcedureCode GetProcCodeFromDb(long codeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ProcedureCode>(MethodBase.GetCurrentMethod(),codeNum);
			}
			ProcedureCode retval=Crud.ProcedureCodeCrud.SelectOne(codeNum);
			if(retval==null) {
				//We clasically return an empty procedurecode object here instead of null.
				return new ProcedureCode();
			}
			return retval;
		}

		///<summary>Supply the human readable proc code such as D####. If not found, returns 0.</summary>
		public static long GetCodeNum(string myCode) {
			//No need to check RemotingRole; no call to db.
			if(IsValidCode(myCode)) {
				return _procedureCodeCache.GetOne(myCode).CodeNum;
			}
			return 0;
		}

		///<summary>Gets a list of ProcedureCode for a given treatment area,including codes in hidden categories if isHiddenIncluded=true.</summary>
		public static List<ProcedureCode> GetProcCodesByTreatmentArea(bool isHiddenIncluded,params TreatmentArea[] arrayTreatmentAreas) {
			return _procedureCodeCache.GetWhere(x => ListTools.In(x.TreatArea,arrayTreatmentAreas) 
				&& (isHiddenIncluded || !Defs.GetHidden(DefCat.ProcCodeCats,x.ProcCat)));
		}

		///<summary>If a substitute exists for the given proc code, then it will give the CodeNum of that code.
		///Otherwise, it will return the codeNum for the given procCode.</summary>
		public static long GetSubstituteCodeNum(string procCode,string toothNum,long planNum,List<SubstitutionLink> listSubLinks=null) {
			//No need to check RemotingRole; no call to db.
			long codeNum=0;
			if(string.IsNullOrEmpty(procCode)) {
				return codeNum;
			}
			ODException.SwallowAnyException(() => {
				ProcedureCode procedureCode=_procedureCodeCache.GetOne(procCode);
				codeNum=procedureCode.CodeNum;
				listSubLinks=listSubLinks??SubstitutionLinks.GetAllForPlans(new long[] { planNum });
				listSubLinks=listSubLinks.Where(x=>x.PlanNum==planNum).ToList();
				//We allow multiple substitution codes for procedures now so we have to check to implement any new hierarchy
				SubstitutionLink subLink=SubstitutionLinks.GetSubLinkByHierarchy(procedureCode,toothNum,listSubLinks);
				//Check procedure code level substitution if no insurance substitution override.
				if((subLink==null || string.IsNullOrEmpty(subLink.SubstitutionCode) || !ProcedureCodes.IsValidCode(subLink.SubstitutionCode))
					&& !string.IsNullOrEmpty(procedureCode.SubstitutionCode) && ProcedureCodes.IsValidCode(procedureCode.SubstitutionCode) && listSubLinks.All(x=>x.CodeNum!=codeNum))
				{
					//Swallow any following exceptions because the old code would first check and make sure the key was in the dictionary.
					ODException.SwallowAnyException(() => {
						codeNum=GetSubstitutionCodeNumHelper(codeNum,procedureCode.SubstOnlyIf,procedureCode.SubstitutionCode,toothNum);
					});
				}
				//Check insplan substituationlink(override) for the procedure.
				else if(subLink!=null && !string.IsNullOrEmpty(subLink.SubstitutionCode) && ProcedureCodes.IsValidCode(subLink.SubstitutionCode)) {
					ODException.SwallowAnyException(() => {
						codeNum=GetSubstitutionCodeNumHelper(codeNum,subLink.SubstOnlyIf,subLink.SubstitutionCode,toothNum);
					});
				}
				if(codeNum==0) {//If somehow we got a bad value up above we'll revert back to the procedure code
					codeNum=procedureCode.CodeNum;
				}
			});
			return codeNum;
		}

		///<summary></summary>
		private static long GetSubstitutionCodeNumHelper(long defaultCodeNum,SubstitutionCondition condition,string substitutionCode,string toothNum) {
			long codeNum=defaultCodeNum;
			if(condition==SubstitutionCondition.Never) {
				return codeNum;
			}
			else if(condition==SubstitutionCondition.Always) {
				codeNum=_procedureCodeCache.GetOne(substitutionCode).CodeNum;
			}
			else if(condition==SubstitutionCondition.Molar && Tooth.IsMolar(toothNum)) {
				codeNum=_procedureCodeCache.GetOne(substitutionCode).CodeNum;
			}
			else if(condition==SubstitutionCondition.SecondMolar && Tooth.IsSecondMolar(toothNum)) {
				codeNum=_procedureCodeCache.GetOne(substitutionCode).CodeNum;
			}
			else if(condition==SubstitutionCondition.Posterior && Tooth.IsPosterior(toothNum)) {
				codeNum=_procedureCodeCache.GetOne(substitutionCode).CodeNum;
			}
			return codeNum;
		}
		
		///<summary>Gets the proc codes as a comma separated list from the preference and finds the corresponding code nums.</summary>
		public static List<long> GetCodeNumsForPref(PrefName pref) {
			List<string> listCodes=PrefC.GetString(pref).Split(',').Select(x => x.Trim()).ToList();
			return GetWhereFromList(x => ListTools.In(x.ProcCode,listCodes)).Select(x => x.CodeNum).ToList();
		}

		///<summary>Gets the CodeNums for the passed in InsHist preference.</summary>
		public static List<long> GetCodeNumsForInsHistPref(PrefName pref) {
			List<long> retVal=GetCodeNumsForPref(pref);
			switch(pref) {
				case PrefName.InsHistBWCodes:
					retVal.AddRange(ProcedureCodes.ListBWCodeNums);
					break;
				case PrefName.InsHistExamCodes:
					retVal.AddRange(ProcedureCodes.ListExamCodeNums);
					break;
				case PrefName.InsHistPanoCodes:
					retVal.AddRange(ProcedureCodes.ListPanoFMXCodeNums);
					break;
				case PrefName.InsHistPerioLLCodes:
				case PrefName.InsHistPerioLRCodes:
				case PrefName.InsHistPerioULCodes:
				case PrefName.InsHistPerioURCodes:
					retVal.AddRange(ProcedureCodes.ListSRPCodeNums);
					break;
				case PrefName.InsHistPerioMaintCodes:
					retVal.AddRange(ProcedureCodes.ListPerioMaintCodeNums);
					break;
				case PrefName.InsHistProphyCodes:
					retVal.AddRange(ProcedureCodes.ListProphyCodeNums);
					break;
				default:
					//InsHistDebridementCodes
					break;
			}
			return retVal;
		}

		///<summary>Returns true if this code is marked as BypassIfZero and if this procFee is zero.</summary>
		public static bool CanBypassLockDate(long codeNum,double procFee) {
			bool isBypassGlobalLock=false;
			ProcedureCode procCode=GetFirstOrDefaultFromList(x => x.CodeNum==codeNum);
			if(procCode!=null && procCode.BypassGlobalLock==BypassLockStatus.BypassIfZero && CompareDouble.IsZero(procFee)) {
				isBypassGlobalLock=true;
			}
			return isBypassGlobalLock;
		}

		///<summary>Returns true if any code is marked as BypassIfZero.</summary>
		public static bool DoAnyBypassLockDate() {
			ProcedureCode procedureCode=GetFirstOrDefaultFromList(x => x.BypassGlobalLock==BypassLockStatus.BypassIfZero);
			return (procedureCode!=null);
		}

		///<summary>Pass in an optional list of procedure codes in order to use it instead of the cache.</summary>
		public static string GetStringProcCode(long codeNum,List<ProcedureCode> listProcedureCodes=null,bool doThrowIfMissing=true) {
			//No need to check RemotingRole; no call to db.
			if(codeNum==0) {
				return "";
				//throw new ApplicationException("CodeNum cannot be zero.");
			}
			ProcedureCode procedureCode;
			if(listProcedureCodes==null) {
				procedureCode=GetFirstOrDefaultFromList(x => x.CodeNum==codeNum);
			}
			else {
				procedureCode=listProcedureCodes.FirstOrDefault(x => x.CodeNum==codeNum);
			}
			if(procedureCode!=null) {
				return procedureCode.ProcCode;
			}
			if(doThrowIfMissing) {
				throw new ApplicationException("Missing codenum");
			}
			return "";
		}

		///<summary></summary>
		public static bool IsValidCode(string myCode){
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(myCode)) {
				return false;
			}
			return _procedureCodeCache.GetContainsKey(myCode);
		}

		///<summary>Grouped by Category.  Used only in FormRpProcCodes.</summary>
		public static ProcedureCode[] GetProcList(Def[][] arrayDefs=null) {
			//No need to check RemotingRole; no call to db.
			List<ProcedureCode> retVal=new List<ProcedureCode>();
			Def[] array=null;
			if(arrayDefs==null) {
				array=Defs.GetDefsForCategory(DefCat.ProcCodeCats,true).ToArray();
			}
			else {
				array=arrayDefs[(int)DefCat.ProcCodeCats];
			}
			List<ProcedureCode> listProcedureCodes=GetListDeep();
			for(int j=0;j<arrayDefs[(int)DefCat.ProcCodeCats].Length;j++) {
				for(int k=0;k<listProcedureCodes.Count;k++) {
					if(arrayDefs[(int)DefCat.ProcCodeCats][j].DefNum==listProcedureCodes[k].ProcCat) {
						retVal.Add(listProcedureCodes[k].Copy());
					}
				}
			}
			return retVal.ToArray();
		}

		///<summary>Gets a list of procedure codes directly from the database.  If categories.length==0, then we will get for all categories.  Categories are defnums.  FeeScheds are, for now, defnums.</summary>
		public static DataTable GetProcTable(string abbr,string desc,string code,List<long> categories,long feeSchedNum,
			long feeSched2Num,long feeSched3Num)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),abbr,desc,code,categories,feeSchedNum,feeSched2Num,feeSched3Num);
			}
			string whereCat;
			if(categories.Count==0){
				whereCat="1";
			}
			else{
				whereCat="(";
				for(int i=0;i<categories.Count;i++){
					if(i>0){
						whereCat+=" OR ";
					}
					whereCat+="ProcCat="+POut.Long(categories[i]);
				}
				whereCat+=")";
			}
			FeeSched feeSched=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			FeeSched feeSched2=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==feeSched2Num);
			FeeSched feeSched3=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==feeSched3Num);
			//Query changed to be compatible with both MySQL and Oracle (not tested).
			string command="SELECT ProcCat,Descript,AbbrDesc,procedurecode.ProcCode,";
			if(feeSched==null) {
				command+="-1 FeeAmt1,";
			}
			else {
				command+="CASE ";
				if(!feeSched.IsGlobal && Clinics.ClinicNum!=0) {//Use local clinic fee if there's one present
					command+="WHEN (feeclinic1.Amount IS NOT NULL) THEN feeclinic1.Amount ";
				}
				command+="WHEN (feehq1.Amount IS NOT NULL) THEN feehq1.Amount ELSE -1 END FeeAmt1,";
			}
			if(feeSched2==null) {
				command+="-1 FeeAmt2,";
			}
			else {
				command+="CASE ";
				if(!feeSched2.IsGlobal && Clinics.ClinicNum!=0) {//Use local clinic fee if there's one present
					command+="WHEN (feeclinic2.Amount IS NOT NULL) THEN feeclinic2.Amount ";
				}
				command+="WHEN (feehq2.Amount IS NOT NULL) THEN feehq2.Amount ELSE -1 END FeeAmt2,";
			}
			if(feeSched3==null) {
				command+="-1 FeeAmt3,";
			}
			else {
				command+="CASE ";
				if(!feeSched3.IsGlobal && Clinics.ClinicNum!=0) {//Use local clinic fee if there's one present
					command+="WHEN (feeclinic3.Amount IS NOT NULL) THEN feeclinic3.Amount ";
				}
				command+="WHEN (feehq3.Amount IS NOT NULL) THEN feehq3.Amount ELSE -1 END FeeAmt3,";				
			}
			command+="procedurecode.CodeNum ";
			if(feeSched!=null && !feeSched.IsGlobal && Clinics.ClinicNum!=0) {
				command+=",CASE WHEN (feeclinic1.Amount IS NOT NULL) THEN 1 ELSE 0 END IsClinic1 ";
			}
			if(feeSched2!=null && !feeSched2.IsGlobal && Clinics.ClinicNum!=0) {
				command+=",CASE WHEN (feeclinic2.Amount IS NOT NULL) THEN 1 ELSE 0 END IsClinic2 ";
			}
			if(feeSched3!=null && !feeSched3.IsGlobal && Clinics.ClinicNum!=0) {
				command+=",CASE WHEN (feeclinic3.Amount IS NOT NULL) THEN 1 ELSE 0 END IsClinic3 ";
			}
			command+="FROM procedurecode ";
			if(feeSched!=null) {
				if(!feeSched.IsGlobal && Clinics.ClinicNum!=0) {//Get local clinic fee if there's one present
					command+="LEFT JOIN fee feeclinic1 ON feeclinic1.CodeNum=procedurecode.CodeNum AND feeclinic1.FeeSched="+POut.Long(feeSched.FeeSchedNum)
						+" AND feeclinic1.ClinicNum="+POut.Long(Clinics.ClinicNum)+" ";
				}
				//Get the hq clinic fee if there's one present
				command+="LEFT JOIN fee feehq1 ON feehq1.CodeNum=procedurecode.CodeNum AND feehq1.FeeSched="+POut.Long(feeSched.FeeSchedNum)
					+" AND feehq1.ClinicNum=0 ";
			}
			if(feeSched2!=null) {
				if(!feeSched2.IsGlobal && Clinics.ClinicNum!=0) {//Get local clinic fee if there's one present
					command+="LEFT JOIN fee feeclinic2 ON feeclinic2.CodeNum=procedurecode.CodeNum AND feeclinic2.FeeSched="+POut.Long(feeSched2.FeeSchedNum)
						+" AND feeclinic2.ClinicNum="+POut.Long(Clinics.ClinicNum)+" ";
				}
				//Get the hq clinic fee if there's one present
				command+="LEFT JOIN fee feehq2 ON feehq2.CodeNum=procedurecode.CodeNum AND feehq2.FeeSched="+POut.Long(feeSched2.FeeSchedNum)
					+" AND feehq2.ClinicNum=0 ";
			}
			if(feeSched3!=null) {
				if(!feeSched3.IsGlobal && Clinics.ClinicNum!=0) {//Get local clinic fee if there's one present
					command+="LEFT JOIN fee feeclinic3 ON feeclinic3.CodeNum=procedurecode.CodeNum AND feeclinic3.FeeSched="+POut.Long(feeSched3.FeeSchedNum)
						+" AND feeClinic3.ClinicNum="+POut.Long(Clinics.ClinicNum)+" ";
				}
				//Get the hq clinic fee if there's one present
				command+="LEFT JOIN fee feehq3 ON feehq3.CodeNum=procedurecode.CodeNum AND feehq3.FeeSched="+POut.Long(feeSched3.FeeSchedNum)
					+" AND feehq3.ClinicNum=0 ";
			}
			command+="LEFT JOIN definition ON definition.DefNum=procedurecode.ProcCat "
			+"WHERE "+whereCat+" "
			+"AND Descript LIKE '%"+POut.String(desc)+"%' "
			+"AND AbbrDesc LIKE '%"+POut.String(abbr)+"%' "
			+"AND procedurecode.ProcCode LIKE '%"+POut.String(code)+"%' "
			+"ORDER BY definition.ItemOrder,procedurecode.ProcCode";
			return Db.GetTable(command);
		}

		///<summary>Returns the LaymanTerm for the supplied codeNum, or the description if none present.</summary>
		public static string GetLaymanTerm(long codeNum) {
			//No need to check RemotingRole; no call to db.
			string laymanTerm="";
			ProcedureCode procedureCode=GetFirstOrDefaultFromList(x => x.CodeNum==codeNum);
			if(procedureCode!=null) {
				laymanTerm=(procedureCode.LaymanTerm!="" ? procedureCode.LaymanTerm : procedureCode.Descript);
			}
			return laymanTerm;
		}

		///<summary>Used to check whether codes starting with T exist and are in a visible category.  If so, it moves them to the Obsolete category.  If the T code has never been used, then it deletes it.</summary>
		public static void TcodesClear() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//first delete any unused T codes
			string command=@"SELECT CodeNum,ProcCode FROM procedurecode
				WHERE CodeNum NOT IN(SELECT CodeNum FROM procedurelog)
				AND CodeNum NOT IN(SELECT CodeNum FROM autocodeitem)
				AND CodeNum NOT IN(SELECT CodeNum FROM procbuttonitem)
				AND CodeNum NOT IN(SELECT CodeNum FROM recalltrigger)
				AND CodeNum NOT IN(SELECT CodeNum FROM benefit)
				AND ProcCode NOT IN(SELECT CodeValue FROM encounter WHERE CodeSystem='CDT')
				AND ProcCode LIKE 'T%'";
			DataTable table=Db.GetTable(command);
			List<long> listCodeNums=new List<long>();
			List<string> listRecallCodes=RecallTypes.GetDeepCopy()
				.SelectMany(x => x.Procedures.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				.ToList();
			for(int i=0;i<table.Rows.Count;i++) {
				if(!listRecallCodes.Contains(PIn.String(table.Rows[i]["ProcCode"].ToString()))) {//The ProcCode is not attached to a recall type.
					listCodeNums.Add(PIn.Long(table.Rows[i]["CodeNum"].ToString()));
				}
			}
			if(listCodeNums.Count>0) {
				ProcedureCodes.ClearFkey(listCodeNums);//Zero securitylog FKey column for rows to be deleted.
				command="SELECT FeeNum FROM fee WHERE CodeNum IN("+String.Join(",",listCodeNums)+")";
				List<long> listFeeNums=Db.GetListLong(command);
				Fees.DeleteMany(listFeeNums);
				command="DELETE FROM proccodenote WHERE CodeNum IN("+String.Join(",",listCodeNums)+")";
				Db.NonQ(command);
				command="DELETE FROM procedurecode WHERE CodeNum IN("+String.Join(",",listCodeNums)+")";
				Db.NonQ(command);
			}
			//then, move any other T codes to obsolete category
			command=@"SELECT DISTINCT ProcCat FROM procedurecode,definition 
				WHERE procedurecode.ProcCode LIKE 'T%'
				AND definition.IsHidden=0
				AND procedurecode.ProcCat=definition.DefNum";
			table=Db.GetTable(command);
			long catNum=Defs.GetByExactName(DefCat.ProcCodeCats,"Obsolete");//check to make sure an Obsolete category exists.
			Def def;
			if(catNum!=0) {//if a category exists with that name
				def=Defs.GetDef(DefCat.ProcCodeCats,catNum);
				if(!def.IsHidden) {
					def.IsHidden=true;
					Defs.Update(def);
					Defs.RefreshCache();
				}
			}
			if(catNum==0) {
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProcCodeCats);
				def=new Def();
				def.Category=DefCat.ProcCodeCats;
				def.ItemName="Obsolete";
				def.ItemOrder=listDefs.Count;
				def.IsHidden=true;
				Defs.Insert(def);
				Defs.RefreshCache();
				catNum=def.DefNum;
			}
			for(int i=0;i<table.Rows.Count;i++) {
				command="UPDATE procedurecode SET ProcCat="+POut.Long(catNum)
					+" WHERE ProcCat="+table.Rows[i][0].ToString()
					+" AND procedurecode.ProcCode LIKE 'T%'";
				Db.NonQ(command);
			}
			//finally, set Never Used category to be hidden.  This isn't really part of clearing Tcodes, but is required
			//because many customers won't have that category hidden
			catNum=Defs.GetByExactName(DefCat.ProcCodeCats,"Never Used");
			if(catNum!=0) {//if a category exists with that name
				def=Defs.GetDef(DefCat.ProcCodeCats,catNum);
				if(!def.IsHidden) {
					def.IsHidden=true;
					Defs.Update(def);
					Defs.RefreshCache();
				}
			}
		}

		public static void ResetApptProcsQuickAdd() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command= "DELETE FROM definition WHERE Category=3";
			Db.NonQ(command);
			string[] array=new string[] {
				"CompEx-4BW-Pano-Pro-Flo","D0150,D0274,D0330,D1110,D1208",
				"CompEx-2BW-Pano-ChPro-Flo","D0150,D0272,D0330,D1120,D1208",
				"PerEx-4BW-Pro-Flo","D0120,D0274,D1110,D1208",
				"LimEx-PA","D0140,D0220",
				"PerEx-4BW-Pro-Flo","D0120,D0274,D1110,D1208",
				"PerEx-2BW-ChildPro-Flo","D0120,D0272,D1120,D1208",
				"Comp Exam","D0150",
				"Per Exam","D0120",
				"Lim Exam","D0140",
				"1 PA","D0220",
				"2BW","D0272",
				"4BW","D0274",
				"Pano","D0330",
				"Pro Adult","D1110",
				"Fluor","D1208",
				"Pro Child","D1120",
				"PostOp","N4101",
				"DentAdj","N4102",
				"Consult","D9310"
			};
			Def def;
			string[] codelist;
			bool allvalid;
			int itemorder=0;
			for(int i=0;i<array.Length;i+=2) {
				//first, test all procedures for valid
				codelist=array[i+1].Split(',');
				allvalid=true;
				for(int c=0;c<codelist.Length;c++) {
					if(!ProcedureCodes.IsValidCode(codelist[c])) {
						allvalid=false;
					}
				}
				if(!allvalid) {
					continue;
				}
				def=new Def();
				def.Category=DefCat.ApptProcsQuickAdd;
				def.ItemOrder=itemorder;
				def.ItemName=array[i];
				def.ItemValue=array[i+1];
				Defs.Insert(def);
				itemorder++;
			}
		}

		public static void ResetApptProcsQuickAddCA() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command= "DELETE FROM definition WHERE Category=3";
			Db.NonQ(command);
			string[] array=new string[] {
				"Exam Recall","01202",
				"Exam Spec","01204",
				"Consult","01703",
				"Occlusal Adjustment","16511",
				"Exam Comp Perm","01103",
				"Exam Comp Mixed","01102",
				"Exam Comp Pri","01101",
				"Exam NP Limited","01201",
				"Exam Emerg","01205",
				"Polishing 1 unit","11101",
				"Polishing .5 unit","11107",
				"Scaling 1 unit","11111",
				"Scaling 2 units","11112",
				"Scaling 3 units","11113",
				"Scaling .5 unit","11117",
				"Fluoride Varnish","12113",
				"Fluoride Foam","12112",
				"Sealant","13401",
				"Sealant add'l","13409",
				"Pan","02601",
				"CT Scan","07043",
				"1 PA","02111",
				"2 PA's","02112",
				"3 PA's","02113",
				"4 PA's","02114",
				"1 BW","02141",
				"2 BW's","02142",
				"4 BW's","02144",
				"OHI","13211",
				"Post-op Check","79601"
			};
			Def def;
			int itemorder=0;
			for(int i=0;i<array.Length;i+=2) {
				if(!ProcedureCodes.IsValidCode(array[i+1])) {//first, test all procedures for valid
					continue;
				}
				def=new Def {
					Category=DefCat.ApptProcsQuickAdd,
					ItemOrder=itemorder++,
					ItemName=array[i],
					ItemValue=array[i+1]
				};
				Defs.Insert(def);
			}
		}

		///<summary>Resets the descriptions and abbreviations for all ADA codes to the official wording.  Descriptions are required by the license, we set the abbreviations.</summary>
		public static int ResetADAdescriptionsAndAbbrs() {
			//No need to check RemotingRole; no call to db.
			return ResetADAdescriptionsAndAbbrs(CDT.Class1.GetADAcodes());
		}

		///<summary>Resets the descriptions and abbreviations for all ADA codes to the official wording.  Descriptions are required by the license, we set the abbreviations.</summary>
		public static int ResetADAdescriptionsAndAbbrs(List<ProcedureCode> codeList) {
			//No need to check RemotingRole; no call to db.
			ProcedureCode code;
			int count=0;
			for(int i=0;i<codeList.Count;i++) {
				if(!ProcedureCodes.IsValidCode(codeList[i].ProcCode)) {//If this code is not in this database
					continue;
				}
				code=ProcedureCodes.GetProcCode(codeList[i].ProcCode);
				DateTime datePrevious=code.DateTStamp;
				bool isDescriptMatch=code.Descript==codeList[i].Descript;
				bool isDbProcAbbrDescBlank=string.IsNullOrWhiteSpace(code.AbbrDesc);
				if(!isDescriptMatch || isDbProcAbbrDescBlank) {//Only increments one time for each code if there are changes necessary.
					count++;
				}
				if(!isDescriptMatch) {//Update description.
					string oldDescript=code.Descript;
					code.Descript=codeList[i].Descript;
					ProcedureCodes.Update(code);		
					SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,"Code "+code.ProcCode+" changed from '"+oldDescript+"' to '"+code.Descript+"' by D-Codes Tool."
						,code.CodeNum,datePrevious);
				}
				if(isDbProcAbbrDescBlank) {//Update abbreviation if current code.AbbrDesc in db is blank.
					string oldAbbrDesc=code.AbbrDesc;
					code.AbbrDesc=codeList[i].AbbrDesc;
					ProcedureCodes.Update(code);
					SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,$"Code {code.ProcCode} changed from '{oldAbbrDesc}' to '{code.AbbrDesc}' by D-Codes Tool."
						,code.CodeNum,datePrevious);
				}
			}
			return count;
			//don't forget to refresh procedurecodes.
		}

		///<summary>Returns true if the ADA missed appointment procedure code is in the database to identify how to track missed appointments.</summary>
		public static bool HasMissedCode() {
			//No need to check RemotingRole; no call to db.
			return _procedureCodeCache.GetContainsKey("D9986");
		}

		///<summary>Returns true if the ADA cancelled appointment procedure code is in the database to identify how to track cancelled appointments.</summary>
		public static bool HasCancelledCode() {
			//No need to check RemotingRole; no call to db.
			return _procedureCodeCache.GetContainsKey("D9987");
		}

		///<summary>Gets a list of procedureCodes from the cache using a comma-delimited list of ProcCodes.
		///Returns a new list is the passed in string is empty.</summary>
		public static List<ProcedureCode> GetFromCommaDelimitedList(string codeStr) {
			List<ProcedureCode> retVal=new List<ProcedureCode>();
			if(String.IsNullOrEmpty(codeStr)) {
				return retVal;
			}
			string[] arrayProcCodes=codeStr.Split(new char[] { ',' });
			foreach(string code in arrayProcCodes) {
				retVal.Add(ProcedureCodes.GetProcCode(code));
			}
			return retVal;
		}

		public static List<ProcedureCode> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcedureCode>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * from procedurecode ORDER BY ProcCode";
			return Crud.ProcedureCodeCrud.SelectMany(command);
		}

		///<summary>Gets all procedurecodes from the database ordered by cat then code.  This is how the cache is ordered.</summary>
		public static List<ProcedureCode> GetAllCodesOrderedCatCode() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcedureCode>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM procedurecode ORDER BY ProcCat,ProcCode";
			return Crud.ProcedureCodeCrud.SelectMany(command);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching codeNum as FKey and are related to ProcedureCode.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the ProcedureCode table type.</summary>
		public static void ClearFkey(long codeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeNum);
				return;
			}
			Crud.ProcedureCodeCrud.ClearFkey(codeNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching codeNums as FKey and are related to ProcedureCode.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the ProcedureCode table type.</summary>
		public static void ClearFkey(List<long> listCodeNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCodeNums);
				return;
			}
			Crud.ProcedureCodeCrud.ClearFkey(listCodeNums);
		}

		///<summary>Gets all procedurecodes for the specified codenums.</summary>
		public static List<ProcedureCode> GetCodesForCodeNums(List<long> listCodeNums) {
			return _procedureCodeCache.GetWhere(x => ListTools.In(x.CodeNum,listCodeNums));
		}

		///<summary>Gets all ortho procedure code nums from the OrthoPlacementProcsList preference if any are present.
		///Otherwise gets all procedure codes that start with D8</summary>
		public static List<long> GetOrthoBandingCodeNums() {
			//No need to check RemotingRole; no call to db.
			string strListOrthoNums = PrefC.GetString(PrefName.OrthoPlacementProcsList);
			List<long> listCodeNums = new List<long>();
			if(strListOrthoNums!="") {
				return strListOrthoNums.Split(new char[] { ',' }).ToList().Select(x => PIn.Long(x)).ToList();
			}
			else {
				return GetWhereFromList(x => x.ProcCode.ToUpper().StartsWith("D8")).Select(x => x.CodeNum).ToList();
			}
		}

		///<summary>HQ only. Used as output for GetAllEServiceFees.</summary>
		public class EServiceFee {
			public ProcedureCode ESvcProcCode;
			public Fee ESvcFee;
		}

		/* js These are not currently in use.  This probably needs to be consolidated with code from other places.  ProcsColored and InsSpans comes to mind.
		///<summary>Returns true if any of the codes in the list fall within the code range.</summary>
		public static bool IsCodeInRange(List<string> myCodes,string range) {
			for(int i=0;i<myCodes.Count;i++) {
				if(IsCodeInRange(myCodes[i],range)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if myCode is within the code range.  Ex: myCode="D####", range="D####-D####"</summary>
		public static bool IsCodeInRange(string myCode,string range) {
			string code1="";
			string code2="";
			if(range.Contains("-")) {
				string[] codeSplit=range.Split('-');
				code1=codeSplit[0].Trim();
				code2=codeSplit[1].Trim();
			}
			else{
				code1=range.Trim();
				code2=range.Trim();
			}
			if(myCode.CompareTo(code1)<0 || myCode.CompareTo(code2)>0) {
				return false;
			}
			return true;
		}*/
	}

	
	
	


}