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
	public partial class FrmUserGroupPicker:FrmODBase {

		public UserGroup UserGroupSelected;
		/// <summary>Set to true by default. If false usergroups with security admin permission will not show.</summary>
		public bool IsAdminMode=true;

		///<summary></summary>
		public FrmUserGroupPicker()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmUserGroupPicker_Load;
			listGroups.MouseDoubleClick+=listGroups_DoubleClick;
		}

		private void FrmUserGroupPicker_Load(object sender,EventArgs e) {
			FillList();
		}

		private void FillList(){
			UserGroups.RefreshCache();
			listGroups.Items.Clear();
			List<UserGroup> listUserGroups=UserGroups.GetWhere(x => IsAdminMode || !GroupPermissions.HasPermission(x.UserGroupNum,EnumPermType.SecurityAdmin,0));
			listGroups.Items.AddList(listUserGroups,x => x.Description);
		}

		private void listGroups_DoubleClick(object sender,MouseButtonEventArgs e) {
			if(listGroups.SelectedIndex==-1) {
				return;
			}
			UserGroupSelected=listGroups.GetSelected<UserGroup>();
			IsDialogOK=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listGroups.SelectedIndex==-1) {
				MsgBox.Show(this,"Select a group.");
				return;
			}
			UserGroupSelected=listGroups.GetSelected<UserGroup>();
			IsDialogOK=true;
		}

	}
}





















