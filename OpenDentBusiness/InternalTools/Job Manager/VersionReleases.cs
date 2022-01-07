using CodeBase;
using DataConnectionBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

namespace OpenDentBusiness {
	///<summary>Handles database commands related to the VersionRelease table in the db.</summary>
	public class VersionReleases {
		///<summary>Contains a list of versions that are to never be released to the public.  Currently, only the Major and Minor version numbers are considered.</summary>
		private static List<Version> _versionsBlackList=new List<Version> { new Version(4,9),new Version(13,3), };

		///<summary>Returns a list of all versions ordered by MajorNum, MinorNum, BuildNum, and IsForeign.</summary>
		public static List<VersionRelease> Refresh(bool useConnectionStore=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VersionRelease>>(MethodBase.GetCurrentMethod(),useConnectionStore);
			}
			List<VersionRelease> listVersionReleases=new List<VersionRelease>();
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM versionrelease ";
				//Exclude black listed versions.
				for(int i=0;i<_versionsBlackList.Count;i++) {
					if(i==0) {
						command+="WHERE ";
					}
					else {
						command+="AND ";
					}
					command+="(MajorNum,MinorNum) != ("+POut.Int(_versionsBlackList[i].Major)+","+POut.Int(_versionsBlackList[i].Minor)+") ";
				}
				//The order is very important to other entites calling this method.
				command+="ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC,IsForeign";
				listVersionReleases=RefreshAndFill(command);
			},useConnectionStore);
			return listVersionReleases;
		}

		///<summary>Returns false if given version was released prior to the last X versions.
		///X is defined by bug.Preference 'BugSubmissionsCountPreviousVersions' value, defaults to 1 if there is an issue with retrieving the pref.
		///Called from WebServiceMainHQ currently.  Does not check if on support.  Returns false if there are no stable or beta versions found.</summary>
		public static bool IsVersionRecent(Version version,bool isForeign) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),version,isForeign);
			}
			int maxVersions;
			Bugs.TryGetPrefValue<int>("BugSubmissionsCountPreviousVersions",out maxVersions);
			maxVersions=Math.Max(1,maxVersions);
			List<VersionRelease> listVersionReleases=Refresh(true);//Ordered by most recent release and then IsForeign
			List<VersionRelease> listBetaVersionReleases=listVersionReleases
				.Where(x => x.IsBeta
					&& x.IsForeign==isForeign 
					&& x.DateRelease!=DateTime.MinValue)
				.Take(maxVersions)//Safe even if list size is less than maxVersions.
				.ToList();
			if(listBetaVersionReleases.Count==0) {
				return false;
			}
			Version versionBetaMin=new Version(listBetaVersionReleases.Last().MajMinBuild0());
			List<VersionRelease> listStableVersionReleases=listVersionReleases
				.Where(x => !x.IsBeta
					&& x.IsForeign==isForeign
					&& x.DateRelease!=DateTime.MinValue
					&& ((x.MajorNum==versionBetaMin.Major && x.MinorNum!=versionBetaMin.Minor) || (x.MajorNum!=versionBetaMin.Major)))
				.Take(maxVersions)//Safe even if list size is less than maxVersions.
				.ToList();
			if(listStableVersionReleases.Count==0) {
				return false;
			}
			Version stablePre=new Version(listStableVersionReleases.Last().MajMinBuild0());
			return (version.CompareTo(stablePre)>=0 || version.CompareTo(versionBetaMin)>=0);
		}

		private static List<VersionRelease> RefreshAndFill(string command) {
			DataTable table=Db.GetTable(command);
			List<VersionRelease> retVal=new List<VersionRelease>();
			VersionRelease vers;
			for(int i=0;i<table.Rows.Count;i++) {
				vers=new VersionRelease();
				vers.VersionReleaseId= PIn.Int(table.Rows[i]["VersionReleaseId"].ToString());
				vers.MajorNum        = PIn.Int(table.Rows[i]["MajorNum"].ToString());
				vers.MinorNum        = PIn.Int(table.Rows[i]["MinorNum"].ToString());
				vers.BuildNum        = PIn.Int(table.Rows[i]["BuildNum"].ToString());
				vers.IsForeign       = PIn.Bool(table.Rows[i]["IsForeign"].ToString());
				vers.DateRelease     = PIn.Date(table.Rows[i]["DateRelease"].ToString());
				vers.IsBeta          = PIn.Bool(table.Rows[i]["IsBeta"].ToString());
				vers.HasConvertScript= PIn.Bool(table.Rows[i]["HasConvertScript"].ToString());
				retVal.Add(vers);
			}
			return retVal;
		}

		///<summary>Returns a fully formatted string of the most recent unreleased versions, from 1 to 3.</summary>
		public static string GetLastReleases(int versionsRequested) {
			//No need to check RemotingRole; no call to db.
			string versionsString="";
			List<VersionRelease> releaseList=GetLastThreeReleases();
			if(releaseList.Count>2 && versionsRequested>2) {
				versionsString+=releaseList[2].MajorNum.ToString()+"."+releaseList[2].MinorNum.ToString()+"."+releaseList[2].BuildNum.ToString()+".0";
			}
			if(releaseList.Count>1 && versionsRequested>1) {
				if(versionsString!="") {
					versionsString+=";";
				}
				versionsString+=releaseList[1].MajorNum.ToString()+"."+releaseList[1].MinorNum.ToString()+"."+releaseList[1].BuildNum.ToString()+".0";
			}
			if(releaseList.Count>0){
				if(versionsString!="") {
					versionsString+=";";
				}
				versionsString+=releaseList[0].MajorNum.ToString()+"."+releaseList[0].MinorNum.ToString()+"."+releaseList[0].BuildNum.ToString()+".0";
			}
			return versionsString;
		}

		public static List<VersionRelease> GetLastThreeReleases() {
			//No need to check RemotingRole; no call to db.
			return GetLastUnreleasedVersions(3);
		}

		///<summary>Returns the last X unreleased versions where X is the limit passed in.</summary>
		public static List<VersionRelease> GetLastUnreleasedVersions(int limit) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VersionRelease>>(MethodBase.GetCurrentMethod(),limit);
			}
			return DataAction.GetBugsHQ(() => {
				string command="SELECT * FROM versionrelease "
					+"WHERE DateRelease < '1880-01-01' "//we are only interested in non-released versions.
					+"AND IsForeign=0 "
					+string.Join("",_versionsBlackList.Select(x => $"AND (MajorNum,MinorNum) != ({POut.Int(x.Major)},{POut.Int(x.Minor)}) "))
					+"ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC "
					+$"LIMIT {POut.Int(limit)}";
				return RefreshAndFill(command);
			},false);
		}

		///<summary>Returns a fully formatted string of the possible head version.</summary>
		public static string GetPossibleHeadRelease() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string versionsString="";
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM versionrelease "
					+"WHERE DateRelease < '1880-01-01' "//we are only interested in non-released versions.
					+"AND IsForeign=0 ";
				//Exclude black listed versions.
				for(int i=0;i<_versionsBlackList.Count;i++) {
					command+="AND (MajorNum,MinorNum) != ("+POut.Int(_versionsBlackList[i].Major)+","+POut.Int(_versionsBlackList[i].Minor)+") ";
				}
				command+="ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC "
					+"LIMIT 3";//Might not be 3.
				List<VersionRelease> releaseList=RefreshAndFill(command);
				versionsString=releaseList[0].MajorNum.ToString()+"."+(releaseList[0].MinorNum+1).ToString()+".1.0";
			},false);
			return versionsString;
		}

		public static DateTime GetBetaDevelopmentStartDate() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod());
			}
			DateTime releaseDate=DateTime.Now.AddMonths(-3);
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM versionrelease "
					+"WHERE IsForeign=0 "
					+"AND BuildNum=1 ";
				//Exclude black listed versions.
				for(int i=0;i<_versionsBlackList.Count;i++) {
					command+="AND (MajorNum,MinorNum) != ("+POut.Int(_versionsBlackList[i].Major)+","+POut.Int(_versionsBlackList[i].Minor)+") ";
				}
				command+="ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC "
					+"LIMIT 3";//Might not be 3.
				List<VersionRelease> listVersionReleases=RefreshAndFill(command);
				if(!listVersionReleases.IsNullOrEmpty() && listVersionReleases.Count > 1) {
					releaseDate=listVersionReleases[1].DateRelease;
				}
			},false);
			return releaseDate;
		}

		public static DateTime GetBetaReleaseDate() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod());
			}
			DateTime releaseDate=DateTime.Now;
			DataAction.RunBugsHQ(() => {
				string command="SELECT * FROM versionrelease "
					+"WHERE IsForeign=0 "
					+"AND BuildNum=1 ";
				//Exclude black listed versions.
				for(int i=0;i<_versionsBlackList.Count;i++) {
					command+="AND (MajorNum,MinorNum) != ("+POut.Int(_versionsBlackList[i].Major)+","+POut.Int(_versionsBlackList[i].Minor)+") ";
				}
				command+="ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC "
					+"LIMIT 3";//Might not be 3.
				List<VersionRelease> listVersionReleases=RefreshAndFill(command);
				if(!listVersionReleases.IsNullOrEmpty()) {
					releaseDate=listVersionReleases[0].DateRelease;
				}
			},false);
			return releaseDate;
		}

		public static List<int> GetVersionsAfter(int majorVersion,int minorVersion) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<int>>(MethodBase.GetCurrentMethod(),majorVersion,minorVersion);
			}
			string command=$@" SELECT *
				FROM versionrelease
				WHERE DateRelease < '1880-01-01'
				AND IsForeign = 0 ";
			//Exclude black listed versions.
			for(int i = 0;i<_versionsBlackList.Count;i++) {
				command+="AND (MajorNum,MinorNum) != ("+POut.Int(_versionsBlackList[i].Major)+","+POut.Int(_versionsBlackList[i].Minor)+") ";
			}
			command+=$@"AND MajorNum>={majorVersion}
				ORDER BY MajorNum DESC,MinorNum DESC,BuildNum DESC";
			List<VersionRelease> listVersions=new List<VersionRelease>();
			DataAction.RunBugsHQ(() => listVersions=RefreshAndFill(command),false);
			List<int> retVal=new List<int>();
			foreach(VersionRelease version in listVersions) {
				if(version.MajorNum<=majorVersion && version.MinorNum<=minorVersion) {
					continue;
				}
				retVal.Add(PIn.Int(version.MajorNum.ToString()+version.MinorNum.ToString()));
			}
			return retVal;
		}

	}

	


	


}









