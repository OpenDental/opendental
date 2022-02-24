using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.IO;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormUcums:FormODBase {
		public bool IsSelectionMode;
		public Ucum SelectedUcum;
		private List<Ucum> _listUcum;

		public FormUcums() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUcums_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text=Lan.g(this,"Cancel");
			}
			else {
				butOK.Visible=false;
			}
			ActiveControl=textCode;
		}
		
		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("UCUM Code",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",500);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listUcum=Ucums.GetBySearchText(textCode.Text);
			for(int i=0;i<_listUcum.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listUcum[i].UcumCode);
				row.Cells.Add(_listUcum[i].Description);
				row.Tag=_listUcum[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				SelectedUcum=(Ucum)gridMain.ListGridRows[e.Row].Tag;
				DialogResult=DialogResult.OK;
				return;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedUcum=(Ucum)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}