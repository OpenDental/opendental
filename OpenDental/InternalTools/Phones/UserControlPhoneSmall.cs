using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class UserControlPhoneSmall:UserControl {
		///<summary>Gets passed in from outside.</summary>
		private List<Phone> _listPhones;
		///<summary>When the GoToChanged event fires, this tells us which patnum.</summary>
		public long GotoPatNum;
		///<summary></summary>
		[Category("Property Changed"),Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToChanged=null;
		public int Extension;
		///<summary>A list of all map areas.  Each map area represents a "tile" which is associated to a phone extension at HQ.</summary>
		private List<MapArea> _listMapAreas;
		private long _mapAreaContainerNum=0;
		private PhoneConf _phoneConf;
		private DateTime _dateTimeConfRoom;
		private DateTime _dateTimeConfRoomEnd;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public PhoneTile PhoneTile {
			get {
				return phoneTile;
			}
		}
		
		public UserControlPhoneSmall() {
			InitializeComponent();
			phoneTile.GoToChanged += new System.EventHandler(this.phoneTile_GoToChanged);
			phoneTile.NeedsHelpClicked+= new System.EventHandler(this.toggleHelp_Click);
			phoneTile.MenuNumbers=menuNumbers;
			phoneTile.MenuStatus=menuStatus;
			phoneTile.CanWebChatResetTimer=true;
		}

		///<summary></summary>
		public void SetPhoneList(List<Phone> listPhones) {
			_listPhones=listPhones;
		}

		///<summary>Set the phone which is linked to the extension at this desk. If phone==null then no phone info shown.</summary>
		public void SetPhone(Phone phone,PhoneEmpDefault phoneEmpDefault,ChatUser chat,bool isTriageOperator,WebChatSession webChatSession,
			PeerInfo remoteSupportSession)
		{
			phoneTile.SetPhone(phone,phoneEmpDefault,chat,isTriageOperator,webChatSession,remoteSupportSession);
		}

		///<summary>Sets the Enabled property on all controls that need to be toggled based on the phone tracking server heartbeat.</summary>
		public void SetEnabledStateForControls(bool enabled) {
			butGetConfRoom.Enabled=enabled;
		}

		private void FillTile() {
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			Phone phone=Phones.GetPhoneForExtension(_listPhones,Extension);
			PhoneEmpDefault phoneEmpDefault=null;
			ChatUser chatUser=null;
			WebChatSession webChatSession=null;
			PeerInfo remoteSupportSession=null;
			if(phone!=null) {
				phoneEmpDefault=PhoneEmpDefaults.GetEmpDefaultFromList(phone.EmployeeNum,listPhoneEmpDefaults);
				chatUser=ChatUsers.GetFromExt(phone.Extension);
				webChatSession=WebChatSessions.GetActiveSessionsForEmployee(phone.EmployeeName);
				remoteSupportSession=PeerInfos.GetActiveSessionForEmployee(phone.EmployeeNum);
			}
			phoneTile.SetPhone(phone,phoneEmpDefault,chatUser,PhoneEmpDefaults.IsTriageOperatorForExtension(Extension,listPhoneEmpDefaults),webChatSession,
				remoteSupportSession);
		}

		private void UserControlPhoneSmall_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			g.FillRectangle(SystemBrushes.Control,this.Bounds);
		}

		private void phoneTile_GoToChanged(object sender,EventArgs e) {
			if(phoneTile.PhoneCur==null) {
				return;
			}
			if(phoneTile.PhoneCur.PatNum==0) {
				return;
			}
			GotoPatNum=phoneTile.PhoneCur.PatNum;
			OnGoToChanged();
		}

		protected void OnGoToChanged() {
			if(GoToChanged!=null) {
				GoToChanged(this,new EventArgs());
			}
		}

		private void menuItemManage_Click(object sender,EventArgs e) {
			PhoneUI.Manage(phoneTile);
		}

		private void menuItemAdd_Click(object sender,EventArgs e) {
			PhoneUI.Add(phoneTile);
		}

		//Timecards-------------------------------------------------------------------------------------

		private void menuItemAvailable_Click(object sender,EventArgs e) {
			SetAvailable();
		}

		private void menuItemAvailableHome_Click(object sender, EventArgs e){
			SetAvailable(true);
		}

		private void menuItemTraining_Click(object sender,EventArgs e) {
			PhoneUI.Training(phoneTile);
			FillTile();
		}

		private void menuItemTeamAssist_Click(object sender,EventArgs e) {
			SetTeamAssist();
		}

		private void menuItemTCResponder_Click(object sender,EventArgs e) {
			SetTCResponder();
		}

		private void menuItemNeedsHelp_Click(object sender,EventArgs e) {
			PhoneUI.NeedsHelp(phoneTile);
			FillTile();
		}

		private void toggleHelp_Click(object sender,EventArgs e) {
			if(phoneTile.PhoneCur.ClockStatus == ClockStatusEnum.NeedsHelp
				|| phoneTile.PhoneCur.ClockStatus == ClockStatusEnum.HelpOnTheWay) {
				PhoneUI.Available(phoneTile);
			}
			else {
				PhoneUI.NeedsHelp(phoneTile);
			}
			FillTile();
		}

		private void menuItemWrapUp_Click(object sender,EventArgs e) {
			PhoneUI.WrapUp(phoneTile);
			FillTile();
		}

		private void menuItemOfflineAssist_Click(object sender,EventArgs e) {
			PhoneUI.OfflineAssist(phoneTile);
			FillTile();
		}

		private void menuItemUnavailable_Click(object sender,EventArgs e) {
			PhoneUI.Unavailable(phoneTile);
			FillTile();
		}

		private void menuItemBackup_Click(object sender,EventArgs e) {
			PhoneUI.Backup(phoneTile);
			FillTile();
		}

		public void SetAvailable(bool isAtHome=false) {
			if(Security.CurUser.EmployeeNum==phoneTile.PhoneCur.EmployeeNum){
				long employeeNum=Security.CurUser.EmployeeNum;
				Employee employee=Employees.GetEmp(employeeNum);
				if(employee.IsWorkingHome!=isAtHome){
					Employee employeeOld=employee.Copy();
					employee.IsWorkingHome=isAtHome;
					Employees.UpdateChanged(employee,employeeOld,true);
				}
			}
			PhoneUI.Available(phoneTile);
			FillTile();
		}

		public void SetTeamAssist() {
			PhoneUI.TeamAssist(phoneTile);
			FillTile();
		}
		
		public void SetTCResponder() {
			PhoneUI.TCResponder(phoneTile);
			FillTile();
		}

		//RingGroups---------------------------------------------------

		private void menuItemQueueTech_Click(object sender,EventArgs e) {
			PhoneUI.QueueTech(phoneTile);
		}

		private void menuItemQueueNone_Click(object sender,EventArgs e) {
			PhoneUI.QueueNone(phoneTile);
		}

		private void menuItemQueueDefault_Click(object sender,EventArgs e) {
			PhoneUI.QueueDefault(phoneTile);
		}

		private void menuItemQueueBackup_Click(object sender,EventArgs e) {
			PhoneUI.QueueBackup(phoneTile);
		}

		//Timecard---------------------------------------------------

		private void menuItemLunch_Click(object sender,EventArgs e) {
			PhoneUI.Lunch(phoneTile);
			FillTile();
		}

		private void menuItemHome_Click(object sender,EventArgs e) {
			PhoneUI.Home(phoneTile);
			FillTile();
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			PhoneUI.Break(phoneTile);
			FillTile();
		}

		//Conference Room--------------------------------------------
		private void butGetConfRoom_Click(object sender,EventArgs e) {
			PhoneEmpDefault ped=null;
			try {
				//phoneTile.PhoneCur could be null which the following line would fail so we will surround this with a try / catch.
				List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
				ped=listPhoneEmpDefaults.FirstOrDefault(x => x.PhoneExt==phoneTile.PhoneCur.Extension);
				if(ped==null) {
					throw new ApplicationException("Invalid PhoneEmpDefault");
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"This extension is not currently associated to a valid PhoneEmpDefault row.");
				return;
			}
			if(ped.SiteNum < 1) {
				MsgBox.Show(this,"This extension is not currently associated to a site.\r\n"
					+"A site must first be set within the Edit Employee Setting window.");
				return;
			}
			if(ped.IsTriageOperator) {
				SetTeamAssist();
			}
			timerConfRoom.Stop();
			labelConfRoom.Text="Reserving Conf Room...";
			labelConfRoom.Visible=true;
			_phoneConf=PhoneConfs.GetAndReserveConfRoom(ped.SiteNum);
			_dateTimeConfRoom=DateTime.Now;
			_dateTimeConfRoomEnd=_dateTimeConfRoom.AddMinutes(5);
			timerConfRoom.Start();
		}

		private void butConfRooms_Click(object sender,EventArgs e) {
			using FormPhoneConfs FormPC=new FormPhoneConfs();
			FormPC.ShowDialog();//ShowDialog because we do not this window to be floating open for long periods of time.
		}

		///<summary>After five minutes has passed, the conference room label will be hidden until the user reserves another conf room.</summary>
		private void timerConfRoom_Tick(object sender,EventArgs e) {
			_dateTimeConfRoom=_dateTimeConfRoom.AddSeconds(1);
			if(_phoneConf==null) {
				labelConfRoom.Text="No Conf Room Available\r\n"
					+"Please Try Again";
			}
			else {
				//A valid conference room was found and reserved.
				labelConfRoom.Text="Conf Room Reserved:\r\n"
					+_phoneConf.Extension.ToString()+"\r\n"
					+(_dateTimeConfRoomEnd - _dateTimeConfRoom).ToStringmmss();
			}
			labelConfRoom.Visible=true;
			if((_dateTimeConfRoom >= _dateTimeConfRoomEnd)) {
				_dateTimeConfRoom=DateTime.MinValue;
				labelConfRoom.Visible=false;
				timerConfRoom.Stop();
			}
		}

		
	}
}
