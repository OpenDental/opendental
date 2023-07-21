using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebSchedASAPHistory:FormODBase {
		private List<AsapComms.AsapCommHist> _listAsapCommHists;
		private DateTime _datePrevFrom;
		private DateTime _datePrevTo;
		private List<long> _listClinicNumsPrevSelected;

		public FormWebSchedASAPHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebSchedASAPHistory_Load(object sender,EventArgs e) {
			datePicker.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			datePicker.SetDateTimeTo(DateTime.Today);
			FillGrid();
		}

		private void GetData() {
			Cursor=Cursors.WaitCursor;
			List<long> listClinicNums=null;
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=comboClinic.ListSelectedClinicNums;
			}
			_listAsapCommHists=AsapComms.GetHist(datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo(),listClinicNums:listClinicNums);
			_datePrevFrom=datePicker.GetDateTimeFrom();
			_datePrevTo=datePicker.GetDateTimeTo();
			_listClinicNumsPrevSelected=new List<long>(comboClinic.ListSelectedClinicNums);
			Cursor=Cursors.Default;
		}

		private void FillGrid() {
			if(_listAsapCommHists==null || datePicker.GetDateTimeFrom() < _datePrevFrom || datePicker.GetDateTimeTo() > _datePrevTo
				|| comboClinic.ListSelectedClinicNums.Any(x => !_listClinicNumsPrevSelected.Contains(x))) 
			{
				//The user is asking for data that we have not fetched yet.
				GetData();
			}
			bool isClinicsEnabled=PrefC.HasClinicsEnabled;
			List<AsapComms.AsapCommHist> listAsapCommHists=_listAsapCommHists.Where(x => x.AsapComm.DateTimeEntry
				.Between(datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo()))
				.Where(x => !isClinicsEnabled || comboClinic.ListSelectedClinicNums.Contains(x.AsapComm.ClinicNum)).ToList();
			gridHistory.BeginUpdate();
			gridHistory.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),120);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),120);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"SMS Send Time"),140);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Email Send Time"),140);
			gridHistory.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),120);
				gridHistory.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Original Appt Time"),140);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Slot Start"),140);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Slot Stop"),140);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Entry"),140);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"SMS Message Text"),250);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Email Message Text"),250);
			gridHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),250);
			gridHistory.Columns.Add(col);
			gridHistory.ListGridRows.Clear();
			for(int i=0;i<listAsapCommHists.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listAsapCommHists[i].PatientName);
				row.Cells.Add(Lan.g(this,listAsapCommHists[i].AsapComm.ResponseStatus.GetDescription()));
				string smsSent;
				if(listAsapCommHists[i].AsapComm.SmsSendStatus==AutoCommStatus.SendSuccessful) {
					smsSent=listAsapCommHists[i].AsapComm.DateTimeSmsSent.ToString();
				}
				else if(listAsapCommHists[i].AsapComm.SmsSendStatus==AutoCommStatus.SendNotAttempted) {
					smsSent=listAsapCommHists[i].AsapComm.DateTimeSmsScheduled.ToString();
				}
				else {
					smsSent="";
				}
				row.Cells.Add(smsSent);
				string emailSent;
				if(listAsapCommHists[i].AsapComm.EmailSendStatus==AutoCommStatus.SendSuccessful) {
					emailSent=listAsapCommHists[i].AsapComm.DateTimeEmailSent.ToString();
				}
				else {
					emailSent="";
				}
				row.Cells.Add(emailSent);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(ODMethodsT.Coalesce(Clinics.GetClinic(listAsapCommHists[i].AsapComm.ClinicNum)).Abbr);
				}
				row.Cells.Add(listAsapCommHists[i].AsapComm.DateTimeOrig.Year>1880 ? listAsapCommHists[i].AsapComm.DateTimeOrig.ToString() : "");
				row.Cells.Add(listAsapCommHists[i].DateTimeSlotStart.ToString());
				row.Cells.Add(listAsapCommHists[i].DateTimeSlotEnd.ToString());
				row.Cells.Add(listAsapCommHists[i].AsapComm.DateTimeEntry.ToString());
				row.Cells.Add(listAsapCommHists[i].SMSMessageText);
				row.Cells.Add(listAsapCommHists[i].EmailMessageText);
				row.Cells.Add(listAsapCommHists[i].AsapComm.Note);
				row.Tag=listAsapCommHists[i];
				gridHistory.ListGridRows.Add(row);
			}
			gridHistory.EndUpdate();
			textFilled.Text=listAsapCommHists.Select(x => x.AsapComm)
				.Where(x => x.ResponseStatus==AsapRSVPStatus.AcceptedAndMoved)
				.DistinctBy(x => x.FKey).Count().ToString();
			textTextsSent.Text=listAsapCommHists.Select(x => x.AsapComm)
				.Where(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful)
				.DistinctBy(x => x.GuidMessageToMobile).Count().ToString();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void datePicker_Leave(object sender,EventArgs e) {
			FillGrid();
		}

		private void datePicker_CalendarClosed(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}