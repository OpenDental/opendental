using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlFamilyGeneral:UserControl {
		
		#region Fields - Private
		private YN _usePhonenumTable=YN.Unknown;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Private

		#region Constructors
		public UserControlFamilyGeneral() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butSyncPhNums_Click(object sender,EventArgs e) {
			if(SyncPhoneNums()) {
				MsgBox.Show(this,"Done");
			}
		}

		private void comboCobRule_SelectionChangeCommitted(object sender,EventArgs e) {
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to change the COB rule for all existing insurance plans?")) {
				InsPlans.UpdateCobRuleForAll((EnumCobRule)comboCobRule.SelectedIndex);
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private bool SyncPhoneNums() {
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ShowCancelButton=false;
			progressOD.ActionMain=PhoneNumbers.SyncAllPats;
			progressOD.StartingMessage=Lan.g(this,"Syncing all patient phone numbers to the phonenumber table")+"...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(Lan.g(this,"The patient phone number sync failed with the message")+":\r\n"+ex.Message+"\r\n"+Lan.g(this,"Please try again."));
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			_usePhonenumTable=YN.Yes;//so it won't sync again if you clicked the button
			return true;
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillFamilyGeneral() {
			for(int i=0;i<Enum.GetNames(typeof(EnumCobRule)).Length;i++) {
				comboCobRule.Items.Add(Lan.g("enumEnumCobRule",Enum.GetNames(typeof(EnumCobRule))[i]));
			}
			comboCobRule.SelectedIndex=PrefC.GetInt(PrefName.InsDefaultCobRule);
			List<EclaimCobInsPaidBehavior> listCobs=Enum.GetValues(typeof(EclaimCobInsPaidBehavior)).Cast<EclaimCobInsPaidBehavior>().ToList();
			listCobs.Remove(EclaimCobInsPaidBehavior.Default);//Exclude Default option, as it is only for the carrier edit window.
			//The following line is similar to ComboBoxPlus.AddEnums(), except we need to exclude Default enum value, thus we are forced to mimic.
			comboCobSendPaidByInsAt.Items.AddList(listCobs,x=>Lan.g("enum"+typeof(EclaimCobInsPaidBehavior).Name,ODPrimitiveExtensions.GetDescription(x)));
			comboCobSendPaidByInsAt.SetSelectedEnum(PrefC.GetEnum<EclaimCobInsPaidBehavior>(PrefName.ClaimCobInsPaidBehavior));
			checkTextMsgOkStatusTreatAsNo.Checked=PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo);
			checkFamPhiAccess.Checked=PrefC.GetBool(PrefName.FamPhiAccess);
			checkGoogleAddress.Checked=PrefC.GetBool(PrefName.ShowFeatureGoogleMaps);
			checkSelectProv.Checked=PrefC.GetBool(PrefName.PriProvDefaultToSelectProv);
			if(!PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				groupBoxSuperFamily.Visible=false;
			}
			else {
				foreach(SortStrategy option in Enum.GetValues(typeof(SortStrategy))) {
					comboSuperFamSort.Items.Add(option.GetDescription());
				}
				comboSuperFamSort.SelectedIndex=PrefC.GetInt(PrefName.SuperFamSortStrategy);
				checkSuperFamSync.Checked=PrefC.GetBool(PrefName.PatientAllSuperFamilySync);
				checkSuperFamAddIns.Checked=PrefC.GetBool(PrefName.SuperFamNewPatAddIns);
				checkSuperFamCloneCreate.Checked=PrefC.GetBool(PrefName.CloneCreateSuperFamily);
			}
			//users should only see the claimsnapshot tab page if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			if(PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true) == ClaimSnapshotTrigger.ClaimCreate) {
				groupBoxClaimSnapshot.Visible=false;
			}
			foreach(ClaimSnapshotTrigger trigger in Enum.GetValues(typeof(ClaimSnapshotTrigger))) {
				comboClaimSnapshotTrigger.Items.Add(trigger.GetDescription());
			}
			comboClaimSnapshotTrigger.SelectedIndex=(int)PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true);
			textClaimSnapshotRunTime.Text=PrefC.GetDateT(PrefName.ClaimSnapshotRunTime).ToShortTimeString();
			checkPreferredReferrals.Checked=PrefC.GetBool(PrefName.ShowPreferedReferrals);
			checkAutoFillPatEmail.Checked=PrefC.GetBool(PrefName.AddFamilyInheritsEmail);
			if(!PrefC.HasClinicsEnabled) {
				checkAllowPatsAtHQ.Visible=false;
			}
			checkAllowPatsAtHQ.Checked=PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters);
			checkPatientSSNMasked.Checked=PrefC.GetBool(PrefName.PatientSSNMasked);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				checkPatientSSNMasked.Text=Lan.g(this,"Mask patient Social Insurance Numbers");
			}
			checkPatientDOBMasked.Checked=PrefC.GetBool(PrefName.PatientDOBMasked);
			checkSameForFamily.Checked=PrefC.GetBool(PrefName.SameForFamilyCheckboxesUnchecked);
			_usePhonenumTable=PrefC.GetEnum<YN>(PrefName.PatientPhoneUsePhonenumberTable);
			if(_usePhonenumTable!=YN.Unknown) {
				checkUsePhoneNumTable.CheckState=CheckState.Unchecked;
				checkUsePhoneNumTable.Checked=_usePhonenumTable==YN.Yes;
			}
			checkPreferredPronouns.Checked=PrefC.GetBool(PrefName.ShowPreferredPronounsForPats);
		}

		public bool SaveFamilyGeneral() {
			DateTime claimSnapshotRunTime=DateTime.MinValue;
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out claimSnapshotRunTime)) {
				MsgBox.Show(this,"Service Snapshot Run Time must be a valid time value.");
				return false;
			}
			if(_usePhonenumTable!=YN.Yes && checkUsePhoneNumTable.CheckState==CheckState.Checked) {//use CheckState since it can be indeterminate
				string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
					"take a couple minutes.  Continue?";
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
					return false;
				}
				if(!SyncPhoneNums()) {
					return false;
				}
				MsgBox.Show(this,"Done");
			}
			if(checkUsePhoneNumTable.CheckState!=CheckState.Indeterminate) {
				Changed|=Prefs.UpdateYN(PrefName.PatientPhoneUsePhonenumberTable,checkUsePhoneNumTable.Checked ? YN.Yes : YN.No);
			}
			claimSnapshotRunTime=new DateTime(1881,01,01,claimSnapshotRunTime.Hour,claimSnapshotRunTime.Minute,claimSnapshotRunTime.Second);
			Changed|=Prefs.UpdateInt(PrefName.InsDefaultCobRule,comboCobRule.SelectedIndex);
			Changed|=Prefs.UpdateInt(PrefName.ClaimCobInsPaidBehavior,(int)comboCobSendPaidByInsAt.GetSelected<EclaimCobInsPaidBehavior>());
			Changed|=Prefs.UpdateBool(PrefName.TextMsgOkStatusTreatAsNo,checkTextMsgOkStatusTreatAsNo.Checked);
			Changed|=Prefs.UpdateBool(PrefName.FamPhiAccess,checkFamPhiAccess.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowFeatureGoogleMaps,checkGoogleAddress.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PriProvDefaultToSelectProv,checkSelectProv.Checked);
			Changed|=Prefs.UpdateInt(PrefName.SuperFamSortStrategy,comboSuperFamSort.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.PatientAllSuperFamilySync,checkSuperFamSync.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SuperFamNewPatAddIns,checkSuperFamAddIns.Checked);
			Changed|=Prefs.UpdateBool(PrefName.CloneCreateSuperFamily,checkSuperFamCloneCreate.Checked);
			Changed|=Prefs.UpdateDateT(PrefName.ClaimSnapshotRunTime,claimSnapshotRunTime);
			Changed|=Prefs.UpdateBool(PrefName.ShowPreferedReferrals,checkPreferredReferrals.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AddFamilyInheritsEmail,checkAutoFillPatEmail.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClinicAllowPatientsAtHeadquarters,checkAllowPatsAtHQ.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PatientSSNMasked,checkPatientSSNMasked.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PatientDOBMasked,checkPatientDOBMasked.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SameForFamilyCheckboxesUnchecked,checkSameForFamily.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowPreferredPronounsForPats,checkPreferredPronouns.Checked);
			foreach(ClaimSnapshotTrigger trigger in Enum.GetValues(typeof(ClaimSnapshotTrigger))) {
				if(trigger.GetDescription()==comboClaimSnapshotTrigger.Text) {
					if(Prefs.UpdateString(PrefName.ClaimSnapshotTriggerType,trigger.ToString())) {
						Changed=true;
					}
					break;
				}
			}
			return true;
		}
		#endregion Methods - Public
	}
}
