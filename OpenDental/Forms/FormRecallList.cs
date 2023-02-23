using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRecallList:FormODBase {
		private int pagesPrinted;
		private DataTable _tableAddress;
		private int patientsPrinted;
		private DataTable _tableRecalls;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		///<summary>The clinics that are signed up for Web Sched.</summary>
		private List<long> _listClinicNumsWebSched=new List<long>();
		private ODThread _threadWebSchedSignups=null;
		///<summary>Indicates whether the Reg Key is currently on Open Dental support.</summary>
		private YN _isOnSupport=YN.Unknown;
		///<summary>The user has clicked the Web Sched button while a thread was busy checking which clinics are signed up for Web Sched.</summary>
		private bool _hasClickedWebSched;
		///<summary>Short list, for the two combo boxes.</summary>
		private List<Provider> _listProviders;
		///<summary>Default starting offset from Date Since for Date Stop.</summary>
		private const int _reactDateStopOffset=-36;

		///<summary>Each tab should have an ODGrid set as the tag, returns the grid attached to the currently selected tab.</summary>
		private GridOD _grid { get { return (GridOD)tabControl.SelectedTab.Tag; } }

		///<summary>Returns the PatNum of the first selected row in the corresponding grid.  Can return 0.</summary>
		private long _patNumCur {
			get {
				if(_grid==gridReminders){
					if(_grid.SelectedTag<Recalls.RecallRecent>()!=null){
						return _grid.SelectedTag<Recalls.RecallRecent>().PatNum;
					}
				}
				else if(_grid.SelectedTag<PatRowTag>()!=null){
					return _grid.SelectedTag<PatRowTag>().PatNum;
				}
				return 0;
			}
		}

		private bool IsRecallGridSelected() {
			return _grid==gridRecalls;
		}

		private bool IsReactivationGridSelected() {
			return _grid==gridReactivations;
		}

		private bool IsReminderGridSelected() {
			return _grid==gridReminders;
		}

		private bool DoGroupFamilies() {
			return (IsRecallGridSelected() && checkGroupFamiliesRecalls.Checked) || (IsReactivationGridSelected() && checkGroupFamiliesReact.Checked);
		}
		
		///<summary></summary>
		public FormRecallList(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			gridRecalls.ContextMenu=menuRightClick;
			gridReminders.ContextMenu=menuRightClick;
			gridReactivations.ContextMenu=menuRightClick;
			if(!PrefC.GetBool(PrefName.ShowFeatureReactivations)) {
				tabControl.Controls.Remove(tabPageReactivations);
			}
			Lan.F(this);
		}

		private void FormRecallList_Load(object sender, System.EventArgs e) {
			CheckClinicsSignedUpForWebSched();
			if(ODBuild.IsDebug()) {
				butECards.Visible=true;
			}
			checkGroupFamiliesRecalls.Checked=PrefC.GetBool(PrefName.RecallGroupByFamily);
			checkGroupFamiliesReact.Checked=PrefC.GetBool(PrefName.ReactivationGroupByFamily);
			#region Fill Sort Types
			comboSortRecalls.Items.AddEnums<RecallListSort>();
			comboSortRecalls.SelectedIndex=0;
			comboSortReact.Items.AddEnums<ReactivationListSort>();
			comboSortReact.SelectedIndex=0;
			#endregion Fill Sort Types
			#region Fill Number Reminders and Reactivations
			comboShowReminders.IncludeAll=true;
			comboShowReactivate.IncludeAll=true;
			int maxReactivationNums=PrefC.GetInt(PrefName.ReactivationCountContactMax);
			for(int i=0;i<=6;i++) {
				if(i==6) {
					comboShowReminders.Items.Add("6+",(RecallListShowNumberReminders)(i+1));
				}
				else{
					comboShowReminders.Items.Add(i.ToString(),(RecallListShowNumberReminders)(i+1));
				}
				if(maxReactivationNums>-1 && maxReactivationNums<(i+1)) {
					continue; //pref doesn't allow us more contacts than this anyway
				}
				if(i==6) {
					comboShowReactivate.Items.Add("6+",(RecallListShowNumberReminders)(i+1));
					continue;
				}
				comboShowReactivate.Items.Add(i.ToString(),(RecallListShowNumberReminders)(i+1));
				
			}
			comboShowReminders.IsAllSelected=true;
			comboShowReactivate.IsAllSelected=true;
			#endregion Fill Number Reminders and Reactivations
			#region Set Recall Dates
			int daysPast=PrefC.GetInt(PrefName.RecallDaysPast);
			int daysFuture=PrefC.GetInt(PrefName.RecallDaysFuture);
			//since this is the selected tab on load the datePicker load event will fire before this load event we need to overwrite the defaults
			if(daysPast==-1) {
				datePickerRecalls.SetDateTimeFrom(DateTime.MinValue);
			}
			else {
				datePickerRecalls.SetDateTimeFrom(DateTime.Today.AddDays(-daysPast));
			}
			if(daysFuture==-1) {
				datePickerRecalls.SetDateTimeTo(DateTime.MinValue);
			}
			else {
				datePickerRecalls.SetDateTimeTo(DateTime.Today.AddDays(daysFuture));
			}
			#endregion Set Recall Dates
			#region Set Reactivation Dates
			int daysSince=PrefC.GetInt(PrefName.ReactivationDaysPast);
			//set the default, datePicker load event will fire when the tab is selected and fill from and to dates with the defaults
			if(daysSince==-1) {
				datePickerReact.DefaultDateTimeFrom=DateTime.MinValue;
				datePickerReact.DefaultDateTimeTo=DateTime.MinValue;
			}
			else {
				datePickerReact.DefaultDateTimeFrom=DateTime.Today.AddDays(-daysSince).AddMonths(_reactDateStopOffset);
				datePickerReact.DefaultDateTimeTo=DateTime.Today.AddDays(-daysSince);
			}
			#endregion Set Reactivation Dates
			#region Set Reminder Dates
			//set the default, datePicker load event will fire when the tab is selected and fill from and to dates with the defaults
			datePickerRemind.DefaultDateTimeFrom=DateTime.Today.AddMonths(-1);
			datePickerRemind.DefaultDateTimeTo=DateTime.Today;
			#endregion Set Reminder Dates
			#region Providers
			_listProviders=Providers.GetDeepCopy(isShort:true);
			comboProviderRecalls.IncludeAll=true;
			comboProviderRecalls.Items.AddList(_listProviders,x => x.GetLongDesc());
			comboProviderRecalls.IsAllSelected=true;
			comboProviderReact.IncludeAll=true;
			comboProviderReact.Items.AddList(_listProviders,x => x.GetLongDesc());
			comboProviderReact.IsAllSelected=true;
			#endregion Providers
			if(PrefC.GetBool(PrefName.EnterpriseApptList)){
				comboClinicRecalls.IncludeAll=false;
			}
			#region Sites
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				labelSiteRecalls.Visible=false;
				comboSiteRecalls.Visible=false;
				labelSiteReact.Visible=false;
				comboSiteReact.Visible=false;
			}
			else {
				List<Site> listSites=Sites.GetDeepCopy();
				comboSiteRecalls.IncludeAll=true;
				comboSiteRecalls.Items.AddList(listSites,x => x.Description);
				comboSiteRecalls.IsAllSelected=true;
				comboSiteReact.IncludeAll=true;
				comboSiteReact.Items.AddList(listSites,x => x.Description);
				comboSiteReact.IsAllSelected=true;
			}
			#endregion Sites
			#region Unscheduled Statuses
			List<Def> listDefsRecallUnschedStatus=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			comboSetStatusRecalls.Items.AddDefNone();
			comboSetStatusRecalls.Items.AddDefs(listDefsRecallUnschedStatus);
			comboSetStatusReact.Items.AddDefNone();
			comboSetStatusReact.Items.AddDefs(listDefsRecallUnschedStatus);
			#endregion Unscheduled Statuses
			#region Billing Types
			comboBillingTypes.IncludeAll=true;
			comboBillingTypes.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes));
			comboBillingTypes.IsAllSelected=true;
			#endregion Billing Types
			#region Recall Types
			comboRecallTypes.IncludeAll=true;
			string recallTypesShowingPref=PrefC.GetString(PrefName.RecallTypesShowingInList);
			List<RecallType> listRecallTypes=null;
			if(string.IsNullOrEmpty(recallTypesShowingPref)) {
				listRecallTypes=RecallTypes.GetDeepCopy();
			}
			else {
				listRecallTypes=RecallTypes.GetWhere(x => recallTypesShowingPref.Split(',').Contains(x.RecallTypeNum.ToString()));
			}
			comboRecallTypes.Items.AddList(listRecallTypes,x => x.Description);
			comboRecallTypes.IsAllSelected=true;
			#endregion Recall Types
			FillComboEmail();
		}
		///<summary>Due to a bug in ODProgress.cs load methods that spawn progress bars must be moved into the Shown() event to prevent the form from popping up behind the main program. (this seems like an old irrelevant note)</summary>
		private void FormRecallList_Shown(object sender,EventArgs e) {
			FillRecalls();
			Plugins.HookAddCode(this,"FormRecallList.Load_End",_tableRecalls);
		}

		/// <summary> Gets the main recall table list </summary>
		private DataTable GetRecallTable(){//long clinicNum, long siteNum, long maxReminders){
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && !comboSiteRecalls.IsAllSelected) {
				siteNum=comboSiteRecalls.GetSelectedKey<Site>(x => x.SiteNum);
			}
			long clinicNum=PrefC.HasClinicsEnabled?comboClinicRecalls.SelectedClinicNum:-1;
			long maxReminders=PrefC.GetLong(PrefName.RecallMaxNumberReminders);
			//If dateTimeTo is blank, default to DateTime.MaxValue.
			return Recalls.GetRecallList(datePickerRecalls.GetDateTimeFrom(),datePickerRecalls.GetDateTimeTo(true),checkGroupFamiliesRecalls.Checked,
				comboProviderRecalls.GetSelectedProvNum(),clinicNum,siteNum,comboSortRecalls.GetSelected<RecallListSort>(),
				comboShowReminders.GetSelected<RecallListShowNumberReminders>(),maxReminders,doShowReminded:checkIncludeReminded.Checked,
				listRecallTypes:comboRecallTypes.GetListSelected<RecallType>());
		}

		private void FillCurGrid(){
			if(IsRecallGridSelected()){
				FillRecalls();
				return;
			}
			if(IsReminderGridSelected()){
				FillGridReminders();
				return;
			}
			if(IsReactivationGridSelected()){
				FillReactivationGrid();
			}
		}

		private void CheckClinicsSignedUpForWebSched() {
			if(_threadWebSchedSignups!=null) {
				return;
			}
			ODThread.WorkerDelegate workerDelegate=new ODThread.WorkerDelegate((ODThread o) => {
				_listClinicNumsWebSched=WebServiceMainHQProxy.GetEServiceClinicsAllowed(
					Clinics.GetDeepCopy().Select(x => x.ClinicNum).ToList(),
					eServiceCode.WebSched);
				_isOnSupport=YN.Yes;
			});
			_threadWebSchedSignups=new ODThread(workerDelegate);
			//Swallow all exceptions and allow thread to exit gracefully.
			_threadWebSchedSignups.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => {}));
			_threadWebSchedSignups.AddExitHandler(new ODThread.WorkerDelegate((ODThread o) => {
				ThreadWebSchedSignupsExitHandler();
			}));
			_threadWebSchedSignups.Name="CheckWebSchedSignups";
			_threadWebSchedSignups.Start(true);
		}

		private void ThreadWebSchedSignupsExitHandler() { 
			if(IsDisposed) {
				return;
			}
			if(InvokeRequired) {
				Action action=new Action(() =>{ThreadWebSchedSignupsExitHandler(); });
				Invoke(action);
				return;
			}
			_threadWebSchedSignups=null;
			Cursor=Cursors.Default;
			if(!_hasClickedWebSched){
				return;
			}
			try {
				SendWebSched();
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error sending Web Sched notifications. Error message:")+" "+ex.Message);
			}
			_hasClickedWebSched=false;
		}

		private void FillComboEmail() {
			comboEmailFromReact.Items.Clear();
			comboEmailFromRecalls.Items.Clear();
			long curUserNum=Security.CurUser.UserNum;
			//Excludes any email addresses that are associated to a clinic or are the default practice and are not associated with the current user.
			List<EmailAddress> listEmailAdresses=EmailAddresses.GetEmailAddressesForComboBoxes(curUserNum);
			comboEmailFromReact.Items.AddList(listEmailAdresses,x=>EmailAddresses.GetDisplayStringForComboBox(x,curUserNum));
			comboEmailFromRecalls.Items.AddList(listEmailAdresses,x=>EmailAddresses.GetDisplayStringForComboBox(x,curUserNum));
			comboEmailFromReact.SetSelected(0);
			comboEmailFromRecalls.SetSelected(0);
		}

		///<summary>Shows a progress window and fills the main Recall List grid. Goes to database.</summary>
		private void FillRecalls() {
			//Verification
			if(!datePickerRecalls.IsValid()) {
				return;
			}
			//remember which recallnums were selected
			List<PatRowTag> listPatRowTagsSelected=gridRecalls.SelectedTags<PatRowTag>();
			Cursor=Cursors.WaitCursor;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => _tableRecalls=GetRecallTable();
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			gridRecalls.BeginUpdate();
			gridRecalls.Columns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.RecallList);
			GridColumn col;
			for(int i=0;i<listDisplayFields.Count;i++) {
				col=new GridColumn(string.IsNullOrEmpty(listDisplayFields[i].Description)?listDisplayFields[i].InternalName:listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth) { Tag=listDisplayFields[i].InternalName };
				if(listDisplayFields[i].InternalName.In("Due Date","LastRemind")) {
					col.SortingStrategy=GridSortingStrategy.DateParse;
					col.TextAlign=HorizontalAlignment.Center;
				}
				else if(listDisplayFields[i].InternalName.In("Age","#Remind")) {
					col.SortingStrategy=GridSortingStrategy.AmountParse;
					col.TextAlign=HorizontalAlignment.Right;
				}
				gridRecalls.Columns.Add(col);
			}
			gridRecalls.ListGridRows.Clear();
			GridRow row;
			List<long> listConflictingPatNums=new List<long>();
			if(checkShowConflictingTypes.Checked) {
				listConflictingPatNums=Recalls.GetConflictingPatNums(_tableRecalls.Rows.OfType<DataRow>().Select(x => PIn.Long(x["PatNum"].ToString())).ToList());
			}
			for(int i=0;i<_tableRecalls.Rows.Count;i++) {
				long patNum=PIn.Long(_tableRecalls.Rows[i]["PatNum"].ToString());
				if(checkShowConflictingTypes.Checked) {
					//If the RecallType checkbox is checked, show patients with future scheduled appointments that have conflicting recall appointments.
					//Ex. A patient is scheduled for a perio recall but their recall type is set to prophy
					if(!listConflictingPatNums.Contains(patNum)) {
						//The patient does not have any conflicting recall type.
						//Continue since we don't want to show them when the RecallTypes checkbox is checked. 
						continue;
					}
					long recallTypeNum=PIn.Long(_tableRecalls.Rows[i]["RecallTypeNum"].ToString());
					if(!RecallTypes.IsSpecialRecallType(recallTypeNum)) {
						//Make sure recall type is Perio or Prophy
						continue;
					}
				}
				row=new GridRow();
				List<String> listInternalNames=listDisplayFields.Select(x => x.InternalName).ToList();
				for(int j=0; j<listInternalNames.Count;j++) {
					switch(listInternalNames[j]){
						case "Due Date":
							row.Cells.Add(_tableRecalls.Rows[i]["dueDate"].ToString());
							break;
						case "Patient":
							row.Cells.Add(_tableRecalls.Rows[i]["patientName"].ToString());
							break;
						case "Age":
							row.Cells.Add(_tableRecalls.Rows[i]["age"].ToString());
							break;
						case "Type":
							row.Cells.Add(_tableRecalls.Rows[i]["recallType"].ToString());
							break;
						case "Interval":
							row.Cells.Add(_tableRecalls.Rows[i]["recallInterval"].ToString());
							break;
						case "#Remind":
							row.Cells.Add(_tableRecalls.Rows[i]["numberOfReminders"].ToString());
							break;
						case "LastRemind":
							row.Cells.Add(_tableRecalls.Rows[i]["dateLastReminder"].ToString());
							break;
						case "Contact":
							row.Cells.Add(_tableRecalls.Rows[i]["contactMethod"].ToString());
							break;
						case "Status":
							row.Cells.Add(_tableRecalls.Rows[i]["status"].ToString());
							break;
						case "Note":
							row.Cells.Add(_tableRecalls.Rows[i]["Note"].ToString());
							break;
						case "BillingType":
							row.Cells.Add(_tableRecalls.Rows[i]["billingType"].ToString());
							break;
						case "WebSched":
							row.Cells.Add(_tableRecalls.Rows[i]["webSchedSendDesc"].ToString());
							break;
					}
				}
				PatRowTag patRowTag=new PatRowTag();
				patRowTag.PatNum=patNum;
				patRowTag.PriKeyNum=PIn.Long(_tableRecalls.Rows[i]["RecallNum"].ToString());
				patRowTag.StatusDefNum=PIn.Long(_tableRecalls.Rows[i]["RecallStatus"].ToString());
				patRowTag.NumReminders=PIn.Int(_tableRecalls.Rows[i]["numberOfReminders"].ToString());
				patRowTag.Email=PIn.String(_tableRecalls.Rows[i]["Email"].ToString());
				patRowTag.ContactMethodRecallPref=PIn.Enum<ContactMethod>(_tableRecalls.Rows[i]["PreferRecallMethod"].ToString());
				patRowTag.GuarantorNum=PIn.Long(_tableRecalls.Rows[i]["Guarantor"].ToString());
				patRowTag.ClinicNum=PIn.Long(_tableRecalls.Rows[i]["ClinicNum"].ToString());
				patRowTag.DateDue=PIn.Date(_tableRecalls.Rows[i]["dueDate"].ToString());
				patRowTag.WirelessPhone=PIn.String(_tableRecalls.Rows[i]["WirelessPhone"].ToString());
				patRowTag.WebSchedSendError=PIn.String(_tableRecalls.Rows[i]["webSchedSendError"].ToString());
				patRowTag.AutoCommStatusWebSchedSend=PIn.Enum<AutoCommStatus>(_tableRecalls.Rows[i]["webSchedSendStatus"].ToString(),defaultEnumOption:AutoCommStatus.Undefined);
				row.Tag=patRowTag;
				gridRecalls.ListGridRows.Add(row);
				if(listPatRowTagsSelected.Any(x => x.PriKeyNum==((PatRowTag)row.Tag).PriKeyNum)) {
					gridRecalls.SetSelected(gridRecalls.ListGridRows.Count-1,true);
				}
			}
			gridRecalls.EndUpdate();
			labelPatientCount.Text=Lan.g(this,"Patient Count:")+" "+gridRecalls.ListGridRows.Count;
			Cursor=Cursors.Default;
		}
		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary> 
		private void grid_CellClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			SetFamilyColors();
		}

		private void SetFamilyColors() { 
			if(_grid==gridReminders) {
				return; //family colors not applicable here
			}
			if(_grid.SelectedIndices.Length!=1) { //If we don't have a single row selected, reset colors to black
				for(int i=0;i<_grid.ListGridRows.Count;i++) {
					_grid.ListGridRows[i].ColorText=Color.Black;
				}
				_grid.Invalidate();
				return;
			}
			//only one row is selected so highlight family members if we can
			long guar=_grid.SelectedTag<PatRowTag>().GuarantorNum;
			int famCount=0;
			for(int i=0;i<_grid.ListGridRows.Count;i++) {
				if(((PatRowTag)_grid.ListGridRows[i].Tag).GuarantorNum==guar){ //family member
					famCount++;
					_grid.ListGridRows[i].ColorText=Color.Red;
					continue;
				}
				_grid.ListGridRows[i].ColorText=Color.Black;
			}
			if(famCount==1) {//only the highlighted patient is red at this point
				_grid.ListGridRows[_grid.SelectedIndices[0]].ColorText=Color.Black;
			}
			_grid.Invalidate();
		}

		private void gridRecalls_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridRecalls.Columns[e.Col].Tag.ToString()=="WebSched") {//A column's tag is its display field internal name.
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).WebSchedSendError);
				msgBoxCopyPaste.Text=Lan.g(this,"Web Sched Notification Send Error");
				msgBoxCopyPaste.ShowDialog();
				return;
			}
			Recall recall=Recalls.GetRecall(((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).PriKeyNum);
			if(recall==null) {
				MsgBox.Show(this,"Recall for this patient has been removed.");
				FillRecalls();
				return;
			}
			long selectedPatNum=((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).PatNum;
			using FormRecallEdit formRecallEdit=new FormRecallEdit();
			formRecallEdit.RecallCur=recall.Copy();
			if(formRecallEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(recall.RecallStatus!=formRecallEdit.RecallCur.RecallStatus//if the status has changed
				|| (recall.IsDisabled != formRecallEdit.RecallCur.IsDisabled)//or any of the three disabled fields was changed
				|| (recall.DisableUntilDate != formRecallEdit.RecallCur.DisableUntilDate)
				|| (recall.DisableUntilBalance != formRecallEdit.RecallCur.DisableUntilBalance)
				|| (recall.Note != formRecallEdit.RecallCur.Note))//or a note was added
			{
				//make a commlog entry
				//unless there is an existing recall commlog entry for today
				bool hasRecallEntryToday=false;
				List<Commlog> listCommlog=Commlogs.Refresh(selectedPatNum);
				for(int i=0;i<listCommlog.Count;i++) {
					if(listCommlog[i].CommDateTime.Date==DateTime.Today	
						&& listCommlog[i].CommType==Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL)) {
						hasRecallEntryToday=true;
					}
				}
				if(!hasRecallEntryToday) {
					Commlog commLog=new Commlog();
					commLog.CommDateTime=DateTime.Now;
					commLog.Mode_=CommItemMode.Phone;//user can change this, of course.
					commLog.SentOrReceived=CommSentOrReceived.Sent;
					commLog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL);
					commLog.PatNum=selectedPatNum;
					commLog.Note="";
					if(recall.RecallStatus!=formRecallEdit.RecallCur.RecallStatus) {
						if(formRecallEdit.RecallCur.RecallStatus==0) {
							commLog.Note+=Lan.g(this,"Status None");
						}
						else {
							commLog.Note+=Defs.GetName(DefCat.RecallUnschedStatus,formRecallEdit.RecallCur.RecallStatus);
						}
					}
					if(recall.DisableUntilDate!=formRecallEdit.RecallCur.DisableUntilDate && formRecallEdit.RecallCur.DisableUntilDate.Year>1880) {
						if(commLog.Note!="") {
							commLog.Note+=",  ";
						}
						commLog.Note+=Lan.g(this,"Disabled until ")+formRecallEdit.RecallCur.DisableUntilDate.ToShortDateString();
					}
					if(recall.DisableUntilBalance!=formRecallEdit.RecallCur.DisableUntilBalance && formRecallEdit.RecallCur.DisableUntilBalance>0) {
						if(commLog.Note!="") {
							commLog.Note+=",  ";
						}
						commLog.Note+=Lan.g(this,"Disabled until balance below ")+formRecallEdit.RecallCur.DisableUntilBalance.ToString("c");
					}
					if(recall.Note!=formRecallEdit.RecallCur.Note) {
						if(commLog.Note!="") {
							commLog.Note+=",  ";
						}
						commLog.Note+=formRecallEdit.RecallCur.Note;
					}
					commLog.Note+=".  ";
					commLog.UserNum=Security.CurUser.UserNum;
					commLog.IsNew=true;
					using FormCommItem formCommItem=new FormCommItem(commLog);
					formCommItem.ShowDialog();
				}
			}
			FillRecalls();
			List<int> listIndexes=new List<int>();
			for(int i=0;i<gridRecalls.ListGridRows.Count;i++) {
				long patNum=((PatRowTag)gridRecalls.ListGridRows[i].Tag).PatNum;
				if(patNum==selectedPatNum){
					listIndexes.Add(i);
				}
			}
			gridRecalls.SetSelected(listIndexes.ToArray(),true);
			SetFamilyColors();
		}

		///<summary>Creates a recall appointment and returns the AptNum in a list.  Validation should be done prior to invoking this method.
		///Shows an error message to the user and then returns an empty list if anything goes wrong.</summary>
		private List<Appointment> SchedPatRecall(long recallNum,Patient patient,List<InsSub> listInSubs,List<InsPlan> listInsPlans) {
			bool hasProphyPerio=Recalls.HasProphyOrPerioScheduled(_patNumCur);
			if(hasProphyPerio) {
				MsgBox.Show(this,"Recall has already been scheduled.");
				FillCurGrid();
				return new List<Appointment>();
			}
			if(PatRestrictionL.IsRestricted(patient.PatNum,PatRestrict.ApptSchedule)) {
				return new List<Appointment>();
			}
			Recall recall=Recalls.GetRecall(recallNum);
			Appointment appointment;
			try{
				appointment=AppointmentL.CreateRecallApt(patient,listInsPlans,recallNum,listInSubs);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return new List<Appointment>();
			}
			//The appointment got saved with min date. We need the object to have the actual appointment date so we can jump to the appointment date.
			appointment.AptDateTime=recall.DateDue;
			return new List<Appointment>() { appointment };
		}

		///<summary>Creates recall appointments for the family and returns a list of all the AptNums.
		///Validation should be done prior to invoking this method.
		///Shows a message to the user if there were any restricted patients or no appointments created.</summary>
		public List<Appointment> SchedFamRecall(Family family,List<InsSub> listInsSubs,List<InsPlan> listInsPlans) {
			Appointment appointment;
			List<Appointment> listAppointments=new List<Appointment>();
			int patsRestricted=0;
			List<Recall> listRecalls=Recalls.GetList(family.ListPats.Select(x => x.PatNum).ToList());
			bool doRefreshGrid=false;
			for(int i=0;i<family.ListPats.Length;i++) {
				if(PatRestrictionL.IsRestricted(family.ListPats[i].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				if(Recalls.HasProphyOrPerioScheduled(family.ListPats[i].PatNum,listRecalls)) {
					doRefreshGrid=true;
					continue;
				}
				Recall recall=listRecalls.FirstOrDefault(x=>x.PatNum==family.ListPats[i].PatNum);
				if(recall==null) {
					continue;
				}
				try{
					//Passes in -1 as the RecallNum. This will create an appointment for either a Perio or Prophy recall type only.
					appointment=AppointmentL.CreateRecallApt(family.ListPats[i],listInsPlans,-1,listInsSubs);
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue;
				}
				//The appointment got saved with min date. We need the object in memory to have the actual appointment date so we can jump to the appointment date.
				appointment.AptDateTime=recall.DateDue;
				listAppointments.Add(appointment);
			}
			if(patsRestricted>0) {
				MessageBox.Show(Lan.g(this,"Family members skipped due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			if(listAppointments.Count==0) {
				MsgBox.Show(this,"No recall is due.");
				if(doRefreshGrid) {
					FillCurGrid();
				}
				return new List<Appointment>();
			}
			return listAppointments;
		}
		
		///<summary>Automatically open the eService Setup window so that they can easily click the Enable button. 
		///Calls CheckClinicsSignedUpForWebSched() before exiting.</summary>
		private void OpenSignupPortal() {
			using FormEServicesSignup formEServicesSignup=new FormEServicesSignup();
			formEServicesSignup.ShowDialog();
			//User may have made changes to signups. Reload the valid clinics from HQ.
			CheckClinicsSignedUpForWebSched();
		}

		private void panelWebSched_MouseClick(object sender,MouseEventArgs e) {
			SendWebSched();
		}

		private void SendWebSched() {
			#region Check Web Sched Pref and Show Promo
			if(IsDisposed) {//The user closed the form while the thread checking Web Sched signups was still running.
				return;
			}
			if(_threadWebSchedSignups!=null) {//The thread has not finished getting the list.
				_hasClickedWebSched=true;//The thread checking Web Sched signups will call this method on exit.
				Cursor=Cursors.AppStarting;
				return;
			}
			if(_isOnSupport!=YN.Yes) {
				MsgBox.Show(this,"You must be on support to use this feature.");
				return;
			}
			bool needsToBeSignedUp=WebSchedRecalls.TemplatesHaveURLTags();
			if(needsToBeSignedUp && _listClinicNumsWebSched.Count==0) {//No clinics are signed up for Web Sched
				string message=PrefC.HasClinicsEnabled ?
					"No clinics are signed up for Web Sched Recall. Open Sign Up Portal?" : 
					"This practice is not signed up for Web Sched Recall. Open Sign Up Portal?";
				message+="\r\n\r\nAlternatively, you could remove all URL tags from Web Sched text and emails templates to use this feature.";
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
					return;
				}
				OpenSignupPortal();
				return;
			}
			//At least one clinic is signed up for Web Sched or the templates are not using URLs.
			List<long> listClinicNumsNotSignedUp=new List<long>();
			List<int> listGridIndicesNotSignUp=new List<int>();
			for(int i=0;i<gridRecalls.SelectedIndices.Length;i++) {
				long clinicNum=((PatRowTag)gridRecalls.ListGridRows[gridRecalls.SelectedIndices[i]].Tag).ClinicNum;
				//We don't want to send users to the sign up portal for clinic 0 if clinics are enabled because there will be nothing for them to do there. 
				if(clinicNum==0 && !_listClinicNumsWebSched.Contains(0)) {
					continue;//We will deselect these rows later.
				}
				if(_listClinicNumsWebSched.Count > 0 && !PrefC.HasClinicsEnabled) {
					//The office is signed up for Web Sched, but the patient's clinic might not be 0.
					continue;//We will let them send for this patient.
				}
				if(!_listClinicNumsWebSched.Contains(clinicNum)) {
					listClinicNumsNotSignedUp.Add(clinicNum);
				}
				listGridIndicesNotSignUp.Add(i);
			}
			if(needsToBeSignedUp && listClinicNumsNotSignedUp.Count > 0) {
				string message=Lan.g(this,"You have selected recalls whose clinic is not signed up for Web Sched recall. "
					+"Do you want to go to the sign up portal to sign these clinics up? "
					+"Clicking 'No' will deselect these recalls and send the remaining.");
				if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.Yes) {
					OpenSignupPortal();
					return;
				}
				//De-select any rows that are not allowed to send WebSched.
				gridRecalls.SetSelected(listGridIndicesNotSignUp.ToArray(),false);
			}
			#endregion Check Web Sched Pref and Show Promo
			#region Recall List Validation
			if(gridRecalls.ListGridRows.Count < 1) {
				MessageBox.Show(Lan.g(this,"There are no Patients in the Recall table.  Must have at least one."));
				return;
			}
			if(!EmailAddresses.ExistsValidEmail()) {
				MsgBox.Show(this,"You need to enter an SMTP server name in email setup before you can send email.");
				return;
			}
			if(PrefC.GetLong(PrefName.RecallStatusEmailed)==0
				|| PrefC.GetLong(PrefName.RecallStatusTexted)==0
				|| PrefC.GetLong(PrefName.RecallStatusEmailedTexted)==0) 
			{
				MsgBox.Show(this,"You need to set an email status, text status, and email and text status first in the Recall Setup window.");
				return;
			}
			if(!ODBuild.IsDebug()) {
				if(EServiceSignals.GetListenerServiceStatus().In(
					eServiceSignalSeverity.None,
					eServiceSignalSeverity.NotEnabled,
					eServiceSignalSeverity.Critical)) 
				{
					MsgBox.Show(this,"Your eConnector is not currently running. Please enable the eConnector to send Web Sched Recalls.");
					return;
				}
			}
			//If the user didn't manually select any recalls we will automatically select all rows that have an email or text prefer recall method.
			if(gridRecalls.SelectedIndices.Length==0) {
				List<int> listIndexes=new List<int>();
				for(int i=0;i<gridRecalls.ListGridRows.Count;i++){
					ContactMethod contactMethod=((PatRowTag)gridRecalls.ListGridRows[i].Tag).ContactMethodRecallPref;
					if(contactMethod.In(ContactMethod.Email,ContactMethod.TextMessage)){
						listIndexes.Add(i);
					}
				}
				gridRecalls.SetSelected(listIndexes.ToArray(),true);
			}
			if(gridRecalls.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No patients prefer contact via email or text.");
				return;
			}
			//Now that there are rows guaranteed to be selected, check each row to see if their recall will yield available Web Sched time slots.
			//Deselect the ones that do not have email or wireless phone specified or are assigned to clinic num 0 when clinics are enabled.
			//Also deselect ones that were just sent but are still in the list because the grid hasn't refreshed yet.
			int skippedContact=0;
			int skippedTimeSlot=0;
			int skippedClinic0=0;
			int skippedNotInList=0;
			int skippedRestricted=0;
			List<long> listRestricted=PatRestrictions.GetAllRestrictedForType(PatRestrict.ApptSchedule);
			DataTable tableRecalls=GetRecallTable();
			List<long> listPatNumsInTableRecallCur=tableRecalls.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
			for(int i=gridRecalls.SelectedIndices.Length-1;i>=0;i--) {
				PatRowTag patRowTag=gridRecalls.ListGridRows[gridRecalls.SelectedIndices[i]].Tag as PatRowTag;
				if(listRestricted.Contains(patRowTag.PatNum)){
					skippedRestricted++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//Check that they at least have an email or wireless phone.
				if(string.IsNullOrEmpty(patRowTag.Email.Trim()) && string.IsNullOrEmpty(patRowTag.WirelessPhone.Trim())) {
					skippedContact++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//If this practice has clinics enabled for Web Sched, then they will not have Web Sched enabled for clinic num 0. They will need to assign
				//patients to a clinic in order to send Web Sched. We will prompt them below to assign them.
				if(needsToBeSignedUp && patRowTag.ClinicNum==0 && !_listClinicNumsWebSched.Contains(0)) {
					skippedClinic0++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//Check that the patient is still in the recall list (they haven't been sent something since the grid has been refreshed)
				if(!listPatNumsInTableRecallCur.Contains(patRowTag.PatNum)) {
					skippedNotInList++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//The eConnector will attempt to send a webschedrecall if WebSchedSendStatus is already SendNotAttempted
				if(patRowTag.AutoCommStatusWebSchedSend==AutoCommStatus.SendNotAttempted) {
					continue;//The eConnector is about to send this anyway.
				}
				//Check to see if they'll have any potential time slots via their Web Sched link.
				DateTime dateTimeDue=patRowTag.DateDue;
				if(patRowTag.DateDue.Date<DateTime.Now.Date) {
					dateTimeDue=DateTime.Now;
				}
				//This takes a long time to run for lots of recalls.  Might consider making a faster overload in the future (213 recalls ~ 10 seconds).
				bool hasTimeSlots=false;
				DateTime date=dateTimeDue.AddMonths(PrefC.GetInt(PrefName.WebSchedRecallApptSearchMaximumMonths));
				if(needsToBeSignedUp) {
					try{
						hasTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(patRowTag.PriKeyNum,dateTimeDue,date).Count>0;
					}
					catch{}
				}
				else {
					hasTimeSlots=true;
				}
				if(!hasTimeSlots) {
					skippedTimeSlot++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
				}
			}
			List<string> listSkippedMsgs=new List<string>();
			if(skippedContact>0) {
				listSkippedMsgs.Add(Lan.g(this,"Selected patients skipped due to missing email addresses and wireless phone:")+" "+skippedContact);
			}
			if(skippedTimeSlot>0) {
				listSkippedMsgs.Add(Lan.g(this,"Selected patients skipped due to no available Web Sched time slots found:")+" "+skippedTimeSlot);
			}
			if(skippedClinic0>0) {
				listSkippedMsgs.Add(Lan.g(this,"Selected patients skipped due to not being assigned to a clinic:")+" "+skippedClinic0);
			}
			if(skippedRestricted>0) {
				listSkippedMsgs.Add(Lan.g(this,"Selected patients skipped due to patient restriction:")+ " "+skippedRestricted);
			}
			if(skippedNotInList>0) {
				FillRecalls();
				listSkippedMsgs.Add(Lan.g(this,"Selected patients skipped due to no longer being in the recall list:")+" "+skippedNotInList);
			}
			if(!listSkippedMsgs.IsNullOrEmpty()) {
				MessageBox.Show(string.Join("\r\n",listSkippedMsgs));
			}
			if(gridRecalls.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No Web Sched emails or texts sent.");
				return;
			}
			#endregion Recall List Validation
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send Web Sched emails and/or texts to all of the selected patients?")) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			List<long> listRecallNums=gridRecalls.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList();
			List<string> listWebSchedErrors=new List<string>();
			List<string> listTemp=WebSchedRecalls.InsertForRecallNums(
				//Queue emails.  AutoComm will filter out any that cannot be sent as email.
				listRecallNums,
				checkGroupFamiliesRecalls.Checked,
				comboSortRecalls.GetSelected<RecallListSort>(),
				WebSchedRecallSource.FormRecallList,
				CommType.Email,
				DateTime.Now
			);
			listWebSchedErrors.AddRange(listTemp);
			listTemp=WebSchedRecalls.InsertForRecallNums(
				listRecallNums,
				checkGroupFamiliesRecalls.Checked,
				comboSortRecalls.GetSelected<RecallListSort>(),
				WebSchedRecallSource.FormRecallList,
				CommType.SecureEmail,
				DateTime.Now
			);
			listWebSchedErrors.AddRange(listTemp);
			listTemp=WebSchedRecalls.InsertForRecallNums(
				//Queue SMS.  AutoComm will filter out any that cannot be sent as SMS.
				listRecallNums,
				checkGroupFamiliesRecalls.Checked,
				comboSortRecalls.GetSelected<RecallListSort>(),
				WebSchedRecallSource.FormRecallList,
				CommType.Text,
				DateTime.Now
			);
			listWebSchedErrors.AddRange(listTemp);
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.WebSchedRecallManualSend,0,Lan.g(this,"Web Sched Recalls manually sent."));
			if(listWebSchedErrors.Count>0) {
				//Show the error (already translated) to the user and then refresh the grid in case any were successful.
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(string.Join("\r\n",listWebSchedErrors));
				msgBoxCopyPaste.Show();
			}
			FillRecalls();
		}

		private void checkGroupFamilies_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillCurGrid();
			Cursor=Cursors.Default;
		}

		private void checkShowConflictingTypes_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillRecalls();
			Cursor=Cursors.Default;
		}

		private void butReport_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			if(gridRecalls.ListGridRows.Count < 1){
				MsgBox.Show(this,"There are no Patients in the Recall table.  Must have at least one to run report.");    
				return;
			}
			List<long> recallNums=new List<long>();
			if(gridRecalls.SelectedIndices.Length==0) {
				recallNums=gridRecalls.ListGridRows.Select(x => ((PatRowTag)x.Tag).PriKeyNum).ToList();
			}
			else{
				recallNums=gridRecalls.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList();
			}
			using FormRpRecall formRpRecall=new FormRpRecall(recallNums);
			formRpRecall.ShowDialog();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabels_Click(object sender, System.EventArgs e) {
			if(IsGridEmpty()){
				return;
			}
			if(!IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed)){
				return;
			}
			if(!IsAnyPatToContact("labels",ContactMethod.Mail)) {
				return;
			}
			CommItemTypeAuto commItemTypeAuto=CommItemTypeAuto.REACT;
			if(IsRecallGridSelected()){
				commItemTypeAuto=CommItemTypeAuto.RECALL;
			}
			_tableAddress=GetAddrTable();
			pagesPrinted=0;
			patientsPrinted=0;
			string listLabelsPrinted=Lan.g(this,(commItemTypeAuto==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list labels printed");
			PrinterL.TryPreview(pdLabels_PrintPage,listLabelsPrinted,PrintSituation.LabelSheet,new Margins(0,0,0,0),PrintoutOrigin.AtMargin,
				totalPages:(int)Math.Ceiling((double)_tableAddress.Rows.Count/30)
			);
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change statuses and make commlog entries for all of the selected patients?")) {
				ProcessComms(commItemTypeAuto,IsRecallGridSelected());
			}
			FillCurGrid();
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabelOne_Click(object sender,EventArgs e) {
			if(!IsAnyRowSelected()){
				return;
			}
			CommItemTypeAuto commItemTypeAuto=CommItemTypeAuto.REACT;
			if(IsRecallGridSelected()){
				commItemTypeAuto=CommItemTypeAuto.RECALL;
			}
			_tableAddress=GetAddrTable();
			patientsPrinted=0;
			string text;
			while(patientsPrinted<_tableAddress.Rows.Count) {
				text="";
				if(DoGroupFamilies() && _tableAddress.Rows[patientsPrinted]["famList"].ToString()!="") {//print family label
					text=_tableAddress.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {//print single label
					text=_tableAddress.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				text+=_tableAddress.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				text+=_tableAddress.Rows[patientsPrinted]["City"].ToString()+", "
					+_tableAddress.Rows[patientsPrinted]["State"].ToString()+" "
					+_tableAddress.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				LabelSingle.PrintText(0,text);
				patientsPrinted++;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the labels finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if labels printed successfully.")) {
				ProcessComms(commItemTypeAuto,IsRecallGridSelected());
			}
			FillCurGrid();
			Cursor=Cursors.Default;
		}

		///<summary>Changes made to printing recall postcards need to be made in FormConfirmList.butPostcards_Click() as well.
		///Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPostcards_Click(object sender,System.EventArgs e) {
			if(IsGridEmpty()){
				return;
			}
			if(!IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed)){
				return;
			}
			if(!IsAnyPatToContact("postcards",ContactMethod.Mail)) {
				return;
			}
			CommItemTypeAuto commItemTypeAuto=CommItemTypeAuto.REACT;
			if(IsRecallGridSelected()){
				commItemTypeAuto=CommItemTypeAuto.RECALL;
			}
			_tableAddress=GetAddrTable();
			pagesPrinted=0;
			patientsPrinted=0;
			PaperSize paperSize;
			PrintoutOrientation orient=PrintoutOrientation.Default;
			long postcardsPerSheet=PrefC.GetLong(PrefName.ReactivationPostcardsPerSheet);
			if(commItemTypeAuto==CommItemTypeAuto.RECALL) {
				postcardsPerSheet=PrefC.GetLong(PrefName.RecallPostcardsPerSheet);
			}
			if(postcardsPerSheet==1) {
				paperSize=new PaperSize("Postcard",500,700);
				orient=PrintoutOrientation.Landscape;
			}
			else if(postcardsPerSheet==3) {
				paperSize=new PaperSize("Postcard",850,1100);
			}
			else {//4
				paperSize=new PaperSize("Postcard",850,1100);
				orient=PrintoutOrientation.Landscape;
			}
			int totalPages=(int)Math.Ceiling((double)_tableAddress.Rows.Count/(double)postcardsPerSheet);
			string listPostCardsPrinted=Lan.g(this,(commItemTypeAuto==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list postcards printed");
			PrinterL.TryPreview(pdCards_PrintPage,listPostCardsPrinted,PrintSituation.Postcard, new Margins(0,0,0,0), PrintoutOrigin.AtMargin, paperSize,orient,
				totalPages
			);
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the postcards finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if postcards printed successfully.")) {
				ProcessComms(commItemTypeAuto,IsRecallGridSelected());
			}
			FillCurGrid();
			Cursor=Cursors.Default;
		}

		private void butUndo_Click(object sender,EventArgs e) {
			using FormRecallListUndo formRecallListUndo=new FormRecallListUndo();
			formRecallListUndo.ShowDialog();
			if(formRecallListUndo.DialogResult==DialogResult.OK) {
				FillRecalls();
			}
		}

		private void butECards_Click(object sender,EventArgs e) {
			if(!Programs.IsEnabledByHq(ProgramName.Divvy,out string err)) {
				MsgBox.Show(err);
				return;
			}
			if(!Programs.IsEnabled(ProgramName.Divvy)) {
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"The Divvy Program Link is not enabled. Would you like to enable it now?")) {
					using FormProgramLinkEdit formProgramLinkEdit=new FormProgramLinkEdit();
					formProgramLinkEdit.ProgramCur=Programs.GetCur(ProgramName.Divvy);
					formProgramLinkEdit.ShowDialog();
					DataValid.SetInvalid(InvalidType.Programs);
				}
				if(!Programs.IsEnabled(ProgramName.Divvy)) {
					return;
				}
			}
			if(gridRecalls.ListGridRows.Count < 1) {
				MessageBox.Show(Lan.g(this,"There are no Patients in the Recall table.  Must have at least one to send."));
				return;
			}
			if(PrefC.GetLong(PrefName.RecallStatusMailed)==0) {
				MsgBox.Show(this,"You need to set a status first in the Recall Setup window.");
				return;
			}
			if(gridRecalls.SelectedIndices.Length==0) {
				for(int i=0;i<gridRecalls.ListGridRows.Count;i++){
					ContactMethod contactMethod=((PatRowTag)gridRecalls.ListGridRows[i].Tag).ContactMethodRecallPref;
					if(contactMethod.In(ContactMethod.Mail,ContactMethod.None)){
						gridRecalls.SetSelected(i);
					}
				}
			}
			if(gridRecalls.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No patients of mail type.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send postcards for all of the selected patients?")) {
				return;
			}
			List<long> listRecallNums=gridRecalls.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList();
			_tableAddress=Recalls.GetAddrTable(listRecallNums,checkGroupFamiliesRecalls.Checked,comboSortRecalls.GetSelected<RecallListSort>());
			DivvyConnect.Postcard postcard;
			DivvyConnect.Recipient recipient;
			DivvyConnect.Postcard[] listPostcards=new DivvyConnect.Postcard[gridRecalls.SelectedIndices.Length];
			string message;
			long clinicNum;
			Clinic clinic;
			string phone;
			for(int i=0;i<_tableAddress.Rows.Count;i++) {
				postcard=new DivvyConnect.Postcard();
				recipient=new DivvyConnect.Recipient();
				recipient.Name=_tableAddress.Rows[i]["patientNameFL"].ToString();
				recipient.ExternalRecipientID=_tableAddress.Rows[i]["patNums"].ToString();
				recipient.Address1=_tableAddress.Rows[i]["Address"].ToString();//Includes Address2
				recipient.City=_tableAddress.Rows[i]["City"].ToString();
				recipient.State=_tableAddress.Rows[i]["State"].ToString();
				recipient.Zip=_tableAddress.Rows[i]["Zip"].ToString();
				postcard.AppointmentDateTime=PIn.Date(_tableAddress.Rows[i]["dateDue"].ToString());//js I don't know why they would ask for this.  We put this in our message.
				//Body text, family card ------------------------------------------------------------------
				if(checkGroupFamiliesRecalls.Checked	&& _tableAddress.Rows[i]["famList"].ToString()!=""){
					if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg);
					}
					else if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg3);
					}
					message=message.Replace("[FamilyList]",_tableAddress.Rows[i]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage);
					}
					else if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardMessage3);
					}
					message=message.Replace("[DueDate]",_tableAddress.Rows[i]["dateDue"].ToString());
					message=message.Replace("[NameF]",_tableAddress.Rows[i]["patientNameF"].ToString());
					message=message.Replace("[NameFL]", _tableAddress.Rows[i]["patientNameFL"].ToString());
				}
				Clinic clinicCur=Clinics.GetClinicForRecall(PIn.Long(_tableAddress.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
				message=message.Replace("[ClinicName]",clinicCur.Abbr);
				message=message.Replace("[ClinicPhone]",clinicCur.Phone);
				message=message.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				message=message.Replace("[PracticePhone]",PrefC.GetString(PrefName.PracticePhone));
				string officePhone=clinicCur.Phone;
				if(string.IsNullOrEmpty(officePhone)) {
					officePhone=PrefC.GetString(PrefName.PracticePhone);
				}
				message=message.Replace("[OfficePhone]",clinicCur.Phone);
				postcard.Message=message;
				postcard.Recipient=recipient;
				postcard.DesignID=PIn.Int(ProgramProperties.GetPropVal(ProgramName.Divvy,"DesignID for Recall Cards"));
				listPostcards[i]=postcard;
			}
			DivvyConnect.Practice practice=new DivvyConnect.Practice();
			clinicNum=PIn.Long(_tableAddress.Rows[patientsPrinted]["ClinicNum"].ToString());
			Clinic clinicAssigned=Clinics.GetClinic(clinicNum); // this patient assigned to a clinic
			bool isUsingClinics=PrefC.HasClinicsEnabled && Clinics.GetCount() > 0; //if using clinics
			if(isUsingClinics && clinicAssigned !=null) { 
				clinic=Clinics.GetClinic(clinicNum);
				practice.Company=clinic.Description;
				practice.Address1=clinic.Address;
				practice.Address2=clinic.Address2;
				practice.City=clinic.City;
				practice.State=clinic.State;
				practice.Zip=clinic.Zip;
				phone=clinic.Phone;
			}
			else {
				practice.Company=PrefC.GetString(PrefName.PracticeTitle);
				practice.Address1=PrefC.GetString(PrefName.PracticeAddress);
				practice.Address2=PrefC.GetString(PrefName.PracticeAddress2);
				practice.City=PrefC.GetString(PrefName.PracticeCity);
				practice.State=PrefC.GetString(PrefName.PracticeST);
				practice.Zip=PrefC.GetString(PrefName.PracticeZip);
				phone=PrefC.GetString(PrefName.PracticePhone);
			}
			practice.Phone=TelephoneNumbers.ReFormat(phone);
			DivvyConnect.PostcardServiceClient postcardServiceClient=new DivvyConnect.PostcardServiceClient();
			DivvyConnect.PostcardReturnMessage postcardReturnMessage=new DivvyConnect.PostcardReturnMessage();
			string messages="";
			Cursor=Cursors.WaitCursor;
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(ProgramName.Divvy,"Password"));
			Guid guid=Guid.Parse(ProgramProperties.GetPropVal(ProgramName.Divvy,"API Key"));
			string username=ProgramProperties.GetPropVal(ProgramName.Divvy,"Username");
			try {
				postcardReturnMessage=postcardServiceClient.SendPostcards(guid,username,password,listPostcards,practice);
			}
			catch (Exception ex) {
				messages+="Exception: "+ex.Message+"\r\nData: "+ex.Data+"\r\n";
			}
			messages+="MessageCode: "+postcardReturnMessage.MessageCode.ToString();//MessageCode enum. 0=CompletedSuccessfully, 1=CompletedWithErrors, 2=Failure
			MsgBox.Show(this,"Return Messages: "+postcardReturnMessage.Message+"\r\n"+messages);
			if(postcardReturnMessage.MessageCode==DivvyConnect.MessageCode.CompletedSucessfully) {
				Cursor=Cursors.WaitCursor;
				if(IsRecallGridSelected()) {
					ProcessComms(CommItemTypeAuto.RECALL,IsRecallGridSelected(),CommItemMode.Mail);
				}
				else {
					ProcessComms(CommItemTypeAuto.REACT,IsRecallGridSelected(),CommItemMode.Mail);
				}
			}
			else if(postcardReturnMessage.MessageCode==DivvyConnect.MessageCode.CompletedWithErrors) {
				for(int i=0;i<postcardReturnMessage.PostcardMessages.Length;i++) {
					//todo: process return messages. Update commlog and change recall statuses for postcards that were sent.
				}
			}
			FillRecalls();
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			if(IsGridEmpty()) {
				return;
			}
			if(!EmailAddresses.ExistsValidEmail()) {
				MsgBox.Show(this,"You need to enter an SMTP server name in e-mail setup before you can send e-mail.");
				return;
			}
			if(!IsStatusSet(PrefName.RecallStatusEmailed,PrefName.ReactivationStatusEmailed)) {
				return;
			}
			//Returns true if some patients already selected, otherwise tries to select all patients with email as preferred recall method automatically.
			if(!IsAnyPatToContact("email",ContactMethod.Email,false)) {
				return;
			}
			int skipped=0;
			for(int i=_grid.SelectedIndices.Length-1;i>=0;i--) {
				if(!EmailAddresses.IsValidEmail(_grid.SelectedTags<PatRowTag>()[i].Email)) {
					skipped++;
					_grid.SetSelected(_grid.SelectedIndices[i],false);
				}
			}
			if(_grid.SelectedIndices.Length==0){
				MsgBox.Show(this,"None of the selected patients had valid email addresses entered.");
				return;
			}
			if(skipped>0){
				MessageBox.Show(Lan.g(this,"Selected patients skipped due to missing/invalid email addresses: ")+skipped.ToString());
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send email to all of the selected patients?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			_tableAddress=GetAddrTable();
			//Email
			EmailMessage emailMessage;
			string str="";
			EmailAddress emailAddress;
			int sentEmailCount=0;
			//Capturing these variables now because SendEmailUnsecure can call Application.DoEvents which means the user can change the selected grid.
			bool isRecallGridSelected=IsRecallGridSelected();
			GridOD grid=_grid;
			bool doGroupFamilies=DoGroupFamilies();
			List<PatRowTag> listPatRowTagsSent=new List<PatRowTag>();
			for(int i=0;i<_tableAddress.Rows.Count;i++){
				emailMessage=new EmailMessage();
				emailMessage.PatNum=PIn.Long(_tableAddress.Rows[i]["emailPatNum"].ToString());
				emailMessage.ToAddress=PIn.String(_tableAddress.Rows[i]["email"].ToString());//might be guarantor email
				Clinic clinic;
				if(isRecallGridSelected) {
					clinic=Clinics.GetClinicForRecall(PIn.Long(_tableAddress.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
				}
				else {
					clinic=Clinics.GetClinic(PIn.Long(_tableAddress.Rows[i]["ClinicNum"].ToString()));
				}
				long clinicNumEmail=clinic?.ClinicNum??Clinics.ClinicNum;
				OpenDental.UI.ComboBox cbEmail=isRecallGridSelected?comboEmailFromRecalls:comboEmailFromReact;
				emailAddress=cbEmail.GetSelected<EmailAddress>()??new EmailAddress();
				if(emailAddress.EmailAddressNum==0) { //clinic/practice default
					clinicNumEmail=PIn.Long(_tableAddress.Rows[i]["ClinicNum"].ToString());
					emailAddress=EmailAddresses.GetByClinic(clinicNumEmail);
				}
				emailAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,clinicNumEmail); //Use clinic's Email Sender Address Override, if present
				emailMessage.FromAddress=emailAddress.GetFrom();
				if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="0") {
					emailMessage.Subject=PrefC.GetString(PrefName.ReactivationEmailSubject);
					if(isRecallGridSelected){
						emailMessage.Subject=PrefC.GetString(PrefName.RecallEmailSubject);
					}
				}
				else if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="1") {
					emailMessage.Subject=PrefC.GetString(PrefName.ReactivationEmailSubject);
					if(isRecallGridSelected) {
						emailMessage.Subject=PrefC.GetString(PrefName.RecallEmailSubject2);
					}
				}
				else {
					emailMessage.Subject=PrefC.GetString(PrefName.ReactivationEmailSubject);
					if(isRecallGridSelected){
						emailMessage.Subject=PrefC.GetString(PrefName.RecallEmailSubject3);
					}
				}
				//family
				if(doGroupFamilies && _tableAddress.Rows[i]["famList"].ToString()!="") {
					if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(PrefName.ReactivationEmailFamMsg);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailFamMsg);
						}
					}
					else if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(PrefName.ReactivationEmailFamMsg);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailFamMsg2);
						}
					}
					else {
						str=PrefC.GetString(PrefName.ReactivationEmailFamMsg);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailFamMsg3);
						}
					}
					str=str.Replace("[FamilyList]",_tableAddress.Rows[i]["famList"].ToString());
				}
				//single
				else {
					if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(PrefName.ReactivationEmailMessage);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailMessage);
						}
					}
					else if(_tableAddress.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(PrefName.ReactivationEmailMessage);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailMessage2);
						}
					}
					else {
						str=PrefC.GetString(PrefName.ReactivationEmailMessage);
						if(isRecallGridSelected) {
							str=PrefC.GetString(PrefName.RecallEmailMessage3);
						}
					}
					str=str.Replace("[DueDate]",PIn.Date(_tableAddress.Rows[i]["dateDue"].ToString()).ToShortDateString());
					str=str.Replace("[NameF]",_tableAddress.Rows[i]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",_tableAddress.Rows[i]["patientNameFL"].ToString());
				}
				string officePhone="";
				string mainPhone=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
				if(clinic==null) {
					str=str.Replace("[ClinicName]",PrefC.GetString(PrefName.PracticeTitle));
					str=str.Replace("[ClinicPhone]",mainPhone);
					officePhone=mainPhone;
				}
				else {
					str=str.Replace("[ClinicName]",clinic.Abbr);
					str=str.Replace("[ClinicPhone]",TelephoneNumbers.ReFormat(clinic.Phone));
					officePhone=clinic.Phone;
				}
				str=str.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				str=str.Replace("[PracticePhone]",mainPhone);
				str=str.Replace("[OfficePhone]",officePhone);
				emailMessage.BodyText=EmailMessages.FindAndReplacePostalAddressTag(str,clinicNumEmail);
				emailMessage.MsgDateTime=DateTime.Now;
				emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
				emailMessage.MsgType=EmailMessageSource.Recall;
				try{
					EmailMessages.SendEmail(emailMessage,emailAddress);
				}
				catch(Exception ex){
					Cursor=Cursors.Default;
					str=ex.Message+"\r\n";
					if(ex.GetType()==typeof(System.ArgumentException)){
						str+=$"Go to Setup, {(isRecallGridSelected ? "Recall":"Reactivation")}.  The subject for an email may not be multiple lines.\r\n";
					}
					MessageBox.Show(str+"Patient:"+_tableAddress.Rows[i]["patientNameFL"].ToString());
					break;
				}
				sentEmailCount++;
				//Add current row to the list of rows sent.
				if(isRecallGridSelected) {
					List<long> listRecallNums=_tableAddress.Rows[i]["recallNums"].ToString().Split(',').Select(x => PIn.Long(x)).ToList();
					for(int j=0;j<grid.ListGridRows.Count;j++) {
						if(listRecallNums.Contains(((PatRowTag)grid.ListGridRows[j].Tag).PriKeyNum)) {
							listPatRowTagsSent.Add((PatRowTag)grid.ListGridRows[j].Tag);
						}
					}
				}
				else {//Reactivation
					List<long> listPatNums=_tableAddress.Rows[i]["patNums"].ToString().Split(',').Select(x => PIn.Long(x)).ToList();
					for(int j=0;j<grid.ListGridRows.Count;j++) {
						if(listPatNums.Contains(((PatRowTag)grid.ListGridRows[j].Tag).PatNum)) {
							listPatRowTagsSent.Add((PatRowTag)grid.ListGridRows[j].Tag);
						}
					}
				}
			}
			if(isRecallGridSelected) {
				ProcessComms(CommItemTypeAuto.RECALL,isRecallGridSelected,CommItemMode.Email,listPatRowTagsSent);
			}
			else {
				ProcessComms(CommItemTypeAuto.REACT,isRecallGridSelected,CommItemMode.Email,listPatRowTagsSent);
			}
			FillCurGrid();
			if(sentEmailCount>0) {
				SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,$"{(isRecallGridSelected? "Recall":"Reactivation")} Emails Sent: "+sentEmailCount);
			}
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes. If gridRows is null, will make commlogs for
		///the selected rows.</summary>
		private void ProcessComms(CommItemTypeAuto commItemTypeAuto,bool isRecallGridSelected,CommItemMode commItemMode=CommItemMode.Mail,
			List<PatRowTag> listPatRowTagsSent=null) 
		{
			Cursor=Cursors.WaitCursor;
			long status;
			if(commItemMode==CommItemMode.Mail) {
				if(isRecallGridSelected) {
					status=PrefC.GetLong(PrefName.RecallStatusMailed);
				}
				else {
					status=PrefC.GetLong(PrefName.ReactivationStatusMailed);
				}
			}
			else {//Email
				if(isRecallGridSelected) {
					status=PrefC.GetLong(PrefName.RecallStatusEmailed);
				}
				else {
					status=PrefC.GetLong(PrefName.ReactivationStatusEmailed);
				}
			}
			listPatRowTagsSent=listPatRowTagsSent??_grid.SelectedTags<PatRowTag>();
			for(int i=0;i<listPatRowTagsSent.Count;i++) {
				Commlogs.InsertForRecallOrReactivation(listPatRowTagsSent[i].PatNum,commItemMode,listPatRowTagsSent[i].NumReminders,status,commItemTypeAuto);
				if(commItemTypeAuto==CommItemTypeAuto.RECALL) { //RECALL
					Recalls.UpdateStatus(listPatRowTagsSent[i].PriKeyNum,status);
					continue;
				}
				//REACTIVATION
				Reactivations.UpdateStatus(listPatRowTagsSent[i].PriKeyNum,status);
			}
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)_tableAddress.Rows.Count/30);
			Graphics g=ev.Graphics;
			float yPos=63;//75;
			float xPos=50;
			string text="";
			while(yPos<1000 && patientsPrinted<_tableAddress.Rows.Count){
				text="";
				if(DoGroupFamilies() && _tableAddress.Rows[patientsPrinted]["famList"].ToString()!=""){//print family label
					text=_tableAddress.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {//print single label
					text=_tableAddress.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				text+=_tableAddress.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				text+=_tableAddress.Rows[patientsPrinted]["City"].ToString()+", "
					+_tableAddress.Rows[patientsPrinted]["State"].ToString()+" "
					+_tableAddress.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				Rectangle rectangle=new Rectangle((int)xPos,(int)yPos,275,100);
				MapCubicle.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rectangle,new StringFormat(),g);
				//reposition for next label
				xPos+=275;
				if(xPos>850){//drop a line
					xPos=50;
					yPos+=100;
				}
				patientsPrinted++;
			}
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;//because it has to print again from the print preview
				patientsPrinted=0;
				return;
			}
			ev.HasMorePages=true;
		}
	
		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender, PrintPageEventArgs ev){
			long postCardsPerSheet=PrefC.GetLong(PrefName.ReactivationPostcardsPerSheet);
			if(IsRecallGridSelected()){
				postCardsPerSheet=PrefC.GetLong(PrefName.RecallPostcardsPerSheet);
			}
			int totalPages=(int)Math.Ceiling((double)_tableAddress.Rows.Count/(double)postCardsPerSheet);
			Graphics g=ev.Graphics;
			int yAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustDown)*100);
			int xAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustRight)*100);
			float yPos=0+yAdj;//these refer to the upper left origin of each postcard
			float xPos=0+xAdj;
			const int bottomPageMargin=100;
			long clinicNum;
			Clinic clinic;
			string str;
			while(yPos<ev.PageBounds.Height-bottomPageMargin && patientsPrinted<_tableAddress.Rows.Count){
				//Return Address--------------------------------------------------------------------------
				clinicNum=PIn.Long(_tableAddress.Rows[patientsPrinted]["ClinicNum"].ToString());
				string practicePhone=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
				if(PrefC.GetBool(PrefName.RecallCardsShowReturnAdd)){
					if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
						&& Clinics.GetClinic(clinicNum)!=null)//and this patient assigned to a clinic
					{
						clinic=Clinics.GetClinic(clinicNum);
						str=clinic.Description+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=clinic.Address+"\r\n";
						if(clinic.Address2!="") {
							str+=clinic.Address2+"\r\n";
						}
						str+=clinic.City+",  "+clinic.State+"  "+clinic.Zip+"\r\n";
						str+=TelephoneNumbers.ReFormat(clinic.Phone);
					}
					else {
						str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
						if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
							str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
						}
						str+=PrefC.GetString(PrefName.PracticeCity)+",  "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
						str+=practicePhone;
					}
					g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,xPos+45,yPos+75);
				}
				//Body text, family card ------------------------------------------------------------------
				if(DoGroupFamilies()	&& _tableAddress.Rows[patientsPrinted]["famList"].ToString()!=""){
					str=GetPostcardMessage(_tableAddress.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:true);
					str=str.Replace("[FamilyList]",_tableAddress.Rows[patientsPrinted]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					str=GetPostcardMessage(_tableAddress.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:false);
					str=str.Replace("[DueDate]",_tableAddress.Rows[patientsPrinted]["dateDue"].ToString());
					str=str.Replace("[NameF]",_tableAddress.Rows[patientsPrinted]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",_tableAddress.Rows[patientsPrinted]["patientNameFL"].ToString());
				}
				if(PrefC.HasClinicsEnabled && Clinics.GetClinic(clinicNum)!=null) {//has clinics and patient is assigned to a clinic.  
					Clinic clinicCur=Clinics.GetClinic(clinicNum);
					str=str.Replace("[ClinicName]",clinicCur.Abbr);
					str=str.Replace("[ClinicPhone]",TelephoneNumbers.ReFormat(clinicCur.Phone));
					string officePhone=clinicCur.Phone;
					if(string.IsNullOrEmpty(officePhone)) {
						officePhone=practicePhone;
					}
					str=str.Replace("[OfficePhone]",TelephoneNumbers.ReFormat(officePhone));
				}
				else {//use practice information for default. 
					str=str.Replace("[ClinicName]",PrefC.GetString(PrefName.PracticeTitle));
					str=str.Replace("[ClinicPhone]",practicePhone);
					str=str.Replace("[OfficePhone]",practicePhone);
				}
				str=str.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				str=str.Replace("[PracticePhone]",practicePhone);
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,10),Brushes.Black,new RectangleF(xPos+45,yPos+180,250,190));
				//Patient's Address-----------------------------------------------------------------------
				if(DoGroupFamilies() && _tableAddress.Rows[patientsPrinted]["famList"].ToString()!="")//print family card
				{
					str=_tableAddress.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else{//print single card
					str=_tableAddress.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				str+=_tableAddress.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				str+=_tableAddress.Rows[patientsPrinted]["City"].ToString()+", "
					+_tableAddress.Rows[patientsPrinted]["State"].ToString()+" "
					+_tableAddress.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,xPos+320,yPos+240);
				if(postCardsPerSheet==1){
					//Setting it to this value will cause it to break out of the while loop.
					yPos=ev.PageBounds.Height-bottomPageMargin;
				}
				else if(postCardsPerSheet==3){
					yPos+=366;
				}
				else{//4
					xPos+=550;
					if(xPos>1000){
						xPos=0+xAdj;
						yPos+=425;
					}
				}
				patientsPrinted++;
			}//while
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;
				patientsPrinted=0;
				return;
			}
			ev.HasMorePages=true;
		}

		///<summary>Shared functionality with Recalls, Recently Contacted, and Reactivations, be careful when making changes.</summary>
		private void butRefresh_Click(object sender, System.EventArgs e) {
			_grid.SetAll(false);
			FillCurGrid();
		}

		private void butSetStatusRecalls_Click(object sender, System.EventArgs e) {
			if(!IsAnyRowSelected()) {
				return;
			}
			long newStatus=0;
			if(comboSetStatusRecalls.SelectedIndex>0){//not None or no selection
				newStatus=comboSetStatusRecalls.GetSelected<Def>().DefNum;
			}
			gridRecalls.SelectedTags<PatRowTag>().ForEach(x => Recalls.UpdateStatus(x.PriKeyNum,newStatus));
			CommCreate(CommItemTypeAuto.RECALL,doIncludeNote:true);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoFamily_Click(object sender,EventArgs e) {
			if(!IsOneRowSelected()) {
				return;
			}
			//button does not show when Recently Contacted tab is selected.
			if(!Security.IsAuthorized(Permissions.FamilyModule)) {
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			GotoModule.GotoFamily(_patNumCur);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoAccount_Click(object sender,EventArgs e) {
			if(!IsOneRowSelected()) {
				return;
			}
			//button does not show when Recently Contacted tab is selected.
			if(!Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			GotoModule.GotoAccount(_patNumCur);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butCommlog_Click(object sender,EventArgs e) {
			if(IsRecallGridSelected()) {
				CommCreate(CommItemTypeAuto.RECALL,doIncludeNote:false);
				return;
			}
			CommCreate(CommItemTypeAuto.REACT,doIncludeNote:false);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void CommCreate(CommItemTypeAuto commItemTypeAuto,bool doIncludeNote,long defNumStatus=0) {
			if(!IsAnyRowSelected()) {
				return;
			}
			List<long> listPatNums=_grid.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList();
			//show the first one, and then make all the others very similar
			Commlog commlog=new Commlog();
			commlog.PatNum=listPatNums[0];
			commlog.CommDateTime=DateTime.Now;
			commlog.SentOrReceived=CommSentOrReceived.Sent;
			commlog.Mode_=CommItemMode.Phone;//user can change this, of course.
			commlog.CommType=Commlogs.GetTypeAuto(commItemTypeAuto);
			commlog.UserNum=Security.CurUser.UserNum;
			if(doIncludeNote) {
				commlog.Note=Lan.g(this,(commItemTypeAuto==CommItemTypeAuto.RECALL?"Recall ":"Reactivation ")+" reminder.");
				string status=Lan.g(this,"Status None");
				if(defNumStatus > 0) {
					status=Defs.GetName(DefCat.RecallUnschedStatus,defNumStatus);
				}
				else if(commItemTypeAuto==CommItemTypeAuto.RECALL && comboSetStatusRecalls.SelectedIndex>0) {//comboStatus not None
					status=comboSetStatusRecalls.GetSelected<Def>().ItemName;
				}
				else if(commItemTypeAuto==CommItemTypeAuto.REACT && comboSetStatusReact.SelectedIndex>0) {//comboReactStatus not None
					status=comboSetStatusReact.GetSelected<Def>().ItemName;
				}
				commlog.Note+="  "+status;
			}
			commlog.IsNew=true;
			using FormCommItem formCommItem=new FormCommItem(commlog);
			if(formCommItem.ShowDialog()!=DialogResult.OK) {
				FillCurGrid();
				return;
			}
			for(int i=1;i<_grid.SelectedIndices.Length;i++) {
				commlog.PatNum=listPatNums[i];
				Commlogs.Insert(commlog);
			}
			FillCurGrid();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} list printed"),PrintoutOrientation.Landscape);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
				//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} List");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				if(IsRecallGridSelected()) {
					text=datePickerRecalls.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+datePickerRecalls.GetDateTimeTo().ToShortDateString();
				}
				else {//Reactivation
					text=$"Since {datePickerReact.GetDateTimeTo()}";
				}
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
#endregion
			_grid.SetScaleAndFont(1);//Temporarily set scale to normal 96dpi for printing
			yPos=_grid.PrintPage(g,pagesPrinted,rectangleBounds,_heightHeadingPrint);
			_grid.SetScaleAndFont(LayoutManager.ScaleMy());//Change the grid back to normal zoom setting
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
				return;
			}
			e.HasMorePages=false;
		}
	
		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(listSignalods.Any(x => x.IType==InvalidType.WebSchedRecallReminders)) {
				FillRecalls();
			}
		}

		///<summary>We don't fill tabPageRecall when selected as that is the default selected tab and is filled on load.</summary>
		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			if(_grid.Columns.Count>0) {
				return;
			}
			if(IsReminderGridSelected()) {//The grid has not been initialized yet.
				FillGridReminders();
				return;
			}
			if(IsReactivationGridSelected()) {
				FillReactivationGrid();
			}
		}

		private void FillGridReminders() {
			List<Recalls.RecallRecent> listRecallsRecent=new List<Recalls.RecallRecent>();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				//If dateTimeTo is blank, default to DateTime.MaxValue.
				listRecallsRecent=Recalls.GetRecentRecalls(datePickerRemind.GetDateTimeFrom(),datePickerRemind.GetDateTimeTo(true),comboClinicRemind.ListSelectedClinicNums);
				};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			gridReminders.BeginUpdate();
			gridReminders.Columns.Clear();
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Date Time Sent"),140,GridSortingStrategy.DateParse));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Patient"),200));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Reminder Type"),180));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Age"),50,GridSortingStrategy.AmountParse));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Due Date"),100,GridSortingStrategy.DateParse));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Recall Type"),130));
			gridReminders.Columns.Add(new GridColumn(Lan.g(this,"Recall Status"),130));
			gridReminders.ListGridRows.Clear();
			for(int i=0;i<listRecallsRecent.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listRecallsRecent[i].DateSent.ToString());
				row.Cells.Add(listRecallsRecent[i].PatientName);
				row.Cells.Add(listRecallsRecent[i].ReminderType);
				row.Cells.Add(listRecallsRecent[i].Age.ToString());
				row.Cells.Add(listRecallsRecent[i].DueDate.Year<1880?"":listRecallsRecent[i].DueDate.ToShortDateString());
				row.Cells.Add(listRecallsRecent[i].RecallType);
				row.Cells.Add(listRecallsRecent[i].RecallStatus);
				row.Tag=listRecallsRecent[i];
				gridReminders.ListGridRows.Add(row);
			}
			gridReminders.EndUpdate();
		}
		private void FillReactivationGrid() {
			if(!Defs.GetDefsForCategory(DefCat.CommLogTypes).Any(x => x.ItemValue==CommItemTypeAuto.REACT.GetDescription(true))) {
				MsgBox.Show(this,Lan.g(this,"First you must set up a Reactivation commlog type in definitions"));
				return;
			}
			//Verification
			if(!datePickerReact.IsValid()) {
				return;
			}
			List<PatRowTag> listPatRowTagsSelected=gridReactivations.SelectedTags<PatRowTag>();//remember selected rows
			long clinicNum=-1;//-1 will show all patients without filtering clinics.
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinicReact.SelectedClinicNum;
			}
			long siteNum;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth) || comboSiteReact.IsAllSelected){
				siteNum=0;
			}
			else {
				siteNum=comboSiteReact.GetSelected<Site>().SiteNum;
			}
			DataTable tableReacts=new DataTable();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				//If dateTimeTo is blank, default to DateTime.MaxValue.
				tableReacts=Reactivations.GetReactivationList(datePickerReact.GetDateTimeTo(true),datePickerReact.GetDateTimeFrom(),
					checkGroupFamiliesReact.Checked,checkShowDoNotContact.Checked,!checkExcludeInactive.Checked,comboProviderReact.GetSelectedProvNum(),
					clinicNum,siteNum,comboBillingTypes.GetSelectedDefNum(),comboSortReact.GetSelected<ReactivationListSort>(),
					comboShowReactivate.GetSelected<RecallListShowNumberReminders>());
			};
			progressOD.StartingMessage=Lans.g(this,"Retrieving Reactivation List...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			gridReactivations.BeginUpdate();
			gridReactivations.Columns.Clear();
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Last Seen"),75,GridSortingStrategy.DateParse));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Patient"),90));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Age"),30,GridSortingStrategy.AmountParse));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Provider"),90));
			if(PrefC.HasClinicsEnabled) {
				gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Clinic"),75));
			}
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Site"),75));
			}
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Billing Type"),85));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"#Remind"),55,GridSortingStrategy.AmountParse));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Last Contacted"),100,GridSortingStrategy.DateParse));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Contact"),100));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Status"),80));
			gridReactivations.Columns.Add(new GridColumn(Lan.g(this,"Note"),150));
			gridReactivations.ListGridRows.Clear();
			for(int i=0;i<tableReacts.Rows.Count;i++) {
				GridRow rowNew=new GridRow();
				if(PIn.Bool(tableReacts.Rows[i]["DoNotContact"].ToString())) {
					rowNew.ColorBackG=Color.Orange;
				}
				rowNew.Cells.Add(PIn.Date(tableReacts.Rows[i]["DateLastProc"].ToString()).ToShortDateString());
				rowNew.Cells.Add(Patients.GetNameLF(tableReacts.Rows[i]["LName"].ToString(),tableReacts.Rows[i]["FName"].ToString(),tableReacts.Rows[i]["Preferred"].ToString(),tableReacts.Rows[i]["MiddleI"].ToString()));
				rowNew.Cells.Add(Patients.DateToAge(PIn.Date(tableReacts.Rows[i]["Birthdate"].ToString())).ToString());
				rowNew.Cells.Add(Providers.GetLongDesc(PIn.Long(tableReacts.Rows[i]["PriProv"].ToString())));
				if(PrefC.HasClinicsEnabled) {
					rowNew.Cells.Add(Clinics.GetDesc(PIn.Long(tableReacts.Rows[i]["ClinicNum"].ToString())));
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					rowNew.Cells.Add(Sites.GetDescription(PIn.Long(tableReacts.Rows[i]["SiteNum"].ToString())));
				}
				rowNew.Cells.Add(tableReacts.Rows[i]["BillingType"].ToString());
				rowNew.Cells.Add(tableReacts.Rows[i]["ContactedCount"].ToString());
				rowNew.Cells.Add(tableReacts.Rows[i]["DateLastContacted"].ToString());
				rowNew.Cells.Add(tableReacts.Rows[i]["ContactMethod"].ToString()); 
				long status=PIn.Long(tableReacts.Rows[i]["ReactivationStatus"].ToString());
				rowNew.Cells.Add(status>0?Defs.GetDef(DefCat.RecallUnschedStatus,status).ItemName:"");
				rowNew.Cells.Add(tableReacts.Rows[i]["ReactivationNote"].ToString());
				PatRowTag patRowTag=new PatRowTag();
				patRowTag.PatNum=PIn.Long(tableReacts.Rows[i]["PatNum"].ToString());
				patRowTag.PriKeyNum=PIn.Long(tableReacts.Rows[i]["ReactivationNum"].ToString());
				patRowTag.StatusDefNum=status;
				patRowTag.NumReminders=PIn.Int(tableReacts.Rows[i]["ContactedCount"].ToString());
				patRowTag.Email=tableReacts.Rows[i]["Email"].ToString();
				patRowTag.ContactMethodRecallPref=PIn.Enum<ContactMethod>(tableReacts.Rows[i]["PreferRecallMethod"].ToString());
				patRowTag.GuarantorNum=PIn.Long(tableReacts.Rows[i]["Guarantor"].ToString());
				patRowTag.ClinicNum=PIn.Long(tableReacts.Rows[i]["ClinicNum"].ToString());
				patRowTag.WirelessPhone=PIn.String(tableReacts.Rows[i]["WirelessPhone"].ToString());
				rowNew.Tag=patRowTag;
				gridReactivations.ListGridRows.Add(rowNew);
				if(listPatRowTagsSelected.Any(x => x.PriKeyNum==((PatRowTag)rowNew.Tag).PriKeyNum)) {
					gridReactivations.SetSelected(gridReactivations.ListGridRows.Count-1,true);
				}
			}
			gridReactivations.EndUpdate();
			labelReactPatCount.Text=Lan.g(this,"Patient Count:")+" "+gridReactivations.ListGridRows.Count.ToString();
		}

		private void gridReactivations_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PatRowTag patRowTag=(PatRowTag)gridReactivations.ListGridRows[e.Row].Tag;
			FormReactivationEdit formReactivationEdit;//disposed
			if(patRowTag.PriKeyNum==0) {//Patient has never been contacted for reactivations before.
				formReactivationEdit=new FormReactivationEdit(patRowTag.PatNum);
			}
			else {
				formReactivationEdit=new FormReactivationEdit(Reactivations.GetOne(patRowTag.PriKeyNum));
			}
			formReactivationEdit.ShowDialog();
			if(formReactivationEdit.DialogResult==DialogResult.Yes) { //indicates the reactivation status changed
				CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true,defNumStatus:formReactivationEdit.ReactivationCur.ReactivationStatus);
			}
			if(formReactivationEdit.DialogResult==DialogResult.OK || formReactivationEdit.DialogResult==DialogResult.Yes || formReactivationEdit.DialogResult==DialogResult.Abort) {
				FillReactivationGrid();
			}
			SetFamilyColors();
			formReactivationEdit?.Dispose();
		}

		private void butSetStatusReact_Click(object sender,EventArgs e) {
			long status=0;
			if(comboSetStatusReact.SelectedIndex>0){//not None or no selection
				status=comboSetStatusReact.GetSelected<Def>().DefNum;
			}
			if(!IsAnyRowSelected()) {
				return;
			}
			List<PatRowTag> listPatRowTags=gridReactivations.SelectedTags<PatRowTag>();
			for(int i=0;i<listPatRowTags.Count;i++) {
				if(listPatRowTags[i].PriKeyNum==0) { //They don't have a reactivation so create one
					Reactivations.Insert(new Reactivation() {
						PatNum=listPatRowTags[i].PatNum,
						ReactivationStatus=status
					});
				}
				else { //update the reactivation status
					Reactivations.UpdateStatus(listPatRowTags[i].PriKeyNum,status);
				}
			}
			CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true);
		}

		public List<Appointment> SchedPatReact(long patNum) {
			List<Appointment> listAppointmentsReturned=new List<Appointment>();
			if(PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)) {
				return listAppointmentsReturned;
			}
			using FormApptEdit formApptEdit=new FormApptEdit(0,patNum);
			if(formApptEdit.ShowDialog()==DialogResult.OK) {
				listAppointmentsReturned.Add(formApptEdit.GetAppointmentCur());
			}
			return listAppointmentsReturned;
		}

		public List<Appointment> SchedFamReact(Family family) {
			List<Appointment> listAppointmentsReturned=new List<Appointment>();
				for(int i=0;i<family.ListPats.Count();i++) {
				if(PatRestrictionL.IsRestricted(family.ListPats[i].PatNum,PatRestrict.ApptSchedule)) {
					MsgBox.Show(Lan.g(this,$"Skipping family member {family.ListPats[i].GetNameFirstOrPrefL()} due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule));
					continue;
				}
				listAppointmentsReturned.AddRange(SchedPatReact(family.ListPats[i].PatNum));
			}
			return listAppointmentsReturned;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsGridEmpty() {
			if(_grid.ListGridRows.Count<1) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");    
				return true;
			}
			return false;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsAnyRowSelected() {
			if(_grid.SelectedIndices.Length>0) {
				return true;
			}
			if(!IsGridEmpty() && _grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
			}
			return false;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsOneRowSelected() {
			if(!IsAnyRowSelected()) {
				return false;
			}
			if(_grid.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please select only one patient first.");
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsStatusSet(PrefName prefNameRecall,PrefName prefNameReactivation) {
			if((IsRecallGridSelected() && PrefC.GetLong(prefNameRecall)==0) || (IsReactivationGridSelected() && PrefC.GetLong(prefNameReactivation)==0)) {
				MsgBox.Show(this,$"You need to set a status first in the {(IsRecallGridSelected()?"Recall":"Reactivations")} Setup window.");
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsAnyPatToContact(string previewType,ContactMethod contactMethod,bool doAskPreview=true) {
			if(_grid.SelectedIndices.Length>0) {
				return true;
			}
			//No rows selected, try to select rows that don't have a status
			for(int i=0;i<_grid.ListGridRows.Count;i++) {
				PatRowTag patRowTag=(PatRowTag)_grid.ListGridRows[i].Tag;
				if(patRowTag.StatusDefNum!=0 //we only want rows without a status
					|| (patRowTag.ContactMethodRecallPref!=contactMethod && (contactMethod!=ContactMethod.Mail || patRowTag.ContactMethodRecallPref!=ContactMethod.None)))
				{
					continue; 
				}
				_grid.SetSelected(i,true);
			}
			if(_grid.SelectedIndices.Length==0){
				MsgBox.Show(this,$"No patients of {contactMethod.ToString()} type.");
				return false;
			}
			if(doAskPreview && !MsgBox.Show(this,MsgBoxButtons.OKCancel,$"Preview {previewType} for all of the selected patients?")) {
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private string GetPostcardMessage(string numReminders,bool isFam) {
			PrefName prefName=PrefName.ReactivationPostcardMessage;
			if(isFam){
				prefName=PrefName.ReactivationPostcardFamMsg;
			}
			if(!IsRecallGridSelected()) {
				return PrefC.GetString(prefName);
			}
			if(numReminders=="0") {
				prefName=PrefName.RecallPostcardMessage;
				if(isFam){
					prefName=PrefName.RecallPostcardFamMsg; 
				}
				return PrefC.GetString(prefName);
			}
			if(numReminders=="1") {
				prefName=PrefName.RecallPostcardMessage2;
				if(isFam){
					prefName=PrefName.RecallPostcardFamMsg2;
				}
				return PrefC.GetString(prefName);
			}
			prefName=PrefName.RecallPostcardMessage3;
			if(isFam){
				prefName=PrefName.RecallPostcardFamMsg3;
			}
			return PrefC.GetString(prefName);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butSched_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(!IsOneRowSelected()) {
				return;
			}
			Family family=Patients.GetFamily(_patNumCur);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<Appointment> listAppointmentsPinned=new List<Appointment>();
			bool isRecall=false;
			switch(((UI.Button)sender).Tag.ToString()) {
				case "SchedPatRecall":
					listAppointmentsPinned=SchedPatRecall(_grid.SelectedTag<PatRowTag>().PriKeyNum,family.GetPatient(_patNumCur),listInsSubs,listInsPlans);
					isRecall=true;
					break;
				case "SchedFamRecall":
					if(!Recalls.IsRecallProphyOrPerio(Recalls.GetRecall(_grid.SelectedTag<PatRowTag>().PriKeyNum))) {
						MsgBox.Show(this,"Only recall types of Prophy or Perio can be scheduled for families.");
						return;
					}
					listAppointmentsPinned=SchedFamRecall(family,listInsSubs,listInsPlans);
					isRecall=true;
					break;
				case "SchedPatReact":
					listAppointmentsPinned=SchedPatReact(_patNumCur);
					break;
				case "SchedFamReact":
					listAppointmentsPinned=SchedFamReact(family);
					break;
			}
			if(listAppointmentsPinned.Count<1) {
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			if(isRecall && listAppointmentsPinned[listAppointmentsPinned.Count-1].AptDateTime>=DateTime.Today) { //we're dealing with a future recall(s), so pin it and jump to it's date
				GotoModule.PinAndGoToAppt(listAppointmentsPinned.Select(x=>x.AptNum).ToList(),_patNumCur,listAppointmentsPinned[listAppointmentsPinned.Count-1].AptDateTime);
			}
			else { //it's a reactivation / it's a recall that is overdue or unsafe to jump to, so just pin it and jump to today's date
				GotoModule.PinToAppt(listAppointmentsPinned.Select(x=>x.AptNum).ToList(),_patNumCur); 
			}
			//no securitylog entry needed.  It will be made as each appt is dragged off pinboard.
			_grid.SetAll(false);
			FillCurGrid();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private DataTable GetAddrTable() {
			if(IsRecallGridSelected()) {//RECALL
				_tableAddress=Recalls.GetAddrTable(_grid.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList(),
					checkGroupFamiliesRecalls.Checked,comboSortRecalls.GetSelected<RecallListSort>());
				return _tableAddress;
			}
			//REACTIVATION
			List<Patient> listPatients=Patients.GetMultPats(_grid.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList()).ToList();
			List<Patient> listPatientGuarantors=Patients.GetMultPats(listPatients.Select(x => x.Guarantor).Distinct().ToList()).ToList();
			_tableAddress=Reactivations.GetAddrTable(listPatients,listPatientGuarantors,checkGroupFamiliesReact.Checked,comboSortReact.GetSelected<ReactivationListSort>());
			return _tableAddress;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
		
		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.
		///Contains the various properties we might need when the user wants to do an operation for a specific row or set of rows.
		///TODO in the future, use this to replace the need to use _tableRecalls in this form.</summary>
		private class PatRowTag {
			public long PatNum;
			///<summary>Can be either the recall num or the reactivation num depending on the grid.</summary>
			public long PriKeyNum;
			public long StatusDefNum;
			public int NumReminders;
			public string Email;
			public ContactMethod ContactMethodRecallPref;
			public long GuarantorNum;
			public long ClinicNum;
			public DateTime DateDue;
			public string WirelessPhone;
			public string WebSchedSendError;
			public AutoCommStatus AutoCommStatusWebSchedSend;
		}

		private void checkGroupFamilies_Click(object sender,MouseEventArgs e) {

		}
	}
	
}
