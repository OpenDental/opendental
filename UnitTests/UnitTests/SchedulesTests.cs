using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;
using UnitTestsCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using CodeBase;

namespace UnitTests.Schedules_Tests {
	[TestClass]
	public class SchedulesTests:TestBase {

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
			OperatoryT.ClearOperatoryTable();
			ScheduleT.ClearScheduleTable();
			ScheduleOpT.ClearScheduleOpTable();
			ApptViewT.ClearApptView();
			ApptViewItemT.ClearApptViewItem();
		}
		
		///<summary>A provider schedule from 8-4 and 1-5 results in a schedule of 8-5. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_8To5() {
			Schedule sched8to4=new Schedule() {
				ProvNum=1,
				ScheduleNum=1,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,16,0,0).TimeOfDay//4pm
			};
			Schedule sched1to5=new Schedule() {
				ProvNum=1,
				ScheduleNum=5,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//1pm
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			Schedule sched8to5=new Schedule() {
				ProvNum=1,
				ScheduleNum=8,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to4,sched1to5 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to5 };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(1,listScheduleProvSched.Count);
			Assert.AreEqual(listRetVal[0].StartTime,listScheduleProvSched[0].StartTime);
			Assert.AreEqual(listRetVal[0].StopTime,listScheduleProvSched[0].StopTime);
		}

		///<summary>A provider schedule from 8-12 and 1-5 results in a schedule of 8-12 and 1-5. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_8To12And1To5() {
			Schedule sched8to12=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,12,0,0).TimeOfDay//12pm
			};
			Schedule sched1to5=new Schedule() {
				ProvNum=1,
				ScheduleNum=5,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//1pm
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to12,sched1to5 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to12,sched1to5 };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(2,listScheduleProvSched.Count);
			Assert.AreEqual(listScheduleProvSched[0].StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(listScheduleProvSched[0].StopTime,listRetVal[0].StopTime);
			Assert.AreEqual(listScheduleProvSched[1].StartTime,listRetVal[1].StartTime);
			Assert.AreEqual(listScheduleProvSched[1].StopTime,listRetVal[1].StopTime);
		}

		///<summary>A provider schedule from 8-12,9-11,and 1-6 results in a schedule of 8-12 and 1-6. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_8To12And1To6() {
			Schedule sched8to12=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,12,0,0).TimeOfDay//12pm
			};
			Schedule sched9to11=new Schedule() {
				ProvNum=1,
				ScheduleNum=3,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,9,0,0).TimeOfDay,//9pm
				StopTime=new DateTime(2018,3,9,11,0,0).TimeOfDay//11am
			};
			Schedule sched1to6=new Schedule() {
				ProvNum=1,
				ScheduleNum=5,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//1pm
				StopTime=new DateTime(2018,3,9,18,0,0).TimeOfDay//6pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to12,sched9to11,sched1to6 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to12,sched1to6 };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(2,listScheduleProvSched.Count);
			Assert.AreEqual(listScheduleProvSched[0].StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(listScheduleProvSched[0].StopTime,listRetVal[0].StopTime);
			Assert.AreEqual(listScheduleProvSched[1].StartTime,listRetVal[1].StartTime);
			Assert.AreEqual(listScheduleProvSched[1].StopTime,listRetVal[1].StopTime);
		}

		///<summary>A provider schedule from 8-12, 9-3, and 4-5 results in a schedule of 8-3 and 4-5. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_8To3And4To5() {
			Schedule sched8to12=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,12,0,0).TimeOfDay//12pm
			};
			Schedule sched9to3=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,9,0,0).TimeOfDay,//9pm
				StopTime=new DateTime(2018,3,9,15,0,0).TimeOfDay//3pm
			};

			Schedule sched4to5=new Schedule() {
				ProvNum=1,
				ScheduleNum=6,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,16,0,0).TimeOfDay,//4pm
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			Schedule sched8to3=new Schedule() {
				ProvNum=1,
				ScheduleNum=7,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,15,0,0).TimeOfDay//3pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to12,sched9to3,sched4to5 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to3,sched4to5 };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(2,listScheduleProvSched.Count);
			Assert.AreEqual(listScheduleProvSched[0].StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(listScheduleProvSched[0].StopTime,listRetVal[0].StopTime);
			Assert.AreEqual( listScheduleProvSched[1].StartTime,listRetVal[1].StartTime);
			Assert.AreEqual(listScheduleProvSched[1].StopTime,listRetVal[1].StopTime);
		}

		///<summary>A provider schedule from 8-11, 1-3, and 4-7 results in a schedule of 8-3 and 4-5. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_8To11And1To3And4To7() {
			Schedule sched8to11=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,11,0,0).TimeOfDay//11pm
			};
			Schedule sched1to3=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//1pm
				StopTime=new DateTime(2018,3,9,15,0,0).TimeOfDay//3pm
			};

			Schedule sched4to7=new Schedule() {
				ProvNum=1,
				ScheduleNum=6,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,16,0,0).TimeOfDay,//4pm
				StopTime=new DateTime(2018,3,9,19,0,0).TimeOfDay//7pm
			};
			Schedule sched8to3=new Schedule() {
				ProvNum=1,
				ScheduleNum=7,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,15,0,0).TimeOfDay//3pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to11,sched1to3,sched4to7 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to11,sched1to3,sched4to7  };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(3,listScheduleProvSched.Count);
			Assert.AreEqual(listScheduleProvSched[0].StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(listScheduleProvSched[0].StopTime,listRetVal[0].StopTime);
			Assert.AreEqual(listScheduleProvSched[1].StartTime,listRetVal[1].StartTime);
			Assert.AreEqual(listScheduleProvSched[1].StopTime,listRetVal[1].StopTime);
			Assert.AreEqual(listScheduleProvSched[2].StartTime,listRetVal[2].StartTime);
			Assert.AreEqual(listScheduleProvSched[2].StopTime,listRetVal[2].StopTime);
		}

		///<summary>A provider schedule from 8-5, 8-4, results in a two schedules for the week with correct stop times. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_DifferentDates() {
			Schedule schedThe12th=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			Schedule schedThe13th=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,13),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,16,0,0).TimeOfDay//4pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { schedThe12th,schedThe13th };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { schedThe12th,schedThe13th  };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(2,listRetVal.Count);
			Assert.AreEqual(listScheduleProvSched[0].StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(listScheduleProvSched[0].StopTime,listRetVal[0].StopTime);
			Assert.AreEqual(listScheduleProvSched[1].StartTime,listRetVal[1].StartTime);
			Assert.AreEqual(listScheduleProvSched[1].StopTime,listRetVal[1].StopTime);
		}

		///<summary>A provider schedule from 8-11, 1-3, and 4-7 results in a schedule of 8-3 and 4-5. </summary>
		[TestMethod]
		public void Schedules_ProvSchedsForProductionGoals_4Overlapping() {
			Schedule sched8to1=new Schedule() {
				ProvNum=1,
				ScheduleNum=2,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,8,0,0).TimeOfDay,//8am
				StopTime=new DateTime(2018,3,9,13,0,0).TimeOfDay//1pm
			};
			Schedule sched12to4=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,12,0,0).TimeOfDay,//12pm
				StopTime=new DateTime(2018,3,9,16,0,0).TimeOfDay//4pm
			};
			Schedule sched1to2=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,13,0,0).TimeOfDay,//1pm
				StopTime=new DateTime(2018,3,9,14,0,0).TimeOfDay//2pm
			};
			Schedule sched3to5=new Schedule() {
				ProvNum=1,
				ScheduleNum=4,
				SchedDate=new DateTime(2018,3,12),
				StartTime=new DateTime(2018,3,9,15,0,0).TimeOfDay,//3pm
				StopTime=new DateTime(2018,3,9,17,0,0).TimeOfDay//5pm
			};
			List<Schedule> listSchedules = new List<Schedule>() { sched8to1,sched12to4,sched1to2,sched3to5 };
			List<Schedule> listScheduleProvSched = new List<Schedule>() { sched8to1,sched12to4,sched1to2,sched3to5  };
			List<Schedule> listRetVal=Schedules.GetProvSchedsForProductionGoals(new Dictionary<long, List<Schedule>>() { { 1,listSchedules } });
			Assert.AreEqual(1,listRetVal.Count);
			Assert.AreEqual(sched8to1.StartTime,listRetVal[0].StartTime);
			Assert.AreEqual(sched3to5.StopTime,listRetVal[0].StopTime);
		}


		#region Provider Schedule Overlapping Tests

		#region Simple (No Operatories)
		
		///<summary>Makes sure that our dynamic provider collision detection logic finds starting time collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_SimpleStartTimeOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Act like we are about to create a schedule that purposefully collides with the start time (08:00 - 10:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumDoc,
				IsNew=true,
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumDoc },
				schedOverlap,
				listSchedulesForDate,
				new List<long>());
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.AreEqual(provNumDoc,listOverlappingSchedProvNums[0]);
		}

		///<summary>Makes sure that our dynamic provider collision detection logic finds stopping time collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_SimpleStopTimeOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Look for a collision for a schedule that purposefully collides with the stop time (17:00).
			//Act like we are about to create a schedule that purposefully collides with the stop time (16:00 - 18:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,16,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumDoc,
				IsNew=true,
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Validate that the collision for a schedule that purposefully collides with the stop time (17:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumDoc },
				schedOverlap,
				listSchedulesForDate,
				new List<long>());
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.AreEqual(provNumDoc,listOverlappingSchedProvNums[0]);
		}

		///<summary>Makes sure that our dynamic provider collision detection logic finds engulfing collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_SimpleEngulfOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Act like we are about to create a schedule that is purposefully engulfed within the start and stop time of our schedule (10:00 - 16:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,16,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumDoc,
				IsNew=true,
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumDoc },
				schedOverlap,
				listSchedulesForDate,
				new List<long>());
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.AreEqual(provNumDoc,listOverlappingSchedProvNums[0]);
		}

		///<summary>Makes sure that our dynamic provider collision detection logic does not find collisions when there are no schedules to collide with.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_SimpleNoProvOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Create a bunch of schedules that are all over the place (even on different dates) that do not collide with each other.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Act like we are about to create a schedule that does not collide with any schedules that we made for this provider (17:00 - 18:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumDoc,
				IsNew=true,
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumDoc },
				schedOverlap,
				listSchedulesForDate,
				new List<long>());
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}

		///<summary>Makes sure that our dynamic provider collision detection logic does NOT find collisions for other providers.
		///Meaning that two providers can have the exact same schedule (this is different functionality to operatory specific schedules).</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_SimpleOtherProvOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSchedDoc","Collider");
			long provNumHyg=ProviderT.CreateProvider("HYG-"+suffix,"ProvSchedHyg","Collider",isSecondary: true);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the doctor.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Act like we are about to create a typical 09:00 - 17:00 schedule for the hygienist.
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>());
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_SimpleCopyPasteDayNoOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Create a bunch of schedules that are all over the place (even on different dates) that do not collide with each other.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Make a schedule for today which will not collide with any schedules that we made for this provider (17:00 - 18:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
			};
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_SimpleCopyPasteDayOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Make a schedule for today which will purposefully collide with the start time (08:00 - 10:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.AreEqual(provNumDoc,listOverlappingSchedProvNums[0]);
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_SimpleCopyPasteWeekNoOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider for 7 straight days.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(2),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(3),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(4),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(5),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			ScheduleT.CreateSchedule(dateSched.AddDays(6),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Make a schedule for an entire week that will not collide with any times in the week above (04:00 - 8:00).
			//It is extremely important that we create a schedule for each day of the week.
			//This is because we are going to act like we are pasting over the week that dateSched falls in.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=dateSched.AddDays(-7),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-8),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-9),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-10),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-11),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-12),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-13),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			//It is only important that the start and end dates encompase dateSched and are not set to the same date.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched.AddDays(7));
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_SimpleCopyPasteWeekOverlap() {
			ScheduleT.ClearScheduleTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC-"+suffix,"ProvSched","Collider");
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc);
			//Make a schedule for last week that will purposefully collide with the start time (08:00 - 10:00).
			//It is extremely important that SchedDate falls on the same DayOfWeek as dateSched is set.
			//This is because we are going to act like we are pasting over the week that dateSched falls in.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=dateSched.AddDays(-7),//Make this schedule for a week prior to dateSched so that it falls on the same DayOfWeek.
					StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			//It is only important that the start and end dates encompase dateSched and are not set to the same date.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched.AddDays(-1),dateSched.AddDays(6));
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.AreEqual(provNumDoc,listOverlappingSchedProvNums[0]);//There should be no collisions whatsoever.
		}
		#endregion

		#region Complex (Specific Operatories)

		///<summary>Makes sure that our provider collision detection logic finds starting time collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_ComplexStartTimeOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Act like we are about to create a schedule for HYG that purposefully collides with the start time (08:00 - 10:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
				Ops=new List<long>() { op.OperatoryNum },
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>() { op.OperatoryNum });
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumHyg));
		}

		///<summary>Makes sure that our provider collision detection logic finds stopping time collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_ComplexStopTimeOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Look for a collision for a schedule for HYG that purposefully collides with the stop time (17:00).
			//Act like we are about to create a schedule that purposefully collides with the stop time (16:00 - 18:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,16,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
				Ops=new List<long>() { op.OperatoryNum },
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Validate that the collision for a schedule that purposefully collides with the stop time (17:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>() { op.OperatoryNum });
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumHyg));
		}

		///<summary>Makes sure that our provider collision detection logic finds engulfing collisions.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_ComplexEngulfOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Act like we are about to create a schedule for HYG that is purposefully engulfed within the start and stop time of our schedule (10:00 - 16:00).
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,16,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
				Ops=new List<long>() { op.OperatoryNum },
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>() { op.OperatoryNum });
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumHyg));
		}

		///<summary>Makes sure that our provider collision detection logic does not find collisions when there are no schedules to collide with.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_ComplexNoProvOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Create a bunch of schedules that are all over the place (even on different dates) that do not collide with each other.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Act like we are about to create a schedule that does not collide with any schedules that we made (17:30 - 18:00).
			Schedule schedOverlap =new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
				Ops=new List<long>() { op.OperatoryNum },
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>() { op.OperatoryNum });
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}

		///<summary>Makes sure that our provider collision detection logic DOES find collisions for other providers in the same operatory.
		///Meaning that two providers cannot have overlapping schedules in the same operatory.</summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNums_ComplexOtherProvOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Act like we are about to create a typical 09:00 - 17:00 schedule for the hygienist.
			Schedule schedOverlap=new Schedule() {
				SchedDate=dateSched,
				StartTime=new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),
				StopTime=new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),
				SchedType=ScheduleType.Provider,
				ProvNum=provNumHyg,
				IsNew=true,
				Ops=new List<long>() { op.OperatoryNum },
			};
			//Get all provider schedules from the database for the date in question (should only be our one schedule).
			List<Schedule> listSchedulesForDate=Schedules.GetDayList(dateSched).FindAll(x => x.SchedType==ScheduleType.Provider);
			//Look for a collision for a schedule that purposefully collides within the start and stop times (09:00 - 17:00).
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNums(
				new List<long>() { provNumHyg },
				schedOverlap,
				listSchedulesForDate,
				new List<long>() { op.OperatoryNum });
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumHyg));
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteDayNoOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Create a bunch of schedules that are all over the place (even on different dates) that do not collide with each other.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for today which will not collide with any schedules that we made for this provider (17:30 - 18:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumHyg,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteDayOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for HYG that purposefully collides with the start time (08:00 - 10:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumHyg,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumHyg));
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteDayOverlapReplacing() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Create a bunch of schedules that are all over the place (even on different dates).
			//Make sure to create a schedule for DOC that will explicitly overlap the 17:30 - 18:00 schedule.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			//This is the schedule that will purposefully overlap with the 17:30 - 18:00 schedule.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for today which will collide with one schedule that was made for this provider (17:30 - 18:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumHyg,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched
				,listIgnoreProvNums:new List<long>() { provNumDoc });
			//There should be no collisions because we told the above method that we are replacing existing schedules for DOC, so the collision is skipped.
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteDayOverlapNotReplacing() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for DOC.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Create a bunch of schedules that are all over the place (even on different dates).
			//Make sure to create a schedule for DOC that will explicitly overlap the 17:30 - 18:00 schedule.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,5,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,8,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,18,05,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,20,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(-1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumHyg
				,listOpNums: new List<long>() { op.OperatoryNum });
			//This is the schedule that will purposefully overlap with the 17:30 - 18:00 schedule.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,30,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for today which will collide with one schedule that was made for this provider (17:30 - 18:00).
			//It doesn't matter what the SchedDate is set to because we are going to act like we are pasting this day over dateSched.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=DateTime.Now,
					StartTime=new TimeSpan(new DateTime(1,1,1,17,30,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumHyg,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);//There should be one collision.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteWeekNoOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider for 7 straight days.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(2),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(3),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(4),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(5),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(6),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for an entire week that will not collide with any times in the week above (04:00 - 8:00).
			//It is extremely important that we create a schedule for each day of the week.
			//This is because we are going to act like we are pasting over the week that dateSched falls in.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=dateSched.AddDays(-7),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-8),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-9),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-10),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-11),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-12),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-13),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			//It is only important that the start and end dates encompase dateSched and are not set to the same date.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched.AddDays(7));
			Assert.AreEqual(0,listOverlappingSchedProvNums.Count);//There should be no collisions whatsoever.
		}
		
		///<summary></summary>
		[TestMethod]
		public void Schedules_GetOverlappingSchedProvNumsForRange_ComplexCopyPasteWeekOverlap() {
			ScheduleT.ClearScheduleTable();
			OperatoryT.ClearOperatoryTable();
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumDoc=ProviderT.CreateProvider("DOC","DocCollider",suffix);
			long provNumHyg=ProviderT.CreateProvider("HYG","HygCollider",suffix);
			Operatory op=OperatoryT.CreateOperatory("OP1",suffix);
			DateTime dateSched=DateTime.Now.AddDays(5);//Random date in the future.
			//Create a typical 09:00 - 17:00 schedule for the provider for 7 straight days.
			ScheduleT.CreateSchedule(dateSched,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(1),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(2),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(3),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(4),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(5),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(dateSched.AddDays(6),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDoc
				,listOpNums: new List<long>() { op.OperatoryNum });
			//Make a schedule for an entire week where one will collide with above times (08:00 - 10:00).
			//It is extremely important that we create a schedule for each day of the week.
			//This is because we are going to act like we are pasting over the week that dateSched falls in.
			List<Schedule> listSchedules=new List<Schedule>() {
				new Schedule() {
					SchedDate=dateSched.AddDays(-7),
					StartTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-8),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-9),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-10),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-11),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-12),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
				new Schedule() {
					SchedDate=dateSched.AddDays(-13),
					StartTime=new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),
					StopTime=new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks),
					SchedType=ScheduleType.Provider,
					ProvNum=provNumDoc,
					IsNew=true,
					Ops=new List<long>() { op.OperatoryNum },
				},
			};
			//Validate that the collision for a schedule that purposefully collides with the start time (09:00) is detected.
			List<long> listOverlappingSchedProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listSchedules,dateSched,dateSched);
			Assert.AreEqual(1,listOverlappingSchedProvNums.Count);
			Assert.IsTrue(listOverlappingSchedProvNums.Contains(provNumDoc));
		}
		#endregion

		#endregion

		#region Copy Blockouts

		///<summary>This test copies blockouts for a single day and repeats them for 10 days starting on the day after. Does not include weekends.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_SingleDayRepeat10Days() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create a single blockout from 9-12
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),TimeSpan.FromHours(9),TimeSpan.FromHours(12),
				schedType:ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,10,29).AddDays(1);
			DateTime dateCopyTo=dateCopyFrom;
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,false,false,true,new DateTime(2018,10,29),new DateTime(2018,10,29),dateCopyFrom,dateCopyTo,10);
			//Get all blockouts for the time frame that was copied to. Get three weeks of data to ensure all 10 are there.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(10,listBlockouts.Count);
			Assert.IsTrue(!listBlockouts.Any(x => ListTools.In(x.SchedDate.DayOfWeek,DayOfWeek.Saturday,DayOfWeek.Sunday)));
		}

		///<summary>This test copies blockouts for a single day and repeats them for 10 days starting on the day after. We are not replacing the
		///existing blockouts and an overlap exists. Should insert blockouts up to the blockout that is found to be overlapping.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_SingleDayRepeat10DaysOverlapExists() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create a single blockout from 9-12
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Create an overlapping blockout from 9-12 five days from now.
			ScheduleT.CreateSchedule(new DateTime(2018,10,29).AddDays(5),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),
				new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),schedType: ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,10,30);
			DateTime dateCopyTo=dateCopyFrom;
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,false,true,false,new DateTime(2018,10,29),new DateTime(2018,10,29),dateCopyFrom,dateCopyTo,10);
			//Get all blockouts for the time frame that was copied to. Get three weeks of data to ensure all are gotten.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreNotEqual("",errors);
			//5 will be within the list as the first four will be inserted fine. After that, the error will occur and the process will stop. 
			//The overlapping blockout will remain.
			Assert.AreEqual(5,listBlockouts.Count);
		}

		///<summary>This test copies blockouts for a single day and repeats them for 10 days starting on the day after. This includes weekends.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_SingleDayRepeat10DaysIncludeWeekends() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create a single blockout from 9-12
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom= new DateTime(2018,10,30);
			DateTime dateCopyTo=dateCopyFrom;
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,false,true,true,new DateTime(2018,10,29),new DateTime(2018,10,29),dateCopyFrom,dateCopyTo,10);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(10),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(10,listBlockouts.Count);
			//As we are including weekends and repeating for 10 days, we should have at least one Saturday and Sunday.
			Assert.IsTrue(listBlockouts.Any(x => x.SchedDate.DayOfWeek==DayOfWeek.Saturday));
			Assert.IsTrue(listBlockouts.Any(x => x.SchedDate.DayOfWeek==DayOfWeek.Sunday));
		}

		///<summary>This test copies blockouts for a single day and repeats them for 10 days starting on the day after. This does not include weekends.
		///This test ensures no weekend blockouts are deleted when doReplace is set to true.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_SingleDayRepeat10DaysNoWeekendsDeleted() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create a single blockout from 9-12
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Create a blockout on Saturday to ensure it still exists after we call copy blockouts
			ScheduleT.CreateSchedule(new DateTime(2018,11,3),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,10,30);//Tuesday
			DateTime dateCopyTo=dateCopyFrom;
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,false,false,true,new DateTime(2018,10,29),new DateTime(2018,10,29),dateCopyFrom,dateCopyTo,10);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(11,listBlockouts.Count);
			Assert.IsTrue(listBlockouts.Any(x => x.SchedDate==new DateTime(2018,11,3)));//Make sure the Saturday was not deleted.
		}

		///<summary>This test copies blockouts for a single day and repeats them for 10 days starting on the day after. This tests to ensure blockouts 
		///that would be overlapping these blockouts are removed when doReplace is set.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_SingleDayRepeat10DaysRemoveOverlap() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create a single blockout from 9-12
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Create a blockout on Wednesday to ensure it is removed.
			ScheduleT.CreateSchedule(new DateTime(2018,10,31),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,10,30);//Tuesday
			DateTime dateCopyTo=dateCopyFrom;
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,false,false,true,new DateTime(2018,10,29),new DateTime(2018,10,29),dateCopyFrom,dateCopyTo,10);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(10,listBlockouts.Count);//This may fail if the blockout was not removed.
		}

		///<summary>This test copies blockouts for a week and repeats them for 3 weeks starting on the week after.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_WeekRepeat3DoNotIncludeWeekends() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create 2 blockouts on Monday and Wednesday.
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(new DateTime(2018,10,31),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,11,5);
			DateTime dateCopyTo=new DateTime(2018,11,9);
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,true,false,true,new DateTime(2018,10,29),new DateTime(2018,11,2),dateCopyFrom,dateCopyTo,3);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(6,listBlockouts.Count);
			Assert.IsTrue(!listBlockouts.Any(x => ListTools.In(x.SchedDate.DayOfWeek,DayOfWeek.Saturday,DayOfWeek.Sunday)));
		}

		///<summary>This test copies blockouts for a week and repeats them for 3 weeks starting on the week after. doReplace is true. This ensures
		///weekend blockouts are not deleted.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_WeekRepeat3NoWeekendsDeleted() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create 2 blockouts on Monday and Wednesday.
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(new DateTime(2018,10,31),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Create a single blockout on Saturday
			ScheduleT.CreateSchedule(new DateTime(2018,11,10),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,11,5);
			DateTime dateCopyTo=new DateTime(2018,11,9);
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,true,false,true,new DateTime(2018,10,29),new DateTime(2018,11,2),dateCopyFrom,dateCopyTo,3);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(7,listBlockouts.Count);//6 copied plus one on Saturday
			Assert.IsTrue(listBlockouts.Any(x => x.SchedDate.DayOfWeek==DayOfWeek.Saturday));
		}

		///<summary>This test copies blockouts for a week and repeats them for 3 weeks starting on the week after. We are not replacing the
		///existing blockouts and an overlap exists.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_WeekRepeat3OverlapExists() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create 2 blockouts on Monday and Wednesday.
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(new DateTime(2018,10,31),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			//Create an overlapping blockout on the third monday that would be copied.
			ScheduleT.CreateSchedule(new DateTime(2018,11,19),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,11,5);
			DateTime dateCopyTo=new DateTime(2018,11,9);
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,true,false,false,new DateTime(2018,10,29),new DateTime(2018,11,2),dateCopyFrom,dateCopyTo,3);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreNotEqual("",errors);
			Assert.AreEqual(5,listBlockouts.Count);//Count of 5 as the last week wasn't copied. Overlapping blockout remains.
			Assert.IsTrue(!listBlockouts.Any(x => ListTools.In(x.SchedDate.DayOfWeek,DayOfWeek.Saturday,DayOfWeek.Sunday)));//No weekends.
		}

		///<summary>This test copies blockouts for a week and repeats them for 3 weeks starting on the week after including weekends.</summary>
		[TestMethod]
		public void Schedules_CopyBlockouts_WeekRepeat3IncludeWeekends() {
			//Create Operatory
			Operatory op=OperatoryT.CreateOperatory("OP1");
			//Create 3 blockouts on Monday,Wednesday and Saturday.
			ScheduleT.CreateSchedule(new DateTime(2018,10,29),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType:ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(new DateTime(2018,10,31),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums:new List<long>() { op.OperatoryNum });
			ScheduleT.CreateSchedule(new DateTime(2018,11,3),new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),
				schedType: ScheduleType.Blockout,listOpNums: new List<long>() { op.OperatoryNum });
			//Set up appointment view
			ApptView aptView=ApptViewT.SetApptView(new List<long> { op.OperatoryNum });
			DateTime dateCopyFrom=new DateTime(2018,11,5);
			DateTime dateCopyTo=new DateTime(2018,11,11);
			//Copy blockouts
			string errors=CopyBlockouts(aptView.ApptViewNum,true,false,true,new DateTime(2018,10,29),new DateTime(2018,11,4),dateCopyFrom,dateCopyTo,3);
			//Get all blockouts for the time frame that was copied to.
			List<Schedule> listBlockouts=Schedules.RefreshPeriodBlockouts(dateCopyFrom,dateCopyTo.AddDays(21),new List<long> { op.OperatoryNum });
			Assert.AreEqual("",errors);
			Assert.AreEqual(9,listBlockouts.Count);
			//As we are including weekends, we should have 3 saturday blockouts.
			Assert.AreEqual(3,listBlockouts.Count(x => x.SchedDate.DayOfWeek==DayOfWeek.Saturday));
		}

		///<summary>This test checks the correct schedules are returned when paring down lists of provider, operatory, and clinic numbers.  </summary>
		[TestMethod]
		public void Schedules_GetSchedulesHelper_CorrectSchedulesReturned() {
			//setup objects for test
			List<int> listSchedTypes=new List<int>() { (int)ScheduleType.Provider,(int)ScheduleType.Blockout };
			Clinic clinic1=ClinicT.CreateClinic("Clinic1");
			Clinic clinic2=ClinicT.CreateClinic("Clinic2");  //not included in listClinics
			List<long> listClinicNums=new List<long>(){clinic1.ClinicNum};
			long provNum1=ProviderT.CreateProvider("Prov1"); //not included in listProvNums
			long provNum2=ProviderT.CreateProvider("Prov2");
			long provNum3=ProviderT.CreateProvider("Prov3");
			long provNum4=ProviderT.CreateProvider("Prov4");
			List<long> listProvNums=new List<long>(){provNum2,provNum3,provNum4,0}; //list will always contain 0
			Operatory op1=OperatoryT.CreateOperatory("OP1",provDentist:provNum1,clinicNum:clinic1.ClinicNum);
			Operatory op2=OperatoryT.CreateOperatory("OP2",provHygienist:provNum2,clinicNum:clinic1.ClinicNum);
			Operatory op3=OperatoryT.CreateOperatory("OP3",provHygienist:provNum3,clinicNum:clinic1.ClinicNum); //not included in listOpNums
			Operatory op4=OperatoryT.CreateOperatory("OP4",provDentist:provNum4,clinicNum:clinic2.ClinicNum);
			List<long> listOpNums=new List<long>(){op1.OperatoryNum,op2.OperatoryNum,op4.OperatoryNum};
			//create a one-day, one-hour schedule for each operatory
			Schedule sched1=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,clinicNum:clinic1.ClinicNum,provNum:provNum1,listOpNums:new List<long>() {op1.OperatoryNum });
			Schedule sched2=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,clinicNum:clinic1.ClinicNum,provNum:provNum2,listOpNums:new List<long>() {op2.OperatoryNum });
			Schedule sched3=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,clinicNum:clinic1.ClinicNum,provNum:provNum3,listOpNums:new List<long>() {op3.OperatoryNum });
			Schedule sched4=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,clinicNum:clinic2.ClinicNum,provNum:provNum4,listOpNums:new List<long>() {op4.OperatoryNum });
			Schedules.Update(sched1);
			Schedules.Update(sched2);
			Schedules.Update(sched3);
			Schedules.Update(sched4);
			List<Schedule> returnedSchedules= Schedules.GetSchedulesHelper(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),listClinicNums,listOpNums,listProvNums,null,listSchedTypes);
			//Should return a single schedule for prov2 in op2 at clinic1. This is sched2.
			Assert.AreEqual(1,returnedSchedules.Where(x => x.ScheduleNum==sched2.ScheduleNum).Count());
		}


		///<summary>Calls Schedules.CopyBlockouts.</summary>
		private string CopyBlockouts(long apptViewNum,bool isWeek,bool includeWeekend,bool doReplace,DateTime dateSelectedStart,
			DateTime dateSelectedEnd,DateTime dateCopyStart,DateTime dateCopyEnd,int numRepeat) 
		{
			return Schedules.CopyBlockouts(apptViewNum,isWeek,includeWeekend,doReplace,dateSelectedStart,dateSelectedEnd,dateCopyStart,dateCopyEnd,
				numRepeat);
		}

		#endregion
	}
}
