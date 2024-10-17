using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	//This Form cannot currently scale to high dpi monitors.
	//It's too complex and customized to easily allow the same high dpi support strategy used on the other forms.
	//I tried inheriting from Form instead of FormODBase, but SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE); still doesn't work even though that works successfully on other forms.  I suspect the culprit is multiple UI threads.
	//The easiest approach is to just deal with it by keeping it on 96 dpi monitors only.
	//If we someday decide that we must do better, we might create a separate application that's not dpi aware, which will at least not malfuction, although it will look blurry because it will still only be 96 dpi.
	public partial class FormJobManager:FormODBase {
		///<summary>Holds the time of the most recent refresh of the active tab and job edit UI.</summary>
		public static DateTime LocalLastRefreshDateTime=DateTime.MinValue;
		///<summary>Dictionary containing row notes shown when hovering.
		///Key		=> Row index
		///Value	=> Note for the row.</summary>
		private Dictionary<int,List<string>> _dicRowNotes=new Dictionary<int, List<string>>();
		///<summary>Used when hovering to show flag explanations.
		///Object tag is the location of the mouse the last time the tip was shown. Used to reduce redraw and flicker.</summary>
		private ToolTip _toolTipHover=new ToolTip();
		private List<Def> _listJobPriorities;
		private List<Userod> _listUsers;
		private Stack<long> _stackBackJobNums=new Stack<long>();
		private Stack<long> _stackForwardJobNums=new Stack<long>();
		///<summary>Allows engineers to change clock status.</summary>
		private MenuItemOD _menuItemEmployeeStatus=null;
		///<summary>Information about engineer employee clock status. Kept current by FormOpenDental typical clock status timer.</summary>
		private Phone _phone=null;
		private static List<FormJobEdit> _listJobEditForms=new List<FormJobEdit>();
		///<summary>Grid themes are gone.  This static color gets reused here, as needed.</summary>
		private static Color _colorGridHeaderBack=Color.FromArgb(223,234,245);
		///<summary>Filled with JobActions that will be collapsed when filling the Needs Action grid.</summary>
		private List<JobAction> _listJobActionsCollapsed=new List<JobAction>();
		///<summary>Filled with JobActions that will be collapsed when filling the Project Management grid.</summary>
		private List<JobAction> _listJobActionsCollapsedProject=new List<JobAction>();
		///<summary>Filled with JobActions that will be collapsed when filling the Needs Action grid.</summary>
		private List<JobTeam> _listJobTeams;
		private List<long> _listQueryPriorityFilter=new List<long>();
		private List<JobPhase> _listQueryJobPhaseFilter=new List<JobPhase>();
		private readonly long _defNumIsOnHold;
		private readonly long _defNumIsUrgent;
		private readonly Brush _brushDefault=SystemBrushes.Control;
		private readonly Brush _brushSelected=Brushes.White;
		private readonly Brush _brushNotify=Brushes.LightSalmon;

		private readonly List<int> _listCategoriesSorted=new List<int> {
			(int)JobCategory.Bug,
			(int)JobCategory.Enhancement,
			(int)JobCategory.ProgramBridge,
			(int)JobCategory.Conversion,
			(int)JobCategory.Feature,
			(int)JobCategory.InternalRequest,
			(int)JobCategory.Research,
			(int)JobCategory.NeedNoApproval,
			(int)JobCategory.HqRequest,
			(int)JobCategory.SpecialProject,
			(int)JobCategory.UnresolvedIssue,
			(int)JobCategory.Query,
			(int)JobCategory.MarketingDesign,
			(int)JobCategory.Project
		};

		private readonly List<int> _listPhasesSorted=new List<int> {
			(int)JobPhase.Concept,
			(int)JobPhase.Definition,
			(int)JobPhase.Quote,
			(int)JobPhase.Development,
			(int)JobPhase.Documentation,
			(int)JobPhase.Complete,
			(int)JobPhase.Cancelled
		};

		public FormJobManager() {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			//This is here so we can see the tabs in the designer, but use the ownerdraw when the program is run.
			tabControlNav.DrawMode=TabDrawMode.OwnerDrawFixed;
			tabControlNav.TabPages.Clear();
			FillPriorityList();
			_defNumIsOnHold=_listJobPriorities.Find(x=>x.ItemValue.Contains("OnHold")).DefNum;
			_defNumIsUrgent=_listJobPriorities.Find(x=>x.ItemValue.Contains("Urgent")).DefNum;
		}

		private void FormJobManager_Load(object sender,EventArgs e) {
			comboUser.Tag=Security.CurUser;
			FillMenu();
			_listUsers=Userods.GetUsersForJobs();
			_listJobTeams=JobTeams.GetDeepCopy();
			FillComboUser();
			FillComboTeamFilter(comboTeamFilterNeedsEngineer,doAddAllOption:true);
			FillComboTeamFilter(comboTeamFilterNeedsExpert,doAddAllOption:true);
			FillComboTeamFilter(comboTeamSearch,doAddAllOption:true);
			FillComboTeamFilter(comboTeamFilterProjectManagement,doAddAllOption:true);
			#region Fill Proposed Version Combos
			comboProposedVersionNeedsAction.Items.Add("All");
			comboProposedVersionNeedsEngineer.Items.Add("All");
			comboProposedVersionNeedsExpert.Items.Add("All");
			comboProposedVersionSearch.Items.Add("All");
			List<string> listProposedVersions=Enum.GetNames(typeof(JobProposedVersion)).ToList();
			foreach(string proposedVersion in listProposedVersions) {
				comboProposedVersionNeedsAction.Items.Add(proposedVersion,(JobProposedVersion)Enum.Parse(typeof(JobProposedVersion),proposedVersion));
				comboProposedVersionNeedsEngineer.Items.Add(proposedVersion,(JobProposedVersion)Enum.Parse(typeof(JobProposedVersion),proposedVersion));
				comboProposedVersionNeedsExpert.Items.Add(proposedVersion,(JobProposedVersion)Enum.Parse(typeof(JobProposedVersion),proposedVersion));
				comboProposedVersionSearch.Items.Add(proposedVersion,(JobProposedVersion)Enum.Parse(typeof(JobProposedVersion),proposedVersion));
			}
			comboProposedVersionNeedsAction.SelectedIndex=0;
			comboProposedVersionNeedsEngineer.SelectedIndex=0;
			comboProposedVersionNeedsExpert.SelectedIndex=0;
			comboProposedVersionSearch.SelectedIndex=0;
			#endregion
			comboCatSearch.Items.Add("All");
			List<string> listJobCategories=Enum.GetNames(typeof(JobCategory)).ToList();
			foreach(string jobCategory in listJobCategories) {
				comboCatSearch.Items.Add(jobCategory,(JobCategory)Enum.Parse(typeof(JobCategory),jobCategory));
			}
			comboCatSearch.SelectedIndex=0;
			comboPrioritySearch.Items.Add("All");
			comboQueryPriorityFilter.IncludeAll=true;
			List<Def> jobPriorities=Defs.GetCatList((int)DefCat.JobPriorities).ToList();
			foreach(Def jobPriority in jobPriorities) {
				comboPrioritySearch.Items.Add(jobPriority.ItemName,jobPriority);
				comboQueryPriorityFilter.Items.Add(jobPriority.ItemName,jobPriority);
			}
			comboPrioritySearch.SelectedIndex=0;
			comboQueryPriorityFilter.IsAllSelected=true;
			comboQueryPhaseFilter.IncludeAll=true;
			comboQueryPhaseFilter.Items.AddEnums<JobPhase>();
			comboQueryPhaseFilter.IsAllSelected=true;
			_listQueryPriorityFilter=comboQueryPriorityFilter.Items.GetAll<Def>().ConvertAll(x=>x.DefNum);
			_listQueryJobPhaseFilter=comboQueryPhaseFilter.Items.GetAll<JobPhase>();
			dateExcludeCompleteBefore.Value=DateTime.Now.AddMonths(-3);
			UpdateTabVisibility();//This speeds up RefreshAndFillThreaded since it will remove some of the grids.
			JobManagerCore.RefreshAndFillThreaded();
			_toolTipHover.OwnerDraw=true;
			_toolTipHover.Draw+=new DrawToolTipEventHandler(toolTipHover_Draw);
			_toolTipHover.Popup+=new PopupEventHandler(toolTipHover_PopupHelper);
			gridTesting.ContextMenu=new ContextMenu();
			gridTesting.ContextMenu.MenuItems.Add("Assign Tester",(o,arg)=>menuItemAssignTester_Click(o,arg));
			gridTesting.ContextMenu.MenuItems.Add("Mark As Tested",(o,arg)=>menuItemMarkAsTested_Click(o,arg));
			gridTesting.ContextMenu.MenuItems.Add("Clear Tested",(o,arg)=>menuItemClearTested_Click(o,arg));
			try {
				Version lastVersion=new Version(VersionReleases.GetLastReleases(1));
				textVersionText.Text=lastVersion.Major+"."+lastVersion.Minor;
			}
			catch {
				textVersionText.Text="";
			}
			if (ClipboardDataIsValid()) {
				//Load the new job into the job mananger controls and cache.
				LoadJob(JobSearchFromClipboard(),false);
				if(userControlJobManagerEditor.JobCur!=null) {
					RefreshGridsForSearch();
					gridSearch.ScrollToIndex(gridSearch.GetSelectedIndex());
				}
			}
			timerRefreshUI.Start();
		}

		///<summary>Sets visibility of tabs based on user permissions. All tabs must be explicitly listed here in order to show/exist because we clear the tabControlNav list of tabs in the ctor.</summary>
		#region TabControl Methods
		private void UpdateTabVisibility() {
			//If user has Concept permission, user has access to the actions tab, etc.
			SetTabVisibility(tabAction,new List<JobPerm>() { JobPerm.Concept,JobPerm.QueryTech });
			SetTabVisibility(tabSubscribed,JobPerm.Concept);
			SetTabVisibility(tabNeedsExpert,JobPerm.Concept);
			SetTabVisibility(tabNeedsEngineer,JobPerm.Concept);
			SetTabVisibility(tabSearch,JobPerm.Concept);
			SetTabVisibility(tabSubmittedJobs,JobPerm.Concept);
			SetTabVisibility(tabProjectManagement,JobPerm.Concept);
			SetTabVisibility(tabQuery,JobPerm.QueryTech);
			SetTabVisibility(tabMarketing,JobPerm.DesignTech);
			SetTabVisibility(tabDocumentation,JobPerm.Documentation);
			SetTabVisibility(tabNotify,JobPerm.NotifyCustomer);
			SetTabVisibility(tabTesting,JobPerm.TestingCoordinator);
			SetTabVisibility(tabOnHold,JobPerm.Approval);
			SetTabVisibility(tabSpecialProjects,JobPerm.SpecialProject);
			SetTabVisibility(tabPatternReview,JobPerm.PatternReview);
			SetTabVisibility(tabUnresolvedIssues,JobPerm.UnresolvedIssues);
		}

		///<summary>Adds tabs to 'tabControlNav' if the current user has the provided permission and the tab doesn't already exist.
		///Removes tabs from 'tabControlNav' if the current user doesn't have the provided permission and the tab already exists.
		///Will not add duplicate tabs, and will not throw if trying to remove a non-existent tab.</summary>
		private void SetTabVisibility(System.Windows.Forms.TabPage tabPage,JobPerm jobPermission) {
			if(JobPermissions.IsAuthorized(jobPermission,true) && !tabControlNav.TabPages.Contains(tabPage)) {
				tabControlNav.TabPages.Add(tabPage);
			}
			if(!JobPermissions.IsAuthorized(jobPermission,true) && tabControlNav.TabPages.Contains(tabPage)) {
				tabControlNav.TabPages.Remove(tabPage);
			}
		}

		///<summary>Adds tabs to 'tabControlNav' if the current user has any of the provided permissions and the tab doesn't already exist.
		///Removes tabs from 'tabControlNav' if the current user doesn't have the provided permission and the tab already exists.
		///Will not add duplicate tabs, and will not throw if trying to remove a non-existent tab.</summary>
		private void SetTabVisibility(System.Windows.Forms.TabPage tabPage,List<JobPerm> listJobPermission) {
			bool isAuthorized=listJobPermission.Any(x => JobPermissions.IsAuthorized(x,true));
			if(isAuthorized && !tabControlNav.TabPages.Contains(tabPage)) {
				tabControlNav.TabPages.Add(tabPage);
			}
			if(!isAuthorized && tabControlNav.TabPages.Contains(tabPage)) {
				tabControlNav.TabPages.Remove(tabPage);
			}
		}

		///<summary>Custom drawing for the tabs. Should not fail even if the subscriber tab does not exist</summary>
		private void tabControlNav_DrawItem(object sender,DrawItemEventArgs e) {
			System.Windows.Forms.TabPage page=tabControlNav.TabPages[e.Index];
			//We only care about changing the color for the Subscribed Jobs tab
			List<Job> listJobsWithNotifications=JobManagerCore.ListJobsAll.FindAll(x=>x.ListJobLinks.Exists(y=>y.LinkType==JobLinkType.Subscriber && y.FKey==Security.CurUser.UserNum) && x.ListJobNotifications.Exists(y=>y.UserNum==Security.CurUser.UserNum));
			if(page.Equals(tabSubscribed) && listJobsWithNotifications.Count>0) {
				e.Graphics.FillRectangle(_brushNotify,e.Bounds);
				if(listJobsWithNotifications.Any(x=>x.Priority==_defNumIsOnHold)) {
					checkSubscribedIncludeOnHold.Checked=true;
				}
			}
			else if(e.State==DrawItemState.Selected) {
				e.Graphics.FillRectangle(_brushSelected,e.Bounds);
			}
			else {
				e.Graphics.FillRectangle(_brushDefault,e.Bounds);
			}
			Rectangle paddedBounds=e.Bounds;
			int yOffset=(e.State==DrawItemState.Selected) ? -2 : 1;
			paddedBounds.Offset(1,yOffset);
			TextRenderer.DrawText(e.Graphics,page.Text,Font,paddedBounds,page.ForeColor);
		}

		private void tabControlNav_Selecting(object sender,TabControlCancelEventArgs e) {
			FillActiveTabGrid();
		}

		///<summary>Refreshes the grid in the active tab. Should be the only reference to any FillGrid in FormJobManager</summary>
		private void FillActiveTabGrid() {
			Cursor=Cursors.WaitCursor;
			tabAction.Text="Needs Action";
			if(tabControlNav.SelectedTab==tabAction) {
				FillGridActions();
			}
			else if(tabControlNav.SelectedTab==tabDocumentation) {
				FillGridDocumentation();
			}
			else if(tabControlNav.SelectedTab==tabNeedsEngineer) {
				FillGridNeedsEngineer();
			}
			else if(tabControlNav.SelectedTab==tabNeedsExpert) {
				FillGridNeedsExpert();
			}
			else if(tabControlNav.SelectedTab==tabNotify) {
				FillGridNotify();
			}
			else if(tabControlNav.SelectedTab==tabOnHold) {
				FillGridOnHold();
			}
			else if(tabControlNav.SelectedTab==tabPatternReview) {
				FillGridPatternReview();
			}
			else if(tabControlNav.SelectedTab==tabQuery) {
				FillGridQueries();
			}
			else if(tabControlNav.SelectedTab==tabMarketing) {
				FillGridMarketing();
			}
			else if(tabControlNav.SelectedTab==tabSearch) {
				FillGridSearch();
			}
			else if(tabControlNav.SelectedTab==tabSpecialProjects) {
				FillGridSpecial();
			}
			else if(tabControlNav.SelectedTab==tabSubscribed) {
				FillGridSubscribed();
			}
			else if(tabControlNav.SelectedTab==tabTesting) {
				FillGridTesting();
			}
			else if(tabControlNav.SelectedTab==tabUnresolvedIssues) {
				FillGridUnresolvedIssues();
			}
			else if(tabControlNav.SelectedTab==tabSubmittedJobs) {
				FillGridSubmittedJobs();
			}
			else if(tabControlNav.SelectedTab==tabProjectManagement) {
				FillGridProjectManagement();
			}
			Cursor=Cursors.Default;
		}

		private void menuItemAssignTester_Click(object o,EventArgs arg) {
			Job jobOld=gridTesting.SelectedTag<Job>();
			if(jobOld==null) {
				return;
			}
			List<Userod> listUsersForPicker=Userods.GetUsersByJobRole(JobPerm.TestingCoordinator,false);
			FrmUserPick frmUserPick=new FrmUserPick();
			frmUserPick.Text="Assign a Tester";
			frmUserPick.IsSelectionMode=true;
			frmUserPick.ListUserodsFiltered=listUsersForPicker;
			frmUserPick.IsPickNoneAllowed=true;
			frmUserPick.IsPickMeAllowed=true;
			frmUserPick.IsShowAllAllowed=false;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return;
			}
			Job jobCur=jobOld.Copy();
			jobCur.UserNumTester=frmUserPick.UserNumSelected;
			JobLogs.MakeLogEntryForTesting(jobCur,"Test User assigned to: "+Userods.GetName(frmUserPick.UserNumSelected));
			if(Jobs.Update(jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
				GoToJob(jobCur.JobNum);
			}
		}

		private void menuItemMarkAsTested_Click(object o,EventArgs arg) {
			Job jobOld=gridTesting.SelectedTag<Job>();
			if(jobOld==null) {
				return;
			}
			Job jobCur=jobOld.Copy();
			jobCur.DateTimeTested=DateTime.Now;
			JobLogs.MakeLogEntryForTesting(jobCur,"Job Marked as Tested");
			if(Jobs.Update(jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
				GoToJob(jobCur.JobNum);
			}
		}

		private void menuItemClearTested_Click(object o,EventArgs arg) {
			Job jobOld=gridTesting.SelectedTag<Job>();
			if(jobOld==null) {
				return;
			}
			Job jobCur=jobOld.Copy();
			jobCur.DateTimeTested=DateTime.MinValue;
			JobLogs.MakeLogEntryForTesting(jobCur,"Job Marked as Untested");
			if(Jobs.Update(jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
				GoToJob(jobCur.JobNum);
			}
		}

		private void datePickerFrom_ValueChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void datePickerTo_ValueChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void menuGoToAccountQueries_Click(object sender,EventArgs e) {
			try {
				GlobalFormOpenDental.GoToModule(EnumModuleType.Chart,patNum:((Job)gridQueries.ListGridRows[gridQueries.SelectedIndices[0]].Tag).ListJobQuotes[0].PatNum);
			}
			catch(Exception ex) {
				MsgBox.Show(this, "Please select a job.");
			}
		}

		private void menuGoToAccountMarketing_Click(object sender,EventArgs e) {
			try {
				GlobalFormOpenDental.GoToModule(EnumModuleType.Chart,patNum:((Job)gridMarketing.ListGridRows[gridMarketing.SelectedIndices[0]].Tag).ListJobQuotes[0].PatNum);
			}
			catch(Exception ex) {
				MsgBox.Show(this, "Please select a job.");
			}
		}
		#endregion

		#region Job Navigation Methods
		private void comboUser_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboUser.SelectedIndex==0) {//All
				comboUser.Tag=new Userod() { UserNum=0 };
			}
			else if(comboUser.SelectedIndex==1) {//Unassigned
				comboUser.Tag=new Userod() { UserNum=-1 };
			}
			else {
				comboUser.Tag=_listUsers[comboUser.SelectedIndex-2];
			}
			FillActiveTabGrid();
			this.Text="Job Manager"+(comboUser.Text=="" ? "" : " - "+comboUser.Text);
		}

		private void butMe_Click(object sender,EventArgs e) {
			if(comboUser.Tag!=Security.CurUser) {
				comboUser.Tag=Security.CurUser;
			}
			else {
				comboUser.Tag=new Userod();
			}
			FillComboUser();
			FillActiveTabGrid();
		}

		private void butBack_Click(object sender,EventArgs e) {
			if(_stackBackJobNums.Count>0) {
				long jobNumBack=_stackBackJobNums.Pop();
				GoToJob(jobNumBack,LoadJobAction.Back);
			}
		}

		private void butForward_Click(object sender,EventArgs e) {
			if(_stackForwardJobNums.Count>0) {
				long jobNumForward=_stackForwardJobNums.Pop();
				GoToJob(jobNumForward,LoadJobAction.Forward);
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			GoToJob(userControlJobManagerEditor.JobNumCur);
		}

		private void UpdateStack(Job job,LoadJobAction loadAction) {
			long jobNumCur=userControlJobManagerEditor.JobNumCur;
			switch(loadAction) {
				case LoadJobAction.Select:
					if(jobNumCur!=0) {
						_stackBackJobNums.Push(jobNumCur);
						butBack.Enabled=true;
					}
					if(_stackForwardJobNums.Count>0 && _stackForwardJobNums.Peek()==job.JobNum) {
						_stackForwardJobNums.Pop();
					}
					else {
						_stackForwardJobNums.Clear();
					}
					if(_stackForwardJobNums.Count==0) {
						butForward.Enabled=false;
					}
					break;
				case LoadJobAction.Back:
					if(_stackBackJobNums.Count==0) {
						butBack.Enabled=false;
					}
					if(jobNumCur!=0) {
						_stackForwardJobNums.Push(jobNumCur);
						butForward.Enabled=true;
					}
					break;
				case LoadJobAction.Forward:
					if(_stackForwardJobNums.Count==0) {
						butForward.Enabled=false;
					}
					if(jobNumCur!=0) {
						_stackBackJobNums.Push(jobNumCur);
						butBack.Enabled=true;
					}
					break;
			}
		}

		public void GoToJob(long jobNum) {
			GoToJob(jobNum,LoadJobAction.Select);
		}

		private void GoToJob(long jobNum,LoadJobAction loadAction) {
			Job job=Jobs.GetOneFilled(jobNum);
			if(job==null) {
				MessageBox.Show("Job not found.");
				return;
			}
			LoadJob(job,true,loadAction);
		}
		#endregion

		#region FillGrids
		//All of these Fill methods should only be called from FillActiveTabGrid
		///<summary>Always fills from _ListJobsAll.</summary>
		private void FillGridActions() {
			if(!tabControlNav.TabPages.Contains(tabAction)) {
				return;
			}
			_dicRowNotes.Clear();
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridAction.Title="Action Items";
			checkShowUnassigned.Enabled=true;
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridAction.BeginUpdate();
			gridAction.Columns.Clear();
			gridAction.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridAction.Columns.Add(new GridColumn("Flag",30,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridAction.Columns.Add(new GridColumn("Owner",65,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridAction.Columns.Add(new GridColumn("",245));
			gridAction.ListGridRows.Clear();
			//Sort jobs into action dictionary
			Dictionary<JobAction,List<Job>> dictActions=new Dictionary<JobAction,List<Job>>();
			foreach(Job job in JobManagerCore.ListJobsAll) {
				JobAction action=(job.Category==JobCategory.Query) ? job.OwnerActionForQuery : job.OwnerAction;
				if(userFilter!=null) {
					action=(job.Category==JobCategory.Query) ? job.ActionForQueryUser(userFilter) : job.ActionForUser(userFilter);
				}
				if(action.In(JobAction.Document,JobAction.ContactCustomerPreDoc)) {
					continue;
				}
				if(action.In(JobAction.NeedsTechnicalWriter,JobAction.ContactCustomer) && job.Category!=JobCategory.Query) {
					continue;
				}
				if(job.Category==JobCategory.SpecialProject || job.Category==JobCategory.MarketingDesign) {
					continue;
				}
				if(job.Category==JobCategory.Query && !JobPermissions.HasQueryPermission()) {
					continue;//Prevent non Query-Team members from seeing query jobs.
				}
				if(job.Category==JobCategory.Query && job.OwnerAction==JobAction.ReviewCode && !JobPermissions.HasQueryPermission(JobPerm.QueryCoordinator)) {
					continue;//Prevent Query Team members w/out sufficient permissions from seeing Query jobs that need a review(er).
				}
				if(comboProposedVersionNeedsAction.SelectedIndex!=0 && job.ProposedVersion!=comboProposedVersionNeedsAction.GetSelected<JobProposedVersion>()) {
					continue;//All is not selected, only select the jobs with the specified proposedversion
				}
				if(!dictActions.ContainsKey(action)) {
					dictActions[action]=new List<Job>();
				}
				dictActions[action].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictActions=dictActions.OrderBy(x=>(int)x.Key).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobAction,List<Job>> kvp in dictActions) {
				if(kvp.Key==JobAction.Undefined || kvp.Key==JobAction.UnknownJobPhase || kvp.Key==JobAction.None) {
					//Undefined occurs when there is no action to take. 
					//UnknownJobPhase occurs when there is something wrong with the programming.
					continue;
				}
				List<Job> listJobsSorted=kvp.Value//filter
					.OrderBy(x=>userFilter==null || x.OwnerNum!=userFilter.UserNum)//sort
					.ThenBy(x=>x.OwnerNum!=0)
					.ThenBy(x=>_listCategoriesSorted.IndexOf((int)x.Category))
					.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder)
					.ToList();
				if(!checkShowUnassigned.Checked) {
					for(int i=listJobsSorted.Count-1;i>=0;i--) {
						if(JobShouldBeHidden(listJobsSorted[i],userFilter,kvp.Key)) {
							listJobsSorted.Remove(listJobsSorted[i]);
						}
					}
				}
				if(listJobsSorted.Count==0) {
					continue;
				}
				if(listJobsSorted.Any(x=>x.Category==JobCategory.Query)) {
					if(JobPermissions.HasQueryPermission()) {
						listJobsSorted=SortQueryJobs(listJobsSorted);
					}
					else {
						listJobsSorted.RemoveAll(x=>x.Category==JobCategory.Query);
					}
				}
				//Add a 'category title' row to the grid which will have the corresponding JobAction enum as its tag.
				//Always show the count of sorted jobs within each job action category regardless if the category is collapsed or not.
				string jobActionRowTitleStr=$"{kvp.Key} ({listJobsSorted.Count})";
				gridAction.ListGridRows.Add(new GridRow("","","",jobActionRowTitleStr) { ColorBackG=_colorGridHeaderBack,Bold=true,Tag=kvp.Key });
				if(_listJobActionsCollapsed.Contains(kvp.Key)) {
					continue;
				}
				Color colorChanged=Security.CurUser.UserNum==9 ? Color.FromArgb(20,Color.LightGreen):Color.FromArgb(80,Color.LightGreen);
				foreach(Job job in listJobsSorted) {
					JobActiveLink activeLink=job.ListJobActiveLinks.Find(x => x.UserNum==userFilter?.UserNum && x.DateTimeEnd==DateTime.MinValue);
					Color colorActiveLink=(activeLink==null) ? Color.Empty : Color.FromArgb(90,Color.Cyan);
					long jobOwnerNum=job.Category==JobCategory.Query ? job.OwnerNumForQuery : job.OwnerNum;
					string ownerString=jobOwnerNum==0 ? "-" : Userods.GetName(jobOwnerNum);
					//If in ReviewCode (you are the reviewer for the job), add a string for who sent it to you
					if(kvp.Key==JobAction.ReviewCode) {
						ownerString+="\r\n("+Userods.GetName(job.UserNumEngineer)+")";
					}
					gridAction.ListGridRows.Add(CreateGridRowNeedsAction(job,ownerString,colorActiveLink,colorChanged));
				}
			}
			gridAction.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridAction);
		}

		#region FillGridActions Helpers

		private bool JobShouldBeHidden(Job job, Userod userFilter, JobAction key) {
			if(userFilter==null || userFilter.UserNum==0) { // UserNum 0 == Unassigned, null == All.
				return false;
			}
			if(job.Priority==_defNumIsOnHold || key==JobAction.TakeJob) {
				//Always hide "onHold" and "TakeJob" jobs, even if they're in the "AlwaysVisible" list.
				return true;
			}
			if(JobActionsAlwaysVisible(job.Category).Contains(key)) {
				return false; //Never hide things in the "AlwaysVisible" list.
			}
			if(job.Category==JobCategory.Query) {
				if(job.OwnerNumForQuery==0 
					|| !JobPermissions.HasQueryPermission(userNum:userFilter.UserNum) 
					|| !JobPermissions.HasQueryPermission(userNum:Security.CurUser.UserNum))  //Always hide Query jobs from people without Query Permissions, even if they're in the "AlwaysVisible" list.
				{
					return true;
				}
			}
			else if(job.OwnerNum==0) {
				return true;
			}
			return false;
		}

		/// <summary>Actions in this list will be not filtered by checkShowUnassigned, i.e.: Actions in this list will always show (assuming the user has the appropriate permissions - For example ApproveJob always shows if user has approval permission.)</summary>
		private List<JobAction> JobActionsAlwaysVisible(JobCategory category) {
			switch(category) {
				case JobCategory.Query:
					return new List<JobAction> {
						JobAction.NeedsQuote,
						JobAction.ReviewCode,
						JobAction.NeedsReviewer,
						JobAction.AssignEngineer
					};
				default:
					return new List<JobAction> {
						JobAction.WriteConcept,
						JobAction.WriteJob,
						JobAction.WriteCode,
						JobAction.ReviewCode,
						JobAction.ApproveChanges,
						JobAction.ApproveConcept,
						JobAction.ApproveJob
					};
			}
		}

		private List<Job> SortQueryJobs(List<Job> listJobsIn) {
			List<Job> listJobsSortedNew=new List<Job>(listJobsIn);
			List<KeyValuePair<int, Job>> indicesAndValues = listJobsSortedNew.Select((v, k) => new KeyValuePair<int, Job>(k, v)).Where(x => x.Value.Category == JobCategory.Query).ToList();
			List<int> listIndices = indicesAndValues.ConvertAll(x => x.Key);
			List<Job> listJobs = indicesAndValues.ConvertAll(x=>x.Value).OrderBy(x=>x.DateTimeJobApproval).ThenBy(x=>x.AckDateTime).ThenBy(x=>x.Priority).ThenBy(x=>x.JobNum).ToList();
			for (int i = 0; i < listIndices.Count; i++) {
				listJobsSortedNew[listIndices[i]]=listJobs[i];
			}
			return listJobsSortedNew;
		}

		private GridRow CreateGridRowNeedsAction(Job job,string ownerString,Color colorActiveLink,Color changedColor) {
			Def jobPriority=_listJobPriorities.Find(y => y.DefNum==job.Priority);
			List<GridCell> listGridCells=new List<GridCell>();
			//PRIORITY CELL
			GridCell cell=new GridCell(jobPriority.ItemName);
			cell.ColorBackG=jobPriority.ItemColor;
			cell.ColorText=(job.Priority==Defs.GetDefsForCategory(DefCat.JobPriorities).Find(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black;
			listGridCells.Add(cell);
			//FLAG CELL
			cell=new GridCell(FlagHelper(job,gridAction.ListGridRows.Count));
			//Set in FlagCellHelper(...), tag is reset everytime FillGridActions() is called.
			cell.ColorText=job.TagOD!=null ? (Color)job.TagOD : Color.Black;
			cell.ColorBackG=colorActiveLink;
			listGridCells.Add(cell);
			//OWNER CELL
			cell=new GridCell(ownerString);
			listGridCells.Add(cell);
			//DESCRIPTION CELL
			cell=new GridCell(GetJobDescription(job));
			cell.ColorBackG=GetGridCellBackgroundColor(job,changedColor);
			listGridCells.Add(cell);
			GridRow row=new GridRow(listGridCells.ToArray());
			row.Tag=job;
			return row;
		}

		private string GetJobDescription(Job job) {
			if(job==null){
				return "";
			}
			return (job.Category==JobCategory.Query) ? Jobs.GetQueryTitle(job) : job.ToString();
		}
		#endregion FillGridActions Helpers

		///<summary>Always fills from _ListJobsAll.</summary>
		private void FillGridSpecial() {
			if(!tabControlNav.TabPages.Contains(tabSpecialProjects)) {
				return;
			}
			_dicRowNotes.Clear();
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridSpecial.Title="Special Project Items";
			checkShowUnassigned.Enabled=true;
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridSpecial.BeginUpdate();
			gridSpecial.Columns.Clear();
			gridSpecial.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridSpecial.Columns.Add(new GridColumn("Flag",30,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridSpecial.Columns.Add(new GridColumn("Owner",65,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridSpecial.Columns.Add(new GridColumn("",245));
			gridSpecial.ListGridRows.Clear();
			//Sort jobs into action dictionary
			Dictionary<JobAction,List<Job>> dictActions=new Dictionary<JobAction,List<Job>>();
			foreach(Job job in JobManagerCore.ListJobsAll) {
				JobAction action;
				if(userFilter==null) {
					action=job.OwnerAction;
				}
				else {
					action=job.ActionForUser(userFilter);
				}
				if(action.In(JobAction.Document,JobAction.NeedsTechnicalWriter,JobAction.ContactCustomer,JobAction.ContactCustomerPreDoc)) {
					continue;
				}
				if(job.Category!=JobCategory.SpecialProject) {
					continue;
				}
				if(!dictActions.ContainsKey(action)) {
					dictActions[action]=new List<Job>();
				}
				dictActions[action].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictActions=dictActions.OrderBy(x=>(int)x.Key).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobAction,List<Job>> kvp in dictActions) {
				if(kvp.Key==JobAction.Undefined || kvp.Key==JobAction.UnknownJobPhase || kvp.Key==JobAction.None) {
					//Undefined occurs when there is no action to take. 
					//UnknownJobPhase occurs when there is something wrong with the programming.
					continue;
				}
				List<Job> listJobsSorted=kvp.Value//filter
					.OrderBy(x=>userFilter==null || x.OwnerNum!=userFilter.UserNum)//sort
					.ThenBy(x=>x.OwnerNum!=0)
					.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				if(!checkShowUnassigned.Checked) {
					listJobsSorted.RemoveAll(x=>x.Priority==_defNumIsOnHold);
					if(userFilter!=null) {
						//Actions in this list will be filtered by checkShowUnassigned. If not in this list, items will always show if applicable 
						//(For example ApproveJob always shows if user has approval permission.)
						JobAction[] JobActionsUnassigned=new JobAction[] {
							JobAction.WriteConcept,
							JobAction.WriteJob,
							JobAction.WriteCode,
							JobAction.TakeJob,
							JobAction.ReviewCode
						};
						if(userFilter.UserNum>0 && JobActionsUnassigned.Contains(kvp.Key)) {
							listJobsSorted.RemoveAll(x=>x.OwnerNum==0 || kvp.Key==JobAction.TakeJob);//filters out passive actions if unassigned. Bug.
						}
					}
				}
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridSpecial.ListGridRows.Add(new GridRow("","","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				JobAction[] writeAdviseReview=new[] { JobAction.WriteCode,JobAction.ReviewCode,JobAction.WaitForReview,JobAction.Advise };
				Color changedColor=Security.CurUser.UserNum==9 ? Color.FromArgb(20,Color.LightGreen):Color.FromArgb(80,Color.LightGreen);
				foreach(Job job in listJobsSorted) {
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					string ownerString=job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum);
					//If in ReviewCode (you are the reviewer for the job), add a string for who sent it to you
					if(kvp.Key==JobAction.ReviewCode) {
						ownerString+="\r\n("+Userods.GetName(job.UserNumEngineer)+")";
					}
					gridSpecial.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
						},
						new GridCell(FlagHelper(job,gridSpecial.ListGridRows.Count)) {
							ColorText=job.TagOD!=null ? (Color)job.TagOD : Color.Black//Set in FlagCellHelper(...), tag is reset everytime FillGridActions() is called.
							},
						new GridCell(ownerString),
						new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job,changedColor) }
						) {
						Tag=job
					}
					);
				}
			}
			gridSpecial.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridSpecial);
		}

		///<summary>Always fills from _ListJobsAll.</summary>
		private void FillGridDocumentation() {
			if(!tabControlNav.TabPages.Contains(tabDocumentation)) {
				return;
			}
			_dicRowNotes.Clear();
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true,userFilter.UserNum)) {
				gridDocumentation.Title="Action Items";
				tabAction.Text="Needs Action";
			}
			else {
				gridDocumentation.Title="Jobs To Document";
				tabAction.Text="Needs Documentation";
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridDocumentation.BeginUpdate();
			gridDocumentation.Columns.Clear();
			gridDocumentation.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridDocumentation.Columns.Add(new GridColumn("Version",95,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridDocumentation.Columns.Add(new GridColumn("",205));
			gridDocumentation.ListGridRows.Clear();
			//Sort jobs into action dictionary
			Dictionary<string,List<Job>> dictActions=new Dictionary<string,List<Job>>();
			foreach(Job job in JobManagerCore.ListJobsAll) {
				JobAction action;
				if(userFilter==null) {
					action=job.OwnerAction;
				}
				else {
					action=job.ActionForUser(userFilter);
				}
				if(!action.In(JobAction.Document,JobAction.NeedsTechnicalWriter)) {
					continue;
				}
				if(job.Category==JobCategory.Query || job.Category==JobCategory.MarketingDesign) {
					continue;
				}
				if(!String.IsNullOrEmpty(textDocumentationVersion.Text) && !job.JobVersion.Contains(textDocumentationVersion.Text)) {
					continue;
				}
				Userod user=new Userod() { UserName="Unassigned",UserNum=0 };
				if(action==JobAction.Document) {
					user=Userods.GetUser(job.UserNumDocumenter)??user;
				}
				if(!JobPermissions.IsAuthorized(JobPerm.DocumentationManager,true) && user.UserNum!=userFilter.UserNum && user.UserNum!=0) {
					continue;
				}
				if(!dictActions.ContainsKey(user.UserName)) {
					dictActions[user.UserName]=new List<Job>();
				}
				dictActions[user.UserName].Add(job);
			}
			//sort dictionary so actions will appear in same order. Current user will be sorted to the top.
			dictActions=dictActions.OrderByDescending(x=>x.Key==Security.CurUser.UserName).ThenBy(x=>x.Key).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<string,List<Job>> kvp in dictActions) {
				List<Job> listJobsSorted=kvp.Value//filter
					.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				if(listJobsSorted.Count==0) {
					continue;
				}
				if(listJobsSorted.Count>0) {
					gridDocumentation.ListGridRows.Add(new GridRow("","",kvp.Key) { ColorBackG=_colorGridHeaderBack,Bold=true });
					JobAction[] writeAdviseReview=new[] { JobAction.WriteCode,JobAction.ReviewCode,JobAction.WaitForReview,JobAction.Advise };
					foreach(Job job in listJobsSorted) {
						Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
						gridDocumentation.ListGridRows.Add(
							new GridRow(
								new GridCell(jobPriority.ItemName+
									(!writeAdviseReview.Contains(job.OwnerAction) ? ""
										: ((job.ListJobReviews.Count==0) ? ""
											: (job.ListJobReviews.Any(y=>y.ReviewStatus!=JobReviewStatus.Done) ? "\r\n(!)" : "\r\n(R)")))) {
									ColorBackG=jobPriority.ItemColor
								},
								new GridCell(job.JobVersion),
								new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }) {
								Tag=job
							}
						);
					}
				}
			}
			gridDocumentation.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridDocumentation);
		}

		private void FillGridTesting() {
			if(!tabControlNav.TabPages.Contains(tabTesting)) {
				return;
			}
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridTesting.BeginUpdate();
			gridTesting.Columns.Clear();
			gridTesting.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridTesting.Columns.Add(new GridColumn("Version",95,HorizontalAlignment.Center));
			gridTesting.Columns.Add(new GridColumn("Date Tested",70,HorizontalAlignment.Center));
			gridTesting.Columns.Add(new GridColumn("",150));
			gridTesting.ListGridRows.Clear();
			JobManagerCore.AddTestingJobsToList(textVersionText.Text);
			//Get a list of all jobs that should be tested.
			List<Job> listTestingJobs=JobManagerCore.ListJobsAll.FindAll(x=>!x.Category.In(
			JobCategory.Query,JobCategory.Research,JobCategory.Conversion,JobCategory.UnresolvedIssue,JobCategory.MarketingDesign,JobCategory.Project)
					&& x.PhaseCur.In(JobPhase.Complete,JobPhase.Documentation)
					&& (string.IsNullOrEmpty(textVersionText.Text) || x.JobVersion.ToLower().Contains(textVersionText.Text.ToLower()))
					&& (string.IsNullOrEmpty(textSearch.Text) || x.ToString().ToLower().Contains(textSearch.Text.ToLower())))
				.OrderBy(x=>Userods.GetUser(x.UserNumTester)?.UserName??"ZZZ")
				.ThenBy(x=>_listCategoriesSorted.IndexOf((int)x.Category))
				.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.PriorityTesting)?.ItemOrder??1000)
				.ToList();
			//Make a dictionary separated by users.
			Dictionary<long,List<Job>> dictTestingJobsByUser=listTestingJobs.GroupBy(x=>x.UserNumTester).ToDictionary(x=>x.Key,x=>x.ToList());
			//Fill the grid with the jobs.
			foreach(long userNum in dictTestingJobsByUser.Keys) {
				if(!checkShowAllUsers.Checked
					&& userFilter!=null
					&& userNum!=0
					&& userNum!=userFilter.UserNum)
				{
					continue;
				}
				//Every user will have their own section.  Might hide other users later once we start getting busy with testing.
				Userod user=Userods.GetUser(userNum)??new Userod() { UserName="Unassigned",UserNum=0 };
				gridTesting.ListGridRows.Add(new GridRow("","","",user.UserName) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in dictTestingJobsByUser[userNum]) {
					if(job.DateTimeTested.Year>1880 && checkHideTested.Checked) {
						continue;
					}
					if(job.IsNotTested && checkHideNotTested.Checked) {
						continue;
					}
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.PriorityTesting)??new Def() { ItemName="",ItemColor=Color.Empty };
					gridTesting.ListGridRows.Add(
						new GridRow(
							new GridCell(jobPriority.ItemName) {
								ColorBackG=jobPriority.ItemColor
							},
							new GridCell(job.JobVersion),
							new GridCell(job.DateTimeTested.Year>1880 ? job.DateTimeTested.ToShortDateString() : ""),
							new GridCell(job.ToString())
							)
						{
							Tag=job
						}
					);
				}
			}
			gridTesting.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridTesting);
		}

		private void FillGridNeedsEngineer() {
			if(!tabControlNav.TabPages.Contains(tabNeedsEngineer)) {
				return;
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			int jobCount=0;
			LayoutManager.Move(gridAvailableJobs,new Rectangle(0,33,tabControlNav.ClientSize.Width,tabControlNav.ClientSize.Height-33));
			gridAvailableJobs.BeginUpdate();
			gridAvailableJobs.Columns.Clear();
			gridAvailableJobs.Columns.Add(new GridColumn("Priority",50) { TextAlign=HorizontalAlignment.Center });
			gridAvailableJobs.Columns.Add(new GridColumn("",245));
			gridAvailableJobs.ListGridRows.Clear();
			Dictionary<JobCategory,List<Job>> dictCategories=GetDictJobsForTeamByCategory(comboTeamFilterNeedsEngineer.GetSelected<JobTeam>().JobTeamNum);
			dictCategories=dictCategories.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobCategory,List<Job>> kvp in dictCategories) {
				if(kvp.Key==JobCategory.Query || kvp.Key==JobCategory.MarketingDesign) {
					continue;
				}
				List<Job> listJobsSorted=kvp.Value.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				listJobsSorted.RemoveAll(x=>x.Priority==_defNumIsOnHold || x.OwnerAction!=JobAction.AssignEngineer);
				if(comboProposedVersionNeedsEngineer.SelectedIndex!=0) {//All is not selected, only select the jobs with the specified proposedversion
					listJobsSorted.RemoveAll(x=>x.ProposedVersion!=comboProposedVersionNeedsEngineer.GetSelected<JobProposedVersion>());
				}
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridAvailableJobs.ListGridRows.Add(new GridRow("",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in listJobsSorted) {
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridAvailableJobs.ListGridRows.Add(
						new GridRow(
							new GridCell(jobPriority.ItemName) { ColorBackG=jobPriority.ItemColor },
							new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
							) { Tag=job }
					);
				}
				jobCount+=listJobsSorted.Count;
			}
			gridAvailableJobs.EndUpdate();
			tabNeedsEngineer.Text="Needs Engineer ("+jobCount+")";
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridAvailableJobs);
		}

		///<summary>Fills tabProjectManagement with top parent Projects.</summary>
		private void FillGridProjectManagement() {
			if(!tabControlNav.TabPages.Contains(tabProjectManagement)) {
				return;
			}
			long jobTeamNum=comboTeamFilterProjectManagement.GetSelected<JobTeam>().JobTeamNum;
			List<Job> listJobs;
			if(checkOnlyShowTopLevel.Checked) {
				listJobs=GetListJobsForTeam(jobTeamNum).FindAll(x=>x.Category==JobCategory.Project && x.JobNum==x.TopParentNum && !x.PhaseCur.In(JobPhase.Complete,JobPhase.Cancelled));
			}
			else{
				listJobs=GetListJobsForTeam(jobTeamNum).FindAll(x=>x.Category==JobCategory.Project && !x.PhaseCur.In(JobPhase.Complete,JobPhase.Cancelled));
			}
			_dicRowNotes.Clear();
			gridProjectManagement.Title="Projects";
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridProjectManagement.BeginUpdate();
			gridProjectManagement.Columns.Clear();
			gridProjectManagement.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridProjectManagement.Columns.Add(new GridColumn("Owner",65,HorizontalAlignment.Center));
			gridProjectManagement.Columns.Add(new GridColumn("",245));
			gridProjectManagement.ListGridRows.Clear();
			//Sort jobs into action dictionary
			Dictionary<JobAction,List<Job>> dictActions=new Dictionary<JobAction,List<Job>>();
			foreach(Job job in listJobs) {
				JobAction action=job.OwnerAction;
				if(!dictActions.ContainsKey(action)) {
					dictActions[action]=new List<Job>();
				}
				dictActions[action].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictActions=dictActions.OrderBy(x=>(int)x.Key).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobAction,List<Job>> kvp in dictActions) {
				if(kvp.Key==JobAction.Undefined || kvp.Key==JobAction.UnknownJobPhase || kvp.Key==JobAction.None) {
					//Undefined occurs when there is no action to take.
					//UnknownJobPhase occurs when there is something wrong with the programming.
					continue;
				}
				List<Job> listJobsSorted=kvp.Value//filter
					.OrderBy(x=>x.OwnerNum!=0)//sort
					.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				if(listJobsSorted.Count==0) {
					continue;
				}
				//Add a 'category title' row to the grid which will have the corresponding JobAction enum as its tag.
				//Always show the count of sorted jobs within each job action category regardless if the category is collapsed or not.
				string jobActionRowTitleStr=$"{kvp.Key} ({listJobsSorted.Count})";
				gridProjectManagement.ListGridRows.Add(new GridRow("","",jobActionRowTitleStr) { ColorBackG=_colorGridHeaderBack,Bold=true,Tag=kvp.Key });
				if(_listJobActionsCollapsedProject.Contains(kvp.Key)) {
					continue;
				}
				JobAction[] writeAdviseReview=new[] { JobAction.WriteCode,JobAction.ReviewCode,JobAction.WaitForReview,JobAction.Advise };
				foreach(Job job in listJobsSorted) {
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					string ownerString=job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum);
					gridProjectManagement.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
						},
						new GridCell(ownerString),
						new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
						) {
						Tag=job
					}
					);
				}
			}
			gridProjectManagement.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridProjectManagement);
		}

		private void FillGridNeedsExpert() {
			if(!tabControlNav.TabPages.Contains(tabNeedsExpert)) {
				return;
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			int jobCount = 0;
			LayoutManager.Move(gridAvailableJobsExpert,new Rectangle(0,33,tabControlNav.ClientSize.Width,tabControlNav.ClientSize.Height-33));
			gridAvailableJobsExpert.BeginUpdate();
			gridAvailableJobsExpert.Columns.Clear();
			gridAvailableJobsExpert.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridAvailableJobsExpert.Columns.Add(new GridColumn("",245));
			gridAvailableJobsExpert.ListGridRows.Clear();
			Dictionary<JobCategory,List<Job>> dictCategories=GetDictJobsForTeamByCategory(comboTeamFilterNeedsExpert.GetSelected<JobTeam>().JobTeamNum);
			dictCategories=dictCategories.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			jobCount=0;
			foreach(KeyValuePair<JobCategory,List<Job>> kvp in dictCategories) {
				if(kvp.Key==JobCategory.Query||kvp.Key==JobCategory.MarketingDesign) {
					continue;
				}
				List<Job> listJobsSorted = kvp.Value.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				listJobsSorted.RemoveAll(x=>x.Priority==_defNumIsOnHold||x.OwnerAction!=JobAction.AssignExpert);
				if(comboProposedVersionNeedsExpert.SelectedIndex!=0) {//All is not selected, only select the jobs with the specified proposedversion
					listJobsSorted.RemoveAll(x=>x.ProposedVersion!=comboProposedVersionNeedsExpert.GetSelected<JobProposedVersion>());
				}
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridAvailableJobsExpert.ListGridRows.Add(new GridRow("",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in listJobsSorted) {
					Def jobPriority = _listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridAvailableJobsExpert.ListGridRows.Add(
						new GridRow(
							new GridCell(jobPriority.ItemName) { ColorBackG=jobPriority.ItemColor },
							new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
							) { Tag=job }
						);
				}
				jobCount+=listJobsSorted.Count;
			}
			gridAvailableJobsExpert.EndUpdate();
			tabNeedsExpert.Text="Needs Expert ("+jobCount+")";
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridAvailableJobsExpert);
		}

		private void FillGridOnHold() {
			if(!tabControlNav.TabPages.Contains(tabOnHold)) {
				return;
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			int jobCount = 0;
			gridJobsOnHold.BeginUpdate();
			gridJobsOnHold.Columns.Clear();
			gridJobsOnHold.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridJobsOnHold.Columns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridJobsOnHold.Columns.Add(new GridColumn("",245));
			gridJobsOnHold.ListGridRows.Clear();
			Dictionary<JobCategory,List<Job>> dictCategories = JobManagerCore.ListJobsAll.GroupBy(x=>x.Category).ToDictionary(y=>y.Key,y=>y.ToList());
			dictCategories=dictCategories.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			jobCount=0;
			foreach(KeyValuePair<JobCategory,List<Job>> kvp in dictCategories) {
				if(kvp.Key==JobCategory.Query||kvp.Key==JobCategory.MarketingDesign) {
					continue;
				}
				List<Job> listJobsSorted = kvp.Value.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				listJobsSorted.RemoveAll(x=>x.Priority!=_defNumIsOnHold||x.PhaseCur==JobPhase.Complete||x.PhaseCur==JobPhase.Documentation||x.PhaseCur==JobPhase.Cancelled);
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridJobsOnHold.ListGridRows.Add(new GridRow("","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in listJobsSorted) {
					Def jobPriority = _listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridJobsOnHold.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) { ColorBackG=jobPriority.ItemColor },
						new GridCell(job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum)),
						new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
						) { Tag=job }
					);
				}
				jobCount+=listJobsSorted.Count;
			}
			gridJobsOnHold.EndUpdate();
			tabOnHold.Text="On Hold ("+jobCount+")";
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridJobsOnHold);
		}

		private void FillGridPatternReview() {
			if(!tabControlNav.TabPages.Contains(tabPatternReview)) {
				return;
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			int jobCount = 0;
			gridPatternReview.ContextMenu=contextMenuPatternReview;
			gridPatternReview.BeginUpdate();
			gridPatternReview.Columns.Clear();
			gridPatternReview.Columns.Add(new GridColumn("DateC",65,HorizontalAlignment.Center));
			gridPatternReview.Columns.Add(new GridColumn("Owner",50,HorizontalAlignment.Center));
			gridPatternReview.Columns.Add(new GridColumn("Status",55,HorizontalAlignment.Center));
			gridPatternReview.Columns.Add(new GridColumn("",245));
			gridPatternReview.ListGridRows.Clear();
			Dictionary<JobCategory,List<Job>> dictCategories = JobManagerCore.ListJobsAll.GroupBy(x=>x.Category).ToDictionary(y=>y.Key,y=>y.ToList());
			dictCategories=dictCategories.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			jobCount=0;
			foreach(KeyValuePair<JobCategory,List<Job>> kvp in dictCategories) {
				if(kvp.Key.In(JobCategory.Query,JobCategory.Conversion,JobCategory.MarketingDesign,JobCategory.UnresolvedIssue)) {
					continue;
				}
				List<Job> listJobsSorted = kvp.Value.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				listJobsSorted.RemoveAll(x=>!x.PatternReviewStatus.In(JobPatternReviewStatus.AwaitingApproval,JobPatternReviewStatus.Tentative)
					||x.PatternReviewProject!=JobPatternReviewProject.OD
					||!x.PhaseCur.In(JobPhase.Complete,JobPhase.Documentation,JobPhase.Development)
					||x.DateTimeImplemented<dateExcludeCompleteBefore.Value);
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridPatternReview.ListGridRows.Add(new GridRow("","","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in listJobsSorted) {
					Def jobPriority = _listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridPatternReview.ListGridRows.Add(
						new GridRow(
							new GridCell(job.DateTimeImplemented==DateTime.MinValue ? "" : job.DateTimeImplemented.ToShortDateString()),
							new GridCell(Userods.GetName(job.OwnerNum)),
							new GridCell(job.PatternReviewStatus.ToString()),
							new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
							) { Tag=job }
						);
				}
				jobCount+=listJobsSorted.Count;
			}
			gridPatternReview.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridPatternReview);
		}

		private void FillGridNotify() {
			if(!tabControlNav.TabPages.Contains(tabNotify)) {
				return;
			}
			_dicRowNotes.Clear();
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridNotify.Title="Action Items";
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridNotify.BeginUpdate();
			gridNotify.Columns.Clear();
			gridNotify.Columns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridNotify.Columns.Add(new GridColumn("Version",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridNotify.Columns.Add(new GridColumn("",245));
			gridNotify.ListGridRows.Clear();
			//Sort jobs into action dictionary
			Dictionary<JobAction,List<Job>> dictActions=new Dictionary<JobAction,List<Job>>();
			foreach(Job job in JobManagerCore.ListJobsAll) {
				JobAction action;
				if(userFilter==null) {
					action=job.OwnerAction;
				}
				else {
					action=job.ActionForUser(userFilter);
				}
				if(!action.In(JobAction.ContactCustomer,JobAction.ContactCustomerPreDoc)) {
					continue;
				}
				if(job.Category==JobCategory.Query || job.Category==JobCategory.MarketingDesign) {
					continue;
				}
				if((checkNotifyShowHqOnly.Checked && job.Category==JobCategory.Feature) //Show HQ Only, do not show Features
					|| (!checkNotifyShowHqOnly.Checked && job.Category==JobCategory.HqRequest)) {//Or, do not show HQ jobs, only show Features
					continue;
				}
				if(!dictActions.ContainsKey(action)) {
					dictActions[action]=new List<Job>();
				}
				dictActions[action].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictActions=dictActions.OrderBy(x=>(int)x.Key).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobAction,List<Job>> kvp in dictActions) {
				if(kvp.Key==JobAction.Undefined || kvp.Key==JobAction.UnknownJobPhase || kvp.Key==JobAction.None) {
					//Undefined occurs when there is no action to take. 
					//UnknownJobPhase occurs when there is something wrong with the programming.
					continue;
				}
				List<Job> listJobsSorted=kvp.Value//filter
					.OrderBy(x=>userFilter==null || x.OwnerNum!=userFilter.UserNum)//sort
					.ThenBy(x=>x.OwnerNum!=0)
					.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
				if(listJobsSorted.Count==0) {
					continue;
				}
				//Remove non-HqRequest jobs with no FR for the ContactCustomer permission because documentation users should never have customer contact permission as well.
				listJobsSorted.RemoveAll(x=>kvp.Key.In(JobAction.ContactCustomer,JobAction.ContactCustomerPreDoc)
								&& x.Category!=JobCategory.HqRequest && !x.ListJobLinks.Any(y=>y.LinkType==JobLinkType.Request));
				if(listJobsSorted.Count>0) {
					gridNotify.ListGridRows.Add(new GridRow("","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
					JobAction[] writeAdviseReview=new[] { JobAction.WriteCode,JobAction.ReviewCode,JobAction.WaitForReview,JobAction.Advise };
					foreach(Job job in listJobsSorted) {
						Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
						gridNotify.ListGridRows.Add(
							new GridRow(
								new GridCell(job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum)),
								new GridCell(job.JobVersion),
								new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }) {
								Tag=job
							}
						);
					}
				}
			}
			gridNotify.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridNotify);
		}

		private void FillGridSubscribed() {
			if(!tabControlNav.TabPages.Contains(tabSubscribed)) {
				return;
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			int jobCount=0;
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			//Filter down to all incomplete jobs with subscription for selected user.
			List<Job> listJobsFiltered=JobManagerCore.ListJobsAll.Where(x=>x.ListJobLinks.Exists(y=>y.LinkType==JobLinkType.Subscriber && y.FKey==userFilter.UserNum) && x.PhaseCur!=JobPhase.Complete).ToList();
			//Optionally exclude OnHold jobs.
			if(!checkSubscribedIncludeOnHold.Checked) {
				listJobsFiltered.RemoveAll(x=>x.Priority==_defNumIsOnHold);
			}
			//Order by priority.
			listJobsFiltered=listJobsFiltered.OrderBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
			//Group jobs in dictionary by category
			Dictionary<JobCategory,List<Job>> dictCategories=listJobsFiltered.GroupBy(x=>x.Category).ToDictionary(y=>y.Key,y=>y.ToList());
			dictCategories=dictCategories.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			gridSubscribedJobs.BeginUpdate();
			gridSubscribedJobs.Columns.Clear();
			gridSubscribedJobs.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridSubscribedJobs.Columns.Add(new GridColumn("Expert",55,HorizontalAlignment.Center));
			gridSubscribedJobs.Columns.Add(new GridColumn("Engineer",55,HorizontalAlignment.Center));
			gridSubscribedJobs.Columns.Add(new GridColumn("Phase",85,HorizontalAlignment.Center));
			gridSubscribedJobs.Columns.Add(new GridColumn("",85){ IsWidthDynamic=true });
			gridSubscribedJobs.ListGridRows.Clear();
			//Create rows and cells for jobs of each category
			foreach(KeyValuePair<JobCategory,List<Job>> kvp in dictCategories) {
				List<Job> listJobsSorted=kvp.Value;
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridSubscribedJobs.ListGridRows.Add(new GridRow("","","","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in listJobsSorted) {
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					string note=JobNotifications.GetNoteForChanges(job.ListJobNotifications.Find(x=>x.UserNum==Security.CurUser.UserNum)?.Changes??JobNotificationChanges.None);
					gridSubscribedJobs.ListGridRows.Add(
						new GridRow(
							new GridCell(jobPriority.ItemName),
							new GridCell(job.UserNumExpert==0?"-":Userods.GetName(job.UserNumExpert)),
							new GridCell(job.UserNumEngineer==0?"-":Userods.GetName(job.UserNumEngineer)),
							new GridCell(job.PhaseCur.ToString()),
							new GridCell(job.ToString())
							) {
							Tag=job,
							Note=note,
							ColorBackG=string.IsNullOrEmpty(note)?Color.White:Color.LightSalmon
						}
					);
				}
				jobCount+=listJobsSorted.Count;
			}
			gridSubscribedJobs.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridSubscribedJobs);
		}

		private void FillGridQueries() {
			if(!tabControlNav.TabPages.Contains(tabQuery)) {
				return;
			}
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridQueries.ContextMenu=contextMenuQueries;
			gridQueries.Title="Query Jobs";
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridQueries.BeginUpdate();
			gridQueries.Columns.Clear();
			gridQueries.Columns.Add(new GridColumn("Sched Date",70,HorizontalAlignment.Center));//Oldest at the top
			gridQueries.Columns.Add(new GridColumn("Priority",70,HorizontalAlignment.Center));
			gridQueries.Columns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridQueries.Columns.Add(new GridColumn("",225));
			gridQueries.ListGridRows.Clear();
			//Sort jobs into action dictionary
			List<Job> listJobsSorted=JobManagerCore.ListJobsAll
				.Where(x=>x.Category==JobCategory.Query
				&& _listQueryPriorityFilter.Contains(x.Priority)
				&& _listQueryJobPhaseFilter.Contains(x.PhaseCur))
				.ToList();
			listJobsSorted=listJobsSorted.OrderBy(x=>x.AckDateTime).ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
			Dictionary<JobPhase,List<Job>> dictPhases=new Dictionary<JobPhase,List<Job>>();
			foreach(Job job in listJobsSorted) {
				if(userFilter!=null && userFilter.UserNum!=job.UserNumEngineer) {
					continue;
				}
				if(!checkShowQueryComplete.Checked && job.PhaseCur==JobPhase.Complete) {
					continue;
				}
				if(!checkShowQueryCancelled.Checked && job.PhaseCur==JobPhase.Cancelled) {
					continue;
				}
				JobPhase phase=job.PhaseCur;
				if(!dictPhases.ContainsKey(phase)) {
					dictPhases[phase]=new List<Job>();
				}
				dictPhases[phase].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictPhases=dictPhases.OrderBy(x=>_listPhasesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobPhase,List<Job>> kvp in dictPhases) {
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridQueries.ListGridRows.Add(new GridRow("","","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in kvp.Value) {
					Color backColor=Color.White;
					if(selectedJobNum>0
						&&userControlJobManagerEditor.JobCur.ListJobQuotes.Count!=0
						&&job.ListJobQuotes.FirstOrDefault().PatNum==userControlJobManagerEditor.JobCur.ListJobQuotes.FirstOrDefault().PatNum) {
						backColor=Color.LightBlue;
					}
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridQueries.ListGridRows.Add(
					new GridRow(
						new GridCell(job.AckDateTime.Year>1880?job.AckDateTime.ToShortDateString():"N/A"),
						new GridCell($"{jobPriority.ItemName}\r\n{(job.DateTimeTested.Year>1880?job.DateTimeTested.ToShortDateString():"")}") {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
						},
						new GridCell(job.UserNumEngineer==0 ? "-" : Userods.GetName(job.UserNumEngineer)),
						new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
						) {
						Tag=job,
						ColorBackG=backColor
					}
					);
				}
			}
			gridQueries.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridQueries);
		}

		private void FillGridMarketing() {
			if(!tabControlNav.TabPages.Contains(tabMarketing)) {
				return;
			}
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridMarketing.ContextMenu=contextMenuMarketing;
			gridMarketing.Title="Marketing/Design Jobs";
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridMarketing.BeginUpdate();
			gridMarketing.Columns.Clear();
			gridMarketing.Columns.Add(new GridColumn("Appr Date",70,HorizontalAlignment.Center));//Oldest at the top
			gridMarketing.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridMarketing.Columns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridMarketing.Columns.Add(new GridColumn("",245));
			gridMarketing.ListGridRows.Clear();
			//Sort jobs into action dictionary
			List<Job> listJobsSorted=JobManagerCore.ListJobsAll.Where(x=>x.Category==JobCategory.MarketingDesign).ToList();
			listJobsSorted=listJobsSorted.OrderBy(x=>x.DateTimeCustContact).ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder).ToList();
			Dictionary<JobPhase,List<Job>> dictPhases=new Dictionary<JobPhase,List<Job>>();
			foreach(Job job in listJobsSorted) {
				if(userFilter!=null && userFilter.UserNum!=job.UserNumEngineer) {
					continue;
				}
				if(!checkShowDesignComplete.Checked && job.PhaseCur==JobPhase.Complete)
				{
					continue;
				}
				if(!checkShowDesignCancelled.Checked && job.PhaseCur==JobPhase.Cancelled)
				{
					continue;
				}
				JobPhase phase=job.PhaseCur;
				if(!dictPhases.ContainsKey(phase)) {
					dictPhases[phase]=new List<Job>();
				}
				dictPhases[phase].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictPhases=dictPhases.OrderBy(x=>_listPhasesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			foreach(KeyValuePair<JobPhase,List<Job>> kvp in dictPhases) {
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridMarketing.ListGridRows.Add(new GridRow("","","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in kvp.Value) {
					Color backColor=Color.White;
					if(selectedJobNum>0
						&& userControlJobManagerEditor.JobCur.ListJobQuotes.Count!=0
						&& job.ListJobQuotes.FirstOrDefault().PatNum==userControlJobManagerEditor.JobCur.ListJobQuotes.FirstOrDefault().PatNum) {
						backColor=Color.LightBlue;
					}
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridMarketing.ListGridRows.Add(
					new GridRow(
						new GridCell(job.DateTimeCustContact.Year>1880?job.DateTimeCustContact.ToShortDateString():"N/A"),
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
						},
						new GridCell(job.UserNumEngineer==0 ? "-" : Userods.GetName(job.UserNumEngineer)),
						new GridCell(job.ToString()) { ColorBackG=GetGridCellBackgroundColor(job) }
						) {
						Tag=job,
						ColorBackG=backColor
					}
					);
				}
			}
			gridMarketing.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridMarketing);
		}

		///<summary>Always fills from _ListJobsAll.</summary>
		private void FillGridUnresolvedIssues() {
			if(!tabControlNav.TabPages.Contains(tabUnresolvedIssues)) {
				return;
			}
			Userod userFilter=Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			long selectedJobNum=userControlJobManagerEditor.JobNumCur;
			gridUnresolvedIssues.BeginUpdate();
			gridUnresolvedIssues.Columns.Clear();
			gridUnresolvedIssues.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridUnresolvedIssues.Columns.Add(new GridColumn("",150));
			gridUnresolvedIssues.ListGridRows.Clear();
			//Get a list of all jobs that should be tested.
			List<Job> listUnresolvedIssues=JobManagerCore.ListJobsAll.FindAll(x=>x.Category.In(JobCategory.UnresolvedIssue) && x.PhaseCur.In(JobPhase.Concept,JobPhase.Cancelled))
				.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Category))
				.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority)?.ItemOrder??1000)
				.ToList();
			//Fill the grid with the jobs.
			foreach(Job job in listUnresolvedIssues) {
				if(job.PhaseCur==JobPhase.Cancelled && !checkIncludeCancelledUnresolved.Checked) {
					continue;
				}
				Def jobPriority = _listJobPriorities.Find(y=>y.DefNum==job.Priority)??new Def() { ItemName="",ItemColor=Color.Empty };
				gridUnresolvedIssues.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor
						},
						new GridCell(job.ToString())
						) {
						Tag=job
					}
				);
			}
			gridUnresolvedIssues.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridUnresolvedIssues);
		}

		///<summary>Always fills from _ListJobsAll.</summary>
		private void FillGridSubmittedJobs() {
			if(!tabControlNav.TabPages.Contains(tabSubmittedJobs)) {
				return;
			}
			Userod userFilter = Security.CurUser;
			if(comboUser.SelectedIndex==1) {
				userFilter=new Userod() { UserName="Unassigned",UserNum=0 };
			}
			//If all user set userFilter to null in order to get all jobs
			if(comboUser.SelectedIndex==0) {
				userFilter=null;
			}
			else if(comboUser.SelectedIndex>1) {
				userFilter=_listUsers[comboUser.SelectedIndex-2];
			}
			gridSubmittedJobs.Title="Submitted Jobs";
			checkShowOnHoldSubmitted.Enabled=true;
			long selectedJobNum = userControlJobManagerEditor.JobNumCur;
			gridSubmittedJobs.BeginUpdate();
			gridSubmittedJobs.Columns.Clear();
			gridSubmittedJobs.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridSubmittedJobs.Columns.Add(new GridColumn("Owner",65,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridSubmittedJobs.Columns.Add(new GridColumn("",245));
			gridSubmittedJobs.ListGridRows.Clear();
			List<Job> listJobsFiltered = new List<Job>();
			if(userFilter!=null) {
				listJobsFiltered=JobManagerCore.ListJobsAll.Where(x=>x.UserNumConcept==userFilter.UserNum && !x.PhaseCur.In(JobPhase.Complete,JobPhase.Cancelled)).ToList();
			}
			else {
				listJobsFiltered=JobManagerCore.ListJobsAll;
			}
			List<Job> listJobsSorted = listJobsFiltered
				.OrderBy(x=>_listCategoriesSorted.IndexOf((int)x.Category))
				.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder)
				.ToList();
			if(!checkShowOnHoldSubmitted.Checked) {
				listJobsSorted.RemoveAll(x=>x.Priority==_defNumIsOnHold);
			}
			Color changedColor = Security.CurUser.UserNum==9 ? Color.FromArgb(20,Color.LightGreen) : Color.FromArgb(80,Color.LightGreen);
			foreach(Job job in listJobsSorted) {
				JobActiveLink activeLink = job.ListJobActiveLinks.Find(x=>x.UserNum==userFilter?.UserNum&&x.DateTimeEnd==DateTime.MinValue);
				Def jobPriority = _listJobPriorities.Find(y=>y.DefNum==job.Priority);
				string ownerString = job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum);
				gridSubmittedJobs.ListGridRows.Add(
				new GridRow(
					new GridCell(jobPriority.ItemName) {
						ColorBackG=jobPriority.ItemColor,
						ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
					},
					new GridCell(ownerString),
					new GridCell(job.ToString()) {
						ColorBackG=GetGridCellBackgroundColor(job,changedColor)
					}
					) {
					Tag=job
				}
				);
			}
			gridSubmittedJobs.EndUpdate();
			//RESELECT JOB
			ReselectJob(selectedJobNum,gridSubmittedJobs);
		}

		#region FillGrid Helper Methods

		///<summary>Helper method. Returns a dictionary of lists of jobs per job category for a given jobTeamNum.</summary>
		private Dictionary<JobCategory,List<Job>> GetDictJobsForTeamByCategory(long jobTeamNum,List<Job> listJobs=null) {
			return GetListJobsForTeam(jobTeamNum,listJobs).GroupBy(x=>x.Category).ToDictionary(y=>y.Key,y=>y.ToList());
		}

		///<summary>Filters the passed in list of jobs by JobTeamNum. Considers -1 and -2 for comboBox 'none' and 'all' options.
		///If listJobs is null, it defaults to JobManagerCore.ListJobsAll.</summary>
		private List<Job> GetListJobsForTeam(long jobTeamNum,List<Job> listJobs=null) {
			listJobs??=JobManagerCore.ListJobsAll;//Default list to all jobs if null
			switch(jobTeamNum) {
				case -1://None selected, only jobs not assigned to a team
					return listJobs.FindAll(x=>!x.ListJobLinks.Exists(y=>y.LinkType==JobLinkType.JobTeam));
				case -2://All selected, all jobs regardless of team
					return listJobs;
				default://Team selected, only jobs for that team
					return listJobs.FindAll(x=>x.ListJobLinks.Exists(y=>y.LinkType==JobLinkType.JobTeam && y.FKey==jobTeamNum));
			}
		}

		///<summary>Reselects job that was 'currently selected' prior to a fillgrid action. Early return on jobNum==0 or if passed in JobNum is different from currently loaded job (in userControlJobManagerEditor.JobNumCur)</summary>
		private void ReselectJob(long selectedJobNum,GridOD gridOd) {
			if(selectedJobNum==0 || selectedJobNum!=userControlJobManagerEditor.JobNumCur) {
				return;
			}
			for(int i=0;i<gridOd.ListGridRows.Count;i++) {
				if(gridOd.ListGridRows[i].Tag is Job job && job.JobNum==selectedJobNum) {
					gridOd.SetSelected(i,true);
					return;
				}
			}
		}

		///<summary>Returns a Color based on various statuses: Light Yellow if search string is found in [job].ToString(), [changedColor] if curUser has 1+ notifications for [job], Color.Empty if no conditions met</summary>
		private Color GetGridCellBackgroundColor(Job job,Color changedColor=default) {
			if(!string.IsNullOrWhiteSpace(textSearch.Text) && GetJobDescription(job).IndexOf(textSearch.Text,StringComparison.CurrentCultureIgnoreCase)>=0) {
				return Color.LightYellow;
			}
			else if(job.ListJobNotifications.Count(x=>x.UserNum==Security.CurUser.UserNum)>0) {
				return changedColor;
			}
			return Color.Empty;
		}

		#endregion FillGrid Helper Methods

		#endregion FillGrids

		#region Job Quick Search

		private void FillGridSearch() {
			if(!tabControlNav.TabPages.Contains(tabSearch)) {
				return;
			}
			Job selectedJob=userControlJobManagerEditor.JobCur;
			long selectedJobNum=0;
			if(selectedJob!=null) {
				selectedJobNum=selectedJob.JobNum;
			}
			gridSearch.BeginUpdate();
			gridSearch.Columns.Clear();
			gridSearch.Columns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridSearch.Columns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));// X for yes, - for unassigned
			gridSearch.Columns.Add(new GridColumn("",245));
			gridSearch.ListGridRows.Clear();
			//Sort jobs into action dictionary
			List<Job> listJobsForSearch=JobManagerCore.ListJobsAll;
			if(checkResults.Checked) {
				listJobsForSearch=JobManagerCore.ListJobsSearch;
			}
			List<Job> listJobsSorted=listJobsForSearch.Where(x=>x.ToString().ToLower().Contains(textSearch.Text.ToLower())).ToList();
			if(!checkContactSearch.Checked) {
				listJobsSorted.RemoveAll(x=>x.PhaseCur.In(JobPhase.Complete) && x.OwnerAction==JobAction.ContactCustomer && x.ListJobLinks.Count(y=>y.LinkType==JobLinkType.Request)!=0);
			}
			if(comboCatSearch.SelectedIndex!=0) {//All is not selected
				listJobsSorted=listJobsSorted.Where(x=>x.Category==comboCatSearch.GetSelected<JobCategory>()).ToList();
			}
			if(comboProposedVersionSearch.SelectedIndex!=0) {//All is not selected
				listJobsSorted=listJobsSorted.Where(x=>x.ProposedVersion==comboProposedVersionSearch.GetSelected<JobProposedVersion>()).ToList();
			}
			if(comboPrioritySearch.SelectedIndices.Count > 0 && !comboPrioritySearch.SelectedIndices.Contains(0)) {
				//At least one item is selected, but none of them are 'All'.
				List<long> listDefNumsSelected=comboPrioritySearch.GetListSelected<Def>().Select(x=>x.DefNum).ToList();
				listJobsSorted=listJobsSorted.FindAll(x=>listDefNumsSelected.Contains(x.Priority));
			}
			listJobsSorted=GetListJobsForTeam(comboTeamSearch.GetSelected<JobTeam>().JobTeamNum,listJobsSorted);
			if(!String.IsNullOrEmpty(textUserSearch.Text)) {
				listJobsSorted=listJobsSorted.Where(x=>Userods.GetName(x.OwnerNum).ToLower().Contains(textUserSearch.Text.ToLower())).ToList();
			}
			listJobsSorted=listJobsSorted.OrderBy(x=>x.OwnerNum!=0)
				.ThenBy(x=>_listCategoriesSorted.IndexOf((int)x.Category))
				.ThenBy(x=>_listJobPriorities.Find(y=>y.DefNum==x.Priority).ItemOrder)
				.ToList();
			Dictionary<JobPhase,List<Job>> dictPhases=new Dictionary<JobPhase,List<Job>>();
			foreach(Job job in listJobsSorted) {
				JobPhase phase=job.PhaseCur;
				if(!dictPhases.ContainsKey(phase)) {
					dictPhases[phase]=new List<Job>();
				}
				dictPhases[phase].Add(job);
			}
			//sort dictionary so actions will appear in same order
			dictPhases=dictPhases.OrderBy(x=>_listPhasesSorted.IndexOf((int)x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
			int indexSelectedJob=-1;
			foreach(KeyValuePair<JobPhase,List<Job>> kvp in dictPhases) {
				if(listJobsSorted.Count==0) {
					continue;
				}
				gridSearch.ListGridRows.Add(new GridRow("","",kvp.Key.ToString()) { ColorBackG=_colorGridHeaderBack,Bold=true });
				foreach(Job job in kvp.Value) {
					Color backColor=Color.White;
					bool isSelectedJob=selectedJobNum>0 && job.JobNum==selectedJobNum;
					if(isSelectedJob) {
						backColor=Color.LightBlue;//Currently selected job should stand out.
						indexSelectedJob=gridSearch.ListGridRows.Count;
					}
					Def jobPriority=_listJobPriorities.Find(y=>y.DefNum==job.Priority);
					gridSearch.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_defNumIsUrgent) ? Color.White : Color.Black,
						},
						new GridCell(job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum)),
						new GridCell(job.ToString())
						) {
						Tag=job,
						ColorBackG=backColor
					}
					);
				}
			}
			gridSearch.EndUpdate();
			if(indexSelectedJob>-1) {
				gridSearch.SetSelected(indexSelectedJob,true);
			}
		}

		private void comboPrioritySearch_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboCatSearch_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboProposedVersionNeedsAction_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboProposedVersionNeedsEngineer_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboTeamFilterNeedsExpert_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboTeamFilterNeedsEngineer_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboTeamFilterProjectManagement_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboProposedVersionNeedsExpert_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboProposedVersionSearch_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void comboTeamSearch_SelectionChangeCommitted(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void timerUserSearch_Tick(object sender,EventArgs e) {
			timerUserSearch.Stop();
			FillActiveTabGrid();
		}

		private void checkCompleteSearch_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkCancelledSearch_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkContactSearch_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void textUserSearch_TextChanged(object sender,EventArgs e) {
			timerUserSearch.Stop();
			timerUserSearch.Start();
		}

		private void gridSearch_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		#endregion

		#region Grid Events
		private void gridAction_MouseMove(object sender,MouseEventArgs e) {
			if(gridAction.Title!="Action Items") {
				return;//Only show toolTip when Non-Documentation.
			}
			if(_toolTipHover.Tag!=null && (Point)_toolTipHover.Tag==e.Location) {
				return;//Mouse has not moved. Avoid flicker.
			}
			_toolTipHover.Tag=e.Location;
			GridOD grid=(GridOD)sender;
			int row=grid.PointToRow(grid.PointToClient(Cursor.Position).Y);
			int col=grid.PointToCol(grid.PointToClient(Cursor.Position).X);
			if(row==-1 || !_dicRowNotes.ContainsKey(row) || col!=1) {
				_toolTipHover.RemoveAll();
				return;
			}
			_toolTipHover.SetToolTip(grid,string.Join("\r",_dicRowNotes[row]));
		}

		private void gridAction_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridSpecial_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridDocumentation_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridAvailableJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridAvailableJobsExpert_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridJobsOnHold_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridSearch_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridTesting_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridQueries_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridNotify_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridSubscribedJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridSubmittedJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridProjectManagement_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
		}

		private void gridAction_CellClick(object sender,ODGridClickEventArgs e) {
			object gridRowTag=((GridOD)sender).ListGridRows[e.Row].Tag;
			//Rows that have a tag of type JobAction are the 'category title' rows that are bold with a colored background.
			//When a user single clicks on one of these rows it should collapse or expand the entire category based on its current state.
			if(gridRowTag is JobAction jobAction) {
				if(_listJobActionsCollapsed.Contains(jobAction)) {
					_listJobActionsCollapsed.RemoveAll(x=>x==jobAction);
				}
				else {
					_listJobActionsCollapsed.Add(jobAction);
				}
				FillActiveTabGrid();
			}
			//Rows that have a tag of type Job should cause the entire JM window and registered entities notified that they need to load the selected job.
			else if(gridRowTag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridProjectManagement_CellClick(object sender,ODGridClickEventArgs e) {
			object gridRowTag=((GridOD)sender).ListGridRows[e.Row].Tag;
			//Rows that have a tag of type JobAction are the 'category title' rows that are bold with a colored background.
			//When a user single clicks on one of these rows it should collapse or expand the entire category based on its current state.
			if(gridRowTag is JobAction jobAction) {
				if(_listJobActionsCollapsedProject.Contains(jobAction)) {
					_listJobActionsCollapsedProject.RemoveAll(x=>x==jobAction);
				}
				else {
					_listJobActionsCollapsedProject.Add(jobAction);
				}
				FillActiveTabGrid();
			}
			//Rows that have a tag of type Job should cause the entire JM window and registered entities notified that they need to load the selected job.
			else if(gridRowTag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridSpecial_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridDocumentation_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridNotify_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridQueries_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridMarketing_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridPatternReview_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridAvailableJobs_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridAvailableJobsExpert_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridJobsOnHold_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridSubscribedJobs_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridTesting_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void gridUnresolvedIssues_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}
		private void gridSubmittedJobs_CellClick(object sender,ODGridClickEventArgs e) {
			if(((GridOD)sender).ListGridRows[e.Row].Tag is Job job) {
				LoadJob(job,false);
			}
		}

		private void toolTipHover_PopupHelper(object sender,PopupEventArgs e) {
			string message=_toolTipHover.GetToolTip(e.AssociatedControl);
			Size size=TextRenderer.MeasureText(message,label4.Font);
			size.Width+=2;//Padding
			size.Height+=2;//Padding
			e.ToolTipSize=size;
		}

		private void toolTipHover_Draw(object sender,DrawToolTipEventArgs e) {
			e.DrawBackground();
			e.DrawBorder();
			List<string> listNotes=e.ToolTipText.Split('\r').ToList();//This way we can change individual Notes text color.
			int y=0;
			foreach(string str in listNotes) {
				Brush brush=Brushes.Black;//Default 
				if(str=="$: Quote Pending") {
					brush=Brushes.Red;
				}
				e.Graphics.DrawString(str,label4.Font,brush,2,y);
				y+=TextRenderer.MeasureText(str,label4.Font).Height;
			}
		}

		///<summary>Attempts to identify flags for the given job.
		///Also constructs an explanation to be displayed when mouse hovers over jobs with flags.</summary>
		private string FlagHelper(Job jobCur,int gridIndex) {
			List<string> listNotes=new List<string>();
			if(jobCur.PhaseCur.In(JobPhase.Concept,JobPhase.Quote) && jobCur.ListJobQuotes.Count>0) {
				jobCur.TagOD=Color.Red;
				listNotes.Add("$: Quote Pending");
			}
			if(jobCur.PhaseCur.In(JobPhase.Development,JobPhase.Definition) && jobCur.ListJobQuotes.Count>0) {
				jobCur.TagOD=Color.Black;
				listNotes.Add("$: Quote Approved");
			}
			if(jobCur.ListJobReviews.Exists(x=>x.ReviewStatus==JobReviewStatus.Done)) {
				jobCur.TagOD=Color.Black;
				listNotes.Add("R: Reviewed");
			}
			else if(jobCur.ListJobLinks.Exists(x=>x.LinkType==JobLinkType.Appointment)) {
				jobCur.TagOD=Color.Red;
				listNotes.Add("A: Appt");
			}
			if(jobCur.ListJobReviews.Exists(x=>x.ReviewStatus==JobReviewStatus.SaveCommit)) {
				jobCur.TagOD=Color.Red;
				listNotes.Add("S: Save Commit");
			}
			else if(jobCur.ListJobReviews.Exists(x=>x.ReviewStatus==JobReviewStatus.SaveCommitted)) {
				jobCur.TagOD=Color.Black;
				listNotes.Add("S: Save Committed");
			}
			_dicRowNotes.Add(gridIndex,listNotes);//To be used on mouse hover.
			List<string> listFlagCodes=listNotes.Select(x=>x.Substring(0,x.IndexOf(":"))).ToList();
			return string.Join(",",listFlagCodes);
		}

		///<summary>Helper method to get job from the passed in grid and row selection. Can return null if not called correctly.</summary>
		private Job GetSelectedJob(GridOD grid,int rowSelection) {
			Job job;
			try {
				job=(Job)grid.ListGridRows[rowSelection].Tag;
			}
			catch(Exception ex) {
				job=null;
			}
			return job;
		}

		private void menuItemNone_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.None);
		}

		private void menuItemNotNeeded_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.NotNeeded);
		}

		private void menuItemAwaiting_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.AwaitingApproval);
		}

		private void menuItemApproved_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.Approved);
		}

		private void menuItemTentative_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.Tentative);
		}

		private void menuItemNotApproved_Click(object sender,EventArgs e) {
			UpdatePatternReviewStatusForSelected(JobPatternReviewStatus.NotApproved);
		}

		private void UpdatePatternReviewStatusForSelected(JobPatternReviewStatus status) {
			int idx=gridPatternReview.GetSelectedIndex();
			if(idx<0) {
				return;
			}
			Job job=(Job)gridPatternReview.ListGridRows[idx].Tag;
			if(job==null) {
				return;
			}
			Job jobOld=job.Copy();
			job.PatternReviewStatus=status;
			if(Jobs.Update(job,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			}
			LoadJob(job,true);
			JobLogs.MakeLogEntryForPatternReview(job,jobOld);
			FillGridPatternReview();
		}

		private void dateExcludeCompleteBefore_ValueChanged(object sender,EventArgs e) {
			FillGridPatternReview();
		}

		private void checkShowAllUsers_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkHideTested_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkHideNotTested_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkNotifyShowHqOnly_CheckedChanged(object sender,EventArgs e) {
			FillGridNotify();
		}

		private void checkSubscribedIncludeCancelled_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkSubscribedIncludeComplete_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkSubscribedIncludeOnHold_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void checkOnlyShowTopLevel_CheckedChanged(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void butQueriesRefresh_Click(object sender,EventArgs e) {
			//Completed and Cancelled jobs are not cached by default.
			//Go get any jobs from the database that match the filter criteria for the queries tab (check boxes and date range).
			if(checkShowQueryComplete.Checked || checkShowQueryCancelled.Checked) {
				JobManagerCore.AddQueryJobsToList();
			}
			FillActiveTabGrid();
		}

		private void butMarketingRefresh_Click(object sender,EventArgs e) {
			//Completed and Cancelled jobs are not cached by default.
			//Go get any jobs from the database that match the filter criteria for the queries tab (check boxes and date range).
			if(checkShowDesignComplete.Checked || checkShowDesignCancelled.Checked) {
				JobManagerCore.AddMarketingJobsToList();
			}
			FillActiveTabGrid();
		}

		private void butTestingRefresh_Click(object sender,EventArgs e) {
			FillActiveTabGrid();
		}

		private void butUnresolvedIssuesRefresh_Click(object sender,EventArgs e) {
			//Cancelled jobs are not cached by default.
			if(checkIncludeCancelledUnresolved.Checked) {
				JobManagerCore.AddUnresolvedIssueJobsToList();
			}
			FillActiveTabGrid();
		}

		private void textDocumentationVersion_TextChanged(object sender,EventArgs e) {
			timerDocumentationVersion.Stop();
			timerDocumentationVersion.Start();
		}

		private void timerDocumentationVersion_Tick(object sender,EventArgs e) {
			timerDocumentationVersion.Stop();
			FillActiveTabGrid();
		}

		private void treeJobs_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			OpenNonModalJob((Job)e.Node.Tag);
		}
		#endregion

		#region FillControls

		private void FillMenu() {
			menuMain.BeginUpdate();
			//Add Job-----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemAddJob=new MenuItemOD("Add Job",butAddJob_Click);
			menuMain.Add(menuItemAddJob);
			//Add Child Job-----------------------------------------------------------------------------------------------------
			MenuItemOD menuItemAddChild=new MenuItemOD("Add Child Job",butAddChildJob_Click);
			menuMain.Add(menuItemAddChild);
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true) && !JobPermissions.IsAuthorized(JobPerm.QueryTech,true) && !JobPermissions.IsAuthorized(JobPerm.DesignTech,true) && !JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true)) {
				menuItemAddJob.Enabled=false;
				menuItemAddChild.Enabled=false;
			}
			//Tools-------------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemTools=new MenuItemOD("Tools");
			menuMain.Add(menuItemTools);
			menuItemTools.Add("Backport",backportToolStripMenuItem_Click);
			menuItemTools.Add("Bug Submissions",butBugSubs_Click);
			menuItemTools.Add("Code Review Report",butCodeReview_Click);
			//TODO: Add this back in later
			//menuItemTools.Add("Job Time Helper",jobTimeHelperToolStripMenuItem_Click);
			menuItemTools.Add("Wiki",butWiki_Click);
			if(JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				menuItemTools.Add("Overview",butOverview_Click);
			}
			if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
				menuItemTools.Add("Teams",butTeams_Click);
			}
			if(JobPermissions.IsAuthorized(JobPerm.Engineer,true)) {
				//This menu will be kept current by FormOpenDental on the typical HQ phone status update timer.
				_menuItemEmployeeStatus=new MenuItemOD("Employee: Status",employeeStatusMenuItem_Click);
				menuMain.Add(_menuItemEmployeeStatus);
				_menuItemEmployeeStatus.Enabled=false;
			}
			menuMain.EndUpdate();
		}

		private void employeeStatusMenuItem_Click(object sender,EventArgs e) {
			if(_phone==null){
				return;
			}
			if(_phone.ClockStatus==ClockStatusEnum.Unavailable){
				PhoneUI.Available(_phone);
			}
			else if(_phone.ClockStatus==ClockStatusEnum.Available){
				PhoneUI.Unavailable(_phone);
			}
		}

		public void SetEmployeeStatus(Phone phone) {
			if(_menuItemEmployeeStatus==null){
				return;
			}
			_phone=phone;
			_menuItemEmployeeStatus.Enabled=false;
			if(_phone==null){
				_menuItemEmployeeStatus.Visible=false;
				return;
			}
			_menuItemEmployeeStatus.Text=$"{_phone.EmployeeName}: {_phone.ClockStatus.GetDescription()}";
			//Only allow simple clock status switching if currently Available or Unavailable. Everything else should be done from FromOpenDental.
			_menuItemEmployeeStatus.Enabled= _phone.ClockStatus.In(ClockStatusEnum.Available,ClockStatusEnum.Unavailable);
			_menuItemEmployeeStatus.Visible=true;
			MenuStripOD menuStripOD=MenuStripOD.GetMenuStripOD(_menuItemEmployeeStatus);
			menuStripOD?.LayoutItems();
		}

		private void FillPriorityList() {
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<string> listPriorityItemValues=_listJobPriorities.SelectMany(y=>y.ItemValue.Split(',')).ToList();
			if(_listJobPriorities.Count<1
				|| !listPriorityItemValues.Contains("OnHold")
				|| !listPriorityItemValues.Contains("Low")
				|| !listPriorityItemValues.Contains("Normal")
				|| !listPriorityItemValues.Contains("MediumHigh")
				|| !listPriorityItemValues.Contains("High")
				|| !listPriorityItemValues.Contains("Urgent")
				|| !listPriorityItemValues.Contains("JobDefault")
				|| !listPriorityItemValues.Contains("BugDefault")
				|| !listPriorityItemValues.Contains("DocumentationDefault"))
			{
				MsgBox.Show(this,"Job priority definition is not currently set up in a way that the JobManager can function");
				Close();
				return;
			}
		}

		private void FillComboUser() {
			Userod userCur=(Userod)comboUser.Tag;
			comboUser.SelectedIndex=-1;
			comboUser.Items.Clear();
			comboUser.Items.Add("All");//All is first.
			comboUser.Items.Add("Unassigned");
			if(userCur.UserNum==0) {//All
				comboUser.SelectedIndex=0;
			}
			else if(userCur.UserNum==-1) {//Unassigned
				comboUser.SelectedIndex=1;
			}
			for(int i=0;i<_listUsers.Count;i++) {
				comboUser.Items.Add(_listUsers[i].UserName);
				if(userCur.UserNum==_listUsers[i].UserNum) {
					comboUser.SelectedIndex=i+2;
				}
			}
			if(comboUser.SelectedIndex==-1) {
				comboUser.SelectedIndex=1;
			}
			this.Text="Job Manager"+(comboUser.Text=="" ? "" : " - "+comboUser.Text);
		}

		private void FillComboTeamFilter(UI.ComboBox comboTeam,bool doAddAllOption=false) {
			comboTeam.Items.Clear();
			if(doAddAllOption) {
				comboTeam.Items.Add("All",new JobTeam(){JobTeamNum=-2});
			}
			comboTeam.Items.Add("None",new JobTeam(){JobTeamNum=-1});
			comboTeam.Items.AddList(_listJobTeams,x=>x.TeamName);
			comboTeam.SelectedIndex=0;
		}
		#endregion

		#region MainMenu Events
		private void butAddJob_Click(object sender,EventArgs e) {
			AddNewJob();
		}

		private void butAddChildJob_Click(object sender,EventArgs e) {
			Job job=userControlJobManagerEditor.JobCur;
			if(job==null) {
				MsgBox.Show(this,"No job currently selected. Select a job to be a parent job.");
				return;
			}
			AddNewJob(job.JobNum,job.TopParentNum);
		}

		private void AddNewJob(long parentNum=0,long topParentNum=0) {
			Job jobNew=new Job();
			jobNew.ParentNum=parentNum;
			jobNew.TopParentNum=topParentNum;
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listJobPriorities.Count==0) {
				MsgBox.Show(this,"You have no priorities setup in definitions. Go to definitions and set up Job Priorities first.");
				return;
			}
			//Get category list for the user
			List<string> categoryList=JobManagerCore.CategoryList;
			if(parentNum>0 && topParentNum>0 && userControlJobManagerEditor.JobCur.Category!=JobCategory.Project) {
				categoryList.Remove("Project");//Projects can only have other Projects as Parents
			}
			if(categoryList.Count==0) {//Should only happen if we forget to stop them from being able to click the button
				MsgBox.Show(this,"You are not authorized to create jobs.");
				return;
			}
			InputBox inputBoxCategory=new InputBox("Choose a job category",categoryList,0);
			inputBoxCategory.ShowDialog();
			if(inputBoxCategory.IsDialogCancel) {
				return;
			}
			if(inputBoxCategory.SelectedIndex==-1) {//Shouldn't ever happen, but I am leaving this here
				MsgBox.Show(this,"You must choose a category to create a job.");
				return;
			}
			jobNew.Category=(JobCategory)Enum.GetNames(typeof(JobCategory)).ToList().IndexOf(categoryList[inputBoxCategory.SelectedIndex]);
			long priorityNum=0;
			priorityNum=listJobPriorities.Find(x=>x.ItemValue.Contains("JobDefault")).DefNum;
			if(jobNew.Category.In(JobCategory.Bug,JobCategory.Conversion,JobCategory.NeedNoApproval,JobCategory.UnresolvedIssue)) {
				priorityNum=listJobPriorities.Find(x=>x.ItemValue.Contains("BugDefault")).DefNum;
			}
			jobNew.Priority=priorityNum==0 ? listJobPriorities[0].DefNum : priorityNum;
			jobNew.PhaseCur=JobPhase.Concept;
			jobNew.UserNumConcept=Security.CurUser.UserNum;
			if(jobNew.Category!=JobCategory.Query && jobNew.Category!=JobCategory.MarketingDesign) {
				using FormJobAdd FormJA=new FormJobAdd(jobNew);
				if(FormJA.ShowDialog()==DialogResult.OK) {
					GoToJob(jobNew.JobNum);
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
				}
				return;//We don't want to continue for normal jobs
			}
			//------------EVERYTHING BELOW IS ONLY FOR QUERY AND MARKETING JOBS----------------------
			InputBox titleBox=new InputBox("Provide a brief title for the job.");
			titleBox.ShowDialog();
			if(titleBox.IsDialogCancel) {
				return;
			}
			if(String.IsNullOrEmpty(titleBox.StringResult)) {
				MsgBox.Show(this,"You must type a title to create a job.");
				return;
			}
			jobNew.Title=titleBox.StringResult;
			JobLink jobLinkNew=new JobLink();
			JobQuote jobQuoteNew=new JobQuote();
			Bug bugNew=new Bug();
			if(jobNew.Category==JobCategory.Bug) {
				jobLinkNew.LinkType=JobLinkType.Bug;
				bugNew=Bugs.GetNewBugForUser();
				InputBox bugDescription=new InputBox("Provide a brief description for the bug. This will appear in the bug tracker.",jobNew.Title);
				bugDescription.ShowDialog();
				if(bugDescription.IsDialogCancel) {
					return;
				}
				if(String.IsNullOrEmpty(bugDescription.StringResult)) {
					MsgBox.Show(this,"You must type a description to create a bug.");
					return;
				}
				using FormVersionPrompt FormVP=new FormVersionPrompt("Enter versions found");
				FormVP.ShowDialog();
				if(FormVP.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(FormVP.VersionText)) {
					MsgBox.Show(this,"You must type a description to create a bug.");
					return;
				}
				bugNew.Status_=BugStatus.Accepted;
				bugNew.VersionsFound=FormVP.VersionText;
				bugNew.Description=bugDescription.StringResult;
			}
			else if(jobNew.Category==JobCategory.Query || jobNew.Category==JobCategory.MarketingDesign) {
				FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
				if(jobQuoteNew.PatNum!=0) {
					frmPatientSelect.ListPatNumsExplicit=new List<long> { jobQuoteNew.PatNum };
				}
				frmPatientSelect.ShowDialog();
				if(frmPatientSelect.IsDialogCancel) {
					return;
				}
				Patient pat=Patients.GetPat(frmPatientSelect.PatNumSelected);
				if(!pat.BillingType.In(41,165,183,200,219,224,226,288,371,379,423,430,436,485,486) &&
					!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This customer is currently not on support.\r\nIf you choose to continue, be sure to discuss support options with this customer."))
				{
					return;
				}
				if(pat!=null) {
					jobQuoteNew.PatNum=pat.PatNum;
				}
				else {
					jobQuoteNew.PatNum=0;
				}
			}
			Jobs.Insert(jobNew);
			jobLinkNew.JobNum=jobNew.JobNum;
			if(jobNew.Category==JobCategory.Bug) {
				jobLinkNew.FKey=Bugs.Insert(bugNew);
				JobLinks.Insert(jobLinkNew);
			}
			if(jobNew.Category==JobCategory.Query || jobNew.Category==JobCategory.MarketingDesign) {
				jobQuoteNew.JobNum=jobNew.JobNum;
				JobQuotes.Insert(jobQuoteNew);
			}
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
			GoToJob(jobNew.JobNum);
		}

		private void butBugSubs_Click(object sender,EventArgs e) {
			FormBugSubmissions FormBugSubs=new FormBugSubmissions();
			FormBugSubs.Show();//Non-modal
		}

		private void butCodeReview_Click(object sender,EventArgs e) {
			FormCodeReviewReport formCodeReviewReport=new FormCodeReviewReport();
			formCodeReviewReport.Show();//Non-modal
		}

		private void butOverview_Click(object sender,EventArgs e) {
			FormJobManagerOverview FormOverview=new FormJobManagerOverview();
			FormOverview.Show();//Non-modal
		}

		private void butTeams_Click(object sender,EventArgs e) {
			FormJobTeams formJobTeams=new FormJobTeams();
			formJobTeams.Show();//Non-modal;
		}

		private void butWiki_Click(object sender,EventArgs e) {
			new FormWiki().Show();//allow multiple
		}

		private void jobTimeHelperToolStripMenuItem_Click(object sender,EventArgs e) {
			if(Application.OpenForms.OfType<FormJobTime>().Any()) {
				Application.OpenForms.OfType<FormJobTime>().ToList()[0].BringToFront();
				return;
			}
			FormJobTime FormJT=new FormJobTime();
			FormJT.Show(this);
		}

		private void backportToolStripMenuItem_Click(object sender,EventArgs e) {
			FormBackport formB=new FormBackport(userControlJobManagerEditor.JobNumCur);
			formB.Show();
		}
		#endregion

		#region Search Methods
		private void textSearchAction_TextChanged(object sender,EventArgs e) {
			timerSearch.Stop();
			timerSearch.Start();
		}

		private void textSearch_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				JobSearch();
			}
		}

		private void timerSearch_Tick(object sender,EventArgs e) {
			RefreshGridsForSearch();
		}

		private void butSearch_Click(object sender,EventArgs e) {
			JobSearch();
		}

		private void JobSearch() {
			string searchTrimmed=textSearch.Text.Trim();
			Job jobFound=null;
			if(string.IsNullOrEmpty(searchTrimmed) && ClipboardDataIsValid()) {
				jobFound=JobSearchFromClipboard();
			}
			else if(!string.IsNullOrEmpty(searchTrimmed) && textSearch.Text.All(x=>char.IsNumber(x))) {
				jobFound=Jobs.GetOneFilled(PIn.Long(searchTrimmed));
			}
			else {
				jobFound=Jobs.GetOneFilled(searchTrimmed);
			}
			if(jobFound!=null) {
				//Load the new job into the job mananger controls and cache.
				LoadJob(jobFound,false);
				RefreshGridsForSearch();
				gridSearch.ScrollToIndex(gridSearch.GetSelectedIndex());
			}
			tabControlNav.SelectedTab=tabSearch;
		}

		private bool ClipboardDataIsValid() {
			if (!System.Windows.Clipboard.ContainsText()) {
				return false;
			}
			try {
				string clipboardText=System.Windows.Clipboard.GetText();
			}
			catch {
				return false;
			}
			return true;
		}

		private Job JobSearchFromClipboard() {
			string clipboardText=System.Windows.Clipboard.GetText().ToLower().Trim();
			if(Regex.IsMatch(clipboardText,@"^jobnum:\d+$")) {
				return Jobs.GetOneFilled(PIn.Long(clipboardText.Substring(7)));
			}
			return null;
		}

		private void RefreshGridsForSearch() {
			timerSearch.Stop();
			FillActiveTabGrid();
		}

		private void butAdvSearch_Click(object sender,EventArgs e) {
			using FormJobSearch formJobSearch=new FormJobSearch();
			formJobSearch.InitialSearchString=textSearch.Text;
			//pass in data here to reduce calls to DB.
			formJobSearch.ShowDialog(this);
			if(formJobSearch.DialogResult!=DialogResult.OK) {
				return;
			}
			checkContactSearch.Checked=true;
			checkResults.Checked=true;//sets control visibility as well.
			tabControlNav.SelectedTab=tabSearch;//Search tab to see search results.
			LoadJob(formJobSearch.SelectedJob,true);
			//Search list has already been updated
			FillGridSearch();
		}

		private void checkResults_CheckedChanged(object sender,EventArgs e) {
			//TODO Change this to be more legible
			//visible==Checked
			checkResults.Visible=checkResults.Checked;
			//visible==!Checked
			checkContactSearch.Visible=!checkResults.Checked;
			comboCatSearch.Visible=!checkResults.Checked;
			comboPrioritySearch.Visible=!checkResults.Checked;
			comboProposedVersionSearch.Visible=!checkResults.Checked;
			comboTeamSearch.Visible=!checkResults.Checked;
			textUserSearch.Visible=!checkResults.Checked;
			labelCatSearch.Visible=!checkResults.Checked;
			labelPrioritySearch.Visible=!checkResults.Checked;
			labelProposedVersionSearch.Visible=!checkResults.Checked;
			labeluserSearch.Visible=!checkResults.Checked;
			if(!checkResults.Visible) {
				FillGridSearch();
			}
		}
		#endregion

		#region JobEdit Methods
		private void userControlJobManagerEditor_GoToJob(object sender,long jobNum) {
			LoadJob(Jobs.GetOneFilled(jobNum),true);
		}

		///<summary>Loads whatever job control is necessary in order to display the job within the manager.
		///Also refreshes the cache with the job passed in and updates all corresponding controls and grids.
		///This method will not load the passed in job if the current job cannot be saved correctly.</summary>
		private void LoadJob(Job job,bool doRefreshUI,LoadJobAction loadAction=LoadJobAction.Select) {
			if(job==null || userControlJobManagerEditor.UnsavedChangesCheck()) {
				return;
			}
			#region Refresh UI Elements
			if(doRefreshUI) {
				FillActiveTabGrid();
				UpdateTabVisibility();
			}
			#endregion
			UpdateStack(job,loadAction);
			butRefresh.Enabled=true;
			//Forcefully load the new job into whatever control it needs to be loaded into in order to be interacted with.
			userControlJobManagerEditor.ShowEditorForJob(job);
		}

		///<summary>Opens a non-modal job editor. This method is here so FormJobManager can still maintain ownership of the form.</summary>
		public static void OpenNonModalJob(Job job) {
			if(job==null) {//Double clicking on a title row, or something went wrong.
				return;
			}
			FormJobEdit FormJE=_listJobEditForms.Find(x=> x.JobCur.JobNum==job.JobNum);
			if(FormJE==null) {
				FormJE=new FormJobEdit(job);
				FormJE.Show();
				_listJobEditForms.Add(FormJE);
				return;
			}
			FormJE.Activate();
		}

		public static void RemoveFormJobEdit(FormJobEdit form) {
			_listJobEditForms.Remove(form);
		}

		private bool CloseJobEditForms() {
			for(int i=Application.OpenForms.Count-1;i>=0;i--) {
				Form formToClose=Application.OpenForms[i];
				if(formToClose.GetType()==typeof(FormJobEdit)) {
					//If the window which showed the messagebox popup causes the form to stay open, then stop the log off event, because the user chose to.
					formToClose.InvokeIfRequired(()=>formToClose.Close());//Attempt to close the form, even if created in another thread.
					//Run Application.DoEvents() to allow the FormClosing/FormClosed events to fire in the form before checking if they have closed below.
					Application.DoEvents();//Required due to invoking.  Otherwise FormClosing/FormClosed will not fire until after we exit.
					if(!IsDisposedOrClosed(formToClose)) {
						return false;//This form needs to stay open and stop all other forms from being closed.
					}
					RemoveFormJobEdit((FormJobEdit)formToClose);
				}
			}
			return true;
		}
		#endregion

		private void timerRefreshUI_Tick(object sender,EventArgs e) {
			if(JobManagerCore.LastRefreshDateTime>LocalLastRefreshDateTime) {
				FillActiveTabGrid();
				userControlJobManagerEditor.LoadMergeJob(JobManagerCore.ListJobsAll.Find(x=>x.JobNum==userControlJobManagerEditor.JobNumCur));
				LocalLastRefreshDateTime=DateTime.Now;
			}
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(!listSignals.Exists(x=>x.IType==InvalidType.Jobs || x.IType==InvalidType.Security || x.IType==InvalidType.Defs)) {
				return;//no job signals;
			}
			if(listSignals.Any(x=>x.IType==InvalidType.Security)) {
				FillComboUser();
			}
			if(listSignals.Any(x=>x.IType==InvalidType.Defs)) {
				FillPriorityList();
			}
			//Get the job nums from the signals passed in.
			List<long> listJobNums=listSignals.FindAll(x=>x.IType==InvalidType.Jobs && x.FKeyType==KeyType.Job)
				.Select(x=>x.FKey)
				.Distinct()
				.ToList();
			if(listJobNums.Count>0) {
				JobManagerCore.RefreshAndFillThreaded(listJobNums);
			}
		}

		private void FormJobManager_FormClosing(object sender,FormClosingEventArgs e) {
			if(userControlJobManagerEditor.UnsavedChangesCheck()) {
				e.Cancel=true;
				return;
			}
			if(!CloseJobEditForms()) {
				e.Cancel=true;
				return;
			}
			JobManagerCore.ClearJobCache();
			//Close();//FormJobManager wasn't closing properly if there were any FormJobEdits open. May have to do with calling Application.DoEvents()
			ODThread.QuitSyncThreadsByGroupName(100,"RefreshAndFillJobManager");//Give the thread 100ms before killing it.
		}

		private void butExport_Click(object sender,EventArgs e) {
			gridAction.Export($"JobManagerActionItems_{DateTime.Today:yyyy-MM-dd}");
		}

		private void comboQueryPhaseFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			_listQueryJobPhaseFilter.Clear();
			_listQueryJobPhaseFilter=comboQueryPhaseFilter.GetListSelected<JobPhase>();
			if(_listQueryJobPhaseFilter.Count==0 || comboQueryPhaseFilter.IsAllSelected) {
				_listQueryJobPhaseFilter=comboQueryPhaseFilter.Items.GetAll<JobPhase>();
				comboQueryPhaseFilter.SetAll(false);
				comboQueryPhaseFilter.IsAllSelected=true;
			}
			FillGridQueries();
		}

		private void comboQueryPriorityFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			_listQueryPriorityFilter.Clear();
			_listQueryPriorityFilter=comboQueryPriorityFilter.GetListSelected<Def>().ConvertAll(x=>x.DefNum);
			if(_listQueryPriorityFilter.Count==0 || comboQueryPriorityFilter.IsAllSelected) {
				_listQueryPriorityFilter=comboQueryPriorityFilter.Items.GetAll<Def>().ConvertAll(x=>x.DefNum);
				comboQueryPriorityFilter.SetAll(false);
				comboQueryPriorityFilter.IsAllSelected=true;
			}
			FillGridQueries();
		}
	}
}