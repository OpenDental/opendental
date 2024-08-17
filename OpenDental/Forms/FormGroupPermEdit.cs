using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormGroupPermEdit : FormODBase {
		private GroupPermission _groupPermission;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormGroupPermEdit(GroupPermission groupPermission){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_groupPermission=groupPermission.Copy();
		}

		private void FormGroupPermEdit_Load(object sender, System.EventArgs e) {
			textName.Text=GroupPermissions.GetDesc(_groupPermission.PermType);
			if(_groupPermission.NewerDate.Year<1880){
				textDate.Text="";
			}
			else{
				textDate.Text=_groupPermission.NewerDate.ToShortDateString();
			}
			if(_groupPermission.NewerDays==0){
				textDays.Text="";
			}
			else{
				textDays.Text=_groupPermission.NewerDays.ToString();
			}
		}

		/*private void textDays_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(textDays.Text==""){
				textDays.Text="0";
				return;
			}
			try{
				if(Convert.ToInt32(textDays.Text)<0){
					MessageBox.Show(Lan.g(this,"Value cannot be less than 0"));
					e.Cancel=true;
					return;
				}
			}
			catch{
				MessageBox.Show(Lan.g(this,"Cannot contain letters or symbols"));
				e.Cancel=true;
				return;
			}
		}*/

		private void textDate_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			textDays.Text="";
		}

		private void textDays_KeyDown(object sender,KeyEventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid() || !textDays.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			int newerDays=PIn.Int(textDays.Text);
			if(newerDays>GroupPermissions.NewerDaysMax) {
				MsgBox.Show(this,$"Days must be less than {GroupPermissions.NewerDaysMax.ToString()}.");
				return;
			}
			_groupPermission.NewerDays=newerDays;
			_groupPermission.NewerDate=PIn.Date(textDate.Text);
			try{
				if(_groupPermission.IsNew) {
					GroupPermissions.Insert(_groupPermission);
				}
				else {
					GroupPermissions.Update(_groupPermission);
				}
				SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,$"Permission '{_groupPermission.PermType}' granted to " +
					$"'{UserGroups.GetGroup(_groupPermission.UserGroupNum).Description}'");
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








