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
	public partial class UserControlAccountClaimReceive:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountClaimReceive() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void butReplacements_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			FrmMessageReplacements frmMessageReplacements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplacements.IsSelectionMode=true;
			frmMessageReplacements.ShowDialog();
			if(frmMessageReplacements.IsDialogCancel) {
				return;
			}
		}

		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountClaimReceive() {
			checkProviderIncomeShows.Checked=PrefC.GetBool(PrefName.ProviderIncomeTransferShows);
			checkAllowProcAdjFromClaim.Checked=PrefC.GetBool(PrefName.AllowProcAdjFromClaim);
			comboClaimCredit.Items.AddList(Enum.GetNames(typeof(ClaimProcCreditsGreaterThanProcFee)));
			comboClaimCredit.SelectedIndex=PrefC.GetInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee);
			//textClaimIdPrefix.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			checkClaimTrackingExcludeNone.Checked=PrefC.GetBool(PrefName.ClaimTrackingStatusExcludesNone);
			checkInsPayNoWriteoffMoreThanProc.Checked=PrefC.GetBool(PrefName.InsPayNoWriteoffMoreThanProc);
			checkNoInitialPrimaryInsMoreThanProc.Checked=PrefC.GetBool(PrefName.InsPayNoInitialPrimaryMoreThanProc);
			checkPromptForSecondaryClaim.Checked=PrefC.GetBool(PrefName.PromptForSecondaryClaim);
			checkInsEstRecalcReceived.Checked=PrefC.GetBool(PrefName.InsEstRecalcReceived);
			checkShowClaimPayTracking.Checked=PrefC.GetBool(PrefName.ClaimEditShowPayTracking);
			checkShowClaimPatResp.Checked=PrefC.GetBool(PrefName.ClaimEditShowPatResponsibility);
			checkClaimPrimaryRecievedForceSecondaryStatus.Checked=PrefC.GetBool(PrefName.ClaimPrimaryRecievedForceSecondaryStatus);
			checkInsAutoReceiveNoAssign.Checked=PrefC.GetBool(PrefName.InsAutoReceiveNoAssign);
			checkClaimPaymentPickStatementType.Checked=PrefC.GetBool(PrefName.ClaimPaymentPickStatementType);
			checkClaimFinalizeWarning.Checked=PrefC.GetBool(PrefName.ClaimFinalizeWarning);
			checkClaimPrimaryReceivedRecalcSecondary.Checked=PrefC.GetBool(PrefName.ClaimPrimaryReceivedRecalcSecondary);
			checkClaimMedReceivedPromptForPrimaryClaim.Checked=PrefC.GetBool(PrefName.ClaimMedReceivedPromptForPrimaryClaim);
			checkClaimMedReceivedForcePrimaryStatus.Checked=PrefC.GetBool(PrefName.ClaimMedReceivedForcePrimaryStatus);
		}

		public bool SaveAccountClaimReceive() {
			Changed|=Prefs.UpdateBool(PrefName.ProviderIncomeTransferShows,checkProviderIncomeShows.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowProcAdjFromClaim,checkAllowProcAdjFromClaim.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee,comboClaimCredit.SelectedIndex);
			//Changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdPrefix.Text);
			Changed|=Prefs.UpdateBool(PrefName.ClaimTrackingStatusExcludesNone,checkClaimTrackingExcludeNone.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPayNoWriteoffMoreThanProc,checkInsPayNoWriteoffMoreThanProc.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPayNoInitialPrimaryMoreThanProc,checkNoInitialPrimaryInsMoreThanProc.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PromptForSecondaryClaim,checkPromptForSecondaryClaim.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsEstRecalcReceived,checkInsEstRecalcReceived.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimEditShowPayTracking,checkShowClaimPayTracking.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimEditShowPatResponsibility,checkShowClaimPatResp.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPrimaryRecievedForceSecondaryStatus,checkClaimPrimaryRecievedForceSecondaryStatus.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsAutoReceiveNoAssign,checkInsAutoReceiveNoAssign.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPaymentPickStatementType,checkClaimPaymentPickStatementType.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimFinalizeWarning,checkClaimFinalizeWarning.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPrimaryReceivedRecalcSecondary,checkClaimPrimaryReceivedRecalcSecondary.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedReceivedPromptForPrimaryClaim,checkClaimMedReceivedPromptForPrimaryClaim.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedReceivedForcePrimaryStatus,checkClaimMedReceivedForcePrimaryStatus.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
