using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormCarrierCombine : FormODBase {
		///<summary>After this window closes, this will be the carrierNum of the selected carrier.</summary>
		public long PickedCarrierNum;
		///<summary>Before opening this Form, set the carrierNums to show.</summary>
		public List<long> ListCarrierNums;
		private List<Carrier> _listCarriers;

		///<summary></summary>
		public FormCarrierCombine()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);

		}

		private void FormCarrierCombine_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_listCarriers=Carriers.GetCarriers(ListCarrierNums);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn column;
			column=new GridColumn(Lan.g("Table Carriers","Carrier Name"),160);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","Phone"),90);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","Address"),130);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","Address2"),120);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","City"),110);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","ST"),60);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","Zip"),90);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("Table Carriers","ElectID"),60);
			gridMain.ListGridColumns.Add(column);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCarriers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listCarriers[i].CarrierName.ToString());
				row.Cells.Add(_listCarriers[i].Phone.ToString());
				row.Cells.Add(_listCarriers[i].Address.ToString());
				row.Cells.Add(_listCarriers[i].Address2.ToString());
				row.Cells.Add(_listCarriers[i].City.ToString());
				row.Cells.Add(_listCarriers[i].State.ToString());
				row.Cells.Add(_listCarriers[i].Zip.ToString());
				row.Cells.Add(_listCarriers[i].ElectID.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PickedCarrierNum=_listCarriers[e.Row].CarrierNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			PickedCarrierNum=_listCarriers[gridMain.SelectedIndices[0]].CarrierNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















