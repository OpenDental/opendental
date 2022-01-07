using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Diagnostics;
using System.IO;

namespace OpenDental.InternalTools.Job_Manager {
	
	public partial class UserControlQueryEdit:UserControl {
		//FIELDS
		public bool IsNew;
		///<summary>Set this to true when the job is open outside of FormJobManager. Disables some functionality.</summary>
		public bool IsPopout;
		private Job _jobOld;
		private Job _jobCur;
		///<summary>Private member for IsChanged Property. Private setter, public getter.</summary>
		private bool _isChanged;
		private bool _isOverride;
		private bool _isLoading;
		///<summary>Passed in to LoadJob from job manager, used to display job family.</summary>
		private TreeNode _treeNode;

		///<summary>Occurs whenever this control saves changes to DB, after the control has redrawn itself. 
		/// Usually connected to either a form close or refresh.</summary>
		[Category("Action"),Description("Whenever this control saves changes to DB, after the control has redrawn itself. Usually connected to either a form close or refresh.")]
		public event EventHandler SaveClick=null;

		public delegate void RequestJobEvent(object sender,long jobNum);
		public event RequestJobEvent RequestJob=null;

		public delegate void UpdateTitleEvent(object sender,string title);
		public event UpdateTitleEvent TitleChanged=null;

		public delegate void JobOverrideEvent(object sender,bool isOverride);
		public event JobOverrideEvent JobOverride=null;
		private DataObject _dragObject=null;

		//PROPERTIES
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

		//FUNCTIONS
		public UserControlQueryEdit() {
			InitializeComponent();
			if(gridFiles.ContextMenu==null) {
				gridFiles.ContextMenu=new ContextMenu();
			}
			gridFiles.ContextMenu.Popup+=FilePopupHelper;
		}

		///<summary>Not a property so that this is compatible with the VS designer.</summary>
		public Job GetJob() {
			if(_jobCur==null) {
				return null;
			}
			Job job = _jobCur.Copy();
			job.Implementation=textEditorMain.MainRtf;
			return job;
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

		///<summary>Should only be called once when new job should be loaded into control. If called again, changes will be lost.</summary>
		public void LoadJob(Job job,TreeNode treeNode) {
			_isLoading=true;
			this.Enabled=false;//disable control while it is filled.
			_isOverride=false;
			_treeNode=treeNode;
			if(job==null) {
				_jobCur=new Job();
			}
			else {
				_jobCur=job.Copy();
				IsNew=job.IsNew;
			}
			_jobOld=_jobCur.Copy();//cannot be null
			try {
				textEditorMain.MainRtf=_jobCur.Implementation;//This is here to convert our old job descriptions to the new RTF descriptions.
			}
			catch {
				textEditorMain.MainText=_jobCur.Implementation;
			}
			//Query Jobs must have a quote attached on creation.
			JobQuote quote=_jobCur.ListJobQuotes.FirstOrDefault();
			Patient quotePatient=Patients.GetPat(quote.PatNum);
			textBillingType.Text=Defs.GetDef(DefCat.BillingTypes,quotePatient.BillingType).ItemName;
			text0_30.Text=quotePatient.Bal_0_30.ToString();
			text31_60.Text=quotePatient.Bal_31_60.ToString();
			text61_90.Text=quotePatient.Bal_61_90.ToString();
			textOver90.Text=quotePatient.BalOver90.ToString();
			textState.Text=quotePatient.State.ToString();
			textZip.Text=quotePatient.Zip.ToString();
			textJobNum.Text=_jobCur.JobNum>0?_jobCur.JobNum.ToString():Lan.g("Jobs","New Job");
			textDateEntry.Text=_jobCur.DateTimeEntry.Year>1880?_jobCur.DateTimeEntry.ToShortDateString():"";
			textTitle.Text=_jobCur.Title.ToString();
			textCustomer.Text=quotePatient.PatNum.ToString()+" - "+quotePatient.GetNameLF();
			comboPriority.Items.Clear();
			comboPriority.Items.AddDefs(Defs.GetDefsForCategory(DefCat.JobPriorities,true));
			comboPriority.SetSelectedDefNum(job.Priority); 
			comboPhase.Items.Clear();
			foreach(JobPhase phase in Enum.GetValues(typeof(JobPhase)).Cast<JobPhase>()) {
				comboPhase.Items.Add(phase.GetDescription(),phase);
				if(phase==job.PhaseCur) {
					comboPhase.SelectedIndex=comboPhase.Items.Count-1;
				}
			}
			Job parent=Jobs.GetOne(_jobCur.ParentNum);
			textParent.Text=parent!=null?parent.JobNum.ToString():"";
			textQuoteHours.Text=quote.Hours.ToString();
			textQuoteAmount.Text=quote.Amount.ToString();
			checkApproved.Checked=quote.IsCustomerApproved;
			textQuoteDate.Text=_jobCur.DateTimeCustContact.Year<1880?"":_jobCur.DateTimeCustContact.ToShortDateString();
			textSchedDate.Text=_jobCur.AckDateTime.Year<1880?"":_jobCur.AckDateTime.ToString();
			textEstHours.Text=_jobCur.HoursEstimateSingleReviewTime.ToString();
			textActualHours.Text=_jobCur.HoursActualSingleReviewTime.ToString();
			SetHoursLeft();
			FillAllGrids();
			IsChanged=false;
			CheckPermissions();
			DisableForPopout();
			if(job!=null) {//re-enable control after we have loaded the job.
				this.Enabled=true;
			}
			_isLoading=false;
		}		

		private void DisableForPopout() {
			if(!IsPopout) {
				return;
			}
			treeRelatedJobs.Enabled=false;
		}
		
		///<summary>When editing a job, and the job has been changed, this loads changes into the current control.</summary>
		public void LoadMergeJob(Job newJob) {
			_isLoading=true;
			Job jobMerge = newJob.Copy();//otherwise changes would be made to the tree view.
			//Set _jobCur lists to the new lists made above.
			_jobCur.ListJobLinks		=jobMerge.ListJobLinks;
			_jobCur.ListJobNotes		=jobMerge.ListJobNotes;
			_jobCur.ListJobQuotes		=jobMerge.ListJobQuotes;
			_jobCur.ListJobReviews	=jobMerge.ListJobReviews;
			_jobCur.ListJobTimeLogs	=jobMerge.ListJobTimeLogs;
			//Update Old lists too
			_jobOld.ListJobLinks		=jobMerge.ListJobLinks.Select(x=>x.Copy()).ToList();
			_jobOld.ListJobNotes		=jobMerge.ListJobNotes.Select(x => x.Copy()).ToList();
			_jobOld.ListJobQuotes		=jobMerge.ListJobQuotes.Select(x => x.Copy()).ToList();
			_jobOld.ListJobReviews	=jobMerge.ListJobReviews.Select(x => x.Copy()).ToList();
			_jobOld.ListJobTimeLogs	=jobMerge.ListJobTimeLogs.Select(x => x.Copy()).ToList();
			//JOB ROLE USER NUMS
			_jobCur.UserNumApproverChange=jobMerge.UserNumApproverChange;
			_jobCur.UserNumApproverConcept=jobMerge.UserNumApproverConcept;
			_jobCur.UserNumApproverJob=jobMerge.UserNumApproverJob;
			_jobCur.UserNumCheckout=jobMerge.UserNumCheckout;
			_jobCur.UserNumConcept=jobMerge.UserNumConcept;
			_jobCur.UserNumDocumenter=jobMerge.UserNumDocumenter;
			_jobCur.UserNumCustContact=jobMerge.UserNumCustContact;
			_jobCur.UserNumEngineer=jobMerge.UserNumEngineer;
			_jobCur.UserNumExpert=jobMerge.UserNumExpert;
			_jobCur.UserNumInfo=jobMerge.UserNumInfo;
			//old
			_jobOld.UserNumApproverChange=jobMerge.UserNumApproverChange;
			_jobOld.UserNumApproverConcept=jobMerge.UserNumApproverConcept;
			_jobOld.UserNumApproverJob=jobMerge.UserNumApproverJob;
			_jobOld.UserNumCheckout=jobMerge.UserNumCheckout;
			_jobOld.UserNumConcept=jobMerge.UserNumConcept;
			_jobOld.UserNumDocumenter=jobMerge.UserNumDocumenter;
			_jobOld.UserNumCustContact=jobMerge.UserNumCustContact;
			_jobOld.UserNumEngineer=jobMerge.UserNumEngineer;
			_jobOld.UserNumExpert=jobMerge.UserNumExpert;
			_jobOld.UserNumInfo=jobMerge.UserNumInfo;
			FillAllGrids();
			//All changes below this point will be lost if there is a conflicting chage detected.
			//TITLE
			if(_jobCur.Title!=jobMerge.Title) {
				if(_jobCur.Title==_jobOld.Title) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Title=jobMerge.Title;
					_jobOld.Title=jobMerge.Title;
					textTitle.Text=_jobCur.Title;
				}
				else {
					//MessageBox.Show("Job Title has been changed to:\r\n"+jobMerge.Title);
				}
			}
			//IMPLEMENTATION
			if(_jobCur.Implementation!=jobMerge.Implementation) {
				if(textEditorMain.MainRtf==_jobOld.Implementation) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Implementation=jobMerge.Implementation;
					_jobOld.Implementation=jobMerge.Implementation;
					try {
						textEditorMain.MainRtf=_jobCur.Implementation;
					}
					catch {
						textEditorMain.MainText=_jobCur.Implementation;
					}
				}
				else {
					//MessageBox.Show("Job Writeup has been changed.");
				}
			}
			//PRIORITY
			if(_jobCur.Priority!=jobMerge.Priority) {
				_jobCur.Priority=jobMerge.Priority;
				_jobOld.Priority=jobMerge.Priority;
				comboPriority.SetSelectedDefNum(_jobCur.Priority);  
			}
			//STATUS
			if(_jobCur.PhaseCur!=jobMerge.PhaseCur) {
				_jobCur.PhaseCur=jobMerge.PhaseCur;
				_jobOld.PhaseCur=jobMerge.PhaseCur;
				comboPhase.SelectedIndex=(int)_jobCur.PhaseCur;
			}
			checkApproved.Checked=_jobCur.ListJobQuotes.FirstOrDefault().IsCustomerApproved;
			//DATEENTRY - Cannot change
			_isLoading=false;
			CheckPermissions();
		}

