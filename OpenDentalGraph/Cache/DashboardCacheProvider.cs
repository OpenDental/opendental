using System;
using System.Linq;
using System.Collections.Generic;
using OpenDentBusiness;
using System.Drawing;

namespace OpenDentalGraph.Cache {

	public class DashboardCacheProvider:DashboardCacheBase<Provider> {
		private Dictionary<long,string> _dictProvNames=new Dictionary<long, string>();
		private Dictionary<string,Color> _dictProvColors=new Dictionary<string, Color>();

		protected override List<Provider> GetCache(DashboardFilter filter) {
			List<Provider> list=Providers.GetDeepCopy();
			if(filter.UseProvFilter) {
				list=list.FindAll(x => x.ProvNum==filter.ProvNum);
			}
			_dictProvNames=list.ToDictionary(x => x.ProvNum,x => string.IsNullOrEmpty(x.Abbr) ? x.ProvNum.ToString() : x.Abbr);
			_dictProvColors=list.GroupBy(x => x.Abbr).ToDictionary(x => string.IsNullOrEmpty(x.Key) ? x.First().ProvNum.ToString() : x.Key,x => x.First().ProvColor);
			return list;
		}

		protected override bool AllowQueryDateFilter() {
			return false;
		}

		public string GetProvName(long provNum) {
			string provName;
			if(!_dictProvNames.TryGetValue(provNum,out provName)) {
				if(provNum == 0) {
					return "None";
				}
				return provNum.ToString();
			}
			return provName;
		}

		///<summary>Returns Provider.ProvColor where applicable. Otherwise returns Color.Empty.</summary>
		public Color GetProvColor(string provAbbr) {
			Color provColor;
			if(!_dictProvColors.TryGetValue(provAbbr,out provColor) //if the provAbbr doesn't exist in the dict
				|| provColor==Color.Empty //or the color was empty
				|| provColor==Color.FromArgb(255,255,255) //or if the color was white
				|| provColor==Color.FromArgb(240,240,240)) { //or if the user never specified a color for the prov
				return Color.Empty;				
			}
			return provColor;
		}
	}
}