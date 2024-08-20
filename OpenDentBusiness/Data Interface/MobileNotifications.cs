using CodeBase;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class MobileNotifications{
		///<summary>For unit testing. Set this in order to get a callback when InsertMobileNotification gets an exceptions.</summary>
		public static Action<Exception> MockExceptionHandler=null;

		#region Methods - Get
		///<summary>Gets notifications that were added to the DB since the last poll for the device with the passed-in deviceId.</summary>
		public static List<MobileNotification> GetManySinceLastPoll(DateTime dateTimeLastPoll,string deviceId,EnumAppTarget appTarget){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<MobileNotification>>(MethodBase.GetCurrentMethod(),dateTimeLastPoll,deviceId,appTarget);
			}
			//DateTimeEntry>=DateTimeLastPoll is inclusive due to the edge case where two mobile notifications are inserted at the same second but a poll is conducted
			//in between both inserts. Because the CRUD generator does not support fractional seconds, if the query were not inclusive, the second mobile notification
			//would be ignored the next poll and never retrieved. The first poll should have been deleted by the time the second poll occurs protecting us from processsing
			//duplicate mobile notifications.
			string command="SELECT * FROM mobilenotification WHERE DateTimeEntry>="+POut.DateT(dateTimeLastPoll)
				+" AND DateTimeExpires>"+POut.DateT(DateTime_.Now)
				+" AND DeviceId='"+POut.String(deviceId)+"'"
				+" AND AppTarget='"+POut.Enum<EnumAppTarget>(appTarget)+"'";
			return Crud.MobileNotificationCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(MobileNotification mobileNotification){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				mobileNotification.MobileNotificationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mobileNotification);
				return mobileNotification.MobileNotificationNum;
			}
			return Crud.MobileNotificationCrud.Insert(mobileNotification);
		}

		///<summary></summary>
		public static void Update(MobileNotification mobileNotification){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileNotification);
				return;
			}
			Crud.MobileNotificationCrud.Update(mobileNotification);
		}

		///<summary></summary>
		public static void Delete(long mobileNotificationNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileNotificationNum);
				return;
			}
			Crud.MobileNotificationCrud.Delete(mobileNotificationNum);
		}

		///<summary>Deletes many MobileNotifications based on the given list of MobileNotificationNums.</summary>
		public static void DeleteMany(List<long> listMobileNotifications) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMobileNotifications);
				return;
			}
			Crud.MobileNotificationCrud.DeleteMany(listMobileNotifications);
		}

		///<summary>Deletes all mobile notifications from the DB that have expired.</summary>
		public static void DeleteExpired() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="DELETE FROM mobilenotification WHERE DateTimeExpires<="+POut.DateT(DateTime_.Now);
			Db.NonQ(command);
		}
		#endregion Methods - Modify

		#region Methods - Misc
		#region eClipboard
		///<summary>Check-in the given patient on the given device. 
		///The device will simulate the patient entering their FName, LName, BDate then clicking Submit and then Confirming their identity.</summary>
		public static void CI_CheckinPatient(long patNum,long mobileAppDeviceNum) {
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				return;
			}
			//See CI_CheckinPatient for input details.
			InsertMobileNotification(MobileNotificationType.CI_CheckinPatient,mobileAppDeviceNum,EnumAppTarget.eClipboard,listTags: new List<string>() { pat.FName,pat.LName,pat.Birthdate.Ticks.ToString(),patNum.ToString() });
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
				InsertMobileNotification(MobileNotificationType.CI_AddSheet,mad.MobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys: new List<long>() { pat.PatNum,sheet.SheetNum });
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
				InsertMobileNotification(MobileNotificationType.CI_RemoveSheet,mad.MobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys: new List<long>() { pat.PatNum,sheetNum });
			}
		}

		///<summary>Take this device back to check-in. Any action being taken in an existing check-in session will be lost.</summary>
		public static void CI_GoToCheckin(long mobileAppDeviceNum) {
			//See CI_GoToCheckin for input details.
			InsertMobileNotification(MobileNotificationType.CI_GoToCheckin,mobileAppDeviceNum,EnumAppTarget.eClipboard);
		}

		///<summary>This mobile notification is inserted when the eClipboard preferences for the given clinic change.</summary>
		public static void CI_NewEClipboardPrefs(long clinicNum) {
			string allowSelfCheckin=POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowSelfCheckIn,clinicNum));
			string eClipboardMessageComplete=ClinicPrefs.GetPrefValue(PrefName.EClipboardMessageComplete,clinicNum);
			string allowSelfPortrait=POut.Bool(EClipboardImageCaptureDefs.Refresh().Any(x => x.ClinicNum==clinicNum && x.IsSelfPortrait));
			string showAvailableFormsOnCheckin=POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardPresentAvailableFormsOnCheckIn,clinicNum));
			string multiPageCheckin=POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardHasMultiPageCheckIn,clinicNum));
			string allowPaymentOnCheckin=POut.Bool(ClinicPrefs.GetBool(PrefName.EClipboardAllowPaymentOnCheckin,clinicNum));
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForClinic(clinicNum);
			for(int i=0;i<listMobileAppDevices.Count;i++) { 
				//See CI_NewEClipboardPrefs for input details.
				InsertMobileNotification(MobileNotificationType.CI_NewEClipboardPrefs,mobileAppDeviceNum:listMobileAppDevices[i].MobileAppDeviceNum,EnumAppTarget.eClipboard,
					listTags:new List<string>() {
						allowSelfCheckin,
						eClipboardMessageComplete,
						allowSelfPortrait,//Pref 'EClipboardAllowSelfPortraitOnCheckIn' deprecated and replaced by 'eclipboardimagecapturedef' table.
						showAvailableFormsOnCheckin,
						multiPageCheckin,
						allowPaymentOnCheckin,
					}
				);
			}
		}

		///<summary>Attempts to insert a mobile notification to the given mobileAppDeviceNum so that the device can view treatPlan and PDF.
		///Returns true if no error occurred, otherwise false and errorMsg is set.</summary>
		public static bool CI_SendTreatmentPlan(PdfDocument doc,TreatPlan treatPlan,bool hasPracticeSig,long mobileAppDeviceNum,out string errorMsg,out long mobileDataByteNum){
			mobileDataByteNum=-1;
			try {
				if(MobileDataBytes.TryInsertPDF(doc,treatPlan.PatNum,null,eActionType.TreatmentPlan,out mobileDataByteNum,out errorMsg,
					new List<string>() { treatPlan.Heading,hasPracticeSig.ToString(),treatPlan.DateTP.Ticks.ToString() })){
					//update the treatment plan MobileAppDeviceNum so we now it is on an device
					TreatPlans.UpdateMobileAppDeviceNum(treatPlan,mobileAppDeviceNum);
					Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
					Signalods.SetInvalid(InvalidType.EClipboard);
					InsertMobileNotification(MobileNotificationType.CI_TreatmentPlan,mobileAppDeviceNum,EnumAppTarget.eClipboard
						,listPrimaryKeys: new List<long>() { mobileDataByteNum,treatPlan.PatNum,treatPlan.TreatPlanNum }
						,listTags: new List<string>() { treatPlan.Heading,hasPracticeSig.ToString(), treatPlan.DateTP.Ticks.ToString() }
					);
				}
			}
			catch(Exception ex){
				errorMsg=ex.Message;
			}
			return (errorMsg.IsNullOrEmpty());
		}

		///<summary>Removes the MobileAppDeviceNum from the treatment plan as well.</summary>
		public static void CI_RemoveTreatmentPlan(long mobileAppDeviceNum,TreatPlan treatPlan){
			InsertMobileNotification(MobileNotificationType.CI_RemoveTreatmentPlan,mobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys:new List<long>() { treatPlan.PatNum,treatPlan.TreatPlanNum });
			//TreatPlanParams are only inserted into the database if this pref is true
			if(PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				TreatPlanParams.DeleteByTreatPlanNum(treatPlan.TreatPlanNum);
			}
			//Treatment plan is being removed from device, so the MobileAppDeviceNum needs to be 0
			TreatPlans.UpdateMobileAppDeviceNum(treatPlan,0);
			Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary>Attempts to insert a mobile notification to the given mobileAppDeviceNum..
		///Returns true if no error occurred, otherwise false and errorMsg is set.</summary>
		public static bool CI_SendPayment(long mobileAppDeviceNum,long patNum,out string errorMsg){
			errorMsg="";
			try {
				InsertMobileNotification(MobileNotificationType.CI_SendPayment,mobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys:new List<long> { patNum });
			}
			catch(Exception ex) {
				errorMsg=ex.Message;
			}
			return errorMsg.IsNullOrEmpty();
		}

		///<summary>Attempts to insert a mobile notification to the given mobileAppDeviceNum so that if a patient is on the payment window, it will refresh
		///the data to display the most recent payments.</summary>
		public static void CI_RefreshPayment(long mobileAppDeviceNum,long patNum,out string errorMsg) {
			errorMsg="";
			try {
				InsertMobileNotification(MobileNotificationType.CI_RefreshPayment,mobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys:new List<long>{ patNum });
			}
			catch(Exception ex) {
				errorMsg=ex.Message;
			}
		}

		///<summary>Attempts to insert a mobile notification to the given mobileAppDeviceNum so that the device can view treatPlan and PDF.
		///Returns true if no error occurred, otherwise false and errorMsg is set.</summary>
		public static bool CI_SendPaymentPlan(PdfDocument doc,PayPlan payPlan,long mobileAppDeviceNum,out string errorMsg,out long mobileDataByteNum) {
			mobileDataByteNum=-1;
			Patient pat=Patients.GetPat(payPlan.PatNum);
			try {
				List<string> listTags=new List<string>() { payPlan.PlanNum.ToString(),payPlan.PayPlanDate.Ticks.ToString(),pat.GetNameFirstOrPrefL() };
				if(MobileDataBytes.TryInsertPDF(doc,payPlan.PatNum,null,eActionType.PaymentPlan,out mobileDataByteNum,out errorMsg,listTags)){
					//update the payment plan MobileAppDeviceNum so we now it is on an device
					PayPlans.UpdateMobileAppDeviceNum(payPlan,mobileAppDeviceNum);
					Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,payPlan.PatNum);
					Signalods.SetInvalid(InvalidType.EClipboard);
					InsertMobileNotification(MobileNotificationType.CI_PaymentPlan,mobileAppDeviceNum,EnumAppTarget.eClipboard
						,listPrimaryKeys: new List<long>() { mobileDataByteNum,payPlan.PatNum,payPlan.PayPlanNum }
						,listTags: listTags
					);
				}
			}
			catch(Exception ex){
				errorMsg=ex.Message;
			}
			return (errorMsg.IsNullOrEmpty());
		}

		public static void CI_RemovePaymentPlan(long mobileAppDeviceNum,PayPlan payPlan) {
			InsertMobileNotification(MobileNotificationType.CI_RemovePaymentPlan,mobileAppDeviceNum,EnumAppTarget.eClipboard,listPrimaryKeys:new List<long>() { payPlan.PatNum,payPlan.PayPlanNum});
			//Treatment plan is being removed from device, so the MobileAppDeviceNum needs to be 0
			PayPlans.UpdateMobileAppDeviceNum(payPlan,0);
			Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,payPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}
		#endregion

		#region ODMobile
		///<summary>Notifies all registered devices that all user for this ClinicNum should be logged out of version OD Mobile.
		///This will only be sent to registered devices attached to the ClinicNum.</summary>
		public static void ODM_LogoutClinic(long clinicNum) {
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForClinic(clinicNum);
			for(int i=0;i<listMobileAppDevices.Count;i++) { 
				//See ODM_LogoutODUser for input details.
				InsertMobileNotification(MobileNotificationType.ODM_LogoutODUser,mobileAppDeviceNum:listMobileAppDevices[i].MobileAppDeviceNum,EnumAppTarget.ODMobile);
			}
		}

		///<summary>Notifies all registered devices that this userNum should be logged out of version OD Mobile.
		///This will only be sent to registered devices attached to the UserNum.</summary>
		public static void ODM_LogoutODUser(long userNum) {
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForUser(Userods.GetUser(userNum),true);
			for(int i = 0;i<listMobileAppDevices.Count;i++) {
				//See ODM_LogoutODUser for input details. Explicitly specify non-clinic specific.
				InsertMobileNotification(MobileNotificationType.ODM_LogoutODUser,mobileAppDeviceNum: listMobileAppDevices[i].MobileAppDeviceNum,EnumAppTarget.ODMobile,listPrimaryKeys: new List<long> { userNum });
			}
		}

		///<summary>This is a workaround due to android push notifications no longer being supported for xamarin. Will only notify android users when ODMobile is running.</summary>
		public static void ODM_NewTextMessage(SmsFromMobile smsFromMobile,long patNum) {
			if(smsFromMobile==null) {
				return;
			}
			//No need to bother mobile users with alert for eConfirmation responses.
			if(smsFromMobile.IsConfirmationResponse){
				return;
			}
			Patient patient=Patients.GetLim(patNum);
			string senderName=patient?.GetNameFirstOrPreferred();
			if(string.IsNullOrEmpty(senderName)) {
				if(smsFromMobile.MobilePhoneNumber.Length==11 && smsFromMobile.MobilePhoneNumber[0]=='1') {
					senderName=$"({smsFromMobile.MobilePhoneNumber[1]}{smsFromMobile.MobilePhoneNumber[2]}{smsFromMobile.MobilePhoneNumber[3]})"+
						$"{smsFromMobile.MobilePhoneNumber[4]}{smsFromMobile.MobilePhoneNumber[5]}{smsFromMobile.MobilePhoneNumber[6]}-" +
						$"{smsFromMobile.MobilePhoneNumber[7]}{smsFromMobile.MobilePhoneNumber[8]}{smsFromMobile.MobilePhoneNumber[9]}{smsFromMobile.MobilePhoneNumber[10]}";
				}
				else {
					senderName=smsFromMobile.MobilePhoneNumber;
				}
			}
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForClinic(smsFromMobile.ClinicNum);
			for(int i = 0;i<listMobileAppDevices.Count;i++) {
				InsertMobileNotification(MobileNotificationType.ODM_NewTextMessage,listMobileAppDevices[i].MobileAppDeviceNum,EnumAppTarget.ODMobile,listTags:new List<string>() { smsFromMobile.MobilePhoneNumber,$@"New Message",$@"Received a new message from {senderName}" });
			}
		}
		#endregion

		#region ODTouch 
		public static bool ODT_ExamSheet(long patNum,long sheetNum,long mobileAppDeviceNum,out string errorMsg) {
			errorMsg="";
			try {
				List<long> listPrimaryKeys=new List<long>() { patNum,sheetNum };
				InsertMobileNotification(MobileNotificationType.ODT_ExamSheet,mobileAppDeviceNum,EnumAppTarget.ODTouch,listPrimaryKeys:listPrimaryKeys);
			}
			catch (Exception ex) {
				errorMsg=ex.Message;
			}
			return errorMsg.IsNullOrEmpty();
		}
		#endregion ODTouch

		#region Multi-App
		///<summary>Occurs when the MobileAppDevice.IsXEnabled changed for this device or this MobileAppDevice row is deleted (send isAllowed = false). Used for eClipboard and ODTouch. Pass in appTarget.</summary>
		public static void IsAllowedChanged(long mobileAppDeviceNum,EnumAppTarget enumAppTarget,bool isAllowed) {
			//See CI_IsAllowedChanged for input details.
			InsertMobileNotification(MobileNotificationType.IsAllowedChanged,mobileAppDeviceNum,enumAppTarget,listTags: new List<string>() { POut.Bool(isAllowed) });
		}
		#endregion

		#region Insert
		///<summary>Inserts a mobile notification to tell open applications to perform a specific task (as defined by the payload). 
		///The user does NOT need to interact with the app to process this action.
		///Mobile notifications are silent and generally the user does not care if it fails. 
		///Do not call this method from outside this class, try to follow the above pattern. This method is public solely for testing purposes.
		///<param name="mobileNotificationType">The action for apps to take in the background.</param>
		///<param name="mobileAppDeviceNum">The MobileAppDeviceNum for the devices that the mobile notification will be sent to. 
		///Only use this when notification is intended for one and only one device.</param>
		///<param name="listPrimaryKeys">Varies depending on specific MobileNotificationType(s).</param>
		///<param name="listTags">Varies depending on specific MobileNotificationType(s).</param>		
		public static void InsertMobileNotification(MobileNotificationType mobileNotificationType,long mobileAppDeviceNum,EnumAppTarget appTarget,List<long> listPrimaryKeys=null,List<string> listTags=null) {
			MobileNotification mobileNotification=new MobileNotification {
				NotificationType=mobileNotificationType,
				PrimaryKeys=JsonConvert.SerializeObject(listPrimaryKeys??new List<long>()),
				Tags=JsonConvert.SerializeObject(listTags??new List<string>()),
				AppTarget=appTarget,
			};
			MobileAppDevice mobileAppDevice=MobileAppDevices.GetOne(mobileAppDeviceNum);
			if(mobileAppDevice==null) {
				throw new Exception("MobileAppDeviceNum not found: "+mobileAppDeviceNum.ToString());
			}
			mobileNotification.DeviceId=mobileAppDevice.UniqueID;
			//Validate that this clinic is signed up for eClipboard if the mobile notification is related to eClipboard
			if(appTarget==EnumAppTarget.eClipboard && !MobileAppDevices.IsClinicSignedUpForEClipboard(mobileAppDevice.ClinicNum)) {
				throw new Exception($"ClinicNum {mobileAppDevice.ClinicNum} is not signed up for eClipboard.");
			}
			//Validate that this clinic is signed up for MobileWeb if the mobile notification is related to ODMobile
			else if(appTarget==EnumAppTarget.ODMobile && !MobileAppDevices.IsClinicSignedUpForMobileWeb(mobileAppDevice.ClinicNum)) {
				if(mobileNotificationType!=MobileNotificationType.ODM_LogoutODUser) { //Logout is allowed to be sent to non-specific clinicNum. All others are not.
					throw new Exception($"ClinicNum {mobileAppDevice.ClinicNum} is not signed up for ODMobile.");
				}
			}
			else if(appTarget==EnumAppTarget.ODTouch && !ClinicPrefs.IsODTouchAllowed(mobileAppDevice.ClinicNum)) {
				throw new Exception($"ClinicNum {mobileAppDevice.ClinicNum} is not signed up for ODTouch.");
			}
			mobileNotification.DateTimeEntry=DateTime_.Now;
			mobileNotification.DateTimeExpires=DateTime_.Now.AddMinutes(10);
			MobileNotifications.Insert(mobileNotification);
		}		
		#endregion
		#endregion Methods - Misc
	}
}