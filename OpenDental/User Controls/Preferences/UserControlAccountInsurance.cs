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
	public partial class UserControlAccountInsurance:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountInsurance() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butReplacements_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			using FormMessageReplacements form=new FormMessageReplacements(listMessageReplaceTypes);
			form.IsSelectionMode=true;
			form.ShowDialog();
			if(form.DialogResult!=DialogResult.OK) {
				return;
			}
			textClaimIdentifier.Focus();
			int cursorIndex=textClaimIdentifier.SelectionStart;
			textClaimIdentifier.Text=textClaimIdentifier.Text.Insert(cursorIndex,form.Replacement);
			textClaimIdentifier.SelectionStart=cursorIndex+form.Replacement.Length;
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountInsurance() {
			checkPpoUseUcr.Checked=PrefC.GetBool(PrefName.InsPpoAlwaysUseUcrFee);
			checkProviderIncomeShows.Checked=PrefC.GetBool(PrefName.ProviderIncomeTransferShows);
			checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked=PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical);
			checkClaimFormTreatDentSaysSigOnFile.Checked=PrefC.GetBool(PrefName.ClaimFormTreatDentSaysSigOnFile);
			textInsWriteoffDescript.Text=PrefC.GetString(PrefName.InsWriteoffDescript);
			textClaimAttachPath.Text=PrefC.GetString(PrefName.ClaimAttachExportPath);
			checkEclaimsMedicalProvTreatmentAsOrdering.Checked=PrefC.GetBool(PrefName.ClaimMedProvTreatmentAsOrdering);
			checkEclaimsSeparateTreatProv.Checked=PrefC.GetBool(PrefName.EclaimsSeparateTreatProv);
			checkClaimsValidateACN.Checked=PrefC.GetBool(PrefName.ClaimsValidateACN);
			checkAllowProcAdjFromClaim.Checked=PrefC.GetBool(PrefName.AllowProcAdjFromClaim);
			comboClaimCredit.Items.AddList(Enum.GetNames(typeof(ClaimProcCreditsGreaterThanProcFee)));
			comboClaimCredit.SelectedIndex=PrefC.GetInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee);
			textClaimIdentifier.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			foreach(ClaimZeroDollarProcBehavior procBehavior in Enum.GetValues(typeof(ClaimZeroDollarProcBehavior))) {
				comboZeroDollarProcClaimBehavior.Items.Add(Lan.g(this,procBehavior.ToString()));
			}
			comboZeroDollarProcClaimBehavior.SelectedIndex=PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior);
			checkClaimTrackingExcludeNone.Checked=PrefC.GetBool(PrefName.ClaimTrackingStatusExcludesNone);
			checkInsPayNoWriteoffMoreThanProc.Checked=PrefC.GetBool(PrefName.InsPayNoWriteoffMoreThanProc);
			checkNoInitialPrimaryInsMoreThanProc.Checked=PrefC.GetBool(PrefName.InsPayNoInitialPrimaryMoreThanProc);
			checkPromptForSecondaryClaim.Checked=PrefC.GetBool(PrefName.PromptForSecondaryClaim);
			checkInsEstRecalcReceived.Checked=PrefC.GetBool(PrefName.InsEstRecalcReceived);
			checkPriClaimAllowSetToHoldUntilPriReceived.Checked=PrefC.GetBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived);
			checkShowClaimPayTracking.Checked=PrefC.GetBool(PrefName.ClaimEditShowPayTracking);
			checkShowClaimPatResp.Checked=PrefC.GetBool(PrefName.ClaimEditShowPatResponsibility);
			checkCanadianPpoLabEst.Checked=PrefC.GetBool(PrefName.CanadaCreatePpoLabEst);
			checkClaimPrimaryRecievedForceSecondaryStatus.Checked=PrefC.GetBool(PrefName.ClaimPrimaryRecievedForceSecondaryStatus);
			checkInsAutoReceiveNoAssign.Checked=PrefC.GetBool(PrefName.InsAutoReceiveNoAssign);
			checkClaimPaymentPickStatementType.Checked=PrefC.GetBool(PrefName.ClaimPaymentPickStatementType);
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				checkCanadianPpoLabEst.Visible=false;
			}
		}

		public bool SaveAccountInsurance() {
			Changed|=Prefs.UpdateBool(PrefName.InsPpoAlwaysUseUcrFee,checkPpoUseUcr.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProviderIncomeTransferShows,checkProviderIncomeShows.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimFormTreatDentSaysSigOnFile,checkClaimFormTreatDentSaysSigOnFile.Checked);
			Changed|=Prefs.UpdateString(PrefName.ClaimAttachExportPath,textClaimAttachPath.Text);
			Changed|=Prefs.UpdateBool(PrefName.EclaimsSeparateTreatProv,checkEclaimsSeparateTreatProv.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimsValidateACN,checkClaimsValidateACN.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical,checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked);
			Changed|=Prefs.UpdateString(PrefName.InsWriteoffDescript,textInsWriteoffDescript.Text);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedProvTreatmentAsOrdering,checkEclaimsMedicalProvTreatmentAsOrdering.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowProcAdjFromClaim,checkAllowProcAdjFromClaim.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee,comboClaimCredit.SelectedIndex);
			Changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdentifier.Text);
			Changed|=Prefs.UpdateInt(PrefName.ClaimZeroDollarProcBehavior,comboZeroDollarProcClaimBehavior.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.ClaimTrackingStatusExcludesNone,checkClaimTrackingExcludeNone.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPayNoWriteoffMoreThanProc,checkInsPayNoWriteoffMoreThanProc.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPayNoInitialPrimaryMoreThanProc,checkNoInitialPrimaryInsMoreThanProc.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PromptForSecondaryClaim,checkPromptForSecondaryClaim.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsEstRecalcReceived,checkInsEstRecalcReceived.Checked);
			Changed|=Prefs.UpdateBool(PrefName.CanadaCreatePpoLabEst,checkCanadianPpoLabEst.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived,checkPriClaimAllowSetToHoldUntilPriReceived.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimEditShowPayTracking,checkShowClaimPayTracking.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimEditShowPatResponsibility,checkShowClaimPatResp.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPrimaryRecievedForceSecondaryStatus,checkClaimPrimaryRecievedForceSecondaryStatus.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsAutoReceiveNoAssign,checkInsAutoReceiveNoAssign.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPaymentPickStatementType,checkClaimPaymentPickStatementType.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
