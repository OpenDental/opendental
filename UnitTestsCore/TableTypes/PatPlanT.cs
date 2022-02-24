using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PatPlanT {
		public static PatPlan CreatePatPlan(byte ordinal,long patNum,long subNum){
			PatPlan patPlan=new PatPlan();
			patPlan.Ordinal=ordinal;
			patPlan.PatNum=patNum;
			patPlan.InsSubNum=subNum;
			PatPlans.Insert(patPlan);
			return patPlan;
		}

		///<summary>Deletes everything from the patplan table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPatPlanTable() {
			string command="DELETE FROM patplan";
			DataCore.NonQ(command);
		}

		///<summary>Call PatPlans.GetOrthoNextClaimDate until DateTime.MinValue is returned which indicates that auto-claim generation has stopped.</summary>
		public static List<DateTime> GenerateAutoOrthoClaimDatesHelper(DateTime dateFirstOrthoProc,OrthoAutoProcFrequency frequency,int monthsTreatment) {
			DateTime dateNextClaim=GenerateFirstAutoOrthoClaimDateHelper(dateFirstOrthoProc,frequency);
			List<DateTime> listClaimDates=new List<DateTime>();
			while(dateNextClaim!=DateTime.MinValue) {
				listClaimDates.Add(dateNextClaim);
				dateNextClaim=PatPlans.GetOrthoNextClaimDate(dateNextClaim,dateFirstOrthoProc,frequency,monthsTreatment);
			}
			return listClaimDates;
		}

		///<summary>Assumes that the first auto-claim will be sent one frequency interval after the initial procedure date.</summary>
		public static DateTime GenerateFirstAutoOrthoClaimDateHelper(DateTime dateFirstOrthoProc,OrthoAutoProcFrequency frequency) {
			DateTime claimDate=dateFirstOrthoProc;
			switch(frequency) {
				case OrthoAutoProcFrequency.Monthly:
					claimDate=dateFirstOrthoProc.AddMonths(1);
					break;
				case OrthoAutoProcFrequency.Quarterly:
					claimDate=dateFirstOrthoProc.AddMonths(3);
					break;
				case OrthoAutoProcFrequency.SemiAnnual:
					claimDate=dateFirstOrthoProc.AddMonths(6);
					break;
				case OrthoAutoProcFrequency.Annual:
					claimDate=dateFirstOrthoProc.AddYears(1);
					break;
			}
			//The system previously would always auto-generate claims on dates after the treatment end date when the frequency was monthly,
			//but this behavior was inconsistent with other frequencies, sometimes generating the last auto-claim the month prior to the treatment end date
			//and sometimes the month after depending on when the first claim was generated. The fix in job B24648 ensures that claims will never be made
			//after the treatment end date, making all frequencies behave in a consistent manner.
			return new DateTime(claimDate.Year,claimDate.Month,1);
		}
	}
}
