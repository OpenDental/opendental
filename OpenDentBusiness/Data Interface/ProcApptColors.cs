using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcApptColors{
		#region CachePattern

		private class ProcApptColorCache : CacheListAbs<ProcApptColor> {
			protected override List<ProcApptColor> GetCacheFromDb() {
				string command="SELECT * FROM procapptcolor ORDER BY CodeRange";
				return Crud.ProcApptColorCrud.SelectMany(command);
			}
			protected override List<ProcApptColor> TableToList(DataTable table) {
				return Crud.ProcApptColorCrud.TableToList(table);
			}
			protected override ProcApptColor Copy(ProcApptColor procApptColor) {
				return procApptColor.Copy();
			}
			protected override DataTable ListToTable(List<ProcApptColor> listProcApptColors) {
				return Crud.ProcApptColorCrud.ListToTable(listProcApptColors,"ProcApptColor");
			}
			protected override void FillCacheIfNeeded() {
				ProcApptColors.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcApptColorCache _procApptColorCache=new ProcApptColorCache();

		public static List<ProcApptColor> GetDeepCopy(bool isShort=false) {
			return _procApptColorCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procApptColorCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procApptColorCache.FillCacheFromTable(table);
				return table;
			}
			return _procApptColorCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ProcApptColor procApptColor){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				procApptColor.ProcApptColorNum=Meth.GetLong(MethodBase.GetCurrentMethod(),procApptColor);
				return procApptColor.ProcApptColorNum;
			}
			return Crud.ProcApptColorCrud.Insert(procApptColor);
		}

		///<summary></summary>
		public static void Update(ProcApptColor procApptColor){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procApptColor);
				return;
			}
			Crud.ProcApptColorCrud.Update(procApptColor);
		}

		///<summary></summary>
		public static void Delete(long procApptColorNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procApptColorNum);
				return;
			}
			string command= "DELETE FROM procapptcolor WHERE ProcApptColorNum = "+POut.Long(procApptColorNum);
			Db.NonQ(command);
		}

		/*
		///<summary>Gets one ProcApptColor from the db.</summary>
		public static ProcApptColor GetOne(long procApptColorNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ProcApptColor>(MethodBase.GetCurrentMethod(),procApptColorNum);
			}
			return Crud.ProcApptColorCrud.SelectOne(procApptColorNum);
		}*/

		///<summary>Supply code such as D####.  Returns null if no match</summary>
		public static ProcApptColor GetMatch(string procCode) {
			string code1="";
			string code2="";
			List<ProcApptColor> listProcApptColors=ProcApptColors.GetDeepCopy();
			for(int i=0;i<listProcApptColors.Count;i++) {//using public property to trigger refresh if needed.
				if(listProcApptColors[i].CodeRange.Contains("-")) {
					string[] codeSplit=listProcApptColors[i].CodeRange.Split('-');
					code1=codeSplit[0].Trim();
					code2=codeSplit[1].Trim();
				}
				else{
					code1=listProcApptColors[i].CodeRange.Trim();
					code2=listProcApptColors[i].CodeRange.Trim();
				}
				if(procCode.CompareTo(code1)<0 || procCode.CompareTo(code2)>0) {
					continue;
				}
				return listProcApptColors[i];
			}
			return null;
		}
	}
}