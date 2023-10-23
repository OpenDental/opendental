using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormTaskHist:FormODBase {
		public long TaskNum;
		///<summary>Contains all TaskHists for the given TaskNumCur. Does not include the "current" revision of non-deleted tasks.</summary>
		private List<TaskHist> _listTaskHistsAudit;

		public FormTaskHist() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskHist_Load(object sender,EventArgs e) {
			_listTaskHistsAudit=TaskHists.GetArchivesForTask(TaskNum);
			FillGrid();
		}

		private void FillGrid() {
			gridTaskHist.BeginUpdate();
			gridTaskHist.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTaskAudit","Create Date"),140);
			gridTaskHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Edit Date"),140);
			gridTaskHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Editing User"),80);
			gridTaskHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Changes"),100);
			gridTaskHist.Columns.Add(col);
			gridTaskHist.ListGridRows.Clear();
			for(int i=1;i<_listTaskHistsAudit.Count;i++) {
				TaskHist taskHist=_listTaskHistsAudit[i-1];
				TaskHist taskHistNext=_listTaskHistsAudit[i];
				GridRow row=new GridRow();//Row describes difference between current row and the Next row. Last row will be the last TaskHist compared to the current Task.
				if(taskHist.DateTimeEntry==DateTime.MinValue) {
					row.Cells.Add(_listTaskHistsAudit[i].DateTimeEntry.ToString());
				}
				else {
					row.Cells.Add(taskHist.DateTimeEntry.ToString());
				}
				row.Cells.Add(taskHist.DateTStamp.ToString());
				long userNum=taskHist.UserNumHist;
				if(userNum==0) {
					userNum=taskHist.UserNum;
				}
				row.Cells.Add(TaskHists.GetUserName(userNum));
				row.Cells.Add(TaskHists.GetChangesDescription(taskHist,taskHistNext));
				gridTaskHist.ListGridRows.Add(row);
			}
			//Compare the current task with the last hist entry (Add the "current revision" of the task if necessary.)
			if(_listTaskHistsAudit.Count<=0) {
				gridTaskHist.EndUpdate();
				return;
			}
			TaskHist taskHistLast=_listTaskHistsAudit[_listTaskHistsAudit.Count-1];
			Task task=Tasks.GetOne(TaskNum);
			if(task!=null) {
				TaskHist taskHistNext=new TaskHist(task);
				GridRow row=new GridRow();
				if(taskHistLast.DateTimeEntry==DateTime.MinValue) {
					row.Cells.Add(taskHistNext.DateTimeEntry.ToString());
				}
				else {
					row.Cells.Add(taskHistLast.DateTimeEntry.ToString());
				}
				row.Cells.Add(taskHistLast.DateTStamp.ToString());
				long userNum=taskHistLast.UserNumHist;
				if(userNum==0) {
					userNum=taskHistLast.UserNum;
				}
				row.Cells.Add(TaskHists.GetUserName(userNum));
				row.Cells.Add(TaskHists.GetChangesDescription(taskHistLast,taskHistNext));
				gridTaskHist.ListGridRows.Add(row);
			}
			gridTaskHist.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

		

	}
}