using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	///<summary>A subscription by an API client that requests events to be fired for db changes or ui actions. Events are currently sent blindly. In the future, we could support acking for db events, but not very useful for ui events.</summary>
	[Serializable]
	public class ApiSubscription:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApiSubscriptionNum;
		///<summary>This is the URL endpoint to which events will be sent. </summary>
		public string EndPointUrl;
		///<summary>Name of the workstation that will fire events. Blank if you want all workstations to fire events. </summary>
		public string Workstation;
		///<summary>API Key the subscribing developer gave the customer. There can be multiple 3rd parties products for one database, each with their own key.</summary>
		public string CustomerKey;
		///<summary>Enum: EnumWatchTable, stored as string </summary>
		public string WatchTable;
		///<summary>Frequency of database polling, in seconds. </summary>
		public int PollingSeconds;
		///<summary>Enum: EnumApiUiEventType, stored as string.</summary>
		public string UiEventType;
		///<summary>When the subscription started. This gets updated each time db is polled so that it represents the start of the date range for the next polling.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStart;
		///<summary>When the subscription will expire. MinVal 01-01-0001 if no expiration. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStop;
		///<summary>.</summary>
		public string Note;

		public ApiSubscription Copy(){
			return (ApiSubscription)this.MemberwiseClone();
		}
	}

	///<summary>Tables that can be monitored for API Subcriptions. These are stored in the database as strings and are converted to strings.</summary>
	public enum EnumWatchTable {
		///<summary>This value is not stored in the database. Just use empty string.</summary>
		None,
		Appointment,
		AppointmentDeleted,
		Operatory,
		PatField,
		Patient,
		Provider,
		Schedule
	}

	///<summary>These are stored in the database as strings and are converted to strings. Do not alter spellings.</summary>
	public enum EnumApiUiEventType{
		///<summary>This value is not stored in the database. Just use empty string.</summary>
		None,
		PatientSelected,
	}

}
