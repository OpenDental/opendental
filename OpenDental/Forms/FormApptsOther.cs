using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using OpenDentBusiness.HL7;
using CodeBase;
using Microsoft.Office.Core;

namespace OpenDental {
	///<summary></summary>
	public partial class FormApptsOther:FormODBase {
		#region Fields - Public
		///<summary>Set to true to allow selecting appointments.</summary>
		public bool AllowSelectOnly;
		///<summary>DateTime that the user clicked in the Appt module before arriving here. Pass in if IsInitialDoubleClick.</summary>
		public DateTime DateTimeClicked;
		public DateTime DateTNew;
		///<summary>True if user double clicked on a blank area of appt module to get to this point. Also, pass in DateTimeClicked and OpNumClicked.</summary>
		public bool IsInitialDoubleClick;
		public bool IsShowCompletePlanned;
		///<summary>After closing, this may contain aptNums of appointments that should be placed on the pinboard. Used when picking appointment for task lists or when the GoTo, Create new, or Recall buttons are pushed.</summary>
		public List<long> ListAptNumsSelected;
		public List<ApptOther> ListApptOthers=new List<ApptOther>();
		public long OpNumNew;
		///<summary>OpNum that the user clicked in the Appt module before arriving here. Pass in if IsInitialDoubleClick.</summary>
		public long OpNumClicked;
		public long PatNumSelected;
		///<summary>After closing, if OResult=PinboardAndSearch then this will contain the date to jump to when beginning the search. If OResult=GoTo then this will also contain the date.  Can't use DateTime type because C# complains about marshal by reference.</summary>
		public string StringDateJumpTo;
		///<summary>List of appointment view nums that contain the operatory num for the appointment to jump to.</summary>
		public List<long> ListAptViewJumpTos=new List<long>();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>The result of the window.  In other words, which button was clicked to exit the window.</summary>
		private Family _family;
		private bool _isFullRefresh;
		private List<Def> _listDefsProgNoteColors=new List<Def>();
		private List<PlannedAppt> _listPlannedAppts=new List<PlannedAppt>();
		private List<PlannedAppt> _listPlannedApptsIncomplete=new List<PlannedAppt>();
		private List<Recall> _listRecalls;
		private OtherResult _otherResult;
		private Patient _patient;
		///<summary>List of Appointment.AptNum that are on the pinboard when FormApptsOther was loaded.</summary>
		private List<long> _listPinboardApptNums;
		private Action _actionFillFamily;
		#endregion Fields - Private

		///<summary>Pass in list of Appointment.AptNum for appointments that are currently on the pinboard if not in AllowSelectOnly mode.</summary>
		public FormApptsOther(long patNum,List<long> listPinboardApptNums) {
			InitializeComponent();
			InitializeLayoutManager();
			_family=Patients.GetFamily(patNum);
			_patient=_family.GetPatient(patNum);
			Lan.F(this);
			for(int i=0;i<listViewFamily.Columns.Count;i++) {
				listViewFamily.Columns[i].Text=Lan.g(this,listViewFamily.Columns[i].Text);
			}
			ListAptNumsSelected=new List<long>();
			SetFillFamilyAction(() => FillFamily());
			_listPinboardApptNums=listPinboardApptNums??new List<long>();
		}

		///<summary>Gets FormApptsOther's result when closing. Meaning, which button was clicked to exit the window.</summary>
		public OtherResult GetOtherResult() {
			return _otherResult;
		}

		private void FormApptsOther_Load(object sender, System.EventArgs e) {
			Text=Lan.g(this,"Appointments for")+" "+_patient.GetNameLF();
			textApptModNote.Text=_patient.ApptModNote;
			if(AllowSelectOnly) {
				butGoTo.Visible=false;
				butPin.Visible=false;
				butNew.Visible=false;
				butRecall.Visible=false;
				butRecallFamily.Visible=false;
				label2.Visible=false;
				listViewFamily.Visible=false;
			}
			else {
				butOK.Visible=false;
			}
			FillFamily();
			RefreshAppts();
			ScrollToEnd();
			CheckStatus();
		}

