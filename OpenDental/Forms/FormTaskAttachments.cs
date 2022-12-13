using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTaskAttachments:FormODBase {
		///<summary>Stores the task that the attachments belong to or will belong to.</summary>
		private Task _task;
		///<summary>Contains all the attachments for the passed in task</summary>
		private List<TaskAttachment> _listTaskAttachments;
		///<summary>Records whether a signal to refresh is required or not. Set to true if user adds or edits an attachment./summary>
		private bool _refreshRequired;
		/// <summary>If this  is set true, editing attachments will be disabled. </summary>
		public bool IsReadOnly=false;

		public FormTaskAttachments(Task task) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_task=task;
		}

		private void FormTaskAttachments_Load(object sender,EventArgs e) {
			_refreshRequired=false;
			FillGrid(true);
			butAdd.Enabled=!IsReadOnly;
		}

		private void FillGrid(bool refreshCache=false) {
			_listTaskAttachments=TaskAttachments.GetManyByTaskNum(_task.TaskNum);
	    gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTaskAttachments","Doc"),50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTaskAttachments","Description"),530);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listTaskAttachments.Count;i++){
				GridRow row=new GridRow();
				TaskAttachment taskAttachment=_listTaskAttachments[i];
				if(taskAttachment.DocNum>0) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(taskAttachment.Description);
				row.Tag=taskAttachment;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!CanAddAttachment()) {
				return;
			}
			TaskAttachment taskAttachment=new TaskAttachment();
			taskAttachment.IsNew=true;
			FormTaskAttachmentEdit formTaskAttachmentEdit=new FormTaskAttachmentEdit(_task);
			formTaskAttachmentEdit.TaskAttachmentCur=taskAttachment;
			formTaskAttachmentEdit.ShowDialog();
			if(formTaskAttachmentEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_refreshRequired=true;
			FillGrid();			
		}

		private bool CanAddAttachment() {
			if(Tasks.IsTaskDeleted(_task.TaskNum)) {
				MsgBox.Show(this,"The task for this attachment was deleted.");
				return false;
			}
			StringBuilder stringBuilder=new StringBuilder();
			if(PrefC.GetLong(PrefName.TaskAttachmentCategory)==0) {
				stringBuilder.AppendLine("Task attachments have not been set up. Set an Image Category with a usage of TaskAttachment in Setup>Definitions>Image Categories. " +
                    "Set the Image Category folder for attachments in Setup>Tasks.");
			}
			if(_task.ObjectType!=TaskObjectType.Patient || (_task.ObjectType==TaskObjectType.Patient && _task.KeyNum==0)) {
				stringBuilder.AppendLine("Task Object Type must be set to 'Patient' and a patient must be attached to task to add new attachments or edit existing attachments.");
			}
			if(!string.IsNullOrWhiteSpace(stringBuilder.ToString())){
				stringBuilder.AppendLine("Any existing attachments are viewable only.");
				MsgBox.Show(this,stringBuilder.ToString());
				return false;
			}
			return true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormTaskAttachmentEdit formTaskAttachmentEdit=new FormTaskAttachmentEdit(_task);
			formTaskAttachmentEdit.IsReadOnly=this.IsReadOnly;
			TaskAttachment taskAttachment=gridMain.SelectedTag<TaskAttachment>();
			if(taskAttachment==null) {
				MsgBox.Show(this,"Could not edit attachment.");
				return;
			}
			formTaskAttachmentEdit.TaskAttachmentCur=taskAttachment;
			formTaskAttachmentEdit.ShowDialog();
			if(formTaskAttachmentEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_refreshRequired=true;
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(_refreshRequired) {
				DialogResult=DialogResult.OK;
				return;
			}
			DialogResult=DialogResult.Cancel;
		}
	}
}