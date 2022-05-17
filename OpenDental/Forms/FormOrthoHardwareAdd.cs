using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormOrthoHardwareAdd:FormODBase {
		private bool _changed;
		private List<OrthoHardwareSpec> _listOrthoHardwareSpecs;
		public List<OrthoHardwareSpec> ListOrthoHardwareSpecsSelected;

		public FormOrthoHardwareAdd() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoHardwareAdd_Load(object sender,EventArgs e) {
			_listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy(isShort:true);
			FillGrid();
		}

		private void listType_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Type"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Description"),200);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Color"),40);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listOrthoHardwareSpecs.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_listOrthoHardwareSpecs[i].OrthoHardwareType.ToString());
				row.Cells.Add(_listOrthoHardwareSpecs[i].Description);
				GridCell cell=new GridCell("");//color
				cell.ColorBackG=_listOrthoHardwareSpecs[i].ItemColor;
				row.Cells.Add(cell);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ListOrthoHardwareSpecsSelected=new List<OrthoHardwareSpec>();
			ListOrthoHardwareSpecsSelected.Add(_listOrthoHardwareSpecs[e.Row]);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please selecte one or more items first.");
				return;
			}
			ListOrthoHardwareSpecsSelected=new List<OrthoHardwareSpec>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				ListOrthoHardwareSpecsSelected.Add(_listOrthoHardwareSpecs[gridMain.SelectedIndices[i]]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}