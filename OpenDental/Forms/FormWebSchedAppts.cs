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
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Date Time Created"),150,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Appt Date Time"),150,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient Name"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient DOB"),100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Confirmation Status"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Appt Note"),0);
			gridMain.ListGridColumns.Add(col);
			//Rows
			gridMain.ListGridRows.Clear();
			foreach(DataRow row in table.Rows) {
				GridRow newRow=new GridRow();
				if(PrefC.HasClinicsEnabled) {
					newRow.Cells.Add(row["ClinicDesc"].ToString());
				}
				DateTime dateTimeCreated=PIn.DateT(row["DateTimeCreated"].ToString());
				DateTime aptDateTime=PIn.DateT(row["AptDateTime"].ToString());
				DateTime birthdate=PIn.Date(row["Birthdate"].ToString());
				newRow.Cells.Add(dateTimeCreated.ToShortDateString()+"  "+dateTimeCreated.ToShortTimeString());
				newRow.Cells.Add(aptDateTime.ToShortDateString()+"  "+aptDateTime.ToShortTimeString());
				newRow.Cells.Add(row["PatName"].ToString());
				newRow.Cells.Add(birthdate.Year < 1880 ? "" : birthdate.ToShortDateString());
				newRow.Cells.Add(Defs.GetDef(DefCat.ApptConfirmed,PIn.Long(row["Confirmed"].ToString())).ItemName);
				newRow.Cells.Add(row["Note"].ToString());
				newRow.Tag=row["AptNum"].ToString();
				gridMain.ListGridRows.Add(newRow);
			}
			gridMain.EndUpdate();
		}

		///<summary>Get the list of websched appointments using the RpAppointments query.</summary>
		private DataTable GetAppointments() {
			List<long> listProvNums=Providers.GetProvidersForWebSchedNewPatAppt().Select(x => x.ProvNum).ToList();
			List<long> listStatus=comboConfStatus.GetListSelected<long>();
			return RpAppointments.GetAppointmentTable(
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
			List<GridRow> listSelected=gridMain.SelectedGridRows;
			if(listSelected.Count==1) {
				long aptNum=PIn.Long(listSelected[0].Tag.ToString());
				Appointment apt=Appointments.GetOneApt(aptNum);
				GotoModule.GotoChart(apt.PatNum);
				DialogResult=DialogResult.OK;
			}
		}

		private void mainGridMenuItemApptEdit_Click(object sender,EventArgs e) {
			List<GridRow> listSelected=gridMain.SelectedGridRows;
			if(listSelected.Count==1) {
				OpenEditAppointmentWindow(listSelected.First());
			}
		}

		private void OpenEditAppointmentWindow(GridRow row) {
			long aptNum=PIn.Long(row.Tag.ToString());
			using FormApptEdit formAE=new FormApptEdit(aptNum);
			formAE.ShowDialog();
			if(formAE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}