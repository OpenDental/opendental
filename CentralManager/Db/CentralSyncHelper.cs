using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using System.Windows.Forms;
using CodeBase;
using System.Threading;
using OpenDental.UI;


namespace CentralManager {
	public static class CentralSyncHelper {
		//Each field represents one thing needed to sync settings to remote databases.
		private static List<Userod> _listCEMTUsers;
		private static string _securityLockDate;
		private static int _securityLockDays;
		private static bool _securityLockAdmin;
		private static bool _securityCentralLock;
		private static string _syncCode;
		private static List<AlertSub> _listAlertSubs;

		///<summary>Called from the interface to sync all settings.</summary>
		public static string PushBoth(List<CentralConnection> listConns) {
			return PushSecurity(listConns,false,false);
		}

		///<summary>Called from the interface to sync just users, usergroups and permissions.</summary>
		public static string PushUsers(List<CentralConnection> listConns) {
			return PushSecurity(listConns,true,false);
		}

		///<summary>Called from the interface to sync just security lock settings.</summary>
		public static string PushLocks(List<CentralConnection> listConns) {
			return PushSecurity(listConns,false,true);
		}

		///<summary>Gets all of the usergroupattaches from the passed-in list and syncs it with the remote server's CEMT usergroupattaches.</summary>
		private static void SyncUserGroupAttaches(List<CentralUserData> listCentralUserData) {
			//Sync usergroup attaches
			//Get all ListUserGroupAttaches in listCentralUserData and flatten it into one list. 
			List<UserGroupAttach> listUserGroupAttachCEMT = listCentralUserData.SelectMany(x => x.ListUserGroupAttaches).Select(x => x.Copy()).ToList();
			listUserGroupAttachCEMT = UserGroupAttaches.TranslateCEMTToLocal(listUserGroupAttachCEMT);
			List<UserGroupAttach> listUserGroupAttachRemote = UserGroupAttaches.GetForCEMTUsersAndUserGroups();
			UserGroupAttaches.SyncCEMT(listUserGroupAttachCEMT,listUserGroupAttachRemote);
		}

