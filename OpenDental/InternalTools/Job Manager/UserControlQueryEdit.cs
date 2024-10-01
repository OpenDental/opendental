using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.IO;
using Newtonsoft.Json;

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
		private List<Def> _listPriorities;

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
			gridFiles.ContextMenu??=new ContextMenu();
			gridFiles.ContextMenu.Popup+=FilePopupHelper;
		}

		///<summary>Not a property so that this is compatible with the VS designer.</summary>
		public Job GetJob() {
			if(_jobCur==null) {
				return null;
			}
			Job job = _jobCur.Copy();
			job.Requirements=textJobEditor.ConceptRtf;
			job.Implementation=textJobEditor.WriteupRtf;
			job.RequirementsJSON=JsonConvert.SerializeObject(textJobEditor.GetListJobRequirements());
			if(comboPriority.SelectedIndex>-1) {
				job.Priority=_listPriorities[comboPriority.SelectedIndex].DefNum;
			}
			job.PhaseCur=(JobPhase)comboPhase.SelectedIndex;
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
		public void LoadJob(Job job,TreeNode treeNode=null) {
			_isLoading=true;
			this.Enabled=false;//disable control while it is filled.
			_isOverride=false;
			if(treeNode==null) {
				treeNode=Jobs.GetJobTreeTop(job,checkIncludeCompleteCanceled.Checked);
			}
			_treeNode=treeNode;
			if(job==null) {
				_jobCur=new Job();
			}
			else {
				_jobCur=job.Copy();
				IsNew=job.IsNew;
			}
			//If TopParentNum is 0, this job and it's entire tree need to be updated to the new system
			if(job!=null&&job.TopParentNum==0) {
				List<Job> listJobTreeOld = new List<Job>();
				List<Job> listJobsAll = Jobs.GetAll();
				Job topParent = Jobs.GetTopParentByParent(job,listJobsAll);
				listJobTreeOld=Jobs.GetChildJobs(topParent,listJobsAll);
				listJobTreeOld.Add(topParent);
				Jobs.UpdateTopParentList(topParent.JobNum,listJobTreeOld.Select(x => x.JobNum).ToList());
				JobManagerCore.AddTreeJobsToList(listJobTreeOld);
				_treeNode=Jobs.GetJobTreeTop(job,checkIncludeCompleteCanceled.Checked);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			}
			_jobOld=_jobCur.Copy();//cannot be null
			try {
				textJobEditor.ConceptRtf=_jobCur.Requirements;//This is here to convert our old job descriptions to the new RTF descriptions.
			}
			catch {
				textJobEditor.ConceptText=_jobCur.Requirements;
			}
			try {
				textJobEditor.WriteupRtf=_jobCur.Implementation;//This is here to convert our old job descriptions to the new RTF descriptions.
			}
			catch {
				textJobEditor.WriteupText=_jobCur.Implementation;
			}
			textJobEditor.ConceptTitle=_jobCur.Title;
			//Query Jobs must have a quote attached on creation to prevent this from throwing.
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
			_listPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList();
			comboPriority.Items.Clear();
			_listPriorities.ForEach(x => comboPriority.Items.Add(x.ItemName));
			comboPriority.SelectedIndex=_listPriorities.FirstOrDefault(x => x.DefNum==_jobCur.Priority)?.ItemOrder??0;
			comboPhase.Items.Clear();
			foreach(JobPhase phase in Enum.GetValues(typeof(JobPhase)).Cast<JobPhase>()) {
				comboPhase.Items.Add(phase.GetDescription(),phase);
				if(phase==job.PhaseCur) {
					comboPhase.SelectedIndex=comboPhase.Items.Count-1;
				}
			}
			checkIsActive.Checked=job.ListJobActiveLinks.Exists(x => x.UserNum==Security.CurUser.UserNum && x.DateTimeEnd==DateTime.MinValue);
			checkIsActive.Enabled=!job.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation) && JobPermissions.IsAuthorized(JobPerm.Concept,true);
			Job parent=Jobs.GetOne(_jobCur.ParentNum);
			textParent.Text=parent!=null?parent.JobNum.ToString():"";
			textQuoteHours.Text=quote.Hours.ToString();
			textQuoteAmount.Text=PIn.Float(quote.ApprovedAmount??"")==0 ? quote.Amount : quote.ApprovedAmount;
			checkApproved.Checked=quote.IsCustomerApproved;
			DatePickerApproved.SetDateTime(_jobCur.DateTimeCustContact);
			DatePickerSched.SetDateTime(_jobCur.AckDateTime);
			textEstHours.Text=_jobCur.HoursEstimateSingleReviewTime.ToString();
			textActualHours.Text=_jobCur.HoursActualSingleReviewTime.ToString();
			DatePickerDeadline.SetDateTime(_jobCur.DateTimeTested);
			SetHoursLeft();
			FillAllGrids();
			IsChanged=false;
			CheckPermissions();
			try {
				textJobEditor.SetListJobRequirements(JsonConvert.DeserializeObject<List<JobRequirement>>(_jobCur.RequirementsJSON));
			}
			catch {
				List<JobRequirement> listJobReqs=new List<JobRequirement>();
			}
			if(textJobEditor.GetListJobRequirements().Count==0) {
				List<JobRequirement> listJobReqs=new List<JobRequirement>();
				listJobReqs.Add(MakeNewJobRequirement("Expedited?: NO"));
				listJobReqs.Add(MakeNewJobRequirement("Complexity: "));
				textJobEditor.SetListJobRequirements(listJobReqs);
			}
			textJobEditor.ResizeTextFields();
			if(job!=null) {//re-enable control after we have loaded the job.
				JobNotifications.DeleteForJobAndUser(job.JobNum,Security.CurUser.UserNum);
				Signalods.SetInvalid(InvalidType.Jobs, KeyType.Job,job.JobNum);
				Enabled=true;
			}
			DisableForPopout();
			if(job!=null) {//re-enable control after we have loaded the job.
				this.Enabled=true;
			}
			_isLoading=false;
		}

		private JobRequirement MakeNewJobRequirement(string description) {
			JobRequirement jobReq = new JobRequirement();
			jobReq.HasEngineer=false;
			jobReq.HasExpert=false;
			jobReq.HasReviewer=false;
			jobReq.Description=description;
			return jobReq;
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
			//REQUIREMENTS
			if(_jobCur.Requirements!=jobMerge.Requirements) {
				if(textJobEditor.ConceptRtf==_jobOld.Requirements) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Requirements=jobMerge.Requirements;
					_jobOld.Requirements=jobMerge.Requirements;
					try {
						textJobEditor.ConceptRtf=_jobCur.Requirements;
					}
					catch {
						textJobEditor.ConceptText=_jobCur.Requirements;
					}
				}
				else {
					//MessageBox.Show("Job Concept has been changed.");
				}
			}
			//REQUIREMENTS JSON
			if(_jobCur.RequirementsJSON!=jobMerge.RequirementsJSON) {
				if(JsonConvert.SerializeObject(textJobEditor.GetListJobRequirements())==_jobOld.RequirementsJSON) {//Was edited, AND user has not already edited it themselves.
					_jobCur.RequirementsJSON=jobMerge.RequirementsJSON;
					_jobOld.RequirementsJSON=jobMerge.RequirementsJSON;
					try {
						textJobEditor.SetListJobRequirements(JsonConvert.DeserializeObject<List<JobRequirement>>(_jobCur.RequirementsJSON));
					}
					catch {
						//Change nothing
					}
				}
			}
			//IMPLEMENTATION
			if(_jobCur.Implementation!=jobMerge.Implementation) {
				if(textJobEditor.WriteupRtf==_jobOld.Implementation) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Implementation=jobMerge.Implementation;
					_jobOld.Implementation=jobMerge.Implementation;
					try {
						textJobEditor.WriteupRtf=_jobCur.Implementation;
					}
					catch {
						textJobEditor.WriteupText=_jobCur.Implementation;
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
				comboPriority.SelectedIndex=_listPriorities.FirstOrDefault(x => x.DefNum==_jobCur.Priority)?.ItemOrder??0;  
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
			textJobEditor.RefreshSpellCheckConcept();
			textJobEditor.RefreshSpellCheckWriteup();
		}

		#region FillGrids

		private void FillAllGrids() {
			FillGridRoles();
			FillTreeRelated();
			FillGridTasks();
			FillGridAppointments();
			FillGridFiles();
			FillGridNote();
			FillGridReviews();
			FillGridQuotes();
		}

		private void FillGridRoles() {
			gridRoles.Title="Query Roles";
			gridRoles.BeginUpdate();
			gridRoles.Columns.Clear();
			gridRoles.Columns.Add(new GridColumn("",126));
			gridRoles.Columns.Add(new GridColumn("User",50));
			gridRoles.NoteSpanStart=0;
			gridRoles.NoteSpanStop=1;
			gridRoles.ListGridRows.Clear();
			//These rows are ordered by convenience, If some other ordering would be more convenient then they should just be re-ordered.
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Query Owner",_jobCur.OwnerNumForQuery));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Definer/Expert",_jobCur.UserNumExpert));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Writer/Engineer",_jobCur.UserNumEngineer));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Submitter",_jobCur.UserNumConcept));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Quoter/Estimater",_jobCur.UserNumQuoter));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Apprv Quote/Estimate",_jobCur.UserNumCustContact));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Reviewer",_jobCur.UserNumTester));
			gridRoles.ListGridRows.Add(CreateGridRowForRole("Documenter",_jobCur.UserNumDocumenter));
			gridRoles.EndUpdate();
		}

		private GridRow CreateGridRowForRole(string title, long userNum) {
			GridRow gridRow=new GridRow();
			gridRow.Cells.Add(title+":");
			gridRow.Cells.Add(Userods.GetName(userNum));
			return gridRow;
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
			gridTasks.Columns.Clear();
			gridTasks.Columns.Add(new GridColumn("Date",70));
			gridTasks.Columns.Add(new GridColumn("TaskList",100));
			gridTasks.Columns.Add(new GridColumn("Done",40) { TextAlign=HorizontalAlignment.Center });
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
			gridAppointments.Columns.Clear();
			gridAppointments.Columns.Add(new GridColumn("Appt Num",150));
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
			gridFiles.Columns.Clear();
			gridFiles.Columns.Add(new GridColumn(Lan.g(this,""),120));
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
				InputBoxParam inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.TextBox;
				inputBoxParam.LabelText="Give a name override for the file";
				inputBoxParam.Text=link.DisplayOverride;
				InputBox inputBox=new InputBox(inputBoxParam);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return;
				}
				link.DisplayOverride=inputBox.StringResult;
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
			gridNotes.Columns.Clear();
			gridNotes.Columns.Add(new GridColumn(Lan.g(this,"Note"),400));
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
			gridReview.Columns.Clear();
			gridReview.Columns.Add(new GridColumn("Date Last Edited",100));
			gridReview.Columns.Add(new GridColumn("Reviewer",80));
			gridReview.Columns.Add(new GridColumn("Status",90));
			gridReview.Columns.Add(new GridColumn("Hours",80));
			gridReview.Columns.Add(new GridColumn("Description",200));
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

		private void FillGridQuotes() {
			gridQuotes.BeginUpdate();
			gridQuotes.Columns.Clear();
			gridQuotes.Columns.Add(new GridColumn("PatNum",50));
			gridQuotes.Columns.Add(new GridColumn("Hours",40,HorizontalAlignment.Center));
			gridQuotes.Columns.Add(new GridColumn("Amt",60,HorizontalAlignment.Right));
			gridQuotes.Columns.Add(new GridColumn("Appr?",50,HorizontalAlignment.Center));
			gridQuotes.NoteSpanStart=0;
			gridQuotes.NoteSpanStop=3;
			gridQuotes.ListGridRows.Clear();
			foreach(JobQuote jobQuote in _jobCur.ListJobQuotes) {
				GridRow row=new GridRow() { Tag=jobQuote };//JobQuote
				row.Cells.Add(jobQuote.PatNum.ToString());
				row.Cells.Add(jobQuote.Hours);
				row.Cells.Add(jobQuote.ApprovedAmount=="0.00" ? jobQuote.Amount : jobQuote.ApprovedAmount);
				row.Cells.Add(jobQuote.IsCustomerApproved ? "X" : "");
				row.Note=jobQuote.Note;
				gridQuotes.ListGridRows.Add(row);
			}
			gridQuotes.EndUpdate();
		}

		#endregion FillGrids

		///<summary>Based on job status, category, and user role, this will enable or disable various controls.</summary>
		private void CheckPermissions() {
			//disable various controls and re-enable them below depending on permissions.
			textJobEditor.ReadOnlyWriteup=true;
			comboPriority.Enabled=false;
			butParentPick.Visible=false;
			butParentRemove.Visible=false;
			gridQuotes.HasAddButton=JobPermissions.IsAuthorized(JobPerm.Quote,true)
				&& _jobOld?.PhaseCur!=JobPhase.Complete && _jobOld?.PhaseCur!=JobPhase.Cancelled;
			if(_jobCur==null) {
				return;
			}
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
						break;
					}
					textJobEditor.ReadOnlyWriteup=false;
					comboPriority.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Definition:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
						break;
					}
					textJobEditor.ReadOnlyWriteup=false;
					comboPriority.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Quote:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
						break;
					}
					textJobEditor.ReadOnlyWriteup=false;
					comboPriority.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Development:
					if(!JobPermissions.IsAuthorized(JobPerm.QueryTech,true)) {
						break;
					}
					comboPriority.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					break;
				case JobPhase.Documentation:
				case JobPhase.Complete:
				case JobPhase.Cancelled:
					break;
				default:
					MessageBox.Show("Unsupported job status. Add to UserControlJobEdit.CheckPermissions()");
					break;
			}
			//Disable description, documentation, and title if "Checked out"
			textJobEditor.Enabled=true;//might still be read only.
		}

		private void checkIncludeCompleteCanceled_CheckedChanged(object sender,EventArgs e) {
			_treeNode=Jobs.GetJobTreeTop(_jobCur,checkIncludeCompleteCanceled.Checked);
			FillTreeRelated();
		}

		#region ActionMenu

		private void butActions_Click(object sender,EventArgs e) {
			ContextMenu actionMenu=CreateActionMenu();
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

		private ContextMenu CreateActionMenu() {
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
					return GetConceptActions();
				case JobPhase.Definition:
					return GetDefinitionActions();
				case JobPhase.Quote:
					return GetQuoteActions();
				case JobPhase.Development:
					return GetDevelopmentActions();
				case JobPhase.Documentation:
					return GetDocumentationActions();
				case JobPhase.Complete:
					return GetCompleteActions();
				case JobPhase.Cancelled:
					return GetCancelledActions();
				default:
					ContextMenu actionMenu=new ContextMenu();
					actionMenu.MenuItems.Add(new MenuItem("Unhandled status "+_jobCur.PhaseCur.ToString(),(o,ea) => { }) { Enabled=false });
					return actionMenu;
			}
		}

		private ContextMenu GetConceptActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.QueryTech,true);
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
			AddCompleteAndCancelled(actionMenu,perm);
			return actionMenu;
		}

		private ContextMenu GetDefinitionActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.QueryTech,true);
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=perm });
			if(JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) {
				actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
			}
			actionMenu.MenuItems.Add(new MenuItem("Send back to Concept",actionMenu_SendToConcept_Click) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });
			AddCompleteAndCancelled(actionMenu,perm);
			return actionMenu;
		}

		private ContextMenu GetQuoteActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.QueryTech,true);
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=perm });
			if(JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) {
				actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
			}
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumQuoter==0 ? "A" : "Rea")+"ssign Quoter",actionMenu_AssignQuoterClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send back to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Development",actionMenu_SendInDevelopmentClick) { Enabled=perm });
			bool hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
			if(_jobCur.IsApprovalNeeded) {
				actionMenu.MenuItems.Add(new MenuItem("Mark as In-Review",actionMenu_AssignReviewerClick) { Enabled=perm });
			}
			else {
				actionMenu.MenuItems.Add(new MenuItem("Send to Needs Review",actionMenu_RequestReviewerClick) { Enabled=perm });
			}
			AddCompleteAndCancelled(actionMenu,perm);
			return actionMenu;
		}

		private ContextMenu GetDevelopmentActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=perm });
			if(JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true)) {
				actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
			}
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=perm||JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true) });
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumTester==0 ? "A" : "Rea")+"ssign Reviewer",actionMenu_AssignReviewerClick) { Enabled=perm||JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true) });
			actionMenu.MenuItems.Add(new MenuItem("Send back to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Documentation",actionMenu_AssignDocumenterClick) { Enabled=perm });
			if(_jobCur.IsApprovalNeeded) {
				actionMenu.MenuItems.Add(new MenuItem("Mark as In-Review",actionMenu_AssignReviewerClick) { Enabled=perm });
			}
			else {
				actionMenu.MenuItems.Add(new MenuItem("Send to Needs Review",actionMenu_RequestReviewerClick) { Enabled=perm });
			}
			AddCompleteAndCancelled(actionMenu,perm);
			return actionMenu;
		}

		private ContextMenu GetDocumentationActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
			actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Documenter",actionMenu_AssignDocumenterClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
			bool hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
			actionMenu.MenuItems.Add(new MenuItem("Send to In Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
			perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
			AddCompleteAndCancelled(actionMenu,perm);
			return actionMenu;
		}

		private ContextMenu GetCompleteActions() {
			ContextMenu actionMenu=new ContextMenu();
			actionMenu.MenuItems.Add(new MenuItem("Completed Job") { Enabled=false });
			bool perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
			actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });
			bool hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
			actionMenu.MenuItems.Add(new MenuItem("Send to In Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
			actionMenu.MenuItems.Add(new MenuItem("Send to Documentation",actionMenu_SendToDocumentation_Click) { Enabled=perm });
			return actionMenu;
		}

		private ContextMenu GetCancelledActions() {
			ContextMenu actionMenu=new ContextMenu();
			bool perm=JobPermissions.IsAuthorized(JobPerm.SeniorQueryCoordinator,true);
			actionMenu.MenuItems.Add(new MenuItem("Send to Definition",actionMenu_SendDefinitionClick) { Enabled=perm });
			actionMenu.MenuItems.Add(new MenuItem("Send to Quote",actionMenu_RequestQuoteClick) { Enabled=perm });
			bool hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
			actionMenu.MenuItems.Add(new MenuItem("Send to In Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
			actionMenu.MenuItems.Add(new MenuItem("Send to Documentation",actionMenu_SendToDocumentation_Click) { Enabled=perm });
			return actionMenu;
		}

		private void AddCompleteAndCancelled(ContextMenu actionMenu,bool perm) {
			bool hasCompleteReview=_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done) && perm;
			actionMenu.MenuItems.Add(new MenuItem("Mark as Complete",actionMenu_ImplementedClick) { Enabled=hasCompleteReview });
			if(perm) {
				actionMenu.MenuItems.Add("-");
				actionMenu.MenuItems.Add(new MenuItem("Cancel Query",actionMenu_CancelJobClick));
			}
		}

		#endregion ActionMenu

		private void butParentRemove_Click(object sender,EventArgs e) {
			_jobCur.ParentNum=0;
			_jobOld.ParentNum=0;
			textParent.Text="";
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job jobCur = Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=jobCur.Copy();
				long topParentNumOld = jobCur.TopParentNum;
				jobCur.ParentNum=0;
				jobCur.TopParentNum=jobCur.JobNum;
				Jobs.Update(jobCur,jobOld);
				List<Job> listJobsUpdated = new List<Job>();
				listJobsUpdated.Add(jobCur);
				if(jobCur.TopParentNum!=topParentNumOld) {
					List<Job> listTopParentJobs = Jobs.GetAllByTopParentNum(topParentNumOld);
					listJobsUpdated.AddRange(Jobs.GetChildJobs(jobCur,listTopParentJobs));
				}
				for(int i = 0;i<listJobsUpdated.Count();i++) {
					listJobsUpdated[i].TopParentNum=jobCur.TopParentNum;
				}
				Jobs.UpdateTopParentList(jobCur.TopParentNum,listJobsUpdated.Select(x => x.JobNum).ToList());
				_treeNode=Jobs.GetJobTreeTop(jobCur,checkIncludeCompleteCanceled.Checked);
				FillTreeRelated();
				for(int i = 0;i<listJobsUpdated.Count;i++) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,listJobsUpdated[i].JobNum);
				}
			}
		}

		private void butParentPick_Click(object sender,EventArgs e) {
			InputBox inBox=new InputBox("Input parent job number.");
			inBox.ShowDialog();
			if(inBox.IsDialogCancel) {
				return;
			}
			long parentNum=0;
			long.TryParse(new string(inBox.StringResult.Where(char.IsDigit).ToArray()),out parentNum);
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
				Job jobCur=Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=jobCur.Copy();
				long topParentNumOld=jobCur.TopParentNum;
				jobCur.ParentNum=parentNum;
				jobCur.TopParentNum=job.TopParentNum;
				Jobs.Update(jobCur,jobOld);
				List<Job> listJobsUpdated = new List<Job> { jobCur };
				if(jobCur.TopParentNum!=topParentNumOld) {
					List<Job> listTopParentJobs = Jobs.GetAllByTopParentNum(topParentNumOld);
					listJobsUpdated.AddRange(Jobs.GetChildJobs(jobCur,listTopParentJobs));
				}
				for(int i = 0;i<listJobsUpdated.Count();i++) {
					listJobsUpdated[i].TopParentNum=jobCur.TopParentNum;
				}
				Jobs.UpdateTopParentList(jobCur.TopParentNum,listJobsUpdated.Select(x => x.JobNum).ToList());
				_treeNode=Jobs.GetJobTreeTop(jobCur,checkIncludeCompleteCanceled.Checked);
				FillTreeRelated();
				for(int i = 0;i<listJobsUpdated.Count;i++) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,listJobsUpdated[i].JobNum);
				}
			}
		}

		///<summary>This should not implement any permissions, this should only check that the fields of the job are valid.</summary>
		private bool ValidateJob(Job _jobCur, bool doRequirementCheck=false) {
			textJobEditor.SetListJobRequirements(textJobEditor.GetListJobRequirements().FindAll(x => !String.IsNullOrEmpty(x.Description)));
			if(string.IsNullOrWhiteSpace(_jobCur.Title)) {
				MessageBox.Show("Invalid Title.");
				return false;
			}
			if(_jobCur.Category==JobCategory.Bug && _jobCur.ListJobLinks.Count(x => x.LinkType==JobLinkType.Bug)==0 && (_jobCur.PhaseCur!=JobPhase.Concept || _jobCur.IsApprovalNeeded)) {
				MsgBox.Show(this,"Bug jobs must have an attached bug.");
				return false;
			}
			if(doRequirementCheck
				//All of the listed job categories don't need requirements
				&& !_jobCur.Category.In(JobCategory.Bug,JobCategory.Query,JobCategory.MarketingDesign,JobCategory.UnresolvedIssue,JobCategory.SpecialProject,JobCategory.Conversion) 
				&& (textJobEditor.GetListJobRequirements()==null || textJobEditor.GetListJobRequirements().Count==0))
			{
				MsgBox.Show(this,"Please add at least one requirement for the job. There must be no empty requirements in the list.");
				return false;
			}
			return true;
		}

		#region ACTION BUTTON MENU ITEMS //====================================================

		#region Send To New Phase

		private void actionMenu_SendDefinitionClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			if(!PickUserByJobPermission("Pick Expert",JobPerm.QueryCoordinator,out userNumExpert,_jobCur.UserNumExpert,JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true,Security.CurUser.UserNum),false)) {
				return;
			}
			_jobCur.UserNumQuoter=0;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.PhaseCur=JobPhase.Definition;
			SaveJob(_jobCur);
		}

		private void actionMenu_SendInDevelopmentClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.QueryCoordinator,out userNumEngineer,_jobCur.UserNumEngineer>0 ? _jobCur.UserNumEngineer : _jobCur.UserNumConcept,true,false)) {
				return;
			}
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.PhaseCur=JobPhase.Development;
			SaveJob(_jobCur);
		}

		private void actionMenu_SendToConcept_Click(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumConcept=_jobCur.UserNumConcept;
			if(userNumConcept==0 && PickUserByJobPermission("Pick Concept Writer", JobPerm.QueryCoordinator, out userNumConcept, _jobCur.UserNumConcept, JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true,Security.CurUser.UserNum), false)) {
				return;
			}
			_jobCur.UserNumConcept=userNumConcept;
			_jobCur.PhaseCur=JobPhase.Concept;
			SaveJob(_jobCur);
		}

		private void actionMenu_SendToDocumentation_Click(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumDocumenter=_jobCur.UserNumDocumenter;
			if(userNumDocumenter==0 && PickUserByJobPermission("Pick Documenter", JobPerm.QueryCoordinator, out userNumDocumenter, _jobCur.UserNumDocumenter, JobPermissions.IsAuthorized(JobPerm.QueryCoordinator,true,Security.CurUser.UserNum), false)) {
				return;
			}
			_jobCur.UserNumDocumenter=userNumDocumenter;
			_jobCur.PhaseCur=JobPhase.Documentation;
			SaveJob(_jobCur);
		}

		#endregion Send To New Phase

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
			if(!PickUserByJobPermission("Pick Expert",JobPerm.SeniorQueryCoordinator,out userNumExpert,_jobCur.UserNumExpert,true,false)) {
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
			if(!PickUserByJobPermission("Pick Engineer",JobPerm.QueryCoordinator,out userNumEngineer,_jobCur.UserNumEngineer,true,false)) {
				return;
			}
			if(userNumEngineer==_jobOld.UserNumEngineer) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumEngineer=userNumEngineer;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignDocumenterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumDocumenter;
			if(!PickUserByJobPermission("Pick Documenter",JobPerm.QueryCoordinator,out userNumDocumenter,_jobCur.UserNumDocumenter,true,false)) {
				return;
			}
			if(userNumDocumenter==_jobOld.UserNumDocumenter) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumDocumenter=userNumDocumenter;
			_jobCur.PhaseCur=JobPhase.Documentation;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignQuoterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumQuoter;
			if(!PickUserByJobPermission("Pick Quoter",JobPerm.SeniorQueryCoordinator,out userNumQuoter,_jobCur.UserNumQuoter,true,false)) {
				return;
			}
			if(userNumQuoter==_jobOld.UserNumQuoter) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumQuoter=userNumQuoter;
			SaveJob(_jobCur);
		}

		#endregion

		#region Request Approval/Reviews

		private void actionMenu_RepealApprovalRequestClick(object sender,EventArgs e) {
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignReviewerClick(object sender,EventArgs e) {
			long userNumReviewer;
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.SeniorQueryCoordinator,out userNumReviewer,_jobCur.UserNumTester,true,false)) {
				return;
			}
			if(userNumReviewer==_jobOld.UserNumTester) {
				return;
			}
			if(_jobCur.ListJobReviews.Count<1) {
				JobReview jobReview=new JobReview();
				jobReview.JobNum=_jobCur.JobNum;
				jobReview.ReviewerNum=userNumReviewer;
				jobReview.DateTStamp=DateTime.Now;
				jobReview.Description="";
				jobReview.ReviewStatus=JobReviewStatus.Sent;
				jobReview.TimeReview=new TimeSpan(0);
				_jobCur.ListJobReviews.Add(jobReview);
			}
			IsChanged=true;
			_jobCur.UserNumTester=userNumReviewer;
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
			if(ODMessageBox.Show(this,"Are you sure you want to cancel this job?","Warning",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Cancelled;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestReviewerClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=true;
			SaveJob(_jobCur);
		}

		private void actionMenu_ImplementedClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(ODMessageBox.Show(this,"Are you sure you want to complete this job?","Warning",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			_jobCur.DateTimeImplemented=DateTime.Now;
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Complete;
			SaveJob(_jobCur);
		}

		#endregion Approval Options

		public void actionMenu_ViewLogsClick(object sender,EventArgs e) {
			FormJobLogs FormJL=new FormJobLogs(_jobCur);
			FormJL.Show();
		}

		#endregion ACTION BUTTON MENU ITEMS //=================================================

		///<summary>If returns false if selection is cancelled. DefaultUserNum is usually the currently set usernum for a given role.</summary>
		private bool PickUserByJobPermission(string prompt,JobPerm jobPerm,out long selectedNum, long suggestedUserNum = 0,bool AllowNone = true,bool AllowAll = true) {
			selectedNum=0;
			List<Userod> listUsersForPicker = Userods.GetUsersByJobRole(jobPerm,false);
			FrmUserPick frmUserPick = new FrmUserPick();
			frmUserPick.Text=prompt;
			frmUserPick.IsSelectionMode=true;
			frmUserPick.ListUserodsFiltered=listUsersForPicker;
			frmUserPick.UserNumSuggested=suggestedUserNum;
			frmUserPick.IsPickNoneAllowed=AllowNone;
			frmUserPick.IsShowAllAllowed=AllowAll;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return false;
			}
			selectedNum=frmUserPick.UserNumSelected;
			return true;
		}

		///<summary>Job must have all in memory fields filled. Eg. Job.ListJobLinks, Job.ListJobNotes, etc. Also makes some of the JobLog entries.</summary>
		private void SaveJob(Job job) {
			_isLoading=true;
			//Validation must happen before this is called.
			job.Requirements=textJobEditor.ConceptRtf;
			job.Implementation=textJobEditor.WriteupRtf;
			job.RequirementsJSON=JsonConvert.SerializeObject(textJobEditor.GetListJobRequirements());
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
			JobActiveLinks.UpsertLink(_jobCur,_jobOld,Security.CurUser.UserNum,checkIsActive.Checked);
			JobLogs.MakeLogEntry(job,_jobOld);
			if(job.PhaseCur!=_jobOld.PhaseCur) {
				JobLogs.MakeLogEntryForPhase(job,_jobOld);
			}
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
			Job jobOld=_jobCur.Copy();
			_jobCur.Title=textTitle.Text;
			if(Jobs.Update(_jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void textCustomer_Click(object sender,EventArgs e) {
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			if(jobQuote.PatNum!=0) {
				frmPatientSelect.ListPatNumsExplicit=new List<long> {jobQuote.PatNum};
			}
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
			Patient pat=Patients.GetPat(frmPatientSelect.PatNumSelected);
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
			Job jobOld=jobFromDB.Copy();
			jobFromDB.HoursEstimateConcept=_jobCur.HoursEstimateConcept;
			jobFromDB.HoursEstimateWriteup=_jobCur.HoursEstimateWriteup;
			jobFromDB.HoursEstimateDevelopment=_jobCur.HoursEstimateDevelopment;
			jobFromDB.HoursEstimateReview=_jobCur.HoursEstimateReview;
			if(Jobs.Update(jobFromDB,jobOld)) {//update the checkout num.
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
			}
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
			SaveComboPriorityHelper(_listPriorities[comboPriority.SelectedIndex].DefNum);
		}

		private void comboPriority_Leave(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			Def defPriorityFromText=_listPriorities.Find(x => x.ItemName==comboPriority.Text);
			if(defPriorityFromText==null) {
				MsgBox.Show("Invalid Job Priority. Priority not saved.");
				comboPriority.SelectedIndex=_listPriorities.Find(x => x.DefNum==_jobCur.Priority)?.ItemOrder??0;//Revert to saved priority
				return;
			}
			if(defPriorityFromText.DefNum==_jobCur.Priority) {
				return;
			}
			//Synchronize the SelectedIndex property of the combo box with the matching definition.
			comboPriority.SelectedIndex=defPriorityFromText.ItemOrder;
			SaveComboPriorityHelper(defPriorityFromText.DefNum);
		}

		private void SaveComboPriorityHelper(long priorityNum) {
			_jobCur.Priority=priorityNum;
			JobLogs.MakeLogEntryForPriority(_jobCur,_jobOld);
			_jobOld.Priority=priorityNum;
			if(!IsNew) {
				Job job=Jobs.GetOne(_jobCur.JobNum);
				Job jobOld=job.Copy();
				job.Priority=priorityNum;
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				}
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
				Job jobOld=job.Copy();
				job.PhaseCur=comboPhase.GetSelected<JobPhase>();
				if(Jobs.Update(job,jobOld)) {
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
				}
			}
		}

		private void textQuoteHours_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			jobQuote.Hours=textQuoteHours.Text;
			JobQuotes.Update(jobQuote);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void textQuoteAmount_Leave(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote = _jobCur.ListJobQuotes.FirstOrDefault();
			jobQuote.Amount=textQuoteAmount.Text;
			JobQuotes.Update(jobQuote);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void checkApproved_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			JobQuote jobQuote=_jobCur.ListJobQuotes.LastOrDefault();
			jobQuote.IsCustomerApproved=checkApproved.Checked;
			UpdateQuoteStatus(jobQuote);
		}

		private void UpdateQuoteStatus(JobQuote jobQuoteNew,JobQuote jobQuoteOld=null) {
			if(jobQuoteNew==null) {
				return;
			}
			Job jobOld=_jobCur.Copy();
			if(jobQuoteNew.IsCustomerApproved) {
				_jobCur.DateTimeCustContact=DateTime.Now;
				_jobCur.UserNumCustContact=Security.CurUser.UserNum;
			}
			else {
				_jobCur.DateTimeCustContact=DateTime.MinValue;
			}
			DatePickerApproved.SetDateTime(_jobCur.DateTimeCustContact);
			_isLoading=true;
			checkApproved.Checked=jobQuoteNew.IsCustomerApproved;
			_isLoading=false;
			if(checkApproved.Checked && _jobCur.PhaseCur!=JobPhase.Development) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Move this job to 'Development' Phase?")) {
					_jobCur.PhaseCur=JobPhase.Development;
				}
			}
			else if(_jobCur.PhaseCur!=JobPhase.Quote) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Move this job to 'Quote' Phase?")) {
					_jobCur.PhaseCur=JobPhase.Quote;
				}
			}
			if(!UpdateJobQuoteInJob(jobQuoteNew)) {
				MsgBox.Show(this,"Error saving quote changes.","Error");
			}
			SaveJob(_jobCur);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private bool UpdateJobQuoteInJob(JobQuote jobQuoteNew) {
			if(_jobCur.ListJobQuotes.Find(x=>x.JobQuoteNum==jobQuoteNew.JobQuoteNum) is JobQuote jobQuoteToUpdate) {
				jobQuoteToUpdate.PatNum=jobQuoteNew.PatNum;
				jobQuoteToUpdate.IsCustomerApproved=jobQuoteNew.IsCustomerApproved;
				jobQuoteToUpdate.Hours=jobQuoteNew.Hours;
				jobQuoteToUpdate.Amount=jobQuoteNew.Amount;
				jobQuoteToUpdate.ApprovedAmount=jobQuoteNew.ApprovedAmount;
				jobQuoteToUpdate.Note=jobQuoteNew.Note;
				return true;
			}
			return false;
		}

		private void checkIsActive_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			_isChanged=true;
		}

		private void DatePickerSched_CalendarSelectionChanged(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			Job jobOld=_jobCur.Copy();
			_jobCur.AckDateTime=DatePickerSched.GetDateTime();
			if(Jobs.Update(_jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			DatePickerSched.SetDateTime(_jobCur.AckDateTime);
		}

		private void DatePickerApproved_CalendarSelectionChanged(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			Job jobOld=_jobCur.Copy();
			_jobCur.DateTimeCustContact=DatePickerApproved.GetDateTime();
			if(Jobs.Update(_jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			DatePickerApproved.SetDateTime(_jobCur.DateTimeCustContact);
		}

		private void DatePickerDeadline_CalendarSelectionChanged(object sender,EventArgs e) {
			if(_isLoading || IsNew) {
				return;
			}
			Job jobOld=_jobCur.Copy();
			_jobCur.DateTimeTested=DatePickerDeadline.GetDateTime();
			if(Jobs.Update(_jobCur,jobOld)) {
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			DatePickerDeadline.SetDateTime(_jobCur.DateTimeTested);
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
			jobLink.FKey=FormTS.TaskNumSelected;
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
			if(_isLoading || _jobCur==null) {
				return;//_jobCur==null should never happen
			}
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.SeniorQueryCoordinator,out long userNumReviewer,_jobCur.UserNumTester,false,false)) {
				return;
			}
			JobReview jobReview=new JobReview();
			jobReview.JobNum=_jobCur.JobNum;
			jobReview.ReviewerNum=userNumReviewer;
			jobReview.DateTStamp=DateTime.Now;
			jobReview.Description="";
			jobReview.ReviewStatus=JobReviewStatus.Sent;
			jobReview.TimeReview=new TimeSpan(0);
			using FormJobReviewEdit FormJobReviewEdit=new FormJobReviewEdit(jobReview);
			FormJobReviewEdit.ShowDialog();
			if(FormJobReviewEdit.DialogResult!=DialogResult.OK || FormJobReviewEdit.JobReviewCur==null) {
				return;
			}
			if(!IsNew) {
				JobReviews.Insert(FormJobReviewEdit.JobReviewCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobReviews.Add(FormJobReviewEdit.JobReviewCur);
			FillGridReviews();
		}

		private void gridQuotes_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading || _jobCur==null) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true))
			{
				return;
			}
			using FormJobQuoteEdit FormJobQuoteEdit=new FormJobQuoteEdit(new JobQuote() {JobNum=_jobCur.JobNum,IsNew=true});
			FormJobQuoteEdit.ShowDialog();
			if(FormJobQuoteEdit.DialogResult!=DialogResult.OK || FormJobQuoteEdit.JobQuoteCur==null) {
				return;
			}
			if(!IsNew) {
				JobQuotes.Insert(FormJobQuoteEdit.JobQuoteCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobQuotes.Add(FormJobQuoteEdit.JobQuoteCur);
			FillGridQuotes();
		}

		private void gridQuotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridQuotes.ListGridRows[e.Row].Tag is JobQuote jobQuote)) {
				return;//should never happen
			}
			using FormJobQuoteEdit FormJobQuoteEdit = new FormJobQuoteEdit(jobQuote);
			FormJobQuoteEdit.ShowDialog();
			if(FormJobQuoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			JobQuote jobQuoteNew=FormJobQuoteEdit.JobQuoteCur;
			//Check if the JobQuote changed. If not, kick out so we don't make extra db calls.
			//Performance hit for serializing these objects is negligible as the objects have a miniscule object tree.
			if(JsonConvert.SerializeObject(jobQuote)==JsonConvert.SerializeObject(jobQuoteNew)) {
				return;
			}
			IsChanged=true;
			if(!IsNew) {
				if(jobQuoteNew==null) {
					if(_jobCur.ListJobQuotes.Count(x=>x.JobQuoteNum!=jobQuote.JobQuoteNum)==0) {
						MsgBox.Show("Cannot delete only quote for a query job.");
						IsChanged=false;
					}
					else {
						_jobCur.ListJobQuotes.RemoveAll(x=>x.JobQuoteNum==jobQuote.JobQuoteNum);//should remove only one
						JobQuotes.Delete(jobQuote.JobQuoteNum);
					}
				}
				else {
					if(_jobCur.UserNumQuoter==0 || (_jobCur.UserNumQuoter!=Security.CurUser.UserNum && MsgBox.Show(MsgBoxButtons.YesNo,"Replace Quoter?"))) {
						_jobCur.UserNumQuoter=Security.CurUser.UserNum;
					}
					UpdateQuoteStatus(jobQuoteNew,jobQuote);
				}
			}
			FillGridQuotes();
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
			if(!(gridReview.ListGridRows[e.Row].Tag is JobReview jobReview)) {
				return;//should never happen.
			}
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
					_jobCur.UserNumTester=FormJRE.JobReviewCur.ReviewerNum;
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
					Job jobOld=jobFromDB.Copy();
					jobFromDB.UserNumCheckout=Security.CurUser.UserNum;//change only the userNumCheckout.
					if(Jobs.Update(jobFromDB,jobOld)) {//update the checkout num.
						Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);//send signal that the job has been checked out.
					}
				}
			}
		}

		private void butCommlog_Click(object sender,EventArgs e) {
			FrmCommItem frmCommItem=new FrmCommItem(GetNewCommlog());
			frmCommItem.ShowDialog();
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
	

