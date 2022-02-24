using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using Newtonsoft.Json;
using DataConnectionBase;
using System.Reflection;
using CodeBase;

namespace UnitTests.DataAction_Tests {
	[TestClass]
	public class DataActionTests:TestBase {
		private static readonly string _databaseNameBugsHQ=$"{UnitTestDbName}_bugshq";
		private static readonly string _databaseNameCustomersHQ=$"{UnitTestDbName}_customershq";
		private static readonly string _databaseNameManualPublisher=$"{UnitTestDbName}_manualpublisher";
		private static readonly string _databaseNameWebChat=$"{UnitTestDbName}_webchat";

		private static Dictionary<ConnectionNames,CentralConnection> _dictConnectionDefaults=new Dictionary<ConnectionNames,CentralConnection>() {
			#region Default Connections
			{
				ConnectionNames.BugsHQ,
				new CentralConnection(){
					ServerName="localhost",
					DatabaseName=_databaseNameBugsHQ,
					MySqlUser="root",
					MySqlPassword=""
				}
			},
			{
				ConnectionNames.CustomersHQ,
				new CentralConnection(){
					ServerName="localhost",
					DatabaseName=_databaseNameCustomersHQ,
					MySqlUser="root",
					MySqlPassword=""
				}
			},
			{
				ConnectionNames.ManualPublisher,
				new CentralConnection(){
					ServerName="localhost",
					DatabaseName=_databaseNameManualPublisher,
					MySqlUser="root",
					MySqlPassword=""
				}
			},
			{
				ConnectionNames.WebChat,
				new CentralConnection(){
					ServerName="localhost",
					DatabaseName=_databaseNameWebChat,
					MySqlUser="root",
					MySqlPassword=""
				}
			}
			#endregion
		};

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//The sitelink table is missing in general.  Create it if needed.
			CreateTableIfNeeded("sitelink",SiteLinkT.GetCreateTableStatement());
			//Make sure that the "missing in general" preference is present within the unit test database.
			if(!Prefs.GetContainsKey(nameof(PrefName.ConnectionSettingsHQ))) {
				JsonSerializerSettings settings=new JsonSerializerSettings();
				settings.DefaultValueHandling=DefaultValueHandling.Ignore;
				string connectionSettingsHQ=JsonConvert.SerializeObject(_dictConnectionDefaults,settings);
				DataCore.NonQ($@"INSERT INTO preference (PrefName,ValueString) 
					VALUES('{nameof(PrefName.ConnectionSettingsHQ)}','{connectionSettingsHQ}')");
				Prefs.RefreshCache();
			}
			//Create databases for all of the default connection settings.
			foreach(KeyValuePair<ConnectionNames,CentralConnection> keyValuePair in _dictConnectionDefaults) {
				CreateDatabaseIfNeeded(keyValuePair.Value.DatabaseName);
			}
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			DataAction.ClearDictHqCentralConnections();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
			//Delete all of the default databases.
			foreach(KeyValuePair<ConnectionNames,CentralConnection> keyValuePair in _dictConnectionDefaults) {
				DropDatabase(keyValuePair.Value.DatabaseName);
			}
		}

		[TestMethod]
		public void DataAction_GetHqConnections_Defaults() {
			Dictionary<ConnectionNames,CentralConnection> dictHqConnections=DataAction.GetHqConnections();
			Assert.IsNotNull(dictHqConnections);
			Assert.AreEqual(_dictConnectionDefaults.Count,dictHqConnections.Count);
			foreach(ConnectionNames connectionName in _dictConnectionDefaults.Keys) {
				Assert.IsTrue(AreConnsEqual(_dictConnectionDefaults[connectionName],dictHqConnections[connectionName]));
			}
		}

		[TestMethod]
		public void DataAction_RunBugsHQ_Default() {
			//Delete any sitelink entries so that there are no overrides in the database.
			SiteLinkT.ClearSiteLinkTable();
			string databaseName="";
			DataAction.RunBugsHQ(() => { databaseName=DataConnection.GetDatabaseName(); },useConnectionStore:false);
			Assert.AreEqual(_databaseNameBugsHQ,databaseName);
		}

		[TestMethod]
		public void DataAction_RunBugsHQ_Override() {
			SiteT.ClearSiteTable();
			SiteLinkT.ClearSiteLinkTable();
			Site site=SiteT.CreateSite(MethodBase.GetCurrentMethod().Name);
			SiteLink siteLink=SiteLinkT.CreateSiteLink(site.SiteNum,
				octetStart: ODEnvironment.GetDefaultGateway(),
				connectionSettingsHQOverrides: GetJsonSerializedConnectionOverride(ConnectionNames.BugsHQ,"127.0.0.1",_databaseNameBugsHQ,"root",""));
			string serverName="";
			DataAction.RunBugsHQ(() => { serverName=DataConnection.GetServerName(); },useConnectionStore: false);
			Assert.AreEqual("127.0.0.1",serverName);
			Assert.AreNotEqual(_dictConnectionDefaults[ConnectionNames.BugsHQ].ServerName,serverName);
		}