		///<summary>Generic function that launches different thread methods depending on how isSyncUsers and isSyncLocks are set.  Set only one or the other to true.</summary>
		private static string PushSecurity(List<CentralConnection> listConns,bool isUsers,bool isLocks) {
			//Get CEMT users, groups, and associated permissions
			List<string> listSyncErrors=new List<string>();
			_listCEMTUsers=Userods.GetUsersForCEMT();
			_securityLockDate=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			_securityLockDays=PrefC.GetInt(PrefName.SecurityLockDays);
			_securityLockAdmin=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
			_securityCentralLock=PrefC.GetBool(PrefName.CentralManagerSecurityLock);
			_syncCode=PrefC.GetString(PrefName.CentralManagerSyncCode);
			_listAlertSubs=AlertSubs.GetAll();
			List<CentralUserData> listCentralUserData=new List<CentralUserData>();
			List<UserGroup> listUserGroups = UserGroups.GetCEMTGroups();
			foreach(UserGroup userGroup in listUserGroups) {//for each CEMT user group
				//get all usergroupattaches, userods, and grouppermissions.
				List<UserGroupAttach> listUserGroupAttaches = UserGroupAttaches.GetForUserGroup(userGroup.UserGroupNum); 
				List<Userod> listUserOds = Userods.GetForGroup(userGroup.UserGroupNum);
				List<GroupPermission> listGroupPermissions = GroupPermissions.GetPerms(userGroup.UserGroupNum);
				//then create a new CentralUserData and add it to the list.
				listCentralUserData.Add(new CentralUserData(userGroup,listUserOds,listGroupPermissions,listUserGroupAttaches));
			}
			string failedConns="";
			string nameConflicts="";
			string failedSyncCodes="";
			for(int i=0;i<listConns.Count;i++) {
				List<CentralUserData> listCentralDataForThreads=new List<CentralUserData>();
				for(int j=0;j<listCentralUserData.Count;j++) {
					listCentralDataForThreads.Add(listCentralUserData[j].Copy());
				}
				ODThread odThread=null;
				if(isUsers) {
					odThread=new ODThread(ConnectAndSyncUsers,new object[] { listConns[i],listCentralDataForThreads});
					odThread.AddExceptionHandler((e) => {
						string error=$"Connection '{GetServerNameDisplay((CentralConnection)odThread.Parameters[0])}' failed to sync users:\r\n{e}";
						listSyncErrors.Add(error);
					});
				}
				else if(isLocks) {
					odThread=new ODThread(ConnectAndSyncLocks,new object[] { listConns[i] });
					odThread.AddExceptionHandler((e) => {
						string error=$"Connection '{GetServerNameDisplay((CentralConnection)odThread.Parameters[0])}' failed to sync locks:\r\n{e}";
						listSyncErrors.Add(error);
					});
				}
				else {
					odThread=new ODThread(ConnectAndSyncAll,new object[] { listConns[i],listCentralDataForThreads });
					odThread.AddExceptionHandler((e) => {
						string error=$"Connection '{GetServerNameDisplay((CentralConnection)odThread.Parameters[0])}' failed to sync all:\r\n{e}";
						listSyncErrors.Add(error);
					});
				}
				odThread.GroupName="Sync";
				odThread.Start(false);
			}
			ODThread.JoinThreadsByGroupName(Timeout.Infinite,"Sync");
			List<ODThread> listComplThreads=ODThread.GetThreadsByGroupName("Sync");
			for(int i=0;i<listComplThreads.Count;i++) {
				if(listComplThreads[i].Tag==null) {
					continue;//Failed due to lacking credentials
				}
				failedConns+=((List<string>)listComplThreads[i].Tag)[0];
				nameConflicts+=((List<string>)listComplThreads[i].Tag)[1];
				failedSyncCodes+=((List<string>)listComplThreads[i].Tag)[2];
				listComplThreads[i].QuitAsync();
			}
			string errorText="";
			if(listSyncErrors.Count > 0) {
				errorText+="Critical Errors:\r\n\r\n"+string.Join("\r\n\r\n",listSyncErrors.Select(x => x.ToString()))+"\r\n\r\n";
			}
			if(failedConns!="") {
				errorText+="Failed Connections:\r\n"+failedConns+"Please try these connections again.\r\n";
			}
			if(nameConflicts!="") {
				errorText+="Name Conflicts:\r\n"+nameConflicts+"Please rename users and try again.\r\n";
			}
			if(failedSyncCodes!="") {
				errorText+="Incorrect Sync Codes:\r\n"+failedSyncCodes+"\r\n";
			}
			if(string.IsNullOrWhiteSpace(errorText)) {
				errorText="Done";
			}
			return errorText;
		}

