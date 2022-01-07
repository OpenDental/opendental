using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>Used on employee timecards to make adjustments.  Used to make the end-of-the week OT entries.  Can be used instead of a clock event by admin so that a clock event doesn't have to be created.</summary>
	[Serializable]
	public class TimeAdjust:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TimeAdjustNum;
		///<summary>FK to employee.EmployeeNum</summary>
		public long EmployeeNum;
		///<summary>The date and time that this entry will show on timecard.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TimeEntry;
		///<summary>The number of regular hours to adjust timecard by.  Can be + or -.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan RegHours;
		///<summary>Overtime hours. Usually +.  Automatically combined with a - adj to RegHours.  Another option is clockevent.OTimeHours.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan OTimeHours;
		///<summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>Set to true if this adjustment was automatically made by the system.  When the calc weekly OT tool is run, these types of adjustments are fair game for deletion.  Other adjustments are preserved.</summary>
		public bool IsAuto;
		///<summary>FK to clinic.ClinicNum.  The clinic the TimeAdjust was entered at.</summary>
		public long ClinicNum;
		///<summary>FK to definition.DefNum.  Defaults to 0.  Is set to 0 for general adjustments.
		///When not 0, points to a definition in the TimeCardAdjTypes category.</summary>
		public long PtoDefNum;
		///<summary>PTO Hours.  The number of PTO hours applied to a specific day.  Ignored if PtoDefNum is 0.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan PtoHours;
		///<summary>Defaults to false. True when this TimeAdjust is for unpaid protected leave. Hours from unpaid protected leave adjustments contribute to hours worked, but not to payable hours.</summary>
		public bool IsUnpaidProtectedLeave;
		///<summary>FK to userod.UserNum. The user that created this TimeAdjust.</summary>
		public long SecuUserNumEntry;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("RegHours",typeof(long))]
		public long RegHoursXml {
			get {
				return RegHours.Ticks;
			}
			set {
				RegHours = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("OTimeHours",typeof(long))]
		public long OTimeHoursXml {
			get {
				return OTimeHours.Ticks;
			}
			set {
				OTimeHours = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("PtoHours",typeof(long))]
		public long PtoHoursXml {
			get {
				return PtoHours.Ticks;
			}
			set {
				PtoHours = TimeSpan.FromTicks(value);
			}
		}
		
		///<summary></summary>
		public TimeAdjust Copy() {
			return (TimeAdjust)MemberwiseClone();
		}


		




	}

	
}




