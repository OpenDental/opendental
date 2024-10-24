using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class ClockEventsTests:TestBase {

		[TestInitialize]
		public void SetupTest() {
			EmployeeT.ClearEmployeeTable();
			PayPeriodT.ClearPayPeriodTable();
			TimeCardRuleT.ClearTimeCardRuleTable();
		}

		[TestMethod]
		public void ClockEvents_WeekStartHelper_SummaryScenario() {
			//This unit test asserts the scenario provided in the summary works as intended.
			//Pass in a start date of "11-01-2013"(Friday) and a stop date of "11-14-2013"(Thursday) which spans 3 weeks.
			//When the workweek starts on Sunday, the method yields "10-27-2013"(Sunday), "11-03-2013"(Sunday), and "11-10-2013"(Sunday).
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			DateTime dateTimeStart=new DateTime(2013,11,01);
			DateTime dateTimeStop=new DateTime(2013,11,14);
			List<DateTime> listDateTimesActual=ClockEvents.WeekStartHelper(dateTimeStart,dateTimeStop);
			Assert.AreEqual(3,listDateTimesActual.Count);
			Assert.IsTrue(listDateTimesActual.Contains(new DateTime(2013,10,27)));
			Assert.IsTrue(listDateTimesActual.Contains(new DateTime(2013,11,03)));
			Assert.IsTrue(listDateTimesActual.Contains(new DateTime(2013,11,10)));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_SingleWeekRatesOneTwoThreeWithOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"Overtime hours should be paid at rates that depend on the ratio of hours worked within each rate.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>40</td>
    <td>5</td>
    <td>16</td>
    <td>21</td>
    <td>61</td>
    <td>26.23</td>
    <td>13.77</td>
    <td>3.28</td>
    <td>1.72</td>
    <td>10.49</td>
    <td>5.51</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_SingleWeekRatesOneTwoThreeWithOT() {
			/*********************************************************************************************************
			Work a full 9-5 week where Tue (11 hours) and Wed (10 hours) accumulated 5 hours of Rate 2 (worked after 17:00).
			Also, work 8 hours each day of the weekend (Sat and Sun) yielding 16 hours of Rate 3 (weekend time).
			40 = Rate 1
			 5 = Rate 2
			16 = Rate 3
			61 total hours, 21 considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("SingleWeek");
			//Create a pay period for 09/19/2024 - 10/03/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,19);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that they can utilize all of the rates (rate 2 starts after 17:00 and weekends count as rate 3).
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,afterTimeOfDay:TimeSpan.FromHours(17),hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Work a full 9-5 week where Tue (11 hours) and Wed (10 hours) accumulate 5 hours Rate 2.
			DateTime dateTimeSunday=new DateTime(2024,09,22);
			DateTime dateTimeMonday=new DateTime(2024,09,23);
			DateTime dateTimeTuesday=new DateTime(2024,09,24);
			DateTime dateTimeWednesday=new DateTime(2024,09,25);
			DateTime dateTimeThursday=new DateTime(2024,09,26);
			DateTime dateTimeFriday=new DateTime(2024,09,27);
			DateTime dateTimeSaturday=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(9),dateTimeSunday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(9),dateTimeMonday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(9),dateTimeTuesday.AddHours(20));//11 hours
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday.AddHours(9),dateTimeWednesday.AddHours(19));//10 hours
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday.AddHours(9),dateTimeThursday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday.AddHours(9),dateTimeFriday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(9),dateTimeSaturday.AddHours(17));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			/*********************************************************************************************************
			The following equations were taken from our online manual page at the time this unit test was created:
			Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
			  ^(1 - (5 + 16) / 61) * 40 = 26.229508196721311475409836064
			Rate 2 hours = (Rate 2/Total hours) * Regular hours
			  ^(5 / 61) * 40 = 3.2786885245901639344262295080
			Rate 3 hours = (Rate 3/Total hours) * Regular hours
			  ^(16 / 61) * 40 = 10.491803278688524590163934428
			Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
			  ^(1 - (5 + 16) / 61) * 21 = 13.770491803278688524590163934
			Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
			  ^(1 - (40 + 16) / 61) * 21 = 1.7213114754098360655737704917
			Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
			  ^61 - (26.229508196721311475409836064 + 3.2786885245901639344262295080 + 10.491803278688524590163934428 + 13.770491803278688524590163934 + 1.7213114754098360655737704917) = 5.508196721311475409836065574
			*********************************************************************************************************/
			RateRatioOT rateRatioOTExpected=new RateRatioOT(40,5,16);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksBothRatesOneTwoThreeWithOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"Overtime hours should be paid at rates that depend on the ratio of hours worked within each rate. Each week within a pay period should calculate overtime ratios independently.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>40</td>
    <td>5</td>
    <td>16</td>
    <td>21</td>
    <td>61</td>
    <td>26.23</td>
    <td>13.77</td>
    <td>3.28</td>
    <td>1.72</td>
    <td>10.49</td>
    <td>5.51</td>
  </tr>
  <tr>
    <td>16</td>
    <td>25</td>
    <td>20</td>
    <td>21</td>
    <td>61</td>
    <td>10.49</td>
    <td>5.51</td>
    <td>16.39</td>
    <td>8.61</td>
    <td>13.11</td>
    <td>6.89</td>
  </tr>
  <tr>
    <td colspan=""11""><b>Pay Period Totals</b></td>
  </tr>
  <tr>
    <td>56</td>
    <td>30</td>
    <td>36</td>
    <td>42</td>
    <td>122</td>
    <td>36.72</td>
    <td>19.28</td>
    <td>19.67</td>
    <td>10.33</td>
    <td>23.61</td>
    <td>12.39</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksBothRatesOneTwoThreeWithOT() {
			/*********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			Work a full 9-5 week where Tue (11 hours) and Wed (10 hours) accumulated 5 hours of Rate 2 (worked after 17:00).
			Also, work 8 hours each day of the weekend (Sat and Sun) yielding 16 hours of Rate 3 (weekend time).
			40 = Rate 1
			 5 = Rate 2
			16 = Rate 3
			61 total hours, 21 considered OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			Work strange hours on a different week that has far more hours during the other rates.
			Only work Sun - Thur.
			Have Mon - Thur work 4 hours before 17:00 and the rest of each shift fall afterwards to easily rack up Rate 2 hours.
			Also, work an outlandish shift on Sunday (20 hours) for the weekend shift (Rate 3).
			Sunday    -  3:00 - 23:00 (20 hours @ Rate 3)
			Monday    - 13:00 - 23:30 (4 hours @ Rate 1, 6.5 hours @ Rate 2)
			Tuesday   - 13:00 - 23:00 (4 hours @ Rate 1, 6 hours @ Rate 2)
			Wednesday - 13:00 - 23:00 (4 hours @ Rate 1, 6 hours @ Rate 2)
			Thursday  - 13:00 - 23:30 (4 hours @ Rate 1, 6.5 hours @ Rate 2)
			16 = Rate 1
			25 = Rate 2
			20 = Rate 3
			61 total hours, 21 considered OT (rule for OT is anything over 40 in a week)
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeks");
			//Create a pay period for 09/19/2024 - 10/03/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,19);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that they can utilize all of the rates (rate 2 starts after 17:00 and weekends count as rate 3).
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,afterTimeOfDay:TimeSpan.FromHours(17),hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a full 9-5 week where Tue (11 hours) and Wed (10 hours) accumulate 5 hours Rate 2.
			DateTime dateTimeSunday=new DateTime(2024,09,22);
			DateTime dateTimeMonday=new DateTime(2024,09,23);
			DateTime dateTimeTuesday=new DateTime(2024,09,24);
			DateTime dateTimeWednesday=new DateTime(2024,09,25);
			DateTime dateTimeThursday=new DateTime(2024,09,26);
			DateTime dateTimeFriday=new DateTime(2024,09,27);
			DateTime dateTimeSaturday=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(9),dateTimeSunday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(9),dateTimeMonday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(9),dateTimeTuesday.AddHours(20));//11 hours
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday.AddHours(9),dateTimeWednesday.AddHours(19));//10 hours
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday.AddHours(9),dateTimeThursday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday.AddHours(9),dateTimeFriday.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(9),dateTimeSaturday.AddHours(17));
			//Week 2 - Work strange hours on a different week that has far more hours during the other rates.
			DateTime dateTimeSunday2=new DateTime(2024,09,29);
			DateTime dateTimeMonday2=new DateTime(2024,09,30);
			DateTime dateTimeTuesday2=new DateTime(2024,10,1);
			DateTime dateTimeWednesday2=new DateTime(2024,10,2);
			DateTime dateTimeThursday2=new DateTime(2024,10,3);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday2.AddHours(3),dateTimeSunday2.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(13),dateTimeMonday2.AddHours(23).AddMinutes(30));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(13),dateTimeTuesday2.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(13),dateTimeWednesday2.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(13),dateTimeThursday2.AddHours(23).AddMinutes(30));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			/*********************************************************************************************************
			The following equations were taken from our online manual page at the time this unit test was created:
			---------------------------------------------WEEK 1---------------------------------------------
			Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
			  ^(1 - (5 + 16) / 61) * 40 = 26.229508196721311475409836064
			Rate 2 hours = (Rate 2/Total hours) * Regular hours
			  ^(5 / 61) * 40 = 3.2786885245901639344262295080
			Rate 3 hours = (Rate 3/Total hours) * Regular hours
			  ^(16 / 61) * 40 = 10.491803278688524590163934428
			Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
			  ^(1 - (5 + 16) / 61) * 21 = 13.770491803278688524590163934
			Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
			  ^(1 - (40 + 16) / 61) * 21 = 1.7213114754098360655737704917
			Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
			  ^61 - (26.229508196721311475409836064 + 3.2786885245901639344262295080 + 10.491803278688524590163934428 + 13.770491803278688524590163934 + 1.7213114754098360655737704917) = 5.508196721311475409836065574
			---------------------------------------------WEEK 2---------------------------------------------
			Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
			  ^(1 - (25 + 20) / 61) * 40 = 10.491803278688524590163934428
			Rate 2 hours = (Rate 2/Total hours) * Regular hours
			  ^(25 / 61) * 40 = 16.393442622950819672131147540
			Rate 3 hours = (Rate 3/Total hours) * Regular hours
			  ^(20 / 61) * 40 = 13.114754098360655737704918032
			Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
			  ^(1 - (25 + 20) / 61) * 21 = 5.5081967213114754098360655747
			Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
			  ^(1 - (16 + 20) / 61) * 21 = 8.606557377049180327868852458
			Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
			  ^61 - (10.491803278688524590163934428 + 16.393442622950819672131147540 + 13.114754098360655737704918032 + 5.5081967213114754098360655747 + 8.606557377049180327868852458) = 6.885245901639344262295081967
			---------------------------------------------Summed---------------------------------------------
			Total Hours:     61 + 61 = 122
			Rate 1 Hours:    26.229508196721311475409836064 + 10.491803278688524590163934428 = 36.721311475409836065573770492
			Rate 1 OT Hours: 13.770491803278688524590163934 + 5.5081967213114754098360655747 = 19.278688524590163934426229509
			Rate 2 Hours:    3.2786885245901639344262295080 + 16.393442622950819672131147540 = 19.672131147540983606557377048
			Rate 2 OT Hours: 1.7213114754098360655737704917 + 8.606557377049180327868852458  = 10.327868852459016393442622950
			Rate 3 Hours:    10.491803278688524590163934428 + 13.114754098360655737704918032 = 23.606557377049180327868852460
			Rate 3 OT Hours: 5.508196721311475409836065574  + 6.885245901639344262295081967  = 12.393442622950819672131147541
			*********************************************************************************************************/
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(40,5,16);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(16,25,20);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeNoOTSecondRatesOneWithOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"A pay period should only pay overtime rates for hours worked during each week. The hours worked at different rates in other weeks should be ignored when calculating overtime ratios.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>1</td>
    <td>10</td>
    <td>29</td>
    <td>0</td>
    <td>40</td>
    <td>1</td>
    <td>0</td>
    <td>10</td>
    <td>0</td>
    <td>29</td>
    <td>0</td>
  </tr>
  <tr>
    <td>80</td>
    <td>0</td>
    <td>0</td>
    <td>40</td>
    <td>80</td>
    <td>40</td>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
  </tr>
  <tr>
    <td colspan=""11""><b>Pay Period Totals</b></td>
  </tr>
  <tr>
    <td>81</td>
    <td>10</td>
    <td>29</td>
    <td>40</td>
    <td>120</td>
    <td>41</td>
    <td>40</td>
    <td>10</td>
    <td>0</td>
    <td>29</td>
    <td>0</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeNoOTSecondRatesOneWithOT() {
			/*********************************************************************************************************
			Work a week where some hours are worked at each rate having the most in Rate 3.
			Also, work another week where 80 hours are all in Rate 1.
			**********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			 1 = Rate 1
			10 = Rate 2
			29 = Rate 3
			40 total hours, no OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			80 = Rate 1
			 0 = Rate 2
			 0 = Rate 3
			80 total hours, 40 considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeksRate123noOTRate1OT");
			//Create a pay period for 09/15/2024 - 09/29/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,15);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that they can utilize all of the rates (rate 2 starts after 17:00 and weekends count as rate 3).
			TimeCardRuleT.CreateTimeCardRule(employeeNum: employee.EmployeeNum,afterTimeOfDay: TimeSpan.FromHours(17),hasWeekendRate3: true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a week which has hours at each rate that is primarily on the weekend (Rate 3).
			DateTime dateTimeSunday=new DateTime(2024,09,15);
			DateTime dateTimeMonday=new DateTime(2024,09,16);
			DateTime dateTimeTuesday=new DateTime(2024,09,17);
			DateTime dateTimeWednesday=new DateTime(2024,09,18);
			DateTime dateTimeThursday=new DateTime(2024,09,19);
			DateTime dateTimeFriday=new DateTime(2024,09,20);
			DateTime dateTimeSaturday=new DateTime(2024,09,21);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(5),dateTimeSunday.AddHours(20));//      15 hours @ Rate 3
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(9),dateTimeMonday.AddHours(10));//       1 hours @ Rate 1
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(17),dateTimeTuesday.AddHours(22));//    5 hours @ Rate 2
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday.AddHours(17),dateTimeWednesday.AddHours(22));//5 hours @ Rate 2
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(5),dateTimeSaturday.AddHours(19));//  14 hours @ Rate 3
			//Week 2 - Work like a dog (80 hours) but have all of the hours fall before 17:00 (Rate 1).
			DateTime dateTimeSunday2=new DateTime(2024,09,22);
			DateTime dateTimeMonday2=new DateTime(2024,09,23);
			DateTime dateTimeTuesday2=new DateTime(2024,09,24);
			DateTime dateTimeWednesday2=new DateTime(2024,09,25);
			DateTime dateTimeThursday2=new DateTime(2024,09,26);
			DateTime dateTimeFriday2=new DateTime(2024,09,27);
			DateTime dateTimeSaturday2=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(1),dateTimeMonday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(1),dateTimeTuesday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(1),dateTimeWednesday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(1),dateTimeThursday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday2.AddHours(1),dateTimeFriday2.AddHours(17));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			/*********************************************************************************************************
			The following equations were taken from our online manual page at the time this unit test was created:
			---------------------------------------------WEEK 1---------------------------------------------
			Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
				^(1 - (10 + 29) / 40) * 40 = 1
			Rate 2 hours = (Rate 2/Total hours) * Regular hours
				^(10 / 40) * 40 = 10
			Rate 3 hours = (Rate 3/Total hours) * Regular hours
				^(29 / 40) * 40 = 29
			Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
				^(1 - (10 + 29) / 40) * 40 * 0 = 0
			Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
				^(1 - (1 + 29) / 40) * 0 = 0
			Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
				^40 - (1 + 10 + 29 + 0 + 0) = 0
			---------------------------------------------WEEK 2---------------------------------------------
			Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
				^(1 - (0 + 0) / 80) * 40 = 40
			Rate 2 hours = (Rate 2/Total hours) * Regular hours
				^(0 / 80) * 40 = 0
			Rate 3 hours = (Rate 3/Total hours) * Regular hours
				^(0 / 80) * 40 = 0
			Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
				^(1 - (0 + 0) / 80) * 40 = 40
			Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
				^(1 - (80 + 0) / 80) * 40 = 0
			Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
				^80 - (40 + 0 + 0 + 40 + 0) = 0
			---------------------------------------------Results---------------------------------------------
			Total Hours:     40 + 80 = 120
			Rate 1 Hours:     1 + 40 =  41
			Rate 1 OT Hours:  0 + 40 =  40
			Rate 2 Hours:    10 +  0 =  10
			Rate 2 OT Hours:  0 +  0 =   0
			Rate 3 Hours:    29 +  0 =  29
			Rate 3 OT Hours:  0 +  0 =   0
			*********************************************************************************************************/
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(1,10,29);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(80,0,0);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesThreeWithOTSecondRatesOneNoOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"A pay period should only pay overtime rates for hours worked during each week. The hours worked at different rates in other weeks should be ignored when calculating overtime ratios.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>0</td>
    <td>0</td>
    <td>46</td>
    <td>6</td>
    <td>46</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>40</td>
    <td>6</td>
  </tr>
  <tr>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>40</td>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
  </tr>
    <tr>
      <td colspan=""11""><b>Pay Period Totals</b></td>
    </tr>
  <tr>
    <td>40</td>
    <td>0</td>
    <td>46</td>
    <td>6</td>
    <td>86</td>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>40</td>
    <td>6</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesThreeWithOTSecondRatesOneNoOT() {
			/*********************************************************************************************************
			Work a week where all hours are worked in Rate 3 follwed by a normal work week.
			**********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			 0 = Rate 1
			 0 = Rate 2
			46 = Rate 3
			 6 hours of OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			40 = Rate 1
			 0 = Rate 2
			 0 = Rate 3
			 0 hours considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeksRate3OTRate1noOT");
			//Create a pay period for 09/15/2024 - 09/29/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,15);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that there is no such thing as rate 2.
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a week which has hours at each rate that is primarily on the weekend (Rate 3).
			DateTime dateTimeSunday=new DateTime(2024,09,15);
			DateTime dateTimeMonday=new DateTime(2024,09,16);
			DateTime dateTimeTuesday=new DateTime(2024,09,17);
			DateTime dateTimeWednesday=new DateTime(2024,09,18);
			DateTime dateTimeThursday=new DateTime(2024,09,19);
			DateTime dateTimeFriday=new DateTime(2024,09,20);
			DateTime dateTimeSaturday=new DateTime(2024,09,21);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(0),dateTimeSunday.AddHours(23));//    23 hours @ Rate 3
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(0),dateTimeSaturday.AddHours(23));//23 hours @ Rate 3
			//Week 2 - Work a typical 9 - 5 week (40 hours @ Rate 1).
			DateTime dateTimeSunday2=new DateTime(2024,09,22);
			DateTime dateTimeMonday2=new DateTime(2024,09,23);
			DateTime dateTimeTuesday2=new DateTime(2024,09,24);
			DateTime dateTimeWednesday2=new DateTime(2024,09,25);
			DateTime dateTimeThursday2=new DateTime(2024,09,26);
			DateTime dateTimeFriday2=new DateTime(2024,09,27);
			DateTime dateTimeSaturday2=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(9),dateTimeMonday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(9),dateTimeTuesday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(9),dateTimeWednesday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(9),dateTimeThursday2.AddHours(17));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday2.AddHours(9),dateTimeFriday2.AddHours(17));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(0,0,46);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(40,0,0);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesTwoThreeWithOTSecondRatesOneWithOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"A pay period should only pay overtime rates for hours worked during each week. The hours worked at different rates in other weeks should be ignored when calculating overtime ratios.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>0</td>
    <td>40</td>
    <td>30</td>
    <td>30</td>
    <td>70</td>
    <td>0</td>
    <td>0</td>
    <td>22.86</td>
    <td>17.14</td>
    <td>17.14</td>
    <td>12.86</td>
  </tr>
  <tr>
    <td>60</td>
    <td>0</td>
    <td>0</td>
    <td>20</td>
    <td>60</td>
    <td>40</td>
    <td>20</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
  </tr>
  <tr>
    <td colspan=""11""><b>Pay Period Totals</b></td>
  </tr>
  <tr>
    <td>60</td>
    <td>40</td>
    <td>30</td>
    <td>50</td>
    <td>130</td>
    <td>40</td>
    <td>20</td>
    <td>22.86</td>
    <td>17.14</td>
    <td>17.14</td>
    <td>12.86</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesTwoThreeWithOTSecondRatesOneWithOT() {
			/*********************************************************************************************************
			Work a week where all hours are worked in Rate 2 and Rate 3 follwed by a week with only Rate 1.
			Both weeks have OT.
			**********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			 0 = Rate 1
			40 = Rate 2
			30 = Rate 3
			30 hours of OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			60 = Rate 1
			 0 = Rate 2
			 0 = Rate 3
			20 hours considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeksRate23OTRate1OT");
			//Create a pay period for 09/15/2024 - 09/29/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,15);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that Rate 2 starts after two.
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,afterTimeOfDay:TimeSpan.FromHours(14),hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a week which only has hours in Rate 2 and Rate 3 (weekend and then after two during the week)
			DateTime dateTimeSunday=new DateTime(2024,09,15);
			DateTime dateTimeMonday=new DateTime(2024,09,16);
			DateTime dateTimeTuesday=new DateTime(2024,09,17);
			DateTime dateTimeWednesday=new DateTime(2024,09,18);
			DateTime dateTimeThursday=new DateTime(2024,09,19);
			DateTime dateTimeFriday=new DateTime(2024,09,20);
			DateTime dateTimeSaturday=new DateTime(2024,09,21);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(5),dateTimeSunday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(15),dateTimeMonday.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(15),dateTimeTuesday.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday.AddHours(15),dateTimeWednesday.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday.AddHours(15),dateTimeThursday.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday.AddHours(15),dateTimeFriday.AddHours(23));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(5),dateTimeSaturday.AddHours(20));
			//Week 2 - Work a typical 40 hours @ Rate 1 (weekdays before two).
			DateTime dateTimeSunday2=new DateTime(2024,09,22);
			DateTime dateTimeMonday2=new DateTime(2024,09,23);
			DateTime dateTimeTuesday2=new DateTime(2024,09,24);
			DateTime dateTimeWednesday2=new DateTime(2024,09,25);
			DateTime dateTimeThursday2=new DateTime(2024,09,26);
			DateTime dateTimeFriday2=new DateTime(2024,09,27);
			DateTime dateTimeSaturday2=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(0),dateTimeMonday2.AddHours(12));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(0),dateTimeTuesday2.AddHours(12));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(0),dateTimeWednesday2.AddHours(12));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(0),dateTimeThursday2.AddHours(12));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday2.AddHours(0),dateTimeFriday2.AddHours(12));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(0,40,30);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(60,0,0);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeWithOTSecondRatesOneWithOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"A pay period should only pay overtime rates for hours worked during each week. The hours worked at different rates in other weeks should be ignored when calculating overtime ratios.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>5</td>
    <td>25</td>
    <td>25</td>
    <td>15</td>
    <td>55</td>
    <td>3.64</td>
    <td>1.36</td>
    <td>18.18</td>
    <td>6.82</td>
    <td>18.18</td>
    <td>6.82</td>
  </tr>
  <tr>
    <td>50</td>
    <td>0</td>
    <td>0</td>
    <td>10</td>
    <td>50</td>
    <td>40</td>
    <td>10</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
  </tr>
  <tr>
    <td colspan=""11""><b>Pay Period Totals</b></td>
  </tr>
  <tr>
    <td>55</td>
    <td>25</td>
    <td>25</td>
    <td>25</td>
    <td>105</td>
    <td>43.64</td>
    <td>11.36</td>
    <td>18.18</td>
    <td>6.82</td>
    <td>18.18</td>
    <td>6.82</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeWithOTSecondRatesOneWithOT() {
			/*********************************************************************************************************
			Work a week where hours are worked in every Rate follwed by a week with only Rate 1.
			Both weeks have OT.
			**********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			 5 = Rate 1
			25 = Rate 2
			25 = Rate 3
			15 hours of OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			50 = Rate 1
			 0 = Rate 2
			 0 = Rate 3
			10 hours considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeksRate123OTRate1OT");
			//Create a pay period for 09/15/2024 - 09/29/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,15);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that Rate 2 starts after 10.
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,afterTimeOfDay:TimeSpan.FromHours(10),hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a week which has hours at all Rates
			DateTime dateTimeSunday=new DateTime(2024,09,15);
			DateTime dateTimeMonday=new DateTime(2024,09,16);
			DateTime dateTimeTuesday=new DateTime(2024,09,17);
			DateTime dateTimeWednesday=new DateTime(2024,09,18);
			DateTime dateTimeThursday=new DateTime(2024,09,19);
			DateTime dateTimeFriday=new DateTime(2024,09,20);
			DateTime dateTimeSaturday=new DateTime(2024,09,21);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(10),dateTimeSunday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(5),dateTimeMonday.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(10),dateTimeTuesday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday.AddHours(10),dateTimeWednesday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday.AddHours(10),dateTimeThursday.AddHours(15));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSaturday.AddHours(5),dateTimeSaturday.AddHours(20));
			//Week 2 - Work a week with 50 hours @ Rate 1 (weekdays before 10).
			DateTime dateTimeSunday2=new DateTime(2024,09,22);
			DateTime dateTimeMonday2=new DateTime(2024,09,23);
			DateTime dateTimeTuesday2=new DateTime(2024,09,24);
			DateTime dateTimeWednesday2=new DateTime(2024,09,25);
			DateTime dateTimeThursday2=new DateTime(2024,09,26);
			DateTime dateTimeFriday2=new DateTime(2024,09,27);
			DateTime dateTimeSaturday2=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(0),dateTimeMonday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(0),dateTimeTuesday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(0),dateTimeWednesday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(0),dateTimeThursday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeFriday2.AddHours(0),dateTimeFriday2.AddHours(10));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(5,25,25);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(50,0,0);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeWithOTSecondRatesOneNoOT)]
		[Documentation.VersionAdded("24.3")]
		[Documentation.Description(@"A pay period should only pay overtime rates for hours worked during each week. The hours worked at different rates in other weeks should be ignored when calculating overtime ratios.
<table>
  <tr>
    <td colspan=""3"">Hours Worked</td>
    <td colspan=""2""></td>
    <td colspan=""6"">Hours Paid</td>
  </tr>
  <tr>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">OT</td>
    <td style=""padding:5px;"">Total</td>
    <td style=""padding:5px;"">Rate 1</td>
    <td style=""padding:5px;"">Rate 1 OT</td>
    <td style=""padding:5px;"">Rate 2</td>
    <td style=""padding:5px;"">Rate 2 OT</td>
    <td style=""padding:5px;"">Rate 3</td>
    <td style=""padding:5px;"">Rate 3 OT</td>
  </tr>
  <tr>
    <td>20</td>
    <td>20</td>
    <td>20</td>
    <td>20</td>
    <td>60</td>
    <td>13.33</td>
    <td>6.67</td>
    <td>13.33</td>
    <td>6.67</td>
    <td>13.33</td>
    <td>6.67</td>
  </tr>
  <tr>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>40</td>
    <td>40</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
    <td>0</td>
  </tr>
  <tr>
    <td colspan=""11""><b>Pay Period Totals</b></td>
  </tr>
  <tr>
    <td>60</td>
    <td>20</td>
    <td>20</td>
    <td>20</td>
    <td>100</td>
    <td>53.33</td>
    <td>6.67</td>
    <td>13.33</td>
    <td>6.67</td>
    <td>13.33</td>
    <td>6.67</td>
  </tr>
</table>")]
		public void ClockEvents_GetTimeCardManage_TwoWeeksFirstRatesOneTwoThreeWithOTSecondRatesOneNoOT() {
			/*********************************************************************************************************
			Work a week where hours are worked in every Rate follwed by a week with only Rate 1 which has no OT.
			**********************************************************************************************************
			                                                    Week 1
			**********************************************************************************************************
			20 = Rate 1
			20 = Rate 2
			20 = Rate 3
			20 hours of OT (rule for OT is anything over 40 hours in a week).
			**********************************************************************************************************
			                                                    Week 2
			**********************************************************************************************************
			40 = Rate 1
			 0 = Rate 2
			 0 = Rate 3
			 0 hours considered OT (rule for OT is anything over 40 hours in a week).
			*********************************************************************************************************/
			//Create an employee that will have clock events.
			Employee employee=EmployeeT.CreateEmployee("MultiWeeksRate123OTRate1noOT");
			//Create a pay period for 09/15/2024 - 09/29/2024
			DateTime dateTimePayPeriodStart=new DateTime(2024,09,15);
			PayPeriod payPeriod=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(dateTimePayPeriodStart);
			//Create a time card rule for the employee so that Rate 2 starts after 10.
			TimeCardRuleT.CreateTimeCardRule(employeeNum:employee.EmployeeNum,afterTimeOfDay:TimeSpan.FromHours(10),hasWeekendRate3:true);
			//Have weeks start on Sunday.
			PrefT.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,(int)DayOfWeek.Sunday);
			//Week 1 - Work a week which has hours at all Rates
			DateTime dateTimeSunday=new DateTime(2024,09,15);
			DateTime dateTimeMonday=new DateTime(2024,09,16);
			DateTime dateTimeTuesday=new DateTime(2024,09,17);
			DateTime dateTimeWednesday=new DateTime(2024,09,18);
			DateTime dateTimeThursday=new DateTime(2024,09,19);
			DateTime dateTimeFriday=new DateTime(2024,09,20);
			DateTime dateTimeSaturday=new DateTime(2024,09,21);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeSunday.AddHours(0),dateTimeSunday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday.AddHours(0),dateTimeMonday.AddHours(20));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday.AddHours(0),dateTimeTuesday.AddHours(20));
			//Week 2 - Work a week with 40 hours @ Rate 1 (weekdays before 10).
			DateTime dateTimeSunday2=new DateTime(2024,09,22);
			DateTime dateTimeMonday2=new DateTime(2024,09,23);
			DateTime dateTimeTuesday2=new DateTime(2024,09,24);
			DateTime dateTimeWednesday2=new DateTime(2024,09,25);
			DateTime dateTimeThursday2=new DateTime(2024,09,26);
			DateTime dateTimeFriday2=new DateTime(2024,09,27);
			DateTime dateTimeSaturday2=new DateTime(2024,09,28);
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeMonday2.AddHours(0),dateTimeMonday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeTuesday2.AddHours(0),dateTimeTuesday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeWednesday2.AddHours(0),dateTimeWednesday2.AddHours(10));
			ClockEventT.InsertWorkPeriod(employee.EmployeeNum,dateTimeThursday2.AddHours(0),dateTimeThursday2.AddHours(10));
			//Act like the user clicked the "Calc Daily" and "Calc Week OT" buttons in the Time Card window.
			try {
				TimeCardRules.CalculateDailyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(employee,payPeriod.DateStart,payPeriod.DateStop);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//Get the table that would display to the user within the Time Card Manage window and assert that the values are as desired.
			DataTable tableTimeCardManage=ClockEvents.GetTimeCardManage(payPeriod.DateStart,payPeriod.DateStop,0,false);
			Assert.AreEqual(1,tableTimeCardManage.Rows.Count);
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("totalHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate1OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate2OTHours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3Hours"));
			Assert.IsTrue(tableTimeCardManage.Columns.Contains("rate3OTHours"));
			RateRatioOT rateRatioOTWeek1=new RateRatioOT(20,20,20);
			RateRatioOT rateRatioOTWeek2=new RateRatioOT(40,0,0);
			RateRatioOT rateRatioOTExpected=new RateRatioOT(rateRatioOTWeek1,rateRatioOTWeek2);
			Assert.AreEqual(rateRatioOTExpected.TimeSpanTotalHours,PIn.Time(tableTimeCardManage.Rows[0]["totalHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate1Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate1OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate1OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate2Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate2OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate2OTHours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3Hours,PIn.Time(tableTimeCardManage.Rows[0]["rate3Hours"].ToString()));
			Assert.AreEqual(rateRatioOTExpected.TimeSpanRate3OTHours,PIn.Time(tableTimeCardManage.Rows[0]["rate3OTHours"].ToString()));
		}

		///<summary>Helper class that executes all of the math for manually calculating time card rates and OT ratios.</summary>
		private class RateRatioOT {
			public double TotalHours;
			public double Rate1Hours;
			public double Rate1OTHours;
			public double Rate1Ratio;
			public double Rate2Hours;
			public double Rate2OTHours;
			public double Rate2Ratio;
			public double Rate3Hours;
			public double Rate3OTHours;
			public double Rate3Ratio;

			public TimeSpan TimeSpanTotalHours => TimeSpan.FromHours(TotalHours);
			public TimeSpan TimeSpanRate1Hours => TimeSpan.FromHours(Rate1Hours);
			public TimeSpan TimeSpanRate1OTHours => TimeSpan.FromHours(Rate1OTHours);
			public TimeSpan TimeSpanRate2Hours => TimeSpan.FromHours(Rate2Hours);
			public TimeSpan TimeSpanRate2OTHours => TimeSpan.FromHours(Rate2OTHours);
			public TimeSpan TimeSpanRate3Hours => TimeSpan.FromHours(Rate3Hours);
			public TimeSpan TimeSpanRate3OTHours => TimeSpan.FromHours(Rate3OTHours);

			public RateRatioOT(double rate1,double rate2,double rate3) {
				TotalHours=rate1 + rate2 + rate3;
				double regularHours=0;
				double overTimeHours=0;
				if(TotalHours > 0) {
					regularHours=Math.Min(TotalHours,40D);
					overTimeHours=Math.Max(TotalHours - 40D,0D);
				}
				if(TotalHours!=0) {
					Rate2Ratio=rate2 / TotalHours;
					Rate3Ratio=rate3 / TotalHours;
					Rate1Ratio=1D - Rate2Ratio - Rate3Ratio;
				}
				//Rate 1 hours = (1 - (Rate 2 + Rate 3)/Total hours) * Regular hours
				Rate1Hours=(1D - (rate2 + rate3) / TotalHours) * regularHours;
				//Rate 2 hours = (Rate 2/Total hours) * Regular hours
				Rate2Hours=(rate2 / TotalHours) * regularHours;
				//Rate 3 hours = (Rate 3/Total hours) * Regular hours
				Rate3Hours=(rate3 / TotalHours) * regularHours;
				//Rate 1 OT hours = (1 - (Rate 2 + Rate 3)/Total hours) * OT hours
				Rate1OTHours=(1D - (rate2 + rate3) / TotalHours) * overTimeHours;
				//Rate 2 OT hours = (1 - (Rate 1 + Rate 3)/Total hours) * OT hours
				Rate2OTHours=(1D - (rate1 + rate3) / TotalHours) * overTimeHours;
				//Rate 3 OT Hours = Total hours - (Rate 1 + Rate 2 + Rate 3 + Rate 1 OT + Rate 2 OT hours)
				Rate3OTHours=TotalHours - (Rate1Hours + Rate2Hours + Rate3Hours + Rate1OTHours + Rate2OTHours);
			}

			///<summary>Creates a singular object by summing all of the objects passed in together.</summary>
			public RateRatioOT(params RateRatioOT[] rateRatioOTArray) {
				for(int i=0;i<rateRatioOTArray.Length;i++) {
					this.TotalHours += rateRatioOTArray[i].TotalHours;
					this.Rate1Hours += rateRatioOTArray[i].Rate1Hours;
					this.Rate1OTHours += rateRatioOTArray[i].Rate1OTHours;
					this.Rate1Ratio += rateRatioOTArray[i].Rate1Ratio;
					this.Rate2Hours += rateRatioOTArray[i].Rate2Hours;
					this.Rate2OTHours += rateRatioOTArray[i].Rate2OTHours;
					this.Rate2Ratio += rateRatioOTArray[i].Rate2Ratio;
					this.Rate3Hours += rateRatioOTArray[i].Rate3Hours;
					this.Rate3OTHours += rateRatioOTArray[i].Rate3OTHours;
					this.Rate3Ratio += rateRatioOTArray[i].Rate3Ratio;
				}
			}
		}

	}
}
