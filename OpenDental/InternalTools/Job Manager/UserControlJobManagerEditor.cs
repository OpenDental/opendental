using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.InternalTools.Job_Manager;

namespace OpenDental {
	public partial class UserControlJobManagerEditor:UserControl {

		private Job _jobCur=null;
		///<summary>Returns the jobnum loaded into the current active editor. Can return 0.</summary>
		public long JobNumCur {
			get {
				if(userControlJobEdit.Visible) {
					return userControlJobEdit.GetJob()?.JobNum??0;
				}
				else if(userControlQueryEdit.Visible) {
					return userControlQueryEdit.GetJob()?.JobNum??0;
				}
				else if(userControlMarketingEdit.Visible) {
					return userControlMarketingEdit.GetJob()?.JobNum??0;
				}
				return 0;
			}
		}		

		///<summary>Returns the job loaded into the current active editor. Can return null.</summary>
		public Job JobCur {
			get {
				if(userControlJobEdit.Visible) {
					return userControlJobEdit.GetJob();
				}
				else if(userControlQueryEdit.Visible) {
					return userControlQueryEdit.GetJob();
				}
				else if(userControlMarketingEdit.Visible) {
					return userControlMarketingEdit.GetJob();
				}
				return null;
			}
			set {
				_jobCur=value;
			}
		}

		///<summary>Checks the active editor for any job changes. Returns false if no editor is active</summary>
		public bool IsChanged {
			get {
				if(userControlJobEdit.Visible) {
					return userControlJobEdit.IsChanged;
				}
				else if(userControlQueryEdit.Visible) {
					return userControlQueryEdit.IsChanged;
				}
				else if(userControlMarketingEdit.Visible) {
					return userControlMarketingEdit.IsChanged;
				}
				return false;
			}
		}

		///<summary>If the userControlJobManager is open in a popout state, this disables some functionality.</summary>
		public bool IsPopout {
			set {
				userControlJobEdit.IsPopout=value;
				userControlMarketingEdit.IsPopout=value;
				userControlQueryEdit.IsPopout=value;
			}
		}

		public UserControlJobManagerEditor() {
			InitializeComponent();
		}

		///<summary>Loads whatever job control is necessary in order to display the job within the manager.
		///Also refreshes the cache with the job passed in and updates all corresponding controls and grids.
		///This method will not load the passed in job if the current job cannot be saved correctly.</summary>
		public void LoadJob(Job job,bool doRefreshUI,LoadJobAction loadAction = LoadJobAction.Select) {
			if(job==null || UnsavedChangesCheck()) {
				return;
			}
			ShowEditorForJob(job);
		}

		public bool UnsavedChangesCheck() {
			//Depending on which control is visible, 
			if(userControlJobEdit.Visible && userControlJobEdit.IsChanged) {
				switch(MessageBox.Show("Save changes to current job?","",MessageBoxButtons.YesNoCancel)) {
					case System.Windows.Forms.DialogResult.OK:
					case System.Windows.Forms.DialogResult.Yes:
						if(!userControlJobEdit.ForceSave()) {
							return true;
						}
						break;
					case System.Windows.Forms.DialogResult.No:
						Checkin();
						break;
					case System.Windows.Forms.DialogResult.Cancel:
						return true;
				}
			}
			if(userControlQueryEdit.Visible && userControlQueryEdit.IsChanged) {
				switch(MessageBox.Show("Save changes to current job?","",MessageBoxButtons.YesNoCancel)) {
					case System.Windows.Forms.DialogResult.OK:
					case System.Windows.Forms.DialogResult.Yes:
						if(!userControlQueryEdit.ForceSave()) {
							return true;
						}
						break;
					case System.Windows.Forms.DialogResult.No:
						Checkin();
						break;
					case System.Windows.Forms.DialogResult.Cancel:
						return true;
				}
			}
			if(userControlMarketingEdit.Visible && userControlMarketingEdit.IsChanged) {
				switch(MessageBox.Show("Save changes to current job?","",MessageBoxButtons.YesNoCancel)) {
					case System.Windows.Forms.DialogResult.OK:
					case System.Windows.Forms.DialogResult.Yes:
						if(!userControlMarketingEdit.ForceSave()) {
							return true;
						}
						break;
					case System.Windows.Forms.DialogResult.No:
						Checkin();
						break;
					case System.Windows.Forms.DialogResult.Cancel:
						return true;
				}
			}
			return false;//no unsaved changes
		}

		private void Checkin() {
			if(_jobCur==null) {
				return;
			}
			if(_jobCur.UserNumCheckout==Security.CurUser.UserNum) {
				_jobCur=Jobs.GetOne(_jobCur.JobNum);
				_jobCur.UserNumCheckout=0;
				Jobs.Update(_jobCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,_jobCur.JobNum);
			}
		}

