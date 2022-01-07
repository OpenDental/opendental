using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.UI{
	//Jordan is the only one allowed to edit this file.
	//Jordan personally reviews all progress bars that get added.

	///<summary>Replacement for ODProgress.  Create an instance, set ActionMain, and ShowDialogProgress.  Shows a progress bar dialog in the main thread, and runs the action in a background thread.  Once the long action has completed, then the progress window will automatically close.  User can Cancel/Abort the thread at any time.  Sample boilerplate is at bottom of this file.</summary>
	public class ProgressOD{
		///<summary>This action can include computations, db calls, etc.  It can read from UI elements, but should not write to them because it's on a different thread.</summary>
		public Action ActionMain=null;
		///<summary>Indicates that the progress form is in "history" mode which will show a text box with all messages it takes action on and then will stay open, forcing the user to click Close.</summary>
		public bool HasHistory=false;
		///<summary>Used with HasHistory and HistoryMsg.  Set to true to close a progress even though it does have history.</summary>
		public bool HistoryClose=false;
		///<summary>Used to chain progress windows together with a common history log.  Use with HasHistory and HistoryClose.  Set this to prepopluate the log and also to get the generated log.</summary>
		public string HistoryMsg="";
		///<summary>Indicates user clicked cancel. Very common to test this.  Cancel button always shows, so always account for cancel even if you don't explicitly test this.</summary>
		public bool IsCancelled;
		///<summary>Very rarely used.  Normally use IsCancelled instead.  True if the action was successful.  False if there was an exception or the user clicked Cancel.  So you would only used this if you also had a try/catch.  This would be used at the end, after the try/catch, and usually after testing for IsCancelled.</summary>
		public bool IsSuccess;
		///<summary>Events can be fired from within your action.  Almost always ProgressBar, but others are allowed.</summary>
		public ODEventType ODEventType=ODEventType.ProgressBar;
		///<summary>The only two options are Marquee and Blocks.  Continuous is an older option not supported by Window when visual styles are not enabled.</summary>
		public ProgressBarStyle ProgStyle=ProgressBarStyle.Marquee;
		///<summary>Always true except in four places: db update and terminal transaction processing via PayConnect and EServicesSetup validation progress (approved by Jordan).  Do not set to false anywhere else.</summary>
		public bool ShowCancelButton=true;
		public string StartingMessage="Please Wait...";
		public string StopNotAllowedMessage="Not allowed to stop because it would corrupt the database.";
		///<summary>Set this string to show a warning message when the user attempts to cancel. Do necessary translations before setting this. It should follow a format similar to this: "You should not Cancel because ... Cancel anyway?" They will get a yes/no msgbox.</summary>
		public string MessageCancel;
		///<summary>If this is set to true, a 3 second sleep will be added to the Action, giving you time to click Cancel during testing.  In case you forget to remove it when you're done, it only works in Debug.  It's totally harmless to leave it in place permanently.</summary>
		public bool TestSleep;
		///<summary>The specific type of ODEvent that should be fired / monitored.  Almost always just ProgressBarEvent, but others are allowed.</summary>
		public Type TypeEvent=typeof(ProgressBarEvent);

		public ProgressOD(){
			//no parameters
		}

		///<summary>This can be surrounded with a try/catch.</summary>
		public void ShowDialogProgress(){
			if(ActionMain==null) {
				return;
			}
			using Progress.FormProgressAuto formProgressAuto=new Progress.FormProgressAuto();
			formProgressAuto.ActionMain=ActionMain;
			formProgressAuto.HasHistory=HasHistory;
			formProgressAuto.HistoryClose=HistoryClose;
			formProgressAuto.HistoryMsg=HistoryMsg;
			formProgressAuto.ODEventTypeMy=ODEventType;
			formProgressAuto.ProgStyle=ProgStyle;
			formProgressAuto.ShowCancelButton=ShowCancelButton;
			formProgressAuto.StartingMessage=StartingMessage;
			formProgressAuto.StopNotAllowedMessage=StopNotAllowedMessage;
			formProgressAuto.MessageCancel=MessageCancel;
			formProgressAuto.TestSleep=TestSleep;
			formProgressAuto.TypeEvent=TypeEvent;
			formProgressAuto.ShowDialog();
			if(formProgressAuto.DialogResult==DialogResult.OK){
				IsSuccess=true;
			}
			if(formProgressAuto.DialogResult==DialogResult.Cancel){
				IsCancelled=true;
			}
			//third possibility is No, indicating error.  In that case, both of the above will remain false.
			HistoryMsg=formProgressAuto.HistoryMsg;
			if(formProgressAuto.ExceptionGenerated!=null){
				if(formProgressAuto.ExceptionGenerated.InnerException!=null){
					ExceptionDispatchInfo exceptionDispatchInfo=
						ExceptionDispatchInfo.Capture(formProgressAuto.ExceptionGenerated.InnerException);//this can happen when methods are invoked by reflection
					exceptionDispatchInfo.Throw();
				}
				ExceptionDispatchInfo exceptionDispatchInfo2=ExceptionDispatchInfo.Capture(formProgressAuto.ExceptionGenerated);
				exceptionDispatchInfo2.Throw();
			}
		}



	}
}

