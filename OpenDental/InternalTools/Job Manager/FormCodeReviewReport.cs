using OpenDentBusiness;
using System;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormCodeReviewReport:FormODBase {
		private List<Userod> _listUserOd=new List<Userod>();
		private string _engineerUserName="";
		private long _engineerUserNum=0;

		public FormCodeReviewReport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormCodeReviewReport_Load(object sender,EventArgs e) {
			FillComboUser();
			dateTimeFrom.Value=DateTime.Now.AddMonths(-1);
			dateTimeTo.Value=DateTime.Now;
		}

		private void FillGrid(List<Job> listJobs) {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.ListGridRows.Clear();
			// Columns
			// The grid will show the JobNum (e.g., B12345), the title, and the Engineer assigned to the job.
			gridMain.Columns.Add(new UI.GridColumn(Lan.g(this,"Engineer"),100,System.Windows.Forms.HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn(Lan.g(this,"JobNum"),100,System.Windows.Forms.HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn(Lan.g(this,"Title"),120));
			// Rows
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listJobs.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				long engineerUserNumForJob=listJobs[i].UserNumEngineer;
				row.Cells.Add((engineerUserNumForJob>0) ? Userods.GetUser(engineerUserNumForJob).UserName : "Unknown");
				row.Cells.Add(listJobs[i].JobNum.ToString());
				row.Cells.Add(listJobs[i].Title);
				row.Tag=listJobs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.Title="Jobs reviewed by "+_engineerUserName;
			gridMain.EndUpdate();
		}

		private void butRunReport_Click(object sender,EventArgs e) {
			DateTime dateFrom=new DateTime(dateTimeFrom.Value.Year,dateTimeFrom.Value.Month,dateTimeFrom.Value.Day,0,0,0);
			DateTime dateTo=new DateTime(dateTimeTo.Value.Year,dateTimeTo.Value.Month,dateTimeTo.Value.Day,23,59,59);
			// We don't bother making this global since it won't ever change unless they edit the search criteria.
			List<Job> listJobs=Jobs.GetJobsWithReviewsByUser(_engineerUserNum,dateFrom,dateTo,checkIncludeInDev.Checked);
			FillGrid(listJobs);
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			Job job=Jobs.GetOneFilled(((Job)gridMain.ListGridRows[e.Row].Tag).JobNum);
			FormJobEdit formJobEdit = new FormJobEdit(job);
			formJobEdit.Show();
		}

		private void FillComboUser() {
			_listUserOd=Userods.GetUsersForJobs();
			comboUser.Items.Clear();
			for(int i=0;i<_listUserOd.Count;i++) {
				comboUser.Items.Add(_listUserOd[i].UserName);
				if(Security.CurUser.UserNum==_listUserOd[i].UserNum) {
					comboUser.SelectedIndex=i;
				}
			}
		}

		private void comboUser_SelectedIndexChanged(object sender,EventArgs e) {
			_engineerUserName=_listUserOd[comboUser.SelectedIndex].UserName;
			_engineerUserNum=_listUserOd[comboUser.SelectedIndex].UserNum;
		}
	}
}
