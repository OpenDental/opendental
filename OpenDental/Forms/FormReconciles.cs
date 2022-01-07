using System;
using System.Drawing;
using System.Collections;
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
		private Reconcile[] RList;
		private long AccountNum;

		///<summary></summary>
		public FormReconciles(long accountNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			AccountNum=accountNum;
		}

		private void FormReconciles_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			RList=Reconciles.GetList(AccountNum);
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableReconciles","Date"),80);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReconciles","Ending Bal"),100,HorizontalAlignment.Right);
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			OpenDental.UI.GridRow row;
			for(int i=0;i<RList.Length;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(RList[i].DateReconcile.ToShortDateString());
				row.Cells.Add(RList[i].EndingBal.ToString("F"));
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void grid_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormReconcileEdit FormR=new FormReconcileEdit(RList[e.Row]);
			FormR.ShowDialog();
			if(FormR.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}

		///<summary></summary>
		private void butAdd_Click(object sender, System.EventArgs e) {
			Reconcile rec=new Reconcile();
			rec.DateReconcile=DateTime.Today;
			rec.AccountNum=AccountNum;
			Reconciles.Insert(rec);
			using FormReconcileEdit FormR=new FormReconcileEdit(rec);
			FormR.IsNew=true;
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		

		

		

		


	}
}





















