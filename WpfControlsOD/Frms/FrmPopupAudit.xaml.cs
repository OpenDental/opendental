using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {

	public partial class FrmPopupAudit:FrmODBase {
		public Popup PopupCur;
		public Patient PatientCur;
		///<summary>All archived popups for the current popup</summary>
		private List<Popup> ListPopups;

		public FrmPopupAudit() {
			InitializeComponent();
			Load+=FrmPopupAudit_Load;
			gridPopupAudit.CellDoubleClick+=gridPopupAudit_CellDoubleClick;
			//Lan.F(this);
		}

		private void FrmPopupAudit_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {//The grid cannot be changed to be sortable. This audit relies on the oldest popup changes being located at the top
			ListPopups=Popups.GetArchivesForPopup(PopupCur.PopupNum);
			gridPopupAudit.BeginUpdate();
			gridPopupAudit.Columns.Clear();
			GridColumn col=new GridColumn(Lans.g("TablePopupsForFamily","Create Date"),140);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lans.g("TablePopupsForFamily","Edit Date"),140);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lans.g("TablePopupsForFamily","Level"),80);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lans.g("TablePopupsForFamily","Disabled"),60,HorizontalAlignment.Center);
			gridPopupAudit.Columns.Add(col);
			col=new GridColumn(Lans.g("TablePopupsForFamily","Popup Message"),100);
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
				row.Cells.Add(Lans.g("enumEnumPopupLevel",ListPopups[i].PopupLevel.ToString()));
				row.Cells.Add((ListPopups[i].DateTimeDisabled!=DateTime.MinValue && ListPopups[i].DateTimeDisabled < DateTime.Now)?"X":"");
				row.Cells.Add(ListPopups[i].Description);
				gridPopupAudit.ListGridRows.Add(row);
			}
			gridPopupAudit.EndUpdate();
		}

		private void gridPopupAudit_CellDoubleClick(object sender,GridClickEventArgs e) {
			FrmPopupEdit frmPopupEdit=new FrmPopupEdit();
			frmPopupEdit.PopupAudit=PopupCur;
			DateTime dateLastEdit=DateTime.MinValue;
			if(e.Row > 0 && ListPopups[e.Row-1].DateTimeEntry.Year > 1880) {
				frmPopupEdit.DateLastEdit=ListPopups[e.Row-1].DateTimeEntry;
			}
			frmPopupEdit.PopupCur=ListPopups[e.Row];
			frmPopupEdit.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			IsDialogOK=false;
		}
	}
}