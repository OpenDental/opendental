using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailBirthdays:FormODBase {
		private bool _doSetInvalidClinicPrefs=false;
		private bool _doSetInvalidPrefs=false;
		private List<ApptReminderRule> _listApptReminderRules;
		private List<EmailHostingTemplate> _listEmailHostingTemplates;
		///<summary>Used for syncing clinics, which clinics are using the default rules.</summary>
		private List<ClinicPref> _listClinicPrefsDefaultOld;
		private List<ClinicPref> _listClinicPrefsDefaultNew;
		///<summary>Used for syncing clinics, which clinics are enabled to send messages for birthdays.</summary>
		private List<ClinicPref> _listClinicPrefsEnabledOld;
		private List<ClinicPref> _listClinicPrefsEnabledNew;
		///<summary>Used when clinics are not enabled to determine if the rule is enabled.</summary>
		private bool _isEnabled;
		///<summary>True while in the load method.</summary>
		private bool _isLoading;

		public FormMassEmailBirthdays() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailBirthdays_Load(object sender,EventArgs e) {
			_isLoading=true;
			#region Fill Data
			_listClinicPrefsDefaultOld=ClinicPrefs.GetPrefAllClinics(PrefName.BirthdayPromotionsUseDefaults);
			_listClinicPrefsDefaultNew=GenericTools.DeepCopy<List<ClinicPref>,List<ClinicPref>>(_listClinicPrefsDefaultOld);
			_listClinicPrefsEnabledOld=ClinicPrefs.GetPrefAllClinics(PrefName.BirthdayPromotionsEnabled,true);//include default to handle 'no clinics'
			_listClinicPrefsEnabledNew=GenericTools.DeepCopy<List<ClinicPref>,List<ClinicPref>>(_listClinicPrefsEnabledOld);
			_isEnabled=PrefC.GetBool(PrefName.BirthdayPromotionsEnabled);
			_listApptReminderRules=ApptReminderRules.GetForTypes(ApptReminderType.Birthday);
			List<long> listEmailHostingTemplateNums=_listApptReminderRules.Select(x => x.EmailHostingTemplateNum).ToList();
			_listEmailHostingTemplates=EmailHostingTemplates.GetMany(listEmailHostingTemplateNums);
			#endregion
			#region Fill UI
			string [] stringArrayLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');///From pref that gets languages used by patients. Abbreviated form.
			if(stringArrayLanguages.Length==0 
				|| (stringArrayLanguages.Length==1 && stringArrayLanguages[0].ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim())) 
			{
				butLangaugeAdd.Visible=false;
			}
			if(_listApptReminderRules.FindAll(x => x.ClinicNum==0).Count==1) {//default to default clinic on load. 
				butLanguageRemove.Visible=false;
			}
			for(int i = 0;i < 22;i++) {
				comboMinorAge.Items.Add(i.ToString());
			}
			FillForClinic();
			#endregion
			if(!Security.IsAuthorized(Permissions.EServicesSetup,true)) {
				DisableAllExcept(butCancel);
			}
			_isLoading=false;
		}

		///<summary>Is either the selected clinic num from the combo box OR clinic zero if selected clinic is using defaults. Will also return 0 if clinics are  turned off.</summary>
		private long ClinicNumEffective(){
			if(IsClinicsUsingDefaults()){
				return 0;
			}
			return comboClinic.SelectedClinicNum;
		}

		private bool IsClinicsUsingDefaults(){
			ClinicPref clinicPref=_listClinicPrefsDefaultNew.Find(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				return clinicPref==null || clinicPref.ValueString=="1";
		}
	
		private void SetClinicsUsingDefaults(bool isUsingDefaults){
			if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
					return;//if not using clinics, defaults do not exist, and 'Defaults' do not exist for the default clinic. 
				}
				ClinicPref clinicPref=_listClinicPrefsDefaultNew.Find(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				if(clinicPref==null) {
					clinicPref=new ClinicPref(comboClinic.SelectedClinicNum,PrefName.BirthdayPromotionsUseDefaults,isUsingDefaults);
					_listClinicPrefsDefaultNew.Add(clinicPref);
				}
				clinicPref.ValueString=POut.Bool(isUsingDefaults);
				_doSetInvalidClinicPrefs=true;
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			long clinicNumPrevious=((ApptReminderRule)tabControlLanguages.SelectedTab.Tag).ClinicNum;
			if(!_isLoading) {
				string errors=ValidateCurrentSelection();
				if(!string.IsNullOrEmpty(errors)) {//if there were problems, do not change clinics.
					comboClinic.SelectedClinicNum=clinicNumPrevious;
					MessageBox.Show(this,errors);
					return;
				}
				MapUiToRules(_listApptReminderRules.First(x => x.ClinicNum==clinicNumPrevious && x.Language==""));
			}
			FillForClinic();
		}

		private string ValidateCurrentSelection() {
			//make sure that the current rule and template are in a state that could be saved before we move on to a different set. 
			StringBuilder stringBuilderErrors=new StringBuilder();
			if(!checkIsEnabled.Checked) {
				return "";//no need to validate disabled rules.
			}
			if(!textDays.IsValid() || Math.Abs(PIn.Int(textDays.Text,false))>364) {
				stringBuilderErrors.AppendLine(Lan.g(this,"Lead time must 364 days or less."));
			}
			else if(PIn.Int(textDays.Text,false)==0 && (radioAfterBirthday.Checked || radioBeforeBirthday.Checked)) {
				stringBuilderErrors.AppendLine(Lan.g(this,"Days cannot be set to 0 when not sending on the patient's birthday"));
			}
			//we also need to make sure the all of the language rules have the same timing information as their leader.If the leader gets updated, they all
			//need to be updated. I don't think there's code for that yet. 
			List<ApptReminderRule> listApptReminderRules = _listApptReminderRules.FindAll(x => x.ClinicNum==ClinicNumEffective());
			for(int i=0;i<listApptReminderRules.Count;i++){
				EmailHostingTemplate emailHostingTemplate=_listEmailHostingTemplates.FirstOrDefault(x => listApptReminderRules[i].EmailHostingTemplateNum==x.EmailHostingTemplateNum);
				if(emailHostingTemplate==null) {
					stringBuilderErrors.AppendLine(Lan.g(this,"Rule is missing template. Please call support."));
				}
				if(string.IsNullOrEmpty(emailHostingTemplate.BodyPlainText)) {//this may be validated already in the template edit window.
					stringBuilderErrors.AppendLine(Lan.g(this,"Plain text field must be filled out"));
				}
				if(emailHostingTemplate.EmailTemplateType!=EmailType.Regular && string.IsNullOrEmpty(emailHostingTemplate.BodyHTML)) {
					stringBuilderErrors.AppendLine(Lan.g(this,"Html Text must be filled out, or email must be changed to a plain text version."));
				}
			}
			return stringBuilderErrors.ToString();
		}

		///<summary>Fills all necessary data for the clinic.</summary>
		private void FillForClinic() {
			labelNotActivated.Visible=!Clinics.IsMassEmailSignedUp(ClinicNumEffective()) && !Clinics.IsSecureEmailSignedUp(ClinicNumEffective());
			checkUseDefaultsBirthday.Checked=comboClinic.SelectedClinicNum>0 && IsClinicsUsingDefaults();
			if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
				checkIsEnabled.Checked=_isEnabled;
			}
			else {
				ClinicPref clinicPref=_listClinicPrefsEnabledNew.Find(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				checkIsEnabled.Checked=clinicPref!=null && PIn.Bool(clinicPref.ValueString);
			}
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			if(comboClinic.SelectedClinicNum>0) {
				checkUseDefaultsBirthday.Visible=true;
				checkUseDefaultsBirthday.Enabled=allowEdit;
				labelDefaults.Visible=false;
			}
			else {//Defaults or Practice
				checkUseDefaultsBirthday.Visible=false;
				labelDefaults.Visible=PrefC.HasClinicsEnabled;
			}
			#region Enabled All Controls By Default
			groupSendTime.Enabled=true;
			butEditTemplate.Enabled=true;
			butLangaugeAdd.Enabled=true;
			butLanguageRemove.Enabled=true;
			textDays.Enabled=true;
			checkBoxSendGuarantorBirthdayForMinor.Enabled=true;
			#endregion
			checkIsEnabled.Enabled=allowEdit;
			List<ApptReminderRule> listApptReminderRules=_listApptReminderRules
				.FindAll(x => x.ClinicNum==ClinicNumEffective())
				.OrderByDescending(x => x.Language=="").ToList();
			//if we do want to send for a minor's birthday then the box should not be checked since it is labeled "Do NOT send..."
			checkBoxSendGuarantorBirthdayForMinor.Checked=!listApptReminderRules.First().IsSendForMinorsBirthday;
			comboMinorAge.Enabled=checkBoxSendGuarantorBirthdayForMinor.Checked;
			comboMinorAge.SelectedIndex=listApptReminderRules.First().MinorAge;
			textDays.Text=Math.Abs(listApptReminderRules.First().TSPrior.Days).ToString();//Days, not total Days.
			if(listApptReminderRules.First().TSPrior==TimeSpan.Zero) {
				radioOnBirthday.Checked=true;
				textDays.Text=PIn.String("0");
				textDays.Enabled=false;
			}
			else if(listApptReminderRules.First().TSPrior > TimeSpan.Zero) {
				radioBeforeBirthday.Checked=true;
			}
			else{
				radioAfterBirthday.Checked=true;
			}
			tabControlLanguages.TabPages.Clear();
			for(int i=0;i<listApptReminderRules.Count;i++){
				EmailHostingTemplate emailHostingTemplate=_listEmailHostingTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==listApptReminderRules[i].EmailHostingTemplateNum);
				UserControlEmailTemplate userControlEmailTemplate=new UserControlEmailTemplate();
				userControlEmailTemplate.RefreshView(emailHostingTemplate.BodyPlainText,emailHostingTemplate.BodyHTML,emailHostingTemplate.EmailTemplateType);
				userControlEmailTemplate.Dock=DockStyle.Fill;
				TabPage tabPage=new TabPage();
				string language="";
				if(string.IsNullOrEmpty(listApptReminderRules[i].Language)) {
					language=Lan.g(this,"Default");
				}
				else {
					CultureInfo cultureInfo=MiscUtils.GetCultureFromThreeLetter(listApptReminderRules[i].Language);
					if(cultureInfo==null) {
						language=listApptReminderRules[i].Language;
					}
					else {
						language=cultureInfo.DisplayName;
					}
				}
				tabPage.Text=language;
				tabPage.Tag=listApptReminderRules[i];
				tabPage.Controls.Add(userControlEmailTemplate);
				LayoutManager.Add(tabPage,tabControlLanguages);		
			}
			LayoutManager.LayoutControlBoundsAndFonts(tabControlLanguages);
			if(comboClinic.SelectedClinicNum > 0 && IsClinicsUsingDefaults()) {
				DisableAllExcept(butOK,butCancel,comboClinic,checkIsEnabled,checkUseDefaultsBirthday,tabControlLanguages);
			}
		}

		private void butLangaugeAdd_Click(object sender,EventArgs e) {
			List<string> listLanguagesDisplay=new List<string>();//contains the user friendly name ex: "Spanish"
			List<string> listLanguagesData=new List<string>();//contains the db friendly string ex: "SPA"
			List<string> listLanguageRulesCurrent=new List<string>();
			listLanguageRulesCurrent=_listApptReminderRules.FindAll(x => x.ClinicNum==ClinicNumEffective()).Select(x => x.Language).ToList();
			string [] stringArrayLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');///From pref that gets languages used by patients. Abbreviated form.
			for(int i=0;i<stringArrayLanguages.Length;i++) {
				if(stringArrayLanguages[i].IsNullOrEmpty() || stringArrayLanguages[i].ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim()) {
					continue;
				}
				if(listLanguageRulesCurrent.Contains(stringArrayLanguages[i]))	{
					continue;//we already have a tab for this langauge. 
				}
				CultureInfo cultureInfo=MiscUtils.GetCultureFromThreeLetter(stringArrayLanguages[i]);
				if(cultureInfo==null) {
					listLanguagesDisplay.Add(stringArrayLanguages[i]);//custom language - display what the user entered
				}
				else {
					listLanguagesDisplay.Add(cultureInfo.DisplayName);//display full name of the abbreviation
				}
				listLanguagesData.Add(stringArrayLanguages[i]);
			}
			if(listLanguagesDisplay.Count==0) {
				MsgBox.Show(this,"No additional languages available.");
				return;
			}
			using InputBox inputBoxLanguageSelect=new InputBox(Lan.g(this,"Select language for template: "),listLanguagesDisplay,0);
			inputBoxLanguageSelect.ShowDialog();
			if(inputBoxLanguageSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			ApptReminderRule apptReminderRule=_listApptReminderRules
				.FindAll(x => x.ClinicNum==ClinicNumEffective())
				.First(x => x.Language=="");
			MapUiToRules(apptReminderRule);//first update anything that may have changed for the default rule before adding the langauge.
			ApptReminderRule apptReminderRuleCopy=CopyRule(apptReminderRule,apptReminderRule.ClinicNum);
			apptReminderRuleCopy.Language=listLanguagesData[inputBoxLanguageSelect.SelectedIndex];
			FillForClinic();
			butLanguageRemove.Visible=true;
		}

		private void butLanguageRemove_Click(object sender,EventArgs e) {
			if(((ApptReminderRule)tabControlLanguages.SelectedTab.Tag).Language=="") {
				MsgBox.Show(this,"Cannot remove the default template.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete the currently selected language?")) {
				return;
			}
			ApptReminderRule apptReminderRule=(ApptReminderRule)tabControlLanguages.SelectedTab.Tag;
			RemoveRules(apptReminderRule);
			LayoutManager.Remove(tabControlLanguages.SelectedTab);
		}

		private void butEditTemplate_Click(object sender,EventArgs e) {
			if(!CanUseMassEmail()) {
				return;
			}
			if(checkUseDefaultsBirthday.Checked) {
				checkUseDefaultsBirthday.Checked=false;
				SwitchFromDefaultsBirthday();
			}
			ApptReminderRule apptReminderRule=(ApptReminderRule)tabControlLanguages.SelectedTab.Tag;
			MapUiToRules(apptReminderRule);//update anything about the rule that may have previously changed before editing.
			EmailHostingTemplate emailHostingTemplate=_listEmailHostingTemplates.First(x => apptReminderRule.EmailHostingTemplateNum==x.EmailHostingTemplateNum);
			//We do not want offices to delete Automated Birthday templates.
			//if a clinic decides they don't want their own Automated Birthday template, they don't delete the template, they should check the "Use Defaults" box to use the HQ rule.
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(emailHostingTemplate,canDeleteTemplate:false);
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillForClinic();
		}

		private bool CanUseMassEmail() {
			if(!Clinics.IsMassEmailEnabled(comboClinic.SelectedClinicNum)) {
				MsgBox.Show(this,"Mass Email first needs to be activated and enabled for this clinic in the eServices Mass Email Setup window.");
				return false;
			}
			return true;
		}

		private void radioBeforeBirthday_Click(object sender,EventArgs e) {
			textDays.Enabled=true;
		}

		private void radioAfterBirthday_Click(object sender,EventArgs e) {
			textDays.Enabled=true;
		}

		private void radioOnBirthday_Click(object sender,EventArgs e) {
			textDays.Text=PIn.String("0");
			textDays.Enabled=false;
		}

		private void checkIsEnabled_Click(object sender,EventArgs e) {
			if(checkIsEnabled.Checked) {//attempting to enable
				if(!CanUseMassEmail()) {
					checkIsEnabled.Checked=false;
					return;
				}
				string errors=ValidateCurrentSelection();
				if(!string.IsNullOrEmpty(errors) || !Security.IsAuthorized(Permissions.EServicesSetup,true)) {
					checkIsEnabled.Checked=false;
					MessageBox.Show(this,errors);
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Mass Email is based on usage. " +
						"By activating and enabling this feature you are agreeing to the charges. Continue?")) 
				{
					checkIsEnabled.Checked=false;
					return;
				}
			}
			if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
				_isEnabled=checkIsEnabled.Checked;
				return;//pref will be set upon saving based only on checkbox status.
			}
			ClinicPref clinicPref=_listClinicPrefsEnabledNew.Find(x => x.ClinicNum==comboClinic.SelectedClinicNum);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(comboClinic.SelectedClinicNum,PrefName.BirthdayPromotionsEnabled,checkIsEnabled.Checked);
				_listClinicPrefsEnabledNew.Add(clinicPref);
			}
			clinicPref.ValueString=POut.Bool(checkIsEnabled.Checked);
			_doSetInvalidClinicPrefs=true;
		}

		private void checkUseDefaultsBirthday_Click(object sender,EventArgs e) {
			if(!checkUseDefaultsBirthday.Checked && comboClinic.SelectedClinicNum>0) {
				SwitchFromDefaultsBirthday();//turning defaults off
				return;
			}
			if(checkUseDefaultsBirthday.Checked && comboClinic.SelectedClinicNum>0) {
				SwitchToDefaultsBirthday();//turning defaults on
			}
		}

		private void checkBoxSendGuarantorBirthdayForMinor_CheckedChanged(object sender,EventArgs e) {
			comboMinorAge.Enabled=checkBoxSendGuarantorBirthdayForMinor.Checked;
		}

		///<summary>Switches the currently selected clinic over to using non-defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchFromDefaultsBirthday() {
			if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			SetClinicsUsingDefaults(false);
			List<ApptReminderRule> listApptReminderRules=_listApptReminderRules.FindAll(x => x.ClinicNum==0);
			for(int i=0;i<listApptReminderRules.Count;i++){
				CopyRule(listApptReminderRules[i],comboClinic.SelectedClinicNum);
			}
			FillForClinic();
			return true;
		}

		private ApptReminderRule CopyRule(ApptReminderRule apptReminderRule,long clinicNum) {
			EmailHostingTemplate emailHostingTemplateDefaultRule=_listEmailHostingTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==apptReminderRule.EmailHostingTemplateNum);
			EmailHostingTemplate emailHostingTemplateClinicRule=GenericTools.DeepCopy<EmailHostingTemplate,EmailHostingTemplate>(emailHostingTemplateDefaultRule);
			emailHostingTemplateClinicRule.TemplateId=0;//So the api uploads this new template.
			emailHostingTemplateClinicRule.ClinicNum=clinicNum;
			EmailHostingTemplates.Insert(emailHostingTemplateClinicRule);
			_listEmailHostingTemplates.Add(emailHostingTemplateClinicRule);
			ApptReminderRule apptReminderRuleCopy=GenericTools.DeepCopy<ApptReminderRule,ApptReminderRule>(apptReminderRule);
			apptReminderRuleCopy.ApptReminderRuleNum=0;//ApptReminderRules.Sync() expects new rules to have 0 pk
			apptReminderRuleCopy.ClinicNum=clinicNum;
			apptReminderRuleCopy.EmailHostingTemplateNum=emailHostingTemplateClinicRule.EmailHostingTemplateNum;
			_listApptReminderRules.Add(apptReminderRuleCopy);
			return apptReminderRuleCopy;
		}

		private void RemoveRules(params ApptReminderRule[] apptReminderRuleArray) {
			for(int i=0;i<apptReminderRuleArray.Length;i++) {
				_listApptReminderRules.Remove(apptReminderRuleArray[i]);
			}
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchToDefaultsBirthday() {
			if(comboClinic.SelectedClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			if(_listApptReminderRules.FindAll(x => x.ClinicNum==comboClinic.SelectedClinicNum).Count>0 && 
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete custom rules for this clinic and switch to using defaults? This cannot be undone.")) 
			{
				checkUseDefaultsBirthday.Checked=false;//undo checking of box.
				return false;
			}
			RemoveRules(_listApptReminderRules.Where(x => x.ClinicNum==comboClinic.SelectedClinicNum).ToArray());
			//The corresponding EmailHostingTemplates will be deleted at save/sync
			SetClinicsUsingDefaults(true);
			FillForClinic();
			return true;
		}

		///<summary>This will update the passed in rule based on the current UI state. It will only save if the user ends up clicking ok.
		///This should be called with the default rule passed in (no language set).</summary>
		private void MapUiToRules(ApptReminderRule apptReminderRule) {
			if(apptReminderRule==null) {
				return;//this may happen in some cases when the defaults are being used, in which case, it is okay to just return. No changes to be saved.
			}
			if(radioBeforeBirthday.Checked) {
				apptReminderRule.TSPrior=new TimeSpan(PIn.Int(textDays.Text,false),0,0,0);
			}
			else {
				apptReminderRule.TSPrior=new TimeSpan(-PIn.Int(textDays.Text,false),0,0,0);
			}
			//if we do want to send for a minors bithday then the box will NOT be checked
			apptReminderRule.IsSendForMinorsBirthday=!checkBoxSendGuarantorBirthdayForMinor.Checked;
			apptReminderRule.MinorAge=PIn.Int(comboMinorAge.SelectedIndex.ToString());//combo box is 1:1 with age from 0 to 21
			List<ApptReminderRule> listApptReminderRules=_listApptReminderRules.FindAll(x => x.ClinicNum==apptReminderRule.ClinicNum && x.Language!="").ToList();
			for(int i=0;i<listApptReminderRules.Count;i++){
				listApptReminderRules[i].TSPrior=apptReminderRule.TSPrior;
				listApptReminderRules[i].IsSendForMinorsBirthday=apptReminderRule.IsSendForMinorsBirthday;
				listApptReminderRules[i].MinorAge=apptReminderRule.MinorAge;
			}
		}

		private void SaveAll() {
			//All UI elements will have been previously saved when switching clinics, the only one that might have not been is the 
			//current, if clinics are off or if we only made changes the the one that loaded in. 
			MapUiToRules(_listApptReminderRules.FindAll(x => x.ClinicNum==ClinicNumEffective())
				.First(x => x.Language==""));
			_doSetInvalidPrefs|=Prefs.UpdateBool(PrefName.BirthdayPromotionsEnabled,_isEnabled);
			if(PrefC.HasClinicsEnabled) {
				_doSetInvalidClinicPrefs|=ClinicPrefs.Sync(_listClinicPrefsDefaultNew,_listClinicPrefsDefaultOld);
				_doSetInvalidClinicPrefs|=ClinicPrefs.Sync(_listClinicPrefsEnabledNew,_listClinicPrefsEnabledOld);
				for(int i=0;i< comboClinic.ListClinics.Count;i++){
					ApptReminderRules.SyncByClinicAndTypes(_listApptReminderRules.FindAll(x => x.ClinicNum==comboClinic.ListClinics[i].ClinicNum),comboClinic.ListClinics[i].ClinicNum,ApptReminderType.Birthday);
				}
			}
			else {
				ApptReminderRules.SyncByClinicAndTypes(_listApptReminderRules.FindAll(x => x.ClinicNum==0),0,ApptReminderType.Birthday);
			}
			ODThread thread=new ODThread(o => {		
				//Syncs EmailHostingTemplate table with ODHQ.
				WebServiceMainHQProxy.GetEServiceSetupFull(SignupPortalPermission.FullPermission);
			});
			thread.AddExceptionHandler((ex) => ex.DoNothing());
			thread.Start();
		}

		private void butOK_Click(object sender,EventArgs e) {
			string errors=ValidateCurrentSelection();
			if(!string.IsNullOrEmpty(errors)) {
				MessageBox.Show(this,errors);
				return;
			}
			try {
				SaveAll();
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Failed to save."),ex);
				return;
			}
			if(_doSetInvalidPrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(_doSetInvalidClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}