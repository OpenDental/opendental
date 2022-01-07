using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CentralManager {
	public partial class FormCentralUserGroupEdit:Form {
		private UserGroup _groupCur;

		public FormCentralUserGroupEdit(UserGroup userGroup) {
			InitializeComponent();
			_groupCur=userGroup;
		}

		private void FormCentralUserGroupEdit_Load(object sender,EventArgs e) {
			textDescription.Text=_groupCur.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_groupCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			try {
				UserGroups.Delete(_groupCur);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text=="") {
				MessageBox.Show(this,"Please enter a description.");
				return;
			}
			_groupCur.Description=textDescription.Text;
			try {
				if(_groupCur.IsNew) {
					long userGroupNum=UserGroups.Insert(_groupCur);
					_groupCur.UserGroupNumCEMT=userGroupNum;
					UserGroups.Update(_groupCur);//Doing this so we don't have to make another version of Insert
				}
				else {
					UserGroups.Update(_groupCur);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			Cache.Refresh(InvalidType.Security);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
