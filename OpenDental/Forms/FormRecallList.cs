/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
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
		private DataTable addrTable;
		private int patientsPrinted;
		private DataTable _tableRecalls;
		private bool headingPrinted;
		private int headingPrintH;
		private List<EmailAddress> _listEmailAddresses;
		///<summary>The clinics that are signed up for Web Sched.</summary>
		private List<long> _listClinicNumsWebSched=new List<long>();
		private ODThread _threadWebSchedSignups=null;
		///<summary>Indicates whether the Reg Key is currently on Open Dental support.</summary>
		private YN _isOnSupport=YN.Unknown;
		///<summary>The user has clicked the Web Sched button while a thread was busy checking which clinics are signed up for Web Sched.</summary>
		private bool _hasClickedWebSched;
		///<summary>A Func that can be called to get the main recall list table.</summary>
		private Func<DataTable> _getRecallTable;
		///<summary>Short list, for the two combo boxes.</summary>
		private List<Provider> _listProv;
		///<summary>Default starting offset from Date Since for Date Stop.</summary>
		private const int _reactDateStopOffset=-36;

		///<summary>True if the thread checking the clinics that are signed up for Web Sched has finished.</summary>
		private bool _isDoneCheckingWebSchedClinics {
			get { return _threadWebSchedSignups==null; }
		}

		///<summary>Each tab should have an ODGrid set as the tag, returns the grid attached to the currently selected tab.</summary>
		private GridOD _gridCur { get { return (GridOD)tabControl.SelectedTab.Tag; } }

		///<summary>Returns the PatNum of the first selected row in the corresponding grid.  Can return 0.</summary>
		private long _patNumCur {
			get {
				try {
					return _gridCur==gridReminders?_gridCur.SelectedTag<Recalls.RecallRecent>().PatNum:_gridCur.SelectedTag<PatRowTag>().PatNum;
				}
				catch(Exception e) {
					e.DoNothing();
					return 0;
				}
			}
		}

		private bool IsRecallGridSelected() {
			return _gridCur==gridRecalls;
		}

		private bool IsReactivationGridSelected() {
			return _gridCur==gridReactivations;
		}

		private bool IsReminderGridSelected() {
			return _gridCur==gridReminders;
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
				comboShowReminders.Items.Add(i==6?"6+":i.ToString(),(RecallListShowNumberReminders)(i+1));
				if(maxReactivationNums>-1 && maxReactivationNums<(i+1)) {
					continue; //pref doesn't allow us more contacts than this anyway
				}
				comboShowReactivate.Items.Add(i==6?"6+":i.ToString(),(RecallListShowNumberReminders)(i+1));
			}
			comboShowReminders.IsAllSelected=true;
			comboShowReactivate.IsAllSelected=true;
			#endregion Fill Number Reminders and Reactivations
			#region Set Recall Dates
			int daysPast=PrefC.GetInt(PrefName.RecallDaysPast);
			int daysFuture=PrefC.GetInt(PrefName.RecallDaysFuture);
			//since this is the selected tab on load the datePicker load event will fire before this load event we need to overwrite the defaults
			datePickerRecalls.SetDateTimeFrom(daysPast==-1?DateTime.MinValue:DateTime.Today.AddDays(-daysPast));
			datePickerRecalls.SetDateTimeTo(daysFuture==-1?DateTime.MinValue:DateTime.Today.AddDays(daysFuture));
			#endregion Set Recall Dates
			#region Set Reactivation Dates
			int daysSince=PrefC.GetInt(PrefName.ReactivationDaysPast);
			//set the default, datePicker load event will fire when the tab is selected and fill from and to dates with the defaults
			datePickerReact.DefaultDateTimeFrom=daysSince==-1?DateTime.MinValue:DateTime.Today.AddDays(-daysSince).AddMonths(_reactDateStopOffset);
			datePickerReact.DefaultDateTimeTo=daysSince==-1?DateTime.MinValue:DateTime.Today.AddDays(-daysSince);
			#endregion Set Reactivation Dates
			#region Set Reminder Dates
			//set the default, datePicker load event will fire when the tab is selected and fill from and to dates with the defaults
			datePickerRemind.DefaultDateTimeFrom=DateTime.Today.AddMonths(-1);
			datePickerRemind.DefaultDateTimeTo=DateTime.Today;
			#endregion Set Reminder Dates
			#region Providers
			_listProv=Providers.GetDeepCopy(isShort:true);
			comboProviderRecalls.IncludeAll=true;
			comboProviderRecalls.Items.AddList(_listProv,x => x.GetLongDesc());
			comboProviderRecalls.IsAllSelected=true;
			comboProviderReact.IncludeAll=true;
			comboProviderReact.Items.AddList(_listProv,x => x.GetLongDesc());
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
			List<Def> listRecallUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			comboSetStatusRecalls.Items.AddDefNone();
			comboSetStatusRecalls.Items.AddDefs(listRecallUnschedStatusDefs);
			comboSetStatusReact.Items.AddDefNone();
			comboSetStatusReact.Items.AddDefs(listRecallUnschedStatusDefs);
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

		private void FillCurGrid(){
			if(IsRecallGridSelected()){
				FillRecalls();
			}
			else if(IsReminderGridSelected()){
				FillGridReminders();
			}
			else if(IsReactivationGridSelected()){
				FillReactivationGrid();
			}
		}

		private void CheckClinicsSignedUpForWebSched() {
			if(_threadWebSchedSignups!=null) {
				return;
			}
			_threadWebSchedSignups=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				_listClinicNumsWebSched=WebServiceMainHQProxy.GetEServiceClinicsAllowed(
					Clinics.GetDeepCopy().Select(x => x.ClinicNum).ToList(),
					eServiceCode.WebSched);
				_isOnSupport=YN.Yes;
			}));
			//Swallow all exceptions and allow thread to exit gracefully.
			_threadWebSchedSignups.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { 	}));
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
					MessageBox.Show(Lan.g(this,"Error sending Web Sched notifications. Error message:")+" "+ex.Message);
				}
			}
			_hasClickedWebSched=false;
		}

		private void FillComboEmail() {
			//Exclude any email addresses that are associated to a clinic or are the default practice and web mail notification email addresses.
			List<long> listExcludeEmailAddressNums=Clinics.GetDeepCopy().Select(x => x.EmailAddressNum)
				.Concat(new[] { PrefC.GetLong(PrefName.EmailDefaultAddressNum),PrefC.GetLong(PrefName.EmailNotifyAddressNum) }).ToList();
			_listEmailAddresses=EmailAddresses.GetWhere(x => !listExcludeEmailAddressNums.Contains(x.EmailAddressNum));
			comboEmailFromRecalls.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFromRecalls.SelectedIndex=0;
			comboEmailFromRecalls.Items.AddRange(_listEmailAddresses.Select(x => x.EmailUsername).ToArray());
			comboEmailFromReact.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFromReact.SelectedIndex=0;
			comboEmailFromReact.Items.AddRange(_listEmailAddresses.Select(x => x.EmailUsername).ToArray());
			//Add user specific email address if present.
			EmailAddress emailAddressMe=EmailAddresses.GetForUser(Security.CurUser.UserNum);//can be null
			if(emailAddressMe!=null) {
				_listEmailAddresses.Insert(0,emailAddressMe);
				comboEmailFromRecalls.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
				comboEmailFromReact.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
			}
		}

		///<summary>Shows a progress window and fills the main Recall List grid. Goes to database.</summary>
		private void FillRecalls() {
			//Verification
			if(!datePickerRecalls.IsValid()) {
				return;
			}
			//remember which recallnums were selected
			List<PatRowTag> listSelectedRows=gridRecalls.SelectedTags<PatRowTag>();
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && !comboSiteRecalls.IsAllSelected) {
				siteNum=comboSiteRecalls.GetSelectedKey<Site>(x => x.SiteNum);
			}
			long clinicNum=PrefC.HasClinicsEnabled?comboClinicRecalls.SelectedClinicNum:-1;
			_getRecallTable=new Func<DataTable>(() => {
				//Storing this as a Func so that we can make the exact same call before sending Web Sched.
				//If dateTimeTo is blank, default to DateTime.MaxValue.
				return Recalls.GetRecallList(datePickerRecalls.GetDateTimeFrom(),datePickerRecalls.GetDateTimeTo(true),checkGroupFamiliesRecalls.Checked,
					comboProviderRecalls.GetSelectedProvNum(),clinicNum,siteNum,comboSortRecalls.GetSelected<RecallListSort>(),
					comboShowReminders.GetSelected<RecallListShowNumberReminders>(),doShowReminded:checkIncludeReminded.Checked,
					listRecallTypes:comboRecallTypes.GetListSelected<RecallType>());
			});
			Cursor=Cursors.WaitCursor;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => _tableRecalls=_getRecallTable();
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			gridRecalls.BeginUpdate();
			gridRecalls.ListGridColumns.Clear();
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.RecallList);
			GridColumn col;
			foreach(DisplayField f in fields) {
				col=new GridColumn(string.IsNullOrEmpty(f.Description)?f.InternalName:f.Description,f.ColumnWidth) { Tag=f.InternalName };
				if(ListTools.In(f.InternalName,"Due Date","LastRemind")) {
					col.SortingStrategy=GridSortingStrategy.DateParse;
					col.TextAlign=HorizontalAlignment.Center;
				}
				else if(ListTools.In(f.InternalName,"Age","#Remind")) {
					col.SortingStrategy=GridSortingStrategy.AmountParse;
					col.TextAlign=HorizontalAlignment.Right;
				}
				gridRecalls.ListGridColumns.Add(col);
			}
			gridRecalls.ListGridRows.Clear();
			GridRow row;
			List<long> listConflictingPatNums=new List<long>();
			if(checkShowConflictingTypes.Checked) {
				listConflictingPatNums=Recalls.GetConflictingPatNums(_tableRecalls.Rows.OfType<DataRow>().Select(x => PIn.Long(x["PatNum"].ToString())).ToList());
			}
			foreach(DataRow rowCur in _tableRecalls.Rows) {
				long patNum=PIn.Long(rowCur["PatNum"].ToString());
				if(checkShowConflictingTypes.Checked) {
					//If the RecallType checkbox is checked, show patients with future scheduled appointments that have conflicting recall appointments.
					//Ex. A patient is scheduled for a perio recall but their recall type is set to prophy
					if(!ListTools.In(patNum,listConflictingPatNums)) {
						//The patient does not have any conflicting recall type.
						//Continue since we don't want to show them when the RecallTypes checkbox is checked. 
						continue;
					}
					long recallTypeNum=PIn.Long(rowCur["RecallTypeNum"].ToString());
					if(!RecallTypes.IsSpecialRecallType(recallTypeNum)) {
						//Make sure recall type is Perio or Prophy
						continue;
					}
				}
				row=new GridRow();
				foreach(string internalName in fields.Select(x => x.InternalName)) {
					switch(internalName){
						case "Due Date":
							row.Cells.Add(rowCur["dueDate"].ToString());
							break;
						case "Patient":
							row.Cells.Add(rowCur["patientName"].ToString());
							break;
						case "Age":
							row.Cells.Add(rowCur["age"].ToString());
							break;
						case "Type":
							row.Cells.Add(rowCur["recallType"].ToString());
							break;
						case "Interval":
							row.Cells.Add(rowCur["recallInterval"].ToString());
							break;
						case "#Remind":
							row.Cells.Add(rowCur["numberOfReminders"].ToString());
							break;
						case "LastRemind":
							row.Cells.Add(rowCur["dateLastReminder"].ToString());
							break;
						case "Contact":
							row.Cells.Add(rowCur["contactMethod"].ToString());
							break;
						case "Status":
							row.Cells.Add(rowCur["status"].ToString());
							break;
						case "Note":
							row.Cells.Add(rowCur["Note"].ToString());
							break;
						case "BillingType":
							row.Cells.Add(rowCur["billingType"].ToString());
							break;
						case "WebSched":
							row.Cells.Add(rowCur["webSchedSendDesc"].ToString());
							break;
					}
				}
				row.Tag=new PatRowTag(
					patNum,
					PIn.Long(rowCur["RecallNum"].ToString()),
					PIn.Long(rowCur["RecallStatus"].ToString()),
					PIn.Int(rowCur["numberOfReminders"].ToString()),
					PIn.String(rowCur["Email"].ToString()),
					PIn.Enum<ContactMethod>(rowCur["PreferRecallMethod"].ToString()),
					PIn.Long(rowCur["Guarantor"].ToString()),
					PIn.Long(rowCur["ClinicNum"].ToString()),
					PIn.Date(rowCur["dueDate"].ToString()),
					PIn.String(rowCur["WirelessPhone"].ToString()),
					PIn.String(rowCur["webSchedSendError"].ToString()),
					PIn.Enum<AutoCommStatus>(rowCur["webSchedSmsSendStatus"].ToString(),defaultEnumOption:AutoCommStatus.Undefined),
					PIn.Enum<AutoCommStatus>(rowCur["webSchedEmailSendStatus"].ToString(),defaultEnumOption:AutoCommStatus.Undefined));
				gridRecalls.ListGridRows.Add(row);
				if(listSelectedRows.Any(x => x.PriKeyNum==((PatRowTag)row.Tag).PriKeyNum)) {
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
			if(_gridCur==gridReminders) {
				return; //family colors not applicable here
			}
			if(_gridCur.SelectedIndices.Length!=1) { //If we don't have a single row selected, reset colors to black
				for(int i=0;i<_gridCur.ListGridRows.Count;i++) {
					_gridCur.ListGridRows[i].ColorText=Color.Black;
				}
				_gridCur.Invalidate();
				return;
			}
			//only one row is selected so highlight family members if we can
			long guar=_gridCur.SelectedTag<PatRowTag>().GuarantorNum;
			int famCount=0;
			for(int i=0;i<_gridCur.ListGridRows.Count;i++) {
				if(((PatRowTag)_gridCur.ListGridRows[i].Tag).GuarantorNum==guar){ //family member
					famCount++;
					_gridCur.ListGridRows[i].ColorText=Color.Red;
				}
				else {
					_gridCur.ListGridRows[i].ColorText=Color.Black;
				}
			}
			if(famCount==1) {//only the highlighted patient is red at this point
				_gridCur.ListGridRows[_gridCur.SelectedIndices[0]].ColorText=Color.Black;
			}
			_gridCur.Invalidate();
		}

		private void gridRecalls_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridRecalls.ListGridColumns[e.Col].Tag.ToString()=="WebSched") {//A column's tag is its display field internal name.
				using(MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).WebSchedSendError)) {
					msgBox.Text=Lan.g(this,"Web Sched Notification Send Error");
					msgBox.ShowDialog();
					return;
				}
			}
			Recall recall=Recalls.GetRecall(((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).PriKeyNum);
			if(recall==null) {
				MsgBox.Show(this,"Recall for this patient has been removed.");
				FillRecalls();
				return;
			}
			long selectedPatNum=((PatRowTag)gridRecalls.ListGridRows[e.Row].Tag).PatNum;
			using(FormRecallEdit FormR=new FormRecallEdit { RecallCur=recall.Copy() }) {
				if(FormR.ShowDialog()!=DialogResult.OK) {
					return;
				}
				if(recall.RecallStatus!=FormR.RecallCur.RecallStatus//if the status has changed
					|| (recall.IsDisabled != FormR.RecallCur.IsDisabled)//or any of the three disabled fields was changed
					|| (recall.DisableUntilDate != FormR.RecallCur.DisableUntilDate)
					|| (recall.DisableUntilBalance != FormR.RecallCur.DisableUntilBalance)
					|| (recall.Note != FormR.RecallCur.Note))//or a note was added
				{
					//make a commlog entry
					//unless there is an existing recall commlog entry for today
					bool recallEntryToday=false;
					List<Commlog> CommlogList=Commlogs.Refresh(selectedPatNum);
					for(int i=0;i<CommlogList.Count;i++) {
						if(CommlogList[i].CommDateTime.Date==DateTime.Today	
							&& CommlogList[i].CommType==Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL)) {
							recallEntryToday=true;
						}
					}
					if(!recallEntryToday) {
						Commlog commlogCur=new Commlog();
						commlogCur.CommDateTime=DateTime.Now;
						commlogCur.Mode_=CommItemMode.Phone;//user can change this, of course.
						commlogCur.SentOrReceived=CommSentOrReceived.Sent;
						commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL);
						commlogCur.PatNum=selectedPatNum;
						commlogCur.Note="";
						if(recall.RecallStatus!=FormR.RecallCur.RecallStatus) {
							if(FormR.RecallCur.RecallStatus==0) {
								commlogCur.Note+=Lan.g(this,"Status None");
							}
							else {
								commlogCur.Note+=Defs.GetName(DefCat.RecallUnschedStatus,FormR.RecallCur.RecallStatus);
							}
						}
						if(recall.DisableUntilDate!=FormR.RecallCur.DisableUntilDate && FormR.RecallCur.DisableUntilDate.Year>1880) {
							if(commlogCur.Note!="") {
								commlogCur.Note+=",  ";
							}
							commlogCur.Note+=Lan.g(this,"Disabled until ")+FormR.RecallCur.DisableUntilDate.ToShortDateString();
						}
						if(recall.DisableUntilBalance!=FormR.RecallCur.DisableUntilBalance && FormR.RecallCur.DisableUntilBalance>0) {
							if(commlogCur.Note!="") {
								commlogCur.Note+=",  ";
							}
							commlogCur.Note+=Lan.g(this,"Disabled until balance below ")+FormR.RecallCur.DisableUntilBalance.ToString("c");
						}
						if(recall.Note!=FormR.RecallCur.Note) {
							if(commlogCur.Note!="") {
								commlogCur.Note+=",  ";
							}
							commlogCur.Note+=FormR.RecallCur.Note;
						}
						commlogCur.Note+=".  ";
						commlogCur.UserNum=Security.CurUser.UserNum;
						commlogCur.IsNew=true;
						using(FormCommItem FormCI=new FormCommItem(commlogCur)) {
							FormCI.ShowDialog();
						}
					}
				}
			}
			FillRecalls();
			gridRecalls.SetSelected(gridRecalls.ListGridRows.Select((x,i) => ((PatRowTag)x.Tag).PatNum==selectedPatNum?i:-1).Where(x => x>-1).ToArray(),true);
			SetFamilyColors();
		}

		///<summary>Creates a recall appointment and returns the AptNum in a list.  Validation should be done prior to invoking this method.
		///Shows an error message to the user and then returns an empty list if anything goes wrong.</summary>
		private List<long> SchedPatRecall(long recallNum,Patient pat,List<InsSub> subList,List<InsPlan> planList) {
			try {
				if(Recalls.HasProphyOrPerioScheduled(_patNumCur)) {
					MsgBox.Show(this,"Recall has already been scheduled.");
					FillCurGrid();
					return new List<long>();
				}
				if(PatRestrictionL.IsRestricted(pat.PatNum,PatRestrict.ApptSchedule)) {
					return new List<long>();
				}
				Appointment apt=AppointmentL.CreateRecallApt(pat,planList,recallNum,subList);
				return new List<long>() { apt.AptNum };
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return new List<long>();
			}
		}

		///<summary>Creates recall appointments for the family and returns a list of all the AptNums.
		///Validation should be done prior to invoking this method.
		///Shows a message to the user if there were any restricted patients or no appointments created.</summary>
		public List<long> SchedFamRecall(Family fam,List<InsSub> subList,List<InsPlan> planList) {
			Appointment apt;
			List<long> pinAptNums=new List<long>();
			int patsRestricted=0;
			List<Recall> listFamRecalls=Recalls.GetList(fam.ListPats.Select(x => x.PatNum).ToList());
			bool doRefreshGrid=false;
			for(int i=0;i<fam.ListPats.Length;i++) {
				if(PatRestrictionL.IsRestricted(fam.ListPats[i].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				if(Recalls.HasProphyOrPerioScheduled(fam.ListPats[i].PatNum,listFamRecalls)) {
					doRefreshGrid=true;
					continue;
				}
				try{
					//Passes in -1 as the RecallNum. This will create an appointment for either a Perio or Prophy recall type only. 
					apt=AppointmentL.CreateRecallApt(fam.ListPats[i],planList,-1,subList);
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue;
				}
				pinAptNums.Add(apt.AptNum);
			}
			if(patsRestricted>0) {
				MessageBox.Show(Lan.g(this,"Family members skipped due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			if(pinAptNums.Count==0) {
				MsgBox.Show(this,"No recall is due.");
				if(doRefreshGrid) {
					FillCurGrid();
				}
				return new List<long>();
			}
			return pinAptNums;
		}
		
		///<summary>Automatically open the eService Setup window so that they can easily click the Enable button. 
		///Calls CheckClinicsSignedUpForWebSched() before exiting.</summary>
		private void OpenSignupPortal() {
			using FormEServicesSignup formESSignup=new FormEServicesSignup();
			formESSignup.ShowDialog();
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
			if(!_isDoneCheckingWebSchedClinics) {//The thread has not finished getting the list. 
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
				if(ListTools.In(EServiceSignals.GetListenerServiceStatus(),
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
				gridRecalls.SetSelected(
					gridRecalls.ListGridRows.Select((x,i) => ListTools.In(((PatRowTag)x.Tag).RecallMethodPreferred,ContactMethod.Email,ContactMethod.TextMessage)?i:-1)
						.Where(x => x>-1).ToArray(),
					true);
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
			DataTable tableRecallsCur=_getRecallTable();
			List<long> listPatNumsInTableRecallCur=tableRecallsCur.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
			for(int i=gridRecalls.SelectedIndices.Length-1;i>=0;i--) {
				PatRowTag rowTag=gridRecalls.ListGridRows[gridRecalls.SelectedIndices[i]].Tag as PatRowTag;
				if(ListTools.In(rowTag.PatNum,listRestricted)){
					skippedRestricted++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//Check that they at least have an email or wireless phone.
				if(string.IsNullOrEmpty(rowTag.Email.Trim()) && string.IsNullOrEmpty(rowTag.WirelessPhone.Trim())) {
					skippedContact++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//If this practice has clinics enabled for Web Sched, then they will not have Web Sched enabled for clinic num 0. They will need to assign
				//patients to a clinic in order to send Web Sched. We will prompt them below to assign them.
				if(needsToBeSignedUp && rowTag.ClinicNum==0 && !_listClinicNumsWebSched.Contains(0)) {
					skippedClinic0++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//Check that the patient is still in the recall list (they haven't been sent something since the grid has been refreshed)
				if(!ListTools.In(rowTag.PatNum,listPatNumsInTableRecallCur)) {
					skippedNotInList++;
					gridRecalls.SetSelected(gridRecalls.SelectedIndices[i],false);
					continue;
				}
				//The eConnector will attempt to send a webschedrecall if either SmsSendStatus or EmailSendStatus is SendNotAttempted and neither of them
				//are SendFailed or SendSuccessful.
				if(new[] { rowTag.WebSchedSmsSendStatus,rowTag.WebSchedEmailSendStatus }.Any(x => x==AutoCommStatus.SendNotAttempted)
					&& new[] { rowTag.WebSchedSmsSendStatus,rowTag.WebSchedEmailSendStatus }.All(x => ListTools.In(x,AutoCommStatus.SendNotAttempted,AutoCommStatus.DoNotSend)))
				{
					continue;//The eConnector is about to send this anyway.
				}
				//Check to see if they'll have any potential time slots via their Web Sched link.
				DateTime dateDue=rowTag.DateDue.Date<DateTime.Now.Date?DateTime.Now:rowTag.DateDue;
				//This takes a long time to run for lots of recalls.  Might consider making a faster overload in the future (213 recalls ~ 10 seconds).
				bool hasTimeSlots=false;
				try {
					if(needsToBeSignedUp) {
						hasTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(rowTag.PriKeyNum,dateDue,dateDue.AddMonths(2)).Count>0;
					}
					else {
						hasTimeSlots=true;
					}
				}
				catch(Exception) {
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
			List<long> recallNums=gridRecalls.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList();
			EmailAddress emailAddressFrom=null;//clinic/practice default if selected index==0, pull patient's clinic email address
			if(comboEmailFromRecalls.SelectedIndex>0) {//me or static email address, email address for 'me' is the first one in _listEmailAddresses
				emailAddressFrom=_listEmailAddresses[comboEmailFromRecalls.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" items in combobox
			}
			List<string> listWebSchedErrors=Recalls.PrepWebSchedNotifications(recallNums,
				checkGroupFamiliesRecalls.Checked,
				comboSortRecalls.GetSelected<RecallListSort>(),
				WebSchedRecallSource.FormRecallList,
				emailAddressFrom);
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.WebSchedRecallManualSend,0,Lan.g(this,"Web Sched Recalls manually sent."));
			if(listWebSchedErrors.Count>0) {
				//Show the error (already translated) to the user and then refresh the grid in case any were successful.
				using(MsgBoxCopyPaste msgBCP=new MsgBoxCopyPaste(string.Join("\r\n",listWebSchedErrors))) {
					msgBCP.Show();
				}
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
      using(FormRpRecall FormRPR=new FormRpRecall(recallNums)) {
	      FormRPR.ShowDialog();
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabels_Click(object sender, System.EventArgs e) {
			if(!IsGridEmpty() && IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed) && IsAnyPatToContact("labels",ContactMethod.Mail)) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				pagesPrinted=0;
				patientsPrinted=0;
				PrinterL.TryPreview(pdLabels_PrintPage,
					Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list labels printed"),
					PrintSituation.LabelSheet,
					new Margins(0,0,0,0),
					PrintoutOrigin.AtMargin,
					totalPages:(int)Math.Ceiling((double)addrTable.Rows.Count/30)
				);
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change statuses and make commlog entries for all of the selected patients?")) {
					ProcessComms(commType,IsRecallGridSelected());
				}
				FillCurGrid();
				Cursor=Cursors.Default;
			}
		}		

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabelOne_Click(object sender,EventArgs e) {
			if(IsAnyRowSelected()) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				patientsPrinted=0;
				string text;
				while(patientsPrinted<addrTable.Rows.Count) {
					text="";
					if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!="") {//print family label
						text=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
					}
					else {//print single label
						text=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
					}
					text+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
					text+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
						+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
						+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
					LabelSingle.PrintText(0,text);
					patientsPrinted++;
				}
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the labels finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if labels printed successfully.")) {
					ProcessComms(commType,IsRecallGridSelected());
				}
				FillCurGrid();
				Cursor=Cursors.Default;
			}
		}

		///<summary>Changes made to printing recall postcards need to be made in FormConfirmList.butPostcards_Click() as well.
		///Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPostcards_Click(object sender,System.EventArgs e) {
			if(!IsGridEmpty() && IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed) && IsAnyPatToContact("postcards",ContactMethod.Mail)) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				pagesPrinted=0;
				patientsPrinted=0;
				PaperSize paperSize;
				PrintoutOrientation orient=PrintoutOrientation.Default;
				long postcardsPerSheet=PrefC.GetLong(commType==CommItemTypeAuto.RECALL?PrefName.RecallPostcardsPerSheet:PrefName.ReactivationPostcardsPerSheet);
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
				int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/(double)postcardsPerSheet);
				PrinterL.TryPreview(pdCards_PrintPage,
					Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list postcards printed"),
					PrintSituation.Postcard,
					new Margins(0,0,0,0),
					PrintoutOrigin.AtMargin,
					paperSize,
					orient,
					totalPages
				);
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the postcards finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if postcards printed successfully.")) {
					ProcessComms(commType,IsRecallGridSelected());
				}
				FillCurGrid();
				Cursor=Cursors.Default;
			}
		}

		private void butUndo_Click(object sender,EventArgs e) {
			using FormRecallListUndo form=new FormRecallListUndo();
			form.ShowDialog();
			if(form.DialogResult==DialogResult.OK) {
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
					using FormProgramLinkEdit FormPE=new FormProgramLinkEdit();
					FormPE.ProgramCur=Programs.GetCur(ProgramName.Divvy);
					FormPE.ShowDialog();
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
				gridRecalls.SetSelected(
					gridRecalls.ListGridRows
						.Select((x,i) => ListTools.In(((PatRowTag)x.Tag).RecallMethodPreferred,ContactMethod.Mail,ContactMethod.None)?i:-1)
						.Where(x => x>-1).ToArray(),
					true);
			}
			if(gridRecalls.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No patients of mail type.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send postcards for all of the selected patients?")) {
				return;
			}
			List<long> recallNums=gridRecalls.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList();
			addrTable=Recalls.GetAddrTable(recallNums,checkGroupFamiliesRecalls.Checked,comboSortRecalls.GetSelected<RecallListSort>());
			DivvyConnect.Postcard postcard;
			DivvyConnect.Recipient recipient;
			DivvyConnect.Postcard[] listPostcards=new DivvyConnect.Postcard[gridRecalls.SelectedIndices.Length];
			string message;
			long clinicNum;
			Clinic clinic;
			string phone;
			for(int i=0;i<addrTable.Rows.Count;i++) {
				postcard=new DivvyConnect.Postcard();
				recipient=new DivvyConnect.Recipient();
				recipient.Name=addrTable.Rows[i]["patientNameFL"].ToString();
				recipient.ExternalRecipientID=addrTable.Rows[i]["patNums"].ToString();
				recipient.Address1=addrTable.Rows[i]["Address"].ToString();//Includes Address2
				recipient.City=addrTable.Rows[i]["City"].ToString();
				recipient.State=addrTable.Rows[i]["State"].ToString();
				recipient.Zip=addrTable.Rows[i]["Zip"].ToString();
				postcard.AppointmentDateTime=PIn.Date(addrTable.Rows[i]["dateDue"].ToString());//js I don't know why they would ask for this.  We put this in our message.
				//Body text, family card ------------------------------------------------------------------
				if(checkGroupFamiliesRecalls.Checked	&& addrTable.Rows[i]["famList"].ToString()!=""){
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg3);
					}
					message=message.Replace("[FamilyList]",addrTable.Rows[i]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardMessage3);
					}
					message=message.Replace("[DueDate]",addrTable.Rows[i]["dateDue"].ToString());
					message=message.Replace("[NameF]",addrTable.Rows[i]["patientNameF"].ToString());
					message=message.Replace("[NameFL]", addrTable.Rows[i]["patientNameFL"].ToString());
				}
				Clinic clinicCur=Clinics.GetClinicForRecall(PIn.Long(addrTable.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
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
			clinicNum=PIn.Long(addrTable.Rows[patientsPrinted]["ClinicNum"].ToString());
			if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
				&& Clinics.GetClinic(clinicNum)!=null)//and this patient assigned to a clinic
			{
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
			DivvyConnect.PostcardServiceClient client=new DivvyConnect.PostcardServiceClient();
			DivvyConnect.PostcardReturnMessage returnMessage=new DivvyConnect.PostcardReturnMessage();
			string messages="";
			Cursor=Cursors.WaitCursor;
			try {
				string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(ProgramName.Divvy,"Password"));
				returnMessage=client.SendPostcards(
				  Guid.Parse(ProgramProperties.GetPropVal(ProgramName.Divvy,"API Key")),
				  ProgramProperties.GetPropVal(ProgramName.Divvy,"Username"),
				  password,
				  listPostcards,practice);
			}
			catch (Exception ex) {
				messages+="Exception: "+ex.Message+"\r\nData: "+ex.Data+"\r\n";
			}
			messages+="MessageCode: "+returnMessage.MessageCode.ToString();//MessageCode enum. 0=CompletedSuccessfully, 1=CompletedWithErrors, 2=Failure
			MsgBox.Show(this,"Return Messages: "+returnMessage.Message+"\r\n"+messages);
			if(returnMessage.MessageCode==DivvyConnect.MessageCode.CompletedSucessfully) {
				Cursor=Cursors.WaitCursor;
				ProcessComms((IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT),IsRecallGridSelected(),CommItemMode.Mail);
			}
			else if(returnMessage.MessageCode==DivvyConnect.MessageCode.CompletedWithErrors) {
				for(int i=0;i<returnMessage.PostcardMessages.Length;i++) {
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
			for(int i=_gridCur.SelectedIndices.Length-1;i>=0;i--) {
				if(!EmailAddresses.IsValidEmail(_gridCur.SelectedTags<PatRowTag>()[i].Email)) {
					skipped++;
					_gridCur.SetSelected(_gridCur.SelectedIndices[i],false);
				}
			}
			if(_gridCur.SelectedIndices.Length==0){
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
			addrTable=GetAddrTable();
			//Email
			EmailMessage message;
			string str="";
			EmailAddress emailAddress;
			int sentEmailCount=0;
			//Capturing these variables now because SendEmailUnsecure can call Application.DoEvents which means the user can change the selected grid.
			bool isRecallGridSelected=IsRecallGridSelected();
			GridOD gridCur=_gridCur;
			bool doGroupFamilies=DoGroupFamilies();
			List<PatRowTag> listRowsSent=new List<PatRowTag>();
			for(int i=0;i<addrTable.Rows.Count;i++){
				message=new EmailMessage();
				message.PatNum=PIn.Long(addrTable.Rows[i]["emailPatNum"].ToString());
				message.ToAddress=PIn.String(addrTable.Rows[i]["email"].ToString());//might be guarantor email
				Clinic clinicCur;
				if(isRecallGridSelected) {
					clinicCur=Clinics.GetClinicForRecall(PIn.Long(addrTable.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
				}
				else {
					clinicCur=Clinics.GetClinic(PIn.Long(addrTable.Rows[i]["ClinicNum"].ToString()));
				}
				long clinicNumEmail=clinicCur?.ClinicNum??Clinics.ClinicNum;
				ComboBox cbEmail=isRecallGridSelected?comboEmailFromRecalls:comboEmailFromReact;
				if(cbEmail.SelectedIndex==0) { //clinic/practice default
					clinicNumEmail=PIn.Long(addrTable.Rows[i]["ClinicNum"].ToString());
					emailAddress=EmailAddresses.GetByClinic(clinicNumEmail);
				}
				else { //me or static email address, email address for 'me' is the first one in _listEmailAddresses
					emailAddress=_listEmailAddresses[cbEmail.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" item in combobox
				}
				message.FromAddress=emailAddress.GetFrom();
				if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
					message.Subject=PrefC.GetString(isRecallGridSelected? PrefName.RecallEmailSubject:PrefName.ReactivationEmailSubject);
				}
				else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
					message.Subject=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailSubject2:PrefName.ReactivationEmailSubject);
				}
				else {
					message.Subject=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailSubject3:PrefName.ReactivationEmailSubject);
				}
				//family
				if(doGroupFamilies && addrTable.Rows[i]["famList"].ToString()!="") {
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailFamMsg:PrefName.ReactivationEmailFamMsg);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailFamMsg2:PrefName.ReactivationEmailFamMsg);
					}
					else {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailFamMsg3:PrefName.ReactivationEmailFamMsg);
					}
					str=str.Replace("[FamilyList]",addrTable.Rows[i]["famList"].ToString());
				}
				//single
				else {
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailMessage:PrefName.ReactivationEmailMessage);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailMessage2:PrefName.ReactivationEmailMessage);
					}
					else {
						str=PrefC.GetString(isRecallGridSelected ? PrefName.RecallEmailMessage3:PrefName.ReactivationEmailMessage);
					}
					str=str.Replace("[DueDate]",PIn.Date(addrTable.Rows[i]["dateDue"].ToString()).ToShortDateString());
					str=str.Replace("[NameF]",addrTable.Rows[i]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",addrTable.Rows[i]["patientNameFL"].ToString());
				}
				string officePhone="";
				string mainPhone=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
				if(clinicCur==null) {
					str=str.Replace("[ClinicName]",PrefC.GetString(PrefName.PracticeTitle));
					str=str.Replace("[ClinicPhone]",mainPhone);
					officePhone=mainPhone;
				}
				else {
					str=str.Replace("[ClinicName]",clinicCur.Abbr);
					str=str.Replace("[ClinicPhone]",TelephoneNumbers.ReFormat(clinicCur.Phone));
					officePhone=clinicCur.Phone;
				}
				str=str.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				str=str.Replace("[PracticePhone]",mainPhone);
				str=str.Replace("[OfficePhone]",officePhone);
				message.BodyText=EmailMessages.FindAndReplacePostalAddressTag(str,clinicNumEmail);
				message.MsgDateTime=DateTime.Now;
				message.SentOrReceived=EmailSentOrReceived.Sent;
				message.MsgType=EmailMessageSource.Recall;
				try{
					EmailMessages.SendEmail(message,emailAddress);
					sentEmailCount++;
				}
				catch(Exception ex){
					Cursor=Cursors.Default;
					str=ex.Message+"\r\n";
					if(ex.GetType()==typeof(System.ArgumentException)){
						str+=$"Go to Setup, {(isRecallGridSelected ? "Recall":"Reactivation")}.  The subject for an email may not be multiple lines.\r\n";
					}
					MessageBox.Show(str+"Patient:"+addrTable.Rows[i]["patientNameFL"].ToString());
					break;
				}
				//Add current row to the list of rows sent.
				if(isRecallGridSelected) {
					List<long> listRecallNums=addrTable.Rows[i]["recallNums"].ToString().Split(',').Select(x => PIn.Long(x)).ToList();
					for(int j=0;j<gridCur.ListGridRows.Count;j++) {
						if(ListTools.In(((PatRowTag)gridCur.ListGridRows[j].Tag).PriKeyNum,listRecallNums)) {
							listRowsSent.Add((PatRowTag)gridCur.ListGridRows[j].Tag);
						}
					}
				}
				else {//Reactivation
					List<long> listPatNums=addrTable.Rows[i]["patNums"].ToString().Split(',').Select(x => PIn.Long(x)).ToList();
					for(int j=0;j<gridCur.ListGridRows.Count;j++) {
						if(ListTools.In(((PatRowTag)gridCur.ListGridRows[j].Tag).PatNum,listPatNums)) {
							listRowsSent.Add((PatRowTag)gridCur.ListGridRows[j].Tag);
						}
					}
				}
			}
			ProcessComms(isRecallGridSelected ? CommItemTypeAuto.RECALL : CommItemTypeAuto.REACT,isRecallGridSelected,CommItemMode.Email,listRowsSent);
			FillCurGrid();
			if(sentEmailCount>0) {
				SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,$"{(isRecallGridSelected? "Recall":"Reactivation")} Emails Sent: "+sentEmailCount);
			}
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes. If gridRows is null, will make commlogs for
		///the selected rows.</summary>
		private void ProcessComms(CommItemTypeAuto commType,bool isRecallGridSelected,CommItemMode mode=CommItemMode.Mail,
			List<PatRowTag> listSentRows=null) 
		{
			Cursor=Cursors.WaitCursor;
			long status;
			if(mode==CommItemMode.Mail) {
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
			listSentRows=listSentRows??_gridCur.SelectedTags<PatRowTag>();
			foreach(PatRowTag tag in listSentRows) {
				Commlogs.InsertForRecallOrReactivation(tag.PatNum,mode,tag.NumReminders,status,commType);
				if(commType==CommItemTypeAuto.RECALL) { //RECALL
					Recalls.UpdateStatus(tag.PriKeyNum,status);
				}
				else { //REACTIVATION
					Reactivations.UpdateStatus(tag.PriKeyNum,status);
				}
			}
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/30);
			Graphics g=ev.Graphics;
			float yPos=63;//75;
			float xPos=50;
			string text="";
			while(yPos<1000 && patientsPrinted<addrTable.Rows.Count){
				text="";
				if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!=""){//print family label
					text=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {//print single label
					text=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				text+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				text+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
					+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
					+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				Rectangle rect=new Rectangle((int)xPos,(int)yPos,275,100);
				MapAreaRoomControl.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rect,new StringFormat(),g);
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
			}
			else{
				ev.HasMorePages=true;
			}
			g.Dispose();
		}
	
		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender, PrintPageEventArgs ev){
			long postCardsPerSheet=PrefC.GetLong(IsRecallGridSelected()?PrefName.RecallPostcardsPerSheet:PrefName.ReactivationPostcardsPerSheet);
			int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/(double)postCardsPerSheet);
			Graphics g=ev.Graphics;
			int yAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustDown)*100);
			int xAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustRight)*100);
			float yPos=0+yAdj;//these refer to the upper left origin of each postcard
			float xPos=0+xAdj;
			const int bottomPageMargin=100;
			long clinicNum;
			Clinic clinic;
			string str;
			while(yPos<ev.PageBounds.Height-bottomPageMargin && patientsPrinted<addrTable.Rows.Count){
				//Return Address--------------------------------------------------------------------------
				clinicNum=PIn.Long(addrTable.Rows[patientsPrinted]["ClinicNum"].ToString());
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
				if(DoGroupFamilies()	&& addrTable.Rows[patientsPrinted]["famList"].ToString()!=""){
					str=GetPostcardMessage(addrTable.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:true);
					str=str.Replace("[FamilyList]",addrTable.Rows[patientsPrinted]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					str=GetPostcardMessage(addrTable.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:false);
					str=str.Replace("[DueDate]",addrTable.Rows[patientsPrinted]["dateDue"].ToString());
					str=str.Replace("[NameF]",addrTable.Rows[patientsPrinted]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",addrTable.Rows[patientsPrinted]["patientNameFL"].ToString());
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
				if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!="")//print family card
				{
					str=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else{//print single card
					str=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				str+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				str+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
					+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
					+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
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
			}
			else{
				ev.HasMorePages=true;
			}
		}

		///<summary>Shared functionality with Recalls, Recently Contacted, and Reactivations, be careful when making changes.</summary>
		private void butRefresh_Click(object sender, System.EventArgs e) {
			_gridCur.SetAll(false);
			FillCurGrid();
		}

		private void butSetStatusRecalls_Click(object sender, System.EventArgs e) {
			if(IsAnyRowSelected()) {
				long newStatus=0;
				if(comboSetStatusRecalls.SelectedIndex>0){//not None or no selection
					newStatus=comboSetStatusRecalls.GetSelected<Def>().DefNum;
				}
				gridRecalls.SelectedTags<PatRowTag>().ForEach(tag => Recalls.UpdateStatus(tag.PriKeyNum,newStatus));
				CommCreate(CommItemTypeAuto.RECALL,doIncludeNote:true);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoFamily_Click(object sender,EventArgs e) {
			//button does not show when Recently Contacted tab is selected.
			if(IsOneRowSelected()) {
				if(!Security.IsAuthorized(Permissions.FamilyModule)) {
					return;
				}
				ShrinkWindowBeforeMinMax();
				WindowState=FormWindowState.Minimized;
				GotoModule.GotoFamily(_patNumCur);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoAccount_Click(object sender,EventArgs e) {
			//button does not show when Recently Contacted tab is selected.
			if(IsOneRowSelected()) {
				if(!Security.IsAuthorized(Permissions.AccountModule)) {
					return;
				}
				ShrinkWindowBeforeMinMax();
				WindowState=FormWindowState.Minimized;
				GotoModule.GotoAccount(_patNumCur);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butCommlog_Click(object sender,EventArgs e) {
			CommCreate(IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT,doIncludeNote:false);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void CommCreate(CommItemTypeAuto commType,bool doIncludeNote) {
			if(IsAnyRowSelected()) {
				List<long> listPatNums=_gridCur.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList();
				//show the first one, and then make all the others very similar
				Commlog commlogCur=new Commlog();
				commlogCur.PatNum=listPatNums[0];
				commlogCur.CommDateTime=DateTime.Now;
				commlogCur.SentOrReceived=CommSentOrReceived.Sent;
				commlogCur.Mode_=CommItemMode.Phone;//user can change this, of course.
				commlogCur.CommType=Commlogs.GetTypeAuto(commType);
				commlogCur.UserNum=Security.CurUser.UserNum;
				if(doIncludeNote) {
					commlogCur.Note=Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall ":"Reactivation ")+" reminder.");
					if(commType==CommItemTypeAuto.RECALL && comboSetStatusRecalls.SelectedIndex>0) {//comboStatus not None
						commlogCur.Note+="  "+comboSetStatusRecalls.GetSelected<Def>().ItemName;
					}
					else if(commType==CommItemTypeAuto.REACT && comboSetStatusReact.SelectedIndex>0) {//comboReactStatus not None
						commlogCur.Note+="  "+comboSetStatusReact.GetSelected<Def>().ItemName;
					}
					else{
						commlogCur.Note+="  "+Lan.g(this,"Status None");
					}
				}
				commlogCur.IsNew=true;
				using FormCommItem FormCI=new FormCommItem(commlogCur);
				if(FormCI.ShowDialog()!=DialogResult.OK) {
					FillCurGrid();
					return;
				}
				for(int i=1;i<_gridCur.SelectedIndices.Length;i++) {
					commlogCur.PatNum=listPatNums[i];
					Commlogs.Insert(commlogCur);
				}
				FillCurGrid();
			}
		}		

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} list printed"),PrintoutOrientation.Landscape);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
				//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(IsRecallGridSelected()) {
					text=datePickerRecalls.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+datePickerRecalls.GetDateTimeTo().ToShortDateString();
				}
				else {//Reactivation
					text=$"Since {datePickerReact.GetDateTimeTo()}";
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
#endregion
			_gridCur.ScaleMy=1;//Temporarily set scale to normal 96dpi for printing
			yPos=_gridCur.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			_gridCur.ScaleMy=LayoutManager.ScaleMy();//Change the grid back to normal zoom setting
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}
	
		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Any(x => x.IType==InvalidType.WebSchedRecallReminders)) {
				FillRecalls();
			}
		}

		///<summary>We don't fill tabPageRecall when selected as that is the default selected tab and is filled on load.</summary>
		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			if(_gridCur.ListGridColumns.Count>0) {
				return;
			}
			if(IsReminderGridSelected()) {//The grid has not been initialized yet.
				FillGridReminders();
			}
			else if(IsReactivationGridSelected()) {
				FillReactivationGrid();
			}
		}

		private void FillGridReminders() {
			List<Recalls.RecallRecent> listRecent=new List<Recalls.RecallRecent>();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				//If dateTimeTo is blank, default to DateTime.MaxValue.
				listRecent=Recalls.GetRecentRecalls(datePickerRemind.GetDateTimeFrom(),datePickerRemind.GetDateTimeTo(true),comboClinicRemind.ListSelectedClinicNums);
				};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			gridReminders.BeginUpdate();
			gridReminders.ListGridColumns.Clear();
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date Time Sent"),140,GridSortingStrategy.DateParse));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),200));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Reminder Type"),180));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Age"),50,GridSortingStrategy.AmountParse));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Due Date"),100,GridSortingStrategy.DateParse));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Recall Type"),130));
			gridReminders.ListGridColumns.Add(new GridColumn(Lan.g(this,"Recall Status"),130));
			gridReminders.ListGridRows.Clear();
			gridReminders.ListGridRows.AddRange(listRecent.Select(x => new GridRow(
				x.DateSent.ToString(),
				x.PatientName,
				x.ReminderType,
				x.Age.ToString(),
				x.DueDate.Year<1880?"":x.DueDate.ToShortDateString(),
				x.RecallType,
				x.RecallStatus) { Tag=x }));
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
			List<PatRowTag> listSelectedRows=gridReactivations.SelectedTags<PatRowTag>();//remember selected rows
			long clinicNum=PrefC.HasClinicsEnabled?comboClinicReact.SelectedClinicNum:-1;//-1 will show all patients without filtering clinics.
			long siteNum=(PrefC.GetBool(PrefName.EasyHidePublicHealth) || comboSiteReact.IsAllSelected)?0:comboSiteReact.GetSelected<Site>().SiteNum;
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
			gridReactivations.ListGridColumns.Clear();
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last Seen"),75,GridSortingStrategy.DateParse));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),90));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Age"),30,GridSortingStrategy.AmountParse));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Provider"),90));
			if(PrefC.HasClinicsEnabled) {
				gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Clinic"),75));
			}
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Site"),75));
			}
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Billing Type"),85));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"#Remind"),55,GridSortingStrategy.AmountParse));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last Contacted"),100,GridSortingStrategy.DateParse));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Contact"),100));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),80));
			gridReactivations.ListGridColumns.Add(new GridColumn(Lan.g(this,"Note"),150));
			gridReactivations.ListGridRows.Clear();
			foreach(DataRow row in tableReacts.Rows) {
				GridRow rowNew=new GridRow();
				if(PIn.Bool(row["DoNotContact"].ToString())) {
					rowNew.ColorBackG=Color.Orange;
				}
				rowNew.Cells.Add(PIn.Date(row["DateLastProc"].ToString()).ToShortDateString());
				rowNew.Cells.Add(Patients.GetNameLF(row["LName"].ToString(),row["FName"].ToString(),row["Preferred"].ToString(),row["MiddleI"].ToString()));
				rowNew.Cells.Add(Patients.DateToAge(PIn.Date(row["Birthdate"].ToString())).ToString());
				rowNew.Cells.Add(Providers.GetLongDesc(PIn.Long(row["PriProv"].ToString())));
				if(PrefC.HasClinicsEnabled) {
					rowNew.Cells.Add(Clinics.GetDesc(PIn.Long(row["ClinicNum"].ToString())));
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					rowNew.Cells.Add(Sites.GetDescription(PIn.Long(row["SiteNum"].ToString())));
				}
				rowNew.Cells.Add(row["BillingType"].ToString());
				rowNew.Cells.Add(row["ContactedCount"].ToString());
				rowNew.Cells.Add(row["DateLastContacted"].ToString());
				rowNew.Cells.Add(row["ContactMethod"].ToString()); 
				long status=PIn.Long(row["ReactivationStatus"].ToString());
				rowNew.Cells.Add(status>0?Defs.GetDef(DefCat.RecallUnschedStatus,status).ItemName:"");
				rowNew.Cells.Add(row["ReactivationNote"].ToString());
				rowNew.Tag=new PatRowTag(
					PIn.Long(row["PatNum"].ToString()),
					PIn.Long(row["ReactivationNum"].ToString()),
					status,
					PIn.Int(row["ContactedCount"].ToString()),
					row["Email"].ToString(),
					PIn.Enum<ContactMethod>(row["PreferRecallMethod"].ToString()),
					PIn.Long(row["Guarantor"].ToString()),
					PIn.Long(row["ClinicNum"].ToString()),
					wirelessPhone:PIn.String(row["WirelessPhone"].ToString()));
				gridReactivations.ListGridRows.Add(rowNew);
				if(listSelectedRows.Any(x => x.PriKeyNum==((PatRowTag)rowNew.Tag).PriKeyNum)) {
					gridReactivations.SetSelected(gridReactivations.ListGridRows.Count-1,true);
				}
			}
			gridReactivations.EndUpdate();
			labelReactPatCount.Text=Lan.g(this,"Patient Count:")+" "+gridReactivations.ListGridRows.Count.ToString();
		}

		private void gridReactivations_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PatRowTag tag=(PatRowTag)gridReactivations.ListGridRows[e.Row].Tag;
			FormReactivationEdit formRE;
			if(tag.PriKeyNum==0) {//Patient has never been contacted for reactivations before.
				formRE=new FormReactivationEdit(tag.PatNum);
			}
			else {
				formRE=new FormReactivationEdit(Reactivations.GetOne(tag.PriKeyNum));
			}
			formRE.ShowDialog();
			if(formRE.DialogResult==DialogResult.Yes) { //indicates the reactivation status changed
				CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true);
			}
			if(formRE.DialogResult==DialogResult.OK || formRE.DialogResult==DialogResult.Yes || formRE.DialogResult==DialogResult.Abort) {
				FillReactivationGrid();
			}
			SetFamilyColors();
			formRE.Dispose();
		}

		private void butSetStatusReact_Click(object sender,EventArgs e) {
			long status=0;
			if(comboSetStatusReact.SelectedIndex>0){//not None or no selection
				status=comboSetStatusReact.GetSelected<Def>().DefNum;
			}
			if(IsAnyRowSelected()) {
				foreach(PatRowTag tag in gridReactivations.SelectedTags<PatRowTag>()) {
					if(tag.PriKeyNum==0) { //They don't have a reactivation so create one
						Reactivations.Insert(new Reactivation() {
							PatNum=tag.PatNum,
							ReactivationStatus=status
						});
					}
					else { //update the reactivation status
						Reactivations.UpdateStatus(tag.PriKeyNum,status);
					}
				}
				CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true);
			}
		}

		public List<long> SchedPatReact(long patNum) {
			List<long> listRet=new List<long>();
			if(PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)) {
				return listRet;
			}
			using(FormApptEdit formAE=new FormApptEdit(0,patNum)) {
				if(formAE.ShowDialog()==DialogResult.OK) {
					listRet.Add(formAE.GetAppointmentCur().AptNum);
				}
			}
			return listRet;
		}

		public List<long> SchedFamReact(Family fam) {
			List<long> listRet=new List<long>();
			foreach(Patient pat in fam.ListPats) {
				if(PatRestrictionL.IsRestricted(pat.PatNum,PatRestrict.ApptSchedule)) {
					MsgBox.Show(Lan.g(this,$"Skipping family member {pat.GetNameFirstOrPrefL()} due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule));
					continue;
				}
				listRet.AddRange(SchedPatReact(pat.PatNum));
			}
			return listRet;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsGridEmpty() {
			if(_gridCur.ListGridRows.Count<1) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");    
        return true;
      }
			return false;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsAnyRowSelected() {
			if(_gridCur.SelectedIndices.Length>0) {
				return true;
			}
			if(!IsGridEmpty() && _gridCur.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
			}
			return false;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsOneRowSelected() {
			if(!IsAnyRowSelected()) {
				return false;
			}
			if(_gridCur.SelectedIndices.Length>1) {
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
		private bool IsAnyPatToContact(string previewType,ContactMethod method,bool doAskPreview=true) {
			if(_gridCur.SelectedIndices.Length>0) {
				return true;
			}
			//No rows selected, try to select rows that don't have a status
			for(int i=0;i<_gridCur.ListGridRows.Count;i++) {
				PatRowTag tag=(PatRowTag)_gridCur.ListGridRows[i].Tag;
				if(tag.StatusDefNum!=0 //we only want rows without a status
					|| (tag.RecallMethodPreferred!=method && (method!=ContactMethod.Mail || tag.RecallMethodPreferred!=ContactMethod.None)))
				{
					continue; 
				}
				_gridCur.SetSelected(i,true);
			}
			if(_gridCur.SelectedIndices.Length==0){
				MsgBox.Show(this,$"No patients of {method.ToString()} type.");
				return false;
			}
			if(doAskPreview && !MsgBox.Show(this,MsgBoxButtons.OKCancel,$"Preview {previewType} for all of the selected patients?")) {
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private string GetPostcardMessage(string numReminders,bool isFam) {
			PrefName prefName=isFam?PrefName.ReactivationPostcardFamMsg:PrefName.ReactivationPostcardMessage;
			if(IsRecallGridSelected()) {
				if(numReminders=="0") {
					prefName=isFam?PrefName.RecallPostcardFamMsg:PrefName.RecallPostcardMessage;
				}
				else if(numReminders=="1") {
					prefName=isFam?PrefName.RecallPostcardFamMsg2:PrefName.RecallPostcardMessage2;
				}
				else {
					prefName=isFam?PrefName.RecallPostcardFamMsg3:PrefName.RecallPostcardMessage3;
				}
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
			Family fam=Patients.GetFamily(_patNumCur);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<long> pinAptNums=new List<long>();
			switch(((UI.Button)sender).Tag.ToString()) {
				case "SchedPatRecall":
					pinAptNums=SchedPatRecall(_gridCur.SelectedTag<PatRowTag>().PriKeyNum,fam.GetPatient(_patNumCur),subList,planList);
					break;
				case "SchedFamRecall":
					if(!Recalls.IsRecallProphyOrPerio(Recalls.GetRecall(_gridCur.SelectedTag<PatRowTag>().PriKeyNum))) {
						MsgBox.Show(this,"Only recall types of Prophy or Perio can be scheduled for families.");
						return;
					}
					pinAptNums=SchedFamRecall(fam,subList,planList);
					break;
				case "SchedPatReact":
					pinAptNums=SchedPatReact(_patNumCur);
					break;
				case "SchedFamReact":
					pinAptNums=SchedFamReact(fam);
					break;
			}
			if(pinAptNums.Count<1) {
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			GotoModule.PinToAppt(pinAptNums,_patNumCur);
			//no securitylog entry needed.  It will be made as each appt is dragged off pinboard.
			_gridCur.SetAll(false);
			FillCurGrid();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private DataTable GetAddrTable() {
			if(IsRecallGridSelected()) {//RECALL
				addrTable=Recalls.GetAddrTable(_gridCur.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList(),
					checkGroupFamiliesRecalls.Checked,comboSortRecalls.GetSelected<RecallListSort>());
			}
			else { //REACTIVATION
				List<Patient> listPats=Patients.GetMultPats(_gridCur.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList()).ToList();
				List<Patient> listGuars=Patients.GetMultPats(listPats.Select(x => x.Guarantor).Distinct().ToList()).ToList();
				addrTable=Reactivations.GetAddrTable(listPats,listGuars,checkGroupFamiliesReact.Checked,comboSortReact.GetSelected<ReactivationListSort>());
			}
			return addrTable;
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
			public ContactMethod RecallMethodPreferred;
			public long GuarantorNum;
			public long ClinicNum;
			public DateTime DateDue;
			public string WirelessPhone;
			public string WebSchedSendError;
			public AutoCommStatus WebSchedSmsSendStatus;
			public AutoCommStatus WebSchedEmailSendStatus;

			public PatRowTag(long patNum,long priKeyNum,long statusDefNum,int numReminders,string email,ContactMethod recallMethodPreferred,long guarNum,
				long clinicNum,DateTime dateDue=default,string wirelessPhone=null,string webSchedSendError=null,
				AutoCommStatus webSchedSmsSendStatus=AutoCommStatus.Undefined,AutoCommStatus webSchedEmailSendStatus=AutoCommStatus.Undefined)
			{
				PatNum=patNum;
				PriKeyNum=priKeyNum;
				StatusDefNum=statusDefNum;
				NumReminders=numReminders;
				Email=email;
				RecallMethodPreferred=recallMethodPreferred;
				GuarantorNum=guarNum;
				ClinicNum=clinicNum;
				DateDue=dateDue;
				WirelessPhone=wirelessPhone;
				WebSchedSendError=webSchedSendError;
				WebSchedSmsSendStatus=webSchedSmsSendStatus;
				WebSchedEmailSendStatus=webSchedEmailSendStatus;
			}
		}

		private void checkGroupFamilies_Click(object sender,MouseEventArgs e) {

		}
	}
	
}