		///<summary>Function used by threads to connect to remote databases and sync all settings.</summary>
		private static void ConnectAndSyncAll(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			List<CentralUserData> listCentralUserData=(List<CentralUserData>)odThread.Parameters[1];
			string serverName=GetServerNameDisplay(connection);
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {//No updating the cache since we're going to be connecting to multiple remote servers at the same time.
				odThread.Tag=new List<string>() { serverName+"\r\n","","" };
				connection.ConnectionStatus="OFFLINE";
				return;
			}
			string remoteSyncCode=PrefC.GetStringNoCache(PrefName.CentralManagerSyncCode);
			if(remoteSyncCode!=_syncCode) {
				if(remoteSyncCode=="") {
					Prefs.UpdateStringNoCache(PrefName.CentralManagerSyncCode,_syncCode);//Lock in the sync code for the remote server.
				}
				else {
					odThread.Tag=new List<string>() { serverName+"\r\n","",remoteSyncCode };
					return;
				}
			}
			//Push the preferences to the server.
			Prefs.UpdateStringNoCache(PrefName.SecurityLockDate,_securityLockDate);
			Prefs.UpdateIntNoCache(PrefName.SecurityLockDays,_securityLockDays);
			Prefs.UpdateBoolNoCache(PrefName.SecurityLockIncludesAdmin,_securityLockAdmin);
			Prefs.UpdateBoolNoCache(PrefName.CentralManagerSecurityLock,_securityCentralLock);
			Signalods.SetInvalidNoCache(InvalidType.Prefs);
			SecurityLogs.MakeLogEntryNoCache(Permissions.SecurityAdmin,0,"Enterprise Management Tool updated security settings");
			//Get remote users, usergroups, associated permissions, and alertsubs
			List<Userod> listRemoteUsers=Userods.GetUsersNoCache();
			#region Detect Conflicts
			//User conflicts
			bool nameConflict=false;
			string nameConflicts="";
			for(int i=0;i<_listCEMTUsers.Count;i++) {
				for(int j=0;j<listRemoteUsers.Count;j++) {
					if(listRemoteUsers[j].UserName==_listCEMTUsers[i].UserName && listRemoteUsers[j].UserNumCEMT==0) {//User doesn't belong to CEMT
						nameConflicts+=listRemoteUsers[j].UserName+" already exists in "+serverName+"\r\n";
						nameConflict=true;
						break;
					}
				}
			}
			if(nameConflict) {
				odThread.Tag=new List<string>() { serverName,nameConflicts,"" };
				return;//Skip on to the next connection.
			}
			#endregion Detect Conflicts
			List<UserGroup> listRemoteCEMTUserGroups=UserGroups.GetCEMTGroupsNoCache();
			List<UserGroup> listCEMTUserGroups=new List<UserGroup>();
			List<Clinic> listRemoteClinics=Clinics.GetClinicsNoCache();
			for(int i=0;i<listCentralUserData.Count;i++) {
				listCEMTUserGroups.Add(listCentralUserData[i].UserGroup.Copy());
			}
			//SyncUserGroups returns the list of UserGroups for deletion so it can be used after syncing Users and GroupPermissions.
			List<UserGroup> listRemoteCEMTUserGroupsForDeletion=CentralUserGroups.Sync(listCEMTUserGroups,listRemoteCEMTUserGroups);
			listRemoteCEMTUserGroups=UserGroups.GetCEMTGroupsNoCache();
			for(int i=0;i<listCentralUserData.Count;i++) {
				List<GroupPermission> listGroupPerms=new List<GroupPermission>();
				for(int j=0;j<listRemoteCEMTUserGroups.Count;j++) {
					if(listCentralUserData[i].UserGroup.UserGroupNumCEMT==listRemoteCEMTUserGroups[j].UserGroupNumCEMT) {
						for(int k=0;k<listCentralUserData[i].ListGroupPermissions.Count;k++) {
							listCentralUserData[i].ListGroupPermissions[k].UserGroupNum=listRemoteCEMTUserGroups[j].UserGroupNum;//fixing primary keys to be what's in remote db
						}
						listGroupPerms=GroupPermissions.GetPermsNoCache(listRemoteCEMTUserGroups[j].UserGroupNum);
					}
				}
				CentralUserods.Sync(listCentralUserData[i].ListUsers,ref listRemoteUsers);
				CentralGroupPermissions.Sync(listCentralUserData[i].ListGroupPermissions,listGroupPerms);
			}
			//Sync usergroup attaches
			SyncUserGroupAttaches(listCentralUserData);
			for(int j=0;j<listRemoteCEMTUserGroupsForDeletion.Count;j++) {
				UserGroups.DeleteNoCache(listRemoteCEMTUserGroupsForDeletion[j]);
			}
			if(_listAlertSubs.Count>0) {
				listRemoteUsers=Userods.GetUsersNoCache();//Refresh users so we can do alertsubs.
			}
			List<AlertSub> listAlertSubsToInsert=GetMissingAlertSubsForRemoteUsers(_listAlertSubs,_listCEMTUsers,listRemoteUsers,listRemoteClinics);
			AlertSubs.DeleteAndInsertForSuperUsers(_listCEMTUsers,listAlertSubsToInsert);
			//Refresh server's cache of userods
			Signalods.SetInvalidNoCache(InvalidType.Security);
			SecurityLogs.MakeLogEntryNoCache(Permissions.SecurityAdmin,0,"Enterprise Management Tool synced users.");
			odThread.Tag=new List<string>() { "","","" };//No errors.
		}

