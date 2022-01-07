using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CodeBase {
	public static class ODExceptionExtensions {
		///<summary>Does nothing to the object except reference it. Usefull for handling the Exception ex declared but never used warning.</summary>
		public static void DoNothing(this Exception ex) { }
	}

	[Serializable]
	public class ODException:ApplicationException {
		private int _errorCode=0;
		///<summary>Contains query text when an ErrorCode in the 700s was thrown. This is the query that was attempted prior to an exception.</summary>
		private string _query="";

		///<summary>Gets the error code associated to this exception.  Defaults to 0 if no error code was explicitly set.</summary>		
		public int ErrorCode {
			get {
				return _errorCode;
			}
		}

		///<summary>Contains query text when an ErrorCode in the 700s was thrown. This is the query that was attempted prior to an exception.</summary>
		public string Query {
			get {
				return _query??"";
			}
		}

		///<summary>Convert an int to an Enum typed ErrorCode. Returns NotDefined if the input errorCode is not defined in ErrorCodes.</summary>		
		public static ErrorCodes GetErrorCodeAsEnum(int errorCode) {
			if(!Enum.IsDefined(typeof(ErrorCodes),errorCode)) {
				return ErrorCodes.NotDefined;
			}
			return (ErrorCodes)errorCode;
		}

		///<summary>Gets the pre-defined error code associated to this exception.  
		///Defaults to NotDefined if the error code (int) specified is not defined in ErrorCodes enum.</summary>		
		public ErrorCodes ErrorCodeAsEnum {
			get {
				return GetErrorCodeAsEnum(_errorCode);
			}
		}

		public ODException() { }

		public ODException(int errorCode) : this("",errorCode) { }

		public ODException(string message) : this(message,0) { }

		public ODException(string message,ErrorCodes errorCodeAsEnum) : this(message,(int)errorCodeAsEnum) { }

		public ODException(string message,int errorCode)
			: base(message) {
			_errorCode=errorCode;
		}
		
		///<summary>Used for query based exceptions in Db.cs</summary>
		public ODException(string message,string query,Exception ex) : base(message,ex) {
			_query=query;
			_errorCode=(int)ErrorCodes.DbQueryError;
		}

		public ODException(string message,Exception ex) : base(message,ex) {
		}

		///<summary>Used for serialization.</summary>
		protected ODException(SerializationInfo info,StreamingContext context) : base(info,context) {
			_errorCode=info.GetInt32(nameof(ErrorCode));
			_query=info.GetString(nameof(Query));
		}

		///<summary>Used for serialization.</summary>
		[SecurityPermission(SecurityAction.Demand,SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info,StreamingContext context) {
			if(info==null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue(nameof(ErrorCode),ErrorCode);
			info.AddValue(nameof(Query),Query);
			base.GetObjectData(info,context);
		}

		///<summary>Wrap the given action in a try/catch and swallow any exceptions that are thrown. 
		///This should be used sparingly as we typically want to handle the exception or let it bubble up to the UI but sometimes you just want to ignore it.</summary>
		public static void SwallowAnyException(Action a) {
			try {
				a();
			}
			catch(Exception ex) { 
				ex.DoNothing();
			}
		}

		///<summary>Swallows and logs any exception that thrown from executing the action..</summary>
		public static void SwallowAndLogAnyException(string subDirectory,Action a) {
			try {
				a();
			}
			catch(Exception ex) {
				Logger.WriteLine(MiscUtils.GetExceptionText(ex),subDirectory);
			}
		}

		///<summary>Does nothing if the exception passed in is null. Preserves the callstack of the exception passed in.
		///Typically used when a work thread throws an exception and we want to wait until we are back on the main thread in order to throw the exception.
		///Calling this when there is no worker thread involved is harmless and unnecessary but will still preserve the call stack.</summary>
		public static void TryThrowPreservedCallstack(Exception ex) {
			if(ex==null) {
				return;
			}
			//We are back in the main thread context so throw a new exception which contains our actual exception (from the worker) as the innner exception.
			//Simply throwing ex here would cause the stack trace to be lost. https://stackoverflow.com/q/3403501
			throw new Exception(ex.Message,ex);
		}

		///<summary>Predefined ODException.ErrorCode field values. ErrorCode field is not limited to these values but this is a convenient way defined known error types.
		///These values must be converted to/from int in order to be stored in ODException.ErrorCode.
		///Number ranges are arbitrary but should reserve plenty of padding for the future of a given range.
		///Each number range should share a similar prefix between all of it's elements.</summary>
		public enum ErrorCodes {
			///<summary>0 is the default. If the given (int) ErrorCode is not defined here, it will be returned at 0 - NotDefined.</summary>
			NotDefined=0,
			//100-199 range. Values used by ODSocket architecture.
			///<summary>No immortal socket connection found for this RegistrationKeyNum.
			///The Proxy is trying to communicate with this eConnector but the eConnector does not have an active connection.</summary>
			ODSocketNotFoundForRegKeyNum=100,
			///<summary>Immortal socket connection was found by Proxy but the remote eConnector socket is not responding. 
			///Most likely because the eConnector has been turned off but the Proxy has not performed an ACK to discover that it's off.</summary>
			ODSocketEConnectorNotResponding=101,
			//200-299 range. Values used by 3rd party integrations.
			///<summary>.</summary>
			OtkArgsInvalid=200,
			///<summary>.</summary>
			OtkCreationFailed=201,
			///<summary>.</summary>
			MaxRequestDataExceeded=202,
			///<summary>.</summary>
			XWebProgramProperties=203,
			///<summary>.</summary>
			PayConnectProgramProperties=204,
			///<summary>.</summary>
			WebPaySetup=205,
			///<summary>DoseSpot user not authorized to perform action.</summary>
			DoseSpotNotAuthorized=206,
			///<summary>An API request to XWeb DTG was failed by XWeb.</summary>
			XWebDTGFailed=207,
			//400-499 range. Values used by web apps
			///<summary>No patient found that matches the specified parameters.</summary>
			NoPatientFound=400,
			///<summary>More than one patient found that matches the specified parameters.</summary>
			MultiplePatientsFound=401,
			///<summary>No appointment found that matches the specified parameters.</summary>
			NoAppointmentFound=402,
			///<summary>The time slot provided was not found or invalid.</summary>
			TimeSlotInvalid=403,
			///<summary>The response status provided is not acceptable.</summary>
			ResponseStatusInvalid=404,
			///<summary>No asapcomm found that matches the specified parameters.</summary>
			NoAsapCommFound=405,
			///<summary>No operatories have been set up for Web Sched.</summary>
			NoOperatoriesSetup=406,
			/// <summary>Recall not found for specified patient</summary>
			NoRecallFound=407,
			///<summary>Recall has already been scheduled</summary>
			RecallAlreadyScheduled=408,
			///<summary>The user's session has expired.</summary>
			SessionExpired=409,
			//500-599 range. Values used by Open Dental UI.
			FormClosed = 500,
			//600-699 range. Values used by RemotingClient/MiddleTier
			///<summary>After successfully logging in to Open Dental, a middle tier call to Userods.CheckUserAndPassword returned an "Invalid user or password" error.</summary>
			CheckUserAndPasswordFailed=600,
			//700-799 range. Values used by failed query exceptions.
			///<summary>Generic database command failed to execute.</summary>
			DbQueryError=700,
			//800-899 range. Values used by BugSubmissions.
			///<summary>Specific error code that represents an unhandled exception that has a uniquely formatted Message property.
			///The goal of this special error code is to preserve StackTrace information from inner exceptions.
			///The first line of this UE's Message property will become the bug submission's ExceptionMessageText field.
			///All subsequent lines of this UE's Message property will become the bug submission's ExceptionStackTrace field.
			///This specific ErrorCode will be looked for within the heart of the BugSubmission constructor.</summary>
			BugSubmissionMessage=800,
			//900-999 range. Values used by Texting services.
			///<summary>Zipwhip verification pending.</summary>
			ZipwhipPendingVerification=900,
			///<summary>An individual SMS is trying to include an HTTPS short url redirect link, but has already used all available HTTPS enabled domains
			///on previous send attempts.  This should be used to simply fail the SMS.</summary>
			NoAvailableHttpsShortUrlRedirectDomains=901,
			//4000-4999. Values used by ODCloud
			///<summary>The file trying to write exists.</summary>
			FileExists=4000,
			///<summary>Unable to communicate with ODCloudClient.</summary>
			ODCloudClientTimeout=4001,
			///<summary>Error occurred when attempting to archive old claims.</summary>
			ClaimArchiveFailed=4002,
			///<summary>Unable to communicate with the browser.</summary>
			BrowserTimeout=4003,
			//5000-5099. Values used by LamportsBakery
			///<summary>Failed to execute locking logic more than the permitted number of attempts.</summary>
			LamportsBakeryLockMaxAttempts=5000,
			///<summary>A request/ticket to lock a process has been lost while evaluating locking queue.</summary>
			LamportsBakeryMissingTicket=5001,
		}
	}
}
