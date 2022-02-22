using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormBugSubmission:FormODBase {
		///<summary></summary>
		private BugSubmission _bugSubmission;
		///<summary>Used to determine if a new bug should show (Enhancement) in the description.</summary>
		private Job _job;
		///<summary>Null unless a bug is added  or alrady exists.</summary>
		private Bug _bug;
		///<summary>The current patient associated to the selected bug submission row. Null if no row selected or if multiple rows selected.</summary>
		private Patient _patient;
		///<summary></summary>
		private List<JobLink> _listJobLinks=new List<JobLink>();

		public FormBugSubmission(BugSubmission bugSubmission,Job job=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_bugSubmission=bugSubmission;
			_job=job;
		}

		private void FormBugSubmission_Load(object sender,EventArgs e) {
			try {
				RegistrationKey registrationKey=RegistrationKeys.GetByKey(_bugSubmission.RegKey);
				_patient=Patients.GetPat(registrationKey.PatNum);
			}
			catch(Exception ex) {
				ex.DoNothing();
				_patient=new Patient();//Just in case, needed mostly for debug.
			}
			labelName.Text=_patient?.GetNameLF()??"";
			labelDateTime.Text=POut.DateT(_bugSubmission.SubmissionDateTime);
			labelVersion.Text=_bugSubmission.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0");
			labelHashNum.Text=POut.Long(_bugSubmission.BugSubmissionHashNum);
			if(_bugSubmission.BugId!=0) {//Already associated to a bug
				_bug=Bugs.GetOne(_bugSubmission.BugId);
				butAddViewBug.Text="View Bug";
			}
			if(_bug!=null) {
				_listJobLinks=JobLinks.GetForType(JobLinkType.Bug,_bug.BugId);
				if(_listJobLinks.Count==1) {
					butAddViewJob.Text="View Job";
				}
			}
			Dictionary<string,Patient> dictionaryPats=new Dictionary<string,Patient>();
			dictionaryPats.Add(_bugSubmission.RegKey,_patient);
			bugSubmissionControl.RefreshData(dictionaryPats,-1,null);//New selelction, refresh control data.
			bugSubmissionControl.RefreshView(_bugSubmission);
		}

		private void butAddViewBug_Click(object sender,EventArgs e) {
			if(butAddViewBug.Text=="View Bug") {
				OpenBug(_bugSubmission);
				return;
			}
			if(AddBug()) {//Bug added.
				butAddViewBug.Text="View Bug";
			}
		}
		
		private void OpenBug(BugSubmission bugSubmission) {
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			using FormBugEdit formBugEdit=new FormBugEdit();
			formBugEdit.BugCur=Bugs.GetOne(bugSubmission.BugId);
			if(formBugEdit.ShowDialog()==DialogResult.OK && formBugEdit.BugCur==null) {//Bug was deleted.
				_bug=null;
				butAddViewBug.Text="Add Bug";
			}
		}

		private bool AddBug() {
			if(butAddViewBug.Text=="View Bug") {
				using FormBugEdit formBugEdit=new FormBugEdit();
				formBugEdit.BugCur=_bug;
				formBugEdit.ShowDialog();
				return false;
			}
			_bug=BugSubmissionL.AddBug(_bugSubmission,_job);
			return (_bug==null);
		}
		
		private void butAddViewJob_Click(object sender,EventArgs e) {
			if(_patient==null) {
				return;
			}
			if(_listJobLinks.Count>0) {//View Job
				if(_listJobLinks.Count==1) {
					JobLink jobLink=_listJobLinks.First();	
					FormOpenDental.S_GoToJob(jobLink.JobNum);
				}
				else {
					MsgBox.Show(this,"Submission is associated to multiple jobs");
				}
				return;
			}
			_bug=BugSubmissionL.AddBugAndJob(this,new List<BugSubmission>() { _bugSubmission },_patient);
			if(_bug==null) {
				return;
			}
			if(this.Modal) {
				DialogResult=DialogResult.OK;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}