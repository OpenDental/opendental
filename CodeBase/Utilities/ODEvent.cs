using System;

namespace CodeBase {
	///<summary>Helper class to allow multiple areas of the program to subscribe to various events which they care about.</summary>
	public class ODEvent {
		///<summary>Occurs when any developer calls Fire().  Can happen from anywhere in the program.
		///Consumers of "global" ODEvents need to register for this handler because this will be the event that mainly gets fired.</summary>
		public static event ODEventHandler Fired;

		///<summary>Triggers the global Fired event to get called with the passed in arguments.</summary>
		public static void Fire(ODEventType odEventType,object tag) {
			Fired?.Invoke(new ODEventArgs(odEventType,tag));
		}
	}

	///<summary>Arguments specifically designed for use in ODEvent.</summary>
	public class ODEventArgs {
		private object _tag;
		private ODEventType _eventType=ODEventType.Undefined;

		///<summary>A generic object related to the event, such as a Commlog object.  Can be null.</summary>
		public object Tag {
			get {
				return _tag;
			}
		}

		///<summary>Used to uniquly identify this ODEvent for consumers.  And event type of Undefined will be treated as a generic ODEvent.</summary>
		public ODEventType EventType {
			get {
				return _eventType;
			}
		}

		///<summary>Used when an ODEvent is needed but no object is needed in the consuming class.</summary>
		public ODEventArgs(ODEventType eventType) : this(eventType,null) {
		}

		///<summary>Creates an ODEventArg with the specified ODEventType and Tag passed in that is designed to be Fired to a progress window.</summary>
		///<param name="eventType">Progress windows registered to this ODEventType will consume this event arg and act upon the tag accordingly.
		///An event type of Undefined will be treated as a generic ODEvent.</param>
		///<param name="tag">Tag can be set to anything that the consumer may need.  E.g. a string for FormProgressStatus to show to users.</param>
		public ODEventArgs(ODEventType eventType,object tag) {
			_tag=tag;
			_eventType=eventType;
		}
	}

	///<summary>Only used for ODEvent.  Not necessary to reference this delegate directly.</summary>
	public delegate void ODEventHandler(ODEventArgs e);

	///<summary>Progress windows will be monitoring for these specific event types.  New ones should be alphabetical.</summary>
	public enum ODEventType {
		///<summary>0 - The event type has not been set.  Treated as a generic ODEvent.</summary>
		Undefined,
		///<summary>Events that occur when a change has been made to an Appointment.</summary>
		AppointmentEdited,
		///<summary></summary>
		Billing,
		///<summary></summary>
		BugSubmission,
		///<summary>These events will get fired sporadically throughout the FormOpenDental.DataValid_BecameInvalid process.</summary>
		Cache,
		///<summary></summary>
		Clearinghouse,
		///<summary></summary>
		Clinic,
		///<summary>Events that occurs when the commitem window should automatically save.</summary>
		CommItemSave,
		///<summary></summary>
		ConfirmationList,
		///<summary>Events that occur during Images Module operations.</summary>
		ContrImages,
		///<summary></summary>
		ConvertDatabases,
		///<summary>Events that occurs when a crashed table has been detected.</summary>
		CrashedTable,
		///<summary>A database maintenance progress event.</summary>
		DatabaseMaint,
		///<summary>Events that occurs during data connection operations.</summary>
		DataConnection,
		///<summary>Events that cccur when unable to read from the MySQL data adapter.</summary>
		DataReaderNull,
		///<summary>Events that occur when any eClipboard device experiences a status change.</summary>
		eClipboard,
		///<summary>Events that occurs when the Email Message Edit window should automatically save.</summary>
		EmailSave,
		///<summary>Event that occurs when the eRx browser window has closed.</summary>
		ErxBrowserClosed,
		///<summary></summary>
		EServices,
		///<summary></summary>
		Etrans,
		///<summary></summary>
		FeeSched,
		///<summary>Events that occurs when FormClaimSend has invoked the GoTo action.</summary>
		FormClaimSend_GoTo,
		///<summary>Events that occurs when FormProcNotBilled has invoked the GoTo action.</summary>
		FormProcNotBilled_GoTo,
		///<summary>Event that occurs when hiding unused fee schedules.</summary>
		HideUnusedFeeSchedules,
		///<summary>Event that occurs when filling the Insurance Verification list.</summary>
		InsVerification,
		///<summary>Event that occurs within job related windows and events at HQ.</summary>
		Job,
		///<summary>Events that occurs during Middle Tier connection operations.</summary>
		MiddleTierConnection,
		///<summary>Miscellaneous things like backing up and repairing the database.</summary>
		MiscData,
		///<summary>Events that occur when a Module is selected/refreshed.</summary>
		ModuleSelected,
		///<summary>Events that occurs during long ODGrid computations.</summary>
		ODGrid,
		///<summary></summary>
		Patient,
		///<summary></summary>
		PrefL,
		///<summary>This can be used for any event fired from within a progress bar action.</summary>
		ProgressBar,
		///<summary></summary>
		Provider,
		///<summary>Events that occur when a query or Middle Tier web call has started or finished executing.</summary>
		QueryMonitor,
		///<summary></summary>
		RecallSync,
		///<summary>There is currently no progress bar around most of ReportComplex.  Only the queries have progress bars.  Leaving the events in place for now.</summary>
		ReportComplex,
		///<summary></summary>
		Schedule,
		///<summary>Events that occurs when the user sends an appointment to the pinboard from DashApptGrid.</summary>
		SendToPinboard,
		///<summary>Events that occurs when credentials have failed.</summary>
		ServiceCredentials,
		///<summary>The program is shutting down.</summary>
		Shutdown,
		///<summary>Events that occur during Userod operations.  e.g. changing users</summary>
		Userod,
		///<summary>Events that occurs when the wiki edit window should automatically save.</summary>
		WikiSave,


		
	}
}
