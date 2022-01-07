using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used to view employee timecards.  Timecard entries are not linked to a pay period.  Instead, payperiods are setup, and the user can only view specific pay periods.  So it feels like they are linked, but it's date based.</summary>
	[Serializable]
	public class PayPeriod:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayPeriodNum;
		///<summary>The first day of the payperiod</summary>
		public DateTime DateStart;
		///<summary>The last day of the payperiod.  Inclusive, ignoring time of day.</summary>
		public DateTime DateStop;
		///<summary>The date that paychecks will be dated.  A few days after the dateStop.  Optional.</summary>
		public DateTime DatePaycheck;

		///<summary></summary>
		public PayPeriod Copy() {
			return (PayPeriod)this.MemberwiseClone();
		}

		public PayPeriod() {
			TagOD=Guid.NewGuid().ToString();//Used to identify PayPeriods that have not been entered into the database yet.
		}

		public bool IsSame(PayPeriod otherPayPeriod) {
			if(PayPeriodNum!=0 && PayPeriodNum==otherPayPeriod.PayPeriodNum) {
				return true;
			}
			if(TagOD==otherPayPeriod.TagOD) {
				return true;
			}
			return false;
		}
	}
	
	public enum PayPeriodInterval {
		///<summary>0</summary>
		Weekly,
		///<summary>1 Pay period every 14 days</summary>
		BiWeekly,
		///<summary>2</summary>
		Monthly,
		//<summary>3 Pay period twice a month on specified days</summary>
		SemiMonthly
	}
	
}




