using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace OpenDentalGraph.Cache {
	///<summary>Not a real cache, just a place to store HQMessages.</summary>
	public class DashboardCacheHQMessages:DashboardCacheBase<HQMessage> {
		public static List<HQMessage> ListHqMessages;
		protected override List<HQMessage> GetCache(DashboardFilter filter) {
			return ListHqMessages;
		}
	}
	
	public class HQMessage:GraphQuantityOverTime.GraphPointBase {
		public string CountryCode;
		public string MsgType;
		public string MsgStatus;
		public string Customer;
	}
}