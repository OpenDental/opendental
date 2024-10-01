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
		private List<Clinic> _listClinicsAll;
		///<summary>A deep copy of Providers.ListShortDeep.  Use the cache instead of this list if you need an up to date list of providers.</summary>
		private List<Provider> _listProvidersAll;
		///<summary>A list of all operatories that have IsWebSched set to true.</summary>
		private List<Operatory> _listOperatoriesWebSched;
		///<summary>Clinic number used to filter the Time Slots grid.  0 is treated as 'Unassigned'</summary>
		private long _clinicNum=0;
		///<summary>Provider number used to filter the Time Slots grid.  0 is treated as 'All'</summary>
		private long _provNum=0;
		#endregion Fields

		public FormEServicesWebSchedRecall() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesWebSchedRecall_Load(object sender,EventArgs e) {
			//Disable all controls if user does not have EServicesSetup permission.
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true)) {
				DisableAllExcept();
			}
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
			_listClinicsAll=Clinics.GetDeepCopy();
			for(int i=0;i<_listClinicsAll.Count;i++) {
				if(_listClinicsAll[i].IsHidden) {
					continue;
				}
				comboWebSchedClinic.Items.Add(_listClinicsAll[i].Abbr,_listClinicsAll[i]);
			}
			comboWebSchedClinic.SelectedIndex=0;
			_listProvidersAll=Providers.GetDeepCopy(true).FindAll(x => !x.IsNotPerson);//Make sure that we only return not is not persons.
			comboWebSchedProviders.Items.Clear();
			comboWebSchedProviders.Items.Add(Lan.g(this,"All"));
			for(int i=0;i<_listProvidersAll.Count;i++) {
				comboWebSchedProviders.Items.Add(_listProvidersAll[i].GetLongDesc());
			}
			comboWebSchedProviders.SelectedIndex=0;
			if(!PrefC.HasClinicsEnabled) {
				labelWebSchedClinic.Visible=false;
				comboWebSchedClinic.Visible=false;
				butWebSchedPickClinic.Visible=false;
				butProvRulePickClinic.Visible=false;
			}
			FillGridWebSchedRecallTypes();
			FillGridWebSchedOperatories();
			FillListboxWebSchedProviderRule();
			FillBlockoutTypeListboxes();
			checkRecallAllowProvSelection.Checked=PrefC.GetBool(PrefName.WebSchedRecallAllowProvSelection);
			long confirmStatus=PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus);
			comboWSRConfirmStatus.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboWSRConfirmStatus.SetSelectedDefNum(confirmStatus);
			checkWSRDoubleBooking.Checked=PrefC.GetInt(PrefName.WebSchedRecallDoubleBooking)>0;//0 = Allow double booking, 1 = prevent
			textNumMonthsCheck.Text=PrefC.GetString(PrefName.WebSchedRecallApptSearchMaximumMonths);
		}

		#region Methods - Private

		private void FillGridWebSchedOperatories() {
			_listOperatoriesWebSched=Operatories.GetOpsForWebSched();
			int widthColOpName=170;
			int widthColClinic=80;
			if(!PrefC.HasClinicsEnabled) {
				widthColOpName+=widthColClinic;
			}
			gridWebSchedOperatories.BeginUpdate();
			gridWebSchedOperatories.Columns.Clear();
			gridWebSchedOperatories.Columns.Add(new GridColumn(Lan.g("TableOperatories","Op Name"),widthColOpName));
			gridWebSchedOperatories.Columns.Add(new GridColumn(Lan.g("TableOperatories","Abbrev"),70));
			if(PrefC.HasClinicsEnabled) {
				gridWebSchedOperatories.Columns.Add(new GridColumn(Lan.g("TableOperatories","Clinic"),widthColClinic));
			}
			gridWebSchedOperatories.Columns.Add(new GridColumn(Lan.g("TableOperatories","Provider"),90));
			gridWebSchedOperatories.Columns.Add(new GridColumn(Lan.g("TableOperatories","Hygienist"),90));
			gridWebSchedOperatories.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listOperatoriesWebSched.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listOperatoriesWebSched[i].OpName);
				row.Cells.Add(_listOperatoriesWebSched[i].Abbrev);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listOperatoriesWebSched[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(_listOperatoriesWebSched[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(_listOperatoriesWebSched[i].ProvHygienist));
				gridWebSchedOperatories.ListGridRows.Add(row);
			}
			gridWebSchedOperatories.EndUpdate();
		}

		///<summary>Also refreshed the combo box of available recall types.</summary>
		private void FillGridWebSchedRecallTypes() {
			//Keep track of the previously selected recall type.
			long recallTypeNumSelected=0;
			if(comboWebSchedRecallTypes.SelectedIndex!=-1) {
				recallTypeNumSelected=_listRecallTypes[comboWebSchedRecallTypes.SelectedIndex].RecallTypeNum;
			}
			//Fill the combo boxes for the time slots preview.
			comboWebSchedRecallTypes.Items.Clear();
			_listRecallTypes=RecallTypes.GetDeepCopy();
			for(int i=0;i<_listRecallTypes.Count;i++) {
				comboWebSchedRecallTypes.Items.Add(_listRecallTypes[i].Description);
				if(_listRecallTypes[i].RecallTypeNum==recallTypeNumSelected) {
					comboWebSchedRecallTypes.SelectedIndex=i;
				}
			}
			if(recallTypeNumSelected==0 && comboWebSchedRecallTypes.Items.Count>0) {
				comboWebSchedRecallTypes.SelectedIndex=0;//Arbitrarily select the first recall type.
			}
			gridWebSchedRecallTypes.BeginUpdate();
			gridWebSchedRecallTypes.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRecallTypes","Description"),130);
			gridWebSchedRecallTypes.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Time Pattern"),100);
			gridWebSchedRecallTypes.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Time Length"),80);
			col.IsWidthDynamic=true;
			col.TextAlign=HorizontalAlignment.Center;
			gridWebSchedRecallTypes.Columns.Add(col);
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
			gridWebSchedTimeSlots.Columns.Clear();
			GridColumn col=new GridColumn("",20);
			col.IsWidthDynamic=true;
			col.TextAlign=HorizontalAlignment.Center;
			gridWebSchedTimeSlots.Columns.Add(col);
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
			Clinic clinic=_listClinicsAll.Find(x => x.ClinicNum==_clinicNum);//null clinic is treated as unassigned.
			List<Provider> listProviders=new List<Provider>(_listProvidersAll);//Use all providers by default.
			Provider provider=_listProvidersAll.Find(x => x.ProvNum==_provNum);
			if(provider!=null) {
				//Only use the provider that the user picked from the provider picker.
				listProviders=new List<Provider>() { provider };
			}
			List<TimeSlot> listTimeSlots=new List<TimeSlot>();
			//This throws exceptions when the selected clinic does not have a operatory with it.
			//Takes a few minutes with the nadg database.
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => {
				//Get the next 30 days of open time schedules with the current settings.
				listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recallType,listProviders,clinic,dateStart,dateStart.AddDays(30),isFromWebSched:false);
			};
			progressOD.StartingMessage=Lan.g(this,"Loading available time slots. This can take a few minutes on large databases.");
			try{
				progressOD.ShowDialog();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
			}
			if(progressOD.IsCancelled){
				//nothing to do.
			}
			FillGridWebSchedTimeSlots(listTimeSlots);
		}

		///<summary>Fills the Generally Allowed and Restricted To blockout listboxes. </summary>
		private void FillBlockoutTypeListboxes() {
			//Get every deflink for RecallType
			List<DefLink> listDefLinksRecallType=DefLinks.GetDefLinksByType(DefLinkType.RecallType);
			//Restricted-To Blockouts
			List<long> listRestrictedToBlockoutRecallTypeNums=listDefLinksRecallType.Select(x => x.DefNum).Distinct().ToList();
			List<long> listRestrictedToBlockoutNums=listDefLinksRecallType.Select(x => x.DefNum).Distinct().ToList();
			List<Def> listDefsRestrictedToBlockouts=Defs.GetDefs(DefCat.BlockoutTypes,listRestrictedToBlockoutNums);
			//Allowed Blockouts
			List<Def> listAllowedBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,PrefC.GetWebSchedRecallAllowedBlockouts);
			List<long> listAllowedBlockoutTypeNums=listAllowedBlockoutTypes.Select(x => x.DefNum).Distinct().ToList();
			List<Def> listDefAllowedBlockouts=Defs.GetDefs(DefCat.BlockoutTypes,listAllowedBlockoutTypeNums.FindAll(x => !listRestrictedToBlockoutRecallTypeNums.Contains(x)));
			//Fill the list box for Restricted to Blockouts. "BlockoutName (RecalType1.Name,RecallType2.Name)"
			List<long> listRecallTypeNums=new List<long>();
			List<RecallType> listRecallTypes=new List<RecallType>();
			List<string> listRecallTypeNames=new List<string>();
			List<string> listBlockoutReasonAssociations=new List<string>();
			for(int i=0;i<listDefsRestrictedToBlockouts.Count;i++) {
				listRecallTypeNums=listDefLinksRecallType.Where(x => x.DefNum==listDefsRestrictedToBlockouts[i].DefNum).Select(x => x.FKey).ToList();
				listRecallTypes=RecallTypes.GetWhere(x => listRecallTypeNums.Contains(x.RecallTypeNum));
				listRecallTypeNames=listRecallTypes.Select(x => x.Description).ToList();
				listBlockoutReasonAssociations.Add(listDefsRestrictedToBlockouts[i].ItemName+" ("+string.Join(",",listRecallTypeNames)+")"); //TODO (restrict to recall type)
			}
			listboxRestrictedToBlockouts.Items.Clear();
			listboxRestrictedToBlockouts.Items.AddList(listBlockoutReasonAssociations,x => x);
			//Fill the list box for Generally Allowed
			listboxWebSchedRecallIgnoreBlockoutTypes.Items.Clear();
			listboxWebSchedRecallIgnoreBlockoutTypes.Items.AddList(listDefAllowedBlockouts.Select(x => x.ItemName),x => x);
		}

		private void FillListboxWebSchedProviderRule() {
			ClinicPref clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.ClinicNumSelected,isDefaultIncluded:true);
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

		private void SetListBoxWebSchedProviderPref(ClinicPref clinicPref) {
			if(clinicPref==null) {//Using defaults.
				listBoxWebSchedProviderPref.SelectedIndex=PrefC.GetInt(PrefName.WebSchedProviderRule);
			}
			else {
				listBoxWebSchedProviderPref.SelectedIndex=PIn.Int(clinicPref.ValueString);
			}
		}

		///<summary>Shows the Operatories window and allows the user to edit them.  Does not show the window if user does not have Setup permission.
		///Refreshes all corresponding grids within the Web Sched tab that display Operatory information.  Feel free to add to this method.</summary>
		private void ShowOperatoryEditAndRefreshGrids() {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormOperatories formOperatories=new FormOperatories();
			formOperatories.ShowDialog();
			if(formOperatories.ListAppointmentsConflicting.Count>0) {
				FormApptConflicts formApptConflicts=new FormApptConflicts(formOperatories.ListAppointmentsConflicting);
				formApptConflicts.Show();
				formApptConflicts.BringToFront();
			}
			FillGridWebSchedOperatories();
			FillGridWebSchedTimeSlots();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Operatories accessed via EServices Setup window.");
		}

		private void EditRecallTypes() {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormRecallTypes formRecallTypes=new FormRecallTypes();
			formRecallTypes.ShowDialog();
			FillGridWebSchedRecallTypes();
			FillBlockoutTypeListboxes();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Recall Types accessed via EServices Setup window.");
		}
		#endregion Methods - Private

		#region Methods - Event Handlers
		private void butEditOperatories_Click(object sender,EventArgs e) {
			ShowOperatoryEditAndRefreshGrids();
		}

		private void butEditRecallTypes_Click(object sender,EventArgs e) {
			EditRecallTypes();
		}

		private void butWebSchedRecallRestrictedToBlockoutEdit_Click(object sender,EventArgs e) {
			EditRecallTypes();
		}

		private void butProvRulePickClinic_Click(object sender,EventArgs e) {
			using FormClinics formClinics=new FormClinics();
			formClinics.IsSelectionMode=true;
			formClinics.ShowDialog();
			if(formClinics.DialogResult!=DialogResult.OK) {
				return;
			}
			comboClinicProvRule.ClinicNumSelected=formClinics.ClinicNumSelected;
			FillListboxWebSchedProviderRule();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridWebSchedTimeSlots();
		}

		private void butWebSchedPickClinic_Click(object sender,EventArgs e) {
			using FormClinics formClinics=new FormClinics();
			formClinics.IsSelectionMode=true;
			formClinics.ShowDialog();
			if(formClinics.DialogResult!=DialogResult.OK) {
				return;
			}
			comboWebSchedClinic.SelectedIndex=_listClinicsAll.FindIndex(x => x.ClinicNum==formClinics.ClinicNumSelected)+1;//+1 for 'Unassigned'
			_clinicNum=formClinics.ClinicNumSelected;
		}

		private void butWebSchedPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick=new FrmProviderPick();
			if(comboWebSchedProviders.SelectedIndex>0) {
				frmProviderPick.ProvNumSelected=_provNum;
			}
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboWebSchedProviders.SelectedIndex=_listProvidersAll.FindIndex(x => x.ProvNum==frmProviderPick.ProvNumSelected)+1;//+1 for 'All'
			_provNum=frmProviderPick.ProvNumSelected;
		}

		private void butWebSchedRecallBlockouts_Click(object sender,EventArgs e) {
			string[] stringArrayDefNums=PrefC.GetString(PrefName.WebSchedRecallIgnoreBlockoutTypes).Split(new char[] {','}); //comma-delimited list.
			List<long> listBlockoutTypes=new List<long>();
			for(int i=0;i<stringArrayDefNums.Length;i++) {
				listBlockoutTypes.Add(PIn.Long(stringArrayDefNums[i]));
			}
			List<Def> listDefsBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,listBlockoutTypes);
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.BlockoutTypes,listDefsBlockoutTypes);
			formDefinitionPicker.HasShowHiddenOption=true;
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				listboxWebSchedRecallIgnoreBlockoutTypes.Items.Clear();
				for(int i=0;i<formDefinitionPicker.ListDefsSelected.Count;i++) {
					listboxWebSchedRecallIgnoreBlockoutTypes.Items.Add(formDefinitionPicker.ListDefsSelected[i].ItemName);
				}
				string strListWebSchedRecallIgnoreBlockoutTypes=String.Join(",",formDefinitionPicker.ListDefsSelected.Select(x => x.DefNum));
				Prefs.UpdateString(PrefName.WebSchedRecallIgnoreBlockoutTypes,strListWebSchedRecallIgnoreBlockoutTypes);
			}
			FillBlockoutTypeListboxes();
		}

		private void butWebSchedRecallNotify_Click(object sender,EventArgs e) {
			using FormEServicesWebSchedNotify formEServicesWebSchedNotify=new FormEServicesWebSchedNotify(WebSchedNotifyType.Recall);
			formEServicesWebSchedNotify.ShowDialog();
		}

		private void butWebSchedSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormRecallSetup formRecallSetup=new FormRecallSetup();
			formRecallSetup.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Recall Setup accessed via EServices Setup window.");
		}

		private void butWebSchedToday_Click(object sender,EventArgs e) {
			textWebSchedDateStart.Text=DateTime.Today.ToShortDateString();
		}

		private void checkUseDefaultProvRule_Click(object sender,EventArgs e) {
			listBoxWebSchedProviderPref.Enabled=(!checkUseDefaultProvRule.Checked);
			ClinicPref clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.ClinicNumSelected,isDefaultIncluded:true);
			if(checkUseDefaultProvRule.Checked) {
				if(ClinicPrefs.DeletePrefs(comboClinicProvRule.ClinicNumSelected,new List<PrefName>() { PrefName.WebSchedProviderRule })>0) {
					DataValid.SetInvalid(InvalidType.ClinicPrefs);//Checking "Use Defaults", delete ClinicPref is exists.
				}
			}
			else if(!comboClinicProvRule.IsUnassignedSelected) {
				//Unchecking "Use Defaults" and on a clinic that doesn't have a ClinicPref yet, create new ClinicPref with default value
				if(clinicPrefProviderRule!=null) {
					ClinicPrefs.DeletePrefs(comboClinicProvRule.ClinicNumSelected,new List<PrefName>() { PrefName.WebSchedProviderRule });
				}
				ClinicPrefs.InsertPref(PrefName.WebSchedProviderRule,comboClinicProvRule.ClinicNumSelected
					,POut.Int(PrefC.GetInt(PrefName.WebSchedProviderRule)));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			clinicPrefProviderRule=ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,comboClinicProvRule.ClinicNumSelected,isDefaultIncluded: true);
			SetListBoxWebSchedProviderPref(clinicPrefProviderRule);
		}

		private void comboClinicProvRule_SelectionChangeCommitted(object sender,EventArgs e) {
			FillListboxWebSchedProviderRule();
		}

		private void comboWebSchedClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			_clinicNum=0;
			if(comboWebSchedClinic.SelectedIndex>0) {//Greater than 0 due to "Unassigned"
				_clinicNum=comboWebSchedClinic.GetSelected<Clinic>().ClinicNum;
			}
		}

		private void comboWebSchedProviders_SelectionChangeCommitted(object sender,EventArgs e) {
			_provNum=0;
			if(comboWebSchedProviders.SelectedIndex>0) {//Greater than 0 due to "All"
				_provNum=_listProvidersAll[comboWebSchedProviders.SelectedIndex-1].ProvNum;//-1 for 'All'
			}
		}

		private void listBoxWebSchedProviderPref_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinicProvRule.IsUnassignedSelected && listBoxWebSchedProviderPref.SelectedIndex!=PrefC.GetInt(PrefName.WebSchedProviderRule)) {
				Prefs.UpdateInt(PrefName.WebSchedProviderRule,listBoxWebSchedProviderPref.SelectedIndex);//Update default preference.
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else if(!comboClinicProvRule.IsUnassignedSelected && !checkUseDefaultProvRule.Checked
				&& ClinicPrefs.Upsert(PrefName.WebSchedProviderRule,comboClinicProvRule.ClinicNumSelected,POut.Int(listBoxWebSchedProviderPref.SelectedIndex))) 
			{//Clinic not set to use defaults.
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
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
			List<string> listSetupErrors=new List<string>();
			if(!radioDoNotSendText.Checked && !textWebSchedPerBatch.IsValid()) {
				listSetupErrors.Add("- "+Lan.g(this,"Max texts per clinic is invalid."));
			}
			if(!textWebSchedRecallApptSearchDays.IsValid()) {
				listSetupErrors.Add("- "+Lan.g(this,"Number of days in the future is invalid."));
			}
			if(!textNumMonthsCheck.IsValid()) {
				listSetupErrors.Add("- "+Lan.g(this,"Max number of months to search is invalid."));
			}
			if(!radioDoNotSend.Checked || !radioDoNotSendText.Checked) {
				if(PrefC.GetLong(PrefName.RecallShowIfDaysFirstReminder)<1) {//Initial Reminder field
					listSetupErrors.Add("- "+Lan.g(this,"Initial Reminder field has to be greater than 0."));
				}
				if(PrefC.GetLong(PrefName.RecallShowIfDaysSecondReminder)<1) {//Second(or more) Reminder field
					listSetupErrors.Add("- "+Lan.g(this,"Second (or more) Reminder field has to be greater than 0."));
				}
			}
			if(listSetupErrors.Count>0) {
				MessageBox.Show(Lan.g(this,"Recall Setup settings are not correctly set in order to Send Messages Automatically to patients:")
						+"\r\n"+string.Join("\r\n",listSetupErrors)
					,Lan.g(this,"Web Sched - Recall Setup Error"));
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
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"WebSched automated email preference changed from "+webSchedAutomaticSendOld.ToString()+" to "+webSchedAutomaticSendNew.ToString()+".");
			}
			WebSchedAutomaticSend webSchedAutomaticSendTextNew=WebSchedAutomaticSend.SendToText;
			if(radioDoNotSendText.Checked) {
				webSchedAutomaticSendTextNew=WebSchedAutomaticSend.DoNotSend;
			}
			WebSchedAutomaticSend webSchedAutomaticSendTextOld=(WebSchedAutomaticSend)PrefC.GetInt(PrefName.WebSchedAutomaticSendTextSetting);
			if(Prefs.UpdateInt(PrefName.WebSchedAutomaticSendTextSetting,(int)webSchedAutomaticSendTextNew)) {
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"WebSched automated text preference changed from "+webSchedAutomaticSendTextOld.ToString()+" to "+webSchedAutomaticSendTextNew.ToString()+".");
			}
			int textsPerBatchOld=PrefC.GetInt(PrefName.WebSchedTextsPerBatch);
			int textsPerBatchNew=PIn.Int(textWebSchedPerBatch.Text,false);
			if(Prefs.UpdateInt(PrefName.WebSchedTextsPerBatch,textsPerBatchNew)) {
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"WebSched batch size preference changed from "+textsPerBatchOld.ToString()+" to "+textsPerBatchNew.ToString()+".");
			}
			int recallApptDays=PIn.Int(textWebSchedRecallApptSearchDays.Text);
			Prefs.UpdateInt(PrefName.WebSchedRecallApptSearchAfterDays,recallApptDays);
			Prefs.UpdateBool(PrefName.WebSchedRecallAllowProvSelection,checkRecallAllowProvSelection.Checked);
			if(comboWSRConfirmStatus.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.WebSchedRecallConfirmStatus,comboWSRConfirmStatus.GetSelectedDefNum());
			}
			int valueWebSchedRecallDoubleBooking=checkWSRDoubleBooking.Checked ? 1 : 0;
			Prefs.UpdateInt(PrefName.WebSchedRecallDoubleBooking,valueWebSchedRecallDoubleBooking);
			Prefs.UpdateInt(PrefName.WebSchedRecallApptSearchMaximumMonths,PIn.Int(textNumMonthsCheck.Text,false));
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!AreSettingsValid()) {
				return;
			}
			SaveWebSchedRecall();
			DialogResult=DialogResult.OK;
		}

	}
}