using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEServicesWebSchedRecall:FormODBase {
		#region Fields
		///<summary>A list of recall types to display.</summary>
		private List<RecallType> _listRecallTypes;
		///<summary>A list of all clinics.  This list could include clinics that the user should not have access to so be careful using it. For the sake of modular code, there are seperate lists for the Integrated Texting (sms) and Automated eConfirmation (eC) tabs.</summary>
		private List<Clinic> _listClinicsWebSched;
		///<summary>A deep copy of Providers.ListShortDeep.  Use the cache instead of this list if you need an up to date list of providers.</summary>
		private List<Provider> _listProvidersWebSched;
		///<summary>A list of all operatories that have IsWebSched set to true.</summary>
		private List<Operatory> _listOpsWebSchedRecall;
		///<summary>Clinic number used to filter the Time Slots grid.  0 is treated as 'Unassigned'</summary>
		private long _webSchedClinicNum=0;
		///<summary>Provider number used to filter the Time Slots grid.  0 is treated as 'All'</summary>
		private long _webSchedProvNum=0;
		#endregion Fields

		public FormEServicesWebSchedRecall() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesWebSchedRecall_Load(object sender,EventArgs e) {
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			textWebSchedRecallApptSearchDays.Enabled=allowEdit;
			butRecallSchedSetup.Enabled=allowEdit;
			butWebSchedRecallBlockouts.Enabled=allowEdit;
			groupWebSchedProvRule.Enabled=allowEdit;
			groupBoxWebSchedAutomation.Enabled=allowEdit;
			groupWebSchedPreview.Enabled=allowEdit;
			groupWebSchedText.Enabled=allowEdit;
			int recallApptDays=PrefC.GetInt(PrefName.WebSchedRecallApptSearchAfterDays);
			string strNumRecallApptDays="";
			if(recallApptDays>0) {
				strNumRecallApptDays=recallApptDays.ToString();
			}
			textWebSchedRecallApptSearchDays.Text=strNumRecallApptDays;
			switch(PrefC.GetInt(PrefName.WebSchedAutomaticSendSetting)) {
				case (int)WebSchedAutomaticSend.DoNotSend:
					radioDoNotSend.Checked=true;
					break;
				case (int)WebSchedAutomaticSend.SendToEmail:
					radioSendToEmail.Checked=true;
					break;
				case (int)WebSchedAutomaticSend.SendToEmailNoPreferred:
					radioSendToEmailNoPreferred.Checked=true;
					break;
				case (int)WebSchedAutomaticSend.SendToEmailOnlyPreferred:
					radioSendToEmailOnlyPreferred.Checked=true;
					break;
			}
			switch(PrefC.GetInt(PrefName.WebSchedAutomaticSendTextSetting)) {
				case (int)WebSchedAutomaticSend.DoNotSend:
					radioDoNotSendText.Checked=true;
					break;
				case (int)WebSchedAutomaticSend.SendToText:
					radioSendText.Checked=true;
					break;
			}
			textWebSchedPerBatch.Enabled=!radioDoNotSendText.Checked;
			textWebSchedPerBatch.Text=PrefC.GetString(PrefName.WebSchedTextsPerBatch);
			textWebSchedDateStart.Text=DateTime.Today.AddDays(recallApptDays).ToShortDateString();
			comboWebSchedClinic.Items.Clear();
			comboWebSchedClinic.Items.Add(Lan.g(this,"Unassigned"));
			_listClinicsWebSched=Clinics.GetDeepCopy();
			for(int i=0;i<_listClinicsWebSched.Count;i++) {
				comboWebSchedClinic.Items.Add(_listClinicsWebSched[i].Abbr);
			}
			comboWebSchedClinic.SelectedIndex=0;
			_listProvidersWebSched=Providers.GetDeepCopy(true);
			comboWebSchedProviders.Items.Clear();
			comboWebSchedProviders.Items.Add(Lan.g(this,"All"));
			for(int i=0;i<_listProvidersWebSched.Count;i++) {
				comboWebSchedProviders.Items.Add(_listProvidersWebSched[i].GetLongDesc());
			}
			comboWebSchedProviders.SelectedIndex=0;
			if(!PrefC.HasClinicsEnabled) {
				labelWebSchedClinic.Visible=false;
				comboWebSchedClinic.Visible=false;
				butWebSchedPickClinic.Visible=false;
				butProvRulePickClinic.Visible=false;
			}
			FillListboxAllowedBlockoutTypes();
			FillGridWebSchedRecallTypes();
			FillGridWebSchedOperatories();
			FillListboxWebSchedProviderRule();
			checkRecallAllowProvSelection.Checked=PrefC.GetBool(PrefName.WebSchedRecallAllowProvSelection);
			long defaultStatus=PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus);
			comboWSRConfirmStatus.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboWSRConfirmStatus.SetSelectedDefNum(defaultStatus);
			checkWSRDoubleBooking.Checked=PrefC.GetInt(PrefName.WebSchedRecallDoubleBooking)>0;//0 = Allow double booking, 1 = prevent
		}

		#region Methods - Private

		private void FillGridWebSchedOperatories() {
			_listOpsWebSchedRecall=Operatories.GetOpsForWebSched();
			int opNameWidth=170;
			int clinicWidth=80;
			if(!PrefC.HasClinicsEnabled) {
				opNameWidth+=clinicWidth;
			}
			gridWebSchedOperatories.BeginUpdate();
			gridWebSchedOperatories.ListGridColumns.Clear();
			gridWebSchedOperatories.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Op Name"),opNameWidth));
			gridWebSchedOperatories.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Abbrev"),70));
			if(PrefC.HasClinicsEnabled) {
				gridWebSchedOperatories.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Clinic"),clinicWidth));
			}
			gridWebSchedOperatories.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Provider"),90));
			gridWebSchedOperatories.ListGridColumns.Add(new GridColumn(Lan.g("TableOperatories","Hygienist"),90));
			gridWebSchedOperatories.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listOpsWebSchedRecall.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listOpsWebSchedRecall[i].OpName);
				row.Cells.Add(_listOpsWebSchedRecall[i].Abbrev);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listOpsWebSchedRecall[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(_listOpsWebSchedRecall[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(_listOpsWebSchedRecall[i].ProvHygienist));
				gridWebSchedOperatories.ListGridRows.Add(row);
			}
			gridWebSchedOperatories.EndUpdate();
		}

		///<summary>Also refreshed the combo box of available recall types.</summary>
		private void FillGridWebSchedRecallTypes() {
			//Keep track of the previously selected recall type.
			long selectedRecallTypeNum=0;
			if(comboWebSchedRecallTypes.SelectedIndex!=-1) {
				selectedRecallTypeNum=_listRecallTypes[comboWebSchedRecallTypes.SelectedIndex].RecallTypeNum;
			}
			//Fill the combo boxes for the time slots preview.
			comboWebSchedRecallTypes.Items.Clear();
			_listRecallTypes=RecallTypes.GetDeepCopy();
			for(int i=0;i<_listRecallTypes.Count;i++) {
				comboWebSchedRecallTypes.Items.Add(_listRecallTypes[i].Description);
				if(_listRecallTypes[i].RecallTypeNum==selectedRecallTypeNum) {
					comboWebSchedRecallTypes.SelectedIndex=i;
				}
			}
			if(selectedRecallTypeNum==0 && comboWebSchedRecallTypes.Items.Count>0) {
				comboWebSchedRecallTypes.SelectedIndex=0;//Arbitrarily select the first recall type.
			}
			gridWebSchedRecallTypes.BeginUpdate();
			gridWebSchedRecallTypes.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRecallTypes","Description"),130);
			gridWebSchedRecallTypes.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Time Pattern"),100);
			gridWebSchedRecallTypes.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Time Length"),80) { IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridWebSchedRecallTypes.ListGridColumns.Add(col);
			gridWebSchedRecallTypes.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listRecallTypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listRecallTypes[i].Description);
				row.Cells.Add(_listRecallTypes[i].TimePattern);
				int timeLength=RecallTypes.ConvertTimePattern(_listRecallTypes[i].TimePattern).Length * 5;
				if(timeLength==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(timeLength.ToString()+" "+Lan.g("TableRecallTypes","mins"));
				}
				gridWebSchedRecallTypes.ListGridRows.Add(row);
			}
			gridWebSchedRecallTypes.EndUpdate();
		}

		private void FillGridWebSchedTimeSlots(List<TimeSlot> listTimeSlots) {
			gridWebSchedTimeSlots.BeginUpdate();
			gridWebSchedTimeSlots.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",20){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Center;
			gridWebSchedTimeSlots.ListGridColumns.Add(col);
			gridWebSchedTimeSlots.ListGridRows.Clear();
			GridRow row;
			DateTime dateTimeSlotLast=DateTime.MinValue;
			for(int i=0;i<listTimeSlots.Count;i++) {
				//Make a new row for every unique day.
				if(dateTimeSlotLast.Date!=listTimeSlots[i].DateTimeStart.Date) {
					dateTimeSlotLast=listTimeSlots[i].DateTimeStart;
					row=new GridRow();
					row.ColorBackG=Color.LightBlue;
					row.Cells.Add(listTimeSlots[i].DateTimeStart.ToShortDateString());
					gridWebSchedTimeSlots.ListGridRows.Add(row);
				}
				row=new GridRow();
				row.Cells.Add(listTimeSlots[i].DateTimeStart.ToShortTimeString()+" - "+listTimeSlots[i].DateTimeStop.ToShortTimeString());
				gridWebSchedTimeSlots.ListGridRows.Add(row);
			}
			gridWebSchedTimeSlots.EndUpdate();
		}

		private void FillGridWebSchedTimeSlots() {
			//Validate time slot settings.
			if(!textWebSchedDateStart.IsValid()) {
				//Don't bother warning the user.  It will just be annoying.  The red indicator should be sufficient.
				return;
			}
			if(comboWebSchedRecallTypes.SelectedIndex<0
				||comboWebSchedClinic.SelectedIndex<0
				||comboWebSchedProviders.SelectedIndex<0) {
				return;
			}
			DateTime dateStart=PIn.Date(textWebSchedDateStart.Text);
			RecallType recallType=_listRecallTypes[comboWebSchedRecallTypes.SelectedIndex];
			Clinic clinic=_listClinicsWebSched.Find(x => x.ClinicNum==_webSchedClinicNum);//null clinic is treated as unassigned.
			List<Provider> listProviders=new List<Provider>(_listProvidersWebSched);//Use all providers by default.
			Provider provider=_listProvidersWebSched.Find(x => x.ProvNum==_webSchedProvNum);
			if(provider!=null) {
				//Only use the provider that the user picked from the provider picker.
				listProviders=new List<Provider>() { provider };
			}
			List<TimeSlot> listTimeSlots=new List<TimeSlot>();
			//This throws exceptions when the selected clinic does not have a operatory with it.
			//Takes a few minutes with the nadg database.
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				//Get the next 30 days of open time schedules with the current settings.
				listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recallType,listProviders,clinic,dateStart,dateStart.AddDays(30));
			};
			progressOD.StartingMessage=Lan.g(this,"Loading available time slots. This can take a few minutes on large databases.");
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
			}
			if(progressOD.IsCancelled){
				//nothing to do.
			}
			FillGridWebSchedTimeSlots(listTimeSlots);
		}

		private void FillListboxAllowedBlockoutTypes() {
			List<Def> listDefBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,PrefC.GetWebSchedRecallAllowedBlockouts);
			listboxWebSchedRecallIgnoreBlockoutTypes.Items.Clear();
			for(int i=0;i<listDefBlockoutTypes.Count;i++) {
				listboxWebSchedRecallIgnoreBlockoutTypes.Items.Add(listDefBlockoutTypes[i].ItemName);
			}
		}

		private void FillListboxWebSchedProviderRule() {
			ClinicPref clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.SelectedClinicNum,isDefaultIncluded:true);
			checkUseDefaultProvRule.Visible=(!comboClinicProvRule.IsUnassignedSelected);//"Use Defaults" checkbox visible when actual clinic is selected.
			checkUseDefaultProvRule.Checked=(clinicPrefProviderRule==null);
			if(checkUseDefaultProvRule.Visible) {
				listBoxWebSchedProviderPref.Enabled=(!checkUseDefaultProvRule.Checked);
			}
			else {
				listBoxWebSchedProviderPref.Enabled=true;
			}
			SetListBoxWebSchedProviderPref(clinicPrefProviderRule);//Select ClincPref's value, or default if no ClinicPref.
		}

		private void SetListBoxWebSchedProviderPref(ClinicPref pref) {
			if(pref==null) {//Using defaults.
				listBoxWebSchedProviderPref.SelectedIndex=PrefC.GetInt(PrefName.WebSchedProviderRule);
			}
			else {
				listBoxWebSchedProviderPref.SelectedIndex=PIn.Int(pref.ValueString);
			}
		}

		///<summary>Shows the Operatories window and allows the user to edit them.  Does not show the window if user does not have Setup permission.
		///Refreshes all corresponding grids within the Web Sched tab that display Operatory information.  Feel free to add to this method.</summary>
		private void ShowOperatoryEditAndRefreshGrids() {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormOperatories FormO=new FormOperatories();
			FormO.ShowDialog();
			if(FormO.ListConflictingAppts.Count>0) {
				FormApptConflicts FormAC=new FormApptConflicts(FormO.ListConflictingAppts);
				FormAC.Show();
				FormAC.BringToFront();
			}
			FillGridWebSchedOperatories();
			FillGridWebSchedTimeSlots();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Operatories accessed via EServices Setup window.");
		}
		#endregion Methods - Private

		#region Methods - Event Handlers
		private void butEditOperatories_Click(object sender,EventArgs e) {
			ShowOperatoryEditAndRefreshGrids();
		}

		private void butEditRecallTypes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormRecallTypes FormRT=new FormRecallTypes();
			FormRT.ShowDialog();
			FillGridWebSchedRecallTypes();
			FillGridWebSchedTimeSlots();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Recall Types accessed via EServices Setup window.");
		}

		private void butProvRulePickClinic_Click(object sender,EventArgs e) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			FormC.ShowDialog();
			if(FormC.DialogResult!=DialogResult.OK) {
				return;
			}
			comboClinicProvRule.SelectedClinicNum=FormC.SelectedClinicNum;
			FillListboxWebSchedProviderRule();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridWebSchedTimeSlots();
		}

		private void butWebSchedPickClinic_Click(object sender,EventArgs e) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			FormC.ShowDialog();
			if(FormC.DialogResult!=DialogResult.OK) {
				return;
			}
			comboWebSchedClinic.SelectedIndex=_listClinicsWebSched.FindIndex(x => x.ClinicNum==FormC.SelectedClinicNum)+1;//+1 for 'Unassigned'
			_webSchedClinicNum=FormC.SelectedClinicNum;
		}

		private void butWebSchedPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick();
			if(comboWebSchedProviders.SelectedIndex>0) {
				FormPP.SelectedProvNum=_webSchedProvNum;
			}
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboWebSchedProviders.SelectedIndex=_listProvidersWebSched.FindIndex(x => x.ProvNum==FormPP.SelectedProvNum)+1;//+1 for 'All'
			_webSchedProvNum=FormPP.SelectedProvNum;
		}

		private void butWebSchedRecallBlockouts_Click(object sender,EventArgs e) {
			string[] arrayDefNums=PrefC.GetString(PrefName.WebSchedRecallIgnoreBlockoutTypes).Split(new char[] {','}); //comma-delimited list.
			List<long> listBlockoutTypes=new List<long>();
			for(int i=0;i<arrayDefNums.Length;i++) {
				listBlockoutTypes.Add(PIn.Long(arrayDefNums[i]));
			}
			List<Def> listDefBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,listBlockoutTypes);
			using FormDefinitionPicker FormDP=new FormDefinitionPicker(DefCat.BlockoutTypes,listDefBlockoutTypes);
			FormDP.HasShowHiddenOption=true;
			FormDP.IsMultiSelectionMode=true;
			FormDP.ShowDialog();
			if(FormDP.DialogResult==DialogResult.OK) {
				listboxWebSchedRecallIgnoreBlockoutTypes.Items.Clear();
				for(int i=0;i<FormDP.ListDefsSelected.Count;i++) {
					listboxWebSchedRecallIgnoreBlockoutTypes.Items.Add(FormDP.ListDefsSelected[i].ItemName);
				}
				string strListWebSchedRecallIgnoreBlockoutTypes=String.Join(",",FormDP.ListDefsSelected.Select(x => x.DefNum));
				Prefs.UpdateString(PrefName.WebSchedRecallIgnoreBlockoutTypes,strListWebSchedRecallIgnoreBlockoutTypes);
			}
		}

		private void butWebSchedRecallNotify_Click(object sender,EventArgs e) {
			using FormEServicesWebSchedNotify formESWebSchedNotify=new FormEServicesWebSchedNotify(WebSchedNotifyType.Recall);
			formESWebSchedNotify.ShowDialog();
		}

		private void butWebSchedSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormRecallSetup FormRS=new FormRecallSetup();
			FormRS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Recall Setup accessed via EServices Setup window.");
		}

		private void butWebSchedToday_Click(object sender,EventArgs e) {
			textWebSchedDateStart.Text=DateTime.Today.ToShortDateString();
		}

		private void checkUseDefaultProvRule_Click(object sender,EventArgs e) {
			listBoxWebSchedProviderPref.Enabled=(!checkUseDefaultProvRule.Checked);
			ClinicPref clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.SelectedClinicNum,isDefaultIncluded:true);
			if(checkUseDefaultProvRule.Checked) {
				if(ClinicPrefs.DeletePrefs(comboClinicProvRule.SelectedClinicNum,new List<PrefName>() { PrefName.WebSchedProviderRule })>0) {
					DataValid.SetInvalid(InvalidType.ClinicPrefs);//Checking "Use Defaults", delete ClinicPref is exists.
				}
			}
			else if(!comboClinicProvRule.IsUnassignedSelected) {
				//Unchecking "Use Defaults" and on a clinic that doesn't have a ClinicPref yet, create new ClinicPref with default value
				if(clinicPrefProviderRule!=null) {
					ClinicPrefs.DeletePrefs(comboClinicProvRule.SelectedClinicNum,new List<PrefName>() { PrefName.WebSchedProviderRule });
				}
				ClinicPrefs.InsertPref(PrefName.WebSchedProviderRule,comboClinicProvRule.SelectedClinicNum
					,POut.Int(PrefC.GetInt(PrefName.WebSchedProviderRule)));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.SelectedClinicNum,isDefaultIncluded: true);
			SetListBoxWebSchedProviderPref(clinicPrefProviderRule);
		}

		private void comboClinicProvRule_SelectionChangeCommitted(object sender,EventArgs e) {
			FillListboxWebSchedProviderRule();
		}

		private void comboWebSchedClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			_webSchedClinicNum=0;
			if(comboWebSchedClinic.SelectedIndex>0) {//Greater than 0 due to "Unassigned"
				_webSchedClinicNum=_listClinicsWebSched[comboWebSchedClinic.SelectedIndex-1].ClinicNum;//-1 for 'Unassigned'
			}
		}

		private void comboWebSchedProviders_SelectionChangeCommitted(object sender,EventArgs e) {
			_webSchedProvNum=0;
			if(comboWebSchedProviders.SelectedIndex>0) {//Greater than 0 due to "All"
				_webSchedProvNum=_listProvidersWebSched[comboWebSchedProviders.SelectedIndex-1].ProvNum;//-1 for 'All'
			}
		}

		private void listBoxWebSchedProviderPref_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinicProvRule.IsUnassignedSelected && listBoxWebSchedProviderPref.SelectedIndex!=PrefC.GetInt(PrefName.WebSchedProviderRule)) {
				Prefs.UpdateInt(PrefName.WebSchedProviderRule,listBoxWebSchedProviderPref.SelectedIndex);//Update default preference.
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else if(!comboClinicProvRule.IsUnassignedSelected && !checkUseDefaultProvRule.Checked
				&& ClinicPrefs.Upsert(PrefName.WebSchedProviderRule,comboClinicProvRule.SelectedClinicNum,POut.Int(listBoxWebSchedProviderPref.SelectedIndex))) 
			{//Clinic not set to use defaults.
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void textWebSchedRecallApptSearchDays_Validated(object sender,EventArgs e) {
			if(!textWebSchedRecallApptSearchDays.IsValid()) {
				return;
			}
			int recallApptDays=PIn.Int(textWebSchedRecallApptSearchDays.Text);
			if(recallApptDays<0) {
				recallApptDays=0;
			}
			Prefs.UpdateInt(PrefName.WebSchedRecallApptSearchAfterDays,recallApptDays);
		}

		private void WebSchedRecallAutoSendRadioButtons_CheckedChanged(object sender,EventArgs e) {
			if(radioDoNotSend.Checked && radioDoNotSendText.Checked) {
				textWebSchedPerBatch.Enabled=false;
				return;
			}
			//Validate the following recall setup preferences.  See task #880961 or #879613 for more details.
			//1. The Days Past field is not blank
			//2. The Initial Reminder field is greater than 0
			//3. The Second(or more) Reminder field is greater than 0
			//4. Integrated texting is enabled if Send Text is checked
			List<string> listSetupErrors=new List<string>();
			bool isEmailSendInvalid=false;
			bool isTextSendInvalid=false;
			if(PrefC.GetLong(PrefName.RecallDaysPast)==-1) {//Days Past field
				listSetupErrors.Add("- "+Lan.g(this,"Days Past (e.g. 1095, blank, etc) field cannot be blank."));
				isEmailSendInvalid=true;
				isTextSendInvalid=true;
			}
			if(PrefC.GetLong(PrefName.RecallShowIfDaysFirstReminder)<1) {//Initial Reminder field
				listSetupErrors.Add("- "+Lan.g(this,"Initial Reminder field has to be greater than 0."));
				isEmailSendInvalid=true;
				isTextSendInvalid=true;
			}
			if(PrefC.GetLong(PrefName.RecallShowIfDaysSecondReminder)<1) {//Second(or more) Reminder field
				listSetupErrors.Add("- "+Lan.g(this,"Second (or more) Reminder field has to be greater than 0."));
				isEmailSendInvalid=true;
				isTextSendInvalid=true;
			}
			if(radioSendText.Checked && !SmsPhones.IsIntegratedTextingEnabled()) {
				listSetupErrors.Add("- "+Lan.g(this,"Integrated texting must be enabled."));
				isTextSendInvalid=true;
			}
			//Checking the "Do Not Send" radio button will automatically uncheck all the other radio buttons in the group box.
			if(isEmailSendInvalid) {
				radioDoNotSend.Checked=true;
			}
			if(isTextSendInvalid) {
				radioDoNotSendText.Checked=true;
			}
			textWebSchedPerBatch.Enabled=!radioDoNotSendText.Checked;
			if(listSetupErrors.Count>0) {
				MessageBox.Show(Lan.g(this,"Recall Setup settings are not correctly set in order to Send Messages Automatically to patients:")
						+"\r\n"+string.Join("\r\n",listSetupErrors)
					,Lan.g(this,"Web Sched - Recall Setup Error"));
			}
		}
		#endregion Methods - Event Handlers

		private bool AreSettingsValid() {
			if(!radioDoNotSendText.Checked && !textWebSchedPerBatch.IsValid()) {
				return false;
			}
			return true;
		}

		private void SaveWebSchedRecall() {
			WebSchedAutomaticSend webSchedAutomaticSendNew=WebSchedAutomaticSend.SendToEmailOnlyPreferred;
			if(radioDoNotSend.Checked) {
				webSchedAutomaticSendNew=WebSchedAutomaticSend.DoNotSend;
			}
			else if(radioSendToEmail.Checked) {
				webSchedAutomaticSendNew=WebSchedAutomaticSend.SendToEmail;
			}
			else if(radioSendToEmailNoPreferred.Checked) {
				webSchedAutomaticSendNew=WebSchedAutomaticSend.SendToEmailNoPreferred;
			}
			WebSchedAutomaticSend webSchedAutomaticSendOld=(WebSchedAutomaticSend)PrefC.GetInt(PrefName.WebSchedAutomaticSendSetting);
			if(Prefs.UpdateInt(PrefName.WebSchedAutomaticSendSetting,(int)webSchedAutomaticSendNew)) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"WebSched automated email preference changed from "+webSchedAutomaticSendOld.ToString()+" to "+webSchedAutomaticSendNew.ToString()+".");
			}
			WebSchedAutomaticSend webSchedAutomaticSendTypeTextNew=WebSchedAutomaticSend.SendToText;
			if(radioDoNotSendText.Checked) {
				webSchedAutomaticSendTypeTextNew=WebSchedAutomaticSend.DoNotSend;
			}
			WebSchedAutomaticSend webSchedAutomaticSendTextOld=(WebSchedAutomaticSend)PrefC.GetInt(PrefName.WebSchedAutomaticSendTextSetting);
			if(Prefs.UpdateInt(PrefName.WebSchedAutomaticSendTextSetting,(int)webSchedAutomaticSendTypeTextNew)) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"WebSched automated text preference changed from "+webSchedAutomaticSendTextOld.ToString()+" to "+webSchedAutomaticSendTypeTextNew.ToString()+".");
			}
			int oldTextsPerBatch=PrefC.GetInt(PrefName.WebSchedTextsPerBatch);
			int newTextsPerBatch=PIn.Int(textWebSchedPerBatch.Text,false);
			if(Prefs.UpdateInt(PrefName.WebSchedTextsPerBatch,newTextsPerBatch)) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"WebSched batch size preference changed from "+oldTextsPerBatch.ToString()+" to "+newTextsPerBatch.ToString()+".");
			}
			Prefs.UpdateBool(PrefName.WebSchedRecallAllowProvSelection,checkRecallAllowProvSelection.Checked);
			if(comboWSRConfirmStatus.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.WebSchedRecallConfirmStatus,comboWSRConfirmStatus.GetSelectedDefNum());
			}
			int isWSRDoubleBooking=checkWSRDoubleBooking.Checked ? 1 : 0;
			Prefs.UpdateInt(PrefName.WebSchedRecallDoubleBooking,isWSRDoubleBooking);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!AreSettingsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SaveWebSchedRecall();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}