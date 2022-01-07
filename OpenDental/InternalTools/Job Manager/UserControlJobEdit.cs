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
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace OpenDental.InternalTools.Job_Manager {
	
	public partial class UserControlJobEdit:UserControl {
		#region Fields
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
		private List<Def> _listPrioritiesAll;
		private List<string> _listCategoryNames;
		private List<string> _listCategoryNamesFiltered;
		///<summary>Used to save one call to the DB when the Mark Tested button is pushed and the job is marked Not Tested</summary>
		private bool _isMarkTestedPushed=false;
		///<summary>Only used in treeJobs_DragOver to reduce flickering.</summary>
		private TreeNode grayNode=null;
		#endregion Fields

		#region Delegates
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

		///<summary>The current job num. 0 if none selected.</summary>
		public long JobNumCur {
			get {
				return _jobCur?.JobNum??0;
			}
		}
		#endregion Properties

		public UserControlJobEdit() {
			InitializeComponent();
			Enum.GetNames(typeof(JobPhase)).ToList().ForEach(x => comboPhase.Items.Add(x));
			Enum.GetNames(typeof(JobPatternReviewProject)).ToList().ForEach(x => comboProject.Items.Add(x));
			Enum.GetNames(typeof(JobPatternReviewStatus)).ToList().ForEach(x => comboPatternStatus.Items.Add(x));
			Enum.GetNames(typeof(JobProposedVersion)).ToList().ForEach(x => comboProposedVersion.Items.Add(x));
			_listCategoryNames=Enum.GetNames(typeof(JobCategory)).ToList();
			_listCategoryNamesFiltered=_listCategoryNames.Where(x => !ListTools.In(x,JobCategory.Query.ToString(),JobCategory.MarketingDesign.ToString())).ToList();
			_listCategoryNamesFiltered.ForEach(x => comboCategory.Items.Add(x));
			if(!JobPermissions.IsAuthorized(JobPerm.TestingCoordinator,true) && tabControlMain.TabPages.Contains(tabTesting)) {
				tabControlMain.TabPages.Remove(tabTesting);
			}
			if(gridFiles.ContextMenu==null) {
				gridFiles.ContextMenu=new ContextMenu();
			}
			gridFiles.ContextMenu.Popup+=FilePopupHelper;
			if(gridReview.ContextMenu==null) {
				gridReview.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemGridReviewMarkComplete=new MenuItem();
			menuItemGridReviewMarkComplete.Text="Mark Complete";
			menuItemGridReviewMarkComplete.Click+=menuItemReviewComplete_Click;
			gridReview.ContextMenu.MenuItems.Add(menuItemGridReviewMarkComplete);
			MenuItem menuItemGridReviewNeedsAdditionalReview=new MenuItem();
			menuItemGridReviewNeedsAdditionalReview.Text="Needs Additional Review";
			menuItemGridReviewNeedsAdditionalReview.Click+=menuItemReviewNeedsAdditionalReview_Click;
			gridReview.ContextMenu.MenuItems.Add(menuItemGridReviewNeedsAdditionalReview);
			if(gridCustomers.ContextMenu==null) {
				gridCustomers.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemCustomersUnlink=new MenuItem();
			menuItemCustomersUnlink.Text="Unlink Customer";
			menuItemCustomersUnlink.Name="Unlink";
			menuItemCustomersUnlink.Click+=new System.EventHandler(this.menuItemCustomersUnlink_Click);
			gridCustomers.ContextMenu.MenuItems.Add(menuItemCustomersUnlink);
			MenuItem menuItemCustomersGoToChart=new MenuItem();
			menuItemCustomersGoToChart.Text="Go To Chart";
			menuItemCustomersGoToChart.Name="GoToChart";
			menuItemCustomersGoToChart.Click+=new System.EventHandler(this.menuItemCustomersGoToChart_Click);
			gridCustomers.ContextMenu.MenuItems.Add(menuItemCustomersGoToChart);
			gridCustomers.ContextMenu.Popup+=GridCustomersPopupHelper;
			if(gridSubscribers.ContextMenu==null) {
				gridSubscribers.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemSubscribersUnlink=new MenuItem();
			menuItemSubscribersUnlink.Text="Unlink Subscriber";
			menuItemSubscribersUnlink.Name="Unlink";
			menuItemSubscribersUnlink.Click+=new System.EventHandler(this.menuItemSubscribersUnlink_Click);
			gridSubscribers.ContextMenu.MenuItems.Add(menuItemSubscribersUnlink);
			gridSubscribers.ContextMenu.Popup+=GridSubscribersPopupHelper;
			if(gridTasks.ContextMenu==null) {
				gridTasks.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemTasksUnlink=new MenuItem();
			menuItemTasksUnlink.Text="Unlink Task";
			menuItemTasksUnlink.Name="Unlink";
			menuItemTasksUnlink.Click+=new System.EventHandler(this.menuItemTasksUnlink_Click);
			gridTasks.ContextMenu.MenuItems.Add(menuItemTasksUnlink);
			gridTasks.ContextMenu.Popup+=GridTasksPopupHelper;
			if(gridAppointments.ContextMenu==null) {
				gridAppointments.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemAppointmentsUnlink=new MenuItem();
			menuItemAppointmentsUnlink.Text="Unlink Appointment";
			menuItemAppointmentsUnlink.Name="Unlink";
			menuItemAppointmentsUnlink.Click+=new System.EventHandler(this.menuItemAppointmentsUnlink_Click);
			gridAppointments.ContextMenu.MenuItems.Add(menuItemAppointmentsUnlink);
			gridAppointments.ContextMenu.Popup+=GridAppointmentsPopupHelper;
			if(gridFeatureReq.ContextMenu==null) {
				gridFeatureReq.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemFeatureReqUnlink=new MenuItem();
			menuItemFeatureReqUnlink.Text="Unlink Feature Request";
			menuItemFeatureReqUnlink.Name="Unlink";
			menuItemFeatureReqUnlink.Click+=new System.EventHandler(this.menuItemFeatureRequestsUnlink_Click);
			gridFeatureReq.ContextMenu.MenuItems.Add(menuItemFeatureReqUnlink);
			gridFeatureReq.ContextMenu.Popup+=GridFeatureRequestsPopupHelper;
			if(gridBugs.ContextMenu==null) {
				gridBugs.ContextMenu=new ContextMenu();
			}
			MenuItem menuItemBugsUnlink=new MenuItem();
			menuItemBugsUnlink.Text="Unlink Bug";
			menuItemBugsUnlink.Name="Unlink";
			menuItemBugsUnlink.Click+=new System.EventHandler(this.menuItemBugsUnlink_Click);
			gridBugs.ContextMenu.MenuItems.Add(menuItemBugsUnlink);
			MenuItem menuItemBugsViewSubmissions=new MenuItem();
			menuItemBugsViewSubmissions.Text="View Submissions";
			menuItemBugsViewSubmissions.Name="ViewSubmissions";
			menuItemBugsViewSubmissions.Click+=new System.EventHandler(this.menuItemBugsViewSubmissions_Click);
			gridBugs.ContextMenu.MenuItems.Add(menuItemBugsViewSubmissions);
			gridBugs.ContextMenu.Popup+=GridBugsPopupHelper;
		}

		#region Popup Helpers
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

		///<summary>Handles enabling the menu item based on if an item is selected and load state.</summary>
		private void MenuItemEnabledHandler(GridOD grid,string itemName) {
			MenuItem menuItem=grid.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name==itemName);
			if(menuItem==null) {
				return; //Should never happen
			}
			if(_isLoading || grid.SelectedIndices.Length==0) {
				menuItem.Enabled=false;
			}
			else {
				menuItem.Enabled=true;
			}
		}

		public void GridCustomersPopupHelper(object sender,EventArgs e) {
			MenuItemEnabledHandler(gridCustomers,"Unlink");
			MenuItemEnabledHandler(gridCustomers,"GoToChart");
		}

		public void GridSubscribersPopupHelper(object sender,EventArgs e) {
			MenuItemEnabledHandler(gridSubscribers,"Unlink");
		}

		public void GridTasksPopupHelper(object sender,EventArgs e) {
			MenuItemEnabledHandler(gridTasks,"Unlink");
		}

		public void GridAppointmentsPopupHelper(object sender,EventArgs e) {
			MenuItemEnabledHandler(gridAppointments,"Unlink");
		}

		public void GridFeatureRequestsPopupHelper(object sender,EventArgs e) {
			MenuItem menuItemUnlink=gridFeatureReq.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="Unlink");
			if(_isLoading || gridFeatureReq.SelectedIndices.Length==0) {
				menuItemUnlink.Enabled=false;
			}
			else if(JobPermissions.IsAuthorized(JobPerm.Concept,true)
				|| JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				|| JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				|| JobPermissions.IsAuthorized(JobPerm.Documentation,true))
			{
				menuItemUnlink.Enabled=true;
			}
			else {
				menuItemUnlink.Enabled=false;
			}
		}

		public void GridBugsPopupHelper(object sender,EventArgs e) {
			MenuItem menuItemUnlink=gridBugs.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="Unlink");
			MenuItem menuItemViewSubmissions=gridBugs.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewSubmissions");
			if(_isLoading || gridBugs.SelectedIndices.Length==0) {
				menuItemUnlink.Enabled=false;
				menuItemViewSubmissions.Enabled=false;
				return;
			}
			if(JobPermissions.IsAuthorized(JobPerm.Concept,true) 
				|| JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				|| JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				|| JobPermissions.IsAuthorized(JobPerm.Documentation,true))
			{
				menuItemUnlink.Enabled=true;
				if(gridBugs.ListGridRows[gridBugs.GetSelectedIndex()].Tag.GetType()==typeof(MobileBug)) {
					menuItemViewSubmissions.Enabled=false;
				}
				else {
					menuItemViewSubmissions.Enabled=true;
				}
			}
			else {
				menuItemUnlink.Enabled=false;
				menuItemViewSubmissions.Enabled=false;
			}
		}
		#endregion Popup Helpers

		#region MenuItems
		///<summary>A helper function that removes a job link for the given key and type.</summary>
		private void RemoveJobLink(JobLinkType linkType,long fKey) {
			List<JobLink> listLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==linkType&&x.FKey==fKey);
			_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==linkType&&x.FKey==fKey);
			_jobOld.ListJobLinks.RemoveAll(x => x.LinkType==linkType&&x.FKey==fKey);
			listLinks.Select(x => x.JobLinkNum).ToList().ForEach(JobLinks.Delete);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
		}

		private void menuItemAppointmentsUnlink_Click(object sender,EventArgs e) {
			long FKey=gridAppointments.SelectedTag<Appointment>().AptNum;
			RemoveJobLink(JobLinkType.Appointment,FKey);
			FillGridAppointments();
		}

		private void menuItemBugsUnlink_Click(object sender,EventArgs e) {
			if(gridBugs.ListGridRows[gridBugs.GetSelectedIndex()].Tag.GetType()==typeof(MobileBug)) {
				RemoveJobLink(JobLinkType.MobileBug,gridBugs.SelectedTag<MobileBug>().MobileBugNum);
			}
			else {
				RemoveJobLink(JobLinkType.Bug,gridBugs.SelectedTag<Bug>().BugId);
			}
			FillGridBugs();
		}

		private void menuItemBugsViewSubmissions_Click(object sender,EventArgs e) {
			if(gridBugs.ListGridRows[gridBugs.GetSelectedIndex()].Tag.GetType()==typeof(MobileBug)) {
				MsgBox.Show("Submissions for mobile bugs is currently unsupported.");
			}
			else {
				using FormBugSubmissions FormBugSubs=new FormBugSubmissions(formBugSubmissionMode:FormBugSubmissionMode.ViewOnly);
				FormBugSubs.ListBugSubmissionsViewed=BugSubmissions.GetForBugId(gridBugs.SelectedTag<Bug>().BugId);
				FormBugSubs.ShowDialog();
			}
		}

		private void menuItemCustomersGoToChart_Click(object sender,EventArgs e) {
			GotoModule.GotoChart(gridCustomers.SelectedTag<Patient>().PatNum);
		}

		private void menuItemCustomersUnlink_Click(object sender,EventArgs e) {
			long FKey=gridCustomers.SelectedTag<Patient>().PatNum;
			RemoveJobLink(JobLinkType.Customer,FKey);
			FillGridCustomers();
		}

		private void menuItemFeatureRequestsUnlink_Click(object sender,EventArgs e) {
			long FKey=gridFeatureReq.SelectedTag<long>();
			List<JobLink> listLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request&&x.FKey==FKey);
			FeatureRequests.MarkAsApproved(listLinks.Select(x => x.JobLinkNum).ToList());
			RemoveJobLink(JobLinkType.Request,FKey);
			FillGridFeatureReq();
		}

		private void menuItemReviewComplete_Click(object sender,EventArgs e) {
			if(gridReview.SelectedIndices.Length<1) {
				return;
			}
			long FKey = ((JobReview)gridReview.ListGridRows[gridReview.SelectedIndices[0]].Tag).JobReviewNum;
			JobReview review = _jobCur.ListJobReviews.First(x => x.JobReviewNum==FKey);
			if(Security.CurUser.UserNum!=review.ReviewerNum) {
				using FormLogOn FormLO=new FormLogOn(review.ReviewerNum,true);
				FormLO.ShowDialog();
				if(FormLO.DialogResult!=DialogResult.OK) {
					return;
				}
				if(FormLO.CurUserSimpleSwitch.UserNum!=review.ReviewerNum) {
					return;
				}
			}
			review.ReviewStatus=JobReviewStatus.Done;
			using InputBox inputBox = new InputBox("Please enter the number of minutes spent on this review.",review.Minutes.ToString());
			if(inputBox.ShowDialog()==DialogResult.OK) {
				double time = 0;
				if(!Double.TryParse(inputBox.textResult.Text,out time)) {
					return;
				}
				review.Minutes=time;
				JobReviews.Update(review);
				FillGridReviews();
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void menuItemReviewNeedsAdditionalReview_Click(object sender,EventArgs e) {
			if(gridReview.SelectedIndices.Length<1) {
				return;
			}
			long FKey = ((JobReview)gridReview.ListGridRows[gridReview.SelectedIndices[0]].Tag).JobReviewNum;
			JobReview review = _jobCur.ListJobReviews.First(x => x.JobReviewNum==FKey);
			if(Security.CurUser.UserNum!=review.ReviewerNum) {
				using FormLogOn FormLO=new FormLogOn(review.ReviewerNum,true);
				FormLO.ShowDialog();
				if(FormLO.DialogResult!=DialogResult.OK) {
					return;
				}
				if(FormLO.CurUserSimpleSwitch.UserNum!=review.ReviewerNum) {
					return;
				}
			}
			review.ReviewStatus=JobReviewStatus.NeedsAdditionalReview;
			using InputBox inputBox = new InputBox("Please enter the number of minutes spent on this review.",review.Minutes.ToString());
			if(inputBox.ShowDialog()==DialogResult.OK) {
				double time = 0;
				if(!Double.TryParse(inputBox.textResult.Text,out time)) {
					return;
				}
				review.Minutes=time;
				JobReviews.Update(review);
				FillGridReviews();
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void menuItemSubscribersUnlink_Click(object sender,EventArgs e) {
			long FKey=gridSubscribers.SelectedTag<Userod>().UserNum;
			RemoveJobLink(JobLinkType.Subscriber,FKey);
			FillGridWatchers();
		}

		private void menuItemTasksUnlink_Click(object sender,EventArgs e) {
			long FKey=gridTasks.SelectedTag<long>();
			RemoveJobLink(JobLinkType.Task,FKey);
			FillGridTasks();
		}
		#endregion MenuItems

		#region Fill Grids
		private void FillAllGrids() {
			FillGridRoles();
			FillTreeRelated();
			FillGridCustomers();
			FillGridWatchers();
			FillGridQuote();
			FillGridTasks();
			FillGridAppointments();
			FillGridFeatureReq();
			FillGridBugs();
			FillGridFiles();
			FillGridDiscussion();
			FillGridTestingNotes();
			FillGridReviews();
		}

		private void FillGridRoles() {
			gridRoles.BeginUpdate();
			gridRoles.ListGridColumns.Clear();
			gridRoles.ListGridColumns.Add(new GridColumn("Role",90));
			gridRoles.ListGridColumns.Add(new GridColumn("User",50));
			gridRoles.NoteSpanStart=0;
			gridRoles.NoteSpanStop=1;
			gridRoles.ListGridRows.Clear();
			//These columns are ordered by convenience, If some other ordering would be more convenient then they should just be re-ordered.
			GridRow	gridRow=new GridRow();
			gridRow.Cells.Add("Expert");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumExpert));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Engineer");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumEngineer));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Documenter");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumDocumenter));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Submitter");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumConcept));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Quoter");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumQuoter));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Cust Quote");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumCustQuote));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Apprv Con");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumApproverConcept));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Apprv Quote");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumApproverQuote));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Apprv Job");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumApproverJob));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Apprv Chg");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumApproverChange));
			gridRoles.ListGridRows.Add(gridRow);
			foreach(JobReview jobReview in _jobCur.ListJobReviews.Where(x => x.ReviewStatus==JobReviewStatus.Done)) {
				gridRow=new GridRow();
				gridRow.Cells.Add("Reviewer");
				gridRow.Cells.Add(Userods.GetName(jobReview.ReviewerNum));
				gridRoles.ListGridRows.Add(gridRow);
			}
			if(_jobCur.ListJobReviews.Count(x => x.ReviewStatus==JobReviewStatus.Done)==0) {
				gridRow=new GridRow();
				gridRow.Cells.Add("Reviewer");
				gridRow.Cells.Add("");
				gridRoles.ListGridRows.Add(gridRow);
			}
			gridRow=new GridRow();
			gridRow.Cells.Add("Tester");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumTester));
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Cust. Contact");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumCustContact));
			if(_jobCur.DateTimeCustContact.Year<1880) {
				gridRow.Note="";
			}
			else {
				gridRow.Note=_jobCur.DateTimeCustContact.ToShortDateString();
			}
			gridRoles.ListGridRows.Add(gridRow);
			gridRow=new GridRow();
			gridRow.Cells.Add("Checked Out");
			gridRow.Cells.Add(Userods.GetName(_jobCur.UserNumCheckout));
			if(_jobCur.UserNumCheckout==0) {
				//Do nothing.
			}
			else if(_jobCur.UserNumCheckout!=Security.CurUser.UserNum) {
				gridRow.ColorBackG=Color.FromArgb(254,235,233);//light red
			}
			else {
				gridRow.ColorBackG=Color.FromArgb(235,254,233);//light green
			}
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

		private void FillGridCustomers() {
			gridCustomers.BeginUpdate();
			gridCustomers.ListGridColumns.Clear();
			gridCustomers.ListGridColumns.Add(new GridColumn("PatNum",50));
			gridCustomers.ListGridColumns.Add(new GridColumn("Name",50) { IsWidthDynamic=true });
			gridCustomers.ListGridColumns.Add(new GridColumn("BillType",50) { IsWidthDynamic=true });
			gridCustomers.ListGridRows.Clear();
			List<Patient> listPatients= Patients.GetMultPats(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Customer)
				.Select(x => x.FKey).ToList()).ToList();
			foreach(Patient pat in listPatients) {
				GridRow row=new GridRow() { Tag=pat };//JobQuote
				row.Cells.Add(pat.PatNum.ToString());
				row.Cells.Add(pat.GetNameFL());
				row.Cells.Add(Defs.GetDef(DefCat.BillingTypes,pat.BillingType).ItemName);
				gridCustomers.ListGridRows.Add(row);
			}
			gridCustomers.EndUpdate();
		}

		private void FillGridWatchers() {
			gridSubscribers.BeginUpdate();
			gridSubscribers.ListGridColumns.Clear();
			gridSubscribers.ListGridColumns.Add(new GridColumn("",50));
			gridSubscribers.ListGridRows.Clear();
			List<Userod> listSubscribers=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber)
				.Select(x => Userods.GetFirstOrDefault(y => y.UserNum==x.FKey)).ToList();
			foreach(Userod user in listSubscribers.FindAll(x => x!=null)) {
				GridRow row=new GridRow() { Tag =user };
				row.Cells.Add(user.UserName);
				gridSubscribers.ListGridRows.Add(row);
			}
			gridSubscribers.EndUpdate();
		}

		private void FillGridQuote() {
			gridQuotes.BeginUpdate();
			gridQuotes.ListGridColumns.Clear();
			gridQuotes.ListGridColumns.Add(new GridColumn("PatNum",50));
			gridQuotes.ListGridColumns.Add(new GridColumn("Hours",40,HorizontalAlignment.Center));
			gridQuotes.ListGridColumns.Add(new GridColumn("Amt",60,HorizontalAlignment.Right));
			gridQuotes.ListGridColumns.Add(new GridColumn("Appr?",50,HorizontalAlignment.Center));
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
			foreach(Task task in listTasks) {
				GridRow row=new GridRow() { Tag=task.TaskNum };//taskNum
				row.Cells.Add(task.DateTimeEntry.ToShortDateString());
				TaskList taskList=TaskLists.GetOne(task.TaskListNum);
				if(taskList==null || taskList.Descript==null) {
					row.Cells.Add("Invalid Task");
				}
				else {
					row.Cells.Add(taskList.Descript);
				}
				row.Cells.Add(task.TaskStatus==TaskStatusEnum.Done ? "X" : "");
				row.Note=StringTools.Truncate(task.Descript,100,true).Trim();
				gridTasks.ListGridRows.Add(row);
			}
			gridTasks.EndUpdate();
		}

		private void FillGridAppointments() {
			gridAppointments.BeginUpdate();
			gridAppointments.ListGridColumns.Clear();
			gridAppointments.ListGridColumns.Add(new GridColumn("Appt Date",75));
			gridAppointments.ListGridColumns.Add(new GridColumn("Provider",75) { IsWidthDynamic=true });
			gridAppointments.ListGridColumns.Add(new GridColumn("Op",75) { IsWidthDynamic=true });
			gridAppointments.NoteSpanStart=0;
			gridAppointments.NoteSpanStop=2;
			gridAppointments.ListGridRows.Clear();
			List<long> listApptNums=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Appointment).Select(x => x.FKey).ToList();
			List<Appointment> listAppts=Appointments.GetMultApts(listApptNums);
			List<Appointment> listApptsFuture=listAppts.Where(x => x.AptDateTime>DateTime.Now).OrderBy(x => x.AptDateTime).ToList();
			List<Appointment> listApptsPast=listAppts.Where(x => x.AptDateTime<=DateTime.Now).OrderByDescending(x => x.AptDateTime).ToList();
			foreach(Appointment apt in listApptsFuture) {
				GridRow row=new GridRow() { Tag=apt };
				row.ColorBackG=Color.FromArgb(125,255,194);
				row.Cells.Add(apt.AptDateTime.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(apt.ProvNum));
				row.Cells.Add(Operatories.GetAbbrev(apt.Op));
				row.Note=StringTools.Truncate(apt.Note,100,true).Trim();
				gridAppointments.ListGridRows.Add(row);
			}
			foreach(Appointment apt in listApptsPast) {
				GridRow row=new GridRow() { Tag=apt };
				row.Cells.Add(apt.AptDateTime.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(apt.ProvNum));
				row.Cells.Add(Operatories.GetAbbrev(apt.Op));
				row.Note=StringTools.Truncate(apt.Note,100,true).Trim();
				gridAppointments.ListGridRows.Add(row);
			}
			gridAppointments.EndUpdate();
		}

		private void FillGridFeatureReq() {
			gridFeatureReq.BeginUpdate();
			gridFeatureReq.ListGridColumns.Clear();
			gridFeatureReq.ListGridColumns.Add(new GridColumn("Feat Req Num",150));
			//todo: add status of FR. Difficult because FR dataset comes from webservice.
			//gridFeatureReq.Columns.Add(new ODGridColumn("Status",50){TextAlign=HorizontalAlignment.Center});
			gridFeatureReq.ListGridRows.Clear();
			List<long> listReqNums=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList();
			foreach(long reqNum in listReqNums) {
				GridRow row=new GridRow() { Tag=reqNum };//FR Num
				row.Cells.Add(reqNum.ToString());
				//todo: add status of FR. Difficult because FR dataset comes from webservice.
				gridFeatureReq.ListGridRows.Add(row);
			}
			gridFeatureReq.EndUpdate();
		}

		private void FillGridBugs() {
			gridBugs.BeginUpdate();
			gridBugs.ListGridColumns.Clear();
			gridBugs.ListGridColumns.Add(new GridColumn("Flags",50));
			gridBugs.ListGridColumns.Add(new GridColumn("Bug Num (From JRMT)",50));
			gridBugs.ListGridRows.Clear();
			List<JobLink> listBugLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug).ToList();
			for(int i = 0;i<listBugLinks.Count;i++) {
				GridRow row=new GridRow();
				if(listBugLinks[i].LinkType==JobLinkType.Bug) {
					Bug bug=Bugs.GetOne(listBugLinks[i].FKey);
					row.Cells.Add("");
					row.Cells.Add(bug==null ? "Invalid Bug" : bug.Description);
					if(bug==null) {
						bug=new Bug();
						bug.BugId=listBugLinks[i].FKey;
					}
					row.Tag=(bug);
				}
				else {
					MobileBug mobileBug=MobileBugs.GetOne(listBugLinks[i].FKey);
					row.Cells.Add(BugFlag.Mobile.ToString());
					row.Cells.Add(mobileBug==null ? "Invalid Bug" : mobileBug.Description);
					if(mobileBug==null) {
						mobileBug=new MobileBug();
						mobileBug.MobileBugNum=listBugLinks[i].FKey;
					}
					row.Tag=(mobileBug);
				}
				gridBugs.ListGridRows.Add(row);
			}
			gridBugs.EndUpdate();
		}

		private void FillGridFiles() {
			gridFiles.BeginUpdate();
			gridFiles.ListGridColumns.Clear();
			gridFiles.ListGridColumns.Add(new GridColumn(Lan.g(this,""),120));
			gridFiles.ListGridRows.Clear();
			GridRow row;
			List<JobLink> listFiles=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.File);
			foreach(JobLink link in listFiles) {
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

		public void FillGridDiscussion() {
			FillGridWithJobNotes(gridNotes,_jobCur.ListJobNotes.FindAll(x => x.NoteType==JobNoteTypes.Discussion));
		}

		public void FillGridTestingNotes() {
			FillGridWithJobNotes(gridTestingNotes,_jobCur.ListJobNotes.FindAll(x => x.NoteType==JobNoteTypes.Testing));
		}

		///<summary>Helper method that fills the grid passed in with the corresponding job notes.
		///This is here because right now every grid that displays JobNotes shows them the same way.</summary>
		private void FillGridWithJobNotes(GridOD grid,List<JobNote> listJobNotes) {
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date Time"),120));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"User"),80));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Note"),400));
			grid.ListGridRows.Clear();
			GridRow row;
			listJobNotes=listJobNotes.OrderBy(x => x.DateTimeNote).ToList();
			foreach(JobNote jobNote in listJobNotes) {
				row=new GridRow();
				row.Cells.Add(jobNote.DateTimeNote.ToShortDateString()+" "+jobNote.DateTimeNote.ToShortTimeString());
				row.Cells.Add(Userods.GetName(jobNote.UserNum));
				row.Cells.Add(jobNote.Note);
				row.Tag=jobNote;
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollValue=grid.ScrollValue;//this forces scroll value to reset if it's > allowed max.
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
			for(int i = 0;i<gridReview.ListGridRows.Count;i++) {
				if(gridReview.ListGridRows[i].Tag is JobReview && ((JobReview)gridReview.ListGridRows[i].Tag).JobReviewNum==selectedReviewNum) {
					gridReview.SetSelected(i,true);
					break;
				}
			}
		}
		#endregion Fill Grids

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
				job.Priority=comboPriority.GetSelectedDefNum();
			}
			if(comboPriorityTesting.SelectedIndex > -1) {
				job.PriorityTesting=comboPriorityTesting.GetSelectedDefNum();
			}
			job.PhaseCur=(JobPhase)comboPhase.SelectedIndex;
			job.Category=(JobCategory)_listCategoryNames.IndexOf(comboCategory.SelectedItem.ToString());
			return job;
		}

		///<summary>Should only be called once when new job should be loaded into control. If called again, changes will be lost.</summary>
		public void LoadJob(Job job,TreeNode treeNode) {
			Job jobPrev=null;
			if(_jobCur!=null) {
				jobPrev = _jobCur.Copy();
			}
			_isLoading=true;
			if(comboPriority.Items.Count==0) {
				_listPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList();
				_listPrioritiesAll=Defs.GetDefsForCategory(DefCat.JobPriorities).OrderBy(x => x.ItemOrder).ToList();
				comboPriority.Items.AddDefs(_listPriorities);
				comboPriorityTesting.Items.AddDefs(_listPriorities);
			}
			this.Enabled=false;//disable control while it is filled.
			_isOverride=false;
			IsChanged=false;
			_treeNode=treeNode;
			if(job==null) {
				_jobCur=new Job();
			}
			else {
				_jobCur=job.Copy();
				IsNew=job.IsNew;
			}
			_jobOld=_jobCur.Copy();//cannot be null
			textTitle.Text=_jobCur.Title;
			textJobNum.Text=_jobCur.JobNum>0?_jobCur.JobNum.ToString():Lan.g("Jobs","New Job");
			comboPriority.SetSelectedDefNum(_jobCur.Priority);
			comboPriorityTesting.SetSelectedDefNum(_jobCur.PriorityTesting);
			comboPhase.SelectedIndex=(int)_jobCur.PhaseCur;
			comboProposedVersion.SelectedIndex=(int)_jobCur.ProposedVersion;
			comboProject.SelectedIndex=(int)_jobCur.PatternReviewProject;
			comboPatternStatus.SelectedIndex=(int)_jobCur.PatternReviewStatus;
			textDateTested.Text=_jobCur.DateTimeTested.ToShortDateString();
			checkNotTested.Checked=_jobCur.IsNotTested;
			checkIsActive.Checked=job.ListJobActiveLinks.Exists(x => x.UserNum==Security.CurUser.UserNum && x.DateTimeEnd==DateTime.MinValue);
			if(JobPermissions.IsAuthorized(JobPerm.Concept,true) && !ListTools.In(job.PhaseCur,JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation)) {
				checkIsActive.Enabled=true;
			}
			else {
				checkIsActive.Enabled=false;
			}
			if(_jobCur.IsApprovalNeeded) {
				textApprove.Text="Waiting";
			}
			else if(_jobCur.UserNumApproverConcept>0 ||_jobCur.UserNumApproverJob>0||_jobCur.UserNumApproverChange>0) {
				textApprove.Text="Yes";
			}
			else {
				textApprove.Text="No";
			}
			comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobCur.Category.ToString());
			textDateEntry.Text=_jobCur.DateTimeEntry.Year>1880?_jobCur.DateTimeEntry.ToShortDateString():"";
			textVersion.Text=_jobCur.JobVersion;
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
			textTestingHours.Text=_jobCur.HoursTesting.ToString();
			textJobEditor.ConceptTitle=_jobCur.Title;
			textEstHours.Text=_jobCur.HoursEstimate.ToString();
			textActualHours.Text=_jobCur.HoursActual.ToString();
			butActions.Enabled=true;
			butPopout.Enabled=true;
			checkIsActive.Enabled=true;
			SetHoursLeft();
			Job parent=Jobs.GetOne(_jobCur.ParentNum);
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				textEditorDocumentation.ReadOnly=true;
			}
			try {
				textEditorDocumentation.MainRtf=_jobCur.Documentation;//This is here to convert our old job descriptions to the new RTF descriptions.
			}
			catch {
				textEditorDocumentation.MainText=_jobCur.Documentation;
			}
			FillAllGrids();
			IsChanged=false;
			CheckPermissions();
			//This needs to be after CheckPermissions
			try {
				textJobEditor.SetListJobRequirements(JsonConvert.DeserializeObject<List<JobRequirement>>(_jobCur.RequirementsJSON));
			}
			catch {
				textJobEditor.SetListJobRequirements(new List<JobRequirement>());
			}
			CreateViewLog(jobPrev);
			if(job!=null) {//re-enable control after we have loaded the job.
				JobNotifications.DeleteForJobAndUser(job.JobNum,Security.CurUser.UserNum);
				this.Enabled=true;
			}
			if(jobPrev==null || jobPrev.JobNum!=job.JobNum) {
				textJobEditor.ResizeTextFields();
			}
			DisableForPopout();
			//Remove the mouse wheel functionality from specific combo boxes.
			comboCategory.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			comboPhase.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			comboProject.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			comboPatternStatus.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			comboProposedVersion.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			//ComboBoxPlus does not support mouse wheel functionality by default so no need to handle the MouseWheel event for them.
			//comboPriority.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			//comboPriorityTesting.MouseWheel+=new MouseEventHandler(comboBox_MouseWheel);
			_isLoading=false;
		}

		private void DisableForPopout() {
			if(!IsPopout) {
				return;
			}
			butPopout.Enabled=false;
			treeRelatedJobs.Enabled=false;
		}

		private void comboBox_MouseWheel(object sender, MouseEventArgs e) {
			ComboBox comboControl=(ComboBox)sender;
			if(!comboControl.DroppedDown) {
				((HandledMouseEventArgs)e).Handled=true;
			}
		}

		#region Tree Related Jobs
		private void TryMoveJobtoJob(TreeNode sourceNode,TreeNode destinationNode) {
			Job sourceJob = (Job)sourceNode.Tag;
			if(!_isOverride
				&&sourceJob.UserNumEngineer!=Security.CurUser.UserNum
				&&sourceJob.UserNumExpert!=Security.CurUser.UserNum
				&&!JobPermissions.IsAuthorized(JobPerm.Approval,true)
				&&!JobPermissions.IsAuthorized(JobPerm.FeatureManager)) {
				return;//only expert, engineer, Approver, FeatureManager, or override can drag and drop.
			}
			if(destinationNode==null) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move selected job to top level?")) {
					return;
				}
				sourceJob.ParentNum=0;
			}
			else if(destinationNode.Tag is Job) {
				Job destinationJob=(Job)destinationNode.Tag;
				//Don't move the job to itself
				if(sourceJob.JobNum==destinationJob.JobNum) {
					return;
				}
				try {
					//This method throws an exception so we can have an accurate message of why this would cause a loop.
					JobManagerCore.ValidateJobLoop(sourceJob,destinationJob.JobNum);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move selected job?")) {
					return;
				}
				if(sourceNode.Parent==null) {
					treeRelatedJobs.Nodes.Remove(sourceNode);
				}
				else {
					sourceNode.Parent.Nodes.Remove(sourceNode);
				}
				//If you move the job to the current parent, make it a sibling of its parent
				//This is the equivalent of dropping the job on the parent of its current parent
				if(sourceJob.ParentNum==destinationJob.JobNum) {
					sourceJob.ParentNum=destinationJob.ParentNum;
					if(destinationNode.Parent!=null) {
						destinationNode.Parent.Nodes.Add(sourceNode);
					}
				}
				//Otherwise set the job as a child of the destination job
				else {
					sourceJob.ParentNum=destinationJob.JobNum;
					destinationNode.Nodes.Add(sourceNode);
				}
			}
			else {
				return;//no valid target
			}
			Jobs.Update(sourceJob);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,sourceJob.JobNum);
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

		private void treeRelatedJobs_AfterSelect(object sender,TreeViewEventArgs e) {
			treeRelatedJobs.SelectedNode=null;
		}

		private void treeRelatedJobs_ItemDrag(object sender,ItemDragEventArgs e) {
			treeRelatedJobs.SelectedNode=(TreeNode)e.Item;
			DoDragDrop(e.Item,DragDropEffects.Move);
		}

		private void treeRelatedJobs_DragEnter(object sender,DragEventArgs e) {
			e.Effect=e.AllowedEffect;
		}

		private void treeRelatedJobs_DragDrop(object sender,DragEventArgs e) {
			if(grayNode!=null) {
				grayNode.BackColor=Color.White;
			}
			if(IsChanged) {
				MessageBox.Show("You must save changes to current job before making drag and drop changes.");
				return;
			}
			if(IsPopout) {
				return;
			}
			if(!e.Data.GetDataPresent("System.Windows.Forms.TreeNode",false)) {
				return;
			}
			Point pt = ((TreeView)sender).PointToClient(new Point(e.X,e.Y));
			TreeNode destinationNode = ((TreeView)sender).GetNodeAt(pt);
			TreeNode sourceNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
			if(!(sourceNode.Tag is Job)) {//only allow move is source node was a job.
				return;//might have to set some additional variable instead of just returning.
			}
			TryMoveJobtoJob(sourceNode,destinationNode);
		}

		private void treeRelatedJobs_DragOver(object sender,DragEventArgs e) {
			Point p=treeRelatedJobs.PointToClient(new Point(e.X,e.Y));
			TreeNode node=treeRelatedJobs.GetNodeAt(p);
			if(grayNode!=null && grayNode!=node) {
				grayNode.BackColor=Color.White;
				grayNode=null;
			}
			if(node!=null && node.BackColor!=Color.LightGray) {
				node.BackColor=Color.LightGray;
				grayNode=node;
			}
			treeRelatedJobs.SelectedNode=node;
			if(p.Y<25) {
				MiscUtils.SendMessage(treeRelatedJobs.Handle,277,0,0);//Scroll Up
			}
			else if(p.Y>treeRelatedJobs.Height-25) {
				MiscUtils.SendMessage(treeRelatedJobs.Handle,277,1,0);//Scroll down.
			}
		}
		#endregion Tree Related Jobs

		///<summary>Based on job status, category, and user role, this will enable or disable various controls.</summary>
		private void CheckPermissions() {
			//disable various controls and re-enable them below depending on permissions.
			textTitle.ReadOnly=true;
			comboPriority.Enabled=false;
			comboPhase.Enabled=false;
			comboCategory.Enabled=false;
			textVersion.ReadOnly=true;
			textEstHours.Enabled=false;
			textActualHours.Enabled=false;
			butParentPick.Visible=false;
			butParentRemove.Visible=false;
			gridQuotes.HasAddButton=false;//Quote permission only
			textJobEditor.ReadOnlyConcept=true;
			textJobEditor.ReadOnlyWriteup=true;
			textJobEditor.ReadOnlyRequirementsGrid=true;
			comboProject.Enabled=false;
			comboPatternStatus.Enabled=JobPermissions.IsAuthorized(JobPerm.PatternReview,true);
			if(_jobCur==null) {
				return;
			}
			if(JobPermissions.IsAuthorized(JobPerm.Quote,true) && _jobOld.PhaseCur!=JobPhase.Complete && _jobOld.PhaseCur!=JobPhase.Cancelled) {
				gridQuotes.HasAddButton=true;
			}
			switch(_jobCur.PhaseCur) {
				case JobPhase.Concept:
					if(!JobPermissions.IsAuthorized(JobPerm.Concept,true) || (_jobCur.IsApprovalNeeded && !JobPermissions.IsAuthorized(JobPerm.Approval,true))) {
						break;
					}
					//Can only edit concept job if you meet one of the following
					//1) You have concept permission.
					//2) Concept needs approval and you have approval permission
					textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					comboCategory.Enabled=true;
					textVersion.ReadOnly=false;
					textEstHours.Enabled=true;
					textActualHours.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					textJobEditor.ReadOnlyConcept=false;
					textJobEditor.ReadOnlyWriteup=false;
					textJobEditor.ReadOnlyRequirementsGrid=false;
					break;
				case JobPhase.Quote:
					if(_jobCur.IsApprovalNeeded && !JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
						break;
					}
					else if(!_jobCur.IsApprovalNeeded && _jobCur.UserNumCustQuote!=0 && !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true) && _jobCur.UserNumCustQuote!=Security.CurUser.UserNum) {
						break;
					}
					else if(!_jobCur.IsApprovalNeeded && _jobCur.UserNumCustQuote==0 && !JobPermissions.IsAuthorized(JobPerm.Writeup,true) && _jobCur.UserNumQuoter!=Security.CurUser.UserNum) {
						break;
					}
					//Can only edit quote job if you meet one of the following
					//1) You have quote permission.
					//2) quote needs approval and you have approval permission
					textEstHours.Enabled=true;
					textJobEditor.ReadOnlyConcept=false;
					textJobEditor.ReadOnlyWriteup=false;
					textJobEditor.ReadOnlyRequirementsGrid=false;
					gridQuotes.HasAddButton=true;//Quote permission only
					break;
				case JobPhase.Definition:
					if(!_jobCur.IsApprovalNeeded
						&& (!JobPermissions.IsAuthorized(JobPerm.Writeup,true)
							|| (JobPermissions.IsAuthorized(JobPerm.Writeup,true) && _jobCur.UserNumExpert!=Security.CurUser.UserNum && _jobCur.UserNumExpert!=0))) 
					{
						break;
					}
					if(_jobCur.IsApprovalNeeded && !JobPermissions.IsAuthorized(JobPerm.Approval,true)) {//job needs approval and you are not authorized.
						break;
					}
					//Can only edit writeup job if you meet one of the following
					//1) You have writeup permission and the job is unnasigned or assigned to you
					//2) Job needs approval and you have approval permission
					textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					comboCategory.Enabled=true;
					textVersion.ReadOnly=false;
					textEstHours.Enabled=true;
					textActualHours.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					textJobEditor.ReadOnlyConcept=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					textJobEditor.ReadOnlyWriteup=false;
					textJobEditor.ReadOnlyRequirementsGrid=false;//There is a check when editing these that handles edits gracefully
					break;
				case JobPhase.Development:
					if(!_jobCur.IsApprovalNeeded
						&& (!JobPermissions.IsAuthorized(JobPerm.Writeup,true)
							|| (JobPermissions.IsAuthorized(JobPerm.Writeup,true) && _jobCur.UserNumExpert!=Security.CurUser.UserNum && _jobCur.UserNumExpert!=0))
						&& (!JobPermissions.IsAuthorized(JobPerm.Engineer,true)
							|| (JobPermissions.IsAuthorized(JobPerm.Engineer,true) && _jobCur.UserNumEngineer!=Security.CurUser.UserNum && _jobCur.UserNumEngineer!=0))) 
					{
						break;//only the expert or engineer can edit the job description.
					}
					if(_jobCur.IsApprovalNeeded && !JobPermissions.IsAuthorized(JobPerm.Approval,true)) {//job needs approval and you are not authorized.
						break;
					}
					//Can only edit development job if you meet one of the following
					//1) You have Writeup permission and you are the expert.
					//1) You have Engineer permission and you are the engineer.
					//2) Job needs approval and you have approval permission
					//textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					comboCategory.Enabled=true;
					textVersion.ReadOnly=false;
					textEstHours.Enabled=true;
					textActualHours.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					textJobEditor.ReadOnlyConcept=true;
					textJobEditor.ReadOnlyWriteup=false; //Using Change Request action allows editing of jobs in development.
					textJobEditor.ReadOnlyRequirementsGrid=false;//There is a check when editing these that handles edits gracefully
					break;
				case JobPhase.Documentation:
					if(!JobPermissions.IsAuthorized(JobPerm.Documentation,true)) {
						break;
					}
					//Can only edit Document job if you meet one of the following
					//1) You have Document permission.
					textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					//comboStatus.Enabled=true;
					//comboCategory.Enabled=true;
					textVersion.ReadOnly=false;
					textEstHours.Enabled=true;
					textActualHours.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					//textEditorMain.ReadOnly=false;
					break;
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
			if(_jobCur.Category==JobCategory.UnresolvedIssue) {
				if(JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true) && _jobCur.PhaseCur!=JobPhase.Cancelled) {
					//Disables category so it can't be changed into anything else
					textTitle.ReadOnly=false;
					comboPriority.Enabled=true;
					comboPhase.Enabled=false;
					comboCategory.Enabled=false;
					textVersion.ReadOnly=false;
					textEstHours.Enabled=true;
					textActualHours.Enabled=true;
					butParentPick.Visible=true;
					butParentRemove.Visible=true;
					//gridCustomerQuotes.HasAddButton=true;//Quote permission only
					textJobEditor.ReadOnlyConcept=false;
					textJobEditor.ReadOnlyWriteup=false;
					textJobEditor.ReadOnlyRequirementsGrid=false;
					comboProject.Enabled=false;
					comboPatternStatus.Enabled=JobPermissions.IsAuthorized(JobPerm.PatternReview,true);
				}
				else {
					textTitle.ReadOnly=true;
					comboPriority.Enabled=false;
					comboPhase.Enabled=false;
					comboCategory.Enabled=false;
					textVersion.ReadOnly=true;
					textEstHours.Enabled=false;
					textActualHours.Enabled=false;
					butParentPick.Visible=false;
					butParentRemove.Visible=false;
					gridQuotes.HasAddButton=false;//Quote permission only
					textJobEditor.ReadOnlyConcept=true;
					textJobEditor.ReadOnlyWriteup=true;
					textJobEditor.ReadOnlyRequirementsGrid=true;
					comboProject.Enabled=false;
					comboPatternStatus.Enabled=JobPermissions.IsAuthorized(JobPerm.PatternReview,true);
				}
			}
			//Disable description, documentation, and title if "Checked out"
			textJobEditor.Enabled=true;//might still be read only.
			textEditorDocumentation.Enabled=true;
			if(_jobCur.UserNumCheckout!=0 && _jobCur.UserNumCheckout!=Security.CurUser.UserNum) {
				textTitle.ReadOnly=true;
				textJobEditor.ReadOnlyConcept=true;
				textJobEditor.ReadOnlyWriteup=true;
				textJobEditor.ReadOnlyRequirementsGrid=true;
				textJobEditor.Enabled=false;
				textEditorDocumentation.Enabled=false;
			}
			if(_isOverride) {//Enable everything and make everything visible
				textTitle.ReadOnly=false;
				comboPriority.Enabled=true;
				comboPhase.Enabled=true;
				comboCategory.Enabled=true;
				textVersion.ReadOnly=false;
				textEstHours.Enabled=true;
				textActualHours.Enabled=true;
				butParentPick.Visible=true;
				butParentRemove.Visible=true;
				gridQuotes.HasAddButton=true;
				textJobEditor.ReadOnlyConcept=false;
				textJobEditor.ReadOnlyWriteup=false;
				if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
					textJobEditor.ReadOnlyRequirementsGrid=false;
				}
				textJobEditor.Enabled=true;
				textEditorDocumentation.Enabled=true;
				if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
					comboPatternStatus.Enabled=true;
					comboProject.Enabled=true;
				}
			}
		}

		///<summary>This is a nasty method. Be careful with making changes to it.</summary>
		private void butActions_Click(object sender,EventArgs e) {
			bool hasPermission = false;
			bool isExpert = false;
			bool isEngineer = false;
			const bool IsApprovalNeeded=true;
			ContextMenu actionMenu = new System.Windows.Forms.ContextMenu();
			//Job Actions are directed by three things, current JobCategory, current JobPhase, and whether the job is awaiting approval
			//Anything not explicitly stated in the switch is not considered to be a valid JobState for this UserControl
			switch(_jobCur.Category, _jobCur.PhaseCur, _jobCur.IsApprovalNeeded) {
				#region Jobs Requiring Approval
				#region Research Jobs
				//Research jobs in concept
				case (JobCategory.Research, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has approval or concept permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestConceptApprovalClick) { Enabled=hasPermission });
					//Cancelling a concept is only allowed if you are the submitter
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission && _jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				//Research jobs waiting for concept approval
				case (JobCategory.Research, JobPhase.Concept, IsApprovalNeeded):
					//If the user has approval permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					if(_jobCur.UserNumConcept==Security.CurUser.UserNum || JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Approve For Research",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestConceptClarificationClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Concept",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				//Research job in research phase
				case (JobCategory.Research, JobPhase.Definition, !IsApprovalNeeded):
					//If the user has approval or writeup permissions, they can assign experts
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Writeup,true)||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					}
					//If the user has approval permissions then they can send a job in definition for approval
					//If the user doesn't have approval permissions, but they have writeup permissions, they can send a job in definition for approval if they are the expert or there is no expert
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)
						||(JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum));
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestConceptApprovalClick) { Enabled=hasPermission });
					//Users with the approval permission can unapprove concepts that are being written up
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Unapprove Job",actionMenu_UnapproveJobClick) { Enabled=true });
					}
					break;
				//Research job waiting for research acknowledgement
				case (JobCategory.Research, JobPhase.Definition, IsApprovalNeeded):
					//If the user does not have approval permissions, but they are the expert, allow them to repeal their own approval request
					if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&_jobCur.UserNumExpert==Security.CurUser.UserNum) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					else {
						//If the user has approval permissions, they can complete the research job or ask for further clarification
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Complete Job",actionMenu_DirectlyComplete) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestDefinitionClarificationClick) { Enabled=hasPermission });
					}
					break;
				//Research Job that has been completed
				case (JobCategory.Research, JobPhase.Complete, !IsApprovalNeeded):
					//Currently no actions
					break;
				//Research Job that hase been cancelled
				case (JobCategory.Research, JobPhase.Cancelled, !IsApprovalNeeded):
					//Users with approval permissions can reopen cancelled jobs as concepts or reopen them for writeup
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen for Definition",actionMenu_ApproveJobClick) { Enabled=hasPermission });
					break;
				#endregion
				#region Normal Jobs
				//All of the following job categories currently follow the exact same process
				case (JobCategory.Enhancement, JobPhase.Concept, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Concept, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Concept, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Concept, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has approval or concept permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestConceptApprovalClick) { Enabled=hasPermission });
					//Cancelling a concept is only allowed if you are the submitter
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.Enhancement, JobPhase.Concept, IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Concept, IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Concept, IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Concept, IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Concept, IsApprovalNeeded):
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					//If the user does not have approval permission and they are the current submitter, they can repeal the approval request
					if((!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&_jobCur.UserNumConcept==Security.CurUser.UserNum) || JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					//If the user has approval permissions, then they can approve jobs or request quotes
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Approve Concept",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Request Quote",actionMenu_RequestQuoteClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Approve Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Concept",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Enhancement, JobPhase.Quote, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Quote, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Quote, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Quote, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Quote, !IsApprovalNeeded):
					//If the a job quote has not been given yet
					if(_jobCur.UserNumApproverQuote==0) {
						//If the user has quote permissions then they can send a quote for approval
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Quote,true)||_isOverride;
						actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestQuoteApprovalClick) { Enabled=hasPermission });
					}
					//If the job quote has been given and approved, but the customer has not approved the quote
					else {
						//If the user has NotifyCustomer permission then they can deliver the quote to the customer
						hasPermission=JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)||_isOverride;
						actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestCustQuoteApprovalClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Quote, IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Quote, IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Quote, IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Quote, IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Quote, IsApprovalNeeded):
					//If the user has approval permissions, they have access to all of the following actions
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					//The job has a quote that has not been approved, it has not been approved by the customer either
					if(_jobCur.UserNumApproverQuote==0) {
						actionMenu.MenuItems.Add(new MenuItem("Approve Quote",actionMenu_ApproveQuoteClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					//The job has a quote that has been approved and it has also been approved by the customer
					else {
						actionMenu.MenuItems.Add(new MenuItem("Send to Writeup",actionMenu_SendWriteupClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Definition, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Definition, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Definition, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Definition, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Definition, !IsApprovalNeeded):
					//If the user has approval or writeup permissions, they can assign experts
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Writeup,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=hasPermission });
					//If the user has approval permissions or they have writeup permissions and they are the expert or none is assigned, then they can send the job for approval
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)
						||(JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum));
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestJobApprovalClick) { Enabled=hasPermission });
					//If the user has approval permissions, they can unapprove the concept or cancel the job.
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Unapprove Concept",actionMenu_UnapproveJobClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Enhancement, JobPhase.Definition, IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Definition, IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Definition, IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Definition, IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Definition, IsApprovalNeeded):
					//If the user does not have approval permission and they are the current expert, they can repeal the approval request
					if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&_jobCur.UserNumExpert==Security.CurUser.UserNum) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					else {
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Approve Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Development, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Development, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Development, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Development, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Development, !IsApprovalNeeded):
					//If the user has approval or writeup permissions, they can assign experts
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Writeup,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					//If the user has approval or is the current expert permissions, they can assign engineers
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||_jobCur.UserNumExpert==Security.CurUser.UserNum||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=true });
					//If the job currently has no engineer then an engineer may take the job
					if(_jobCur.UserNumEngineer==0&&JobPermissions.IsAuthorized(JobPerm.Engineer,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Take Job",actionMenu_TakeJobClick) { Enabled=true });
					}
					//Only the assigned engineer may request a review.
					hasPermission=_jobCur.UserNumEngineer==Security.CurUser.UserNum;
					actionMenu.MenuItems.Add(new MenuItem("Request Review",actionMenu_RequestReviewClick) { Enabled=hasPermission });
					//If the user is an expert or an engineer assigned to the job then they can request approval changes to the job
					isExpert=JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum);
					isEngineer=JobPermissions.IsAuthorized(JobPerm.Engineer,true)&&(_jobCur.UserNumEngineer==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Request Change Approval",actionMenu_RequestChangeApprovalClick) { Enabled=isExpert||isEngineer });
					//If the user is the expert or engineer and there is at least one review that is in the SaveCommit status, the user may Save Commit the job
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.SaveCommit);
					actionMenu.MenuItems.Add(new MenuItem("Save Commit",actionMenu_SaveCommitClick) { Enabled=hasPermission });
					//If the user is the expert or engineer and there is at least one review that is in the Completed status, the user may mark the job as implemented
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Implemented",actionMenu_ImplementedClick) { Enabled=hasPermission });
					//If the user has approval permissions they can unapprove a job in this step
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Unapprove Job",actionMenu_UnapproveJobClick) { Enabled=true });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Development, IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Development, IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Development, IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Development, IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Development, IsApprovalNeeded):
					//If the user does not have approval permission and they are the current expert or engineer, they can repeal the approval request
					if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&(_jobCur.UserNumExpert==Security.CurUser.UserNum||_jobCur.UserNumEngineer==Security.CurUser.UserNum)) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					else {
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Approve Changes",actionMenu_ApproveChangeClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Documentation, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Documentation, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Documentation, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Documentation, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Documentation, !IsApprovalNeeded):
					//If the user has Notify Customer permissions and they have not yet contacted the customer, give them the option to email the customer
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)&&_jobCur.DateTimeCustContact.Year<1880) {
						actionMenu.MenuItems.Add(new MenuItem("Email Attached Customers",actionMenu_EmailAttachedClick) { Enabled=true });
					}
					//If the user has documentation permission they can assign documenters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true);
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumDocumenter==0 ? "A" : "Rea")+"ssign Documenter",actionMenu_AssignDocumenterClick) { Enabled=hasPermission });
					//If the user has documentation permission and they are the assigned documenter or there is currently no documenter assigned, then they can mark the job as documented
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true)&&(_jobCur.UserNumDocumenter==0||_jobCur.UserNumDocumenter==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Documented",actionMenu_DocumentedClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Enhancement, JobPhase.Complete, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Complete, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Complete, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Complete, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Complete, !IsApprovalNeeded):
					//If the user has Notify Customer permission, they can assign Contacters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumCustContact==0 ? "A" : "Rea")+"ssign Contacter",actionMenu_AssignContacterClick) { Enabled=hasPermission });
					//If the user has Notify Customer permission and they are the assigned Contacter or none is currently assigned, then they have permission to the following actions
					hasPermission=JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)&&(_jobCur.UserNumCustContact==0||_jobCur.UserNumCustContact==Security.CurUser.UserNum);
					if(_jobCur.DateTimeCustContact.Year<1880) {
						//If Uncontacted
						actionMenu.MenuItems.Add(new MenuItem("Email Attached Customers",actionMenu_EmailAttachedClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Mark as Contacted",actionMenu_ContactClick) { Enabled=hasPermission });
					}
					else {
						//If Contacted
						actionMenu.MenuItems.Add(new MenuItem("Mark as Uncontacted",actionMenu_UnContactClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Enhancement, JobPhase.Cancelled, !IsApprovalNeeded):
				case (JobCategory.Feature, JobPhase.Cancelled, !IsApprovalNeeded):
				case (JobCategory.HqRequest, JobPhase.Cancelled, !IsApprovalNeeded):
				case (JobCategory.InternalRequest, JobPhase.Cancelled, !IsApprovalNeeded):
				case (JobCategory.ProgramBridge, JobPhase.Cancelled, !IsApprovalNeeded):
					//If a user has approval permissions, then they can reopen cancelled jobs
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
					break;
				#endregion
				#endregion
				#region Jobs With Optional Approval
				#region Bug Jobs
				case (JobCategory.Bug, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has approval or concept permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestConceptApprovalClick) { Enabled=hasPermission });
					//Bugs are allowed to skip the approval process
					actionMenu.MenuItems.Add(new MenuItem("Send to In Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
					//Cancelling a concept is only allowed if you are the submitter
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.Bug, JobPhase.Concept, IsApprovalNeeded):
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					//If the user does not have approval permission and they are the current submitter, they can repeal the approval request
					if((!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&_jobCur.UserNumConcept==Security.CurUser.UserNum) || JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					//Bugs jobs can be sent directly to development
					//They do not need to be processed through the quote system
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Approve Concept",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Approve Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Concept",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Bug, JobPhase.Definition, !IsApprovalNeeded):
					//If the user has approval or writeup permissions, they can assign experts
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Writeup,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=hasPermission });
					//If the user has approval permissions or they have writeup permissions and they are the expert or none is assigned, then they can send the job for approval
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)
						||(JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum));
					//Once the approval process has begun, they cannot send the job for development without it going through the entire process
					actionMenu.MenuItems.Add(new MenuItem("Send for Approval",actionMenu_RequestJobApprovalClick) { Enabled=hasPermission });
					//If the user has approval permissions, they can unapprove the concept or cancel the job.
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Unapprove Concept",actionMenu_UnapproveJobClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Bug, JobPhase.Definition, IsApprovalNeeded):
					//If the user does not have approval permission and they are the current expert, they can repeal the approval request
					if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&_jobCur.UserNumExpert==Security.CurUser.UserNum) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					else {
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Approve Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Bug, JobPhase.Development, !IsApprovalNeeded):
					//If the user has approval or writeup permissions, they can assign experts
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Writeup,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					//If the user has approval or is the current expert permissions, they can assign engineers
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||_jobCur.UserNumExpert==Security.CurUser.UserNum||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=true });
					//If the job currently has no engineer then an engineer may take the job
					if(_jobCur.UserNumEngineer==0&&JobPermissions.IsAuthorized(JobPerm.Engineer,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Take Job",actionMenu_TakeJobClick) { Enabled=true });
					}
					//Only the assigned engineer may request a review.
					hasPermission=_jobCur.UserNumEngineer==Security.CurUser.UserNum;
					actionMenu.MenuItems.Add(new MenuItem("Request Review",actionMenu_RequestReviewClick) { Enabled=hasPermission });
					//If the user is an expert or an engineer assigned to the job then they can request approval changes to the job
					isExpert=JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum);
					isEngineer=JobPermissions.IsAuthorized(JobPerm.Engineer,true)&&(_jobCur.UserNumEngineer==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Request Change Approval",actionMenu_RequestChangeApprovalClick) { Enabled=isExpert||isEngineer });
					//If the user is the expert or engineer and there is at least one review that is in the SaveCommit status, the user may Save Commit the job
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.SaveCommit);
					actionMenu.MenuItems.Add(new MenuItem("Save Commit",actionMenu_SaveCommitClick) { Enabled=hasPermission });
					//If the user is the expert or engineer and there is at least one review that is in the Completed status, the user may mark the job as implemented
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Implemented",actionMenu_ImplementedClick) { Enabled=hasPermission });
					//If the user has approval permissions they can unapprove a job in this step
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
						actionMenu.MenuItems.Add(new MenuItem("Unapprove Job",actionMenu_UnapproveJobClick) { Enabled=true });
					}
					break;
				case (JobCategory.Bug, JobPhase.Development, IsApprovalNeeded):
					//If the user does not have approval permission and they are the current expert or engineer, they can repeal the approval request
					if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)&&(_jobCur.UserNumExpert==Security.CurUser.UserNum||_jobCur.UserNumEngineer==Security.CurUser.UserNum)) {
						actionMenu.MenuItems.Add(new MenuItem("Repeal Approval Request",actionMenu_RepealApprovalRequestClick) { Enabled=true });
					}
					else {
						hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
						actionMenu.MenuItems.Add(new MenuItem("Approve Changes",actionMenu_ApproveChangeClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Request Clarification",actionMenu_RequestClarificationClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Bug, JobPhase.Documentation, !IsApprovalNeeded):
					//If the user has Notify Customer permissions and they have not yet contacted the customer, give them the option to email the customer
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)&&_jobCur.DateTimeCustContact.Year<1880) {
						actionMenu.MenuItems.Add(new MenuItem("Email Attached Customers",actionMenu_EmailAttachedClick) { Enabled=true });
					}
					//If the user has documentation permission they can assign documenters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true);
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumDocumenter==0 ? "A" : "Rea")+"ssign Documenter",actionMenu_AssignDocumenterClick) { Enabled=hasPermission });
					//If the user has documentation permission and they are the assigned documenter or there is currently no documenter assigned, then they can mark the job as documented
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true)&&(_jobCur.UserNumDocumenter==0||_jobCur.UserNumDocumenter==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Documented",actionMenu_DocumentedClick) { Enabled=hasPermission });
					break;
				case (JobCategory.Bug, JobPhase.Complete, !IsApprovalNeeded):
					//If the user has Notify Customer permission, they can assign Contacters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumCustContact==0 ? "A" : "Rea")+"ssign Contacter",actionMenu_AssignContacterClick) { Enabled=hasPermission });
					//If the user has Notify Customer permission and they are the assigned Contacter or none is currently assigned, then they have permission to the following actions
					hasPermission=JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)&&(_jobCur.UserNumCustContact==0||_jobCur.UserNumCustContact==Security.CurUser.UserNum);
					if(_jobCur.DateTimeCustContact.Year<1880) {
						//If Uncontacted
						actionMenu.MenuItems.Add(new MenuItem("Email Attached Customers",actionMenu_EmailAttachedClick) { Enabled=hasPermission });
						actionMenu.MenuItems.Add(new MenuItem("Mark as Contacted",actionMenu_ContactClick) { Enabled=hasPermission });
					}
					else {
						//If Contacted
						actionMenu.MenuItems.Add(new MenuItem("Mark as Uncontacted",actionMenu_UnContactClick) { Enabled=hasPermission });
					}
					break;
				case (JobCategory.Bug, JobPhase.Cancelled, !IsApprovalNeeded):
					//If a user has approval permissions, then they can reopen cancelled jobs
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ApproveConceptClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Job",actionMenu_ApproveJobClick) { Enabled=hasPermission });
					break;
				#endregion
				#endregion
				#region Jobs Not Requiring Approval
				#region Conversion Jobs
				case (JobCategory.Conversion, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has approval or concept permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					//Conversions jobs go directly to in-development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send to In-Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.Conversion, JobPhase.Development, !IsApprovalNeeded):
					//Users with approval or concept permissions can reassign the expert of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					}
					//Users who have approval or who are marked as the expert can reassign the engineer of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||_jobCur.UserNumExpert==Security.CurUser.UserNum||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=true });
					}
					//If the user is an expert and there is no expert set or they are the current expert set
					isExpert=JobPermissions.IsAuthorized(JobPerm.Writeup,true) && (_jobCur.UserNumExpert==0 || _jobCur.UserNumExpert==Security.CurUser.UserNum);
					//If the user is an engineer and they are the currently assigned engineer
					isEngineer=JobPermissions.IsAuthorized(JobPerm.Engineer,true) && (_jobCur.UserNumEngineer==Security.CurUser.UserNum);
					//If the user is an expert or engineer and there is an engineer set
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0;
					actionMenu.MenuItems.Add(new MenuItem("Mark Complete",actionMenu_DirectlyComplete) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.Conversion, JobPhase.Complete, !IsApprovalNeeded):
					//Nothing in this section currently
					break;
				case (JobCategory.Conversion, JobPhase.Cancelled, !IsApprovalNeeded):
					//Users with concept permissions can reopen cancelled conversion jobs as concepts or reopen them for development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ReopenConceptNoApprovalClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen for Development",actionMenu_ReopenJobNoApprovalClick) { Enabled=hasPermission });
					break;
				#endregion
				#region NeedNoApproval Jobs
				case (JobCategory.NeedNoApproval, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has approval or concept permissions, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSubmitterClick) { Enabled=hasPermission });
					//NeedNoApproval jobs go directly to in-development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send to In-Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.NeedNoApproval, JobPhase.Development, !IsApprovalNeeded):
					//Users with approval or concept permissions can reassign the expert of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					}
					//Users who have approval or who are marked as the expert can reassign the engineer of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||_jobCur.UserNumExpert==Security.CurUser.UserNum||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=true });
					}
					//If the user is an expert and there is no expert set or they are the current expert set
					isExpert=JobPermissions.IsAuthorized(JobPerm.Writeup,true)&&(_jobCur.UserNumExpert==0||_jobCur.UserNumExpert==Security.CurUser.UserNum);
					//If the user is an engineer and they are the currently assigned engineer
					isEngineer=JobPermissions.IsAuthorized(JobPerm.Engineer,true)&&(_jobCur.UserNumEngineer==Security.CurUser.UserNum);
					//If the user is the expert or engineer and there is at least one review that is in the SaveCommit status, the user may Save Commit the job
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.SaveCommit);
					actionMenu.MenuItems.Add(new MenuItem("Save Commit",actionMenu_SaveCommitClick) { Enabled=hasPermission });
					//If the user is an expert or engineer, there is an engineer set, there is at least one review for the job, and the job has at least one review marked Done
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0&&_jobCur.ListJobReviews.Count>0&&_jobCur.ListJobReviews.Exists(x => x.ReviewStatus==JobReviewStatus.Done);
					actionMenu.MenuItems.Add(new MenuItem("Mark As Implemented",actionMenu_ImplementedClick) { Enabled=hasPermission });
					//If the user has ProjectManager permission or they are the original concept creator
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)||_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.NeedNoApproval, JobPhase.Documentation, !IsApprovalNeeded):
					if(JobPermissions.IsAuthorized(JobPerm.Documentation,true)) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumDocumenter==0 ? "A" : "Rea")+"ssign Documenter",actionMenu_AssignDocumenterClick) { Enabled=true });//x
					}
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true)&&(_jobCur.UserNumDocumenter==0||_jobCur.UserNumDocumenter==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Documented",actionMenu_DocumentedClick) { Enabled=hasPermission });
					break;
				case (JobCategory.NeedNoApproval, JobPhase.Complete, !IsApprovalNeeded):
					//Nothing in this section currently
					break;
				case (JobCategory.NeedNoApproval, JobPhase.Cancelled, !IsApprovalNeeded):
					//Users with ProjectManager permissions can reopen cancelled conversion jobs as concepts or reopen them for development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.ProjectManager,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ReopenConceptNoApprovalClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen for Development",actionMenu_ReopenJobNoApprovalClick) { Enabled=hasPermission });
					break;
				#endregion
				#region SpecialProject Jobs
				case (JobCategory.SpecialProject, JobPhase.Concept, !IsApprovalNeeded):
					//If the user has SpecialProject permission, they can assign submitters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.SpecialProject,true)||_isOverride;
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumConcept==0 ? "A" : "Rea")+"ssign Submitter",actionMenu_AssignSpecialProjectSubmitterClick) { Enabled=hasPermission });
					//Special Projects jobs go directly to in-development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send to In-Development",actionMenu_SendInDevelopmentClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;
				case (JobCategory.SpecialProject, JobPhase.Development, !IsApprovalNeeded):
					//Users with approval or concept permissions can reassign the expert of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||JobPermissions.IsAuthorized(JobPerm.Concept,true)||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumExpert==0 ? "A" : "Rea")+"ssign Expert",actionMenu_AssignExpertClick) { Enabled=true });
					}
					//Users who have approval or who are marked as the expert can reassign the engineer of a conversion job
					if(JobPermissions.IsAuthorized(JobPerm.Approval,true)||_jobCur.UserNumExpert==Security.CurUser.UserNum||_isOverride) {
						actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumEngineer==0 ? "A" : "Rea")+"ssign Engineer",actionMenu_AssignEngineerClick) { Enabled=true });
					}
					//If the user is an expert and there is no expert set or they are the current expert set
					isExpert=JobPermissions.IsAuthorized(JobPerm.Writeup,true) && (_jobCur.UserNumExpert==0 || _jobCur.UserNumExpert==Security.CurUser.UserNum);
					//If the user is an engineer and they are the currently assigned engineer
					isEngineer=JobPermissions.IsAuthorized(JobPerm.Engineer,true) && (_jobCur.UserNumEngineer==Security.CurUser.UserNum);
					//If the user is an expert or engineer and there is an engineer set
					hasPermission=(isExpert||isEngineer)&&_jobCur.UserNumEngineer>0;
					actionMenu.MenuItems.Add(new MenuItem("Mark Implemented",actionMenu_ImplementedClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission&&_jobCur.UserNumConcept==Security.CurUser.UserNum });
					break;				
				case (JobCategory.SpecialProject, JobPhase.Documentation, !IsApprovalNeeded):
					//If the user has Notify Customer permissions and they have not yet contacted the customer, give them the option to email the customer
					if(JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)&&_jobCur.DateTimeCustContact.Year<1880) {
						actionMenu.MenuItems.Add(new MenuItem("Email Attached Customers",actionMenu_EmailAttachedClick) { Enabled=true });
					}
					//If the user has documentation permission they can assign documenters
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true);
					actionMenu.MenuItems.Add(new MenuItem((_jobCur.UserNumDocumenter==0 ? "A" : "Rea")+"ssign Documenter",actionMenu_AssignDocumenterClick) { Enabled=hasPermission });
					//If the user has documentation permission and they are the assigned documenter or there is currently no documenter assigned, then they can mark the job as documented
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Documentation,true)&&(_jobCur.UserNumDocumenter==0||_jobCur.UserNumDocumenter==Security.CurUser.UserNum);
					actionMenu.MenuItems.Add(new MenuItem("Mark as Documented",actionMenu_DocumentedClick) { Enabled=hasPermission });
					break;
				case (JobCategory.SpecialProject, JobPhase.Complete, !IsApprovalNeeded):
					//Nothing in this section currently
					break;
				case (JobCategory.SpecialProject, JobPhase.Cancelled, !IsApprovalNeeded):
					//Users with concept permissions can reopen cancelled conversion jobs as concepts or reopen them for development
					hasPermission=JobPermissions.IsAuthorized(JobPerm.SpecialProject,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen as Concept",actionMenu_ReopenConceptNoApprovalClick) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Reopen for Development",actionMenu_ReopenJobNoApprovalClick) { Enabled=hasPermission });
					break;
				#endregion
				#region UnresolvedIssue Jobs
				case (JobCategory.UnresolvedIssue, JobPhase.Concept, !IsApprovalNeeded):
					//The primary phase for Unresolved Issues. Jobs will be either cancelled or turned into Bug/Enhancement jobs from here
					//Concept permissions allow users to create jobs from Unresolved Issues.
					hasPermission=JobPermissions.IsAuthorized(JobPerm.Concept,true);
					actionMenu.MenuItems.Add(new MenuItem("Send To Bugs",actionMenu_UnresolvedIssueToBugs) { Enabled=hasPermission });
					actionMenu.MenuItems.Add(new MenuItem("Send To Enhancements",actionMenu_UnresolvedIssueToEnhancement) { Enabled=hasPermission });
					//UnresolvedIssues permissions allow users to cancel Unresolved Issues jobs.
					hasPermission=JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true);
					actionMenu.MenuItems.Add(new MenuItem("Cancel Job",actionMenu_CancelJobClick) { Enabled=hasPermission });
					break;
				case (JobCategory.UnresolvedIssue, JobPhase.Cancelled, !IsApprovalNeeded):
					//Cancelled Unresolved Issues occur when the issue was solved without a commit or if it was solved by another job.
					hasPermission=JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true);
					actionMenu.MenuItems.Add(new MenuItem("Reopen Issue",actionMenu_UnresolvedIssueReopen) { Enabled=hasPermission });
					break;
					#endregion
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

		///<summary>Functions as a toggle for setting jobs active.</summary>
		private void checkIsActive_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			IsChanged=true;
		}
			
		private bool ValidateJob(Job _jobCur,bool doRequirementCheck=true) {
			textJobEditor.SetListJobRequirements(textJobEditor.GetListJobRequirements().FindAll(x => !String.IsNullOrEmpty(x.Description)));
			if(string.IsNullOrWhiteSpace(_jobCur.Title)) {
				MessageBox.Show("Invalid Title.");
				return false;
			}
			if(_jobCur.Category==JobCategory.Bug
				&& !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)
				&& ListTools.In(_jobCur.PhaseCur,JobPhase.Definition,JobPhase.Development))
			{
				MsgBox.Show(this,"Bug jobs must have an attached bug.");
				return false;
			}
			if(_jobCur.Priority==0) {
				MsgBox.Show(this,"Please select a priority before saving the job.");
				return false;
			}
			if(doRequirementCheck
				//All of the listed job categories don't need requirements
				&& !ListTools.In(_jobCur.Category,JobCategory.Bug,JobCategory.Query,JobCategory.MarketingDesign,JobCategory.UnresolvedIssue,JobCategory.SpecialProject,JobCategory.Conversion) 
				&& textJobEditor.GetListJobRequirements()!=null
				&& textJobEditor.GetListJobRequirements().Count()==0)
			{
				MsgBox.Show(this,"Please add at least one requirement for the job. There must be no empty requirements in the list.");
				return false;
			}
			//Validation portion is complete, now we need to make sure that NeedsRequirementReapproval is invoked before SaveJob can be invoked.
			if(NeedsRequirementReapproval()) {
				SaveJob(_jobCur);//The above method just manipulated _jobCur so make sure to save the changes.
				return false;
			}
			return true;
		}

		///<summary>Evaluates the list of requirements and detects any description changes.
		///If changes are detected then this method gives the option to send the job to reapproval.
		///If ValidateJob was called before SaveJob then this must be called directly prior to SaveJob.</summary>
		private bool NeedsRequirementReapproval() {
			if(JobPermissions.IsAuthorized(JobPerm.Approval,true)       //Users with the Approval job permission never need reapproval.
				|| ListTools.In(_jobCur.Category,JobCategory.Bug,JobCategory.Query,JobCategory.MarketingDesign,JobCategory.Conversion,JobCategory.NeedNoApproval) //Bug, Query, Conversion, and NeedNoApproval jobs do not need requirement approval at all.
				|| ListTools.In(_jobCur.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote))  //Concepts, Definitions, and Quotes can change requirements without reapproval.
			{
				return false;
			}
			//At this point we know that _jobCur is in a Category and Phase where any changes to the Requirments List will need reapproval.
			textJobEditor.SetListJobRequirements(textJobEditor.GetListJobRequirements().FindAll(x => !String.IsNullOrEmpty(x.Description)));
			List<JobRequirement> listRequirementsOld=JsonConvert.DeserializeObject<List<JobRequirement>>(_jobCur.RequirementsJSON);
			bool hasRequirementChanges=(listRequirementsOld==null || !(listRequirementsOld.Count==textJobEditor.GetListJobRequirements().Count));
			//The number of requirements did not change but the descriptions might have.
			if(!hasRequirementChanges) {
				for(int i=0;i<listRequirementsOld.Count();i++) {
					if(listRequirementsOld[i].Description!=textJobEditor.GetListJobRequirements()[i].Description) {
						hasRequirementChanges=true;
						break;
					}
				}
			}
			if(hasRequirementChanges) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Editing the requirements for this job will require reapproval. Continue?")) {
					//Revert the requirements list to the way it was prior to the new changes.
					textJobEditor.SetListJobRequirements(listRequirementsOld??new List<JobRequirement>());
					return false;
				}
				//Set the job back to the Concept phase for reapproval.
				IsChanged=true;
				if(_jobCur.PhaseCur==JobPhase.Development) {
					_jobCur.IsApprovalNeeded=true;
					_jobCur.UserNumApproverChange=0;
				}
				else {
					_jobCur.PhaseCur=JobPhase.Concept;
					_jobCur.IsApprovalNeeded=false;
					_jobCur.UserNumApproverConcept=0;
					_jobCur.UserNumApproverJob=0;
					_jobCur.UserNumApproverChange=0;
					_jobCur.UserNumExpert=0;
					_jobCur.UserNumEngineer=0;
				}
				return true;
			}
			return false;
		}

		#region ACTION BUTTON MENU ITEMS //====================================================
		#region Bug Actions
		private void actionMenu_SendWriteupClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.Category==JobCategory.Bug && !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
				if(!AddBug()) {
					return;
				}
			}
			long userNumExpert = _jobCur.UserNumExpert;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumExpert,JobPermissions.IsAuthorized(JobPerm.Writeup,true,Security.CurUser.UserNum),false)) {
				return;
			}
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.PhaseCur=JobPhase.Definition;
			SaveJob(_jobCur);
		}

		private void actionMenu_SendInDevelopmentClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.Category==JobCategory.Bug && !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
				if(!AddBug()) {
					return;
				}
			}
			if(_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Request)) {
				FeatureRequests.MarkAsInProgress(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList());
			}
			long userNumExpert=_jobCur.UserNumExpert;
			long userNumEngineer=_jobCur.UserNumEngineer;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumExpert>0 ? _jobCur.UserNumExpert : _jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,_jobCur.UserNumEngineer,true,false)) {
				return;
			}
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.PhaseCur=JobPhase.Development;
			if(_jobCur.PatternReviewProject==JobPatternReviewProject.OD) {
				_jobCur.PatternReviewStatus=JobPatternReviewStatus.AwaitingApproval;
			}
			SaveJob(_jobCur);
		}
		
		private void actionMenu_UnresolvedIssueToBugs(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(!_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
				if(!AddBug()) {
					return;
				}
			}
			long userNumExpert=_jobCur.UserNumExpert;
			long userNumEngineer=_jobCur.UserNumEngineer;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumExpert>0 ? _jobCur.UserNumExpert : _jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,_jobCur.UserNumEngineer,true,false)) {
				return;
			}
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.UserNumConcept=Security.CurUser.UserNum;
			_jobCur.Category=JobCategory.Bug;
			if(_jobCur.PatternReviewProject==JobPatternReviewProject.OD) {
				_jobCur.PatternReviewStatus=JobPatternReviewStatus.AwaitingApproval;
			}
			SaveJob(_jobCur);
		}

		private void actionMenu_UnresolvedIssueToEnhancement(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumConcept=Security.CurUser.UserNum;
			_jobCur.Category=JobCategory.Enhancement;
			SaveJob(_jobCur);
		}

		private void actionMenu_UnresolvedIssueReopen(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Concept;
			SaveJob(_jobCur);
		}
		#endregion
		#region Assign Users
		private void actionMenu_AssignDocumenterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumDocumenter;
			if(!PickUserByJobPermission("Pick Documenter",JobPerm.Documentation,out userNumDocumenter,_jobCur.UserNumDocumenter,true,false)) {
				return;
			}
			if(userNumDocumenter==_jobOld.UserNumDocumenter) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumDocumenter=userNumDocumenter;
			SaveJob(_jobCur);
		}

		private void actionMenu_ResearchAssignSubmitterClick(object sender,EventArgs e) {
			AssignSubmitter();
		}

		private void actionMenu_AssignSubmitterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur,false)) {
				return;
			}
			long userNumConcept;
			if(!PickUserByJobPermission("Pick Submitter",JobPerm.Concept,out userNumConcept,_jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(userNumConcept==_jobOld.UserNumConcept) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumConcept=userNumConcept;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignSubmitterNeedNoApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur,false)) {
				return;
			}
			long userNumConcept;
			if(!PickUserByJobPermission("Pick Submitter",JobPerm.ProjectManager,out userNumConcept,_jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(userNumConcept==_jobOld.UserNumConcept) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumConcept=userNumConcept;
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignSpecialProjectSubmitterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur,false)) {
				return;
			}
			long userNumConcept;
			if(!PickUserByJobPermission("Pick Submitter",JobPerm.SpecialProject,out userNumConcept,_jobCur.UserNumConcept,false,false)) {
				return;
			}
			if(userNumConcept==_jobOld.UserNumConcept) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumConcept=userNumConcept;
			SaveJob(_jobCur);
		}

		private void AssignSubmitter() {
			if(!ValidateJob(_jobCur,false)) {
				return;
			}
			long userNumConcept;
			if(!PickUserByJobPermission("Pick Submitter",JobPerm.Concept,out userNumConcept,_jobCur.UserNumConcept,false,false)) {
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
			long userNumExpert;
			if(!PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumExpert,true,false)) {
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
			long userNumEngineer;
			if(!PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,_jobCur.UserNumEngineer,true,false)) {
				return;
			}
			if(userNumEngineer==_jobOld.UserNumEngineer) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumEngineer=userNumEngineer;
			SaveJob(_jobCur);
		}

		private void actionMenu_TakeJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			_jobCur.UserNumEngineer=Security.CurUser.UserNum;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestReviewClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert;
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumExpert,false,false)) {
				return;
			}
			IsChanged=true;
			JobReview jobReview = new JobReview();
			jobReview.JobNum=_jobCur.JobNum;
			jobReview.ReviewerNum=userNumExpert;//can be zero
			jobReview.ReviewStatus=JobReviewStatus.Sent;
			_jobCur.ListJobReviews.Add(jobReview);
			SaveJob(_jobCur);
		}

		private void actionMenu_AssignContacterClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumContact;
			if(!PickUserByJobPermission("Pick Contacter",JobPerm.NotifyCustomer,out userNumContact,_jobCur.UserNumCustContact,true,false)) {
				return;
			}
			if(userNumContact==_jobOld.UserNumCustContact) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumCustContact=userNumContact;
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
			_jobCur.UserNumExpert=0;
			_jobCur.UserNumEngineer=0;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestConceptApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.Category==JobCategory.Feature && _jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Count==0) {
				if(!AddFeatureRequest()) {
					return;
				}
			}
			if(_jobCur.HoursEstimate==0) {
				using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
				if(FormJE.ShowDialog()!=DialogResult.OK || _jobCur.HoursEstimate==0) {
					return;
				}
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			IsChanged=true;
			_jobCur.UserNumConcept=Security.CurUser.UserNum;
			_jobCur.IsApprovalNeeded=true;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestJobApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.Category==JobCategory.Feature && !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Request)) {
				if(!AddFeatureRequest()) {
					return;
				}
			}
			if(_jobCur.Category==JobCategory.Bug && !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
				if(!AddBug()) {
					return;
				}
			}
			if(_jobCur.HoursEstimate==0) {
				using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
				if(FormJE.ShowDialog()!=DialogResult.OK || _jobCur.HoursEstimate==0) {
					return;
				}
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=true;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestChangeApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.HoursEstimate==0) {
				using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
				if(FormJE.ShowDialog()!=DialogResult.OK || _jobCur.HoursEstimate==0) {
					return;
				}
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			IsChanged=true;
			_jobOld.UserNumApproverChange=0;//in case it was previously set.
			_jobCur.IsApprovalNeeded=true;
			_jobCur.DateTimeJobApproval=DateTime.Now;
			SaveJob(_jobCur);
		}
		#endregion
		#region Approval Options
		private void actionMenu_RequestClarificationClick(object sender,EventArgs e) {
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			if(_jobCur.PhaseCur==JobPhase.Concept) {
				_jobCur.UserNumExpert=0;
				_jobCur.UserNumApproverConcept=0;
			}
			if(_jobCur.PhaseCur==JobPhase.Development) {
				//This happens only when a change request is made. This process should be enhanced so that when an approver denies a change request, 
				//the job is reverted to its previous version, instead of requiring the Expert to manually undo the changes and get the job re-approved.
				_jobCur.PhaseCur=JobPhase.Definition;
				_jobCur.UserNumApproverJob=0;
				_jobCur.UserNumApproverChange=0;
			}
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestConceptClarificationClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumExpert=0;
			_jobCur.UserNumApproverConcept=0;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestDefinitionClarificationClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			SaveJob(_jobCur);
		}

		private void actionMenu_ApproveConceptClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumConcept,true,false)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverConcept=Security.CurUser.UserNum;
			_jobCur.UserNumApproverJob=0;//in case it was previously set.
			_jobCur.UserNumApproverChange=0;//in case it was previously set.
			_jobCur.PhaseCur=JobPhase.Definition;
			_jobCur.DateTimeConceptApproval=DateTime.Now;
			SaveJob(_jobCur);
			JobLogs.MakeLogEntryForRequirementApproval(_jobCur);
		}

		private void actionMenu_ReopenConceptNoApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumConcept,true,false)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverConcept=0;//in case it was previously set.
			_jobCur.UserNumApproverJob=0;//in case it was previously set.
			_jobCur.UserNumApproverChange=0;//in case it was previously set.
			_jobCur.PhaseCur=JobPhase.Concept;
			SaveJob(_jobCur);
			JobLogs.MakeLogEntryForPhase(_jobCur,_jobOld);
		}

		private void actionMenu_RequestQuoteClick(object sender,EventArgs e) {
			long userNumQuoter = _jobCur.UserNumQuoter;
			if(_jobCur.UserNumQuoter==0 && !PickUserByJobPermission("Pick Quoter",JobPerm.Quote,out userNumQuoter,0,false,false)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumQuoter=userNumQuoter;
			_jobCur.UserNumApproverQuote=0;
			_jobCur.UserNumCustQuote=0;
			_jobCur.PhaseCur=JobPhase.Quote;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestQuoteApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=true;
			SaveJob(_jobCur);
		}

		private void actionMenu_ApproveQuoteClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumCustQuote = _jobCur.UserNumCustQuote;
			if(_jobCur.UserNumCustQuote==0 && !PickUserByJobPermission("Pick Customer Contact",JobPerm.NotifyCustomer,out userNumCustQuote,_jobCur.UserNumConcept,false,false)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumCustQuote=userNumCustQuote;
			_jobCur.UserNumApproverQuote=Security.CurUser.UserNum;
			SaveJob(_jobCur);
		}

		private void actionMenu_RequestCustQuoteApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=true;
			_jobCur.PhaseCur=JobPhase.Concept;
			SaveJob(_jobCur);
		}

		private void actionMenu_ApproveJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumConcept,true,false)) {
				return;
			}
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,userNumExpert,true,false)) {
				return;
			}
			if(_jobCur.UserNumApproverConcept==0) {
				if(_jobCur.HoursEstimate==0) {
					using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
					if(FormJE.ShowDialog()!=DialogResult.OK || _jobCur.HoursEstimate==0) {
						return;
					}
					textEstHours.Text=_jobCur.HoursEstimate.ToString();
					SetHoursLeft();
				}
				_jobCur.UserNumApproverConcept=Security.CurUser.UserNum;
			}
			if(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Count!=0) {
				FeatureRequests.MarkAsInProgress(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList());
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverJob=Security.CurUser.UserNum;
			_jobCur.UserNumApproverChange=0;//in case it was previously set.
			_jobCur.PhaseCur=JobPhase.Development;
			if(_jobCur.PatternReviewProject==JobPatternReviewProject.OD) {
				_jobCur.PatternReviewStatus=JobPatternReviewStatus.AwaitingApproval;
			}
			_jobCur.DateTimeJobApproval=DateTime.Now;
			SaveJob(_jobCur);
		}

		private void actionMenu_ReopenJobNoApprovalClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumConcept,true,false)) {
				return;
			}
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,userNumExpert,true,false)) {
				return;
			}
			if(_jobCur.UserNumApproverConcept==0) {
				if(_jobCur.HoursEstimate==0) {
					using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
					if(FormJE.ShowDialog()!=DialogResult.OK || _jobCur.HoursEstimate==0) {
						return;
					}
					textEstHours.Text=_jobCur.HoursEstimate.ToString();
					SetHoursLeft();
				}
				_jobCur.UserNumApproverConcept=0;//No Approval
			}
			if(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Count!=0) {
				FeatureRequests.MarkAsInProgress(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList());
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverJob=0;
			_jobCur.UserNumApproverChange=0;
			_jobCur.PhaseCur=JobPhase.Development;
			if(_jobCur.PatternReviewProject==JobPatternReviewProject.OD) {
				_jobCur.PatternReviewStatus=JobPatternReviewStatus.AwaitingApproval;
			}
			SaveJob(_jobCur);
		}

		private void actionMenu_ApproveChangeClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			long userNumExpert = _jobCur.UserNumExpert;
			long userNumEngineer = _jobCur.UserNumEngineer;
			if(_jobCur.UserNumExpert==0 && !PickUserByJobPermission("Pick Expert",JobPerm.Writeup,out userNumExpert,_jobCur.UserNumConcept,true,false)) {
				return;
			}
			if(_jobCur.UserNumEngineer==0 && !PickUserByJobPermission("Pick Engineer",JobPerm.Engineer,out userNumEngineer,userNumExpert,true,false)) {
				return;
			}
			IsChanged=true;
			_jobCur.UserNumExpert=userNumExpert;
			_jobCur.UserNumEngineer=userNumEngineer;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.UserNumApproverChange=Security.CurUser.UserNum;
			_jobCur.PhaseCur=JobPhase.Development;
			SaveJob(_jobCur);
		}

		private void actionMenu_CancelJobClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
				MsgBox.Show(this,"You cannot cancel a job that has a bug attached. Please detach the bug first before canceling the job.");
				return;
			}
			IsChanged=true;
			_jobCur.UserNumInfo=0;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Cancelled;
			SaveJob(_jobCur);
		}
		#endregion

		private void actionMenu_EmailAttachedClick(object sender,EventArgs e) {
			using FormEmailJobs FormEJ = new FormEmailJobs();
			FormEJ.JobCur=_jobCur.Copy();
			FormEJ.ShowDialog();
			if(FormEJ.DialogResult!=DialogResult.OK) {
				return;
			}
			_jobCur.DateTimeCustContact=MiscData.GetNowDateTime();
			_jobCur.UserNumCustContact=Security.CurUser.UserNum;
			_jobOld.DateTimeCustContact=MiscData.GetNowDateTime();
			_jobOld.UserNumCustContact=Security.CurUser.UserNum;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.DateTimeCustContact=MiscData.GetNowDateTime();
				job.UserNumCustContact=Security.CurUser.UserNum;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			SaveJob(_jobCur);
		}

		private void actionMenu_DirectlyComplete(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(!CheckJobLinks()) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			_jobCur.PhaseCur=JobPhase.Complete;
			SaveJob(_jobCur);
		}

		private void actionMenu_SaveCommitClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(_jobCur.ListJobReviews.Where(x => x.ReviewStatus==JobReviewStatus.SaveCommit).Count()==0) {
				MsgBox.Show(this,"A review of status \"Save Commit\" does not exist.");
				return;
			}
			//Don't check job links for a Save Commit
			//if(!CheckJobLinks()) {
			//	return;
			//}
			//Save Commits are for the Head Only!
			textVersion.Text=VersionReleases.GetPossibleHeadRelease();
			_jobCur.JobVersion=textVersion.Text;
			string description = "";
			description=_jobCur.Category.ToString().Substring(0,1)+_jobCur.JobNum+" - (Save Commit) "+_jobCur.Title;
			string reviewers = String.Join(", ",_jobCur.ListJobReviews.Where(x => x.ReviewStatus==JobReviewStatus.SaveCommit).Select(x => Userods.GetName(x.ReviewerNum)).Distinct().ToList());
			string logMsg = "";
			logMsg=POut.String(description)+"\r\nCommitted to: "+POut.String(textVersion.Text)+"\r\nReviewed by: "+POut.String(reviewers);
			using FormCommitPrompt FormCP=new FormCommitPrompt(0,logMsg);
			FormCP.ShowDialog();
			int commitVal=FormCP.GetCommitValue();
			string pathCommit = "";
			string pathCommitInternal = "";
			switch(System.Environment.MachineName) {
				default:
					pathCommit=@"C:\development\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"C:\development\Shared Projects Subversion";
					break;
				case "JORDANS":
					pathCommit=@"E:\Documents\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"E:\development\Shared Projects Subversion";
					break;
				case "CAMERON":
				case "JASON":
				case "DEREK":
				case "RYAN":
				case "RYAN1":
				case "MICHAEL":
				case "TRAVIS":
					pathCommit=@"C:\development\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"C:\development\Shared Projects Subversion";
					break;
			}
			Process process = new Process();
			string arguments = "/command:commit /path:\""+pathCommit+"\" /logmsg:\""+logMsg+"\"";
			ProcessStartInfo startInfo = new ProcessStartInfo("TortoiseProc.exe",arguments);
			if(FormCP.DialogResult==DialogResult.OK && (commitVal==1 || commitVal==3)) {//Public Repo or Both
				process.StartInfo=startInfo;
				process.Start();
				process.WaitForExit();
			}
			if(FormCP.DialogResult==DialogResult.OK && (commitVal==2 || commitVal==3)) {//Internal Repo or Both
				process=new Process();
				arguments="/command:commit /path:\""+pathCommitInternal+"\" /logmsg:\""+logMsg+"\"";
				startInfo=new ProcessStartInfo("TortoiseProc.exe",arguments);
				process.StartInfo=startInfo;
				process.Start();
				process.WaitForExit();
			}
			//Set all the save commit reviews to save committed.
			_jobCur.ListJobReviews.Where(x => x.ReviewStatus==JobReviewStatus.SaveCommit).ToList().ForEach(x => x.ReviewStatus=JobReviewStatus.SaveCommitted);
			JobLogs.MakeLogEntryForSaveCommit(_jobCur);
			IsChanged=true;
			//Does not change phase
			//_jobCur.PhaseCur=JobPhase.Documentation;
			SaveJob(_jobCur);
		}

		private void actionMenu_ImplementedClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur) || !CheckJobLinks()) {
				return;
			}
			if(string.IsNullOrEmpty(textActualHours.Text) || textActualHours.Text=="0") {
				if(!AddTime()) {
					return;
				}
			}
			bool isHeadOnlyCommit=true;//True for conversions
			bool isUnversioned=true;//True for conversions
			if(_jobCur.Category==JobCategory.Conversion) {
				textVersion.Text="Unversioned";
				_jobCur.JobVersion="Unversioned";
			}
			else {
				using FormVersionPrompt FormVP=new FormVersionPrompt("",true);
				FormVP.VersionText=textVersion.Text;
				FormVP.ShowDialog();
				if(FormVP.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(FormVP.VersionText)) {
					return;
				}
				isHeadOnlyCommit = FormVP.IsHeadOnly;
				isUnversioned = FormVP.IsUnversioned;
				if(!isHeadOnlyCommit && !isUnversioned && !_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"All backported jobs must have a bug attached. Would you like to add one?")) {
						return;
					}
					if(!AddBug() || !CheckJobLinks()) {
						return;
					}
				}
				textVersion.Text=FormVP.VersionText;
				_jobCur.JobVersion=FormVP.VersionText;
			}
			if(_jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug)
					&& !_jobCur.ListJobLinks.Where(x => x.LinkType==JobLinkType.Bug).All(x => Bugs.GetOne(x.FKey).VersionsFixed==_jobCur.JobVersion)) 
			{
				MsgBox.Show(this,"The job version and the bug fixed version are mismatched. Make sure the versions are the same and in the same order before sending the job to documentation.");
				return;
			}
			if(string.IsNullOrEmpty(textEditorDocumentation.MainText) 
				&& _jobCur.Category!=JobCategory.Conversion) 
			{
				using InputBox inBoxDocumentation = new InputBox("Please add a brief description of the job for documentation (including simple step by step usage instructions).",true);
				inBoxDocumentation.Text="Documentation Summary";
				inBoxDocumentation.ShowDialog();
				if(inBoxDocumentation.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(inBoxDocumentation.textResult.Text)) {
					return;
				}
				textEditorDocumentation.MainText=Security.CurUser.UserName+" - "+DateTime.Now.ToString()+"\r\n"+inBoxDocumentation.textResult.Text;
			}
			using InputBox inBoxTesting = new InputBox("Please choose a new priority for testing.",_listPrioritiesAll.Select(x => x.ItemName).ToList()
				,_listPrioritiesAll.FindIndex(x => x.ItemValue.Contains("JobDefault")));
			inBoxTesting.Text="Testing Priority";
			inBoxTesting.ShowDialog();
			if(inBoxTesting.DialogResult!=DialogResult.OK || inBoxTesting.SelectedIndex==-1) {
				_jobCur.PriorityTesting=_listPrioritiesAll.FirstOrDefault(x => x.ItemValue.Contains("JobDefault")).DefNum;
			}
			else {
				_jobCur.PriorityTesting=_listPrioritiesAll[inBoxTesting.SelectedIndex].DefNum;
			}
			comboPriorityTesting.SetSelectedDefNum(_jobCur.PriorityTesting);
			string description = "";
			if(_jobCur.Category==JobCategory.Bug) {
				JobLink jobLink=_jobCur.ListJobLinks.First(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug);
				string bugDescription="";
				if(jobLink.LinkType==JobLinkType.Bug) {
					bugDescription=Bugs.GetOne(jobLink.FKey).Description.Replace("\"","");
				}
				else {
					bugDescription=MobileBugs.GetOne(jobLink.FKey).Description.Replace("\"","");
				}
				description=_jobCur.Category.ToString().Substring(0,1)+_jobCur.JobNum+" - "+bugDescription;
			}
			else {
				description=_jobCur.Category.ToString().Substring(0,1)+_jobCur.JobNum+" - "+_jobCur.Title;
			}
			string reviewers = String.Join(", ",_jobCur.ListJobReviews.Where(x => x.ReviewStatus==JobReviewStatus.Done || x.ReviewStatus==JobReviewStatus.NeedsAdditionalReview).Select(x => Userods.GetName(x.ReviewerNum)).ToList());
			string logMsg = "";
			if(isHeadOnlyCommit) {
				logMsg=POut.String(description)+"\r\nCommitted to: "+POut.String(textVersion.Text)+"\r\nReviewed by: "+POut.String(reviewers);
			}
			else {
				logMsg=POut.String(description)+"\r\nBackported to: "+POut.String(textVersion.Text)+"\r\nReviewed by: "+POut.String(reviewers);
			}
			int repository=0;
			if(_jobCur.Category==JobCategory.Conversion) {
				repository=2;//Internal Repo
			}
			using FormCommitPrompt FormCP=new FormCommitPrompt(repository,logMsg);
			FormCP.ShowDialog();
			int commitVal=FormCP.GetCommitValue();
			string pathCommit = "";
			string pathCommitInternal = "";
			switch(System.Environment.MachineName) {
				default:
					pathCommit=@"C:\development\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"C:\development\Shared Projects Subversion";
					break;
				case "JORDANS":
					pathCommit=@"E:\Documents\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"E:\development\Shared Projects Subversion";
					break;
				case "CAMERON":
				case "JASON":
				case "DEREK":
				case "RYAN":
				case "RYAN1":
				case "MICHAEL":
				case "TRAVIS":
					pathCommit=@"C:\development\OPEN DENTAL SUBVERSION";
					pathCommitInternal=@"C:\development\Shared Projects Subversion";
					break;
			}
			Process process = new Process();
			string arguments = "/command:commit /path:\""+pathCommit+"\" /logmsg:\""+logMsg+"\"";
			ProcessStartInfo startInfo = new ProcessStartInfo("TortoiseProc.exe",arguments);
			if(FormCP.DialogResult==DialogResult.OK && (commitVal==1 || commitVal==3)) {//Public Repo or Both
				process.StartInfo=startInfo;
				process.Start();
				process.WaitForExit();
			}
			if(FormCP.DialogResult==DialogResult.OK && (commitVal==2 || commitVal==3)) {//Internal Repo or Both
				process=new Process();
				arguments="/command:commit /path:\""+pathCommitInternal+"\" /logmsg:\""+logMsg+"\"";
				startInfo=new ProcessStartInfo("TortoiseProc.exe",arguments);
				process.StartInfo=startInfo;
				process.Start();
				process.WaitForExit();
			}
			_jobCur.Priority=_listPrioritiesAll.FirstOrDefault(x => x.ItemValue.Contains("DocumentationDefault")).DefNum;
			comboPriority.SetSelectedDefNum(_jobCur.Priority);
			IsChanged=true;
			if(_jobCur.Category==JobCategory.Conversion) {
				_jobCur.PhaseCur=JobPhase.Complete;//Conversion Jobs go directly to complete.
			}
			else {
				_jobCur.PhaseCur=JobPhase.Documentation;
			}
			_jobCur.DateTimeImplemented=DateTime.Now;
			SaveJob(_jobCur);
		}

		private void actionMenu_DocumentedClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will mark the job as documented. Do you wish to continue?")) {
				return;
			}
			IsChanged=true;
			_jobCur.PhaseCur=JobPhase.Complete;
			if(_jobCur.UserNumDocumenter==0) {
				_jobCur.UserNumDocumenter=Security.CurUser.UserNum;
			}
			SaveJob(_jobCur);
		}

		private void actionMenu_ContactClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			if(_jobCur.UserNumCustContact==0) {
				_jobCur.UserNumCustContact=Security.CurUser.UserNum;
			}
			_jobCur.DateTimeCustContact=MiscData.GetNowDateTime();
			SaveJob(_jobCur);
		}

		private void actionMenu_UnContactClick(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			if(_jobCur.UserNumCustContact!=0) {
				_jobCur.UserNumCustContact=0;
			}
			_jobCur.DateTimeCustContact=DateTime.MinValue;
			SaveJob(_jobCur);
		}

		private void actionMenu_OverrideClick(object sender,EventArgs e) {
			IsOverride=true;
		}

		private void actionMenu_ViewLogsClick(object sender,EventArgs e) {
			FormJobLogs FormJL=new FormJobLogs(_jobCur);
			FormJL.Show();
		}

		#endregion ACTION BUTTON MENU ITEMS //=================================================

		private void butPopout_Click(object sender,EventArgs e) {
			if(!ValidateJob(_jobCur)) {
				return;//Job failed validation
			}
			SaveJob(_jobCur);
			FormJobManager.OpenNonModalJob(_jobCur);
		}

		///<summary>Deprecated: Show an input box to allow the user to choose a project for the job. Returns JobPatternReviewProject.None if the user hits cancel.</summary>
		private JobPatternReviewProject ShowProjectMessage() {
			List<string> listProjectNames=Enum.GetNames(typeof(JobPatternReviewProject)).Where(x => x!=JobPatternReviewProject.None.ToString()).ToList();
			using InputBox projectSelection=new InputBox("Choose a project for this job",listProjectNames);
			if(projectSelection.ShowDialog()!=DialogResult.OK) {
				return JobPatternReviewProject.None;
			}
			return (JobPatternReviewProject)(projectSelection.SelectedIndex+1);//+1 added since None is removed as an option
		}

		///<summary>If returns false if selection is cancelled. DefaultUserNum is usually the currently set usernum for a given role.</summary>
		private bool PickUserByJobPermission(string prompt,JobPerm jobPerm,out long selectedNum,long suggestedUserNum=0,bool AllowNone=true,bool AllowAll=true) {
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
			//All changes below this point will be lost if there is a conflicting change detected.
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
				else {
					
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
			//DOCUMENTATION
			if(_jobCur.Documentation!=jobMerge.Documentation) {
				if(textEditorDocumentation.MainRtf==_jobOld.Documentation || string.IsNullOrWhiteSpace(_jobOld.Documentation)) {//Was edited, AND user has not already edited it themselves.
					_jobCur.Documentation=jobMerge.Documentation;
					_jobOld.Documentation=jobMerge.Documentation;
					try {
						textEditorDocumentation.MainRtf=_jobCur.Documentation;
					}
					catch {
						textEditorDocumentation.MainText=_jobCur.Documentation;
					}
				}
				else {
					//MessageBox.Show("Job Documentation has been changed.");//possibly implement locking the documentation pane.
				}
			}
			//PRIORITY
			if(_jobCur.Priority!=jobMerge.Priority) {
				_jobCur.Priority=jobMerge.Priority;
				_jobOld.Priority=jobMerge.Priority;
				comboPriority.SetSelectedDefNum(_jobCur.Priority);
			}
			//PRIORITY TESTING
			if(_jobCur.PriorityTesting!=jobMerge.PriorityTesting) {
				_jobCur.PriorityTesting=jobMerge.PriorityTesting;
				_jobOld.PriorityTesting=jobMerge.PriorityTesting;
				comboPriorityTesting.SetSelectedDefNum(_jobCur.PriorityTesting);
			}
			//STATUS
			if(_jobCur.PhaseCur!=jobMerge.PhaseCur) {
				_jobCur.PhaseCur=jobMerge.PhaseCur;
				_jobOld.PhaseCur=jobMerge.PhaseCur;
				comboPhase.SelectedIndex=(int)_jobCur.PhaseCur;
			}
			//PROPOSED VERSION
			if(_jobCur.ProposedVersion!=jobMerge.ProposedVersion) {
				_jobCur.ProposedVersion=jobMerge.ProposedVersion;
				_jobOld.ProposedVersion=jobMerge.ProposedVersion;
				comboProposedVersion.SelectedIndex=(int)_jobCur.ProposedVersion;
			}
			//APPROVAL STATUS
			if(_jobCur.IsApprovalNeeded!=jobMerge.IsApprovalNeeded) {
				_jobCur.IsApprovalNeeded=jobMerge.IsApprovalNeeded;
				_jobOld.IsApprovalNeeded=jobMerge.IsApprovalNeeded;
				if(_jobCur.IsApprovalNeeded) {
					textApprove.Text="Waiting";
				}
				else if(_jobCur.UserNumApproverConcept>0 ||_jobCur.UserNumApproverJob>0||_jobCur.UserNumApproverChange>0) {
					textApprove.Text="Yes";
				}
				else {
					textApprove.Text="No";
				}
			}
			//CATEGORY
			if(_jobCur.Category!=jobMerge.Category) {//Was edited, AND user has not already edited it themselves.
				_jobCur.Category=jobMerge.Category;
				_jobOld.Category=jobMerge.Category;
			comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobCur.Category.ToString());
			}
			//DATEENTRY - Cannot change
			//VERSION
			if(_jobCur.JobVersion!=jobMerge.JobVersion && _jobCur.JobVersion==_jobOld.JobVersion) {//Was edited, AND user has not already edited it themselves.
				_jobCur.JobVersion=jobMerge.JobVersion;
				_jobOld.JobVersion=jobMerge.JobVersion;
				textVersion.Text=_jobCur.JobVersion;
			}
			//CONCEPT EST
			if(_jobCur.HoursEstimateConcept!=jobMerge.HoursEstimateConcept && _jobCur.HoursEstimateConcept==_jobOld.HoursEstimateConcept) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateConcept=jobMerge.HoursEstimateConcept;
				_jobOld.HoursEstimateConcept=jobMerge.HoursEstimateConcept;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			//WRITEUP EST
			if(_jobCur.HoursEstimateWriteup!=jobMerge.HoursEstimateWriteup && _jobCur.HoursEstimateWriteup==_jobOld.HoursEstimateWriteup) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateWriteup=jobMerge.HoursEstimateWriteup;
				_jobOld.HoursEstimateWriteup=jobMerge.HoursEstimateWriteup;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			//DEVELOPMENT EST
			if(_jobCur.HoursEstimateDevelopment!=jobMerge.HoursEstimateDevelopment && _jobCur.HoursEstimateDevelopment==_jobOld.HoursEstimateDevelopment) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateDevelopment=jobMerge.HoursEstimateDevelopment;
				_jobOld.HoursEstimateDevelopment=jobMerge.HoursEstimateDevelopment;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			//REVIEW EST
			if(_jobCur.HoursEstimateReview!=jobMerge.HoursEstimateReview && _jobCur.HoursEstimateReview==_jobOld.HoursEstimateReview) {//Was edited, AND user has not already edited it themselves.
				_jobCur.HoursEstimateReview=jobMerge.HoursEstimateReview;
				_jobOld.HoursEstimateReview=jobMerge.HoursEstimateReview;
				textEstHours.Text=_jobCur.HoursEstimate.ToString();
				SetHoursLeft();
			}
			//HOURS ACT
			if(_jobCur.HoursActual!=jobMerge.HoursActual && _jobCur.HoursActual==_jobOld.HoursActual) {//Was edited, AND user has not already edited it themselves.
				//Stored in another table now, cannot set this value and no real need to.
				//_jobCur.HoursActual=jobMerge.HoursActual;
				//_jobOld.HoursActual=jobMerge.HoursActual;
				textActualHours.Text=_jobCur.HoursActual.ToString();
				SetHoursLeft();
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
			_isLoading=false;
			CheckPermissions();
			textJobEditor.RefreshSpellCheckConcept();
			textJobEditor.RefreshSpellCheckWriteup();
		}

		private void butParentRemove_Click(object sender,EventArgs e) {
			_jobCur.ParentNum=0;
			_jobOld.ParentNum=0;
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job jobCur = Jobs.GetOne(_jobCur.JobNum);
				jobCur.ParentNum=0;
				Jobs.Update(jobCur);
				_treeNode=Jobs.GetJobTree(_jobCur,JobManagerCore.ListJobsAll);
				FillTreeRelated();
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
			if(IsNew) {
				IsChanged=true;
			}
			else {
				Job jobCur = Jobs.GetOne(_jobCur.JobNum);
				jobCur.ParentNum=parentNum;
				Jobs.Update(jobCur);
				_treeNode=Jobs.GetJobTree(_jobCur,JobManagerCore.ListJobsAll);
				FillTreeRelated();
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
			}
		}

		///<summary>Job must have all in memory fields filled. Eg. Job.ListJobLinks, Job.ListJobNotes, etc. Also makes some of the JobLog entries.</summary>
		private void SaveJob(Job job) {
			_isLoading=true;
			timerTitle.Stop();
			timerVersion.Stop();
			//Validation must happen before this is called.
			job.Requirements=textJobEditor.ConceptRtf;
			job.Implementation=textJobEditor.WriteupRtf;
			job.Documentation=textEditorDocumentation.MainRtf;
			job.RequirementsJSON=JsonConvert.SerializeObject(textJobEditor.GetListJobRequirements());
			if(comboPriority.SelectedIndex>-1) {
				job.Priority=comboPriority.GetSelectedDefNum();
			}
			if(comboPriorityTesting.SelectedIndex > -1) {
				job.PriorityTesting=comboPriorityTesting.GetSelectedDefNum();
			}
			if(job.ListJobLinks.Exists(x => x.LinkType==JobLinkType.Request) && job.PhaseCur==JobPhase.Development) {
				if(_listPrioritiesAll.FirstOrDefault(x => x.DefNum==job.Priority).ItemValue.Contains("OnHold")) {
					FeatureRequests.MarkAsApproved(job.ListJobLinks.Where(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList());
				}
				else {
					FeatureRequests.MarkAsInProgress(job.ListJobLinks.Where(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList());
				}
			}
			job.JobVersion=textVersion.Text;
			job.Title=textTitle.Text;
			//job.PhaseCur=(JobPhase)comboStatus.SelectedIndex;
			//job.Category=(JobCategory)comboCategory.SelectedIndex;
			//All other fields should have been maintained while editing the job in the form.
			job.UserNumCheckout=0;
			if(job.JobNum==0 || IsNew) {
				if(job.UserNumConcept==0 && JobPermissions.IsAuthorized(JobPerm.Concept,true)) {
					job.UserNumConcept=Security.CurUser.UserNum;
				}
				Jobs.Insert(job);
				job.ListJobLinks.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobNotes.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobReviews.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobTimeLogs.ForEach(x=>x.JobNum=job.JobNum);
				job.ListJobQuotes.ForEach(x=>x.JobNum=job.JobNum);
				//job.ListJobEvents.ForEach(x=>x.JobNum=job.JobNum);//do not sync
			}
			else {
				Jobs.Update(job);
			}
			JobLinks.Sync(job.ListJobLinks,job.JobNum);
			JobNotes.Sync(job.ListJobNotes,job.JobNum);
			JobReviews.SyncReviews(job.ListJobReviews,job.JobNum);
			JobReviews.SyncTimeLogs(job.ListJobTimeLogs,job.JobNum);
			JobQuotes.Sync(job.ListJobQuotes,job.JobNum);
			JobActiveLinks.UpsertLink(job,_jobOld,Security.CurUser.UserNum,checkIsActive.Checked);
			MakeLogEntry(job,_jobOld);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			JobManagerCore.UpdateForSingleJob(job);
			LoadJob(job,_treeNode);//Tree view may become out of date if viewing a job for an extended period of time.
			if(SaveClick!=null) {
				SaveClick(this,new EventArgs());
			}
			_isLoading=false;
		}

		//Allows save to be called from outside this control.
		public bool ForceSave() {
			if(_jobCur==null || IsChanged==false) {
				return true;//Nothing to save
			}
			bool doRequirementCheck=true;
			if(_jobCur.PhaseCur==JobPhase.Concept && !_jobCur.IsApprovalNeeded) {
				doRequirementCheck=false;
			}
			if(!ValidateJob(_jobCur,doRequirementCheck)) {
				return false;//Job Failed Validation
			}
			SaveJob(_jobCur);
			return true;
		}

		private bool CheckJobLinks() {
			List<JobLink> listBugLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Bug);
			List<JobLink> listMobileBugLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.MobileBug);
			List<JobLink> listRequestLinks=_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request);
			if(_jobCur.Category==JobCategory.Enhancement && listBugLinks.Count==0 && listMobileBugLinks.Count==0) {
				MsgBox.Show(this,"Enhancements must have an attached bug marked as completed before marking as implemented.");
				return false;
			}
			if((listBugLinks.Count>0 && !Bugs.CheckForCompletion(listBugLinks.Select(x => x.FKey).ToList()))
				|| (listMobileBugLinks.Count>0 && !MobileBugs.CheckForCompletion(listMobileBugLinks.Select(x => x.FKey).ToList())))
			{
				MsgBox.Show(this,"This job has incomplete bugs. Complete them before sending the job to documentation.");
				return false;
			}
			if(_jobCur.Category==JobCategory.Bug && listBugLinks.Count==0 && listMobileBugLinks.Count==0) {
				MsgBox.Show(this,"All bug jobs must have at least one bug attached.  Please attach a bug and retry.");
				return false;
			}
			if(listRequestLinks.Count>0 && !FeatureRequests.CheckForCompletion(listRequestLinks.Select(x => x.FKey).ToList())) {
				using FormFeatureRequestPrompt FormFRP=new FormFeatureRequestPrompt(listRequestLinks);
				FormFRP.ShowDialog();
				if(FormFRP.DialogResult==DialogResult.Cancel || !FeatureRequests.CheckForCompletion(listRequestLinks.Select(x => x.FKey).ToList())) {
					MsgBox.Show(this,"Incomplete feature requests will need to be completed before sending the job to documentation.");
					return false;
				}
				return true;
			}
			if(_jobCur.Category==JobCategory.Feature && listRequestLinks.Count==0) {
				MsgBox.Show(this,"All Feature jobs must have at least one feature attached.  Please attach a feature and retry.");
				return false;
			}
			return true;
		}

		private void comboPriority_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(ListTools.In(_jobCur.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Development,JobPhase.Quote) 
				&& comboPriority.GetSelected<Def>().ItemValue.Contains("OnHold") 
				&& _jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug))
			{
				comboPriority.SetSelectedDefNum(_jobCur.Priority);
				MsgBox.Show(this,"Please remove all bugs from a job before marking it as on hold.");
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

		private void comboProposedVersion_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			JobProposedVersion proposedVersion=(JobProposedVersion)comboProposedVersion.SelectedIndex;
			_jobCur.ProposedVersion=proposedVersion;
			JobLogs.MakeLogEntryForCategory(_jobCur,_jobOld);
			_jobOld.ProposedVersion=proposedVersion;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.ProposedVersion=(JobProposedVersion)comboProposedVersion.SelectedIndex;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboPriorityTesting_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			long priorityNum=comboPriorityTesting.GetSelectedDefNum();
			_jobCur.PriorityTesting=priorityNum;
			_jobOld.PriorityTesting=priorityNum;
			if(!IsNew) {
				Job job=Jobs.GetOne(_jobCur.JobNum);
				job.PriorityTesting=priorityNum;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboProject_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			//This should normally never get hit. Project changes should almost exclusively happen due to Job Actions.
			JobPatternReviewProject jobProject=(JobPatternReviewProject)comboProject.SelectedIndex;
			_jobCur.PatternReviewProject=jobProject;
			JobLogs.MakeLogEntryForProject(_jobCur,_jobOld);
			_jobOld.PatternReviewProject=jobProject;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.PatternReviewProject=(JobPatternReviewProject)comboProject.SelectedIndex;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboPatternStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			JobPatternReviewStatus jobPatternStatus=(JobPatternReviewStatus)comboPatternStatus.SelectedIndex;
			_jobCur.PatternReviewStatus=jobPatternStatus;
			JobLogs.MakeLogEntryForPatternReview(_jobCur,_jobOld);
			_jobOld.PatternReviewStatus=jobPatternStatus;
			if(!IsNew) {
				Job job=Jobs.GetOne(_jobCur.JobNum);
				job.PatternReviewStatus=(JobPatternReviewStatus)comboPatternStatus.SelectedIndex;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			//This should normally never get hit. Status changes should almost exclusively happen due to Job Actions.
			JobPhase jobPhaseNew=(JobPhase)comboPhase.SelectedIndex;
			if(jobPhaseNew==JobPhase.Cancelled
				&& _jobCur.PhaseCur!=JobPhase.Cancelled
				&& _jobCur.ListJobLinks.Any(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug))
			{
				MsgBox.Show(this,"You cannot cancel a job that has a bug attached. Please detach the bug first before canceling the job.");
				comboPhase.SelectedIndex=(int)_jobCur.PhaseCur;
				return;
			}
			_jobCur.PhaseCur=jobPhaseNew;
			JobLogs.MakeLogEntryForPhase(_jobCur,_jobOld);
			_jobOld.PhaseCur=jobPhaseNew;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.PhaseCur=(JobPhase)comboPhase.SelectedIndex;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void comboCategory_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			JobCategory jobCategoryNew=(JobCategory)_listCategoryNames.IndexOf(comboCategory.SelectedItem.ToString());
			if((jobCategoryNew==JobCategory.SpecialProject && !JobPermissions.IsAuthorized(JobPerm.SpecialProject))
				|| (jobCategoryNew==JobCategory.NeedNoApproval && !JobPermissions.IsAuthorized(JobPerm.ProjectManager))) 
			{
				comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobCur.Category.ToString());
				return;
			}
			_jobCur.Category=jobCategoryNew;
			JobLogs.MakeLogEntryForCategory(_jobCur,_jobOld);
			_jobOld.Category=jobCategoryNew;
			if(!IsNew) {
				Job job = Jobs.GetOne(_jobCur.JobNum);
				job.Category=(JobCategory)_listCategoryNames.IndexOf(comboCategory.SelectedItem.ToString());
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void gridCustomers_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink = new JobLink() {
				FKey=FormPS.SelectedPatNum,
				JobNum=_jobCur.JobNum,
				LinkType=JobLinkType.Customer
			};
			if(!IsNew) {
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobLinks.Add(jobLink);
			FillGridCustomers();
		}

		private void gridWatchers_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading || _jobCur==null) {
				return;//should never happen
			}
			using FormUserPick FormUP = new FormUserPick();
			//Suggest current user if not already watching.
			if(_jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber).All(x => x.FKey!=Security.CurUser.UserNum)) {
				FormUP.SuggestedUserNum=Security.CurUser.UserNum;
			}
			FormUP.IsSelectionmode=true;
			FormUP.ShowDialog();
			if(FormUP.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink = new JobLink() {
				FKey=FormUP.SelectedUserNum,
				JobNum=_jobCur.JobNum,
				LinkType=JobLinkType.Subscriber
			};
			if(!IsNew) {
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobLinks.Add(jobLink);
			FillGridWatchers();
		}

		private void gridCustomerQuotes_TitleAddClick(object sender,EventArgs e) {
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
			using FormJobQuoteEdit FormJQE=new FormJobQuoteEdit(new JobQuote() {JobNum=_jobCur.JobNum,IsNew=true});
			FormJQE.ShowDialog();
			if(FormJQE.DialogResult!=DialogResult.OK || FormJQE.JobQuoteCur==null) {
				return;
			}
			if(!IsNew) {
				JobQuotes.Insert(FormJQE.JobQuoteCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobQuotes.Add(FormJQE.JobQuoteCur);
			FillGridQuote();
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
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			AddAppointment(FormPS.SelectedPatNum);
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

		private void gridFeatureReq_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			AddFeatureRequest();
		}

		private bool AddFeatureRequest() {
			if(_jobCur==null) {
				return false;//should never happen
			}
			using FormFeatureRequest FormFR=new FormFeatureRequest() {IsSelectionMode=true};
			FormFR.ShowDialog();
			if(FormFR.DialogResult!=DialogResult.OK) {
				return false;
			}
			JobLink jobLink=new JobLink();
			jobLink.JobNum=_jobCur.JobNum;
			jobLink.FKey=FormFR.SelectedFeatureNum;
			jobLink.LinkType=JobLinkType.Request;
			if(!IsNew) {
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobLinks.Add(jobLink);
			FillGridFeatureReq();
			return true;
		}

		private void gridBugs_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			AddBug();
		}

		private bool AddBug() {
			if(_jobCur==null) {
				return false;//should never happen
			}
			using FormBugSearch FormBS=new FormBugSearch(_jobCur);
			FormBS.ShowDialog();
			if(FormBS.DialogResult!=DialogResult.OK || (FormBS.BugCur==null && FormBS.MobileBugCur==null)) {
				return false;
			}
			JobLink jobLink=new JobLink();
			jobLink.JobNum=_jobCur.JobNum;
			if(FormBS.BugCur!=null) {
				jobLink.FKey=FormBS.BugCur.BugId;
				jobLink.LinkType=JobLinkType.Bug;
			}
			else {
				jobLink.FKey=FormBS.MobileBugCur.MobileBugNum;
				jobLink.LinkType=JobLinkType.MobileBug;
			}
			if(!IsNew) {
				JobLinks.Insert(jobLink);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobLinks.Add(jobLink);
			FillGridBugs();
			return true;
		}

		private void gridFiles_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
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

		private void gridReview_TitleAddClick(object sender,EventArgs e) {
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
			long userNumReviewer=0;
			if(!PickUserByJobPermission("Pick Reviewer",JobPerm.Writeup,out userNumReviewer,_jobCur.UserNumExpert,false,false)) {
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
			FillGridReviews();
		}

		private void gridNotes_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(AddJobNote()) {
				FillGridDiscussion();
			}
		}

		private void gridTestingNotes_TitleAddClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(AddJobNote(JobNoteTypes.Testing)) {
				FillGridTestingNotes();
			}
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
			using FormJobNoteEdit FormJNE=new FormJobNoteEdit(jobNote);
			FormJNE.ShowDialog();
			if(FormJNE.DialogResult!=DialogResult.OK || FormJNE.JobNoteCur==null) {
				return false;
			}
			if(!IsNew) {
				JobNotes.Insert(FormJNE.JobNoteCur);
				JobNotifications.UpsertAllNotifications(_jobCur,Security.CurUser.UserNum,JobNotificationChanges.NoteAdded);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobNotes.Add(FormJNE.JobNoteCur);
			return true;
		}

		private void gridCustomerQuotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridQuotes.ListGridRows[e.Row].Tag is JobQuote)) {
				return;//should never happen
			}
			JobQuote jq = (JobQuote)gridQuotes.ListGridRows[e.Row].Tag;
			using FormJobQuoteEdit FormJQE = new FormJobQuoteEdit(jq);
			FormJQE.ShowDialog();
			if(FormJQE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(!IsNew) {
				if(FormJQE.JobQuoteCur==null) {
					JobQuotes.Delete(jq.JobQuoteNum);
				}
				else {
					JobQuotes.Update(FormJQE.JobQuoteCur);
				}
			}
			else {
				IsChanged=true;
			}
			_jobCur.ListJobQuotes.RemoveAll(x=>x.JobQuoteNum==jq.JobQuoteNum);//should remove only one
			if(FormJQE.JobQuoteCur!=null) {//re-add altered version, iff the jobquote was modified.
				_jobCur.ListJobQuotes.Add(FormJQE.JobQuoteCur);
			}
			FillGridQuote();
		}

		private void gridTasks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridTasks.ListGridRows[e.Row].Tag is long)) {
				return;//should never happen
			}
			//GoTo patietn will not work from this form. It would require a delegate to be passed in all the way from FormOpenDental.
			Task task=Tasks.GetOne((long)gridTasks.ListGridRows[e.Row].Tag);
			if(task==null) {
				MsgBox.Show(Lan.g(this,"Task can no longer be found, it may have been deleted or canceled."));
				FillGridTasks();
				return;
			}
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

		private void gridAppointments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridAppointments.ListGridRows[e.Row].Tag is Appointment)) {
				return;//should never happen.
			}
			try {
				FormApptEdit FormAE = new FormApptEdit(((Appointment)gridAppointments.ListGridRows[e.Row].Tag).AptNum);
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
				job.Title=textTitle.Text;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			}
			textTitle.SpellCheck();
		}
		
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
			using FormVersionPrompt FormVP=new FormVersionPrompt();
			FormVP.VersionText=textVersion.Text;
			FormVP.ShowDialog();
			if(FormVP.DialogResult!=DialogResult.OK || string.IsNullOrEmpty(FormVP.VersionText)) {
				return;
			}
			textVersion.Text=FormVP.VersionText;
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
				job.JobVersion=textVersion.Text;
				Jobs.Update(job);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,job.JobNum);
			}
		}

		///<summary>SaveClick for textboxes: Concept, Writeup, and Documentation</summary>
		private void textEditor_SaveClick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			bool doRequirementCheck=true;
			if(_jobCur.PhaseCur==JobPhase.Concept && !_jobCur.IsApprovalNeeded) {
				doRequirementCheck=false;
			}
			if(!ValidateJob(_jobCur,doRequirementCheck)) {
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

		private void textTestingHours_TextChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			textTestingHours.BackColor=Color.FromArgb(255,255,230);//light yellow
			timerTesting.Stop();
			timerTesting.Start();
		}

		private void timerTesting_Tick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(IsNew) {
				//do nothing
			}
			else {
					timerTesting.Stop();
					textTestingHours.BackColor=Color.White;
					_jobCur.HoursTesting=PIn.Double(textTestingHours.Text);
					Job jobFromDB = Jobs.GetOne(_jobCur.JobNum);//Get from DB to ensure freshest copy (Lists not filled)
					jobFromDB.HoursTesting=PIn.Double(textTestingHours.Text);
					Jobs.Update(jobFromDB);
					Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		private void gridFeatureReq_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridFeatureReq.ListGridRows[e.Row].Tag is long)) {
				return;//should never happen.
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			//TODO: fix this to get rid of orphaned links when FRs are deleted.
			FormRequestEdit FormFR=new FormRequestEdit();
			FormFR.RequestId=(long)gridFeatureReq.ListGridRows[e.Row].Tag;
			FormFR.IsAdminMode=PrefC.IsODHQ;
			FormFR.Show();
		}

		private void gridNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridNotes.ListGridRows[e.Row].Tag is JobNote)) {
				return;//should never happen.
			}
			if(EditJobNote((JobNote)gridNotes.ListGridRows[e.Row].Tag)) {
				FillGridDiscussion();
			}
		}

		private void gridTestingNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridTestingNotes.ListGridRows[e.Row].Tag is JobNote)) {
				return;//should never happen.
			}
			if(EditJobNote((JobNote)gridTestingNotes.ListGridRows[e.Row].Tag)) {
				FillGridTestingNotes();
			}
		}

		///<summary>Edits the job note at the selected index provided.  Updates _jobCur.ListJobNotes accordingly.
		///Returns true if the note was changed; Otherwise, false.</summary>
		private bool EditJobNote(JobNote jobNote) {
			JobNote jobNoteOld=jobNote.Copy();
			using FormJobNoteEdit FormJNE=new FormJobNoteEdit(jobNote);
			FormJNE.ShowDialog();
			if(FormJNE.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(jobNote.NoteType==JobNoteTypes.Discussion) {
				MakeLogEntryForNote(FormJNE.JobNoteCur,jobNoteOld);
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
			_jobCur.ListJobNotes.RemoveAll(x => x.JobNoteNum==jobNote.JobNoteNum);
			if(FormJNE.JobNoteCur!=null) {
				_jobCur.ListJobNotes.Add(FormJNE.JobNoteCur);
			}
			return true;
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

		private void gridBugs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading || gridBugs.SelectedIndices.Length==0) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			if(gridBugs.ListGridRows[gridBugs.GetSelectedIndex()].Tag.GetType()==typeof(MobileBug)) {
				using FormMobileBugEdit formMobileBugEdit=new FormMobileBugEdit();
				formMobileBugEdit.MobileBugCur=MobileBugs.GetOne(gridBugs.SelectedTag<MobileBug>().MobileBugNum);
				formMobileBugEdit.ShowDialog();
				try {
					if(formMobileBugEdit.MobileBugCur==null) {
						_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.MobileBug && x.FKey==gridBugs.SelectedTag<MobileBug>().MobileBugNum);
					}
				}
				catch(Exception ex) {
					//Just continue, the link has already been removed most likely
				}
			}
			else {
				using FormBugEdit FormBE=new FormBugEdit();
				FormBE.BugCur=Bugs.GetOne(gridBugs.SelectedTag<Bug>().BugId);
				FormBE.ShowDialog();
				try {
					if(FormBE.BugCur==null) {
						_jobCur.ListJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Bug&&x.FKey==gridBugs.SelectedTag<Bug>().BugId);
					}
				}
				catch(Exception ex) {
					//Just continue, the link has already been removed most likely
				}
			}
			FillGridBugs();
		}

		private void gridFiles_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(!(gridFiles.ListGridRows[e.Row].Tag is JobLink)) {
				return;
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
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

		///<summary>Adds error checking to the input parameters for JobLogs.MakeLogEntry also inserts joblog into the UI and _jobCur.ListJobLog if returned.</summary>
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

		private void butAddTime_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(_jobCur==null) {
				return;//should never happen
			}
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
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
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			using FormJobEstimate FormJE=new FormJobEstimate(_jobCur);
			if(FormJE.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textEstHours.Text=_jobCur.HoursEstimate.ToString();
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
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
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
			textActualHours.Text=_jobCur.HoursActual.ToString();
			SetHoursLeft();
			return true;
		}

		private void textApprove_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(_jobCur==null || !IsOverride || !_jobCur.IsApprovalNeeded) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Job approval pending, retract approval request?")) {
				return;
			}
			if(!ValidateJob(_jobCur)) {
				return;
			}
			IsChanged=true;
			_jobCur.IsApprovalNeeded=false;
			SaveJob(_jobCur);
		}

		private void butParentJob_Click(object sender,EventArgs e) {
			if(IsNew || _jobCur.ParentNum==0) {
				return;
			}
			if(RequestJob!=null) {
				RequestJob(this,_jobCur.ParentNum);
			}
		}

		private void butTimeLog_Click(object sender,EventArgs e) {
			FormJobTimeLog FormJTL=new FormJobTimeLog(_jobCur);
			FormJTL.Show(this);
		}
		
		private void butTested_Click(object sender,EventArgs e) {
			_isMarkTestedPushed=true; //Make sure the checkNotTested call below doesn't call an extra SaveJob(_jobCur).
			_jobCur.DateTimeTested=DateTime.Now;
			checkNotTested.Checked=false;
			SaveJob(_jobCur);
			JobLogs.MakeLogEntryForTesting(_jobCur,"Job Marked as Tested");
			_isMarkTestedPushed=false;
		}

		private void checkNotTested_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			_jobCur.IsNotTested=checkNotTested.Checked;
			if(!_isMarkTestedPushed) {
				SaveJob(_jobCur);
			}
			if(_jobCur.IsNotTested) {
				JobLogs.MakeLogEntryForTesting(_jobCur,"Job marked as not tested");
			}
			else {
				JobLogs.MakeLogEntryForTesting(_jobCur,"Not tested status removed");
			}
		}

		private void SetHoursLeft() {
			double hoursLeft=_jobCur.HoursEstimate-_jobCur.HoursActual;
			textHoursLeft.BackColor=SystemColors.Control;
			if(hoursLeft<0) {
				textHoursLeft.BackColor=Color.Salmon;
			}
			textHoursLeft.Text=(_jobCur.HoursEstimate-_jobCur.HoursActual).ToString();
		}

		private void splitContainerMiddle_SplitterMoved(object sender, SplitterEventArgs e){
			//LayoutManager.LayoutControlBoundsAndFonts(splitContainerMiddle);
		}

		private void panelLeftTop_SizeChanged(object sender,EventArgs e) {
			treeRelatedJobs.Height=panelLeftTop.Height-treeRelatedJobs.Top-2;
		}
	}//end class

}//end namespace
	

