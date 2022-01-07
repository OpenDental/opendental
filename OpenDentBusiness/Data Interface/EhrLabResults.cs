using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabResults {
		///<summary></summary>
		public static EhrLabResult InsertItem(EhrLabResult ehrLabResult) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrLabResult>(MethodBase.GetCurrentMethod(),ehrLabResult);
			}
			ehrLabResult.EhrLabResultNum=Crud.EhrLabResultCrud.Insert(ehrLabResult);
			for(int i=0;i<ehrLabResult.ListEhrLabResultNotes.Count;i++) {//save attached notes.
				ehrLabResult.ListEhrLabResultNotes[i].EhrLabNum=ehrLabResult.EhrLabNum;
				ehrLabResult.ListEhrLabResultNotes[i].EhrLabResultNum=ehrLabResult.EhrLabResultNum;
				EhrLabNotes.Insert(ehrLabResult.ListEhrLabResultNotes[i]);
			}
			return ehrLabResult;
		}

		///<summary>Does not insert lab result notes if attached.  Use InsertItem instead.</summary>
		public static long Insert(EhrLabResult ehrLabResult) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrLabResult.EhrLabResultNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabResult);
				return ehrLabResult.EhrLabResultNum;
			}
			return Crud.EhrLabResultCrud.Insert(ehrLabResult);
		}

		///<summary>Get all lab results for one patient.</summary>
		public static List<EhrLabResult> GetAllForPatient(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabresult WHERE EhrLabNum IN (SELECT EhrLabNum FROM ehrlab WHERE PatNum="+POut.Long(patNum)+")";
			return Crud.EhrLabResultCrud.SelectMany(command);
		}

		///<summary>Returns all EhrLabResults for a given EhrLab.</summary>
		public static List<EhrLabResult> GetForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabResult>>(MethodBase.GetCurrentMethod(),ehrLabNum);
			}
			string command="SELECT * FROM ehrlabresult WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			return Crud.EhrLabResultCrud.SelectMany(command);
		}

		///<summary>Only deletes the notes for the Lab, there may still be notes attached to LabResults, that are attached to the lab.  Those notes are taken care of by DeleteForLabResults().</summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			string command="DELETE FROM ehrlabresult WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			Db.NonQ(command);
		}

		///<summary>Helper function to return a list of descriptions for the HL70078 enumeration.</summary>
		public static List<string> GetHL70078Descriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("Abnormal");//A
			retVal.Add("Above absolute high-off instrument scale");//_gt
			retVal.Add("Above high normal");//H
			retVal.Add("Above upper panic limits");//HH
			retVal.Add("Below absolute low-off instrument scale");//_lt
			retVal.Add("Below low normal");//L
			retVal.Add("Below lower panic limits");//LL
			retVal.Add("Better");//B
			retVal.Add("Intermediate");//I
			retVal.Add("Moderately susceptible");//MS
			retVal.Add("No range defined");//_null
			retVal.Add("Normal");//N
			retVal.Add("Resistant");//R
			retVal.Add("Significant change down");//D
			retVal.Add("Significant change up");//U
			retVal.Add("Susceptible");//S
			retVal.Add("Very abnormal");//AA
			retVal.Add("Very susceptible");//VS
			retVal.Add("Worse");//W
			return retVal;
		}

		///<summary>Helper function to return a list of descriptions for the HL70085 enumeration.  First item in the list is blank.</summary>
		public static List<string> GetHL70085Descriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("");//Blank
			retVal.Add("Deletes OBX");//D
			retVal.Add("Final results");//F
			retVal.Add("Not asked");//N
			retVal.Add("Order description only");//O
			retVal.Add("Partial results");//S
			retVal.Add("Wrong");//W
			retVal.Add("Preliminary results");//P
			retVal.Add("Correction");//C
			retVal.Add("Cannot be obtained");//X
			retVal.Add("Results entered (not verified)");//R
			retVal.Add("Final w/o retransmitting");//U
			retVal.Add("Specimen in lab");//I
			return retVal;
		}

		///<summary>Helper function to return a list of descriptions for the HL70125 enumeration.  First item in the list is blank.</summary>
		public static List<string> GetHL70125Descriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("");//Blank
			retVal.Add("Coded entry");//CE
			retVal.Add("Coded with exceptions");//CWE
			retVal.Add("Date");//DT
			retVal.Add("Formatted text");//FT
			retVal.Add("Numeric");//NM
			retVal.Add("Structured numeric");//SN
			retVal.Add("String data");//ST
			retVal.Add("Time");//TM
			retVal.Add("Time stamp");//TS
			retVal.Add("Text data");//TX
			return retVal;
		}

		///<summary>Helper function to return a list of descriptions for the HL70190 enumeration.  First item in the list is blank.</summary>
		public static List<string> GetHL70190Descriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("");//Blank
			retVal.Add("Bad address");//BA
			retVal.Add("Birth address, not otherwise specified");//N
			retVal.Add("Birth delivery location");//BDL
			retVal.Add("Country of origin");//F
			retVal.Add("Current or temporary");//C
			retVal.Add("Firm/Business");//B
			retVal.Add("Home");//H
			retVal.Add("Legal address");//L
			retVal.Add("Mailing");//M
			retVal.Add("Office");//O
			retVal.Add("Permanent");//P
			retVal.Add("Registry home");//RH
			retVal.Add("Residence at birth");//BR
			return retVal;
		}

		///<summary>Helper function to return a list of descriptions for the HL70200 enumeration.  First item in the list is blank.</summary>
		public static List<string> GetHL70200Descriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("");//Blank
			retVal.Add("Adopted Name");//C
			retVal.Add("Alias Name");//A
			retVal.Add("Coded Pseudo-Name");//S
			retVal.Add("Display Name");//D
			retVal.Add("Indigenous/Tribal/Community Name");//T
			retVal.Add("Legal Name");//L
			retVal.Add("Licensing Name");//I
			retVal.Add("Maiden Name");//M
			retVal.Add("Name at Birth");//B
			retVal.Add("Name of Partner/Spouse");//P
			retVal.Add("Nickname");//N
			retVal.Add("Registered Name");//R
			retVal.Add("Unspecified");//U
			return retVal;
		}


		///<summary>Helper function to return a list of descriptions for the USPSAlphaStateCode enumeration.  First item in the list is blank.</summary>
		public static List<string> GetUSPSAlphaStateCodeDescriptions() {
			//No need to check RemotingRole;
			List<string> retVal=new List<string>();
			retVal.Add("");//Blank
			retVal.Add("Alabama");//AL
			retVal.Add("Alaska");//AK
			retVal.Add("Arizona");//AZ
			retVal.Add("Arkansas");//AR
			retVal.Add("California");//CA
			retVal.Add("Colorado");//CO
			retVal.Add("Connecticut");//CT
			retVal.Add("Delaware");//DE
			retVal.Add("District of Columbia");//DC
			retVal.Add("Florida");//FL
			retVal.Add("Georgia");//GA
			retVal.Add("Hawaii");//HI
			retVal.Add("Idaho");//ID
			retVal.Add("Illinois");//IL
			retVal.Add("Indiana");//IN
			retVal.Add("Iowa");//IA
			retVal.Add("Kansas");//KS
			retVal.Add("Kentucky");//KY
			retVal.Add("Louisiana");//LA
			retVal.Add("Maine");//ME
			retVal.Add("Maryland");//MD
			retVal.Add("Massachusetts");//MA
			retVal.Add("Michigan");//MI
			retVal.Add("Minnesota");//MN
			retVal.Add("Mississippi");//MS
			retVal.Add("Missouri");//MO
			retVal.Add("Montana");//MT
			retVal.Add("Nebraska");//NE
			retVal.Add("Nevada");//NV
			retVal.Add("New Hampshire");//NH
			retVal.Add("New Jersey");//NJ
			retVal.Add("New Mexico");//NM
			retVal.Add("New York");//NY
			retVal.Add("North Carolina");//NC
			retVal.Add("North Dakota");//ND
			retVal.Add("Ohio");//OH
			retVal.Add("Oklahoma");//OK
			retVal.Add("Oregon");//OR
			retVal.Add("Pennsylvania");//PA
			retVal.Add("Rhode Island");//RI
			retVal.Add("South Carolina");//SC
			retVal.Add("South Dakota");//SD
			retVal.Add("Tennessee");//TN
			retVal.Add("Texas");//TX
			retVal.Add("Utah");//UT
			retVal.Add("Vermont");//VT
			retVal.Add("Virginia");//VA
			retVal.Add("Washington");//WA
			retVal.Add("West Virginia");//WV
			retVal.Add("Wisconsin");//WI
			retVal.Add("Wyoming");//WY
			retVal.Add("American Samoa");//AS
			retVal.Add("Federated States of Micronesia");//FM
			retVal.Add("Guam");//GU
			retVal.Add("Marshall Islands");//MH
			retVal.Add("Northern Mariana Islands");//MP
			retVal.Add("Palau");//PW
			retVal.Add("Puerto Rico");//PR
			retVal.Add("U.S. Minor Outlying Islands");//UM
			retVal.Add("Virgin Islands of the U.S.");//VI
			return retVal;
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class EhrLabResultCache : CacheListAbs<EhrLabResult> {
			protected override List<EhrLabResult> GetCacheFromDb() {
				string command="SELECT * FROM EhrLabResult ORDER BY ItemOrder";
				return Crud.EhrLabResultCrud.SelectMany(command);
			}
			protected override List<EhrLabResult> TableToList(DataTable table) {
				return Crud.EhrLabResultCrud.TableToList(table);
			}
			protected override EhrLabResult Copy(EhrLabResult EhrLabResult) {
				return EhrLabResult.Clone();
			}
			protected override DataTable ListToTable(List<EhrLabResult> listEhrLabResults) {
				return Crud.EhrLabResultCrud.ListToTable(listEhrLabResults,"EhrLabResult");
			}
			protected override void FillCacheIfNeeded() {
				EhrLabResults.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EhrLabResult EhrLabResult) {
				return !EhrLabResult.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrLabResultCache _EhrLabResultCache=new EhrLabResultCache();

		///<summary>A list of all EhrLabResults. Returns a deep copy.</summary>
		public static List<EhrLabResult> ListDeep {
			get {
				return _EhrLabResultCache.ListDeep;
			}
		}

		///<summary>A list of all visible EhrLabResults. Returns a deep copy.</summary>
		public static List<EhrLabResult> ListShortDeep {
			get {
				return _EhrLabResultCache.ListShortDeep;
			}
		}

		///<summary>A list of all EhrLabResults. Returns a shallow copy.</summary>
		public static List<EhrLabResult> ListShallow {
			get {
				return _EhrLabResultCache.ListShallow;
			}
		}

		///<summary>A list of all visible EhrLabResults. Returns a shallow copy.</summary>
		public static List<EhrLabResult> ListShort {
			get {
				return _EhrLabResultCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EhrLabResultCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EhrLabResultCache.FillCacheFromTable(table);
				return table;
			}
			return _EhrLabResultCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabResult> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabResult>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabresult WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabResultCrud.SelectMany(command);
		}

		///<summary>Gets one EhrLabResult from the db.</summary>
		public static EhrLabResult GetOne(long ehrLabResultNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrLabResult>(MethodBase.GetCurrentMethod(),ehrLabResultNum);
			}
			return Crud.EhrLabResultCrud.SelectOne(ehrLabResultNum);
		}

		///<summary></summary>
		public static void Update(EhrLabResult ehrLabResult){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabResult);
				return;
			}
			Crud.EhrLabResultCrud.Update(ehrLabResult);
		}

		///<summary></summary>
		public static void Delete(long ehrLabResultNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabResultNum);
				return;
			}
			string command= "DELETE FROM ehrlabresult WHERE EhrLabResultNum = "+POut.Long(ehrLabResultNum);
			Db.NonQ(command);
		}
		*/
	}
}