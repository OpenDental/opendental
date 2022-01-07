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
		private List<ApptReminderRule> _listClinicBirthdayRules;
		private List<EmailHostingTemplate> _listTemplates;
		///<summary>Used for syncing clinics, which clinics are using the default rules.</summary>
		private List<ClinicPref> _listClinicPrefDefaultOld;
		private List<ClinicPref> _listClinicPrefDefaultNew;
		///<summary>USed for syncing clinics, which clinics are enabled to send messages for birthdays.</summary>
		private List<ClinicPref> _listClinicPrefEnabledOld;
		private List<ClinicPref> _listClinicPrefEnabledNew;
		///<summary>Used when clinics are not enabled to determine if the rule is enabled.</summary>
		private bool _isEnabled;
		///<summary>True while in the load method.</summary>
		private bool _isLoading;

		///<summary>Is either the selected clinic num from the combo box OR clinic zero if selected clinic is using defaults. Will also return 0 if
		///clinics are  turned off.</summary>
		private long _effectiveClinicNum {	get {		return _isClinicUsingDefaults?0:comboClinic.SelectedClinicNum;	}	}

		///<summary>From pref that gets languages used by patients. Abbreviated form. </summary>
		private string[] _arrLanguages {
			get {
				return PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');
			}
		}

		private bool _isClinicUsingDefaults {
			get {
				ClinicPref clinicPref=_listClinicPrefDefaultNew.FirstOrDefault(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				return clinicPref==null || clinicPref.ValueString=="1";
			}
			set {
				if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
					return;//if not using clinics, defaults do not exist, and 'Defaults' do not exist for the default clinic. 
				}
				ClinicPref clinicPref=_listClinicPrefDefaultNew.FirstOrDefault(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				if(clinicPref==null) {
					clinicPref=new ClinicPref(comboClinic.SelectedClinicNum,PrefName.BirthdayPromotionsUseDefaults,value);
					_listClinicPrefDefaultNew.Add(clinicPref);
				}
				clinicPref.ValueString=POut.Bool(value);
				_doSetInvalidClinicPrefs=true;
			}
		}

		private bool _isClinicEnabled {
			get {
				if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
					return _isEnabled;
				}
				else {
					ClinicPref clinicPref=_listClinicPrefEnabledNew.FirstOrDefault(x => x.ClinicNum==comboClinic.SelectedClinicNum);
					return clinicPref!=null && PIn.Bool(clinicPref.ValueString);
				}
			}
			set {
				if(!PrefC.HasClinicsEnabled || comboClinic.SelectedClinicNum==0) {
					_isEnabled=value;
					return;//pref will be set upon saving based only on checkbox status.
				}
				ClinicPref clinicPref=_listClinicPrefEnabledNew.FirstOrDefault(x => x.ClinicNum==comboClinic.SelectedClinicNum);
				if(clinicPref==null) {
					clinicPref=new ClinicPref(comboClinic.SelectedClinicNum,PrefName.BirthdayPromotionsEnabled,value);
					_listClinicPrefEnabledNew.Add(clinicPref);
				}
				clinicPref.ValueString=POut.Bool(value);
				_doSetInvalidClinicPrefs=true;
			}
		}

		public FormMassEmailBirthdays() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailBirthdays_Load(object sender,EventArgs e) {
			_isLoading=true;
			#region Fill Data
			_listClinicPrefDefaultOld=ClinicPrefs.GetPrefAllClinics(PrefName.BirthdayPromotionsUseDefaults);
			_listClinicPrefDefaultNew=GenericTools.DeepCopy<List<ClinicPref>,List<ClinicPref>>(_listClinicPrefDefaultOld);
			_listClinicPrefEnabledOld=ClinicPrefs.GetPrefAllClinics(PrefName.BirthdayPromotionsEnabled,true);//include default to handle 'no clinics'
			_listClinicPrefEnabledNew=GenericTools.DeepCopy<List<ClinicPref>,List<ClinicPref>>(_listClinicPrefEnabledOld);
			_isEnabled=PrefC.GetBool(PrefName.BirthdayPromotionsEnabled);
			_listClinicBirthdayRules=ApptReminderRules.GetForTypes(ApptReminderType.Birthday);
			List<long> listBirthdayTemplates=_listClinicBirthdayRules.Select(x => x.EmailHostingTemplateNum).ToList();
			_listTemplates=EmailHostingTemplates.GetMany(listBirthdayTemplates);
			#endregion
			#region Fill UI
			if(_arrLanguages.Length==0 
				|| (_arrLanguages.Length==1 && _arrLanguages[0].ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim())) 
			{
				butLangaugeAdd.Visible=false;
			}
			if(_listClinicBirthdayRules.FindAll(x => x.ClinicNum==0).Count==1) {//default to default clinic on load. 
				butLanguageRemove.Visible=false;
			}
			for(int i = 0;i < 22;i++) {
				comboMinorAge.Items.Add(i);
			}
			FillForClinic();
			#endregion
			if(!Security.IsAuthorized(Permissions.EServicesSetup,true)) {
				DisableAllExcept(butCancel);
			}
			_isLoading=false;
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			long previousClinicNum=((ApptReminderRule)tabControlLanguages.SelectedTab.Tag).ClinicNum;
			if(!_isLoading) {
				string errors=ValidateCurrentSelection();
				if(!string.IsNullOrEmpty(errors)) {//if there were problems, do not change clinics.
					comboClinic.SelectedClinicNum=previousClinicNum;
					MessageBox.Show(this,errors);
					return;
				}
				MapUiToRules(_listClinicBirthdayRules.First(x => x.ClinicNum==previousClinicNum && x.Language==""));
			}
			FillForClinic();
		}

		private string ValidateCurrentSelection() {
			//make sure that the current rule and template are in a state that could be saved before we move on to a different set. 
			StringBuilder error=new StringBuilder();
			if(!checkIsEnabled.Checked) {
				return "";//no need to validate disabled rules.
			}
			if(!textDays.IsValid() || Math.Abs(PIn.Int(textDays.Text,false))>364) {
				error.AppendLine(Lan.g(this,"Lead time must 364 days or less."));
			}
			else if(PIn.Int(textDays.Text,false)==0 && (radioAfterBirthday.Checked || radioBeforeBirthday.Checked)) {
				error.AppendLine(Lan.g(this,"Days cannot be set to 0 when not sending on the patient's birthday"));
			}
			//we also need to make sure the all of the language rules have the same timing information as their leader.If the leader gets updated, they all
			//need to be updated. I don't think there's code for that yet. 
			foreach(ApptReminderRule rule in _listClinicBirthdayRules.FindAll(x => x.ClinicNum==_effectiveClinicNum)) {
				EmailHostingTemplate template=_listTemplates.FirstOrDefault(x => rule.EmailHostingTemplateNum==x.EmailHostingTemplateNum);
				if(template==null) {
					error.AppendLine(Lan.g(this,"Rule is missing template. Please call support."));
				}
				if(string.IsNullOrEmpty(template.BodyPlainText)) {//this may be validated already in the template edit window.
					error.AppendLine(Lan.g(this,"Plain text field must be filled out"));
				}
				if(template.EmailTemplateType!=EmailType.Regular && string.IsNullOrEmpty(template.BodyHTML)) {
					error.AppendLine(Lan.g(this,"Html Text must be filled out, or email must be changed to a plain text version."));
				}
			}
			return error.ToString();
		}

		///<summary>Fills all necessary data for the clinic.</summary>
		private void FillForClinic() {
			labelNotActivated.Visible=!Clinics.IsMassEmailSignedUp(_effectiveClinicNum) && !Clinics.IsSecureEmailSignedUp(_effectiveClinicNum);
			checkUseDefaultsBirthday.Checked=comboClinic.SelectedClinicNum>0 && _isClinicUsingDefaults;
			checkIsEnabled.Checked=_isClinicEnabled;
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
			List<ApptReminderRule> listBirthdayRulesForClinic=_listClinicBirthdayRules
				.FindAll(x => x.ClinicNum==_effectiveClinicNum)
				.OrderByDescending(x => x.Language=="").ToList();
			//if we do want to send for a minor's birthday then the box should not be checked since it is labeled "Do NOT send..."
			checkBoxSendGuarantorBirthdayForMinor.Checked=!listBirthdayRulesForClinic.First().IsSendForMinorsBirthday;
			comboMinorAge.Enabled=checkBoxSendGuarantorBirthdayForMinor.Checked;
			comboMinorAge.SelectedIndex=listBirthdayRulesForClinic.First().MinorAge;
			textDays.Text=Math.Abs(listBirthdayRulesForClinic.First().TSPrior.Days).ToString();//Days, not total Days.
			if(listBirthdayRulesForClinic.First().TSPrior==TimeSpan.Zero) {
				radioOnBirthday.Checked=true;
				textDays.Text=PIn.String("0");
				textDays.Enabled=false;
			}
			else if(listBirthdayRulesForClinic.First().TSPrior > TimeSpan.Zero) {
				radioBeforeBirthday.Checked=true;
			}
			else{
				radioAfterBirthday.Checked=true;
			}
			tabControlLanguages.TabPages.Clear();
			foreach(ApptReminderRule rule in listBirthdayRulesForClinic) {
				EmailHostingTemplate template=_listTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==rule.EmailHostingTemplateNum);
				UserControlEmailTemplate control=new UserControlEmailTemplate();
				control.RefreshView(template.BodyPlainText,template.BodyHTML,template.EmailTemplateType);
				control.Dock=DockStyle.Fill;
				TabPage page=new TabPage();
				string language="";
				if(string.IsNullOrEmpty(rule.Language)) {
					language=Lan.g(this,"Default");
				}
				else {
					CultureInfo culture=MiscUtils.GetCultureFromThreeLetter(rule.Language);
					if(culture==null) {
						language=rule.Language;
					}
					else {
						language=culture.DisplayName;
					}
				}
				page.Text=language;
				page.Tag=rule;
				page.Controls.Add(control);
				LayoutManager.Add(page,tabControlLanguages);			
			}
			LayoutManager.LayoutControlBoundsAndFonts(tabControlLanguages);
			if(comboClinic.SelectedClinicNum > 0 && _isClinicUsingDefaults) {
				DisableAllExcept(butOK,butCancel,comboClinic,checkIsEnabled,checkUseDefaultsBirthday,tabControlLanguages);
			}
		}

		private void butLangaugeAdd_Click(object sender,EventArgs e) {
			List<string> listLanguagesDisplay=new List<string>();//contains the user friendly name ex: "Spanish"
			List<string> listLanguagesData=new List<string>();//contains the db friendly string ex: "SPA"
			List<string> listLanguageRulesCurrent=new List<string>();
			listLanguageRulesCurrent=_listClinicBirthdayRules.FindAll(x => x.ClinicNum==_effectiveClinicNum).Select(x => x.Language).ToList();
			foreach(string language in _arrLanguages) {
				if(language.IsNullOrEmpty() || language.ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim()) {
					continue;
				}
				if(ListTools.In(language,listLanguageRulesCurrent))	{
					continue;//we already have a tab for this langauge. 
				}
				CultureInfo culture=MiscUtils.GetCultureFromThreeLetter(language);
				if(culture==null) {
					listLanguagesDisplay.Add(language);//custom language - display what the user entered
				}
				else {
					listLanguagesDisplay.Add(culture.DisplayName);//display full name of the abbreviation
				}
				listLanguagesData.Add(language);
			}
			if(listLanguagesDisplay.Count==0) {
				MsgBox.Show(this,"No additional languages available.");
				return;
			}
			using InputBox languageSelect=new InputBox(Lan.g(this,"Select language for template: "),listLanguagesDisplay,0);
			languageSelect.ShowDialog();
			if(languageSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			ApptReminderRule rule=_listClinicBirthdayRules
				.FindAll(x => x.ClinicNum==_effectiveClinicNum)
				.First(x => x.Language=="");
			MapUiToRules(rule);//first update anything that may have changed for the default rule before adding the langauge.
			ApptReminderRule copiedRule=CopyRule(rule,rule.ClinicNum);
			copiedRule.Language=listLanguagesData[languageSelect.SelectedIndex];
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
			ApptReminderRule ruleRemoving=(ApptReminderRule)tabControlLanguages.SelectedTab.Tag;
			RemoveRules(ruleRemoving);
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
			ApptReminderRule ruleCur=(ApptReminderRule)tabControlLanguages.SelectedTab.Tag;
			MapUiToRules(ruleCur);//update anything about the rule that may have previously changed before editing.
			EmailHostingTemplate template=_listTemplates.First(x => ruleCur.EmailHostingTemplateNum==x.EmailHostingTemplateNum);
			//We do not want offices to delete Automated Birthday templates.
			//if a clinic decides they don't want their own Automated Birthday template, they don't delete the template, they should check the "Use Defaults" box to use the HQ rule.
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(template,canDeleteTemplate:false);
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
				else if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Mass Email is based on usage. " +
						"By activating and enabling this feature you are agreeing to the charges. Continue?")) 
				{
					checkIsEnabled.Checked=false;
					return;
				}
			}
			_isClinicEnabled=checkIsEnabled.Checked;
		}

		private void checkUseDefaultsBirthday_Click(object sender,EventArgs e) {
			if(!checkUseDefaultsBirthday.Checked && comboClinic.SelectedClinicNum>0) {
				SwitchFromDefaultsBirthday();//turning defaults off
			}
			else if(checkUseDefaultsBirthday.Checked && comboClinic.SelectedClinicNum>0) {
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
			_isClinicUsingDefaults=false;
			foreach(ApptReminderRule rule in _listClinicBirthdayRules.FindAll(x => x.ClinicNum==0)) {
				CopyRule(rule,comboClinic.SelectedClinicNum);
			}
			FillForClinic();
			return true;
		}

		private ApptReminderRule CopyRule(ApptReminderRule rule,long clinicNum) {
			EmailHostingTemplate templateForDefaultRule=_listTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==rule.EmailHostingTemplateNum);
			EmailHostingTemplate templateForClinicRule=GenericTools.DeepCopy<EmailHostingTemplate,EmailHostingTemplate>(templateForDefaultRule);
			templateForClinicRule.TemplateId=0;//So the api uploads this new template.
			templateForClinicRule.ClinicNum=clinicNum;
			EmailHostingTemplates.Insert(templateForClinicRule);
			_listTemplates.Add(templateForClinicRule);
			ApptReminderRule copiedRule=GenericTools.DeepCopy<ApptReminderRule,ApptReminderRule>(rule);
			copiedRule.ApptReminderRuleNum=0;//ApptReminderRules.Sync() expects new rules to have 0 pk
			copiedRule.ClinicNum=clinicNum;
			copiedRule.EmailHostingTemplateNum=templateForClinicRule.EmailHostingTemplateNum;
			_listClinicBirthdayRules.Add(copiedRule);
			return copiedRule;
		}

		private void RemoveRules(params ApptReminderRule[] arrRules) {
			foreach(ApptReminderRule rule in arrRules) {
				_listClinicBirthdayRules.Remove(rule);
			}
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchToDefaultsBirthday() {
			if(comboClinic.SelectedClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			if(_listClinicBirthdayRules.FindAll(x => x.ClinicNum==comboClinic.SelectedClinicNum).Count>0 && 
				!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete custom rules for this clinic and switch to using defaults? This cannot be undone.")) 
			{
				checkUseDefaultsBirthday.Checked=false;//undo checking of box.
				return false;
			}
			RemoveRules(_listClinicBirthdayRules.Where(x => x.ClinicNum==comboClinic.SelectedClinicNum).ToArray());
			//The corresponding EmailHostingTemplates will be deleted at save/sync
			_isClinicUsingDefaults=true;
			FillForClinic();
			return true;
		}

		///<summary>This will update the passed in rule based on the current UI state. It will only save if the user ends up clicking ok.
		///This should be called with the default rule passed in (no language set).</summary>
		private void MapUiToRules(ApptReminderRule rule) {
			if(rule==null) {
				return;//this may happen in some cases when the defaults are being used, in which case, it is okay to just return. No changes to be saved.
			}
			if(radioBeforeBirthday.Checked) {
				rule.TSPrior=new TimeSpan(PIn.Int(textDays.Text,false),0,0,0);
			}
			else {
				rule.TSPrior=new TimeSpan(-PIn.Int(textDays.Text,false),0,0,0);
			}
			//if we do want to send for a minors bithday then the box will NOT be checked
			rule.IsSendForMinorsBirthday=!checkBoxSendGuarantorBirthdayForMinor.Checked;
			rule.MinorAge=PIn.Int(comboMinorAge.SelectedIndex.ToString());//combo box is 1:1 with age from 0 to 21
			foreach(ApptReminderRule langRule in _listClinicBirthdayRules.FindAll(x => x.ClinicNum==rule.ClinicNum && x.Language!="").ToList()) {
				langRule.TSPrior=rule.TSPrior;
				langRule.IsSendForMinorsBirthday=rule.IsSendForMinorsBirthday;
				langRule.MinorAge=rule.MinorAge;
			}
		}

		private void SaveAll() {
			//All UI elements will have been previously saved when switching clinics, the only one that might have not been is the 
			//current, if clinics are off or if we only made changes the the one that loaded in. 
			MapUiToRules(_listClinicBirthdayRules.FindAll(x => x.ClinicNum==_effectiveClinicNum).First(x => x.Language==""));
			_doSetInvalidPrefs|=Prefs.UpdateBool(PrefName.BirthdayPromotionsEnabled,_isEnabled);
			if(PrefC.HasClinicsEnabled) {
				_doSetInvalidClinicPrefs|=ClinicPrefs.Sync(_listClinicPrefDefaultNew,_listClinicPrefDefaultOld);
				_doSetInvalidClinicPrefs|=ClinicPrefs.Sync(_listClinicPrefEnabledNew,_listClinicPrefEnabledOld);
				foreach(Clinic clinic in comboClinic.ListClinics) {
					ApptReminderRules.SyncByClinicAndTypes(_listClinicBirthdayRules.FindAll(x => x.ClinicNum==clinic.ClinicNum),clinic.ClinicNum,ApptReminderType.Birthday);
				}
			}
			else {
				ApptReminderRules.SyncByClinicAndTypes(_listClinicBirthdayRules.FindAll(x => x.ClinicNum==0),0,ApptReminderType.Birthday);
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