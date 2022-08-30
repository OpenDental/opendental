using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;
using OpenDentBusiness.WebServices;
using System.Collections.Generic;

namespace UnitTests.Reports_Tests {
	[TestClass]
	public class ReportsTests:TestBase {
		private static OpenDentalServerMockIIS _middleTierMockOld=null;
		private const string USER_LOW_NAME="unittestuserlow";
		private const string USER_LOW_PASS="Password1";

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Create and set the Open Dental user called UnitTest so that we don't get trolled by failed login attempts from an invalid Security.CurUser.
			TestBase.CreateAndSetUnitTestUser();
			//Drop any users that already exist with this specific name.
			DbAdminMysql.DropUser(new DataConnection(),USER_LOW_NAME);
			//Create a new user with this unit test method name as the database user name.
			DataCore.NonQ($"CREATE USER '{USER_LOW_NAME}'@'localhost' IDENTIFIED BY '{USER_LOW_PASS}'");
			//Only give the SELECT permission to simulate a user of lower status in life.
			DataCore.NonQ($"GRANT SELECT ON *.* TO '{USER_LOW_NAME}'@'localhost'");
			//Reload all privileges to make sure the proletariat permission takes effect.
			DataCore.NonQ("FLUSH PRIVILEGES");
			//Preserve the old Middle Tier Mock service and replace it with a new one that knows about our new user low.
			_middleTierMockOld=OpenDentalServerProxy.MockOpenDentalServerCur;
			//Pass in new connection settings so that this plebeian is set as our "UserLow" when invoking Reports.GetTable().
			OpenDentalServerProxy.MockOpenDentalServerCur=new OpenDentalServerMockIIS("localhost"
				,UnitTestDbName
				,"root"
				,""
				,USER_LOW_NAME
				,USER_LOW_PASS
				,DatabaseType.MySql);
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Drop the user that this unit test class created within SetupClass().
			DbAdminMysql.DropUser(new DataConnection(),USER_LOW_NAME);
			//Put the Middle Tier mock server back to the way it was when this test class was initialized.
			OpenDentalServerProxy.MockOpenDentalServerCur=_middleTierMockOld;
		}

		[TestMethod]
		public void Reports_GetTable_MySqlUserLow() {
			Exception ex=null;
			//Spawn a new thread so that we don't manipulate any global DataConnection settings.
			ODThread thread=new ODThread(o => {
				//Prepare some simple queries to verify how both user and user low react.
				string tempTableName="tmpreportsunittesttable";
				string tempTableDropQuery=$"DROP TABLE IF EXISTS {tempTableName}";
				string tempTableCreateQuery=$"CREATE TABLE {tempTableName} (Col1 VARCHAR(1))";
				string tempTableShowQuery=$"SHOW TABLES LIKE '{tempTableName}'";
				//Make sure that we can create and drop tables as the normal user but cannot do the same thing via Reports.GetTable().
				//First, make sure that the regular user works as intended.
				DataCore.NonQ(tempTableDropQuery);
				DataCore.NonQ(tempTableCreateQuery);
				Assert.AreEqual(1,DataCore.GetTable(tempTableShowQuery).Rows.Count,"'root' user was not able to create a new table.");
				DataCore.NonQ(tempTableDropQuery);
				//Next, make sure that user low cannot create the new table.  Required to use the Middle Tier otherwise user low is ignored.
				DataAction.RunMiddleTierMock(() => {
					//User low should be able to run SELECT and SHOW commands.
					if(Reports.GetTable(tempTableShowQuery).Rows.Count!=0) {//Should have been dropped via root user above.
						throw new ApplicationException("Temporary table was not dropped correctly.");
					}
					//Reports.GetTable() should throw an exception due to the lack of the CREATE permission.  Swallow it.
					ODException.SwallowAnyException(() => Reports.GetTable(tempTableCreateQuery));
					//User low should not have been able to create the table.
					if(Reports.GetTable(tempTableShowQuery).Rows.Count!=0) {
						throw new ApplicationException("User low was able to create a table.");
					}
				});
			});
			thread.AddExceptionHandler(e => { ex=e; });//This will cause the unit test to fail.
			thread.Name="thread"+MethodBase.GetCurrentMethod().Name;
			thread.Start();
			thread.Join(Timeout.Infinite);
			Assert.IsNull(ex,ex?.Message);
		}

	}
}
