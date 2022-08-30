using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class LabPanelm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long LabPanelNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>Both name and address in a single field.  OBR-20</summary>
		public string LabNameAddress;
		///<summary>OBR-13.  Usually blank.  Example: hemolyzed.</summary>
		public string SpecimenCondition;
		///<summary>OBR-15-1.  Usually blank.  Example: LNA&amp;Arterial Catheter&amp;HL70070.</summary>
		public string SpecimenSource;
		///<summary>OBR-4-0, Service performed, id portion, LOINC.  For example, 24331-1.</summary>
		public string ServiceId;
		///<summary>OBR-4-1, Service performed description.  Example, Lipid Panel.</summary>
		public string ServiceName;
		///<summary>FK to medicalorder.MedicalOrderNum.  Used to attach in imported lab panel to a lab order.  Multiple panels may be attached to an order.</summary>
		public long MedicalOrderNum;

		///<summary></summary>
		public LabPanelm Copy() {
			return (LabPanelm)this.MemberwiseClone();
		}



	}
}
