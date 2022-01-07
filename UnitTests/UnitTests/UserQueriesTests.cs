using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.UserQueries_Tests {
	[TestClass]
	public class UserQueriesTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
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
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		public static List<string> CreateList(params string[] list) {
			List<string> retVal=new List<string>();
			foreach(string str in list) {
				retVal.Add(str);
			}
			return retVal;
		}

		private static List<string> SplitQuery(string strQuery,string strSplit=";") {
			List<string> stmts = UserQueries.SplitQuery(strQuery,false,strSplit);
			UserQueries.TrimList(stmts);
			stmts.RemoveAll(x => string.IsNullOrEmpty(x));
			return stmts;
		}

		/// <summary>Asserts that UserQueries.SplitQuery() splits strQuery up into a list of strings that matches listExpectedStrings.</summary>
		/// <param name="strQuery">Provide a query that will be split up.</param>
		/// <param name="listExpectedStrings">Provide the list of strings that strQuery should get split up into.</param>
		private static void RunSplitQueryTest(string strQuery,List<string> listExpectedStrings) {
			List<string> listSplitQueries=SplitQuery(strQuery);
			foreach(string strSplit in listSplitQueries) {
				Assert.IsTrue(listExpectedStrings.Contains(strSplit));
			}
		}

		/// <summary>Assert.True UserQueries.ParseSetStatements() matches listExpectedStrings.</summary>
		/// <param name="strQuery">Provide a query that will be split up.</param>
		/// <param name="listExpectedStrings">Provide the list of strings that strQuery should get split up into.</param>
		private static void RunParseSetStatementsTest(string strQuery,List<string> listExpectedStrings) {
			List<string> listSplitQueries=UserQueries.ParseSetStatements(strQuery);
			foreach(string strSplit in listSplitQueries) {
				Assert.IsTrue(listExpectedStrings.Contains(strSplit));
			}
		}

		[TestMethod]
		public void UserQueries_SplitQuery_CaseStatement() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_CaseStatementMissingEnd() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 ELSE 10 END)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_MoreThanOneSetStmt() {
			string strQuery=@"SET @FromDate='2012-12-03' , @ToDate='2016-12-03';
			SET @CarrierName='CSC Medicaid'; 
			SET @OperatoryName='%%';
			SELECT * FROM patient;";
			RunSplitQueryTest(strQuery,CreateList("SET @FromDate='2012-12-03' , @ToDate='2016-12-03'","SET @CarrierName='CSC Medicaid'",
				"SET @OperatoryName='%%'","SELECT * FROM patient"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_MultiCaseStatement() {
			string strQuery=@"SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 WHEN CURDATE()='2019-05-20' THEN 55 ELSE 10 END);
				SELECT @Test;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=(CASE WHEN CURDATE()='2019-03-20' THEN 1 WHEN CURDATE()='2019-05-20' THEN 55 ELSE 10 END)","SELECT @Test"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_OneSetStmt() {
			string strQuery=@"SET @ExcAllergy='%None%'; 
				SELECT * FROM allergydef";
			RunSplitQueryTest(strQuery,CreateList("SET @ExcAllergy='%None%'","SELECT * FROM allergydef"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_CommandInFunction() {
			string strQuery=@"SET @Test=DATE_SUB(CURDATE(), INTERVAL 30 DAY);
				SET @End=CURDATE();
				SELECT @Test, @End;";
			RunSplitQueryTest(strQuery,CreateList("SET @Test=DATE_SUB(CURDATE(), INTERVAL 30 DAY)","SET @End=CURDATE()","SELECT @Test, @End"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_SetStmtMissingQuote() {
			string strQuery=@"SET @FromDate='2012-12-03 , @ToDate=2016-12-03';
				SET @FromDate2='2012-12-03 , @ToDate2=2016-12-03;
				SET @CarrierName='CSC Medicaid'; 
				SET @OperatoryName='%%';
				SELECT * FROM patient;";
			RunParseSetStatementsTest(strQuery,CreateList("SET @FromDate='2012-12-03 , @ToDate=2016-12-03'","SET @CarrierName='CSC Medicaid'",
				"SET @OperatoryName='%%'","SELECT * FROM patient","SET @FromDate2='2012-12-03 , @ToDate2=2016-12-03"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_SetStmtWithCommaInside() {
			string strQuery=@"SET @LName='Gonz,lez', @FName='Saul';
				SELECT * FROM patient WHERE LName LIKE @LName";
			RunParseSetStatementsTest(strQuery,CreateList("SET @LName='Gonz,lez', @FName='Saul'"));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_QuoteInsideFunction() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=@"SELECT REPLACE(p.email,"";"","","") FROM patient;";
			Assert.IsTrue(Db.IsSqlAllowed(command,true));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_QuoteInsideSeparator() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=@"SELECT GROUP_CONCAT(p.email SEPARATOR "";""),p.* FROM patient p GROUP BY p.PatNum;";
			Assert.IsTrue(Db.IsSqlAllowed(command,true));
		}

		private void SetCurrentUserWOCommandQueryPerm(string userGroupName) {
			long group1=UserGroupT.CreateUserGroup(userGroupName);
			Userod userWOCommandPerm=UserodT.CreateUser(MethodBase.GetCurrentMethod().Name+DateTime.Now.Ticks,userGroupNumbers:new List<long>() {group1 });
			Security.CurUser=userWOCommandPerm;
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_NotAllowed() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=$@"SELECT GROUP_CONCAT(p.email SEPARATOR "";""),p.* FROM patient p GROUP BY p.PatNum;UPDATE patient p SET p.Email=""saul@opendental.com""
				WHERE p.PatNum=10;";
			Assert.IsFalse(Db.IsSqlAllowed(command,true));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_NotAllowedSplitWithDashes() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=$@"SELECT GROUP_CONCAT(p.email SEPARATOR ""-;-;""),p.* FROM patient p GROUP BY p.PatNum; "+
				@"UPDATE patient p SET p.Email=""saul@opendental.com"" WHERE p.PatNum=10;";
			Assert.IsFalse(Db.IsSqlAllowed(command,true));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_IsAllowedSplitWithDashes() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=$@"SELECT GROUP_CONCAT(p.email SEPARATOR ""-;-;""),p.* FROM patient p GROUP BY p.PatNum;";
			Assert.IsTrue(Db.IsSqlAllowed(command,true));
		}

		[TestMethod]
		public void UserQueries_SplitQuery_IsSQLAllowed_IsAllowedSemiColonInQuotes() {
			SetCurrentUserWOCommandQueryPerm($"usergroup_{MethodBase.GetCurrentMethod()}");
			string command=$@"SELECT ';',PatNum FROM patient";
			Assert.IsTrue(Db.IsSqlAllowed(command,true));
		}

	}
}
