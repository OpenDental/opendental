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
		private BugSubmission _subCur;
		///<summary>Used to determine if a new bug should show (Enhancement) in the description.</summary>
		private Job _jobCur;
		///<summary>Null unless a bug is added  or alrady exists.</summary>
		private Bug _bug;
		///<summary>The current patient associated to the selected bug submission row. Null if no row selected or if multiple rows selected.</summary>
		private Patient _patCur;
		///<summary></summary>
		private List<JobLink> _listLinks=new List<JobLink>();

		public FormBugSubmission(BugSubmission sub,Job job=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_subCur=sub;
			_jobCur=job;
		}

		private void FormBugSubmission_Load(object sender,EventArgs e) {
			try {
				RegistrationKey key=RegistrationKeys.GetByKey(_subCur.RegKey);
				_patCur=Patients.GetPat(key.PatNum);
			}
			catch(Exception ex) {
				ex.DoNothing();
				_patCur=new Patient();//Just in case, needed mostly for debug.
			}
			labelName.Text=_patCur?.GetNameLF()??"";
			labelDateTime.Text=POut.DateT(_subCur.SubmissionDateTime);
			labelVersion.Text=_subCur.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0");
			labelHashNum.Text=POut.Long(_subCur.BugSubmissionHashNum);
			if(_subCur.BugId!=0) {//Already associated to a bug
				_bug=Bugs.GetOne(_subCur.BugId);
				butAddViewBug.Text="View Bug";
			}
			if(_bug!=null) {
				_listLinks=JobLinks.GetForType(JobLinkType.Bug,_bug.BugId);
				if(_listLinks.Count==1) {
					butAddViewJob.Text="View Job";
				}
			}
			Dictionary<string,Patient> dictPats=new Dictionary<string,Patient>();
			dictPats.Add(_subCur.RegKey,_patCur);
			bugSubmissionControl.RefreshData(dictPats,-1,null);//New selelction, refresh control data.
			bugSubmissionControl.RefreshView(_subCur);
		}

		private void butAddViewBug_Click(object sender,EventArgs e) {
			if(butAddViewBug.Text=="View Bug") {
				OpenBug(_subCur);
				return;
			}
			if(AddBug()) {//Bug added.
				butAddViewBug.Text="View Bug";
			}
		}
		
		private void OpenBug(BugSubmission sub) {
			if(!JobPermissions.IsAuthorized(JobPerm.Concept,true)
				&& !JobPermissions.IsAuthorized(JobPerm.NotifyCustomer,true)
				&& !JobPermissions.IsAuthorized(JobPerm.FeatureManager,true)
				&& !JobPermissions.IsAuthorized(JobPerm.Documentation,true)) 
			{
				return;
			}
			using FormBugEdit FormBE=new FormBugEdit();
			FormBE.BugCur=Bugs.GetOne(sub.BugId);
			if(FormBE.ShowDialog()==DialogResult.OK && FormBE.BugCur==null) {//Bug was deleted.
				_bug=null;
				butAddViewBug.Text="Add Bug";
			}
		}

		private bool AddBug() {
			if(butAddViewBug.Text=="View Bug") {
				using FormBugEdit formBE=new FormBugEdit();
				formBE.BugCur=_bug;
				formBE.ShowDialog();
				return false;
			}
			_bug=BugSubmissionL.AddBug(_subCur,_jobCur);
			return (_bug==null);
		}
		
		private void butAddViewJob_Click(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			if(_listLinks.Count>0) {//View Job
				if(_listLinks.Count==1) {
					JobLink link=_listLinks.First();	
					FormOpenDental.S_GoToJob(link.JobNum);
				}
				else {
					MsgBox.Show(this,"Submission is associated to multiple jobs");
				}
				return;
			}
			_bug=BugSubmissionL.AddBugAndJob(this,new List<BugSubmission>() { _subCur },_patCur);
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