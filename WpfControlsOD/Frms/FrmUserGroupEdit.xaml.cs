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
	public partial class FrmUserGroupEdit:FrmODBase {

		private UserGroup _userGroup;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FrmUserGroupEdit(UserGroup curGroup)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			_userGroup=curGroup;
			Load+=FrmUserGroupEdit_Load;
		}

		private void FrmUserGroupEdit_Load(object sender, EventArgs e) {
			textDescription.Text=_userGroup.Description;
			textDescription.SelectAll();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				IsDialogOK=false;
				return;
			}
			if(PrefC.GetLong(PrefName.DefaultUserGroup)==_userGroup.UserGroupNum) {
				MsgBox.Show(this,"Cannot delete user group that is set as the default user group.");
				return;
			}
			try{
				UserGroups.Delete(_userGroup);
				DataValid.SetInvalid(InvalidType.Security);
				IsDialogOK=true;
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			_userGroup.Description=textDescription.Text;
			try{
				if(IsNew){
					UserGroups.Insert(_userGroup);
				}
				else{
					UserGroups.Update(_userGroup);
				}
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.Security);
			IsDialogOK=true;
		}
	}
}





















