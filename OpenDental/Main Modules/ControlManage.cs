using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentalCloud;
using OpenDentBusiness;

namespace OpenDental{

	///<summary></summary>
	public partial class ControlManage : UserControl{
		#region Fields - Public
		public FormAccounting FormAccounting;
		///<summary></summary>
		//[Category("Data"),Description("Occurs when user changes current patient, usually by clicking on the Select Patient button.")]
		//public event PatientSelectedEventHandler PatientSelected=null;
		#endregion Fields - Public

		#region Fields - Private
		private SigElementDef[] _sigElementDefArrayExtras;
		private SigElementDef[] _sigElementDefArrayMessages;
		private SigElementDef[] _sigElementDefArrayUsers;
		private Employee _employee;
		private ErrorProvider _errorProvider1=new ErrorProvider();
		private FormArManager _formArManager;
		private FormBilling _formBilling;
		private FormClaimsSend _formClaimsSend;
		private FormEmailInbox _formEmailInbox=null;
		private FormEtrans834Import _formEtrans834Import=null;
		private FormGraphEmployeeTime _formGraphEmployeeTime;
		//private bool _initializedOnStartup;
		private List<Employee> _listEmployees=new List<Employee>();
		/// <summary>Always 1:1 with values in listStatus.Items</summary>
		private List<TimeClockStatus> _listTimeClockStatusesShown=new List<TimeClockStatus>();
		///<summary>Collection of SigMessages</summary>
		private List<SigMessage> _listSigMessages;
		private long _patNum;
		///<summary>Server time minus local computer time, usually +/- 1 or 2 minutes</summary>
		private TimeSpan _timeSpanDelta;
		#endregion Fields - Private

		
		#region Constructor
		///<summary></summary>
		public ControlManage(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			this.listBoxStatus.Click += new System.EventHandler(this.listStatus_Click);
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Methods - Event Handlers - Buttons General
		private void butAccounting_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Accounting)) {
				return;
			}
			if(FormAccounting==null || FormAccounting.IsDisposed) {
				FormAccounting=new FormAccounting();
			}
			FormAccounting.Show();
			if(FormAccounting.WindowState==FormWindowState.Minimized) {
				FormAccounting.WindowState=FormWindowState.Normal;
			}
			FormAccounting.BringToFront();
		}

		private void butBackup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Backup)){
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Backup,0,"FormBackup was accessed");
			using FormBackup formBackup=new FormBackup();
			formBackup.ShowDialog();
			if(formBackup.DialogResult==DialogResult.Cancel){
				return;
			}
			//ok signifies that a database was restored
			GlobalFormOpenDental.PatientSelected(new Patient(),false);//unload patient after restore.
			//ParentForm.Text=PrefC.GetString(PrefName.MainWindowTitle");
			DataValid.SetInvalid(true);
			ModuleSelected(_patNum);
		}

		private void butBilling_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Billing)) {
				return;
			}
			bool unsentStatementsExist=Statements.UnsentStatementsExist();
			if(unsentStatementsExist) {
				if(PrefC.HasClinicsEnabled) {//Using clinics.
					if(Statements.UnsentClinicStatementsExist(Clinics.ClinicNum)) {//Check if clinic has unsent bills.
						ShowBilling(new List<long>() { Clinics.ClinicNum });//Clinic has unsent bills.  Simply show billing window.
					}
					else {//No unsent bills for clinic.  Show billing options to generate a billing list.
						ShowBillingOptions(Clinics.ClinicNum);
					}
				}
				else {//Not using clinics and has unsent bills.  Simply show billing window.
					ShowBilling(new List<long>() { 0 });
				}
				SecurityLogs.MakeLogEntry(EnumPermType.Billing,0,"");
				return;
			}
			//No unsent statements exist.  Have user create a billing list.
			if(PrefC.HasClinicsEnabled) {
				ShowBillingOptions(Clinics.ClinicNum);
			}
			else {
				ShowBillingOptions(0);
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Billing,0,"");
		}

		private void butBreaks_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=_employee;
			formTimeCard.IsBreaks=true;
			formTimeCard.ShowDialog();
			ModuleSelected(_patNum);
		}

		private void butClaimPay_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPayCreate,true) && !Security.IsAuthorized(EnumPermType.InsPayEdit,true)) {
				//Custom message for multiple permissions.
				MessageBox.Show(Lan.g(this,"Not authorized")+".\r\n"
					+Lan.g(this,"A user with the SecurityAdmin permission must grant you access for")+":\r\n"
					+Lan.g(this,"Insurance Payment Create or Insurance Payment Edit"));
				return;
			}
			FormClaimPayList formClaimPayList=new FormClaimPayList();
			formClaimPayList.Show();
		}

		//private void butClear_Click(object sender, System.EventArgs e) {
			//textMessage.Clear();
			//textMessage.Select();
		//}

		private void butClockIn_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow) && !Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)) {
				//Check if the employee set their ext to 0 in the phoneempdefault table.
				if(PhoneEmpDefaults.GetByExtAndEmp(0,_employee.EmployeeNum)==null) {
					MessageBox.Show("Not allowed.  Use the small phone panel or the \"Big\" phone window to clock in.\r\nIf you are trying to clock in as a \"floater\", you need to set your extension to 0 first before using this Clock In button.");
					return;
				}
			}
			ProgressWin progressOD=new ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				bool[] isAuthorized=new bool[1] { false };
				if(Plugins.HookMethod(this,"ContrStaff.butClockIn_Click_ClockIn",isAuthorized,_employee)) {
					if(!isAuthorized[0]) {
						throw new Exception(Lans.g(this,"You need to authenticate to clock-in"));
					}
				}
				ClockEvents.ClockIn(_employee.EmployeeNum,isAtHome:false);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(this,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			Employee EmployeeOld=_employee.Copy();
			_employee.ClockStatus=Lan.g(this,"Working");
			Employees.UpdateChanged(_employee, EmployeeOld, true);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				Phones.SetPhoneStatus(ClockStatusEnum.Available,Phones.GetExtensionForEmp(_employee.EmployeeNum),_employee.EmployeeNum);
			}
			ModuleSelected(_patNum);
			if(!PayPeriods.HasPayPeriodForDate(DateTime.Today)) {
				MsgBox.Show(this,"No dates exist for this pay period.  Time clock events will not display until pay periods have been created for this date range");
			}
			if(PrefC.GetBoolSilent(PrefName.ChildDaycare,false)) {
				//So teacher has their status updated when they clock in
				Signalods.SetInvalid(InvalidType.Children);
			}
		}

		private void butClockOut_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow) && !Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)) {
				//Check if the employee set their ext to 0 in the phoneempdefault table.
				if(PhoneEmpDefaults.GetByExtAndEmp(0,_employee.EmployeeNum)==null) {
					MessageBox.Show("Not allowed.  Use the small phone panel or the \"Big\" phone window to clock out.\r\nIf you are trying to clock out as a \"floater\", you need to set your extension to 0 first before using this Clock Out For: button.");
					return;
				}
			}
			if(listBoxStatus.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a status first.");
				return;
			}
			ProgressWin progressOD=new ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				bool[] boolArrayAuth=new bool[1] { false };
				if(Plugins.HookMethod(this,"ContrStaff.butClockOut_Click_ClockOut",boolArrayAuth,_employee,_listTimeClockStatusesShown[listBoxStatus.SelectedIndex])) {
					if(!boolArrayAuth[0]) {
						throw new Exception(Lans.g(this,"You need to authenticate to clock-out"));
					}
				}
				ClockEvents.ClockOut(_employee.EmployeeNum,_listTimeClockStatusesShown[listBoxStatus.SelectedIndex]);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock in again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(this,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Employee employeeOld=_employee.Copy();
			_employee.ClockStatus=Lan.g("enumTimeClockStatus",(_listTimeClockStatusesShown[listBoxStatus.SelectedIndex]).GetDescription());
			Employees.UpdateChanged(_employee, employeeOld, true);
			ModuleSelected(_patNum);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				Phones.SetPhoneStatus(Phones.GetClockStatusFromEmp(_employee.ClockStatus),Phones.GetExtensionForEmp(_employee.EmployeeNum),_employee.EmployeeNum);
			}
			//Automatically create a leaving log when an employee clocks out
			if(PrefC.GetBoolSilent(PrefName.ChildDaycare,false)) {
				List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForEmployee(_employee.EmployeeNum,DateTime.Now);
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
				Signalods.SetInvalid(InvalidType.Children);
			}
		}

		private void butDeposit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DepositSlips,DateTime.Today)) {
				return;
			}
			using FormDeposits formDeposits=new FormDeposits();
			formDeposits.ShowDialog();
		}

		private void butEmailInbox_Click(object sender,EventArgs e) {
			if(_formEmailInbox==null || _formEmailInbox.IsDisposed) {
				_formEmailInbox=null;
				_formEmailInbox=new FormEmailInbox();
				_formEmailInbox.Show();
				return;
			}
			if(_formEmailInbox.WindowState==FormWindowState.Minimized) {
				_formEmailInbox.WindowState=FormWindowState.Maximized;
			}
			_formEmailInbox.BringToFront();
		}

		private void butEras_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPayCreate)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			FormEtrans835s formEtrans835s=new FormEtrans835s();
			formEtrans835s.Show();//non-modal
			Cursor=Cursors.Default;
		}

		private void butImportInsPlans_Click(object sender,EventArgs e) {
			if(_formEtrans834Import!=null && _formEtrans834Import.FormEtrans834PreviewCur!=null && !_formEtrans834Import.FormEtrans834PreviewCur.IsDisposed) {
				_formEtrans834Import.FormEtrans834PreviewCur.Show();
				_formEtrans834Import.FormEtrans834PreviewCur.BringToFront();
				return;
			}
			if(_formEtrans834Import==null || _formEtrans834Import.IsDisposed) {
				_formEtrans834Import=new FormEtrans834Import();
			}
			_formEtrans834Import.Show();
			_formEtrans834Import.BringToFront();
		}

		private void butManage_Click(object sender,EventArgs e) {
			using FormTimeCardManage formTimeCardManage=new FormTimeCardManage(_listEmployees);
			formTimeCardManage.ShowDialog();
			ModuleSelected(_patNum);
		}

		private void butManageAR_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Billing)) {
				return;
			}
			if(!Programs.IsEnabled(ProgramName.Transworld)) {
				try {
					Process.Start("https://opendental.com/resources/redirects/redirecttransworldsystems.html");
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show(this,"Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet and then try again.");
				}
				return;
			}
			if(_formArManager==null || _formArManager.IsDisposed) {
				while(!ValidateConnectionDetails()) {//only validate connection details if the ArManager form does not exist yet
					string msgText="An SFTP connection could not be made using the connection details "+(PrefC.HasClinicsEnabled ? "for any clinic " : "")
						+"in the enabled Transworld (TSI) program link.  Would you like to edit the Transworld program link now?";
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)) {//if user does not want to edit program link, return
						return;
					}
					using FormTransworldSetup formTransworldSetup=new FormTransworldSetup();
					if(formTransworldSetup.ShowDialog()!=DialogResult.OK) {//if user cancels edits in the setup window, return
						return;
					}
				}
				_formArManager=new FormArManager();//connections settings have been validated, create a new ArManager form
				_formArManager.FormClosed+=new FormClosedEventHandler((o,ev) => { _formArManager=null; });//So that the form can release its objects for garbage collection
			}
			_formArManager.Restore();
			_formArManager.Show();//form has a Go To option and is shown as a non-modal window so the user can view the pat account and the collection list at the same time.
			if(_formArManager!=null) {
				//When things go wrong running aging, user is prompted to load the existing account info.  If they say no, the form closes, and the closing 
				//event handler sets _formAR=null.
				_formArManager.BringToFront();
			}
		}

		private void butSchedule_Click(object sender,EventArgs e){
			//only visible at ODHQ
			DateTime date=DateTime.Today;
			if(ODBuild.IsDebug() && Environment.MachineName=="JORDANHOME") {
				//date=new DateTime(2020,3,18);
			}
			if(_formGraphEmployeeTime!=null && !_formGraphEmployeeTime.IsDisposed) {
				_formGraphEmployeeTime.Show();
				_formGraphEmployeeTime.Restore();
				return;
			}
			_formGraphEmployeeTime=new FormGraphEmployeeTime(date);
			_formGraphEmployeeTime.Show();
		}

		private void butDaycare_Click(object sender,EventArgs e) {
			FrmChildCareMap frmChildCareMap=new FrmChildCareMap();
			if(!Security.IsAuthorized(EnumPermType.ChildDaycareEdit,true)) {
				frmChildCareMap.ViewOnly=true;
			}
			frmChildCareMap.Show();
		}

		private void butDaycareCheckIn_Click(object sender,EventArgs e) {
			FrmChildCheckIn frmChildCheckIn=new FrmChildCheckIn();
			if(!Security.IsAuthorized(EnumPermType.ChildDaycareEdit,false)) {
				return;
			}
			frmChildCheckIn.Show();
		}

		private void butSendClaims_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ClaimSend)) {
				return;
			}
			if(_formClaimsSend!=null && !_formClaimsSend.IsDisposed) {//Form is open
				_formClaimsSend.Focus();//Don't open a new form.
				//We may need to close and reopen the form in the future if the window is not being brought to the front.
				//It is complicated to Close() and reopen the form, because the user might be in the middle of a task.
				return;
			}
			Cursor=Cursors.WaitCursor;
			_formClaimsSend=new FormClaimsSend();
			_formClaimsSend.FormClosed+=(s,ea) => { ODEvent.Fired-=formClaimsSend_GoToChanged; };
			ODEvent.Fired+=formClaimsSend_GoToChanged;
			_formClaimsSend.Show();//FormClaimsSend has a GoTo option and is shown as a non-modal window.
			_formClaimsSend.BringToFront();
			Cursor=Cursors.Default;
		}

		private void butSupply_Click(object sender,EventArgs e) {
			using FormSupplyInventory formSupplyInventory=new FormSupplyInventory();
			formSupplyInventory.ShowDialog();
		}

		private void butTasks_Click(object sender,EventArgs e) {
			LaunchTaskWindow(false);
			/*  //This is the old code exactly how it was before making the task window non-modal in case issues arise.
			using FormTasks FormT=new FormTasks();
			FormT.ShowDialog();
			if(FormT.GotoType==TaskObjectType.Patient){
				if(FormT.GotoKeyNum!=0){
					Patient pat=Patients.GetPat(FormT.GotoKeyNum);
					OnPatientSelected(pat);
					GotoModule.GotoAccount(0);
				}
			}
			if(FormT.GotoType==TaskObjectType.Appointment){
				if(FormT.GotoKeyNum!=0){
					Appointment apt=Appointments.GetOneApt(FormT.GotoKeyNum);
					if(apt==null){
						MsgBox.Show(this,"Appointment has been deleted, so it's not available.");
						return;
						//this could be a little better, because window has closed, but they will learn not to push that button.
					}
					DateTime dateSelected=DateTime.MinValue;
					if(apt.AptStatus==ApptStatus.Planned || apt.AptStatus==ApptStatus.UnschedList){
						//I did not add feature to put planned or unsched apt on pinboard.
						MsgBox.Show(this,"Cannot navigate to appointment.  Use the Other Appointments button.");
						//return;
					}
					else{
						dateSelected=apt.AptDateTime;
					}
					Patient pat=Patients.GetPat(apt.PatNum);
					OnPatientSelected(pat);
					GotoModule.GotoAppointment(dateSelected,apt.AptNum);
				}
			}
			*/
		}

		private void butTimeCard_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=_employee;
			formTimeCard.ShowDialog();
			ModuleSelected(_patNum);
		}

		private void butViewSched_Click(object sender,EventArgs e) {
			List<long> listEmployeeNumsPreSelected=gridEmp.SelectedGridRows.Select(x => ((Employee)x.Tag).EmployeeNum).ToList();
			List<long> listProvNumsPreSelected=Userods.GetWhere(x => listEmployeeNumsPreSelected.Contains(x.EmployeeNum) && x.ProvNum!=0)
				.Select(x => x.ProvNum)
				.ToList();
			using FormSchedule formSchedule=new FormSchedule(listEmployeeNumsPreSelected,listProvNumsPreSelected);
			formSchedule.ShowDialog();
		}
		#endregion Methods - Event Handlers - Buttons General

		#region Methods - Event Handlers - Messaging
		private void butAck_Click(object sender,EventArgs e) {
			if(gridMessages.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			SigMessage sigMessage;
			for(int i=gridMessages.SelectedIndices.Length-1;i>=0;i--) {//go backwards so that we can remove rows without problems.
				sigMessage=(SigMessage)gridMessages.ListGridRows[gridMessages.SelectedIndices[i]].Tag;
				if(sigMessage.AckDateTime.Year>1880) {
					continue;//totally ignore if trying to ack a previously acked signal
				}
				SigMessages.AckSigMessage(sigMessage);
				//change the grid temporarily until the next timer event.  This makes it feel more responsive.
				if(checkIncludeAck.Checked) {
					gridMessages.ListGridRows[gridMessages.SelectedIndices[i]].Cells[3].Text=sigMessage.MessageDateTime.ToShortTimeString();
					Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
					continue;
				}
				try {
					gridMessages.ListGridRows.RemoveAt(gridMessages.SelectedIndices[i]);
				}
				catch {
					//do nothing
				}
				Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
			}
			gridMessages.SetAll(false);
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(textMessage.Text=="") {
				MsgBox.Show(this,"Please type in a message first.");
				return;
			}
			SigMessage sigMessage=new SigMessage();
			sigMessage.SigText=textMessage.Text;
			if(listBoxTo.SelectedIndex!=-1) {
				sigMessage.ToUser=_sigElementDefArrayUsers[listBoxTo.SelectedIndex].SigText;
				sigMessage.SigElementDefNumUser=_sigElementDefArrayUsers[listBoxTo.SelectedIndex].SigElementDefNum;
			}
			if(listBoxFrom.SelectedIndex!=-1) {
				sigMessage.FromUser=_sigElementDefArrayUsers[listBoxFrom.SelectedIndex].SigText;
			}
			if(listBoxExtras.SelectedIndex!=-1) {
				sigMessage.SigElementDefNumExtra=_sigElementDefArrayExtras[listBoxExtras.SelectedIndex].SigElementDefNum;
			}
			SigMessages.Insert(sigMessage);
			textMessage.Text="";
			listBoxFrom.SelectedIndex=-1;
			listBoxTo.SelectedIndex=-1;
			listBoxExtras.SelectedIndex=-1;
			listBoxMessages.SelectedIndex=-1;
			ShowSendingLabel();
			Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
		}

		private void checkIncludeAck_Click(object sender,EventArgs e) {
			if(checkIncludeAck.Checked) {
				textDays.Text="1";
				labelDays.Visible=true;
				textDays.Visible=true;
				FillMessages();
				return;
			}
			labelDays.Visible=false;
			textDays.Visible=false;
			_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today);//since midnight this morning.
			FillMessages();
		}

		private void comboViewUser_SelectionChangeCommitted(object sender,EventArgs e) {
			FillMessages();
		}

		private void listMessages_Click(object sender,EventArgs e) {
			if(listBoxMessages.SelectedIndex==-1) {
				return;
			}
			SigMessage sigMessage=new SigMessage();
			sigMessage.SigText=textMessage.Text;
			if(listBoxTo.SelectedIndex!=-1) {
				sigMessage.ToUser=_sigElementDefArrayUsers[listBoxTo.SelectedIndex].SigText;
				sigMessage.SigElementDefNumUser=_sigElementDefArrayUsers[listBoxTo.SelectedIndex].SigElementDefNum;
			}
			if(listBoxFrom.SelectedIndex!=-1) {
				sigMessage.FromUser=_sigElementDefArrayUsers[listBoxFrom.SelectedIndex].SigText;
				//We do not set a SigElementDefNumUser for From.
			}
			if(listBoxExtras.SelectedIndex!=-1) {
				sigMessage.SigElementDefNumExtra=_sigElementDefArrayExtras[listBoxExtras.SelectedIndex].SigElementDefNum;
			}
			sigMessage.SigElementDefNumMsg=_sigElementDefArrayMessages[listBoxMessages.SelectedIndex].SigElementDefNum;
			//need to do this all as a transaction, so need to do a writelock on the signal table first.
			//alternatively, we could just make sure not to retrieve any signals that were less the 300ms old.
			SigMessages.Insert(sigMessage);
			//reset the controls
			textMessage.Text="";
			listBoxFrom.SelectedIndex=-1;
			listBoxTo.SelectedIndex=-1;
			listBoxExtras.SelectedIndex=-1;
			listBoxMessages.SelectedIndex=-1;
			ShowSendingLabel();
			Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
		}

		private void textDays_TextChanged(object sender,EventArgs e) {
			if(!textDays.Visible) {
				_errorProvider1.SetError(textDays,"");
				return;
			}
			int numDays=0;
			try{
				numDays=int.Parse(textDays.Text);
			}
			catch {
				_errorProvider1.SetError(textDays,Lan.g(this,"Invalid number.  Usually 1 or 2."));
				return;
			}
			_errorProvider1.SetError(textDays,"");
			_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today.AddDays(-numDays));
			try {
				FillMessages();
			}
			catch{
				_errorProvider1.SetError(textDays,Lan.g(this,"Invalid number.  Usually 1 or 2."));
			}
		}
		#endregion Methods - Event Handlers - Messaging

		#region Methods - Event Handlers - Other
		private void ControlManage_Load(object sender,EventArgs e) {
			if(!PrefC.IsODHQ) {
				butSchedule.Visible=false;
			}
			if(!PrefC.GetBoolSilent(PrefName.ChildDaycare,false)) {
				butDaycare.Visible=false;
				butDaycareCheckIn.Visible=false;
			}
		}

		private void formClaimsSend_GoToChanged(ODEventArgs e) {
			if(e.EventType!=ODEventType.FormClaimSend_GoTo) {
				return;
			}
			ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)e.Tag;
			Patient patient=Patients.GetPat(claimSendQueueItem.PatNum);
			GlobalFormOpenDental.PatientSelected(patient,false);
			GlobalFormOpenDental.GotoClaim(claimSendQueueItem.ClaimNum);
		}

		private void gridEmp_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridEmp.SelectedIndices.Length>=2) {
				SelectEmpI(-1,false);//Disable various UI elements.
				return;
			}
			bool isPrefTimeCardSecurityEnabled=PrefC.GetBool(PrefName.TimecardSecurityEnabled);
			if(!isPrefTimeCardSecurityEnabled) {
				SelectEmpI(e.Row);
				return;
			}
			if(Security.CurUser.EmployeeNum!=((Employee)gridEmp.ListGridRows[e.Row].Tag).EmployeeNum) {
				if(!Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)){
					SelectEmpI(-1,false);
					return;
				}
			}
			SelectEmpI(e.Row);
		}

		private void gridEmp_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {//Just in case
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			if(!butTimeCard.Enabled) {
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=(Employee)gridEmp.ListGridRows[e.Row].Tag;
			formTimeCard.ShowDialog();
			ModuleSelected(_patNum);
		}

		private void listStatus_Click(object sender,EventArgs e) {
			//
		}

		private void textFilterName_TextChanged(object sender,EventArgs e) {
			FillEmps(false);
		}

		private void timerUpdateTime_Tick(object sender,EventArgs e) {
			//this will happen once a second
			if(this.Visible) {
				labelTime.Text=(DateTime.Now+_timeSpanDelta).ToLongTimeString();
			}
		}
		#endregion Methods - Event Handlers - Other

		#region Methods - Public
		///<summary>Only gets run on startup.</summary>
		public void InitializeOnStartup() {
			//if(InitializedOnStartup) {
			//	return;
			//}
			//InitializedOnStartup=true;
			//can't use Lan.F
			Lan.C(this,new Control[]
				{
				groupBox3,//groupBox3 is the 'Daily' grouping.
				groupBox2,//groupBox2 is the 'Messaging' grouping.
				label1,//label1 is the 'To' text.
				butSend,
				groupBox1,//groupBox2 is the 'Time Clock' grouping.
				labelFilterName,
				butManage,
				butTimeCard,
				labelCurrentTime,
				butClaimPay,
				butClockIn,
				butClockOut,
				butEmailInbox,
				butSendClaims,
				butBilling,
				butDeposit,
				butSupply,
				butTasks,
				butBackup,
				butAccounting,
				butBreaks,
				butViewSched,
				label3,//label3 is the 'From' text.
				label4,//label4 is the 'Extras' text.
				label5,//label5 is the 'Message (&& Send)' text.
				label7,//label7 is the 'Message' text.
				labelSending,
				checkIncludeAck,
				labelDays,
				butAck,
				label6,//label6 is the 'To User' text.
				gridEmp,
				gridMessages,
				});
			RefreshFullMessages();//after this, messages just get added to the list.
			//But if checkIncludeAck is clicked,then it does RefreshMessages again.
		}

		///<summary>Only used internally to launch the task window with the Triage task list.</summary>
		public void JumpToTriageTaskWindow() {
			LaunchTaskWindow(true);
		}

		///<summary>Used to launch the task window preloaded with a certain task list open.  isTriage is only used at OD HQ.</summary>
		public void LaunchTaskWindow(bool isTriage,UserControlTasksTab tab=UserControlTasksTab.Invalid) {
			FormTasks FormTasks=new FormTasks();
			FormTasks.Show();
			if(isTriage) {
				FormTasks.ShowTriage();
				return;
			}
			if(tab!=UserControlTasksTab.Invalid) {
				FormTasks.SetUserControlTasksTab(tab);
			}
		}

		///<summary></summary>
		public void ModuleSelected(long patNum) {
			_patNum=patNum;
			RefreshModuleData(patNum);
			RefreshModuleScreen();
			Plugins.HookAddCode(this,"ContrStaff.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected() {
			//this is not getting triggered yet.
			Plugins.HookAddCode(this,"ContrStaff.ModuleUnselected_end");
		}

		public void TryRefreshFormClaimSend() {
			if(_formClaimsSend!=null && !FormODBase.IsDisposedOrClosed(_formClaimsSend)) {
				_formClaimsSend.RefreshClaimsGrid();
			}
		}
		#endregion Methods - Public

		#region Methods - Public - Messaging
		///<summary>This processes timed messages coming in from the main form.  Buttons are handled in the main form, and then sent here for further display.  The list gets filtered before display.</summary>
		public void LogMsgs(List<SigMessage> listSigMessages) {
			for(int i=0;i<listSigMessages.Count();i++) {
				SigMessage sigMessageUpdate=_listSigMessages.FirstOrDefault(x => x.SigMessageNum==listSigMessages[i].SigMessageNum);
				if(sigMessageUpdate==null) {
					_listSigMessages.Add(listSigMessages[i].Copy());
					continue;
				}
				//SigMessage is already in our list and we just need to update it.
				sigMessageUpdate.AckDateTime=listSigMessages[i].AckDateTime;
			}
			_listSigMessages.Sort();
			FillMessages();
		}
		#endregion Methods - Public - Messaging

		#region Methods - Private
		/// <summary>Returns a translated TimeClockStatus enum description from the given status.  Also considers PrefName.ClockEventAllowBreak to switch 'Lunch' to 'Break' for the UI.</summary>
		private string ConvertClockStatus(string status) {
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak) && status==TimeClockStatus.Lunch.GetDescription()) {
				status=TimeClockStatus.Break.GetDescription();
			}
			return Lans.g("enumTimeClockStatus",status);
		}

		private void FillEmps(bool selectUserEmployee) {
			gridEmp.BeginUpdate();
			gridEmp.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEmpClock","Employee"),180);
			gridEmp.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEmpClock","Status"),104);
			gridEmp.Columns.Add(col);
			gridEmp.ListGridRows.Clear();
			GridRow row;
			if(PrefC.HasClinicsEnabled) {
				_listEmployees=Employees.GetEmpsForClinic(Clinics.ClinicNum,false,true);
			}
			else {
				_listEmployees=Employees.GetDeepCopy(true);
			}
			for(int i=0;i<_listEmployees.Count();i++) {
				bool isEmployeeFNameStartingWithFilterName=_listEmployees[i].FName.ToLower().StartsWith(textFilterName.Text.ToLower());
				if(textFilterName.Text!="" && !isEmployeeFNameStartingWithFilterName) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(Employees.GetNameFL(_listEmployees[i]));
				row.Cells.Add(ConvertClockStatus(_listEmployees[i].ClockStatus));//Translated in function.
				row.Tag=_listEmployees[i];
				gridEmp.ListGridRows.Add(row);
			}
			gridEmp.EndUpdate();
			listBoxStatus.Items.Clear();
			_listTimeClockStatusesShown.Clear();
			List<TimeClockStatus> listTimeClockStatuses=Enum.GetValues(typeof(TimeClockStatus)).Cast<TimeClockStatus>().ToList();
			for(int i=0;i<listTimeClockStatuses.Count();i++) {
				string statusDescript=listTimeClockStatuses[i].GetDescription();
				if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					if(listTimeClockStatuses[i]==TimeClockStatus.Break) {
						continue;//Skip Break option.
					}
					else if(listTimeClockStatuses[i]==TimeClockStatus.Lunch) {
						statusDescript=TimeClockStatus.Break.GetDescription();//Change "Lunch" to "Break", still functions as Lunch.
					}
				}
				_listTimeClockStatusesShown.Add(listTimeClockStatuses[i]);
				listBoxStatus.Items.Add(Lan.g("enumTimeClockStatus",statusDescript));
			}
			int index=-1;
			if(!selectUserEmployee) {
				SelectEmpI(index);
				return;
			}
			//Only select current user's employee when refreshing the module.
			for(int i=0;i<gridEmp.ListGridRows.Count;i++) {
				Employee employee=(Employee)gridEmp.ListGridRows[i].Tag;
				if(employee.EmployeeNum==Security.CurUser.EmployeeNum) {
					index=i;
					break;
				}
			}
			SelectEmpI(index);
		}

		private void RefreshModuleData(long patNum) {
			if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
				_timeSpanDelta=new TimeSpan(0);
			}
			else {
				_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			}
			Employees.RefreshCache();
			//RefreshModulePatient(patNum);
		}

		///<summary>Here so it's parallel with other modules.</summary>
		//private void RefreshModulePatient(int patNum){
		//	PatCurNum=patNum;
		//	if(patNum==0){
		//		OnPatientSelected(patNum,"",false,"");
		//	}
		//	else{
		//		Patient pat=Patients.GetPat(patNum);
		//		OnPatientSelected(patNum,pat.GetNameLF(),pat.Email!="",pat.ChartNumber);
		//	}
		//}

		private void RefreshModuleScreen() {
			if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
				labelCurrentTime.Text=Lan.g(this,"Local Time");
			}
			else {
				labelCurrentTime.Text=Lan.g(this,"Server Time");
			}
			labelTime.Text=(DateTime.Now+_timeSpanDelta).ToLongTimeString();
			textFilterName.Text="";
			FillEmps(true);
			FillMessageDefs();
			if(Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)) {
				butManage.Enabled=true;
			}
			else {
				butManage.Enabled=false;
			}
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {//Breaks turned off, Lunch is now "Break", but maintains Lunch functionality.
				butBreaks.Visible=false;
			}
			else {
				butBreaks.Visible=true;
			}
			butImportInsPlans.Visible=true;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				butImportInsPlans.Visible=false;//Import Ins Plans button is only visible when Public Health feature is enabled.
			}
			butManageAR.Visible=!ProgramProperties.IsAdvertisingDisabled(ProgramName.Transworld);
		}

		///<summary>-1 is also valid.</summary>
		private void SelectEmpI(int index,bool clearGridSelection=true) {
			if(clearGridSelection) {
				gridEmp.SetAll(false);
			}
			if(index==-1) {
				butClockIn.Enabled=false;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=false;
				butBreaks.Enabled=false;
				listBoxStatus.Enabled=false;
				return;
			}
			gridEmp.SetSelected(index,true);
			_employee=(Employee)gridEmp.ListGridRows[index].Tag;
			ClockEvent clockEvent=ClockEvents.GetLastEvent(_employee.EmployeeNum);
			if(clockEvent==null) {//new employee.  They need to clock in.
				butClockIn.Enabled=true;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=true;
				butBreaks.Enabled=true;
				listBoxStatus.SelectedIndex=_listTimeClockStatusesShown.IndexOf(TimeClockStatus.Home);
				listBoxStatus.Enabled=false;
				return;
			}
			if(clockEvent.ClockStatus==TimeClockStatus.Break) {//only incomplete breaks will have been returned.
				//clocked out for break, but not clocked back in
				butClockIn.Enabled=true;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=true;
				butBreaks.Enabled=true;
				if(PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					listBoxStatus.SelectedIndex=_listTimeClockStatusesShown.IndexOf(TimeClockStatus.Break);
				}
				else {
					//This will only happen when ClockEventAllowBreak has just changed to false, but employees are clocked out for TimeClockStatus.Break.
					//Because listStatus only contains TimeClockStatus.Home and TimeClockStatus.Lunch(displays as "Break"), we can't choose TimeClockStatus.Break.
					//Choose TimeClockStatus.Lunch which displays as "Break", and allow normal clocking in/out to handle transition into newly disabled 
					//preference statuses.
					listBoxStatus.SelectedIndex=_listTimeClockStatusesShown.IndexOf(TimeClockStatus.Lunch);
				}
				listBoxStatus.Enabled=false;
				return;
			}
			//normal clock in/out
			if(clockEvent.TimeDisplayed2.Year<1880) {//clocked in to work, but not clocked back out.
				butClockIn.Enabled=false;
				butClockOut.Enabled=true;
				butTimeCard.Enabled=true;
				butBreaks.Enabled=true;
				listBoxStatus.Enabled=true;
				return;
			}
			//clocked out for home or lunch.  Need to clock back in.
			butClockIn.Enabled=true;
			butClockOut.Enabled=false;
			butTimeCard.Enabled=true;
			butBreaks.Enabled=true;
			listBoxStatus.SelectedIndex=(int)clockEvent.ClockStatus;
			listBoxStatus.Enabled=false;
		}

		///<summary>Shows FormBilling and displays warning message if needed.  Pass 0 to show all clinics.  Make sure to check for unsent bills before calling this method.  IsAllSelected is based upon
		///the comboClinic selection from formBillingOptions</summary>
		private void ShowBilling(List<long> listClinicNums,bool isHistStartMinDate=false,bool showBillTransSinceZero=false,bool isAllSelected=false) {
			//Check to see if there is an instance of the billing list window already open that needs to be closed.
			//This can happen if multiple people are trying to send bills at the same time.
			if(_formBilling!=null && !_formBilling.IsDisposed) {
				//It does not hurt to always close this window before loading a new instance, because the unsent bills are saved in the database and the entire purpose of FormBilling is the Go To feature.
				//Any statements that were showing in the old billing list window that we are about to close could potentially be stale and are now invalid and should not be sent.
				//Another good reason to close the window is when using clinics.  It was possible to show a different clinic billing list than the one chosen.
				_formBilling.Close();
			}
			_formBilling=new FormBilling();
			_formBilling.ClinicNumsSelectedInitial=listClinicNums;
			_formBilling.IsAllSelected=isAllSelected;
			_formBilling.IsHistoryStartMinDate=isHistStartMinDate;
			_formBilling.ShowBillTransSinceZero=showBillTransSinceZero;
			_formBilling.Show();//FormBilling has a Go To option and is shown as a non-modal window so the user can view the patient account and the billing list at the same time.
			_formBilling.BringToFront();
		}

		///<summary>Shows FormBillingOptions and FormBilling if needed.  Pass 0 to show all clinics.  Make sure to check for unsent bills before calling this method.</summary>
		private void ShowBillingOptions(long clinicNum) {
			using FormBillingOptions formBillingOptions=new FormBillingOptions();
			formBillingOptions.ListClinicNumsSelected=new List<long>() { clinicNum };
			formBillingOptions.ShowDialog();
			if(formBillingOptions.DialogResult==DialogResult.OK) {
				ShowBilling(formBillingOptions.ListClinicNumsSelected,formBillingOptions.IsHistoryStartMinDate,formBillingOptions.ShowBillTransSinceZero,formBillingOptions.IsAllSelected);
			}
		}

		private bool ValidateConnectionDetails() {
			Program program=Programs.GetCur(ProgramName.Transworld);
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
				if(!Security.CurUser.ClinicIsRestricted) {
					listClinicNums.Add(0);
				}
			}
			else {
				listClinicNums.Add(0);
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(program.ProgramNum);
			for(int i=0;i<listClinicNums.Count();i++) {
				List<ProgramProperty> listProgPropsForClinic=new List<ProgramProperty>();
				if(listProgramProperties.All(x => x.ClinicNum!=listClinicNums[i])) {//if no prog props exist for the clinic, continue, clinicNum 0 will be tested once as well
					continue;
				}
				listProgPropsForClinic=listProgramProperties.FindAll(x => x.ClinicNum==listClinicNums[i]);
				string sftpAddress=listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
				int sftpPort;
				if(!int.TryParse(listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
					sftpPort=22;//default to port 22
				}
				string userName=listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
				string userPassword=CDT.Class1.TryDecrypt(listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
				if(Sftp.IsConnectionValid(sftpAddress,userName,userPassword,sftpPort)) {
					return true;
				}
			}
			return false;
		}
		#endregion Methods - Private

		#region Methods - Private - Messaging
		///<summary>Gets run with each module selected.  Should be very fast.</summary>
		private void FillMessageDefs() {
			_sigElementDefArrayUsers=SigElementDefs.GetSubList(SignalElementType.User);
			_sigElementDefArrayExtras=SigElementDefs.GetSubList(SignalElementType.Extra);
			_sigElementDefArrayMessages=SigElementDefs.GetSubList(SignalElementType.Message);
			listBoxTo.Items.Clear();
			listBoxTo.Items.AddList(_sigElementDefArrayUsers,x => x.SigText);
			listBoxFrom.Items.Clear();
			listBoxFrom.Items.AddList(_sigElementDefArrayUsers,x => x.SigText);
			listBoxExtras.Items.Clear();
			listBoxExtras.Items.AddList(_sigElementDefArrayExtras,x => x.SigText);
			listBoxMessages.Items.Clear();
			listBoxMessages.Items.AddList(_sigElementDefArrayMessages,x => x.SigText);
			comboBoxViewUser.Items.Clear();
			comboBoxViewUser.Items.Add(Lan.g(this,"all"));
			for(int i=0;i<_sigElementDefArrayUsers.Length;i++) {
				comboBoxViewUser.Items.Add(_sigElementDefArrayUsers[i].SigText);
			}
			comboBoxViewUser.SelectedIndex=0;
		}

		///<summary>This does not refresh any data, just fills the grid.</summary>
		private void FillMessages() {
			if(textDays.Visible && _errorProvider1.GetError(textDays) !="") {
				return;
			}
			List<long> listSigMessageNumsSelected=gridMessages.SelectedTags<SigMessage>().Select(x => x.SigMessageNum).ToList();
			gridMessages.BeginUpdate();
			gridMessages.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTextMessages","To"),60);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","From"),60);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Sent"),63);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Ack'd"),63);
			col.TextAlign=HorizontalAlignment.Center;
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Text"),274);
			gridMessages.Columns.Add(col);
			gridMessages.ListGridRows.Clear();
			GridRow row;
			string strSigText;
			for(int i=0;i<_listSigMessages.Count;i++) {
				if(checkIncludeAck.Checked) {
					if(_listSigMessages[i].AckDateTime.Year>1880//if this is acked
						&& _listSigMessages[i].AckDateTime<DateTime.Today.AddDays(1-PIn.Long(textDays.Text))) {
						continue;
					}
				}
				else {//user does not want to include acked
					if(_listSigMessages[i].AckDateTime.Year>1880) {//if this is acked
						continue;
					}
				}
				if(_listSigMessages[i].ToUser!=""//blank user always shows
					&& comboBoxViewUser.SelectedIndex!=0 //anything other than 'all'
					&& _sigElementDefArrayUsers!=null//for startup
					&& _sigElementDefArrayUsers[comboBoxViewUser.SelectedIndex-1].SigText!=_listSigMessages[i].ToUser)//and users don't match
				{
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_listSigMessages[i].ToUser);
				row.Cells.Add(_listSigMessages[i].FromUser);
				if(_listSigMessages[i].MessageDateTime.Date==DateTime.Today) {
					row.Cells.Add(_listSigMessages[i].MessageDateTime.ToShortTimeString());
				}
				else {
					row.Cells.Add(_listSigMessages[i].MessageDateTime.ToShortDateString()+"\r\n"+_listSigMessages[i].MessageDateTime.ToShortTimeString());
				}
				if(_listSigMessages[i].AckDateTime.Year>1880) {//ok
					if(_listSigMessages[i].AckDateTime.Date==DateTime.Today) {
						row.Cells.Add(_listSigMessages[i].AckDateTime.ToShortTimeString());
					}
					else {
						row.Cells.Add(_listSigMessages[i].AckDateTime.ToShortDateString()+"\r\n"+_listSigMessages[i].AckDateTime.ToShortTimeString());
					}
				}
				else {
					row.Cells.Add("");
				}
				strSigText=_listSigMessages[i].SigText;
				SigElementDef sigElementDefExtra=SigElementDefs.GetElementDef(_listSigMessages[i].SigElementDefNumExtra);
				if(sigElementDefExtra!=null && !string.IsNullOrEmpty(sigElementDefExtra.SigText)) {
					strSigText+=(strSigText=="")?"":".  ";
					strSigText+=sigElementDefExtra.SigText;
				}
				SigElementDef sigElementDefMsg=SigElementDefs.GetElementDef(_listSigMessages[i].SigElementDefNumMsg);
				if(sigElementDefMsg!=null && !string.IsNullOrEmpty(sigElementDefMsg.SigText)) {
					strSigText+=(strSigText=="")?"":".  ";
					strSigText+=sigElementDefMsg.SigText;
				}
				row.Cells.Add(strSigText);
				row.Tag=_listSigMessages[i].Copy();
				gridMessages.ListGridRows.Add(row);
			}
			gridMessages.EndUpdate();
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) {
				SigMessage sigMessage=(SigMessage)gridMessages.ListGridRows[i].Tag;
				if(listSigMessageNumsSelected.Contains(sigMessage.SigMessageNum)) {
					gridMessages.SetSelected(i,true);
				}
			}
		}

		///<summary>Gets all new data from the database for the text messages.  Not sure yet if this will also reset the lights along the left.</summary>
		private void RefreshFullMessages() {
			_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today);//since midnight this morning.
			FillMessages();
		}

		///<summary>Shows the sending label for 1 second.</summary>
		private void ShowSendingLabel() {
			labelSending.Visible=true;
			ODThread odThread=new ODThread((o) => {
				Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
				ODException.SwallowAnyException(() => { this.Invoke(() => { labelSending.Visible=false; }); });
			});
			odThread.Start();
		}
		#endregion Methods - Private - Messaging
	}
}