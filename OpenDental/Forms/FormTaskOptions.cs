using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental {
	public partial class FormTaskOptions:FormODBase {
		public bool IsShowFinishedTasks;
		public bool IsShowArchivedTaskLists;
		public DateTime DateTimeStartShowFinished;
		public bool IsSortApptDateTime;
		private UserOdPref _taskCollapsedPref;

		public FormTaskOptions(bool isShowFinishedTasks,DateTime dateTimeStartShowFinished,bool isAptDateTimeSort,bool isShowArchivedTaskLists) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			checkShowFinished.Checked=isShowFinishedTasks;
			checkShowArchivedTaskLists.Checked=isShowArchivedTaskLists;
			textStartDate.Text=dateTimeStartShowFinished.ToShortDateString();
			checkTaskSortApptDateTime.Checked=isAptDateTimeSort;
			List<UserOdPref> listPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskCollapse);
			_taskCollapsedPref=listPrefs.Count==0? null : listPrefs[0];
			checkCollapsed.Checked=_taskCollapsedPref==null ? false : PIn.Bool(_taskCollapsedPref.ValueString);
			if(!isShowFinishedTasks) {
				labelStartDate.Enabled=false;
				textStartDate.Enabled=false;
			}
		}

		private void checkShowFinished_Click(object sender,EventArgs e) {
			if(checkShowFinished.Checked) {
				labelStartDate.Enabled=true;
				textStartDate.Enabled=true;
			}
			else {
				labelStartDate.Enabled=false;
				textStartDate.Enabled=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textStartDate.IsValid()) {
				if(checkShowFinished.Checked) {
					MsgBox.Show(this,"Invalid finished task start date");
					return;
				}
				else {
					//We are not going to be using the textStartDate so not reason to warn the user, just reset it back to the default value.
					textStartDate.Text=DateTime.Today.AddDays(-7).ToShortDateString();
				}
			}
			if(_taskCollapsedPref==null) {
				_taskCollapsedPref=new UserOdPref();
				_taskCollapsedPref.Fkey=0;
				_taskCollapsedPref.FkeyType=UserOdFkeyType.TaskCollapse;
				_taskCollapsedPref.UserNum=Security.CurUser.UserNum;
				_taskCollapsedPref.ValueString=POut.Bool(checkCollapsed.Checked);
				UserOdPrefs.Insert(_taskCollapsedPref);
			}
			else { 
				_taskCollapsedPref.ValueString=POut.Bool(checkCollapsed.Checked);
				UserOdPrefs.Update(_taskCollapsedPref);
			}
			IsShowFinishedTasks=checkShowFinished.Checked;
			IsShowArchivedTaskLists=checkShowArchivedTaskLists.Checked;
			DateTimeStartShowFinished=PIn.Date(textStartDate.Text);//Note that this may have not been enabled but we'll pass it back anyway, won't be used.
			IsSortApptDateTime=checkTaskSortApptDateTime.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}