using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormWebSchedAppts:FormODBase {

		///<summary>Form showing new appointments made using web sched.</summary>
		public FormWebSchedAppts() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public FormWebSchedAppts(bool isNewPat,bool isRecall,bool isASAP,bool isExistingPat):this() {
			checkWebSchedNewPat.Checked=isNewPat;
			checkWebSchedRecall.Checked=isRecall;
			checkASAP.Checked=isASAP;
			checkWebSchedExistingPat.Checked=isExistingPat;
		}

		///<summary></summary>
		private void FormWebSchedAppts_Load(object sender,System.EventArgs e) {
			//Set the initial date
			datePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			datePicker.SetDateTimeTo(DateTime.Today);
			//Add the appointment confirmation types
			comboConfStatus.Items.Clear();
			long defaultStatus=PrefC.GetLong(PrefName.WebSchedNewPatConfirmStatus);
			if(!checkWebSchedNewPat.Checked && checkWebSchedRecall.Checked) {
				defaultStatus=PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus);
			}
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<listDefs.Count;i++){
				comboConfStatus.Items.Add(listDefs[i].ItemName,listDefs[i].DefNum);
				if((checkWebSchedNewPat.Checked || checkWebSchedRecall.Checked) && listDefs[i].DefNum==defaultStatus) {
					comboConfStatus.SetSelected(i,true);
				}
			}
			FillGrid();
		}

		///<summary></summary>
		private void FillGrid() {
			if(!checkWebSchedNewPat.Checked && !checkWebSchedRecall.Checked && !checkASAP.Checked && !checkWebSchedExistingPat.Checked) {
				gridMain.BeginUpdate();
				gridMain.ListGridRows.Clear();
				gridMain.EndUpdate();
				return;
			}
			DataTable table=GetAppointments();
			gridMain.BeginUpdate();
			//Columns
			gridMain.Columns.Clear();
			GridColumn col;
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Date Time Created"),150,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Appt Date Time"),150,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient Name"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient DOB"),100,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Confirmation Status"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Appt Note"),0);
			gridMain.Columns.Add(col);
			//Rows
			gridMain.ListGridRows.Clear();
			for(int i=0;i<table.Rows.Count;i++) { 
				GridRow row=new GridRow();
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(table.Rows[i]["ClinicDesc"].ToString());
				}
				DateTime dateTimeCreated=PIn.DateT(table.Rows[i]["DateTimeCreated"].ToString());
				DateTime aptDateTime=PIn.DateT(table.Rows[i]["AptDateTime"].ToString());
				DateTime birthdate=PIn.Date(table.Rows[i]["Birthdate"].ToString());
				row.Cells.Add(dateTimeCreated.ToShortDateString()+"  "+dateTimeCreated.ToShortTimeString());
				row.Cells.Add(aptDateTime.ToShortDateString()+"  "+aptDateTime.ToShortTimeString());
				row.Cells.Add(table.Rows[i]["PatName"].ToString());
				row.Cells.Add(birthdate.Year < 1880 ? "" : birthdate.ToShortDateString());
				row.Cells.Add(Defs.GetDef(DefCat.ApptConfirmed,PIn.Long(table.Rows[i]["Confirmed"].ToString())).ItemName);
				row.Cells.Add(table.Rows[i]["Note"].ToString());
				row.Tag=table.Rows[i]["AptNum"].ToString();
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Get the list of websched appointments using the RpAppointments query.</summary>
		private DataTable GetAppointments() {
			List<long> listProvNums=Providers.GetDeepCopy(isShort:true).Select(x => x.ProvNum).ToList();
			List<long> listStatus=comboConfStatus.GetListSelected<long>();
			DataTable table=RpAppointments.GetAppointmentTable(
				datePicker.GetDateTimeFrom(),
				datePicker.GetDateTimeTo(),
				listProvNums,
				comboBoxClinicMulti.ListSelectedClinicNums,
				PrefC.HasClinicsEnabled,
				checkWebSchedRecall.Checked,
				checkWebSchedNewPat.Checked,
				checkASAP.Checked,
				checkWebSchedExistingPat.Checked,
				RpAppointments.SortAndFilterBy.SecDateTEntry,
				new List<ApptStatus>(),
				listStatus,
				nameof(FormWebSchedAppts));
			return table;
		}

		///<summary></summary>
		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		///<summary></summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OpenEditAppointmentWindow(gridMain.ListGridRows[e.Row]);
		}

		private void mainGridMenuItemPatChart_Click(object sender,EventArgs e) {
			List<GridRow> listRowsSelected=gridMain.SelectedGridRows;
			if(listRowsSelected.Count==1) {
				long aptNum=PIn.Long(listRowsSelected[0].Tag.ToString());
				Appointment appointment=Appointments.GetOneApt(aptNum);
				GotoModule.GotoChart(appointment.PatNum);
				DialogResult=DialogResult.OK;
			}
		}

		private void mainGridMenuItemApptEdit_Click(object sender,EventArgs e) {
			List<GridRow> listRowsSelected=gridMain.SelectedGridRows;
			if(listRowsSelected.Count==1) {
				OpenEditAppointmentWindow(listRowsSelected.First());
			}
		}

		private void OpenEditAppointmentWindow(GridRow row) {
			long aptNum=PIn.Long(row.Tag.ToString());
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum);
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}