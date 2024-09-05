using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildCheckInBadge:FrmODBase {
		///<summary>Stores the list of children that the parent is able to check in/out. This will be will by the parent frm</summary>
		public List<Child> ListChildren=new List<Child>();

		///<summary></summary>
		public FrmChildCheckInBadge() {
			InitializeComponent();
			Load+=FrmChildCheckInBadge_Load;
			checkBoxAll.Click+=CheckBoxAll_Click;
			listBoxChildren.SelectionChangeCommitted+=ListBoxChildren_SelectionChangeCommitted;
		}

		private void FrmChildCheckInBadge_Load(object sender,EventArgs e) {
			FillListBox();
			checkBoxAll.Checked=true;
			listBoxChildren.SetAll(true);
		}

		private void FillListBox() {
			listBoxChildren.Items.Clear();
			for(int i=0;i<ListChildren.Count;i++) {
				Child child=ListChildren[i];
				listBoxChildren.Items.Add(child.FName+" "+child.LName,child);
			}
		}

		private void ListBoxChildren_SelectionChangeCommitted(object sender,EventArgs e) {
			if(listBoxChildren.SelectedIndices.Count==listBoxChildren.Items.Count) {
				checkBoxAll.Checked=true;//If all are selected, check the all checkbox
			}
			else {
				checkBoxAll.Checked=false;
			}
		}

		private void CheckBoxAll_Click(object sender,EventArgs e) {
			if(checkBoxAll.Checked==true) {
				listBoxChildren.SetAll(true);
			}
			else {
				listBoxChildren.ClearSelected();
			}
		}

		private void buttonCheckIn_Click(object sender,EventArgs e) {
			if(listBoxChildren.SelectedIndices.Count==0) {
				MsgBox.Show("At least one child must be selected.");
				return;
			}
			List<Child> listChildren=listBoxChildren.GetListSelected<Child>();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Now.Date);
			List<string> listChildNamesNoPrimary=new List<string>();
			for(int i=0;i<listChildren.Count;i++) {
				if(listChildren[i].ChildRoomNumPrimary==0) {
					listChildNamesNoPrimary.Add(listChildren[i].FName);
					continue;//Children with no primary room will remain in the checked out listbox
				}
				//Create leaving entry if needed
				List<ChildRoomLog> listChildRoomLogsOneChild=listChildRoomLogs.FindAll(x => x.ChildNum==listChildren[i].ChildNum);
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogsOneChild);
				//Create coming to entry
				ChildRoomLog childRoomLog=new ChildRoomLog();
				childRoomLog.DateTDisplayed=DateTime.Now;
				childRoomLog.DateTEntered=DateTime.Now;
				childRoomLog.IsComing=true;
				childRoomLog.ChildNum=listChildren[i].ChildNum;
				childRoomLog.ChildRoomNum=listChildren[i].ChildRoomNumPrimary;
				ChildRoomLogs.Insert(childRoomLog);
			}
			string msg="Checked in: ";
			msg+=string.Join(", ",listChildren.FindAll(x => x.ChildRoomNumPrimary!=0).Select(y => y.FName));
			MsgBox.Show(msg);
			Signalods.SetInvalid(InvalidType.Children);
			if(listChildNamesNoPrimary.Count!=0) {
				MsgBox.Show("The following children have no primary room set: "
					+string.Join(", ",listChildNamesNoPrimary)
					+". Ask the front desk employee for help.");
			}
			IsDialogOK=true;
		}

		private void buttonCheckOut_Click(object sender,EventArgs e) {
			if(listBoxChildren.SelectedIndices.Count==0) {
				MsgBox.Show("At least one child must be selected.");
				return;
			}
			List<Child> listChildren=listBoxChildren.GetListSelected<Child>();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Now.Date);
			for(int i=0;i<listChildren.Count;i++) {
				List<ChildRoomLog> listChildRoomLogsOneChild=listChildRoomLogs.FindAll(x => x.ChildNum==listChildren[i].ChildNum);
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogsOneChild);
			}
			string msg="Checked out: ";
			msg+=string.Join(", ",listChildren.Select(x => x.FName));
			MsgBox.Show(msg);
			IsDialogOK=true;
		}
	}
}