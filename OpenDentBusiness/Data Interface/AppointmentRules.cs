using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AppointmentRules {
		#region Cache Pattern
		private class AppointmentRuleCache : CacheListAbs<AppointmentRule> {
			protected override List<AppointmentRule> GetCacheFromDb() {
				string command="SELECT * FROM appointmentrule";
				return Crud.AppointmentRuleCrud.SelectMany(command);
			}
			protected override List<AppointmentRule> TableToList(DataTable table) {
				return Crud.AppointmentRuleCrud.TableToList(table);
			}
			protected override AppointmentRule Copy(AppointmentRule appointmentRule) {
				return appointmentRule.Clone();
			}
			protected override DataTable ListToTable(List<AppointmentRule> listAppointmentRules) {
				return Crud.AppointmentRuleCrud.ListToTable(listAppointmentRules,"AppointmentRule");
			}
			protected override void FillCacheIfNeeded() {
				AppointmentRules.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AppointmentRuleCache _appointmentRuleCache=new AppointmentRuleCache();

		public static int GetCount(bool isShort=false) {
			return _appointmentRuleCache.GetCount(isShort);
		}

		public static List<AppointmentRule> GetDeepCopy(bool isShort=false) {
			return _appointmentRuleCache.GetDeepCopy(isShort);
		}

		public static List<AppointmentRule> GetWhere(Predicate<AppointmentRule> match,bool isShort=false) {
			return _appointmentRuleCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_appointmentRuleCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_appointmentRuleCache.FillCacheFromTable(table);
				return table;
			}
			return _appointmentRuleCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(AppointmentRule appointmentRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				appointmentRule.AppointmentRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),appointmentRule);
				return appointmentRule.AppointmentRuleNum;
			}
			return Crud.AppointmentRuleCrud.Insert(appointmentRule);
		}

		///<summary></summary>
		public static void Update(AppointmentRule appointmentRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointmentRule);
				return;
			}
			Crud.AppointmentRuleCrud.Update(appointmentRule);
		}

		///<summary></summary>
		public static void Delete(AppointmentRule rule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rule);
				return;
			}
			string command="DELETE FROM appointmentrule" 
				+" WHERE AppointmentRuleNum = "+POut.Long(rule.AppointmentRuleNum);
 			Db.NonQ(command);
		}

		///<summary>Whenever an appointment is scheduled, the procedures which would be double booked are calculated.  In this method, those procedures are checked to see if the double booking should be blocked.  If double booking is indeed blocked, then a separate function will tell the user which category.</summary>
		public static bool IsBlocked(ArrayList codes){
			//No need to check RemotingRole; no call to db.
			List<AppointmentRule> listRules=GetWhere(x => x.IsEnabled);
			for(int j=0;j<codes.Count;j++){
				for(int i=0;i<listRules.Count;i++) {
					if(String.Compare((string)codes[j],listRules[i].CodeStart) < 0){
						continue;
					}
					if(String.Compare((string)codes[j],listRules[i].CodeEnd) > 0) {
						continue;
					}
					return true;
				}
			}
			return false;
		}

		///<summary>Whenever an appointment is blocked from being double booked, this method will tell the user which category.</summary>
		public static string GetBlockedDescription(ArrayList codes){
			//No need to check RemotingRole; no call to db.
			List<AppointmentRule> listRules=GetDeepCopy();
			for(int j=0;j<codes.Count;j++) {
				for(int i=0;i<listRules.Count;i++) {
					if(!listRules[i].IsEnabled) {
						continue;
					}
					if(String.Compare((string)codes[j],listRules[i].CodeStart) < 0) {
						continue;
					}
					if(String.Compare((string)codes[j],listRules[i].CodeEnd) > 0) {
						continue;
					}
					return listRules[i].RuleDesc;
				}
			}
			return "";
		}
	}
	


}













