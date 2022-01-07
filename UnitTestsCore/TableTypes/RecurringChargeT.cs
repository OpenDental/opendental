using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RecurringChargeT {

		public static RecurringCharge CreateRecurringCharge(long patNum,RecurringChargeStatus status,double chargeAmt,long creditCardNum,
			DateDefaultToday dateCharge=default(DateDefaultToday)) 
		{
			RecurringCharge charge=new RecurringCharge {
				ChargeAmt=chargeAmt,
				ChargeStatus=status,
				CreditCardNum=creditCardNum,
				PatNum=patNum,
				DateTimeCharge=dateCharge.Date,
			};
			RecurringCharges.Insert(charge);
			return charge;
		}

		///<summary>Deletes everything from the recurringcharge table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRecurringChargeTable() {
			string command="DELETE FROM recurringcharge WHERE RecurringChargeNum > 0";
			DataCore.NonQ(command);
		}
	}
}
