using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildEdit:FrmODBase {
		///<summary></summary>
		public Child ChildCur;
		///<summary>Store the room selected until the child is saved.</summary>
		private long _childRoomNumSelected;

		///<summary></summary>
		public FrmChildEdit() {
			InitializeComponent();
			Load+=FrmChildEdit_Load;
			listAuthorized.MouseDoubleClick+=listAuthorized_MouseDoubleClick;
		}

		private void FrmChildEdit_Load(object sender,EventArgs e) {
			textFirstName.Text=ChildCur.FName;
			textLastName.Text=ChildCur.LName;
			textBirthdate.Value=ChildCur.BirthDate;
			FillListBox();
			_childRoomNumSelected=ChildCur.ChildRoomNumPrimary;
			textPrimaryRoom.Text=ChildRooms.GetRoomId(ChildCur.ChildRoomNumPrimary);
			textBadgeId.Text=ChildCur.BadgeId;
			checkHidden.Checked=ChildCur.IsHidden;
			textNotes.Text=ChildCur.Notes;
		}

		private void FillListBox() {
			List<ChildParentLink> listChildParentLinks=ChildParentLinks.GetChildParentLinksByChildNum(ChildCur.ChildNum);
			listAuthorized.Items.Clear();
			for(int i=0;i<listChildParentLinks.Count;i++) {
				ChildParent childParent=ChildParents.GetOne(listChildParentLinks[i].ChildParentNum);
				listAuthorized.Items.Add(childParent.FName+" "+childParent.LName,listChildParentLinks[i]);
			}
		}

		private void listAuthorized_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			if(listAuthorized.SelectedIndex==-1) {
				return;
			}
			FrmChildParentLinkEdit frmParentEdit=new FrmChildParentLinkEdit();
			frmParentEdit.ChildParentLinkCur=(ChildParentLink)listAuthorized.SelectedItem;
			frmParentEdit.ShowDialog();
			if(!frmParentEdit.IsDialogOK) {
				return;
			}
			FillListBox();//Refill to show changes
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmChildParentLinkEdit frmParentEdit=new FrmChildParentLinkEdit();
			ChildParentLink childParentLink=new ChildParentLink();
			childParentLink.ChildNum=ChildCur.ChildNum;//Assign child
			childParentLink.IsNew=true;
			frmParentEdit.ChildParentLinkCur=childParentLink;
			frmParentEdit.ShowDialog();
			if(!frmParentEdit.IsDialogOK) {
				return;
			}
			FillListBox();//Refill to show changes
		}

		private void butRoomSelect_Click(object sender,EventArgs e) {
			//Select a child room using a combobox
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.ComboSelect;
			inputBoxParam.LabelText="Select a child room.";
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			inputBoxParam.ListSelections=listChildRooms.Select(x => x.RoomId).ToList();
			inputBoxParam.SizeParam=new Size(width:200,height:20);
			InputBox inputBox=new InputBox(inputBoxParam);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return;
			}
			ChildRoom childRoomSelected=listChildRooms[inputBox.SelectedIndex];
			_childRoomNumSelected=childRoomSelected.ChildRoomNum;
			textPrimaryRoom.Text=childRoomSelected.RoomId;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textFirstName.Text=="") {
				MsgBox.Show("This child must have a first name.");
				return;
			}
			ChildCur.FName=textFirstName.Text;
			ChildCur.LName=textLastName.Text;
			ChildCur.BirthDate=textBirthdate.Value;
			ChildCur.Notes=textNotes.Text;
			ChildCur.ChildRoomNumPrimary=_childRoomNumSelected;
			ChildCur.BadgeId=textBadgeId.Text;
			ChildCur.IsHidden=checkHidden.Checked==true;
			//Only need to update as new children will already be inserted by the parent FrmChildren
			Children.Update(ChildCur);
			Signalods.SetInvalid(InvalidType.Children);
			IsDialogOK=true;
		}


	}
}