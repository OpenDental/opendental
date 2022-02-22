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
		private const string COLUMN_SEND="Send";
		public List<PatientInfo> ListPatientInfos=new List<PatientInfo>();
		private PatientHasNecessaryData _patientHasNecessaryData;

		public FormAdvertisingPatientList(PatientHasNecessaryData hasNecessaryData,params Control[] controlArrayAdditionalFilters) {
			InitializeComponent();
			_patientHasNecessaryData=hasNecessaryData;
			AddAdditionalFilters(controlArrayAdditionalFilters);
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void AddAdditionalFilters(Control[] controlArrayAdditionalFilters) {
			//Locate and add additional filter controls
			int paddingBetweenControls=2;//padding between addtional filter controls
			int controlRowHeight=0;
			int controlRowFloorYCoordinate=panelAdditionalFilters.Height;
			Control controlFilterPrevious=null;
			//Tallest to shortest. Layout filters left to right, starting from bottom left corner, wrapping upward to next row.
			controlArrayAdditionalFilters=controlArrayAdditionalFilters.OrderByDescending(x => x.Height).ToArray();
			for(int i=0;i<controlArrayAdditionalFilters.Length;i++) {
				Control controlFilter=controlArrayAdditionalFilters[i];
				int nextControlFilterXCoordinate=(controlFilterPrevious?.Location.X??0)+(controlFilterPrevious?.Width??0)+paddingBetweenControls;
				if(nextControlFilterXCoordinate+controlFilter.Width>panelAdditionalFilters.Width) {
					if(i==0) {
						throw new Exception("Additional filter control is too wide for panel.");
					}
					//This filter would overlap the right hand side of the panel.  Wrap to next row.
					nextControlFilterXCoordinate=paddingBetweenControls;
					controlRowFloorYCoordinate-=(controlRowHeight+paddingBetweenControls);
					if(controlRowFloorYCoordinate<=0) {
						throw new Exception("Additional filter control will not fit in the panel.");
					}
					controlRowHeight=0;
					controlFilterPrevious=null;
				}
				int nextControlFilterYCoordinate=controlRowFloorYCoordinate-controlFilter.Height;
				controlFilter.Location=new Point(nextControlFilterXCoordinate,nextControlFilterYCoordinate);
				controlFilter.Anchor=AnchorStyles.Bottom | AnchorStyles.Left;
				controlFilterPrevious=controlFilter;
				controlRowHeight=Math.Max(controlFilter.Height,controlRowHeight);
			}
			panelAdditionalFilters.Controls.AddRange(controlArrayAdditionalFilters);
		}

		private void FormMassPostcardList_Load(object sender,EventArgs e) {
			FillFilters();
			FillGrid(ListPatientInfos);
		}

		private void FillFilters() {
			//patient status list box
			listBoxPatientStatuses.Items.Clear();
			foreach(PatientStatus patientStatus in Enum.GetValues(typeof(PatientStatus))) {
				if(patientStatus==PatientStatus.Deceased || patientStatus==PatientStatus.Deleted) {
					continue;
				}
				listBoxPatientStatuses.Items.Add(patientStatus.GetDescription(),patientStatus);
			}
			listBoxPatientStatuses.SetSelected(0);
			//preferred contact method list box
			listBoxContactMethods.Items.Clear();
			listBoxContactMethods.Items.Add(Lans.g(this,"Any"),-1);
			listBoxContactMethods.Items.Add(Lans.g(this,ContactMethod.Email.GetDescription()),(int)ContactMethod.Email);
			listBoxContactMethods.Items.Add(Lans.g(this,ContactMethod.None.GetDescription()),(int)ContactMethod.None);
			listBoxContactMethods.SelectedIndex=0;
			//Age Range
			textPatientAgeFrom.Text="1";
			textPatientAgeTo.Text="110";
			//patient billing type list box
			listBoxPatientBillingType.Items.Clear();
			listBoxPatientBillingType.Items.Add(Lan.g(this,"All"),new Def());
			foreach(Def billingType in Defs.GetDefsForCategory(DefCat.BillingTypes,true)) {
				listBoxPatientBillingType.Items.Add(billingType.ItemName,billingType);
			}
			listBoxPatientBillingType.SelectedIndex=0;
			//NotSeenSince and SeenSince datePicker and checkBox 
			datePickerPatientsNotSeenSince.SetDateTime(DateTime.Now.AddYears(-3));
			checkHidePatientsSeenSince.Checked=false;//Should be defaulted to unchecked on load.
			datePickerPatientsSeenSince.SetDateTime(DateTime.Now.AddYears(-3));
			checkHidePatientsNotSeenSince.Checked=true;
			checkUserQuery.Enabled=Security.IsAuthorized(Permissions.UserQuery,true);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.SelectedIndices.Length!=1 || gridMain.Columns[e.Col].Heading!=Lans.g(gridMain.TranslationName,COLUMN_SEND)) {
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
				ListPatientInfos=RefreshPatients();
			}
			FillGridPatients(ListPatientInfos);
			UpdateSelectedCount();
		}

		private void FillGridPatients(List<PatientInfo> listPatientInfos) {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,COLUMN_SEND),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Name"),140,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Birthdate"),70,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Email"),200,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Address"),170,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Address2"),140,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"City"),60,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"State"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Zip"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Contact Method"),60,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Status"),50,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Last Appointment"),80,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridMain.TranslationName,"Next Appointment"),80,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn();
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			foreach(PatientInfo patientInfo in listPatientInfos) {
				gridRow=new GridRow();
				gridRow.Cells.Add(""); //Check for selected patients
				gridRow.Cells.Add(patientInfo.Name);//Patient Name
				gridRow.Cells.Add(patientInfo.Birthdate.ToShortDateString());//Patient birthdate
				gridRow.Cells.Add(patientInfo.Email);//Patient Email
				gridRow.Cells.Add(patientInfo.Address);//Patient Address
				gridRow.Cells.Add(patientInfo.Address2);//Patient Address2
				gridRow.Cells.Add(patientInfo.City);//Patient City
				gridRow.Cells.Add(patientInfo.State);//Patient State
				gridRow.Cells.Add(patientInfo.Zip);//Patient Zip
				gridRow.Cells.Add(patientInfo.ContactMethod.GetDescription());//Contact method
				gridRow.Cells.Add(patientInfo.Status.GetDescription());//Patient Status
				if(patientInfo.DateTimeLastAppt==DateTime.MinValue) {
					gridRow.Cells.Add("");//Patients Last Appointment (absent)
				}
				else {
					gridRow.Cells.Add(patientInfo.DateTimeLastAppt.ToShortDateString()); //Patients Last Appointment
				}
				if(patientInfo.DateTimeNextAppt==DateTime.MinValue) {
					gridRow.Cells.Add("");//Patients next Appointment (absent)
				}
				else {
					gridRow.Cells.Add(patientInfo.DateTimeNextAppt.ToShortDateString());//Patients next Appointment 
				}
				gridRow.Tag=patientInfo;
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void UpdateSelectedCount() {
			labelNumberPatientsSelected.Text=gridMain.ListGridRows.Count(x=>x.Cells[0].Text.ToUpper()=="X").ToString();
		}

		///<summary>Refreshed the patient data for the available patient's grid only. Uses filters to limit the data. If a patient already exists 
		///in the selected patients grid then they will not additionally be added here again so the user can't add them twice.</summary>
		private List<PatientInfo> RefreshPatients() {
			List<PatientInfo> listPatientInfos=new List<PatientInfo>();
			DataTable tablePatientInfoRaw=GetPatientDataTable();
			if(tablePatientInfoRaw!=null) {
				listPatientInfos=PatientInfo.GetListPatientInfos(tablePatientInfoRaw);
			}
			return listPatientInfos.Where(x=>_patientHasNecessaryData(x)).ToList();
		}

		private DataTable GetPatientDataTable() {
			DataTable tablePatientInfoRaw=new DataTable();
			List<long> listClinicNums=new List<long>();
			if(checkUserQuery.Checked) { //If we are using a custom query, return a DataTable based on those patnums
				List<long> listPatNums=GetPatNumsFromUserQuery();
				if(listPatNums.IsNullOrEmpty()) {
					return tablePatientInfoRaw;
				}
				List<PatientStatus> listPatientStatuses=new List<PatientStatus>();
				foreach(PatientStatus patientStatus in Enum.GetValues(typeof(PatientStatus))) {
					if(patientStatus==PatientStatus.Deceased || patientStatus==PatientStatus.Deleted) {
						continue;
					}
					listPatientStatuses.Add(patientStatus);
				}
				tablePatientInfoRaw=Patients.GetPatientsWithFirstLastAppointments(listPatientStatuses,false,listClinicNums,0,999,DateTime.MinValue,DateTime.MinValue,null,listPatNums:listPatNums);
			}
			else {//If we are using a standard filter, use the generic query
				int patientAgeLowerBound=PIn.Int(textPatientAgeFrom.Text);
				int patientAgeUpperBound=PIn.Int(textPatientAgeTo.Text);
				int contactMethod=listBoxContactMethods.GetListSelected<int>().First();
				List<PatientStatus> listPatientStatusesSelected=listBoxPatientStatuses.GetListSelected<PatientStatus>();
				if(patientAgeLowerBound > patientAgeUpperBound) {
					MsgBox.Show(this, "The 'From age' cannot be greater than the 'To age'.");
					return null;
				}
				listClinicNums=GetSelectedActivatedClinicNums();
				tablePatientInfoRaw=Patients.GetPatientsWithFirstLastAppointments(listPatientStatusesSelected,checkHiddenFutureAppt.Checked
					,listClinicNums,patientAgeLowerBound,patientAgeUpperBound,GetSeenSinceDateTime(),GetNotSeenSinceDateTime(),GetPatBillingType(),contactMethod);
			}
			return tablePatientInfoRaw;
		}

		private List<long> GetPatNumsFromUserQuery() {
			List<long> listPatNums=new List<long>();
			string stringQueryCommand=textUserQuery.Text;
			if(stringQueryCommand.IsNullOrEmpty()) {
				return listPatNums;
			}
			DataTable tableReportQueryResults=new DataTable();
			if(!UserQueries.ValidateQueryForMassEmail(stringQueryCommand)) {
				string stringError=Lan.g(this,"Query cannot include the following keywords:\n")+string.Join("\n",UserQueries.ListMassEmailBlacklistCmds);
				MessageBox.Show(this,stringError);
				return listPatNums;
			}
			//Try to execute the user query and show the error if the query was invalid
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					tableReportQueryResults=Reports.GetTable(stringQueryCommand);
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
			if(tableReportQueryResults.Columns["PatNum"]==null) {
				MsgBox.Show("Resulting table did not include a PatNum column.");
				return listPatNums;
			}
			if(tableReportQueryResults.Rows.Count==0) {
				MsgBox.Show("No results for this query.");
				return listPatNums;
			}
			return tableReportQueryResults.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
		}

		public DateTime GetNotSeenSinceDateTime() {
			if(!checkHidePatientsNotSeenSince.Checked) {
				return DateTime.MinValue;
			}
			return datePickerPatientsNotSeenSince.GetDateTime().Date;
		}

		public DateTime GetSeenSinceDateTime() {
			if(!checkHidePatientsSeenSince.Checked) {
				return DateTime.MinValue;
			}
			return datePickerPatientsSeenSince.GetDateTime().Date;
		}

		public List<Def> GetPatBillingType() {
			//First load of the UI/Form, nothing can be selected, null allows us to ignore it for the query
			//In that case, treat it the same as "All" and return the full list
			if(listBoxPatientBillingType.GetListSelected<Def>()==null || listBoxPatientBillingType.SelectedIndices.Contains(0)){
				List<Def> listBillingTypesAll=new List<Def>();
				foreach(Def def in Defs.GetDefsForCategory(DefCat.BillingTypes)) {
					listBillingTypesAll.Add(def);
				}
				return listBillingTypesAll;
			}
			return listBoxPatientBillingType.GetListSelected<Def>();
		}

		///<summary>List of all activated and enabled clinics selected by the user that are allowed to send mass emails.</summary>
		private List<long> GetSelectedActivatedClinicNums() {
			List<long> listClinicNums=(comboClinics.SelectedClinicNum<0) ? comboClinics.ListClinics.Select(x => x.ClinicNum).ToList() 
				: new List<long> { comboClinics.SelectedClinicNum };
			return listClinicNums;
		}

		private void butFavorite_Click(object sender,EventArgs e) {
			using FormQueryFavorites formQueryFavorites = new FormQueryFavorites();
			formQueryFavorites.IsQueryAllowed = (query) => UserQueries.ValidateQueryForMassEmail(query.QueryText);
			if(formQueryFavorites.ShowDialog() == DialogResult.OK) {
				textUserQuery.Text=formQueryFavorites.UserQueryCur.QueryText;
			}
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
			panelFilterControls.Visible=!checkUserQuery.Checked;
		}

		
		private void butCommitList_Click(object sender,EventArgs e) {
			ListPatientInfos=gridMain.ListGridRows.Where(x=>x.Cells[0].Text=="X").Select(x=>((PatientInfo)x.Tag)).ToList();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		public delegate bool PatientHasNecessaryData(PatientInfo pat);
	}
}