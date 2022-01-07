using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Stores information on mobile app devices. These are devices that utilize the Xamarin mobile application.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class MobileAppDevice : TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileAppDeviceNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>The name of the device.</summary>
		public string DeviceName;
		///<summary>The unique identifier of the device. Platform specific.</summary>
		public string UniqueID;
		///<summary>Indicates whether the device is allowed to operate the checkin app. 
		///For BYOD sessions will always be true because BYOD is authenticated by a unique URL link in a text message.</summary>
		public bool IsAllowed;
		///<summary>FK to patient.PatNum. Indicates which patient is currently using the device. 0 indicates the device is not in use. -1 indicates
		///that the device is in use but we do not yet know which patient is using the device.</summary>
		public long PatNum;
		///<summary>Indicates whether a device is a BYOD device, defaults to false.</summary>
		public bool IsBYODDevice;
		///<summary>The date and time when we last updated the PatNum field for this device (indication the current use-state of the device).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastCheckInActivity;
		///<summary>The date and time of the last attempted login.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastAttempt;
		///<summary>The date and time of the last succesful login.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastLogin;

		///<summary>Returns a copy of this MobileAppDevice.</summary>
		public MobileAppDevice Copy() {
			return (MobileAppDevice)this.MemberwiseClone();
		}
	}

}

