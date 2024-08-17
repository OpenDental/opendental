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
	public partial class FormHcpcs:FormODBase {
		public bool IsSelectionMode;
		public Hcpcs HcpcsSelected;
		private List<Hcpcs> listHcpcses;

		public FormHcpcs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHcpcs_Load(object sender,EventArgs e) {
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
			col=new GridColumn("HCPCS Code",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",500);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			listHcpcses=Hcpcses.GetBySearchText(textCode.Text);
			for(int i=0;i<listHcpcses.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listHcpcses[i].HcpcsCode);
				row.Cells.Add(listHcpcses[i].DescriptionShort);
				row.Tag=listHcpcses[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				HcpcsSelected=(Hcpcs)gridMain.ListGridRows[e.Row].Tag;
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			HcpcsSelected=(Hcpcs)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	}
}