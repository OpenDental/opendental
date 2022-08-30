using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Tracks which clinics have access to eRx based on ClinicDescr.  Synchronized with HQ.</summary>
	[Serializable,CrudTable(IsSynchable=true)]
	public class ClinicErx:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClinicErxNum;
		///<summary>FK to patient.PatNum.  Holder of registration key only for HQ record, in customer record this will be 0.</summary>
		public long PatNum;
		///<summary>Description of a clinic from the clinic table.  Only used by OD HQ.  For customer records, use ClinicNum.</summary>
		public string ClinicDesc;
		///<summary>FK to clinic.ClinicNum.  Is the clinic that is used for accessing eRx.</summary>
		public long ClinicNum;
		///<summary>Enum:ErxStatus Set to true if the clinic with the given ClinicName has access to eRx.</summary>
		public ErxStatus EnabledStatus;
		///<summary>Clinic identifier used by the erx option.  Only used by OD HQ.</summary>
		public string ClinicId;
		///<summary>Unique key used by the erx option.  Only used by OD HQ.</summary>
		public string ClinicKey;
		///<summary>Only used by OD HQ.</summary>
		public string AccountId;
		///<summary>FK to registrationkey.RegistrationKeyNum.  HQ only, links to the registration key used to make this clinicerx row.</summary>
		public long RegistrationKeyNum;

		///<summary></summary>
		public ClinicErx Copy() {
			return (ClinicErx)this.MemberwiseClone();
		}
	}
}
