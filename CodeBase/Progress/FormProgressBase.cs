using System;
using System.Windows.Forms;
using System.Reflection;

namespace CodeBase {
	///<summary>A base window designed to run in a separate thread so that the progress bar can smoothly spin without waiting on the main thread.
	///Takes care of registering and unregistering for the ODEvent passed into the constructor.
	///Also takes care of making sure that this window does not get "stuck" open by spawning a thread that monitors if CloseGracefully has been called.
	///Extending classes are supposed to take care of the desired UI.  Does not extend ODForm on purpose.</summary>
	public class FormProgressBase : Form {
		///<summary>An indicator owned by Open Dental indicating that this progress window needs to close regardless if it is done computing or not.
		///Set to true by the entity that instantiated this progress form to gracefully close when the long computation has finished.</summary>
		public bool ForceClose;
		///<summary>An indicator if this form has closed itself and no longer needs to monitor the ForceClose boolean.</summary>
		private bool _hasClosed;

		public FormProgressBase() {
			this.FormClosing+=new FormClosingEventHandler(this.FormProgressStatus_FormClosing);
			this.FormClosed+=new FormClosedEventHandler(this.FormProgressStatus_FormClosed);
			this.Shown+=new EventHandler(this.FormProgressStatus_Shown);
		}

		private void FormProgressStatus_Shown(object sender,EventArgs e) {
			ODEvent.Fired+=ODEvent_Fired;
			//Spawn a separate thread that will monitor if this progress form has indicated that it needs to close.
			//This thread will be a fail-safe in the sense that it will constantly monitor a separate indicator that this window should close.
			ODThread threadForceCloseMonitor=new ODThread(100,new ODThread.WorkerDelegate((o) => {
				if(_hasClosed) {
					o.QuitAsync();//Stop monitoring as soon as we detect that this window has been "closed".
					return;
				}
				if(ForceClose) {
					//Something triggered the fact that this form should have closed.
					this.InvokeIfRequired(() => { 
						DialogResult=DialogResult.OK;
						ODException.SwallowAnyException(() => Close());//Can throw exceptions for many reasons.
					});
					o.QuitAsync();//We tried our best to "unstuck" this window.  The user will have to Alt + F4 this window closed?
					return;
				}
			}));
			threadForceCloseMonitor.AddExceptionHandler((ex) => {
				ex.DoNothing();//The form might stay open forever which was already happening... we tried our best.
			});
			threadForceCloseMonitor.Name="FormProgressStatusMonitor_"+DateTime.Now.Ticks;
			threadForceCloseMonitor.Start();
		}

		///<summary>Shows a message to the user with this progress window as the owner.</summary>
		public DialogResult MsgBoxShow(string text,string caption="") {
			return MessageBox.Show(this,text,caption);
		}

		///<summary>Shows a message to the user with this progress window as the owner.</summary>
		public DialogResult MsgBoxShow(string text,string caption,MessageBoxButtons buttons) {
			return MessageBox.Show(this,text,caption,buttons);
		}

		///<summary>Shows a message to the user with this progress window as the owner.</summary>
		public DialogResult MsgBoxShow(string text,string caption,MessageBoxButtons buttons,MessageBoxIcon icon) {
			return MessageBox.Show(this,text,caption,buttons,icon);
		}

		public void ODEvent_Fired(ODEventArgs e) {
			try {
				//We don't know what thread will cause a progress status change, so invoke this method as a delegate if necessary.
				if(this.InvokeRequired) {
					this.Invoke((Action)delegate() { ODEvent_Fired(e); });
					return;
				}
				//If Tag on the ODEvent is null then there is nothing to for this event handler to do.
				if(e.Tag==null) {
					return;
				}
				ProgressBarHelper progHelper=new ProgressBarHelper("");
				bool hasProgHelper=false;
				string status="";
				if(e.Tag.GetType()==typeof(string)) {
					status=((string)e.Tag);
				}
				else if(e.Tag.GetType()==typeof(ProgressBarHelper)) {
					progHelper=(ProgressBarHelper)e.Tag;
					status=progHelper.LabelValue;
					hasProgHelper=true;
				}
				else {//Unsupported type passed in.
					return;
				}
				UpdateProgress(status,progHelper,hasProgHelper);
			}
			catch(Exception ex) {
				ex.DoNothing();//It's just progress...
			}
		}

		///<summary>Extending classes are required to implement this method.
		///This class was originally an abstract class which made this fact apparent but Visual Studio's designer doesn't play nicely.</summary>
		public virtual void UpdateProgress(string status,ProgressBarHelper progHelper,bool hasProgHelper) {
			throw new NotImplementedException();//STOP!  It is up to extending methods to implement this.  Go away.
		}

		private void FormProgressStatus_FormClosing(object sender,FormClosingEventArgs e) {

		}

		private void FormProgressStatus_FormClosed(object sender,FormClosedEventArgs e) {
			_hasClosed=true;
			ODEvent.Fired-=ODEvent_Fired;
		}

	}
}