using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormConnectionLost:FormODBase {
		private bool _shouldWindowClose=false;
		private Func<bool> _funcShouldWindowClose;
		private ODThread _threadShouldWindowClose;
		///<summary>The type of ODEvents that this window should listen to.</summary>
		private ODEventType _odEventType;
		///<summary>The "Fired" event that is currently registered to this form.</summary>
		private EventInfo _eventInfoFired;

		///<summary>funcShouldWindowClose should return a boolean indicating if this window should close or not.
		///Optionally set errorMessage to override the label text that is displayed to the user.
		///Optionally set a custom eventType in order to listen for specific types of ODEvent.Fired events.  Defaults to DataConnectionEvent.</summary>
		public FormConnectionLost(Func<bool> funcShouldWindowClose,ODEventType odEventType=ODEventType.Undefined,string errorMessage=""
			,Type eventType=null)
		{
			InitializeComponent();
			InitializeLayoutManager();
			labelErrMsg.Text=errorMessage;
			_funcShouldWindowClose=funcShouldWindowClose;
			_odEventType=odEventType;
			if(eventType==null) {
				eventType=typeof(DataConnectionEvent);
			}
			//Make sure that the event type passed in has a "Fired" event to listen to.
			ODException.SwallowAnyException(() => _eventInfoFired=eventType.GetEvent("Fired"));
			if(_eventInfoFired==null) {
				throw new ApplicationException("The 'eventType' passed into FormConnectionLost does not have a 'Fired' event.\r\n"
					+"Type passed in: "+eventType.GetType());
			}
			Delegate delegateFired=GetDelegateFired(_eventInfoFired.EventHandlerType);
			MethodInfo methodAddHandler=_eventInfoFired.GetAddMethod();
			methodAddHandler.Invoke(this,new object[] { delegateFired });
		}

		private void FormConnectionLost_Load(object sender,EventArgs e) {
			//Spawn a thread that will monitor _shouldWindowClose every 500 MS (expecting another thread or the Retry button to set it correctly).
			if(_funcShouldWindowClose!=null) {
				StartShouldCloseMonitor();
			}
		}

		private Delegate GetDelegateFired(Type eventHandlerType) {
			if(_eventInfoFired.EventHandlerType==typeof(CrashedTableEventHandler)) {
				return Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,nameof(CrashedTableEvent_Fired));
			}
			else if(_eventInfoFired.EventHandlerType==typeof(DataConnectionEventHandler)) {
				return Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,nameof(DataConnectionEvent_Fired));
			}
			else if(_eventInfoFired.EventHandlerType==typeof(MiddleTierConnectionEventHandler)) {
				return Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,nameof(MiddleTierConnectionEvent_Fired));
			}
			else if(_eventInfoFired.EventHandlerType==typeof(DataReaderNullEventHandler)) {
				return Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,nameof(DataReaderNullEvent_Fired));
			}
			else {
				throw new ApplicationException("The 'EventHandlerType' on the eventType.Fired event passed into FormConnectionLost is not supported.\r\n"
					+"EventHandlerType passed in: "+_eventInfoFired.EventHandlerType);
			}
		}

		private void StartShouldCloseMonitor() {
			if(_threadShouldWindowClose!=null) {
				return;
			}
			_threadShouldWindowClose=new ODThread(500,(o)=> {
				if(_shouldWindowClose) {
					ODException.SwallowAnyException(() => {
						Invoke(new Action(() => { DialogResult=DialogResult.OK; }));
					});
					o.QuitAsync();
				}
			});
			_threadShouldWindowClose.Name="FormConnectionLost_ShouldCloseMonitorThread";
			_threadShouldWindowClose.AddExceptionHandler((e) => e.DoNothing());
			_threadShouldWindowClose.AddExitHandler((e) => _threadShouldWindowClose=null);
			_threadShouldWindowClose.Start();
		}

		public void CrashedTableEvent_Fired(CrashedTableEventArgs e) {
			try {
				//Check to see if an ODEventType was set otherwise check the ODEventName to make sure this is an event that this instance cares to process.
				if(_odEventType!=ODEventType.Undefined) {
					if(_odEventType!=e.EventType) {
						return;
					}
				}
				if(!e.IsTableCrashed) {
					_shouldWindowClose=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		public void DataConnectionEvent_Fired(DataConnectionEventArgs e) {
			try {
				//Check to see if an ODEventType was set otherwise check the ODEventName to make sure this is an event that this instance cares to process.
				if(_odEventType!=ODEventType.Undefined) {
					if(_odEventType!=e.EventType) {
						return;
					}
				}
				if(e.IsConnectionRestored) {
					_shouldWindowClose=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		public void MiddleTierConnectionEvent_Fired(MiddleTierConnectionEventArgs e) {
			try {
				//Check to see if an ODEventType was set otherwise check the ODEventName to make sure this is an event that this instance cares to process.
				if(_odEventType!=ODEventType.Undefined) {
					if(_odEventType!=e.EventType) {
						return;
					}
				}
				if(e.IsConnectionRestored) {
					_shouldWindowClose=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		public void DataReaderNullEvent_Fired(DataReaderNullEventArgs e) {
			if(e.IsQuerySuccessful) {
				_shouldWindowClose=true;
			}
		}

		private void butRetry_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			bool isConnected=_funcShouldWindowClose?.Invoke()??false;
			Cursor=Cursors.Default;
			if(isConnected) {
				DialogResult=DialogResult.OK;
			}
		}

		private void butExit_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormConnectionLost_FormClosing(object sender,FormClosingEventArgs e) {
			if(_threadShouldWindowClose!=null && !_threadShouldWindowClose.HasQuit) {
				_threadShouldWindowClose.QuitAsync();
			}
			//Unregister the "Fired" event from this form.  The constructor will throw an exception if _eventInfoFired is not valid.
			Delegate delegateFired=GetDelegateFired(_eventInfoFired.EventHandlerType);
			MethodInfo methodRemoveHandler=_eventInfoFired.GetRemoveMethod();
			methodRemoveHandler.Invoke(this,new object[] { delegateFired });
		}
	}
}