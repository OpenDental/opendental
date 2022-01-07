using System;
using System.Windows.Forms;

namespace CodeBase {
	///<summary>Purposefully overrides or hides System.Windows.Forms.MessageBox from any extending namespace.
	///This is so that we can inject our own code prior to System.Windows.Forms.MessageBox.Show().
	///This is necessary when a separate thread has UI and the owner thread needs to show a message box.
	///The idea behind this class is for any project to create its own MessageBox class that simply extends this one.
	///This will successfully hide System.Windows.Forms.MessageBox for the entire namespace of the extending class.
	///E.g. see OpenDental.MessageBox for more details.
	///Side note: Visual Studio may suggest simplifying MessageBox.Show() to ODMessageBox.Show().  Do not do this.  Either ignore the suggestion
	///or go remove the suggestion entirely in the Visual Studio settings.  To remove this suggestion from the OpenDental project...
	///Expand the References node in the Solution Explorer of the desired project > right click on Analyzers > Open Active Rule Set > 
	///Once the ruleset editor is open, search for IDE0002 and uncheck the check box > Save (a warning will show about making a new file).</summary>
	public class ODMessageBox {

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(string text) {
			return ShowHelper((formPB) => formPB.MsgBoxShow(text),() => System.Windows.Forms.MessageBox.Show(text));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(string text,string caption) {
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption),() => System.Windows.Forms.MessageBox.Show(text,caption));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner,string text) {
			return ShowHelper((formPB) => formPB.MsgBoxShow(text),() => System.Windows.Forms.MessageBox.Show(owner,text));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner,string text,string caption) {
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption),() => System.Windows.Forms.MessageBox.Show(owner,text,caption));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(string text,string caption,System.Windows.Forms.MessageBoxButtons buttons) {
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption,buttons),() => System.Windows.Forms.MessageBox.Show(text,caption,buttons));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner,string text,string caption,
			System.Windows.Forms.MessageBoxButtons buttons)
		{
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption,buttons),() => System.Windows.Forms.MessageBox.Show(owner,text,caption,buttons));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(string text,string caption,System.Windows.Forms.MessageBoxButtons buttons,
			System.Windows.Forms.MessageBoxIcon icon)
		{
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption,buttons,icon),() => System.Windows.Forms.MessageBox.Show(text,caption,buttons,icon));
		}

		///<summary>Shows a message to the user.  Automatically checks to see if a progress window is showing and will ask the progress window
		///to show the message to the user so that the progress window doesn't cover up the question.</summary>
		public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner,string text,string caption,
			System.Windows.Forms.MessageBoxButtons buttons,System.Windows.Forms.MessageBoxIcon icon)
		{
			return ShowHelper((formPB) => formPB.MsgBoxShow(text,caption,buttons,icon),() => System.Windows.Forms.MessageBox.Show(owner,text,caption,buttons,icon));
		}

		///<summary>Invokes one of the funcs passed in based on if there are any active progress windows showing and has focus.
		///Will invoke funcShowProgress if a progress window is active.  Otherwise; invokes funcShow.
		///Recursively calls itself as needed if the active progress window was in the middle of closing when this method was invoked.</summary>
		///<param name="funcShowOverProgress">The func that should execute if a progress window is currently showing to the user.</param>
		///<param name="funcShow">The func that should execute if no progress window is currently showing to the user.</param>
		///<returns>The dialog result from the func that ended up getting invoked.</returns>
		private static System.Windows.Forms.DialogResult ShowHelper(Func<FormProgressBase,System.Windows.Forms.DialogResult> funcShowOverProgress,
			Func<System.Windows.Forms.DialogResult> funcShow)
		{
			//Unit tests are not designed to display message boxes.
			//Throw an exception instead of displaying the message so that unit tests cannot get locked up.
			if(ODInitialize.IsRunningInUnitTest) {
				throw new ApplicationException("Message boxes are not allowed for unit tests.");
			}
			//Get the active form for the current application.  This property will return null if another application has focus (not our application).
			//This is rare enough that it is acceptable to default the parent of the message box to the progress window (if one is present).
			//This may cause the MessageBox to show up behind a form that HAD focus with a progress window behind it (e.g. Registration Key Edit window).
			Form FormActive=Form.ActiveForm;
			//Get the "active" progress window if one is present.  Utilize a shallow copy so race conditions don't affect us inadvertently.
			FormProgressBase FormPB=ODProgress.FormProgressActive;
			//So that the logic is easier to follow, check for the two scenarios that can cause an immediate kick out.
			if(FormPB==null                               //The easiest scenario is when there is no active progress window.
				|| (FormActive!=null && FormActive!=FormPB))//There is a progress window but there is another form owned by the application that has focus.
			{
				//There is no progress window showing or there is one showing but we know that a different window of our application has focus.
				return funcShow();//Show the message box like normal and don't override its Parent property with the progress window.
			}
			//There is a progress window present and it could be the active form for the application or another application has focus and we don't know.
			//It is rare enough for applications to leave progress windows open while showing dialogs or new forms to the user.
			//Default to forcing the active progress window to be the parent form of the new message box because that scenario is so rare.
			System.Windows.Forms.DialogResult dialogResult=System.Windows.Forms.DialogResult.Abort;
			try {
				FormPB.InvokeIfRequired(() => dialogResult=funcShowOverProgress(FormPB));
			}
			catch(ObjectDisposedException ode) {
				//Explicitly catch object disposed exceptions due to rare race conditions.
				//The active progress window that was just showing could have been placed on the "invoke stack" for close and disposal prior to our invoke.
				//The active progress window would successfully close and dispose first and then FormPB would be a reference to a disposed window
				//which would cause .InvokeIfRequired() to throw an ObjectDisposedException.
				//No error should be thrown in this scenario and instead we should retry this method because the active progress window could be different.
				//E.g. there will be a different active progress window if multiple progress windows were showing at the same time
				//OR we we eventually get back to the main thread which will not require invoking over to a progress window at all.
				ode.DoNothing();
				dialogResult=ShowHelper(funcShowOverProgress,funcShow);//Recursive call on purpose.
			}
			return dialogResult;
		}

	}
}
