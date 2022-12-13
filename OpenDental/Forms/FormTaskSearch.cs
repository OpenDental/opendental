using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace OpenDental {
	public partial class FormTaskSearch:FormODBase {

		private DataTable _tableTasks;
		private List<long> _listPreLoadedTaskNums;
		private List<Def> _listDefsTaskPriorities;
		///<summary>List of all users.</summary>
		private List<Userod> _listUserodsAll;
		///<summary>List of all users that are not hidden.</summary>
		private List<Userod> _listUserodsNotHidden;
		private Point _pointLastClicked;
		public TaskObjectType taskObjectTypeGoto;
		public long UserNum;
		public long KeyNumGoTo;
		public bool IsSelectionMode;
		public long TaskNumSelected;
		public string TaskNum;

		public FormTaskSearch(List<long> listPreLoadedTask=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPreLoadedTaskNums=listPreLoadedTask;
		}

		private void FormTaskSearch_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text="Cancel";
			}
			//Note: DateTime strings that are empty actually are " " due to how the empty datetime control behaves.
			_listDefsTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			long userNum=0;
			_listUserodsAll=Userods.GetDeepCopy();
			_listUserodsNotHidden=_listUserodsAll.Where(x => !x.IsHidden).ToList();
			FillComboUsers();
			comboPriority.Items.Add(Lan.g(this,"All"));
			for(int i=0;i<_listDefsTaskPriorities.Count;i++) {
				comboPriority.Items.Add(_listDefsTaskPriorities[i].ItemName);
			}
			comboPriority.SelectedIndex=0;
			checkLimit.Checked=true;
			if(PrefC.HasReportServer) {
				checkReportServer.Checked=true;
			}
			else {
				checkReportServer.Visible=false;
			}
			if(_listPreLoadedTaskNums!=null || !string.IsNullOrEmpty(TaskNum)) {
				List<long> listTaskNums=new List<long>() {};
				if(_listPreLoadedTaskNums!=null) {
					listTaskNums=_listPreLoadedTaskNums;
					textTaskNum.Text=string.Join(",",listTaskNums);//Reflect taskNums in UI
				}
				if(!string.IsNullOrEmpty(TaskNum)) {
					listTaskNums.Add(PIn.Long(TaskNum));
					textTaskNum.Text=string.Join(",",listTaskNums);
				}
				_tableTasks=Tasks.GetDataSet(userNum,new List<long>(),listTaskNums," "," "," "," ",textIncluding.Text,textExcluding.Text,0,0,checkBoxIncludesTaskNotes.Checked,
					checkBoxIncludeCompleted.Checked,checkIncludeAttachments.Checked,true,checkReportServer.Checked);
				FillGrid();
			}
			textTaskNum.Select();
			FillContextMenu();
		}

		private void FillComboUsers() {
			comboUsers.Items.Clear();
			comboUsers.Items.Add(Lan.g(this,"All"));
			comboUsers.Items.Add(Lan.g(this,"Me"));
			comboUsers.SelectedIndex=0;//Always default to All.
			if(checkShowHiddenUsers.Checked) {
				comboUsers.Items.AddList(_listUserodsAll,x => x.UserName);
				return;
			}
			comboUsers.Items.AddList(_listUserodsNotHidden,x => x.UserName);
		}

		private void FillGrid() {
			gridTasks.BeginUpdate();
			gridTasks.Columns.Clear();
			gridTasks.ListGridRows.Clear();
			GridColumn col=new GridColumn("Created",70,HorizontalAlignment.Left);
			gridTasks.Columns.Add(col);
			col=new GridColumn("Completed",70,HorizontalAlignment.Left);
			gridTasks.Columns.Add(col);
			col=new GridColumn("Description",70){ IsWidthDynamic=true };
			gridTasks.Columns.Add(col);
			if(_tableTasks==null) {
				gridTasks.EndUpdate();
				return;
			}
			GridRow row;
			for(int i=0;i<_tableTasks.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableTasks.Rows[i]["dateCreate"].ToString());
				row.Cells.Add(_tableTasks.Rows[i]["dateComplete"].ToString());
				row.Cells.Add(_tableTasks.Rows[i]["description"].ToString());
				row.Note=_tableTasks.Rows[i]["note"].ToString();
				row.ColorLborder=Color.Black;
				row.ColorText=Color.FromArgb(PIn.Int(_tableTasks.Rows[i]["color"].ToString()));
				gridTasks.ListGridRows.Add(row);
				row.Tag=_tableTasks.Rows[i]["TaskNum"].ToString();
			}
			gridTasks.EndUpdate();
		}
		
		///<summary>MenuItems here appear and work the same as in UserControlTasks.cs</summary>
		private void FillContextMenu() {
			if(gridTasks.ContextMenu!=null) {
				return;
			}
			gridTasks.ContextMenu=new ContextMenu();//ODGrid will automatically attach the default Popups
			MenuItem menuItemGoTo=gridTasks.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "Go To");
			menuItemGoTo=new MenuItem("Go To",menuItemGoTo_Click);
			menuItemGoTo.Enabled=false;
			menuItemGoTo.Visible=true;
			gridTasks.ContextMenu.MenuItems.Add(menuItemGoTo);
			if(PrefC.IsODHQ) {
				MenuItem menuItemNavToJob=gridTasks.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "Navigate to Job");
				menuItemNavToJob=new MenuItem("Navigate to Job");
				menuItemNavToJob.Enabled=false;
				menuItemNavToJob.Visible=true;
				gridTasks.ContextMenu.MenuItems.Add(menuItemNavToJob);
			}
			gridTasks.ContextMenu.Popup+=MenuPopup;
		}

		private void MenuPopup(object sender,EventArgs e) {
			int mouseLocationGridRow=gridTasks.PointToRow(_pointLastClicked.Y);
			MenuItem menuItemGoTo=gridTasks.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "Go To");
			MenuItem menuItemNavJob=gridTasks.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "Navigate to Job");
			if(mouseLocationGridRow==-1) {
				menuItemSetEnabled(menuItemGoTo,false);
				menuItemSetEnabled(menuItemNavJob,false);
				return;
			}
			long taskNum=PIn.Long(gridTasks.ListGridRows[mouseLocationGridRow].Tag.ToString());
			Task task=Tasks.GetOne(taskNum);
			if(task==null) {//Verify a task exists.
				menuItemSetEnabled(menuItemGoTo,false);
				menuItemSetEnabled(menuItemNavJob,false);
				return;
			}
			if(task.ObjectType!=TaskObjectType.None && task.KeyNum>0) {
				menuItemSetEnabled(menuItemGoTo,true);
			}
			else {
				menuItemSetEnabled(menuItemGoTo,false);
			}
			menuItemSetEnabled(menuItemNavJob,false);
			if(!PrefC.IsODHQ) {
				return;
			}
			List<JobLink> listJobLinks=JobLinks.GetForTask(task.TaskNum);
			List<Job> listJobs=Jobs.GetMany(listJobLinks.Select(x => x.JobNum).ToList());
			//If there are no jobs attached to the task, then kickout
			if(listJobs.Count<=0) {
				return;
			}
			menuItemNavJob.MenuItems.Clear(); //clear whatever items were in the menu before.
			MenuItem menuItemNew;
			string title;
			//Get a job num that matches the column in task menu
			for(int i=0;i<listJobs.Count;i++) {
				Job selectedJob=listJobs[i];
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
				menuItemNew=new MenuItem(title);
				menuItemNew.Tag=selectedJob;
				menuItemNew.Click+=(sender,e) => menuNavJob_Click(sender,e,selectedJob);	//set a custom click event
				menuItemNavJob.MenuItems.Add(menuItemNew);
			}
			menuItemSetEnabled(menuItemNavJob,true);
		}

		private void gridTasks_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			_pointLastClicked=e.Location;
		}

		private void menuItemSetEnabled(MenuItem menuItem,bool isEnabled) { 
			if(menuItem!=null) {
				menuItem.Enabled=isEnabled;
			}
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			int index=gridTasks.PointToRow(_pointLastClicked.Y);
			string strTaskNum=(string)gridTasks.ListGridRows[index].Tag;
			long taskNum=PIn.Long(strTaskNum);
			//not even allowed to get to this point unless a valid task
			Task task=Tasks.GetOne(taskNum);
			taskObjectTypeGoto=task.ObjectType;
			KeyNumGoTo=task.KeyNum;
			FormOpenDental.S_TaskGoTo(taskObjectTypeGoto,KeyNumGoTo);
		}

		private void menuNavJob_Click(object sender,EventArgs e,Job selectedJob) {
			int index=gridTasks.PointToRow(_pointLastClicked.Y);
			string strTaskNum=(string)gridTasks.ListGridRows[index].Tag;
			long taskNum=PIn.Long(strTaskNum);
			//not even allowed to get to this point unless a valid task
			Task task=Tasks.GetOne(taskNum);
			taskObjectTypeGoto=task.ObjectType;
			KeyNumGoTo=task.KeyNum;
			FormOpenDental.S_GoToJob(selectedJob.JobNum);
		}

		private void textTaskNum_KeyUp(object sender,KeyEventArgs e) {
			if(textTaskNum.Text.Length<7) {
				return;
			}
			//If invalid characters, then a messagebox pops up in RefreshTable.
			//Clicking enter in that window triggers KeyUp here.  Unclear why, but this solves it.
			if(e.KeyCode==Keys.Enter) {
				return;
			}
			RefreshTable();
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshTable();
			FillGrid();
		}

		private void RefreshTable() {
			long priority=0;
			if(comboPriority.SelectedIndex!=0) {
				priority=_listDefsTaskPriorities[comboPriority.SelectedIndex-1].DefNum;
			}
			long userNum=0;
			if(comboUsers.SelectedIndex==1){//Me
				userNum=Security.CurUser.UserNum;
			}
			else if(comboUsers.SelectedIndex!=0) {
				userNum=comboUsers.GetSelected<Userod>().UserNum;
			}
			List<long> listTaskListNums=new List<long>();
			if(textTaskList.Text!="") {
				listTaskListNums=TaskLists.GetNumsByDescription(textTaskList.Text,checkReportServer.Checked);
				if(listTaskListNums.Count==0) {
					MsgBox.Show(this,"Task List not found.");
					return;
				}
			}
			List<long> listTaskNums=new List<long>() {};
			if(textTaskNum.Text!="") {
				try {
					listTaskNums=textTaskNum.Text.Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x =>PIn.Long(x)).ToList();
				}
				catch {
					MsgBox.Show(this,"Invalid Task Num format.");
					return;
				}
			}
			long patNum=0;
			if(textPatNum.Text!="") {
				try {
					patNum=PIn.Long(textPatNum.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid PatNum format.");
					return;
				}
			}
			_tableTasks=Tasks.GetDataSet(userNum,listTaskListNums,listTaskNums,dateCreatedFrom.Text,dateCreatedTo.Text,dateCompletedFrom.Text,
				dateCompletedTo.Text,textIncluding.Text,textExcluding.Text,priority,patNum,checkBoxIncludesTaskNotes.Checked,checkBoxIncludeCompleted.Checked,
				checkIncludeAttachments.Checked,checkLimit.Checked,checkReportServer.Checked);
		}

		private void gridTasks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long taskNum=PIn.Long(gridTasks.ListGridRows[e.Row].Tag.ToString());
			if(IsSelectionMode) {
				TaskNumSelected=taskNum;
				DialogResult=DialogResult.OK;
				return;
			}
			Task task=Tasks.GetOne(taskNum);
			if(task!=null) {
				FormTaskEdit formTaskEdit=new FormTaskEdit(task);
				formTaskEdit.Show();
			}
			else {
				MsgBox.Show(this,"The task no longer exists.");
			}
		}

		private void checkShowHiddenUsers_Click(object sender,EventArgs e) {
			FillComboUsers();
		}

		private void butPatPicker_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			if(formPatientSelect.ShowDialog()==DialogResult.OK) {
				long patNum=formPatientSelect.PatNumSelected;
				textPatNum.Text=patNum.ToString();
			}
		}

		private void butUserPicker_Click(object sender,EventArgs e) {
			using FormUserPick formUserPick=new FormUserPick();
			formUserPick.ListUserodsFiltered=_listUserodsAll;
			formUserPick.IsSelectionmode=true;
			if(!checkShowHiddenUsers.Checked) {
				formUserPick.ListUserodsFiltered=_listUserodsNotHidden;
			}
			if(formUserPick.ShowDialog()==DialogResult.OK) {
				comboUsers.SetSelectedKey<Userod>(formUserPick.SelectedUserNum,x => x.UserNum);
			}
		}

		private void dateCreatedFrom_ValueChanged(object sender,EventArgs e) {
			dateCreatedFrom.CustomFormat=CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		}

		private void dateCreatedTo_ValueChanged(object sender,EventArgs e) {
			dateCreatedTo.CustomFormat=CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		}

		private void dateCompletedFrom_ValueChanged(object sender,EventArgs e) {
			dateCompletedFrom.CustomFormat=CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		}

		private void dateCompletedTo_ValueChanged(object sender,EventArgs e) {
			dateCompletedTo.CustomFormat=CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		}

		private void butClearCreated_Click(object sender,EventArgs e) {
			dateCreatedFrom.Value=DateTime.UtcNow;
			dateCreatedTo.Value=DateTime.UtcNow;
			dateCreatedFrom.CustomFormat=" ";
			dateCreatedTo.CustomFormat=" ";
		}

		private void butClearCompleted_Click(object sender,EventArgs e) {
			dateCompletedFrom.Value=DateTime.UtcNow;
			dateCompletedTo.Value=DateTime.UtcNow;
			dateCompletedFrom.CustomFormat=" ";
			dateCompletedTo.CustomFormat=" ";
		}

		private void butNewTask_Click(object sender,EventArgs e) {
			using FormTaskListSelect formtaskListSelect = new FormTaskListSelect(TaskObjectType.Patient);
			formtaskListSelect.Text=Lan.g(formtaskListSelect,"Add Task")+" - "+formtaskListSelect.Text;
			formtaskListSelect.ShowDialog();
			if(formtaskListSelect.DialogResult!=DialogResult.OK || formtaskListSelect.ListSelectedLists[0]==0) {
				return;
			}
			Task task = new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld = task.Copy();
			task.UserNum=Security.CurUser.UserNum;
			task.TaskListNum=formtaskListSelect.ListSelectedLists[0];
			using FormTaskEdit formTaskEdit = new FormTaskEdit(task,taskOld);
			formTaskEdit.IsNew=true;
			formTaskEdit.ShowDialog();//modal
			if(formTaskEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			TaskNumSelected=task.TaskNum;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}