using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlFamilyInsurance:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlFamilyInsurance() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butCoPayFeeScheduleBlankLikeZero_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"Set how blank entries in copay fee schedules are handled.\r\n\r\n" +
				"Checked: Blank entries are treated as 0.\r\n" +
				"Example: UCR =$200, Contracted = $150, Write-off = 50, Copay = blank, Percentage = %100, Patient Portion = $0\r\n\r\n" +
				"Unchecked: Blank entries are treated as 100% copay.\r\n" + 
				"Example: UCR =$200, Contracted = $150, Write-off = 50, Copay = blank, Percentage = %100, Patient Portion = $150");
		}

		private void butFixedBenefitBlankLikeZero_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"Set how blank entries in fixed benefit fee schedules are handled.\r\n\r\n" +
				"Checked: Blank entries are treated as 0.\r\n" +
				"Example: UCR =$200, PPO Fee = $150, Write-off = 50, Fixed = blank, Percentage = %100, Patient Portion = $150\r\n\r\n" +
				"Unchecked: Blank entries are treated as 100% the PPO fee.\r\n" +
				"Example: UCR =$200, PPO Fee = $150, Write-off = 50, Fixed = blank, Percentage = %100, Patient Portion = $0");
		}

		private void butInsPlanExclusionsUseUCR_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"For use with PPO plans where certain excluded procedures are allowed to be billed using UCR fee rather than a negotiated rate.\r\n\r\n" +
				"Exclusions are defined using an Other Benefits exclusion rule, or for any benefit set to a 0% coverage level.");
		}

		private void checkInsDefaultAssignmentOfBenefits_Click(object sender,EventArgs e) {
			//Users with Setup permission are always allowed to change the Checked property of this check box.
			//However, there is a second step when changing the value that can only be performed by users with the InsPlanChangeAssign permission.
			if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign,true)) {
				return;
			}
			string promptMsg=Lan.g(this,"Would you like to immediately change all plans to use assignment of benefits?\r\n"
					+$"Warning: This will update all existing plans to render payment to the provider on all future claims.");
			if(!checkInsDefaultAssignmentOfBenefits.Checked) {
				promptMsg=Lan.g(this,"Would you like to immediately change all plans to use assignment of benefits?\r\n"
					+$"Warning: This will update all existing plans to render payment to the patient on all future claims.");
			}
			if(MessageBox.Show(promptMsg,Lan.g(this,"Change all plans?"),MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			long subsAffected=InsSubs.SetAllSubsAssignBen(checkInsDefaultAssignmentOfBenefits.Checked);
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeAssign,0
				,Lan.g(this,"The following count of plan(s) had their assignment of benefits updated in the Family tab in Module Preferences:")+" "+POut.Long(subsAffected)
			);
			MessageBox.Show(Lan.g(this,"Plans affected:")+" "+POut.Long(subsAffected));
		}

		private void checkInsDefaultShowUCRonClaims_Click(object sender,EventArgs e) {
			if(!checkInsDefaultShowUCRonClaims.Checked) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to immediately change all plans to show office UCR fees on claims?")) {
				return;
			}
			long plansAffected=InsPlans.SetAllPlansToShowUCR();
			MessageBox.Show(Lan.g(this,"Plans affected: ")+plansAffected.ToString());
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillFamilyInsurance() {
			checkInsurancePlansShared.Checked=PrefC.GetBool(PrefName.InsurancePlansShared);
			checkPPOpercentage.Checked=PrefC.GetBool(PrefName.InsDefaultPPOpercent);
			checkCoPayFeeScheduleBlankLikeZero.Checked=PrefC.GetBool(PrefName.CoPay_FeeSchedule_BlankLikeZero);
			checkFixedBenefitBlankLikeZero.Checked=PrefC.GetBool(PrefName.FixedBenefitBlankLikeZero);
			checkInsDefaultShowUCRonClaims.Checked=PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims);
			checkInsDefaultAssignmentOfBenefits.Checked=PrefC.GetBool(PrefName.InsDefaultAssignBen);
			checkInsPPOsecWriteoffs.Checked=PrefC.GetBool(PrefName.InsPPOsecWriteoffs);
			checkClaimUseOverrideProcDescript.Checked=PrefC.GetBool(PrefName.ClaimPrintProcChartedDesc);
			checkClaimTrackingRequireError.Checked=PrefC.GetBool(PrefName.ClaimTrackingRequiresError);
			checkInsPlanExclusionsUseUCR.Checked=PrefC.GetBool(PrefName.InsPlanUseUcrFeeForExclusions);
			checkInsPlanExclusionsMarkDoNotBill.Checked=PrefC.GetBool(PrefName.InsPlanExclusionsMarkDoNotBillIns);
			checkPatInitBillingTypeFromPriInsPlan.Checked=PrefC.GetBool(PrefName.PatInitBillingTypeFromPriInsPlan);
		}

		public bool SaveFamilyInsurance() {
			Changed|=Prefs.UpdateBool(PrefName.InsurancePlansShared,checkInsurancePlansShared.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsDefaultPPOpercent,checkPPOpercentage.Checked);
			Changed|=Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero,checkCoPayFeeScheduleBlankLikeZero.Checked);
			Changed|=Prefs.UpdateBool(PrefName.FixedBenefitBlankLikeZero,checkFixedBenefitBlankLikeZero.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsDefaultShowUCRonClaims,checkInsDefaultShowUCRonClaims.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsDefaultAssignBen,checkInsDefaultAssignmentOfBenefits.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPPOsecWriteoffs,checkInsPPOsecWriteoffs.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimPrintProcChartedDesc,checkClaimUseOverrideProcDescript.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimTrackingRequiresError,checkClaimTrackingRequireError.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPlanExclusionsMarkDoNotBillIns,checkInsPlanExclusionsMarkDoNotBill.Checked);
			Changed|=Prefs.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,checkInsPlanExclusionsUseUCR.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PatInitBillingTypeFromPriInsPlan,checkPatInitBillingTypeFromPriInsPlan.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
