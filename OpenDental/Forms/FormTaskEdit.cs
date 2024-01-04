using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTaskEdit:FormODBase {
		public Task TaskCur;
		private Task _taskOld;
		///<summary></summary>
		public bool IsNew;
		///<summary>After closing, if this is not zero, then it will jump to the object specified in GotoKeyNum.</summary>
		public TaskObjectType TaskObjectTypeGoTo;
		///<summary>After closing, if this is not zero, then it will jump to the specified patient.</summary>
		public long KeyNumGoTo;
		private TaskList _taskList;
		///<summary>Will be set to true if any note was added or an existing note changed. Does not track changes in the description.</summary>
		public bool DidNotesChange;
		private List<TaskNote> _listTaskNotes;
		///<summary>If the reply button is visible, this stores who to reply to.  It's determined when loading the form.</summary>
		private long _userNumReplyTo;
		///<summary>Gets set to true externally if this window popped up without user interaction.  It will behave slightly differently.  
		///Specifically, the New checkbox will be unchecked so that if user clicks OK, the task will be marked as read.
		///Also if IsPop is set to true, this window will not steal focus from other windows when poping up.</summary>
		public bool IsPopup;
		///<summary>When tracking status by user, this tracks whether it has changed.  This is so that if it has changed, a signal can be sent for a refresh of lists.</summary>
		private bool _hasStatusChanged;
		///<summary>When this window is first opened, if this task is in someone else's inbox, then the "new" status is meaningless and will not show.  In that case, this variable is set to true.  Only used when tracking new status by user.</summary>
		private bool _startedInOthersInbox;
		///<summary>Filled on load with all non-hidden task priority definitions.</summary>
		private List<Def> _listDefsTaskPriorities;
		private long _defNumPrioritySelected;
		///<summary>Keeps track of the number of notes that were associated to this task on load and after refilling the task note grid.  Only used for HQ in order to keep track of task note manipulation.</summary>
		private int _numNotes=-1;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for red.</summary>
		private const long _triageNumRed=501;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for blue.</summary>
		private const long _triageNumBlue=502;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for white.</summary>
		private const long _triageNumWhite=503;
		private List<JobLink> _listJobLinks;
		private List<Job> _listJobs;
		private List<TaskAttachment> _listTaskAttachments;
		private bool _isLoading;
		private List<TaskReminderType> _listTaskReminderTypes;
		///<summary>Do not allow any task or task related changes.  Only allow copy and cancel buttons, and copying of text.
		///Used when task has been deleted from elsewhere while still open.</summary>
		private bool _isTaskDeleted=false;
		///<summary>UserNum attached to task when form was loaded.  Used in OK click to detect changes.  Can be 0.</summary>
		private long _userNumFrom;
		private const string _autoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";
		///<summary>PatNum attached to task when form was loaded.  Used in OK click to detect changes.  Can be 0.</summary>
		private long _patNum;
		///<summary>Modal window</summary>
		private FormTaskNoteEdit _formTaskNoteEdit;

		///<summary>This is used to make the task window not steal focus when opening as a popup.</summary>
		protected override bool ShowWithoutActivation{
			get { 
				return IsPopup; 
			}
		}

		///<summary>Task gets inserted ahead of time, then frequently altered before passing in here.  The taskOld that is passed in should be the task as it is in the database.  When saving, taskOld will be compared with db to make sure no changes.</summary>
		public FormTaskEdit(Task task,Task taskOld=null) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			TaskCur=task;
			if(taskOld==null) {
				_taskOld=task.Copy();
			}
			else {
				_taskOld=taskOld;
			}
			_taskList=TaskLists.GetOne(task.TaskListNum);
			Lan.F(this);
		}

		private void FormTaskListEdit_Load(object sender,System.EventArgs e) {
			LoadTask();
			gridMain.ScrollToEnd();
			splitContainer.SplitterDistance=LayoutManager.Scale(splitContainer.SplitterDistance);
		}

		private void LoadTask() {
			textTaskNum.Text=TaskCur.TaskNum.ToString();//Assign TaskNum
			if(PrefC.IsODHQ){
				butSubscribers.Visible=true;
				if(!IsNew) {//to simplify the code, only allow jobs to be attached to existing tasks at HQ, not new tasks.
					comboJobs.Visible=true;
					labelJobs.Visible=true;
					FillComboJobs();
				}
			}
			else{
				labelCategory.Visible=false;
				listCategory.Visible=false;
				Width=splitContainer.Right+25;
			}
			FillTextAttachments();
			if(IsNew) {
				//butDelete.Enabled always stays true
				//textDescript always editable
			}
			else {//trying to edit an existing task, so need to block some things
				bool isTaskForCurUser=true;
				if(TaskCur.UserNum!=Security.CurUser.UserNum) {//current user didn't write this task, so block them.
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(TaskCur.TaskListNum!=Security.CurUser.TaskListInBox) {//the task is not in the logged-in user's inbox
					isTaskForCurUser=false;
				}
				if(isTaskForCurUser) {//this just allows getting the NoteList less often
					_listTaskNotes=TaskNotes.GetForTask(TaskCur.TaskNum);//so we can check so see if other users have added notes
					for(int i=0;i<_listTaskNotes.Count;i++) {
						if(Security.CurUser.UserNum!=_listTaskNotes[i].UserNum) {
							isTaskForCurUser=false;
							break;
						}
					}
				}
				if(!Security.IsAuthorized(EnumPermType.TaskDelete,true)) {//need to block them if they don't have TaskDelete permission
					butDelete.Enabled=false;
				}
				if(!isTaskForCurUser && !Security.IsAuthorized(EnumPermType.TaskEdit,true)) {
					butAutoNote.Enabled=false;
					butEditAutoNote.Enabled=false;
					textDescript.ReadOnly=true;
					textDescript.BackColor=System.Drawing.SystemColors.Window;
					textDescriptOverride.ReadOnly=true;
					textDescriptOverride.BackColor=System.Drawing.SystemColors.Window;
				}
			}
			_listDefsTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);//Fill list with non-hidden priorities.
			//There must be at least one priority in Setup | Definitions.  Do not let them load the task edit window without at least one priority.
			if(_listDefsTaskPriorities.Count < 1) {
				MsgBox.Show(this,"There are no task priorities in Setup | Definitions.  There must be at least one in order to use the task system.");
				DialogResult=DialogResult.Cancel;
				Close();
			}
			bool hasDefault=false;
			_defNumPrioritySelected=TaskCur.PriorityDefNum;
			if(_defNumPrioritySelected==0 && IsNew && TaskCur.ReminderType!=TaskReminderType.NoReminder) {
				for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
					if(_listDefsTaskPriorities[i].ItemValue=="R") {
						_defNumPrioritySelected=_listDefsTaskPriorities[i].DefNum;
						hasDefault=true;
						break;
					}
				}
			}
			if(_defNumPrioritySelected==0) {//The task does not yet have a priority assigned.  Find the default and assign it, if available.
				for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
					if(_listDefsTaskPriorities[i].ItemValue=="D") {
						_defNumPrioritySelected=_listDefsTaskPriorities[i].DefNum;
						hasDefault=true;
						break;
					}
				}
			}
			comboTaskPriorities.Items.Clear();
			for(int i=0;i<_listDefsTaskPriorities.Count;i++) {//Add non-hidden defs first
				comboTaskPriorities.Items.Add(_listDefsTaskPriorities[i].ItemName);
				if(_defNumPrioritySelected==_listDefsTaskPriorities[i].DefNum) {//Use priority listed within the database.
					comboTaskPriorities.SelectedIndex=i;//Sets combo text too
				}
			}
			if(_defNumPrioritySelected==0 && !hasDefault) {//If no default has been set in the definitions, select the last item in the list.
				comboTaskPriorities.SelectedIndex=comboTaskPriorities.Items.Count-1;
				_defNumPrioritySelected=_listDefsTaskPriorities[_listDefsTaskPriorities.Count-1].DefNum;
			}
			if(_taskList!=null && IsNew && _taskList.TaskListNum==1697 && PrefC.GetBool(PrefName.DockPhonePanelShow)) {//Set to triage blue if HQ, triage list, and is new.
				for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
					if(_listDefsTaskPriorities[i].DefNum==_triageNumBlue) {//Finding the option that is triageBlue to select it in the combobox (Combobox mirrors _listTaskPriorityDefs)
						comboTaskPriorities.SelectedIndex=i;
						break;
					}
				}
				_defNumPrioritySelected=_triageNumBlue;
			}
			if(comboTaskPriorities.SelectedIndex==-1) {//Priority for task wasn't found in the non-hidden priorities list (and isn't triageBlue), so it must be a hidden priority.
				List<Def> listDefsCategories=Defs.GetDefsForCategory(DefCat.TaskPriorities);//Get all priorities
				for(int i=0;i<listDefsCategories.Count;i++) {
					if(listDefsCategories[i].DefNum==_defNumPrioritySelected) {//We find the hidden priority and set the text of the combo box.
						comboTaskPriorities.DropDownStyle=ComboBoxStyle.DropDown;
						comboTaskPriorities.Text=(listDefsCategories[i].ItemName+" (Hidden)");
						butColor.BackColor=listDefsCategories[i].ItemColor;
					}
				}
			}
			textUser.Text=Userods.GetName(TaskCur.UserNum);//might be blank.
			if(_taskList!=null) {
				TaskCur.ParentDesc=_taskList.Descript;
				textTaskList.Text=_taskList.Descript;
			}
			if(TaskCur.DateTimeOriginal==DateTime.MinValue) {
				label3.Visible=false;
				textBoxDateTimeCreated.Visible=false;
			}
			else {
				textBoxDateTimeCreated.Text=TaskCur.DateTimeOriginal.ToString();
			}
			if(TaskCur.DateTimeEntry.Year<1880) {
				textDateTimeEntry.Text=DateTime.Now.ToString();
			}
			else {
				textDateTimeEntry.Text=TaskCur.DateTimeEntry.ToString();
			}
			if(TaskCur.DateTimeFinished.Year<1880) {
				textDateTimeFinished.Text="";//DateTime.Now.ToString();
			}
			else {
				textDateTimeFinished.Text=TaskCur.DateTimeFinished.ToString();
			}
			textDescript.Text=TaskCur.Descript;
			textDescriptOverride.Text=TaskCur.DescriptOverride;
			if(!IsPopup) {//otherwise, TextUser is selected, and it cannot accept input.  This prevents a popup from accepting using input accidentally.
				textDescript.Select();//Focus does not work for some reason.
			}
			long userNumMailbox=0;
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser) && TaskCur.TaskListNum !=0) {
				userNumMailbox=TaskLists.GetMailboxUserNum(TaskCur.TaskListNum);
				if(userNumMailbox != 0 && userNumMailbox != Security.CurUser.UserNum) {
					_startedInOthersInbox=true;
					checkNew.Checked=false;
					checkNew.Enabled=false;
				}
			}
			//this section must come after textDescript is set:
			if(TaskCur.TaskStatus==TaskStatusEnum.Done) {//global even if new status is tracked by user
				checkDone.Checked=true;
			}
			else {//because it can't be both new and done.
				checkDone.Checked=false;
				if(IsPopup) {//It clearly is Unread, but we don't want to leave it that way upon close OK.
					checkNew.Checked=false;
					_hasStatusChanged=true;
				}
				else if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					if(_startedInOthersInbox) {
						TaskCur.IsUnread=TaskUnreads.IsUnread(userNumMailbox,TaskCur);
					}
					else {
						TaskCur.IsUnread=TaskUnreads.IsUnread(Security.CurUser.UserNum,TaskCur);
						if(TaskCur.IsUnread) {
							checkNew.Checked=false;
							_hasStatusChanged=true;
						}
					}
				}
				else {//tracked globally, the old way
					if(TaskCur.TaskStatus==TaskStatusEnum.New) {
						TaskCur.IsUnread=true;//Not using taskunread table.
						checkNew.Checked=true;
					}
				}
			}
			groupReminder.Visible=(!PrefC.GetBool(PrefName.TasksUseRepeating));
			panelRepeating.Visible=PrefC.GetBool(PrefName.TasksUseRepeating);
			textReminderRepeatFrequency.Text=(IsNew?"1":TaskCur.ReminderFrequency.ToString());
			//Fill comboReminderRepeat with repeating options.
			_listTaskReminderTypes=new List<TaskReminderType>() {
				TaskReminderType.NoReminder,
				TaskReminderType.Once,
				TaskReminderType.Daily,
				TaskReminderType.Weekly,
				TaskReminderType.Monthly,
				TaskReminderType.Yearly
			};
			comboReminderRepeat.Items.Clear();
			for(int i=0;i<_listTaskReminderTypes.Count;i++) {
				comboReminderRepeat.Items.Add(_listTaskReminderTypes[i].ToString());
				if(TaskCur.ReminderType.HasFlag(_listTaskReminderTypes[i])) {
					comboReminderRepeat.SelectedIndex=i;
				}
			}
			if(TaskCur.ReminderType==TaskReminderType.Once) {
				if(IsNew) {
					datePickerReminder.Value=DateTime.Now;
					timePickerReminder.Value=DateTime.Now;
				}
				else {
					datePickerReminder.Value=TaskCur.DateTimeEntry;
					timePickerReminder.Value=TaskCur.DateTimeEntry;
				}
			}
			checkReminderRepeatMonday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Monday);
			checkReminderRepeatTuesday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Tuesday);
			checkReminderRepeatWednesday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Wednesday);
			checkReminderRepeatThursday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Thursday);
			checkReminderRepeatFriday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Friday);
			checkReminderRepeatSaturday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Saturday);
			checkReminderRepeatSunday.Checked=TaskCur.ReminderType.HasFlag(TaskReminderType.Sunday);
			if(TaskCur.DateTask.Year>1880) {
				textDateTask.Text=TaskCur.DateTask.ToShortDateString();
			}
			butEditAutoNote.Visible=GetHasAutoNotePrompt();
			if(TaskCur.IsRepeating) {
				checkNew.Enabled=false;
				checkDone.Enabled=false;
				textDateTask.Enabled=false;
				listObjectType.Enabled=false;
				if(TaskCur.TaskListNum!=0) {//not a main parent
					comboDateType.Enabled=false;
				}
			}
			for(int i=0;i<Enum.GetNames(typeof(TaskDateType)).Length;i++) {
				comboDateType.Items.Add(Lan.g("enumTaskDateType",Enum.GetNames(typeof(TaskDateType))[i]));
				if((int)TaskCur.DateType==i) {
					comboDateType.SelectedIndex=i;
				}
			}
			if(TaskCur.FromNum==0) {
				checkFromNum.Checked=false;
				checkFromNum.Enabled=false;
			}
			else {
				checkFromNum.Checked=true;
			}
			listObjectType.Items.Clear();
			listObjectType.Items.AddEnums<TaskObjectType>();
			_listDefsTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);//Fill list with non-hidden priorities.
			//There must be at least one priority in Setup | Definitions.  Do not let them load the task edit window without at least one priority.
			if(_listDefsTaskPriorities.Count < 1) {
				MsgBox.Show(this,"There are no task priorities in Setup | Definitions.  There must be at least one in order to use the task system.");
				DialogResult=DialogResult.Cancel;
				Close();
			}
			FillListCategory();
			FillObject();
			FillGrid();//Need this in order to pick ReplyToUserNum next.
			if(IsNew) {
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else if(_taskList==null) {
				//|| TaskListCur.TaskListNum!=Security.CurUser.TaskListInBox) {//if this task is not in my inbox
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else if(_listTaskNotes.Count==0 && TaskCur.UserNum==Security.CurUser.UserNum) {//if this is my task
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else {//reply button will be visible
				if(_listTaskNotes.Count==0) {//no notes, so reply to owner
					_userNumReplyTo=TaskCur.UserNum;
				}
				else {//reply to most recent author who is not me
					//loop backward through the notes to find who to reply to
					for(int i=_listTaskNotes.Count-1;i>=0;i--) {
						if(_listTaskNotes[i].UserNum!=Security.CurUser.UserNum) {
							_userNumReplyTo=_listTaskNotes[i].UserNum;
							break;
						}
					}
					if(_userNumReplyTo==0) {//can't figure out who to reply to.
						labelReply.Visible=false;
						butReply.Visible=false;
					}
				}
				labelReply.Text=Lan.g(this,"(Send to ")+Userods.GetName(_userNumReplyTo)+")";
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//Show red and blue buttons for HQ always
				butRed.Visible=true;
				butBlue.Visible=true;
			}
			if(!Security.IsAuthorized(EnumPermType.TaskEdit,true)){
				butAudit.Visible=false;
			}
			SetTaskStartingLocation();
			_userNumFrom=_taskOld.UserNum;
			_patNum=0;
			if(_taskOld.ObjectType==TaskObjectType.Patient && _taskOld.KeyNum>0) {//Patient is attached.
				_patNum=_taskOld.KeyNum;
			}
			checkIsReadOnly.Checked=TaskCur.IsReadOnly;
			//User is only allowed to set tasks as read only if they have permission or created the task.
			checkIsReadOnly.Enabled=Tasks.IsAuthorizedOrOwner(TaskCur);
			if(!PrefC.IsODHQ) {
				checkIsReadOnly.Visible=false;
			}
			if(!TaskCur.IsReadOnly) {
				return;
			}
			//If the task is read only, initially disable all of the fields.
			SetFormReadOnly(true);
			butSend.Enabled=Tasks.IsAuthorizedOrOwner(TaskCur);
		}

		///<summary>Sets the starting location of this form. Should only be called on load.
		///The first Task window will default to the primary monitor. After that we will cascade down.
		///If FormOpenDental is not minimized, the first Task will instead appear in the center of wherever it is.
		///If any part of this form will be off screen we will default the next task to the top left of the monitor.</summary>
		private void SetTaskStartingLocation() { 
			List<FormTaskEdit> listFormTaskEdits=Application.OpenForms.OfType<FormTaskEdit>().ToList();
			Point pointStart;
			this.StartPosition=FormStartPosition.Manual;
			//Since this form has already gone through the initilize, there will be at least 1. Except when running unit tests.
			if(listFormTaskEdits.Count<=1) {
				FormOpenDental formOpenDental=Application.OpenForms.OfType<FormOpenDental>().ToList().FirstOrDefault();//Should be exactly 1.
				if(formOpenDental!=null) { //Should never be null.
					pointStart=formOpenDental.Location;
					pointStart.X+=(formOpenDental.Width/2)-(this.Width/2);
					pointStart.Y+=(formOpenDental.Height/2)-(this.Height/2);
					System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
					if(screenArray.Any(x => x.WorkingArea.Contains(pointStart))) {//Make sure the task would actually be visible in the manual location.
						this.Location=pointStart;
					}
				}
				return;
			}
			//There are multiple task edit windows open, so offset the new window by a little so that it does not show up directly over the old one.
			const int OFFSET=20;//Sets how far to offset the cascaded form location.
			this.StartPosition=FormStartPosition.Manual;
			//Count is 1 based, list index is 0 based, -2 to get the "last" window
			FormTaskEdit formTaskEditPrevious=listFormTaskEdits[listFormTaskEdits.Count-2];
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.PrimaryScreen;
			//Figure out what monitor the previous task edit window is on.
			if(formTaskEditPrevious!=null && !formTaskEditPrevious.IsDisposed && formTaskEditPrevious.WindowState!=FormWindowState.Minimized) {
				screen=System.Windows.Forms.Screen.FromControl(formTaskEditPrevious);
				//Get the start point relative to the screen the form will open on.
				//pointStart=new Point(formPrevious.Location.X-screenCur.Bounds.Left,formPrevious.Location.Y-screenCur.Bounds.Top);
				pointStart=formTaskEditPrevious.Location;
			}
			else {
				pointStart=new Point(screen.WorkingArea.X,screen.WorkingArea.Y);
			}
			//Temporarily apply the offset and see if that rectangle can fit on screenCur, if not, default to a high location on the primary screen.
			pointStart.X+=OFFSET;
			pointStart.Y+=OFFSET;
			Rectangle rectangle=new Rectangle(pointStart,this.Size);
			if(!screen.WorkingArea.Contains(rectangle)) {
				//A portion of the new window is outside of the usable area on the current monitor.
				//Force the new window to be at "0,50" (relatively) in regards to the primary monitor.
				pointStart=new Point(screen.WorkingArea.X,screen.WorkingArea.Y+50);
			}
			this.Location=pointStart;
		}

		private void FillComboJobs() {
			if(_isTaskDeleted) {
				return;
			}
			if(!PrefC.IsODHQ) {
				return;
			}
			_isLoading=true;
			comboJobs.Items.Clear();
			_listJobLinks = JobLinks.GetForTask(this.TaskCur.TaskNum);
			_listJobs = Jobs.GetMany(_listJobLinks.Select(x => x.JobNum).ToList());
			for(int i=0;i<_listJobs.Count;i++) {
				comboJobs.Items.Add(_listJobs[i].ToString());
			}
			if(_listJobs.Count==0) {
				comboJobs.Items.Add("None");
				if(JobPermissions.IsAuthorized(JobPerm.Concept,true)) {
					butCreateJob.Visible=true;
				}
			}
			if(!checkIsReadOnly.Checked) {
				comboJobs.Items.Add("Attach");
			}
			comboJobs.SelectedIndex=0;
			labelJobs.Text=Lan.g(this,"Jobs")+" ("+_listJobs.Count+")";
			_isLoading=false;
		}

		private void comboJobs_SelectedIndexChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(comboJobs.SelectedIndex<1 && _listJobs.Count==0) {
				return;//selected none
			}
			if(comboJobs.SelectedIndex!=comboJobs.Items.Count-1 || checkIsReadOnly.Checked) {
				FormOpenDental.S_GoToJob(_listJobs[comboJobs.SelectedIndex].JobNum);
				return;
			}
			if(Tasks.IsTaskDeleted(TaskCur.TaskNum)){
				SetFormToDeletedMode();
				MsgBox.Show(this,"This task was deleted from elsewhere.  Cannot attach to job.");
				return;
			}
			//Atach new job
			using FormJobSearch formJobSearch=new FormJobSearch();
			formJobSearch.DoBlockProjects=true;
			formJobSearch.ShowDialog();
			if(formJobSearch.DialogResult!=DialogResult.OK || formJobSearch.SelectedJobNum==0) {
				return;
			}
			JobLink jobLink=new JobLink();
			jobLink.JobNum=formJobSearch.SelectedJobNum;
			jobLink.FKey=TaskCur.TaskNum;
			jobLink.LinkType=JobLinkType.Task;
			JobLinks.Insert(jobLink);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobLink.JobNum);
			FillComboJobs();
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,TaskCur.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(TaskCur,_listTaskNotes,new List<long>() { signalNum });
		}

		private void FillGrid() {
			if(_isTaskDeleted) {
				return;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date Time"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"User"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),400);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			_listTaskNotes=TaskNotes.GetForTask(TaskCur.TaskNum);
			//Only do weird logic when editing a task associated with the triage task list.
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				if(_numNotes==-1) {//Only fill _numNotes here the first time FillGrid is called.  This is used for coloring triage tasks.
					_numNotes=_listTaskNotes.Count;
				}
				if(_defNumPrioritySelected==_triageNumBlue && _numNotes==0 && _listTaskNotes.Count!=0) {//Blue triage task with an added note
					_defNumPrioritySelected=_triageNumWhite;//Change priority to white
					for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
						if(_listDefsTaskPriorities[i].DefNum==_triageNumWhite) {
							comboTaskPriorities.SelectedIndex=i;//Change selection to the triage white
						}
					}
				}
				else if(_defNumPrioritySelected==_triageNumWhite && _numNotes!=0 && _listTaskNotes.Count==0) {//White triage task with note that has been deleted, change it back to blue.
					_defNumPrioritySelected=_triageNumBlue;
					for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
						if(_listDefsTaskPriorities[i].DefNum==_triageNumBlue) {
							comboTaskPriorities.SelectedIndex=i;//Change selection to the triage blue
						}
					}
				}
				_numNotes=_listTaskNotes.Count;
			}
			for(int i=0;i<_listTaskNotes.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listTaskNotes[i].DateTimeNote.ToShortDateString()+" "+_listTaskNotes[i].DateTimeNote.ToShortTimeString());
				row.Cells.Add(Userods.GetName(_listTaskNotes[i].UserNum));
				row.Cells.Add(_listTaskNotes[i].Note);
				row.Tag=_listTaskNotes[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(checkIsReadOnly.Checked) {	//If read only, dont open formTaskNoteEdit
				return;
			}
			if(_isTaskDeleted) {
				return;//The user can copy text with right click, or copy button.
			}
			if(_formTaskNoteEdit!= null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			_formTaskNoteEdit=new FormTaskNoteEdit();
			_formTaskNoteEdit.TaskNoteCur=(TaskNote)gridMain.ListGridRows[e.Row].Tag;
			_formTaskNoteEdit.FormClosed+=FormTaskNoteEdit_FormClosed_DoubleClick;
			_formTaskNoteEdit.Show(this);//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.
		}

		/// <summary>Adds a new note to the task, and opens it for editing by the user.  Will not open if the task is not visible, is deleted, 
		/// or if a child form is already open.</summary>
		public bool AddNoteToTaskAndEdit(string initialText="") {
			//We only want to show the note edit window if the task is visible, it's not deleted, and another window from this task isn't open.
			if(!this.Visible || _isTaskDeleted){ 
				return false;
			}
			if(_formTaskNoteEdit!=null && !_formTaskNoteEdit.IsDisposed) {//if a window is already open
				return false;
			}
			_formTaskNoteEdit=new FormTaskNoteEdit();
			_formTaskNoteEdit.TaskNoteCur=new TaskNote();
			_formTaskNoteEdit.TaskNoteCur.TaskNum=TaskCur.TaskNum;
			_formTaskNoteEdit.TaskNoteCur.DateTimeNote=DateTime.Now;//Will be slightly adjusted at server.
			_formTaskNoteEdit.TaskNoteCur.UserNum=Security.CurUser.UserNum;
			_formTaskNoteEdit.TaskNoteCur.IsNew=true;
			_formTaskNoteEdit.TaskNoteCur.Note=initialText;
			_formTaskNoteEdit.FormClosed+=FormTaskNoteEdit_FormClosed_Edit;
			//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.
			_formTaskNoteEdit.Show(this);
			return true;
		}

		private void FormTaskNoteEdit_FormClosed_DoubleClick(object sender,EventArgs e) {
			if(_formTaskNoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			DidNotesChange=true;
			if(_taskOld.TaskStatus==TaskStatusEnum.Done) {//If task was marked Done when opened, we uncheck the Done checkbox so people can see the changes.
				checkDone.Checked=false;
			}
			FillGrid();
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(TaskCur);
		}

		private void butAddNote_Click(object sender,EventArgs e) {
			if(_formTaskNoteEdit!=null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			AddNoteToTaskAndEdit();
		}

		private void FormTaskNoteEdit_FormClosed_Edit(object sender,EventArgs e) {
			if(_formTaskNoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			DidNotesChange=true;
			if(_taskOld.TaskStatus==TaskStatusEnum.Done) {//If task was marked Done when opened, we uncheck the Done checkbox so people can see the changes.
				checkDone.Checked=false;
			}
			FillGrid();
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(TaskCur);
		}

		private void checkNew_Click(object sender,EventArgs e) {
			if(checkNew.Checked && checkDone.Checked) {
				checkDone.Checked=false;
			}
			_hasStatusChanged=true;
		}

		private void checkDone_Click(object sender,EventArgs e) {
			if(checkNew.Checked && checkDone.Checked) {
				checkNew.Checked=false;
			}
		}

		private void FillObject() {
			if(_isTaskDeleted) {
				return;
			}
			listObjectType.SetSelectedEnum(TaskCur.ObjectType);
			if(TaskCur.ObjectType==TaskObjectType.None) {
				panelObject.Visible=false;
				return;
			}
			if(TaskCur.ObjectType==TaskObjectType.Patient) {
				panelObject.Visible=true;
				labelObjectDesc.Text=Lan.g(this,"Patient Name");
				if(TaskCur.KeyNum>0) {
					TaskCur.PatientName=Patients.GetPat(TaskCur.KeyNum).GetNameLF();
					textObjectDesc.Text=TaskCur.PatientName+" - "+TaskCur.KeyNum;
				}
				else {
					textObjectDesc.Text="";
				}
				return;
			}
			if(TaskCur.ObjectType!=TaskObjectType.Appointment) {
				return;
			}
			panelObject.Visible=true;
			labelObjectDesc.Text=Lan.g(this,"Appointment Desc");
			if(TaskCur.KeyNum<=0) {
				textObjectDesc.Text="";
				return;
			}
			Appointment appointment=Appointments.GetOneApt(TaskCur.KeyNum);
			if(appointment==null) {
				textObjectDesc.Text=Lan.g(this,"(appointment deleted)");
				return;
			}
			textObjectDesc.Text=Patients.GetPat(appointment.PatNum).GetNameLF()
				+"  "+appointment.AptDateTime.ToString()
				+"  "+appointment.ProcDescript
				+"  "+appointment.Note;
		}

		private void FillListCategory() {
			listCategory.Items.Clear();
			List<Def> listDefsCategories=Defs.GetDefsForCategory(DefCat.TaskCategories);
			listDefsCategories=listDefsCategories.FindAll(x => !x.IsHidden || x.DefNum==TaskCur.TriageCategory);
			listCategory.Items.AddNone<Def>();
			listCategory.Items.AddList(listDefsCategories,x => x.ItemName);
			listCategory.SetSelectedKey<Def>(TaskCur.TriageCategory,x => x.DefNum);
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateTimeEntry.Text=MiscData.GetNowDateTime().ToString();
		}

		private void butNowFinished_Click(object sender,EventArgs e) {
			textDateTimeFinished.Text=MiscData.GetNowDateTime().ToString();
		}

		private void comboReminderRepeat_SelectedIndexChanged(object sender,EventArgs e) {
			RefreshReminderGroup();
		}

		private void textReminderRepeatFrequency_KeyUp(object sender,KeyEventArgs e) {
			RefreshReminderGroup();
		}

		private void RefreshReminderGroup() {
			TaskReminderType taskReminderType=_listTaskReminderTypes[comboReminderRepeat.SelectedIndex];
			panelReminderFrequency.Visible=true;
			panelReminderDays.Visible=false;
			datePickerReminder.Visible=false;
			timePickerReminder.Visible=false;
			int reminderFrequency=PIn.Int(textReminderRepeatFrequency.Text,false);
			if(taskReminderType==TaskReminderType.NoReminder) {
				panelReminderFrequency.Visible=false;
				return;
			}
			if(taskReminderType==TaskReminderType.Once) {
				panelReminderFrequency.Visible=false;
				datePickerReminder.Visible=true;
				timePickerReminder.Visible=true;
				return;
			}
			if(taskReminderType==TaskReminderType.Daily) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Day");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Days");
				}
				return;
			}
			if(taskReminderType==TaskReminderType.Weekly) {
				panelReminderDays.Visible=true;
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Week");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Weeks");
				}
				return;
			}
			if(taskReminderType==TaskReminderType.Monthly) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Month");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Months");
				}
				return;
			}
			if(taskReminderType==TaskReminderType.Yearly) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Year");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Years");
				}
			}
		}

		private void butBlue_Click(object sender,EventArgs e) {
			if(_defNumPrioritySelected==_triageNumBlue) {//Blue button is clicked while it's already blue
				_defNumPrioritySelected=_triageNumWhite;//Change to white.
				for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
					if(_listDefsTaskPriorities[i].DefNum==_triageNumWhite) {
						comboTaskPriorities.SelectedIndex=i;//Change selection to the triage white
					}
				}
				return;
			}
			//Blue button is clicked while it's red or white, simply change it to blue
			_defNumPrioritySelected=_triageNumBlue;//Change to blue.
			for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
				if(_listDefsTaskPriorities[i].DefNum==_triageNumBlue) {
					comboTaskPriorities.SelectedIndex=i;//Change selection to the triage blue
				}
			}
		}

		private void butRed_Click(object sender,EventArgs e) {
			if(_defNumPrioritySelected==_triageNumRed) {//Red button is clicked while it's already red
				_defNumPrioritySelected=_triageNumWhite;//Change to white.
				for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
					if(_listDefsTaskPriorities[i].DefNum==_triageNumWhite) {
						comboTaskPriorities.SelectedIndex=i;//Change combo selection to the triage white
					}
				}
				return;
			}
			//Red button is clicked while it's blue or white, simply change it to red
			_defNumPrioritySelected=_triageNumRed;//Change to red.
			for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
				if(_listDefsTaskPriorities[i].DefNum==_triageNumRed) {
					comboTaskPriorities.SelectedIndex=i;//Change combo selection to the triage red
				}
			}
			if(_taskList!=null && _taskList.TaskListNum==Tasks.TriageTaskListNum) {
				textDateTimeEntry.Text=MiscData.GetNowDateTime().ToString();
			}
		}

		private void butAutoNote_Click(object sender,EventArgs e) {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.ShowDialog();
			if(!frmAutoNoteCompose.IsDialogOK) {
				return;
			}
			textDescript.AppendText(frmAutoNoteCompose.StrCompletedNote);
			butEditAutoNote.Visible=GetHasAutoNotePrompt();
		}

		///<summary>This event is fired whenever the combo box is changed manually or the index is changed programmatically.</summary>
		private void comboTaskPriorities_SelectedIndexChanged(object sender,EventArgs e) {
			_defNumPrioritySelected=_listDefsTaskPriorities[comboTaskPriorities.SelectedIndex].DefNum;
			butColor.BackColor=Defs.GetColor(DefCat.TaskPriorities,_defNumPrioritySelected);//Change the color swatch so people know the priority's color
		}

		private void comboTaskPriorities_SelectionChangeCommitted(object sender,EventArgs e) {
			if(PrefC.IsODHQ 
				//Changing the priority to 'Red' from another priority for a Triage task
				&& _listDefsTaskPriorities[comboTaskPriorities.SelectedIndex].DefNum==_triageNumRed 
				&& TaskCur.PriorityDefNum!=_triageNumRed
				&& _taskList!=null && _taskList.TaskListNum==Tasks.TriageTaskListNum) 
			{
				textDateTimeEntry.Text=MiscData.GetNowDateTime().ToString();
			}
		}

		private void listCategory_SelectionChangeCommitted(object sender,EventArgs e) {
			Def defSelectedCategory=listCategory.GetSelected<Def>();
			if(defSelectedCategory==null) {
				MsgBox.Show(this,"Invalid Triage Category. Please select a valid Triage Category.");
				return;
			}
			TaskCur.TriageCategory=defSelectedCategory.DefNum;
		}

		private void listObjectType_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(TaskCur.KeyNum>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The linked object will no longer be attached.  Continue?")) {
					FillObject();
					return;
				}
			}
			TaskCur.KeyNum=0;
			TaskCur.ObjectType=listObjectType.GetSelected<TaskObjectType>();
			FillObject();
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(Tasks.IsTaskDeleted(TaskCur.TaskNum)){
				SetFormToDeletedMode();
				MsgBox.Show(this,"Task has been deleted, no history can be retrieved.");
				return;
			}
			FormTaskHist formTaskHist=new FormTaskHist();
			formTaskHist.TaskNum=TaskCur.TaskNum;
			formTaskHist.Show();
		}

		private void butSubscribers_Click(object sender,EventArgs e) {
			FormTaskSubscribers formTaskSubscribers=new FormTaskSubscribers();
			formTaskSubscribers.TaskForSubscribers=TaskCur;
			formTaskSubscribers.Show();//This is a read-only form
		}

		private void butChange_Click(object sender,System.EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(TaskCur.ObjectType==TaskObjectType.Patient) {
				TaskCur.KeyNum=formPatientSelect.PatNumSelected;
			}
			if(TaskCur.ObjectType!=TaskObjectType.Appointment) {
				FillObject();
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(formPatientSelect.PatNumSelected,null);//Select only, can't create new appt so don't need pinboard appointments.
			formApptsOther.AllowSelectOnly=true;
			formApptsOther.ShowDialog();
			if(formApptsOther.DialogResult!=DialogResult.OK) {
				return;
			}
			TaskCur.KeyNum=formApptsOther.ListAptNumsSelected[0];
			FillObject();
		}

		private void butGoto_Click(object sender,System.EventArgs e) {
			if(!TrySaveAndShouldClose()) {
				return;
			}
			TaskObjectTypeGoTo=TaskCur.ObjectType;
			KeyNumGoTo=TaskCur.KeyNum;
			Close();
			FormOpenDental.S_TaskGoTo(TaskObjectTypeGoTo,KeyNumGoTo);
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			if(IsNew) {
				MessageBox.Show(Lan.g(this,"From User cannot be changed on new tasks. Save the task first."));
				return;
			}
			using FormLogOn formLogOn=new FormLogOn(isSimpleSwitch:true);
			formLogOn.ShowDialog();
			if(formLogOn.DialogResult!=DialogResult.OK) {
				return;
			}
			TaskCur.UserNum=formLogOn.UserodSimpleSwitch.UserNum; //assign task new UserNum
			textUser.Text=Userods.GetName(TaskCur.UserNum); //update user textbox on task.
		}

		private void textDescript_TextChanged(object sender,EventArgs e) {
			if(_taskOld.TaskStatus==TaskStatusEnum.Done && textDescript.Text!=_taskOld.Descript) {
				checkDone.Checked=false;
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(CreateCopyTask());
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
				ex.DoNothing();
				return;
			}
			Tasks.TaskEditCreateLog(Lan.g(this,"Copied Task Note"),TaskCur);
		}

		private string CreateCopyTask() {
			string taskText=
				Lan.g(this,"Tasknum")+" #"+TaskCur.TaskNum //tasknum
				+((TaskCur.ObjectType==TaskObjectType.Patient && TaskCur.KeyNum!=0) ? (" - "+Lan.g(this,"Patnum")+" #"+TaskCur.KeyNum) : ("")) //patnum
				+"\r\n"+TaskCur.DateTimeEntry.ToShortDateString() //date
				+" "+TaskCur.DateTimeEntry.ToShortTimeString() //time
				+(textObjectDesc.Visible?" - "+textObjectDesc.Text:"")//patient name, time, etc
				+" - "+textUser.Text //user name
				+" - "+textDescript.Text; //task desc
			for(int i=0;i<_listTaskNotes.Count;i++) {
				taskText+="\r\n--------------------------------------------------\r\n";
				taskText+="=="+Userods.GetName(_listTaskNotes[i].UserNum)+" - "+_listTaskNotes[i].DateTimeNote.ToShortDateString()+" "+_listTaskNotes[i].DateTimeNote.ToShortTimeString()+" - "+_listTaskNotes[i].Note;
			}
			return taskText;
		}

		private void butCreateJob_Click(object sender,EventArgs e) {
			if(Tasks.IsTaskDeleted(TaskCur.TaskNum)){
				SetFormToDeletedMode();
				MsgBox.Show(this,"This task was deleted from elsewhere.  Cannot attach to job.");
				return;
			}
			Job jobNew=new Job();
			List<string> listCategories=Enum.GetNames(typeof(JobCategory)).ToList();
			//Queries can't be created from here
			listCategories.Remove(JobCategory.Query.ToString());
			//Projects can't be created from here
			listCategories.Remove(JobCategory.Project.ToString());
			listCategories.Remove(JobCategory.MarketingDesign.ToString());
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				listCategories.Remove(JobCategory.NeedNoApproval.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.SpecialProject,true)) {
				listCategories.Remove(JobCategory.SpecialProject.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true)) {
				listCategories.Remove(JobCategory.UnresolvedIssue.ToString());
			}
			using InputBox inputBoxCategoryChoose=new InputBox("Choose a job category",listCategories);
			if(inputBoxCategoryChoose.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(inputBoxCategoryChoose.comboSelection.SelectedIndex==-1) {
				MsgBox.Show(this,"You must choose a category to create a job.");
				return;
			}
			JobCategory jobCategory=(JobCategory)Enum.GetNames(typeof(JobCategory)).ToList().IndexOf(inputBoxCategoryChoose.comboSelection.GetSelected<string>());
			jobNew.Category=jobCategory;
			using InputBox inputBoxTitle=new InputBox("Provide a brief title for the job.");
			if(inputBoxTitle.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(String.IsNullOrEmpty(inputBoxTitle.textResult.Text)) {
				MsgBox.Show(this,"You must type a title to create a job.");
				return;
			}
			List<Def> listDefsJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listDefsJobPriorities.Count==0) {
				MsgBox.Show(this,"You have no priorities setup in definitions.");
				return;
			}
			jobNew.Title=inputBoxTitle.textResult.Text;
			long priorityNum=0;
			priorityNum=listDefsJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("JobDefault")).DefNum;
			if(jobNew.Category.In(JobCategory.Bug,JobCategory.Conversion,JobCategory.UnresolvedIssue)) {
				priorityNum=listDefsJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault")).DefNum;
				jobNew.Requirements=CreateCopyTask();
			}
			else {
				jobNew.Requirements=CreateCopyTask();
			}
			jobNew.Priority=priorityNum==0?listDefsJobPriorities.First().DefNum:priorityNum;
			jobNew.PhaseCur=JobPhase.Concept;
			jobNew.UserNumConcept=Security.CurUser.UserNum;
			jobNew.ProposedVersion=JobProposedVersion.Current;
			JobLink jobLinkNew=new JobLink();
			JobLink jobLinkTask=new JobLink();
			jobLinkTask.LinkType=JobLinkType.Task;
			jobLinkTask.FKey=TaskCur.TaskNum;
			Jobs.Insert(jobNew);
			jobLinkNew.JobNum=jobNew.JobNum;
			jobLinkTask.JobNum=jobNew.JobNum;
			JobLinks.Insert(jobLinkTask);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
			FillComboJobs();
			FormOpenDental.S_GoToJob(jobNew.JobNum);
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,TaskCur.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(TaskCur,_listTaskNotes,new List<long>() { signalNum });
		}

		private void butAttachmnents_Click(object sender,EventArgs e) {
			TaskCur.IsNew=IsNew;
			using FormTaskAttachments formTaskAttachments=new FormTaskAttachments(TaskCur);
			//If the user does not heave edit read only permission, and the task it read only, set attachment form to read only.
			if(checkIsReadOnly.Checked) {
				formTaskAttachments.IsReadOnly=true;
			}
			formTaskAttachments.ShowDialog();
			if(formTaskAttachments.DialogResult!=DialogResult.OK) {
				return;
			}
			FillTextAttachments();
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,TaskCur.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(TaskCur,_listTaskNotes,new List<long>() { signalNum });
		}

		private void FillTextAttachments() {
			_listTaskAttachments=TaskAttachments.GetManyByTaskNum(TaskCur.TaskNum);
			textAttachments.Text=string.Join(", ",_listTaskAttachments.Select(x => x.Description));
			labelAttachments.Text=Lan.g(this,"Attachments")+$" ({_listTaskAttachments.Count})";
		}

		public void OnTaskEdited() {
			//This gets hit even when this window is the one that made a change.  This means we will refresh the grid even though we just did.
			//In the future we might think about enhancing this window to keep track of which signals it inserted.
			if(IsNew) {
				return;//If this task is new then no one else can edit it
			}
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start,"TaskNum: "+TaskCur.TaskNum.ToString());
			Task taskDb=Tasks.GetOne(TaskCur.TaskNum);
			if(taskDb==null) {//Task was deleted
				SetFormToDeletedMode();
				Logger.LogToPath("",LogPath.Signals,LogPhase.End,"TaskNum: "+TaskCur.TaskNum.ToString());
				return;
			}
			if(!taskDb.Equals(_taskOld)) {
				butRefresh.Visible=true;
				labelTaskChanged.Visible=true;
			}
			_taskOld=taskDb;
			FillGrid();
			FillComboJobs();
			FillObject();
			FillTextAttachments();
			Logger.LogToPath("",LogPath.Signals,LogPhase.End,"TaskNum: "+TaskCur.TaskNum.ToString());
		}

		///<summary>Does validation and then updates the _taskCur object with the current content of the TaskEdit window.</summary>
		private bool SaveCur() {
			if(!textDateTask.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			DateTime dateTimeEntry=DateTime.MinValue;
			TaskReminderType taskReminderType=_listTaskReminderTypes[comboReminderRepeat.SelectedIndex];
			if(textDateTimeEntry.Text!="" || taskReminderType!=TaskReminderType.NoReminder) {//Reminders always require a DateTimeEntry
				try {
					dateTimeEntry=DateTime.Parse(textDateTimeEntry.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix Date/Time Entry.");
					return false;
				}
			}
			if(textDateTimeFinished.Text!="") {
				try {
					DateTime.Parse(textDateTimeFinished.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix Date/Time Finished.");
					return false;
				}
			}
			if(TaskCur.TaskListNum==-1) {
				MsgBox.Show(this,"Since no task list is selected, the Send To button must be used.");
				return false;
			}
			if(textDescript.Text=="") {
				MsgBox.Show(this,"Please enter a description.");
				return false;
			}
			if(taskReminderType!=TaskReminderType.NoReminder && !PrefC.GetBool(PrefName.TasksUseRepeating)) {//Is a reminder and not using legacy task system
				if(taskReminderType!=TaskReminderType.Once &&
					(!textReminderRepeatFrequency.IsValid() || PIn.Int(textReminderRepeatFrequency.Text)<1)) 
				{
					MsgBox.Show(this,"Reminder frequency must be a positive number.");
					return false;
				}
				if(taskReminderType==TaskReminderType.Weekly) {
					if(!checkReminderRepeatMonday.Checked && !checkReminderRepeatTuesday.Checked
						&& !checkReminderRepeatWednesday.Checked && !checkReminderRepeatThursday.Checked && !checkReminderRepeatFriday.Checked
						&& !checkReminderRepeatSaturday.Checked && !checkReminderRepeatSunday.Checked)
					{
						MsgBox.Show(this,"Since the weekly reminder repeat option is selected, at least one day option must be chosen.");
						return false;
					}
					if(checkReminderRepeatMonday.Checked) {
						taskReminderType|=TaskReminderType.Monday;
					}
					if(checkReminderRepeatTuesday.Checked) {
						taskReminderType|=TaskReminderType.Tuesday;
					}
					if(checkReminderRepeatWednesday.Checked) {
						taskReminderType|=TaskReminderType.Wednesday;
					}
					if(checkReminderRepeatThursday.Checked) {
						taskReminderType|=TaskReminderType.Thursday;
					}
					if(checkReminderRepeatFriday.Checked) {
						taskReminderType|=TaskReminderType.Friday;
					}
					if(checkReminderRepeatSaturday.Checked) {
						taskReminderType|=TaskReminderType.Saturday;
					}
					if(checkReminderRepeatSunday.Checked) {
						taskReminderType|=TaskReminderType.Sunday;
					}
				}
				TaskCur.ReminderType=taskReminderType;
				TaskCur.ReminderFrequency=0;
				if(taskReminderType!=TaskReminderType.Once) {
					TaskCur.ReminderFrequency=PIn.Int(textReminderRepeatFrequency.Text);
				}
				if(String.IsNullOrEmpty(TaskCur.ReminderGroupId)) {//Make a new ID if it's blank no matter what.  Could be an old task being changed.
					Tasks.SetReminderGroupId(TaskCur);
				}
			}
			else {
				TaskCur.ReminderType=TaskReminderType.NoReminder;
				TaskCur.ReminderGroupId="";
			}
			if(taskReminderType==TaskReminderType.Once) {
				TaskCur.DateTimeEntry=new DateTime(datePickerReminder.Value.Date.Year,datePickerReminder.Value.Date.Month,datePickerReminder.Value.Date.Day,
					timePickerReminder.Value.TimeOfDay.Hours,timePickerReminder.Value.TimeOfDay.Minutes,timePickerReminder.Value.TimeOfDay.Seconds);
			}
			else {
				TaskCur.DateTimeEntry=PIn.DateT(textDateTimeEntry.Text);
				if(taskReminderType!=TaskReminderType.NoReminder && IsNew && DateTime.Now>TaskCur.DateTimeEntry) { //New Reminder Task.
					//Could be a future reminder, so we want to calculate when the reminder would be due.
					TaskCur.DateTimeEntry=Tasks.CalcTaskForwardDate(TaskCur);
				}
			}
			//Techs want to be notified of any changes made to completed tasks.
			//Check if the task list changed on a task marked Done.
			if(TaskCur.TaskListNum!=_taskOld.TaskListNum && _taskOld.TaskStatus==TaskStatusEnum.Done) {
				//Forcing the status to viewed will put the task in other user's "New for" task list but not the user that made the change.
				TaskCur.TaskStatus=TaskStatusEnum.Viewed;
				checkDone.Checked=false;
			}
			if(checkDone.Checked) {
				TaskCur.TaskStatus=TaskStatusEnum.Done;//global even if new status is tracked by user
				TaskUnreads.DeleteForTask(TaskCur);//clear out taskunreads. We have too many tasks to read the done ones.
			}
			else {//because it can't be both new and done.
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					if(TaskCur.TaskStatus==TaskStatusEnum.Done) {
						TaskCur.TaskStatus=TaskStatusEnum.Viewed;
					}
					if(Tasks.IsReminderTask(TaskCur) && TaskCur.DateTimeEntry>DateTime.Now) {//Future Reminder.
						if(!TaskCur.IsUnread) {
							TaskUnreads.SetUnread(Security.CurUser.UserNum,TaskCur);//Future Reminders need to stay Unread for this user so they popup at due time.
						}
					}
					else if(!_startedInOthersInbox) {
						//Because the task could have been modified by another user at this point
						//RefreshTask();
						if(checkNew.Checked) {
							TaskUnreads.SetUnread(Security.CurUser.UserNum,TaskCur);
						}
						else {
							TaskUnreads.SetRead(Security.CurUser.UserNum,TaskCur);
						}
					}
					else if(!checkNew.Checked) {//Just in case, checkbox should not be enabled or checked when _startedInOthersInbox is true.
						//A task was sent to a tasklist I'm subscribed to, but before I had a chance to open it, it was sent to someone else's inbox.
						//If I open this task, I've read it, so it should still be marked as read for me.
						TaskUnreads.SetRead(Security.CurUser.UserNum,TaskCur);
						_hasStatusChanged=true;
					}
				}
				else {//tracked globally, the old way
					if(checkNew.Checked) {
						TaskCur.TaskStatus=TaskStatusEnum.New;
					}
					else {
						TaskCur.TaskStatus=TaskStatusEnum.Viewed;
					}
				}
			}
			//UserNum no longer allowed to change automatically
			//if(resetUser && TaskCur.Descript!=textDescript.Text){
			//	TaskCur.UserNum=Security.CurUser.UserNum;
			//}
			if(TaskCur.TaskStatus==TaskStatusEnum.Done && textDateTimeFinished.Text=="") {
				TaskCur.DateTimeFinished=DateTime.Now;
			}
			else {
				TaskCur.DateTimeFinished=PIn.DateT(textDateTimeFinished.Text);
			}
			TaskCur.Descript=textDescript.Text;
			TaskCur.DescriptOverride=textDescriptOverride.Text;
			TaskCur.DateTask=PIn.Date(textDateTask.Text);
			TaskCur.DateType=(TaskDateType)comboDateType.SelectedIndex;
			TaskCur.IsReadOnly=checkIsReadOnly.Checked;
			//Original task was read only, updated  task is not.
			if(!TaskCur.IsReadOnly && _taskOld!=null && _taskOld.IsReadOnly) {
				bool isReadOnly=MsgBox.Show(MsgBoxButtons.YesNo,"Original task was read only. Set updated task to read only?");
				TaskCur.IsReadOnly=isReadOnly;
			}
			if(!checkFromNum.Checked) {//user unchecked the box. Never allowed to check if initially unchecked
				TaskCur.FromNum=0;
			}
			//ObjectType already handled
			//Cur.KeyNum already handled
			TaskCur.PriorityDefNum=_defNumPrioritySelected;
			if(IsNew) {
				TaskCur.IsNew=true;
				try {
					Tasks.Update(TaskCur,_taskOld);
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return false;
				}
				return true;
			}
			if(butRefresh.Visible) {
				//force them to refresh before pressing ok.
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"There have been changes to the task since it has been loaded. "+
				" You must refresh before saving. Would you like to refresh now?")) 
				{
					RefreshTask();
				}
				return false;
			}
			if(!TaskCur.Equals(_taskOld)) {//If user clicks OK without making any changes, then skip.
				Cursor=Cursors.WaitCursor;
				try {
					Tasks.Update(TaskCur,_taskOld);//if task has already been altered, then this is where it will fail.
				}	
				catch(Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return false;
				}
				Cursor=Cursors.Default;
			}
			if(TaskCur.Equals(_taskOld) && !DidNotesChange) {
				return true;
			}
			//We want to make a TaskHist entry if notes were changed as well as if the task was changed.
			TaskHist taskHist=new TaskHist(_taskOld);
			taskHist.UserNumHist=Security.CurUser.UserNum;
			taskHist.IsNoteChange=DidNotesChange;
			try {
				TaskHists.Insert(taskHist);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		///<summary>Tries to save the task based on the current state of the window. Warns the user if anything is wrong.
		///Sets the DialogResult correctly and sends task signals if necessary.
		///Returns true if the calling method should close the window. Otherwise false, which indicates that the window should stay open.</summary>
		private bool TrySaveAndShouldClose() {
			if(_formTaskNoteEdit != null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return false;
			}
			if(!SaveCur()) {//If user clicked OK without changing anything, then this will have no effect.
				return false;
			}
			if(TaskCur.Equals(_taskOld) && !_hasStatusChanged) {//if there were no changes, then don't bother with the signal
				DialogResult=DialogResult.OK;
				return true;
			}
			if(IsNew || textDescript.Text!=TaskCur.Descript //notes or descript changed
				|| (DidNotesChange && _taskOld.TaskStatus==TaskStatusEnum.Done) //Because the taskunread would not have been inserted when saving the note
				|| (_taskOld.ReminderType==TaskReminderType.NoReminder && TaskCur.ReminderType!=TaskReminderType.NoReminder)) //Add taskunread for when "due"
			{
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
				TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			}
			SendSignalsRefillLocal(TaskCur);
			DialogResult=DialogResult.OK;
			return true;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshTask();
		}

		private void RefreshTask() {
			if(TaskCur==null) {
				MsgBox.Show(this,"This task is in an invalid state. The task will now be closed so it can be opened again in a valid state.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			TaskCur=Tasks.GetOne(TaskCur.TaskNum);
			if(TaskCur==null) {
				MsgBox.Show(this,"This task has been deleted and must be closed.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			_taskList=TaskLists.GetOne(TaskCur.TaskListNum);
			LoadTask();
			butRefresh.Visible=false;
			labelTaskChanged.Visible=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//NOTE: Any changes here need to be made to UserControlTasks.Delete_Clicked() as well.
			if(_formTaskNoteEdit!= null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			if(!IsNew) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
				if(Tasks.GetOne(TaskCur.TaskNum)==null) {
					MsgBox.Show(this,"Task already deleted.");//if task has remained open and has become stale on a workstation.
					butDelete.Enabled=false;
					butOK.Enabled=false;
					butSend.Enabled=false;
					butAddNote.Enabled=false;
					Text+=" - {"+Lan.g(this,"Deleted")+"}";
					return;
				}
				//TaskListNum=-1 is only possible if it's new.  This will never get hit if it's new.
				if(TaskCur.TaskListNum==0) {
					Tasks.TaskEditCreateLog(Lan.g(this,"Deleted task"),TaskCur);
				}
				else {
					string logText=Lan.g(this,"Deleted task from tasklist");
					TaskList taskList=TaskLists.GetOne(TaskCur.TaskListNum);
					if(taskList!=null) {
						logText+=" "+taskList.Descript;
					}
					else {
						logText+=". Task list no longer exists";
					}
					logText+=".";
					Tasks.TaskEditCreateLog(logText,TaskCur);
				}
			}
			int countDocuments=TaskAttachments.GetCountDocumentForTaskNum(TaskCur.TaskNum);
			Tasks.Delete(TaskCur.TaskNum);//always do it this way to clean up all five tables (six if hq)
			if(countDocuments>0) { 
				MsgBox.Show(this,"This task has linked document(s). The task attachments have been deleted and the documents can be deleted via the imaging module, if desired.");
			}
			SendSignalsRefillLocal(TaskCur,TaskCur.TaskListNum,false);
			TaskHist taskHist=new TaskHist(_taskOld);
			taskHist.IsNoteChange=DidNotesChange;
			taskHist.UserNum=Security.CurUser.UserNum;
			TaskHists.Insert(taskHist);
			SecurityLogs.MakeLogEntry(EnumPermType.TaskDelete,0,"Task "+POut.Long(TaskCur.TaskNum)+" deleted",0);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butReply_Click(object sender,EventArgs e) {
			//This can't happen if IsNew
			//This also can't happen if the task is mine with no replies.
			//Button not visible unless a ReplyToUserNum has been calculated successfully.
			if(_formTaskNoteEdit!= null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			long taskListNum=TaskCur.TaskListNum;
			long inbox=Userods.GetInbox(_userNumReplyTo);
			if(inbox==0) {
				MsgBox.Show(this,"No inbox has been set up for this user yet.");
				return;
			}
			if(!DidNotesChange && textDescript.Text==TaskCur.Descript) {//nothing changed
				_formTaskNoteEdit=new FormTaskNoteEdit();
				_formTaskNoteEdit.TaskNoteCur=new TaskNote();
				_formTaskNoteEdit.TaskNoteCur.TaskNum=TaskCur.TaskNum;
				_formTaskNoteEdit.TaskNoteCur.DateTimeNote=DateTime.Now;//Will be slightly adjusted at server.
				_formTaskNoteEdit.TaskNoteCur.UserNum=Security.CurUser.UserNum;
				_formTaskNoteEdit.TaskNoteCur.IsNew=true;
				_formTaskNoteEdit.TaskNoteCur.Note="";
				_formTaskNoteEdit.FormClosed+=FormTaskNoteEdit_FormClosed_Reply;
				_formTaskNoteEdit.Show(this);//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.;
				return;
			}
			TaskCur.TaskListNum=inbox;
			if(!SaveCur()) {
				return;
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			//Both tasklistnums are different, since SaveCur() returned true.  Thereore, send signals for both task lists.
			SendSignalsRefillLocal(TaskCur,taskListNum);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void FormTaskNoteEdit_FormClosed_Reply(object sender,EventArgs e) {
			if(_formTaskNoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			long taskListNum=TaskCur.TaskListNum;
			TaskCur.TaskListNum=Userods.GetInbox(_userNumReplyTo);
			if(!SaveCur()) {
				return;
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(TaskCur,taskListNum);
			_formTaskNoteEdit.FormClosed-=FormTaskNoteEdit_FormClosed_Reply;
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Send to another user.</summary>
		private void butSend_Click(object sender,EventArgs e) {
			//This button is always present.
			if(_formTaskNoteEdit!= null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			long taskListNum=TaskCur.TaskListNum;
			using FormTaskListSelect formTaskListSelect=new FormTaskListSelect(listObjectType.GetSelected<TaskObjectType>(),IsNew);
			formTaskListSelect.ShowDialog();
			if(formTaskListSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			TaskCur.TaskListNum=formTaskListSelect.ListSelectedLists[0];
			_taskList=TaskLists.GetOne(TaskCur.TaskListNum);
			TaskCur.ParentDesc=_taskList.Descript;
			textTaskList.Text=_taskList.Descript;
			if(!SaveCur()) {
				return;
			}
			SaveCopy(formTaskListSelect.ListSelectedLists.Skip(1).ToList());//Copies/Inserts task and sends to inboxes if multiple lists were selected.
			//Check for changes.  If something changed, send a signal.
			if(DidNotesChange || !TaskCur.Equals(_taskOld) || _hasStatusChanged) {
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
				TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			}
			SendSignalsRefillLocal(TaskCur,taskListNum);
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Saves a copy of the task</summary>
		private bool SaveCopy(List<long> listTaskListNums) {
			for(int i=0;i< listTaskListNums.Count;i++) {
				Task taskCopy=TaskCur.Copy();
				taskCopy.TaskListNum=listTaskListNums[i];
				taskCopy.IsUnread=true;
				taskCopy.ReminderGroupId="";
				if(taskCopy.ReminderType!=TaskReminderType.NoReminder && !PrefC.GetBool(PrefName.TasksUseRepeating)) {//Make a new ID if it's blank no matter what.  Could be an old task being changed.
					Tasks.SetReminderGroupId(taskCopy);
				}
				taskCopy.IsNew=true;
				try {
					Tasks.Insert(taskCopy);
					for(int j=0;j<_listTaskNotes.Count;j++) {
						TaskNote taskNote=_listTaskNotes[j].Copy();
						taskNote.TaskNum=taskCopy.TaskNum;
						TaskNotes.Insert(taskNote);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return false;
				}
				if(taskCopy.TaskListNum==Userods.GetInbox(Security.CurUser.UserNum)) {//My inbox.
					FormTaskEdit formTaskEdit=new OpenDental.FormTaskEdit(taskCopy);//Maintain previous behavior. If I send to myself, should popup.
					formTaskEdit.Show();//Non-modal. 
					UserControlTasks.RefillLocalTaskGrids(taskCopy,_listTaskNotes,null);//Refills local grids with _taskCur, which has a new taskNum now.
					continue;
				}
				//Sent to someone else. Task should popup for that user.
				TaskUnreads.AddUnreads(taskCopy,Security.CurUser.UserNum);//Tell the database about all the users with unread tasks prior to sending signals.
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,taskCopy.TaskNum);//popup
				Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskCopy.TaskListNum);//signal so all instances of UserControlTasks refreshes.
			}
			return true;
		}
		
		///<summary>Sets the form into "Deleted" mode to disallow changes to be made.</summary>
		private void SetFormToDeletedMode() {
				_isTaskDeleted=true;
				labelTaskChanged.Visible=true;
				labelTaskChanged.Text=Lan.g(this,"The task has been deleted");
				DisableMostControls();//Set form into a "read-only" mode to allow user to copy text only.
		}
		
		/// <summary>Sets fields and buttons that modify the task enabled or disabled based on the input  </summary>
		private void SetFormReadOnly(bool doSetReadOnly) {
			textDateTask.ReadOnly=doSetReadOnly;
			textDateTimeFinished.ReadOnly=doSetReadOnly;
			textDateTimeEntry.ReadOnly=doSetReadOnly;
			butNow.Enabled=!doSetReadOnly;
			butNowFinished.Enabled=!doSetReadOnly;
			textDescript.ReadOnly=doSetReadOnly;
			butAutoNote.Enabled=!doSetReadOnly;
			textDescriptOverride.ReadOnly=doSetReadOnly;
			butAddNote.Enabled=!doSetReadOnly;
			butDelete.Enabled=!doSetReadOnly;
			comboTaskPriorities.Enabled=!doSetReadOnly;
			butReply.Enabled=!doSetReadOnly;
			butChangeUser.Enabled=!doSetReadOnly;
			butRed.Enabled=!doSetReadOnly;
			butBlue.Enabled=!doSetReadOnly;
			listObjectType.Enabled=!doSetReadOnly;
			butChange.Enabled=!doSetReadOnly;
			butEditAutoNote.Enabled=!doSetReadOnly;
			butCreateJob.Enabled=!doSetReadOnly;
			groupReminder.Enabled=!doSetReadOnly;
			listCategory.Enabled=!doSetReadOnly;
			long taskListNum= Security.CurUser.TaskListInBox;
			//If this task is in the current users task list, or they created it, or they have permission, allow them to mark it done. 
			if((TaskCur.TaskListNum==taskListNum && taskListNum!=0) || Tasks.IsAuthorizedOrOwner(TaskCur)) {
				checkDone.Enabled=true;
				return;
			}
			if(checkIsReadOnly.Checked) {
				//Only disable the done checkbox if the task is read only, isn't in the current user's task list, and wasn't created by the current user.
				checkDone.Enabled=false;
			}
		}

		private void checkIsReadOnly_CheckedChanged(object sender,EventArgs e) {
			SetFormReadOnly(checkIsReadOnly.Checked);
			FillComboJobs();
		}
		
		///<summary>Sets all controls to read-only or disabled except cancel and copy button.</summary>
		private void DisableMostControls() {
			//Disable most controls.
			DisableAllExcept(butCancel,butCopy,textDateTimeEntry,textDateTimeFinished,labelTaskChanged,splitContainer);
			//Enable and set read-only for fields we want to allow copying from.
			#region splitContainerDescriptNote
			splitContainer.Enabled=true;  //This was probably already true but just in case
			butBlue.Enabled=false;
			butRed.Enabled=false;
			butAutoNote.Enabled=false;
			textDateTimeEntry.ReadOnly=true;
			textDateTimeFinished.ReadOnly=true;
			textDescript.ReadOnly=true;
			#endregion splitContainerDescriptNote
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!TrySaveAndShouldClose()) {
				return;
			}
			Close();
		}

		///<summary>Determines which signals need to be sent, and sends them.  Pass in taskListNumOld if the taskListNum has possibly changed.</summary>
		private void SendSignalsRefillLocal(Task task,long taskListNumOld=-1,bool canKeepTask=true) {
			List<long> listSignalNums=new List<long>();
			listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,task.TaskListNum));//Signal for current tasklist.
			listSignalNums.Add(Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum));//Signal for current task.
			if(taskListNumOld!=-1 && task.TaskListNum!=taskListNumOld) {
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskListNumOld));//Signal for old tasklist.
			}
			if(_userNumFrom!=task.UserNum) {//From User has changed.
				if(_userNumFrom!=0) {//Should never be 0, but might be 0 in much older databases?
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskAuthor,KeyType.Undefined,_userNumFrom));//Signal for previous From User.
				}
				if(task.UserNum!=0) {//Should never be 0, but might be 0 in much older databases?
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskAuthor,KeyType.Undefined,task.UserNum));//Signal for new From User.
				}
			}
			if(_patNum==task.KeyNum) {
				UserControlTasks.RefillLocalTaskGrids(TaskCur,_listTaskNotes,listSignalNums,canKeepTask);
				return;
			}
			//Attached patient changed.
			if(_patNum!=0) {//Previous Object was a Patient
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskPatient,KeyType.Undefined,_patNum));//Signal for previous Patient.
			}
			if(TaskCur.ObjectType==TaskObjectType.Patient && task.KeyNum!=0) {//New Object is a Patient
				listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskPatient,KeyType.Undefined,task.KeyNum));//Signal for new Patient.
			}
			UserControlTasks.RefillLocalTaskGrids(TaskCur,_listTaskNotes,listSignalNums,canKeepTask);
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			if(_formTaskNoteEdit != null && !_formTaskNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			if(IsNew) { 
				List<TaskAttachment> listTaskAttachments=TaskAttachments.GetManyByTaskNum(TaskCur.TaskNum);
				if(listTaskAttachments!=null && listTaskAttachments.Count>0 && 
					!MsgBox.Show(this,MsgBoxButtons.YesNo,"This task has attachments. Continue? If yes, any linked documents can be deleted from the imaging module if desired.")) {
					return;
				}
			}
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormTaskEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Abort) {
				return;
			}
			if(DialogResult==DialogResult.None && (_formTaskNoteEdit!=null && !_formTaskNoteEdit.IsDisposed)) {//This can only happen if the user closes the window using the X in the upper right.
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				e.Cancel=true;
				return;
			}
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				//No more automation here
			}
			else {
				if(Security.CurUser!=null) {//Because tasks are modal, user may log off and this form may close with no user.
					TaskUnreads.SetRead(Security.CurUser.UserNum,TaskCur);//no matter why it was closed
				}
			}
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew) {
				Tasks.Delete(TaskCur.TaskNum);//Shouldn't be displayed in UserControlTasks yet, so no refill needed.
				SecurityLogs.MakeLogEntry(EnumPermType.TaskDelete,0,"Task "+POut.Long(TaskCur.TaskNum)+" deleted",0);
			}
			else if(DidNotesChange) {//Note changed and dialogue result was not OK
				//This should only ever be hit if the user clicked cancel or X.  Everything else will have dialogue result OK and exit above.
				//Make a TaskHist entry to note that the task notes were changed.
				TaskHist taskHist = new TaskHist(_taskOld);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				taskHist.IsNoteChange=true;
				TaskHists.Insert(taskHist);
				//Task has already been invalidated in FromTaskNoteEdit when the Note was added/edited.  Other Users have already been notified the task changed.
				//Do not send new TaskInvalid signal.
			}
			//Notes didn't changed on a task marked Done when the task was opened.
			if(!DidNotesChange) {
				return;
			}
			if(_taskOld.TaskStatus!=TaskStatusEnum.Done) {
				return;
			}
			if(butRefresh.Visible) {
				return;
			}
			//If a note was added to a Done task and the user hits cancel, the task status is set to Viewed because the note is still there and the task didn't move lists.
			//Notes changed on a task marked Done when the task was opened.
			if(checkDone.Checked) {//Will happen when the Done checkbox has been manually re-checked by the user or refreshing the task checked the box.
				return;
			}
			TaskCur.TaskStatus=TaskStatusEnum.Viewed;
			try {
				Tasks.Update(TaskCur,_taskOld);//if task has already been altered, then this is where it will fail.
			}
			catch {
				return;//Silently leave because the user could be trying to cancel out of a task that had been edited by another user.
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,TaskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(TaskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(TaskCur);
			
		}

		private void ButEditAutoNote_Click(object sender,EventArgs e) {
			if(!GetHasAutoNotePrompt()) {
				MessageBox.Show(Lan.g(this,"No Auto Note available to edit."));
				return;
			}
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.StrMainTextNote=textDescript.Text;
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textDescript.Text=frmAutoNoteCompose.StrCompletedNote;
				butEditAutoNote.Visible=GetHasAutoNotePrompt();
			}
		}

		private bool GetHasAutoNotePrompt() {
			return Regex.IsMatch(textDescript.Text,_autoNotePromptRegex);
		}

	}
}