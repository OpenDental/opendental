using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormTaskInboxSetup:FormODBase {
		private List<Userod> listUserods;
		private List<Userod> listUserodsOld;
		private List<TaskList> listTaskListsTrunk;

		public FormTaskInboxSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskInboxSetup_Load(object sender,EventArgs e) {
			listUserods=Userods.GetDeepCopy(true);
			listUserodsOld=Userods.GetDeepCopy(true);
			listTaskListsTrunk=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.All)
				.FindAll(x => x.TaskListStatus==TaskListStatusEnum.Active);
			listMain.Items.Add(Lan.g(this,"none"));
			listMain.Items.AddList(listTaskListsTrunk,x => x.Descript);
			FillGrid();
		}

		private void FillGrid(){
			//doesn't refresh from db because nothing actually gets saved until we hit the OK button.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTaskSetup","User"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTaskSetup","Inbox"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listUserods.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(listUserods[i].UserName);
				row.Cells.Add(GetDescription(listUserods[i].TaskListInBox));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private string GetDescription(long taskListNum) {
			if(taskListNum==0){
				return "";
			}
			for(int i=0;i<listTaskListsTrunk.Count;i++){
				if(listTaskListsTrunk[i].TaskListNum==taskListNum){
					return listTaskListsTrunk[i].Descript;
				}
			}
			return "";
		}

		private void butSet_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a user first.");
				return;
			}
			if(listMain.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item from the list first.");
				return;
			}
			if(listMain.SelectedIndex==0){
				listUserods[gridMain.GetSelectedIndex()].TaskListInBox=0;
			}
			else{
				listUserods[gridMain.GetSelectedIndex()].TaskListInBox=listMain.GetSelected<TaskList>().TaskListNum;
			}
			FillGrid();
			listMain.SelectedIndex=-1;
		}

		// Maps an exception message to a List of userods
		private class FailedUpdates {
			public string ExceptionMessage="";
			public List<Userod> ListUserods;
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool hasChanged=false;
			List<FailedUpdates> listFailedUserUpdates=new List<FailedUpdates>();
			for(int i=0;i<listUserods.Count;i++) {
				if(listUserods[i].TaskListInBox!=listUserodsOld[i].TaskListInBox) {
					try {
						Userods.Update(listUserods[i]);
						hasChanged=true;
					}
					catch(Exception ex) {
						FailedUpdates failedUpdates=listFailedUserUpdates.Find(x => x.ExceptionMessage==ex.Message);
						if(failedUpdates==null) {
							failedUpdates=new FailedUpdates();
							failedUpdates.ExceptionMessage=ex.Message;
							failedUpdates.ListUserods=new List<Userod>();
							listFailedUserUpdates.Add(failedUpdates);
						}
						failedUpdates.ListUserods.Add(listUserods[i]);
					}
				}
			}
			if(listFailedUserUpdates.Count>0) {//Inform user that user inboxes could not be updated.
				StringBuilder stringBuilder=new StringBuilder();
				for(int i=0;i<listFailedUserUpdates.Count;i++) {
					for(int j=0;j<listFailedUserUpdates[i].ListUserods.Count;j++) {
						stringBuilder.AppendLine("  "+listFailedUserUpdates[i].ListUserods[j].UserName+" - "+listFailedUserUpdates[i].ExceptionMessage);
					}
				}
				MessageBox.Show(this,Lans.g(this,"The following users could not be updated:\r\n")+stringBuilder.ToString());
			}
			if(hasChanged){
				DataValid.SetInvalid(InvalidType.Security);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}