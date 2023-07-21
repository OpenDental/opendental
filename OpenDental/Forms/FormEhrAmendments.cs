using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEhrAmendments:FormODBase {
		private List<EhrAmendment> _listEhrAmendments;
		public Patient PatientCur;

		public FormEhrAmendments() {
			InitializeComponent();
			InitializeLayoutManager(); 
		}

		private void FormEhrAmendments_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Entry Date",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			col=new GridColumn("Status",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Source",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",170);
			gridMain.Columns.Add(col);
			col=new GridColumn("Scanned",25);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			_listEhrAmendments=EhrAmendments.Refresh(PatientCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEhrAmendments.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEhrAmendments[i].DateTRequest.ToShortDateString());
				if(_listEhrAmendments[i].IsAccepted==YN.Yes) {
					row.Cells.Add("Accepted");
				}
				else if(_listEhrAmendments[i].IsAccepted==YN.No) {
					row.Cells.Add("Denied");
				}
				else {
					row.Cells.Add("Requested");
				}
				row.Cells.Add(_listEhrAmendments[i].Source.ToString());
				row.Cells.Add(_listEhrAmendments[i].Description);
				if(_listEhrAmendments[i].FileName!="") {
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
			EhrAmendment ehrAmendment=_listEhrAmendments[e.Row];
			using FormEhrAmendmentEdit formEhrAmendmentEdit=new FormEhrAmendmentEdit(ehrAmendment);
			formEhrAmendmentEdit.ShowDialog();
			FillGrid();//Always have to refresh grid due to using the images module to update the db.
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EhrAmendment ehrAmendment=new EhrAmendment();
			ehrAmendment.PatNum=PatientCur.PatNum;
			ehrAmendment.IsNew=true;
			EhrAmendments.Insert(ehrAmendment);
			using FormEhrAmendmentEdit formEhrAmendmentEdit=new FormEhrAmendmentEdit(ehrAmendment);
			formEhrAmendmentEdit.ShowDialog();
			FillGrid();//Always have to refresh grid due to using the images module to update the db.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
