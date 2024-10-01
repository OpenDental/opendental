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

	public partial class FrmChildParentLinkEdit:FrmODBase {
		///<summary>Assigned by calling frm. The Parent can be unassigned intially if IsNew and can change. ChildNum does not change.</summary>
		public ChildParentLink ChildParentLinkCur;

		public FrmChildParentLinkEdit(){
			InitializeComponent();
			Load+=FrmChildParentLinkEdit_Load;
		}

		private void FrmChildParentLinkEdit_Load(object sender,EventArgs e) {
			Child child=Children.GetOne(ChildParentLinkCur.ChildNum);
			textChild.Text=child.FName+" "+child.LName;
			textParent.Text=ChildParents.GetName(ChildParentLinkCur.ChildParentNum);
			textRelationship.Text=ChildParentLinkCur.Relationship;
		}
		private void butPick_Click(object sender,EventArgs e) {
			FrmChildParents frmChildParents=new FrmChildParents();
			frmChildParents.IsSelectionMode=true;
			frmChildParents.ShowDialog();
			if(!frmChildParents.IsDialogOK) {
				return;//No user was selected
			}
			ChildParent childParentSelected=ChildParents.GetOne(frmChildParents.ChildParentNumSelected);
			//Stop users from creating duplicate ChildParentLink relationships
			List<ChildParentLink> listChildParentLinks=ChildParentLinks.GetChildParentLinksByChildNum(ChildParentLinkCur.ChildNum);
			if(listChildParentLinks.Any(x => x.ChildParentNum==childParentSelected.ChildParentNum)) {
				MsgBox.Show("A relationship between this child and parent already exists. Duplicates are not allowed.");
				return;
			}
			textParent.Text=childParentSelected.FName+" "+childParentSelected.LName;
			ChildParentLinkCur.ChildParentNum=childParentSelected.ChildParentNum;//Assign parent selected in FrmChildParentLinkSelect
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ChildParentLinkCur.IsNew) {//Kick out if new relationship
				IsDialogCancel=true;
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Delete?")){
				return;
			}
			ChildParentLinks.Delete(ChildParentLinkCur.ChildParentLinkNum);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(ChildParentLinkCur.ChildParentNum==0) {
				MsgBox.Show("An authorized user must be selected.");
				return;
			}
			ChildParentLinkCur.Relationship=textRelationship.Text;
			if(ChildParentLinkCur.IsNew) {
				ChildParentLinks.Insert(ChildParentLinkCur);
			}
			else {
				ChildParentLinks.Update(ChildParentLinkCur);
			}
			IsDialogOK=true;
		}
	}
}