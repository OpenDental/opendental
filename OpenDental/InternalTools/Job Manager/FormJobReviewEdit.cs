using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormJobReviewEdit:FormODBase {
		private JobReview _jobReviewCur;
		private List<Userod> _listReviewers;
		private Userod secUser;

		///<summary>Used for existing Reviews. Pass in the jobNum and the jobReviewNum.</summary>
		public FormJobReviewEdit(JobReview jobReview) {
			secUser=Security.CurUser;
			_jobReviewCur=jobReview.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Can be null if deleted from this form.</summary>
		public JobReview JobReviewCur {
			get {
				return _jobReviewCur;
			}
		}

		private void FormJobReviewEdit_Load(object sender,EventArgs e) {
			_listReviewers=Userods.GetUsersByJobRole(JobPerm.Writeup,false);
			_listReviewers.ForEach(x => comboReviewer.Items.Add(x.UserName));
			comboReviewer.SelectedIndex=_listReviewers.FindIndex(x => x.UserNum==_jobReviewCur.ReviewerNum);
			//TimeLogs are used for storing job time rather than reviews so we remove it as an option here.
			List<string> listReviewStatusNames=Enum.GetNames(typeof(JobReviewStatus)).Where(x => x!="TimeLog").ToList();
			for(int i=0;i<listReviewStatusNames.Count;i++) {
				listBoxStatus.Items.Add(listReviewStatusNames[i]);
			}
			listBoxStatus.SelectedIndex=(int)_jobReviewCur.ReviewStatus;
			CheckPermissions();
			if(!_jobReviewCur.IsNew) {
				textDateLastEdited.Text=_jobReviewCur.DateTStamp.ToShortDateString();
			} 
			textDescription.Text=_jobReviewCur.Description;
			textReviewTime.Text=_jobReviewCur.TimeReview.TotalMinutes.ToString();
		}

		private void CheckPermissions() {
			//Disable all controls
			comboReviewer.Enabled=false;
			//textDescription.ReadOnly=true;
			listBoxStatus.Enabled=false;
			butDelete.Enabled=false;
			textReviewTime.Enabled=false;
			if(_jobReviewCur.ReviewerNum==0 && JobPermissions.IsAuthorized(JobPerm.Writeup,true,secUser.UserNum)) {//allow any expert to change the expert if there is no expert.
				comboReviewer.Enabled=true;
			}
			if(_jobReviewCur.ReviewerNum==secUser.UserNum || (_jobReviewCur.ReviewerNum==0 && JobPermissions.IsAuthorized(JobPerm.Writeup,true,secUser.UserNum))) { //allow current expert to edit things.
				//textDescription.ReadOnly=false;
				listBoxStatus.Enabled=true;
				butDelete.Enabled=true;
				textReviewTime.Enabled=true;
			}
			if(_jobReviewCur.Description=="" && _jobReviewCur.ReviewStatus!=JobReviewStatus.Done && JobPermissions.IsAuthorized(JobPerm.Writeup,true,secUser.UserNum)) {
				butDelete.Enabled=true;
			}
			if(new[] { JobReviewStatus.Done,JobReviewStatus.NeedsAdditionalWork }.Contains(_jobReviewCur.ReviewStatus)) {
				butDelete.Enabled=false;
			}
		}

		private void butLogin_Click(object sender,EventArgs e) {
			//Logout
			if(secUser.UserNum!=Security.CurUser.UserNum) {
				butLogin.Text=Lan.g(this,"Login as...");
				this.Text=Lan.g(this,"Job Review Edit");
				secUser=Security.CurUser;
				CheckPermissions();
				return;
			}
			//Otherwise login
			long userNumReviewer=0;
			if(JobReviewCur!=null) {
				userNumReviewer=JobReviewCur.ReviewerNum;
			}
			using FormLogOn FormLO=new FormLogOn(userNumReviewer,true);
			FormLO.ShowDialog();
			if(FormLO.DialogResult!=DialogResult.OK) {
				return;
			}
			secUser=FormLO.CurUserSimpleSwitch;
			CheckPermissions();
			if(secUser.UserNum!=Security.CurUser.UserNum) {
				butLogin.Text=Lan.g(this,"Logout");
				this.Text=Lan.g(this,"Job Review Edit")+" - Logged in as "+secUser.UserName;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_jobReviewCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete the current job review?")) {
				_jobReviewCur=null;
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textReviewTime.IsValid() || String.IsNullOrEmpty(textReviewTime.Text)) {
				MsgBox.Show(this,"Please provide a valid number for your review time");
				return;
			}
			if(comboReviewer.SelectedIndex>-1) {
				_jobReviewCur.ReviewerNum=_listReviewers[comboReviewer.SelectedIndex].UserNum;
			}
			_jobReviewCur.TimeReview=new TimeSpan(0,int.Parse(textReviewTime.Text),0);
			//Get from the JobReviewStatus enum since the list in this is missing the TimeLog enum value
			_jobReviewCur.ReviewStatus=(JobReviewStatus)listBoxStatus.SelectedIndex;
			_jobReviewCur.Description=textDescription.Text;
			if(_jobReviewCur.IsNew) {
				_jobReviewCur.DateTStamp=DateTime.Now;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel; //removing new jobs from the DB is taken care of in FormClosing
		}

	}
}