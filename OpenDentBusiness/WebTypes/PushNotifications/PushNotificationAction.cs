﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// XAMARIN VERSION OF THIS FILE NEEDS TO MATCH ODBUSINESS VERSION OF FILE. IT IS A COPY, NOT THE SAME FILE
/// </summary>
namespace OpenDentBusiness.WebTypes {

	///<summary>The payload of the push notification. This contains what to do in the case of this type of notification.</summary>
	public class PushNotificationAction {
		///<summary>The type of push notification it is.</summary>
		public PushType TypeOfPush;
		///<summary>A list of primary keys associated with the push notifications. See PushType for what is included with each type.</summary>
		public List<long> ListPrimaryKeys;
		///<summary>A list of tags for this push action. Can be anything. Different for each type. See PushType for what is included with each type.</summary>
		public List<string> ListTags;
	}

	///<summary>The different types of push notifications. Preserve order. CI = Checkin portion of app (eClipboard). ODM = ODMobile</summary>
	public enum PushType {
		///<summary>Default. Can be used to send alert pushes that do nothing, just text to show the user.</summary>
		None,
		///<summary>Background push. Check-in a patient on a given device. For this type, the tag will have 3 items: the first name, last name, and birthdate of the patient in that order. 
		///The birthdate will be in DateTime.Ticks.</summary>
		CI_CheckinPatient,
		///<summary>Background push. Tells the device that is currently filling out sheets to add a sheet to the list. For this type, the list of primary keys will have two items: the patnum and
		///the SheetNum in that order.</summary>
		CI_AddSheet,
		///<summary>Background push. Tells the device that is currently fillout out sheets to remove a sheet from the list. For this type, the list of primary keys will have two items: the patnum
		///and the SheetNum in that order.</summary>
		CI_RemoveSheet,
		///<summary>Background push. This push tells the device to stop whatever it is doing and go to a fresh checkin page. This may be a blank self-checkin or may be waiting for a push. This
		///allows users from OD to "clear" the device of a stale patient. No primary keys or tags needed.</summary>
		CI_GoToCheckin,
		///<summary>Background push. This push occurs when the preferences for this device's clinic changes. The tags for this push will be the EClipboardAllowSelfCheckIn(bool), EClipboardMessageComplete(string), 
		///EClipboardAllowSelfPortraitOnCheckIn(bool), and EClipboardPresentAvailableFormsOnCheckIn(bool) in that order.</summary>
		CI_NewEClipboardPrefs,
		///<summary>Background push. This push occurs when the MobileAppDevice.IsAllowed changed for this device. The tag for this push will be IsAllowed (bool).
		///If true then device which is currently awaiting in 'Not Allowed' state will try another login, should work this time. If false then force signout.</summary>
		CI_IsAllowedChanged,
		///<summary>Alert. This push occurs when a new text message is sent to the office. The ListPrimaryKeys will contain the PatNum of the message and the SmsFromMobileNum in that order.
		///The ListTags contains the phone number.</summary>
		ODM_NewTextMessage,
		///<summary>Background push. This push occurs when a permission has changed for a given OD user and they are no longer allowed to use OD Mobile. 
		///The ListPrimaryKeys may contain the UserNum of the user who is no longer allowed. This session will then be logged out of versioned OD Mobile.
		///If ListPrimaryKeys IsNullOrEmpty() then assume all users for the given ClinicNum should be logged out. No UserNum filter necessary in this case.</summary>
		ODM_LogoutODUser,
		///<summary>Background push. This push occurs when a OD proper user sends a patients treatment plan to a specific device to show the user.
		///ListPrimaryKeys => [MobileDataByteNum, PatNum, TreatPlanNum].
		///ListTags Keys => The treatPlan.Heading, hasPracticeSig(based on if TP sheet has SigBoxPractice) .</summary>
		CI_TreatmentPlan,
		/// <summary>
		/// Background push. This push occurs when a TreatmentPlan is deleted in OD and we want to tell a specific device so that they can remove it when viewing TreatmentPlans.
		/// ListPrimaryKeys => [TreatPlan.PatNum,TreatPlan.TreatPlanNum]
		/// </summary>
		CI_RemoveTreatmentPlan,
		///<summary>Background push. This push occurs when a payment needs to be made on an eClip device. This either adds the Make Payment action item to the checkin checklist
		///or it will open the QR code to scan from OD.
		///ListPrimaryKeys => [TreatPlan.PatNum]</summary>
		CI_SendPayment,
		///<summary>Background push. This push occurs when a patient is currently on the device, when a payment is made, when a new card is added (XWeb only), and when a new 
		///statement is created in OD.
		///ListPrimaryKeys => [PatNum]</summary>
		CI_RefreshPayment,
		///<summary>Background push. This push occurs when an OD proper user sends a payment plan to a specific device.
		///ListPrimaryKeys => [MobileDataByte.MobileDataByteNum,PayPlan.PatNum,PayPlan.PayPlanNum]
		///ListTags => [PayPlan.PayPlanDate]</summary>
		CI_PaymentPlan,
		///<summary>Background push. This push occurs when a payment plan is removed from the associated eClip device or when a payment plan is removed from OD proper.
		///This will remove a payment plan from user view on eClip.
		///ListPrimaryKeys => [PayPlan.PatNum,PayPlan.PayPlanNum]</summary>
		CI_RemovePaymentPlan,
		///<summary>Alert push. Opens the generic eRx view without a patient selected so the user can see their eRx notifications.</summary>
		ODM_Erx,
		///<summary>Alert push. Opens the eRx view with a specific patient selected to view their DS chart.
		///ListPrimaryKeys => [PatNum]</summary>
		ODM_ErxPatient,
		///<summary></summary>
		CL_ExamSheetsAll,
		///<summary></summary>
		CL_ExamSheet,
	}
}
