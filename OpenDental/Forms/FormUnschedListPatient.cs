using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormUnschedListPatient:FormODBase {
		private List<Appointment> _listUnschedApptsForPat;
		private Patient _patCur;
		///<summary>Holds the selected appointment from the grid. Only one selection is allowed.</summary>
		public Appointment SelectedAppt;

		public FormUnschedListPatient(Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			_patCur=pat;
		}

		private void FormPatientUnschedList_Load(object sender,EventArgs e) {
			this.Text=" "+_patCur.GetNameLF();
			_listUnschedApptsForPat=Appointments.GetUnschedApptsForPat(_patCur.PatNum);
			FillGrid();
		}

		private void FillGrid() {
			this.Cursor=Cursors.WaitCursor;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(gridMain.TranslationName,"Date"),65,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"AptStatus"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"UnschedStatus"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Prov"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Procedures"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Notes"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Appointment apt in _listUnschedApptsForPat) {
				row=new GridRow();
				row.Cells.Add(apt.AptDateTime.ToShortDateString());
				row.Cells.Add(Lan.g(this,apt.AptStatus.ToString()));
				row.Cells.Add(Lan.g(this,apt.UnschedStatus.ToString()));
				row.Cells.Add(Providers.GetAbbr(apt.ProvNum));
				row.Cells.Add(apt.ProcDescript);
				row.Cells.Add(apt.Note);
				row.Tag=apt;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			this.Cursor=Cursors.Default;
		}

		///<summary>Sets SelectedAppt to the appointment that is currently selected in the grid.  Shows an error message to the user if no appointment is selected.
		///Otherwise; Sets the appointment and then sets DialogResult to OK.</summary>
		private void SetSelectedAppt() {
			SelectedAppt=gridMain.SelectedTag<Appointment>();
			if(SelectedAppt==null) {//No row was selected.
				MsgBox.Show(this,Lan.g(this,"Please select an unscheduled appointment to use."));
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SetSelectedAppt();
		}

		private void butOK_Click(object sender,EventArgs e) {
			SetSelectedAppt();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
