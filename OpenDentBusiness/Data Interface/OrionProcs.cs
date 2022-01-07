using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrionProcs{
		/*
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add RefreshCache and FillCache 
		//to the Cache.cs file with all the other Cache types.
#region CachePattern

		private class OrionProcCache : CacheListAbs<OrionProc> {
			protected override List<OrionProc> GetCacheFromDb() {
				string command="SELECT * FROM OrionProc ORDER BY ItemOrder";
				return Crud.OrionProcCrud.SelectMany(command);
			}
			protected override List<OrionProc> TableToList(DataTable table) {
				return Crud.OrionProcCrud.TableToList(table);
			}
			protected override OrionProc Copy(OrionProc OrionProc) {
				return OrionProc.Clone();
			}
			protected override DataTable ListToTable(List<OrionProc> listOrionProcs) {
				return Crud.OrionProcCrud.ListToTable(listOrionProcs,"OrionProc");
			}
			protected override void FillCacheIfNeeded() {
				OrionProcs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(OrionProc OrionProc) {
				return !OrionProc.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static OrionProcCache _OrionProcCache=new OrionProcCache();

		///<summary>A list of all OrionProcs. Returns a deep copy.</summary>
		public static List<OrionProc> ListDeep {
			get {
				return _OrionProcCache.ListDeep;
			}
		}

		///<summary>A list of all visible OrionProcs. Returns a deep copy.</summary>
		public static List<OrionProc> ListShortDeep {
			get {
				return _OrionProcCache.ListShortDeep;
			}
		}

		///<summary>A list of all OrionProcs. Returns a shallow copy.</summary>
		public static List<OrionProc> ListShallow {
			get {
				return _OrionProcCache.ListShallow;
			}
		}

		///<summary>A list of all visible OrionProcs. Returns a shallow copy.</summary>
		public static List<OrionProc> ListShort {
			get {
				return _OrionProcCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_OrionProcCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_OrionProcCache.FillCacheFromTable(table);
				return table;
			}
			return _OrionProcCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary>Gets one OrionProc from the db.</summary>
		public static OrionProc GetOne(long orionProcNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrionProc>(MethodBase.GetCurrentMethod(),orionProcNum);
			}
			return Crud.OrionProcCrud.SelectOne(orionProcNum);
		}

		///<summary>Gets one OrionProc from the db by ProcNum</summary>
		public static OrionProc GetOneByProcNum(long ProcNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrionProc>(MethodBase.GetCurrentMethod(),ProcNum);
			}
			string command="SELECT * FROM orionproc "
				+"WHERE ProcNum="+POut.Long(ProcNum)+" "+DbHelper.LimitAnd(1);
			return Crud.OrionProcCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(OrionProc orionProc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				orionProc.OrionProcNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orionProc);
				return orionProc.OrionProcNum;
			}
			return Crud.OrionProcCrud.Insert(orionProc);
		}

		///<summary></summary>
		public static void Update(OrionProc orionProc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orionProc);
				return;
			}
			Crud.OrionProcCrud.Update(orionProc);
		}

		public static DataTable GetCancelledScheduleDateByToothOrSurf(long PatNum, string ToothNum, string Surf) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatNum,ToothNum,Surf);
			}
			string optionalclause="";
			if(POut.String(ToothNum)==""){
				optionalclause=" AND procedurelog.Surf='"+POut.String(Surf)+"'";
			}
			string command="SELECT orionproc.DateScheduleBy,procedurelog.ToothNum,procedurelog.Surf "
				+"FROM orionproc "
				+"LEFT JOIN procedurelog ON orionproc.ProcNum=procedurelog.ProcNum "
				+"WHERE procedurelog.PatNum="+POut.Long(PatNum)
				+" AND orionproc.Status2=128 "
				+" AND procedurelog.ToothNum='"+POut.String(ToothNum)+"'"
				+optionalclause
				+" AND "+DbHelper.Year("orionproc.DateScheduleBy")+">1880"
				+" ORDER BY orionproc.DateScheduleBy ";
			command=DbHelper.LimitOrderBy(command,1);
			return Db.GetTable(command);
		}

		///<summary>Loops through every procedure attached to an appt and sets the Status2 to complete.</summary>
		public static void SetCompleteInAppt(List<Procedure> procsInAppt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procsInAppt);
				return;
			}
			OrionProc orionProc;
			for(int i=0;i<procsInAppt.Count;i++) {
				orionProc=GetOneByProcNum(procsInAppt[i].ProcNum);
				orionProc.Status2=OrionStatus.C;
				Update(orionProc);
			}
		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OrionProc> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrionProc>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orionproc WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrionProcCrud.SelectMany(command);
		}
		
		///<summary></summary>
		public static void Delete(long orionProcNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orionProcNum);
				return;
			}
			string command= "DELETE FROM orionproc WHERE OrionProcNum = "+POut.Long(orionProcNum);
			Db.NonQ(command);
		}
		

		
		*/
	}
}