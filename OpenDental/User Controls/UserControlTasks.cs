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
		private TaskList _clipTaskList;
		///<summary>A Task that is on the 'clipboard' waiting to be pasted.  Will be null if nothing has been copied yet.</summary>
		private Task _clipTask;
		///<summary>If there is an item on our 'clipboard', this tracks whether it was cut.</summary>
		private bool _wasCut;
		///<summary>The index of the last clicked item in the main list.</summary>
		private int _clickedI;
		///<summary>After closing, if this is not zero, then it will jump to the object specified in GotoKeyNum.</summary>
		public TaskObjectType GotoType;
		///<summary>After closing, if this is not zero, then it will jump to the specified patient.</summary>
		public long GotoKeyNum;
		///<summary>All notes for the showing tasks, ordered by date time.</summary>
		private List<TaskNote> _listTaskNotes=new List<TaskNote>();
		///<summary>A friendly string that could be used as the title of any window that has this control on it.
		///It will contain the description of the currently selected task list and a count of all new tasks within that list.</summary>
		public string ControlParentTitle;
		private const int _TriageListNum=1697;
		private bool _isTaskSortApptDateTime;//Use task AptDateTime sort setup in FormTaskOptions.
		private bool _isShowFinishedTasks=false;//Show finished task setup in FormTaskOptions.
		private bool _isShowArchivedTaskLists;//Show archived task lists in FormTaskOptions.
		private DateTime _dateTimeStartShowFinished=DateTime.Today.AddDays(-7);//Show finished task date setup in FormTaskOptions.
		///<summary>Keeps track of which tasks are expanded.  Persists between TaskList list updates.</summary>
		private List<long> _listExpandedTaskNums=new List<long>();
		private bool _isCollapsedByDefault;
		private bool _hasListSwitched;
		///<summary>This can be three states: 0 for all tasks expanded, 1 for all tasks collapsed, and -1 for mixed.</summary>
		private int _taskCollapsedState;
		///<summary>When a task is selected via right click, we make a shallow copy of the task so menu options are performed on the correct task.</summary>
		private Task _clickedTask;
		///<summary>The states of patients from previously seen tasks.</summary>
		private Dictionary<long,string> _dictPatStates=new Dictionary<long, string>();
		///<summary>Signalnums for Task or TaskList signals sent from this machine, that have not yet been received back from 
		///FormOpenDental.OnProcessSignals(). Do not include InvalidType.TaskPopup.</summary>
		private List<long> _listSentTaskSignalNums=new List<long>();
		private static List <UserControlTasks> _listInstances=new List<UserControlTasks>();
		///<summary>TaskListNums for TaskLists the current user is subscribed to.
		///Is static so can be referenced from multiple instances of this control.  Locked each time it is accessed so it is thread safe.</summary>
		private static List<long> _listSubscribedTaskListNums=new List<long>();
		///<summary>A list of job links used for displaying whether a task has an attached job.</summary>
		private static List<JobLink> _listJobLinks=new List<JobLink>();
		///<summary>A list of jobs that are used for determining whether a task has an attached job in the fill grid.</summary>
		private static List<Job> _listJobs=new List<Job>();
		///<summary>The action which occurs when the Toggle Chat button is clicked.  Only set for OD HQ triage.</summary>
		private Action _actionChatToggle=null;
		///<summary>Defines which filter type is currently in use for filtering the Task grid.</summary>
		private GlobalTaskFilterType _globalFilterType;
		///<summary>Foreign keys to either a Clinic or a Region Def.  Indicates current filter on the selected TaskList.</summary>
		private List<long> _listFilterFkeys;
		///<summary>Defined here so we can change the text on this button depending on filer setting.</summary>
		private ODToolBarButton _butFilter;
		///<summary>Dictionary to look up task list by primary key. Key: TaskListNum. Value: TaskList.</summary>
		private Dictionary<long,TaskList> _dictTaskLists;
		///<summary></summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		///<summary>Creates a thread safe copy of _listSubscribedTaskListNums.</summary>
		private static List<long> ListSubscribedTaskListNums {
			get {
				List <long> listTaskListNums=null;
				lock(_listSubscribedTaskListNums) {
					listTaskListNums=new List<long>(_listSubscribedTaskListNums);
				}
				return listTaskListNums;
			}
		}

		public UserControlTasks() {
			InitializeComponent();
			//this.listMain.ContextMenu = this.menuEdit;
			//Lan.F(this);
			Lan.C(this,menuEdit);
			gridMain.ContextMenu=menuEdit;
			_listInstances.Add(this);
		}

		///<summary>Destructor.  Removes this instance from the private list of instances.</summary>
		~UserControlTasks() {
			_listInstances.Remove(this);
		}

		private void UserControlTasks_Resize(object sender,EventArgs e) {
			//ToolBarMain is docked, h=25
			LayoutManager.Move(tabContr,new Rectangle(0,LayoutManager.Scale(27),Width,LayoutManager.Scale(23)));
			LayoutManager.Move(monthCalendar,new Rectangle(0,LayoutManager.Scale(52),LayoutManager.Scale(227),LayoutManager.Scale(162)));
			LayoutManager.MoveWidth(tree,Width);
			LayoutManager.MoveWidth(gridMain,Width);
			FillGrid(new List<Signalod>());//Sets height.  Does not run query.
		}

		///<summary>Calls RefreshTasks for all known instances of UserControlTasks for each instance which is visible and not disposed.</summary>
		public static void RefreshTasksForAllInstances(List<Signalod> listSignals,UserControlTasksTab tabToRefresh=UserControlTasksTab.Invalid) {
			foreach(UserControlTasks control in _listInstances) {
				if(!control.Visible || control.IsDisposed) {
					continue;
				}
				if(tabToRefresh!=UserControlTasksTab.Invalid && control.TaskTab!=tabToRefresh) {
					continue;
				}
				Logger.LogAction("UserControlTasks.RefreshTasks",LogPath.Signals,() => control.FillGrid(listSignals));
			}
		}

		///<summary>Resets the currently applied filter to either the preference TasksGlobalFilterType, or the selected TaskList's override, for all 
		///instances of UserControlTasks.</summary>
		public static void ResetGlobalTaskFilterTypesToDefaultAllInstances() {
			foreach(UserControlTasks control in _listInstances) {
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
			tabUser.Text=Lan.g(this,"for ")+Security.CurUser.UserName;
			tabNew.Text=Lan.g(this,"New for ")+Security.CurUser.UserName;
			if(PrefC.GetBool(PrefName.TasksShowOpenTickets)) {
				if(!tabContr.TabPages.Contains(tabOpenTickets)) {
					LayoutManager.Add(tabOpenTickets,tabContr);
					tabContr.TabPages.Remove(tabOpenTickets);
					tabContr.TabPages.Insert(2,tabOpenTickets);
				}
			}
			else{
				if(tabContr.TabPages.Contains(tabOpenTickets)) {
					LayoutManager.Remove(tabOpenTickets);
				}
			}
			LayoutToolBar();
			if(PrefC.GetBool(PrefName.TasksUseRepeating)) {
				if(!tabContr.TabPages.Contains(tabRepeating)) {
					LayoutManager.Add(tabRepeating,tabContr);
					LayoutManager.Add(tabDate,tabContr);
					LayoutManager.Add(tabWeek,tabContr);
					LayoutManager.Add(tabMonth,tabContr);
				}
				if(tabContr.TabPages.Contains(tabReminders)) {
					LayoutManager.Remove(tabReminders);
				}
			}
			else {//Repeating tasks disabled.
				if(tabContr.TabPages.Contains(tabRepeating)) {
					LayoutManager.Remove(tabRepeating);
					LayoutManager.Remove(tabDate);
					LayoutManager.Remove(tabWeek);
					LayoutManager.Remove(tabMonth);
				}
				if(!tabContr.TabPages.Contains(tabReminders)) {
					LayoutManager.Add(tabReminders,tabContr);
				}
			}
			if(Tasks.LastOpenList==null) {//first time openning
				_listTaskListTreeHistory=new List<TaskList>();
				monthCalendar.SelectionStart=DateTime.Today;
			}
			else {//reopening
				if(Tasks.LastOpenGroup >= tabContr.TabPages.Count) {
					//This happens if the user changes the TasksUseRepeating from true to false, then refreshes the tasks.
					Tasks.LastOpenGroup=0;
				}
				tabContr.SelectedIndex=Tasks.LastOpenGroup;
				_listTaskListTreeHistory=new List<TaskList>();
				//for(int i=0;i<Tasks.LastOpenList.Count;i++) {
				//	TreeHistory.Add(((TaskList)Tasks.LastOpenList[i]).Copy());
				//}
				monthCalendar.SelectionStart=Tasks.LastOpenDate;
			}
			if(PrefC.IsODHQ) {
				menuNavJob.Visible=true;
				menuNavJob.Enabled=false;
				if(Security.IsAuthorized(Permissions.TaskEdit,true)) {
					menuDeleteTaken.Visible=true;
				}
			}
			_isTaskSortApptDateTime=PrefC.GetBool(PrefName.TaskSortApptDateTime);//This sets it for use and also for the task options default value.
			List<UserOdPref> listPrefsForCollapsing=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse);
			_isCollapsedByDefault=listPrefsForCollapsing.Count==0 ? false : PIn.Bool(listPrefsForCollapsing[0].ValueString);
			_hasListSwitched=true;
			_taskCollapsedState=_isCollapsedByDefault ? 1 : 0;
			SetFiltersToDefault();//Fills Tree and Grid
			if(tabContr.SelectedTab!=tabOpenTickets) {//because it will have alread been set
				SetOpenTicketTab(-1);
			}
			SetPatientTicketTab(-1);
			SetMenusEnabled();
		}

		public UserControlTasksTab TaskTab {
			get {
				if(tabContr.SelectedTab==tabUser) {
					return UserControlTasksTab.ForUser;
				}
				else if(tabContr.SelectedTab==tabNew) {
					return UserControlTasksTab.UserNew;
				}
				else if(tabContr.SelectedTab==tabOpenTickets) {
					return UserControlTasksTab.OpenTickets;
				}
				else if(tabContr.SelectedTab==tabPatientTickets) {
					return UserControlTasksTab.PatientTickets;
				}
				else if(tabContr.SelectedTab==tabMain) {
					return UserControlTasksTab.Main;
				}
				else if(tabContr.SelectedTab==tabReminders) {
					return UserControlTasksTab.Reminders;
				}
				else if(tabContr.SelectedTab==tabRepeating) {
					return UserControlTasksTab.RepeatingSetup;
				}
				else if(tabContr.SelectedTab==tabDate) {
					return UserControlTasksTab.RepeatingByDate;
				}
				else if(tabContr.SelectedTab==tabWeek) {
					return UserControlTasksTab.RepeatingByWeek;
				}
				else if(tabContr.SelectedTab==tabMonth) {
					return UserControlTasksTab.RepeatingByMonth;
				}
				return UserControlTasksTab.Invalid;//Default.  Should not happen.
			}
			set {
				TabPage tabOld=tabContr.SelectedTab;
				if(value==UserControlTasksTab.ForUser) {
					tabContr.SelectedTab=tabUser;
				}
				else if(value==UserControlTasksTab.UserNew) {
					tabContr.SelectedTab=tabNew;
				}
				else if(value==UserControlTasksTab.OpenTickets && PrefC.GetBool(PrefName.TasksShowOpenTickets)) {
					tabContr.SelectedTab=tabOpenTickets;
				}
				else if(value==UserControlTasksTab.Main) {
					tabContr.SelectedTab=tabMain;
				}
				else if(value==UserControlTasksTab.Reminders) {
					tabContr.SelectedTab=tabReminders;
				}
				else if(value==UserControlTasksTab.RepeatingSetup && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabContr.SelectedTab=tabRepeating;
				}
				else if(value==UserControlTasksTab.RepeatingByDate && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabContr.SelectedTab=tabDate;
				}
				else if(value==UserControlTasksTab.RepeatingByWeek && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabContr.SelectedTab=tabWeek;
				}
				else if(value==UserControlTasksTab.RepeatingByMonth && PrefC.GetBool(PrefName.TasksUseRepeating)) {
					tabContr.SelectedTab=tabMonth;
				}
				else if(value==UserControlTasksTab.PatientTickets) {
					tabContr.SelectedTab=tabPatientTickets;
				}
				else if(value==UserControlTasksTab.Invalid) {
					//Do nothing.
				}
				if(tabContr.SelectedTab!=tabOld) {//Tab changed, refresh the tree.
					_listTaskListTreeHistory=new List<TaskList>();//clear the tree no matter which tab selected.
					FillTree();
					FillGrid();
				}
			}
		}

		///<summary>Called whenever OpenTicket tab is refreshed to set the count at the top.  Also called from InitializeOnStartup.  In that case, we don't know what the count should be, so we pass in a -1.</summary>
		private void SetOpenTicketTab(int countSet) {
			if(!tabContr.TabPages.Contains(tabOpenTickets)) {
				return;
			}
			if(countSet==-1) {
				countSet=Tasks.GetCountOpenTickets(Security.CurUser.UserNum);
			}
			tabOpenTickets.Text=Lan.g(this,"Open Tasks")+" ("+countSet.ToString()+")";
		}

		///<summary>Called whenever PatientTickets tab is refreshed to set the count at the top.  Also called from InitializeOnStartup.  In that case, we don't know what the count should be, so we pass in a -1.</summary>
		private void SetPatientTicketTab(int countSet) {
			if(!tabContr.TabPages.Contains(tabPatientTickets)) {
				return;
			}
			if(countSet==-1 && FormOpenDental.CurPatNum!=0) {
				countSet=Tasks.GetCountPatientTickets(FormOpenDental.CurPatNum);
			}
			tabPatientTickets.Text=Lan.g(this,"Patient Tasks")+" ("+(countSet==-1?"0":countSet.ToString())+")";
		}

		public void ClearLogOff() {
			tabUser.Text="for";
			tabNew.Text="New for";
			_listTaskListTreeHistory=new List<TaskList>();
			FillTree();
			gridMain.ListGridRows.Clear();
			gridMain.Invalidate();
		}

		///<summary>Used by OD HQ.</summary>
		public void FillGridWithTriageList() {
			TaskList tlOne=TaskLists.GetOne(_TriageListNum);
			tabContr.SelectedTab=tabMain;
			if(_listTaskListTreeHistory==null) {
				_listTaskListTreeHistory=new List<TaskList>();
			}
			_listTaskListTreeHistory.Clear();
			_listTaskListTreeHistory.Add(tlOne);
			if(_listTaskLists==null) {
				_listTaskLists=new List<TaskList>();
			}
			_listTaskLists.Clear();
			if(_listTasks==null) {
				_listTasks=new List<Task>();
			}
			_listTasks.Clear();
			_listTasks=Tasks.RefreshChildren(_TriageListNum,false,monthCalendar.SelectionStart,Security.CurUser.UserNum,0,TaskType.All,_globalFilterType,_listFilterFkeys);
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
		}

		///<summary></summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ODToolBarButton butOptions=new ODToolBarButton();
			butOptions.Text=Lan.g(this,"Options");
			butOptions.ToolTipText=Lan.g(this,"Set session specific task options.");
			butOptions.Tag="Options";
			ToolBarMain.Buttons.Add(butOptions);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add Task List"),0,"","AddList"));
			ODToolBarButton butTask=new ODToolBarButton(Lan.g(this,"Add Task"),1,"","AddTask");
			butTask.Style=ODToolBarButtonStyle.DropDownButton;
			butTask.DropDownMenu=menuTask;
			ToolBarMain.Buttons.Add(butTask);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Search"),-1,"","Search"));
			ODToolBarButton button=new ODToolBarButton();
			button.Text=Lan.g(this,"Manage Blocks");
			button.ToolTipText=Lan.g(this,"Manage which task lists will have popups blocked even when subscribed.");
			button.Tag="BlockSubsc";
			button.Pushed=Security.CurUser.DefaultHidePopups;
			ToolBarMain.Buttons.Add(button);
			//Filtering only works if Clinics are enabled and preference turned on.
			if(PrefC.HasClinicsEnabled && (GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)!=GlobalTaskFilterType.Disabled) {
				string textBut=string.Empty;
				if(_globalFilterType==GlobalTaskFilterType.None) {
					textBut="Unfiltered";
				}
				else {
					textBut="Filtered by "+_globalFilterType.GetDescription();
				}
				_butFilter=new ODToolBarButton(textBut,-1,"Select filter.","Filter");
				_butFilter.Style=ODToolBarButtonStyle.DropDownButton;
				_butFilter.DropDownMenu=menuFilter;
				ToolBarMain.Buttons.Add(_butFilter);
			}
			ToolBarMain.Invalidate();
		}

		private void FillTree() {
			tree.Nodes.Clear();
			TreeNode node;
			//TreeNode lastNode=null;
			string nodedesc;
			for(int i=0;i<_listTaskListTreeHistory.Count;i++) {
				nodedesc=_listTaskListTreeHistory[i].Descript;
				if(tabContr.SelectedTab==tabUser) {
					nodedesc=_listTaskListTreeHistory[i].ParentDesc+nodedesc;
				}
				node=new TreeNode(nodedesc);
				node.Tag=_listTaskListTreeHistory[i].TaskListNum;
				if(tree.SelectedNode==null) {
					tree.Nodes.Add(node);
				}
				else {
					tree.SelectedNode.Nodes.Add(node);
				}
				tree.SelectedNode=node;
			}
			if(tree.SelectedNode!=null) {
				switch(_globalFilterType) {
					case GlobalTaskFilterType.Clinic:
						tree.SelectedNode.Text+="(filtering by [Clinic(s):"+string.Join(",",_listFilterFkeys.Select(x => Clinics.GetAbbr(x)))+"])";
						break;
					case GlobalTaskFilterType.Region:
						tree.SelectedNode.Text+="(filtering by [Region(s):"+string.Join(",",_listFilterFkeys.Select(x => Defs.GetName(DefCat.Regions,x)))+"])";
						break;
					case GlobalTaskFilterType.None:
					default:
						break;
				}
			}
			//remember this position for the next time we open tasks
			Tasks.LastOpenList=new ArrayList();
			for(int i=0;i<_listTaskListTreeHistory.Count;i++) {
				Tasks.LastOpenList.Add(_listTaskListTreeHistory[i].Copy());
			}
			Tasks.LastOpenGroup=tabContr.SelectedIndex;
			Tasks.LastOpenDate=monthCalendar.SelectionStart;
			//layout
			if(tabContr.SelectedTab==tabDate || tabContr.SelectedTab==tabWeek || tabContr.SelectedTab==tabMonth) {
				LayoutManager.MoveLocation(tree,new Point(tree.Left,monthCalendar.Bottom+1));//Show the calendar.
			}
			else {
				LayoutManager.MoveLocation(tree,new Point(tree.Left,tabContr.Bottom));//Hide the calendar.
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

		///<summary>Sets the classwide filter variables to their default values.</summary>
		private void SetFiltersToDefault(TaskList taskListSelected=null) {
			GlobalTaskFilterType globalFilterType=GlobalTaskFilterType.Default;
			if(taskListSelected==null && tree.SelectedNode!=null) {//Check if there is a current tasklist selected.
				taskListSelected=_listTaskListTreeHistory.FirstOrDefault(x => x.TaskListNum==(long)tree.SelectedNode.Tag);
			}
			if(taskListSelected!=null) {//And if so, use its GlobalTaskFilterType override.
				globalFilterType=taskListSelected.GlobalTaskFilterType;
			}
			if(Clinics.ClinicNum==0) {//HQ clinic, do not apply filtering.
				globalFilterType=GlobalTaskFilterType.None;
			}
			if(globalFilterType==GlobalTaskFilterType.Default) {//Get the default filter setting.
				globalFilterType=(GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType);
			}
			//At this point, we have the GlobalFilterType to use.  Make sure it's a valid choice.
			globalFilterType=DowngradeFilterTypeIfNeeded(globalFilterType);
			switch(globalFilterType) {
				case GlobalTaskFilterType.None:
					SetFilters(GlobalTaskFilterType.None,new List<long> { 0 });
					break;
				case GlobalTaskFilterType.Clinic:
					SetFilters(GlobalTaskFilterType.Clinic,new List<long> { Clinics.ClinicNum });//Default to currently selected clinic.
					break;
				case GlobalTaskFilterType.Region:
					//Default to currently selected clinic's region.  Use 0 if no region defined.
					SetFilters(GlobalTaskFilterType.Region,new List<long> { Clinics.GetClinic(Clinics.ClinicNum)?.Region??0 });
					break;
				case GlobalTaskFilterType.Disabled:
					FillTree();
					FillGrid();
					break;
			}
		}

		///<summary>Determines if globalFilterType should be downgraded based on Clinics being enabled/disabled and Region definitions.</summary>
		private GlobalTaskFilterType DowngradeFilterTypeIfNeeded(GlobalTaskFilterType globalFilterType) {
			if(!PrefC.HasClinicsEnabled) {//Downgrade to None if Clinics are disabled.
				globalFilterType=GlobalTaskFilterType.None;
			}
			else if(globalFilterType==GlobalTaskFilterType.Region && Defs.GetDefsForCategory(DefCat.Regions).Count==0) {
				globalFilterType=GlobalTaskFilterType.None;//Downgrade to None if Region selected but no Regions defined.
			}
			return globalFilterType;
		}

		private void SetFilters(GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys) {
			bool isChangingFilterType=(_globalFilterType!=globalFilterType);
			_globalFilterType=globalFilterType;
			_listFilterFkeys=listFilterFkeys;
			if(isChangingFilterType && _butFilter!=null) {
				if(_globalFilterType==GlobalTaskFilterType.None) {
					_butFilter.Text="Unfiltered";
				}
				else {
					_butFilter.Text="Filtered by "+_globalFilterType.GetDescription();
				}
				ToolBarMain.Invalidate();//Redraw immediately.
			}
			FillTree();
			FillGrid();
		}
		
		///<summary>Allows the user to reset filtering to the default setting for the current Tasklist.</summary>
		private void menuItemFilterDefault_Click(object sender,EventArgs e) {
			SetFiltersToDefault();
		}

		///<summary>Allows the user to temporarily turn of Filtering.</summary>
		private void menuItemFilterNone_Click(object sender,EventArgs e) {
			SetFilters(GlobalTaskFilterType.None,new List<long>{ 0 });//Fills Tree and Grid
		}

		///<summary>Allows the user to temporarily select a different Clinic to filter by.  Only allows user choices from unrestricted Clinics.</summary>
		private void menuItemFilterClinic_Click(object sender,EventArgs e) {
			List<GridColumn> listGridCols=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Abbr"),70),
				new GridColumn(Lan.g(this,"Description"),100,HorizontalAlignment.Left){ IsWidthDynamic=true }
			};
			List<GridRow> listGridRows=new List<GridRow>();
			Clinics.GetAllForUserod(Security.CurUser).ForEach(x => {//Only Clinics the user has access to.
				GridRow row=new GridRow(x.Abbr,x.Description);
				row.Tag=x.ClinicNum;
				listGridRows.Add(row);
			});
			using FormGridSelection formSelect=new FormGridSelection(listGridCols,listGridRows,"Select a Clinic","Clinics",GridSelectionMode.MultiExtended);
			if(formSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			SetFilters(GlobalTaskFilterType.Clinic,formSelect.ListSelectedTags.Select(x => (long)x).ToList());//Fills Tree and Grid
		}

		///<summary>Allows the user to temporarily select a different Region to filter by.  Only allows user choices from Regions associated to 
		///unrestricted clinics.</summary>
		private void menuItemFilterRegion_Click(object sender,EventArgs e) {
			List<GridColumn> listGridCols=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Name"),70),
			};
			List<GridRow> listGridRows=new List<GridRow>();
			//Regions associated to clinics that the user has access to (unrestricted).
			List<long> listRegionDefNums=Clinics.GetAllForUserod(Security.CurUser).FindAll(x => x.Region!=0).Select(x => x.Region).ToList();
			listRegionDefNums.Distinct().ForEach(x => {
				Def regionDef=Defs.GetDef(DefCat.Regions,x);
				GridRow row=new GridRow(regionDef.ItemName);
				row.Tag=regionDef.DefNum;
				listGridRows.Add(row);
			});
			using FormGridSelection formSelect=new FormGridSelection(listGridCols,listGridRows,"Select a Region","Regions",GridSelectionMode.MultiExtended);
			if(formSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			SetFilters(GlobalTaskFilterType.Region,formSelect.ListSelectedTags.Select(x => (long)x).ToList());//Fills Tree and Grid
		}

		///<summary>Causes all instances of UserControlTasks to replace/remove the passed in Task and TaskNotes from the list of currently displayed 
		///Tasks, then, if necessary, refills the grid without querying the database for the Task or TaskNotes. Adds signalNums for signals associated
		///to the refreshes occurring in this method to the list of signals that have been sent, so FillGrid can ignore them if the refresh has already
		///occurred locally.
		///To remove task from grid in all instances, pass in canKeepTask=true.</summary>
		public static void RefillLocalTaskGrids(Task task,List<TaskNote> listTaskNotes,List<long> listSentSignalNums,bool canKeepTask=true) {
			DataValid.SetInvalid(InvalidType.Task);//Fires plugin hook, refreshes Chart module if visible.
			UserControlTasks.AddSentSignalNums(listSentSignalNums);
			foreach(UserControlTasks control in _listInstances) {
				if(!control.Visible && !control.IsDisposed) {//Verify control is visible and active
					continue;
				}
				long parent=0;//Default to one of the main trunks.
				if(control._listTaskListTreeHistory!=null && control._listTaskListTreeHistory.Count>0) {//not on main trunk
					parent=control._listTaskListTreeHistory[control._listTaskListTreeHistory.Count-1].TaskListNum;
				}
				if(task==null) {
					//Just FillGrid.
				}
				else if(task.TaskStatus==TaskStatusEnum.Done 
					&& (!control._isShowFinishedTasks || (control._isShowFinishedTasks && control.tabContr.SelectedTab==control.tabNew))) {
					//Task is Done, and option to Show Finished Tasks is off, or Done and Show Finished Tasks is on and on New for User tab.
					control._listTasks.RemoveAll(x => x.TaskNum==task.TaskNum);
					control._listTaskNotes.RemoveAll(x => x.TaskNum==task.TaskNum);//Remove corresponding taskNotes.
				}
				else if(canKeepTask && (task.TaskListNum==parent//Task is in the currently displayed TaskList.
					|| (control.tabContr.SelectedTab==control.tabNew && control.IsInNewTab(task))//Task should display in 'New for User' tab.
					|| (control.tabContr.SelectedTab==control.tabOpenTickets && task.UserNum==Security.CurUser.UserNum 
						&& task.ObjectType==TaskObjectType.Patient)//Open Tab
					|| (control.tabContr.SelectedTab==control.tabPatientTickets && task.KeyNum==FormOpenDental.CurPatNum)))//Patient tab
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
				control.FullRefreshIfNeeded(parent);
			}
		}

		///<summary>Causes all instances of UserControlTasks to replace/remove the passed in TaskList from the list of currently displayed 
		///TaskLists, then, if necessary, refills the grid without querying the database for the TaskList. Adds signalNums for signals associated
		///to the refreshes occurring in this method to the list of signals that have been sent, so FillGrid can ignore them if the refresh has already
		///occurred locally.
		///To remove taskList from grid in all instances, pass in canKeepTask=true.</summary>
		public static void RefillLocalTaskGrids(TaskList taskList,List<long> listSentSignalNums,bool canKeepTaskList=true) {
			AddSentSignalNums(listSentSignalNums);
			List <long> listSubscribedTaskListNums=ListSubscribedTaskListNums;//Get list copy above loop to avoid making unnecessary copies.
			foreach(UserControlTasks control in _listInstances) {
				long parent=0;//Default to one of the main trunks.
				if(control._listTaskListTreeHistory!=null && control._listTaskListTreeHistory.Count>0) {//not on main trunk
					parent=control._listTaskListTreeHistory[control._listTaskListTreeHistory.Count-1].TaskListNum;
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
				else if(control.tabContr.SelectedTab==control.tabUser && !listSubscribedTaskListNums.Contains(taskList.TaskListNum)) {
					control._listTaskLists.RemoveAll(x => x.TaskListNum==taskList.TaskListNum);
				}
				else if(canKeepTaskList 
					//not on 'New for User' and taskList is in the currently displayed TaskList ('New for User' only shows Tasks)
					&& ((control.tabContr.SelectedTab!=control.tabNew && taskList.Parent==parent)
						//On 'for User' tab and taskList is a subscribed TaskList
						|| (control.tabContr.SelectedTab==control.tabUser && listSubscribedTaskListNums.Contains(taskList.TaskListNum)))) 
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
				control.FullRefreshIfNeeded(parent);
			}
		}

		private void FullRefreshIfNeeded(long parent) {
			if(parent!=0 || tabContr.SelectedTab==tabNew) {//Not a trunk, or on the New for User tab. 
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
			foreach(UserControlTasks control in _listInstances) {
				control._listSentTaskSignalNums.AddRange(listSignalNums);
			}
		}

		///<summary>Removes any matching Signalod.SignalNums from this instance's list of sent Task/TaskList related signalnums.</summary>
		private List<Signalod> RemoveSentSignalNums(List<Signalod> listReceivedSignals) {
			if(listReceivedSignals==null || listReceivedSignals.Count==0) {
				return new List<Signalod>();
			}
			for(int i=listReceivedSignals.Count-1;i>=0;i--) {
				long receivedSignalNum=listReceivedSignals[i].SignalNum;
				if(ListTools.In(receivedSignalNum,_listSentTaskSignalNums)) {
					_listSentTaskSignalNums.Remove(receivedSignalNum);
					listReceivedSignals.RemoveAt(i);
				}
			}
			return listReceivedSignals;
		}

		///<summary>Determines if Task should display in the 'New for' tab.  If using TasksNewTrackedByUser preference, Task.IsUnread must be correctly 
		///set prior to calling this method.</summary>
		private bool IsInNewTab(Task task) {
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser) && task.IsUnread) {//The new way
				if(!ListSubscribedTaskListNums.Contains(task.TaskListNum)) {
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
		private void FillGrid(List<Signalod> listSignals=null){
			if(Security.CurUser==null 
				|| (RemotingClient.RemotingRole==RemotingRole.ClientWeb && !Security.IsUserLoggedIn)) 
			{
				gridMain.BeginUpdate();
				gridMain.ListGridRows.Clear();
				gridMain.EndUpdate();
				return;
			}
			long parent;
			DateTime date;
			if(_listTaskListTreeHistory==null){
				return;
			}
			if(_listTaskListTreeHistory.Count>0) {//not on main trunk
				parent=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
				date=DateTime.MinValue;
			}
			else {//one of the main trunks
				parent=0;
				date=monthCalendar.SelectionStart;
			}
			LayoutManager.MoveHeight(gridMain,ClientSize.Height-gridMain.Top);
			if(listSignals==null) {//Full refresh.
				RefreshMainLists(parent,date);
			}
			else {
				//Remove any Task related signals that originated from this instance of OpenDental.
				listSignals=RemoveSentSignalNums(listSignals.FindAll(x => ListTools.In(x.IType,new List<InvalidType>()
					{ InvalidType.Task,InvalidType.TaskList,InvalidType.TaskAuthor,InvalidType.TaskPatient })));
				//User is observing a task list for which a TaskList signal is specified, or TaskList from signal is a sublist of current view.
				if(listSignals.Exists(x => x.IType==InvalidType.TaskList && (x.FKey==parent || _listTaskLists.Exists(y => y.TaskListNum==x.FKey)))) {
					RefreshMainLists(parent,date);
				}
				//User is observing the New Tasks tab and a TaskList signal is received for a TaskList the user is subscribed to.
				else if(tabContr.SelectedTab==tabNew && listSignals.Exists(x => x.IType==InvalidType.TaskList && ListSubscribedTaskListNums.Contains(x.FKey))) {
					RefreshMainLists(parent,date);
				}
				//User is observing the Open Tasks tab and a TaskAuthor signal is received with the current user specified in the FKey.
				else if(tabContr.SelectedTab==tabOpenTickets && listSignals.Exists(x => x.IType==InvalidType.TaskAuthor && x.FKey==Security.CurUser.UserNum)) {
					RefreshMainLists(parent,date);
				}
				//User is observing the Patient Tasks tab and a TaskPatient signal is received for the patient the user currently has selected.
				else if(tabContr.SelectedTab==tabPatientTickets && listSignals.Exists(x => x.IType==InvalidType.TaskPatient && x.FKey==FormOpenDental.CurPatNum)) {
					RefreshMainLists(parent,date);
				}
				else {//Individual Task signals. Only refreshes if the task is in the currently displayed list of Tasks. Add/Remove is addressed with TaskList signals.
					foreach(Signalod signal in listSignals) {
						if(ListTools.In(signal.IType,InvalidType.Task,InvalidType.TaskPopup) && signal.FKeyType==KeyType.Task) {
							if(_listTasks.Exists(x => x.TaskNum==signal.FKey)) {//A signal indicates that a task we are looking at has been modified.
								RefreshMainLists(parent,date);//Full refresh.
								break;
							}
						}
					}
				}
			}
			#region dated trunk automation
			//dated trunk automation-----------------------------------------------------------------------------
			if(_listTaskListTreeHistory.Count==0//main trunk
				&& (tabContr.SelectedTab==tabDate || tabContr.SelectedTab==tabWeek || tabContr.SelectedTab==tabMonth))
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
						SecurityLogs.MakeLogEntry(Permissions.TaskEdit,0,"Task "+POut.Long(_listTasks[i].TaskNum)+" deleted",0);
						changeMade=true;
					}
				}
				if(changeMade) {
					RefreshMainLists(parent,date);
				}
				//now add back any repeating lists and tasks that meet the criteria
				//Get lists of all repeating lists and tasks of one type.  We will pick items from these two lists.
				List<TaskList> repeatingLists=new List<TaskList>();
				List<Task> repeatingTasks=new List<Task>();
				if(tabContr.SelectedTab==tabDate){
					repeatingLists=TaskLists.RefreshRepeating(TaskDateType.Day,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					repeatingTasks=Tasks.RefreshRepeating(TaskDateType.Day,Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
				}
				if(tabContr.SelectedTab==tabWeek){
					repeatingLists=TaskLists.RefreshRepeating(TaskDateType.Week,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					repeatingTasks=Tasks.RefreshRepeating(TaskDateType.Week,Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
				}
				if(tabContr.SelectedTab==tabMonth) {
					repeatingLists=TaskLists.RefreshRepeating(TaskDateType.Month,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
					repeatingTasks=Tasks.RefreshRepeating(TaskDateType.Month,Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
				}
				//loop through list and add back any that meet criteria.
				changeMade=false;
				bool alreadyExists;
				for(int i=0;i<repeatingLists.Count;i++) {
					//if already exists, skip
					alreadyExists=false;
					for(int j=0;j<_listTaskLists.Count;j++) {//loop through Main list
						if(_listTaskLists[j].FromNum==repeatingLists[i].TaskListNum) {
							alreadyExists=true;
							break;
						}
					}
					if(alreadyExists) {
						continue;
					}
					//otherwise, duplicate the list
					repeatingLists[i].DateTL=date;
					repeatingLists[i].FromNum=repeatingLists[i].TaskListNum;
					repeatingLists[i].IsRepeating=false;
					repeatingLists[i].Parent=0;
					repeatingLists[i].ObjectType=0;//user will have to set explicitly
					DuplicateExistingList(repeatingLists[i],true);//repeating lists cannot be subscribed to, so send null in as old list, will not attempt to move subscriptions
					changeMade=true;
				}
				for(int i=0;i<repeatingTasks.Count;i++) {
					//if already exists, skip
					alreadyExists=false;
					for(int j=0;j<_listTasks.Count;j++) {//loop through Main list
						if(_listTasks[j].FromNum==repeatingTasks[i].TaskNum) {
							alreadyExists=true;
							break;
						}
					}
					if(alreadyExists) {
						continue;
					}
					//otherwise, duplicate the task
					repeatingTasks[i].DateTask=date;
					repeatingTasks[i].FromNum=repeatingTasks[i].TaskNum;
					repeatingTasks[i].IsRepeating=false;
					repeatingTasks[i].TaskListNum=0;
					//repeatingTasks[i].UserNum//repeating tasks shouldn't get a usernum
					Tasks.Insert(repeatingTasks[i]);
					changeMade=true;
				}
				if(changeMade) {
					RefreshMainLists(parent,date);
				}
			}//End of dated trunk automation--------------------------------------------------------------------------
			#endregion dated trunk automation
			//bool isTaskSelectedVisible=gridMain.IsTagVisible(_clickedTask);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",17);
			col.ImageList=imageListTree;
			gridMain.ListGridColumns.Add(col);//Checkbox column
			if(tabContr.SelectedTab==tabNew && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//The old way
				col=new GridColumn(Lan.g("TableTasks","Read"),35,HorizontalAlignment.Center);
				//col.ImageList=imageListTree;
				gridMain.ListGridColumns.Add(col);
			}
			if(tabContr.SelectedTab==tabNew || tabContr.SelectedTab==tabOpenTickets || tabContr.SelectedTab==tabPatientTickets) {
				col=new GridColumn(Lan.g("TableTasks","Task List"),90);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"+/-"),17,HorizontalAlignment.Center);
			col.CustomClickEvent+=GridHeaderClickEvent;
			gridMain.ListGridColumns.Add(col);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)){//HQ
				col=new GridColumn(Lan.g("TableTasks","ST"),30,HorizontalAlignment.Center);//ST
				gridMain.ListGridColumns.Add(col);
				List<long> listPatsNotInDict=_listTasks.Where(x => x.ObjectType==TaskObjectType.Patient && x.KeyNum!=0 && !_dictPatStates.ContainsKey(x.KeyNum))
					.Select(x => x.KeyNum).ToList();
				Dictionary<long,string> dictPatNewStates=Patients.GetStatesForPats(listPatsNotInDict);
				foreach(long patNum in dictPatNewStates.Keys) {
					_dictPatStates.Add(patNum,dictPatNewStates[patNum]);
				}
				if(parent!=_TriageListNum) {//Everything that's not triage
					col=new GridColumn(Lan.g("TableTasks","Job"),30,HorizontalAlignment.Center);//Job
					gridMain.ListGridColumns.Add(col);
				}
			}
			col=new GridColumn(Lan.g("TableTasks","Description"),200);//any width
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string dateStr="";
			string objDesc="";
			string tasklistdescript="";
			string notes="";
			//These strings are always inserted into cells, so they are always set to "" if there no job
			string jobNumString="";
			int imageindex;
			for(int i=0;i<_listTaskLists.Count;i++) {
				dateStr="";
				if(_listTaskLists[i].DateTL.Year>1880
					&& (tabContr.SelectedTab==tabUser || tabContr.SelectedTab==tabMain || tabContr.SelectedTab==tabReminders))
				{
					if(_listTaskLists[i].DateType==TaskDateType.Day) {
						dateStr=_listTaskLists[i].DateTL.ToShortDateString()+" - ";
					}
					else if(_listTaskLists[i].DateType==TaskDateType.Week) {
						dateStr=Lan.g(this,"Week of")+" "+_listTaskLists[i].DateTL.ToShortDateString()+" - ";
					}
					else if(_listTaskLists[i].DateType==TaskDateType.Month) {
						dateStr=_listTaskLists[i].DateTL.ToString("MMMM")+" - ";
					}
				}
				objDesc="";
				if(tabContr.SelectedTab==tabUser){
					objDesc=_listTaskLists[i].ParentDesc;
				}
				tasklistdescript=_listTaskLists[i].Descript;
				imageindex=0;
				if(_listTaskLists[i].NewTaskCount>0){
					imageindex=3;//orange
					tasklistdescript=tasklistdescript+"("+_listTaskLists[i].NewTaskCount.ToString()+")";
				}
				row=new GridRow();
				row.Cells.Add(imageindex.ToString());
				row.Cells.Add("");
				if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ.  Add if job manager is available
					row.Cells.Add("");//ST
					if(parent!=_TriageListNum) {//Everything that's not triage
						row.Cells.Add("");//Job
					}
				}
				row.Cells.Add(dateStr+objDesc+tasklistdescript);
				row.Tag=_listTaskLists[i];
				gridMain.ListGridRows.Add(row);
			}
			List<long> listAptNums=_listTasks.Where(x => x.ObjectType==TaskObjectType.Appointment).Select(y => y.KeyNum).ToList();
			SerializableDictionary<long,string> dictApptObjDescripts=Tasks.GetApptObjDescripts(listAptNums);
			int selectedTaskIndex=-1;
			for(int i=0;i<_listTasks.Count;i++) {
				dateStr="";
				jobNumString="";
				string stateString="";
				if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
					stateString=HQStateColumn(_listTasks[i]);//ST
					if(parent!=_TriageListNum) {//Everything that's not triage
						//get list of jobs attached to task then insert info about those jobs.
						List<long> listJobNums=_listJobLinks.Where(x => x.FKey==_listTasks[i].TaskNum).Select(x => x.JobNum).ToList();
						if(_listJobs.Any(x => ListTools.In(x.JobNum,listJobNums))) {
							jobNumString="X";//Is job
						}
					}
				}
				if(tabContr.SelectedTab==tabUser || tabContr.SelectedTab==tabNew
					|| tabContr.SelectedTab==tabOpenTickets || tabContr.SelectedTab==tabMain 
					|| tabContr.SelectedTab==tabReminders	|| tabContr.SelectedTab==tabPatientTickets) 
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
				List<TaskNote> listNotesForTask=_listTaskNotes.FindAll(x => x.TaskNum==_listTasks[i].TaskNum);
				if(!_listExpandedTaskNums.Contains(_listTasks[i].TaskNum) && listNotesForTask.Count>1) {
					TaskNote lastNote=listNotesForTask[listNotesForTask.Count-1];
					notes+="\r\n\u22EE\r\n" //Vertical ellipse followed by last note. \u22EE - vertical ellipses
							+"=="+Userods.GetName(lastNote.UserNum)+" - "
							+lastNote.DateTimeNote.ToShortDateString()+" "
							+lastNote.DateTimeNote.ToShortTimeString()
							+" - "+lastNote.Note;
				}
				else { //Expanded
					foreach(TaskNote note in listNotesForTask) {
						notes+="\r\n"//even on the first loop
							+"=="+Userods.GetName(note.UserNum)+" - "
							+note.DateTimeNote.ToShortDateString()+" "
							+note.DateTimeNote.ToShortTimeString()
							+" - "+note.Note;
					}
				}
				row=new GridRow();
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//The new way
					if(_listTasks[i].TaskStatus==TaskStatusEnum.Done) {
						row.Cells.Add("1");
					}
					else {
						if(_listTasks[i].IsUnread) {
							row.Cells.Add("4");
						}
						else{
							row.Cells.Add("2");
						}
					}
				}
				else {
					switch(_listTasks[i].TaskStatus) {
						case TaskStatusEnum.New:
							row.Cells.Add("4");
							break;
						case TaskStatusEnum.Viewed:
							row.Cells.Add("2");
							break;
						case TaskStatusEnum.Done:
							row.Cells.Add("1");
							break;
					}
					if(tabContr.SelectedTab==tabNew) {//In this mode, there's a extra column in this tab
						row.Cells.Add("read");
					}
				}
				if(tabContr.SelectedTab==tabNew || tabContr.SelectedTab==tabOpenTickets || tabContr.SelectedTab==tabPatientTickets) {
					row.Cells.Add(_listTasks[i].ParentDesc);
				}
				if(_listTasks[i].DescriptOverride!=""){
					row.Cells.Add("");// +/- is irrelevant
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
						row.Cells.Add(stateString);//ST
						if(parent!=_TriageListNum) {//Everything that's not triage
							row.Cells.Add(jobNumString);//Job
						}
					}
					row.Cells.Add(dateStr+objDesc+_listTasks[i].DescriptOverride);
				}
				else if(_listExpandedTaskNums.Contains(_listTasks[i].TaskNum)) {//Expanded
					if(_listTasks[i].Descript.Length>250 || listNotesForTask.Count>1 || (listNotesForTask.Count==1 && notes.Length>250)) {
						row.Cells.Add("-");
					}
					else {
						row.Cells.Add("");
					}
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
						row.Cells.Add(stateString);//ST
						if(parent!=_TriageListNum) {//Everything that's not triage
							row.Cells.Add(jobNumString);//Job
						}
					}
					row.Cells.Add(dateStr+objDesc+_listTasks[i].Descript+notes);
				}
				else {//not expanded
					//Conditions for giving collapse option: Descript is long, there is more than one note, or there is one note and it's long.
					if(_listTasks[i].Descript.Length>250 || listNotesForTask.Count>1 || (listNotesForTask.Count==1 && notes.Length>250)) {
						row.Cells.Add("+");
						if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
							row.Cells.Add(stateString);//ST
							if(parent!=_TriageListNum) {//Everything that's not triage
								row.Cells.Add(jobNumString);//Job
							}
						}
						string rowString=dateStr+objDesc;
						if(_listTasks[i].Descript.Length>250) {
							rowString+=_listTasks[i].Descript.Substring(0,250)+"(...)";//546,300 tasks have average Descript length of 142.1 characters.
						}
						else {
							rowString+=_listTasks[i].Descript;
						}
						if(notes.Length>250) {
							rowString+=notes.Substring(0,250)+"(...)";
						}
						else {
							rowString+=notes;
						}
						row.Cells.Add(rowString);
					}
					else {//Descript length <= 250 and notes <=1 and note length is <= 250.  No collapse option.
						row.Cells.Add("");
						if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//HQ
							row.Cells.Add(stateString);//ST
							if(parent!=_TriageListNum) {//Everything that's not triage
								row.Cells.Add(jobNumString);//Job
							}
						}
						row.Cells.Add(dateStr+objDesc+_listTasks[i].Descript+notes);
					}
				}
				row.ColorBackG=Defs.GetColor(DefCat.TaskPriorities,_listTasks[i].PriorityDefNum);//No need to do any text detection for triage priorities, we'll just use the task priority colors.
				row.Tag=_listTasks[i];
				gridMain.ListGridRows.Add(row);
				if(_clickedTask is Task && _listTasks[i].TaskNum==_clickedTask.TaskNum) {//_clickedTask can be a TaskList
					selectedTaskIndex=gridMain.ListGridRows.Count-1;
				}
			}
			gridMain.EndUpdate();
			//if(isTaskSelectedVisible) {//Only scroll the previously selected task (now reselected task) into view if it was previously visible.
				//gridMain.ScrollToIndex(selectedTaskIndex); For now, this is confusing techs, revisit later.
			//}
			gridMain.SetSelected(selectedTaskIndex,true);
			//Without this 'scroll value reset', drilling down into a tasklist that contains tasks will sometimes result in an empty grid, until the user 
			//interacts with the grid, example, scrolling will cause the grid to repaint and properly display the expected tasks.
			gridMain.ScrollValue=gridMain.ScrollValue;//this forces scroll value to reset if it's > allowed max.
			if(tabContr.SelectedTab==tabOpenTickets) {
				SetOpenTicketTab(gridMain.ListGridRows.Count);
			}
			if(tabContr.SelectedTab==tabPatientTickets) {
				SetPatientTicketTab(gridMain.ListGridRows.Count);
			}
			else {
				SetPatientTicketTab(-1);
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
			string taskListDescript="";
			if(tabContr.SelectedTab==tabNew) {//Special case tab. All grid rows are guaranteed to be task so we manually set values.
				taskListDescript=Lan.g(this,"New for")+" "+Security.CurUser.UserName;
			}
			else if(_listTaskListTreeHistory.Count>0){//Not in main trunk
				taskListDescript=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].Descript;
			}
			if(taskListDescript=="") {//Should only happen when at main trunk.
				ControlParentTitle=Lan.g(this,"Tasks");
			}
			else {
				int tasksNewCount=_listTaskLists.Sum(x => x.NewTaskCount);
				tasksNewCount+=_listTasks.Sum(x => x.TaskStatus==TaskStatusEnum.New?1:0);
				ControlParentTitle=Lan.g(this,"Tasks")+" - "+taskListDescript+" ("+tasksNewCount.ToString()+")";
			}
			FillGridEvent.Invoke(this,new EventArgs());
		}

		///<summary>A recursive function that checks every child in a list IsFromRepeating.  If any are marked complete, then it returns true, signifying that this list should be immune from being deleted since it's already in use.</summary>
		private bool AnyAreMarkedComplete(TaskList list) {
			//get all children:
			List<TaskList> childLists=TaskLists.RefreshChildren(list.TaskListNum,Security.CurUser.UserNum,0,TaskType.Normal);
			List<Task> childTasks=Tasks.RefreshChildren(list.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.Normal);
			for(int i=0;i<childLists.Count;i++) {
				if(AnyAreMarkedComplete(childLists[i])) {
					return true;
				}
			}
			for(int i=0;i<childTasks.Count;i++) {
				if(childTasks[i].TaskStatus==TaskStatusEnum.Done) {
					return true;
				}
			}
			return false;
		}

		///<summary>If parent=0, then this is a trunk.</summary>
		private void RefreshMainLists(long parent,DateTime date) {
			if(this.DesignMode){
				_listTaskLists=new List<TaskList>();
				_listTasks=new List<Task>();
				_listTaskNotes=new List<TaskNote>();
				return;
			}
			_listSentTaskSignalNums.Clear();//Full refresh, tracked sent signals are now irrelevant and taking up memory.
			TaskType taskType=TaskType.Normal;
			if(tabContr.SelectedTab==tabReminders) {
				taskType=TaskType.Reminder;
			}
			if(parent!=0){//not a trunk
				//if(TreeHistory.Count>0//we already know this is true
				long userNumInbox=TaskLists.GetMailboxUserNum(_listTaskListTreeHistory[0].TaskListNum);
				_listTaskLists=TaskLists.RefreshChildren(parent,Security.CurUser.UserNum,userNumInbox,taskType,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshChildren(parent,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,userNumInbox,taskType,
					_isTaskSortApptDateTime,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabUser) {
				//If HQ clinic or clinics disabled, default to "0" Region.
				_listTaskLists=TaskLists.RefreshUserTrunk(Security.CurUser.UserNum,Clinics.ClinicNum,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				lock(_listSubscribedTaskListNums) {
					_listSubscribedTaskListNums=_listTaskLists.Select(x => x.TaskListNum).ToList();
				}
				_listTasks=new List<Task>();//no tasks in the user trunk
			}
			else if(tabContr.SelectedTab==tabNew) {
				_listTaskLists=new List<TaskList>();//no task lists in new tab
				_listTasks=Tasks.RefreshUserNew(Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
				lock(_listSubscribedTaskListNums) {
					_listSubscribedTaskListNums=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
				}
			}
			else if(tabContr.SelectedTab==tabOpenTickets) {
				_listTaskLists=new List<TaskList>();//no task lists in new tab
				_listTasks=Tasks.RefreshOpenTickets(Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabPatientTickets) {
				_listTaskLists=new List<TaskList>();
				_listTasks=new List<Task>();
				if(FormOpenDental.CurPatNum!=0) {
					_listTasks=Tasks.RefreshPatientTickets(FormOpenDental.CurPatNum,Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
				}
			}
			else if(tabContr.SelectedTab==tabMain) {
				_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Normal,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Normal,_globalFilterType
					,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabReminders) {
				_listTaskLists=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.Reminder,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshMainTrunk(_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum,TaskType.Reminder
					,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabRepeating) {
				_listTaskLists=TaskLists.RefreshRepeatingTrunk(Security.CurUser.UserNum,Clinics.ClinicNum,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshRepeatingTrunk(Security.CurUser.UserNum,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabDate) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Day,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Day,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabWeek) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Week,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Week,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_globalFilterType,_listFilterFkeys);
			}
			else if(tabContr.SelectedTab==tabMonth) {
				_listTaskLists=TaskLists.RefreshDatedTrunk(date,TaskDateType.Month,Security.CurUser.UserNum,Clinics.ClinicNum
					,Clinics.GetClinic(Clinics.ClinicNum)?.Region??0);
				_listTasks=Tasks.RefreshDatedTrunk(date,TaskDateType.Month,_isShowFinishedTasks,_dateTimeStartShowFinished,Security.CurUser.UserNum
					,_globalFilterType,_listFilterFkeys);
			}
			if(PrefC.IsODHQ && _listTasks!=null && _listTasks.Count>0) {
				_listJobLinks=JobLinks.GetForTaskNums(_listTasks.Select(x => x.TaskNum).ToList());
				_listJobs=Jobs.GetMany(_listJobLinks.Select(x => x.JobNum).ToList());
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
					_listExpandedTaskNums.Clear();
				}
				else {
					_listExpandedTaskNums.AddRange(taskNums);
				}
				_hasListSwitched=false;
			}
			else {
				if(_taskCollapsedState==1) {//Header was clicked, make all collapsed
					_listExpandedTaskNums.Clear();				
				}
				else if(_taskCollapsedState==0) {//Header was clicked, make all expanded
					_listExpandedTaskNums.AddRange(taskNums);
				}
				else { 
					for(int i=_listExpandedTaskNums.Count-1;i>=0;i--) {
						if(!taskNums.Contains(_listExpandedTaskNums[i])) {
							_listExpandedTaskNums.Remove(_listExpandedTaskNums[i]);//The Task was removed from the visual list, don't keep it around in the expanded list.
						}
					}
				}
			}
			_listTaskNotes=TaskNotes.RefreshForTasks(taskNums);
		}

		///<summary>Returns a list of TaskLists containing all directly and indirectly subscribed TaskLists for the current user.</summary>
		public static List<TaskList> GetSubscribedTaskLists(long userNum) {
			List<TaskList> listAllTaskLists=TaskLists.GetAll();
			List<long> listSubscribedTaskListNums=TaskSubscriptions.GetTaskSubscriptionsForUser(userNum).Select(x => x.TaskListNum).ToList();
			List<TaskList> listQueueTaskLists=listAllTaskLists.FindAll(x => listSubscribedTaskListNums.Contains(x.TaskListNum));//Task lists to consider.
			List<TaskList> listSubscribedTaskLists=new List<TaskList>();
			while(listQueueTaskLists.Count>0) {
				TaskList taskList=listQueueTaskLists[0];
				listQueueTaskLists.RemoveAt(0);//pop
				if(!listSubscribedTaskLists.Contains(taskList)) {//Avoid duplicate return values
					listSubscribedTaskLists.Add(taskList);//Each item added to the queue will be part of the return list.
				}
				List<TaskList> listChildren=listAllTaskLists.FindAll(x => x.Parent==taskList.TaskListNum);//Children of taskList.
				foreach(TaskList child in listChildren) {
					if(!listSubscribedTaskLists.Contains(child) && !listQueueTaskLists.Contains(child)) {//Avoid duplicate return values.
						listQueueTaskLists.Add(child);//push
					}
				}
			}
			return listSubscribedTaskLists;
		}
		
		private void tabContr_MouseDown(object sender,MouseEventArgs e) {
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
			}
		}
	
		private void Options_Clicked() {
			using FormTaskOptions formTaskOptions=new FormTaskOptions(_isShowFinishedTasks,_dateTimeStartShowFinished,_isTaskSortApptDateTime
				,_isShowArchivedTaskLists);
			formTaskOptions.StartPosition=FormStartPosition.Manual;//Allows us to set starting form starting Location.
			Point pointFormLocation=this.PointToScreen(ToolBarMain.Location);//Since we cant get ToolBarMain.Buttons["Options"] location directly.
			pointFormLocation.X+=ToolBarMain.Buttons["Options"].Bounds.Width;//Add Options button width so by default form opens along side button.
			Rectangle screenDim=SystemInformation.VirtualScreen;//Dimensions of users screen. Includes if user has more then 1 screen.
			if(pointFormLocation.X+formTaskOptions.Width > screenDim.Width) {//Not all of form will be on screen, so adjust.
				pointFormLocation.X=screenDim.Width-formTaskOptions.Width-5;//5 for some padding.
			}
			if(pointFormLocation.Y+formTaskOptions.Height > screenDim.Height) {//Not all of form will be on screen, so adjust.
				pointFormLocation.Y=screenDim.Height-formTaskOptions.Height-5;//5 for some padding.
			}
			formTaskOptions.Location=pointFormLocation;
			formTaskOptions.ShowDialog();
			if(formTaskOptions.DialogResult!=DialogResult.OK){
				return;
			}
			_isShowFinishedTasks=formTaskOptions.IsShowFinishedTasks;
			_isShowArchivedTaskLists=formTaskOptions.IsShowArchivedTaskLists;
			_dateTimeStartShowFinished=formTaskOptions.DateTimeStartShowFinished;
			_isTaskSortApptDateTime=formTaskOptions.IsSortApptDateTime;
			_isCollapsedByDefault=PIn.Bool(UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse)[0].ValueString);
			_hasListSwitched=true;//To display tasks in correctly collapsed/expanded state
			FillGrid();
		}

		private void AddList_Clicked() {
			if(!Security.IsAuthorized(Permissions.TaskListCreate,false)) {
				return;
			}
			if(tabContr.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//trunk of user tab
				MsgBox.Show(this,"Not allowed to add a task list to the trunk of the user tab.  Either use the subscription feature, or add it to a child list.");
				return;
			}
			if(tabContr.SelectedTab==tabNew) {//new tab
				MsgBox.Show(this,"Not allowed to add items to the 'New' tab.");
				return;
			}
			if(tabContr.SelectedTab==tabPatientTickets) {
				MsgBox.Show(this,"Not allowed to add a task list to the 'Patient Tasks' tab.");
				return;
			}
			TaskList taskList=new TaskList();
			//if this is a child of any other taskList
			if(_listTaskListTreeHistory.Count>0) {
				taskList.Parent=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
			}
			else {
				taskList.Parent=0;
				if(tabContr.SelectedTab==tabDate) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Day;
				}
				else if(tabContr.SelectedTab==tabWeek) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Week;
				}
				else if(tabContr.SelectedTab==tabMonth) {
					taskList.DateTL=monthCalendar.SelectionStart;
					taskList.DateType=TaskDateType.Month;
				}
			}
			if(tabContr.SelectedTab==tabRepeating) {
				taskList.IsRepeating=true;
			}
			taskList.GlobalTaskFilterType=GlobalTaskFilterType.Default;//Results in this taskList inheriting value from PrefName.TasksGlobalFilterType
			using FormTaskListEdit FormT=new FormTaskListEdit(taskList);
			FormT.IsNew=true;
			if(FormT.ShowDialog()==DialogResult.OK) {
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskList.Parent);//Signal for source parent tasklist.
				RefillLocalTaskGrids(taskList,listSentSignalNums:new List<long>() { signalNum });
			}
		}

		private void AddTask(bool isReminder) {
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
			else if(tabContr.SelectedTab==tabNew) {//new tab
				task.TaskListNum=-1;//Force FormTaskEdit to ask user to pick a task list.
			}
			else if(tabContr.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//trunk of user tab
				task.TaskListNum=-1;//Force FormTaskEdit to ask user to pick a task list.
			}
			else {
				task.TaskListNum=0;
				if(tabContr.SelectedTab==tabDate) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Day;
				}
				else if(tabContr.SelectedTab==tabWeek) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Week;
				}
				else if(tabContr.SelectedTab==tabMonth) {
					task.DateTask=monthCalendar.SelectionStart;
					task.DateType=TaskDateType.Month;
				}
			}
			if(tabContr.SelectedTab==tabRepeating) {
				task.IsRepeating=true;
			}
			task.UserNum=Security.CurUser.UserNum;
			if(isReminder) {
				task.ReminderType=TaskReminderType.Once;
			}
			FormTaskEdit FormT=new FormTaskEdit(task,taskOld);
			FormT.IsNew=true;
			FormT.Closing+=new CancelEventHandler(TaskGoToEvent);
			FormT.Show();//non-modal
		}

		private void AddTask_Clicked() {
			bool isReminder=false;
			if(tabContr.SelectedTab==tabReminders) {
				isReminder=true;
			}
			AddTask(isReminder);
		}

		private void menuItemTaskReminder_Click(object sender,EventArgs e) {
			AddTask(true);
		}

		public void Search_Clicked() {
			bool isClipMatch=false;
			string taskNum="";
			try {
				if(Clipboard.ContainsText()) {
					string txtClip="";
					txtClip=Clipboard.GetText();
					if(Regex.IsMatch(txtClip,@"^TaskNum:\d+$")){//very restrictive specific match for "TaskNum:##"
						taskNum=txtClip.Substring(8);
						if(!taskNum.IsNullOrEmpty()) {
							Clipboard.Clear();
							isClipMatch=true;
						}
					}
				}
			} catch {
				//do nothing, not sure how to handle external exceptions
			}
			FormTaskSearch FormTS=new FormTaskSearch();
			if(!isClipMatch) { //if there is no match, open the form as it normally would
				FormTS.Show(this);
				return;
			}
			FormTS.TaskNum=taskNum;
			Task task=Tasks.GetOne(PIn.Long(taskNum));
			if(task==null) {//if the task doesn't match, open the form but pass in the task to search
				FormTS.Show(this);
				return;
			}
			FormTaskEdit FormTE=new FormTaskEdit(task);//don't show the task search form and just open up the task that has been found
			FormTE.Show();
			FormTE.BringToFront();
		}

		public void TaskGoToEvent(object sender,CancelEventArgs e) {
			FormTaskEdit FormT=(FormTaskEdit)sender;
			if(FormT.GotoType!=TaskObjectType.None) {
				GotoType=FormT.GotoType;
				GotoKeyNum=FormT.GotoKeyNum;
				FormOpenDental.S_TaskGoTo(GotoType,GotoKeyNum);
			}
			if(!this.IsDisposed) {
				FillGrid();
			}
		}

		private void BlockSubsc_Clicked() {
			using FormTaskListBlocks FormTLB = new FormTaskListBlocks();
			FormTLB.ShowDialog();
			if(FormTLB.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Security);
			}
		}

		private void Done_Clicked() {
			//already blocked if list
			Task task=_clickedTask;
			Task oldTask=task.Copy();
			task.TaskStatus=TaskStatusEnum.Done;
			if(task.DateTimeFinished.Year<1880) {
				task.DateTimeFinished=DateTime.Now;
			}
			try {
				Tasks.Update(task,oldTask);
			}
			catch(Exception ex) {
				//We manipulated the TaskStatus and need to set it back to what it was because something went wrong.
				int idx=_listTasks.FindIndex(x => x.TaskNum==oldTask.TaskNum);
				if(idx>-1) {
					_listTasks[idx]=oldTask;
				}
				MessageBox.Show(ex.Message);
				return;
			}
			TaskUnreads.DeleteForTask(task);
			TaskHist taskHist=new TaskHist(oldTask);
			taskHist.UserNumHist=Security.CurUser.UserNum;
			TaskHists.Insert(taskHist);
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum);//Only needs to send signal for the one task.
			RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),new List<long>() { signalNum });//No db call.
		}

		private void Edit_Clicked() {
			if(_clickedI < _listTaskLists.Count) {//is list
				using FormTaskListEdit FormT=new FormTaskListEdit(_listTaskLists[_clickedI]);
				FormT.ShowDialog();
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,_listTaskLists[_clickedI].Parent);//Signal for source parent tasklist.
				RefillLocalTaskGrids(_listTaskLists[_clickedI],new List<long>() { signalNum });//No db call.
			}
			else {//task
				FormTaskEdit FormT=new FormTaskEdit(_clickedTask);//Handles signals for this task edit.
				FormT.Show();//non-modal
			}
		}

		private void Cut_Clicked() {
			if(_clickedI < _listTaskLists.Count) {//is list
				_clipTaskList=_listTaskLists[_clickedI].Copy();
				_clipTask=null;
			}
			else {//task
				_clipTaskList=null;
				_clipTask=_clickedTask.Copy();
			}
			_wasCut=true;
		}

		private void Copy_Clicked() {
			if(_clickedI < _listTaskLists.Count) {//is list
				_clipTaskList=_listTaskLists[_clickedI].Copy();
				_clipTask=null;
			}
			else {//task
				_clipTaskList=null;
				_clipTask=_clickedTask.Copy();
				if(!String.IsNullOrEmpty(_clipTask.ReminderGroupId)) {
					//Any reminder tasks duplicated must have a brand new ReminderGroupId
					//so that they do not affect the original reminder task chain.
					Tasks.SetReminderGroupId(_clipTask);
				}
			}
			_wasCut=false;
		}

		///<summary>When copying and pasting, Task hist will be lost because the pasted task has a new TaskNum.</summary>
		private void Paste_Clicked() {
			if(_clipTaskList!=null) {//a taskList is on the clipboard
				if(!_wasCut) {
					return;//Tasklists are no longer allowed to be copied, only cut.  Code should never make it this far.
				}
				TaskList newTL=_clipTaskList.Copy();
				long clipTlParentNum=_clipTaskList.Parent;
				if(_listTaskListTreeHistory.Count>0) {//not on main trunk
					newTL.Parent=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
					if(tabContr.SelectedTab==tabUser){
						//treat pasting just like it's the main tab, because not on the trunk.
					}
					else if(tabContr.SelectedTab==tabMain){
						//even though usually only trunks are dated, we will leave the date alone in main
						//category since user may wish to preserve it. All other children get date cleared.
					}
					else if(tabContr.SelectedTab==tabReminders) {
						//treat pasting just like it's the main tab.
					}
					else if(tabContr.SelectedTab==tabRepeating){
						newTL.DateTL=DateTime.MinValue;//never a date
						//leave dateType alone, since that affects how it repeats
					}
					else if(tabContr.SelectedTab==tabDate
						|| tabContr.SelectedTab==tabWeek
						|| tabContr.SelectedTab==tabMonth) 
					{
						newTL.DateTL=DateTime.MinValue;//children do not get dated
						newTL.DateType=TaskDateType.None;//this doesn't matter either for children
					}
				}
				else {//one of the main trunks
					newTL.Parent=0;
					if(tabContr.SelectedTab==tabUser) {
						//maybe we should treat this like a subscription rather than a paste.  Implement later.  For now:
						MsgBox.Show(this,"Not allowed to paste directly to the trunk of this tab.  Try using the subscription feature instead.");
						return;
					}
					else if(tabContr.SelectedTab==tabMain) {
						newTL.DateTL=DateTime.MinValue;
						newTL.DateType=TaskDateType.None;
					}
					else if(tabContr.SelectedTab==tabReminders) {
						newTL.DateTL=DateTime.MinValue;
						newTL.DateType=TaskDateType.None;
					}
					else if(tabContr.SelectedTab==tabRepeating) {
						newTL.DateTL=DateTime.MinValue;//never a date
						//newTL.DateType=TaskDateType.None;//leave alone
					}
					else if(tabContr.SelectedTab==tabDate){
						newTL.DateTL=monthCalendar.SelectionStart;
						newTL.DateType=TaskDateType.Day;
					}
					else if(tabContr.SelectedTab==tabWeek) {
						newTL.DateTL=monthCalendar.SelectionStart;
						newTL.DateType=TaskDateType.Week;
					}
					else if(tabContr.SelectedTab==tabMonth) {
						newTL.DateTL=monthCalendar.SelectionStart;
						newTL.DateType=TaskDateType.Month;
					}
				}
				if(tabContr.SelectedTab==tabRepeating) {
					newTL.IsRepeating=true;
				}
				else {
					newTL.IsRepeating=false;
				}
				newTL.FromNum=0;//always
				if(_clipTaskList.TaskListNum==newTL.Parent && _wasCut) {
					MsgBox.Show(this,"Cannot cut and paste a task list into itself.  Please move it into a different task list.");
					return;
				}
				if(TaskLists.IsAncestor(_clipTaskList.TaskListNum,newTL.Parent)) {
					//The user is attempting to cut or copy a TaskList into one of its ancestors.  We don't want to do normal movement logic for this case.
					//We move the TaskList desired to have its parent to the list they desire.  
					//We change the TaskList's direct children to have the parent of the TaskList being moved.
					MoveListIntoAncestor(newTL,_clipTaskList.Parent);
				}
				else {
					//If the user has task filters on this TaskList or one of its children, prompt the user they may be moving tasks that are filtered.
					if((GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)!=GlobalTaskFilterType.Disabled &&
						(_globalFilterType!=GlobalTaskFilterType.None || TaskLists.HasGlobalFilterTypeInTree(newTL)) && !ODInitialize.IsRunningInUnitTest)
					{
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel
							,"Task filters are turned on in this task list or one of its sub lists.  Pasting will cause filtered tasks to move as "
							+"well.  Affects all users.  Continue?")) 
						{
							return;
						}
					}
					if(tabContr.SelectedTab==tabUser || tabContr.SelectedTab==tabMain || tabContr.SelectedTab==tabReminders) {
						MoveTaskList(newTL,true);
					}
					else {
						MoveTaskList(newTL,false);
					}
				}
				List<long> listSignalNums=new List<long>();
				if(clipTlParentNum!=0) {
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,clipTlParentNum));//Signal for source parent tasklist.
				}
				if(newTL.Parent!=0) {
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,newTL.Parent));//Signal for destination parent tasklist.
				}
				RefillLocalTaskGrids(newTL,listSignalNums);//No db call.
			}
			else if(_clipTask!=null) {//a task is on the clipboard
				Task newT=_clipTask.Copy();
				long clipTaskTaskListNum=_clipTask.TaskListNum;
				if(_listTaskListTreeHistory.Count>0) {//not on main trunk
					newT.TaskListNum=_listTaskListTreeHistory[_listTaskListTreeHistory.Count-1].TaskListNum;
					if(tabContr.SelectedTab==tabUser) {
						//treat pasting just like it's the main tab, because not on the trunk.
					}
					else if(tabContr.SelectedTab==tabMain) {
						//even though usually only trunks are dated, we will leave the date alone in main
						//category since user may wish to preserve it. All other children get date cleared.
					}
					else if(tabContr.SelectedTab==tabReminders) {
						//treat pasting just like it's the main tab.
					}
					else if(tabContr.SelectedTab==tabRepeating) {
						newT.DateTask=DateTime.MinValue;//never a date
						//leave dateType alone, since that affects how it repeats
					}
					else if(tabContr.SelectedTab==tabDate
						|| tabContr.SelectedTab==tabWeek
						|| tabContr.SelectedTab==tabMonth) 
					{
						newT.DateTask=DateTime.MinValue;//children do not get dated
						newT.DateType=TaskDateType.None;//this doesn't matter either for children
					}
				}
				else {//one of the main trunks
					newT.TaskListNum=0;
					if(tabContr.SelectedTab==tabUser) {
						//never allowed to have a task on the user trunk.
						MsgBox.Show(this,"Tasks may not be pasted directly to the trunk of this tab.  Try pasting within a list instead.");
						return;
					}
					else if(tabContr.SelectedTab==tabMain) {
						newT.DateTask=DateTime.MinValue;
						newT.DateType=TaskDateType.None;
					}
					else if(tabContr.SelectedTab==tabReminders) {
						newT.DateTask=DateTime.MinValue;
						newT.DateType=TaskDateType.None;
					}
					else if(tabContr.SelectedTab==tabRepeating) {
						newT.DateTask=DateTime.MinValue;//never a date
						//newTL.DateType=TaskDateType.None;//leave alone
					}
					else if(tabContr.SelectedTab==tabDate) {
						newT.DateTask=monthCalendar.SelectionStart;
						newT.DateType=TaskDateType.Day;
					}
					else if(tabContr.SelectedTab==tabWeek) {
						newT.DateTask=monthCalendar.SelectionStart;
						newT.DateType=TaskDateType.Week;
					}
					else if(tabContr.SelectedTab==tabMonth) {
						newT.DateTask=monthCalendar.SelectionStart;
						newT.DateType=TaskDateType.Month;
					}
				}
				if(tabContr.SelectedTab==tabRepeating) {
					newT.IsRepeating=true;
				}
				else {
					newT.IsRepeating=false;
				}
				newT.FromNum=0;//always
				if(!String.IsNullOrEmpty(newT.ReminderGroupId)) {
					//Any reminder tasks duplicated to another task list must have a brand new ReminderGroupId
					//so that they do not affect the original reminder task chain.
					Tasks.SetReminderGroupId(newT);
				}
				if(_wasCut && Tasks.WasTaskAltered(_clipTask)){
					MsgBox.Show("Tasks","Not allowed to move because the task has been altered by someone else.");
					FillGrid();
					return;
				}
				string histDescript="";
				List<TaskNote> noteList;
				List<long> listSignalNums=new List<long>();
				if(_wasCut) { //cut
					if(clipTaskTaskListNum==newT.TaskListNum) {//User cut then paste into the same task list.
						return;//Nothing to do.
					}
					noteList=TaskNotes.GetForTask(newT.TaskNum);
					histDescript="This task was cut from task list "+TaskLists.GetFullPath(_clipTask.TaskListNum)+" and pasted into "+TaskLists.GetFullPath(newT.TaskListNum);
					Tasks.Update(newT,_clipTask);
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,clipTaskTaskListNum));//Signal for source tasklist.
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,_clipTask.TaskNum));//Signal for current task.
				}
				else { //copied
					noteList=TaskNotes.GetForTask(newT.TaskNum);
					newT.TaskNum=Tasks.Insert(newT);//Creates a new PK for newT  Copy, no need to signal source.
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,newT.TaskNum));//Signal for new task.
					histDescript="This task was copied from task "+_clipTask.TaskNum+" in task list "+TaskLists.GetFullPath(_clipTask.TaskListNum);
					for(int t=0;t<noteList.Count;t++) {
						noteList[t].TaskNum=newT.TaskNum;
						TaskNotes.Insert(noteList[t]);//Creates the new note with the current datetime stamp.
						TaskNotes.Update(noteList[t]);//Restores the historical datetime for the note.
					}
				}
				TaskHist hist=new TaskHist(newT);
				hist.Descript=histDescript;
				hist.UserNum=Security.CurUser.UserNum;
				TaskHists.Insert(hist);
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,newT.TaskNum);//Popup
				TaskUnreads.AddUnreads(newT,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,newT.TaskListNum));//Signal for destination tasklist.
				RefillLocalTaskGrids(newT,noteList,listSignalNums);//No db call.
			}
			//Turn the cut into a copy once the users has pasted at least once.
			_wasCut=false;
		}

		/// <summary>Return the FormTaskEdit that was created from showing the task.  Can return null.</summary>
		private FormTaskEdit SendToMe_Clicked(bool doOpenTask=true) {
			if(Security.CurUser.TaskListInBox==0) {
				MsgBox.Show(this,"You do not have an inbox.");
				return null;
			}
			Task task=_clickedTask;
			Task oldTask=task.Copy();
			task.TaskListNum=Security.CurUser.TaskListInBox;
			Cursor=Cursors.WaitCursor;
			List<long> listSignalNums=new List<long>();
			try {
				Tasks.Update(task,oldTask);
				//At HQ the refresh interval wasn't quick enough for the task to pop up.
				//We will immediately show the task instead of waiting for the refresh interval.
				TaskHist taskHist=new TaskHist(oldTask);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,oldTask.TaskListNum));//Signal for old TaskList containing this Task.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,task.TaskListNum));//Signal for new tasklist.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum));//Signal for task.
				RefillLocalTaskGrids(task,_listTaskNotes.FindAll(x => x.TaskNum==task.TaskNum),listSignalNums);
				Cursor=Cursors.Default;
				FormTaskEdit FormT=new FormTaskEdit(task,task.Copy());
				FormT.IsPopup=true;
				if(doOpenTask) {
					FormT.Show();//non-modal
				}
				return FormT;
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
			FormTaskEdit openedForm=SendToMe_Clicked(doOpenTask:false);
			if(openedForm==null) {
				return;
			}
			Goto_Clicked();
			openedForm.Show();//We want to show any popups first before we open the task.
			//If opened from another form and the user presses cancel on FormTaskNoteEdit, it will hide the task behind the parent form (this).  
			//Calling activate makes sure if we cancel out, the topmost form will be FormTaskEdit.
			openedForm.Activate();
			//String should not be changed.  Used for auditing triage tasks.
			openedForm.AddNoteToTaskAndEdit("Returned call. ");
			Tasks.TaskEditCreateLog(Permissions.TaskNoteEdit,Lan.g(this,"Automatically added task note")+": Returned Call",Tasks.GetOne(openedForm.TaskNumCur));
		}

		private void Goto_Clicked() {
			//not even allowed to get to this point unless a valid task
			Task task=_clickedTask;
			GotoType=task.ObjectType;
			GotoKeyNum=task.KeyNum;
			FormOpenDental.S_TaskGoTo(GotoType,GotoKeyNum);
		}

		///<summary>Marks the selected task as read and updates the grid.</summary>
		private void MarkRead(Task markedTask) {
			if(markedTask==null) {
				MsgBox.Show(this,"Please select a valid task.");
				return;
			}
			markedTask.IsUnread=TaskUnreads.IsUnread(Security.CurUser.UserNum,markedTask);
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				if(tabContr.SelectedTab==tabNew){
					//these are never in someone else's inbox, so don't block.
				}
				else if(tabContr.SelectedTab==tabPatientTickets 
					&& markedTask.IsUnread) 
				{
					//Task clicked is new for the user, don't block.
				}
				else{
					long userNumInbox=0;
					if(ListTools.In(tabContr.SelectedTab,tabOpenTickets,tabPatientTickets)) {
						userNumInbox=TaskLists.GetMailboxUserNumByAncestor(markedTask.TaskNum);
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
				if(markedTask.IsUnread) {
					if(Tasks.IsReminderTask(markedTask) && markedTask.DateTimeEntry>DateTime.Now){
						MsgBox.Show(this,"Not allowed to mark future Reminders as read.");
					}
					else{
						TaskUnreads.SetRead(Security.CurUser.UserNum,markedTask);//Takes care of Db.
					}
				}
				long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,markedTask.TaskNum);//Signal for markedTask.
				RefillLocalTaskGrids(markedTask,_listTaskNotes.FindAll(x => x.TaskNum==markedTask.TaskNum),new List<long>() { signalNum });
				//if already read, nothing else to do.  If done, nothing to do
			}
			else {
				if(markedTask.TaskStatus==TaskStatusEnum.New) {
					Task task=markedTask.Copy();
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

		private void MoveListIntoAncestor(TaskList newList,long oldListParent) {
			if(_wasCut) {//If the TaskList was cut, move direct children of the list "up" one in the hierarchy and then update
				List<TaskList> childLists=TaskLists.RefreshChildren(newList.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
				for(int i=0;i<childLists.Count;i++) {
					childLists[i].Parent=oldListParent;
					TaskLists.Update(childLists[i]);
				}
				TaskLists.Update(newList);
			}
			else {//Just insert a new TaskList if it was copied.
				TaskLists.Insert(newList);
			}
		}

		///<summary>Assign new parent FKey for existing tasklist, and update TaskAncestors.  Used when cutting and pasting a tasklist.
		///Does not create new task or tasklist entries.</summary>
		private void MoveTaskList(TaskList newList,bool isInMainOrUser) {
			List<TaskList> childLists=TaskLists.RefreshChildren(newList.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> childTasks=Tasks.RefreshChildren(newList.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All
				,GlobalTaskFilterType.None);//No filtering, because all child tasks should move regardless of filtration.
			TaskLists.Update(newList);//Not making a new TaskList, just moving an old one
			for(int i=0;i<childLists.Count;i++) { //updates all the child tasklists and recursively calls this method for each of their children lists.
				childLists[i].Parent=newList.TaskListNum;
				if(newList.IsRepeating) {
					childLists[i].IsRepeating=true;
					childLists[i].DateTL=DateTime.MinValue;//never a date
				}
				else {
					childLists[i].IsRepeating=false;
				}
				childLists[i].FromNum=0;
				if(!isInMainOrUser) {
					childLists[i].DateTL=DateTime.MinValue;
					childLists[i].DateType=TaskDateType.None;
				}
				MoveTaskList(childLists[i],isInMainOrUser);//delete any existing subscriptions
			}
			TaskAncestors.SynchManyForSameTasklist(childTasks,newList.TaskListNum,newList.Parent);
		}

		///<summary>Only used for dated task lists. Should NOT be used for regular task lists, puts too much strain on DB with large amount of tasks.
		///A recursive function that duplicates an entire existing TaskList.  
		///For the initial loop, make changes to the original taskList before passing it in.  
		///That way, Date and type are only set in initial loop.  All children preserve original dates and types. 
		///The isRepeating value will be applied in all loops.  Also, make sure to change the parent num to the new one before calling this function.
		///The taskListNum will always change, because we are inserting new record into database. </summary>
		private void DuplicateExistingList(TaskList newList,bool isInMainOrUser) {
			//get all children:
			List<TaskList> childLists=TaskLists.RefreshChildren(newList.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> childTasks=Tasks.RefreshChildren(newList.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All,
				GlobalTaskFilterType.None);//No filtering, because all child tasks should duplicate regardless of filtration.
			if(_wasCut) { //Not making a new TaskList, just moving an old one
				TaskLists.Update(newList);
			}
			else {//copied -- We are making a new TaskList, we're keeping the old one as well
				TaskLists.Insert(newList);
			}
			//now we have a new taskListNum to work with
			for(int i=0;i<childLists.Count;i++) { //updates all the child tasklists and recursively calls this method for each of their children lists.
				childLists[i].Parent=newList.TaskListNum;
				if(newList.IsRepeating) {
					childLists[i].IsRepeating=true;
					childLists[i].DateTL=DateTime.MinValue;//never a date
				}
				else {
					childLists[i].IsRepeating=false;
				}
				childLists[i].FromNum=0;
				if(!isInMainOrUser) {
					childLists[i].DateTL=DateTime.MinValue;
					childLists[i].DateType=TaskDateType.None;
				}
				DuplicateExistingList(childLists[i],isInMainOrUser);//delete any existing subscriptions
			}
			for(int i = 0;i<childTasks.Count;i++) { //updates all the child tasks. If the task list was cut, then just update the child tasks' ancestors.
				if(_wasCut) {
					TaskAncestors.Synch(childTasks[i]);
				}
				else {//copied
					childTasks[i].TaskListNum=newList.TaskListNum;
					if(newList.IsRepeating) {
						childTasks[i].IsRepeating=true;
						childTasks[i].DateTask=DateTime.MinValue;//never a date
					}
					else {
						childTasks[i].IsRepeating=false;
					}
					childTasks[i].FromNum=0;
					if(!isInMainOrUser) {
						childTasks[i].DateTask=DateTime.MinValue;
						childTasks[i].DateType=TaskDateType.None;
					}
					if(!String.IsNullOrEmpty(childTasks[i].ReminderGroupId)) {
						//Any reminder tasks duplicated to another task list must have a brand new ReminderGroupId
						//so that they do not affect the original reminder task chain.
						Tasks.SetReminderGroupId(childTasks[i]);
					}
					List<TaskNote> noteList=TaskNotes.GetForTask(childTasks[i].TaskNum);
					long newTaskNum=Tasks.Insert(childTasks[i]);
					for(int t=0;t<noteList.Count;t++) {
						noteList[t].TaskNum=newTaskNum;
						TaskNotes.Insert(noteList[t]);//Creates the new note with the current datetime stamp.
						TaskNotes.Update(noteList[t]);//Restores the historical datetime for the note.
					}
				}
			}
		}

		private void Delete_Clicked() {
			if(_clickedI < _listTaskLists.Count) {//is list
				TaskList taskListToDelete=_listTaskLists[_clickedI];
				//check to make sure the list is empty.  Do not filter tasks so we don't try to delete a list that still has tasks.
				List<Task> tsks=Tasks.RefreshChildren(taskListToDelete.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All,
					GlobalTaskFilterType.None);
				List<TaskList> tsklsts=TaskLists.RefreshChildren(taskListToDelete.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
				int countHiddenTasks=tsklsts.Sum(x => x.NewTaskCount)+tsks.Count-taskListToDelete.NewTaskCount;
				if(tsks.Count>0 || tsklsts.Count>0){
					MessageBox.Show(Lan.g(this,"Not allowed to delete a list unless it's empty.  This task list contains:")+"\r\n"
						+tsks.FindAll(x => String.IsNullOrEmpty(x.ReminderGroupId)).Count+" "+Lan.g(this,"normal tasks")+"\r\n"
						+tsks.FindAll(x => !String.IsNullOrEmpty(x.ReminderGroupId)).Count+" "+Lan.g(this,"reminder tasks")+"\r\n"
						+countHiddenTasks+" "+Lan.g(this,"filtered tasks")+"\r\n"
						+tsklsts.Count+" "+Lan.g(this,"task lists"));
					return;
				}
				if(TaskLists.GetMailboxUserNum(taskListToDelete.TaskListNum)!=0) {
					MsgBox.Show(this,"Not allowed to delete task list because it is attached to a user inbox.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this empty list?")) {
					return;
				}
				TaskSubscriptions.UpdateTaskListSubs(taskListToDelete.TaskListNum,0);
				TaskLists.Delete(taskListToDelete);
				long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskListToDelete.Parent);//Signal for source tasklist.
				RefillLocalTaskGrids(taskListToDelete,new List<long>() { signalNum },false);//No db calls.
			}
			else {//Is task
				//This security logic should match FormTaskEdit for when we enable the delete button.
				bool isTaskForCurUser = true;
				if(_clickedTask.UserNum!=Security.CurUser.UserNum) {//current user didn't write this task, so block them.
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(_clickedTask.TaskListNum!=Security.CurUser.TaskListInBox) {//the task is not in the logged-in user's inbox
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(isTaskForCurUser) {
					List<TaskNote> listTaskNotes=TaskNotes.GetForTask(_clickedTask.TaskNum);//so we can check so see if other users have added notes
					for(int i = 0;i<listTaskNotes.Count;i++) {
						if(Security.CurUser.UserNum!=listTaskNotes[i].UserNum) {
							isTaskForCurUser=false;
							break;
						}
					}
				}
				//Purposefully show a popup if the user is not authorized to delete this task.
				if(!isTaskForCurUser && (!Security.IsAuthorized(Permissions.TaskEdit) || !Security.IsAuthorized(Permissions.TaskNoteEdit))) {
					return;
				}
				//This logic should match FormTaskEdit.butDelete_Click()
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Task?")) {
					return;
				}
				if(Tasks.GetOne(_clickedTask.TaskNum)==null) {
					MsgBox.Show(this,"Task already deleted.");
					return;
				}
				if(_clickedTask.TaskListNum==0) {
					Tasks.TaskEditCreateLog(Lan.g(this,"Deleted task"),_clickedTask);
				}
				else {
					string logText=Lan.g(this,"Deleted task from tasklist");
					TaskList tList=TaskLists.GetOne(_clickedTask.TaskListNum);
					if(tList!=null) {
						logText+=" "+tList.Descript;
					}
					else {
						logText+=". Task list no longer exists";
					}
					logText+=".";
					Tasks.TaskEditCreateLog(logText,_clickedTask);
				}
				Tasks.Delete(_clickedTask.TaskNum);//always do it this way to clean up all four tables
				List<long> listSignalNums=new List<long>();
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,_clickedTask.TaskListNum));//Signal for source tasklist.
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,_clickedTask.TaskNum));//Signal for current task.
				RefillLocalTaskGrids(_clickedTask,_listTaskNotes.FindAll(x => x.TaskNum==_clickedTask.TaskNum),listSignalNums,false);
				TaskHist taskHistory = new TaskHist(_clickedTask);
				taskHistory.IsNoteChange=false;
				taskHistory.UserNum=Security.CurUser.UserNum;
				TaskHists.Insert(taskHistory);
				SecurityLogs.MakeLogEntry(Permissions.TaskEdit,0,"Task "+POut.Long(_clickedTask.TaskNum)+" deleted",0);
			}
		}

		///<summary>A recursive function that deletes the specified list and all children.</summary>
		private void DeleteEntireList(TaskList list) {
			//get all children:
			List<TaskList> childLists=TaskLists.RefreshChildren(list.TaskListNum,Security.CurUser.UserNum,0,TaskType.All);
			List<Task> childTasks=Tasks.RefreshChildren(list.TaskListNum,true,DateTime.MinValue,Security.CurUser.UserNum,0,TaskType.All);
			for(int i=0;i<childLists.Count;i++) {
				DeleteEntireList(childLists[i]);
			}
			for(int i=0;i<childTasks.Count;i++) {
				Tasks.Delete(childTasks[i].TaskNum);
				SecurityLogs.MakeLogEntry(Permissions.TaskEdit,0,"Task "+POut.Long(childTasks[i].TaskNum)+" deleted",0);
			}
			try {
				TaskLists.Delete(list);
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
			if(tabContr.SelectedTab==tabNew && e.Col==2 && PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//+/- column (an index varaible would help)
				return;//Don't double click on expand column, because it already has a single click functionality.
			}
			else if(tabContr.SelectedTab==tabNew && e.Col==3 && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//ST column (an index varaible would help)
				return;//Don't double click on ST column.
			}
			else if(tabContr.SelectedTab==tabNew && e.Col==4 && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//Job column (an index varaible would help)
				return;//Don't double click on Job column.
			}
			else if(e.Col==1) {//Task List column (an index varaible would help)
				return;//Don't double click on expand column
			}
			if(e.Row >= _listTaskLists.Count) {//is task
				if(IsInvalidTaskRow(e.Row)) {
					return; //could happen if the task list refreshed while the double-click was happening.
				}
				//It's important to grab the task directly from the db because the status in this list is fake, being the "unread" status instead.
				Task task=Tasks.GetOne(_listTasks[e.Row-_listTaskLists.Count].TaskNum);
				if(task==null) {//Task was deleted or moved.
					return;
				}
				FormTaskEdit FormT=new FormTaskEdit(task);
				FormT.Show();//non-modal
			}
		}

		///<summary>Necessary to use this handler to set _clickedI before menuEdit_Popup.  This is due to the order MouseDown vs CellClick events fire.
		///Only using CellClick to set these variables resulted in stale values in menuEdit_Popup.</summary>
		private void gridMain_MouseDown(object sender,MouseEventArgs e) {
			SetClickedIAndTask(e);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			int clickedCol=e.Col;
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(_clickedI < _listTaskLists.Count) {//is list
				_listTaskListTreeHistory.Add(_listTaskLists[_clickedI]);
				_hasListSwitched=true;
				SetFiltersToDefault(_listTaskLists[_clickedI]);//Fills Tree and Grid
				return;
			}
			_taskCollapsedState=-1;
			if(tabContr.SelectedTab==tabNew && !PrefC.GetBool(PrefName.TasksNewTrackedByUser)){//There's an extra column
				if(clickedCol==1) {
					TaskUnreads.SetRead(Security.CurUser.UserNum,_listTasks[_clickedI-_listTaskLists.Count]);
					FillGrid();
				}
				if(clickedCol==3) {//Expand column
					if(_listExpandedTaskNums.Contains(_listTasks[_clickedI-_listTaskLists.Count].TaskNum)) {
						_listExpandedTaskNums.Remove(_listTasks[_clickedI-_listTaskLists.Count].TaskNum);
					}
					else { 
						_listExpandedTaskNums.Add(_listTasks[_clickedI-_listTaskLists.Count].TaskNum);
					}
					FillGrid();
				}
				return;//but ignore column 0 for now.  We would need to add that as a new feature.
			}
			if(clickedCol==0){//check tasks off
				MarkRead(_listTasks[_clickedI-_listTaskLists.Count]);
			}
			if((ListTools.In(tabContr.SelectedTab,tabNew,tabPatientTickets,tabOpenTickets) && clickedCol==2) 
				|| (tabContr.SelectedTab!=tabNew && clickedCol==1)) 
			{
				if(_listExpandedTaskNums.Contains(_listTasks[_clickedI-_listTaskLists.Count].TaskNum)) {
					_listExpandedTaskNums.Remove(_listTasks[_clickedI-_listTaskLists.Count].TaskNum);
				}
				else { 
					_listExpandedTaskNums.Add(_listTasks[_clickedI-_listTaskLists.Count].TaskNum);
				}
				FillGrid();
			}
		}
				
		///<summary>Helper function to centralize _clickedI and _clickedTask logic.</summary>
		private void SetClickedIAndTask(object e) {
			if(e is ODGridClickEventArgs) {
				_clickedI=((ODGridClickEventArgs)e).Row;
			}
			else if(e is MouseEventArgs) {
				_clickedI=gridMain.PointToRow(((MouseEventArgs)e).Y);
			}
			if(_clickedI==-1){
				return;
			}
			if(_clickedI>=gridMain.ListGridRows.Count) {//Grid refreshed mid-click and _clickedI is no longer valid.
				_clickedI=-1;
				_clickedTask=null;
				SetMenusEnabled();
				return;
			}
			if(gridMain.ListGridRows[_clickedI].Tag is Task) {
				_clickedTask=(Task)gridMain.ListGridRows[_clickedI].Tag;//Task lists cause _clickedTask to be null
			}
			else {
				_clickedTask=null;
			}
		}

		private void menuEdit_Popup(object sender,System.EventArgs e) {
			SetMenusEnabled();
		}

		private void SetMenusEnabled() {
			//Done----------------------------------
			if(gridMain.SelectedIndices.Length==0 || _clickedI < _listTaskLists.Count) {//or a tasklist selected
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
				if(_clickedI < _listTaskLists.Count) {//Is a tasklist
					menuItemCopy.Enabled=false;//We don't want users to copy tasklists, only move them by cut.
				}
				else {
					menuItemCopy.Enabled=true;
				}
				menuItemDelete.Enabled=true;
			}
			//Paste----------------------------------------
			if(tabContr.SelectedTab==tabUser && _listTaskListTreeHistory.Count==0) {//not allowed to paste into the trunk of a user tab
				menuItemPaste.Enabled=false;
			}
			else if(_clipTaskList==null && _clipTask==null) {
				menuItemPaste.Enabled=false;
			}
			else {//there is an item on our clipboard
				menuItemPaste.Enabled=true;
			}
			//(overrides)
			if(tabContr.SelectedTab==tabNew || tabContr.SelectedTab==tabOpenTickets || tabContr.SelectedTab==tabPatientTickets) {
				menuItemCut.Enabled=false;
				menuItemDelete.Enabled=false;
				menuItemPaste.Enabled=false;
			}
			//Subscriptions----------------------------------------------------------
			if(gridMain.SelectedIndices.Length==0) {
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=false;
			}
			else if(tabContr.SelectedTab==tabUser && _clickedI<_listTaskLists.Count) {//user tab and is a list
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=true;
			}
			else if(tabContr.SelectedTab==tabMain && _clickedI < _listTaskLists.Count) {//main and tasklist
				menuItemSubscribe.Enabled=true;
				menuItemUnsubscribe.Enabled=false;
			}
			else if(tabContr.SelectedTab==tabReminders && _clickedI < _listTaskLists.Count) {//reminders and tasklist
				menuItemSubscribe.Enabled=true;
				menuItemUnsubscribe.Enabled=false;
			}
			else{//either any other tab, or a task on the main list
				menuItemSubscribe.Enabled=false;
				menuItemUnsubscribe.Enabled=false;
			}
			menuItemPriority.MenuItems.Clear();
			//SendToMe/GoTo/Task Priority/DeleteTaskTaken---------------------------------------------------------------
			if(gridMain.SelectedIndices.Length>0 && _clickedI >= _listTaskLists.Count){//is task
				//The clicked task was removed from _listTasks, could happen between FillGrid(), mouse click, and now
				if(IsInvalidTaskRow(_clickedI)) {
					IgnoreTaskClick();
					return;
				}
				Task task=_listTasks[_clickedI-_listTaskLists.Count];
				if(task.ObjectType==TaskObjectType.None) {
					menuItemGoto.Enabled=false;
				}
				else {
					menuItemGoto.Enabled=true;
				}
				if(PrefC.IsODHQ && Security.IsAuthorized(Permissions.TaskEdit,true)) {
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
					Def[] defs=Defs.GetDefsForCategory(DefCat.TaskPriorities,true).ToArray();
					foreach(Def def in defs) {
						MenuItem item=menuItemPriority.MenuItems.Add(def.ItemName);
						item.Click+=(sender,e) => menuTaskPriority_Click(task,def);
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
			if(gridMain.SelectedIndices.Length>0 && _clickedI >= _listTaskLists.Count && PrefC.IsODHQ) {
				//The clicked task was removed from _listTasks, could happen between FillGrid(), mouse click, and now
				if(IsInvalidTaskRow(_clickedI)) {
					IgnoreTaskClick();
					return;
				}
				Task task=_listTasks[_clickedI-_listTaskLists.Count];
				//get list of jobs attached to task then insert info about those jobs.
				List<JobLink> _listJobLinks=JobLinks.GetForTask(task.TaskNum);
				List<Job> _listJobs=Jobs.GetMany(_listJobLinks.Select(x => x.JobNum).ToList());
				//If a job exists that is attached to the task
				if(_listJobs.Count>0) {
					menuNavJob.MenuItems.Clear();	//clear whatever items were in the menu before.
					MenuItem newItem;
					string title;
					//Get a jobnum that matches the column in task menu
					foreach(Job selectedJob in _listJobs) {
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
			//Archived/Unarchived-------------------------------------------------------------
			menuArchive.Visible=false;
			menuUnarchive.Visible=false;
			if(_clickedI > -1 && tabContr.SelectedTab==tabMain && _clickedI < _listTaskLists.Count 
				&& _listTaskLists[_clickedI].TaskListStatus==TaskListStatusEnum.Active)
			{
				menuArchive.Visible=true;
			}
			if(_clickedI > -1 && ListTools.In(tabContr.SelectedTab,tabUser,tabMain,tabReminders) && _clickedI < _listTaskLists.Count
				&& _listTaskLists[_clickedI].TaskListStatus==TaskListStatusEnum.Archived)
			{
				menuUnarchive.Visible=true;
			}
			if(_clickedI<0) {//Not clicked on any row
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

		private bool IsInvalidTaskRow(int row) {//Index out of range
			return (row-_listTaskLists.Count < 0 || row-_listTaskLists.Count >= _listTasks.Count);
		}

		private void IgnoreTaskClick() {
			gridMain.SetSelected(_clickedI,false);//unselect problem row
			_clickedI=-1;//since row is unselected		
			foreach(MenuItem menuItem in gridMain.ContextMenu.MenuItems) { //disable ContextMenu options
				menuItem.Enabled=false;
			}
			Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,_clickedTask.TaskListNum);
			FillGrid();//Full Refresh.
		}

		private void OnSubscribe_Click(){
			//Won't even get to this point unless it is a list.  TaskListNum will never be 0.
			if(TaskSubscriptions.TrySubscList(_listTaskLists[_clickedI].TaskListNum,Security.CurUser.UserNum)) {
				lock(_listSubscribedTaskListNums) {
					_listSubscribedTaskListNums=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
				}
			}
			else { //already subscribed.
				MsgBox.Show(this,"User already subscribed.");
				return;
			}
			MsgBox.Show(this,"Done");
			RefillLocalTaskGrids(_listTaskLists[_clickedI],null);
		}

		private void OnUnsubscribe_Click() {
			TaskSubscriptions.UnsubscList(_listTaskLists[_clickedI].TaskListNum,Security.CurUser.UserNum);
			lock(_listSubscribedTaskListNums) {
				_listSubscribedTaskListNums=GetSubscribedTaskLists(Security.CurUser.UserNum).Select(x => x.TaskListNum).ToList();
			};
			RefillLocalTaskGrids(_listTaskLists[_clickedI],null);
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
			MarkRead(_clickedTask);
		}

		private void menuNavJob_Click(object sender,EventArgs e,Job selectedJob) {
			FormOpenDental.S_GoToJob(selectedJob.JobNum);
		}

		private void menuDeleteTaken_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			TaskTakens.DeleteForTask(_clickedTask.TaskNum);
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Task taken deleted");
		}

		private void menuTaskPriority_Click(Task task,Def priorityDef) {
			Task taskNew=task.Copy();
			taskNew.PriorityDefNum=priorityDef.DefNum;
			try {
				Tasks.Update(taskNew,task);
				if(PrefC.IsODHQ && priorityDef.DefNum==502) {//They chose Blue as their priority
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
			TaskLists.Archive(_listTaskLists[_clickedI]);
			long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,_listTaskLists[_clickedI].Parent);//Signal for source parent tasklist.
			RefillLocalTaskGrids(_listTaskLists[_clickedI],new List<long>() { signalNum });//No db call.
		}

		private void menuUnarchive_Click(object sender,EventArgs e) {
			//Will not get here unless clicked index is an archived task list
			TaskLists.Unarchive(_listTaskLists[_clickedI]);
			long signalNum=Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,_listTaskLists[_clickedI].Parent);//Signal for source parent tasklist.
			RefillLocalTaskGrids(_listTaskLists[_clickedI],new List<long>() { signalNum });//No db call.
		}

		//private void listMain_SelectedIndexChanged(object sender,System.EventArgs e) {
		//	SetMenusEnabled();
		//}

		private void tree_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			TreeNode selectedNode=tree.GetNodeAt(e.X,e.Y);
			if(selectedNode==null) {
				return;//Clicking just below a node results in tree.GetNodeAt(e.X,e.Y) to be null.  Since user didn't make an actual selection, return.
			}
			for(int i=_listTaskListTreeHistory.Count-1;i>0;i--) {
				if(_listTaskListTreeHistory[i].TaskListNum==(long)selectedNode.Tag) {
					break;//don't remove the node click on or any higher node
				}
				_listTaskListTreeHistory.RemoveAt(i);
			}
			TaskList taskListNewSelection=_listTaskListTreeHistory.FirstOrDefault(x => x.TaskListNum==(long)selectedNode.Tag);
			SetFiltersToDefault(taskListNewSelection);//Fills Tree and Grid.
		}
		
		///<summary>Currently only used so that we can set the title of FormTask.</summary>
		public delegate void FillGridEventHandler(object sender,EventArgs e);

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
