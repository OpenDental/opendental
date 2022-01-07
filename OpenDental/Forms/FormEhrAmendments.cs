using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEhrAmendments:FormODBase {
		private List<EhrAmendment> ListAmendments;
		public Patient PatCur;

		public FormEhrAmendments() {
			InitializeComponent();
			InitializeLayoutManager(); 
		}

		private void FormEhrAmendments_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Entry Date",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Status",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Source",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Scanned",25);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			ListAmendments=EhrAmendments.Refresh(PatCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListAmendments.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListAmendments[i].DateTRequest.ToShortDateString());
				if(ListAmendments[i].IsAccepted==YN.Yes) {
					row.Cells.Add("Accepted");
				}
				else if(ListAmendments[i].IsAccepted==YN.No) {
					row.Cells.Add("Denied");
				}
				else {
					row.Cells.Add("Requested");
				}
				row.Cells.Add(ListAmendments[i].Source.ToString());
				row.Cells.Add(ListAmendments[i].Description);
				if(ListAmendments[i].FileName!="") {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EhrAmendment ehrAmd=ListAmendments[e.Row];
			using FormEhrAmendmentEdit FormEAE=new FormEhrAmendmentEdit(ehrAmd);
			FormEAE.ShowDialog();
			FillGrid();//Always have to refresh grid due to using the images module to update the db.
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EhrAmendment ehrAmd=new EhrAmendment();
			ehrAmd.PatNum=PatCur.PatNum;
			ehrAmd.IsNew=true;
			EhrAmendments.Insert(ehrAmd);
			using FormEhrAmendmentEdit FormEAE=new FormEhrAmendmentEdit(ehrAmd);
			FormEAE.ShowDialog();
			FillGrid();//Always have to refresh grid due to using the images module to update the db.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
