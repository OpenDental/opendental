using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.TimeCardRules_Tests {
	[TestClass]
	public class TimeCardRulesTests:TestBase {

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before running up a test.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		///<summary>Option for adjusting breaks over 30 minutes is on. The employee had a break of 40 minutes and should be adjusted accordingly. 
		///Because the break is split into the employee's two shifts, the adjustment will happen on the second shift as the last 10 minutes of 
		///break time occurred then. Previously called Legacy Test 24.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateDailyOvertime_WithSplitBreak)]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Description(
			@"There is an overtime rule for the employee for  more than 10 hours per day. Option is on to adjust if breaks over 30  minutes. Push the Calc Daily button. 10:00 of regular time.
<table>
  <tr>
    <td colspan='3'>Scenario</td>
    <td colspan='2'>Results</td>
    <td></td>
  </tr>
  <tr>
    <td>TimeDisplayed1</td>
    <td>TimeDisplayed2</td>
    <td>ClockStatus</td>
    <td>AdjustAuto</td>
    <td>OtimeAuto</td>
    <td>Calc. Len.</td>
  </tr>
  <tr>
    <td>08:00:00 AM</td>
    <td>01:00:00 PM</td>
    <td>Lunch</td>
    <td></td>
    <td></td>
    <td>5:00</td>
  </tr>
  <tr>
    <td>10:00:00 AM</td>
    <td>10:20:00 AM</td>
    <td>break</td>
    <td></td>
    <td></td>
    <td>0:20</td>
  </tr>
  <tr>
    <td>02:00:00 PM</td>
    <td>09:00:00 PM</td>
    <td>Home</td>
    <td>-00:10:00</td>
    <td>01:50:00</td>
    <td >7:00</td>
  </tr>
  <tr>
    <td>04:00:00 PM</td>
    <td>4:20:00 PM</td>
    <td>break</td>
    <td></td>
    <td></td>
    <td>0:20</td>
  </tr>
</table>")]



		public void TimeCardRules_CalculateDailyOvertime_WithSplitBreak() {
			string suffix="24";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,true);
			TimeCardRuleT.CreateHoursTimeRule(emp.EmployeeNum,TimeSpan.FromHours(10));
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(8),startDate.AddHours(13),0);
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(14),startDate.AddHours(21),0);
			ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(10),20,0);
			ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(16),20,0);
			CalculateDailyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			Assert.AreEqual(TimeSpan.FromMinutes(-10),ClockEvents.GetOne(clockEvent2).AdjustAuto);
		}

		///<summary>This employee works four days for 11 hours each day. Because of this, they will have 40 hours of regular time and 4 hours of OT.
		///Previously called Legacy Test 25.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_DuringNormalWorkWeek)]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Description(@"Work week is entirely within the same pay  period. Push the Calc Week OT button.
<table>
  <tr>
    <td colspan='3'>Scenario</td>
    <td colspan='2'>Results</td>
    <td></td>
  </tr>
  <tr>
    <td>TimeDisplayed1</td>
    <td>TimeDisplayed2</td>
    <td>ClockStatus</td>
    <td>AdjustAuto</td>
    <td>OtimeAuto</td>
    <td>Calc. Len.</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
</table>
<table>
  <tr>
    <td colspan='4'>Results (TimeAdjust Entry)</td>
  </tr>
  <tr>
    <td>TimeEntry</td>
    <td>RegHours</td>
    <td>OtimeHours</td>
    <td>IsAuto</td>
  </tr>
  <tr>
    <td>8:00 PM</td>
    <td>-4:00</td>
    <td>4:00</td>
    <td>true</td>
  </tr>
