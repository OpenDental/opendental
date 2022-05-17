namespace OpenDentBusiness.Email {

	///<summary>Represents what will be sent to the OpenDentalEmail process. May be an email or a command for the process to carry out. Serialized
	///in XML.</summary>
	public class EmailRequest {
		///<summary>A unique identifier for the given request. Allows the eConnector to provide the results to the correct thread waiting on them.</summary>
		public string RequestIdentifier;
		///<summary>The address of the email to be sent.</summary>
		public BasicEmailAddress Address;
		///<summary>The message of the email to be sent. All of the processing in terms of images, attachments, and scrubbing is done on the eConnector
		///side.</summary>
		public BasicEmailMessage Message;
		///<summary>Indicates whether the eConnector is telling the process to kill itself. The process will finish up what its doing before dying
		///a graceful death.</summary>
		public bool IsKillCommand;

		public EmailRequest() {

		}

		public EmailRequest(string requestID="",BasicEmailAddress address=null,BasicEmailMessage message=null) {
			RequestIdentifier=requestID;
			Address=address;
			Message=message;
		}

	}

}
