using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes {

	///<summary>Represents the payload that comes to HQ to send a push notification. Do NOT change field names after release to preserve as much backwords compatibility
	///as possbile.</summary>
	public class PushNotificationPayload {

		///<summary>Indicates if this push notification is an alert. If not, it is a background refresh.</summary>
		public bool IsAlert;
		///<summary>The title of the alert. Only used if IsAlert is true.</summary>
		public string AlertTitle;
		///<summary>The message of the alert. Only used if IsAlert is true.</summary>
		public string AlertMessage;
		///<summary>The clinic num for the push notification. Used as a tag.</summary>
		public long ClinicNum;
		///<summary>The device id for the push notification. Used as a tag.</summary>
		public string DeviceId;
		///<summary>The user num for the push notification. Used as a tag. Only applies to ODMobile. If 0 then UserNum tag will be ommitted.</summary>
		public long UserNum;
		///<summary>The serialized push notification action. This object can be changed as much as needed from version to version. Stores
		///the steps the device will take when the push notification is received.</summary>
		public string PushNotificationActionJSON;

	}

}
