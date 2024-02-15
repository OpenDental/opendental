using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormOrthoHardwareSpecs:FormODBase {
		private bool _isChanged;
		private List<OrthoHardwareSpec> _listOrthoHardwareSpecsAll=new List<OrthoHardwareSpec>();
		private List<OrthoHardwareSpec> _listOrthoHardwareSpecsOneType;

		public FormOrthoHardwareSpecs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoHardwareSpecs_Load(object sender,EventArgs e) {
			listType.Items.AddEnums<EnumOrthoHardwareType>();
			listType.SelectedIndex=0;
			FillGrid();
		}

		private void listType_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(int selectedIndex=-1){
			OrthoHardwareSpecs.RefreshCache();
			_listOrthoHardwareSpecsAll=OrthoHardwareSpecs.GetDeepCopy();
			_listOrthoHardwareSpecsOneType=_listOrthoHardwareSpecsAll.FindAll(x=>x.OrthoHardwareType==(EnumOrthoHardwareType)listType.SelectedIndex);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Description"),200);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Color"),40,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoHardwareSpecs","Hidden"),50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listOrthoHardwareSpecsOneType.Count;i++){
				if(_listOrthoHardwareSpecsOneType[i].ItemOrder!=i){
					_listOrthoHardwareSpecsOneType[i].ItemOrder=i;
					OrthoHardwareSpecs.Update(_listOrthoHardwareSpecsOneType[i]);//fixes any item order bugs
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listOrthoHardwareSpecsOneType[i].Description);
				GridCell cell=new GridCell("");//color
				cell.ColorBackG=_listOrthoHardwareSpecsOneType[i].ItemColor;
				row.Cells.Add(cell);
				row.Cells.Add(new GridCell(_listOrthoHardwareSpecsOneType[i].IsHidden?"X":""));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIndex);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormOrthoHardwareSpecEdit formOrthoHardwareSpecEdit=new FormOrthoHardwareSpecEdit();
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur=_listOrthoHardwareSpecsOneType[e.Row];
			formOrthoHardwareSpecEdit.ShowDialog();
			if(formOrthoHardwareSpecEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_isChanged=true;
			FillGrid(formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.ItemOrder);
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormOrthoHardwareSpecEdit formOrthoHardwareSpecEdit=new FormOrthoHardwareSpecEdit();
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur=new OrthoHardwareSpec();
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.IsNew=true;
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.ItemOrder=_listOrthoHardwareSpecsOneType.Count;
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.OrthoHardwareType=(EnumOrthoHardwareType)listType.SelectedIndex;
			formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.ItemColor=Color.Silver;
			formOrthoHardwareSpecEdit.ShowDialog();
			if(formOrthoHardwareSpecEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_isChanged=true;
			FillGrid(formOrthoHardwareSpecEdit.OrthoHardwareSpecCur.ItemOrder);
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(gridMain.GetSelectedIndex()==0) {
				return;
			}
			int idx=gridMain.GetSelectedIndex();
			OrthoHardwareSpec orthoHardwareSpecSelected=_listOrthoHardwareSpecsOneType[idx];
			orthoHardwareSpecSelected.ItemOrder--;
			OrthoHardwareSpecs.Update(orthoHardwareSpecSelected);
			OrthoHardwareSpec orthoHardwareSpecAbove=_listOrthoHardwareSpecsOneType[idx-1];
			orthoHardwareSpecAbove.ItemOrder++;
			OrthoHardwareSpecs.Update(orthoHardwareSpecAbove);
			_isChanged=true;
			FillGrid(orthoHardwareSpecSelected.ItemOrder);
		}

		
		private void butDown_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(gridMain.GetSelectedIndex()==gridMain.ListGridRows.Count-1) {
				return;
			}
			int idx=gridMain.GetSelectedIndex();
			OrthoHardwareSpec orthoHardwareSpecSelected=_listOrthoHardwareSpecsOneType[idx];
			orthoHardwareSpecSelected.ItemOrder++;
			OrthoHardwareSpecs.Update(orthoHardwareSpecSelected);
			OrthoHardwareSpec orthoHardwareSpecBelow=_listOrthoHardwareSpecsOneType[idx+1];
			orthoHardwareSpecBelow.ItemOrder--;
			OrthoHardwareSpecs.Update(orthoHardwareSpecBelow);
			_isChanged=true;
			FillGrid(orthoHardwareSpecSelected.ItemOrder);
		}

		private void FormOrthoHardwareSpecs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.OrthoChartTabs);
			}
		}

	}
}