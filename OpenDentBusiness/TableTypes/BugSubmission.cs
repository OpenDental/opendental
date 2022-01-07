using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace OpenDentBusiness {

	///<summary>The BugSubmission table lives on the bugs database.
	///All queries should be done with the bugs database context from WebServiceMainHQ.
	///This table is only in OpenDentBusiness because the bug table from the bugs database lives here as well,
	/// and we want to follow that pattern since this table links to it.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class BugSubmission:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long BugSubmissionNum;
		/// <summary>Automatically set to the date and time upon insert. Uses server time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SubmissionDateTime;
		///<summary>FK to bug.BugId. Can be 0.</summary>
		public long BugId;
		///<summary>The value of PrefName.RegistrationKey from the submitting office's DB.
		///Automatically set in constructor.</summary>
		public string RegKey;
		///<summary>The value of PrefName.DataBaseVersion from the database when this bug was submitted.</summary>
		public string DbVersion;
		///<summary>The string from an excetions.GetMessage</summary>
		public string ExceptionMessageText;
		///<summary>The raw full statck trace.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ExceptionStackTrace;
		///<summary>A JSON object storing important key value pairs for backwards/forwards compatibility.
		///Will contain important preferences, as well as DbInfoField values, in the "key":"value" format of JSON
		///Automatically set in constructor.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DbInfoJson;
		///<summary>Used to add general notes to a submissions.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DevNote;
		///<summary>True when this bug submission should not be shown in UI, otherwise false.</summary>
		public bool IsHidden;
		///<summary>CSV category tags.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string CategoryTags;
		///<summary>FK to bugsubmissionhash.BugSubmissionHashNum.</summary>
		public long BugSubmissionHashNum;

		///<summary>Sometimes set to the bug object identified by the BugId column.  Will be null if instantiating method did not set or no bug found.
		///This property is passed over MiddleTier so do not add XmlIgnore or JsonIgnore attributes.</summary>
		[XmlIgnore, JsonIgnore]
		public Bug BugObj { get; set; }

		///<summary></summary>
		[XmlIgnore, JsonIgnore]
		public List<string> ListCategoryTags {
			get {
				return CategoryTags.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			}
		}

		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private string _simplifiedStackTrace;

		///<summary>Returns ExceptionStackTrace without the line numbers and file paths.</summary>
		[XmlIgnore,JsonIgnore]
		public string SimplifiedStackTrace {
			get {
				if(_simplifiedStackTrace==null) {
					_simplifiedStackTrace=BugSubmissions.SimplifyStackTrace(ExceptionStackTrace);
				}
				return _simplifiedStackTrace;
			}
		}
		
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private List<string> _listOdExceptionLines;

		///<summary>Returns list of OD exception lines from ExceptionStackTrace.</summary>
		[XmlIgnore,JsonIgnore]
		public List<string> ListOdExceptionLines {
			get {
				if(_listOdExceptionLines==null) {
					_listOdExceptionLines=new List<string>();
					List<string> listExceptionLines=this.SimplifiedStackTrace.Split('\n').ToList();
					foreach(string line in listExceptionLines) {
						if(!line.ToLower().Contains("opendent")&&!line.ToLower().Contains("codebase")) {
							continue;
						}
						_listOdExceptionLines.Add(line);
					}
				}
				return _listOdExceptionLines;
			}
		}
		
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private string _odStackSignature;

		///<summary>Helper to who ListOdExceptionLines in a single string.</summary>
		[XmlIgnore,JsonIgnore]
		public string OdStackSignature {
			get {
				if(string.IsNullOrEmpty(_odStackSignature)) {
					StringBuilder sb=new StringBuilder();
					ListOdExceptionLines.ForEach(x => sb.AppendLine(x));
					_odStackSignature=sb.ToString();
				}
				return _odStackSignature;
			}
		}
		
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private SubmissionInfo _info;
		
		///<summary>Any additional information gathered at the time the bug is submitted.</summary>
		[XmlIgnore,JsonIgnore]
		public SubmissionInfo Info {
			get {
				if(_info==null) {
					_info=JsonConvert.DeserializeObject<SubmissionInfo>(DbInfoJson);
				}
				return _info;
			}
			//set { } Read only property
		}

		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private string _programVersion;

		///<summary>When IsMobileSubmission is false this is the value of PrefName.ProgramVersion from the database when this bug was submitted.
		///Otherwise when IsMobileSubmission is true this is the value of the eServiceAccount.ProgramVersion when this bug was submitted.
		///This is a helper property that is designed to be used in lieu of the Info property when only the ProgramVersion info is needed.
		///This is mainly due to JsonConvert.DeserializeObject being CPU intensive.  This property uses Regex which is far less CPU intensive.</summary>
		[XmlIgnore, JsonIgnore]
		public string ProgramVersion {
			get {
				if(_programVersion==null) {
					if(IsMobileSubmission) {
						_programVersion=Info.MobileProgramVersion;
					}
					else {
						//The regular expression to pull out the program version from the DbInfoJson column.
						//The format within the JSON will be like "ProgramVersion":"17.4.44.0"
						Regex programVersionRegex=new Regex(@"""ProgramVersion"":""(([\d]+\.?){4})""");
						Match match=programVersionRegex.Match(DbInfoJson);
						_programVersion=match.Success ? match.Groups[1].Value : "0.0.0.0";
					}
				}
				return _programVersion;
			}
		}

		///<summary></summary>
		[XmlIgnore, JsonIgnore]
		public bool IsMobileSubmission {
			get { return !Info.DeviceId.IsNullOrEmpty(); }
		}

		///<summary>List of bugs and additional metadata that is not automatically set.
		///Should typically be set to bugs that share the same BugId or stack trace hashes (see BugSubmissionsHashes).</summary>
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		public List<MatchedBugInfo> ListMatchedBugInfos;
		
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private string _hashedStackTrace=null;

		/// <summary></summary>
		[XmlIgnore,JsonIgnore]
		public string HashedStackTrace{
			get{
				if(_hashedStackTrace==null) {
					HashAlgorithm algorithm=SHA256.Create();
					_hashedStackTrace=Convert.ToBase64String(algorithm.ComputeHash(Encoding.Unicode.GetBytes(this.ExceptionStackTrace)));
				}
				return _hashedStackTrace;
			}
		}
		
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private string _hashedSimpleStackTrace=null;

		/// <summary></summary>
		[XmlIgnore,JsonIgnore]
		public string HashedSimpleStackTrace{
			get{
				if(_hashedSimpleStackTrace==null) {
					HashAlgorithm algorithm=SHA256.Create();
					_hashedSimpleStackTrace=Convert.ToBase64String(algorithm.ComputeHash(Encoding.Unicode.GetBytes(this.SimplifiedStackTrace)));
				}
				return _hashedSimpleStackTrace;
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary></summary>
		public List<MatchedBugInfo> ListPendingFixBugInfos {
			get { 
				return this.ListMatchedBugInfos.FindAll(x => x.BugId > 0 && x.VersionsFixed.IsNullOrEmpty());
			}
		}
		
		[XmlIgnore,JsonIgnore]
		///<summary></summary>
		public List<MatchedBugInfo> ListFixedBugInfos {
			get { 
				return this.ListMatchedBugInfos.FindAll(x => !x.VersionsFixed.IsNullOrEmpty());
			}
		}

		///<summary>This constructor should only be used when obtaining existing rows from the database.</summary>
		public BugSubmission() {
			
		}

		///<summary>This constructor utilizes the Open Dental preference cache.
		///Only use this constructor from an Open Dental proper instance, never call from other entities (e.g. WebServiceMainHQ).</summary>
		public BugSubmission(Exception e,string threadName="",long patNum=-1,string moduleName="") : this() {
			ExceptionMessageText=e.Message;
			ExceptionStackTrace=MiscUtils.GetExceptionText(e);
			//Check to see if the exception passed in is a special BugSubmissionMessage exception.
			if(e.GetType()==typeof(ODException) && ((ODException)e).ErrorCodeAsEnum==ODException.ErrorCodes.BugSubmissionMessage) {
				//This specific type of ODException expects the ExceptionMessageText and ExceptionStackTrace fields to be overridden.
				//The first line of the UE's Message property will turn into the BugSubmission's ExceptionMessageText field.
				//All subsequent lines will become the BugSubmission's ExceptionStackTrace field.
				string[] lines=e.Message.Split(new[] { Environment.NewLine },StringSplitOptions.None);
				if(lines.Length>=1) {
					ExceptionMessageText=lines[0];
					ExceptionStackTrace=e.Message.Remove(0,lines[0].Length).TrimStart();
				}
			}
			DbInfoJson=GetDbInfoJSON(patNum,moduleName);
			try {
				RegKey=Info.DictPrefValues[PrefName.RegistrationKey];
				DbVersion=Info.DictPrefValues[PrefName.DataBaseVersion];
			}
			catch(Exception ex) {
				ex.DoNothing();
				//The previous lines will fail if there is no database context.  These bug submissions are still desired.
				//Set a fake RegKey value that HQ will accept bug submissions for.
				RegKey="7E57-1NPR-0DUC-710N";
			}
			Info.ThreadName=threadName;
		}
	
		public BugSubmission Copy() {
			return (BugSubmission)this.MemberwiseClone();
		}

		///<summary>Returns serialized DbInfo object as JSON string of database info from both the preference table and non preferernce table info.
		///Every unique bit of information is individually try / caught so that we return as much information as possible.</summary>
		private string GetDbInfoJSON(long patNum,string moduleName) {
			_info=new BugSubmission.SubmissionInfo();
			ODException.SwallowAnyException(() => {
				//This list is not in a separate method because we want to ensure that future development related to bug submissions don't try to make assumptions
				//on which preferences are in an object at any given time.
				//Ex.  Let's say in version 17.4, the list doesn't contain the payplan version preference, but 17.5 does.
				//If we called the method that retrieves the used preferences from WebServiceMainHQ which in this example is on version 17.5,
				// it would think all bugsubmission rows contain the payplan version preference when that is not the case.
				List<PrefName> listPrefs=new List<PrefName>() {
					PrefName.AtoZfolderUsed,
					PrefName.ClaimSnapshotEnabled,
					PrefName.ClaimSnapshotRunTime,
					PrefName.ClaimSnapshotTriggerType,
					PrefName.CorruptedDatabase,
					PrefName.DataBaseVersion,
					PrefName.EasyNoClinics,
					PrefName.LanguageAndRegion,
					PrefName.MySqlVersion,
					PrefName.PayPlansVersion,
					PrefName.ProcessSigsIntervalInSecs,
					//PrefName.ProgramVersionLastUpdated, //deprecated
					PrefName.ProgramVersion,
					PrefName.RandomPrimaryKeys,
					PrefName.RegistrationKey,
					PrefName.RegistrationKeyIsDisabled,
					PrefName.ReplicationFailureAtServer_id,
					PrefName.ReportingServerCompName,
					PrefName.ReportingServerDbName,
					PrefName.ReportingServerMySqlUser,
					PrefName.ReportingServerMySqlPassHash,
					PrefName.ReportingServerURI,
					PrefName.SecurityLogOffAfterMinutes,
					PrefName.WebServiceServerName
				};
				foreach(PrefName pref in listPrefs) {
					_info.DictPrefValues[pref]=Prefs.GetOne(pref).ValueString;
				}
			});
			ODException.SwallowAnyException(() => { _info.CountClinics=Clinics.GetCount(); });
			ODException.SwallowAnyException(() => {
				//External plugins have no program name, so use program description with the plugin name instead. Otherwise, use the program name.
				_info.EnabledPlugins=Programs.GetWhere(x => x.Enabled && !string.IsNullOrWhiteSpace(x.PluginDllName))
					.Select(x => x.ProgName=="" ? $"{x.ProgDesc}({x.PluginDllName})" : x.ProgName).ToList();
			});
			ODException.SwallowAnyException(() => { _info.ClinicNumCur=Clinics.ClinicNum; });
			ODException.SwallowAnyException(() => { _info.UserNumCur=Security.CurUser.UserNum; });
			ODException.SwallowAnyException(() => { _info.PatientNumCur=patNum; });
			ODException.SwallowAnyException(() => { _info.IsOfficeOnReplication=ReplicationServers.IsUsingReplication(); });
			ODException.SwallowAnyException(() => { _info.IsOfficeUsingMiddleTier=(RemotingClient.RemotingRole==RemotingRole.ClientWeb ? true : false); });
			ODException.SwallowAnyException(() => { _info.WindowsVersion=MiscData.GetOSVersionInfo(); });
			ODException.SwallowAnyException(() => { _info.CompName=Security.CurComputerName; });
			ODException.SwallowAnyException(() => {
				List<UpdateHistory> listHist=UpdateHistories.GetPreviousUpdateHistories(2);//Ordered by newer versions first.
				_info.PreviousUpdateVersion=listHist.Count==2 ? listHist[1].ProgramVersion : "";//Show the previous version they updated from
				_info.PreviousUpdateTime=listHist.Count>0 ? listHist[0].DateTimeUpdated : DateTime.MinValue;//Show when they updated to the current version.
			});
			ODException.SwallowAnyException(() => { _info.ModuleNameCur=moduleName; });
			ODException.SwallowAnyException(() => { _info.DatabaseName=DataConnection.GetDatabaseName(); });
			ODException.SwallowAnyException(() => { _info.ServerName=DataConnection.GetServerName(); });
			ODException.SwallowAnyException(() => { _info.StorageEngine=MiscData.GetStorageEngine(); });
			ODException.SwallowAnyException(() => { _info.OpenDentBusinessVersion=MiscData.GetAssemblyVersion(); });
			ODException.SwallowAnyException(() => { _info.OpenDentBusinessMiddleTierVersion=MiscData.GetAssemblyVersionForMiddleTier(); });
			ODException.SwallowAnyException(() => { _info.EnvSessionName=Environment.GetEnvironmentVariable("SESSIONNAME"); });
			return JsonConvert.SerializeObject(_info,new JsonSerializerSettings { NullValueHandling=NullValueHandling.Ignore });
		}

		public string TryGetPrefValue(PrefName prefName,string defaultReturn = "") {
			string prefValue;
			if(Info.DictPrefValues.TryGetValue(PrefName.ProgramVersion,out prefValue)) {
				return prefValue;
			}
			return defaultReturn;
		}

		/// <summary>ListFixedBugInfos must be set prior to calling this.</summary>
		/// <param name="programVersionOverride">Rarely we might need to pass in a target program version when this submissions programversion is not set.
		/// For example this is done in unit test to target a specific match.</param>
		/// <returns>Returns the pertinent MatchedBugInfo.</returns>
		public MatchedBugInfo GetPertinentFixedVersion(Version programVersionOverride=null) {
			Version pertinentFixedVersion=null;
			MatchedBugInfo pertinentBugInfo=null;
			if(programVersionOverride==null){
				programVersionOverride=new Version(this.ProgramVersion);
			}
			foreach(MatchedBugInfo bugInfo in this.ListFixedBugInfos) {
				List<Version> listFixedVersions=bugInfo.VersionsFixed.Split(';').Select(x => new Version(x)).ToList();
				if(BugSubmissions.TryMatchPertinentFixedVersion(programVersionOverride,listFixedVersions,out Version versionFound)){
					if(pertinentFixedVersion==null || pertinentFixedVersion < versionFound) {
						pertinentBugInfo=bugInfo;
					}
					pertinentFixedVersion=new[] { pertinentFixedVersion??new Version(),versionFound }.Max();
				}
			}
			return pertinentBugInfo;
		}
		
		///<summary>This is not a database table.  This is the structure of the DbInfoJSON column.
		///This class doesn't throw exceptions when using JsonConvert.SerializeObject and JsonConvert.DeserializeObject
		/// for missing/additional columns that what it thinks it should get.</summary>
		public class SubmissionInfo {
			#region OD Proper UE submission info
			///<summary></summary>
			public Dictionary<PrefName,string> DictPrefValues=new Dictionary<PrefName, string>();
			///<summary>List of enabled plugin names.</summary>
			public List<string> EnabledPlugins;
			///<summary></summary>
			public int CountClinics;
			///<summary></summary>
			public long UserNumCur;
			///<summary></summary>
			public long PatientNumCur;
			///<summary>The module the user was on when the exception occurred.</summary>
			public string ModuleNameCur;
			///<summary></summary>
			public bool IsOfficeOnReplication;
			///<summary></summary>
			public bool IsOfficeUsingMiddleTier;
			///<summary></summary>
			public string WindowsVersion;
			///<summary></summary>
			public string CompName;
			///<summary>Lets us know which version the user came from.  E.g. this will always be a version prior to CurrentUpdateVersion.
			///Helpful for conversations with the customer and gives us a timeline / story as to where the user came from and how long they were using that previous version.</summary>
			public string PreviousUpdateVersion;
			///<summary>Lets us know when the user updated to this particular version.  E.g. this will always be a date and time prior to CurrentUpdateTime.
			///Helpful for conversations with the customer and gives us a timeline / story as to where the user came from and how long they were using that previous version.</summary>
			public DateTime PreviousUpdateTime;
			///<summary>The version that the user is currently using.</summary>
			public string CurrentUpdateVersion;
			///<summary>The exact time that the user started using the current version.  Helpful in case we have a dangerous upgrade script get exposed to the public.</summary>
			public DateTime CurrentUpdateTime;
			///<summary>The thread that had the UE. This will say ProgramEntry if it was on the main thread of OD proper.</summary>
			public string ThreadName;
			///<summary></summary>
			public string DatabaseName;
			///<summary></summary>
			public string ServerName;
			///<summary></summary>
			public string StorageEngine;
			///<summary></summary>
			public string ConnectionString;//TODO: might not have variable for this.
			///<summary>Typically blank, but when the office is using middle tier, it might be helpful to get their URI so that we can connect from our end (if they make us a user).</summary>
			public string MiddleTierURI;//TODO:Both this and ConnectionString are pulled from a config file.
			///<summary></summary>
			public string OpenDentBusinessVersion;
			///<summary></summary>
			public string OpenDentBusinessMiddleTierVersion;
			///<summary></summary>
			public string EnvSessionName;
			#endregion

			#region Mobile UE submission info
			///<summary></summary>
			public long RegKeyNum;
			///<summary></summary>
			public string DeviceId;
			///<summary>Like E-Clipboard or Open Dental Mobile. (Enum ApplicationTarget: 0=None,1=CheckIin,2=MobileWeb)</summary>
			public string AppTarget;
			///<summary></summary>
			public string EConnectorVersion;
			///<summary>ODUser.UserNum</summary>
			public long MWUserIdentifier;
			///<summary></summary>
			public DateTime TimeSignalsLastReceived;
			///<summary></summary>
			public string DevicePlatform;
			///<summary></summary>
			public string DeviceModel;
			///<summary></summary>
			public string DeviceVersion;
			///<summary></summary>
			public string DeviceManufacturer;
			///<summary></summary>
			public string AppVersion;
			///<summary></summary>
			public string MobileProgramVersion;
			#endregion

			#region Shared submission info (mobile and proper)
			///<summary></summary>
			public long ClinicNumCur;
			#endregion

			public SubmissionInfo() {
				//Needed for middle tier serialization.	
			}
		}

		///<summary>This class is basically a DTO that transfers information to OD regarding information (e.g. the status) of the bug submission.</summary>
		[Serializable]
		public class Response {
			///<summary></summary>
			public BugSubmissionResult SubmissionResult;
			///<summary>A friendly human readable text that can be used in user facing UI/forms.</summary>
			public string DisplayString;
		}

		///<summary>Helper class that contains additional information for a specific bug that matched a submission's hash.</summary>
		public class MatchedBugInfo {
			///<summary></summary>
			public long BugSubmissionHashNum;
			///<summary></summary>
			public long BugId;
			///<summary></summary>
			public string VersionsFixed;
			
			public MatchedBugInfo(long bugId,string versionsFixed):this(0,bugId,versionsFixed) {
				
			}

			public MatchedBugInfo(long bugSubmissionHashNum,long bugId,string versionsFixed) {
				BugSubmissionHashNum=bugSubmissionHashNum;
				BugId=bugId;
				VersionsFixed=versionsFixed;
			}
		}

	}

}

/*
if(DataConnection.DBtype==DatabaseType.MySql) {
	command="DROP TABLE IF EXISTS bugsubmission";
	Db.NonQ(command);
	command=@"CREATE TABLE bugsubmission (
		BugSubmissionNum bigint NOT NULL auto_increment PRIMARY KEY,
		SubmissionDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
		BugId bigint NOT NULL,
		RegKey varchar(255) NOT NULL,
		DbVersion varchar(255) NOT NULL,
		ExceptionMessageText varchar(255) NOT NULL,
		ExceptionStackTrace text NOT NULL,
		DbInfoJson text NOT NULL,
		INDEX(BugId)
		) DEFAULT CHARSET=utf8";
	Db.NonQ(command);
}
else {//oracle
	command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE bugsubmission'; EXCEPTION WHEN OTHERS THEN NULL; END;";
	Db.NonQ(command);
	command=@"CREATE TABLE bugsubmission (
		BugSubmissionNum number(20) NOT NULL,
		SubmissionDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
		BugId number(20) NOT NULL,
		RegKey varchar2(255),
		DbVersion varchar2(255),
		ExceptionMessageText varchar2(255),
		ExceptionStackTrace clob,
		DbInfoJson clob,
		CONSTRAINT bugsubmission_BugSubmissionNum PRIMARY KEY (BugSubmissionNum)
		)";
	Db.NonQ(command);
	command=@"CREATE INDEX bugsubmission_BugId ON bugsubmission (BugId)";
	Db.NonQ(command);
}
*/
