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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),useConnectionStore);
			}
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission";
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubs;
		}

		///<summary>Returns a list of BugSubmissions for the given arrayHashNums.</summary>
		public static List<BugSubmission> GetForHashNums(bool useConnectionStore,params long[] arrayHashNums){
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			if(arrayHashNums.IsNullOrEmpty()){
				return listBugSubs;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),useConnectionStore,arrayHashNums);
			}
			DataAction.RunBugsHQ(() => { 
				string command=$@"SELECT * FROM bugsubmission WHERE BugSubmissionHashNum IN ({string.Join(",",arrayHashNums.Select(x => POut.Long(x)))})";
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubs;
			
		}

		///<summary>Returns BugSubmissions for the hash nums provided.  Key: BugSubmissionHashNum  Value: bugsubmissions.</summary>
		public static SerializableDictionary<long,List<BugSubmission>> GetForHashNums(List<long> listHashNums,bool useConnectionStore=false) {
			SerializableDictionary<long,List<BugSubmission>> dictHashBugSubs=new SerializableDictionary<long,List<BugSubmission>>();
			if(listHashNums.IsNullOrEmpty()) {
				return dictHashBugSubs;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,List<BugSubmission>>>(MethodBase.GetCurrentMethod(),listHashNums,useConnectionStore);
			}
			DataAction.RunBugsHQ(() => { 
				List<BugSubmission> listBugSubs=GetForHashNums(useConnectionStore,listHashNums.ToArray());
				listHashNums.ForEach(x => 
					dictHashBugSubs.Add(x,listBugSubs.FindAll(y => y.BugSubmissionHashNum==x))
				);
			},useConnectionStore);
			return dictHashBugSubs;
		}

		///<summary>Returns a list of bug submissions and their corresponding bugs.
		///If a bugsubmission is not associated to a bug then the BugObj field on the bugsubmission object will be null.
		///Performs grouping logic in order to minimize the amount of bugsubmissions in the return results.</summary>
		public static List<BugSubmission> GetBugSubsForRegKeys(List<string> listRegKeys,DateTime dateFrom,DateTime dateTo) {
			if(listRegKeys==null || listRegKeys.Count==0) {
				return new List<BugSubmission>();//No point in going through middle tier.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),listRegKeys,dateFrom,dateTo);
			}
			List<BugSubmission> listRetVals=new List<BugSubmission>();
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
				foreach(DataRow row in table.Select().Where(x => PIn.Long(x["BugId"].ToString(),false)!=0)) {
					tableBugs.ImportRow(row);
				}
				//Extract all of the bug objects from the subset table.
				List<Bug> listBugs=Crud.BugCrud.TableToList(tableBugs);
				//Extract all of the bugsubmission objects from the results.
				List<BugSubmission> listBugSubs = Crud.BugSubmissionCrud.TableToList(table);
				//Associate any bug object with its corresponding bugsubmission object.
				listBugSubs.ForEach(x => x.BugObj=listBugs.FirstOrDefault(y => y.BugId==x.BugId));
				//Group the bug submissions by RegKey, ExceptionStackTrace, and BugId.
				listBugSubs.GroupBy(x => new { x.RegKey,x.ExceptionStackTrace,x.BugId })
					//Each grouping will be ordered by ProgramVersion individually.
					.ToDictionary(x => x.Key,x => x.OrderByDescending(x => new Version(x.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"))))
					//Add the first and most pertinent bug submission in the grouping to the return value.
					.ForEach(x => listRetVals.Add(x.Value.First()));
			},false);
			return listRetVals;
		}

		///<summary></summary>
		public static List<BugSubmission> GetAllInRange(DateTime dateFrom,DateTime dateTo,List<string> listVersionFilters=null,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listVersionFilters,useConnectionStore);
			}
			bool hasSelections=(!listVersionFilters.IsNullOrEmpty());
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command=$@"SELECT * FROM bugsubmission WHERE {DbHelper.BetweenDates("SubmissionDateTime",dateFrom,dateTo)} 
					{(hasSelections 
						? $@" AND ({string.Join(" OR ",listVersionFilters.Select(x => 
								(x=="Mobile"
									? "DbInfoJson like '%\"DeviceId\":%' and DbInfoJson NOT LIKE '%\"DeviceId\":null%'" 
									: "DbVersion LIKE '"+POut.String(x)+"%'"
								)))
							})" 
						: ""
					)}
				";
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},useConnectionStore);
			return listBugSubs;
		}

		///<summary>Returns list of BugSubmissions for given arrayBugIds.</summary>
		public static List<BugSubmission> GetForBugId(params long[] arrayBugIds) {
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			if(arrayBugIds.IsNullOrEmpty()){
				return listBugSubs;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),arrayBugIds);
			}
			DataAction.RunBugsHQ(() => { 
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(
					"SELECT * FROM bugsubmission WHERE BugId IN ("+string.Join(",",arrayBugIds.Select(x => POut.Long(x)))+")"
				);
			},false);
			return listBugSubs;
		}

		public static List<BugSubmission> GetAllAttached() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod());
			}
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission WHERE BugId!=0";
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},false);
			return listBugSubs;
		}

		///<summary>Gets one BugSubmission from the db.</summary>
		public static BugSubmission GetOne(long bugSubmissionId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<BugSubmission>(MethodBase.GetCurrentMethod(),bugSubmissionId);
			}
			BugSubmission bugSub=null;
			DataAction.RunBugsHQ(() => { 
				bugSub=Crud.BugSubmissionCrud.SelectOne(bugSubmissionId);
			},false);
			return bugSub;
		}
		
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(BugSubmission bugSubmission) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
		public static BugSubmissionResult TryInsertBugSubmission(BugSubmission sub,out string matchedFixedVersion,Func<BugSubmission,bool> funcFilterValidation,bool doFilterValidation=true) {
			//No need to check RemotingRole; out parameter.
			BugSubmissionResult result=BugSubmissionResult.None;
			matchedFixedVersion=null;
			if(!doFilterValidation || funcFilterValidation(sub)) {
				result=BugSubmissionHashes.ProcessSubmission(sub,out long matchedBugId,out matchedFixedVersion,out long matchedBugSubmissionHashNum);
				switch(result){
					default:
					case BugSubmissionResult.SuccessMatched://Hash found and points to valid bugId, but associated bug not flagged as fixed.
					case BugSubmissionResult.SuccessMatchedFixed://Same as above but bug is flagged as fixed, matchedVersionsFixed is set also.
						sub.BugId=matchedBugId;
						sub.BugSubmissionHashNum=matchedBugSubmissionHashNum;
						break;
					case BugSubmissionResult.SuccessHashFound://Seen this submission before but no attached bug found. BugId=0.
						sub.BugSubmissionHashNum=matchedBugSubmissionHashNum;
						break;
					case BugSubmissionResult.SuccessHashNeeded://New submission we do not have a hash for. BugId=0.
						long bugSubmissionHashNum=BugSubmissionHashes.Insert(new BugSubmissionHash() {
							FullHash=sub.HashedStackTrace,
							PartialHash=sub.HashedSimpleStackTrace
						});
						sub.BugSubmissionHashNum=bugSubmissionHashNum;
						break;
					case BugSubmissionResult.Failed:
						break;
				}
				Insert(sub);
			}
			else{
				result=BugSubmissionResult.UpdateRequired;
			}
			return result;
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(BugSubmission subNew, BugSubmission subOld,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),subNew,subOld,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Crud.BugSubmissionCrud.Update(subNew,subOld);
			},useConnectionStore);
		}
		
		///<summary>Updates all bugIds for given bugSubmissionNums.</summary>
		public static void UpdateBugIds(long bugId,List<BugSubmission> listBugSubs) {
			if(listBugSubs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugId,listBugSubs);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET BugId="+POut.Long(bugId)
					+" WHERE BugSubmissionNum IN ("+string.Join(",",listBugSubs.Select(x => POut.Long(x.BugSubmissionNum)))+")");
			},false);
			BugSubmissionHashes.UpdateBugIds(listBugSubs,bugId);
		}

		///<summary>Updates various columns based on in memory changes in listSubs.</summary>
		public static void UpdateMany(List<BugSubmission> listSubs,params string[] listColumns) {
			if(listSubs.Count==0 || listColumns.Count()==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSubs,listColumns);
				return;
			}
			List<string> listColumnUpdates=new List<string>();
			foreach(string column in listColumns) {
				List<string> listCases=new List<string>();
				switch(column) {
					#region IsHidden
					case "IsHidden":
						foreach(BugSubmission sub in listSubs) {
							listCases.Add("WHEN "+POut.Long(sub.BugSubmissionNum)+" THEN "+POut.Bool(sub.IsHidden));
						}
						break;
					#endregion
					#region BugId
					case "BugId":
						foreach(BugSubmission sub in listSubs) {
							listCases.Add("WHEN "+POut.Long(sub.BugSubmissionNum)+" THEN "+POut.Long(sub.BugId));
						}
						break;
					#endregion
				}
				listColumnUpdates.Add(column+"=(CASE BugSubmissionNum "+string.Join(" ",listCases)+" END)");
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET "
					+string.Join(",",listColumnUpdates)+" "
					+"WHERE BugSubmissionNum IN ("+string.Join(",",listSubs.Select(x => x.BugSubmissionNum))+")");
			},false);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long bugSubmissionId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			//No remoting role check; no call to db
			return SubmitException(ex,out string displayMsg,threadName,patNumCur,moduleName);
		}
		
		///<summary>Attempts to submit an exception to HQ.
		///Checks PrefName.SendUnhandledExceptionsToHQ prior to web call.</summary>
		public static BugSubmissionResult SubmitException(Exception ex,out string displayMsg,string threadName="",long patNumCur=-1,string moduleName="") {
			//No remoting role check; no call to db
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
			//No remoting role check; out parameter
			displayMsg=null;
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			if(doc.SelectSingleNode("//Error")!=null) {
				return BugSubmissionResult.Failed;
			}
			//A BugSubmission.Response object will get returned.
			XmlNode responseNode=doc.SelectSingleNode("//SubmissionResult");
			if(responseNode!=null) {
				BugSubmissionResult resultSub;
				if(Enum.TryParse(responseNode.InnerText,out resultSub)) {
					displayMsg=doc.SelectSingleNode("//DisplayString")?.InnerText;
					return resultSub;
				}
			}
			return BugSubmissionResult.None;//Just in case;
		}

		/// <summary>
		/// Returns true if currentVersion has a pertinent update version in listFixedVersions.
		/// When true pertinentFixedVersion is set to the specific version that can be used.
		/// </summary>
		public static bool TryMatchPertinentFixedVersion(Version currentVersion,List<Version> listFixedVersions,out Version pertinentFixedVersion) {
			pertinentFixedVersion=null;
			//Make sure that the current bug submission is at a version that is lower than all fixed versions.  Rare but it has happened.
			if(listFixedVersions.All(x => x<currentVersion)) {
				//Not a single fixed version is higher than the bug submission that just came in.  Guaranteed to be a new bug.
			}
			//Only versions of the same Major and Minor matter.
			else if(listFixedVersions.Any(x => x.Major==currentVersion.Major && x.Minor==currentVersion.Minor && x.Build<=currentVersion.Build)) {
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
				pertinentFixedVersion=listFixedVersions.FirstOrDefault(x => x>currentVersion);
			}
			return (pertinentFixedVersion!=null);
		}
		
		public static string GetSubmissionDescription(Patient patCur,BugSubmission sub) {
			return "Caller Name and #: "+patCur.GetNameLF() +" (work) "+patCur.WkPhone+"\r\n"
				+"Quick desc: "+sub.ExceptionMessageText+"\r\n"
				+"OD version: "+sub.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0")+"\r\n"
				+"Windows version: "+sub.Info.WindowsVersion+"\r\n"
				+"Comps affected: "+sub.Info.CompName+"\r\n"
				+"Database name: "+sub.Info.DatabaseName+"\r\n"
				+"Example PatNum: " +sub.Info.PatientNumCur+"\r\n"
				+"Details: "+"\r\n"
				+"Duplicable?: "+"\r\n"
				+"Steps to duplicate: "+"\r\n"
				+"Exception:  "+sub.ExceptionStackTrace;
		}

		///<summary></summary>
		public static List<string> GetFilterPhrases(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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