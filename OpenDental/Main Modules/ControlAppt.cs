using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using System.Windows.Interop;
using Bridges;

namespace OpenDental {
	///<summary>The Appointments Module.</summary>
	public partial class ControlAppt:UserControl {
		#region Fields - Private
		///<summary>Used for blockouts.  OpNum.</summary>
		private long _opNumClickedBlockout;
		///<summary>Used for blockouts.  Already handles week view.  Time is not rounded.</summary>
		private DateTime _dateTimeClickedBlockout;
		///<summary>The last dateTime that the waiting room was refreshed.  Local computer time.</summary>
		private DateTime _dateTimeWaitingRmRefreshed;
		///<summary></summary>
		private bool _isPrintCardFamily;
		private FormASAP _formASAP;
		private FormConfirmList _formConfirmList;
		private FormRecallList _formRecallList;
		private FormTrackNext _formTrackNext;
		private FormUnsched _formUnsched;
		///<summary>This prevents extra refreshes during the convoluted startup sequence.  Remains false until the end of InitializeOnStartup().</summary>
		private bool _hasInitializedOnStartup;
		///<summary>So that SetInitialStartTime only runs once.</summary>
		private bool _hasSetInitialStartTime;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private List<Provider> _listProvidersSearch;
		private List<ScheduleOpening> _listScheduleOpenings;
		private ToolStripMenuItem toolStripMenuItem;
		private Patient _patient;
		///<summary>If the user has done a blockout/copy, then this will contain the blockout that is on the "clipboard".</summary>
		private Schedule _scheduleBlockoutClipboard;
		private OpenDentBusiness.AutoComm.Arrivals _arrivalsLoaded;
		#endregion Fields - Private