		///<summary>Function used by threads to connect to remote databases and sync user settings.</summary>
		private static void ConnectAndSyncUsers(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			List<CentralUserData> listCentralUserData=(List<CentralUserData>)odThread.Parameters[1];
			string serverName=GetServerNameDisplay(connection);
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {//No updating the cache since we're going to be connecting to multiple remote servers at the same time.
				odThread.Tag=new List<string>() { serverName+"\r\n","","" };
				connection.ConnectionStatus="OFFLINE";
				return;
			}
			string remoteSyncCode=PrefC.GetStringNoCache(PrefName.CentralManagerSyncCode);
			if(remoteSyncCode!=_syncCode) {
				if(remoteSyncCode=="") {
					Prefs.UpdateStringNoCache(PrefName.CentralManagerSyncCode,_syncCode);//Lock in the sync code for the remote server.
				}
				else {
					odThread.Tag=new List<string>() { serverName+"\r\n","",remoteSyncCode };
					return;
				}
			}
			//Get remote users, usergroups, associated permissions, and alertsubs
			List<Userod> listRemoteUsers=Userods.GetUsersNoCache();
			#region Detect Conflicts
			//User conflicts
			bool nameConflict=false;
			string nameConflicts="";
			for(int i=0;i<_listCEMTUsers.Count;i++) {
				for(int j=0;j<listRemoteUsers.Count;j++) {
					if(listRemoteUsers[j].UserName==_listCEMTUsers[i].UserName && listRemoteUsers[j].UserNumCEMT==0) {//User doesn't belong to CEMT
						nameConflicts+=listRemoteUsers[j].UserName+" already exists in "+serverName+"\r\n";
						nameConflict=true;
						break;
					}
				}
			}
			if(nameConflict) {
				odThread.Tag=new List<string>() { serverName+"\r\n",nameConflicts,"" };
				return;//Skip on to the next connection.
			}
			#endregion Detect Conflicts
			List<UserGroup> listRemoteCEMTUserGroups=UserGroups.GetCEMTGroupsNoCache();
			List<UserGroup> listCEMTUserGroups=new List<UserGroup>();
			List<Clinic> listRemoteClinics=Clinics.GetClinicsNoCache();
			for(int i=0;i<listCentralUserData.Count;i++) {
				listCEMTUserGroups.Add(listCentralUserData[i].UserGroup.Copy());
			}
			//SyncUserGroups returns the list of UserGroups for deletion so it can be used after syncing Users and GroupPermissions.
			List<UserGroup> listRemoteCEMTUserGroupsForDeletion=CentralUserGroups.Sync(listCEMTUserGroups,listRemoteCEMTUserGroups);
			listRemoteCEMTUserGroups=UserGroups.GetCEMTGroupsNoCache();
			for(int i=0;i<listCentralUserData.Count;i++) {
				List<GroupPermission> listGroupPerms=new List<GroupPermission>();
				for(int j=0;j<listRemoteCEMTUserGroups.Count;j++) {
					if(listCentralUserData[i].UserGroup.UserGroupNumCEMT==listRemoteCEMTUserGroups[j].UserGroupNumCEMT) {
						for(int k=0;k<listCentralUserData[i].ListGroupPermissions.Count;k++) {
							listCentralUserData[i].ListGroupPermissions[k].UserGroupNum=listRemoteCEMTUserGroups[j].UserGroupNum;//fixing primary keys to be what's in remote db
						}
						listGroupPerms=GroupPermissions.GetPermsNoCache(listRemoteCEMTUserGroups[j].UserGroupNum);
					}
				}
				CentralUserods.Sync(listCentralUserData[i].ListUsers,ref listRemoteUsers);
				CentralGroupPermissions.Sync(listCentralUserData[i].ListGroupPermissions,listGroupPerms);
			}
			//Sync usergroup attaches
			SyncUserGroupAttaches(listCentralUserData);
			for(int j=0;j<listRemoteCEMTUserGroupsForDeletion.Count;j++) {
				UserGroups.DeleteNoCache(listRemoteCEMTUserGroupsForDeletion[j]);
			}
			if(_listAlertSubs.Count>0) {
				listRemoteUsers=Userods.GetUsersNoCache();//Refresh users so we can do alertsubs.
			}
			List<AlertSub> listAlertSubsToInsert=GetMissingAlertSubsForRemoteUsers(_listAlertSubs,_listCEMTUsers,listRemoteUsers,listRemoteClinics);
			AlertSubs.DeleteAndInsertForSuperUsers(_listCEMTUsers,listAlertSubsToInsert);
			//Refresh server's cache of userods
			Signalods.SetInvalidNoCache(InvalidType.Security);
			SecurityLogs.MakeLogEntryNoCache(Permissions.SecurityAdmin,0,"Enterprise Management Tool synced users.");
			odThread.Tag=new List<string>() { "","","" };//No errors.
		}

