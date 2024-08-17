﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormUnschedListPatient:FormODBase {
		private List<Appointment> _listAppointmentsForPatUnsched;
		private Patient _patient;
		///<summary>Holds the selected appointment from the grid. Only one selection is allowed.</summary>
		public Appointment Appointment;

		public FormUnschedListPatient(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patient;
		}

		private void FormPatientUnschedList_Load(object sender,EventArgs e) {
			this.Text=" "+_patient.GetNameLF();
			_listAppointmentsForPatUnsched=Appointments.GetUnschedApptsForPat(_patient.PatNum);
			FillGrid();
		}

		private void FillGrid() {
			this.Cursor=Cursors.WaitCursor;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(gridMain.TranslationName,"Date"),65,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"AptStatus"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"UnschedStatus"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Prov"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Procedures"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Notes"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_listAppointmentsForPatUnsched.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listAppointmentsForPatUnsched[i].AptDateTime.ToShortDateString());
				row.Cells.Add(Lan.g(this,_listAppointmentsForPatUnsched[i].AptStatus.ToString()));
				row.Cells.Add(Lan.g(this,_listAppointmentsForPatUnsched[i].UnschedStatus.ToString()));
				row.Cells.Add(Providers.GetAbbr(_listAppointmentsForPatUnsched[i].ProvNum));
				row.Cells.Add(_listAppointmentsForPatUnsched[i].ProcDescript);
				row.Cells.Add(_listAppointmentsForPatUnsched[i].Note);
				row.Tag=_listAppointmentsForPatUnsched[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			this.Cursor=Cursors.Default;
		}

		///<summary>Sets SelectedAppt to the appointment that is currently selected in the grid.  Shows an error message to the user if no appointment is selected.
		///Otherwise; Sets the appointment and then sets DialogResult to OK.</summary>
		private void SetSelectedAppt() {
			Appointment=gridMain.SelectedTag<Appointment>();
			if(Appointment==null) {//No row was selected.
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