		private void FillAllGrids() {
			FillGridRoles();
			FillTreeRelated();
			FillGridTasks();
			FillGridAppointments();
			FillGridFiles();
			FillGridNote();
			FillGridReviews();
		}

		#region FillGrids

		private void FillGridRoles() {
			gridRoles.Title="Query Roles";
			gridRoles.BeginUpdate();
			gridRoles.ListGridColumns.Clear();
			gridRoles.ListGridColumns.Add(new GridColumn("",90));
			gridRoles.ListGridColumns.Add(new GridColumn("User",50));
			gridRoles.NoteSpanStart=0;
			gridRoles.NoteSpanStop=1;
			gridRoles.ListGridRows.Clear();
			//These columns are ordered by convenience, If some other ordering would be more convenient then they should just be re-ordered.
			GridRow	gridRow=new GridRow();
			gridRow.Cells.Add("Created By:");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumConcept));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Owner:");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumEngineer));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Reviewer:");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumExpert));
			gridRoles.ListGridRows.Add(gridRow);
			gridRoles.EndUpdate();
		}

		private void FillTreeRelated() {
			labelRelatedJobs.Visible=!IsNew;
			treeRelatedJobs.Visible=!IsNew;
			treeRelatedJobs.Nodes.Clear();
			if(IsNew || _treeNode==null) {
				return;
			}
			//Color the current job grey
			List<TreeNode> listNodes = new List<TreeNode>();
			listNodes.Add(_treeNode);
			for(int i = 0;i<listNodes.Count;i++) {
				if(((Job)listNodes[i].Tag).JobNum==_jobCur.JobNum) {
					listNodes[i].BackColor=Color.LightGray;
				}
				else {
					listNodes[i].BackColor=Color.White;
				}
				listNodes.AddRange(listNodes[i].Nodes.Cast<TreeNode>());
			}
			treeRelatedJobs.Nodes.Add(_treeNode);
			treeRelatedJobs.ExpandAll();
			//Make sure that the currently selected tree node is visible (this will auto-scroll for us).
			treeRelatedJobs.SelectedNode=listNodes.FirstOrDefault(x => ((Job)x.Tag).JobNum==_jobCur.JobNum);
			if(treeRelatedJobs.SelectedNode!=null) {
				treeRelatedJobs.SelectedNode.EnsureVisible();
			}
		}

		private void FillGridTasks() {
			gridTasks.BeginUpdate();
			gridTasks.ListGridColumns.Clear();
			gridTasks.ListGridColumns.Add(new GridColumn("Date",70));
			gridTasks.ListGridColumns.Add(new GridColumn("TaskList",100));
			gridTasks.ListGridColumns.Add(new GridColumn("Done",40) { TextAlign=HorizontalAlignment.Center });
			gridTasks.NoteSpanStart=0;
			gridTasks.NoteSpanStop=2;
			gridTasks.ListGridRows.Clear();
			List<Task> listTasks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Task)
				.Select(x => Tasks.GetOne(x.FKey))
				.Where(x => x!=null)
				.OrderBy(x =>x.DateTimeEntry).ToList();
			foreach(Task task in listTasks){
				GridRow row=new GridRow() { Tag=task.TaskNum };//taskNum
				row.Cells.Add(task.DateTimeEntry.ToShortDateString());
				row.Cells.Add(TaskLists.GetOne(task.TaskListNum)?.Descript??"<TaskListNum:"+task.TaskListNum+">");
				row.Cells.Add(task.TaskStatus==TaskStatusEnum.Done?"X":"");
				row.Note=StringTools.Truncate(task.Descript,100,true).Trim();
				gridTasks.ListGridRows.Add(row);
			}
			gridTasks.EndUpdate();
		}

		private void FillGridAppointments() {
			gridAppointments.BeginUpdate();
			gridAppointments.ListGridColumns.Clear();
			gridAppointments.ListGridColumns.Add(new GridColumn("Appt Num",150));
			gridAppointments.ListGridRows.Clear();
			List<long> listApptNums=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Appointment).Select(x => x.FKey).ToList();
			foreach(long aptNum in listApptNums){
				GridRow row=new GridRow() { Tag=aptNum };
				row.Cells.Add(aptNum.ToString());
				gridAppointments.ListGridRows.Add(row);
			}
			gridAppointments.EndUpdate();
		}

		private void FillGridFiles() {
			gridFiles.BeginUpdate();
			gridFiles.ListGridColumns.Clear();
			gridFiles.ListGridColumns.Add(new GridColumn(Lan.g(this,""),120));
			gridFiles.ListGridRows.Clear();
			GridRow row;
			foreach(JobLink link in _jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.File)) {
				row=new GridRow();
				if(String.IsNullOrEmpty(link.DisplayOverride)) {
					row.Cells.Add(link.Tag.Split('\\').Last());
				}
				else {
					row.Cells.Add(link.DisplayOverride.ToString());
				}
				row.Tag=link;
				gridFiles.ListGridRows.Add(row);
			}
			gridFiles.EndUpdate();
		}
		#endregion

		///<summary>Just prior to displaying the context menu, add wiki links if neccesary.</summary>
		private void FilePopupHelper(object sender,EventArgs e) {
			ContextMenu menu = gridFiles.ContextMenu;
			//Always clear the options in the context menu because it could contain options for a previous row.
			menu.MenuItems.Clear();
			if(gridFiles.SelectedIndices.Length==0) {
				return;
			}
			JobLink link = _jobCur.ListJobLinks.FirstOrDefault(x => x.JobLinkNum==((JobLink)gridFiles.ListGridRows[gridFiles.SelectedIndices[0]].Tag).JobLinkNum);
			menu.MenuItems.Add("Override display name",(o,arg) => {
				using InputBox inputBox = new InputBox("Give a name override for the file");
				inputBox.textResult.Text=link.DisplayOverride;
				inputBox.textResult.SelectAll();
				if(inputBox.ShowDialog()==DialogResult.Cancel) {
					return;
				}
				link.DisplayOverride=inputBox.textResult.Text;
				JobLinks.Update(link);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				FillGridFiles();
			});
			menu.MenuItems.Add("Open File",(o,arg) => {
				try {
					System.Diagnostics.Process.Start(link.Tag);
				}
				catch(Exception ex) {
					ex.DoNothing();
					MessageBox.Show("Unable to open file.");
					try {
						System.Diagnostics.Process.Start(link.Tag.Substring(0,link.Tag.LastIndexOf('\\')));
					}
					catch(Exception exc) { exc.DoNothing(); }
				}
			});
			if(link.Tag.Contains("\\")) {
				try {
					string folder = link.Tag.Substring(0,link.Tag.LastIndexOf('\\'));
					menu.MenuItems.Add("Open Folder",(o,arg) => {
						try {
							System.Diagnostics.Process.Start(folder);
						}
						catch(Exception ex) { ex.DoNothing(); }
					});
				}
				catch(Exception ex) { ex.DoNothing(); }
			}
			menu.MenuItems.Add("-");
			menu.MenuItems.Add("Unlink File",(o,arg) => {
				List<JobLink> listLinks = _jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.File && x.Tag==link.Tag);
				_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.File && x.Tag==link.Tag);
				_jobOld.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.File && x.Tag==link.Tag);
				listLinks.Select(x => x.JobLinkNum).ToList().ForEach(JobLinks.Delete);
				FillGridFiles();
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			});
			menu.MenuItems.Add(new MenuItem(link.Tag) { Enabled=false });//show file path in gray
		}

		public void FillGridNote() {
			gridNotes.BeginUpdate();
			gridNotes.ListGridColumns.Clear();
			gridNotes.ListGridColumns.Add(new GridColumn(Lan.g(this,"Note"),400));
			gridNotes.ListGridRows.Clear();
			GridRow row;
			List<JobNote> listJobNotes=_jobCur.ListJobNotes.ToList();
			listJobNotes=listJobNotes.OrderByDescending(x => x.DateTimeNote).ToList();
			foreach(JobNote jobNote in listJobNotes) {
				row=new GridRow();
				row.Cells.Add(jobNote.DateTimeNote.ToShortDateString()+" "+jobNote.DateTimeNote.ToShortTimeString()+" - "+Userods.GetName(jobNote.UserNum)+" - "+jobNote.Note);
				row.Tag=jobNote;
				gridNotes.ListGridRows.Add(row);
			}
			gridNotes.EndUpdate();
		}

		private void FillGridReviews() {
			long selectedReviewNum=0;
			if(gridReview.GetSelectedIndex()!=-1 && (gridReview.ListGridRows[gridReview.GetSelectedIndex()].Tag is JobReview)) {
				selectedReviewNum=((JobReview)gridReview.ListGridRows[gridReview.GetSelectedIndex()].Tag).JobNum;
			}
			gridReview.BeginUpdate();
			gridReview.ListGridColumns.Clear();
			gridReview.ListGridColumns.Add(new GridColumn("Date Last Edited",100));
			gridReview.ListGridColumns.Add(new GridColumn("Reviewer",80));
			gridReview.ListGridColumns.Add(new GridColumn("Status",90));
			gridReview.ListGridColumns.Add(new GridColumn("Hours",80));
			gridReview.ListGridColumns.Add(new GridColumn("Description",200));
			gridReview.ListGridRows.Clear();
			GridRow row;
			foreach(JobReview jobReview in _jobCur.ListJobReviews) {
				row=new GridRow();
				row.Cells.Add(jobReview.DateTStamp.ToShortDateString());
				row.Cells.Add(Userods.GetName(jobReview.ReviewerNum));
				row.Cells.Add(Enum.GetName(typeof(JobReviewStatus),(int)jobReview.ReviewStatus));
				row.Cells.Add(Math.Round(jobReview.Hours,2).ToString());
				row.Cells.Add(StringTools.Truncate(jobReview.Description,500,true));
				row.Tag=jobReview;
				gridReview.ListGridRows.Add(row);
			}
			gridReview.EndUpdate();
			for(int i=0;i<gridReview.ListGridRows.Count;i++) {
				if(gridReview.ListGridRows[i].Tag is JobReview && ((JobReview)gridReview.ListGridRows[i].Tag).JobReviewNum==selectedReviewNum) {
					gridReview.SetSelected(i,true);
					break;
				}
			}
		}

		///<summary>Based on job status, category, and user role, this will enable or disable various controls.</summary>
		private void CheckPermissions() {
			//disable various controls and re-enable them below depending on permissions.
			textEditorMain.ReadOnly=true;
			comboPriority.Enabled=false;
			butParentPick.Visible=false;
			butParentRemove.Visible=false;
			if(_jobCur==null) {
				return;
			}
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
				case JobPhase.Quote:
				case JobPhase.Definition:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
						break;
					}
					comboPriority.Enabled=true;
					textEditorMain.ReadOnly=false;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Development:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true)) {
						break;
					}
					comboPriority.Enabled=true;
					textEditorMain.ReadOnly=false;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Documentation:
				case JobPhase.Complete:
					//Can only edit concept job if you meet one of the following
					//1) You have concept permission.
					//2) Concept needs approval and you have approval permission
					//textTitle.ReadOnly=false;
					//comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					//comboCategory.Enabled=true;
					//textVersion.ReadOnly=false;
					//textEstHours.Enabled=true;
					//textActualHours.Enabled=true;
					//butParentPick.Visible=true;
					//butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					//textEditorMain.ReadOnly=false;
					break;
				case JobPhase.Cancelled:
					//if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)) {
					//	break;
					//}
					//Can only edit concept job if you meet one of the following
					//1) You have concept permission.
					//2) Concept needs approval and you have approval permission
					//textTitle.ReadOnly=false;
					//comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					//comboCategory.Enabled=true;
					//textVersion.ReadOnly=false;
					//textEstHours.Enabled=true;
					//textActualHours.Enabled=true;
					//butParentPick.Visible=true;
					//butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					//textEditorMain.ReadOnly=false;
					break;
				default:
					MessageBox.Show("Unsupported job status. Add to UserControlJobEdit.CheckPermissions()");
					break;
			}
			//Disable description, documentation, and title if "Checked out"
			textEditorMain.Enabled=true;//might still be read only.
		}

		private void butActions_Click(object sender,EventArgs e) {
			bool perm=false;
			ContextMenu actionMenu=new System.Windows.Forms.ContextMenu();
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
				case JobPhase.Definition:
				case JobPhase.Quote:
					perm=JobPermissions.IsAuthorized(JobPerm.QueryTech,true);
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Creator",actionMenu_AssignSubmitterClick) { Enabled=perm });
					actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Owner",actionMenu_AssignEngineerClick) { Enabled=perm });//x
					actionMenu.MenuItems.Add(new MenuItem("Send to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });//x
					if(JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Reviewer",actionMenu_AssignExpertClick) { Enabled=true });//x
					}
					actionMenu.MenuItems.Add(new MenuItem("Send to Development",actionMenu_SendInDevelopmentClick) { Enabled=perm });//x
					bool hasCompleteReview = _jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done)&&perm;//Has QueryTech perm
					actionMenu.MenuItems.Add(new MenuItem("Mark as Complete",actionMenu_ImplementedClick) { Enabled=hasCompleteReview });
					if(perm) {
						actionMenu.MenuItems.Add("-");
						actionMenu.MenuItems.Add(new MenuItem("Cancel Query",actionMenu_CancelJobClick));
					}
					break;
				case JobPhase.Development:
					perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Creator",actionMenu_AssignSubmitterClick) { Enabled=perm });
					actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Owner",actionMenu_AssignEngineerClick) { Enabled=perm||JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true) });//x
					actionMenu.MenuItems.Add(new MenuItem("Send to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });//x
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Reviewer",actionMenu_AssignExpertClick) { Enabled=perm });//x
					actionMenu.MenuItems.Add(new MenuItem("Send to Development",actionMenu_SendInDevelopmentClick) { Enabled=perm });//x
					hasCompleteReview=_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done&&JobPermissions.IsAuthorized(JobPerm.QueryTech,true));//Has QueryTech perm
					actionMenu.MenuItems.Add(new MenuItem("Mark as Complete",actionMenu_ImplementedClick) { Enabled=hasCompleteReview });
					if(perm) {
						actionMenu.MenuItems.Add("-");
						actionMenu.MenuItems.Add(new MenuItem("Cancel Query",actionMenu_CancelJobClick));
					}
					break;
				case JobPhase.Complete:
					actionMenu.MenuItems.Add(new MenuItem("Completed Job") { Enabled=false });
					break;
				case JobPhase.Cancelled:
					perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
					//actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ApproveConceptClick) { Enabled=perm });//x
					break;
				default:
					actionMenu.MenuItems.Add(new MenuItem("Unhandled status "+_jobCur.PhaseCur.ToString(),(o,ea) => { }) { Enabled=false });
					break;
			}
			if(_jobCur.UserNumCheckout>0 && _jobCur.UserNumCheckout!=Security.CurUser.UserNum && !_isOverride) {
				//disable all menu items if job is checked out by other user.
				actionMenu.MenuItems.OfType<MenuItem>().ToList().ForEach(x => x.Enabled=false);
			}
			if(actionMenu.MenuItems.Count>0 && actionMenu.MenuItems[0].Text=="-") {
				actionMenu.MenuItems.RemoveAt(0);
			}
			if(actionMenu.MenuItems.Count==0) {
				actionMenu.MenuItems.Add(new MenuItem("No Actions Available"){Enabled=false});
			}
			actionMenu.MenuItems.Add("View Logs",actionMenu_ViewLogsClick);
			butActions.ContextMenu=actionMenu;
			butActions.ContextMenu.Show(butActions,new Point(0,butActions.Height));
		}

		private void butParentRemove_Click(object sender,EventArgs e) {
			_jobCur.ParentNum=0;
			_jobOld.ParentNum=0;
			textParent.Text="";
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job jobCur = Jobs.GetOne(_jobCur.JobNum);
				jobCur.ParentNum=0;
				Jobs.Update(jobCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
			}
		}

		private void butParentPick_Click(object sender,EventArgs e) {
			using InputBox inBox=new InputBox("Input parent job number.");
			inBox.ShowDialog();
			if(inBox.DialogResult!=DialogResult.OK) {
				return;
			}
			long parentNum=0;
			long.TryParse(new string(inBox.textResult.Text.Where(char.IsDigit).ToArray()),out parentNum);
			Job job=Jobs.GetOne(parentNum);
			if(job==null) {
				return;
			}
			if(Jobs.CheckForLoop(_jobCur.JobNum,parentNum)) {
				MsgBox.Show(this,"Invalid parent job, would create an infinite loop.");
				return;
			}
			_jobCur.ParentNum=job.JobNum;
			_jobOld.ParentNum=job.JobNum;
			textParent.Text=job.ToString();
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job jobCur = Jobs.GetOne(_jobCur.JobNum);
				jobCur.ParentNum=parentNum;
				Jobs.Update(jobCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
			}
		}

		///<summary>This should not implement any permissions, this should only check that the fields of the job are valid.</summary>
		/// <param name="_jobCur"></param>
		/// <returns></returns>
		private bool ValidateJob(Job _jobCur) {
			if(string.IsNullOrWhiteSpace(_jobCur.Title)) {
				MessageBox.Show("Invalid Title.");
				return false;
			}
			if(_jobCur.Category==JobCategory.Bug && _jobCur.ListJobLinks.Count(x => x.LinkType==JobLinkType.Bug)==0 && (_jobCur.PhaseCur!=JobPhase.Concept || _jobCur.IsApprovalNeeded)) {
				MsgBox.Show(this,"Bug jobs must have an attached bug.");
				return false;
			}
			return true;
		}

		#region ACTION BUTTON MENU ITEMS //====================================================
		#region Bug Actions
		private void actionMenu_SendDefinitionClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(!PickUserByJobPermission("Pick Owner",JobPerm.QueryCoordinator,out userNumEngineer,_jobCur.UserNumEngineer,JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true,Security.CurUser.UserNum),false)) {
				return;
			}
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.PhaseCur=JobPhase.Definition;
			SaveJob(_jobCur);
		}

		private void actionMenu_SendInDevelopmentClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Owner",JobPerm.QueryCoordinator,out userNumEngineer,_jobCur.UserNumEngineer>0 ? _jobCur.UserNumEngineer : _jobCur.UserNumConcept,true,false)) {
				return;
			}
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.PhaseCur=JobPhase.Development;
			SaveJob(_jobCur);
		}
		#endregion
		#region Assign Users

		private void actionMenu_AssignSubmitterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumConcept=_jobOld.UserNumConcept;
			if(!PickUserByJobPermission("Pick Creator",JobPerm.QueryTech,out userNumConcept,_jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(userNumConcept==_jobOld.UserNumConcept) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumConcept=userNumConcept;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignExpertClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert=_jobOld.UserNumExpert;
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.SeniorQueryCoordinator,out userNumExpert,_jobCur.UserNumExpert,true,false)) {
				return;
			}
			if(userNumExpert==_jobOld.UserNumExpert) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignEngineerClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumEngineer=_jobOld.UserNumEngineer;
			if(!PickUserByJobPermission("Pick Owner",JobPerm.QueryCoordinator,out userNumEngineer,_jobCur.UserNumEngineer,true,false)) {
				return;
			}
			if(userNumEngineer==_jobOld.UserNumEngineer) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumEngineer=userNumEngineer;
			SaveJob(_jobCur);
		}
		#endregion
		#region Request Approval/Reviews
		
		private void actionMenu_RepealApprovalRequestClick(object sender,EventArgs e) {
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			SaveJob(_jobCur);
		}

		private void actionMenu_UnapproveJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Concept;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverConcept=0;
			_jobCur.UserNumApproverJob=0;
			_jobCur.UserNumApproverChange=0;
			SaveJob(_jobCur);
		}
		#endregion
		#region Approval Options
		private void actionMenu_RequestQuoteClick(object sender,EventArgs e) {
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Quote;
			SaveJob(_jobCur);
		}

		private void actionMenu_CancelJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Cancelled;
			SaveJob(_jobCur);
		}
		#endregion

		private void actionMenu_ImplementedClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			//_jobCur.Priority=JobPriority.Normal;
			//comboPriority.SelectedIndex=_listJobPrioritiesSorted.IndexOf(_jobCur.Priority);
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Complete;
			SaveJob(_jobCur);
		}

		public void actionMenu_ViewLogsClick(object sender,EventArgs e) {
			FormJobLogs FormJL=new FormJobLogs(_jobCur);
			FormJL.Show();
		}

		#endregion ACTION BUTTON MENU ITEMS //=================================================

		///<summary>If returns false if selection is cancelled. DefaultUserNum is usually the currently set usernum for a given role.</summary>
		private bool PickUserByJobPermission(string prompt,JobPerm jobPerm,out long selectedNum, long suggestedUserNum = 0,bool AllowNone = true,bool AllowAll = true) {
			selectedNum=0;
			List<Userod> listUsersForPicker = Userods.GetUsersByJobRole(jobPerm,false);
			using FormUserPick FormUP = new FormUserPick();
			FormUP.Text=prompt;
			FormUP.IsSelectionmode=true;
			FormUP.ListUserodsFiltered=listUsersForPicker;
			FormUP.SuggestedUserNum=suggestedUserNum;
			FormUP.IsPickNoneAllowed=AllowNone;
			FormUP.IsShowAllAllowed=AllowAll;
			if(FormUP.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			selectedNum=FormUP.SelectedUserNum;
			return true;
		}

		///<summary>Job must have all in memory fields filled. Eg. Job.ListJobLinks, Job.ListJobNotes, etc. Also makes some of the JobLog entries.</summary>
		private void SaveJob(Job job) {
			_isLoading=true;
			//Validation must happen before this is called.
			job.Implementation=textEditorMain.MainRtf;
			//All other fields should have been maintained while editing the job in the form.
			job.UserNumCheckout=0;
			if(job.JobNum==0 || IsNew) {
				if(job.UserNumConcept==0 && JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
					job.UserNumConcept=Security.CurUser.UserNum;
				}
				Jobs.Insert(job);
				job.ListJobLinks.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobNotes.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobReviews.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobQuotes.ForEach(x=>x.JobNum=job.JobNum);
				//job.ListJobEvents.ForEach(x=>x.JobNum=job.JobNum);//do not sync
			}
			else {
				Jobs.Update(job);
			}
			JobLinks.Sync(job.ListJobLinks,job.JobNum);
			JobNotes.Sync(job.ListJobNotes,job.JobNum);
			JobReviews.SyncReviews(job.ListJobReviews,job.JobNum);
			JobQuotes.Sync(job.ListJobQuotes,job.JobNum);
			MakeLogEntry(job,_jobOld);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			JobManagerCore.UpdateForSingleJob(job);
			LoadJob(job,_treeNode);//Tree view may become out of date if viewing a job for an extended period of time.
			if(SaveClick!=null) {
				SaveClick(this,new EventArgs());
			}
			_isLoading=false;
		}
		
		private void textTitle_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			_jobCur.Title=textTitle.Text;
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void textCustomer_Click(object sender,EventArgs e) {
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			using FormPatientSelect FormPS=new FormPatientSelect();
			if(jobQuote.PatNum!=0) {
				FormPS.ExplicitPatNums=new List<long> {jobQuote.PatNum};
			}
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			Patient pat=Patients.GetPat(FormPS.SelectedPatNum);
			if(pat!=null) {
				jobQuote.PatNum=pat.PatNum;
			}
			else {
				jobQuote.PatNum=0;
			}
			textCustomer.Text=jobQuote.PatNum.ToString()+" - "+Patients.GetPat(jobQuote.PatNum).GetNameLF();
			textBillingType.Text=Defs.GetDef(DefCat.BillingTypes,pat.BillingType).ItemName;
			text0_30.Text=pat.Bal_0_30.ToString();
			text31_60.Text=pat.Bal_31_60.ToString();
			text61_90.Text=pat.Bal_61_90.ToString();
			textOver90.Text=pat.BalOver90.ToString();
			textState.Text=pat.State.ToString();
			textZip.Text=pat.Zip.ToString();
			JobQuotes.Update(jobQuote);
		}

		private void butPhoneNums_Click(object sender,EventArgs e) {
			JobQuote quote=_jobCur.ListJobQuotes.FirstOrDefault();
			using FormPhoneNumbersManage FormM=new FormPhoneNumbersManage();
			FormM.PatNum=quote.PatNum;
			FormM.ShowDialog();
		}

		private void butAddTime_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			if(!JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true)
				&& !JobPermissions.IsAuthorized(JobPerm.QueryTech,true)
				&& !JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) 
			{
				return;
			}
			AddTime();
		}

		private void butChangeEst_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			if(!JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true)
				&& !JobPermissions.IsAuthorized(JobPerm.QueryTech,true)
				&& !JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) 
			{
				return;
			}
			using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
			if(FormJE.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textEstHours.Text=_jobCur.HoursEstimateSingleReviewTime.ToString();
			Job jobFromDB = Jobs.GetOne(_jobCur.JobNum);//Get from DB to ensure freshest copy (Lists not filled)
			jobFromDB.HoursEstimateConcept=_jobCur.HoursEstimateConcept;
			jobFromDB.HoursEstimateWriteup=_jobCur.HoursEstimateWriteup;
			jobFromDB.HoursEstimateDevelopment=_jobCur.HoursEstimateDevelopment;
			jobFromDB.HoursEstimateReview=_jobCur.HoursEstimateReview;
			Jobs.Update(jobFromDB);//update the checkout num.
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
			SetHoursLeft();
		}		
		
		private bool AddTime() {
			if(_isLoading) {
				return false;
			}
			if(_jobCur==null) {
				return false;//should never happen
			}
			if(!JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true)
				&& !JobPermissions.IsAuthorized(JobPerm.QueryTech,true)
				&& !JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) 
			{
				return false;
			}
			JobReview timeLog=new JobReview();
			timeLog.JobNum=_jobCur.JobNum;
			using FormJobTimeAdd FormJTA=new FormJobTimeAdd(timeLog);
			FormJTA.ShowDialog();
			if(FormJTA.DialogResult!=DialogResult.OK || FormJTA.TimeLogCur==null) {
				return false;
			}
			JobReviews.Insert(FormJTA.TimeLogCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			_jobCur.ListJobTimeLogs.Add(FormJTA.TimeLogCur);
			textActualHours.Text=_jobCur.HoursActualSingleReviewTime.ToString();
			SetHoursLeft();
			return true;
		}	
		
		private void SetHoursLeft() {
			double hoursLeft=_jobCur.HoursEstimateSingleReviewTime-_jobCur.HoursActualSingleReviewTime;
			textHoursLeft.BackColor=SystemColors.Control;
			if(hoursLeft<0) {
				textHoursLeft.BackColor=Color.Salmon;
			}
			textHoursLeft.Text=(_jobCur.HoursEstimateSingleReviewTime-_jobCur.HoursActualSingleReviewTime).ToString();
		}

		private void butTimeLog_Click(object sender,EventArgs e) {
			FormJobTimeLog FormJTL=new FormJobTimeLog(_jobCur);
			FormJTL.Show(this);
		}

		private void comboPriority_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			long priorityNum=comboPriority.GetSelectedDefNum();
			_jobCur.Priority=priorityNum;
			JobLogs.MakeLogEntryForPriority(_jobCur,_jobOld);
			_jobOld.Priority=priorityNum;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.Priority=priorityNum;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboPhase_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			_jobCur.PhaseCur=comboPhase.GetSelected<JobPhase>();
			JobLogs.MakeLogEntryForPhase(_jobCur,_jobOld);
			_jobOld.PhaseCur=_jobCur.PhaseCur;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.PhaseCur=comboPhase.GetSelected<JobPhase>();
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void textQuoteHours_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			jobQuote.Hours=textQuoteHours.Text;
			JobQuotes.Update(jobQuote);
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void textQuoteAmount_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			jobQuote.Amount=textQuoteAmount.Text;
			JobQuotes.Update(jobQuote);
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void checkApproved_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			jobQuote.IsCustomerApproved=checkApproved.Checked;
			if(checkApproved.Checked) {
				_jobCur.DateTimeCustContact=DateTime.Now;
			}
			else {
				_jobCur.DateTimeCustContact=DateTime.MinValue;
			}
			textQuoteDate.Text=_jobCur.DateTimeCustContact.Year<1880?"":_jobCur.DateTimeCustContact.ToShortDateString();
			JobQuotes.Update(jobQuote);
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void textQuoteDate_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			DateTime date;
			if(!DateTime.TryParse(textQuoteDate.Text,out date)) {
				MsgBox.Show(this,"Not a valid date");
			}
			else {
				_jobCur.DateTimeCustContact=date;
			}
			textQuoteDate.Text=_jobCur.DateTimeCustContact.Year<1880?"":_jobCur.DateTimeCustContact.ToShortDateString();
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void textSchedDate_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			DateTime date;
			if(!DateTime.TryParse(textSchedDate.Text,out date)) {
				MsgBox.Show(this,"Not a valid date");
			}
			else {
				_jobCur.AckDateTime=date;
			}
			textSchedDate.Text=_jobCur.AckDateTime.Year<1880?"":_jobCur.AckDateTime.ToString();
			Jobs.Update(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void gridTasks_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			if(!IsNew && Control.ModifierKeys == Keys.Shift) {
				Task task = new Task() {
					TaskListNum=-1,//don't show it in any list yet.
					UserNum=Security.CurUser.UserNum
				};
				Tasks.Insert(task);
				FormTaskEdit FormTE = new FormTaskEdit(task);
				JobLink jl = new JobLink();
				jl.JobNum=_jobCur.JobNum;
				jl.FKey=task.TaskNum;
				jl.LinkType=JobLinkType.Task;
				FormTE.FormClosing+=(o,ea) => {
					if(FormTE.DialogResult!=DialogResult.OK) {
						return;
					}
					if(Tasks.GetOne(jl.FKey)==null) {
						return;
					}
					JobLinks.Insert(jl);
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jl.JobNum);
					if(this==null || this.IsDisposed) {
						return;//might be called after job manager has closed.
					}
					this.Invoke((Action)FillGridTasks);
				};
				FormTE.Show();
				return;
			}//end +Shift
			using FormTaskSearch FormTS=new FormTaskSearch() {IsSelectionMode=true};
			FormTS.ShowDialog();
			if(FormTS.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink=new JobLink();
			jobLink.JobNum=_jobCur.JobNum;
			jobLink.FKey=FormTS.SelectedTaskNum;
			jobLink.LinkType=JobLinkType.Task;
			if(!IsNew) {
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobLinks.Add(jobLink);
			FillGridTasks();
		}

		private void gridAppointments_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur.ListJobQuotes.FirstOrDefault().PatNum==0) {
				MsgBox.Show(this,"Customer must be attached to this query before attaching an appointment");
				return;
			}
			AddAppointment(_jobCur.ListJobQuotes.FirstOrDefault().PatNum);
		}

		private bool AddAppointment(long patNum) {
			if(_jobCur==null) {
				return false;//should never happen
			}
			using FormApptsOther FormAO=new FormApptsOther(patNum,null);//Select only, can't create new appt so don't need pinboard appointments.
			FormAO.AllowSelectOnly=true;
			if(FormAO.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			foreach(long aptNum in FormAO.ListAptNumsSelected) {
				JobLink jobLink = new JobLink();
				jobLink.JobNum=_jobCur.JobNum;
				jobLink.FKey=aptNum;
				jobLink.LinkType=JobLinkType.Appointment;
				if(!IsNew) {
					JobLinks.Insert(jobLink);
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				}
				else {
					IsChanged=true;
				}
				_jobCur.ListJobLinks.Add(jobLink);
			}
			FillGridAppointments();
			return true;
		}

		private void gridFiles_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			//Form to find file.
			OpenFileDialog formF = new OpenFileDialog();
			formF.Multiselect=true;
			if(formF.ShowDialog()!=DialogResult.OK) {
				return;
			}
			foreach(string filename in formF.FileNames) {
				JobLink jobLink = new JobLink();
				jobLink.JobNum=_jobCur.JobNum;
				jobLink.LinkType=JobLinkType.File;
				jobLink.Tag=filename;
				_jobCur.ListJobLinks.Add(jobLink);
				if(!IsNew) {
					JobLinks.Insert(jobLink);
				}
			}
			if(!IsNew) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			FillGridFiles();
		}

		private void gridNotes_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			JobNote jobNote=new JobNote() {
				DateTimeNote=MiscData.GetNowDateTime(),
				IsNew=true,
				JobNum=_jobCur.JobNum,
				UserNum=Security.CurUser.UserNum
			};
			using FormJobNoteEdit FormJNE=new FormJobNoteEdit(jobNote);
			FormJNE.ShowDialog();
			if(FormJNE.DialogResult!=DialogResult.OK || FormJNE.JobNoteCur==null) {
				return;
			}
			if(!IsNew) {
				JobNotes.Insert(FormJNE.JobNoteCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobNotes.Add(FormJNE.JobNoteCur);
			FillGridNote();
		}

		private void gridReview_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			long userNumReviewer=0;
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.SeniorQueryCoordinator,out userNumReviewer,_jobCur.UserNumExpert,false,false)) {
				return;
			}
			using FormJobReviewEdit FormJRE=new FormJobReviewEdit(new JobReview { ReviewerNum=userNumReviewer,JobNum=_jobCur.JobNum,IsNew=true });
			FormJRE.ShowDialog();
			if(FormJRE.DialogResult!=DialogResult.OK || FormJRE.JobReviewCur==null) {
				return;
			}
			if(!IsNew) {
				JobReviews.Insert(FormJRE.JobReviewCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobReviews.Add(FormJRE.JobReviewCur);
			_jobOld.ListJobReviews.Add(FormJRE.JobReviewCur);
			FillGridReviews();
		}

		private void gridTasks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridTasks.ListGridRows[e.Row].Tag is long)) {
				return;//should never happen
			}
			//GoTo patient will not work from this form. It would require a delegate to be passed in all the way from FormOpenDental.
			Task task=Tasks.GetOne((long)gridTasks.ListGridRows[e.Row].Tag);
			FormTaskEdit FormTE=new FormTaskEdit(task);
			FormTE.Show();
			FormTE.FormClosing+=(o,ea) => {
				if(FormTE.DialogResult!=DialogResult.OK) {
					return;
				}
				if(this==null || this.IsDisposed) {
					return;
				}
				try { this.Invoke((Action)FillGridTasks); } catch(Exception) { }//If form disposed, this will catch.
			};
		}

		///<summary>SaveClick for both the Descritpion and Documentation</summary>
		private void textEditor_SaveClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!ValidateJob(_jobCur)) {
				return;
			}
			SaveJob(_jobCur);
		}

		private void gridAppointments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridAppointments.ListGridRows[e.Row].Tag is long)) {
				return;//should never happen.
			}
			try {
				FormApptEdit FormAE = new FormApptEdit((long)gridAppointments.ListGridRows[e.Row].Tag);
				FormAE.Show();
				FormAE.FormClosing+=(o,ea) => {
					if(FormAE.DialogResult!=DialogResult.OK) {
						return;
					}
					if(this==null || this.IsDisposed) {
						return;
					}
					try {
						this.Invoke((Action)FillGridAppointments);
					}
					catch(Exception) { }//If form disposed, this will catch.
				};
			}
			catch {
				MsgBox.Show(this,"Appointment is most likely deleted. Please unlink this appointment from the job.");
			}
		}

		private void gridNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			JobNote jobNote = (JobNote)gridNotes.ListGridRows[e.Row].Tag;
			JobNote jobNoteOld=jobNote.Copy();
			using FormJobNoteEdit FormJNE=new FormJobNoteEdit(jobNote);
			FormJNE.ShowDialog();
			if(FormJNE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(jobNote.NoteType==JobNoteTypes.Discussion) {
				MakeLogEntryForNote(FormJNE.JobNoteCur,jobNoteOld);
			}
			if(!(gridNotes.ListGridRows[e.Row].Tag is JobNote)) {
				return;//should never happen.
			}
			if(IsNew) {
				IsChanged=true;
			}
			else {
				if(FormJNE.JobNoteCur==null) {
					JobNotes.Delete(jobNote.JobNoteNum);
				}
				else {
					JobNotes.Update(FormJNE.JobNoteCur);
				}
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			_jobCur.ListJobNotes.RemoveAll(x => x.JobNoteNum==jobNote.JobNoteNum);//should remove only one
			if(FormJNE.JobNoteCur!=null) {
				_jobCur.ListJobNotes.Add(FormJNE.JobNoteCur);
			}
			FillGridNote();
		}

		private void MakeLogEntryForNote(JobNote jobNoteNew,JobNote jobNoteOld) {
			JobLogs.MakeLogEntryForNote(_jobCur,jobNoteNew,jobNoteOld);
		}

		private void gridReview_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridReview.ListGridRows[e.Row].Tag is JobReview)) {
				return;//should never happen.
			}
			JobReview jobReview=(JobReview)gridReview.ListGridRows[e.Row].Tag;
			using FormJobReviewEdit FormJRE=new FormJobReviewEdit(jobReview);
			FormJRE.ShowDialog();
			if(FormJRE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(IsNew) {
				IsChanged=true;
			}
			else {
				if(FormJRE.JobReviewCur==null) {
					JobReviews.Delete(jobReview.JobReviewNum);
				}
				else {
					JobReviews.Update(FormJRE.JobReviewCur);
				}
			}
			_jobCur.ListJobReviews.RemoveAt(e.Row);
			_jobOld.ListJobReviews.RemoveAt(e.Row);
			if(FormJRE.JobReviewCur!=null) {
				_jobCur.ListJobReviews.Add(FormJRE.JobReviewCur);
				_jobOld.ListJobReviews.Add(FormJRE.JobReviewCur);
			}
			FillGridReviews();
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void gridTasks_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(e.Button==MouseButtons.Right && (gridTasks.ListGridRows[e.Row].Tag is long)) {
				ContextMenu menu = new ContextMenu();
				long FKey = (long)gridTasks.ListGridRows[e.Row].Tag;
				menu.MenuItems.Add("Unlink Task",(o,arg) => {
					List<JobLink> listLinks = _jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Task&&x.FKey==FKey);
					_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Task&&x.FKey==FKey);
					_jobOld.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Task&&x.FKey==FKey);
					listLinks.Select(x => x.JobLinkNum).ToList().ForEach(JobLinks.Delete);
					FillGridTasks();
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				});
				menu.Show(gridTasks,gridTasks.PointToClient(Cursor.Position));
			}
		}

		private void gridAppointments_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(e.Button==MouseButtons.Right && (gridAppointments.ListGridRows[e.Row].Tag is long)) {
				ContextMenu menu = new ContextMenu();
				long FKey = (long)gridAppointments.ListGridRows[e.Row].Tag;
				menu.MenuItems.Add("Unlink Appointment : "+FKey.ToString(),(o,arg) => {
					List<JobLink> listLinks = _jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Appointment&&x.FKey==FKey);
					_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Appointment&&x.FKey==FKey);
					_jobOld.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Appointment&&x.FKey==FKey);
					listLinks.Select(x => x.JobLinkNum).ToList().ForEach(JobLinks.Delete);
					FillGridAppointments();
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				});
				menu.Show(gridAppointments,gridAppointments.PointToClient(Cursor.Position));
			}
		}

		private void gridFiles_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridFiles.ListGridRows[e.Row].Tag is JobLink)) {
				return;
			}
			try {
				System.Diagnostics.Process.Start(((JobLink)gridFiles.ListGridRows[e.Row].Tag).Tag);
			}
			catch(Exception ex) {
				ex.DoNothing();
				MessageBox.Show("Unable to open file.");
			}
		}

		private void treeRelatedJobs_AfterSelect(object sender,TreeViewEventArgs e) {
			treeRelatedJobs.SelectedNode=null;
		}

		private void treeRelatedJobs_NodeMouseClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(IsNew || !(e.Node.Tag is Job)) {
				return;
			}
			if(e.Button==MouseButtons.Right && ((Job)e.Node.Tag is Job)) {
				ContextMenu menu=new ContextMenu();
				menu.MenuItems.Add("Open Job",(o,arg) => {
					FormJobManager.OpenNonModalJob((Job)e.Node.Tag);
				});
				menu.Show(treeRelatedJobs,treeRelatedJobs.PointToClient(Cursor.Position));
				return;
			}
			if(RequestJob!=null) {
				RequestJob(this,((Job)e.Node.Tag).JobNum);
			}
		}

		///<summary>Adds error checking to the input parameters for JobLogs.MakeLogEntry also inserts joblog into the UI and _jobCur.ListJobLog if returned.</summary>
		private void MakeLogEntry(Job jobNew,Job jobOld) {
			JobLogs.MakeLogEntry(jobNew,jobOld);
		}

		///<summary>Does nothing, just used to make those annoying compile warnings go away.</summary>
		private void DoNothing() {
			var x = new object[] { TitleChanged,JobOverride };//simplest way to get rid of the "variable assigned but not used" warning.
		}
		
		///<summary>This is fired whenever a change is made to the textboxes: Concept, Writeup, Documentation.</summary>
		private void textEditorMain_OnTextEdited() {
			if(_isLoading) {
				return;
			}
			IsChanged=true;
			if(IsNew) {
				//do nothing
			}
			else {
				if(!_isLoading && _jobCur.UserNumCheckout==0) {
					_jobCur.UserNumCheckout=Security.CurUser.UserNum;
					Job jobFromDB = Jobs.GetOne(_jobCur.JobNum);//Get from DB to ensure freshest copy (Lists not filled)
					jobFromDB.UserNumCheckout=Security.CurUser.UserNum;//change only the userNumCheckout.
					Jobs.Update(jobFromDB);//update the checkout num.
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
				}
			}
		}

		private void butCommlog_Click(object sender,EventArgs e) {
			using FormCommItem formCommItem=new FormCommItem(GetNewCommlog());
			formCommItem.ShowDialog();
		}

		///<summary>This is a helper method to get a new commlog object for the commlog tool bar buttons.</summary>
		private Commlog GetNewCommlog() {
			Commlog commlog=new Commlog();
			commlog.PatNum=_jobCur.ListJobQuotes.FirstOrDefault().PatNum;
			commlog.CommDateTime=DateTime.Now;
			commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			if(PrefC.GetBool(PrefName.DistributorKey)) {//for OD HQ
				commlog.Mode_=CommItemMode.None;
				commlog.SentOrReceived=CommSentOrReceived.Neither;
			}
			else {
				commlog.Mode_=CommItemMode.Phone;
				commlog.SentOrReceived=CommSentOrReceived.Received;
			}
			commlog.UserNum=Security.CurUser.UserNum;
			commlog.IsNew=true;
			return commlog;
		}

		private void butEmail_Click(object sender,EventArgs e) {
			EmailMessage message=new EmailMessage();
			message.IsNew=true;
			Patient pat=Patients.GetPat(_jobCur.ListJobQuotes.FirstOrDefault().PatNum);
			message.PatNum=pat.PatNum;
			message.ToAddress=pat.Email;
			message.FromAddress=EmailAddresses.GetOne(PrefC.GetLong(PrefName.EmailDefaultAddressNum)).GetFrom();				
			message.Subject="";
			message.MsgType=EmailMessageSource.JobManager;
			using FormEmailMessageEdit FormEME=new FormEmailMessageEdit(message);
			FormEME.IsNew=true;
			FormEME.ShowDialog();
			if(FormEME.DialogResult!=DialogResult.OK) {
				return;
			}
		}

		private void gridFiles_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left || gridFiles.ListGridRows.Count==0 || gridFiles.SelectedIndices.Count()==0) {
				return;
			}
			if(_dragObject==null) {
				JobLink link = (JobLink)gridFiles.ListGridRows[gridFiles.GetSelectedIndex()].Tag;
				if(File.Exists(link.Tag)) {
					string[] fileList=new string[] { link.Tag };
					_dragObject = new DataObject(DataFormats.FileDrop,fileList);
				}
				else {
					_dragObject=null;
					return;
				}
				DoDragDrop(_dragObject,DragDropEffects.Copy);
				_dragObject=null;
			}
		}
	}//end class

}//end namespace
	

