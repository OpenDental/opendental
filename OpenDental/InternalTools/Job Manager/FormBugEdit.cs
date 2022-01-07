using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBugEdit:Form {
		public Bug BugCur;
		public bool IsNew;
		private List<BugSubmission> _listBugSubs=new List<BugSubmission>();

		///<summary>When listBugSubs is set, on OK click all BugSubmissions.BugId will be updated to the current bugId.
		///Seting listBugSubs will add to any existing BugSubmissions they may or may not exist  that areassociated to BugCur.</summary>
		public FormBugEdit(List<BugSubmission> listBugSubs=null) {
			InitializeComponent();
			if(listBugSubs!=null) {
				_listBugSubs.AddRange(listBugSubs);
			}
		}

		private void FormBugEdit_Load(object sender,EventArgs e) {
			if(BugCur==null) {
				MsgBox.Show(this,"An invalid bug was attempted to be loaded.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			textBugId.Text=BugCur.BugId.ToString();
			textCreationDate.Text=BugCur.CreationDate.ToString();
			comboStatus.Text=BugCur.Status_.ToString();
			comboPriority.Text=BugCur.PriorityLevel.ToString();
			textVersionsFound.Text=BugCur.VersionsFound;
			textVersionsFixed.Text=BugCur.VersionsFixed;
			textDescription.Text=BugCur.Description;
			textLongDesc.Text=BugCur.LongDesc;
			textPrivateDesc.Text=BugCur.PrivateDesc;
			textDiscussion.Text=BugCur.Discussion;
			textSubmitter.Text=Bugs.GetSubmitterName(BugCur.Submitter);
			if(!IsNew) {
				_listBugSubs.AddRange(BugSubmissions.GetForBugId(BugCur.BugId));
			}
			FillGrid();
		}
		
		private void FillGrid() {
			gridSubs.BeginUpdate();
			if(gridSubs.ListGridColumns.Count==0) {
				gridSubs.ListGridColumns.Add(new GridColumn("Reg Key",125));
				gridSubs.ListGridColumns.Add(new GridColumn("Version",60,GridSortingStrategy.VersionNumber));
				gridSubs.ListGridColumns.Add(new GridColumn("DateTime",75,GridSortingStrategy.DateParse));
				gridSubs.AllowSortingByColumn=true;
			}
			gridSubs.ListGridRows.Clear();
			foreach(BugSubmission sub in _listBugSubs){
				GridRow row=new GridRow();
				row.Cells.Add(sub.RegKey);
				row.Cells.Add(sub.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"));
				row.Cells.Add(sub.SubmissionDateTime.ToString());
				row.Tag=sub;
				gridSubs.ListGridRows.Add(row);
			}
			gridSubs.EndUpdate();
			gridSubs.SortForced(1,true);/*Sort by Version column*/
		}

		private void gridSubs_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			gridSubs.ContextMenu=new ContextMenu();
			ContextMenu menu=gridSubs.ContextMenu;
			BugSubmission sub=(BugSubmission)gridSubs.ListGridRows[e.Row].Tag;
			menu.MenuItems.Add("Unlink Submission",(o,arg) => {
				_listBugSubs.Remove(sub);
				BugSubmissions.UpdateBugIds(0,new List<BugSubmission>() { sub });
				FillGrid();
			});
			menu.Show(gridSubs,gridSubs.PointToClient(Cursor.Position));
		}

		private void gridSubs_TitleAddClick(object sender,EventArgs e) {
			using FormBugSubmissions FormBS=new FormBugSubmissions(formBugSubmissionMode:FormBugSubmissionMode.SelectionMode);
			if(FormBS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_listBugSubs.AddRange(FormBS.ListBugSubmissionsSelected);
			FillGrid();
		}
		
		private void gridSubs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			FormBugSubmission formBugSub=new FormBugSubmission(gridSubs.ListGridRows[e.Row].Tag as BugSubmission);
			formBugSub.Show();
		}

		private void butCopyDown_Click(object sender,EventArgs e) {
			textVersionsFixed.Text=textVersionsFound.Text;
		}

		private void butLast1found_Click(object sender,EventArgs e) {
			textVersionsFound.Text=VersionReleases.GetLastReleases(1);
		}

		private void butLast2found_Click(object sender,EventArgs e) {
			textVersionsFound.Text=VersionReleases.GetLastReleases(2);
		}

		private void butLast3found_Click(object sender,EventArgs e) {
			textVersionsFound.Text=VersionReleases.GetLastReleases(3);
		}

		private void butLast1_Click(object sender,EventArgs e) {
			textVersionsFixed.Text=VersionReleases.GetLastReleases(1);
		}

		private void butLast2_Click(object sender,EventArgs e) {
			textVersionsFixed.Text=VersionReleases.GetLastReleases(2);
		}

		private void butLast3_Click(object sender,EventArgs e) {
			textVersionsFixed.Text=VersionReleases.GetLastReleases(3);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will also delete all attachments to jobs and bug submissions for this bug. Do you wish to continue?")) {
				return;
			}
			Bugs.Delete(BugCur.BugId);
			BugSubmissions.UpdateBugIds(0,_listBugSubs);
			BugCur=null;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butLeaveStatus_Click(object sender,EventArgs e) {
			BugCur.Status_=(BugStatus)Enum.Parse(typeof(BugStatus),comboStatus.Text);
			SaveToDb();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(BugCur.Submitter==0) {
				MessageBox.Show("A valid submitter wasn't picked.  Make sure the computer being used is associated to a buguser.");
				return;
			}
			if(!Bugs.VersionsAreValid(textVersionsFixed.Text)){
				MsgBox.Show("Please fix your version format. Must be like '18.4.8.0;19.1.22.0;19.2.3.0'");
				return;
			}
			if(textVersionsFixed.Text!=""){
				BugCur.Status_=BugStatus.Fixed;
			}
			else if(comboStatus.SelectedIndex==0) {//none
				BugCur.Status_=BugStatus.Accepted;
			}
			else{
				BugCur.Status_=(BugStatus)Enum.Parse(typeof(BugStatus),comboStatus.Text);
			}
			SaveToDb();
			BugSubmissions.UpdateBugIds(BugCur.BugId,_listBugSubs);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void SaveToDb(){
			//BugId
			//CreationDate
			BugCur.Type_=BugType.Bug;//user can't change
			BugCur.PriorityLevel=PIn.Int(comboPriority.Text);
			BugCur.VersionsFound=textVersionsFound.Text;
			BugCur.VersionsFixed=textVersionsFixed.Text;
			BugCur.Description=textDescription.Text;
			BugCur.LongDesc=textLongDesc.Text;
			BugCur.PrivateDesc=textPrivateDesc.Text;
			BugCur.Discussion=textDiscussion.Text;
			//Submitter
			if(IsNew){
				Bugs.Insert(BugCur);
			}
			else{
				Bugs.Update(BugCur);
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}