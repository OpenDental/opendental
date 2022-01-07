using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	public class FeeSchedGroup:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FeeSchedGroupNum;
		///<summary></summary>
		public string Description;
		///<summary>FK to FeeSched.FeeSchedNum.</summary>
		public long FeeSchedNum;
		///<summary>Comma delimited list of Clinic.ClinicNums.</summary>
		public string ClinicNums;

		///<summary></summary>
		public FeeSchedGroup Copy() {
			return (FeeSchedGroup)this.MemberwiseClone();
		}

		///<summary>The list of Clinic.ClinicNums filled from the ClinicNums comma delimited string.  Does not filter out restricted clinics.</summary>
		[XmlIgnore, JsonIgnore]
		public List<long> ListClinicNumsAll {
			get {
				if(this.ClinicNums=="") {
					return new List<long>();
				}
				return new List<long>(this.ClinicNums.Split(',').Select(long.Parse).Distinct().ToList());
			}
			set {
				ClinicNums=string.Join(",",value);
			}
		}

		///<summary>The list of Clinic.ClinicNums filled from the ClinicNums comma delimited string that exist in the given list of clinics.</summary>
		public List<long> GetListClinicNumsFiltered(List<Clinic> listClinicsFiltered) {
			return new List<long>(this.ClinicNums.Split(',').Select(long.Parse).Distinct().ToList())
				.FindAll(x => listClinicsFiltered.Select(y => y.ClinicNum).Contains(x));
		}
	}
}