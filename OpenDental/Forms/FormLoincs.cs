using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using OpenDental;

namespace OpenDental {
	public partial class FormLoincs:FormODBase {
		public bool IsSelectionMode;
		public Loinc LoincSelected;
		private List<Loinc> _listLoincsSearch;
		public Loinc LoincCur;

		public FormLoincs() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLoincPicker_Load(object sender,EventArgs e) {
			_listLoincsSearch=new List<Loinc>();
			ActiveControl=textCode;
		}

		private void fillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Loinc Code",80);//,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("Status",80);//,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("Long Name",500);//,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("UCUM Units",100);//,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("Order or Observation",100);//,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listLoincsSearch=Loincs.GetBySearchString(textCode.Text);
			for(int i=0;i<_listLoincsSearch.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listLoincsSearch[i].LoincCode);
				row.Cells.Add(_listLoincsSearch[i].StatusOfCode);
				row.Cells.Add(_listLoincsSearch[i].NameLongCommon);
				row.Cells.Add(_listLoincsSearch[i].UnitsUCUM);
				row.Cells.Add(_listLoincsSearch[i].OrderObs);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Nothing to do if not selection mode
			if(!IsSelectionMode){
				return;
			}
			LoincSelected=_listLoincsSearch[e.Row];
			DialogResult=DialogResult.OK;
		}

		private void butSearch_Click(object sender,EventArgs e) {
			fillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a Loinc code from the list.");
				return;
			}
			if(IsSelectionMode) {
				LoincSelected=_listLoincsSearch[gridMain.GetSelectedIndex()];
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
