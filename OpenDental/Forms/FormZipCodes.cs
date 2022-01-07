using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.UI;

namespace OpenDental{
///<summary></summary>
	public partial class FormZipCodes : FormODBase {
		private bool changed;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FormZipCodes(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormZipCodes_Load(object sender, System.EventArgs e) {
		  FillGrid();
		}

		private void FillGrid(){
			ZipCodes.RefreshCache();
			_listZipCodes=ZipCodes.GetDeepCopy();
			gridZipCode.BeginUpdate();
			gridZipCode.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"ZipCode"),75);
			gridZipCode.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"City"),270);
			gridZipCode.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"State"),50);
			gridZipCode.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Frequent"),80);
			gridZipCode.ListGridColumns.Add(col);
			gridZipCode.ListGridRows.Clear();
			GridRow row;
			foreach(ZipCode zip in _listZipCodes) {
				row=new GridRow();
				row.Cells.Add(zip.ZipCodeDigits);
				row.Cells.Add(zip.City);
				row.Cells.Add(zip.State);
				row.Cells.Add((zip.IsFrequent ? "X" : ""));
				row.Tag=zip;
				gridZipCode.ListGridRows.Add(row);
			}
			gridZipCode.EndUpdate();
		}

		private void gridZipCode_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridZipCode.SelectedIndices.Length==0) {
				return;
			}
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=(ZipCode)gridZipCode.ListGridRows[e.Row].Tag;
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridZipCode.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}	
			ZipCode ZipCur=gridZipCode.SelectedTag<ZipCode>();
			if(MessageBox.Show(Lan.g(this,"Delete Zipcode?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;   
			}
			changed=true;
			ZipCodes.Delete(ZipCur);
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=new ZipCode();
			FormZCE.IsNew=true;
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			FillGrid(); 				
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormZipCodes_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}
	

	}
}
