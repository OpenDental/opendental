using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailList:FormODBase {
		private bool _isLoading;

		public FormMassEmailList() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmail_Load(object sender,EventArgs e) {
			_isLoading=true;
			FillFilters();
			FillGrid();
			FillComboEmailHostingTemplate();
			UpdateClinicIsNotEnabled();
			_isLoading=false;
		}

		private void FillGrid() {
			List<PatientInfo> listPatients=RefreshPatients();
			labelRefreshNeeded.Visible=false;
			bool isMassEmailEnabled=IsMassEmailEnabled();
			butSendEmails.Enabled=isMassEmailEnabled;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lans.g(gridMain.TranslationName,"Send"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Name"),140,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Birthdate"),75,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Email"),110,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Contact Method"),60,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Status"),50,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Last Appointment"),75,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(gridMain.TranslationName,"Next Appointment"),75,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse);
			col.IsWidthDynamic=true;
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(PatientInfo patient in listPatients) {
				row=new GridRow();
				row.Cells.Add(""); //0-Check for selected patients
				row.Cells.Add(patient.Name);//1-Patient Name
				row.Cells.Add(patient.Birthdate.ToShortDateString());//2-Patient birthdate
				row.Cells.Add(patient.Email);//3-Patient email
				row.Cells.Add(patient.ContactMethod.GetDescription());//4-Contact method
				row.Cells.Add(patient.Status.GetDescription());//5-Patient Status
				if(patient.DateTimeLastAppt==DateTime.MinValue) {
					row.Cells.Add("");//6-Patients Last Appointment (absent)
				}
				else {
					row.Cells.Add(patient.DateTimeLastAppt.ToShortDateString()); //6-Patients Last Appointment
				}
				if(patient.DateTimeNextAppt==DateTime.MinValue) {
					row.Cells.Add("");//7-Patients next Appointment (absent)
				}
				else {
					row.Cells.Add(patient.DateTimeNextAppt.ToShortDateString());//7-Patients next Appointment 
				}
				row.Tag=patient;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
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

		private void FillComboEmailHostingTemplate() {
			textEmailPreview.Clear();
			comboEmailHostingTemplate.Items.Clear();
		  long clinicNum;
			if(comboClinicPatient.IsAllSelected) {
				clinicNum=0;//Only display 'default' templates. This will prevent accidentally using a specific clinic's mass email signiture.
			}
			else {
				clinicNum=comboClinicPatient.SelectedClinicNum;
			}
			List<EmailHostingTemplate> listTemplates=EmailHostingTemplates.Refresh().FindAll(x => x.TemplateType==PromotionType.Manual && x.ClinicNum==clinicNum);
			for(int i = 0;i<listTemplates.Count;i++) {
				comboEmailHostingTemplate.Items.Add(listTemplates[i].TemplateName,listTemplates[i]);
			}
		}

		///<summary>Refreshed the patient data for the available patient's grid only. Uses filters to limit the data. If a patient already exists 
		///in the selected patients grid then they will not additionally be added here again so the user can't add them twice.</summary>
		private List<PatientInfo> RefreshPatients() {
			List<PatientInfo> listPatients=new List<PatientInfo>();
			List<long> listClinicNums=GetSelectedActivatedClinicNums();
			if(listClinicNums.Count>0) {//at least one valid selected clinic 
				DataTable tablePatients=GetPatientDataTable(checkUserQuery.Checked, listClinicNums);
				if(tablePatients!=null) {
					listPatients=PatientInfo.GetListPatientInfos(tablePatients);
				}
			}
			return listPatients;
		}

		private DataTable GetPatientDataTable(bool isCustomQuery, List<long> listClinicNums) {
			DataTable tablePatients=new DataTable();
			if(isCustomQuery) { //If we are using a custom query, return a DataTable based on those patnums
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
				int daysExcluding=checkExcludeWithin.Checked?PIn.Int(textNumDays.Text):-1;
				List<PatientStatus> listSelectedPatStatus=listBoxPatStatus.GetListSelected<PatientStatus>();
				if(ageFrom > ageTo) {
					MsgBox.Show(this, "The 'From age' cannot be greater than the 'To age'.");
					return null;
				}
				tablePatients=Patients.GetPatientsWithFirstLastAppointments(listSelectedPatStatus,checkHiddenFutureAppt.Checked
					,listClinicNums,ageFrom,ageTo,GetSeenSinceDateTime(),GetNotSeenSinceDateTime(),GetPatBillingType(),contactMethod,daysExcluding);
			}
			return tablePatients;
		}

		///<summary>List of all activated and enabled clinics selected by the user that are allowed to send mass emails.</summary>
		private List<long> GetSelectedActivatedClinicNums() {
			List<long> listClinicsActivatedEnabled=new List<long>();
			List<long> listClinics=(comboClinicPatient.SelectedClinicNum<0) ? comboClinicPatient.ListClinics.Select(x => x.ClinicNum).ToList() 
				: new List<long> { comboClinicPatient.SelectedClinicNum };
			foreach(long clinicNum in listClinics) {
				if(Clinics.IsMassEmailEnabled(clinicNum)) {
					listClinicsActivatedEnabled.Add(clinicNum);
				}
			}
			return listClinicsActivatedEnabled;
		}

		private bool IsMassEmailEnabled() {
			return GetSelectedActivatedClinicNums().Count>0;
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

		private void UpdateSelectedCount() {
			labelNumberPats.Text=gridMain.ListGridRows.Count(x=>x.Cells[0].Text.ToUpper()=="X").ToString();
		}

		private void listBoxPatStatus_SelectedIndexChanged(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void listBoxContactMethod_SelectedIndexChanged(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void listBoxPatBillingType_SelectedIndexChanged(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void checkHiddenFutureAppt_Click(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void checkExcludeWithin_Click(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void checkHideNotSeenSince_Click(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}
		
		private void checkBoxHideSeenSince_Click(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void checkUserQuery_Click(object sender,EventArgs e) {
			//TODO - Might want to check user preferences or permissions
			groupBoxUserQuery.Visible=checkUserQuery.Checked;
			groupBoxFilters.Visible=!checkUserQuery.Checked;
		}

		private void textNumDays_TextChanged(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
		}

		private void comboClinicPatient_SelectionChangeCommitted(object sender,EventArgs e) {
			labelRefreshNeeded.Visible=true;
			UpdateClinicIsNotEnabled();
			FillComboEmailHostingTemplate();
		}

		private void UpdateClinicIsNotEnabled() {
			EmailHostingTemplate template=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			//todo, change from ternary
			labelNotEnabled.Text=Lan.g(this,"* Mass Email not enabled for office");
			if(PrefC.HasClinicsEnabled) {
				labelNotEnabled.Text=Lan.g(this,"* Mass Email not enabled for clinic");
			}
			long clinicNum=comboClinicPatient.SelectedClinicNum;//todo, if All selected, use 0;
			if(comboClinicPatient.IsAllSelected) {
				clinicNum=0;
			}
			if(template!=null) {
				clinicNum=template.ClinicNum;
			}
			labelNotEnabled.Visible=!Clinics.IsMassEmailEnabled(clinicNum);
		}

		private void comboEmailHostingTemplate_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboEmailHostingTemplate.SelectedIndex==-1) {
				return;
			}
			textEmailPreview.Text=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().BodyPlainText;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.SelectedIndices.Length>1) {
				return;
			}
			gridMain.BeginUpdate();
			if(e.Col.ToString()=="0") {
				if(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text=="X") {
					gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text="";
				} else {
					gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text="X";
				}
			}
			gridMain.EndUpdate();
			UpdateSelectedCount();
			gridMain.SetSelected(e.Row);
		}

		private void textAgeFrom_TextChanged(object sender,EventArgs e) {
			if(!_isLoading) {
				labelRefreshNeeded.Visible=true;
			}
		}

		private void textAgeTo_TextChanged(object sender,EventArgs e) {
			if(!_isLoading) {
				labelRefreshNeeded.Visible=true;
			}
		}

		//This is for testing the new FormMassEmailTemplates and should be removed before any commits
		private void butSetupTemplates_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			using FormMassEmailTemplates formMassEmailTemplates=new FormMassEmailTemplates();
			formMassEmailTemplates.ShowDialog();
			//Incase a new template was added, refresh the template comboBox
			FillComboEmailHostingTemplate();
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

		private void butPreviewTemplate_Click(object sender,EventArgs e) {
			if(comboEmailHostingTemplate.SelectedIndex==-1) {
				MsgBox.Show("Please select an Email Template first.");
				return;
			}
			using FormWebBrowser formWebBrowser=new FormWebBrowser();
			EmailHostingTemplate template=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			string body=template.BodyPlainText;
			if(template.EmailTemplateType==EmailType.Html && !string.IsNullOrEmpty(template.BodyHTML)) {
				try { 
					body=MarkupEdit.TranslateToXhtml(template.BodyHTML,true,false,true);
				}
				catch(Exception ex) {
					ex.DoNothing();
					if(!MsgBox.Show(MsgBoxButtons.YesNo,"There was an issue rendering your email.  If you use this template, you may send malformed emails to " +
						"every selected patient. Do you want to continue saving?")) 
					{
						return;
					}
				}
			}
			formWebBrowser.browser.DocumentText=body;
			formWebBrowser.ShowDialog();
		}

		private void butSendEmails_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			EmailHostingTemplate template=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			if(template==null) {
				MsgBox.Show(this,"Template must be selected before email can be sent.");
				return;
			}
			if(!Clinics.IsMassEmailEnabled(template.ClinicNum)) {
				string error=Lan.g(this,"Mass Email not enabled ");
				string clinicAbbr=Clinics.GetAbbr(template.ClinicNum);
				if(!string.IsNullOrWhiteSpace(clinicAbbr)) {
					error+=Lan.g(this,"for ")+clinicAbbr;
				}
				MessageBox.Show(this,error);
				return;
			}
			List<PatientInfo> listPatientsSelected=new List<PatientInfo>();
			for(int i = 0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Cells[0].Text=="X") {
					listPatientsSelected.Add((PatientInfo)gridMain.ListGridRows[i].Tag);
				}
			}
			//send the email with the selected template to each person that is in the selected patients grid.
			if(listPatientsSelected.Count<=0) {
				MsgBox.Show(this,"Using the Available Patients list, highlight which patients to send this email to and then click 'Set Selected'.");
				return;
			}
			if(template.TemplateId==0) {
				string xhtml;//api templates must have the full html text, even if only partial html. Database templates will store partial as plain text. 
				xhtml=template.BodyHTML;
				if(template.EmailTemplateType==EmailType.Html && !string.IsNullOrEmpty(template.BodyHTML)) {
					//This might not work for images, we should consider blocking them or warning them about sending if we detect images
					try {
						xhtml=MarkupEdit.TranslateToXhtml(template.BodyHTML,true,false,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
						if(!MsgBox.Show(MsgBoxButtons.YesNo,"There was an issue rendering your email.  If you use this template, you may send malformed emails to " +
							"every selected patient. Do you want to continue saving?")) 
						{
							return;
						}
					}
				}
				//most likely case for this scenario, someone is sending one of the templates that came in the convert, without modifying it.
				//Create an API instance with the clinic num for this template.
				IAccountApi api=EmailHostingTemplates.GetAccountApi(comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().ClinicNum);
				try {
					CreateTemplateResponse response=api.CreateTemplate(new CreateTemplateRequest { 
						Template=new Template { 
							TemplateName=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().TemplateName,
							TemplateBodyHtml=xhtml,
							TemplateBodyPlainText=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().BodyPlainText,
							TemplateSubject=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().Subject,
						},
					});
					//This is how we can update the template later
					comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().TemplateId=response.TemplateNum;
					if(comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>().EmailHostingTemplateNum==0) {//New (not expected, should only execute when it's an existing that hasn't been added to backend.)
						EmailHostingTemplates.Insert(comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>());
					}
					else {
						EmailHostingTemplates.Update(comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>());
					}
				}
				catch(Exception ex) {
					FriendlyException.Show("Failed to create email from template. Please try again.",ex);
					return;
				}
			}
			//Verify that the email template is actually the apropriate form to be sending out to people
			using FormMassEmailSend formMassEmailSend=new FormMassEmailSend(template,listPatientsSelected);
			if(formMassEmailSend.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		///<summary>Helper class to store data for the main object used in this form.</summary>
		public class PatientInfo {
			public long PatNum;
			public string Name;
			public DateTime Birthdate;
			public string Email;
			public ContactMethod ContactMethod;
			public PatientStatus Status;
			public long ClinicNum;
			//DateTime of patient's most recent appointment
			public DateTime DateTimeLastAppt;
			public long NextAptNum;
			//DateTime of patient's next scheduled appointment
			public DateTime DateTimeNextAppt;

			public static List<PatientInfo> GetListPatientInfos(DataTable table) {
				List<PatientInfo> listPatientInfos=new List<PatientInfo>();
				foreach(DataRow row in table.Rows) {
					PatientInfo patInfo=new PatientInfo();
					patInfo.PatNum=PIn.Long(row["PatNum"].ToString());
					patInfo.Name=row["LName"].ToString()+", "+row["FName"].ToString();
					patInfo.Birthdate=PIn.Date(row["Birthdate"].ToString());
					patInfo.Email=row["Email"].ToString();
					patInfo.Status=PIn.Enum<PatientStatus>(row["PatStatus"].ToString());
					patInfo.ContactMethod=PIn.Enum<ContactMethod>(row["PreferContactMethod"].ToString());
					patInfo.DateTimeLastAppt=PIn.Date(row["DateTimeLastApt"].ToString());
					patInfo.DateTimeNextAppt=PIn.Date(row["DateTimeNextApt"].ToString());
					patInfo.NextAptNum=PIn.Long(row["NextAptNum"].ToString(),false);
					patInfo.ClinicNum=PIn.Long(row["ClinicNum"].ToString(),false);
					listPatientInfos.AddRange(CreateListForEmails(patInfo));//add a row for each email the patient has
				}
				return listPatientInfos;
			}

			///<summary>Returns a list for each unique email the patient has. Will not put patients in the list if they don't have an email.</summary>
			private static List<PatientInfo> CreateListForEmails(PatientInfo patient) {
				List<PatientInfo> listPatInfoForEmails=new List<PatientInfo>();
				if(!string.IsNullOrEmpty(patient.Email)) {
					string[] emails=patient.Email.Split(',',';');
					foreach(string email in emails) {
						if(!EmailAddresses.IsValidEmail(email,out MailAddress mailAddress)) {
							continue;
						}
						PatientInfo patEmail=GenericTools.DeepCopy<PatientInfo,PatientInfo>(patient);
						patEmail.Email=mailAddress.Address;
						listPatInfoForEmails.Add(patEmail);
					}
				}
				return listPatInfoForEmails.DistinctBy(x => x.Email).ToList();
			}
		}
	}
}