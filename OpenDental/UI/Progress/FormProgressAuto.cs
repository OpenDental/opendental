using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI.Progress{
	public partial class FormProgressAuto : FormODBase{
		public Action ActionMain;
		//public Action<Exception> ActionException;//handled in calling code
		public Exception ExceptionGenerated;
		public bool HasHistory;
		public bool HistoryClose;
		public string HistoryMsg;
		public ODEventType ODEventTypeMy;
		public ProgressBarStyle ProgStyle;
		public bool ShowCancelButton;
		public string StartingMessage;
		public string StopNotAllowedMessage;
		public string MessageCancel;
		public bool TestSleep;
		public Type TypeEvent;

		///<summary>The date and time of the most recent event that this form processed. Used for history.</summary>
		private DateTime _dateTimeLastEvent;
		private EventInfo _eventInfoFired;
		private Thread _threadWorker;

		///<summary>Do not use this form.  Only for ProgressOD.</summary>
		public FormProgressAuto(){
			InitializeComponent();
			InitializeLayoutManager();
			//base.TopMost=true;//too agressive.  Not needed.  Obscures debugging.
		}

		private void FormProgressAuto_Load(object sender, EventArgs e){
			labelMsg.Text=StartingMessage;//this will never be null because of how it's passed in from ProgressOD.
			if(!ShowCancelButton){
				butCancel.Visible=false;
			}
			if(HasHistory){
				//labelMsg.Visible=false;//this will happen with first event
				_dateTimeLastEvent=DateTime.MinValue;
				if(!HistoryMsg.IsNullOrEmpty() && !HistoryMsg.EndsWith("\r\n")){
					textHistoryMsg.Text=HistoryMsg+"\r\n";
				}
			}
			else{
				Size=new Size(LayoutManager.Scale(518),LayoutManager.Scale(208));
				textHistoryMsg.Visible=false;
				butCopy.Visible=false;
			}
			progressBar.Style=ProgStyle;
			_threadWorker=new Thread(new ThreadStart(DoWork));
			_threadWorker.IsBackground=true;//shuts down automatically when program shuts down
			if(TypeEvent!=null){
				//Registers this form for any progress status updates that happen throughout the entire program.
				ODException.SwallowAnyException(() => _eventInfoFired=TypeEvent.GetEvent("Fired"));
				if(_eventInfoFired==null) {
					throw new ApplicationException("The 'eventType' passed into FormProgressStatus does not have a 'Fired' event.\r\n"
						+"Type passed in: "+TypeEvent.GetType());
				}
				//Register the "Fired" event to the ODEvent_Fired delegate. 
				Delegate delegateFired=Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,"ODEvent_Fired");
				MethodInfo methodAddHandler=_eventInfoFired.GetAddMethod();
				methodAddHandler.Invoke(this,new object[] { delegateFired });
			}
			_threadWorker.Start();
		}

		private void DoWork(){	
			if(ODBuild.IsDebug()){
				if(TestSleep){
					Thread.Sleep(3000);
				}
			}
			try {
				ActionMain();
			}
			catch(ThreadAbortException){//this gets thrown when we forcefully close the thread.
				this.InvokeIfNotDisposed(() => { 
					AbortAndUnregister();
					DialogResult=DialogResult.Cancel;
				});
				return;
			}
			catch(Exception ex) {//normal exception
				ExceptionGenerated=ex;
				this.InvokeIfNotDisposed(() => { 
					AbortAndUnregister();
					DialogResult=DialogResult.No;//we use this to indicate that there was an exception
				});
				return;
			}
			if(HasHistory && !HistoryClose){
				this.InvokeIfNotDisposed(() => { 
					butCancel.Text=Lan.g(this,"Close");
					labelMsg.Text=Lan.g(this,"Done");
					labelMsg.Visible=true;
					progressBar.Visible=false;
				});
				//do not close form
			}
			else{
				this.InvokeIfNotDisposed(() => { 
					AbortAndUnregister();
					DialogResult=DialogResult.OK;
				});
			}
		}

		public void ODEvent_Fired(ODEventArgs e) {
			//We don't know what thread will cause a progress status change, so invoke this method as a delegate if necessary.
			if(this.InvokeRequired) {
				this.Invoke((Action)delegate() { ODEvent_Fired(e); });
				return;
			}
			//If Tag on the ODEvent is null then there is nothing to for this event handler to do.
			if(e.Tag==null) {
				return;
			}
			if(ODEventTypeMy!=ODEventType.Undefined) {
				if(ODEventTypeMy!=e.EventType) {
					return;
				}
			}
			ProgressBarHelper progHelper=new ProgressBarHelper("");
			bool hasProgHelper=false;
			string status="";
			if(e.Tag is string) {
				status=(string)e.Tag;
			}
			else if(e.Tag is ProgressBarHelper) {
				progHelper=(ProgressBarHelper)e.Tag;
				status=progHelper.LabelValue;
				hasProgHelper=true;
			}
			else {//Unsupported type passed in.
				return;
			}
			try {
				UpdateProgress(status,progHelper,hasProgHelper);
			}
			catch(Exception ex) {
				ex.DoNothing();//It's just progress...
			}
		}

		public void UpdateProgress(string status,ProgressBarHelper progHelper,bool hasProgHelper) {
			if(HasHistory && !progressBar.Visible) {
				//Once the progress bar is hidden when history is showing, we never want to process another event.
				return;
			}
			if(HasHistory){
				labelMsg.Visible=false;//was visible until first event
			}
			else{
				labelMsg.Text=status;
			}
			if(hasProgHelper) {
				if(progHelper.BlockMax!=0) {
					progressBar.Maximum=progHelper.BlockMax;
				}
				if(progHelper.BlockValue!=0 && progHelper.BlockValue>=progressBar.Minimum && progHelper.BlockValue<=progressBar.Maximum) {
					progressBar.Value=progHelper.BlockValue;
				}
				if(progHelper.ProgressStyle==ProgBarStyle.Marquee) {
					progressBar.Style=ProgressBarStyle.Marquee;
				}
				else if(progHelper.ProgressStyle==ProgBarStyle.Blocks) {
					progressBar.Style=ProgressBarStyle.Blocks;
				}
				else if(progHelper.ProgressStyle==ProgBarStyle.Continuous) {
					progressBar.Style=ProgressBarStyle.Continuous;
				}
			}
			if(HasHistory) {
				if(_dateTimeLastEvent==DateTime.MinValue) {
					textHistoryMsg.AppendText(status.PadRight(60));//left aligned
					_dateTimeLastEvent=DateTime.Now;
				}
				else {
					textHistoryMsg.AppendText(GetElapsedTime(_dateTimeLastEvent,DateTime.Now)+"\r\n"+status.PadRight(60));
					_dateTimeLastEvent=DateTime.Now;
				}
				HistoryMsg=textHistoryMsg.Text;
			}
		}

		///<summary>Gets a user friendly elapsed time message to display in the history text box.</summary>
		private string GetElapsedTime(DateTime start, DateTime end) {
			TimeSpan timeElapsed=new TimeSpan(end.Ticks-start.Ticks);
			if(timeElapsed.TotalMinutes>2) {
				return "Elapsed Time: "+timeElapsed.TotalMinutes.ToString("F1")+" minutes";
			}
			else if(timeElapsed.TotalSeconds>2) {
				return "Elapsed Time: "+timeElapsed.TotalSeconds.ToString("F0")+" seconds";
			}
			else {
				return "Elapsed Time: "+timeElapsed.TotalMilliseconds.ToString("F0")+" milliseconds";
			}
		}

		///<summary>Aborts _threadWorker immediately if it has not already finished. Also unregisters "Fired" event from this form.  Any place in the code that call this method should also close this form by providing a DialogResult.</summary>
		private void AbortAndUnregister(){
			//Once we get this far we want the thread to exit immediately. No sense in setting ODThread.HasQuit and waiting, also no sense in calling ODThread.QuitAsync().
			//Both of these methods would only have value if the instance of _threadWorker exposed its HasQuit flag here, which it does not.
			//In this case _threadWorker.HasQuit is not available for reading to the owner of the thread delegate method (since _threadWorker is private).
			//For all these reasons, by the time we get here, just Join/Abort the thread, which is exactly what QuitSync(1) will accomplish.
			_threadWorker.Abort();
			//unregister the "Fired" event from this form.  The constructor will throw an exception if _eventInfoFired is not valid.
			if(_eventInfoFired!=null){
				Delegate fireDelegate=Delegate.CreateDelegate(_eventInfoFired.EventHandlerType,this,"ODEvent_Fired");
				MethodInfo methodRemoveHandler=_eventInfoFired.GetRemoveMethod();
				methodRemoveHandler.Invoke(this,new object[] { fireDelegate });
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(textHistoryMsg.Text);
				MessageBox.Show("Copied");
			}
			catch(Exception) {
				MessageBox.Show("Could not copy contents to the clipboard.  Please try again.");
			}
		}

		private void butCancel_Click(object sender, EventArgs e){
			if(!MessageCancel.IsNullOrEmpty()) {
				if(MessageBox.Show(MessageCancel,"",MessageBoxButtons.YesNo)==DialogResult.No) {
					return;
				}
			}
			if(_threadWorker.IsAlive){ 
				AbortAndUnregister();
				DialogResult=DialogResult.Cancel;
			}
			else{
				//must be HasHistory.  Button says "Close" here.
				AbortAndUnregister();
				DialogResult=DialogResult.OK;
			}
		}

		private void FormProgressAuto_CloseXClicked(object sender, CancelEventArgs e){
			if(!ShowCancelButton){
				MessageBox.Show(StopNotAllowedMessage);//Message is only used in three places.
				e.Cancel=true;
				return;
			}
			if(_threadWorker.IsAlive){ 
				AbortAndUnregister();
				DialogResult=DialogResult.Cancel;
			}
			else{
				//must be HasHistory.  Button says "Close" here.
				AbortAndUnregister();
				DialogResult=DialogResult.OK;
			}
		}

		
	}
}