		///<summary>Function used by threads to connect to remote databases and sync security lock settings.</summary>
		private static void ConnectAndSyncLocks(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			string serverName=GetServerNameDisplay(connection);
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {//No updating the cache since we're going to be connecting to multiple remote servers at the same time.				
				odThread.Tag=new List<string>() { serverName+"\r\n","","" };
				connection.ConnectionStatus="OFFLINE";
				return;
			}
			string remoteSyncCode=PrefC.GetString(PrefName.CentralManagerSyncCode);
			if(remoteSyncCode!=_syncCode) {
				if(remoteSyncCode=="") {
					Prefs.UpdateStringNoCache(PrefName.CentralManagerSyncCode,_syncCode);//Lock in the sync code for the remote server.
				}
				else {
					odThread.Tag=new List<string>() { serverName+"\r\n","",remoteSyncCode };
					return;
				}
			}
			//Push the preferences to the server.
			Prefs.UpdateStringNoCache(PrefName.SecurityLockDate,_securityLockDate);
			Prefs.UpdateIntNoCache(PrefName.SecurityLockDays,_securityLockDays);
			Prefs.UpdateBoolNoCache(PrefName.SecurityLockIncludesAdmin,_securityLockAdmin);
			Prefs.UpdateBoolNoCache(PrefName.CentralManagerSecurityLock,_securityCentralLock);
			Signalods.SetInvalidNoCache(InvalidType.Prefs);
			SecurityLogs.MakeLogEntryNoCache(Permissions.SecurityAdmin,0,"Enterprise Management Tool updated security settings");
			odThread.Tag=new List<string>() { "","","" };//No errors.
		}

		///<summary>Returns a list of alert subs that exists in the CEMT list and not in the remote list.</summary>
		private static List<AlertSub> GetMissingAlertSubsForRemoteUsers(List<AlertSub> listCEMTAlertSubs,List<Userod> listCEMTUsers,
			List<Userod> listRemoteUsers,List<Clinic> listRemoteClinics) 
		{
			List<AlertSub> listAlertSubsToInsert=new List<AlertSub>();
			//For each AlertSub, make a copy of that AlertSub for each Clinic.
			foreach(AlertSub alertSub in listCEMTAlertSubs) {
				Userod userodCEMT=listCEMTUsers.Find(x => x.UserNum==alertSub.UserNum);
				if(userodCEMT==null) {
					continue;//no alert to enter for CEMT user
				}
				foreach(Clinic clinic in listRemoteClinics) {
					Userod userodRemote=listRemoteUsers.Find(x => x.UserName==userodCEMT.UserName);
					if(userodRemote==null) {
						continue;//no alert to enter for remote user
					}
					AlertSub alert=new AlertSub();
					alert.ClinicNum=clinic.ClinicNum;
					alert.Type=alertSub.Type;
					alert.UserNum=userodRemote.UserNum;
					listAlertSubsToInsert.Add(alert);
				}
			}
			return listAlertSubsToInsert;
		}

		private static string GetServerNameDisplay(CentralConnection connection) {
			if(connection==null) {
				return "";
			}
			if(string.IsNullOrEmpty(connection.ServiceURI)) {
				return connection.ServerName+", "+connection.DatabaseName;
			}
			else {
				return connection.ServiceURI;
			}
		}

		///<summary>Custom data structure for containing a UserGroup and its associated list of GroupPermissions and users.</summary>
		public struct CentralUserData {
			public UserGroup UserGroup;
			public List<Userod> ListUsers;
			public List<GroupPermission> ListGroupPermissions;
			public List<UserGroupAttach> ListUserGroupAttaches;

			public CentralUserData(UserGroup userGroup,List<Userod> listUsers,List<GroupPermission> listGroupPermissions, List<UserGroupAttach> listUserGroupAttaches) {
				UserGroup=userGroup;
				ListUsers=listUsers;
				ListGroupPermissions=listGroupPermissions;
				ListUserGroupAttaches=listUserGroupAttaches;
			}

			public CentralUserData Copy() {
				List<Userod> listUsers=new List<Userod>();
				for(int i=0;i<ListUsers.Count;i++){
					listUsers.Add(ListUsers[i].Copy());
				}
				List<GroupPermission> listGroupPermissions=new List<GroupPermission>();
				for(int i=0;i<ListGroupPermissions.Count;i++){
					listGroupPermissions.Add(ListGroupPermissions[i].Copy());
				}
				List<UserGroupAttach> listUserGroupAttaches = new List<UserGroupAttach>();
				for(int i = 0;i<ListUserGroupAttaches.Count;i++) {
					listUserGroupAttaches.Add(ListUserGroupAttaches[i].Copy());
				}
				return new CentralUserData(UserGroup.Copy(),listUsers,listGroupPermissions,listUserGroupAttaches);//DEEP COPY OVERKILL BUT WE LIKE IT THAT WAY
			}
		}

	}
}
