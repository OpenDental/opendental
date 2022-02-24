using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListAdvancedSearch:FormODBase {
		public int[] SelectedColumnIndices=new int[0];
		private List<WikiListHeaderWidth> _listColHeaders;

		public FormWikiListAdvancedSearch(List<WikiListHeaderWidth> headers) {
			InitializeComponent();
			InitializeLayoutManager();
			_listColHeaders=headers;
			Lan.F(this);
		}

		private void FormWikiListAdvancedSearch_Load(object sender,EventArgs e) {
			FillGrid();
		}

		/// <summary>Populates the grid with the current Wiki's column headers</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("",80));
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(_listColHeaders.Select(x => new GridRow(x.ColName)));
			gridMain.EndUpdate();
			if(SelectedColumnIndices.Length>0) {
				gridMain.SetSelected(SelectedColumnIndices,true);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SelectedColumnIndices=gridMain.SelectedIndices;
			DialogResult=DialogResult.OK;
		}

	}
}
