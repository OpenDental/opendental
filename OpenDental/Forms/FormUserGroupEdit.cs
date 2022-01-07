using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormUserGroupEdit : FormODBase {
		private UserGroup CurGroup;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormUserGroupEdit(UserGroup curGroup)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			CurGroup=curGroup;
		}

		private void FormUserGroupEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=CurGroup.Description;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(PrefC.GetLong(PrefName.DefaultUserGroup)==CurGroup.UserGroupNum) {
				MsgBox.Show(this,"Cannot delete user group that is set as the default user group.");
				return;
			}
			try{
				UserGroups.Delete(CurGroup);
				DataValid.SetInvalid(InvalidType.Security);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			CurGroup.Description=textDescription.Text;
			try{
				if(IsNew){
					UserGroups.Insert(CurGroup);
				}
				else{
					UserGroups.Update(CurGroup);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.Security);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















