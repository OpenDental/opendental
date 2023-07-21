using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskSubscribers:FormODBase {
		public Task TaskForSubscribers;

		public FormTaskSubscribers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskSubscribers_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridODSubscribers.BeginUpdate();
			gridODSubscribers.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn("User",-1);
			gridColumn.IsWidthDynamic=true;
			gridODSubscribers.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Read",55,HorizontalAlignment.Center);
			gridODSubscribers.Columns.Add(gridColumn);
			gridODSubscribers.ListGridRows.Clear();
			List<long> listUserNums=TaskSubscriptions.GetSubscribersForTask(TaskForSubscribers);
			List<TaskUnread> listTaskUnreads=TaskUnreads.GetForTask(TaskForSubscribers.TaskNum);
			for(int i=0;i<listUserNums.Count;i++) {
				if(TaskForSubscribers.UserNum==listUserNums[i]) {
					continue;//Skip the creator of the task.
				}
				GridRow gridRow=new GridRow();
				string username=Userods.GetUser(listUserNums[i]).UserName;//Uses cache
				gridRow.Cells.Add(username);
				bool isUnread=listTaskUnreads.Exists(x => x.UserNum==listUserNums[i]);
				if(isUnread) {
					gridRow.Cells.Add("Unread");
				}
				else {
					gridRow.Cells.Add("Read");
				}
				gridODSubscribers.ListGridRows.Add(gridRow);
			}
			gridODSubscribers.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	}
}