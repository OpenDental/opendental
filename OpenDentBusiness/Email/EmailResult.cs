using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenDentBusiness.Email {

	///<summary>Represents what will be sent back to the eConnector for each request. Will provide details on errors, exceptions, and timeouts.</summary>
	public class EmailResult {
		///<summary>A unique identifier for the given response. Allows the eConnector to provide the results to the correct thread waiting on them.</summary>
		public string RequestIdentifier;
		///<summary>The outcome of the email being sent.</summary>
		public SendOutcome Status;
		///<summary>If an exception occurs that is not a timeout, the exception will be sent back to the eConnector and thrown on that thread.</summary>
		public string ExceptionText;
	}

	///<summary>Represents the overall outcome of the email to be sent</summary>
	public enum SendOutcome {
		///<summary>No attempt made to send the email.  Should be resent.</summary>
		SendNotAttempted,
		///<summary>This request is telling the eConnector that the process has finished sending its emails and it is going to die. This is only the 
		///response for when the eConnector specifically tells the process to die.</summary>
		ProcessStopped,
		///<summary>The email sent successfully.</summary>
		Success,
		///<summary>Some other error/exception occurred. The exception will be serialized and sent back to the eConnector.</summary>
		Error,
		///<summary>Gmail OAuth2 token is expired/invalid.</summary>
		ExpiredToken,
		///<summary>External process is in a state where it wants to be told to shut down.</summary>
		HaltRequests,
	}

}
