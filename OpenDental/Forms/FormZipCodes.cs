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
		private bool _isChanged;
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
			gridZipCode.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"ZipCode"),75);
			gridZipCode.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"City"),270);
			gridZipCode.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"State"),50);
			gridZipCode.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Frequent"),80);
			gridZipCode.Columns.Add(col);
			gridZipCode.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listZipCodes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listZipCodes[i].ZipCodeDigits);
				row.Cells.Add(_listZipCodes[i].City);
				row.Cells.Add(_listZipCodes[i].State);
				row.Cells.Add((_listZipCodes[i].IsFrequent ? "X" : ""));
				row.Tag=_listZipCodes[i];
				gridZipCode.ListGridRows.Add(row);	
			}
			gridZipCode.EndUpdate();
		}

		private void gridZipCode_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridZipCode.SelectedIndices.Length==0) {
				return;
			}
			using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
			formZipCodeEdit.ZipCodeCur=(ZipCode)gridZipCode.ListGridRows[e.Row].Tag;
			formZipCodeEdit.ShowDialog();
			if(formZipCodeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridZipCode.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}	
			ZipCode zipCode=gridZipCode.SelectedTag<ZipCode>();
			if(MessageBox.Show(Lan.g(this,"Delete Zipcode?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;   
			}
			_isChanged=true;
			ZipCodes.Delete(zipCode);
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
			formZipCodeEdit.ZipCodeCur=new ZipCode();
			formZipCodeEdit.IsNew=true;
			formZipCodeEdit.ShowDialog();
			if(formZipCodeEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_isChanged=true;
			FillGrid(); 				
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormZipCodes_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}
	

	}
}
