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
		private const string KEY="kS3o1OTpghLLPg49+Q7WKg==";
		private const string MSG=@"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";


		public FormJobAdd(Job jobNew) {
			_jobNew=jobNew;
			_listJobLinks=new List<JobLink>();
			_listJobQuotes=new List<JobQuote>();
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
		}

		private void FormJobAdd_Load(object sender,EventArgs e) {
			_listCategoryNames=Enum.GetNames(typeof(JobCategory)).ToList();
			_listCategoryNamesFiltered=_listCategoryNames.Where(x => !ListTools.In(x,JobCategory.Query.ToString(),JobCategory.MarketingDesign.ToString())).ToList();
			if(!JobPermissions.IsAuthorized(JobPerm.ProjectManager,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.NeedNoApproval.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.SpecialProject,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.SpecialProject.ToString());
			}
			if(!JobPermissions.IsAuthorized(JobPerm.UnresolvedIssues,true)) {
				_listCategoryNamesFiltered.Remove(JobCategory.UnresolvedIssue.ToString());
			}
			_listCategoryNamesFiltered.ForEach(x=>comboCategory.Items.Add(x));
			comboCategory.SelectedIndex=_listCategoryNamesFiltered.IndexOf(_jobNew.Category.ToString());
			_listPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList();
			_listPriorities.ForEach(x=>comboPriority.Items.Add(x.ItemName,x));
			comboPriority.SelectedIndex=_listPriorities.Select(x => x.DefNum).ToList().IndexOf(_jobNew.Priority);
			Enum.GetNames(typeof(JobProposedVersion)).ToList().ForEach(x => comboProposedVersion.Items.Add(x));
			comboProposedVersion.SelectedIndex=(int)JobProposedVersion.Current;
		}

		#region FillGrids
		private void FillGridCustomers() {
			gridCustomers.BeginUpdate();
			gridCustomers.ListGridColumns.Clear();
			gridCustomers.ListGridColumns.Add(new GridColumn("PatNum",40){ IsWidthDynamic=true });
			gridCustomers.ListGridColumns.Add(new GridColumn("Name",40){ IsWidthDynamic=true });
			gridCustomers.ListGridColumns.Add(new GridColumn("BillType",40){ IsWidthDynamic=true });
			gridCustomers.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
			gridSubscribers.ListGridColumns.Clear();
			gridSubscribers.ListGridColumns.Add(new GridColumn("User Name",40){ IsWidthDynamic=true });
			gridSubscribers.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
			gridQuotes.ListGridColumns.Clear();
			gridQuotes.ListGridColumns.Add(new GridColumn("PatNum",40){ IsWidthDynamic=true });
			gridQuotes.ListGridColumns.Add(new GridColumn("Hours",40,HorizontalAlignment.Center){ IsWidthDynamic=true });
			gridQuotes.ListGridColumns.Add(new GridColumn("Amt",40,HorizontalAlignment.Right){ IsWidthDynamic=true });
			gridQuotes.ListGridColumns.Add(new GridColumn("Appr?",40,HorizontalAlignment.Center));
			gridQuotes.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
			gridTasks.ListGridColumns.Clear();
			gridTasks.ListGridColumns.Add(new GridColumn("Date",40){ IsWidthDynamic=true });
			gridTasks.ListGridColumns.Add(new GridColumn("TaskList",40){ IsWidthDynamic=true });
			gridTasks.ListGridColumns.Add(new GridColumn("Done",40) { TextAlign=HorizontalAlignment.Center });
			gridTasks.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
			gridFeatureReq.ListGridColumns.Clear();
			gridFeatureReq.ListGridColumns.Add(new GridColumn("Feat Req Num",40){ IsWidthDynamic=true });
			gridFeatureReq.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
			gridBugs.ListGridColumns.Clear();
			gridBugs.ListGridColumns.Add(new GridColumn("Flags",50));
			gridBugs.ListGridColumns.Add(new GridColumn("Bug Num (From JRMT)",40){ IsWidthDynamic=true });
			gridBugs.ListGridColumns.Add(new GridColumn("Unlink",40,HorizontalAlignment.Center));
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
				FKey=FormPS.SelectedPatNum,
				LinkType=JobLinkType.Customer
			};
			_listJobLinks.Add(jobLink);
			FillGridCustomers();
		}

		private void gridSubscribers_TitleAddClick(object sender,EventArgs e) {
			using FormUserPick FormUP = new FormUserPick();
			//Suggest current user if not already watching.
			if(_listJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber).All(x => x.FKey!=Security.CurUser.UserNum)) {
				FormUP.SuggestedUserNum=Security.CurUser.UserNum;
			}
			FormUP.IsSelectionmode=true;
			FormUP.ShowDialog();
			if(FormUP.DialogResult!=DialogResult.OK) {
				return;
			}
			JobLink jobLink = new JobLink() {
				FKey=FormUP.SelectedUserNum,
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
			jobLink.FKey=FormTS.SelectedTaskNum;
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

		private bool ValidateTitle() {
			//Key Hint: Nick Fury quote from Captain America Winter Soldier, 3 words, no spaces, CamelCased
			if(Authentication.HashPasswordMD5(textTitle.Text)!=KEY) {
				return true;
			}
			butOK.Enabled=false;
			butCancel.Enabled=false;
			tabControlExtra.SelectedTab=tabConcept;
			tabConcept.BackColor=Color.Black;
			labelCategory.Text="Protect";
			labelPriority.Text="The";
			labelVersion.Text="Program";
			comboCategory.Visible=false;
			comboPriority.Visible=false;
			comboProposedVersion.Visible=false;
			label1.Visible=false;
			textConcept.ReadOnly=true;
			textConcept.TextBox.BackColor=Color.Black;
			textConcept.TextBox.ForeColor=Color.White;
			tabControlExtra.TabPages.Remove(tabCustomers);
			tabControlExtra.TabPages.Remove(tabQuotes);
			tabControlExtra.TabPages.Remove(tabSubscribers);
			tabControlExtra.TabPages.Remove(tabRequests);
			tabControlExtra.TabPages.Remove(tabBugs);
			tabControlExtra.TabPages.Remove(tabTasks);
			BackColor=Color.Black;
			foreach(Control c in this.Controls) {
				c.BackColor=Color.Black;
				c.ForeColor=Color.White;
			}
			Text="AVENGINEERS MODE ACTIVE";
			string str="";
			CDT.Class1.Decrypt(MSG,out str);
			textTitle.Text="Welcome Director Fury";
			textConcept.MainText=str;
			return false;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateTitle()) {
				return;
			}
			if(!ValidateJob()) {
				return;
			}
			JobProposedVersion proposedVersion=(JobProposedVersion)comboProposedVersion.SelectedIndex;
			_jobNew.ProposedVersion=proposedVersion;
			_jobNew.Requirements=textConcept.MainRtf;
			long jobNum=Jobs.Insert(_jobNew);
			foreach(JobLink link in _listJobLinks) {
				link.JobNum=jobNum;
				JobLinks.Insert(link);
			}
			foreach(JobQuote quote in _listJobQuotes) {
				quote.JobNum=jobNum;
				JobQuotes.Insert(quote);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}