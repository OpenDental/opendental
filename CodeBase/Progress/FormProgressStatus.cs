using System;
using System.Windows.Forms;

namespace CodeBase {
	///<summary>Launch this window in a separate thread so that the progress bar can smoothly spin without waiting on the main thread.
	///Invoke CloseGracefully() in order to have the window gracefully close (as to not rely on thread abort).</summary>
	public partial class FormProgressStatus : FormProgressBase {
		///<summary>Indicates that this progress form is in "history" mode which will show a text box with all messages it takes action on and then will stay open forcing the user to click Close.</summary>
		private bool _hasHistory;
		///<summary>The date and time of the most recent event that this form processed.</summary>
		private DateTime _dateTimeLastEvent;
		///<summary>The date and time that this form was initialized.  Used to help calculate the total time.</summary>
		private DateTime _dateTimeInit;

		///<summary>Do not instatiate this class.  It is not meant for public use.  Use ODProgress.ShowProgressStatus() instead.
		///Launches a progress window that will constantly spin and display status updates for global ODEvents with corresponding name.
		///eventType must be a Type that contains an event called Fired.</summary>
		public FormProgressStatus(ODEventType odEventType=ODEventType.Undefined,Type typeEvent=null,bool hasHistory=false,bool hasMinimize=true,
			string startingMessage="",ProgressBarStyle progStyle=ProgressBarStyle.Marquee) : base(odEventType,typeEvent)
		{
			InitializeComponent();
			labelMsg.Text=startingMessage;
			progressBar.Style=progStyle;
			if(!hasMinimize) {
				panelMinimize.Visible=false;
			}
			this.ControlBox=false;
			_hasHistory=hasHistory;
			if(_hasHistory) {
				this.Height+=120;
				this.Width+=60;
				_dateTimeLastEvent=DateTime.MinValue;
				labelMsg.Visible=false;
				textHistoryMsg.Visible=true;
			}
			_dateTimeInit=DateTime.Now;
		}

		public sealed override void UpdateProgress(string status,ProgressBarHelper progHelper,bool hasProgHelper) {
			if(Visible && _hasHistory && !progressBar.Visible) {
				//Once the progress bar is hidden when history is showing, we never want to process another event.
				return;
			}
			labelMsg.Text=status;
			if(hasProgHelper) {
				if(progHelper.BlockMax!=0) {
					progressBar.Maximum=progHelper.BlockMax;
				}
				if(progHelper.BlockValue!=0) {
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
			if(_hasHistory) {
				if(_dateTimeLastEvent==DateTime.MinValue) {
					textHistoryMsg.AppendText(status.PadRight(60));
					_dateTimeLastEvent=DateTime.Now;
				}
				else {
					textHistoryMsg.AppendText(GetElapsedTime(_dateTimeLastEvent,DateTime.Now)+"\r\n"+status.PadRight(60));
					_dateTimeLastEvent=DateTime.Now;
				}
			}
		}

		///<summary>Gets a user friendly elapsed time message to display in the history text box.</summary>
		private string GetElapsedTime(DateTime start, DateTime end) {
			TimeSpan timeElapsed=new TimeSpan(end.Ticks-start.Ticks);
			if(timeElapsed.TotalMinutes>2) {
				return "Elapsed Time: "+timeElapsed.TotalMinutes+" minutes";
			}
			else if(timeElapsed.TotalSeconds>2) {
				return "Elapsed Time: "+timeElapsed.TotalSeconds+" seconds";
			}
			else {
				return "Elapsed Time: "+timeElapsed.TotalMilliseconds+" milliseconds";
			}
		}

		private void labelMinimize_Click(object sender,EventArgs e) {
			WindowState=FormWindowState.Minimized;
			if(Owner!=null) {
				Owner.Focus();//This is because the above line would sometimes cause the application behind OD come into focus.
			}
		}
		
		private void labelMinimize_MouseEnter(object sender,EventArgs e) {
			labelMinimize.BackColor=System.Drawing.SystemColors.ControlDark;
		}

		private void labelMinimize_MouseLeave(object sender,EventArgs e) {
			labelMinimize.BackColor=System.Drawing.SystemColors.ControlLight;
		}

		private void butCopyToClipboard_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(textHistoryMsg.Text);
				MessageBox.Show("Copied");
			}
			catch(Exception) {
				MessageBox.Show("Could not copy contents to the clipboard.  Please try again.");
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			ForceClose=true;
		}
	}
}