using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskSubscribers:FormODBase {
		public Task TaskForSubscribers;

		public FormTaskSubscribers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskSubscribers_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridODSubscribers.BeginUpdate();
			gridODSubscribers.Columns.Clear();
			GridColumn gridColumn;
			GridRow gridRow;
			gridColumn=new GridColumn("User",-1);
			gridColumn.IsWidthDynamic=true;
			gridODSubscribers.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Read",55,HorizontalAlignment.Center);
			gridODSubscribers.Columns.Add(gridColumn);
			gridODSubscribers.ListGridRows.Clear();
			DataTable tabletaskUnreads=TaskUnreads.GetForTask(TaskForSubscribers.TaskNum);
			for(int i=0;i<tabletaskUnreads.Rows.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(tabletaskUnreads.Rows[i]["User"].ToString());
				gridRow.Cells.Add(tabletaskUnreads.Rows[i]["Unread"].ToString());
				gridODSubscribers.ListGridRows.Add(gridRow);
			}
			gridODSubscribers.EndUpdate();
		}

	}
}