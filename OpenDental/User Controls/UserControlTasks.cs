using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class UserControlTasks:UserControl {
		[Category("Action"),Description("Fires towards the end of the FillGrid method.")]
		public event FillGridEventHandler FillGridEvent;
		///<summary>List of all TastLists that are to be displayed in the main window. Combine with TasksList.</summary>
		private List<TaskList> _listTaskLists=new List<TaskList>();
		///<summary>List of all Tasks that are to be displayed in the main window.  Combine with TaskListsList.</summary>
		private List<Task> _listTasks=new List<Task>();
		//<Summary>Only used if viewing user tab.  This is a list of all task lists in the general tab.  It is used to generate full path heirarchy info for each task list in the user tab.</Summary>
		//private List<TaskList> TaskListsAllGeneral;
		///<summary>An arraylist of TaskLists beginning from the trunk and adding on branches.  If the count is 0, then we are in the trunk of one of the five categories.  The last TaskList in the TreeHistory is the one that is open in the main window.</summary>
		private List<TaskList> _listTaskListTreeHistory;
		///<summary>A TaskList that is on the 'clipboard' waiting to be pasted.  Will be null if nothing has been copied yet.</summary>
		private TaskList _taskListClip;
		///<summary>A Task that is on the 'clipboard' waiting to be pasted.  Will be null if nothing has been copied yet.</summary>
		private Task _taskClip;
		///<summary>If there is an item on our 'clipboard', this tracks whether it was cut.</summary>
		private bool _wasCut;
		///<summary>After closing, if this is not zero, then it will jump to the object specified in GotoKeyNum.</summary>
		public TaskObjectType TaskObjectTypeGoTo;
		///<summary>After closing, if this is not zero, then it will jump to the specified patient.</summary>
		public long KeyNumGoTo;
		///<summary>All notes for the showing tasks, ordered by date time.</summary>
		private List<TaskNote> _listTaskNotes=new List<TaskNote>();
		///<summary>A friendly string that could be used as the title of any window that has this control on it.
		///It will contain the description of the currently selected task list and a count of all new tasks within that list.</summary>
		public string TitleControlParent;
		private const int _TriageListNum=1697;
		private bool _isTaskSortApptDateTime;//Use task AptDateTime sort setup in FormTaskOptions.
		private bool _isShowFinishedTasks=false;//Show finished task setup in FormTaskOptions.
		private bool _isShowArchivedTaskLists;//Show archived task lists in FormTaskOptions.
		private DateTime _dateTimeStartShowFinished=DateTime.Today.AddDays(-7);//Show finished task date setup in FormTaskOptions.
		///<summary>Keeps track of which tasks are expanded.  Persists between TaskList list updates.</summary>
		private List<long> _listTaskNumsExpanded=new List<long>();
		private bool _isCollapsedByDefault;
		private bool _hasListSwitched;
		///<summary>This can be three states: 0 for all tasks expanded, 1 for all tasks collapsed, and -1 for mixed.</summary>
		private int _taskCollapsedState;
		///<summary>The states of patients from previously seen tasks.</summary>
		private Dictionary<long,string> _dictPatStates=new Dictionary<long, string>();
		///<summary>Signalnums for Task or TaskList signals sent from this machine, that have not yet been received back from 
		///FormOpenDental.OnProcessSignals(). Do not include InvalidType.TaskPopup.</summary>
		private List<long> _listSignalNumsSentTask=new List<long>();
		///<summary>All instances of this control that are currently open.</summary>
		private static List <UserControlTasks> _listUserControlTaskss=new List<UserControlTasks>();
		///<summary>TaskListNums for TaskLists the current user is subscribed to.
		///Is static so can be referenced from multiple instances of this control.  Locked each time it is accessed so it is thread safe.</summary>
		private static List<long> _listTaskListNumsSubscribed=new List<long>();
		///<summary>A list of job links used for displaying whether a task has an attached job.</summary>
		private static List<JobLink> _listJobLinks=new List<JobLink>();
		///<summary>A list of jobs that are used for determining whether a task has an attached job in the fill grid.</summary>
		private static List<Job> _listJobs=new List<Job>();
		///<summary>The action which occurs when the Toggle Chat button is clicked.  Only set for OD HQ triage.</summary>
		private Action _actionChatToggle=null;
		///<summary>Defines which filter type is the default for the current tasklist for filtering the Task grid.</summary>
		private EnumTaskFilterType _enumTaskFilterTypeForList;
		///<summary>This is the patient used to filter tasks from.</summary>
		private Patient _patientFilter;
		///<summary>This is the start date to filter tasks from.</summary>
		private DateTime _dateFilterStart;
		///<summary>This is the end date to filter tasks from</summary>
		private DateTime _dateFilterEnd;
		///<summary>The current clinic selected for filtering</summary>
		private List<long> _listClinicNumsFilter=new List<long>();
		///<summary>The current region selected for filtering</summary>
		private List<long> _listDefNumsRegionFilter=new List<long>();
		private EnumTaskPatientFilterType _enumTaskPatientFilterType;
		///<summary>Defined here so we can change the text on this button depending on filer setting.</summary>
		private ODToolBarButton _odToolBarButtonFilter;
		///<summary>Dictionary to look up task list by primary key. Key: TaskListNum. Value: TaskList.</summary>
		private Dictionary<long,TaskList> _dictTaskLists;
		///<summary>A list of all task attachments for tasks in the selected task list.</summary>
		private List<TaskAttachment> _listTaskAttachments=new List<TaskAttachment>();
		///<summary></summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Makes an additional reference pointer to _listTaskLists when at the trunk of the Main or Reminder tab and manual refresh is enabled.</summary>
		private List<TaskList> _listTaskListsCopy=new List<TaskList>();
		///<summary>Makes an additional reference pointer to _listTasks when at the trunk of the Main or Reminder tab and manual refresh is enabled.</summary>
		private List<Task> _listTasksCopy=new List<Task>();
		///<summary></summary>
		private string _tabNamePrevious;
		/// <summary>used for searching tasknum via the clipboard</summary>
		private long _taskNumOld=-1;
		///<summary>Only used from UI.</summary>
		private List<TaskList> _listTaskListsHistory;

		///<summary>Creates a thread safe copy of _listSubscribedTaskListNums.</summary>
		private static List<long> ListTaskListNumsSubscribed {
			get {
				List <long> listTaskListNums=null;
				lock(_listTaskListNumsSubscribed) {
					listTaskListNums=new List<long>(_listTaskListNumsSubscribed);
				}
				return listTaskListNums;
			}
		}

		public UserControlTasks() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			//this.listMain.ContextMenu = this.menuEdit;
			//Lan.F(this);
			Lan.C(this,menuEdit);
			gridMain.ContextMenu=menuEdit;
			_listUserControlTaskss.Add(this);
		}

		///<summary>Destructor.  Removes this instance from the private list of instances.</summary>
		~UserControlTasks() {
			_listUserControlTaskss.Remove(this);
		}

		private void UserControlTasks_Resize(object sender,EventArgs e) {
			//ToolBarMain is docked, h=25
			//Filter area always show
			LayoutManager.Move(tabControl,new Rectangle(0,LayoutManager.Scale(49),Width,LayoutManager.Scale(20)));
			LayoutManager.Move(monthCalendar,new Rectangle(0,LayoutManager.Scale(70),LayoutManager.Scale(227),LayoutManager.Scale(162)));
			LayoutManager.MoveWidth(tree,Width);
			LayoutManager.MoveWidth(gridMain,Width);
			FillGrid(new List<Signalod>());//Sets height.  Does not run query.
			LayoutTreeAndGrid();
		}

		///<summary>Calls RefreshTasks for all known instances of UserControlTasks for each instance which is visible and not disposed.</summary>
		public static void RefreshTasksForAllInstances(List<Signalod> listSignalods,UserControlTasksTab userControlTasksTabRefresh=UserControlTasksTab.Invalid) {
			foreach(UserControlTasks control in _listUserControlTaskss) {
				if(!control.Visible || control.IsDisposed) {
					continue;
				}
				if(userControlTasksTabRefresh!=UserControlTasksTab.Invalid && control.TaskTab!=userControlTasksTabRefresh) {
					continue;
				}
				Logger.LogAction("UserControlTasks.RefreshTasks",LogPath.Signals,() => control.FillGrid(listSignalods));
			}
		}

		///<summary>Resets the currently applied filter to either the preference TasksGlobalFilterType, or the selected TaskList's override, for all 
		///instances of UserControlTasks.</summary>
		public static void ResetGlobalTaskFilterTypesToDefaultAllInstances() {
			foreach(UserControlTasks control in _listUserControlTaskss) {
				if(!control.Visible || control.IsDisposed) {
					continue;
				}
				control.SetFiltersToDefault();
			}
		}

		///<summary>And resets the tabs if the user changes.</summary>
		public void InitializeOnStartup(){
			if(Security.CurUser==null) {
				return;
			}
			tabUser.Text=Lan.g(this,"For ")+Security.CurUser.UserName;
			tabNew.Text=Lan.g(this,"New for ")+Security.CurUser.UserName;
			if(PrefC.GetBool(PrefName.TasksShowOpenTickets)) {
				if(!tabControl.TabPages.Contains(tabOpenTickets)) {
					LayoutManager.AddAt(tabOpenTickets,tabControl,2);
				}
			}
			else{
				if(tabControl.TabPages.Contains(tabOpenTickets)) {
					tabControl.TabPages.Remove(tabOpenTickets);
				}
			}
			LayoutToolBar();
			if(PrefC.GetBool(PrefName.TasksUseRepeating)) {
				if(!tabControl.TabPages.Contains(tabRepeating)) {
					LayoutManager.Add(tabRepeating,tabControl);
					LayoutManager.Add(tabDate,tabControl);
					LayoutManager.Add(tabWeek,tabControl);
					LayoutManager.Add(tabMonth,tabControl);
				}
				if(tabControl.TabPages.Contains(tabReminders)) {
					tabControl.TabPages.Remove(tabReminders);
				}
			}
			else {//Repeating tasks disabled.
				if(tabControl.TabPages.Contains(tabRepeating)) {
					tabControl.TabPages.Remove(tabRepeating);
					tabControl.TabPages.Remove(tabDate);
					tabControl.TabPages.Remove(tabWeek);
					tabControl.TabPages.Remove(tabMonth);
				}
				if(!tabControl.TabPages.Contains(tabReminders)) {
					LayoutManager.Add(tabReminders,tabControl);
				}
			}
			if(_listTaskListsHistory==null) {//first time opening
				_listTaskListTreeHistory=new List<TaskList>();
				monthCalendar.SelectionStart=DateTime.Today;
			}
			else {//reopening
				if(Tasks.LastOpenGroup >= tabControl.TabPages.Count) {
					//This happens if the user changes the TasksUseRepeating from true to false, then refreshes the tasks.
					Tasks.LastOpenGroup=0;
				}
				tabControl.SelectedIndex=Tasks.LastOpenGroup;
				_listTaskListTreeHistory=new List<TaskList>();
				//for(int i=0;i<Tasks.LastOpenList.Count;i++) {
				//	TreeHistory.Add(((TaskList)Tasks.LastOpenList[i]).Copy());
				//}
				monthCalendar.SelectionStart=Tasks.dateLastOpen;
			}
			if(PrefC.GetBool(PrefName.EnterpriseManualRefreshMainTaskLists)) {
				butRefresh.Visible=true;
			}
			else {
				butRefresh.Visible=false;
			}
			if(PrefC.IsODHQ) {
				menuNavJob.Visible=true;
				menuNavJob.Enabled=false;
				if(Security.IsAuthorized(EnumPermType.TaskDelete,true)) {
					menuDeleteTaken.Visible=true;
				}
			}
			menuNavAttachment.Enabled=false;
			_isTaskSortApptDateTime=PrefC.GetBool(PrefName.TaskSortApptDateTime);//This sets it for use and also for the task options default value.
			List<UserOdPref> listUserOdPrefsForCollapsing=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse);
			_isCollapsedByDefault=listUserOdPrefsForCollapsing.Count==0 ? false : PIn.Bool(listUserOdPrefsForCollapsing[0].ValueString);
			_hasListSwitched=true;
			_taskCollapsedState=_isCollapsedByDefault ? 1 : 0;
			SetFiltersToDefault();//Fills Tree and Grid
			if(tabControl.SelectedTab!=tabOpenTickets) {//because it will have alread been set
				SetOpenTicketTab(-1);
			}
			SetPatientTicketTab(-1);
			SetMenusEnabled();
		}

		public UserControlTasksTab TaskTab {
			get {
				if(tabControl.SelectedTab==tabUser) {
					return UserControlTasksTab.ForUser;
				}
				else if(tabControl.SelectedTab==tabNew) {
					return UserControlTasksTab.UserNew;
				}
				else if(tabControl.SelectedTab==tabOpenTickets) {
					return UserControlTasksTab.OpenTickets;
				}
				else if(tabControl.SelectedTab==tabPatientTickets) {
					return UserControlTasksTab.PatientTickets;
				}
				else if(tabControl.SelectedTab==tabMain) {
					return UserControlTasksTab.Main;
				}
				else if(tabControl.SelectedTab==tabReminders) {
					return UserControlTasksTab.Reminders;
				}
				else if(tabControl.SelectedTab==tabRepeating) {
					return UserControlTasksTab.RepeatingSetup;
				}
				else if(tabControl.SelectedTab==tabDate) {
					return UserControlTasksTab.RepeatingByDate;
				}
				else if(tabControl.SelectedTab==tabWeek) {
					return UserControlTasksTab.RepeatingByWeek;
				}
				else if(tabControl.SelectedTab==tabMonth) {
					return UserControlTasksTab.RepeatingByMonth;
				}
				return UserControlTasksTab.Invalid;//Default.  Should not happen.
			}
			set {
				UI.TabPage tabPageOld=tabControl.SelectedTab;
				if(value==UserControlTasksTab.ForUser) {
					tabControl.SelectedTab=tabUser;
				}
				else if(value==UserControlTasksTab.UserNew) {
					tabControl.SelectedTab=tabNew;
				}
				else if(value==UserControlTasksTab.OpenTickets && PrefC.GetBool(PrefName.TasksShowOpenTickets)) {
					tabControl.SelectedTab=tabOpenTickets;
				}
				else if(value==UserControlTasksTab.Main) {
					tabControl.SelectedTab=tabMain;
				}
				else if(value==UserControlTasksTab.Reminders) {
					tabControl.SelectedTab=tabReminders;
				}
				else if(value==UserControlTasksTab.RepeatingSetup && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabControl.SelectedTab=tabRepeating;
				}
				else if(value==UserControlTasksTab.RepeatingByDate && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabControl.SelectedTab=tabDate;
				}
				else if(value==UserControlTasksTab.RepeatingByWeek && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabControl.SelectedTab=tabWeek;
				}
				else if(value==UserControlTasksTab.RepeatingByMonth && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabControl.SelectedTab=tabMonth;
				}
				else if(value==UserControlTasksTab.PatientTickets) {
					tabControl.SelectedTab=tabPatientTickets;
				}
				else if(value==UserControlTasksTab.Invalid) {
					//Do nothing.
				}
				if(tabControl.SelectedTab!=tabPageOld) {//Tab changed, refresh the tree.
					_listTaskListTreeHistory=new List<TaskList>();//clear the tree no matter which tab selected.
					FillTree();
					FillGrid();
				}
			}
		}

		///<summary>Called whenever OpenTicket tab is refreshed to set the count at the top.  Also called from InitializeOnStartup.  In that case, we don't know what the count should be, so we pass in a -1.</summary>
		private void SetOpenTicketTab(int countSet) {
			if(!tabControl.TabPages.Contains(tabOpenTickets)) {
				return;
			}
			if(countSet==-1) {
				countSet=Tasks.GetCountOpenTickets(Security.CurUser.UserNum);
			}
			tabOpenTickets.Text=Lan.g(this,"Open Tasks")+" ("+countSet.ToString()+")";
		}

		///<summary>Called whenever PatientTickets tab is refreshed to set the count at the top.  Also called from InitializeOnStartup.  In that case, we don't know what the count should be, so we pass in a -1.</summary>
		private void SetPatientTicketTab(int countSet) {
			if(!tabControl.TabPages.Contains(tabPatientTickets)) {
				return;
			}
			if(countSet==-1 && FormOpenDental.PatNumCur!=0) {
				countSet=Tasks.GetCountPatientTickets(FormOpenDental.PatNumCur);
			}
			tabPatientTickets.Text=Lan.g(this,"Patient Tasks")+" ("+(countSet==-1?"0":countSet.ToString())+")";
		}

		public void ClearLogOff() {
			tabUser.Text="for";
			tabNew.Text="New for";
			_listTaskListTreeHistory=new List<TaskList>();
			FillTree();
			gridMain.SetAll(false);
			gridMain.ListGridRows.Clear();
			gridMain.Invalidate();
		}

		///<summary>Used by OD HQ.</summary>
		public void FillGridWithTriageList() {
			TaskList taskList=TaskLists.GetOne(_TriageListNum);
			tabControl.SelectedTab=tabMain;
			if(_listTaskListTreeHistory==null) {
				_listTaskListTreeHistory=new List<TaskList>();
			}
			_listTaskListTreeHistory.Clear();
			_listTaskListTreeHistory.Add(taskList);
			if(_listTaskLists==null) {
				_listTaskLists=new List<TaskList>();
			}
			_listTaskLists.Clear();
			if(_listTasks==null) {
				_listTasks=new List<Task>();
			}
			_listTasks.Clear();
			_listTasks=Tasks.RefreshChildren(_TriageListNum,false,monthCalendar.SelectionStart,Security.CurUser.UserNum,0,TaskType.All,_listClinicNumsFilter,_listDefNumsRegionFilter);
			FillTree();
			FillGrid();
			gridMain.Focus();//Allow immediate mouse wheel scroll when loading triage list, no click required
			LayoutToolBar();//To show the triage specific buttons.
		}

		private void UserControlTasks_Load(object sender,System.EventArgs e) {
			if(this.DesignMode){
				return;
			}
			if(!PrefC.GetBool(PrefName.TaskAncestorsAllSetInVersion55)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"A one-time routine needs to be run.  It will take a few minutes.  Do you have time right now?")){
					return;
				}
				Cursor=Cursors.WaitCursor;
				TaskAncestors.SynchAll();
				Prefs.UpdateBool(PrefName.TaskAncestorsAllSetInVersion55,true);
				DataValid.SetInvalid(InvalidType.Prefs);
				Cursor=Cursors.Default;
			}
			ODEvent.Fired+=PatientChangedEvent_Fired;
		}

		/// <summary> Refreshes patient task list when we switch patients</summary>
		private void PatientChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.Patient){
				return;
			}
			RefreshPatTicketsIfNeeded();
		}

		///<summary></summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ODToolBarButton odToolBarButtonOptions=new ODToolBarButton();
			odToolBarButtonOptions.Text=Lan.g(this,"Options");
			odToolBarButtonOptions.ToolTipText=Lan.g(this,"Set session specific task options.");
			odToolBarButtonOptions.Tag="Options";
			ToolBarMain.Buttons.Add(odToolBarButtonOptions);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add Task List"),0,"","AddList"));
			ODToolBarButton odToolBarButtonTask=new ODToolBarButton(Lan.g(this,"Add Task"),1,"","AddTask");
			odToolBarButtonTask.Style=ODToolBarButtonStyle.DropDownButton;
			odToolBarButtonTask.DropDownMenu=menuTask;
			ToolBarMain.Buttons.Add(odToolBarButtonTask);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Search"),-1,"","Search"));
			ODToolBarButton odToolBarButton=new ODToolBarButton();
			odToolBarButton.Text=Lan.g(this,"Manage Blocks");
			odToolBarButton.ToolTipText=Lan.g(this,"Manage which task lists will have popups blocked even when subscribed.");
			odToolBarButton.Tag="BlockSubsc";
			odToolBarButton.IsTogglePushed=Security.CurUser.DefaultHidePopups;
			ToolBarMain.Buttons.Add(odToolBarButton);
			//Filtering only works if Clinics are enabled and preference turned on.
			if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)!=EnumTaskFilterType.Disabled) {
				//string textBut=string.Empty;
				//if(_enumTaskFilterType==EnumTaskFilterType.None) {
				//	textBut="Unfiltered";
				//}
				//else {
				//	textBut="Filtered by "+_enumTaskFilterType.GetDescription();
				//}
				_odToolBarButtonFilter=new ODToolBarButton("Task Filter",-1,"Select filter.","Filter");
				ToolBarMain.Buttons.Add(_odToolBarButtonFilter);
			}
			ToolBarMain.Invalidate();
		}

		private void FillTree() {
			tree.Nodes.Clear();
			TreeNode treeNode;
			//TreeNode lastNode=null;
			string descNode;
			for(int i=0;i<_listTaskListTreeHistory.Count;i++) {
				descNode=_listTaskListTreeHistory[i].Descript;
				if(tabControl.SelectedTab==tabUser) {
					descNode=_listTaskListTreeHistory[i].ParentDesc+descNode;
				}
				treeNode=new TreeNode(descNode);
				treeNode.Tag=_listTaskListTreeHistory[i].TaskListNum;
				if(tree.SelectedNode==null) {
					tree.Nodes.Add(treeNode);
				}
				else {
					tree.SelectedNode.Nodes.Add(treeNode);
				}
				tree.SelectedNode=treeNode;
			}
			if(tree.SelectedNode!=null) {
				bool isHQClinic=true;
				bool isHQRegion=true;
				if(_listClinicNumsFilter.Count!=1 || !_listClinicNumsFilter.Contains(0)) {
					isHQClinic=false;
				}
				if(_listDefNumsRegionFilter.Count!=1 || !_listDefNumsRegionFilter.Contains(0)) {
					isHQRegion=false;
				}
				string filterText="";
				if(_listClinicNumsFilter.Count>0 && !isHQClinic) {
					filterText+=""+string.Join(",",_listClinicNumsFilter.Select(x => Clinics.GetAbbr(x)))+"";
				}
				else if(_listDefNumsRegionFilter.Count>0 && !isHQRegion) {
					filterText+=""+string.Join(",",_listDefNumsRegionFilter.Select(x => Defs.GetName(DefCat.Regions,x)))+"";
				}
				if(_dateFilterStart==DateTime.Today && _dateFilterEnd==DateTime.Today) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+="Today";
				}
				else if(_dateFilterStart==DateTime.Today.AddDays(1) && _dateFilterEnd==DateTime.Today.AddDays(1)) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+="Tomorrow";
				}
				else if(_dateFilterStart!=DateTime.MinValue && _dateFilterEnd!=DateTime.MinValue) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+=_dateFilterStart.ToShortDateString()+"-"+_dateFilterEnd.ToShortDateString();
				}
				else if(_dateFilterStart!=DateTime.MinValue && _dateFilterEnd==DateTime.MinValue) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+="Starting "+_dateFilterStart.ToShortDateString();
				}
				else if(_dateFilterEnd!=DateTime.MinValue && _dateFilterStart==DateTime.MinValue) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+="Through "+_dateFilterEnd.ToShortDateString();
				}
				if(_patientFilter!=null) {
					if(filterText!="") {
						filterText+=", ";
					}
					filterText+=_patientFilter.GetNameLF();
				}
				if(filterText!="") {
					tree.SelectedNode.Text+=" ("+filterText+")";
				}
			}
			//remember this position for the next time we open tasks
			_listTaskListsHistory=new List<TaskList>();
			for(int i=0;i<_listTaskListTreeHistory.Count;i++) {
				_listTaskListsHistory.Add(_listTaskListTreeHistory[i].Copy());
			}
			Tasks.LastOpenGroup=tabControl.SelectedIndex;
			Tasks.dateLastOpen=monthCalendar.SelectionStart;
			//layout
			LayoutTreeAndGrid();
		}

		private void LayoutTreeAndGrid() {
			if(_listTaskListTreeHistory==null) {
				return;
			}
			if(tabControl.SelectedTab==tabDate || tabControl.SelectedTab==tabWeek || tabControl.SelectedTab==tabMonth) {
				LayoutManager.MoveLocation(tree,new Point(tree.Left,monthCalendar.Bottom+1));//Show the calendar.
			}
			else {
				LayoutManager.MoveLocation(tree,new Point(tree.Left,tabControl.Bottom));//Hide the calendar.
			}
			LayoutManager.MoveHeight(tree,_listTaskListTreeHistory.Count*tree.ItemHeight+8);
			tree.Refresh();
			LayoutManager.MoveLocation(gridMain,new Point(0,tree.Bottom));
		}

		public void RefreshPatTicketsIfNeeded() {
			if(TaskTab==UserControlTasksTab.PatientTickets) {
				FillGrid();
			}
			else {
				SetPatientTicketTab(-1);
			}
		}

		///<summary>Sets the classwide filter variables to the current tasklist's default values.</summary>
		private void SetFiltersToDefault(TaskList taskList=null) {
			_enumTaskFilterTypeForList=EnumTaskFilterType.Default;
			_patientFilter=null;
			_enumTaskPatientFilterType=EnumTaskPatientFilterType.AllPatients;
			_dateFilterStart=DateTime.MinValue;
			_dateFilterEnd=DateTime.MinValue;
			if(taskList==null && tree.SelectedNode!=null) {//Check if there is a current tasklist selected.
				taskList=_listTaskListTreeHistory.FirstOrDefault(x => x.TaskListNum==(long)tree.SelectedNode.Tag);
			}
			if(taskList!=null) {//And if so, use its GlobalTaskFilterType override.
				_enumTaskFilterTypeForList=taskList.GlobalTaskFilterType;
			}
			if(Clinics.ClinicNum==0) {//HQ clinic, do not apply filtering.
				_enumTaskFilterTypeForList=EnumTaskFilterType.None;
			}
			if(_enumTaskFilterTypeForList==EnumTaskFilterType.Default) {//Get the global default filter setting.
				_enumTaskFilterTypeForList=(EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType);
			}
			//At this point, we have the GlobalFilterType to use.  Make sure it's a valid choice.
			_enumTaskFilterTypeForList=DowngradeFilterTypeIfNeeded(_enumTaskFilterTypeForList);
			switch(_enumTaskFilterTypeForList) {
				case EnumTaskFilterType.None:
					_listClinicNumsFilter=new List<long> { 0 };
					_listDefNumsRegionFilter=new List<long> { 0 };
					break;
				case EnumTaskFilterType.Clinic:
					_listClinicNumsFilter=new List<long> { Clinics.ClinicNum };//Default to currently selected clinic.
					_listDefNumsRegionFilter=new List<long> { 0 };
					break;
				case EnumTaskFilterType.Region:
					//Default to currently selected clinic's region.  Use 0 if no region defined.
					_listDefNumsRegionFilter=new List<long> { Clinics.GetClinic(Clinics.ClinicNum)?.Region??0 };
					_listClinicNumsFilter=new List<long> { 0 };
					break;
			}
			FillTree();
			FillGrid();
		}

		///<summary>Determines if globalFilterType should be downgraded based on Clinics being enabled/disabled and Region definitions.</summary>
		private EnumTaskFilterType DowngradeFilterTypeIfNeeded(EnumTaskFilterType enumTaskFilterType) {
			if(!PrefC.HasClinicsEnabled) {//Downgrade to None if Clinics are disabled.
				enumTaskFilterType=EnumTaskFilterType.None;
			}
			else if(enumTaskFilterType==EnumTaskFilterType.Region && Defs.GetDefsForCategory(DefCat.Regions).Count==0) {
				enumTaskFilterType=EnumTaskFilterType.None;//Downgrade to None if Region selected but no Regions defined.
			}
			return enumTaskFilterType;
		}

		//private void SetFilters(EnumTaskFilterType enumTaskFilterType,List<long> listFilterFkeys) {
		//	bool isChangingFilterType=(_enumTaskFilterType!=enumTaskFilterType);
		//	_enumTaskFilterType=enumTaskFilterType;
			//_listFKeysFilter=listFilterFkeys;
			//if(isChangingFilterType && _odToolBarButtonFilter!=null) {
			//	if(_enumTaskFilterType==EnumTaskFilterType.None) {
			//		_odToolBarButtonFilter.Text="Unfiltered";
			//	}
				//else {
				//	_odToolBarButtonFilter.Text="Filtered by "+_enumTaskFilterType.GetDescription();
				//}
				//ToolBarMain.Invalidate();//Redraw immediately.
			//}
		//	FillTree();
		//	FillGrid();
		//}

		///<summary>Causes all instances of UserControlTasks to replace/remove the passed in Task and TaskNotes from the list of currently displayed 
		///Tasks, then, if necessary, refills the grid without querying the database for the Task or TaskNotes. Adds signalNums for signals associated
		///to the refreshes occurring in this method to the list of signals that have been sent, so FillGrid can ignore them if the refresh has already
		///occurred locally.
		///To remove task from grid in all instances, pass in canKeepTask=true.</summary>
		public static void RefillLocalTaskGrids(Task task,List<TaskNote> listTaskNotes,List<long> listSentSignalNums,bool canKeepTask=true) {
			DataValid.SetInvalid(InvalidType.Task);//Fires plugin hook, refreshes Chart module if visible.
			UserControlTasks.AddSentSignalNums(listSentSignalNums);
			foreach(UserControlTasks control in _listUserControlTaskss) {
				if(!control.Visible && !control.IsDisposed) {//Verify control is visible and active
					continue;
				}
				long taskListNum=0;//Default to one of the main trunks.
				if(control._listTaskListTreeHistory!=null && control._listTaskListTreeHistory.Count>0) {//not on main trunk
					taskListNum=control._listTaskListTreeHistory[control._listTaskListTreeHistory.Count-1].TaskListNum;
				}
				if(task==null) {
					//Just FillGrid.
				}
				else if(task.TaskStatus==TaskStatusEnum.Done 
					&& (!control._isShowFinishedTasks || (control._isShowFinishedTasks && control.tabControl.SelectedTab==control.tabNew))) {
					//Task is Done, and option to Show Finished Tasks is off, or Done and Show Finished Tasks is on and on New for User tab.
					control._listTasks.RemoveAll(x => x.TaskNum==task.TaskNum);
					control._listTaskNotes.RemoveAll(x => x.TaskNum==task.TaskNum);//Remove corresponding taskNotes.
				}
				else if(canKeepTask && (task.TaskListNum==taskListNum//Task is in the currently displayed TaskList.
					|| (control.tabControl.SelectedTab==control.tabNew && control.IsInNewTab(task))//Task should display in 'New for User' tab.
					|| (control.tabControl.SelectedTab==control.tabOpenTickets && task.UserNum==Security.CurUser.UserNum 
						&& task.ObjectType==TaskObjectType.Patient)//Open Tab
					|| (control.tabControl.SelectedTab==control.tabPatientTickets && task.KeyNum==FormOpenDental.PatNumCur)))//Patient tab
				{
					if(control._listTasks.Count==0) {
						control._listTasks.Add(task);//Task newly moved to this TaskList.
					}
					else {
						for(int i=0;i<control._listTasks.Count;i++) {
							if(control._listTasks[i].TaskNum==task.TaskNum) {
								control._listTasks[i]=task;//Replace Task in list with new task.
								break;
							}
							if(i==control._listTasks.Count-1) {//Looped through all current Tasks and didn't find this Task, so it must be new to the TaskList.
								control._listTasks.Add(task);//Task newly moved to this TaskList.
								break;
							}
						}
					}
					control._listTaskNotes.RemoveAll(x => x.TaskNum==task.TaskNum);//Remove corresponding taskNotes.
					control._listTaskNotes.AddRange(listTaskNotes);//Add the refreshed TaskNotes back.
				}
				else {//Task is not in current TaskList, or was deleted(canKeepTask==false)
					control._listTasks.RemoveAll(x => x.TaskNum==task.TaskNum);
					control._listTaskNotes.RemoveAll(x => x.TaskNum==task.TaskNum);//Remove corresponding taskNotes.
				}
				control.FullRefreshIfNeeded(taskListNum);
			}
		}

		///<summary>Causes all instances of UserControlTasks to replace/remove the passed in TaskList from the list of currently displayed 
		///TaskLists, then, if necessary, refills the grid without querying the database for the TaskList. Adds signalNums for signals associated
		///to the refreshes occurring in this method to the list of signals that have been sent, so FillGrid can ignore them if the refresh has already
		///occurred locally.
		///To remove taskList from grid in all instances, pass in canKeepTask=true.</summary>
		public static void RefillLocalTaskGrids(TaskList taskList,List<long> listSentSignalNums,bool canKeepTaskList=true) {
			AddSentSignalNums(listSentSignalNums);
			List <long> listTaskListNumsSubscribed=ListTaskListNumsSubscribed;//Get list copy above loop to avoid making unnecessary copies.
			foreach(UserControlTasks control in _listUserControlTaskss) {
				long taskListNum=0;//Default to one of the main trunks.
				if(control._listTaskListTreeHistory!=null && control._listTaskListTreeHistory.Count>0) {//not on main trunk
					taskListNum=control._listTaskListTreeHistory[control._listTaskListTreeHistory.Count-1].TaskListNum;
				}
				if(taskList!=null) {
					foreach(TaskList list in control._listTaskLists) {
						if(list.TaskListNum==taskList.TaskListNum) {
							list.TaskListStatus=taskList.TaskListStatus;
						}
					}
					if(control._dictTaskLists!=null && control._dictTaskLists.ContainsKey(taskList.TaskListNum)) {
						control._dictTaskLists[taskList.TaskListNum].TaskListStatus=taskList.TaskListStatus;
					}
				}
				if(taskList==null) {
					//Just FillGrid
				}
				else if(!control._isShowArchivedTaskLists && taskList.TaskListStatus==TaskListStatusEnum.Archived) {
					if(control._dictTaskLists!=null) {
						Dictionary<long,TaskList> dictTaskLists=control._dictTaskLists;
						control._listTaskLists.RemoveAll(x => x.TaskListStatus==TaskListStatusEnum.Archived ||
							TaskLists.IsAncestorTaskListArchived(ref dictTaskLists,x));
						control._dictTaskLists=dictTaskLists;
					}
				}
				//On 'ForUser' tab and not subscribed to this list.
				else if(control.tabControl.SelectedTab==control.tabUser && !listTaskListNumsSubscribed.Contains(taskList.TaskListNum)) {
					control._listTaskLists.RemoveAll(x => x.TaskListNum==taskList.TaskListNum);
				}
				else if(canKeepTaskList 
					//not on 'New for User' and taskList is in the currently displayed TaskList ('New for User' only shows Tasks)
					&& ((control.tabControl.SelectedTab!=control.tabNew && taskList.Parent==taskListNum)
						//On 'for User' tab and taskList is a subscribed TaskList
						|| (control.tabControl.SelectedTab==control.tabUser && listTaskListNumsSubscribed.Contains(taskList.TaskListNum)))) 
				{
					int insertIndex=0;
					if(control._listTaskLists.Count==0) {
						control._listTaskLists.Insert(insertIndex,taskList);//TaskList newly moved to this TaskList.
					}
					else {
						for(int i=0;i<control._listTaskLists.Count;i++) {
							if(control._listTaskLists[i].TaskListNum==taskList.TaskListNum) {
								control._listTaskLists[i]=taskList;//Replace TaskList in list with new taskList.
								break;
							}
							if(taskList.Descript.CompareTo(control._listTaskLists[i].Descript)>=0) {//Does taskList come after this list?
								insertIndex=i+1;//Insert here to maintain order.
							}
							if(i==control._listTaskLists.Count-1) {//Looped through all current TaskLists and didn't find this TaskList, so it must be new to the TaskList.
								control._listTaskLists.Insert(insertIndex,taskList);//TaskList newly moved to this TaskList.
								break;
							}
						}
					}
				}
				else {//TaskList is not in current TaskList,or was deleted(canKeepTaskList==false)
					control._listTaskLists.RemoveAll(x => x.TaskListNum==taskList.TaskListNum);
				}
				control.FullRefreshIfNeeded(taskListNum);
			}
		}

		private void FullRefreshIfNeeded(long parent) {
			if(parent!=0 || tabControl.SelectedTab==tabNew) {//Not a trunk, or on the New for User tab. 
				//These scenarios have additional sorting after the query when executing a full refresh.
				FillGrid();//For now, do a full refresh if we are drilled into a TaskList, or on the New for User tab, so sorting works properly.
			}
			else {
				FillGrid(new List<Signalod>());//Invalidate view without calling db.
			}
		}

		///<summary>Adds Signalod.SignalNums to each instance of this control's list of sent Task/TaskList related signalnums.
		///Method is static so that each signalNum is only added once to each instance of UserControlTasks.</summary>
		private static void AddSentSignalNums(List<long> listSignalNums) {
			if(listSignalNums==null || listSignalNums.Count==0) {
				return;
			}
			foreach(UserControlTasks control in _listUserControlTaskss) {
				control._listSignalNumsSentTask.AddRange(listSignalNums);
			}
		}

		///<summary>Removes any matching Signalod.SignalNums from this instance's list of sent Task/TaskList related signalnums.</summary>
		private List<Signalod> RemoveSentSignalNums(List<Signalod> listSignalodsReceived) {
			if(listSignalodsReceived==null || listSignalodsReceived.Count==0) {
				return new List<Signalod>();
			}
			for(int i=listSignalodsReceived.Count-1;i>=0;i--) {
				long signalNumReceived=listSignalodsReceived[i].SignalNum;
				if(_listSignalNumsSentTask.Contains(signalNumReceived)) {
					_listSignalNumsSentTask.Remove(signalNumReceived);
					listSignalodsReceived.RemoveAt(i);
				}
			}
			return listSignalodsReceived;
		}

		///<summary>Determines if Task should display in the 'New for' tab.  If using TasksNewTrackedByUser preference, Task.IsUnread must be correctly 
		///set prior to calling this method.</summary>
		private bool IsInNewTab(Task task) {
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser) && task.IsUnread) {//The new way
				if(!ListTaskListNumsSubscribed.Contains(task.TaskListNum)) {
					return false;
				}
				return true;
			}
			else if(!PrefC.GetBool(PrefName.TasksNewTrackedByUser) && task.TaskStatus==TaskStatusEnum.New) {//Tasks are shared by everyone.
				return true;
			}
			return false;
		}

		///<summary>If listSignals is NULL, a full refresh/query will be run for the grid.  If listSignals contains one signal of InvalidType.Task for 
		///a task in _listTasks, the task is already refreshed in memory and only the one task is refreshed from the database.
		///Otherwise, a full refresh will only be run when certain types of signals corresonding to the current selected tabs are found in listSignals.
		///</summary>
		private void FillGrid(List<Signalod> listSignalods=null,bool isManualRefresh=false,bool isFilterRefresh=false) {
			// These two try-catches will preserve the selection when a signal comes in and triggers fillgrid()
			Task taskSelected=null;
			try {
				taskSelected=gridMain.SelectedTag<Task>();
			}
			catch(Exception ex) {
				//Grid can come desynced with its selected indices after a refresh, which was causing crashes. 
				//Now we won't crash, but there's a non-zero chance of the task we just marked read
				//losing its selected status.
				ex.DoNothing();
			}
			TaskList taskListSelected=null;
			try{
				taskListSelected=gridMain.SelectedTag<TaskList>();
			}
			catch(Exception ex) {
				//Same problem as above. Our task list might lose its selected status,
				//but it's better than just crashing.
				ex.DoNothing();
			}
			if(Security.CurUser==null 
				|| (RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT && !Security.IsUserLoggedIn)) 
			{
				gridMain.BeginUpdate();
				gridMain.SetAll(false);
				gridMain.ListGridRows.Clear();
				gridMain.EndUpdate();
				return;
			}
			long taskListNum;
			DateTime dateTime;
			if(_listTaskListTreeHistory==null){
				return;
			}
			if(_listTaskListTreeHistory.Count>0) {//not on main trunk
				taskListNum=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
				dateTime=DateTime.MinValue;
			}
			else {//one of the main trunks
				taskListNum=0;
				dateTime=monthCalendar.SelectionStart;
			}
			LayoutManager.MoveHeight(gridMain,ClientSize.Height-gridMain.Top);
			if(listSignalods==null) {//Full refresh.
				if(!isFilterRefresh) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
			}
			else {
				//Remove any Task related signals that originated from this instance of OpenDental.
				listSignalods=RemoveSentSignalNums(listSignalods.FindAll(x => new List<InvalidType>()
					{ InvalidType.Task,InvalidType.TaskList,InvalidType.TaskAuthor,InvalidType.TaskPatient }.Contains(x.IType)));
				//User is observing a task list for which a TaskList signal is specified, or TaskList from signal is a sublist of current view.
				if(listSignalods.Exists(x => x.IType==InvalidType.TaskList && (x.FKey==taskListNum || _listTaskLists.Exists(y => y.TaskListNum==x.FKey)))) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
				//User is observing the New Tasks tab and a TaskList signal is received for a TaskList the user is subscribed to.
				else if(tabControl.SelectedTab==tabNew && listSignalods.Exists(x => x.IType==InvalidType.TaskList && ListTaskListNumsSubscribed.Contains(x.FKey))) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
				//User is observing the Open Tasks tab and a TaskAuthor signal is received with the current user specified in the FKey.
				else if(tabControl.SelectedTab==tabOpenTickets && listSignalods.Exists(x => x.IType==InvalidType.TaskAuthor && x.FKey==Security.CurUser.UserNum)) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
				//User is observing the Patient Tasks tab and a TaskPatient signal is received for the patient the user currently has selected.
				else if(tabControl.SelectedTab==tabPatientTickets && listSignalods.Exists(x => x.IType==InvalidType.TaskPatient && x.FKey==FormOpenDental.PatNumCur)) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
				else {//Individual Task signals. Only refreshes if the task is in the currently displayed list of Tasks. Add/Remove is addressed with TaskList signals.
					foreach(Signalod signalod in listSignalods) {
						if(signalod.IType.In(InvalidType.Task,InvalidType.TaskPopup) && signalod.FKeyType==KeyType.Task) {
							if(_listTasks.Exists(x => x.TaskNum==signalod.FKey)) {//A signal indicates that a task we are looking at has been modified.
								RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);//Full refresh.
								break;
							}
						}
					}
				}
			}
			#region dated trunk automation
			//dated trunk automation-----------------------------------------------------------------------------
			if(_listTaskListTreeHistory.Count==0//main trunk
				&& (tabControl.SelectedTab==tabDate || tabControl.SelectedTab==tabWeek || tabControl.SelectedTab==tabMonth))
			{
				//clear any lists which are derived from a repeating list and which do not have any items checked off
				bool changeMade=false;
				for(int i=0;i<_listTaskLists.Count;i++) {
					if(_listTaskLists[i].FromNum==0) {//ignore because not derived from a repeating list
						continue;
					}
					if(!AnyAreMarkedComplete(_listTaskLists[i])) {
						DeleteEntireList(_listTaskLists[i]);
						changeMade=true;
					}
				}
				//clear any tasks which are derived from a repeating task 
				//and which are still new (not marked viewed or done)
				for(int i=0;i<_listTasks.Count;i++) {
					if(_listTasks[i].FromNum==0) {
						continue;
					}
					if(_listTasks[i].TaskStatus==TaskStatusEnum.New) {
						Tasks.Delete(_listTasks[i].TaskNum);
						changeMade=true;
					}
				}
				if(changeMade) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
				//now add back any repeating lists and tasks that meet the criteria
				//Get lists of all repeating lists and tasks of one type.  We will pick items from these two lists.
				List<TaskList> listTaskListsRepeating=new List<TaskList>();
				List<Task> tasksRepeating=new List<Task>();
				if(tabControl.SelectedTab==tabDate){
					listTaskListsRepeating=TaskLists.RefreshRepeating(TaskDateType.Day,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					tasksRepeating=Tasks.RefreshRepeating(TaskDateType.Day,Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
				if(tabControl.SelectedTab==tabWeek){
					listTaskListsRepeating=TaskLists.RefreshRepeating(TaskDateType.Week,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					tasksRepeating=Tasks.RefreshRepeating(TaskDateType.Week,Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
				if(tabControl.SelectedTab==tabMonth) {
					listTaskListsRepeating=TaskLists.RefreshRepeating(TaskDateType.Month,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					tasksRepeating=Tasks.RefreshRepeating(TaskDateType.Month,Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
				//loop through list and add back any that meet criteria.
				changeMade=false;
				bool alreadyExists;
				for(int i=0;i<listTaskListsRepeating.Count;i++) {
					//if already exists, skip
					alreadyExists=false;
					for(int j=0;j<_listTaskLists.Count;j++) {//loop through Main list
						if(_listTaskLists[j].FromNum==listTaskListsRepeating[i].TaskListNum) {
							alreadyExists=true;
							break;
						}
					}
					if(alreadyExists) {
						continue;
					}
					//otherwise, duplicate the list
					listTaskListsRepeating[i].DateTL=dateTime;
					listTaskListsRepeating[i].FromNum=listTaskListsRepeating[i].TaskListNum;
					listTaskListsRepeating[i].IsRepeating=false;
					listTaskListsRepeating[i].Parent=0;
					listTaskListsRepeating[i].ObjectType=0;//user will have to set explicitly
					DuplicateExistingList(listTaskListsRepeating[i],true);//repeating lists cannot be subscribed to, so send null in as old list, will not attempt to move subscriptions
					changeMade=true;
				}
				for(int i=0;i<tasksRepeating.Count;i++) {
					//if already exists, skip
					alreadyExists=false;
					for(int j=0;j<_listTasks.Count;j++) {//loop through Main list
						if(_listTasks[j].FromNum==tasksRepeating[i].TaskNum) {
							alreadyExists=true;
							break;
						}
					}
					if(alreadyExists) {
						continue;
					}
					//otherwise, duplicate the task
					tasksRepeating[i].DateTask=dateTime;
					tasksRepeating[i].FromNum=tasksRepeating[i].TaskNum;
					tasksRepeating[i].IsRepeating=false;
					tasksRepeating[i].TaskListNum=0;
					//repeatingTasks[i].UserNum//repeating tasks shouldn't get a usernum
					Tasks.Insert(tasksRepeating[i]);
					changeMade=true;
				}
				if(changeMade) {
					RefreshMainLists(taskListNum,dateTime,isManualRefresh: isManualRefresh);
				}
			}//End of dated trunk automation--------------------------------------------------------------------------
			#endregion dated trunk automation
			// Having GetFilteredTaskLists() out here prevents having to call DB every time we type in the filter.
			List<TaskList> listTaskLists=GetFilteredTaskLists();
			//bool isTaskSelectedVisible=gridMain.IsTagVisible(_clickedTask);
			bool isHqAndTriageList=PrefC.GetBool(PrefName.DockPhonePanelShow) && taskListNum==_TriageListNum;//True if HQ and looking at 'Triage' task list
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("",17);
			gridColumn.ImageList=imageListTree;
			gridMain.Columns.Add(gridColumn);//Checkbox column
			if(tabControl.SelectedTab==tabNew && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//The old way
				gridColumn=new GridColumn(Lan.g("TableTasks","Read"),35,HorizontalAlignment.Center);
				//col.ImageList=imageListTree;
				gridMain.Columns.Add(gridColumn);
			}
			if(tabControl.SelectedTab==tabNew || tabControl.SelectedTab==tabOpenTickets || tabControl.SelectedTab==tabPatientTickets) {
				gridColumn=new GridColumn(Lan.g("TableTasks","Task List"),90);
				gridMain.Columns.Add(gridColumn);
			}
			gridColumn=new GridColumn(Lan.g(this,"+/-"),17,HorizontalAlignment.Center);
			gridColumn.HeaderClick+=GridHeaderClickEvent;
			gridMain.Columns.Add(gridColumn);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)){//HQ
				gridColumn=new GridColumn(Lan.g("TableTasks","ST"),30,HorizontalAlignment.Center);//ST
				gridMain.Columns.Add(gridColumn);
				List<long> listPatsNotInDict=_listTasks.Where(x => x.ObjectType==TaskObjectType.Patient && x.KeyNum!=0 && !_dictPatStates.ContainsKey(x.KeyNum))
					.Select(x => x.KeyNum).ToList();
				Dictionary<long,string> dictPatNewStates=Patients.GetStatesForPats(listPatsNotInDict);
				foreach(long patNum in dictPatNewStates.Keys) {
					_dictPatStates.Add(patNum,dictPatNewStates[patNum]);
				}
				if(taskListNum!=_TriageListNum) {//Everything that's not triage
					gridColumn=new GridColumn(Lan.g("TableTasks","Job"),30,HorizontalAlignment.Center);//Job
					gridMain.Columns.Add(gridColumn);
				}
			}
			if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
				gridColumn=new GridColumn(Lan.g("TableTasks","Att"),30,HorizontalAlignment.Center);//Attachment(s)
				gridMain.Columns.Add(gridColumn);
			}
			if(isHqAndTriageList){//HQ and triage list only
				gridColumn=new GridColumn(Lan.g("TableTasks","Category"),70,HorizontalAlignment.Center);//Category
				gridMain.Columns.Add(gridColumn);
			}
			gridColumn=new GridColumn(Lan.g("TableTasks","Description"),80);//any width
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			string dateStr="";
			string objDesc="";
			string tasklistdescript="";
			string notes="";
			//These strings are always inserted into cells, so they are always set to "" even if there is no job or attachment.
 			string jobNumString="";
			string attStr="";
			string categoryStr="";
			int imageindex;
			List<Def> listDefsTaskCategory=Defs.GetDefsForCategory(DefCat.TaskCategories);
			List<Def> listDefsTaskPriority=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			// pre-define the index that will represent taskSelected or taskListSelected
			int selectedIndex=-1;
			for(int i=0;i<listTaskLists.Count;i++) {
				dateStr="";
				if(listTaskLists[i].DateTL.Year>1880
					&& (tabControl.SelectedTab==tabUser || tabControl.SelectedTab==tabMain || tabControl.SelectedTab==tabReminders))
				{
					if(listTaskLists[i].DateType==TaskDateType.Day) {
						dateStr=listTaskLists[i].DateTL.ToShortDateString()+" - ";
					}
					else if(listTaskLists[i].DateType==TaskDateType.Week) {
						dateStr=Lan.g(this,"Week of")+" "+listTaskLists[i].DateTL.ToShortDateString()+" - ";
					}
					else if(listTaskLists[i].DateType==TaskDateType.Month) {
						dateStr=listTaskLists[i].DateTL.ToString("MMMM")+" - ";
					}
				}
				objDesc="";
				if(tabControl.SelectedTab==tabUser){
					objDesc=listTaskLists[i].ParentDesc;
				}
				tasklistdescript=listTaskLists[i].Descript;
				imageindex=0;
				if(listTaskLists[i].NewTaskCount>0){
					imageindex=3;//orange
					tasklistdescript=tasklistdescript+"("+listTaskLists[i].NewTaskCount.ToString()+")";
				}
				gridRow=new GridRow();
				gridRow.Cells.Add(imageindex.ToString());
				gridRow.Cells.Add("");
				if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ.  Add if job manager is available
					gridRow.Cells.Add("");//ST
					if(taskListNum!=_TriageListNum) {//Everything that's not triage
						gridRow.Cells.Add("");//Job
					}
				}
				if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
					gridRow.Cells.Add("");//Att
				}
				if(isHqAndTriageList){//HQ and triage list only
					gridRow.Cells.Add("");//Category
				}
				gridRow.Cells.Add(dateStr+objDesc+tasklistdescript);
				gridRow.Tag=listTaskLists[i];
				gridMain.ListGridRows.Add(gridRow);
				// Check if this taskList was selected before FillGrid() was called.
				if(taskListSelected!=null && _listTaskLists[i].TaskListNum==taskListSelected.TaskListNum ){
					selectedIndex=gridMain.ListGridRows.Count-1;
				}
			}
			List<long> listAptNums=_listTasks.Where(x => x.ObjectType==TaskObjectType.Appointment).Select(y => y.KeyNum).ToList();
			SerializableDictionary<long,string> dictApptObjDescripts=Tasks.GetApptObjDescripts(listAptNums);
			for(int i=0;i<_listTasks.Count;i++) {
				dateStr="";
				jobNumString="";
				string stateString="";
				attStr="";
				categoryStr="";
				Color colorTaskCategory=Defs.GetColor(DefCat.TaskCategories,_listTasks[i].TriageCategory,listDefsTaskCategory);
				Color colorTaskPriority=Defs.GetColor(DefCat.TaskPriorities,_listTasks[i].PriorityDefNum,listDefsTaskPriority);
				if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
					stateString=HQStateColumn(_listTasks[i]);//ST
					if(taskListNum!=_TriageListNum) {//Everything that's not triage
						//get list of jobs attached to task then insert info about those jobs.
						List<long> listJobNums=_listJobLinks.Where(x => x.FKey==_listTasks[i].TaskNum).Select(x => x.JobNum).ToList();
						if(_listJobs.Any(x => listJobNums.Contains(x.JobNum))) {
							jobNumString="X";//Is job
						}
					}
				}
				if(_listTaskAttachments.Any(x => x.TaskNum==_listTasks[i].TaskNum)) {
					attStr="X";//Has attachments
				}
				if(isHqAndTriageList){//HQ and triage list only
					categoryStr=Defs.GetName(DefCat.TaskCategories,_listTasks[i].TriageCategory,listDefsTaskCategory);
				}
				if(tabControl.SelectedTab==tabUser || tabControl.SelectedTab==tabNew
					|| tabControl.SelectedTab==tabOpenTickets || tabControl.SelectedTab==tabMain 
					|| tabControl.SelectedTab==tabReminders	|| tabControl.SelectedTab==tabPatientTickets) 
				{
					if(_listTasks[i].DateTask.Year>1880) {
						if(_listTasks[i].DateType==TaskDateType.Day) {
							dateStr+=_listTasks[i].DateTask.ToShortDateString()+" - ";
						}
						else if(_listTasks[i].DateType==TaskDateType.Week) {
							dateStr+=Lan.g(this,"Week of")+" "+_listTasks[i].DateTask.ToShortDateString()+" - ";
						}
						else if(_listTasks[i].DateType==TaskDateType.Month) {
							dateStr+=_listTasks[i].DateTask.ToString("MMMM")+" - ";
						}
					}
					else if(_listTasks[i].DateTimeEntry.Year>1880) {
						dateStr+=_listTasks[i].DateTimeEntry.ToShortDateString();
						//if(_listTasks[i].DescriptOverride==""){
							dateStr+=" "+_listTasks[i].DateTimeEntry.ToShortTimeString();
						//}
						dateStr+=" - ";
					}
				}
				objDesc="";
				if(_listTasks[i].TaskStatus==TaskStatusEnum.Done){
					objDesc=Lan.g(this,"Done:")+_listTasks[i].DateTimeFinished.ToShortDateString()+" - ";
				}
				if(_listTasks[i].ObjectType==TaskObjectType.Patient){// && _listTasks[i].DescriptOverride=="") {
					if(_listTasks[i].KeyNum!=0) {
						objDesc+=_listTasks[i].PatientName+" - ";
						if(PrefC.IsODHQ) {
							objDesc+=_listTasks[i].KeyNum+" - ";
						}
					}
				}
				else if(_listTasks[i].ObjectType==TaskObjectType.Appointment) {
					if(_listTasks[i].KeyNum!=0) {
						dictApptObjDescripts.TryGetValue(_listTasks[i].KeyNum,out objDesc);
					}
				}
				if(!_listTasks[i].Descript.StartsWith("==") && _listTasks[i].UserNum!=0) {
					objDesc+=Userods.GetName(_listTasks[i].UserNum)+" - ";
				}
				notes="";
				List<TaskNote> listTaskNotes=_listTaskNotes.FindAll(x => x.TaskNum==_listTasks[i].TaskNum);
				if(!_listTaskNumsExpanded.Contains(_listTasks[i].TaskNum) && listTaskNotes.Count>1) {
					TaskNote taskNoteLast=listTaskNotes[listTaskNotes.Count-1];
					notes+="\r\n\u22EE\r\n" //Vertical ellipse followed by last note. \u22EE - vertical ellipses
							+"=="+Userods.GetName(taskNoteLast.UserNum)+" - "
							+taskNoteLast.DateTimeNote.ToShortDateString()+" "
							+taskNoteLast.DateTimeNote.ToShortTimeString()
							+" - "+taskNoteLast.Note;
				}
				else { //Expanded
					foreach(TaskNote note in listTaskNotes) {
						notes+="\r\n"//even on the first loop
							+"=="+Userods.GetName(note.UserNum)+" - "
							+note.DateTimeNote.ToShortDateString()+" "
							+note.DateTimeNote.ToShortTimeString()
							+" - "+note.Note;
					}
				}
				gridRow=new GridRow();
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//The new way
					if(_listTasks[i].TaskStatus==TaskStatusEnum.Done) {
						gridRow.Cells.Add("1");
					}
					else {
						if(_listTasks[i].IsUnread) {
							gridRow.Cells.Add("4");
						}
						else{
							gridRow.Cells.Add("2");
						}
					}
				}
				else {
					switch(_listTasks[i].TaskStatus) {
						case TaskStatusEnum.New:
							gridRow.Cells.Add("4");
							break;
						case TaskStatusEnum.Viewed:
							gridRow.Cells.Add("2");
							break;
						case TaskStatusEnum.Done:
							gridRow.Cells.Add("1");
							break;
					}
					if(tabControl.SelectedTab==tabNew) {//In this mode, there's a extra column in this tab
						gridRow.Cells.Add("read");
					}
				}
				if(tabControl.SelectedTab==tabNew || tabControl.SelectedTab==tabOpenTickets || tabControl.SelectedTab==tabPatientTickets) {
					gridRow.Cells.Add(_listTasks[i].ParentDesc);
				}
				if(_listTasks[i].DescriptOverride!=""){
					gridRow.Cells.Add("");// +/- is irrelevant
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
						gridRow.Cells.Add(stateString);//ST
						if(taskListNum!=_TriageListNum) {//Everything that's not triage
							gridRow.Cells.Add(jobNumString);//Job
						}
					}
					if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
						gridRow.Cells.Add(attStr);//Att
					}
					if(isHqAndTriageList){//HQ and triage list only
						gridRow.Cells.Add(categoryStr);//Category
						gridRow.Cells.Last().ColorBackG=colorTaskCategory;
					}
					gridRow.Cells.Add(dateStr+objDesc+_listTasks[i].DescriptOverride);
					if(isHqAndTriageList) {//HQ and triage list only
						gridRow.Cells.Last().ColorBackG=colorTaskPriority;
					}
				}
				else if(_listTaskNumsExpanded.Contains(_listTasks[i].TaskNum)) {//Expanded
					if(_listTasks[i].Descript.Length>250 || listTaskNotes.Count>1 || (listTaskNotes.Count==1 && notes.Length>250)) {
						gridRow.Cells.Add("-");
					}
					else {
						gridRow.Cells.Add("");
					}
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
						gridRow.Cells.Add(stateString);//ST
						if(taskListNum!=_TriageListNum) {//Everything that's not triage
							gridRow.Cells.Add(jobNumString);//Job
						}
					}
					if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
						gridRow.Cells.Add(attStr);//Att
					}
					if(isHqAndTriageList){//HQ and triage list only
						gridRow.Cells.Add(categoryStr);//Category
						gridRow.Cells.Last().ColorBackG=colorTaskCategory;
					}
					gridRow.Cells.Add(dateStr+objDesc+_listTasks[i].Descript+notes);
					if(isHqAndTriageList) {//HQ and triage list only
						gridRow.Cells.Last().ColorBackG=colorTaskPriority;
					}
				}
				else {//not expanded
					//Conditions for giving collapse option: Descript is long, there is more than one note, or there is one note and it's long.
					if(_listTasks[i].Descript.Length>250 || listTaskNotes.Count>1 || (listTaskNotes.Count==1 && notes.Length>250)) {
						gridRow.Cells.Add("+");
						if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
							gridRow.Cells.Add(stateString);//ST
							if(taskListNum!=_TriageListNum) {//Everything that's not triage
								gridRow.Cells.Add(jobNumString);//Job
							}
						}
						if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
							gridRow.Cells.Add(attStr);//Att
						}
						if(isHqAndTriageList){//HQ and triage list only
							gridRow.Cells.Add(categoryStr);//Category
							gridRow.Cells.Last().ColorBackG=colorTaskCategory;
						}
						string stringRow=dateStr+objDesc;
						if(_listTasks[i].Descript.Length>250) {
							stringRow+=_listTasks[i].Descript.Substring(0,250)+"(...)";//546,300 tasks have average Descript length of 142.1 characters.
						}
						else {
							stringRow+=_listTasks[i].Descript;
						}
						if(notes.Length>250) {
							stringRow+=notes.Substring(0,250)+"(...)";
						}
						else {
							stringRow+=notes;
						}
						gridRow.Cells.Add(stringRow);
						if(isHqAndTriageList) {//HQ and triage list only
							gridRow.Cells.Last().ColorBackG=colorTaskPriority;
						}
					}
					else {//Descript length <= 250 and notes <=1 and note length is <= 250.  No collapse option.
						gridRow.Cells.Add("");
						if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
							gridRow.Cells.Add(stateString);//ST
							if(taskListNum!=_TriageListNum) {//Everything that's not triage
								gridRow.Cells.Add(jobNumString);//Job
							}
						}
						if(!isHqAndTriageList) {//Everything that is not HQ's triage task list will have the attachments column
							gridRow.Cells.Add(attStr);//Att
						}
						if(isHqAndTriageList){//HQ and triage list only
							gridRow.Cells.Add(categoryStr);//Category
							gridRow.Cells.Last().ColorBackG=colorTaskCategory;
						}
						gridRow.Cells.Add(dateStr+objDesc+_listTasks[i].Descript+notes);
						if(isHqAndTriageList) {//HQ and triage list only
							gridRow.Cells.Last().ColorBackG=colorTaskPriority;
						}
					}
				}
				if(!isHqAndTriageList){//HQ's triage list should not have color coded row as that task list is treated differently
					gridRow.ColorBackG=Defs.GetColor(DefCat.TaskPriorities,_listTasks[i].PriorityDefNum);//No need to do any text detection for triage priorities, we'll just use the task priority colors.
				}
				gridRow.Tag=_listTasks[i];
				gridMain.ListGridRows.Add(gridRow);
				// Check if this task was selected before FillGrid() was called
				if(taskSelected!=null && _listTasks[i].TaskNum==taskSelected.TaskNum) {//_clickedTask can be a TaskList
					selectedIndex=gridMain.ListGridRows.Count-1;
				}
			}
			gridMain.EndUpdate();
			// Reselect the Task / TaskList that was selected before FillGrid() was called
			gridMain.SetSelected(selectedIndex,true);
			//Without this 'scroll value reset', drilling down into a tasklist that contains tasks will sometimes result in an empty grid, until the user 
			//interacts with the grid, example, scrolling will cause the grid to repaint and properly display the expected tasks.
			gridMain.ScrollValue=gridMain.ScrollValue;//this forces scroll value to reset if it's > allowed max.
			if(tabControl.SelectedTab==tabOpenTickets) {
				SetOpenTicketTab(gridMain.ListGridRows.Count);
			}
			if(tabControl.SelectedTab==tabPatientTickets) {
				SetPatientTicketTab(gridMain.ListGridRows.Count);
			}
			else {
				if(!isFilterRefresh) {
					SetPatientTicketTab(-1);
				}
			}
			SetControlTitleHelper();
		}

		///<summary>Helper used to fill ST column for HQ.
		///Only call after determining if HQ.</summary>
		private string HQStateColumn(Task task) {
			long patNum=(task.ObjectType==TaskObjectType.Patient?task.KeyNum:0);
			if(_dictPatStates.ContainsKey(patNum)) {
				return _dictPatStates[patNum];
			}
			else {
				return "";
			}
		}

		///<summary>Click event for GridMain's collapse/expand column header.</summary>
		private void GridHeaderClickEvent(object sender,EventArgs e) {
			if(_taskCollapsedState==-1) {//Mixed mode
				_taskCollapsedState=_isCollapsedByDefault ? 1 : 0;
				FillGrid();//Re-do the grid with whatever their default mode is.
				return; 
			}
			if(_taskCollapsedState==0) {//All are NOT collapsed. Make them all collapsed.
				_taskCollapsedState=1;
				FillGrid();
				return;
			}
			if(_taskCollapsedState==1) {//All ARE collapsed.  Make them all NOT collapsed.
				_taskCollapsedState=0;
				FillGrid();
				return;
			}
		}

		///<summary>Updates ControlParentTitle to give more information about the currently selected task list.  Currently only called in FillGrid()</summary>
		private void SetControlTitleHelper() {
			if(FillGridEvent==null){//Delegate has not been assigned, so we do not care.
				return;
			}
			string descriptTaskList="";
			if(tabControl.SelectedTab==tabNew) {//Special case tab. All grid rows are guaranteed to be task so we manually set values.
				descriptTaskList=Lan.g(this,"New for")+" "+Security.CurUser.UserName;
			}
			else if(_listTaskListTreeHistory.Count>0){//Not in main trunk
				descriptTaskList=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].Descript;
			}
			if(descriptTaskList=="") {//Should only happen when at main trunk.
				TitleControlParent=Lan.g(this,"Tasks");
			}
			else {
				int tasksNewCount=_listTaskLists.Sum(x => x.NewTaskCount);
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					tasksNewCount+=_listTasks.Count(x => x.IsUnread);
				}
				else {
					tasksNewCount+=_listTasks.Count(x => x.TaskStatus==TaskStatusEnum.New);
				}
				TitleControlParent=Lan.g(this,"Tasks")+" - "+descriptTaskList+" ("+tasksNewCount.ToString()+")";
			}
			FillGridEvent.Invoke(this,new EventArgs());
		}

		///<summary>A recursive function that checks every child in a list IsFromRepeating.  If any are marked complete, then it returns true, signifying that this list should be immune from being deleted since it's already in use.</summary>
		private bool AnyAreMarkedComplete(TaskList taskList) {
			//get all children:
			List<TaskList> listTaskListsChildren=TaskLists.RefreshChildren(taskList.TaskListNum,Security.CurUser.UserNum,0,TaskType.Normal);
			List<Task> listTasksChild=Tasks.RefreshChildren(taskList.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.Normal);
			for(int i=0;i<listTaskListsChildren.Count;i++) {
				if(AnyAreMarkedComplete(listTaskListsChildren[i])) {
					return true;
				}
			}
			for(int i=0;i<listTasksChild.Count;i++) {
				if(listTasksChild[i].TaskStatus==TaskStatusEnum.Done) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns a filtered and reordered list of task lists based on the text within the Task List Filter control.</summary>
		private List<TaskList> GetFilteredTaskLists() {
			string filterToUpper=textListFilter.Text.ToUpper().Trim();
			if(string.IsNullOrWhiteSpace(filterToUpper)) {
				return _listTaskLists;
			}
			//Filter out any task lists that do not contail the filter text.
			List<TaskList> listTaskLists=_listTaskLists.FindAll(x => x.Descript.ToUpper().Trim().Contains(filterToUpper));
			//Ordering will be:
			//1. Task lists whose name starts with the filter text
			//2. Task lists whose name contains the filter text (alphabetically)
			return listTaskLists.OrderByDescending(x => x.Descript.ToUpper().Trim().StartsWith(filterToUpper))
				.ThenBy(x => x.Descript).ToList();
		}

		///<summary>If parent=0, then this is a trunk.</summary>
		private void RefreshMainLists(long taskListNum,DateTime date,bool isManualRefresh=false) {
			if(this.DesignMode){
				_listTaskLists=new List<TaskList>();
				_listTasks=new List<Task>();
				_listTaskNotes=new List<TaskNote>();
				return;
			}
			if(tabControl==null) {
				return;
			}
			_listSignalNumsSentTask.Clear();//Full refresh, tracked sent signals are now irrelevant and taking up memory.
			TaskType taskType=TaskType.Normal;
			if(tabControl.SelectedTab==tabReminders) {
				taskType=TaskType.Reminder;
			}
			//Clear copy lists if switching between tabs.
			UI.TabPage tabPageSelected=tabControl.SelectedTab;
			if(tabPageSelected is null || _tabNamePrevious!=tabPageSelected.Name) {
				_listTaskListsCopy=new List<TaskList>();
				_listTasksCopy=new List<Task>();
			}
			if(taskListNum!=0){//not a trunk
				//if(TreeHistory.Count>0//we already know this is true
				long userNumInbox=TaskLists.GetMailboxUserNum(_listTaskListTreeHistory[0].TaskListNum);
				_listTaskLists=TaskLists.RefreshChildren(taskListNum,Security.CurUser.UserNum,userNumInbox,taskType,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshChildren(taskListNum,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,userNumInbox,taskType,
					_isTaskSortApptDateTime,_listClinicNumsFilter,_listDefNumsRegionFilter,_dateFilterStart,_dateFilterEnd,_patientFilter);
			}
			else if(tabControl.SelectedTab==tabUser) {
				//If HQ clinic or clinics disabled, default to "0" Region.
				_listTaskLists=TaskLists.RefreshUserTrunk(Security.CurUser.UserNum,Clinics.ClinicNum,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				lock(_listTaskListNumsSubscribed) {
					_listTaskListNumsSubscribed=_listTaskLists.Select(x => x.TaskListNum).ToList();
				}
				_listTasks=new List<Task>();//no tasks in the user trunk
			}
			else if(tabControl.SelectedTab==tabNew) {
				_listTaskLists=new List<TaskList>();//no task lists in new tab
				_listTasks=Tasks.RefreshUserNew(Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
				lock(_listTaskListNumsSubscribed) {
					_listTaskListNumsSubscribed=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
				}
			}
			else if(tabControl.SelectedTab==tabOpenTickets) {
				_listTaskLists=new List<TaskList>();//no task lists in new tab
				_listTasks=Tasks.RefreshOpenTickets(Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
			}
			else if(tabControl.SelectedTab==tabPatientTickets) {
				_listTaskLists=new List<TaskList>();
				_listTasks=new List<Task>();
				if(FormOpenDental.PatNumCur!=0) {
					_listTasks=Tasks.RefreshPatientTickets(FormOpenDental.PatNumCur,Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
			}
			else if(tabControl.SelectedTab==tabMain) {
				if(!PrefC.GetBool(PrefName.EnterpriseManualRefreshMainTaskLists)) {
					_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Normal,Clinics.ClinicNum
						,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Normal
						,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
				else if(isManualRefresh) {
					_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Normal,Clinics.ClinicNum
						,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Normal
						,_listClinicNumsFilter,_listDefNumsRegionFilter);
					//Store references to the list of Tasks and TaskLists so they can be used when navigating from a child task list to the parent of the Main tab.
					_listTaskListsCopy=_listTaskLists;
					_listTasksCopy=_listTasks;
				}
				else {//Navigating from Main tab child task list, use the referenced lists instead of running a query.
					_listTaskLists=_listTaskListsCopy;
					_listTasks=_listTasksCopy;
				}
			}
			else if(tabControl.SelectedTab==tabReminders) {
				if(!PrefC.GetBool(PrefName.EnterpriseManualRefreshMainTaskLists)) {
					_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Reminder,Clinics.ClinicNum
						,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Reminder
						,_listClinicNumsFilter,_listDefNumsRegionFilter);
				}
				else if(isManualRefresh) { 
					_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Reminder,Clinics.ClinicNum
						,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Reminder
						,_listClinicNumsFilter,_listDefNumsRegionFilter);
					//Store references to the list of Tasks and TaskLists so they can be used when navigating from a child task list to the parent of the Reminder tab.
					_listTaskListsCopy=_listTaskLists;
					_listTasksCopy=_listTasks;
				}
				else {//Navigating from Reminder tab child task list, use the referenced lists instead of running a query.
					_listTaskLists=_listTaskListsCopy;
					_listTasks=_listTasksCopy;
				}
			}
			else if(tabControl.SelectedTab==tabRepeating) {
				_listTaskLists=TaskLists.RefreshRepeatingTrunk(Security.CurUser.UserNum,Clinics.ClinicNum,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshRepeatingTrunk(Security.CurUser.UserNum,_listClinicNumsFilter,_listDefNumsRegionFilter);
			}
			else if(tabControl.SelectedTab==tabDate) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Day,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Day,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_listClinicNumsFilter,_listDefNumsRegionFilter);
			}
			else if(tabControl.SelectedTab==tabWeek) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Week,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Week,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_listClinicNumsFilter,_listDefNumsRegionFilter);
			}
			else if(tabControl.SelectedTab==tabMonth) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Month,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Month,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_listClinicNumsFilter,_listDefNumsRegionFilter);
			}
			if(PrefC.IsODHQ && _listTasks!=null && _listTasks.Count>0) {
				_listJobLinks=JobLinks.GetForTaskNums(_listTasks.Select(x => x.TaskNum).ToList());
				_listJobs=Jobs.GetMany(_listJobLinks.Select(x => x.JobNum).ToList());
			}
			if(_listTasks!=null && _listTasks.Count>0) {		
				_listTaskAttachments=TaskAttachments.GetForTaskNums(_listTasks.Select(x => x.TaskNum).ToList());
			}
			//An old bug allowed a user to be subscribed to the same task list more than once so we need to try/catch the filling of this dictionary
			if(_dictTaskLists==null) {
				_dictTaskLists=new Dictionary<long,TaskList>();
				foreach(TaskList taskList in _listTaskLists) {
					try {
						_dictTaskLists.Add(taskList.TaskListNum,taskList);
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
			}
			if(!_isShowArchivedTaskLists) {
				_listTaskLists.RemoveAll(x => x.TaskListStatus==TaskListStatusEnum.Archived || TaskLists.IsAncestorTaskListArchived(ref _dictTaskLists,x));
			}
			//notes
			List<long> taskNums=new List<long>();
			for(int i=0;i<_listTasks.Count;i++) {
				taskNums.Add(_listTasks[i].TaskNum);
			}
			if(_hasListSwitched) {
				if(_isCollapsedByDefault) {
					_listTaskNumsExpanded.Clear();
				}
				else {
					_listTaskNumsExpanded.AddRange(taskNums);
				}
				_hasListSwitched=false;
			}
			else {
				if(_taskCollapsedState==1) {//Header was clicked, make all collapsed
					_listTaskNumsExpanded.Clear();				
				}
				else if(_taskCollapsedState==0) {//Header was clicked, make all expanded
					_listTaskNumsExpanded.AddRange(taskNums);
				}
				else { 
					for(int i=_listTaskNumsExpanded.Count-1;i>=0;i--) {
						if(!taskNums.Contains(_listTaskNumsExpanded[i])) {
							_listTaskNumsExpanded.Remove(_listTaskNumsExpanded[i]);//The Task was removed from the visual list, don't keep it around in the expanded list.
						}
					}
				}
			}
			_listTaskNotes=TaskNotes.RefreshForTasks(taskNums);
			_tabNamePrevious="";
			if(tabControl.SelectedTab!=null) {
				_tabNamePrevious=tabControl.SelectedTab.Name;
			}
		}

		///<summary>Returns a list of TaskLists containing all directly and indirectly subscribed TaskLists for the current user.</summary>
		public static List<TaskList> GetSubscribedTaskLists(long userNum) {
			List<TaskList> listTaskListsAll=TaskLists.GetAll();
			List<long> listSubscribedTaskListNums=TaskSubscriptions.GetTaskSubscriptionsForUser(userNum).Select(x => x.TaskListNum).ToList();
			List<TaskList> listTaskListsQueue=listTaskListsAll.FindAll(x => listSubscribedTaskListNums.Contains(x.TaskListNum));//Task lists to consider.
			List<TaskList> listTaskListsSubscribed=new List<TaskList>();
			while(listTaskListsQueue.Count>0) {
				TaskList taskList=listTaskListsQueue[0];
				listTaskListsQueue.RemoveAt(0);//pop
				if(!listTaskListsSubscribed.Contains(taskList)) {//Avoid duplicate return values
					listTaskListsSubscribed.Add(taskList);//Each item added to the queue will be part of the return list.
				}
				List<TaskList> taskListChildren=listTaskListsAll.FindAll(x => x.Parent==taskList.TaskListNum);//Children of taskList.
				foreach(TaskList child in taskListChildren) {
					if(!listTaskListsSubscribed.Contains(child) && !listTaskListsQueue.Contains(child)) {//Avoid duplicate return values.
						listTaskListsQueue.Add(child);//push
					}
				}
			}
			return listTaskListsSubscribed;
		}

		private void tabControl_Selecting(object sender,int e) {
			//We use Selecting so that this fires even if user clicks on the same tab that's already selected.
			tabControl.SelectedIndex=e;
			_listTaskListTreeHistory=new List<TaskList>();//clear the tree no matter which tab clicked.
			_hasListSwitched=true;
			SetFiltersToDefault();//Fills Tree and Grid
			//Allows mouse wheel scroll without having to click in grid.  Helpful on 'Main' as it is populated with task lists, which drill down on single click.
			gridMain.Focus();
		}

		private void cal_DateSelected(object sender,System.Windows.Forms.DateRangeEventArgs e) {
			_listTaskListTreeHistory=new List<TaskList>();//clear the tree
			FillTree();
			FillGrid();
		}

		private void ToolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//if(e.Button.Tag.GetType()==typeof(string)){
			//standard predefined button
			switch(e.Button.Tag.ToString()) {
				case "Options":
					Options_Clicked();
					break;
				case "AddList":
					AddList_Clicked();
					break;
				case "AddTask":
					AddTask_Clicked();
					break;
				case "Search":
					Search_Clicked();
					break;
				case "BlockSubsc":
					BlockSubsc_Clicked();
					break;
				case "Filter":
					Filter_Clicked();
					break;
			}
		}
	
		private void Options_Clicked() {
			using FormTaskOptions formTaskOptions=new FormTaskOptions(_isShowFinishedTasks,_dateTimeStartShowFinished,_isTaskSortApptDateTime
				,_isShowArchivedTaskLists);
			formTaskOptions.StartPosition=FormStartPosition.Manual;//Allows us to set starting form starting Location.
			Point pointFormLocation=this.PointToScreen(ToolBarMain.Location);//Since we cant get ToolBarMain.Buttons["Options"] location directly.
			pointFormLocation.X+=ToolBarMain.Buttons["Options"].Bounds.Width;//Add Options button width so by default form opens along side button.
			Rectangle rectangleScreenDim=SystemInformation.VirtualScreen;//Dimensions of users screen. Includes if user has more then 1 screen.
			if(pointFormLocation.X+formTaskOptions.Width > rectangleScreenDim.Width) {//Not all of form will be on screen, so adjust.
				pointFormLocation.X=rectangleScreenDim.Width-formTaskOptions.Width-5;//5 for some padding.
			}
			if(pointFormLocation.Y+formTaskOptions.Height > rectangleScreenDim.Height) {//Not all of form will be on screen, so adjust.
				pointFormLocation.Y=rectangleScreenDim.Height-formTaskOptions.Height-5;//5 for some padding.
			}
			formTaskOptions.Location=pointFormLocation;
			formTaskOptions.ShowDialog();
			if(formTaskOptions.DialogResult!=DialogResult.OK){
				return;
			}
			_isShowFinishedTasks=formTaskOptions.ShowFinishedTasks;
			_isShowArchivedTaskLists=formTaskOptions.ShowArchivedTaskLists;
			_dateTimeStartShowFinished=formTaskOptions.DateTimeStartShowFinished;
			_isTaskSortApptDateTime=formTaskOptions.DoSortApptDateTime;
			_isCollapsedByDefault=PIn.Bool(UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse)[0].ValueString);
			_hasListSwitched=true;//To display tasks in correctly collapsed/expanded state
			FillGrid();
		}

		private void Filter_Clicked() {

			long keyNum;
			FrmTaskFilter frmTaskFilter=new FrmTaskFilter();
			Task task = gridMain.SelectedTag<Task>();
			if(task==null) {
				keyNum=0;
			}
			else {
				keyNum=task.KeyNum;
				if(task.ObjectType==TaskObjectType.Patient) {
					frmTaskFilter.PatientTask=Patients.GetPat(keyNum);
				}
				else if(task.ObjectType==TaskObjectType.Appointment) {
					Appointment appointment=Appointments.GetOneApt(keyNum);
					if(appointment!=null) {//Appointment could get deleted after clicking on task, so check for null
						frmTaskFilter.PatientTask=Patients.GetPat(appointment.PatNum);
					}
				}
			}
			if(_patientFilter!=null) {
				frmTaskFilter.PatientSelected=_patientFilter;
			}
			frmTaskFilter.PatientMain=Patients.GetPat(FormOpenDental.PatNumCur);
			frmTaskFilter.EnumTaskPatientFilterType_=_enumTaskPatientFilterType;
			frmTaskFilter.DateStart=_dateFilterStart;
			frmTaskFilter.DateEnd=_dateFilterEnd;
			frmTaskFilter.ListClinicNumsSelected=_listClinicNumsFilter;
			frmTaskFilter.ListDefNumsRegionsSelected=_listDefNumsRegionFilter;
			frmTaskFilter.ShowDialog();
			if(!frmTaskFilter.IsDialogOK) {
				return;
			}
			_enumTaskPatientFilterType=frmTaskFilter.EnumTaskPatientFilterType_;
			_dateFilterStart=frmTaskFilter.DateStart;
			_dateFilterEnd=frmTaskFilter.DateEnd;
			_listClinicNumsFilter=frmTaskFilter.ListClinicNumsSelected;
			_listDefNumsRegionFilter=frmTaskFilter.ListDefNumsRegionsSelected;
			_patientFilter=frmTaskFilter.PatientSelected;
			if(frmTaskFilter.ClearAllClicked) {
				if(_listTaskListTreeHistory==null) {
					_listTaskListTreeHistory=new List<TaskList>();
				}
				if(_listTaskListTreeHistory.Count>0) {
					TaskList taskList=_listTaskListTreeHistory.Last();
					SetFiltersToDefault(taskList);
				}
				else {
					SetFiltersToDefault();
				}
			}
			FillTree();
			FillGrid();
		}

		private void AddList_Clicked() {
			if(!Security.IsAuthorized(EnumPermType.TaskListCreate,false)) {
				return;
			}
			if(tabControl.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//trunk of user tab
				MsgBox.Show(this,"Not allowed to add a task list to the trunk of the user tab.  Either use the subscription feature, or add it to a child list.");
				return;
			}
			if(tabControl.SelectedTab==tabNew) {//new tab
				MsgBox.Show(this,"Not allowed to add items to the 'New' tab.");
				return;
			}
			if(tabControl.SelectedTab==tabPatientTickets) {
				MsgBox.Show(this,"Not allowed to add a task list to the 'Patient Tasks' tab.");
				return;
			}
			if(tabControl.SelectedTab==tabDate || tabControl.SelectedTab==tabMonth || tabControl.SelectedTab==tabWeek) {
				MsgBox.Show(this,"Not allowed to add a repeating task list here. Changes to repeating task lists can only be made in the Repeating(setup) tab.");
				return;
			}
			TaskList taskList=new TaskList();
			//if this is a child of any other taskList
			if(_listTaskListTreeHistory.Count>0) {
				taskList.Parent=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
			}
			else {
				taskList.Parent=0;
				if(tabControl.SelectedTab==tabDate) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Day;
				}
				else if(tabControl.SelectedTab==tabWeek) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Week;
				}
				else if(tabControl.SelectedTab==tabMonth) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Month;
				}
			}
			if(tabControl.SelectedTab==tabRepeating) {
				taskList.IsRepeating=true;
			}
			taskList.GlobalTaskFilterType=EnumTaskFilterType.Default;//Results in this taskList inheriting value from PrefName.TasksGlobalFilterType
			using FormTaskListEdit formTaskListEdit=new FormTaskListEdit(taskList);
			formTaskListEdit.IsNew=true;
			if(formTaskListEdit.ShowDialog()==DialogResult.OK) {
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source parent tasklist.
				RefillLocalTaskGrids(taskList,listSentSignalNums:new List<long>() { signalNum });
			}
		}

		private void AddTask(bool isReminder) {
			if(tabControl.SelectedTab==tabDate || tabControl.SelectedTab==tabMonth || tabControl.SelectedTab==tabWeek) {
				MsgBox.Show(this,"Not allowed to add a repeating task here. Changes to repeating task lists can only be made in the Repeating(setup) tab.");
				return;
			}
			if(Plugins.HookMethod(this,"UserControlTasks.AddTask_Clicked")) {
				return;
			}
			//if(tabContr.SelectedTab==tabUser && TreeHistory.Count==0) {//trunk of user tab
			//	MsgBox.Show(this,"Not allowed to add a task to the trunk of the user tab.  Add it to a child list instead.");
			//	return;
			//}
			//if(tabContr.SelectedTab==tabNew) {//new tab
			//	MsgBox.Show(this,"Not allowed to add items to the 'New' tab.");
			//	return;
			//}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			//if this is a child of any taskList
			if(_listTaskListTreeHistory.Count>0) {
				task.TaskListNum=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
			}
			else if(tabControl.SelectedTab==tabNew) {//new tab
				task.TaskListNum=-1;//Force FormTaskEdit to ask user to pick a task list.
			}
			else if(tabControl.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//trunk of user tab
				task.TaskListNum=-1;//Force FormTaskEdit to ask user to pick a task list.
			}
			else {
				task.TaskListNum=0;
				if(tabControl.SelectedTab==tabDate) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Day;
				}
				else if(tabControl.SelectedTab==tabWeek) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Week;
				}
				else if(tabControl.SelectedTab==tabMonth) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Month;
				}
			}
			if(tabControl.SelectedTab==tabRepeating) {
				task.IsRepeating=true;
			}
			task.UserNum=Security.CurUser.UserNum;
			if(isReminder) {
				task.ReminderType=TaskReminderType.Once;
			}
			FormTaskEdit formTaskEdit=new FormTaskEdit(task,taskOld);
			formTaskEdit.IsNew=true;
			formTaskEdit.Closing+=new CancelEventHandler(TaskGoToEvent);
			formTaskEdit.Show();//non-modal
		}

		private void AddTask_Clicked() {
			bool isReminder=false;
			if(tabControl.SelectedTab==tabReminders) {
				isReminder=true;
			}
			AddTask(isReminder);
		}

		private void menuItemTaskReminder_Click(object sender,EventArgs e) {
			AddTask(true);
		}

		public void Search_Clicked() {
			string textClip="";
			try {
				textClip=System.Windows.Clipboard.GetText().Trim().ToLower();//System.Windows.Forms.Clipboard fails for Thinfinity
			}
			catch {
				//do nothing
			}
			if(Regex.IsMatch(textClip,@"^tasknum:\d+$")) { //very restrictive specific match for "TaskNum:##"
				long taskNum=PIn.Long(textClip.Substring(8));
				// if the tasknum was the same as last time then we have already tried this search once
				if (taskNum!=_taskNumOld) { // if #'s differ then we are doing a fresh search and should just look for the tasknum
					_taskNumOld=taskNum;
					Task task=Tasks.GetOne(taskNum);
					if (task!=null) { //don't show the task search form and just open up the task that has been found
						FormTaskEdit formTaskEdit=new FormTaskEdit(task);
						formTaskEdit.Show();
						return;
					}
				}
			}
			//if there is no match, open the form as it normally would
			// this doesn't need to be disposed of as it is not shown modally (https://stackoverflow.com/a/3097383)
			FormTaskSearch formTaskbarSearch=new FormTaskSearch();
			_taskNumOld=-1; // reset _taskNumCur so if they click search again with same clipboard contents it will go straight to taskEdit
			formTaskbarSearch.Show();
		}

		public void TaskGoToEvent(object sender,CancelEventArgs e) {
			FormTaskEdit formTaskEdit=(FormTaskEdit)sender;
			if(formTaskEdit.TaskObjectTypeGoTo!=TaskObjectType.None) {
				TaskObjectTypeGoTo=formTaskEdit.TaskObjectTypeGoTo;
				KeyNumGoTo=formTaskEdit.KeyNumGoTo;
				FormOpenDental.S_TaskGoTo(TaskObjectTypeGoTo,KeyNumGoTo);
			}
			if(!this.IsDisposed) {
				FillGrid();
			}
		}

		private void BlockSubsc_Clicked() {
			using FormTaskListBlocks formTaskListBlocks = new FormTaskListBlocks();
			formTaskListBlocks.ShowDialog();
			if(formTaskListBlocks.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Security);
			}
		}

		private void Done_Clicked() {
			//already blocked if list
			Task task=gridMain.SelectedTag<Task>();
			if(task==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return;
			}
			Task taskOld=task.Copy();
			task.TaskStatus=TaskStatusEnum.Done;
			if(task.DateTimeFinished.Year<1880) {
				task.DateTimeFinished=DateTime.Now;
			}
			try {
				Tasks.Update(task,taskOld);
			}
			catch(Exception ex) {
				//We manipulated the TaskStatus and need to set it back to what it was because something went wrong.
				int idx=_listTasks.FindIndex(x => x.TaskNum==taskOld.TaskNum);
				if(idx>-1) {
					_listTasks[idx]=taskOld;
				}
				MessageBox.Show(ex.Message);
				return;
			}
			TaskUnreads.DeleteForTask(task);
			TaskHist taskHist=new TaskHist(taskOld);
			taskHist.UserNumHist=Security.CurUser.UserNum;
			TaskHists.Insert(taskHist);
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum);//Only needs to send signal for the one task.
			RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),new List<long>() { signalNum });//No db call.
		}

		private void Edit_Clicked() {
			TaskList taskList = gridMain.SelectedTag<TaskList>();
			Task task = gridMain.SelectedTag<Task>();
			if(taskList!=null) {//is list
				using FormTaskListEdit formTaskListEdit=new FormTaskListEdit(taskList);
				formTaskListEdit.ShowDialog();
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source parent tasklist.
				RefillLocalTaskGrids(taskList,new List<long>() { signalNum });//No db call.
			}
			else if(task!=null) {//task
				FormTaskEdit formTaskEdit=new FormTaskEdit(task);//Handles signals for this task edit.
				formTaskEdit.Show();//non-modal
			}
			else { //both are null; object was removed and right-click never left.
				MsgBox.Show(this,"Please select a valid task or task list.");
				return;
			}
		}

		private void Cut_Clicked() {
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			Task task=gridMain.SelectedTag<Task>();
			_wasCut=true;
			if(taskList!=null) {//is list
				_taskListClip=taskList.Copy();
				_taskClip=null;
			}
			else if(task!=null) {//task
				_taskListClip=null;
				_taskClip=gridMain.SelectedTag<Task>().Copy();
			}
			else {
				MsgBox.Show(this,"Please select a valid task or task list.");
				_taskListClip=null;
				_taskClip=null;
				_wasCut=false;
			}
		}

		private void Copy_Clicked() {
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			Task task=gridMain.SelectedTag<Task>();
			if(taskList!=null) {//is list
				_taskListClip=taskList.Copy();
				_taskClip=null;
			}
			else if(task!=null) {//task
				_taskListClip=null;
				_taskClip=gridMain.SelectedTag<Task>().Copy();
				if(!String.IsNullOrEmpty(_taskClip.ReminderGroupId)) {
					//Any reminder tasks duplicated must have a brand new ReminderGroupId
					//so that they do not affect the original reminder task chain.
					Tasks.SetReminderGroupId(_taskClip);
				}
			}
			else {
				MsgBox.Show(this,"Please select a valid task or task list.");
				_taskListClip=null;
				_taskClip=null;
			}
			_wasCut=false;
		}

		///<summary>When copying and pasting, Task hist will be lost because the pasted task has a new TaskNum.</summary>
		private void Paste_Clicked() {
			if(_taskListClip!=null) {//a taskList is on the clipboard
				if(!_wasCut) {
					return;//Tasklists are no longer allowed to be copied, only cut.  Code should never make it this far.
				}
				TaskList taskListNew=_taskListClip.Copy();
				long parentNumClipTL=_taskListClip.Parent;
				if(_listTaskListTreeHistory.Count>0) {//not on main trunk
					taskListNew.Parent=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
					if(tabControl.SelectedTab==tabUser){
						//treat pasting just like it's the main tab, because not on the trunk.
					}
					else if(tabControl.SelectedTab==tabMain){
						//even though usually only trunks are dated, we will leave the date alone in main
						//category since user may wish to preserve it. All other children get date cleared.
					}
					else if(tabControl.SelectedTab==tabReminders) {
						//treat pasting just like it's the main tab.
					}
					else if(tabControl.SelectedTab==tabRepeating){
						taskListNew.DateTL=DateTime.MinValue;//never a date
						//leave dateType alone, since that affects how it repeats
					}
					else if(tabControl.SelectedTab==tabDate
						|| tabControl.SelectedTab==tabWeek
						|| tabControl.SelectedTab==tabMonth) 
					{
						taskListNew.DateTL=DateTime.MinValue;//children do not get dated
						taskListNew.DateType=TaskDateType.None;//this doesn't matter either for children
					}
				}
				else {//one of the main trunks
					taskListNew.Parent=0;
					if(tabControl.SelectedTab==tabUser) {
						//maybe we should treat this like a subscription rather than a paste.  Implement later.  For now:
						MsgBox.Show(this,"Not allowed to paste directly to the trunk of this tab.  Try using the subscription feature instead.");
						return;
					}
					else if(tabControl.SelectedTab==tabMain) {
						taskListNew.DateTL=DateTime.MinValue;
						taskListNew.DateType=TaskDateType.None;
					}
					else if(tabControl.SelectedTab==tabReminders) {
						taskListNew.DateTL=DateTime.MinValue;
						taskListNew.DateType=TaskDateType.None;
					}
					else if(tabControl.SelectedTab==tabRepeating) {
						taskListNew.DateTL=DateTime.MinValue;//never a date
						//newTL.DateType=TaskDateType.None;//leave alone
					}
					else if(tabControl.SelectedTab==tabDate){
						taskListNew.DateTL=monthCalendar.SelectionStart;
						taskListNew.DateType=TaskDateType.Day;
					}
					else if(tabControl.SelectedTab==tabWeek) {
						taskListNew.DateTL=monthCalendar.SelectionStart;
						taskListNew.DateType=TaskDateType.Week;
					}
					else if(tabControl.SelectedTab==tabMonth) {
						taskListNew.DateTL=monthCalendar.SelectionStart;
						taskListNew.DateType=TaskDateType.Month;
					}
				}
				if(tabControl.SelectedTab==tabRepeating) {
					taskListNew.IsRepeating=true;
				}
				else {
					taskListNew.IsRepeating=false;
				}
				taskListNew.FromNum=0;//always
				if(_taskListClip.TaskListNum==taskListNew.Parent && _wasCut) {
					MsgBox.Show(this,"Cannot cut and paste a task list into itself.  Please move it into a different task list.");
					return;
				}
				if(TaskLists.IsAncestor(_taskListClip.TaskListNum,taskListNew.Parent)) {
					//The user is attempting to cut or copy a TaskList into one of its ancestors.  We don't want to do normal movement logic for this case.
					//We move the TaskList desired to have its parent to the list they desire.  
					//We change the TaskList's direct children to have the parent of the TaskList being moved.
					MoveListIntoAncestor(taskListNew,_taskListClip.Parent);
				}
				else {
					//If the user has task filters on this TaskList or one of its children, prompt the user they may be moving tasks that are filtered.
					if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)!=EnumTaskFilterType.Disabled &&
						(_enumTaskFilterTypeForList!=EnumTaskFilterType.None || TaskLists.HasGlobalFilterTypeInTree(taskListNew)) && !ODBuild.IsUnitTest)
					{
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel
							,"Task filters are turned on in this task list or one of its sub lists.  Pasting will cause filtered tasks to move as "
							+"well.  Affects all users.  Continue?")) 
						{
							return;
						}
					}
					if(tabControl.SelectedTab==tabUser || tabControl.SelectedTab==tabMain || tabControl.SelectedTab==tabReminders) {
						MoveTaskList(taskListNew,true);
					}
					else {
						MoveTaskList(taskListNew,false);
					}
				}
				List<long> listSignalNums=new List<long>();
				if(parentNumClipTL!=0) {
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,parentNumClipTL));//Signal for source parent tasklist.
				}
				if(taskListNew.Parent!=0) {
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskListNew.Parent));//Signal for destination parent tasklist.
				}
				RefillLocalTaskGrids(taskListNew,listSignalNums);//No db call.
			}
			else if(_taskClip!=null) {//a task is on the clipboard
				Task taskNew=_taskClip.Copy();
				long clipTaskTaskListNum=_taskClip.TaskListNum;
				if(_listTaskListTreeHistory.Count>0) {//not on main trunk
					taskNew.TaskListNum=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
					if(tabControl.SelectedTab==tabUser) {
						//treat pasting just like it's the main tab, because not on the trunk.
					}
					else if(tabControl.SelectedTab==tabMain) {
						//even though usually only trunks are dated, we will leave the date alone in main
						//category since user may wish to preserve it. All other children get date cleared.
					}
					else if(tabControl.SelectedTab==tabReminders) {
						//treat pasting just like it's the main tab.
					}
					else if(tabControl.SelectedTab==tabRepeating) {
						taskNew.DateTask=DateTime.MinValue;//never a date
						//leave dateType alone, since that affects how it repeats
					}
					else if(tabControl.SelectedTab==tabDate
						|| tabControl.SelectedTab==tabWeek
						|| tabControl.SelectedTab==tabMonth) 
					{
						taskNew.DateTask=DateTime.MinValue;//children do not get dated
						taskNew.DateType=TaskDateType.None;//this doesn't matter either for children
					}
				}
				else {//one of the main trunks
					taskNew.TaskListNum=0;
					if(tabControl.SelectedTab==tabUser) {
						//never allowed to have a task on the user trunk.
						MsgBox.Show(this,"Tasks may not be pasted directly to the trunk of this tab.  Try pasting within a list instead.");
						return;
					}
					else if(tabControl.SelectedTab==tabMain) {
						taskNew.DateTask=DateTime.MinValue;
						taskNew.DateType=TaskDateType.None;
					}
					else if(tabControl.SelectedTab==tabReminders) {
						taskNew.DateTask=DateTime.MinValue;
						taskNew.DateType=TaskDateType.None;
					}
					else if(tabControl.SelectedTab==tabRepeating) {
						taskNew.DateTask=DateTime.MinValue;//never a date
						//newTL.DateType=TaskDateType.None;//leave alone
					}
					else if(tabControl.SelectedTab==tabDate) {
						taskNew.DateTask=monthCalendar.SelectionStart;
						taskNew.DateType=TaskDateType.Day;
					}
					else if(tabControl.SelectedTab==tabWeek) {
						taskNew.DateTask=monthCalendar.SelectionStart;
						taskNew.DateType=TaskDateType.Week;
					}
					else if(tabControl.SelectedTab==tabMonth) {
						taskNew.DateTask=monthCalendar.SelectionStart;
						taskNew.DateType=TaskDateType.Month;
					}
				}
				if(tabControl.SelectedTab==tabRepeating) {
					taskNew.IsRepeating=true;
				}
				else {
					taskNew.IsRepeating=false;
				}
				taskNew.FromNum=0;//always
				if(!String.IsNullOrEmpty(taskNew.ReminderGroupId)) {
					//Any reminder tasks duplicated to another task list must have a brand new ReminderGroupId
					//so that they do not affect the original reminder task chain.
					Tasks.SetReminderGroupId(taskNew);
				}
				if(_wasCut && Tasks.WasTaskAltered(_taskClip)){
					MsgBox.Show("Tasks","Not allowed to move because the task has been altered by someone else.");
					FillGrid();
					return;
				}
				string descriptHist="";
				List<TaskNote> listTaskNotes;
				List<long> listSignalNums=new List<long>();
				if(_wasCut) { //cut
					if(clipTaskTaskListNum==taskNew.TaskListNum) {//User cut then paste into the same task list.
						return;//Nothing to do.
					}
					listTaskNotes=TaskNotes.GetForTask(taskNew.TaskNum);
					descriptHist="This task was cut from task list "+TaskLists.GetFullPath(_taskClip.TaskListNum)+" and pasted into "+TaskLists.GetFullPath(taskNew.TaskListNum);
					Tasks.Update(taskNew,_taskClip);
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,clipTaskTaskListNum));//Signal for source tasklist.
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,_taskClip.TaskNum));//Signal for current task.
				}
				else { //copied
					listTaskNotes=TaskNotes.GetForTask(taskNew.TaskNum);
					taskNew.TaskNum=Tasks.Insert(taskNew);//Creates a new PK for newT  Copy, no need to signal source.
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskNew.TaskNum));//Signal for new task.
					descriptHist="This task was copied from task "+_taskClip.TaskNum+" in task list "+TaskLists.GetFullPath(_taskClip.TaskListNum);
					for(int t=0;t<listTaskNotes.Count;t++) {
						listTaskNotes[t].TaskNum=taskNew.TaskNum;
						TaskNotes.Insert(listTaskNotes[t]);//Creates the new note with the current datetime stamp.
						TaskNotes.Update(listTaskNotes[t]);//Restores the historical datetime for the note.
					}
				}
				TaskHist taskHist=new TaskHist(taskNew);
				taskHist.Descript=descriptHist;
				taskHist.UserNum=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,taskNew.TaskNum);//Popup
				TaskUnreads.AddUnreads(taskNew,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskNew.TaskListNum));//Signal for destination tasklist.
				RefillLocalTaskGrids(taskNew,listTaskNotes,listSignalNums);//No db call.
			}
			//Turn the cut into a copy once the users has pasted at least once.
			_wasCut=false;
		}

		/// <summary>Return the FormTaskEdit that was created from showing the task.  Can return null.</summary>
		private FormTaskEdit SendToMe_Clicked(bool openTask=true) {
			if(Security.CurUser.TaskListInBox==0) {
				MsgBox.Show(this,"You do not have an inbox.");
				return null;
			}
			Task task=gridMain.SelectedTag<Task>();
			if(task==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return null;
			}
			Task taskOld=task.Copy();
			task.TaskListNum=Security.CurUser.TaskListInBox;
			Cursor=Cursors.WaitCursor;
			List<long> listSignalNums=new List<long>();
			try {
				Tasks.Update(task,taskOld);
				//At HQ the refresh interval wasn't quick enough for the task to pop up.
				//We will immediately show the task instead of waiting for the refresh interval.
				TaskHist taskHist=new TaskHist(taskOld);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskOld.TaskListNum));//Signal for old TaskList containing this Task.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,task.TaskListNum));//Signal for new tasklist.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum));//Signal for task.
				RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),listSignalNums);
				Cursor=Cursors.Default;
				FormTaskEdit formTaskEdit=new FormTaskEdit(task,task.Copy());
				formTaskEdit.IsPopup=true;
				if(openTask) {
					formTaskEdit.Show();//non-modal
				}
				return formTaskEdit;
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				FillGrid();//Full refresh on local machine.  This will revert/refresh the clicked task so any changes made above are ignored.
				return null;
			}
		}

		/// <summary>Sends a task to the current user, opens the task, and opens a new tasknote for the user to edit.</summary>
		private void SendToMeAndGoto_Clicked() {
			Task task=gridMain.SelectedTag<Task>();
			FormTaskEdit formTaskEditOpened=SendToMe_Clicked(openTask:false);
			if(formTaskEditOpened==null) {
				return;
			}
			Goto_Clicked(task);
			formTaskEditOpened.Show();//We want to show any popups first before we open the task.
			//If opened from another form and the user presses cancel on FormTaskNoteEdit, it will hide the task behind the parent form (this).  
			//Calling activate makes sure if we cancel out, the topmost form will be FormTaskEdit.
			formTaskEditOpened.Activate();
			//String should not be changed.  Used for auditing triage tasks.
			formTaskEditOpened.AddNoteToTaskAndEdit("Returned call. ");
			Tasks.TaskEditCreateLog(EnumPermType.TaskNoteEdit,Lan.g(this,"Automatically added task note")+": Returned Call",Tasks.GetOne(formTaskEditOpened.TaskCur.TaskNum));
		}

		private void Goto_Clicked(Task task=null) {
			Task taskGoTo=task??gridMain.SelectedTag<Task>();
			//not even allowed to get to this point unless a valid task
			if(taskGoTo==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return;
			}
			TaskObjectTypeGoTo=taskGoTo.ObjectType;
			KeyNumGoTo=taskGoTo.KeyNum;
			FormOpenDental.S_TaskGoTo(TaskObjectTypeGoTo,KeyNumGoTo);
		}

		///<summary>Marks the selected task as read and updates the grid.</summary>
		private void MarkRead(Task taskMarked) {
			if(taskMarked==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return;
			}
			taskMarked.IsUnread=TaskUnreads.IsUnread(Security.CurUser.UserNum,taskMarked);
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				if(tabControl.SelectedTab==tabNew){
					//these are never in someone else's inbox, so don't block.
				}
				else if(tabControl.SelectedTab==tabPatientTickets 
					&& taskMarked.IsUnread) 
				{
					//Task clicked is new for the user, don't block.
				}
				else{
					long userNumInbox=0;
					if(tabControl.SelectedTab.In(tabOpenTickets,tabPatientTickets)) {
						userNumInbox=TaskLists.GetMailboxUserNumByAncestor(taskMarked.TaskNum);
					}
					else {
						if(_listTaskListTreeHistory.Count!=0) {
							userNumInbox=TaskLists.GetMailboxUserNum(_listTaskListTreeHistory[0].TaskListNum);
						}
						else {
							MsgBox.Show(this,"Please setup task lists before marking tasks as read.");
							return;
						}
					}
					if(userNumInbox != 0 && userNumInbox != Security.CurUser.UserNum) {
						MsgBox.Show(this,"Not allowed to mark off tasks in someone else's inbox.");
						return;
					}
				}
				if(taskMarked.IsUnread) {
					if(Tasks.IsReminderTask(taskMarked) && taskMarked.DateTimeEntry>DateTime.Now){
						MsgBox.Show(this,"Not allowed to mark future Reminders as read.");
					}
					else{
						TaskUnreads.SetRead(Security.CurUser.UserNum,taskMarked);//Takes care of Db.
					}
				}
				long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskMarked.TaskNum);//Signal for markedTask.
				RefillLocalTaskGrids(taskMarked,_listTaskNotes.FindAll(x => x.TaskNum==taskMarked.TaskNum),new List<long>() { signalNum });
				//if already read, nothing else to do.  If done, nothing to do
			}
			else {
				if(taskMarked.TaskStatus==TaskStatusEnum.New) {
					Task task=taskMarked.Copy();
					Task taskOld=task.Copy();
					task.TaskStatus=TaskStatusEnum.Viewed;
					try {
						Tasks.Update(task,taskOld);
						long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum);//Send signal for this task.
						RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),new List<long>() { signalNum });
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
				//no longer allowed to mark done from here
			}
		}

		private void NavAtt_Clicked(TaskAttachment taskAttachment) { 
			if(taskAttachment==null) {
				MsgBox.Show(this,"Could not open attachment.");
				return;
			}
			Task task=Tasks.GetOne(taskAttachment.TaskNum);
			if(task==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return;
			}
			FormTaskAttachmentEdit formTaskAttachmentEdit=new FormTaskAttachmentEdit(task);
			formTaskAttachmentEdit.TaskAttachmentCur=taskAttachment;
			formTaskAttachmentEdit.ShowDialog();
			if(formTaskAttachmentEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(task,_listTaskNotes,new List<long>() { signalNum });			
		}

		private void MoveListIntoAncestor(TaskList taskListNew,long taskListNumParentOld) {
			if(_wasCut) {//If the TaskList was cut, move direct children of the list "up" one in the hierarchy and then update
				List<TaskList> listTaskListsChildren=TaskLists.RefreshChildren(taskListNew.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
				for(int i=0;i<listTaskListsChildren.Count;i++) {
					listTaskListsChildren[i].Parent=taskListNumParentOld;
					TaskLists.Update(listTaskListsChildren[i]);
				}
				TaskLists.Update(taskListNew);
			}
			else {//Just insert a new TaskList if it was copied.
				TaskLists.Insert(taskListNew);
			}
		}

		///<summary>Assign new parent FKey for existing tasklist, and update TaskAncestors.  Used when cutting and pasting a tasklist.
		///Does not create new task or tasklist entries.</summary>
		private void MoveTaskList(TaskList taskListNew,bool isInMainOrUser) {
			List<TaskList> listTaskListsChildren=TaskLists.RefreshChildren(taskListNew.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> listTasksChild=Tasks.RefreshChildren(taskListNew.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All);//No filtering, because all child tasks should move regardless of filtration.
			TaskLists.Update(taskListNew);//Not making a new TaskList, just moving an old one
			for(int i=0;i<listTaskListsChildren.Count;i++) { //updates all the child tasklists and recursively calls this method for each of their children lists.
				listTaskListsChildren[i].Parent=taskListNew.TaskListNum;
				if(taskListNew.IsRepeating) {
					listTaskListsChildren[i].IsRepeating=true;
					listTaskListsChildren[i].DateTL=DateTime.MinValue;//never a date
				}
				else {
					listTaskListsChildren[i].IsRepeating=false;
				}
				listTaskListsChildren[i].FromNum=0;
				if(!isInMainOrUser) {
					listTaskListsChildren[i].DateTL=DateTime.MinValue;
					listTaskListsChildren[i].DateType=TaskDateType.None;
				}
				MoveTaskList(listTaskListsChildren[i],isInMainOrUser);//delete any existing subscriptions
			}
			TaskAncestors.SynchManyForSameTasklist(listTasksChild,taskListNew.TaskListNum,taskListNew.Parent);
		}

		///<summary>Only used for dated task lists. Should NOT be used for regular task lists, puts too much strain on DB with large amount of tasks.
		///A recursive function that duplicates an entire existing TaskList.  
		///For the initial loop, make changes to the original taskList before passing it in.  
		///That way, Date and type are only set in initial loop.  All children preserve original dates and types. 
		///The isRepeating value will be applied in all loops.  Also, make sure to change the parent num to the new one before calling this function.
		///The taskListNum will always change, because we are inserting new record into database. </summary>
		private void DuplicateExistingList(TaskList taskListNew,bool isInMainOrUser) {
			//get all children:
			List<TaskList> listTaskListChildren=TaskLists.RefreshChildren(taskListNew.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> listTasksChild=Tasks.RefreshChildren(taskListNew.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All);//No filtering, because all child tasks should duplicate regardless of filtration.
			if(_wasCut) { //Not making a new TaskList, just moving an old one
				TaskLists.Update(taskListNew);
			}
			else {//copied -- We are making a new TaskList, we're keeping the old one as well
				TaskLists.Insert(taskListNew);
			}
			//now we have a new taskListNum to work with
			for(int i=0;i<listTaskListChildren.Count;i++) { //updates all the child tasklists and recursively calls this method for each of their children lists.
				listTaskListChildren[i].Parent=taskListNew.TaskListNum;
				if(taskListNew.IsRepeating) {
					listTaskListChildren[i].IsRepeating=true;
					listTaskListChildren[i].DateTL=DateTime.MinValue;//never a date
				}
				else {
					listTaskListChildren[i].IsRepeating=false;
				}
				listTaskListChildren[i].FromNum=0;
				if(!isInMainOrUser) {
					listTaskListChildren[i].DateTL=DateTime.MinValue;
					listTaskListChildren[i].DateType=TaskDateType.None;
				}
				DuplicateExistingList(listTaskListChildren[i],isInMainOrUser);//delete any existing subscriptions
			}
			for(int i = 0;i<listTasksChild.Count;i++) { //updates all the child tasks. If the task list was cut, then just update the child tasks' ancestors.
				if(_wasCut) {
					TaskAncestors.Synch(listTasksChild[i]);
				}
				else {//copied
					listTasksChild[i].TaskListNum=taskListNew.TaskListNum;
					if(taskListNew.IsRepeating) {
						listTasksChild[i].IsRepeating=true;
						listTasksChild[i].DateTask=DateTime.MinValue;//never a date
					}
					else {
						listTasksChild[i].IsRepeating=false;
					}
					listTasksChild[i].FromNum=0;
					if(!isInMainOrUser) {
						listTasksChild[i].DateTask=DateTime.MinValue;
						listTasksChild[i].DateType=TaskDateType.None;
					}
					if(!String.IsNullOrEmpty(listTasksChild[i].ReminderGroupId)) {
						//Any reminder tasks duplicated to another task list must have a brand new ReminderGroupId
						//so that they do not affect the original reminder task chain.
						Tasks.SetReminderGroupId(listTasksChild[i]);
					}
					List<TaskNote> noteList=TaskNotes.GetForTask(listTasksChild[i].TaskNum);
					long taskNumNew=Tasks.Insert(listTasksChild[i]);
					for(int t=0;t<noteList.Count;t++) {
						noteList[t].TaskNum=taskNumNew;
						TaskNotes.Insert(noteList[t]);//Creates the new note with the current datetime stamp.
						TaskNotes.Update(noteList[t]);//Restores the historical datetime for the note.
					}
				}
			}
		}

		private void Delete_Clicked() {
			TaskList taskList = gridMain.SelectedTag<TaskList>();
			Task task = gridMain.SelectedTag<Task>();
			if(taskList!=null) {//is list
				//check to make sure the list is empty.  Do not filter tasks so we don't try to delete a list that still has tasks.
				List<Task> listTasks=Tasks.RefreshChildren(taskList.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All);
				List<TaskList> listTaskLists=TaskLists.RefreshChildren(taskList.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
				int countHiddenTasks=listTaskLists.Sum(x => x.NewTaskCount)+listTasks.Count-taskList.NewTaskCount;
				if(listTasks.Count>0 || listTaskLists.Count>0){
					MessageBox.Show(Lan.g(this,"Not allowed to delete a list unless it's empty.  This task list contains:")+"\r\n"
						+listTasks.FindAll(x => String.IsNullOrEmpty(x.ReminderGroupId)).Count+" "+Lan.g(this,"normal tasks")+"\r\n"
						+listTasks.FindAll(x => !String.IsNullOrEmpty(x.ReminderGroupId)).Count+" "+Lan.g(this,"reminder tasks")+"\r\n"
						+countHiddenTasks+" "+Lan.g(this,"filtered tasks")+"\r\n"
						+listTaskLists.Count+" "+Lan.g(this,"task lists"));
					return;
				}
				if(TaskLists.GetMailboxUserNum(taskList.TaskListNum)!=0) {
					MsgBox.Show(this,"Not allowed to delete task list because it is attached to a user inbox.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this empty list?")) {
					return;
				}
				TaskSubscriptions.UpdateTaskListSubs(taskList.TaskListNum,0);
				TaskLists.Delete(taskList);
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source tasklist.
				RefillLocalTaskGrids(taskList,new List<long>() { signalNum },false);//No db calls.
			}
			else if (task!=null) {//Is task
				//This security logic should match FormTaskEdit for when we enable the delete button.
				bool isTaskForCurUser = true;
				if(task.UserNum!=Security.CurUser.UserNum) {//current user didn't write this task, so block them.
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(task.TaskListNum!=Security.CurUser.TaskListInBox) {//the task is not in the logged-in user's inbox
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(isTaskForCurUser) {
					List<TaskNote> listTaskNotes=TaskNotes.GetForTask(task.TaskNum);//so we can check so see if other users have added notes
					for(int i = 0;i<listTaskNotes.Count;i++) {
						if(Security.CurUser.UserNum!=listTaskNotes[i].UserNum) {
							isTaskForCurUser=false;
							break;
						}
					}
				}
				//Purposefully show a popup if the user is not authorized to delete this task.
				if(!Security.IsAuthorized(EnumPermType.TaskDelete)) {
					return;
				}
				//This logic should match FormTaskEdit.butDelete_Click()
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Task?")) {
					return;
				}
				if(Tasks.GetOne(task.TaskNum)==null) {
					MsgBox.Show(this,"Task already deleted.");
					return;
				}
				if(task.TaskListNum==0) {
					Tasks.TaskEditCreateLog(Lan.g(this,"Deleted task"),task);
				}
				else {
					string logText=Lan.g(this,"Deleted task from tasklist");
					if(taskList!=null) {
						logText+=" "+taskList.Descript;
					}
					else {
						logText+=". Task list no longer exists";
					}
					logText+=".";
					Tasks.TaskEditCreateLog(logText,task);
				}
				int countDocuments=TaskAttachments.GetCountDocumentForTaskNum(task.TaskNum);
				Tasks.Delete(task.TaskNum);//always do it this way to clean up all five tables (six if hq)
				if(countDocuments>0) { 
					MsgBox.Show(this,"This task has linked document(s). The task attachments have been deleted and the documents can be deleted via the imaging module, if desired.");
				}
				List<long> listSignalNums=new List<long>();
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,task.TaskListNum));//Signal for source tasklist.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum));//Signal for current task.
				RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),listSignalNums,false);
				TaskHist taskHist = new TaskHist(task);
				taskHist.IsNoteChange=false;
				taskHist.UserNum=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				SecurityLogs.MakeLogEntry(EnumPermType.TaskDelete,0,"Task "+POut.Long(task.TaskNum)+" deleted",0);
			}
			else {
				MsgBox.Show(this, "Please select a valid task or task list.");
				return;
			}
		}

		///<summary>A recursive function that deletes the specified list and all children.</summary>
		private void DeleteEntireList(TaskList taskList) {
			//get all children:
			List<TaskList> listTaskListsChildren=TaskLists.RefreshChildren(taskList.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> listTasksChild=Tasks.RefreshChildren(taskList.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All);
			for(int i=0;i<listTaskListsChildren.Count;i++) {
				DeleteEntireList(listTaskListsChildren[i]);
			}
			for(int i=0;i<listTasksChild.Count;i++) {
				Tasks.Delete(listTasksChild[i].TaskNum);
			}
			try {
				TaskLists.Delete(taskList);
			}
			catch(Exception e) {
				MessageBox.Show(e.Message);
			}
		}

		///<summary>The indexing logic here could be improved to be easier to read, by modifying the fill grid to save
		///column indexes into class-wide private varaibles.  This way we will have access to the index without performing any logic.
		///Additionally, each variable could be set to -1 when the column is not present.</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==0) {//check box column
				//no longer allow double click on checkbox, because it's annoying.
				return;
			}
			if(tabControl.SelectedTab==tabNew && e.Col==2 && PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//+/- column (an index varaible would help)
				return;//Don't double click on expand column, because it already has a single click functionality.
			}
			else if(tabControl.SelectedTab==tabNew && e.Col==3 && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//ST column (an index varaible would help)
				return;//Don't double click on ST column.
			}
			else if(tabControl.SelectedTab==tabNew && e.Col==4 && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//Job column (an index varaible would help)
				return;//Don't double click on Job column.
			}
			else if(e.Col==1) {//Task List column (an index varaible would help)
				return;//Don't double click on expand column
			}
			Task task=gridMain.SelectedTag<Task>();
			if(task!=null) {//is task
				//It's important to grab the task directly from the db because the status in this list is fake, being the "unread" status instead.
				task=Tasks.GetOne(task.TaskNum);
				if(task==null) {//Task was deleted or moved.
					return;
				}
				FormTaskEdit formTaskEdit=new FormTaskEdit(task);
				formTaskEdit.Show();//non-modal
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			if(taskList!=null) { // is a TaskList
				_listTaskListTreeHistory.Add(taskList);
				_hasListSwitched=true;
				SetFiltersToDefault(taskList);//Fills Tree and Grid
				return;
			}
			Task taskSelected=gridMain.SelectedTag<Task>();
			_taskCollapsedState=-1;
			if(tabControl.SelectedTab==tabNew && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)){//There's an extra column
				if(e.Col==1) {
					TaskUnreads.SetRead(Security.CurUser.UserNum,taskSelected);
					FillGrid();
				}
				if(e.Col==3) {//Expand column
					if(_listTaskNumsExpanded.Contains(taskSelected.TaskNum)) {
						_listTaskNumsExpanded.Remove(taskSelected.TaskNum);
					}
					else { 
						_listTaskNumsExpanded.Add(taskSelected.TaskNum);
					}
					FillGrid();
				}
				return;
			}
			if(e.Col==0){//check tasks off
				MarkRead(taskSelected);
			}
			if((tabControl.SelectedTab.In(tabNew,tabPatientTickets,tabOpenTickets) && e.Col==2) 
				|| (tabControl.SelectedTab!=tabNew && e.Col==1)) 
			{
				if(_listTaskNumsExpanded.Contains(taskSelected.TaskNum)) {
					_listTaskNumsExpanded.Remove(taskSelected.TaskNum);
				}
				else { 
					_listTaskNumsExpanded.Add(taskSelected.TaskNum);
				}
				FillGrid();
			}
		}

		private void menuEdit_Popup(object sender,System.EventArgs e) {
			SetMenusEnabled();
		}

		private void SetMenusEnabled() {
			TaskList taskList = gridMain.SelectedTag<TaskList>();
			Task task=gridMain.SelectedTag<Task>();
			//Done----------------------------------
			if(gridMain.SelectedIndices.Length==0 || taskList!=null) {//or a tasklist selected
				menuItemDone.Enabled=false;
			}
			else {
				menuItemDone.Enabled=true;
			}
			//Edit,Cut,Copy,Delete-------------------------
			if(gridMain.SelectedIndices.Length==0) {
				menuItemEdit.Enabled=false;
				menuItemCut.Enabled=false;
				menuItemCopy.Enabled=false;
				menuItemDelete.Enabled=false;
			}
			else {
				menuItemEdit.Enabled=true;
				menuItemCut.Enabled=true;
				if(taskList!=null) {//Is a tasklist
					menuItemCopy.Enabled=false;//We don't want users to copy tasklists, only move them by cut.
				}
				else {
					menuItemCopy.Enabled=true;
				}
				menuItemDelete.Enabled=true;
			}
			//Paste----------------------------------------
			if(tabControl.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//not allowed to paste into the trunk of a user tab
				menuItemPaste.Enabled=false;
			}
			else if(_taskListClip==null && _taskClip==null) {
				menuItemPaste.Enabled=false;
			}
			else {//there is an item on our clipboard
				menuItemPaste.Enabled=true;
			}
			//(overrides)
			if(tabControl.SelectedTab==tabNew || tabControl.SelectedTab==tabOpenTickets || tabControl.SelectedTab==tabPatientTickets) {
				menuItemCut.Enabled=false;
				menuItemDelete.Enabled=false;
				menuItemPaste.Enabled=false;
			}
			//Subscriptions----------------------------------------------------------
			if(gridMain.SelectedIndices.Length==0) {
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=false;
			}
			else if(tabControl.SelectedTab==tabUser && taskList!=null) {//user tab and is a list
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=true;
			}
			else if(tabControl.SelectedTab==tabMain && taskList!=null) {//main and tasklist
				menuItemSubscribe.Enabled=true;
				menuItemUnsubscribe.Enabled=false;
			}
			else if(tabControl.SelectedTab==tabReminders && taskList!=null) {//reminders and tasklist
				menuItemSubscribe.Enabled=true;
				menuItemUnsubscribe.Enabled=false;
			}
			else{//either any other tab, or a task on the main list
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=false;
			}
			menuItemPriority.MenuItems.Clear();
			//SendToMe/GoTo/Task Priority/DeleteTaskTaken---------------------------------------------------------------
			if(gridMain.SelectedIndices.Length>0 && task!=null){//is task
				//The clicked task was removed from _listTasks, could happen between FillGrid(), mouse click, and now
				if(_listTasks.IndexOf(task)<0) {
					IgnoreTaskClick();
					return;
				}
				if(task.ObjectType==TaskObjectType.None) {
					menuItemGoto.Enabled=false;
				}
				else {
					menuItemGoto.Enabled=true;
				}
				if(PrefC.IsODHQ && Security.IsAuthorized(EnumPermType.TaskDelete,true)) {
					menuDeleteTaken.Enabled=true;
				}
				else {
					menuDeleteTaken.Visible=false;//Without this, HQ users without Permissions.TaskEdit still see this disabled item.
				}
				menuItemMarkRead.Enabled=true;
				menuItemSendToMe.Enabled=true;
				//Check if task has patient attached
				if(task.ObjectType==TaskObjectType.Patient) {
					menuItemSendAndGoto.Enabled=true;
				}
				else {
					menuItemSendAndGoto.Enabled=false;
				}
				if(Defs.GetDefsForCategory(DefCat.TaskPriorities,true).Count==0) {
					menuItemPriority.Enabled=false;
				}
				else {
					menuItemPriority.Enabled=true;
					Def[] defArray=Defs.GetDefsForCategory(DefCat.TaskPriorities,true).ToArray();
					foreach(Def def in defArray) {
						MenuItem menuItem=menuItemPriority.MenuItems.Add(def.ItemName);
						menuItem.Click+=(sender,e) => menuTaskPriority_Click(task,def);
					}
				}
				//If a task is read only, disable actions in the right click menu that modify the task
				if(task.IsReadOnly) {
					menuItemDelete.Enabled=false;
					menuItemCut.Enabled=false;
					menuDeleteTaken.Enabled=false;
					menuItemPriority.Enabled=false;
					menuItemDone.Enabled=false;
					menuDeleteTaken.Enabled=false;
					menuItemSendToMe.Enabled=false;
					menuItemSendAndGoto.Enabled=false;
					//If the task is in  the current users task list, the current user created the task, or they have permission to modify read only tasks, allow them to mark it as done.
					if(Tasks.IsAuthorizedOrOwner(task) ||  task.TaskListNum==Security.CurUser.TaskListInBox) {
						menuItemSendToMe.Enabled=true;
						menuItemDone.Enabled=true;
						//Check if task has patient attached
						if(task.ObjectType==TaskObjectType.Patient) {
							menuItemSendAndGoto.Enabled=true;
						}
					}
				}
			}
			else {
				menuItemGoto.Enabled=false;//not a task
				menuItemSendToMe.Enabled=false;
				menuItemSendAndGoto.Enabled=false;
				menuItemPriority.Enabled=false;
				menuItemMarkRead.Enabled=false;
				menuDeleteTaken.Enabled=false;
			}
			//Navigate to Job-------------------------------------------------------------
			if(gridMain.SelectedIndices.Length>0 && task!=null && PrefC.IsODHQ) {
				//The clicked task was removed from _listTasks, could happen between FillGrid(), mouse click, and now
				if(_listTasks.IndexOf(task)<0) {
					IgnoreTaskClick();
					return;
				}
				//get list of jobs attached to task then insert info about those jobs.
				List<JobLink> listJobLinks=JobLinks.GetForTask(task.TaskNum);
				List<Job> listJobs=Jobs.GetMany(listJobLinks.Select(x => x.JobNum).ToList());
				//If a job exists that is attached to the task
				if(listJobs.Count>0) {
					menuNavJob.MenuItems.Clear();	//clear whatever items were in the menu before.
					MenuItem newItem;
					string title;
					//Get a jobnum that matches the column in task menu
					foreach(Job selectedJob in listJobs) {
						title=selectedJob.JobNum.ToString()+" ";
						//Append the correct letter to the jobnum
						switch(selectedJob.Category) {
							case JobCategory.Feature:
								title="F"+title;
								break;
							case JobCategory.Bug:
								title="B"+title;
								break;
							case JobCategory.Enhancement:
								title="E"+title;
								break;
							case JobCategory.Query:
								title="Q"+title;
								break;
							case JobCategory.ProgramBridge:
								title="P"+title;
								break;
							case JobCategory.InternalRequest:
								title="I"+title;
								break;
							case JobCategory.HqRequest:
								title="H"+title;
								break;
							case JobCategory.Conversion:
								title="C"+title;
								break;
							case JobCategory.Research:
								title="R"+title;
								break;
							case JobCategory.NeedNoApproval:
								title="N"+title;
								break;
							case JobCategory.MarketingDesign:
								title="M"+title;
								break;
							case JobCategory.UnresolvedIssue:
								title="U"+title;
								break;
						}
						//Title is: "%jobnum% %description%" with a character limit of 30
						title+=selectedJob.Title;
						if(title.Length>=30) {
							title=title.Substring(0,30);
						}
						title+="...";
						newItem=new MenuItem(title);
						newItem.Tag=selectedJob;
						newItem.Click+=(sender,e) => menuNavJob_Click(sender,e,selectedJob);	//set a custom click event
						menuNavJob.MenuItems.Add(newItem);
					}
					menuNavJob.Enabled=true;
				}
				else {
					menuNavJob.Enabled=false;	//if there are no jobs, then just disable the ability to click or expand the sub-menu
				}
			}
			//Navigate to attachment
			if(gridMain.SelectedIndices.Length>0 && task!=null){//is task
				//The clicked task was removed from _listTasks, could happen between FillGrid(), mouse click, and now
				if(_listTasks.IndexOf(task)<0) {
					IgnoreTaskClick();
					return;
				}
				List<TaskAttachment> listTaskAttachments=TaskAttachments.GetManyByTaskNum(task.TaskNum);
				if(!listTaskAttachments.IsNullOrEmpty()) {
					menuNavAttachment.MenuItems.Clear();	//clear whatever items were in the menu before.
					MenuItem menuItemNew;
					string description;
					for(int i=0;i<listTaskAttachments.Count;i++) { 
						TaskAttachment taskAttachment=listTaskAttachments[i];
						description=taskAttachment.Description;
						if(description.Length>=30) {
							description=description.Substring(0,30);
						}
						description+="...";
						menuItemNew=new MenuItem(description);
						menuItemNew.Tag=taskAttachment;
						menuItemNew.Click+=(sender,e) => menuNavAtt_Click(sender,e,taskAttachment);	//set a custom click event
						menuNavAttachment.MenuItems.Add(menuItemNew);
					}
					menuNavAttachment.Enabled=true;
				}
				else {
					menuNavAttachment.Enabled=false;	//if there are no attachments, then just disable the ability to click or expand the sub-menu
				}
			}
			//Archived/Unarchived-------------------------------------------------------------
			menuArchive.Visible=false;
			menuUnarchive.Visible=false;
			if(taskList!=null && tabControl.SelectedTab==tabMain && _listTaskLists.IndexOf(taskList)>-1 
				&& taskList.TaskListStatus==TaskListStatusEnum.Active)
			{
				menuArchive.Visible=true;
			}
			if(taskList!=null && tabControl.SelectedTab.In(tabUser,tabMain,tabReminders) && _listTaskLists.IndexOf(taskList)>-1
				&& taskList.TaskListStatus==TaskListStatusEnum.Archived)
			{
				menuUnarchive.Visible=true;
			}
			if(gridMain.GetSelectedIndex()<0) {//Not clicked on any row
				menuItemDone.Enabled=false;
				menuItemEdit.Enabled=false;
				menuItemCut.Enabled=false;
				menuItemCopy.Enabled=false;
				//menuItemPaste.Enabled=false;//Don't disable paste because this one makes sense for user to do.
				menuItemDelete.Enabled=false;
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=false;
				menuItemSendToMe.Enabled=false;
				menuItemGoto.Enabled=false;
				menuItemPriority.Enabled=false;
				menuItemMarkRead.Enabled=false;
				return;
			}
		}

		private void IgnoreTaskClick() {
			gridMain.SetAll(false);//unselect problem row
			foreach(MenuItem menuItem in gridMain.ContextMenu.MenuItems) { //disable ContextMenu options
				menuItem.Enabled=false;
			}
			FillGrid();//Full Refresh.
		}

		private void OnSubscribe_Click(){
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			if(taskList==null) {
				MsgBox.Show(this,"Please select a valid task list");
				return;
			}
			//Won't even get to this point unless it is a list.  TaskListNum will never be 0.
			if(TaskSubscriptions.TrySubscList(taskList.TaskListNum,Security.CurUser.UserNum)) {
				lock(_listTaskListNumsSubscribed) {
					_listTaskListNumsSubscribed=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
				}
			}
			else { //already subscribed.
				MsgBox.Show(this,"User already subscribed.");
				return;
			}
			MsgBox.Show(this,"Done");
			RefillLocalTaskGrids(taskList,null);
		}

		private void OnUnsubscribe_Click() {
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			if(taskList==null) {
				MsgBox.Show(this,"Please select a valid task list");
				return;
			}
			TaskSubscriptions.UnsubscList(taskList.TaskListNum,Security.CurUser.UserNum);
			lock(_listTaskListNumsSubscribed) {
				_listTaskListNumsSubscribed=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
			};
			RefillLocalTaskGrids(taskList,null);
		}

		private void menuItemDone_Click(object sender,EventArgs e) {
			Done_Clicked();
		}

		private void menuItemEdit_Click(object sender,System.EventArgs e) {
			Edit_Clicked();
		}

		private void menuItemCut_Click(object sender,System.EventArgs e) {
			Cut_Clicked();
		}

		private void menuItemCopy_Click(object sender,System.EventArgs e) {
			Copy_Clicked();
		}

		private void menuItemPaste_Click(object sender,System.EventArgs e) {
			Paste_Clicked();
		}

		private void menuItemDelete_Click(object sender,System.EventArgs e) {
			Delete_Clicked();
		}

		private void menuItemSubscribe_Click(object sender,EventArgs e) {
			OnSubscribe_Click();
		}

		private void menuItemUnsubscribe_Click(object sender,EventArgs e) {
			OnUnsubscribe_Click();
		}

		private void menuItemSendToMe_Click(object sender,EventArgs e) {
			SendToMe_Clicked();
		}

		private void menuItemSendAndGoto_Click(object sender,EventArgs e) {
			SendToMeAndGoto_Clicked();
		}

		private void menuItemGoto_Click(object sender,System.EventArgs e) {
			Goto_Clicked();
		}

		private void menuItemMarkRead_Click(object sender,EventArgs e) {
			MarkRead(gridMain.SelectedTag<Task>());
		}

		private void menuNavJob_Click(object sender,EventArgs e,Job selectedJob) {
			FormOpenDental.S_GoToJob(selectedJob.JobNum);
		}

		private void menuNavAtt_Click(object sender,EventArgs e,TaskAttachment taskAttachment) {
			NavAtt_Clicked(taskAttachment);
		}

		private void menuDeleteTaken_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			TaskTakens.DeleteForTask(gridMain.SelectedTag<Task>().TaskNum);
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Task taken deleted");
		}

		private void menuTaskPriority_Click(Task task,Def defPriority) {
			Task taskNew=task.Copy();
			taskNew.PriorityDefNum=defPriority.DefNum;
			try {
				Tasks.Update(taskNew,task);
				if(PrefC.IsODHQ && defPriority.DefNum==502) {//They chose Blue as their priority
					TaskNote taskNote=new TaskNote();
					taskNote.UserNum=Security.CurUser.UserNum;
					taskNote.TaskNum=task.TaskNum;
					taskNote.Note="Setting priority to blue.";
					TaskNotes.Insert(taskNote);
					_listTaskNotes.Add(taskNote);
				}
				TaskHist taskHist=new TaskHist(task);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskNew.TaskNum);
				RefillLocalTaskGrids(taskNew,_listTaskNotes.FindAll(x => x.TaskNum==taskNew.TaskNum),new List<long>() { signalNum });
			}
			catch(Exception ex) {//Happens when two users edit the same task at the same time.
				MessageBox.Show(ex.Message);
			}
		}

		private void menuArchive_Click(object sender,EventArgs e) {
			//Will not get here unless clicked index is an unarchived task list
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Archiving a task list will remove all Subscribers. Continue?")) {
				return;
			}
			TaskList taskList = gridMain.SelectedTag<TaskList>();
			TaskLists.Archive(taskList);
			long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source parent tasklist.
			RefillLocalTaskGrids(taskList,new List<long>() { signalNum });//No db call.
		}

		private void menuUnarchive_Click(object sender,EventArgs e) {
			TaskList taskList=gridMain.SelectedTag<TaskList>();
			//Will not get here unless clicked index is an archived task list
			TaskLists.Unarchive(taskList);
			long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source parent tasklist.
			RefillLocalTaskGrids(taskList,new List<long>() { signalNum });//No db call.
		}

		private void textFilter_TextChanged(object sender, EventArgs e) {
			FillGrid(isFilterRefresh:true);
		}

		//private void listMain_SelectedIndexChanged(object sender,System.EventArgs e) {
		//	SetMenusEnabled();
		//}

		private void tree_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			TreeNode treeNodeSelected=tree.GetNodeAt(e.X,e.Y);
			if(treeNodeSelected==null) {
				return;//Clicking just below a node results in tree.GetNodeAt(e.X,e.Y) to be null.  Since user didn't make an actual selection, return.
			}
			for(int i=_listTaskListTreeHistory.Count-1;i>0;i--) {
				if(_listTaskListTreeHistory[i].TaskListNum==(long)treeNodeSelected.Tag) {
					break;//don't remove the node click on or any higher node
				}
				_listTaskListTreeHistory.RemoveAt(i);
			}
			TaskList taskListNewSelection=_listTaskListTreeHistory.FirstOrDefault(x => x.TaskListNum==(long)treeNodeSelected.Tag);
			SetFiltersToDefault(taskListNewSelection);//Fills Tree and Grid.
		}
		
		///<summary>Currently only used so that we can set the title of FormTask.</summary>
		public delegate void FillGridEventHandler(object sender,EventArgs e);

		private void butClearFilter_Click(object sender,EventArgs e) {
			textListFilter.Text="";
		}
		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid(isManualRefresh: true);
		}
	}


	///<summary>Each item in this enumeration identifies a specific tab within UserControlTasks.</summary>
	public enum UserControlTasksTab {
		///<summary>0</summary>
		Invalid,
		///<summary>1</summary>
		ForUser,
		///<summary>2</summary>
		UserNew,
		///<summary>3</summary>
		OpenTickets,
		///<summary>4</summary>
		Main,
		///<summary>5</summary>
		Reminders,
		///<summary>6</summary>
		RepeatingSetup,
		///<summary>7</summary>
		RepeatingByDate,
		///<summary>8</summary>
		RepeatingByWeek,
		///<summary>9</summary>
		RepeatingByMonth,
		///<summary>10</summary>
		PatientTickets
	}

}
