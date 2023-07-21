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
	public partial class FormReferenceEdit:FormODBase {
		private CustReference _custReference;
		private List<CustRefEntry> _listCustRefEntries;

		public FormReferenceEdit(CustReference custReference) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_custReference=custReference;
		}

		private void FormReferenceEdit_Load(object sender,EventArgs e) {
			textName.Text=CustReferences.GetCustNameFL(_custReference.PatNum);
			textNote.Text=_custReference.Note;
			checkBadRef.Checked=_custReference.IsBadRef;
			if(_custReference.DateMostRecent.Year>1880) {
				textRecentDate.Text=_custReference.DateMostRecent.ToShortDateString();
			}
			FillMain();
		}

		private void FillMain() {
			_listCustRefEntries=CustRefEntries.GetEntryListForReference(_custReference.PatNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("PatNum",65);
			gridMain.Columns.Add(col);
			col=new GridColumn("Last Name",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("First Name",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("Date Entry",40){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listCustRefEntries.Count;i++) {
				GridRow row=new GridRow();
				Patient patient=Patients.GetLim(_listCustRefEntries[i].PatNumCust);
				row.Cells.Add(patient.PatNum.ToString());
				row.Cells.Add(patient.LName);
				row.Cells.Add(patient.FName);
				row.Cells.Add(_listCustRefEntries[i].DateEntry.ToShortDateString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormReferenceEntryEdit formReferenceEntryEdit=new FormReferenceEntryEdit(_listCustRefEntries[e.Row]);
			formReferenceEntryEdit.ShowDialog();
			FillMain();
		}

		private void butToday_Click(object sender,EventArgs e) {
			textRecentDate.Text=DateTime.Now.ToShortDateString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textRecentDate.IsValid()) {
				MsgBox.Show(this,"Please enter a valid date.");
			}
			_custReference.DateMostRecent=PIn.Date(textRecentDate.Text);
			_custReference.IsBadRef=checkBadRef.Checked;
			_custReference.Note=textNote.Text;
			CustReferences.Update(_custReference);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}