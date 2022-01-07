using System;
using System.Collections.Generic;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary>The different type of account transmissions available to send.</summary>
	public enum AccountTransmissionType {
		///<summary></summary>
		Unknown,
		///<summary>Occurs when an email transmission's status changes. Payload: TransmissionNum = long,TransmissionStatus = string. The EmailTranmssionNum that the account was given when they sent the email (for mass).</summary>
		EmailTransmissionStatus,
		///<summary>The verification status of an identity has changed. Payload: String.</summary>
		IdentityVerificationChanged,
		///<summary>The DKIM status of an identity has changed. Payload: String.</summary>
		IdentityDKIMStatusChanged,
		///<summary>We are letting the user know they are back in the healthy range for email sending. Payload: String.</summary>
		SendingStatusHealthy,
		///<summary>We are warning the users that a certain percent of their emails have been complained about or bounced. Payload: String.</summary>
		SendingStatusWarning,
		///<summary>We are alerting the users we have paused sending on their account due to the number of bounced/complaint emails. Payload: String.</summary>
		SendingStatusStopped,
		///<summary>We are alerting the user that a new email has been received and is available for download. Payload: List{new {long EmailFK,long EmailChainFK}}</summary>
		SecureEmailReceived,
	}
}
