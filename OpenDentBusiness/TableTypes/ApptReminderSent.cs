using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>When a reminder is sent for an appointment a record of that send is stored here. This is used to prevent re-sends of the same reminder.</summary>
	[Serializable,CrudTable(HasBatchWriteMethods=true)]
	public class ApptReminderSent:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptReminderSentNum;
		///<summary>FK to appointment.AptNum.</summary>
		public long ApptNum;
		///<summary>The Date and time of the original appointment. We need this in case the appointment was moved and needs another reminder sent out.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime ApptDateTime;
		///<summary>Once sent, this was the date and time that the reminder was sent out on.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
		public DateTime DateTimeSent;
		///<summary>This was the TSPrior used to send this reminder. </summary>
		[XmlIgnore]
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong)]
		public TimeSpan TSPrior;
		///<summary>FK to apptreminderrule.ApptReminderRuleNum. Allows us to look up the rules to determine how to send this apptcomm out.</summary>
		public long ApptReminderRuleNum;
		///<summary>Indicates if an SMS message was succesfully sent.</summary>
		public bool IsSmsSent;
		///<summary>Indicates if an email was succesfully sent.</summary>
		public bool IsEmailSent;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("TSPrior",typeof(long))]
		public long TSPriorXml {
			get {
				return TSPrior.Ticks;
			}
			set {
				TSPrior=TimeSpan.FromTicks(value);
			}
		}

		public bool IsValidDuration {
			get {
				return TSPrior.TotalDays>0;
			}
		}
		public bool IsSameDay {
			get {
				return IsValidDuration&&TSPrior.TotalDays<1;
			}
		}

		public bool IsFutureDay {
			get {
				return IsValidDuration&&TSPrior.TotalDays>=1;
			}
		}

		public int DaysInFuture {
			get {
				if(!IsFutureDay) {
					return 0;
				}
				//Rounds 1.1 to 2. So anything greater than exactly n days will be n+1 days.
				return (int)Math.Ceiling(TSPrior.TotalDays);
			}
		}

		public int NumMinutesPrior {
			get {
				if(!IsSameDay) {
					return 0;
				}
				return (int)TSPrior.TotalMinutes;
			}
		}
	}

}
