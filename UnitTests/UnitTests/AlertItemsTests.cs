using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.AlertItems_Tests {
	[TestClass]
	public class AlertItemsTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Clear out the alertitem table before every test.
			AlertItemT.ClearAlertItemTable();
		}
		
		public void AlertItems_CreateAlertsForWebmailMethodCall() {
			AlertItems.CreateAlertsForNewWebmail(_log);
		}

		/// <summary></summary>
		[TestMethod]
		public void AlertItems_CreateAlertsForNewWebmail() {
			//Test Sections:
			//Create 5 users, part of 2 providers.
			//Test adding an email for each provider, then clear alerts table.
			//Test adding 4 emails for each provider
			//Test adding 3 additional emails for 1 provider
			//Test marking 2 emails as read for 1 provider
			//Test marking all emails as read for 1 provider
			EmailMessageT.ClearEmailMessageTable();//Clear out the emailmessage table
			List<Userod> listTestUsers=new List<Userod>();
			//Create or reuse 5 users, and set their provnum to 1 or 2.  There'll be 3 provnum=1 and 2 provnum=2
			//In queries always filter by usernum because there may be users left over from other/old tests.
			for(int i=0;i<5;i++) { 
				Userod user=UserodT.CreateUser();
				user.ProvNum=i%2+1;
				listTestUsers.Add(user);
				Userods.Update(user);
			}
			listTestUsers=listTestUsers.Distinct().ToList();
			long examplePatnum=2;	//Patnum can be anything, needed for webmail.
			//Create one email for each provider.
			foreach(long provnum in listTestUsers.Select(x => x.ProvNum).Distinct()) {
				EmailMessageT.CreateWebMail(provnum,examplePatnum);
			}
			AlertItems_CreateAlertsForWebmailMethodCall();
			//Count the total # of alertitem entries, not what the description is.
			string alertCount=DataCore.GetScalar("SELECT COUNT(*) FROM alertitem WHERE UserNum IN ("+string.Join(",",listTestUsers.Select(x => POut.Long(x.UserNum)))
				+") AND Type="+POut.Int((int)AlertType.WebMailRecieved));
			Assert.AreEqual("5",alertCount);
			//
			//Clear out ALERT table and add some new emails
			AlertItemT.ClearAlertItemTable();
			foreach(long provnum in listTestUsers.Select(x => x.ProvNum).Distinct()) {
				EmailMessageT.CreateWebMail(provnum,examplePatnum);
				EmailMessageT.CreateWebMail(provnum,examplePatnum);
				EmailMessageT.CreateWebMail(provnum,examplePatnum);
				EmailMessageT.CreateWebMail(provnum,examplePatnum);
			}
			//This section tests adding more unread emails, and changing the description of the alertitem
			Userod selectedUser=listTestUsers.First();
			AlertItems_CreateAlertsForWebmailMethodCall();
			alertCount=DataCore.GetScalar("SELECT Description FROM alertitem WHERE Type="+POut.Int((int)AlertType.WebMailRecieved)+" AND UserNum="+selectedUser.UserNum);
			Assert.AreEqual("5",alertCount);
			//
			//Add 3 more unread emails.
			EmailMessageT.CreateWebMail(selectedUser.ProvNum,examplePatnum);
			EmailMessageT.CreateWebMail(selectedUser.ProvNum,examplePatnum);
			EmailMessageT.CreateWebMail(selectedUser.ProvNum,examplePatnum);
			AlertItems_CreateAlertsForWebmailMethodCall();
			alertCount=DataCore.GetScalar("SELECT Description FROM alertitem WHERE Type="+POut.Int((int)AlertType.WebMailRecieved)+" AND UserNum="+selectedUser.UserNum);
			Assert.AreEqual("8",alertCount);
			//
			//Mark 2 of the emails as read, to decrease the amount of unread emails
			string command="UPDATE emailmessage SET SentOrReceived="+POut.Int((int)EmailSentOrReceived.WebMailRecdRead)+
				" WHERE SentOrReceived="+POut.Int((int)EmailSentOrReceived.WebMailReceived)+" AND ProvNumWebMail="+POut.Long(selectedUser.ProvNum)+" LIMIT 2";
			DataCore.NonQ(command);
			AlertItems_CreateAlertsForWebmailMethodCall();
			alertCount=DataCore.GetScalar("SELECT Description FROM alertitem WHERE Type="+POut.Int((int)AlertType.WebMailRecieved)+" AND UserNum="+selectedUser.UserNum);
			Assert.AreEqual("6",alertCount);
			//
			//Now we mark all of this user's emails as read, as if that user has read all of their webmail.
			command="UPDATE emailmessage SET SentOrReceived="+POut.Int((int)EmailSentOrReceived.WebMailRecdRead)+
				" WHERE SentOrReceived="+POut.Int((int)EmailSentOrReceived.WebMailReceived)+" AND ProvNumWebMail="+POut.Long(selectedUser.ProvNum);
			DataCore.NonQ(command);
			AlertItems_CreateAlertsForWebmailMethodCall();
			alertCount=DataCore.GetScalar("SELECT COUNT(*) FROM alertitem WHERE Type="+POut.Int((int)AlertType.WebMailRecieved)+" AND UserNum="+selectedUser.UserNum);
			Assert.AreEqual("0",alertCount);
		}

		///<summary>Here we test the CheckUniqueAlerts method to determine that the correct number of unique alerts are being returned in the the list
		///utilized to display alerts in the alerts menu.
		///</summary>
		[TestMethod]
		public void AlertItems_GetUniqueAlerts_Addclinic() {
			//This test will check the funcionality of alert items which are marked to show in all clinics(AlertItem.ClinicNum==-1).
			//Expected behaviour is that a user subscribed to all alert categories, and all clinics(AlertSub.ClinicNum==-1), 
			//will see the alert no matter which clinic they are in.
			//In addition, when a new clinic is added, the user will be able to see alerts in that new clinic without the need to reenter
			//FormUserEdit and select "All" again under clinics for alert subscriptions.
			//Clear AlertSub table.
			AlertSubT.ClearAlertSubTable();
			//Create Users
			Userod userAdmin=UserodT.CreateUser();
			Userod userNormal=UserodT.CreateUser();
			//Create Clinics
			List<Clinic> listClinics=new List<Clinic>();
			for(int i=0;i<2;i++) {
				listClinics.Add(ClinicT.CreateClinic());
			}
			//Create AlertItems
			//First alert Item is an alert item for all clinics(ClinicNum==-1).
			CreateAlertItem(true);
			//Second AlertItem is an AlertItem for HQ(ClinicNum==0).
			CreateAlertItem(false);
			List<AlertCategory> listAlertCats=AlertCategories.GetDeepCopy();
			List<AlertSub> listAlertSubOld=new List<AlertSub>();
			List<AlertSub> listAlertSubNew=new List<AlertSub>();
			foreach(AlertCategory alertCat in listAlertCats) {
				AlertSub alSub=new AlertSub(userAdmin.UserNum,-1,alertCat.AlertCategoryNum);
				listAlertSubNew.Add(alSub);
			}
			AlertSubs.Sync(listAlertSubNew,listAlertSubOld);
			//Check number of alerts which will display in headquarters clinic.
			//Call CheckUniqueAlerts for user subscribed to all alert categories
			List<List<AlertItem>> listUniqueAlertsAll=AlertItems.GetUniqueAlerts(userAdmin.UserNum,0);
			List<List<AlertItem>> listUniqueAlertsOne=AlertItems.GetUniqueAlerts(userNormal.UserNum,0);
			//Assert lists are correct
			//UserAdmin should see two alerts, one for the generic headquarters alert and one for the eConnector all clinics alert.
			Assert.AreEqual(2,listUniqueAlertsAll.Count());
			//UserNormal is not subscribed to any clinics or alert categories and should not see any alerts.
			Assert.AreEqual(0,listUniqueAlertsOne.Count());
			//Add clinic
			listClinics.Add(ClinicT.CreateClinic());
			//Check that alert for all clinics is included for userAdmin(subscribed to all clinics)
			listUniqueAlertsAll=AlertItems.GetUniqueAlerts(userAdmin.UserNum,listClinics.LastOrDefault().ClinicNum);
			Assert.AreEqual(1,listUniqueAlertsAll.Count());
			//Check new clinic for user who is not subscribed to all alerts. 
			listUniqueAlertsOne=AlertItems.GetUniqueAlerts(userNormal.UserNum,listClinics.LastOrDefault().ClinicNum);
			Assert.AreEqual(0,listUniqueAlertsOne.Count());
			//Add new alert for new clinic only.
			CreateAlertItem(false,listClinics.LastOrDefault().ClinicNum);
			//Check that userAdmin sees new alert item in new clinic. Should have 2, one all clinic econnector alert and the new clinic specific alert.
			listUniqueAlertsAll=AlertItems.GetUniqueAlerts(userAdmin.UserNum,listClinics.LastOrDefault().ClinicNum);
			Assert.AreEqual(2,listUniqueAlertsAll.Count());
			//Check that userNormal sees no alerts in new clinic, as they are not subscribed to any alert categories, nor clinics.
			listUniqueAlertsOne=AlertItems.GetUniqueAlerts(userNormal.UserNum,listClinics.LastOrDefault().ClinicNum);
			Assert.AreEqual(0,listUniqueAlertsOne.Count());
		}

		///<summary>Creates an alert item. Setting allClinicAlert bool to true creates an eConnector alert which will have a clinicNum of -1
		///and appear in all clinics for users subscribed to the alert category containing econnector alerts. </summary>
		private static void CreateAlertItem(bool allClinicAlert,long clinicNum=0) {
			if(allClinicAlert) {
				AlertItems.Insert(new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.MarkAsRead,
					Description=Lans.g("Econnector","eConnector needs to be restarted"),
					Severity=SeverityType.High,
					Type=AlertType.EConnectorDown,
					ClinicNum=-1,//Show for all clinics
					FormToOpen=FormType.FormEServicesEConnector,
				});
			}
			else if(clinicNum!=0) {
				AlertItems.Insert(new AlertItem() {
					Type=AlertType.Generic,
					Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.ShowItemValue,
					Description="A generic alert.",
					Severity=SeverityType.Low,
					ItemValue="A generic alert created for testing alert Items for all clinics. ClinicNum==" + clinicNum,
					ClinicNum=clinicNum,
				}); ;
			}
			else {
				AlertItems.CreateGenericAlert("A generic alert.","A generic alert created for testing alert Items for all clinics. ClinicNum==0");
			}
		}
	}
}