/*
Example, Simple======================================================================================================================
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => ;
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}

Example, Complex=====================================================================================================================
(Obviously, you wouldn't normally use all of these.  Pick and choose.)
ProgressOD progressOD=new ProgressOD();
progressOD.ActionMain=DoThings;
progressOD.ProgStyle=ProgressBarStyle.Blocks;//marquee is more popular because it requires less programming effort.
progressOD.StartingMessage=Lan.g(this,"Please wait while we do things.");
progressOD.TypeEvent=typeof(SomeEvent);
progressOD.ODEventTypeMy=ODEventType.SomeType;
try{
	progressOD.ShowDialogProgress();
}
catch(Exception ex){
	MyExceptionHandler(ex,this);
	//usually, it's best to just return here instead of checking IsSuccess further down
}
if(progressOD.IsCancelled){
	return;//or similar to gracefully handle when they click Cancel
}
//do any cleanup that should be done regardless of an exception
return progressOD.IsSuccess;//If the calling class wants to know if success, for example.

Example Chained Series with common log=======================================================================================================
//this pattern will probably not be used much, but it could come in handy.
ProgressOD progressOD=new ProgressOD();
progressOD.HasHistory=true;
progressOD.HistoryClose=true;
progressOD.ActionMain=() => { 
	ProgressBarEvent.Fire(ODEventType.ProgressBar,"Doing first thing");
	//do something
};
progressOD.ShowDialogProgress();
if(progressOD.IsCancelled){
	return;
}
string progressMsg=progressOD.HistoryMsg;
//some intermittent code here, which is why we didn't just have one progress bar in the first place.
progressOD=new ProgressOD();
progressOD.HasHistory=true;
progressOD.HistoryMsg=progressMsg;
progressOD.ActionMain=() => { 
	ProgressBarEvent.Fire(ODEventType.ProgressBar,"Doing second thing");
	//do something
};
//dlg will not close here because HistoryClose is not set true
progressOD.ShowDialogProgress();
if(progressOD.IsCancelled){
	return;
}	

Events==============================================================================================================================
Inside of your background thread, you can fire global events:
MiscDataEvent.Fire(ODEventType.MiscData,"Done with step "+i.ToString());
or
ProgressBarHelper progressBarHelper=new ProgressBarHelper("Done with step "+i.ToString(),blockValue:i,blockMax:100);
ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);

General Guidelines======================================================================================================
There are two main considerations: 
1. Handle Cancel (described in examples above). This is a new feature that the old progress bar did not have.
2. Shrink the scope (discussed below).
The old progress bar is probably including far too much code within the action. This is a bad pattern.  
The first step is usually to identify the smallest and slowest section of code, usually one line.
This very small section of code should not include any UI.
If it makes a call to a method, you must examine all of the code in that method to ensure it contains no UI.
Once you've identified the line that needs to be wrapped, you can proceed to code for the new progress bar and remove the old one.
If UI code is intermingled with code that requires a progress bar, then you may end up with a few more progress bars.
They don't need to be chained together, as in the example above.  They can simply be a series of individual progress bars.
A series of progress bars is not at all annoying, as long as they each automatically close on their own.
Use wait cursors for everything else:
-if the wait is generally under 5 seconds
-You want to completely lock the user out (of a short action) because the action is impossible to clean up if cancelled.
-UI manipulation and dialogs.


 
*/