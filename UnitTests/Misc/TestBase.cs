using System;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System.Collections.Generic;
using UnitTestsCore;
using System.Reflection;
using System.Data;

namespace UnitTests {
	[TestClass]
	public abstract class TestBase {

		protected const string UnitTestUserName="UnitTest";
		protected const string UnitTestPassword="password";
		public const string TestEmaiAddress="opendentalunittests@gmail.com";
		public const string TestEmaiPwd="welovetesting!";
		///<summary>This is the OD test department's Google Voice number. See [[eService Regression Test Database Setup]]. 
		///Linked to email eServices.od.test@gmail.com.</summary>
		public const string TestPatPhone="1(971)301-2247";
		public TestContext TestContext { get; set; }
		protected Logger.IWriteLine _log;
		///<summary>Put this in the watch window to see the logger results.</summary>
		protected string _logStr="";
		///<summary>Indicates whether the user wanted to run all tests against Middle Tier.</summary>
		private static bool _runAllAgainstMiddleTier=false;

		public static string UnitTestDbName {
			get {
				Version versionBusiness=Assembly.GetAssembly(typeof(PatientStatus)).GetName().Version;
				return "unittest"+versionBusiness.Major+versionBusiness.Minor;
			}
		}

		public TestBase() {
			_log=new LogDelegate((log,ll) => {
				_logStr+=log+"\r\n";
				//To view the output, in the Test Explorer in the bottom half, click 'Output'.
				Console.WriteLine(log);
			});
		}

		///<summary>Write to _logStr and the test output window.</summary>
		protected void LogWriteLine(string log) {
			_log.WriteLine(log,LogLevel.Verbose);
		}

		/// <summary> TestBase.Initialize will be called before the ClassInitialize and TestInitialize methods specific to each class.
		/// Do this first so that the time the Initialize and ClassInitialize methods take doesn't get counted in the test times. </summary>
		[AssemblyInitialize]
		public static void Initialize(TestContext context) {
			ODInitialize.IsRunningInUnitTest=true;//Causes FormFriendlyException to throw rather than displaying a MessageBox.
			ODInitialize.Initialize();
			if(!UnitTestsCore.DatabaseTools.SetDbConnection(UnitTestDbName,"localhost","3306","root","",false)) {//Put this in a config file in the future.
				UnitTestsCore.DatabaseTools.SetDbConnection("","localhost","3306","root","",false);
				DatabaseTools.FreshFromDump("localhost","3306","root","",false);//this also sets database to be unittest.
			}
			else {
				//Clear the database before running the unittests (instead of after) for two reasons
				//1- if the cleanup is done using [TestCleanup], the cleanup will not be run if the user cancels in the middle of a test while debugging
				//2- if a test fails, we may want to look at the data in the db to see why it failed.
				UnitTestsCore.DatabaseTools.ClearDb();
			}
			if(!ODBuild.IsDebug()) {
				throw new Exception("Active solution configuration must be set to Debug for running unit tests.");
			}
			CreateAndSetUnitTestUser();
			RevertMiddleTierSettingsIfNeeded();//Set initial connection
		}

		[TestInitialize]
		public void ResetTest() {
			PrefT.RevertPrefChanges();
		}

		protected static void RunTestsAgainstMiddleTier() {
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS());
		}

		protected static void RunTestsAgainstMiddleTier(OpenDentBusiness.WebServices.OpenDentalServerMockIIS mockServer) {
			RemotingClient.RemotingRole=RemotingRole.ClientWeb;
			OpenDentBusiness.WebServices.OpenDentalServerProxy.MockOpenDentalServerCur=mockServer;
		}

		protected static void RunTestsAgainstDirectConnection() {
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			OpenDentBusiness.WebServices.OpenDentalServerProxy.MockOpenDentalServerCur=null;
		}

		protected static void RevertMiddleTierSettingsIfNeeded() {
			//If they want to run all tests against middle tier, do not revert.
			if(_runAllAgainstMiddleTier) {
				RunTestsAgainstMiddleTier();//Reset settings to default.
				return;
			}
			RunTestsAgainstDirectConnection();
		}

		public static EmailAddress InsertEmailAddress() {
			EmailAddress email=new EmailAddress() {
				SenderAddress=TestEmaiAddress,
				EmailUsername=TestEmaiAddress,
				EmailPassword=MiscUtils.Encrypt(TestEmaiPwd),
				ServerPort=587,
				UseSSL=true,
				Pop3ServerIncoming="pop.gmail.com",
				ServerPortIncoming=110,
				SMTPserver="smtp.gmail.com",
				UserNum=0,
				WebmailProvNum=0,
			};
			EmailAddresses.Insert(email);
			return email;
		}

		public static void CreateUnitTestUser() {
			if(Userods.GetUserByName(UnitTestUserName,false)==null) {
				UserodT.CreateUser(UnitTestUserName,UnitTestPassword,new List<long> { 1 });
			}
		}

		public static void CreateAndSetUnitTestUser() {
			CreateUnitTestUser();
			//Get the Admin user, should always exist
			Security.CurUser=Userods.GetUserByName(UnitTestUserName,false);
			Security.PasswordTyped=UnitTestPassword;//For middle tier unit tests.
		}

		///<summary>Creates a database and a preference table so that DataConnection can pass our arbitrary test query.</summary>
		public static void CreateDatabaseIfNeeded(string databaseName) {
			string command=$"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{POut.String(databaseName)}'";
			DataTable table=DataCore.GetTable(command);
			//Create a custom database if needed.
			if(table.Rows.Count==0) {
				DataCore.NonQ($"CREATE DATABASE `{POut.String(databaseName)}` CHARACTER SET utf8");
				DataCore.NonQ($@"CREATE TABLE `{POut.String(databaseName)}`.`preference` (
						`PrefName` varchar(255) NOT NULL DEFAULT '',
						`ValueString` text NOT NULL,
						`PrefNum` bigint(20) NOT NULL AUTO_INCREMENT,
						`Comments` text,
						PRIMARY KEY (`PrefNum`)
					) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8");
			}
		}

		public static void DropDatabase(string databaseName) {
			ODException.SwallowAnyException(() => DataCore.NonQ($"DROP DATABASE `{POut.String(databaseName)}`"));
		}

		///<summary>Executes the createTableStatement passed in if the table does not exists in the current database context.</summary>
		public static void CreateTableIfNeeded(string tableName,string createTableStatement) {
			string command=$@"SELECT * FROM INFORMATION_SCHEMA.TABLES
				WHERE TABLE_SCHEMA = DATABASE()
				AND TABLE_NAME = '{POut.String(tableName)}'
				LIMIT 1";
			DataTable table=DataCore.GetTable(command);
			if(table.Rows.Count==0) {
				DataCore.NonQ(createTableStatement);
			}
		}
	}
}
