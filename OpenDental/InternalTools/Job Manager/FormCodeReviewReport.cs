using OpenDentBusiness;
using System;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormCodeReviewReport:FormODBase {
		private List<Userod> _listUserOd=new List<Userod>();

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
			gridMain.Title="Jobs reviewed by "+_listUserOd[comboUser.SelectedIndex].UserName;
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
				row.Cells.Add(Userods.GetUser(listJobs[i].UserNumEngineer).UserName);
				row.Cells.Add(listJobs[i].JobNum.ToString());
				row.Cells.Add(listJobs[i].Title);
				row.Tag=listJobs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butRunReport_Click(object sender,EventArgs e) {
			// We don't bother making this global since it won't ever change unless they edit the search criteria.
			List<Job> listJobs=Jobs.GetJobsWithReviewsByUser(_listUserOd[comboUser.SelectedIndex].UserNum,dateTimeFrom.Value,dateTimeTo.Value);
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
	}
}
