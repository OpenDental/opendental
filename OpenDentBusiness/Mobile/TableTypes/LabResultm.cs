using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class LabResultm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long LabResultNum;
		///<summary>FK to labpanel.LabPanelNum.</summary>
		public long LabPanelNum;
		///<summary>OBX-14.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeTest;
		///<summary>OBX-3, text portion.</summary>
		public string TestName;
		///<summary>OBX-3 id portion, LOINC.  For example, 10676-5.</summary>
		public string TestID;
		///<summary>Value always stored as a string because the type can vary.</summary>
		public string ObsValue;
		///<summary>OBX-6  For example, mL.  Was FK to drugunit.DrugUnitNum, but that would make reliable import problematic, so now it's just text.</summary>
		public string ObsUnits;
		///<summary>OBX-7  For example, &lt;200 or &gt;=40.</summary>
		public string ObsRange;
		///<summary>Enum:LabAbnormalFlag 0-No value, 1-Below normal, 2-Normal, 3-Above high normal.</summary>
		public LabAbnormalFlag AbnormalFlag;

		///<summary></summary>
		public LabResultm Copy() {
			return (LabResultm)this.MemberwiseClone();
		}



	}
}
