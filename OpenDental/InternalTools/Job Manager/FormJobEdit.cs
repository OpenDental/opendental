using System.Collections.Generic;
using OpenDentBusiness;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using OpenDental.NewCrop;

namespace OpenDental {
	public partial class FormJobEdit:FormODBase {

		public Job JobCur {
			get {
				return userControlJobManagerEditor.JobCur;
			}
		}

		public FormJobEdit(Job job) {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
			Text="Job: "+job.ToString();
			userControlJobManagerEditor.IsPopout=true;
			userControlJobManagerEditor.LoadJob(job,true);
			userControlJobManagerEditor.ShowEditorForJob();
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(!listSignals.Exists(x => x.IType==InvalidType.Jobs)) {
				return;//no job signals;
			}
			//Get the job nums from the signals passed in.
			List<long> listJobNums=listSignals.FindAll(x => x.IType==InvalidType.Jobs && x.FKeyType==KeyType.Job)
				.Select(x => x.FKey)
				.Distinct()
				.ToList();
			if(listJobNums.Contains(userControlJobManagerEditor.JobNumCur)) {
				Job jobMerge=Jobs.GetOneFilled(userControlJobManagerEditor.JobNumCur);
				userControlJobManagerEditor.LoadMergeJob(jobMerge);
			}
		}

		private bool JobUnsavedChangesCheck() {
			if(userControlJobManagerEditor.IsChanged) {
				switch(MessageBox.Show($"Save changes to Job: {userControlJobManagerEditor.JobCur.ToString()}","",MessageBoxButtons.YesNoCancel)) {
					case System.Windows.Forms.DialogResult.OK:
					case System.Windows.Forms.DialogResult.Yes:
						if(!userControlJobManagerEditor.ForceSave()) {
							return true;
						}
						break;
					case System.Windows.Forms.DialogResult.No:
						CheckinJob();
						break;
					case System.Windows.Forms.DialogResult.Cancel:
						return true;
				}
			}
			return false;//no unsaved changes
		}

		private void CheckinJob() {
			Job jobCur=userControlJobManagerEditor.JobCur;
			if(jobCur==null) {
				return;
			}
			if(jobCur.UserNumCheckout==Security.CurUser.UserNum) {
				jobCur=Jobs.GetOne(jobCur.JobNum);
				jobCur.UserNumCheckout=0;
				Jobs.Update(jobCur);
				Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobCur.JobNum);
			}
		}

		private void FormJobEdit_Activated(object sender,System.EventArgs e) {
			if(WindowState==FormWindowState.Minimized) {
				this.WindowState=FormWindowState.Normal;
			}
		}

		private void FormJobEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(JobUnsavedChangesCheck()) {
				e.Cancel=true;
				return;
			}
			FormJobManager.RemoveFormJobEdit(this);
		}
	}
}