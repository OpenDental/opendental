using System;

namespace OpenDentBusiness {
	///<summary>Mobile App devices periodically poll this table and retrieve any records that are relevant to the device itself, the user using the device, or the clinic
	///the device belongs to. The mobile apps will then perform an action based on the mobile notification type.</summary>
	[Serializable]
	public class MobileNotification:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileNotificationNum;
		///<summary>Enum:MobileNotificationType The type of notification. Example: TP. This will determine what actions the mobile app will perform upon retrieving this notification.</summary>
		public MobileNotificationType NotificationType;
		///<summary>The device id for the mobile notification. Example is random string of 10-12 characters. Only the device with this DeviceId will retrieve this record.</summary>
		public string DeviceId;
		///<summary>A comma-delimited list of primary keys associated with the mobile notification. See MobileNotificationType for what is included with each type. 
		///Can include MobileDataByteNums, TreatPlanNum, SheetNums, and others.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string PrimaryKeys;
		///<summary>A comma-delimited list of tags for this mobile notification. Can be anything. Different for each MobileNotificationType. See MobileNotificationType for what is included with each type.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Tags;
		///<summary>DateTime notification was entered into Db. Should not be edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEntry;
		///<summary>DateTime notification expires and becomes invalid.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeExpires;
		///<summary>Enum:EnumAppTarget Stores the mobile app that this notification is targeting. Prohibits a device running one app from consuming mobile 
		///notifications intended for a different app.</summary>
		public EnumAppTarget AppTarget;

		///<summary></summary>
		public MobileNotification Copy() {
			return (MobileNotification)this.MemberwiseClone();
		}
	}

	///<summary>The different types of mobile notifications. Preserve order. CI = Checkin portion of app (eClipboard). ODM = ODMobile</summary>
	public enum MobileNotificationType {
		///<summary>Default.</summary>
		None,
		///<summary>Check-in a patient on a given device. For this type, the tag will have 3 items: the first name, last name, and birthdate of the patient in that order. 
		///The birthdate will be in DateTime.Ticks.</summary>
		CI_CheckinPatient,
		///<summary>Tells the device that is currently filling out sheets to add a sheet to the list. For this type, the list of primary keys will have two items: the patnum and
		///the SheetNum in that order.</summary>
		CI_AddSheet,
		///<summary>Tells the device that is currently fillout out sheets to remove a sheet from the list. For this type, the list of primary keys will have two items: the patnum
		///and the SheetNum in that order.</summary>
		CI_RemoveSheet,
		///<summary>This mobile notification tells the device to stop whatever it is doing and go to a fresh checkin page. This may be a blank self-checkin or may be waiting for a mobile notification. This
		///allows users from OD to "clear" the device of a stale patient. No primary keys or tags needed.</summary>
		CI_GoToCheckin,
		///<summary>This mobile notification occurs when the preferences for this device's clinic changes. The tags for this mobile notification will be the EClipboardAllowSelfCheckIn(bool), EClipboardMessageComplete(string), 
		///EClipboardAllowSelfPortraitOnCheckIn(bool), and EClipboardPresentAvailableFormsOnCheckIn(bool) in that order.</summary>
		CI_NewEClipboardPrefs,
		///<summary>This mobile notification occurs when the MobileAppDevice.IsAllowed changed for this device. The tag for this mobile notification will be IsAllowed (bool).
		///If true then device which is currently awaiting in 'Not Allowed' state will try another login, should work this time. If false then force signout. Used for eClipboard and ODTouch.</summary>
		IsAllowedChanged,
		///<summary>This mobile notification occurs when a permission has changed for a given OD user and they are no longer allowed to use OD Mobile. 
		///The ListPrimaryKeys may contain the UserNum of the user who is no longer allowed. This session will then be logged out of versioned OD Mobile.
		///If ListPrimaryKeys IsNullOrEmpty() then assume all users for the given ClinicNum should be logged out. No UserNum filter necessary in this case.</summary>
		ODM_LogoutODUser,
		///<summary>This mobile notification occurs when a OD proper user sends a patients treatment plan to a specific device to show the user.
		///ListPrimaryKeys => [MobileDataByteNum, PatNum, TreatPlanNum].
		///ListTags Keys => The treatPlan.Heading, hasPracticeSig(Obsolete; based on if TP sheet has SigBoxPractice) .</summary>
		CI_TreatmentPlan,
		/// <summary>
		/// This mobile notification occurs when a TreatmentPlan is deleted in OD and we want to tell a specific device so that they can remove it when viewing TreatmentPlans.
		/// ListPrimaryKeys => [TreatPlan.PatNum,TreatPlan.TreatPlanNum]
		/// </summary>
		CI_RemoveTreatmentPlan,
		///<summary>This mobile notification occurs when a payment needs to be made on an eClip device. This either adds the Make Payment action item to the checkin checklist
		///or it will open the QR code to scan from OD.
		///ListPrimaryKeys => [TreatPlan.PatNum]</summary>
		CI_SendPayment,
		///<summary>This mobile notification occurs when a patient is currently on the device, when a payment is made, when a new card is added (XWeb only), and when a new 
		///statement is created in OD.
		///ListPrimaryKeys => [PatNum]</summary>
		CI_RefreshPayment,
		///<summary>This mobile notification occurs when an OD proper user sends a payment plan to a specific device.
		///ListPrimaryKeys => [MobileDataByte.MobileDataByteNum,PayPlan.PatNum,PayPlan.PayPlanNum]
		///ListTags => [PayPlan.PayPlanDate]</summary>
		CI_PaymentPlan,
		///<summary>This mobile notification occurs when a payment plan is removed from the associated eClip device or when a payment plan is removed from OD proper.
		///This will remove a payment plan from user view on eClip.
		///ListPrimaryKeys => [PayPlan.PatNum,PayPlan.PayPlanNum]</summary>
		CI_RemovePaymentPlan,
		///<summary></summary>
		ODT_ExamSheetsAll,
		///<summary></summary>
		ODT_ExamSheet,
	}

	///<summary>The mobile apps that support mobile notifications. Must stay synched 1:1 with the ODXamBusiness.ApplicationTarget enum.</summary>
	public enum EnumAppTarget {
		///<summary>0</summary>
		eClipboard, 
		///<summary>1</summary>
		ODMobile,
		///<summary>2</summary>
		ODTouch,
	}
}