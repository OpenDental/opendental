using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReconciles : FormODBase {
		private List<Reconcile> _listReconciles;
		private long _accountNum;

		///<summary></summary>
		public FormReconciles(long accountNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_accountNum=accountNum;
		}

		private void FormReconciles_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_listReconciles=Reconciles.GetList(_accountNum);
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableReconciles","Date"),80);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableReconciles","Ending Bal"),100,HorizontalAlignment.Right);
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			OpenDental.UI.GridRow row;
			for(int i=0;i<_listReconciles.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listReconciles[i].DateReconcile.ToShortDateString());
				row.Cells.Add(_listReconciles[i].EndingBal.ToString("F"));
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void grid_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormReconcileEdit formReconcileEdit=new FormReconcileEdit(_listReconciles[e.Row]);
			formReconcileEdit.ShowDialog();
			if(formReconcileEdit.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}

		///<summary></summary>
		private void butAdd_Click(object sender, System.EventArgs e) {
			Reconcile reconcile=new Reconcile();
			reconcile.DateReconcile=DateTime.Today;
			reconcile.AccountNum=_accountNum;
			Reconciles.Insert(reconcile);
			using FormReconcileEdit formReconcileEdit=new FormReconcileEdit(reconcile);
			formReconcileEdit.IsNew=true;
			formReconcileEdit.ShowDialog();
			if(formReconcileEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
		}

	}
}