		public void ShowEditorForJob(Job jobCur=null) {
			_jobCur=jobCur;
			if(_jobCur!=null) {
				//Forcefully load the new job into whatever control it needs to be loaded into in order to be interacted with.
				if(_jobCur.Category==JobCategory.Query) {
					userControlQueryEdit.Visible=true;
					userControlMarketingEdit.Visible=false;
					userControlJobEdit.Visible=false;
					userControlQueryEdit.LoadJob(_jobCur,Jobs.GetJobTree(_jobCur,JobManagerCore.ListJobsAll));
				}
				else if(_jobCur.Category==JobCategory.MarketingDesign) {
					userControlQueryEdit.Visible=false;
					userControlMarketingEdit.Visible=true;
					userControlJobEdit.Visible=false;
					userControlMarketingEdit.LoadJob(_jobCur,Jobs.GetJobTree(_jobCur,JobManagerCore.ListJobsAll));
				}
				else {
					userControlQueryEdit.Visible=false;
					userControlMarketingEdit.Visible=false;
					userControlJobEdit.Visible=true;
					userControlJobEdit.LoadJob(_jobCur,Jobs.GetJobTree(_jobCur,JobManagerCore.ListJobsAll));
				}
			}
		}

		public void LoadMergeJob(Job jobRefresh) {
			Job jobCur=JobCur;
			if(jobCur==null) {
				return;
			}
			if(jobRefresh==null) {
				return;
			}
			Dictionary<long,Job> dictRefreshJobs=new Dictionary<long,Job>();
			dictRefreshJobs.Add(jobRefresh.JobNum,jobRefresh);
			LoadMergeJob(dictRefreshJobs);
		}

		public void LoadMergeJob(Dictionary<long,Job> dictRefreshJobs) {
			if(JobCur==null) {
				return;
			}
			if(!dictRefreshJobs.TryGetValue(JobCur.JobNum,out Job jobCur)) {
				//Check to see if the current job is included in the list of jobs we need to refresh (could be due to processing a signal).
				return;
			}
			if(jobCur==null) {
				return;
			}
			//Refresh the job in the control with the new version of the job that was just retrieved from the database.
			if(userControlJobEdit.Visible) {
				userControlJobEdit.LoadMergeJob(jobCur);
			}
			if(userControlQueryEdit.Visible) {
				userControlQueryEdit.LoadMergeJob(jobCur);
			}
			if(userControlMarketingEdit.Visible) {
				userControlMarketingEdit.LoadMergeJob(jobCur);
			}
		}

		public bool ForceSave() {
			if(userControlJobEdit.Visible) {
				return userControlJobEdit.ForceSave();
			}
			if(userControlQueryEdit.Visible) {
				return userControlQueryEdit.ForceSave();
			}
			if(userControlMarketingEdit.Visible) {
				return userControlMarketingEdit.ForceSave();
			}
			return true;
		}

		private void userControlJobEdit_SaveClick(object sender,EventArgs e) {
			Dictionary<long,Job> dictRefreshJob=new Dictionary<long, Job>();
			dictRefreshJob.Add(JobNumCur,JobCur);
			JobManagerCore.UpdateCachedLists(dictRefreshJob);
		}

		private void userControlMarketingEdit_SaveClick(object sender,EventArgs e) {
			Dictionary<long,Job> dictRefreshJob=new Dictionary<long, Job>();
			dictRefreshJob.Add(JobNumCur,JobCur);
			JobManagerCore.UpdateCachedLists(dictRefreshJob);
		}

		private void userControlQueryEdit_SaveClick(object sender,EventArgs e) {
			Dictionary<long,Job> dictRefreshJob=new Dictionary<long,Job>();
			dictRefreshJob.Add(JobNumCur,JobCur);
			JobManagerCore.UpdateCachedLists(dictRefreshJob);
		}		
		
		private void userControlJobEdit_RequestJob(object sender,long jobNum) {
			Job job=JobManagerCore.ListJobsAll.FirstOrDefault(x => x.JobNum==jobNum);
			if(job==null) {
				GoToJob(jobNum);//Try and get the job from the database.
				return;
			}
			LoadJob(job,true);
		}

		public void GoToJob(long jobNum) {
			GoToJob(jobNum,LoadJobAction.Select);
		}

		private void GoToJob(long jobNum,LoadJobAction loadAction) {
			Job job=Jobs.GetOneFilled(jobNum);
			if(job==null) {
				MessageBox.Show("Job not found.");
				return;
			}
			LoadJob(job,true,loadAction);
		}
	}
}