		#region Constructor
		public ControlAppt() {
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			gridReminders.ContextMenu=menuReminderEdit;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Properties
		//protected override bool ScaleChildren => false;
		private OpenDentBusiness.AutoComm.Arrivals GetArrivalsLoaded() {
			if(_arrivalsLoaded is null) {
					_arrivalsLoaded=OpenDentBusiness.AutoComm.Arrivals.LoadArrivals();
				}
			return _arrivalsLoaded;
		}
		private void SetArrivalsLoaded(OpenDentBusiness.AutoComm.Arrivals value) {
			_arrivalsLoaded=value;
		}
		#endregion Properties

		#region Methods - Event Handlers ContrAppt
		private void ContrAppt_Load(object sender,EventArgs e) {

		}

		private void ContrAppt_Resize(object sender,EventArgs e) {
			//This handles dpi changes, and the property is designed to ignore if no change.
			if(_hasInitializedOnStartup) {
				contrApptPanel.SizeFont=float.Parse(PrefC.GetString(PrefName.ApptFontSize));
			}
			//LayoutPanels();
			//if(groupSearch.Visible){
			//	groupSearch.Location=new Point(panelCalendar.Location.X,panelCalendar.Location.Y+pinBoard.Bottom+2);
			//}
		}
		#endregion Methods - Event Handlers ContrAppt

		#region Methods - Event Handlers ContrApptPanel
		private void contrApptPanel_ApptDoubleClicked(object sender,UI.ApptEventArgs e) {
			//security handled inside the form
			long patnum=e.Appt.PatNum;
			using FormApptEdit formApptEdit=new FormApptEdit(contrApptPanel.SelectedAptNum);
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult==DialogResult.OK) {//appt already saved or deleted inside that window
				Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
				if(appointment!=null) {
					Appointment appointmentOld=appointment.Copy(); //this needs to happen before TryAdjustAppointmentPattern.
					//if appts appttype is associated to blockouts and appt would start on an unassociated blockout, send to pinboard.
					if(!CanScheduleAppointmentTypeOnBlockoutType(appointment)) {
						MsgBox.Show(this,"Appointment type cannot be scheduled on this blockout.  Moving appointment to pinboard.");
						SendToPinBoardAptNums(new List<long> { appointment.AptNum });
						UpdateAppointmentToUnscheduled(appointment,appointmentOld);
					}
					else if(TryAdjustAppointmentPattern(appointment,contrApptPanel.ListOpsVisible)) {
						MsgBox.Show(this,"Appointment is too long and would overlap another appointment or blockout.  Automatically shortened to fit.");
						try {
							Appointments.Update(appointment,appointmentOld);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
					}
				}
				ModuleSelected(patnum);//apt.PatNum);//apt might be null if user deleted appt.
			}
			else if(formApptEdit.DialogResult==DialogResult.Cancel && formApptEdit.HasProcsChangedAndCancel) { //If user canceled but changed the procs on appt first
				//Refresh the grid, don't need to check length because it didn't change.  Plus user might not want to change length.
				ModuleSelected(patnum);
				Signalods.SetInvalidAppt(formApptEdit.GetAppointmentOld());//use old here because they cancelled.  Only calling this because there is no S-Class call.
			}
		}

		private void contrApptPanel_ApptMainAreaDoubleClicked(object sender,UI.ApptMainClickEventArgs e) {
			if(Operatories.GetOperatory(e.OpNum) is null) {//Prevents user from making an appointment by clicking into the rectangles along the operatory header and the bottom scroll bar
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(_patient!=null) {
				formPatientSelect.PatNumInitial=_patient.PatNum;
			}
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(_patient==null || formPatientSelect.PatNumSelected!=_patient.PatNum) {//if the patient was changed
				RefreshModuleDataPatient(formPatientSelect.PatNumSelected);
				GlobalFormOpenDental.PatientSelected(_patient,true,false);
			}
			if(_patient!=null && PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Patient patientMerged = AppointmentL.GetPatientMergePrompt(_patient.PatNum);
			if(patientMerged!=null) {
				_patient=patientMerged;
				RefreshModuleDataPatient(_patient.PatNum);
				GlobalFormOpenDental.PatientSelected(_patient,isRefreshCurModule: true,isApptRefreshDataPat: false);
			}
			if(_patient!=null && _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(Lan.g(this,"Appointments cannot be scheduled for ")+_patient.PatStatus.ToString().ToLower()+Lan.g(this," patients."));
				return;
			}
			Appointment appointment=null;
			bool isUpdateAppt=false;
			if(formPatientSelect.IsNewPatientAdded) {
				Operatory operatory=Operatories.GetOperatory(e.OpNum);
				//if(contrApptPanel.IsWeeklyView) {//handled before this event
				//	dateSelected=WeekStartDate.AddDays(SheetClickedonDay);
				DateTime dateTimeAskedToArrive=DateTime.MinValue;
				if(_patient.AskToArriveEarly > 0) {
					dateTimeAskedToArrive=e.DateT.AddMinutes(-_patient.AskToArriveEarly);
					MessageBox.Show(Lan.g(this,"Ask patient to arrive ")+_patient.AskToArriveEarly
						+Lan.g(this," minutes early at ")+dateTimeAskedToArrive.ToShortTimeString()+".");
				}
				appointment=Appointments.CreateNewAppointment(_patient,operatory,e.DateT,dateTimeAskedToArrive,null,contrApptPanel.ListSchedules);
				//New patient. Set to prospective if operatory is set to set prospective.
				if(operatory.SetProspective) {
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
						Patient patientOld=_patient.Copy();
						_patient.PatStatus=PatientStatus.Prospective;
						Patients.UpdateRecalls(_patient,patientOld,"Appointment Module, New Patient appointment created in prospective operatory");
						Patients.Update(_patient,patientOld);
						string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
							+_patient.PatStatus.GetDescription()+Lan.g(this," by creating an appointment in a prospective operatory.");
						SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,logEntry);
					}
				}
				using FormApptEdit formApptEdit=new FormApptEdit(appointment.AptNum);//this is where security log entry is made
				formApptEdit.IsNew=true;
				formApptEdit.ShowDialog();
				if(formApptEdit.DialogResult==DialogResult.OK) {
					if(appointment.IsNewPatient) {
						AutomationL.Trigger(EnumAutomationTrigger.ApptNewPatCreate,null,appointment.PatNum,appointment.AptNum);
					}
					AutomationL.Trigger(EnumAutomationTrigger.ApptCreate,null,appointment.PatNum,appointment.AptNum);
					RefreshModuleDataPatient(_patient.PatNum);
					GlobalFormOpenDental.PatientSelected(_patient,true,false);
					if(!HasValidStartTime(appointment)) {
						Appointment appointmentOld2=appointment.Copy();
						MsgBox.Show(this,"Appointment start time would overlap another appointment.  Moving appointment to pinboard.");
						SendToPinBoardAptNums(new List<long> { appointment.AptNum });
						appointment.AptStatus=ApptStatus.UnschedList;
						try {
							Appointments.Update(appointment,appointmentOld2);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
						RefreshPeriod();
						return;//It's ok to skip the rest of the method here. The appointment is now on the pinboard and must be rescheduled
					}
					appointment=Appointments.GetOneApt(appointment.AptNum);  //Need to get appt from DB so we have the time pattern
					contrApptPanel.SelectedAptNum=appointment.AptNum;
					isUpdateAppt=true;						
				}
			}
			else {//new patient not added
				if(Appointments.HasOutstandingAppts(_patient.PatNum) | (Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_DoubleClick_apptOtherShow"))) {
					DisplayOtherDlg(true,e.DateT,e.OpNum);
					RefreshModuleScreenButtonsRight();
				}
				else {
					using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());//not shown
					CheckStatus();
					formApptsOther.IsInitialDoubleClick=true;
					formApptsOther.DateTimeClicked=contrApptPanel.DateTimeClicked;
					formApptsOther.OpNumClicked=contrApptPanel.OpNumClicked;
					formApptsOther.DateTNew=e.DateT;
					formApptsOther.OpNumNew=e.OpNum;
					formApptsOther.MakeAppointment();
					if(formApptsOther.ListAptNumsSelected.Count>0) {
						contrApptPanel.SelectedAptNum=formApptsOther.ListAptNumsSelected[0];
					}
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					isUpdateAppt=true;
				}
			}
			if(appointment == null) {
				return; // appointment was already moved to pinboard from other dialogue, no need to continue method.
			}
			Appointment appointmentOld=appointment.Copy();
			//if appts appttype is associated to blockouts and appt would start on an unassociated blockout, send to pinboard.
			if(!CanScheduleAppointmentTypeOnBlockoutType(appointment)) {
				MsgBox.Show(this,"Appointment type cannot be scheduled on this blockout.  Moving appointment to pinboard.");
				SendToPinBoardAptNums(new List<long> { appointment.AptNum });
				UpdateAppointmentToUnscheduled(appointment,appointmentOld);
				RefreshPeriod();
				return;//It's ok to skip the rest of the method here. The appointment is now on the pinboard and must be rescheduled
			}
			if(!HasValidStartTime(appointment)) {
				MsgBox.Show(this,"Appointment start time would overlap another appointment.  Moving appointment to pinboard.");
				SendToPinBoardAptNums(new List<long> { appointment.AptNum });
				UpdateAppointmentToUnscheduled(appointment,appointmentOld);
				RefreshPeriod();
				return;//It's ok to skip the rest of the method here. The appointment is now on the pinboard and must be rescheduled
			}
			if(isUpdateAppt && appointment!=null) {
				#region Provider Term Date Check
				//Prevents appointments with providers that are past their term end date from being scheduled
				string message=Providers.CheckApptProvidersTermDates(appointment);
				if(message!="") {
					MessageBox.Show(message);//translated in Providers S class method
					return;
				}
				#endregion Provider Term Date Check	
				appointmentOld=appointment.Copy();
				if(TryAdjustAppointmentPattern(appointment,contrApptPanel.ListOpsVisible)) {
					MsgBox.Show(this,"Appointment is too long and would overlap another appointment or blockout.  Automatically shortened to fit.");
					try {
						Appointments.Update(appointment,appointmentOld);//Appointments S-Class handles Signalods
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
					}
					Appointments.TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);
				}
				RefreshPeriod();
				RefreshModuleScreenButtonsRight();
			}
		}

		///<summary>Throws an ODException if we can't find a certain ToolStripItem with custom exception message. Added for attempted bug fix job 43418.</summary>
		private ToolStripItem FindBlockoutToolStripItem(string name) {
			ToolStripItem[] arrayToolStripItems=menuBlockout.Items.Find(name,false);
			if(arrayToolStripItems.Length==0) {
				string patNum="No Patient Selected";
				if(_patient!=null) {
					patNum=_patient.PatNum.ToString();
				}
				ODException odException=new ODException($"Patnum: {patNum}\r\n" 
				+$"OD Version: {Application.ProductVersion}\r\n"
				+$"Missing ToolStripItem: {name}");
				throw odException;
			}
			return arrayToolStripItems[0];
		}

		private void ContrApptPanel_ApptMainAreaRightClicked(object sender, UI.ApptMainClickEventArgs e){
			ToolStripItem toolStripItemEdit=FindBlockoutToolStripItem(MenuItemNames.EditBlockout);
			ToolStripItem toolStripItemCut=FindBlockoutToolStripItem(MenuItemNames.CutBlockout);
			ToolStripItem toolStripItemCopy=FindBlockoutToolStripItem(MenuItemNames.CopyBlockout);
			ToolStripItem toolStripItemPaste=FindBlockoutToolStripItem(MenuItemNames.PasteBlockout);
			ToolStripItem toolStripItemDelete=FindBlockoutToolStripItem(MenuItemNames.DeleteBlockout);
			//AddBlockout is not used here
			ToolStripItem toolStripItemCutCopyPaste=FindBlockoutToolStripItem(MenuItemNames.BlockoutCutCopyPaste);
			ToolStripItem toolStripItemClearForDay=new ToolStripMenuItem();
			ToolStripItem toolStripItemClearForDayOp=new ToolStripMenuItem();
			ToolStripItem toolStripItemClearForDayClinics=new ToolStripMenuItem();
			if(PrefC.HasClinicsEnabled) {//No clear for day if clinics enabled
				toolStripItemClearForDayOp=FindBlockoutToolStripItem(MenuItemNames.ClearAllBlockoutsForDayOpOnly);
				toolStripItemClearForDayClinics=FindBlockoutToolStripItem(MenuItemNames.ClearAllBlockoutsForDayClinicOnly);
			}
			else {//Clinics disabled, no clear for day clinics
				toolStripItemClearForDay=FindBlockoutToolStripItem(MenuItemNames.ClearAllBlockoutsForDay);
				toolStripItemClearForDayOp=FindBlockoutToolStripItem(MenuItemNames.ClearAllBlockoutsForDayOpOnly);
			}
			if(!Security.IsAuthorized(EnumPermType.Blockouts,true)) {
				toolStripItemCutCopyPaste.Enabled=false;
				toolStripItemClearForDay.Enabled=false;
				toolStripItemClearForDayOp.Enabled=false;
				toolStripItemClearForDayClinics.Enabled=false;
			}
			else if(Security.IsAuthorized(EnumPermType.Blockouts,true)) {
				toolStripItemCutCopyPaste.Enabled=true;
				toolStripItemClearForDay.Enabled=true;
				toolStripItemClearForDayOp.Enabled=true;
				toolStripItemClearForDayClinics.Enabled=true;
			}
			_opNumClickedBlockout=e.OpNum;
			_dateTimeClickedBlockout=e.DateT;
			int clickedOnBlockCount=0;
			string blockoutFlags="";
			List<Schedule> ListSchedulesBlockout=Schedules.GetListForType(contrApptPanel.ListSchedules,ScheduleType.Blockout,0);
			//List<ScheduleOp> listForSched;
			for(int i=0;i<ListSchedulesBlockout.Count;i++) {
				if(ListSchedulesBlockout[i].SchedDate.Date!=e.DateT.Date) {
					continue;
				}
				if(ListSchedulesBlockout[i].StartTime > e.DateT.TimeOfDay
					|| ListSchedulesBlockout[i].StopTime <= e.DateT.TimeOfDay) {
					continue;
				}
				//listForSched=ScheduleOps.GetForSched(ListForType[i].ScheduleNum);
				for(int p=0;p<ListSchedulesBlockout[i].Ops.Count;p++) {
					if(ListSchedulesBlockout[i].Ops[p]==e.OpNum) {
						clickedOnBlockCount++;
						blockoutFlags=Defs.GetDef(DefCat.BlockoutTypes,ListSchedulesBlockout[i].BlockoutType).ItemValue;
						break;//out of ops loop
					}
				}
			}
			if(clickedOnBlockCount>0) {
				toolStripItemPaste.Enabled=false;//Can't paste on top of an existing blockout
				toolStripItemEdit.Enabled=true;
				toolStripItemCopy.Enabled=true;
				toolStripItemCut.Enabled=true;
				toolStripItemDelete.Enabled=true;
				if(blockoutFlags.Contains(BlockoutType.DontCopy.GetDescription())) {
					//users without blockout permission are still allowed to add and edit this blockout. 
					toolStripItemCut.Enabled=false;
					toolStripItemCopy.Enabled=false;
				}
				else if(blockoutFlags.Contains(BlockoutType.NoSchedule.GetDescription())) {
					//users without blockout permission are still allowed to add and edit this blockout.
					if(!Security.IsAuthorized(EnumPermType.Blockouts,true)) {
						toolStripItemCut.Enabled=false;
						toolStripItemCopy.Enabled=false;
					}
				}
				else { //this is not a blockout type that this user is allowed to edit. 
					if(!Security.IsAuthorized(EnumPermType.Blockouts,true)) {
						toolStripItemEdit.Enabled=false;
						toolStripItemCopy.Enabled=false;
						toolStripItemCut.Enabled=false;
						toolStripItemDelete.Enabled=false;
					}
				}
				if(clickedOnBlockCount>1) {
					string message=Lan.g(this,"There are multiple blockouts in this slot.  You should try to delete or move one of them.");
					FormPopupFade formPopupFade=new FormPopupFade(message);
					formPopupFade.Show();
				}
			}
			else {//Not clicked on blockout
				toolStripItemEdit.Enabled=false;
				toolStripItemCut.Enabled=false;
				toolStripItemCopy.Enabled=false;
				if(_scheduleBlockoutClipboard==null) {
					toolStripItemPaste.Enabled=false;
				}
				else {
					toolStripItemPaste.Enabled=true;
				}
				toolStripItemDelete.Enabled=false;
			}
			bool isTextingEnabled=SmsPhones.IsIntegratedTextingEnabled();
			bool isDeleteAsapBlockoutVisible=false;
			if(PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				if(!SmsPhones.IsIntegratedTextingEnabled()) {//Some customers have the Bundle without texting
					textASAPContextMenuHelper(true,"Email ASAP List");
				}
				else {
					textASAPContextMenuHelper(true,"Text ASAP List");
				}
				if(Security.IsAuthorized(EnumPermType.Blockouts)) {
					Schedule schedule=GetClickedSchedule(ScheduleType.WebSchedASAP);
					if(schedule!=null) {
						isDeleteAsapBlockoutVisible=true;
					}
				}
			}
			else if(isTextingEnabled) {//Don't have Web Sched ASAP but do have texting
				textASAPContextMenuHelper(true,"Text ASAP List (manual)");
			}
			else {//Don't have Web Sched ASAP or texting
				textASAPContextMenuHelper(false);
			}
			SetMenuItemProperty(menuBlockout,MenuItemNames.DeleteWebSchedAsapBlockout,x => x.Visible=isDeleteAsapBlockoutVisible);
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDayOp,x => x.Visible=isTextingEnabled);
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDayView,x => x.Visible=isTextingEnabled);
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDay,x => {
				x.Visible=isTextingEnabled;
				x.Text=MenuItemNames.TextApptsForDay+(PrefC.HasClinicsEnabled ? ", Clinic only" : "");
			});
			Plugins.HookAddCode(this,"ControlAppt.ContrApptPanel_ApptMainAreaRightClick_end",menuBlockout);
			menuBlockout.Show(contrApptPanel,e.Location);
		}

		private void textASAPContextMenuHelper(bool isVisible,string itemText="") {
			ToolStripItem[] toolStripItemArray=menuBlockout.Items.Find(MenuItemNames.TextAsapList,false);
			if(toolStripItemArray.Length > 0) {
				toolStripItemArray[0].Visible=isVisible;
				toolStripItemArray[0].Text=Lans.g(this,itemText);
			}
		}

		private void contrApptPanel_ApptMoved(object sender,UI.ApptMovedEventArgs e) {
			Appointment appointment=e.Appt;
			Appointment appointmentOld=e.ApptOld;
			List<Procedure> listProceduresOld=Procedures.GetProcsForSingle(appointment.AptNum,false);//get the procedures on the appointment before they are updated
			MoveAppointment(appointment,appointmentOld);//This does a lot.  Many nested calls, including to SetProvidersInAppointment.
			#region Update UI and cache
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(e.Appt.PatNum);
			//check if the proc fees on the moved appointment need updating
			bool isUpdatingFees = false;
			List<Procedure> listProceduresNew=listProceduresOld.Select(x => Procedures.ChangeProcInAppointment(appointment,x.Copy())).ToList();
			if(listProceduresOld.Exists(x => x.ProvNum!=listProceduresNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
				string promptText = "";
				isUpdatingFees=Procedures.ShouldFeesChange(listProceduresNew,listProceduresOld,ref promptText,procFeeHelper);
				if(isUpdatingFees) {//Made it pass the pref check.
					if(promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {//prompt is fixed text
						isUpdatingFees=false;
					}
				}
			}
			Procedures.SetProvidersInAppointment(appointment,listProceduresOld,isUpdatingFees,procFeeHelper);//to update fees to db
			RefreshModuleDataPatient(appointment.PatNum);
			GlobalFormOpenDental.PatientSelected(_patient,true,false);
			RefreshPeriod();				
			Recalls.SynchScheduledApptFull(appointment.PatNum);
			ODEvent.Fire(ODEventType.AppointmentEdited,appointment);
			#endregion Update UI and cache
			//The first hook's naming convention was maintained to allow it to continue to function.
			//Plugin developers should use the "ContrAppt.contrApptPanel_ApptMoved_end" hook instead.
			Plugins.HookAddCode(this,"ContrAppt.ContrApptSheet2_MouseUp_end",appointment,appointmentOld);
			Plugins.HookAddCode(this,"ContrAppt.contrApptPanel_ApptMoved_end",appointment,appointmentOld);
		}

		private void contrApptPanel_ApptMovedToPinboard(object sender,UI.ApptDataRowEventArgs e) {
			SendToPinboardDataRow(e.DataRowAppt);//sets selectedAptNum=-1. do before refresh prev
			//If pref BrokenApptRequiredOnMove is on, refresh pinboard right away to show broken appt.
			if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
				RefreshPinboardImages();
			}
		}

		private void contrApptPanel_ApptNullFound(object sender,EventArgs e) {
			MsgBox.Show(this,"Selected appointment no longer exists.");
			RefreshPeriod();
		}

		private void contrApptPanel_ApptResized(object sender,UI.ApptEventArgs e) {
			RefreshModuleDataPatient(e.Appt.PatNum);
			GlobalFormOpenDental.PatientSelected(_patient,true,false);
			RefreshPeriod();
			ODEvent.Fire(ODEventType.AppointmentEdited,e.Appt);
		}

		private void ContrApptPanel_ApptRightClicked(object sender, UI.ApptRightClickEventArgs e){
			DataRow dataRowAppointment=contrApptPanel.TableAppointments.Select().FirstOrDefault(x => PIn.Long(x["AptNum"].ToString())==contrApptPanel.SelectedAptNum);
			if(PrefC.IsODHQ) {
				menuApt.Items.RemoveByKey(MenuItemNames.Jobs);
				menuApt.Items.RemoveByKey(MenuItemNames.JobsSpacer);
				menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.JobsSpacer });
				ToolStripMenuItem menuJobs=new ToolStripMenuItem(Lan.g(this,"Jobs"),null,null,MenuItemNames.Jobs);
				menuJobs.DropDownItems.Add(Lan.g(this,"Attach Job"),null,menuJobs_Attach);
				List<JobLink> listJobLinks = JobLinks.GetForApptNum(contrApptPanel.SelectedAptNum);
				List<Job> listJobs = Jobs.GetMany(listJobLinks.Select(x => x.JobNum).ToList());
				menuJobs.DropDownItems.AddRange(listJobs.Select(x => new ToolStripMenuItem(x.ToString(),null,menuJobs_GoToJob) { Tag=x.JobNum }).ToArray());
				menuApt.Items.Add(menuJobs);
			}
			menuApt.Items.RemoveByKey(MenuItemNames.PhoneDiv);
			menuApt.Items.RemoveByKey(MenuItemNames.HomePhone);
			menuApt.Items.RemoveByKey(MenuItemNames.WorkPhone);
			menuApt.Items.RemoveByKey(MenuItemNames.WirelessPhone);
			menuApt.Items.RemoveByKey(MenuItemNames.TextDiv);
			menuApt.Items.RemoveByKey(MenuItemNames.SendText);
			menuApt.Items.RemoveByKey(MenuItemNames.SendConfirmationText);
			menuApt.Items.RemoveByKey(MenuItemNames.SendComeInText);
			menuApt.Items.RemoveByKey(MenuItemNames.EClipboardQR);
			//To enable Texting Statements on right click, uncomment this code.
			//menuApt.Items.RemoveByKey(MenuItemNames.SendPaymentLinkText);
			menuApt.Items.RemoveByKey(MenuItemNames.SendEClipboardByod);
			menuApt.Items.RemoveByKey(MenuItemNames.OrthoChart);
			menuApt.Items.RemoveByKey(MenuItemNames.CareCredit);
			menuApt.Items.RemoveByKey(MenuItemNames.CareCreditAcceptDeclineOffer);
			menuApt.Items.RemoveByKey(MenuItemNames.CareCreditApplicationNeeded);
			menuApt.Items.RemoveByKey(MenuItemNames.CareCreditDiv);
			ToolStripItem menuItem;
			if(PrefC.GetBool(PrefName.ApptModuleShowOrthoChartItem)) {
				menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Go To ")+OrthoChartTabs.GetFirst(true).TabName,null,menuApt_Click,MenuItemNames.OrthoChart));
			}
			//Phone numbers
			//The menu items to "Call Home/Work/Cell" will only be added to this context menu if action will actually be taken when clicking on them, i.e.
			//when DentalTek bridge is disabled and advertising is disabled, nothing happens when clicking this buttons.
			if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled 
				|| !ProgramProperties.IsAdvertisingDisabled(ProgramName.DentalTekSmartOfficePhone))
			{
				if(!String.IsNullOrEmpty(_patient.HmPhone)||!String.IsNullOrEmpty(_patient.WkPhone)||!String.IsNullOrEmpty(_patient.WirelessPhone)) {
					menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.PhoneDiv });
				}
				if(!String.IsNullOrEmpty(_patient.HmPhone)) {
					menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Call Home Phone ")+_patient.HmPhone,null,menuApt_Click,MenuItemNames.HomePhone));
				}
				if(!String.IsNullOrEmpty(_patient.WkPhone)) {
					menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Call Work Phone ")+_patient.WkPhone,null,menuApt_Click,MenuItemNames.WorkPhone));
				}
				if(!String.IsNullOrEmpty(_patient.WirelessPhone)) {
					menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Call Wireless Phone ")+_patient.WirelessPhone,null,menuApt_Click,MenuItemNames.WirelessPhone));
				}
			}
			//Texting
			menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.TextDiv });
			menuItem=new ToolStripMenuItem(Lan.g(this,"Send Text"),null,menuApt_Click,MenuItemNames.SendText);
			menuApt.Items.Add(menuItem);
			if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
				menuItem.Enabled=false;
			}
			menuItem=new ToolStripMenuItem(Lan.g(this,"Send Confirmation Text"),null,menuApt_Click,MenuItemNames.SendConfirmationText);
			menuApt.Items.Add(menuItem);
			if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
				menuItem.Enabled=false;
			}
			long clinicNum=(dataRowAppointment is null) ? 0 : PIn.Long(dataRowAppointment["ClinicNum"].ToString()); 
			if(OpenDentBusiness.AutoComm.Byod.IsSetup(clinicNum,out string err)) {//Check-In Links feature is enabled.  (handles PrefC.HasClinicsEnabled)
				menuItem=new ToolStripMenuItem(Lan.g(this,MenuItemNames.SendEClipboardByod),null,menuApt_Click,MenuItemNames.SendEClipboardByod);
				menuApt.Items.Add(menuItem);
				long confirmed=(dataRowAppointment is null) ? 0 : PIn.Long(dataRowAppointment["Confirmed"].ToString()); 
				menuItem.Enabled=OpenDentBusiness.AutoComm.Byod.IsEnabledForConfirmed(confirmed,clinicNum,out err);//(handles PrefC.HasClinicsEnabled)
			}
			//If the appointment is not for today, or the office is not signed up for eclipboard, hide this item. We are using the calendars selected date instead of the appointments date to save us from making a database call.
			if(monthCalendarOD.GetDateSelected()==DateTime.Today.Date && MobileAppDevices.IsClinicSignedUpForEClipboard(Clinics.ClinicNum)) {
				menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Show eClipboard QR"),null,menuApt_Click,MenuItemNames.EClipboardQR));
			}
			menuItem=new ToolStripMenuItem(Lan.g(this,MenuItemNames.SendComeInText),null,menuApt_Click,MenuItemNames.SendComeInText);
			menuApt.Items.Add(menuItem);
			if(!GetArrivalsLoaded().HasComeInMsg(contrApptPanel.SelectedAptNum)) {
				menuItem.Enabled=false;
			}
			else if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
				menuItem.Enabled=false;
			}
			//To enable Texting Statements on right click, uncomment this code.
			//menuItem=new ToolStripMenuItem(Lan.g(this,MenuItemNames.SendPaymentLinkText),null,menuApt_Click,MenuItemNames.SendPaymentLinkText);
			//menuApt.Items.Add(menuItem);
			//ApptStatus apptStatus=(apptRow is null) ? ApptStatus.None : PIn.Enum<ApptStatus>(apptRow["AptStatus"].ToString());
			//if(!SmsPhones.IsIntegratedTextingEnabled() 
			//	&& !Programs.IsEnabled(ProgramName.CallFire) 
			//	&& !PrefC.GetBool(PrefName.PatientPortalSignedUp) 
			//	|| apptStatus!=ApptStatus.Complete) 
			//{
			//	menuItem.Enabled=false;
			//}
			//CareCredit
			if(Programs.IsEnabled(ProgramName.CareCredit)) {
				DataRow dataRow=contrApptPanel.GetDataRowForSelected();
				PatFieldDef patFieldDef=PatFieldDefs.GetPatFieldCareCredit();
				string careCreditFieldName="";
				if(patFieldDef!=null) {
					careCreditFieldName=patFieldDef.FieldName;
				}
				string careCreditStatus="";
				for(int i=0;i<contrApptPanel.TablePatFields.Rows.Count;i++) {
					if(contrApptPanel.TablePatFields.Rows[i]["PatNum"].ToString()!=dataRow["PatNum"].ToString()) {
						continue;
					}
					if(contrApptPanel.TablePatFields.Rows[i]["FieldName"].ToString()!=careCreditFieldName) {
						continue;
					}
					if(contrApptPanel.TablePatFields.Rows[i]["FieldName"].ToString()==careCreditFieldName) {
						careCreditStatus=contrApptPanel.TablePatFields.Rows[i]["FieldValue"].ToString();
						break;
					}
				}
				Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
				if(careCreditStatus.ToLower().In(CareCreditWebStatus.PreApproved.GetDescription().ToLower())) 
				{
					menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.CareCreditDiv });
					menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.CareCreditAcceptDeclineOffer),null,menuApt_Click,MenuItemNames.CareCreditAcceptDeclineOffer));
				}
				if(careCreditStatus.ToLower()==CareCreditWebStatus.Declined.GetDescription().ToLower()
					||careCreditStatus.ToLower()==CareCreditWebStatus.AccountNotFoundQS.GetDescription().ToLower())
				{
					menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.CareCreditDiv });
					menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.CareCreditApplicationNeeded),null,menuApt_Click,MenuItemNames.CareCreditApplicationNeeded));
				}
			}
			else if(!ProgramProperties.IsAdvertisingDisabled(ProgramName.CareCredit)) {
				menuApt.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.CareCreditDiv });
				menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.CareCredit),null,menuApt_Click,MenuItemNames.CareCredit));
			}
			Plugins.HookAddCode(this,"ContrAppt.MouseDownAppointment_menuApt_right_click",menuApt,contrApptPanel.SelectedAptNum);
			menuApt.Show(contrApptPanel,e.Location);
		}

		private void ContrApptPanel_DateChanged(object sender, EventArgs e){
			SetWeeklyView(contrApptPanel.IsWeeklyView);//because weekly view changed internally as well.
		}

		private void contrApptPanel_SelectedApptChanged(object sender,UI.ApptSelectedChangedEventArgs e) {
			pinBoard.SelectedIndex=-1;
			if(e.AptNumNew==-1){				
				RefreshModuleScreenButtonsRight();//just disables the buttons on the right
				//we will leave current patient selected.
				return;
			};
			if(_patient==null || _patient.PatNum!=e.PatNumNew){//patient changed
				RefreshModuleDataPatient(e.PatNumNew);
				if(_patient.PatStatus==PatientStatus.Deleted) {
					Patient patientOld=Patients.GetPat(e.PatNumNew);
					_patient.PatStatus=PatientStatus.Archived;
					if(Patients.Update(_patient,patientOld)) {
						MsgBox.Show("Patient has been set to archived because they were deleted by another user while making this appointment.");
					}
				}
				GlobalFormOpenDental.PatientSelected(_patient,true,false);
				Plugins.HookAddCode(this,"ContrAppt.contrApptPanel_SelectedApptChanged_patientchanged_end");
				return;
			}
			//patient not changed
			if(e.AptNumNew!=e.AptNumOld){
				RefreshModuleScreenButtonsRight();//otherwise included above in RefreshModuleDataPatient
			}
		}

		///<summary>Finds the MenuItem on the contextMenu and performs the action on it.</summary>
		private void SetMenuItemProperty(ContextMenuStrip contextMenu,string menuItemName,Action<ToolStripItem> actionSetMenuItem) {
			ToolStripItem[] toolStripItemArray=contextMenu.Items.Find(menuItemName,false);
			if(toolStripItemArray.Length > 0) {
				actionSetMenuItem(toolStripItemArray[0]);
			}
		}
		#endregion Methods - Event Handlers ContrApptPanel

		#region Methods - Event Handlers ToolBarMain
		private void FormIVL_FormClosing(object sender,FormClosingEventArgs e) {
			//Action does not currently need to be taken when leaving the insurance verification list window.
		}		

		private void FormIVL_FormClosed(object sender,FormClosedEventArgs e) {
			RefreshModuleDataPeriod();
			RefreshModuleScreenPeriod();
		}

		private void ListASAP_Click() {
			if(_formASAP==null || _formASAP.IsDisposed) {
				_formASAP=new FormASAP();
			}
			_formASAP.Show();
			if(_formASAP.WindowState==FormWindowState.Minimized) {
				_formASAP.WindowState=FormWindowState.Normal;
			}
			_formASAP.BringToFront();
		}

		private void ListConfirm_Click() {
			if(_formConfirmList==null || _formConfirmList.IsDisposed) {
				_formConfirmList=new FormConfirmList();
			}
			_formConfirmList.Show();
			if(_formConfirmList.WindowState==FormWindowState.Minimized) {
				_formConfirmList.WindowState=FormWindowState.Normal;
			}
			_formConfirmList.BringToFront();
		}

		private void ListInsVerify_Click() {
			List<FormInsVerificationList> listFormInsVerificationLists=Application.OpenForms.OfType<FormInsVerificationList>().ToList();
			if(listFormInsVerificationLists.Count>0) {
				listFormInsVerificationLists[0].FillControls();
				listFormInsVerificationLists[0].BringToFront();
				return;
			}
			if(Security.IsAuthorized(EnumPermType.InsuranceVerification)) {
				FormInsVerificationList formInsVerificationList=new FormInsVerificationList();
				formInsVerificationList.FormClosing+=FormIVL_FormClosing;
				formInsVerificationList.FormClosed+=FormIVL_FormClosed;
				formInsVerificationList.Show();
			}
		}

		private void ListPlanned_Click() {
			if(_formTrackNext==null || _formTrackNext.IsDisposed) {
				_formTrackNext=new FormTrackNext();
			}
			_formTrackNext.Show();
			if(_formTrackNext.WindowState==FormWindowState.Minimized) {
				_formTrackNext.WindowState=FormWindowState.Normal;
			}
			_formTrackNext.BringToFront();
		}

		private void ListRadiology_Click() {
			List<FormRadOrderList> listFormRadOrderLists=Application.OpenForms.OfType<FormRadOrderList>().ToList();
			if(listFormRadOrderLists.Count > 0) {
				listFormRadOrderLists[0].RefreshRadOrdersForUser(Security.CurUser);
				listFormRadOrderLists[0].BringToFront();
				return;
			}
			FormRadOrderList formRadOrderList=new FormRadOrderList(Security.CurUser);
			formRadOrderList.Show();
		}

		private void ListRecall_Click() {
			if(_formRecallList==null || _formRecallList.IsDisposed) {
				_formRecallList=new FormRecallList();
			}
			_formRecallList.Show();
			if(_formRecallList.WindowState==FormWindowState.Minimized) {
				_formRecallList.WindowState=FormWindowState.Normal;
			}
			_formRecallList.BringToFront();
		}

		private void ListUnsched_Click() {
			//Reselect existing window if available, if not create a new instance
			if(_formUnsched==null || _formUnsched.IsDisposed) {
				_formUnsched=new FormUnsched();
			}
			_formUnsched.Show();
			if(_formUnsched.WindowState==FormWindowState.Minimized) {//only applicable if re-using an existing instance
				_formUnsched.WindowState=FormWindowState.Normal;
			}
			_formUnsched.BringToFront();
		}

		private void toolBarLists_Click() {
			FrmApptLists frmApptLists=new FrmApptLists();
			frmApptLists.ShowDialog();
			if(!frmApptLists.IsDialogOK) {
				return;
			}
			switch(frmApptLists.ApptListSelectionResult) {
				case ApptListSelection.Recall:
					ListRecall_Click();
					break;
				case ApptListSelection.Confirm:
					ListConfirm_Click();
					break;
				case ApptListSelection.Planned:
					ListPlanned_Click();
					break;
				case ApptListSelection.Unsched:
					ListUnsched_Click();
					break;
				case ApptListSelection.ASAP:
					ListASAP_Click();
					break;
				case ApptListSelection.Radiology:
					ListRadiology_Click();
					break;
				case ApptListSelection.InsVerify:
					ListInsVerify_Click();
					break;
			}
		}

		private void toolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)) {
				//standard predefined button
				switch(e.Button.Tag.ToString()) {
					case "Print":
						toolBarPrint_Click();
						break;
					case "Lists":
						toolBarLists_Click();
						break;
					case "Unsched":
						butUnsched_Click();
						break;
					case "Break":
						butBreak_Click();
						break;
					case "Complete":
						butComplete_Click();
						break;
					case "Delete":
						butDelete_Click(showPrompt:true);
						break;
					case "PatAppts":
						DisplayOtherDlg(false);
						break;
					case "Make":
						butMakeAppt_Click(this,new EventArgs());
						break;
					case "Recall":
						butMakeRecall_Click(this,new EventArgs());
						break;
					//Family recall handled in context menu
					case "RapidCall":
						try {
							OpenDental.Bridges.RapidCall.ShowPage();
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
						}
						break;
				}
				return;
			}
			if(e.Button.Tag.GetType()==typeof(Program)) {
				Patient patient=null;
				if(_patient!=null) {
					patient=Patients.GetPat(_patient.PatNum);
				}
				WpfControls.ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,patient);
			}
		}

		private void toolBarPrint_Click() {
			if(contrApptPanel.ListOpsVisible.Count==0) {//no ops visible.
				MsgBox.Show(this,"There must be at least one operatory showing in order to Print Appointments.");
				return;
			}
			if(PrinterSettings.InstalledPrinters.Count==0) {
				MsgBox.Show(this,"Printer not installed.");
				return;
			}
			List<long> listOperatoryNums=contrApptPanel.ListOpsVisible.Select(x => x.OperatoryNum).ToList();
			//Have to order listApptNums sent to FormApptPrintSetup so that routing slips can be printed in chronological order.
			List<long> listAptNums=contrApptPanel.TableAppointments.Select()
				.Where(x => listOperatoryNums.Contains(PIn.Long(x["Op"].ToString())))
				.OrderBy(x => PIn.DateT(x["AptDateTime"].ToString()))
				.Select(x => PIn.Long(x["AptNum"].ToString()))
				.ToList();
			using FormApptPrintSetup formApptPrintSetup=new FormApptPrintSetup(listAptNums,contrApptPanel.DateSelected,contrApptPanel.IsWeeklyView);
			formApptPrintSetup.ShowDialog();
			if(formApptPrintSetup.DialogResult!=DialogResult.OK) {
				return;
			}
			contrApptPanel.DateTimePrintStart=formApptPrintSetup.DateTimeApptPrintStart;
			contrApptPanel.DateTimePrintStop=formApptPrintSetup.DateTimeApptPrintStop;
			contrApptPanel.PrintingSizeFont=formApptPrintSetup.ApptPrintFontSize;
			contrApptPanel.PrintingColsPerPage=formApptPrintSetup.ApptPrintColsPerPage;
			contrApptPanel.IsPrintPreview=formApptPrintSetup.IsPrintPreview;
			contrApptPanel.PrintColorBehavior=formApptPrintSetup.ApptPrintColorBehavior_;
			contrApptPanel.PagesPrinted=0;
			contrApptPanel.PrintingPageRow=0;
			contrApptPanel.PrintingPageColumn=0;
			DateTime dateTimePrintStart=formApptPrintSetup.DateTimeApptPrintStart;//to avoid marshal by reference error in next line

			PrintoutOrientation printoutOrientation=PrintoutOrientation.Portrait;
			if(formApptPrintSetup.IsLandscape) {
				printoutOrientation=PrintoutOrientation.Landscape;
			}
			PrinterL.TryPrintOrDebugClassicPreview(contrApptPanel.PrintPage,
				Lan.g(this,"Daily appointment view for ")+dateTimePrintStart.ToShortDateString()+Lan.g(this," printed"),
				totalPages:0,
				printSituation:PrintSituation.Appointments,
				isForcedPreview:contrApptPanel.IsPrintPreview,
				printoutOrientation:printoutOrientation
			);
			if(_patient==null) {
				ModuleSelected(0);
				return;
			}
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers ToolBarMain

		#region Methods - Event Handlers PanelCalendar Upper
		private void Calendar2_SizeChanged(object sender, EventArgs e){
			//LayoutPanels();//didn't work too well
		}

		///<summary>Clicked today.</summary>
		private void butToday_Click(object sender,System.EventArgs e) {
			ModuleSelected(DateTime.Today);
		}

		///<summary>Clicked back one day.</summary>
		private void butBack_Click(object sender,System.EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddDays(-1));
		}

		///<summary>Clicked forward one day.</summary>
		private void butFwd_Click(object sender,System.EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddDays(1));
		}

		private void butBackWeek_Click(object sender,EventArgs e) {
			butBackWeek.Enabled=false;
			Application.DoEvents();//process any events before the user should be able to click the button again
			ModuleSelected(contrApptPanel.DateSelected.AddDays(-7));
			butBackWeek.Enabled=true;
		}

		private void butFwdWeek_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddDays(7));
		}
				
		private void butBackMonth_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddMonths(-1));
		}

		private void butFwdMonth_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddMonths(1));
		}

		private void butFwd3_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddMonths(3));
		}

		private void butFwd4_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddMonths(4));
		}

		private void butFwd6_Click(object sender,EventArgs e) {
			ModuleSelected(contrApptPanel.DateSelected.AddMonths(6));
		}

		///<summary>Clicked a date on the calendar.</summary>
		private void Calendar2_DateSelected(object sender,EventArgs e) {
			ModuleSelected(monthCalendarOD.GetDateSelected());
		}

		///<summary></summary>
		private void comboView_SelectionChangeCommitted(object sender,EventArgs e) {
			ComboViewChanged();
		}

		private void toggleDayWeek_DayClick(object sender, EventArgs e){
			SetWeeklyView(false);
		}

		private void toggleDayWeek_WeekClick(object sender, EventArgs e){
			SetWeeklyView(true);
		}
		#endregion Methods - Event Handlers PanelCalendar Upper

		#region Methods - Event Handlers PanelCalendar Buttons
		//Left Buttons------------------------------------------------------------------------------------------------
		private void butBreak_Click() {
			if(PrefC.GetBool(PrefName.BrokenApptAdjustment) 
				&& PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType)==0) 
			{
				//They want broken appointment adjustments but don't have it set up.
				MsgBox.Show(this,"Broken appointment adjustment type is not setup yet.  Please go to Setup | Appointment | Appts Preferences to fix this.");
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Appointment appointment = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) {
				return;
			}
			Patient patient=Patients.GetPat(appointment.PatNum);
			if(appointment.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentEdit)) {//separate permissions for completed appts.
				return;
			}
			if(appointment.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentCompleteEdit)) {
				return;
			}
			if(appointment.AptStatus == ApptStatus.PtNote || appointment.AptStatus == ApptStatus.PtNoteCompleted) {
				MsgBox.Show(this,"Patient Notes cannot be broken.");
				return;
			}
			ProcedureCode procedureCode=null;//Will not chart if it stays null.
			ApptBreakSelection apptBreakSelection=ApptBreakSelection.None;
			bool hasBrokenProcs=AppointmentL.HasBrokenApptProcs();//When true, we show FormApptBreak.cs
			if(hasBrokenProcs) {//If true, user cannot get here from right click 'Break Appointment' directly.
				using FormApptBreak formApptBreak=new FormApptBreak(appointment);
				if(formApptBreak.ShowDialog()!=DialogResult.OK) {
					if(formApptBreak.ApptBreakSelection_==ApptBreakSelection.Delete) {
						//User wants to delete the appointment.
						butDelete_Click(showPrompt:false);
					}
					return;
				}
				procedureCode=formApptBreak.ProcedureCodeSelected;
				apptBreakSelection=formApptBreak.ApptBreakSelection_;
			}
			else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Break appointment?")) {
				return;
			}
			//This hook is specifically called after we know a valid appointment has been identified.
			Plugins.HookAddCode(this, "ContrAppt.OnBreak_Click_validation_end",appointment);
			AppointmentL.BreakApptHelper(appointment,patient,procedureCode);
			if(hasBrokenProcs) {//FormApptBreak was shown and user made a selection
				switch(apptBreakSelection) {
					case ApptBreakSelection.Unsched:
						if(AppointmentL.ValidateApptUnsched(appointment)) {
							AppointmentL.SetApptUnschedHelper(appointment,patient,false);
						}
						break;
					case ApptBreakSelection.Pinboard:
						if(AppointmentL.ValidateApptToPinboard(appointment)) {
							AppointmentL.CopyAptToPinboardHelper(appointment);
						}
						break;
					case ApptBreakSelection.ApptBook:
						//Intentionally blank.
						break;
				}
			}
			ModuleSelected(patient.PatNum);//Must be ran after the "D9986" break logic due to the addition of a completed procedure.
			Plugins.HookAddCode(this,"ContrAppt.OnBreak_Click_end",appointment,_patient);
		}

		private void butComplete_Click() {
			if(!Security.IsAuthorized(EnumPermType.AppointmentEdit)) {
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Appointment appointment = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) {
				return;
			}
			Patient patient=Patients.GetPat(appointment.PatNum);
			if(appointment.AptDateTime.Date>DateTime.Today) {
				if(!PrefC.GetBool(PrefName.ApptAllowFutureComplete)){
					MsgBox.Show(this,"Not allowed to set future appointments complete.");
					return;
				}
			}
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
			List<string> listHiddenProcCodes=ProcedureCodes.GetProcCodesInHiddenCats(listProcedures.Select(x => x.CodeNum).ToArray());
			if(listHiddenProcCodes.Count > 0) {
				MsgBox.Show(Lan.g(this,"Cannot complete appointment because the following procedures are in a hidden category:")+" "+string.Join(", ",listHiddenProcCodes));
				return;
			}
			if(appointment.AptStatus!=ApptStatus.PtNote && appointment.AptStatus!=ApptStatus.PtNoteCompleted  //Ptnote cannot have procs attached
				&& !PrefC.GetBool(PrefName.ApptAllowEmptyComplete)//Appointments must have at least 1 proc
				&& listProcedures.Count==0)
			{
				MsgBox.Show(this,"Appointments without procedures attached cannot be set complete.");
				return;
			}
			if(appointment.AptStatus == ApptStatus.PtNoteCompleted) {
				return;
			}
			if(ProcedureCodes.DoAnyBypassLockDate()) {
				for(int i=0;i<listProcedures.Count;i++) {
					if(!Security.IsAuthorized(EnumPermType.ProcComplCreate,appointment.AptDateTime,listProcedures[i].CodeNum,listProcedures[i].ProcFee)) {
						return;
					}
				}
			}
			else if(!Security.IsAuthorized(EnumPermType.ProcComplCreate,appointment.AptDateTime)) {
				return;
			}
			if(listProcedures.Count>0 && appointment.AptDateTime.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Not allowed to set procedures complete with future dates.");
				return;
			}
			#region Provider Term Date Check
			//Prevents appointments with providers that are past their term end date from being completed
			string message=Providers.CheckApptProvidersTermDates(appointment,isSetComplete:true);
			if(message!="") {
				MsgBox.Show(message);
				return;
			}
			#endregion Provider Term Date Check
			bool removeCompletedProcs=ProcedureL.DoRemoveCompletedProcs(appointment,listProcedures.FindAll(x => x.ProcStatus==ProcStat.C));
			ApptStatus apptStatusOld=appointment.AptStatus;
			ODTuple<Appointment,List<Procedure>> result=Appointments.CompleteClick(appointment,listProcedures,removeCompletedProcs);
			appointment=result.Item1;
			listProcedures=result.Item2;
			if(apptStatusOld!=ApptStatus.Complete && appointment.AptStatus==ApptStatus.Complete) {
				AutomationL.Trigger(EnumAutomationTrigger.ApptComplete,null,appointment.PatNum);
			}
			if(appointment.AptStatus!=ApptStatus.PtNote) {
				AutomationL.Trigger(EnumAutomationTrigger.ProcedureComplete,listProcedures.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList(),appointment.PatNum);
			}
			List<Procedure> listProceduresForAppt=Procedures.GetProcsForSingle(appointment.AptNum,false); //The procedures were never updated, fetch from db.
			Procedures.AfterProcsSetComplete(listProcedures);
			ModuleSelected(appointment.PatNum);
			ODEvent.Fire(ODEventType.AppointmentEdited,appointment);
			//If necessary, prompt the user to ask the patient to opt in to using Short Codes.
			FrmShortCodeOptIn.PromptIfNecessary(patient,appointment.ClinicNum);
			Plugins.HookAddCode(this,"ContrAppt.OnComplete_Click_end",appointment,_patient);
			Signalods.SetInvalid(InvalidType.BillingList);
		}

		private void butDelete_Click(bool showPrompt) {
			if(contrApptPanel.SelectedAptNum==-1){
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Appointment appointment = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) { return; }
			if((appointment.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentDelete))
				|| (appointment.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentCompleteDelete))) 
			{
				return;
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow==null){
				MsgBox.Show(this,"Appointment not found.");
				return;
			}
			if(AppointmentL.DoPreventChangesToCompletedAppt(appointment,PreventChangesApptAction.Delete)) {
				return;
			}
			if(appointment.AptStatus == ApptStatus.PtNote | appointment.AptStatus == ApptStatus.PtNoteCompleted) {
				if(showPrompt && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Patient Note?")) {
					return;
				}
				if(appointment.Note != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(appointment.Note,appointment.AptStatus),Lan.g(this,"Question..."),MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog commlog = new Commlog();
						commlog.PatNum = appointment.PatNum;
						commlog.CommDateTime = DateTime.Now;
						commlog.CommType =Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						commlog.Note = Lan.g(this,"Deleted Patient NOTE from schedule, saved copy: ");
						commlog.Note += appointment.Note;
						commlog.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(commlog);
					}
				}
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,_patient.PatNum,
					dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+Lan.g(this,"NOTE Deleted"),
					appointment.AptNum,appointment.DateTStamp);
			}
			else {
				if(showPrompt && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Appointment?")) {
					return;
				}
				if(appointment.Note != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(appointment.Note,appointment.AptStatus),Lan.g(this,"Question..."),MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog commlog = new Commlog();
						commlog.PatNum = appointment.PatNum;
						commlog.CommDateTime = DateTime.Now;
						commlog.CommType =Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						commlog.Note = "Deleted Appointment & saved note: ";
						if(appointment.ProcDescript != "") {
							commlog.Note += appointment.ProcDescript + ": ";
						}
						commlog.Note += appointment.Note;
						commlog.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(commlog);
					}
				}
				if(appointment.AptStatus==ApptStatus.Complete) {// seperate log entry for editing completed appointments.
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteDelete,_patient.PatNum,
						dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+Lan.g(this,"Deleted"),
						appointment.AptNum,appointment.DateTStamp);
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentDelete,_patient.PatNum,
						dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+Lan.g(this,"Deleted"),
						appointment.AptNum,appointment.DateTStamp);
				}
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S17 - Appt Deletion event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(_patient,Patients.GetPat(_patient.Guarantor),EventTypeHL7.S17,appointment);
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
			}
			if(!DoApptBreakRequired(appointment)) {
				return;
			}
			Appointments.Delete(contrApptPanel.SelectedAptNum,true);//Appointments S-Class handles Signalods
			ODEvent.Fire(ODEventType.AppointmentEdited,appointment);
			contrApptPanel.SelectedAptNum=-1;
			pinBoard.SelectedIndex=-1;
			for(int i=0;i<pinBoard.ListPinBoardItems.Count;i++) {
				if(appointment.AptNum==pinBoard.ListPinBoardItems[i].AptNum) {
					pinBoard.ClearAt(i);
				}
			}
			if(_patient==null) {
				ModuleSelected(0);
			}
			else {
				ModuleSelected(_patient.PatNum);
			}
			Recalls.SynchScheduledApptFull(appointment.PatNum);
			Plugins.HookAddCode(this,"ContrAppt.OnDelete_Click_end",appointment,_patient);
		}

		///<summary>Sends current appointment to unscheduled list.</summary>
		private void butUnsched_Click() {
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Appointment appointment = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) {
				return;
			}
			if(appointment.AptStatus==ApptStatus.PtNote || appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				MsgBox.Show(this,"Patient Notes cannot be sent to the Unscheduled List.");
				return;
			}
			if(!AppointmentL.ValidateApptUnsched(appointment)) {
				return;
			}
			if(PrefC.GetBool(PrefName.UnscheduledListNoRecalls) && Appointments.IsRecallAppointment(appointment)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Recall appointments cannot be sent to the Unscheduled List.\r\nDelete appointment instead?")) {
					butDelete_Click(showPrompt:false);
				}
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send Appointment to Unscheduled List?")) {
				return;
			}	
			Patient patient=Patients.GetPat(appointment.PatNum);
			if(!DoApptBreakRequired(appointment,patient)) {
				return;
			}
			AppointmentL.SetApptUnschedHelper(appointment,patient);
			ModuleSelected(patient.PatNum);
			Plugins.HookAddCode(this,"ContrAppt.OnUnsched_Click_end",appointment,_patient);
		}

		//Confirmation list------------------------------------------------------------------------------------------
		private void ListConfirmed_MouseDown(object sender, MouseEventArgs e){
			if(listConfirmed.IndexFromPoint(e.X,e.Y)==-1) {
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				return;
			}
			Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(appointment==null) {
				MsgBox.Show(this,"Patient appointment was removed.");
				contrApptPanel.SelectedAptNum=-1;
				ModuleSelected(_patient.PatNum);//keep same pat
				return;
			}
			Appointment appointmentOld=appointment.Copy();
			long newStatus=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[listConfirmed.IndexFromPoint(e.X,e.Y)].DefNum;
			Appointments.SetConfirmed(appointment,newStatus);//Appointments S-Class handles Signalods
			if(newStatus!=appointmentOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,Lan.g(this,"Appointment confirmation status changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" "+Lan.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,newStatus)
					+" "+Lans.g(this,"from the appointment module")+".",contrApptPanel.SelectedAptNum,appointmentOld.DateTStamp);
			}
			RefreshPeriod();
			//Need to pass in aptOld since we compare the appointment's old confirmed status to the new confirmed status to help determine if the kiosk manager should be shown.
			AppointmentL.ShowKioskManagerIfNeeded(appointmentOld,newStatus);
		}

		//Right Make Buttons----------------------------------------------------------------------------------------
		private void butFamRecall_Click(object sender,EventArgs e) {
			if(_patient==null) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(Appointments.HasOutstandingAppts(_patient.PatNum)) {
				DisplayOtherDlg(false);
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());//not shown
			formApptsOther.IsInitialDoubleClick=false;
			formApptsOther.MakeRecallFamily();
			if(formApptsOther.DialogResult!=DialogResult.OK) {
				return;
			}
			SendToPinBoardAptNums(formApptsOther.ListAptNumsSelected);
			RefreshPeriod(listPinApptNums:formApptsOther.ListAptNumsSelected);
			if(contrApptPanel.IsWeeklyView) {
				return;
			}
			dateSearch.Text=formApptsOther.StringDateJumpTo;
			if(!groupSearch.Visible) {//if search not already visible
				ShowSearch();
			}
			DoSearch();
		}

		private void butMakeAppt_Click(object sender,System.EventArgs e) {
			if(_patient==null) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Patient patient=AppointmentL.GetPatientMergePrompt(_patient.PatNum);
			if(patient!=null) {
				_patient=patient;
				RefreshModuleDataPatient(_patient.PatNum);
				GlobalFormOpenDental.PatientSelected(_patient,isRefreshCurModule: true,isApptRefreshDataPat: false);
			}
			if(_patient!=null && _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(Lans.g(this,"Appointments cannot be scheduled for")+" "+_patient.PatStatus.ToString().ToLower()+" "+Lans.g(this,"patients."));
				return;
			}
			if(Appointments.HasOutstandingAppts(_patient.PatNum)) {
				DisplayOtherDlg(false);
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());//not shown
			CheckStatus();
			formApptsOther.IsInitialDoubleClick=false;
			formApptsOther.MakeAppointment();
			SendToPinBoardAptNums(formApptsOther.ListAptNumsSelected);
			RefreshPeriod(listPinApptNums:formApptsOther.ListAptNumsSelected);
		}

		private void butMakeRecall_Click(object sender,EventArgs e) {
			if(_patient==null) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Patient patient=AppointmentL.GetPatientMergePrompt(_patient.PatNum);
			if(patient!=null) {
				_patient=patient;
				RefreshModuleDataPatient(_patient.PatNum);
				GlobalFormOpenDental.PatientSelected(_patient,isRefreshCurModule: true,isApptRefreshDataPat: false);
			}
			if(_patient!=null && _patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(Lans.g(this,"Appointments cannot be scheduled for")+" "+_patient.PatStatus.ToString().ToLower()+" "+Lans.g(this,"patients."));
				return;
			}
			if(Appointments.HasOutstandingAppts(_patient.PatNum,true)) {
				DisplayOtherDlg(false);
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());//not shown
			formApptsOther.IsInitialDoubleClick=false;
			formApptsOther.MakeRecallAppointment();
			if(formApptsOther.DialogResult!=DialogResult.OK) {
				return;
			}
			SendToPinBoardAptNums(formApptsOther.ListAptNumsSelected);
			RefreshPeriod(listPinApptNums:formApptsOther.ListAptNumsSelected);
			if(contrApptPanel.IsWeeklyView) {
				return;
			}
			dateSearch.Text=formApptsOther.StringDateJumpTo;
			if(!groupSearch.Visible) {//if search not already visible
				ShowSearch();
			}
			DoSearch(isForMakeRecall:true);
		}

		private void butViewAppts_Click(object sender,EventArgs e) {
			DisplayOtherDlg(false);
		}
		#endregion Methods - Event Handlers PanelCalendar Buttons

		#region Methods - Event Handlers PinBoard
		private void butClearPin_Click(object sender,EventArgs e) {
			if(pinBoard.ListPinBoardItems.Count==0) {
				MsgBox.Show(this,"There are no appointments on the pinboard to clear.");
				return;
			}
			DataRow dataRow;
			int idx;
			if(pinBoard.ListPinBoardItems.Count==1) {
				dataRow=pinBoard.ListPinBoardItems[0].DataRowAppt;//even if unselected
				idx=0;
			}
			else{//multiple items on pinboard
				if(pinBoard.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select an appointment first.");
					return;
				}
				dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
				idx=pinBoard.SelectedIndex;
			}
			DateTime dateTime=PIn.DateT(dataRow["AptDateTime"].ToString());
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			ApptStatus aptStatus=(ApptStatus)PIn.Int(dataRow["AptStatus"].ToString());
			if(aptStatus==ApptStatus.UnschedList) {//unscheduled status
				if(dateTime.Year<1880) {//Indicates that this was a brand new appt
					Appointment appointment=Appointments.GetOneApt(aptNum);
					if(appointment==null || appointment.AptDateTime.Year>1880){//If appointment is already deleted or if date is now present
						//don't do anything to db.  Appt removed from pinboard above, and Refresh will happen below.
					}
					else{
						Appointments.Delete(aptNum,true);
					}
				}
				else {//was actually on the unscheduled list
					//do nothing to database
				}
			}
			else if(dateTime.Year>1880) {//already scheduled
				//do nothing to database
			}
			else if(aptStatus==ApptStatus.Planned) {
				//do nothing except remove it from pinboard
			}
			else {//Not sure when this would apply, since new appts start out as unsched.  Maybe patient notes?  Leave it just in case.
				//this gets rid of new appointments that never made it off the pinboard
				Appointments.Delete(aptNum,true);
			}
			pinBoard.ClearAt(idx);
			if(pinBoard.ListPinBoardItems.Count>0) {
				pinBoard.SelectedIndex=pinBoard.ListPinBoardItems.Count-1;
			}
			if(_patient==null) {//not sure how to test this. Doesn't seem possible.
				RefreshModuleScreenButtonsRight();
				return;
			}
			ModuleSelected(_patient.PatNum);
			/*}
			else {
			RefreshModuleDataPatient(pinBoard.ApptList[pinBoard.SelectedIndex].PatNum);
			FormOpenDental.S_Contr_PatientSelected(PatCur,true,false);
			}*/
		}

		private void pinBoard_ApptMovedFromPinboard(object sender,UI.ApptFromPinboardEventArgs e) {
			//Any return from this point forward will cause HideDraggableTempApptSingle();
			//Make sure there are operatories for the appointment to be scheduled and make sure the user dragged the appointment to a valid location.
			if(contrApptPanel.ListOpsVisible.Count==0) {
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			ApptStatus apptStatus=(ApptStatus)PIn.Int(e.DataRowAppt["AptStatus"].ToString());
			long patNum=PIn.Long(e.DataRowAppt["PatNum"].ToString());
			if(apptStatus==ApptStatus.Planned) {//if Planned appt is on pinboard
				if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {//and no permission to create a new appt
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
				if(PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)) {//or pat restricted
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
			}
			//security prevents moving an appointment by preventing placing it on the pinboard, not here
			//We do not ask user, "Move Appointment?" because that's just slow.
			//convert loc to new time
			Appointment appointment=Appointments.GetOneApt(PIn.Long(e.DataRowAppt["AptNum"].ToString()));
			if(appointment==null) {
				MsgBox.Show(this,"This appointment has been deleted since it was moved to the pinboard. It will now be cleared from the pinboard.");
				pinBoard.ClearAt(pinBoard.SelectedIndex);
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			//This hook is specifically called after we know a valid appointment has been identified.
			Plugins.HookAddCode(this, "ContrAppt.pinBoard_MouseUp_validation_end",appointment);//hook name is historical
			Appointment appointmentOld=appointment.Copy();
			RefreshModuleDataPatient(appointment.PatNum);//This is to change _patCur.
			GlobalFormOpenDental.PatientSelected(_patient,true,false);//especially to change name showing in title bar
			if(appointment.IsNewPatient && contrApptPanel.DateSelected!=appointment.AptDateTime.Date) {
				Procedures.SetDateFirstVisit(contrApptPanel.DateSelected,4,_patient);
			}
			//e.Location is in ContrAppt coords
			TimeSpan? timeSpanNew=contrApptPanel.YPosToTime(e.Location.Y-contrApptPanel.Top);//in contrApptPanel coords
			if(timeSpanNew==null){
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			TimeSpan timeSpanNewRounded=UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanNew.Value,contrApptPanel.MinPerIncr);
			contrApptPanel.RoundToNearestDateAndOp(e.Location.X-contrApptPanel.Location.X,//passing in as coordinates of the control
				out DateTime dateNew,
				out int opIdx,e.BitmapAppt.Width);
			if(opIdx<0){
				MsgBox.Show(this,"Invalid operatory");
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			appointment.AptDateTime=dateNew+timeSpanNewRounded;
			//Compare beginning of new appointment against end to see if the appointment spans two days
			if(appointment.AptDateTime.Day!=appointment.AptDateTime.AddMinutes(appointment.Pattern.Length*5).Day) {
				MsgBox.Show(this,"You cannot have an appointment that starts and ends on different days.");
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			//Prevent double-booking
			if(contrApptPanel.IsDoubleBooked(appointment)) {
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			Operatory operatory=contrApptPanel.ListOpsVisible[opIdx];
			appointment.Op=operatory.OperatoryNum;
			if(!appointment.IsHygiene) {//If a non-hygiene appointment is moved, update the IsHygiene value to that of the new operatory.
				appointment.IsHygiene=operatory.IsHygiene;
			}
			//opCur.OperatoryNum;
			//Set providers----------------------Similar to UpdateAppointments()
			long assignedDent=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatory,false,appointment.AptDateTime);
			long assignedHyg=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatory,true,appointment.AptDateTime);
			List<Procedure> procsForSingleApt=null;
			if(appointment.AptStatus!=ApptStatus.PtNote&&appointment.AptStatus!=ApptStatus.PtNoteCompleted) {
				#region Update Appt's DateTimeAskedToArrive
				if(_patient.AskToArriveEarly>0) {
					appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-_patient.AskToArriveEarly);
					MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+_patient.AskToArriveEarly
						+" "+Lan.g(this,"minutes early at")+" "+appointment.DateTimeAskedToArrive.ToShortTimeString()+".");
				}
				else {
					appointment.DateTimeAskedToArrive=DateTime.MinValue;
				}
				#endregion Update Appt's DateTimeAskedToArrive
				#region Update Appt's Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				//if no dentist/hygienist is assigned to spot, then keep the original dentist/hygienist without prompt.  All appts must have prov.
				if((assignedDent!=0&&assignedDent!=appointment.ProvNum)||(assignedHyg!=0&&assignedHyg!=appointment.ProvHyg)) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change provider?")) {
						if(assignedDent!=0) {//the dentist will only be changed if the spot has a dentist.
							appointment.ProvNum=assignedDent;
						}
						if(assignedHyg!=0||PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
							appointment.ProvHyg=assignedHyg;
						}
						if(operatory.IsHygiene) {
							appointment.IsHygiene=true;
						}
						else {//op not marked as hygiene op
							if(assignedDent==0) {//no dentist assigned
								if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
									appointment.IsHygiene=true;
								}
							}
							else {//dentist is assigned
								if(assignedHyg==0) {//hyg is not assigned
									appointment.IsHygiene=false;
								}
								//if both dentist and hyg are assigned, it's tricky
								//only explicitly set it if user has a dentist assigned to the op
								if(operatory.ProvDentist!=0) {
									appointment.IsHygiene=false;
								}
							}
						}
						bool isplanned=appointment.AptStatus==ApptStatus.Planned;
						procsForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,isplanned);
						List<long> codeNums=new List<long>();
						for(int p = 0;p<procsForSingleApt.Count;p++) {
							codeNums.Add(procsForSingleApt[p].CodeNum);
						}
						bool doMake5Minute=procsForSingleApt.Count>0;//Appointments without procs are already returned in 5 minute increments.
						string calcPattern=Appointments.CalculatePattern(appointment.ProvNum,appointment.ProvHyg,codeNums,doMake5Minute);
						if(appointment.Pattern!=calcPattern) {
							if(Security.IsAuthorized(EnumPermType.AppointmentResize,suppressMessage:true) && !appointment.TimeLocked) {
								//User is authorized, and appt time not locked. Do not give popup for users without resizing permissions or for Timelocked appointments.
								if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change length for new provider?")) {
									appointment.Pattern=calcPattern;
								}
							}
						}
					}
				}
				#region Provider Term Date Check
				//Prevents appointments with providers that are past their term end date from being scheduled
				string message=Providers.CheckApptProvidersTermDates(appointment);
				if(message!="") {
					MessageBox.Show(message);//translated in Providers S class method
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
				#endregion Provider Term Date Check
				#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
			}
			#region Prevent overlap
			//Check for any blockout collisions when overlapping appointments are allowed.
			if(PrefC.GetBool(PrefName.ApptsAllowOverlap)) {
				if(ShowAppointmentBlockoutMessage(appointment)) {
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
			}
			else {//Appointments are not allowed to overlap so check for both appointment and blockout collisions.
				if(!Appointments.TryAdjustAppointmentOp(appointment,contrApptPanel.ListOpsVisible)) {
					MsgBox.Show(this,"Appointment overlaps existing appointment or blockout.");
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
			}
			#endregion Prevent overlap
			#region Detect Frequency Conflicts
			//Detect frequency conflicts with procedures in the appointment
			DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(_patient.PatNum);
			if(discountPlanSub==null) {
				if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
					procsForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);
					string frequencyConflicts=""; 
					try {
						frequencyConflicts=Procedures.CheckFrequency(procsForSingleApt,appointment.PatNum,appointment.AptDateTime);
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"There was an error checking frequencies.  Disable the Insurance Frequency Checking feature or try to fix the following error:")
							+"\r\n"+ex.Message);
						pinBoard_ApptMovedFromPinboard_Cleanup();
						return;
					}
					if(frequencyConflicts!="" && MessageBox.Show(
						Lan.g(this,"Scheduling this appointment for this date will cause frequency conflicts for the following procedures")
							+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No) 
					{
						pinBoard_ApptMovedFromPinboard_Cleanup();
						return;
					}
				}
			}
			else { 
				procsForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);
				string frequencyDiscountConflicts="";
				try {
					frequencyDiscountConflicts=DiscountPlans.CheckDiscountFrequencyAndValidateDiscountPlanSub(procsForSingleApt,appointment.PatNum,appointment.AptDateTime);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"There was an error checking discount frequencies:")
						+"\r\n"+ex.Message);
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
				if(!string.IsNullOrEmpty(frequencyDiscountConflicts) && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
					+":\r\n"+frequencyDiscountConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
				{
					pinBoard_ApptMovedFromPinboard_Cleanup();
					return;
				}
			}
			#endregion Detect Frequency Conflicts
			#region Patient status
			//Operatory opCur=Operatories.GetOperatory(apptCur.Op);
			Operatory opOld=Operatories.GetOperatory(appointmentOld.Op);
			if(opOld==null||operatory.SetProspective!=opOld.SetProspective) {
				if(operatory.SetProspective && _patient.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
						Patient patientOld=_patient.Copy();
						_patient.PatStatus=PatientStatus.Prospective;
						Patients.UpdateRecalls(_patient,patientOld,"Appointment Module, Appointment moved to prospective operatory");
						Patients.Update(_patient,patientOld);
						string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
							+_patient.PatStatus.GetDescription()+Lan.g(this," by moving the patient appointment to a prospective operatory.");
						SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,logEntry);
					}
				}
				else if(!operatory.SetProspective && _patient.PatStatus==PatientStatus.Prospective) {
					//Do we need to warn about changing FROM prospective? Assume so for now.
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
						Patient patientOld=_patient.Copy();
						_patient.PatStatus=PatientStatus.Patient;
						Patients.UpdateRecalls(_patient,patientOld,"Appointment Module, Appointment moved from prospective operatory");
						Patients.Update(_patient,patientOld);
						string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
							+_patient.PatStatus.GetDescription()+Lan.g(this," by moving the patient appointment from a prospective operatory.");
						SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,logEntry);
					}
				}
			}
			#endregion Patient status
			#region Update Appt's AptStatus, ClinicNum
			if(appointment.AptStatus==ApptStatus.Broken) {
				appointment.AptStatus=ApptStatus.Scheduled;
			}
			if(appointment.AptStatus==ApptStatus.UnschedList) {
				appointment.AptStatus=ApptStatus.Scheduled;
			}
			//original position of provider settings
			if(operatory.ClinicNum==0) {
				appointment.ClinicNum=_patient.ClinicNum;
			}
			else {
				appointment.ClinicNum=operatory.ClinicNum;
			}
			#endregion Update Appt's AptStatus, ClinicNum
			bool isCreate=false;
			#region Update/Insert Appt in db
			if(!AppointmentL.IsSpecialtyMismatchAllowed(_patient.PatNum,appointment.ClinicNum)) {//ClinicNum just set using either opCur or PatCur.
				pinBoard_ApptMovedFromPinboard_Cleanup();
				return;
			}
			if(appointment.AptStatus==ApptStatus.Planned) {//if Planned appt is on pinboard
				#region Planned appointment
				DataTable tableApptFields=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].TableApptFields;
				List<ApptField> listApptFields=new List<ApptField>();
				for(int i = 0;i<tableApptFields.Rows.Count;i++) {
					if(appointmentOld.AptNum!=PIn.Long(tableApptFields.Rows[i]["AptNum"].ToString())) {
						continue;//should never happen
					}
					ApptField apptField = new ApptField();
					apptField.FieldName=PIn.String(tableApptFields.Rows[i]["FieldName"].ToString());
					apptField.FieldValue=PIn.String(tableApptFields.Rows[i]["FieldValue"].ToString());
					//the other two fields are not important
					listApptFields.Add(apptField);
				}
				bool procAlreadyAttached;
				procAlreadyAttached=Appointments.IsProcAlreadyAttached(appointment);//test planned appt
				appointment=Appointments.SchedulePlannedApt(appointment,_patient,listApptFields,appointment.AptDateTime,appointment.Op);//Appointments S-Class handles Signalods
				isCreate=true;
				if(procAlreadyAttached) {
					MsgBox.Show(this,"One or more procedures could not be scheduled because they were already attached to another appointment. Someone probably forgot to update the Next appointment in the Chart module.");
					using FormApptEdit formApptEdit=new FormApptEdit(appointment.AptNum);
					CheckStatus();
					formApptEdit.IsNew=true;
					formApptEdit.ShowDialog();//to force refresh of aptDescript
					if(formApptEdit.DialogResult!=DialogResult.OK) {//apt gets deleted from within aptEdit window.
						RefreshModuleScreenButtonsRight();
						RefreshPeriod();
						pinBoard_ApptMovedFromPinboard_Cleanup();
						return;
					}
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCreate,appointment.PatNum,
						appointment.AptDateTime.ToString()+", "+appointment.ProcDescript,
						appointment.AptNum,appointmentOld.DateTStamp);
				}
				procsForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,false);
				#endregion Planned appointment
			}
			else {//simple drag off pinboard to a new date/time
				#region Previously scheduled appointment (not a planned appointment)
				appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
				Appointments.Update(appointment,appointmentOld);//Appointments S-Class handles Signalods
				Appointments.TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);
				if(appointmentOld.AptStatus==ApptStatus.UnschedList&&appointmentOld.AptDateTime==DateTime.MinValue) { //If new appt is being added to schedule from pinboard
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCreate,appointment.PatNum,
						appointment.AptDateTime.ToString()+", "+appointment.ProcDescript,
						appointment.AptNum,appointmentOld.DateTStamp);
					isCreate=true;
				}
				else { //If existing appt is being moved
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentMove,appointment.PatNum,
						appointment.ProcDescript+", from "+appointmentOld.AptDateTime.ToString()+", to "+appointment.AptDateTime.ToString(),
						appointment.AptNum,appointmentOld.DateTStamp);
					if(appointmentOld.AptStatus==ApptStatus.UnschedList) {
						isCreate=true;
					}
				}
				if(appointment.Confirmed!=appointmentOld.Confirmed) {
						//Log confirmation status changes.
						SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,
							Lan.g(this,"Appointment confirmation status automatically changed from ")
							+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
							+Lan.g(this," from the appointment module")+".",appointment.AptNum,appointmentOld.DateTStamp);
				}
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S12 - New Appt Booking event, S13 - Appt Rescheduling
					MessageHL7 messageHL7=null;
					if(isCreate) {
						messageHL7=MessageConstructor.GenerateSIU(_patient,Patients.GetPat(_patient.Guarantor),EventTypeHL7.S12,appointment);
					}
					else {
						messageHL7=MessageConstructor.GenerateSIU(_patient,Patients.GetPat(_patient.Guarantor),EventTypeHL7.S13,appointment);
					}
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
			}
			#endregion Previously scheduled appointment (not a planned appointment)
			#endregion Update/Insert Appt in db
			if(procsForSingleApt==null) {
				procsForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,false);
			}
			#region Update UI and cache
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(appointment.PatNum);
			bool isUpdatingFees=false;
			List<Procedure> listProcedures=procsForSingleApt.Select(x => Procedures.ChangeProcInAppointment(appointment,x.Copy())).ToList();
			if(procsForSingleApt.Exists(x => x.ProvNum!=listProcedures.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
				string promptText="";
				isUpdatingFees=Procedures.ShouldFeesChange(listProcedures,procsForSingleApt,ref promptText,procFeeHelper);
				if(isUpdatingFees) {//Made it pass the pref check.
					if(promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
							isUpdatingFees=false;
					}
				}
			}
			Procedures.SetProvidersInAppointment(appointment,procsForSingleApt,isUpdatingFees,procFeeHelper);
			pinBoard.ClearAt(pinBoard.SelectedIndex);
			contrApptPanel.SelectedAptNum=appointment.AptNum;
			//SetDateSelected(apptCur.AptDateTime);
			//RefreshPeriod(isRefreshSchedules:true);//date moving to for this computer; This line may not be needed
			RefreshPeriod();
			RefreshModuleScreenButtonsRight();
			if(isCreate) {//new appointment is being added to the schedule from the pinboard, trigger ScheduleProcedure automation
				List<string> procCodes=procsForSingleApt.Select(x => ProcedureCodes.GetProcCode(x.CodeNum).ProcCode).ToList();					
				AutomationL.Trigger(EnumAutomationTrigger.ProcSchedule,procCodes,appointment.PatNum);
			}
			ODEvent.Fire(ODEventType.AppointmentEdited,appointment);
			Recalls.SynchScheduledApptFull(appointment.PatNum);
			Plugins.HookAddCode(this,"ContrAppt.pinBoard_MouseUp_end",appointment,_patient);//hook name is historical
			#endregion Update UI and cache
			pinBoard_ApptMovedFromPinboard_Cleanup();
		}

		private void pinBoard_ApptMovedFromPinboard_Cleanup(){
			pinBoard.HideDraggableTempApptSingle();
			if(pinBoard.ListPinBoardItems.Count>0) {//If there are any more items in the pinboard, update the selected index.
				pinBoard.SelectedIndex=pinBoard.ListPinBoardItems.Count-1;
			}
		}

		private void pinBoard_ModuleNeedsRefresh(object sender, EventArgs e){
			long pinAptNum=0;
			if(pinBoard.SelectedIndex!=-1){
				pinAptNum=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].AptNum;
			}
			List<long> listOpNums=null;
			List<long> listProvNums=null;
			if(Clinics.ClinicNum!=0 || !ApptViews.IsNoneView(GetApptViewCur())) {
				listOpNums=contrApptPanel.ListOpsVisible.Select(x => x.OperatoryNum).ToList();
				listProvNums=contrApptPanel.ListProvsVisible.Select(x => x.ProvNum).ToList();
			}
			ModuleSelected(_patient.PatNum,listOpNums:listOpNums,listProvNums:listProvNums);
			if(pinAptNum!=0){
				SendToPinBoardAptNums(new List<long>(){pinAptNum});
			}
		}

		private void pinBoard_PreparingToDragFromPinboard(object sender,UI.ApptDataRowEventArgs e) {
			string pattern=PIn.String(e.DataRowAppt["Pattern"].ToString());
			string patternShowing=contrApptPanel.GetPatternShowing(pattern);
			SizeF sizeAppt=contrApptPanel.SetSize(pattern);
			if(sizeAppt.Width==0){
				pinBoard.SetBitmapTempPinAppt(null);//tells pinboard that we can't drag appt off.
				return;
			}
			//the whole point of having all of this here is to set the width of the dragging appt to be same as in the main area instead of like pinboard
			Bitmap bitmap=new Bitmap((int)sizeAppt.Width,(int)sizeAppt.Height);
			using(Graphics g = Graphics.FromImage(bitmap)){
				contrApptPanel.GetBitmapForPinboard(g,e.DataRowAppt,patternShowing,bitmap.Width,bitmap.Height);
			}
			pinBoard.SetBitmapTempPinAppt(bitmap);
			bitmap.Dispose();//?
		}

		private void pinBoard_SelectedIndexChanged(object sender,EventArgs e) {
			contrApptPanel.SelectedAptNum=-1;
			RefreshModuleScreenButtonsRight();
		}
		#endregion Methods - Event Handlers PinBoard

		#region Methods - Event Handlers LR Tabs
		private void GridEmpOrProv_DoubleClick(object sender, EventArgs e){
			if(contrApptPanel.IsWeeklyView) {
				MsgBox.Show(this,"Not available in weekly view");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.Schedules)) {
				return;
			}
			using FormScheduleDayEdit formScheduleDayEdit=new FormScheduleDayEdit(contrApptPanel.DateSelected,Clinics.ClinicNum);
			formScheduleDayEdit.ShowOkSchedule=true;
			formScheduleDayEdit.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,"");
			RefreshPeriod();
			if(formScheduleDayEdit.GotoScheduleOnClose) {
				using FormSchedule formSchedule = new FormSchedule();
				formSchedule.ShowDialog();
			}
		}

		///<summary>Logic mimics UserControlTasks.gridMain_CellDoubleClick()</summary>
		private void gridReminders_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==0){
				//no longer allow double click on checkbox, because it's annoying.
				return;
			}
			GridRow row=gridReminders.ListGridRows[e.Row];
			Task taskReminder=((Task)row.Tag);
			//It's important to grab the task directly from the db because the status in this list is fake, being the "unread" status instead.
			Task task=Tasks.GetOne(taskReminder.TaskNum);
			FormTaskEdit formTaskEdit=new FormTaskEdit(task);
			formTaskEdit.Show();//non-modal
		}

		///<summary>The logic for this function was copied from UserControlTasks.gridMain_MouseDown() and modified slightly for this scenaro.</summary>
		private void gridReminders_MouseDown(object sender,MouseEventArgs e) {
			int clickedI=gridReminders.PointToRow(e.Y);
			int clickedCol=gridReminders.PointToCol(e.X);
			if(clickedI==-1){
				return;
			}
			gridReminders.SetSelected(clickedI,true);//if right click.
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			GridRow row=gridReminders.ListGridRows[clickedI];
			Task taskReminder=((Task)row.Tag).Copy();
			if(clickedCol==0){//check tasks off
				if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
					long userNumInbox=TaskLists.GetMailboxUserNum(taskReminder.TaskListNum);
					if(userNumInbox != 0 && userNumInbox != Security.CurUser.UserNum) {
						MsgBox.Show(this,"Not allowed to mark off tasks in someone else's inbox.");
						return;
					}
					//might not need to go to db to get this info 
					//might be able to check this:
					//if(task.IsUnread) {
					//But seems safer to go to db.
					if(TaskUnreads.IsUnread(Security.CurUser.UserNum,taskReminder)) {
						TaskUnreads.SetRead(Security.CurUser.UserNum,taskReminder);
						taskReminder.TaskStatus=TaskStatusEnum.Viewed;
						gridReminders.BeginUpdate();
						SetReminderGridRow(row,taskReminder);//To get the status to immediately show up in the reminders grid.
						gridReminders.EndUpdate();
						long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskReminder.TaskNum);
						UserControlTasks.RefillLocalTaskGrids(taskReminder,TaskNotes.GetForTask(taskReminder.TaskNum),new List<long>() { signalNum });
					}
					return;
					//if already read, nothing else to do.  If done, nothing to do
				}
				if(taskReminder.TaskStatus==TaskStatusEnum.New) {
					Task taskOld=taskReminder.Copy();
					taskReminder.TaskStatus=TaskStatusEnum.Viewed;
					try {
						Tasks.Update(taskReminder,taskOld);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}//no longer allowed to mark done from here
					gridReminders.BeginUpdate();
					SetReminderGridRow(row,taskReminder);//To get the status to immediately show up in the reminders grid.
					gridReminders.EndUpdate();
					long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskReminder.TaskNum);
					UserControlTasks.RefillLocalTaskGrids(taskReminder,TaskNotes.GetForTask(taskReminder.TaskNum),new List<long>() { signalNum });
				}
			}
		}

		///<summary>Logic mimics UserControlTasks.DoneClicked(). Code copied from version 19.2 ContrAppt.cs.</summary>
		private void menuItemReminderDone_Click(object sender,EventArgs e) {
			if(gridReminders.GetSelectedIndex()==-1) {
				return;
			}
			Task task=(Task)gridReminders.ListGridRows[gridReminders.GetSelectedIndex()].Tag;
			Task oldTask=task.Copy();
			task.TaskStatus=TaskStatusEnum.Done;
			if(task.DateTimeFinished.Year<1880) {
				task.DateTimeFinished=DateTime.Now;
			}
			try {
				Tasks.Update(task,oldTask);
			}
			catch(Exception ex) {
				//Revert the changes to the task because something went wrong.
				task.TaskStatus=oldTask.TaskStatus;
				task.DateTimeFinished=oldTask.DateTimeFinished;
				MessageBox.Show(ex.Message);
				return;
			}
			TaskUnreads.DeleteForTask(task);
			TaskHist taskHist=new TaskHist(oldTask);
			taskHist.UserNumHist=Security.CurUser.UserNum;
			TaskHists.Insert(taskHist);
			long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,task.TaskNum);
			UserControlTasks.RefillLocalTaskGrids(task,TaskNotes.GetForTask(task.TaskNum),new List<long>() { signalNum });
			gridReminders.BeginUpdate();
			gridReminders.ListGridRows.RemoveAt(gridReminders.GetSelectedIndex());
			gridReminders.EndUpdate();
		}

		///<summary>Logic mimics UserControlTasks.GoTo_Clicked(). Code copied from version 19.2 ContrAppt.cs.</summary>
		private void menuItemReminderGoto_Click(object sender,EventArgs e) {
			if(gridReminders.GetSelectedIndex()==-1) {
				return;
			}
			Task task=(Task)gridReminders.ListGridRows[gridReminders.GetSelectedIndex()].Tag;
			FormOpenDental.S_TaskGoTo(task.ObjectType,task.KeyNum);
		}

		private void timerWaitingRoom_Tick(object sender,EventArgs e) {
			FillWaitingRoom();
		}
		#endregion Methods - Event Handlers LR Tabs

		#region Methods - Event Handlers Menu Popup
		private Appointment BreakApptHelper(object sender,bool suppressModuleSelected=false) {
			ToolStripMenuItem menuItem=(ToolStripMenuItem)sender;
			ToolStripItem menuItemParent=menuItem.OwnerItem;
			Appointment appointment=Appointments.GetOneApt((long)menuItemParent.Tag);//Refresh since they could of waited to interact with menu.
			if(appointment==null) {//This can happen if another user deleted the appt just after the current user right clicked on the appt.
				MsgBox.Show(this,"Appointment not found.");
				return null;
			}
			if(appointment.AptStatus==ApptStatus.PtNote || appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				MsgBox.Show(this,"Patient Notes cannot be broken.");
				return null;
			}
			if(AppointmentL.DoPreventChangesToCompletedAppt(appointment,PreventChangesApptAction.Break)) {
				return null;
			}
			ProcedureCode procedureCode=(ProcedureCode)menuItem.Tag;
			Patient patient=Patients.GetPat(appointment.PatNum);
			AppointmentL.BreakApptHelper(appointment,patient,procedureCode);
			if(!suppressModuleSelected) {
				ModuleSelected(patient.PatNum);
			}
			return appointment;
		}

		private void menuApt_Opening(object sender,CancelEventArgs e) {
			if(toolStripMenuItem==null) {
				return;
			}
			toolStripMenuItem.DropDownItems.Clear();
			toolStripMenuItem.Tag=contrApptPanel.SelectedAptNum;//Refresh later, just in case.
			ToolStripItem item=null;
			bool dontAllowUnscheduled=PrefC.GetBool(PrefName.UnscheduledListNoRecalls) 
				&& Appointments.IsRecallAppointment(Appointments.GetOneApt(contrApptPanel.SelectedAptNum));
			BrokenApptProcedure brokenApptProcs=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			if(brokenApptProcs.In(BrokenApptProcedure.Missed,BrokenApptProcedure.Both)) {
				if(dontAllowUnscheduled) {
					item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Missed - Delete Appointment"),null,menuBreakDelete_Click);
				}
				else {
					item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Missed - Send to Unscheduled List"),null,menuBreakToUnsched_Click);
				}
				item.Tag=ProcedureCodes.GetProcCode("D9986");
				item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Missed - Copy To Pinboard"),null,menuBreakToPin_Click);
				item.Tag=ProcedureCodes.GetProcCode("D9986");
				item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Missed - Leave on Appt Book"),null,menuBreak_Click);
				item.Tag=ProcedureCodes.GetProcCode("D9986");
			}
			if(brokenApptProcs.In(BrokenApptProcedure.Cancelled,BrokenApptProcedure.Both)) {				
				if(toolStripMenuItem.DropDownItems.Count > 0) {
					toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
				}
				if(dontAllowUnscheduled) {
					item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Cancelled - Delete Appointment"),null,menuBreakDelete_Click);
				}		
				else {
					item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Cancelled - Send to Unscheduled List"),null,menuBreakToUnsched_Click);
				}
				item.Tag=ProcedureCodes.GetProcCode("D9987");
				item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Cancelled - Copy To Pinboard"),null,menuBreakToPin_Click);
				item.Tag=ProcedureCodes.GetProcCode("D9987");
				item=toolStripMenuItem.DropDownItems.Add(Lan.g(this,"Cancelled - Leave on Appt Book"),null,menuBreak_Click);
				item.Tag=ProcedureCodes.GetProcCode("D9987");
			}
			toolStripMenuItem.Click-=menuApt_Click;//if there are items in DropDownItems, clicking on "Break Appointment" should not fire menuApt_Click, just expand menu
			if(toolStripMenuItem.DropDownItems.Count==0) {
				toolStripMenuItem.Click+=menuApt_Click;
			}
		}

		private void menuBreak_Click(object sender,EventArgs e) {
			BreakApptHelper(sender);
		}

		private void menuBreakDelete_Click(object sender,EventArgs e) {
			Appointment appointment=BreakApptHelper(sender,true);
			if(appointment!=null) {
				DisplayFormAsapForWebSched(appointment);
				butDelete_Click(showPrompt:false);
			}
		}

		private void menuBreakToPin_Click(object sender,EventArgs e) {
			Appointment appointment=BreakApptHelper(sender);
			if(appointment!=null && AppointmentL.ValidateApptToPinboard(appointment)) {
				DisplayFormAsapForWebSched(appointment);
				AppointmentL.CopyAptToPinboardHelper(appointment);
			}
		}

		private void menuBreakToUnsched_Click(object sender,EventArgs e) {
			Appointment appointment=BreakApptHelper(sender,true);
			if(appointment!=null && AppointmentL.ValidateApptUnsched(appointment)) {
				DisplayFormAsapForWebSched(appointment);
				AppointmentL.SetApptUnschedHelper(appointment);
				ModuleSelected(appointment.PatNum);
			}
		}

		#endregion Methods - Event Handlers Menu Popup

		#region Methods - Event Handlers Menu Apt Click
		private void menuApt_Click(object sender,System.EventArgs e) {
			Appointment appointment;
			switch(((ToolStripMenuItem)sender).Name) {
				case MenuItemNames.CopyToPinboard: //Menu: Copy to Pinboard
					CopyToPin_Click();
					//If pref on, refresh pinboard right away to show broken appt.
					if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove)) {
						RefreshPinboardImages();
					}
					break;
				case MenuItemNames.CopyAppointmentStructure: //Menu: Copy Appointment Structure
					CopyApptStructure(Appointments.GetOneApt(contrApptPanel.SelectedAptNum));
					break;
				//1: divider
				case MenuItemNames.SendToUnscheduledList: //Menu: Send to Unscheduled List
					butUnsched_Click();
					break;
				case MenuItemNames.BreakAppointment: //Menu: Break Appointment
					butBreak_Click();
					break;
				case MenuItemNames.MarkAsAsap:
					ASAP_Click();
					break;
				case MenuItemNames.SetComplete: // Menu: Set Complete
					butComplete_Click();
					break;
				case MenuItemNames.Delete: // Menu: Delete
					butDelete_Click(showPrompt:true);
					break;
				case MenuItemNames.EClipboardQR:
					EClipboardQR_Click(sender, e);
					break;
				case MenuItemNames.PatientAppointments: // Menu: Patient Appointments
					DisplayOtherDlg(false);
					break;
				//8: divider
				case MenuItemNames.PrintLabel: // Menu: Print Label
					PrintApptLabel();
					break;
				case MenuItemNames.PrintCard: // Menu: Print Card
					_isPrintCardFamily=false;
					PrintApptCard();
					break;
				case MenuItemNames.PrintCardEntireFamily: // Menu: Print Card for Entire Family
					_isPrintCardFamily=true;
					PrintApptCard();
					break;
				case MenuItemNames.RoutingSlip: // Menu: Routing Slip
					//for now, this only allows one type of routing slip.  But it could be easily changed.
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					using(FormRpRouting formRpRouting=new FormRpRouting()) {
						formRpRouting.AptNum=contrApptPanel.SelectedAptNum;
						formRpRouting.DateSelected=contrApptPanel.DateSelected;
						List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.RoutingSlip);
						if(customSheetDefs.Count==0) {
							formRpRouting.SheetDefNum=0;
						}
						else {
							formRpRouting.SheetDefNum=customSheetDefs[0].SheetDefNum;
						}
						formRpRouting.ShowDialog();
					}
					break;
				case MenuItemNames.OrthoChart: //Open Patient Ortho Chart
					using(FormOrthoChart formOrthoChart=new FormOrthoChart(_patient)) {
						formOrthoChart.ShowDialog();
					}
					break;
				case MenuItemNames.HomePhone: //Call Home Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patient.HmPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					 break;
				case MenuItemNames.WorkPhone: //Call Work Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patient.WkPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					break;
				case MenuItemNames.WirelessPhone: //Call Wireless Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patient.WirelessPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					break;
				case MenuItemNames.SendText:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					if(!Security.IsAuthorized(EnumPermType.TextMessageSend)) {
						return;
					}
					FormOpenDental.S_TxtMsg_Click(appointment.PatNum,"");
					break;
				case MenuItemNames.SendConfirmationText:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					Appointment aptOld=appointment.Copy();
					Patient patient=Patients.GetPat(appointment.PatNum);
					string message=PatComm.BuildConfirmMessage(ContactMethod.TextMessage,patient,appointment.DateTimeAskedToArrive,appointment.AptDateTime);
					bool wasTextSent=FormOpenDental.S_TxtMsg_Click(patient.PatNum,message);
					if(wasTextSent) {
						long newStatus=PrefC.GetLong(PrefName.ConfirmStatusTextMessaged);
						Appointments.SetConfirmed(appointment,newStatus);
						appointment.Confirmed=newStatus; //SetConfirmed doesn't set the appt status in memory
						if(appointment.Confirmed!=aptOld.Confirmed) { 
							SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,Lans.g(this,"Appointment confirmation status changed from")+" "
								+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
								+" "+Lans.g(this,"from the appointment module")+".",appointment.AptNum,aptOld.DateTStamp);
						}
						ModuleSelected(patient.PatNum);//Refresh the module so that the current appt status is updated
					}
					break;
				case MenuItemNames.SendComeInText:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					if(!GetArrivalsLoaded().TryGetComeInMsg(appointment,out message)) {
						MsgBox.Show(this,$"Unable to {MenuItemNames.SendComeInText} ");
						return;
					}
					FormOpenDental.S_TxtMsg_Click(appointment.PatNum,message);
					break;
				case MenuItemNames.SendPaymentLinkText:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					patient=Patients.GetPat(appointment.PatNum);
					if(UserWebs.GetByFKeyAndType(_patient.PatNum,UserWebFKeyType.PatientPortal)==null) {
						MsgBox.Show(this,"The patient on this appointment is not signed up for PatientPortal.");
						//return;
					}
					using (FormTextPayLink formTextPayLink=new FormTextPayLink(appointment,patient)){ 
						formTextPayLink.ShowDialog();
						if(formTextPayLink.DialogResult==DialogResult.Cancel) { return; }
						bool didSend=FormOpenDental.S_TxtMsg_Click(appointment.PatNum,formTextPayLink.Message);
						if(didSend) {
							formTextPayLink.StatementCur.SmsSendStatus=AutoCommStatus.SendSuccessful;
							formTextPayLink.StatementCur.IsSent=true;
							Statements.Update(formTextPayLink.StatementCur);
						}
						else {
							try {
								Statements.DeleteStatements(new List<Statement>{ formTextPayLink.StatementCur },true);
							}
							catch(Exception ex) {
								//This shouldn't happen
							}
						}
					}
					break;
				case MenuItemNames.SendEClipboardByod:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					AppointmentL.SendByodLink(appointment);
					break;
				case MenuItemNames.CareCredit:
					CareCredit.ShowPage(CareCredit.ProviderSignupURL);
					break;
				case MenuItemNames.CareCreditAcceptDeclineOffer:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					decimal total=Appointments.GetEstPatientPortion(appointment);
					CareCreditL.LaunchQuickScreenIndividualPage(Patients.GetPat(appointment.PatNum),appointment.ProvNum,appointment.ClinicNum,(double)total);
					break;
				case MenuItemNames.CareCreditApplicationNeeded:
					appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(ApptIsNull(appointment)) { return; }
					total=Appointments.GetEstPatientPortion(appointment);
					CareCreditL.LaunchCreditApplicationPage(Patients.GetPat(appointment.PatNum),appointment.ProvNum,appointment.ClinicNum,(double)total,(double)total);
					break;
				/*case MenuItemNames.BringOverlapToFront:
					ContrApptSheet2.OverlapOrdering.CycleOverlappingAppts(contrApptPanel.SelectedAptNum);//Change priorities
					contrApptPanel.SelectedAptNum=-1;//removed selected appointment as to show changes with overlap ordering
					ContrApptSheet2.DoubleBufferDraw(createApptShadows:true);
					break;*/
			}
		}
		#endregion Methods - Event Handlers Menu Appt Click

		#region Methods - Event Handlers Menu Blockout Click
		/// <summary>Deletes selected web schedule ASAP blockout.</summary>
		private void DeleteWebSchedAsapBlockout_Click(object sender,EventArgs e) {
			Schedule schedule=GetClickedSchedule(ScheduleType.WebSchedASAP);
			if(schedule!=null && MsgBox.Show(this,MsgBoxButtons.OKCancel,"This could cause messages to not be sent.  Are you sure?"
				,MenuItemNames.DeleteWebSchedAsapBlockout))
			{
				Schedules.Delete(schedule,true);
				Schedules.BlockoutLogHelper(BlockoutAction.Delete,schedule);
				RefreshPeriodSchedules();
			}
		}

		private void menuBlockAdd_Click(object sender,EventArgs e) {
			//Pre-calculate the list of Blockout Types to show in FormScheduleBlockEdit
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true);	
			if(!Security.IsAuthorized(EnumPermType.Blockouts,true)) {
				//This is a special case, so we only keep blockouts that are marked as NoSchedule or DontCopy
				listDefsBlockoutTypes.RemoveAll(x => !x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()) 
					&& !x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
				if(listDefsBlockoutTypes.Count==0 && !Security.IsAuthorized(EnumPermType.Blockouts)) {//Intentional for error message
					//Security.IsAuthorized(...) will display the "Not authorized message."
					return;
				}
			}
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateTimeClickedBlockout.Date;
			schedule.StartTime=ControlApptPanel.RoundTimeDown(_dateTimeClickedBlockout.TimeOfDay,contrApptPanel.MinPerIncr);
			schedule.StopTime=schedule.StartTime+TimeSpan.FromHours(1);
			if(schedule.StartTime>TimeSpan.FromHours(23)) {//if user clicked anywhere during the last hour of the day, set blockout to the last hour of the day.
				schedule.StartTime=new TimeSpan(23,00,00);
				schedule.StopTime=new TimeSpan(23,59,00);
			}
			schedule.Ops.Add(_opNumClickedBlockout);//jordan 2019-05-20-This is new behavior to prefill op
			schedule.SchedType=ScheduleType.Blockout;
			using FormScheduleBlockEdit formScheduleBlockEdit=new FormScheduleBlockEdit(schedule,Clinics.ClinicNum, listDefsBlockoutTypes);
			formScheduleBlockEdit.IsNew=true;
			formScheduleBlockEdit.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockClearClinic_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all blockouts for day for this clinic?")) {
				return;
			}
			Operatory operatory=Operatories.GetOperatory(_opNumClickedBlockout);
			Schedules.ClearBlockoutsForClinic(operatory.ClinicNum,_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date,clinicNum:operatory.ClinicNum);
			RefreshPeriodSchedules();
		}

		private void menuBlockClearDay_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all blockouts for day? (This may include blockouts not shown in the current appointment view)")) {
				return;
			}
			Schedules.ClearBlockoutsForDay(_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date);
			RefreshPeriodSchedules();
		}

		private void menuBlockClearOp_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all blockouts for day in this operatory?")) {
				return;
			}
			Schedules.ClearBlockoutsForOp(_opNumClickedBlockout,_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date,opNum:_opNumClickedBlockout);
			RefreshPeriodSchedules();
		}

		private void menuBlockCopy_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			//not even enabled if not right click on a blockout
			Schedule schedule=GetClickedBlockout();
			if(schedule==null) {
				MsgBox.Show(this,"Blockout not found.");
				return;//should never happen
			}
			_scheduleBlockoutClipboard=schedule.Copy();
		}

		private void menuBlockCut_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			//not even enabled if not right click on a blockout
			Schedule schedule=GetClickedBlockout();
			if(schedule==null) {
				MsgBox.Show(this,"Blockout not found.");
				return;//should never happen
			}
			_scheduleBlockoutClipboard=schedule.Copy();
			Schedules.Delete(schedule,true);
			Schedules.BlockoutLogHelper(BlockoutAction.Cut,schedule);
			RefreshPeriodSchedules();
		}

		private void menuBlockCutCopyPaste_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			using FormBlockoutCutCopyPaste formBlockoutCutCopyPaste=new FormBlockoutCutCopyPaste();
			formBlockoutCutCopyPaste.DateSelected=_dateTimeClickedBlockout.Date;
			ApptView apptView=GetApptViewCur();
			if(ApptViews.IsNoneView(apptView)) {//None view.
				formBlockoutCutCopyPaste.ApptViewNum=ApptViews.APPTVIEWNUM_NONE;
			}
			else {
				formBlockoutCutCopyPaste.ApptViewNum=apptView.ApptViewNum;
			}
			formBlockoutCutCopyPaste.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockDelete_Click(object sender,EventArgs e) {
			//If the user doesn't have permission to delete, the menu option will not be enabled.
			Schedule schedule=GetClickedBlockout();
			if(schedule==null) {
				MsgBox.Show(this,"Blockout not found.");
				return;//should never happen
			}
			Schedules.Delete(schedule,true);
			Schedules.BlockoutLogHelper(BlockoutAction.Delete,schedule);
			RefreshPeriodSchedules();
		}

		private void menuBlockEdit_Click(object sender,EventArgs e) {
			//Pre-calculate the list of blockouts to show.  If the user doesn't have the permission then a modified list is shown.
			List<Def> listDefsUserBlockout=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true);	
			if(!Security.IsAuthorized(EnumPermType.Blockouts,true)) {
				//The modified list will only show blockouts marked as "DontCopy" or "NoSchedule"
				listDefsUserBlockout.RemoveAll(x => !x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()) 
					&& !x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			}
			//not even enabled if not right click on a blockout
			Schedule scheduleClicked=GetClickedBlockout();
			if(scheduleClicked==null) {
				MsgBox.Show(this,"Blockout not found.");
				return;//should never happen
			}
			using FormScheduleBlockEdit formScheduleBlockEdit=new FormScheduleBlockEdit(scheduleClicked,Clinics.ClinicNum,listDefsUserBlockout);
			formScheduleBlockEdit.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockPaste_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Blockouts)) {
				return;
			}
			Schedule schedule=_scheduleBlockoutClipboard.Copy();
			schedule.Ops=new List<long>();
			schedule.Ops.Add(_opNumClickedBlockout);
			schedule.SchedDate=_dateTimeClickedBlockout.Date;
			TimeSpan timeSpanOriginalLength=schedule.StopTime-schedule.StartTime;
			schedule.StartTime=ControlApptPanel.RoundTimeDown(_dateTimeClickedBlockout.TimeOfDay,contrApptPanel.MinPerIncr);
			schedule.StopTime=schedule.StartTime+timeSpanOriginalLength;
			if(schedule.StopTime >= TimeSpan.FromDays(1)) {//long span that spills over to next day
				MsgBox.Show(this,"This Blockout would go past midnight.");
				return;
			}
			schedule.ScheduleNum=0;//Because Schedules.Overlaps() ignores matching ScheduleNums and we used the Copy() function above. Also, we insert below, so a new key will be created anyway.
			List<Schedule> listSchedulesOverlap;
			if(Schedules.Overlaps(schedule,out listSchedulesOverlap)) {
				if(!PrefC.GetBool(PrefName.ReplaceExistingBlockout) || !Schedules.IsAppointmentBlocking(schedule.BlockoutType)) {
					MsgBox.Show(this,"Blockouts not allowed to overlap.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Creating this blockout will cause blockouts to overlap. Continuing will delete the existing blockout(s). Continue?")) {
					return;
				}
				Schedules.DeleteMany(listSchedulesOverlap.Select(x => x.ScheduleNum).ToList());
			}
			Schedules.Insert(schedule,true);
			Schedules.BlockoutLogHelper(BlockoutAction.Paste,schedule);
			RefreshPeriodSchedules();
		}

		private void menuBlockTypes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(DefCat.BlockoutTypes);
			formDefinitions.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.DefEdit,0,"Definitions.");
			RefreshPeriodSchedules();
		}
		
		private Schedule GetClickedBlockout() {
			return GetClickedSchedule(ScheduleType.Blockout);
		}

		private Schedule GetClickedSchedule(ScheduleType schedType) {
			List<Schedule> listSchedulesForType=Schedules.GetListForType(contrApptPanel.ListSchedules,schedType,0);
			//now find which blockout
			Schedule schedule=null;
			for(int i=0;i<listSchedulesForType.Count;i++) {
				//skip if op doesn't match
				if(!listSchedulesForType[i].Ops.Contains(_opNumClickedBlockout)) {
					continue;
				}
				if(listSchedulesForType[i].SchedDate.Date!=_dateTimeClickedBlockout.Date) {
					continue;
				}
				if(listSchedulesForType[i].StartTime <= _dateTimeClickedBlockout.TimeOfDay
					&& _dateTimeClickedBlockout.TimeOfDay < listSchedulesForType[i].StopTime) 
				{
					schedule=listSchedulesForType[i];
					break;
				}
			}
			return schedule;//might be null;
		}
		#endregion Methods - Event Handlers Menu Blockout Click

		#region Methods - Event Handlers Menu TextAppts Click
		private void menuTextApptsForDay_Click(object sender,EventArgs e) {
			List<Appointment> listAppointments=Appointments.GetForPeriodList(contrApptPanel.DateSelected,contrApptPanel.DateSelected,Clinics.ClinicNum)
				.Where(x => !x.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)).ToList();
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum==0) {
				//The above query would have gotten appointments for all clinics. We need to filter to just ClinicNums of 0.
				listAppointments=listAppointments.Where(x => x.ClinicNum==0).ToList();
			}
			SendTextMessages(listAppointments.Select(x => x.PatNum).ToList());
		}

		private void menuTextApptsForDayOp_Click(object sender,EventArgs e) {
			DateTime dateClicked=contrApptPanel.DateSelected;
			long opNumClicked=contrApptPanel.OpNumClicked;
			List<long> listPatNums=new List<long>();
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				if(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString())!=opNumClicked){
					continue;
				}
				if(PIn.DateT(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString()).Date!=dateClicked){
					continue;
				}
				listPatNums.Add(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["PatNum"].ToString()));
			}
			SendTextMessages(listPatNums);
		}

		private void menuTextApptsForDayView_Click(object sender,EventArgs e) {
			DateTime dateClicked=contrApptPanel.DateSelected;
			List<long> listPatNums=new List<long>();
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				long opNum=PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString());
				if(!contrApptPanel.ListOpsVisible.Any(y => y.OperatoryNum==opNum)){//Make sure the appointments are visible in the current view.
					continue;
				}
				if(PIn.DateT(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString()).Date!=dateClicked){
					continue;
				}
				listPatNums.Add(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["PatNum"].ToString()));
			}
			SendTextMessages(listPatNums);
		}

		private void menuTextASAPList_Click(object sender,EventArgs e) {
			DateTime dateTimeClicked=contrApptPanel.DateTimeClicked;
			if(PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				DisplayFormAsapForWebSched();
				return;
			}
			//Texting the ASAP list manually
			if(_formASAP!=null && !_formASAP.IsDisposed) {
				_formASAP.Close();
			}
			_formASAP=new FormASAP();
			_formASAP.DateTimeChosen=dateTimeClicked;
			_formASAP.Show();
			if(_formASAP.WindowState==FormWindowState.Minimized) {
				_formASAP.WindowState=FormWindowState.Normal;
			}
			_formASAP.BringToFront();
		}

		private void FormASAP_FormClosed(object sender,FormClosedEventArgs e) {
			RefreshModuleDataPeriod();
			RefreshModuleScreenPeriod();
		}
		#endregion Methods - Event Handlers Menu TextAppts Click

		#region Methods - Event Handlers Menu Jobs
		private void menuJobs_Attach(object sender,System.EventArgs e) {
			if(contrApptPanel.SelectedAptNum<=0) {
				return;
			}
			//Atach new job
			using FormJobSearch formJobSearch = new FormJobSearch();
			formJobSearch.DoBlockProjects=true;
			formJobSearch.ShowDialog();
			if(formJobSearch.DialogResult!=DialogResult.OK || formJobSearch.SelectedJobNum==0) {
				return;
			}
			JobLink jobLink = new JobLink();
			jobLink.JobNum=formJobSearch.SelectedJobNum;
			jobLink.FKey=contrApptPanel.SelectedAptNum;
			jobLink.LinkType=JobLinkType.Appointment;
			JobLinks.Insert(jobLink);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobLink.JobNum);
			return;
		}

		private void menuJobs_GoToJob(object sender,System.EventArgs e) {
				FormOpenDental.S_GoToJob(((long)((ToolStripMenuItem)sender).Tag));
		}
		#endregion Methods - Event Handlers Menu Jobs

		#region Methods - Event Handlers Search
		private void ButAdvSearch_Click(object sender, EventArgs e){
			if(pinBoard.SelectedIndex==-1) {
				if(pinBoard.ListPinBoardItems.Count==0){
					MsgBox.Show(this,"There is no appointment on the pinboard.");
					return;
				}
				pinBoard.SelectedIndex=0;
			}
			long aptNum=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].AptNum;
			using FormApptSearchAdvanced formApptSearchAdvanced=new FormApptSearchAdvanced(aptNum);
			List<long> listProvNumsInBox=new List<long>();
			List<Provider> listProviders=_listBoxProviders.Items.GetAll<Provider>();
			for(int i=0;i<listProviders.Count;i++) {
				listProvNumsInBox.Add(listProviders[i].ProvNum);
			}
			formApptSearchAdvanced.SetSearchArgs(aptNum,listProvNumsInBox,textBefore.Text,textAfter.Text,PIn.Date(dateSearch.Text));
			formApptSearchAdvanced.ShowDialog();
		}

		private void ButLab_Click(object sender, EventArgs e){
			using FormLabCases formLabCases=new FormLabCases();
			formLabCases.ShowDialog();
			if(formLabCases.GoToAptNum!=0) {
				Appointment appointment=Appointments.GetOneApt(formLabCases.GoToAptNum);
				Patient patient=Patients.GetPat(appointment.PatNum);
				//PatientSelectedEventArgs eArgs=new OpenDental.PatientSelectedEventArgs(pat.PatNum,pat.GetNameLF(),pat.Email!="",pat.ChartNumber);
				//if(PatientSelected!=null){
				//	PatientSelected(this,eArgs);
				//}
				//Contr_PatientSelected(this,eArgs);
				GlobalFormOpenDental.PatientSelected(patient,false,false);
				GlobalFormOpenDental.GotoAppointment(appointment.AptDateTime,appointment.AptNum);
			}
		}

		private void butProvDentist_Click(object sender,EventArgs e) {
			_listProvidersSearch=new List<Provider>();
			_listBoxProviders.Items.Clear();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			for(int i=0;i<listProvidersShort.Count;i++){
				if(contrApptPanel.ListApptViewItems.Exists(x=>x.ProvNum==listProvidersShort[i].ProvNum)) {
					if(!listProvidersShort[i].IsSecondary) {
						_listProvidersSearch.Add(listProvidersShort[i]);
						_listBoxProviders.Items.Add(listProvidersShort[i].Abbr,listProvidersShort[i]);
					}
				}
			}
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show(this,"There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butProvHygenist_Click(object sender,EventArgs e) {
			_listProvidersSearch=new List<Provider>();
			_listBoxProviders.Items.Clear();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			for(int i=0;i<listProvidersShort.Count;i++) {
				if(contrApptPanel.ListApptViewItems.Exists(x=>x.ProvNum==listProvidersShort[i].ProvNum)) {
					if(listProvidersShort[i].IsSecondary) {
						_listProvidersSearch.Add(listProvidersShort[i]);
						_listBoxProviders.Items.Add(listProvidersShort[i].Abbr,listProvidersShort[i]);
					}
				}
			}
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show(this,"There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butProvPick_Click(object sender,EventArgs e) {
			using FormProvidersMultiPick formProvidersMultiPick=new FormProvidersMultiPick();
			formProvidersMultiPick.ListProvidersSelected=_listProvidersSearch;
			formProvidersMultiPick.ShowDialog();
			if(formProvidersMultiPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_listBoxProviders.Items.Clear();
			for(int i=0;i<formProvidersMultiPick.ListProvidersSelected.Count;i++) {
				_listBoxProviders.Items.Add(formProvidersMultiPick.ListProvidersSelected[i].Abbr,formProvidersMultiPick.ListProvidersSelected[i]);
			}
			_listProvidersSearch=formProvidersMultiPick.ListProvidersSelected;
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show(this,"There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(pinBoard.SelectedIndex==-1) {
				if(pinBoard.ListPinBoardItems.Count>0) {//if there are any appointments on the pinboard.
					pinBoard.SelectedIndex=pinBoard.ListPinBoardItems.Count-1;//select last appt
				}
				else {
					MsgBox.Show(this,"There are no appointments on the pinboard.");
					return;
				}
			}
			DoSearch();
		}

		private void butSearch_Click(object sender,System.EventArgs e) {
			if(pinBoard.ListPinBoardItems.Count==0) {
				MsgBox.Show(this,"An appointment must be placed on the pinboard before a search can be done.");
				return;
			}
			if(pinBoard.SelectedIndex==-1) {
				if(pinBoard.ListPinBoardItems.Count==1) {
					pinBoard.SelectedIndex=0;
				}
				else {
					MsgBox.Show(this,"An appointment on the pinboard must be selected before a search can be done.");
					return;
				}
			}
			if(!groupSearch.Visible) {//if search not already visible
				dateSearch.Text=DateTime.Today.ToShortDateString();
				ShowSearch();
			}
			DoSearch();
		}

		private void butSearchClose_Click(object sender,System.EventArgs e) {
			groupSearch.Visible=false;
		}

		private void butSearchCloseX_Click(object sender,System.EventArgs e) {
			groupSearch.Visible=false;
		}

		private void butSearchMore_Click(object sender,System.EventArgs e) {
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show(this,"There is no appointment on the pinboard.");
				return;
			}
			if(_listScheduleOpenings==null || _listScheduleOpenings.Count<1) {
				return;
			}
			dateSearch.Text=_listScheduleOpenings[_listScheduleOpenings.Count-1].DateTimeAvail.ToShortDateString();
			DoSearch();
		}

		private void listSearchResults_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			int clickedI=listSearchResults.IndexFromPoint(e.X,e.Y);
			if(clickedI==-1) {
				return;
			}
			ModuleSelected(_listScheduleOpenings[clickedI].DateTimeAvail);
		}
		#endregion Methods - Event Handlers Search

		#region Methods - Public Initialize
		///<summary>Called from FormOpenDental each time the module is selected, but it doesn't do anything after the first time.</summary>
		public void InitializeOnStartup(){
			if(_hasInitializedOnStartup) {
				return;
			}
			contrApptPanel.DateSelected=DateTime.Today;
			FillViews();
			menuApt.Items.Clear();
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Copy to Pinboard"),null,menuApt_Click,MenuItemNames.CopyToPinboard));
			if(PrefC.IsODHQ) {
				menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Copy Appointment Structure"),null,menuApt_Click,MenuItemNames.CopyAppointmentStructure));
			}
			menuApt.Items.Add(new ToolStripSeparator());
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Send to Unscheduled List"),null,menuApt_Click,MenuItemNames.SendToUnscheduledList));
			toolStripMenuItem=new ToolStripMenuItem(Lan.g(this,"Break Appointment"),null,menuApt_Click,MenuItemNames.BreakAppointment);
			menuApt.Items.Add(toolStripMenuItem);
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Mark as ASAP"),null,menuApt_Click,MenuItemNames.MarkAsAsap));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Set Complete"),null,menuApt_Click,MenuItemNames.SetComplete));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Delete"),null,menuApt_Click,MenuItemNames.Delete));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Patient Appointments"),null,menuApt_Click,MenuItemNames.PatientAppointments));
			menuApt.Items.Add(new ToolStripSeparator());
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Print Label"),null,menuApt_Click,MenuItemNames.PrintLabel));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Print Card"),null,menuApt_Click,MenuItemNames.PrintCard));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Print Card for Entire Family"),null,menuApt_Click,MenuItemNames.PrintCardEntireFamily));
			menuApt.Items.Add(new ToolStripMenuItem(Lan.g(this,"Routing Slip"),null,menuApt_Click,MenuItemNames.RoutingSlip));
			//menuBlockout
			menuBlockout.Items.Clear();
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Edit Blockout"),null,menuBlockEdit_Click,MenuItemNames.EditBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Cut Blockout"),null,menuBlockCut_Click,MenuItemNames.CutBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Copy Blockout"),null,menuBlockCopy_Click,MenuItemNames.CopyBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Paste Blockout"),null,menuBlockPaste_Click,MenuItemNames.PasteBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Delete Blockout"),null,menuBlockDelete_Click,MenuItemNames.DeleteBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.DeleteWebSchedAsapBlockout),null,DeleteWebSchedAsapBlockout_Click,MenuItemNames.DeleteWebSchedAsapBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Add Blockout"),null,menuBlockAdd_Click,MenuItemNames.AddBlockout));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Blockout Cut-Copy-Paste"),null,menuBlockCutCopyPaste_Click,MenuItemNames.BlockoutCutCopyPaste));
			if(!PrefC.HasClinicsEnabled) {//Clear All Blockouts for Day is too aggressive when Clinics are enabled.
				menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Clear All Blockouts for Day"),null,menuBlockClearDay_Click,MenuItemNames.ClearAllBlockoutsForDay));
			}
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Clear All Blockouts for Day, Op only"),null,menuBlockClearOp_Click,MenuItemNames.ClearAllBlockoutsForDayOpOnly));
			if(PrefC.HasClinicsEnabled) {
				menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Clear All Blockouts for Day, Clinic only"),null,menuBlockClearClinic_Click,MenuItemNames.ClearAllBlockoutsForDayClinicOnly));
			}
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Edit Blockout Types"),null,menuBlockTypes_Click,MenuItemNames.EditBlockoutTypes));
			menuBlockout.Items.Add(new ToolStripSeparator() { Name=MenuItemNames.BlockoutSpacer });
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,"Text ASAP List"),null,menuTextASAPList_Click,MenuItemNames.TextAsapList));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.TextApptsForDayOp),null,menuTextApptsForDayOp_Click,MenuItemNames.TextApptsForDayOp));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.TextApptsForDayView),null,menuTextApptsForDayView_Click,MenuItemNames.TextApptsForDayView));
			menuBlockout.Items.Add(new ToolStripMenuItem(Lan.g(this,MenuItemNames.TextApptsForDay),null,menuTextApptsForDay_Click,MenuItemNames.TextApptsForDay));
			//Recall Family
			menuRecall.MenuItems.Clear();
			menuRecall.MenuItems.Add(Lan.g(this,"Make Family Recall"),butFamRecall_Click);
			Lan.C(this,
				butBackMonth,
				butBackWeek,
				butToday,
				butFwdWeek,
				butFwdMonth,
				butAdvSearch,
				butSearch,
				butClearPin,
				label2,//label2 is the 'View' label
				butLab,
				labelProduction,
				labelProdGoal,
				label1,//label1 is the 'Confirmation Status' label
				tabControl,
				gridEmpSched,
				gridProv,
				gridReminders,
				gridWaiting,
				groupSearch,//groupSearch is the 'Openings in View' grouping.
				groupBox1,//groupBox1 is the 'Search by' grouping
				butProvHygenist,
				butProvDentist,
				butProvPick,
				butRefresh,
				butSearchClose,
				groupBox2,//groupBox2 is the 'Date/Time Restrictions' grouping.
				label11,//label11 is the 'After' text.
				radioBeforeAM,
				radioBeforePM,
				label10,//label10 is the 'Before' text.
				label9,//label9 is also 'After' text.
				radioAfterAM,
				radioAfterPM,
				label8,//label8 is the 'Providers' text.
				butSearchNext);
			LayoutToolBar();
			SetWeeklyView(PrefC.GetBool(PrefName.ApptModuleDefaultToWeek));
			ODEvent.Fired+=HandlePinClicked;
			_hasInitializedOnStartup=true;
		}

		///<summary></summary>
		public void LayoutToolBar(){
			toolBarMain.Buttons.Clear();
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Lists"),1,Lan.g(this,"Appointment Lists"),"Lists"));
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Pat Appts"),EnumIcons.Patient,Lan.g(this,"Patient Appointments"),"PatAppts"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Make Appt"),EnumIcons.Add,Lan.g(this,"Make Appointment"),"Make"));
			ODToolBarButton toolBarButton=new ODToolBarButton(Lan.g(this,"Make Recall"),EnumIcons.Recall,Lan.g(this,"Make Recall"),"Recall");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButton.DropDownMenu=menuRecall;
			toolBarMain.Buttons.Add(toolBarButton);
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Unsched"),3,Lan.g(this,"Send to Unscheduled List"),"Unsched"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Break"),EnumIcons.BreakAptX,Lan.g(this,"Break Appointment"),"Break"));
			//asap?
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Complete"),EnumIcons.Complete,"","Complete"));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Delete"),EnumIcons.DeleteX,"","Delete"));
			if(!ProgramProperties.IsAdvertisingDisabled(ProgramName.RapidCall)) {
				toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Rapid Call"),2,"","RapidCall"));
			}
			ProgramL.LoadToolBar(toolBarMain,EnumToolBar.ApptModule);
			toolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrAppt.LayoutToolBar_end",_patient);
		}
		#endregion Methods - Public Initialize

		#region Methods - Public Module Select
		///<summary>This is a good way to set contrApptPanel.DateSelected while also refreshing the module.  If you are not changing the date or patient, then instead, simply use RefreshPeriod().</summary>
		public void ModuleSelected(DateTime date){
			contrApptPanel.BeginUpdate();//otherwise, the appointments will disappear
			contrApptPanel.DateSelected=date;
			long apptViewNum=0;
			if(contrApptPanel.ApptViewCur!=null) {
				apptViewNum=contrApptPanel.ApptViewCur.ApptViewNum;
			}
			if(contrApptPanel.IsWeeklyView) {
				if(_patient==null) {
					ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
				else {
					ModuleSelected(_patient.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
			}
			else {
				RefreshPeriod(listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum),isRefreshSchedules:true);
			}
			contrApptPanel.EndUpdate();
		}

		///<summary>Refreshes the module for the passed in patient.  A patNum of 0 is acceptable.  Any ApptNums within listPinApptNums will get forcefully added to the main DataSet for the appointment module.</summary>
		public void ModuleSelected(long patNum,List<long> listPinApptNums=null,List<long> listOpNums=null,List<long> listProvNums=null){
			LayoutControls();
			if(IsHqNoneView()) {
				return;
			}
			contrApptPanel.BeginUpdate();
			//Setting these properties is low overhead, and is necessary if user changed them.
			contrApptPanel.MinPerIncr=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AppointmentColors,true);
			Color colorOpen=listDefs[0].ItemColor;
			Color colorClosed=listDefs[1].ItemColor;
			Color colorHoliday=listDefs[3].ItemColor;
			Color colorBlockText=listDefs[4].ItemColor;
			Color colorTimeLine=PrefC.GetColor(PrefName.AppointmentTimeLineColor);
			contrApptPanel.SetColors(colorOpen,colorClosed,colorHoliday,colorBlockText,colorTimeLine);
			contrApptPanel.SizeFont=float.Parse(PrefC.GetString(PrefName.ApptFontSize));
			contrApptPanel.WidthProvOnAppt=float.Parse(PrefC.GetString(PrefName.ApptProvbarWidth));
			SetWeeklyView(contrApptPanel.IsWeeklyView,skipModuleSelection:true);//in case they changed the pref for start day of week
			RefreshModuleDataPatient(patNum);
			if(_patient!=null && _patient.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patient.PatNum);
				GlobalFormOpenDental.PatientSelected(new Patient(),false);
				RefreshModuleDataPatient(0);
			}
			if(_patient!=null && _patient.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(EnumPermType.ArchivedPatientSelect,suppressMessage:true)) {
				GlobalFormOpenDental.PatientSelected(new Patient(),false);
				RefreshModuleDataPatient(0);
			}
			RefreshModuleDataPeriod(listPinApptNums,listOpNums,listProvNums,forceRefreshSchedules:true);
			RefreshModuleScreenButtonsRight();
			RefreshModuleScreenPeriod();
			SetInitialStartTime();//only runs once
			contrApptPanel.EndUpdate();
			//There is no "LoadData" in this Module, so at least pass _patCur.
			ODEvent.Fire(ODEventType.ModuleSelected,_patient);
			Plugins.HookAddCode(this,"ContrAppt.ModuleSelected_end",patNum);
		}

		///<summary>Jumping here from another module and selecting an appointment.  Patient is already taken care of, frequently because the appointment is for the current patient.</summary>
		public void ModuleSelectedGoToAppt(long aptNum,DateTime dateSelected){
			ModuleSelected(dateSelected);
			contrApptPanel.SelectedAptNum=aptNum;
		}

		///<summary>Jumping here from another module and placing appointments on the pinboard.</summary>
		public void ModuleSelectedWithPinboard(long patNum,List<long> listPinApptNums,DateTime dateSelected,bool showSearch){
			contrApptPanel.BeginUpdate();
			contrApptPanel.DateSelected=dateSelected;
			ModuleSelected(patNum,listPinApptNums);
			SendToPinBoardAptNums(listPinApptNums);
			if(showSearch) {
				dateSearch.Text=dateSelected.ToShortDateString();
				if(!groupSearch.Visible) { //if search not already visible
					ShowSearch();
				}
				DoSearch();
			}
			contrApptPanel.EndUpdate();
		}

		///<summary></summary>
		public void ModuleUnselected(){
			//We could get rid of our main bitmaps to free up a little memory, I suppose			
			Plugins.HookAddCode(this,"ContrAppt.ModuleUnselected_end");
		}

		///<summary>>Refreshes everything except the patient info. isRefreshBubble will refresh the appointment bubble.  If another workstation made a change, then refreshes datatables.</summary>
		public void RefreshPeriod(List<long> listOpNums=null,List<long> listProvNums=null,bool isRefreshAppointments=true,
			bool isRefreshSchedules=false,List<long> listPinApptNums=null)
			{
			if(IsHqNoneView()) {
				return;
			}
			contrApptPanel.BeginUpdate();
			RefreshModuleDataPeriod(listPinApptNums,listOpNums:listOpNums,listProvNums:listProvNums,forceRefreshAppointments:isRefreshAppointments,forceRefreshSchedules:isRefreshSchedules);
			RefreshModuleScreenPeriod();
			contrApptPanel.EndUpdate();
		}

		/// <summary>Wrapper for RefreshPeriod, refreshes the schedules, but not the appointments</summary>
		public void RefreshPeriodSchedules() {
			RefreshPeriod(isRefreshAppointments:false,isRefreshSchedules:true);
		}
		#endregion Methods - Public Module Select

		#region Methods - Public Get Fields From Panel
		public DateTime GetDateSelected(){
			return contrApptPanel.DateSelected;
		}

		public List<Operatory> GetListOpsVisible(){
			return contrApptPanel.ListOpsVisible;
		}

		public List<Provider> GetListProvsVisible(){
			return contrApptPanel.ListProvsVisible;
		}
		#endregion Methods - Public Get Fields From Panel

		#region Methods - Public Other
		///<summary>Displays the Other Appointments for the current patient, then refreshes screen as needed.  initialClick specifies whether the user 
		///doubleclicked on a blank time to get to this dialog.</summary>
		public void DisplayOtherDlg(bool didInitialClick,DateTime dateTime,long opNum) {
			if(_patient==null) {
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());
			formApptsOther.IsInitialDoubleClick=didInitialClick;
			formApptsOther.DateTimeClicked=contrApptPanel.DateTimeClicked;
			formApptsOther.OpNumClicked=contrApptPanel.OpNumClicked;
			formApptsOther.DateTNew=dateTime;
			formApptsOther.OpNumNew=opNum;
			formApptsOther.ShowDialog();
			ProcessOtherDlg(formApptsOther.GetOtherResult(),formApptsOther.PatNumSelected,formApptsOther.StringDateJumpTo,formApptsOther.ListAptNumsSelected.ToArray());
		}

		///<summary>Displays the Other Appointments for the current patient, then refreshes screen as needed.  initialClick specifies whether the user doubleclicked on a blank time to get to this dialog.</summary>
		public void DisplayOtherDlg(bool didInitialClick) {
			if(_patient==null) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			using FormApptsOther formApptsOther=new FormApptsOther(_patient.PatNum,pinBoard.ListPinBoardItems.Select(x => x.AptNum).ToList());
			formApptsOther.IsInitialDoubleClick=didInitialClick;
			formApptsOther.DateTimeClicked=contrApptPanel.DateTimeClicked;
			formApptsOther.OpNumClicked=contrApptPanel.OpNumClicked;
			formApptsOther.ShowDialog();
			ProcessOtherDlg(formApptsOther.GetOtherResult(),formApptsOther.PatNumSelected,formApptsOther.StringDateJumpTo,formApptsOther.ListAptNumsSelected.ToArray(),formApptsOther.ListAptViewJumpTos);
		}

		///<summary>The key press from the main form is passed down to this module.  This is guaranteed to be between the keys of F1 and F12.</summary>
		public void FunctionKeyPress(Keys keys) {
			string keyName=Enum.GetName(typeof(Keys),keys);//keyName will be F1, F2, ... F12
			int fKeyVal=int.Parse(keyName.TrimStart('F'));//strip off the F and convert to an int
			if(comboView.Items.GetAll<ApptView>().All(x => x.ApptViewNum!=ApptViews.APPTVIEWNUM_NONE)) {
				fKeyVal--;//None view was excluded during "FillViews".  Map index 0 to F1, 1 to F2, etc.
			}
			if(comboView.Items.Count-1 < fKeyVal) {//Check for valid F key.
				return;
			}
			SetView(((ApptView)comboView.Items.GetObjectAt(fKeyVal)).ApptViewNum,true);
		}

		///<summary>Used by parent form when a dialog needs to be displayed, but mouse might be down.  This forces a mouse up, and cleans up any mess so that dlg can show.</summary>
		public void MouseUpForced() {
			pinBoard.MouseUpForced();
			contrApptPanel.MouseUpForced();
		}

		///<summary>This is public so that FormOpenDental can pass refreshed tasks here in order to avoid an extra query.</summary>
		public void RefreshReminders(List <Task> listTasksReminder) {
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			List<Task> listSortedReminderTasks=listTasksReminder
				.Where(x => x.DateTimeEntry.Date <= DateTime.Today)
				.OrderBy(x => x.DateTimeEntry)
				.ToList();
			tabReminders.Text=Lan.g(this,"Reminders");
			if(listSortedReminderTasks.Count > 0) {
				tabReminders.Text+="*";
			}
			gridReminders.BeginUpdate();
			if(gridReminders.Columns.Count==0) {
				gridReminders.Columns.Clear();
				GridColumn col=new GridColumn("",17);//The status column showing new/viewed in a checkbox.
				col.ImageList=imageListTasks;
				gridReminders.Columns.Add(col);
				col=new GridColumn(Lan.g("TableTasks","Description"),200);//any width
				gridReminders.Columns.Add(col);
			}
			gridReminders.ListGridRows.Clear();
			for(int i=0;i<listSortedReminderTasks.Count;i++) {
				GridRow row=new GridRow();
				SetReminderGridRow(row,listSortedReminderTasks[i]);
				gridReminders.ListGridRows.Add(row);
			}
			gridReminders.EndUpdate();
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
		}

		///<summary>This logic mimics filling a row within UserControlTasks.FillGrid().
		///However, the logic is simpler here because we are only dealing with reminders.</summary>
		private void SetReminderGridRow(GridRow row,Task taskReminder) {
			row.Tag=taskReminder;
			row.Cells.Clear();
			string dateStr="";
			if(taskReminder.DateTask.Year>1880) {
				if(taskReminder.DateType==TaskDateType.Day) {
					dateStr+=taskReminder.DateTask.ToShortDateString()+" - ";
				}
				else if(taskReminder.DateType==TaskDateType.Week) {
					dateStr+=Lan.g(this,"Week of")+" "+taskReminder.DateTask.ToShortDateString()+" - ";
				}
				else if(taskReminder.DateType==TaskDateType.Month) {
					dateStr+=taskReminder.DateTask.ToString("MMMM")+" - ";
				}
			}
			else if(taskReminder.DateTimeEntry.Year>1880) {
				dateStr+=taskReminder.DateTimeEntry.ToShortDateString()+" "+taskReminder.DateTimeEntry.ToShortTimeString()+" - ";
			}
			string objDesc="";
			if(taskReminder.TaskStatus==TaskStatusEnum.Done){
				objDesc=Lan.g(this,"Done:")+taskReminder.DateTimeFinished.ToShortDateString()+" - ";
			}
			if(taskReminder.ObjectType==TaskObjectType.Patient) {
				if(taskReminder.KeyNum!=0) {
					objDesc+=Patients.GetPat(taskReminder.KeyNum).GetNameLF()+" - ";
				}
			}
			else if(taskReminder.ObjectType==TaskObjectType.Appointment) {
				if(taskReminder.KeyNum!=0) {
					Appointment appointment=Appointments.GetOneApt(taskReminder.KeyNum);
					if(appointment!=null) {
						objDesc=Patients.GetPat(appointment.PatNum).GetNameLF()//this is going to stay. Still not optimized, but here at HQ, we don't use it.
							+"  "+appointment.AptDateTime.ToString()
							+"  "+appointment.ProcDescript
							+"  "+appointment.Note
							+" - ";
					}
				}
			}
			if(!taskReminder.Descript.StartsWith("==") && taskReminder.UserNum!=0) {
				objDesc+=Userods.GetName(taskReminder.UserNum)+" - ";
			}
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//The new way
				if(taskReminder.TaskStatus==TaskStatusEnum.Done) {
					row.Cells.Add("1");
				}
				else {
					if(taskReminder.IsUnread) {
						row.Cells.Add("4");
					}
					else{
						row.Cells.Add("2");
					}
				}
			}
			else {
				switch(taskReminder.TaskStatus) {
					case TaskStatusEnum.New:
						row.Cells.Add("4");
						break;
					case TaskStatusEnum.Viewed:
						row.Cells.Add("2");
						break;
					case TaskStatusEnum.Done:
						row.Cells.Add("1");
						break;
				}
			}
			row.Cells.Add(dateStr+objDesc+taskReminder.Descript);
			//No need to do any text detection for triage priorities, we'll just use the task priority colors.
			row.ColorBackG=Defs.GetColor(DefCat.TaskPriorities,taskReminder.PriorityDefNum);
		}
		#endregion Methods - Public Other

		#region Methods - Private Refresh Data
		/// <summary>This corresponds to the old ApptViewItemL.GetForCurView.  Its job is to set the ApptViewCur and then send VisOps and VisProvs data to the drawing.</summary>
		private void GetForCurView(ApptView apptView,bool isWeekly,List<Schedule> listSchedulesDaily){
			//contrApptPanel.BeginUpdate();//already handled in RefreshModuleDataPeriod
			contrApptPanel.ApptViewCur=apptView;
			List<Provider> listProvidersVis=null;
			List<Operatory> listOperatoriesVis=null;
			int rowsPerIncr=0;
			List<ApptViewItem> listApptViewItemRowElements=null;
			List<ApptViewItem> listApptViewItems=null;
			ApptViewItemL.FillForApptView(isWeekly,apptView,out listProvidersVis,out listOperatoriesVis,out listApptViewItems,out listApptViewItemRowElements,out rowsPerIncr);
			ApptViewItemL.AddOpsForScheduledProvs(isWeekly,listSchedulesDaily,apptView,ref listOperatoriesVis);
			listOperatoriesVis.Sort(ApptViewItemL.CompareOps);
			listProvidersVis.Sort(ApptViewItemL.CompareProvs);
			contrApptPanel.ListProvsVisible=listProvidersVis;
			contrApptPanel.ListOpsVisible=listOperatoriesVis;
			contrApptPanel.RowsPerIncr=rowsPerIncr;
			contrApptPanel.ListApptViewItemRowElements=listApptViewItemRowElements;
			contrApptPanel.ListApptViewItems=listApptViewItems;
			//contrApptPanel.EndUpdate();
		}

		///<summary>If needed, refreshes TableAppointments, TableApptFields, and TablePatFields tables.</summary>
		private void RefreshAppointmentsIfNeeded(DateTime dateStart,DateTime dateEnd,List<long> listPinApptNums=null,
			List<long> listOpNums=null,List<long> listProvNums=null,bool forceRefresh=false)
		{
			if(contrApptPanel.TableAppointments!=null && contrApptPanel.TableApptFields!=null && contrApptPanel.TablePatFields!=null && !forceRefresh) {
				return;//If all data is already in memory and we are not forcing the refresh.
			}
			bool includeVerifyIns=false;
			if(contrApptPanel.ListApptViewItems!=null
				&& contrApptPanel.ListApptViewItems.Exists(x=>x.ElementDesc==EnumApptViewElement.VerifyIns_V.GetDescription()))
			{
				includeVerifyIns=true;
			}
			DataTable table=Appointments.GetPeriodApptsTable(dateStart, dateEnd, aptNum:0, isPlanned:false, listPinApptNums, listOpNums, listProvNums, allowRunQueryOnNoOps:false, includeVerifyIns:includeVerifyIns);
			if(table.Rows.Count > 0) {
				//This is an arbitrary but fixed order so that appointments always get drawn in the same order and don't appear to jump
				table=table.Select().OrderBy(x => PIn.Long(x["AptNum"].ToString())).CopyToDataTable();
			}
			contrApptPanel.TableAppointments=table;
			contrApptPanel.TableApptFields=Appointments.GetApptFields(contrApptPanel.TableAppointments);
			contrApptPanel.TablePatFields=Appointments.GetPatFields(contrApptPanel.TableAppointments.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList());
			OpenDentBusiness.AutoComm.Arrivals arrivals=OpenDentBusiness.AutoComm.Arrivals.LoadArrivals(
				contrApptPanel.TableAppointments.Select().Select(x => PIn.Long(x["ClinicNum"].ToString())).Distinct().ToList(),
				contrApptPanel.TableAppointments.Select().Select(x => PIn.Long(x["AptNum"].ToString())).Distinct().ToList()
			);
			SetArrivalsLoaded(arrivals);
		}

		///<summary>Fills PatCur from the database.</summary>
		public void RefreshModuleDataPatient(long patNum){
			if(patNum==0) {
				_patient=null;
				return;
			}
			//We have to go to the db because we need to get the most recent patient info, mainly the AskedToArriveEarly time.
			_patient=Patients.GetPat(patNum);
			if(_patient!=null && DatabaseIntegrities.DoShowPopup(_patient.PatNum,EnumModuleType.Appointments)) {
				List<Appointment> listAppointments=Appointments.GetAppointmentsForPat(_patient.PatNum);
				bool areHashesValid=Patients.AreAllHashesValid(_patient,listAppointments,new List<PayPlan>(),new List<PaySplit>());
				if(!areHashesValid) {
					DatabaseIntegrities.AddPatientModuleToCache(_patient.PatNum,EnumModuleType.Appointments); //Add to cached list for next time
					//show popup
					DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetModule();
					FrmDatabaseIntegrity frmDatabaseIntegrity=new FrmDatabaseIntegrity();
					frmDatabaseIntegrity.MessageToShow=databaseIntegrity.Message;
					frmDatabaseIntegrity.ShowDialog();
				}
			}
			Plugins.HookAddCode(this, "ContrAppt.RefreshModuleDataPatient_end");
		}

		///<summary>Gets op nums and prov nums for current view if not passed in.  Will refresh the appointments and schedules if the respective forceRefreshes are set.</summary>
		private void RefreshModuleDataPeriod(List<long> listPinApptNums = null,List<long> listOpNums = null,List<long> listProvNums = null ,bool forceRefreshAppointments=true,bool forceRefreshSchedules=false)
		{
			long apptViewNum=-1;
			if(listOpNums==null) {
				apptViewNum=GetApptViewNumForUser();
				listOpNums=ApptViewItems.GetOpsForView(apptViewNum);
			}
			if(listProvNums==null) {
				if(apptViewNum<0) {
					apptViewNum=GetApptViewNumForUser();//Only run this query if we have to (haven't run it yet from this method).
				}
				listProvNums=ApptViewItems.GetProvsForView(apptViewNum);
			}
			RefreshSchedulesIfNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listOpNums,forceRefreshSchedules);
			//no dependencies:
			RefreshWaitingRoomTable();
			_dateTimeWaitingRmRefreshed=DateTime.Now;
			//SchedListPeriod=Schedules.ConvertTableToList(_dtSchedule);//happens internally in contrApptPanel when setting TableSchedule
			//no dependencies:
			ApptView apptView=GetApptViewCur(apptViewNumOverride:apptViewNum);
			//for this line, I need contrApptPanel.ListSchedules to be already filled, which happens in RefreshSchedulesIfNeeded.
			GetForCurView(apptView,contrApptPanel.IsWeeklyView,contrApptPanel.ListSchedules);
			//for this line, I need contrApptPanel.ListApptViewItems to be already filled, which currently happens in GetForCurView:
			//Also need listOpNums and listProvNums to be prefilled.
			RefreshAppointmentsIfNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listPinApptNums,listOpNums,listProvNums,forceRefresh:forceRefreshAppointments);
		}

		/// <summary>If needed, refreshes TableSchedule, TableEmpSched, and TableProvSched tables.</summary>
		private void RefreshSchedulesIfNeeded(DateTime dateStart,DateTime dateEnd,List<long> listOpNums,bool isRefreshNeeded=false) {
			if(contrApptPanel.TableSchedule!=null && contrApptPanel.TableEmpSched!=null && contrApptPanel.TableProvSched!=null && !isRefreshNeeded) {
				return;//If all data is already in memory and we are not forcing the refresh.
			}
			contrApptPanel.TableEmpSched=Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,Clinics.ClinicNum);
			contrApptPanel.TableProvSched=Schedules.GetPeriodProviderSchedTable(dateStart,dateEnd,Clinics.ClinicNum);
			contrApptPanel.TableSchedule=Schedules.GetPeriodSchedule(dateStart,dateEnd,listOpNums,false);
		}

		///<summary>Always refreshes the _dtWaitingRoom table.</summary>
		private void RefreshWaitingRoomTable() {
			contrApptPanel.TableWaitingRoom=Appointments.GetPeriodWaitingRoomTable(DateTime.Now);
		}
		#endregion Methods - Private Refresh Data

		#region Methods - Private Refresh Screen
		///<summary></summary>
		private void FillEmpSched(bool hasNotes) {
			DataTable table=contrApptPanel.TableEmpSched;
			gridEmpSched.BeginUpdate();
			gridEmpSched.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptEmpSched","Employee"),80);
			gridEmpSched.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptEmpSched","Schedule"),70);
			gridEmpSched.Columns.Add(col);
			if(hasNotes) {
				col=new GridColumn(Lan.g("TableApptEmpSched","Notes"),100);
				gridEmpSched.Columns.Add(col);
			}
			gridEmpSched.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["empName"].ToString());
				row.Cells.Add(table.Rows[i]["schedule"].ToString());
				if(hasNotes) {
					row.Cells.Add(table.Rows[i]["Note"].ToString());
				}
				gridEmpSched.ListGridRows.Add(row);
			}
			gridEmpSched.EndUpdate();
		}

		///<summary>Fills the lab summary for the day.</summary>
		private void FillLab(List<LabCase> labCaseList) {
			int countNotRec=0;
			for(int i=0;i<labCaseList.Count;i++) {
				if(labCaseList[i].DateTimeChecked.Year>1880) {
					continue;
				}
				if(labCaseList[i].DateTimeRecd.Year>1880) {
					continue;
				}
				countNotRec++;
			}
			if(countNotRec==0) {
				textLab.Font=new Font("Microsoft Sans Serif",LayoutManager.Scale(8.25f),FontStyle.Regular);
				textLab.ForeColor=Color.Black;
				textLab.Text=Lan.g(this,"All Received");
				return;
			}
			textLab.Font=new Font("Microsoft Sans Serif",LayoutManager.Scale(8.25f),FontStyle.Bold);
			textLab.ForeColor=Color.DarkRed;
			textLab.Text=countNotRec.ToString()+Lan.g(this," NOT RECEIVED");
		}

		///<summary>Fills the production summary for the day. ContrApptSheet2.Controls should be current with ContrApptSingle(s) for the select Op and date.</summary>
		private void FillProduction(DateTime start,DateTime end) {
			if(!contrApptPanel.ListApptViewItemRowElements.Exists(x => x.ElementDesc.In("Production","NetProduction"))){
				textProduction.Text="";
				return;
			}
			//If the PrefName.ApptModuleProductionUsesOps is true, the list will be filled with OpNums for the appointment view. Otherwise, the list will be empty.
			List<long> listProvNumsForApptView=new List<long>();
			List<long> listOpsForApptView=new List<long>();
			long apptViewNum=GetApptViewNumForUser();
			if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
				listOpsForApptView=ApptViewItems.GetOpsForView(apptViewNum);
			}
			else {
				listProvNumsForApptView=ApptViewItems.GetProvsForView(apptViewNum);
			}
			textProduction.Text=contrApptPanel.GetProduction(
				contrApptPanel.TableAppointments.Rows.OfType<DataRow>().ToList(),listOpsForApptView,listProvNumsForApptView,start,end);
		}

		private void FillProductionGoal(DateTime start,DateTime end) {
			if(!contrApptPanel.ListApptViewItemRowElements.Exists(x => x.ElementDesc.In("Production","NetProduction"))) {
				textProdGoal.Text="";
				return;
			}
			long apptViewNum=GetApptViewNumForUser();
			//If the PrefName.ApptModuleProductionUsesOps is false, it will be filled with ProvNums for the appointment view. Otherwise, the list will be empty.
			List<long> listProvNumsForApptView=new List<long>();
			//If the PrefName.ApptModuleProductionUsesOps is true, the list will be filled with OpNums for the appointment view. Otherwise, the list will be empty.
			List<long> listOpsForApptView=new List<long>();
			if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
				listOpsForApptView=ApptViewItems.GetOpsForView(apptViewNum);
			}
			else {
				listProvNumsForApptView=ApptViewItems.GetProvsForView(apptViewNum);
			}
			//This will return a dict of production goals for either the providers for the provider bars for the appointment view or the providers 
			//scheduled for the appointment view ops. 
			decimal prodGoalAmt=Providers.GetProductionGoalForProviders(listProvNumsForApptView,listOpsForApptView,start,end);
			textProdGoal.Text=prodGoalAmt.ToString("c0");
		}

		///<summary></summary>
		private void FillProvSched(bool hasNotes) {
			DataTable table=contrApptPanel.TableProvSched;
			gridProv.BeginUpdate();
			gridProv.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAppProv","Provider"),80);
			gridProv.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAppProv","Schedule"),70);
			gridProv.Columns.Add(col);
			if(hasNotes) {
				col=new GridColumn(Lan.g("TableAppProv","Notes"),100);
				gridProv.Columns.Add(col);
			}
			gridProv.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) { 
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["ProvAbbr"].ToString());
				row.Cells.Add(table.Rows[i]["schedule"].ToString());
				if(hasNotes) {
					row.Cells.Add(table.Rows[i]["Note"].ToString());
				}
				gridProv.ListGridRows.Add(row);
			}
			gridProv.EndUpdate();
		}

		///<summary>Fills comboView with the current list of views. Triggers ModuleSelected().  Also called from FormOpenDental.RefreshLocalData().</summary>
		public void FillViews() {
			List<ApptView> listApptViews=new List<ApptView>();
			//Do NOT allow 'Headquarters' to have access to clinic specific apptviews.
			//Likewise, we do not want clinic specific views to be accessible from specific clinic filters.
			List<ApptView> listApptViewsClinic=ApptViews.GetWhere(x => !(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=x.ClinicNum));
			int offset=1;
			if(listApptViewsClinic.Count==0 || !PrefC.GetBool(PrefName.EnterpriseNoneApptViewDefaultDisabled)) {
				//First item is "None" view.
				listApptViews.Add(new ApptView() {
					ApptViewNum=ApptViews.APPTVIEWNUM_NONE,
					Description=Lan.g(this,"none")
				});
				offset=0;
			}
			listApptViews.AddRange(listApptViewsClinic);
			comboView.Items.Clear();
			comboView.Items.AddList(listApptViews
				//Views 1 through 12 get Function key shortcuts. None view does not.
				,(x) => (listApptViews.IndexOf(x).Between(1-offset,12-offset) ? $"F{listApptViews.IndexOf(x)+offset}-" : "")+x.Description);
			ApptView apptView=GetApptViewForUser();//Determine which view this user should select on load.
			SetView(apptView.ApptViewNum,false);//this also triggers ModuleSelected()
		}

		///<summary>Once per second, this grid refills itself in order to show the time ticking by.  This does not require a trip to the database.</summary>
		private void FillWaitingRoom() {
			if(!this.Visible){
				return;
			}
			if(contrApptPanel.TableWaitingRoom==null){
				return;
			}
			TimeSpan timeSpanDeltaSinceRefresh=DateTime.Now-_dateTimeWaitingRmRefreshed;
			DataTable table=contrApptPanel.TableWaitingRoom;
			List<Operatory> listOperatoriesForClinic=new List<Operatory>();
			List<Operatory> listOperatoriesForApptView=new List<Operatory>();
			if(PrefC.GetBool(PrefName.WaitingRoomFilterByView)) {
				//In order to filter the waiting room by appointment view, we need to always grab the operatories visible for TODAY.
				//This way, regardless of what day the customer is looking at, the waiting room will only change when they change appointment views.
				//Always use the schedules from SchedListPeriod which is refreshed any time RefreshModuleDataPeriod() is invoked.
				ApptView apptView=GetApptViewCur();
				List<Schedule> listSchedulesForToday=contrApptPanel.ListSchedules.FindAll(x => x.SchedDate==DateTime.Today);
				listOperatoriesForApptView=ApptViewItemL.GetOpsForApptView(apptView,contrApptPanel.IsWeeklyView,listSchedulesForToday);
			}
			if(PrefC.HasClinicsEnabled) {//Using clinics
				listOperatoriesForClinic=Operatories.GetOpsForClinic(Clinics.ClinicNum);
			}
			gridWaiting.BeginUpdate();
			gridWaiting.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptWaiting","Patient"),130);
			gridWaiting.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptWaiting","Waited"),100,HorizontalAlignment.Center);
			gridWaiting.Columns.Add(col);
			gridWaiting.ListGridRows.Clear();
			DateTime timeWait;
			GridRow row;
			int waitingRoomAlertTime=PrefC.GetInt(PrefName.WaitingRoomAlertTime);
			Color waitingRoomAlertColor=PrefC.GetColor(PrefName.WaitingRoomAlertColor);
			for(int i=0;i<table.Rows.Count;i++) {
				//Always filter the waiting room by appointment view first, regardless of using clinics or not.
				if(PrefC.GetBool(PrefName.WaitingRoomFilterByView)) {
					bool isInView=false;
					for(int j=0;j<listOperatoriesForApptView.Count;j++) {
						if(listOperatoriesForApptView[j].OperatoryNum==PIn.Long(table.Rows[i]["OpNum"].ToString())) {
							isInView=true;
							break;
						}
					}
					if(!isInView) {
						continue;
					}
				}
				//We only want to filter the waiting room by the clinic's operatories when clinics are enabled and they are not using 'Headquarters' mode.
				if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=0) {
					bool isInView=false;
					for(int j=0;j<listOperatoriesForClinic.Count;j++) {
						if(listOperatoriesForClinic[j].OperatoryNum==PIn.Long(table.Rows[i]["OpNum"].ToString())) {
							isInView=true;
							break;
						}
					}
					if(!isInView) {
						continue;
					}
				}
				row=new GridRow();
				string patName="";
				ApptView apptView=GetApptViewCur();
				if(apptView!=null) {
					switch(apptView.WaitingRmName) {
						case EnumWaitingRmName.LastFirst: 
							patName=$"{table.Rows[i]["LName"]}, {table.Rows[i]["FName"]}";
						break;
						case EnumWaitingRmName.FirstLastI: 
							patName=$"{table.Rows[i]["FName"]}, {table.Rows[i]["LName"].ToString().Substring(0,1)}";
						break;
						case EnumWaitingRmName.First: 
							patName=$"{table.Rows[i]["FName"]}";
						break;
					}
				} else {
					patName=table.Rows[i]["patName"].ToString();
				}
				row.Cells.Add(patName);
				timeWait=DateTime.Parse(table.Rows[i]["waitTime"].ToString());//we ignore date
				timeWait+=timeSpanDeltaSinceRefresh;
				row.Cells.Add(timeWait.ToString("H:mm:ss"));
				row.Bold=false;
				if(waitingRoomAlertTime>0 && waitingRoomAlertTime<=timeWait.Minute+(timeWait.Hour*60)) {
					row.ColorText=waitingRoomAlertColor;
					row.Bold=true;
				}
				gridWaiting.ListGridRows.Add(row);
			}
			gridWaiting.EndUpdate();
		}

		///<summary>Sets buttons at right to enabled/disabled. Sets value of listConfirmed. Was previously called RefreshModuleScreenPatient.</summary>
		public void RefreshModuleScreenButtonsRight(){
			//considered only doing this once when starting program, but would then need to refresh it if we change definitions.  Might still try that.
			listConfirmed.Items.Clear();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<listDefs.Count;i++) {
				this.listConfirmed.Items.Add(listDefs[i].ItemValue);
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow!=null) {
				toolBarMain.Buttons["Unsched"].Enabled=true;
				toolBarMain.Buttons["Break"].Enabled=true;
				toolBarMain.Buttons["Complete"].Enabled=true;
				toolBarMain.Buttons["Delete"].Enabled=true;
				string confirmed=dataRow["Confirmed"].ToString();
				listConfirmed.SelectedIndex=Defs.GetOrder(DefCat.ApptConfirmed,PIn.Long(confirmed));//could be -1
				if(!Security.IsAuthorized(EnumPermType.ApptConfirmStatusEdit,true)) {//Suppress message because it would be very annoying to users.
					listConfirmed.Enabled=false;
				}
				else {
					listConfirmed.Enabled=true;
				}
			}
			else {//even if an appt on the pinboard is selected, these are all grayed out
				toolBarMain.Buttons["Unsched"].Enabled=false;
				toolBarMain.Buttons["Break"].Enabled=false;
				toolBarMain.Buttons["Complete"].Enabled=false;
				toolBarMain.Buttons["Delete"].Enabled=false;
				listConfirmed.Enabled=false;
				if(pinBoard.SelectedIndex!=-1){
					dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
					listConfirmed.SelectedIndex=Defs.GetOrder(DefCat.ApptConfirmed,PIn.Long(dataRow["Confirmed"].ToString()));//could be -1
				}
			}
			toolBarMain.Invalidate();
		}

		///<summary>Redraws screen based on data already gathered.  RefreshModuleDataPeriod will have already retrieved the data from the db.</summary>
		public void RefreshModuleScreenPeriod() {
			monthCalendarOD.SetDateSelected(contrApptPanel.DateSelected);
			//LayoutPanels();
			labelDate.Text=contrApptPanel.DateStart.ToString("ddd");
			labelDate2.Text=contrApptPanel.DateStart.ToString("-  MMM d");
			RefreshPinboardImages();
			List<long> listOperatoryNums = null;
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum>0) {
				listOperatoryNums = Operatories.GetOpsForClinic(Clinics.ClinicNum).Select(x => x.OperatoryNum).ToList();
			}
			List<LabCase> listLabCases=LabCases.GetForPeriod(contrApptPanel.DateStart,contrApptPanel.DateEnd,listOperatoryNums);
			FillLab(listLabCases);
			FillProduction(contrApptPanel.DateStart,contrApptPanel.DateEnd);
			FillProductionGoal(contrApptPanel.DateStart,contrApptPanel.DateEnd);
			bool hasNotes=true;
			if(PrefC.IsODHQ){
				hasNotes=Security.IsAuthorized(EnumPermType.Schedules,true);
			}
			FillProvSched(hasNotes);
			FillEmpSched(hasNotes);
			FillWaitingRoom();
			contrApptPanel.RedrawAsNeeded();
			Plugins.HookAddCode(this,"ControlAppt.RefreshModuleScreenPeriod_end");
		}

		/// <summary>This refreshes the images on the pinboard, in case the view changed.</summary>
		private void RefreshPinboardImages(){
			if(contrApptPanel.TableAppointments==null){
				return;
			}
			for(int i=0;i<pinBoard.ListPinBoardItems.Count;i++){
				//I'm a little worried that this could be slow, but there is usually only 0 to 1 appt on pinboard
				DataRow dataRow=contrApptPanel.TableAppointments.Rows.OfType<DataRow>().FirstOrDefault(x=>PIn.Long(x["AptNum"].ToString())==pinBoard.ListPinBoardItems[i].AptNum);
				if(dataRow==null){
					continue;
				}
				pinBoard.ListPinBoardItems[i].DataRowAppt=dataRow;
				string pattern=PIn.String(dataRow["Pattern"].ToString());
				string patternShowing=contrApptPanel.GetPatternShowing(pattern);
				SizeF sizeAppt=contrApptPanel.SetSize(pattern);
				Bitmap bitmap=new Bitmap(pinBoard.Width-2,(int)sizeAppt.Height);
				using(Graphics g = Graphics.FromImage(bitmap)){
					contrApptPanel.GetBitmapForPinboard(g,dataRow,patternShowing,bitmap.Width,bitmap.Height);
				}
				pinBoard.ListPinBoardItems[i].BitmapAppt=bitmap;
				pinBoard.Invalidate();
				//bitmap.Dispose();//crashes
			}
		}

		///<summary>Happens once per minute.  It used to just move the red timebar down without querying the database.  If pref.ApptModuleRefreshesEveryMinute is on (it is by default), then this instead queries the database for appt signals so that the waiting room list shows accurately.  The update to the waiting room grid is on a different timer.</summary>
		public void TickRefresh(){
			//dates already set
			if(PrefC.GetBool(PrefName.ApptModuleRefreshesEveryMinute)) {
				if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0) {//Signal processing is disabled.
					RefreshPeriod();
				}
				else {
					//Calling Signalods.RefreshTimed() was causing issues for large customers. This resulted in 100,000+ rows of signalod's returned.
					//Now we only query for the specific signals we care about. Instead of using Signalods.SignalLastRefreshed we now use Signalods.ApptSignalLastRefreshed.
					//Signalods.ApptSignalLastRefreshed mimics the behavior of Signalods.SignalLastRefreshed but is guaranteed to not be stale from inactive sessions.
					List<Signalod> listSignals=Signalods.RefreshTimed(Signalods.DateTApptSignalLastRefreshed,new List<InvalidType>(){ InvalidType.Appointment,InvalidType.Schedules });
					List<long> listOpNumsVisible = contrApptPanel.ListOpsVisible.Select(x => x.OperatoryNum).ToList();
					List<long> listProvNumsVisible = contrApptPanel.ListProvsVisible.Select(x => x.ProvNum).ToList();
					bool isApptRefresh=Signalods.IsApptRefreshNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listSignals,listOpNumsVisible,listProvNumsVisible);
					bool isSchedRefresh=Signalods.IsSchedRefreshNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listSignals,listOpNumsVisible,listProvNumsVisible);
					//either we have signals from other machines telling us to refresh, or we aren't using signals, in which case we still want to refresh
					RefreshPeriod(isRefreshAppointments:isApptRefresh,isRefreshSchedules:isSchedRefresh);
				}
			}
			else {
				contrApptPanel.RedrawAsNeeded();//just for the red time line
			}
			Signalods.DateTApptSignalLastRefreshed=MiscData.GetNowDateTime();
			//GC.Collect();	
		}
		#endregion Methods - Private Refresh Screen

		#region Methods - Private Search
		private void DoSearch(bool isForMakeRecall=false) {
			Cursor=Cursors.WaitCursor;
			DateTime dateAfter;
			dateAfter=PIn.Date(dateSearch.Text);
			if(dateAfter.Year<1880) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			TimeSpan timeSpanBefore=new TimeSpan(0);
			if(textBefore.Text!="") {
				string[] stringArrayHrmin=textBefore.Text.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);//doesn't work with foreign times.
				string hr="0";
				if(stringArrayHrmin.Length>0) {
					hr=stringArrayHrmin[0];
				}
				string min="0";
				if(stringArrayHrmin.Length>1) {
					min=stringArrayHrmin[1];
				}
				timeSpanBefore=TimeSpan.FromHours(PIn.Double(hr))
					+TimeSpan.FromMinutes(PIn.Double(min));
				if(radioBeforePM.Checked && timeSpanBefore.Hours<12) {
						timeSpanBefore=timeSpanBefore+TimeSpan.FromHours(12);
				}
			}
			TimeSpan timeSpanAfter=new TimeSpan(0);
			if(textAfter.Text!="") {
				string[] stringArrayHrmin=textAfter.Text.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);//doesn't work with foreign times.
				string hr="0";
				if(stringArrayHrmin.Length>0) {
					hr=stringArrayHrmin[0];
				}
				string min="0";
				if(stringArrayHrmin.Length>1) {
					min=stringArrayHrmin[1];
				}
				timeSpanAfter=TimeSpan.FromHours(PIn.Double(hr))
					+TimeSpan.FromMinutes(PIn.Double(min));
				if(radioAfterPM.Checked && timeSpanAfter.Hours<12) {
					timeSpanAfter=timeSpanAfter+TimeSpan.FromHours(12);
				}
			}
			if(_listBoxProviders.Items.Count==0) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please pick a provider.");
				return;
			}
			long[] longArrayProvNums=new long[_listBoxProviders.Items.Count];
			List<long> listProvNums = new List<long>();
			for(int i=0;i<longArrayProvNums.Length;i++) {
				longArrayProvNums[i]=_listProvidersSearch[i].ProvNum;
				listProvNums.Add(_listProvidersSearch[i].ProvNum);
				//providersList.Add(providers[i]);
			}
			List<long> listOperatoryNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(Clinics.ClinicNum!=0) {//not HQ
					listClinicNums.Add(Clinics.ClinicNum);
					listOperatoryNums=Operatories.GetOpsForClinic(Clinics.ClinicNum).Select(x => x.OperatoryNum).ToList();//get ops for the currently selected clinic only
				}
				else {//HQ
					ApptView apptView=GetApptViewCur();
					if(ApptViews.IsNoneView(apptView)) {//none view
						MsgBox.Show(this,"Must have a view selected to search for appointment.");//this should never get hit. Just in case.
						return;
					}
					//get the disctinct clinic nums for the operatories in the current appointment view
					List<long> listOperatoryNumsForView=ApptViewItems.GetOpsForView(apptView.ApptViewNum);
					List<Operatory> listOperatories=Operatories.GetOperatories(listOperatoryNumsForView,true);
					listClinicNums=listOperatories.Select(x => x.ClinicNum).Distinct().ToList();
					listOperatoryNums=listOperatories.Select(x => x.OperatoryNum).ToList();
				}
			}
			else {//all non hidden ops
				listOperatoryNums=Operatories.GetDeepCopy(true).Select(x => x.OperatoryNum).ToList();
			}
			//the result might be empty
			if(pinBoard.SelectedIndex==-1){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please select an item on the pinboard.");//shouldn't happen
				return;
			}
			List<long> listDefNumsBlockoutTypes=null;
			AppointmentType appointmentType=null;
			long aptNum=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].AptNum;
			Appointment appointment=Appointments.GetOneApt(aptNum);
			if(appointment!=null) {
				appointmentType=AppointmentTypes.GetOne(appointment.AppointmentTypeNum);
			}
			//If this appointment has an AppointmentType associated to any BlockoutTypes, set them as listBlockoutTypes. We want to automatically filter results for them.
			if(appointmentType!=null && !String.IsNullOrEmpty(appointmentType.BlockoutTypes)) {
				List<string> listStrings=appointmentType.BlockoutTypes.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
				//Convert into a list of longs
				listDefNumsBlockoutTypes=listStrings.Select(x => PIn.Long(x,hasExceptions:false)).ToList();
			}
			_listScheduleOpenings=ApptSearch.GetSearchResults(aptNum,dateAfter,dateAfter.AddDays(731)
				,listProvNums,listOperatoryNums,listClinicNums,timeSpanBefore,timeSpanAfter,listBlockoutTypes:listDefNumsBlockoutTypes,isForMakeRecall:isForMakeRecall);
			listSearchResults.Items.Clear();
			for(int i=0;i<_listScheduleOpenings.Count;i++) {
				listSearchResults.Items.Add(
					_listScheduleOpenings[i].DateTimeAvail.ToString("ddd")+"\t"+_listScheduleOpenings[i].DateTimeAvail.ToShortDateString()+"     "
					+_listScheduleOpenings[i].DateTimeAvail.ToShortTimeString());
			}
			if(listSearchResults.Items.Count>0) {
				listSearchResults.SetSelected(0);
				ModuleSelected(_listScheduleOpenings[0].DateTimeAvail);
			}
			Cursor=Cursors.Default;
			//scroll to make visible?
			//highlight anything?
		}		
		
		///<summary>Positions the search box, fills it with initial data except date, and makes it visible.</summary>
		private void ShowSearch() {
			_listProvidersSearch=new List<Provider>();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			LayoutManager.MoveLocation(groupSearch,new Point(panelCalendar.Location.X,panelCalendar.Location.Y+panelCalendarLower.Location.Y+pinBoard.Bottom+2));
			textBefore.Text="";
			textAfter.Text="";
			_listBoxProviders.Items.Clear();
			if(pinBoard.SelectedIndex==-1){
				return;
			}
			DataRow dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
			bool isHygiene=PIn.Bool(dataRow["IsHygiene"].ToString());
			long provHyg=PIn.Long(dataRow["ProvHyg"].ToString());
			long provNum=PIn.Long(dataRow["ProvNum"].ToString());
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			for(int i=0;i<listProvidersShort.Count;i++) {
				if(isHygiene && listProvidersShort[i].ProvNum==provHyg) {
					//If their appiontment is hygine, the list will start with just their hygine provider
					_listBoxProviders.Items.Add(listProvidersShort[i].Abbr,listProvidersShort[i]);
					_listProvidersSearch.Add(listProvidersShort[i]);
				}
				else if(!isHygiene && listProvidersShort[i].ProvNum==provNum) {
					//If their appointment is not hygine, they will start with just their primary provider
					_listBoxProviders.Items.Add(listProvidersShort[i].Abbr,listProvidersShort[i]);
					_listProvidersSearch.Add(listProvidersShort[i]);
				}
			}
			Plugins.HookAddCode(this,"ContrAppt.ShowSearch_end",_listBoxProviders,aptNum);
			groupSearch.Visible=true;
		}
		#endregion Methods - Private Search

		#region Methods - Private Other
		///<summary>Handles the display and refresh when the appointment we are trying to operate on is null.</summary>
		private bool ApptIsNull(Appointment appointment) {
			if(appointment==null) {
				MsgBox.Show(this,"Selected appointment no longer exists.");
				RefreshPeriod();
				return true;
			}
			return false;
		}

		private void ASAP_Click() {
			if(!Security.IsAuthorized(EnumPermType.AppointmentEdit)) {
				return;
			}
			Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) {
				return;
			}		
			if(appointment.Priority==ApptPriority.ASAP) {
				MsgBox.Show(this,"Already ASAP");
				return;
			}
			Appointments.SetPriority(appointment,ApptPriority.ASAP);
			MsgBox.Show(this,"Done");
			Plugins.HookAddCode(this,"ContrAppt.OnASAP_Click_end",appointment,_patient);
		}

		private void AutomaticCallDialingDisabledMessage() {
			if(ProgramProperties.IsAdvertisingDisabled(ProgramName.DentalTekSmartOfficePhone)) {
				return;
			}
			MsgBox.Show("Automatic dialing of patient phone numbers requires an additional service.\r\n"
				+"Contact Open Dental for more information.");
			try {
				System.Diagnostics.Process.Start("http://www.opendental.com/resources/redirects/redirectdentaltekinfo.html");
			}
			catch(Exception) {
				MsgBox.Show("Could not find http://www.opendental.com/contact.html \r\n"
					+"Please set up a default web browser.");
			}
		}

		///<summary>Copied from FormApptsOther. Does not limit appointment creation, only warns user. This check should be run before creating a new appointment. </summary>
		private void CheckStatus() {
			if(_patient.PatStatus == PatientStatus.Inactive
				|| _patient.PatStatus == PatientStatus.Archived
				|| _patient.PatStatus == PatientStatus.Prospective) {
				MsgBox.Show(this,"Warning. Patient is not active.");
			}
			if(_patient.PatStatus == PatientStatus.Deceased) {
				MsgBox.Show(this,"Warning. Patient is deceased.");
			}
		}

		///<summary>Determines the selected Appt View based on the selected index in comboView.  When doSetOnNoChange=true, always sets the new ApptView
		///based on selection, otherwise, no change therefore do not set the view.</summary>
		private void ComboViewChanged() {
			long viewNumSelected=comboView.GetSelected<ApptView>()?.ApptViewNum??0;//Selected view.
			SetView(viewNumSelected,true);
		}

		///<summary>Copies several fields from the supplied Appointment to a new Appointment object, inserts it into the database, and sends the new 
		///appointment to the Pinboard. Only used for HQ currently.</summary>
		private void CopyApptStructure(Appointment appointment) {
			if(ApptIsNull(appointment)) {
				return;
			}
			Appointment appointmentNew=Appointments.CopyStructure(appointment);
			Appointments.Insert(appointmentNew);
			DataTable dataTable=Appointments.GetPeriodApptsTable(contrApptPanel.DateStart,contrApptPanel.DateStart,appointmentNew.AptNum,false);
			if(dataTable.Rows.Count==0){
				return;//silently fail
			}
			DataRow dataRow=dataTable.Rows[0];
			SendToPinboardDataRow(dataRow);
		}

		private void CopyToPin_Click() {
			if(!Security.IsAuthorized(EnumPermType.AppointmentMove)) {
				return;
			}
			//cannot allow moving completed procedure because it could cause completed procs to change date.  Security must block this.
			//ContrApptSingle3[thisIndex].DataRoww;
			Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(appointment==null) {
				MsgBox.Show(this,"Appointment not found.");
				return;
			}
			if(appointment.AptStatus==ApptStatus.Complete) {
				MsgBox.Show(this,"Not allowed to move completed appointments.");
				return;
			}
			if(PatRestrictionL.IsRestricted(appointment.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow==null){
				return;//silently fail
			}
			SendToPinboardDataRow(dataRow);
		}

		/// <summary>Opens a QR code to scan on a mobile device to checkin a patient.</summary>
		private void EClipboardQR_Click(object sender,System.EventArgs e) {
			ToolStripMenuItem menuItem=sender as ToolStripMenuItem;
			if(menuItem==null || contrApptPanel==null || contrApptPanel.SelectedAptNum==0) {
				MsgBox.Show("No appointment selected, please select an appointment first.");
				return;
			}
			Appointment apt=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(apt==null) {
				MsgBox.Show("No appointment selected, please select an appointment first.");
				return;
			}
			Func<string, MobileDataByte> funcInsertUnlock=InsertDataForUnlockCode;
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertUnlock);
			formMobileCode.ShowDialog();			
		}

		/// <summary>Inserts MobileDataByte for checkin.</summary>
		private MobileDataByte InsertDataForUnlockCode(string unlockCode) {
			if(MobileDataBytes.TryInsertPatientCheckin(_patient,unlockCode,out string errorMsg,out MobileDataByte mobileDataByte)) {
				return mobileDataByte;
			}
			MsgBox.Show(errorMsg);
			return null;
		}

		///<summary>Brings up FormASAP ready to send for an open time slot.</summary>
		private void DisplayFormAsapForWebSched(Appointment appointment=null) {
			DateTime dateTimeSelected=contrApptPanel.DateTimeClicked;
			long opNum=contrApptPanel.OpNumClicked;
			long apptNum=0;
			int apptLength=0;
			if(appointment!=null) {
				dateTimeSelected=appointment.AptDateTime;
				apptLength=appointment.Length;
				int apptTimeIncrement=PrefC.GetInt(PrefName.AppointmentTimeIncrement);//Based on the users appt view increment.
				//Get the current time and strip off the seconds.
				DateTime dateTimeMinReplacementStart=DateTimeOD.GetDateTimeHourAndMins(DateTime.Now).AddMinutes(20);
				//If the current time is less than 20 minutes before the appts start time, adjust the length of the allowed appointment replacement.
				//This way we only suggest an ASAP appointment that fits within the time slot of the broken appt minus the 20 mins needed to arrive at the location.
				//If the appt length is ever zero or less, simply return. 
				//E.g. The current time is 11:00 AM and an hour long appt that starts at 11:00 AM is broken. Only search for ASAP appts that fit within 11:20 AM - 12:00 PM
				if(appointment.AptDateTime<dateTimeMinReplacementStart) {
					//Adjust the minimum replacement starting time so that it is on a perfect time increment.
					int minsToAdd=0;
					int mod=dateTimeMinReplacementStart.Minute%apptTimeIncrement;
					if(mod>0) {
						minsToAdd=apptTimeIncrement-mod;
					}
					dateTimeMinReplacementStart=dateTimeMinReplacementStart.AddMinutes(minsToAdd);
					//Figure out how many minutes are needed to adjust the length of the allowed appointment replacement in order to fit within the new time slot.
					TimeSpan timeSpanApptLengthAdjust=dateTimeMinReplacementStart-appointment.AptDateTime;
					//If the entire appt is either in the past or ends before the minimum replacement start time, simply return.
					if(timeSpanApptLengthAdjust.TotalMinutes>=appointment.Length) {
						return;
					}
					//Adjust the "selected" date time so that it doesn't look for time slots too close to the current time.
					dateTimeSelected=dateTimeMinReplacementStart;
					//Only search for appointments with a length that will fit within the new time slot.
					apptLength-=(int)timeSpanApptLengthAdjust.TotalMinutes;
				}
				//An appointment is being broken/deleted, so we may need to prompt the user to make this slot available to ASAP List.
				if(!AppointmentL.PromptTextAsapList(appointment.ClinicNum)) {
					return;
				}
				opNum=appointment.Op;
				apptNum=appointment.AptNum;
			}
			DateRange dateRange;
			try {
				dateRange=AppointmentL.GetAsapRange(opNum,dateTimeSelected,apptNum,contrApptPanel.ListSchedules);
			}
			catch(ODException ex) {
				MessageBox.Show(this,ex.Message);
				return;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unexpected error occurred."),ex);
				return;
			}
			dateRange.Start=ODMathLib.Max(dateTimeSelected,dateRange.Start);
			if(_formASAP==null||_formASAP.IsDisposed) {
				_formASAP=new FormASAP();
			}
			_formASAP.ShowFormForWebSched(dateTimeSelected,dateRange.Start,dateRange.End,opNum,maxApptLengthFilter:apptLength);
			_formASAP.FormClosed+=FormASAP_FormClosed;
		}

		///<summary>Helper function for users who have BrokenApptRequiredOnMove enabled. Pref forces the user to pick whether the appt was missed or
		///cancelled before moving, deleting, copying to pinboard or sending to the unsched list.</summary>
		private bool DoApptBreakRequired(Appointment appointment,Patient patient=null) {
			if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove) && appointment.AptStatus==ApptStatus.Scheduled) {
				FrmApptBreakRequired frmApptBreakRequiredForce=new FrmApptBreakRequired();
				frmApptBreakRequiredForce.ShowDialog();
				if(!frmApptBreakRequiredForce.IsDialogOK) {
					return false;
				}
				if(patient==null) {
					patient=Patients.GetPat(appointment.PatNum);
				}
				AppointmentL.BreakApptHelper(appointment,patient,frmApptBreakRequiredForce.ProcedureCodeBrokenSelected);
			}
			return true;
		}

		///<summary>Gets the most recently selected ApptView, or runs queries to determine which ApptView is the appropriate view for the current user.
		///Can return null.</summary>
		///<param name="apptViewNumOverride">If contrApptPanel.ApptViewCur is null, and the appropriate ApptView.ApptViewNum has already been determined,
		///pass in apptViewNumOverride to avoid running queries.</param>
		private ApptView GetApptViewCur(long apptViewNumOverride=-1) {
			ApptView apptView=contrApptPanel.ApptViewCur;
			if(apptView!=null) {
				return apptView;
			}
			if(apptViewNumOverride < 0) {
				apptViewNumOverride=GetApptViewNumForUser();//Only run this query if we have to.
			}
			if(apptViewNumOverride==ApptViews.APPTVIEWNUM_NONE //and we specified it's the None view
				&& comboView.Items.GetAll<ApptView>().Any(x => x.ApptViewNum==ApptViews.APPTVIEWNUM_NONE)) //and the None view exists
			{
				apptView=comboView.Items.GetAll<ApptView>().FirstOrDefault(x => x.ApptViewNum==ApptViews.APPTVIEWNUM_NONE);
			}
			else {//Or a real ApptView was specified.
				apptView=ApptViews.GetApptView(apptViewNumOverride);
			}
			return apptView;
		}

		///<summary>Returns an ApptView for the currently logged in user and clinic combination. Can return null.  Will return the first available appointment view if this is the first time that this computer has connected to this database.</summary>
		private ApptView GetApptViewForUser() {
			//load the recently used apptview from the db, either the userodapptview table if an entry exists or the computerpref table if an entry for this computer exists
			ApptView apptView=null;
			UserodApptView userodApptView=UserodApptViews.GetOneForUserAndClinic(Security.CurUser.UserNum,Clinics.ClinicNum);
			if(userodApptView!=null) { //if there is an entry in the userodapptview table for this user
				if(_hasInitializedOnStartup) { //if ContrAppt has already been initialized
					apptView=ApptViews.GetApptView(userodApptView.ApptViewNum); //then load the view for the user in the userodapptview table
				}
				else if(Security.CurUser.ClinicIsRestricted) { //current user is restricted
					if(Clinics.ClinicNum!=ComputerPrefs.LocalComputer.ClinicNum) { //and FormOpenDental.ClinicNum (set to the current user's clinic) is not the computerpref clinic
						apptView=ApptViews.GetApptView(userodApptView.ApptViewNum); //then load the view for the user in the userodapptview table
					}
				}
			}
			if(apptView==null //if no entry in the userodapptview table
				&& Clinics.ClinicNum==ComputerPrefs.LocalComputer.ClinicNum) //and if the program level ClinicNum is the stored recent ClinicNum for this computer 
			{
				apptView=ApptViews.GetApptView(ComputerPrefs.LocalComputer.ApptViewNum);//use the computerpref for this computer and user
			}
			//Larger offices do not want to take the time to load all the data required to display the "none" view.
			//Therefore, for a NEW computer that is connecting to the database for the first time, load up the first available view that is not the none view.
			if(apptView==null) { //if no entry in the ComputerPref table (or "none" view in ComputerPref table)
				if(comboView.Items.GetAll<ApptView>().Any(x => x.ApptViewNum!=ApptViews.APPTVIEWNUM_NONE)) { //An appointment view other than "none" to select
					if(!_hasInitializedOnStartup) { //and ContrAppt has NOT been initialized yet.
						apptView=comboView.Items.GetAll<ApptView>().FirstOrDefault(x => x.ApptViewNum!=ApptViews.APPTVIEWNUM_NONE);
					}
					else if(PrefC.GetBool(PrefName.EnterpriseNoneApptViewDefaultDisabled) && comboView.SelectedIndex==-1) {
						//or enterprise preference is on to never default to 'None' and no selection has been made thus preserving appointment view behavior and
						//avoiding unnecessary refreshing of module that can introduce a bug where appointments would vanish if they were from hidden providers or
						//from views that were visible through 'None' view directly. 
						//comboView.SelectedIndex will be -1 when user selects a new clinic and our AptViews are refreshed.
						//Get the first view in comboView that is not the "None" view.
						apptView=comboView.Items.GetAll<ApptView>().FirstOrDefault(x => x.ApptViewNum!=ApptViews.APPTVIEWNUM_NONE);
					}
				}
			}
			//If apptView==null at this point, the user has explicitly selected the "none" view and we must honor this.
			//apptViewCur will be null at this point if the "None" view is the view the user last had selected, because we do not store an actual view for
			//the "None" view in the database.  However, we do keep a "None" view in memory in comboView's options, and we should return it here.
			if(apptView==null) {
				//Get the "None" view.  apptViewCur will be 
				apptView=comboView.Items.GetAll<ApptView>().FirstOrDefault(x => x.ApptViewNum==ApptViews.APPTVIEWNUM_NONE);
			}
			return apptView;
		}

		/// <summary></summary>
		private long GetApptViewNumForUser() {
			if(contrApptPanel.ApptViewCur!=null) {
				return contrApptPanel.ApptViewCur.ApptViewNum;
			}
			//GetApptViewForUser needs a CurUser to the specific appointment views for the clinic/user combination
			if(Security.CurUser==null) {
				//No valid user so the appointment view will be set to whatever the computerpref table has for the current computer
				return ComputerPrefs.LocalComputer.ApptViewNum;
			}
			ApptView apptView=GetApptViewForUser();
			if(apptView==null) {
				return ApptViews.APPTVIEWNUM_NONE;
			}
			return apptView.ApptViewNum;
		}

		private void HandlePinClicked(ODEventArgs e) {//What to do if a user double clicks an appointment in DashApptGrid, then clicks 'Send to Pinboard'.
			if(e.EventType!=ODEventType.SendToPinboard){
				return;
			}
			if(e==null||e.Tag==null||e.Tag.GetType()!=typeof(PinBoardArgs)) {
				return;
			}
			ApptOther apptOther=((PinBoardArgs)e.Tag).ApptOther_;
			List<ApptOther> listApptOthers=((PinBoardArgs)e.Tag).ListApptOthers;
			long[] apptOtherNumArray={ apptOther.AptNum };
			this.InvokeIfRequired(() => {
				if(!AppointmentL.OKtoSendToPinboard(apptOther,listApptOthers,this)) {
					return;
				}
				ProcessOtherDlg(OtherResult.CopyToPinBoard,((PinBoardArgs)e.Tag).Patient_.PatNum,"",apptOtherNumArray);
			});
		}

		///<summary>Checks if the appointment's start time overlaps another appt in the Op which the apt resides.  Tests all appts for the day, even if not visible.
		///Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary.
		///Returns true if no overlap found. Returns false if given apt start time conflicts with another apt in the Op.</summary>
		private bool HasValidStartTime(Appointment apt) {
			if(PrefC.GetBool(PrefName.ApptsAllowOverlap)){
				return true;
			}
			bool notUsed;
			//Only valid if no adjust was needed.
			return !Appointments.TryAdjustAppointment(apt,contrApptPanel.ListOpsVisible,false,false,false,false,out notUsed);
		}

		///<summary>Checks if the appointment's start time would overlap a blockout that it cannot be scheduled over.
		///Returns true if no overlap is found. Returns false if given appointment's start time would conflict with a blockout in the operatory.</summary>
		private bool CanScheduleAppointmentTypeOnBlockoutType(Appointment appointment) {
			List<Schedule> listScheduleBlockoutsOverlapping=Appointments.GetBlockoutsOverlappingNoSchedule(appointment);
			for(int i=0;i<listScheduleBlockoutsOverlapping.Count;i++) {
				if(appointment.AptDateTime>=listScheduleBlockoutsOverlapping[i].DateTimeStart && appointment.AptDateTime<listScheduleBlockoutsOverlapping[i].DateTimeStop) {
					return false;
				}
			}
			return true;
		}

		private void UpdateAppointmentToUnscheduled(Appointment appointment,Appointment appointmentOld) {
			appointment.AptStatus=ApptStatus.UnschedList;
			try {
				Appointments.Update(appointment,appointmentOld);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Returns true if the none appointment view is selected, clinics is turned on, and the Headquarters clinic is selected.  Also disables pretty much every control available in the appointment module if it is going to return true, otherwise re-enables them.</summary>
		private bool IsHqNoneView() {
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum==0 && ApptViews.IsNoneView(contrApptPanel.ApptViewCur)) {
				//The "none" appt view is selected
				contrApptPanel.Visible=false;
				labelNoneView.Visible=true;
				butBack.Enabled=false;
				butBackMonth.Enabled=false;
				butBackWeek.Enabled=false;
				butToday.Enabled=false;
				butFwd.Enabled=false;
				butFwdMonth.Enabled=false;
				butFwdWeek.Enabled=false;
				monthCalendarOD.Enabled=false;
				pinBoard.ClearAt(pinBoard.SelectedIndex);
				textLab.Text="";
				textProduction.Text="";
				//Future improvement: Change this to only stop printing and lists
				toolBarMain.Visible=false;
				return true;
			}
			//either clinics are not enabled, or a clinic is selected
			contrApptPanel.Visible=true;
			labelNoneView.Visible=false;
			butBack.Enabled=true;
			butBackMonth.Enabled=true;
			butBackWeek.Enabled=true;
			butToday.Enabled=true;
			butFwd.Enabled=true;
			butFwdMonth.Enabled=true;
			butFwdWeek.Enabled=true;
			monthCalendarOD.Enabled=true;
			toolBarMain.Visible=true;
			return false;
		}

		public void LayoutControls(){
			Size sizeMonthCalendar=monthCalendarOD.GetDefaultSize();
			LayoutManager.Move(monthCalendarOD,new Rectangle(0,labelDate.Bottom+LayoutManager.Scale(3),
				LayoutManager.Scale(sizeMonthCalendar.Width),LayoutManager.Scale(sizeMonthCalendar.Height)));
			int w=monthCalendarOD.Width;//a number of things will be set to this width
			LayoutManager.Move(panelCalendarLower,new Rectangle(0,monthCalendarOD.Bottom+1,w,LayoutManager.Scale(317)));
			LayoutManager.Move(panelCalendar,new Rectangle(Width-w,toolBarMain.Bottom,w,panelCalendarLower.Bottom));
			LayoutManager.Move(tabControl,new Rectangle(panelCalendar.Left,panelCalendar.Bottom,Width-tabControl.Left,Height-tabControl.Top));
			LayoutManager.Move(contrApptPanel,new Rectangle(0,toolBarMain.Height,panelCalendar.Left,Height-toolBarMain.Height));
		}

		///<summary>Used for the UpdateProvs tool to reassign all future appointments for one op to another prov. </summary>
		public void MoveAppointments(List<Appointment> listAppts,List<Appointment> listApptsOld,Operatory operatoryCur){
			List<Schedule> listSchedulesForOp=Schedules.GetSchedsForOp(operatoryCur,listAppts.Select(x => x.AptDateTime).ToList());
			List<Operatory> listOperatoriesForClinic=contrApptPanel.ListOpsVisible.Select(x => x.Copy()).ToList();
			if(((ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)).In(
				ApptSchedEnforceSpecialty.Block,ApptSchedEnforceSpecialty.Warn)) {
				//if specialties are enforced, don't auto-move appt into an op assigned to a different clinic than the curOp's clinic
				listOperatoriesForClinic.RemoveAll(x => x.ClinicNum!=operatoryCur.ClinicNum);
			}
			for(int i=0;i<listAppts.Count;i++) {
				Appointment appointment=listAppts[i];
				Appointment appointmentOld=listApptsOld[i];
				MoveAppointment(appointment,appointmentOld,listSchedulesForOp,true);
			}
		}

		///<summary>Mostly for moving a single appointment.  Similar to the logic which runs in pinBoard_MouseUp(), but pinBoard_MouseUp() has additional things that are done.  This is also used for the UpdateProvs tool to reassign all future appointments for one op to another prov.</summary>
		private void MoveAppointment(Appointment appointment,Appointment appointmentOld,List<Schedule> listSchedeulesForOp=null,bool isOpUpdate=false){
			bool timeWasMoved=appointment.AptDateTime!=appointmentOld.AptDateTime;
			bool isOpChanged=appointment.Op!=appointmentOld.Op;
			Operatory operatory=Operatories.GetOperatory(appointment.Op);
			Patient patient=null;
			//List<Schedule> listSchedsForOp=Schedules.GetSchedsForOp(appt.Op,listAppts.Select(x => x.AptDateTime).ToList());
			List<Operatory> listOperatoriesForClinic=contrApptPanel.ListOpsVisible.Select(x => x.Copy()).ToList();
			if(((ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)).In(
				ApptSchedEnforceSpecialty.Block,ApptSchedEnforceSpecialty.Warn)) {
				//if specialties are enforced, don't auto-move appt into an op assigned to a different clinic than the curOp's clinic
				listOperatoriesForClinic.RemoveAll(x => x.ClinicNum!=operatory.ClinicNum);
			}
			patient=Patients.GetPat(appointment.PatNum);
			if(!isOpUpdate && appointment.AptDateTime.Date!=appointmentOld.AptDateTime.Date) {
				//Not moving a list of appointments, and the appointment is moving across days. Check if we need to force an appt break.
				if(PrefC.GetBool(PrefName.BrokenApptRequiredOnMove) && appointment.AptStatus==ApptStatus.Scheduled) {
					FrmApptBreakRequired frmApptBreakRequiredForce=new FrmApptBreakRequired();
					frmApptBreakRequiredForce.ShowDialog();
					if(!frmApptBreakRequiredForce.IsDialogOK) {
						return;
					}
					AppointmentL.BreakApptHelper(appointment,patient,frmApptBreakRequiredForce.ProcedureCodeBrokenSelected);
					//The appointment status in the database has been updated, but the in memory object MUST have its status updated for
					//logic that will run later on down the line.
					appointment.AptStatus=ApptStatus.Broken;
				}
			}
			bool provChanged=false;
			bool hygChanged=false;
			long assignedDent=0;
			long assignedHyg=0;
			if(isOpUpdate) {
				assignedDent=Schedules.GetAssignedProvNumForSpot(listSchedeulesForOp,operatory,false,appointment.AptDateTime);
				assignedHyg=Schedules.GetAssignedProvNumForSpot(listSchedeulesForOp,operatory,true,appointment.AptDateTime);
			}
			else { 
				assignedDent=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatory,false,appointment.AptDateTime);
				assignedHyg=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatory,true,appointment.AptDateTime);
			}
			List<Procedure> listProceduresForSingleApt=null;
			if(appointment.AptStatus!=ApptStatus.PtNote && appointment.AptStatus!=ApptStatus.PtNoteCompleted) {
				if(timeWasMoved) {
					#region Update Appt's DateTimeAskedToArrive
					if(patient.AskToArriveEarly>0) {
						appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-patient.AskToArriveEarly);
						MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+patient.AskToArriveEarly
							+" "+Lan.g(this,"minutes early at")+" "+appointment.DateTimeAskedToArrive.ToShortTimeString()+".");
					}
					else {
						if(appointment.DateTimeAskedToArrive.Year>1880 && (appointmentOld.AptDateTime-appointmentOld.DateTimeAskedToArrive).TotalMinutes>0) {
							appointment.DateTimeAskedToArrive=appointment.AptDateTime-(appointmentOld.AptDateTime-appointmentOld.DateTimeAskedToArrive);
							if(MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+(appointmentOld.AptDateTime-appointmentOld.DateTimeAskedToArrive).TotalMinutes
								+" "+Lan.g(this,"minutes early at")+" "+appointment.DateTimeAskedToArrive.ToShortTimeString()+"?","",MessageBoxButtons.YesNo)==DialogResult.No) {
								appointment.DateTimeAskedToArrive=appointmentOld.DateTimeAskedToArrive;
							}
						}
						else {
							appointment.DateTimeAskedToArrive=DateTime.MinValue;
						}
					}
					#endregion Update Appt's DateTimeAskedToArrive
				}
				#region Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				//if no dentist/hygienist is assigned to spot, then keep the original dentist/hygienist without prompt.  All appts must have prov.
				if((assignedDent!=0 && assignedDent!=appointment.ProvNum) || (assignedHyg!=0 && assignedHyg!=appointment.ProvHyg)) {
					object[] objectArray3={ appointment,assignedDent,assignedHyg,listProceduresForSingleApt,this };//Only used in following plugin hook.
					if((Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_MouseUp_apptProvChangeQuestion",objectArray3))) {
						appointment=(Appointment)objectArray3[0];
						assignedDent=(long)objectArray3[1];
						assignedDent=(long)objectArray3[2];
						goto PluginApptProvChangeQuestionEnd;
					}
					if(isOpUpdate || MsgBox.Show(this,MsgBoxButtons.YesNo,"Change provider?")) {//Short circuit logic.  If we're updating op through right click, never ask.
						if(assignedDent!=0) {//the dentist will only be changed if the spot has a dentist.
							appointment.ProvNum=assignedDent;
							provChanged=true;
						}
						if(assignedHyg!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
							appointment.ProvHyg=assignedHyg;
							hygChanged=true;
						}
						appointment.IsHygiene=IsOperatoryHygiene(appointment.IsHygiene,operatory,assignedDent,assignedHyg);
						listProceduresForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,false);
						List<long> codeNums=new List<long>();
						for(int p = 0;p<listProceduresForSingleApt.Count;p++) {
							codeNums.Add(listProceduresForSingleApt[p].CodeNum);
						}
						if(!isOpUpdate) { 
							bool doMake5Minute=listProceduresForSingleApt.Count>0;//Appointments without procs are already returned in 5 minute increments.
							string calcPattern=Appointments.CalculatePattern(appointment.ProvNum,appointment.ProvHyg,codeNums,doMake5Minute);
							if(appointment.Pattern!=calcPattern) {//Updating op provs will not change apt lengths.
								if(Security.IsAuthorized(EnumPermType.AppointmentResize,suppressMessage:true) && !appointment.TimeLocked) {
									//User is authorized, and appt time not locked. Do not give popup for users without resizing permissions or for Timelocked appointments.
									if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change length for new provider?")) {
										appointment.Pattern=calcPattern;
									}
								}
							}
						}
					}
					PluginApptProvChangeQuestionEnd: { }
				}
				else if(isOpUpdate) {
					//It should not be possible to remove the dentist from the appointment.
					//However, a user could have removed the hygienist from the operatory level (optional field) and should propagate to the appointment.
					if(assignedHyg!=appointment.ProvHyg) {
						appointment.ProvHyg=assignedHyg;
						hygChanged=true;
					}
					//Reconsider the IsHygiene flag defaulting to the appointments current value.
					appointment.IsHygiene=IsOperatoryHygiene(appointment.IsHygiene,operatory,assignedDent,assignedHyg);
				}
				#region Provider Term Date Check
				//Prevents appointments with providers that are past their term end date from being scheduled
				string message=Providers.CheckApptProvidersTermDates(appointment);
				if(message!="") {
					MessageBox.Show(message);//translated in Providers S class method
					return;
				}
				#endregion Provider Term Date Check
				#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
			}
			#region Prevent overlap
			//Check for any blockout collisions when overlapping appointments are allowed.
			if(PrefC.GetBool(PrefName.ApptsAllowOverlap)) {
				if(!isOpUpdate && ShowAppointmentBlockoutMessage(appointment)) {
					return;
				}
			}
			else {//Appointments are not allowed to overlap so check for both appointment and blockout collisions.
				if(!isOpUpdate && !Appointments.TryAdjustAppointmentOp(appointment,listOperatoriesForClinic)) {
					MsgBox.Show(this,"Appointment overlaps existing appointment or blockout.");
					return;
				}
			}
			#endregion Prevent overlap
			#region Detect Frequency Conflicts
			//Detect frequency conflicts with procedures in the appointment
			DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(patient.PatNum);
			if(discountPlanSub==null) {
				if(!isOpUpdate && PrefC.GetBool(PrefName.InsChecksFrequency)) {
					listProceduresForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);
					string frequencyConflicts="";
					try {
						frequencyConflicts=Procedures.CheckFrequency(listProceduresForSingleApt,appointment.PatNum,appointment.AptDateTime);
					}
					catch(Exception e) {
						MessageBox.Show(Lan.g(this,"There was an error checking frequencies.  Disable the Insurance Frequency Checking feature or try to fix the following error:")
							+"\r\n"+e.Message);
						return;
					}
					if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"Scheduling this appointment for this date will cause frequency conflicts for the following procedures")
						+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
					{
						return;
					}
				}
			}
			else {
				listProceduresForSingleApt=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);
				string frequencyConflicts="";
				try {
					frequencyConflicts=DiscountPlans.CheckDiscountFrequencyAndValidateDiscountPlanSub(listProceduresForSingleApt,appointment.PatNum,appointment.AptDateTime);
				}
				catch(Exception e) {
					MessageBox.Show(Lan.g(this,"There was an error checking discount frequencies:")
						+"\r\n"+e.Message);
					return;
				}
				if(!string.IsNullOrEmpty(frequencyConflicts) && MessageBox.Show(Lan.g(this,"This appointment will cause frequency conflicts for the following procedures")
					+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
				{
					return;
				}
			}
			#endregion Detect Frequency Conflicts
			#region Patient status
			if(!isOpUpdate) {
				Operatory operatory2=Operatories.GetOperatory(appointment.Op);
				Operatory operatoryOld=Operatories.GetOperatory(appointmentOld.Op);
				if(operatoryOld==null||operatory2.SetProspective!=operatoryOld.SetProspective) {
					if(operatory2.SetProspective&&patient.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
							Patient patientOld=patient.Copy();
							patient.PatStatus=PatientStatus.Prospective;
							Patients.UpdateRecalls(patient,patientOld,"Appointment Module, Appointment moved to prospective operatory");
							Patients.Update(patient,patientOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+patient.PatStatus.GetDescription()+Lan.g(this," by moving the patient appointment to a prospective operatory.");
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patient.PatNum,logEntry);
						}
					}
					else if(!operatory2.SetProspective&&patient.PatStatus==PatientStatus.Prospective) {
						//Do we need to warn about changing FROM prospective? Assume so for now.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
							Patient patientOld=patient.Copy();
							patient.PatStatus=PatientStatus.Patient;
							Patients.UpdateRecalls(patient,patientOld,"Appointment Module, Appointment moved from prospective operatory");
							Patients.Update(patient,patientOld);
							string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()+Lan.g(this," to ")
								+patient.PatStatus.GetDescription()+Lan.g(this," by moving the patient appointment from a prospective operatory.");
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patient.PatNum,logEntry);
						}
					}
				}
			}
			#endregion Patient status
			#region Update Appt's AptStatus, ClinicNum, Confirmed
			object[] objectArray = { appointment.AptDateTime,appointmentOld.AptDateTime,appointment.AptStatus };
			if((Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_MouseUp_apptDoNotUnbreakApptSameDay",objectArray))) {
				appointment.AptStatus=(ApptStatus)objectArray[2];
				goto PluginApptDoNotUnbreakApptSameDay;
			}
			if(appointment.AptStatus==ApptStatus.Broken) {
				if(timeWasMoved||isOpChanged) {
					//If pref BrokenApptRequiredOnMove is on, then we want users to be able to move appointments across days but force a break.
					//Appt goes from scheduled to broken back to scheduled. This is relevent because later on we compare if the apptOld.AptStatus has changed
					//to update the DB and without this line we would be comparing Scheduled to Scheduled, missing the update for the broken in the middle.
					appointmentOld.AptStatus=appointmentOld.AptStatus==ApptStatus.Scheduled ? ApptStatus.Broken : appointmentOld.AptStatus; 
					appointment.AptStatus=ApptStatus.Scheduled;
				}
			}
			PluginApptDoNotUnbreakApptSameDay: { }
			//original location of provider code
			if(operatory.ClinicNum==0) {
				appointment.ClinicNum=patient.ClinicNum;
			}
			else {
				appointment.ClinicNum=operatory.ClinicNum;
			}
			if(appointment.AptDateTime!=appointmentOld.AptDateTime
				&& appointment.Confirmed!=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum
				&& appointment.AptDateTime.Date!=DateTime.Today) 
			{
				string prompt;
				if(PrefC.GetBool(PrefName.ApptConfirmAutoEnabled)){
					prompt=Lan.g(this,"Do you want to resend the eConfirmation?");
				}
				else if(PrefC.GetBool(PrefName.ApptThankYouAutoEnabled)) {
					prompt=Lan.g(this,"Do you want to resend the eThankYou?");
				}
				else{
					prompt=Lan.g(this,"Reset Confirmation Status?");
				}
				bool isResetConf=MsgBox.Show(this,MsgBoxButtons.YesNo,prompt);
				if(isResetConf) {
					appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
				}
				if(PrefC.GetBool(PrefName.ApptConfirmAutoEnabled)) {
					List<ConfirmationRequest> listConfirmationRequests=ConfirmationRequests.GetAllForAppts(new List<long> { appointment.AptNum});
					for(int i=0;i<listConfirmationRequests.Count();i++) {
						//If they selected No, this will force the econnector to not delete the row and therefore not send another eConfirmation.
						//If they selected Yes, we will clear the DoNotResend flag so that it will get sent.
						listConfirmationRequests[i].DoNotResend=!isResetConf;
						ConfirmationRequests.Update(listConfirmationRequests[i]);
					}
				}
				if(PrefC.GetBool(PrefName.ApptThankYouAutoEnabled)) {
					List<ApptThankYouSent> listApptThankYouSents=ApptThankYouSents.GetForApt(appointment.AptNum);
					for(int i=0;i<listApptThankYouSents.Count();i++) {
						//If they selected No, this will force the econnector to not delete the row and therefore not send another eThankYou.
						//If they selected Yes, we will clear the DoNotResend flag so that it will get sent.
						listApptThankYouSents[i].DoNotResend=!isResetConf;
						ApptThankYouSents.Update(listApptThankYouSents[i]);
					}
				}
			}
			if(isOpChanged && !appointment.IsHygiene) {//If a non-hygiene appointment is moved, update the IsHygiene value to that of the new operatory.
				appointment.IsHygiene=operatory.IsHygiene;
			}
			#endregion Update Appt's AptStatus, ClinicNum, Confirmed
			//Should only need this check if changing/updating Op. Assumes we didn't previously schedule the apt somewhere it shouldn't have been.
			if(!AppointmentL.IsSpecialtyMismatchAllowed(patient.PatNum,appointment.ClinicNum)) {
				return;
			}
			try {
				if(isOpUpdate) {
					Appointments.MoveValidatedAppointment(appointment,appointmentOld,patient,operatory,listSchedeulesForOp,listOperatoriesForClinic,provChanged,hygChanged,timeWasMoved,isOpChanged,isOpUpdate);
				}
				else { 
					Appointments.MoveValidatedAppointment(appointment,appointmentOld,patient,operatory,contrApptPanel.ListSchedules,listOperatoriesForClinic,provChanged,hygChanged,timeWasMoved,isOpChanged,isOpUpdate);
				}
			}
			catch(Exception e) {
				MsgBox.Show(this,e.Message);
			}
		}

		///<summary>Returns whether or not the op and assigned providers should be treated as IsHygiene. Returns the value of isHygieneCur if the IsHygiene status of the operatory cannot be determined.</summary>
		private bool IsOperatoryHygiene(bool isHygiene,Operatory operatory,long assignedDent,long assignedHyg) {
			if(operatory!=null && operatory.IsHygiene) {
				return true;
			}
			//op not marked as hygiene op
			if(assignedDent==0) {//no dentist assigned
				if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
					return true;
				}
				return isHygiene;
			}
			//dentist is assigned
			if(assignedHyg==0) {//hyg is not assigned
				return false;
			}
			//if both dentist and hyg are assigned, it's tricky
			//only explicitly set it if user has a dentist assigned to the op
			if(operatory != null && operatory.ProvDentist!=0) {
				return false;
			}
			return isHygiene;
		}

		///<summary></summary>
		private void PrintApptCard() {
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintApptCard,
				Lan.g(this,"Appointment reminder postcard printed"),
				printSituation:PrintSituation.Postcard,
				auditPatNum:_patient.PatNum,
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd2_PrintApptCard(object sender,PrintPageEventArgs ev) {
			Graphics g=ev.Graphics;
			long apptClinicNum=0;
			if(contrApptPanel.SelectedAptNum>0){
				apptClinicNum=PIn.Long(contrApptPanel.GetDataRowForSelected()["ClinicNum"].ToString());
			}		
			Clinic clinic=Clinics.GetClinic(apptClinicNum);
			//Return Address--------------------------------------------------------------------------
			string str="";
			string phone="";
			if(PrefC.HasClinicsEnabled && clinic!=null) {//Use clinic on appointment if clinic exists and has clinics enabled
				str=clinic.Description+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,60,60);
				str=clinic.Address+"\r\n";
				if(clinic.Address2!="") {
					str+=clinic.Address2+"\r\n";
				}
				str+=clinic.City+"  "+clinic.State+"  "+clinic.Zip+"\r\n";
				phone=clinic.Phone;
			}
			else {//Otherwise use practice information
				str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,60,60);
				str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
				if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
					str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
				}
				str+=PrefC.GetString(PrefName.PracticeCity)+"  "
					+PrefC.GetString(PrefName.PracticeST)+"  "
					+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
				phone=PrefC.GetString(PrefName.PracticePhone);
			}
			if(phone.Length==10) {
				str+=TelephoneNumbers.ReFormat(phone);
			}
			else {//any other phone format
				str+=phone;
			}
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,60,75);
			//Body text-------------------------------------------------------------------------------
			string name;
			str=Lan.g(this,"Appointment Reminders:")+"\r\n\r\n";
			Appointment[] appointmentArrayOnePat;
			Family family=Patients.GetFamily(_patient.PatNum);
			Patient patient=family.GetPatient(_patient.PatNum);
			for(int i=0;i<family.ListPats.Length;i++) {
				if(!_isPrintCardFamily && family.ListPats[i].PatNum!=patient.PatNum) {
					continue;
				}
				name=family.ListPats[i].FName;
				if(name.Length>15) {//trim name so it won't be too long
					name=name.Substring(0,15);
				}
				appointmentArrayOnePat=Appointments.GetForPat(family.ListPats[i].PatNum);
				for(int a=0;a<appointmentArrayOnePat.Length;a++) {
					if(appointmentArrayOnePat[a].AptDateTime.Date<=DateTime.Today) {
						continue;//ignore old appts
					}
					if(appointmentArrayOnePat[a].AptStatus!=ApptStatus.Scheduled){
						continue;
					}
					str+=name+": "+appointmentArrayOnePat[a].AptDateTime.ToShortDateString()+" "+appointmentArrayOnePat[a].AptDateTime.ToShortTimeString()+"\r\n";
				}
			}
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,9),Brushes.Black,40,180);
			//Patient's Address-----------------------------------------------------------------------
			Patient patientGuar;
			if(_isPrintCardFamily) {
				patientGuar=family.ListPats[0].Copy();
			}
			else {
				patientGuar=patient.Copy();
			}
			str=patientGuar.FName+" "+patientGuar.LName+"\r\n"
				+patientGuar.Address+"\r\n";
			if(patientGuar.Address2!="") {
				str+=patientGuar.Address2+"\r\n";
			}
			str+=patientGuar.City+"  "+patientGuar.State+"  "+patientGuar.Zip;
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,300,240);
			//CommLog entry---------------------------------------------------------------------------
			Commlog commlog=new Commlog();
			commlog.CommDateTime=DateTime.Now;
			commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			commlog.Note=Lan.g(this,"Appointment card sent");
			commlog.PatNum=patient.PatNum;
			commlog.UserNum=Security.CurUser.UserNum;
			//there is no dialog here because it is just a simple entry
			Commlogs.Insert(commlog);
			ev.HasMorePages = false;
		}

		private void PrintApptLabel() {
			Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appointment)) { return; }
			LabelSingle.PrintAppointment(contrApptPanel.SelectedAptNum);
		}

		///<summary>Processes the OtherResult from a call to FormApptsOther.</summary>
		private void ProcessOtherDlg(OtherResult otherResult,long patNum,string strDateJumpTo,long[] aptNumArray,List<long> listApptViewNums=null) {
			if(otherResult==OtherResult.Cancel) {
				return;
			}
			List<long> listSelectedAptNums;
			switch(otherResult) {
				case OtherResult.CopyToPinBoard:
				case OtherResult.NewToPinBoard:
					listSelectedAptNums=aptNumArray.ToList();
					//Looks scary, but currently users can only select one appointment at a time in FormApptsOther.cs.
					if(!DoApptBreakRequired(Appointments.GetOneApt(listSelectedAptNums.First()))) {
						return;
					}
					SendToPinBoardAptNums(listSelectedAptNums);
					//RefreshModuleDataPatient(patNum);//Do we need this it gets called at the end of SendToPinBoardAptNums
					GlobalFormOpenDental.PatientSelected(_patient,true,false);
					RefreshPeriod(listPinApptNums:listSelectedAptNums);
					break;
				case OtherResult.PinboardAndSearch:
					listSelectedAptNums=aptNumArray.ToList();
					SendToPinBoardAptNums(aptNumArray.ToList());
					if(contrApptPanel.IsWeeklyView) {
						break;
					}
					dateSearch.Text=strDateJumpTo;
					if(!groupSearch.Visible) {//if search not already visible
						ShowSearch();
					}
					DoSearch(isForMakeRecall:true);
					RefreshPeriod(listPinApptNums:listSelectedAptNums);
					break;
				case OtherResult.CreateNew:
					contrApptPanel.SelectedAptNum=aptNumArray[0];
					RefreshModuleDataPatient(patNum);
					GlobalFormOpenDental.PatientSelected(_patient,true,false);
					Appointment appointment=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					if(appointment==null) {//apt has been deleted
						return;
					}
					Appointment appointmentOld=appointment.Copy();
					if(!HasValidStartTime(appointment)) {
						MsgBox.Show(this,"Appointment start time would overlap another appointment.  Moving appointment to pinboard.");
						SendToPinBoardAptNums(new List<long> { appointment.AptNum });
						UpdateAppointmentToUnscheduled(appointment,appointmentOld);
						RefreshPeriod();
						break;
					}
					//if appts appttype is associated to blockouts and appt would start on an unassociated blockout, send to pinboard.
					if(!CanScheduleAppointmentTypeOnBlockoutType(appointment)) {
						MsgBox.Show(this,"Appointment type cannot be scheduled on this blockout.  Moving appointment to pinboard.");
						SendToPinBoardAptNums(new List<long> { appointment.AptNum });
						UpdateAppointmentToUnscheduled(appointment,appointmentOld);
						RefreshPeriod();
						break;
					}
					if(TryAdjustAppointmentPattern(appointment,contrApptPanel.ListOpsVisible)) {
						MsgBox.Show(this,"Appointment is too long and would overlap another appointment or blockout.  Automatically shortened to fit.");						
						try {
							Appointments.Update(appointment,appointmentOld);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
					}
					RefreshPeriod();
					break;
				case OtherResult.GoTo:
					contrApptPanel.SelectedAptNum=aptNumArray[0];
					contrApptPanel.DateSelected=PIn.Date(strDateJumpTo);
					if(_patient.PatNum!=patNum) {
						//ModuleSelected->RefreshModuleScreenPeriod, Appt won't be selected if PatCur.PatNum!=Appt.PatNum
						_patient=Patients.GetPat(patNum);
					}
					GlobalFormOpenDental.PatientSelected(_patient,true,false,true);
					long apptViewNum=comboView.GetSelected<ApptView>()?.ApptViewNum??0; //default to 'none' view
					long apptViewNumGoTo=GetApptViewNumGoTo(listApptViewNums,apptViewNum);
					if(apptViewNum!=apptViewNumGoTo && apptViewNumGoTo>0) {
						SetView(apptViewNumGoTo,saveToDb:false);
					}
					break;
			}
		}

		///<summary>Helper method to select an appointment view that contains the operatory a selected appointment was assigned to when coming from FormApptsOther.but_GoTo(...)</summary>
		private long GetApptViewNumGoTo(List<long> listApptViewNums,long curApptViewNum) {
			if(listApptViewNums==null) {
				return -1;//Shouldn't happen if we're in this method but just in case, we don't want anything to break.
			}
			if(curApptViewNum==ApptViews.APPTVIEWNUM_NONE) {
				return ApptViews.APPTVIEWNUM_NONE;
			}
			if(listApptViewNums.Count==0) {//No views contain the operatory associated with the appointment that was selected in FormApptsOther
				MsgBox.Show(Lan.g(this,"There are no appointment views that contain the selected appointment's operatory. You will need to create one in the setup menu."));
				return -1;
			}
			if(listApptViewNums.Contains(curApptViewNum)) {
				return curApptViewNum;//We already have the view we need. Do nothing.
			}
			return listApptViewNums.FirstOrDefault();
		}

		///<summary>Brings up the window to send text messagse to the patients.</summary>
		private void SendTextMessages(List<long> listPatNums) {
			if(!Security.IsAuthorized(EnumPermType.TextMessageSend)) {
				return;
			}
			if(listPatNums.Count==0) {
				MsgBox.Show(this,"No appointments this day to send text messages to.");
				return;
			}
			Clinic clinic=Clinics.GetClinic(Clinics.ClinicNum)??Clinics.GetDefaultForTexting()??Clinics.GetPracticeAsClinicZero();
			List<PatComm> listPatComms=Patients.GetPatComms(listPatNums,clinic,false);
			List<string> listPatsSkipped=new List<string>();
			for(int i=listPatComms.Count-1;i>=0;i--) {
				//Remove patients that can't receive texts.
				PatComm patComm=listPatComms[i];
				if(patComm.IsSmsAnOption) {
					continue;
				}
				listPatsSkipped.Add(patComm.FName+" "+patComm.LName+": "+patComm.GetReasonCantText());
				listPatComms.RemoveAt(i);
			}
			if(listPatsSkipped.Count > 0) {
				string msg=listPatsSkipped.Count+Lan.g(this," of the ")+listPatNums.Distinct().Count()+" "
					+Lan.g(this,"patients cannot receive text messages:")+"\r\n"+string.Join("\r\n",listPatsSkipped);
				if(listPatsSkipped.Count < 8) {
					MessageBox.Show(msg);
				}
				else {
					using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
					msgBoxCopyPaste.ShowDialog();
				}
			}
			if(listPatComms.Count==0) {
				return;
			}
			FormTxtMsgMany formTxtMsgMany=new FormTxtMsgMany(listPatComms,"",Clinics.ClinicNum,SmsMessageSource.DirectSms);
			formTxtMsgMany.DoCombineNumbers=true;
			formTxtMsgMany.Show();
		}

		///<summary>Used to send one or more appontments to the pinboard.  The other way to do it is SendToPinboardDataRow.</summary>
		private void SendToPinBoardAptNums(List<long> aptNums) {
			if(IsHqNoneView()) {
				MsgBox.Show(this,"Appointments can't be sent to the pinboard when an appointment view or clinic hasn't been selected.");
				return;
			}
			if(aptNums.Count==0) {
				return;
			}
			long patNum=0;
			for(int i=0;i<aptNums.Count;i++){
				//sometimes, before this method was called, module was refreshed, and these appts were included.
				DataRow dataRow=null;
				//We need to know the indexOfAppt in case it is a planned appointment and we need to find the appointment in the Rows collection in order to get information for it later
				int indexOfAppt=-1;
				for(int r=0;r<contrApptPanel.TableAppointments.Rows.Count;r++){
					if(contrApptPanel.TableAppointments.Rows[r]["AptNum"].ToString()!=aptNums[i].ToString()){
						continue;
					}
					indexOfAppt=r;
					dataRow=contrApptPanel.TableAppointments.Rows[r];
				}
				if(dataRow==null){
					bool includeVerifyIns=false;
					if(contrApptPanel.ListApptViewItems!=null
						&& contrApptPanel.ListApptViewItems.Exists(x=>x.ElementDesc==EnumApptViewElement.VerifyIns_V.GetDescription()))
					{
						includeVerifyIns=true;
					}
					//but sometimes, we need to go get the row manually
					DataTable dataTable=Appointments.GetPeriodApptsTable(contrApptPanel.DateStart,contrApptPanel.DateEnd,aptNums[i],false,includeVerifyIns:includeVerifyIns);
					if(dataTable.Rows.Count==0){
						continue;//fail silently?
					}
					dataRow=dataTable.Rows[0];
				}
				if(dataRow["AptStatus"].ToString()==((int)ApptStatus.Planned).ToString()) {
					//Planned appointment is on the pinboard, so we have to retrieve the table again for slightly different info.
					//This won't happen very frequently, and it's faster to do it again than to intelligently figure out how to do it once.
					DataTable table=Appointments.RefreshOneApt(aptNums[i],true).Tables["Appointments"];
					if(table.Rows.Count==0) {
						MsgBox.Show(this,"Planned appointment no longer exists.");
						continue;
					}
					dataRow=table.Rows[0];
					//We must update the information for this current appointment with what we just gathered in RefreshOneApt (currently it holds information from GetPeriodApptsTable which is incorrect)
					if(indexOfAppt>=0) {
						contrApptPanel.TableAppointments.Rows[indexOfAppt].ItemArray=dataRow.ItemArray;
					}
				}
				string pattern=PIn.String(dataRow["Pattern"].ToString());
				string patternShowing=contrApptPanel.GetPatternShowing(pattern);
				SizeF sizeAppt=contrApptPanel.SetSize(pattern);
				Bitmap bitmap=new Bitmap(pinBoard.Width-2,(int)sizeAppt.Height);
				using(Graphics g = Graphics.FromImage(bitmap)){
					contrApptPanel.GetBitmapForPinboard(g,dataRow,patternShowing,bitmap.Width,bitmap.Height);
				}
				long aptNum=PIn.Long(dataRow["AptNum"].ToString());
				pinBoard.AddAppointment(bitmap,aptNum,dataRow);
				bitmap.Dispose();//?
				if(i==aptNums.Count-1) { //Set the pt to the last appt on the pinboard.
					patNum=PIn.Long(dataRow["PatNum"].ToString());
				}
			}
			if(patNum==0 && _patient!=null) {
				patNum=_patient.PatNum;
			}
			RefreshModuleDataPatient(patNum); 
			if(_patient!=null) {
				GlobalFormOpenDental.PatientSelected(_patient,true,false);
			}
		}

		///<summary>Used when dragging an appt to the pinboard.  Another way to do it would be SendToPinBoardAptNums.</summary>
		private void SendToPinboardDataRow(DataRow dataRow) {
			if(IsHqNoneView()) {
				MsgBox.Show(this,"Appointments can't be sent to the pinboard when an appointment view or clinic hasn't been selected.");
				return;
			}
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			if(!DoApptBreakRequired(Appointments.GetOneApt(aptNum))) {
				return;
			}
			string pattern=PIn.String(dataRow["Pattern"].ToString());
			string patternShowing=contrApptPanel.GetPatternShowing(pattern);
			SizeF sizeAppt=contrApptPanel.SetSize(pattern);
			Bitmap bitmap=new Bitmap(pinBoard.Width-2,(int)sizeAppt.Height);
			using(Graphics g = Graphics.FromImage(bitmap)){
				contrApptPanel.GetBitmapForPinboard(g,dataRow,patternShowing,bitmap.Width,bitmap.Height);
			}
			pinBoard.AddAppointment(bitmap,aptNum,dataRow);
			bitmap.Dispose();//?
			long patNum=PIn.Long(dataRow["PatNum"].ToString());
			RefreshModuleDataPatient(patNum); 
			GlobalFormOpenDental.PatientSelected(_patient,true,false);
		}

		/// <summary>Called from ModuleSelected.  Just runs once for the purpose of setting start time.</summary>
		public void SetInitialStartTime() {
			if(_hasSetInitialStartTime) {
				return;
			}
			ApptView apptView=GetApptViewCur();
			if(!ApptViews.IsNoneView(apptView)) { //None view NOT selected.
				TimeSpan timeSpanApptScrollStart=apptView.ApptTimeScrollStart;
				#region IsScrollStartDynamic
				if(apptView.IsScrollStartDynamic) {//Scroll start time at the earliest scheduled operatory or appointment
					//jordan IsScrollStartDynamic seems annoying to me, but it does help prevent an appt from getting hidden above the start time.
					//And, it's just a one-time thing.
					//Get the schedules that have any operatory visible
					List<Schedule> listSchedulesVisible=new List<Schedule>();
					for(int i=0;i<contrApptPanel.ListSchedules.Count();i++) {
						if(contrApptPanel.ListSchedules[i].Ops.Any(x => contrApptPanel.ListOpsVisible.Exists(y => x==y.OperatoryNum))//The schedule is linked to a visible operatory
							|| contrApptPanel.ListOpsVisible.Exists(x => x.ProvDentist==contrApptPanel.ListSchedules[i].ProvNum && !x.IsHygiene)//The dentist is in a visible operatory
							|| contrApptPanel.ListOpsVisible.Exists(x => x.ProvHygienist==contrApptPanel.ListSchedules[i].ProvNum && x.IsHygiene)) //The hygienist is in a visible operatory
						{
							listSchedulesVisible.Add(contrApptPanel.ListSchedules[i]);
						}
					}
					long schedProvUnassinged=PrefC.GetLong(PrefName.ScheduleProvUnassigned);
					bool opShowsDefaultProv=false;
					for(int i=0;i<contrApptPanel.ListOpsVisible.Count();i++) {
						if((contrApptPanel.ListOpsVisible[i].ProvDentist!=0 && !contrApptPanel.ListOpsVisible[i].IsHygiene)) {
							continue;//The operatory has a provider assigned to it
						}
						if((contrApptPanel.ListOpsVisible[i].ProvHygienist!=0 && contrApptPanel.ListOpsVisible[i].IsHygiene)){
							continue;//The operatory has a provider assigned to it
						}
						if(contrApptPanel.ListSchedules.Any(x => x.Ops.Contains(contrApptPanel.ListOpsVisible[i].OperatoryNum))) {
							continue;//The operatory has a schedule assigned to it
						}
						opShowsDefaultProv=true;//The operatory will have the provider for unassigned operatories
						break;
					}
					if(opShowsDefaultProv && contrApptPanel.ListSchedules.Exists(x => x.ProvNum==schedProvUnassinged)) {//The provider for unassigned ops has a schedule
						//Add that provider's earliest schedule
						listSchedulesVisible.Add(contrApptPanel.ListSchedules.FindAll(x => x.ProvNum==schedProvUnassinged).OrderBy(x => x.StartTime).FirstOrDefault());
					}
					//Get the appointment times that are in a visible operatory
					List<TimeSpan> listVisAptTimes=new List<TimeSpan>();
					for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++) {
						long opNum=PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString());
//todo:
						if(!contrApptPanel.ListOpsVisible.Exists(x => x.OperatoryNum==opNum) //The appointment is in a visible operatory
							|| !new[] { "1","2","4","5","7","8" }.Contains(contrApptPanel.TableAppointments.Rows[i]["AptStatus"].ToString())) //Scheduled,Complete,ASAP,Broken,PtNote,PtNoteComp
						{
							continue;
						}
						listVisAptTimes.Add(PIn.Date(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString()).TimeOfDay);
					}
					TimeSpan timeSpanEarliestApt=new TimeSpan();
					TimeSpan timeSpanEarliestOp=new TimeSpan();
					if(listVisAptTimes.Count>0 && listSchedulesVisible.Count>0) {//There is at least one schedule and at least one appointment visible
						timeSpanEarliestApt=listSchedulesVisible.Min(x => x.StartTime);
						timeSpanEarliestOp=listVisAptTimes.Min();
						if(TimeSpan.Compare(timeSpanEarliestOp,timeSpanEarliestApt)==1) {//earliestOp is later than earliestApt
							timeSpanApptScrollStart=timeSpanEarliestApt;
						}
						else {//earliestApt is later than earliestOp or they are both equal
							timeSpanApptScrollStart=timeSpanEarliestOp;
						}
					}
					else if(listSchedulesVisible.Count>0) {//There is at least one visible schedule and no visible appointments
						timeSpanApptScrollStart=listSchedulesVisible.Min(x => x.StartTime);
					}
					else if(listVisAptTimes.Count>0) {//There is at least one visible appointment and no visible schedules
						timeSpanApptScrollStart=listVisAptTimes.Min();
					}
					//else apptTimeScrollStart will remain as the start time listed in the appt view		
				}
				#endregion IsScrollStartDynamic
				//Even if we skip isScrollStartDynamic, above, the code below will still handle ApptTimeScrollStart
				contrApptPanel.SetScrollByTime(timeSpanApptScrollStart);
			}
			else{//else no view selected
				contrApptPanel.SetScrollByTime(TimeSpan.FromHours(8));
			}
			_hasSetInitialStartTime=true;
		}

		/// <summary>Sets the index of comboView for the specified ApptViewNum.  Then, does a ModuleSelected().  If saveToDb, then it will remember the ApptViewNum and currently selected ClinicNum for this workstation.</summary>
		private void SetView(long apptViewNum,bool saveToDb) {
			comboView.SetSelectedKey<ApptView>(apptViewNum,x=>x.ApptViewNum,x=>Lan.g(this,"none"));//First item is None/0 view.
			if(comboView.SelectedIndex<0) { //Index will be -1 if office has 'None' ApptView disabled.
				comboView.SetSelected(0);
			}
			contrApptPanel.ApptViewCur=comboView.GetSelected<ApptView>();
			if(!_hasInitializedOnStartup) {
				return;//prevent ModuleSelected().
			}
			if(_hasInitializedOnStartup && !Visible) {
				return;
			}
			if(saveToDb) {
				ComputerPrefs.LocalComputer.ApptViewNum=apptViewNum;
				ComputerPrefs.LocalComputer.ClinicNum=Clinics.ClinicNum;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
				UserodApptViews.InsertOrUpdate(Security.CurUser.UserNum,Clinics.ClinicNum,apptViewNum);
			}
			if(_patient==null) {
				ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				return;
			}
			ModuleSelected(_patient.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
		}

		///<summary>Switches between weekly view and daily view.   Calls either RefreshPeriod or ModuleSelected.</summary>
		private void SetWeeklyView(bool isWeeklyView,bool skipModuleSelection=false) {
			//Completely independent from and does not affect contrApptPanel.DateSelected.
			//if the weekly view doesn't change, then use SetDateSelected or RefreshPeriod
			if(isWeeklyView) {
				toggleDayWeek.SetWeek();
				//butFwd.Enabled=false;//jordan 2023-11-29-This wasn't working very well
				//butBack.Enabled=false;
			}
			else {
				toggleDayWeek.SetDay();
				//butFwd.Enabled=true;
				//butBack.Enabled=true;
			}
			contrApptPanel.IsWeeklyView=isWeeklyView;
			if(!_hasInitializedOnStartup) {
				return;//prevent refreshing repeatedly on startup
			}
			if(skipModuleSelection) {
				return;//to prevent infinite loop. SetWeeklyView must be called to refresh day headers after changing pref start day.
			}
			long apptViewNum=contrApptPanel.ApptViewCur?.ApptViewNum??ApptViews.APPTVIEWNUM_NONE;
			if(isWeeklyView) {
				if(_patient==null) {
					ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
					return;
				}
				ModuleSelected(_patient.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				return;
			}
			RefreshPeriod(listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum),isRefreshSchedules:true);
		}

		///<summary>Shortens appt.Pattern if overlap is found in neighboring op within appt.Op. Pattern will be adjusted to a minimum of 1 until no overlap occurs. Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary. Returns true if pattern was adjusted. Returns false if pattern was not adjusted.</summary>
		public static bool TryAdjustAppointmentPattern(Appointment appointment,List<Operatory> listOpsVisible) {
			bool isPatternChanged;
			Appointments.TryAdjustAppointment(appointment,listOpsVisible,false,true,true,true,out isPatternChanged);
			return isPatternChanged;
		}

		///<summary>Returns true if a message was displayed to the user telling them that the appointment could not be scheduled, otherwise false.</summary>
		private bool ShowAppointmentBlockoutMessage(Appointment appointment) {
			Schedule scheduleBlockout=Appointments.GetBlockoutsOverlappingNoSchedule(appointment).FirstOrDefault();
			if(scheduleBlockout!=null) { //Appointment was moved on top of at least one conflicting blockout.
				string itemValue=Defs.GetValue(DefCat.BlockoutTypes,scheduleBlockout.BlockoutType);
				if(itemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
					MsgBox.Show(this,"Appointment cannot be scheduled on a blockout marked as 'Block appointment scheduling'.");
				} 
				else {
					MsgBox.Show(this,"Appointment type cannot be scheduled on this blockout.");
				}
				return true;
			}
			return false;
		}

		#endregion Methods - Private Other

		
	}

	#region Other Classes
	class MenuItemNames {
		public const string TextAsapList="Text Asap List";
		public const string BringOverlapToFront="Bring Overlapped Appt to Front";
		public const string CopyToPinboard="Copy to Pinboard";
		public const string CopyAppointmentStructure="Copy Appointment Structure";
		public const string EClipboardQR="Show eClipboard QR";
		public const string SendToUnscheduledList="Send to Unscheduled List";
		public const string BreakAppointment="Break Appointment";
		public const string MarkAsAsap="Mark as ASAP";
		public const string SetComplete="Set Complete";
		public const string Delete="Delete";
		public const string PatientAppointments="Patient Appointments";
		public const string PrintLabel="Print Label";
		public const string PrintCard="Print Card";
		public const string PrintCardEntireFamily="Print Card for Entire Family";
		public const string RoutingSlip="Routing Slip";
		public const string ClearAllBlockoutsForDay="Clear All Blockouts for Day";
		public const string ClearAllBlockoutsForDayOpOnly="Clear All Blockouts for Day, Op only";
		public const string ClearAllBlockoutsForDayClinicOnly="Clear All Blockouts for Day, Clinic only";
		public const string EditBlockoutTypes="Edit Blockout Types";
		public const string BlockoutSpacer="Blockout Spacer";
		public const string PhoneDiv="Phone Div";
		public const string HomePhone="Home Phone";
		public const string WorkPhone="Work Phone";
		public const string WirelessPhone="Wireless Phone";
		public const string TextDiv="Text Div";
		public const string SendText="Send Text";
		public const string SendConfirmationText="Send Confirmation Text";
		public const string SendComeInText="Send Come In Text";
		public const string SendPaymentLinkText="Send Payment Text";
		public const string SendEClipboardByod="Send eClipboard BYOD Text";
		public const string OrthoChart="Ortho Chart";
		public const string Jobs="Jobs";
		public const string JobsSpacer="Jobs Spacer";
		public const string TextApptsForDayOp="Text Appointments for Day, Op only";
		public const string TextApptsForDayView="Text Appointments for Day, Current View only";
		public const string TextApptsForDay="Text Appointments for Day";
		public const string DeleteWebSchedAsapBlockout="Delete Web Schedule ASAP Blockout";
		public const string EditBlockout="Edit Blockout";
		public const string CutBlockout="Cut Blockout";
		public const string CopyBlockout="Copy Blockout";
		public const string PasteBlockout="Paste Blockout";
		public const string DeleteBlockout="Delete Blockout";
		public const string AddBlockout="Add Blockout";
		public const string BlockoutCutCopyPaste="Blockout Cut-Copy-Paste";
		public const string CareCredit="CareCredit";
		public const string CareCreditAcceptDeclineOffer="CareCredit Accept/Decline Offer";
		public const string CareCreditApplicationNeeded="CareCredit Application Needed";
		public const string CareCreditDiv="CareCredit Div";
	}
	#endregion Other Classes
}




