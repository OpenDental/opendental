using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;

namespace ProximityOD {
	public partial class FormProximityOD : Form {
		private ODSonicSensor _sensor;
		private Timer _timerDatabase;
		private Timer _timerStopWebCamSnapshotPref;
		///<summary>The last value saved to the DB for proximity.</summary>
		private bool _proxDB;
		private bool _proxSoftLogic;
		private Timer _acquisitionTimer;
		private Timer _releaseTimer;
		private bool _isAcquiring;
		private bool _isReleasing;
		///<summary>Reflects the StopWebCamSnapshot preference in customers db.  Set by _timerStopWebCamSnapshotPref which runs every minute.</summary>
		private bool _isStopWebCamSnapshot;
		///<summary>DateTime.MinValue when away, otherwise this is the last dateTime that the sensor was both present, and movement was detected.</summary>
		private DateTime _dtLastMovement;
		private DateTime _dtLastSave=DateTime.MinValue;
		private int _extension=0;

		private int Extension {
			get {
				if(_extension==0) {
					ODException.SwallowAnyException(() => {
						_extension=PhoneComps.GetExtForComputer(Environment.MachineName);
					});
				}
				return _extension;
			}
		}

		private int _rangeOverride { get; set; }

		public FormProximityOD() {
			InitializeComponent();
			labelDateTProximal.Text="";
		}

		private void FormProximityOD_Load(object sender,EventArgs e) {
			#region Check ProximityOD Process
			if(!ODBuild.IsDebug()) {
				Process[] processes = Process.GetProcessesByName("ProximityOD");
				for(int p = 0;p<processes.Length;p++) {
					if(Process.GetCurrentProcess().Id==processes[p].Id) {
						continue;
					}
					//another process was found
					MessageBox.Show(this,"ProximityOD is already running.","ProximityOD");
					Application.Exit();
					return;
				}
			}
			#endregion
			ODException.SwallowAnyException(() => { GetSettings(); });
			#region Database Connection
			//First connect to customers database using ConnectionStore.xml.
			//This will be overridden by site specific connection settings later (see TryDatabaseConnectionSettings() and DataAction.RunCustomers()).
			try {
				ConnectionStore.SetDb(ConnectionNames.CustomersHQ);
			}
			catch(Exception ex) {
				labelError.Visible=true;
				MessageBox.Show("This tool is not designed for general use.\r\n\r\n"+ex.Message);
				return;
			}
			#endregion
			_sensor=new ODSonicSensor();
			_sensor.ProximityChanged+=Sensor_ProximityChanged;
			_sensor.RangeChanged+=Sensor_RangeChanged;
			_timerStopWebCamSnapshotPref=new Timer() { Interval=60000 };
			_timerStopWebCamSnapshotPref.Tick+=TimerStopWebCamSnapshotPref_Tick;
			_timerStopWebCamSnapshotPref.Start();
			_timerDatabase=new Timer() { Interval=1600 };
			_timerDatabase.Tick+=TimerDatabase_Tick;
			_timerDatabase.Start();
			_acquisitionTimer=new Timer() { Interval=2500 };
			_acquisitionTimer.Tick+=acquisitionTimer_Tick;
			_releaseTimer=new Timer() { Interval=1500 };
			_releaseTimer.Tick+=releaseTimer_Tick;
			textProximityDev.Text="Away";
			notifyIcon.Visible=false;
			textProximityDev.BackColor=Color.FromArgb(255,200,200);//light red
			textProximitySoft.Text="Away";
			textProximitySoft.BackColor=Color.FromArgb(255,200,200);//light red
			textTriggerSoft.Text=_rangeOverride.ToString()+"\"";
		}

		private void FormProximityOD_Shown(object sender,EventArgs e) {
			//Automatically minimize the application if there is a successful database connection.
			//This has to happen within the Shown event handler because the window has yet to show to the user and Hide() will be ignored otherwise.
			if(TryDatabaseConnectionSettings()) {
				//The database connection was successful.  Minimize the application by default.
				MinimizeToTray();
			}
			else {
				//The database connection failed.  Show the application to the user so that they can see that something is wrong.
				this.WindowState=FormWindowState.Normal;
			}
		}

