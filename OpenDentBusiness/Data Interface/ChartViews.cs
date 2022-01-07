using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChartViews{
		#region CachePattern
		
		private class ChartViewCache : CacheListAbs<ChartView> {
			protected override List<ChartView> GetCacheFromDb() {
				string command="SELECT * FROM chartview ORDER BY ItemOrder";
				return Crud.ChartViewCrud.SelectMany(command);
			}
			protected override List<ChartView> TableToList(DataTable table) {
				return Crud.ChartViewCrud.TableToList(table);
			}
			protected override ChartView Copy(ChartView chartView) {
				return chartView.Copy();
			}
			protected override DataTable ListToTable(List<ChartView> listChartViews) {
				return Crud.ChartViewCrud.ListToTable(listChartViews,"ChartView");
			}
			protected override void FillCacheIfNeeded() {
				ChartViews.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChartViewCache _chartViewCache=new ChartViewCache();

		public static List<ChartView> GetDeepCopy(bool isShort=false) {
			return _chartViewCache.GetDeepCopy(isShort);
		}

		public static ChartView GetFirst(bool isShort=false) {
			return _chartViewCache.GetFirst(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_chartViewCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_chartViewCache.FillCacheFromTable(table);
				return table;
			}
			return _chartViewCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ChartView chartView) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				chartView.ChartViewNum=Meth.GetLong(MethodBase.GetCurrentMethod(),chartView);
				return chartView.ChartViewNum;
			}
			return Crud.ChartViewCrud.Insert(chartView);
		}

		///<summary></summary>
		public static void Update(ChartView chartView) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chartView);
				return;
			}
			Crud.ChartViewCrud.Update(chartView);
		}

		///<summary></summary>
		public static void Delete(long chartViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chartViewNum);
				return;
			}
			string command= "DELETE FROM chartview WHERE ChartViewNum = "+POut.Long(chartViewNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ChartView> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ChartView>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM chartview WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChartViewCrud.SelectMany(command);
		}

		///<summary>Gets one ChartView from the db.</summary>
		public static ChartView GetOne(long chartViewNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ChartView>(MethodBase.GetCurrentMethod(),chartViewNum);
			}
			return Crud.ChartViewCrud.SelectOne(chartViewNum);
		}

		*/
	}
}