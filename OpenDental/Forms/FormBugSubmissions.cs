using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBugSubmissions:FormODBase {
	
		///<summary>List of all bugSubmissons from the bugs DB.</summary>
		private List<BugSubmission> _listAllSubs;
		///<summary>When FormBugSumissionMode.AddBug, form will close after adding a bug.
		///When FormBugSumissionMode.ViewOnly, the "Add Bug" button is not visable.
		///When FormBugSumissionMode.SelectionMode, the "Add Bug" is changed to "Ok".</summary>
		private FormBugSubmissionMode _viewMode;
		///<summary>Used to determine if a new bug should show (Enhancement) in the description.</summary>
		private Job _jobCur;
		///<summary>Null unless a bug is added when _viewMode is FormBugSumissionMode.AddBug.</summary>
		public Bug BugCur;
		///<summary>List of selected bugSubmissions when _viewMode is FormBugSumissionMode.SelectionMode.</summary>
		public List<BugSubmission> ListSelectedSubs=new List<BugSubmission>();
		///<summary>List of bugSubmissions to view when _viewMode is FormBugSumissionMode.ViewMode.</summary>
		public List<BugSubmission> ListViewedSubs=new List<BugSubmission>();
		///<summary>Dictionary of patients that will lazy load as users click on entries.  The key is the Registration Key.</summary>
		private Dictionary<string,Patient> _dictPatients=new Dictionary<string, Patient>();
		///<summary>The current patient associated to the selected bug submission row. Null if no row selected or if multiple rows selected.</summary>
		private Patient _patCur;
		///<summary>BugSubmission from the currently selected submission in either gridSubs or gridCustomerSubs if any, otherwise null.</summary>
		private BugSubmission _subCur;
		///<summary></summary>
		private List<JobLink> _listJobLinks;
		///<summary>The number of minimum submission for a group to show when 'Min Count' is selected.</summary>
		private long _minGroupingCount=-1;

		///<summary>Set job if you would like to create a bug with (Enhancement) in the bug text.
		///When isViewOnlyMode is true, you will not be able to create a bug.
		///When isSelectedMode is true, the form will close after a double click selection or a group selection.</summary>
		public FormBugSubmissions(Job job=null,FormBugSubmissionMode viewMode=FormBugSubmissionMode.AddBug) {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
			_jobCur=job;
			_viewMode=viewMode;
		}

		private void FormBugSubmissions_Load(object sender,EventArgs e) {
			LayoutMenu();
			SetFilterControlsAndAction(() => FillSubGrid(),
				textDevNoteFilter,textPatNums,textStackFilter,textMsgText,textCategoryFilters,listShowHideOptions);
			switch(_viewMode) {
				case FormBugSubmissionMode.AddBug:
					dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-60));
					dateRangePicker.SetDateTimeTo(DateTime.Today);
					break;
				case FormBugSubmissionMode.ViewOnly:
					dateRangePicker.SetDateTimeFrom(DateTime.MinValue);
					dateRangePicker.SetDateTimeTo(DateTime.MaxValue.AddDays(-1));//Subtract a day for DbHelper.DateTConditionColumn(...)
					butAddJob.Visible=false;
					listShowHideOptions.SetSelected(1);
					break;
				case FormBugSubmissionMode.SelectionMode:
					dateRangePicker.SetDateTimeFrom(DateTime.MinValue);
					dateRangePicker.SetDateTimeTo(DateTime.MaxValue.AddDays(-1));//Subtract a day for DbHelper.DateTConditionColumn(...)
					butAddJob.Text="OK";//On click the selected rows are saved and this form will close.
					break;
				case FormBugSubmissionMode.ValidationMode:
					dateRangePicker.SetDateTimeFrom(DateTime.MinValue);
					dateRangePicker.SetDateTimeTo(DateTime.MaxValue.AddDays(-1));//Subtract a day for DbHelper.DateTConditionColumn(...)
					butAddJob.Text="OK";
					listShowHideOptions.SetSelected(1);
					groupFilters.Enabled=false;
					break;
			}
			bugSubmissionControl.TextDevNoteLeave+=textDevNote_PostLeave;
			bugSubmissionControl.OnGridCustomerSubsCellClick+=customerSubsGridClick;
			#region comboGrouping
			comboGrouping.Items.Add("None");
			comboGrouping.Items.Add("RegKey/Ver/Stack");
			comboGrouping.Items.Add("StackTrace");
			comboGrouping.Items.Add("95%");
			comboGrouping.Items.Add("StackSig");
			comboGrouping.Items.Add("StackSimple");
			comboGrouping.Items.Add("Hash");
			switch(_viewMode) {
				case FormBugSubmissionMode.AddBug:
					comboGrouping.SelectedIndex=2;//Default to StackTrace.
					break;
				case FormBugSubmissionMode.SelectionMode:
				case FormBugSubmissionMode.ValidationMode:
				case FormBugSubmissionMode.ViewOnly:
					comboGrouping.SelectedIndex=0;//Default to None.
					break;
			}
			#endregion
			#region comboSortBy
			comboSortBy.Items.Add("Vers./Count");
			comboSortBy.SelectedIndex=0;//Default to Vers./Count
			#endregion
			#region Right Click Menu
			ContextMenu gridSubMenu=new ContextMenu();
			Menu.MenuItemCollection menuItemCollection=new Menu.MenuItemCollection(gridSubMenu);
			List<MenuItem> listMenuItems=new List<MenuItem>();
			listMenuItems.Add(new MenuItem(Lan.g(this,"Open Submission"),new EventHandler(gridSubs_RightClickHelper)));
			listMenuItems.Add(new MenuItem(Lan.g(this,"Open Bug"),new EventHandler(gridSubs_RightClickHelper)));//Enabled by default
			listMenuItems.Add(new MenuItem(Lan.g(this,"Hide"),new EventHandler(gridSubs_RightClickHelper)));
			listMenuItems.Add(new MenuItem(Lan.g(this,"Link Bug"),new EventHandler(gridSubs_RightClickHelper)));
			menuItemCollection.AddRange(listMenuItems.ToArray());
			gridSubMenu.Popup+=new EventHandler((o,ea) => {
				BugSubGridRow bugSubGridRow=gridSubs.SelectedTag<BugSubGridRow>();
				bool isBugSubSelected=(bugSubGridRow!=null);
				bool hasBugId=false;
				bool isHidden=false;
				if(isBugSubSelected) {
					hasBugId=(bugSubGridRow.BugSubmissionDisplay.BugId > 0);
					isHidden=bugSubGridRow.BugSubmissionDisplay.IsHidden;
				}
				gridSubMenu.MenuItems[2].Text=(isHidden ? "Unhide" : "Hide");
				gridSubMenu.MenuItems[3].Text=(hasBugId ? "UnLink Bug" : "Link Bug");
				//The menu items are only enabled when a valid BugSubGridRow is selected.
				gridSubMenu.MenuItems[0].Enabled=isBugSubSelected;//Open Submission
				gridSubMenu.MenuItems[1].Enabled=(isBugSubSelected && hasBugId);//Open Bug
				gridSubMenu.MenuItems[2].Enabled=isBugSubSelected;//Hide or Unhide Submissions
				gridSubMenu.MenuItems[3].Enabled=isBugSubSelected;//Link or Unlink bug
			});
			gridSubs.ContextMenu=gridSubMenu;
			#endregion
			FillVersionsFilter();
			FillSubGrid(true);
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			MenuItemOD menuItemTools=new MenuItemOD("Tools");
			menuMain.Add(menuItemTools);
			menuItemTools.Add("Find Similar Submissions",findPreviouslyFixedSubmisisonsToolStripMenuItem_Click);
			menuItemTools.Add("Match Hidden Submissions",matchHiddenSubmissionsToolStripMenuItem_Click);
			menuItemTools.Add("Hash Vitals",HashVitalsToolStripMenuItem_Click);
			menuMain.EndUpdate();
		}
		
		private void findPreviouslyFixedSubmisisonsToolStripMenuItem_Click(object sender,EventArgs e) {
			MsgBox.Show("This option has been disabled for now.");
			return;
			//if(!BugSubmissionL.TryAssociateSimilarBugSubmissions(this.Location)) {
			//	return;
			//}
			//FillSubGrid(true);
		}

		private void matchHiddenSubmissionsToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!BugSubmissionL.HideMatchedBugSubmissions()){
				return;
			}
			FillSubGrid(true);
		}
		
		private void HashVitalsToolStripMenuItem_Click(object sender,EventArgs e) {
			FormBugSubmissionHashVitals form=new FormBugSubmissionHashVitals();
			form.Show();
		}

		///<summary></summary>
		private void gridSubs_RightClickHelper(object sender,EventArgs e) {
			int index=gridSubs.GetSelectedIndex();
			if(index==-1) {//Should not happen, menu item is only enabled when exactly 1 row is selected.
				return;
			}
			BugSubGridRow bugSubGridRow=gridSubs.SelectedTag<BugSubGridRow>();
			List<BugSubmission> listBugSubmissions=gridSubs.SelectedTags<BugSubGridRow>().SelectMany(x => x.ListBugSubmissions).ToList();
			switch(((MenuItem)sender).Index) {
				case 0://Open Submission
					FormBugSubmission formBugSub=new FormBugSubmission(bugSubGridRow.BugSubmissionDisplay,_jobCur);
					formBugSub.Show();
					break;
				case 1://Open Bug
					OpenBug(bugSubGridRow.BugSubmissionDisplay);
					break;
				case 2://Hide or Unhide submission
					//Flip all grouped submissions based on what the user selected/sees in the grid.
					bool isHidden=(!bugSubGridRow.BugSubmissionDisplay.IsHidden);
					listBugSubmissions.ForEach(x => x.IsHidden=isHidden);
					BugSubmissions.UpdateMany(listBugSubmissions,"IsHidden");
					FillSubGrid(true);
					break;
				case 3://Link or Unlink bug
					if(bugSubGridRow.BugSubmissionDisplay.BugId==0) {//Not linked to existing bug, so link
						using FormBugSearch formBS=new FormBugSearch(new Job());
						if(formBS.ShowDialog()!=DialogResult.OK || formBS.BugCur==null) {
							return;
						}
						listBugSubmissions.ForEach(x => x.BugId=formBS.BugCur.BugId);
						BugSubmissionHashes.UpdateBugIds(listBugSubmissions,formBS.BugCur.BugId);
					}
					else {//Unlink
						listBugSubmissions.ForEach(x => x.BugId=0);
						BugSubmissionHashes.UpdateBugIds(listBugSubmissions,0);
					}
					BugSubmissions.UpdateMany(listBugSubmissions,"BugId");
					FillSubGrid(true);
					break;
			}
		}

		private void FillSubGrid(bool isRefreshNeeded=false,string grouping95="") {
			#region gridSubs columns
			gridSubs.BeginUpdate();
			gridSubs.ListGridColumns.Clear();
			gridSubs.ListGridColumns.Add(new GridColumn("Submitter",140));
			gridSubs.ListGridColumns.Add(new GridColumn("Vers.",55,GridSortingStrategy.VersionNumber));
			if(comboGrouping.SelectedIndex==0) {//Group by 'None'
				gridSubs.ListGridColumns.Add(new GridColumn("DateTime",75,GridSortingStrategy.DateParse));
			}
			else {
				gridSubs.ListGridColumns.Add(new GridColumn("#",30,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			}
			gridSubs.ListGridColumns.Add(new GridColumn("Flag",50,HorizontalAlignment.Center));
			gridSubs.ListGridColumns.Add(new GridColumn("Msg Text",200) { IsWidthDynamic=true });
			gridSubs.AllowSortingByColumn=true;
			gridSubs.ListGridRows.Clear();
			#endregion
			bugSubmissionControl.ClearCustomerInfo();
			bugSubmissionControl.SetTextDevNoteEnabled(false);
			Action loadingProgress=null;
			try {
				if(isRefreshNeeded) {
					loadingProgress=ODProgress.Show(ODEventType.BugSubmission,typeof(BugSubmissionEvent),Lan.g(this,"Refreshing Data")+"...");
				}
				RefreshFilterGroupSortAndFillGridSubs(isRefreshNeeded,grouping95,loadingProgress);
				loadingProgress?.Invoke();
			}
			catch(Exception ex) {
				loadingProgress?.Invoke();
				FriendlyException.Show("There was a problem loading bug submissions. Change your filters and try again.",ex);
			}
			gridSubs.EndUpdate();
		}

		private void RefreshFilterGroupSortAndFillGridSubs(bool isRefreshNeeded,string grouping95,Action loadingProgress=null) {
			List<string> listSelectedVersions=listVersionsFilter.GetListSelected<string>();
			if(listSelectedVersions.Contains("All")) {
				listSelectedVersions.Clear();
			}
			if(isRefreshNeeded) {
				#region Refresh Logic
				if(ListTools.In(_viewMode,FormBugSubmissionMode.ViewOnly,FormBugSubmissionMode.ValidationMode)) {
					_listAllSubs=ListViewedSubs;
				}
				else {
					BugSubmissionEvent.Fire(ODEventType.BugSubmission,Lan.g(this,"Refreshing Data: Bugs"));
					_listAllSubs=new List<BugSubmission>();//Dereference the list of subs so that the old items can be garbage collected.
					_listAllSubs=BugSubmissions.GetAllInRange(dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),listSelectedVersions);
				}
				try {
					BugSubmissionEvent.Fire(ODEventType.BugSubmission,Lan.g(this,"Refreshing Data: Patients"));
					_dictPatients=new Dictionary<string,Patient>();//Dereference the dictionary of patients so that the old items can be garbage collected.
					_dictPatients=RegistrationKeys.GetPatientsByKeys(_listAllSubs.Select(x => x.RegKey).ToList());
				}
				catch(Exception e) {
					e.DoNothing();
					_dictPatients=new Dictionary<string, Patient>();
				}
				BugSubmissionEvent.Fire(ODEventType.BugSubmission,Lan.g(this,"Refreshing Data: JobLinks"));
				_listJobLinks=new List<JobLink>();//Dereference the list of job links so that the old items can be garbage collected.
				_listJobLinks=JobLinks.GetManyForType(JobLinkType.Bug,_listAllSubs.Select(x => x.BugId).Where(x => x!=0).Distinct().ToList());
				#endregion
			}
			#region Filter Logic
			BugSubmissionEvent.Fire(ODEventType.BugSubmission,"Filtering Data");
			List<string> listSelectedRegKeys=comboRegKeys.GetListSelected<string>();
			if(comboRegKeys.IsAllSelected) {
				listSelectedRegKeys.Clear();//An empty list means that all is selected
			}
			List<string> listStackFilters=textStackFilter.Text.Split(',')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.ToLower()).ToList();
			List<string> listPatNumFilters=textPatNums.Text.Split(',')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.ToLower()).ToList();
			List<string> listCategoryFilters=textCategoryFilters.Text.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			string msgText=textMsgText.Text;
			string devNoteFilter=textDevNoteFilter.Text;
			DateTime dateTimeFrom=dateRangePicker.GetDateTimeFrom();
			DateTime dateTimeTo=dateRangePicker.GetDateTimeTo();
			//Filter the list of all bug submissions and then order it by program version and submission date time so that the grouping is predictable.
			List<BugSubmission> listFilteredSubs=_listAllSubs.Where(x => 
					PassesFilterValidation(x,listCategoryFilters,listSelectedRegKeys,listStackFilters,listPatNumFilters,listSelectedVersions,grouping95,
						msgText,devNoteFilter,dateTimeFrom,dateTimeTo)
				)
				.OrderByDescending(x => new Version(x.ProgramVersion))
				.ThenByDescending(x => x.SubmissionDateTime)
				.ToList();
			if(isRefreshNeeded) {
				FillPatNameFilter(_listAllSubs);
			}
			#endregion
			#region Grouping Logic
			List<BugSubGridRow> listGridSubmissions=new List<BugSubGridRow>();
			BugSubmissionEvent.Fire(ODEventType.BugSubmission,"Grouping Data");
			switch(comboGrouping.SelectedIndex) {
				case 0:
					#region None
					foreach(BugSubmission bugSubmission in listFilteredSubs) {
						listGridSubmissions.Add(new BugSubGridRow(bugSubmission));
					}
					listShowHideOptions.SetSelected(3,false);//Deselect 'None'
					_minGroupingCount=-1;
					butAddJob.Enabled=true;
					#endregion
					break;
				case 1:
					#region RegKey/Ver/Stack
					listFilteredSubs.GroupBy(x => new {
							x.BugId,
							x.RegKey,
							x.ProgramVersion,
							x.ExceptionMessageText,
							x.ExceptionStackTrace
						})
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listGridSubmissions.Add(new BugSubGridRow(x.Value.ToArray())));
					butAddJob.Enabled=true;
					#endregion
					break;
				case 2:
					#region StackTrace
					listFilteredSubs.GroupBy(x => new {
							x.BugId,
							x.ExceptionMessageText,
							x.ExceptionStackTrace
						})
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listGridSubmissions.Add(new BugSubGridRow(x.Value.ToArray())));
					butAddJob.Enabled=true;
					#endregion
					break;
				case 3:
					#region 95%
					//At this point all bugSubmissions in listFilteredSubs is at least a 95% match. Group them all together in a single row.
					if(!listFilteredSubs.IsNullOrEmpty()) {
						listGridSubmissions.Add(new BugSubGridRow(listFilteredSubs.ToArray()));
					}
					butAddJob.Enabled=true;
					#endregion
					break;
				case 4:
					#region StackSig
					listFilteredSubs.GroupBy(x => new {
							x.BugId,
							x.ExceptionMessageText,
							x.OdStackSignature
						})
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listGridSubmissions.Add(new BugSubGridRow(x.Value.ToArray())));
					butAddJob.Enabled=false;//Can not add jobs in this mode.
					#endregion
					break;
				case 5:
					#region StackSimple
					listFilteredSubs.GroupBy(x => new {
							x.BugId,
							x.ExceptionMessageText,
							x.SimplifiedStackTrace
						})
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listGridSubmissions.Add(new BugSubGridRow(x.Value.ToArray())));
					butAddJob.Enabled=false;//Can not add jobs in this mode.
					#endregion
					break;
				case 6:
					#region Hash
					listFilteredSubs.GroupBy(x => new {
							x.BugId,
							x.BugSubmissionHashNum
						})
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listGridSubmissions.Add(new BugSubGridRow(x.Value.ToArray())));
					butAddJob.Enabled=false;//Can not add jobs in this mode.
					#endregion
				break;
			}
			if(_minGroupingCount>0) {
				listGridSubmissions.RemoveAll(x => x.ListBugSubmissions.Count < _minGroupingCount);
			}
			#endregion
			#region Sorting Logic
			BugSubmissionEvent.Fire(ODEventType.BugSubmission,"Sorting Data");
			switch(comboSortBy.SelectedIndex) {
				case 0:
					listGridSubmissions=listGridSubmissions.OrderByDescending(x => new Version(x.BugSubmissionDisplay.ProgramVersion))
						.ThenByDescending(x => x.ListBugSubmissions.Count)
						.ThenByDescending(x => x.BugSubmissionDisplay.SubmissionDateTime).ToList();
					break;
			}
			#endregion
			#region Fill gridSubs
			BugSubmissionEvent.Fire(ODEventType.BugSubmission,"Filling Grid");
			foreach(BugSubGridRow sub in listGridSubmissions) {
				gridSubs.ListGridRows.Add(GetODGridRowForSub(sub));
			}
			#endregion
		}

		private GridRow GetODGridRowForSub(BugSubGridRow subRow) {
			BugSubmission sub=subRow.BugSubmissionDisplay;
			GridRow row=new GridRow();
			row.Cells.Add(_dictPatients.ContainsKey(sub.RegKey)?_dictPatients[sub.RegKey].GetNameLF():sub.RegKey);
			row.Cells.Add(sub.ProgramVersion);
			switch(comboGrouping.SelectedIndex) {
				case 0://None
					row.Cells.Add(sub.SubmissionDateTime.ToString().Replace('\r',' '));
					break;
				case 1://Customer
				case 2://StackTrace
				case 3://95%
				case 4://StackSig
				case 5://StackSimple
					row.Cells.Add(subRow.ListBugSubmissions.Count.ToString());
					break;
			}
			List<string> listStatuses=new List<string>();
			if(sub.BugId!=0) {
				if(_listJobLinks.Any(x => x.FKey==sub.BugId)) {
					listStatuses.Add("J");
				}
				listStatuses.Add("B");
			}
			if(sub.IsHidden) {
				listStatuses.Add("H");
			}
			row.Cells.Add(string.Join(",",listStatuses));
			row.Cells.Add(sub.ExceptionMessageText+(string.IsNullOrEmpty(sub.DevNote)?"":"\r\n\r\nDevNote: "+sub.DevNote));
			row.Tag=subRow;
			return row;
		}

		private bool PassesFilterValidation(BugSubmission sub,List<string> listCategoryFilters,List<string> listSelectedPatNames,
			List<string> listStackFilters,List<string> listPatNumFilters,List<string> listSelectedVersions,string grouping95,string msgText,
			string devNoteFilter,DateTime dateTimeFrom,DateTime dateTimeTo)
		{
			bool hasMobileSelected=listSelectedVersions.Count(x => x=="Mobile")!=0;
			bool hasVersionsSelected=listSelectedVersions.Count(x => x!="Mobile")!=0;
			if(_viewMode!=FormBugSubmissionMode.ValidationMode
					&& ((!string.IsNullOrWhiteSpace(msgText)&&!sub.ExceptionMessageText.ToLower().Contains(msgText.ToLower()))
					||(listSelectedPatNames.Count!=0 && !listSelectedPatNames.Contains(_dictPatients.ContainsKey(sub.RegKey)?_dictPatients[sub.RegKey].GetNameLF():sub.RegKey))
					||(listStackFilters.Count!=0 && !listStackFilters.Exists(x => sub.ExceptionStackTrace.ToLower().Contains(x)))
					||(listPatNumFilters.Count!=0 && (!_dictPatients.ContainsKey(sub.RegKey) || !listPatNumFilters.Exists(x => x==_dictPatients[sub.RegKey].PatNum.ToString())))
					||(sub.BugId!=0 && !listShowHideOptions.SelectedIndices.Contains(1))
					||(!listShowHideOptions.SelectedIndices.Contains(0) && _dictPatients.ContainsKey(sub.RegKey) && (_dictPatients[sub.RegKey].BillingType==436||_dictPatients[sub.RegKey].PatNum==1486))//436 is "Internal Use" def, 1486 is HQ patNum.
					||(hasVersionsSelected && !sub.IsMobileSubmission && !listSelectedVersions.Contains(StringTools.SubstringBefore(sub.ProgramVersion,'.',2)))
					||(hasMobileSelected && !hasVersionsSelected && !sub.IsMobileSubmission)
					||(!hasMobileSelected && sub.IsMobileSubmission)
					||(!sub.SubmissionDateTime.Between(dateTimeFrom,dateTimeTo))
					||(!string.IsNullOrWhiteSpace(devNoteFilter) && !sub.DevNote.ToLower().Contains(devNoteFilter.ToLower()))
					||(!string.IsNullOrEmpty(grouping95) && BugSubmissionL.CalculateSimilarity(grouping95,sub.ExceptionStackTrace)<95))
					||(!listShowHideOptions.SelectedIndices.Contains(2) && sub.IsHidden)
					||(listCategoryFilters.Count>0 && listCategoryFilters.All(x => !sub.ListCategoryTags.Any(y => y.ToLower().Contains(x.ToLower())))))
			{
				return false;
			}
			return true;
		}

		private void FillVersionsFilter() {
			listVersionsFilter.Items.Clear();
			listVersionsFilter.Items.Add("All");
			//HQ has a preference that stores the number of versions that are actively accepting bug submissions.
			//Allow the bug submissions window to select any of said versions.
			//The user will have to use "All" in order to see bug submissions for even older versions.
			int countLastVersions=Math.Max(Bugs.GetCountPreviousVersions(useConnectionStore:false),3);//Defaut to last 3 if anything goes wrong.
			List<VersionRelease> listVersions=VersionReleases.GetLastUnreleasedVersions(countLastVersions);
			if(listVersions.IsNullOrEmpty()) {
				listVersionsFilter.SetSelected(0);//Select 'All' and return.
				return;
			}
			//Otherwise; add the versions found and select the first three by default so that we don't load up too many bug submissions.
			foreach(VersionRelease version in listVersions) {
				listVersionsFilter.Items.Add($"{version.MajorNum}.{version.MinorNum}");
				if(listVersionsFilter.SelectedIndices.Count < 3) {
					listVersionsFilter.SetSelected(listVersionsFilter.Items.Count-1);
				}
			}
			listVersionsFilter.Items.Add("Mobile");//butRefreshMobile_Click(...) assumes this is at the bottom.
		}
		
		private void FillPatNameFilter(List<BugSubmission> listSubmissions) {
			comboRegKeys.Items.Clear();
			List<string> listCustomerNames=listSubmissions.Select(x => _dictPatients.ContainsKey(x.RegKey)?_dictPatients[x.RegKey].GetNameLF():x.RegKey)
				.Distinct()
				.ToList();
			listCustomerNames.Sort();
			listCustomerNames.ForEach(x => comboRegKeys.Items.Add(x,x));
			if(comboRegKeys.SelectedIndices.Count==0) {
				comboRegKeys.IsAllSelected=true;//Select 'All' by default
			}
		}

		private void gridSubs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_viewMode==FormBugSubmissionMode.ViewOnly) {//Not allowed to create a bug.
				return;
			}
			//Because it is a double click, we know there will only be 1 item in list
			List<BugSubmission> listSubs=gridSubs.SelectedTag<BugSubGridRow>().ListBugSubmissions;
			if(_viewMode==FormBugSubmissionMode.SelectionMode) {
				ListSelectedSubs=listSubs;
				DialogResult=DialogResult.OK;
				return;
			}
			//The only time listSubs will have more than 1 item in it is when grouping.
			//The grouping logic ensures that all grouped items have the same bugid
			if(listSubs[0].BugId!=0) {
				OpenBug(listSubs[0]);
			}
			else {
				FormBugSubmission formBugSub=new FormBugSubmission(listSubs[0],_jobCur);
				formBugSub.Show();
			}
		}

		private void OpenBug(BugSubmission sub) {
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			using FormBugEdit FormBE=new FormBugEdit();
			FormBE.BugCur=Bugs.GetOne(sub.BugId);
			if(FormBE.ShowDialog()==DialogResult.OK && FormBE.BugCur==null) {//Bug was deleted.
				FillSubGrid(true);
			}
		}
		
		private void gridSubs_CellClick(object sender,UI.ODGridClickEventArgs e) {
			butAddJob.Text="Add Job";//Always reset
			if(e.Row==-1 || gridSubs.SelectedIndices.Length!=1) {
				bugSubmissionControl.ClearCustomerInfo();
				_subCur=null;
				labelDateTime.Text="";
				labelHashNum.Text="";
				bugSubmissionControl.SetTextDevNoteEnabled(false);
				return;
			}
			bugSubmissionControl.SetTextDevNoteEnabled(true);
			_subCur=gridSubs.SelectedTag<BugSubGridRow>().BugSubmissionDisplay;
			if(_dictPatients.ContainsKey(_subCur.RegKey)) {
				_patCur=_dictPatients[_subCur.RegKey];
			}
			else {
				try {
					RegistrationKey key=RegistrationKeys.GetByKey(_subCur.RegKey);
					_patCur=Patients.GetPat(key.PatNum);
				}
				catch(Exception ex) {
					ex.DoNothing();
					_patCur=new Patient();//Just in case, needed mostly for debug.
				}
				_dictPatients.Add(_subCur.RegKey,_patCur);
			}
			List<BugSubmission> listSubs=_listAllSubs;
			if(ListTools.In(comboGrouping.SelectedIndex,1,2,3,4,5)) {
				listSubs=gridSubs.SelectedTag<BugSubGridRow>().ListBugSubmissions;
			}
			butAddJob.Tag=null;
			bugSubmissionControl.RefreshData(_dictPatients,comboGrouping.SelectedIndex,listSubs);//New selelction, refresh control data.
			bugSubmissionControl.RefreshView(_subCur);
			labelDateTime.Text=POut.DateT(_subCur.SubmissionDateTime);
			labelHashNum.Text=POut.Long(_subCur.BugSubmissionHashNum);
			if(_subCur.BugId!=0) {
				List<JobLink> listJobLink=_listJobLinks.Where(x => x.FKey==_subCur.BugId).ToList();
				if(listJobLink.Count==1) {
					butAddJob.Text="View Job";
					butAddJob.Tag=listJobLink.First();
				}
			}
			if(ListTools.In(_viewMode,FormBugSubmissionMode.SelectionMode,FormBugSubmissionMode.ValidationMode)) {
				butAddJob.Text="OK";
			}
		}
		
		public void textDevNote_PostLeave(object sender,EventArgs e){
			if(gridSubs.SelectedIndices.Count()>0) {
				int index=gridSubs.SelectedIndices[0];
				gridSubs.BeginUpdate();
				gridSubs.ListGridRows[index]=GetODGridRowForSub(gridSubs.SelectedTag<BugSubGridRow>());
				gridSubs.EndUpdate();
				gridSubs.SetSelected(index,true);
			}	
		}
		
		public void customerSubsGridClick(BugSubmission sub) {
			labelDateTime.Text=POut.DateT(sub.SubmissionDateTime);
			labelHashNum.Text=POut.Long(sub.BugSubmissionHashNum);
		}

		private void dateRangePicker_CalendarClosed(object sender,EventArgs e) {
			FillSubGrid(true);//Refresh _listAllSubs
		}

		private void comboVersions_SelectionChangeCommitted(object sender,EventArgs e) {
			string group95Matching="";
			if(sender==comboGrouping && comboGrouping.SelectedIndex==3) {//95%
				using InputBox input=new InputBox("Paste the stack trace you wish to match against.",true);
				if(input.ShowDialog()!=DialogResult.OK) {
					return;
				}
				group95Matching=input.textResult.Text;
			}
			FillSubGrid(grouping95:group95Matching);
		}

		private void ListShowHideOptions_SelectedIndexChanged(object sender,EventArgs e) {
			if(listShowHideOptions.SelectedIndices.Contains(3) && _minGroupingCount>=0) {//Already Set and still selected, another item was clicked.
				return;
			}
			else if(!listShowHideOptions.SelectedIndices.Contains(3)) {
				_minGroupingCount=-1;
				return;
			}
			else if(comboGrouping.SelectedIndex<=0) {//Do not allow when 'None' selected.
				MsgBox.Show("Min Count only applies when subissions are grouped together, can not be used with 'None'.");
				listShowHideOptions.SetSelected(3,false);//Deselect 'None'
				_minGroupingCount=-1;
				return;
			}
			using InputBox input=new InputBox("Minimum number of submissions:");
			if(input.ShowDialog()!=DialogResult.OK || input.textResult.Text.IsNullOrEmpty()){
				listShowHideOptions.SetSelected(3,false);//Deselect 'None'
				_minGroupingCount=-1;
				return;
			}
			_minGroupingCount=PIn.Int(input.textResult.Text,false);
			FillSubGrid();
		}
		
		private void butRefreshMobile_Click(object sender,EventArgs e) {
			listVersionsFilter.ClearSelected();
			listVersionsFilter.SetSelected(listVersionsFilter.Items.Count-1);//Mobile
			FillSubGrid(true);//Refresh _listAllSubs
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillSubGrid(true);//Refresh _listAllSubs
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(_viewMode==FormBugSubmissionMode.SelectionMode) {//Text is set to "Ok" when SelectionMode
				ListSelectedSubs=gridSubs.SelectedTags<BugSubGridRow>().SelectMany(x => x.ListBugSubmissions).ToList();
				DialogResult=DialogResult.OK;
				return;
			}
			if(_viewMode==FormBugSubmissionMode.ValidationMode) {//Text is set to "Ok" when SelectionMode
				ListSelectedSubs=_listAllSubs;
				DialogResult=DialogResult.OK;
				return;
			}
			if(butAddJob.Text=="View Job" && butAddJob.Tag is JobLink) {//Assocaited to job, see gridSubs_CellClick(...)	
				FormOpenDental.S_GoToJob((butAddJob.Tag as JobLink).JobNum);
				return;
			}
			List<BugSubmission> listSelectedSubs=gridSubs.SelectedTags<BugSubGridRow>().SelectMany(x => x.ListBugSubmissions).ToList();
			BugCur=BugSubmissionL.AddBugAndJob(this,listSelectedSubs,_patCur);
			if(BugCur==null) {
				return;
			}
			if(this.Modal) {
				this.DialogResult=DialogResult.OK;
			}
			else {
				FillSubGrid(true);
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private class BugSubGridRow {
			///<summary>Filled in the constructor. Guaranteed to have at least one bug submission present.</summary>
			public List<BugSubmission> ListBugSubmissions;
			///<summary>Returns the first bug submission in the list of all bug submissions that this grid row represents.</summary>
			public BugSubmission BugSubmissionDisplay {
				get {
					return ListBugSubmissions.First();
				}
			}
			
			public BugSubGridRow(params BugSubmission[] arrayBugSubmissions) {
				if(arrayBugSubmissions.IsNullOrEmpty()) {
					throw new ApplicationException("No bug submissions provided for the new BugSubGridRow.");
				}
				ListBugSubmissions=arrayBugSubmissions.ToList();
			}
		}

	}

	///<summary>Enum controling the way the form displays and behaves.</summary>
	public enum FormBugSubmissionMode {
		///<summary>This is the default way for the form to load. Used by job manager to add bugs</summary>
		AddBug,
		///<summary>Used when we wish to simply view the bug submissions, does not allow users to add bugs. Filter validation is skipped.</summary>
		ViewOnly,
		///<summary>Used when attaching bug submissions to exiting bugs. Changed butAdd to show "OK" and return selected rows.</summary>
		SelectionMode,
		///<summary>Used when using the similiar bugs tool. Changed butAdd to show "OK" and returns all BugSubmissions in the grid on Ok click. Filter validation is skipped.</summary>
		ValidationMode,
	}
}