</table>")]
		public void TimeCardRules_CalculateWeeklyOvertime_DuringNormalWorkWeek() {
			string suffix="25";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(0).AddHours(6),startDate.AddDays(0).AddHours(17));
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(6),startDate.AddDays(1).AddHours(17));
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(2).AddHours(6),startDate.AddDays(2).AddHours(17));
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(3).AddHours(6),startDate.AddDays(3).AddHours(17));
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			TimeAdjust result=TimeAdjusts.Refresh(emp.EmployeeNum,startDate,startDate.AddDays(13))[0];
			Assert.AreEqual(TimeSpan.FromHours(-4),result.RegHours);
			Assert.AreEqual(TimeSpan.FromHours(4),result.OTimeHours);
		}

		///<summary>This employee works four days for 11 hours each day. The fourth day of the week is on another pay period. When calculating the OT 
		///for the second pay period, they should have four hours of OT and 40 regular hours. Previously called Legacy Test 26.</summary>
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriods)]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Description(@"Work week is split  between two pay periods
	While viewing the first pay period, push the  Calc Week OT button. View the second pay period, and push the Calc Week OT button.
<table>
  <tr>
    <td colspan='3'>Scenario</td>
    <td colspan='2'>Results</td>
    <td></td>
  </tr>
  <tr>
    <td>TimeDisplayed1</td>
    <td>TimeDisplayed2</td>
    <td >ClockStatus</td>
    <td>AdjustAuto</td>
    <td>OtimeAuto</td>
    <td>Calc. Len.</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td colspan='5'><strong>Start of new pay period.</strong></td>
    <td></td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
</table>
<table>
  <tr>
    <td colspan='4'>Results (TimeAdjust Entry)</td>
  </tr>
  <tr>
    <td>TimeEntry</td>
    <td>RegHours</td>
    <td>OtimeHours</td>
    <td>IsAuto</td>
  </tr>
  <tr>
    <td>8:00 PM</td>
    <td>-4:00</td>
    <td>4:00</td>
    <td>true</td>
  </tr>
</table>")]
		[TestMethod]
		public void TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriods() {
			string suffix="26";
			DateTime startDate=DateTime.Parse("2001-02-01");//This will create a pay period that splits a work week.
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriod payP2=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate.AddDays(14));
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(10).AddHours(6),startDate.AddDays(10).AddHours(17),0);
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(11).AddHours(6),startDate.AddDays(11).AddHours(17),0);
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(12).AddHours(6),startDate.AddDays(12).AddHours(17),0);
			//new pay period
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(14).AddHours(6),startDate.AddDays(14).AddHours(17),0);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP2.DateStart,payP2.DateStop);
			//Validate
			List<TimeAdjust> resultList=TimeAdjusts.Refresh(emp.EmployeeNum,startDate,startDate.AddDays(28));
			Assert.IsFalse(resultList.Count < 1);
			TimeAdjust result=resultList[0];
			Assert.AreEqual(TimeSpan.FromHours(-4),result.RegHours);
			Assert.AreEqual(TimeSpan.FromHours(4),result.OTimeHours);
		}

		///<summary>This employee works six days, Mon-Sat, for 11 hours each day. The work week starts on Wednesday. When calculating OT,
		///the second week that begins on Wednesday will have four hours of OT and 40 regular hours. Previously called Legacy Test 27.</summary>
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_OneWeekWorkWeekStartsOnWednesday)]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Description(@"The work week starts on  Wednesday. Push the Calc Daily button, then push the Calc Week OT  button.
<table>
  <tr>
    <td colspan='3'>Scenario</td>
    <td colspan='2'>Results</td>
    <td></td>
  </tr>
  <tr>
    <td>TimeDisplayed1</td>
    <td>TimeDisplayed2</td>
    <td>ClockStatus</td>
    <td>AdjustAuto</td>
    <td>OtimeAuto</td>
    <td>Calc. Len.</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td> </td>
    <td> </td>
    <td>11:00</td>
  </tr>
  <tr>
    <td colspan='5'>Wednesday, start of new work week.</td>
    <td></td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
  <tr>
    <td>06:00:00 AM</td>
    <td>05:00:00 PM</td>
    <td>Home</td>
    <td></td>
    <td></td>
    <td>11:00</td>
  </tr>
</table>
<table>
  <tr>
    <td colspan='4'>Results (TimeAdjust Entry)</td>
  </tr>
  <tr>
    <td>TimeEntry</td>
    <td>RegHours</td>
    <td>OtimeHours</td>
    <td>IsAuto</td>
  </tr>
  <tr>
    <td>8:00 PM</td>
    <td>-4:00</td>
    <td>4:00</td>
    <td>true</td>
  </tr>
</table>")]
		[TestMethod]
		public void TimeCardRules_CalculateWeeklyOvertime_OneWeekWorkWeekStartsOnWednesday() {
			string suffix="27";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,3);
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(0).AddHours(6),startDate.AddDays(0).AddHours(17),0);
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(6),startDate.AddDays(1).AddHours(17),0);
			//new work week	
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(2).AddHours(6),startDate.AddDays(2).AddHours(17),0);
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(3).AddHours(6),startDate.AddDays(3).AddHours(17),0);
			long clockEvent5=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(4).AddHours(6),startDate.AddDays(4).AddHours(17),0);
			long clockEvent6=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(5).AddHours(6),startDate.AddDays(5).AddHours(17),0);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			TimeAdjust result=TimeAdjusts.Refresh(emp.EmployeeNum,startDate,startDate.AddDays(28))[0];
			Assert.AreEqual(TimeSpan.FromHours(-4),result.RegHours);
			Assert.AreEqual(TimeSpan.FromHours(4),result.OTimeHours);
		}

		///<summary>There is a special rule that allows for overtime for time worked after 4pm. This employee works from 8am-4:40pm with a 40 minute 
		///break in between. With the adjust for breaks over 30 minutes set, when calculating overtime, there is a 10 minute adjustment for the break
		///and 40 minutes of overtime for the time worked after 4pm. Previously called Legacy Test 32.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateDailyOvertime_ForHoursWorkedAfterACertainTime)]
		[Documentation.Description(@"There is a special rule that allows for overtime for time worked after 4pm. This employee works from 8am-4:40pm with a 40 minute break in between. With the adjust for breaks over 30 minutes set, when calculating overtime, there is a 10 minute adjustment for the breakand 40 minutes of overtime for the time worked after 4pm. Previously called Legacy Test 32.")]
		public void TimeCardRules_CalculateDailyOvertime_ForHoursWorkedAfterACertainTime() {
			string suffix="32";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,true);
			Prefs.RefreshCache();
			TimeCardRuleT.CreatePMTimeRule(emp.EmployeeNum,TimeSpan.FromHours(16));
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(8),startDate.AddHours(16).AddMinutes(40),0);
			ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(11),40,0);
			CalculateDailyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			Assert.AreEqual(TimeSpan.FromMinutes(-10),ClockEvents.GetOne(clockEvent1).AdjustAuto);
			Assert.AreEqual(TimeSpan.FromMinutes(40),ClockEvents.GetOne(clockEvent1).Rate2Auto);
		}

		///<summary>There is a special rule that allows for overtime for time worked before 7:30am. This employee works from 6am-4pm with a 40 minute 
		///break in between. With the adjust for breaks over 30 minutes set, when calculating overtime, there is a 10 minute adjustment for the break
		///and 90 minutes of overtime for the time worked before 7:30am. Previously called Legacy Test 33.</summary>
		[Documentation.Description(@"There is a special rule that allows for overtime for time worked before 7:30am. This employee works from 6am-4pm with a 40 minute break in between. With the adjust for breaks over 30 minutes set, when calculating overtime, there is a 10 minute adjustment for the break and 90 minutes of overtime for the time worked before 7:30am. Previously called Legacy Test 33.")]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateDailyOvertime_ForHoursWorkedBeforeACertainTime)]
		[TestMethod]
		public void TimeCardRules_CalculateDailyOvertime_ForHoursWorkedBeforeACertainTime() {
			string suffix="33";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,true);
			TimeCardRuleT.CreateAMTimeRule(emp.EmployeeNum,TimeSpan.FromHours(7.5));
			TimeCardRules.RefreshCache();
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(6),startDate.AddHours(16),0);
			ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(11),40,0);
			CalculateDailyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			Assert.AreEqual(TimeSpan.FromMinutes(-10),ClockEvents.GetOne(clockEvent1).AdjustAuto);
			Assert.AreEqual(TimeSpan.FromMinutes(90),ClockEvents.GetOne(clockEvent1).Rate2Auto);
		}

		///<summary>Tests clinic-specific overtime hour adjustments for a single work period. Previously called Legacy Test 62.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinics)]
		[Documentation.Description("Tests clinic-specific overtime hour adjustments for a single work period. Previously called Legacy Test 62.")]
		public void TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinics() {
			string suffix="62";
			DateTime startDate=DateTime.Parse("2001-01-01");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			//Each of these are 11 hour days. Should have 4 hours of OT with clinic 3 and 11 hours OT with clinic 4 the end of the pay period.
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(0).AddHours(6),startDate.AddDays(0).AddHours(17),0);
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(6),startDate.AddDays(1).AddHours(17),1);
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(2).AddHours(6),startDate.AddDays(2).AddHours(17),2);
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(3).AddHours(6),startDate.AddDays(3).AddHours(17),3);
			long clockEvent5=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(4).AddHours(6),startDate.AddDays(4).AddHours(17),4);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			List<TimeAdjust> listAdjusts=TimeAdjusts.GetValidList(emp.EmployeeNum,startDate,startDate.AddDays(5)).OrderBy(x=>x.OTimeHours).ToList();
			Assert.AreEqual(2,listAdjusts.Count);
			Assert.AreEqual(TimeSpan.FromHours(-4),listAdjusts[0].RegHours);
			Assert.AreEqual(3,listAdjusts[0].ClinicNum);
			Assert.AreEqual(TimeSpan.FromHours(4),listAdjusts[0].OTimeHours);
			Assert.AreEqual(TimeSpan.FromHours(-11),listAdjusts[1].RegHours);
			Assert.AreEqual(4,listAdjusts[1].ClinicNum);
			Assert.AreEqual(TimeSpan.FromHours(11),listAdjusts[1].OTimeHours);
		}

		///<summary>Tests clinic-specific overtime hour adjustments for work week spanning two pay periods. Previously called Legacy Test 63.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinics)]
		[Documentation.Description("Tests clinic-specific overtime hour adjustments for work week spanning two pay periods. Previously called Legacy Test 63.")]
		public void TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinics() {
			string suffix="63";
			DateTime startDate=DateTime.Parse("2001-02-01");//This will create a pay period that splits a work week.
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriod payP2=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate.AddDays(14));
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			//Each of these are 11 hour days. Should have 4 hours of OT with clinic 3 in the second pay period.
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(10).AddHours(6),startDate.AddDays(10).AddHours(17),0);
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(11).AddHours(6),startDate.AddDays(11).AddHours(17),1);
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(12).AddHours(6),startDate.AddDays(12).AddHours(17),2);
			//New pay period
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(14).AddHours(6),startDate.AddDays(14).AddHours(17),3);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP2.DateStart,payP2.DateStop);
			//Validate
			List<TimeAdjust> listAdjusts=TimeAdjusts.GetValidList(emp.EmployeeNum,startDate,startDate.AddDays(28));
			Assert.AreEqual(1,listAdjusts.Count);
			Assert.AreEqual(TimeSpan.FromHours(-4),listAdjusts[0].RegHours);
			Assert.AreEqual(3,listAdjusts[0].ClinicNum);
			Assert.AreEqual(TimeSpan.FromHours(4),listAdjusts[0].OTimeHours);
		}

		///<summary>Tests clinic-specific overtime hour adjustments for work week spanning two pay periods and expecting adjustments for multiple 
		///clinics. Previously called Legacy Test 64.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinicPreferences)]
		[Documentation.Description("Tests clinic-specific overtime hour adjustments for work week spanning two pay periods and expecting adjustments for multiple clinics. Previously called Legacy Test 64.")]
		public void TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinicPreferences() {
			string suffix="64";
			DateTime startDate=DateTime.Parse("2001-02-01");//This will create a pay period that splits a work week.
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriod payP2=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate.AddDays(14));
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			//Each of these are 11 hour days. Should have 4 hours of OT with clinic 3 in the second pay period and 11 hours for clinic 4.
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(10).AddHours(6),startDate.AddDays(10).AddHours(17),0);//Sun
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(11).AddHours(6),startDate.AddDays(11).AddHours(17),1);//Mon
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(12).AddHours(6),startDate.AddDays(12).AddHours(17),2);//Tue
			//new pay period
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(14).AddHours(6),startDate.AddDays(14).AddHours(17),3);//Wed
			long clockEvent5=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(15).AddHours(6),startDate.AddDays(15).AddHours(17),4);//Thurs
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			TimeCardRules.CalculateWeeklyOvertime(emp,payP2.DateStart,payP2.DateStop);
			//Validate
			List<TimeAdjust> listAdjusts=TimeAdjusts.GetValidList(emp.EmployeeNum,startDate,startDate.AddDays(28)).OrderBy(x=>x.OTimeHours).ToList();
			Assert.AreEqual(2,listAdjusts.Count);
			Assert.AreEqual(TimeSpan.FromHours(-4),listAdjusts[0].RegHours);
			Assert.AreEqual(3,listAdjusts[0].ClinicNum);
			Assert.AreEqual(TimeSpan.FromHours(4),listAdjusts[0].OTimeHours);
			Assert.AreEqual(TimeSpan.FromHours(-11),listAdjusts[1].RegHours);
			Assert.AreEqual(4,listAdjusts[1].ClinicNum);
			Assert.AreEqual(TimeSpan.FromHours(11),listAdjusts[1].OTimeHours);
		}


		///<summary>Tests a normal work week with a start of the week in the previous pay period with break adjustments. Previously called 
		///Legacy Test 66.
		///Note: This unit test was based on real data from a real set of timecard entries.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinicsRealData)]
		[Documentation.Description(@"Tests a normal work week with a start of the week in the previous pay period with break adjustments. Previously called Legacy Test 66.
		Note: This unit test was based on real data from a real set of timecard entries.")]
		public void TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinicsRealData() {
			string suffix="66";
			DateTime startDate=DateTime.Parse("2016-05-09");//This will create a pay period that splits a work week.
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			//Each of these are 11 hour days. Should have 4 hours of OT with clinic 3 in the second pay period and 11 hours for clinic 4.
			//Week 1 - 40.4 hours
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(0).AddHours(6),startDate.AddDays(0).AddHours(6+8),0);//Mon
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(6),startDate.AddDays(1).AddHours(6+8),0);//Tue
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(2).AddHours(6),startDate.AddDays(2).AddHours(6+8.76),0,0.06);//Wed
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(3).AddHours(6),startDate.AddDays(3).AddHours(6+8.72),0,0.73);//Thurs
			long clockEvent5=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(4).AddHours(6),startDate.AddDays(4).AddHours(6+8.12),0,0.41);//Fri
			//Week 2 - 41.23 hours
			long clockEvent6=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(7).AddHours(6),startDate.AddDays(7).AddHours(6+8.79),0,0.4);//Mon
			long clockEvent7=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(8).AddHours(6),startDate.AddDays(8).AddHours(6+8.85),0,0.38);//Tue
			long clockEvent8=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(9).AddHours(6),startDate.AddDays(9).AddHours(6+7.78),0,0.29);//Wed
			long clockEvent9=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(10).AddHours(6),startDate.AddDays(10).AddHours(6+8.88),0,0.02);//Thurs
			long clockEvent10=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(11).AddHours(6),startDate.AddDays(11).AddHours(6+8.59),0,0.57);//Fri
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			List<TimeAdjust> listAdjusts=TimeAdjusts.GetValidList(emp.EmployeeNum,startDate,startDate.AddDays(28)).OrderBy(x=>x.OTimeHours).ToList();
			Assert.AreEqual(2,listAdjusts.Count);
			Assert.AreEqual(TimeSpan.FromHours(-0.4),listAdjusts[0].RegHours);
			Assert.AreEqual(TimeSpan.FromHours(0.4),listAdjusts[0].OTimeHours);
			Assert.AreEqual(TimeSpan.FromHours(-1.23),listAdjusts[1].RegHours);
			Assert.AreEqual(TimeSpan.FromHours(1.23),listAdjusts[1].OTimeHours);
		}

		///<summary>Test work week with manual overtime hours. Previously called Legacy Test 67.
		///Note: This unit test was based on real data from a real set of timecard entries including dates, timespans, and the like.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.TimeCardRules_CalculateWeeklyOvertime_CalculationWithManualOvertime)]
		[Documentation.Description(@"Test work week with manual overtime hours. Previously called Legacy Test 67.
		Note: This unit test was based on real data from a real set of timecard entries including dates, timespans, and the like.")]
		public void TimeCardRules_CalculateWeeklyOvertime_CalculationWithManualOvertime() {
			string suffix="67";
			DateTime startDate=DateTime.Parse("2016-03-14");
			Employee emp=EmployeeT.CreateEmployee(suffix);
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			TimeCardRules.RefreshCache();
			//Each of these are 11 hour days. Should have 4 hours of OT with clinic 3 in the second pay period and 11 hours for clinic 4.
			//Week 1 - 40.13 (Note: These appear as they should after CalculateDaily is run.)
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(0).AddHours(6),startDate.AddDays(0).AddHours(6+8.06),0);//Mon
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(6),startDate.AddDays(1).AddHours(6+8),0);//Tue
			long clockEvent3=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(2).AddHours(6),startDate.AddDays(2).AddHours(6+8.08),0);//Wed
			long clockEvent4=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(3).AddHours(6),startDate.AddDays(3).AddHours(6+8),0,0.02);//Thurs
			long clockEvent5=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(4).AddHours(6),startDate.AddDays(4).AddHours(6+8.01),0);//Fri
			//SATURDAY - 4.1 HRS OF OVERTIME 
			ClockEvent ce=new ClockEvent();
			ce.ClinicNum=0;
			ce.ClockStatus=TimeClockStatus.Home;
			ce.EmployeeNum=emp.EmployeeNum;
			ce.OTimeHours=TimeSpan.FromHours(4.1);
			ce.TimeDisplayed1=new DateTime(startDate.Year,startDate.Month,startDate.AddDays(5).Day,6,54,0);
			ce.TimeDisplayed2=new DateTime(startDate.Year,startDate.Month,startDate.AddDays(5).Day,11,0,0);
			ce.TimeEntered1=ce.TimeDisplayed1;
			ce.TimeEntered2=ce.TimeDisplayed2;
			ce.ClockEventNum=ClockEvents.Insert(ce);
			ClockEvents.Update(ce);
			//Week 2 - 41.06
			long clockEvent6=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(7).AddHours(6),startDate.AddDays(7).AddHours(6+8.02),0);//Mon
			long clockEvent7=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(8).AddHours(6),startDate.AddDays(8).AddHours(6+8),0);//Tue
			long clockEvent8=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(9).AddHours(6),startDate.AddDays(9).AddHours(6+8),0);//Wed
			long clockEvent9=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(10).AddHours(6),startDate.AddDays(10).AddHours(6+9.04),0);//Thurs
			long clockEvent10=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(11).AddHours(6),startDate.AddDays(11).AddHours(6+8),0);//Fri
			TimeCardRules.CalculateWeeklyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Validate
			List<TimeAdjust> listAdjusts=TimeAdjusts.GetValidList(emp.EmployeeNum,startDate,startDate.AddDays(28)).OrderBy(x=>x.OTimeHours).ToList();
			Assert.AreEqual(2,listAdjusts.Count);
			Assert.AreEqual(TimeSpan.FromHours(-0.13),listAdjusts[0].RegHours);
			Assert.AreEqual(TimeSpan.FromHours(0.13),listAdjusts[0].OTimeHours);
			Assert.AreEqual(TimeSpan.FromHours(-1.06),listAdjusts[1].RegHours);
			Assert.AreEqual(TimeSpan.FromHours(1.06),listAdjusts[1].OTimeHours);
		}

		///<summary>When calculating daily adjustments for a pay period while the pay period is ongoing, we do not include the current day. This
		///is so that breaks can be calculated without clocking out employees. This unit test unsures breaks can be calculated for a patient who
		///is still clocked in for the day the calculation is occuring.</summary>
		[TestMethod]
		public void TimeCardRules_CalculateDailyOvertime_WhileEmployeeClockedInTodayDuringPayPeriod() {
			DateTime startDate=DateTime.Now.Date.AddDays(-1);
			Employee emp=EmployeeT.CreateEmployee("CalculateDailyWhileEmployeeClockedInDuringPayPeriod");
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,true);
			Prefs.RefreshCache();
			TimeCardRules.RefreshCache();
			//10 hour day with 45 minute break
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(8),startDate.AddHours(18));
			long break1=ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(12),45);
			//Next day clock in, but have no clock out event yet.
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(8),DateTime.MinValue);
			CalculateDailyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Ensure that the 15 minutes was subtracted from the shift.
			Assert.AreEqual(TimeSpan.FromMinutes(-15),ClockEvents.GetOne(clockEvent1).AdjustAuto);
		}

		///<summary>When calculating daily adjustments for a pay period while the pay period is ongoing, we want to include the current day if the
		///employee is clocked out for home for today. This is so that breaks can be calculated without clocking out employees. This unit test ensures
		///breaks can be calculated for an employee who has clocked out for the day the calculation is occurring.</summary>
		[TestMethod]
		public void TimeCardRules_CalculateDailyOvertime_WhileEmployeeClockedOutTodayDuringPayPeriod() {
			DateTime startDate=DateTime.Today.AddDays(-1);
			Employee emp=EmployeeT.CreateEmployee("CalculateDailyWhileEmployeeClockedOutDuringPayPeriod");
			PayPeriod payP1=PayPeriodT.CreateTwoWeekPayPeriodIfNotExists(startDate);
			PayPeriods.RefreshCache();
			Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);
			Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,true);
			Prefs.RefreshCache();
			TimeCardRules.RefreshCache();
			//10 hour day with 45 minute break
			long clockEvent1=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddHours(8),startDate.AddHours(18));
			long break1=ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddHours(12),45);
			//Next day clock in and out for home
			long clockEvent2=ClockEventT.InsertWorkPeriod(emp.EmployeeNum,startDate.AddDays(1).AddHours(8),startDate.AddDays(1).AddHours(18));
			long break2=ClockEventT.InsertBreak(emp.EmployeeNum,startDate.AddDays(1).AddHours(12),45);
			CalculateDailyOvertime(emp,payP1.DateStart,payP1.DateStop);
			//Ensure that the 15 minutes was subtracted from both shifts.
			Assert.AreEqual(TimeSpan.FromMinutes(-15),ClockEvents.GetOne(clockEvent1).AdjustAuto);
			Assert.AreEqual(TimeSpan.FromMinutes(-15),ClockEvents.GetOne(clockEvent2).AdjustAuto);
		}

		///<summary>Calculates daily adjustments for an employee. Used in order to keep the number of references to 
		///TimeCardRules.CalculateDailyOvertime low.</summary>
		///<param name="emp">Employee that the daily overtime will be calculated for.</param>
		///<param name="dateStart">The start date of the pay period.</param>
		///<param name="dateStop">The end date of the pay period</param>
		private void CalculateDailyOvertime(Employee emp,DateTime dateStart,DateTime dateStop) {
			TimeCardRules.CalculateDailyOvertime(emp,dateStart,dateStop);
		}
	}
}
