using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheAR:DashboardCacheBase<DashboardAR> {
		protected override List<DashboardAR> GetCache(DashboardFilter filter) {
			DateTime firstOfMonth = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			filter.DateTo=filter.DateTo < firstOfMonth ? filter.DateTo : firstOfMonth;
			filter.DateFrom=new DateTime(filter.DateFrom.Year,filter.DateFrom.Month,1);
			return DashboardQueries.GetAR(filter.DateFrom,filter.DateTo,DashboardARs.Refresh(filter.DateFrom));
		}

		protected override bool AllowQueryDateFilter() {
			return false;
		}
	}
}