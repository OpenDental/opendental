using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data;

namespace OpenDental
{
	/// <summary>
	/// Summary description for FormRpApptWithPhones.
	/// </summary>
	public partial class FormRpAppointments : FormODBase {
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private bool _hasClinicsEnabled;

		///<summary></summary>
		public FormRpAppointments()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpApptWithPhones_Load(object sender,System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			listProvs.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(!PrefC.HasClinicsEnabled) {
				labelClinics.Visible=false;
				checkAllClinics.Visible=false;
				listClinics.Visible=false;
				_hasClinicsEnabled=false;
			}
			else {//Clinics enabled.
				_hasClinicsEnabled=true;
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				listClinics.Items.Clear();
				if(!Security.CurUser.ClinicIsRestricted) {
					listClinics.Items.Add(Lan.g(this,"Unassigned"));
					listClinics.SetSelected(0);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClinics.Items.Add(_listClinics[i].Abbr);
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClinics.SelectedIndices.Clear();
						listClinics.SetSelected(listClinics.Items.Count-1);
					}
				}
			}
			SetTomorrow();
		}

		private void SetTomorrow() {
			textDateFrom.Text=DateTime.Today.AddDays(1).ToShortDateString();
			textDateTo.Text=DateTime.Today.AddDays(1).ToShortDateString();
		}

		///<summary>Validates the fields on the form.  Returns false is something is not filled out correctly.</summary>
		private bool IsValid() {
			//validate user input
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(textDateFrom.Text.Length==0
				|| textDateTo.Text.Length==0) 
			{
				MessageBox.Show(Lan.g(this,"From and To dates are required."));
				return false;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo < dateFrom) {
				MessageBox.Show(Lan.g(this,"To date cannot be before From date."));
				return false;
			}
			if(!checkAllProvs.Checked && listProvs.SelectedIndices.Count==0) {
				MessageBox.Show(Lan.g(this,"You must select at least one provider."));
				return false;
			}
			if(_hasClinicsEnabled) {//Not no clinics.
				if(!checkAllClinics.Checked && listClinics.SelectedIndices.Count==0) {
					MsgBox.Show(this,"You must select at least one clinic.");
					return false;
				}
			}
			return true;
		}

		private void checkAllProvs_Click(object sender,EventArgs e) {
			if(checkAllProvs.Checked) {
				listProvs.ClearSelected();
			}
		}

