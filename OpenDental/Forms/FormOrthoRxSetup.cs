using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormOrthoRxSetup:FormODBase {
		private List<OrthoRx> _listOrthoRxs;
		private bool _changed;

		public FormOrthoRxSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoRxSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(int selectedIndex=-1){
			OrthoRxs.RefreshCache();
			_listOrthoRxs=OrthoRxs.GetDeepCopy();
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxs","Hardware Spec"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxs","Description"),230);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxs","Teeth"),200);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxs","Color"),40);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listOrthoRxs.Count;i++){
				if(_listOrthoRxs[i].ItemOrder!=i){
					_listOrthoRxs[i].ItemOrder=i;
					OrthoRxs.Update(_listOrthoRxs[i]);//fixes any item order bugs
				}
				OrthoHardwareSpec orthoHardwareSpec=listOrthoHardwareSpecs.Find(x=>x.OrthoHardwareSpecNum==_listOrthoRxs[i].OrthoHardwareSpecNum);
				GridRow row=new GridRow();
				row.Cells.Add(orthoHardwareSpec.Description);
				row.Cells.Add(_listOrthoRxs[i].Description);
				ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
				if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
					toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
				}
				string txtCell="";
				if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
					txtCell=Tooth.DisplayOrthoCommas(_listOrthoRxs[i].ToothRange,toothNumberingNomenclature);
				}
				if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
					txtCell=Tooth.DisplayOrthoDash(_listOrthoRxs[i].ToothRange,toothNumberingNomenclature);
				}
				row.Cells.Add(txtCell);
				GridCell cell=new GridCell("");//color
				cell.ColorBackG=orthoHardwareSpec.ItemColor;
				row.Cells.Add(cell);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIndex);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormOrthoRxEdit formOrthoRxEdit=new FormOrthoRxEdit();
			formOrthoRxEdit.OrthoRxCur=_listOrthoRxs[e.Row];
			formOrthoRxEdit.ShowDialog();
			if(formOrthoRxEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_changed=true;
			FillGrid(formOrthoRxEdit.OrthoRxCur.ItemOrder);
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormOrthoRxEdit formOrthoRxEdit=new FormOrthoRxEdit();
			formOrthoRxEdit.OrthoRxCur=new OrthoRx();
			formOrthoRxEdit.OrthoRxCur.IsNew=true;
			formOrthoRxEdit.OrthoRxCur.ItemOrder=_listOrthoRxs.Count;
			formOrthoRxEdit.ShowDialog();
			if(formOrthoRxEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_changed=true;
			FillGrid(formOrthoRxEdit.OrthoRxCur.ItemOrder);
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
			OrthoRx orthoRxSelected=_listOrthoRxs[idx];
			orthoRxSelected.ItemOrder--;
			OrthoRxs.Update(orthoRxSelected);
			OrthoRx orthoHardwareSpecAbove=_listOrthoRxs[idx-1];
			orthoHardwareSpecAbove.ItemOrder++;
			OrthoRxs.Update(orthoHardwareSpecAbove);
			_changed=true;
			FillGrid(orthoRxSelected.ItemOrder);
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
			OrthoRx orthoRxSelected=_listOrthoRxs[idx];
			orthoRxSelected.ItemOrder++;
			OrthoRxs.Update(orthoRxSelected);
			OrthoRx orthoRxBelow=_listOrthoRxs[idx+1];
			orthoRxBelow.ItemOrder--;
			OrthoRxs.Update(orthoRxBelow);
			_changed=true;
			FillGrid(orthoRxSelected.ItemOrder);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormOrthoHardwareSpecs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_changed){
				DataValid.SetInvalid(InvalidType.OrthoChartTabs);
			}
		}
		
	}
}