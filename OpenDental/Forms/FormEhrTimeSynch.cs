using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Only shows for EHR users.  Synchronizes local time to specified NIST server.</summary>
	public partial class FormEhrTimeSynch:FormODBase {
		private DateTime _timeNist;
		private DateTime _timeServer;
		private DateTime _timeLocal;
		///<summary>Set true when launched while OpenDental starts.  Will automatically check times and close form silently if server time is in synch with local time.</summary>
		public bool IsAutoLaunch;

		public FormEhrTimeSynch() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrTime_Load(object sender,EventArgs e) {
			if(IsAutoLaunch) { //Already updated time fields. Dont need to check again, just open.
				refreshDisplays();
				return;
			}
			textNistUrl.Text=PrefC.GetString(PrefName.NistTimeServerUrl);
			SynchTimes();			
		}

		///<summary>Called from FormOpenDental.Load.  Updates local time and checks to see if server time is in synch, with a fast db call (only acurate to seconds, not miliseconds).</summary>
		public bool TimesInSynchFast() {
			this.Cursor=Cursors.WaitCursor;
			textNistUrl.Text=PrefC.GetString(PrefName.NistTimeServerUrl);
			double nistOffset=GetNistOffset();
			if(nistOffset==double.MaxValue) { //Timed out
				MsgBox.Show(this,"No response received from NIST time server.  Click synch time after four seconds.");
				this.Cursor=Cursors.Default;
				return false;
			}
			if(nistOffset==double.MinValue) { //Invalid Nist Server Address
				this.Cursor=Cursors.Default;
				return false;
			}
			//Get current times from offsets
			_timeLocal=DateTime.Now;
			Stopwatch stopwatch = new Stopwatch(); //Used to keep NIST time in time even when system fails to set local machine time (Can't pull it later)
			stopwatch.Start();
			_timeNist=_timeLocal.AddMilliseconds(nistOffset);
			try {
				WindowsTime.SetTime(_timeNist); //Sets local machine time
			}
			catch {
				MsgBox.Show(this,"Error setting local machine time.");
			}
			_timeLocal=DateTime.Now;//Update time since it has now been set
			double serverOffset=(MiscData.GetNowDateTime()-DateTime.Now).TotalSeconds; //Cannot get milliseconds from Now() in Mysql Pre-5.6.4, Only gets whole seconds.
			_timeServer=_timeLocal.AddSeconds(serverOffset);
			if(ServerInSynchFast() && LocalInSynch()) { //All times in synch
				return true;
			}
			//Some times out of synch, so form will open, but we don't want to make another call to NIST server.
			serverOffset=(MiscData.GetNowDateTimeWithMilli()-DateTime.Now).TotalSeconds; //_timeServer needs to be more accurate before displaying
			_timeLocal=DateTime.Now;
			stopwatch.Stop();
			_timeServer=_timeLocal.AddSeconds(serverOffset);
			_timeNist=_timeNist.AddMilliseconds(stopwatch.ElapsedMilliseconds);
			this.Cursor=Cursors.Default;
			return false;
		}

		///<summary>Updates local time and Refreshes all time variables.  Saves NIST server URL as preference.</summary>
		private void SynchTimes() {
			this.Cursor=Cursors.WaitCursor;
			//Get NistTime Offset
			double nistOffset=GetNistOffset();
			if(nistOffset==double.MaxValue) { //Timed out
				MsgBox.Show(this,"No response received from NIST time server.  Click synch time after four seconds.");
				this.Cursor=Cursors.Default;
				return;
			}
			if(nistOffset==double.MinValue) { //Invalid Nist Server Address
				this.Cursor=Cursors.Default;
				return;
			}
			double serverOffset=(MiscData.GetNowDateTimeWithMilli()-DateTime.Now).TotalMilliseconds;
			//Get current times from offsets
			_timeLocal=DateTime.Now;
			_timeServer=_timeLocal.AddMilliseconds(serverOffset);
			_timeNist=_timeLocal.AddMilliseconds(nistOffset);
			try {
				WindowsTime.SetTime(_timeNist); //Sets local machine time
			}
			catch {
				MsgBox.Show(this,"Error setting local machine time.");
			}
			_timeLocal=DateTime.Now; //Update time since it has now been set
			this.Cursor=Cursors.Default;
			refreshDisplays();
		}

		///<summary>Get the offset from the nist server and DateTime.Now().  Returns double.MinValue if invalid NIST Server URL.  Returns double.MaxValue if request timed out.</summary>
		private double GetNistOffset() {
			//NistTime
			NTPv4 ntp=new NTPv4();
			double nistOffset;
			try {
				nistOffset=ntp.getTime(textNistUrl.Text);
			}
			catch {
				MsgBox.Show(this,"Invalid NIST Server URL");
				return double.MinValue;
			}
			timerSendingLimit.Enabled=true;
			return nistOffset;
		}

		///<summary>Returns true if server time is out of synch with local machine.</summary>
		private bool ServerInSynch() {
			//Would be better to check against NIST time, but doing it this way to match 2014 EHR Proctor Sheet conditions.
			double difference=Math.Abs(_timeServer.Subtract(_timeLocal).TotalSeconds);
			if(difference>.99) {
				return false;
			}
			return true;
		}

		///<summary>Used when launching check automatically on startup.  Rounds to whole seconds.  Returns true if server time is out of synch with local machine.</summary>
		private bool ServerInSynchFast() {
			double difference=Math.Abs(_timeServer.Subtract(_timeLocal).TotalSeconds);
			if(Math.Floor(difference)>1) {
				return false;
			}
			return true;
		}

		///<summary>Returns true if local time is out of synch with NIST server.  Should always be true since we just updated time.</summary>
		private bool LocalInSynch() {
			double difference=Math.Abs(_timeLocal.Subtract(_timeNist).TotalSeconds);
			if(difference>.99) {
				return false;
			}
			return true;
		}

		///<summary>Refresh the time textboxes.  Stops users from sending requests to NIST server more than once every 4 seconds.</summary>
		private void butRefreshTime_Click(object sender,EventArgs e) {
			if(timerSendingLimit.Enabled) {
				MsgBox.Show(this,"Cannot send a time request more than once every four seconds");
				return;
			}
			SynchTimes();
		}

		///<summary>Refresh all time displays and labels.</summary>
		private void refreshDisplays() {
			if(_timeNist==DateTime.MinValue || _timeServer==DateTime.MinValue || _timeLocal==DateTime.MinValue) {
				textLocalTime.Text="";
				textNistTime.Text="";
				textServerTime.Text="";
				textMessage.Text="";
				return;
			}
			textNistTime.Text=_timeNist.ToString("hh:mm:ss.fff tt");
			textServerTime.Text=_timeServer.ToString("hh:mm:ss.fff tt");
			textLocalTime.Text=_timeLocal.ToString("hh:mm:ss.fff tt");
			//Update NistURL preference
			Prefs.UpdateString(PrefName.NistTimeServerUrl,textNistUrl.Text);
			//Update message textbox
			if(!LocalInSynch()) { //This should not happen, time is updated automatically. If you get to this point, you should have already had a message box pop up saying there was an error updating your local time.
				textMessage.Text="Your local machine time is out of synch.  Please ensure Open Dental is running with Administrator Windows privileges.  If you have done this and you still see this message, please call Open Dental support.";
			}
			else if(!ServerInSynch()) {
				textMessage.Text="Your database time is out of synch with your local machine.  Please start the Open Dental application on your database server and leave it running to keep times synchronized as required for EHR compliance.  If you have done this and you still see this message, please call Open Dental support.";
			}
			else {//All times in synch
				textMessage.Text="All times synchronized within one second.  You may close this window.";
			}
		}

		///<summary>Do not allow user to send another request until timer has ticked.</summary>
		private void timerSendingLimit_Tick(object sender,EventArgs e) {
			timerSendingLimit.Enabled=false;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}