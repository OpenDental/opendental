using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace OpenDental.Misc {
		public class DbmInfo {
			public string MethodName { get; set;}// Name of the dbm method.
			public string Explanation { get; set;} //The explanation of the dbm method.
			public string ManualFix { get; set;} //The details on how to manually fix.
			public string WhyItHappens { get; set;} //The detail on why it happens.

			//has all of the info from https://www.opendental.com/manual/dbmlogresults.html
			public static List<DbmInfo> ListDbmInfos=>new List<DbmInfo>() {
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.AppointmentCompleteWithTpAttached),
						Explanation="Completed appointments have treatment planned procedures attached.",
						ManualFix="Manually delete the procedures, set them complete, or detach them from appointment.",
						WhyItHappens="A completed appointment was opened, procedures were highlighted, then Cancel was clicked instead of OK. Cancel does not undo the selection of procedures, so they may remain treatment-planned."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.AppointmentScheduledWithCompletedProcs),
						Explanation="Scheduled appointments have completed procedures attached.",
						ManualFix="If the appointments are in the past, manually set each appointment complete or set each procedure's status to treatment planned. If the appointments are in the future, the procedures will detach from the appointment once the notification is received. Manually revisit each appointment and reattach the procedures.",
						WhyItHappens="Procedures were set complete from the Chart Module but the appointments they were attached to were not set complete."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.BenefitsWithPartialDuplicatesForInsPlan),
						Explanation="Multiple benefits on a single insurance plan are nearly identical except they have differing Percent or MonetaryAmts.",
						ManualFix="In Lists, Insurance Plans double-click the matching insurance plan to open it, then double-click benefit information to open the Edit Benefits window. Check that the box in the top left called Simplified View is unchecked, then look through the benefits list to find any duplicates, double clicking and deleting them. Click OK twice to close the windows.",
						WhyItHappens="Either a duplicate benefit was entered manually, or in an older version a bug sometimes created a duplicate benefit when adding a plan to a patient and changing benefit information."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimMissingProcedures),
						Explanation="Claims missing all claim procedure information (e.g., codes, descriptions, etc).",
						ManualFix="Note any necessary claim information (e.g., date sent, etc.), delete the claim, recreate the claim with the correct procedures, re-enter any information from the deleted claim.",
						WhyItHappens="A bug in an older version of Open Dental caused this issue to occur sometimes when processing payments for ERAs."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimPaymentCheckAmt),
						Explanation="Insurance payments amounts do not match the amounts of the claims they are attached to.",
						ManualFix="Edit the claim's Ins Pay or insurance payment amount so that they match.",
						WhyItHappens="After an insurance payment has been finalized, a user has re-opened the insurance payment window and manually adjusted the Ins Pay amount so that it no longer matches the sum of payment amounts from the attached claim(s). Edits to insurance payments are logged in the audit trail under Permission InsPayEdit."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimPaymentsNotPartialWithNoClaimProcs),
						Explanation="Insurance payments are not marked as Partial, yet they have no insurance claims attached.",
						ManualFix="In Batch Insurance, find the insurance payment and delete it.",
						WhyItHappens=""
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimProcEstimateAttachedToGroupNote),
						Explanation="It is impossible to attach a group note to a claim, because group notes always have status EC, but status C is required to attach to a claim, or status TP for a preauth. Since the group note cannot be attached to a claim, it also cannot be attached to a claim payment.",
						ManualFix="No manual fix. Run the ClaimProcEstimateAttachedToGroupNote DBM method or call support.",
						WhyItHappens="Sometimes claimprocs get attached to GroupNotes when running bulk tools, or recalculating."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimProcPreauthNotMatchClaim),
						Explanation="Insurance estimates attached to a preauthorization do not have a status of PreAuthorization.",
						ManualFix="Find the preauthorization and open it. Click OK. A message should appear saying Status of procedures was changed back to preauth to match status of claim.",
						WhyItHappens="The status of the insurance estimate was manually changed."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimProcStatusNotMatchClaim),
						Explanation="Insurance estimates attached to a claim have a status that does not match the claim status.",
						ManualFix="In the Account Module, double-click the claim. If the claim is marked as received but the insurance estimate isn't, click OK to mark all procedures as received. If the claim isn't marked as received, open the insurance estimate and change its status to match the claim.",
						WhyItHappens="The claim has a received status, but not all procedures in the claim have a received status. In most cases, the status of the insurance estimate was manually changed."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimProcWriteOffNegative),
						Explanation="Patients with procedures have negative writeoff amounts.",
						ManualFix="Go to the patients listed and manually correct the write-off amounts on the insurance estimates.",
						WhyItHappens="A negative writeoff amount was entered."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.ClaimProcsWithPartialDuplicates),
						Explanation="Only occurs on Canada databases. There is a duplicate claimproc on the claim and a duplicate estimate on the procedure.",
						ManualFix="Navigate to the claim and delete the duplicate claim procedures.",
						WhyItHappens=""
					},
				new DbmInfo() {
 						MethodName=nameof(DatabaseMaintenances.ClinicNumMissingInvalid),
 						Explanation="There are invalid clinics associated to patients or procedures. New clinics are created for the invalid clinics.",
						ManualFix="Update the newly created clinics by referencing the patients and procedures.",
 						WhyItHappens="Past issue that could cause procedures with invalid clinics."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.DepositsWithIncorrectAmount),
						Explanation="Deposits amounts do not match the payments they are attached to.",
						ManualFix="Edit the deposit so that its amount matches the payments it is attached to.",
						WhyItHappens=""
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.NotesWithTooMuchWhiteSpace),
						Explanation="Searches for notes that have too many consecutive spaces, tabs, or carriage returns.",
						ManualFix="Automatically fixed. No manual fix needed.",
						WhyItHappens="Can occur when copy/pasting content."
					},
				/*new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PatientsNoClinicSet),
						Explanation="The office has clinics enabled, but there are patients with no clinic set.",
						ManualFix="Edit the patient's info to assign them a clinic.",
						WhyItHappens="No clinic was assigned to the patient."
					},*/
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PatientPriProvHidden),
						Explanation="There are patients whose primary provider has been marked as hidden from the list of providers.",
						ManualFix="Either unhide the provider, or go to Lists, Providers to move all patients from the hidden provider to another provider.",
						WhyItHappens="A provider was marked as hidden before reassigning patients."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PatPlanOrdinalDuplicates),
						Explanation="Insurance plan entries for a patient have the same ordinal.",
						ManualFix="Go into each account's Family Module, double-click the secondary insurance plan and set Order to 2.",
						WhyItHappens=""
					},
				new DbmInfo() {
 						MethodName=nameof(DatabaseMaintenances.PaymentMissingPaySplit),
 						Explanation="Shows payments that have a PaymentAmt that doesn't match the sum of all associated PaySplits. Payments with no PaySplits are dealt with in PaymentMissingPaySplit. Note: this just returns info for a patient that has a paysplit for the payment because the payment only shows in the ledger for the patient with the paysplit.",
 						ManualFix="No manual fix. Run this DBM method or call support.",
 						WhyItHappens="Past issue that could causes payment entries with no split attached."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PayPlanChargeProvNum),
						Explanation="Payment plan charges do not have a provider set.",
						ManualFix="Open the payment plan and double-click on the charge. Set the provider on the charge.",
						WhyItHappens=""
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PaySplitAttachedToInsurancePaymentPlan),
						Explanation="Paysplits and/or ClaimPayments are attached to payment plan with the incorrect type.",
						ManualFix="Find problem payments and detach them from payment plans.",
						WhyItHappens="Paysplits or ClaimPayments accidentally got attached to a payment plan with the incorrect type."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.PaySplitWithInvalidPrePayNum),
						Explanation="A prepayment was deleted after it was allocated to a pay split.",
						ManualFix="In the Account Module, double-click the allocated pay split (transfer) and delete it.",
						WhyItHappens="In past versions we allowed users to delete prepayments that were allocated."
					},
				new DbmInfo() {
   					MethodName=nameof(DatabaseMaintenances.PreferenceDateDepositsStarted),
   					Explanation="If the program locks up when trying to create a deposit slip, it's because someone removed the start date from the deposit edit window.",
   					ManualFix="Deposit start date needs to be reset.",
   					WhyItHappens=""
					},
				new DbmInfo() {
 					 MethodName=nameof(DatabaseMaintenances.SpecialCharactersInNotes),
 					 Explanation="Looks through all appointment notes and procedure descriptions and shows the number of special characters.",
 					 ManualFix="Run the Spec Char tool to remove special characters from appt notes and appt proc descriptions.",
 					 WhyItHappens="Can occur when copying/pasting Extended ASCII, unicode characters, or extra spaces into the textbox."
					},
				new DbmInfo() {
						MethodName=nameof(DatabaseMaintenances.UserodDuplicateUser),
						Explanation="Multiple user accounts that are not hidden have the same username.",
						ManualFix="Go to Setup, Security and hide all but one of each unique user.",
						WhyItHappens="Duplicate user accounts were created but one of them was not hidden."
					}
			};
		}
}
