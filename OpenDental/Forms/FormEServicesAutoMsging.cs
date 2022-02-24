using CodeBase;
using Microsoft.Win32;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Linq;
using System.IO;
using WebServiceSerializer;
using OpenDentBusiness.WebServiceMainHQ;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDental {

	public partial class FormEServicesAutoMsging:FormODBase {
		//==================== eConfirm & eRemind Variables ====================
		private List<Def> _listDefsApptStatus;
		private List<Clinic> _ecListClinics;
		private Clinic _ecClinicCur;
		///<summary>When using clinics, this is the index of the clinic rules to use.</summary>
		///<summary>not acutal idx, actually just ClinicNum.</summary>
		private long _clinicRuleClinicNum;
		///<summary>Key = ClinicNum, 0=Practice/Defaults. Value = Rules defined for that clinic. If a clinic uses defaults, its respective list of rules will be empty.</summary>
		private Dictionary<long,List<ApptReminderRule>> _dictClinicRules;
		private List<ClinicPref> _listThankYouTitlesLoaded;
		private ApptReminderType[] _arrApptReminderTypes=new ApptReminderType[] { ApptReminderType.Reminder
			,ApptReminderType.ConfirmationFutureDay,ApptReminderType.ScheduleThankYou,ApptReminderType.Arrival };
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

		private List<ClinicPref> _listThankYouTitles {
			get {
				//On load only, load all ApptThankYouCalendarTitle ClinicPrefs, including a pseudo 0 ClinicPref.
				_listThankYouTitlesLoaded=_listThankYouTitlesLoaded??ClinicPrefs.GetPrefAllClinics(PrefName.ApptThankYouCalendarTitle,includeDefault:true);
				return _listThankYouTitlesLoaded;
			}
		}
		
		public FormEServicesAutoMsging(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesECR_Load(object sender,EventArgs e) {
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			FillECRActivationButtons();
			checkEnableNoClinic.Checked=PrefC.GetBool(PrefName.ApptConfirmEnableForClinicZero);
			if(PrefC.HasClinicsEnabled) {//CLINICS
				checkUseDefaultsEC.Visible=true;
				checkUseDefaultsEC.Enabled=false;//when loading form we will be viewing defaults.
				checkIsConfirmEnabled.Visible=true;
				groupAutomationStatuses.Text=Lan.g(this,"eConfirmation Settings")+" - "+Lan.g(this,"Affects all Clinics");
			}
			else {//NO CLINICS
				checkUseDefaultsEC.Visible=false;
				checkUseDefaultsEC.Enabled=false;
				checkUseDefaultsEC.Checked=false;
				checkIsConfirmEnabled.Visible=false;
				checkEnableNoClinic.Visible=false;
				groupAutomationStatuses.Text=Lan.g(this,"eConfirmation Settings");
			}
			setListClinicsAndDictRulesHelper();
			comboClinicEConfirm.SelectedIndex=0;
			_listDefsApptStatus=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			comboStatusESent.Items.Clear();
			comboStatusEAccepted.Items.Clear();
			comboStatusEDeclined.Items.Clear();
			comboStatusEFailed.Items.Clear();
			comboStatusESent.Items.AddDefs(_listDefsApptStatus);
			comboStatusEAccepted.Items.AddDefs(_listDefsApptStatus);
			comboStatusEDeclined.Items.AddDefs(_listDefsApptStatus);
			comboStatusEFailed.Items.AddDefs(_listDefsApptStatus);
			long prefApptEConfirmStatusSent=PrefC.GetLong(PrefName.ApptEConfirmStatusSent);
			long prefApptEConfirmStatusAccepted=PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted);
			long prefApptEConfirmStatusDeclined=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined);
			long prefApptEConfirmStatusSendFailed=PrefC.GetLong(PrefName.ApptEConfirmStatusSendFailed);
			//SENT
			if(prefApptEConfirmStatusSent>0) {
				//Selects combo box option if it exists, if it doesn't it sets the text of the combo box to the hidden one.
				comboStatusESent.SetSelectedDefNum(prefApptEConfirmStatusSent);
			}
			else {
				comboStatusESent.SelectedIndex=0;
			}
			//CONFIRMED
			if(prefApptEConfirmStatusAccepted>0) {
				comboStatusEAccepted.SetSelectedDefNum(prefApptEConfirmStatusAccepted);
			}
			else {
				comboStatusEAccepted.SelectedIndex=0;
			}
			//NOT CONFIRMED
			if(prefApptEConfirmStatusDeclined>0) {
				comboStatusEDeclined.SetSelectedDefNum(prefApptEConfirmStatusDeclined);
			}
			else {
				comboStatusEDeclined.SelectedIndex=0;
			}
			//Failed
			if(prefApptEConfirmStatusSendFailed>0) {
				comboStatusEFailed.SetSelectedDefNum(prefApptEConfirmStatusSendFailed);
			}
			else {
				comboStatusEFailed.SelectedIndex=0;
			}
			if(PrefC.GetBool(PrefName.ApptEConfirm2ClickConfirmation)) {
				radio2ClickConfirm.Checked=true;
			}
			else {
				radio1ClickConfirm.Checked=true;
			}
			FillConfStatusesGrid();
			FillRemindConfirmData();
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			AuthorizeECR(allowEdit);
		}

		private void SaveTabECR() {
			if(comboStatusESent.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.ApptEConfirmStatusSent,_listDefsApptStatus[comboStatusESent.SelectedIndex].DefNum);
			}
			if(comboStatusEAccepted.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.ApptEConfirmStatusAccepted,_listDefsApptStatus[comboStatusEAccepted.SelectedIndex].DefNum);
			}
			if(comboStatusEDeclined.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.ApptEConfirmStatusDeclined,_listDefsApptStatus[comboStatusEDeclined.SelectedIndex].DefNum);
			}
			if(comboStatusEFailed.SelectedIndex!=-1) {
				Prefs.UpdateLong(PrefName.ApptEConfirmStatusSendFailed,_listDefsApptStatus[comboStatusEFailed.SelectedIndex].DefNum);
			}
			Prefs.UpdateBool(PrefName.ApptConfirmEnableForClinicZero,checkEnableNoClinic.Checked);
			Prefs.UpdateBool(PrefName.ApptEConfirm2ClickConfirmation,radio2ClickConfirm.Checked);
			ApptReminderRules.SyncByClinicAndTypes(_dictClinicRules[_ecClinicCur.ClinicNum],_ecClinicCur.ClinicNum,_arrApptReminderTypes);
			if(_ecClinicCur!=null&&_ecClinicCur.ClinicNum!=0) {
				_ecClinicCur.IsConfirmEnabled=checkIsConfirmEnabled.Checked;
				Clinics.Update(_ecClinicCur);
			}
			#region ApptThankYou Titles
			ParseThankYouTitle();
			bool changedPrefs=false;
			bool changedClinicPrefs=false;
			foreach(ClinicPref clinicPref in _listThankYouTitles) {
				if(clinicPref.ClinicNum!=0) {
					changedClinicPrefs |= ClinicPrefs.Upsert(clinicPref.PrefName,clinicPref.ClinicNum,clinicPref.ValueString);
				}
				else {
					changedPrefs |= Prefs.UpdateString(clinicPref.PrefName,clinicPref.ValueString);
				}
			}
			if(changedClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			if(changedPrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);  
			}
			#endregion
		}

		private void AuthorizeECR(bool allowEdit) {
			groupAutomationStatuses.Enabled=allowEdit;
			butActivateReminder.Enabled=allowEdit;
			butActivateConfirm.Enabled=allowEdit;
			butActivateThanks.Enabled=allowEdit;
			butActivateArrivals.Enabled=allowEdit;
			checkIsConfirmEnabled.Enabled=allowEdit;
			checkUseDefaultsEC.Enabled=allowEdit;
		}

		///<summary>Fills in memory Rules dictionary and clinics list based. This is very different from AppointmentReminderRules.GetRuleAndClinics.</summary>
		private void setListClinicsAndDictRulesHelper() {
			if(PrefC.HasClinicsEnabled) {//CLINICS
				_ecListClinics=new List<Clinic>() { new Clinic() { Description="Defaults",Abbr="Defaults" } };
				_ecListClinics.AddRange(Clinics.GetForUserod(Security.CurUser));
			}
			else {//NO CLINICS
				_ecListClinics=new List<Clinic>() { new Clinic() { Description="Practice",Abbr="Practice" } };
			}
			List<ApptReminderRule> listRulesTemp=ApptReminderRules.GetForTypes(_arrApptReminderTypes);
			_dictClinicRules=_ecListClinics.Select(x => x.ClinicNum).ToDictionary(x => x,x => listRulesTemp.FindAll(y => y.ClinicNum==x));
			int idx = comboClinicEConfirm.SelectedIndex>0 ? comboClinicEConfirm.SelectedIndex : 0;
			comboClinicEConfirm.BeginUpdate();
			comboClinicEConfirm.Items.Clear();
			_ecListClinics.ForEach(x => comboClinicEConfirm.Items.Add(x.Abbr));//combo clinics may not be visible.
			if(idx>-1&&idx<comboClinicEConfirm.Items.Count) {
				//textThankYouTitle needs to be filled before selecting an index in comboClinicEConfirm so the initial value is not overwritten.
				textThankYouTitle.Text=_listThankYouTitles.FirstOrDefault(x => x.ClinicNum==_ecListClinics[idx].ClinicNum).ValueString;
				comboClinicEConfirm.SelectedIndex=idx;
			}
			comboClinicEConfirm.EndUpdate();
		}

		private void FillConfStatusesGrid() {
			List<long> listDontSendConf=PrefC.GetString(PrefName.ApptConfirmExcludeESend).Split(',').Select(x => PIn.Long(x)).ToList();
			List<long> listDontChange=PrefC.GetString(PrefName.ApptConfirmExcludeEConfirm).Split(',').Select(x => PIn.Long(x)).ToList();
			List<long> listDontSendRem=PrefC.GetString(PrefName.ApptConfirmExcludeERemind).Split(',').Select(x => PIn.Long(x)).ToList();
			gridConfStatuses.BeginUpdate();
			gridConfStatuses.ListGridColumns.Clear();
			gridConfStatuses.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),100));
			gridConfStatuses.ListGridColumns.Add(new GridColumn(Lan.g(this,"Don't Send"),70,HorizontalAlignment.Center));
			gridConfStatuses.ListGridColumns.Add(new GridColumn(Lan.g(this,"Don't Change"),70,HorizontalAlignment.Center));
			gridConfStatuses.ListGridRows.Clear();
			foreach(Def defConfStatus in _listDefsApptStatus) {
				GridRow row=new GridRow();
				row.Cells.Add(defConfStatus.ItemName);
				row.Cells.Add(listDontSendConf.Contains(defConfStatus.DefNum) ? "X" : "");
				row.Cells.Add(listDontChange.Contains(defConfStatus.DefNum) ? "X" : "");
				row.Tag=defConfStatus;
				gridConfStatuses.ListGridRows.Add(row);
			}
			gridConfStatuses.EndUpdate();
		}

		private void FillRemindConfirmData() {
			#region Fill Reminders grid.
			gridRemindersMain.BeginUpdate();
			gridRemindersMain.ListGridColumns.Clear();
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Type"),150) { TextAlign=HorizontalAlignment.Center });
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Lead Time"),250));
			//gridRemindersMain.Columns.Add(new ODGridColumn("Send\r\nAll",50) { TextAlign=HorizontalAlignment.Center });
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send Order"),100));
			gridRemindersMain.NoteSpanStart=1;
			gridRemindersMain.NoteSpanStop=2;
			gridRemindersMain.ListGridRows.Clear();
			GridRow row;
			if(_ecClinicCur==null||_ecClinicCur.IsConfirmDefault) {//Use defaults
				_clinicRuleClinicNum=0;
			}
			else {
				_clinicRuleClinicNum=_ecClinicCur.ClinicNum;
			}
			IEnumerable<ApptReminderRule> apptReminderRules=_dictClinicRules[_clinicRuleClinicNum]
				.OrderBy(x => x.TypeCur)//Reminders first, then Confirmation
				.ThenBy(x => !x.IsEnabled)//Show enabled before disabled
				.ThenBy(x => x.TSPrior);
			foreach(ApptReminderRule apptRule in apptReminderRules) {
				string sendOrderText = string.Join(", ",apptRule.SendOrder.Split(',').Select(x => Enum.Parse(typeof(CommType),x).ToString()));
				if(!apptRule.Language.IsNullOrEmpty()) {
					continue;//only show default langauges in this grid.
				}
				row=new GridRow();
				row.Cells.Add(Lan.g(this,apptRule.TypeCur.GetDescription())
					+(_ecClinicCur.IsConfirmDefault ? "\r\n("+Lan.g(this,"Defaults")+")" : ""));
				long tsPriorTicks=(apptRule.TypeCur==ApptReminderType.ScheduleThankYou) ? Math.Abs(apptRule.TSPrior.Ticks) : apptRule.TSPrior.Ticks;
				string tsPrior=DateTools.ToStringDH(new TimeSpan(tsPriorTicks));
				if(!apptRule.IsEnabled) {
					tsPrior="("+Lan.g(this,"Disabled")+") "+tsPrior;
				}
				row.Cells.Add(tsPrior);
				row.Cells.Add(apptRule.IsSendAll ? Lan.g(this,"All") : sendOrderText);
				string strTemplateSMS=apptRule.TemplateSMS;
				row.Note=Lan.g(this,"SMS Template")+":\r\n"+strTemplateSMS+"\r\n\r\n"+Lan.g(this,"Email Subject Template")+":\r\n"
					+apptRule.TemplateEmailSubject+"\r\n"+Lan.g(this,"Email Template")+":\r\n"+apptRule.TemplateEmail;
				row.Tag=apptRule;
				if(gridRemindersMain.ListGridRows.Count%2==1) {
					row.ColorBackG=Color.FromArgb(240,240,240);//light gray every other row.
				}
				gridRemindersMain.ListGridRows.Add(row);
			}
			gridRemindersMain.EndUpdate();
			#endregion
			#region Set add buttons
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			if(comboClinicEConfirm.SelectedIndex>0) {//REAL CLINIC
				checkUseDefaultsEC.Visible=true;
				checkUseDefaultsEC.Enabled=allowEdit;
				checkIsConfirmEnabled.Enabled=allowEdit;//because we either cannot see it, or we are editing defaults.
				checkIsConfirmEnabled.Visible=true;
			}
			else {//CLINIC DEFAULTS/PRACTICE
				checkUseDefaultsEC.Visible=false;
				checkUseDefaultsEC.Enabled=false;
				checkIsConfirmEnabled.Enabled=false;//because we either cannot see it, or we are editing defaults.
				checkIsConfirmEnabled.Visible=false;
			}
			checkUseDefaultsEC.Checked=(_ecClinicCur!=null&&_ecClinicCur.ClinicNum>0&&_ecClinicCur.IsConfirmDefault);
			butAddReminder.Enabled=allowEdit;
			butAddConfirmation.Enabled=allowEdit;
			butAddThankYouVerify.Enabled=allowEdit;
			#endregion
			#region Thank You Event Calendar Title
			ClinicPref thankYouTitle=_listThankYouTitles.FirstOrDefault(x => x.ClinicNum==(_ecClinicCur?.ClinicNum??0));
			if(thankYouTitle is null) {
				thankYouTitle=_listThankYouTitles.FirstOrDefault(x => x.ClinicNum==0);
			}
			textThankYouTitle.Text=thankYouTitle.ValueString;
			textThankYouTitle.Enabled=PrefC.GetBool(PrefName.ApptThankYouAutoEnabled) && allowEdit;
			labelThankYouTitle.Enabled=PrefC.GetBool(PrefName.ApptThankYouAutoEnabled) && allowEdit;
			#endregion
		}

		private void FillECRActivationButtons() {
			//Reminder Activation Status
			FillActivateButton(PrefName.ApptRemindAutoEnabled,Lan.g(this,"eReminders"),butActivateReminder,textStatusReminders);
			//Confirmation Activation Status
			FillActivateButton(PrefName.ApptConfirmAutoEnabled,Lan.g(this,"eConfirmations"),butActivateConfirm,textStatusConfirmations);
			//ThankYou Activation Status
			FillActivateButton(PrefName.ApptThankYouAutoEnabled,Lan.g(this,"Auto Thank-You"),butActivateThanks,textStatusThankYous);
			//Arrivals Activation Status
			FillActivateButton(PrefName.ApptArrivalAutoEnabled,Lan.g(this,ApptReminderType.Arrival.GetDescription()),butActivateArrivals,textStatusArrivals);
		}

		private void FillActivateButton(PrefName prefEnabled,string serviceName,UI.Button button,TextBox textBoxStatus) {
			if(PrefC.GetBool(prefEnabled)) {
				textBoxStatus.Text=serviceName+" : "+Lan.g(this,"Active");
				textBoxStatus.BackColor=Color.FromArgb(236,255,236);//light green
				textBoxStatus.ForeColor=Color.Black;//instead of disabled grey
				button.Text=Lan.g(this,"Deactivate ")+serviceName;
			}
			else {
				textBoxStatus.Text=serviceName+" : "+Lan.g(this,"Inactive");
				textBoxStatus.BackColor=Color.FromArgb(254,235,233);//light red;
				textBoxStatus.ForeColor=Color.Black;//instead of disabled grey
				button.Text=Lan.g(this,"Activate ")+serviceName;
			}
		}

		private void gridConfStatuses_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAutomatedConfirmationStatuses formACS=new FormAutomatedConfirmationStatuses();
			formACS.ShowDialog();
			if(formACS.DialogResult==DialogResult.OK) {
				Defs.RefreshCache();
				_listDefsApptStatus=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
				FillConfStatusesGrid();
			}
		}

		private void gridRemindersMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			if(e.Row<0 || !(gridRemindersMain.ListGridRows[e.Row].Tag is ApptReminderRule)) {
				return;//we did not click on a valid row.
			}
			if(_ecClinicCur!=null && _ecClinicCur.ClinicNum>0 && _ecClinicCur.IsConfirmDefault && !SwitchFromDefaults()) {
				return;
			}
			ApptReminderRule arr = (ApptReminderRule)gridRemindersMain.ListGridRows[e.Row].Tag;
			int idx=_dictClinicRules[_clinicRuleClinicNum].IndexOf(arr);
			using FormApptReminderRuleEdit FormARRE = new FormApptReminderRuleEdit(arr,_dictClinicRules[_clinicRuleClinicNum]);
			FormARRE.ShowDialog();
			if(FormARRE.DialogResult!=DialogResult.OK) {
				_dictClinicRules[_clinicRuleClinicNum]=FormARRE.ListRulesClinicOld;
				FillRemindConfirmData();
				return;
			}
			if(FormARRE.ApptReminderRuleCur==null) {//Delete
				_dictClinicRules[_clinicRuleClinicNum].RemoveAt(idx);
			}
			else if(FormARRE.ApptReminderRuleCur.IsNew) {//Insert
				_dictClinicRules[_clinicRuleClinicNum].Add(FormARRE.ApptReminderRuleCur);//should never happen from the double click event
			}
			else {//Update
				_dictClinicRules[_clinicRuleClinicNum][idx]=FormARRE.ApptReminderRuleCur;
				foreach(ApptReminderRule langRule in FormARRE.ListLanguageRules.FindAll(x => x.ApptReminderRuleNum!=0)) {//update associated language rules
					int index=_dictClinicRules[_clinicRuleClinicNum].IndexOf(langRule);
					_dictClinicRules[_clinicRuleClinicNum][index]=langRule;
				}
			}
			AddRemoveLanguageRules(FormARRE);
			if(FormARRE.IsPrefsChanged) {
				FillConfStatusesGrid();
			}
			FillRemindConfirmData();
		}
		
		private void butAddReminder_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.Reminder);
		}
		
		private void butAddConfirmation_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.ConfirmationFutureDay);
		}

		private void butAddThankYou_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.ScheduleThankYou);
		}

		private void butAddArrival_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.Arrival);
		}

		private void AddRule(ApptReminderType apptReminderType) {
			if(_ecClinicCur!=null && _ecClinicCur.ClinicNum>0 && _ecClinicCur.IsConfirmDefault) {
				if(!SwitchFromDefaults()) {
					return;
				}
			}
			ApptReminderRule arr=ApptReminderRules.CreateDefaultReminderRule(apptReminderType,_ecClinicCur.ClinicNum);
			using FormApptReminderRuleEdit FormARRE = new FormApptReminderRuleEdit(arr,_dictClinicRules[_clinicRuleClinicNum]);
			FormARRE.ShowDialog();
			if(FormARRE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormARRE.ApptReminderRuleCur==null||FormARRE.ApptReminderRuleCur.IsNew) {
				//Delete or Update
				//Nothing to delete or update, this was a new rule.
			}
			else {//Insert
				_dictClinicRules[_clinicRuleClinicNum].Add(FormARRE.ApptReminderRuleCur);
			}
			AddRemoveLanguageRules(FormARRE);
			if(FormARRE.IsPrefsChanged) {
				FillConfStatusesGrid();
			}
			FillRemindConfirmData();
		}

		private void AddRemoveLanguageRules(FormApptReminderRuleEdit FormARRE) {
			if(FormARRE.ApptReminderRuleCur!=null) {
				foreach(ApptReminderRule langRuleNew in FormARRE.ListNonDefaultRulesAdded) { //Insert any new associated language rules
					_dictClinicRules[_clinicRuleClinicNum].Add(langRuleNew);
				}
			}
			foreach(ApptReminderRule associatedRule in FormARRE.ListNonDefaultRulesRemoving) { //Remove any associated language rules no longer needed
				int index=_dictClinicRules[_clinicRuleClinicNum].IndexOf(associatedRule);
				_dictClinicRules[_clinicRuleClinicNum].RemoveAt(index);
			}
		}

		private void comboClinicEConfirm_SelectedIndexChanged(object sender,EventArgs e) {
			if(_ecListClinics.Count==0||_dictClinicRules.Count==0) {
				return;//form load;
			}
			if(_ecClinicCur!=null&&_ecClinicCur.ClinicNum>0) {//do not update this clinic-pref if we are editing defaults.
				_ecClinicCur.IsConfirmEnabled=checkIsConfirmEnabled.Checked;
				Clinics.Update(_ecClinicCur);
				Signalods.SetInvalid(InvalidType.Providers);
				//no need to save changes here because all Appointment reminder rules are saved to the DB from the edit window.
			}
			#region ApptThankYou Titles
			ParseThankYouTitle();
			#endregion
			if(_ecClinicCur!=null) {
				ApptReminderRules.SyncByClinicAndTypes(_dictClinicRules[_ecClinicCur.ClinicNum],_ecClinicCur.ClinicNum,_arrApptReminderTypes);
			}
			if(comboClinicEConfirm.SelectedIndex>-1&&comboClinicEConfirm.SelectedIndex<_ecListClinics.Count) {
				_ecClinicCur=_ecListClinics[comboClinicEConfirm.SelectedIndex];
			}
			checkUseDefaultsEC.Checked=_ecClinicCur!=null&&_ecClinicCur.IsConfirmDefault;
			checkIsConfirmEnabled.Checked=_ecClinicCur!=null&&_ecClinicCur.IsConfirmEnabled;
			FillRemindConfirmData();
		}

		///<summary>Parses textThankYouTitle textbox into the appropriate ClinicPref in _listThankYouTitles.</summary>
		private void ParseThankYouTitle() {
			long clinicNum=_ecClinicCur?.ClinicNum??0;
			ClinicPref clinicPref=_listThankYouTitles.FirstOrDefault(x => x.ClinicNum==clinicNum);
			if(clinicPref is null) {
				clinicPref=new ClinicPref(clinicNum,PrefName.ApptThankYouCalendarTitle,textThankYouTitle.Text);
				_listThankYouTitles.Add(clinicPref);
			}
			clinicPref.ValueString=textThankYouTitle.Text;
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchFromDefaults() {
			if(_ecClinicCur==null||_ecClinicCur.ClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			//if(!MsgBox.Show(this,true,"Would you like to make a copy of the defaults for this clinic and continue editing the copy?")) {
			//	return false;
			//}
			_dictClinicRules[_ecClinicCur.ClinicNum]=_dictClinicRules[0].Select(x => x.Copy()).ToList();
			_dictClinicRules[_ecClinicCur.ClinicNum].ForEach(x => x.ClinicNum=_ecClinicCur.ClinicNum);
			_ecClinicCur.IsConfirmDefault=false;
			_ecListClinics[_ecListClinics.FindIndex(x => x.ClinicNum==_ecClinicCur.ClinicNum)].IsConfirmDefault=false;
			//Clinics.Update(_clinicCur);
			//Signalods.SetInvalid(InvalidType.Providers);//for clinics
			FillRemindConfirmData();
			return true;
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchToDefaults() {
			if(_ecClinicCur==null||_ecClinicCur.ClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			if(_dictClinicRules[_ecClinicCur.ClinicNum].Count>0&&!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete custom rules for this clinic and switch to using defaults? This cannot be undone.")) {
				checkUseDefaultsEC.Checked=false;//undo checking of box.
				return false;
			}
			_ecClinicCur.IsConfirmDefault=true;
			_dictClinicRules[_ecClinicCur.ClinicNum]=new List<ApptReminderRule>();
			FillRemindConfirmData();
			return true;
		}

		private void checkIsConfirmEnabled_CheckedChanged(object sender,EventArgs e) {
			FillRemindConfirmData();
		}

		private void checkUseDefaultsEC_CheckedChanged(object sender,EventArgs e) {
			//TURNING DEFAULTS OFF
			if(!checkUseDefaultsEC.Checked&&_ecClinicCur.IsConfirmDefault&&_ecClinicCur.ClinicNum>0) {//Default switched off
				_ecClinicCur.IsConfirmDefault=false;
				_ecListClinics[comboClinicEConfirm.SelectedIndex].IsConfirmDefault=false;
				FillRemindConfirmData();
				return;
			}
			//TURNING DEFAULTS ON
			else if(checkUseDefaultsEC.Checked&&!_ecClinicCur.IsConfirmDefault&&_ecClinicCur.ClinicNum>0) {//Default switched on
				SwitchToDefaults();
				return;
			}
			//Silently do nothing because we just "changed" the checkbox to the state of the current clinic. 
			//I.e. When switching from clinic 1 to clinic 2, if 1 uses defaults and 2 does not, then this allows the new clinic to be loaded without updating the DB.
		}

		private void butActivateConfirm_Click(object sender,EventArgs e) {
			//also validates office is signed up for confirmations.
			if(ActivateEService(PrefName.ApptConfirmAutoEnabled,ApptReminderType.ConfirmationFutureDay,eServiceCode.ConfirmationRequest)) {
				AddDefaults(ApptReminderType.ConfirmationFutureDay);//Add one default confirmation rule if none exists.
			}
		}

		private void butActivateReminder_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.ApptRemindAutoEnabled,ApptReminderType.Reminder)) {
				AddDefaults(ApptReminderType.Reminder,null,TimeSpan.FromDays(2));//Add two default reminder rules if none exists.
			}
		}

		private void butActivateThankYou_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.ApptThankYouAutoEnabled,ApptReminderType.ScheduleThankYou)) {
				AddDefaults(ApptReminderType.ScheduleThankYou);//Add one default thankyou rule if none exists.
			}
		}

		private void butActivateArrivals_Click(object sender,EventArgs e) {
			//also valdates office is signed up for confirmations.
			if(ActivateEService(PrefName.ApptArrivalAutoEnabled,ApptReminderType.Arrival,eServiceCode.ConfirmationRequest)) {
				AddDefaults(ApptReminderType.Arrival);//Add one default arrival rule if none exists.
			}
		}

		private bool ActivateEService(PrefName prefEnabled,ApptReminderType eService,eServiceCode? codeToValidate=null) {		
			bool isAutoEnabled=PrefC.GetBool(prefEnabled);	
			if(!isAutoEnabled && codeToValidate!=null && !WebServiceMainHQProxy.IsEServiceActive(_signupOut,(eServiceCode)codeToValidate)) { //Not yet activated with HQ.
				MsgBox.Show(this,$"You must first signup for {codeToValidate.GetDescription()} via the Signup tab before activating {codeToValidate.GetDescription()}.");
				return false;
			}
			isAutoEnabled=!isAutoEnabled;
			Prefs.UpdateBool(prefEnabled,isAutoEnabled);
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,$"{eService.GetDescription()} "+(isAutoEnabled ? "activated" : "deactivated")+".");
			Prefs.RefreshCache();
			Signalods.SetInvalid(InvalidType.Prefs);
			FillECRActivationButtons();
			return isAutoEnabled;
		}

		private void AddDefaults(ApptReminderType reminderType,params TimeSpan?[] arrTimeSpans) {
			if(_dictClinicRules[0].Count(x => x.TypeCur==reminderType)==0) {
				if(arrTimeSpans.Length==0) {
					arrTimeSpans=new TimeSpan?[1] { null };
				}
				foreach(TimeSpan? timeSpan in arrTimeSpans) {
					ApptReminderRule arr=ApptReminderRules.CreateDefaultReminderRule(reminderType,0);
					arr.TSPrior=timeSpan??arr.TSPrior;//Maintain default if not set.
					_dictClinicRules[0].Add(arr);
					FillRemindConfirmData();
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(new[] { comboStatusEAccepted.SelectedIndex,comboStatusESent.SelectedIndex,comboStatusEDeclined.SelectedIndex,comboStatusEFailed.SelectedIndex }
				.Where(x => x!=-1)
				.GroupBy(x => x)
				.Any(x => x.Count()>1)) 
			{
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"All eConfirmation appointment statuses should be different. Continue anyway?")) {
					return;
				}
			}
			foreach(long clinicNum in _dictClinicRules.Keys) {
				var groupedRules=_dictClinicRules[clinicNum]
					//Default rule will have no language set.
					.FindAll(x => x.Language==string.Empty)
					.FindAll(x => x.IsEnabled)
					.GroupBy(x => new {x.TypeCur,x.TSPrior});
				foreach(var group in groupedRules) {
					if(group.ToList().Count>1) {
						MessageBox.Show(this,Lans.g(this,"Duplicate rules are not allowed for the same type and days."));
						return;
					}
				}
			}
			SaveTabECR();
			DialogResult=DialogResult.OK;	
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}