using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReferenceEntryEdit:FormODBase {
		private CustRefEntry CustRefEntryCur;

		public FormReferenceEntryEdit(CustRefEntry refEntry) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			CustRefEntryCur=refEntry;
		}

		private void FormReferenceEdit_Load(object sender,EventArgs e) {
			textCustomer.Text=CustReferences.GetCustNameFL(CustRefEntryCur.PatNumCust);
			textReferredTo.Text=CustReferences.GetCustNameFL(CustRefEntryCur.PatNumRef);
			if(CustRefEntryCur.DateEntry.Year>1880) {
				textDateEntry.Text=CustRefEntryCur.DateEntry.ToShortDateString();
			}
			textNote.Text=CustRefEntryCur.Note;
		}

		private void butDeleteAll_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete Reference Entry?")) {
				return;
			}
			CustRefEntries.Delete(CustRefEntryCur.CustRefEntryNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			CustRefEntryCur.Note=textNote.Text;
			CustRefEntries.Update(CustRefEntryCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}