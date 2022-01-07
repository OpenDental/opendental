using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	public partial class FormUserGroupPicker:FormODBase {
		public UserGroup UserGroup;
		/// <summary>Set to true by default. If false usergroups with security admin permission will not show.</summary>
		public bool IsAdminMode=true;

		///<summary></summary>
		public FormUserGroupPicker()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUserGroupPicker_Load(object sender,EventArgs e) {
			FillList();
		}

		private void FillList(){
			UserGroups.RefreshCache();
			listGroups.Items.Clear();
			List<UserGroup> listUserGroups=UserGroups.GetWhere(x => IsAdminMode || !GroupPermissions.HasPermission(x.UserGroupNum,Permissions.SecurityAdmin,0));
			listGroups.Items.AddList(listUserGroups,x => x.Description);
		}

		private void listGroups_DoubleClick(object sender,EventArgs e) {
			if(listGroups.SelectedIndex==-1) {
				return;
			}
			UserGroup=listGroups.GetSelected<UserGroup>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listGroups.SelectedIndex==-1) {
				MsgBox.Show(this,"Select a group.");
				return;
			}
			UserGroup=listGroups.GetSelected<UserGroup>();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















