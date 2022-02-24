using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>One clock-in / clock-out pair.  Or, if the pair is a break, then it's an out/in pair. With normal clock in/out pairs, we want to know how long the employee was working.  It's the opposite with breaks. We want to know how long they were not working, so the pair is backwards. This means that a normal clock in is left incomplete when the clock out for break is created. And once both are finished, the regular in/out will surround the break. Breaks cannot be viewed easily on the same grid as regular clock events for this reason. And since breaks do not affect pay, they should not clutter the normal grid.</summary>
	[Serializable]
	public class ClockEvent:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClockEventNum;
		///<summary>FK to employee.EmployeeNum</summary>
		public long EmployeeNum;
		///<summary>The actual time that this entry was entered.  Cannot be 01-01-0001.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime TimeEntered1;
		///<summary>The time to display and to use in all calculations.  Cannot be 01-01-0001.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime TimeDisplayed1;
		///<summary>Enum:TimeClockStatus  Home, Lunch, or Break.  The status really only applies to the clock out.  Except the Break status applies to both out and in.</summary>
		public TimeClockStatus ClockStatus;
		///<summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The user can never edit this, but the program has to be able to edit this when user clocks out.  Can be 01-01-0001 if not clocked out yet.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TimeEntered2;
		///<summary>User can edit. Can be 01-01-0001 if not clocked out yet.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TimeDisplayed2;
		///<summary>This is a manual override for OTimeAuto.  Typically -1 hour (-01:00:00) to indicate no override.  When used as override, allowed values are zero or positive.  This is an alternative to using a TimeAdjust row.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan OTimeHours;
		///<summary>Automatically calculated OT.  Will be zero if none.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan OTimeAuto;
		///<summary>This is a manual override of AdjustAuto.  Ignored unless AdjustIsOverridden set to true.  When used as override, it's typically negative, although zero and positive are also allowed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan Adjust;
		///<summary>Automatically calculated Adjust.  Will be zero if none.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan AdjustAuto;
		///<summary>True if AdjustAuto is overridden by Adjust.</summary>
		public bool AdjustIsOverridden;
		///<summary>This is a manual override for Rate2Auto.  Typically -1 hour (-01:00:00) to indicate no override.  When used as override, allowed values are zero or positive.  This is the portion of the hours worked which are at Rate2, so it's not in addition to the hours worked.  Also used to calculate the Rate2 OT.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan Rate2Hours;
		///<summary>Automatically calculated rate2 pay.  Will be zero if none.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		[XmlIgnore]
		public TimeSpan Rate2Auto;
		///<summary>FK to clinic.ClinicNum.  The clinic the ClockEvent was entered at.</summary>
		public long ClinicNum;

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
		[XmlElement("OTimeAuto",typeof(long))]
		public long OTimeAutoXml {
			get {
				return OTimeAuto.Ticks;
			}
			set {
				OTimeAuto = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("Adjust",typeof(long))]
		public long AdjustXml {
			get {
				return Adjust.Ticks;
			}
			set {
				Adjust = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("AdjustAuto",typeof(long))]
		public long AdjustAutoXml {
			get {
				return AdjustAuto.Ticks;
			}
			set {
				AdjustAuto = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("Rate2Hours",typeof(long))]
		public long DiffHoursXml {
			get {
				return Rate2Hours.Ticks;
			}
			set {
				Rate2Hours = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("Rate2Auto",typeof(long))]
		public long DiffAutoXml {
			get {
				return Rate2Auto.Ticks;
			}
			set {
				Rate2Auto = TimeSpan.FromTicks(value);
			}
		}

		public ClockEvent() {
			OTimeHours=TimeSpan.FromHours(-1);
			Rate2Hours=TimeSpan.FromHours(-1);
		}

		///<summary></summary>
		public ClockEvent Copy() {
			return (ClockEvent)MemberwiseClone();
		}



	}

	
}




