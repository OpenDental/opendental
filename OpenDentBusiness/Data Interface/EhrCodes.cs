using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness {
	///<summary>Never insert or update, use cache pattern only.  This is not referencing a real table in the database, it is a static object filled by the contents of the EHR.dll.</summary>
	public class EhrCodes {
		#region CachePattern
		//Atypical cache pattern. Cache is only filled when we have access to the EHR.dll file, otherwise listt will be an empty list of EHR codes (not null, just empty as if the table were there but with no codes in it.)

		///<summary>A list of all EhrCodes.</summary>
		private static List<EhrCode> listt;
		private static object lockListt=new object();

		///<summary>A list of all EhrCodes.</summary>
		public static List<EhrCode> Listt {
			get {
				lock(lockListt) {
					if(listt==null) {//instead of refreshing the cache using the normal pattern we must retrieve the cache from the EHR.dll. No call to DB.
						object ObjEhrCodeList;
						Assembly AssemblyEHR;
						string dllPathEHR;
						if(RemotingClient.RemotingRole==RemotingRole.ServerWeb) {
							dllPathEHR=CodeBase.ODFileUtils.CombinePaths(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,@"bin\EHR.dll");
						}
						else {
							dllPathEHR=CodeBase.ODFileUtils.CombinePaths(System.Windows.Forms.Application.StartupPath,"EHR.dll");
						}
						ObjEhrCodeList=null;
						AssemblyEHR=null;
						if(System.IO.File.Exists(dllPathEHR)) {//EHR.dll is available, so load it up
							AssemblyEHR=Assembly.LoadFile(dllPathEHR);
							Type type=AssemblyEHR.GetType("EHR.EhrCodeList");//namespace.class
							ObjEhrCodeList=Activator.CreateInstance(type);
							object[] args=null;
							listt=Crud.EhrCodeCrud.TableToList((DataTable)type.InvokeMember("GetListt",System.Reflection.BindingFlags.InvokeMethod,null,ObjEhrCodeList,args));
						}
						else {//no EHR.dll. "Return" empty list.
							listt=new List<EhrCode>();
						}
						updateCodeExistsHelper();
					}
					return listt;
				}
			}
			set {
				lock(lockListt) {
					listt=value;
				}
			}
		}

		///<summary>Forces an update to the in-memory list.  The list is filled from an in-memory list that is inside our oubfuscated EHR.dll.  For future reference, this method should never call the database because it is called by a background thread during startup.</summary>
		public static void UpdateList() {
			if(listt==null) {
				List<EhrCode> tempList=Listt;//Forces data to be cached.
			}
			else {
				updateCodeExistsHelper();
			}
		}

		/// <summary>If the number of codes in the code tables (I.e. Snomed, Loinc, Cpt, etc.) are greater than the number we expect then this will set IsInDb=true otherwise false.</summary>
		private static void updateCodeExistsHelper() {
			//No need to check RemotingRole; no call to db.
			if(listt.Count==0) {
				return;
			}
			//Cache lists of codes.
			#region Count Variables
			//Counts from the DB
			long countCdcDB=-1;
			long countCdtDB=-1;
			long countCptDB=-1;
			long countCvxDB=-1;
			long countHcpcsDB=-1;
			long countIcd9DB=-1;
			long countIcd10DB=-1;
			long countLoincDB=-1;
			long countRxNormDB=-1;
			long countSnomedDB=-1;
			long countSopDB=-1;
			//Counts hard-coded from the EhrCodes.Listt. Lowered slightly to give a buffer, in case we decide to remove some codes later.
			const long countCdcList			=5;
			const long countCdtList			=10;
			const long countCptList			=300;
			const long countCvxList			=5;
			const long countHcpcsList		=20;
			const long countIcd9List		=1500;
			const long countIcd10List		=2000;
			const long countLoincList		=20;
			const long countRxNormList	=40;
			const long countSnomedList	=700;
			const long countSopList			=100;
			#endregion
			for(int i=0;i<listt.Count;i++) {
				if(listt[i].IsInDb) {
					continue;//The codes are already present in the database, so we don't need to check again.
				}
				switch(listt[i].CodeSystem) {
					case "AdministrativeSex"://always "in DB", even though there is no DB table 
						listt[i].IsInDb=true;
						break;
					case "CDCREC":
						if(countCdcDB==-1) {
							countCdcDB=Cdcrecs.GetCodeCount();
						}
						if(countCdcDB>countCdcList) {
							listt[i].IsInDb=true;
						}
						break;
					case "CDT":
						if(countCdtDB==-1) {
							countCdtDB=ProcedureCodes.GetCodeCount();
						}
						if(countCdtDB>countCdtList) {
							listt[i].IsInDb=true;
						}
						break;
					case "CPT":
						if(countCptDB==-1) {
							countCptDB=Cpts.GetCodeCount();
						}
						if(countCptDB>countCptList) {
							listt[i].IsInDb=true;
						}
						break;
					case "CVX":
						if(countCvxDB==-1) {
							countCvxDB=Cvxs.GetCodeCount();
						}
						if(countCvxDB>countCvxList) {
							listt[i].IsInDb=true;
						}
						break;
					case "HCPCS":
						if(countHcpcsDB==-1) {
							countHcpcsDB=Hcpcses.GetCodeCount();
						}
						if(countHcpcsDB>countHcpcsList) {
							listt[i].IsInDb=true;
						}
						break;
					case "ICD9CM":
						if(countIcd9DB==-1) {
							countIcd9DB=ICD9s.GetCodeCount();
						}
						if(countIcd9DB>countIcd9List) {
							listt[i].IsInDb=true;
						}
						break;
					case "ICD10CM":
						if(countIcd10DB==-1) {
							countIcd10DB=Icd10s.GetCodeCount();
						}
						if(countIcd10DB>countIcd10List) {
							listt[i].IsInDb=true;
						}
						break;
					case "LOINC":
						if(countLoincDB==-1) {
							countLoincDB=Loincs.GetCodeCount();
						}
						if(countLoincDB>countLoincList) {
							listt[i].IsInDb=true;
						}
						break;
					case "RXNORM":
						if(countRxNormDB==-1) {
							countRxNormDB=RxNorms.GetCodeCount();
						}
						if(countRxNormDB>countRxNormList) {
							listt[i].IsInDb=true;
						}
						break;
					case "SNOMEDCT":
						if(countSnomedDB==-1) {
							countSnomedDB=Snomeds.GetCodeCount();
						}
						if(countSnomedDB>countSnomedList) {
							listt[i].IsInDb=true;
						}
						break;
					case "SOP":
						if(countSopDB==-1) {
							countSopDB=Sops.GetCodeCount();
						}
						if(countSopDB>countSopList) {
							listt[i].IsInDb=true;
						}
						break;
				}
			}

			//This updates the last column "ExistsInDatabse" based on weather or not the code is found in another table in the database.

		}

		///<summary></summary>
		public static void FillCache(DataTable table) {
			//No need to check RemotingRole; no call to db.
			listt=Crud.EhrCodeCrud.TableToList(table);
		}
		#endregion

		///<summary></summary>
		public static string GetMeasureIdsForCode(string codeValue,string codeSystem) {
			//No need to check RemotingRole; no call to db.
			string retval="";
			//System.Diagnostics.Stopwatch sw=new System.Diagnostics.Stopwatch();
			//sw.Start();
			for(int i=0;i<Listt.Count;i++) {
				if(Listt[i].CodeValue==codeValue && Listt[i].CodeSystem==codeSystem) {
					if(retval.Contains(Listt[i].MeasureIds)) {
						continue;
					}
					if(retval!="") {
						retval+=",";
					}
					retval+=Listt[i].MeasureIds;
				}
			}
			//sw.Stop();
			return retval;
		}

		///<summary>Returns a list of EhrCode objects that belong to one of the value sets identified by the ValueSetOIDs supplied.</summary>
		public static List<EhrCode> GetForValueSetOIDs(List<string> listValueSetOIDs) {
			return GetForValueSetOIDs(listValueSetOIDs,false);
		}

		///<summary>Returns a list of EhrCode objects that belong to one of the value sets identified by the ValueSetOIDs supplied AND only those codes that exist in the corresponding table in the database.</summary>
		public static List<EhrCode> GetForValueSetOIDs(List<string> listValueSetOIDs,bool usingIsInDb) {
			List<EhrCode> retval=new List<EhrCode>();
			for(int i=0;i<Listt.Count;i++) {
				if(usingIsInDb && !Listt[i].IsInDb) {
					continue;
				}
				if(listValueSetOIDs.Contains(Listt[i].ValueSetOID)) {
					retval.Add(Listt[i]);
				}
			}
			return retval;
		}

		///<summary>Returns a dictionary of CodeValue and CodeSystem pairs where the value set is in the supplied list.</summary>
		public static Dictionary<string,string> GetCodeAndCodeSystem(List<string> listValueSetOIDs,bool usingIsInDb) {
			Dictionary<string,string> retval=new Dictionary<string,string>();
			for(int i=0;i<Listt.Count;i++) {
				if(usingIsInDb && !Listt[i].IsInDb) {
					continue;
				}
				for(int j=0;j<listValueSetOIDs.Count;j++) {
					if(Listt[i].ValueSetOID!=listValueSetOIDs[j]) {
						continue;
					}
					if(!retval.ContainsKey(Listt[i].CodeValue)) {
						retval.Add(Listt[i].CodeValue,Listt[i].CodeSystem);
					}
					break;
				}
			}
			return retval;
		}

		///<summary>Returns a dictionary of CodeValue,CodeSystem pairs of all codes that belong to every ValueSetOID sent in the incoming list as long as the code exists in the corresponding table in the database.</summary>
		public static Dictionary<string,string> GetCodesExistingInAllSets(List<string> listValueSetOIDs) {
			Dictionary<string,string> retval=new Dictionary<string,string>();
			Dictionary<string,int> codecount=new Dictionary<string,int>();
			for(int i=0;i<Listt.Count;i++) {
				if(!Listt[i].IsInDb) {
					continue;
				}
				for(int j=0;j<listValueSetOIDs.Count;j++) {
					if(Listt[i].ValueSetOID!=listValueSetOIDs[j]) {
						continue;
					}
					string keyCur=Listt[i].CodeValue+","+Listt[i].CodeSystem;
					if(codecount.ContainsKey(keyCur)) {
						codecount[keyCur]++;//code already in list, increase find count
					}
					else {
						codecount.Add(keyCur,1);//new find
					}
				}
			}
			foreach(KeyValuePair<string,int> kpairCur in codecount) {
				string[] codeValueSystem=kpairCur.Key.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
				if(retval.ContainsKey(codeValueSystem[0])) {
					continue;
				}
				if(kpairCur.Value==listValueSetOIDs.Count) {
					retval.Add(codeValueSystem[0],codeValueSystem[1]);
				}
			}
			return retval;
		}

		public static List<string> GetValueSetOIDsForCode(string codeValue,string codeSystem) {
			List<string> retval=new List<string>();
			for(int i=0;i<Listt.Count;i++) {
				if(retval.Contains(Listt[i].ValueSetOID)) {
					continue;
				}
				if(Listt[i].CodeValue==codeValue && Listt[i].CodeSystem==codeSystem) {
					retval.Add(Listt[i].ValueSetOID);
				}
			}
			return retval;
		}

		///<summary>Returns EhrCodes for the specified EhrMeasureEventType ordered by how often and how recently they have been used.  Results are
		///ordered by applying a weight based on the date diff from current date to DateTEvent of the EhrMeasureEvents.  EhrCodes used most
		///recently will have the largest weight and help move the EhrCode to the top of the list.  Specify a limit amount if the result set should only
		///be a certain number of EhrCodes at most.</summary>
		public static List<EhrCode> GetForEventTypeByUse(EhrMeasureEventType ehrMeasureEventTypes) {
			List<EhrCode> retVal=new List<EhrCode>();
			//list of CodeValueResults of the specified type ordered by a weight calculated by summing values based on how recently the codes were used
			List<string> listCodes=EhrMeasureEvents.GetListCodesUsedForType(ehrMeasureEventTypes);
			foreach(string codeStr in listCodes) {
				EhrCode codeCur=Listt.FirstOrDefault(x => x.CodeValue==codeStr);
				Snomed sCur=null;
				if(codeCur==null) {
					sCur=Snomeds.GetByCode(codeStr);
					if(sCur==null) {
						continue;
					}
					codeCur=new EhrCode { CodeValue=sCur.SnomedCode,Description=sCur.Description };
				}
				retVal.Add(codeCur);
			}
			return retVal.OrderBy(x => x.Description).ToList();
		}

		///<summary>Returns a list of EhrCodes for the specified InterventionCodeSet and ValueSetOID for any medication interventions.  Results will
		///contain both intervention codes and medication codes that have been used in the last year.</summary>
		public static List<EhrCode> GetForIntervAndMedByUse(InterventionCodeSet codeSet,List<string> listMedValueSetOIDs) {
			//get all MedicationPats where the RxCui is one of the RxCui strings for list of ValueSetOIDs provided
			List<string> listMedPats=MedicationPats.GetAllForRxCuis(GetForValueSetOIDs(listMedValueSetOIDs,true).Select(x => x.CodeValue).ToList());
			List<string> listInterventions=Interventions.GetAllForCodeSet(codeSet);
			List<EhrCode> retVal=new List<EhrCode>();
			foreach(string codeStr in listMedPats.Union(listInterventions)) {
				EhrCode codeCur=Listt.FirstOrDefault(x => x.CodeValue==codeStr);
				if(codeCur==null) {
					continue;
				}
				retVal.Add(codeCur);
			}
			//we might find the "wrong" ehr code, because a single code may be in multiple code sets. This is still a valid code.
			return retVal.OrderBy(x => x.Description).ToList();
		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<string> GetValueSetFromCodeAndCategory(string codeValue,string codeSystem,string category) {
			List<string> retval=new List<string>();
			for(int i=0;i<Listt.Count;i++) {
				if(Listt[i].CodeValue==codeValue && Listt[i].CodeSystem==codeSystem && Listt[i].QDMCategory==category) {
					retval.Add(Listt[i].ValueSetName);
				}
			}
			return retval;
		}

		///<summary>Used for adding codes, returns a hashset of codevalue+valuesetoid.</summary>
		public static HashSet<string> GetAllCodesHashSet() {
			HashSet<string> retVal=new HashSet<string>();
			for(int i=0;i<Listt.Count;i++) {
				retVal.Add(Listt[i].CodeValue+Listt[i].ValueSetOID);
			}
			return retVal;
		}
		 * 
		///<summary></summary>
		public static List<EhrCode> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrCode>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrcode WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrCodeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrCode ehrCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrCode.EhrCodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrCode);
				return ehrCode.EhrCodeNum;
			}
			return Crud.EhrCodeCrud.Insert(ehrCode);
		}

		///<summary>Gets one EhrCode from the db.</summary>
		public static EhrCode GetOne(long ehrCodeNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrCode>(MethodBase.GetCurrentMethod(),ehrCodeNum);
			}
			return Crud.EhrCodeCrud.SelectOne(ehrCodeNum);
		}

		///<summary></summary>
		public static void Update(EhrCode ehrCode){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrCode);
				return;
			}
			Crud.EhrCodeCrud.Update(ehrCode);
		}

		///<summary></summary>
		public static void Delete(long ehrCodeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrCodeNum);
				return;
			}
			string command= "DELETE FROM ehrcode WHERE EhrCodeNum = "+POut.Long(ehrCodeNum);
			Db.NonQ(command);
		}
		*/
	}
}