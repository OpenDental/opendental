using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrProviderKeys:FormODBase {
		private List<EhrProvKey> _listKeys;

		public FormEhrProviderKeys() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrProviderKeys_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Last Name",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("First Name",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Year",30);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Key",100);
			gridMain.ListGridColumns.Add(col);
			_listKeys=EhrProvKeys.GetAllKeys();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listKeys.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listKeys[i].LName);
				row.Cells.Add(_listKeys[i].FName);
				row.Cells.Add(_listKeys[i].YearValue.ToString());
				row.Cells.Add(_listKeys[i].ProvKey);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EhrProvKey keycur=_listKeys[e.Row];
			keycur.IsNew=false;
			using FormEhrProviderKeyEdit formE=new FormEhrProviderKeyEdit(keycur);
			formE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EhrProvKey keycur=new EhrProvKey();
			keycur.IsNew=true;
			using FormEhrProviderKeyEdit formE=new FormEhrProviderKeyEdit(keycur);
			formE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	

		
	}
}