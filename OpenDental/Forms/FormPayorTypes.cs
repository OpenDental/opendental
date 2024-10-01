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
		private List<PayorType> _listPayorTypes;
		public Patient PatientCur;

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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Date Start",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			col=new GridColumn("Date End",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			col=new GridColumn("SOP Code",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",250);
			gridMain.Columns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.Columns.Add(col);
			_listPayorTypes=PayorTypes.GetPatientData(PatientCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPayorTypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listPayorTypes[i].DateStart.ToShortDateString());
				if(i==_listPayorTypes.Count-1) {
					row.Cells.Add("Current");
				}
				else {
					row.Cells.Add(_listPayorTypes[i+1].DateStart.ToShortDateString());
				}
				row.Cells.Add(_listPayorTypes[i].SopCode);
				row.Cells.Add(Sops.GetDescriptionFromCode(_listPayorTypes[i].SopCode));
				row.Cells.Add(_listPayorTypes[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PayorType payorType=_listPayorTypes[e.Row];
			using FormPayorTypeEdit formPayorTypeEdit=new FormPayorTypeEdit(payorType);
			formPayorTypeEdit.ShowDialog();
			if(formPayorTypeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			PayorType payorType=new PayorType();
			payorType.PatNum=PatientCur.PatNum;
			payorType.DateStart=DateTime.Today;
			using FormPayorTypeEdit formPayorTypeEdit=new FormPayorTypeEdit(payorType);
			formPayorTypeEdit.IsNew=true;
			formPayorTypeEdit.ShowDialog();
			if(formPayorTypeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

	}
}