		private void checkAllClinics_Click(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listClinics.SetAll(true);
			}
			else {
				listClinics.ClearSelected();
			}
		}

		private void listProvs_Click(object sender,EventArgs e) {
			if(listProvs.SelectedIndices.Count>0) {
				checkAllProvs.Checked=false;
			}
		}

		private void listClinics_Click(object sender,EventArgs e) {
			if(listClinics.SelectedIndices.Count>0) {
				checkAllClinics.Checked=false;
			}
		}

		private void butToday_Click(object sender, System.EventArgs e) {
			textDateFrom.Text=DateTime.Today.ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
		}

		private void butTomorrow_Click(object sender,System.EventArgs e) {
			SetTomorrow();
		}

		private void checkWebSched_CheckedChanged(object sender,EventArgs e) {
			if(((CheckBox)sender).Checked) {
				radioDateAptCreated.Checked=true;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!IsValid()) {
				return;
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listClinics.SelectedIndices.Count;i++) {
				if(Security.CurUser.ClinicIsRestricted) {
						listClinicNums.Add(_listClinics[listClinics.SelectedIndices[i]].ClinicNum);//we know that the list is a 1:1 to _listClinics
					}
				else {
					if(listClinics.SelectedIndices[i]==0) {
						listClinicNums.Add(0);
					}
					else {
						listClinicNums.Add(_listClinics[listClinics.SelectedIndices[i]-1].ClinicNum);//Minus 1 from the selected index
					}
				}
			}
			if(checkAllClinics.Checked) {//Add all hidden unrestricted clinics to the list
				listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
			}
			List<long> listProvNums=new List<long>();
			if(checkAllProvs.Checked) {
				for(int i = 0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			else {
				for(int i=0;i<listProvs.SelectedIndices.Count;i++) {
					listProvNums.Add(_listProviders[listProvs.SelectedIndices[i]].ProvNum);
				}
			}
			ReportComplex report=new ReportComplex(true,true);
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			DataTable table = new DataTable();
			List<ApptStatus> listStatuses=new List<ApptStatus> { ApptStatus.Planned,ApptStatus.UnschedList };
			if(!checkShowNoteAppts.Checked) {
				listStatuses.Add(ApptStatus.PtNote);
				listStatuses.Add(ApptStatus.PtNoteCompleted);
			}
			RpAppointments.SortAndFilterBy sortBy=radioDateAptCreated.Checked ? RpAppointments.SortAndFilterBy.SecDateTEntry : RpAppointments.SortAndFilterBy.AptDateTime;
			table=RpAppointments.GetAppointmentTable(dateFrom,dateTo,listProvNums,listClinicNums,_hasClinicsEnabled,checkWebSchedRecall.Checked,
				checkWebSchedNewPat.Checked,checkWebSchedASAP.Checked,checkWebSchedExistingPat.Checked,sortBy,listStatuses,new List<long>(),nameof(FormRpAppointments));
			//create the report
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Appointments");
			report.AddTitle("Title",Lan.g(this,"Appointments"),fontTitle);
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",dateFrom.ToShortDateString()+" - "+dateTo.ToShortDateString(),fontSubTitle);
			if(checkAllProvs.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				string str="";
				for(int i=0;i<listProvs.SelectedIndices.Count;i++) {
					if(i>0) {
						str+=", ";
					}
					str+=_listProviders[listProvs.SelectedIndices[i]].Abbr;
				}
				report.AddSubTitle("Providers",str);
			}
			QueryObject query;
			//setup query
			if(!_hasClinicsEnabled) {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			}
			else {
				query=report.AddQuery(table,"","ClinicDesc",SplitByKind.Value,1,true);
			}
			// add columns to report
			if(radioAptDate.Checked) {
				query.AddColumn("Date",80,FieldValueType.Date,font);
				query.GetColumnDetail("Date").SuppressIfDuplicate = true;
				query.GetColumnDetail("Date").StringFormat="d";
			}
			else {
				query.AddColumn("DateCreated",80,FieldValueType.Date,font);
				query.GetColumnDetail("DateCreated").SuppressIfDuplicate = true;
				query.GetColumnDetail("DateCreated").StringFormat="d";
				query.AddColumn("AptDate",80,FieldValueType.Date,font);
				query.GetColumnDetail("AptDate").StringFormat="d";
			}
			query.AddColumn("PatNum",55,FieldValueType.String,font);
			query.AddColumn("Patient",150,FieldValueType.String,font);
			query.AddColumn("Age",45,FieldValueType.Age,font);
			query.AddColumn("Time",65,FieldValueType.Date,font);
			query.GetColumnDetail("Time").StringFormat="t";
			query.GetColumnDetail("Time").ContentAlignment = ContentAlignment.MiddleRight;
			query.GetColumnHeader("Time").ContentAlignment = ContentAlignment.MiddleRight;
			query.AddColumn("Length",45,FieldValueType.Integer,font);
			query.GetColumnHeader("Length").Location=new Point(
				query.GetColumnHeader("Length").Location.X,
				query.GetColumnHeader("Length").Location.Y);
			query.GetColumnHeader("Length").ContentAlignment = ContentAlignment.MiddleCenter;
			query.GetColumnDetail("Length").ContentAlignment = ContentAlignment.MiddleCenter;
			query.GetColumnDetail("Length").Location=new Point(
				query.GetColumnDetail("Length").Location.X,
				query.GetColumnDetail("Length").Location.Y);
			query.AddColumn("Description",170,FieldValueType.String,font);
			query.AddColumn("Home Ph.",120,FieldValueType.String,font);
			query.AddColumn("Work Ph.",120,FieldValueType.String,font);
			query.AddColumn("Cell Ph.",120,FieldValueType.String,font);
			report.AddPageNum(font);
			report.AddGridLines();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}
	}
}
