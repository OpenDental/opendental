using System;
using System.Collections.Generic;
using System.ComponentModel;
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

	public partial class FrmParentEdit:FrmODBase {
		///<summary>Assigned by calling frm. The Parent can be unassigned intially if IsNew and can change. ChildNum does not change.</summary>
		public ChildParent ChildParentCur;

		public FrmParentEdit(){
			InitializeComponent();
			Load+=FrmGuardianEdit_Load;
		}

		private void FrmGuardianEdit_Load(object sender,EventArgs e) {
			Child child=Children.GetOne(ChildParentCur.ChildNum);
			textChild.Text=child.FName+" "+child.LName;
			textParent.Text=Userods.GetName(ChildParentCur.Parent);
		}
		private void butPick_Click(object sender,EventArgs e) {
			FrmParentSelect frmParentSelect=new FrmParentSelect();
			frmParentSelect.ShowDialog();
			if(!frmParentSelect.IsDialogOK) {
				return;//No user was selected
			}
			Userod userodSelected=Userods.GetUser(frmParentSelect.UserNumSelected);
			//Stop users from creating duplicate ChildParent relationships
			List<ChildParent> listChildParents=ChildParents.GetChildParentsByChildNum(ChildParentCur.ChildNum);
			if(listChildParents.Any(x => x.Parent==userodSelected.UserNum)) {
				MsgBox.Show("A relationship between this child and parent already exists. Duplicates are not allowed.");
				return;
			}
			textParent.Text=userodSelected.UserName;
			ChildParentCur.Parent=userodSelected.UserNum;//Assign parent selected in FrmParentSelect
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ChildParentCur.IsNew) {//Kick out if new relationship
				IsDialogCancel=true;
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Delete?")){
				return;
			}
			ChildParents.Delete(ChildParentCur.ChildParentNum);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(ChildParentCur.Parent==0) {
				MsgBox.Show("An authorized user must be selected.");
				return;
			}
			if(ChildParentCur.IsNew) {
				ChildParents.Insert(ChildParentCur);
			}
			else {
				ChildParents.Update(ChildParentCur);
			}
			IsDialogOK=true;
		}
	}
}