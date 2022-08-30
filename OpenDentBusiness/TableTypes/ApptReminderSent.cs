using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenDentBusiness.WebTypes.AutoComm;

namespace OpenDentBusiness {
	///<summary>When a reminder is sent for an appointment a record of that send is stored here. This is used to prevent re-sends of the same reminder.</summary>
	[Serializable,CrudTable(HasBatchWriteMethods=true)]
	public class ApptReminderSent:AutoCommAppt {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptReminderSentNum;

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
