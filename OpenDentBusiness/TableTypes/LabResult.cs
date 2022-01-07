using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Medical labs, not dental labs.  Multiple labresults are attached to a labpanel.  Loosely corresponds to the OBX segment in HL7.</summary>
	[Serializable]
	public class LabResult:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LabResultNum;
		///<summary>FK to labpanel.LabPanelNum.</summary>
		public long LabPanelNum;
		///<summary>OBX-14.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeTest;
		///<summary>OBX-3-1, text portion.</summary>
		public string TestName;
		///<summary>To be used for synch with web server.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>OBX-3-0, id portion, LOINC.  For example, 10676-5.</summary>
		public string TestID;
		///<summary>OBX-5. Value always stored as a string because the type might vary in the future.</summary>
		public string ObsValue;
		///<summary>OBX-6  For example, mL.  Was FK to drugunit.DrugUnitNum, but that would make reliable import problematic, so now it's just text.</summary>
		public string ObsUnits;
		///<summary>OBX-7  For example, &lt;200 or &gt;=40.</summary>
		public string ObsRange;
		///<summary>Enum:LabAbnormalFlag 0-None, 1-Below, 2-Normal, 3-Above.</summary>
		public LabAbnormalFlag AbnormalFlag;

		///<summary></summary>
		public LabResult Copy() {
			return (LabResult)this.MemberwiseClone();
		}


	}

	///<summary></summary>
	public enum LabAbnormalFlag{
		///<summary>0-No value.</summary>
		None,
		///<summary>1-Below normal.</summary>
		Below,
		///<summary>2-Normal.</summary>
		Normal,
		///<summary>3-Above high normal.</summary>
		Above
	}
}