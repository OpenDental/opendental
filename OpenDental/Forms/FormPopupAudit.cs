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
	public partial class FormPopupAudit:FormODBase {
		public Popup PopupCur;
		public Patient PatientCur;
		///<summary>All archived popups for the current popup</summary>
		private List<Popup> ListPopups;

		public FormPopupAudit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPopupAudit_Load(object sender,System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {//The grid cannot be changed to be sortable. This audit relies on the oldest popup changes being located at the top
			ListPopups=Popups.GetArchivesForPopup(PopupCur.PopupNum);
			gridPopupAudit.BeginUpdate();
			gridPopupAudit.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePopupsForFamily","Create Date"),140);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Edit Date"),140);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Level"),80);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Disabled"),60,HorizontalAlignment.Center);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Popup Message"),100);
			gridPopupAudit.Columns.Add(col);
			gridPopupAudit.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListPopups.Count;i++) {
				row=new GridRow();
				if(PopupCur.DateTimeEntry.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(PopupCur.DateTimeEntry.ToString());
				}
				if(i==0) {
					row.Cells.Add("");//Very first pop up entry.  No edit date.
				}
				else {
					row.Cells.Add(ListPopups[i-1].DateTimeEntry.ToString());//Gets the previous DateTimeEntry to show as the last edit date.
				}
				row.Cells.Add(Lan.g("enumEnumPopupLevel",ListPopups[i].PopupLevel.ToString()));
				row.Cells.Add((ListPopups[i].DateTimeDisabled!=DateTime.MinValue && ListPopups[i].DateTimeDisabled < DateTime.Now)?"X":"");
				row.Cells.Add(ListPopups[i].Description);
				gridPopupAudit.ListGridRows.Add(row);
			}
			gridPopupAudit.EndUpdate();
		}

		private void gridPopupAudit_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPopupEdit FormPopupEdit=new FormPopupEdit();
			FormPopupEdit.PopupAudit=PopupCur;
			DateTime dateLastEdit=DateTime.MinValue;
			if(e.Row > 0 && ListPopups[e.Row-1].DateTimeEntry.Year > 1880) {
				FormPopupEdit.DateLastEdit=ListPopups[e.Row-1].DateTimeEntry;
			}
			FormPopupEdit.PopupCur=ListPopups[e.Row];
			FormPopupEdit.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}