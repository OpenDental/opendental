using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenDental;
using OpenDentBusiness;
using CodeBase;

namespace WpfControls.UI {
	/// <summary>This Frm is only used by ProgressOD. Do not call it directly.</summary>
	public partial class FrmProgressAuto:FrmODBase {
		public Action ActionMain;
		public Exception ExceptionGenerated;
		public bool HasHistory;
		public bool HistoryClose;
		public string HistoryMsg;
		///<summary>Default is a continuous indeterminate marquee appearance.  Setting IsBlocks to true allows you to manually control the progress out of 100%.</summary>
		public bool IsBlocks=false;
		public bool ShowCancelButton;
		public string StartingMessage;
		public string StopNotAllowedMessage;
		public string MessageCancel;
		public bool TestSleep;
		public Action CancelAction { get; set; }

		///<summary>The date and time of the most recent event that this form processed. Used for history.</summary>
		private DateTime _dateTimeLastEvent;
		private EventInfo _eventInfoFired;
		private Thread _threadWorker;
		///<summary>If there's some problem with the action and an exception is thrown, this gets set true.</summary>
		private bool _isForceClose=false;

		public FrmProgressAuto() {
			InitializeComponent();
			Load+=FrmProgressAuto_Load;
			FormClosing+=FormProgressAuto_FormClosing;
		}

		private void FrmProgressAuto_Load(object sender,EventArgs e) {
			ODEvent.Fired+=ODEvent_Fired;
			labelError.Visible=false;
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
				//597x406 client area
			}
			else{
				//540x148 client area = 556x182 form size
				_formFrame.Width=ScaleFormValue(556);
				_formFrame.Height=ScaleFormValue(182);
				textHistoryMsg.Visible=false;
				butCopy.Visible=false;
			}
			if(IsBlocks){
				progressBar.IsIndeterminate=false;
			}
			else{
				progressBar.IsIndeterminate=true;
			}
			_threadWorker=new Thread(new ThreadStart(DoWork));
			_threadWorker.IsBackground=true;//shuts down automatically when program shuts down
			_threadWorker.Start();
		}

		private void ODEvent_Fired(ODEventArgs eventArgs) {
			//There seem to be a variety of types used that were clearly intended to be shown in progress bar, so we'll include all.
			//Besides, the msgs only hit this spot if a progress window is open, so it's probably relevant.
			if(eventArgs.Tag is string str){
				ProgressBarHelper progressBarHelper2=new ProgressBarHelper(str);
				try {
					Dispatcher.Invoke(delegate() { UpdateProgress(str,progressBarHelper2); });
				}
				catch(Exception ex) {
					ex.DoNothing();//It's just progress...
				}
			}
			if(eventArgs.Tag is ProgressBarHelper progressBarHelper){
				try {
					Dispatcher.Invoke(delegate() { UpdateProgress(progressBarHelper.LabelValue,progressBarHelper); });
				}
				catch(Exception ex) {
					ex.DoNothing();//It's just progress...
				}
			}
		}

		///<summary>This happens on background thread.</summary>
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
				Dispatcher.Invoke(() => { 
					_isForceClose=true;
					IsDialogOK=false;
				});
				return;
			}
			catch(Exception ex) {//normal exception
				ExceptionGenerated=ex;
				Dispatcher.Invoke(() => { 
					_threadWorker.Abort();
					_isForceClose=true;
					IsDialogOK=false;
				});
				return;
			}
			if(HasHistory && !HistoryClose){
				Dispatcher.Invoke(() => { 
					butCancel.Text=Lans.g(this,"Close");
					labelMsg.Text=Lans.g(this,"Done");
					labelMsg.Visible=true;
					progressBar.Visibility=Visibility.Collapsed;
				});
				//do not close form
			}
			else{
				Dispatcher.Invoke(() => { 
					_threadWorker.Abort();
					IsDialogOK=true;
				});
			}
		}

		private void UpdateProgress(string status,ProgressBarHelper progressBarHelper) {
			if(HasHistory && progressBar.Visibility==Visibility.Collapsed) {
				//Once the progress bar is hidden when history is showing, we never want to process another event.
				return;
			}
			if(HasHistory){
				labelMsg.Visible=false;//was visible until first event
			}
			else{
				labelMsg.Text=status;
			}
			//if(hasProgHelper) {
			if(progressBarHelper.BlockMax!=0) {
				progressBar.Maximum=progressBarHelper.BlockMax;
			}
			if(progressBarHelper.BlockValue!=0 && progressBarHelper.BlockValue>=progressBar.Minimum && progressBarHelper.BlockValue<=progressBar.Maximum) {
				progressBar.Value=progressBarHelper.BlockValue;
			}
			if(progressBarHelper.ProgressStyle==ProgBarStyle.Marquee) {
				progressBar.IsIndeterminate=true;;
			}
			else if(progressBarHelper.ProgressStyle==ProgBarStyle.Blocks) {
				progressBar.IsIndeterminate=false;
			}
			//}
			if(HasHistory) {
				if(_dateTimeLastEvent==DateTime.MinValue) {
					textHistoryMsg.Text=textHistoryMsg.Text+status.PadRight(60);//left aligned
					_dateTimeLastEvent=DateTime.Now;
				}
				else {
					textHistoryMsg.Text=textHistoryMsg.Text+GetElapsedTime(_dateTimeLastEvent,DateTime.Now)+"\r\n"+status.PadRight(60);
					_dateTimeLastEvent=DateTime.Now;
				}
				HistoryMsg=textHistoryMsg.Text;
			}
			if(progressBarHelper.ErrorMsg==""){
				labelError.Visible=false;
			}
			else{
				labelError.Visible=true;
				labelError.Text=progressBarHelper.ErrorMsg;
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

		private void butCancel_Click(object sender,EventArgs e) {
			if(!MessageCancel.IsNullOrEmpty()) {
				if(!MsgBox.Show(MsgBoxButtons.YesNo,MessageCancel,"")) {
					return;
				}
			}
			IsDialogCancel=true;//also causes window to Close.
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

		private void FormProgressAuto_FormClosing(object sender,CancelEventArgs e) {
			if(IsDialogOK) {
				ODEvent.Fired-=ODEvent_Fired;
				return;
			}
			if(_isForceClose) {
				ODEvent.Fired-=ODEvent_Fired;
				return;
			}
			//From here down, user intentionally tried to close window, either with X or with Cancel button, if visible.
			if(!ShowCancelButton) {//tried to close with X, which is not allowed if Cancel button is not showing.
				MessageBox.Show(StopNotAllowedMessage);//Message is only used in a few places.
				e.Cancel=true;
				return;
			}
			if(_threadWorker.IsAlive){//user manually cancelled action with either cancel or X, and they are allowed to.
				_threadWorker.Abort();
			}
			else{
				//must be HasHistory.  Button says "Close" here
				//in this case, the action is finished, but the form was kept open to display information to user.
				//User had to manually close form by either "Close" or XClick when finished viewing
				_threadWorker.Abort();
				IsDialogOK=true;
			}
			ODEvent.Fired-=ODEvent_Fired;
		}
		
	}
}
