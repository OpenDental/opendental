using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmUserPick:FrmODBase {

		///<summary>The filtered list of Users to pick from.</summary>
		public List<Userod> ListUserodsFiltered;
		///<summary>If this form closes with OK, then this value will be filled.</summary>
		public long UserNumSelected;
		///<summary>If provided, this usernum will be preselected if it is also in the list of available usernums.</summary>
		public long UserNumSuggested=0;
		public bool IsSelectionMode;
		public bool IsShowAllAllowed;
		///<summary>Will return 0 for SelectedUserNum if the None 
		public bool IsPickNoneAllowed;
		///<summary>Will return -1 for SelectedUserNum if the All 
		public bool IsPickAllAllowed;
		///<summary>Will return the currently logged in user as the SelectedUserNum
		public bool IsPickMeAllowed;

		public FrmUserPick() {
			InitializeComponent();
			Load+=FrmUserPick_Load;
			listUser.MouseDoubleClick+=listUser_DoubleClick;
			PreviewKeyDown+=FrmUserPick_PreviewKeyDown;
		}

		private void FrmUserPick_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(!IsShowAllAllowed || ListUserodsFiltered==null || ListUserodsFiltered.Count<=0) {
				butShow.Visible=false;
			}
			if(!IsPickAllAllowed) {
				butAll.Visible=false;
			}
			if(!IsPickNoneAllowed) {
				butNone.Visible=false;
			}
			if(!IsPickMeAllowed) {
				butMe.Visible=false;
			}
			if(butNone.Visible==false && butAll.Visible==false) {
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
			listUser.SelectedIndex=listUserods.FindIndex(x => x.UserNum==UserNumSuggested);
		}

		private void listUser_DoubleClick(object sender,MouseButtonEventArgs e) {
			if(listUser.SelectedIndex==-1) {
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.TaskEdit,true) && Userods.GetInbox(listUser.GetSelected<Userod>().UserNum)!=0 && !IsSelectionMode) {
				MsgBox.Show(this,"Please select a user that does not have an inbox.");
				return;
			}
			UserNumSelected=listUser.GetSelected<Userod>().UserNum;
			IsDialogOK=true;
		}

		private void FrmUserPick_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listUser.SelectedIndex==-1) {
				MsgBox.Show(this,"Please pick a user first.");
				return;
			}
			if(!IsSelectionMode && !Security.IsAuthorized(EnumPermType.TaskEdit,true) && Userods.GetInbox(listUser.GetSelected<Userod>().UserNum)!=0) {
				MsgBox.Show(this,"Please select a user that does not have an inbox.");
				return;
			}
			UserNumSelected=listUser.GetSelected<Userod>().UserNum;
			IsDialogOK=true;
		}

		private void butAll_Click(object sender,EventArgs e) {
			UserNumSelected=-1;
			IsDialogOK=true;
		}

		private void butNone_Click(object sender,EventArgs e) {
			UserNumSelected=0;
			IsDialogOK=true;
		}

		private void butMe_Click(object sender,EventArgs e) {
			UserNumSelected=Security.CurUser.UserNum;
			IsDialogOK=true;
		}

		private void butShow_Click(object sender,EventArgs e) {
			UserNumSelected=0;
			if(Text=="Show All") {
				Text="Show Filtered";
				FillList(null);
				return;
			}
			Text="Show All";
			FillList(ListUserodsFiltered);
		}

	}
}