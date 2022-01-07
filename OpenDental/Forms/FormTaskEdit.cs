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

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTaskEdit:FormODBase {
		private Task _taskCur;
		private Task _taskOld;
		///<summary></summary>
		public bool IsNew;
		///<summary>After closing, if this is not zero, then it will jump to the object specified in GotoKeyNum.</summary>
		public TaskObjectType GotoType;
		///<summary>After closing, if this is not zero, then it will jump to the specified patient.</summary>
		public long GotoKeyNum;
		private TaskList _taskListCur;
		///<summary>Will be set to true if any note was added or an existing note changed. Does not track changes in the description.</summary>
		public bool NotesChanged;
		private List<TaskNote> _listTaskNotes;
		///<summary>If the reply button is visible, this stores who to reply to.  It's determined when loading the form.</summary>
		private long _replyToUserNum;
		///<summary>Gets set to true externally if this window popped up without user interaction.  It will behave slightly differently.  
		///Specifically, the New checkbox will be unchecked so that if user clicks OK, the task will be marked as read.
		///Also if IsPop is set to true, this window will not steal focus from other windows when poping up.</summary>
		public bool IsPopup;
		///<summary>When tracking status by user, this tracks whether it has changed.  This is so that if it has changed, a signal can be sent for a refresh of lists.</summary>
		private bool _statusChanged;
		///<summary>If this task starts out 'unread', then this starts out true.  If the user changes the description, changes a note, or simply clicks 
		///"OK", then the task gets set to read.  But the user can manually change it back and this variable gets set to false.  From then on, any 
		///changes to description or note do not trigger the task to get set to read.  In other words, the automation only happens once.</summary>
		private bool _mightNeedSetRead;
		///<summary>When this window is first opened, if this task is in someone else's inbox, then the "new" status is meaningless and will not show.  In that case, this variable is set to true.  Only used when tracking new status by user.</summary>
		private bool _startedInOthersInbox;
		///<summary>Filled on load with all non-hidden task priority definitions.</summary>
		private List<Def> _listTaskPriorities;
		private long _priorityDefNumSelected;
		///<summary>Keeps track of the number of notes that were associated to this task on load and after refilling the task note grid.  Only used for HQ in order to keep track of task note manipulation.</summary>
		private int _numNotes=-1;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for red.</summary>
		private const long _triageRedNum=501;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for blue.</summary>
		private const long _triageBlueNum=502;
		///<summary>FK to the definition.DefNum at HQ for the triage priority color for white.</summary>
		private const long _triageWhiteNum=503;
		private List<JobLink> _jobLinks;
		private List<Job> _listJobs;
		private bool _isLoading;
		private List<TaskReminderType> _listTaskReminderTypeNames;
		///<summary>Do not allow any task or task related changes.  Only allow copy and cancel buttons, and copying of text.
		///Used when task has been deleted from elsewhere while still open.</summary>
		private bool _isTaskDeleted=false;
		///<summary>UserNum attached to task when form was loaded.  Used in OK click to detect changes.  Can be 0.</summary>
		private long _userNumFrom;
		private const string _autoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";
		///<summary>PatNum attached to task when form was loaded.  Used in OK click to detect changes.  Can be 0.</summary>
		private long _patientPatNum;

		///<summary>This is used to make the task window not steal focus when opening as a popup.</summary>
		protected override bool ShowWithoutActivation
		{
			get { return IsPopup; }
		}

		///<summary>Checks if FormTaskNoteEdit is currently open. This form being open
		///must block a number of actions in this form. </summary>
		private bool TaskNoteEditExists {
			get {
				if(OwnedForms==null || OwnedForms.Length<=0) {
					return false;
				}
				foreach(Form form in OwnedForms) {
					if(form.Name=="FormTaskNoteEdit") {
						return true;
					}
				}
				return false;
			}
		}

		public long TaskNumCur {
			get {
				return _taskCur.TaskNum;
			}
		}

		///<summary>Task gets inserted ahead of time, then frequently altered before passing in here.  The taskOld that is passed in should be the task as it is in the database.  When saving, taskOld will be compared with db to make sure no changes.</summary>
		public FormTaskEdit(Task taskCur,Task taskOld=null) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_taskCur=taskCur;
			if(taskOld==null) {
				_taskOld=taskCur.Copy();
			}
			else {
				_taskOld=taskOld;
			}
			_taskListCur=TaskLists.GetOne(taskCur.TaskListNum);
			Lan.F(this);
		}

		private void FormTaskListEdit_Load(object sender,System.EventArgs e) {
			LoadTask();
			gridMain.ScrollToEnd();
		}

		private void LoadTask() {
			textTaskNum.Text=_taskCur.TaskNum.ToString();//Assign TaskNum
			if(PrefC.IsODHQ && !IsNew) {//to simplify the code, only allow jobs to be attached to existing tasks at HQ, not new tasks.
				comboJobs.Visible=true;
				labelJobs.Visible=true;
				FillComboJobs();
			}
			if(IsNew) {
				//butDelete.Enabled always stays true
				//textDescript always editable
			}
			else {//trying to edit an existing task, so need to block some things
				bool isTaskForCurUser=true;
				if(_taskCur.UserNum!=Security.CurUser.UserNum) {//current user didn't write this task, so block them.
					isTaskForCurUser=false;//Delete will only be enabled if the user has the TaskEdit and TaskNoteEdit permissions.
				}
				if(_taskCur.TaskListNum!=Security.CurUser.TaskListInBox) {//the task is not in the logged-in user's inbox
					isTaskForCurUser=false;
				}
				if(isTaskForCurUser) {//this just allows getting the NoteList less often
					_listTaskNotes=TaskNotes.GetForTask(_taskCur.TaskNum);//so we can check so see if other users have added notes
					for(int i=0;i<_listTaskNotes.Count;i++) {
						if(Security.CurUser.UserNum!=_listTaskNotes[i].UserNum) {
							isTaskForCurUser=false;
							break;
						}
					}
				}
				if(!isTaskForCurUser && !Security.IsAuthorized(Permissions.TaskNoteEdit,true)) {//but only need to block them if they don't have TaskNoteEdit permission
					butDelete.Enabled=false;
				}
				if(!isTaskForCurUser && !Security.IsAuthorized(Permissions.TaskEdit,true)) {
					butDelete.Enabled=false;
					butAutoNote.Enabled=false;
					butEditAutoNote.Enabled=false;
					textDescript.ReadOnly=true;
					textDescript.BackColor=System.Drawing.SystemColors.Window;
					textDescriptOverride.ReadOnly=true;
					textDescriptOverride.BackColor=System.Drawing.SystemColors.Window;
				}
			}
			_listTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);//Fill list with non-hidden priorities.
			//There must be at least one priority in Setup | Definitions.  Do not let them load the task edit window without at least one priority.
			if(_listTaskPriorities.Count < 1) {
				MsgBox.Show(this,"There are no task priorities in Setup | Definitions.  There must be at least one in order to use the task system.");
				DialogResult=DialogResult.Cancel;
				Close();
			}
			bool hasDefault=false;
			_priorityDefNumSelected=_taskCur.PriorityDefNum;
			if(_priorityDefNumSelected==0 && IsNew && _taskCur.ReminderType!=TaskReminderType.NoReminder) {
				foreach(Def defTaskPriority in _listTaskPriorities) {
					if(defTaskPriority.ItemValue=="R") {
						_priorityDefNumSelected=defTaskPriority.DefNum;
						hasDefault=true;
						break;
					}
				}
			}
			if(_priorityDefNumSelected==0) {//The task does not yet have a priority assigned.  Find the default and assign it, if available.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].ItemValue=="D") {
						_priorityDefNumSelected=_listTaskPriorities[i].DefNum;
						hasDefault=true;
						break;
					}
				}
			}
			comboTaskPriorities.Items.Clear();
			for(int i=0;i<_listTaskPriorities.Count;i++) {//Add non-hidden defs first
				comboTaskPriorities.Items.Add(_listTaskPriorities[i].ItemName);
				if(_priorityDefNumSelected==_listTaskPriorities[i].DefNum) {//Use priority listed within the database.
					comboTaskPriorities.SelectedIndex=i;//Sets combo text too
				}
			}
			if(_priorityDefNumSelected==0 && !hasDefault) {//If no default has been set in the definitions, select the last item in the list.
				comboTaskPriorities.SelectedIndex=comboTaskPriorities.Items.Count-1;
				_priorityDefNumSelected=_listTaskPriorities[_listTaskPriorities.Count-1].DefNum;
			}
			if(_taskListCur!=null && IsNew && _taskListCur.TaskListNum==1697 && PrefC.GetBool(PrefName.DockPhonePanelShow)) {//Set to triage blue if HQ, triage list, and is new.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].DefNum==_triageBlueNum) {//Finding the option that is triageBlue to select it in the combobox (Combobox mirrors _listTaskPriorityDefs)
						comboTaskPriorities.SelectedIndex=i;
						break;
					}
				}
				_priorityDefNumSelected=_triageBlueNum;
			}
			if(comboTaskPriorities.SelectedIndex==-1) {//Priority for task wasn't found in the non-hidden priorities list (and isn't triageBlue), so it must be a hidden priority.
				List<Def> listTaskDefsLong=Defs.GetDefsForCategory(DefCat.TaskPriorities);//Get all priorities
				for(int i=0;i<listTaskDefsLong.Count;i++) {
					if(listTaskDefsLong[i].DefNum==_priorityDefNumSelected) {//We find the hidden priority and set the text of the combo box.
						comboTaskPriorities.DropDownStyle=ComboBoxStyle.DropDown;
						comboTaskPriorities.Text=(listTaskDefsLong[i].ItemName+" (Hidden)");
						butColor.BackColor=listTaskDefsLong[i].ItemColor;
					}
				}
			}
			textUser.Text=Userods.GetName(_taskCur.UserNum);//might be blank.
			if(_taskListCur!=null) {
				_taskCur.ParentDesc=_taskListCur.Descript;
				textTaskList.Text=_taskListCur.Descript;
			}
			if(_taskCur.DateTimeOriginal==DateTime.MinValue) {
				label3.Visible=false;
				textBoxDateTimeCreated.Visible=false;
			}
			else {
				textBoxDateTimeCreated.Text=_taskCur.DateTimeOriginal.ToString();
			}
			if(_taskCur.DateTimeEntry.Year<1880) {
				textDateTimeEntry.Text=DateTime.Now.ToString();
			}
			else {
				textDateTimeEntry.Text=_taskCur.DateTimeEntry.ToString();
			}
			if(_taskCur.DateTimeFinished.Year<1880) {
				textDateTimeFinished.Text="";//DateTime.Now.ToString();
			}
			else {
				textDateTimeFinished.Text=_taskCur.DateTimeFinished.ToString();
			}
			textDescript.Text=_taskCur.Descript;
			textDescriptOverride.Text=_taskCur.DescriptOverride;
			if(!IsPopup) {//otherwise, TextUser is selected, and it cannot accept input.  This prevents a popup from accepting using input accidentally.
				textDescript.Select();//Focus does not work for some reason.
				textDescript.Select(_taskCur.Descript.Length,0);//Place the cursor at the end of the description box.
			}
			long mailboxUserNum=0;
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser) && _taskCur.TaskListNum !=0) {
				mailboxUserNum=TaskLists.GetMailboxUserNum(_taskCur.TaskListNum);
				if(mailboxUserNum != 0 && mailboxUserNum != Security.CurUser.UserNum) {
					_startedInOthersInbox=true;
					checkNew.Checked=false;
					checkNew.Enabled=false;
				}
			}
			//this section must come after textDescript is set:
			if(_taskCur.TaskStatus==TaskStatusEnum.Done) {//global even if new status is tracked by user
				checkDone.Checked=true;
			}
			else {//because it can't be both new and done.
				checkDone.Checked=false;
				if(IsPopup) {//It clearly is Unread, but we don't want to leave it that way upon close OK.
					checkNew.Checked=false;
					_statusChanged=true;
				}
				else if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					if(_startedInOthersInbox) {
						_taskCur.IsUnread=TaskUnreads.IsUnread(mailboxUserNum,_taskCur);
					}
					else {
						_taskCur.IsUnread=TaskUnreads.IsUnread(Security.CurUser.UserNum,_taskCur);
						if(_taskCur.IsUnread) {
							checkNew.Checked=true;
							_mightNeedSetRead=true;
						}
					}
				}
				else {//tracked globally, the old way
					if(_taskCur.TaskStatus==TaskStatusEnum.New) {
						_taskCur.IsUnread=true;//Not using taskunread table.
						checkNew.Checked=true;
					}
				}
			}
			groupReminder.Visible=(!PrefC.GetBool(PrefName.TasksUseRepeating));
			panelRepeating.Visible=PrefC.GetBool(PrefName.TasksUseRepeating);
			textReminderRepeatFrequency.Text=(IsNew?"1":_taskCur.ReminderFrequency.ToString());
			//Fill comboReminderRepeat with repeating options.
			_listTaskReminderTypeNames=new List<TaskReminderType>() {
				TaskReminderType.NoReminder,
				TaskReminderType.Once,
				TaskReminderType.Daily,
				TaskReminderType.Weekly,
				TaskReminderType.Monthly,
				TaskReminderType.Yearly
			};
			comboReminderRepeat.Items.Clear();
			for(int i=0;i<_listTaskReminderTypeNames.Count;i++) {
				comboReminderRepeat.Items.Add(_listTaskReminderTypeNames[i].ToString());
				if(_taskCur.ReminderType.HasFlag(_listTaskReminderTypeNames[i])) {
					comboReminderRepeat.SelectedIndex=i;
				}
			}
			if(_taskCur.ReminderType==TaskReminderType.Once) {
				if(IsNew) {
					datePickerReminder.Value=DateTime.Now;
					timePickerReminder.Value=DateTime.Now;
				}
				else {
					datePickerReminder.Value=_taskCur.DateTimeEntry;
					timePickerReminder.Value=_taskCur.DateTimeEntry;
				}
			}
			checkReminderRepeatMonday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Monday);
			checkReminderRepeatTuesday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Tuesday);
			checkReminderRepeatWednesday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Wednesday);
			checkReminderRepeatThursday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Thursday);
			checkReminderRepeatFriday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Friday);
			checkReminderRepeatSaturday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Saturday);
			checkReminderRepeatSunday.Checked=_taskCur.ReminderType.HasFlag(TaskReminderType.Sunday);
			if(_taskCur.DateTask.Year>1880) {
				textDateTask.Text=_taskCur.DateTask.ToShortDateString();
			}
			butEditAutoNote.Visible=GetHasAutoNotePrompt();
			if(_taskCur.IsRepeating) {
				checkNew.Enabled=false;
				checkDone.Enabled=false;
				textDateTask.Enabled=false;
				listObjectType.Enabled=false;
				if(_taskCur.TaskListNum!=0) {//not a main parent
					comboDateType.Enabled=false;
				}
			}
			for(int i=0;i<Enum.GetNames(typeof(TaskDateType)).Length;i++) {
				comboDateType.Items.Add(Lan.g("enumTaskDateType",Enum.GetNames(typeof(TaskDateType))[i]));
				if((int)_taskCur.DateType==i) {
					comboDateType.SelectedIndex=i;
				}
			}
			if(_taskCur.FromNum==0) {
				checkFromNum.Checked=false;
				checkFromNum.Enabled=false;
			}
			else {
				checkFromNum.Checked=true;
			}
			listObjectType.Items.Clear();
			listObjectType.Items.AddEnums<TaskObjectType>();
			_listTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);//Fill list with non-hidden priorities.
			//There must be at least one priority in Setup | Definitions.  Do not let them load the task edit window without at least one priority.
			if(_listTaskPriorities.Count < 1) {
				MsgBox.Show(this,"There are no task priorities in Setup | Definitions.  There must be at least one in order to use the task system.");
				DialogResult=DialogResult.Cancel;
				Close();
			}
			FillObject();
			FillGrid();//Need this in order to pick ReplyToUserNum next.
			if(IsNew) {
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else if(_taskListCur==null) {
				//|| TaskListCur.TaskListNum!=Security.CurUser.TaskListInBox) {//if this task is not in my inbox
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else if(_listTaskNotes.Count==0 && _taskCur.UserNum==Security.CurUser.UserNum) {//if this is my task
				labelReply.Visible=false;
				butReply.Visible=false;
			}
			else {//reply button will be visible
				if(_listTaskNotes.Count==0) {//no notes, so reply to owner
					_replyToUserNum=_taskCur.UserNum;
				}
				else {//reply to most recent author who is not me
					//loop backward through the notes to find who to reply to
					for(int i=_listTaskNotes.Count-1;i>=0;i--) {
						if(_listTaskNotes[i].UserNum!=Security.CurUser.UserNum) {
							_replyToUserNum=_listTaskNotes[i].UserNum;
							break;
						}
					}
					if(_replyToUserNum==0) {//can't figure out who to reply to.
						labelReply.Visible=false;
						butReply.Visible=false;
					}
				}
				labelReply.Text=Lan.g(this,"(Send to ")+Userods.GetName(_replyToUserNum)+")";
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//Show red and blue buttons for HQ always
				butRed.Visible=true;
				butBlue.Visible=true;
			}
			if(!Security.IsAuthorized(Permissions.TaskEdit,true)){
				butAudit.Visible=false;
			}
			SetTaskStartingLocation();
			_userNumFrom=_taskOld.UserNum;
			_patientPatNum=0;
			if(_taskOld.ObjectType==TaskObjectType.Patient && _taskOld.KeyNum>0) {//Patient is attached.
				_patientPatNum=_taskOld.KeyNum;
			}
		}

		///<summary>Sets the starting location of this form. Should only be called on load.
		///The first Task window will default to CenterScreen. After that we will cascade down.
		///If any part of this form will be off screen we will default the next task to the top left of the primary monitor.</summary>
		private void SetTaskStartingLocation() { 
			List<FormTaskEdit> listTaskEdits=Application.OpenForms.OfType<FormTaskEdit>().ToList();
			//Since this form has already gone through the initilize, there will be at least 1. Except when running unit tests.
			if(listTaskEdits.Count<=1) {
				this.StartPosition=FormStartPosition.CenterScreen;
				return;
			}
			Point pointStart;
			//There are multiple task edit windows open, so offset the new window by a little so that it does not show up directly over the old one.
			const int OFFSET=20;//Sets how far to offset the cascaded form location.
			this.StartPosition=FormStartPosition.Manual;
			//Count is 1 based, list index is 0 based, -2 to get the "last" window
			FormTaskEdit formPrevious=listTaskEdits[listTaskEdits.Count-2];
			System.Windows.Forms.Screen screenCur=System.Windows.Forms.Screen.PrimaryScreen;
			//Figure out what monitor the previous task edit window is on.
			if(formPrevious!=null && !formPrevious.IsDisposed && formPrevious.WindowState!=FormWindowState.Minimized) {
				screenCur=System.Windows.Forms.Screen.FromControl(formPrevious);
				//Get the start point relative to the screen the form will open on.
				//pointStart=new Point(formPrevious.Location.X-screenCur.Bounds.Left,formPrevious.Location.Y-screenCur.Bounds.Top);
				pointStart=formPrevious.Location;
			}
			else {
				pointStart=new Point(screenCur.WorkingArea.X,screenCur.WorkingArea.Y);
			}
			//Temporarily apply the offset and see if that rectangle can fit on screenCur, if not, default to a high location on the primary screen.
			pointStart.X+=OFFSET;
			pointStart.Y+=OFFSET;
			Rectangle rect=new Rectangle(pointStart,this.Size);
			if(!screenCur.WorkingArea.Contains(rect)) {
				//A portion of the new window is outside of the usable area on the current monitor.
				//Force the new window to be at "0,50" (relatively) in regards to the primary monitor.
				pointStart=new Point(screenCur.WorkingArea.X,screenCur.WorkingArea.Y+50);
			}
			this.Location=pointStart;
		}

		private void FillComboJobs() {
			if(_isTaskDeleted || !PrefC.IsODHQ) {
				return;
			}
			_isLoading=true;
			comboJobs.Items.Clear();
			_jobLinks = JobLinks.GetForTask(this._taskCur.TaskNum);
			_listJobs = Jobs.GetMany(_jobLinks.Select(x => x.JobNum).ToList());
			foreach(Job job in _listJobs) {
				comboJobs.Items.Add(job.ToString());
			}
			if(_listJobs.Count==0) {
				comboJobs.Items.Add("None");
				if(JobPermissions.IsAuthorized(JobPerm.Concept,true)) {
					butCreateJob.Visible=true;
				}
			}
			comboJobs.Items.Add("Attach");
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
			if(comboJobs.SelectedIndex==comboJobs.Items.Count-1) {
				if(Tasks.IsTaskDeleted(_taskCur.TaskNum)){
					SetFormToDeletedMode();
					MsgBox.Show(this,"This task was deleted from elsewhere.  Cannot attach to job.");
					return;
				}
				//Atach new job
				using FormJobSearch FormJS = new FormJobSearch();
				FormJS.ShowDialog();
				if(FormJS.DialogResult!=DialogResult.OK || FormJS.SelectedJobNum==0) {
					return;
				}
				JobLink jobLink = new JobLink();
				jobLink.JobNum=FormJS.SelectedJobNum;
				jobLink.FKey=_taskCur.TaskNum;
				jobLink.LinkType=JobLinkType.Task;
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobLink.JobNum);
				FillComboJobs();
				long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,_taskCur.TaskNum);
				UserControlTasks.RefillLocalTaskGrids(_taskCur,_listTaskNotes,new List<long>() { signalNum });
				return;
			}
			FormOpenDental.S_GoToJob(_listJobs[comboJobs.SelectedIndex].JobNum);
		}

		private void FillGrid() {
			if(_isTaskDeleted) {
				return;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date Time"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"User"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),400);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listTaskNotes=TaskNotes.GetForTask(_taskCur.TaskNum);
			//Only do weird logic when editing a task associated with the triage task list.
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				if(_numNotes==-1) {//Only fill _numNotes here the first time FillGrid is called.  This is used for coloring triage tasks.
					_numNotes=_listTaskNotes.Count;
				}
				if(_priorityDefNumSelected==_triageBlueNum && _numNotes==0 && _listTaskNotes.Count!=0) {//Blue triage task with an added note
					_priorityDefNumSelected=_triageWhiteNum;//Change priority to white
					for(int i=0;i<_listTaskPriorities.Count;i++) {
						if(_listTaskPriorities[i].DefNum==_triageWhiteNum) {
							comboTaskPriorities.SelectedIndex=i;//Change selection to the triage white
						}
					}
				}
				else if(_priorityDefNumSelected==_triageWhiteNum && _numNotes!=0 && _listTaskNotes.Count==0) {//White triage task with note that has been deleted, change it back to blue.
					_priorityDefNumSelected=_triageBlueNum;
					for(int i=0;i<_listTaskPriorities.Count;i++) {
						if(_listTaskPriorities[i].DefNum==_triageBlueNum) {
							comboTaskPriorities.SelectedIndex=i;//Change selection to the triage blue
						}
					}
				}
				_numNotes=_listTaskNotes.Count;
			}
			for(int i=0;i<_listTaskNotes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listTaskNotes[i].DateTimeNote.ToShortDateString()+" "+_listTaskNotes[i].DateTimeNote.ToShortTimeString());
				row.Cells.Add(Userods.GetName(_listTaskNotes[i].UserNum));
				row.Cells.Add(_listTaskNotes[i].Note);
				row.Tag=_listTaskNotes[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isTaskDeleted) {
				return;//The user can copy text with right click, or copy button.
			}
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			FormTaskNoteEdit form=new FormTaskNoteEdit();
			form.TaskNoteCur=(TaskNote)gridMain.ListGridRows[e.Row].Tag;
			form.EditComplete=OnNoteEditComplete_CellDoubleClick;
			form.Show(this);//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.
		}

		/// <summary>Adds a new note to the task, and opens it for editing by the user.  Will not open if the task is not visible, is deleted, 
		/// or if a child form is already open.</summary>
		public bool AddNoteToTaskAndEdit(string initialText="") {
			//We only want to show the note edit window if the task is visible, it's not deleted, and another window from this task isn't open.
			if(!this.Visible || _isTaskDeleted || TaskNoteEditExists) {
				return false;
			}
			FormTaskNoteEdit form=new FormTaskNoteEdit();
			form.TaskNoteCur=new TaskNote();
			form.TaskNoteCur.TaskNum=_taskCur.TaskNum;
			form.TaskNoteCur.DateTimeNote=DateTime.Now;//Will be slightly adjusted at server.
			form.TaskNoteCur.UserNum=Security.CurUser.UserNum;
			form.TaskNoteCur.IsNew=true;
			form.TaskNoteCur.Note=initialText;
			form.EditComplete=OnNoteEditComplete_Add;
			//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.
			form.Show(this);
			return true;
		}

		private void OnNoteEditComplete_CellDoubleClick(object sender) {
			NotesChanged=true;
			if(_taskOld.TaskStatus==TaskStatusEnum.Done) {//If task was marked Done when opened, we uncheck the Done checkbox so people can see the changes.
				checkDone.Checked=false;
			}
			FillGrid();
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(_taskCur);
		}

		private void butAddNote_Click(object sender,EventArgs e) {
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			AddNoteToTaskAndEdit();
		}

		private void OnNoteEditComplete_Add(object sender) {
			NotesChanged=true;
			if(_taskOld.TaskStatus==TaskStatusEnum.Done) {//If task was marked Done when opened, we uncheck the Done checkbox so people can see the changes.
				checkDone.Checked=false;
			}
			FillGrid();
			if(_mightNeedSetRead) {//'new' box is checked
				checkNew.Checked=false;
				_statusChanged=true;
				_mightNeedSetRead=false;//so that the automation won't happen again
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(_taskCur);
		}

		private void checkNew_Click(object sender,EventArgs e) {
			if(checkNew.Checked && checkDone.Checked) {
				checkDone.Checked=false;
			}
			_statusChanged=true;
			_mightNeedSetRead=false;//don't override user's intent
		}

		private void checkDone_Click(object sender,EventArgs e) {
			if(checkNew.Checked && checkDone.Checked) {
				checkNew.Checked=false;
			}
			_mightNeedSetRead=false;//don't override user's intent
		}

		private void FillObject() {
			if(_isTaskDeleted) {
				return;
			}
			listObjectType.SetSelectedEnum(_taskCur.ObjectType);
			if(_taskCur.ObjectType==TaskObjectType.None) {
				panelObject.Visible=false;
			}
			else if(_taskCur.ObjectType==TaskObjectType.Patient) {
				panelObject.Visible=true;
				labelObjectDesc.Text=Lan.g(this,"Patient Name");
				if(_taskCur.KeyNum>0) {
					_taskCur.PatientName=Patients.GetPat(_taskCur.KeyNum).GetNameLF();
					textObjectDesc.Text=_taskCur.PatientName+" - "+_taskCur.KeyNum;
				}
				else {
					textObjectDesc.Text="";
				}
			}
			else if(_taskCur.ObjectType==TaskObjectType.Appointment) {
				panelObject.Visible=true;
				labelObjectDesc.Text=Lan.g(this,"Appointment Desc");
				if(_taskCur.KeyNum>0) {
					Appointment AptCur=Appointments.GetOneApt(_taskCur.KeyNum);
					if(AptCur==null) {
						textObjectDesc.Text=Lan.g(this,"(appointment deleted)");
					}
					else {
						textObjectDesc.Text=Patients.GetPat(AptCur.PatNum).GetNameLF()
							+"  "+AptCur.AptDateTime.ToString()
							+"  "+AptCur.ProcDescript
							+"  "+AptCur.Note;
					}
				}
				else {
					textObjectDesc.Text="";
				}
			}
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
			TaskReminderType taskReminderType=_listTaskReminderTypeNames[comboReminderRepeat.SelectedIndex];
			panelReminderFrequency.Visible=true;
			panelReminderDays.Visible=false;
			datePickerReminder.Visible=false;
			timePickerReminder.Visible=false;
			int reminderFrequency=PIn.Int(textReminderRepeatFrequency.Text,false);
			if(taskReminderType==TaskReminderType.NoReminder) {
				panelReminderFrequency.Visible=false;
			}
			else if(taskReminderType==TaskReminderType.Once) {
				panelReminderFrequency.Visible=false;
				datePickerReminder.Visible=true;
				timePickerReminder.Visible=true;
			}
			else if(taskReminderType==TaskReminderType.Daily) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Day");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Days");
				}
			}
			else if(taskReminderType==TaskReminderType.Weekly) {
				panelReminderDays.Visible=true;
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Week");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Weeks");
				}
			}
			else if(taskReminderType==TaskReminderType.Monthly) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Month");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Months");
				}
			}
			else if(taskReminderType==TaskReminderType.Yearly) {
				if(reminderFrequency==1) {
					labelRemindFrequency.Text=Lan.g(this,"Year");
				}
				else {
					labelRemindFrequency.Text=Lan.g(this,"Years");
				}
			}
		}

		private void butBlue_Click(object sender,EventArgs e) {
			if(_priorityDefNumSelected==_triageBlueNum) {//Blue button is clicked while it's already blue
				_priorityDefNumSelected=_triageWhiteNum;//Change to white.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].DefNum==_triageWhiteNum) {
						comboTaskPriorities.SelectedIndex=i;//Change selection to the triage white
					}
				}	
			}
			else {//Blue button is clicked while it's red or white, simply change it to blue
				_priorityDefNumSelected=_triageBlueNum;//Change to blue.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].DefNum==_triageBlueNum) {
						comboTaskPriorities.SelectedIndex=i;//Change selection to the triage blue
					}
				}	
			}
		}

		private void butRed_Click(object sender,EventArgs e) {
			if(_priorityDefNumSelected==_triageRedNum) {//Red button is clicked while it's already red
				_priorityDefNumSelected=_triageWhiteNum;//Change to white.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].DefNum==_triageWhiteNum) {
						comboTaskPriorities.SelectedIndex=i;//Change combo selection to the triage white
					}
				}	
			}
			else {//Red button is clicked while it's blue or white, simply change it to red
				_priorityDefNumSelected=_triageRedNum;//Change to red.
				for(int i=0;i<_listTaskPriorities.Count;i++) {
					if(_listTaskPriorities[i].DefNum==_triageRedNum) {
						comboTaskPriorities.SelectedIndex=i;//Change combo selection to the triage red
					}
				}
				if(_taskListCur!=null && _taskListCur.TaskListNum==Tasks.TriageTaskListNum) {
					textDateTimeEntry.Text=MiscData.GetNowDateTime().ToString();
				}
			}
		}

		private void butAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textDescript.AppendText(FormA.CompletedNote);
				butEditAutoNote.Visible=GetHasAutoNotePrompt();
			}
		}

		private void splitContainerNoFlicker1_SplitterMoved(object sender,SplitterEventArgs e) {
			textDescript.Invalidate();//We do this so that the scrollbar will not disappear. The size is set through the anchors.
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerDescriptNote);
		}

		///<summary>This event is fired whenever the combo box is changed manually or the index is changed programmatically.</summary>
		private void comboTaskPriorities_SelectedIndexChanged(object sender,EventArgs e) {
			_priorityDefNumSelected=_listTaskPriorities[comboTaskPriorities.SelectedIndex].DefNum;
			butColor.BackColor=Defs.GetColor(DefCat.TaskPriorities,_priorityDefNumSelected);//Change the color swatch so people know the priority's color
		}

		private void comboTaskPriorities_SelectionChangeCommitted(object sender,EventArgs e) {
			if(PrefC.IsODHQ 
				//Changing the priority to 'Red' from another priority for a Triage task
				&& _listTaskPriorities[comboTaskPriorities.SelectedIndex].DefNum==_triageRedNum 
				&& _taskCur.PriorityDefNum!=_triageRedNum
				&& _taskListCur!=null && _taskListCur.TaskListNum==Tasks.TriageTaskListNum) 
			{
				textDateTimeEntry.Text=MiscData.GetNowDateTime().ToString();
			}
		}

		private void listObjectType_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(_taskCur.KeyNum>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The linked object will no longer be attached.  Continue?")) {
					FillObject();
					return;
				}
			}
			_taskCur.KeyNum=0;
			_taskCur.ObjectType=listObjectType.GetSelected<TaskObjectType>();
			FillObject();
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(Tasks.IsTaskDeleted(_taskCur.TaskNum)){
				SetFormToDeletedMode();
				MsgBox.Show(this,"Task has been deleted, no history can be retrieved.");
				return;
			}
			FormTaskHist FormTH=new FormTaskHist();
			FormTH.TaskNumCur=_taskCur.TaskNum;
			FormTH.Show();
		}

		private void butChange_Click(object sender,System.EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(_taskCur.ObjectType==TaskObjectType.Patient) {
				_taskCur.KeyNum=FormPS.SelectedPatNum;
			}
			if(_taskCur.ObjectType==TaskObjectType.Appointment) {
				using FormApptsOther FormA=new FormApptsOther(FormPS.SelectedPatNum,null);//Select only, can't create new appt so don't need pinboard appointments.
				FormA.AllowSelectOnly=true;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.Cancel) {
					return;
				}
				_taskCur.KeyNum=FormA.ListAptNumsSelected[0];
			}
			FillObject();
		}

		private void butGoto_Click(object sender,System.EventArgs e) {
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			if(!SaveCur()) {
				return;
			}
			GotoType=_taskCur.ObjectType;
			GotoKeyNum=_taskCur.KeyNum;
			DialogResult=DialogResult.OK;
			Close();
			FormOpenDental.S_TaskGoTo(GotoType,GotoKeyNum);
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			if(IsNew) {
				MessageBox.Show(Lan.g(this,"From User cannot be changed on new tasks. Save the task first."));
				return;
			}
			using FormLogOn FormChangeUser=new FormLogOn(isSimpleSwitch:true);
			FormChangeUser.ShowDialog();
			if(FormChangeUser.DialogResult==DialogResult.OK) {
				_taskCur.UserNum=FormChangeUser.CurUserSimpleSwitch.UserNum; //assign task new UserNum
				textUser.Text=Userods.GetName(_taskCur.UserNum); //update user textbox on task.
			}
		}

		private void textDescript_TextChanged(object sender,EventArgs e) {
			if(_mightNeedSetRead) {//'new' box is checked
				checkNew.Checked=false;
				_statusChanged=true;
				_mightNeedSetRead=false;//so that the automation won't happen again
			}
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
			Tasks.TaskEditCreateLog(Lan.g(this,"Copied Task Note"),_taskCur);
		}

		private string CreateCopyTask() {
			string taskText=
				Lan.g(this,"Tasknum")+" #"+_taskCur.TaskNum //tasknum
				+((_taskCur.ObjectType==TaskObjectType.Patient && _taskCur.KeyNum!=0) ? (" - "+Lan.g(this,"Patnum")+" #"+_taskCur.KeyNum) : ("")) //patnum
				+"\r\n"+_taskCur.DateTimeEntry.ToShortDateString() //date
				+" "+_taskCur.DateTimeEntry.ToShortTimeString() //time
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
			if(Tasks.IsTaskDeleted(_taskCur.TaskNum)){
				SetFormToDeletedMode();
				MsgBox.Show(this,"This task was deleted from elsewhere.  Cannot attach to job.");
				return;
			}
			Job jobNew=new Job();
			List<string> categoryList=Enum.GetNames(typeof(JobCategory)).ToList();
			//Queries can't be created from here
			categoryList.Remove(JobCategory.Query.ToString());	
			categoryList.Remove(JobCategory.MarketingDesign.ToString());	
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				categoryList.Remove(JobCategory.NeedNoApproval.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.SpecialProject,true)) {
				categoryList.Remove(JobCategory.SpecialProject.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true)) {
				categoryList.Remove(JobCategory.UnresolvedIssue.ToString());
			}
			using InputBox categoryChoose=new InputBox("Choose a job category",categoryList);
			if(categoryChoose.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(categoryChoose.comboSelection.SelectedIndex==-1) {
				MsgBox.Show(this,"You must choose a category to create a job.");
				return;
			}
			JobCategory category=(JobCategory)Enum.GetNames(typeof(JobCategory)).ToList().IndexOf(categoryChoose.comboSelection.GetSelected<string>());
			jobNew.Category=category;
			using InputBox titleBox=new InputBox("Provide a brief title for the job.");
			if(titleBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(String.IsNullOrEmpty(titleBox.textResult.Text)) {
				MsgBox.Show(this,"You must type a title to create a job.");
				return;
			}
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listJobPriorities.Count==0) {
				MsgBox.Show(this,"You have no priorities setup in definitions.");
				return;
			}
			jobNew.Title=titleBox.textResult.Text;
			long priorityNum=0;
			priorityNum=listJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("JobDefault")).DefNum;
			if(ListTools.In(jobNew.Category,JobCategory.Bug,JobCategory.Conversion,JobCategory.UnresolvedIssue)) {
				priorityNum=listJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault")).DefNum;
				jobNew.Requirements=CreateCopyTask();
			}
			else {
				jobNew.Requirements=CreateCopyTask();
			}
			jobNew.Priority=priorityNum==0?listJobPriorities.First().DefNum:priorityNum;
			jobNew.PhaseCur=JobPhase.Concept;
			jobNew.UserNumConcept=Security.CurUser.UserNum;
			jobNew.ProposedVersion=JobProposedVersion.Current;
			JobLink jobLinkNew=new JobLink();
			JobLink jobLinkTask=new JobLink();
			jobLinkTask.LinkType=JobLinkType.Task;
			jobLinkTask.FKey=_taskCur.TaskNum;
			Jobs.Insert(jobNew);
			jobLinkNew.JobNum=jobNew.JobNum;
			jobLinkTask.JobNum=jobNew.JobNum;
			JobLinks.Insert(jobLinkTask);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
			FillComboJobs();
			FormOpenDental.S_GoToJob(jobNew.JobNum);
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,_taskCur.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(_taskCur,_listTaskNotes,new List<long>() { signalNum });
		}

		public void OnTaskEdited() {
			//This gets hit even when this window is the one that made a change.  This means we will refresh the grid even though we just did.
			//In the future we might think about enhancing this window to keep track of which signals it inserted.
			if(IsNew) {
				return;//If this task is new then no one else can edit it
			}
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start,"TaskNum: "+_taskCur.TaskNum.ToString());
			Task taskDb=Tasks.GetOne(_taskCur.TaskNum);
			if(taskDb==null) {//Task was deleted
				SetFormToDeletedMode();
				Logger.LogToPath("",LogPath.Signals,LogPhase.End,"TaskNum: "+_taskCur.TaskNum.ToString());
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
			Logger.LogToPath("",LogPath.Signals,LogPhase.End,"TaskNum: "+_taskCur.TaskNum.ToString());
		}

		///<summary>Does validation and then updates the _taskCur object with the current content of the TaskEdit window.</summary>
		private bool SaveCur() {
			if(!textDateTask.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			DateTime dateTimeEntry=DateTime.MinValue;
			TaskReminderType taskReminderType=_listTaskReminderTypeNames[comboReminderRepeat.SelectedIndex];
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
			if(_taskCur.TaskListNum==-1) {
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
				_taskCur.ReminderType=taskReminderType;
				_taskCur.ReminderFrequency=0;
				if(taskReminderType!=TaskReminderType.Once) {
					_taskCur.ReminderFrequency=PIn.Int(textReminderRepeatFrequency.Text);
				}
				if(String.IsNullOrEmpty(_taskCur.ReminderGroupId)) {//Make a new ID if it's blank no matter what.  Could be an old task being changed.
					Tasks.SetReminderGroupId(_taskCur);
				}
			}
			else {
				_taskCur.ReminderType=TaskReminderType.NoReminder;
				_taskCur.ReminderGroupId="";
			}
			if(taskReminderType==TaskReminderType.Once) {
				_taskCur.DateTimeEntry=new DateTime(datePickerReminder.Value.Date.Year,datePickerReminder.Value.Date.Month,datePickerReminder.Value.Date.Day,
					timePickerReminder.Value.TimeOfDay.Hours,timePickerReminder.Value.TimeOfDay.Minutes,timePickerReminder.Value.TimeOfDay.Seconds);
			}
			else {
				_taskCur.DateTimeEntry=PIn.DateT(textDateTimeEntry.Text);
				if(taskReminderType!=TaskReminderType.NoReminder && IsNew && DateTime.Now>_taskCur.DateTimeEntry) { //New Reminder Task.
					//Could be a future reminder, so we want to calculate when the reminder would be due.
					_taskCur.DateTimeEntry=Tasks.CalcTaskForwardDate(_taskCur);
				}
			}
			//Techs want to be notified of any changes made to completed tasks.
			//Check if the task list changed on a task marked Done.
			if(_taskCur.TaskListNum!=_taskOld.TaskListNum	&& _taskOld.TaskStatus==TaskStatusEnum.Done) {
				//Forcing the status to viewed will put the task in other user's "New for" task list but not the user that made the change.
				_taskCur.TaskStatus=TaskStatusEnum.Viewed;
				checkDone.Checked=false;
			}
			if(checkDone.Checked) {
				_taskCur.TaskStatus=TaskStatusEnum.Done;//global even if new status is tracked by user
				TaskUnreads.DeleteForTask(_taskCur);//clear out taskunreads. We have too many tasks to read the done ones.
			}
			else {//because it can't be both new and done.
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					if(_taskCur.TaskStatus==TaskStatusEnum.Done) {
						_taskCur.TaskStatus=TaskStatusEnum.Viewed;
					}
					if(Tasks.IsReminderTask(_taskCur) && _taskCur.DateTimeEntry>DateTime.Now) {//Future Reminder.
						if(!_taskCur.IsUnread) {
							TaskUnreads.SetUnread(Security.CurUser.UserNum,_taskCur);//Future Reminders need to stay Unread for this user so they popup at due time.
						}
					}
					else if(!_startedInOthersInbox) {
						if(_mightNeedSetRead && _taskCur.IsUnread) {//This user opened the task, therefore, unless they manually changed checkNew, it's now read.
							checkNew.Checked=false;
							_statusChanged=true;
						}
						//Because the task could have been modified by another user at this point
						//RefreshTask();
						if(checkNew.Checked) {
							TaskUnreads.SetUnread(Security.CurUser.UserNum,_taskCur);
						}
						else {
							TaskUnreads.SetRead(Security.CurUser.UserNum,_taskCur);
						}
					}
					else if(!checkNew.Checked) {//Just in case, checkbox should not be enabled or checked when _startedInOthersInbox is true.
						//A task was sent to a tasklist I'm subscribed to, but before I had a chance to open it, it was sent to someone else's inbox.
						//If I open this task, I've read it, so it should still be marked as read for me.
						TaskUnreads.SetRead(Security.CurUser.UserNum,_taskCur);
						_statusChanged=true;
					}
				}
				else {//tracked globally, the old way
					if(checkNew.Checked) {
						_taskCur.TaskStatus=TaskStatusEnum.New;
					}
					else {
						_taskCur.TaskStatus=TaskStatusEnum.Viewed;
					}
				}
			}
			//UserNum no longer allowed to change automatically
			//if(resetUser && TaskCur.Descript!=textDescript.Text){
			//	TaskCur.UserNum=Security.CurUser.UserNum;
			//}
			if(_taskCur.TaskStatus==TaskStatusEnum.Done && textDateTimeFinished.Text=="") {
				_taskCur.DateTimeFinished=DateTime.Now;
			}
			else {
				_taskCur.DateTimeFinished=PIn.DateT(textDateTimeFinished.Text);
			}
			_taskCur.Descript=textDescript.Text;
			_taskCur.DescriptOverride=textDescriptOverride.Text;
			_taskCur.DateTask=PIn.Date(textDateTask.Text);
			_taskCur.DateType=(TaskDateType)comboDateType.SelectedIndex;
			if(!checkFromNum.Checked) {//user unchecked the box. Never allowed to check if initially unchecked
				_taskCur.FromNum=0;
			}
			//ObjectType already handled
			//Cur.KeyNum already handled
			_taskCur.PriorityDefNum=_priorityDefNumSelected;
			try {
				if(IsNew) {
					_taskCur.IsNew=true;
					Tasks.Update(_taskCur,_taskOld);
				}
				else {
					if(butRefresh.Visible) {	
						//force them to refresh before pressing ok.
						if(MsgBox.Show(this,MsgBoxButtons.YesNo,"There have been changes to the task since it has been loaded. "+
						" You must refresh before saving. Would you like to refresh now?")) 
						{
							RefreshTask();
						}
						return false;					
					}
					if(!_taskCur.Equals(_taskOld)) {//If user clicks OK without making any changes, then skip.
						Cursor=Cursors.WaitCursor;
						Tasks.Update(_taskCur,_taskOld);//if task has already been altered, then this is where it will fail.
						Cursor=Cursors.Default;
					}
					if(!_taskCur.Equals(_taskOld) || NotesChanged) {//We want to make a TaskHist entry if notes were changed as well as if the task was changed.
						TaskHist taskHist=new TaskHist(_taskOld);
						taskHist.UserNumHist=Security.CurUser.UserNum;
						taskHist.IsNoteChange=NotesChanged;
						TaskHists.Insert(taskHist);
					}
				}
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshTask();
		}

		private void RefreshTask() {
			if(_taskCur==null) {
				MsgBox.Show(this,"This task is in an invalid state. The task will now be closed so it can be opened again in a valid state.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			_taskCur=Tasks.GetOne(_taskCur.TaskNum);
			if(_taskCur==null) {
				MsgBox.Show(this,"This task has been deleted and must be closed.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			_taskListCur=TaskLists.GetOne(_taskCur.TaskListNum);
			LoadTask();
			butRefresh.Visible=false;
			labelTaskChanged.Visible=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//NOTE: Any changes here need to be made to UserControlTasks.Delete_Clicked() as well.
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			if(!IsNew) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
				if(Tasks.GetOne(_taskCur.TaskNum)==null) {
					MsgBox.Show(this,"Task already deleted.");//if task has remained open and has become stale on a workstation.
					butDelete.Enabled=false;
					butOK.Enabled=false;
					butSend.Enabled=false;
					butAddNote.Enabled=false;
					Text+=" - {"+Lan.g(this,"Deleted")+"}";
					return;
				}
				//TaskListNum=-1 is only possible if it's new.  This will never get hit if it's new.
				if(_taskCur.TaskListNum==0) {
					Tasks.TaskEditCreateLog(Lan.g(this,"Deleted task"),_taskCur);
				}
				else {
					string logText=Lan.g(this,"Deleted task from tasklist");
					TaskList tList=TaskLists.GetOne(_taskCur.TaskListNum);
					if(tList!=null) {
						logText+=" "+tList.Descript;
					}
					else {
						logText+=". Task list no longer exists";
					}
					logText+=".";
					Tasks.TaskEditCreateLog(logText,_taskCur);
				}
			}
			Tasks.Delete(_taskCur.TaskNum);//always do it this way to clean up all four tables
			SendSignalsRefillLocal(_taskCur,_taskCur.TaskListNum,false);
			TaskHist taskHistory=new TaskHist(_taskOld);
			taskHistory.IsNoteChange=NotesChanged;
			taskHistory.UserNum=Security.CurUser.UserNum;
			TaskHists.Insert(taskHistory);
			SecurityLogs.MakeLogEntry(Permissions.TaskEdit,0,"Task "+POut.Long(_taskCur.TaskNum)+" deleted",0);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butReply_Click(object sender,EventArgs e) {
			//This can't happen if IsNew
			//This also can't happen if the task is mine with no replies.
			//Button not visible unless a ReplyToUserNum has been calculated successfully.
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			long taskListNumCur=_taskCur.TaskListNum;
			long inbox=Userods.GetInbox(_replyToUserNum);
			if(inbox==0) {
				MsgBox.Show(this,"No inbox has been set up for this user yet.");
				return;
			}
			if(!NotesChanged && textDescript.Text==_taskCur.Descript) {//nothing changed
				FormTaskNoteEdit form=new FormTaskNoteEdit();
				form.TaskNoteCur=new TaskNote();
				form.TaskNoteCur.TaskNum=_taskCur.TaskNum;
				form.TaskNoteCur.DateTimeNote=DateTime.Now;//Will be slightly adjusted at server.
				form.TaskNoteCur.UserNum=Security.CurUser.UserNum;
				form.TaskNoteCur.IsNew=true;
				form.TaskNoteCur.Note="";
				form.EditComplete=OnNoteEditComplete_Reply;
				form.Show(this);//non-modal subwindow, but if the parent is closed by the user when the child is open, then the child is forced closed along with the parent and after the parent.
				return;
			}
			_taskCur.TaskListNum=inbox;
			if(!SaveCur()) {
				return;
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			//Both tasklistnums are different, since SaveCur() returned true.  Thereore, send signals for both task lists.
			SendSignalsRefillLocal(_taskCur,taskListNumCur);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void OnNoteEditComplete_Reply(object sender) {
			if(_mightNeedSetRead) {//'new' box is checked
				checkNew.Checked=false;
				_statusChanged=true;
				_mightNeedSetRead=false;//so that the automation won't happen again
			}
			long taskListNumCur=_taskCur.TaskListNum;
			_taskCur.TaskListNum=Userods.GetInbox(_replyToUserNum);
			if(!SaveCur()) {
				return;
			}
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
			TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			SendSignalsRefillLocal(_taskCur,taskListNumCur);
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Send to another user.</summary>
		private void butSend_Click(object sender,EventArgs e) {
			//This button is always present.
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			long taskListNumCur=_taskCur.TaskListNum;
			using FormTaskListSelect FormT=new FormTaskListSelect(listObjectType.GetSelected<TaskObjectType>(),IsNew);
			FormT.ShowDialog();
			if(FormT.DialogResult!=DialogResult.OK) {
				return;
			}
			_taskCur.TaskListNum=FormT.ListSelectedLists[0];
			_taskListCur=TaskLists.GetOne(_taskCur.TaskListNum);
			_taskCur.ParentDesc=_taskListCur.Descript;
			textTaskList.Text=_taskListCur.Descript;
			if(!SaveCur()) {
				return;
			}
			SaveCopy(FormT.ListSelectedLists.Skip(1).ToList());//Copies/Inserts task and sends to inboxes if multiple lists were selected.
			//Check for changes.  If something changed, send a signal.
			if(NotesChanged || !_taskCur.Equals(_taskOld) || _statusChanged) {
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
				TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			}
			SendSignalsRefillLocal(_taskCur,taskListNumCur);
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Saves a copy of the task</summary>
		private bool SaveCopy(List<long> listTaskListNums) {
			foreach(long taskListNum in listTaskListNums) {
				Task taskCopy=_taskCur.Copy();
				taskCopy.TaskListNum=taskListNum;
				taskCopy.IsUnread=true;
				taskCopy.ReminderGroupId="";
				if(taskCopy.ReminderType!=TaskReminderType.NoReminder && !PrefC.GetBool(PrefName.TasksUseRepeating)) {//Make a new ID if it's blank no matter what.  Could be an old task being changed.
					Tasks.SetReminderGroupId(taskCopy);
				}
				try {
					taskCopy.IsNew=true;
					Tasks.Insert(taskCopy);
					foreach(TaskNote note in _listTaskNotes) {
						TaskNote noteCopy=note.Copy();
						noteCopy.TaskNum=taskCopy.TaskNum;
						TaskNotes.Insert(noteCopy);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return false;
				}
				if(taskCopy.TaskListNum==Userods.GetInbox(Security.CurUser.UserNum)) {//My inbox.
					FormTaskEdit formT=new OpenDental.FormTaskEdit(taskCopy);//Maintain previous behavior. If I send to myself, should popup.
					formT.Show();//Non-modal. 
					UserControlTasks.RefillLocalTaskGrids(taskCopy,_listTaskNotes,null);//Refills local grids with _taskCur, which has a new taskNum now.
				}
				else {//Sent to someone else. Task should popup for that user.
					TaskUnreads.AddUnreads(taskCopy,Security.CurUser.UserNum);//Tell the database about all the users with unread tasks prior to sending signals.
					Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,taskCopy.TaskNum);//popup
					Signalods.SetInvalid(InvalidType.TaskList,KeyType.Undefined,taskCopy.TaskListNum);//signal so all instances of UserControlTasks refreshes.
				}
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
		
		///<summary>Sets all controls to read-only or disabled except cancel and copy button.</summary>
		private void DisableMostControls() {
			//Disable most controls.
			this.DisableAllExcept(butCancel,butCopy,textDateTimeEntry,textDateTimeFinished,labelTaskChanged,splitContainerDescriptNote);
			//Enable and set read-only for fields we want to allow copying from.
			#region splitContainerDescriptNote
			splitContainerDescriptNote.Enabled=true;  //This was probably already true but just in case
			butBlue.Enabled=false;
			butRed.Enabled=false;
			butAutoNote.Enabled=false;
			textDateTimeEntry.ReadOnly=true;
			textDateTimeFinished.ReadOnly=true;
			textDescript.ReadOnly=true;
			#endregion splitContainerDescriptNote
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			if(!SaveCur()) {//If user clicked OK without changing anything, then this will have no effect.
				return;
			}
			if(_taskCur.Equals(_taskOld) && !_statusChanged) {//if there were no changes, then don't bother with the signal
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			if(IsNew || textDescript.Text!=_taskCur.Descript //notes or descript changed
				|| (NotesChanged && _taskOld.TaskStatus==TaskStatusEnum.Done) //Because the taskunread would not have been inserted when saving the note
				|| (_taskOld.ReminderType==TaskReminderType.NoReminder && _taskCur.ReminderType!=TaskReminderType.NoReminder)) //Add taskunread for when "due"
			{ 
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
				TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
			}
			SendSignalsRefillLocal(_taskCur);
			DialogResult=DialogResult.OK;
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
			if(_patientPatNum!=task.KeyNum) {//Attached patient changed.
				if(_patientPatNum!=0) {//Previous Object was a Patient
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskPatient,KeyType.Undefined,_patientPatNum));//Signal for previous Patient.
				}
				if(_taskCur.ObjectType==TaskObjectType.Patient && task.KeyNum!=0) {//New Object is a Patient
					listSignalNums.Add(Signalods.SetInvalid(InvalidType.TaskPatient,KeyType.Undefined,task.KeyNum));//Signal for new Patient.
				}
			}
			UserControlTasks.RefillLocalTaskGrids(_taskCur,_listTaskNotes,listSignalNums,canKeepTask);
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			if(TaskNoteEditExists) {
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				return;
			}
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormTaskEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Abort) {
				return;
			}
			if(DialogResult==DialogResult.None && TaskNoteEditExists) {//This can only happen if the user closes the window using the X in the upper right.
				MsgBox.Show(this,"One or more task note edit windows are open and must be closed.");
				e.Cancel=true;
				return;
			}
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				//No more automation here
			}
			else {
				if(Security.CurUser!=null) {//Because tasks are modal, user may log off and this form may close with no user.
					TaskUnreads.SetRead(Security.CurUser.UserNum,_taskCur);//no matter why it was closed
				}
			}
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew) {
				Tasks.Delete(_taskCur.TaskNum);//Shouldn't be displayed in UserControlTasks yet, so no refill needed.
				SecurityLogs.MakeLogEntry(Permissions.TaskEdit,0,"Task "+POut.Long(_taskCur.TaskNum)+" deleted",0);
			}
			else if(NotesChanged) {//Note changed and dialogue result was not OK
				//This should only ever be hit if the user clicked cancel or X.  Everything else will have dialogue result OK and exit above.
				//Make a TaskHist entry to note that the task notes were changed.
				TaskHist taskHist = new TaskHist(_taskOld);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				taskHist.IsNoteChange=true;
				TaskHists.Insert(taskHist);
				//Task has already been invalidated in FromTaskNoteEdit when the Note was added/edited.  Other Users have already been notified the task changed.
				//Do not send new TaskInvalid signal.
			}
			//If a note was added to a Done task and the user hits cancel, the task status is set to Viewed because the note is still there and the task didn't move lists.
			if(NotesChanged && _taskOld.TaskStatus==TaskStatusEnum.Done && !butRefresh.Visible) {//notes changed on a task marked Done when the task was opened.
				if(checkDone.Checked) {//Will happen when the Done checkbox has been manually re-checked by the user or refreshing the task checked the box.
					return;
				}
				_taskCur.TaskStatus=TaskStatusEnum.Viewed;
				try {
					Tasks.Update(_taskCur,_taskOld);//if task has already been altered, then this is where it will fail.
				}
				catch {
					return;//Silently leave because the user could be trying to cancel out of a task that had been edited by another user.
				}
				Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,_taskCur.TaskNum);//popup
				TaskUnreads.AddUnreads(_taskCur,Security.CurUser.UserNum);//we also need to tell the database about all the users with unread tasks
				SendSignalsRefillLocal(_taskCur);
			}
		}

		private void ButEditAutoNote_Click(object sender,EventArgs e) {
			if(GetHasAutoNotePrompt()) {
				using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
				FormA.MainTextNote=textDescript.Text;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.OK) {
					textDescript.Text=FormA.CompletedNote;
					butEditAutoNote.Visible=GetHasAutoNotePrompt();
				}
			}
			else {
				MessageBox.Show(Lan.g(this,"No Auto Note available to edit."));
			}
		}

		private bool GetHasAutoNotePrompt() {
			return Regex.IsMatch(textDescript.Text,_autoNotePromptRegex);
		}
	}
}