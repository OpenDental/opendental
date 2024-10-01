using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;

namespace UnitTests.ChildDaycare_Tests {
	[TestClass]
	public class ChildDaycareTests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_ZeroChildrenUnderTwo() {
			int teachersRequired;
			//Test 1-10
			for(int i=1;i<11;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:0);
				Assert.AreEqual(1,teachersRequired);
			}
			//Test 11-16
			for(int i=11;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:0);
				Assert.AreEqual(2,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_ZeroChildren() {
			int teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:0,childrenUnderTwo:0);
			Assert.AreEqual(0,teachersRequired);
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_OverSixteenChildren() {
			int teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:17,childrenUnderTwo:0);
			Assert.AreEqual(-1,teachersRequired);
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_OneChildUnderTwo() {
			int teachersRequired;
			//Test 1-8
			for(int i=1;i<9;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:1);
				Assert.AreEqual(1,teachersRequired);
			}
			//Test 9-16
			for(int i=9;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:1);
				Assert.AreEqual(2,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_TwoChildUnderTwo() {
			int teachersRequired;
			//Test 1-7
			for(int i=1;i<8;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:2);
				Assert.AreEqual(1,teachersRequired);
			}
			//Test 8-16
			for(int i=8;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:2);
				Assert.AreEqual(2,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_ThreeChildUnderTwo() {
			int teachersRequired;
			//Test 1-6
			for(int i=1;i<6;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:3);
				Assert.AreEqual(1,teachersRequired);
			}
			//Test 7-16
			for(int i=7;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:3);
				Assert.AreEqual(2,teachersRequired);
			}
		}


		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_FourChildUnderTwo() {
			int teachersRequired;
			//Test 1-14
			for(int i=1;i<15;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:4);
				Assert.AreEqual(2,teachersRequired);
			}
			//Test 15-16
			for(int i=15;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:4);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_FiveChildUnderTwo() {
			int teachersRequired;
			//Test 1-12
			for(int i=1;i<13;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:5);
				Assert.AreEqual(2,teachersRequired);
			}
			//Test 13-16
			for(int i=13;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:5);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_SixChildUnderTwo() {
			int teachersRequired;
			//Test 1-11
			for(int i=1;i<12;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:6);
				Assert.AreEqual(2,teachersRequired);
			}
			//Test 12-16
			for(int i=12;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:6);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_SevenChildUnderTwo() {
			int teachersRequired;
			//Test 1-10
			for(int i=1;i<11;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:7);
				Assert.AreEqual(2,teachersRequired);
			}
			//Test 11-16
			for(int i=11;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:7);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_EightChildUnderTwo() {
			int teachersRequired;
			//Test 1-8
			for(int i=1;i<9;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:8);
				Assert.AreEqual(2,teachersRequired);
			}
			//Test 9-16
			for(int i=9;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:8);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_NineChildUnderTwo() {
			int teachersRequired;
			//Test 1-16
			for(int i=1;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:9);
				Assert.AreEqual(3,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_TenChildUnderTwo() {
			int teachersRequired;
			//Test 1-15
			for(int i=1;i<16;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:10);
				Assert.AreEqual(3,teachersRequired);
			}
			//Test 16
			teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:16,childrenUnderTwo:10);
			Assert.AreEqual(4,teachersRequired);
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_ElevenChildUnderTwo() {
			int teachersRequired;
			//Test 1-14
			for(int i=1;i<15;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:11);
				Assert.AreEqual(3,teachersRequired);
			}
			//Test 15-16
			for(int i=15;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:11);
				Assert.AreEqual(4,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_TwelveChildUnderTwo() {
			int teachersRequired;
			//Test 1-12
			for(int i=1;i<13;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:12);
				Assert.AreEqual(3,teachersRequired);
			}
			//Test 13-16
			for(int i=15;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:12);
				Assert.AreEqual(4,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_ThirteenChildUnderTwo() {
			int teachersRequired;
			//Test 1-16
			for(int i=1;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:13);
				Assert.AreEqual(4,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_FourteenChildUnderTwo() {
			int teachersRequired;
			//Test 1-16
			for(int i=1;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:14);
				Assert.AreEqual(4,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_FifteenChildUnderTwo() {
			int teachersRequired;
			//Test 1-16
			for(int i=1;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:15);
				Assert.AreEqual(4,teachersRequired);
			}
		}

		[TestMethod]
		public void ChildRoomLogs_GetNumberTeachersMixed_SixteenChildUnderTwo() {
			int teachersRequired;
			//Test 1-16
			for(int i=1;i<17;i++) {
				teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:i,childrenUnderTwo:16);
				Assert.AreEqual(4,teachersRequired);
			}
		}


	}
}
