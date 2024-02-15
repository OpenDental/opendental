using CodeBase;
using OpenDental.UI;
using OpenDentalGraph.Extensions;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using MenuItem = System.Windows.Forms.MenuItem;

namespace OpenDental.InternalTools.Job_Manager {

	public partial class UserControlProjectEdit:UserControl {
		#region Fields
		private Job _jobOld;
		///<summary>Current Project selected in FormJobManager.gridProjectManagement.</summary>
		private Job _jobCur;
		///<summary>Private member for IsChanged Property. Private setter, public getter.</summary>
		private bool _isChanged=false;
		private bool _isLoading=false;
		private bool _isOverride;
		private List<Def> _listJobPriorities;
		private List<JobTeam> _listJobTeams;
		private List<Def> _listPriorities;
		private List<Def> _listPrioritiesAll;
		private List<Job> _listJobsByTopParent;
		private const string _CHILD_JOB_MENU="Create Child Job";
		public bool IsNew;
		///<summary>Set this to true when the job is open outside of FormJobManager. Disables some functionality.</summary>
		public bool IsPopout;
		#endregion Fields

		#region Delegates
		public delegate void RequestJobEvent(object sender,long jobNum);
		public event RequestJobEvent RequestJob=null;
		#endregion Delegates

		#region Properties
		public bool IsChanged {
			get { return _isChanged; }
			private set {
				if(_isLoading) {
					_isChanged=false;
					return;
				}
				_isChanged=value;
			}
		}

		public bool IsOverride {
			get {return _isOverride;}
			set {
				_isOverride=value;
				CheckPermissions();
			}
		}
		#endregion Properties

		public UserControlProjectEdit() {
			InitializeComponent();
			comboPhase.Items.AddListEnum(new List<JobPhase>() { JobPhase.Concept,JobPhase.Development,JobPhase.Complete,JobPhase.Cancelled });
			FillPriorityList();
			_listJobTeams=JobTeams.GetDeepCopy();
			comboJobTeam.Items.Add("None",new JobTeam(){JobTeamNum=-1});
			comboJobTeam.Items.AddList(_listJobTeams,x => x.TeamName);
			comboJobTeam.SelectedIndex=0;
			CreateGridContextMenus();
		}

		private bool TryGetSelectedJobFromMenuItem(MenuItem menuItem,out Job job,out string error) {
			job=null;
			error="";
			if(menuItem==null) {
				error="Provided MenuItem was null.";
				return false;
			}
			if(menuItem.Tag.GetType()!=typeof(GridOD)){
				error=$"Expected MenuItem.Tag to be type {nameof(GridOD)} but received {nameof(menuItem.Tag)}.";
				return false;
			}
			if(!TryGetSelectedJobFromGrid((GridOD)menuItem.Tag,out job)) {
				error="Could not find the selected job.";
				return false;
			}
			return true;
		}

		///<summary>Should only be called once when new job should be loaded into control. If called again, changes will be lost.</summary>
		public void LoadJob(Job job) {
			Job jobPrev=null;
			if(_jobCur!=null) {
				jobPrev=_jobCur.Copy();
			}
			_isLoading=true;
			this.Enabled=false;//disable control while it is filled.
			_isOverride=false;
			IsChanged=false;
			if(job==null) {
				_jobCur=new Job();
			}
			else {
				_jobCur=job.Copy();
				IsNew=job.IsNew;
			}
			_jobOld=_jobCur.Copy();//cannot be null
			textTitle.Text=_jobCur.Title;
			textJobNum.Text=_jobCur.JobNum>0?_jobCur.JobNum.ToString():Lan.g("Jobs","New Project");
			textProjectManager.Text=_jobCur.UserNumExpert==0 ? "Unassigned" : Userods.GetName(_jobCur.UserNumExpert);
			textSubmitter.Text=Userods.GetName(_jobCur.UserNumConcept);
			textCheckedOut.Text=Userods.GetName(_jobCur.UserNumCheckout);
			if(_jobCur.UserNumCheckout==0) {
				textCheckedOut.BackColor=Color.FromArgb(255, 240, 240, 240);//control
			}
			else if(_jobCur.UserNumCheckout!=Security.CurUser.UserNum) {
				textCheckedOut.BackColor=Color.FromArgb(254,235,233);//light red
			}
			else {
				textCheckedOut.BackColor=Color.FromArgb(235,254,233);//light green
			}
			textDateEntry.Text=_jobCur.DateTimeEntry.Year>1880?_jobCur.DateTimeEntry.ToShortDateString():"";
			textVersion.Text=_jobCur.JobVersion;
			textHoursLeftDescendants.Text="";
			textHoursEstDescendants.Text="";
			textHoursActualDescendants.Text="";
			comboPhase.SetSelectedEnum(_jobCur.PhaseCur);
			textEstHours.Text=_jobCur.HoursEstimate.ToString();
			if(comboPriority.Items.Count==0) {
				_listPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList();
				_listPrioritiesAll=Defs.GetDefsForCategory(DefCat.JobPriorities).OrderBy(x => x.ItemOrder).ToList();
				comboPriority.Items.AddDefs(_listPriorities);
			}
			comboPriority.SetSelectedDefNum(_jobCur.Priority);
			string gitBranchName=Jobs.GetGitBranchName(_jobCur);
			if(gitBranchName.Length>50){
				gitBranchName=gitBranchName.Substring(0,50);
			}
			textGitBranchName.Text=gitBranchName;
			try {
				textEditorProjectDescription.MainRtf=_jobCur.Requirements;//This is here to convert our old job descriptions to the new RTF descriptions.
			}
			catch {
				textEditorProjectDescription.MainText=_jobCur.Requirements;
			}
			JobLink jobLink=_jobCur.ListJobLinks.Find(x => x.LinkType==JobLinkType.JobTeam);
			if(jobLink==null) {
				comboJobTeam.SelectedIndex=0;
			}
			else {
				comboJobTeam.SetSelectedKey<JobTeam>(jobLink.FKey,x => x.JobTeamNum);
			}
			checkIsActive.Checked=_jobCur.ListJobActiveLinks.Exists(x => x.UserNum==Security.CurUser.UserNum && x.DateTimeEnd==DateTime.MinValue);
			if(JobPermissions.IsAuthorized(JobPerm.ProjectManager,true) && !_jobCur.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation)) {
				checkIsActive.Enabled=true;
			}
			else {
				checkIsActive.Enabled=false;
			}
			FillGridProjects();
			FillAllChildGrids();
			CheckPermissions();
			CreateViewLog(jobPrev);
			this.Enabled=true;//disable control while it is filled.
			_isLoading=false;
		}

