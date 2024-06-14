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
	public partial class UserControlEnterpriseFamily:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlEnterpriseFamily() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers

		#region Methods - Event Handlers Sync
		private void textClaimSnapShotRunTime_Validating(object sender,CancelEventArgs e) {
			DateTime timeRun=DateTime.MinValue;//we only use time component
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out timeRun)) {
				MsgBox.Show(this,"Service Snapshot Run Time must be a valid time value.");
				return;
			}
			timeRun=new DateTime(1881,01,01,timeRun.Hour,timeRun.Minute,timeRun.Second);
			//return Prefs.UpdateDateT(PrefName.ClaimSnapshotRunTime,dateTSnapshotRunTime);
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotRunTime);
			prefValSync.PrefVal=POut.DateT(timeRun,false);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void comboClaimSnapshotTriggerType_ChangeCommitted(object sender,EventArgs e) {
			ClaimSnapshotTrigger claimSnapshotTriggerType=comboClaimSnapshotTriggerType.GetSelected<ClaimSnapshotTrigger>();
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			prefValSync.PrefVal=claimSnapshotTriggerType.ToString();
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkShowFeatureSuperFamilies_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ShowFeatureSuperfamilies);
			prefValSync.PrefVal=POut.Bool(checkShowFeatureSuperfamilies.Checked);
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
		///<summary>Checks preferences that take user entry for errors, returns true if all entries are valid.</summary>
		private bool ValidateEntries() {
			string errorMsg="";
			//ClaimSnapshotRuntime
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out DateTime claimSnapshotRunTime)) {
				errorMsg+="Service Snapshot Run Time must be a valid time value.\r\n";
			}
			if(errorMsg!="") {
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return false;
			}
			return true;
		}

		///<summary>Load values from database for hidden preferences if they exist.  If a pref doesn't exist then the corresponding UI is hidden.</summary>
		private void FillHiddenPrefs() {
			FillOptionalPrefBool(checkClaimSnapshotEnabled,PrefName.ClaimSnapshotEnabled);
		}

		///<summary>Returns the ValueString of a pref or null if that pref is not found in the database.</summary>
		private string GetHiddenPrefString(PrefName prefName) {
			Pref prefHidden=null;
			try {
				prefHidden=Prefs.GetOne(prefName);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return null;
			}
			return prefHidden.ValueString;
		}

		///<summary>Helper method for setting UI for boolean preferences.  Some of the preferences calling this may not exist in the database.</summary>
		private void FillOptionalPrefBool(UI.CheckBox checkBox,PrefName prefName) {
			string valueString=GetHiddenPrefString(prefName);
			if(valueString==null) {
				checkBox.Visible=false;
				return;
			}
			checkBox.Checked=PIn.Bool(valueString);
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillEnterpriseFamily() {
			try {
				FillHiddenPrefs();
			}
			catch(Exception ex) {
				ex.DoNothing();//Suppress unhandled exceptions from hidden preferences, since they are read only.
			}
			//checkShowFeatureSuperfamilies.Checked=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies);
			checkShowFeaturePatientClone.Checked=PrefC.GetBool(PrefName.ShowFeaturePatientClone);
			//checkCloneCreateSuperFamily.Checked=PrefC.GetBool(PrefName.CloneCreateSuperFamily);
			checkShowFeeSchedGroups.Checked=PrefC.GetBool(PrefName.ShowFeeSchedGroups);
			//users should only see the snapshot trigger and service runtime if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			//if(PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),isEnumAsString:true)==ClaimSnapshotTrigger.ClaimCreate) {
			//	groupClaimSnapshot.Visible=false;
			//}
			//stays in FillFamilyGeneral() to only hide groupClaimSnapshot upon form open when ClaimCreate is selected
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			ClaimSnapshotTrigger claimSnapshotTrigger=PIn.Enum<ClaimSnapshotTrigger>(prefValSync.PrefVal,isEnumAsString:true);
			if(claimSnapshotTrigger==ClaimSnapshotTrigger.ClaimCreate) {
				groupClaimSnapshot.Visible=false;
			}
			comboClaimSnapshotTriggerType.Items.AddEnums<ClaimSnapshotTrigger>();
			//comboClaimSnapshotTriggerType.SelectedIndex=(int)PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true);
			//textClaimSnapshotRunTime.Text=PrefC.GetDateT(PrefName.ClaimSnapshotRunTime).ToShortTimeString();
		}

		public bool SaveEnterpriseFamily() {
			if(!ValidateEntries()) {
				return false;
			}
			//Changed|=Prefs.UpdateBool(PrefName.ShowFeatureSuperfamilies,checkShowFeatureSuperfamilies.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowFeaturePatientClone,checkShowFeaturePatientClone.Checked);
			//Changed|=Prefs.UpdateBool(PrefName.CloneCreateSuperFamily,checkCloneCreateSuperFamily.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowFeeSchedGroups,checkShowFeeSchedGroups.Checked);
			//Changed|=UpdateClaimSnapshotTrigger();
			//Changed|=UpdateClaimSnapshotRuntime();
			return true;
		}

		public void FillSynced(){
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotRunTime);
			textClaimSnapshotRunTime.Text=PIn.DateT(prefValSync.PrefVal).ToShortTimeString();
			//--------------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			//users should only see the snapshot trigger and service runtime if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			ClaimSnapshotTrigger claimSnapshotTrigger=PIn.Enum<ClaimSnapshotTrigger>(prefValSync.PrefVal,isEnumAsString:true);
			comboClaimSnapshotTriggerType.SetSelectedEnum(claimSnapshotTrigger);
			//--------------------------------------------------------------------------------------------------------------------------------------------------
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ShowFeatureSuperfamilies);
			checkShowFeatureSuperfamilies.Checked=PIn.Bool(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.CloneCreateSuperFamily);
			checkCloneCreateSuperFamily.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}
