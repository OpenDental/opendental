using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormTaskListSelect : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private TaskObjectType OType;
		private Label labelMulti;
		private CheckBox checkMulti;
		private CheckBox checkIncludeAll;
		public List<long> ListSelectedLists=new List<long>();
		private List<TaskList> _listUnfilteredTaskList;
		private List<TaskList> _listAllTaskLists;
		///<summary>Can be null. The inbox of the current users. Used in order to add a shortcut to send to at top of list.</summary>
		private TaskList _userCurTaskListInbox;
		private TextBox textFilter;
		private UI.GridOD gridMain;
		private Label label1;

		///<summary></summary>
		public FormTaskListSelect(TaskObjectType oType,bool IsTaskNew=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			OType=oType;
			checkMulti.Visible=IsTaskNew;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskListSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelMulti = new System.Windows.Forms.Label();
			this.checkMulti = new System.Windows.Forms.CheckBox();
			this.textFilter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIncludeAll = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(350, 481);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(350, 451);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelMulti
			// 
			this.labelMulti.Location = new System.Drawing.Point(11, 3);
			this.labelMulti.Name = "labelMulti";
			this.labelMulti.Size = new System.Drawing.Size(270, 57);
			this.labelMulti.TabIndex = 0;
			this.labelMulti.Text = "Pick task list to send to.";
			this.labelMulti.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkMulti
			// 
			this.checkMulti.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMulti.Location = new System.Drawing.Point(12, 112);
			this.checkMulti.Name = "checkMulti";
			this.checkMulti.Size = new System.Drawing.Size(182, 18);
			this.checkMulti.TabIndex = 1;
			this.checkMulti.Text = "Send copies to multiple";
			this.checkMulti.UseVisualStyleBackColor = true;
			this.checkMulti.Visible = false;
			this.checkMulti.CheckedChanged += new System.EventHandler(this.checkMulti_CheckedChanged);
			// 
			// textFilter
			// 
			this.textFilter.Location = new System.Drawing.Point(12, 90);
			this.textFilter.Name = "textFilter";
			this.textFilter.Size = new System.Drawing.Size(181, 20);
			this.textFilter.TabIndex = 0;
			this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
			this.textFilter.DoubleClick += new System.EventHandler(this.textFilter_DoubleClick);
			this.textFilter.Enter += new System.EventHandler(this.textFilter_Enter);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 70);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(183, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIncludeAll
			// 
			this.checkIncludeAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeAll.Location = new System.Drawing.Point(12, 129);
			this.checkIncludeAll.Name = "checkIncludeAll";
			this.checkIncludeAll.Size = new System.Drawing.Size(182, 18);
			this.checkIncludeAll.TabIndex = 2;
			this.checkIncludeAll.Text = "Show all task lists";
			this.checkIncludeAll.UseVisualStyleBackColor = true;
			this.checkIncludeAll.CheckedChanged += new System.EventHandler(this.checkIncludeSubTasks_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(11, 150);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(325, 355);
			this.gridMain.TabIndex = 3;
			this.gridMain.TitleVisible = false;
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormTaskListSelect
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(437, 517);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkIncludeAll);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textFilter);
			this.Controls.Add(this.labelMulti);
			this.Controls.Add(this.checkMulti);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTaskListSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Task List";
			this.Load += new System.EventHandler(this.FormTaskListSelect_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormTaskListSelect_Load(object sender, System.EventArgs e) {
			SetLabelText();
			_userCurTaskListInbox=TaskLists.GetOne(Security.CurUser.TaskListInBox);
			if(_userCurTaskListInbox!=null) {
				_userCurTaskListInbox.Descript=Lan.g(this,"My Inbox")+": "+_userCurTaskListInbox.Descript;
			}
			else {
				//Is null when the current user does not have an inbox set up
				//or if OType is TaskObjectType.Patient and the current users inbox is not of ObjectType Patient.
			}
			_listAllTaskLists=TaskLists.GetForDateType(TaskDateType.None,false);
			FillGrid();
		}

		private void LoadUnfilteredTaskLists() {
			switch(OType) {
				case TaskObjectType.Patient:
					_listUnfilteredTaskList=TaskLists.GetForObjectType(OType,false);
					break;
				case TaskObjectType.Appointment:
					_listUnfilteredTaskList=TaskLists.GetForObjectType(OType,false);
					_listUnfilteredTaskList.AddRange(
						GetUserInboxTaskLists().FindAll(x => x.ObjectType!=TaskObjectType.Appointment)
					);
					_listUnfilteredTaskList.Sort(SortTaskListByDescript);
					break;
				case TaskObjectType.None:
					_listUnfilteredTaskList=GetUserInboxTaskLists();
					this.Text=Lan.g(this,"Task Send User");//Form title assumes tasklist.
					break;
				default://Just in case
					_listUnfilteredTaskList=new List<TaskList>();
					break;
			}
		}

		///<summary>Returns a translated string representing the OType to be shown in the UI.</summary>
		private string GetOTypeDescription() {
			string retVal="";
			switch(OType) {
				case TaskObjectType.Patient:
				case TaskObjectType.Appointment:
					retVal=Lan.g(this,"task list");
					break;
				case TaskObjectType.None:
					retVal=Lan.g(this,"user");
					if(checkMulti.Checked) {
						retVal=Lan.g(this,"users");
					}
					break;
			}
			return retVal;
		}

		///<summary>Compares two given tasklist based on their Descript.</summary>
		private static int SortTaskListByDescript(TaskList x,TaskList y) {
			return x.Descript.CompareTo(y.Descript);
		}

		///<summary>Returns a list of TaskList inboxes for non hidden users with an inbox setup.</summary>
		private List<TaskList> GetUserInboxTaskLists() {
			List<TaskList> listUserInboxTaskLists=TaskLists.GetMany(Userods.GetDeepCopy(true).Select(x => x.TaskListInBox).ToList());
			return listUserInboxTaskLists.OrderBy(x => x.Descript).ThenBy(x => x.DateTimeEntry).ToList();
		}

		private void FillGrid() {
			List<TaskList> listTaskLists=null;
			//If we are showing all tasks lists, use a different method to fill and filter the list
			if(checkIncludeAll.Checked) {
				listTaskLists=GetListAll();
			}
			else {
				listTaskLists=GetListUser();
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Task List"),-150) { IsWidthDynamic=true,DynamicWeight=4 });
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Full Path"),-150) { IsWidthDynamic=true,DynamicWeight=5 });
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(listTaskLists.Select(x => new GridRow(x.Descript,x.TagOD?.ToString()??"") { Tag=x }));
			gridMain.EndUpdate();
			if(gridMain.ListGridRows.Count!=0) {
				gridMain.SetSelected(0);
			}
		}

		/// <summary>Builds a list of task list inboxes for a given user, and stores the resulting list and descriptions in the out vars.
		/// Also performs filtering of the list</summary>
		private List<TaskList> GetListUser() {
			LoadUnfilteredTaskLists();
			List<TaskList> retVal=GetFilteredList();
			if(_userCurTaskListInbox!=null) {
				if(_userCurTaskListInbox.Descript.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim())
					|| retVal.Any(x => x.TaskListNum==Security.CurUser.TaskListInBox))//Show "My Inbox:" shortcut if current users inbox is in filtered list.
				{
					retVal.Insert(0,_userCurTaskListInbox);
				}
			}
			TaskList taskListTriage=retVal.Find(x => x.Descript==" Triage");
			if(taskListTriage!=null) {
				retVal.Remove(taskListTriage);
				retVal.Insert(0,taskListTriage);
			}
			return retVal;
		}

		/// <summary>Fills listMain with all tasklists with DateType.None.  Each task list will also list out it's children lists.
		/// Items will be filtered the same way as FillListUser (based on textFilter).</summary>
		private List<TaskList> GetListAll() {
			List<TaskList> retVal=GetFilteredList();
			//Add the user's primary inbox to the top
			if(_userCurTaskListInbox!=null && _userCurTaskListInbox.Descript.Contains(textFilter.Text)) {
				retVal.Insert(0,_userCurTaskListInbox);
			}
			return retVal;
		}

		///<summary>Returns a filtered list of tasklists with descriptions that include paths for sublists.  If textFilter is not empty, the filtered
		///list will be ordered by those tasklists whose names start with textFilter above those whose names contain textFilter but don't start with it,
		///then by description.</summary>
		private List<TaskList> GetFilteredList() {
			//This will hold the final list of task lists, which will have different descriptions than normal.
			List<TaskList> listFilteredTaskLists=new List<TaskList>();
			Dictionary<long,TaskList> dictAllTaskLists=_listAllTaskLists.ToDictionary(x => x.TaskListNum);//Convert to dictionary for faster lookups
			List<TaskList> listTaskLists=checkIncludeAll.Checked?_listAllTaskLists:_listUnfilteredTaskList;
			StringBuilder sbSubListPath;//String builder because we want speeeed
			foreach(TaskList tList in listTaskLists) {
				TaskList taskListCur=tList.Copy(); //Copy so we don't modify the original in case it's another list's parent.
				sbSubListPath=new StringBuilder();
				long listParentNum=taskListCur.Parent;
				bool isSubList=false;
				while(dictAllTaskLists.TryGetValue(listParentNum,out TaskList parentTaskList)) {
					sbSubListPath.Insert(0,parentTaskList.Descript+(isSubList?">":""));
					listParentNum=parentTaskList.Parent;
					isSubList=true;
				}
				taskListCur.TagOD=sbSubListPath.ToString();
				listFilteredTaskLists.Add(taskListCur);//Store task list with extended name in our final list of tasklists
			}
			//We wait to filter until the entire description is created because one task might be filtered, but it's parent isn't
			if(!string.IsNullOrWhiteSpace(textFilter.Text)) {
				//Odering will be:
					//1. tasklists whose name starts with the search text
					//2. tasklists whose name doesn't start with, but does contain search text
					//3. tasklists whose name doesn't contain search text but whose path starts with the search text
					//4. tasklists whose name doesn't contain search text and whose path doesn't start with search text but whose path does contain search text
					//5. then by tasklist.Descript
				listFilteredTaskLists=listFilteredTaskLists
					.FindAll(x => x.Descript.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim())//either the tasklist name contains search text
						|| x.TagOD.ToString().ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim()))//or the path to the tasklist contains search text
					.OrderByDescending(x => x.Descript.ToUpper().Trim().StartsWith(textFilter.Text.ToUpper().Trim()))
					.ThenByDescending(x => x.Descript.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim()))
					.ThenByDescending(x => x.TagOD.ToString().ToUpper().Trim().StartsWith(textFilter.Text.ToUpper().Trim()))
					.ThenBy(x => x.Descript).ToList();
			}
			//if no search text entered, list is already sorted by description
			return listFilteredTaskLists;
		}

		///<summary>Sets the main label on this form depending on OType.</summary>
		private void SetLabelText() {
			//strUserSetup only shows when selecting a user to send to.
			string strUserSetup=(OType==TaskObjectType.None ? Lan.g(this,"If a user is not in the list, then their inbox has not been setup yet. ") : "");
			string objTypeStr=GetOTypeDescription();
			if(checkMulti.Checked) {
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
				labelMulti.Text=Lan.g(this,"Pick")+" "+objTypeStr+" "+Lan.g(this,"to send to.")+"  "
					+strUserSetup+Lan.g(this,"Click on")+" "+objTypeStr+" "+Lan.g(this,"to toggle.");
			}
			else {
				gridMain.SelectionMode=GridSelectionMode.One;
				labelMulti.Text=Lan.g(this,"Pick ")+objTypeStr+" "+Lan.g(this,"to send to.  ")+strUserSetup;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1 || (checkMulti.Checked && PrefC.IsODHQ)) {
				return;
			}
			ListSelectedLists=gridMain.SelectedTags<TaskList>().Select(x => x.TaskListNum).ToList();
			DialogResult=DialogResult.OK;
		}

		private void checkMulti_CheckedChanged(object sender,EventArgs e) {
			if(checkMulti.Checked && PrefC.IsODHQ) {//If user is in checkMulti state, deselect the list
				gridMain.SetAll(false);
				textFilter.Select();
			}
			SetLabelText();
		}

		private void checkIncludeSubTasks_CheckedChanged(object sender,EventArgs e) {
			//Refills the list.  This method checks the status of this checkbox.
			FillGrid();
		}

		protected override bool ProcessCmdKey(ref Message msg,Keys keyData) {
			if(keyData==Keys.Up) {
				if(!checkMulti.Checked && gridMain.ListGridRows.Count!=0) {
					switch(gridMain.GetSelectedIndex()) {
						case -1:
							//nothing selected, do nothing
							break;
						case 0:
							textFilter.Select();
							textFilter.Select(textFilter.Text.Length,0);
							return true;
						default:
							gridMain.SetSelected(gridMain.GetSelectedIndex()-1);
							return true;
					}
				}
			}
			else if(keyData==Keys.Down) {
				if(!checkMulti.Checked && gridMain.GetSelectedIndex()>-1 && gridMain.GetSelectedIndex()<gridMain.ListGridRows.Count-1) {
					gridMain.SetSelected(gridMain.GetSelectedIndex()+1);
					return true;
				}
			}
			bool retval=base.ProcessCmdKey(ref msg,keyData);
			return retval;
		}

		private void textFilter_Enter(object sender,EventArgs e) {
			textFilter.SelectAll();
		}

		private void textFilter_DoubleClick(object sender,EventArgs e) {
			textFilter.SelectAll();
		}
		
		private void textFilter_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				string msg=Lan.g(this,"Please select a ")+GetOTypeDescription()+Lan.g(this," first.");
				MessageBox.Show(msg);
				return;
			}
			ListSelectedLists=gridMain.SelectedTags<TaskList>().Select(x => x.TaskListNum).ToList();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}

