using Dropbox.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDentalCloud {

	public delegate void ProgressHandler(double newCurVal,string newDisplayText,double newMaxVal,string errorMessage);

	public abstract class TaskState {

		public ProgressHandler ProgressHandler;
		public bool IsDone { private set; get; }
		public bool HasExceptions { get; set; }
		public Exception Error { private set; get; }
		///<summary>This is a quick way to determine if there were complications with deleting the file.  
		///The Error variable can provide more info.</summary>
		public bool HasFailed {
			get {
				return (IsDone && Error!=null);
			}
		}
		///<summary>This property allows the user with this TaskState to cancel an async task if they so choose.  
		///This is usually wired up to a Cancel button in a progress form.</summary>
		public bool DoCancel { get; set; }
		protected object _lock=new object();

		protected void OnProgress(double newCurVal,string newDisplayText,double newMaxVal,string errorMessage) {
			if(ProgressHandler==null) {
				return;
			}
			ProgressHandler(newCurVal,newDisplayText,newMaxVal,errorMessage);
		}

		protected abstract Task PerformIO();

		///<summary>Runs PerformIO logic behind a synchronous or asynchronous task.  See implemented class for PerformIO logic.</summary>
		public void Execute(bool isAsync) {
			if(isAsync) {
				new Task(async () => {
					try {
						//Effectively makes this a blocking call within the context of this anonymous task.
						await PerformIO();
						OnProgress(0,"",0,"");//This will automatically close the FormProgress window.		
					}
					catch(Exception e) {
						Error=e;
						OnProgress(0,"",100,e.Message);
					}
					finally {
						IsDone=true;
						if(HasExceptions && HasFailed) {
							throw Error;
						}
					}
				}).Start();
			}
			else {
				ManualResetEvent wait=new ManualResetEvent(false);
				new Task(async () => {
					try {
						await PerformIO();
					}
					catch(Exception e) {
						Error=e;
					}
					finally {
						IsDone=true;
						wait.Set();
					}
				}).Start();
				if(!wait.WaitOne(-1)) {
					throw new Exception("Action timed out.");
				}
				if(HasExceptions && HasFailed) {
					throw Error;
				}
			}
		}
	}
}