		private void CheckStatus() {
			if (_patient.PatStatus==PatientStatus.Inactive
				|| _patient.PatStatus==PatientStatus.Archived
				|| _patient.PatStatus==PatientStatus.Prospective)
			{
				MsgBox.Show(this, "Warning. Patient is not active.");
			}
			if (_patient.PatStatus==PatientStatus.Deceased) {
				MsgBox.Show(this, "Warning. Patient is deceased.");
			}
		}

		private void FillFamily() {
			PatNumSelected=_patient.PatNum;//just in case user has selected a different family member
			_listRecalls=Recalls.GetList(_family.ListPats.ToList());
			//Appointment[] aptsOnePat;
			List<PatientLink> listPatientLinks=PatientLinks.GetLinks(_family.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			listViewFamily.Items.Clear();
			ListViewItem listViewItem;
			DateTime dateDue;
			DateTime dateSched;
			for(int i=0;i<_family.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(_family.ListPats[i].PatNum,listPatientLinks)) {
					continue;//Do not include Merged patients in the displayed list.
				}
				listViewItem=new ListViewItem(_family.GetNameInFamFLI(i));
				listViewItem.Tag=_family.ListPats[i];
				if(_family.ListPats[i].PatNum==_patient.PatNum) {
					listViewItem.BackColor=Color.Silver;
				}
				listViewItem.SubItems.Add(_family.ListPats[i].Age.ToString());
				listViewItem.SubItems.Add(_family.ListPats[i].Gender.ToString());
				dateDue=DateTime.MinValue;
				dateSched=DateTime.MinValue;
				bool isdisabled=false;
				for(int j=0;j<_listRecalls.Count;j++) {
					if(_listRecalls[j].PatNum==_family.ListPats[i].PatNum) {
						if(_listRecalls[j].RecallTypeNum==RecallTypes.PerioType || _listRecalls[j].RecallTypeNum==RecallTypes.ProphyType) {
							dateDue=_listRecalls[j].DateDue;
							dateSched=_listRecalls[j].DateScheduled;
							isdisabled=_listRecalls[j].IsDisabled;
						}
					}
				}
				if(isdisabled) {
					listViewItem.SubItems.Add(Lan.g(this,"disabled"));
				}
				else if(dateDue.Year<1880) {
					listViewItem.SubItems.Add("");
				}
				else {
					listViewItem.SubItems.Add(dateDue.ToShortDateString());
				}
				if(dateDue<=DateTime.Today) {
					listViewItem.ForeColor=Color.Red;
				}
				if(dateSched.Year<1880) {
					listViewItem.SubItems.Add("");
				}
				else{
					listViewItem.SubItems.Add(dateSched.ToShortDateString());
				}
				listViewFamily.Items.Add(listViewItem);
			}
			checkDone.Checked=_patient.PlannedIsDone;
			textFinUrg.Text=_family.ListPats[0].FamFinUrgNote;
		}

		private void listFamily_DoubleClick(object sender, System.EventArgs e) {
			if(listViewFamily.SelectedIndices.Count==0) {
				return;
			}
			using FormRecallsPat formRecallsPat=new FormRecallsPat();
			formRecallsPat.PatNum=_patient.PatNum;
			formRecallsPat.ShowDialog();
		}

		private void checkShowCompletePlanned_CheckedChanged(object sender,EventArgs e) {
			IsShowCompletePlanned=checkShowCompletePlanned.Checked;
			RefreshAppts();
		}

		private void butRecall_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Patient patientMerged=AppointmentL.GetPatientMergePrompt(_patient.PatNum);
			if(patientMerged!=null) {
				_patient=patientMerged;
				FillFamily();
				RefreshAppts();
				FormOpenDental.S_Contr_PatientSelected(_patient,isRefreshCurModule: true,isApptRefreshDataPat: false);
			}
			if(_patient!=null && _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+_patient.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			MakeRecallAppointment();
		}

