using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormJobSprints:FormODBase {
		public JobSprint SelectedJobSprint;

		///<summary>Opens with links to the passed in JobNum.</summary>
		public FormJobSprints() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobLinks_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<JobSprint> listJobSprints=JobSprints.GetAll();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridRows.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Date Start",70,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
			gridMain.ListGridColumns.Add(new GridColumn("Date End",70,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
			gridMain.ListGridColumns.Add(new GridColumn("Title",70){ IsWidthDynamic=true });
			foreach(JobSprint jobSprint in listJobSprints) {
				gridMain.ListGridRows.Add(
					new GridRow(
						new GridCell(jobSprint.DateStart.ToShortDateString()),
						new GridCell(jobSprint.DateEndTarget.ToShortDateString()),
						new GridCell(jobSprint.Title))
					{
						Tag=jobSprint
					}
				);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectedJobSprint=gridMain.SelectedTag<JobSprint>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				MsgBox.Show(this,"A sprint must be selected before opening the dashboard.");
				return;
			}
			SelectedJobSprint=gridMain.SelectedTag<JobSprint>();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}