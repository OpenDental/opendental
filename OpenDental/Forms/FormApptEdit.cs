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
using System.Threading;
using System.Globalization;

namespace OpenDental{
	///<summary> AptCur.AptNum cannot be trusted fully inside of this form. This form can create new appointments without inserting them into the DB.
	///Due to this, make sure you consider new appointments and handle accordingly. See _isInsertRequired.
	///Edit window for appointments.  Will have a DialogResult of Cancel if the appointment was marked as new and is deleted.</summary>
	public partial class FormApptEdit : FormODBase {
		public bool PinIsVisible;
		public bool PinClicked;
		public bool IsNew;
		private Appointment AptCur;
		private Appointment AptOld;
		private List <InsPlan> PlanList;
		private List<InsSub> SubList;
		private Patient pat;
		private Family fam;
		///<summary>This is the way to pass a "signal" up to the parent form that OD is to close.</summary>
		public bool CloseOD;
		///<summary>True if appt was double clicked on from the chart module gridProg.  Currently only used to trigger an appointment overlap check.</summary>
		public bool IsInChartModule;
		///<summary>True if appt was double clicked on from the ApptsOther form.  Currently only used to trigger an appointment overlap check.</summary>
		public bool IsInViewPatAppts;
		///<summary>Matches list of appointments in comboAppointmentType. Does not include hidden types unless current appointment is of that type.</summary>
		private List<AppointmentType> _listAppointmentType;
		///<summary>Procedure were attached/detached from appt and the user clicked cancel or closed the form.  Used in ApptModule to tell if we need to refresh.</summary>
		public bool HasProcsChangedAndCancel;
		///<summary>Lab for the current appointment.  It may be null if there is no lab.</summary>
		private LabCase _labCur;
		///<summary>A list of all appointments that are associated to any procedures in the Procedures on this Appointment grid.</summary>
		private List<Appointment> _listAppointments;
		///<summary>Stale deep copy of _listAppointments to use with sync.</summary>
		private List<Appointment> _listAppointmentsOld;
		private bool _isPlanned;
		private DataTable _tableFields;
		private DataTable _tableComms;
		///<summary>All ProcNums attached to the appt when form opened.</summary>
		private List<long> _listProcNumsAttachedStart=new List<long>();
		///<summary>All ProcNums intended to be selected on load, but without altering any procedure properties.</summary>
		private List<long> _listPreSelectedProcNums;
		///<summary>Used when first loading the form to skip calling fill methods multiple times.</summary>
		private bool _isOnLoad;
		///<summary>List of all procedures that show within the Procedures on this Appointment grid.  Filled on load.  Used to double check that we update other appointments that we could steal procedures from (e.g. planned appts with tp procs).</summary>
		private List<Procedure> _listProcsForAppt;
		///<summary>The selected appointment type when this form loads.</summary>
		private AppointmentType _selectedAptType;
		///<summary>The exact index of the selected item in comboApptType.</summary>
		private int _aptTypeIndex;
		private List<PatPlan> _listPatPlans;
		List<Benefit> _benefitList;
		private bool _isDeleted;
		private bool _isClickLocked;
		///<summary>When AptCur.Status is not Planned, PtNote or PtNoteCompleted this is set to the index of Broken in comobStatus, otherwise -1.</summary>
		private int _indexStatusBroken=-1;
		///<summary>Used when FormApptBreak is required to track what the user has selected.</summary>
		private ApptBreakSelection _formApptBreakSelection=ApptBreakSelection.None;
		private ProcedureCode _procCodeBroken=null;
		private List<Employee> _listEmployees;
		///<summary>eCW Tight or Full enabled and a DFT msg for this appt has already been sent.  The 'Finish &amp; Send' button will say 'Revise'</summary>
		private bool _isEcwHL7Sent=false;
		///<summary>If no aptNum was passed into this form, this boolean will be set to true to indicate that AptCur.AptNum cannot be trusted until after the insert occurs.
		///Someday we should consider using the IsNew flag instead after we remove all of the appointment pre-insert logic.</summary>
		private bool _isInsertRequired=false;
		private List<Def> _listRecallUnschedStatusDefs;
		private List<Def> _listApptConfirmedDefs;
		private List<Def> _listApptProcsQuickAddDefs;
		///<summary>A list of all ClaimProcs that are related to the patient's current procedures</summary>
		private List<ClaimProc> _listClaimProcs;
		///<summary>A list of all Adjustments that are related to the patient's current procedures</summary>
		private List<Adjustment> _listAdjustments;
		///<summary>The data necesary to load the form.</summary>
		private ApptEdit.LoadData _loadData;
		///<summary>Indicates this appointment has been opened from the Unscheduled list.</summary>
		private bool _isSchedulingUnscheduledAppt;

		#region Properties
	 ///<summary>Indicates whether _selectedApptStatus is a patient note</summary>
		private bool _isPtNote {
			get {
				return ListTools.In(_selectedApptStatus,ApptStatus.PtNote,ApptStatus.PtNoteCompleted);
			}
		}

		///<summary>The currently selected ApptStatus.</summary>
		private ApptStatus _selectedApptStatus {
			get {
				if(AptCur.AptStatus==ApptStatus.Planned) {
					return AptCur.AptStatus;
				}
				else if(comboStatus.SelectedIndex==-1) {
					return ApptStatus.Scheduled;
				}
				else if(AptCur.AptStatus==ApptStatus.PtNote || AptCur.AptStatus==ApptStatus.PtNoteCompleted) {
					return (ApptStatus)comboStatus.SelectedIndex + 7;
				}
				else if(comboStatus.SelectedIndex==3) {//Broken
					return ApptStatus.Broken;
				}
				else {//Scheduled, Complete, Unscheduled
					return (ApptStatus)comboStatus.SelectedIndex+1;
				}
			}
		}

		///<summary>Indicates the Appointment is being opened from the unscheduled list.</summary>
		public bool IsSchedulingUnscheduledAppt
		{
			get {
				return _isSchedulingUnscheduledAppt;
			}
			set {
				_isSchedulingUnscheduledAppt=value;
			}
		}

		#endregion Properties

		///<summary>When aptNum is 0, make sure to set a valid patNum because a new appointment will be created/inserted on OK click.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///listPreSelectedProcNums is used to preselect procs in the grid without pre-altering the procs properties, such as AptNum/PlannedAptNum</summary>
		public FormApptEdit(long aptNum,long patNum=0,bool useApptDrawingSettings=false,Patient patient=null,List<long> listPreSelectedProcNums=null,DateTime? dateTNew=null,long? opNumNew=null) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isClickLocked=true;
			if(aptNum==0) {//Creating a new appointment
				_isInsertRequired=true;
				Patient pat=patient??Patients.GetPat(patNum);
				if(pat==null) {
					MsgBox.Show(this,"Invalid patient passed in.  Please call support or try again.");
					DialogResult=DialogResult.Cancel;
					if(!this.Modal) {
						Close();
					}
					return;
				}
				//not really needed, but makes it more obvious that these two parameters are not null
				AptCur=AppointmentL.MakeNewAppointment(pat,useApptDrawingSettings,dateTNew,opNumNew);
			}
			else {
				AptCur=Appointments.GetOneApt(aptNum);//We need this query to get the PatNum for the appointment.
			}
			_listPreSelectedProcNums=listPreSelectedProcNums;
			this.contrApptProvSlider.FormApptEdit_CheckTimeLocked=checkTimeLocked;
		}

		public Appointment GetAppointmentCur() {
			return AptCur.Copy();
		}

		public Appointment GetAppointmentOld() {
			return AptOld.Copy();
		}

