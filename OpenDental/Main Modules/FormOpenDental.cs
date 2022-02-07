/*=============================================================================================================
Open Dental 
Copyright 2003-2021  Jordan Sparks, DMD.  http://www.opendental.com

This program is free software; you can redistribute it and/or modify it under the terms of the
GNU Db Public License as published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

This program is distributed in the hope that it will be useful, but without any warranty. See the GNU Db Public License
for more details, available at http://www.opensource.org/licenses/gpl-license.php

Any changes to this program must follow the guidelines of the GPL license if a modified version is to be
redistributed.
===============================================================================================================*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using CodeBase;
using DataConnectionBase;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using SparksToothChart;
//using OpenDental.SmartCards;
using OpenDental.UI;
using System.ServiceProcess;
using System.Linq;
using OpenDental.Bridges;
using OpenDentBusiness.WebServiceMainHQ;
using ServiceManager;
using System.DirectoryServices;
using WMPLib;
using OpenDentalImaging;
using OpenDental.Thinfinity;
#if EHRTEST
using EHR;
#endif

namespace OpenDental{
	///<summary></summary>
	public partial class FormOpenDental:FormODBase {
		#region Fields - Public
		///<summary>Represents if the regkey is a developer regkey.</summary>
		public static bool IsRegKeyForTesting;
		///<summary>This is used to determine how Open Dental closed.  If this is set to anything but 0 then some kind of error occurred and Open Dental was forced to close.  Currently only used when updating Open Dental silently.</summary>
		public static int ExitCode=0;
		///<summary>Class for holding command line args passed in when program starts.</summary>
		public CommandLineArgs CommandLineArgs;
		///<summary>True if there is already a different instance of OD running.  This prevents attempting to start the listener.</summary>
		public bool IsSecondInstance;
		#endregion Fields - Public

		#region Fields - Private Static
		///<summary>This is the singleton instance of the FormOpenDental. This allows us to have S_ methods that are public static and can be called from anywhere in the program to update FormOpenDental.</summary>
		private static FormOpenDental _formOpenDentalSingleton;
		///<summary>HQ only. Multiple phone maps can be opened at the same time. This keeps a list of all that are open so we can modify their contents.</summary>
		private static List<FormMapHQ> _listFormMapHQs=new List<FormMapHQ>();
		///<summary>Todo: look into this dict of dicts.</summary>
		private static Dictionary<long,Dictionary<long,DateTime>> _dictionaryBlockedAutomations;
		///<summary>Task Popups use this upper limit of open FormTaskEdit instances to determine if a task should popup.  More than 115 open FormTaskEdit has been observed to crash the program.  See task #1481164.</summary>
		private static int _popupPressureReliefLimit=20;//20 is chosen arbitrarily.  We could implement a preference for this, with a max of 115.
		private static bool _isTreatPlanSortByTooth;
		///<summary>In most cases, CurPatNum should be used instead of _CurPatNum.</summary>
		private static long _patNumCur;
		#endregion Fields - Private Static

		#region Fields - Private
		///<summary>When user logs out, this keeps track of where they were for when they log back in.</summary>
		private EnumModuleType _moduleTypeLast;
		private Bitmap _bitmapIcon;
		///<summary>A list of button definitions for this computer.  These button defs display in the lightSignalGrid1 control.</summary>
		private SigButDef[] _arraySigButDefs;
		private bool _isMouseDownOnSplitter;
		private Point _pointSplitterOriginalLocation;
		private Point _pointOriginalMouse;
		///<summary>This list will only contain events for this computer where the users clicked to disable a popup for a specified period of time.  So it won't typically have many items in it.</summary>
		private List<PopupEvent> _listPopupEvents;
		private FormAlerts _formAlerts;
		private FormPhoneTiles _formPhoneTiles;
		private FormTerminalManager _formTerminalManager;
		private Form _formRecentlyOpenForLogoff;
		///<summary>When auto log off is in use, we don't want to log off user if they are in the FormLogOn window.  Mostly a problem when using web service because CurUser is not null.</summary>
		private bool _isFormLogOnLastActive;
		private FormCertifications _formCertifications;
		private FormCreditRecurringCharges _formCreditRecurringCharges;
		private long _patNumPrevious;
		private DateTime _datePopupDelay;
		///<summary>A secondary cache only used to determine if preferences related to the redrawing of the Chart module have been changed.</summary>
		private Dictionary<string,object> _dictionaryChartPrefsCache=new Dictionary<string,object>();
		///<summary>A secondary cache only used to determine if preferences related to the redrawing of the non-modal task list have been changed.</summary>
		private Dictionary<string,object> _dictionaryTaskListPrefsCache=new Dictionary<string,object>();
		///<summary>HQ only. Keep track of last time EServiceMetrics were filled. Server is only updating every 30 seconds so no need to go any faster than that.</summary>
		private DateTime _dateTimeHqEServiceMetricsLastRefreshed=DateTime.MinValue;
		///<summary>A specific reference to the "Text" button.  This special reference helps us preserve the notification text on the button after setup is modified.</summary>
		private ODToolBarButton _toolBarButtonText;
		///<summary>A specific reference to the "Task" button. This special reference helps us refresh the notification text on the button after the user changes.</summary>
		private ODToolBarButton _toolBarButtonTask;
		/// <summary>Command line can pass in show=... "Popup", "Popups", "ApptsForPatient", or "SearchPatient".  Stored here as lowercase.</summary>
		private string _strCmdLineShow="";
    private FormSmsTextMessaging _formSmsTextMessaging;
		private FormQuery _formQuery;
		private OpenDentalGraph.FormDashboardEditTab _formDashboardEditTab;
		private FormJobManager _formJobManager;
		///<summary>Tracks the reminder tasks for the currently logged in user.  Is null until the first signal refresh.  Includes new and viewed tasks.</summary>
		private List<Task> _listTasksReminders=null;
		///<summary>Gets initialized or refreshed when searching for archived task lists to exclude from reminders</summary>
		private Dictionary<long,TaskList> _dictionaryAllTaskLists;
		///<summary>Tracks reminder tasks that were not allowed to popup because we had too many FormTaskEdit windows open already.</summary>
		private List<Task> _listTasksRemindersOverLimit=null;
		///<summary>Tracks the normal (non-reminder) tasks for the currently logged in user.  Is null until the first signal refresh.</summary>
		private List<long> _listTaskNumsNormal=null;
		///<summary>Tracks the UserNum of the user for which the _listReminderTaskNums and _listOtherTaskNums belong to so we can compensate for different users logging off/on.</summary>
		private long _userNumTasks=0;
		///<summary>The date the appointment module reminders tab was last refreshed.</summary>
		private DateTime _dateReminderRefresh=DateTime.MinValue;
		///<summary>HQ only. Keep track of the last time the office down was checked. Too taxing on the server to perform every 1.6 seconds with the rest of the HQ thread metrics. Will be refreshed on ProcessSigsIntervalInSecs interval.</summary>
		private DateTime _dateHqOfficeDownLastRefreshed=DateTime.MinValue;
		///<summary>List of AlerReads for the current User.</summary>
		private List<AlertRead> _listAlertReads=new List<AlertRead>();
		///<summary>List of AlertItems for the current user and clinic.</summary>
		private List<AlertItem> _listAlertItems=new List<AlertItem>();
		private FormXWebTransactions _formXWebTransactions;
		private FormVoiceMails _formVoiceMails;
		private FormLoginFailed _formLoginFailed=null;
		///<summary>We will send a maximum of 1 exception to HQ that occurs when processing signals.</summary>
		private Exception _exceptionSignalsTick;
		///<summary>This will be set to true if signal processing has been paused due to inactivity or if the login window is showing when the signal timer ticks.  When signal processing resumes the current Application.ProductVersion will be compared to the db value of PrefName.ProgramVersion and if these two versions don't match, the user will get a message box informing them that OD will have to shutdown and the user will have to relaunch to correct the version mismatch.  We will also check the UpdateInProgressOnComputerName and CorruptedDatabase prefs.</summary>
		private bool _hasSignalProcessingPaused=false;
		///<summary>This is the location of the splitter at 96dpi. That way, it can be reliably and consistently redrawn, regardless of the current dpi.  Either X or Y will be ignored.  For example, if it's docked to the bottom, then only Y will be used.</summary>
		private PointF _pointPanelSplitter96dpi;
		private FormCareCreditTransactions _formCareCreditTransactions;
		///<summary>This is used for checking for shutdown signals while inactive, see SignalsTickWhileInactive()</summary>
		private DateTime _dateTimeSignalShutdownLastChecked=DateTime.MinValue;
		///<summary>Only instance of Dynamic Pay Plan Overcharge window.</summary>
		private FormRpDPPOvercharged _formRpDPPOvercharged;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public FormOpenDental(string[] cla){
			_formOpenDentalSingleton=this;
			Logger.DoVerboseLogging=PrefC.IsVerboseLoggingSession;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			CommandLineArgs=new CommandLineArgs(cla);
			FormSplash formSplash=new FormSplash();
			if(CommandLineArgs.ArrayCommandLineArgs.Length==0) {
				formSplash.Show();
			}
			InitializeComponent();
			InitializeLayoutManager();
			if(LayoutManagerForms.IsTestingMode()){//For rapid testing, we need it to start center screen, not max.
				/*
				WindowState=FormWindowState.Normal;
				Size=new Size(1400, 900);
				Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(Handle).WorkingArea;
				Location=new Point(
					rectangleWorkingArea.X+rectangleWorkingArea.Width/2-Width/2,
					rectangleWorkingArea.Y+rectangleWorkingArea.Height/2-Height/2
					);*/
			}
			_fontTitle=new Font("Segoe",13);
			SystemEvents.SessionSwitch+=new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			//toolbar		
			ToolBarMain=new ToolBarOD();
			ToolBarMain.Name="ToolBarMain";
			ToolBarMain.Location=new Point(51,0);
			ToolBarMain.Size=new Size(931,25);
			ToolBarMain.Dock=DockStyle.Top;
			ToolBarMain.ImageList=imageListMain;
			ToolBarMain.ButtonClick+=new ODToolBarButtonClickEventHandler(toolBarMain_ButtonClick);
			LayoutManager.Add(ToolBarMain,this);
			//module bar
			moduleBar=new ModuleBar();
			moduleBar.Location=new Point(0,0);
			moduleBar.Size=new Size(51,626);
			moduleBar.Dock=DockStyle.Left;
			moduleBar.ButtonClicked+=new ButtonClickedEventHandler(moduleBar_ButtonClicked);
			LayoutManager.Add(moduleBar,this);
			menuMain.SendToBack();//so it has top dock priority
			//MAIN MODULE CONTROLS
			//contrApptJ
			controlAppt=new ControlAppt() { Visible=false };
			controlAppt.Dock=DockStyle.Fill;
			LayoutManager.Add(controlAppt,splitContainerNoFlickerDashboard.Panel1);
			//contrFamily
			controlFamily=new ControlFamily() { Visible=false };
			controlFamily.Dock=DockStyle.Fill;
			LayoutManager.Add(controlFamily,splitContainerNoFlickerDashboard.Panel1);
			//contrFamilyEcw
			controlFamilyEcw=new ControlFamilyEcw() { Visible=false };
			controlFamily.Dock=DockStyle.Fill;
			LayoutManager.Add(controlFamilyEcw,splitContainerNoFlickerDashboard.Panel1);
			//contrAccount
			controlAccount=new ControlAccount() { Visible=false };
			controlAccount.Dock=DockStyle.Fill;
			LayoutManager.Add(controlAccount,splitContainerNoFlickerDashboard.Panel1);
			//contrTreat
			controlTreat=new ControlTreat() { Visible=false };
			controlTreat.Dock=DockStyle.Fill;
			LayoutManager.Add(controlTreat,splitContainerNoFlickerDashboard.Panel1);
			//contrChart
			controlChart=new ControlChart() { Visible=false };
			controlChart.Dock=DockStyle.Fill;
			LayoutManager.Add(controlChart,splitContainerNoFlickerDashboard.Panel1);
			//contrImages
			//Moved down to Load because it needs a pref to decide which one to load.
			//contrManage
			controlManage=new ControlManage() { Visible=false };
			controlManage.Dock=DockStyle.Fill;
			LayoutManager.Add(controlManage,splitContainerNoFlickerDashboard.Panel1);
			userControlDashboard=new UserControlDashboard();
			userControlDashboard.Anchor=AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			userControlDashboard.Dock=DockStyle.Fill;
			userControlDashboard.Size=new Size(splitContainerNoFlickerDashboard.Panel2.Width,splitContainerNoFlickerDashboard.Panel2.Height);
			userControlDashboard.AutoScroll=true;
			LayoutManager.Add(userControlDashboard,splitContainerNoFlickerDashboard.Panel2);
			userControlTasks1=new UserControlTasks() { Visible=false };
			LayoutManager.Add(userControlTasks1,this);
			panelSplitter.ContextMenu=menuSplitter;
			menuItemDockBottom.Checked=true;
			phoneSmall=new UserControlPhoneSmall();
			phoneSmall.GoToChanged += new System.EventHandler(this.phoneSmall_GoToChanged);
			//phoneSmall.Visible=false;
			//this.Controls.Add(phoneSmall);
			LayoutManager.Add(phoneSmall,panelPhoneSmall);
			panelPhoneSmall.Visible=false;
			//phonePanel=new UserControlPhonePanel();
			//phonePanel.Visible=false;
			//this.Controls.Add(phonePanel);
			//phonePanel.GoToChanged += new System.EventHandler(this.phonePanel_GoToChanged);
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
			//Plugins.HookAddCode(this,"FormOpenDental.Constructor_end");//Can't do this because no plugins loaded.
			formSplash.Close();
		}
		#endregion Constructor

		#region Properties
		///<summary>PatNum for currently loaded patient.</summary>
		[Browsable(false)]
		public static long PatNumCur {
			get {
				return _patNumCur;
			}
			set {
				if(value==_patNumCur) {
					return;
				}
				_patNumCur=value;
				PatientChangedEvent.Fire(ODEventType.Patient,value);
			}
		}

		///<summary>Dictionary of AutomationNums mapped to a dictionary of patNums and dateTimes. The dateTime is the time that the given automation for a specific patient should be blocked until. Dictionary removes any entries whos blocked until dateTime is greater than DateTime.Now before returning.  Currently only used when triggered Automation.AutoAction == AutomationAction.PopUp</summary>
		[Browsable(false)]
		public static Dictionary<long,Dictionary<long,DateTime>> DicBlockedAutomations {
			get {
				if(_dictionaryBlockedAutomations==null){
					_dictionaryBlockedAutomations=new Dictionary<long,Dictionary<long,DateTime>>();
					return _dictionaryBlockedAutomations;
				}
				List<long> listAutoNums=_dictionaryBlockedAutomations.Keys.ToList();
				List<long> listPatNums;
				foreach(long automationNum in listAutoNums) {//Key is an AutomationNum
					listPatNums=_dictionaryBlockedAutomations[automationNum].Keys.ToList();
					foreach(long patNum in listPatNums) {//Key is a patNum for current AutomationNum key.
						if(_dictionaryBlockedAutomations[automationNum][patNum]>DateTime.Now) {//Disable time has not expired yet.
							continue;
						}
						_dictionaryBlockedAutomations[automationNum].Remove(patNum);//Remove automation for current user since block time has expired.
						//Since we removed an entry from the lower level dictionary we need to check if there are still entries in the top level dictionary. 
					}
					if(_dictionaryBlockedAutomations[automationNum].Count()==0) {//Top level dictionary no longer contains entries for current automationNum.
						_dictionaryBlockedAutomations.Remove(automationNum);
					}
				}
				return _dictionaryBlockedAutomations;
			}
		}

		///<summary>Loads this value from PrefName.TreatPlanSortByTooth on startup.  The user can change this value without changing the pref from the treatplan module.</summary>
		[Browsable(false)]
		public static bool IsTreatPlanSortByTooth {
			get {
				return _isTreatPlanSortByTooth;
			}
			set {
				_isTreatPlanSortByTooth=value;
				PrefC.IsTreatPlanSortByTooth=value;
			}
		}

		[Browsable(false)]
		public static PhoneTile PhoneTile {
			get {
				return _formOpenDentalSingleton.phoneSmall.PhoneTile;
			}
		}

		///<summary>List of tab titles for the TabProc control. Used to get accurate preview in sheet layout design view. Returns a list of one item called "Tab" if something goes wrong.</summary>
		[Browsable(false)]
		public static List<string> S_Contr_TabProcPageTitles { 
			get { 
				return _formOpenDentalSingleton.controlChart.ListTabProcPageTitles; 
			} 
		}
		#endregion Properties

		#region Methods - Public Static
		public static void S_DataValid_BecomeInvalid(OpenDental.ValidEventArgs e) {
			_formOpenDentalSingleton?.DataValid_BecameInvalid(e);//Can be null if called from other projects like CEMT
		}

		///<summary>Referenced at least 40 times indirectly.</summary>
		public static void S_GotoModule_ModuleSelected(ModuleEventArgs e) {
			_formOpenDentalSingleton.GotoModule_ModuleSelected(e);
		}

		///<summary>This is a way that any form within Open Dental can ask the main form to refresh whatever module is currently selected.</summary>
		public static void S_RefreshCurrentModule(bool hasForceRefresh=false,bool isApptRefreshDataPat=true,bool isClinicRefresh=true) {
			_formOpenDentalSingleton.RefreshCurrentModule(hasForceRefresh,isApptRefreshDataPat,isClinicRefresh);
		}

		///<summary>If the call to this is followed by ModuleSelected or GotoModule, set isRefreshCurModule=false to prevent the module from being refreshed twice.  If the current module is ContrAppt and the call to this is preceded by a call to RefreshModuleDataPatient, set isApptRefreshDataPat=false so the query to get the patient does not run twice.</summary>
		public static void S_Contr_PatientSelected(Patient pat,bool isRefreshCurModule,bool isApptRefreshDataPat=true,bool hasForcedRefresh=false) {
			_formOpenDentalSingleton.Contr_PatientSelected(pat,isRefreshCurModule,isApptRefreshDataPat,hasForcedRefresh);
		}
		#endregion Methods - Public Static

		#region Methods - Event Handlers - Form
		private void FormOpenDental_Load(object sender,System.EventArgs e) {
			//We want the main window to show prior to the DataConnection dialog, so there's nothing much to do here.
			//LayoutToolBar();//It would be nice to see the toolbar, but it would take a little fiddling.
			Logger.LogToPath("LayoutMenu",LogPath.Startup,LogPhase.Start);
			LayoutMenu();
			Logger.LogToPath("LayoutMenu",LogPath.Startup,LogPhase.End);
			Logger.LogToPath("LayoutFormBoundsAndFonts",LogPath.Startup,LogPhase.Start);
			LayoutManager.LayoutFormBoundsAndFonts(this);
			Logger.LogToPath("LayoutFormBoundsAndFonts",LogPath.Startup,LogPhase.End);
			string appDir=Application.StartupPath;
			if(File.Exists(Path.Combine(appDir,"NoD2D.txt"))){
				IconLibrary.OnlyGDI=true;
			}
		}

		private void FormOpenDental_Shown(object sender,EventArgs e) {
			Logger.LogToPath("FormOpenDentalShown",LogPath.Startup,LogPhase.Start);
			FormOpenDentalShown();
			Logger.LogToPath("FormOpenDentalShown",LogPath.Startup,LogPhase.End);
		}

		public void FormOpenDentalShown() {
			//In order for the "Automatically show the touch keyboard in windowed apps when there's no keyboard attached to your device" Windows setting
			//to work we have to invoke the following line.  Surrounded in a try catch because the user can simply put the OS into tablet mode.
			//Affects WPF RichTextBoxes accross the entire program.
			ODException.SwallowAnyException(() => {
				System.Windows.Automation.AutomationElement.FromHandle(this.Handle);//Just invoking this method wakes up something deep within Windows...
			});
			//Flag the userod cache as NOT allowed to cache any items for security purposes.
			Userods.SetIsCacheAllowed(false);
			//TopMost=true;
			//Application.DoEvents();
			//TopMost=false;
			//Activate();
			//This will be increased to 4 hours below but only after the convert script has succeeded.
			DataConnection.ConnectionRetryTimeoutSeconds=(int)TimeSpan.FromMinutes(1).TotalSeconds;
			//Have the auto retry timeout monitor throw an exception after the timeout specified above.
			//If left false, then the application would fall into an infinite wait and we can't afford to have that happen at this point.
			//This will get set to false down below after we register for the DataConnectionLost event which will display the Data Connection Lost window.
			DataConnection.DoThrowOnAutoRetryTimeout=true;
			AllNeutral();
			string odUser=CommandLineArgs.UserName??"";
			string odPassHash=CommandLineArgs.PassHash??"";
			bool isSilentUpdate=(CommandLineArgs.IsSilentUpdate??"").Contains("true");
			string odPassword=CommandLineArgs.OdPassword??"";
			string serverName=CommandLineArgs.ServerName??"";
			string databaseName=CommandLineArgs.DatabaseName??"";
			string mySqlUser=CommandLineArgs.MySqlUser??"";
			string mySqlPassword=CommandLineArgs.MySqlPassword??"";
			string mySqlPassHash=CommandLineArgs.MySqlPassHash??"";
			bool useDynamicMode=(CommandLineArgs.UseDynamicMode??"").Contains("true");
			string domainUser=CommandLineArgs.DomainUser??"";
			string webServiceUri=CommandLineArgs.WebServiceUri??"";
			YN webServiceIsEcw=YN.Unknown;
			if(!CommandLineArgs.WebServiceIsEcw.IsNullOrEmpty() && CommandLineArgs.WebServiceIsEcw=="true") {
				webServiceIsEcw=YN.Yes;
			}
			else {
				webServiceIsEcw=YN.No;
			}
			YN noShow=YN.Unknown;
			if(webServiceUri!="") {//a web service was specified
				if(odUser!="" && odPassword!="") {//and both a username and password were specified
					noShow=YN.Yes;
				}
			}
			else if(databaseName!="") {
				noShow=YN.Yes;
			}
			//Users that want to silently update MUST pass in the following command line args.
			if(isSilentUpdate && (odUser.Trim()==""
					|| (odPassword.Trim()=="" && odPassHash.Trim()=="")
					|| serverName.Trim()==""
					|| databaseName.Trim()==""
					|| mySqlUser.Trim()==""
					|| (mySqlPassword.Trim()=="" && mySqlPassHash.Trim()==""))) {
				ExitCode=104;//Required command line arguments have not been set for silent updating
				Environment.Exit(ExitCode);
				return;
			}
			Version versionOd=Assembly.GetAssembly(typeof(FormOpenDental)).GetName().Version;
			Version versionObBus=Assembly.GetAssembly(typeof(Db)).GetName().Version;
			if(versionOd!=versionObBus) {
				if(isSilentUpdate) {
					ExitCode=105;//File versions do not match
				}
				else {//Not a silent update.  Show a warning message.
							//No MsgBox or Lan.g() here, because we don't want to access the database if there is a version conflict.
					MessageBox.Show("Mismatched program file versions. Please run the Open Dental setup file again on this computer.");
				}
				Environment.Exit(ExitCode);
				return;
			}
			ChooseDatabaseInfo chooseDatabaseInfo=new ChooseDatabaseInfo();
			try {
				chooseDatabaseInfo=ChooseDatabaseInfo.GetChooseDatabaseInfoFromConfig(webServiceUri,webServiceIsEcw,odUser,serverName,databaseName
					,mySqlUser,mySqlPassword,mySqlPassHash,noShow,odPassword,useDynamicMode,odPassHash);
			}
			catch(ODException ode) {
				if(isSilentUpdate) {
					//Technically the only way GetChooseDatabaseInfoFromConfig() can throw an exception when silent updating is if DatabaseName wasn't set.
					ExitCode=104;//Required command line arguments have not been set for silent updating
				}
				else {
					MessageBox.Show(ode.Message);
				}
				Environment.Exit(ExitCode);
				return;
			}
			//Hook up MT connection lost event. Nothing prior to this point fires LostConnection events.
			MiddleTierConnectionEvent.Fired+=MiddleTierConnection_ConnectionLost;
			FormSplash formSplash=new FormSplash();
			using FormChooseDatabase FormCD=new FormChooseDatabase(chooseDatabaseInfo);
			ChooseDatabaseInfo modelFromForm=null;
			while(true) {//Most users will loop through once.  If user tries to connect to a db with replication failure, they will loop through again.
				if(chooseDatabaseInfo.NoShow==YN.Yes) {
					try {
						Logger.LogToPath("CentralConnections.TryToConnect",LogPath.Startup,LogPhase.Start);
						CentralConnections.TryToConnect(chooseDatabaseInfo.CentralConnectionCur,chooseDatabaseInfo.DatabaseType,
							chooseDatabaseInfo.ConnectionString,noShowOnStartup: (chooseDatabaseInfo.NoShow==YN.Yes),chooseDatabaseInfo.ListAdminCompNames,
							isCommandLineArgs: (CommandLineArgs.ArrayCommandLineArgs.Length!=0),useDynamicMode: chooseDatabaseInfo.UseDynamicMode);
						Logger.LogToPath("CentralConnections.TryToConnect",LogPath.Startup,LogPhase.End);
					}
					catch(Exception) {
						if(isSilentUpdate) {
							ExitCode=106;//Connection to specified database has failed
							Environment.Exit(ExitCode);
							return;
						}
						//The current connection settings are invalid so simply show the choose database window for the user to correct them.
						FormCD.ShowDialog();
						if(FormCD.DialogResult==DialogResult.Cancel) {
							Environment.Exit(ExitCode);
							return;
						}
						modelFromForm=FormCD.ChooseDatabaseInfo_;
					}
				}
				else {
					FormCD.ShowDialog();
					if(FormCD.DialogResult==DialogResult.Cancel) {
						Environment.Exit(ExitCode);
						return;
					}
					modelFromForm=FormCD.ChooseDatabaseInfo_;
				}
				Cursor=Cursors.WaitCursor;
				try {
					Logger.LogToPath("Plugins.LoadAllPlugins",LogPath.Startup,LogPhase.Start);
					Plugins.LoadAllPlugins(this);//moved up from near RefreshLocalData(invalidTypes). New position might cause problems.
				}
				catch(Exception e){
					//Do nothing since this will likely only fail if a column is added to the program table, 
					//due to this method getting called before the update script.  If the plugins do not load, then the simple solution is to restart OD.
					Logger.LogToPath("Plugins.LoadAllPlugins failed: "+e.Message,LogPath.Startup,LogPhase.Unspecified);
				}
				finally {
					Logger.LogToPath("Plugins.LoadAllPlugins",LogPath.Startup,LogPhase.End);
				}
				if(CommandLineArgs.ArrayCommandLineArgs.Length==0 //eCW doesn't load splash screen
					&& !Web.IsWeb) { //don't show splash screen a second time if web is enabled
					formSplash.Show(this);
				}
				//If there is no model and they are trying to run dynamic mode via command line arguments, use the model from the 
				//command line args/config file for use in PrefL to launch the appropriate version.
				if(modelFromForm==null && chooseDatabaseInfo.NoShow==YN.Yes && chooseDatabaseInfo.UseDynamicMode) {
					modelFromForm=chooseDatabaseInfo;
				}
				if(!PrefsStartup(isSilentUpdate,modelFromForm)) {//In Release, refreshes the Pref cache if conversion successful.
					Cursor=Cursors.Default;
					formSplash.Close();
					if(ExitCode==0) {
						//PrefsStartup failed and ExitCode is still 0 which means an unexpected error must have occurred.
						//Set the exit code to 999 which will represent an Unknown Error
						ExitCode=999;
					}
					Environment.Exit(ExitCode);
					return;
				}
				if(isSilentUpdate) {
					//The db was successfully updated so there is nothing else that needs to be done after this point.
					Application.Exit();//Exits with ExitCode=0
					return;
				}
				if(ReplicationServers.Server_id!=0 && ReplicationServers.Server_id==PrefC.GetLong(PrefName.ReplicationFailureAtServer_id)) {
					MsgBox.Show(this,"This database is temporarily unavailable.  Please connect instead to your alternate database at the other location.");
					chooseDatabaseInfo.NoShow=YN.No;//This ensures they will get a choose db window next time through the loop.
					ReplicationServers.Server_id=-1;
					formSplash.Close();
					formSplash=new FormSplash();//force the splash screen to show again.
					continue;
				}
				break;
			}
			if(Programs.UsingEcwTightOrFullMode()) {
				formSplash.Close();
			}
			//Setting the time that we want to wait when the database connection has been lost.
			//We don't want a LostConnection event to fire when updating because of Silent Updating which would fail due to window pop-ups from this event.
			//When the event is triggered a "connection lost" window will display allowing the user to attempt reconnecting to the database
			//and then resume what they were doing.  The purpose of this is to prevent UE's from happening with poor connections or temporary outages.
			DataConnection.ConnectionRetryTimeoutSeconds=(int)TimeSpan.FromHours(4).TotalSeconds;
			DataConnectionEvent.Fired+=DataConnection_ConnectionLost;//Hook up the connection lost event. Nothing prior to this point will have LostConnection events fired.
			DataConnection.DoThrowOnAutoRetryTimeout=false;//Depend on the Connection Lost window after the timeout has been reached.
			DataConnection.CrashedTableTimeoutSeconds=TimeSpan.FromMinutes(1).TotalSeconds;
			DataConnection.DataReaderNullTimeoutSeconds=TimeSpan.FromMinutes(1).TotalSeconds;
			CrashedTableEvent.Fired+=CrashedTable_Detected;
			DataReaderNullEvent.Fired+=DataReaderNull_Detected;
			CredentialsFailedAfterLoginEvent.Fired+=DataConnection_CredentialsFailedAfterLogin;
			Logger.LogToPath("RefreshLocalData Prefs",LogPath.Startup,LogPhase.Unspecified);
			RefreshLocalData(InvalidType.Prefs);//should only refresh preferences so that SignalLastClearedDate preference can be used in ClearOldSignals()
			Signalods.ClearOldSignals();
			//We no longer do this shotgun approach because it can slow the loading time.
			//RefreshLocalData(InvalidType.AllLocal);
			List<InvalidType> invalidTypes=new List<InvalidType>();
			//invalidTypes.Add(InvalidType.Prefs);//Preferences were refreshed above.  The only preference which might be stale is SignalLastClearedDate, but it is not used anywhere after calling ClearOldSignals() above.
			invalidTypes.Add(InvalidType.Defs);
			invalidTypes.Add(InvalidType.Providers);//obviously heavily used
			invalidTypes.Add(InvalidType.Programs);//already done above, but needs to be done explicitly to trigger the PostCleanup 
			invalidTypes.Add(InvalidType.ToolButsAndMounts);//so program buttons will show in all the toolbars
			if(Programs.UsingEcwTightMode()) {
				lightSignalGrid1.Visible=false;
			}
			else {
				invalidTypes.Add(InvalidType.SigMessages);//so when mouse moves over light buttons, it won't crash
			}
			Logger.LogToPath("RefreshLocalData invalidTypes",LogPath.Startup,LogPhase.Unspecified);
			RefreshLocalData(invalidTypes.ToArray());
			Logger.LogToPath("FillSignalButtons",LogPath.Startup,LogPhase.Unspecified);
			FillSignalButtons();
			controlManage.InitializeOnStartup();//so that when a signal is received, it can handle it.
			//Images module.  The other modules are in constructor because they don't need the pref.
			if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)) {
				Logger.LogToPath("ControlImages Old Init",LogPath.Startup,LogPhase.Start);
				controlImages=new ControlImages() { Visible=false };
				controlImages.Dock=DockStyle.Fill;
				LayoutManager.Add(controlImages,splitContainerNoFlickerDashboard.Panel1);
				Logger.LogToPath("ControlImages Old Init",LogPath.Startup,LogPhase.End);
			}
			else {
				Logger.LogToPath("ControlImagesJ Init",LogPath.Startup,LogPhase.Start);
				controlImagesJ=new ControlImagesJ() { Visible=false };
				controlImagesJ.Dock=DockStyle.Fill;
				controlImagesJ.EventKeyDown+=(sender,e)=>FormOpenDental_KeyDown(sender,e);
				controlImagesJ.Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));//not sure why FormOpenDenta.Font wasn't already scaled by this point
				LayoutManager.Add(controlImagesJ,splitContainerNoFlickerDashboard.Panel1);
				Logger.LogToPath("ControlImagesJ Init",LogPath.Startup,LogPhase.End);
			}
			//Lan.Refresh();//automatically skips if current culture is en-US
			//LanguageForeigns.Refresh(CultureInfo.CurrentCulture);//automatically skips if current culture is en-US			
			moduleBar.RefreshButtons();
			Lan.C("MainMenu",menuMain);
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				_menuItemTranslation.Available=false;
			}
			if(!File.Exists("Help.chm")) {
				_menuItemLocalHelpWindows.Available=false;
			}
			if(Environment.OSVersion.Platform==PlatformID.Unix) {//Create A to Z unsupported on Unix for now.
				_menuItemCreateAtoZ.Available=false;
			}
			if(!PrefC.GetBool(PrefName.ProcLockingIsAllowed)) {
				_menuItemProcLockTool.Available=false;
			}
			//If in testing mode, show the testing mode overrides editor
			_menuItemEditTestModeOverrides.Available=Introspection.IsTestingMode;
			//If on a Middle Tier client, show the Payload Monitor
			_menuItemPayloadMonitor.Available=(RemotingClient.RemotingRole==RemotingRole.ClientWeb);
			//Query Monitor does not capture queries from a Middle Tier client, only show Query Monitor menu item when directly connected to the database.
			_menuItemQueryMonitor.Available=(RemotingClient.RemotingRole==RemotingRole.ClientDirect);
			if(Security.IsAuthorized(Permissions.ProcCodeEdit,true) && !PrefC.GetBool(PrefName.ADAdescriptionsReset)) {
				ProcedureCodes.ResetADAdescriptionsAndAbbrs();
				Prefs.UpdateBool(PrefName.ADAdescriptionsReset,true);
			}
			//Spawn a thread so that attempting to start services on this computer does not hinder the loading time of Open Dental.
			//This is placed before login on pupose so it will run even when the user does not login properly.
			BeginODServiceStarterThread();
			formSplash.Close();
			Logger.LogToPath("LogOnOpenDentalUser",LogPath.Startup,LogPhase.Start);
			LogOnOpenDentalUser(odUser,odPassword,domainUser);
			Logger.LogToPath("LogOnOpenDentalUser",LogPath.Startup,LogPhase.End);
			//At this point a user has successfully logged in.  Flag the userod cache as safe to cache data.
			Userods.SetIsCacheAllowed(true);
			//Active Instances Validation for OD Cloud
			if(!ActiveInstanceUnderLimit()) {
				return;
			}
			//If clinics are enabled, we will set the public ClinicNum variable
			//If the user is restricted to a clinic(s), and the computerpref clinic is not one of the user's restricted clinics, the user's clinic will be selected
			//If the user is not restricted, or if the user is restricted but has access to the computerpref clinic, the computerpref clinic will be selected
			//The ClinicNum will determine which view is loaded, either from the computerpref table or from the userodapptview table
			if(PrefC.HasClinicsEnabled && Security.CurUser!=null) {//If block must be run before StartCacheFillForFees() so correct clinic filtration occurs.
				Clinics.LoadClinicNumForUser();
				RefreshMenuClinics();
			}
			BeginODDashboardStarterThread();
			Logger.LogToPath("FillSignalButtons",LogPath.Startup,LogPhase.Unspecified);
			FillSignalButtons();
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				Logger.LogToPath("GetPreferredAtoZpath",LogPath.Startup,LogPhase.Unspecified);
				string prefImagePath=ImageStore.GetPreferredAtoZpath();
				if(prefImagePath==null || !Directory.Exists(prefImagePath)) {//AtoZ folder not found
																																		 //Cache.Refresh(InvalidType.Security);
					using FormPath FormP=new FormPath();
					FormP.IsStartingUp=true;
					FormP.ShowDialog();
					if(FormP.DialogResult!=DialogResult.OK) {
						MsgBox.Show(this,"Invalid A to Z path.  Closing program.");
						Application.Exit();
					}
				}
			}
			IsTreatPlanSortByTooth=PrefC.GetBool(PrefName.TreatPlanSortByTooth); //not a great place for this, but we don't have a better alternative.
			if(userControlTasks1.Visible) {
				Logger.LogToPath("userControlTasks1.InitializeOnStartup",LogPath.Startup,LogPhase.Unspecified);
				userControlTasks1.InitializeOnStartup();
			}
			moduleBar.SelectedIndex=Security.GetModule(0);//for eCW, this fails silently.
			Logger.LogToPath("Programs.UsingEcwTightOrFullMode",LogPath.Startup,LogPhase.Unspecified);
			if(Programs.UsingEcwTightOrFullMode()
				|| (HL7Defs.IsExistingHL7Enabled() && !HL7Defs.GetOneDeepEnabled().ShowAppts)) {
				moduleBar.SelectedModule=EnumModuleType.Chart;
				LayoutControls();
			}
			moduleBar.Invalidate();
			Logger.LogToPath("LayoutToolBar",LogPath.Startup,LogPhase.Unspecified);
			LayoutToolBar();
			Logger.LogToPath("RefreshMenuReports",LogPath.Startup,LogPhase.Unspecified);
			RefreshMenuReports();
			Cursor=Cursors.Default;
			if(moduleBar.SelectedModule==EnumModuleType.None) {
				MsgBox.Show(this,"You do not have permission to use any modules.");
			}
			Logger.LogToPath("Bridges",LogPath.Startup,LogPhase.Unspecified);
			Bridges.Trojan.StartupCheck();
			FormUAppoint.StartThreadIfEnabled();
			Bridges.ICat.StartFileWatcher();
			Bridges.TigerView.StartFileWatcher();
			if(!PrefC.IsODHQ) {
				_menuItemJobManager.Available=false;
				_menuItemWebChatTools.Available=false;
				_menuItemResellers.Available=false;
				_menuItemXChargeReconcile.Available=false;
			}
			if(PrefC.IsODHQ) {
				if(!ODBuild.IsDebug()) {
					if(Process.GetProcessesByName("ProximityOD").Length==0) {
						try {
							Process.Start("ProximityOD.exe");
						}
						catch { }//for example, if working from home.
					}
				}
				FillComboTriageCoordinator();
			}
			Logger.LogToPath("BackupReminder",LogPath.Startup,LogPhase.Unspecified);
			//Users can have strange values in their preference table which can cause unhandled exceptions when the parsed date is manipulated.
			//Manipulate DateTime.Today instead since it should always yield a reasonable DateTime for manipulation.
			bool isBackupReminderNeeded=PrefC.GetDate(PrefName.BackupReminderLastDateRun) < DateTime.Today.AddMonths(-1);//Remind users every month.
			if(!ODBuild.IsTrial() && isBackupReminderNeeded) {
				using FormBackupReminder FormBR=new FormBackupReminder();
				FormBR.ShowDialog();
				if(FormBR.DialogResult==DialogResult.OK) {
					Prefs.UpdateDateT(PrefName.BackupReminderLastDateRun,DateTime.Today);
				}
				else {
					Application.Exit();
					return;
				}
			}
			Logger.LogToPath("FillPatientButton",LogPath.Startup,LogPhase.Unspecified);
			FillPatientButton(null);
			ProcessCommandLine();
			ODException.SwallowAnyException(() => {
				Logger.LogToPath("UpdateHeartBeat",LogPath.Startup,LogPhase.Unspecified);
				Computers.UpdateHeartBeat(ODEnvironment.MachineName,true);
			});
			Text=PatientL.GetMainTitle(Patients.GetPat(PatNumCur),Clinics.ClinicNum);
			Security.DateTimeLastActivity=DateTime.Now;
			//Certificate stores for emails need to be created on all computers since any of the computers are able to potentially send encrypted email.
			//If this fails, prrobably a permission issue creating the stores. Nothing we can do except explain in the manual.
			ODException.SwallowAnyException(() => {
				Logger.LogToPath("CreateCertificateStoresIfNeeded",LogPath.Startup,LogPhase.Unspecified);
				EmailMessages.CreateCertificateStoresIfNeeded();
			});
			Patient pat=Patients.GetPat(PatNumCur);
			if(pat!=null && (_strCmdLineShow=="popup" || _strCmdLineShow=="popups") && moduleBar.SelectedModule!=EnumModuleType.None) {
				using FormPopupsForFam FormP=new FormPopupsForFam(_listPopupEvents);
				FormP.PatCur=pat;
				FormP.ShowDialog();
			}
			bool isApptModuleSelected=false;
			if(moduleBar.SelectedModule==EnumModuleType.Appointments) {
				isApptModuleSelected=true;
			}
			if(PatNumCur!=0 && _strCmdLineShow=="apptsforpatient" && isApptModuleSelected) {
				controlAppt.DisplayOtherDlg(false);
			}
			if(_strCmdLineShow=="searchpatient") {
				using FormPatientSelect formPatientSelect=new FormPatientSelect();
				formPatientSelect.ShowDialog();
				if(formPatientSelect.DialogResult==DialogResult.OK) {
					PatNumCur=formPatientSelect.SelectedPatNum;
					pat=Patients.GetPat(PatNumCur);
					if(controlChart.Visible) {
						controlChart.ModuleSelectedErx(PatNumCur);
					}
					else {
						RefreshCurrentModule();
					}
					FillPatientButton(pat);
				}
			}
			if(!PrefC.IsODHQ) {
				_menuItemDefaultCCProcs.Available=false;
			}
			Logger.LogToPath("LanguageAndRegion",LogPath.Startup,LogPhase.Unspecified);
			if(PrefC.GetString(PrefName.LanguageAndRegion)!=CultureInfo.CurrentCulture.Name && !ComputerPrefs.LocalComputer.NoShowLanguage) {
				StringBuilder strBuild=new StringBuilder();
				strBuild.AppendLine(Lans.g(this,"Warning, having mismatched language settings between the workstation and server may cause the program to behave in unexpected ways"));
				strBuild.AppendLine(Lans.g(this,"Database setting: ")+ PrefC.GetString(PrefName.LanguageAndRegion)??"");
				strBuild.AppendLine(Lans.g(this,"Computer setting: ")+CultureInfo.CurrentCulture.Name);
				strBuild.AppendLine(Lans.g(this,"Would you like to view the language and region setup window?"));
				if(MessageBox.Show(strBuild.ToString(),"",MessageBoxButtons.YesNo)==DialogResult.Yes){
					using FormLanguageAndRegion FormLAR=new FormLanguageAndRegion();
					FormLAR.ShowDialog();
				}
			}
			Logger.LogToPath("CurrencyDecimalDigits",LogPath.Startup,LogPhase.Unspecified);
			if(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits != 2 //We want our users to have their currency decimal setting set to 2.
				&& !ComputerPrefs.LocalComputer.NoShowDecimal) {
				using FormDecimalSettings FormDS=new FormDecimalSettings();
				FormDS.ShowDialog();
			}
			Logger.LogToPath("DirectXFormat",LogPath.Startup,LogPhase.Unspecified);
			//Choose a default DirectX format when no DirectX format has been specified and running in DirectX tooth chart mode.
			if(ComputerPrefs.LocalComputer.GraphicsSimple==DrawingMode.DirectX && ComputerPrefs.LocalComputer.DirectXFormat=="") {
				try {
					ComputerPrefs.LocalComputer.DirectXFormat=FormGraphics.GetPreferredDirectXFormat(this);
					if(ComputerPrefs.LocalComputer.DirectXFormat=="invalid") {
						//No valid local DirectX format could be found.
						ComputerPrefs.LocalComputer.GraphicsSimple=DrawingMode.Simple2D;
					}
					ComputerPrefs.Update(ComputerPrefs.LocalComputer);
					//Reinitialize the tooth chart because the graphics mode was probably changed which should change the tooth chart appearence.
					controlChart.InitializeOnStartup();
				}
				catch(Exception) {
					//The tooth chart will default to Simple2D mode if the above code fails for any reason.  This will at least get the user into the program.
				}
			}
			//Only show enterprise setup if it is enabled
			_menuItemEnterprise.Available=PrefC.GetBool(PrefName.ShowFeatureEnterprise);
			_menuItemReactivation.Available=PrefC.GetBool(PrefName.ShowFeatureReactivations);
			Logger.LogToPath("UpdateLocalComputerOS",LogPath.Startup,LogPhase.Unspecified);
			ComputerPrefs.UpdateLocalComputerOS();
			WikiPages.NavPageDelegate=S_WikiLoadPage;
			Patients.NavPatDelegate=S_Contr_PatientSelected;
			Tasks.NavTaskDelegate=S_TaskNumLoad;
			Logger.LogToPath("SignalLastRefreshed",LogPath.Startup,LogPhase.Unspecified);
			//We are about to start signal processing for the first time so set the initial refresh timestamp.
			Signalods.SignalLastRefreshed=MiscData.GetNowDateTime();
			Signalods.ApptSignalLastRefreshed=Signalods.SignalLastRefreshed;
			SetTimersAndThreads(true);//Safe to start timers since this method call is on the main thread.
			if(ODBuild.IsWeb()) {
				_menuItemCreateAtoZ.Available=false;
				_menuItemServiceManager.Available=false;
				_menuItemReplication.Available=false;
				_menuItemPrinter.Available=false;
				//If the office needs to reset their office passowrd, we will prompt them until they change it.
				if(PrefC.GetEnum<YN>(PrefName.CloudPasswordNeedsReset)!=YN.No) {
					string message="You must reset the office password. ";
					if(Security.IsAuthorized(Permissions.SecurityAdmin)) {
						if(MsgBox.Show(this,MsgBoxButtons.YesNo,message+"Do you want to open the Change Office Password window?")) {
							using FormChangeCloudPassword formChangeCloudPassword=new FormChangeCloudPassword();
							formChangeCloudPassword.ShowDialog();
						}
					}
					else {
						MsgBox.Show(this,message+"This must be done by a SecurityAdmin user.");
					}
				}
			}
			Logger.LogToPath("MainBorderColors",LogPath.Startup,LogPhase.Unspecified);
			List<Def> listDefsMisColors=Defs.GetDefsForCategory(DefCat.MiscColors);
			SetBorderColor(DefCatMiscColors.MainBorder,listDefsMisColors[(int)DefCatMiscColors.MainBorder].ItemColor);
			SetBorderColor(DefCatMiscColors.MainBorderOutline,listDefsMisColors[(int)DefCatMiscColors.MainBorderOutline].ItemColor);
			SetBorderColor(DefCatMiscColors.MainBorderText,listDefsMisColors[(int)DefCatMiscColors.MainBorderText].ItemColor);
			Logger.LogToPath("LayoutManager",LogPath.Startup,LogPhase.Unspecified);
			LayoutManager.LayoutFormBoundsAndFonts(this);
			LayoutControls();
			if(CommandLineArgs.ArrayCommandLineArgs.Contains("--runLoadSimulation")) {
				StartLoadSimulation();
			}
			Logger.LogToPath("LicenseAgreement",LogPath.Startup,LogPhase.Unspecified);
			//Determine if customer needs to accept the License Agreement
			if(!PrefC.GetBool(PrefName.LicenseAgreementAccepted)) { 
				bool needsSignature=UpdateHistories.IsLicenseAgreementNeeded();
				if(needsSignature) {
					PromptForLicenseSignature(); //Make web call
				}
				else {	//License Agreement already signed in local database, just need to send to HQ
					BeginLicenseAgreementSignatureThread();
				}
			}
			Plugins.HookAddCode(this,"FormOpenDental.Load_end");
		}

		/// <summary>Helper method that checks if the number of Active Instances (OD Cloud) exceed the allowed amount. Exits the application on false.
		/// Calling method should return in that circumstance.</summary>
		private bool ActiveInstanceUnderLimit() {
			if(!ODBuild.IsWeb()
				|| ActiveInstances.GetCountCloudActiveInstances(ActiveInstances.CurrentActiveInstance?.ActiveInstanceNum??0)<PrefC.GetInt(PrefName.CloudSessionLimit)) 
			{
				return true;
			}
			string msg="You have exceeded the allowed number of concurrent Open Dental Cloud sessions.\r\n"
			+"Would you like to view Cloud Management?";
			if(MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,msg))) {
				using FormCloudManagement formCloudManagement=new FormCloudManagement();
				formCloudManagement.ShowDialog();
				//If the user still exceeds the allowed sessions, close this session
				if(ActiveInstances.GetCountCloudActiveInstances(ActiveInstances.CurrentActiveInstance?.ActiveInstanceNum??0)>=PrefC.GetInt(PrefName.CloudSessionLimit)) {
					MsgBox.Show(this,"Your active sessions still exceed the allowed number of concurrent Open Dental Cloud sessions. Closing this session.");
					Application.Exit();
					return false;
				}
			}
			else {
				Application.Exit();
				return false;
			}
			return true;
		}

		protected override void OnResize(EventArgs e){
			//FormOpenDental_Resize wouldn't work because base needs to come first
			base.OnResize(e);
			LayoutControls();
			if(Plugins.PluginsAreLoaded) {
				Plugins.HookAddCode(this,"FormOpenDental.FormOpenDental_Resize_end");
			}
		}

		protected override void OnResizeEnd(EventArgs e){
			base.OnResizeEnd(e);
			//todo: Need to differentiate between normal resizing handled by LayoutManager
			//vs layouts required by dpi change, startup, module change, etc.
			//Also, we have to be more careful about laying out visible module and also layout out newly selected module.
			//Using "Update" isn't really the solution because that's a MS layout rather than a LayoutManager or manual layout.
			//This all works right now, but it's a little rough.
			LayoutControls();//for dpi change when dragging
			if(controlAppt.Visible){
				controlAppt.LayoutControls();
			}
			if(controlFamily.Visible){
				controlFamily.Update();
			}
			if(controlFamilyEcw.Visible) {
				controlFamilyEcw.Update();
			}
			if(controlAccount.Visible){
				controlAccount.LayoutPanelsAndRefreshMainGrids();
			}
			if(controlTreat.Visible){
				controlTreat.Update();
			}
			if(controlChart.Visible){
				controlChart.LayoutControls();
			}
			if(controlImages!=null && controlImages.Visible){
				controlImages.Update();
			}
			if(controlImagesJ!=null && controlImagesJ.Visible){
				controlImagesJ.LayoutControls();
			}
			if(controlManage.Visible){
				controlManage.Update();
			}
		}

		/// <summary>Sends function key presses to the appointment module and chart module</summary>
		private void FormOpenDental_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			//This suppresses the base windows functionality for giving focus to the main menu on F10. See Job 8289
			if(e.KeyCode==Keys.F10) {
				e.SuppressKeyPress=true;
			}
			if(controlAppt.Visible && e.KeyCode>=Keys.F1 && e.KeyCode<=Keys.F12){
				controlAppt.FunctionKeyPress(e.KeyCode);
				return;
			}
			if(controlChart.Visible && e.KeyCode>=Keys.F1 && e.KeyCode<=Keys.F12) {
				controlChart.FunctionKeyPressContrChart(e.KeyCode);
				return;
			}
			//Ctrl-Alt-R is supposed to show referral window, but it doesn't work on some computers.
			//so we're also going to use Ctrl-X to show the referral window.
			if(PatNumCur!=0
				&& (e.Modifiers==(Keys.Alt|Keys.Control) && e.KeyCode==Keys.R)
					|| (e.Modifiers==Keys.Control && e.KeyCode==Keys.X))
			{
				using FormReferralsPatient FormRE=new FormReferralsPatient();
				FormRE.PatNum=PatNumCur;
				FormRE.ShowDialog();
			}
			if(controlImagesJ!=null) {//ensure field is present and not null before triggering key down
				if(controlImagesJ.Visible) {
					controlImagesJ.ControlImagesJ_KeyDown(e.KeyCode);//for video capture trigger
				}
			}
			if(e.Modifiers==Keys.Control && e.KeyCode==Keys.P) {
				toolButPatient_Click();
			}
			Plugins.HookAddCode(this,"FormOpenDental_KeyDown_end",e);
		}
		#endregion Methods - Event Handlers - Form

		#region Methods - Event Handlers - Misc
		private void toolButPatient_Click() {
			bool isClipMatch=false;
			try {
				if(Clipboard.ContainsText() ){
					string txtClip="";
					txtClip=Clipboard.GetText().Trim();
					if(Regex.IsMatch(txtClip,@"^PatNum:\d+$")){//very restrictive specific match for "PatNum:##"
						long patNum=PIn.Long(txtClip.Substring(7));
						Patient patient=Patients.GetLim(patNum);
						if(patient.PatNum!=0){
							Clipboard.Clear();//so if they click it again, the can select a patient
							PatNumCur=patNum;
							isClipMatch=true;
						}
					}
				}
			}
			catch {
				//do nothing
			}
			if(!isClipMatch){
				using FormPatientSelect formPatientSelect=new FormPatientSelect();
				formPatientSelect.ShowDialog();
				if(formPatientSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				PatNumCur=formPatientSelect.SelectedPatNum;
			}
			Patient pat=Patients.GetPat(PatNumCur);
			if(controlChart.Visible) {
				userControlTasks1.RefreshPatTicketsIfNeeded();//This is a special case.  Normally it's called in RefreshCurrentModule()
				controlChart.ModuleSelectedErx(PatNumCur);
			}
			else {
				RefreshCurrentModule();
			}
			FillPatientButton(pat);
			Plugins.HookAddCode(this,"FormOpenDental.OnPatient_Click_end"); //historical name 
		}
		#endregion Methods - Event Handlers - Misc

		#region Methods - Private
		///<summary></summary>
		private void ProcessCommandLine() {
			//if(!Programs.UsingEcwTight() && args.Length==0){
			if(!Programs.UsingEcwTightOrFullMode() && CommandLineArgs.ArrayCommandLineArgs.Length==0){//May have to modify to accept from other sw.
				SetModuleSelected();
				return;
			}
			/*string descript="";
			for(int i=0;i<args.Length;i++) {
				if(i>0) {
					descript+="\r\n";
				}
				descript+=args[i];
			}
			MessageBox.Show(descript);*/
			/*
			PatNum (the integer primary key)
			ChartNumber (alphanumeric)
			SSN (exactly nine digits. If required, we can gracefully handle dashes, but that is not yet implemented)
			UserName
			Password*/
			long patNum=0;
			if(!CommandLineArgs.PatNum.IsNullOrEmpty()) {
				try {
					patNum=Convert.ToInt64(CommandLineArgs.PatNum);
				}
				catch { }
			}
			string chartNumber=CommandLineArgs.ChartNumber??"";
			string ssn=CommandLineArgs.SSN??"";
			string userName=CommandLineArgs.UserName??"";
			string passHash=CommandLineArgs.PassHash??"";
			string aptNum=CommandLineArgs.AptNum??"";
			string ecwConfigPath=CommandLineArgs.EcwConfigPath??"";
			long userId=0;
			if(!CommandLineArgs.UserId.IsNullOrEmpty()) {
				try {
					userId=Convert.ToInt64(CommandLineArgs.UserId);
				}
				catch { }
			}
			string jSessionId=CommandLineArgs.JSESSIONID??"";
			string jSessionIdSSO=CommandLineArgs.JSESSIONIDSSO??"";
			string lbSessionId=CommandLineArgs.LBSESSIONID??"";
			Dictionary<string,EnumModuleType> dictModules=new Dictionary<string,EnumModuleType>();
			dictModules.Add("appt",EnumModuleType.Appointments);
			dictModules.Add("family",EnumModuleType.Family);
			dictModules.Add("account",EnumModuleType.Account);
			dictModules.Add("txplan",EnumModuleType.TreatPlan);
			dictModules.Add("treatplan",EnumModuleType.TreatPlan);
			dictModules.Add("chart",EnumModuleType.Chart);
			dictModules.Add("images",EnumModuleType.Imaging);
			dictModules.Add("manage",EnumModuleType.Manage);
			EnumModuleType startingModule=EnumModuleType.None;
			if(!CommandLineArgs.Module.IsNullOrEmpty() && dictModules.ContainsKey(CommandLineArgs.Module)) {
				startingModule=dictModules[CommandLineArgs.Module];
			}
			_strCmdLineShow=CommandLineArgs.Show??"";
			if(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"IsLBSessionIdExcluded")=="1" //if check box in Program Links is checked
				&& lbSessionId=="" //if lbSessionId not previously set
				&& CommandLineArgs.ArrayCommandLineArgs.Length > 0 //there is at least one argument passed in
				&& !CommandLineArgs.ArrayCommandLineArgs[CommandLineArgs.ArrayCommandLineArgs.Length-1].StartsWith("LBSESSIONID="))//if there is an argument that is the last argument that is not called "LBSESSIONID", then use that argument, including the "name=" part
			{
				//An example of this is command line includes LBSESSIONID= icookie=ECWAPP3ECFH. The space makes icookie a separate parameter. We want to set lbSessionId="icookie=ECWAPP3ECFH". 
				//We are not guaranteed that the parameter is always going to be named icookie, in fact it will be different on each load balancer depending on the setup of the LB.  
				//Therefore, we cannot look for parameter name, but Aislinn from eCW guaranteed that it would be the last parameter every time during our (Cameron and Aislinn's) conversation on 3/5/2014.
				//jsalmon - This is very much a hack but the customer is very large and needs this change ASAP.  Nathan has suggested that we create a ticket with eCW to complain about this and make them fix it.
				lbSessionId=CommandLineArgs.ArrayCommandLineArgs[CommandLineArgs.ArrayCommandLineArgs.Length-1].Trim('"');
			}
			#region eCW bridge
			Bridges.ECW.AptNum=PIn.Long(aptNum);
			Bridges.ECW.EcwConfigPath=ecwConfigPath;
			Bridges.ECW.UserId=userId;
			Bridges.ECW.JSessionId=jSessionId;
			Bridges.ECW.JSessionIdSSO=jSessionIdSSO;
			Bridges.ECW.LBSessionId=lbSessionId;
			#endregion
			#region UserName and PassHash
			//Only consider username and password here when not in Middle Tier mode.
			//If credentials were passed in the command line arguments for Middle Tier, they were already considered in the Choose Database window.
			if(RemotingClient.RemotingRole==RemotingRole.ClientDirect) {
				//Users are allowed to use eCW tight integration without command line.  They can manually launch Open Dental.
				//We always want to trigger login window for eCW tight, even if no username was passed in.
				if((Programs.UsingEcwTightOrFullMode() && Security.CurUser==null)
					//Or if a username was passed in and it's different from the current user
					|| (userName!="" && (Security.CurUser==null || Security.CurUser.UserName != userName)))
				{
					//Use the username and passhash that was passed in to determine which user to log in
					//log out------------------------------------
					_moduleTypeLast=moduleBar.SelectedModule;
					moduleBar.SelectedModule=EnumModuleType.None;
					moduleBar.Invalidate();
					UnselectActive();
					AllNeutral();
					Userod user=Userods.GetUserByName(userName,true);
					if(user==null) {
						if(Programs.UsingEcwTightOrFullMode() && userName!="") {
							user=new Userod();
							user.UserName=userName;
							user.LoginDetails=Authentication.GenerateLoginDetailsMD5(passHash,true);
							//This can fail if duplicate username because of capitalization differences.
							Userods.Insert(user,new List<long> { PIn.Long(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"DefaultUserGroup")) });
							DataValid.SetInvalid(InvalidType.Security);
						}
						else {//not using eCW in tight integration mode
							//So present logon screen
							ShowLogOn();
							user=Security.CurUser.Copy();
						}
					}
					//Can't use Userods.CheckPassword, because we only have the hashed password.
					if(passHash!=user.PasswordHash || !Programs.UsingEcwTightOrFullMode()) {//password not accepted or not using eCW
						//So present logon screen
						ShowLogOn();
					}
					else {//password accepted and using eCW tight.
						//this part usually happens in the logon window
						Security.CurUser=user.Copy();
						SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "+Lan.g(this,"has logged on via command line."));
					}
					moduleBar.SelectedIndex=Security.GetModule(moduleBar.IndexOf(_moduleTypeLast));
					moduleBar.Invalidate();
					SetModuleSelected();
					Patient pat=Patients.GetPat(PatNumCur);//pat could be null
					Text=PatientL.GetMainTitle(pat,Clinics.ClinicNum);//handles pat==null by not displaying pat name in title bar
					if(userControlTasks1.Visible) {
						userControlTasks1.InitializeOnStartup();
					}
					if(moduleBar.SelectedModule==EnumModuleType.None) {
						MsgBox.Show(this,"You do not have permission to use any modules.");
					}
				}
			}
			#endregion
			#region Module
			if(startingModule!=EnumModuleType.None && moduleBar.IndexOf(startingModule)==Security.GetModule(moduleBar.IndexOf(startingModule))) {
				UnselectActive();
				AllNeutral();//Sets all controls to false.  Needed to set the new module as selected.
				moduleBar.SelectedModule=startingModule;
				moduleBar.Invalidate();
			}
			SetModuleSelected();
			#endregion
			#region PatNum
			if(patNum!=0) {
				Patient pat=Patients.GetPat(patNum);
				if(pat==null) {
					PatNumCur=0;
					RefreshCurrentModule();
					FillPatientButton(null);
				}
				else {
					PatNumCur=patNum;
					RefreshCurrentModule();
					FillPatientButton(pat);
				}
			}
			#endregion
			#region ChartNumber
			else if(chartNumber!="") {
				Patient pat=Patients.GetPatByChartNumber(chartNumber);
				if(pat==null) {
					//todo: decide action
					PatNumCur=0;
					RefreshCurrentModule();
					FillPatientButton(null);
				}
				else {
					PatNumCur=pat.PatNum;
					RefreshCurrentModule();
					FillPatientButton(pat);
				}
			}
			#endregion
			#region SSN
			else if(ssn!="") {
				Patient pat=Patients.GetPatBySSN(ssn);
				if(pat==null) {
					//todo: decide action
					PatNumCur=0;
					RefreshCurrentModule();
					FillPatientButton(null);
				}
				else {
					PatNumCur=pat.PatNum;
					RefreshCurrentModule();
					FillPatientButton(pat);
				}
			}
			#endregion
			else {
				FillPatientButton(null);
			}
		}

		protected override string GetHelpOverride() {
			switch(moduleBar.SelectedModule){
				case EnumModuleType.Appointments:
					return nameof(ControlAppt);
				case EnumModuleType.Family:
					return nameof(ControlFamily);
				case EnumModuleType.Account:
					return nameof(ControlAccount);
				case EnumModuleType.TreatPlan:
					return nameof(ControlTreat);
				case EnumModuleType.Chart:
					return nameof(ControlChart);
				case EnumModuleType.Imaging:
					return nameof(ControlImages);
				case EnumModuleType.Manage:
					return nameof(ControlManage);
				default:
					return "";
			}
		}

		private bool PrefsStartup() {
			//Default usePreviousVersions to false as this is only called after Open Dental is already fully functional. No versions will have changed
			//by the time this is called.
			return PrefsStartup(false,null);
		}

		///<summary>Returns false if it can't complete a conversion, find datapath, or validate registration key. A silent update will have no UI elements appear. model stores all the info used within the choose database window. Stores all information entered within the window.</summary>
		private bool PrefsStartup(bool isSilentUpdate,ChooseDatabaseInfo chooseDatabaseInfo){
			try {
				Cache.Refresh(InvalidType.Prefs);
			}
			catch(Exception ex) {
				if(isSilentUpdate) {
					ExitCode=100;//Database could not be accessed for cache refresh
					Environment.Exit(ExitCode);
					return false;
				}
				MessageBox.Show(ex.Message);
				return false;//shuts program down.
			}
			if(!PrefL.CheckMySqlVersion(isSilentUpdate)){
				return false;
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				try {
					MiscData.SetSqlMode();
				}
				catch {
					if(isSilentUpdate) {
						ExitCode=111;//Global SQL mode could not be set
						Environment.Exit(ExitCode);
						return false;
					}
					MessageBox.Show("Unable to set global sql mode.  User probably does not have enough permission.");
					return false;
				}
				string updateComputerName=PrefC.GetStringSilent(PrefName.UpdateInProgressOnComputerName);
				//The update could have been initiated from an older version of the program (prior to v21.3) via a Remote Desktop (RDP) session.
				//Starting in v21.3, ODEnvironment.MachineName returns what is commonly referred to as the client name instead of the machine name.
				//Allow access to the program when either of these names match for backwards compatibility purposes.
				string clientNameUpper=ODEnvironment.MachineName.ToUpper();
				string machineNameUpper=Environment.MachineName.ToUpper();
				if(updateComputerName!="" && !ListTools.In(updateComputerName.ToUpper(),clientNameUpper,machineNameUpper)) {
					if(isSilentUpdate) {
						ExitCode=120;//Computer trying to access DB during update
						Environment.Exit(ExitCode);
						return false;
					}
					using FormUpdateInProgress formUIP=new FormUpdateInProgress(updateComputerName);
					DialogResult result=formUIP.ShowDialog();
					if(result!=DialogResult.OK) {
						return false;//Either the user canceled out of the window or clicked the override button which 
					}
				}
			}
			//if RemotingRole.ClientWeb, version will have already been checked at login, so no danger here.
			//ClientWeb version can be older than this version, but that will be caught in a moment.
			string dbVersionOld = PrefC.GetString(PrefName.DataBaseVersion);
			if(isSilentUpdate) {
				if(!PrefL.ConvertDB(true,Application.ProductVersion,this,false)) {//refreshes Prefs if converted successfully.
					if(ExitCode==0) {//Unknown error occurred
						ExitCode=200;//Convert Database has failed during execution (Unknown Error)
					}
					Environment.Exit(ExitCode);
					return false;
				}
			}
			else {
				if(!PrefL.ConvertDB(this,chooseDatabaseInfo?.UseDynamicMode??false)) {//refreshes Prefs if converted successfully.
					return false;
				}
			}
			if(!isSilentUpdate) {
				PrefL.MySqlVersion55Remind();
			}
			if(!PrefL.CheckProgramVersion(this,isSilentUpdate,chooseDatabaseInfo,CommandLineArgs)){
				return false;
			}
			if(dbVersionOld!=PrefC.GetString(PrefName.DataBaseVersion) && !isSilentUpdate) {
				MsgBox.Show("Database update successful");
			}
			ComputerPref _ =ComputerPrefs.LocalComputer;//just to trigger refresh. See notes in LayoutManagerForms.ScaleMy().
			if(!FormRegistrationKey.ValidateKey(PrefC.GetString(PrefName.RegistrationKey))){
				if(isSilentUpdate) {
					ExitCode=311;//Registration Key could not be validated
					Environment.Exit(ExitCode);
					return false;
				}
				using FormRegistrationKey FormR=new FormRegistrationKey();
				FormR.ShowDialog();
				if(FormR.DialogResult!=DialogResult.OK){
					Environment.Exit(ExitCode);
					return false;
				}
				Cache.Refresh(InvalidType.Prefs);
			}
			//This must be done at startup in case the user does not perform any action to save something to temp file.
			//This will cause slowdown, but only for the first week.
			if(DateTime.Today<PrefC.GetDate(PrefName.TempFolderDateFirstCleaned).AddDays(7)) {
				PrefC.GetTempFolderPath();//We don't care about the return value.  Just trying to trigger the one-time cleanup and create the temp/opendental directory.
			}
			Lans.RefreshCache();//automatically skips if current culture is en-US
			LanguageForeigns.RefreshCache();//automatically skips if current culture is en-US
			//menuItemMergeDatabases.Visible=PrefC.GetBool(PrefName.RandomPrimaryKeys");
			return true;
		}

		///<summary>Refreshes certain rarely used data from database.  Must supply the types of data to refresh as flags.  Also performs a few other tasks that must be done when local data is changed.</summary>
		private void RefreshLocalData(params InvalidType[] arrayITypes) {
			RefreshLocalData(true,arrayITypes);
		}
		
		///<summary>Refreshes certain rarely used data from database.  Must supply the types of data to refresh as flags.  Also performs a few other tasks that must be done when local data is changed.</summary>
		private void RefreshLocalData(bool doRefreshServerCache,params InvalidType[] arrayITypes) {
			if(arrayITypes==null || arrayITypes.Length==0) {
				return;//Just in case.
			}
			Cache.Refresh(doRefreshServerCache,arrayITypes);
			//This method manipulates UI controls.  Calling from a thread throws a cross-threaded exception.
			this.InvokeIfRequired(() => RefreshLocalDataPostCleanup(arrayITypes));
		}

		///<summary>Performs a few tasks that must be done when local data is changed.</summary>
		private void RefreshLocalDataPostCleanup(params InvalidType[] arrayITypes) {//This is where the flickering and reset of windows happens
			bool isAll=arrayITypes.Contains(InvalidType.AllLocal);
			#region IvalidType.Prefs
			if(arrayITypes.Contains(InvalidType.Prefs) || isAll) {
				if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					_menuItemSites.Available=false;
					_menuItemCounties.Available=false;
					_menuItemPublicHealthScreening.Available=false;
				}
				if(PrefC.GetBool(PrefName.EasyNoClinics)) {
					_menuItemClinics.Available=false;
					_menuItemClinicsMain.Available=false;
				}
				//See other solution @3401 for past commented out code.
				moduleBar.RefreshButtons();
				if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					_menuItemDentalSchoolClass.Available=false;
					_menuItemDentalSchoolCourses.Available=false;
					_menuItemDentalSchools.Available=false;
					_menuItemRequirementsNeeded.Available=false;
					_menuItemStudentRequirements.Available=false;
					_menuItemEvaluations.Available=false;
				}
				if(PrefC.GetBool(PrefName.EasyHideRepeatCharges)) {
					_menuItemRepeatingCharges.Available=false;
				}
				if(!PrefC.HasOnlinePaymentEnabled(out ProgramName progNameForPayments)) {
					_menuItemPendingOnlinePayments.Available=false;
					_menuItemPatPortalTransactions.Available=false;
				}
				if(PrefC.GetString(PrefName.DistributorKey)=="") {
					_menuItemCustManagement.Available=false;
					_menuItemNewCropBilling.Available=false;
				}
				_menuItemFeeSchedGroups.Available=PrefC.GetBool(PrefName.ShowFeeSchedGroups);
				bool isLateChargeFeatureActive=PrefC.GetBool(PrefName.ShowFeatureLateCharges);
				if(isLateChargeFeatureActive) {
					_menuItemLateCharges.Available=true;
					_menuItemFinanceCharges.Available=false;
				}
				else {
					_menuItemLateCharges.Available=false;
					_menuItemFinanceCharges.Available=true;
				}
				CheckCustomReports();
				if(NeedsRedraw("ChartModule")) {
					controlChart.InitializeLocalData();
				}
				if(NeedsRedraw("TaskLists")) {
					if(PrefC.GetBool(PrefName.TaskListAlwaysShowsAtBottom)) {//Refreshing task list here may not be the best course of action.
						//separate if statement to prevent database call if not showing task list at bottom to begin with
						//ComputerPref computerPref = ComputerPrefs.GetForLocalComputer();
						if(ComputerPrefs.LocalComputer.TaskKeepListHidden) {
							userControlTasks1.Visible = false;
						}
						else if(this.WindowState!=FormWindowState.Minimized) {//task list show and window is not minimized.
							userControlTasks1.Visible = true;
							userControlTasks1.InitializeOnStartup();
							if(ComputerPrefs.LocalComputer.TaskDock == 0) {//bottom
								menuItemDockBottom.Checked = true;
								menuItemDockRight.Checked = false;
								panelSplitter.Cursor=Cursors.HSplit;
								panelSplitter.Height=7;
								int splitterNewY=LayoutManager.Scale(540);
								if(ComputerPrefs.LocalComputer.TaskY!=0) {
									splitterNewY=ComputerPrefs.LocalComputer.TaskY;
									if(splitterNewY<300) {
										splitterNewY=300;//keeps it from going too high
									}
									if(splitterNewY>ClientSize.Height-50) {
										splitterNewY=ClientSize.Height-panelSplitter.Height-50;//keeps it from going off the bottom edge
									}
								}
								_pointPanelSplitter96dpi=new PointF(0,LayoutManager.UnscaleF(splitterNewY));
								panelSplitter.Location=new Point(moduleBar.Width,LayoutManager.Scale(_pointPanelSplitter96dpi.Y));
							}
							else {//right
								menuItemDockRight.Checked = true;
								menuItemDockBottom.Checked = false;
								panelSplitter.Cursor=Cursors.VSplit;
								panelSplitter.Width=7;
								int splitterNewX=LayoutManager.Scale(900);
								if(ComputerPrefs.LocalComputer.TaskX!=0) {
									splitterNewX=ComputerPrefs.LocalComputer.TaskX;
									if(splitterNewX<300) {
										splitterNewX=300;//keeps it from going too far to the left
									}
									if(splitterNewX>ClientSize.Width-60) {
										splitterNewX=ClientSize.Width-panelSplitter.Width-60;//keeps it from going off the right edge
									}
								}
								_pointPanelSplitter96dpi=new PointF(LayoutManager.UnscaleF(splitterNewX),0);
								panelSplitter.Location=new Point(LayoutManager.Scale(_pointPanelSplitter96dpi.X),ToolBarMain.Height);
							}
						}
					}
					else {
						userControlTasks1.Visible = false;
					}
				}
				LayoutControls();
			}
			if(arrayITypes.Contains(InvalidType.Sheets) && userControlDashboard.IsInitialized) {
				LayoutControls();//The current dashboard may have changed.
				userControlDashboard.RefreshDashboard();
				ResizeDashboard();
				RefreshMenuDashboards();
			}
			if(arrayITypes.Contains(InvalidType.Security) || isAll) {
				RefreshMenuDashboards();
			}
			#endregion
			#region InvalidType.Signals
			if(arrayITypes.Contains(InvalidType.SigMessages) || isAll) {
				FillSignalButtons();
			}
			#endregion
			#region InvalidType.Programs
			if(arrayITypes.Contains(InvalidType.Programs) || isAll) {
				if(Programs.GetCur(ProgramName.PT).Enabled && Programs.IsEnabledByHq(ProgramName.PT,out _)) {
					Bridges.PaperlessTechnology.InitializeFileWatcher();
				}
				_menuItemCareCreditTransactions.Available=Programs.GetCur(ProgramName.CareCredit).Enabled;
			}
			#endregion
			#region InvalidType.Programs OR InvalidType.Prefs
			if(arrayITypes.Contains(InvalidType.Programs) || arrayITypes.Contains(InvalidType.Prefs) || isAll) {
				if(PrefC.GetBool(PrefName.EasyBasicModules)) {
					moduleBar.SetVisible(EnumModuleType.TreatPlan,false);
					moduleBar.SetVisible(EnumModuleType.Imaging,false);
					moduleBar.SetVisible(EnumModuleType.Manage,false);
				}
				else {
					moduleBar.SetVisible(EnumModuleType.TreatPlan,true);
					moduleBar.SetVisible(EnumModuleType.Imaging,true);
					moduleBar.SetVisible(EnumModuleType.Manage,true);
				}
				if(Programs.UsingEcwTightOrFullMode()) {//has nothing to do with HL7
					if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"ShowImagesModule")=="1") {
						moduleBar.SetVisible(EnumModuleType.Imaging,true);
					}
					else {
						moduleBar.SetVisible(EnumModuleType.Imaging,false);
					}
				}
				if(Programs.UsingEcwTightMode()) {//has nothing to do with HL7
					moduleBar.SetVisible(EnumModuleType.Manage,false);
				}
				if(Programs.UsingEcwTightOrFullMode()) {//old eCW interfaces
					if(Programs.UsingEcwTightMode()) {
						moduleBar.SetVisible(EnumModuleType.Appointments,false);
						moduleBar.SetVisible(EnumModuleType.Account,false);
					}
					else if(Programs.UsingEcwFullMode()) {
						//We might create a special Appt module for eCW full users so they can access Recall.
						moduleBar.SetVisible(EnumModuleType.Appointments,false);
					}
				}
				else if(HL7Defs.IsExistingHL7Enabled()) {//There may be a def enabled as well as the old program link enabled. In this case, do not look at the def for whether or not to show the appt and account modules, instead go by the eCW interface enabled.
					HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
					moduleBar.SetVisible(EnumModuleType.Appointments,hl7Def.ShowAppts);
					moduleBar.SetVisible(EnumModuleType.Account,hl7Def.ShowAccount);
				}
				else {//no def and not using eCW tight or full program link
					moduleBar.SetVisible(EnumModuleType.Appointments,true);
					moduleBar.SetVisible(EnumModuleType.Account,true);
				}
				if(Programs.UsingOrion) {
					moduleBar.SetVisible(EnumModuleType.Appointments,false);
					moduleBar.SetVisible(EnumModuleType.Account,false);
					moduleBar.SetVisible(EnumModuleType.TreatPlan,false);
				}
				moduleBar.Invalidate();
			}
			#endregion
			#region InvalidType.ToolButsAndMounts
			if(arrayITypes.Contains(InvalidType.ToolButsAndMounts) || isAll) {
				controlAccount.LayoutToolBar();
				controlAppt.LayoutToolBar();
				if(controlChart.Visible) {
					//When the invalidated (running DBM) if we just layout the tool bar the buttons would be enabled, need to consider if no patient is selected.
					//The following line calls LayoutToolBar() and then does the toolbar enable/disable logic.
					controlChart.RefreshModuleScreen(false);//false because module is already selected.
				}
				else {
					controlChart.LayoutToolBar();
				}
				if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
					if(controlImages!=null){//can be null on startup
						controlImages.LayoutToolBar();
					}
				}
				else{
					if(controlImagesJ!=null){
						controlImagesJ.LayoutToolBars();
					}
				}
				controlFamily.LayoutToolBar();
				LayoutToolBar();//Ensures the main toolbar refreshes with the rest.
			}
			#endregion
			#region InvalidType.Views
			if(arrayITypes.Contains(InvalidType.Views) || isAll) {
				controlAppt.FillViews();
			}
			#endregion
			#region HQ Only
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				if(arrayITypes.Contains(InvalidType.Employees) || arrayITypes.Contains(InvalidType.Sites) || isAll) { 
					FillComboTriageCoordinator();
				}
			}
			#endregion
			//TODO: If there are still issues with TP refreshing, include TP prefs in needsRedraw()
			controlTreat.InitializeLocalData();//easier to leave this here for now than to split it.
			_dictionaryChartPrefsCache.Clear();
			_dictionaryTaskListPrefsCache.Clear();
			//Chart Drawing Prefs
			_dictionaryChartPrefsCache.Add(PrefName.DistributorKey.ToString(),PrefC.GetBool(PrefName.DistributorKey));
			_dictionaryChartPrefsCache.Add(PrefName.UseInternationalToothNumbers.ToString(),PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			_dictionaryChartPrefsCache.Add("GraphicsUseHardware",ComputerPrefs.LocalComputer.GraphicsUseHardware);
			_dictionaryChartPrefsCache.Add("PreferredPixelFormatNum",ComputerPrefs.LocalComputer.PreferredPixelFormatNum);
			_dictionaryChartPrefsCache.Add("GraphicsSimple",ComputerPrefs.LocalComputer.GraphicsSimple);
			_dictionaryChartPrefsCache.Add(PrefName.ShowFeatureEhr.ToString(),PrefC.GetBool(PrefName.ShowFeatureEhr));
			_dictionaryChartPrefsCache.Add("DirectXFormat",ComputerPrefs.LocalComputer.DirectXFormat);
			//Task list drawing prefs
			_dictionaryTaskListPrefsCache.Add("TaskDock",ComputerPrefs.LocalComputer.TaskDock);
			_dictionaryTaskListPrefsCache.Add("TaskY",ComputerPrefs.LocalComputer.TaskY);
			_dictionaryTaskListPrefsCache.Add("TaskX",ComputerPrefs.LocalComputer.TaskX);
			_dictionaryTaskListPrefsCache.Add(PrefName.TaskListAlwaysShowsAtBottom.ToString(),PrefC.GetBool(PrefName.TaskListAlwaysShowsAtBottom));
			_dictionaryTaskListPrefsCache.Add(PrefName.TasksUseRepeating.ToString(),PrefC.GetBool(PrefName.TasksUseRepeating));
			_dictionaryTaskListPrefsCache.Add(PrefName.TasksNewTrackedByUser.ToString(),PrefC.GetBool(PrefName.TasksNewTrackedByUser));
			_dictionaryTaskListPrefsCache.Add(PrefName.TasksShowOpenTickets.ToString(),PrefC.GetBool(PrefName.TasksShowOpenTickets));
			_dictionaryTaskListPrefsCache.Add("TaskKeepListHidden",ComputerPrefs.LocalComputer.TaskKeepListHidden);
			if(Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				_menuItemUserQuery.Available=true;
			}
			else {
				_menuItemUserQuery.Available=false;
			}
			_menuItemQueryFavorites.Available=Security.IsAuthorized(Permissions.UserQuery,true);
		}

		private void FillComboTriageCoordinator() {
			SiteLink siteLink=SiteLinks.GetSiteLinkByGateway();
			if(siteLink is null){
				return;
			}
			ODException.SwallowAnyException(() => {
				comboTriageCoordinator.Items.Clear();
				//The following line will purposefully throw an exception if there is not a valid site link for the current IP octet.
				//We need the triage combo box to look incorrect so that we have a visual indicator to go fix our sites.
				Employee currentTriageCoordinator=Employees.GetEmp(siteLink.EmployeeNum);
				List<Employee> listEmps=Employees.GetDeepCopy(true);
				for(int i=0; i<listEmps.Count;i++) {
					comboTriageCoordinator.Items.Add(Employees.GetNameFL(listEmps[i]),listEmps[i]);
				}
				comboTriageCoordinator.SetSelectedKey<Employee>(currentTriageCoordinator.EmployeeNum, x=> x.EmployeeNum);
			});
		}

		///<summary>Compares preferences related to sections of the program that require redraws and returns true if a redraw is necessary, false otherwise.  If anything goes wrong with checking the status of any preference this method will return true.</summary>
		private bool NeedsRedraw(string section) {
			try {
				switch(section) {
					case "ChartModule":
						if(_dictionaryChartPrefsCache.Count==0
							|| PrefC.GetBool(PrefName.DistributorKey)!=(bool)_dictionaryChartPrefsCache["DistributorKey"]
							|| PrefC.GetInt(PrefName.UseInternationalToothNumbers)!=(int)_dictionaryChartPrefsCache["UseInternationalToothNumbers"]
							|| ComputerPrefs.LocalComputer.GraphicsUseHardware!=(bool)_dictionaryChartPrefsCache["GraphicsUseHardware"]
							|| ComputerPrefs.LocalComputer.PreferredPixelFormatNum!=(int)_dictionaryChartPrefsCache["PreferredPixelFormatNum"]
							|| ComputerPrefs.LocalComputer.GraphicsSimple!=(DrawingMode)_dictionaryChartPrefsCache["GraphicsSimple"]
							|| PrefC.GetBool(PrefName.ShowFeatureEhr)!=(bool)_dictionaryChartPrefsCache["ShowFeatureEhr"]
							|| ComputerPrefs.LocalComputer.DirectXFormat!=(string)_dictionaryChartPrefsCache["DirectXFormat"]) 
						{
							return true;
						}
						break;
					case "TaskLists":
						if(_dictionaryTaskListPrefsCache.Count==0
							|| ComputerPrefs.LocalComputer.TaskDock!=(int)_dictionaryTaskListPrefsCache["TaskDock"] //Checking for task list redrawing
							|| ComputerPrefs.LocalComputer.TaskY!=(int)_dictionaryTaskListPrefsCache["TaskY"]
							|| ComputerPrefs.LocalComputer.TaskX!=(int)_dictionaryTaskListPrefsCache["TaskX"]
							|| PrefC.GetBool(PrefName.TaskListAlwaysShowsAtBottom)!=(bool)_dictionaryTaskListPrefsCache["TaskListAlwaysShowsAtBottom"]
							|| PrefC.GetBool(PrefName.TasksUseRepeating)!=(bool)_dictionaryTaskListPrefsCache["TasksUseRepeating"]
							|| PrefC.GetBool(PrefName.TasksNewTrackedByUser)!=(bool)_dictionaryTaskListPrefsCache["TasksNewTrackedByUser"]
							|| PrefC.GetBool(PrefName.TasksShowOpenTickets)!=(bool)_dictionaryTaskListPrefsCache["TasksShowOpenTickets"]
							|| ComputerPrefs.LocalComputer.TaskKeepListHidden!=(bool)_dictionaryTaskListPrefsCache["TaskKeepListHidden"]) 
						{
							return true;
						}
						break;
					//case "TreatmentPlan":
					//	//If needed implement this section
					//	break;
				}//end switch
				return false;
			}
			catch {
				return true;//Should never happen.  Would most likely be caused by invalid preferences within the database.
			}
		}

		///<summary>Sets up the custom reports list in the main menu when certain requirements are met, or disables the custom reports menu item when those same conditions are not met. This function is called during initialization, and on the event that the A to Z folder usage has changed.</summary>
		private void CheckCustomReports(){
			_menuItemCustomReports.DropDown.Items.Clear();
			//Try to load custom reports, but only if using the A to Z folders.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				try {
					string imagePath=ImageStore.GetPreferredAtoZpath();
					string reportFolderName=PrefC.GetString(PrefName.ReportFolderName);
					string reportDir=ODFileUtils.CombinePaths(imagePath,reportFolderName);
					if(Directory.Exists(reportDir)) {
						DirectoryInfo infoDir=new DirectoryInfo(reportDir);
						FileInfo[] filesRdl=infoDir.GetFiles("*.rdl");
						for(int i=0;i<filesRdl.Length;i++) {
							string itemName=Path.GetFileNameWithoutExtension(filesRdl[i].Name);
							_menuItemCustomReports.Add(itemName,new System.EventHandler(this.menuItemRDLReport_Click));
						}
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show(this,"Failed to retrieve custom reports.");
				}
			}
			if(_menuItemCustomReports.DropDown.Items.Count==0) {
				_menuItemCustomReports.Available=false;
			}
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		private void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.ImageList=imageListMain;
			ODToolBarButton button;
			button=new ODToolBarButton(Lan.g(this,"Select Patient"),EnumIcons.PatSelect,"","Patient");
			button.Style=ODToolBarButtonStyle.DropDownButton;
			button.DropDownMenu=menuPatient;
			ToolBarMain.Buttons.Add(button);
			if(!Programs.UsingEcwTightMode()) {//eCW tight only gets Patient Select and Popups toolbar buttons
				button=new ODToolBarButton(Lan.g(this,"Commlog"),EnumIcons.CommLog,Lan.g(this,"New Commlog Entry"),"Commlog");
				if(PrefC.IsODHQ) {
					button.Style=ODToolBarButtonStyle.DropDownButton;
					button.DropDownMenu=menuCommlog;
				}
				ToolBarMain.Buttons.Add(button);
				button=new ODToolBarButton(Lan.g(this,"E-mail"),EnumIcons.Email,Lan.g(this,"Send E-mail"),"Email");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuEmail;
				ToolBarMain.Buttons.Add(button);
				button=new ODToolBarButton(Lan.g(this,"WebMail"),EnumIcons.WebMail,Lan.g(this,"Secure WebMail"),"WebMail");
				button.Enabled=true;//Always enabled.  If the patient does not have an email address, then the user will be blocked from the FormWebMailMessageEdit window.
				ToolBarMain.Buttons.Add(button);
				if(_toolBarButtonText==null) {//If laying out again (after modifying setup), we keep the button to preserve the current notification text.
					_toolBarButtonText=new ODToolBarButton(Lan.g(this,"Text"),EnumIcons.Text,Lan.g(this,"Send Text Message"),"Text");
					_toolBarButtonText.Style=ODToolBarButtonStyle.DropDownButton;
					_toolBarButtonText.DropDownMenu=menuText;
					_toolBarButtonText.Enabled=Programs.IsEnabled(ProgramName.CallFire)||SmsPhones.IsIntegratedTextingEnabled();
					//The Notification text has not been set since startup.  We need an accurate starting count.
					if(SmsPhones.IsIntegratedTextingEnabled()) {
						//Init.  Will query for sms notification signal, or insert one if not found (eConnector hasn't updated this signal since we last cleared
						//old signals).
						SetSmsNotificationText();
					}
				}
				ToolBarMain.Buttons.Add(_toolBarButtonText);
				button=new ODToolBarButton(Lan.g(this,"Letter"),-1,Lan.g(this,"Quick Letter"),"Letter");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuLetter;
				ToolBarMain.Buttons.Add(button);
				button=new ODToolBarButton(Lan.g(this,"Forms"),-1,"","Form");
				//button.Style=ODToolBarButtonStyle.DropDownButton;
				//button.DropDownMenu=menuForm;
				ToolBarMain.Buttons.Add(button);
				if(_toolBarButtonTask==null) {
					_toolBarButtonTask=new ODToolBarButton(Lan.g(this,"Tasks"),3,Lan.g(this,"Open Tasks"),"Tasklist");
					_toolBarButtonTask.Style=ODToolBarButtonStyle.DropDownButton;
					_toolBarButtonTask.DropDownMenu=menuTask;
				}
				ToolBarMain.Buttons.Add(_toolBarButtonTask);
				button=new ODToolBarButton(Lan.g(this,"Label"),4,Lan.g(this,"Print Label"),"Label");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuLabel;
				ToolBarMain.Buttons.Add(button);
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Popups"),-1,Lan.g(this,"Edit popups for this patient"),"Popups"));
			ProgramL.LoadToolbar(ToolBarMain,ToolBarsAvail.MainToolbar);
			Plugins.HookAddCode(this,"FormOpenDental.LayoutToolBar_end");
			ToolBarMain.Invalidate();
		}

		///<summary>Starts a thread that repeatedly gets a random patient and selects each module. Goes until the program is closed.</summary>
		private void StartLoadSimulation() {
			ODThread threadLoad=new ODThread(o => {
				void selectModule(int moduleBarIdx) {
					this.Invoke(() => {
						moduleBar.SelectedIndex=moduleBarIdx;
						SetModuleSelected(true);
						moduleBar_ButtonClicked(moduleBar,new ButtonClicked_EventArgs(null,false));
					});
					Thread.Sleep(1000);
				}
				while(true) {
					Patient pat=Patients.GetRandomPatient();
					this.Invoke(() => {
						PatNumCur=pat.PatNum;
						Text=PatientL.GetMainTitle(pat,Clinics.ClinicNum);
					});
					for(int i=0;i<=6;i++) {
						selectModule(i);
					}
				}
			});
			threadLoad.Name="LoadSimulation";
			threadLoad.Start();
		}

		#endregion Methods - Private
	
		#region Splitters
		private void panelSplitter_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDownOnSplitter=true;
			_pointSplitterOriginalLocation=panelSplitter.Location;
			_pointOriginalMouse=new Point(panelSplitter.Left+e.X,panelSplitter.Top+e.Y);
		}

		private void panelSplitter_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDownOnSplitter){
				return;
			}
			if(menuItemDockBottom.Checked){
				int splitterNewY=_pointSplitterOriginalLocation.Y+(panelSplitter.Top+e.Y)-_pointOriginalMouse.Y;
				if(splitterNewY<300){
					splitterNewY=300;//keeps it from going too high
				}
				if(splitterNewY>ClientSize.Height-50){
					splitterNewY=ClientSize.Height-panelSplitter.Height-50;//keeps it from going off the bottom edge
				}
				//panelSplitter.Top=splitterNewY;
				_pointPanelSplitter96dpi.Y=LayoutManager.UnscaleF(splitterNewY);
			}
			else{//docked right
				int splitterNewX=_pointSplitterOriginalLocation.X+(panelSplitter.Left+e.X)-_pointOriginalMouse.X;
				if(splitterNewX<300) {
					splitterNewX=300;//keeps it from going too far to the left
				}
				if(splitterNewX>ClientSize.Width-60) {
					splitterNewX=ClientSize.Width-panelSplitter.Width-60;//keeps it from going off the right edge
				}
				//panelSplitter.Left=splitterNewX;
				_pointPanelSplitter96dpi.X=LayoutManager.UnscaleF(splitterNewX);
			}
			LayoutControls();
		}

		private void panelSplitter_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDownOnSplitter=false;
			TaskDockSavePos();
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void menuItemDockBottom_Click(object sender,EventArgs e) {
			menuItemDockBottom.Checked=true;
			menuItemDockRight.Checked=false;
			panelSplitter.Cursor=Cursors.HSplit;
			TaskDockSavePos();
			LayoutControls();
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void menuItemDockRight_Click(object sender,EventArgs e) {
			if(IsDashboardVisible) {
				MsgBox.Show("Tasks cannot be docked to the right when Dashboards are in use.");
				return;
			}
			menuItemDockBottom.Checked=false;
			menuItemDockRight.Checked=true;
			//included now with layoutcontrols
			panelSplitter.Cursor=Cursors.VSplit;
			TaskDockSavePos();
			LayoutControls();
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		///<summary>This used to be called much more frequently when it was an actual layout event.</summary>
		private void LayoutControls(){
			if(WindowState==FormWindowState.Minimized) {
				return;
			}
			if(Width<200){
				Width=200;
			}
			if(moduleBar==null){
				return;
			}
			LayoutManager.MoveLocation(lightSignalGrid1,new Point(0,LayoutManager.Scale(489)));
			LayoutManager.MoveWidth(lightSignalGrid1,moduleBar.Width-1);
			//LayoutManager.SynchRecursive(userControlDashboard);
			Point position=new Point(moduleBar.Width,ToolBarMain.Bottom);
			int width=ClientSize.Width-position.X;
			int height=ClientSize.Height-position.Y;
			if(userControlTasks1.Visible) {
				if(menuItemDockBottom.Checked) {
					if(panelSplitter.Height>LayoutManager.Scale(9)) {//docking needs to be changed from right to bottom
						LayoutManager.MoveHeight(panelSplitter,LayoutManager.Scale(7));
						_pointPanelSplitter96dpi=new Point(0,540);
					}
					LayoutManager.MoveLocation(panelSplitter,new Point(position.X,LayoutManager.Scale(_pointPanelSplitter96dpi.Y)));
					LayoutManager.MoveWidth(panelSplitter,width);
					panelSplitter.Visible=true;
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)){
						LayoutManager.Move(panelPhoneSmall,new Rectangle(position.X,panelSplitter.Bottom,LayoutManager.Scale(213),ClientSize.Height-panelSplitter.Bottom));
						LayoutManager.Move(userControlTasks1,new Rectangle(panelPhoneSmall.Right,panelSplitter.Bottom,width-panelPhoneSmall.Width,ClientSize.Height-panelSplitter.Bottom));
						LayoutManager.MoveLocation(phoneSmall,new Point(0,comboTriageCoordinator.Bottom));//??
						//LayoutManager.MoveLocation(userControlTasks1,new Point(position.X+phoneSmall.Width,panelSplitter.Bottom));
						//LayoutManager.MoveWidth(userControlTasks1,width-phoneSmall.Width);
						panelPhoneSmall.Visible=true;
						//LayoutManager.MoveLocation(panelPhoneSmall,new Point(position.X,panelSplitter.Bottom));
						panelPhoneSmall.BringToFront();
					}
					else{
						panelPhoneSmall.Visible=false;
						LayoutManager.MoveLocation(userControlTasks1,new Point(position.X,panelSplitter.Bottom));
						LayoutManager.MoveWidth(userControlTasks1,width);
					}
					LayoutManager.MoveHeight(userControlTasks1,ClientSize.Height-userControlTasks1.Top);
					height=ClientSize.Height-panelSplitter.Height-userControlTasks1.Height-ToolBarMain.Height-menuMain.Height;
				}
				else {//docked Right
					panelPhoneSmall.Visible=false;
					if(panelSplitter.Width>LayoutManager.Scale(9)) {//docking needs to be changed
						LayoutManager.MoveWidth(panelSplitter,LayoutManager.Scale(7));
						_pointPanelSplitter96dpi=new Point(900,0);
					}
					LayoutManager.MoveLocation(panelSplitter,new Point(LayoutManager.Scale(_pointPanelSplitter96dpi.X),position.Y));
					LayoutManager.MoveHeight(panelSplitter,height);
					panelSplitter.Visible=true;
					LayoutManager.Move(userControlTasks1,new Rectangle(panelSplitter.Right,position.Y,ClientSize.Width-panelSplitter.Right,height));
					width=ClientSize.Width-panelSplitter.Width-userControlTasks1.Width-position.X;
				}
				panelSplitter.BringToFront();
				panelSplitter.Invalidate();
				userControlTasks1.Refresh();//draw even if not resizing
			}
			else {
				panelPhoneSmall.Visible=false;
				panelSplitter.Visible=false;
			}
			LayoutManager.Move(splitContainerNoFlickerDashboard,new Rectangle(position.X,position.Y,width,height));
			//splitContainerNoFlickerDashboard.Location=position;
			//splitContainerNoFlickerDashboard.Height=height;
			//splitContainerNoFlickerDashboard.Width=width;
			//Size sizePanel1=splitContainerNoFlickerDashboard.Panel1.ClientSize;
			if(userControlDashboard.IsInitialized && userControlDashboard.ListOpenWidgets.Count>0) {
				if(splitContainerNoFlickerDashboard.Panel2Collapsed) {
					splitContainerNoFlickerDashboard.Panel2Collapsed=false;//Make the Patient Dashboard visible.
				}
			}
			else {
				splitContainerNoFlickerDashboard.Panel2Collapsed=true;
			}
			ResizeDashboard();
			splitContainerNoFlickerDashboard.PerformLayout();//Amazingly, it's too stupid to do this on its own
			Point locationSplitContainer=splitContainerNoFlickerDashboard.Location;
			FillSignalButtons(null);//Refresh using cache only, do not run query, because this is fired a lot when resizing window or docked task control.
		}

		///<summary>Sets the splitter distance and Patient Dashboard height.  Width is not really up to the user.  They can drag the splitter, but it will quickly revert.</summary>
		private void ResizeDashboard() {
			int width=userControlDashboard.WidgetWidth;
			int widthScrollBar=0;
			if(userControlDashboard.VerticalScroll.Visible) {
				widthScrollBar=System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
			}
			LayoutManager.IsLayingOut=true;//so the next line won't trigger a layout
			splitContainerNoFlickerDashboard.SplitterDistance=Math.Max(splitContainerNoFlickerDashboard.Width-splitContainerNoFlickerDashboard.SplitterWidth-width
				-userControlDashboard.Margin.Left-widthScrollBar,0);
			LayoutManager.MoveSize(userControlDashboard,new Size(width,splitContainerNoFlickerDashboard.Height));
			LayoutManager.IsLayingOut=false;
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerNoFlickerDashboard);
		}

		private void splitContainerNoFlickerDashboard_SplitterMoved(object sender,SplitterEventArgs e) {
			if(userControlDashboard==null || splitContainerNoFlickerDashboard.Panel2Collapsed) {
				return;
			}
			if(controlAppt.Visible) {
				controlAppt.LayoutControls();
			}
			if(LayoutManager.IsLayingOut){
				return;
			}
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerNoFlickerDashboard);
		}

		///<summary>Every time user changes dock position, it will save automatically.</summary>
		private void TaskDockSavePos(){
			//ComputerPref computerPref = ComputerPrefs.GetForLocalComputer();
			if(menuItemDockBottom.Checked){
				ComputerPrefs.LocalComputer.TaskY =LayoutManager.Round(_pointPanelSplitter96dpi.Y); //panelSplitter.Top;
				ComputerPrefs.LocalComputer.TaskDock = 0;
			}
			else{
				ComputerPrefs.LocalComputer.TaskX =LayoutManager.Round(_pointPanelSplitter96dpi.X);  //panelSplitter.Left;
				ComputerPrefs.LocalComputer.TaskDock = 1;
			}
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
		}
		#endregion Splitters

		#region ToolBar
		private void toolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)) {
				//standard predefined button
				switch(e.Button.Tag.ToString()) {
					case "Patient":
						toolButPatient_Click();
						break;
					case "Commlog":
						toolButCommlog_Click();
						break;
					case "Email":
						toolButEmail_Click();
						break;
					case "WebMail":
						toolButWebMail_Click();
						break;
					case "Text":
						toolButTxtMsg_Click(PatNumCur);
						break;
					case "Letter":
						toolButLetter_Click();
						break;
					case "Form":
						toolButForm_Click();
						break;
					case "Tasklist":
						toolButTasks_Click();
						break;
					case "Label":
						toolButLabel_Click();
						break;
					case "Popups":
						toolButPopups_Click();
						break;
				}
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,Patients.GetPat(PatNumCur));
			}
		}
		
		///<Summary>Serves four functions.  1. Sends the new patient to the dropdown menu for select patient.  2. Changes which toolbar buttons are enabled.  3. Sets main form text.  4. Displays any popup.</Summary>
		private void FillPatientButton(Patient pat){
			if(pat==null) {
				pat=new Patient();
			}
			Text=PatientL.GetMainTitle(pat, Clinics.ClinicNum);
			bool patChanged=PatientL.AddPatsToMenu(menuPatient,new EventHandler(menuPatient_Click),pat.GetNameLF(),pat.PatNum);
			if(patChanged){
				if(AutomationL.Trigger(AutomationTrigger.OpenPatient,null,pat.PatNum)) {//if a trigger happened
					if(controlAppt.Visible) {
						controlAppt.MouseUpForced();
					}
				}
			}
			if(ToolBarMain.Buttons==null || ToolBarMain.Buttons.Count<2){//on startup.  js Not sure why it's checking count.
				return;
			}
			if(PatNumCur==0) {//Only on startup, I think.
				if(!Programs.UsingEcwTightMode()) {//eCW tight only gets Patient Select and Popups toolbar buttons
					//We need a drafts folder the user can view saved emails in before we allow the user to save email without a patient selected.
					ToolBarMain.Buttons["Email"].Enabled=false;
					ToolBarMain.Buttons["WebMail"].Enabled=false;
					ToolBarMain.Buttons["Commlog"].Enabled=false;
					ToolBarMain.Buttons["Letter"].Enabled=false;
					ToolBarMain.Buttons["Form"].Enabled=false;
					ToolBarMain.Buttons["Tasklist"].Enabled=true;
					ToolBarMain.Buttons["Label"].Enabled=false;
				}
				ToolBarMain.Buttons["Popups"].Enabled=false;
			}
			else {
				if(!Programs.UsingEcwTightMode()) {//eCW tight only gets Patient Select and Popups toolbar buttons
					ToolBarMain.Buttons["Commlog"].Enabled=true;
					ToolBarMain.Buttons["Email"].Enabled=true;
					if(_toolBarButtonText!=null) {
						_toolBarButtonText.Enabled=Programs.IsEnabled(ProgramName.CallFire)||SmsPhones.IsIntegratedTextingEnabled();
					}
					ToolBarMain.Buttons["WebMail"].Enabled=true;
					ToolBarMain.Buttons["Letter"].Enabled=true;
					ToolBarMain.Buttons["Form"].Enabled=true;
					ToolBarMain.Buttons["Tasklist"].Enabled=true;
					ToolBarMain.Buttons["Label"].Enabled=true;
				}
				ToolBarMain.Buttons["Popups"].Enabled=true;
			}
			ToolBarMain.Invalidate();
			if(_listPopupEvents==null){
				_listPopupEvents=new List<PopupEvent>();
			}
			if(Plugins.HookMethod(this,"FormOpenDental.FillPatientButton_popups",pat,_listPopupEvents,patChanged)) {
				return;
			}
			if(!patChanged) {
				return;
			}
			if(controlChart.Visible) {
				TryNonPatientPopup();
			}
			if(!PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
				controlImagesJ.CloseFloaters();
			}
			//New patient selected.  Everything below here is for popups.
			//First, remove all expired popups from the event list.
			for(int i=_listPopupEvents.Count-1;i>=0;i--){//go backwards
				if(_listPopupEvents[i].DisableUntil<DateTime.Now){//expired
					_listPopupEvents.RemoveAt(i);
				}
			}
			//Now, loop through all popups for the patient.
			List<Popup> popList=Popups.GetForPatient(pat);//get all possible 
			for(int i=0;i<popList.Count;i++) {
				//skip any popups that are disabled because they are on the event list
				bool popupIsDisabled=false;
				for(int e=0;e<_listPopupEvents.Count;e++){
					if(popList[i].PopupNum==_listPopupEvents[e].PopupNum){
						popupIsDisabled=true;
						break;
					}
				}
				if(popupIsDisabled){
					continue;
				}
				//This popup is not disabled, so show it.
				//A future improvement would be to assemble all the popups that are to be shown and then show them all in one large window.
				//But for now, they will show in sequence.
				if(controlAppt.Visible) {
					controlAppt.MouseUpForced();
				}
				using FormPopupDisplay FormP=new FormPopupDisplay();
				FormP.PopupCur=popList[i];
				FormP.ShowDialog();
				if(FormP.MinutesDisabled>0){
					PopupEvent popevent=new PopupEvent();
					popevent.PopupNum=popList[i].PopupNum;
					popevent.DisableUntil=DateTime.Now+TimeSpan.FromMinutes(FormP.MinutesDisabled);
					popevent.LastViewed=DateTime.Now;
					_listPopupEvents.Add(popevent);
					_listPopupEvents.Sort();
				}
			}
		}

		///<summary>Happens when any of the modules changes the current patient or when this main form changes the patient.  The calling module should refresh itself.  The current patNum is stored here in the parent form so that when switching modules, the parent form knows which patient to call up for that module.</summary>
		private void Contr_PatientSelected(Patient pat,bool isRefreshCurModule,bool isApptRefreshDataPat,bool hasForcedRefresh) {
			PatNumCur=pat.PatNum;
			if(isRefreshCurModule) {
				RefreshCurrentModule(hasForcedRefresh,isApptRefreshDataPat);
			}
			userControlTasks1.RefreshPatTicketsIfNeeded();
			FillPatientButton(pat);
		}

		private void TryNonPatientPopup() {
			if(PatNumCur!=0 && _patNumPrevious!=PatNumCur) {
				_datePopupDelay=DateTime.Now;
				_patNumPrevious=PatNumCur;
			}
			if(!PrefC.GetBool(PrefName.ChartNonPatientWarn)) {
				return;
			}
			Patient patCur=Patients.GetPat(PatNumCur);
			if(patCur!=null 
						&& patCur.PatStatus.ToString()=="NonPatient"
						&& _datePopupDelay<=DateTime.Now) {
				MsgBox.Show(this,"A patient with the status NonPatient is currently selected.");
				_datePopupDelay=DateTime.Now.AddMinutes(5);
			}
		}

		private void menuPatient_Click(object sender,System.EventArgs e) {
			Family fam=Patients.GetFamily(PatNumCur);
			PatNumCur=PatientL.ButtonSelect(menuPatient,sender,fam);
			//new family now
			Patient pat=Patients.GetPat(PatNumCur);
			RefreshCurrentModule();
			FillPatientButton(pat);
		}

		private void menuPatient_Popup(object sender,EventArgs e) {
			Family fam=null;
			if(PatNumCur!=0) {
				fam=Patients.GetFamily(PatNumCur);
			}
			//Always refresh the patient menu to reflect any patient status changes.
			PatientL.AddFamilyToMenu(menuPatient,new EventHandler(menuPatient_Click),PatNumCur,fam);
		}

		private void toolButEmail_Click() {
			if(PatNumCur==0) {
				MsgBox.Show(this,"Please select a patient to send an email.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.EmailSend)){
				return;
			}
			EmailMessage message=new EmailMessage();
			message.PatNum=PatNumCur;
			Patient pat=Patients.GetPat(PatNumCur);
			message.ToAddress=pat.Email;
			EmailAddress selectedAddress = EmailAddresses.GetNewEmailDefault(Security.CurUser.UserNum,pat.ClinicNum);
			message.FromAddress=selectedAddress.GetFrom();
			message.MsgType=EmailMessageSource.Manual;
			using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,selectedAddress);
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult==DialogResult.OK) {
				RefreshCurrentModule();
			}
		}

		private void menuEmail_Popup(object sender,EventArgs e) {
			menuEmail.MenuItems.Clear();
			MenuItem menuItem;
			menuItem=new MenuItem(Lan.g(this,"Referrals:"));
			menuItem.Tag=null;
			menuEmail.MenuItems.Add(menuItem);
			List<RefAttach> refAttaches=RefAttaches.Refresh(PatNumCur);
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral'
				referralDescript=Lan.g(this,"Referral");
			}
			Referral refer;
			string str;
			for(int i=0;i<refAttaches.Count;i++) {
				if(!Referrals.TryGetReferral(refAttaches[i].ReferralNum,out refer)) {
					continue;
				}
				if(refAttaches[i].RefType==ReferralType.RefFrom) {
					str=Lan.g(this,"From");
				}
				else if(refAttaches[i].RefType==ReferralType.RefTo) {
					str=Lan.g(this,"To");
				}
				else {
					str=referralDescript;
				}
				str+=" "+Referrals.GetNameFL(refer.ReferralNum)+" <";
				if(refer.EMail==""){
					str+=Lan.g(this,"no email");
				}
				else{
					str+=refer.EMail;
				}
				str+=">";
				menuItem=new MenuItem(str,menuEmail_Click);
				menuItem.Tag=refer;
				menuEmail.MenuItems.Add(menuItem);
			}
		}

		private void toolButWebMail_Click() {
			if(!Security.IsAuthorized(Permissions.WebMailSend)) {
				return;
			}
			using FormWebMailMessageEdit FormWMME=new FormWebMailMessageEdit(PatNumCur);
			FormWMME.ShowDialog();
		}

		private void menuEmail_Click(object sender,System.EventArgs e) {
			if(((MenuItem)sender).Tag==null){
				return;
			}
			LabelSingle label=new LabelSingle();
			if(((MenuItem)sender).Tag.GetType()==typeof(Referral)) {
				Referral refer=(Referral)((MenuItem)sender).Tag;
				if(refer.EMail==""){
					return;
					//MsgBox.Show(this,"");
				}
				EmailMessage message=new EmailMessage();
				message.PatNum=PatNumCur;
				Patient pat=Patients.GetPat(PatNumCur);
				message.ToAddress=refer.EMail;//pat.Email;
				EmailAddress address=EmailAddresses.GetByClinic(pat.ClinicNum);
				message.FromAddress=address.GetFrom();
				message.Subject=Lan.g(this,"RE: ")+pat.GetNameFL();
				message.MsgType=EmailMessageSource.Manual;
				using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,address);
				FormE.IsNew=true;
				FormE.ShowDialog();
				if(FormE.DialogResult==DialogResult.OK) {
					RefreshCurrentModule();
				}
			}
		}

		private void toolButCommlog_Click() {
			if(Plugins.HookMethod(this,"FormOpenDental.OnCommlog_Click",PatNumCur)) {
				return;
			}
			using FormCommItem FormCI=new FormCommItem(GetNewCommlog());
			if(FormCI.ShowDialog()==DialogResult.OK) {
				RefreshCurrentModule();
			}
		}

		private void menuItemCommlogPersistent_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.CommlogPersistent)) {
				return;
			}
			FormCommItem FormCI=Application.OpenForms.OfType<FormCommItem>().FirstOrDefault(x => !x.IsDisposed);
			if(FormCI==null) {
				FormCI=new FormCommItem(GetNewCommlog(),isPersistent:true);
			}
			if(FormCI.WindowState==FormWindowState.Minimized) {
				FormCI.WindowState=FormWindowState.Normal;
			}
			FormCI.Show();
			FormCI.BringToFront();
		}

		///<summary>This is a helper method to get a new commlog object for the commlog tool bar buttons.</summary>
		private Commlog GetNewCommlog() {
			Commlog commlog=new Commlog();
			commlog.PatNum=PatNumCur;
			commlog.CommDateTime=DateTime.Now;
			commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			if(PrefC.GetBool(PrefName.DistributorKey)) {//for OD HQ
				commlog.Mode_=CommItemMode.None;
				commlog.SentOrReceived=CommSentOrReceived.Neither;
			}
			else {
				commlog.Mode_=CommItemMode.Phone;
				commlog.SentOrReceived=CommSentOrReceived.Received;
			}
			commlog.UserNum=Security.CurUser.UserNum;
			commlog.IsNew=true;
			return commlog;
		}

		private void toolButLetter_Click() {
			using FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.PatientLetter;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			SheetDef sheetDef=FormS.SelectedSheetDefs[0];
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatNumCur);
			SheetParameter.SetParameter(sheet,"PatNum",PatNumCur);
			if(SheetDefs.ContainsStaticFields(sheetDef,StaticTextField.apptDateMonthSpelled,
				StaticTextField.apptProcs,
				StaticTextField.apptProvNameFormal))
			{
				long aptNum=GetAptNumForLetter();
				if(aptNum < 0) {
					return;
				}
				SheetParameter.SetParameter(sheet,"AptNum",aptNum);
			}
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void menuLetter_Popup(object sender,EventArgs e) {
			menuLetter.MenuItems.Clear();
			MenuItem menuItem;
			menuItem=new MenuItem(Lan.g(this,"Merge"),menuLetter_Click);
			menuItem.Tag="Merge";
			menuLetter.MenuItems.Add(menuItem);
			//menuItem=new MenuItem(Lan.g(this,"Stationery"),menuLetter_Click);
			//menuItem.Tag="Stationery";
			//menuLetter.MenuItems.Add(menuItem);
			menuLetter.MenuItems.Add("-");
			//Referrals---------------------------------------------------------------------------------------
			menuItem=new MenuItem(Lan.g(this,"Referrals:"));
			menuItem.Tag=null;
			menuLetter.MenuItems.Add(menuItem);
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral'
				referralDescript=Lan.g(this,"Referral");
			}
			List<RefAttach> refAttaches=RefAttaches.Refresh(PatNumCur);
			Referral refer;
			string str;
			for(int i=0;i<refAttaches.Count;i++) {
				if(!Referrals.TryGetReferral(refAttaches[i].ReferralNum,out refer)) {
					continue;
				}
				if(refAttaches[i].RefType==ReferralType.RefFrom) {
					str=Lan.g(this,"From");
				}
				else if(refAttaches[i].RefType==ReferralType.RefTo) {
					str=Lan.g(this,"To");
				}
				else {
					str=referralDescript;
				}
				str+=" "+Referrals.GetNameFL(refer.ReferralNum);
				menuItem=new MenuItem(str,menuLetter_Click);
				menuItem.Tag=refer;
				menuLetter.MenuItems.Add(menuItem);
			}
		}

		private void menuLetter_Click(object sender,System.EventArgs e) {
			if(((MenuItem)sender).Tag==null) {
				return;
			}
			Patient pat=Patients.GetPat(PatNumCur);
			if(((MenuItem)sender).Tag.GetType()==typeof(string)) {
				if(((MenuItem)sender).Tag.ToString()=="Merge") {
					if(ODBuild.IsWeb()) {
						MsgBox.Show(this,"Letter Merge is not available while viewing through the web.");
						return;
					}
					using FormLetterMerges FormL=new FormLetterMerges(pat);
					FormL.ShowDialog();
				}
				//if(((MenuItem)sender).Tag.ToString()=="Stationery") {
				//	FormCommunications.PrintStationery(pat);
				//}
			}
			if(((MenuItem)sender).Tag.GetType()==typeof(Referral)) {
				Referral refer=(Referral)((MenuItem)sender).Tag;
				using FormSheetPicker FormS=new FormSheetPicker();
				FormS.SheetType=SheetTypeEnum.ReferralLetter;
				FormS.ShowDialog();
				if(FormS.DialogResult!=DialogResult.OK){
					return;
				}
				SheetDef sheetDef=FormS.SelectedSheetDefs[0];
				Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatNumCur);
				SheetParameter.SetParameter(sheet,"PatNum",PatNumCur);
				SheetParameter.SetParameter(sheet,"ReferralNum",refer.ReferralNum);
				//Don't fill these params if the sheet doesn't use them.
				if(sheetDef.SheetFieldDefs.Any(x =>
					(x.FieldType==SheetFieldType.Grid && x.FieldName=="ReferralLetterProceduresCompleted")
					|| (x.FieldType==SheetFieldType.Special && x.FieldName=="toothChart")))
				{
					List<Procedure> listProcs=Procedures.GetCompletedForDateRange(sheet.DateTimeSheet,sheet.DateTimeSheet
						,listPatNums:new List<long>() { PatNumCur }
						,includeNote:true
						,includeGroupNote:true);
					if(sheetDef.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.Grid && x.FieldName=="ReferralLetterProceduresCompleted")) {
						SheetParameter.SetParameter(sheet,"CompletedProcs",listProcs);
					}
					if(sheetDef.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.Special && x.FieldName=="toothChart")) {
						SheetParameter.SetParameter(sheet,"toothChartImg",SheetPrinting.GetToothChartHelper(PatNumCur,false,listProceduresFilteredOverride:listProcs));
					}
				}
				if(SheetDefs.ContainsStaticFields(sheetDef,StaticTextField.apptDateMonthSpelled,
					StaticTextField.apptProcs,
					StaticTextField.apptProvNameFormal))
				{
					long aptNum=GetAptNumForLetter();
					if(aptNum < 0) {
						return;
					}
					SheetParameter.SetParameter(sheet,"AptNum",aptNum);
				}
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
			}
		}

		///<summary>Helper method for selecting an appointment for letters. Automatically returns an AptNum if the currently selected patient has exactly one appointment (not very helpful). Otherwise, launches the "Appointment for" window in selection mode. Returns 0 if the user does not have any appointments. Returns -1 if the user cancelled out of the appointment select window.</summary>
		private long GetAptNumForLetter() {
			Appointment[] arrayAppts=Appointments.GetForPat(PatNumCur);
			long aptNum;
			if(arrayAppts.Length==0) {
				aptNum=0;
			}
			else if(arrayAppts.Length==1) {
				aptNum=arrayAppts[0].AptNum;
			}
			else {
				using FormApptsOther FormA=new FormApptsOther(PatNumCur,null);
				FormA.AllowSelectOnly=true;
				FormA.ShowDialog();
				aptNum=-1;
				if(FormA.DialogResult==DialogResult.OK) {
					aptNum=FormA.ListAptNumsSelected[0];
				}
			}
			return aptNum;
		}

		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK || ((FormSheetFillEdit)sender).DidChangeSheet) {
				RefreshCurrentModule();
			}
		}

		private void toolButForm_Click() {
			using FormPatientForms formP=new FormPatientForms();
			formP.PatNum=PatNumCur;
			formP.ShowDialog();
			//always refresh, especially to get the titlebar right after an import.
			Patient pat=Patients.GetPat(PatNumCur);
			RefreshCurrentModule(docNum:formP.DocNum);
			FillPatientButton(pat);
		}

		private void toolButTasks_Click(){
			using FormTaskListSelect FormT=new FormTaskListSelect(TaskObjectType.Patient);
			FormT.Location=new Point(50,50);
			FormT.Text=Lan.g(FormT,"Add Task")+" - "+FormT.Text;
			FormT.ShowDialog();
			if(FormT.DialogResult!=DialogResult.OK) {
				return;
			}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			task.KeyNum=PatNumCur;
			task.ObjectType=TaskObjectType.Patient;
			task.TaskListNum=FormT.ListSelectedLists[0];
			task.UserNum=Security.CurUser.UserNum;
			FormTaskEdit FormTE=new FormTaskEdit(task,taskOld);
			FormTE.IsNew=true;
			FormTE.Show();
		}

		private void menuTask_Popup(object sender,EventArgs e) {
			menuItemTaskNewForUser.Text=Lan.g(this,"for")+" "+Security.CurUser.UserName;
			menuItemTaskReminders.Text=Lan.g(this,"Reminders");
			int reminderTaskNewCount=GetNewReminderTaskCount();
			if(reminderTaskNewCount > 0) {
				menuItemTaskReminders.Text+=" ("+reminderTaskNewCount+")";
			}
			if(PrefC.GetBool(PrefName.TasksUseRepeating)) {
				menuItemTaskReminders.Visible=false;
			}
			else { 
				menuItemTaskReminders.Visible=true;
			}
			int otherTaskCount=(_listTaskNumsNormal!=null)?_listTaskNumsNormal.Count:0;
			if(otherTaskCount > 0) {
				menuItemTaskNewForUser.Text+=" ("+otherTaskCount+")";
			}			
		}

		private void RefreshTasksNotification() {
			if(_toolBarButtonTask==null) {
				return;
			}
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			int otherTaskCount=(_listTaskNumsNormal!=null)?_listTaskNumsNormal.Count:0;
			int totalTaskCount=GetNewReminderTaskCount()+otherTaskCount;
			string notificationText="";
			if(totalTaskCount > 0) {
				notificationText=Math.Min(totalTaskCount,99).ToString();
			}
			if(notificationText!=_toolBarButtonTask.NotificationText) {
				_toolBarButtonTask.NotificationText=notificationText;
				ToolBarMain.Invalidate(_toolBarButtonTask.Bounds);//Cause the notification text on the Task button to update as soon as possible.
			}
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
		}

		private int GetNewReminderTaskCount() {
			if(_listTasksReminders==null) {
				return 0;
			}
			//Mimics how checkNew is set in FormTaskEdit.
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//Per definition of task.IsUnread.
				return _listTasksReminders.FindAll(x => x.IsUnread && x.DateTimeEntry<=DateTime.Now).Count;
			}
			return _listTasksReminders.FindAll(x => x.TaskStatus==TaskStatusEnum.New && x.DateTimeEntry<=DateTime.Now).Count;
		}

		private void menuItemTaskNewForUser_Click(object sender,EventArgs e) {
			controlManage.LaunchTaskWindow(false,UserControlTasksTab.ForUser);//Set the tab to the "for [User]" tab.
		}

		private void menuItemTaskReminders_Click(object sender,EventArgs e) {
			controlManage.LaunchTaskWindow(false,UserControlTasksTab.Reminders);//Set the tab to the "Reminders" tab
		}

		private delegate void ToolBarMainClick(long patNum);

		private void toolButLabel_Click() {
			//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
			//when it comes from a toolbar click.
			//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
			ToolBarMainClick toolClick=LabelSingle.PrintPat;
			this.BeginInvoke(toolClick,PatNumCur);
		}

		private void menuLabel_Popup(object sender,EventArgs e) {
			menuLabel.MenuItems.Clear();
			MenuItem menuItem;
			List<SheetDef> LabelList=SheetDefs.GetCustomForType(SheetTypeEnum.LabelPatient);
			if(LabelList.Count==0){
				menuItem=new MenuItem(Lan.g(this,"LName, FName, Address"),menuLabel_Click);
				menuItem.Tag="PatientLFAddress";
				menuLabel.MenuItems.Add(menuItem);
				menuItem=new MenuItem(Lan.g(this,"Name, ChartNumber"),menuLabel_Click);
				menuItem.Tag="PatientLFChartNumber";
				menuLabel.MenuItems.Add(menuItem);
				menuItem=new MenuItem(Lan.g(this,"Name, PatNum"),menuLabel_Click);
				menuItem.Tag="PatientLFPatNum";
				menuLabel.MenuItems.Add(menuItem);
				menuItem=new MenuItem(Lan.g(this,"Radiograph"),menuLabel_Click);
				menuItem.Tag="PatRadiograph";
				menuLabel.MenuItems.Add(menuItem);
			}
			else{
				for(int i=0;i<LabelList.Count;i++) {
					menuItem=new MenuItem(LabelList[i].Description,menuLabel_Click);
					menuItem.Tag=LabelList[i];
					menuLabel.MenuItems.Add(menuItem);
				}
			}
			menuLabel.MenuItems.Add("-");
			//Carriers---------------------------------------------------------------------------------------
			Family fam=Patients.GetFamily(PatNumCur);
			//Received multiple bug submissions where CurPatNum==0, even though this toolbar button should not be enabled when no patient is selected.
			if(fam.ListPats!=null && fam.ListPats.Length>0) {
				List <PatPlan> PatPlanList=PatPlans.Refresh(PatNumCur);
				List<InsSub> subList=InsSubs.RefreshForFam(fam);
				List<InsPlan> PlanList=InsPlans.RefreshForSubList(subList);
				Carrier carrier;
				InsPlan plan;
				InsSub sub;
				for(int i=0;i<PatPlanList.Count;i++) {
					sub=InsSubs.GetSub(PatPlanList[i].InsSubNum,subList);
					plan=InsPlans.GetPlan(sub.PlanNum,PlanList);
					carrier=Carriers.GetCarrier(plan.CarrierNum);
					menuItem=new MenuItem(carrier.CarrierName,menuLabel_Click);
					menuItem.Tag=carrier;
					menuLabel.MenuItems.Add(menuItem);
				}
				menuLabel.MenuItems.Add("-");
			}
			//Referrals---------------------------------------------------------------------------------------
			menuItem=new MenuItem(Lan.g(this,"Referrals:"));
			menuItem.Tag=null;
			menuLabel.MenuItems.Add(menuItem);
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral'
				referralDescript=Lan.g(this,"Referral");
			}
			List<RefAttach> refAttaches=RefAttaches.Refresh(PatNumCur);
			Referral refer;
			string str;
			for(int i=0;i<refAttaches.Count;i++){
				if(!Referrals.TryGetReferral(refAttaches[i].ReferralNum,out refer)) {
					continue;
				}
				if(refAttaches[i].RefType==ReferralType.RefFrom) {
					str=Lan.g(this,"From");
				}
				else if(refAttaches[i].RefType==ReferralType.RefTo) {
					str=Lan.g(this,"To");
				}
				else {
					str=referralDescript;
				}
				str+=" "+Referrals.GetNameFL(refer.ReferralNum);
				menuItem=new MenuItem(str,menuLabel_Click);
				menuItem.Tag=refer;
				menuLabel.MenuItems.Add(menuItem);
			}
		}

		private void menuLabel_Click(object sender,System.EventArgs e) {
			if(((MenuItem)sender).Tag==null) {
				return;
			}
			//LabelSingle label=new LabelSingle();
			if(((MenuItem)sender).Tag.GetType()==typeof(string)){
				if(((MenuItem)sender).Tag.ToString()=="PatientLFAddress"){
					LabelSingle.PrintPatientLFAddress(PatNumCur);
				}
				if(((MenuItem)sender).Tag.ToString()=="PatientLFChartNumber") {
					LabelSingle.PrintPatientLFChartNumber(PatNumCur);
				}
				if(((MenuItem)sender).Tag.ToString()=="PatientLFPatNum") {
					LabelSingle.PrintPatientLFPatNum(PatNumCur);
				}
				if(((MenuItem)sender).Tag.ToString()=="PatRadiograph") {
					LabelSingle.PrintPatRadiograph(PatNumCur);
				}
			}
			else if(((MenuItem)sender).Tag.GetType()==typeof(SheetDef)){
				LabelSingle.PrintCustomPatient(PatNumCur,(SheetDef)((MenuItem)sender).Tag);
			}
			else if(((MenuItem)sender).Tag.GetType()==typeof(Carrier)){
				Carrier carrier=(Carrier)((MenuItem)sender).Tag;
				LabelSingle.PrintCarrier(carrier.CarrierNum);
			}
			else if(((MenuItem)sender).Tag.GetType()==typeof(Referral)) {
				Referral refer=(Referral)((MenuItem)sender).Tag;
				LabelSingle.PrintReferral(refer.ReferralNum);
			}
		}

		private void toolButPopups_Click() {
			using FormPopupsForFam FormPFF=new FormPopupsForFam(_listPopupEvents);
			FormPFF.PatCur=Patients.GetPat(PatNumCur);
			FormPFF.ShowDialog();
		}
		#endregion ToolBar

		#region SMS Text Messaging
		///<summary>Returns true if the message was sent successfully.</summary>
		public static bool S_TxtMsg_Click(long patNum,string startingText="") {
			return _formOpenDentalSingleton.toolButTxtMsg_Click(patNum,startingText);
		}

		///<summary>Called from the text message button and the right click context menu for an appointment. Returns true if the message was sent successfully.</summary>
		private bool toolButTxtMsg_Click(long patNum,string startingText="") {
			if(patNum==0) {
				using FormTxtMsgEdit FormTxtME=new FormTxtMsgEdit();
				FormTxtME.Message=startingText;
				FormTxtME.PatNum=0;
				FormTxtME.ShowDialog();
				if(FormTxtME.DialogResult==DialogResult.OK) {
					RefreshCurrentModule();
					return true;
				}
				return false;
			}
			Patient pat=Patients.GetPat(patNum);
			bool updateTextYN=false;
			if(pat.TxtMsgOk==YN.No){
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient is marked to not receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) 
				{
					updateTextYN=true;
				}
				else {
					return false;
				}
			}
			if(pat.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)){
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient might not want to receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) 
				{
					updateTextYN=true;
				}
				else {
					return false;
				}
			}
			if(updateTextYN) {
				Patient patOld=pat.Copy();
				pat.TxtMsgOk=YN.Yes;
				Patients.Update(pat,patOld);
				Patients.InsertAddressChangeSecurityLogEntry(patOld,pat);// track change in securitylog for TxtOK Field
			}
			using FormTxtMsgEdit FormTME=new FormTxtMsgEdit();
			FormTME.Message=startingText;
			FormTME.PatNum=patNum;
			FormTME.WirelessPhone=pat.WirelessPhone;
			FormTME.TxtMsgOk=pat.TxtMsgOk;
			FormTME.ShowDialog();
			if(FormTME.DialogResult==DialogResult.OK) {
				RefreshCurrentModule();
				return true;
			}
			return false;
		}

		private void menuItemTextMessagesReceived_Click(object sender,EventArgs e) {
			ShowFormTextMessagingModeless(false,true);
		}

		private void menuItemTextMessagesSent_Click(object sender,EventArgs e) {
			ShowFormTextMessagingModeless(true,false);
		}

		private void menuItemTextMessagesAll_Click(object sender,EventArgs e) {
			ShowFormTextMessagingModeless(true,true);
		}

		private void ShowFormTextMessagingModeless(bool isSent, bool isReceived) {
			if(_formSmsTextMessaging==null || _formSmsTextMessaging.IsDisposed) {
				_formSmsTextMessaging=new FormSmsTextMessaging(isSent,isReceived,(x) => { SetSmsNotificationText(increment:x); });
				_formSmsTextMessaging.FormClosed+=new FormClosedEventHandler((o,e) => { _formSmsTextMessaging=null; });
			}
			_formSmsTextMessaging.Show();
			_formSmsTextMessaging.BringToFront();
		}

		///<summary>Sets the SMS "Text" button's Notification Text based on structured data parsed from signalSmsCount.MsgValue.  This Signalod will have been inserted into the db by the eConnector.  If signalSmsCount is not passed in, attempts to find the most recent Signalod of type SmsTextMsgReceivedUnreadCount, using it to update the notification text, or if not found, either creates and inserts the Signalod (this occurs on startup if the Signalod table does not have an entry for this signal type) or uses the currently displayed value of the Sms notification 
		///Text and the 'increment' value to update the locally displayed notification count (occurs when this method is called between signal intervals
		///and the eConnector has not updated the SmsTextmsgReceivedUnreadCount signal in the last signal interval).
		///</summary>
		///<param name="signalSmsCount">Signalod, inserted by the eConnector, containing a list of clinicnums and the count of unread SmsFromMobiles for 
		///each clinic.</param>
		///<param name="doUseSignalInterval">Defaults to true.  Indicates, in the event that signalSmsCount is null, if the query to find the most 
		///recent SmsTextMsgReceivedUnreadCount type Signalod should be run for the interval since signals were last processed, or if the entire table
		///should be considered.</param>
		///<param name="increment">Defaults to 0.  Increments the value displayed in the Sms notification "Text" button for unread SmsFromMobiles, but
		///only if a Signalod of type SmsTextMsgReceivedUnreadCount was not found.  This can occur if signalSmsCount is null, doUseSignalInteral is true
		///and the signal was not found in the the last signal interval (meaning the eConnector has not updated the SmsNotification count recently).
		///</param>
		private void SetSmsNotificationText(Signalod signalSmsCount=null,bool doUseSignalInterval=true,int increment=0) {
			if(_toolBarButtonText==null) {
				return;//This button does not exist in eCW tight integration mode.
			}
			try {
				if(!_toolBarButtonText.Enabled) {
					return;//This button is disabled when neither of the Text Messaging bridges have been enabled.
				}
				List<SmsFromMobiles.SmsNotification> listNotifications=null;
				if(signalSmsCount==null) {
					//If we are here because the user changed clinics, then get the absolute most recent sms notification signal.
					//Otherwise, use DateTime since last signal refresh.
					DateTime signalStartTime=doUseSignalInterval ? Signalods.SignalLastRefreshed : DateTime.MinValue;
					//Get the most recent SmsTextMsgReceivedUnreadCount. Should only be one, but just in case, order desc.
					signalSmsCount=Signalods.RefreshTimed(signalStartTime,new List<InvalidType>() { InvalidType.SmsTextMsgReceivedUnreadCount })
						.OrderByDescending(x => x.SigDateTime)
						.FirstOrDefault();
					if(signalSmsCount==null && signalStartTime==DateTime.MinValue) {
						//No SmsTextMsgReceivedUnreadCount signal in db.  This means the eConnector has not updated the sms notification signal in quite some 
						//time.  Do the eConnector's job; 
						listNotifications=Signalods.UpsertSmsNotification();
					}
				}
				if(signalSmsCount!=null) {//Either the signal was passed in, or we found it when we queried.
					listNotifications=SmsFromMobiles.SmsNotification.GetListFromJson(signalSmsCount.MsgValue);//Extract notifications from signal.
					if(listNotifications==null) {
						return;//Something went wrong deserializing the signal.  Leave the stale notification count until eConnector updates the signal.
					}
				}
				int smsUnreadCount=0;
				if(listNotifications==null) {
					//listNotifications might still be null if signalSmsCount was not passed in, signal processing had already started, and we didn't find the
					//sms notification signal in the last signal interval.  We will assume the signal is stale.  We know the count has changed (based on some 
					//action) if 'increment' is non-zero, so increment according to our known changes.
					smsUnreadCount=PIn.Int(_toolBarButtonText.NotificationText)+increment;
				}
				else if(!PrefC.HasClinicsEnabled||Clinics.ClinicNum==0) {
					//No clinics or HQ clinic is active so sum them all.
					smsUnreadCount=listNotifications.Sum(x => x.Count);
				}
				else {
					//Only count the active clinic.
					smsUnreadCount=listNotifications.Where(x => x.ClinicNum==Clinics.ClinicNum).Sum(x => x.Count);
				}
				//Default to empty so we show nothing if there aren't any notifications.
				string smsNotificationText="";
				if(smsUnreadCount>99) { //We only have room in the UI for a 2-digit number.
					smsNotificationText="99";
				}
				else if(smsUnreadCount>0) { //We have a "real" number so show it.
					smsNotificationText=smsUnreadCount.ToString();
				}
				if(_toolBarButtonText.NotificationText==smsNotificationText) { //Prevent the toolbar from being invalidated unnecessarily.
					return;
				}
				_toolBarButtonText.NotificationText=smsNotificationText;
				if(menuItemTextMessagesReceived.Text.Contains("(")) {//Remove the old count from the menu item.
					menuItemTextMessagesReceived.Text=menuItemTextMessagesReceived.Text.Substring(0,menuItemTextMessagesReceived.Text.IndexOf("(")-1);
				}
				if(smsNotificationText!="") {
					menuItemTextMessagesReceived.Text+=" ("+smsNotificationText+")";
				}
				Plugins.HookAddCode(this,"FormOpenDental.SetSmsNotificationText_end",_toolBarButtonText,menuItemTextMessagesReceived,increment);
			}
			finally { //Always redraw the toolbar item.
				ToolBarMain.Invalidate(_toolBarButtonText.Bounds);//To cause the Text button to redraw.			
			}
		}

		#endregion SMS Text Messaging

		#region Clinics
		private void RefreshMenuClinics() {
			_menuItemClinicsMain.DropDown.Items.Clear();
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
			if(listClinics.Count<30) { //This number of clinics will fit in a 990x735 form.
				MenuItemOD menuItem;
				if(!Security.CurUser.ClinicIsRestricted) {
					menuItem=new MenuItemOD(Lan.g(this,"Headquarters"),menuClinic_Click);
					menuItem.Tag=new Clinic();//Having a ClinicNum of 0 will make OD act like 'Headquarters'.  This allows the user to see unassigned appt views, all operatories, etc.
					if(Clinics.ClinicNum==0) {
						menuItem.Checked=true;
					}
					
					_menuItemClinicsMain.Add(menuItem);
					_menuItemClinicsMain.AddSeparator();
				}
				for(int i=0;i<listClinics.Count;i++) {
					menuItem=new MenuItemOD(listClinics[i].Abbr,menuClinic_Click);
					menuItem.Tag=listClinics[i];
					if(Clinics.ClinicNum==listClinics[i].ClinicNum) {
						menuItem.Checked=true;
					}
					_menuItemClinicsMain.Add(menuItem);
				}
			}
			else {//too many clinics to put in a menu drop down
				_menuItemClinicsMain.Click-=menuClick_OpenPickList;
				_menuItemClinicsMain.Click+=menuClick_OpenPickList;
			}
			RefreshLocalData(InvalidType.Views);//fills apptviews, sets the view, and then calls ContrAppt.ModuleSelected
			if(!controlAppt.Visible) {
				RefreshCurrentModule();//calls ModuleSelected of the current module, don't do this if ContrAppt2 is visible since it was just done above
			}
			moduleBar.RefreshButtons();
			CheckAlerts();
		}

		private void menuClick_OpenPickList(object sender,EventArgs e) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			if(!Security.CurUser.ClinicIsRestricted) {
				FormC.IncludeHQInList=true;
			}
			FormC.ShowDialog();
			if(FormC.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormC.SelectedClinicNum==0) {//'Headquarters' was selected.
				RefreshCurrentClinic(new Clinic());
				return;
			}
			Clinic clinicCur=Clinics.GetFirstOrDefault(x => x.ClinicNum==FormC.SelectedClinicNum);
			if(clinicCur!=null) { //Should never be null because the clinic should always be in the list
				RefreshCurrentClinic(clinicCur);
			}
			CheckAlerts();
		}

		///<summary>This is will set Clinics.ClinicNum and refresh the current module.</summary>
		private void menuClinic_Click(object sender,System.EventArgs e) {
			if(sender.GetType()!=typeof(MenuItemOD) && ((MenuItemOD)sender).Tag!=null) {
				return;
			}
			Clinic clinicCur=(Clinic)((MenuItemOD)sender).Tag;
			RefreshCurrentClinic(clinicCur);
		}

		///<summary>This is used to set Clinics.ClinicNum and refreshes the current module.</summary>
		private void RefreshCurrentClinic(Clinic clinicCur) {
			bool isChangingClinic=(Clinics.ClinicNum!=clinicCur.ClinicNum);
			Clinics.ClinicNum=clinicCur.ClinicNum;
			Text=PatientL.GetMainTitle(Patients.GetPat(PatNumCur),Clinics.ClinicNum);
			SetSmsNotificationText(doUseSignalInterval:!isChangingClinic);
			if(PrefC.GetBool(PrefName.AppointmentClinicTimeReset)) {
				controlAppt.ModuleSelected(DateTime.Today);
				//this actually refreshes the module, which is possibly different behavior than before the overhaul.
				//Alternatively, we might check to see of that module is selected first.
			}
			RefreshMenuClinics();
			if(isChangingClinic) {
				_listTaskNumsNormal=null;//Will cause task preprocessing to run again.
				_listTasksReminders=null;//Will cause task preprocessing to run again.
				UserControlTasks.ResetGlobalTaskFilterTypesToDefaultAllInstances();
				UserControlTasks.RefreshTasksForAllInstances(null);//Refresh tasks so any filter changes are applied immediately.
				//In the future this may need to be enhanced to also consider refreshing other clinic specific features
				LayoutToolBar();
				FillPatientButton(Patients.GetPat(PatNumCur));//Need to do this also for disabling of buttons when no pat is selected.
			}
		}
		#endregion Clinics

		#region Signals
		///<summary>This is called when any local data becomes outdated.  It's purpose is to tell the other computers to update certain local data.</summary>
		private void DataValid_BecameInvalid(OpenDental.ValidEventArgs e) {
			string suffix=Lan.g(nameof(Cache),"Refreshing Caches")+": ";
			ODEvent.Fire(ODEventType.Cache,suffix);
			if(e.OnlyLocal) {//Currently used after doing a restore from FormBackup so that the local cache is forcefully updated.
				ODEvent.Fire(ODEventType.Cache,suffix+Lan.g(nameof(Cache),"PrefsStartup"));
				if(!PrefsStartup()){//??
					return;
				}
				ODEvent.Fire(ODEventType.Cache,suffix+Lan.g(nameof(Cache),"AllLocal"));
				RefreshLocalData(InvalidType.AllLocal);//does local computer only
				return;
			}
			if(!e.ITypes.Contains(InvalidType.Appointment) //local refresh for dates is handled within ContrAppt, not here
				&& !e.ITypes.Contains(InvalidType.Task)//Tasks are not "cached" data.
				&& !e.ITypes.Contains(InvalidType.TaskPopup))
			{
				RefreshLocalData(e.ITypes);//does local computer
			}
			if(e.ITypes.Contains(InvalidType.Task) || e.ITypes.Contains(InvalidType.TaskPopup)) {
				Plugins.HookAddCode(this,"FormOpenDental.DataValid_BecameInvalid_taskInvalidTypes");
				if(controlChart?.Visible??false) {
					ODEvent.Fire(ODEventType.Cache,suffix+Lan.g(nameof(Cache),"Chart Module"));
					controlChart.ModuleSelected(PatNumCur);
				}
				return;//All task signals should already be sent. Sending more Task signals here would cause unnecessary refreshes.
			}
			ODEvent.Fire(ODEventType.Cache,suffix+Lan.g(nameof(Cache),"Inserting Signals"));
			foreach(InvalidType iType in e.ITypes) {
				Signalod sig=new Signalod();
				sig.IType=iType;
				switch(iType) {
					case InvalidType.Task:
					case InvalidType.TaskPopup:
						sig.FKey=e.TaskNum;
						sig.FKeyType=KeyType.Task;
						break;
					case InvalidType.UserOdPrefs:
						sig.FKey=Security.CurUser?.UserNum??0;
						sig.FKeyType=KeyType.UserOd;
						break;
				}
				Signalods.Insert(sig);
			}
		}

		///<summary>Manipulates the current lightSignalGrid1 control based on the SigMessages passed in. Pass in a null list in order to simply refresh the lightSignalGrid1 control in its current state (no database call).</summary>
		private void FillSignalButtons(List<SigMessage> listSigMessages) {
			if(!DoFillSignalButtons()) {
				return;
			}
			if(_arraySigButDefs==null) {
				_arraySigButDefs=SigButDefs.GetByComputer(ODEnvironment.MachineName);
			}
			int maxButton=_arraySigButDefs.Select(x => x.ButtonIndex).DefaultIfEmpty(-1).Max()+1;
			int lightGridHeightOld=lightSignalGrid1.Height;
			int lightGridHeightNew=Math.Min(maxButton*25+1,this.ClientRectangle.Height-lightSignalGrid1.Location.Y);
			if(lightGridHeightOld!=lightGridHeightNew) {
				lightSignalGrid1.Visible=false;//"erases" light signal grid that has been drawn on FormOpenDental
				LayoutManager.MoveHeight(lightSignalGrid1,lightGridHeightNew);
				lightSignalGrid1.Visible=true;//re-draws light signal grid to the correct size.
			}
			if(listSigMessages==null) {
				return;//No new SigMessages to process.
			}
			SigButDef butDef;
			int row;
			Color color;
			bool hadErrorPainting=false;
			foreach(SigMessage sigMessage in listSigMessages) {
				if(sigMessage.AckDateTime.Year > 1880) {//process ack
					int buttonIndex=lightSignalGrid1.ProcessAck(sigMessage.SigMessageNum);
					if(buttonIndex!=-1) {
						butDef=SigButDefs.GetByIndex(buttonIndex,_arraySigButDefs);
						if(butDef!=null) {
							try {
								PaintOnIcon(butDef.SynchIcon,Color.White);
							}
							catch(Exception ex) {
								ex.DoNothing();
								hadErrorPainting=true;
							}
						}
					}
				}
				else {//process normal message
					row=0;
					color=Color.White;
					List<SigElementDef> listSigElementDefs=SigElementDefs.GetDefsForSigMessage(sigMessage);
					foreach(SigElementDef sigElementDef in listSigElementDefs) {
						if(sigElementDef.LightRow!=0) {
							row=sigElementDef.LightRow;
						}
						if(sigElementDef.LightColor.ToArgb()!=Color.White.ToArgb()) {
							color=sigElementDef.LightColor;
						}
					}
					if(row!=0 && color!=Color.White) {
						lightSignalGrid1.SetButtonActive(row-1,color,sigMessage);
						butDef=SigButDefs.GetByIndex(row-1,_arraySigButDefs);
						if(butDef!=null) {
							try {
								PaintOnIcon(butDef.SynchIcon,color);
							}
							catch(Exception ex) {
								ex.DoNothing();
								hadErrorPainting=true;
							}
						}
					}
				}
			}
			if(hadErrorPainting) {
				MessageBox.Show("Error painting on program icon.  Probably too many non-ack'd messages.");
			}
		}

		///<summary>Refreshes the entire lightSignalGrid1 control to the current state according to the database. This is typically used when the program is first starting up or when a signal is processed for a change to the SigButDef cache.</summary>
		private void FillSignalButtons() {
			if(!DoFillSignalButtons()) {
				return;
			}
			_arraySigButDefs=SigButDefs.GetByComputer(ODEnvironment.MachineName);
			lightSignalGrid1.SetButtons(_arraySigButDefs);
			lightSignalGrid1.Visible=(_arraySigButDefs.Length > 0);
			FillSignalButtons(SigMessages.RefreshCurrentButState());//Get the current SigMessages from the database.
		}

		private bool DoFillSignalButtons() {
			if(!Security.IsUserLoggedIn) {
				return false;
			}
			if(!lightSignalGrid1.Visible && Programs.UsingEcwTightOrFullMode()) {//for faster eCW loading
				return false;
			}
			return true;
		}

		///<summary>Pass in the cellNum as 1-based.</summary>
		private void PaintOnIcon(int cellNum,Color color){
			Graphics g;
			if(_bitmapIcon==null){
				_bitmapIcon=new Bitmap(16,16);
				g=Graphics.FromImage(_bitmapIcon);
				g.FillRectangle(new SolidBrush(Color.White),0,0,15,15);
				//horizontal
				g.DrawLine(Pens.Black,0,0,15,0);
				g.DrawLine(Pens.Black,0,5,15,5);
				g.DrawLine(Pens.Black,0,10,15,10);
				g.DrawLine(Pens.Black,0,15,15,15);
				//vertical
				g.DrawLine(Pens.Black,0,0,0,15);
				g.DrawLine(Pens.Black,5,0,5,15);
				g.DrawLine(Pens.Black,10,0,10,15);
				g.DrawLine(Pens.Black,15,0,15,15);
				g.Dispose();
			}
			if(cellNum==0){
				return;
			}
			g=Graphics.FromImage(_bitmapIcon);
			int x=0;
			int y=0;
			switch(cellNum){
				case 1: x=1; y=1; break;
				case 2: x=6; y=1; break;
				case 3: x=11; y=1; break;
				case 4: x=1; y=6; break;
				case 5: x=6; y=6; break;
				case 6: x=11; y=6; break;
				case 7: x=1; y=11; break;
				case 8: x=6; y=11; break;
				case 9: x=11; y=11; break;
			}
			g.FillRectangle(new SolidBrush(color),x,y,4,4);
			IntPtr intPtr=_bitmapIcon.GetHicon();
			Icon icon=Icon.FromHandle(intPtr);
			Icon=(Icon)icon.Clone();
			DestroyIcon(intPtr);
			icon.Dispose();
			g.Dispose();
		}

		[System.Runtime.InteropServices.DllImport("user32.dll",CharSet = CharSet.Auto)]
		extern static bool DestroyIcon(IntPtr handle);

		private void lightSignalGrid1_ButtonClick(object sender,OpenDental.UI.ODLightSignalGridClickEventArgs e) {
			if(e.ActiveSignal!=null) {//user trying to ack an existing light signal
				//Acknowledge all sigmessages in the database which correspond with the button that was just clicked.
				//Only acknowledge sigmessages which have a MessageDateTime prior to the last time we processed signals in the singal timer.
				//This is so that we don't accidentally acknowledge any sigmessages that we are currently unaware of.
				SigMessages.AckButton(e.ButtonIndex+1,Signalods.SignalLastRefreshed);
				//Immediately update the signal button instead of waiting on our instance to process its own signals.
				e.ActiveSignal.AckDateTime=DateTime.Now;
				FillSignalButtons(new List<SigMessage>() { e.ActiveSignal });//Does not run query.
				return;
			}
			if(e.ButtonDef==null || (e.ButtonDef.SigElementDefNumUser==0 && e.ButtonDef.SigElementDefNumExtra==0 && e.ButtonDef.SigElementDefNumMsg==0)) {
				return;//There is no signal to send.
			}
			//user trying to send a signal
			SigMessage sigMessage=new SigMessage();
			sigMessage.SigElementDefNumUser=e.ButtonDef.SigElementDefNumUser;
			sigMessage.SigElementDefNumExtra=e.ButtonDef.SigElementDefNumExtra;
			sigMessage.SigElementDefNumMsg=e.ButtonDef.SigElementDefNumMsg;
			SigElementDef sigElementDefUser=SigElementDefs.GetElementDef(e.ButtonDef.SigElementDefNumUser);
			if(sigElementDefUser!=null) {
				sigMessage.ToUser=sigElementDefUser.SigText;
			}
			SigMessages.Insert(sigMessage);
			FillSignalButtons(new List<SigMessage>() { sigMessage });//Does not run query.
			//Let the other computers in the office know to refresh this specific light.
			Signalod signal=new Signalod();
			signal.IType=InvalidType.SigMessages;
			signal.FKeyType=KeyType.SigMessage;
			signal.FKey=sigMessage.SigMessageNum;
			Signalods.Insert(signal);
		}

		private void timerTimeIndic_Tick(object sender,System.EventArgs e) {
			//every minute:
			if(WindowState!=FormWindowState.Minimized && controlAppt.Visible) {
				controlAppt.TickRefresh();
			}
		}

		///<summary>Helper method to check if we need to start or stop preprocessing signals</summary>
		private bool IsWorkStationActive() {
			int sigInactiveMin=PrefC.GetInt(PrefName.SignalInactiveMinutes);
			if(sigInactiveMin==0) {
				return true;
			}
			DateTime dateTimeToSignalInactive=Security.DateTimeLastActivity+TimeSpan.FromMinutes(sigInactiveMin);
			if(DateTime.Now > dateTimeToSignalInactive) {
				return false;
			}
			return true;
		}

		///<summary>Checks for shutdown signals while inactive and closes program if any are found</summary>
		private void SignalsTickWhileInactive() {
			DateTime now=MiscData.GetNowDateTime();//update signal to this time after checking
			if(Signalods.DoesNeedToShutDown(_dateTimeSignalShutdownLastChecked)) {
				Logger.WriteLine("Shutdown signal found while inactive. Closing Open Dental.","Signals");
				timerSignals.Stop();
				ProcessKillCommand();
				return;
			}
			if(PrefC.IsODHQ) {//HQ stores important information within the sitelink table and should always process signals for the 'Sites' invalid type.
				if(Signalods.DoesNeedToRefreshSitesCache(_dateTimeSignalShutdownLastChecked)) {
					SiteLinks.RefreshCache();
				}
			}
			_dateTimeSignalShutdownLastChecked=now;
		}

		///<summary>Usually set at 4 to 6 second intervals.</summary>
		private void timerSignals_Tick(object sender,System.EventArgs e) {
			try {
				if(_hasSignalProcessingPaused && !IsWorkStationActive()) {
					SignalsTickWhileInactive();
				}
				else {
					SignalsTick();
				}
			}
			catch(Exception ex) {
				SignalsTickExceptionHandler(ex);
			}
		}

		///<summary>Processes signals.</summary>
		private void SignalsTick() {
			try {
				Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
				//This checks if any forms are open that make us want to continue processing signals even if inactive. Currently only FormTerminal.
				if(Application.OpenForms.OfType<FormTerminal>().Count()==0) {
					//check if we're inactive and if so, pause regular signal processing and set the private shutdown signal check variable
					if(!IsWorkStationActive()) {
						_hasSignalProcessingPaused=true;
						_dateTimeSignalShutdownLastChecked=Signalods.SignalLastRefreshed;
						return;
					}
				}
				if(Security.CurUser==null) {
					//User must be at the log in screen, so no need to process signals. We will need to look for shutdown signals since the last refreshed time when the user attempts to log in.
					_hasSignalProcessingPaused=true;
					return;
				}
				//if signal processing paused due to inactivity or due to Security.CurUser being null (i.e. login screen visible) and we are now going to 
				//process signals again, we will shutdown OD if:
				//1. there is a mismatch between the current software version and the program version stored in the db (ProgramVersion pref)
				//2. the UpdateInProgressOnComputerName pref is set (regardless of whether or not the computer name matches this machine name)
				//3. the CorruptedDatabase flag is set
				if(_hasSignalProcessingPaused) {
					string errorMsg;
					if(!ODBuild.IsDebug() && !IsDbConnectionSafe(out errorMsg)) {//Running version verses ProgramVersion preference can be different in debug.
						timerSignals.Stop();
						MessageBox.Show(this,errorMsg);
						ProcessKillCommand();
						return;
					}
					_hasSignalProcessingPaused=false;
				}
			}
			catch {
				//Currently do nothing.
			}
			#region Task Preprocessing
			if(_userNumTasks!=Security.CurUser.UserNum //The user has changed since the last signal tick was run (when logoff then logon),
				|| _listTasksReminders==null || _listTaskNumsNormal==null)//or first time processing signals since the program started.
			{
				Logger.LogToPath("CurUser change",LogPath.Signals,LogPhase.Start);
				_userNumTasks=Security.CurUser.UserNum;
				List<Task> listRefreshedTasks=Tasks.GetNewTasksThisUser(Security.CurUser.UserNum,Clinics.ClinicNum);//Get all tasks pertaining to current user.
				_listTaskNumsNormal=new List<long>();
				_listTasksReminders=new List<Task>();
				_listTasksRemindersOverLimit=new List<Task>();
				List<UserOdPref> listBlockedTaskLists=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskListBlock);
				if(_dictionaryAllTaskLists==null || listRefreshedTasks.Exists(x => !_dictionaryAllTaskLists.ContainsKey(x.TaskListNum))) {//Refresh dict if needed.
					_dictionaryAllTaskLists=TaskLists.GetAll().ToDictionary(x => x.TaskListNum);
				}
				foreach(Task taskForUser in listRefreshedTasks) {//Construct the initial task meta data for the current user's tasks.
					//If task's taskList is in dictionary and it's archived or has an archived ancestor, ignore it.
					if(_dictionaryAllTaskLists.ContainsKey(taskForUser.TaskListNum)
						&& (_dictionaryAllTaskLists[taskForUser.TaskListNum].TaskListStatus==TaskListStatusEnum.Archived 
						|| TaskLists.IsAncestorTaskListArchived(ref _dictionaryAllTaskLists,_dictionaryAllTaskLists[taskForUser.TaskListNum])))
					{
						continue;
					}
					bool isTrackedByUser=PrefC.GetBool(PrefName.TasksNewTrackedByUser);
					if(String.IsNullOrEmpty(taskForUser.ReminderGroupId)) {//A normal task.
						//Mimics how checkNew is set in FormTaskEdit.
						if((isTrackedByUser && taskForUser.IsUnread) || (!isTrackedByUser && taskForUser.TaskStatus==TaskStatusEnum.New)) {//See def of task.IsUnread
							_listTaskNumsNormal.Add(taskForUser.TaskNum);
						}
					}
					else if(PrefC.GetBool(PrefName.TasksUseRepeating) && taskForUser.DateTimeEntry<=DateTime.Now) {
						if(isTrackedByUser && taskForUser.IsUnread) {
							_listTaskNumsNormal.Add(taskForUser.TaskNum);
						}
						else if(!isTrackedByUser && taskForUser.TaskStatus==TaskStatusEnum.New) {
							_listTaskNumsNormal.Add(taskForUser.TaskNum);
						}
					}
					else if(!PrefC.GetBool(PrefName.TasksUseRepeating)) {//A reminder task (new or viewed).  Reminders not allowed if repeating tasks enabled.
						_listTasksReminders.Add(taskForUser);
						if(taskForUser.DateTimeEntry<=DateTime.Now) {//Do not show reminder popups for future reminders which are not due yet.
							//Mimics how checkNew is set in FormTaskEdit.
							if((isTrackedByUser && taskForUser.IsUnread) || (!isTrackedByUser && taskForUser.TaskStatus==TaskStatusEnum.New)) {//See def of task.IsUnread
								//NOTE: POPUPS ONLY HAPPEN IF THEY ARE MARKED AS NEW. (Also, they will continue to pop up as long as they are marked "new")
								TaskPopupHelper(taskForUser,listBlockedTaskLists);
							}
						}
					}
				}
				//Refresh the appt module to show the current list of reminders, even if the appt module not visible.  This refresh is fast.
				//The user will load the appt module eventually and these refreshes are the only updates the appointment module receives for reminders.
				controlAppt.RefreshReminders(_listTasksReminders);
				_dateReminderRefresh=DateTime.Today;
				Logger.LogToPath("CurUser change",LogPath.Signals,LogPhase.End);
			}
			//Check to see if a reminder task became due between the last signal interval and the current signal interval.
			else if(_listTasksReminders.FindAll(x => x.DateTimeEntry <= DateTime.Now
				&& x.DateTimeEntry >= DateTime.Now.AddSeconds(-PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs))).Count > 0)
			{
				List<Task> listDueReminderTasks=_listTasksReminders.FindAll(x => x.DateTimeEntry <= DateTime.Now
					&& x.DateTimeEntry >= DateTime.Now.AddSeconds(-PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)));
				Logger.LogToPath("Reminder task due",LogPath.Signals,LogPhase.Start);
				List<Signalod> listSignals=new List<Signalod>();
				foreach(Task task in listDueReminderTasks) {
					Signalod sig=new Signalod();
					sig.IType=InvalidType.TaskList;
					sig.FKey=task.TaskListNum;
					sig.FKeyType=KeyType.Undefined;
					listSignals.Add(sig);
				}
				UserControlTasks.RefreshTasksForAllInstances(listSignals);
				List<UserOdPref> listBlockedTaskLists=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskListBlock);
				foreach(Task reminderTask in listDueReminderTasks) {
					TaskPopupHelper(reminderTask,listBlockedTaskLists);
				}
				Logger.LogToPath("Reminder task due",LogPath.Signals,LogPhase.End);
			}
			else if(_listTasksRemindersOverLimit.Count>0) {//Try to display any due reminders that previously exceeded our limit of FormTaskEdit to show.
				List<UserOdPref> listBlockedTaskLists=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskListBlock);
				for(int i=_listTasksRemindersOverLimit.Count-1;i>=0;i--) {//TaskPopupHelper
					TaskPopupHelper(_listTasksRemindersOverLimit[i],listBlockedTaskLists);
				}
			}
			else if(_dateReminderRefresh.Date < DateTime.Today) {
				Logger.LogToPath("Daily reminder refresh is due",LogPath.Signals,LogPhase.Unspecified);
				//Refresh the appt module to show the current list of reminders, even if the appt module not visible.  This refresh is fast.
				//The user will load the appt module eventually and these refreshes are the only updates the appointment module receives for reminders.
				controlAppt.RefreshReminders(_listTasksReminders);
				_dateReminderRefresh=DateTime.Today;
			}
			RefreshTasksNotification();
			#endregion Task Preprocessing
			//Signal Processing
			timerSignals.Stop();
			FormODBase.SignalsTick(
				(value) => this.Invoke(() => InitiateShutdown(value)),
				(listODForms,listSignals) => {
					//Synchronize the thread static Security.CurUser on the main thread if the Userod cache was refreshed.
					if(listSignals.Any(x => ListTools.In(x.IType,InvalidType.Security,InvalidType.AllLocal))) {
						this.Invoke(() => ODException.SwallowAnyException(() => Security.SyncCurUser()));
					}
					//Make a shallow copy of the list of forms that cannot be manipulated while looping through it.
					List<FormODBase> listFormODBases=new List<FormODBase>(listODForms);
					//Broadcast to all subscribed signal processors.
					this.Invoke(() => {
						listFormODBases.ForEach(x => {
							ODException.SwallowAnyException(() => x.ProcessSignals(listSignals));
						});
					});
				},
				() => {
					//For Middle Tier users, it is possible that the processing of signals just failed due to a credential validation failure.
					//E.g. When a user has two instances of Open Dental open and they change the password in one of them.
					//Therefore, we do not want to start up the signal timer if there was a login failure.
					//The signal timer will start back up once RemotingClient.HasLoginFailed has been set back to false (valid log in attempt).
					if(!RemotingClient.HasLoginFailed) {
						this.Invoke(timerSignals.Start);//Either not a ClientWeb instance or the credentials are still valid, continue processing signals.
					}
				}
			);
			//Be careful about doing anything that takes a long amount of computation time after the SignalsTick.
			//The UI will appear invalid for the time it takes any methods to process.
			//Post Signal Processing
			if(PrefC.IsODHQ) {//No actual signals are sent, so this must happen independantly from SignalsTick.
				BeginPhoneConferenceThread();
			}
			//STOP! 
			//If you are trying to do something in FormOpenDental that uses a signal, you should use FormOpenDental.OnProcessSignals() instead.
			//This Function is only for processing things at regular intervals IF IT DOES NOT USE SIGNALS.
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
		}

		///<summary>Called when _hasSignalProcessingPaused is true and we are about to start processing signals again.  We may have missed a shutdown workstations signal, so this method will check the version, the update in progress pref, and the corrupt db pref.  Returns false if the OD
		///instance should be restarted.  The errorMsg out variable will be set to the error message for the first failed check.</summary>
		private bool IsDbConnectionSafe(out string errorMsg) {
			errorMsg="";
			Prefs.RefreshCache();//this is a db call, but will only happen once when an inactive workstation is re-activated
			//The logic below mimics parts of PrefL.CheckProgramVersion().
			Version storedVersion=new Version(PrefC.GetString(PrefName.ProgramVersion));
			Version currentVersion=new Version(Application.ProductVersion);
			if(storedVersion!=currentVersion) {
				errorMsg=Lan.g(this,"You are attempting to run version")+" "+currentVersion.ToString(3)+", "
					+Lan.g(this,"but the database is using version")+" "+storedVersion.ToString(3)+".\r\n\r\n"
					+Lan.g(this,"You will have to restart")+" "+PrefC.GetString(PrefName.SoftwareName)+" "+Lan.g(this,"to correct the version mismatch.");
				return false;
			}
			string updateComputerName=PrefC.GetString(PrefName.UpdateInProgressOnComputerName);
			if(!string.IsNullOrEmpty(updateComputerName)) {
				errorMsg=Lan.g(this,"An update is in progress on workstation")+": '"+updateComputerName+"'.\r\n\r\n"
					+Lan.g(this,"You will have to restart")+" "+PrefC.GetString(PrefName.SoftwareName)+" "+Lan.g(this,"once the update has finished.");
				return false;
			}
			if(PrefC.GetBool(PrefName.CorruptedDatabase)) {
				//only happens if the UpdateInProgressOnComputerName is blank and the CorruptedDatabase flag is set, i.e. an update has failed
				errorMsg=Lan.g(this,"Your database is corrupted because an update failed.  Please contact us.  This database is unusable and you will "
					+"need to restore from a backup.");
				return false;
			}
			return true;
		}

		///<summary>Catches an exception from signal processing and sends the first one to HQ.</summary>
		private void SignalsTickExceptionHandler(Exception ex) {
			//If an exception happens during processing signals, we will not close the program because the user is not trying to do anything. We will
			//send the first exception to HQ.
			if(_exceptionSignalsTick==null) {
				_exceptionSignalsTick=new Exception("SignalsTick exception.",ex);
				ODException.SwallowAnyException(() => {
					BugSubmissions.SubmitException(_exceptionSignalsTick,patNumCur: PatNumCur,moduleName: GetSelectedModuleName());
				});
			}
		}

		///<summary>Adds the alert items to the alert menu item.</summary>
		private void AddAlertsToMenu() {
			//At this point _listAlertItems and _listAlertReads should be user, clinic and subscription filtered.
			//If the counts match this means they have read all AlertItems. 
			//This will result in the 'Alerts' menu item to not be colored.
			int alertCount=_listAlertItems.Count-_listAlertReads.Count;
			if(alertCount>99) {
				_menuItemAlerts.Text=Lan.g(this,"Alerts")+" (99)";
				_menuItemAlerts.ForeColor=Color.Red;
			}
			else if(alertCount==0) {
				_menuItemAlerts.Text=Lan.g(this,"Alerts")+" ("+alertCount+")";
				_menuItemAlerts.ForeColor=Color.Black;
			}
			else{
				_menuItemAlerts.Text=Lan.g(this,"Alerts")+" ("+alertCount+")";
				_menuItemAlerts.ForeColor=Color.Red;
			}
			if(PrefC.IsODHQ) {
				//Disable the Get Conf Room button when the PhoneTrackingServer goes down.
				phoneSmall.SetEnabledStateForControls(!_listAlertItems.Any(x => x.Type==AlertType.AsteriskServerMonitor));
			}
			Logger.LogToPath("CheckAlerts",LogPath.Signals,LogPhase.End);
		}

		///<summary>This only contains UI signal processing. See Signalods.SignalsTick() for cache updates.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignalODs) {
			if(listSignalODs.Exists(x => x.IType==InvalidType.Programs)) {
				RefreshMenuReports();
			}
			if(listSignalODs.Exists(x => x.IType==InvalidType.Prefs)) {
				PrefC.InvalidateVerboseLogging();
			}
			#region SMS Notifications
			Logger.LogToPath("SMS Notifications",LogPath.Signals,LogPhase.Start);
			Signalod signalSmsCount=listSignalODs.OrderByDescending(x => x.SigDateTime)
				.FirstOrDefault(x => x.IType==InvalidType.SmsTextMsgReceivedUnreadCount && x.FKeyType==KeyType.SmsMsgUnreadCount);
			if(signalSmsCount!=null) {
				//Provide the pre-existing value here. This will act as a flag indicating that we should not resend the signal.  This would cause infinite signal loop.
				SetSmsNotificationText(signalSmsCount);
			}
			Logger.LogToPath("SMS Notifications",LogPath.Signals,LogPhase.End);
			#endregion SMS Notifications
			#region Tasks
			List<Signalod> listSignalTasks=listSignalODs.FindAll(x => x.IType==InvalidType.Task || x.IType==InvalidType.TaskPopup 
				|| x.IType==InvalidType.TaskList || x.IType==InvalidType.TaskAuthor || x.IType==InvalidType.TaskPatient);
			List<long> listEditedTaskNums=listSignalTasks.FindAll(x => x.FKeyType==KeyType.Task).Select(x => x.FKey).ToList();
			BeginTasksThread(listSignalTasks,listEditedTaskNums);	
			#endregion Tasks
			#region Appointment Module
			if(controlAppt.Visible) {
				List<long> listOpNumsVisible =controlAppt.GetListOpsVisible().Select(x => x.OperatoryNum).ToList();
				List<long> listProvNumsVisible =controlAppt.GetListProvsVisible().Select(x => x.ProvNum).ToList();
				bool isRefreshAppts=Signalods.IsApptRefreshNeeded(controlAppt.GetDateSelected().Date,listSignalODs,listOpNumsVisible,listProvNumsVisible);
				bool isRefreshScheds=Signalods.IsSchedRefreshNeeded(controlAppt.GetDateSelected().Date,listSignalODs,listOpNumsVisible,listProvNumsVisible);
				bool isRefreshPanelButtons=Signalods.IsContrApptButtonRefreshNeeded(listSignalODs);
				if(isRefreshAppts || isRefreshScheds) {
					Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.Start);
					controlAppt.RefreshPeriod(isRefreshAppointments:isRefreshAppts,isRefreshSchedules:isRefreshScheds);
					Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.End);
				}
				if(isRefreshPanelButtons) {
					controlAppt.RefreshModuleScreenButtonsRight();
				}
			}
			Signalod signalTP=listSignalODs.FirstOrDefault(x => x.IType==InvalidType.TPModule && x.FKeyType==KeyType.PatNum);
			if(controlTreat.Visible && signalTP!=null && signalTP.FKey==controlTreat.PatCur.PatNum){
				RefreshCurrentModule();
			}
			Signalod signalPP=listSignalODs.FirstOrDefault(x => x.IType==InvalidType.AccModule && x.FKeyType==KeyType.PatNum);
			if(controlAccount.Visible && signalPP!=null && signalPP.FKey==controlAccount.PatCur.PatNum) {
				RefreshCurrentModule();
			}
			#endregion Appointment Module
			#region Unfinalize Pay Menu Update
			UpdateUnfinalizedPayCount(listSignalODs.FindAll(x => x.IType==InvalidType.UnfinalizedPayMenuUpdate));
			#endregion Unfinalize Pay Menu Update
			#region eClipboard/Kiosk
			if(listSignalODs.Exists(x => x.IType==InvalidType.EClipboard)) {
				EClipboardEvent.Fire(ODEventType.eClipboard);
			}
			#endregion
			#region Refresh
			InvalidType[] arrInvalidTypes=Signalods.GetInvalidTypes(listSignalODs);
			if(arrInvalidTypes.Length > 0) {
				RefreshLocalDataPostCleanup(arrInvalidTypes);
			}
			#endregion Refresh
			//Sig Messages must be the last code region to run in the process signals method because it changes the application icon.
			#region Sig Messages (In the manual as "Internal Messages")
			//Check to see if any signals are sigmessages.
			List<long> listSigMessageNums=listSignalODs.FindAll(x => x.IType==InvalidType.SigMessages && x.FKeyType==KeyType.SigMessage).Select(x => x.FKey).ToList();
			if(listSigMessageNums.Count>0) {
				Logger.LogToPath("SigMessages",LogPath.Signals,LogPhase.Start);
				//Any SigMessage iType means we need to refresh our lights or buttons.
				List<SigMessage> listSigMessages=SigMessages.GetSigMessages(listSigMessageNums);
				controlManage.LogMsgs(listSigMessages);
				FillSignalButtons(listSigMessages);
				//Need to add a test to this: do not play messages that are over 2 minutes old.
				BeginPlaySoundsThread(listSigMessages);
				Logger.LogToPath("SigMessages",LogPath.Signals,LogPhase.End);
			}
			#endregion Sig Messages
			Plugins.HookAddCode(this,"FormOpenDental.ProcessSignals_end",listSignalODs);
		}
		#endregion Signals

		#region Tasks
		///<summary>Will invoke a refresh of tasks on the only instance of FormOpenDental. listRefreshedTaskNotes and listBlockedTaskLists are only used 
		///for Popup tasks, only used if listRefreshedTasks includes at least one popup task.</summary>
		public static void S_HandleRefreshedTasks(List<Signalod> listSignalTasks,List<long> listEditedTaskNums,List<Task> listRefreshedTasks,
			List<TaskNote> listRefreshedTaskNotes,List<UserOdPref> listBlockedTaskLists) 
		{
			_formOpenDentalSingleton.HandleRefreshedTasks(listSignalTasks,listEditedTaskNums,listRefreshedTasks,listRefreshedTaskNotes,listBlockedTaskLists);
		}

		///<summary>Refreshes tasks and pops up as necessary. Invoked from thread callback in OnProcessSignals(). listRefreshedTaskNotes and 
		///listBlockedTaskLists are only used for Popup tasks, only used if listRefreshedTasks includes at least one popup task.</summary>
		private void HandleRefreshedTasks(List<Signalod> listSignalTasks,List<long> listEditedTaskNums,List<Task> listRefreshedTasks,
			List<TaskNote> listRefreshedTaskNotes,List<UserOdPref> listBlockedTaskLists) 
		{
			bool hasChangedReminders=UpdateTaskMetaData(listEditedTaskNums,listRefreshedTasks);
			RefreshTasksNotification();
			RefreshOpenTasksOrPopupNewTasks(listSignalTasks,listRefreshedTasks,listRefreshedTaskNotes,listBlockedTaskLists);
			//Refresh the appt module if reminders have changed, even if the appt module not visible.
			//The user will load the appt module eventually and these refreshes are the only updates the appointment module receives for reminders.
			if(hasChangedReminders) { 
				controlAppt.RefreshReminders(_listTasksReminders);
				_dateReminderRefresh=DateTime.Today;
			}
		}

		///<summary>Updates the class-wide meta data used for updating the task notification UI elements.
		///Returns true if a reminder task has changed.  Otherwise; false.</summary>
		private bool UpdateTaskMetaData(List<long> listEditedTaskNums,List<Task> listRefreshedTasks) {
			//Check to make sure there are edited task nums passed in and that the meta data lists have been initialized by the signal processor.
			if(listEditedTaskNums==null || _listTasksReminders==null || _listTaskNumsNormal==null) {
				return false;//Nothing to do.
			}
			bool hasChangedReminders=false;
			for(int i=0;i<listEditedTaskNums.Count;i++) {//Update the task meta data for the current user based on the query results.
				long editedTaskNum=listEditedTaskNums[i];//The tasknum mentioned in the signal.
				Task taskForUser=listRefreshedTasks?.FirstOrDefault(x => x.TaskNum==editedTaskNum);
				Task taskNewForUser=null;
				if(taskForUser!=null) {
					bool isTrackedByUser=PrefC.GetBool(PrefName.TasksNewTrackedByUser);
					//Mimics how checkNew is set in FormTaskEdit.
					if(((isTrackedByUser && taskForUser.IsUnread) || (!isTrackedByUser && taskForUser.TaskStatus==TaskStatusEnum.New))//See def of task.IsUnread
						//Reminders not due yet are excluded from Tasks.RefreshUserNew().
						&& (string.IsNullOrEmpty(taskForUser.ReminderGroupId) || taskForUser.DateTimeEntry<=DateTime.Now)) 
					{
						taskNewForUser=taskForUser;
					}
				}
				Task taskReminderOld=_listTasksReminders.FirstOrDefault(x => x.TaskNum==editedTaskNum);
				if(taskReminderOld!=null) {//The task is a reminder which is relevant to the current user.
					hasChangedReminders=true;
					_listTasksReminders.RemoveAll(x => x.TaskNum==editedTaskNum);//Remove the old copy of the task.
					if(taskForUser!=null) {//The updated reminder task is relevant to the current user.
						_listTasksReminders.Add(taskForUser);//Add the updated reminder task into the list (replacing the old reminder task).
					}
				}
				else if(_listTaskNumsNormal.Contains(editedTaskNum)) {//The task is a normal task which is relevant to the current user.
					if(taskNewForUser==null) {//But now the task is no longer relevant to the user.
						_listTaskNumsNormal.Remove(editedTaskNum);
					}
				}
				else {//The edited tasknum is not currently in our meta data.
					if(taskNewForUser!=null && String.IsNullOrEmpty(taskNewForUser.ReminderGroupId)) {//A new normal task has now become relevant.
						_listTaskNumsNormal.Add(editedTaskNum);
					}
					else if(taskForUser!=null && !String.IsNullOrEmpty(taskForUser.ReminderGroupId)) {//A reminder task has become relevant (new or viewed)
						hasChangedReminders=true;
						_listTasksReminders.Add(taskForUser);
					}
				}//else
			}//for
			return hasChangedReminders;
		}

		private void RefreshOpenTasksOrPopupNewTasks(List<Signalod> listSignalTasks,List<Task> listRefreshedTasks,List<TaskNote> listRefreshedTaskNotes,
			List<UserOdPref> listBlockedTaskLists) 
		{
			if(listSignalTasks==null) {
				return;//Nothing to do if there was no signal sent which means no task has been flagged as needing to be refreshed.
			}
			List<long> listSignalTasksNums=listSignalTasks.Select(x => x.FKey).ToList();
			List<long> listTaskNumsOpen=new List<long>();
			for(int i=0;i<Application.OpenForms.Count;i++) {
				Form form=Application.OpenForms[i];
				if(!(form is FormTaskEdit)) {
					continue;
				}
				FormTaskEdit FormTE=(FormTaskEdit)form;
				if(listSignalTasksNums.Contains(FormTE.TaskNumCur)) {
					FormTE.OnTaskEdited();
					listTaskNumsOpen.Add(FormTE.TaskNumCur);
				}
			}
			List<Task> tasksPopup=new List<Task>();
			if(listRefreshedTasks!=null) {
				for(int i=0;i<listRefreshedTasks.Count;i++) {//Locate any popup tasks in the returned list of tasks.
					//Verify the current task is a popup task.
					if(!listSignalTasks.Exists(x => x.FKeyType==KeyType.Task && x.IType==InvalidType.TaskPopup && x.FKey==listRefreshedTasks[i].TaskNum)
						|| listTaskNumsOpen.Contains(listRefreshedTasks[i].TaskNum))
					{
						continue;//Not a popup task or is already open.
					}
					tasksPopup.Add(listRefreshedTasks[i]);
				}
			}
			for(int i=0;i<tasksPopup.Count;i++) {
				//Reminders sent to a subscribed tasklist will pop up prior to the reminder date/time.
				TaskPopupHelper(tasksPopup[i],listBlockedTaskLists,listRefreshedTaskNotes?.FindAll(x => x.TaskNum==tasksPopup[i].TaskNum));
			}
			if(listSignalTasks.Count > 0 || tasksPopup.Count>0) {
				UserControlTasks.RefreshTasksForAllInstances(listSignalTasks);
			}
		}

		///<summary>Takes one task and determines if it should popup for the current user.  Displays task popup if needed.</summary>
		private void TaskPopupHelper(Task taskPopup,List<UserOdPref> listBlockedTaskLists,List<TaskNote> listNotesForTask=null) {
			try {
				//Check if application is in kiosk mode. If so, no popups should happen. 
				if(Application.OpenForms.OfType<FormTerminal>().Count()>0) {
					string msg=Lan.g(this,"Kiosk mode enabled, popup blocked for TaskNum:");
					Logger.LogToPath("",LogPath.Signals,LogPhase.Start,msg+" "+POut.Long(taskPopup.TaskNum));
					return;
				} 
				Logger.LogToPath("",LogPath.Signals,LogPhase.Start,"TaskNum: "+taskPopup.TaskNum.ToString());
				if(taskPopup.DateTimeEntry>DateTime.Now && taskPopup.ReminderType!=TaskReminderType.NoReminder) {
					return;//Don't pop up future dated reminder tasks
				}
				//Don't pop up reminders if we reach our upper limit of open FormTaskEdit windows to avoid overwhelming users with popups.
				//Add the task to another list that temporarily holds the reminder task until it is allowed to popup.
				if(taskPopup.ReminderType!=TaskReminderType.NoReminder) {//Is a reminder task.
					if(Application.OpenForms.OfType<FormTaskEdit>().ToList().Count>=_popupPressureReliefLimit){//Open Task Edit windows over display limit.
						if(!_listTasksRemindersOverLimit.Exists(x => x.TaskNum==taskPopup.TaskNum)) {
							_listTasksRemindersOverLimit.Add(taskPopup);//Add to list to be shown later to prevent too many windows from being open at same time.
						}
						return;//We are over the display limit for now.   Will try again later after user closes some Task Edit windows.
					}
					_listTasksRemindersOverLimit.RemoveAll(x => x.TaskNum==taskPopup.TaskNum);//Remove from list if present.
				}
				//Even though this is triggered to popup, if this is my own task, then do not popup.
				List<TaskNote> notesForThisTask=(listNotesForTask??TaskNotes.GetForTask(taskPopup.TaskNum)).OrderBy(x => x.DateTimeNote).ToList();
				if(taskPopup.ReminderType==TaskReminderType.NoReminder) {//We care about notes and task sender only if it's not a reminder.
					if(notesForThisTask.Count==0) {//'sender' is the usernum on the task and it's not a reminder
						if(taskPopup.UserNum==Security.CurUser.UserNum) {
							return;
						}
					}
					else {//'sender' is the user on the last added note
						if(notesForThisTask[notesForThisTask.Count-1].UserNum==Security.CurUser.UserNum) {
							return;
						}
					}
				}
				if(RemotingClient.RemotingRole==RemotingRole.ClientWeb && !Security.IsUserLoggedIn) {//User isn't actually logged in, so don't popup a task on their computer.
					return;
				}
				List<TaskList> listUserTaskListSubsTrunk=TaskLists.RefreshUserTrunk(Security.CurUser.UserNum);//Get the list of directly subscribed tasklists.
				List<long> listUserTaskListSubNums=listUserTaskListSubsTrunk.Select(x => x.TaskListNum).ToList();
				bool isUserSubscribed=listUserTaskListSubNums.Contains(taskPopup.TaskListNum);//First check if user is directly subscribed.
				if(!isUserSubscribed) {
					isUserSubscribed=listUserTaskListSubsTrunk.Any(x => TaskLists.IsAncestor(x.TaskListNum,taskPopup.TaskListNum));//Check ancestors for subscription.
				}
				if(isUserSubscribed) {//User is subscribed to this TaskList, or one of its ancestors.
					byte[] rawData=new byte[Properties.Resources.notify.Length];
					Properties.Resources.notify.Read(rawData,0,rawData.Length);
					if(!listBlockedTaskLists.Any(x => x.Fkey==taskPopup.TaskListNum && PIn.Bool(x.ValueString))){//Subscribed and Unblocked, Show it!
						SoundHelper.PlaySound(rawData);
						FormTaskEdit FormT=new FormTaskEdit(taskPopup);
						FormT.IsPopup=true;
						if(taskPopup.ReminderType!=TaskReminderType.NoReminder) {//If a reminder task, make an audit trail entry
							Tasks.TaskEditCreateLog(Permissions.TaskReminderPopup,$"Reminder task {taskPopup.TaskNum} shown to user",taskPopup);
						}
						FormT.Show();//non-modal
						FormT.BringToFront();//Bring these tasks in front of the main window, so they don't disappear when one is closed.
					}
					else {
						UserOdPref userOdPrefTaskSound=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskBlockedMakeSound);
						if(!userOdPrefTaskSound.IsNew && PIn.Bool(userOdPrefTaskSound.ValueString)) {
							SoundHelper.PlaySound(rawData);
						}
					}
				}
			}
			finally {
				Logger.LogToPath("",LogPath.Signals,LogPhase.End,"TaskNum: "+taskPopup.TaskNum.ToString());
			}
		}
		#endregion Tasks

		#region Modules
		private void GotoModule_ModuleSelected(ModuleEventArgs e){
			//patient can also be set separately ahead of time instead of doing it this way:
			if(e.PatNum!=0) {
				if(e.PatNum!=PatNumCur) { //Currently selected patient changed.
					PatNumCur=e.PatNum;
					//Going to Chart Module, to specifically handle the SendToMeCreateTask_Click in FormVoiceMails to make sure Patient tab refreshes.
					if(PrefC.IsODHQ && e.ModuleType==EnumModuleType.Chart) { 
						UserControlTasks.RefreshTasksForAllInstances(null,UserControlTasksTab.PatientTickets);//Force a refresh on Task area or Triage.
					}
				}
				Patient pat=Patients.GetPat(PatNumCur);
				FillPatientButton(pat);
			}
			UnselectActive();
			AllNeutral();
			if(e.ClaimNum>0){
				moduleBar.SelectedModule=e.ModuleType;
				controlAccount.Visible=true;
				this.ActiveControl=this.controlAccount;
				controlAccount.ModuleSelected(PatNumCur,e.ClaimNum);
			}
			else if(e.ListPinApptNums.Count!=0){
				moduleBar.SelectedModule=e.ModuleType;
				controlAppt.Visible=true;
				this.ActiveControl=this.controlAppt;
				controlAppt.ModuleSelectedWithPinboard(PatNumCur,e.ListPinApptNums,e.DateSelected);
			}
			else if(e.SelectedAptNum!=0){
				moduleBar.SelectedModule=e.ModuleType;
				controlAppt.Visible=true;
				this.ActiveControl=this.controlAppt;
				controlAppt.ModuleSelectedGoToAppt(e.SelectedAptNum,e.DateSelected);
			}
			else if(e.DocNum>0) {
				if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
					moduleBar.SelectedModule=e.ModuleType;
					controlImages.InitializeOnStartup();
					controlImages.Visible=true;
					this.ActiveControl=this.controlImages;
					controlImages.ModuleSelected(PatNumCur,e.DocNum);
				}
				else{
					moduleBar.SelectedModule=e.ModuleType;
					controlImagesJ.InitializeOnStartup();
					controlImagesJ.Visible=true;
					this.ActiveControl=this.controlImagesJ;
					controlImagesJ.ModuleSelected(PatNumCur,e.DocNum);
				}
			}
			else if(e.ModuleType!=EnumModuleType.None){
				moduleBar.SelectedModule=e.ModuleType;
				SetModuleSelected();
			}
			moduleBar.Invalidate();
		}

		private void moduleBar_ButtonClicked(object sender, OpenDental.ButtonClicked_EventArgs e){
			switch(moduleBar.SelectedModule){
				case EnumModuleType.Appointments:
					if(!Security.IsAuthorized(Permissions.AppointmentsModule)){
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.Family:
					if(PrefC.GetBool(PrefName.EhrEmergencyNow)) {//if red emergency button is on
						if(Security.IsAuthorized(Permissions.EhrEmergencyAccess,true)) {
							break;//No need to check other permissions.
						}
					}
					//Whether or not they were authorized by the special situation above,
					//they can get into the Family module with the ordinary permissions.
					if(!Security.IsAuthorized(Permissions.FamilyModule)) {
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.Account:
					if(!Security.IsAuthorized(Permissions.AccountModule)){
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.TreatPlan:
					if(!Security.IsAuthorized(Permissions.TPModule)){
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.Chart:
					if(!Security.IsAuthorized(Permissions.ChartModule)){
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.Imaging:
					if(!Security.IsAuthorized(Permissions.ImagingModule)){
						e.Cancel=true;
						return;
					}
					break;
				case EnumModuleType.Manage:
					if(!Security.IsAuthorized(Permissions.ManageModule)){
						e.Cancel=true;
						return;
					}
					break;
			}
			UnselectActive();
			AllNeutral();
			SetModuleSelected(true);
		}

		///<summary>Returns the translated name of the currently selected module for logging bug submissions.</summary>
		public string GetSelectedModuleName() {
			try {
				return moduleBar.SelectedModule.ToString();//.Buttons[moduleBar.SelectedIndex].Caption;
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "";
			}
		}

		///<summary>Sets the currently selected module based on the selectedIndex of the outlook bar. If selectedIndex is -1, which might happen if user does not have permission to any module, then this does nothing.</summary>
		private void SetModuleSelected() {
			SetModuleSelected(false);
		}

		///<summary>Sets the currently selected module based on the selectedIndex of the outlook bar. If selectedIndex is -1, which might happen if user does not have permission to any module, then this does nothing. The menuBarClicked variable should be set to true when a module button is clicked, and should be false when called for refresh purposes.</summary>
		private void SetModuleSelected(bool menuBarClicked){
			switch(moduleBar.SelectedModule){
				case EnumModuleType.Appointments:
					controlAppt.InitializeOnStartup();
					controlAppt.Visible=true;
					this.ActiveControl=this.controlAppt;
					controlAppt.ModuleSelected(PatNumCur);
					break;
				case EnumModuleType.Family:
					if(HL7Defs.IsExistingHL7Enabled()) {
						HL7Def def=HL7Defs.GetOneDeepEnabled();
						if(def.ShowDemographics==HL7ShowDemographics.Hide) {
							controlFamilyEcw.Visible=true;
							this.ActiveControl=this.controlFamilyEcw;
							controlFamilyEcw.ModuleSelected(PatNumCur);
						}
						else {
							controlFamily.InitializeOnStartup();
							controlFamily.Visible=true;
							this.ActiveControl=this.controlFamily;
							controlFamily.ModuleSelected(PatNumCur);
						}
					}
					else {
						if(Programs.UsingEcwTightMode()) {
							controlFamilyEcw.Visible=true;
							this.ActiveControl=this.controlFamilyEcw;
							controlFamilyEcw.ModuleSelected(PatNumCur);
						}
						else {
							controlFamily.InitializeOnStartup();
							controlFamily.Visible=true;
							this.ActiveControl=this.controlFamily;
							controlFamily.ModuleSelected(PatNumCur);
						}
					}
					break;
				case EnumModuleType.Account:
					controlAccount.InitializeOnStartup();
					controlAccount.Visible=true;
					this.ActiveControl=this.controlAccount;
					controlAccount.ModuleSelected(PatNumCur);
					break;
				case EnumModuleType.TreatPlan:
					controlTreat.InitializeOnStartup();
					controlTreat.Visible=true;
					this.ActiveControl=this.controlTreat;
					if(menuBarClicked) {
						controlTreat.ModuleSelected(PatNumCur,true);//Set default date to true when button is clicked.
					}
					else {
						controlTreat.ModuleSelected(PatNumCur);
					}
					break;
				case EnumModuleType.Chart:
					controlChart.InitializeOnStartup();
					controlChart.Visible=true;
					this.ActiveControl=this.controlChart;
					if(menuBarClicked) {
						controlChart.ModuleSelectedErx(PatNumCur);
					}
					else {
						controlChart.ModuleSelected(PatNumCur,true);
					}
					TryNonPatientPopup();
					break;
				case EnumModuleType.Imaging:
					if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
						controlImages.InitializeOnStartup();
						controlImages.Visible=true;
						this.ActiveControl=this.controlImages;
						controlImages.ModuleSelected(PatNumCur);
					}
					else{
						controlImagesJ.InitializeOnStartup();
						controlImagesJ.Visible=true;
						this.ActiveControl=this.controlImagesJ;
						controlImagesJ.ModuleSelected(PatNumCur);
					}
					break;
				case EnumModuleType.Manage:
					//ContrManage2.InitializeOnStartup();//This gets done earlier.
					controlManage.Visible=true;
					this.ActiveControl=this.controlManage;
					controlManage.ModuleSelected(PatNumCur);
					break;
			}
		}

		private void AllNeutral(){
			controlAppt.Visible=false;
			controlFamily.Visible=false;
			controlFamilyEcw.Visible=false;
			controlAccount.Visible=false;
			controlTreat.Visible=false;
			controlChart.Visible=false;
			if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
				controlImages.Visible=false;
			}
			else{
				if(controlImagesJ!=null){//can be null on startup
					controlImagesJ.Visible=false;
				}
			}
			controlManage.Visible=false;
		}

		private void UnselectActive(bool isLoggingOff=false){
			if(controlAppt.Visible){
				controlAppt.ModuleUnselected();
			}
			if(controlFamily.Visible){
				controlFamily.ModuleUnselected();
			}
			if(controlFamilyEcw.Visible) {
				//ContrFamily2Ecw.ModuleUnselected();
			}
			if(controlAccount.Visible){
				controlAccount.ModuleUnselected();
			}
			if(controlTreat.Visible){
				controlTreat.ModuleUnselected();
			}
			if(controlChart.Visible){
				controlChart.ModuleUnselected(isLoggingOff);
			}
			if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
				if(controlImages.Visible){
					controlImages.ModuleUnselected();
				}
			}
			else{
				if(controlImagesJ.Visible){
					controlImagesJ.ModuleUnselected();
				}
			}
		}

		///<Summary>This also passes CurPatNum down to the currently selected module (except the Manage module).  If calling from ContrAppt and RefreshModuleDataPatient was called before calling this method, set isApptRefreshDataPat=false so the get pat query isn't run twice.</Summary>
		private void RefreshCurrentModule(bool hasForceRefresh=false,bool isApptRefreshDataPat=true,bool isClinicRefresh=true,long docNum=0){
			if(controlAppt.Visible){
				if(hasForceRefresh) {
					controlAppt.ModuleSelected(PatNumCur);
				}
				else {
					if(isApptRefreshDataPat) {//don't usually skip data refresh, only if CurPatNum was set just prior to calling this method
						controlAppt.RefreshModuleDataPatient(PatNumCur);
					}
					controlAppt.RefreshModuleScreenButtonsRight();
				}
			}
			if(controlFamily.Visible){
				controlFamily.ModuleSelected(PatNumCur);
			}
			if(controlFamilyEcw.Visible) {
				controlFamilyEcw.ModuleSelected(PatNumCur);
			}
			if(controlAccount.Visible){
				controlAccount.ModuleSelected(PatNumCur);
			}
			if(controlTreat.Visible){
				controlTreat.ModuleSelected(PatNumCur);
			}
			if(controlChart.Visible){
				controlChart.ModuleSelected(PatNumCur,isClinicRefresh);
			}
			if(PrefC.GetBoolSilent(PrefName.ImagesModuleUsesOld2020,false)){
				if(controlImages.Visible){
					controlImages.ModuleSelected(PatNumCur,docNum);
				}
			}
			else{
				if(controlImagesJ.Visible){
					controlImagesJ.ModuleSelected(PatNumCur,docNum);
				}
			}
			if(controlManage.Visible){
				controlManage.ModuleSelected(PatNumCur);
			}
			userControlTasks1.RefreshPatTicketsIfNeeded();
		}
		#endregion Modules

		#region DataConnection
		///<summary>Decrypt the connection string and try to connect to the database directly. Only called if using a connection string and ChooseDatabase is not to be shown. Must call GetOraConfig first.</summary>
		public bool TryWithConnStr() {
			DataConnection dcon=new DataConnection();
			try {
				//a direct connection does not utilize lower privileges.
				RemotingClient.RemotingRole=RemotingRole.ClientDirect;
				return true;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		///<summary>This method stops all (local) timers and displays a connection lost window that will let users attempt to reconnect.
		///At any time during the lifespan of the application connection to the database can be lost for unknown reasons.
		///When anything spawned by FormOpenDental (main thread) tries to connect to the database and fails, this event will get fired.</summary>
		private void DataConnection_ConnectionLost(DataConnectionEventArgs e) {
			if(InvokeRequired) {
				this.BeginInvoke(() => DataConnection_ConnectionLost(e));
				return;
			}
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				return;
			}
			if(e==null || e.EventType!=ODEventType.DataConnection || e.IsConnectionRestored) {
				return;
			}
			BeginDataConnectionLostThread(e);
		}

		///<summary></summary>
		private void CrashedTable_Detected(CrashedTableEventArgs e) {
			if(InvokeRequired) {
				this.BeginInvoke(() => CrashedTable_Detected(e));
				return;
			}
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				return;
			}
			if(e==null || e.EventType!=ODEventType.CrashedTable || !e.IsTableCrashed) {
				return;
			}
			BeginCrashedTableMonitorThread(e);
		}

		///At any time during the lifespan of the application connection to the Middle Tier server can be lost for unknown reasons.
		///When anything spawned by FormOpenDental (main thread) tries to connect to the MiddleTier server and fails, this event will fire.</summary>
		private void MiddleTierConnection_ConnectionLost(MiddleTierConnectionEventArgs e) {
			if(InvokeRequired) {
				this.BeginInvoke(() => MiddleTierConnection_ConnectionLost(e));
				return;
			}
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				return;
			}
			if(e==null || e.EventType!=ODEventType.MiddleTierConnection || e.IsConnectionRestored) {
				return;
			}
			BeginMiddleTierConnectionMonitorThread(e);
		}

		///<summary>This method stops all (local) timers and displays a bad credentials window that will let users attempt to login again.  This is to
		///handle the situation where a user is logged into multiple computers via middle tier and changes their password on 1 connection.  The other
		///connection(s) would attempt to access the database using the old password (for signal refreshes etc) and lock the user's account for too many
		///failed attempts.  FormLoginFailed will not allow a different user to login, only the current user or exit the program.</summary>
		private void DataConnection_CredentialsFailedAfterLogin(ODEventArgs e) {
			if(InvokeRequired) {
				this.BeginInvoke(() => DataConnection_CredentialsFailedAfterLogin(e));
				return;
			}
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				return;
			}
			if(e!=null && e.EventType!=ODEventType.ServiceCredentials) {
				return;
			}
			if(Security.CurUser==null) {
				Environment.Exit(0);//shouldn't be possible, would have to have a user logged in to get here, but just in case, exit the program
			}
			if(RemotingClient.HasLoginFailed || (_formLoginFailed!=null && !_formLoginFailed.IsDisposed)) {//_formLoginFailed already displayed, wait for _formLoginFailed to close
				return;
			}
			RemotingClient.HasLoginFailed=true;//first thread to get the lock (or invoke this method so the main thread gets the lock) will display the login form
			try {
				SetTimersAndThreads(false);//Safe to stop timers since this method was invoked on the main thread if required.
				Security.IsUserLoggedIn=false;
				string errorMsg=(string)e.Tag;
				_formLoginFailed=new FormLoginFailed(errorMsg);
				_formLoginFailed.ShowDialog();
				if(_formLoginFailed.DialogResult==DialogResult.Cancel) {
					Environment.Exit(0);
				}
				SetTimersAndThreads(true);//Safe to start timers since this method was invoked on the main thread if required.
				Security.DateTimeLastActivity=DateTime.Now;
				_formLoginFailed.Dispose();
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw;
			}
			finally {
				RemotingClient.HasLoginFailed=false;
				_formLoginFailed=null;
			}
		}

		///<summary></summary>
		private void DataReaderNull_Detected(DataReaderNullEventArgs e) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				return;
			}
			if(e==null || e.IsQuerySuccessful) {
				return;
			}
			BeginDataReaderNullMonitorThread(e);
		}
		#endregion DataConnection

		#region Phone Panel
		private void comboTriageCoordinator_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTriageCoordinator.SelectedIndex<0) {
				return;
			}
			ODException.SwallowAnyException(() => {
				if(SiteLinks.UpdateTriageCoordinator(SiteLinks.GetSiteLinkByGateway().SiteLinkNum
					,comboTriageCoordinator.GetSelected<Employee>().EmployeeNum))
				{
					DataValid.SetInvalid(InvalidType.Sites);
				}
			});
		}

		public static void S_TaskGoTo(TaskObjectType taskOT,long keyNum) {
			_formOpenDentalSingleton.TaskGoTo(taskOT,keyNum);
		}

		private bool IsPatInRestrictedClinic(long patNum) {
			if(!PrefC.HasClinicsEnabled) {
				return false;
			}
			if(Security.IsAuthorized(Permissions.UnrestrictedSearch,suppressMessage:true)){
				return false;
			}
			List<long> listUserClinicNums = Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
			Patient patient = Patients.GetLim(patNum);
			if(ListTools.In(patient.ClinicNum,listUserClinicNums) 
				|| Appointments.GetAppointmentsForPat(patNum).Select(x => x.ClinicNum).Any(x => ListTools.In(x,listUserClinicNums))) {
				return false;
			}
			return true;
		}

	private void TaskGoTo(TaskObjectType taskOT,long keyNum){
			if(taskOT==TaskObjectType.None || keyNum==0) {
				return;
			}
			if(taskOT==TaskObjectType.Patient) {
				if(IsPatInRestrictedClinic(keyNum)) {
					MsgBox.Show(this,"This patient is assigned to a clinic that you are not authorized for. Contact an Administrator to grant you access or to " +
					"create an appointment in your clinic to avoid patient duplication.");
					return;
				}
				PatNumCur=keyNum;
				Patient pat=Patients.GetPat(PatNumCur);
				RefreshCurrentModule();
				FillPatientButton(pat);
			}
			if(taskOT==TaskObjectType.Appointment) {
				Appointment apt=Appointments.GetOneApt(keyNum);
				if(apt==null) {
					MsgBox.Show(this,"Appointment has been deleted, so it's not available.");
					return;
				}
				if(IsPatInRestrictedClinic(apt.PatNum)) {
					MsgBox.Show(this,"This patient is assigned to a clinic that you are not authorized for. Contact an Administrator to grant you access or to " +
					"create an appointment in your clinic to avoid patient duplication.");
					return;
				}
				DateTime dateSelected=DateTime.MinValue;
				if(apt.AptStatus==ApptStatus.Planned || apt.AptStatus==ApptStatus.UnschedList) {
					//I did not add feature to put planned or unsched apt on pinboard.
					MsgBox.Show(this,"Cannot navigate to appointment.  Use the Other Appointments button.");
					//return;
					DateTime dateTemp = controlAppt.GetDateSelected();
					if(dateTemp == null || dateTemp == DateTime.MinValue) {
						dateSelected=DateTime.Now;
					}
					dateSelected=dateTemp;
				}
				else {
					dateSelected=apt.AptDateTime;
				}
				PatNumCur=apt.PatNum;//OnPatientSelected(apt.PatNum);
				FillPatientButton(Patients.GetPat(PatNumCur));
				GotoModule.GotoAppointment(dateSelected,apt.AptNum);
			}
		}

		private void butBigPhones_Click(object sender,EventArgs e) {
			if(_formPhoneTiles==null || _formPhoneTiles.IsDisposed) {
				_formPhoneTiles=new FormPhoneTiles();
				_formPhoneTiles.GoToPatient += new System.EventHandler(this.phonePanel_GoToChanged);
				_formPhoneTiles.Show();
				Rectangle rect=System.Windows.Forms.Screen.FromControl(this).Bounds;
				_formPhoneTiles.Location=new Point((rect.Width-_formPhoneTiles.Width)/2+rect.X,0);
				_formPhoneTiles.BringToFront();
			}
			else {
				if(_formPhoneTiles.WindowState==FormWindowState.Minimized) {
					_formPhoneTiles.WindowState=FormWindowState.Normal;
				}
				_formPhoneTiles.Show();
				_formPhoneTiles.BringToFront();
			}
		}

		private void butMapPhones_Click(object sender,EventArgs e) {
			FormMapHQ formMapHQ;
			if(_listFormMapHQs.Count==0) {
				formMapHQ=new FormMapHQ();
				formMapHQ.RoomControlClicked+=FormMapHQ_RoomControlClicked;
				formMapHQ.ExtraMapClicked+=FormMapHQ_ExtraMapClicked;
				formMapHQ.GoToChanged+=MapAreaRoomControl_GoToChanged;
			}
			else {
				formMapHQ=_listFormMapHQs[0]; //always just take the first one.
				if(formMapHQ.WindowState==FormWindowState.Minimized) {
					_listFormMapHQs[0].WindowState=FormWindowState.Normal; //always just take the first map in the list
				}
			}
			formMapHQ.Show();
			formMapHQ.BringToFront();
		}

		private void butTriage_Click(object sender,EventArgs e) {
			controlManage.JumpToTriageTaskWindow();
		}

		private void butVoiceMails_Click(object sender,EventArgs e) {
			//Change the ClockStatus to TeamAssist if the logged on user is clocked in and the same user as the extension.
			if(PhoneTile.PhoneCur!=null
				&& ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)
				&& Security.CurUser.EmployeeNum==PhoneTile.PhoneCur.EmployeeNum) 
			{
				phoneSmall.SetTeamAssist();
			}
			if(_formVoiceMails==null || _formVoiceMails.IsDisposed) {
				_formVoiceMails=new FormVoiceMails();
				_formVoiceMails.FormClosed+=new FormClosedEventHandler((o,e1) => { _formVoiceMails=null; });
				_formVoiceMails.Show();
			}
			if(_formVoiceMails.WindowState==FormWindowState.Minimized) {
				_formVoiceMails.WindowState=FormWindowState.Normal;
			}
			_formVoiceMails.BringToFront();
		}

		public static void S_SetPhoneStatusAvailable() {
			//maintain if they were clocked in at home or at the office
			_formOpenDentalSingleton.phoneSmall.SetAvailable(Employees.GetEmp(Security.CurUser.EmployeeNum).IsWorkingHome);
		}

		private void phonePanel_GoToChanged(object sender,EventArgs e) {
			if(_formPhoneTiles.PatNumGoTo!=0) {
				PatNumCur=_formPhoneTiles.PatNumGoTo;
				Patient pat=Patients.GetPat(PatNumCur);
				RefreshCurrentModule();
				FillPatientButton(pat);
			}
		}

		private void MapAreaRoomControl_GoToChanged(object sender,EventArgs e) {
			MapAreaRoomControl room=(MapAreaRoomControl)sender;
			PatNumCur=room.PhoneCur.PatNum;
			Patient pat=Patients.GetPat(PatNumCur);
			RefreshCurrentModule();
			FillPatientButton(pat);
		}

		private void phoneSmall_GoToChanged(object sender,EventArgs e) {
			if(phoneSmall.GotoPatNum==0) {
				return;
			}
			PatNumCur=phoneSmall.GotoPatNum;
			Patient pat=Patients.GetPat(PatNumCur);
			RefreshCurrentModule();
			FillPatientButton(pat);
			Commlog commlog=Commlogs.GetIncompleteEntry(Security.CurUser.UserNum,PatNumCur);
			PhoneEmpDefault ped=PhoneEmpDefaults.GetByExtAndEmp(phoneSmall.Extension,Security.CurUser.EmployeeNum);
			if(ped!=null && ped.IsTriageOperator) {
				if(Plugins.HookMethod(this,"FormOpenDental.phoneSmall_GoToChanged_IsTriage",pat,phoneSmall.Extension)) {
					return;
				}
				Task task=new Task();
				task.TaskListNum=-1;//don't show it in any list yet.
				Tasks.Insert(task);
				Task taskOld=task.Copy();
				task.KeyNum=PatNumCur;
				task.ObjectType=TaskObjectType.Patient;
				task.TaskListNum=1697;//Hardcoded for internal Triage task list.
				task.UserNum=Security.CurUser.UserNum;
				task.Descript=Phones.GetPhoneForExtensionDB(phoneSmall.Extension).CustomerNumberRaw+" ";//Prefill description with customers number.
				FormTaskEdit FormTE=new FormTaskEdit(task,taskOld);
				FormTE.IsNew=true;
				FormTE.Show();
			}
			else {//Not a triage operator.
				if(Plugins.HookMethod(this,"FormOpenDental.phoneSmall_GoToChanged_NotTriage",pat)) {
					return;
				}
				if(commlog==null) {
					commlog=new Commlog();
					commlog.IsNew=true;
					commlog.PatNum = PatNumCur;
					commlog.CommDateTime = DateTime.Now;
					commlog.CommType =Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
					commlog.Mode_=CommItemMode.Phone;
					commlog.SentOrReceived=CommSentOrReceived.Received;
					commlog.UserNum=Security.CurUser.UserNum;
				}
				using FormCommItem FormCI=new FormCommItem(commlog);
				if(FormCI.ShowDialog()==DialogResult.OK) {
					RefreshCurrentModule();
				}
			}
		}
		#endregion Phone Panel

		#region Menu
		private void menuItemLogOff_Click(object sender, System.EventArgs e) {
			NullUserCheck("menuItemLogOff_Click");
			if(!AreYouSurePrompt(Security.CurUser.UserNum,Lan.g(this,"Are you sure you would like to log off?"))) {
				return;
			}
			LogOffNow(false);
		}

		///<summary>First checks if users have a message prompt turned off for logging off/closing the program. If they don't, then a message is passed in that corresponds with intent (ie logging off vs closing the program). userNum: Used to check if user has "Close/Log off" message preference turned off under File->User Settings.  message: Used for passing in message that lets User know what is being done</summary>
		private bool AreYouSurePrompt(long userNum,string message) {
			UserOdPref logOffMessage=UserOdPrefs.GetByUserAndFkeyType(userNum,UserOdFkeyType.SuppressLogOffMessage).FirstOrDefault();
			if(logOffMessage==null) {//Doesn't exist in the database
				using InputBox inputBox=new InputBox(message,Lan.g(this,"Do not show me this message again."),true,new Point(0,10));
				inputBox.ShowDialog();
				if(inputBox.DialogResult==DialogResult.Cancel) {
					return false;
				}
				else if(inputBox.DialogResult==DialogResult.OK) {
					if(inputBox.checkBoxResult.Checked) {
						UserOdPrefs.Insert(new UserOdPref() {
							UserNum=Security.CurUser.UserNum,
							FkeyType=UserOdFkeyType.SuppressLogOffMessage
						});
						DataValid.SetInvalid(InvalidType.UserOdPrefs);
					}
				}
			}
			return true;
		}
		#endregion Menu

		#region Menu - File

		//File
		private void menuItemPassword_Click(object sender,EventArgs e) {
			SecurityL.ChangePassword(false);
		}

		private void menuItemUserEmailAddress_Click(object sender,EventArgs e) {
			EmailAddress emailAddressCur=EmailAddresses.GetForUser(Security.CurUser.UserNum);
			if(emailAddressCur==null) {
				using FormEmailAddressEdit formEAE=new FormEmailAddressEdit(Security.CurUser.UserNum);
				formEAE.ShowDialog();
			}
			else {
				using FormEmailAddressEdit formEAE=new FormEmailAddressEdit(emailAddressCur);
				formEAE.ShowDialog();
			}
		}
		
		private void menuItemUserSettings_Click(object sender,EventArgs e) {
			using FormUserSetting FormUS=new OpenDental.FormUserSetting();
			FormUS.ShowDialog();
		}

		private void menuItemPrinter_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormPrinterSetup FormPS=new FormPrinterSetup();
			FormPS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Printers");
		}

		private void menuItemGraphics_Click(object sender,EventArgs e) {
			//if(ToothChartRelay.IsSparks3DPresent){
			//	MsgBox.Show(this,"You are using the new 3D tooth chart (Sparks3D.dll), so the Graphics setup window is not needed.");
			//	return;
			//}
			if(!Security.IsAuthorized(Permissions.GraphicsEdit)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			using FormGraphics fg=new FormGraphics();
			fg.ShowDialog();
			Cursor=Cursors.Default;
			if(fg.DialogResult==DialogResult.OK) {
				controlChart.InitializeLocalData();
				RefreshCurrentModule();
			}
		}

		private void menuItemConfig_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ChooseDatabase)){
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.ChooseDatabase,0,"");//make the entry before switching databases.
			ChooseDatabaseInfo chooseDatabaseInfo=ChooseDatabaseInfo.GetChooseDatabaseInfoFromConfig();
			ChooseDatabaseInfo.UpdateChooseDatabaseInfoFromCurrentConnection(chooseDatabaseInfo);
			chooseDatabaseInfo.IsAccessedFromMainMenu=true;
			using FormChooseDatabase FormCD=new FormChooseDatabase(chooseDatabaseInfo);
			if(FormCD.ShowDialog()!=DialogResult.OK) {
				return;
			}
			PatNumCur=0;
			if(!PrefsStartup()){
				return;
			}
			RefreshLocalData(InvalidType.AllLocal);
			UnselectActive();//Deselect the currently Visible module.
			AllNeutral();//Set all modules invisible.
			//The following 2 methods mimic RefreshCurrentModule()
			SetModuleSelected(true);//Reselect the previously selected module, UI is reset to same state as when program starts.
			userControlTasks1.RefreshPatTicketsIfNeeded();
			FillPatientButton(null);
		}

		private void menuItemExit_Click(object sender, System.EventArgs e) {
			Application.Exit();
		}

		#endregion Menu - File

		#region Menu - Setup

		//FormBackupJobsSelect FormBJS=new FormBackupJobsSelect();
		//FormBJS.ShowDialog();	

		//Setup
		private void menuItemApptFieldDefs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormApptFieldDefs FormA=new FormApptFieldDefs();
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Appointment Field Defs");
		}

		private void menuItemApptRules_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormApptRules FormA=new FormApptRules();
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Appointment Rules");
		}

		private void menuItemApptTypes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormApptTypes FormA=new FormApptTypes();
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Appointment Types");
		}

		private void menuItemApptViews_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormApptViews FormAV=new FormApptViews();
			FormAV.ShowDialog();
			RefreshCurrentModule(true);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Appointment Views");
		}

		private void menuItemAlertCategories_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)){
				return;
			}
			using FormAlertCategorySetup FormACS=new FormAlertCategorySetup();
			FormACS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Alert Categories");
		}

		private void menuItemAllocations_Click(object sender, System.EventArgs e) {
			//All security is inside the window
			using FormAllocationsSetup formAllocationsSetup=new FormAllocationsSetup();
			formAllocationsSetup.ShowDialog();
		}

		private void menuItemAutoCodes_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormAutoCode FormAC=new FormAutoCode();
			FormAC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Auto Codes");
		}

		private void menuItemAutomation_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormAutomation FormA=new FormAutomation();
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Automation");
		}

		private void menuItemAutoNotes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit)) {
				return;
			}
			using FormAutoNotes FormA=new FormAutoNotes();
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.AutoNoteQuickNoteEdit,0,"Auto Notes Setup");
		}

		private void menuItemClaimForms_Click(object sender, System.EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
				MsgBox.Show(this,"Claim Forms feature is unavailable when data path A to Z folder is disabled.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormClaimForms FormCF=new FormClaimForms();
			FormCF.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Claim Forms");
		}

		private void menuItemClearinghouses_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormClearinghouses FormC=new FormClearinghouses();
			FormC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Clearinghouses");
		}

		private void menuItemCloudManagement_Click(object sender,EventArgs e) {
			using FormCloudManagement FormCM=new FormCloudManagement();
			FormCM.ShowDialog();
		}
		private void menuItemDiscountPlans_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDiscountPlans FormDP=new FormDiscountPlans();
			FormDP.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Discount Plans");
		}

		private void menuItemComputers_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormComputers FormC=new FormComputers();
			FormC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Computers");
		}

		private void menuItemDataPath_Click(object sender, System.EventArgs e) {
			//Security is handled from within the form.
			//Audit trail is handled within the form due to being able to access FormPath from multiple areas.
			using FormPath FormP=new FormPath();
			FormP.ShowDialog();
			CheckCustomReports();
			this.RefreshCurrentModule();
		}

		private void menuItemDefaultCCProcs_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDefaultCCProcs FormD=new FormDefaultCCProcs();
			FormD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Default CC Procedures");
		}

		private void menuItemDefinitions_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormDefinitions FormD=new FormDefinitions(DefCat.AccountColors);//just the first cat.
			FormD.ShowDialog();
			RefreshCurrentModule(true);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Definitions");
		}

		private void menuItemDentalSchools_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDentalSchoolSetup FormDSS=new FormDentalSchoolSetup();
			FormDSS.ShowDialog();
			RefreshCurrentModule();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Dental Schools");
		}

		private void menuItemDisplayFields_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDisplayFieldCategories FormD=new FormDisplayFieldCategories();
			FormD.ShowDialog();
			RefreshCurrentModule(true);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Display Fields");
		}

		private void menuItemEnterprise_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormEnterpriseSetup FormES=new FormEnterpriseSetup();
			if(FormES.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)==0){
				timerSignals.Enabled=false;
				_hasSignalProcessingPaused=true;
			}
			else{
				timerSignals.Interval=PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)*1000;
				timerSignals.Enabled=true;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Enterprise");
		}

		private void menuItemEmail_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormEmailAddresses FormEA=new FormEmailAddresses();
			FormEA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Email");
		}

		private void menuItemEHR_Click(object sender,EventArgs e) {
			//if(!Security.IsAuthorized(Permissions.Setup)) {
			//  return;
			//}
			using FormEhrSetup FormE=new FormEhrSetup();
			FormE.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"EHR");
		}

		private void menuItemFeeScheds_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit)){
				return;
			}
			using FormFeeScheds FormF=new FormFeeScheds(false);
			FormF.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.FeeSchedEdit,0,"Fee Schedules");
		}

		private void menuFeeSchedGroups_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit)) {
				return;
			}
			//Users that are clinic restricted are not allowed to setup Fee Schedule Groups.
			if(Security.CurUser.ClinicIsRestricted) {
				MsgBox.Show(this,"You are restricted from accessing certain clinics.  Only user without clinic restrictions can edit Fee Schedule Groups.");
				return;
			}
			using FormFeeSchedGroups FormF=new FormFeeSchedGroups();
			FormF.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.FeeSchedEdit,0,"Fee Schedule Groups");
		}

		private void menuItemFHIR_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			using FormFHIRSetup FormFS=new FormFHIRSetup();
			FormFS.ShowDialog();
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"FHIR");
		}

		private void menuItemHIE_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormHieSetup FormHS=new FormHieSetup();
			FormHS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"HIE");
		}

		private void menuItemHL7_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormHL7Defs FormH=new FormHL7Defs();
			FormH.CurPatNum=PatNumCur;
			FormH.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"HL7");
		}

		private void menuItemScanning_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormImagingSetup FormI=new FormImagingSetup();
			FormI.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Imaging");
		}

		private void menuItemInsCats_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormInsCatsSetup FormE=new FormInsCatsSetup();
			FormE.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Insurance Categories");
		}

		private void menuItemInsFilingCodes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormInsFilingCodes FormF=new FormInsFilingCodes();
			FormF.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Insurance Filing Codes");
		}

		private void menuItemLaboratories_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(Plugins.HookMethod(this,"FormOpenDental.menuItemLaboratories_Click")) {
				return;
			}
			using FormLaboratories FormL=new FormLaboratories();
			FormL.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Laboratories");
		}

		private void menuItemLetters_Click(object sender,EventArgs e) {
			using FormLetters FormL=new FormLetters();
			FormL.ShowDialog();
		}

		private void menuItemMessaging_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMessagingSetup FormM=new FormMessagingSetup();
			FormM.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Messaging");
		}

		private void menuItemMessagingButs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMessagingButSetup FormM=new FormMessagingButSetup();
			FormM.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Messaging");
		}

		private void menuItemMisc_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormMisc FormM=new FormMisc();
			if(FormM.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)==0){
				timerSignals.Enabled=false;
				_hasSignalProcessingPaused=true;
			}
			else{
				timerSignals.Interval=PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)*1000;
				timerSignals.Enabled=true;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Misc");
		}

		private void menuItemModules_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(0);//Default to Appts tab.
		}

		private void menuItemMounts_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMountDefs formMountDefs=new FormMountDefs();
			formMountDefs.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Mounts");
		}

		private void menuItemImagingDevices_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormImagingDevices formImagingDevices=new FormImagingDevices();
			formImagingDevices.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Imaging Devices");
		}

		private void menuItemOrtho_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormOrthoSetup FormOS = new FormOrthoSetup();
			FormOS.ShowDialog();
		}

		private void menuItemPreferencesAppts_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(0);
		}

		private void menuItemPreferencesFamily_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(1);
		}

		private void menuItemPreferencesAccount_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(2);
		}

		private void menuItemPreferencesTreatPlan_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(3);
		}

		private void menuItemPreferencesChart_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(4);
		}

		private void menuItemPreferencesImaging_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(5);
		}

		private void menuItemPreferencesManage_Click(object sender,EventArgs e) {
			LaunchModuleSetupWithTab(6);
		}

		///<summary>Checks setup permission, launches the module setup window with the specified tab and then makes an audit entry.
		///This is simply a helper method because every preferences menu item will do the exact same code.</summary>
		private void LaunchModuleSetupWithTab(int selectedTab) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormModuleSetup FormM=new FormModuleSetup(selectedTab);
			if(FormM.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillPatientButton(Patients.GetPat(PatNumCur));
			RefreshCurrentModule(true);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Modules");
		}

		private void menuItemOperatories_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormOperatories FormO=new FormOperatories();
			FormO.ContrApptRef=controlAppt;
			FormO.ShowDialog();
			if(FormO.ListConflictingAppts.Count > 0) {
				FormApptConflicts FormAC=new FormApptConflicts(FormO.ListConflictingAppts);
				FormAC.Show();
				FormAC.BringToFront();
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Operatories");
		}

		private void menuItemPatFieldDefs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormPatFieldDefs FormP=new FormPatFieldDefs();
			FormP.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Patient Field Defs");
		}

		private void menuItemPayerIDs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormElectIDs FormE=new FormElectIDs();
			FormE.IsSelectMode=false;
			FormE.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Payer IDs");
		}

		private void menuItemInsBlueBook_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormInsBlueBookRules FormIBBR=new FormInsBlueBookRules();
			FormIBBR.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Insurance Blue Book");
		}

		private void menuItemPractice_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormPractice FormPr=new FormPractice();
			FormPr.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Practice Info");
			if(FormPr.DialogResult!=DialogResult.OK) {
				return;
			}
			moduleBar.RefreshButtons();
			RefreshCurrentModule();
		}

		private void menuItemProblems_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDiseaseDefs FormD=new FormDiseaseDefs();
			FormD.ShowDialog();
			//RefreshCurrentModule();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Disease Defs");
		}

		private void menuItemProcedureButtons_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormProcButtons FormPB=new FormProcButtons();
			FormPB.Owner=this;
			FormPB.ShowDialog();
			SetModuleSelected();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Procedure Buttons");	
		}

		private void menuItemLinks_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormProgramLinks FormPL=new FormProgramLinks();
			FormPL.ShowDialog();
			controlChart.InitializeLocalData();//for eCW
			RefreshMenuReports();
			if(PatNumCur>0) {
				Patient pat=Patients.GetPat(PatNumCur);
				FillPatientButton(pat);
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Program Links");
		}

		/*
		private void menuItem_ProviderAllocatorSetup_Click(object sender,EventArgs e) {
			// Check Permissions
			if(!Security.IsAuthorized(Permissions.Setup)) {
				// Failed security prompts message box. Consider adding overload to not show message.
				//MessageBox.Show("Not Authorized to Run Setup for Provider Allocation Tool");
				return;
			}
			using Reporting.Allocators.MyAllocator1.FormInstallAllocator_Provider fap = new OpenDental.Reporting.Allocators.MyAllocator1.FormInstallAllocator_Provider();
			fap.ShowDialog();
		}*/

		private void menuItemAsapList_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormAsapSetup FormAS=new FormAsapSetup();
			FormAS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"ASAP List Setup");
		}

		private void menuItemConfirmations_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormConfirmationSetup FormCS=new FormConfirmationSetup();
			FormCS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Confirmation Setup");
		}

		private void menuItemInsVerify_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormInsVerificationSetup FormIV=new FormInsVerificationSetup();
			FormIV.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Insurance Verification");
        }

		private void menuItemQuestions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormQuestionDefs FormQ=new FormQuestionDefs();
			FormQ.ShowDialog();
			//RefreshCurrentModule();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Questionnaire");
		}

		private void menuItemRecall_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormRecallSetup FormRS=new FormRecallSetup();
			FormRS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Recall");	
		}

		private void menuItemRecallTypes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormRecallTypes FormRT=new FormRecallTypes();
			FormRT.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Recall Types");	
		}

		private void menuItemReactivation_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormReactivationSetup FormRS=new FormReactivationSetup();
			FormRS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Reactivation");	
		}

		private void menuItemReplication_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ReplicationSetup)) {
				return;
			}
			using FormReplicationSetup FormRS=new FormReplicationSetup();
			FormRS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.ReplicationSetup,0,"Replication setup.");
		}
		
		private void menuItemReports_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormReportSetup FormRS=new FormReportSetup(0,false);
			FormRS.ShowDialog();
		}

		private void menuItemRequiredFields_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormRequiredFields FormRF=new FormRequiredFields();
			FormRF.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Required Fields");
		}

		private void menuItemRequirementsNeeded_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormReqNeededs FormR=new FormReqNeededs();
			FormR.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Requirements Needed");
		}

		private void menuItemSched_Click(object sender,EventArgs e) {
			//anyone should be able to view. Security must be inside schedule window.
			//if(!Security.IsAuthorized(Permissions.Schedules)) {
			//	return;
			//}
			using FormSchedule FormS=new FormSchedule();
			FormS.ShowDialog();
			//SecurityLogs.MakeLogEntry(Permissions.Schedules,0,"");
		}

		private void MenuItemScheduledProcesses_Click(object sender,EventArgs e) {
			using FormScheduledProcesses formScheduledProcesses=new FormScheduledProcesses();
			formScheduledProcesses.ShowDialog();
		}

		/*private void menuItemBlockoutDefault_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			using FormSchedDefault FormSD=new FormSchedDefault(ScheduleType.Blockout);
			FormSD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Blockouts,0,"Default");
		}*/

		public static void S_MenuItemSecurity_Click(object sender,EventArgs e) {
			_formOpenDentalSingleton.menuItemSecurity_Click(sender,e);
		}

		private void menuItemSecurity_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormSecurity FormS=new FormSecurity();
			FormS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Security Window");
			if(!PrefC.HasClinicsEnabled) {//clinics not enabled, refresh current module and return
				RefreshCurrentModule();
				return;
			}
			//clinics is enabled
			long clinicNumOld=Clinics.ClinicNum;
			if(Security.CurUser.ClinicIsRestricted) {
				Clinics.ClinicNum=Security.CurUser.ClinicNum;
			}
			Text=PatientL.GetMainTitle(Patients.GetPat(PatNumCur),Clinics.ClinicNum);
			SetSmsNotificationText(doUseSignalInterval:(clinicNumOld==Clinics.ClinicNum));//Clinic selection changed, update sms notifications.
			RefreshMenuClinics();//this calls ModuleSelected, so no need to call RefreshCurrentModule
			RefreshMenuDashboards();
		}

		private void menuItemSecurityAddUser_Click(object sender,EventArgs e) {
			bool isAuthorizedAddNewUser=Security.IsAuthorized(Permissions.AddNewUser,true);
			bool isAuthorizedSecurityAdmin=Security.IsAuthorized(Permissions.SecurityAdmin,true);
			if(!(isAuthorizedAddNewUser || isAuthorizedSecurityAdmin)) {
				MsgBox.Show(this,"Not authorized to add a new user.");
				return;
			}
			if(PrefC.GetLong(PrefName.DefaultUserGroup)==0) {
				if(isAuthorizedSecurityAdmin) {
					//Prompt to go to form.
					string msg="Default user group is not set.  Would you like to set the default user group now?";
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,msg,"Default user group")){
						using FormGlobalSecurity FormGS=new FormGlobalSecurity();
						FormGS.ShowDialog();//No refresh needed; Signals sent from this form.
					}
				}
				else {
					//Using verbage similar to that found in the manual for describing how to navigate to a window in the program.
					string msg="Default user group is not set.  A user with the SecurityAdmin permission must set a default user group.  "
						+"To view the default user group, in the Main Menu, click Setup, Security, Security Settings, Global Security Settings.";
					MsgBox.Show(this,msg,"Default user group");
				}
				return;
			}
			using FormUserEdit FormUE=new FormUserEdit(new Userod(),true);
			FormUE.IsNew=true;
			FormUE.ShowDialog();
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetDefs FormSD=new FormSheetDefs();
			FormSD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
		}

		//This shows as "Show Features"
		private void menuItemEasy_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormShowFeatures FormE=new FormShowFeatures();
			FormE.ShowDialog();
			controlAccount.LayoutToolBar();//for repeating charges
			RefreshCurrentModule(true);
			//Show enterprise setup if it was enabled
			_menuItemEnterprise.Available=PrefC.GetBool(PrefName.ShowFeatureEnterprise);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Show Features");
		}

		private void menuItemSpellCheck_Click(object sender,EventArgs e) {
			using FormSpellCheck FormD=new FormSpellCheck();
			FormD.ShowDialog();
		}

		private void menuItemTimeCards_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormTimeCardSetup FormTCS=new FormTimeCardSetup();
			FormTCS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Time Card Setup");
		}

		private void menuItemTask_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormTaskPreferences formTaskSetup = new FormTaskPreferences();
			if(formTaskSetup.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(userControlTasks1.Visible) {
				userControlTasks1.InitializeOnStartup();
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Task");
		}
		
		private void menuItemQuickPasteNotes_Click(object sender,EventArgs e) {
			using FormQuickPaste formQP=new FormQuickPaste(true);
			formQP.QuickType=QuickPasteType.None;
			formQP.ShowDialog();
		}

		private void menuItemWebForm_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormWebFormSetup formWFS=new FormWebFormSetup();
			formWFS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Web Forms Setup");
		}

		#endregion Menu - Setup

		#region Menu - Lists

		//Lists
		private void menuItemProcCodes_Click(object sender, System.EventArgs e) {
			//security handled within form
			using(FormProcCodes FormP=new FormProcCodes(true)) {
				FormP.ShowDialog();
			}
		}

		private void menuItemAllergies_Click(object sender,EventArgs e) {
			using FormAllergySetup formAllergySetup= new FormAllergySetup();
			formAllergySetup.ShowDialog();
		}

		private void menuItemClinics_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormClinics FormC=new FormClinics();
			FormC.IncludeHQInList=true;
			FormC.IsMultiSelect=true;
			FormC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Clinics");
			//this menu item is only visible if the clinics show feature is enabled (!EasyNoClinics)
			if(Clinics.GetDesc(Clinics.ClinicNum)=="") {//will be empty string if ClinicNum is not valid, in case they deleted the clinic
				Clinics.ClinicNum=Security.CurUser.ClinicNum;
				SetSmsNotificationText(doUseSignalInterval:true);//Update sms notification text.
				Text=PatientL.GetMainTitle(Patients.GetPat(PatNumCur),Clinics.ClinicNum);
			}
			RefreshMenuClinics();
			//reset the main title bar in case the user changes the clinic description for the selected clinic
			Patient pat=Patients.GetPat(PatNumCur);
			Text=PatientL.GetMainTitle(pat,Clinics.ClinicNum);
			//reset the tip text in case the user changes the clinic description
		}
		
		private void menuItemContacts_Click(object sender, System.EventArgs e) {
			using FormContacts FormC=new FormContacts();
			FormC.ShowDialog();
		}

		private void menuItemCounties_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormCounties FormC=new FormCounties();
			FormC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Counties");
		}

		private void menuItemSchoolClass_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormSchoolClasses FormS=new FormSchoolClasses();
			FormS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Dental School Classes");
		}

		private void menuItemSchoolCourses_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormSchoolCourses FormS=new FormSchoolCourses();
			FormS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Dental School Courses");
		}

		private void menuItemEmployees_Click(object sender, System.EventArgs e) {
			if(!PrefC.IsODHQ && !Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormEmployeeSelect FormEmp=new FormEmployeeSelect();
			FormEmp.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Employees");	
		}

		private void menuItemEmployers_Click(object sender, System.EventArgs e) {
			using FormEmployers FormE=new FormEmployers();
			FormE.ShowDialog();
		}

		private void menuItemInstructors_Click(object sender, System.EventArgs e) {
			/*if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormInstructors FormI=new FormInstructors();
			FormI.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Dental School Instructors");*/
		}

		private void menuItemCarriers_Click(object sender, System.EventArgs e) {
			using FormCarriers FormC=new FormCarriers();
			FormC.ShowDialog();
			RefreshCurrentModule();
		}

		private void menuItemInsPlans_Click(object sender, System.EventArgs e) {
			using FormInsPlans FormIP = new FormInsPlans();
			FormIP.ShowDialog();
			RefreshCurrentModule(true);
		}

		private void menuItemJobManager_Click(object sender,System.EventArgs e) {
			if(_formJobManager==null || _formJobManager.IsDisposed) {
				//Dpi.SetUnaware();//this fails for some unknown reason.  I suspect UI threads.  No time to research further.
				_formJobManager=new FormJobManager();
				//Dpi.SetAware();
			}
			_formJobManager.Show();
			if(_formJobManager.WindowState==FormWindowState.Minimized) {
				_formJobManager.WindowState=FormWindowState.Normal;
			}
			_formJobManager.BringToFront();
		}

		public static void S_GoToJob(long jobNum) {
			_formOpenDentalSingleton.GoToJob(jobNum);
		}

		///<summary>Can be called from anywhere in OD layer to load job. 
		///It is in FormOpenDental because this is where the static reference to theJob Manager is.</summary>
		private void GoToJob(long jobNum) {
			if(_formJobManager==null || _formJobManager.IsDisposed) {
				//Dpi.SetUnaware();//this fails for some unknown reason.  I suspect UI threads.  No time to research further.
				_formJobManager=new FormJobManager();
				//Dpi.SetAware();
			}
			_formJobManager.Show();
			if(_formJobManager.WindowState==FormWindowState.Minimized) {
				_formJobManager.WindowState=FormWindowState.Normal;
			}
			_formJobManager.BringToFront();
			_formJobManager.GoToJob(jobNum);
		}

		private void menuItemLabCases_Click(object sender,EventArgs e) {
			using FormLabCases FormL=new FormLabCases();
			FormL.ShowDialog();
			if(FormL.GoToAptNum!=0) {
				Appointment apt=Appointments.GetOneApt(FormL.GoToAptNum);
				Patient pat=Patients.GetPat(apt.PatNum);
				S_Contr_PatientSelected(pat,false);
				//OnPatientSelected(pat.PatNum,pat.GetNameLF(),pat.Email!="",pat.ChartNumber);
				GotoModule.GotoAppointment(apt.AptDateTime,apt.AptNum);
			}
		}

		private void menuItemMedications_Click(object sender, System.EventArgs e) {
			using FormMedications FormM=new FormMedications();
			FormM.ShowDialog();
		}

		private void menuItemPharmacies_Click(object sender,EventArgs e) {
			using FormPharmacies FormP=new FormPharmacies();
			FormP.ShowDialog();
		}

		private void menuItemProviders_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Providers,true) && !Security.IsAuthorized(Permissions.AdminDentalStudents,true)) {
				MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"
					+GroupPermissions.GetDesc(Permissions.Providers)+" "+Lans.g("Security","or")+" "+GroupPermissions.GetDesc(Permissions.AdminDentalStudents));
				return;
			}
			using FormProviderSetup FormPS=new FormProviderSetup();
			FormPS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Providers");		
		}

		private void menuItemPrescriptions_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormRxSetup FormRxSetup2=new FormRxSetup();
			FormRxSetup2.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rx");		
		}

		private void menuItemReferrals_Click(object sender, System.EventArgs e) {
			using FormReferralSelect FormRS=new FormReferralSelect();
			FormRS.ShowDialog();		
		}

		private void menuItemSites_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormSites FormS=new FormSites();
			FormS.ShowDialog();
			RefreshCurrentModule();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sites");
		}

		private void menuItemStateAbbrs_Click(object sender,System.EventArgs e) {
			using FormStateAbbrs formSA=new FormStateAbbrs();
			formSA.ShowDialog();
			RefreshCurrentModule();
		}

		private void menuItemZipCodes_Click(object sender, System.EventArgs e) {
			//if(!Security.IsAuthorized(Permissions.Setup)){
			//	return;
			//}
			using FormZipCodes FormZ=new FormZipCodes();
			FormZ.ShowDialog();
			//SecurityLogs.MakeLogEntry(Permissions.Setup,"Zip Codes");
		}

		#endregion Menu - Lists

		#region Menu - Reports
		private void menuItemReportsStandard_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Reports)) {
				return;
			}
			using FormReportsMore FormR=new FormReportsMore();
			FormR.DateSelected=controlAppt.GetDateSelected();
			FormR.ShowDialog();
			NonModalReportSelectionHelper(FormR.RpNonModalSelection);
		}

		private void menuItemReportsGraphic_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.GraphicalReports)) {
				return;
			}
			if(_formDashboardEditTab!=null) {
				_formDashboardEditTab.BringToFront();
				return;
			}
			//on extremely large dbs, the ctor can take a few seconds to load, so show the wait cursor.
			Cursor=Cursors.WaitCursor;
			//Check if the user has permission to view all providers in production and income reports
			bool hasAllProvsPermission=Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true);
			if(!hasAllProvsPermission && Security.CurUser.ProvNum==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The current user must be a provider or have the 'All Providers' permission to view provider reports. Continue?")) {
					return;
				}
			}
			_formDashboardEditTab=new OpenDentalGraph.FormDashboardEditTab(Security.CurUser.ProvNum,!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) { IsEditMode=false };
			_formDashboardEditTab.FormClosed+=new FormClosedEventHandler((object senderF,FormClosedEventArgs eF) => { _formDashboardEditTab=null; });
			Cursor=Cursors.Default;
			_formDashboardEditTab.Show();
		}
				
		private void menuItemReportsUserQuery_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				MsgBox.Show(this,"Not allowed while using Oracle.");
				return;
			}
			if(Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,Lan.g(this,"User query form accessed."));
				if(_formQuery==null || _formQuery.IsDisposed) {
					_formQuery=new FormQuery(null);
					_formQuery.FormClosed+=new FormClosedEventHandler((object senderF,FormClosedEventArgs eF) => { _formQuery=null; });
					_formQuery.Show();
				}
				if(_formQuery.WindowState==FormWindowState.Minimized) {
					_formQuery.WindowState=FormWindowState.Normal;
				}
				_formQuery.BringToFront();
			}
			else {
				using FormQueryFavorites FormQF=new FormQueryFavorites();
				FormQF.ShowDialog();
				if(FormQF.DialogResult==DialogResult.OK) {
					ExecuteQueryFavorite(FormQF.UserQueryCur);
				}
			}
		}

		private void menuItemReportsFilteredClick_Click(object sender,EventArgs e) {
			using FormReportsFiltered formReportsFiltered=new FormReportsFiltered();
			formReportsFiltered.ShowDialog();
			if(formReportsFiltered.DialogResult==DialogResult.OK) {
				StandardReport_Click(formReportsFiltered.DisplayReportCur);
			}
		}

		private void menuItemReportsQueryFavorites_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			using FormQueryFavorites formQueryFavorites=new FormQueryFavorites();
			formQueryFavorites.ShowDialog();
			if(formQueryFavorites.DialogResult==DialogResult.OK) {
				ExecuteQueryFavorite(formQueryFavorites.UserQueryCur);
			}
		}

		private void menuItemReportsUnfinalizedPay_Click(object sender,EventArgs e) {
			DisplayReport unfinalizedPayReport=DisplayReports.GetWhere(x => x.InternalName==DisplayReports.ReportNames.UnfinalizedInsPay).FirstOrDefault();
			if(unfinalizedPayReport.IsHidden) {
				MsgBox.Show(this,"The administrator has hidden this report.");
				return;
			}
			if(!GroupPermissions.HasReportPermission(DisplayReports.ReportNames.UnfinalizedInsPay,Security.CurUser)) {
				MsgBox.Show(this,"You do not have permission to run this report.");
				return;
			}
			using FormRpUnfinalizedInsPay formRp=new FormRpUnfinalizedInsPay();
			formRp.ShowDialog();
		}

		private void menuItemReportsActivityLog_Click(object sender, EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Reports)) {
				return;
			}
			using FormActivityLog FormA=new FormActivityLog();
			FormA.ShowDialog();
		}

		private void UpdateUnfinalizedPayCount(List<Signalod> listSignals) {
			if(listSignals.Count==0) {
				_menuItemUnfinalizedPay.Text=Lan.g(this,"Unfinalized Payments");
				return;
			}
			Signalod signal=listSignals.OrderByDescending(x => x.SigDateTime).First();
			_menuItemUnfinalizedPay.Text=Lan.g(this,"Unfinalized Payments")+": "+signal.MsgValue;
		}

		private void RefreshMenuReports() {
			_menuItemUserQuery.Available=Security.IsAuthorized(Permissions.UserQueryAdmin,true);
			_menuItemQueryFavorites.Available=Security.IsAuthorized(Permissions.UserQuery,true);
			//Find the index of the last separator which separates the static menu items from the dynamic menu items.
			int separatorIndex=-1;
			for(int i=0;i<_menuItemReports.DropDown.Items.Count;i++) {
				if(_menuItemReports.DropDown.Items[i].Text=="-") {
					separatorIndex=i;
				}
			}
			//Remove dynamic items and separator.  Leave hard coded items.
			if(separatorIndex!=-1) {
				for(int i=_menuItemReports.DropDown.Items.Count-1;i>=separatorIndex;i--) {
					_menuItemReports.DropDown.Items.RemoveAt(i);
				}
			}
			List<ToolButItem> listToolButItems=ToolButItems.GetForToolBar(ToolBarsAvail.ReportsMenu);
			if(listToolButItems.Count==0) {
				return;//Return early to avoid adding a useless separator in the menu.
			}
			//Add separator, then dynamic items to the bottom of the menu.
			_menuItemReports.AddSeparator();//Separator
			int newSeparatorIndex=_menuItemReports.DropDown.Items.Count-1;//Determine the seperator's row by index
			_menuItemReports.DropDown.Items[newSeparatorIndex].Text="-";//Add text to assist with identification of the new separator
			listToolButItems.Sort(ToolButItem.Compare);//Alphabetical order
			foreach(ToolButItem toolButItemCur in listToolButItems) {
				MenuItemOD menuItem=new MenuItemOD(toolButItemCur.ButtonText,menuReportLink_Click);
				menuItem.Tag=toolButItemCur;
				_menuItemReports.Add(menuItem);
			}
		}

		private void menuReportLink_Click(object sender,System.EventArgs e) {
			MenuItemOD menuItem=(MenuItemOD)sender;
			ToolButItem toolButItemCur=((ToolButItem)menuItem.Tag);			
			ProgramL.Execute(toolButItemCur.ProgramNum,Patients.GetPat(PatNumCur));
		}

		private void menuItemRDLReport_Click(object sender,System.EventArgs e) {
			//This point in the code is only reached if the A to Z folders are enabled, thus
			//the image path should exist.
			using FormReportCustom FormR=new FormReportCustom();
			FormR.SourceFilePath=
				ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),PrefC.GetString(PrefName.ReportFolderName),((MenuItemOD)sender).Text+".rdl");
			FormR.ShowDialog();
		}

		private void StandardReport_Click(DisplayReport displayReport) {
			//Permission already validated.
			ReportNonModalSelection selection=FormReportsMore.OpenReportHelper(displayReport,controlAppt.GetDateSelected(),doValidatePerm:false);
			NonModalReportSelectionHelper(selection);
		}

		private void NonModalReportSelectionHelper(ReportNonModalSelection selection) {
			switch(selection) {
				case ReportNonModalSelection.TreatmentFinder:
					FormRpTreatmentFinder FormT=new FormRpTreatmentFinder();
					FormT.Show();
					break;
				case ReportNonModalSelection.OutstandingIns:
					FormRpOutstandingIns FormOI=new FormRpOutstandingIns();
					FormOI.Show();
					break;
				case ReportNonModalSelection.UnfinalizedInsPay:
					FormRpUnfinalizedInsPay FormU=new FormRpUnfinalizedInsPay();
					FormU.Show();
					break;
				case ReportNonModalSelection.UnsentClaim:
					FormRpClaimNotSent FormCNS=new FormRpClaimNotSent();
					FormCNS.Show();
					break;
				case ReportNonModalSelection.WebSchedAppointments:
					FormWebSchedAppts formWSA=new FormWebSchedAppts(true,true,true,true);
					formWSA.Show();
					break;
				case ReportNonModalSelection.CustomAging:
					FormRpCustomAging FormCAO=new FormRpCustomAging();
					FormCAO.Show();
					break;
				case ReportNonModalSelection.IncompleteProcNotes:
					FormRpProcNote FormPN=new FormRpProcNote(this);
					FormPN.Show();
					break;
				case ReportNonModalSelection.ProcNotBilledIns:
					FormRpProcNotBilledIns FormProc=new FormRpProcNotBilledIns(this);
					//Both FormRpProcNotBilledIns and FormClaimsSend are non-modal.
					//If both forms are open try and update FormClaimsSend to reflect any newly created claims.
					FormProc.OnPostClaimCreation+=() => controlManage.TryRefreshFormClaimSend();
					FormProc.FormClosed+=(s,ea) => { ODEvent.Fired-=formProcNotBilled_GoToChanged; };
					ODEvent.Fired+=formProcNotBilled_GoToChanged;
					FormProc.Show();//FormProcSend has a GoTo option and is shown as a non-modal window.
					FormProc.BringToFront();
					break;
				case ReportNonModalSelection.ODProcsOverpaid:
					FormRpProcOverpaid FormPO=new FormRpProcOverpaid();
					FormPO.Show();
					break;
				case ReportNonModalSelection.DPPOvercharged:
					if(_formRpDPPOvercharged==null || _formRpDPPOvercharged.IsDisposed) {
						_formRpDPPOvercharged=new FormRpDPPOvercharged();
					}
					_formRpDPPOvercharged.Show();
					if(_formRpDPPOvercharged.WindowState==FormWindowState.Minimized) {
						_formRpDPPOvercharged.WindowState=FormWindowState.Normal;
					}
					_formRpDPPOvercharged.BringToFront();
					break;
				case ReportNonModalSelection.PatPortionUncollected:
					FormRpPatPortionUncollected formPatPortUncol = new FormRpPatPortionUncollected();
					formPatPortUncol.Show();
					break;
				case ReportNonModalSelection.EraAutoProcessed:
					FormRpEraAutoProcessed formRpEraAutoProcessed=new FormRpEraAutoProcessed();
					formRpEraAutoProcessed.Show();
					break;
				case ReportNonModalSelection.None:
				default:
					//Do nothing.
					break;
			}
		}

		private void formProcNotBilled_GoToChanged(ODEventArgs e) {
			if(e.EventType!=ODEventType.FormProcNotBilled_GoTo) {
				return;
			}
			Patient pat=Patients.GetPat((long)e.Tag);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoClaim((long)e.Tag);
		}

		private void UserQuery_ClickEvent(object sender,EventArgs e) {
			UserQuery userQuery=(UserQuery) ((MenuItem) sender).Tag;
			ExecuteQueryFavorite(userQuery,true);
		}

		private void ExecuteQueryFavorite(UserQuery userQuery,bool doRunPrompt=false) {
			SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,Lan.g(this,"User query form accessed."));
			//ReportSimpleGrid report=new ReportSimpleGrid();
			if(doRunPrompt && userQuery.IsPromptSetup && UserQueries.ParseSetStatements(userQuery.QueryText).Count>0) {
				//if the user is not a query admin, they will not have the ability to edit 
				//the query before it is run, so show them the SET statement edit window.
				using FormQueryParser FormQP = new FormQueryParser(userQuery);
				FormQP.ShowDialog();
				if(FormQP.DialogResult!=DialogResult.OK) {
					//report.Query=userQuery.QueryText;
					return;
				}
			}
			if(_formQuery!=null) {
				_formQuery.textQuery.Text=userQuery.QueryText;
				_formQuery.textTitle.Text=userQuery.FileName;
				_formQuery.SubmitQueryThreaded();
				_formQuery.BringToFront();
				return;
			}
			_formQuery=new FormQuery(null,true);
			_formQuery.FormClosed+=new FormClosedEventHandler((object senderF,FormClosedEventArgs eF) => { _formQuery=null; });
			_formQuery.textQuery.Text=userQuery.QueryText;
			_formQuery.textTitle.Text=userQuery.FileName;
			_formQuery.Show();
		}
		#endregion Menu - Reports

		#region Menu - Tools

		//Tools
		private void menuItemPrintScreen_Click(object sender, System.EventArgs e) {
			using FormPrntScrn FormPS=new FormPrntScrn();
			FormPS.ShowDialog();
		}

		private void menuItemScreenSnip_Click(object sender,EventArgs e) {
			if(!FormClaimAttachment.StartSnipAndSketchOrSnippingTool()) {
				MsgBox.Show(this,"Neither the Snip & Sketch tool nor the Snipping Tool could be launched.");
				return;
			}
		}

		//MiscTools
		private void menuItemDuplicateBlockouts_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormBlockoutDuplicatesFix form=new FormBlockoutDuplicatesFix();
			Cursor=Cursors.WaitCursor;
			form.ShowDialog();
			Cursor=Cursors.Default;
			//Security log entries are made from within the form.
		}

		private void menuItemCareCreditTrans_Click(object sender,EventArgs e) {
			if(_formCareCreditTransactions==null || _formCareCreditTransactions.IsDisposed) {
				_formCareCreditTransactions=new FormCareCreditTransactions();
				_formCareCreditTransactions.FormClosed+=new FormClosedEventHandler((o,e1) => { _formCareCreditTransactions=null; });
				_formCareCreditTransactions.Show();
			}
			if(_formCareCreditTransactions.WindowState==FormWindowState.Minimized) {
				_formCareCreditTransactions.WindowState=FormWindowState.Normal;
			}
			_formCareCreditTransactions.BringToFront();
		}

		private void menuItemCreateAtoZFolders_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormAtoZFoldersCreate FormA=new FormAtoZFoldersCreate();
			FormA.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemDatabaseMaintenancePat_Click(object sender,EventArgs e) {
			//Purposefully not checking permissions.  All users need the ability to call patient specific DBMs ATM.
			using FormDatabaseMaintenancePat FormDMP=new FormDatabaseMaintenancePat(PatNumCur);
			FormDMP.ShowDialog();
		}

		private void menuItemMergeDPs_Click(object sender,EventArgs e) {
			using FormDiscountPlanMerge FormDPM=new FormDiscountPlanMerge();
			FormDPM.ShowDialog();
		}

		private void menuItemEditTestModeOverrides_Click(object sender,EventArgs e) {
			using FormEditTestModeOverrides formEditTestModeOverrides=new FormEditTestModeOverrides();
			formEditTestModeOverrides.ShowDialog();
		}

		private void menuItemMergeBillingType_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormBillingTypeMerge formBillingTypeMerge=new FormBillingTypeMerge();
			formBillingTypeMerge.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemMergeImageCat_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormImageCatMerge FormImageCat=new FormImageCatMerge();
			FormImageCat.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemMergeMedications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.MedicationMerge)) {
				return;
			}
			using FormMedicationMerge FormMM=new FormMedicationMerge();
			FormMM.ShowDialog();
			//Securitylog entries are handled within the form.
		}

		private void menuItemMergePatients_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PatientMerge)) {
				return;
			}
			using FormPatientMerge fpm=new FormPatientMerge();
			fpm.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemMergeReferrals_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ReferralMerge)) {
				return;
			}
			using FormReferralMerge FormRM=new FormReferralMerge();
			FormRM.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemMergeProviders_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ProviderMerge)) {
				return;
			}
			using FormProviderMerge FormPM=new FormProviderMerge();
			FormPM.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemMoveSubscribers_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanChangeSubsc)) {
				return;
			}
			using FormSubscriberMove formSM=new FormSubscriberMove();
			formSM.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuPatientStatusSetter_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormPatientStatusTool formPST = new FormPatientStatusTool();
			formPST.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemProcLockTool_Click(object sender,EventArgs e) {
			using FormProcLockTool FormT=new FormProcLockTool();
			FormT.ShowDialog();
			//security entries made inside the form
			//SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Proc Lock Tool");
		}

		private void menuItemSetupWizard_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSetupWizard FormSW = new FormSetupWizard();
			FormSW.ShowDialog();
		}

		private void menuItemServiceManager_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMain FormM=new FormMain();
			FormM.ShowDialog();
		}

		private void menuItemShutdown_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormShutdown FormS=new FormShutdown();
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			//turn off signal reception for 5 seconds so this workstation will not shut down.
			Signalods.SignalLastRefreshed=MiscData.GetNowDateTime().AddSeconds(5);
			Signalod sig=new Signalod();
			sig.IType=InvalidType.ShutDownNow;
			Signalods.Insert(sig);
			Computers.ClearAllHeartBeats(ODEnvironment.MachineName);//always assume success
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Shutdown all workstations.");
		}
		
		private void menuTelephone_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormTelephone FormT=new FormTelephone();
			FormT.ShowDialog();
			//Security log entries are made from within the form.
		}

		private void menuItemTestLatency_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormTestLatency formTL=new FormTestLatency();
			formTL.ShowDialog();
		}
		
		private void menuItemXChargeReconcile_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Accounting)) {
				return;
			}
			using FormXChargeReconcile FormXCR=new FormXChargeReconcile();
			FormXCR.ShowDialog();
		}
		//End of MiscTools

		private void menuItemAdvertisingPostcards_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Advertising)) {
				return;
			}
			if(PrefC.GetString(PrefName.AdvertisingPostCardGuid)=="") {
				using FormAdvertisingPostcardsSetup formMassPostcardSetup=new FormAdvertisingPostcardsSetup();
				formMassPostcardSetup.ShowDialog();
			}
			using FormAdvertisingPostcardsSend formMassPostcardSend=new FormAdvertisingPostcardsSend();
			formMassPostcardSend.ShowDialog();
		}

		private void menuItemAging_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormAging FormAge=new FormAging();
			FormAge.ShowDialog();
		}

		private void menuItemAuditTrail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AuditTrail)) {
				return;
			}
			using FormAudit FormA=new FormAudit();
			FormA.CurPatNum=PatNumCur;
			FormA.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.AuditTrail,0,"Audit Trail");
		}

		private void menuItemFinanceCharge_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormFinanceCharges FormFC=new FormFinanceCharges();
			FormFC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Run Finance Charges");
		}

		private void menuItemCCRecurring_Click(object sender,EventArgs e) {
			if(_formCreditRecurringCharges==null || _formCreditRecurringCharges.IsDisposed) {
				_formCreditRecurringCharges=new FormCreditRecurringCharges();
			}
			Cursor=Cursors.WaitCursor;
			_formCreditRecurringCharges.Show();
			Cursor=Cursors.Default;
			if(_formCreditRecurringCharges.WindowState==FormWindowState.Minimized) {
				_formCreditRecurringCharges.WindowState=FormWindowState.Normal;
			}
			_formCreditRecurringCharges.BringToFront();
		}

		private void menuItemCertifications_Click(object sender,EventArgs e) {
			if(_formCertifications==null || _formCertifications.IsDisposed) {
				_formCertifications=new FormCertifications();
			}
			_formCertifications.Show();
			if(_formCertifications.WindowState==FormWindowState.Minimized) {
				_formCertifications.WindowState=FormWindowState.Normal;
			}
			_formCertifications.BringToFront();
		}

		private void menuItemCustomerManage_Click(object sender,EventArgs e) {
			using FormCustomerManagement FormC=new FormCustomerManagement();
			FormC.ShowDialog();
			if(FormC.SelectedPatNum!=0) {
				PatNumCur=FormC.SelectedPatNum;
				Patient pat=Patients.GetPat(PatNumCur);
				RefreshCurrentModule();
				FillPatientButton(pat);
			}
		}

		private void menuItemDatabaseMaintenance_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormDatabaseMaintenance FormDM=new FormDatabaseMaintenance();
			FormDM.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Database Maintenance");
		}

		private void menuItemDispensary_Click(object sender,System.EventArgs e) {
			using FormDispensary FormD=new FormDispensary();
			FormD.ShowDialog();
		}

		private void menuItemEvaluations_Click(object sender,EventArgs e) {
			bool isAllowed;
			if(Security.CurUser.ProvNum==0) {
				isAllowed=Security.IsAuthorized(Permissions.AdminDentalEvaluations,true);
			}
			else {
				isAllowed=Providers.GetProv(Security.CurUser.ProvNum).IsInstructor || Security.IsAuthorized(Permissions.AdminDentalEvaluations,true);
			}
			if(!isAllowed){
				MsgBox.Show(this,$"Only users with the {Permissions.AdminDentalEvaluations.GetDescription()} permission or Instructors may view or edit evaluations.");
				return;
			}
			using FormEvaluations FormE=new FormEvaluations();
			FormE.ShowDialog();
		}

		private void menuItemTerminal_Click(object sender,EventArgs e) {
			if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0) {
				MsgBox.Show(this,"Cannot open terminal unless process signal interval is set. To set it, go to Setup > Miscellaneous.");
				return;
			}
			if(ODBuild.IsWeb()) {
				//Thinfinity messes up window ordering so sometimes FormOpenDental is visible in Kiosk mode.
				foreach(Form form in Application.OpenForms) {
					form.Visible=false;
				}
			}
			using FormTerminal FormT=new FormTerminal();
			FormT.ShowDialog(); 
			Application.Exit();//always close after coming out of terminal mode as a safety precaution.*/
		}

		private void menuItemTerminalManager_Click(object sender,EventArgs e) {
			if(_formTerminalManager==null || _formTerminalManager.IsDisposed) {
				_formTerminalManager=new FormTerminalManager(setupMode:true);
			}
			_formTerminalManager.Show();
			_formTerminalManager.BringToFront();
		}

		private void menuItemTranslation_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormTranslationCat FormTC=new FormTranslationCat();
			FormTC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Translations");
		}

		private void menuItemLateCharges_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormLateCharges FormLC=new FormLateCharges();
			FormLC.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Late Charges window");
		}

		private void menuItemMobileSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormEServicesMobileSynch formESMobileSynch=new FormEServicesMobileSynch();
			formESMobileSynch.ShowDialog();
			ShowEServicesSetup();
		}

		private void menuItemNewCropBilling_Click(object sender,EventArgs e) {
			using FormNewCropBilling FormN=new FormNewCropBilling();
			FormN.ShowDialog();
		}

		private void menuItemPendingPayments_Click(object sender,EventArgs e) {
			FormPendingPayments FormPP=new FormPendingPayments();
			FormPP.Show();//Non-modal so the user can view the patient's account
		}

		private void menuItemRepeatingCharges_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RepeatChargeTool)) {
				return;
			}
			using FormRepeatChargesUpdate FormR=new FormRepeatChargesUpdate();
			FormR.ShowDialog();
		}

		private void menuItemResellers_Click(object sender,EventArgs e) {
			using FormResellers FormR=new FormResellers();
			FormR.ShowDialog();
		}

		private void menuItemScreening_Click(object sender,System.EventArgs e) {
			using FormScreenGroups FormS=new FormScreenGroups();
			FormS.ShowDialog();
		}

		private void menuItemReqStudents_Click(object sender,EventArgs e) {
			Provider prov=Providers.GetProv(Security.CurUser.ProvNum);
			if(prov==null) {
				MsgBox.Show(this,"The current user is not attached to a provider. Attach the user to a provider to gain access to this feature.");
				return;
			}
			if(!prov.IsInstructor){//if a student is logged in
				//the student always has permission to view their own requirements
				using FormReqStudentOne FormO=new FormReqStudentOne();
				FormO.ProvNum=prov.ProvNum;
				FormO.ShowDialog();
				return;
			}
			if(prov.IsInstructor) {
				using FormReqStudentsMany FormM=new FormReqStudentsMany();
				FormM.ShowDialog();
			}
		}

		private void menuItemWebForms_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.WebFormAccess)) {
				return;
			}
			FormWebForms FormWF = new FormWebForms();
			FormWF.Show();
		}

		private void menuItemWiki_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormOpenDental.menuItemWiki_Click")) {
				return;
			}
			new FormWiki().Show();//allow multiple
		}

		private void menuItemXWebTrans_Click(object sender,EventArgs e) {
			if(_formXWebTransactions==null || _formXWebTransactions.IsDisposed) {
				_formXWebTransactions=new FormXWebTransactions();
				_formXWebTransactions.FormClosed+=new FormClosedEventHandler((o,e1) => { _formXWebTransactions=null; });
				_formXWebTransactions.Show();
			}
			if(_formXWebTransactions.WindowState==FormWindowState.Minimized) {
				_formXWebTransactions.WindowState=FormWindowState.Normal;
			}
			_formXWebTransactions.BringToFront();
		}

		private void menuItemZoom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Zoom)) {
				return;
			}
			SizeF size96Old=new SizeF(LayoutManager.UnscaleF(Width),LayoutManager.UnscaleF(Height));
			using FormZoom formZoom=new FormZoom();
			formZoom.ShowDialog();
			if(formZoom.DialogResult!=DialogResult.OK){
				return;
			}
			//Don't send WndProc for WM_DPICHANGED because it didn't change.
			if(WindowState==FormWindowState.Maximized){
				LayoutManager.LayoutFormBoundsAndFonts(this);
				LayoutControls();
			}
			else{
				//new Size(_sizeClientOriginal.Width+_widthBorder96*2,_sizeClientOriginal.Height+_widthBorder96+_heightTitleBar96);
				//Size size96=LayoutManager.GetFormSize96();
				//(int)Math.Round(val96*ScaleMy());
				this.Size=new Size(LayoutManager.Scale(size96Old.Width),LayoutManager.Scale(size96Old.Height));
			}
			Plugins.HookAddCode(this,"FormOpenDental.menuItemZoom_Click_end",this.Menu);
			if(userControlDashboard.Visible){
				MsgBox.Show("Please restart to fix the Dashboard layout.");
			}
		}
		

		public static void S_WikiLoadPage(string pageTitle) {
			if(!PrefC.GetBool(PrefName.WikiCreatePageFromLink) && !WikiPages.CheckPageNamesExist(new List<string>{ pageTitle })[0]) {
				MsgBox.Show("FormOpenDental","Wiki page does not exist.");
				return;
			}
			FormWiki FormW=new FormWiki();
			FormW.Show();
			FormW.LoadWikiPagePublic(pageTitle);//This has to be after the form has loaded
		}

		public static void S_TaskNumLoad(long taskNum) {
			Task task=Tasks.GetOne(taskNum);
			if(task==null) {
				MsgBox.Show("FormOpenDental","Task does not exist.");
				return;
			}
			FormTaskEdit FormTE=new FormTaskEdit(task);
			FormTE.Show();
		}

		private void menuItemAutoClosePayPlans_Click(object sender,EventArgs e) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				MsgBox.Show(this,"Tool does not currently support Oracle.  Please call support to see if you need this fix.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Running this tool will automatically mark all payment plans that have"
				+" been paid off and have no future charges as closed.  Do you want to continue?")) 
			{
				return;
			}
			long plansClosed=PayPlans.AutoClose(); //returns # of payplans closed.
			string msgText;
			if(plansClosed>0) {
				msgText=Lan.g(this,"Success.")+"  "+plansClosed+" "+Lan.g(this,"plan(s) closed.");
			}
			else {
				msgText=Lan.g(this,"There were no plans to close.");
			}
			MessageBox.Show(msgText);
		}

		private void menuItemOrthoAuto_Click(object sender,EventArgs e) {
			using FormOrthoAutoClaims FormOAC = new FormOrthoAutoClaims();
			FormOAC.ShowDialog();
		}

		private void menuItemWebChatSessions_Click(object sender,EventArgs e) {
			if(_formWCT==null || _formWCT.IsDisposed) {
				_formWCT=new FormWebChatTools();
			}
			_formWCT.Show(); 
			if(_formWCT.WindowState==FormWindowState.Minimized) {
				_formWCT.WindowState=FormWindowState.Normal;
			}
			_formWCT.BringToFront();
		}

		private void menuItemWebChatSurveys_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			if(_formWebChatSurveys==null || _formWebChatSurveys.IsDisposed) {
				_formWebChatSurveys=new FormWebChatSurveys();
			}
			_formWebChatSurveys.Show(); 
			if(_formWebChatSurveys.WindowState==FormWindowState.Minimized) {
				_formWebChatSurveys.WindowState=FormWindowState.Normal;
			}
			_formWebChatSurveys.BringToFront();
		}

		private void menuItemMassEmails_Click(object sender,EventArgs e) {
			if(!Clinics.IsMassEmailSignedUp(Clinics.ClinicNum)) {
				using FormMassEmailSetup formMassEmailsSetup=new FormMassEmailSetup();
				formMassEmailsSetup.ShowDialog();
			}
			using FormMassEmailSend formMassEmailSend=new FormMassEmailSend();
			formMassEmailSend.ShowDialog();
		}
		#endregion Menu - Tools

		#region Menu - Clinics
		//menuClinics is a dynamic menu that is maintained within RefreshMenuClinics()
		#endregion Menu - Clinics

		#region Menu - Dashboard
		private void RefreshMenuDashboards() {
			List<SheetDef> listDashboards=SheetDefs.GetWhere(x => x.SheetType==SheetTypeEnum.PatientDashboardWidget 
				&& Security.IsAuthorized(Permissions.DashboardWidget,x.SheetDefNum,true),true);
			bool isAuthorizedForSetup=Security.IsAuthorized(Permissions.Setup,true);
			this.InvokeIfRequired(() => {
				_menuItemPatDashboards.DropDown.Items.Clear();
				if(listDashboards.Count>28) {//This number of items+line+Setup will fit in a 990x735 form.
					_menuItemPatDashboards.Click-=OpenDashboardSelect;//Make sure we only subscribe once.
					_menuItemPatDashboards.Click+=OpenDashboardSelect;
					return;
				}
				List<long> listOpenDashboardsSheetDefNums=userControlDashboard.ListOpenWidgets.Select(x => x.SheetDefWidget.SheetDefNum).ToList();
				MenuItemOD menuItem=new MenuItemOD(Lan.g("MainMenu","Dashboard Setup"),OpenDashboardSetup);
				if(!isAuthorizedForSetup) {
					menuItem.Enabled=false;
				}
				_menuItemPatDashboards.Add(menuItem);
				if(listDashboards.Count>0) {
					_menuItemPatDashboards.AddSeparator();
				}
				foreach(SheetDef dashboardDef in listDashboards) {
					menuItem=new MenuItemOD(dashboardDef.Description,DashboardMenuClick);
					menuItem.Tag=dashboardDef;
					if(ListTools.In(dashboardDef.SheetDefNum,listOpenDashboardsSheetDefNums)) {//Currently open Dashboard.
						menuItem.Checked=true;
					}
					_menuItemPatDashboards.Add(menuItem);
				}
			});
		}

		private void OpenDashboardSelect(object sender,EventArgs e) {
			using FormDashboardWidgets formDashboards=new FormDashboardWidgets();//Open the LaunchDashboard window.
			if(formDashboards.ShowDialog()==DialogResult.OK && formDashboards.SheetDefDashboardWidget!=null) {
				TryLaunchPatientDashboard(formDashboards.SheetDefDashboardWidget);
			}
			RefreshMenuDashboards();
		}

		private void OpenDashboardSetup(object sender,System.EventArgs e) {
			using FormDashboardWidgetSetup formDS=new FormDashboardWidgetSetup();
			formDS.ShowDialog();
			RefreshMenuDashboards();
		}

		///<summary>Opens a UserControlDashboardWidget, closing the previously selected UserControlDashboardWidget if one is already open.  If the user clicked on the menu item corresponding to the currently open Patient Dashboard, this means "Close".</summary>
		private void DashboardMenuClick(object sender,System.EventArgs e) {
			if(sender.GetType()!=typeof(MenuItemOD) || ((MenuItemOD)sender).Tag==null || ((MenuItemOD)sender).Tag.GetType()!=typeof(SheetDef)) {
				return;
			}
			SheetDef widgetNew=(SheetDef)((MenuItemOD)sender).Tag;
			bool opened=TryLaunchPatientDashboard(widgetNew);//Open the newly selected Patient Dashboard.
			if(opened){
				MsgBox.Show(this,"You will probably need to restart to set the layout of the new Dashboard.");
			}
			else{
				//closed existing.
			}
		}

		///<summary>Opens a UserControlDashboardWidget.  The user's permissions should be validated prior to calling this method.</summary>
		private bool TryLaunchPatientDashboard(SheetDef sheetDefWidget) {
			if(userControlDashboard.IsInitialized) {
				if(userControlDashboard.ListOpenWidgets.Any(x => x.Name==POut.Long(sheetDefWidget.SheetDefNum))) {
					//Clicked on the currently open Patient Dashboard.  This means "Close the Patient Dashboard".
					userControlDashboard.CloseDashboard(false);//Causes userodpref to be deleted.
					OnResizeEnd(new EventArgs());
					return false;
				}
				//Changing which Patient Dashboard is being shown.  First add the new one, then close the old, and update user pref.
				//This order of operations helps avoid unnecessary UI flicker and slowness because we don't actually close the entire Dashboard control.
				UserOdPref userPrefDashboard=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.Dashboard).FirstOrDefault();
				List<UserControlDashboardWidget> listOpenWidgets=userControlDashboard.ListOpenWidgets;
				userControlDashboard.AddWidget(sheetDefWidget);
				for(int w=0;w<listOpenWidgets.Count;w++){
					listOpenWidgets[w].CloseWidget();
				}
				ResizeDashboard();
				UserOdPref userOdPrefOld=userPrefDashboard.Clone();
				userPrefDashboard.Fkey=sheetDefWidget.SheetDefNum;
				if(UserOdPrefs.Update(userPrefDashboard,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
				RefreshMenuDashboards();
			}
			else {
				UserOdPref userPrefDashboard=new UserOdPref() {//If Patient Dashboard was not open, so we need a new user pref for the current user.
					UserNum=Security.CurUser.UserNum,
					Fkey=sheetDefWidget.SheetDefNum,
					FkeyType=UserOdFkeyType.Dashboard,
					ClinicNum=Clinics.ClinicNum
				};
				if(Security.CurUser.UserNum!=0){//If the userNum is 0 for the following command it will delete all Patient Dashboard UserOdPrefs!
					//if any Patient Dashboard UserOdPrefs already exists for this user, remove them. This could happen due to a previous concurrency bug.
					UserOdPrefs.DeleteForValueString(Security.CurUser.UserNum,UserOdFkeyType.Dashboard,"");
				}
				userPrefDashboard.UserOdPrefNum=UserOdPrefs.Insert(userPrefDashboard);//Pre-insert for PK.
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
				try {
					InitDashboards(Security.CurUser.UserNum,userPrefDashboard);
				}
				catch(NotImplementedException niex) {
					MessageBox.Show(this,"Error loading Patient Dashboard:\r\n"+niex.Message+"\r\nCorrect errors in Dashboard Setup.");
				}
				catch(Exception ex) {
					throw new Exception("Unexpected error loading Patient Dashboard: "+ex.Message,ex);//So we get bug submission.
				}
			}
			//LayoutManager.SynchRecursive(userControlDashboard);//We accept the bounds of every single child at this point, passing them all to the LayoutManager so that we can handle resizes.
			//LayoutManager.LayoutFormBoundsAndFonts(this);//splitContainerNoFlickerDashboard);
			//LayoutControls();
			OnResizeEnd(new EventArgs());
			return userControlDashboard.IsInitialized;
		}

		///<summary>Determines if there is a user preference for which Dashboard to open on startup, and launches it if the user has permissions to launch the dashboard.</summary>
		private void InitDashboards(long userNum,UserOdPref userPrefDashboard=null) {
			bool isOpenedManually=(userPrefDashboard!=null);
			userPrefDashboard=userPrefDashboard??UserOdPrefs.GetByUserAndFkeyType(userNum,UserOdFkeyType.Dashboard).FirstOrDefault();
			if(userPrefDashboard==null) {
				return;//User didn't have the dashboard open the last time logged out.
			}
			if(userControlTasks1.Visible && ComputerPrefs.LocalComputer.TaskDock==1) {//Tasks are docked right
				this.InvokeIfRequired(() => {
					MsgBox.Show(this,"Dashboards are disabled when Tasks are docked to the right.");
					if(Security.CurUser.UserNum!=0){//If the userNum is 0 for the following command it will delete all Patient Dashboard UserOdPrefs!
						//Stop the Patient Dashboard from attempting to open on next login.
						UserOdPrefs.DeleteForValueString(Security.CurUser.UserNum,UserOdFkeyType.Dashboard,"");
						DataValid.SetInvalid(InvalidType.UserOdPrefs);
					}
				});
				return;
			}
			SheetDef sheetDefDashboard=GetUserDashboard(userPrefDashboard);
			if(sheetDefDashboard==null) {//Couldn't find the SheetDef, no sense trying to initialize the Patient Dashboard.
				if(isOpenedManually) {//Only prompt if user attempted to open a Patient Dashboard from the menu.
					this.InvokeIfRequired(() => {
						MsgBox.Show(this,"Patient Dashboard could not be found.");
					});
				}
				return;
			}
			//Pass in SheetDef describing Dashboard layout.
			userControlDashboard.Initialize(sheetDefDashboard,() => { this.InvokeIfRequired(() => LayoutControls()); }
				,() => {//What to do when the user closes the dashboard.
					if(Security.CurUser.UserNum!=0){//If the userNum is 0 for the following command it will delete all Patient Dashboard UserOdPrefs!
						//Stop the Patient Dashboard from attempting to open on next login.
						UserOdPrefs.DeleteForValueString(Security.CurUser.UserNum,UserOdFkeyType.Dashboard,"");
						DataValid.SetInvalid(InvalidType.UserOdPrefs);
					}
					RefreshMenuDashboards();
					if(controlAppt.Visible) {//Ensure appointment view redraws.
						controlAppt.LayoutControls();
					}
				}
			);
			//this.InvokeIfRequired(() => {
				//moving this to LayoutControls, which is a specified action 14 lines up.
				//LayoutControls will malfunction if LayoutManager has not synched the new controls.
			//	LayoutManager.SynchRecursive(userControlDashboard);
			//});
			RefreshMenuDashboards();
		}

		///<summary>Gets the current user's Patient Dashboard SheetDef.  Returns null if the SheetDef linked via userPrefDashboard does not exist.
		///If the Patient Dashboard SheetDef linked via userPrefDashboard no longer exists, userPrefDashboard is deleted.</summary>
		private static SheetDef GetUserDashboard(UserOdPref userPrefDashboard) {
			if(userPrefDashboard==null) {
				return null;
			}
			long sheetDefDashboardNum=userPrefDashboard.Fkey;
			SheetDef sheetDefDashboard=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==sheetDefDashboardNum);
			if(sheetDefDashboard==null) {
				//The linked Patient Dashboard for this user no longer exists.  Clean up the UserOdPref.
				if(userPrefDashboard.UserNum!=0) {//Defensive to ensure all Patient Dashboard userprefs are not deleted.
					UserOdPrefs.DeleteForValueString(userPrefDashboard.UserNum,UserOdFkeyType.Dashboard,string.Empty);//All Dashboard userodprefs for this user.
				}
				else {
					UserOdPrefs.Delete(userPrefDashboard.UserOdPrefNum);//Otherwise, be safe and only delete this one userpref.
				}
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else if(sheetDefDashboard.SheetType==SheetTypeEnum.PatientDashboard) {
				//Previously, users could open multiple Patient Dashboard SheetDefs as UserControlDashboardWidgets within the UserControlDashboard container.
				//These UserControlDashboardWidgets could be arranged by dragging/dropping them around the container, and this layout was saved on a user by
				//user basis.
				//In E13631, we only allow one UserControlDashboardWidget to be open at a time, and in E13629 we removed the drag/drop functionality.  This
				//eliminates the need for a separate SheetDef to save the user's Patient Dashboard layout.  However, we want to as seemlessly as possible
				//transition users away from the multiple Patient Dashboard functionality, ideally without any interruption in user experience.
				//If the user had a saved layout, we will pick the first (hopefully only) UserControlDashboardWidget/SheetDef and use it as the last opened
				//Patient Dashboard, simultaneously removing the user specific layout SheetDef and linking via userPrefDashboard to the first 
				//UserControlDashboard that was previoulsy open. This will allow us to remove the obsolete layout SheetDef, as well as save the selected
				//Patient Dashboard for the user.
				SheetDefs.GetFieldsAndParameters(sheetDefDashboard);
				//FieldValue corresponds to the Patient Dashboard widget SheetDef.SheetDefNum
				long firstWidgetSheetDefNum=PIn.Long(sheetDefDashboard.SheetFieldDefs.FirstOrDefault().FieldValue);
				SheetDefs.DeleteObject(sheetDefDashboard.SheetDefNum);//Delete the layout SheetDef.
				sheetDefDashboard=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==firstWidgetSheetDefNum);
				UserOdPref userOdPrefOld=userPrefDashboard.Clone();
				userPrefDashboard.Fkey=firstWidgetSheetDefNum;//May not exist.  Will get cleaned up later.
				if(UserOdPrefs.Update(userPrefDashboard,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
			return sheetDefDashboard;
		}

		///<summary>Determines if the Dashboard is currently visible.</summary>
		public static bool IsDashboardVisible {
			get {
				return (!_formOpenDentalSingleton.splitContainerNoFlickerDashboard.Panel2Collapsed && _formOpenDentalSingleton.userControlDashboard.IsInitialized);
			}
		}
		#endregion Menu - Dashboard

		#region Menu - eServices
		private void menuItemEServices_Click(object sender,EventArgs e) {
			using FormEServicesSetup formESSetup=new FormEServicesSetup();
			formESSetup.ShowDialog();
		}

		private void ShowEServicesSetup() {
			if(_toolBarButtonText!=null) { //User may just have signed up for texting.
				_toolBarButtonText.Enabled=Programs.IsEnabled(ProgramName.CallFire)||SmsPhones.IsIntegratedTextingEnabled();
			}
		}
		#endregion Menu - eServices

		#region Menu - Alerts
		private void menuItemAlerts_Click(object sender,EventArgs e) {
			_formAlerts=new FormAlerts();//Disposed a few lines down. Unusual situation.
			_formAlerts.ListAlertItems=_listAlertItems;
			_formAlerts.ListAlertReads=_listAlertReads;
			_formAlerts.ClickAlert+=FormAlerts_Click;
			_formAlerts.ShowDialog();//modal
			if(_formAlerts.ActionTypeCur!=ActionType.OpenForm){
				return;
			}
			_formAlerts.Dispose();
			//Open form:
			AlertItem alertItem=_formAlerts.AlertItemCur;
			List<long> listAlertItemNums=(List<long>)alertItem.TagOD;
			AlertReadsHelper(listAlertItemNums);
			CheckAlerts();
			switch(alertItem.FormToOpen) {
				case FormType.FormPendingPayments:
					FormPendingPayments FormPP=new FormPendingPayments();
					FormPP.Show();//Non-modal so the user can view the patient's account
					FormPP.FormClosed+=this.alertFormClosingHelper;
					break;
				case FormType.FormEServicesWebSchedRecall:
					using(FormEServicesWebSchedRecall formEServicesWebSchedRecall=new FormEServicesWebSchedRecall()) {
						formEServicesWebSchedRecall.ShowDialog();
					}
					ShowEServicesSetup();
					break;
				case FormType.FormRadOrderList:
					List<FormRadOrderList> listFormROLs=Application.OpenForms.OfType<FormRadOrderList>().ToList();
					if(listFormROLs.Count > 0) {
						listFormROLs[0].RefreshRadOrdersForUser(Security.CurUser);
						listFormROLs[0].BringToFront();
					}
					else {
						FormRadOrderList FormROL=new FormRadOrderList(Security.CurUser);
						FormROL.Show();
						FormROL.FormClosed+=this.alertFormClosingHelper;
					}
					break;
				case FormType.FormEServicesSignupPortal:
					using(FormEServicesSignup formESSignup=new FormEServicesSignup()) {
						formESSignup.ShowDialog();
					}
					ShowEServicesSetup();
					break;
				case FormType.FormEServicesWebSchedNewPat:
					using(FormEServicesWebSchedPat formEServicesWebSchedPat=new FormEServicesWebSchedPat(true)) {
						formEServicesWebSchedPat.ShowDialog();
					}
					ShowEServicesSetup();
					break;
				case FormType.FormEServicesEConnector:
					using(FormEServicesEConnector formESEConnector=new FormEServicesEConnector()) {
						formESEConnector.ShowDialog();
					}
					ShowEServicesSetup();
					break;
				case FormType.FormApptEdit:
					Appointment appt=Appointments.GetOneApt(alertItem.FKey);
					Patient pat=Patients.GetPat(appt.PatNum);
					S_Contr_PatientSelected(pat,false);
					FormApptEdit FormAE=new FormApptEdit(appt.AptNum);//Dispose below due to local variable.
					FormAE.ShowDialog();
					FormAE.Dispose();
					break;
				case FormType.FormWebSchedAppts:
					FormWebSchedAppts FormWebSchedAppts=new FormWebSchedAppts(alertItem.Type==AlertType.WebSchedNewPatApptCreated,
						alertItem.Type==AlertType.WebSchedRecallApptCreated,alertItem.Type==AlertType.WebSchedASAPApptCreated,alertItem.Type==AlertType.WebSchedExistingPatApptCreated);
					FormWebSchedAppts.Show();
					break;
				case FormType.FormPatientEdit:
					if(!Security.IsAuthorized(Permissions.PatientEdit)) {
						return;
					}
					pat=Patients.GetPat(alertItem.FKey);
					Family fam=Patients.GetFamily(pat.PatNum);
					S_Contr_PatientSelected(pat,false);
					using(FormPatientEdit FormPE=new FormPatientEdit(pat,fam)) {
						FormPE.ShowDialog();
					}
					break;
				case FormType.FormDoseSpotAssignUserId:
					if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
						break;
					}
					using(FormDoseSpotAssignUserId FormAU=new FormDoseSpotAssignUserId(alertItem.FKey)) {
						FormAU.ShowDialog();
					}
					break;
				case FormType.FormDoseSpotAssignClinicId:
					if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
						break;
					}
					using(FormDoseSpotAssignClinicId FormACI=new FormDoseSpotAssignClinicId(alertItem.FKey)) {
						FormACI.ShowDialog();
					}
					break;
				case FormType.FormEmailInbox:
					//Will open the email inbox form and set the current inbox to "WebMail".
					FormEmailInbox FormEI=new FormEmailInbox("WebMail");
					FormEI.FormClosed+=this.alertFormClosingHelper;
					FormEI.Show();
					break;
				case FormType.FormEmailAddresses:
					//Will open the email addresses window that is usually opened from email inbox setup.
					using(FormEmailAddresses formEA=new FormEmailAddresses()) {
						formEA.FormClosed+=this.alertFormClosingHelper;
						formEA.ShowDialog();
					}
					break;
				case FormType.FormCareCreditTransactions:
					FormCareCreditTransactions FormCCT=new FormCareCreditTransactions();
					FormCCT.FormClosed+=this.alertFormClosingHelper;
					FormCCT.Show();
					break;
				case FormType.FormCloudManagement:
					using(FormCloudManagement formCloudManagement=new FormCloudManagement()) {
						formCloudManagement.FormClosed+=this.alertFormClosingHelper;
						formCloudManagement.ShowDialog();
					}
					break;
				case FormType.FormWebForms:
					if(!Security.IsAuthorized(Permissions.WebFormAccess)) {
						break;
					}
					using(FormWebForms formWebForms=new FormWebForms()) {
						formWebForms.FormClosed+=this.alertFormClosingHelper;
						formWebForms.ShowDialog();
					}
					break;					
				case FormType.FormModuleSetup:
					LaunchModuleSetupWithTab((int)alertItem.FKey);
					break;
			}
		}

		public void FormAlerts_Click(object sender,EventArgs e) {
			AlertItem alertItem=_formAlerts.AlertItemCur;
			ActionType actionType=_formAlerts.ActionTypeCur;
			//The TagOD on the alert is the list of AlertItemNums for all alerts that are its duplicate.
			List<long> listAlertItemNums=(List<long>)alertItem.TagOD;
			if(actionType==ActionType.MarkAsRead) {
				AlertReadsHelper(listAlertItemNums);
				CheckAlerts();
				_formAlerts.ListAlertReads=_listAlertReads;
				_formAlerts.FillGrid();
				return;
			}
			if(actionType==ActionType.Delete) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will delete the alert for all users. Are you sure you want to delete it?")) {
					return;
				}
				AlertItems.Delete(listAlertItemNums);
				CheckAlerts();
				_formAlerts.ListAlertItems=_listAlertItems;
				_formAlerts.ListAlertReads=_listAlertReads;
				_formAlerts.FillGrid();
				return;
			}
			if(actionType==ActionType.ShowItemValue) {
				AlertReadsHelper(listAlertItemNums);
				CheckAlerts();
				_formAlerts.ListAlertReads=_listAlertReads;
				_formAlerts.FillGrid();
				MsgBoxCopyPaste msgBCP=new MsgBoxCopyPaste($"{alertItem.Description}\r\n\r\n{alertItem.ItemValue}");
				msgBCP.Show();
			}
		}

		///<summary>This is used to force the alert logic to run on the server in OpenDentalService.
		///OpenDentalService Alerts logic will re run on signal update interval time.
		///This could be enhanced eventually only invalidate when something from the form changed.</summary>
		private void alertFormClosingHelper(object sender,FormClosedEventArgs e) {
			DataValid.SetInvalid(InvalidType.AlertItems);//THIS IS NOT CACHED. But is used to make server run the alert logic in OpenDentalService.
		}

		///<summary>Refreshes AlertReads for current user and creates a new one if one does not exist for given alertItem.</summary>
		private void AlertReadsHelper(List<long> listAlertItemNums) {			
			listAlertItemNums.RemoveAll(x => _listAlertReads.Exists(y => y.AlertItemNum==x));//Remove all the ones the user has already read.
			listAlertItemNums.ForEach(x => AlertReads.Insert(new AlertRead(x,Security.CurUser.UserNum)));
		}
		#endregion Menu - Alerts

		#region Menu - Help
		private void menuItemRemote_Click(object sender,System.EventArgs e) {
			string site="http://www.opendental.com/contact.html";
			if(Programs.GetCur(ProgramName.BencoPracticeManagement).Enabled) {
				site="https://support.benco.com/";
			}
			try {
				Process.Start(site);
			}
			catch(Exception) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+site+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
			/*
			if(!MsgBox.Show(this,true,"A remote connection will now be attempted. Do NOT continue unless you are already on the phone with us.  Do you want to continue?"))
			{
				return;
			}
			try{
				Process.Start("remoteclient.exe");//Network streaming remote client or any other similar client
			}
			catch{
				MsgBox.Show(this,"Could not find file.");
			}*/
		}

		private void menuItemHelpWindows_Click(object sender, System.EventArgs e) {
			try{
				Process.Start("Help.chm");
			}
			catch{
				MsgBox.Show(this,"Could not find file.");
			}
		}

		private void menuItemHelpContents_Click(object sender, System.EventArgs e) {
			try{
				Process.Start("https://www.opendental.com/manual/manual.html");
			}
			catch{
				MsgBox.Show(this,"Could not find file.");
			}
		}

		private void menuItemHelpIndex_Click(object sender, System.EventArgs e) {
			try{
				Process.Start("https://www.opendental.com/site/searchsite.html");
			}
			catch{
				MsgBox.Show(this,"Could not find file.");
			}
		}
		
		private void menuItemWebinar_Click(object sender,EventArgs e) {
			try{
				Process.Start("https://opendental.com/webinars/webinars.html");
			}
			catch{
				MsgBox.Show(this,"Could not open page.");
			}
		}

		private void MenuItemQueryMonitor_Click(object sender,EventArgs e) {
            if(!Security.IsAuthorized(Permissions.QueryMonitor)) {
				return;
            }
			SecurityLogs.MakeLogEntry(Permissions.QueryMonitor,0,"Query Monitor opened.");
			new FormQueryMonitor().Show();//Mildly annoying because this tool cannot be interacted with when other windows utilize ShowDialog()...
			/*********************************************************************************************************************************
				//The following code does not work.  Well, technically it works but ODGrid.OnPaint() is not thread safe so it crashes after a while.
				ODThread threadQueryMonitor=new ODThread((o) => {
					using FormQueryMonitor FormQM=new FormQueryMonitor();
					Application.Run(FormQM);//FormQM.ShowDialog() closes as soon as a button is clicked.  Give the query monitor its own OS Message Queue.
				});
				threadQueryMonitor.AddExceptionHandler((ex) => ex.DoNothing());
				threadQueryMonitor.SetApartmentState(ApartmentState.STA);
				threadQueryMonitor.Name=$"QueryMonitorThread_{DateTime.Now.Ticks}";
				threadQueryMonitor.Start();
			**********************************************************************************************************************************/
		}

		private void menuItemRequestFeatures_Click(object sender,EventArgs e) {
			FormFeatureRequest FormF=new FormFeatureRequest();
			FormF.Show();
		}

		private void MenuItemSupportStatus_Click(object sender,EventArgs e) {
			FormSupportStatus formSS=new FormSupportStatus();
			formSS.Show();
		}

		private void menuItemUpdate_Click(object sender, System.EventArgs e) {
			//If A to Z folders are disabled, this menu option is unavailable, since
			//updates are handled more automatically.
			using FormUpdate FormU = new FormUpdate();
			FormU.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Update Version");
		}

		private void menuItemAbout_Click(object sender, System.EventArgs e) {
			using FormAbout FormA=new FormAbout();
			FormA.ShowDialog();
		}
		#endregion Menu - Help

		#region HQ-only metrics

		///<summary>Filtered all the way up from MapAreaRoomControl.</summary>
		private void FormMapHQ_RoomControlClicked(object sender,EventArgs e) {
			MapAreaRoomControl clickedPhone=(MapAreaRoomControl)sender;
			if(clickedPhone==null) {
				return;
			}
			if(!clickedPhone.IsFlashing && clickedPhone.Status!="OnWay") { //If OnWay we're setting them to available, dont return.
				return;
			}
			//all we need to do is the Database and let ProcessHQMetrics handle the rest.
			Phones.SetPhoneStatus(ClockStatusEnum.HelpOnTheWay,PIn.Int(clickedPhone.Extension));
			ODThread.WakeUpThreadsByGroupName(FormODThreadNames.HqMetrics.GetDescription());
		}

		private void FormMapHQ_ExtraMapClicked(object sender,EventArgs e) {
			FormMapHQ formMapHQ;
			formMapHQ=new FormMapHQ();
			formMapHQ.RoomControlClicked+=FormMapHQ_RoomControlClicked;
			formMapHQ.ExtraMapClicked+=FormMapHQ_ExtraMapClicked;
			formMapHQ.GoToChanged+=MapAreaRoomControl_GoToChanged;
			formMapHQ.Show();
			formMapHQ.BringToFront();
		}

		/// <summary>HQ Only. ProcessOfficeDowns must be invoked from a worker thread. These are the arguments necessary.</summary>
		protected delegate void ProcessOfficeDownArgs(List<Task> listOfficeDowns);

		///<summary>HQ Only.  Every 1.6 seconds. Digest results of ProcessHqMetrics and update form controls accordingly.  phoneList is the list of all phone rows just pulled from the database.  phone is the one that we should display here, and it can be null.</summary>
		private void ProcessHqMetricsResults(List<PhoneEmpDefault> phoneEmpDefaultList,List<Phone> phoneList,List<PhoneEmpSubGroup> listSubGroups,
			List<ChatUser> listChatUsers,Phone phone,bool isTriageOperator,List<WebChatSession> listWebChatSessions,
			List<PeerInfo> listRemoteSupportSessions) 
		{
			try {
				//Send the phoneList to the 2 places where it's needed.
				//1) Send to the small display in the main OD form (quick-glance).
				phoneSmall.SetPhoneList(phoneList);
				if(_formPhoneTiles!=null && !_formPhoneTiles.IsDisposed) { //2) Send to the big phones panel if it is open.
					_formPhoneTiles.SetPhoneList(phoneList,listChatUsers,listWebChatSessions,listRemoteSupportSessions);
				}
				foreach(FormMapHQ formMapHQ in _listFormMapHQs) {
					formMapHQ.SetPhoneList(phoneList,listSubGroups,listChatUsers,listWebChatSessions,listRemoteSupportSessions);
				}
				//Now set the small display's current phone extension info.
				long employeeNum=0;
				ChatUser chatUserCur;
				WebChatSession webChatSession=null;
				PeerInfo remoteSupportSession=null;
				if(phone==null) {
					phoneSmall.Extension=0;
					if(Security.CurUser!=null) {
						employeeNum=Security.CurUser.EmployeeNum;
					}
					chatUserCur=null;
				}
				else {
					phoneSmall.Extension=phone.Extension;
					employeeNum=phone.EmployeeNum;
					chatUserCur=listChatUsers.Where(x => x.Extension == phone.Extension).FirstOrDefault();
					webChatSession=listWebChatSessions.FirstOrDefault(x => x.TechName==phone.EmployeeName);
					remoteSupportSession=listRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
				}
				phoneSmall.SetPhone(phone,PhoneEmpDefaults.GetEmpDefaultFromList(employeeNum,phoneEmpDefaultList),chatUserCur,isTriageOperator,webChatSession,
					remoteSupportSession);
				if(!_formJobManager?.IsDisposed??false) {
					_formJobManager.SetEmployeeStatus(phone);
				}
			}
			catch(Exception e) {
				e.DoNothing();
				//HQ users are complaining of unhandled exception when they close OD.
				//Suspect it could be caused here if the thread tries to access a control that has been disposed.
			}
		}

		///<summary>HQ Only. Send the office downs to any Call Center Maps that are open.</summary>
		private void ProcessOfficeDowns(List<Task> listOfficeDowns) {
			try {
				foreach(FormMapHQ formMapHQ in _listFormMapHQs) {
					formMapHQ.SetOfficesDownList(listOfficeDowns);
				}
			}
			catch {
				//HQ users are complaining of unhandled exception when they close OD.
				//Suspect it could be caused here if the thread tries to access a control that has been disposed.
			}
		}

		public static void AddMapToList(FormMapHQ map) {
			_listFormMapHQs.Add(map);
		}

		public static void RemoveMapFromList(FormMapHQ map) {
			_listFormMapHQs.Remove(map);
		}

		/// <summary>HQ Only. OnFillTriageLabelsResults must be invoked from a worker thread. These are the arguments necessary.</summary>
		private delegate void FillTriageLabelsResultsArgs(TriageMetric triageMetric);

		/// <summary>HQ Only. Digest results of Phones.GetTriageMetrics() in ProcessHqMetrics(). Fills the triage labels and update form controls accordingly.</summary>
		private void OnFillTriageLabelsResults(TriageMetric triageMetric) {
			int countBlueTasks=triageMetric.CountBlueTasks;
			int countWhiteTasks=triageMetric.CountWhiteTasks;
			int countRedTasks=triageMetric.CountRedTasks;
			DateTime timeOfOldestBlueTaskNote=triageMetric.DateTimeOldestTriageTaskOrTaskNote;
			DateTime timeOfOldestRedTaskNote=triageMetric.DateTimeOldestUrgentTaskOrTaskNote;
			TimeSpan triageBehind=new TimeSpan(0);
			if(timeOfOldestBlueTaskNote.Year>1880 && timeOfOldestRedTaskNote.Year>1880) {
				if(timeOfOldestBlueTaskNote<timeOfOldestRedTaskNote) {
					triageBehind=DateTime.Now-timeOfOldestBlueTaskNote;
				}
				else {//triageBehind based off of older RedTask
					triageBehind=DateTime.Now-timeOfOldestRedTaskNote;
				}
			}
			else if(timeOfOldestBlueTaskNote.Year>1880) {
				triageBehind=DateTime.Now-timeOfOldestBlueTaskNote;
			}
			else if(timeOfOldestRedTaskNote.Year>1880) {
				triageBehind=DateTime.Now-timeOfOldestRedTaskNote;
			}
			string countStr="0";
			if(countBlueTasks>0 || countRedTasks>0) {//Triage show red so users notice more.
				countStr=(countBlueTasks+countRedTasks).ToString();
				labelTriage.ForeColor=Color.Firebrick;
			}
			else {
				if(countWhiteTasks>0) {
					countStr="("+countWhiteTasks.ToString()+")";
				}
				labelTriage.ForeColor=Color.Black;
			}
			labelTriage.Text="T:"+countStr;
			labelWaitTime.Text=((int)triageBehind.TotalMinutes).ToString()+"m";
			foreach(FormMapHQ formMapHQ in _listFormMapHQs) {
				formMapHQ.SetTriageNormal(countWhiteTasks,countBlueTasks,triageBehind,countRedTasks);
				TimeSpan urgentTriageBehind=new TimeSpan(0);
				if(timeOfOldestRedTaskNote.Year>1880) {
					urgentTriageBehind=DateTime.Now-timeOfOldestRedTaskNote;
				}
				formMapHQ.SetTriageUrgent(countRedTasks,urgentTriageBehind);
				formMapHQ.SetChatCount();
			}
		}
		#endregion HQ-only metrics

		#region LogOn
		///<summary>Logs on a user using the passed in credentials or Active Directory or the good old-fashioned log on window.</summary>
		private void LogOnOpenDentalUser(string odUser,string odPassword,string domainUserFromCmd) {
			//CurUser will be set if using web service because login from ChooseDatabase window.
			if(Security.CurUser!=null) {
				if(!IsCloudUserIpAllowed()) {
					ShowLogOn();
				}
				else {
					CheckForPasswordReset();
				}
				Security.IsUserLoggedIn=true;
				SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "+Lan.g(this,"has logged on."));
				return;
			}
			#region eCW Tight or Full
			//Leave Security.CurUser null if a user was passed in on the commandline.  If starting OD manually, it continues below.
			if(Programs.UsingEcwTightOrFullMode() && odUser!="") {
				//Purposefully leave Security.CurUser as null.
				Security.IsUserLoggedIn=true;//This might be wrong.  We set to true for backward compatibility.
				return;
			}
			#endregion
			#region Command Line Args
			//Both a username and password was passed in via command line arguments.
			if(odUser!="" && odPassword!="") {
				try {
					bool isEcwTightOrFullMode=Programs.UsingEcwTightOrFullMode();
					Security.CurUser=Userods.CheckUserAndPassword(odUser,odPassword,isEcwTightOrFullMode);
					if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
						string pw=odPassword;
						if(isEcwTightOrFullMode) {//ecw requires hash, but non-ecw requires actual password
							pw=Authentication.HashPasswordMD5(pw,true);
						}
						Security.PasswordTyped=pw;
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					Application.Exit();
					return;
				}
				if(!IsCloudUserIpAllowed()) {
					ShowLogOn();
				}
				SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "+Lan.g(this,"has logged on via command line."));
			}
			#endregion
			#region Good Old-fashioned Log On
			if(Security.CurUser==null) {//Security.CurUser could be set if valid command line arguments were passed in.
				#region Admin User No Password
				if(!Userods.HasSecurityAdminUserNoCache()) {
					MsgBox.Show(this,"There are no users with the SecurityAdmin permission.  Call support.");
					Application.Exit();
					return;
				}
				long userNumFirstAdminNoPass=Userods.GetFirstSecurityAdminUserNumNoPasswordNoCache();
				if(userNumFirstAdminNoPass > 0) {
					Security.CurUser=Userods.GetUserNoCache(userNumFirstAdminNoPass);
					if(!IsCloudUserIpAllowed()) {
						ShowLogOn();
					}
					else {
						CheckForPasswordReset();
					}
					SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "+Lan.g(this,"has logged on."));
				}
				#endregion
				#region Domain Login
				else if(PrefC.GetBool(PrefName.DomainLoginEnabled) && !string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.DomainLoginPath))) {
					string loginPath=PrefC.GetString(PrefName.DomainLoginPath);
					try {
						Logger.LogToPath("Domain Login",LogPath.Startup,LogPhase.Start);
						DirectoryEntry loginEntry=new DirectoryEntry(loginPath);
						string distinguishedName=loginEntry.Properties["distinguishedName"].Value.ToString();
						string domainGuid=loginEntry.Guid.ToString();
						string domainGuidPref=PrefC.GetString(PrefName.DomainObjectGuid);
						if(domainGuidPref.IsNullOrEmpty()) {
							//Domain login was setup before we started recording the domain's ObjectGuid. We will save it now for future use.
							Prefs.UpdateString(PrefName.DomainObjectGuid,domainGuid);
							domainGuidPref=domainGuid;
						}
						string domainUser=(domainGuidPref+'\\'+Environment.UserName);
						//All LDAP servers must expose a special entry, called the root DSE. This gets the current user's domain path.
						DirectoryEntry rootDSE=new DirectoryEntry("LDAP://RootDSE");
						string defaultNamingContext=rootDSE.Properties["defaultNamingContext"].Value.ToString();
						Logger.LogToPath("Domain Login",LogPath.Startup,LogPhase.End);
						if(!string.IsNullOrEmpty(domainUserFromCmd)) {
							domainUser=domainUserFromCmd;
						}
						else if(//If the domain of the current user doesn't match the provided LDAP Path
							!distinguishedName.ToLower().Contains(defaultNamingContext.ToLower()) ||
							//Or the domain's ObjectGuid does not match what's in the database
							domainGuid!=domainGuidPref) 
						{
							ShowLogOn();
							Security.IsUserLoggedIn=true;
							return;
						}
						SerializableDictionary<long,string> dictDomainUserNumsAndNames=Userods.GetUsersByDomainUserNameNoCache(domainUser);
						if(dictDomainUserNumsAndNames.Count==0) { //Log on normally if no user linked the current domain user
							ShowLogOn();
						}
						else if(dictDomainUserNumsAndNames.Count > 1) { //Select a user if multiple users linked to the current domain user
							using InputBox box=new InputBox(Lan.g(this,"Select an Open Dental user to log in with:"),dictDomainUserNumsAndNames.Select(x => x.Value).ToList());
							box.ShowDialog();
							if(box.DialogResult==DialogResult.OK) {
								Security.CurUser=Userods.GetUserNoCache(dictDomainUserNumsAndNames.Keys.ElementAt(box.SelectedIndex));
								if(!IsCloudUserIpAllowed()) {
									ShowLogOn();
								}
								else {
									CheckForPasswordReset();
								}
								SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "
									+Lan.g(this,"has logged on automatically via ActiveDirectory."));
							}
							else {
								ShowLogOn();
							}
						}
						else { //log on automatically if only one user is linked to current domain user
							Security.CurUser=Userods.GetUserNoCache(dictDomainUserNumsAndNames.Keys.First());
							if(!IsCloudUserIpAllowed()) {
								ShowLogOn();
							}
							else {
								CheckForPasswordReset();
							}
							SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "
									+Lan.g(this,"has logged on automatically via ActiveDirectory."));
						}
					}
					catch(Exception ex) {
						Logger.LogToPath("Domain Login Failed: "+ex.Message,LogPath.Startup,LogPhase.Unspecified);
						ShowLogOn();
						Security.IsUserLoggedIn=true;
						return;
					}
				}
				#endregion
				#region Manual LogOn Window
				else {
					ShowLogOn();
				}
				#endregion
			}
			#endregion
			Security.IsUserLoggedIn=true;//User is guaranteed to be logged in at this point.
		}

		///<summary>
		///If the user is permitted to login from this IP address, return true. Otherwise, inform the user and return false.
		///If a Security Admin does not have permission, they are prompted to add their current IP address to the list of allowed addresses.
		///</summary>
		private bool IsCloudUserIpAllowed() {
			if(!ODBuild.IsWeb()) {
				return true;
			}
			string addressCur=Browser.GetComputerIpAddress();
			CloudAddress cloudAddressCur=CloudAddresses.GetByIpAddress(addressCur);
			if(cloudAddressCur!=null) {
				cloudAddressCur.UserNumLastConnect=Security.CurUser.UserNum;
				cloudAddressCur.DateTimeLastConnect=DateTime.Now;
				CloudAddresses.Update(cloudAddressCur);
				return true;
			}
			if(Security.IsAuthorized(Permissions.AllowLoginFromAnyLocation, true)) {
				return true;
			}
			//Security Admins can add their own IP if it is not already allowed.
			if(Security.IsAuthorized(Permissions.SecurityAdmin, true)) {
				string message=$"The IP address you are attempting to connect from, {addressCur}, is not an approved address for accessing this cloud" +
					" environment. To connect from this IP address, you will need to add it to the approved list.";
				if(MsgBox.Show(MsgBoxButtons.OKCancel, message)) {
					using(FormCloudManagement formCloudManagement=new FormCloudManagement()) {
						//Either Security Admin has no password or the user must have authenticated to get to this step.
						//We can safely (temporarily) allow the cache so formCloudManagement can be shown without crashing.
						Userods.SetIsCacheAllowed(true);//Enable the cache
						formCloudManagement.ShowDialog();
						Userods.SetIsCacheAllowed(false);//Disable the cache, it will be enabled again by the calling method if it needs to be.
					}
					return IsCloudUserIpAllowed();
				}
			}
			else {
				MessageBox.Show($"The IP address you are attempting to connect from, {addressCur}, is not an approved address for accessing this cloud environment.");
			}
			return false;
		}

		///<summary>Show the log on window.</summary>
		private void ShowLogOn() {
			Logger.LogToPath("ShowLogOn",LogPath.Startup,LogPhase.Start);
			Userods.SetIsCacheAllowed(false);
			using FormLogOn FormLogOn=new FormLogOn(doRefreshSecurityCache:false);
			FormLogOn.ShowDialog(this);
			if(FormLogOn.DialogResult!=DialogResult.OK) {
				//Using FormLogOn_.DialogResult==DailogResult.CANCEL previously resulted in a null user/UE.
				Cursor=Cursors.Default;
				Application.Exit();
			}
			if(!IsCloudUserIpAllowed()) {
				ShowLogOn();
				return;
			}
			CheckForPasswordReset();
			Userods.SetIsCacheAllowed(true);
			if(FormLogOn.RefreshSecurityCache) {//Refresh the cache if we need to since cache allowed was just set to true
				DataValid.SetInvalid(InvalidType.Security);
			}
			bool needsSignature=UpdateHistories.IsLicenseAgreementNeeded();
			if(!PrefC.GetBool(PrefName.LicenseAgreementAccepted) && needsSignature) {
				PromptForLicenseSignature();
			}
			Logger.LogToPath("ShowLogOn",LogPath.Startup,LogPhase.End);
		}

		///<summary>Opens the License Agreements for the user to sign. This prompting will only display when the user is a SecurityAdmin with a valid regkey. Closes Open Dental if they refuse to sign. </summary>
		private void PromptForLicenseSignature() {
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true) && !PrefC.GetString(PrefName.RegistrationKey).IsNullOrEmpty()
				&& !ODBuild.IsTrial() && !ODBuild.IsDebug()) 
				{
				using FormRegistrationKey formRegistrationKey=new FormRegistrationKey();
				formRegistrationKey.NeedsSignature=true;
				formRegistrationKey.ShowDialog();
				if(formRegistrationKey.DialogResult!=DialogResult.OK) {
					MsgBox.Show(this,"The License Agreement must be accepted by a SecurityAdmin user. The application will now close.");
					Application.Exit();
					return;
				}
			}
		}

		///<summary>Checks to see if the currently logged-in user needs to reset their password.  If they do, then this method will force the user to reset the password otherwise the program will exit.</summary>
		private void CheckForPasswordReset() {
			if(Security.CurUser==null) {
				return;
			}
			try {
				if(Security.CurUser.IsPasswordResetRequired) {
					using FormUserPassword FormUP=new FormUserPassword(false,Security.CurUser.UserName,isPasswordReset:true);
					FormUP.ShowDialog();
					if(FormUP.DialogResult!=DialogResult.OK) {
						Cursor=Cursors.Default;
						Application.Exit();
					}
					bool isPasswordStrong=FormUP.PasswordIsStrong;
					try {
						Security.CurUser.IsPasswordResetRequired=false;
						Userods.Update(Security.CurUser);
						Userods.UpdatePassword(Security.CurUser,FormUP.LoginDetails,isPasswordStrong);
						Security.PasswordTyped=FormUP.PasswordTyped;//Update the last typed in for middle tier refresh
						Security.CurUser=Userods.GetUserNoCache(Security.CurUser.UserNum);//UpdatePassword() changes multiple fields.  Refresh from db.
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			finally {
				//Record last login time always, regardless of if we are changing password or not.
				//This catches domain logins, middle-tier, regular login, and logging in with passwords disabled.
				try {
					Security.CurUser.DateTLastLogin=DateTime.Now;
					Userods.Update(Security.CurUser);//Unfortunately there is no update(new,old) for Userods yet due to comlexity.
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			DataValid.SetInvalid(InvalidType.Security);
		}
		#endregion LogOn

		#region Logoff

		///<summary>Returns a list of forms that are currently open, excluding FormOpenDental, FormLogOn, and duplicates. This method is typically called in order to close any open forms except the aforementioned ones.  Therefore, the list returned is ordered with the intent that the calling method will close children first and then parents last.</summary>
		private List<Form> GetOpenForms() {
			if(this.InvokeRequired) {
				return (List<Form>)this.Invoke(new Func<List<Form>>(() => GetOpenForms()));
			}
			List<Form> listOpenForms=new List<Form>();
			for(int f=Application.OpenForms.Count-1;f>=0;f--) {//Loop backwards assuming children are added later in the collection.
				Form openForm=Application.OpenForms[f];
				if(listOpenForms.Contains(openForm)){//MS list has duplicates
					continue;
				}
				if(openForm==this) {// main form
					continue;
				}
				if(openForm.Name=="FormLogOn") {
					continue;
				}
				listOpenForms.Add(Application.OpenForms[f]);
			}
			return listOpenForms;
		}

		///<summary>Enumerates open forms and saves work for those forms which have a save handler.  Some forms are closed as part of saving work.</summary>
		private bool SaveWork(bool isForceClose) {
			if(this.InvokeRequired) {
				return (bool)this.Invoke(new Func<bool>(() => SaveWork(isForceClose)));
			}
			List<Form> listOpenForms=GetOpenForms();
			foreach(Form openForm in listOpenForms) {
				//If force closing, we HAVE to forcefully close everything related to Open Dental, regardless of plugins.  Otherwise, give plugins a chance to stop the log off event.
				if(!isForceClose) {
					//This hook was moved into this method so that the form closing loop could be shared.
					//It is correctly named and was not updated to say "FormOpenDental.CloseOpenForms" on purpose for backwards compatibility.
					if(Plugins.HookMethod(this,"FormOpenDental.LogOffNow_loopingforms",openForm)) {
						continue;//if some criteria are met in the hook, don't close a certain form
					}
				}
				if(openForm.Name=="FormWikiEdit") {
					if(!isForceClose) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are currently editing a wiki page and it will be saved as a draft.  Continue?")) {
							return false;//This form needs to stay open and the close operation should be aborted.
						}
					}
				}
			}
			GeneralProgramEvent.Fire(ODEventType.Shutdown,isForceClose);
			foreach(Form formToClose in listOpenForms) {
				if(formToClose.Name=="FormWikiEdit") {
					WikiSaveEvent.Fire(ODEventType.WikiSave);
				}
				if(formToClose.Name=="FormCommItem") {
					CommItemSaveEvent.Fire(ODEventType.CommItemSave, "ShutdownAllWorkstations");
				}
				if(formToClose.Name=="FormEmailMessageEdit") {
					EmailSaveEvent.Fire(ODEventType.EmailSave);
				}
			}
			return true;
		}

		///<summary>Do not call this function inside of an invoke, or else the form closing events will not return from ShowDialog() calls in time.
		///Closes all open forms except FormOpenDental.  Set isForceClose to true if you want to close all forms asynchronously.  Set 
		///forceCloseTimeoutMS when isForceClose is set to true to specify a timeout value for forms that take too long to close, e.g. a form hanging in 
		///a FormClosing event on a MessageBox.  If the timeout value is reached, the program will exit.  E.g. FormWikiEdit will ask users on closing if 
		///they are sure they want to discard unsaved work.  Returns false if there is an open form that requests attention, thus needs to stop the 
		///closing of the forms.</summary>
		public bool CloseOpenForms(bool isForceClose,int forceCloseTimeoutMS=15000,bool isSnip=false) {
			if(!SaveWork(isForceClose)) {
				return false;
			}
			List<Form> listFormsToClose=GetOpenForms();
			#region Close forms and quit threads.  Some form closing events rely on the closing events of parent forms.
			while(listFormsToClose.Count > 0) {
				Form formToClose=listFormsToClose[0];
				if(isSnip) {
					if(!formToClose.Modal) {
						listFormsToClose.Remove(formToClose);//don't close windows that are not dialogs
						continue;
					}
				}
				bool hasShown=false;
				while(!hasShown) {
					hasShown=true;//In case not inherited ODForm.
					if(formToClose is FormODBase) {
						hasShown=((FormODBase)formToClose).HasShown;
					}
					else if(formToClose.GetType().GetProperty("HasShown")!=null) {
						//Is a Form and has property HasShown => Assume is an ODForm.  Ex FormHelpBrowser is not an ODForm.
						hasShown=(bool)formToClose.GetType().GetProperty("HasShown").GetValue(formToClose);
					}
					//Window handle has not been created yet.  Calling formToClose.Invoke() will throw an InvalidOperationException.
					//GetOpenForms() will return a subset of forms which are in the Application.OpenForms array.
					//A form can only be placed into the Application.OpenForms array if Show() or ShowDialog() is called on it.
					//The fact that Show() or ShowDialog() was called on the form, means that the form constructor ran completely before showing,
					//therefore nothing in the constructor could cause the loop to lock up here.					
					//When Show() or ShowDialog() is called, the form will fire the Load() event followed by the Shown() event.
					//Therefore, a form returned by GetOpenForms() will fire the Load() event of the form, then the Shown() event.
					//If the form gets stuck in the Load() event due to an infinite or very long waiting period (ex Lan.F() lost connection to database),
					//then the main thread will lock up and CloseOpenForms() thread will not fire and we will never get here anyway,
					//unless CloseOpenForms() is called from a thread, in which case sleeping will not affect the main loop.
					//Finally, since we are here and the form will eventually fire the Shown() event, we can count on HasShown quickly becoming true.
					if(!hasShown) {
						Thread.Sleep(100);//Check 10 times per second.
					}
				}
				if(isForceClose) {
					ODThread threadCloseForm=new ODThread((o) => {
						if(!IsDisposedOrClosed(formToClose)) {
							formToClose.Invoke(formToClose.Close);
						}
					});
					threadCloseForm.Name="ForceCloseForm";
					bool hasError=false;
					threadCloseForm.AddExceptionHandler((ex) => {
						hasError=true;
						ODException.SwallowAnyException(() => {
							//A FormClosing() or FormClosed() event caused an exception.  Try to submit the exception so that we are made aware.
							BugSubmissions.SubmitException(new ODException(
									"Form failed to close when force closing.\r\n"
									+"FormName: "+formToClose.Name+"\r\n"
									+"FormType: "+formToClose.GetType().FullName+"\r\n"
									+"FormIsODForm: "+((formToClose is FormODBase)?"Yes":"No")+"\r\n"
									+"FormIsDiposed: "+(FormODBase.IsDisposedOrClosed(formToClose)?"Yes":"No")+"\r\n"
									,"",ex),
								threadCloseForm.Name);
						});
					});
					threadCloseForm.Start();
					threadCloseForm.Join(1000);//Give the form a limited amount of time to close, and continue if not responsive.
					this.Invoke(Application.DoEvents);//Run on main thread so that ShowDialog() for the form will continue in the parent context immediately.
					if(hasError || !IsDisposedOrClosed(formToClose)) {
						formToClose.Invoke(formToClose.Dispose);//If failed to close, kill window so that the ShowDialog() call can continue in parent context.
					}
					//In case the form we just closed created new popup forms inside the FormClosing or FormClosed event,
					//we need to check for newly created forms and add them to the queue of forms to close.
					//Any new forms will be closed next, so that child forms are closed as soon as possible before closing any parent forms.
					List<Form> listNewForms=GetOpenForms();
					listFormsToClose.ForEach(x => listNewForms.Remove(x));
					foreach(Form brandNewForm in listNewForms) {
						listFormsToClose.Insert(0,brandNewForm);
					}
				}
				else {//User manually chose to logoff/shutdown.  Gracefully close each window.
					//If the window which showed the messagebox popup causes the form to stay open, then stop the log off event, because the user chose to.
					formToClose.InvokeIfRequired(() => formToClose.Close());//Attempt to close the form, even if created in another thread (ex FormHelpBrowser).
					//Run Applicaiton.DoEvents() to allow the FormClosing/FormClosed events to fire in the form before checking if they have closed below.
					Application.DoEvents();//Required due to invoking.  Otherwise FormClosing/FormClosed will not fire until after we exit CloseOpenForms.
					if(!IsDisposedOrClosed(formToClose)) {
						//E.g. The wiki edit window will ask users if they want to lose their work or continue working.  This will get hit if they chose to continue working.
						return false;//This form needs to stay open and stop all other forms from being closed.
					}
				}
				listFormsToClose.Remove(formToClose);
			}
			#endregion
			return true;//All open forms have been closed at this point.
		}

		private void LogOffNow() {
			bool isForceClose=PrefC.LogOffTimer>0;
			LogOffNow(isForceClose);
		}

		public void LogOffNow(bool isForced) {
			if(!CloseOpenForms(isForced)) {
				return;//A form is still open.  Do not continue to log the user off.
			}
			FinishLogOff(isForced);
		}

		private void FinishLogOff(bool isForced) {
			if(this.InvokeRequired) {
				this.Invoke(() => { FinishLogOff(isForced); });
				return;
			}
			Plugins.HookAddCode(this,"FormOpenDental.FinishLogOff_start",isForced);//perform logoff 
			NullUserCheck("FinishLogOff");
			_moduleTypeLast=moduleBar.SelectedModule;
			moduleBar.SelectedModule=EnumModuleType.None;
			moduleBar.Invalidate();
			UnselectActive(true);
			AllNeutral();
			controlChart.UserLogOffCommited();//Ensures that we refresh view when user logs back on or a new user logs on.
			if(userControlTasks1.Visible) {
				userControlTasks1.ClearLogOff();
			}
			if(isForced) {
				SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,"User: "+Security.CurUser.UserName+" has auto logged off.");
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,"User: "+Security.CurUser.UserName+" has logged off.");
			}
			Clinics.LogOff();
			Userod oldUser=Security.CurUser;
			Security.CurUser=null;
			_listTasksReminders=null;
			_listTaskNumsNormal=null;
			controlAppt.RefreshReminders(new List<Task>());
			RefreshTasksNotification();
			Security.IsUserLoggedIn=false;
			Text=PatientL.GetMainTitle(null,0);
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Security.CurUser=oldUser;//so that the queries in FormLogOn() will work for the web service, since the web service requires a valid user to run queries.
			}
			SetTimersAndThreads(false);//Safe to stop timers since this method was invoked on the main thread if required.
			userControlDashboard.CloseDashboard(true);
			ShowLogOn();
			//If a different user logs on and they have clinics enabled, and they don't want data to persist, then clear the patient drop down history
			//since the current user may not have permission to access patients from the same clinic(s) as the old user
			if(oldUser.UserNum==Security.CurUser.UserNum || !PrefC.HasClinicsEnabled) {
				//continue //we do not need to blank out patient or menu
			}
			else if(!PrefC.GetBool(PrefName.PatientMaintainedOnUserChange)) {//not the same user and clinics are enabled. 
				PatNumCur=0;
				PatientL.RemoveAllFromMenu(menuPatient);
			}
			else {
				//Preserve data if new user is allowed to access the same clinics
				List<Clinic> listClinicsForUser=Clinics.GetAllForUserod(Security.CurUser);
				List<Patient> listMenuPatsLim=PatientL.GetPatientsLimFromMenu();
				foreach(Patient patLim in listMenuPatsLim) {
					if(!ListTools.In(patLim.ClinicNum,listClinicsForUser.Select(x => x.ClinicNum))) {
						//user is not allowed to access the clinic this patient is located in, remove.
						PatientL.RemoveFromMenu(patLim.PatNum);
						if(PatNumCur==patLim.PatNum) {
							PatNumCur=0;
						}
					}
				}
			}
			moduleBar.SelectedIndex=Security.GetModule(moduleBar.IndexOf(_moduleTypeLast));
			moduleBar.Invalidate();
			if(PrefC.HasClinicsEnabled) {
				Clinics.LoadClinicNumForUser();
				RefreshMenuClinics();
			}
			SetModuleSelected();
			Patient pat=Patients.GetPat(PatNumCur);//pat could be null
			Text=PatientL.GetMainTitle(pat,Clinics.ClinicNum);//handles pat==null by not displaying pat name in title bar
			FillPatientButton(pat);
			if(userControlTasks1.Visible) {
				userControlTasks1.InitializeOnStartup();
			}
			BeginODDashboardStarterThread();
			SetTimersAndThreads(true);//Safe to start timers since this method was invoked on the main thread if required.
			//User logged back in so log on form is no longer the active window.
			_isFormLogOnLastActive=false;
			Security.DateTimeLastActivity=DateTime.Now;
			if(moduleBar.SelectedModule==EnumModuleType.None) {
				MsgBox.Show(this,"You do not have permission to use any modules.");
			}
		}

		///<summary>Call this method in places where Security.CurUser should not be null in order to
		///notify HQ with additional information when a null Security.CurUser is detected.  Does nothing if CurUser is not null.</summary>
		private void NullUserCheck(string methodName) {
			if(Security.CurUser!=null) {
				return;
			}
			StringBuilder strBld=new StringBuilder("OpenForms:");
			foreach(Form form in Application.OpenForms) {
				strBld.Append($"\r\n  {(form==null ? "Unknown" : form.Name)}");
			}
			ODException.SwallowAnyException(() => BugSubmissions.SubmitException(new ODException("Null user detected during log off.\r\n"
				+$"Method: {methodName}\r\n"
				+$"ActiveForm.Name: {(FormODBase.ActiveForm==null ? "Unknown" : FormODBase.ActiveForm.Name)}\r\n"
				+strBld.ToString()))
			);
		}
		#endregion Logoff

		#region Closing
		///<summary>Called when a shutdown signal is found.</summary>
		private void InitiateShutdown(bool doShutdownAll=true) {
			if(timerSignals.Tag?.ToString()=="shutdown") {
				//We have already responded to the shutdown signal.
				return;
			}
			timerSignals.Enabled=false;//quit receiving signals.
			timerSignals.Tag="shutdown";
			string msg = "";
			if(!doShutdownAll) {
				msg="Your instance of Open Dental ";
			}
			else if(Process.GetCurrentProcess().ProcessName=="OpenDental") {
				msg+="All copies of Open Dental ";
			}
			else {
				msg+=Process.GetCurrentProcess().ProcessName+" ";
			}
			msg+=Lan.g(this,"will shut down in 15 seconds.  Quickly click OK on any open windows with unsaved data.");
			MsgBoxCopyPaste msgbox = new MsgBoxCopyPaste(msg);
			msgbox.Size=new Size(300,300);
			msgbox.TopMost=true;
			msgbox.Show();
			BeginShutdownThread();
			return;
		}

		///<summary></summary>
		public void ProcessKillCommand() {
			//It is crucial that every form be forcefully closed so that they do not stay connected to a database that has been updated to a more recent version.
			CloseOpenForms(true);
			Application.Exit();//This will call FormOpenDental's closing event which will clean up all threads that are currently running.
		}

		///<summary></summary>
		public static void S_ProcessKillCommand() {
			_formOpenDentalSingleton.ProcessKillCommand();
		}

		private void SystemEvents_SessionSwitch(object sender,SessionSwitchEventArgs e) {
			if(e.Reason!=SessionSwitchReason.SessionLock) {
				return;
			}
			//CurUser will be null if Open Dental is already in a 'logged off' state.  Check Security.IsUserLoggedIn as well because Middle Tier does not 
			//set CurUser to null when logging off.
			//Also catches the case where Open Dental has NEVER connected to a database yet and checking PrefC would throw an exception (no db conn).
			if(Security.CurUser==null || !Security.IsUserLoggedIn) {
				return;
			}
			if(!PrefC.GetBool(PrefName.SecurityLogOffWithWindows)) {
				return;
			}
			LogOffNow(true);
		}

		private void FormOpenDental_Deactivate(object sender,EventArgs e) {
			//There is a chance that the user has gone to a non-modal form (e.g. task) and can change the patient from that form.
			//We need to save the Treatment Note in the chart module because the "on leave" event might not get fired for the text box.
			if(controlChart.TreatmentNoteChanged) {
				controlChart.UpdateTreatmentNote();
			}
			if(controlAccount.canUpdateUrgFinNote()) {
				controlAccount.UpdateUrgFinNote();
			}
			if(controlAccount.canUpdateFinNote()) {
				controlAccount.UpdateFinNote();
			}
			if(controlTreat.HasNoteChanged) {
				controlTreat.UpdateTPNoteIfNeeded();
			}
		}

		private void FormOpenDental_FormClosing(object sender,FormClosingEventArgs e) {
			if(e.CloseReason==CloseReason.UserClosing && Security.CurUser!=null && Security.IsUserLoggedIn) {//Checking if User clicked the 'X' button
				if(!AreYouSurePrompt(Security.CurUser.UserNum,Lan.g(this,"Are you sure you would like to close?"))) {
					e.Cancel=true;
					return;
				}
			}
			try {
				FormOpenDentalClosing(sender,e);
			}
			catch(Exception ex) {
				try {
					//Allow the program to close quietly, but send us at HQ a bug report so we can look into the problem.
					BugSubmissions.SubmitException(ex,patNumCur:PatNumCur);
				}
				catch(Exception exp) {
					exp.DoNothing();
				}
			}
		}

		private void FormOpenDentalClosing(object sender,FormClosingEventArgs e) {
			//ExitCode will only be set if trying to silently update.  
			//If we start using ExitCode for anything other than silently updating, this can be moved towards the bottom of this closing.
			//If moved to the bottom, all of the clean up code that this closing event does needs to be considered in regards to updating silently from a CEMT computer.
			if(ExitCode!=0) {
				Environment.Exit(ExitCode);
			}
			List<Form> listFormsToClose=GetOpenForms();
			bool hadMultipleFormsOpen=listFormsToClose.Count>0;
			//CloseOpenForms should have already been called with isForceClose=true if we are force closing Open Dental
			//In that scenario, calling CloseOpenForms with isForceClose=false should not leave the program open.
			//However, if Open Dental is closing from any other means, we want to give all forms the opportunity to stop closing.
			//Example, if you have FormWikiEdit open, it will attempt to save it as a draft unless the user wants to back out.
			if(!CloseOpenForms(false)) {
				e.Cancel=true;
				return;
			}
			if(hadMultipleFormsOpen) {
				//If this form is closing because someone called Application.Exit, then the call above to CloseOpenForms would cause an exception later in 
				//Application.Exit because CloseOpenForms altered a collection inside a foreach loop inside of Application.Exit. We still want to exit, but
				//we need to start afresh in order to not cause an exception.
				e.Cancel=true;
				this.BeginInvoke(() => Application.Exit());
				return;
			}
			//Put any meaningful code below this point. The hadMultipleFormsOpen check above can lead to this method being invoked twice.
			//The first invoke only exists to invoke the second invoke.
			try {
				Programs.ScrubExportedPatientData();//Required for EHR module d.7.
			}
			catch {
				//Can happen if cancel is clicked in Choose Database window.
			}
			try {
				Computers.ClearHeartBeat(ODEnvironment.MachineName);
			}
			catch { }
			try {
				List<ActiveInstance> listActiveInstances=ActiveInstances.GetAllOldInstances();
				if(ActiveInstances.CurrentActiveInstance!=null) {
					listActiveInstances.Add(ActiveInstances.CurrentActiveInstance);
				}
				ActiveInstances.DeleteMany(listActiveInstances);
			}
			catch { }
			FormUAppoint.AbortThread();
			ODThread.QuitSyncAllOdThreads();
			if(Security.CurUser!=null) {
				try {
					SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,"User: "+Security.CurUser.UserName+" has logged off.");
					Clinics.LogOff();
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			//Per https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.systemevents.sessionswitch?view=netframework-4.7.2 we need to unsubscribe 
			//from the SessionSwitch event "Because this is a static event, you must detach your event handlers when your application is disposed, or 
			//memory leaks will result."
			SystemEvents.SessionSwitch-=new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			//if(PrefC.GetBool(PrefName.DistributorKey)) {//for OD HQ
			//  for(int f=Application.OpenForms.Count-1;f>=0;f--) {
			//    if(Application.OpenForms[f]==this) {// main form
			//      continue;
			//    }
			//    Application.OpenForms[f].Close();
			//  }
			//}
			string tempPath="";
			string[] arrayFileNames;
			List <string> listDirectories;
			try {
				tempPath=PrefC.GetTempFolderPath();
				arrayFileNames=Directory.GetFiles(tempPath,"*.*",SearchOption.AllDirectories);//All files in the current directory plus all files in all subdirectories.
				listDirectories=new List<string>(Directory.GetDirectories(tempPath,"*",SearchOption.AllDirectories));//All subdirectories.
			}
			catch {
				//We will only reach here if we error out of getting the temp folder path
				//If we can't get the path, then none of the stuff below matters
				Plugins.HookAddCode(null,"FormOpenDental.FormClosing_end");
				return;
			}
			for(int i=0;i<arrayFileNames.Length;i++) {
				try {
					//All files related to updates need to stay.  They do not contain PHI information and will not harm anything if left around.
					if(arrayFileNames[i].Contains("UpdateFileCopier.exe")) {
						continue;//Skip any files related to updates.
					}
					//When an update is in progress, the binaries will be stored in a subfolder called UpdateFiles within the temp directory.
					if(arrayFileNames[i].Contains("UpdateFiles")) {
						continue;//Skip any files related to updates.
					}
					//The UpdateFileCopier will create temporary backups of source and destination setup files so that it can revert if copying fails.
					if(arrayFileNames[i].Contains("updatefilecopier")) {
						continue;//Skip any files related to updates.
					}
					File.Delete(arrayFileNames[i]);
				}
				catch {
					//Do nothing because the file could have been in use or there were not sufficient permissions.
					//This file will most likely get deleted next time a temp file is created.
				}
			}
			listDirectories.Sort();//We need to sort so that we know for certain which directories are parent directories of other directories.
			for(int i=listDirectories.Count-1;i>=0;i--) {//Easier than recursion.  Since the list is ordered ascending, then going backwards means we delete subdirectories before their parent directories.
				try {
					//When an update is in progress, the binaries will be stored in a subfolder called UpdateFiles within the temp directory.
					if(listDirectories[i].Contains("UpdateFiles")) {
						continue;//Skip any files related to updates.
					}
					//The UpdateFileCopier will create temporary backups of source and destination setup files so that it can revert if copying fails.
					if(listDirectories[i].Contains("updatefilecopier")) {
						continue;//Skip any files related to updates.
					}
					Directory.Delete(listDirectories[i]);
				}
				catch {
					//Do nothing because the folder could have been in use or there were not sufficient permissions.
					//This folder will most likely get deleted next time Open Dental closes.
				}
			}
			Plugins.HookAddCode(null,"FormOpenDental.FormClosing_end");
		}

		private void FormOpenDental_FormClosed(object sender,FormClosedEventArgs e) {
			//Cleanup all resources related to the program which have their Dispose methods properly defined.
			//This helps ensure that the chart module and its tooth chart wrapper are properly disposed of in particular.
			//This step is necessary so that graphics memory does not fill up.
			Dispose();
			//"=====================================================
			//https://msdn.microsoft.com/en-us/library/system.environment.exit%28v=vs.110%29.aspx
			//Environment.Exit Method:
			//Terminates this process and gives the underlying operating system the specified exit code.
			//For the exitCode parameter, use a non-zero number to indicate an error. In your application, you can define your own error codes in an
			//enumeration, and return the appropriate error code based on the scenario. For example, return a value of 1 to indicate that the required file
			//is not present and a value of 2 to indicate that the file is in the wrong format. For a list of exit codes used by the Windows operating
			//system, see System Error Codes in the Windows documentation.
			//Calling the Exit method differs from using your programming language's return statement in the following ways:
			//*Exit always terminates an application. Using the return statement may terminate an application only if it is used in the application entry
			//	point, such as in the Main method.
			//*Exit terminates an application immediately, even if other threads are running. If the return statement is called in the application entry
			//	point, it causes an application to terminate only after all foreground threads have terminated.
			//*Exit requires the caller to have permission to call unmanaged code. The return statement does not.
			//*If Exit is called from a try or finally block, the code in any catch block does not execute. If the return statement is used, the code in the
			//catch block does execute.
			//====================================================="
			//Call Environment.Exit() to kill all threads which we forgot to close.  Also sends exit code 0 to the command line to indicate success.
			//If a thread needs to be gracefully quit, then it is up to the designing engineer to Join() to that thread before we get to this point.
			//We considered trying to get a list of active threads and logging debug information for those threads, but there is no way
			//to get the list of managed threads from the system.  It is our responsibility to keep track of our own managed threads.  There is a way
			//to get the list of unmanaged system threads for our application using Process.GetCurrentProcess().Threads, but that does not help us enough.
			//See http://stackoverflow.com/questions/466799/how-can-i-enumerate-all-managed-threads-in-c.  To keep track of a managed thread, use ODThread.
			//Environment.Exit requires permission for unmanaged code, which we have explicitly specified in the solution already.
			Environment.Exit(0);//Guaranteed to kill any threads which are still running.
		}
		#endregion Closing
	}

	#region Classes
	public class PopupEvent:IComparable{
		public long PopupNum;
		///<summary>Disable this popup until this time.</summary>
		public DateTime DisableUntil;
		///<summary>The last time that this popup popped up.</summary>
		public DateTime LastViewed;

		public int CompareTo(object obj) {
			PopupEvent pop=(PopupEvent)obj;
			return DisableUntil.CompareTo(pop.DisableUntil);
		}

		public override string ToString() {
			return PopupNum.ToString()+", "+DisableUntil.ToString();
		}
	}

	///<summary>This is a global class because it must run at the application level in order to catch application level system input events. WM_KEYDOWN (0x0100) message details: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646280(v=vs.85).aspx.  WM_MOUSEMOVE (0x0200) message details: https://msdn.microsoft.com/en-us/library/windows/desktop/ms645616(v=vs.85).aspx. ///</summary>
	public class ODGlobalUserActiveHandler:IMessageFilter {
		///<summary>Compare position of mouse at the time of the message to the previously stored mouse position to correctly identify a mouse movement. In testing, a mouse will sometimes fire a series of multiple MouseMove events with the same position, possibly due to wireless mouse chatter.  Comparing to previous position allows us to only update the last activity timer when the mouse actually changes position.</summary>
		private Point _prevMousePos;

		///<summary>Returning false guarantees that the message will continue to the next filter control.  Therefore this method inspects the messages, but the messages are not consumed.</summary>
		public bool PreFilterMessage(ref Message m) {
			if(m.Msg==0x0100) {//Any keyboard input (WM_KEYDOWN=0x0100).
				Security.DateTimeLastActivity=DateTime.Now;
			}
			else if(m.Msg==0x0200 && _prevMousePos!=Cursor.Position) {//Mouse input (WM_MOUSEMOVE=0x0200) and position changed since last checked.
				_prevMousePos=Cursor.Position;
				Security.DateTimeLastActivity=DateTime.Now;
			}
			return false;//Always allow the message to continue to the next filter control
		}
	}

	public class CommandLineArgs {
		///<summary>Command line arguments passed in when program starts.</summary>
		public string[] ArrayCommandLineArgs;
		public string AptNum;
		public string ChartNumber;
		public string DatabaseName;
		///<summary>Not in manual</summary>
		public string DomainUser;
		public string UseDynamicMode;
		public string EcwConfigPath;
		public string JSESSIONID;
		public string JSESSIONIDSSO;
		///<summary>Not in manual</summary>
		public string LBSESSIONID;
		public string IsSilentUpdate;
		public string Module;
		public string MySqlUser;
		public string MySqlPassHash;
		public string MySqlPassword;
		public string OdPassword;
		public string PassHash;
		public string PatNum;
		public string ServerName;
		public string Show;
		public string SSN;
		public string UserId;
		public string UserName;
		public string WebServiceUri;
		public string WebServiceIsEcw;

		public CommandLineArgs(string[] arrayCommandLineArgs) {
			FillArguments(arrayCommandLineArgs);
		}

		///<summary>Fill method for getting the arguments from the command line for dynamic mode.</summary>
		public void FillArguments(string[] arrayCommandLineArgs) {
			ArrayCommandLineArgs=arrayCommandLineArgs;
			if(arrayCommandLineArgs==null || arrayCommandLineArgs.Length==0) {
				return;
			}
			AptNum=GetArgFromCommandLineArgs("AptNum=",arrayCommandLineArgs);
			ChartNumber=GetArgFromCommandLineArgs("ChartNumber=",arrayCommandLineArgs);
			DatabaseName=GetArgFromCommandLineArgs("DatabaseName=",arrayCommandLineArgs);
			DomainUser=GetArgFromCommandLineArgs("DomainUser=",arrayCommandLineArgs);
			string dynamicModeValue=GetArgFromCommandLineArgs("DynamicMode=",arrayCommandLineArgs);
			if(!dynamicModeValue.IsNullOrEmpty()) {
				UseDynamicMode=dynamicModeValue.ToLower();
			}
			EcwConfigPath=GetArgFromCommandLineArgs("EcwConfigPath=",arrayCommandLineArgs);
			JSESSIONID=GetArgFromCommandLineArgs("JSESSIONID=",arrayCommandLineArgs);
			JSESSIONIDSSO=GetArgFromCommandLineArgs("JSESSIONIDSSO=",arrayCommandLineArgs);
			LBSESSIONID=GetArgFromCommandLineArgs("LBSESSIONID=",arrayCommandLineArgs);
			string isSilentUpdateValue=GetArgFromCommandLineArgs("IsSilentUpdate=",arrayCommandLineArgs);
			if(!isSilentUpdateValue.IsNullOrEmpty()) {
				IsSilentUpdate=isSilentUpdateValue.ToLower();
			}
			string moduleValue=GetArgFromCommandLineArgs("module=",arrayCommandLineArgs);
			if(!moduleValue.IsNullOrEmpty()) {
				Module=moduleValue.ToLower();
			}
			MySqlUser=GetArgFromCommandLineArgs("MySqlUser=",arrayCommandLineArgs);
			MySqlPassHash=GetArgFromCommandLineArgs("MySqlPassHash=",arrayCommandLineArgs);
			MySqlPassword=GetArgFromCommandLineArgs("MySqlPassword=",arrayCommandLineArgs);
			OdPassword=GetArgFromCommandLineArgs("OdPassword=",arrayCommandLineArgs);
			PassHash=GetArgFromCommandLineArgs("PassHash=",arrayCommandLineArgs);
			PatNum=GetArgFromCommandLineArgs("PatNum=",arrayCommandLineArgs);
			ServerName=GetArgFromCommandLineArgs("ServerName=",arrayCommandLineArgs);
			string showValue=GetArgFromCommandLineArgs("show=",arrayCommandLineArgs);
			if(!showValue.IsNullOrEmpty()) {
				Show=showValue.ToLower();
			}
			SSN=GetArgFromCommandLineArgs("SSN=",arrayCommandLineArgs);
			UserId=GetArgFromCommandLineArgs("UserId=",arrayCommandLineArgs);
			UserName=GetArgFromCommandLineArgs("UserName=",arrayCommandLineArgs);
			WebServiceUri=GetArgFromCommandLineArgs("WebServiceUri=",arrayCommandLineArgs);
			string isECWValue=GetArgFromCommandLineArgs("WebServiceIsEcw=",arrayCommandLineArgs);
			if(!isECWValue.IsNullOrEmpty()) {
				WebServiceIsEcw=isECWValue.ToLower();
			}
		}

		///<summary>Returns the value without double quotes if the argument being searched for is found within the array of command line arguments. Returns null if not found.</summary>
		private string GetArgFromCommandLineArgs(string argSearch,string[] arrayCommandLineArgs) {
			argSearch=argSearch.ToLower();
			string match=arrayCommandLineArgs.FirstOrDefault(x => x.ToLower().StartsWith(argSearch) && x.Length>argSearch.Length);
			if(match==null) {
				return match;
			}
			return match.Substring(argSearch.Length).Trim('"');
		}

		///<summary>Returns a single string of all the retrieved command line arguments. Returns an empty string if no arguments are present.</summary>
		public string GetCommandLineArgs() {
			if(ArrayCommandLineArgs.Length==0) {
				return "";
			}
			string arguments="";
			if(AptNum!=null) {
				arguments+="AptNum=\""+AptNum+"\" ";
			}
			if(ChartNumber!=null) {
				arguments+="ChartNumber=\""+ChartNumber+"\" ";
			}
			if(EcwConfigPath!=null) {
				arguments+="EcwConfigPath=\""+EcwConfigPath+"\" ";
			}
			if(DatabaseName!=null) {
				arguments+="DatabaseName=\""+DatabaseName+"\" ";
			}
			if(DomainUser!=null) {
				arguments+="DomainUser=\""+DomainUser+"\" ";
			}
			if(UseDynamicMode!=null) {
				arguments+="DynamicMode=\""+UseDynamicMode.Contains("true")+"\" ";
			}
			if(JSESSIONID!=null) {
				arguments+="JSESSIONID=\""+JSESSIONID+"\" ";
			}
			if(JSESSIONIDSSO!=null) {
				arguments+="JSESSIONIDSSO=\""+JSESSIONIDSSO+"\" ";
			}
			if(LBSESSIONID!=null) {
				arguments+="LBSESSIONID=\""+LBSESSIONID+"\" ";
			}
			if(IsSilentUpdate!=null) {
				arguments+="IsSilentUpdate=\""+IsSilentUpdate+"\" ";
			}
			if(Module!=null) { 
				arguments+="module=\""+Module+"\" ";
			}
			if(MySqlUser!=null) {
				arguments+="MySqlUser=\""+MySqlUser+"\" ";
			}
			if(MySqlPassHash!=null) {
				arguments+="MySqlPassHash=\""+MySqlPassHash+"\" ";
			}
			if(MySqlPassword!=null) {
				arguments+="MySqlPassword=\""+MySqlPassword+"\" ";
			}
			if(OdPassword!=null) {
				arguments+="OdPassword=\""+OdPassword+"\" ";
			}
			if(PassHash!=null) {
				arguments+="PassHash=\""+PassHash+"\" ";
			}
			if(PatNum!=null) {
				arguments+="PatNum=\""+PatNum+"\" ";
			}
			if(ServerName!=null) {
				arguments+="ServerName=\""+ServerName+"\" ";
			}
			if(Show!=null) {
				arguments+="show=\""+Show+"\" ";
			}
			if(SSN!=null) {
				arguments+="SSN=\""+SSN+"\" ";
			}
			if(UserId!=null) {
				arguments+="UserId=\""+UserId+"\" ";
			}
			if(UserName!=null) {
				arguments+="UserName=\""+UserName+"\" ";
			}
			if(WebServiceUri!=null) {
				arguments+="WebServiceUri=\""+WebServiceUri+"\" ";
			}
			if(WebServiceIsEcw!=null) {
				arguments+="WebServiceIsEcw=\""+WebServiceIsEcw+"\" ";
			}
			return arguments;
		}
	}
	#endregion Classes
}
