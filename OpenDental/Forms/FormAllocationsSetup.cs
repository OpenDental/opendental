using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormAllocationsSetup:FormODBase {
		private bool _didChange;
		private YN _yNPrePayAllowedForTpProcs;

		public FormAllocationsSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAllocationsSetup_Load(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup)) {
				labelPermission.Visible=false;
			}
			else{
				butOK.Enabled=false;
			}
			RigorousAccounting rigorousAccounting=(RigorousAccounting)PrefC.GetInt(PrefName.RigorousAccounting);
			switch(rigorousAccounting){
				case RigorousAccounting.EnforceFully:
					radioPayEnforce.Checked=true;
					break;
				case RigorousAccounting.AutoSplitOnly:
					radioPayAuto.Checked=true;
					break;
				case RigorousAccounting.DontEnforce:
					radioPayDont.Checked=true;
					break;
			}
			RigorousAdjustments rigorousAdjustments=(RigorousAdjustments)PrefC.GetInt(PrefName.RigorousAdjustments);
			switch(rigorousAdjustments){
				case RigorousAdjustments.EnforceFully:
					radioAdjustEnforce.Checked=true;
					break;
				case RigorousAdjustments.LinkOnly:
					radioAdjustLink.Checked=true;
					break;
				case RigorousAdjustments.DontEnforce:
					radioAdjustDont.Checked=true;
					break;
			}
			checkHidePaysplits.Checked=PrefC.GetBool(PrefName.PaymentWindowDefaultHideSplits);
			checkShowIncomeTransferManager.Checked=PrefC.GetBool(PrefName.ShowIncomeTransferManager);
			checkClaimPayByTotalSplitsAuto.Checked=PrefC.GetBool(PrefName.ClaimPayByTotalSplitsAuto);
			checkAdjustmentsOffset.Checked=PrefC.GetBool(PrefName.AdjustmentsOffsetEachOther);
			//Treatment Plan
			_yNPrePayAllowedForTpProcs=PrefC.GetEnum<YN>(PrefName.PrePayAllowedForTpProcs);
			YN yNAutoTransferOnClaimReceive=PrefC.GetEnum<YN>(PrefName.IncomeTransfersMadeUponClaimReceived);
			switch(yNAutoTransferOnClaimReceive) {
				case YN.Unknown:
					checkIncomeTransfersMadeUponClaimReceived.CheckState=CheckState.Indeterminate;
					break;
				case YN.Yes:
					checkIncomeTransfersMadeUponClaimReceived.CheckState=CheckState.Checked;
					break;
				case YN.No:
					checkIncomeTransfersMadeUponClaimReceived.CheckState=CheckState.Unchecked;
					break;
			}
			SetIncomeTransfersMadeUponClaimReceivedDesc();
			checkAllowPrePayToTpProcs.Checked=PrefC.GetYN(PrefName.PrePayAllowedForTpProcs);
			checkIsRefundable.Checked=PrefC.GetBool(PrefName.TpPrePayIsNonRefundable);
			checkIsRefundable.Visible=checkAllowPrePayToTpProcs.Checked;//pref will be unchecked if parent gets turned off.
			labelRefundable.Visible=checkAllowPrePayToTpProcs.Checked;
			comboTpUnearnedType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboTpUnearnedType.SetSelectedDefNum(PrefC.GetLong(PrefName.TpUnearnedType));
		}

		private void SetIncomeTransfersMadeUponClaimReceivedDesc() {
			//Make claim specific income transfers when received
			if(checkIncomeTransfersMadeUponClaimReceived.CheckState==CheckState.Checked) {
				labelIncomeTransfersMadeUponClaimReceivedDesc.Text=Lan.g(this,"Automatically transfer patient overpayment when necessary.");
			}
			if(checkIncomeTransfersMadeUponClaimReceived.CheckState==CheckState.Unchecked) {
				labelIncomeTransfersMadeUponClaimReceivedDesc.Text=Lan.g(this,"Never make transfers automatically.");
			}
			if(checkIncomeTransfersMadeUponClaimReceived.CheckState==CheckState.Indeterminate) {
				labelIncomeTransfersMadeUponClaimReceivedDesc.Text=Lan.g(this,"Only transfer patient overpayment if Paysplits - Rigorous is enabled.");
			}
		}

		private void butLineItem_Click(object sender, EventArgs e){
			radioPayEnforce.Checked=true;
			radioAdjustEnforce.Checked=true;
			checkAllowPrePayToTpProcs.Checked=false;
			checkIsRefundable.Checked=false;
			checkIsRefundable.Visible=false;
			labelRefundable.Visible=false;
			checkHidePaysplits.Checked=false;
			checkShowIncomeTransferManager.Checked=true;
			checkClaimPayByTotalSplitsAuto.Checked=true;
			checkAdjustmentsOffset.Checked=true;
		}

		private void butDefault_Click(object sender, EventArgs e){
			radioPayAuto.Checked=true;
			radioAdjustLink.Checked=true;
			checkAllowPrePayToTpProcs.Checked=false;
			checkIsRefundable.Checked=false;
			checkIsRefundable.Visible=false;
			labelRefundable.Visible=false;
			checkHidePaysplits.Checked=false;
			checkShowIncomeTransferManager.Checked=true;
			checkClaimPayByTotalSplitsAuto.Checked=true;
			checkAdjustmentsOffset.Checked=true;
		}

		private void butSimple_Click(object sender, EventArgs e){
			radioPayDont.Checked=true;
			radioAdjustDont.Checked=true;
			checkAllowPrePayToTpProcs.Checked=false;
			checkIsRefundable.Checked=false;
			checkIsRefundable.Visible=false;
			labelRefundable.Visible=false;
			checkHidePaysplits.Checked=false;
			checkShowIncomeTransferManager.Checked=false;
			checkClaimPayByTotalSplitsAuto.Checked=true;
			checkAdjustmentsOffset.Checked=true;
		}

		private void checkAllowPrePayToTpProcs_Click(object sender,EventArgs e) {
			if(checkAllowPrePayToTpProcs.Checked) {
				checkIsRefundable.Visible=true;
				checkIsRefundable.Checked=PrefC.GetBool(PrefName.TpPrePayIsNonRefundable);
				labelRefundable.Visible=true;
				_yNPrePayAllowedForTpProcs=YN.Yes;
			}
			else {
				checkIsRefundable.Visible=false;
				checkIsRefundable.Checked=false;
				labelRefundable.Visible=false;
				_yNPrePayAllowedForTpProcs=YN.No;
			}
		}

		private void checkAutoIncomeTransfer_CheckedStateChanged(object sender,EventArgs e) {
			SetIncomeTransfersMadeUponClaimReceivedDesc();
		}

		private void butOK_Click(object sender,EventArgs e) {
			RigorousAccounting rigorousAccounting=RigorousAccounting.EnforceFully;
			if(radioPayAuto.Checked){
				rigorousAccounting=RigorousAccounting.AutoSplitOnly;
			}
			if(radioPayDont.Checked){
				rigorousAccounting=RigorousAccounting.DontEnforce;
			}
			int prefRigorousAccounting=PrefC.GetInt(PrefName.RigorousAccounting);
			if(Prefs.UpdateInt(PrefName.RigorousAccounting,(int)rigorousAccounting)) {
				_didChange=true;
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rigorous accounting changed from "+
					((RigorousAccounting)prefRigorousAccounting).GetDescription()+" to "
					+rigorousAccounting.GetDescription()+".");
			}
			RigorousAdjustments rigorousAdjustments=RigorousAdjustments.EnforceFully;
			if(radioAdjustLink.Checked){
				rigorousAdjustments=RigorousAdjustments.LinkOnly;
			}
			if(radioAdjustDont.Checked){
				rigorousAdjustments=RigorousAdjustments.DontEnforce;
			}
			int prefRigorousAdjustments=PrefC.GetInt(PrefName.RigorousAdjustments);
			if(Prefs.UpdateInt(PrefName.RigorousAdjustments,(int)rigorousAdjustments)) {
				_didChange=true;
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rigorous adjustments changed from "+
					((RigorousAdjustments)prefRigorousAdjustments).GetDescription()+" to "
					+rigorousAdjustments.GetDescription()+".");
			}
			_didChange|=Prefs.UpdateBool(PrefName.PaymentWindowDefaultHideSplits,checkHidePaysplits.Checked);
			_didChange|=Prefs.UpdateBool(PrefName.ShowIncomeTransferManager,checkShowIncomeTransferManager.Checked);
			_didChange|=Prefs.UpdateBool(PrefName.ClaimPayByTotalSplitsAuto,checkClaimPayByTotalSplitsAuto.Checked);
			_didChange|=Prefs.UpdateYN(PrefName.PrePayAllowedForTpProcs,_yNPrePayAllowedForTpProcs);
			_didChange|=Prefs.UpdateYN(PrefName.IncomeTransfersMadeUponClaimReceived,checkIncomeTransfersMadeUponClaimReceived.CheckState);
			_didChange|=Prefs.UpdateLong(PrefName.TpUnearnedType,comboTpUnearnedType.GetSelectedDefNum());
			_didChange|=Prefs.UpdateBool(PrefName.TpPrePayIsNonRefundable,checkIsRefundable.Checked);
			_didChange|=Prefs.UpdateBool(PrefName.AdjustmentsOffsetEachOther,checkAdjustmentsOffset.Checked);
			if(_didChange){
				DataValid.SetInvalid(InvalidType.Prefs);
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Auto Codes");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}