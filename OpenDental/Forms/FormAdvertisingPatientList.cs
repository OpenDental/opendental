using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAdvertisingPatientList:FormODBase {
		private const string COL_SEND="Send";
		public List<PatientInfo> ListPatientInfo=new List<PatientInfo>();
		private HasNecessaryData _hasNecessaryData;

		public FormAdvertisingPatientList(HasNecessaryData hasNecessaryData,params Control[] arrAdditionalFilters) {
			InitializeComponent();
			_hasNecessaryData=hasNecessaryData;
			AddAdditionalFilters(arrAdditionalFilters);
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void AddAdditionalFilters(Control[] arrAdditionalFilters) {
			//Locate and add additional filter controls
			int padding=2;//padding between addtional filter controls
			int maxHeightRow=0;
			int yFloor=panelAdditionalFilters.Height;
			Control filterPrev=null;
			//Tallest to shortest. Layout filters left to right, starting from bottom left corner, wrapping upward to next row.
			arrAdditionalFilters=arrAdditionalFilters.OrderByDescending(x => x.Height).ToArray();
			for(int i=0;i<arrAdditionalFilters.Length;i++) {
				Control filter=arrAdditionalFilters[i];
				int x=(filterPrev?.Location.X??0)+(filterPrev?.Width??0)+padding;
				if(x+filter.Width>panelAdditionalFilters.Width) {
					if(i==0) {
						throw new Exception("Additional filter control is too wide for panel.");
					}
					//This filter would overlap the right hand side of the panel.  Wrap to next row.
					x=padding;
					yFloor-=(maxHeightRow+padding);
					if(yFloor<=0) {
						throw new Exception("Additional filter control will not fit in the panel.");
					}
					maxHeightRow=0;
					filterPrev=null;
				}
				int y=yFloor-filter.Height;
				filter.Location=new Point(x,y);
				filter.Anchor=AnchorStyles.Bottom | AnchorStyles.Left;
				filterPrev=filter;
				maxHeightRow=Math.Max(filter.Height,maxHeightRow);
			}
			panelAdditionalFilters.Controls.AddRange(arrAdditionalFilters);
		}

		private void FormMassPostcardList_Load(object sender,EventArgs e) {
			FillFilters();
			FillGrid(ListPatientInfo);
		}

		private void FillFilters() {
			//patient status list box
			listBoxPatStatus.Items.Clear();
			foreach(PatientStatus status in Enum.GetValues(typeof(PatientStatus))) {
				if(status==PatientStatus.Deceased || status==PatientStatus.Deleted) {
					continue;
				}
				listBoxPatStatus.Items.Add(status.GetDescription(),status);
			}
			listBoxPatStatus.SetSelected(0);
			//preferred contact method list box
			listBoxContactMethod.Items.Clear();
			listBoxContactMethod.Items.Add(Lans.g(this,"Any"),-1);
			listBoxContactMethod.Items.Add(Lans.g(this,ContactMethod.Email.GetDescription()),(int)ContactMethod.Email);
			listBoxContactMethod.Items.Add(Lans.g(this,ContactMethod.None.GetDescription()),(int)ContactMethod.None);
			listBoxContactMethod.SelectedIndex=0;
			//Age Range
			textAgeFrom.Text="1";
			textAgeTo.Text="110";
			//patient billing type list box
			listBoxPatBillingType.Items.Clear();
			listBoxPatBillingType.Items.Add(Lan.g(this,"All"),new Def());
			foreach(Def billingType in Defs.GetDefsForCategory(DefCat.BillingTypes,true)) {
				listBoxPatBillingType.Items.Add(billingType.ItemName,billingType);
			}
			listBoxPatBillingType.SelectedIndex=0;
			//NotSeenSince and SeenSince datePicker and checkBox 
			datePickerNotSeenSince.SetDateTime(DateTime.Now.AddYears(-3));
			checkHideSeenSince.Checked=false;//Should be defaulted to unchecked on load.
			datePickerSeenSince.SetDateTime(DateTime.Now.AddYears(-3));
			checkHideNotSeenSince.Checked=true;
			checkUserQuery.Enabled=Security.IsAuthorized(Permissions.UserQuery,true);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.SelectedIndices.Length!=1 || gridMain.Columns[e.Col].Heading!=COL_SEND) {
				return;
			}
			gridMain.BeginUpdate();
			if(gridMain.ListGridRows[e.Row].Cells[e.Col].Text=="X") {
				gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
			} 
			else {
				gridMain.ListGridRows[e.Row].Cells[e.Col].Text="X";
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
			gridMain.SetSelected(e.Row);
		}

		private void FillGrid(List<PatientInfo> listPatients=null) {
			if(listPatients.IsNullOrEmpty()) {
				ListPatientInfo=RefreshPatients();
			}
			FillGridPatients(ListPatientInfo);
			UpdateSelectedCount();
		}

		private void FillGridPatients(List<PatientInfo> listPatients) {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lans.g(gridMain.TranslationName,COL_SEND),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Name"),140,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Birthdate"),70,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Email"),200,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Address"),170,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Address2"),140,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"City"),60,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"State"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Zip"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Contact Method"),60,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Status"),50,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Last Appointment"),80,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Next Appointment"),80,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			gridMain.Columns.Add(col);
			col=new GridColumn();
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(PatientInfo patient in listPatients) {
				row=new GridRow();
				row.Cells.Add(""); //Check for selected patients
				row.Cells.Add(patient.Name);//Patient Name
				row.Cells.Add(patient.Birthdate.ToShortDateString());//Patient birthdate
				row.Cells.Add(patient.Email);//Patient Email
				row.Cells.Add(patient.Address);//Patient Address
				row.Cells.Add(patient.Address2);//Patient Address2
				row.Cells.Add(patient.City);//Patient City
				row.Cells.Add(patient.State);//Patient State
				row.Cells.Add(patient.Zip);//Patient Zip
				row.Cells.Add(patient.ContactMethod.GetDescription());//Contact method
				row.Cells.Add(patient.Status.GetDescription());//Patient Status
				if(patient.DateTimeLastAppt==DateTime.MinValue) {
					row.Cells.Add("");//Patients Last Appointment (absent)
				}
				else {
					row.Cells.Add(patient.DateTimeLastAppt.ToShortDateString()); //Patients Last Appointment
				}
				if(patient.DateTimeNextAppt==DateTime.MinValue) {
					row.Cells.Add("");//Patients next Appointment (absent)
				}
				else {
					row.Cells.Add(patient.DateTimeNextAppt.ToShortDateString());//Patients next Appointment 
				}
				row.Tag=patient;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void UpdateSelectedCount() {
			labelNumberPats.Text=gridMain.ListGridRows.Count(x=>x.Cells[0].Text.ToUpper()=="X").ToString();
		}

		///<summary>Refreshed the patient data for the available patient's grid only. Uses filters to limit the data. If a patient already exists 
		///in the selected patients grid then they will not additionally be added here again so the user can't add them twice.</summary>
		private List<PatientInfo> RefreshPatients() {
			List<PatientInfo> listPatients=new List<PatientInfo>();
			DataTable tablePatients=GetPatientDataTable();
			if(tablePatients!=null) {
				listPatients=PatientInfo.GetListPatientInfos(tablePatients);
			}
			return listPatients.Where(x=>_hasNecessaryData(x)).ToList();
		}

		private DataTable GetPatientDataTable() {
			DataTable tablePatients=new DataTable();
			List<long> listClinicNums=new List<long>();
			if(checkUserQuery.Checked) { //If we are using a custom query, return a DataTable based on those patnums
				List<long> listPatNums=GetPatNumsFromUserQuery();
				if(listPatNums.IsNullOrEmpty()) {
					return tablePatients;
				}
				List<PatientStatus> listSelectedPatStatuses=new List<PatientStatus>();
				foreach(PatientStatus status in Enum.GetValues(typeof(PatientStatus))) {
					if(status==PatientStatus.Deceased || status==PatientStatus.Deleted) {
						continue;
					}
					listSelectedPatStatuses.Add(status);
				}
				tablePatients=Patients.GetPatientsWithFirstLastAppointments(listSelectedPatStatuses,false,listClinicNums,0,999,DateTime.MinValue,DateTime.MinValue,null,listPatNums:listPatNums);
			}
			else {//If we are using a standard filter, use the generic query
				int ageFrom=PIn.Int(textAgeFrom.Text);
				int ageTo=PIn.Int(textAgeTo.Text);
				int contactMethod=listBoxContactMethod.GetListSelected<int>().First();
				List<PatientStatus> listSelectedPatStatus=listBoxPatStatus.GetListSelected<PatientStatus>();
				if(ageFrom > ageTo) {
					MsgBox.Show(this, "The 'From age' cannot be greater than the 'To age'.");
					return null;
				}
				listClinicNums=GetSelectedActivatedClinicNums();
				tablePatients=Patients.GetPatientsWithFirstLastAppointments(listSelectedPatStatus,checkHiddenFutureAppt.Checked
					,listClinicNums,ageFrom,ageTo,GetSeenSinceDateTime(),GetNotSeenSinceDateTime(),GetPatBillingType(),contactMethod);
			}
			return tablePatients;
		}

		private List<long> GetPatNumsFromUserQuery() {
			List<long> listPatNums=new List<long>();
			string command=textUserQuery.Text;
			if(command.IsNullOrEmpty()) {
				return listPatNums;
			}
			DataTable tableUserQuery=new DataTable();
			if(!UserQueries.ValidateQueryForMassEmail(command)) {
				string err=Lan.g(this,"Query cannot include the following keywords:\n")+string.Join("\n",UserQueries.ListMassEmailBlacklistCmds);
				MessageBox.Show(this,err);
				return listPatNums;
			}
			//Try to execute the user query and show the error if the query was invalid
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					tableUserQuery=Reports.GetTable(command);
				};
				progressOD.StartingMessage="Running Query...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return null;
				}
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error Running Query")+": "+ex.Message);
				return listPatNums;
			}
			//Validate that the resulting DataTable has a PatNum column (this is for SELECT * queries) and contains results
			if(tableUserQuery.Columns["PatNum"]==null) {
				MsgBox.Show("Resulting table did not include a PatNum column.");
				return listPatNums;
			}
			if(tableUserQuery.Rows.Count==0) {
				MsgBox.Show("No results for this query.");
				return listPatNums;
			}
			return tableUserQuery.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
		}

		public DateTime GetNotSeenSinceDateTime() {
			if(!checkHideNotSeenSince.Checked) {
				return DateTime.MinValue;
			}
			return datePickerNotSeenSince.GetDateTime().Date;
		}

		public DateTime GetSeenSinceDateTime() {
			if(!checkHideSeenSince.Checked) {
				return DateTime.MinValue;
			}
			return datePickerSeenSince.GetDateTime().Date;
		}

		public List<Def> GetPatBillingType() {
			//First load of the UI/Form, nothing can be selected, null allows us to ignore it for the query
			//In that case, treat it the same as "All" and return the full list
			if(listBoxPatBillingType.GetListSelected<Def>()==null || listBoxPatBillingType.SelectedIndices.Contains(0)){
				List<Def> listAllBillinTpes=new List<Def>();
				foreach(Def def in Defs.GetDefsForCategory(DefCat.BillingTypes)) {
					listAllBillinTpes.Add(def);
				}
				return listAllBillinTpes;
			}
			return listBoxPatBillingType.GetListSelected<Def>();
		}

		///<summary>List of all activated and enabled clinics selected by the user that are allowed to send mass emails.</summary>
		private List<long> GetSelectedActivatedClinicNums() {
			List<long> listClinicsActivatedEnabled=new List<long>();
			List<long> listClinics=(comboClinicPatient.SelectedClinicNum<0) ? comboClinicPatient.ListClinics.Select(x => x.ClinicNum).ToList() 
				: new List<long> { comboClinicPatient.SelectedClinicNum };
			return listClinics;
		}

		private void butRefreshPatientFilters_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			for(int i = 0;i<gridMain.ListGridRows.Count ;i++) {
				gridMain.ListGridRows[i].Cells[0].Text="X";
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
		}
		
		private void butSetSelected_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			for(int i = 0;i<gridMain.SelectedGridRows.Count;i++) {
				gridMain.SelectedGridRows[i].Cells[0].Text="X";
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
		}

		private void butClearSelected_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			for(int i = 0;i<gridMain.SelectedGridRows.Count;i++) {
				gridMain.SelectedGridRows[i].Cells[0].Text="";
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
		}

		private void butClearAll_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			for(int i = 0;i<gridMain.ListGridRows.Count ;i++) {
				gridMain.ListGridRows[i].Cells[0].Text="";
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
		}

		private void checkUserQuery_Click(object sender,EventArgs e) {
			panelUserQuery.Visible=checkUserQuery.Checked;
			panelFilters.Visible=!checkUserQuery.Checked;
		}

		
		private void butCommitList_Click(object sender,EventArgs e) {
			ListPatientInfo=gridMain.ListGridRows.Where(x=>x.Cells[0].Text=="X").Select(x=>((PatientInfo)x.Tag)).ToList();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		public delegate bool HasNecessaryData(PatientInfo pat);
	}
}