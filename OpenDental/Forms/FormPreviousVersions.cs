using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPreviousVersions:FormODBase {

		public FormPreviousVersions() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPreviousVersions_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Version"),117);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),117);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row=null;
			List<UpdateHistory> listUpdateHistories=UpdateHistories.GetAll();
			foreach(UpdateHistory updateHistory in listUpdateHistories) {
				row=new GridRow();
				row.Cells.Add(updateHistory.ProgramVersion);
				row.Cells.Add(updateHistory.DateTimeUpdated.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}