		///<summary>Attempts to connect and get connection settings from the database that is used by DataAction.RunCustomers().
		///Returns true and sets text boxes to the corresponding connectino setting values if a successful connection was made.
		///Otherwise; displays an error message to the user and returns false.  Assumes a connection to a 'customers' db has already been made.</summary>
		private bool TryDatabaseConnectionSettings() {
			//Assume that we have a guaranteed connection to HQ's 'customers' database.
			//Look and see what connection settings this computer should use via DataAction.RunCustomers().
			_timerDatabase.Stop();
			try {
				//Clear out the currently cached HQ database connections and refresh both the preference and site link caches.
				DataAction.ClearDictHqCentralConnections();
				Prefs.RefreshCache();
				SiteLinks.RefreshCache();
				string serverName="";
				string databaseName="";
				string mySqlUser="";
				string mySqlPassword="";
				Action action=() => {
					serverName=DataConnection.GetServerName();
					databaseName=DataConnection.GetDatabaseName();
					mySqlUser=DataConnection.GetMysqlUser();
					mySqlPassword=DataConnection.GetMysqlPass();
				};
				DataAction.RunCustomers(action,useConnectionStore:false);
				//Display the settings to the user so that we can easily tell what database this instance is pointing to.
				textServerName.Text=serverName;
				textDatabaseName.Text=databaseName;
				textMySQLUser.Text=mySqlUser;
				textMySQLPassword.Text=mySqlPassword;
			}
			catch(Exception ex) {
				labelError.Visible=true;
				MessageBox.Show(this,"There was an error making a connection to the database.\r\n"
					+"Please correct the database connection settings then click Reconnect.\r\n\r\n"+ex.Message);
				return false;
			}
			_timerDatabase.Start();
			return true;
		}

		///<summary>Surround with Try/Catch</summary>
		private void GetSettings() {
			if(!System.IO.File.Exists("ProximitySettings.txt")) {
				ODException.SwallowAnyException(SaveSettings);
			}
			List<string> settings = System.IO.File.ReadAllLines("ProximitySettings.txt").ToList();//should be located in same directory as WebcamOD.
			_rangeOverride=int.Parse(settings.FirstOrDefault(x => x.ToLower().StartsWith("rangeoverride")).Split('|')[1]);
			menuOnTop.Checked=("1"==settings.FirstOrDefault(x => x.ToLower().StartsWith("isalwaysontop")).Split('|')[1]);
			switch(int.Parse(settings.FirstOrDefault(x => x.ToLower().StartsWith("proxoverride")).Split('|')[1])) {
				case 1:
					radioOverridePres.Checked=true;
					break;
				case 2:
					radioOverrideAway.Checked=true;
					break;
				case 0:
				default:
					radioOverrideAuto.Checked=true;
					break;
			}
		}

		///<summary>Surround with Try/Catch.</summary>
		private void SaveSettings() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("RangeOverride|"+_rangeOverride);
			sb.AppendLine("IsAlwaysOnTop|"+(menuOnTop.Checked ? 1 : 0));
			int idxOverride = new[] { radioOverrideAuto.Checked,radioOverridePres.Checked,radioOverrideAway.Checked }.ToList().FindIndex(x => x);
			sb.AppendLine("ProxOverride|"+idxOverride);
			System.IO.File.WriteAllText("ProximitySettings.txt",sb.ToString());
		}

		///<summary>Timer that runs every minute and queries the current database connection for the StopWebCamSnapshot preference.
		///Updates _isStopWebCamSnapshot to reflect the current preference value.  This is so that we do not have to process signals.</summary>
		private void TimerStopWebCamSnapshotPref_Tick(object sender,EventArgs e) {
			_timerStopWebCamSnapshotPref.Stop();
			ODException.SwallowAnyException(() => {
				DataAction.RunCustomers(() => { _isStopWebCamSnapshot=Prefs.GetBoolNoCache(PrefName.StopWebCamSnapshot); },useConnectionStore:false);
			});
			_timerStopWebCamSnapshotPref.Start();
		}

