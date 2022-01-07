using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using Tamir.SharpSsh.java.lang;

namespace OpenDental {
	///<summary></summary>
	public partial class FormASAP:FormODBase {
		///<summary>When texting patients on this list, this will be when the beginning of the slot is.</summary>
		public DateTime DateTimeChosen;
		///<summary>When texting patients on this list, this will be when the beginning of the slot is.</summary>
		public DateTime DateTimeSlotStart;
		///<summary>When texting patients on this list, this will be when the ending of the slot is.</summary>
		public DateTime DateTimeSlotEnd;
		private List<Appointment> _listAppointmentsASAP;
		private bool _doPrintHeading;
		private int _pagesPrinted;
		///<summary>A list of patient names and patNums. Gets refilled every time FillGridAppts() is invoked.</summary>
		private List<Patients.PatientName> _listPatientNames=new List<Patients.PatientName>();
		///<summary>The clinics that are signed up for Web Sched.</summary>
		private List<long> _listClinicNumsWebSched=new List<long>();
		ODThread _threadWebSchedSignups=null;
		///<summary>The user has clicked the Web Sched button while a thread was busy checking which clinics are signed up for Web Sched.</summary>
		private bool _hasClickedWebSched;
		///<summary>The operatory selected to send Web Sched messages for.</summary>
		private long _opNum;
		///<summary>Classwide instance of FormWebSchedASAPHistory.</summary>
		private FormWebSchedASAPHistory _formWebSchedASAPHistory;
		private List<Provider> _listProviders;
		private List<Site> _listSites;
		///<summary>Holds the maximum appt length which is then used to filter out appts with larger lengths from the Appointment ASAP List.
		///Also, appts still showing after considering this filter will be reordered so that the ones with lengths closest to this filter are first in the grid.</summary>
		private int _maxApptLengthFilter=0;

		private DateTime GetDateTimeSelectedStart() {
			DateTime dateTime=comboStart.GetSelected<DateTime>();
			if(dateTime==null) {
				return DateTime.MinValue;
			}
			return dateTime;
		}

		private DateTime GetDateTimeSelectedEnd() {
			DateTime dateTime=comboEnd.GetSelected<DateTime>();
			if(dateTime==null) {
				return DateTime.MinValue;
			}
			return dateTime;
		}

		///<summary>Whether or not the user is sending Web Sched notifications to patients.</summary>
		private bool IsSendingWebSched() {
			if(_opNum==0) {
				return false;
			}
			return true;
		}

		///<summary></summary>
		public FormASAP(long opNum=0) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_opNum=opNum;
		}

