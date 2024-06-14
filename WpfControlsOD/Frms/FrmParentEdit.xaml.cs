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
		///<summary>Set true if this is a new Child-Parent relationship.</summary>
		public bool IsNew=false;
		///<summary>Assigned by calling frm. The Parent can be unassigned intially if IsNew and can change. ChildNum does not change.</summary>
		public ChildParent ChildParentCur;

		public FrmParentEdit(){
			InitializeComponent();
			Load+=FrmGuardianEdit_Load;
		}

		private void FrmGuardianEdit_Load(object sender,EventArgs e) {
			Child child=Children.GetOne(ChildParentCur.ChildNum);
			textChild.Text=child.FName+" "+child.LName;
			if(IsNew) {//No parent selected yet
				return;
			}
			textParent.Text=Userods.GetName(ChildParentCur.Parent);
		}
		private void butPick_Click(object sender,EventArgs e) {
			FrmParentSelect frmParentSelect=new FrmParentSelect();
			frmParentSelect.ShowDialog();
			if(!frmParentSelect.IsDialogOK) {
				return;//No user was selected
			}
			Userod userodSelected=Userods.GetUser(frmParentSelect.UserNumSelected);
			textParent.Text=userodSelected.UserName;
			ChildParentCur.Parent=userodSelected.UserNum;//Assign parent selected in FrmParentSelect
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {//Kickout if new relationship
				IsDialogCancel=true;
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
			if(IsNew) {
				ChildParents.Insert(ChildParentCur);
			}
			else {
				ChildParents.Update(ChildParentCur);
			}
			IsDialogOK=true;
		}
	}
}