		private void TimerDatabase_Tick(object sender,EventArgs e) {
			//Check to see if our "kill switch" has been triggered within the customers db (_isStopWebCamSnapshot is updated every minute).
			if(_isStopWebCamSnapshot) {
				return;//Simply do nothing and purposefully leave _timerDatabase ticking so that it resumes acting ASAP.
			}
			try {
				_timerDatabase.Stop();
				bool isProximal=false;
				if(radioOverrideAuto.Checked) { //Auto checked, calculate proximity based on normal logic.
					isProximal=(_rangeOverride>0) ? (_sensor.Range<_rangeOverride) : _sensor.IsProximal;
				}
				else { //Handles both remaining cases, Override present and override away.
					isProximal=radioOverridePres.Checked;
				}
				if(isProximal==_proxDB && DateTime.Now.Subtract(_dtLastSave).TotalSeconds<30) { //Nothing changed and we just saved within the last 30 seconds. No db update needed.
					return;
				}
				if(DateTime.Now.Subtract(_dtLastMovement).TotalSeconds>60) { //We can't possibly be proximal if there has been no movement in the past 60 seconds.
					isProximal=false;
				}
				if(!isProximal && !_proxDB) {
					//The user is not proximal AND the database already has them as not proximal then we do NOT need to send a query to the database.
					//phone.IsProximal TRUMPS the value of DateTProximal so it is wasteful to update it with "NOW()".
					return;
				}
				if(SetProximity(isProximal)) { //We saved to db so update the local cache.
					_dtLastSave=DateTime.Now;
					_proxDB=isProximal;
					//Db comm is working so set the interval to it's normal rate.
					_timerDatabase.Interval=1600;
					labelDateTProximal.Text=_dtLastSave.ToString();
					labelError.Visible=false;
				}
				else { //Db comm is not working so slow the interval down to prevent mini DDOS from this program.
					_timerDatabase.Interval=15000;
					labelError.Visible=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			finally { //Restart the timer no matter what.
				_timerDatabase.Start();
			}
		}

		///<summary>Returns true if db success; otherwise returns false.</summary>
		private bool SetProximity(bool isProximal) {
			try {
				if(Extension==0) {
					return false;//There is most likely a setup issue or db connection problem.
				}
				DataAction.RunCustomers(() => Phones.SetProximity(isProximal,Extension),useConnectionStore: false);
				//We got here so everything worked ok.
				return true;
			}
			catch(Exception e) {
				e.DoNothing();
			}
			//We got here so something failed.
			return false;
		}

		private void Sensor_RangeChanged(int range) {
			_dtLastMovement=DateTime.Now;
			this.Invoke((Action)(() => {
				try {
					textRange.Text=range.ToString()+"\"";
					if(_proxSoftLogic && (range<_rangeOverride)) { //currently proximal and should be proximal
						_isReleasing=false;
						_releaseTimer.Stop();
						return;
					}
					if(!_proxSoftLogic && (range>=_rangeOverride)) { //currently not proximal and should not be
						_isAcquiring=false;
						_acquisitionTimer.Stop();
						return;
					}
					if(range<_rangeOverride && !_isAcquiring) {
						_acquisitionTimer.Start();
						_isAcquiring=true;
					}
					else if(range>=_rangeOverride && !_isReleasing) {
						_releaseTimer.Start();
						_isReleasing=true;
					}
				}
				catch { }
			}));
		}

		private void Sensor_ProximityChanged(bool isProximal) {
			_dtLastMovement=DateTime.Now;
			this.Invoke((Action)(() => {
				notifyIcon.Visible=false;
				try {
					bool proxCur = (isProximal && radioOverrideAuto.Checked) || (radioOverridePres.Checked);
					if(proxCur) {
						textProximityDev.Text="Present";
						textProximityDev.BackColor=Color.FromArgb(200,255,200);//light green
						notifyIcon.Icon=Properties.Resources.proximity_present;
						this.Icon=Properties.Resources.proximity_present;
					}
					else {
						textProximityDev.Text="Away";
						textProximityDev.BackColor=Color.FromArgb(255,200,200);//light red
						notifyIcon.Icon=Properties.Resources.proximity_away;
						this.Icon=Properties.Resources.proximity_away;
					}
				}
				catch { }
				notifyIcon.Visible=true;
			}));
		}

		private void SetSoftwarePresence() {
			this.Invoke((Action)(() => {
				try {
					bool proxCur = (_proxSoftLogic&&radioOverrideAuto.Checked) || (radioOverridePres.Checked);
					if(proxCur) {
						textProximitySoft.Text="Present";
						textProximitySoft.BackColor=Color.FromArgb(200,255,200);//light green
					}
					else {
						textProximitySoft.Text="Away";
						textProximitySoft.BackColor=Color.FromArgb(255,200,200);//light red
					}
				}
				catch { }
			}));
		}

		private void releaseTimer_Tick(object sender,EventArgs e) {
			_proxSoftLogic=false;
			_isReleasing=false;
			_releaseTimer.Stop();
			SetSoftwarePresence();
		}

		private void acquisitionTimer_Tick(object sender,EventArgs e) {
			_proxSoftLogic=true;
			_isAcquiring=false;
			_acquisitionTimer.Stop();
			SetSoftwarePresence();
		}

		private void textTriggerSoft_KeyPress(object sender,KeyPressEventArgs e) {
			if(e.KeyChar == (char)13) {//enter
				textTriggerSoft.Text=_rangeOverride+"\"";
				textTriggerSoft.SelectAll();
			}
		}

		private void textTriggerSoft_TextChanged(object sender,EventArgs e) {
			try {
				_rangeOverride=int.Parse(new string(textTriggerSoft.Text.Where(x => char.IsDigit(x)).ToArray()));
			}
			catch(Exception) {
				_rangeOverride=0;
			}
		}

		private void textTriggerSoft_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
			try {
				_rangeOverride=int.Parse(new string(textTriggerSoft.Text.Where(x => char.IsDigit(x)).ToArray()));
			}
			catch(Exception) {
				_rangeOverride=0;
			}
			textTriggerSoft.Text=_rangeOverride+"\"";
		}

		private void checkAlwaysOnTop_CheckedChanged(object sender,EventArgs e) {
			this.TopMost=menuOnTop.Checked;
		}

		private void radioOverrideAuto_CheckedChanged(object sender,EventArgs e) {
			SetSoftwarePresence();
			Sensor_ProximityChanged(_proxDB);//just update UI, in case we are using device settings.
		}

		private void MinimizeToTray() {
			this.WindowState=FormWindowState.Minimized;
			Hide();
			notifyIcon.Visible=true;
		}

		private void menuMinimize_Click(object sender,EventArgs e) {
			MinimizeToTray();
		}

		private void notifyIcon_DoubleClick(object sender,EventArgs e) {
			notifyIcon.Visible=false;
			Show();
			this.WindowState=FormWindowState.Normal;
			this.BringToFront();
		}

		private void exitProgramToolStripMenuItem_Click(object sender,EventArgs e) {
			if(notifyIcon!=null) {
				notifyIcon.Dispose();
			}	
			Application.Exit();
		}

		private void ButReconnect_Click(object sender,EventArgs e) {
			_timerDatabase.Stop();
			if(!TryDatabaseConnectionSettings()) {
				return;
			}
			labelError.Visible=false;
			MessageBox.Show(this,"Success, the database connection settings have been updated.");
			_timerDatabase.Start();
		}

		private void FormProximityOD_FormClosing(object sender,FormClosingEventArgs e) {
			ODException.SwallowAnyException(SaveSettings);
			if(_sensor!=null) {
				_sensor.ThreadExit=true;
			}
			if(notifyIcon!=null) {
				notifyIcon.Dispose();
			}	
		}
	}

}
