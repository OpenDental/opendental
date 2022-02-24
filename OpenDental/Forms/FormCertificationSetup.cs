using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCertificationSetup:FormODBase {

		private List<Cert> _listCertsFiltered;

		public FormCertificationSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCertificationSetup_Load(object sender,EventArgs e) {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.CertificationCategories,true);
			listBoxCategories.Items.AddList(listDefs,x => x.ItemName);
			FillGrid();
		}

		private void FillGrid(){
			if(listBoxCategories.SelectedIndex==-1) {
				return;
			}
			_listCertsFiltered=Certs.GetAllForCategory(listBoxCategories.GetSelected<Def>().DefNum);
			//Columns: Certification, Wikipage	
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn();
			col=new GridColumn(Lan.g("FormCertificationSetup","Certification"),260);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertificationSetup","WikiPage"),260);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertificationSetup","Hidden"),66,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listCertsFiltered.Count;i++) {
				if(_listCertsFiltered[i].ItemOrder!=i) {
					_listCertsFiltered[i].ItemOrder=i;
					Certs.Update(_listCertsFiltered[i]);
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listCertsFiltered[i].Description);
				row.Cells.Add(_listCertsFiltered[i].WikiPageLink);
				row.Cells.Add(_listCertsFiltered[i].IsHidden ? "X":"");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void listBoxCategories_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			if(idx==0){
				return;
			}
			_listCertsFiltered[idx].ItemOrder=idx-1;
			Certs.Update(_listCertsFiltered[idx]);
			_listCertsFiltered[idx-1].ItemOrder=idx;//oldItemOrder;
			Certs.Update(_listCertsFiltered[idx-1]);
			FillGrid();
			gridMain.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			if(idx==_listCertsFiltered.Count-1) {
				return;
			}
			_listCertsFiltered[idx].ItemOrder=idx+1;
			Certs.Update(_listCertsFiltered[idx]);
			_listCertsFiltered[idx+1].ItemOrder=idx;//oldItemOrder;
			Certs.Update(_listCertsFiltered[idx+1]);
			FillGrid();
			gridMain.SetSelected(idx+1,true);
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(listBoxCategories.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			using FormCertificationEdit formCertificationEdit=new FormCertificationEdit();
			formCertificationEdit.CertCur=new Cert();
			formCertificationEdit.CertCur.IsNew=true;
			formCertificationEdit.CertCur.CertCategoryNum=listBoxCategories.GetSelected<Def>().DefNum;
			formCertificationEdit.CertCur.ItemOrder=_listCertsFiltered.Count;
			formCertificationEdit.ShowDialog();
			if(formCertificationEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormCertificationEdit formCertificationEdit=new FormCertificationEdit();
			formCertificationEdit.CertCur=Certs.GetOne(_listCertsFiltered[e.Row].CertNum);
			formCertificationEdit.ShowDialog();
			if(formCertificationEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//Category may have changed, we will fix the item orders in the FillGrid
			FillGrid();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}