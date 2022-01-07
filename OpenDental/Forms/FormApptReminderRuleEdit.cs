using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;

namespace OpenDental {
	///<summary>In-memory form. Changes are not saved to the DB from this form.</summary>
	public partial class FormApptReminderRuleEdit:FormODBase {
		///<summary>The default appointment reminder rule</summary>
		public ApptReminderRule ApptReminderRuleCur;
		///<summary>Do not edit this. A copy of the passed in default reminder rule.</summary>
		public readonly ApptReminderRule ApptReminderRuleOld;
		///<summary>True if any preferences were updated.</summary>
		public bool IsPrefsChanged;
		private List<CommType> _sendOrder;
		private List<ApptReminderRule> _listRulesClinic;
		/// <summary>Public so it can be passed back to the parent form. The list of new language rules that were added from this window. </summary>
		public List<ApptReminderRule> ListNonDefaultRulesAdded=new List<ApptReminderRule>();
		///<summary>A list to keep track of the loosely associated rules that get removed in this form so they can get sync'd properly.</summary>
		public List<ApptReminderRule> ListNonDefaultRulesRemoving=new List<ApptReminderRule>();
		///<summary>Do not edit this. A copied list of the passed in list rules for clinic.</summary>
		public readonly List<ApptReminderRule> ListRulesClinicOld=new List<ApptReminderRule>();
		///<summary>The control that handles the message templates for the default language.</summary>
		private UserControlReminderMessage _userControlReminderMessageDefault;

		public FormApptReminderRuleEdit(ApptReminderRule apptReminderCur,List<ApptReminderRule> listRulesClinic = null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ApptReminderRuleCur=apptReminderCur;
			ApptReminderRuleOld=GenericTools.DeepCopy<ApptReminderRule,ApptReminderRule>(apptReminderCur);
			if(listRulesClinic is null) {
				_listRulesClinic=new List<ApptReminderRule>();
			}
			else {
				_listRulesClinic=listRulesClinic;
			}
			ListRulesClinicOld=ListTools.DeepCopy<ApptReminderRule,ApptReminderRule>(listRulesClinic);
		}

		///<summary>A list that holds associated language rules for this rule.</summary>
		public List<ApptReminderRule> GetListLanguageRules() {
			return _listRulesClinic.FindAll(x => x.TSPrior==ApptReminderRuleCur.TSPrior
				&& x.ClinicNum==ApptReminderRuleCur.ClinicNum
				&& x.TypeCur==ApptReminderRuleCur.TypeCur
				&& x.Language!="");
		}

		private void FormApptReminderRuleEdit_Load(object sender,EventArgs e) {
			Text=Lan.g(this,"Edit")+" "+ApptReminderRuleCur.TypeCur.GetDescription(true)+" "+Lan.g(this,"Rule");
			checkEnabled.Checked=ApptReminderRuleCur.IsEnabled;
			checkEConfirmationAutoReplies.Checked=ApptReminderRuleCur.IsAutoReplyEnabled;
			labelRuleType.Text=ApptReminderRuleCur.TypeCur.GetDescription();
			labelTags.Text=Lan.g(this,"Use the following replacement tags to customize messages : ")
				+string.Join(", ",ApptReminderRules.GetAvailableTags(ApptReminderRuleCur.TypeCur));
			if(ListTools.In(ApptReminderRuleCur.TypeCur,ApptReminderType.PatientPortalInvite,ApptReminderType.Arrival)) {
				checkSendAll.Visible=false;
			}
			if(!ListTools.In(ApptReminderRuleCur.TypeCur,ApptReminderType.ConfirmationFutureDay,ApptReminderType.Arrival)) {
				checkEConfirmationAutoReplies.Visible=false;
			}
			//case where ArrLanguages is empty or is unspecified, hide the add language button
			string[] languagesUsedByPatients=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');
			if(languagesUsedByPatients.Length==0) {
				butLanguage.Visible=false;
			}
			if(languagesUsedByPatients.Length==1) {
				if(languagesUsedByPatients[0].ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim()) {
					butLanguage.Visible=false;
				}
			}
			if(GetListLanguageRules().Count==0) {
				butRemove.Visible=false;
			}
			_sendOrder=ApptReminderRuleCur.SendOrder.Split(',').Select(x => (CommType)PIn.Int(x)).ToList();
			FillGridPriority();
			FillTimeSpan();
			FillTabs();
			checkSendAll.Checked=ApptReminderRuleCur.IsSendAll;
		}
		
