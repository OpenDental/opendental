using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using Newtonsoft.Json;
using PdfSharp.Pdf;

namespace OpenDentBusiness.WebTypes {
	public static class PushNotificationUtils {

		///<summary>For unit testing. Set this in order to get a callback when SendPush gets an exceptions.</summary>
		public static Action<Exception> MockExceptionHandler=null;

		#region eClipboard
		///<summary>Check-in the given patient on the given device. 
		///The device will simulate the patient entering their FName, LName, BDate then clicking Submit and then Confirming their identity.</summary>
		public static void CI_CheckinPatient(long patNum,long mobileAppDeviceNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				return;
			}
			//See CI_CheckinPatient for input details.
			SendPushBackground(PushType.CI_CheckinPatient,mobileAppDeviceNum,listTags: new List<string>() { pat.FName,pat.LName,pat.Birthdate.Ticks.ToString() });
		}

		///<summary>Add a sheet the the device check-in checklist.</summary>
		public static void CI_AddSheet(long patNum,long sheetNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				return;
			}
			List<MobileAppDevice> listMads=MobileAppDevices.GetAll(patNum);
			if(listMads.IsNullOrEmpty()) {
				return;
			}
			Sheet sheet=Sheets.GetOne(sheetNum);
			if(sheet==null) {
				return;
			}
			//See CI_AddSheet for input details.
			foreach(MobileAppDevice mad in listMads) {
				SendPushBackground(PushType.CI_AddSheet,mad.MobileAppDeviceNum,listPrimaryKeys: new List<long>() { pat.PatNum,sheet.SheetNum });
			}
		}

		///<summary>Remove a sheet from a device check-in checklist.</summary>
		public static void CI_RemoveSheet(long patNum,long sheetNum) {
			Patient pat=Patients.GetPat(patNum);
			//Inentionally not getting the actual Sheet from the db here. It may have already been deleted, now time to get it off the device.
			if(pat==null) {
				return;
			}
			List<MobileAppDevice> listMads=MobileAppDevices.GetAll(patNum);
			if(listMads.IsNullOrEmpty()) {
				return;
			}
			//See CI_RemoveSheet for input details.
			foreach(MobileAppDevice mad in listMads) {
				SendPushBackground(PushType.CI_RemoveSheet,mad.MobileAppDeviceNum,listPrimaryKeys: new List<long>() { pat.PatNum,sheetNum });
			}
		}

		///<summary>Take this device back to check-in. Any action being taken in an existing check-in session will be lost.</summary>
		public static void CI_GoToCheckin(long mobileAppDeviceNum) {
			//See CI_GoToCheckin for input details.
			SendPushBackground(PushType.CI_GoToCheckin,mobileAppDeviceNum);
		}

		///<summary>Occurs when the MobileAppDevice.IsAllowed changed for this device or this MobileAppDevice row is deleted (send isAllowed = false).</summary>
		public static void CI_IsAllowedChanged(long mobileAppDeviceNum,bool isAllowed) {
			//See CI_IsAllowedChanged for input details.
			SendPushBackground(PushType.CI_IsAllowedChanged,mobileAppDeviceNum,listTags: new List<string>() { POut.Bool(isAllowed) },runAsync:false);
		}

		///<summary>This push occurs when the eClipboard preferences for the given clinic change.</summary>
		public static void CI_NewEClipboardPrefs(long clinicNum) {
			//See CI_NewEClipboardPrefs for input details.
			SendPushBackground(PushType.CI_NewEClipboardPrefs,clinicNum:clinicNum,listTags:new List<string>() {
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowSelfCheckIn,clinicNum)),
				ClinicPrefs.GetPrefValue(PrefName.EClipboardMessageComplete,clinicNum),
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicNum)),
				POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicNum)),
			});
		}

		///<summary>Attempts to send a push notificaiton to the given mobileAppDeviceNum so that the device can view treatPlan and PDF.
		///Returns true if no error occured, otherwise false and errorMsg is set.</summary>
		public static bool CI_SendTreatmentPlan(PdfDocument doc,TreatPlan treatPlan,bool hasPracticeSig,long mobileAppDeviceNum
			,out string errorMsg,out long mobileDataByteNum)
		{
			mobileDataByteNum=-1;
			try {
				if(MobileDataBytes.TryInsertPDF(doc,treatPlan.PatNum,null,eActionType.TreatmentPlan,out mobileDataByteNum,out errorMsg,
					new List<string>() { treatPlan.Heading,hasPracticeSig.ToString(),treatPlan.DateTP.Ticks.ToString() })){
					//update the treatment plan MobileAppDeviceNum so we now it is on an device
					TreatPlans.UpdateMobileAppDeviceNum(treatPlan,mobileAppDeviceNum);
					Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
					Signalods.SetInvalid(InvalidType.EClipboard);
					SendPushBackground(PushType.CI_TreatmentPlan,mobileAppDeviceNum
						,listPrimaryKeys: new List<long>() { mobileDataByteNum,treatPlan.PatNum,treatPlan.TreatPlanNum }
						,listTags: new List<string>() { treatPlan.Heading,hasPracticeSig.ToString(), treatPlan.DateTP.Ticks.ToString() }
						,runAsync: true
					);
				}
			}
			catch(Exception ex){
				errorMsg=ex.Message;
			}
			return (errorMsg.IsNullOrEmpty());
		}

		///<summary>Removes the MobileAppDeviceNum from the treatment plan as well.</summary>
		public static void CI_RemoveTreatmentPlan(long mobileDeviceNum,TreatPlan treatPlan){
			SendPushBackground(PushType.CI_RemoveTreatmentPlan,mobileDeviceNum,listPrimaryKeys:new List<long>() { treatPlan.PatNum,treatPlan.TreatPlanNum });
			//TreatPlanParams are only inserted into the database if this pref is true
			if(PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				TreatPlanParams.DeleteByTreatPlanNum(treatPlan.TreatPlanNum);
			}
			//Treatment plan is being removed from device, so the MobileAppDeviceNum needs to be 0
			TreatPlans.UpdateMobileAppDeviceNum(treatPlan,0);
			Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}
		#endregion

		#region ODMobile

		///<summary>Notifies all registered devices that a new SMS has been received. Pass in the patient for this SmsFromMobile if one exists. This will
		///only be sent to registered devices attached to the ClinicNum</summary>
		public static void ODM_NewTextMessage(SmsFromMobile sms,long patNum=0) {
			if(sms==null) {
				return;
			}
			//No need to bother mobile users with alert for eConfirmation responses.
			if(sms.IsConfirmationResponse){
				return;
			}
			Patient pat=Patients.GetLim(patNum);
			string senderName=pat?.GetNameFirstOrPreferred();
			if(string.IsNullOrEmpty(senderName)) {
				if(sms.MobilePhoneNumber.Length==11 && sms.MobilePhoneNumber[0]=='1') {
					senderName=$"({sms.MobilePhoneNumber[1]}{sms.MobilePhoneNumber[2]}{sms.MobilePhoneNumber[3]})"+
						$"{sms.MobilePhoneNumber[4]}{sms.MobilePhoneNumber[5]}{sms.MobilePhoneNumber[6]}-" +
						$"{sms.MobilePhoneNumber[7]}{sms.MobilePhoneNumber[8]}{sms.MobilePhoneNumber[9]}{sms.MobilePhoneNumber[10]}";
				}
				else {
					senderName=sms.MobilePhoneNumber;
				}
			}
			//See ODM_NewTextMessage for input details.
			SendPushAlert(PushType.ODM_NewTextMessage,$@"New Message",$@"Received a new message from {senderName}",clinicNum:sms.ClinicNum,
				listTags:new List<string>() { sms.MobilePhoneNumber });
		}

		///<summary>Notifies all registered devices that all user for this ClinicNum should be logged out of version OD Mobile.
		///This will only be sent to registered devices attached to the ClinicNum.</summary>
		public static void ODM_LogoutClinic(long clinicNum) {
			//See ODM_LogoutODUser for input details.
			SendPushBackground(PushType.ODM_LogoutODUser,clinicNum: clinicNum,runAsync:false);
		}

		///<summary>Notifies all registered devices that this userNum should be logged out of version OD Mobile.
		///This will only be sent to registered devices attached to the UserNum.</summary>
		public static void ODM_LogoutODUser(long userNum) {
			//See ODM_LogoutODUser for input details. Explicitly specify non-clinic specific.
			SendPushBackground(PushType.ODM_LogoutODUser,clinicNum:-1,userNum:userNum,listPrimaryKeys: new List<long> { userNum },runAsync: false);
		}

		#endregion

		#region Send Helpers
		///<summary>Sends a push notification as a background message to tell open applications to perform a specific task (as defined by the payload). 
		///The user does NOT need to interact with the app to process this action.
		///Background push is silent and generally the user does not care if it fails. 
		///Swallows errors and logs to PushNotifications Logger directory.  </summary>
		///<param name="pushType">The action for apps to take in the background.</param>
		///<param name="mobileAppDeviceNum">The MobileAppDeviceNum for the devices that the push notification will be sent to. 
		///Only use this when notification is intended for one and only one device. ClinicNum param will be ignored if MobileAppDeviceNum is specified.
		///In this case the ClinicNum belonging to this MobileAppDevice row will be used instead..</param>
		///<param name="clinicNum">The ClinicNum for the devices that the push notification will be sent to. 
		///If the office does not have clinics enabled, use clinicNum of 0. -1 is a flag that means do not attach or validate a specific clinic.</param>
		///<param name="userNum">The UserNum for the UserOD that the push notification will be sent to. 
		///Only applies to ODMobile. If userNum=0 then will not be targeted to a specific UserOD.</param>
		///<param name="listPrimaryKeys">Varies depending on specific PushType(s).</param>
		///<param name="listTags">Varies depending on specific PushType(s).</param>
		///<param name="runAsync">Spawn thread and do not wait for result if true.</param>
		private static void SendPushBackground(PushType pushType,long mobileAppDeviceNum=0,long clinicNum=0,long userNum=0,List<long> listPrimaryKeys=null,List<string> listTags=null,bool runAsync = true) {
			SendPush(pushType,false,mobileAppDeviceNum,clinicNum,userNum,listPrimaryKeys,listTags,runAsync:runAsync);
		}

		///<summary>Sends a push notification as an alert. Makes an alert on the device if possible. 
		///If the user clicks on the alert or the app is already open, will do the given action.
		///Swallows errors and logs to PushNotifications Logger directory.  </summary>
		///<param name="pushType">The action for apps to take in the background.</param>
		///<param name="alertTitle">The title of the alert that will appear in the notification center.</param>
		///<param name="alertMessage">The contents of the alert.</param>
		///<param name="mobileAppDeviceNum">The MobileAppDeviceNum for the devices that the push notification will be sent to. 
		///Only use this when notification is intended for one and only one device. ClinicNum param will be ignored if MobileAppDeviceNum is specified.
		///In this case the ClinicNum belonging to this MobileAppDevice row will be used instead..</param>
		///<param name="clinicNum">The ClinicNum for the devices that the push notification will be sent to. 
		///If the office does not have clinics enabled, use clinicNum of 0. -1 is a flag that means do not attach or validate a specific clinic.</param>
		///<param name="userNum">The UserNum for the UserOD that the push notification will be sent to. 
		///Only applies to ODMobile. If userNum=0 then will not be targeted to a specific UserOD.</param>
		///<param name="listPrimaryKeys">Varies depending on specific PushType(s).</param>
		///<param name="listTags">Varies depending on specific PushType(s).</param>
		///<param name="runAsync">Spawn thread and do not wait for result if true.</param>
		private static void SendPushAlert(PushType pushType,string alertTitle,string alertMessage,long mobileAppDeviceNum=0,long clinicNum=0,
			long userNum=0,List<long> listPrimaryKeys=null,List<string> listTags=null,bool runAsync = true) 
		{
			if(string.IsNullOrEmpty(alertTitle)||string.IsNullOrEmpty(alertMessage)) {
				throw new Exception("Alert title and message are required.");
			}
			SendPush(pushType,true,mobileAppDeviceNum,clinicNum,userNum,listPrimaryKeys,listTags,alertTitle,alertMessage,runAsync);
		}

		///<summary>Only call this method from SendPushBackground() or SendPushAlert(). See summary of those methods for documentation.
		///If runAsync==true then spawns a worker thread and makes the web call. This method is passive and returns void so no reason to wait for return.</summary>
		private static void SendPush(PushType pushType,bool isAlert,long mobileAppDeviceNum=0,long clinicNum=0,long userNum=0,
			List<long> listPrimaryKeys=null,List<string> listTags=null,string alertTitle=null,string alertMessage=null,bool runAsync=true) 
		{
			void execute() {
				eServiceCode eService=eServiceCode.EClipboard;
				switch(pushType) {
					case PushType.CI_CheckinPatient:
					case PushType.CI_AddSheet:
					case PushType.CI_RemoveSheet:
					case PushType.CI_GoToCheckin:
					case PushType.CI_NewEClipboardPrefs:
					case PushType.CI_IsAllowedChanged:
					case PushType.CI_TreatmentPlan:
					case PushType.CI_RemoveTreatmentPlan:
						eService=eServiceCode.EClipboard;
						break;
					case PushType.ODM_NewTextMessage:
					case PushType.ODM_LogoutODUser:
						eService=eServiceCode.MobileWeb;
						break;
					case PushType.None:
					default:
						throw new Exception("Unsupported PushType: "+pushType.ToString());
				}
				PushNotificationPayload payload=new PushNotificationPayload {
					IsAlert=isAlert,
					AlertMessage=alertMessage??"",
					AlertTitle=alertTitle??"",
					UserNum=userNum,
					PushNotificationActionJSON=JsonConvert.SerializeObject( new PushNotificationAction() {
						TypeOfPush=pushType,
						ListPrimaryKeys=listPrimaryKeys??new List<long>(),
						ListTags=listTags??new List<string>()
					},
					typeof(PushNotificationAction),
					new JsonSerializerSettings())
				};
				if(mobileAppDeviceNum>0) { //Send to one exact device.
					MobileAppDevice mad=MobileAppDevices.GetOne(mobileAppDeviceNum);
					if(mad==null) {
						throw new Exception("MobileAppDeviceNum not found: "+mobileAppDeviceNum.ToString());
					}
					payload.DeviceId=mad.UniqueID;
					payload.ClinicNum=mad.ClinicNum;
				}
				else {
					payload.ClinicNum=clinicNum;
				}
				//Validate that this clinic is signed up for eClipboard if the push is related to eClipboard
				if(eService==eServiceCode.EClipboard && !MobileAppDevices.IsClinicSignedUpForEClipboard(payload.ClinicNum)) {
					throw new Exception($"ClinicNum {payload.ClinicNum} is not signed up for eClipboard.");
				}
				//Validate that this clinic is signed up for MobileWeb if the push is related to ODMobile
				else if(eService==eServiceCode.MobileWeb && !MobileAppDevices.IsClinicSignedUpForMobileWeb(payload.ClinicNum)) {
					if(clinicNum>-1 || pushType!=PushType.ODM_LogoutODUser) { //Logout is allowed to be sent to non-specific clinicNum. All others are not.
						throw new Exception($"ClinicNum {payload.ClinicNum} is not signed up for ODMobile.");
					}
				}
				string jsonPayload=JsonConvert.SerializeObject(payload,typeof(PushNotificationPayload),new JsonSerializerSettings());
				string result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().SendPushNotification(PayloadHelper.CreatePayload("",eService),jsonPayload);
				if(result.ToLower()!="success") {
					throw new Exception(result);
				}
			}
			ODThread th=new ODThread(new ODThread.WorkerDelegate((o) => { execute(); }));
			th.AddExceptionHandler((e) => {
				if(MockExceptionHandler!=null) {
					MockExceptionHandler(e);
				}
				else {
					Logger.WriteException(e,"PushNotifications");
				}
			});
			th.Name="SendPush_"+pushType.ToString()+"_ClinicNum_"+clinicNum.ToString()+"_UserNum_"+userNum.ToString();
			th.Start();
			if(MockExceptionHandler!=null || !runAsync) { //Join back to main thread to cause this to be a blocking call. Unit tests will always block.
				th.Join(Timeout.Infinite);
			}
		}		
		#endregion
	}
}
