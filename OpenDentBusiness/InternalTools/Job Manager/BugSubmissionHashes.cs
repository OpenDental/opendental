using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class BugSubmissionHashes{
		#region Get Methods
		///<summary></summary>
		public static List<BugSubmissionHash> GetForHashes(string fullHash,string partialHash,bool useConnectionStore=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmissionHash>>(MethodBase.GetCurrentMethod(),fullHash,partialHash,useConnectionStore);
			}
			List<BugSubmissionHash> listBugHashes=new List<BugSubmissionHash>();
			DataAction.RunBugsHQ(() => {
				string command=$@"SELECT * FROM bugsubmissionhash 
					WHERE FullHash = {POut.String(fullHash)}
					AND  PartialHash = {POut.String(partialHash)}";
				listBugHashes=Crud.BugSubmissionHashCrud.SelectMany(command);
			},useConnectionStore);
			return listBugHashes;
		}
		
		///<summary>Gets one BugSubmissionHash from the db.</summary>
		public static BugSubmissionHash GetOne(long bugSubmissionHashNum,bool useConnectionStore=true){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<BugSubmissionHash>(MethodBase.GetCurrentMethod(),bugSubmissionHashNum,useConnectionStore);
			}
			BugSubmissionHash bugHash=null;
			DataAction.RunBugsHQ(() => {
				 bugHash=Crud.BugSubmissionHashCrud.SelectOne(bugSubmissionHashNum);
			},useConnectionStore);
			return bugHash;
		}

		///<summary>Returns a list of BugSubmissionHashes based on DateTimeEntry and given date range.</summary>
		public static List<BugSubmissionHash> GetMany(DateTime dateTimeFrom=default,DateTime dateTimeTo=default,bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmissionHash>>(MethodBase.GetCurrentMethod(),dateTimeFrom,dateTimeTo,useConnectionStore);
			}
			List<BugSubmissionHash> listRetVals=new List<BugSubmissionHash>();
			DataAction.RunBugsHQ(() => {
				string command=$@"SELECT * FROM bugsubmissionhash ";
				if(dateTimeFrom!=DateTime.MinValue && dateTimeTo!=DateTime.MinValue) {
					command+=$@"WHERE "+DbHelper.BetweenDates("DateTimeEntry",dateTimeFrom,dateTimeTo);
				}
				command+="ORDER BY DateTimeEntry DESC";
				listRetVals=Crud.BugSubmissionHashCrud.SelectMany(command);
			},useConnectionStore);
			return listRetVals;
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(BugSubmissionHash bugSubmissionHash,bool useConnectionStore=true){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				bugSubmissionHash.BugSubmissionHashNum=Meth.GetLong(MethodBase.GetCurrentMethod(),bugSubmissionHash,useConnectionStore);
				return bugSubmissionHash.BugSubmissionHashNum;
			}
			long val=-1;
			DataAction.RunBugsHQ(() => {
				 val=Crud.BugSubmissionHashCrud.Insert(bugSubmissionHash);
			},useConnectionStore);
			return val;
		}

		///<summary></summary>
		public static void InsertMany(List<BugSubmissionHash> listBugSubmissionHash,bool useConnectionStore=true){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listBugSubmissionHash,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() =>{
				Crud.BugSubmissionHashCrud.InsertMany(listBugSubmissionHash);
			},useConnectionStore);
		}
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(BugSubmissionHash bugSubmissionHash,bool useConnectionStore){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugSubmissionHash,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() =>{
				Crud.BugSubmissionHashCrud.Update(bugSubmissionHash);
			},useConnectionStore);
		}
		#endregion Update
		#region Delete
		///<summary></summary>
		public static void Delete(long bugSubmissionHashNum,bool useConnectionStore) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugSubmissionHashNum,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() =>{
				Crud.BugSubmissionHashCrud.Delete(bugSubmissionHashNum);
			},useConnectionStore);
		}
		#endregion Delete
		#endregion Modification Methods

		#region Misc Methods

		/// <summary>
		/// Returns the BugSubmissionResult result for a given bug submission hash check.
		/// </summary>
		/// <param name="sub"></param>
		/// <param name="matchedBugId"></param>
		/// <param name="matchedFixedVersions"></param>
		/// <param name="matchedBugSubmissionHashNum"></param>
		/// <param name="submissionVersionOverride"></param>
		/// <returns></returns>
		public static BugSubmissionResult ProcessSubmission(BugSubmission sub,out long matchedBugId,out string matchedFixedVersions
			,out long matchedBugSubmissionHashNum,Version submissionVersionOverride=null,bool useConnectionStore=true)
		{
			matchedBugId=-1;
			matchedFixedVersions=null;
			matchedBugSubmissionHashNum=-1;
			if(sub.ListMatchedBugInfos==null){//Not already defined (like in unit test) so query for it.
				DataAction.RunBugsHQ(() => {
					DataTable table=Db.GetTable($@"
						SELECT bugSubmissionHashNum, COALESCE(bug.BugId,0) _bugId,COALESCE(bug.VersionsFixed,'') _versionsFixed
						FROM bugsubmissionhash 
						LEFT JOIN bug on bug.BugId=bugsubmissionhash.BugId
						WHERE FullHash='{sub.HashedStackTrace}'
						AND PartialHash='{sub.HashedSimpleStackTrace}'"
					);
					sub.ListMatchedBugInfos=table.Select().Select(x =>  
						new BugSubmission.MatchedBugInfo(x.GetLong("bugSubmissionHashNum"),x.GetLong("_bugId"),x.GetString("_versionsFixed")
					)).ToList();
				},useConnectionStore);
			}
			if(sub.ListPendingFixBugInfos.Count>1) {//There are too many pending bugs and a developer will have to decide which to associate with.
				return BugSubmissionResult.Failed;
			}
			else if(sub.ListPendingFixBugInfos.Count==1) {
				matchedBugId=sub.ListPendingFixBugInfos.First().BugId;
				matchedBugSubmissionHashNum=sub.ListPendingFixBugInfos.First().BugSubmissionHashNum;
				return BugSubmissionResult.SuccessMatched;
			}
			//At this point, there are no pending fixes.  Decide if a new bug submission hash should be inserted or find a pertinent fix.
			BugSubmission.MatchedBugInfo bugInfo=sub.GetPertinentFixedVersion(submissionVersionOverride);
			if(bugInfo==null) {//No previously fixed bugs apply to this submission version.
				if(sub.ListMatchedBugInfos.Any(x => x.BugSubmissionHashNum>0 && x.BugId==0 && string.IsNullOrEmpty(x.VersionsFixed))) {
					//We have seen this submission/hash before but it has not been associated to an internal bug yet.
					matchedBugSubmissionHashNum=sub.ListMatchedBugInfos.First(x => x.BugSubmissionHashNum>0 && x.BugId==0 && string.IsNullOrEmpty(x.VersionsFixed)).BugSubmissionHashNum;
					return BugSubmissionResult.SuccessHashFound;
				}
				//Insert new hash for submission that indicates that a previous fix did not work as intended or we have not seen this submission yet.
				return BugSubmissionResult.SuccessHashNeeded;
			}
			if(!sub.ListMatchedBugInfos.Any(x => x.BugSubmissionHashNum>0 && x.BugId==0 && string.IsNullOrEmpty(x.VersionsFixed))) {
				//Even though we found a pertinent fix for the inbound submission there appears to be another hash that was created due to the bug resurfacing again.
				matchedFixedVersions=bugInfo.VersionsFixed;
			}
			matchedBugId=bugInfo.BugId;
			matchedBugSubmissionHashNum=bugInfo.BugSubmissionHashNum;
			return BugSubmissionResult.SuccessMatchedFixed;
		}

		/// <summary>
		/// Updates the BugSubmissionHash.BugId to the given bugId for all hash rows associated to the given listSubs.
		/// </summary>
		public static void UpdateBugIds(List<BugSubmission> listSubs,long bugId,bool useConnectionStore=false) {
			if(listSubs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSubs,bugId,useConnectionStore);
				return;
			}
			DataAction.RunBugsHQ(() => {
				Db.NonQ($@"
					UPDATE bugsubmissionhash 
					SET BugId={POut.Long(bugId)}
					WHERE BugSubmissionHashNum IN({string.Join(",",listSubs.Select(x => POut.Long(x.BugSubmissionHashNum)))})"
				);
			},useConnectionStore);
		}
		#endregion Misc Methods

	}
}
