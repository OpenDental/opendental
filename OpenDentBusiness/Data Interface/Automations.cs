using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Automations {
		#region Cache Pattern

		private class AutomationCache : CacheListAbs<Automation> {
			protected override List<Automation> GetCacheFromDb() {
				string command="SELECT * FROM automation";
				return Crud.AutomationCrud.SelectMany(command);
			}
			protected override List<Automation> TableToList(DataTable table) {
				return Crud.AutomationCrud.TableToList(table);
			}
			protected override Automation Copy(Automation automation) {
				return automation.Copy();
			}
			protected override DataTable ListToTable(List<Automation> listAutomations) {
				return Crud.AutomationCrud.ListToTable(listAutomations,"Automation");
			}
			protected override void FillCacheIfNeeded() {
				Automations.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutomationCache _automationCache=new AutomationCache();

		public static List<Automation> GetDeepCopy(bool isShort=false) {
			return _automationCache.GetDeepCopy(isShort);
		}

		public static Automation GetFirstOrDefault(Func<Automation,bool> match,bool isShort=false) {
			return _automationCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_automationCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_automationCache.FillCacheFromTable(table);
				return table;
			}
			return _automationCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(Automation auto) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				auto.AutomationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),auto);
				return auto.AutomationNum;
			}
			return Crud.AutomationCrud.Insert(auto);
		}

		///<summary></summary>
		public static void Update(Automation auto) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),auto);
				return;
			}
			Crud.AutomationCrud.Update(auto);
		}

		///<summary></summary>
		public static void Delete(Automation auto) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),auto);
				return;
			}
			string command="DELETE FROM automation" 
				+" WHERE AutomationNum = "+POut.Long(auto.AutomationNum);
 			Db.NonQ(command);
		}
	}
	


}













