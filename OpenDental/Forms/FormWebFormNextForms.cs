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
		private List<WebForms_SheetDef> _listWebFormsSheetDefsAll;
		///<summary>List of sheet defs selected. This is manipulated with butLeft, butRight, butUp, butDown.</summary>
		private List<WebForms_SheetDef> _listWebFormsSheetDefsSelected;
		///<summary>List of sheet defs selected when exiting the form with OK click.</summary>
		public List<WebForms_SheetDef> ListWebFormsSheetDefsSelected;

		public FormWebFormNextForms(List<WebForms_SheetDef> listWebFormSheetDefsAll,List<WebForms_SheetDef> listWebFormSheetDefsSelected) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listWebFormsSheetDefsAll=listWebFormSheetDefsAll;
			_listWebFormsSheetDefsSelected=listWebFormSheetDefsSelected;
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
			List<WebForms_SheetDef> listWebFormSheetDefsAvailable=_listWebFormsSheetDefsAll.FindAll(x => !_listWebFormsSheetDefsSelected.Contains(x));
			for(int i=0;i < listWebFormSheetDefsAvailable.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listWebFormSheetDefsAvailable[i].Description);
				row.Tag=listWebFormSheetDefsAvailable[i];
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
			for(int i=0;i < _listWebFormsSheetDefsSelected.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listWebFormsSheetDefsSelected[i].Description);
				row.Tag=_listWebFormsSheetDefsSelected[i];
				gridWebFormsSelected.ListGridRows.Add(row);
			}
			gridWebFormsSelected.EndUpdate();
			#endregion
		}

		private void butLeft_Click(object sender,EventArgs e) {
			List<WebForms_SheetDef> listWebFormsSheetDefToRemove=gridWebFormsSelected.SelectedTags<WebForms_SheetDef>();
			if(listWebFormsSheetDefToRemove==null || listWebFormsSheetDefToRemove.Count==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			for(int i=0;i < listWebFormsSheetDefToRemove.Count;i++) {
				_listWebFormsSheetDefsSelected.Remove(listWebFormsSheetDefToRemove[i]);
			}
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			List<WebForms_SheetDef> listWebFormsSheetDefToAdd=gridWebFormsAvailable.SelectedTags<WebForms_SheetDef>();
			if(listWebFormsSheetDefToAdd==null || listWebFormsSheetDefToAdd.Count==0) {
				MsgBox.Show(this,"Please select an item in the grid on the left first.");
				return;
			}
			_listWebFormsSheetDefsSelected.AddRange(listWebFormsSheetDefToAdd);
			FillGrids();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridWebFormsSelected.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			List<int> listSelectedIndices=new List<int>();
			listSelectedIndices.AddRange(gridWebFormsSelected.SelectedIndices);
			if(listSelectedIndices[0]==0) {//Can't go higher than the first row.
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) {//Swap places with the previous row for every selected sheet def.
				WebForms_SheetDef sheetDefPrev=_listWebFormsSheetDefsSelected[listSelectedIndices[i]-1];
				_listWebFormsSheetDefsSelected[listSelectedIndices[i]-1]=_listWebFormsSheetDefsSelected[listSelectedIndices[i]];
				_listWebFormsSheetDefsSelected[listSelectedIndices[i]]=sheetDefPrev;
			}
			FillGrids();
			for(int i=0;i<listSelectedIndices.Count;i++) {
				gridWebFormsSelected.SetSelected(listSelectedIndices[i]-1,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridWebFormsSelected.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the right first.");
				return;
			}
			List<int> listSelectedIndices=new List<int>();
			listSelectedIndices.AddRange(gridWebFormsSelected.SelectedIndices);
			if(listSelectedIndices[listSelectedIndices.Count-1]==gridWebFormsSelected.ListGridRows.Count-1) {//Can't go lower than the last row. -1 to get index number.
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) {//Swap places with the next row for every selected sheet def.
				WebForms_SheetDef sheetDefPrev=_listWebFormsSheetDefsSelected[listSelectedIndices[i]+1];
				_listWebFormsSheetDefsSelected[listSelectedIndices[i]+1]=_listWebFormsSheetDefsSelected[listSelectedIndices[i]];
				_listWebFormsSheetDefsSelected[listSelectedIndices[i]]=sheetDefPrev;
			}
			FillGrids();
			for(int i=0;i<listSelectedIndices.Count;i++) {
				gridWebFormsSelected.SetSelected(listSelectedIndices[i]+1,true);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ListWebFormsSheetDefsSelected=_listWebFormsSheetDefsSelected;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}