		///<summary>Creates a single recall appointment. If it's from a double click, then it will end up on that spot in the Appts module.  If not, it will end up on the pinboard with StringDateJumpTo as due date to jump to.  ListAptNumsSelected will contain the AptNum of the new appointment.</summary>
		public void MakeRecallAppointment() {
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			Appointment appointment=null;
			DateTime dateTimeApt=DateTime.MinValue;
			if(this.IsInitialDoubleClick) {
				dateTimeApt=DateTimeClicked;
			}
			try{
				appointment=AppointmentL.CreateRecallApt(_patient,listInsPlans,-1,listInsSubs,dateTimeApt);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DateTime datePrevious=appointment.DateTStamp;
			ListAptNumsSelected.Add(appointment.AptNum);
			if(IsInitialDoubleClick) {
				Appointment appointmentOld=appointment.Copy();
				if(_patient.AskToArriveEarly>0) {
					appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-_patient.AskToArriveEarly);
					MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+_patient.AskToArriveEarly
						+" "+Lan.g(this,"minutes early at")+" "+appointment.DateTimeAskedToArrive.ToShortTimeString()+".");
				}
				appointment.AptStatus=ApptStatus.Scheduled;
				appointment.ClinicNum=_patient.ClinicNum;
				appointment.Op=OpNumClicked;
				appointment=Appointments.AssignFieldsForOperatory(appointment);
				//Use apt.ClinicNum because it was just set based on Op.ClinicNum in AssignFieldsForOperatory().
				if(!AppointmentL.IsSpecialtyMismatchAllowed(_patient.PatNum,appointment.ClinicNum)) {
					return;
				}
				Appointments.Update(appointment,appointmentOld);
				Appointments.TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);
				_otherResult=OtherResult.CreateNew;
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCreate,appointment.PatNum,appointment.AptDateTime.ToString(),appointment.AptNum,datePrevious);
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S12 - New Appt Booking event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(_patient,_family.GetPatient(_patient.Guarantor),EventTypeHL7.S12,appointment);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=appointment.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=_patient.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							MessageBox.Show(this,messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(_patient.PatNum));
				}
				Recalls.Synch(appointment.PatNum);
				Recalls.SynchScheduledApptFull(appointment.PatNum);
				DialogResult=DialogResult.OK;
				return;
			}
			//not initialClick
			_otherResult=OtherResult.PinboardAndSearch;
			Recall recall=Recalls.GetRecallProphyOrPerio(_patient.PatNum);//shouldn't return null.
			if(recall.DateDue<DateTime.Today) {
				StringDateJumpTo=DateTime.Today.ToShortDateString();//they are overdue
			}
			else{
				StringDateJumpTo=recall.DateDue.ToShortDateString();
			}
			//no securitylog entry needed here.  That will happen when it's dragged off pinboard.
			DialogResult=DialogResult.OK;
		}

		private void butRecallFamily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			MakeRecallFamily();
		}

		///<summary>Creates appointments for each patient in _famCur.  MsgBox informs user of anyone skipped.  StringDateJumpTo will contain the due date (of the last family member) to jump to.  ListAptNumsSelected will contain the AptNums of the new appointments on the pinboard.</summary>
		public void MakeRecallFamily() {
			List<Recall> listRecallsPat;
			List <InsPlan> listInsPlans;
			List<InsSub> listInsSubs;
			Appointment appointment=null;
			Recall recall;
			int countAlreadySched=0;
			int countNoRecalls=0;
			int countPatsRestricted=0;
			int countPatsArchivedOrDeceased=0;
			for(int i=0;i<_family.ListPats.Length;i++) {
				Patient patientCur=_family.ListPats[i];
				if(PatRestrictionL.IsRestricted(patientCur.PatNum,PatRestrict.ApptSchedule,suppressMessage: true)) {
					countPatsRestricted++;
					continue;
				}
				if(patientCur.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
					countPatsArchivedOrDeceased++;
					continue;
				}
				listRecallsPat=Recalls.GetList(patientCur.PatNum);//get the recall for this pt
				//Check to see if the special type recall is disabled or already scheduled.  This is also done in AppointmentL.CreateRecallApt() below so I'm
				//	not sure why we do it here.
				List<Recall> listRecalls=listRecallsPat.FindAll(x => x.RecallTypeNum==RecallTypes.PerioType || x.RecallTypeNum==RecallTypes.ProphyType);
				if(listRecalls.Count==0 || listRecalls.Exists(x => x.IsDisabled)) {
					countNoRecalls++;
					continue;
				}
				if(listRecalls.Exists(x => x.DateScheduled.Year > 1880)) {
					countAlreadySched++;
					continue;
				}
				listInsSubs=InsSubs.RefreshForFam(_family);
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				try {
					appointment=AppointmentL.CreateRecallApt(patientCur,listInsPlans,-1,listInsSubs);
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue;
				}
				ListAptNumsSelected.Add(appointment.AptNum);
				_otherResult=OtherResult.PinboardAndSearch;
				recall=Recalls.GetRecallProphyOrPerio(patientCur.PatNum);//should not return null
				if(recall.DateDue<DateTime.Today) {
					StringDateJumpTo=DateTime.Today.ToShortDateString();//they are overdue
				}
				else {
					StringDateJumpTo=recall.DateDue.ToShortDateString();
				}
				//Log will be made when appointment dragged off of the pinboard.
				//SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,apt.PatNum,apt.AptDateTime.ToString(),apt.AptNum);
			}
			List<string> listUserMsgs=new List<string>();
			if(countPatsRestricted > 0) {
				listUserMsgs.Add(Lan.g(this,"Family members skipped due to patient restriction")+" "
					+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+": "+countPatsRestricted+".");
			}
			if(countNoRecalls > 0) {
				listUserMsgs.Add(Lan.g(this,"Family members skipped because recall disabled")+": "+countNoRecalls+".");
			}
			if(countAlreadySched > 0) {
				listUserMsgs.Add(Lan.g(this,"Family members skipped because already scheduled")+": "+countAlreadySched+".");
			}
			if(countPatsArchivedOrDeceased > 0) {
				listUserMsgs.Add(Lan.g(this,"Family members skipped because status is archived or deceased")+": "+countPatsArchivedOrDeceased+".");
			}
			if(ListAptNumsSelected.Count==0) {
				listUserMsgs.Add(Lan.g(this,"There are no recall appointments to schedule."));
			}
			if(listUserMsgs.Count > 0) {
				MessageBox.Show(string.Join("\r\n",listUserMsgs));
				if(ListAptNumsSelected.Count==0) {
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butNote_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Appointment appointment=new Appointment();
			appointment.PatNum=_patient.PatNum;
			if(_patient.DateFirstVisit.Year < 1880
				&& !Procedures.AreAnyComplete(_patient.PatNum))//this only runs if firstVisit blank
			{
				appointment.IsNewPatient=true;
			}
			appointment.Pattern="/X/";
			if(_patient.PriProv==0) {
				appointment.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				appointment.ProvNum=_patient.PriProv;
			}
			appointment.ProvHyg=_patient.SecProv;
			appointment.AptStatus=ApptStatus.PtNote;
			appointment.ClinicNum=_patient.ClinicNum;
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			if(IsInitialDoubleClick) {//initially double clicked on appt module
				appointment.AptDateTime=DateTimeClicked;
				appointment.Op=OpNumClicked;
			}
			else {
				//new appt will be placed on pinboard instead of specific time
			}
			try {
				if(!AppointmentL.IsSpecialtyMismatchAllowed(_patient.PatNum,appointment.ClinicNum)) {
					return;
				}
				Appointments.Insert(appointment);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(appointment.AptNum);
			formApptEdit.IsNew=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			ListAptNumsSelected.Add(appointment.AptNum);
			if(IsInitialDoubleClick) {
				_otherResult=OtherResult.CreateNew;
			}
			else {
				_otherResult=OtherResult.NewToPinBoard;
			}
			DialogResult=DialogResult.OK;

		}

		private void butNew_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Patient patientMerged=AppointmentL.GetPatientMergePrompt(_patient.PatNum);
			if(patientMerged!=null) {
				_patient=patientMerged;
				Text=Lan.g(this,"Appointments for")+" "+_patient.GetNameLF();
				FillFamily();
				RefreshAppts();
				FormOpenDental.S_Contr_PatientSelected(_patient,
					isRefreshCurModule: true,
					isApptRefreshDataPat: true);//isApptRefreshDataPat: true because of a scenario where the user does switch from a merged patient but does not end up making an appointment. 
			}
			if(_patient!=null && _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+_patient.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			MakeAppointment();
		}

		///<summary>Offers to use unscheduled appt.  Shows ApptEdit window. Sets Prospective, if necessary.  Fires Automation triggers.  ListAptNumsSelected will contain the AptNum of the new appointment.</summary>
		public void MakeAppointment() {
			//Check to see if the patient has any unscheduled appointments and inform the user.
			List<Appointment> listAppointmentsUnsched=Appointments.GetUnschedApptsForPat(_patient.PatNum);
			//Per Nathan, pinboard appointments will not be considered unscheduled for this logic.
			listAppointmentsUnsched.RemoveAll(x => _listPinboardApptNums.Contains(x.AptNum));
			long aptNum=0;
			bool isSchedulingUnscheduled=false;
			if(listAppointmentsUnsched.Count>0 &&
				 MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient has an unscheduled appointment, would you like to use an existing unscheduled appointment?"))
			{
				if(listAppointmentsUnsched.Count==1) {
					aptNum=listAppointmentsUnsched[0].AptNum;
				}
				else {//Multiple unscheduled appointments, let the user pick which one to use.
					using FormUnschedListPatient formUnschedListPatient=new FormUnschedListPatient(_patient);
					if(formUnschedListPatient.ShowDialog()!=DialogResult.OK) {
						return;
					}
					//Use the appointment the user selected.
					aptNum=formUnschedListPatient.Appointment.AptNum;
				}
				isSchedulingUnscheduled=true;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum,patNum:_patient.PatNum,useApptDrawingSettings:IsInitialDoubleClick,patient:_patient,dateTNew:DateTNew,opNumNew:OpNumNew);
			formApptEdit.IsNew=(aptNum==0);
			formApptEdit.IsSchedulingUnscheduledAppt(isSchedulingUnscheduled);
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			Appointment appointment=formApptEdit.GetAppointmentCur();
			if(IsInitialDoubleClick) {
				if(isSchedulingUnscheduled) {//User double clicked in Appointment Module, intending to schedule appointment at a specific time/op/etc.
					Appointment appointmentOld=appointment.Copy();
					appointment.AptDateTime=DateTimeClicked;
					appointment.Op=OpNumClicked;
					if(_patient!=null && _patient.AskToArriveEarly>0) {
						appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-_patient.AskToArriveEarly);
					}
					appointment=Appointments.AssignFieldsForOperatory(appointment);
					appointment.AptStatus=ApptStatus.Scheduled;
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentMove,appointment.PatNum,
						appointment.ProcDescript+", from "+appointmentOld.AptDateTime.ToString()+", to "+appointment.AptDateTime.ToString(),
						appointment.AptNum,appointmentOld.DateTStamp);
					Appointments.Update(appointment,appointmentOld);
					Appointments.TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);
				}
				//Change PatStatus to Prospective or from Prospective.
				Operatory operatory=Operatories.GetOperatory(appointment.Op);
				if(operatory!=null) {
					if(operatory.SetProspective && _patient.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
							Patient patOld=_patient.Copy();
							_patient.PatStatus=PatientStatus.Prospective;
							Patients.UpdateRecalls(_patient,patOld,"Appointment Module, Patient appointment created in prospective operatory");
							Patients.Update(_patient,patOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+_patient.PatStatus.GetDescription()+Lan.g(this," by making an appointment from the Other Appointments window.");
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,logEntry);
						}
					}
					else if(!operatory.SetProspective && _patient.PatStatus==PatientStatus.Prospective) {
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
							Patient patientOld=_patient.Copy();
							_patient.PatStatus=PatientStatus.Patient;
							Patients.UpdateRecalls(_patient,patientOld,"Appointment Module, Prospective Patient appointment created in non-prospective operatory");
							Patients.Update(_patient,patientOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+_patient.PatStatus.GetDescription()+Lan.g(this," by making an appointment from the Other Appointments window.");
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,logEntry);
						}
					}
				}
			}
			ListAptNumsSelected.Add(appointment.AptNum);
			if(IsInitialDoubleClick) {
				_otherResult=OtherResult.CreateNew;
			}
			else{
				_otherResult=OtherResult.NewToPinBoard;
			}
			if(appointment.IsNewPatient) {
				AutomationL.Trigger(EnumAutomationTrigger.ApptNewPatCreate,listProcCodes: null,appointment.PatNum,appointment.AptNum);
			}
			AutomationL.Trigger(EnumAutomationTrigger.ApptCreate,listProcCodes: null,appointment.PatNum,appointment.AptNum);
			DialogResult=DialogResult.OK;
		}

		private void butPin_Click(object sender, System.EventArgs e) {
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==-1) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			ApptOther apptOther=ListApptOthers.FirstOrDefault(x => x.AptNum==gridMain.SelectedTag<long>());
			if(IsSelectedApptOtherNull()) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(!AppointmentL.OKtoSendToPinboard(apptOther,ListApptOthers,this)) {//Tag is AptNum
				return;
			}
			ListAptNumsSelected.Add(apptOther.AptNum);
			_otherResult=OtherResult.CopyToPinBoard;
			DialogResult=DialogResult.OK;
		}

		private void listFamily_Click(object sender,EventArgs e) {
			//Changes the patient to whoever was clicked in the list 
			if(listViewFamily.SelectedIndices.Count==0) {
				return;
			}
			long patNumOld=_patient.PatNum;
			long patNumNew=((Patient)listViewFamily.SelectedItems[0].Tag).PatNum;
			if(patNumNew==patNumOld) {
				return;
			}
			_patient=_family.GetPatient(patNumNew);
			Text=Lan.g(this,"Appointments for")+" "+_patient.GetNameLF();
			textApptModNote.Text=_patient.ApptModNote;
			FillFamily();
			RefreshAppts();
			CheckStatus();
		}

		private void UpdateTextApptModNote() {
			if(textApptModNote.Text!=_patient.ApptModNote) {
				Patient patientOld=_patient.Copy();
				_patient.ApptModNote=textApptModNote.Text;
				Patients.Update(_patient,patientOld);
			}
		}

		private void textApptModNote_Leave(object sender,EventArgs e) {
			UpdateTextApptModNote();
		}

		private void butGoTo_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			if(IsSelectedApptOtherNull()) {
				return;
			}
			ApptOther apptOtherSelected=ListApptOthers.FirstOrDefault(x => x.AptNum==gridMain.SelectedTag<long>());
			if(apptOtherSelected==null) {
				MsgBox.Show(this,"Unable to go to appointment.");
				return;
			}
			if(apptOtherSelected.AptDateTime.Year<1880) {
				MsgBox.Show(this,"Unable to go to unscheduled appointment.");
				return;
			}
			ListAptNumsSelected.Add(apptOtherSelected.AptNum);
			StringDateJumpTo=apptOtherSelected.AptDateTime.Date.ToShortDateString();
			ListAptViewJumpTos=ApptViewItems.GetViewsByOp(apptOtherSelected.Op);
			_otherResult=OtherResult.GoTo;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//only visible when selecting from TaskList. oResult is completely ignored in this case.
			//I didn't bother enabling double click. Maybe later.
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			ApptOther apptOther=ListApptOthers.FirstOrDefault(x => x.AptNum==gridMain.SelectedTag<long>());
			ListAptNumsSelected.Add(apptOther.AptNum);
			DialogResult=DialogResult.OK;
		}

		private void FormApptsOther_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			UpdateTextApptModNote();
			if(DialogResult==DialogResult.OK) {
				return;
			}
			_otherResult=OtherResult.Cancel;
		}

		private void FillGrid() {
			gridMain.SetScaleAndZoom(LayoutManager.GetScaleMS(),LayoutManager.GetZoomLocal());
			long selectedApptOtherNum=gridMain.GetSelectedIndex();
			int selectedIndex=-1;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormApptsOther","Appt Status"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Prov"),50);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("FormApptsOther","Clinic"),80);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("FormApptsOther","Date"),70);//If the order changes, reflect the change for dateIndex below.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Time"),70);//Must immediately follow Date column.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Min"),40);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Procedures"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormApptsOther","Notes"),320);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			int idxDate=3;
			if(!PrefC.HasClinicsEnabled) {
				idxDate=2;
			}
			for(int i=0;i<ListApptOthers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListApptOthers[i].AptStatus.ToString());
				row.Cells.Add(Providers.GetAbbr(ListApptOthers[i].ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(ListApptOthers[i].ClinicNum));
				}
				row.Cells.Add("");//Date
				row.Cells.Add("");//Time
				if(ListApptOthers[i].AptDateTime.Year > 1880) {
					//only regular still scheduled appts
					if(ListApptOthers[i].AptStatus!=ApptStatus.Planned && ListApptOthers[i].AptStatus!=ApptStatus.PtNote 
						&& ListApptOthers[i].AptStatus!=ApptStatus.PtNoteCompleted && ListApptOthers[i].AptStatus!=ApptStatus.UnschedList 
						&& ListApptOthers[i].AptStatus!=ApptStatus.Broken) 
					{
						row.Cells[idxDate].Text=ListApptOthers[i].AptDateTime.ToString("d");
						row.Cells[idxDate+1].Text=ListApptOthers[i].AptDateTime.ToString("t");
						if(ListApptOthers[i].AptDateTime < DateTime.Today) { //Past
							row.ColorBackG=_listDefsProgNoteColors[11].ItemColor;
							row.ColorText=_listDefsProgNoteColors[10].ItemColor;
						}
						else if(ListApptOthers[i].AptDateTime.Date==DateTime.Today.Date) { //Today
							row.ColorBackG=_listDefsProgNoteColors[9].ItemColor;
							row.ColorText=_listDefsProgNoteColors[8].ItemColor;
							row.Cells[0].Text=Lan.g(this,"Today");
						}
						else if(ListApptOthers[i].AptDateTime > DateTime.Today) { //Future
							row.ColorBackG=_listDefsProgNoteColors[13].ItemColor;
							row.ColorText=_listDefsProgNoteColors[12].ItemColor;
						}
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.Planned) { //show line for planned appt
						row.ColorBackG=_listDefsProgNoteColors[17].ItemColor;
						row.ColorText=_listDefsProgNoteColors[16].ItemColor;
						string strText=Lan.g("enumApptStatus","Planned")+" ";
						int idxPlannedApt=_listPlannedApptsIncomplete.FindIndex(x => x.AptNum==ListApptOthers[i].AptNum);
						if(IsShowCompletePlanned) {
							for(int p=0;p<_listPlannedAppts.Count;p++) {
								if(_listPlannedAppts[p].AptNum==ListApptOthers[i].AptNum) {
									strText+="#"+_listPlannedAppts[p].ItemOrder.ToString();
								}
							}
						}
						else {
							if(idxPlannedApt>=0) {
								strText+="#"+(idxPlannedApt+1);
							}
							else {
								continue;
							}
						}
						if(idxPlannedApt<0) {//attached to a completed appointment
							strText+=" ("+Lan.g("enumApptStatus",ApptStatus.Complete.ToString())+")";
						}
						if(ListApptOthers.FindAll(x => x.NextAptNum==ListApptOthers[i].AptNum)
							.Exists(x => x.AptStatus==ApptStatus.Scheduled)) //attached to a scheduled appointment
						{
							strText+=" ("+Lan.g("enumApptStatus",ApptStatus.Scheduled.ToString())+")";
						}
						row.Cells[0].Text=strText;
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.PtNote) {
						row.ColorBackG=_listDefsProgNoteColors[19].ItemColor;
						row.ColorText=_listDefsProgNoteColors[18].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNote");
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.PtNoteCompleted) {
						row.ColorBackG=_listDefsProgNoteColors[21].ItemColor;
						row.ColorText=_listDefsProgNoteColors[20].ItemColor;
						row.Cells[0].Text=Lan.g("enumApptStatus","PtNoteCompleted");
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.Broken) {
						row.Cells[0].Text=Lan.g("enumApptStatus","Broken");
						row.Cells[idxDate].Text=ListApptOthers[i].AptDateTime.ToString("d");
						row.Cells[idxDate+1].Text=ListApptOthers[i].AptDateTime.ToString("t");
						row.ColorBackG=_listDefsProgNoteColors[15].ItemColor;
						row.ColorText=_listDefsProgNoteColors[14].ItemColor;
					}
					else if(ListApptOthers[i].AptStatus==ApptStatus.UnschedList) {
						row.Cells[0].Text=Lan.g("enumApptStatus","UnschedList");
						row.ColorBackG=_listDefsProgNoteColors[15].ItemColor;
						row.ColorText=_listDefsProgNoteColors[14].ItemColor;
					}
				}
				row.Cells.Add((ListApptOthers[i].Pattern.Length * 5).ToString());
				row.Cells.Add(ListApptOthers[i].ProcDescript);
				row.Cells.Add(ListApptOthers[i].Note);
				row.Tag=ListApptOthers[i].AptNum;
				gridMain.ListGridRows.Add(row);
				if((long)row.Tag==selectedApptOtherNum) {
					//we will not use i as the index because there are "continue"s in this loop
					selectedIndex=gridMain.ListGridRows.Count-1;//select the row that was just added if it matches
				}
			}
			gridMain.EndUpdate();
			if(selectedIndex>-1) {
				gridMain.SetSelected(selectedIndex,true);
			}
		}

		public void SetFillFamilyAction(Action action) {
			_actionFillFamily=action;
		}

		public void RefreshAppts() {
			RefreshData();
			RefreshView();
		}

		public void RefreshData() {
			if(_patient==null) {
				return;
			}
			ListApptOthers=Appointments.GetApptOthersForPat(_patient.PatNum);
			_listPlannedAppts=PlannedAppts.Refresh(_patient.PatNum);
			int offset=0;
			if(_listPlannedAppts.Count > 0) {
				offset=_listPlannedAppts.Max(x => x.ItemOrder)+1;
			}
			ListApptOthers=ListApptOthers.OrderBy(appt => {
				var plannedAppt=_listPlannedAppts.FirstOrDefault(x => x.AptNum==appt.AptNum);
				return plannedAppt?.ItemOrder??(offset+ListApptOthers.IndexOf(appt));//Place planned appts above other appts in the case of a patient having a longer history of scheduled appts
			}).ToList();
			_listPlannedApptsIncomplete=_listPlannedAppts.FindAll(x => !ListApptOthers.ToList()
				.Exists(y => y.NextAptNum==x.AptNum && y.AptStatus==ApptStatus.Complete))
				.OrderBy(x => x.ItemOrder).ToList();
			_listDefsProgNoteColors=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			_isFullRefresh=true;
		}

		public void RefreshView() {
			FillGrid();
			if(_isFullRefresh) {
				gridMain.ScrollToEnd();
				_isFullRefresh=false;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;
			int currentScroll=gridMain.ScrollValue;
			long aptNum=gridMain.SelectedTag<long>();//Tag is AptNum
			if(IsSelectedApptOtherNull()) {
				return;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum);
			formApptEdit.IsInViewPatAppts=true;
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formApptEdit.PinClicked) {
				if(IsSelectedApptOtherNull()) {
					return;
				}
				ApptOther apptOther=ListApptOthers.FirstOrDefault(x => x.AptNum==(long)gridMain.ListGridRows[e.Row].Tag);
				SendToPinboardEvent.Fire(ODEventType.SendToPinboard,new PinBoardArgs(_patient,apptOther,ListApptOthers));
				return;
			}
			RefreshData();
			_actionFillFamily?.Invoke();
			FillGrid();
			gridMain.SetSelected(currentSelection,true);
			gridMain.ScrollValue=currentScroll;
		}

		///<summary>Returns true if SelectedApptOther is null or is set to an appointment with an AptNum that is not in the database.
		///If SelectedApptOther was null or set to a valid appointment but is now invalid (no longer in the database) then a warning message will display
		///to the user and RefreshAppts will be invoked in order to remove the invalid appt from the UI.</summary>
		public bool IsSelectedApptOtherNull() {
			int idxSelected=gridMain.GetSelectedIndex();
			Appointment appointment=null;
			if(idxSelected!=-1) {
				ApptOther apptOther=ListApptOthers.FirstOrDefault(x => x.AptNum==gridMain.SelectedTag<long>());
				appointment=Appointments.GetOneApt(apptOther.AptNum);
			}
			if(idxSelected==-1 || appointment==null) {
				MsgBox.Show(this,"Selected appointment no longer exists.");
				RefreshAppts();
				return true;
			}
			return false;
		}

		public void ScrollToEnd() {
			gridMain.ScrollToEnd();
		}

	
	}
}
