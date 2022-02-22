using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes {

	///<summary>Shared between OD and Xamarin. Contains constants and template definitions for push notifications. Xamarin will always use the head
	///version.</summary>
	public class PushNotificationConstants {

		///<summary>The name of the alert template registered with Azure.</summary>
		public static string AlertTemplateName="AlertTemplate";
		///<summary>The name of the background template registered with Azure.</summary>
		public static string BackgroundTemplateName="BackgroundTemplate";
		///<summary>A tag added to a template registration to distinguish the background template from the alert template.</summary>
		public static string BackgroundTag="background";
		///<summary>A tag added to a template registration to distinguish the background template from the alert template.</summary>
		public static string AlertTag="alert";
		///<summary>The key for the title of an alert in the templates.</summary>
		public static string AlertTitleTemplateKey="alertTitle";
		///<summary>The key for the message of an alert in the templates.</summary>
		public static string AlertMessageTemplateKey="alertMessage";
		///<summary>The key for the version in the templates.</summary>
		public static string VersionTemplateKey="notificationVersion";
		///<summary>The key for the versioned json in the templates.</summary>
		public static string PushJsonTemplateKey="pushJson";
		///<summary>The alert template for iOS.</summary>
		public static string AlertTemplateiOS=$"{{\"aps\":{{\"alert\":\"{{$({AlertTitleTemplateKey})+': '+$({AlertMessageTemplateKey})}}\"}},"
			+$"\"acme2\":{{\"{VersionTemplateKey}\":\"$({VersionTemplateKey})\",\"{PushJsonTemplateKey}\":\"$({PushJsonTemplateKey})\"}}}}";
		///<summary>The background template for iOS.</summary>
		public static string BackgroundTemplateiOS="{\"aps\":{\"content-available\":1},"
			+$"\"acme2\":{{\"{VersionTemplateKey}\":\"$({VersionTemplateKey})\",\"{PushJsonTemplateKey}\":\"$({PushJsonTemplateKey})\"}}}}";
		///<summary>The alert template for Android.</summary>
		public static string AlertTemplateAndroid=$"{{\"data\":{{\"{PushJsonTemplateKey}\":\"$({PushJsonTemplateKey})\",\"{VersionTemplateKey}\":\"$("
			+$"{VersionTemplateKey})\",\"{AlertTitleTemplateKey}\":\"$({AlertTitleTemplateKey})\",\"{AlertMessageTemplateKey}\":\"$({AlertMessageTemplateKey})\"}}}}";
		///<summary>The background template for Android.</summary>
		public static string BackgroundTemplateAndroid=$"{{\"data\":{{\"{PushJsonTemplateKey}\":\"$({PushJsonTemplateKey})\",\"{VersionTemplateKey}\":\"$("
			+$"{VersionTemplateKey})\"}}}}";
		public static string AlertTemplateUWP=$"{{\"{VersionTemplateKey}\":\"$({VersionTemplateKey})\",\"{PushJsonTemplateKey}\":\"$("
			+$"{PushJsonTemplateKey})\",\"{AlertTitleTemplateKey}\":\"$({AlertMessageTemplateKey})\"}}";
		public static string BackgroundTemplateUWP=$"{{\"{VersionTemplateKey}\":\"$({VersionTemplateKey})\",\"{PushJsonTemplateKey}\":\"$("
			+$"{PushJsonTemplateKey})\"}}";
		///<summary>The template expiration. We do not expire templates.</summary>
		public static string PushHubExpiry=DateTime.Now.AddYears(5).ToString(CultureInfo.CreateSpecificCulture("en-us"));
		///<summary>The prefix for the RegKeyNum tag.</summary>
		public static string RegKeyNumPrefix="RegKeyNum:";
		///<summary>The prefix for the ClinicNum tag.</summary>
		public static string ClinicNumPrefix="ClinicNum:";
		///<summary>The prefix for the DeviceId tag.</summary>
		public static string DeviceIdPrefix="DeviceId:";
		///<summary>The prefix for the UserNum tag.</summary>
		public static string UserNumPrefix="UserNum:";
	}

}
