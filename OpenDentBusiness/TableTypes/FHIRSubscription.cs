using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	///<summary>A subscription by a client that requests an alert whenever a change is made to a FHIR resource.</summary>
	[Serializable]
	public class FHIRSubscription:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FHIRSubscriptionNum;
		///<summary>Rule for server push criteria.</summary>
		public string Criteria;
		///<summary>Description of why this subscription was created.</summary>
		public string Reason;
		///<summary>Enum:SubscriptionStatus </summary>
		public SubscriptionStatus SubStatus;
		///<summary>Latest error note.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ErrorNote;
		///<summary>Enum:SubscriptionChannelType </summary>
		public SubscriptionChannelType ChannelType;
		///<summary>Where the channel points to.</summary>
		public string ChannelEndpoint;
		///<summary>Mimetype to send, or blank for no payload.</summary>
		public string ChannelPayLoad;
		///<summary>Usage depends on the channel type.</summary>
		public string ChannelHeader;
		///<summary>When to automatically delete the subscription.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateEnd;
		///<summary>A hash of the API key that was used in the request to create this subscription.</summary>
		public string APIKeyHash;
		///<summary>List of attached ContactPoints for this Subscription.  Limit in db: 16M char.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<FHIRContactPoint> _listContactPoints;

		///<summary>Set list to null to force refresh.</summary>
		[XmlIgnore, JsonIgnore]
		public List<FHIRContactPoint> ListContactPoints {
			get {
				if(_listContactPoints==null) {
					_listContactPoints=FHIRContactPoints.GetContactPoints(FHIRSubscriptionNum);
				}
				return _listContactPoints;
			}
			set {
				_listContactPoints=value;
			}
		}

		///<summary></summary>
		public FHIRSubscription Copy() {
			FHIRSubscription retVal=(FHIRSubscription)this.MemberwiseClone();
			retVal.ListContactPoints=this.ListContactPoints.ToList();//Deep copy because of value type
			return retVal;
		}

	}

	///<summary>The status of a Subscription. https://www.hl7.org/fhir/valueset-subscription-status.html </summary>
	public enum SubscriptionStatus {
		///<summary>The client has requested the subscription, and the server has not yet set it up.</summary>
		Requested,
		///<summary>The subscription is active.</summary>
		Active,
		///<summary>The server has an error executing the notification.</summary>
		Error,
		///<summary>Too many errors have occurred or the subscription has expired.</summary>
		Off
	}

	///<summary>Only rest_hook is supported by Open Dental FHIR. https://www.hl7.org/fhir/valueset-subscription-channel-type.html </summary>
	public enum SubscriptionChannelType {
		///<summary>The channel is executed by making a post to the URI. If a payload is included, the URL is interpreted as the service base, and an 
		///update (PUT) is made.</summary>
		Rest_Hook,
		///<summary>The channel is executed by sending a packet across a web socket connection maintained by the client. The URL identifies the 
		///websocket, and the client binds to this URL.</summary>
		Websocket,
		///<summary>	The channel is executed by sending an email to the email addressed in the URI (which must be a mailto:).</summary>
		Email,
		///<summary>The channel is executed by sending an SMS message to the phone number identified in the URL (tel:).</summary>
		Sms,
		///<summary>	The channel is executed by sending a message (e.g. a Bundle with a MessageHeader resource etc.) to the application identified in 
		///the URI.</summary>
		Message
	}
}
