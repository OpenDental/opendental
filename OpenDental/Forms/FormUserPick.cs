using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormUserPick:FormODBase {
		///<summary>The filtered list of Users to pick from.</summary>
		public List<Userod> ListUserodsFiltered;
		///<summary>If this form closes with OK, then this value will be filled.</summary>
		public long SelectedUserNum;
		///<summary>If provided, this usernum will be preselected if it is also in the list of available usernums.</summary>
		public long SuggestedUserNum=0;
		public bool IsSelectionmode;
		public bool IsShowAllAllowed;
		///<summary>Will return 0 for SelectedUserNum if the None 
		public bool IsPickNoneAllowed;
		///<summary>Will return -1 for SelectedUserNum if the All 
		public bool IsPickAllAllowed;
		///<summary>Will return the currently logged in user as the SelectedUserNum
		public bool IsPickMeAllowed;

		public FormUserPick() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUserPick_Load(object sender,EventArgs e) {
			if(IsShowAllAllowed && ListUserodsFiltered!=null && ListUserodsFiltered.Count>0) {
				butShow.Visible=true;
			}
			if(IsPickAllAllowed) {
				butAll.Visible=true;
			}
			if(IsPickNoneAllowed) {
				butNone.Visible=true;
			}
			if(IsPickMeAllowed) {
				butMe.Visible=true;
			}
			if(!butNone.Visible && !butAll.Visible) {
				groupSelect.Visible=false;
			}
			FillList(ListUserodsFiltered);
		}

		private void FillList(List<Userod> listUserods) {
			if(listUserods==null) {
				listUserods=Userods.GetDeepCopy(true);
			}
			listUser.Items.Clear();
			listUser.Items.AddList(listUserods,x => x.UserName);
			listUser.SelectedIndex=listUserods.FindIndex(x => x.UserNum==SuggestedUserNum);
		}

		private void listUser_DoubleClick(object sender,EventArgs e) {
			if(listUser.SelectedIndex==-1) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.TaskEdit,true) && Userods.GetInbox(listUser.GetSelected<Userod>().UserNum)!=0 && !IsSelectionmode) {
				MsgBox.Show(this,"Please select a user that does not have an inbox.");
				return;
			}
			SelectedUserNum=listUser.GetSelected<Userod>().UserNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listUser.SelectedIndex==-1) {
				MsgBox.Show(this,"Please pick a user first.");
				return;
			}
			if(!IsSelectionmode && !Security.IsAuthorized(Permissions.TaskEdit,true) && Userods.GetInbox(listUser.GetSelected<Userod>().UserNum)!=0) {
				MsgBox.Show(this,"Please select a user that does not have an inbox.");
				return;
			}
			SelectedUserNum=listUser.GetSelected<Userod>().UserNum;
			DialogResult=DialogResult.OK;
		}

		private void butAll_Click(object sender,EventArgs e) {
			SelectedUserNum=-1;
			DialogResult=DialogResult.OK;
		}

		private void butNone_Click(object sender,EventArgs e) {
			SelectedUserNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butMe_Click(object sender,EventArgs e) {
			SelectedUserNum=Security.CurUser.UserNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butShow_Click(object sender,EventArgs e) {
			SelectedUserNum=0;
			if(Text=="Show All") {
				Text="Show Filtered";
				FillList(null);
			}
			else {
				Text="Show All";
				FillList(ListUserodsFiltered);
			}
		}
	}
}