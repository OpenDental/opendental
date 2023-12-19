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

		///<summary>The date and time of the most recent event that this form processed. Used for history.</summary>
		private DateTime _dateTimeLastEvent;
		private EventInfo _eventInfoFired;
		private Thread _threadWorker;

		public FrmProgressAuto() {
			InitializeComponent();
			Load+=FrmProgressAuto_Load;
			FormClosing+=FormProgressAuto_FormClosing;
			CloseXClicked+=FrmProgressAuto_CloseXClicked;
		}

		private void FrmProgressAuto_Load(object sender,EventArgs e) {
			ODEvent.Fired+=ODEvent_Fired;
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
				SetWidth(556);
				SetHeight(182);
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
					Dispatcher.Invoke(delegate() { UpdateProgress(str,progressBarHelper2,true); });
				}
				catch(Exception ex) {
					ex.DoNothing();//It's just progress...
				}
			}
			if(eventArgs.Tag is ProgressBarHelper progressBarHelper){
				try {
					Dispatcher.Invoke(delegate() { UpdateProgress(progressBarHelper.LabelValue,progressBarHelper,true); });
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
					IsDialogOK=false;
				});
				return;
			}
			catch(Exception ex) {//normal exception
				ExceptionGenerated=ex;
				Dispatcher.Invoke(() => { 
					_threadWorker.Abort();
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

		public void UpdateProgress(string status,ProgressBarHelper progHelper,bool hasProgHelper) {
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
			if(hasProgHelper) {
				if(progHelper.BlockMax!=0) {
					progressBar.Maximum=progHelper.BlockMax;
				}
				if(progHelper.BlockValue!=0 && progHelper.BlockValue>=progressBar.Minimum && progHelper.BlockValue<=progressBar.Maximum) {
					progressBar.Value=progHelper.BlockValue;
				}
				if(progHelper.ProgressStyle==ProgBarStyle.Marquee) {
					progressBar.IsIndeterminate=true;;
				}
				else if(progHelper.ProgressStyle==ProgBarStyle.Blocks) {
					progressBar.IsIndeterminate=false;
				}
			}
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
			if(_threadWorker.IsAlive){ 
				_threadWorker.Abort();
				IsDialogOK=false;
			}
			else{
				//must be HasHistory.  Button says "Close" here.
				_threadWorker.Abort();
				IsDialogOK=true;
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

		private void FrmProgressAuto_CloseXClicked(object sender,CancelEventArgs e) {
			if(!ShowCancelButton){
				MessageBox.Show(StopNotAllowedMessage);//Message is only used in a few places.
				e.Cancel=true;
				return;
			}
			if(_threadWorker.IsAlive){ 
				_threadWorker.Abort();
				IsDialogOK=false;
			}
			else{
				//must be HasHistory.  Button says "Close" here.
				_threadWorker.Abort();
				IsDialogOK=true;
			}
		}

		private void FormProgressAuto_FormClosing(object sender,CancelEventArgs e) {
			ODEvent.Fired-=ODEvent_Fired;
		}
		
	}
}
