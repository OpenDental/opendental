using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>One lab panel comes back from the lab with multiple lab results.  Multiple panels can come back in one HL7 message.  This table loosely corresponds to the OBR segment.</summary>
	[Serializable]
	public class LabPanel:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LabPanelNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>The entire raw HL7 message.  Can contain other labpanels in addition to this one.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string RawMessage;
		///<summary>Both name and address in a single field.  OBR-20.</summary>
		public string LabNameAddress;
		///<summary>To be used for synch with web server.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>OBR-13.  Usually blank.  Example: hemolyzed.</summary>
		public string SpecimenCondition;
		///<summary>OBR-15.  Usually blank.  Example: LNA&amp;Arterial Catheter&amp;HL70070.</summary>
		public string SpecimenSource;
		///<summary>OBR-4-0, Service performed, id portion, LOINC.  For example, 24331-1.</summary>
		public string ServiceId;
		///<summary>OBR-4-1, Service performed description.  Example, Lipid Panel.</summary>
		public string ServiceName;
		///<summary>FK to medicalorder.MedicalOrderNum.  Used to attach in imported lab panel to a lab order.  Multiple panels may be attached to an order.</summary>
		public long MedicalOrderNum;

		///<summary></summary>
		public LabPanel Copy() {
			return (LabPanel)this.MemberwiseClone();
		}

	}
}