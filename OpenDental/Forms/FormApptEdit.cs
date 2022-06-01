using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MigraDoc.DocumentObjectModel;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDentBusiness.UI;
using PdfSharp.Pdf;
using CodeBase;
using System.Collections;
using System.Globalization;

namespace OpenDental{
	///<summary>_appointment.AptNum cannot be trusted fully inside of this form. This form can create new appointments without inserting them into the DB. Due to this, make sure you consider new appointments and handle accordingly. See _isInsertRequired. Edit window for appointments.  Will have a DialogResult of Cancel if the appointment was marked as new and is deleted.</summary>
	public partial class FormApptEdit:FormODBase {
		#region Fields - Public
		///<summary>Procedure were attached/detached from appt and the user clicked cancel or closed the form.  Used in ApptModule to tell if we need to refresh.</summary>
		public bool HasProcsChangedAndCancel;
		///<summary>This is the way to pass a "signal" up to the parent form that OD is to close.</summary>
		public bool IsEcwCloseOD;
		///<summary>True if appt was double clicked on from the chart module gridProg.  Currently only used to trigger an appointment overlap check.</summary>
		public bool IsInChartModule;
		///<summary>True if appt was double clicked on from the ApptsOther form.  Currently only used to trigger an appointment overlap check.</summary>
		public bool IsInViewPatAppts;
		public bool IsNew;
		public bool PinClicked;
		public bool PinIsVisible;
		#endregion Fields - Public

		#region Fields - Private
		private Appointment _appointment;
		private Appointment _appointmentOld;
		///<summary>The selected appointment type when this form loads.</summary>
		private AppointmentType _appointmentTypeSelected;
		///<summary>Used when FormApptBreak is required to track what the user has selected.</summary>
		private ApptBreakSelection _apptBreakSelection=ApptBreakSelection.None;
		///<summary>The exact index of the selected item in comboApptType.</summary>
		private int _aptTypeIndex;
		private Family _family;
		///<summary>When _appointment.Status is not Planned, PtNote or PtNoteCompleted this is set to the index of Broken in comobStatus, otherwise -1.</summary>
		private int _indexStatusBroken=-1;
		private bool _isClickLocked;
		private bool _isDeleted;
		///<summary>eCW Tight or Full enabled and a DFT msg for this appt has already been sent.  The 'Finish &amp; Send' button will say 'Revise'</summary>
		private bool _isEcwHL7Sent=false;
		///<summary>If no aptNum was passed into this form, this boolean will be set to true to indicate that _appointment.AptNum cannot be trusted until after the insert occurs. Someday we should consider using the IsNew flag instead after we remove all of the appointment pre-insert logic.</summary>
		private bool _isInsertRequired=false;
		///<summary>Used when first loading the form to skip calling fill methods multiple times.</summary>
		private bool _isOnLoad;
		private bool _isPlanned;
		///<summary>Indicates this appointment has been opened from the Unscheduled list.</summary>
		private bool _isSchedulingUnscheduledAppt;
		///<summary>Lab for the current appointment.  It may be null if there is no lab.</summary>
		private LabCase _labCase;
		///<summary>A list of all Adjustments that are related to the patient's current procedures</summary>
		private List<Adjustment> _listAdjustments;
		///<summary>A list of all appointments that are associated to any procedures in the Procedures on this Appointment grid.</summary>
		private List<Appointment> _listAppointments;
		///<summary>Stale deep copy of _listAppointments to use with sync.</summary>
		private List<Appointment> _listAppointmentsOld;
		///<summary>Matches list of appointments in comboAppointmentType. Does not include hidden types unless current appointment is of that type.</summary>
		private List<AppointmentType> _listAppointmentTypes;
		private List<Benefit> _listBenefits;
		///<summary>A list of all ClaimProcs that are related to the patient's current procedures</summary>
		private List<ClaimProc> _listClaimProcs;
		private List<Def> _listDefsApptConfirmed;
		private List<Def> _listDefsApptProcsQuickAdd;
		private List<Def> _listDefsRecallUnschedStatus;
		private List<Employee> _listEmployees;
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List<PatPlan> _listPatPlans;
		///<summary>List of all procedures that show within the Procedures on this Appointment grid.  Filled on load.  Used to double check that we update other appointments that we could steal procedures from (e.g. planned appts with tp procs).</summary>
		private List<Procedure> _listProceduresForAppointment;
		///<summary>All ProcNums attached to the appt when form opened.</summary>
		private List<long> _listProcNumsAttachedStart=new List<long>();
		///<summary>All ProcNums intended to be selected on load, but without altering any procedure properties.</summary>
		private List<long> _listProcNumsPreSelected;
		///<summary>The data necessary to load the form.</summary>
		private ApptEdit.LoadData _loadData;
		private Patient _patient;
		private ProcedureCode _procedureCodeBroken=null;
		private DataTable _tableComms;
		private DataTable _tableFields;
		///<summary>Timer delays interaction with gridProc, listQuickAdd, and butAttachAll based on PrefName.FormClickDelay value. Disposed.</summary>
		private System.Windows.Forms.Timer _timerLockDelay;
		#endregion Fields - Private

		#region Constructor
		///<summary>When aptNum is 0, make sure to set a valid patNum because a new appointment will be created/inserted on OK click.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///listPreSelectedProcNums is used to preselect procs in the grid without pre-altering the procs properties, such as AptNum/PlannedAptNum</summary>
		public FormApptEdit(long aptNum,long patNum = 0,bool useApptDrawingSettings = false,Patient patient = null,
			List<long> listProcNumsPreSelected = null,DateTime? dateTNew = null,long? opNumNew = null) 
			{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isClickLocked=true;
			if(aptNum==0) {//Creating a new appointment
				_isInsertRequired=true;
				Patient patientCur=patient??Patients.GetPat(patNum);
				if(patientCur==null) {
					MsgBox.Show(this,"Invalid patient passed in.  Please call support or try again.");
					DialogResult=DialogResult.Cancel;
					if(!this.Modal) {
						Close();
					}
					return;
				}
				//not really needed, but makes it more obvious that these two parameters are not null
				_appointment=AppointmentL.MakeNewAppointment(patientCur,useApptDrawingSettings,dateTNew,opNumNew);
			}
			else {
				_appointment=Appointments.GetOneApt(aptNum);//We need this query to get the PatNum for the appointment.
			}
			_listProcNumsPreSelected=listProcNumsPreSelected;
			this.contrApptProvSlider.FormApptEdit_CheckTimeLocked=checkTimeLocked;
		}
		#endregion Constructor
		
