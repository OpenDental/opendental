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
		#endregion

		#region Send Helpers
		///<summary>Sends a push notification as an alert. Makes an alert on the device if possible. 
		///If the user clicks on the alert or the app is already open, will do the given action.
		///Swallows errors and logs to PushNotifications Logger directory.  </summary>
		///<param name="pushType">The alert type for apps to show.</param>
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
			SendPush(pushType,mobileAppDeviceNum,clinicNum,userNum,listPrimaryKeys,listTags,alertTitle,alertMessage,runAsync);
		}

		///<summary>Only call this method from SendPushBackground() or SendPushAlert(). See summary of those methods for documentation.
		///If runAsync==true then spawns a worker thread and makes the web call. This method is passive and returns void so no reason to wait for return.</summary>
		private static void SendPush(PushType pushType,long mobileAppDeviceNum=0,long clinicNum=0,long userNum=0,
			List<long> listPrimaryKeys=null,List<string> listTags=null,string alertTitle=null,string alertMessage=null,bool runAsync=true) 
		{
			void execute() {
				eServiceCode eService=eServiceCode.EClipboard;
				switch(pushType) {
					case PushType.ODM_NewTextMessage:
						eService=eServiceCode.MobileWeb;
						break;
					case PushType.ODM_Erx:
					case PushType.ODM_ErxPatient:
					case PushType.None:
					default:
						throw new Exception("Unsupported PushType: "+pushType.ToString());
				}
				PushNotificationPayload payload=new PushNotificationPayload {
					IsAlert=true,
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
					throw new Exception($"ClinicNum {payload.ClinicNum} is not signed up for ODMobile.");
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
