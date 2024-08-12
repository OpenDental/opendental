using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FrmChildCheckIn:FrmODBase {
		///<summary>Keeps track of keyboard inputs. Will be cleared if any input entered is not a number as it is looking for an employee badge Id. Will also be cleared if the key inputs come in too slow to prevent users from typing out an Id manually. The correct way to fill this string is by using a badge scanner.</summary>
		private string _keyboardInput="";
		///<summary>Tracks the amount of time since start of keydown input. On the timer tick attempt to log in.</summary>
		private DispatcherTimer _dispatcherTimer;

		///<summary></summary>
		public FrmChildCheckIn() {
			InitializeComponent();
			Load+=FrmChildCheckIn_Load;
			KeyDown+=FrmODBase_KeyDown;
			_dispatcherTimer=new DispatcherTimer();
			//Faster than someone could type 8 digits.
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(500);
			_dispatcherTimer.Tick+=_dispatcherTimer_Tick;
		}

		private void FrmChildCheckIn_Load(object sender, EventArgs e) {
			StartMaximized=true;
			FillListBoxes();
			GlobalFormOpenDental.EventProcessSignalODs+=GlobalFormOpenDental_EventProcessSignalODs;
		}

		private void GlobalFormOpenDental_EventProcessSignalODs(object sender,List<Signalod> listSignalods) {
			for(int i=0;i<listSignalods.Count;i++) {
				if(listSignalods[i].IType!=InvalidType.Children) {
					continue;
				}
				FillListBoxes();
				return;
			}
		}

		private void FillListBoxes() {
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Now.Date);
			List<long> listChildNumsPresent=new List<long>();
			List<long> listChildNumsUnique=listChildRoomLogs.Select(x => x.ChildNum).Distinct().ToList();
			//Determine which children are present
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				ChildRoomLog childRoomLog=listChildRoomLogs.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.ChildRoomLogNum).First();
				if(!childRoomLog.IsComing) {
					continue;//Child is absent
				}
				listChildNumsPresent.Add(listChildNumsUnique[i]);
			}
			List<Child> listChildren=Children.GetAll().FindAll(x => !x.IsHidden);//Remove hidden children
			//Fill absent and present lists. Order by last name so children from the same parents are together in the list where possible
			List<Child> listChildrenPresent=listChildren.FindAll(x => listChildNumsPresent.Any(y => y==x.ChildNum)).OrderBy(z => z.LName).ToList();
			listChildren.RemoveAll(x => listChildrenPresent.Any(y => y.ChildNum==x.ChildNum));//Remove present children
			List<Child> listChildrenAbsent=listChildren.OrderBy(z => z.LName).ToList();
			//Fill listboxes
			listBoxAbsent.Items.Clear();
			for(int i=0;i<listChildrenAbsent.Count;i++) {
				Child child=listChildrenAbsent[i];
				listBoxAbsent.Items.Add(child.FName+" "+child.LName,child);
			}
			listBoxPresent.Items.Clear();
			for(int i=0;i<listChildrenPresent.Count;i++) {
				Child child=listChildrenPresent[i];
				listBoxPresent.Items.Add(child.FName+" "+child.LName,child);
			}
		}

		///<summary>Looks for input from badge reader. The timer interval should be faster a user could type.</summary>
		private void _dispatcherTimer_Tick(object sender,EventArgs e) {
			//Fires 1/2 second after beginning of swipe, by which time, all 8 characters should be present.
			_dispatcherTimer.Stop();
			if(_keyboardInput.Length<8) {//IDs will always come in as 8 digits from the reader no matter the badge ID length
				_keyboardInput="";
				return;
			}
			//Find either the child or parent based on the badge number
			ChildParent childParent=ChildParents.GetByBadgeId(_keyboardInput);
			Child child=Children.GetUserByBadgeId(_keyboardInput);
			if(childParent==null && child==null) {//Kickout if no parent or child was found
				_keyboardInput="";
				return;
			}
			if(child!=null) {//This is a child badge
				List<Child> listChildrenOneChild=new List<Child>();
				listChildrenOneChild.Add(child);
				FrmChildCheckInBadge frmChildCheckInBadgeChild=new FrmChildCheckInBadge();
				frmChildCheckInBadgeChild.ListChildren=listChildrenOneChild;
				frmChildCheckInBadgeChild.ShowDialog();
				_keyboardInput="";
				if(frmChildCheckInBadgeChild.IsDialogOK) {
					FillListBoxes();
				}
				return;
			}
			//This is a parent badge
			//Find the children linked to this parent
			List<ChildParentLink> listChildParentLinks=ChildParentLinks.GetAllByChildParentNum(childParent.ChildParentNum);
			List<long> listChildNumsForParent=listChildParentLinks.Select(x => x.ChildNum).ToList();
			List<Child> listChildren=Children.GetAll().FindAll(x => !x.IsHidden);//Do not use hidden children
			List<Child> listChildrenForParentChild=listChildren.FindAll(x => listChildNumsForParent.Contains(x.ChildNum));
			if(listChildrenForParentChild.Count==0) {
				MsgBox.Show("The parent with this badge has no children assigned to them.");
				_keyboardInput="";
				return;
			}
			FrmChildCheckInBadge frmChildCheckInBadge=new FrmChildCheckInBadge();
			frmChildCheckInBadge.ListChildren=listChildrenForParentChild;
			frmChildCheckInBadge.ShowDialog();
			if(frmChildCheckInBadge.IsDialogOK) {
				FillListBoxes();
			}
			_keyboardInput="";
		}

		///<summary>Starts dispatch timer and determines what keys are being pressed. Focus must be on this frm to work.</summary>
		private void FrmODBase_KeyDown(object sender,KeyEventArgs e) {
			//When scanning a card, we will see a series of KeyDown and KeyUp events,
			//just like if someone was typing in.
			if(_keyboardInput=="") {
				//typically this happens when user first swipes card
				_dispatcherTimer.Start();
			}
			//numbers come in from the card as Key.D0, etc.
			bool isNumber=(e.Key>=Key.D0 && e.Key<=Key.D9);
			if(!isNumber) { //If obvious keyboard input clear the string
				_keyboardInput="";
				return;
			}
			_keyboardInput+=e.Key.ToString().Substring(1);//Get the key number pressed
		}

		private void butCheckIn_Click(object sender,EventArgs e) {
			if(listBoxAbsent.SelectedIndices.Count==0) {
				MsgBox.Show("At least one child must be selected.");
				return;
			}
			List<Child> listChildrenSelected=listBoxAbsent.GetListSelected<Child>();
			for(int i=0;i<listChildrenSelected.Count;i++) {
				//In case someone using the map checks in the child right before the parent attempts to
				List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(listChildrenSelected[i].ChildNum,DateTime.Now);
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
				//Make coming to log
				ChildRoomLog childRoomLog=new ChildRoomLog();
				childRoomLog.DateTDisplayed=DateTime.Now;
				childRoomLog.DateTEntered=DateTime.Now;
				childRoomLog.IsComing=true;
				childRoomLog.ChildNum=listChildrenSelected[i].ChildNum;
				childRoomLog.ChildRoomNum=listChildrenSelected[i].ChildRoomNumPrimary;
				ChildRoomLogs.Insert(childRoomLog);
			}
			Signalods.SetInvalid(InvalidType.Children);
			//Refresh to show changes
			FillListBoxes();
		}

		private void butCheckOut_Click(object sender,EventArgs e) {
			if(listBoxPresent.SelectedIndices.Count==0) {
				MsgBox.Show("At least one child must be selected.");
				return;
			}
			List<Child> listChildrenSelected=listBoxPresent.GetListSelected<Child>();
			for(int i=0;i<listChildrenSelected.Count;i++) {
				List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(listChildrenSelected[i].ChildNum,DateTime.Now.Date);
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
			}
			//Refresh to show changes
			FillListBoxes();
		}
	}
}