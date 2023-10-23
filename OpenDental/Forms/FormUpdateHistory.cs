using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormUpdateHistory:FormODBase {

		public FormUpdateHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUpdateHistory_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Version"),117);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),117);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row=null;
			List<UpdateHistory> listUpdateHistories=UpdateHistories.GetAll().OrderByDescending(x => x.DateTimeUpdated).ToList();
			for(int i = 0;i<listUpdateHistories.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listUpdateHistories[i].ProgramVersion);
				row.Cells.Add(listUpdateHistories[i].DateTimeUpdated.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}