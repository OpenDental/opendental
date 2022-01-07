using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormGridSelection:FormODBase {
		
		///<summary></summary>
		private List<GridColumn> _listGridColumns;
		///<summary></summary>
		private List<GridRow> _listGridRows;
		///<summary>Set to the tag of the rows that are double selected.</summary>
		public List<object> ListSelectedTags=new List<object>();

		/// <summary>listGridRows must have tags set.</summary>
		public FormGridSelection(List<GridColumn> listGridColumns,List<GridRow> listGridRows,string formTitle,string gridTitle,GridSelectionMode selectMode=GridSelectionMode.One) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listGridColumns=listGridColumns;
			_listGridRows=listGridRows;
			this.Text=formTitle;
			gridMain.Title=gridTitle;
			gridMain.SelectionMode=selectMode;
		}
		
		private void FormGridSelection_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			_listGridColumns.ForEach(x => gridMain.ListGridColumns.Add(x) );
			_listGridRows.ForEach(x => gridMain.ListGridRows.Add(x) );
			gridMain.EndUpdate();
		}

		private void gridEras_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			ListSelectedTags=new List<object>(gridMain.SelectedTags<object>());
			this.DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this.Text+" "+gridMain.Title,"No items are selected.  Please select an item before continuing."));
				return;
			}
			ListSelectedTags=new List<object>(gridMain.SelectedTags<object>());
			this.DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}