		[TestMethod]
		public void DataAction_RunCustomers_Default() {
			//Delete any sitelink entries so that there are no overrides in the database.
			SiteLinkT.ClearSiteLinkTable();
			string databaseName="";
			DataAction.RunCustomers(() => { databaseName=DataConnection.GetDatabaseName(); },useConnectionStore: false);
			Assert.AreEqual(_databaseNameCustomersHQ,databaseName);
		}

		[TestMethod]
		public void DataAction_RunCustomers_Override() {
			SiteT.ClearSiteTable();
			SiteLinkT.ClearSiteLinkTable();
			Site site=SiteT.CreateSite(MethodBase.GetCurrentMethod().Name);
			SiteLink siteLink=SiteLinkT.CreateSiteLink(site.SiteNum,
				octetStart: ODEnvironment.GetDefaultGateway(),
				connectionSettingsHQOverrides: GetJsonSerializedConnectionOverride(ConnectionNames.CustomersHQ,"127.0.0.1",_databaseNameCustomersHQ,"root",""));
			string serverName="";
			DataAction.RunCustomers(() => { serverName=DataConnection.GetServerName(); },useConnectionStore: false);
			Assert.AreEqual("127.0.0.1",serverName);
			Assert.AreNotEqual(_dictConnectionDefaults[ConnectionNames.CustomersHQ].ServerName,serverName);
		}

		[TestMethod]
		public void DataAction_ManualPublisherHQ_Default() {
			//Delete any sitelink entries so that there are no overrides in the database.
			SiteLinkT.ClearSiteLinkTable();
			string databaseName="";
			DataAction.RunManualPublisherHQ(() => { databaseName=DataConnection.GetDatabaseName(); },useConnectionStore: false);
			Assert.AreEqual(_databaseNameManualPublisher,databaseName);
		}

		[TestMethod]
		public void DataAction_ManualPublisherHQ_Override() {
			SiteT.ClearSiteTable();
			SiteLinkT.ClearSiteLinkTable();
			Site site=SiteT.CreateSite(MethodBase.GetCurrentMethod().Name);
			SiteLink siteLink=SiteLinkT.CreateSiteLink(site.SiteNum,
				octetStart: ODEnvironment.GetDefaultGateway(),
				connectionSettingsHQOverrides: GetJsonSerializedConnectionOverride(ConnectionNames.ManualPublisher,"127.0.0.1",_databaseNameManualPublisher,"root",""));
			string serverName="";
			DataAction.RunManualPublisherHQ(() => { serverName=DataConnection.GetServerName(); },useConnectionStore: false);
			Assert.AreEqual("127.0.0.1",serverName);
			Assert.AreNotEqual(_dictConnectionDefaults[ConnectionNames.ManualPublisher].ServerName,serverName);
		}

		[TestMethod]
		public void DataAction_WebChat_Default() {
			//Delete any sitelink entries so that there are no overrides in the database.
			SiteLinkT.ClearSiteLinkTable();
			string databaseName="";
			DataAction.RunWebChat(() => { databaseName=DataConnection.GetDatabaseName(); });
			Assert.AreEqual(_databaseNameWebChat,databaseName);
		}

		[TestMethod]
		public void DataAction_WebChat_Override() {
			SiteT.ClearSiteTable();
			SiteLinkT.ClearSiteLinkTable();
			Site site=SiteT.CreateSite(MethodBase.GetCurrentMethod().Name);
			SiteLink siteLink=SiteLinkT.CreateSiteLink(site.SiteNum,
				octetStart: ODEnvironment.GetDefaultGateway(),
				connectionSettingsHQOverrides: GetJsonSerializedConnectionOverride(ConnectionNames.WebChat,"127.0.0.1",_databaseNameWebChat,"root",""));
			string serverName="";
			DataAction.RunWebChat(() => { serverName=DataConnection.GetServerName(); });
			Assert.AreEqual("127.0.0.1",serverName);
			Assert.AreNotEqual(_dictConnectionDefaults[ConnectionNames.WebChat].ServerName,serverName);
		}

		private bool AreConnsEqual(CentralConnection con1, CentralConnection con2) {
			return (con1.ServerName.Equals(con2.ServerName)
				&& con1.DatabaseName.Equals(con2.DatabaseName)
				&& con1.MySqlUser.Equals(con2.MySqlUser)
				&& con1.MySqlPassword.Equals(con2.MySqlPassword));
		}

		private string GetJsonSerializedConnectionOverride(ConnectionNames connName,string serverName,string databaseName,string mySqlUser,
			string mySqlPassword)
		{
			Dictionary<ConnectionNames,CentralConnection> dictHqConnectionOverrides=new Dictionary<ConnectionNames,CentralConnection>() {
				{
					connName,
					new CentralConnection() {
						ServerName=serverName,
						DatabaseName=databaseName,
						MySqlUser=mySqlUser,
						MySqlPassword=mySqlPassword,
					}
				}
			};
			JsonSerializerSettings settings=new JsonSerializerSettings();
			settings.DefaultValueHandling=DefaultValueHandling.Ignore;
			return JsonConvert.SerializeObject(dictHqConnectionOverrides,settings);
		}

	}
}
