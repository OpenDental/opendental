using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListAdvancedSearch:FormODBase {
		public int[] IntArrayColumnIndicesSelected=new int[0];
		private List<WikiListHeaderWidth> _listWikiListHeaderWidths;

		public FormWikiListAdvancedSearch(List<WikiListHeaderWidth> listWikiListHeaderWidths) {
			InitializeComponent();
			InitializeLayoutManager();
			_listWikiListHeaderWidths=listWikiListHeaderWidths;
			Lan.F(this);
		}

		private void FormWikiListAdvancedSearch_Load(object sender,EventArgs e) {
			FillGrid();
		}

		/// <summary>Populates the grid with the current Wiki's column headers</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("",80));
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(_listWikiListHeaderWidths.Select(x => new GridRow(x.ColName)));
			gridMain.EndUpdate();
			if(IntArrayColumnIndicesSelected.Length>0) {
				gridMain.SetSelected(IntArrayColumnIndicesSelected,true);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			IntArrayColumnIndicesSelected=gridMain.SelectedIndices;
			DialogResult=DialogResult.OK;
		}

	}
}
