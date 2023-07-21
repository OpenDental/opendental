using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental {
	public partial class FormTaskOptions:FormODBase {
		public bool ShowFinishedTasks;
		public bool ShowArchivedTaskLists;
		public DateTime DateTimeStartShowFinished;
		public bool DoSortApptDateTime;
		private UserOdPref _userOdPrefTaskCollapsed;
		private UserOdPref _userOdPrefTaskSound;

		public FormTaskOptions(bool showFinishedTasks,DateTime dateTimeStartShowFinished,bool doAptDateTimeSort,bool showArchivedTaskLists) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			checkShowFinished.Checked=showFinishedTasks;
			checkShowArchivedTaskLists.Checked=showArchivedTaskLists;
			textStartDate.Text=dateTimeStartShowFinished.ToShortDateString();
			checkTaskSortApptDateTime.Checked=doAptDateTimeSort;
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse);
			if(listUserOdPrefs.Count==0) {
				_userOdPrefTaskCollapsed=null;
			}
			else{
				_userOdPrefTaskCollapsed=listUserOdPrefs[0];
			}
			if(_userOdPrefTaskCollapsed==null) {
				checkCollapsed.Checked=false;
			}
			else{
				checkCollapsed.Checked=PIn.Bool(_userOdPrefTaskCollapsed.ValueString);
			}
			if(!showFinishedTasks) {
				labelStartDate.Enabled=false;
				textStartDate.Enabled=false;
			}
			//this returns a new userodpref if none is found
			_userOdPrefTaskSound=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskBlockedMakeSound);
			checkBlockedTaskPlaySound.Checked=PIn.Bool(_userOdPrefTaskSound.ValueString); //if new will return as false, otherwise current database value
		}

		private void checkShowFinished_Click(object sender,EventArgs e) {
			if(checkShowFinished.Checked) {
				labelStartDate.Enabled=true;
				textStartDate.Enabled=true;
				return;
			}
			labelStartDate.Enabled=false;
			textStartDate.Enabled=false;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textStartDate.IsValid()) {
				if(checkShowFinished.Checked) {
					MsgBox.Show(this,"Invalid finished task start date");
					return;
				}
				//We are not going to be using the textStartDate so not reason to warn the user, just reset it back to the default value.
				textStartDate.Text=DateTime.Today.AddDays(-7).ToShortDateString();
			}
			if(_userOdPrefTaskCollapsed==null) {
				_userOdPrefTaskCollapsed=new UserOdPref();
				_userOdPrefTaskCollapsed.Fkey=0;
				_userOdPrefTaskCollapsed.FkeyType=UserOdFkeyType.TaskCollapse;
				_userOdPrefTaskCollapsed.UserNum=Security.CurUser.UserNum;
				_userOdPrefTaskCollapsed.ValueString=POut.Bool(checkCollapsed.Checked);
				UserOdPrefs.Insert(_userOdPrefTaskCollapsed);
			}
			else { 
				_userOdPrefTaskCollapsed.ValueString=POut.Bool(checkCollapsed.Checked);
				UserOdPrefs.Update(_userOdPrefTaskCollapsed);
			}
			_userOdPrefTaskSound.ValueString=POut.Bool(checkBlockedTaskPlaySound.Checked);
			UserOdPrefs.Upsert(_userOdPrefTaskSound);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
			ShowFinishedTasks=checkShowFinished.Checked;
			ShowArchivedTaskLists=checkShowArchivedTaskLists.Checked;
			DateTimeStartShowFinished=PIn.Date(textStartDate.Text);//Note that this may have not been enabled but we'll pass it back anyway, won't be used.
			DoSortApptDateTime=checkTaskSortApptDateTime.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}