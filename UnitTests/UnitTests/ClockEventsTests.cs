using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class ClockEventsTests:TestBase {

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

	}
}
