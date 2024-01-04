using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDental {
	public partial class FormJobAdd:FormODBase {
		private Job _jobNew;
		private List<JobLink> _listJobLinks;
		private List<JobQuote> _listJobQuotes;
		private List<string> _listCategoryNames;
		private List<string> _listCategoryNamesFiltered;
		private List<Def> _listPriorities;

		public FormJobAdd(Job jobNew) {
			_jobNew=jobNew;
			_listJobLinks=new List<JobLink>();
			_listJobQuotes=new List<JobQuote>();
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
		}

		private void FormJobAdd_Load(object sender,EventArgs e) {
			UpdateTabVisibilityByJobCategory(_jobNew.Category);
			comboProposedVersion.Visible=true;
			labelVersion.Visible=true;
			if(_jobNew.Category==JobCategory.Project) {
				comboProposedVersion.Visible=false;
				labelVersion.Visible=false;
			}
			_listCategoryNames=Enum.GetNames(typeof(JobCategory)).ToList();
			_listCategoryNamesFiltered=_listCategoryNames.Where(x => !x.In(JobCategory.Query.ToString(),JobCategory.MarketingDesign.ToString())).ToList();
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.NeedNoApproval.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.SpecialProject,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.SpecialProject.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.UnresolvedIssue.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true) || (_jobNew.ParentNum>0 && Jobs.GetOne(_jobNew.ParentNum).Category!=JobCategory.Project)) {
				_listCategoryNamesFiltered.Remove(JobCategory.Project.ToString());
			}
			_listCategoryNamesFiltered.ForEach(x=>comboCategory.Items.Add(x));
			comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobNew.Category.ToString());
			_listPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList();
			_listPriorities.ForEach(x=>comboPriority.Items.Add(x.ItemName,x));
			comboPriority.SelectedIndex=_listPriorities.Select(x => x.DefNum).ToList().IndexOf(_jobNew.Priority);
			List<JobTeam> listJobTeams=JobTeams.GetDeepCopy();
			comboJobTeam.Items.Add("None", new JobTeam(){JobTeamNum=-1});
			comboJobTeam.Items.AddList(listJobTeams,x => x.TeamName);
			comboJobTeam.SelectedIndex=0;
			Enum.GetNames(typeof(JobProposedVersion)).ToList().ForEach(x => comboProposedVersion.Items.Add(x));
			comboProposedVersion.SelectedIndex=(int)JobProposedVersion.Current;
		}

		#region FillGrids
		private void FillGridCustomers() {
			gridCustomers.BeginUpdate();
			gridCustomers.Columns.Clear();
			gridCustomers.Columns.Add(new GridColumn("PatNum",40){ IsWidthDynamic=true });
			gridCustomers.Columns.Add(new GridColumn("Name",40){ IsWidthDynamic=true });
			gridCustomers.Columns.Add(new GridColumn("BillType",40){ IsWidthDynamic=true });
			gridCustomers.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			gridCustomers.ListGridRows.Clear();
			List<Patient> listPatients= Patients.GetMultPats(_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Customer)
				.Select(x => x.FKey).ToList()).ToList();
			foreach(Patient pat in listPatients){
				GridRow row=new GridRow() { Tag=pat };
				row.Cells.Add(pat.PatNum.ToString());
				row.Cells.Add(pat.GetNameFL());
				row.Cells.Add(Defs.GetDef(DefCat.BillingTypes,pat.BillingType).ItemName);
				row.Cells.Add("X");
				gridCustomers.ListGridRows.Add(row);
			}
			gridCustomers.EndUpdate();
		}

		private void FillGridSubscribers() {
			gridSubscribers.BeginUpdate();
			gridSubscribers.Columns.Clear();
			gridSubscribers.Columns.Add(new GridColumn("User Name",40){ IsWidthDynamic=true });
			gridSubscribers.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			gridSubscribers.ListGridRows.Clear();
			List<Userod> listSubscribers=_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber)
				.Select(x => Userods.GetFirstOrDefault(y => y.UserNum==x.FKey)).ToList();
			foreach(Userod user in listSubscribers.FindAll(x => x!=null)){
				GridRow row=new GridRow() { Tag =user };
				row.Cells.Add(user.UserName);
				row.Cells.Add("X");
				gridSubscribers.ListGridRows.Add(row);
			}
			gridSubscribers.EndUpdate();
		}

		private void FillGridQuotes() {
			gridQuotes.BeginUpdate();
			gridQuotes.Columns.Clear();
			gridQuotes.Columns.Add(new GridColumn("PatNum",40){ IsWidthDynamic=true });
			gridQuotes.Columns.Add(new GridColumn("Hours",40,HorizontalAlignment.Center){ IsWidthDynamic=true });
			gridQuotes.Columns.Add(new GridColumn("Amt",40,HorizontalAlignment.Right){ IsWidthDynamic=true });
			gridQuotes.Columns.Add(new GridColumn("Appr?",40,HorizontalAlignment.Center));
			gridQuotes.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			gridQuotes.NoteSpanStart=0;
			gridQuotes.NoteSpanStop=4;
			gridQuotes.ListGridRows.Clear();
			foreach(JobQuote jobQuote in _listJobQuotes){
				GridRow row=new GridRow() { Tag=jobQuote };
				row.Cells.Add(jobQuote.PatNum.ToString());
				row.Cells.Add(jobQuote.Hours);
				row.Cells.Add(jobQuote.ApprovedAmount=="0.00"?jobQuote.Amount:jobQuote.ApprovedAmount);
				row.Cells.Add(jobQuote.IsCustomerApproved?"X":"");
				row.Cells.Add("X");
				row.Note=jobQuote.Note;
				gridQuotes.ListGridRows.Add(row);
			}
			gridQuotes.EndUpdate();
		}

		private void FillGridTasks() {
			gridTasks.BeginUpdate();
			gridTasks.Columns.Clear();
			gridTasks.Columns.Add(new GridColumn("Date",40){ IsWidthDynamic=true });
			gridTasks.Columns.Add(new GridColumn("TaskList",40){ IsWidthDynamic=true });
			gridTasks.Columns.Add(new GridColumn("Done",40) { TextAlign=HorizontalAlignment.Center });
			gridTasks.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			gridTasks.NoteSpanStart=0;
			gridTasks.NoteSpanStop=3;
			gridTasks.ListGridRows.Clear();
			List<Task> listTasks=_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Task)
				.Select(x => Tasks.GetOne(x.FKey))
				.Where(x => x!=null)
				.OrderBy(x =>x.DateTimeEntry).ToList();
			foreach(Task task in listTasks){
				GridRow row=new GridRow() { Tag=task.TaskNum };//taskNum
				row.Cells.Add(task.DateTimeEntry.ToShortDateString());
				row.Cells.Add(TaskLists.GetOne(task.TaskListNum)?.Descript??"<TaskListNum:"+task.TaskListNum+">");
				row.Cells.Add(task.TaskStatus==TaskStatusEnum.Done?"X":"");
				row.Cells.Add("X");
				row.Note=StringTools.Truncate(task.Descript,100,true).Trim();
				gridTasks.ListGridRows.Add(row);
			}
			gridTasks.EndUpdate();
		}

		private void FillGridFeatureReq() {
			gridFeatureReq.BeginUpdate();
			gridFeatureReq.Columns.Clear();
			gridFeatureReq.Columns.Add(new GridColumn("Feat Req Num",40){ IsWidthDynamic=true });
			gridFeatureReq.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			//todo: add status of FR. Difficult because FR dataset comes from webservice.
			//gridFeatureReq.Columns.Add(new ODGridColumn("Status",50){TextAlign=HorizontalAlignment.Center});
			gridFeatureReq.ListGridRows.Clear();
			List<long> listReqNums=_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList();
			foreach(long reqNum in listReqNums){
				GridRow row=new GridRow() { Tag=reqNum };//FR Num
				row.Cells.Add(reqNum.ToString());
				row.Cells.Add("X");
				//todo: add status of FR. Difficult because FR dataset comes from webservice.
				gridFeatureReq.ListGridRows.Add(row);
			}
			gridFeatureReq.EndUpdate();
		}

		private void FillGridBugs() {
			gridBugs.BeginUpdate();
			gridBugs.Columns.Clear();
			gridBugs.Columns.Add(new GridColumn("Flags",50));
			gridBugs.Columns.Add(new GridColumn("Bug Num (From JRMT)",40){ IsWidthDynamic=true });
			gridBugs.Columns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
			gridBugs.ListGridRows.Clear();
			List<JobLink> listBugLinks=_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Bug || x.LinkType==JobLinkType.MobileBug).ToList();
			for(int i=0;i<listBugLinks.Count;i++) {
				GridRow row=new GridRow();
				if(listBugLinks[i].LinkType==JobLinkType.Bug) {
					Bug bug=Bugs.GetOne(listBugLinks[i].FKey);
					row.Cells.Add("");
					row.Cells.Add(bug==null ? "Invalid Bug" : bug.Description);
					row.Tag=bug;
				}
				else {
					MobileBug mobileBug=MobileBugs.GetOne(listBugLinks[i].FKey);
					row.Cells.Add(BugFlag.Mobile.ToString());
					row.Cells.Add(mobileBug==null ? "Invalid Bug" : mobileBug.Description);
					row.Tag=mobileBug;
				}
				row.Cells.Add("X");
				gridBugs.ListGridRows.Add(row);
			}
			gridBugs.EndUpdate();
		}
		#endregion

		#region GridActions
		private void gridCustomers_TitleAddClick(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink = new JobLink() {
				FKey=FormPS.PatNumSelected,
				LinkType=JobLinkType.Customer
			};
			_listJobLinks.Add(jobLink);
			FillGridCustomers();
		}

		private void gridSubscribers_TitleAddClick(object sender,EventArgs e) {
			FrmUserPick frmUserPick = new FrmUserPick();
			//Suggest current user if not already watching.
			if(_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber).All(x => x.FKey!=Security.CurUser.UserNum)) {
				frmUserPick.UserNumSuggested=Security.CurUser.UserNum;
			}
			frmUserPick.IsSelectionMode=true;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return;
			}
			JobLink jobLink = new JobLink() {
				FKey=frmUserPick.UserNumSelected,
				LinkType=JobLinkType.Subscriber
			};
			_listJobLinks.Add(jobLink);
			FillGridSubscribers();
		}

		private void gridCustomerQuotes_TitleAddClick(object sender,EventArgs e) {
			using FormJobQuoteEdit FormJQE=new FormJobQuoteEdit(new JobQuote() {IsNew=true});
			FormJQE.ShowDialog();
			if(FormJQE.DialogResult!=DialogResult.OK || FormJQE.JobQuoteCur==null) {
				return;
			}
			_listJobQuotes.Add(FormJQE.JobQuoteCur);
			FillGridQuotes();
		}

		private void gridTasks_TitleAddClick(object sender,EventArgs e) {
			using FormTaskSearch FormTS=new FormTaskSearch() {IsSelectionMode=true};
			FormTS.ShowDialog();
			if(FormTS.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink=new JobLink();
			jobLink.FKey=FormTS.TaskNumSelected;
			jobLink.LinkType=JobLinkType.Task;
			_listJobLinks.Add(jobLink);
			FillGridTasks();
		}

		private void gridFeatureReq_TitleAddClick(object sender,EventArgs e) {
			AddFeatureRequest();
		}

		private bool AddFeatureRequest() {
			using FormFeatureRequest FormFR=new FormFeatureRequest() {IsSelectionMode=true};
			FormFR.ShowDialog();
			if(FormFR.DialogResult!=DialogResult.OK) {
				return false;
			}
			JobLink jobLink=new JobLink();
			jobLink.FKey=FormFR.SelectedFeatureNum;
			jobLink.LinkType=JobLinkType.Request;
			_listJobLinks.Add(jobLink);
			FillGridFeatureReq();
			return true;
		}

		private void gridBugs_TitleAddClick(object sender,EventArgs e) {
			AddBug();
		}

		private bool AddBug() {
			using FormBugSearch FormBS=new FormBugSearch(_jobNew);
			FormBS.ShowDialog();
			if(FormBS.DialogResult!=DialogResult.OK || (FormBS.BugCur==null && FormBS.MobileBugCur==null)) {
				return false;
			}
			JobLink jobLink=new JobLink();
			if(FormBS.BugCur!=null) {
				jobLink.FKey=FormBS.BugCur.BugId;
				jobLink.LinkType=JobLinkType.Bug;
			}
			else {
				jobLink.FKey=FormBS.MobileBugCur.MobileBugNum;
				jobLink.LinkType=JobLinkType.MobileBug;
			}
			_listJobLinks.Add(jobLink);
			FillGridBugs();
			return true;
		}

		private void gridBugs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridBugs.ListGridRows[e.Row].Tag.GetType()==typeof(MobileBug)) {
				using FormMobileBugEdit formMobileBugEdit=new FormMobileBugEdit();
				formMobileBugEdit.MobileBugCur=MobileBugs.GetOne(gridBugs.SelectedTag<MobileBug>().MobileBugNum);
				formMobileBugEdit.ShowDialog();
				if(formMobileBugEdit.MobileBugCur==null) {
					_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.MobileBug && x.FKey==gridBugs.SelectedTag<MobileBug>().MobileBugNum);
				}
			}
			else {
				using FormBugEdit formBugEdit=new FormBugEdit();
				formBugEdit.BugCur=Bugs.GetOne(gridBugs.SelectedTag<Bug>().BugId);
				formBugEdit.ShowDialog();
				if(formBugEdit.BugCur==null) {
					_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Bug && x.FKey==gridBugs.SelectedTag<Bug>().BugId);
				}
			}
			FillGridBugs();
		}

		private void gridQuotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridQuotes.ListGridRows[e.Row].Tag is JobQuote)) {
				return;//should never happen
			}
			JobQuote jq = (JobQuote)gridQuotes.ListGridRows[e.Row].Tag;
			using FormJobQuoteEdit FormJQE = new FormJobQuoteEdit(jq);
			FormJQE.ShowDialog();
			if(FormJQE.DialogResult!=DialogResult.OK) {
				return;
			}
			_listJobQuotes.RemoveAll(x=>x.JobQuoteNum==jq.JobQuoteNum);//should remove only one
			if(FormJQE.JobQuoteCur!=null) {//re-add altered version, iff the jobquote was modified.
				_listJobQuotes.Add(FormJQE.JobQuoteCur);
			}
			FillGridQuotes();
		}

		private void gridTasks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!(gridTasks.ListGridRows[e.Row].Tag is long)) {
				return;//should never happen
			}
			//GoTo patietn will not work from this form. It would require a delegate to be passed in all the way from FormOpenDental.
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

		private void gridCustomers_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==3) {//Remove
				long FKey = ((Patient)gridCustomers.ListGridRows[e.Row].Tag).PatNum;
				_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Customer&&x.FKey==FKey);
				FillGridCustomers();
			}
		}

		private void gridSubscribers_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==1) {//Remove
				long FKey = ((Userod)gridSubscribers.ListGridRows[e.Row].Tag).UserNum;
				_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Subscriber&&x.FKey==FKey);
				FillGridSubscribers();
			}
		}

		private void gridQuotes_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==4) {//Remove
				long FKey = ((JobQuote)gridQuotes.ListGridRows[e.Row].Tag).JobQuoteNum;
				_listJobQuotes.RemoveAll(x => x.JobQuoteNum==FKey);
				FillGridQuotes();
			}
		}

		private void gridTasks_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==3) {//Remove
				long FKey = (long)gridTasks.ListGridRows[e.Row].Tag;
				_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Request&&x.FKey==FKey);
				FillGridFeatureReq();
			}
		}

		private void gridFeatureReq_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==1) {//Remove
				long FKey = (long)gridFeatureReq.ListGridRows[e.Row].Tag;
				_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Task&&x.FKey==FKey);
				FillGridTasks();
			}
		}

		private void gridBugs_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==2) { //Remove
				if(gridBugs.ListGridRows[e.Row].Tag.GetType()==typeof(MobileBug)) {
					_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.MobileBug&&x.FKey==gridBugs.SelectedTag<MobileBug>().MobileBugNum);
				}
				else {
					_listJobLinks.RemoveAll(x => x.LinkType==JobLinkType.Bug&&x.FKey==gridBugs.SelectedTag<Bug>().BugId);
				}
				FillGridBugs();
			}
		}
		#endregion
		
		private void textTitle_TextChanged(object sender,EventArgs e) {
			_jobNew.Title=textTitle.Text;
		}

		private void comboPriority_SelectionChangeCommitted(object sender,EventArgs e) {
			long priorityNum=comboPriority.GetSelected<Def>().DefNum;
			_jobNew.Priority=priorityNum;
		}

		private void comboCategory_SelectionChangeCommitted(object sender,EventArgs e) {
			JobCategory jobCategoryNew=(JobCategory)_listCategoryNames.IndexOf(comboCategory.SelectedItem.ToString());
			if(jobCategoryNew==JobCategory.SpecialProject && !JobPermissions.IsAuthorized(JobPerm.SpecialProject)) {
				comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobNew.Category.ToString());
				return;
			}
			if(jobCategoryNew==JobCategory.Bug) {
				Def bugDef=_listPriorities.FirstOrDefault(x => x.ItemValue.Contains("BugDefault"));
				comboPriority.SelectedIndex=_listPriorities.IndexOf(bugDef);
				_jobNew.Priority=bugDef.DefNum;
			}
			comboProposedVersion.Visible=true;
			labelVersion.Visible=true;
			if(jobCategoryNew==JobCategory.Project) {
				comboProposedVersion.Visible=false;
				labelVersion.Visible=false;
			}
			UpdateTabVisibilityByJobCategory(jobCategoryNew);
			_jobNew.Category=jobCategoryNew;
		}

		private bool ValidateJob() {
			if(string.IsNullOrWhiteSpace(textTitle.Text)) {
				MessageBox.Show("Invalid Title.");
				return false;
			}
			if(_jobNew.Priority==0) {
				MsgBox.Show(this,"Please select a priority before saving the job.");
				return false;
			}
			return true;
		}

		private void UpdateTabVisibilityByJobCategory(JobCategory jobCategory) {
			//If jobCategory is not set to Project, show all tabs except Concept.
			if(jobCategory!=JobCategory.Project) {
				if(!tabControlExtra.TabPages.Contains(tabCustomers)) {
					tabControlExtra.TabPages.Add(tabCustomers);
				}
				if(!tabControlExtra.TabPages.Contains(tabSubscribers)) {
					tabControlExtra.TabPages.Add(tabSubscribers);
				}
				if(!tabControlExtra.TabPages.Contains(tabRequests)) {
					tabControlExtra.TabPages.Add(tabRequests);
				}
				if(!tabControlExtra.TabPages.Contains(tabBugs)) {
					tabControlExtra.TabPages.Add(tabBugs);
				}
				if(!tabControlExtra.TabPages.Contains(tabTasks)) {
					tabControlExtra.TabPages.Add(tabTasks);
				}
				if(!tabControlExtra.TabPages.Contains(tabQuotes)) {
					tabControlExtra.TabPages.Add(tabQuotes);
				}
				tabConcept.Text="Concept";
			}
			else {
				//If jobCategory is set to Project, remove all tabs except Concept.
				if(tabControlExtra.TabPages.Contains(tabCustomers)) {
					tabControlExtra.TabPages.Remove(tabCustomers);
				}
				if(tabControlExtra.TabPages.Contains(tabSubscribers)) {
					tabControlExtra.TabPages.Remove(tabSubscribers);
				}
				if(tabControlExtra.TabPages.Contains(tabRequests)) {
					tabControlExtra.TabPages.Remove(tabRequests);
				}
				if(tabControlExtra.TabPages.Contains(tabBugs)) {
					tabControlExtra.TabPages.Remove(tabBugs);
				}
				if(tabControlExtra.TabPages.Contains(tabTasks)) {
					tabControlExtra.TabPages.Remove(tabTasks);
				}
				if(tabControlExtra.TabPages.Contains(tabQuotes)) {
					tabControlExtra.TabPages.Remove(tabQuotes);
				}
				tabConcept.Text="Project Description";
				//Clear all job links and anything not required
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateJob()) {
				return;
			}
			long jobTeamNum=comboJobTeam.GetSelected<JobTeam>().JobTeamNum;
			if(jobTeamNum>-1){
				JobLink jobLink=new JobLink();
				jobLink.FKey=jobTeamNum;
				jobLink.LinkType=JobLinkType.JobTeam;
				_listJobLinks.Add(jobLink);
			}
			if(_jobNew.Category!=JobCategory.Project) {
				JobProposedVersion proposedVersion=(JobProposedVersion)comboProposedVersion.SelectedIndex;
				_jobNew.ProposedVersion=proposedVersion;
			}
			_jobNew.Requirements=textConcept.MainRtf;
			long jobNum=Jobs.Insert(_jobNew);
			foreach(JobLink link in _listJobLinks) {
				if(_jobNew.Category==JobCategory.Project && link.LinkType!=JobLinkType.JobTeam) {
					continue;
				}
				link.JobNum=jobNum;
				JobLinks.Insert(link);
			}
			if(_jobNew.Category!=JobCategory.Project) {
				foreach(JobQuote quote in _listJobQuotes) {
					quote.JobNum=jobNum;
					JobQuotes.Insert(quote);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}