using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using OpenDentBusiness.HL7;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormApptsOther : FormODBase {
		#region Fields - Public
		///<summary>Set to true to allow selecting appointments.</summary>
		public bool AllowSelectOnly;
		///<summary>DateTime that the user clicked in the Appt module before arriving here. Pass in if IsInitialDoubleClick.</summary>
		public DateTime DateTimeClicked;
		public DateTime DateTNew;
		///<summary>True if user double clicked on a blank area of appt module to get to this point. Also, pass in DateTimeClicked and OpNumClicked.</summary>
		public bool IsInitialDoubleClick;
		///<summary>After closing, this may contain aptNums of appointments that should be placed on the pinboard. Used when picking appointment for task lists or when the GoTo, Create new, or Recall buttons are pushed.</summary>
		public List<long> ListAptNumsSelected;
		public long OpNumNew;
		///<summary>OpNum that the user clicked in the Appt module before arriving here. Pass in if IsInitialDoubleClick.</summary>
		public long OpNumClicked;
		///<summary>After closing, this will be the patNum of the last patient viewed.  Calling forms should make use of this to refresh to that patient. Calling form should not refresh if this is set to 0.</summary>
		public long PatNumSelected;
		///<summary>After closing, if OResult=PinboardAndSearch then this will contain the date to jump to when beginning the search. If OResult=GoTo then this will also contain the date.  Can't use DateTime type because C# complains about marshal by reference.</summary>
		public string StringDateJumpTo;
		///<summary>List of appointment view nums that contain the operatory num for the appointment to jump to.</summary>
		public List<long> ListAptViewJumpTos=new List<long>();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>The result of the window.  In other words, which button was clicked to exit the window.</summary>
		private Family _famCur;
		private List<Recall> _listRecalls;
		private OtherResult _otherResult;
		private Patient _patCur;
		///<summary>List of Appointment.AptNum that are on the pinboard when FormApptsOther was loaded.</summary>
		private List<long> _listPinboardApptNums;
		#endregion Fields - Private

		///<summary>Pass in list of Appointment.AptNum for appointments that are currently on the pinboard if not in AllowSelectOnly mode.</summary>
		public FormApptsOther(long patNum,List<long> listPinboardApptNums) {
			InitializeComponent();
			InitializeLayoutManager();
			_famCur=Patients.GetFamily(patNum);
			_patCur=_famCur.GetPatient(patNum);
			Lan.F(this);
			for(int i=0;i<listViewFamily.Columns.Count;i++) {
				listViewFamily.Columns[i].Text=Lan.g(this,listViewFamily.Columns[i].Text);
			}
			ListAptNumsSelected=new List<long>();
			odApptGrid.PatCur=_patCur;
			odApptGrid.SetFillFamilyAction(() => FillFamily());
			_listPinboardApptNums=listPinboardApptNums??new List<long>();
		}

		///<summary></summary>
		public OtherResult OResult {
			get { return _otherResult; }
		}

		private void FormApptsOther_Load(object sender, System.EventArgs e) {
			Text=Lan.g(this,"Appointments for")+" "+_patCur.GetNameLF();
			textApptModNote.Text=_patCur.ApptModNote;
			if(AllowSelectOnly) {
				butGoTo.Visible=false;
				butPin.Visible=false;
				butNew.Visible=false;
				label2.Visible=false;
				listViewFamily.Visible=false;
			}
			else {
				butOK.Visible=false;
			}
			FillFamily();
			odApptGrid.RefreshAppts();
			odApptGrid.ScrollToEnd();
			CheckStatus();
		}

		private void CheckStatus() {
			if (_patCur.PatStatus==PatientStatus.Inactive
				|| _patCur.PatStatus==PatientStatus.Archived
				|| _patCur.PatStatus==PatientStatus.Prospective)
			{
				MsgBox.Show(this, "Warning. Patient is not active.");
			}
			if (_patCur.PatStatus==PatientStatus.Deceased) {
				MsgBox.Show(this, "Warning. Patient is deceased.");
			}
		}

		private void FillFamily() {
			PatNumSelected=_patCur.PatNum;//just in case user has selected a different family member
			_listRecalls=Recalls.GetList(_famCur.ListPats.ToList());
			//Appointment[] aptsOnePat;
			List<PatientLink> listLinks=PatientLinks.GetLinks(_famCur.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			listViewFamily.Items.Clear();
			ListViewItem item;
			DateTime dateDue;
			DateTime dateSched;
			for(int i=0;i<_famCur.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(_famCur.ListPats[i].PatNum,listLinks)) {
					continue;//Do not include Merged patients in the displayed list.
				}
				item=new ListViewItem(_famCur.GetNameInFamFLI(i));
				item.Tag=_famCur.ListPats[i];
				if(_famCur.ListPats[i].PatNum==_patCur.PatNum) {
					item.BackColor=Color.Silver;
				}
				item.SubItems.Add(_famCur.ListPats[i].Age.ToString());
				item.SubItems.Add(_famCur.ListPats[i].Gender.ToString());
				dateDue=DateTime.MinValue;
				dateSched=DateTime.MinValue;
				bool isdisabled=false;
				for(int j=0;j<_listRecalls.Count;j++) {
					if(_listRecalls[j].PatNum==_famCur.ListPats[i].PatNum
						&& (_listRecalls[j].RecallTypeNum==RecallTypes.PerioType
						|| _listRecalls[j].RecallTypeNum==RecallTypes.ProphyType))
					{
						dateDue=_listRecalls[j].DateDue;
						dateSched=_listRecalls[j].DateScheduled;
						isdisabled=_listRecalls[j].IsDisabled;
					}
				}
				if(isdisabled) {
					item.SubItems.Add(Lan.g(this,"disabled"));
				}
				else if(dateDue.Year<1880) {
					item.SubItems.Add("");
				}
				else {
					item.SubItems.Add(dateDue.ToShortDateString());
				}
				if(dateDue<=DateTime.Today) {
					item.ForeColor=Color.Red;
				}
				if(dateSched.Year<1880) {
					item.SubItems.Add("");
				}
				else{
					item.SubItems.Add(dateSched.ToShortDateString());
				}
				listViewFamily.Items.Add(item);
			}
			checkDone.Checked=_patCur.PlannedIsDone;
			textFinUrg.Text=_famCur.ListPats[0].FamFinUrgNote;
		}

		private void listFamily_DoubleClick(object sender, System.EventArgs e) {
			if(listViewFamily.SelectedIndices.Count==0) {
				return;
			}
			using FormRecallsPat formRecallsPat=new FormRecallsPat();
			formRecallsPat.PatNum=_patCur.PatNum;
			formRecallsPat.ShowDialog();
		}

		private void checkShowCompletePlanned_CheckedChanged(object sender,EventArgs e) {
			odApptGrid.IsShowCompletePlanned=checkShowCompletePlanned.Checked;
			odApptGrid.RefreshAppts();
		}

		private void butRecall_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(AppointmentL.PromptForMerge(_patCur,out _patCur)) {
				FillFamily();
				odApptGrid.PatCur=_patCur;
				odApptGrid.RefreshAppts();
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && ListTools.In(_patCur.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+_patCur.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			MakeRecallAppointment();
		}

		///<summary>Creates a single recall appointment. If it's from a double click, then it will end up on that spot in the Appts module.  If not, it will end up on the pinboard with StringDateJumpTo as due date to jump to.  ListAptNumsSelected will contain the AptNum of the new appointment.</summary>
		public void MakeRecallAppointment() {
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_famCur);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			Appointment apt=null;
			DateTime dateTimeApt=DateTime.MinValue;
			if(this.IsInitialDoubleClick) {
				dateTimeApt=DateTimeClicked;
			}
			try{
				apt=AppointmentL.CreateRecallApt(_patCur,listInsPlans,-1,listInsSubs,dateTimeApt);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DateTime datePrevious=apt.DateTStamp;
			ListAptNumsSelected.Add(apt.AptNum);
			if(IsInitialDoubleClick) {
				Appointment oldApt=apt.Copy();
				if(_patCur.AskToArriveEarly>0) {
					apt.DateTimeAskedToArrive=apt.AptDateTime.AddMinutes(-_patCur.AskToArriveEarly);
					MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+_patCur.AskToArriveEarly
						+" "+Lan.g(this,"minutes early at")+" "+apt.DateTimeAskedToArrive.ToShortTimeString()+".");
				}
				apt.AptStatus=ApptStatus.Scheduled;
				apt.ClinicNum=_patCur.ClinicNum;
				apt.Op=OpNumClicked;
				apt=Appointments.AssignFieldsForOperatory(apt);
				//Use apt.ClinicNum because it was just set based on Op.ClinicNum in AssignFieldsForOperatory().
				if(!AppointmentL.IsSpecialtyMismatchAllowed(_patCur.PatNum,apt.ClinicNum)) {
					return;
				}
				Appointments.Update(apt,oldApt);
				_otherResult=OtherResult.CreateNew;
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,apt.PatNum,apt.AptDateTime.ToString(),apt.AptNum,datePrevious);
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S12 - New Appt Booking event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(_patCur,_famCur.GetPatient(_patCur.Guarantor),EventTypeHL7.S12,apt);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=apt.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=_patCur.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							MessageBox.Show(this,messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(_patCur.PatNum));
				}
				DialogResult=DialogResult.OK;
				return;
			}
			//not initialClick
			_otherResult=OtherResult.PinboardAndSearch;
			Recall recall=Recalls.GetRecallProphyOrPerio(_patCur.PatNum);//shouldn't return null.
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
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			MakeRecallFamily();
		}

		///<summary>Creates appointments for each patient in _famCur.  MsgBox informs user of anyone skipped.  StringDateJumpTo will contain the due date (of the last family member) to jump to.  ListAptNumsSelected will contain the AptNums of the new appointments on the pinboard.</summary>
		public void MakeRecallFamily() {
			List<Recall> listPatRecalls;
			List <InsPlan> listInsPlans;
			List<InsSub> listInsSubs;
			Appointment apt=null;
			Recall recall;
			int countAlreadySched=0;
			int countNoRecalls=0;
			int countPatsRestricted=0;
			int countPatsArchivedOrDeceased=0;
			for(int i=0;i<_famCur.ListPats.Length;i++) {
				Patient patCur=_famCur.ListPats[i];
				if(PatRestrictionL.IsRestricted(patCur.PatNum,PatRestrict.ApptSchedule,true)) {
					countPatsRestricted++;
					continue;
				}
				if(ListTools.In(patCur.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
					countPatsArchivedOrDeceased++;
					continue;
				}
				listPatRecalls=Recalls.GetList(patCur.PatNum);//get the recall for this pt
				//Check to see if the special type recall is disabled or already scheduled.  This is also done in AppointmentL.CreateRecallApt() below so I'm
				//	not sure why we do it here.
				List<Recall> listRecalls=listPatRecalls.FindAll(x => x.RecallTypeNum==RecallTypes.PerioType || x.RecallTypeNum==RecallTypes.ProphyType);
				if(listRecalls.Count==0 || listRecalls.Exists(x => x.IsDisabled)) {
					countNoRecalls++;
					continue;
				}
				if(listRecalls.Exists(x => x.DateScheduled.Year > 1880)) {
					countAlreadySched++;
					continue;
				}
				listInsSubs=InsSubs.RefreshForFam(_famCur);
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				try {
					apt=AppointmentL.CreateRecallApt(patCur,listInsPlans,-1,listInsSubs);
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue;
				}
				ListAptNumsSelected.Add(apt.AptNum);
				_otherResult=OtherResult.PinboardAndSearch;
				recall=Recalls.GetRecallProphyOrPerio(patCur.PatNum);//should not return null
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
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Appointment aptCur=new Appointment();
			aptCur.PatNum=_patCur.PatNum;
			if(_patCur.DateFirstVisit.Year < 1880
				&& !Procedures.AreAnyComplete(_patCur.PatNum))//this only runs if firstVisit blank
			{
				aptCur.IsNewPatient=true;
			}
			aptCur.Pattern="/X/";
			if(_patCur.PriProv==0) {
				aptCur.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				aptCur.ProvNum=_patCur.PriProv;
			}
			aptCur.ProvHyg=_patCur.SecProv;
			aptCur.AptStatus=ApptStatus.PtNote;
			aptCur.ClinicNum=_patCur.ClinicNum;
			aptCur.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			if(IsInitialDoubleClick) {//initially double clicked on appt module
				aptCur.AptDateTime=DateTimeClicked;
				aptCur.Op=OpNumClicked;
			}
			else {
				//new appt will be placed on pinboard instead of specific time
			}
			try {
				if(!AppointmentL.IsSpecialtyMismatchAllowed(_patCur.PatNum,aptCur.ClinicNum)) {
					return;
				}
				Appointments.Insert(aptCur);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(aptCur.AptNum);
			formApptEdit.IsNew=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			ListAptNumsSelected.Add(aptCur.AptNum);
			if(IsInitialDoubleClick) {
				_otherResult=OtherResult.CreateNew;
			}
			else {
				_otherResult=OtherResult.NewToPinBoard;
			}
			DialogResult=DialogResult.OK;

		}

		private void butNew_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(AppointmentL.PromptForMerge(_patCur,out _patCur)) {
				FillFamily();
				odApptGrid.PatCur=_patCur;
				odApptGrid.RefreshAppts();
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && ListTools.In(_patCur.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+_patCur.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			MakeAppointment();
		}

		///<summary>Offers to use unscheduled appt.  Shows ApptEdit window. Sets Prospective, if necessary.  Fires Automation triggers.  ListAptNumsSelected will contain the AptNum of the new appointment.</summary>
		public void MakeAppointment() {
			//Check to see if the patient has any unscheduled appointments and inform the user.
			List<Appointment> listUnschedAppts=Appointments.GetUnschedApptsForPat(_patCur.PatNum);
			//Per Nathan, pinboard appointments will not be considered unscheduled for this logic.
			listUnschedAppts.RemoveAll(x => ListTools.In(x.AptNum,_listPinboardApptNums));
			long aptNum=0;
			bool isSchedulingUnscheduled=false;
			if(listUnschedAppts.Count>0 &&
				 MsgBox.Show(this,MsgBoxButtons.YesNo,"This patient has an unscheduled appointment, would you like to use an existing unscheduled appointment?"))
			{
				if(listUnschedAppts.Count==1) {
					aptNum=listUnschedAppts[0].AptNum;
				}
				else {//Multiple unscheduled appointments, let the user pick which one to use.
					using FormUnschedListPatient formUnschedListPatient=new FormUnschedListPatient(_patCur);
					if(formUnschedListPatient.ShowDialog()!=DialogResult.OK) {
						return;
					}
					//Use the appointment the user selected.
					aptNum=formUnschedListPatient.SelectedAppt.AptNum;
				}
				isSchedulingUnscheduled=true;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum,patNum:_patCur.PatNum,useApptDrawingSettings:IsInitialDoubleClick,patient:_patCur,dateTNew:DateTNew,opNumNew:OpNumNew);
			formApptEdit.IsNew=(aptNum==0);
			formApptEdit.IsSchedulingUnscheduledAppt=isSchedulingUnscheduled;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			Appointment aptCur=formApptEdit.GetAppointmentCur();
			if(IsInitialDoubleClick) {
				if(isSchedulingUnscheduled) {//User double clicked in Appointment Module, intending to schedule appointment at a specific time/op/etc.
					Appointment aptOld=aptCur.Copy();
					aptCur.AptDateTime=DateTimeClicked;
					aptCur.Op=OpNumClicked;
					if(_patCur!=null && _patCur.AskToArriveEarly>0) {
						aptCur.DateTimeAskedToArrive=aptCur.AptDateTime.AddMinutes(-_patCur.AskToArriveEarly);
					}
					aptCur=Appointments.AssignFieldsForOperatory(aptCur);
					aptCur.AptStatus=ApptStatus.Scheduled;
					Appointments.Update(aptCur,aptOld);
				}
				//Change PatStatus to Prospective or from Prospective.
				Operatory opCur=Operatories.GetOperatory(aptCur.Op);
				if(opCur!=null) {
					if(opCur.SetProspective && _patCur.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
							Patient patOld=_patCur.Copy();
							_patCur.PatStatus=PatientStatus.Prospective;
							Patients.Update(_patCur,patOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+_patCur.PatStatus.GetDescription()+Lan.g(this," by making an appointment from the Other Appointments window.");
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,_patCur.PatNum,logEntry);
						}
					}
					else if(!opCur.SetProspective && _patCur.PatStatus==PatientStatus.Prospective) {
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
							Patient patOld=_patCur.Copy();
							_patCur.PatStatus=PatientStatus.Patient;
							Patients.Update(_patCur,patOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+_patCur.PatStatus.GetDescription()+Lan.g(this," by making an appointment from the Other Appointments window.");
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,_patCur.PatNum,logEntry);
						}
					}
				}
			}
			ListAptNumsSelected.Add(aptCur.AptNum);
			if(IsInitialDoubleClick) {
				_otherResult=OtherResult.CreateNew;
			}
			else{
				_otherResult=OtherResult.NewToPinBoard;
			}
			if(aptCur.IsNewPatient) {
				AutomationL.Trigger(AutomationTrigger.CreateApptNewPat,null,aptCur.PatNum,aptCur.AptNum);
			}
			AutomationL.Trigger(AutomationTrigger.CreateAppt,null,aptCur.PatNum,aptCur.AptNum);
			DialogResult=DialogResult.OK;
		}

		private void butPin_Click(object sender, System.EventArgs e) {
			if(odApptGrid.SelectedApptOther==null) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			if(odApptGrid.IsSelectedApptOtherNull()) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(!AppointmentL.OKtoSendToPinboard(odApptGrid.SelectedApptOther,odApptGrid.ListApptOthers,this)) {//Tag is AptNum
				return;
			}
			ListAptNumsSelected.Add(odApptGrid.SelectedApptOther.AptNum);
			_otherResult=OtherResult.CopyToPinBoard;
			DialogResult=DialogResult.OK;
		}

		private void listFamily_Click(object sender,EventArgs e) {
			//Changes the patient to whoever was clicked in the list 
			if(listViewFamily.SelectedIndices.Count==0) {
				return;
			}
			long oldPatNum=_patCur.PatNum;
			long newPatNum=((Patient)listViewFamily.SelectedItems[0].Tag).PatNum;
			if(newPatNum==oldPatNum) {
				return;
			}
			_patCur=_famCur.GetPatient(newPatNum);
			Text=Lan.g(this,"Appointments for")+" "+_patCur.GetNameLF();
			textApptModNote.Text=_patCur.ApptModNote;
			FillFamily();
			odApptGrid.PatCur=_patCur;
			odApptGrid.RefreshAppts();
			CheckStatus();
		}

		private void textApptModNote_Leave(object sender,EventArgs e) {
			if(textApptModNote.Text!=_patCur.ApptModNote) {
				Patient PatOld=_patCur.Copy();
				_patCur.ApptModNote=textApptModNote.Text;
				Patients.Update(_patCur,PatOld);
			}
		}

		private void butGoTo_Click(object sender, System.EventArgs e) {
			if(odApptGrid.SelectedApptOther==null) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			if(odApptGrid.IsSelectedApptOtherNull()) {
				return;
			}
			ApptOther aptSelected=odApptGrid.SelectedApptOther;
			if(aptSelected.AptDateTime.Year<1880) {
				MsgBox.Show(this,"Unable to go to unscheduled appointment.");
				return;
			}
			ListAptNumsSelected.Add(aptSelected.AptNum);
			StringDateJumpTo=aptSelected.AptDateTime.Date.ToShortDateString();
			ListAptViewJumpTos=ApptViewItems.GetViewsByOp(aptSelected.Op);
			_otherResult=OtherResult.GoTo;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//only used when selecting from TaskList. oResult is completely ignored in this case.
			//I didn't bother enabling double click. Maybe later.
			if(odApptGrid.SelectedApptOther==null) {
				MsgBox.Show(this,"Please select appointment first.");
				return;
			}
			ListAptNumsSelected.Add(odApptGrid.SelectedApptOther.AptNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormApptsOther_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			_otherResult=OtherResult.Cancel;
		}
	}
}