		#region Methods - Event Handlers - Standard
		private void FormApptEdit_Load(object sender,System.EventArgs e) {
			if(_appointment==null) {//Can happen if appointment was deleted by another WS.
				MsgBox.Show(this,"Appointment no longer exists.");
				DialogResult=DialogResult.Cancel;
				if(!this.Modal) {
					Close();
				}
				return;
			}
			_appointmentTypeSelected=null;
			_aptTypeIndex=0;
			if(PrefC.GetBool(PrefName.AppointmentTypeShowPrompt) && IsNew
				&& !_appointment.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				using FormApptTypes formApptTypes=new FormApptTypes();
				formApptTypes.IsSelectionMode=true;
				formApptTypes.IsNoneAllowed=true;
				formApptTypes.ShowDialog();
				if(formApptTypes.DialogResult==DialogResult.OK) {
					_appointmentTypeSelected=formApptTypes.AppointmentTypeSelected;
				}
			}
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.Appointment,Appointments.IsAppointmentHashValid(_appointment));
			_isOnLoad=true;
			_timerLockDelay=new Timer();
			_timerLockDelay.Tick+=timerLockDelay_Tick;
			_timerLockDelay.Interval=Math.Max((int)(TimeSpan.FromSeconds(PrefC.GetDouble(PrefName.FormClickDelay,doUseEnUSFormat: true)).TotalMilliseconds),1);
			_timerLockDelay.Start();
			_loadData=ApptEdit.GetLoadData(_appointment,IsNew);
			_listProceduresForAppointment=_loadData.ListProceduresForAppointment;
			_listAppointments=_loadData.ListAppointments;
			if(_listAppointments.Find(x => x.AptNum==_appointment.AptNum)==null) {
				_listAppointments.Add(_appointment);//Add _appointment if there are no procs attached to it.
			}
			_listAppointmentsOld=_listAppointments.Select(x => x.Copy()).ToList();
			for(int i = 0;i<_listAppointments.Count;i++) {
				if(_listAppointments[i].AptNum==_appointment.AptNum) {
					_appointment=_listAppointments[i];//Changing the variable pointer so all changes are done on the element in the list.
				}
			}
			_appointmentOld=_appointment.Copy();
			if(IsNew) {
				if(!Security.IsAuthorized(Permissions.AppointmentCreate)) { //Should have been checked before appointment was inserted into DB and this form was loaded.  Left here just in case.
					DialogResult=DialogResult.Cancel;
					if(!this.Modal) {
						Close();
					}
					return;
				}
			}
			else {
				//The order of the conditional matters; C# will not evaluate the second part of the conditional if it is not needed. 
				//Changing the order will cause unneeded Security MsgBoxes to pop up.
				if(_appointment.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentEdit)
					|| (_appointment.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentCompleteEdit))) {//completed apts have their own perm.
					butOK.Enabled=false;
					butDelete.Enabled=false;
					butPin.Enabled=false;
					butTask.Enabled=false;
					gridProc.Enabled=false;
					listQuickAdd.Enabled=false;
					butAdd.Enabled=false;
					butDeleteProc.Enabled=false;
					butInsPlan1.Enabled=false;
					butInsPlan2.Enabled=false;
					butComplete.Enabled=false;
				}
			}
			if(!Security.IsAuthorized(Permissions.ApptConfirmStatusEdit,suppressMessage: true)) {//Suppress message because it would be very annoying to users.
				comboConfirmed.Enabled=false;
			}
			else if(_isSchedulingUnscheduledAppt) {//User is authorized for Permissions.ApptConfirmStatusEdit.
				//Causes the confirmation status to be reset in the UI, mimics ContrAppt.pinBoard_MouseUp(...)
				_appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,isShort: true).DefNum;
			}
			//The objects below are needed when adding procs to this appt.
			_family=_loadData.Family;
			_patient=_family.GetPatient(_appointment.PatNum);
			_listPatPlans=_loadData.ListPatPlans;
			_listBenefits=_loadData.ListBenefits;
			_listInsSubs=_loadData.ListInsSubs;
			_listInsPlans=_loadData.ListInsPlans;
			if(!PatPlans.IsPatPlanListValid(_listPatPlans,listInsSubs: _listInsSubs)) {
				_listPatPlans=PatPlans.Refresh(_appointment.PatNum);
				_listInsSubs=InsSubs.RefreshForFam(_family);
				_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			}
			_tableFields=_loadData.TableApppointmentFields;
			_tableComms=_loadData.TableComms;
			_listAdjustments=_loadData.ListAdjustments;
			_listClaimProcs=_loadData.ListClaimProcs;
			_labCase=_loadData.LabCase;
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				butRequirement.Visible=false;
				textRequirement.Visible=false;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				butSyndromicObservations.Visible=true;
				labelSyndromicObservations.Visible=true;
			}
			if(!PinIsVisible) {
				butPin.Visible=false;
			}
			string titleText = this.Text;
			_isPlanned=false;
			if(_appointment.AptStatus==ApptStatus.Planned) {
				_isPlanned=true;
				titleText=Lan.g(this,"Edit Planned Appointment")+" - "+_patient.GetNameFL();
				labelStatus.Visible=false;
				comboStatus.Visible=false;
				butDelete.Visible=false;
				if(_listAppointments.FindAll(x => x.NextAptNum==_appointment.AptNum)//This planned appt is attached to a completed appt.
					.Exists(x => x.AptStatus==ApptStatus.Complete)) {
					labelPlannedComplete.Visible=true;
				}
			}
			else if(_appointment.AptStatus==ApptStatus.PtNote) {
				labelApptNote.Text="Patient NOTE:";
				titleText=Lan.g(this,"Edit Patient Note")+" - "+_patient.GetNameFL()+" on "+_appointment.AptDateTime.DayOfWeek+", "+_appointment.AptDateTime;
				comboStatus.Items.Add(Lan.g("enumApptStatus","Patient Note"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Completed Pt. Note"));
				labelQuickAdd.Visible=false;
				labelStatus.Visible=false;
				gridProc.Visible=false;
				listQuickAdd.Visible=false;
				butAdd.Visible=false;
				butDeleteProc.Visible=false;
				butAttachAll.Visible=false;
				//textNote.Width = 400;
			}
			else if(_appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				labelApptNote.Text="Completed Patient NOTE:";
				titleText=Lan.g(this,"Edit Completed Patient Note")+" - "+_patient.GetNameFL()+" on "+_appointment.AptDateTime.DayOfWeek+", "+_appointment.AptDateTime;
				comboStatus.Items.Add(Lan.g("enumApptStatus","Patient Note"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Completed Pt. Note"));
				labelQuickAdd.Visible=false;
				labelStatus.Visible=false;
				gridProc.Visible=false;
				listQuickAdd.Visible=false;
				butAdd.Visible=false;
				butDeleteProc.Visible=false;
				butAttachAll.Visible=false;
				//textNote.Width = 400;
			}
			else {
				titleText=Lan.g(this,"Edit Appointment")+" - "+_patient.GetNameFL()+" on "+_appointment.AptDateTime.DayOfWeek+", "+_appointment.AptDateTime;
				comboStatus.Items.Add(Lan.g("enumApptStatus","Scheduled"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Complete"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","UnschedList"));
				_indexStatusBroken=comboStatus.Items.Add(Lan.g("enumApptStatus","Broken"));
			}
			SetAptCurComboStatusSelection();
			if(_appointment.Op != 0) {
				titleText+=" | "+Operatories.GetAbbrev(_appointment.Op);
			}
			this.Text = titleText;
			contrApptProvSlider.ProvBarText=_appointment.ProvBarText;
			checkASAP.Checked=_appointment.Priority==ApptPriority.ASAP;
			if(_appointment.AptStatus==ApptStatus.UnschedList) {
				if(Programs.UsingEcwTightOrFullMode()) {
					comboStatus.Enabled=true;
				}
				else if(HL7Defs.GetOneDeepEnabled()!=null && !HL7Defs.GetOneDeepEnabled().ShowAppts) {
					comboStatus.Enabled=true;
				}
				else {
					comboStatus.Enabled=false;
				}
			}
			comboUnschedStatus.Items.Add(Lan.g(this,"none"));
			comboUnschedStatus.SelectedIndex=0;
			_listDefsRecallUnschedStatus=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,isShort: true);
			_listDefsApptConfirmed=Defs.GetDefsForCategory(DefCat.ApptConfirmed,isShort: true);
			_listDefsApptProcsQuickAdd=Defs.GetDefsForCategory(DefCat.ApptProcsQuickAdd,isShort: true);
			for(int i = 0;i<_listDefsRecallUnschedStatus.Count;i++) {
				comboUnschedStatus.Items.Add(_listDefsRecallUnschedStatus[i].ItemName);
				if(_listDefsRecallUnschedStatus[i].DefNum==_appointment.UnschedStatus)
					comboUnschedStatus.SelectedIndex=i+1;
			}
			for(int i = 0;i<_listDefsApptConfirmed.Count;i++) {
				comboConfirmed.Items.Add(_listDefsApptConfirmed[i].ItemName);
				if(_listDefsApptConfirmed[i].DefNum==_appointment.Confirmed) {
					comboConfirmed.SelectedIndex=i;
				}
			}
			if(comboConfirmed.SelectedIndex==-1) {
				comboConfirmed.SetSelected(0); //default to not called
			}
			checkTimeLocked.Checked=_appointment.TimeLocked;
			textNote.Text=_appointment.Note;
			for(int i = 0;i<_listDefsApptProcsQuickAdd.Count;i++) {
				listQuickAdd.Items.Add(_listDefsApptProcsQuickAdd[i].ItemName);
			}
			comboClinic.SelectedClinicNum=_appointment.ClinicNum;
			FillCombosProv();
			comboProv.SetSelectedProvNum(_appointment.ProvNum);
			comboProvHyg.SetSelectedProvNum(_appointment.ProvHyg);//ok if 0
			checkIsHygiene.Checked=_appointment.IsHygiene;
			//Fill comboAssistant with employees and none option
			comboAssistant.Items.Add(Lan.g(this,"none"));
			comboAssistant.SelectedIndex=0;
			_listEmployees=Employees.GetDeepCopy(isShort: true);
			for(int i = 0;i<_listEmployees.Count;i++) {
				comboAssistant.Items.Add(_listEmployees[i].FName);
				if(_listEmployees[i].EmployeeNum==_appointment.Assistant)
					comboAssistant.SelectedIndex=i+1;
			}
			textLabCase.Text=GetLabCaseDescript();
			textTimeArrived.ContextMenu=contextMenuTimeArrived;
			textTimeSeated.ContextMenu=contextMenuTimeSeated;
			textTimeDismissed.ContextMenu=contextMenuTimeDismissed;
			if(_appointment.DateTimeAskedToArrive.TimeOfDay>TimeSpan.FromHours(0)) {
				textTimeAskedToArrive.Text=_appointment.DateTimeAskedToArrive.ToShortTimeString();
			}
			if(_appointment.DateTimeArrived.TimeOfDay>TimeSpan.FromHours(0)) {
				textTimeArrived.Text=_appointment.DateTimeArrived.ToShortTimeString();
			}
			if(_appointment.DateTimeSeated.TimeOfDay>TimeSpan.FromHours(0)) {
				textTimeSeated.Text=_appointment.DateTimeSeated.ToShortTimeString();
			}
			if(_appointment.DateTimeDismissed.TimeOfDay>TimeSpan.FromHours(0)) {
				textTimeDismissed.Text=_appointment.DateTimeDismissed.ToShortTimeString();
			}
			if(_appointment.AptStatus==ApptStatus.Complete
				|| _appointment.AptStatus==ApptStatus.Broken
				|| _appointment.AptStatus==ApptStatus.PtNote
				|| _appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				textInsPlan1.Text=InsPlans.GetCarrierName(_appointment.InsPlan1,_listInsPlans);
				textInsPlan2.Text=InsPlans.GetCarrierName(_appointment.InsPlan2,_listInsPlans);
			}
			else {//Get the current ins plans for the patient.
				butInsPlan1.Enabled=false;
				butInsPlan2.Enabled=false;
				InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,_listPatPlans,_listInsPlans,_listInsSubs)),_listInsSubs);
				InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,_listPatPlans,_listInsPlans,_listInsSubs)),_listInsSubs);
				_appointment.InsPlan1=insSub1.PlanNum;
				_appointment.InsPlan2=insSub2.PlanNum;
				textInsPlan1.Text=InsPlans.GetCarrierName(_appointment.InsPlan1,_listInsPlans);
				textInsPlan2.Text=InsPlans.GetCarrierName(_appointment.InsPlan2,_listInsPlans);
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				List<ReqStudent> listReqStudents=_loadData.ListReqStudents;
				string requirements="";
				for(int i = 0;i<listReqStudents.Count;i++) {
					if(i > 0) {
						requirements+="\r\n";
					}
					Provider providerStudent=Providers.GetDeepCopy().First(x => x.ProvNum==listReqStudents[i].ProvNum);
					requirements+=providerStudent.LName+", "+providerStudent.FName+": "+listReqStudents[i].Descript;
				}
				textRequirement.Text=requirements;
			}
			//IsNewPatient is set well before opening this form.
			checkIsNewPatient.Checked=_appointment.IsNewPatient;
			butColor.BackColor=_appointment.ColorOverride;
			contrApptProvSlider.MinPerIncr=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			if(Programs.UsingEcwTightOrFullMode() && !_isInsertRequired) {
				//These buttons are ONLY for eCW, not any other HL7 interface.
				butComplete.Visible=true;
				butPDF.Visible=true;
				//for eCW, we need to hide some things--------------------
				if(Bridges.ECW.AptNum==_appointment.AptNum) {
					butDelete.Visible=false;
				}
				butPin.Visible=false;
				butTask.Visible=false;
				butAddComm.Visible=false;
				if(HL7Msgs.MessageWasSent(_appointment.AptNum)) {
					_isEcwHL7Sent=true;
					butComplete.Text="Revise";
					//if(!Security.IsAuthorized(Permissions.Setup,suppressMessage: true)) {
					//	butComplete.Enabled=false;
					//	butPDF.Enabled=false;
					//}
					butOK.Enabled=false;
					gridProc.Enabled=false;
					listQuickAdd.Enabled=false;
					butAdd.Enabled=false;
					butDeleteProc.Enabled=false;
				}
				else {//hl7 was not sent for this appt
					_isEcwHL7Sent=false;
					butComplete.Text="Finish && Send";
					if(Bridges.ECW.AptNum != _appointment.AptNum) {
						butComplete.Enabled=false;
					}
					butPDF.Enabled=false;
				}
			}
			else {
				butComplete.Visible=false;
				butPDF.Visible=false;
			}
			//Hide text message button sometimes
			if(_patient.WirelessPhone=="" || (!Programs.IsEnabled(ProgramName.CallFire) && !SmsPhones.IsIntegratedTextingEnabled())) {
				butText.Enabled=false;
			}
			else {//Pat has a wireless phone number and CallFire is enabled
				butText.Enabled=true;//TxtMsgOk checking performed on button click.
			}
			//AppointmentType
			_listAppointmentTypes=AppointmentTypes.GetWhere(x => !x.IsHidden || x.AppointmentTypeNum==_appointment.AppointmentTypeNum);
			comboApptType.Items.Add(Lan.g(this,"None"));
			comboApptType.SelectedIndex=0;
			for(int i = 0;i<_listAppointmentTypes.Count;i++) {
				comboApptType.Items.Add(_listAppointmentTypes[i].AppointmentTypeName);
			}
			int selectedIndex=-1;
			if(IsNew && _appointmentTypeSelected!=null) { //_appointmentTypeSelected will be null if they didn't select anything.
				selectedIndex=_listAppointmentTypes.FindIndex(x => x.AppointmentTypeNum==_appointmentTypeSelected.AppointmentTypeNum);
			}
			else {
				selectedIndex=_listAppointmentTypes.FindIndex(x => x.AppointmentTypeNum==_appointment.AppointmentTypeNum);
			}
			comboApptType.SelectedIndex=selectedIndex+1;//+1 for none
			_aptTypeIndex=comboApptType.SelectedIndex;
			HasProcsChangedAndCancel=false;
			FillProcedures();
			if(IsNew && comboApptType.SelectedIndex!=0) {
				AptTypeHelper();
			}
			//if this is a new appointment with no procedures attached, set the time pattern using the default preference
			else if(IsNew && gridProc.SelectedIndices.Length < 1) {
				_appointment.Pattern=Appointments.GetApptTimePatternForNoProcs();
			}
			contrApptProvSlider.Pattern=_appointment.Pattern;
			contrApptProvSlider.PatternSecondary=_appointment.PatternSecondary;
			FillPatient();//Must be after FillProcedures(), so that the initial amount for the appointment can be calculated.
			SetTimeSliderColors();
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog).FirstOrDefault();
			if(userOdPrefShowAutoCommlog==null) {
				checkShowCommAuto.Checked=true;
			}
			else {
				checkShowCommAuto.Checked=PIn.Bool(userOdPrefShowAutoCommlog.ValueString);
			}
			FillComm();
			FillFields();
			textNote.Focus();
			textNote.SelectionStart = 0;
			_isOnLoad=false;
			Plugins.HookAddCode(this,"FormApptEdit.Load_End",_patient,butText);
			Plugins.HookAddCode(this,"FormApptEdit.Load_end2",_appointment);//Lower casing the code area (_end) is the newer pattern for this.
			Plugins.HookAddCode(this,"FormApptEdit.Load_end3",_appointment,_patient);
		}

		private void checkASAP_CheckedChanged(object sender,EventArgs e) {
			if(checkASAP.Checked) {
				checkASAP.ForeColor=System.Drawing.Color.Red;
			}
			else {
				checkASAP.ForeColor=SystemColors.ControlText;
			}
		}

		private void checkIsHygiene_Click(object sender,EventArgs e) {
			SetTimeSliderColors();
		}

		///<summary>Uses the UserODPref to store ShowAutomatedCommlog separately from the chart module.</summary>
		private void checkShowCommAuto_Click(object sender,EventArgs e) {
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog);
			userOdPrefShowAutoCommlog.ValueString=POut.Bool(checkShowCommAuto.Checked);
			UserOdPrefs.Upsert(userOdPrefShowAutoCommlog);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
			//refresh the data
			FillComm();
		}

		private void checkTimeLocked_Click(object sender,EventArgs e) {
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
		}

		///<summary>Only catches user changes, not programatic changes. For instance this does not fire when loading the form.</summary>
		private void comboApptType_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!AptTypeHelper()) {
				comboApptType.SelectedIndex=_aptTypeIndex;
				return;
			}
			_aptTypeIndex=comboApptType.SelectedIndex;
		}

		private void ComboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillCombosProv();
			FillProcedures();
		}

		private void comboConfirmed_SelectionChangeCommitted(object sender,EventArgs e) {
			if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)!=0 //Using appointmentTimeArrivedTrigger preference
				&& _listDefsApptConfirmed[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeArrived.Text))//time not already set 
			{
				textTimeArrived.Text=DateTime.Now.ToShortTimeString();
			}
			if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)!=0 //Using AppointmentTimeSeatedTrigger preference
				&& _listDefsApptConfirmed[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeSeated.Text))//time not already set 
			{
				textTimeSeated.Text=DateTime.Now.ToShortTimeString();
			}
			if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)!=0 //Using AppointmentTimeDismissedTrigger preference
				&& _listDefsApptConfirmed[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeDismissed.Text))//time not already set 
			{
				textTimeDismissed.Text=DateTime.Now.ToShortTimeString();
			}
		}

		private void comboProvHyg_SelectionChangeCommitted(object sender,EventArgs e) {
			SetTimeSliderColors();
		}

		private void comboProv_SelectionChangeCommitted(object sender,EventArgs e) {
			SetTimeSliderColors();
		}

		private void comboStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			//This block of logic must happen first(The if statement).
			if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
				if(_appointmentOld.AptStatus==ApptStatus.Scheduled && GetApptStatusSelected()==ApptStatus.UnschedList) {
					using FormApptBreakRequired formApptBreakRequired=new FormApptBreakRequired();
					formApptBreakRequired.ShowDialog();
					if(formApptBreakRequired.DialogResult!=DialogResult.OK) {
						return;
					}
					_apptBreakSelection=ApptBreakSelection.Unsched;
					_procedureCodeBroken=formApptBreakRequired.ProcedureCodeBrokenSelected;
					return;
				}
			}
			if(comboStatus.SelectedIndex!=_indexStatusBroken) {
				return;
			}
			if(!AppointmentL.HasBrokenApptProcs()) {
				return;
			}
			//Patient note appointment types can't have a aptstatus of broken.
			if(_appointment.AptStatus==ApptStatus.PtNoteCompleted || _appointment.AptStatus==ApptStatus.PtNote) {
				return;
			}
			if(DoPreventCompletedApptChange(PreventChangesApptAction.Status)) {
				//Change the status back to Complete before returning.
				comboStatus.SelectedIndex=1;//Complete
				return;
			}
			using FormApptBreak formApptBreak=new FormApptBreak(_appointment);
			if(formApptBreak.ShowDialog()!=DialogResult.OK) {
				SetAptCurComboStatusSelection();//Sets status back to on load selection.
				if(formApptBreak.ApptBreakSelection_==ApptBreakSelection.Delete) {
					//User wants to delete the appointment.
					OnDelete_Click(isSkipDeletePrompt: true);//Skip the standard "Delete Appointment?" prompt since we have already prompted in FormApptBreak.
					return;
				}
				_apptBreakSelection=ApptBreakSelection.None;
				_procedureCodeBroken=null;
				return;
			}
			_apptBreakSelection=formApptBreak.ApptBreakSelection_;
			_procedureCodeBroken=formApptBreak.ProcedureCodeSelected;
		}

		private void gridProc_MouseLeave(object sender,EventArgs e) {
			toolTip1.RemoveAll();
		}

		private void listQuickAdd_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(_isClickLocked) {
				return;
			}
			if(comboProv.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			if(listQuickAdd.IndexFromPoint(e.Location)==-1) {
				return;
			}
			if(_appointment.AptStatus==ApptStatus.Complete) {
				//added procedures would be marked complete when form closes. We'll just stop it here.
				if(!Security.IsAuthorized(Permissions.ProcComplCreate)) {
					return;
				}
			}
			string[] stringArrayProcCodes=_listDefsApptProcsQuickAdd[listQuickAdd.IndexFromPoint(e.Location)].ItemValue.Split(',');
			try {
				//This is the only place where a toothnum is stored in the db in international format
				ProcedureCodes.ValidateProcedureCodeEntry(stringArrayProcCodes,doAllowToothNum: true);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			List<Procedure> listProceduresAdded=ApptEdit.QuickAddProcs(_appointment,_patient,stringArrayProcCodes.ToList(),comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),_listInsSubs,_listInsPlans,_listPatPlans,_listBenefits);
			_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);
			listQuickAdd.SelectedIndex=-1;
			FillProcedures();
			for(int i = 0;i<gridProc.ListGridRows.Count;i++) {
				//at this point, all procedures in the list should have a Primary Key.
				long procNumCur=((Procedure)gridProc.ListGridRows[i].Tag).ProcNum;
				if(listProceduresAdded.Any(x => x.ProcNum==procNumCur)) {
					gridProc.SetSelected(i,true);//Select those that were just added.
				}
			}
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
			RefreshEstPatientPortion();
		}

		private void menuItemArrivedNow_Click(object sender,EventArgs e) {
			textTimeArrived.Text=DateTime.Now.ToShortTimeString();
		}

		private void menuItemDismissedNow_Click(object sender,EventArgs e) {
			textTimeDismissed.Text=DateTime.Now.ToShortTimeString();
		}

		private void menuItemSeatedNow_Click(object sender,EventArgs e) {
			textTimeSeated.Text=DateTime.Now.ToShortTimeString();
		}

		private void timerLockDelay_Tick(object sender,EventArgs e) {
			_isClickLocked=false;
			_timerLockDelay.Stop();
		}
		#endregion Methods - Event Handlers - Standard

		#region Methods - Event Handlers - Click - Center
		//The following 9 methods are ordered by usage in the center of FormApptEdit.
		private void butPickDentist_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.SelectedProvNum=comboProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formProviderPick.SelectedProvNum);
			SetTimeSliderColors();
		}

		private void butPickHyg_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(comboProvHyg.Items.GetAll<Provider>());//none option will show.
			formProviderPick.SelectedProvNum=comboProvHyg.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvHyg.SetSelectedProvNum(formProviderPick.SelectedProvNum);
			SetTimeSliderColors();
		}

		private void butColor_Click(object sender,EventArgs e) {
			ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butColorClear_Click(object sender,EventArgs e) {
			butColor.BackColor=System.Drawing.Color.FromArgb(0);
		}

		private void butLab_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(isClosing: false)) {
				return;
			}
			if(_labCase==null) {//no labcase
				//so let user pick one to add
				using FormLabCaseSelect formLabCaseSelect=new FormLabCaseSelect();
				formLabCaseSelect.PatNum=_appointment.PatNum;
				formLabCaseSelect.IsPlanned=_isPlanned;
				formLabCaseSelect.ShowDialog();
				if(formLabCaseSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				if(_isPlanned) {
					LabCases.AttachToPlannedAppt(formLabCaseSelect.SelectedLabCaseNum,_appointment.AptNum);
				}
				else {
					LabCases.AttachToAppt(formLabCaseSelect.SelectedLabCaseNum,_appointment.AptNum);
				}
			}
			else {//already a labcase attached
				using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
				formLabCaseEdit.LabCaseCur=_labCase;
				formLabCaseEdit.ShowDialog();
				if(formLabCaseEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				//Deleting or detaching labcase would have been done from in that window
			}
			_labCase=LabCases.GetForApt(_appointment);
			textLabCase.Text=GetLabCaseDescript();
		}

		private void butInsPlan1_Click(object sender,EventArgs e) {
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(_appointment.PatNum);
			formInsPlanSelect.ShowNoneButton=true;
			formInsPlanSelect.ViewRelat=false;
			formInsPlanSelect.ShowDialog();
			if(formInsPlanSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formInsPlanSelect.InsPlanSelected==null) {
				_appointment.InsPlan1=0;
				textInsPlan1.Text="";
				return;
			}
			_appointment.InsPlan1=formInsPlanSelect.InsPlanSelected.PlanNum;
			textInsPlan1.Text=InsPlans.GetCarrierName(_appointment.InsPlan1,_listInsPlans);
		}

		private void butInsPlan2_Click(object sender,EventArgs e) {
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(_appointment.PatNum);
			formInsPlanSelect.ShowNoneButton=true;
			formInsPlanSelect.ViewRelat=false;
			formInsPlanSelect.ShowDialog();
			if(formInsPlanSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formInsPlanSelect.InsPlanSelected==null) {
				_appointment.InsPlan2=0;
				textInsPlan2.Text="";
				return;
			}
			_appointment.InsPlan2=formInsPlanSelect.InsPlanSelected.PlanNum;
			textInsPlan2.Text=InsPlans.GetCarrierName(_appointment.InsPlan2,_listInsPlans);
		}

		private void butRequirement_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(isClosing: false)) {
				return;
			}
			using FormReqAppt formReqAppt=new FormReqAppt();
			formReqAppt.AptNum=_appointment.AptNum;
			formReqAppt.PatNum=_appointment.PatNum;
			formReqAppt.ShowDialog();
			if(formReqAppt.DialogResult!=DialogResult.OK) {
				return;
			}
			List<ReqStudent> listReqStudents=ReqStudents.GetForAppt(_appointment.AptNum);
			textRequirement.Text=string.Join("\r\n",listReqStudents
				.Select(x => new { Student = Providers.GetDeepCopy().First(y => y.ProvNum==x.ProvNum),Descript = x.Descript })
				.Select(x => x.Student.LName+", "+x.Student.FName+": "+x.Descript).ToList());
		}

		private void butSyndromicObservations_Click(object sender,EventArgs e) {
			if(_appointment.AptNum==0) {
				MsgBox.Show("Please click OK to create this appointment before taking this action.");
				return;
			}
			using FormEhrAptObses formEhrAptObses=new FormEhrAptObses(_appointment);
			formEhrAptObses.ShowDialog();
		}
		#endregion Methods - Event Handlers - Click - Center

		#region Methods - Event Handlers - Click - Upper
		//The following 3 methods are ordered by usage in the upper right of FormApptEdit.
		private void butDeleteProc_Click(object sender,EventArgs e) {
			//This button will not be enabled if user does not have permission for AppointmentEdit.
			if(gridProc.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one or more procedures first.");
				return;
			}
			string deleteMsg="Permanently delete all selected procedure(s)?";
			if(PrefC.GetBool(PrefName.ApptsRequireProc)) {
				List<long> listApptNums=_listAppointments.Where(x => x.AptNum!=_appointment.AptNum).Select(x => x.AptNum).ToList();
				if(!listApptNums.IsNullOrEmpty()) {//Will be empty if there is only the current appointment in the grid.
					bool areApptsGoingToBeEmpty=Appointments.AreApptsGoingToBeEmpty(gridProc.SelectedTags<Procedure>(),listApptNums);
					if(areApptsGoingToBeEmpty) {
						deleteMsg="One or more procedures being deleted are attached to another appointment. "
							+"If you permanently delete all selected procedure(s), it will leave an appointment empty. Continue?";
					}
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,deleteMsg)) {//deleteMsg can only be of two strings, okay to use in MsgBox.Show()
				return;
			}
			int skipped=0;
			int skippedSecurity=0;
			int skippedLinkedToOrthoCase=0;
			int skippedPreauth=0;
			bool isProcDeleted=false;
			List<OrthoProcLink> listOrthoProcLinks=OrthoProcLinks.GetManyForProcs(gridProc.ListGridRows.Select(x => ((Procedure)x.Tag).ProcNum).ToList());
			List<long> listSelectedProcNums=gridProc.SelectedTags<Procedure>().Select(x => x.ProcNum).ToList();
			List<ClaimProc> listClaimProcsForProc=ClaimProcs.GetForProcs(listSelectedProcNums,new List<ClaimProcStatus>(){ClaimProcStatus.Preauth}).Where(x => x.ClaimNum!=0).ToList();
			listClaimProcsForProc.DistinctBy(x => x.ClaimNum);//Removes duplicate ClaimNums.
			for(int i = 0;i<listClaimProcsForProc.Count;i++) {
				List<long> listProcNumsForClaim=ClaimProcs.RefreshForClaim(listClaimProcsForProc[i].ClaimNum).Select(x => x.ProcNum).ToList();
				//We block you from deleting all procedures on a preauth, which is consistent with the claim edit window, the chart module, and procedure edit form.
				if(listProcNumsForClaim.Except(listSelectedProcNums).Count()==0) {
					listSelectedProcNums.RemoveAll(x => listProcNumsForClaim.Contains(x));
				}
			}
			for(int i = gridProc.SelectedIndices.Length-1;i>=0;i--) {
				Procedure procedure=(Procedure)gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag;
				if(!Procedures.IsProcComplDeleteAuthorized(procedure)) {
					skipped++;
					skippedSecurity++;
					continue;
				}
				if(!procedure.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)
					&& !Security.IsAuthorized(Permissions.ProcDelete,Procedures.GetDateForPermCheck(procedure),suppressMessage: true)) {
					skippedSecurity++;
					continue;
				}
				//If selected procedure.ProcNum is linked to an ortho case, you're not allowed to delete it.
				OrthoProcLink orthoProcLink=listOrthoProcLinks.FirstOrDefault(x=>x.ProcNum==procedure.ProcNum);
				if(orthoProcLink!=null) {
					skippedLinkedToOrthoCase++;
					continue;
				}
				if(!listSelectedProcNums.Contains(procedure.ProcNum)) {
					skippedPreauth++;
					continue;
				}
				try {
					Procedures.Delete(procedure.ProcNum);
					isProcDeleted=true;
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					break;
				}
				if(procedure.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
					Permissions perm=Permissions.ProcCompleteStatusEdit;
					if(procedure.ProcStatus.In(ProcStat.EO,ProcStat.EC)) {
						perm=Permissions.ProcExistingEdit;
					}
					SecurityLogs.MakeLogEntry(perm,_appointment.PatNum,ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode
						+" ("+procedure.ProcStatus+"), "+procedure.ProcFee.ToString("c")+", Deleted");
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.ProcDelete,_appointment.PatNum,ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode
						+" ("+procedure.ProcStatus+"), "+procedure.ProcFee.ToString("c"));
				}
			}
			_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);
			if(isProcDeleted) {
				Appointments.SetProcDescript(_appointment,_listProceduresForAppointment);//This is called in Procedures.Delete(...) but is not reflected in our local _appointment.
				//This is to fix a very rare bug where the user deletes a set of procedures and then re-attaches the same procedures before closing the window.
				//This would cause the DB to have correct ProcDesript and ProcsColored values at the time. But when the user closes the window after reselecting
				//the same proces, the Appointments.Update(old,new) will not update those fields due to them being identical.
				//This would cause the appt bubble to contain the incorrect values.
				_appointmentOld.ProcDescript=_appointment.ProcDescript;
				_appointmentOld.ProcsColored=_appointment.ProcsColored;
			}
			FillProcedures();
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
			RefreshEstPatientPortion();
			if(skipped>0) {
				MessageBox.Show(Lan.g(this,"Procedures skipped due to lack of permission to edit completed procedures: ")+skipped.ToString());
			}
			if(skippedSecurity>0) {
				MessageBox.Show(Lan.g(this,"Procedures skipped due to lack of permission to delete procedures: ")+skippedSecurity.ToString());
			}
			if(skippedLinkedToOrthoCase>0) {
				MessageBox.Show(Lan.g(this,"Procedures skipped because they are linked to one or more ortho cases: ")+skippedLinkedToOrthoCase.ToString()+"\r"
					+"Detach the procedure(s) or delete the ortho case(s) first.");
			}
			if(skippedPreauth>0) {
				MessageBox.Show(Lan.g(this,"Procedures skipped because you are not allowed to delete the last procedure attached to a preauthorization: ")+POut.Int(skippedPreauth)+"\r"
					+"Detach the procedure(s) or delete the preauthorization first.");
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(comboProv.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(formProcCodes.SelectedCodeNum);
			List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(_listInsPlans);
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(_patient.PatNum); //Enforcing the discount plan date is done in Procedures.Insert
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){procedureCode },listMedicalCodes: null,//no procs to pull medical codes from yet
				new List<long>(){comboProv.GetSelectedProvNum()},_patient.PriProv,_patient.SecProv,_patient.FeeSched,_listInsPlans,new List<long>(){_appointment.ClinicNum},
				new List<Appointment>(){_appointment},listSubstitutionLinks,discountPlanNum);
			Procedure procedure=Procedures.ConstructProcedureForAppt(formProcCodes.SelectedCodeNum,_appointment,_patient,_listPatPlans,_listInsPlans,_listInsSubs,listFees);
			Procedures.Insert(procedure);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProcsRefresh=ClaimProcs.RefreshForTP(_patient.PatNum);
			List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){_patient.PatNum});
			for(int i = 0;i<listProcedures.Count;i++) {
				if(listProcedures[i].ProcNum==procedure.ProcNum) {
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProcsRefresh,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
			}
			Procedures.ComputeEstimates(procedure,_patient.PatNum,ref listClaimProcs,isInitialEntry: true,_listInsPlans,_listPatPlans,_listBenefits,
				_loadData.ListClaimProcHists,listClaimProcHistsLoop,saveToDb: true,_patient.Age,_listInsSubs,listClaimProcsAll: null,
				isClaimProcRemoveNeeded: false,useProcDateOnProc: false,listSubstitutionLinks,isForOrtho: false,listFees);
			using FormProcEdit formProcEdit=new FormProcEdit(procedure,_patient.Copy(),_family);
			formProcEdit.ListClaimProcHists=_loadData.ListClaimProcHists;
			formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			formProcEdit.IsNew=true;
			formProcEdit.ShowDialog();
			if(formProcEdit.DialogResult==DialogResult.Cancel) {
				//any created claimprocs are automatically deleted from within procEdit window.
				try {
					Procedures.Delete(procedure.ProcNum);//also deletes the claimprocs
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);
			FillProcedures();
			for(int i = 0;i<gridProc.ListGridRows.Count;i++) {
				if(procedure.ProcNum==((Procedure)gridProc.ListGridRows[i].Tag).ProcNum) {
					gridProc.SetSelected(i,true);//Select those that were just added.
				}
			}
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
			RefreshEstPatientPortion();
		}

		private void butAttachAll_Click(object sender,EventArgs e) {
			if(_isClickLocked) {
				return;
			}
			gridProc.SetAll(true);
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
		}
		#endregion Methods - Event Handlers - Click - Upper

		#region Methods - Event Handlers - ODGridClick
		private void gridComm_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow row=((DataRow)gridComm.ListGridRows[e.Row].Tag);
			long commNum=PIn.Long(row["CommlogNum"].ToString());
			long msgNum=PIn.Long(row["EmailMessageNum"].ToString());
			if (commNum>0) {
				Commlog item=Commlogs.GetOne(commNum);
				if(item==null) {
					MsgBox.Show(this,"This commlog has been deleted by another user.");
					return;
				}
				using FormCommItem formCommItem=new FormCommItem(item);
				formCommItem.ShowDialog();
			}
			else if (msgNum>0) {
				EmailMessage emailMessage=EmailMessages.GetOne(msgNum);
				if (emailMessage==null) {
					MsgBox.Show(this,"This e-mail has been deleted by another user.");
					return;
				}
				using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,isDeleteAllowed:false);
				formEmailMessageEdit.ShowDialog();
			}
			_tableComms=Appointments.GetCommTable(_appointment.PatNum.ToString(),_appointment.AptNum);
			FillComm();
		}

		private void gridPatient_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCell=gridPatient.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCell.ColorText==System.Drawing.Color.Blue && gridCell.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCell.Text);
			}
		}

		private void gridProc_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isClickLocked) {
				return;
			}
			toolTip1.RemoveAll();
			Procedure procedureSelected=((Procedure)gridProc.ListGridRows[e.Row].Tag);
			if(DisableDetachingOfCompletedProcFromCompletedAppt(procedureSelected,_appointment,out string msg)) {
				toolTip1.AutoPopDelay=5000;//5000 is the maximum a tooltip can be displayed for using this method. 
				toolTip1.IsBalloon=true;//Shows the tooltip in an easier to read speach bubble like view.
				//Anything greater requires us to use a variation of Show(...) that takes in a time duration.
				toolTip1.SetToolTip(gridProc,msg);
				return;
			}
			InvertCurProcSelected(e.Row);
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
			CalcEstPatientPortion();
		}

		private void gridProc_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isClickLocked) {
				return;
			}
			toolTip1.RemoveAll();//Ensure that any tooltip that was showing due to gridProc_CellClick(...) is removed since user was trying to double click.
			Procedure procedureSelected=((Procedure)gridProc.ListGridRows[e.Row].Tag);
			//Only invert the procedure if we didn't block the original row inversino in gridProc_CellClick(...)
			if(!DisableDetachingOfCompletedProcFromCompletedAppt(procedureSelected,_appointment,out string msg)) {
				InvertCurProcSelected(e.Row);
			}
			//This will put the selection back to what is was before the single click event.
			//Get fresh copy from DB so we are not editing a stale procedure.
			//If this is to be changed, make sure that this window is registering for procedure changes via signals or by some other means.
			Procedure procedure=Procedures.GetOneProc(((Procedure)gridProc.ListGridRows[e.Row].Tag).ProcNum,includeNote: true);
			List<ClaimProcHist> listClaimProcHists=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForTP(_patient.PatNum);
			List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){_patient.PatNum});
			for(int i = 0;i<listProcedures.Count;i++) {
				if(listProcedures[i].ProcNum==procedure.ProcNum) {
					break;
				}
				listClaimProcHists.AddRange(ClaimProcs.GetHistForProc(listClaimProcs,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
			}
			using FormProcEdit formProcEdit=new FormProcEdit(procedure,_patient,_family);
			formProcEdit.ListClaimProcHists=_loadData.ListClaimProcHists;
			formProcEdit.ListClaimProcHistsLoop=listClaimProcHists;
			formProcEdit.ShowDialog();
			if(formProcEdit.DialogResult!=DialogResult.OK) {
				CalculatePatternFromProcs();
				//SetTimeSliderColors();
				return;
			}
			_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);//We need to refresh in case the user changed the ProcCode or set the proc complete.
			//The next 3 lines are a duplicate of a section in butDeleteProc to handle deleted procedures.
			Appointments.SetProcDescript(_appointment,_listProceduresForAppointment);
			_appointmentOld.ProcDescript=_appointment.ProcDescript;
			_appointmentOld.ProcsColored=_appointment.ProcsColored;
			FillProcedures();
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			RefreshEstPatientPortion();//Need to refresh in case the user changed the ProcCode or set the proc complete.
		}

		private void gridFields_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(isClosing: false)) {
				return;
			}
			if(ApptFieldDefs.HasDuplicateFieldNames()) {//Check for duplicate field names.
				MsgBox.Show(this,"There are duplicate appointment field defs, go rename or delete the duplicates.");
				return;
			}
			ApptField apptField=ApptFields.GetOne(PIn.Long(_tableFields.Rows[e.Row]["ApptFieldNum"].ToString()));
			if(apptField==null) {
				apptField=new ApptField();
				apptField.IsNew=true;
				apptField.AptNum=_appointment.AptNum;
				apptField.FieldName=_tableFields.Rows[e.Row]["FieldName"].ToString();
				ApptFieldDef apptFieldDef=ApptFieldDefs.GetFieldDefByFieldName(apptField.FieldName);
				if(apptFieldDef==null) {//This could happen if the field def was deleted while the appointment window was open.
					MsgBox.Show(this,"This Appointment Field Def no longer exists.");
				}
				else {
					if(apptFieldDef.FieldType==ApptFieldType.Text) {
						using FormApptFieldEdit formApptFieldEdit=new FormApptFieldEdit(apptField);
						formApptFieldEdit.ShowDialog();
					}
					else if(apptFieldDef.FieldType==ApptFieldType.PickList) {
						using FormApptFieldPickEdit formApptFieldPickEdit=new FormApptFieldPickEdit(apptField);
						formApptFieldPickEdit.ShowDialog();
					}
				}
			}
			else if(ApptFieldDefs.GetFieldDefByFieldName(apptField.FieldName)!=null) {
				if(ApptFieldDefs.GetFieldDefByFieldName(apptField.FieldName).FieldType==ApptFieldType.Text) {
					using FormApptFieldEdit formApptFieldEdit=new FormApptFieldEdit(apptField);
					formApptFieldEdit.ShowDialog();
				}
				else if(ApptFieldDefs.GetFieldDefByFieldName(apptField.FieldName).FieldType==ApptFieldType.PickList) {
					using FormApptFieldPickEdit formApptFieldPickEdit=new FormApptFieldPickEdit(apptField);
					formApptFieldPickEdit.ShowDialog();
				}
			}
			else {//This probably won't happen because a field def should not be able to be deleted while in use.
				MsgBox.Show(this,"This Appointment Field Def no longer exists.");
			}
			_tableFields=Appointments.GetApptFields(_appointment.AptNum);
			FillFields();
		}
		#endregion Methods - Event Handlers - ODGridClick

		#region Methods - Event Handlers - Click - Right
		//The following 7 methods are ordered by usage on the right side of FormApptEdit.
		private void butAddComm_Click(object sender,EventArgs e) {
			Commlog commlog=new Commlog();
			commlog.IsNew=true;
			commlog.PatNum=_appointment.PatNum;
			commlog.CommDateTime=DateTime.Now;
			commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
			commlog.UserNum=Security.CurUser.UserNum;
			using FormCommItem formCommItem=new FormCommItem(commlog);
			formCommItem.DoOmitDefaults=PrefC.GetBool(PrefName.EnterpriseCommlogOmitDefaults);
			formCommItem.ShowDialog();
			_tableComms=Appointments.GetCommTable(_appointment.PatNum.ToString(),_appointment.AptNum);
			FillComm();
		}

		private void butText_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormApptEdit.butText_Click_start",_patient,_appointment,this)) {
				return;
			}
			bool updateTextYN=false;
			if(_patient.TxtMsgOk==YN.No) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient is marked to not receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return;
				}
			}
			if(_patient.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient might not want to receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return;
				}
			}
			if(updateTextYN) {
				Patient patientOld=_patient.Copy();
				_patient.TxtMsgOk=YN.Yes;
				Patients.Update(_patient,patientOld);
			}
			string message=PatComm.BuildConfirmMessage(ContactMethod.TextMessage,_patient,_appointment.DateTimeAskedToArrive,_appointment.AptDateTime);
			using FormTxtMsgEdit formTxtMsgEdit=new FormTxtMsgEdit();
			formTxtMsgEdit.PatNum=_patient.PatNum;
			formTxtMsgEdit.WirelessPhone=_patient.WirelessPhone;
			formTxtMsgEdit.Message=message;
			formTxtMsgEdit.TxtMsgOk=_patient.TxtMsgOk;
			formTxtMsgEdit.ShowDialog();
		}

		private void butPDF_Click(object sender,EventArgs e) {
			if(_isInsertRequired) {
				MsgBox.Show(this,"Please click OK to create this appointment before taking this action.");
				return;
			}
			//this will only happen for eCW HL7 interface users.
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(_appointment.AptNum,_appointment.AptStatus==ApptStatus.Planned);
			string duplicateProcs=ProcedureL.ProcsContainDuplicates(listProcedures);
			if(duplicateProcs!="") {
				MessageBox.Show(duplicateProcs);
				return;
			}
			//Send DFT to eCW containing a dummy procedure with this appointment in a .pdf file.	
			//no security
			string pdfDataStr=GenerateProceduresIntoPdf();
			if(HL7Defs.IsExistingHL7Enabled()) {
				//PDF messages do not contain FT1 segments, so proc list can be empty
				//MessageHL7 messageHL7=MessageConstructor.GenerateDFT(procs,EventTypeHL7.P03,pat,Patients.GetPat(pat.Guarantor),AptCur.AptNum,"progressnotes",pdfDataStr);
				MessageHL7 messageHL7=MessageConstructor.GenerateDFT(new List<Procedure>(),EventTypeHL7.P03,_patient,Patients.GetPat(_patient.Guarantor),_appointment.AptNum,"progressnotes",pdfDataStr);
				if(messageHL7==null) {
					MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
					return;
				}
				HL7Msg hl7Msg=new HL7Msg();
				//hl7Msg.AptNum=_appointment.AptNum;
				hl7Msg.AptNum=0;//Prevents the appt complete button from changing to the "Revise" button prematurely.
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=messageHL7.ToString();
				hl7Msg.PatNum=_patient.PatNum;
				HL7Msgs.Insert(hl7Msg);
				if(ODBuild.IsDebug()) {
					MessageBox.Show(this,messageHL7.ToString());
				}
			}
			else {
				//Note: _appointment.ProvNum may not reflect the selected provider in comboProv. This is still the Provider that the appointment was last saved with.
				Bridges.ECW.SendHL7(_appointment.AptNum,_appointment.ProvNum,_patient,pdfDataStr,"progressnotes",justPDF: true,listProcs: null);//justPDF, passing null proc list
			}
			MsgBox.Show(this,"Notes PDF sent.");
		}

		private void butComplete_Click(object sender,EventArgs e) {
			//It is OK to let the user click the OK button as long as _appointment.AptNum is NOT used prior to UpdateListAndDB().
			//if(_isInsertRequired) {
			//	MsgBox.Show(this,"Please click OK to create this appointment before taking this action.");
			//	return;
			//}
			//This is only used with eCW HL7 interface.
			DateTime datePrevious=_appointment.DateTStamp;
			if(_isEcwHL7Sent) {
				if(!Security.IsAuthorized(Permissions.EcwAppointmentRevise)) {
					return;
				}
				MsgBox.Show(this,"Any changes that you make will not be sent to eCW.  You will also have to make the same changes in eCW.");
				//revise is only clickable if user has permission
				butOK.Enabled=true;
				gridProc.Enabled=true;
				listQuickAdd.Enabled=true;
				butAdd.Enabled=true;
				butDeleteProc.Enabled=true;
				return;
			}
			List<Procedure> listProceduresForAppts=gridProc.SelectedIndices.OfType<int>().Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
			string duplicateProcs=ProcedureL.ProcsContainDuplicates(listProceduresForAppts);
			if(duplicateProcs!="") {
				MessageBox.Show(duplicateProcs);
				return;
			}
			if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"ProcNotesNoIncomplete")=="1") {
				if(listProceduresForAppts.Any(x => x.Note!=null && x.Note.Contains("\"\""))) {
					MsgBox.Show(this,"This appointment cannot be sent because there are incomplete procedure notes.");
					return;
				}
			}
			if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"ProcRequireSignature")=="1") {
				if(listProceduresForAppts.Any(x => !string.IsNullOrEmpty(x.Note) && string.IsNullOrEmpty(x.Signature))) {
					MsgBox.Show(this,"This appointment cannot be sent because there are unsigned procedure notes.");
					return;
				}
			}
			//user can only get this far if aptNum matches visit num previously passed in by eCW.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send attached procedures to eClinicalWorks and exit?")) {
				return;
			}
			comboStatus.SelectedIndex=1;//Set the appointment status to complete. This will trigger the procedures to be completed in UpdateToDB() as well.
			if(!UpdateListAndDB()) {
				return;
			}
			listProceduresForAppts=Procedures.GetProcsForSingle(_appointment.AptNum,_appointment.AptStatus==ApptStatus.Planned);
			//Send DFT to eCW containing the attached procedures for this appointment in a .pdf file.				
			string pdfDataStr=GenerateProceduresIntoPdf();
			if(HL7Defs.IsExistingHL7Enabled()) {
				//MessageConstructor.GenerateDFT(procs,EventTypeHL7.P03,pat,Patients.GetPat(pat.Guarantor),AptCur.AptNum,"progressnotes",pdfDataStr);
				MessageHL7 messageHL7=MessageConstructor.GenerateDFT(listProceduresForAppts,EventTypeHL7.P03,_patient,Patients.GetPat(_patient.Guarantor),_appointment.AptNum,
					"progressnotes",pdfDataStr);
				if(messageHL7==null) {
					MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
					return;
				}
				HL7Msg hl7Msg=new HL7Msg();
				hl7Msg.AptNum=_appointment.AptNum;
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=messageHL7.ToString();
				hl7Msg.PatNum=_patient.PatNum;
				HL7ProcAttach hl7ProcAttach=new HL7ProcAttach();
				hl7ProcAttach.HL7MsgNum=HL7Msgs.Insert(hl7Msg);
				for(int i = 0;i<listProceduresForAppts.Count;i++) {
					hl7ProcAttach.ProcNum=listProceduresForAppts[i].ProcNum;
					HL7ProcAttaches.Insert(hl7ProcAttach);
				}
			}
			else {
				Bridges.ECW.SendHL7(_appointment.AptNum,_appointment.ProvNum,_patient,pdfDataStr,"progressnotes",justPDF: false,listProceduresForAppts);
			}
			IsEcwCloseOD=true;
			if(IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,_patient.PatNum,
				_appointment.AptDateTime.ToString()+", "+_appointment.ProcDescript,
				_appointment.AptNum,datePrevious);
			}
			DialogResult=DialogResult.OK;
			if(!this.Modal) {
				Close();
			}
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(_isInsertRequired) {
				MsgBox.Show(this,"Please click OK to create this appointment before taking this action.");
				return;
			}
			List<Permissions> listPermissions=new List<Permissions>();
			listPermissions.Add(Permissions.AppointmentCreate);
			listPermissions.Add(Permissions.AppointmentEdit);
			listPermissions.Add(Permissions.AppointmentMove);
			listPermissions.Add(Permissions.AppointmentCompleteEdit);
			listPermissions.Add(Permissions.ApptConfirmStatusEdit);
			using FormAuditOneType formAuditOneType=new FormAuditOneType(_patient.PatNum,listPermissions,Lan.g(this,"Audit Trail for Appointment"),_appointment.AptNum);
			formAuditOneType.ShowDialog();
		}

		private void butTask_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(isClosing: false)) {
				return;
			}
			using FormTaskListSelect formTaskListSelect=new FormTaskListSelect(TaskObjectType.Appointment);//,_appointment.AptNum);
			formTaskListSelect.Text=Lan.g(formTaskListSelect,"Add Task")+" - "+formTaskListSelect.Text;
			formTaskListSelect.ShowDialog();
			if(formTaskListSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			task.KeyNum=_appointment.AptNum;
			task.ObjectType=TaskObjectType.Appointment;
			task.TaskListNum=formTaskListSelect.ListSelectedLists[0];
			task.UserNum=Security.CurUser.UserNum;
			using FormTaskEdit formTaskEdit=new FormTaskEdit(task,taskOld);
			formTaskEdit.IsNew=true;
			formTaskEdit.ShowDialog();
		}

		private void butPin_Click(object sender,System.EventArgs e) {
			if(_appointment.AptStatus.In(ApptStatus.UnschedList,ApptStatus.Planned)
				&& _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+_patient.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			if(!UpdateListAndDB()) {
				return;
			}
			PinClicked=true;
			DialogResult=DialogResult.OK;
			if(!this.Modal) {
				Close();
			}
		}
		#endregion Methods - Event Handlers - Click - Right

		#region Methods - Public
		public Appointment GetAppointmentCur() {
			return _appointment.Copy();
		}

		public Appointment GetAppointmentOld() {
			return _appointmentOld.Copy();
		}

		///<summary>Indicates the Appointment is being opened from the unscheduled list.</summary>
		public void IsSchedulingUnscheduledAppt(bool isSchedulingUnscheduledAppt) {
			_isSchedulingUnscheduledAppt=isSchedulingUnscheduledAppt;
		}
		#endregion Methods - Public

		#region Methods - Private
		///<summary>Returns true if the appointment type was successfully changed, returns false if the user decided to cancel out of doing so.</summary>
		private bool AptTypeHelper() {
			if(comboApptType.SelectedIndex==0) {//'None' is selected so maintain grid selections.
				return true;
			}
			if(_appointment.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				return true;//Patient notes can't have procedures associated to them.
			}
			AppointmentType appointmentType=_listAppointmentTypes[comboApptType.SelectedIndex-1];
			List<ProcedureCode> listProcedureCodesApptType=ProcedureCodes.GetFromCommaDelimitedList(appointmentType.CodeStr);
			List<Procedure> listProceduresSelected;
			if(listProcedureCodesApptType.Count>0) {//AppointmentType is associated to procs.
				listProceduresSelected=gridProc.SelectedTags<Procedure>();
				List<long> listProcCodeNumsToDetach=listProceduresSelected.Select(y => y.CodeNum).ToList()
				.Except(listProcedureCodesApptType.Select(x => x.CodeNum).ToList()).ToList();
				//if there are procedures that would get detached
				//and if they have the preference AppointmentTypeWarning on,
				//Display the warning
				if(listProcCodeNumsToDetach.Count>0 && PrefC.GetBool(PrefName.AppointmentTypeShowWarning)) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Selecting this appointment type will dissociate the current procedures from this "
						+"appointment and attach the procedures defined for this appointment type.  Do you want to continue?")) {
						return false;
					}
				}
				Appointments.ApptTypeMissingProcHelper(_appointment,appointmentType,_listProceduresForAppointment,_patient,
					canUpdateApptPattern: true,_listPatPlans,_listInsSubs,_listInsPlans,_listBenefits);
				FillProcedures();
				//Since we have detached and attached all pertinent procs by this point it is safe to just use the PlannedAptNum or AptNum.
				gridProc.SetAll(false);
				for(int i = 0;i<listProcedureCodesApptType.Count;i++) {
					List<Procedure> listProceduresSelectedApptType = listProceduresSelected.FindAll(x => x.CodeNum==listProcedureCodesApptType[i].CodeNum);
					if(listProceduresSelectedApptType.Count>0) { // If procedures with this code were already selected, preserve those selections
						gridProc.SetSelected(listProceduresSelectedApptType.Select(x => gridProc.GetTags<Procedure>().FindIndex(y => x==y)).Where(x => x>-1).ToArray(),true);
						continue;
					}
					for(int j = 0;j<gridProc.ListGridRows.Count;j++) {
						Procedure procedure=(Procedure)gridProc.ListGridRows[j].Tag;
						if(procedure.CodeNum==listProcedureCodesApptType[i].CodeNum
							//if the procedure code already exists in the grid and it's not attached to another appointment or planned appointment
							&& (_isPlanned && (procedure.PlannedAptNum==0 || procedure.PlannedAptNum==_appointment.AptNum)
								|| (!_isPlanned && (procedure.AptNum==0 || procedure.AptNum==_appointment.AptNum)))
							//The row is not already selected. This is necessary so that Apt Types with two of the same procs will select both procs.
							&& !gridProc.SelectedIndices.Contains(j)) {
							gridProc.SetSelected(j,true); //set procedures selected in the grid.
							break;
						}
					}
				}
			}
			butColor.BackColor=appointmentType.AppointmentTypeColor;
			if(appointmentType.Pattern!=null && appointmentType.Pattern!="") {
				contrApptProvSlider.Pattern=appointmentType.Pattern;
			}
			//calculate the new time pattern.
			if(appointmentType!=null && listProcedureCodesApptType != null) {
				//Has Procs, but not time.
				if(appointmentType.Pattern=="" && listProcedureCodesApptType.Count > 0) {
					//Calculate and Fill
					CalculatePatternFromProcs(ignoreTimeLocked: true);
					_appointment.Pattern=contrApptProvSlider.Pattern;
					//SetTimeSliderColors();
				}
				//Has fixed time
				else if(appointmentType.Pattern!="") {
					_appointment.Pattern=appointmentType.Pattern;
					//SetTimeSliderColors();
				}
				//No Procs, No time.
				else {
					//do nothing to the time pattern
				}
			}
			return true;
		}

		///<summary>Calculates the estimated patient portion to insert into the grid</summary>
		private void CalcEstPatientPortion() {
			List<Procedure> listProceduresSelected=gridProc.SelectedTags<Procedure>();
			decimal totalEstPatientPortion=0;
			for(int i = 0;i<listProceduresSelected.Count;i++) {
				totalEstPatientPortion+=ClaimProcs.GetPatPortion(listProceduresSelected[i],_listClaimProcs,_listAdjustments);
			}
			GridRow row=gridPatient.ListGridRows.ToList().Find(x => x.Cells[0].Text==Lans.g("FormApptEdit","Est. Patient Portion"));
			if(row==null) {
				return;//Probably some weird translation issue
			}
			row.Cells[1].Text=totalEstPatientPortion.ToString("F");
		}

		///<summary>Calculates the fee for this appointment using the highlighted procedures in the procedure list.</summary>
		private void CalcPatientFeeThisAppt() {
			double feeThisAppt=0;
			for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
				feeThisAppt+=((Procedure)(gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag)).ProcFeeTotal;
			}
			gridPatient.ListGridRows[gridPatient.ListGridRows.Count-1].Cells[1].Text=POut.Double(feeThisAppt);
			gridPatient.Invalidate();
		}

		private void CalculatePatternFromProcs(bool ignoreTimeLocked = false) {
			List<Procedure> listProcedures=new List<Procedure>();
			for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
				listProcedures.Add((Procedure)gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag);
			}
			contrApptProvSlider.Pattern=Appointments.CalculatePattern(_patient,comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),
				listProcedures,checkTimeLocked.Checked,ignoreTimeLocked);
			//contrApptProvSlider will automatically change the PatternSecondary length to match.
		}

		private bool CheckFrequencies() {
			List<Procedure> listProceduresFrequencies=new List<Procedure>();
			for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
				Procedure procedure=((Procedure)gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag).Copy();
				if(procedure.ProcStatus==ProcStat.TP) {
					listProceduresFrequencies.Add(procedure);
				}
			}
			if(listProceduresFrequencies.Count>0) {
				string frequencyConflicts="";
				DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(_patient.PatNum);
				if(discountPlanSub==null) {
					try {
						frequencyConflicts=Procedures.CheckFrequency(listProceduresFrequencies,_patient.PatNum,_appointment.AptDateTime);
					}
					catch(Exception e) {
						MessageBox.Show(Lan.g(this,"There was an error checking frequencies."
							+"  Disable the Insurance Frequency Checking feature or try to fix the following error:")
							+"\r\n"+e.Message);
						return false;
					}
					if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
						+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No) {
						return false;
					}
				}
				else {
					try {
						frequencyConflicts=DiscountPlans.CheckDiscountFrequency(listProceduresFrequencies,_patient.PatNum,_appointment.AptDateTime);
					}
					catch(Exception e) {
						MessageBox.Show(Lan.g(this,"There was an error checking discount frequencies.")
							+"\r\n"+e.Message);
						return false;
					}
					if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
						+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No) {
						return false;
					}
				}
			}
			return true;
		}

		///<summary>Validates a given procedure and appointment if the PrefName.ApptPreventChangesToCompleted is true. 
		///If the preference is on, the Procedure is complete and the Appointment is complete - it returns true. 
		///If the preference is off, the Procedure is not complete OR the Appointment is not complete, it returns false. 
		///This method outs a msg variable that is passed up from DoPreventCompletedApptProcChange(...).</summary>
		private bool DisableDetachingOfCompletedProcFromCompletedAppt(Procedure proc,Appointment aptCur,out string msg) {
			msg="";//We will not need a message in a false case, so set it to an empty string.
			return (proc.ProcStatus==ProcStat.C//Row is assoicated to completed proc
				&& aptCur.AptStatus==ApptStatus.Complete//the proc is on a completed appt
				&& DoPreventCompletedApptProcChange(proc,out msg)//PrefName.ApptPreventChangesToCompleted is enable
			);
		}

		///<summary>Returns true if the user is not allowed to change a completed appointment.</summary>
		private bool DoPreventCompletedApptChange(PreventChangesApptAction action) {
			List<Procedure> listProceduresAttached=gridProc.SelectedTags<Procedure>();
			bool doPreventChange=false;
			switch(action) {
				case PreventChangesApptAction.Delete:
					doPreventChange=AppointmentL.DoPreventChangesToCompletedAppt(_appointmentOld,action,listProceduresAttached);
					break;
				case PreventChangesApptAction.Status:
					doPreventChange=comboStatus.SelectedIndex!=1 && //Setting the Apt status to something other than Complete
						AppointmentL.DoPreventChangesToCompletedAppt(_appointmentOld,action,listProceduresAttached);
					break;
				default:
					throw new ApplicationException("Unsupported action");
			}
			return doPreventChange;
		}

		///<summary>An overload for DoPreventCompletedApptChange(...) that also handles Procedures. The inclusion of Proc's being blocked from
		///detachment when complete on a complete Appt necessitates a Proc be passed in to validate its status and a list of Procs be passed 
		///in to ensure that when a singular Proc remains on an Appt, the list can be checked to ensure that we do not allow its detachment 
		///from the Appt due to consumate logic.</summary>
		private bool DoPreventCompletedApptProcChange(Procedure proc,out string msg) {
			msg="";
			if(proc==null) {//Explicitly checking proc that was passed in because it is required.
				return true;//a valid procedure object is required when checking the Procedures action.
			}
			if(proc.ProcStatus==ProcStat.C) {
				List<Procedure> listProceduresAttached=_listProceduresForAppointment.FindAll(x => x.AptNum == _appointment.AptNum).Select(x => x.Copy()).ToList();
				return AppointmentL.DoPreventChangesToCompletedAppt(_appointmentOld,PreventChangesApptAction.Procedures,out msg,listProceduresAttached);
			}
			return false;
		}

		///<summary>Fills combo providers based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProv.SetSelectedProvNum(provNum);
			long provNumHyg=comboProvHyg.GetSelectedProvNum();
			comboProvHyg.Items.Clear();
			comboProvHyg.Items.AddProvNone();
			comboProvHyg.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProvHyg.SetSelectedProvNum(provNumHyg);
		}

		private void FillComm() {
			gridComm.BeginUpdate();
			gridComm.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCommLog","DateTime"),80);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLog","Description"),80);
			gridComm.Columns.Add(col);
			gridComm.ListGridRows.Clear();
			GridRow row;
			List<Def> listDefsMiscColors=Defs.GetDefsForCategory(DefCat.MiscColors);
			List<Def> listDefsCommLogTypes=Defs.GetDefsForCategory(DefCat.CommLogTypes);
			bool isCommlogAutomated;
			for(int i=0;i<_tableComms.Rows.Count;i++) {
				long commTypeDefNum=PIn.Long(_tableComms.Rows[i]["CommType"].ToString());
				Def defCur=Defs.GetDef(DefCat.CommLogTypes,commTypeDefNum,listDefsCommLogTypes);
				string commType=defCur==null?"":defCur.ItemValue;//EmailMessages are included in _tableComms and do not have a CommType set.
				isCommlogAutomated=Commlogs.IsAutomated(commType,PIn.Enum<CommItemSource>(_tableComms.Rows[i]["CommSource"].ToString()));
				if(!checkShowCommAuto.Checked && isCommlogAutomated) { //Skip automated commlogs if not checked.
					continue;
				}
				row=new GridRow();
				if(PIn.Long(_tableComms.Rows[i]["CommlogNum"].ToString())>0) {
					row.Cells.Add(PIn.Date(_tableComms.Rows[i]["commDateTime"].ToString()).ToShortDateString());
					if(isCommlogAutomated) {//If it's an automated commlog, show only the first line.
						row.Cells.Add(Commlogs.GetNoteFirstLine(_tableComms.Rows[i]["Note"].ToString()));
					}
					else {
						row.Cells.Add(_tableComms.Rows[i]["Note"].ToString());
					}
					if(_tableComms.Rows[i]["CommType"].ToString()==Commlogs.GetTypeAuto(CommItemTypeAuto.APPT).ToString()) {
						row.ColorBackG=listDefsMiscColors[(int)DefCatMiscColors.CommlogApptRelated].ItemColor;
					}
				}
				else if(PIn.Long(_tableComms.Rows[i]["EmailMessageNum"].ToString())>0) {
					if(((HideInFlags)PIn.Int(_tableComms.Rows[i]["EmailMessageHideIn"].ToString())).HasFlag(HideInFlags.ApptEdit)) {
						continue;
					}
					row.Cells.Add(PIn.Date(_tableComms.Rows[i]["commDateTime"].ToString()).ToShortDateString());
					row.Cells.Add(_tableComms.Rows[i]["Subject"].ToString());
				}
				row.Tag=_tableComms.Rows[i];
				gridComm.ListGridRows.Add(row);
			}
			gridComm.EndUpdate();
			gridComm.ScrollToEnd();
		}

		private void FillFields() {
			gridFields.BeginUpdate();
			gridFields.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridFields.Columns.Add(col);
			col=new GridColumn("",100);
			gridFields.Columns.Add(col);
			gridFields.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_tableFields.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableFields.Rows[i]["FieldName"].ToString());
				row.Cells.Add(_tableFields.Rows[i]["FieldValue"].ToString());
				gridFields.ListGridRows.Add(row);
			}
			gridFields.EndUpdate();
		}

		private void FillPatient() {
			DataTable table=_loadData.TablePatients;
			gridPatient.BeginUpdate();
			gridPatient.Columns.Clear();
			GridColumn col=new GridColumn("",120);//Add 2 blank columns
			gridPatient.Columns.Add(col);
			col=new GridColumn("",120);
			gridPatient.Columns.Add(col);
			gridPatient.ListGridRows.Clear();
			GridRow row;
			for(int i = 1;i<table.Rows.Count;i++) {//starts with 1 to skip name
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["field"].ToString());
				row.Cells.Add(table.Rows[i]["value"].ToString());
				if(table.Rows[i]["field"].ToString().EndsWith("Phone")  && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
					row.Cells[row.Cells.Count-1].ColorText=System.Drawing.Color.Blue;
					row.Cells[row.Cells.Count-1].Underline=YN.Yes;
				}
				gridPatient.ListGridRows.Add(row);
			}
			//Add a UI managed row to display the total fee for the selected procedures in this appointment.
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Fee This Appt"));
			row.Cells.Add("");//Calculated below
			gridPatient.ListGridRows.Add(row);
			CalcPatientFeeThisAppt();
			gridPatient.EndUpdate();
			gridPatient.ScrollToEnd();
		}

		private void FillProcedures() {
			//Every time the procedures available have been manipulated (associated to appt, deleted, etc) we need to refresh the list from the db.
			//This has the potential to call the database a lot (cell click via a grid) but we accept this inefficiency for the benefit of concurrency.
			//If the following call to the db is to be removed, make sure that all procedure manipulations from FormProcEdit, FormClaimProcEdit, etc.
			//handle the changes accordingly.  Changing this call to the database should not be done 'lightly'.  Heed our warning.
			List<Procedure> listProcedures=_listProceduresForAppointment;//Gets a full list because GetProcsForApptEdit has its optional param set true in ApptEdit.cs
			ProcedureLogic.SortProcedures(ref listProcedures);
			List<long> listNumsSelected=new List<long>();
			if(_isOnLoad && !_isInsertRequired) {//First time filling the grid and not a new appointment.
				if(_listProcNumsPreSelected!=null) {
					//Allows us to preselect procs without setting proc.PlannedAptNum in AppointmentL.CreatePlannedAppt(). Otherwise, downstream attach/detach
					//logic has problems if we preselect by setting AptNum/PlannedAptNum because that logic uses _listProcNumsAttachedStart to determine if
					//these procs were already attached to this appointment.
					listNumsSelected.AddRange(_listProcNumsPreSelected.FindAll(x => listProcedures.Any(y => y.ProcNum==x)));
				}
				if(_isPlanned) {
					_listProcNumsAttachedStart=listProcedures.FindAll(x => x.PlannedAptNum==_appointment.AptNum).Select(x => x.ProcNum).ToList();
				}
				else {//regular appointment
					//set ProcNums attached to the appt when form opened for use in automation on closing.
					_listProcNumsAttachedStart=listProcedures.FindAll(x => x.AptNum==_appointment.AptNum).Select(x => x.ProcNum).ToList();
				}
				listNumsSelected.AddRange(_listProcNumsAttachedStart);
				if(Programs.UsingEcwTightOrFullMode() && !_isEcwHL7Sent) {//for eCW only and only if not in 'Revise' mode, select completed procs from _listProcedureForAppointments with ProcDate==AptDateTime
					//Attach procs to this appointment in memory only so that Cancel button still works.
					listNumsSelected.AddRange(listProcedures.Where(x => x.ProcStatus==ProcStat.C && x.ProcDate.Date==_appointment.AptDateTime.Date).Select(x => x.ProcNum));
				}
			}
			else {//Filling the grid later on.
				listNumsSelected.AddRange(gridProc.SelectedIndices.OfType<int>().Select(x => ((Procedure)gridProc.ListGridRows[x].Tag).ProcNum));
			}
			bool isMedical=Clinics.IsMedicalPracticeOrClinic(comboClinic.SelectedClinicNum);
			gridProc.BeginUpdate();
			gridProc.ListGridRows.Clear();
			gridProc.Columns.Clear();
			List<DisplayField> listDisplayFieldsAppts;
			if(_appointment.AptStatus==ApptStatus.Planned) {
				listDisplayFieldsAppts=DisplayFields.GetForCategory(DisplayFieldCategory.PlannedAppointmentEdit);
			}
			else {
				listDisplayFieldsAppts=DisplayFields.GetForCategory(DisplayFieldCategory.AppointmentEdit);
			}
			for(int i=0;i<listDisplayFieldsAppts.Count;i++) {
				if(isMedical && (listDisplayFieldsAppts[i].InternalName=="Surf" || listDisplayFieldsAppts[i].InternalName=="Tth")) {
					continue;
				}
				gridProc.Columns.Add(new GridColumn(listDisplayFieldsAppts[i].InternalName,listDisplayFieldsAppts[i].ColumnWidth));
			}
			if(listDisplayFieldsAppts.Sum(x => x.ColumnWidth) > gridProc.Width) {
				gridProc.HScrollVisible=true;
			}
			GridRow row;
			for(int i=0;i<listProcedures.Count;i++) {
				row=new GridRow();
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
				for(int j=0;j<listDisplayFieldsAppts.Count;j++) {
					switch(listDisplayFieldsAppts[j].InternalName) {
						case "Stat":
							if(ProcMultiVisits.IsProcInProcess(listProcedures[i].ProcNum)) {
								row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
							}
							else {
								row.Cells.Add(Lans.g("enumProcStat",listProcedures[i].ProcStatus.ToString()));
							}
							break;
						case "Priority":
							row.Cells.Add(Defs.GetName(DefCat.TxPriorities,listProcedures[i].Priority));
							break;
						case "Code":
							row.Cells.Add(procedureCode.ProcCode);
							break;
						case "Tth":
							if(isMedical) {
								continue;
							}
							row.Cells.Add(Tooth.Display(listProcedures[i].ToothNum));
							break;
						case "Surf":
							if(isMedical) {
								continue;
							}
							string displaySurf;
							if(ProcedureCodes.GetProcCode(listProcedures[i].CodeNum).TreatArea==TreatmentArea.Sextant) {
								displaySurf=Tooth.GetSextant(listProcedures[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
							}
							else {
								displaySurf=Tooth.SurfTidyFromDbToDisplay(listProcedures[i].Surf,listProcedures[i].ToothNum);
							}
							row.Cells.Add(displaySurf);
							break;
						case "Description":
							string descript="";
							if(listProcedures[i].ProcNumLab!=0) {//Proc is a Canadian Lab.
								//This descript is gotten the same way it was in Appointments.GetProcTable()
								descript="^ ^ "+descript;//Visual indicator that this lab is linked to the procedure on the row above this row.
							}
							if(_isPlanned && listProcedures[i].PlannedAptNum!=0 && listProcedures[i].PlannedAptNum!=_appointment.AptNum) {
								descript+=Lan.g(this,"(other appt) ");
							}
							else if(_isPlanned && listProcedures[i].AptNum!=0 && listProcedures[i].AptNum!=_appointment.AptNum) {
								descript+=Lan.g(this,"(scheduled appt) ");
							}
							else if(!_isPlanned && listProcedures[i].PlannedAptNum!=0 && listProcedures[i].PlannedAptNum!=_appointment.AptNum) {
								descript+=Lan.g(this,"(planned appt) ");
							}
							else if(!_isPlanned && listProcedures[i].AptNum!=0 && listProcedures[i].AptNum!=_appointment.AptNum) {
								descript+=Lan.g(this,"(other appt) ");
							}
							if(procedureCode.LaymanTerm=="") {
								descript+=procedureCode.Descript;
							}
							else {
								descript+=procedureCode.LaymanTerm;
							}
							if(listProcedures[i].ToothRange!="") {
								descript+=" #"+Tooth.DisplayRange(listProcedures[i].ToothRange);
							}
							row.Cells.Add(descript);
							break;
						case "Fee":
							row.Cells.Add(listProcedures[i].ProcFeeTotal.ToString("F"));
							break;
						case "Abbreviation":
							row.Cells.Add(procedureCode.AbbrDesc);
							break;
						case "Layman's Term":
							row.Cells.Add(procedureCode.LaymanTerm);
							break;
					}
				}
				row.Tag=listProcedures[i];
				gridProc.ListGridRows.Add(row);
			}
			gridProc.EndUpdate();
			for(int i=0;i<listProcedures.Count;i++) {
				//Proc is selected, or is a Canadian Lab, and its parent is selected 
				//Selection logic to ensure the parent and children labs are selected together, this mimicks logic in ContrAccount.cs
				//See gridAccount_CellClick(...) toward the bottom.
				if(listNumsSelected.Contains(listProcedures[i].ProcNum) || listNumsSelected.Contains(listProcedures[i].ProcNumLab)) {
					gridProc.SetSelected(i,true);
				}
			}
		}

		///<summary>Creates a new .pdf file containing all of the procedures attached to this appointment and returns the contents of the .pdf file as a base64 encoded string.</summary>
		private string GenerateProceduresIntoPdf() {
			MigraDoc.DocumentObjectModel.Document document=new MigraDoc.DocumentObjectModel.Document();
			document.DefaultPageSetup.PageWidth=Unit.FromInch(8.5);
			document.DefaultPageSetup.PageHeight=Unit.FromInch(11);
			document.DefaultPageSetup.TopMargin=Unit.FromInch(.5);
			document.DefaultPageSetup.LeftMargin=Unit.FromInch(.5);
			document.DefaultPageSetup.RightMargin=Unit.FromInch(.5);
			MigraDoc.DocumentObjectModel.Section section=document.AddSection();
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,isBold: true);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(9,isBold: false);
			string text;
			//Heading---------------------------------------------------------------------------------------------------------------
			Paragraph paragraph=section.AddParagraph();
			ParagraphFormat paragraphFormat=new ParagraphFormat();
			paragraphFormat.Alignment=ParagraphAlignment.Center;
			paragraphFormat.Font=MigraDocHelper.CreateFont(10,isBold: true);
			paragraph.Format=paragraphFormat;
			text=Lan.g(this,"procedures").ToUpper();
			paragraph.AddFormattedText(text,headingFont);
			paragraph.AddLineBreak();
			text=_patient.GetNameFLFormal();
			paragraph.AddFormattedText(text,headingFont);
			paragraph.AddLineBreak();
			text=DateTime.Now.ToShortDateString();
			paragraph.AddFormattedText(text,headingFont);
			paragraph.AddLineBreak();
			paragraph.AddLineBreak();
			//Procedure List--------------------------------------------------------------------------------------------------------
			GridOD gridProg=new GridOD();
			gridProg.TranslationName="";
			this.Controls.Add(gridProg);//Only added temporarily so that printing will work. Removed at end with Dispose().
			gridProg.BeginUpdate();
			gridProg.Columns.Clear();
			GridColumn col;
			List<DisplayField> listDisplayFields=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			for(int i=0;i<listDisplayFields.Count;i++) {
				if(listDisplayFields[i].InternalName=="User" || listDisplayFields[i].InternalName=="Signed") {
					continue;
				}
				if(listDisplayFields[i].Description=="") {
					col=new GridColumn(listDisplayFields[i].InternalName,listDisplayFields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth);
				}
				if(listDisplayFields[i].InternalName=="Amount") {
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(listDisplayFields[i].InternalName=="Proc Code") {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridProg.Columns.Add(col);
			}
			gridProg.NoteSpanStart=2;
			gridProg.NoteSpanStop=7;
			gridProg.ListGridRows.Clear();
			List<Procedure> listProceduresForDay=Procedures.GetProcsForPatByDate(_appointment.PatNum,_appointment.AptDateTime);
			List<Def> listDefsProgNoteColors=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			List<Def> listDefsMiscColors=Defs.GetDefsForCategory(DefCat.MiscColors);
			for(int i = 0;i<listProceduresForDay.Count;i++) {
				Procedure procedure=listProceduresForDay[i];
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				Provider provider=Providers.GetDeepCopy().First(x => x.ProvNum==procedure.ProvNum);
				Userod userod=Userods.GetUser(procedure.UserNum);
				GridRow row=new GridRow();
				row.ColorLborder=System.Drawing.Color.Black;
				for(int f=0;f<listDisplayFields.Count;f++) {
					switch(listDisplayFields[f].InternalName) {
						case "Date":
							row.Cells.Add(procedure.ProcDate.Date.ToShortDateString());
							break;
						case "Time":
							row.Cells.Add(procedure.ProcDate.ToString("h:mm")+procedure.ProcDate.ToString("%t").ToLower());
							break;
						case "Th":
							row.Cells.Add(Tooth.Display(procedure.ToothNum));
							break;
						case "Surf":
							row.Cells.Add(procedure.Surf);
							break;
						case "Dx":
							row.Cells.Add(procedure.Dx.ToString());
							break;
						case "Description":
							row.Cells.Add((procedureCode.LaymanTerm!="") ? procedureCode.LaymanTerm : procedureCode.Descript);
							break;
						case "Stat":
							if(ProcMultiVisits.IsProcInProcess(procedure.ProcNum)) {
								row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
							}
							else {
								row.Cells.Add(Lans.g("enumProcStat",procedure.ProcStatus.ToString()));
							}
							break;
						case "Prov":
							row.Cells.Add(StringTools.Truncate(provider.Abbr,5));
							break;
						case "Amount":
							row.Cells.Add(procedure.ProcFee.ToString("F"));
							break;
						case "Proc Code":
							if(procedureCode.ProcCode.Length>5 && procedureCode.ProcCode.StartsWith("D")) {
								row.Cells.Add(procedureCode.ProcCode.Substring(0,5));//Remove suffix from all D codes.
							}
							else {
								row.Cells.Add(procedureCode.ProcCode);
							}
							break;
					}
				}
				row.Note=procedure.Note;
				//Row text color.
				switch(procedure.ProcStatus) {
					case ProcStat.TP:
						row.ColorText=listDefsProgNoteColors[0].ItemColor;
						break;
					case ProcStat.C:
						row.ColorText=listDefsProgNoteColors[1].ItemColor;
						break;
					case ProcStat.EC:
						row.ColorText=listDefsProgNoteColors[2].ItemColor;
						break;
					case ProcStat.EO:
						row.ColorText=listDefsProgNoteColors[3].ItemColor;
						break;
					case ProcStat.R:
						row.ColorText=listDefsProgNoteColors[4].ItemColor;
						break;
					case ProcStat.D:
						row.ColorText=System.Drawing.Color.Black;
						break;
					case ProcStat.Cn:
						row.ColorText=listDefsProgNoteColors[22].ItemColor;
						break;
				}
				row.ColorBackG=System.Drawing.Color.White;
				if(procedure.ProcDate.Date==DateTime.Today) {
					row.ColorBackG=listDefsMiscColors[(int)DefCatMiscColors.ChartTodaysProcs].ItemColor;
				}
				gridProg.ListGridRows.Add(row);
			}
			gridProg.EndUpdate();
			MigraDocHelper.DrawGrid(section,gridProg);
			MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(unicode: true,PdfFontEmbedding.Always);
			pdfRenderer.Document=document;
			pdfRenderer.RenderDocument();
			using MemoryStream memorystream=new MemoryStream();
			pdfRenderer.PdfDocument.Save(memorystream);
			byte[] pdfBytes=memorystream.GetBuffer();
			//#region Remove when testing is complete.
			//string tempFilePath=Path.GetTempFileName();
			//File.WriteAllBytes(tempFilePath,pdfBytes);
			//#endregion
			string pdfDataStr=Convert.ToBase64String(pdfBytes);
			return pdfDataStr;
		}

		///<summary>The currently selected ApptStatus.</summary>
		private ApptStatus GetApptStatusSelected() {
			if(_appointment.AptStatus==ApptStatus.Planned) {//Planned is not a selectable choice in the comboStatus box.
				return _appointment.AptStatus;
			}
			else if(comboStatus.SelectedIndex==-1) {
				return ApptStatus.Scheduled;
			}
			//When appointment is a patient note, comboStatus only displays 2 options; PtNote and PtNoteCompleted. See SetAptCurComboStatusSelection.
			else if(_appointment.AptStatus==ApptStatus.PtNote || _appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				if(comboStatus.SelectedIndex==0) {//Ptnote selected from comboStatus.
					return ApptStatus.PtNote;
				}
				return ApptStatus.PtNoteCompleted;
			}
			else if(comboStatus.SelectedIndex==3) {//Broken is SelectedIndex 3 but enum value 5, so when selected, we return ApptStatus.Broken.
				return ApptStatus.Broken;
			}
			else {//Scheduled, Complete, and Unscheduled are SelectedIndex 0,1,2 but enum values 1,2,3, so we index by 1 and return the appropriate ApptStatus.
				return (ApptStatus)comboStatus.SelectedIndex+1;
			}
		}

		private string GetLabCaseDescript() {
			StringBuilder stringBuilder=new StringBuilder();
			if(_labCase is null) {
				return "";
			}
			Laboratory laboratory=Laboratories.GetOne(_labCase.LaboratoryNum);
			if(laboratory!=null) {  //Laboratory won't be set if the program closed in the middle of creating a new lab entry.
				stringBuilder.Append(laboratory.Description);
			}
			else {
				stringBuilder.Append(Lan.g(this,"ERROR retrieving laboratory."));
			}
			if(_labCase.DateTimeChecked.Year>1880) {//Logic from Appointments.cs lines 1098 to 1117
				stringBuilder.Append(", "+Lan.g(this,"Quality Checked"));
				return stringBuilder.ToString();
			}
			if(_labCase.DateTimeRecd.Year>1880) {
				stringBuilder.Append(", "+Lan.g(this,"Received"));
				return stringBuilder.ToString();
			}
			if(_labCase.DateTimeSent.Year>1880) {
				stringBuilder.Append(", "+Lan.g(this,"Sent"));
			}
			else {
				stringBuilder.Append(", "+Lan.g(this,"Not Sent"));
			}
			if(_labCase.DateTimeDue.Year>1880) {
				stringBuilder.Append(", "+Lan.g(this,"Due: ")+_labCase.DateTimeDue.ToString("ddd")+" "
					+_labCase.DateTimeDue.ToShortDateString()+" "
					+_labCase.DateTimeDue.ToShortTimeString()
				);
			}
			return stringBuilder.ToString();
		}

		///<summary>Will only invert the specified procedure in the grid, even if the procedure belongs to another appointment.</summary>
		private void InvertCurProcSelected(int index) {
			bool isSelected=gridProc.SelectedIndices.Contains(index);
			List <int> listIndicies=new List<int>();
			listIndicies.Add(index);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				Procedure procedureSelected=((Procedure)gridProc.ListGridRows[index].Tag);
				if(procedureSelected.ProcNumLab==0) {//Not a lab, but could be a parent to a lab.
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						Procedure proc=(Procedure)gridProc.ListGridRows[i].Tag;
						if(proc.ProcNumLab==procedureSelected.ProcNum) {//Is lab of selected procedure.
							listIndicies.Add(i);
						}
					}
				}
				else {//Is a lab.
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						Procedure procedure=(Procedure)gridProc.ListGridRows[i].Tag;
						if(procedure.ProcNum==procedureSelected.ProcNumLab) {//Parent of selected lab.
							listIndicies.Add(i);
						}
						else if(procedure.ProcNumLab==procedureSelected.ProcNumLab && !listIndicies.Contains(i)) {
							listIndicies.Add(i);
						}
					}
				}
			}
			for(int i=0;i<listIndicies.Count;i++) {
				gridProc.SetSelected(listIndicies[i],!isSelected);//Invert selection.
			}
		}

		///<summary>Deletes the appointment, creating appropriate logs and commlogs.  Pass in </summary>
		private void OnDelete_Click(bool isSkipDeletePrompt = false) {
			if(DoPreventCompletedApptChange(PreventChangesApptAction.Delete)) {
				return;
			}
			DateTime datePrevious=_appointment.DateTStamp;
			if(_appointment.AptStatus==ApptStatus.PtNote || _appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				if(!isSkipDeletePrompt && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient Note?")) {
					return;
				}
				if(textNote.Text != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(textNote.Text,_appointment.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog Commlog = new Commlog();
						Commlog.PatNum = _appointment.PatNum;
						Commlog.CommDateTime = DateTime.Now;
						Commlog.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						Commlog.Note = "Deleted Pt NOTE from schedule, saved copy: ";
						Commlog.Note += textNote.Text;
						Commlog.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(Commlog);
					}
				}
			}
			else {//ordinary appointment
				if(!isSkipDeletePrompt && MessageBox.Show(Lan.g(this,"Delete appointment?"),"",MessageBoxButtons.OKCancel) != DialogResult.OK) {
					return;
				}
				//Only want to be able to break already scheduled appointments, this does not include new appointments in "schedule" status.
				if(_appointmentOld.AptNum!=0 && _appointment.AptStatus==ApptStatus.Scheduled && PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
					using FormApptBreakRequired formApptBreakRequired=new FormApptBreakRequired();
					formApptBreakRequired.ShowDialog();
					if(formApptBreakRequired.DialogResult!=DialogResult.OK) {
						return;
					}
					AppointmentL.BreakApptHelper(_appointment,_patient,formApptBreakRequired.ProcedureCodeBrokenSelected);
				}
				if(textNote.Text != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(textNote.Text,_appointment.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog Commlog=new Commlog();
						Commlog.PatNum=_appointment.PatNum;
						Commlog.CommDateTime=DateTime.Now;
						Commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						Commlog.Note="Deleted Appt. & saved note: ";
						if(_appointment.ProcDescript != "") {
							Commlog.Note+=_appointment.ProcDescript + ": ";
						}
						Commlog.Note+=textNote.Text;
						Commlog.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(Commlog);
					}
				}
				//If there is an existing HL7 def enabled with an outbound SIU message defined, this appointment has been inserted, and there is an outbound
				//message with AptCur.AptNum, send an SIU_S17 Appt Deletion message
				if(_appointment.AptNum>0 && HL7Defs.IsExistingHL7Enabled() && HL7Msgs.MessageWasSent(_appointment.AptNum)) {
					//S17 - Appt Deletion event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(_patient,_family.GetPatient(_patient.Guarantor),EventTypeHL7.S17,_appointment);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=_appointment.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=_patient.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							MessageBox.Show(this,messageHL7.ToString());
						}
					}
				}
				if(_appointment.AptNum>0 && HieClinics.IsEnabled()) {//Ignore new appointment delete
					HieQueues.Insert(new HieQueue(_patient.PatNum));
				}
			}
			_listAppointments.RemoveAll(x => x.AptNum==_appointment.AptNum);
			if(_appointmentOld.AptStatus!=ApptStatus.Complete) { //seperate log entry for completed appointments
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_patient.PatNum,
					"Delete for date/time: "+_appointment.AptDateTime.ToString(),
					_appointment.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,_patient.PatNum,
					"Delete for date/time: "+_appointment.AptDateTime.ToString(),
					_appointment.AptNum,datePrevious);
			}
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				if(!this.Modal) {
					Close();
				}
			}
			else {
				DialogResult=DialogResult.OK;
				_isDeleted=true;
				if(!this.Modal) {
					Close();
				}
			}
			Plugins.HookAddCode(this,"FormApptEdit.butDelete_Click_end",_appointment);
		}

		///<summary>Fully refreshes the data and then calculate the estimated patient portion</summary>
		private void RefreshEstPatientPortion() {
			_listClaimProcs=ClaimProcs.RefreshForProcs(_listProceduresForAppointment.Select(x => x.ProcNum).ToList());
			_listAdjustments=Adjustments.GetForProcs(_listProceduresForAppointment.Select(x => x.ProcNum).ToList());
			CalcEstPatientPortion();
		}

		///<summary>Sets comboStatus based on _appointment.AptStatus.
		///_appointment.AptStatus is not updated with UI selection until after UpdateListAndDB(...) is called.</summary>
		private void SetAptCurComboStatusSelection() {
			switch(_appointment.AptStatus) {
				case ApptStatus.PtNote:
				case ApptStatus.PtNoteCompleted:
					//Only Patient Note and Completed Pt. Note are options in comboStatus.
					//Subtract 7 to get either 0 (PtNote) or 1(PtNoteCompleted) for these options.
					comboStatus.SelectedIndex=(int)_appointment.AptStatus-7;
					break;
				case ApptStatus.Broken:
					comboStatus.SelectedIndex=_indexStatusBroken;
					break;
				case ApptStatus.Planned:
					//Intentionally empty, comboStatus is not visable.
					break;
				default://Scheduled, Completed, Unscheduled (When Planned comboStatus is not visable).
					comboStatus.SelectedIndex=(int)_appointment.AptStatus-1;
					break;
			}
		}

		///<summary>This was FillTime, but all it does now is set the color.  This is still useful.  Color can change frequently.</summary>
		private void SetTimeSliderColors() {
			System.Drawing.Color colorProv=System.Drawing.Color.White;
			System.Drawing.Color colorProv2=System.Drawing.Color.White;
			if(checkIsHygiene.Checked) {
				if(comboProvHyg.GetSelectedProvNum()!=0) {
					colorProv=Providers.GetColor(comboProvHyg.GetSelectedProvNum());
				}
				if(comboProv.GetSelectedProvNum()!=0) {
					colorProv2=Providers.GetColor(comboProv.GetSelectedProvNum());
				}
			}
			else {//normal
				if(comboProv.GetSelectedProvNum()!=0) {
					colorProv=Providers.GetColor(comboProv.GetSelectedProvNum());//could be white if bad provNum
				}
				if(comboProvHyg.GetSelectedProvNum()!=0) {
					colorProv2=Providers.GetColor(comboProvHyg.GetSelectedProvNum());
				}
			}
			contrApptProvSlider.ColorProv=colorProv;
			contrApptProvSlider.ColorProv2=colorProv2;
			//contrApptProvSlider.Pattern= //already handled
		}

		///<summary>Validates and saves appointment and procedure information to DB.</summary>
		private bool UpdateListAndDB(bool isClosing = true,bool doCreateSecLog = false,bool doInsertHL7 = false) {
			DateTime datePrevious=_appointment.DateTStamp;
			_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);//We need to refresh so we can check for concurrency issues.
			FillProcedures();//This refills the tags in the grid so we can use the tags below.  Will also show concurrent changes by other users.
			#region PrefName.ApptsRequireProc and Permissions.ProcComplCreate check
			//First check that they have an procedures attached to this appointment. If the appointment is an existing appointment that did not originally
			//have any procedures attached, the prompt will not come up.
			if((IsNew || _listProcNumsAttachedStart.Count>0)
				&& PrefC.GetBool(PrefName.ApptsRequireProc)
				&& gridProc.SelectedIndices.Length==0
				&& !_appointment.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				MsgBox.Show(this,"At least one procedure must be attached to the appointment.");
				return false;
			}
			if(GetApptStatusSelected()==ApptStatus.Complete
				&& gridProc.SelectedIndices.Select(x => (Procedure)gridProc.ListGridRows[x].Tag).Any(x => x.ProcStatus!=ProcStat.C)) {//Appt is complete, but a selected proc is not.
				List<Procedure> listProcedureSelected=gridProc.SelectedIndices.Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
				listProcedureSelected.RemoveAll(x => x.ProcStatus==ProcStat.C);//only care about the procs that are not already complete (new attaching procs)
				for(int i=0;i<listProcedureSelected.Count;i++) {
					if(!Security.IsAuthorized(Permissions.ProcComplCreate,_appointment.AptDateTime,listProcedureSelected[i].CodeNum,listProcedureSelected[i].ProcFee)) {
						return false;
					}
				}
			}
			#endregion
			#region Check for Procs Attached to Another Appt
			List<long> listAptNums_ToDelete = new List<long>();
			Func<List<long>,bool> funcListAptsToDelete = (listAptNumsToDelete) => {
				listAptNums_ToDelete = listAptNumsToDelete;
				return MsgBox.Show(this, MsgBoxButtons.YesNo,Appointments.PROMPT_ListAptsToDelete);
			};
			Func<bool> funcProcsConcurrentAndPlanned = () => {
				return MsgBox.Show(this, MsgBoxButtons.OKCancel,Appointments.PROMPT_PlannedProcsConcurrent);
			};
			Func<bool> funcProcsConcurrentAndNotPlanned = () => {
				return MsgBox.Show(this, MsgBoxButtons.OKCancel,Appointments.PROMPT_NotPlannedProcsConcurrent);
			};
			Action actionCompletedProceduresBeingMoved = () => {
				MsgBox.Show(this,MsgBoxButtons.OKCancel,Appointments.PROMPT_CompletedProceduresBeingMoved);
			};
			List<Procedure> listProceduresAll=Procedures.GetPatientData(_appointment.PatNum);
			List<Procedure> listProceduresInGrid = gridProc.ListGridRows.Select(x => x.Tag as Procedure).ToList();
			List<long> listProcNumsSelected = gridProc.SelectedIndices.Select(x => (gridProc.ListGridRows[x].Tag as Procedure).ProcNum).ToList();
			bool isValid=Appointments.ProcsAttachedToOtherAptsHelper(
				listProceduresInGrid, _appointment, listProcNumsSelected, _listProcNumsAttachedStart, listProceduresAll,
				funcListAptsToDelete, funcProcsConcurrentAndPlanned, funcProcsConcurrentAndNotPlanned, actionCompletedProceduresBeingMoved
			);
			if(!isValid) {
				_listProceduresForAppointment=Procedures.GetProcsForApptEdit(_appointment);//Refresh so user can see which procedures weren't added
				FillProcedures();
				return false;
			}
			#endregion Check for Procs Attached to Another Appt
			#region Validate Form Data
			//initial clinic selection based on Op, but user may also edit, so use selection.  The clinic combobox is the logical place to look
			//when being warned/blocked about specialty mismatch.  
			if(!AppointmentL.IsSpecialtyMismatchAllowed(_appointment.PatNum,comboClinic.SelectedClinicNum)) {
				return false;
			}
			if(_appointmentOld.AptStatus!=ApptStatus.UnschedList && comboStatus.SelectedIndex==2) {//previously not on unsched list and sending to unscheduled list
				if(PatRestrictionL.IsRestricted(_appointment.PatNum,PatRestrict.ApptSchedule,suppressMessage: true)) {
					MessageBox.Show(Lan.g(this,"Not allowed to send this appointment to the unscheduled list due to patient restriction")+" "
						+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+".");
					return false;
				}
				if(PrefC.GetBool(PrefName.UnscheduledListNoRecalls)
					&& Appointments.IsRecallAppointment(_appointment,gridProc.SelectedGridRows.Select(x => (Procedure)(x.Tag)).ToList())) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Recall appointments cannot be sent to the Unscheduled List.\r\nDelete appointment instead?")) {
						OnDelete_Click(isSkipDeletePrompt: true);//Skip the standard "Delete Appointment?" prompt since we have already prompted here. Closes form and syncs data.
					}
					return false;//Always return false since the appointment was either deleted of the user canceled.
				}
			}
			DateTime dateTimeAskedToArrive=DateTime.MinValue;
			if((_appointmentOld.AptStatus==ApptStatus.Complete && comboStatus.SelectedIndex!=1)
				|| (_appointmentOld.AptStatus==ApptStatus.Broken && comboStatus.SelectedIndex!=4)) //Un-completing or un-breaking the appt.  We must use selectedindex due to _appointment gets updated later UpdateDB()
			{
				//If the insurance plans have changed since this appt was completed, warn the user that the historical data will be neutralized.
				List<PatPlan> listPatPlans=PatPlans.Refresh(_patient.PatNum);
				InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,_listInsPlans,_listInsSubs)),_listInsSubs);
				InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,_listInsPlans,_listInsSubs)),_listInsSubs);
				if(insSub1.PlanNum!=_appointment.InsPlan1 || insSub2.PlanNum!=_appointment.InsPlan2) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The current insurance plans for this patient are different than the plans associated to this appointment.  They will be updated to the patient's current insurance plans.  Continue?")) {
						return false;
					}
					//Update the ins plans associated to this appointment so that they're the most accurate at this time.
					_appointment.InsPlan1=insSub1.PlanNum;
					_appointment.InsPlan2=insSub2.PlanNum;
				}
			}
			if(textTimeAskedToArrive.Text!="") {
				try {
					dateTimeAskedToArrive=_appointment.AptDateTime.Date+DateTime.Parse(textTimeAskedToArrive.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Time Asked To Arrive invalid.");
					return false;
				}
			}
			DateTime dateTimeArrived=_appointment.AptDateTime.Date;
			if(textTimeArrived.Text!="") {
				try {
					dateTimeArrived=_appointment.AptDateTime.Date+DateTime.Parse(textTimeArrived.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Time Arrived invalid.");
					return false;
				}
			}
			DateTime dateTimeSeated=_appointment.AptDateTime.Date;
			if(textTimeSeated.Text!="") {
				try {
					dateTimeSeated=_appointment.AptDateTime.Date+DateTime.Parse(textTimeSeated.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Time Seated invalid.");
					return false;
				}
			}
			DateTime dateTimeDismissed=_appointment.AptDateTime.Date;
			if(textTimeDismissed.Text!="") {
				try {
					dateTimeDismissed=_appointment.AptDateTime.Date+DateTime.Parse(textTimeDismissed.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Time Dismissed invalid.");
					return false;
				}
			}
			//This change was just slightly too risky to make to 6.9, so 7.0 only
			if(!PrefC.GetBool(PrefName.ApptAllowFutureComplete)//Not allowed to set future appts complete.
				&& _appointment.AptStatus!=ApptStatus.Complete//was not originally complete
				&& _appointment.AptStatus!=ApptStatus.PtNote
				&& _appointment.AptStatus!=ApptStatus.PtNoteCompleted
				&& comboStatus.SelectedIndex==1 //making it complete
				&& _appointment.AptDateTime.Date > DateTime.Today)//and future appt
			{
				MsgBox.Show(this,"Not allowed to set future appointments complete.");
				return false;
			}
			//get a list of procs selected on the appointment
			List<Procedure> listAttachedProcs = gridProc.SelectedIndices.Select(x=>gridProc.ListGridRows[x].Tag as Procedure).ToList();
			//check to see if any procedures on the appointment have a proc code in a hidden category
			List<string> listHiddenProcCodes=ProcedureCodes.GetProcCodesInHiddenCats(listAttachedProcs.Select(x => x.CodeNum).ToArray());
			// if they do, and we are setting the appointment complete, block from completing appointment
			if(listHiddenProcCodes.Count > 0 && GetApptStatusSelected() == ApptStatus.Complete) {
				string message=Lan.g(this,"Cannot complete appointment because the following procedures are in a hidden category:\r\n")+" "+string.Join("\r\n",listHiddenProcCodes);
				MsgBox.Show(message);
				return false;
			}
			bool hasProcsAttached=gridProc.SelectedIndices
				//Get tags on rows as procedures if possible
				.Select(x=>gridProc.ListGridRows[x].Tag as Procedure)
				//true if any row had a valid procedure as a tag
				.Any(x=>x!=null);
			if(!PrefC.GetBool(PrefName.ApptAllowEmptyComplete)
				&& _appointment.AptStatus!=ApptStatus.Complete//was not originally complete
				&& _appointment.AptStatus!=ApptStatus.PtNote
				&& _appointment.AptStatus!=ApptStatus.PtNoteCompleted
				&& comboStatus.SelectedIndex==1)//making it complete
			{
				if(!hasProcsAttached) {
					MsgBox.Show(this,"Appointments without procedures attached can not be set complete.");
					return false;
				}
			}
			if(DoPreventCompletedApptChange(PreventChangesApptAction.Status)) {
				//Not allowed to change existing completed appointment.
				//Change the status back to Complete before returning.
				comboStatus.SelectedIndex=1;//Complete
				return false;
			}
			#region Security checks
			if(_appointment.AptStatus!=ApptStatus.Complete//was not originally complete
				&& GetApptStatusSelected()==ApptStatus.Complete //trying to make it complete
				&& hasProcsAttached
				&& !Security.IsAuthorized(Permissions.ProcComplCreate,_appointment.AptDateTime))//aren't authorized to complete procedures
			{
				return false;
			}
			#endregion
			#region Provider Term Date Check
			//Prevents appointments with providers that are past their term end date from being scheduled
			Appointment appointmentProviderCheck=_appointment.Copy();//Appt used only for the providers S class method
			appointmentProviderCheck.ProvNum=comboProv.GetSelectedProvNum();
			appointmentProviderCheck.ProvHyg=comboProvHyg.GetSelectedProvNum();
			if(GetApptStatusSelected()!=ApptStatus.UnschedList && GetApptStatusSelected()!=ApptStatus.Planned) {
				string message=Providers.CheckApptProvidersTermDates(appointmentProviderCheck);
				if(message!="") {
					MessageBox.Show(this,message);//translated in Providers S class method
					return false;
				}
			}
			#endregion Provider Term Date Check
			List<Procedure> listProcedures=gridProc.SelectedIndices.OfType<int>().Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
			if(listProcedures.Count>0 && comboStatus.SelectedIndex==1 && _appointment.AptDateTime.Date>DateTime.Today.Date
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Not allowed to set procedures complete with future dates.");
				return false;
			}
			#endregion Validate Form Data
			//-----Point of no return-----
			#region Broken appt selections
			if(_apptBreakSelection==ApptBreakSelection.Unsched && !AppointmentL.ValidateApptUnsched(_appointment)) {
				_apptBreakSelection=ApptBreakSelection.None;//This way no additional logic runs below.
			}
			if(_apptBreakSelection==ApptBreakSelection.Pinboard && !AppointmentL.ValidateApptToPinboard(_appointment)) {
				_apptBreakSelection=ApptBreakSelection.None;//This way no additional logic runs below.
			}
			#endregion
			#region Set _appointment Fields
			_appointment.Pattern=contrApptProvSlider.Pattern;
			_appointment.PatternSecondary=contrApptProvSlider.PatternSecondary;
			//Only run appt overlap check if editing an appt not in unscheduled list and in chart module and eCW program link not enabled.
			//Also need to see if there is a generic HL7 def enabled where Open Dental is not the filler application.
			//Open Dental is the filler application if appointments, schedules, and operatories are maintained by Open Dental and messages are sent out
			//to inform another software of any changes made.  If Open Dental is an auxiliary application, appointments are created from inbound SIU
			//messages and Open Dental no longer has control over whether the appointments overlap or which operatory/provider's schedule the appointment
			//belongs to.  In this case, we do not want to check for overlapping appointments and the appointment module should be hidden.
			HL7Def hl7DefEnabled=HL7Defs.GetOneDeepEnabled();//the ShowAppts check box is hidden for MedLab HL7 interfaces, so only need to check the others
			bool isAuxiliaryRole=false;
			if(hl7DefEnabled!=null && !hl7DefEnabled.ShowAppts) {//if the appts module is hidden
				//if an inbound SIU message is defined, OD is the auxiliary application which neither exerts control over nor requests changes to a schedule
				isAuxiliaryRole=hl7DefEnabled.hl7DefMessages.Any(x => x.MessageType==MessageTypeHL7.SIU && x.InOrOut==InOutHL7.Incoming);
			}
			if((IsInChartModule || IsInViewPatAppts)
				&& !Programs.UsingEcwTightOrFullMode()//if eCW Tight or Full mode, appts created from inbound SIU messages and appt module always hidden
				&& _appointment.AptStatus!=ApptStatus.UnschedList
				&& !isAuxiliaryRole)//generic HL7 def enabled, appt module hidden and an inbound SIU msg defined, appts created from msgs so no overlap check
			{
				//Adjusts _appointment.Pattern directly when necessary.
				if(ControlAppt.TryAdjustAppointmentPattern(_appointment,ControlApptPanel.GetListOpsVisible())) {
					MsgBox.Show(this,"Appointment is too long and would overlap another appointment or blockout.  Automatically shortened to fit.");
//todo? Consider changing PatternSecondary length to match Pattern length.  But there are many places in the program where this would need to be done.  Probably easier to assume they can be out of synch.
				}
			}
			_appointment.ProvBarText=contrApptProvSlider.ProvBarText;
			_appointment.Priority=ApptPriority.Normal;
			if(checkASAP.Checked) {
				_appointment.Priority=ApptPriority.ASAP;
			}
			_appointment.AptStatus=GetApptStatusSelected();
			//set procs complete was moved further down
			if(comboUnschedStatus.SelectedIndex==0) {//none
				_appointment.UnschedStatus=0;
			}
			else {
				_appointment.UnschedStatus=_listDefsRecallUnschedStatus[comboUnschedStatus.SelectedIndex-1].DefNum;
			}
			if(comboConfirmed.SelectedIndex!=-1) {
				_appointment.Confirmed=_listDefsApptConfirmed[comboConfirmed.SelectedIndex].DefNum;
			}
			_appointment.TimeLocked=checkTimeLocked.Checked;
			_appointment.ColorOverride=butColor.BackColor;
			_appointment.Note=textNote.Text;
			_appointment.ClinicNum=comboClinic.SelectedClinicNum;
			_appointment.ProvNum=comboProv.GetSelectedProvNum();
			_appointment.ProvHyg=comboProvHyg.GetSelectedProvNum();
			_appointment.IsHygiene=checkIsHygiene.Checked;
			if(comboAssistant.SelectedIndex==0) {//none
				_appointment.Assistant=0;
			}
			else {
				_appointment.Assistant=_listEmployees[comboAssistant.SelectedIndex-1].EmployeeNum;
			}
			_appointment.IsNewPatient=checkIsNewPatient.Checked;
			_appointment.DateTimeAskedToArrive=dateTimeAskedToArrive;
			_appointment.DateTimeArrived=dateTimeArrived;
			_appointment.DateTimeSeated=dateTimeSeated;
			_appointment.DateTimeDismissed=dateTimeDismissed;
			//_appointment.InsPlan1 and InsPlan2 already handled 
			if(comboApptType.SelectedIndex==0) {//0 index = none.
				_appointment.AppointmentTypeNum=0;
			}
			else {
				_appointment.AppointmentTypeNum=_listAppointmentTypes[comboApptType.SelectedIndex-1].AppointmentTypeNum;
			}
			#endregion Set _appointment Fields
			#region Update ProcDescript for Appt
			//Use the current selections to set _appointment.ProcDescript.
			List<Procedure> listProceduresGridSelected=new List<Procedure>();
			gridProc.SelectedIndices.ToList().ForEach(x => listProceduresGridSelected.Add(_listProceduresForAppointment[x].Copy()));
			for(int i=0;i<listProceduresGridSelected.Count;i++) {
				//This allows Appointments.SetProcDescript(...) to associate all the passed in procs into _appointment.ProcDescript
				//listProcedureGridSelected is only used here and contains copies of procs.
				listProceduresGridSelected[i].AptNum=_appointment.AptNum;
				listProceduresGridSelected[i].PlannedAptNum=_appointment.AptNum;
			}
			Appointments.SetProcDescript(_appointment,listProceduresGridSelected);
			#endregion Update ProcDescript for Appt
			#region Provider change and fee change check
			//Determines if we would like to update ProcFees when a provider changes, considers PrefName.ProcFeeUpdatePrompt.
			bool updateProcFees=false;
			if(_appointment.AptStatus!=ApptStatus.Complete && (comboProv.GetSelectedProvNum()!=_appointmentOld.ProvNum || comboProvHyg.GetSelectedProvNum()!=_appointmentOld.ProvHyg)) {//Either the primary or hygienist changed.
				List<Procedure> listProceduresNew=gridProc.SelectedIndices.Select(x => Procedures.ChangeProcInAppointment(_appointment,((Procedure)gridProc.ListGridRows[x].Tag).Copy())).ToList();
				List<Procedure> listProceduresOld=gridProc.SelectedIndices.Select(x => ((Procedure)gridProc.ListGridRows[x].Tag).Copy()).ToList();
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(_appointment.PatNum);
				string promptText="";
				//Indicates whether GetApptStatusSelected() is a patient note.
				bool isPtNote = GetApptStatusSelected().In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted);
				//PatientNote "Appointment" will never have fees.  Prompting/Updating proc fees unnecessary.
				updateProcFees=(!isPtNote && Procedures.ShouldFeesChange(listProceduresNew,listProceduresOld,ref promptText,procFeeHelper));
				if(updateProcFees && promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
					updateProcFees=false;
				}
			}
			bool removeCompletedProcs=ProcedureL.DoRemoveCompletedProcs(_appointment,listProceduresGridSelected,checkForAllProcCompl: true);
			#endregion
			#region Save to DB
			Appointments.ApptSaveHelperResult apptSaveHelperResult;
			try {
				apptSaveHelperResult=Appointments.ApptSaveHelper(_appointment,_appointmentOld,_isInsertRequired,_listProceduresForAppointment,_listAppointments,
					gridProc.SelectedIndices.ToList(),_listProcNumsAttachedStart,_isPlanned,_listInsPlans,_listInsSubs,comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),
					listProceduresGridSelected,IsNew,_patient,_family,updateProcFees,removeCompletedProcs,doCreateSecLog,doInsertHL7);
				_appointment=apptSaveHelperResult.AptCur;
				_listProceduresForAppointment=apptSaveHelperResult.ListProcsForAppt;
				_listAppointments=apptSaveHelperResult.ListAppts;
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
			if(_isInsertRequired && _appointmentOld.AptNum==0) {
				//Update the the old AptNum since this is a new appointment.
				//This stops Appointments.Sync(...) from double insertings this new appointment.
				_appointmentOld.AptNum=_appointment.AptNum;
				_listAppointmentsOld.FirstOrDefault(x => x.AptNum==0).AptNum=_appointment.AptNum;
			}
			_isInsertRequired=false;//Now that we have inserted the new appointment, let typical appointment logic handle from here on.
			#endregion Save changes to DB
			#region Update gridProc tags
			//update tags with changes made so that anyone accessing it later has an updated copy.
			for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
				Procedure procedure=_listProceduresForAppointment.FirstOrDefault(x => x.ProcNum==((Procedure)gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag).ProcNum);
				if(procedure==null) {
					continue;
				}
				gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag=procedure.Copy();
			}
			#endregion
			#region Automation
			if(apptSaveHelperResult.DoRunAutomation) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,_listProceduresForAppointment.FindAll(x => x.AptNum==_appointment.AptNum)
					.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList(),_appointment.PatNum);
			}
			if(_appointment.AptStatus==ApptStatus.Complete) {
				ProcedureL.AfterProcsSetComplete(listProceduresGridSelected);
			}
			#endregion Automation
			#region Broken Appt Logic
			//Do the appointment "break" automation for appointments that were just broken or going to the unscheduled list (sometimes).
			//If BrokenApptRequiredOnMove is on and a user selects the unsched list drop down item, the appointment 
			//ends up here with a status of UnschedList because the appointment has not been broken yet.
			if(_appointment.AptStatus==ApptStatus.Broken && _appointmentOld.AptStatus!=ApptStatus.Broken || (PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)
				&& _appointment.AptStatus==ApptStatus.UnschedList && _appointmentOld.AptStatus==ApptStatus.Scheduled)) {
				AppointmentL.BreakApptHelper(_appointment,_patient,_procedureCodeBroken);
				if(isClosing) {
					switch(_apptBreakSelection) {//ApptBreakSelection.None by default.
						case ApptBreakSelection.Unsched:
							AppointmentL.SetApptUnschedHelper(_appointment,_patient);
							break;
						case ApptBreakSelection.Pinboard:
							AppointmentL.CopyAptToPinboardHelper(_appointment);
							break;
						case ApptBreakSelection.None://User did not makes selection
						case ApptBreakSelection.ApptBook://User made selection, no extra logic required.
							break;
					}
				}
			}
			#endregion Broken Appt Logic
			#region Cleanup Empty Apts
			Appointments.DeleteEmptyPlannedAppts(listAptNums_ToDelete, _appointment.PatNum);
			#endregion
			return true;
		}
		#endregion Methods - Private

		private void butDelete_Click(object sender,EventArgs e) {
			OnDelete_Click();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DateTime datePrevious=_appointment.DateTStamp;
			if(comboProv.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			if(_appointmentOld.AptStatus!=ApptStatus.UnschedList && _appointment.AptStatus==ApptStatus.UnschedList) {
				//Extra log entry if the appt was sent to the unscheduled list
				Permissions permissions=Permissions.AppointmentMove;
				if(_appointmentOld.AptStatus==ApptStatus.Complete) {
					permissions=Permissions.AppointmentCompleteEdit;
				}
				SecurityLogs.MakeLogEntry(permissions,_appointment.PatNum,_appointment.ProcDescript+", "+_appointment.AptDateTime.ToString()
					+", Sent to Unscheduled List",_appointment.AptNum,datePrevious);
			}
			#region Validate Apt Start and End
			int minutes=contrApptProvSlider.Pattern.Length*5;
			//compare beginning of new appointment against end to see if they fall on different days
			if(_appointment.AptDateTime.Day!=_appointment.AptDateTime.AddMinutes(minutes).Day) {
				MsgBox.Show(this,"You cannot have an appointment that starts and ends on different days.");
				return;
			}
			#endregion
			if(!UpdateListAndDB(isClosing: true, doCreateSecLog: true, doInsertHL7: true)) {
				return;
			}
			AppointmentL.ShowKioskManagerIfNeeded(_appointmentOld,_appointment.Confirmed);
			Plugins.HookAddCode(this,"FormApptEdit.butOK_Click_end",_appointment,_appointmentOld,_patient);
			DialogResult=DialogResult.OK;
			if(!this.Modal) {
				Close();
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			if(!this.Modal) {
				Close();
			}
		}

		private void FormApptEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_appointment==null) {//Could not find _appointment in the Db on load.
				return;
			}
			//Do not use pat.PatNum here.  Use _appointment.PatNum instead.  Pat will be null in the case that the user does not have the appt create permission.
			DateTime datePrevious=_appointment.DateTStamp;
			if(DialogResult!=DialogResult.OK) {
				if(_appointment.AptStatus==ApptStatus.Complete) {
					//This is a completed appointment and we need to warn the user if they are trying to leave the window and need to detach procs first.
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						bool attached=false;
						if(_appointment.AptStatus==ApptStatus.Planned && ((Procedure)gridProc.ListGridRows[i].Tag).PlannedAptNum==_appointment.AptNum) {
							attached=true;
						}
						else if(((Procedure)gridProc.ListGridRows[i].Tag).AptNum==_appointment.AptNum) {
							attached=true;
						}
						if(((Procedure)gridProc.ListGridRows[i].Tag).ProcStatus!=ProcStat.TP || !attached) {
							continue;
						}
						if(!Security.IsAuthorized(Permissions.AppointmentCompleteEdit,suppressMessage: true)) {
							continue;
						}
						MsgBox.Show(this,"Detach treatment planned procedures or click OK in the appointment edit window to set them complete.");
						e.Cancel=true;
						return;
					}
				}
				if(IsNew) {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_appointment.PatNum,
						"Create cancel for date/time: "+_appointment.AptDateTime.ToString(),
						_appointment.AptNum,datePrevious);
					//If cancel was pressed we want to un-do any changes to other appointments that were done.
					_listAppointments=Appointments.GetAppointmentsForProcs(_listProceduresForAppointment);
					//Add the current appointment if it is not in this list so it can get properly deleted by the sync later.
					if(!_listAppointments.Exists(x => x.AptNum==_appointment.AptNum)) {
						_listAppointments.Add(_appointment);
					}
					//We need to add this current appointment to the list of old appointments so we run the Appointments.Delete fucntion on it
					//This will remove any procedure connections that we created while in this window.
					_listAppointmentsOld=_listAppointments.Select(x => x.Copy()).ToList();
					//Now we also have to remove the appointment that was pre-inserted and is in this list as well so it is deleted on sync.
					_listAppointments.RemoveAll(x => x.AptNum==_appointment.AptNum);
				}
				else {  //User clicked cancel (or X button) on an existing appt
					_appointment=_appointmentOld.Copy();  //We do not want to save any other changes made in this form.
					//Setting _appointment to a copy of _appointmentOld causes the _appointment reference in _listAppointments to be lost. Remove and add back in so the sync below does not make 
					//any changes to _appointment. We had an issue with changes to _appointment were happening outside of the OK_Click method.
					_listAppointments.RemoveAll(x => x.AptNum==_appointment.AptNum);
					_listAppointments.Add(_appointment);
					if(_appointment.AptStatus==ApptStatus.Scheduled && PrefC.GetBool(PrefName.InsChecksFrequency) && !CheckFrequencies()) {
						e.Cancel=true;
						return;
					}
				}
			}
			else {//DialogResult==DialogResult.OK (User clicked OK or Delete)
				//Note that Procedures.Sync is never used.  This is intentional.  In order to properly use procedure.Sync logic in this form we would
				//need to enhance ProcEdit and all its possible child forms to also not insert into DB until OK is clicked.  This would be a massive undertaking
				//and as such we just immediately push changes to DB.
				if(_appointment.AptStatus==ApptStatus.Scheduled && !_isDeleted && PrefC.GetBool(PrefName.InsChecksFrequency) && !CheckFrequencies()) {
					e.Cancel=true;
					return;
				}
				if(_appointment.AptStatus==ApptStatus.Scheduled) {
					//find all procs that are currently attached to the appt that weren't when the form opened
					List<string> listProcureCodes = _listProceduresForAppointment.FindAll(x => x.AptNum==_appointment.AptNum && !_listProcNumsAttachedStart.Contains(x.ProcNum))
						.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).Distinct().ToList();//get list of string proc codes
					AutomationL.Trigger(AutomationTrigger.ScheduleProcedure,listProcureCodes,_appointment.PatNum);
				}
			}
			if(_appointmentOld.AptStatus!=ApptStatus.Complete && _appointment.AptStatus==ApptStatus.Complete) {
				//If necessary, prompt the user to ask the patient to opt in to using Short Codes.
				FormShortCodeOptIn.PromptIfNecessary(_patient,_appointment.ClinicNum);
			}
			//Sync detaches any attached procedures within Appointments.Delete() but doesn't create any ApptComm items.
			if(Appointments.Sync(_listAppointments,_listAppointmentsOld,_appointment.PatNum)) {
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,_appointment);
			}
			//Synch the recalls for this patient.  This is necessary in case the date of the appointment has change or has been deleted entirely.
			Recalls.Synch(_appointment.PatNum);
			Recalls.SynchScheduledApptFull(_appointment.PatNum);
		}
	

	}
}