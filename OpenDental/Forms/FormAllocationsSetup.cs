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
		private bool _changed;
		private YN _ynPrePayAllowedForTpProcs;

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
			//Treatment Plan
			_ynPrePayAllowedForTpProcs=PrefC.GetEnum<YN>(PrefName.PrePayAllowedForTpProcs);
			checkAllowPrePayToTpProcs.Checked=PrefC.GetYN(PrefName.PrePayAllowedForTpProcs);
			checkIsRefundable.Checked=PrefC.GetBool(PrefName.TpPrePayIsNonRefundable);
			checkIsRefundable.Visible=checkAllowPrePayToTpProcs.Checked;//pref will be unchecked if parent gets turned off.
			labelRefundable.Visible=checkAllowPrePayToTpProcs.Checked;
			comboTpUnearnedType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboTpUnearnedType.SetSelectedDefNum(PrefC.GetLong(PrefName.TpUnearnedType));
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
		}

		private void checkAllowPrePayToTpProcs_Click(object sender,EventArgs e) {
			if(checkAllowPrePayToTpProcs.Checked) {
				checkIsRefundable.Visible=true;
				checkIsRefundable.Checked=PrefC.GetBool(PrefName.TpPrePayIsNonRefundable);
				labelRefundable.Visible=true;
				_ynPrePayAllowedForTpProcs=YN.Yes;
			}
			else {
				checkIsRefundable.Visible=false;
				checkIsRefundable.Checked=false;
				labelRefundable.Visible=false;
				_ynPrePayAllowedForTpProcs=YN.No;
			}
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
				_changed=true;
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
				_changed=true;
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rigorous adjustments changed from "+
					((RigorousAdjustments)prefRigorousAdjustments).GetDescription()+" to "
					+rigorousAdjustments.GetDescription()+".");
			}
			_changed|=Prefs.UpdateBool(PrefName.PaymentWindowDefaultHideSplits,checkHidePaysplits.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ShowIncomeTransferManager,checkShowIncomeTransferManager.Checked);
			_changed|=Prefs.UpdateBool(PrefName.ClaimPayByTotalSplitsAuto,checkClaimPayByTotalSplitsAuto.Checked);
			_changed|=Prefs.UpdateYN(PrefName.PrePayAllowedForTpProcs,_ynPrePayAllowedForTpProcs);
			_changed|=Prefs.UpdateLong(PrefName.TpUnearnedType,comboTpUnearnedType.GetSelectedDefNum());
			_changed|=Prefs.UpdateBool(PrefName.TpPrePayIsNonRefundable,checkIsRefundable.Checked);
			if(_changed){
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