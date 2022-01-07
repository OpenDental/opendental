using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CodeBase {
	///<summary>Deprecated.  Use ProgressOD instead.</summary>
	public class ODProgress {
		///<summary>Explicit locker for _listFormProgresses.</summary>
		private static ReaderWriterLockSlim _lockProgressCur=new ReaderWriterLockSlim();
		///<summary>Used for keeping track of active progress windows.  Multiple progress windows can be shown at the same time.</summary>
		private static List<FormProgressBase> _listActiveProgressForms=new List<FormProgressBase>();

		///<summary>Gets the last progress window in the static list of currently opened progress windows.  Returns null when no active progress windows.
		///The last progress window should be the most recent and thus the one showing to the user (on top of other progress windows).
		///This is treated as the "active progress window" and all message boxes should be ran from its context.</summary>
		public static FormProgressBase FormProgressActive {
			get {
				_lockProgressCur.EnterReadLock();
				try {
					return _listActiveProgressForms.LastOrDefault();
				}
				finally {
					_lockProgressCur.ExitReadLock();
				}
			}
		}

		private static void AddActiveProgressWindow(FormProgressBase formPB) {
			_lockProgressCur.EnterWriteLock();
			try {
				_listActiveProgressForms.Add(formPB);
			}
			finally {
				_lockProgressCur.ExitWriteLock();
			}
		}

		private static void RemoveActiveProgressWindow(FormProgressBase formPB) {
			_lockProgressCur.EnterWriteLock();
			try {
				_listActiveProgressForms.Remove(formPB);
			}
			finally {
				_lockProgressCur.ExitWriteLock();
			}
		}

		///<summary>Non-blocking call. Shows a progress window that will listen for ODEvents in order to update the label and progress bar.
		///The progress window that is shown to the user is owned by a separate thread so that the main thread can continue to execute.
		///This type of progress window is good for showing progress when manipulating UI elements on the main thread (filling grids).
		///It is up to the calling method to let notify this progress window when it should close by invoking the action returned.</summary>
		///<param name="odEventType">Causes the progress window to only process ODEvents of this ODEventType.  Undefined will process all types.</param>
		///<param name="typeEvent">Causes the progress window to only process ODEvents of this Type.  Null defaults to typeof(ODEvent).</param>
		///<param name="startingMessage">Sets the label of the progress window to the value passed in.  Defaults to "Please Wait..."
		///It is always up to the calling method to translate this message before passing it in.  This is to avoid translating multiple times.</param>
		///<param name="hasHistory">Set to true if the progress window should show a history of events that it has processed.
		///This will cause the UI of the progress window to change slightly and will also make it so the user has to click a Close button.</param>
		///<param name="hasMinimize">Set to true if the progress window should allow minimizing.  False by default.</param>
		///<param name="progStyle">Sets the style of the progress bar within the progress window that is going to be shown to the user.</param>
		///<returns>An action that will close the progress window.  Invoke this action whenever long computations are finished.</returns>
		[Obsolete("Deprecated.  Use ProgressOD instead.")]
		public static Action Show(ODEventType odEventType=ODEventType.Undefined,Type typeEvent=null,string startingMessage="Please Wait...",
			bool hasHistory=false,bool hasMinimize=false,ProgressBarStyle progStyle=ProgressBarStyle.Marquee)
		{
			return ShowProgressBase(
				() => {
					return new FormProgressStatus(odEventType,typeEvent,hasHistory,hasMinimize,startingMessage,progStyle) {
						TopMost=true,//Make this window show on top of ALL other windows.
					};
				},"Thread_ODProgress_Show_"+DateTime.Now.Ticks);
		}

		///<summary>Non-blocking call. FormProgressExtended is an extremely tailored version of FormProgressStatus.
		///It is a progress window that can have multiple progress bars showing at the same time with pause and cancel functionality.
		///This "extended" progress window is exactly like the "Show()" progress window in that the close action that is returned must be invoked
		///by the calling method in order to programmatically close.</summary>
		///<param name="odEventType">Causes the progress window to only process ODEvents of this ODEventType.  Undefined will process all types.</param>
		///<param name="typeEvent">Causes the progress window to only process ODEvents of this Type.  Null defaults to typeof(ODEvent).</param>
		///<param name="currentForm">The form to activate once the progress is done. If you cannot possibly pass in a form, it is okay to pass in null.</param>
		///<param name="tag">Optionally set tag to an object that should be sent as the first "event arg" to the new progress window.
		///This will typically be a ProgressBarHelper or a string.</param>
		///<param name="progCanceled">Optionally pass in a delegate that will get invoked when the user clicks Cancel.</param>
		///<param name="progPaused">Optionally pass in a delegate that will get invoked when the user clicks Pause.</param>
		///<returns>An action that will close the progress window.  Invoke this action whenever long computations are finished.</returns>
		public static Action ShowExtended(ODEventType odEventType,Type typeEvent,Form currentForm,object tag=null,
			ProgressCanceledHandler progCanceled=null,ProgressPausedHandler progPaused=null,string cancelButtonText=null)
		{
			Action actionCloseProgressWindow=ShowProgressBase(
				() => {
					FormProgressExtended FormPE=new FormProgressExtended(odEventType,typeEvent,cancelButtonText:cancelButtonText);
					if(progCanceled!=null) {
						FormPE.ProgressCanceled+=progCanceled;
					}
					if(progPaused!=null) {
						FormPE.ProgressPaused+=progPaused;
					}
					//FormProgressExtended should NOT be the top most form.  Other windows might be popping up requiring attention from the user.
					//FormPE.TopMost=true;//Make this window show on top of ALL other windows.
					if(tag!=null) {
						FormPE.ODEvent_Fired(new ODEventArgs(odEventType,tag));
					}
					return FormPE;
				},"Thread_ODProgress_ShowExtended_"+DateTime.Now.Ticks);
			return () => {
				actionCloseProgressWindow();//Make sure the progress window is closed first.
				//If a form was passed in, activate it so that it blinks or gets focus.
				if(currentForm!=null && !currentForm.IsDisposed) {
					currentForm.Activate();
				}
			};
		}

		///<summary>This is a blocking call. Runs the action on the main thread and displays a progress window on another thread.
		///Once the long computation has completed, then the progress window will automatically be closed.
		///Throws any exceptions that occurred within the action if no exception delegate was provided.</summary>
		///<param name="actionComputation">Any long computation.  Returns and does nothing if null.</param>
		///<param name="startingMessage">Sets the label of the progress window to the value passed in.  Defaults to "Please Wait..."
		///It is always up to the calling method to translate this message before passing it in.  This is to avoid translating multiple times.</param>
		///<param name="actionException">A custom UE handler for the worker thread.  Null will cause this method to rethrow any exceptions.</param>
		///<param name="typeEvent">Causes the progress window to only process ODEvents of this Type.  Null defaults to typeof(ODEvent).</param>
		///<param name="odEventType">Causes the progress window to only process ODEvents of this ODEventType.  Undefined will process all types.</param>
		///<param name="progStyle">Sets the style of the progress bar within the progress window that is going to be shown to the user.</param>
		///<param name="hasMinimize">Set to true if the progress window should allow minimizing.  False by default.</param>
		///<param name="hasHistory">Set to true if the progress window should show a history of events that it has processed.
		///This will cause the UI of the progress window to change slightly and will also make it so the user has to click a Close button.</param>
		[Obsolete("Deprecated.  Use ProgressOD instead.")]
		public static void ShowAction(Action actionComputation,string startingMessage="Please Wait...",
			Action<Exception> actionException=null,Type typeEvent=null,ODEventType odEventType=ODEventType.Undefined,
			ProgressBarStyle progStyle=ProgressBarStyle.Marquee,bool hasMinimize=true,bool hasHistory=false)
		{
			if(actionComputation==null) {
				return;//No work to be done.  Simply return.
			}
			Action actionCloseProgressWindow=Show(odEventType,typeEvent,startingMessage,hasHistory,hasMinimize,progStyle);
			//Invoke the action on the main thread that was passed in.
			try {
				actionComputation();
			}
			catch(Exception e) {
				if(actionException==null) {
					throw;
				}
				else {
					actionException(e);
				}
			}
			finally {
				actionCloseProgressWindow();
			}
		}

		///<summary>Non-blocking call. Every type of progress window should eventually call this method which does the hard stuff for the calling method.
		///Spawns a separate thread that will instantiate a FormProgressBase within the new thread's context by invoking the func passed in.
		///The FormProgressBase that funcGetNewProgress returns should be a form that is instatiated within the func (in order to avoid cross-threading).
		///The global static FormProgressCurS will get set to the newly instantiated progress so that the entire application knows progress is showing.
		///Finally returning a close action for the calling method to invoke whenever long computations are finished.
		///Two critical portions of the closing method are 1 - it closes progress gracefully and 2 - FormProgressCurS gets set to null.</summary>
		public static Action ShowProgressBase(Func<FormProgressBase> funcGetNewProgress,string threadName="Thread_ODProgress_ShowProgressBase") {
			if(ODEnvironment.IsWindows7(false) || ODInitialize.IsRunningInUnitTest) {
				return new Action(() => {
					//Do nothing.
				});
			}
			FormProgressBase FormPB=null;
			ManualResetEvent manualReset=new ManualResetEvent(false);
			ODThread odThread=new ODThread(o => {
				//It is very important that this particular thread instantiates the form and not the calling method.
				//This is what allows the progress window to show and be interacted with without joining or invoking back to the parent thread.
				FormPB=funcGetNewProgress();
				AddActiveProgressWindow(FormPB);//Let the entire application know that a progress window is showing.
				FormPB.Shown+=(obj,eArg) => manualReset.Set();
				FormPB.FormClosed+=(obj,eArg) => RemoveActiveProgressWindow(FormPB);
				FormPB.ShowDialog();//We cannot utilize the "owner" overload because that would cause a cross threaded exception.
			});
			odThread.SetApartmentState(ApartmentState.STA);//This is required for ReportComplex due to the history UI elements.
			odThread.AddExceptionHandler(e => e.DoNothing());//The progress window had an exception... Not worth crashing the program over this.
			odThread.Name=threadName;
			odThread.Start();
			//Force the calling thread to wait for the progress window to actually show to the user before continuing.
			manualReset.WaitOne();
			return () => {
				FormPB.ForceClose=true;
				odThread.Join(Timeout.Infinite);
			};
		}

	}
}
