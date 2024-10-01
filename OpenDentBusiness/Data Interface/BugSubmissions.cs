using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DataConnectionBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class BugSubmissions {

		public static IBugSubmissions MockBugSubmissions {
			get;
			set;
		}
		#region Get Methods
		///<summary></summary>
		public static List<BugSubmission> GetAll(bool useConnectionStore=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),useConnectionStore);
			}
			List<BugSubmission> listBugSubmissions=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission";
				listBugSubmissions=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubmissions;
		}

		///<summary>Returns a list of BugSubmissions for the given arrayHashNums.</summary>
		public static List<BugSubmission> GetForHashNums(bool useConnectionStore,params long[] hashNumArray){
			List<BugSubmission> listBugSubmissions=new List<BugSubmission>();
			if(hashNumArray.IsNullOrEmpty()){
				return listBugSubmissions;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),useConnectionStore,hashNumArray);
			}
			DataAction.RunBugsHQ(() => { 
				string command=$@"SELECT * FROM bugsubmission WHERE BugSubmissionHashNum IN ({string.Join(",",hashNumArray.Select(x => POut.Long(x)))})";
				listBugSubmissions=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubmissions;
		}

		///<summary>Returns BugSubmissions for the hash nums provided.  Key: BugSubmissionHashNum  Value: bugsubmissions.</summary>
		public static SerializableDictionary<long,List<BugSubmission>> GetForHashNums(List<long> listHashNums,bool useConnectionStore=false) {
			//Jordan-We would like to remove this dict, but too hard to test, so leaving for now.
			SerializableDictionary<long,List<BugSubmission>> dictionaryHashBugSubs=new SerializableDictionary<long,List<BugSubmission>>();
			if(listHashNums.IsNullOrEmpty()) {
				return dictionaryHashBugSubs;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SerializableDictionary<long,List<BugSubmission>>>(MethodBase.GetCurrentMethod(),listHashNums,useConnectionStore);
			}
			DataAction.RunBugsHQ(() => { 
				List<BugSubmission> listBugSubmissions=GetForHashNums(useConnectionStore,listHashNums.ToArray());
				listHashNums.ForEach(x => 
					dictionaryHashBugSubs.Add(x,listBugSubmissions.FindAll(y => y.BugSubmissionHashNum==x))
				);
			},useConnectionStore);
			return dictionaryHashBugSubs;
		}

		///<summary>Returns a list of bug submissions and their corresponding bugs.
		///If a bugsubmission is not associated to a bug then the BugObj field on the bugsubmission object will be null.
		///Performs grouping logic in order to minimize the amount of bugsubmissions in the return results.</summary>
		public static List<BugSubmission> GetBugSubsForRegKeys(List<string> listRegKeys,DateTime dateFrom,DateTime dateTo) {
			if(listRegKeys==null || listRegKeys.Count==0) {
				return new List<BugSubmission>();//No point in going through middle tier.
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),listRegKeys,dateFrom,dateTo);
			}
			List<BugSubmission> listBugSubmissionsRetVals=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission "
					+"LEFT JOIN bug ON bug.BugId=bugsubmission.BugId "
					+"WHERE bugsubmission.RegKey IN("+string.Join(",",listRegKeys.Select(x => "'"+POut.String(x)+"'"))+") "
					+"AND "+DbHelper.BetweenDates("SubmissionDateTime",dateFrom,dateTo)+" "
					+"ORDER BY bug.CreationDate DESC, bugsubmission.SubmissionDateTime DESC";
				//The query selects all columns for the bugsubmission and bug tables in one query.  Hopefully we never have conflicting columns that differ.
				DataTable table=Db.GetTable(command);
				//Make a clone of the table structure for the bug objects and only fill it with entries where the BugId row is valid.
				DataTable tableBugs=table.Clone();
				List<DataRow> listDataRows = table.Select().Where(x => PIn.Long(x["BugId"].ToString(),false)!=0).ToList();
				for(int i=0;i<listDataRows.Count;i++){
					tableBugs.ImportRow(listDataRows[i]);
				}
				//Extract all of the bug objects from the subset table.
				List<Bug> listBugs=Crud.BugCrud.TableToList(tableBugs);
				//Extract all of the bugsubmission objects from the results.
				List<BugSubmission> listBugSubmissions = Crud.BugSubmissionCrud.TableToList(table);
				//Associate any bug object with its corresponding bugsubmission object.
				for(int i=0;i<listBugSubmissions.Count;i++){
					listBugSubmissions[i].BugObj=listBugs.Find(y => y.BugId==listBugSubmissions[i].BugId);
				}
				//Group the bug submissions by RegKey, ExceptionStackTrace, and BugId.
				listBugSubmissions.GroupBy(x => new { x.RegKey,x.ExceptionStackTrace,x.BugId })
					//Each grouping will be ordered by ProgramVersion individually.
					.ToDictionary(x => x.Key,x => x.OrderByDescending(x => new Version(x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))))
					//Add the first and most pertinent bug submission in the grouping to the return value.
					.ForEach(x => listBugSubmissionsRetVals.Add(x.Value.First()));
			},false);
			return listBugSubmissionsRetVals;
		}

		///<summary></summary>
		public static List<BugSubmission> GetAllInRange(DateTime dateFrom,DateTime dateTo,List<string> listVersionFilters=null,bool useConnectionStore=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listVersionFilters,useConnectionStore);
			}
			bool hasSelections=(!listVersionFilters.IsNullOrEmpty());
			List<BugSubmission> listBugSubmissions=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission WHERE " + DbHelper.BetweenDates("SubmissionDateTime",dateFrom,dateTo);
				if(hasSelections){
					command += " AND (";
					for(int i=0;i<listVersionFilters.Count;i++){
						if(i>0){
							command += " OR ";
						}
						if(listVersionFilters[i] == "Mobile"){
							command += "DbInfoJson LIKE '%\"DeviceId\":%' AND DbInfoJson NOT LIKE '%\"DeviceId\":null%'";
							continue;
						}
						command += "DbVersion LIKE '"+POut.String(listVersionFilters[i])+"%'";
					}
					command += " ) ";
				}
				listBugSubmissions=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubmissions;
		}

		///<summary>Returns list of BugSubmissions for given arrayBugIds.</summary>
		public static List<BugSubmission> GetForBugId(params long[] arrayBugIds) {
			List<BugSubmission> listBugSubmissions=new List<BugSubmission>();
			if(arrayBugIds.IsNullOrEmpty()){
				return listBugSubmissions;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),arrayBugIds);
			}
			DataAction.RunBugsHQ(() => { 
				listBugSubmissions=Crud.BugSubmissionCrud.SelectMany(
					"SELECT * FROM bugsubmission WHERE BugId IN ("+string.Join(",",arrayBugIds.Select(x => POut.Long(x)))+")"
				);
			},false);
			return listBugSubmissions;
		}

		public static List<BugSubmission> GetAllAttached() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod());
			}
			List<BugSubmission> listBugSubmissions=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission WHERE BugId!=0";
				listBugSubmissions=Crud.BugSubmissionCrud.SelectMany(command);
			},false);
			return listBugSubmissions;
		}

		///<summary>Gets one BugSubmission from the db.</summary>
		public static BugSubmission GetOne(long bugSubmissionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<BugSubmission>(MethodBase.GetCurrentMethod(),bugSubmissionNum);
			}
			BugSubmission bugSubmission=null;
			DataAction.RunBugsHQ(() => { 
				bugSubmission=Crud.BugSubmissionCrud.SelectOne(bugSubmissionNum);
			},false);
			return bugSubmission;
		}

		/// <summary>
		/// Generalizes and centralizesa code to get diagnostic information from an OD session. Used to live in FormAbout.cs. 
		/// BugSubmissions defaults patNum to -1, so we do the same here. When called during supplemental backup handshake, patNum will
		/// not be present.
		/// </summary>
		public static string GetDiagnostics(long patNum=-1) {
			BugSubmission.SubmissionInfo submissionInfo = new BugSubmission(new Exception(), patNum: patNum).Info;
			StringBuilder stringBuilder = new StringBuilder();
			Action<Action> actionWithTryCatch = (action) => {
				try {
					action();
				}
				catch(Exception ex) {
					stringBuilder.AppendLine($"ERROR: {ex.Message}");
				}
			};
			stringBuilder.AppendLine("-------------");
			stringBuilder.AppendLine($"Connection settings");
			actionWithTryCatch(() => stringBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
			actionWithTryCatch(() => stringBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
			actionWithTryCatch(() => stringBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
			//Servername, database name, msq user and password
			List<FieldInfo> listFieldInfos = submissionInfo.GetType().GetFields().ToList();
			for(int i = 0; i < listFieldInfos.Count; i++) {
				object objectValue = listFieldInfos[i].GetValue(submissionInfo);
				if(objectValue.In(null, "")) {
					continue;
				}
				if(objectValue is Dictionary<PrefName, string>) {//DictPrefValues
					Dictionary<PrefName, string> dictionaryPrefValues = objectValue as Dictionary<PrefName, string>;
					if(dictionaryPrefValues.Keys.Count > 0) {
						stringBuilder.AppendLine(listFieldInfos[i].Name + ":");
						List<KeyValuePair<PrefName, string>> listKeyValuePairsPrefVals = dictionaryPrefValues.ToList();
						for(int j=0;j<listKeyValuePairsPrefVals.Count;j++){
							stringBuilder.AppendLine(" " + listKeyValuePairsPrefVals[j].Key.ToString() + " " + listKeyValuePairsPrefVals[j].Value);
						}
						stringBuilder.AppendLine("-------------");
					}
				}
				else if(objectValue is List<string>) {//EnabledPlugins
					List<string> listEnabledPlugins = objectValue as List<string>;
					if(listEnabledPlugins.Count > 0) {
						stringBuilder.AppendLine(listFieldInfos[i].Name + ":");
						for(int j=0;j<listEnabledPlugins.Count;j++){
							stringBuilder.AppendLine(" " + listEnabledPlugins[j]);
						}
						stringBuilder.AppendLine("-------------");
					}
				}
				else if(objectValue is bool) {
					stringBuilder.AppendLine(listFieldInfos[i].Name + ": " + (((bool)objectValue) == true ? "true" : "false"));
				}
				else if(listFieldInfos[i].Name == "CountClinics") {
					int countTotalClinics = (int)objectValue;
					int countHiddenClinics = countTotalClinics - Clinics.GetCount(true);
					stringBuilder.AppendLine($"{listFieldInfos[i].Name}: {countTotalClinics} ({countHiddenClinics} hidden)");
				}
				else {
					stringBuilder.AppendLine(listFieldInfos[i].Name + ": " + objectValue);
				}
			}
			//Display the current HQ connection information.
			if(PrefC.IsODHQ) {
				Action<ConnectionNames> action = (connName) => {
					actionWithTryCatch(() => stringBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
					actionWithTryCatch(() => stringBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
					actionWithTryCatch(() => stringBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
				};
				stringBuilder.AppendLine("-------------");
				stringBuilder.AppendLine("HQ Connection Settings");
				stringBuilder.AppendLine($"{ConnectionNames.BugsHQ.ToString()}:");
				actionWithTryCatch(() => DataAction.RunBugsHQ(() => action(ConnectionNames.BugsHQ), useConnectionStore: false));
				stringBuilder.AppendLine($"{ConnectionNames.CustomersHQ.ToString()}:");
				actionWithTryCatch(() => DataAction.RunCustomers(() => action(ConnectionNames.CustomersHQ), useConnectionStore: false));
				stringBuilder.AppendLine($"{ConnectionNames.ManualPublisher.ToString()}:");
				actionWithTryCatch(() => DataAction.RunManualPublisherHQ(() => action(ConnectionNames.ManualPublisher)));
				stringBuilder.AppendLine($"{ConnectionNames.WebChat.ToString()}:");
				actionWithTryCatch(() => DataAction.RunWebChat(() => action(ConnectionNames.WebChat)));
				stringBuilder.AppendLine("-------------");
			}
			return stringBuilder.ToString();
		}
		
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(BugSubmission bugSubmission) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),bugSubmission);
			}
			long retVal=0;
			//Always use the connection store config file because creating BugSubmissions should always happen via OpenDentalWebServiceHQ.
			DataAction.RunBugsHQ(() => { 
				retVal=Crud.BugSubmissionCrud.Insert(bugSubmission);
			});
			return retVal;
		}
		
		///<summary>Attempts to insert the given BugSubmission.</summary>
		public static BugSubmissionResult TryInsertBugSubmission(BugSubmission bugSubmission,out string matchedFixedVersion,Func<BugSubmission,bool> funcFilterValidation,bool doFilterValidation=true) {
			Meth.NoCheckMiddleTierRole();//Out parameter.
			BugSubmissionResult bugSubmissionResult=BugSubmissionResult.None;
			matchedFixedVersion=null;
			if(doFilterValidation && !funcFilterValidation(bugSubmission)) {
				bugSubmissionResult=BugSubmissionResult.UpdateRequired;
				return bugSubmissionResult;
			}
			bugSubmissionResult=BugSubmissionHashes.ProcessSubmission(bugSubmission,out long matchedBugId,out matchedFixedVersion,out long matchedBugSubmissionHashNum);
			switch(bugSubmissionResult){
				default:
				case BugSubmissionResult.SuccessMatched://Hash found and points to valid bugId, but associated bug not flagged as fixed.
				case BugSubmissionResult.SuccessMatchedFixed://Same as above but bug is flagged as fixed, matchedVersionsFixed is set also.
					bugSubmission.BugId=matchedBugId;
					bugSubmission.BugSubmissionHashNum=matchedBugSubmissionHashNum;
					break;
				case BugSubmissionResult.SuccessHashFound://Seen this submission before but no attached bug found. BugId=0.
					bugSubmission.BugSubmissionHashNum=matchedBugSubmissionHashNum;
					break;
				case BugSubmissionResult.SuccessHashNeeded://New submission we do not have a hash for. BugId=0.
					long bugSubmissionHashNum=BugSubmissionHashes.Insert(new BugSubmissionHash() {
						FullHash=bugSubmission.HashedStackTrace,
						PartialHash=bugSubmission.HashedSimpleStackTrace
					});
					bugSubmission.BugSubmissionHashNum=bugSubmissionHashNum;
					break;
				case BugSubmissionResult.Failed:
					break;
			}
			Insert(bugSubmission);
			return bugSubmissionResult;
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(BugSubmission bugSubmissionNew, BugSubmission bugSubmissionOld,bool useConnectionStore=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugSubmissionNew,bugSubmissionOld,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Crud.BugSubmissionCrud.Update(bugSubmissionNew,bugSubmissionOld);
			},useConnectionStore);
		}
		
		///<summary>Updates all bugIds for given bugSubmissionNums.</summary>
		public static void UpdateBugIds(long bugId,List<BugSubmission> listBugSubmissions) {
			if(listBugSubmissions.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugId,listBugSubmissions);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET BugId="+POut.Long(bugId)
					+" WHERE BugSubmissionNum IN ("+string.Join(",",listBugSubmissions.Select(x => POut.Long(x.BugSubmissionNum)))+")");
			},false);
			BugSubmissionHashes.UpdateBugIds(listBugSubmissions,bugId);
		}

		///<summary>Updates various columns based on in memory changes in listSubs.</summary>
		public static void UpdateMany(List<BugSubmission> listBugSubmissions,params string[] strArrayColumns) {
			if(listBugSubmissions.Count==0 || strArrayColumns.Count()==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listBugSubmissions,strArrayColumns);
				return;
			}
			List<string> listColumnUpdates=new List<string>();
			for(int i=0;i<strArrayColumns.Length;i++){
				List<string> listCases=new List<string>();
				switch(strArrayColumns[i]) {
					#region IsHidden
					case "IsHidden":
						for(int j=0;j<listBugSubmissions.Count;j++){
							listCases.Add("WHEN "+POut.Long(listBugSubmissions[j].BugSubmissionNum)+" THEN "+POut.Bool(listBugSubmissions[j].IsHidden));
						}
						break;
					#endregion
					#region BugId
					case "BugId":
						for(int j=0;j<listBugSubmissions.Count;j++){
							listCases.Add("WHEN "+POut.Long(listBugSubmissions[j].BugSubmissionNum)+" THEN "+POut.Long(listBugSubmissions[j].BugId));
						}
						break;
					#endregion
				}
				listColumnUpdates.Add(strArrayColumns[i]+"=(CASE BugSubmissionNum "+string.Join(" ",listCases)+" END)");
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET "
					+string.Join(",",listColumnUpdates)+" "
					+"WHERE BugSubmissionNum IN ("+string.Join(",",listBugSubmissions.Select(x => x.BugSubmissionNum))+")");
			},false);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long bugSubmissionId) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugSubmissionId);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Crud.BugSubmissionCrud.Delete(bugSubmissionId);
			},false);
		}
		#endregion

		#region Misc Methods
		///<summary>Attempts to submit an exception to HQ.
		///Checks PrefName.SendUnhandledExceptionsToHQ prior to web call.</summary>
		public static BugSubmissionResult SubmitException(Exception ex,string threadName="",long patNumCur=-1,string moduleName="") {
			Meth.NoCheckMiddleTierRole();
			return SubmitException(ex,out string displayMsg,threadName,patNumCur,moduleName);
		}
		
		///<summary>Attempts to submit an exception to HQ.
		///Checks PrefName.SendUnhandledExceptionsToHQ prior to web call.</summary>
		public static BugSubmissionResult SubmitException(Exception ex,out string displayMsg,string threadName="",long patNumCur=-1,string moduleName="") {
			Meth.NoCheckMiddleTierRole();
			displayMsg=null;
			if(MockBugSubmissions!=null) {
				return MockBugSubmissions.SubmitException(ex,threadName,patNumCur,moduleName);
			}
			//Default SendUnhandledExceptionsToHQ to true if the preference cache is null or the preference was not found.
			//There might not be a database connection yet, therefore the preference cache could be null.
			//HQ needs to know more information regarding unhandled exceptions prior to setting a database connection (.NET issue, release issue, etc).
			if(!PrefC.GetBoolSilent(PrefName.SendUnhandledExceptionsToHQ,true)) {
				return BugSubmissionResult.None;
			}
			BugSubmission bugSubmission=new BugSubmission(ex,threadName,patNumCur,moduleName);
			string registrationKey=null;
			string practiceTitle=null;
			string practicePhone=null;
			string programVersion=null;
			string webServiceHqURL="";
			if(bugSubmission.RegKey=="7E57-1NPR-0DUC-710N") {
				registrationKey=bugSubmission.RegKey;
				practiceTitle="Unknown";
				practicePhone="Unknown";
				programVersion=bugSubmission.Info.OpenDentBusinessVersion;
				webServiceHqURL="https://www.patientviewer.com:49997/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx";
			}
			return ParseBugSubmissionResult(
				WebServiceMainHQProxy.GetWebServiceMainHQInstance(webServiceHqURL).SubmitUnhandledException(
					PayloadHelper.CreatePayload(
						PayloadHelper.CreatePayloadContent(bugSubmission,"bugSubmission"),eServiceCode.BugSubmission,registrationKey,practiceTitle,practicePhone,programVersion
					)
				)
				,out displayMsg
			);
		}

		///<summary>After calling WebServiceMainHQ.SubmitUnhandledException(...) this will digest the result and provide the result.
		///displayMsg can be set when there is a message to show to the user after a successful bug submission match has been found.</summary>
		public static BugSubmissionResult ParseBugSubmissionResult(string result,out string displayMsg) {
			Meth.NoCheckMiddleTierRole();//Out parameter
			displayMsg=null;
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			if(xmlDocument.SelectSingleNode("//Error")!=null) {
				return BugSubmissionResult.Failed;
			}
			//A BugSubmission.Response object will get returned.
			XmlNode xmlNodeResponse=xmlDocument.SelectSingleNode("//SubmissionResult");
			if(xmlNodeResponse!=null) {
				BugSubmissionResult bugSubmissionResult;
				if(Enum.TryParse(xmlNodeResponse.InnerText,out bugSubmissionResult)) {
					displayMsg=xmlDocument.SelectSingleNode("//DisplayString")?.InnerText;
					return bugSubmissionResult;
				}
			}
			return BugSubmissionResult.None;//Just in case;
		}

		/// <summary>
		/// Returns true if currentVersion has a pertinent update version in listFixedVersions.
		/// When true pertinentFixedVersion is set to the specific version that can be used.
		/// </summary>
		public static bool TryMatchPertinentFixedVersion(Version currentVersion,List<Version> listVersionsFixed,out Version versionPertinentFixed) {
			Meth.NoCheckMiddleTierRole();
			versionPertinentFixed=null;
			//Make sure that the current bug submission is at a version that is lower than all fixed versions.  Rare but it has happened.
			if(listVersionsFixed.All(x => x<currentVersion)) {
				//Not a single fixed version is higher than the bug submission that just came in.  Guaranteed to be a new bug.
			}
			//Only versions of the same Major and Minor matter.
			else if(listVersionsFixed.Any(x => x.Major==currentVersion.Major && x.Minor==currentVersion.Minor && x.Build<=currentVersion.Build)) {
				//There are fixed versions, they may or may not be pertinent to the bug submission version.
				//Only care about fixed versions that share the same Major version.  
				//E.g. a fixed version with a higher Major version doesn't necessarily mean this bug is fixed.  
				//subProgVersion = 17.4.32.0  and  listFixedVersions = { 16.8.23.0, 17.4.44.0, 18.1.1.0 }
				//As you can see, the presence of a "fix" being in 18.1.1.0 does not matter to the current bug submission.
				//The inverse can be applied to versions that are less than the current bug submission version.
				//Meaning, just because 16.8.23.0 has been flagged as "fixed", doesn't mean that all major versions above it are fixed as well.
			}
			else {
				//Fix is on more recent major version than what user is on.
				versionPertinentFixed=listVersionsFixed.Find(x => x>currentVersion);
			}
			return (versionPertinentFixed!=null);
		}
		
		public static string GetSubmissionDescription(Patient patient,BugSubmission bugSubmission) {
			Meth.NoCheckMiddleTierRole();
			string retVal="";
			ODException.SwallowAnyException(() => { 
				retVal="Caller Name and #: "+patient.GetNameLF() +" (work) "+patient.WkPhone+"\r\n"
					+"Quick desc: "+bugSubmission.ExceptionMessageText+"\r\n"
					+"OD version: "+bugSubmission.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0")+"\r\n"
					+"Windows version: "+bugSubmission.Info.WindowsVersion+"\r\n"
					+"Comps affected: "+bugSubmission.Info.CompName+"\r\n"
					+"Database name: "+bugSubmission.Info.DatabaseName+"\r\n"
					+"Example PatNum: " +bugSubmission.Info.PatientNumCur+"\r\n"
					+"Details: "+"\r\n"
					+"Duplicable?: "+"\r\n"
					+"Steps to duplicate: "+"\r\n"
					+"Exception:  "+bugSubmission.ExceptionStackTrace;
			});
			return retVal;
		}

		///<summary></summary>
		public static List<string> GetFilterPhrases(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> listPhrases=null;
			//Query the database for all phrases that we don't accept and see if the exception text passed in contains any of the phrases from the database.
			DataAction.RunBugsHQ(() => {
				listPhrases=Db.GetListString("SELECT Phrase FROM bugsubmissionfilter WHERE Phrase!=''");
			});
			return listPhrases;
		}

		///<summary>Removes file names and line numbers to reveal a simplified stack path.</summary>
		public static string SimplifyStackTrace(string stackTrace) {
			return Regex.Replace(stackTrace.ToLower(),@" in [a-z0-9.\\: ]+\n","\n");//Case insensitive.
		}
		#endregion
	}

	public enum BugSubmissionResult {
		///<summary></summary>
		None,
		///<summary>Submitter is not on support or there was an exception in the web method</summary>
		Failed,
		///<summary>Submitter must be on the most recent stable or any beta version.</summary>
		UpdateRequired,
		///<summary>Submitter sucessfully inserted a bugSubmission at HQ</summary>
		SuccessHashFound,
		///<summary>Submitter sucessfully inserted a bugSubmission at HQ and a hash row was also inserted.</summary>
		SuccessHashNeeded,
		///<summary>Submitter sucessfully inserted a bugSubmission at HQ and it was matched to a bug that is NOT currently flagged as fixed.</summary>
		SuccessMatched,
		///<summary>Submitter sucessfully inserted a bugSubmission at HQ and it was matched to a bug that is currently flagged as fixed.</summary>
		SuccessMatchedFixed,
	}

	///<summary>Used to test bug submissions.</summary>
	public interface IBugSubmissions {
		BugSubmissionResult SubmitException(Exception ex,string threadName="",long patNumCur=-1,string moduleName="");
	}
}