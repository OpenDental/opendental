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
	public partial class FormCentralGroupPermEdit:Form {
		private GroupPermission _permCur;

		public FormCentralGroupPermEdit(GroupPermission perm) {
			InitializeComponent();
			_permCur=perm.Copy();
		}

		private void FormCentralGroupPermEdit_Load(object sender,EventArgs e) {
			textName.Text=GroupPermissions.GetDesc(_permCur.PermType);
			if(_permCur.NewerDate.Year<1880){
				textDate.Text="";
			}
			else{
				textDate.Text=_permCur.NewerDate.ToShortDateString();
			}
			if(_permCur.NewerDays==0){
				textDays.Text="";
			}
			else{
				textDays.Text=_permCur.NewerDays.ToString();
			}
		}

		private void textDate_KeyDown(object sender,KeyEventArgs e) {
			textDays.Text="";
		}

		private void textDays_KeyDown(object sender,KeyEventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()) {
				MessageBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			int newerDays=PIn.Int(textDays.Text);
			if(newerDays>GroupPermissions.NewerDaysMax) {
				MessageBox.Show(this,$"Days must be less than {GroupPermissions.NewerDaysMax}.");
				return;
			}
			_permCur.NewerDays=newerDays;
			_permCur.NewerDate=PIn.Date(textDate.Text);
			try{
				if(_permCur.IsNew) {
					GroupPermissions.Insert(_permCur);
				}
				else {
					GroupPermissions.Update(_permCur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
