using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlFamilyGeneral:UserControl {
		
		#region Fields - Private
		private bool _doUsePhonenumTable=false;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Private

		#region Constructors
		public UserControlFamilyGeneral() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void butSyncPhNums_Click(object sender,EventArgs e) {
			if(SyncPhoneNums()) {
				MsgBox.Show(this,"Done");
			}
		}

		#region Methods - Event Handlers Sync
		private void checkPatientPhoneUsePhoneNumberTable_Click(object sender,EventArgs e) {
			if(!_doUsePhonenumTable && checkPatientPhoneUsePhonenumberTable.Checked) {
				string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
					"take a couple minutes.  Continue?";
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
					if(SyncPhoneNums()) {
						MsgBox.Show(this,"Done");
					}
				}
			}
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientPhoneUsePhonenumberTable);
			prefValSync.PrefVal=POut.Bool(checkPatientPhoneUsePhonenumberTable.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textClaimSnapShotRunTime_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotRunTime);
			DateTime timeClaimRun=DateTime.MinValue;
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out timeClaimRun)) {
				MsgBox.Show(this,"Service Snapshot Run Time must be a valid time value.");
				return;
			}
			timeClaimRun=new DateTime(1881,01,01,timeClaimRun.Hour,timeClaimRun.Minute,timeClaimRun.Second);
			prefValSync.PrefVal=POut.DateT(timeClaimRun,false);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void comboClaimSnapshotTriggerType_ChangeCommitted(object sender,EventArgs e) {
			ClaimSnapshotTrigger claimSnapshotTriggerType=comboClaimSnapshotTriggerType.GetSelected<ClaimSnapshotTrigger>();
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			prefValSync.PrefVal=claimSnapshotTriggerType.ToString();
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkCloneCreateSuperFamily_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.CloneCreateSuperFamily);
			prefValSync.PrefVal=POut.Bool(checkCloneCreateSuperFamily.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		private bool SyncPhoneNums() {
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;
			progressOD.ActionMain=PhoneNumbers.SyncAllPats;
			progressOD.StartingMessage=Lan.g(this,"Syncing all patient phone numbers to the phonenumber table")+"...";
			try{
				progressOD.ShowDialog();
			}
			catch(Exception ex){
				MsgBox.Show(Lan.g(this,"The patient phone number sync failed with the message")+":\r\n"+ex.Message+"\r\n"+Lan.g(this,"Please try again."));
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			_doUsePhonenumTable=true;//so it won't sync again if you clicked the button
			return true;
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillFamilyGeneral() {
			checkTextMsgOkStatusTreatAsNo.Checked=PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo);
			checkFamPhiAccess.Checked=PrefC.GetBool(PrefName.FamPhiAccess);
			checkGoogleAddress.Checked=PrefC.GetBool(PrefName.ShowFeatureGoogleMaps);
			checkSelectProv.Checked=PrefC.GetBool(PrefName.PriProvDefaultToSelectProv);
			if(!PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {//Keep if here so values only get set to prefName values upon FillFamilyGeneral()
				groupBoxSuperFamily.Visible=false;
			}
			//always set superFamily controls in case groupBox becomes visible after formPreferences is first opened
			foreach(SortStrategy option in Enum.GetValues(typeof(SortStrategy))) {
				comboSuperFamSort.Items.Add(option.GetDescription());
			}
			comboSuperFamSort.SelectedIndex=PrefC.GetInt(PrefName.SuperFamSortStrategy);
			checkSuperFamSync.Checked=PrefC.GetBool(PrefName.PatientAllSuperFamilySync);
			checkSuperFamAddIns.Checked=PrefC.GetBool(PrefName.SuperFamNewPatAddIns);
				//checkCloneCreateSuperFamily.Checked=PrefC.GetBool(PrefName.CloneCreateSuperFamily);
			//users should only see the claimsnapshot tab page if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			//if(PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true) == ClaimSnapshotTrigger.ClaimCreate) {
			//	groupBoxClaimSnapshot.Visible=false;
			//}
			//stays in FillFamilyGeneral() to only hide groupBoxClaimSnapshot upon form open when ClaimCreate is selected
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			ClaimSnapshotTrigger claimSnapshotTrigger=PIn.Enum<ClaimSnapshotTrigger>(prefValSync.PrefVal,isEnumAsString:true);
			if(claimSnapshotTrigger==ClaimSnapshotTrigger.ClaimCreate) {
				groupBoxClaimSnapshot.Visible=false;
			}
			comboClaimSnapshotTriggerType.Items.AddEnums<ClaimSnapshotTrigger>();
			//comboClaimSnapshotTriggerType.SelectedIndex=(int)PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true);
			//textClaimSnapshotRunTime.Text=PrefC.GetDateT(PrefName.ClaimSnapshotRunTime).ToShortTimeString();
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
			//_doUsePhonenumTable=PrefC.GetBool(PrefName.PatientPhoneUsePhonenumberTable);
			//checkPatientPhoneUsePhonenumberTable.Checked=_doUsePhonenumTable;
			checkPreferredPronouns.Checked=PrefC.GetBool(PrefName.ShowPreferredPronounsForPats);
			checkAddressVerifyWithUSPS.Checked=PrefC.GetBool(PrefName.AddressVerifyWithUSPS);
		}

		public bool SaveFamilyGeneral() {
			DateTime claimSnapshotRunTime=DateTime.MinValue;
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out claimSnapshotRunTime)) {
				MsgBox.Show(this,"Service Snapshot Run Time must be a valid time value.");
				return false;
			}
			//if(!_doUsePhonenumTable && checkPatientPhoneUsePhonenumberTable.Checked) {
			//	string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
			//		"take a couple minutes.  Continue?";
			//	if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
			//		return false;
			//	}
			//	if(!SyncPhoneNums()) {
			//		return false;
			//	}
			//	MsgBox.Show(this,"Done");
			//}
			//Changed|=Prefs.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,checkPatientPhoneUsePhonenumberTable.Checked);
			//claimSnapshotRunTime=new DateTime(1881,01,01,claimSnapshotRunTime.Hour,claimSnapshotRunTime.Minute,claimSnapshotRunTime.Second);
			Changed|=Prefs.UpdateBool(PrefName.TextMsgOkStatusTreatAsNo,checkTextMsgOkStatusTreatAsNo.Checked);
			Changed|=Prefs.UpdateBool(PrefName.FamPhiAccess,checkFamPhiAccess.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowFeatureGoogleMaps,checkGoogleAddress.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PriProvDefaultToSelectProv,checkSelectProv.Checked);
			if(groupBoxSuperFamily.Visible) {//don't save if the groupBox was hidden after editing
				Changed|=Prefs.UpdateInt(PrefName.SuperFamSortStrategy,comboSuperFamSort.SelectedIndex);
				Changed|=Prefs.UpdateBool(PrefName.PatientAllSuperFamilySync,checkSuperFamSync.Checked);
				Changed|=Prefs.UpdateBool(PrefName.SuperFamNewPatAddIns,checkSuperFamAddIns.Checked);
			}
			//Changed|=Prefs.UpdateBool(PrefName.CloneCreateSuperFamily,checkCloneCreateSuperFamily.Checked);
			//Changed|=Prefs.UpdateDateT(PrefName.ClaimSnapshotRunTime,claimSnapshotRunTime);
			Changed|=Prefs.UpdateBool(PrefName.ShowPreferedReferrals,checkPreferredReferrals.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AddFamilyInheritsEmail,checkAutoFillPatEmail.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClinicAllowPatientsAtHeadquarters,checkAllowPatsAtHQ.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PatientSSNMasked,checkPatientSSNMasked.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PatientDOBMasked,checkPatientDOBMasked.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SameForFamilyCheckboxesUnchecked,checkSameForFamily.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowPreferredPronounsForPats,checkPreferredPronouns.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AddressVerifyWithUSPS,checkAddressVerifyWithUSPS.Checked);
			//foreach(ClaimSnapshotTrigger claimSnapshotTriggerType in Enum.GetValues(typeof(ClaimSnapshotTrigger))) {
			//	if(claimSnapshotTriggerType.GetDescription()==comboClaimSnapshotTriggerType.Text) {
			//		if(Prefs.UpdateString(PrefName.ClaimSnapshotTriggerType,claimSnapshotTriggerType.ToString())) {
			//			Changed=true;
			//		}
			//		break;
			//	}
			//}
			return true;
		}

		public void FillSynced(){
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientPhoneUsePhonenumberTable);
			_doUsePhonenumTable=PIn.Bool(prefValSync.PrefVal);
			checkPatientPhoneUsePhonenumberTable.Checked=_doUsePhonenumTable;
			//-------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotRunTime);
			textClaimSnapshotRunTime.Text=PIn.DateT(prefValSync.PrefVal).ToShortTimeString();
			//-------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			//users should only see the claimsnapshot tab page if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			ClaimSnapshotTrigger claimSnapshotTrigger=PIn.Enum<ClaimSnapshotTrigger>(prefValSync.PrefVal,isEnumAsString:true);
			comboClaimSnapshotTriggerType.SetSelectedEnum(claimSnapshotTrigger);
			//-------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ShowFeatureSuperfamilies);
			if(!PIn.Bool(prefValSync.PrefVal)) {
				groupBoxSuperFamily.Visible=false;
			}
			else {
				groupBoxSuperFamily.Visible=true;
			}
			//-------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.CloneCreateSuperFamily);
			checkCloneCreateSuperFamily.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}