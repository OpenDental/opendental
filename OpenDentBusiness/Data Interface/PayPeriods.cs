using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayPeriods {
		#region CachePattern

		private class PayPeriodCache : CacheListAbs<PayPeriod> {
			protected override List<PayPeriod> GetCacheFromDb() {
				string command="SELECT * from payperiod ORDER BY DateStart";
				return Crud.PayPeriodCrud.SelectMany(command);
			}
			protected override List<PayPeriod> TableToList(DataTable table) {
				return Crud.PayPeriodCrud.TableToList(table);
			}
			protected override PayPeriod Copy(PayPeriod payPeriod) {
				return payPeriod.Copy();
			}
			protected override DataTable ListToTable(List<PayPeriod> listPayPeriods) {
				return Crud.PayPeriodCrud.ListToTable(listPayPeriods,"PayPeriod");
			}
			protected override void FillCacheIfNeeded() {
				PayPeriods.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PayPeriodCache _payPeriodCache=new PayPeriodCache();

		public static List<PayPeriod> GetDeepCopy(bool isShort=false) {
			return _payPeriodCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _payPeriodCache.GetCount(isShort);
		}

		public static int GetFindIndex(Predicate<PayPeriod> match,bool isShort=false) {
			return _payPeriodCache.GetFindIndex(match,isShort);
		}

		public static PayPeriod GetFirstOrDefault(Func<PayPeriod,bool> match,bool isShort=false) {
			return _payPeriodCache.GetFirstOrDefault(match,isShort);
		}

		public static PayPeriod GetLast(bool isShort=false) {
			return _payPeriodCache.GetLast(isShort);
		}

		public static List<PayPeriod> GetWhere(Predicate<PayPeriod> match,bool isShort = false) {
			return _payPeriodCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_payPeriodCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_payPeriodCache.FillCacheFromTable(table);
				return table;
			}
			return _payPeriodCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(PayPeriod pp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pp.PayPeriodNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pp);
				return pp.PayPeriodNum;
			}
			return Crud.PayPeriodCrud.Insert(pp);
		}

		///<summary></summary>
		public static void Update(PayPeriod pp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pp);
				return;
			}
			Crud.PayPeriodCrud.Update(pp);
		}

		///<summary></summary>
		public static void Delete(PayPeriod pp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pp);
				return;
			}
			string command= "DELETE FROM payperiod WHERE PayPeriodNum = "+POut.Long(pp.PayPeriodNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static int GetForDate(DateTime date){
			//No need to check RemotingRole; no call to db.
			PayPeriod payPeriod=GetFirstOrDefault(x => date.Date >= x.DateStart.Date && date.Date <= x.DateStop.Date);
			if(payPeriod==null) {
				//Get the last most recent pay period. 
				List<PayPeriod> listPayPeriods=GetWhere(x => date.Date >= x.DateStart.Date && date.Date > x.DateStop.Date);
				if(listPayPeriods.Count>0) {
					payPeriod=listPayPeriods.Aggregate((x1,x2) => x1.DateStop>x2.DateStop ? x1 : x2);
				}
			}
			if(payPeriod==null) {
				//Get the next most recent pay period.
				List<PayPeriod> listPayPeriods=GetWhere(x => date.Date < x.DateStart.Date && date.Date <= x.DateStop.Date);
				if(listPayPeriods.Count>0) {
					payPeriod=listPayPeriods.Aggregate((x1,x2) => x1.DateStart<x2.DateStart ? x1 : x2);
				}
			}
			if(payPeriod==null) {
				return GetCount()-1;
			}
			int index=GetFindIndex(x => x.PayPeriodNum==payPeriod.PayPeriodNum);
			return (index > -1 ? index : GetCount()-1);
		}

		///<summary></summary>
		public static bool HasPayPeriodForDate(DateTime date) {
			//No need to check RemotingRole; no call to db.
			PayPeriod payPeriod=GetFirstOrDefault(x => date.Date >= x.DateStart.Date && date.Date <= x.DateStop.Date);
			return (payPeriod!=null);
		}

		/// <summary>Pass in the TimeAdjust.EmployeeNum or ClockEvent.EmployeeNum. If true, the user cannot edit the TimeAdjust/ClockEvent for that date.</summary>
		public static bool CannotEditPayPeriodOfDate(DateTime date,long employeeNum) {
			return Security.CurUser!=null
				&& Security.CurUser.EmployeeNum==employeeNum
				&& PrefC.GetBool(PrefName.TimecardSecurityEnabled)
				&& PrefC.GetBool(PrefName.TimecardUsersCantEditPastPayPeriods)
				&& (!HasPayPeriodForDate(date) || GetForDate(date)!=GetForDate(DateTime.Today));
		}

		///<summary>Returns the most recent payperiod object or null if none were found.</summary>
		public static PayPeriod GetMostRecent() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PayPeriod>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM payperiod WHERE DateStop=(SELECT MAX(DateStop) FROM payperiod)";
			return Crud.PayPeriodCrud.SelectOne(command);
		}


		///<summary>Determines whether there is any overlap in dates between the two passed-in list of pay periods.
		///Same-date overlaps are not allowed (eg you cannot have a pay period that ends the same day as the next one starts).</summary>
		public static bool AreAnyOverlapping(List<PayPeriod> listFirst,List<PayPeriod> listSecond) {
			//no remoting role check; no call to db.
			foreach(PayPeriod payPeriodFirst in listFirst) {
				if(listSecond.Where(payPeriodSecond => !payPeriodSecond.IsSame(payPeriodFirst)
					&& ((payPeriodFirst.DateStop >= payPeriodSecond.DateStart && payPeriodFirst.DateStop <= payPeriodSecond.DateStop) //the bottom of first overlaps
					|| (payPeriodFirst.DateStart >= payPeriodSecond.DateStart && payPeriodFirst.DateStart <= payPeriodSecond.DateStop))) //the top of first overlaps
				.Count() > 0) 
				{
					return true;
				}
			}
			return false;
		}
	}

}




