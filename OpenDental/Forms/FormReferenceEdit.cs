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
		private CustReference RefCur;
		private List<CustRefEntry> RefEntryList;

		public FormReferenceEdit(CustReference refCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			RefCur=refCur;
		}

		private void FormReferenceEdit_Load(object sender,EventArgs e) {
			textName.Text=CustReferences.GetCustNameFL(RefCur.PatNum);
			textNote.Text=RefCur.Note;
			checkBadRef.Checked=RefCur.IsBadRef;
			if(RefCur.DateMostRecent.Year>1880) {
				textRecentDate.Text=RefCur.DateMostRecent.ToShortDateString();
			}
			FillMain();
		}

		private void FillMain() {
			RefEntryList=CustRefEntries.GetEntryListForReference(RefCur.PatNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Last Name",120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("First Name",120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Date Entry",40){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<RefEntryList.Count;i++) {
				row=new GridRow();
				Patient pat=Patients.GetLim(RefEntryList[i].PatNumCust);
				row.Cells.Add(pat.PatNum.ToString());
				row.Cells.Add(pat.LName);
				row.Cells.Add(pat.FName);
				row.Cells.Add(RefEntryList[i].DateEntry.ToShortDateString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormReferenceEntryEdit FormREE=new FormReferenceEntryEdit(RefEntryList[e.Row]);
			FormREE.ShowDialog();
			FillMain();
		}

		private void butToday_Click(object sender,EventArgs e) {
			textRecentDate.Text=DateTime.Now.ToShortDateString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textRecentDate.IsValid()) {
				MsgBox.Show(this,"Please enter a valid date.");
			}
			RefCur.DateMostRecent=PIn.Date(textRecentDate.Text);
			RefCur.IsBadRef=checkBadRef.Checked;
			RefCur.Note=textNote.Text;
			CustReferences.Update(RefCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}