using System;
using System.Linq;
using System.Collections.Generic;
using OpenDentBusiness;
using System.Drawing;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheClinic:DashboardCacheBase<Clinic> {
		private Dictionary<long,string> _dictClinicNames=new Dictionary<long, string>();
		protected override List<Clinic> GetCache(DashboardFilter filter) {
			List<Clinic> list=Clinics.GetDeepCopy();
			_dictClinicNames=list.ToDictionary(x => x.ClinicNum,x => string.IsNullOrEmpty(x.Description) ? x.ClinicNum.ToString() : x.Description);
			return list;
		}

		public string GetClinicName(long clinicNum) {
			string clinicName;
			if(!_dictClinicNames.TryGetValue(clinicNum,out clinicName)) {
				if(clinicNum == 0) {
					return "Unassigned";
				}
				return clinicNum.ToString();
			}
			return clinicName;
		}
	}
}