		private void FormApptEdit_Load(object sender, System.EventArgs e) {
			if(AptCur==null) {//Can happen if appointment was deleted by another WS.
				MsgBox.Show(this,"Appointment no longer exists.");
				DialogResult=DialogResult.Cancel;
				if(!this.Modal) {
					Close();
				}
				return;
			}
			_selectedAptType=null;
			_aptTypeIndex=0;
			if(PrefC.GetBool(PrefName.AppointmentTypeShowPrompt) && IsNew
				&& !ListTools.In(AptCur.AptStatus,ApptStatus.PtNote,ApptStatus.PtNoteCompleted))
			{
				using FormApptTypes FormAT=new FormApptTypes();
				FormAT.IsSelectionMode=true;
				FormAT.IsNoneAllowed=true;
				FormAT.ShowDialog();
				if(FormAT.DialogResult==DialogResult.OK) {
					_selectedAptType=FormAT.SelectedAptType;
				}
			}
			_isOnLoad=true;
			new ODThread((o) => {
				//Sleep for the delay and then set the variable to false.
				Thread.Sleep(Math.Max((int)(TimeSpan.FromSeconds(PrefC.GetDouble(PrefName.FormClickDelay,doUseEnUSFormat:true)).TotalMilliseconds),1));
				_isClickLocked=false;
			}).Start();
			_loadData=ApptEdit.GetLoadData(AptCur,IsNew);
			_listProcsForAppt=_loadData.ListProcsForAppt;
			_listAppointments=_loadData.ListAppointments;
			if(_listAppointments.Find(x => x.AptNum==AptCur.AptNum)==null) {
				_listAppointments.Add(AptCur);//Add AptCur if there are no procs attached to it.
			}
			_listAppointmentsOld=_listAppointments.Select(x => x.Copy()).ToList();
			for(int i=0;i<_listAppointments.Count;i++) {
				if(_listAppointments[i].AptNum==AptCur.AptNum) {
					AptCur=_listAppointments[i];//Changing the variable pointer so all changes are done on the element in the list.
				}
			}
			AptOld=AptCur.Copy();
			if(IsNew){
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
				if (AptCur.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentEdit)
					|| (AptCur.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentCompleteEdit))) 
				{//completed apts have their own perm.
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
			if(!Security.IsAuthorized(Permissions.ApptConfirmStatusEdit,true)) {//Suppress message because it would be very annoying to users.
				comboConfirmed.Enabled=false;
			}
			else if(IsSchedulingUnscheduledAppt) {//User is authorized for Permissions.ApptConfirmStatusEdit.
				//Causes the confirmation status to be reset in the UI, mimics ContrAppt.pinBoard_MouseUp(...)
				AptCur.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
			}
			//The objects below are needed when adding procs to this appt.
			fam=_loadData.Family;
			pat=fam.GetPatient(AptCur.PatNum);
			_listPatPlans=_loadData.ListPatPlans;
			_benefitList=_loadData.ListBenefits;
			SubList=_loadData.ListInsSubs;
			PlanList=_loadData.ListInsPlans;
			if(!PatPlans.IsPatPlanListValid(_listPatPlans,listInsSubs:SubList)) {
				_listPatPlans=PatPlans.Refresh(AptCur.PatNum);
				SubList=InsSubs.RefreshForFam(fam);
				PlanList=InsPlans.RefreshForSubList(SubList);
			}
			_tableFields=_loadData.TableApptFields;
			_tableComms=_loadData.TableComms;
			_listAdjustments=_loadData.ListAdjustments;
			_listClaimProcs=_loadData.ListClaimProcs;
			_labCur=_loadData.Lab;
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				butRequirement.Visible=false;
				textRequirement.Visible=false;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				butSyndromicObservations.Visible=true;
				labelSyndromicObservations.Visible=true;
			}
			if(!PinIsVisible){
				butPin.Visible=false;
			}
			string titleText = this.Text;
			_isPlanned=false;
			if(AptCur.AptStatus==ApptStatus.Planned) {
				_isPlanned=true;
				titleText=Lan.g(this,"Edit Planned Appointment")+" - "+pat.GetNameFL();
				labelStatus.Visible=false;
				comboStatus.Visible=false;
				butDelete.Visible=false;
				if(_listAppointments.FindAll(x => x.NextAptNum==AptCur.AptNum)//This planned appt is attached to a completed appt.
					.Exists(x => x.AptStatus==ApptStatus.Complete)) 
				{
					labelPlannedComplete.Visible=true;
				}
			}
			else if(AptCur.AptStatus==ApptStatus.PtNote) {
				labelApptNote.Text="Patient NOTE:";
				titleText=Lan.g(this,"Edit Patient Note")+" - "+pat.GetNameFL()+" on "+AptCur.AptDateTime.DayOfWeek+", "+AptCur.AptDateTime;
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
			else if(AptCur.AptStatus==ApptStatus.PtNoteCompleted) {
				labelApptNote.Text="Completed Patient NOTE:";
				titleText=Lan.g(this,"Edit Completed Patient Note")+" - "+pat.GetNameFL()+" on "+AptCur.AptDateTime.DayOfWeek+", "+AptCur.AptDateTime;
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
				titleText=Lan.g(this, "Edit Appointment")+" - "+pat.GetNameFL()+" on "+AptCur.AptDateTime.DayOfWeek+", "+AptCur.AptDateTime;
				comboStatus.Items.Add(Lan.g("enumApptStatus","Scheduled"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","Complete"));
				comboStatus.Items.Add(Lan.g("enumApptStatus","UnschedList"));
				_indexStatusBroken=comboStatus.Items.Add(Lan.g("enumApptStatus","Broken"));
			}
			SetAptCurComboStatusSelection();
			if(AptCur.Op != 0) {
				titleText+=" | "+Operatories.GetAbbrev(AptCur.Op);
			}
			this.Text = titleText;
			contrApptProvSlider.ProvBarText=AptCur.ProvBarText;			
			checkASAP.Checked=AptCur.Priority==ApptPriority.ASAP;
			if(AptCur.AptStatus==ApptStatus.UnschedList) {
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
			_listRecallUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			_listApptConfirmedDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			_listApptProcsQuickAddDefs=Defs.GetDefsForCategory(DefCat.ApptProcsQuickAdd,true);
			for(int i=0;i<_listRecallUnschedStatusDefs.Count;i++) {
				comboUnschedStatus.Items.Add(_listRecallUnschedStatusDefs[i].ItemName);
				if(_listRecallUnschedStatusDefs[i].DefNum==AptCur.UnschedStatus)
					comboUnschedStatus.SelectedIndex=i+1;
			}
			for(int i=0;i<_listApptConfirmedDefs.Count;i++) {
				comboConfirmed.Items.Add(_listApptConfirmedDefs[i].ItemName);
				if(_listApptConfirmedDefs[i].DefNum==AptCur.Confirmed) {
					comboConfirmed.SelectedIndex=i;
				}
			}
			checkTimeLocked.Checked=AptCur.TimeLocked;
			textNote.Text=AptCur.Note;
			for(int i=0;i<_listApptProcsQuickAddDefs.Count;i++) {
				listQuickAdd.Items.Add(_listApptProcsQuickAddDefs[i].ItemName);
			}
			comboClinic.SelectedClinicNum=AptCur.ClinicNum;
			FillCombosProv();
			comboProv.SetSelectedProvNum(AptCur.ProvNum);
			comboProvHyg.SetSelectedProvNum(AptCur.ProvHyg);//ok if 0
			checkIsHygiene.Checked=AptCur.IsHygiene;
			//Fill comboAssistant with employees and none option
			comboAssistant.Items.Add(Lan.g(this,"none"));
			comboAssistant.SelectedIndex=0;
			_listEmployees=Employees.GetDeepCopy(true);
			for(int i=0;i<_listEmployees.Count;i++) {
				comboAssistant.Items.Add(_listEmployees[i].FName);
				if(_listEmployees[i].EmployeeNum==AptCur.Assistant)
					comboAssistant.SelectedIndex=i+1;
			}
			textLabCase.Text=GetLabCaseDescript();
			textTimeArrived.ContextMenu=contextMenuTimeArrived;
			textTimeSeated.ContextMenu=contextMenuTimeSeated;
			textTimeDismissed.ContextMenu=contextMenuTimeDismissed;
			if(AptCur.DateTimeAskedToArrive.TimeOfDay>TimeSpan.FromHours(0)) {
				textTimeAskedToArrive.Text=AptCur.DateTimeAskedToArrive.ToShortTimeString();
			}
			if(AptCur.DateTimeArrived.TimeOfDay>TimeSpan.FromHours(0)){
				textTimeArrived.Text=AptCur.DateTimeArrived.ToShortTimeString();
			}
			if(AptCur.DateTimeSeated.TimeOfDay>TimeSpan.FromHours(0)){
				textTimeSeated.Text=AptCur.DateTimeSeated.ToShortTimeString();
			}
			if(AptCur.DateTimeDismissed.TimeOfDay>TimeSpan.FromHours(0)){
				textTimeDismissed.Text=AptCur.DateTimeDismissed.ToShortTimeString();
			}
			if(AptCur.AptStatus==ApptStatus.Complete
				|| AptCur.AptStatus==ApptStatus.Broken
				|| AptCur.AptStatus==ApptStatus.PtNote
				|| AptCur.AptStatus==ApptStatus.PtNoteCompleted) 
			{
				textInsPlan1.Text=InsPlans.GetCarrierName(AptCur.InsPlan1,PlanList);
				textInsPlan2.Text=InsPlans.GetCarrierName(AptCur.InsPlan2,PlanList);
			}
			else {//Get the current ins plans for the patient.
				butInsPlan1.Enabled=false;
				butInsPlan2.Enabled=false;
				InsSub sub1=InsSubs.GetSub(PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,_listPatPlans,PlanList,SubList)),SubList);
				InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,_listPatPlans,PlanList,SubList)),SubList);
				AptCur.InsPlan1=sub1.PlanNum;
				AptCur.InsPlan2=sub2.PlanNum;
				textInsPlan1.Text=InsPlans.GetCarrierName(AptCur.InsPlan1,PlanList);
				textInsPlan2.Text=InsPlans.GetCarrierName(AptCur.InsPlan2,PlanList);
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				List<ReqStudent> listStudents=_loadData.ListStudents;
				string requirements="";
				for(int i=0;i<listStudents.Count;i++) {
					if(i > 0) {
						requirements+="\r\n";
					}
					Provider student=Providers.GetDeepCopy().First(x => x.ProvNum==listStudents[i].ProvNum);
					requirements+=student.LName+", "+student.FName+": "+listStudents[i].Descript;
				}
				textRequirement.Text=requirements;
			}
			//IsNewPatient is set well before opening this form.
			checkIsNewPatient.Checked=AptCur.IsNewPatient;
			butColor.BackColor=AptCur.ColorOverride;
			contrApptProvSlider.MinPerIncr=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			if(Programs.UsingEcwTightOrFullMode() && !_isInsertRequired) {
				//These buttons are ONLY for eCW, not any other HL7 interface.
				butComplete.Visible=true;
				butPDF.Visible=true;
				//for eCW, we need to hide some things--------------------
				if(Bridges.ECW.AptNum==AptCur.AptNum) {
					butDelete.Visible=false;
				}
				butPin.Visible=false;
				butTask.Visible=false;
				butAddComm.Visible=false;
				if(HL7Msgs.MessageWasSent(AptCur.AptNum)) {
					_isEcwHL7Sent=true;
					butComplete.Text="Revise";
					//if(!Security.IsAuthorized(Permissions.Setup,true)) {
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
					if(Bridges.ECW.AptNum != AptCur.AptNum) {
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
			if(pat.WirelessPhone=="" || (!Programs.IsEnabled(ProgramName.CallFire) && !SmsPhones.IsIntegratedTextingEnabled())) {
				butText.Enabled=false;
			}
			else {//Pat has a wireless phone number and CallFire is enabled
				butText.Enabled=true;//TxtMsgOk checking performed on button click.
			}
			//AppointmentType
			_listAppointmentType=AppointmentTypes.GetWhere(x => !x.IsHidden || x.AppointmentTypeNum==AptCur.AppointmentTypeNum);
			comboApptType.Items.Add(Lan.g(this,"None"));
			comboApptType.SelectedIndex=0;
			foreach(AppointmentType aptType in _listAppointmentType) {
				comboApptType.Items.Add(aptType.AppointmentTypeName);
			}
			int selectedIndex=-1;
			if(IsNew && _selectedAptType!=null) { //selectedAptType will be null if they didn't select anything.
				selectedIndex=_listAppointmentType.FindIndex(x => x.AppointmentTypeNum==_selectedAptType.AppointmentTypeNum);
			}
			else {
				selectedIndex=_listAppointmentType.FindIndex(x => x.AppointmentTypeNum==AptCur.AppointmentTypeNum);
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
				AptCur.Pattern=Appointments.GetApptTimePatternForNoProcs();
			}
			contrApptProvSlider.Pattern=AptCur.Pattern;
			contrApptProvSlider.PatternSecondary=AptCur.PatternSecondary;
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
			Plugins.HookAddCode(this,"FormApptEdit.Load_End",pat,butText);
			Plugins.HookAddCode(this,"FormApptEdit.Load_end2",AptCur);//Lower casing the code area (_end) is the newer pattern for this.
			Plugins.HookAddCode(this,"FormApptEdit.Load_end3",AptCur,pat);
		}

		///<summary>Sets comboStatus based on AptCur.AptStatus.
		///AptCur.AptStatus is not updated with UI selection until after UpdateListAndDB(...) is called.</summary>
		private void SetAptCurComboStatusSelection() {
			switch(AptCur.AptStatus) {
				case ApptStatus.PtNote:
				case ApptStatus.PtNoteCompleted:
					//Only Patient Note and Completed Pt. Note are options in comboStatus.
					//Subtract 7 to get either 0 (PtNote) or 1(PtNoteCompleted) for these options.
					comboStatus.SelectedIndex=(int)AptCur.AptStatus-7;
					break;
				case ApptStatus.Broken:
					comboStatus.SelectedIndex=_indexStatusBroken;
					break;
				case ApptStatus.Planned:
					//Intentionally empty, comboStatus is not visable.
					break;
				default://Scheduled, Completed, Unscheduled (When Planned comboStatus is not visable).
					comboStatus.SelectedIndex=(int)AptCur.AptStatus-1;
					break;
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillCombosProv();
			FillProcedures();
		}

		private void butPickDentist_Click(object sender,EventArgs e) {
			using FormProviderPick formp=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formp.SelectedProvNum=comboProv.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formp.SelectedProvNum);
			SetTimeSliderColors();
		}

		private void butPickHyg_Click(object sender,EventArgs e) {
			using FormProviderPick formp=new FormProviderPick(comboProvHyg.Items.GetAll<Provider>());//none option will show.
			formp.SelectedProvNum=comboProvHyg.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvHyg.SetSelectedProvNum(formp.SelectedProvNum);
			SetTimeSliderColors();
		}

		private void comboProv_SelectionChangeCommitted(object sender, EventArgs e){
			SetTimeSliderColors();
		}

		private void comboProvHyg_SelectionChangeCommitted(object sender, EventArgs e){
			SetTimeSliderColors();
		}

		private void checkIsHygiene_Click(object sender, EventArgs e){
			SetTimeSliderColors();
		}

		///<summary>Uses the UserODPref to store ShowAutomatedCommlog separately from the chart module.</summary>
		private void checkShowCommAuto_Click(object sender,EventArgs e) {
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog).FirstOrDefault();
			if(userOdPrefShowAutoCommlog==null) {
				userOdPrefShowAutoCommlog=new UserOdPref();
				userOdPrefShowAutoCommlog.UserNum=Security.CurUser.UserNum;
				userOdPrefShowAutoCommlog.FkeyType=UserOdFkeyType.ShowAutomatedCommlog;
				userOdPrefShowAutoCommlog.Fkey=0;
			}
			userOdPrefShowAutoCommlog.ValueString=POut.Bool(checkShowCommAuto.Checked);
			UserOdPrefs.Upsert(userOdPrefShowAutoCommlog);
			//refresh the data
			FillComm();
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

		private void butColor_Click(object sender,EventArgs e) {
			ColorDialog colorDialog1=new ColorDialog();
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
		}

		private void butColorClear_Click(object sender,EventArgs e) {
			butColor.BackColor=System.Drawing.Color.FromArgb(0);
		}

		private void FillPatient(){
			DataTable table=_loadData.PatientTable;
			gridPatient.BeginUpdate();
			gridPatient.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",120);//Add 2 blank columns
			gridPatient.ListGridColumns.Add(col);
			col=new GridColumn("",120);
			gridPatient.ListGridColumns.Add(col);
			gridPatient.ListGridRows.Clear();
			GridRow row;
			for(int i=1;i<table.Rows.Count;i++) {//starts with 1 to skip name
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

		///<summary>Calculates the fee for this appointment using the highlighted procedures in the procedure list.</summary>
		private void CalcPatientFeeThisAppt() {
			double feeThisAppt=0;
			for(int i=0;i<gridProc.SelectedIndices.Length;i++) {
				feeThisAppt+=((Procedure)(gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag)).ProcFeeTotal;
			}
			gridPatient.ListGridRows[gridPatient.ListGridRows.Count-1].Cells[1].Text=POut.Double(feeThisAppt);
			gridPatient.Invalidate();
		}

		///<summary>Fully refreshes the data and then calculate the estimated patient portion</summary>
		private void RefreshEstPatientPortion() {
			_listClaimProcs=ClaimProcs.RefreshForProcs(_listProcsForAppt.Select(x => x.ProcNum).ToList());
			_listAdjustments=Adjustments.GetForProcs(_listProcsForAppt.Select(x => x.ProcNum).ToList());
			CalcEstPatientPortion();
		}

		///<summary>Calculates the estimated patient portion to insert into the grid</summary>
		private void CalcEstPatientPortion() {
			List<Procedure> listSelectedProcedures=gridProc.SelectedTags<Procedure>();
			decimal totalEstPatientPortion=0;
			foreach(Procedure proc in listSelectedProcedures) {
				totalEstPatientPortion+=ClaimProcs.GetPatPortion(proc,_listClaimProcs,_listAdjustments);
			}
			GridRow row=gridPatient.ListGridRows.ToList().Find(x => x.Cells[0].Text==Lans.g("FormApptEdit","Est. Patient Portion"));
			if(row==null) {
				return;//Probably some weird translation issue
			}
			row.Cells[1].Text=totalEstPatientPortion.ToString("F");
		}

		private void FillFields() {
			gridFields.BeginUpdate();
			gridFields.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",100);
			gridFields.ListGridColumns.Add(col);
			col=new GridColumn("",100);
			gridFields.ListGridColumns.Add(col);
			gridFields.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableFields.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableFields.Rows[i]["FieldName"].ToString());
				row.Cells.Add(_tableFields.Rows[i]["FieldValue"].ToString());
				gridFields.ListGridRows.Add(row);
			}
			gridFields.EndUpdate();
		}

		private void FillComm(){
			gridComm.BeginUpdate();
			gridComm.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCommLog","DateTime"),80);
			gridComm.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCommLog","Description"),80);
			gridComm.ListGridColumns.Add(col);
			gridComm.ListGridRows.Clear();
			GridRow row;
			List<Def> listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
			List<Def> listCommLogTypeDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes);
			bool isCommlogAutomated;
			for(int i=0;i<_tableComms.Rows.Count;i++) {
				long commTypeDefNum=PIn.Long(_tableComms.Rows[i]["CommType"].ToString());
				Def defCur=Defs.GetDef(DefCat.CommLogTypes,commTypeDefNum,listCommLogTypeDefs);
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
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.CommlogApptRelated].ItemColor;
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
				using FormCommItem FormCI=new FormCommItem(item);
				FormCI.ShowDialog();
			}
			else if (msgNum>0) {
				EmailMessage email=EmailMessages.GetOne(msgNum);
				if (email==null) {
					MsgBox.Show(this,"This e-mail has been deleted by another user.");
					return;
				}
				using FormEmailMessageEdit FormEME=new FormEmailMessageEdit(email,isDeleteAllowed:false);
				FormEME.ShowDialog();
			}
			_tableComms=Appointments.GetCommTable(AptCur.PatNum.ToString(),AptCur.AptNum);
			FillComm();
		}

		private void FillProcedures(){
			//Every time the procedures available have been manipulated (associated to appt, deleted, etc) we need to refresh the list from the db.
			//This has the potential to call the database a lot (cell click via a grid) but we accept this inefficiency for the benefit of concurrency.
			//If the following call to the db is to be removed, make sure that all procedure manipulations from FormProcEdit, FormClaimProcEdit, etc.
			//  handle the changes accordingly.  Changing this call to the database should not be done 'lightly'.  Heed our warning.
			List<Procedure> listProcs=_listProcsForAppt;//Gets a full list because GetProcsForApptEdit has its optional param set true in ApptEdit.cs
			ProcedureLogic.SortProcedures(ref listProcs);
			List<long> listNumsSelected=new List<long>();
			if(_isOnLoad && !_isInsertRequired) {//First time filling the grid and not a new appointment.
				if(_listPreSelectedProcNums!=null) {
					//Allows us to preselect procs without setting proc.PlannedAptNum in AppointmentL.CreatePlannedAppt(). Otherwise, downstream attach/detach
					//logic has problems if we preselect by setting AptNum/PlannedAptNum because that logic uses _listProcNumsAttachedStart to determine if
					//these procs were already attached to this appointment.
					listNumsSelected.AddRange(_listPreSelectedProcNums.FindAll(x => listProcs.Any(y => y.ProcNum==x)));
				}
				if(_isPlanned) {
					_listProcNumsAttachedStart=listProcs.FindAll(x => x.PlannedAptNum==AptCur.AptNum).Select(x => x.ProcNum).ToList();
				}
				else {//regular appointment
					//set ProcNums attached to the appt when form opened for use in automation on closing.
					_listProcNumsAttachedStart=listProcs.FindAll(x => x.AptNum==AptCur.AptNum).Select(x => x.ProcNum).ToList();
				}
				listNumsSelected.AddRange(_listProcNumsAttachedStart);
				if(Programs.UsingEcwTightOrFullMode() && !_isEcwHL7Sent) {//for eCW only and only if not in 'Revise' mode, select completed procs from _listProcsForAppt with ProcDate==AptDateTime
					//Attach procs to this appointment in memory only so that Cancel button still works.
					listNumsSelected.AddRange(listProcs.Where(x => x.ProcStatus==ProcStat.C && x.ProcDate.Date==AptCur.AptDateTime.Date).Select(x=>x.ProcNum));
				}
			}
			else {//Filling the grid later on.
				listNumsSelected.AddRange(gridProc.SelectedIndices.OfType<int>().Select(x => ((Procedure)gridProc.ListGridRows[x].Tag).ProcNum));
			}
			bool isMedical=Clinics.IsMedicalPracticeOrClinic(comboClinic.SelectedClinicNum);
			gridProc.BeginUpdate();
			gridProc.ListGridRows.Clear();
			gridProc.ListGridColumns.Clear();
			List<DisplayField> listAptDisplayFields;
			if(AptCur.AptStatus==ApptStatus.Planned){
				listAptDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PlannedAppointmentEdit);
			}
			else {
				listAptDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.AppointmentEdit);
			}
			foreach(DisplayField displayField in listAptDisplayFields) {
				if(isMedical && (displayField.InternalName=="Surf" || displayField.InternalName=="Tth")) {
					continue;
				}
				gridProc.ListGridColumns.Add(new GridColumn(displayField.InternalName,displayField.ColumnWidth));
			}
			if(listAptDisplayFields.Sum(x => x.ColumnWidth) > gridProc.Width) {
				gridProc.HScrollVisible=true;
			}
			GridRow row;
			foreach(Procedure proc in listProcs) {
				row=new GridRow();
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				foreach(DisplayField displayField in listAptDisplayFields) {
					switch (displayField.InternalName) {
						case "Stat":
							if(ProcMultiVisits.IsProcInProcess(proc.ProcNum)) {
								row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
							}
							else {
								row.Cells.Add(Lans.g("enumProcStat",proc.ProcStatus.ToString()));
							}
							break;
						case "Priority":
							row.Cells.Add(Defs.GetName(DefCat.TxPriorities,proc.Priority));
							break;
						case "Code":
								row.Cells.Add(procCode.ProcCode);
							break;
						case "Tth":
							if(isMedical) {
								continue;
							}
							row.Cells.Add(Tooth.GetToothLabel(proc.ToothNum));
							break;
						case "Surf":
							if(isMedical) {
								continue;
							}
							String displaySurf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);//Fixes surface display for Canadian users
							row.Cells.Add(displaySurf);
							break;
						case "Description":
							string descript="";
							if(proc.ProcNumLab!=0) {//Proc is a Canadian Lab.
								//This descript is gotten the same way it was in Appointments.GetProcTable()
								descript="^ ^ "+descript;//Visual indicator that this lab is linked to the procedure on the row above this row.
							}
							if(_isPlanned && proc.PlannedAptNum!=0 && proc.PlannedAptNum!=AptCur.AptNum) {
								descript+=Lan.g(this,"(other appt) ");
							}
							else if (_isPlanned && proc.AptNum!=0 && proc.AptNum!=AptCur.AptNum) {
								descript+=Lan.g(this,"(scheduled appt) ");
							}
							else if (!_isPlanned && proc.PlannedAptNum!=0 && proc.PlannedAptNum!=AptCur.AptNum) {
								descript+=Lan.g(this,"(planned appt) ");
							}
							else if(!_isPlanned && proc.AptNum!=0 && proc.AptNum!=AptCur.AptNum) {
								descript+=Lan.g(this,"(other appt) ");
							}
							if(procCode.LaymanTerm=="") {
								descript+=procCode.Descript;
							}
							else {
								descript+=procCode.LaymanTerm;
							}
							if(proc.ToothRange!="") {
								descript+=" #"+Tooth.FormatRangeForDisplay(proc.ToothRange);
							}
							row.Cells.Add(descript);
							break;
						case "Fee":
							row.Cells.Add(proc.ProcFeeTotal.ToString("F"));
							break;
						case "Abbreviation":
							row.Cells.Add(procCode.AbbrDesc);
							break;
						case "Layman's Term":
							row.Cells.Add(procCode.LaymanTerm);
							break;
					}
				}
				row.Tag=proc;
				gridProc.ListGridRows.Add(row);
			}
			gridProc.EndUpdate();
			for(int i=0;i<listProcs.Count;i++) {
				//Proc is selected, or is a Canadian Lab, and its parent is selected 
				//Selection logic to ensure the parent and children labs are selected together, this mimicks logic in ContrAccount.cs
				//See gridAccount_CellClick(...) toward the bottom.
				if(listNumsSelected.Contains(listProcs[i].ProcNum) || listNumsSelected.Contains(listProcs[i].ProcNumLab)) {
					gridProc.SetSelected(i,true);
				}
			}
		}
		
		private string GetLabCaseDescript() {
			StringBuilder descript=new StringBuilder();
			if(_labCur!=null) {
				Laboratory lab=Laboratories.GetOne(_labCur.LaboratoryNum);
				if(lab!=null) {	//Laboratory won't be set if the program closed in the middle of creating a new lab entry.
					descript.Append(lab.Description);
				}
				else {
					descript.Append(Lan.g(this,"ERROR retrieving laboratory."));
				}
				if(_labCur.DateTimeChecked.Year>1880) {//Logic from Appointments.cs lines 1818 to 1840
					descript.Append(", "+Lan.g(this,"Quality Checked"));
				}
				else {
					if(_labCur.DateTimeRecd.Year>1880) {
						descript.Append(", "+Lan.g(this,"Received"));
					}
					else {
						if(_labCur.DateTimeSent.Year>1880) {
							descript.Append(", "+Lan.g(this,"Sent"));
						}
						else {
							descript.Append(", "+Lan.g(this,"Not Sent"));
						}
						if(_labCur.DateTimeDue.Year>1880) {
							descript.Append(", "+Lan.g(this,"Due: ")+_labCur.DateTimeDue.ToString("ddd")+" "
								+_labCur.DateTimeDue.ToShortDateString()+" "
								+_labCur.DateTimeDue.ToShortTimeString()
							);
						}
					}
				}
			}
			return descript.ToString();
		}

		private void butAddComm_Click(object sender,EventArgs e) {
			Commlog commlogCur=new Commlog();
			commlogCur.IsNew=true;
			commlogCur.PatNum=AptCur.PatNum;
			commlogCur.CommDateTime=DateTime.Now;
			commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
			commlogCur.UserNum=Security.CurUser.UserNum;
			using FormCommItem FormCI=new FormCommItem(commlogCur);
			FormCI.ShowDialog();
			_tableComms=Appointments.GetCommTable(AptCur.PatNum.ToString(),AptCur.AptNum);
			FillComm();
		}

		private void butText_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormApptEdit.butText_Click_start",pat,AptCur,this)) {
				return;
			}
			bool updateTextYN=false;
			if(pat.TxtMsgOk==YN.No) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient is marked to not receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return;
				}
			}
			if(pat.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient might not want to receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return;
				}
			}
			if(updateTextYN) {
				Patient patOld=pat.Copy();
				pat.TxtMsgOk=YN.Yes;
				Patients.Update(pat,patOld);
			}
			string message=PatComm.BuildConfirmMessage(ContactMethod.TextMessage,pat,AptCur.DateTimeAskedToArrive,AptCur.AptDateTime);
			using FormTxtMsgEdit FormTME=new FormTxtMsgEdit();
			FormTME.PatNum=pat.PatNum;
			FormTME.WirelessPhone=pat.WirelessPhone;
			FormTME.Message=message;
			FormTME.TxtMsgOk=pat.TxtMsgOk;
			FormTME.ShowDialog();
		}

		///<summary>Will only invert the specified procedure in the grid, even if the procedure belongs to another appointment.</summary>
		private void InvertCurProcSelected(int index) {
			bool isSelected=gridProc.SelectedIndices.Contains(index);
			List <int> listIndicies=new List<int>();
			listIndicies.Add(index);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				Procedure procSelected=((Procedure)gridProc.ListGridRows[index].Tag);
				if(procSelected.ProcNumLab==0) {//Not a lab, but could be a parent to a lab.
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						Procedure proc=(Procedure)gridProc.ListGridRows[i].Tag;
						if(proc.ProcNumLab==procSelected.ProcNum) {//Is lab of selected procedure.
							listIndicies.Add(i);
						}
					}
				}
				else {//Is a lab.
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						Procedure proc=(Procedure)gridProc.ListGridRows[i].Tag;
						if(proc.ProcNum==procSelected.ProcNumLab) {//Parent of selected lab.
							listIndicies.Add(i);
						}
						else if(proc.ProcNumLab==procSelected.ProcNumLab && !listIndicies.Contains(i)) {
							listIndicies.Add(i);
						}
					}
				}
			}
			foreach(int selectIndex in listIndicies) {
				gridProc.SetSelected(selectIndex,!isSelected);//Invert selection.
			}
		}

		private void gridProc_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isClickLocked) {
				return;
			}
			toolTip1.RemoveAll();
			Procedure selectedProc=((Procedure)gridProc.ListGridRows[e.Row].Tag);
			if(DisableDetachingOfCompletedProcFromCompletedAppt(selectedProc,AptCur,out string msg)){
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
			Procedure selectedProc=((Procedure)gridProc.ListGridRows[e.Row].Tag);
			//Only invert the procedure if we didn't block the original row inversino in gridProc_CellClick(...)
			if(!DisableDetachingOfCompletedProcFromCompletedAppt(selectedProc,AptCur,out string msg)){
				InvertCurProcSelected(e.Row);
			}
			//This will put the selection back to what is was before the single click event.
			//Get fresh copy from DB so we are not editing a stale procedure
			//If this is to be changed, make sure that this window is registering for procedure changes via signals or by some other means.
			Procedure proc=Procedures.GetOneProc(((Procedure)gridProc.ListGridRows[e.Row].Tag).ProcNum,true);
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(pat.PatNum);
			List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){pat.PatNum});
			for(int i=0;i<listProcedures.Count;i++) {
				if(listProcedures[i].ProcNum==proc.ProcNum) {
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
			}
			using FormProcEdit FormP=new FormProcEdit(proc,pat,fam);
			FormP.ListClaimProcHists=_loadData.ListClaimProcHists;
			FormP.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				CalculatePatternFromProcs();
				//SetTimeSliderColors();
				return;
			}
			_listProcsForAppt=Procedures.GetProcsForApptEdit(AptCur);//We need to refresh in case the user changed the ProcCode or set the proc complete.
			//The next 3 lines are a duplicate of a section in butDeleteProc to handle deleted procedures.
			Appointments.SetProcDescript(AptCur,_listProcsForAppt);
			AptOld.ProcDescript=AptCur.ProcDescript;
			AptOld.ProcsColored=AptCur.ProcsColored;
			FillProcedures();
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			RefreshEstPatientPortion();//Need to refresh in case the user changed the ProcCode or set the proc complete.
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

		private void gridProc_MouseLeave(object sender,EventArgs e) {
			toolTip1.RemoveAll();
		}

		private void butDeleteProc_Click(object sender,EventArgs e) {
			//this button will not be enabled if user does not have permission for AppointmentEdit
			if(gridProc.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select one or more procedures first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Permanently delete all selected procedure(s)?")){
				return;
			}
			int skipped=0;
			int skippedSecurity=0;
			int skippedLinkedToOrthoCase=0;
			int skippedPreauth=0;
			bool isProcDeleted=false;
			OrthoProcLink orthoProcLink;
			Dictionary<long,OrthoProcLink> dictOrthoProcLinks=
				OrthoProcLinks.GetManyForProcs(gridProc.ListGridRows.Select(x => ((Procedure)x.Tag).ProcNum).ToList()).ToDictionary(y => y.ProcNum,y => y);
			List<long> listSelectedProcNums=gridProc.SelectedTags<Procedure>().Select(x => x.ProcNum).ToList();
			List<ClaimProc> listClaimProcsForProc=ClaimProcs.GetForProcs(listSelectedProcNums,new List<ClaimProcStatus>(){ClaimProcStatus.Preauth}).Where(x => x.ClaimNum!=0).ToList();
			foreach(ClaimProc claimProc in listClaimProcsForProc.DistinctBy(x=>x.ClaimNum)) {
				List<long> listProcNumsForClaim=ClaimProcs.RefreshForClaim(claimProc.ClaimNum).Select(x => x.ProcNum).ToList();
				//We block you from deleting all procedures on a preauth, which is consistent with the claim edit window, the chart module, and procedure edit form.
				if(listProcNumsForClaim.Except(listSelectedProcNums).Count()==0) {
					listSelectedProcNums.RemoveAll(x=>listProcNumsForClaim.Contains(x));
				}
			}
			try{
				for(int i=gridProc.SelectedIndices.Length-1;i>=0;i--) {
					Procedure proc=(Procedure)gridProc.ListGridRows[gridProc.SelectedIndices[i]].Tag;
					if(!Procedures.IsProcComplDeleteAuthorized(proc)) {
							skipped++;
							skippedSecurity++;
							continue;
					}
					if(!ListTools.In(proc.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)
						&& !Security.IsAuthorized(Permissions.ProcDelete,Procedures.GetDateForPermCheck(proc),true)) 
					{
						skippedSecurity++;
						continue;
					}
					dictOrthoProcLinks.TryGetValue(proc.ProcNum,out orthoProcLink);
					if(orthoProcLink!=null) {
						skippedLinkedToOrthoCase++;
						continue;
					}
					if(!listSelectedProcNums.Contains(proc.ProcNum)) {
						skippedPreauth++;
						continue;
					}
					Procedures.Delete(proc.ProcNum);
					isProcDeleted=true;
					if(ListTools.In(proc.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)) {
						Permissions perm=Permissions.ProcCompleteStatusEdit;
						if(ListTools.In(proc.ProcStatus,ProcStat.EO,ProcStat.EC)) {
							perm=Permissions.ProcExistingEdit;
						}
						SecurityLogs.MakeLogEntry(perm,AptCur.PatNum,ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode
							+" ("+proc.ProcStatus+"), "+proc.ProcFee.ToString("c")+", Deleted");
					}
					else {
						SecurityLogs.MakeLogEntry(Permissions.ProcDelete,AptCur.PatNum,ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode
							+" ("+proc.ProcStatus+"), "+proc.ProcFee.ToString("c"));
					}
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
			_listProcsForAppt=Procedures.GetProcsForApptEdit(AptCur);
			if(isProcDeleted) {
				Appointments.SetProcDescript(AptCur,_listProcsForAppt);//This is called in Procedures.Delete(...) but is not reflected in our local AptCur.
				//This is to fix a very rare bug where the user deletes a set of procedures and then re-attaches the same procedures before closing the window.
				//This would cause the DB to have correct ProcDesript and ProcsColored values at the time. But when the user closes the window after reselecting
				//the same proces, the Appointments.Update(old,new) will not update those fields due to them being identical.
				//This would cause the appt bubble to contain the incorrect values.
				AptOld.ProcDescript=AptCur.ProcDescript;
				AptOld.ProcsColored=AptCur.ProcsColored;
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
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(FormP.SelectedCodeNum);
			List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(PlanList);
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum); //Enforcing the discount plan date is done in Procedures.Insert
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){procedureCode },null,//no procs to pull medical codes from yet
				new List<long>(){comboProv.GetSelectedProvNum()},pat.PriProv,pat.SecProv,pat.FeeSched,PlanList,new List<long>(){AptCur.ClinicNum},
				new List<Appointment>(){AptCur},listSubstitutionLinks,discountPlanNum);
			Procedure proc=Procedures.ConstructProcedureForAppt(FormP.SelectedCodeNum,AptCur,pat,_listPatPlans,PlanList,SubList,listFees);
			Procedures.Insert(proc);
			List<ClaimProc> claimProcList=new List<ClaimProc>();
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(pat.PatNum);
			List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){pat.PatNum});
			for(int i=0;i<listProcedures.Count;i++) {
				if(listProcedures[i].ProcNum==proc.ProcNum) {
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
			}
			Procedures.ComputeEstimates(proc,pat.PatNum,ref claimProcList,true,PlanList,_listPatPlans,_benefitList,
				_loadData.ListClaimProcHists,listClaimProcHistsLoop,true,
				pat.Age,SubList,
				null,false,false,listSubstitutionLinks,false,
				listFees);
			using FormProcEdit FormPE=new FormProcEdit(proc,pat.Copy(),fam);
			FormPE.ListClaimProcHists=_loadData.ListClaimProcHists;
			FormPE.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			FormPE.IsNew=true;
			FormPE.ShowDialog();
			if(FormPE.DialogResult==DialogResult.Cancel) {
				//any created claimprocs are automatically deleted from within procEdit window.
				try {
					Procedures.Delete(proc.ProcNum);//also deletes the claimprocs
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			_listProcsForAppt=Procedures.GetProcsForApptEdit(AptCur);
			FillProcedures();
			for(int i=0;i<gridProc.ListGridRows.Count;i++) {
				if(proc.ProcNum==((Procedure)gridProc.ListGridRows[i].Tag).ProcNum) {
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

		///<summary>This was FillTime, but all it does now is set the color.  This is still useful.  Color can change frequently.</summary>
		private void SetTimeSliderColors() {
			System.Drawing.Color colorProv=System.Drawing.Color.White;
			System.Drawing.Color colorProv2=System.Drawing.Color.White;
			if(checkIsHygiene.Checked){
				if(comboProvHyg.GetSelectedProvNum()!=0) {
					colorProv=Providers.GetColor(comboProvHyg.GetSelectedProvNum());
				}
				if(comboProv.GetSelectedProvNum()!=0) {
					colorProv2=Providers.GetColor(comboProv.GetSelectedProvNum());
				}
			}
			else{//normal
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

		private void CalculatePatternFromProcs(bool ignoreTimeLocked=false) {
			List<Procedure> listProcs=new List<Procedure>();
			foreach(int i in gridProc.SelectedIndices) {
				listProcs.Add((Procedure)gridProc.ListGridRows[i].Tag);
			}
			contrApptProvSlider.Pattern=Appointments.CalculatePattern(pat,comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),
				listProcs,checkTimeLocked.Checked,ignoreTimeLocked);
			//contrApptProvSlider will automatically change the PatternSecondary length to match.
		}

		private void checkTimeLocked_Click(object sender,EventArgs e) {
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
		}

		private void gridPatient_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCellCur=gridPatient.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCellCur.ColorText==System.Drawing.Color.Blue && gridCellCur.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCellCur.Text);
			}
		}

		private void listQuickAdd_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(_isClickLocked) {
				return;
			}
			if(comboProv.GetSelectedProvNum()==0){
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			if(listQuickAdd.IndexFromPoint(e.Location)==-1) {
				return;
			}
			if(AptCur.AptStatus==ApptStatus.Complete) {
				//added procedures would be marked complete when form closes. We'll just stop it here.
				if(!Security.IsAuthorized(Permissions.ProcComplCreate)) {
					return;
				}
			}
			string[] codes=_listApptProcsQuickAddDefs[listQuickAdd.IndexFromPoint(e.Location)].ItemValue.Split(',');
			for(int i=0;i<codes.Length;i++) {
				if(!ProcedureCodes.GetContainsKey(codes[i])) {//these are D codes, not codeNums.
					MsgBox.Show(this,"Definition contains invalid code.");
					return;
				}
			}
			ODTuple<List<Procedure>,List<Procedure>> result=ApptEdit.QuickAddProcs(AptCur,pat,codes.ToList(),comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),SubList,
				PlanList,_listPatPlans,_benefitList);
			List<Procedure> listAddedProcs=result.Item1;
			_listProcsForAppt=result.Item2;
			listQuickAdd.SelectedIndex=-1;
			FillProcedures();
			for(int i=0;i<gridProc.ListGridRows.Count;i++) {
				//at this point, all procedures in the list should have a Primary Key.
				long procNumCur=((Procedure)gridProc.ListGridRows[i].Tag).ProcNum;
				if(listAddedProcs.Any(x => x.ProcNum==procNumCur)) {
					gridProc.SetSelected(i,true);//Select those that were just added.
				}
			}
			CalculatePatternFromProcs();
			//SetTimeSliderColors();
			CalcPatientFeeThisAppt();
			RefreshEstPatientPortion();
		}

		private void butLab_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(false)) {
				return;
			}			
			if(_labCur==null) {//no labcase
				//so let user pick one to add
				using FormLabCaseSelect FormL=new FormLabCaseSelect();
				FormL.PatNum=AptCur.PatNum;
				FormL.IsPlanned=_isPlanned;
				FormL.ShowDialog();
				if(FormL.DialogResult!=DialogResult.OK){
					return;
				}
				if(_isPlanned) {
					LabCases.AttachToPlannedAppt(FormL.SelectedLabCaseNum,AptCur.AptNum);
				}
				else{
					LabCases.AttachToAppt(FormL.SelectedLabCaseNum,AptCur.AptNum);
				}
			}
			else{//already a labcase attached
				using FormLabCaseEdit FormLCE=new FormLabCaseEdit();
				FormLCE.CaseCur=_labCur;
				FormLCE.ShowDialog();
				if(FormLCE.DialogResult!=DialogResult.OK){
					return;
				}
				//Deleting or detaching labcase would have been done from in that window
			}
			_labCur=LabCases.GetForApt(AptCur);
			textLabCase.Text=GetLabCaseDescript();
		}

		private void butInsPlan1_Click(object sender,EventArgs e) {
			using FormInsPlanSelect FormIPS=new FormInsPlanSelect(AptCur.PatNum);
			FormIPS.ShowNoneButton=true;
			FormIPS.ViewRelat=false;
			FormIPS.ShowDialog();
			if(FormIPS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormIPS.SelectedPlan==null) {
				AptCur.InsPlan1=0;
				textInsPlan1.Text="";
				return;
			}
			AptCur.InsPlan1=FormIPS.SelectedPlan.PlanNum;
			textInsPlan1.Text=InsPlans.GetCarrierName(AptCur.InsPlan1,PlanList);
		}

		private void butInsPlan2_Click(object sender,EventArgs e) {
			using FormInsPlanSelect FormIPS=new FormInsPlanSelect(AptCur.PatNum);
			FormIPS.ShowNoneButton=true;
			FormIPS.ViewRelat=false;
			FormIPS.ShowDialog();
			if(FormIPS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormIPS.SelectedPlan==null) {
				AptCur.InsPlan2=0;
				textInsPlan2.Text="";
				return;
			}
			AptCur.InsPlan2=FormIPS.SelectedPlan.PlanNum;
			textInsPlan2.Text=InsPlans.GetCarrierName(AptCur.InsPlan2,PlanList);
		}

		private void butRequirement_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(false)) {
				return;
			}			
			using FormReqAppt FormR=new FormReqAppt();
			FormR.AptNum=AptCur.AptNum;
			FormR.PatNum=AptCur.PatNum;
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK){
				return;
			}
			List<ReqStudent> listStudents=ReqStudents.GetForAppt(AptCur.AptNum);
			textRequirement.Text = string.Join("\r\n",listStudents
				.Select(x => new { Student = Providers.GetDeepCopy().First(y => y.ProvNum==x.ProvNum),Descript = x.Descript })
				.Select(x => x.Student.LName+", "+x.Student.FName+": "+x.Descript).ToList());
		}

		private void butSyndromicObservations_Click(object sender,EventArgs e) {
			if(AptCur.AptNum==0) {
				MsgBox.Show("Please click OK to create this appointment before taking this action.");
				return;
			}
			using FormEhrAptObses formE=new FormEhrAptObses(AptCur);
			formE.ShowDialog();
		}

		private void menuItemArrivedNow_Click(object sender,EventArgs e) {
			textTimeArrived.Text=DateTime.Now.ToShortTimeString();
		}

		private void menuItemSeatedNow_Click(object sender,EventArgs e) {
			textTimeSeated.Text=DateTime.Now.ToShortTimeString();
		}

		private void menuItemDismissedNow_Click(object sender,EventArgs e) {
			textTimeDismissed.Text=DateTime.Now.ToShortTimeString();
		}

		private void gridFields_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(false)) {
				return;
			}
			if(ApptFieldDefs.HasDuplicateFieldNames()) {//Check for duplicate field names.
				MsgBox.Show(this,"There are duplicate appointment field defs, go rename or delete the duplicates.");
				return;
			}
			ApptField field=ApptFields.GetOne(PIn.Long(_tableFields.Rows[e.Row]["ApptFieldNum"].ToString()));
			if(field==null) {
				field=new ApptField();
				field.IsNew=true;
				field.AptNum=AptCur.AptNum;
				field.FieldName=_tableFields.Rows[e.Row]["FieldName"].ToString();
				ApptFieldDef fieldDef=ApptFieldDefs.GetFieldDefByFieldName(field.FieldName);
				if(fieldDef==null) {//This could happen if the field def was deleted while the appointment window was open.
					MsgBox.Show(this,"This Appointment Field Def no longer exists.");
				}
				else {
					if(fieldDef.FieldType==ApptFieldType.Text) {
						using FormApptFieldEdit formAF=new FormApptFieldEdit(field);
						formAF.ShowDialog();
					}
					else if(fieldDef.FieldType==ApptFieldType.PickList) {
						using FormApptFieldPickEdit formAF=new FormApptFieldPickEdit(field);
						formAF.ShowDialog();
					}
				}
			}
			else if(ApptFieldDefs.GetFieldDefByFieldName(field.FieldName)!=null) {
				if(ApptFieldDefs.GetFieldDefByFieldName(field.FieldName).FieldType==ApptFieldType.Text) {
					using FormApptFieldEdit formAF=new FormApptFieldEdit(field);
					formAF.ShowDialog();
				}
				else if(ApptFieldDefs.GetFieldDefByFieldName(field.FieldName).FieldType==ApptFieldType.PickList) {
					using FormApptFieldPickEdit formAF=new FormApptFieldPickEdit(field);
					formAF.ShowDialog();
				}
			}
			else {//This probably won't happen because a field def should not be able to be deleted while in use.
				MsgBox.Show(this,"This Appointment Field Def no longer exists.");
			}
			_tableFields=Appointments.GetApptFields(AptCur.AptNum);
			FillFields();
		}

		///<summary>Validates and saves appointment and procedure information to DB.</summary>
		private bool UpdateListAndDB(bool isClosing=true,bool doCreateSecLog=false,bool doInsertHL7=false) {
			DateTime datePrevious=AptCur.DateTStamp;
			List<long> listAptsToDelete=new List<long>();
			_listProcsForAppt=Procedures.GetProcsForApptEdit(AptCur);//We need to refresh so we can check for concurrency issues.
			FillProcedures();//This refills the tags in the grid so we can use the tags below.  Will also show concurrent changes by other users.
			#region PrefName.ApptsRequireProc and Permissions.ProcComplCreate check
			//First check that they have an procedures attached to this appointment. If the appointment is an existing appointment that did not originally
			//have any procedures attached, the prompt will not come up.
			if((IsNew || _listProcNumsAttachedStart.Count>0)
				&& PrefC.GetBool(PrefName.ApptsRequireProc)
				&& gridProc.SelectedIndices.Length==0
				&& !ListTools.In(AptCur.AptStatus,ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) 
			{
				MsgBox.Show(this,"At least one procedure must be attached to the appointment.");
				return false;
			}
			List<Procedure> listSelectedProcs=gridProc.SelectedIndices.Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
			if(_selectedApptStatus==ApptStatus.Complete 
				&& gridProc.SelectedIndices.Select(x => (Procedure)gridProc.ListGridRows[x].Tag).Any(x => x.ProcStatus!=ProcStat.C)) 
			{//Appt is complete, but a selected proc is not.
				listSelectedProcs.RemoveAll(x => x.ProcStatus==ProcStat.C);//only care about the procs that are not already complete (new attaching procs)
				foreach(Procedure proc in listSelectedProcs) {
					if(!Security.IsAuthorized(Permissions.ProcComplCreate,AptCur.AptDateTime,proc.CodeNum,proc.ProcFee)) {
						return false;
					}
				}
			}
			#endregion
			#region Check for Procs Attached to Another Appt
			//When _isInsertRequired is true AptCur.AptNum=0.
			//The below logic works when 0 due to AptCur.[Planned]AptNum!=0 checks.
			bool hasProcsConcurrent=false;
			//This dictionary holds the original aptNum for a previously attached procedure. 
			//The value is the count of procedures being moved from the associated aptNum.
			//We will use this to determine if the procedure's original appointment needs to be deleted (if all procedures are moved to another appointment).
			Dictionary<long,int> dictProcsBeingMoved=new Dictionary<long,int>();
			for(int i=0;i<gridProc.ListGridRows.Count;i++) {
				Procedure proc=(Procedure)gridProc.ListGridRows[i].Tag;
				bool isAttaching=gridProc.SelectedIndices.Contains(i);
				bool isAttachedStart=_listProcNumsAttachedStart.Contains(proc.ProcNum);
				if(!isAttachedStart && isAttaching && _isPlanned) {//Attaching to this planned appointment.
					if(proc.PlannedAptNum != 0 && proc.PlannedAptNum != AptCur.AptNum) {//However, the procedure is attached to another planned appointment.
						hasProcsConcurrent=true;
						//Make note of the appointment the procedure will be moved off of.
						if(!dictProcsBeingMoved.ContainsKey(proc.PlannedAptNum)) {
							dictProcsBeingMoved[proc.PlannedAptNum]=0;
						}
						dictProcsBeingMoved[proc.PlannedAptNum]++;
					}
				}
				else if(!isAttachedStart && isAttaching && !_isPlanned) {//Attaching to this appointment.
					if(proc.AptNum != 0 && proc.AptNum != AptCur.AptNum) {//However, the procedure is attached to another appointment.
						hasProcsConcurrent=true;
						//Make note of the appointment the procedure will be moved off of.
						if(!dictProcsBeingMoved.ContainsKey(proc.AptNum)) {
							dictProcsBeingMoved[proc.AptNum]=0;
						}
						dictProcsBeingMoved[proc.AptNum]++;
					}
				}
			}
			if(PrefC.GetBool(PrefName.ApptsRequireProc) && dictProcsBeingMoved.Count>0) {//Only check if we are actually moving procedures.
				Dictionary<long,int> dictAptsProcCount=Appointments.GetProcCountForUnscheduledApts(dictProcsBeingMoved.Keys.ToList());
				//Check to see if the number of procedures we are stealing from the original appointment is the same
				//as the total number of procedures on the appointment. If this is the case the appointment must be deleted.
				//Per the job for this feature we will only delete unscheduled appointments that become empty.
				foreach(long aptNum in dictAptsProcCount.Keys) {
					if(dictProcsBeingMoved[aptNum]==dictAptsProcCount[aptNum]) {
						listAptsToDelete.Add(aptNum);
					}
				}
			}
			if(listAptsToDelete.Count>0) {
				//Verbiage approved by Allen
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,
					"One or more procedures are attached to another appointment.\r\n"
					+"All selected procedures will be detached from the other appointment which will result in its deletion.\r\n"
					+"Continue?"))
				{
					return false;
				}
			}
			else if(hasProcsConcurrent && _isPlanned) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"One or more procedures are attached to another planned appointment.\r\n"
					+"All selected procedures will be detached from the other planned appointment.\r\n"
					+"Continue?"))
				{
					return false;
				}
			}
			else if(hasProcsConcurrent && !_isPlanned) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"One or more procedures are attached to another appointment.\r\n"
					+"All selected procedures will be detached from the other appointment.\r\n"
					+"Continue?"))
				{
					return false;
				}
			}
			#endregion Check for Procs Attached to Another Appt
			#region Validate Form Data
			//initial clinic selection based on Op, but user may also edit, so use selection.  The clinic combobox is the logical place to look
			//when being warned/blocked about specialty mismatch.  
			if(!AppointmentL.IsSpecialtyMismatchAllowed(AptCur.PatNum,comboClinic.SelectedClinicNum)) {
				return false;
			}
			if(AptOld.AptStatus!=ApptStatus.UnschedList && comboStatus.SelectedIndex==2) {//previously not on unsched list and sending to unscheduled list
				if(PatRestrictionL.IsRestricted(AptCur.PatNum,PatRestrict.ApptSchedule,true)) {
					MessageBox.Show(Lan.g(this,"Not allowed to send this appointment to the unscheduled list due to patient restriction")+" "
						+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+".");
					return false;
				}
				if(PrefC.GetBool(PrefName.UnscheduledListNoRecalls) 
					&& Appointments.IsRecallAppointment(AptCur,gridProc.SelectedGridRows.Select(x => (Procedure)(x.Tag)).ToList())) 
				{
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Recall appointments cannot be sent to the Unscheduled List.\r\nDelete appointment instead?")) {
						OnDelete_Click(true);//Skip the standard "Delete Appointment?" prompt since we have already prompted here. Closes form and syncs data.
					}
					return false;//Always return false since the appointment was either deleted of the user canceled.
				}
			}
			DateTime dateTimeAskedToArrive=DateTime.MinValue;
			if((AptOld.AptStatus==ApptStatus.Complete && comboStatus.SelectedIndex!=1)
				|| (AptOld.AptStatus==ApptStatus.Broken && comboStatus.SelectedIndex!=4)) //Un-completing or un-breaking the appt.  We must use selectedindex due to AptCur gets updated later UpdateDB()
			{
				//If the insurance plans have changed since this appt was completed, warn the user that the historical data will be neutralized.
				List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
				InsSub sub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,PlanList,SubList)),SubList);
				InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,PlanList,SubList)),SubList);
				if(sub1.PlanNum!=AptCur.InsPlan1 || sub2.PlanNum!=AptCur.InsPlan2) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The current insurance plans for this patient are different than the plans associated to this appointment.  They will be updated to the patient's current insurance plans.  Continue?")) {
						return false;
					}
					//Update the ins plans associated to this appointment so that they're the most accurate at this time.
					AptCur.InsPlan1=sub1.PlanNum;
					AptCur.InsPlan2=sub2.PlanNum;
				}
			}
			if(textTimeAskedToArrive.Text!=""){
				try{
					dateTimeAskedToArrive=AptCur.AptDateTime.Date+DateTime.Parse(textTimeAskedToArrive.Text).TimeOfDay;
				}
				catch{
					MsgBox.Show(this,"Time Asked To Arrive invalid.");
					return false;
				}
			}
			DateTime dateTimeArrived=AptCur.AptDateTime.Date;
			if(textTimeArrived.Text!=""){
				try{
					dateTimeArrived=AptCur.AptDateTime.Date+DateTime.Parse(textTimeArrived.Text).TimeOfDay;
				}
				catch{
					MsgBox.Show(this,"Time Arrived invalid.");
					return false;
				}
			}
			DateTime dateTimeSeated=AptCur.AptDateTime.Date;
			if(textTimeSeated.Text!=""){
				try{
					dateTimeSeated=AptCur.AptDateTime.Date+DateTime.Parse(textTimeSeated.Text).TimeOfDay;
				}
				catch{
					MsgBox.Show(this,"Time Seated invalid.");
					return false;
				}
			}
			DateTime dateTimeDismissed=AptCur.AptDateTime.Date;
			if(textTimeDismissed.Text!=""){
				try{
					dateTimeDismissed=AptCur.AptDateTime.Date+DateTime.Parse(textTimeDismissed.Text).TimeOfDay;
				}
				catch{
					MsgBox.Show(this,"Time Dismissed invalid.");
					return false;
				}
			}
			//This change was just slightly too risky to make to 6.9, so 7.0 only
			if(!PrefC.GetBool(PrefName.ApptAllowFutureComplete)//Not allowed to set future appts complete.
				&& AptCur.AptStatus!=ApptStatus.Complete//was not originally complete
				&& AptCur.AptStatus!=ApptStatus.PtNote
				&& AptCur.AptStatus!=ApptStatus.PtNoteCompleted
				&& comboStatus.SelectedIndex==1 //making it complete
				&& AptCur.AptDateTime.Date > DateTime.Today)//and future appt
			{
				MsgBox.Show(this,"Not allowed to set future appointments complete.");
				return false;
			}
			bool hasProcsAttached=gridProc.SelectedIndices
				//Get tags on rows as procedures if possible
				.Select(x=>gridProc.ListGridRows[x].Tag as Procedure)
				//true if any row had a valid procedure as a tag
				.Any(x=>x!=null);
			if(!PrefC.GetBool(PrefName.ApptAllowEmptyComplete)
				&& AptCur.AptStatus!=ApptStatus.Complete//was not originally complete
				&& AptCur.AptStatus!=ApptStatus.PtNote
				&& AptCur.AptStatus!=ApptStatus.PtNoteCompleted
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
			if(AptCur.AptStatus!=ApptStatus.Complete//was not originally complete
				&& _selectedApptStatus==ApptStatus.Complete //trying to make it complete
				&& hasProcsAttached
				&& !Security.IsAuthorized(Permissions.ProcComplCreate,AptCur.AptDateTime))//aren't authorized to complete procedures
			{
				return false;
			}
			#endregion
			#region Provider Term Date Check
			//Prevents appointments with providers that are past their term end date from being scheduled
			Appointment aptModified=AptCur.Copy();//Appt used only for the providers S class method
			aptModified.ProvNum=comboProv.GetSelectedProvNum();
			aptModified.ProvHyg=comboProvHyg.GetSelectedProvNum();
			if(_selectedApptStatus!=ApptStatus.UnschedList && _selectedApptStatus!=ApptStatus.Planned) {
				string message=Providers.CheckApptProvidersTermDates(aptModified);
				if(message!="") {
					MessageBox.Show(this,message);//translated in Providers S class method
					return false;
				}
			}
			#endregion Provider Term Date Check
			List<Procedure> listProcs=gridProc.SelectedIndices.OfType<int>().Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
			if(listProcs.Count > 0 && comboStatus.SelectedIndex==1 && AptCur.AptDateTime.Date > DateTime.Today.Date 
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) 
			{
				MsgBox.Show(this,"Not allowed to set procedures complete with future dates.");
				return false;
			}
			#endregion Validate Form Data
			//-----Point of no return-----
			#region Broken appt selections
			if(_formApptBreakSelection==ApptBreakSelection.Unsched && !AppointmentL.ValidateApptUnsched(AptCur)) {
				_formApptBreakSelection=ApptBreakSelection.None;//This way no additional logic runs below.
			}
			if(_formApptBreakSelection==ApptBreakSelection.Pinboard && !AppointmentL.ValidateApptToPinboard(AptCur)) {
				_formApptBreakSelection=ApptBreakSelection.None;//This way no additional logic runs below.
			}
			#endregion
			#region Set AptCur Fields
			AptCur.Pattern=contrApptProvSlider.Pattern;
			AptCur.PatternSecondary=contrApptProvSlider.PatternSecondary;
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
				&& AptCur.AptStatus!=ApptStatus.UnschedList
				&& !isAuxiliaryRole)//generic HL7 def enabled, appt module hidden and an inbound SIU msg defined, appts created from msgs so no overlap check
			{
				//Adjusts AptCur.Pattern directly when necessary.
				if(ControlAppt.TryAdjustAppointmentPattern(AptCur,ControlApptPanel.GetListOpsVisible())) {
					MsgBox.Show(this,"Appointment is too long and would overlap another appointment or blockout.  Automatically shortened to fit.");
//todo? Consider changing PatternSecondary length to match Pattern length.  But there are many places in the program where this would need to be done.  Probably easier to assume they can be out of synch.
				}
			}
			AptCur.ProvBarText=contrApptProvSlider.ProvBarText;
			AptCur.Priority=checkASAP.Checked ? ApptPriority.ASAP : ApptPriority.Normal;
			AptCur.AptStatus=_selectedApptStatus;
			//set procs complete was moved further down
			if(comboUnschedStatus.SelectedIndex==0){//none
				AptCur.UnschedStatus=0;
			}
			else{
				AptCur.UnschedStatus=_listRecallUnschedStatusDefs[comboUnschedStatus.SelectedIndex-1].DefNum;
			}
			if(comboConfirmed.SelectedIndex!=-1){
				AptCur.Confirmed=_listApptConfirmedDefs[comboConfirmed.SelectedIndex].DefNum;
			}
			AptCur.TimeLocked=checkTimeLocked.Checked;
			AptCur.ColorOverride=butColor.BackColor;
			AptCur.Note=textNote.Text;
			AptCur.ClinicNum=comboClinic.SelectedClinicNum;
			AptCur.ProvNum=comboProv.GetSelectedProvNum();
			AptCur.ProvHyg=comboProvHyg.GetSelectedProvNum();
			AptCur.IsHygiene=checkIsHygiene.Checked;
			if(comboAssistant.SelectedIndex==0) {//none
				AptCur.Assistant=0;
			}
			else {
				AptCur.Assistant=_listEmployees[comboAssistant.SelectedIndex-1].EmployeeNum;
			}
			AptCur.IsNewPatient=checkIsNewPatient.Checked;
			AptCur.DateTimeAskedToArrive=dateTimeAskedToArrive;
			AptCur.DateTimeArrived=dateTimeArrived;
			AptCur.DateTimeSeated=dateTimeSeated;
			AptCur.DateTimeDismissed=dateTimeDismissed;
			//AptCur.InsPlan1 and InsPlan2 already handled 
			if(comboApptType.SelectedIndex==0) {//0 index = none.
				AptCur.AppointmentTypeNum=0;
			}
			else {
				AptCur.AppointmentTypeNum=_listAppointmentType[comboApptType.SelectedIndex-1].AppointmentTypeNum;
			}
			#endregion Set AptCur Fields
			#region Update ProcDescript for Appt
			//Use the current selections to set AptCur.ProcDescript.
			List<Procedure> listGridSelectedProcs=new List<Procedure>();
			gridProc.SelectedIndices.ToList().ForEach(x => listGridSelectedProcs.Add(_listProcsForAppt[x].Copy()));
			foreach(Procedure proc in listGridSelectedProcs) {
				//This allows Appointments.SetProcDescript(...) to associate all the passed in procs into AptCur.ProcDescript
				//listGridSelectedProcs is only used here and contains copies of procs.
				proc.AptNum=AptCur.AptNum;
				proc.PlannedAptNum=AptCur.AptNum;
			}
			Appointments.SetProcDescript(AptCur,listGridSelectedProcs);
			#endregion Update ProcDescript for Appt
			#region Provider change and fee change check
			//Determins if we would like to update ProcFees when a provider changes, considers PrefName.ProcFeeUpdatePrompt.
			bool updateProcFees=false;
			if(AptCur.AptStatus!=ApptStatus.Complete && (comboProv.GetSelectedProvNum()!=AptOld.ProvNum || comboProvHyg.GetSelectedProvNum()!=AptOld.ProvHyg)) {//Either the primary or hygienist changed.
				List<Procedure> listNewProcs=gridProc.SelectedIndices.Select(x => Procedures.ChangeProcInAppointment(AptCur,((Procedure)gridProc.ListGridRows[x].Tag).Copy())).ToList();
				List<Procedure> listOldProcs=gridProc.SelectedIndices.Select(x => ((Procedure)gridProc.ListGridRows[x].Tag).Copy()).ToList();
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(AptCur.PatNum);
				string promptText="";
				//PatientNote "Appointment" will never have fees.  Prompting/Updating proc fees unnecessary.
				updateProcFees=(!_isPtNote && Procedures.ShouldFeesChange(listNewProcs,listOldProcs,ref promptText,procFeeHelper));
				if(updateProcFees && promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
					updateProcFees=false;
				}
			}
			bool removeCompleteProcs=ProcedureL.DoRemoveCompletedProcs(AptCur,listGridSelectedProcs,true);
			#endregion
			#region Save to DB
			Appointments.ApptSaveHelperResult result;
			try {
				result=Appointments.ApptSaveHelper(AptCur,AptOld,_isInsertRequired,_listProcsForAppt,_listAppointments,
					gridProc.SelectedIndices.ToList(),_listProcNumsAttachedStart,_isPlanned,PlanList,SubList,comboProv.GetSelectedProvNum(),comboProvHyg.GetSelectedProvNum(),
					listGridSelectedProcs,IsNew,pat,fam,updateProcFees,removeCompleteProcs,doCreateSecLog,doInsertHL7);
				AptCur=result.AptCur;
				_listProcsForAppt=result.ListProcsForAppt;
				_listAppointments=result.ListAppts;
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return false;
			}			
			if(_isInsertRequired && AptOld.AptNum==0) {
				//Update the the old AptNum since this is a new appointment.
				//This stops Appointments.Sync(...) from double insertings this new appointment.
				AptOld.AptNum=AptCur.AptNum;
				_listAppointmentsOld.FirstOrDefault(x => x.AptNum==0).AptNum=AptCur.AptNum;
			}
			_isInsertRequired=false;//Now that we have inserted the new appointment, let typical appointment logic handle from here on.
			#endregion Save changes to DB
			#region Update gridProc tags
			//update tags with changes made so that anyone accessing it later has an updated copy.
			foreach(int index in gridProc.SelectedIndices) {
				Procedure procNew=_listProcsForAppt.FirstOrDefault(x => x.ProcNum==((Procedure)gridProc.ListGridRows[index].Tag).ProcNum);
				if(procNew==null) {
					continue;
				}
				gridProc.ListGridRows[index].Tag=procNew.Copy();
			}
			#endregion
			#region Automation
			if(result.DoRunAutomation) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,_listProcsForAppt.FindAll(x => x.AptNum==AptCur.AptNum)
					.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList(),AptCur.PatNum);
			}
			if(AptCur.AptStatus==ApptStatus.Complete) {
				ProcedureL.AfterProcsSetComplete(listSelectedProcs);
			}
			#endregion Automation
			#region Broken Appt Logic
			//Do the appointment "break" automation for appointments that were just broken or going to the unscheduled list (sometimes).
			//If BrokenApptRequiredOnMove is on and a user selects the unsched list drop down item, the appointment 
			//ends up here with a status of UnschedList because the appointment has not been broken yet.
			if(AptCur.AptStatus==ApptStatus.Broken && AptOld.AptStatus!=ApptStatus.Broken || (PrefC.GetBool(PrefName.BrokenApptRequiredOnMove) 
				&& AptCur.AptStatus==ApptStatus.UnschedList && AptOld.AptStatus==ApptStatus.Scheduled)) 
			{
				AppointmentL.BreakApptHelper(AptCur,pat,_procCodeBroken);
				if(isClosing) {
					switch(_formApptBreakSelection) {//ApptBreakSelection.None by default.
						case ApptBreakSelection.Unsched:
							AppointmentL.SetApptUnschedHelper(AptCur,pat);
							break;
						case ApptBreakSelection.Pinboard:
							AppointmentL.CopyAptToPinboardHelper(AptCur);
							break;
						case ApptBreakSelection.None://User did not makes selection
						case ApptBreakSelection.ApptBook://User made selection, no extra logic required.
							break;
					}
				}
			}
			#endregion Broken Appt Logic
			#region Cleanup Empty Apts
			//We have finished saving this appointment. We can now safely delete the unscheduled appointments marked for deletion.
			if(listAptsToDelete.Count>0) {
				Appointments.Delete(listAptsToDelete);
				//Nathan asked for a specific log entry message explaining why each apt was deleted.
				foreach(long aptNumDeleted in listAptsToDelete) {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,AptCur.PatNum
						,"All procedures were moved off of the appointment, resulting in its deletion."
						,aptNumDeleted,DateTime.MinValue);
				}
			}
			#endregion
			return true;
		}
		
		private void butPDF_Click(object sender,EventArgs e) {
			if(_isInsertRequired) {
				MsgBox.Show(this,"Please click OK to create this appointment before taking this action.");
				return;
			}
			//this will only happen for eCW HL7 interface users.
			List<Procedure> listProcsForAppt=Procedures.GetProcsForSingle(AptCur.AptNum,AptCur.AptStatus==ApptStatus.Planned);
			string duplicateProcs=ProcedureL.ProcsContainDuplicates(listProcsForAppt);
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
				MessageHL7 messageHL7=MessageConstructor.GenerateDFT(new List<Procedure>(),EventTypeHL7.P03,pat,Patients.GetPat(pat.Guarantor),AptCur.AptNum,"progressnotes",pdfDataStr);
				if(messageHL7==null) {
					MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
					return;
				}
				HL7Msg hl7Msg=new HL7Msg();
				//hl7Msg.AptNum=AptCur.AptNum;
				hl7Msg.AptNum=0;//Prevents the appt complete button from changing to the "Revise" button prematurely.
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=messageHL7.ToString();
				hl7Msg.PatNum=pat.PatNum;
				HL7Msgs.Insert(hl7Msg);
				if(ODBuild.IsDebug()) {
					MessageBox.Show(this,messageHL7.ToString());
				}
			}
			else {
				//Note: AptCur.ProvNum may not reflect the selected provider in comboProv. This is still the Provider that the appointment was last saved with.
				Bridges.ECW.SendHL7(AptCur.AptNum,AptCur.ProvNum,pat,pdfDataStr,"progressnotes",true,null);//just pdf, passing null proc list
			}
			MsgBox.Show(this,"Notes PDF sent.");
		}

		///<summary>Creates a new .pdf file containing all of the procedures attached to this appointment and 
		///returns the contents of the .pdf file as a base64 encoded string.</summary>
		private string GenerateProceduresIntoPdf(){
			MigraDoc.DocumentObjectModel.Document doc=new MigraDoc.DocumentObjectModel.Document();
			doc.DefaultPageSetup.PageWidth=Unit.FromInch(8.5);
			doc.DefaultPageSetup.PageHeight=Unit.FromInch(11);
			doc.DefaultPageSetup.TopMargin=Unit.FromInch(.5);
			doc.DefaultPageSetup.LeftMargin=Unit.FromInch(.5);
			doc.DefaultPageSetup.RightMargin=Unit.FromInch(.5);
			MigraDoc.DocumentObjectModel.Section section=doc.AddSection();
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(9,false);
			string text;
			//Heading---------------------------------------------------------------------------------------------------------------
			#region printHeading
			Paragraph par=section.AddParagraph();
			ParagraphFormat parformat=new ParagraphFormat();
			parformat.Alignment=ParagraphAlignment.Center;
			parformat.Font=MigraDocHelper.CreateFont(10,true);
			par.Format=parformat;
			text=Lan.g(this,"procedures").ToUpper();
			par.AddFormattedText(text,headingFont);
			par.AddLineBreak();
			text=pat.GetNameFLFormal();
			par.AddFormattedText(text,headingFont);
			par.AddLineBreak();
			text=DateTime.Now.ToShortDateString();
			par.AddFormattedText(text,headingFont);
			par.AddLineBreak();
			par.AddLineBreak();
			#endregion
			//Procedure List--------------------------------------------------------------------------------------------------------
			#region Procedure List
			GridOD gridProg=new GridOD();
			gridProg.TranslationName="";
			this.Controls.Add(gridProg);//Only added temporarily so that printing will work. Removed at end with Dispose().
			gridProg.BeginUpdate();
			gridProg.ListGridColumns.Clear();
			GridColumn col;
			List<DisplayField> fields=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			for(int i=0;i<fields.Count;i++){
				if(fields[i].InternalName=="User" || fields[i].InternalName=="Signed"){
					continue;
				}
				if(fields[i].Description==""){
					col=new GridColumn(fields[i].InternalName,fields[i].ColumnWidth);
				}
				else{
					col=new GridColumn(fields[i].Description,fields[i].ColumnWidth);
				}
				if(fields[i].InternalName=="Amount"){
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(fields[i].InternalName=="Proc Code")
				{
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridProg.ListGridColumns.Add(col);
			}
			gridProg.NoteSpanStart=2;
			gridProg.NoteSpanStop=7;
			gridProg.ListGridRows.Clear();
			List<Procedure> procsForDay=Procedures.GetProcsForPatByDate(AptCur.PatNum,AptCur.AptDateTime);
			List<Def> listProgNoteColorDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			List<Def> listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
			for(int i=0;i<procsForDay.Count;i++){
				Procedure proc=procsForDay[i];
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				Provider prov=Providers.GetDeepCopy().First(x => x.ProvNum==proc.ProvNum);
				Userod usr=Userods.GetUser(proc.UserNum);
				GridRow row=new GridRow();
				row.ColorLborder=System.Drawing.Color.Black;
				for(int f=0;f<fields.Count;f++) {
					switch(fields[f].InternalName){
						case "Date":
							row.Cells.Add(proc.ProcDate.Date.ToShortDateString());
							break;
						case "Time":
							row.Cells.Add(proc.ProcDate.ToString("h:mm")+proc.ProcDate.ToString("%t").ToLower());
							break;
						case "Th":
							row.Cells.Add(Tooth.GetToothLabel(proc.ToothNum));
							break;
						case "Surf":
							row.Cells.Add(proc.Surf);
							break;
						case "Dx":
							row.Cells.Add(proc.Dx.ToString());
							break;
						case "Description":
							row.Cells.Add((procCode.LaymanTerm!="")?procCode.LaymanTerm:procCode.Descript);
							break;
						case "Stat":
							if(ProcMultiVisits.IsProcInProcess(proc.ProcNum)) {
								row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
							}
							else {
								row.Cells.Add(Lans.g("enumProcStat",proc.ProcStatus.ToString()));
							}
							break;
						case "Prov":
							row.Cells.Add(StringTools.Truncate(prov.Abbr,5));
							break;
						case "Amount":
							row.Cells.Add(proc.ProcFee.ToString("F"));
							break;
						case "Proc Code":
							if(procCode.ProcCode.Length>5 && procCode.ProcCode.StartsWith("D")) {
								row.Cells.Add(procCode.ProcCode.Substring(0,5));//Remove suffix from all D codes.
							}
							else {
								row.Cells.Add(procCode.ProcCode);
							}
							break;
					}
				}
				row.Note=proc.Note;
				//Row text color.
				switch(proc.ProcStatus) {
					case ProcStat.TP:
						row.ColorText=listProgNoteColorDefs[0].ItemColor;
						break;
					case ProcStat.C:
						row.ColorText=listProgNoteColorDefs[1].ItemColor;
						break;
					case ProcStat.EC:
						row.ColorText=listProgNoteColorDefs[2].ItemColor;
						break;
					case ProcStat.EO:
						row.ColorText=listProgNoteColorDefs[3].ItemColor;
						break;
					case ProcStat.R:
						row.ColorText=listProgNoteColorDefs[4].ItemColor;
						break;
					case ProcStat.D:
						row.ColorText=System.Drawing.Color.Black;
						break;
					case ProcStat.Cn:
						row.ColorText=listProgNoteColorDefs[22].ItemColor;
						break;
				}
				row.ColorBackG=System.Drawing.Color.White;
				if(proc.ProcDate.Date==DateTime.Today) {
					row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartTodaysProcs].ItemColor;
				}				
				gridProg.ListGridRows.Add(row);
			}
			gridProg.EndUpdate();
			MigraDocHelper.DrawGrid(section,gridProg);
			#endregion		
			MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(true,PdfFontEmbedding.Always);
			pdfRenderer.Document=doc;
			pdfRenderer.RenderDocument();
			MemoryStream ms=new MemoryStream();
			pdfRenderer.PdfDocument.Save(ms);
			byte[] pdfBytes=ms.GetBuffer();
			//#region Remove when testing is complete.
			//string tempFilePath=Path.GetTempFileName();
			//File.WriteAllBytes(tempFilePath,pdfBytes);
			//#endregion
			string pdfDataStr=Convert.ToBase64String(pdfBytes);
			ms.Dispose();
			return pdfDataStr;
		}

		private void butComplete_Click(object sender,EventArgs e) {
			//It is OK to let the user click the OK button as long as AptCur.AptNum is NOT used prior to UpdateListAndDB().
			//if(_isInsertRequired) {
			//	MsgBox.Show(this,"Please click OK to create this appointment before taking this action.");
			//	return;
			//}
			//This is only used with eCW HL7 interface.
			DateTime datePrevious=AptCur.DateTStamp;
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
			List<Procedure> listProcsForAppt=gridProc.SelectedIndices.OfType<int>().Select(x => (Procedure)gridProc.ListGridRows[x].Tag).ToList();
			string duplicateProcs=ProcedureL.ProcsContainDuplicates(listProcsForAppt);
			if(duplicateProcs!="") {
				MessageBox.Show(duplicateProcs);
				return;
			}
			if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"ProcNotesNoIncomplete")=="1") {
				if(listProcsForAppt.Any(x => x.Note!=null && x.Note.Contains("\"\""))) {
					MsgBox.Show(this,"This appointment cannot be sent because there are incomplete procedure notes.");
					return;
				}
			}
			if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"ProcRequireSignature")=="1") {
				if(listProcsForAppt.Any(x => !string.IsNullOrEmpty(x.Note) && string.IsNullOrEmpty(x.Signature))) {
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
			listProcsForAppt=Procedures.GetProcsForSingle(AptCur.AptNum,AptCur.AptStatus==ApptStatus.Planned);
			//Send DFT to eCW containing the attached procedures for this appointment in a .pdf file.				
			string pdfDataStr=GenerateProceduresIntoPdf();
			if(HL7Defs.IsExistingHL7Enabled()) {
				//MessageConstructor.GenerateDFT(procs,EventTypeHL7.P03,pat,Patients.GetPat(pat.Guarantor),AptCur.AptNum,"progressnotes",pdfDataStr);
				MessageHL7 messageHL7=MessageConstructor.GenerateDFT(listProcsForAppt,EventTypeHL7.P03,pat,Patients.GetPat(pat.Guarantor),AptCur.AptNum,
					"progressnotes",pdfDataStr);
				if(messageHL7==null) {
					MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
					return;
				}
				HL7Msg hl7Msg=new HL7Msg();
				hl7Msg.AptNum=AptCur.AptNum;
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=messageHL7.ToString();
				hl7Msg.PatNum=pat.PatNum;
				HL7ProcAttach hl7ProcAttach=new HL7ProcAttach();
				hl7ProcAttach.HL7MsgNum=HL7Msgs.Insert(hl7Msg);
				foreach(Procedure proc in listProcsForAppt) {
					hl7ProcAttach.ProcNum=proc.ProcNum;
					HL7ProcAttaches.Insert(hl7ProcAttach);
				}
			}
			else {
				Bridges.ECW.SendHL7(AptCur.AptNum,AptCur.ProvNum,pat,pdfDataStr,"progressnotes",false,listProcsForAppt);
			}
			CloseOD=true;
			if(IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,pat.PatNum,
				AptCur.AptDateTime.ToString()+", "+AptCur.ProcDescript,
				AptCur.AptNum,datePrevious);
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
			List<Permissions> perms=new List<Permissions>();
			perms.Add(Permissions.AppointmentCreate);
			perms.Add(Permissions.AppointmentEdit);
			perms.Add(Permissions.AppointmentMove);
			perms.Add(Permissions.AppointmentCompleteEdit);
			perms.Add(Permissions.ApptConfirmStatusEdit);
			using FormAuditOneType FormA=new FormAuditOneType(pat.PatNum,perms,Lan.g(this,"Audit Trail for Appointment"),AptCur.AptNum);
			FormA.ShowDialog();
		}

		private void butTask_Click(object sender,EventArgs e) {
			if(_isInsertRequired && !UpdateListAndDB(false)) {
				return;
			}
			using FormTaskListSelect FormT=new FormTaskListSelect(TaskObjectType.Appointment);//,AptCur.AptNum);
			FormT.Text=Lan.g(FormT,"Add Task")+" - "+FormT.Text;
			FormT.ShowDialog();
			if(FormT.DialogResult!=DialogResult.OK) {
				return;
			}
			Task task=new Task();
			task.TaskListNum=-1;//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			task.KeyNum=AptCur.AptNum;
			task.ObjectType=TaskObjectType.Appointment;
			task.TaskListNum=FormT.ListSelectedLists[0];
			task.UserNum=Security.CurUser.UserNum;
			using FormTaskEdit FormTE=new FormTaskEdit(task,taskOld);
			FormTE.IsNew=true;
			FormTE.ShowDialog();
		}

		private void butPin_Click(object sender,System.EventArgs e) {
			if(ListTools.In(AptCur.AptStatus,ApptStatus.UnschedList,ApptStatus.Planned)
				&& ListTools.In(pat.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) 
			{
				MsgBox.Show(this,"Appointments cannot be scheduled for "+pat.PatStatus.ToString().ToLower()+" patients.");
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

		///<summary>Returns true if the appointment type was successfully changed, returns false if the user decided to cancel out of doing so.</summary>
		private bool AptTypeHelper() {
			if(comboApptType.SelectedIndex==0) {//'None' is selected so maintain grid selections.
				return true;
			}
			if(ListTools.In(AptCur.AptStatus,ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				return true;//Patient notes can't have procedures associated to them.
			}
			AppointmentType aptTypeCur=_listAppointmentType[comboApptType.SelectedIndex-1];
			List<ProcedureCode> listAptTypeProcs=ProcedureCodes.GetFromCommaDelimitedList(aptTypeCur.CodeStr);
			List<Procedure> listSelectedProcs;
			if(listAptTypeProcs.Count>0) {//AppointmentType is associated to procs.
				listSelectedProcs=gridProc.SelectedTags<Procedure>();
				List<long> listProcCodeNumsToDetach=listSelectedProcs.Select(y => y.CodeNum).ToList()
				.Except(listAptTypeProcs.Select(x => x.CodeNum).ToList()).ToList();
				//if there are procedures that would get detached
				//and if they have the preference AppointmentTypeWarning on,
				//Display the warning
				if(listProcCodeNumsToDetach.Count>0 && PrefC.GetBool(PrefName.AppointmentTypeShowWarning)) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Selecting this appointment type will dissociate the current procedures from this "
						+"appointment and attach the procedures defined for this appointment type.  Do you want to continue?")) {
						return false;
					}
				}
				Appointments.ApptTypeMissingProcHelper(AptCur,aptTypeCur,_listProcsForAppt,pat,true,_listPatPlans,SubList,PlanList,_benefitList);
				FillProcedures();
				//Since we have detached and attached all pertinent procs by this point it is safe to just use the PlannedAptNum or AptNum.
				gridProc.SetAll(false);
				foreach(ProcedureCode procCodeCur in listAptTypeProcs) {
					List<Procedure> listSelectedAptTypeProcs = listSelectedProcs.FindAll(x => x.CodeNum==procCodeCur.CodeNum);
					if(listSelectedAptTypeProcs.Count>0) { // If procedures with this code were already selected, preserve those selections
						gridProc.SetSelected(listSelectedAptTypeProcs.Select(x => gridProc.GetTags<Procedure>().FindIndex(y => x==y)).Where(x => x>-1).ToArray(),true);
						continue;
					}
					for(int i=0;i<gridProc.ListGridRows.Count;i++) {
						Procedure rowProc=(Procedure)gridProc.ListGridRows[i].Tag;
						if(rowProc.CodeNum==procCodeCur.CodeNum
							//if the procedure code already exists in the grid and it's not attached to another appointment or planned appointment
							&& (_isPlanned && (rowProc.PlannedAptNum==0 || rowProc.PlannedAptNum==AptCur.AptNum)
								|| (!_isPlanned && (rowProc.AptNum==0 || rowProc.AptNum==AptCur.AptNum)))
							//The row is not already selected. This is necessary so that Apt Types with two of the same procs will select both procs.
							&& !gridProc.SelectedIndices.Contains(i)) 
						{
							gridProc.SetSelected(i,true); //set procedures selected in the grid.
							break;
						}
					}
				}
			}
			butColor.BackColor=aptTypeCur.AppointmentTypeColor;
			if(aptTypeCur.Pattern!=null && aptTypeCur.Pattern!="") {
				contrApptProvSlider.Pattern=aptTypeCur.Pattern;
			}
			//calculate the new time pattern.
			if(aptTypeCur!=null && listAptTypeProcs != null) {
				//Has Procs, but not time.
				if(aptTypeCur.Pattern=="" && listAptTypeProcs.Count > 0) {
					//Calculate and Fill
					CalculatePatternFromProcs(true);
					AptCur.Pattern=contrApptProvSlider.Pattern;
					//SetTimeSliderColors();
				}
				//Has fixed time
				else if(aptTypeCur.Pattern!="") {
					AptCur.Pattern=aptTypeCur.Pattern;
					//SetTimeSliderColors();
				}
				//No Procs, No time.
				else {
					//do nothing to the time pattern
				}
			}
			return true;
		}

		///<summary>Only catches user changes, not programatic changes. For instance this does not fire when loading the form.</summary>
		private void comboApptType_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!AptTypeHelper()) {
				comboApptType.SelectedIndex=_aptTypeIndex;
				return;
			}
			_aptTypeIndex=comboApptType.SelectedIndex;
		}

		private void comboConfirmed_SelectionChangeCommitted(object sender,EventArgs e) {
			if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)!=0 //Using appointmentTimeArrivedTrigger preference
				&& _listApptConfirmedDefs[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeArrived.Text))//time not already set 
			{
				textTimeArrived.Text=DateTime.Now.ToShortTimeString();
			}
			if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)!=0 //Using AppointmentTimeSeatedTrigger preference
				&& _listApptConfirmedDefs[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeSeated.Text))//time not already set 
			{
				textTimeSeated.Text=DateTime.Now.ToShortTimeString();
			}
			if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)!=0 //Using AppointmentTimeDismissedTrigger preference
				&& _listApptConfirmedDefs[comboConfirmed.SelectedIndex].DefNum==PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger) //selected index matches pref
				&& String.IsNullOrWhiteSpace(textTimeDismissed.Text))//time not already set 
			{
				textTimeDismissed.Text=DateTime.Now.ToShortTimeString();
			}
		}

		private void comboStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			//This block of logic must happen first(The if statement).
			if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
				if(AptOld.AptStatus==ApptStatus.Scheduled && _selectedApptStatus==ApptStatus.UnschedList) {
					using FormApptBreakRequired formApptForceBreak=new FormApptBreakRequired();
					formApptForceBreak.ShowDialog();
					if(formApptForceBreak.DialogResult!=DialogResult.OK) {
						return;
					}
					_formApptBreakSelection=ApptBreakSelection.Unsched;
					_procCodeBroken=formApptForceBreak.SelectedBrokenProcCode;
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
			if(AptCur.AptStatus==ApptStatus.PtNoteCompleted||AptCur.AptStatus==ApptStatus.PtNote) {
				return;
			}
			if(DoPreventCompletedApptChange(PreventChangesApptAction.Status)) {
				//Change the status back to Complete before returning.
				comboStatus.SelectedIndex=1;//Complete
				return;
			}
			using FormApptBreak formAB=new FormApptBreak(AptCur);
			if(formAB.ShowDialog()!=DialogResult.OK) {
				SetAptCurComboStatusSelection();//Sets status back to on load selection.
				if(formAB.FormApptBreakSelection==ApptBreakSelection.Delete) {
					//User wants to delete the appointment.
					OnDelete_Click(true);//Skip the standard "Delete Appointment?" prompt since we have already prompted in FormApptBreak.
					return;
				}
				_formApptBreakSelection=ApptBreakSelection.None;
				_procCodeBroken=null;
				return;
			}
			_formApptBreakSelection=formAB.FormApptBreakSelection;
			_procCodeBroken=formAB.SelectedProcCode;
		}

		///<summary>Returns true if the user is not allowed to change a completed appointment.</summary>
		private bool DoPreventCompletedApptChange(PreventChangesApptAction action) {
			List<Procedure> listAttachedProcs=gridProc.SelectedTags<Procedure>();
			bool doPreventChange=false;
			switch(action) {
				case PreventChangesApptAction.Delete:
					doPreventChange=AppointmentL.DoPreventChangesToCompletedAppt(AptOld,action,listAttachedProcs);
					break;
				case PreventChangesApptAction.Status:
					doPreventChange=comboStatus.SelectedIndex!=1 && //Setting the Apt status to something other than Complete
						AppointmentL.DoPreventChangesToCompletedAppt(AptOld,action,listAttachedProcs);
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
				List<Procedure> listAttachedProcs=_listProcsForAppt.FindAll(x => x.AptNum == AptCur.AptNum).Select(x => x.Copy()).ToList();
				return AppointmentL.DoPreventChangesToCompletedAppt(AptOld,PreventChangesApptAction.Procedures,out msg,listAttachedProcs);
		  }
			return false;
		}

		private void checkASAP_CheckedChanged(object sender,EventArgs e) {
			if(checkASAP.Checked) {
				checkASAP.ForeColor=System.Drawing.Color.Red;
			}
			else {
				checkASAP.ForeColor=SystemColors.ControlText;
			}
		}

		private bool CheckFrequencies() {
			List<Procedure> listProcsForFrequency=new List<Procedure>();
			foreach(int index in gridProc.SelectedIndices) {
				Procedure proc=((Procedure)gridProc.ListGridRows[index].Tag).Copy();
				if(proc.ProcStatus==ProcStat.TP) {
					listProcsForFrequency.Add(proc);
				}
			}
			if(listProcsForFrequency.Count>0) {
				string frequencyConflicts="";
				DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(pat.PatNum);
				if(discountPlanSub==null) {
					try {
						frequencyConflicts=Procedures.CheckFrequency(listProcsForFrequency,pat.PatNum,AptCur.AptDateTime);
					}
					catch(Exception e) {
						MessageBox.Show(Lan.g(this,"There was an error checking frequencies."
							+"  Disable the Insurance Frequency Checking feature or try to fix the following error:")
							+"\r\n"+e.Message);
						return false;
					}
					if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
						+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
					{
						return false;
					}
				} 
				else {
					try {
						frequencyConflicts=DiscountPlans.CheckDiscountFrequency(listProcsForFrequency,pat.PatNum,AptCur.AptDateTime);
					}
					catch(Exception e) {
						MessageBox.Show(Lan.g(this,"There was an error checking discount frequencies.")
							+"\r\n"+e.Message);
						return false;
					}
					if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
						+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			OnDelete_Click();
		}

		///<summary>Deletes the appointment, creating appropriate logs and commlogs.  Pass in </summary>
		private void OnDelete_Click(bool isSkipDeletePrompt=false) {
			if(DoPreventCompletedApptChange(PreventChangesApptAction.Delete)) {
				return;
			}
			DateTime datePrevious=AptCur.DateTStamp;
			if (AptCur.AptStatus == ApptStatus.PtNote || AptCur.AptStatus == ApptStatus.PtNoteCompleted) {
				if(!isSkipDeletePrompt && !MsgBox.Show(this, MsgBoxButtons.OKCancel, "Delete Patient Note?")) {
					return;
				}
				if(textNote.Text != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(textNote.Text,AptCur.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog CommlogCur = new Commlog();
						CommlogCur.PatNum = AptCur.PatNum;
						CommlogCur.CommDateTime = DateTime.Now;
						CommlogCur.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						CommlogCur.Note = "Deleted Pt NOTE from schedule, saved copy: ";
						CommlogCur.Note += textNote.Text;
						CommlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(CommlogCur);
					}
				}
			}
			else {//ordinary appointment
				if (!isSkipDeletePrompt && MessageBox.Show(Lan.g(this, "Delete appointment?"), "", MessageBoxButtons.OKCancel) != DialogResult.OK) {
					return;
				}
				//Only want to be able to break already scheduled appointments, this does not include new appointments in "schedule" status.
				if(AptOld.AptNum!=0 && AptCur.AptStatus==ApptStatus.Scheduled && PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
					using FormApptBreakRequired formApptForceBreak=new FormApptBreakRequired();
					formApptForceBreak.ShowDialog();
					if(formApptForceBreak.DialogResult!=DialogResult.OK) {
						return;
					}
					AppointmentL.BreakApptHelper(AptCur,pat,formApptForceBreak.SelectedBrokenProcCode);
				}
				if(textNote.Text != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(textNote.Text,AptCur.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog CommlogCur = new Commlog();
						CommlogCur.PatNum = AptCur.PatNum;
						CommlogCur.CommDateTime = DateTime.Now;
						CommlogCur.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						CommlogCur.Note = "Deleted Appt. & saved note: ";
						if(AptCur.ProcDescript != "") {
							CommlogCur.Note += AptCur.ProcDescript + ": ";
						}
						CommlogCur.Note += textNote.Text;
						CommlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(CommlogCur);
					}
				}
				//If there is an existing HL7 def enabled with an outbound SIU message defined, this appointment has been inserted, and there is an outbound
				//message with AptCur.AptNum, send an SIU_S17 Appt Deletion message
				if(AptCur.AptNum>0 && HL7Defs.IsExistingHL7Enabled() && HL7Msgs.MessageWasSent(AptCur.AptNum)) {
					//S17 - Appt Deletion event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(pat,fam.GetPatient(pat.Guarantor),EventTypeHL7.S17,AptCur);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=AptCur.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=pat.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							MessageBox.Show(this,messageHL7.ToString());
						}
					}
				}
				if(AptCur.AptNum>0 && HieClinics.IsEnabled()) {//Ignore new appointment delete
					HieQueues.Insert(new HieQueue(pat.PatNum));
				}
			}
			_listAppointments.RemoveAll(x => x.AptNum==AptCur.AptNum);
			if(AptOld.AptStatus!=ApptStatus.Complete) { //seperate log entry for completed appointments
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,
					"Delete for date/time: "+AptCur.AptDateTime.ToString(),
					AptCur.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,pat.PatNum,
					"Delete for date/time: "+AptCur.AptDateTime.ToString(),
					AptCur.AptNum,datePrevious);
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
			Plugins.HookAddCode(this,"FormApptEdit.butDelete_Click_end",AptCur);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DateTime datePrevious=AptCur.DateTStamp;
			if(comboProv.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			if(AptOld.AptStatus!=ApptStatus.UnschedList && AptCur.AptStatus==ApptStatus.UnschedList) {
				//Extra log entry if the appt was sent to the unscheduled list
				Permissions perm=Permissions.AppointmentMove;
				if(AptOld.AptStatus==ApptStatus.Complete) {
					perm=Permissions.AppointmentCompleteEdit;
				}
				SecurityLogs.MakeLogEntry(perm,AptCur.PatNum,AptCur.ProcDescript+", "+AptCur.AptDateTime.ToString()
					+", Sent to Unscheduled List",AptCur.AptNum,datePrevious);
			}
			#region Validate Apt Start and End
			int minutes=contrApptProvSlider.Pattern.Length*5;
			//compare beginning of new appointment against end to see if they fall on different days
			if(AptCur.AptDateTime.Day!=AptCur.AptDateTime.AddMinutes(minutes).Day) {
				MsgBox.Show(this,"You cannot have an appointment that starts and ends on different days.");
				return;
			}
			#endregion
			if(!UpdateListAndDB(true,true,true)) {
				return;
			}
			AppointmentL.ShowKioskManagerIfNeeded(AptOld,AptCur.Confirmed);
			Plugins.HookAddCode(this,"FormApptEdit.butOK_Click_end",AptCur,AptOld,pat);
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
			if(AptCur==null) {//Could not find AptCur in the Db on load.
				return;
			}
			//Do not use pat.PatNum here.  Use AptCur.PatNum instead.  Pat will be null in the case that the user does not have the appt create permission.
			DateTime datePrevious=AptCur.DateTStamp;
			if(DialogResult!=DialogResult.OK) {
				if(AptCur.AptStatus==ApptStatus.Complete) {
					//This is a completed appointment and we need to warn the user if they are trying to leave the window and need to detach procs first.
					foreach(GridRow row in gridProc.ListGridRows) {
						bool attached=false;
						if(AptCur.AptStatus==ApptStatus.Planned && ((Procedure)row.Tag).PlannedAptNum==AptCur.AptNum) {
							attached=true;
						}
						else if(((Procedure)row.Tag).AptNum==AptCur.AptNum) {
							attached=true;
						}
						if(((Procedure)row.Tag).ProcStatus!=ProcStat.TP || !attached) {
							continue;
						}
						if(!Security.IsAuthorized(Permissions.AppointmentCompleteEdit,true)) {
							continue;
						}
						MsgBox.Show(this,"Detach treatment planned procedures or click OK in the appointment edit window to set them complete.");
						e.Cancel=true;
						return;
					}
				}
				if(IsNew) {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,AptCur.PatNum,
						"Create cancel for date/time: "+AptCur.AptDateTime.ToString(),
						AptCur.AptNum,datePrevious);
					//If cancel was pressed we want to un-do any changes to other appointments that were done.
					_listAppointments=Appointments.GetAppointmentsForProcs(_listProcsForAppt);
					//Add the current appointment if it is not in this list so it can get properly deleted by the sync later.
					if(!_listAppointments.Exists(x => x.AptNum==AptCur.AptNum)) {
						_listAppointments.Add(AptCur);
					}
					//We need to add this current appointment to the list of old appointments so we run the Appointments.Delete fucntion on it
					//This will remove any procedure connections that we created while in this window.
					_listAppointmentsOld=_listAppointments.Select(x => x.Copy()).ToList();
					//Now we also have to remove the appointment that was pre-inserted and is in this list as well so it is deleted on sync.
					_listAppointments.RemoveAll(x => x.AptNum==AptCur.AptNum);
				}
				else {  //User clicked cancel (or X button) on an existing appt
					AptCur=AptOld.Copy();  //We do not want to save any other changes made in this form.
					//Setting AptCur to a copy of AptOld causes the AptCur reference in _listAppointments to be lost. Remove and add back in so the sync below does not make 
					//any changes to AptCur. We had an issue with changes to AptCur were happening outside of the OK_Click method.
					_listAppointments.RemoveAll(x => x.AptNum==AptCur.AptNum);
					_listAppointments.Add(AptCur);
					if(AptCur.AptStatus==ApptStatus.Scheduled && PrefC.GetBool(PrefName.InsChecksFrequency) && !CheckFrequencies()) {
						e.Cancel=true;
						return;
					}
				}
			}
			else {//DialogResult==DialogResult.OK (User clicked OK or Delete)
				//Note that Procedures.Sync is never used.  This is intentional.  In order to properly use procedure.Sync logic in this form we would
				//need to enhance ProcEdit and all its possible child forms to also not insert into DB until OK is clicked.  This would be a massive undertaking
				//and as such we just immediately push changes to DB.
				if(AptCur.AptStatus==ApptStatus.Scheduled && !_isDeleted && PrefC.GetBool(PrefName.InsChecksFrequency) && !CheckFrequencies()) {
					e.Cancel=true;
					return;
				}
				if(AptCur.AptStatus==ApptStatus.Scheduled) {
					//find all procs that are currently attached to the appt that weren't when the form opened
					List<string> listProcCodes = _listProcsForAppt.FindAll(x => x.AptNum==AptCur.AptNum && !_listProcNumsAttachedStart.Contains(x.ProcNum))
						.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).Distinct().ToList();//get list of string proc codes
					AutomationL.Trigger(AutomationTrigger.ScheduleProcedure,listProcCodes,AptCur.PatNum);
				}
			}
			if(AptOld.AptStatus!=ApptStatus.Complete && AptCur.AptStatus==ApptStatus.Complete) {
				//If necessary, prompt the user to ask the patient to opt in to using Short Codes.
				FormShortCodeOptIn.PromptIfNecessary(pat,AptCur.ClinicNum);
			}
			//Sync detaches any attached procedures within Appointments.Delete() but doesn't create any ApptComm items.
			if(Appointments.Sync(_listAppointments,_listAppointmentsOld,AptCur.PatNum)) {
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,AptCur);
			}
			//Synch the recalls for this patient.  This is necessary in case the date of the appointment has change or has been deleted entirely.
			Recalls.Synch(AptCur.PatNum);
			Recalls.SynchScheduledApptFull(AptCur.PatNum);
		}
	}
}