using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeBase;
using Newtonsoft.Json.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ActiveInstances{
		private static ReaderWriterLockSlim _lock=new ReaderWriterLockSlim();
		private static ActiveInstance _activeInstance=null;

		///<summary>Gets the Active Instance for the current Open Dental session. This is not
		///honored on the middle tier server, and should not be called from there.</summary>
		public static ActiveInstance GetActiveInstance(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				throw new ApplicationException("Cannot access CurrentActiveInstace from middle tier server");
			}
			_lock.EnterReadLock();
			try {
				return _activeInstance;
			}
			finally {
				_lock.ExitReadLock();
			}
		}
		
		///<summary>Sets the Active Instance for the current Open Dental session. This is not
		///honored on the middle tier server, and should not be called from there.</summary>
		private static void SetActiveInstance(ActiveInstance activeInstance){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				throw new ApplicationException("Cannot access CurrentActiveInstace from middle tier server");
			}
			_lock.EnterWriteLock();
			try {
				_activeInstance=activeInstance;
			}
			finally {
				_lock.ExitWriteLock();
			}
		}	

		#region Get Methods
		///<summary>Returns all Active Instance rows from db</summary>
		public static List<ActiveInstance> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ActiveInstance>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM activeinstance";
			return Crud.ActiveInstanceCrud.SelectMany(command);
		}

		/// <summary>Returns all ActiveInstance rows from db with a DateTRecorded value older than 4 minutes</summary>
		public static List<ActiveInstance> GetAllOldInstances() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ActiveInstance>>(MethodBase.GetCurrentMethod());
			}
			DateTime dateTimeToCheck=DateTime.Now.AddMinutes(-4);
			string command="SELECT * FROM activeinstance WHERE DateTRecorded < " + POut.DateT(dateTimeToCheck);
			return Crud.ActiveInstanceCrud.SelectMany(command);
		}

		///<summary>Returns all ActiveInstance rows from db with a DateTRecorded value newer than 4 minutes.</summary>
		public static List<ActiveInstance> GetAllResponsiveActiveInstances() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ActiveInstance>>(MethodBase.GetCurrentMethod());
			}
			DateTime dateTimeToCheck=DateTime.Now.AddMinutes(-4);
			string command="SELECT * FROM activeinstance WHERE DateTRecorded > "+POut.DateT(dateTimeToCheck);
			return Crud.ActiveInstanceCrud.SelectMany(command);
		}

		/// <summary>Returns a count of all ODCloud Active Instances newer than 4 minutes. The timeframe is important so old Active Instance rows
		/// that linger due to a non-graceful shutdown do not affect user's ability to log in.</summary>
		public static int GetCountCloudActiveInstances(long excludeInstanceNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),excludeInstanceNum);
			}
			DateTime dateTimeToCheck=DateTime.Now.AddMinutes(-4);
			string command="SELECT COUNT(*) FROM activeinstance WHERE DateTRecorded > "+POut.DateT(dateTimeToCheck)
				+" AND ConnectionType="+POut.Enum(ConnectionTypes.ODCloud);
			if(excludeInstanceNum!=0) {
				command+=" AND ActiveInstanceNum!="+POut.Long(excludeInstanceNum);
			}
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Gets one ActiveInstance from the db based on the ActiveInstanceNum.</summary>
		public static ActiveInstance GetOne(long activeInstanceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ActiveInstance>(MethodBase.GetCurrentMethod(),activeInstanceNum);
			}
			return Crud.ActiveInstanceCrud.SelectOne(activeInstanceNum);
		}

		/// <summary>Gets on ActiveInstance from the db based on the UserNum, ComputerNum, and ProcessId</summary>
		public static ActiveInstance GetOne(long userNum,long computerNum,long processId) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ActiveInstance>(MethodBase.GetCurrentMethod(),userNum,computerNum,processId);
			}
			string command="SELECT * FROM activeinstance"
				+" WHERE UserNum="+POut.Long(userNum)
				+" AND ComputerNum="+POut.Long(computerNum)
				+" AND ProcessId="+POut.Long(processId);
			return Crud.ActiveInstanceCrud.SelectOne(command);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(ActiveInstance activeInstance) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				activeInstance.ActiveInstanceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),activeInstance);
				return activeInstance.ActiveInstanceNum;
			}
			activeInstance.DateTRecorded=DateTime.Now;
			long activeInstanceNum=Crud.ActiveInstanceCrud.Insert(activeInstance);
			return activeInstanceNum;
		}

		///<summary></summary>
		public static void Update(ActiveInstance activeInstance) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),activeInstance);
				return;
			}
			activeInstance.DateTRecorded=DateTime.Now;
			Crud.ActiveInstanceCrud.Update(activeInstance);
		}

		/// <summary>Checks whether an Active Instance exists based on parameters given. Updates or inserts conditionally based on existence.
		/// If no current Active Instance is detected it will set the current Active Instance to one just inserted.</summary>
		public static void Upsert(long userNum,long computerNum,long processId) {
			//No need to check remoting role, no call to db. Sets the public static field CurrentActiveInstance.
			ActiveInstance activeInstance=GetActiveInstance()??GetOne(userNum,computerNum,processId);
			if(activeInstance==null) {
				activeInstance=new ActiveInstance();
				if(ODBuild.IsWeb()) {
					activeInstance.ConnectionType=ConnectionTypes.ODCloud;
				}
				else if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientDirect) {
					activeInstance.ConnectionType=ConnectionTypes.Direct;
				}
				else {
					activeInstance.ConnectionType=ConnectionTypes.MiddleTier;
				}
				activeInstance.ComputerNum=computerNum;
				activeInstance.ProcessId=processId;
				activeInstance.UserNum=userNum;
				activeInstance.DateTimeLastActive=DateTime.Now;
				Insert(activeInstance);
				SetActiveInstance(activeInstance);
			}
			else {
				activeInstance.DateTimeLastActive=Security.DateTimeLastActivity;
				if(ODBuild.IsWeb() && (activeInstance.UserNum!=userNum || activeInstance.ComputerNum!=computerNum || activeInstance.ProcessId!=processId)) {
					//Cloud instances start with computer name UNKNOWN until the computer name can be retrieved from the browser by the ODCloudMachineName thread
					activeInstance.UserNum=userNum;
					activeInstance.ComputerNum=computerNum;
					activeInstance.ProcessId=processId;
					SetActiveInstance(activeInstance);
				}
				Update(activeInstance);
			}
		}

		///<summary></summary>
		public static void Delete(long activeInstanceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),activeInstanceNum);
				return;
			}
			Crud.ActiveInstanceCrud.Delete(activeInstanceNum);
		}

		///<summary>Delete several rows based on a list of passed in Active Instances.</summary>
		public static void DeleteMany(List<ActiveInstance> listActiveInstances) {
			listActiveInstances?.RemoveAll(x => x==null);
			if(listActiveInstances.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listActiveInstances);
				return;
			}
			string command="DELETE FROM activeinstance WHERE ActiveInstanceNum IN("+string.Join(",",listActiveInstances.Select(x => x.ActiveInstanceNum).ToList())+")";
			Db.NonQ(command);
		}
		#endregion Modification Methods

		#region Misc Methods
		///<summary>Runs Signalods.SetInvalid on each ActiveInstance passed in.</summary>
		public static void CloseActiveInstances(List<ActiveInstance> listActiveInstances) {
			if(listActiveInstances.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listActiveInstances);
				return;
			}
			for(int i=0;i<listActiveInstances.Count;i++) {
				Signalods.SetInvalid(InvalidType.ActiveInstance,KeyType.Undefined,listActiveInstances[i].ActiveInstanceNum);
			}
		}
		#endregion Misc Methods
	}
}