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
		public Ucum UcumSelected;
		private List<Ucum> _listUcums;

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
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("UCUM Code",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",500);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listUcums=Ucums.GetBySearchText(textCode.Text);
			for(int i=0;i<_listUcums.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listUcums[i].UcumCode);
				row.Cells.Add(_listUcums[i].Description);
				row.Tag=_listUcums[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				UcumSelected=(Ucum)gridMain.ListGridRows[e.Row].Tag;
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
			UcumSelected=(Ucum)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}