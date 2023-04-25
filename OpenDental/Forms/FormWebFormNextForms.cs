using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;

namespace OpenDental {
	public partial class FormWebFormNextForms:FormODBase {
		///<summary>List of all sheet defs. This is passed in and never changed.</summary>
		private List<WebForms_SheetDef> _listWebForms_SheetDefsAll;
		///<summary>List of sheet defs selected. This is manipulated with butLeft, butRight, butUp, butDown.</summary>
		private List<WebForms_SheetDef> _listWebForms_SheetDefsSelected;
		///<summary>List of sheet defs selected when exiting the form with OK click.</summary>
		public List<WebForms_SheetDef> ListWebForms_SheetDefsSelected;

		public FormWebFormNextForms(List<WebForms_SheetDef> listWebForms_SheetDefsAll,List<WebForms_SheetDef> listWebForms_SheetDefsSelected) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listWebForms_SheetDefsAll=listWebForms_SheetDefsAll;
			_listWebForms_SheetDefsSelected=listWebForms_SheetDefsSelected;
		}

		private void FormWebFormNextForms_Load(object sender,EventArgs e) {
			FillGrids();
		}

		private void FillGrids() {
			#region gridWebFormsAvailable
			gridWebFormsAvailable.BeginUpdate();
			gridWebFormsAvailable.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormWebFormNextForms","Description"),0);
			gridWebFormsAvailable.Columns.Add(col);
			gridWebFormsAvailable.ListGridRows.Clear();
			GridRow row;
			List<WebForms_SheetDef> listWebForms_SheetDefsAvailable=_listWebForms_SheetDefsAll.FindAll(x => !_listWebForms_SheetDefsSelected.Contains(x));
			for(int i=0;i<listWebForms_SheetDefsAvailable.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listWebForms_SheetDefsAvailable[i].Description);
				row.Tag=listWebForms_SheetDefsAvailable[i];
				gridWebFormsAvailable.ListGridRows.Add(row);
			}
			gridWebFormsAvailable.EndUpdate();
			#endregion
			#region gridWebFormsSelected
			gridWebFormsSelected.BeginUpdate();
			gridWebFormsSelected.Columns.Clear();
			col=new GridColumn(Lan.g("FormWebFormNextForms","Description"),0);
			gridWebFormsSelected.Columns.Add(col);
			gridWebFormsSelected.ListGridRows.Clear();
			for(int i=0;i<_listWebForms_SheetDefsSelected.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listWebForms_SheetDefsSelected[i].Description);
				row.Tag=_listWebForms_SheetDefsSelected[i];
				gridWebFormsSelected.ListGridRows.Add(row);
			}
			gridWebFormsSelected.EndUpdate();
			#endregion
		}

		private void butLeft_Click(object sender,EventArgs e) {
			List<WebForms_SheetDef> listWebForms_SheetDefsToRemove=gridWebFormsSelected.SelectedTags<WebForms_SheetDef>();
			if(listWebForms_SheetDefsToRemove==null || listWebForms_SheetDefsToRemove.Count==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			for(int i=0;i<listWebForms_SheetDefsToRemove.Count;i++) {
				_listWebForms_SheetDefsSelected.Remove(listWebForms_SheetDefsToRemove[i]);
			}
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			List<WebForms_SheetDef> listWebForms_SheetDefsToAdd=gridWebFormsAvailable.SelectedTags<WebForms_SheetDef>();
			if(listWebForms_SheetDefsToAdd==null || listWebForms_SheetDefsToAdd.Count==0) {
				MsgBox.Show(this,"Please select an item in the grid on the left first.");
				return;
			}
			_listWebForms_SheetDefsSelected.AddRange(listWebForms_SheetDefsToAdd);
			FillGrids();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridWebFormsSelected.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			List<int> listIndicesSelected=new List<int>();
			listIndicesSelected.AddRange(gridWebFormsSelected.SelectedIndices);
			if(listIndicesSelected[0]==0) {//Can't go higher than the first row.
				return;
			}
			for(int i=0;i<listIndicesSelected.Count;i++) {//Swap places with the previous row for every selected sheet def.
				WebForms_SheetDef webForms_SheetDefPrev=_listWebForms_SheetDefsSelected[listIndicesSelected[i]-1];
				_listWebForms_SheetDefsSelected[listIndicesSelected[i]-1]=_listWebForms_SheetDefsSelected[listIndicesSelected[i]];
				_listWebForms_SheetDefsSelected[listIndicesSelected[i]]=webForms_SheetDefPrev;
			}
			FillGrids();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridWebFormsSelected.SetSelected(listIndicesSelected[i]-1,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridWebFormsSelected.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			List<int> listIndicesSelected=new List<int>();
			listIndicesSelected.AddRange(gridWebFormsSelected.SelectedIndices);
			if(listIndicesSelected[listIndicesSelected.Count-1]==gridWebFormsSelected.ListGridRows.Count-1) {//Can't go lower than the last row. -1 to get index number.
				return;
			}
			for(int i=0;i<listIndicesSelected.Count;i++) {//Swap places with the next row for every selected sheet def.
				WebForms_SheetDef webForm_SheetDefPrev=_listWebForms_SheetDefsSelected[listIndicesSelected[i]+1];
				_listWebForms_SheetDefsSelected[listIndicesSelected[i]+1]=_listWebForms_SheetDefsSelected[listIndicesSelected[i]];
				_listWebForms_SheetDefsSelected[listIndicesSelected[i]]=webForm_SheetDefPrev;
			}
			FillGrids();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridWebFormsSelected.SetSelected(listIndicesSelected[i]+1,true);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ListWebForms_SheetDefsSelected=_listWebForms_SheetDefsSelected;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}