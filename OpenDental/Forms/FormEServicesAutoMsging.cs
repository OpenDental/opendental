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

	public partial class FormEServicesAutoMsging:FormODBase {
		//==================== eConfirm & eRemind Variables ====================
		private List<Clinic> _listClinicsEConfirm;
		private Clinic _clinicCurEConfirm;
		///<summary>A list used to track ClinicPrefs that have been created or changed.</summary>
		private List<ClinicPref> _listClinicPrefsUseDefaults=new List<ClinicPref>();
		///<summary>All rules for all clinics.</summary>
		private List<ApptReminderRule> _listApptReminderRulesAll;
		private ApptReminderType[] _arrayApptReminderTypes=new ApptReminderType[] { ApptReminderType.Reminder
			,ApptReminderType.ConfirmationFutureDay,ApptReminderType.ScheduleThankYou,ApptReminderType.Arrival
			,ApptReminderType.PatientPortalInvite,ApptReminderType.GeneralMessage };
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

		public FormEServicesAutoMsging(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesAutoMsging_Load(object sender,EventArgs e) {
			LayoutMenu();
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			_listClinicPrefsUseDefaults=ClinicPrefs.GetWhere(x => ListTools.In(x.PrefName,new List<PrefName>() { PrefName.ApptArrivalUseDefaults,PrefName.ApptConfirmUseDefaults,
				PrefName.ApptThankYouUseDefaults,PrefName.PatientPortalInviteUseDefaults,PrefName.ApptReminderUseDefaults,
				PrefName.ApptGeneralMessageUseDefaults }));
			FillActivationButtons();
			SetListClinicsAndRulesHelper();
			FillRemindConfirmData();
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			butActivateReminder.Enabled=allowEdit;
			butActivateConfirm.Enabled=allowEdit;
			butActivateThanks.Enabled=allowEdit;
			butActivateArrivals.Enabled=allowEdit;
			butActivateInvites.Enabled=allowEdit;
			butActivateGeneralMessages.Enabled=allowEdit;
			butAddArrival.Enabled=allowEdit;
			butAddReminder.Enabled=allowEdit;
			butAddConfirmation.Enabled=allowEdit;
			butAddPatientPortalInviteBefore.Enabled=allowEdit;
			butAddPatientPortalInvite.Enabled=allowEdit;
			butAddThankYouVerify.Enabled=allowEdit;
			butAddGeneralMessage.Enabled=allowEdit;
			checkIsConfirmEnabled.Enabled=allowEdit;
			checkUseDefaultsArrival.Enabled=allowEdit;
			checkUseDefaultsConfirmation.Enabled=allowEdit;
			checkUseDefaultsInviteBefore.Enabled=allowEdit;
			checkUseDefaultsInvite.Enabled=allowEdit;
			checkUseDefaultsReminder.Enabled=allowEdit;
			checkUseDefaultThanks.Enabled=allowEdit;
			checkUseDefaultsGeneralMessage.Enabled=allowEdit;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Advanced Settings",menuItemAdvancedSettings_Click));
			menuMain.EndUpdate();
		}

		private void SaveToDb() {
			List<ApptReminderRule> listApptReminderRules=_listApptReminderRulesAll.FindAll(x => x.ClinicNum==_clinicCurEConfirm.ClinicNum);
			ApptReminderRules.SyncByClinicAndTypes(listApptReminderRules,_clinicCurEConfirm.ClinicNum,_arrayApptReminderTypes);
			if(_clinicCurEConfirm.ClinicNum!=0) {
				_clinicCurEConfirm.IsConfirmEnabled=checkIsConfirmEnabled.Checked;
				Clinics.Update(_clinicCurEConfirm);
				DataValid.SetInvalid(InvalidType.Providers); //Providers also includes clinics.
			}
			bool changedClinicPrefs=false;
			for(int i=0;i<_listClinicPrefsUseDefaults.Count;i++) {
				changedClinicPrefs|=ClinicPrefs.Upsert(_listClinicPrefsUseDefaults[i].PrefName,_listClinicPrefsUseDefaults[i].ClinicNum,_listClinicPrefsUseDefaults[i].ValueString);
			}
			if(changedClinicPrefs) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		///<summary>Fills in memory Rules dictionary and clinics list based. This is very different from AppointmentReminderRules.GetRuleAndClinics.</summary>
		private void SetListClinicsAndRulesHelper() {
			if(PrefC.HasClinicsEnabled) {//CLINICS
				_listClinicsEConfirm=new List<Clinic>() { new Clinic() { Description="Defaults",Abbr="Defaults" } };
				_listClinicsEConfirm.AddRange(Clinics.GetForUserod(Security.CurUser));
			}
			else {//NO CLINICS
				_listClinicsEConfirm=new List<Clinic>() { new Clinic() { Description="Practice",Abbr="Practice" } };
			}
			_listApptReminderRulesAll=ApptReminderRules.GetForTypes(_arrayApptReminderTypes);
			comboClinicEConfirm.BeginUpdate();
			comboClinicEConfirm.Items.Clear();
			for(int i=0;i<_listClinicsEConfirm.Count;i++) {
				comboClinicEConfirm.Items.Add(_listClinicsEConfirm[i].Abbr);//combo clinics may not be visible.
			}
			comboClinicEConfirm.SelectedIndex=0;
			comboClinicEConfirm.EndUpdate();
			_clinicCurEConfirm=_listClinicsEConfirm[0];
		}

		private void FillRemindConfirmData() {
			#region Fill Reminders grid
			gridRemindersMain.BeginUpdate();
			gridRemindersMain.ListGridColumns.Clear();
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Type"),150) { TextAlign=HorizontalAlignment.Center });
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Lead Time"),250));
			gridRemindersMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send Order"),100));
			gridRemindersMain.NoteSpanStart=1;
			gridRemindersMain.NoteSpanStop=2;
			gridRemindersMain.ListGridRows.Clear();
			GridRow row;
			//For each ApptReminderType, use the associated defaults preference.
			List<ApptReminderRule> listApptReminderRules=new List<ApptReminderRule>();
			long clinicNum=0;
			for(int i=0;i<_arrayApptReminderTypes.Length;i++) {
				bool useDefaultForApptType=GetUseDefaultForApptType(_arrayApptReminderTypes[i],_clinicCurEConfirm.ClinicNum);
				if(useDefaultForApptType) {
					clinicNum=0;
				}
				else {
					clinicNum=_clinicCurEConfirm.ClinicNum;
				}
				listApptReminderRules.AddRange(_listApptReminderRulesAll.FindAll(x => x.ClinicNum==clinicNum && x.TypeCur==_arrayApptReminderTypes[i]));
			}
			listApptReminderRules=listApptReminderRules
				.OrderBy(x => x.TypeCur)//Reminders first, then Confirmation
				.ThenBy(x => !x.IsEnabled)//Show enabled before disabled
				.ThenBy(x => x.TSPrior).ToList();
			for(int i=0;i<listApptReminderRules.Count();i++)  {
				List<string> listStringSendOrder=listApptReminderRules[i].SendOrder.Split(',').Select(x => Enum.Parse(typeof(CommType),x).ToString()).ToList();
				string sendOrderText=string.Join(", ",listStringSendOrder);
				if(!listApptReminderRules[i].Language.IsNullOrEmpty()) {
					continue;//only show default langauges in this grid.
				}
				row=new GridRow();
				bool useDefaultForApptType=GetUseDefaultForApptType(listApptReminderRules[i].TypeCur,_clinicCurEConfirm.ClinicNum);
				if(_clinicCurEConfirm.ClinicNum>0 && useDefaultForApptType) {
					row.Cells.Add(Lan.g(this,listApptReminderRules[i].TypeCur.GetDescription())+"\r\n("+Lan.g(this,"Defaults")+")");
				}
				else {
					row.Cells.Add(Lan.g(this,listApptReminderRules[i].TypeCur.GetDescription())+"");
				}
				long tsPriorTicks;
				if(listApptReminderRules[i].TypeCur==ApptReminderType.ScheduleThankYou) {
					tsPriorTicks=Math.Abs(listApptReminderRules[i].TSPrior.Ticks);
				}
				else {
					tsPriorTicks=listApptReminderRules[i].TSPrior.Ticks;
				}
				string strTsPrior=DateTools.ToStringDH(new TimeSpan(tsPriorTicks));
				if(!listApptReminderRules[i].IsEnabled) {
					strTsPrior="("+Lan.g(this,"Disabled")+") "+strTsPrior;
				}
				row.Cells.Add(strTsPrior);
				if(listApptReminderRules[i].IsSendAll) {
					row.Cells.Add(Lan.g(this,"All"));
				}
				else {
					row.Cells.Add(sendOrderText);
				}
				string note="";
				if(listStringSendOrder.Contains(CommType.Text.ToString())) {
					note+=Lan.g(this,"SMS Template")+":\r\n"+GetShortenedNote(listApptReminderRules[i].TemplateSMS)+"\r\n\r\n";
				}
				if(listStringSendOrder.Contains(CommType.Email.ToString())) {
					note+=Lan.g(this,"Email Subject Template")+":\r\n"+GetShortenedNote(listApptReminderRules[i].TemplateEmailSubject)+"\r\n"
					+Lan.g(this,"Email Template")+":\r\n"+GetShortenedNote(listApptReminderRules[i].TemplateEmail);
				}
				row.Note=note;
				row.Tag=listApptReminderRules[i];
				if(gridRemindersMain.ListGridRows.Count%2==1) {
					row.ColorBackG=Color.FromArgb(240,240,240);//light gray every other row.
				}
				gridRemindersMain.ListGridRows.Add(row);
			}
			gridRemindersMain.EndUpdate();
			#endregion Fill Reminders grid
			#region Set UseDefaults Checkboxes
			bool isVisible=_clinicCurEConfirm.ClinicNum!=0;
			checkUseDefaultsArrival.Visible=isVisible;
			checkUseDefaultsConfirmation.Visible=isVisible;
			checkUseDefaultsInviteBefore.Visible=isVisible;
			checkUseDefaultsInvite.Visible=isVisible;
			checkUseDefaultsReminder.Visible=isVisible;
			checkUseDefaultThanks.Visible=isVisible;
			checkUseDefaultsGeneralMessage.Visible=isVisible;
			checkIsConfirmEnabled.Visible=isVisible;
			checkUseDefaultsArrival.Checked=GetIsClinicPrefEnabled(PrefName.ApptArrivalUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultsConfirmation.Checked=GetIsClinicPrefEnabled(PrefName.ApptConfirmUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultsInviteBefore.Checked=GetIsClinicPrefEnabled(PrefName.PatientPortalInviteUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultsInvite.Checked=GetIsClinicPrefEnabled(PrefName.PatientPortalInviteUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultsReminder.Checked=GetIsClinicPrefEnabled(PrefName.ApptReminderUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultThanks.Checked=GetIsClinicPrefEnabled(PrefName.ApptThankYouUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkUseDefaultsGeneralMessage.Checked=GetIsClinicPrefEnabled(PrefName.ApptGeneralMessageUseDefaults,_clinicCurEConfirm.ClinicNum);
			checkIsConfirmEnabled.Checked=_clinicCurEConfirm.IsConfirmEnabled;
			#endregion
		}

		private void FillActivationButtons() {
			//Reminder Activation Status
			FillActivateButton(PrefName.ApptRemindAutoEnabled,Lan.g(this,"eReminders"),butActivateReminder,textStatusReminders);
			//Confirmation Activation Status
			FillActivateButton(PrefName.ApptConfirmAutoEnabled,Lan.g(this,"eConfirmations"),butActivateConfirm,textStatusConfirmations);
			//ThankYou Activation Status
			FillActivateButton(PrefName.ApptThankYouAutoEnabled,Lan.g(this,"Auto Thank-You"),butActivateThanks,textStatusThankYous);
			//Arrivals Activation Status
			FillActivateButton(PrefName.ApptArrivalAutoEnabled,Lan.g(this,ApptReminderType.Arrival.GetDescription()),butActivateArrivals,textStatusArrivals);
			//Patient Portal Invite Activation Status
			FillActivateButton(PrefName.PatientPortalInviteEnabled,Lan.g(this,"Patient Portal Invites"),butActivateInvites,textStatusNotifications);
			//General Message Activation Status
			FillActivateButton(PrefName.ApptGeneralMessageAutoEnabled,Lan.g(this,"General Messages"),butActivateGeneralMessages,textStatusGeneralMessage);
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

		private void menuItemAdvancedSettings_Click(object sender,EventArgs e) {
			using FormEServicesAutoMsgingAdvanced formAutomatedConfirmationStatuses=new FormEServicesAutoMsgingAdvanced();
			formAutomatedConfirmationStatuses.ShowDialog();
		}

		private void GridRemindersMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			if(e.Row<0 || !(gridRemindersMain.ListGridRows[e.Row].Tag is ApptReminderRule)) {
				return;//we did not click on a valid row.
			}
			ApptReminderRule apptReminderRule=(ApptReminderRule)gridRemindersMain.ListGridRows[e.Row].Tag;
			bool useDefaultForApptType=GetUseDefaultForApptType(apptReminderRule.TypeCur,_clinicCurEConfirm.ClinicNum);
			if(_clinicCurEConfirm.ClinicNum>0 && useDefaultForApptType && !SwitchFromDefaults(apptReminderRule.TypeCur)) {
				return;
			}
			List<ApptReminderRule> listApptReminderRules=_listApptReminderRulesAll.FindAll(x => x.ClinicNum==apptReminderRule.ClinicNum && x.TypeCur==apptReminderRule.TypeCur);
			using FormApptReminderRuleEdit formApptReminderRuleEdit = new FormApptReminderRuleEdit(apptReminderRule,listApptReminderRules);
			formApptReminderRuleEdit.ShowDialog();
			if(formApptReminderRuleEdit.DialogResult!=DialogResult.OK) {
				_listApptReminderRulesAll.RemoveAll(x => x.ClinicNum==apptReminderRule.ClinicNum && x.TypeCur==apptReminderRule.TypeCur);
				_listApptReminderRulesAll.AddRange(formApptReminderRuleEdit.ListRulesClinicOld);
				FillRemindConfirmData();
				return;
			}
			if(formApptReminderRuleEdit.ApptReminderRuleCur==null) {//Delete
				_listApptReminderRulesAll.Remove(apptReminderRule);
			}
			else if(formApptReminderRuleEdit.ApptReminderRuleCur.IsNew) {//Insert
				_listApptReminderRulesAll.Add(formApptReminderRuleEdit.ApptReminderRuleCur);//should never happen from the double click event
			}
			else {//Update
				int idxRule=_listApptReminderRulesAll.IndexOf(apptReminderRule);
				_listApptReminderRulesAll[idxRule]=formApptReminderRuleEdit.ApptReminderRuleCur;
				//todo: Why would an ApptReminderRuleNum ever be zero?
				//GetListLanguageRules returns a list of language rules associated with the currently selected apptReminderRule.
				//They are gauranteed to have matching timespan, type, and clinicNum to the current rule, and not to have an empty language field. 
				List<ApptReminderRule> listApptReminderRulesLanguage=formApptReminderRuleEdit.GetListLanguageRules().FindAll(x => x.ApptReminderRuleNum!=0);
				//todo: this loop does nothing.  What was the intent?	
				//There are only elements  in listApptReminderRulesLanguage when a language rule is deleted.
				//it is assigned a key value somewhere before being returned to this form.
				//When this loop is commented out, the form still seems to function properly
				//in FormApptReminderRuleEdit when new language rules are added,
				//initially they have no key value within formApptReminderRuleEdit, and it is added later when saved to db by autoincriment				
				for(int i = 0;i<listApptReminderRulesLanguage.Count;i++) {
					int index=_listApptReminderRulesAll.IndexOf(listApptReminderRulesLanguage[i]);
					_listApptReminderRulesAll[index]=listApptReminderRulesLanguage[i];
				}
			}
			AddRemoveLanguageRules(formApptReminderRuleEdit);
			FillRemindConfirmData();
		}
		
		private void ButAddReminder_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.Reminder);
		}
		
		private void ButAddConfirmation_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.ConfirmationFutureDay);
		}

		private void ButAddThankYou_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.ScheduleThankYou);
		}

		private void ButAddArrival_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.Arrival);
		}

		private void butAddPatientPortalInvite_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.PatientPortalInvite,false);
		}

		private void butAddPatientPortalInviteBefore_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.PatientPortalInvite);
		}

		private void butAddGeneralMessage_Click(object sender,EventArgs e) {
			AddRule(ApptReminderType.GeneralMessage);
		}

		private void AddRule(ApptReminderType apptReminderType,bool isBeforeAppointment=true) {
			bool useDefaultForApptType=GetUseDefaultForApptType(apptReminderType,_clinicCurEConfirm.ClinicNum);
			if(_clinicCurEConfirm.ClinicNum>0 && useDefaultForApptType) {
				if(SwitchFromDefaults(apptReminderType)) {
					FillRemindConfirmData();
				}
				else {
					return;
				}
			}
			ApptReminderRule apptReminderRule=ApptReminderRules.CreateDefaultReminderRule(apptReminderType,_clinicCurEConfirm.ClinicNum,isBeforeAppointment);
			List<ApptReminderRule> listApptReminderRulesClin=_listApptReminderRulesAll.FindAll(x => x.ClinicNum==apptReminderRule.ClinicNum);
			using FormApptReminderRuleEdit formApptReminderRuleEdit=new FormApptReminderRuleEdit(apptReminderRule,listApptReminderRulesClin);
			formApptReminderRuleEdit.ShowDialog();
			if(formApptReminderRuleEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formApptReminderRuleEdit.ApptReminderRuleCur==null || formApptReminderRuleEdit.ApptReminderRuleCur.IsNew) {
				//Delete or Update
				//Nothing to delete or update, this was a new rule.
			}
			else {//Insert
				_listApptReminderRulesAll.Add(formApptReminderRuleEdit.ApptReminderRuleCur);
			}
			AddRemoveLanguageRules(formApptReminderRuleEdit);
			FillRemindConfirmData();
		}

		private void AddRemoveLanguageRules(FormApptReminderRuleEdit formApptReminderRuleEdit) {
			if(formApptReminderRuleEdit.ApptReminderRuleCur!=null) {
				for(int i=0; i<formApptReminderRuleEdit.ListNonDefaultRulesAdded.Count; i++) {
					_listApptReminderRulesAll.Add(formApptReminderRuleEdit.ListNonDefaultRulesAdded[i]);
				}
			}
			for(int i=0;i<formApptReminderRuleEdit.ListNonDefaultRulesRemoving.Count;i++) {
				_listApptReminderRulesAll.Remove(formApptReminderRuleEdit.ListNonDefaultRulesRemoving[i]);
			}
		}

		private void checkUseDefaultsReminder_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.Reminder);
		}

		private void checkUseDefaultsConfirmation_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.ConfirmationFutureDay);
		}

		private void checkUseDefaultThanks_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.ScheduleThankYou);
		}

		private void checkUseDefaultsArrival_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.Arrival);
		}

		private void checkUseDefaultsInviteBefore_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.PatientPortalInvite);
		}

		private void checkUseDefaultsInvite_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.PatientPortalInvite);
		}

		private void checkUseDefaultsGeneralMessage_Click(object sender,EventArgs e) {
			ChangeUseDefaults((CheckBox)sender,ApptReminderType.GeneralMessage);
		}

		private void comboClinicEConfirm_SelectionChangeCommitted(object sender,EventArgs e) {
			SaveToDb();
			_clinicCurEConfirm=_listClinicsEConfirm[comboClinicEConfirm.SelectedIndex];
			FillRemindConfirmData();
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchFromDefaults(ApptReminderType apptReminderType) {
			if(_clinicCurEConfirm.ClinicNum==0) {
				return false;//somehow editing default clinic anyways, no need to switch.
			}
			List<ApptReminderRule> listApptReminderRulesDefault=_listApptReminderRulesAll.FindAll(x => x.ClinicNum==0 && x.TypeCur==apptReminderType).Select(x => x.Copy()).ToList();
			_listApptReminderRulesAll.RemoveAll(x => x.ClinicNum==_clinicCurEConfirm.ClinicNum && x.TypeCur==apptReminderType);
			for(int i=0;i<listApptReminderRulesDefault.Count;i++) {
				listApptReminderRulesDefault[i].ClinicNum=_clinicCurEConfirm.ClinicNum;
			}
			_listApptReminderRulesAll.AddRange(listApptReminderRulesDefault);
			PrefName prefName=GetPrefNameForApptReminderType(apptReminderType);
			ClinicPref clinicPref=_listClinicPrefsUseDefaults.FirstOrDefault(x => x.ClinicNum==_clinicCurEConfirm.ClinicNum && x.PrefName==prefName);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(_clinicCurEConfirm.ClinicNum,prefName,false);
				_listClinicPrefsUseDefaults.Add(clinicPref);
			}
			clinicPref.ValueString=POut.Bool(false);
			return true;
		}

		///<summary>Switches the currently selected clinic over to using defaults. Also prompts user before continuing. 
		///Returns false if user cancelled or if there is no need to have switched to defaults.</summary>
		private bool SwitchToDefaults(ApptReminderType apptReminderType) {
			if(_listApptReminderRulesAll.Any(x => x.ClinicNum==_clinicCurEConfirm.ClinicNum && x.TypeCur==apptReminderType)) {
				string prompt=Lans.g(this,"Delete custom ")+apptReminderType.GetDescription()+Lans.g(this," rules for this clinic and switch to using defaults? This cannot be undone.");
				if(MessageBox.Show(this,prompt,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return false;
				}
			}
			_listApptReminderRulesAll.RemoveAll(x => x.TypeCur==apptReminderType && x.ClinicNum==_clinicCurEConfirm.ClinicNum);
			return true;
		}

		private void checkIsConfirmEnabled_Click(object sender,EventArgs e) {
			_clinicCurEConfirm.IsConfirmEnabled=checkIsConfirmEnabled.Checked;
		}

		private void ChangeUseDefaults(CheckBox checkBox,ApptReminderType apptReminderType) {
			if(_clinicCurEConfirm.ClinicNum==0) {
				return;//Clinic 0, we don't need to do anything here.
			}
			PrefName prefName=GetPrefNameForApptReminderType(apptReminderType);
			ClinicPref clinicPref=_listClinicPrefsUseDefaults.FirstOrDefault(x => x.ClinicNum==_clinicCurEConfirm.ClinicNum && x.PrefName==prefName);
			if(clinicPref==null) {
				clinicPref=new ClinicPref(_clinicCurEConfirm.ClinicNum,prefName,false);
				_listClinicPrefsUseDefaults.Add(clinicPref);
			}
			//TURNING DEFAULTS OFF
			if(!checkBox.Checked && PIn.Bool(clinicPref.ValueString)) {//Default switched off
				clinicPref.ValueString=POut.Bool(false);
				FillRemindConfirmData();
			}
			//TURNING DEFAULTS ON
			else if(checkBox.Checked && !PIn.Bool(clinicPref.ValueString)) {//Default switched on
				if(SwitchToDefaults(apptReminderType)) {
					clinicPref.ValueString=POut.Bool(true);
					FillRemindConfirmData();
				}
				else {
					checkBox.Checked=false;
				}
			}
			//Silently do nothing because we just "changed" the checkbox to the state of the current clinic. 
			//I.e. When switching from clinic 1 to clinic 2, if 1 uses defaults and 2 does not, then this allows the new clinic to be loaded without updating the DB.
		}

		private void ButActivateConfirm_Click(object sender,EventArgs e) {
			//also validates office is signed up for confirmations.
			if(ActivateEService(PrefName.ApptConfirmAutoEnabled,ApptReminderType.ConfirmationFutureDay,eServiceCode.ConfirmationRequest)) {
				AddDefaults(ApptReminderType.ConfirmationFutureDay);//Add one default confirmation rule if none exists.
			}
		}

		private void ButActivateReminder_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.ApptRemindAutoEnabled,ApptReminderType.Reminder)) {
				AddDefaults(ApptReminderType.Reminder,null,TimeSpan.FromDays(2));//Add two default reminder rules if none exists.
			}
		}

		private void ButActivateThankYou_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.ApptThankYouAutoEnabled,ApptReminderType.ScheduleThankYou)) {
				AddDefaults(ApptReminderType.ScheduleThankYou);//Add one default thankyou rule if none exists.
			}
		}

		private void ButActivateArrivals_Click(object sender,EventArgs e) {
			//also valdates office is signed up for confirmations.
			if(ActivateEService(PrefName.ApptArrivalAutoEnabled,ApptReminderType.Arrival,eServiceCode.ConfirmationRequest)) {
				AddDefaults(ApptReminderType.Arrival);//Add one default arrival rule if none exists.
			}
		}

		private void butActivateInvites_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.PatientPortalInviteEnabled,ApptReminderType.PatientPortalInvite,eServiceCode.PatientPortal)) {
				AddDefaults(ApptReminderType.PatientPortalInvite);
			}
		}

		private void butActivateGeneralMessages_Click(object sender,EventArgs e) {
			if(ActivateEService(PrefName.ApptGeneralMessageAutoEnabled,ApptReminderType.GeneralMessage)) {
				AddDefaults(ApptReminderType.GeneralMessage);
			}
		}

		private bool ActivateEService(PrefName prefEnabled,ApptReminderType appointmentReminderType,eServiceCode? codeToValidate=null) {		
			bool isAutoEnabled=PrefC.GetBool(prefEnabled);	
			if(!isAutoEnabled && codeToValidate!=null) { //Not yet activated with HQ.
				bool isEserviceActive=WebServiceMainHQProxy.IsEServiceActive(_signupOut,(eServiceCode)codeToValidate);
				if(!isEserviceActive) {
					MsgBox.Show(this,$"You must first signup for {codeToValidate.GetDescription()} via eServices Signup before activating {codeToValidate.GetDescription()}.");
					return false;
				}
			}
			isAutoEnabled=!isAutoEnabled;
			Prefs.UpdateBool(prefEnabled, isAutoEnabled);
			if(isAutoEnabled) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,$"{appointmentReminderType.GetDescription()}"+("activated")+".");
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,$"{appointmentReminderType.GetDescription()}"+("deactivated")+".");
			}
			Prefs.RefreshCache();
			Signalods.SetInvalid(InvalidType.Prefs);
			FillActivationButtons();
			return isAutoEnabled;
		}

		private void AddDefaults(ApptReminderType apptReminderType,params TimeSpan?[] arrayTimeSpans) {
			if(_listApptReminderRulesAll.FindAll(x => x.ClinicNum == 0).Count(x => x.TypeCur==apptReminderType)==0) {
				if(arrayTimeSpans.Length==0) {
					arrayTimeSpans=new TimeSpan?[1] { null };
				}
				for(int i=0;i<arrayTimeSpans.Length;i++) {
					ApptReminderRule apptReminderRule=ApptReminderRules.CreateDefaultReminderRule(apptReminderType,0);
					if(arrayTimeSpans[i]!=null) {
						apptReminderRule.TSPrior=arrayTimeSpans[i].Value;
					}
					_listApptReminderRulesAll.Add(apptReminderRule);
				}
				FillRemindConfirmData();
			}
		}

		///<summary>Returns true if the clinicpref exists and is set to true.</summary>
		private bool GetIsClinicPrefEnabled(PrefName prefName,long clinicNum) {
			ClinicPref clinicPref=_listClinicPrefsUseDefaults.FirstOrDefault(x => x.PrefName==prefName && x.ClinicNum==clinicNum);
			return clinicPref!=null && PIn.Bool(clinicPref.ValueString);
		}

		private bool GetUseDefaultForApptType(ApptReminderType apptReminderType,long clinicNum) {
			PrefName prefName=GetPrefNameForApptReminderType(apptReminderType);
			return GetIsClinicPrefEnabled(prefName,clinicNum);
		}

		private PrefName GetPrefNameForApptReminderType(ApptReminderType apptReminderType) {
			switch(apptReminderType) {
				case ApptReminderType.Reminder:
					return PrefName.ApptReminderUseDefaults;
				case ApptReminderType.ConfirmationFutureDay:
					return PrefName.ApptConfirmUseDefaults;
				case ApptReminderType.ScheduleThankYou:
					return PrefName.ApptThankYouUseDefaults;
				case ApptReminderType.Arrival:
					return PrefName.ApptArrivalUseDefaults;
				case ApptReminderType.PatientPortalInvite:
					return PrefName.PatientPortalInviteUseDefaults;
				case ApptReminderType.GeneralMessage:
					return PrefName.ApptGeneralMessageUseDefaults;
			}
			return PrefName.NotApplicable; //Should probably throw an exception.
		}

		///<summary>Helper function that limits strings to 100 characters and adds an ellipsis.</summary>
		private string GetShortenedNote(string note) {
			if(note.Length<=100) {
				return note;
			}
			return note.Substring(0,100)+"(...)";
		}

		private void ButOK_Click(object sender,EventArgs e) {
			//check for duplicate rules
			List<long> listClinicNums=_listApptReminderRulesAll.Select(x => x.ClinicNum).Distinct().ToList();
			for(int i=0;i<listClinicNums.Count;i++) {
				List<ApptReminderRule> listApptReminderRulesClinicDefaults=_listApptReminderRulesAll			//List of all active default rules for the clinic
					.FindAll(x => x.ClinicNum==listClinicNums[i]
						&& x.Language==string.Empty
						&& x.IsEnabled);																																									
				for(int k=0;k<listApptReminderRulesClinicDefaults.Count;k++) {
					if(listApptReminderRulesClinicDefaults.FindAll(x=>x.TypeCur==listApptReminderRulesClinicDefaults[k].TypeCur 
						&& x.TSPrior==listApptReminderRulesClinicDefaults[k].TSPrior).Count()>1) 
					{
						MessageBox.Show(this,Lans.g(this,"Duplicate rules are not allowed for the same type and days."));
						return;
					}
				}
			}
			SaveToDb();
			DialogResult=DialogResult.OK;	
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}