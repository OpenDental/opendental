using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPayorTypes:FormODBase {
		private List<PayorType> ListPayorTypes;
		public Patient PatCur;

		public FormPayorTypes() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayorTypes_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date Start",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Date End",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("SOP Code",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",250);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.ListGridColumns.Add(col);
			ListPayorTypes=PayorTypes.Refresh(PatCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListPayorTypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListPayorTypes[i].DateStart.ToShortDateString());
				if(i==ListPayorTypes.Count-1) {
					row.Cells.Add("Current");
				}
				else {
					row.Cells.Add(ListPayorTypes[i+1].DateStart.ToShortDateString());
				}
				row.Cells.Add(ListPayorTypes[i].SopCode);
				row.Cells.Add(Sops.GetDescriptionFromCode(ListPayorTypes[i].SopCode));
				row.Cells.Add(ListPayorTypes[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PayorType payorType=ListPayorTypes[e.Row];
			using FormPayorTypeEdit FormPTE=new FormPayorTypeEdit(payorType);
			FormPTE.ShowDialog();
			if(FormPTE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			PayorType payorType=new PayorType();
			payorType.PatNum=PatCur.PatNum;
			payorType.DateStart=DateTime.Today;
			using FormPayorTypeEdit FormPTE=new FormPayorTypeEdit(payorType);
			FormPTE.IsNew=true;
			FormPTE.ShowDialog();
			if(FormPTE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
