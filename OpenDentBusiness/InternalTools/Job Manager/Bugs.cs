using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Bugs{
		private const int THREAD_TIMEOUT=5000;

		///<summary>Returns a list of bugs for given bugIds.</summary>
		public static List<Bug> GetMany(List<long> listBugIds) {
			if(listBugIds==null || listBugIds.Count==0) {
				return new List<Bug>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Bug>>(MethodBase.GetCurrentMethod(),listBugIds);
			}
			List<Bug> listBugs=new List<Bug>();
			DataAction.RunBugsHQ(() => {
				listBugs=Crud.BugCrud.TableToList(DataCore.GetTable("SELECT * FROM bug WHERE BugID IN ("+string.Join(",",listBugIds)+")"));
			},false);
			return listBugs;
		}

		///<summary>Must pass in version as "Maj" or "Maj.Min" or "Maj.Min.Rev". Uses like operator.</summary>
		public static List<Bug> GetByVersion(string versionMajMin,string filter="") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Bug>>(MethodBase.GetCurrentMethod(),versionMajMin,filter);
			}
			List<Bug> listBugs=new List<Bug>();
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM bug "
				+"WHERE (VersionsFound LIKE '"+POut.String(versionMajMin)+"%' "
					+"OR VersionsFound LIKE '%;"+POut.String(versionMajMin)+"%' "
					+"OR VersionsFixed LIKE '"+POut.String(versionMajMin)+"%' "
					+"OR VersionsFixed LIKE '%;"+POut.String(versionMajMin)+"%') ";
				if(filter!="") {
					command+="AND Description LIKE '%"+POut.String(filter)+"%'";
				}
				listBugs=Crud.BugCrud.SelectMany(command);
			},false);
			return listBugs;
		}

		///<summary>Gets all bugs ordered by CreationDate DESC</summary>
		public static List<Bug> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Bug>>(MethodBase.GetCurrentMethod());
			}
			List<Bug> listBugs=new List<Bug>();
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM bug ORDER BY CreationDate DESC";
				listBugs=Crud.BugCrud.SelectMany(command);
			},false);
			return listBugs;
		}

		///<summary>Gets one Bug from the db.</summary>
		public static Bug GetOne(long bugId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Bug>(MethodBase.GetCurrentMethod(),bugId);
			}
			Bug bug=null;
			DataAction.RunBugsHQ(() => bug=Crud.BugCrud.SelectOne(bugId),false);
			return bug;
		}

		///<summary></summary>
		public static long Insert(Bug bug) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				bug.BugId=Meth.GetLong(MethodBase.GetCurrentMethod(),bug);
				return bug.BugId;
			}
			DataAction.RunBugsHQ(() => Crud.BugCrud.Insert(bug),false);
			return bug.BugId;
		}

		///<summary></summary>
		public static void Update(Bug bug) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bug);
				return;
			}
			DataAction.RunBugsHQ(() => Crud.BugCrud.Update(bug),false);
		}

		///<summary></summary>
		public static void Delete(long bugId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugId);
				return;
			}
			JobLinks.DeleteForType(JobLinkType.Bug,bugId);
			DataAction.RunBugsHQ(() => Crud.BugCrud.Delete(bugId),false);
		}

		///<Summary>Gets name from database for the submitter passed in.  Not very efficient.</Summary>
		public static string GetSubmitterName(long bugUserId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),bugUserId);
			}
			string submitterName="";
			DataAction.RunBugsHQ(() => {
				string command="SELECT UserName FROM buguser WHERE BugUserId="+POut.Long(bugUserId);
				submitterName=Db.GetScalar(command);
			},false);
			return submitterName;
		}

		///<Summary>Returns a dictionary where key: BugUserId, value: UserName from database for the submitters passed in.  Not very efficient.</Summary>
		public static SerializableDictionary<long,string> GetDictSubmitterNames(List<long> listBugUserIds) {
			if(listBugUserIds==null || listBugUserIds.Count==0) {
				return new SerializableDictionary<long, string>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,string>>(MethodBase.GetCurrentMethod(),listBugUserIds);
			}
			SerializableDictionary<long,string> dictSubmitterNames=new SerializableDictionary<long, string>();
			DataAction.RunBugsHQ(() => {
				dictSubmitterNames=Db.GetTable("SELECT BugUserId,UserName FROM buguser WHERE BugUserId IN ("+string.Join(",",listBugUserIds)+")").Select()
					.ToSerializableDictionary(x => PIn.Long(x["BugUserId"].ToString()),x => PIn.String(x["UserName"].ToString()));
			},false);
			return dictSubmitterNames;
		}

		///<Summary>Checks bugIDs in list for incompletes. Returns false if incomplete exists.</Summary>
		public static bool CheckForCompletion(List<long> listBugIDs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listBugIDs);
			}
			int count=0;
			DataAction.RunBugsHQ(() => {
				string command="SELECT COUNT(*) FROM bug "
					+"WHERE VersionsFixed='' "
					+"AND BugId IN ("+String.Join(",",listBugIDs)+")";
				count=PIn.Int(Db.GetCount(command));
			},false);
			return (count==0);
		}

		public static Bug GetNewBugForUser() {
			Bug bug=new Bug();
			bug.CreationDate=DateTime.Now;
			bug.Status_=BugStatus.None;
			bug.Submitter=GetBugSubmitter();
			bug.Type_=BugType.Bug;
			bug.VersionsFound=VersionReleases.GetLastReleases(2);
			return bug;
		}

		///<summary>Returns the BugUser int from the machine name.</summary>
		public static int GetBugSubmitter() {
			int bugUser;
			switch(System.Environment.MachineName) {
				case "ANDREW":
				case "ANDREW1":
					bugUser=29;//Andrew
					break;
				case "JORDANS":
				case "JORDANS3":
					bugUser=4;//jsparks
					break;
				case "JASON":
					bugUser=18;//jsalmon
					break;
				case "DAVID":
					bugUser=27;//david
					break;
				case "DEREK":
					bugUser=1;//grahamde
					break;
				case "SAM":
					bugUser=25;//sam
					break;
				case "RYAN":
				case "RYAN1":
					bugUser=20;//Ryan
					break;
				case "CAMERON":
					bugUser=21;//Cameron
					break;
				case "TRAVIS":
					bugUser=22;//tgriswold
					break;
				case "ALLEN":
					bugUser=24;//allen
					break;
				case "JOSH":
					bugUser=26;//josh
					break;
				case "JOE":
					bugUser=28;//joe
					break;
				case "CHRISM":
					bugUser=30;//chris
					break;
				case "SAUL":
					bugUser=31;//saul
					break;
				case "MATHERINL":
					bugUser=32;//matherinl
					break;
				case "LINDSAYS":
					bugUser=33;//linsdays
					break;
				case "BRENDANB":
					bugUser=34;//brendanb
					break;
				case "KENDRAS":
					bugUser=35;//kendras
					break;
				case "STEVENS":
					bugUser=37;//stevens
					break;
				case "ANDREWD":
					bugUser=39;
					break;
				case "LUKEM":
					bugUser=41;//lukem
					break;
				case "DEVINF":
					bugUser=43;
					break;
				case "NICHOLASL":
					bugUser=45;
					break;
				case "ROCHELLES":
					bugUser=47;
					break;
				case "PATRICKC":
					bugUser=49;
					break;
				case "JOES1":
					bugUser=51;
					break;
				case "HANNAHN":
					bugUser=53;
					break;
				case "ALEXB":
					bugUser=55;
					break;
				case "JASONL":
				case "JASONL1":
					bugUser=57;
					break;
				case "BRITTANYM":
				case "BRITTANYM1":
					bugUser=59;
					break;
				case "ANDREWJ":
					bugUser=61;
					break;
				case "RANDOMD":
					bugUser=63;
					break;
				case "GABEJ":
					bugUser=65;
					break;
				case "SEANS":
					bugUser=67;
					break;
				case "MARKD":
					bugUser=69;
					break;
				case "BRANDONJ":
					bugUser=71;
					break;
				case "SARAHL":
					bugUser=73;
					break;
				case "JASONY":
					bugUser=75;//JasonY
					break;
				case "ZAKK":
					bugUser=77;
					break;
				case "ALEXG":
					bugUser=83;
					break;
				case "FELIXR":
					bugUser=81;
					break;
				case "JUSTINE":
					bugUser=79;
					break;
				default:
					bugUser=2;//Tech Support
					break;
			}
			return bugUser;
		}

		///<summary>Returns the value stored within the BugSubmissionsCountPreviousVersions preference from the bugs database. Returns -1 if error.</summary>
		public static int GetCountPreviousVersions(bool useConnectionStore=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),useConnectionStore);
			}
			if(!TryGetPrefValue("BugSubmissionsCountPreviousVersions",out int countPreviousVersions,useConnectionStore:useConnectionStore)) {
				countPreviousVersions=-1;
			}
			return countPreviousVersions;
		}

		///<summary>Attempts to get the ValueString from the bug's preference table for the given PrefName.  The ValueString retrieved from the db will be converted to T.
		///Returns true if the preference was found and successfully converted to the type of T.  Otherwise retVal will be set to the default for T and false will be returned.</summary>
		public static bool TryGetPrefValue<T>(string prefName,out T retVal,bool useConnectionStore=true) {
			//No need to check RemotingRole; uses an out parameter.
			retVal=default(T);
			try {
				string val="";
				DataAction.RunBugsHQ(() => {
					val=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName)+"'");
				},useConnectionStore:useConnectionStore);
				retVal=(T)Convert.ChangeType(val,typeof(T));
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		/// <summary>Returns true if versionsFixed is empty or if all version formats in text can be parsed into Version object.</summary>
		public static bool VersionsAreValid(string versionsFixed){
			if(versionsFixed.IsNullOrEmpty()){
				return true;
			}
			return versionsFixed.Split(';').All(x => Version.TryParse(x,out Version result));
		}

		///<summary>Performs a Remoting Role Check before retrieving Customer Version Table from BugsHQ.</summary>
		public static DataTable GetCustomerVersionsForRegKey(string regKey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),regKey);
			}
			string command=$@"SELECT * FROM customerversion 
					WHERE RegistrationKey='{POut.String(regKey)}'
					ORDER BY DateTimeUpdate DESC;";
			DataTable table=DataAction.GetBugsHQ(() => Db.GetTable(command),false);
			return table;
		}
	}
}