		private void FillTabs() {
			_userControlReminderMessageDefault=new UserControlReminderMessage(ApptReminderRuleCur,LayoutManager);
			_userControlReminderMessageDefault.Anchor=((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom);
			List<ApptReminderRule> listLanguageRules=GetListLanguageRules();
			if(butLanguage.Visible==false && listLanguageRules.Count==0) {
				//not using languages and no reminders have been added for this default. 
				tabControl.Visible=false;
				_userControlReminderMessageDefault.Location=new Point(LayoutManager.Scale(18),LayoutManager.Scale(264));
				LayoutManager.AddUnscaled(_userControlReminderMessageDefault,PanelClient);
				LayoutManager.MoveSize(_userControlReminderMessageDefault,LayoutManager.ScaleSize(_userControlReminderMessageDefault.Size));
				return;
			}
			//using languages
			_userControlReminderMessageDefault.Dock=DockStyle.Fill;
			tabPageDefault.Tag=ApptReminderRuleCur;
			LayoutManager.AddUnscaled(_userControlReminderMessageDefault,tabPageDefault);
			for(int i = 0; i < listLanguageRules.Count; i++) {
				TabPage languageTab=new TabPage();
				CultureInfo culture=MiscUtils.GetCultureFromThreeLetter(listLanguageRules[i].Language);
				if(culture==null) {
					languageTab.Text=listLanguageRules[i].Language;
				}
				else {
					languageTab.Text=culture.DisplayName;
				}
				languageTab.Tag=listLanguageRules[i];
				LayoutManager.Add(languageTab,tabControl);
				UserControlReminderMessage userControlReminderMessageLang=new UserControlReminderMessage(listLanguageRules[i],LayoutManager);
				userControlReminderMessageLang.Anchor=_userControlReminderMessageDefault.Anchor;
				userControlReminderMessageLang.Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(userControlReminderMessageLang,languageTab);
			}
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void FillTimeSpan() {
			textHours.Text=Math.Abs(ApptReminderRuleCur.TSPrior.Hours).ToString();//Hours, not total hours.
			textDays.Text=Math.Abs(ApptReminderRuleCur.TSPrior.Days).ToString();//Days, not total Days.
			if(ApptReminderRuleCur.TSPrior>=TimeSpan.Zero) {
				radioBeforeAppt.Checked=true;
			}
			else {
				radioAfterAppt.Checked=true;
			}
			if(ApptReminderRuleCur.TypeCur!=ApptReminderType.PatientPortalInvite) {
				radioBeforeAppt.Visible=false;
				radioAfterAppt.Visible=false;
			}
			switch(ApptReminderRuleCur.TypeCur) {
				case ApptReminderType.Arrival:
				case ApptReminderType.Reminder:
				case ApptReminderType.ConfirmationFutureDay:
					groupSendTime.Text=Lan.g(this,"Send Time - before appointment.");
					break;
				case ApptReminderType.ScheduleThankYou:
					groupSendTime.Text=Lan.g(this,"Send Time - after appointment scheduled.");
					break;
				case ApptReminderType.GeneralMessage:
					groupSendTime.Text=Lan.g(this,"Send Time - after appointment.");
					break;
				//Patient Portal Invite will just say "Send Time" since customers can select before / after appointment.
			}
			if(ApptReminderRuleCur.DoNotSendWithin.Days > 0) {
				textDaysWithin.Text=ApptReminderRuleCur.DoNotSendWithin.Days.ToString();
			}
			if(ApptReminderRuleCur.DoNotSendWithin.Hours > 0) {
				textHoursWithin.Text=ApptReminderRuleCur.DoNotSendWithin.Hours.ToString();
			}
			UpdateDoNotSendWithinLabel();
		}

		private void FillGridPriority() {
			gridPriorities.BeginUpdate();
			gridPriorities.ListGridColumns.Clear();
			gridPriorities.ListGridColumns.Add(new GridColumn("",50){ IsWidthDynamic=true });
			gridPriorities.ListGridRows.Clear();
			for(int i = 0;i < _sendOrder.Count; i++) {
				CommType typeCur = _sendOrder[i];
				GridRow gridRow;
				if(typeCur==CommType.Preferred) {
					if(checkSendAll.Checked) {
						//"Preferred" is irrelevant when SendAll is checked.
						continue;
					}
					gridRow=new GridRow();
					gridRow.Cells.Add(Lan.g(this,"Preferred Confirm Method"));
					gridPriorities.ListGridRows.Add(gridRow);
					continue;
				}
				if(typeCur==CommType.Text && !SmsPhones.IsIntegratedTextingEnabled()) {
					gridRow=new GridRow();
					gridRow.Cells.Add(Lan.g(this,typeCur.ToString())+" ("+Lan.g(this,"Not Configured")+")");
					gridRow.ColorBackG=Color.LightGray;
					gridPriorities.ListGridRows.Add(gridRow);
				}
				else {
					gridRow=new GridRow();
					gridRow.Cells.Add(Lan.g(this,typeCur.ToString()));
					gridPriorities.ListGridRows.Add(gridRow);
				}
			}
			gridPriorities.EndUpdate();
		}

		private void radioBeforeAfterAppt_CheckedChanged(object sender,EventArgs e) {
			bool enabled=radioBeforeAppt.Checked || ApptReminderRuleCur.TypeCur==ApptReminderType.ScheduleThankYou;
			labelDoNotSendWithin.Enabled=enabled;
			labelDaysWithin.Enabled=enabled;
			labelHoursWithin.Enabled=enabled;
			textDaysWithin.Enabled=enabled;
			textHoursWithin.Enabled=enabled;
		}

		private void textDoNotSendWithin_TextChanged(object sender,EventArgs e) {
			UpdateDoNotSendWithinLabel();
		}

		private void UpdateDoNotSendWithinLabel() {
			string daysHoursTxt="";
			int daysWithin=PIn.Int(textDaysWithin.Text,false);
			int hoursWithin=PIn.Int(textHoursWithin.Text,false);
			if(!textDaysWithin.IsValid() || !textHoursWithin.IsValid()
				|| (daysWithin==0 && hoursWithin==0)) 
			{
				daysHoursTxt="_____________";
			}
			else {
				if(daysWithin==1) {
					daysHoursTxt+=daysWithin+" "+Lans.g(this,"day");
				}
				else if(daysWithin > 1) {
					daysHoursTxt+=daysWithin+" "+Lans.g(this,"days");
				}
				if(daysWithin > 0 && hoursWithin > 0) {
					daysHoursTxt+=" ";
				}
				if(hoursWithin==1) {
					daysHoursTxt+=hoursWithin+" "+Lans.g(this,"hour");
				}
				else if(hoursWithin > 1) {
					daysHoursTxt+=hoursWithin+" "+Lans.g(this,"hours");
				}
			}
			labelDoNotSendWithin.Text=Lans.g(this,"Do not send within")+" "+daysHoursTxt+" "+Lans.g(this,"of appointment");
		}

		private void butUp_Click(object sender,EventArgs e) {
			int idx = gridPriorities.GetSelectedIndex();
			if(idx<1) {
				//-1 if nothing selected. 0 if top item selected.
				return;
			}
			_sendOrder.Reverse(idx-1,2);
			FillGridPriority();
			gridPriorities.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int idx = gridPriorities.GetSelectedIndex();
			if(idx==-1 || idx==_sendOrder.Count-1) {
				//-1 nothing selected. Count-1 if last item selected.
				return;
			}
			_sendOrder.Reverse(idx,2);
			FillGridPriority();
			gridPriorities.SetSelected(idx+1,true);
		}

		private void butAdvanced_Click(object sender,EventArgs e) {
			List<ApptReminderRule> listRulesOldAndNew=new List<ApptReminderRule>();
			listRulesOldAndNew.AddRange(GetListLanguageRules());
			listRulesOldAndNew.AddRange(ListNonDefaultRulesAdded);
			string selectedLanguage="";
			if(tabControl.TabPages.Count > 1) {
				selectedLanguage=((ApptReminderRule)tabControl.SelectedTab.Tag).Language;
			}
			using FormApptReminderRuleAggEdit formAddEdit=new FormApptReminderRuleAggEdit(ApptReminderRuleCur,listRulesOldAndNew,selectedLanguage);
			formAddEdit.ShowDialog();
		}

		///<summary>Removes 'Do not send eConfirmations' from the confirmed status for 'eConfirm Sent' if multiple eConfirmations are set up.</summary>
		private void CheckMultipleEConfirms() {
			int countEConfirm=_listRulesClinic?.Count(x => x.TypeCur==ApptReminderType.ConfirmationFutureDay && x.Language=="")??0;
			string confStatusEConfirmSent=Defs.GetDef(DefCat.ApptConfirmed,PrefC.GetLong(PrefName.ApptEConfirmStatusSent)).ItemName;
			List<string> listExclude=PrefC.GetString(PrefName.ApptConfirmExcludeESend)
				.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			if(ApptReminderRuleCur.TypeCur==ApptReminderType.ConfirmationFutureDay
				//And there is more than 1 eConfirmation rule.
				&& (countEConfirm > 1 || (countEConfirm==1 && ApptReminderRuleCur.ApptReminderRuleNum==0))
				//And the confirmed status for 'eConfirm Sent' is marked 'Do not send eConfirmations'
				&& listExclude.Contains(PrefC.GetString(PrefName.ApptEConfirmStatusSent))
				//Ask them to fix their exclude send statuses
				&& MessageBox.Show(Lans.g(this,"Appointments will not receive multiple eConfirmations if the '")+confStatusEConfirmSent+"' "+
						Lans.g(this,"status is set as 'Don't Send'. Would you like to remove 'Don't Send' from that status?"),
					"",MessageBoxButtons.YesNo)==DialogResult.Yes) 
			{
				listExclude.RemoveAll(x => x==PrefC.GetString(PrefName.ApptEConfirmStatusSent));
				IsPrefsChanged|=Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,string.Join(",",listExclude));
			}
		}

		private void butLanguage_Click(object sender,EventArgs e) {
			List<string> listLanguagesDisplay=new List<string>();//contains the user friendly name ex: "Spanish"
			List<string> listLanguagesData=new List<string>();//contains the db friendly string ex: "SPA"
			List<ApptReminderRule> listCurrentControls=new List<ApptReminderRule>();
			for( int i=0; i<tabControl.TabPages.Count;i++) {
				listCurrentControls.Add((ApptReminderRule)tabControl.TabPages[i].Tag);
			}
			string[] languagesUsedbyPatients=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');
			for(int i = 0; i < languagesUsedbyPatients.Length; i++) {
				if(languagesUsedbyPatients[i].IsNullOrEmpty() || languagesUsedbyPatients[i].ToLower().Trim()==PrefC.GetString(PrefName.LanguagesIndicateNone).ToLower().Trim()) {
					continue;
				}
				if(ListTools.In(languagesUsedbyPatients[i],listCurrentControls.Select(x => x.Language).ToList()))	{
					continue;//we already have a tab for this langauge. 
				}
				CultureInfo culture=MiscUtils.GetCultureFromThreeLetter(languagesUsedbyPatients[i]);
				if(culture==null) {
					listLanguagesDisplay.Add(languagesUsedbyPatients[i]);//custom language - display what the user entered
				}
				else {
					listLanguagesDisplay.Add(culture.DisplayName);//display full name of the abbreviation
				}
				listLanguagesData.Add(languagesUsedbyPatients[i]);
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
			ApptReminderRule rule=GenericTools.DeepCopy<ApptReminderRule,ApptReminderRule>(ApptReminderRuleCur);
			rule.ApptReminderRuleNum=0;
			rule.Language=listLanguagesData[languageSelect.SelectedIndex];
			TabPage tabLang=new TabPage();
			tabLang.Tag=rule;
			tabLang.Text=listLanguagesDisplay[languageSelect.SelectedIndex];
			LayoutManager.Add(tabLang,tabControl);
			UserControlReminderMessage ruleControl=new UserControlReminderMessage(rule,LayoutManager);
			ruleControl.Anchor=tabPageDefault.Controls[0].Anchor;
			ruleControl.Dock=DockStyle.Fill;
			LayoutManager.AddUnscaled(ruleControl,tabLang);
			tabControl.SelectedTab=tabLang;
			ListNonDefaultRulesAdded.Add(rule);
			butRemove.Visible=true;
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void butRemove_Click(object sender,EventArgs e) {
			//Don't delete the default.
			if(((ApptReminderRule)tabControl.SelectedTab.Tag).Language=="") {
				MsgBox.Show(this,"Cannot remove the default template.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete the currently selected language?")) {
				return;
			}
			ApptReminderRule ruleRemoving=(ApptReminderRule)tabControl.SelectedTab.Tag;
			if(!ListNonDefaultRulesAdded.Contains(ruleRemoving)) {
				ListNonDefaultRulesRemoving.Add(ruleRemoving);
			}
			ListNonDefaultRulesAdded.Remove(ruleRemoving);
			tabControl.TabPages.Remove(tabControl.SelectedTab);
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(!textHours.IsValid()	|| !textDays.IsValid() || !textHoursWithin.IsValid() || !textDaysWithin.IsValid()) {
				MsgBox.Show(this,"Fix data entry errors first.");
				return;
			}
			if(!ValidateRule()) {
				return;
			}
			TimeSpan timeSpanPrior= new TimeSpan(PIn.Int(textDays.Text,false),PIn.Int(textHours.Text,false),0,0);
			if(!radioBeforeAppt.Checked) {
				timeSpanPrior=timeSpanPrior.Negate();
			}
			else if(ApptReminderRuleCur.TypeCur==ApptReminderType.ScheduleThankYou) {
				timeSpanPrior=timeSpanPrior.Negate();
			}
			if(_listRulesClinic.Any(x => x.TypeCur!=ApptReminderRuleCur.TypeCur && x.TSPrior==timeSpanPrior && x.IsEnabled)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"There are multiple rules for sending at this send time. Are you sure you want to send multiple "
				+"messages at the same time?")) 
				{
					return;
				}
			}
			if(_listRulesClinic.FindAll(
				x => x.TSPrior==timeSpanPrior 
				&& x.ClinicNum==ApptReminderRuleCur.ClinicNum
				&& x.TypeCur==ApptReminderRuleCur.TypeCur
				&& x.ApptReminderRuleNum!=ApptReminderRuleCur.ApptReminderRuleNum 
				&& x.Language=="").Count>0) 
			{
				MsgBox.Show(this,"Not allowed to create a duplicate rule for the same send time.");
				return;
			}
			CheckMultipleEConfirms();
			ApptReminderRuleCur.SendOrder=string.Join(",",_sendOrder.Select(x => ((int)x).ToString()).ToArray());
			ApptReminderRuleCur.IsSendAll=checkSendAll.Checked;
			ApptReminderRuleCur.TSPrior=timeSpanPrior;
			if(radioBeforeAppt.Checked || ApptReminderRuleCur.TypeCur==ApptReminderType.ScheduleThankYou) {
				ApptReminderRuleCur.DoNotSendWithin=new TimeSpan(PIn.Int(textDaysWithin.Text,false),PIn.Int(textHoursWithin.Text,false),0,0);
			}
			ApptReminderRuleCur.IsEnabled=checkEnabled.Checked;
			ApptReminderRuleCur.IsAutoReplyEnabled=checkEConfirmationAutoReplies.Checked;
			_userControlReminderMessageDefault.SaveControlTemplates();
			if(tabControl.TabPages.Count>1) {//handle additional language rules
				for(int i = 0; i < tabControl.TabPages.Count; i++) {
					UserControlReminderMessage reminderControl=(UserControlReminderMessage)tabControl.TabPages[i].Controls[0];//update the additional languages to match
					if(reminderControl.Rule.Language=="") {
						continue;//this is the default, which we already took care of.
					}
					reminderControl.Rule.TSPrior=ApptReminderRuleCur.TSPrior;
					reminderControl.Rule.IsAutoReplyEnabled=ApptReminderRuleCur.IsAutoReplyEnabled;
					reminderControl.Rule.IsEnabled=ApptReminderRuleCur.IsEnabled;
					reminderControl.Rule.SendOrder=ApptReminderRuleCur.SendOrder;
					reminderControl.Rule.IsSendAll=ApptReminderRuleCur.IsSendAll;
					reminderControl.Rule.DoNotSendWithin=ApptReminderRuleCur.DoNotSendWithin;
					reminderControl.SaveControlTemplates();
				}
			}
			DialogResult=DialogResult.OK;
		}

		private bool ValidateRule() {
			if(!checkEnabled.Checked) {
				return true;
			}
			List<string> errors = new List<string>();
			if(butLanguage.Visible==false) {
				errors.AddRange(_userControlReminderMessageDefault.ValidateTemplates());
			}
			else{ 
				foreach(TabPage page in tabControl.TabPages) {
					errors.AddRange(((UserControlReminderMessage)page.Controls[0]).ValidateTemplates());
				}
			}
			if(PIn.Int(textDays.Text,false)>=366) {
				errors.Add(Lan.g(this,"Lead time must 365 days or less."));
			}
			//ScheduleThankYou and GeneralMessage can be 0, meaning send immediately.
			if(checkEnabled.Checked && PIn.Int(textHours.Text,false)==0 
				&& PIn.Int(textDays.Text,false)==0 
				&& !ListTools.In(ApptReminderRuleCur.TypeCur,ApptReminderType.ScheduleThankYou,ApptReminderType.GeneralMessage))
			{
				errors.Add(Lan.g(this,"Lead time must be greater than 0 hours."));
			}
			if(ApptReminderRuleCur.TypeCur==ApptReminderType.ConfirmationFutureDay) {
				if(PIn.Int(textDays.Text,false)==0) {
					errors.Add(Lan.g(this,"Lead time must 1 day or more for confirmations."));
				}
			}
			if(radioBeforeAppt.Checked) {
				TimeSpan tsPrior=new TimeSpan(PIn.Int(textDays.Text,false),PIn.Int(textHours.Text,false),0,0);
				TimeSpan doNotSendWithin=new TimeSpan(PIn.Int(textDaysWithin.Text,false),PIn.Int(textHoursWithin.Text,false),0,0);
				if(doNotSendWithin >= tsPrior && ApptReminderRuleCur.TypeCur!=ApptReminderType.ScheduleThankYou) {
					errors.Add(Lan.g(this,"'Send Time' must be greater than 'Do Not Send Within' time."));
				}
			}
			if(errors.Count>0) {
				MessageBox.Show(Lan.g(this,"You must fix the following errors before continuing.")+"\r\n\r\n-"+string.Join("\r\n-",errors));
				return false;
			}
			return true;
		}

		private void checkSendAll_CheckedChanged(object sender,EventArgs e) {
			butUp.Enabled=!checkSendAll.Checked;
			butDown.Enabled=!checkSendAll.Checked;
			gridPriorities.Enabled=!checkSendAll.Checked;
			gridPriorities.SetAll(false);
			FillGridPriority();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ApptReminderRuleCur.IsAutoReplyEnabled 
				&& ConfirmationRequests.GetPendingForRule(ApptReminderRuleCur.ApptReminderRuleNum).Count > 0
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Outstanding confirmation text messages associated to this appointment rule were found.  " +
					"Auto reply text messages will no longer be sent.  Continue?")) 
			{
				return;
			}
			List<ApptReminderRule> listLanguageRules=GetListLanguageRules();
			for(int i = 0; i < listLanguageRules.Count; i++) {
				if(!ListNonDefaultRulesRemoving.Contains(listLanguageRules[i]) && !ListNonDefaultRulesAdded.Contains(listLanguageRules[i])) {
					ListNonDefaultRulesRemoving.Add(listLanguageRules[i]);
				}
			}
			ListNonDefaultRulesAdded.Clear();
			ApptReminderRuleCur=null;
			DialogResult=DialogResult.OK;
		}
	}
}