		#region Fill Grids
		private void FillGridProjects() {
			if(_jobCur==null) {
				return;
			}
			List<Job> listJobs=JobManagerCore.ListJobsAll.FindAll(x => x.TopParentNum==_jobCur.TopParentNum && x.Category==JobCategory.Project 
				&& (x.PhaseCur.In(JobPhase.Concept,JobPhase.Development) 
					|| (x.PhaseCur==JobPhase.Complete && checkIncludeComplete.Checked) 
					|| (x.PhaseCur==JobPhase.Cancelled && checkIncludeCancelled.Checked)))
				.OrderBy(x => x.JobNum!=_jobCur.TopParentNum)
				.ThenBy(x => x.PhaseCur==JobPhase.Cancelled)
				.ThenBy(x => x.PhaseCur==JobPhase.Complete)
				.ThenBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder)
				.ThenBy(x => x.JobNum).ToList();
			int jobCount=0;
			gridProjects.Enabled=true;
			gridProjects.BeginUpdate();
			gridProjects.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Priority"),50,HorizontalAlignment.Center);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Phase"),75);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"JobNum"),50);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Parent"),50);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Title"),600){ IsWidthDynamic=true };
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Team"),150);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Project Manager"),100);
			gridProjects.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Hrs. Est"),50);
			gridProjects.Columns.Add(col);
			gridProjects.ListGridRows.Clear();
			string teamName;
			GridRow row;
			GridCell gridCell;
			foreach(Job job in listJobs) {
				JobActiveLink activeLink=job.ListJobActiveLinks.FirstOrDefault(x => x.UserNum==Security.CurUser.UserNum && x.DateTimeEnd==DateTime.MinValue);
				Def jobPriority=_listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
				string owner=job.UserNumExpert==0 ? "Unassigned" : Userods.GetName(job.UserNumExpert);
				JobLink jobLinkTeam=job.ListJobLinks.Find(x => x.LinkType==JobLinkType.JobTeam);
				teamName="";
				if(jobLinkTeam!=null) {
						teamName=_listJobTeams.Find(x => x.JobTeamNum==jobLinkTeam.FKey)?.TeamName??"";
				}
				string parent="P"+job.ParentNum;
				if(job.JobNum==job.TopParentNum) {
					parent="Self";
				}
				row=new GridRow();
				gridCell=new GridCell(jobPriority.ItemName);
				gridCell.ColorBackG=jobPriority.ItemColor;
				gridCell.ColorText=(job.Priority==_listJobPriorities.FirstOrDefault(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black;
				row.Cells.Add(gridCell);
				row.Cells.Add(job.PhaseCur.ToString());
				gridCell=new GridCell(job.Category.ToString().Substring(0,1)+job.JobNum);
				gridCell.ColorText=job.TagOD!=null ? (Color)job.TagOD : Color.Black;
				row.Cells.Add(gridCell);
				row.Cells.Add(parent);
				row.Cells.Add(job.Title);
				row.Cells.Add(teamName);
				row.Cells.Add(owner);
				row.Cells.Add(job.HoursEstimate.ToString());
				row.Tag=job;
				gridProjects.ListGridRows.Add(row);
			}
			jobCount+=listJobs.Count;
			gridProjects.Title="Projects ("+jobCount+")";
			gridProjects.EndUpdate();
			//RESELECT JOB
			int index=gridProjects.ListGridRows.FindIndex(x => x.Tag is Job && ((Job)x.Tag).JobNum==_jobCur.JobNum);
			gridProjects.SetSelected(index < 0 ? -1 : index,true);
		}

		private void FillAllChildGrids() {
			FillGridDiscussion();
			FillJobGrid(JobPhase.Concept,gridConcept);
			FillJobGrid(JobPhase.Definition,gridDefinition);
			FillJobGrid(JobPhase.Development,gridDevelopment);
			FillJobGrid(JobPhase.Development,gridPendingReview);
			FillJobGrid(JobPhase.Documentation,gridDocumentation);
			FillJobGrid(JobPhase.Complete,gridComplete);
		}

		///<summary>Helper method that fills the grid passed in with the corresponding job notes.
		///This is here because right now every grid that displays JobNotes shows them the same way.</summary>
		private void FillGridDiscussion() {
			gridDiscussion.BeginUpdate();
			gridDiscussion.Columns.Clear();
			gridDiscussion.Columns.Add(new GridColumn(Lan.g(this,"Date Time"),120));
			gridDiscussion.Columns.Add(new GridColumn(Lan.g(this,"User"),80));
			gridDiscussion.Columns.Add(new GridColumn(Lan.g(this,"Note"),400));
			gridDiscussion.ListGridRows.Clear();
			GridRow row;
			List<JobNote> listJobNotes=_jobCur.ListJobNotes.FindAll(x => x.NoteType==JobNoteTypes.Discussion).OrderBy(x => x.DateTimeNote).ToList();
			foreach(JobNote jobNote in listJobNotes) {
				row=new GridRow();
				row.Cells.Add(jobNote.DateTimeNote.ToShortDateString()+" "+jobNote.DateTimeNote.ToShortTimeString());
				row.Cells.Add(Userods.GetName(jobNote.UserNum));
				row.Cells.Add(jobNote.Note);
				row.Tag=jobNote;
				gridDiscussion.ListGridRows.Add(row);
			}
			gridDiscussion.EndUpdate();
			gridDiscussion.ScrollValue=gridDiscussion.ScrollValue;//this forces scroll value to reset if it's > allowed max.
		}

		private void FillJobGrid(JobPhase jobPhase,GridOD grid) {
			if(_jobCur==null) {
				return;
			}
			List<Job> listJobs=new List<Job>();
			if(checkShowAllChildJobs.Checked) {
				listJobs=GetDescedentJobs(_jobCur.JobNum,true);
				RemoveCompletedJobsFromListJobs(listJobs);
				RemoveCompletedJobsFromListJobs(_listJobsByTopParent);
			}
			else {
				listJobs=JobManagerCore.ListJobsAll.FindAll(x => x.ParentNum==_jobCur.JobNum);
			}
			if(jobPhase!=JobPhase.Complete) {
				listJobs=listJobs.FindAll(x => x.PhaseCur==jobPhase)
					.OrderBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder)
					.ThenBy(x => x.JobNum).ToList();
			}
			else {
				listJobs=listJobs.FindAll(x => (x.PhaseCur==JobPhase.Complete && checkIncludeComplete.Checked) || (x.PhaseCur==JobPhase.Cancelled && checkIncludeCancelled.Checked))
					.OrderBy(x => x.PhaseCur==JobPhase.Cancelled)
					.ThenBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder)
					.ThenBy(x => x.JobNum).ToList();
			}
			if(jobPhase==JobPhase.Development && grid==gridPendingReview) {
				listJobs=listJobs.FindAll(x => x.ListJobReviews.Exists(y => y.ReviewStatus.In(JobReviewStatus.Sent,JobReviewStatus.Seen,JobReviewStatus.UnderReview)));
			}
			else if(jobPhase==JobPhase.Development && grid==gridDevelopment) {
				listJobs=listJobs.FindAll(x => !x.ListJobReviews.Exists(y => y.ReviewStatus.In(JobReviewStatus.Sent,JobReviewStatus.Seen,JobReviewStatus.UnderReview)));
			}
			grid.Enabled=true;
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Priority"),50,HorizontalAlignment.Center);
			grid.Columns.Add(col);
			if(jobPhase==JobPhase.Complete) {
				col=new GridColumn(Lan.g(this,"Phase"),60);
				grid.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"JobNum"),50);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Title"),250);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Team"),100);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Owner"),75);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Expert"),75);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Hours"),75);
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			GridCell gridCell;
			Def jobPriority;
			string text;
			for(int i=0;i<listJobs.Count;i++) {
				jobPriority=_listJobPriorities.FirstOrDefault(x => x.DefNum==listJobs[i].Priority);
				row=new GridRow();
				gridCell=new GridCell(jobPriority.ItemName);
				gridCell.ColorBackG=jobPriority.ItemColor;
				gridCell.ColorText=(listJobs[i].Priority==_listJobPriorities.FirstOrDefault(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black;
				row.Cells.Add(gridCell);
				if(jobPhase==JobPhase.Complete) {
					row.Cells.Add(listJobs[i].PhaseCur.ToString());
				}
				row.Cells.Add(listJobs[i].Category.ToString().Substring(0,1)+listJobs[i].JobNum);
				gridCell=new GridCell(listJobs[i].Title);
				if((jobPhase==JobPhase.Definition && listJobs[i].UserNumExpert==0) || (jobPhase==JobPhase.Development && listJobs[i].UserNumEngineer==0)) {
					gridCell.ColorBackG=Color.Gold;
				}
				row.Cells.Add(gridCell);
				JobLink jobLink=listJobs[i].ListJobLinks.Find(x => x.LinkType==JobLinkType.JobTeam);
				JobTeam jobTeam=null;
				if(jobLink!=null) {
					jobTeam=_listJobTeams.FirstOrDefault(x => jobLink!=null && x.JobTeamNum==jobLink.FKey);
				}
				text=(jobTeam==null) ? "" : jobTeam.TeamName;
				row.Cells.Add(text);
				text=listJobs[i].OwnerNum==0 ? "" : Userods.GetName(listJobs[i].OwnerNum);
				if(listJobs[i].Category==JobCategory.Project) {
					text=listJobs[i].UserNumExpert==0 ? Userods.GetName(listJobs[i].UserNumConcept) : Userods.GetName(listJobs[i].UserNumExpert);
				}
				row.Cells.Add(text);
				text=listJobs[i].UserNumExpert==0 ? "Unassigned" : Userods.GetName(listJobs[i].UserNumExpert);
				row.Cells.Add(text);
				text=listJobs[i].HoursActual + " / " + listJobs[i].HoursEstimate;
				row.Cells.Add(text);
				row.Tag=listJobs[i];
				grid.ListGridRows.Add(row);
			}
			grid.Title=jobPhase.ToString()+" ("+listJobs.Count+")";
			if(grid==gridPendingReview) {
				grid.Title="Pending Review" +" ("+listJobs.Count+")";
			}
			grid.EndUpdate();
		}
		#endregion Fill Grids

		///<summary>Not a property so that this is compatible with the VS designer.</summary>
		public Job GetJob() {
			if(_jobCur==null) {
				return null;
			}
			Job job=_jobCur.Copy();
			return job;
		}

		///<summary>Based on job status, category, and user role, this will enable or disable various controls.</summary>
		private void CheckPermissions() {
			//disable various controls and re-enable them below depending on permissions.
			textTitle.ReadOnly=true;
			comboPriority.Enabled=false;
			comboPhase.Enabled=false;
			butParentPick.Visible=false;
			butParentRemove.Visible=false;
			comboJobTeam.Enabled=false;
			textVersion.ReadOnly=true;
			butVersionPrompt.Enabled=false;
			textEditorProjectDescription.ReadOnly=true;
			butChangeEst.Enabled=false;
			butAddExistingJob.Enabled=false;
			if(_jobCur==null) {
				return;
			}
			butAddExistingJob.Enabled=true;
			if(_isOverride) {//Enable everything and make everything visible
				textTitle.ReadOnly=false;
				comboPriority.Enabled=true;
				comboPhase.Enabled=true;
				textVersion.ReadOnly=false;
				butParentPick.Visible=true;
				butParentRemove.Visible=true;
				comboJobTeam.Enabled=true;
				butVersionPrompt.Enabled=true;
				textEditorProjectDescription.ReadOnly=false;
				return;
			}
			//At this point, we want to return because users with view-only permission should not be able to make changes to the Project
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				return;
			}
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
					textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					textVersion.ReadOnly=false;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					comboJobTeam.Enabled=true;
					butVersionPrompt.Enabled=true;
					textEditorProjectDescription.ReadOnly=false;
					butChangeEst.Enabled=true;
					break;
				case JobPhase.Development:
					if(_jobCur.UserNumExpert!=0 && _jobCur.UserNumExpert!=Security.CurUser.UserNum) {
						break;//only the expert can edit the job description.
					}
					comboPriority.Enabled=true;
					textVersion.ReadOnly=false;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					comboJobTeam.Enabled=true;
					butVersionPrompt.Enabled=true;
					textEditorProjectDescription.ReadOnly=false;
					butChangeEst.Enabled=true;
					break;
				case JobPhase.Complete:
					break;
				case JobPhase.Cancelled:
					break;
				default:
					MessageBox.Show("Unsupported job status. Add to UserControlProjectEdit.CheckPermissions()");
					break;
			}
			//Set project description and title to readonly if "Checked out"
			textEditorProjectDescription.ReadOnly=false;
			if(_jobCur.UserNumCheckout!=0 && _jobCur.UserNumCheckout!=Security.CurUser.UserNum) {
				textTitle.ReadOnly=true;
				textEditorProjectDescription.ReadOnly=true;
			}
		}

		///<summary>This is a nasty method. Be careful with making changes to it.</summary>
		private void butActions_Click(object sender,EventArgs e) {
			bool hasPermission=false;
			ContextMenu actionMenu=new System.Windows.Forms.ContextMenu();
			//Job Actions are directed by current JobPhase
			//Anything not explicitly stated in the switch is not considered to be a valid JobState for this UserControl
			switch(_jobCur.PhaseCur) {
				#region Project Jobs
				case (JobPhase.Concept):
					hasPermission=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true) || _isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Send to In Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission});
					//Cancelling a concept is only allowed if you are the submitter
					actionMenu.MenuItems.Add(new MenuItem("Cancel Project",actionMenu_CancelJobClick) { Enabled=hasPermission && _jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobPhase.Development):
					hasPermission=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true) || _isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Project Manager",actionMenu_AssignProjectManagerClick) { Enabled=hasPermission });
					//If the job doesn't have an expert then an engineer may take the job
					if(_jobCur.UserNumExpert==0 && hasPermission) {
						actionMenu.MenuItems.Add(new MenuItem("Take Project",actionMenu_TakeJobClick) { Enabled=true });
					}
					hasPermission=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true) && (_jobCur.UserNumExpert==0 || _jobCur.UserNumExpert==Security.CurUser.UserNum);
					//The user may mark the job as implemented
					actionMenu.MenuItems.Add(new MenuItem("Mark as Implemented",actionMenu_ImplementedClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Project",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobPhase.Cancelled):
					hasPermission=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen Project as Concept",actionMenu_ReopenConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen Project as in Development",actionMenu_ReopenProjectClick) { Enabled=hasPermission });
					break;
				#endregion
			}
			if(_jobCur.UserNumCheckout>0 && _jobCur.UserNumCheckout!=Security.CurUser.UserNum && !_isOverride) {
				//disable all menu items if job is checked out by other user.
				actionMenu.MenuItems.OfType<MenuItem>().ToList().ForEach(x => x.Enabled=false);
			}
			//Always add the ability to view JobLogs
			actionMenu.MenuItems.Add("View Logs",actionMenu_ViewLogsClick);
			//If a user has the Override permission, then add the Override action
			if(JobPermissions.IsAuthorized(JobPerm.Override,true)) {
				actionMenu.MenuItems.Add("-");
				actionMenu.MenuItems.Add("Override",actionMenu_OverrideClick);
			}
			//If the user has no actions other than Override, then remove the extra "-" item
			if(actionMenu.MenuItems.Count>0 && actionMenu.MenuItems[0].Text=="-") {
				actionMenu.MenuItems.RemoveAt(0);
			}
			//If the user has no actions for whatever reason, show them a menu that indicates they can do perform no actions
			if(actionMenu.MenuItems.Count==0) {
				actionMenu.MenuItems.Add(new MenuItem("No Actions Available") { Enabled=false });
			}
			butActions.ContextMenu=actionMenu;
			butActions.ContextMenu.Show(butActions,new Point(0,butActions.Height));
		}

		//Allows save to be called from outside this control.
		public bool ForceSave() {
			if(_jobCur==null || IsChanged==false) {
				return true;//Nothing to save
			}
			if(!ValidateJob(_jobCur)) {
				return false;//Job failed validation
			}
			SaveJob(_jobCur);
			return true;
		}

		///<summary>If returns false if selection is cancelled. DefaultUserNum is usually the currently set usernum for a given role.</summary>
		private bool PickUserByJobPermission(string prompt,JobPerm jobPerm,out long selectedNum,long suggestedUserNum=0,bool AllowNone=true) {
			selectedNum=0;
			List<Userod> listUsersForPicker=Userods.GetUsersByJobRole(jobPerm,false);
			FrmUserPick frmUserPick=new FrmUserPick();
			frmUserPick.Text=prompt;
			frmUserPick.IsSelectionMode=true;
			frmUserPick.ListUserodsFiltered=listUsersForPicker;
			frmUserPick.UserNumSuggested=suggestedUserNum;
			frmUserPick.IsPickNoneAllowed=AllowNone;
			frmUserPick.IsShowAllAllowed=false;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return false;
			}
			selectedNum=frmUserPick.UserNumSelected;
			return true;
		}

		///<summary>When editing a job, and the job has been changed, this loads changes into the current control.</summary>
		public void LoadMergeJob(Job newJob) {
			_isLoading=true;
			Job jobMerge = newJob.Copy();//Set _jobCur lists to the new lists made above.
			_jobCur.ListJobLinks    =jobMerge.ListJobLinks;
			_jobCur.ListJobNotes    =jobMerge.ListJobNotes;
			_jobCur.ListJobTimeLogs =jobMerge.ListJobTimeLogs;
			//Update Old lists too
			_jobOld.ListJobLinks    =jobMerge.ListJobLinks.Select(x => x.Copy()).ToList();
			_jobOld.ListJobNotes    =jobMerge.ListJobNotes.Select(x => x.Copy()).ToList();
			_jobOld.ListJobTimeLogs =jobMerge.ListJobTimeLogs.Select(x => x.Copy()).ToList();
			//JOB ROLE USER NUMS
			_jobCur.UserNumCheckout=jobMerge.UserNumCheckout;
			_jobCur.UserNumConcept=jobMerge.UserNumConcept;
			_jobCur.UserNumEngineer=jobMerge.UserNumEngineer;
			_jobCur.UserNumExpert=jobMerge.UserNumExpert;
			_jobCur.UserNumInfo=jobMerge.UserNumInfo;
			//old
			_jobOld.UserNumCheckout=jobMerge.UserNumCheckout;
			_jobOld.UserNumConcept=jobMerge.UserNumConcept;
			_jobOld.UserNumEngineer=jobMerge.UserNumEngineer;
			_jobOld.UserNumExpert=jobMerge.UserNumExpert;
			_jobOld.UserNumInfo=jobMerge.UserNumInfo;
			textProjectManager.Text=_jobCur.UserNumExpert==0 ? "Unassigned" : Userods.GetName(_jobCur.UserNumExpert);
			textSubmitter.Text=Userods.GetName(_jobCur.UserNumConcept);
			textCheckedOut.Text=Userods.GetName(_jobCur.UserNumCheckout);
			if(_jobCur.UserNumCheckout==0) {
				textCheckedOut.BackColor=Color.FromArgb(255, 240, 240, 240);//control
			}
			else if(_jobCur.UserNumCheckout!=Security.CurUser.UserNum) {
				textCheckedOut.BackColor=Color.FromArgb(254,235,233);//light red
			}
			else {
				textCheckedOut.BackColor=Color.FromArgb(235,254,233);//light green
			}
			//All changes below this point will be lost if there is a conflicting change detected.
			//TITLE
			if(_jobCur.Title!=jobMerge.Title) {
				if(_jobCur.Title==_jobOld.Title) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Title=jobMerge.Title;
					_jobOld.Title=jobMerge.Title;
					textTitle.Text=_jobCur.Title;
				}
			}
			//PROJECT DESCRIPTION
			if(_jobCur.Requirements!=jobMerge.Requirements) {
				if(textEditorProjectDescription.MainRtf==_jobOld.Requirements) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Requirements=jobMerge.Requirements;
					_jobOld.Requirements=jobMerge.Requirements;
					try {
						textEditorProjectDescription.MainRtf=_jobCur.Requirements;
					}
					catch {
						textEditorProjectDescription.MainText=_jobCur.Requirements;
					}
				}
			}
			//PRIORITY
			if(_jobCur.Priority!=jobMerge.Priority) {
				_jobCur.Priority=jobMerge.Priority;
				_jobOld.Priority=jobMerge.Priority;
				comboPriority.SelectedIndex=_listPriorities.FirstOrDefault(x => x.DefNum==_jobCur.Priority)?.ItemOrder??0;
			}
			//STATUS
			if(_jobCur.PhaseCur!=jobMerge.PhaseCur) {
				_jobCur.PhaseCur=jobMerge.PhaseCur;
				_jobOld.PhaseCur=jobMerge.PhaseCur;
				comboPhase.SetSelectedEnum(_jobCur.PhaseCur);
			}
			//JOBTEAM
			JobLink jobLink = _jobCur.ListJobLinks.Find(x => x.LinkType==JobLinkType.JobTeam);
			if(jobLink==null) {
				comboJobTeam.SelectedIndex=0;
			}
			else {
				comboJobTeam.SetSelectedKey<JobTeam>(jobLink.FKey,x => x.JobTeamNum);
			}
			//DATEENTRY - Cannot change
			//VERSION
			if(_jobCur.JobVersion!=jobMerge.JobVersion && _jobCur.JobVersion==_jobOld.JobVersion) {//Was edited, AND user has not already edited it themselves.
				_jobCur.JobVersion=jobMerge.JobVersion;
				_jobOld.JobVersion=jobMerge.JobVersion;
				textVersion.Text=_jobCur.JobVersion;
			}
			textVersion.BackColor=Color.White;
			//CONCEPT EST
			if(_jobCur.HoursEstimateConcept!=jobMerge.HoursEstimateConcept && _jobCur.HoursEstimateConcept==_jobOld.HoursEstimateConcept) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateConcept=jobMerge.HoursEstimateConcept;
				_jobOld.HoursEstimateConcept=jobMerge.HoursEstimateConcept;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
			}
			//WRITEUP EST
			if(_jobCur.HoursEstimateWriteup!=jobMerge.HoursEstimateWriteup && _jobCur.HoursEstimateWriteup==_jobOld.HoursEstimateWriteup) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateWriteup=jobMerge.HoursEstimateWriteup;
				_jobOld.HoursEstimateWriteup=jobMerge.HoursEstimateWriteup;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
			}
			//DEVELOPMENT EST
			if(_jobCur.HoursEstimateDevelopment!=jobMerge.HoursEstimateDevelopment && _jobCur.HoursEstimateDevelopment==_jobOld.HoursEstimateDevelopment) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateDevelopment=jobMerge.HoursEstimateDevelopment;
				_jobOld.HoursEstimateDevelopment=jobMerge.HoursEstimateDevelopment;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
			}
			//REVIEW EST
			if(_jobCur.HoursEstimateReview!=jobMerge.HoursEstimateReview && _jobCur.HoursEstimateReview==_jobOld.HoursEstimateReview) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateReview=jobMerge.HoursEstimateReview;
				_jobOld.HoursEstimateReview=jobMerge.HoursEstimateReview;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
			}
			//PARENT
			if(_jobCur.ParentNum!=jobMerge.ParentNum && _jobCur.JobNum==_jobOld.JobNum) {//Parent was edited, AND user has not already edited the parent themselves.
				_jobCur.JobNum=jobMerge.JobNum;
				_jobOld.JobNum=jobMerge.JobNum;
			}
			//IS ACTIVE
			if(_jobCur.ListJobActiveLinks.Exists(x => x.UserNum==Security.CurUser.UserNum && x.DateTimeEnd==DateTime.MinValue)) {
				checkIsActive.Checked=true;
			}
			else {
				checkIsActive.Checked=false;
			}
			FillGridProjects();
			FillAllChildGrids();
			_isLoading=false;
			CheckPermissions();
		}

		#region Grid Events
		private void FillPriorityList() {
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<string> listPriorityItemValues=_listJobPriorities.SelectMany(y => y.ItemValue.Split(',')).ToList();
		}

		private void gridProjects_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridProjects.ListGridRows[e.Row].Tag!=null && gridProjects.ListGridRows[e.Row].Tag is Job) {
				if(RequestJob!=null) {
					RequestJob(this,((Job)gridProjects.ListGridRows[e.Row].Tag).JobNum);
				}
			}
		}

		private void gridProjects_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!IsPopout) {
				FormJobManager.OpenNonModalJob(GetSelectedJob(gridProjects,e.Row));
			}
		}

		private void gridDiscussion_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridDiscussion.ListGridRows[e.Row].Tag is JobNote)) {
				return;//should never happen.
			}
			if(EditJobNote((JobNote)gridDiscussion.ListGridRows[e.Row].Tag)) {
				FillGridDiscussion();
			}
		}

		private void gridJob_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!IsPopout) {
				FormJobManager.OpenNonModalJob(GetSelectedJob((GridOD)sender,e.Row));
			}
		}
		#endregion Grid Events

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

		private void AddNewJob(JobCategory jobCategory,long parentNum=0,long topParentNum=0) {
			Job jobNew=new Job();
			jobNew.ParentNum=parentNum;
			jobNew.TopParentNum=topParentNum;
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true);
			if(listJobPriorities.Count==0) {
				MsgBox.Show(this,"You have no priorities setup in definitions. Go to definitions and set up Job Priorities first.");
				return;
			}
			jobNew.Category=jobCategory;
			long priorityNum=listJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("JobDefault")).DefNum;
			if(jobNew.Category.In(JobCategory.Bug,JobCategory.Conversion,JobCategory.NeedNoApproval,JobCategory.UnresolvedIssue)) {
				priorityNum=listJobPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault")).DefNum;
			}
			jobNew.Priority=priorityNum==0 ? listJobPriorities.First().DefNum : priorityNum;
			jobNew.PhaseCur=JobPhase.Concept;
			jobNew.UserNumConcept=Security.CurUser.UserNum;
			using FormJobAdd formJobAdd=new FormJobAdd(jobNew);
			if(formJobAdd.ShowDialog()==DialogResult.OK) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobNew.JobNum);
			}
		}

		#region Combo Events
		private void comboJobTeam_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			long fKey=comboJobTeam.GetSelected<JobTeam>().JobTeamNum;
			JobLink jobLink=_jobCur.ListJobLinks.FirstOrDefault(x=>x.LinkType==JobLinkType.JobTeam);
			//If it was switched from a team to none, remove jobLink.
			if(fKey==-1) {
				if(jobLink!=null) {
					RemoveJobLink(JobLinkType.JobTeam,jobLink.FKey);
				}
				return;
			}
			//If it was switched from one team to another.
			if(jobLink!=null) {
				jobLink.FKey=fKey;
				JobLinks.Update(jobLink);
			}
			else {//If it was switched from none to a team.
				jobLink=new JobLink();
				jobLink.FKey=fKey;
				jobLink.JobNum=_jobCur.JobNum;
				jobLink.LinkType=JobLinkType.JobTeam;
				JobLinks.Insert(jobLink);
				_jobCur.ListJobLinks.Add(jobLink);
				_jobOld.ListJobLinks.Add(jobLink);
			}
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void comboPriority_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading || comboPriority.SelectedIndex==-1) {
				return;
			}
			long priorityNum=comboPriority.GetSelectedDefNum();
			_jobCur.Priority=priorityNum;
			JobLogs.MakeLogEntryForPriority(_jobCur,_jobOld);
			_jobOld.Priority=priorityNum;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=job.Copy();
				job.Priority=priorityNum;
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				}
			}
		}

		private void comboPhase_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			JobPhase jobPhaseNew=comboPhase.GetSelected<JobPhase>();
			_jobCur.PhaseCur=jobPhaseNew;
			JobLogs.MakeLogEntryForPhase(_jobCur,_jobOld);
			_jobOld.PhaseCur=jobPhaseNew;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=job.Copy();
				job.PhaseCur=comboPhase.GetSelected<JobPhase>();
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				}
			}
		}

		///<summary>A helper function that removes a job link for the given key and type.</summary>
		private void RemoveJobLink(JobLinkType linkType,long fKey) {
			List<JobLink> listLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==linkType && x.FKey==fKey);
			_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==linkType && x.FKey==fKey);
			_jobOld.ListJobLinks.RemoveAll(x => x.LinkType==linkType && x.FKey==fKey);
			listLinks.Select(x => x.JobLinkNum).ToList().ForEach(JobLinks.Delete);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}
		#endregion

		#region Button Events
		private void textVersion_TextChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(IsNew) {
				_jobCur.JobVersion=textVersion.Text;
				_jobOld.JobVersion=textVersion.Text;
				return;
			}
			textVersion.BackColor=Color.FromArgb(255,255,230);//light yellow
			timerVersion.Stop();
			timerVersion.Start();
		}

		private void butVersionPrompt_Click(object sender,EventArgs e) {
			using FormVersionPrompt formVersionPrompt=new FormVersionPrompt();
			formVersionPrompt.VersionText=textVersion.Text;
			formVersionPrompt.ShowDialog();
			if(formVersionPrompt.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(formVersionPrompt.VersionText)) {
				return;
			}
			textVersion.Text=formVersionPrompt.VersionText;
		}

		private void timerVersion_Tick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			timerVersion.Stop();
			textVersion.BackColor=Color.White;
			_jobCur.JobVersion=textVersion.Text;
			_jobOld.JobVersion=textVersion.Text;
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=job.Copy();
				job.JobVersion=textVersion.Text;
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
				}
			}
		}

		private void butChangeEst_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			using FormJobEstimate formJobEstimate=new FormJobEstimate(_jobCur);
			if(formJobEstimate.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textEstHours.Text=_jobCur.HoursEstimate.ToString();
			Job jobFromDB = Jobs.GetOne(_jobCur.JobNum);//Get from DB to ensure freshest copy (Lists not filled)
			Job jobOld=jobFromDB.Copy();
			jobFromDB.HoursEstimateConcept=_jobCur.HoursEstimateConcept;
			jobFromDB.HoursEstimateWriteup=_jobCur.HoursEstimateWriteup;
			jobFromDB.HoursEstimateDevelopment=_jobCur.HoursEstimateDevelopment;
			jobFromDB.HoursEstimateReview=_jobCur.HoursEstimateReview;
			if(Jobs.Update(jobFromDB,jobOld)) {//update the checkout num.
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
			}
		}
		private void butAddExistingJob_Click(object sender,EventArgs e) {
			AddExistingJobToProject();
		}

		private void AddExistingJobToProject() {
			if(_isLoading || _jobCur==null) {
				return;
			}
			if((_jobCur.PhaseCur==JobPhase.Cancelled || _jobCur.PhaseCur==JobPhase.Complete) && !IsOverride) {
				MessageBox.Show("Cannot add child Jobs to Projects that are Complete or Cancelled.");
				return;
			}
			using FormJobSearch formJobSearch=new FormJobSearch();
			formJobSearch.ShowDialog();
			if(formJobSearch.DialogResult!=DialogResult.OK) {
				return;
			}
			Job job=formJobSearch.SelectedJob;
			if(job==null) {
				return;
			}
			if(job.JobNum==_jobCur.JobNum) {
				MsgBox.Show(this,"A child job must be different from its parent. "+_jobCur.ToString());
				return;
			}
			else if(job.ParentNum==_jobCur.JobNum) {
				MsgBox.Show(this,"The selected job is already a child of "+_jobCur.ToString()+".");
				return;
			}
			else if(job.ParentNum!=0
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected job already has a parent. Are you sure you want to reassign its parent to "+_jobCur.ToString()+"?")) {
				return;
			}
			//Assign parent
			if(Jobs.CheckForLoop(job.JobNum,_jobCur.JobNum)) {//check infinite loop for selected job and new parent
				MsgBox.Show(this,"Invalid parent job, would create an infinite loop.");
				return;
			}
			long topParentNumOld=job.TopParentNum;// old top parent of selected
			long parentNumOld=job.ParentNum;
			job.ParentNum=_jobCur.JobNum;// set new
			job.TopParentNum=_jobCur.TopParentNum;// set new
			UpdateParentForJobs(job,parentNumOld,topParentNumOld);
		}

		private void UpdateParentForJobs(Job job,long parentNumOld,long topParentNumOld) {
			Jobs.Update(job);
			if(job.ParentNum!=parentNumOld) {
				JobLogs.MakeLogEntryForParentChange(job,job.ParentNum,parentNumOld);
			}
			List<Job> listJobsUpdated=new List<Job>{job};
			if(job.TopParentNum!=topParentNumOld) {
				List<Job> listTopParentJobs=Jobs.GetAllByTopParentNum(topParentNumOld);
				listJobsUpdated.AddRange(Jobs.GetChildJobs(job,listTopParentJobs));
			}
			listJobsUpdated.ForEach(x => x.TopParentNum=job.TopParentNum);
			Jobs.UpdateTopParentList(job.TopParentNum,listJobsUpdated.Select(x => x.JobNum).ToList());
			for(int i=0;i<listJobsUpdated.Count;i++) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,listJobsUpdated[i].JobNum);
			}
		}

		private void butParentRemove_Click(object sender,EventArgs e) {
			if(_jobCur==null) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to remove the parent of the currently selected Project?")) {
				return;
			}
			long parentNumOld=_jobCur.ParentNum;
			if(IsNew) {
				IsChanged=true;
			}
			else {
				long topParentNumOld=_jobCur.TopParentNum;
				_jobCur.ParentNum=0;
				_jobOld.ParentNum=0;
				_jobCur.TopParentNum=_jobCur.JobNum;
				_jobOld.TopParentNum=_jobCur.JobNum;
				UpdateParentForJobs(_jobCur,parentNumOld,topParentNumOld);
			}
		}

		private void butParentPick_Click(object sender,EventArgs e) {
			if(_jobCur==null) {
				return;
			}
			InputBox inputBox=new InputBox("Input parent job number.");
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return;
			}
			long parentNum=0;
			long topParentNumOld=_jobCur.TopParentNum;
			long.TryParse(new string(inputBox.StringResult.Where(char.IsDigit).ToArray()),out parentNum);
			Job job=Jobs.GetOne(parentNum);
			if(job==null) {
				return;
			}
			if(job.Category!=JobCategory.Project) {
				MsgBox.Show(this,"Invalid parent, parent must be a Project.");
				return;
			}
			if(Jobs.CheckForLoop(_jobCur.JobNum,job.JobNum)) {
				MsgBox.Show(this,"Invalid parent job, would create an infinite loop.");
				return;
			}
			long parentNumOld=_jobCur.ParentNum;
			if(IsNew) {
				IsChanged=true;
			}
			else {
				_jobCur.ParentNum=job.JobNum;
				_jobOld.ParentNum=job.JobNum;
				_jobCur.TopParentNum=job.TopParentNum;
				_jobOld.TopParentNum=job.TopParentNum;
				UpdateParentForJobs(_jobCur,parentNumOld,topParentNumOld);
			}
		}
		#endregion

		#region Project Options Events

		private void actionMenu_SendInDevelopmentClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert=_jobCur.UserNumExpert;
			long userNumSuggestedExpert=(_jobCur.UserNumExpert>0) ? _jobCur.UserNumExpert : _jobCur.UserNumConcept;
			if(_jobCur.UserNumExpert==0 
				&& !PickUserByJobPermission("Pick Project Manager",JobPerm.ProjectManager,out userNumExpert,userNumSuggestedExpert,true)) 
			{
				return;
			}
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumExpert;
			_jobCur.PhaseCur=JobPhase.Development;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignSubmitterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumConcept;
			if(!PickUserByJobPermission("Pick Submitter",JobPerm.ProjectManager,out userNumConcept,_jobCur.UserNumConcept,false)) {
				return;
			}
			if(userNumConcept==_jobOld.UserNumConcept) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumConcept=userNumConcept;
			SaveJob(_jobCur);
		}

		private void actionMenu_ReopenProjectClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert=_jobCur.UserNumExpert;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Project Manager",JobPerm.ProjectManager,out userNumExpert,_jobCur.UserNumConcept,true)) {
				return;
			}
			if(_jobCur.UserNumApproverConcept==0) {
				_jobCur.UserNumApproverConcept=Security.CurUser.UserNum;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumExpert;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverJob=0;
			_jobCur.UserNumApproverChange=0;
			_jobCur.PhaseCur=JobPhase.Development;
			_jobCur.DateTimeJobApproval=DateTime.Now;
			SaveJob(_jobCur);
		}

		private void actionMenu_ReopenConceptClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert=_jobCur.UserNumExpert;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Project Manager",JobPerm.ProjectManager,out userNumExpert,_jobCur.UserNumConcept,true)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumExpert;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverConcept=Security.CurUser.UserNum;
			_jobCur.UserNumApproverJob=0;
			_jobCur.UserNumApproverChange=0;
			_jobCur.PhaseCur=JobPhase.Concept;
			_jobCur.DateTimeConceptApproval=DateTime.Now;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignProjectManagerClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert;
			if(!PickUserByJobPermission("Pick Project Manager",JobPerm.ProjectManager,out userNumExpert,_jobCur.UserNumExpert,true)) {
				return;
			}
			if(userNumExpert==_jobOld.UserNumExpert) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumExpert;
			SaveJob(_jobCur);
		}

		private void actionMenu_TakeJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			_jobCur.UserNumExpert=Security.CurUser.UserNum;
			_jobCur.UserNumEngineer=Security.CurUser.UserNum;
			SaveJob(_jobCur);
		}

		private void actionMenu_ImplementedClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			using FormVersionPrompt formVersionPrompt=new FormVersionPrompt("",true);
			formVersionPrompt.VersionText=textVersion.Text;
			formVersionPrompt.ShowDialog();
			if(formVersionPrompt.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(formVersionPrompt.VersionText)) {
				return;
			}
			textVersion.Text=formVersionPrompt.VersionText;
			_jobCur.JobVersion=formVersionPrompt.VersionText;
			_jobCur.Priority=_listPrioritiesAll.FirstOrDefault(x => x.ItemValue.Contains("DocumentationDefault")).DefNum;
			comboPriority.SetSelectedDefNum(_jobCur.Priority);
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Complete;
			_jobCur.DateTimeImplemented=DateTime.Now;
			SaveJob(_jobCur);
		}

		private void actionMenu_CancelJobClick(object sender,EventArgs e) {
			List<Job> listJobsChildren=JobManagerCore.ListJobsAll.FindAll(x => x.ParentNum==_jobCur.JobNum && !x.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete));
			string text=listJobsChildren.IsNullOrEmpty() ? "" : "There are child jobs for this Project. ";
			if(!ValidateJob(_jobCur) 
				|| !MsgBox.Show(this,MsgBoxButtons.YesNo,text+"Are you sure you want to cancel this Project?")) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Cancelled;
			SaveJob(_jobCur);
		}

		private void actionMenu_ViewLogsClick(object sender,EventArgs e) {
			FormJobLogs formJobLogs=new FormJobLogs(_jobCur);
			formJobLogs.Show();
		}

		private void actionMenu_OverrideClick(object sender,EventArgs e) {
			IsOverride=true;
		}
		#endregion Project Options Events

		#region ContextMenu Events
		/// <summary>Use this to set custom rules for enabling/disabling MenuItems for a grid's ContextMenu.</summary>
		private void gridContextMenu_Popup(object sender, EventArgs e) { 
			try {
				ContextMenu contextMenu=(ContextMenu)sender;
				//Always start with an Enabled state and disable as needed below
				SetEnabledForAllChildMenuItems(contextMenu.MenuItems,true);
				if(!TryGetSelectedJobFromGrid((GridOD)contextMenu.Tag,out Job job)){
					SetEnabledForAllChildMenuItems(contextMenu.MenuItems,false);
					return;
				}
				//Cannot create child jobs for Documentation/Cancelled/Complete Phases
				bool isEnabled=(!job.PhaseCur.In(JobPhase.Documentation,JobPhase.Complete,JobPhase.Cancelled));
				contextMenu.MenuItems.Find(_CHILD_JOB_MENU,true).FirstOrDefault().Enabled=isEnabled;
			}
			catch(Exception ex) { 
				MsgBox.Show("There was an error setting the Enabled values for this context menu's items.\r\n"+ex.Message);
			}
		}

		private void menuItemCreateChildJob_Click(object sender, EventArgs e) {
			MenuItem menuItem=(MenuItem)sender;
			if(!TryGetSelectedJobFromMenuItem(menuItem,out Job job,out string error)) {
				MsgBox.Show(error);
				return;
			}
			if(!Enum.TryParse(menuItem.Text,out JobCategory category)) {
				MsgBox.Show("There was an error getting the JobCategory for your selection.");
				return;
			}
			AddNewJob(category,job.JobNum,job.TopParentNum);
		}

		/// <summary>Use this to set custom rules for enabling/disabling JobCategory MenuItems in the ContextMenu.</summary>
		private void menuItemCreateChildJob_Popup(object sender, EventArgs e) {
			try {
				MenuItem parentMenuItem=(MenuItem)sender;
				//Always start with an Enabled state and disable as needed below
				SetEnabledForAllChildMenuItems(parentMenuItem.MenuItems,true);
				if(!TryGetSelectedJobFromGrid((GridOD)parentMenuItem.Tag,out Job job)){
					SetEnabledForAllChildMenuItems(parentMenuItem.MenuItems,false);
					return;
				}
				//Projects must be children of other Projects
				bool isEnabled=(job.Category==JobCategory.Project);
				MenuItem menuItem=parentMenuItem.MenuItems.Find(JobCategory.Project.ToString(),true).FirstOrDefault();
				if(menuItem!=null) {
					menuItem.Enabled=isEnabled;
				}
				//Additional rules here ...
			}
			catch(Exception ex) { 
				MsgBox.Show("There was an error setting the Enabled values for Child Job Categories.\r\n"+ex.ToString());
			}
		}

		private void menuItemGoToJob_Click(object sender, EventArgs e) {
			if(!TryGetSelectedJobFromMenuItem((MenuItem)sender,out Job job,out string error)){
				MsgBox.Show(error);
				return;
			}
			if(RequestJob!=null) { 
				RequestJob(this,job.JobNum);
			}
		}
				
		/// <summary>Adds MenuItems to the MenuItem passed in, one for each JobCategory the user has permission to. For clarity: this manipulates the passed in object.</summary>
		private void AddChildJobCategoriesToMenuItem(MenuItem menuItemCreateChildJob,GridOD grid) {
			if(menuItemCreateChildJob==null || grid==null) {
				return;
			}
			//The context menu will be populated with the categories a user is allowed to create based on their permissions.
			foreach(string category in JobManagerCore.CategoryList) {
				MenuItem menuItem = new MenuItem();
				menuItem.Text=category;
				menuItem.Tag=grid;
				menuItem.Name=category;
				menuItem.Click+=new System.EventHandler(menuItemCreateChildJob_Click);
				menuItemCreateChildJob.MenuItems.Add(menuItem);
			}
		}

		private void CreateGridContextMenus() {
			List<GridOD> listGrids=new List<GridOD>() {
				gridProjects,
				gridConcept,
				gridDefinition,
				gridDevelopment,
				gridPendingReview,
				gridDocumentation,
				gridComplete
			};
			foreach(GridOD grid in listGrids) {
				if(grid.ContextMenu==null) { 
					grid.ContextMenu=new ContextMenu();
					grid.ContextMenu.Tag=grid;
					grid.ContextMenu.Popup+=new System.EventHandler(gridContextMenu_Popup);
					//Go to Job
					MenuItem menuItemGoToJob=new MenuItem();
					menuItemGoToJob.Text="Go to Job";
					menuItemGoToJob.Click+=new System.EventHandler(menuItemGoToJob_Click);
					menuItemGoToJob.Tag=grid;
					grid.ContextMenu.MenuItems.Add(menuItemGoToJob);
					//Create child job (nested menu)
					MenuItem menuItemCreateChildJob=new MenuItem();
					menuItemCreateChildJob.Text=_CHILD_JOB_MENU;
					menuItemCreateChildJob.Name=_CHILD_JOB_MENU;
					menuItemCreateChildJob.Tag= grid;
					AddChildJobCategoriesToMenuItem(menuItemCreateChildJob,grid);
					menuItemCreateChildJob.Popup+=new System.EventHandler(menuItemCreateChildJob_Popup);
					grid.ContextMenu.MenuItems.Add(menuItemCreateChildJob);
				}
			}
		}

		private void SetEnabledForAllChildMenuItems(System.Windows.Forms.Menu.MenuItemCollection menuItems,bool isEnabled) {
			foreach(MenuItem childMenuItem in menuItems) {
				childMenuItem.Enabled=isEnabled;
			}
		}

		private bool TryGetSelectedJobFromGrid(GridOD grid,out Job job) {
			job=null;
			if(grid==null) {
				return false;
			}
			job=grid.SelectedTag<Job>();
			if(job==null) {
				return false;
			}
			return true;
		}
		#endregion ContextMenu Events

		private bool ValidateJob(Job job) {
			if(string.IsNullOrWhiteSpace(job.Title)) {
				MessageBox.Show("Invalid Title.");
				return false;
			}
			return true;
		}
		
		///<summary>Job must have all in memory fields filled. Eg. Job.ListJobLinks, Job.ListJobNotes, etc. Also makes some of the JobLog entries.</summary>
		private void SaveJob(Job job) {
			_isLoading=true;
			timerTitle.Stop();
			timerVersion.Stop();
			//Validation must happen before this is called.
			job.Requirements=textEditorProjectDescription.MainRtf;
			job.JobVersion=textVersion.Text;
			job.Title=textTitle.Text;
			//All other fields should have been maintained while editing the job in the form.
			job.UserNumCheckout=0;
			if(job.JobNum==0 || IsNew) {
				if(job.UserNumConcept==0) {
					job.UserNumConcept=Security.CurUser.UserNum;
				}
				Jobs.Insert(job);
				job.ListJobLinks.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobNotes.ForEach(x=>x.JobNum=job.JobNum);
			}
			else {
				Jobs.Update(job);
			}
			JobLinks.Sync(job.ListJobLinks,job.JobNum);
			JobNotes.Sync(job.ListJobNotes,job.JobNum);
			JobActiveLinks.UpsertLink(job,_jobOld,Security.CurUser.UserNum,checkIsActive.Checked);
			MakeLogEntry(job,_jobOld);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			JobManagerCore.UpdateForSingleJob(job);
			LoadJob(job);
			_isLoading=false;
		}

		///<summary>Displays the Job Note Edit to the user.  Manipulates _jobCur.ListJobNotes accordingly.
		///Returns true if the note was added; Otherwise, false.</summary>
		private bool AddJobNote(JobNoteTypes noteType=JobNoteTypes.Discussion) {
			if(_jobCur==null) {
				return false;//should never happen
			}
			JobNote jobNote=new JobNote() {
				DateTimeNote=MiscData.GetNowDateTime(),
				IsNew=true,
				JobNum=_jobCur.JobNum,
				UserNum=Security.CurUser.UserNum,
				NoteType=noteType,
			};
			using FormJobNoteEdit formJobNoteEdit=new FormJobNoteEdit(jobNote);
			formJobNoteEdit.ShowDialog();
			if(formJobNoteEdit.DialogResult!=DialogResult.OK || formJobNoteEdit.JobNoteCur==null) {
				return false;
			}
			if(!IsNew) {
				JobNotes.Insert(formJobNoteEdit.JobNoteCur);
				JobNotifications.UpsertAllNotifications(_jobCur,Security.CurUser.UserNum,JobNotificationChanges.NoteAdded);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobNotes.Add(formJobNoteEdit.JobNoteCur);
			return true;
		}

		///<summary>Edits the job note at the selected index provided.  Updates _jobCur.ListJobNotes accordingly.
		///Returns true if the note was changed; Otherwise, false.</summary>
		private bool EditJobNote(JobNote jobNote) {
			JobNote jobNoteOld=jobNote.Copy();
			using FormJobNoteEdit formJobNoteEdit=new FormJobNoteEdit(jobNote);
			formJobNoteEdit.ShowDialog();
			if(formJobNoteEdit.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(jobNote.NoteType==JobNoteTypes.Discussion) {
				MakeLogEntryForNote(formJobNoteEdit.JobNoteCur,jobNoteOld);
			}
			if(IsNew) {
				IsChanged=true;
			}
			else {
				if(formJobNoteEdit.JobNoteCur==null) {
					JobNotes.Delete(jobNote.JobNoteNum);
				}
				else {
					JobNotes.Update(formJobNoteEdit.JobNoteCur);
				}
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			_jobCur.ListJobNotes.RemoveAll(x => x.JobNoteNum==jobNote.JobNoteNum);
			if(formJobNoteEdit.JobNoteCur!=null) {
				_jobCur.ListJobNotes.Add(formJobNoteEdit.JobNoteCur);
			}
			return true;
		}

		private void textTitle_TextChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(IsNew) {
				_jobCur.Title=textTitle.Text;
				_jobOld.Title=textTitle.Text;
				return;
			}
			textTitle.BackColor=Color.FromArgb(255,255,230);//light yellow
			timerTitle.Stop();
			timerTitle.Start();
		}

		private void timerTitle_Tick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			timerTitle.Stop();
			if(string.IsNullOrWhiteSpace(textTitle.Text)) {
				textTitle.BackColor=Color.FromArgb(254,235,233);//light red
				return;
			}
			JobLogs.MakeLogEntryForTitleChange(_jobCur,_jobOld.Title,textTitle.Text);
			textTitle.BackColor=Color.White;
			_jobCur.Title=textTitle.Text;
			_jobOld.Title=textTitle.Text;
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=job.Copy();
				job.Title=textTitle.Text;
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
				}
			}
			textTitle.SpellCheck();
		}

		///<summary>SaveClick for textboxes: Concept</summary>
		private void textEditor_SaveClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			SaveJob(_jobCur);
		}

		///<summary>This is fired whenever a change is made to the textboxes: Concept, Writeup, Documentation.</summary>
		private void textEditor_OnTextEdited() {
			if(_isLoading) {
				return;
			}
			IsChanged=true;
			if(!_isLoading && _jobCur.UserNumCheckout==0) {
				_jobCur.UserNumCheckout=Security.CurUser.UserNum;
				Job job=Jobs.GetOne(_jobCur.JobNum);//Get from DB to ensure freshest copy (Lists not filled)
				Job jobOld=job.Copy();
				job.UserNumCheckout=Security.CurUser.UserNum;//change only the userNumCheckout.
				if(Jobs.Update(job,jobOld)) {//update the checkout num.
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
				}
			}
		}

		private void gridDiscussion_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(AddJobNote()) {
				FillGridDiscussion();
			}
		}	
		
		private void checkIncludeComplete_CheckedChanged(object sender,EventArgs e) {
			Jobs.RefreshInMemoryListByTopParent(_jobCur,checkIncludeComplete.Checked,checkIncludeCancelled.Checked);
			FillGridProjects();
			FillJobGrid(JobPhase.Complete,gridComplete);
		}

		private void checkIncludeCancelled_CheckedChanged(object sender,EventArgs e) {
			Jobs.RefreshInMemoryListByTopParent(_jobCur,checkIncludeComplete.Checked,checkIncludeCancelled.Checked);
			FillGridProjects();
			FillJobGrid(JobPhase.Complete,gridComplete);
		}

		private void MakeLogEntry(Job jobNew,Job jobOld,bool isManualUpdate=false) {
			JobLogs.MakeLogEntry(jobNew,jobOld,isManualUpdate);
		}

		private void MakeLogEntryForNote(JobNote jobNoteNew,JobNote jobNoteOld) {
			JobLogs.MakeLogEntryForNote(_jobCur,jobNoteNew,jobNoteOld);
		}

		private void CreateViewLog(Job jobPrevious) {
			if(jobPrevious!=null && jobPrevious.JobNum==_jobCur.JobNum) {//Skip out if you click on the same job twice
				return;
			}
			if(JobLogs.HasViewLogForToday(_jobCur.JobNum,Security.CurUser.UserNum)) {
				return;
			}
			JobLogs.MakeLogEntryForView(_jobCur);
		}

		///<summary>Gets a list of all descendents that are not cancelled jobs.</summary>
		private List<Job> GetDescedentJobs(long parentNum,bool doRefreshJobs=false) {
			if(_listJobsByTopParent==null || doRefreshJobs) {
				bool includeComplete=checkIncludeComplete.Checked || checkShowAllChildJobs.Checked;
				RefreshListJobsByTopParent(includeComplete,checkIncludeCancelled.Checked);
			}
			List<Job> listJobsChildren=_listJobsByTopParent.FindAll(x => x.ParentNum==parentNum);
			List<Job> listJobsDescedents=new List<Job>();
			if(listJobsChildren.IsNullOrEmpty()) {
				return listJobsChildren;
			}
			foreach(Job job in listJobsChildren) {
				if(!checkIncludeCancelled.Checked && job.PhaseCur==JobPhase.Cancelled) {
					continue;
				}
				listJobsDescedents.AddRange(GetDescedentJobs(job.JobNum,false));
			}
			listJobsChildren.AddRange(listJobsDescedents);
			return listJobsChildren;
		}

		private void butCalculateSums_Click(object sender,EventArgs e) {
			if(_jobCur==null || _jobCur.PhaseCur==JobPhase.Cancelled) {
				return;
			}
			//Get listJobs
			RefreshListJobsByTopParent(true,false);
			List<Job> listJobsAllDescendants=GetDescedentJobs(_jobCur.JobNum,false);
			listJobsAllDescendants.RemoveAll(x => x.PhaseCur.In(JobPhase.Complete,JobPhase.Cancelled));
			RemoveCompletedJobsFromListJobs(_listJobsByTopParent);
			double totalHoursEst=0;
			double totalHourActual=0;
			foreach(Job job in listJobsAllDescendants) {
				if(job.Category!=JobCategory.Project) {
					totalHourActual+=job.HoursActual;
					totalHoursEst+=job.HoursEstimate;
				}
			}
			textHoursEstDescendants.Text=totalHoursEst.ToString();
			textHoursActualDescendants.Text=totalHourActual.ToString();
			textHoursLeftDescendants.Text=(totalHoursEst-totalHourActual).ToString();
		}

		private void RefreshListJobsByTopParent(bool includeComplete,bool includeCancelled) {
			Jobs.RefreshInMemoryListByTopParent(_jobCur,includeComplete,includeCancelled);
			_listJobsByTopParent=JobManagerCore.ListJobsAll.FindAll(x => x.TopParentNum==_jobCur.TopParentNum);
			if(!includeComplete) {
				_listJobsByTopParent.RemoveAll(x => x.PhaseCur==JobPhase.Complete);
			}
			if(!includeCancelled) {
				_listJobsByTopParent.RemoveAll(x => x.PhaseCur==JobPhase.Cancelled);
			}
		}

		private void checkShowAllChildJobs_Click(object sender,EventArgs e) {
			FillAllChildGrids();
		}
		///<summary>Removes completed jobs from provided list if checkIncludeComplete is not checked. This is helpful for filtering out completed after getting descendents from completed jobs.</summary>
		private void RemoveCompletedJobsFromListJobs(List<Job> listJobs) {
			if(listJobs.IsNullOrEmpty()) {
				return;
			}
			if(!checkIncludeComplete.Checked) {
				listJobs.RemoveAll(x => x.PhaseCur==JobPhase.Complete);
			}
		}
	}//end class

}//end namespace
