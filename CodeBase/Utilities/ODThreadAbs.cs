using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase {
	public abstract class ODThreadAbs : IODThread {

		private ODThread _thread=null;
		///<summary>Used to identify whether or not an ODThread has been initialized.  
		///This is used to avoid creating more instances of the same thread from ODThread.MakeThread()</summary>
		private bool _isInit;
		private LogLevel _logLevelThread;

		public ODThread ODThread {
			get {
				return _thread;
			}
			set {
				_thread=value;
			}
		}
		
		public bool IsInit {
			get {
				return _isInit;
			}
			set {
				_isInit=value;
			}
		}

		///<summary>The default level at which this thread should log.</summary>
		public LogLevel LogLevelThread {
			get {
				return _logLevelThread;
			}
			set {
				_logLevelThread=value;
			}
		}

		///<summary>The directory in \OpenDental\OpenDentalService\Logger that holds the log files that are written to by CodeBase.Logger.cs</summary>
		public abstract string GetLogDirectoryName();
		
		///<summary>The name of the thread for debugging/development purposes.</summary>
		public abstract string GetThreadName();
		
		///<summary>The interval of time in milliseconds to wait between calling OnThreadRun</summary>
		public abstract int GetThreadRunIntervalMS();
		
		
		///<summary>What the thread does every time the specified interval of time has passed.</summary>
		public abstract void OnThreadRun(ODThread odThread);
		
		
		///<summary>What the thread does when it catches an unhandled exception.</summary>
		public void OnThreadException(Exception e) {
			Logger.WriteException(e,GetLogDirectoryName());
		}
		
		
		///<summary>What the thread does right before it dies.</summary>
		public virtual void OnThreadExit(ODThread odThread) {
		}
		
		
		///<summary>Asynchronously stops the thread.  Guards against re-entrance.</summary>
		public void Stop() {
			if(_thread!=null) {
				_thread.QuitAsync();
			}
		}

	}
}
