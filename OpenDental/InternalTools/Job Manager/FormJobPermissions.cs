using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;

namespace OpenDental {
	public partial class FormJobPermissions:FormODBase {
		private long _userNum;
		private List<JobPermission> _jobPermissions;

		///<summary>Pass in the jobNum for existing jobs.</summary>
		public FormJobPermissions(long userNum) {
			_userNum=userNum;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobRoles_Load(object sender,EventArgs e) {
			_jobPermissions=JobPermissions.GetForUser(_userNum);
			listAvailable.Items.AddEnums<JobPerm>();
			_jobPermissions.Select(x=>(int)x.JobPermType).Distinct().ToList().ForEach(x=>listAvailable.SetSelected(x));
		}

		private void butEngineer_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Engineer);
			listAvailable.SetSelectedEnum(JobPerm.Concept);
		}

		private void butPreExpert_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Writeup);
			listAvailable.SetSelectedEnum(JobPerm.Assignment);
			listAvailable.SetSelectedEnum(JobPerm.Review);
			listAvailable.SetSelectedEnum(JobPerm.Engineer);
			listAvailable.SetSelectedEnum(JobPerm.Concept);
		}

		private void butExpert_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Writeup);
			listAvailable.SetSelectedEnum(JobPerm.Assignment);
			listAvailable.SetSelectedEnum(JobPerm.Review);
			listAvailable.SetSelectedEnum(JobPerm.Engineer);
			listAvailable.SetSelectedEnum(JobPerm.Concept);
			listAvailable.SetSelectedEnum(JobPerm.Quote);
			listAvailable.SetSelectedEnum(JobPerm.Override);
		}

		private void butTechWriter_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Documentation);
		}

		private void butJobManager_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Assignment);
			listAvailable.SetSelectedEnum(JobPerm.Approval);
			listAvailable.SetSelectedEnum(JobPerm.Concept);
			listAvailable.SetSelectedEnum(JobPerm.Quote);
			listAvailable.SetSelectedEnum(JobPerm.Override);
		}

		private void butFeatureManager_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Concept);
			listAvailable.SetSelectedEnum(JobPerm.FeatureManager);
		}

		private void butQueryManager_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.SeniorQueryCoordinator);
			listAvailable.SetSelectedEnum(JobPerm.QueryCoordinator);
			listAvailable.SetSelectedEnum(JobPerm.QueryTech);
		}

		private void butCustomerManager_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Concept);
			listAvailable.SetSelectedEnum(JobPerm.NotifyCustomer);
		}

		private void butQuoteManager_Click(object sender,EventArgs e) {
			listAvailable.ClearSelected();
			listAvailable.SetSelectedEnum(JobPerm.Concept);
			listAvailable.SetSelectedEnum(JobPerm.Quote);
		}

		private void butOK_Click(object sender,EventArgs e) {
			_jobPermissions.Clear();
			JobPermission jobPermission;
			List<JobPerm> listJobPermEnum=listAvailable.GetListSelected<JobPerm>();
			for(int i=0;i<listJobPermEnum.Count;i++) {
				jobPermission=new JobPermission();
				jobPermission.UserNum=_userNum;
				jobPermission.JobPermType=listJobPermEnum[i];
				_jobPermissions.Add(jobPermission);
			}
			JobPermissions.Sync(_jobPermissions,_userNum);
			DataValid.SetInvalid(InvalidType.JobPermission);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}