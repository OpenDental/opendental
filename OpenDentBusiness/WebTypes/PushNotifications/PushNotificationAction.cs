using System;
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
		///<summary>Alert. This push occurs when a new text message is sent to the office. The ListPrimaryKeys will contain the PatNum of the message and the SmsFromMobileNum in that order.
		///The ListTags contains the phone number.</summary>
		ODM_NewTextMessage,
		///<summary>Alert push. Opens the generic eRx view without a patient selected so the user can see their eRx notifications.</summary>
		ODM_Erx,
		///<summary>Alert push. Opens the eRx view with a specific patient selected to view their DS chart.
		///ListPrimaryKeys => [PatNum]</summary>
		ODM_ErxPatient,
	}

}
