using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AlertItems{
		#region Insert
		///<summary>Inserts a generic alert where description will show in the menu item and itemValue will be shown within a MsgBoxCopyPaste.
		///Set itemValue to more specific reason for the alert.  E.g. exception text details as to help the techs give better support.</summary>
		public static void CreateGenericAlert(string description,string itemValue) {
			//No need to check MiddleTierRole; no call to db.
			AlertItem alertItem=new AlertItem();
			alertItem.Type=AlertType.Generic;
			alertItem.Actions=ActionType.MarkAsRead | ActionType.Delete | ActionType.ShowItemValue;
			alertItem.Description=description;
			alertItem.Severity=SeverityType.Low;
			alertItem.ItemValue=itemValue;
			AlertItems.Insert(alertItem);
		}
		#endregion

		#region Misc Methods

		///<summary>Checks to see if the heartbeat for Open Dental Service was within the last six minutes. If not, an alert will be sent telling 
		///users OpenDental Service is down.</summary>
		public static void CheckODServiceHeartbeat() {
			//No need to check MiddleTierRole; no call to db.
			if(!IsODServiceRunning()) {//If the heartbeat is over 6 minutes old, send the alert if it does not already exist
				//Check if there are any previous alert items
				//Get previous alerts of this type
				List<AlertItem> listAlertItemsOld=RefreshForType(AlertType.OpenDentalServiceDown);
				if(listAlertItemsOld.Count==0) {//an alert does not already exist
					AlertItem alertItem=new AlertItem();
					alertItem.Actions=ActionType.MarkAsRead;
					alertItem.ClinicNum=-1;//all clinics
					alertItem.Description=Lans.g("Alerts","No instance of Open Dental Service is running.");
					alertItem.Type=AlertType.OpenDentalServiceDown;
					alertItem.Severity=SeverityType.Medium;
					Insert(alertItem);
				}
			}
		}

		///<summary>Returns true if the heartbeat is less than 6 minutes old.</summary>
		public static bool IsODServiceRunning() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT ValueString,NOW() FROM preference WHERE PrefName='OpenDentalServiceHeartbeat'";
			DataTable table=DataCore.GetTable(command);
			DateTime dateTimeLastHeartbeat=PIn.DateT(table.Rows[0][0].ToString());
			DateTime dateTimeNow=PIn.DateT(table.Rows[0][1].ToString());
			if(dateTimeLastHeartbeat.AddMinutes(6)<dateTimeNow) {
				return false;
			}
			return true;
		}

		/// <summary>This method grabs all unread webmails, and creates/modifies/deletes alerts for the providers and linked users the webmails are addressed to.</summary>
		public static void CreateAlertsForNewWebmail(Logger.IWriteLine log){
			//No need to check MiddleTierRole; no call to db.
			//This method first collect all unread webmails, and counts how many each provider has.
			//It then fetches all WebMailRecieved alerts, and will create/modify alerts for each provider who was counted before.
			//Finally, it will sync the alerts in the database with the ones we created.
			//If the user has unread webmail and an existing alert, it is modified.
			//If the user has unread webmail and no existing alert, an alert is created.
			//If the user has no unread webmail and an existing alert, it will be deleted.
			List<long> listProvNumWebMails=EmailMessages.GetProvUnreadWebMail();
			String logMessage="Collected Webmails for the following providers (ProvNum: # Webmails): ";
			List<long> listProvNumsDistinct=listProvNumWebMails.Distinct().ToList();
			for(int i=0;i<listProvNumsDistinct.Count();i++){
				logMessage+=listProvNumsDistinct[i]+":";
				logMessage+=listProvNumWebMails.FindAll(x => x==listProvNumsDistinct[i]).Count()+" ";		
			}	
			log.WriteLine(logMessage,LogLevel.Verbose);
			//This list contains every single WebMailReceived alert and is synced with listAlerts later.
			List<AlertItem> listAlertItemsOld=AlertItems.RefreshForType(AlertType.WebMailReceived);
			log.WriteLine("Fetched current alerts for users: "+String.Join(", ",listAlertItemsOld.Select(x => POut.Long(x.UserNum))),LogLevel.Verbose);
			//If a user doesn't have any unread webmail, they won't be placed on this list, and any alert they have in listOldAlerts will be deleted.
			List<AlertItem> listAlertItems=new List<AlertItem>();
			List<long> listAlertItemNumsChanged=new List<long>();
			//Go through each provider value, and create/update alerts for each patnum under that provider.
			//There will only be a value if they have atleast 1 unread webmail.
			for(int p=0;p<listProvNumsDistinct.Count();p++){
				List<Userod> listUserods=Providers.GetAttachedUsers(listProvNumsDistinct[p]);
				int countProvNumWebMail=listProvNumWebMails.FindAll(x => x==listProvNumsDistinct[p]).Count();
				//Go through each usernum and create/update their alert item.
				List<long> listUserNums=listUserods.Select(x=>x.UserNum).ToList();
				for(int u=0;u<listUserNums.Count();u++){
					AlertItem alertItemForUser=listAlertItemsOld.FirstOrDefault(x => x.UserNum==listUserNums[u]);
					//If an alert doesn't exist for the user, we'll create it.
					if(alertItemForUser==null) {
						if(!AlertSubs.GetAllAlertTypesForUser(listUserNums[u]).Contains(AlertType.WebMailReceived)) {
							continue;//Don't create alert if user has no AlertSubs that include the WebMailReceived AlertType.
						}
						alertItemForUser=new AlertItem();
						alertItemForUser.Type=AlertType.WebMailReceived;
						alertItemForUser.FormToOpen=FormType.FormEmailInbox;
						alertItemForUser.Actions=ActionType.MarkAsRead|ActionType.OpenForm;	//Removed delete because the alert will just be re-added next time it checks.
						alertItemForUser.Severity=SeverityType.Normal;
						alertItemForUser.ClinicNum=-1;	//The alert is user dependent, not clinic dependent.
						alertItemForUser.UserNum=listUserNums[u];
						alertItemForUser.Description=POut.Long(countProvNumWebMail);
						listAlertItems.Add(alertItemForUser);
						log.WriteLine("Created webmail alert for user "+POut.Long(listUserNums[u]),LogLevel.Verbose);
					}
					else {
						//If the alert already exists, we'll be updating it and usually mark it as unread.
						AlertItem alertItemSelected=alertItemForUser.Copy();
						long previousValue=PIn.Long(alertItemSelected.Description);
						//We only need to modify the alert if the amount of unread webmails changed.
						if(previousValue!=countProvNumWebMail){
							alertItemSelected.Description=POut.Long(countProvNumWebMail);
							//If the new value is greater, the user has recieved more webmails so we want to mark the alert as "Unread".
							if(previousValue<countProvNumWebMail){
								listAlertItemNumsChanged.Add(alertItemSelected.AlertItemNum);
							}
						}
						listAlertItems.Add(alertItemSelected);
						log.WriteLine("Modified webmail alert for user "+POut.Long(listUserNums[u]),LogLevel.Verbose);
					}
				}
			}
			//Push our changes to the database.
			AlertItems.Sync(listAlertItems,listAlertItemsOld);
			List<AlertItem> listAlertItemsDeleted=listAlertItemsOld.Where(x => !listAlertItems.Any(y => y.AlertItemNum==x.AlertItemNum)).ToList();
			log.WriteLine("Deleted webmail alerts for users: "+String.Join(", ",listAlertItemsDeleted.Select(x => POut.Long(x.UserNum))),LogLevel.Verbose);
			//Make sure to mark alerts that were deleted, modified (not created) and increased as unread.
			listAlertItemNumsChanged.AddRange(listAlertItemsDeleted.Select(x => x.AlertItemNum));
			AlertReads.DeleteForAlertItems(listAlertItemNumsChanged);
		}

		///<summary>Returns a list of alert items for the current user.</summary>
		public static List<AlertItem> GetAlertsItemsForUser(long userNum, long clinicNum){
			//No need to check MiddleTierRole; no call to db.
			List<AlertSub> listAlertSubsForUser=AlertSubs.GetAllForUser(userNum);
			bool isAllClinics=listAlertSubsForUser.Any(x => x.ClinicNum==-1);
			List<long> listAlertCategoryNums=new List<long>();
			if(isAllClinics) {//User subscribed to all clinics.
				listAlertCategoryNums=listAlertSubsForUser.Select(x => x.AlertCategoryNum).Distinct().ToList();
			}
			else {
				//List of AlertSubs for current clinic and user combo.
				List<AlertSub> listAlertSubsForClinicUser=listAlertSubsForUser.FindAll(x => x.ClinicNum==clinicNum);
				listAlertCategoryNums=listAlertSubsForClinicUser.Select(y => y.AlertCategoryNum).ToList();
			}
			//AlertTypes current user is subscribed to.
			List<AlertType> listAlertTypesForUser=AlertCategoryLinks.GetWhere(x => listAlertCategoryNums.Contains(x.AlertCategoryNum))
				.Select(x => x.AlertType).ToList();
			List<long> listAlertCategoryNumsAll=listAlertSubsForUser.Select(y => y.AlertCategoryNum).ToList();
			//AlertTypes current user is subscribed to for AlertItems which are not clinic specific.
			List<AlertType> listAlertTypesAll=AlertCategoryLinks.GetWhere(x => listAlertCategoryNumsAll.Contains(x.AlertCategoryNum))
				.Select(x => x.AlertType).ToList();
			List<AlertItem> listAlertItemsForUser=RefreshForClinicAndTypes(clinicNum,listAlertTypesForUser)//Get alert items for the current clinic
				.Union(RefreshForClinicAndTypes(-1,listAlertTypesAll))//Get alert items that are for all clinics
				.Union(GetAllForUserNum(userNum))//Get alert items for the current user
				.DistinctBy(x => x.AlertItemNum).ToList();
			return listAlertItemsForUser;
		}

		///<summary>Returns true if the two alerts match all fields other than AlertItemNum.</summary>
		public static bool AreDuplicates(AlertItem alertItem1,AlertItem alertItem2) {
			//No need to check MiddleTierRole; no call to db.
			if(alertItem1==null || alertItem2==null) {
				return false;
			}
			return alertItem1.Actions==alertItem2.Actions
				&& alertItem1.ClinicNum==alertItem2.ClinicNum
				&& alertItem1.Description==alertItem2.Description
				&& alertItem1.FKey==alertItem2.FKey
				&& alertItem1.FormToOpen==alertItem2.FormToOpen
				&& alertItem1.ItemValue==alertItem2.ItemValue
				&& alertItem1.Severity==alertItem2.Severity
				&& alertItem1.Type==alertItem2.Type
				&& alertItem1.UserNum==alertItem2.UserNum;
		}

		#endregion

		///<summary>Returns a list of AlertItems for the given clinicNum.  Doesn't include alerts that are assigned to other users.</summary>
		public static List<AlertItem> RefreshForClinicAndTypes(long clinicNum,List<AlertType> listAlertTypes=null){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AlertItem>>(MethodBase.GetCurrentMethod(),clinicNum,listAlertTypes);
			}
			if(listAlertTypes==null || listAlertTypes.Count==0) {
				return new List<AlertItem>();
			}
			long provNum=0;
			if(Security.CurUser!=null && Userods.IsUserCpoe(Security.CurUser)) {
				provNum=Security.CurUser.ProvNum;
			}
			long userNum=0;
			if(Security.CurUser!=null) {
				userNum=Security.CurUser.UserNum;
			}
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT * FROM alertitem "
					+"WHERE Type IN ("+String.Join(",",listAlertTypes.Cast<int>().ToList())+") "
					+"AND (UserNum=0 OR UserNum="+POut.Long(userNum)+") "
					//For AlertType.RadiologyProcedures we only care if the alert is associated to the current logged in provider.
					//When provNum is 0 the initial WHEN check below will not bring any rows by definition of the FKey column.
					+"AND (CASE TYPE WHEN "+POut.Int((int)AlertType.RadiologyProcedures)+" THEN FKey="+POut.Long(provNum)+" "
					+"ELSE ClinicNum = "+POut.Long(clinicNum)+" OR ClinicNum=-1 END)";
			}
			else {//oracle
				//Case statements cannot change column return results unless they are within the SELECT case.
				command="SELECT AlertItemNum,CASE Type WHEN 3 THEN ClinicNum ELSE 0 END ClinicNum,Description,Type,Severity,Actions,FormToOpen,CASE Type WHEN 3 THEN 0 ELSE FKey END FKey,ItemValue "
					+"FROM alertitem "
					+"WHERE Type IN ("+String.Join(",",listAlertTypes.Cast<int>().ToList())+") ";
			}
			return Crud.AlertItemCrud.SelectMany(command);
		}

		///<summary>Returns a list of AlertItems for the given alertType.</summary>
		public static List<AlertItem> RefreshForType(AlertType alertType){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AlertItem>>(MethodBase.GetCurrentMethod(),alertType);
			}
			string command="SELECT * FROM alertitem WHERE Type="+POut.Int((int)alertType)+";";
			return Crud.AlertItemCrud.SelectMany(command);
		}

		///<summary>Get all AlertItems for the passed in UserNum.</summary>
		public static List<AlertItem> GetAllForUserNum(long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AlertItem>>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM alertitem WHERE UserNum="+POut.Long(userNum);
			return Crud.AlertItemCrud.SelectMany(command);
		}

		///<summary>Gets one AlertItem from the db.</summary>
		public static AlertItem GetOne(long alertItemNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<AlertItem>(MethodBase.GetCurrentMethod(),alertItemNum);
			}
			return Crud.AlertItemCrud.SelectOne(alertItemNum);
		}

		///<summary></summary>
		public static long Insert(AlertItem alertItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				alertItem.AlertItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alertItem);
				return alertItem.AlertItemNum;
			}
			return Crud.AlertItemCrud.Insert(alertItem);
		}

		///<summary></summary>
		public static void Update(AlertItem alertItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertItem);
				return;
			}
			Crud.AlertItemCrud.Update(alertItem);
		}

		///<summary>Inserts if it doesn't exist, otherwise updates.</summary>
		public static long Upsert(AlertItem alertItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				alertItem.AlertItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alertItem);
				return alertItem.AlertItemNum;
			}
			if(alertItem.AlertItemNum==0) {
				Insert(alertItem);
			}
			else {
				Update(alertItem);
			}
			return alertItem.AlertItemNum;
		}

		///<summary>If null listFKeys is provided then all rows of the given alertType will be deleted. Otherwise only rows which match listFKeys entries.</summary>
		public static void DeleteFor(AlertType alertType,List<long> listFKeys=null) {
			//No need to check MiddleTierRole; no call to db.
			List<AlertItem> listAlerts=RefreshForType(alertType);
			if(listFKeys!=null) { //Narrow down to just the FKeys provided.
				listAlerts=listAlerts.FindAll(x => listFKeys.Contains(x.FKey));
			}
			foreach(AlertItem alert in listAlerts) {
				Delete(alert.AlertItemNum);
			}
		}

		///<summary>Also deletes any AlertRead objects for this AlertItem.</summary>
		public static void Delete(long alertItemNum) {
			//No need to check MiddleTierRole; no call to db.
			Delete(new List<long> { alertItemNum });
		}

		///<summary>Also deletes any AlertRead objects for these AlertItems.</summary>
		public static void Delete(List<long> listAlertItemNums) {
			if(listAlertItemNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAlertItemNums);
				return;
			}
			AlertReads.DeleteForAlertItems(listAlertItemNums);
			string command="DELETE FROM alertitem WHERE AlertItemNum IN("+string.Join(",",listAlertItemNums.Select(POut.Long))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.</summary>
		public static void Sync(List<AlertItem> listAlertItemsNew,List<AlertItem> listAlertItemsOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAlertItemsNew,listAlertItemsOld);
				return;
			}
			Crud.AlertItemCrud.Sync(listAlertItemsNew,listAlertItemsOld);
		}
	}
}