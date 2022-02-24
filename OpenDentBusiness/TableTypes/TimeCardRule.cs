using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;


namespace OpenDentBusiness{
	///<summary>A rule for automation of timecard overtime.  Can apply to one employee or all.</summary>
	[Serializable]
	[CrudTableAttribute(HasBatchWriteMethods=true)]
	public class TimeCardRule:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TimeCardRuleNum;
		///<summary>FK to employee.EmployeeNum. If zero, then this rule applies to all employees.</summary>
		public long EmployeeNum;
		///<summary>Typical example is 8:00.  In California, any work after the first 8 hours is overtime.</summary>
		[XmlIgnore]
		public TimeSpan OverHoursPerDay;
		///<summary>Typical example is 16:00 to indicate that all time worked after 4pm for specific employees is at differential rate.</summary>
		[XmlIgnore]
		public TimeSpan AfterTimeOfDay;
		///<summary>Typical example is 6:00 to indicate that all time worked before 6am for specific employees is at differential rate.</summary>
		[XmlIgnore]
		public TimeSpan BeforeTimeOfDay;
		///<summary>Indicates if the employee should have overtime calculated for their hours worked in a pay period.</summary>
		public bool IsOvertimeExempt;
		///<summary>When set this is the earliest an employee can clock in. Otherwise minimum dateTime represents not set.</summary>
		[XmlIgnore]
		public TimeSpan MinClockInTime;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("OverHoursPerDay",typeof(long))]
		public long OverHoursPerDayXml {
			get {
				return OverHoursPerDay.Ticks;
			}
			set {
				OverHoursPerDay = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("AfterTimeOfDay",typeof(long))]
		public long AfterTimeOfDayXml {
			get {
				return AfterTimeOfDay.Ticks;
			}
			set {
				AfterTimeOfDay = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("BeforeTimeOfDay",typeof(long))]
		public long BeforeTimeOfDayXml {
			get {
				return BeforeTimeOfDay.Ticks;
			}
			set {
				BeforeTimeOfDay = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("MinClockInTime",typeof(long))]
		public long MinClockInTimeXml {
			get {
				return MinClockInTime.Ticks;
			}
			set {
				MinClockInTime=TimeSpan.FromTicks(value);
			}
		}

		///<summary></summary>
		public TimeCardRule Clone() {
			return (TimeCardRule)this.MemberwiseClone();
		}

	}
}