using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CreditCardT {
		public static CreditCard CreateCard(long patNum,double chargeAmt,DateTime dateStart,long payPlanNum,string authorizedProcs="",
			ChargeFrequencyType frequencyType=ChargeFrequencyType.FixedDayOfMonth,DayOfWeekFrequency dayOfWeekFrequency=DayOfWeekFrequency.Every,
			DayOfWeek dayOfWeek=DayOfWeek.Friday,string daysOfMonth="",bool canChargeWhenZeroBal=false)
		{
			CreditCard card=new CreditCard();
			card.PatNum=patNum;
			card.ChargeAmt=chargeAmt;
			card.DateStart=dateStart;
			card.PayPlanNum=payPlanNum;
			card.CCExpiration=DateTime.Today.AddYears(3);
			card.CCNumberMasked="XXXXXXXXXXXXX1234";
			card.Procedures=authorizedProcs;
			if(frequencyType==ChargeFrequencyType.FixedDayOfMonth) {
				card.ChargeFrequency=POut.Int((int)ChargeFrequencyType.FixedDayOfMonth)
					+"|"+(daysOfMonth=="" ? dateStart.Day.ToString() : daysOfMonth);
			}
			else if(frequencyType==ChargeFrequencyType.FixedWeekDay) {
				card.ChargeFrequency=POut.Int((int)ChargeFrequencyType.FixedWeekDay)
					+"|"+POut.Int((int)dayOfWeekFrequency)+"|"+POut.Int((int)dayOfWeek);
			}
			card.CanChargeWhenNoBal=canChargeWhenZeroBal;
			CreditCards.Insert(card);
			return card;
		}

		///<summary>Deletes everything from the creditcard table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearCreditCardTable() {
			string command="DELETE FROM creditcard WHERE CreditCardNum > 0";
			DataCore.NonQ(command);
		}
	}
}
