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
		public long TaskNumCur;
		///<summary>Contains all TaskHists for the given TaskNumCur. Does not include the "current" revision of non-deleted tasks.</summary>
		private List<TaskHist> _listTaskAudit;

		public FormTaskHist() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskHist_Load(object sender,EventArgs e) {
			_listTaskAudit=TaskHists.GetArchivesForTask(TaskNumCur);
			FillGrid();
		}

		private void FillGrid() {
			gridTaskHist.BeginUpdate();
			gridTaskHist.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTaskAudit","Create Date"),140);
			gridTaskHist.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Edit Date"),140);
			gridTaskHist.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Editing User"),80);
			gridTaskHist.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAudit","Changes"),100);
			gridTaskHist.ListGridColumns.Add(col);
			gridTaskHist.ListGridRows.Clear();
			GridRow row;//Row describes difference between current row and the Next row. Last row will be the last TaskHist compared to the current Task.
			for(int i=1;i<_listTaskAudit.Count;i++) {
				TaskHist taskHistCur=_listTaskAudit[i-1];
				TaskHist taskHistNext=_listTaskAudit[i];
				row=new GridRow();
				if(taskHistCur.DateTimeEntry==DateTime.MinValue) {
					row.Cells.Add(_listTaskAudit[i].DateTimeEntry.ToString());
				}
				else {
					row.Cells.Add(taskHistCur.DateTimeEntry.ToString());
				}
				row.Cells.Add(taskHistCur.DateTStamp.ToString());
				long usernum=taskHistCur.UserNumHist;
				if(usernum==0) {
					usernum=taskHistCur.UserNum;
				}
				row.Cells.Add(TaskHists.GetUserName(usernum));
				row.Cells.Add(TaskHists.GetChangesDescription(taskHistCur,taskHistNext));
				gridTaskHist.ListGridRows.Add(row);
			}
			//Compare the current task with the last hist entry (Add the "current revision" of the task if necessary.)
			if(_listTaskAudit.Count>0) {
				TaskHist taskHistCur=_listTaskAudit[_listTaskAudit.Count-1];
				Task task=Tasks.GetOne(TaskNumCur);
				if(task!=null) {
					TaskHist taskHistNext=new TaskHist(task);
					row=new GridRow();
					if(taskHistCur.DateTimeEntry==DateTime.MinValue) {
						row.Cells.Add(taskHistNext.DateTimeEntry.ToString());
					}
					else {
						row.Cells.Add(taskHistCur.DateTimeEntry.ToString());
					}
					row.Cells.Add(taskHistCur.DateTStamp.ToString());
					long usernum=taskHistCur.UserNumHist;
					if(usernum==0) {
						usernum=taskHistCur.UserNum;
					}
					row.Cells.Add(TaskHists.GetUserName(usernum));
					row.Cells.Add(TaskHists.GetChangesDescription(taskHistCur,taskHistNext));
					gridTaskHist.ListGridRows.Add(row);
				}
			}
			gridTaskHist.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

		

	}
}