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
		private List<Userod> UserList;
		private List<Userod> UserListOld;
		private List<TaskList> TrunkList;

		public FormTaskInboxSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskInboxSetup_Load(object sender,EventArgs e) {
			UserList=Userods.GetDeepCopy(true);
			UserListOld=Userods.GetDeepCopy(true);
			TrunkList=TaskLists.RefreshMainTrunk(Security.CurUser.UserNum,TaskType.All)
				.FindAll(x => x.TaskListStatus==TaskListStatusEnum.Active);
			listMain.Items.Add(Lan.g(this,"none"));
			listMain.Items.AddList(TrunkList,x => x.Descript);
			FillGrid();
		}

		private void FillGrid(){
			//doesn't refresh from db because nothing actually gets saved until we hit the OK button.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTaskSetup","User"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTaskSetup","Inbox"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<UserList.Count;i++){
				row=new GridRow();
				row.Cells.Add(UserList[i].UserName);
				row.Cells.Add(GetDescription(UserList[i].TaskListInBox));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private string GetDescription(long taskListNum) {
			if(taskListNum==0){
				return "";
			}
			for(int i=0;i<TrunkList.Count;i++){
				if(TrunkList[i].TaskListNum==taskListNum){
					return TrunkList[i].Descript;
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
				UserList[gridMain.GetSelectedIndex()].TaskListInBox=0;
			}
			else{
				UserList[gridMain.GetSelectedIndex()].TaskListInBox=listMain.GetSelected<TaskList>().TaskListNum;
			}
			FillGrid();
			listMain.SelectedIndex=-1;
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool changed=false;
			Dictionary<string,List<Userod>> dictFailedUserUpdates=new Dictionary<string, List<Userod>>();
			for(int i=0;i<UserList.Count;i++){
				if(UserList[i].TaskListInBox!=UserListOld[i].TaskListInBox){
					try {
						Userods.Update(UserList[i]);
						changed=true;
					}
					catch(Exception ex) {
						if(!dictFailedUserUpdates.ContainsKey(ex.Message)){
							dictFailedUserUpdates.Add(ex.Message,new List<Userod>());
						}
						dictFailedUserUpdates[ex.Message].Add(UserList[i]);
					}
				}
			}
			if(dictFailedUserUpdates.Count>0) {//Inform user that user inboxes could not be updated.
				StringBuilder sb=new StringBuilder();
				foreach(string exceptionMsgKey in dictFailedUserUpdates.Keys) {
					foreach(Userod user in dictFailedUserUpdates[exceptionMsgKey]) {
						sb.AppendLine("  "+user.UserName+" - "+exceptionMsgKey);
					}
				}
				MessageBox.Show(this,Lans.g(this,"The following users could not be updated:\r\n")+sb.ToString());
			}
			if(changed){
				DataValid.SetInvalid(InvalidType.Security);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}