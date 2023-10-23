using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	/*
Jordan 2023-09-03 This is an interesting bunch of classes.
One problem is that they should really all be one class which would probably be done by merging them into the ODEvent class.
Another problem is that, while some of the methods seem to be wired up, many of the corresponding events are not,
which means they are either doing nothing or they are being used via reflection, neither of which is any good.
A final problem is that they all take the event type as the first argument, even though we already know the event type.
I think the solution is to move each of them over to ODEvent.
All of the different Fired events will be reduced to one,
and all of the different Fire methods will be reduced to one.
There's already an existing pattern where each event handler filters out the type they want, and we'll keep doing that.
I started with the ProgressBar type, and it was a very easy switch.
	*/
	public class MiscDataEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class AppointmentEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class BillingEvent:IODEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
		public void FireEvent(ODEventArgs e) { Fired?.Invoke(e); }
	}

	public class BugSubmissionEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class ClearinghouseEvent:IODEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
		public void FireEvent(ODEventArgs e) { Fired?.Invoke(e); }
	}

	public class ClinicEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class CommItemSaveEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag=null) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class ConfirmationListEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class CredentialsFailedAfterLoginEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
		public static bool IsFiredNull() { return Fired==null; }
	}

	public class DatabaseMaintEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class EmailSaveEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag=null) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class EClipboardEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag=null) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class EServicesEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class EtransEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class FeeSchedEvent:IODEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
		public void FireEvent(ODEventArgs e) { Fired?.Invoke(e); }
	}

	public class GeneralProgramEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class InsuranceVerificationEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class JobEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class ODGridEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag=null) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class PatientChangedEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class PatientDashboardDataEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class PatientEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	//public class ProgressBarEvent {
		//See notes at top of page
		//public static event ODEventHandler Fired;
		//public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	//}

	public class RecallSyncEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class ReportComplexEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class ScheduleEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class SendToPinboardEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class UserodChangedEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}

	public class WikiSaveEvent {
		public static event ODEventHandler Fired;
		public static void Fire(ODEventType odEventType,object tag=null) { Fired?.Invoke(new ODEventArgs(odEventType,tag)); }
	}
}
