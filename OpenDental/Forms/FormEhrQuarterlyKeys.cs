using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrQuarterlyKeys:FormODBase {
		private List<EhrQuarterlyKey> listKeys;

		public FormEhrQuarterlyKeys() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrQuarterlyKeys_Load(object sender,EventArgs e) {
			textPracticeTitle.Text=PrefC.GetString(PrefName.PracticeTitle);
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Year",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Quarter",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Key",100);
			gridMain.ListGridColumns.Add(col);
			listKeys=EhrQuarterlyKeys.Refresh(0);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listKeys.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listKeys[i].YearValue.ToString());
				row.Cells.Add(listKeys[i].QuarterValue.ToString());
				row.Cells.Add(listKeys[i].KeyValue);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrQuarterlyKeyEdit formE=new FormEhrQuarterlyKeyEdit();
			EhrQuarterlyKey keycur=listKeys[e.Row];
			keycur.IsNew=false;
			formE.KeyCur=keycur;
			formE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEhrQuarterlyKeyEdit formE=new FormEhrQuarterlyKeyEdit();
			EhrQuarterlyKey keycur=new EhrQuarterlyKey();
			keycur.IsNew=true;
			formE.KeyCur=keycur;
			formE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	

		
	}
}