		private void FormASAP_Load(object sender,System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			LayoutMenu();
			SetFilterControlsAndAction(() => {
					if(tabControl.SelectedIndex==0) {
						FillGridAppts();
					}
					else {
						FillGridRecalls();
					}
				},comboProv,comboSite,comboClinic,comboAptStatus,comboNumberReminders,checkGroupFamilies);
			CheckClinicsSignedUpForWebSched();
			comboProv.Items.Add(Lan.g(this,"All"));
			comboProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(PrefC.GetBool(PrefName.EnterpriseApptList)){
				comboClinic.IncludeAll=false;
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else{
				comboSite.Items.Add(Lan.g(this,"All"));
				comboSite.SelectedIndex=0;
				_listSites=Sites.GetDeepCopy();
				for(int i=0;i<_listSites.Count;i++) {
					comboSite.Items.Add(_listSites[i].Description);
				}
			}
			splitContainer.Panel2Collapsed=true;
			if(PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				if(IsSendingWebSched()) {
					FillForWebSched();
				}
				else {//Not sending Web Sched ASAP
					LayoutManager.MoveLocation(butSendWebSched,new Point(butSendWebSched.Location.X,labelOperatory.Location.Y+6));
					LayoutManager.MoveLocation(butWebSchedHist,new Point(butWebSchedHist.Location.X,labelStart.Location.Y+9));
					LayoutManager.MoveLocation(butWebSchedNotify,new Point(butWebSchedNotify.Location.X,butWebSchedHist.Location.Y+29));
					LayoutManager.MoveHeight(groupWebSched,butSendWebSched.Height+LayoutManager.Scale(92));
				}
			}
			else {//Not signed up for Web Sched ASAP
				LayoutManager.MoveLocation(butSendWebSched,new Point(butSendWebSched.Location.X,labelOperatory.Location.Y+2));
				butSendWebSched.Text=Lan.g(this,"Sign up");
				butWebSchedHist.Visible=false;
				LayoutManager.MoveHeight(groupWebSched,butSendWebSched.Height+LayoutManager.Scale(26));
			}
			comboAptStatus.Items.Add(Lan.g(this,ApptStatus.Scheduled.ToString()),ApptStatus.Scheduled);
			comboAptStatus.Items.Add(Lan.g(this,ApptStatus.Planned.ToString()),ApptStatus.Planned);
			comboAptStatus.Items.Add(Lan.g(this,ApptStatus.UnschedList.ToString()),ApptStatus.UnschedList);
			comboAptStatus.Items.Add(Lan.g(this,ApptStatus.Broken.ToString()),ApptStatus.Broken);
			comboShowHygiene.Items.Add(Lan.g(this,FromASAPShowAppointment.All.GetDescription()),FromASAPShowAppointment.All);
			comboShowHygiene.Items.Add(Lan.g(this,FromASAPShowAppointment.NonHygiene.GetDescription()),FromASAPShowAppointment.NonHygiene);
			comboShowHygiene.Items.Add(Lan.g(this,FromASAPShowAppointment.Hygiene.GetDescription()),FromASAPShowAppointment.Hygiene);
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.RecallGroupByFamily);
			comboNumberReminders.Items.Add(Lan.g(this,"All"));
			comboNumberReminders.Items.Add("0");
			comboNumberReminders.Items.Add("1");
			comboNumberReminders.Items.Add("2");
			comboNumberReminders.Items.Add("3");
			comboNumberReminders.Items.Add("4");
			comboNumberReminders.Items.Add("5");
			comboNumberReminders.Items.Add("6+");
			comboNumberReminders.SelectedIndex=0;
			int daysPast=PrefC.GetInt(PrefName.RecallDaysPast);
			int daysFuture=PrefC.GetInt(PrefName.RecallDaysFuture);
			if(daysPast==-1){
				textDateStart.Text="";
			}
			else{
				textDateStart.Text=DateTime.Today.AddDays(-daysPast).ToShortDateString();
			}
			if(daysFuture==-1) {
				textDateEnd.Text="";
			}
			else {
				textDateEnd.Text=DateTime.Today.AddDays(daysFuture).ToShortDateString();
			}
			FillTimeCombos();
			if(IsSendingWebSched()) {
				FillWebSchedSent();
			}
			LayoutManager.LayoutFormBoundsAndFonts(this);
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Settings",menuItemSettings_Click));
			menuMain.EndUpdate();
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormASAP_Shown(object sender,EventArgs e) {
			FillGridAppts();
		}

		private void splitContainer_SplitterMoved(object sender,SplitterEventArgs e) {
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			LayoutManager.Move(gridRecalls,
				new Rectangle(1,LayoutManager.Scale(55),tabPageRecalls.Width-2,tabPageRecalls.Height-LayoutManager.Scale(55)-2));
			switch((sender as TabControl).SelectedIndex) {
				case 0:
				default:
					FillGridAppts();
					break;
				case 1:
					FillGridRecalls();
					break;
			}
		}

		#region Appointments Tab

		private void FillGridAppts(){
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			if(!SmsPhones.IsIntegratedTextingEnabled()) {
				butText.Enabled=false;
			}
			List<ApptStatus> listAppStatuses=comboAptStatus.GetListSelected<ApptStatus>();
			long clinicNum=-1;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				_listAppointmentsASAP=Appointments.RefreshASAP(provNum,siteNum,clinicNum,listAppStatuses,codeRangeFilter.StartRange,
					codeRangeFilter.EndRange);
				};
			progressOD.ShowDialogProgress();
			int scrollVal=gridAppts.ScrollValue;
			List<long> listAptNumsSelected=gridAppts.SelectedTags<Appointment>().Select(x => x.AptNum).ToList();
			if(_maxApptLengthFilter>0) {
				_listAppointmentsASAP.RemoveAll(x => x.Length>_maxApptLengthFilter);
				_listAppointmentsASAP=_listAppointmentsASAP.OrderByDescending(x => x.Length).ThenBy(x => x.AptDateTime).ToList();
			}
			gridAppts.BeginUpdate();
			gridAppts.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableASAP","Patient"),140);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Date"),65,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Unsched Status"),110);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Apt Status"),75);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Prov"),50);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Procedures"),150);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Length"),60,GridSortingStrategy.AmountParse);
			gridAppts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableASAP","Notes"),160);
			gridAppts.Columns.Add(col);
			gridAppts.ListGridRows.Clear();
			GridRow row;
			List<long> listPatNums=_listPatientNames.Select(x=>x.PatNum).ToList();
			if(_listAppointmentsASAP.Any(x => !listPatNums.Contains(x.PatNum))) {
				_listPatientNames=Patients.GetPatientNameList(_listAppointmentsASAP.Select(x => x.PatNum).ToList());
			}
			FromASAPShowAppointment fromASAPShowAppointment=comboShowHygiene.GetSelected<FromASAPShowAppointment>();
			for(int i=0;i<_listAppointmentsASAP.Count;i++) {
				if(fromASAPShowAppointment==FromASAPShowAppointment.Hygiene && !_listAppointmentsASAP[i].IsHygiene) {
					continue;
				}
				if(fromASAPShowAppointment==FromASAPShowAppointment.NonHygiene && _listAppointmentsASAP[i].IsHygiene) {
					continue;
				}
				row=new GridRow();
				Patients.PatientName patientName=_listPatientNames.FirstOrDefault(x=>x.PatNum==_listAppointmentsASAP[i].PatNum);
				if(patientName.Name is null) {
					patientName.Name=Lan.g(this,"UNKNOWN");
				}
				row.Cells.Add(patientName.Name);
				if(_listAppointmentsASAP[i].AptDateTime.Year < 1880){//shouldn't be possible.
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listAppointmentsASAP[i].AptDateTime.ToShortDateString());
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,_listAppointmentsASAP[i].UnschedStatus));
				row.Cells.Add(_listAppointmentsASAP[i].AptStatus.ToString());
				if(_listAppointmentsASAP[i].IsHygiene) {
					row.Cells.Add(Providers.GetAbbr(_listAppointmentsASAP[i].ProvHyg));
				}
				else {
					row.Cells.Add(Providers.GetAbbr(_listAppointmentsASAP[i].ProvNum));
				}
				row.Cells.Add(_listAppointmentsASAP[i].ProcDescript);
				row.Cells.Add(_listAppointmentsASAP[i].Length.ToString());
				row.Cells.Add(_listAppointmentsASAP[i].Note);
				row.Tag=_listAppointmentsASAP[i];
				gridAppts.ListGridRows.Add(row);
			}
			gridAppts.EndUpdate();
			gridAppts.ScrollValue=scrollVal;
			for(int i=0;i<gridAppts.ListGridRows.Count;i++) {
				if(ListTools.In(((Appointment)gridAppts.ListGridRows[i].Tag).AptNum,listAptNumsSelected)) {
					gridAppts.SetSelected(i,true);
				}
			}
		}

		private void gridAppts_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;
			int currentScroll=gridAppts.ScrollValue;
			Patient patient=Patients.GetPat(((Appointment)gridAppts.ListGridRows[e.Row].Tag).PatNum); //check against grid
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule: true);
			using FormApptEdit formApptEdit=new FormApptEdit(((Appointment)gridAppts.ListGridRows[e.Row].Tag).AptNum);
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK){
				return;
			}
			if(formApptEdit.PinClicked) {
				SendPinboard_Click(); //Whatever they double clicked on will still be selected, just fire the event to send it to the pinboard.
				DialogResult=DialogResult.OK;
			}
			else {
				FillGridAppts();
				gridAppts.SetSelected(currentSelection,true);
				gridAppts.ScrollValue=currentScroll;
			}
		}

		private void gridAppts_MouseUp(object sender,MouseEventArgs e) {
			if(IsSendingWebSched()) {
				FillWebSchedSent();
			}
		}

		private void RemoveAppts_Click() {
			if(!Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			if(gridAppts.SelectedIndices.Length>0 && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change priority to normal for all selected appointments?")) {
				return;
			}
			for(int i=0;i<gridAppts.SelectedIndices.Length;i++) {
				Appointment appointment=(Appointment)gridAppts.SelectedGridRows[i].Tag;
				DateTime datePrevious=appointment.DateTStamp;
				Appointments.SetPriority(appointment,ApptPriority.Normal);
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,appointment.PatNum,"Appointment priority set from ASAP to normal from ASAP list.",appointment.AptNum,
					datePrevious);
			}
			FillGridAppts();
		}

		private void SendPinboard_Click() {
			if(gridAppts.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listAptNums=new List<long>();
			List<Appointment> listAppointmentsSelected=gridAppts.SelectedIndices.Select(x => (Appointment)gridAppts.ListGridRows[x].Tag).ToList();
			List<Patient> listPatients=Patients.GetMultPats(listAppointmentsSelected.Select(x => x.PatNum).ToList()).ToList();
			int patsArchivedOrDeceased=0;
			for(int i=0;i<listAppointmentsSelected.Count;i++) {
				Patient patient=listPatients.FirstOrDefault(x => x.PatNum==listAppointmentsSelected[i].PatNum);
				if(patient!=null && ListTools.In(patient.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
					patsArchivedOrDeceased++;
					continue;
				}
				if(listAppointmentsSelected[i].AptStatus==ApptStatus.Planned) {
					if(Procedures.GetProcsForSingle(listAppointmentsSelected[i].AptNum,isPlanned: true).Count(x => x.ProcStatus==ProcStat.C) > 0) {
						MsgBox.Show(this,"Not allowed to send a planned appointment to the pinboard if completed procedures are attached. Edit the planned "
							+"appointment first.");
						return;
					}
					Appointment apppointmentNext=Appointments.GetScheduledPlannedApt(listAppointmentsSelected[i].AptNum);
					if(apppointmentNext!=null) {
						if(apppointmentNext.AptStatus==ApptStatus.Complete) {
							//Warn the user they are moving a completed appointment.
							if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to move an already completed appointment.  Continue?")) {
								return;
							}
						}
						else if(apppointmentNext.AptStatus==ApptStatus.Scheduled) {
							//Warn the user they are moving an already scheduled appointment.
							if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to move an appointment already on the schedule.  Continue?")) {
								return;
							}
						}
					}
				}
				listAptNums.Add(listAppointmentsSelected[i].AptNum);
			}
			List<string> listUserMsgs=new List<string>();
			if(patsArchivedOrDeceased > 0) {
				listUserMsgs.Add(Lan.g(this,"Appointments skipped because patient status is archived or deceased:")+" "+patsArchivedOrDeceased+".");
			}
			if(listAptNums.Count==0) {
				listUserMsgs.Add(Lan.g(this,"There are no appointments to send to the pinboard."));
			}
			if(listUserMsgs.Count>0) {
				MessageBox.Show(string.Join("\r\n",listUserMsgs));
				if(listAptNums.Count==0) {
					return;
				}
			}
			GotoModule.PinToAppt(listAptNums,0); //Pins all appointments to the pinboard that were in listAptNums.
		}
		#endregion Appointments Tab

		#region Recalls Tab

		private void FillGridRecalls() {
			DateTime dateFrom;
			DateTime dateTo;
			if(textDateStart.Text==""){
				dateFrom=DateTime.MinValue;
			}
			else{
				dateFrom=PIn.Date(textDateStart.Text);
			}
			if(textDateEnd.Text=="") {
				dateTo=DateTime.MaxValue;
			}
			else {
				dateTo=PIn.Date(textDateEnd.Text);
			}
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			long clinicNum=-1;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			RecallListShowNumberReminders recallListShowNumberReminders=(RecallListShowNumberReminders)comboNumberReminders.SelectedIndex;
			DataTable tableRecalls=null;
			List<Recall> listRecalls=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				tableRecalls=Recalls.GetRecallList(dateFrom,dateTo,checkGroupFamilies.Checked,provNum,clinicNum,
					siteNum,RecallListSort.DueDate,recallListShowNumberReminders,isAsap: true,codeRangeFilter.StartRange,codeRangeFilter.EndRange);
				listRecalls=Recalls.GetMultRecalls(tableRecalls.Rows.OfType<DataRow>().Select(x => PIn.Long(x["RecallNum"].ToString())).ToList());
			};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			List<long> listRecallNumsSelected=gridRecalls.SelectedTags<Recall>().Select(x => x.RecallNum).ToList();
			bool hasGridBeenFilledBefore=(gridRecalls.Columns.Count > 0);
			gridRecalls.BeginUpdate();
			gridRecalls.Columns.Clear();
			GridColumn col;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.RecallList);
			for(int i=0;i<listDisplayFields.Count;i++) {
				if(listDisplayFields[i].Description=="") {
					col=new GridColumn(listDisplayFields[i].InternalName,listDisplayFields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth);
				}
				col.Tag=listDisplayFields[i].InternalName;
				gridRecalls.Columns.Add(col);
			}
			gridRecalls.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<tableRecalls.Rows.Count;i++){
				row=new GridRow();
				for(int f=0;f<listDisplayFields.Count;f++) {
					switch(listDisplayFields[f].InternalName){
						case "Due Date":
							row.Cells.Add(tableRecalls.Rows[i]["dueDate"].ToString());
							break;
						case "Patient":
							row.Cells.Add(tableRecalls.Rows[i]["patientName"].ToString());
							break;
						case "Age":
							row.Cells.Add(tableRecalls.Rows[i]["age"].ToString());
							break;
						case "Type":
							row.Cells.Add(tableRecalls.Rows[i]["recallType"].ToString());
							break;
						case "Interval":
							row.Cells.Add(tableRecalls.Rows[i]["recallInterval"].ToString());
							break;
						case "#Remind":
							row.Cells.Add(tableRecalls.Rows[i]["numberOfReminders"].ToString());
							break;
						case "LastRemind":
							row.Cells.Add(tableRecalls.Rows[i]["dateLastReminder"].ToString());
							break;
						case "Contact":
							row.Cells.Add(tableRecalls.Rows[i]["contactMethod"].ToString());
							break;
						case "Status":
							row.Cells.Add(tableRecalls.Rows[i]["status"].ToString());
							break;
						case "Note":
							row.Cells.Add(tableRecalls.Rows[i]["Note"].ToString());
							break;
						case "BillingType":
							row.Cells.Add(tableRecalls.Rows[i]["billingType"].ToString());
							break;
						case "WebSched":
							row.Cells.Add(tableRecalls.Rows[i]["webSchedSendDesc"].ToString());
							break;
					}
				}
				row.Tag=listRecalls.FirstOrDefault(x => x.RecallNum==PIn.Long(tableRecalls.Rows[i]["RecallNum"].ToString()));
				Patients.PatientName patientName=new Patients.PatientName();
				patientName.PatNum=PIn.Long(tableRecalls.Rows[i]["PatNum"].ToString());
				patientName.Name=PIn.String(tableRecalls.Rows[i]["patientName"].ToString());
				if(_listPatientNames.Count==0 || !_listPatientNames.Any(x => x.PatNum==patientName.PatNum)) {
					_listPatientNames.Add(patientName);
				}
				gridRecalls.ListGridRows.Add(row);
			}
			gridRecalls.EndUpdate();
			if(!hasGridBeenFilledBefore) {
				SetSelectedRecalls();
			}
			for(int i=0;i<gridRecalls.ListGridRows.Count;i++) {
				if(ListTools.In(((Recall)gridRecalls.ListGridRows[i].Tag).RecallNum,listRecallNumsSelected)) {
					gridRecalls.SetSelected(i,true);
				}
			}
		}

		private void gridRecalls_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Recall recall=(Recall)gridRecalls.ListGridRows[e.Row].Tag;
			Patient patient=Patients.GetPat(recall.PatNum);
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule: true);
			using FormRecallEdit formRecallEdit=new FormRecallEdit();
			formRecallEdit.RecallCur=recall;
			formRecallEdit.ShowDialog();
			if(formRecallEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGridRecalls();
		}

		private void RemoveRecalls_Click() {
			if(gridRecalls.SelectedIndices.Length==0 
				|| !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change priority to normal for all selected recalls?")) 
			{
				return;
			}
			List<Recall> listRecallsSelected=gridRecalls.SelectedIndices.Select(x => (Recall)gridRecalls.ListGridRows[x].Tag).ToList();
			for(int i=0;i<listRecallsSelected.Count;i++) {
				listRecallsSelected[i].Priority=RecallPriority.Normal;
				Recalls.Update(listRecallsSelected[i]);
				SecurityLogs.MakeLogEntry(Permissions.RecallEdit,listRecallsSelected[i].PatNum,"Recall priority set to Normal from the ASAP List.");
			}
			FillGridRecalls();
		}

		private void MakeAppointment_Click() {
			if(!Security.IsAuthorized(Permissions.AppointmentCreate) || gridRecalls.SelectedIndices.Length==0) {
				return;
			}
			List<Recall> listRecallsSelected=gridRecalls.SelectedIndices.Select(x => (Recall)gridRecalls.ListGridRows[x].Tag).ToList();
			for(int i=0;i<listRecallsSelected.Count;i++) {
				Patient patient=Patients.GetPat(listRecallsSelected[i].PatNum);
				Patient patientMerged=AppointmentL.GetPatientMergePrompt(patient.PatNum);
				if(patientMerged!=null) {
					patient=patientMerged;
				}
				if(ListTools.In(patient.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
					MsgBox.Show(this,"Appointments cannot be scheduled for "+patient.PatStatus.ToString().ToLower()+" patients.");
					return;
				}
				Family family=Patients.GetFamily(patient.PatNum);
				List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
				List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				Appointment appointment=AppointmentL.CreateRecallApt(patient,listInsPlans,listRecallsSelected[i].RecallNum,listInsSubs);
				GotoModule.PinToAppt(new List<long>() { appointment.AptNum },patient.PatNum);
			}
			FillGridRecalls();
		}
		#endregion Recalls Tab
		
		private void gridMenuRight_click(object sender,System.EventArgs e) {
			ToolStripMenuItem toolStripMenuItem=(ToolStripMenuItem)sender;
			if(tabControl.SelectedIndex==0) {
				if(gridAppts.SelectedIndices.Length<=0) {
					return;
				}
				switch(menuApptsRightClick.Items.IndexOf(toolStripMenuItem)) {
					case 0:
						SelectPatient_Click(((Appointment)gridAppts.SelectedGridRows.Last().Tag).PatNum);
						break;
					case 1:
						if(gridAppts.SelectedIndices.Length==0) {
							MsgBox.Show(this,"Please select an appointment first.");
							return;
						}
						SeeChart_Click(((Appointment)gridAppts.SelectedGridRows.Last().Tag).PatNum);
						break;
					case 2:
						SendPinboard_Click();
						break;
					case 3:
						RemoveAppts_Click();
						break;
				}
			}
			else {//Recall tab selected
				if(gridRecalls.SelectedIndices.Length<=0) {
					return;
				}
				Recall recall=gridRecalls.SelectedIndices.Select(x => (Recall)gridRecalls.ListGridRows[x].Tag).ToList().FirstOrDefault();
				switch(menuRecallsRightClick.Items.IndexOf(toolStripMenuItem)) {
					case 0:
						SelectPatient_Click(recall.PatNum);
						break;
					case 1:
						if(gridRecalls.SelectedIndices.Length==0) {
							MsgBox.Show(this,"Please select a recall first first.");
							return;
						}
						SeeChart_Click(recall.PatNum);
						break;
					case 2:
						MakeAppointment_Click();
						break;
					case 3:
						RemoveRecalls_Click();
						break;
				}
			}
		}

		private void SelectPatient_Click(long patnum) {
			Patient patient=Patients.GetPat(patnum);//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule: true);
		}

		///<summary>If multiple patients are selected in the list, it will use the last patient to show the chart for.</summary>
		private void SeeChart_Click(long patnum) {
			Patient patient=Patients.GetPat(patnum); //If multiple selected, just use the last one.
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule: false); //Selects the patient in OpenDental.
			GotoModule.GotoChart(patient.PatNum);
		}

		private void butText_Click(object sender,EventArgs e) {
			Clinic clinic=Clinics.GetClinic(Clinics.ClinicNum)??Clinics.GetDefaultForTexting()??Clinics.GetPracticeAsClinicZero();
			List<PatComm> listPatCommsToSend;
			if(tabControl.SelectedIndex==0) {//Appt Tab selected
				Func<int,long> getPatNumFromGridRow=new Func<int,long>((rowIdx) =>	{
					return ((Appointment)gridAppts.ListGridRows[rowIdx].Tag).PatNum;
				});
				listPatCommsToSend=GetPatCommsForTexting(clinic,gridAppts,_listAppointmentsASAP.Select(x => x.PatNum).ToList(),getPatNumFromGridRow);
			}
			else {//Recall tab selected
				List<Recall> listRecalls=new List<Recall>();
				for(int i=0;i<gridRecalls.ListGridRows.Count;i++) {
					listRecalls.Add((Recall)gridRecalls.ListGridRows[i].Tag);
				}
				Func<int,long> getPatNumFromGridRow=new Func<int,long>((rowIdx) =>	{
					return ((Recall)gridRecalls.ListGridRows[rowIdx].Tag).PatNum;
				});
				listPatCommsToSend=GetPatCommsForTexting(clinic,gridRecalls,listRecalls.Select(x => x.PatNum).ToList(),getPatNumFromGridRow);
			}
			if(listPatCommsToSend.Count==0) {
				return;
			}
			string textTemplate=GetTextMessageText(clinic);
			using FormTxtMsgMany formTxtMsgMany=new FormTxtMsgMany(listPatCommsToSend,textTemplate,clinic.ClinicNum,SmsMessageSource.AsapManual);
			formTxtMsgMany.ShowDialog();
		}

		private List<PatComm> GetPatCommsForTexting(Clinic clinic,GridOD grid,List<long> listPatNumsInGrid,Func<int,long> getPatNumFromGridRow) {
			List<PatComm> listPatComms=Patients.GetPatComms(listPatNumsInGrid,clinic,false);
			List<PatComm> listPatCommsToSend=new List<PatComm>();
			if(grid.SelectedIndices.Length==0) {//None selected. Select all that can be texted.
				for(int i=0;i<listPatNumsInGrid.Count;i++) {
					PatComm patCommForAppt=listPatComms.FirstOrDefault(x => x.PatNum==listPatNumsInGrid[i]);
					if(patCommForAppt==null || !patCommForAppt.IsSmsAnOption) {
						continue;
					}
					grid.SetSelected(i,true);
				}
				if(grid.SelectedIndices.Length==0) {
					MsgBox.Show(this,"None of the patients in the list are able to receive text messages.");
					return listPatCommsToSend;
				}
			}
			//deselect the ones that are not okay to send texts to.
			List<string> listPatsSkipped=new List<string>();
			int numRowsSelected=grid.SelectedIndices.Length;
			for(int i=grid.SelectedIndices.Length-1;i>=0;i--) {//Backwards since we are deselecting rows.
				PatComm patCommForAppt=listPatComms.FirstOrDefault(x => x.PatNum==getPatNumFromGridRow(grid.SelectedIndices[i]));
				if(patCommForAppt!=null && patCommForAppt.IsSmsAnOption) {
					listPatCommsToSend.Add(patCommForAppt);
					continue;
				}
				//Cannot send text message to this patient
				if(patCommForAppt==null) {//Shouldn't happen
					listPatsSkipped.Add(Lan.g(this,"Unknown patient")+": "+Lan.g(this,"Cannot find contact info"));
				}
				else {
					listPatsSkipped.Add(patCommForAppt.FName+" "+patCommForAppt.LName+": "+Lan.g(this,patCommForAppt.GetReasonCantText()));
				}
				grid.SetSelected(grid.SelectedIndices[i],false);
			}
			if(listPatsSkipped.Count > 0) {
				MessageBox.Show(listPatsSkipped.Count+" "+Lan.g(this,"of the")+" "+numRowsSelected+" "
					+Lan.g(this,"selected patients cannot receive text messages and have been deselected:")+"\r\n"+string.Join("\r\n",listPatsSkipped));
			}
			return listPatCommsToSend;
		}		

		///<summary>Gets the template for this clinic and fills in the tags.</summary>
		private string GetTextMessageText(Clinic curClinic) {
			string textTemplate;
			ClinicPref clinicPref=ClinicPrefs.GetPref(PrefName.ASAPTextTemplate,Clinics.ClinicNum);
			if(clinicPref==null) {
				textTemplate=PrefC.GetString(PrefName.ASAPTextTemplate);
			}
			else {
				textTemplate=clinicPref.ValueString;
			}
			textTemplate=textTemplate.Replace("[OfficeName]",Clinics.GetOfficeName(curClinic));
			textTemplate=textTemplate.Replace("[OfficePhone]",TelephoneNumbers.ReFormat(Clinics.GetOfficePhone(curClinic)));
			if(DateTimeChosen.Year > 1880) {//The user clicked on the appt schedule to get here.
				textTemplate=textTemplate.Replace("[Date]",DateTimeChosen.ToString(PrefC.PatientCommunicationDateFormat));
				textTemplate=textTemplate.Replace("[Time]",DateTimeChosen.ToShortTimeString());
				return textTemplate;
			}
			//Need to use an input box to specify the date and time.
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			if(textTemplate.Contains("[Date]")) {
				listInputBoxParams.Add(new InputBoxParam(InputBoxType.ValidDate,"Enter date for available time"));
			}
			if(textTemplate.Contains("[Time]")) {
				listInputBoxParams.Add(new InputBoxParam(InputBoxType.ValidTime,"Enter time for available time"));
			}
			using InputBox inputBox=new InputBox(listInputBoxParams);
			if(listInputBoxParams.Count > 0 && inputBox.ShowDialog()==DialogResult.OK) {
				if(inputBox.DateEntered.Year > 1880) {
					textTemplate=textTemplate.Replace("[Date]",inputBox.DateEntered.ToString(PrefC.PatientCommunicationDateFormat));
				}
				if(textTemplate.Contains("[Time]") && inputBox.ListTextEntered[listInputBoxParams.Count-1]!="") {//A time was entered.
					textTemplate=textTemplate.Replace("[Time]",inputBox.TimeEntered.ToShortTimeString());
				}
			}
			return textTemplate;
		}

		#region Web Sched

		///<summary>Use this instead of Show() or ShowDialog() in order to send Web Sched messages.</summary>
		public void ShowFormForWebSched(DateTime dateTimeChosen,DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd,long opNum,int maxApptLengthFilter=0) {
			DateTimeChosen=DateTimeOD.GetDateTimeHourAndMins(dateTimeChosen);
			//Truncate the seconds off of the start and end times passed in.
			DateTimeSlotStart=DateTimeOD.GetDateTimeHourAndMins(dateTimeSlotStart);
			DateTimeSlotEnd=DateTimeOD.GetDateTimeHourAndMins(dateTimeSlotEnd);
			_maxApptLengthFilter=maxApptLengthFilter;
			_opNum=opNum;
			Show();
			if(WindowState==FormWindowState.Minimized) {
				WindowState=FormWindowState.Normal;
			}
			BringToFront();
			comboClinic.SelectedClinicNum=ODMethodsT.Coalesce(Operatories.GetOperatory(_opNum)).ClinicNum;
			FillGridAppts();
			FillTimeCombos();
			SetSelectedAppts();
			SetSelectedRecalls();
		}

		private void FillForWebSched() {
			if(DateTimeChosen.Date < DateTime.Today) {
				labelOperatory.Text=Lan.g(this,"Cannot send for a past time slot.");
				labelOperatory.Visible=true;
				if(IsSendingWebSched()) {
					_opNum=0;
				}
				return;
			}
			comboClinic.SelectedClinicNum=ODMethodsT.Coalesce(Operatories.GetOperatory(_opNum)).ClinicNum;
			comboClinic.Enabled=false;//We only want them to choose appointments from the clinic of the operatory selected.
			labelOperatory.Text=Lan.g(this,"Operatory:")+" "+Operatories.GetOperatory(_opNum).Abbrev;
			splitContainer.Panel2Collapsed=false;
			labelOperatory.Visible=true;
			labelStart.Visible=true;
			comboStart.Visible=true;
			labelEnd.Visible=true;
			comboEnd.Visible=true;
		}

		private void FillTimeCombos() {
			if(!IsSendingWebSched()) {
				return;
			}
			int timeIncrement=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			comboStart.Items.Clear();
			for(DateTime time=DateTimeSlotStart;time<DateTimeSlotEnd;time=time.AddMinutes(timeIncrement)) {
				int addedIdx=comboStart.Items.Add(time.ToShortTimeString(),time);
				if(time==DateTimeChosen) {
					comboStart.SelectedIndex=addedIdx;
				}
			}
			FillComboEnd();
		}

		private void FillComboEnd() {
			int timeIncrement=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			DateTime dateTimeSelectedEnd=GetDateTimeSelectedEnd();
			comboEnd.Items.Clear();
			DateTime dateTimeSelectedStart=ODMathLib.Max(GetDateTimeSelectedStart(),DateTime.Today).AddMinutes(timeIncrement);
			for(DateTime dateTime=dateTimeSelectedStart;dateTime<=DateTimeSlotEnd;dateTime=dateTime.AddMinutes(timeIncrement)) {
				int idx=comboEnd.Items.Add(dateTime.ToShortTimeString(),dateTime);
				if(dateTimeSelectedEnd==dateTime) {
					comboEnd.SelectedIndex=idx;
				}
			}
			if(comboEnd.SelectedIndex==-1) {
				//Put the end time one hour after the start time or the last available time.
				int idx1HourAfterStart=60/timeIncrement-1;
				comboEnd.SelectedIndex=Math.Min(idx1HourAfterStart,comboEnd.Items.Count-1);
			}
		}

		private void comboStart_SelectedIndexChanged(object sender,EventArgs e) {
			FillComboEnd();
			SetSelectedAppts();
			SetSelectedRecalls();
		}

		private void comboEnd_SelectionChangeCommitted(object sender,EventArgs e) {
			SetSelectedAppts();
			SetSelectedRecalls();
		}

		private void FillWebSchedSent() {
			List<Appointment> listAppointmentsSelected=gridAppts.SelectedIndices.Select(x => (Appointment)gridAppts.ListGridRows[x].Tag).ToList();
			List<Recall> listRecallsSelected=gridRecalls.SelectedIndices.Select(x => (Recall)gridRecalls.ListGridRows[x].Tag).ToList();
			List<long> listPatNumsSelected=listAppointmentsSelected.Select(x => x.PatNum).Distinct().ToList();
			listPatNumsSelected.AddRange(listRecallsSelected.Select(x => x.PatNum));
			List<long> listPatNums=_listPatientNames.Select(x=>x.PatNum).ToList();
			if(listPatNumsSelected.Any(x => !listPatNums.Contains(x))) {
				_listPatientNames=Patients.GetPatientNameList(listPatNumsSelected).ToList();
			}
			Func<AsapComms.AsapCommHist,DateTime> funcOrderBy=new Func<AsapComms.AsapCommHist,DateTime>(x => {
				DateTime dateTimeSmsSent=DateTime.MinValue;
				if(x.AsapComm.SmsSendStatus==AutoCommStatus.SendSuccessful) {
					dateTimeSmsSent=x.AsapComm.DateTimeSmsSent;
				}
				else if(x.AsapComm.SmsSendStatus==AutoCommStatus.SendNotAttempted) {
					dateTimeSmsSent=x.AsapComm.DateTimeSmsScheduled;
				}
				return dateTimeSmsSent;
			});
			List<AsapComms.AsapCommHist> listAsapHists=AsapComms.GetHist(DateTime.Today.AddMonths(-1),DateTime.Today,listPatNumsSelected)
				.OrderByDescending(funcOrderBy)
				.ThenBy(x=>x.PatientName).ToList();
			gridWebSched.BeginUpdate();
			gridWebSched.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),150);
			gridWebSched.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Text Send Time"),120);
			gridWebSched.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Email Send Time"),120);
			gridWebSched.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time Slot Start"),120);
			gridWebSched.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),300);
			gridWebSched.Columns.Add(col);
			gridWebSched.ListGridRows.Clear();
			for(int i=0;i<listAsapHists.Count;i++) {
				GridRow row=new GridRow();
				Patients.PatientName patientName=_listPatientNames.Find(x => x.PatNum==listAsapHists[i].AsapComm.PatNum);
				row.Cells.Add(patientName.Name);
				string smsSent;
				if(listAsapHists[i].AsapComm.SmsSendStatus==AutoCommStatus.SendSuccessful) {
					smsSent=listAsapHists[i].AsapComm.DateTimeSmsSent.ToString();
				}
				else if(listAsapHists[i].AsapComm.SmsSendStatus==AutoCommStatus.SendNotAttempted) {
					smsSent=listAsapHists[i].AsapComm.DateTimeSmsScheduled.ToString();
				}
				else {
					smsSent="";
				}
				row.Cells.Add(smsSent);
				string emailSent;
				if(listAsapHists[i].AsapComm.EmailSendStatus==AutoCommStatus.SendSuccessful) {
					emailSent=listAsapHists[i].AsapComm.DateTimeEmailSent.ToString();
				}
				else {
					emailSent="";
				}
				row.Cells.Add(emailSent);
				row.Cells.Add(listAsapHists[i].DateTimeSlotStart.ToString());
				row.Cells.Add(listAsapHists[i].AsapComm.Note);
				row.Tag=listAsapHists[i];
				gridWebSched.ListGridRows.Add(row);
			}
			gridWebSched.EndUpdate();
		}

		private void SetSelectedAppts() {
			if(!IsSendingWebSched() || tabControl.SelectedIndex!=0) {//Recall tab is selected.
				return;
			}
			int slotLength=(int)(GetDateTimeSelectedEnd()-GetDateTimeSelectedStart()).TotalMinutes;
			for(int i=0;i<gridAppts.ListGridRows.Count;i++) {
				if(((Appointment)gridAppts.ListGridRows[i].Tag).Length<=slotLength) {
					gridAppts.SetSelected(i,true);
				}
				else {
					gridAppts.SetSelected(i,false);
				}
			}
		}

		private void SetSelectedRecalls() {
			if(!IsSendingWebSched() || tabControl.SelectedIndex!=1) {//Appt tab is selected.
				return;
			}
			int timeIncrements=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			int slotLength=(int)(GetDateTimeSelectedEnd()-GetDateTimeSelectedStart()).TotalMinutes;
			for(int i=0;i<gridRecalls.ListGridRows.Count;i++) {
				Recall recall=gridRecalls.ListGridRows[i].Tag as Recall;
				string timePattern=RecallTypes.GetTimePattern(recall.RecallTypeNum);
				int length=timePattern.Length*timeIncrements;
				if(length<=slotLength) {
					gridRecalls.SetSelected(i,true);
				}
				else {
					gridRecalls.SetSelected(i,false);
				}
			}
		}

		private void CheckClinicsSignedUpForWebSched() {
			if(_threadWebSchedSignups!=null) {
				return;
			}
			_threadWebSchedSignups=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				_listClinicNumsWebSched=WebServiceMainHQProxy.GetEServiceClinicsAllowed(
					Clinics.GetDeepCopy().Select(x => x.ClinicNum).ToList(),
					eServiceCode.WebSchedASAP);
				bool isAllowedByHq=(_listClinicNumsWebSched.Count > 0);
				if(isAllowedByHq!=PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
					Prefs.UpdateBool(PrefName.WebSchedAsapEnabled,isAllowedByHq);
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}));
			//Swallow all exceptions and allow thread to exit gracefully.
			_threadWebSchedSignups.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { }));
			_threadWebSchedSignups.AddExitHandler(new ODThread.WorkerDelegate((ODThread o) => {
				try {
					ThreadWebSchedSignupsExitHandler();
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}));
			_threadWebSchedSignups.Name="CheckWebSchedSignups";
			_threadWebSchedSignups.Start(true);
		}

		private void ThreadWebSchedSignupsExitHandler() {
			if(IsDisposed) {
				return;
			}
			if(InvokeRequired) {
				Invoke((Action)(() => { ThreadWebSchedSignupsExitHandler(); }));
				return;
			}
			_threadWebSchedSignups=null;
			Cursor=Cursors.Default;
			if(_hasClickedWebSched) {
				try {
					SendWebSched();
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error sending Web Sched messages."),ex);
				}
			}
			_hasClickedWebSched=false;
		}

		///<summary>Automatically open the eService Setup window so that they can easily click the Enable button. 
		///Calls CheckClinicsSignedUpForWebSched() before exiting.</summary>
		private void OpenSignupPortal() {
			using FormEServicesSignup formEServicesSignup=new FormEServicesSignup();
			formEServicesSignup.ShowDialog();
			//User may have made changes to signups. Reload the valid clinics from HQ.
			CheckClinicsSignedUpForWebSched();
		}

		private void butWebSchedNotify_Click(object sender,EventArgs e) {
			using FormEServicesWebSchedNotify formEServicesWebSchedNotify=new FormEServicesWebSchedNotify(WebSchedNotifyType.ASAP);
			formEServicesWebSchedNotify.ShowDialog();
		}

		private void butWebSched_Click(object sender,EventArgs e) {
			SendWebSched();
		}

		private void SendWebSched() {
			if(IsDisposed) {//The user closed the form while the thread checking Web Sched signups was still running.
				return;
			}
			if(_threadWebSchedSignups!=null) {//The thread checking clinics that are signed up for Web Sched has not finished getting the list.
				_hasClickedWebSched=true;//The thread checking Web Sched signups will call this method on exit.
				Cursor=Cursors.AppStarting;
				return;
			}
			if(_listClinicNumsWebSched.Count==0) {//No clinics are signed up for Web Sched
				string message="This practice is not signed up for Web Sched ASAP. Open Sign Up Portal?";
				if(PrefC.HasClinicsEnabled) {
					message="No clinics are signed up for Web Sched ASAP. Open Sign Up Portal?";
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
					return;
				}
				OpenSignupPortal();
				return;
			}
			//At least one clinic is signed up for Web Sched.
			if(!IsSendingWebSched()) {//Did not get to this window by right-clicking in the Appointment module.
				MsgBox.Show(this,
					"To use this feature, right-click on the appointment schedule where no appointment is scheduled and select \"Text ASAP List\".");
				return;
			}
			if(gridAppts.SelectedIndices.Length==0 && gridRecalls.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one patient from the appointment or recall list.");
				return;
			}
			if(comboStart.SelectedIndex==-1|| comboEnd.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a valid start and end time.");
				return;
			}
			long clinicNum=ODMethodsT.Coalesce(Operatories.GetOperatory(_opNum)).ClinicNum;
			if(!PrefC.HasClinicsEnabled) {
				clinicNum=0;
			}
			bool isSignedUp=(ListTools.In(clinicNum,_listClinicNumsWebSched) || (!PrefC.HasClinicsEnabled && _listClinicNumsWebSched.Count > 0));
			if(!isSignedUp) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,
					"The clinic the selected operatory belongs to is not signed up for Web Sched ASAP. Open Sign Up Portal?")) 
				{
					return;
				}
				OpenSignupPortal();
				return;
			}
			List<Appointment> listAppointments=new List<Appointment>();
			List<Recall> listRecalls=new List<Recall>();
			if(tabControl.SelectedIndex==0) {
				listAppointments=gridAppts.SelectedIndices.Select(x => (Appointment)gridAppts.ListGridRows[x].Tag).ToList();
			}
			else {
				listRecalls=gridRecalls.SelectedIndices.Select(x => (Recall)gridRecalls.ListGridRows[x].Tag).ToList();
			}
			using FormWebSchedASAPSend formWebSchedASAPSend=new FormWebSchedASAPSend(clinicNum,_opNum,GetDateTimeSelectedStart(),GetDateTimeSelectedEnd(),listAppointments,listRecalls);
			formWebSchedASAPSend.ShowDialog();
		}

		private void butWebSchedHist_Click(object sender,EventArgs e) {
			if(_formWebSchedASAPHistory==null || _formWebSchedASAPHistory.IsDisposed) {
				_formWebSchedASAPHistory=new FormWebSchedASAPHistory();
			}
			_formWebSchedASAPHistory.Show();
			if(_formWebSchedASAPHistory.WindowState==FormWindowState.Minimized) {
				_formWebSchedASAPHistory.WindowState=FormWindowState.Normal;
			}
			_formWebSchedASAPHistory.BringToFront();
		}
		#endregion Web Sched

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridAppts();
			FillGridRecalls();
		}

		private void menuItemSettings_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormAsapSetup formAsapSetup=new FormAsapSetup();
			formAsapSetup.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"ASAP List Setup");
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;	
			_doPrintHeading=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"ASAP list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font headingFont=new Font("Arial",13,FontStyle.Bold);
			using Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int y=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			int headingPrintH=0;
			if(!_doPrintHeading) {
				if(tabControl.SelectedIndex==0) {
					text=Lan.g(this,"ASAP Appointment List");
				}
				else {
					text=Lan.g(this,"ASAP Recall List");
				}
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,y);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				y+=25;
				_doPrintHeading=true;
				headingPrintH=y;
			}
			#endregion
			if(tabControl.SelectedIndex==0) {
				y=gridAppts.PrintPage(g,_pagesPrinted,bounds,headingPrintH);
			}
			else {
				y=gridRecalls.PrintPage(g,_pagesPrinted,bounds,headingPrintH);
			}
			_pagesPrinted++;
			if(y==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void comboShowHygiene_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridAppts();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	
	}
}
