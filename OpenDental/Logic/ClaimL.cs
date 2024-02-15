using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness.Eclaims;
using static OpenDentBusiness.Eclaims.Canadian;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;

namespace OpenDental {
	public class ClaimL {

		///<summary>Validates that default clearinghouses are set up correctly.
		///Shows an error message to the user and returns false if they are not set up; Otherwise, returns true.</summary>
		public static bool CheckClearinghouseDefaults() {
			string errorMessage=Clearinghouses.CheckClearinghouseDefaults();
			if(!string.IsNullOrEmpty(errorMessage)) {
				MessageBox.Show(errorMessage);
				return false;
			}
			return true;
		}

		///<summary>Adds the message passed in to the ErrorMessage of the createClaimDataWrapper passed in.
		///Shows the message passed in to the user if isVerbose is true. Be sure to translate message before calling this function.</summary>
		private static void LogClaimError(CreateClaimDataWrapper createClaimDataWrapper,string message,bool isVerbose,string msgBoxHeader="") {
			if(createClaimDataWrapper.ErrorMessage!="") {
				createClaimDataWrapper.ErrorMessage+="\r\n";
			}
			createClaimDataWrapper.ErrorMessage+=$"{msgBoxHeader}: {message}";
			createClaimDataWrapper.HasError=true;
			if(isVerbose) {
				MessageBox.Show(message,msgBoxHeader);
			}
		}

		///<summary>Gets a list of CreateClaimItems from the current table and selected items.
		///If no selections are made, the entire table is converted into CreateClaimItems and is returned.</summary>
		///<param name="dataTable">This table must have three columns at minimum: ProcNum, ProcNumLab, and chargesDouble</param>
		///<param name="arraySelectedIndices">Any selected rows in the corresponding table. An empty array is acceptable.</param>
		///<returns>List of objects that can be used for creating claims instead of directly utilizing DataTable\ODGrid objects directly.</returns>
		public static List<CreateClaimItem> GetCreateClaimItems(DataTable dataTable,int[] arraySelectedIndices) {
			List<DataRow> listDataRows=dataTable.Select().ToList();
			//If the user manually selected items in the grid then we want to only include the selected rows.
			bool hasSelections=(arraySelectedIndices.Length > 0);
			if(hasSelections) {
				listDataRows=listDataRows.Where((x,index) => index.In(arraySelectedIndices)).ToList();
			}
			//Create CreateClaimItem objects from the data rows so that we have deep copied and strongly typed objects to work with in other methods.
			return listDataRows.Select(x => new CreateClaimItem() {
				ProcNum=PIn.Long(x["ProcNum"].ToString()),
				ProcNumLab=PIn.Long(x["ProcNumLab"].ToString()),
				ChargesDouble=PIn.Double(x["chargesDouble"].ToString()),
				IsSelected=hasSelections,
			}).ToList();
		}

		/// <summary> Block users from creating claims when a selected procedure is associated with duplicate claimprocs. A 
		/// 'duplicate claimproc' does not mean that all of the information has been duplicated but instead means that there are two claimprocs for the same insurance plan.
		/// Specifically, a duplicate claimproc for primary insurance, or secondary insurance, or tertiary insurance, etc.
		/// Create a list of ProcNums that have duplicate claimprocs associated with them so that we can let the user know which procedures need manual intervention.
		/// </summary>
		public static bool WarnUsersForDuplicateClaimProcs(CreateClaimDataWrapper createClaimDataWrapper,bool isVerbose=true) {
			List<long> listProcNums=new List<long>();
			//Do not count 'Preauth' claimprocs as duplicates since these are expected to be present for the same insurance plan.
			List<ClaimProc> listClaimProcsAvailable=createClaimDataWrapper.CreateClaimData_.ListClaimProcs
				.FindAll(x => x.Status!=ClaimProcStatus.Preauth && x.Status!=ClaimProcStatus.Supplemental);
			//Only consider the claimprocs associated with the selected procedures (even if there are other procedures on the account with this problem).
			List<CreateClaimItem> listCreateClaimItemsSelected=createClaimDataWrapper.ListCreateClaimItems.FindAll(x => x.IsSelected);
			for(int i=0;i<listCreateClaimItemsSelected.Count;i++) {
				List<ClaimProc> listClaimProcs=ClaimProcs.GetForProc(listClaimProcsAvailable,listCreateClaimItemsSelected[i].ProcNum);
				//Group all of the claimprocs for this procedure by InsSubNum (e.g. by primary insurance).
				//Keep track of any procedures that have multiple claimprocs for the same InsSubNum.
				if(listClaimProcs.GroupBy(x => x.InsSubNum).Any(x => x.Count() > 1)) {
					listProcNums.Add(listCreateClaimItemsSelected[i].ProcNum);
				}
			}
			if(listProcNums.Count>0) {
				string message=Lan.g("ContrAccount","Procedures with multiple claimprocs for the same insurance found:")+" "+listProcNums.Count;
				for(int i=0;i<listProcNums.Count;i++) {
					Procedure procedure=Procedures.GetProcFromList(createClaimDataWrapper.CreateClaimData_.ListProcs,listProcNums[i]);
					message+="\r\n  "+Procedures.ConvertProcToString(procedure.CodeNum,procedure.Surf,procedure.ToothNum,true);
				}
				message+="\r\n\r\n"+Lan.g("ContrAccount","The above procedures need to have the duplicate claimprocs deleted before creating a claim.");
				LogClaimError(createClaimDataWrapper,message,isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return true;
			}
			return false;
		}

		///<summary>Returns a CreateClaimDataWrapper object that is specifically designed for the claim creation process from within the UI.
		///It contains strongly typed variables which help indicate to the claim creation method how to correctly create the claim.
		///It also contains variables that indicate to consuming methods what happened during the claim creation process.
		///It is a requirement to have listCreateClaimItems filled with at least one item.
		///Optionally set isSelectionRequired to true if the user must have already selected indices within the grid passed in.
		///This method throws exceptions (specifically for developers), shows messages, and other UI related actions during claim creation.</summary>
		public static CreateClaimDataWrapper GetCreateClaimDataWrapper(Patient patient,Family family,List<CreateClaimItem> listCreateClaimItems,bool isVerbose
			,bool isSelectionRequired=false)
		{
			CreateClaimDataWrapper createClaimDataWrapper=new CreateClaimDataWrapper();
			if(listCreateClaimItems==null) {//An engineer incorrectly called this method.
				throw new ArgumentException("Invalid argument passed in.",nameof(listCreateClaimItems));
			}
			createClaimDataWrapper.ListCreateClaimItems=listCreateClaimItems;
			if(!Security.IsAuthorized(EnumPermType.ClaimView,true)) {
				LogClaimError(createClaimDataWrapper,Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(EnumPermType.ClaimView),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			if(listCreateClaimItems.Count < 1) {
				//There is nothing to do because at least one item is required in order to create a claim...
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			if(!CheckClearinghouseDefaults()) {
				createClaimDataWrapper.ShouldRefresh=false;
				createClaimDataWrapper.HasError=true;
				return createClaimDataWrapper;
			}
			createClaimDataWrapper.Patient_=patient;
			createClaimDataWrapper.Family_=family;
			createClaimDataWrapper.CreateClaimData_=AccountModules.GetCreateClaimData(patient,family);
			if(createClaimDataWrapper.CreateClaimData_.ListPatPlans.Count==0) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Patient does not have insurance."),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			int countSelected=0;
			InsSub insSub;
			if(listCreateClaimItems.All(x => !x.IsSelected)) {
				if(isSelectionRequired) {
					LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
					createClaimDataWrapper.ShouldRefresh=false;
					return createClaimDataWrapper;
				}
				//autoselect procedures
				for(int i=0;i<listCreateClaimItems.Count;i++){
					if(listCreateClaimItems[i].ProcNum==0) {
						continue;//ignore non-procedures
					}
					if(listCreateClaimItems[i].ChargesDouble==0) {
						continue;//ignore zero fee procedures, but user can explicitly select them
					}
					//payment rows skipped
					Procedure procedure=Procedures.GetProcFromList(createClaimDataWrapper.CreateClaimData_.ListProcs,listCreateClaimItems[i].ProcNum);
					ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x => x.CodeNum==procedure.CodeNum)??new ProcedureCode();
					if(procedureCode.IsCanadianLab) {
						continue;
					}
					int ordinal=PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.CreateClaimData_.ListPatPlans
						,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					int ordinalSec=PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.CreateClaimData_.ListPatPlans
						,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					if(ordinal==0) { //No primary dental plan. Must be a medical plan. Use the first medical plan instead.
						ordinal=1;
					}
					insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.CreateClaimData_.ListPatPlans,ordinal),createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					InsSub insSubSec=null;
					if(ordinalSec>0) {
						insSubSec=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.CreateClaimData_.ListPatPlans,ordinalSec),createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					}
					//Select the item if any procedure needs primary or secondary insurance sent.
					if(Procedures.NeedsSent(procedure.ProcNum,insSub.InsSubNum,createClaimDataWrapper.CreateClaimData_.ListClaimProcs) 
						|| (insSubSec!=null && Procedures.NeedsSent(procedure.ProcNum,insSubSec.InsSubNum,createClaimDataWrapper.CreateClaimData_.ListClaimProcs)))
					{
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && countSelected==7) {//Canadian. en-CA or fr-CA
							if(isVerbose) {//This is an informational message, not an error.
								MsgBox.Show("ContrAccount","Only the first 7 procedures will be automatically selected.\r\n"
								+"You will need to create another claim for the remaining procedures.");
							}
							break;//Continue creating the claim, but stop selecting procedures so that only 7 will be added to the claim.
						}
						countSelected++;
						listCreateClaimItems[i].IsSelected=true;
					}
				}
				if(listCreateClaimItems.All(x => !x.IsSelected)) {//if still none selected
					LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
					createClaimDataWrapper.ShouldRefresh=false;
					return createClaimDataWrapper;
				}
			}
			if(listCreateClaimItems.Any(x => x.IsSelected && x.ProcNum==0)) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","You can only select procedures."),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			//At this point, all selected items are procedures. In Canada, the selections may also include labs.
			InsCanadaValidateProcs(createClaimDataWrapper,isVerbose);
			if(createClaimDataWrapper.ListCreateClaimItems.All(x => !x.IsSelected)) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			List<CreateClaimItem> listCreateClaimItemsSelected=createClaimDataWrapper.ListCreateClaimItems.FindAll(x => x.IsSelected);
			WarnUsersForDuplicateClaimProcs(createClaimDataWrapper);
			return createClaimDataWrapper;
		}

		///<summary>Returns the createClaimDataWrapper object that was passed in after manipulating it.
		///This object will contain information about what happened during the claim creation process (e.g. error messages, refresh indicator, etc).
		///E.g. CreateClaimDataWrapper.HasError will be true if any errors occurred. ErrorMessage might contain additional information about the error.
		///This method assumes that createClaimDataWrapper was set up correctly (refer to GetCreateClaimDataWrapper() for how to set up this object).
		///Set flag hasPrimaryClaim to true if the primary claim has already been created and should not be recreated.
		///Set flag hasSecondaryClaim to true if the secondary claim has already been created and should not be recreated.</summary>
		public static CreateClaimDataWrapper CreateClaimFromWrapper(bool isVerbose,CreateClaimDataWrapper createClaimDataWrapper
			,bool hasPrimaryClaim=false,bool hasSecondaryClaim=false)
		{
			createClaimDataWrapper.ErrorMessage="";
			if(!Security.IsAuthorized(EnumPermType.ClaimView,true)) {
				LogClaimError(createClaimDataWrapper,Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(EnumPermType.ClaimView),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			createClaimDataWrapper.CountClaimsCreated=0;
			if(!CheckClearinghouseDefaults()) {
				createClaimDataWrapper.ShouldRefresh=false;
				createClaimDataWrapper.HasError=true;
				return createClaimDataWrapper;
			}
			if(createClaimDataWrapper.CreateClaimData_.ListPatPlans.Count==0) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Patient does not have insurance."),isVerbose);
				createClaimDataWrapper.ShouldRefresh=false;
				return createClaimDataWrapper;
			}
			bool doCreateSecondaryClaim=!hasSecondaryClaim && PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,
				createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs)>0 //if there exists a secondary plan
				&& !CultureInfo.CurrentCulture.Name.EndsWith("CA");//And not Canada (don't create secondary claim for Canada)
			Claim claimPrimary=null;
			if(!hasPrimaryClaim) {
				string claimType="P";
				//If they have medical insurance and no dental, make the claim type Medical. This is to avoid the scenario of multiple med ins and no dental.
				if(PatPlans.GetOrdinal(PriSecMed.Medical,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs)>0
					&& PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs)==0
					&& PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListInsSubs)==0) 
				{
					claimType="Med";
				}
				claimPrimary=new Claim();
				claimPrimary.DateSent=DateTime.Today;
				claimPrimary.DateSentOrig=DateTime.MinValue;
				claimPrimary.ClaimStatus="W";
				//Set claimPrimary to what CreateClaim returns because the reference to claimPrimary gets broken when inserting.
				claimPrimary=CreateClaim(claimPrimary,claimType,isVerbose,createClaimDataWrapper,"Primary Claim Error");
				if(claimPrimary.ClaimNum==0) {
					createClaimDataWrapper.ShouldRefresh=true;
					return createClaimDataWrapper; 
				}
				if(isVerbose && claimPrimary.ClaimNum!=0) {//Only provide the user with the option to cancel the claim if attempting to create a single claim manually.
					using FormClaimEdit formClaimEdit=new FormClaimEdit(claimPrimary,createClaimDataWrapper.Patient_,createClaimDataWrapper.Family_);
					formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
					formClaimEdit.ShowDialog();
					if(formClaimEdit.DialogResult!=DialogResult.OK) {
						createClaimDataWrapper.ShouldRefresh=true;//will have already been deleted
						return createClaimDataWrapper;
					}
					double unearnedAmount=(double)PaySplits.GetTotalAmountOfUnearnedForPats(createClaimDataWrapper.Family_.GetPatNums());
					AllocateUnearnedPayment(createClaimDataWrapper.Patient_,createClaimDataWrapper.Family_,unearnedAmount,claimPrimary);
				}
				else if(claimPrimary.ClaimNum!=0) {//isVerbose is false, still need to log.
					Patient patient=createClaimDataWrapper.Patient_;
					SecurityLogs.MakeLogEntry(EnumPermType.ClaimEdit,patient.PatNum,"New claim created for "+patient.LName+","+patient.FName,
						claimPrimary.ClaimNum,claimPrimary.SecDateTEdit);
				}
			}
			Claim claimSecondary=null;
			if(doCreateSecondaryClaim) {
				//ClaimL.CalculateAndUpdate() could have added new claimprocs for additional insurance plans if they were added after the proc was completed
				createClaimDataWrapper.CreateClaimData_.ListClaimProcs=ClaimProcs.Refresh(createClaimDataWrapper.Patient_.PatNum);
				claimSecondary=new Claim();
				claimSecondary.ClaimStatus="H";
				//Set claimSecondary to CreateClaim because the reference to claimSecondary gets broken when inserting.
				claimSecondary=CreateClaim(claimSecondary,"S",isVerbose,createClaimDataWrapper,"Secondary Claim Error");
				if(claimSecondary.ClaimNum==0) {
					createClaimDataWrapper.ShouldRefresh=true;
					return createClaimDataWrapper;
				}
				Patient patient=createClaimDataWrapper.Patient_;
				SecurityLogs.MakeLogEntry(EnumPermType.ClaimEdit,patient.PatNum,"New claim created for "+patient.LName+","+patient.FName,
					claimSecondary.ClaimNum,claimSecondary.SecDateTEdit);
			}
			if(claimPrimary!=null) {
				claimPrimary=Claims.GetClaim(claimPrimary.ClaimNum);
				if(claimSecondary!=null && claimSecondary.ClaimStatus.In("H","U") && claimPrimary.ClaimStatus=="R") {
					if(PrefC.GetBool(PrefName.PromptForSecondaryClaim) && Security.IsAuthorized(EnumPermType.ClaimSend,suppressMessage:true)) {
						List<ClaimProc> listClaimProcs=createClaimDataWrapper.CreateClaimData_.ListClaimProcs.FindAll(x=>x.ClaimNum==claimPrimary.ClaimNum);
						PromptForSecondaryClaim(listClaimProcs);
					}
				}
			}
			createClaimDataWrapper.ShouldRefresh=true;
			return createClaimDataWrapper;
		}

		///<summary>Prompts the user to allocate unearned if necessary.</summary>
		public static void AllocateUnearnedPayment(Patient patient,Family family,double unearnedAmt,Claim claim) {
			if(CompareDouble.IsLessThanOrEqualToZero(unearnedAmt)) {
				return;//There is no unearned money to allocate. Nothing to do.
			}
			//do not try to allocate payment if preference is disabled or if there isn't a payment to allocate
			if(!PrefC.GetBool(PrefName.ShowAllocateUnearnedPaymentPrompt) || ClaimProcs.GetPatPortionForClaim(claim)<=0) { 
				return;
			}
			using FormProcSelect formProcSelect=new FormProcSelect(patient.PatNum,false,true,doShowAdjustments:true,doShowTreatmentPlanProcs:false);
			if(formProcSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Payment payment=new Payment();
			payment.PayDate=DateTime.Today;
			payment.PatNum=patient.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			payment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					payment.ClinicNum=patient.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					payment.ClinicNum=(Clinics.ClinicNum==0)?patient.ClinicNum:Clinics.ClinicNum;
				}
				else {
					payment.ClinicNum=Clinics.ClinicNum;
				}
			}
			payment.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				payment.PayType=listDefs[0].DefNum;
			}
			payment.PaymentSource=CreditCardSource.None;
			payment.ProcessStatus=ProcessStat.OfficeProcessed;
			payment.PayAmt=0;
			Payments.Insert(payment);
			using FormPayment formPayment=new FormPayment(patient,family,payment,false);
			formPayment.IsNew=true;
			formPayment.UnearnedAmt=unearnedAmt;
			formPayment.ListAccountEntriesPayFirst=formProcSelect.ListAccountEntries;
			formPayment.IsIncomeTransfer=true;
			formPayment.ShowDialog();
		}

		///<summary>Only allows up to 7 CreateClaimItems to be selected within createClaimDataWrapper when Canadian.
		///Shows a message to the user stating this fact if enforced; Otherwise does nothing.</summary>
		private static void InsCanadaValidateProcs(CreateClaimDataWrapper createClaimDataWrapper,bool isVerbose) {
			List<CreateClaimItem> listCreateClaimItemsToolBarInsSelectedItems=createClaimDataWrapper.ListCreateClaimItems.FindAll(x => x.IsSelected);
			int procCount=listCreateClaimItemsToolBarInsSelectedItems.Count(x => x.ProcNumLab==0);//Not a lab
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && procCount > 7) {//Canadian. en-CA or fr-CA
				//Unselect all but the first 7 procedures with the smallest index numbers.
				int procSelectedCount=0;
				for(int i=0;i<listCreateClaimItemsToolBarInsSelectedItems.Count;i++){
					listCreateClaimItemsToolBarInsSelectedItems[i].IsSelected=(procSelectedCount < 7);
					if(listCreateClaimItemsToolBarInsSelectedItems[i].ProcNumLab==0) {//Not a lab
						procSelectedCount++;
					}
				}
				string message=Lan.g("ContrAccount","Only the first 7 procedures will be selected.  "
					+"You will need to create another claim for the remaining procedures.");
				LogClaimError(createClaimDataWrapper,message,isVerbose);
			}
		}

		///<summary>All validation on the procedures is done here. Creates and saves the claim initially, attaching all selected procedures.
		///But it does not refresh any data, does not do a final update of the new claim, and does not enter fee amounts.
		///claimType=P,S,Med,or Other
		///This method assumes that createClaimDataWrapper was set up correctly (refer to GetCreateClaimDataWrapper() for how to set up this object).
		///Returns a 'new' claim object (ClaimNum=0) to indicate that the user does not want to create the claim or there are validation issues..</summary>
		public static Claim CreateClaim(Claim claim,string claimType,bool isVerbose,CreateClaimDataWrapper createClaimDataWrapper,string msgBoxHeader="") {
			Patient patient=createClaimDataWrapper.Patient_;
			InsPlan insPlan=new InsPlan();
			InsSub insSub=new InsSub();
			Relat relatOther=Relat.Self;
			string claimTypeDesc="";
			switch(claimType){
				case "P":
					insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.CreateClaimData_.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans
						,createClaimDataWrapper.CreateClaimData_.ListInsSubs)),createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,createClaimDataWrapper.CreateClaimData_.ListInsPlans);
					claimTypeDesc="primary";
					break;
				case "S":
					insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.CreateClaimData_.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans
						,createClaimDataWrapper.CreateClaimData_.ListInsSubs)),createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,createClaimDataWrapper.CreateClaimData_.ListInsPlans);
					claimTypeDesc="secondary";
					break;
				case "Med":
					//It's already been verified that a med plan exists
					insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.CreateClaimData_.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Medical,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans
						,createClaimDataWrapper.CreateClaimData_.ListInsSubs)),createClaimDataWrapper.CreateClaimData_.ListInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,createClaimDataWrapper.CreateClaimData_.ListInsPlans);
					claimTypeDesc="medical";
					break;
				case "Other":
					using(FormClaimCreate formClaimCreate=new FormClaimCreate(patient.PatNum)) {
						formClaimCreate.ShowDialog();
						if(formClaimCreate.DialogResult!=DialogResult.OK){
							return new Claim();
						}
						insPlan=formClaimCreate.InsPlanSelected;
						insSub=formClaimCreate.InsSubSelected;
						relatOther=formClaimCreate.RelatPat;
					}
					break;
			}
			List<long> listSelectedProcNums=createClaimDataWrapper.ListCreateClaimItems
				.Where(x => x.IsSelected)
				.Select(x => x.ProcNum).ToList();
			List<Procedure> listProceduresSelected=createClaimDataWrapper.CreateClaimData_.ListProcs.FindAll(x => listSelectedProcNums.Contains(x.ProcNum));
			List<Procedure> listProceduresBillIns=listProceduresSelected.FindAll(x => !Procedures.NoBillIns(x,createClaimDataWrapper.CreateClaimData_.ListClaimProcs,insPlan.PlanNum));
			List<Procedure> listProceduresNoBillIns=listProceduresSelected.FindAll(x => !listProceduresBillIns.Select(y => y.ProcNum).Contains(x.ProcNum));
			//If all the procedures are marked as NoBillIns, then warn the user that the primary/secondary claim will not get created and then return a new Claim.
			if(listProceduresSelected.Count>0 && listProceduresBillIns.Count==0) {
				//No claim to create since we will be creating a secondary. Continue on to the secondary. Nothing to log.
				MsgBox.Show($"No {claimTypeDesc} claim will be created due to all procedures being marked as NoBillIns.");
				return new Claim();
			}
			if(listProceduresNoBillIns.Count > 0) {
				StringBuilder sbMsg=new StringBuilder($"The following procedures were marked as NoBillIns. They will be excluded from the {claimTypeDesc} claim.\r\n");
				for(int i=0;i<listProceduresNoBillIns.Count;i++){
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresNoBillIns[i].CodeNum);
					sbMsg.AppendLine($"{procedureCode.ProcCode} {(string.IsNullOrEmpty(listProceduresNoBillIns[i].ToothNum) ? "" : ($"Tth {listProceduresNoBillIns[i].ToothNum} "))} - {procedureCode.Descript}");
				}
				MsgBox.Show(sbMsg.ToString());
			}
			//If we are going to block based on a preference, do that before figuring out other claim validation.
			//Ignore "No Bill Ins" here, because we want "No Bill Ins" to be the more important block for backwards compatability.
			switch(PIn.Enum<ClaimZeroDollarProcBehavior>(PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior))) {
				case ClaimZeroDollarProcBehavior.Warn:
					if(listProceduresBillIns.Any(x => CompareDouble.IsZero(x.ProcFee))
						&& MessageBox.Show(Lan.g("ContrAccount","You are about to make a")+" "+(claimTypeDesc=="" ? "" : (claimTypeDesc+" "))
							+Lan.g("ContrAccount","claim that will include a $0 procedure.  Continue?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
						//Nothing to log. The user hit Cancel.
						return new Claim();
					}
					break;
				case ClaimZeroDollarProcBehavior.Block:
					if(listProceduresBillIns.Any(x => CompareDouble.IsZero(x.ProcFee))) {
						MsgBox.Show("ContrAccount","You can't make a claim for a $0 procedure.");
						//Nothing to log. The user was just notified.
						return new Claim();
					}
					break;
				case ClaimZeroDollarProcBehavior.Allow:
				default:
					break;
			}
			try{
				claim=AccountModules.CreateClaim(claim,claimType,createClaimDataWrapper.CreateClaimData_.ListPatPlans
					,createClaimDataWrapper.CreateClaimData_.ListInsPlans,createClaimDataWrapper.CreateClaimData_.ListClaimProcs,createClaimDataWrapper.CreateClaimData_.ListProcs
					,createClaimDataWrapper.CreateClaimData_.ListInsSubs,patient,createClaimDataWrapper.CreateClaimData_.PatNote,listProceduresBillIns
					,createClaimDataWrapper.ErrorMessage,insPlan,insSub,relatOther);
			}
			catch(Exception e){
				LogClaimError(createClaimDataWrapper,e.Message,isVerbose,msgBoxHeader);
				return claim;
			}
			createClaimDataWrapper.CountClaimsCreated++;
			return claim;
		}

		public static void PromptForSecondaryClaim(List<ClaimProc> listClaimProcsForClaim) {
			List<Claim> listClaimsSecondary=Claims.GetSecondaryClaimsNotReceived(listClaimProcsForClaim);
			if(listClaimsSecondary.Count==0) {
				return;//No secondary claims for the procedures attached to the primary.
			}
			string msg=Lan.g("ContrAccount","There is at least one unsent secondary claim for the received procedures.");
			msg+="\r\n";
			msg+=Lan.g("ContrAccount","Would you like to:");
			string strChangeStatus=Lan.g("ContrAccount","Change the claim status to 'Waiting to send'");
			string strSendClaims=Lan.g("ContrAccount","Send secondary claim(s) now");
			string strPrintClaims=Lan.g("ContrAccount","Print secondary claims(s) now");
			string strDoNothing=Lan.g("ContrAccount","Do nothing");
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.RadioButton;
			inputBoxParam.LabelText=msg;
			inputBoxParam.Text=strChangeStatus;
			listInputBoxParams.Add(inputBoxParam);
			inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.RadioButton;
			inputBoxParam.Text=strSendClaims;
			listInputBoxParams.Add(inputBoxParam);
			inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.RadioButton;
			inputBoxParam.Text=strPrintClaims;
			listInputBoxParams.Add(inputBoxParam);
			if(!PrefC.GetBool(PrefName.ClaimPrimaryRecievedForceSecondaryStatus)) {
				inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.RadioButton;
				inputBoxParam.Text=strDoNothing;
				listInputBoxParams.Add(inputBoxParam);
			}
			InputBox inputBox=new InputBox(listInputBoxParams);
			inputBox.SetTitle(Lan.g("ContrAccount","Outstanding secondary claims"));
			inputBox.SizeInitial=new System.Windows.Size(450,200);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return;
			}
			string radioButtonSelected=inputBox.RadioButtonResult;
			if(radioButtonSelected==strDoNothing) {
				return;
			}
			//Update status of claims to 'Waiting to Send'.
			//This must happen for the "Change the claim status" and "Send secondary claim(s)" options.
			//See Claims.GetQueueList(...) below.
			for(int i=0;i<listClaimsSecondary.Count;i++){
				listClaimsSecondary[i].ClaimStatus="W";
				Claims.Update(listClaimsSecondary[i]);
			}
			if(radioButtonSelected==strSendClaims) {
				//Most likely all of the procedures on the primary claim will have all of the procedures on 1 secondary claim. Expecially since most of time the 
				//claim will be created automatically. The only time they don't get created automatically is when the patient doesn't have a secondary claim 
				//at the time the primary claim gets created. Even if the user created the claim manually, the chances that the procedures on the primary have more than
				//one claim are low.
				ClaimSendQueueItem[] listQueues=Claims.GetQueueList(listClaimsSecondary.Select(x=>x.ClaimNum).ToList(),0,0);
				List<ClaimSendQueueItem> listClaimSendQueueItemsSent=SendClaimSendQueueItems(listQueues.ToList(),0);//Use clearinghouseNum of 0 to indicate automatic calculation of clearinghouse
				for(int i=0;i<listClaimSendQueueItemsSent.Count;i++) {
					if(listClaimSendQueueItemsSent[i].ClaimStatus!="S") {
						continue;
					}
					Claims.ReceiveAsNoPaymentIfNeeded(listClaimSendQueueItemsSent[i].ClaimNum);
				}
				return;
			}
			if(radioButtonSelected!=strPrintClaims){
				return;
			}
			bool doUsePrinterSettingsForAll=false;
			if(listClaimsSecondary.Count>1) {
				string stringSameSettings=Lan.g("FormClaimPrint","Use the same printer settings for all ");
				string stringClaims=Lan.g("FormClaimPrint"," claims?");
				string stringMessage=stringSameSettings+listClaimsSecondary.Count.ToString()+stringClaims;
				doUsePrinterSettingsForAll=MsgBox.Show(MsgBoxButtons.YesNo,stringMessage);
			}
			for(int i=0;i<listClaimsSecondary.Count;i++) {
				Claim claim=listClaimsSecondary[i];
				using FormClaimPrint formClaimPrint=new FormClaimPrint();
				formClaimPrint.PatNum=claim.PatNum;
				formClaimPrint.ClaimNum=claim.ClaimNum;
				formClaimPrint.ClaimFormCur=null;//so it will pull from the individual claim or plan.
				string stringAuditDescription=Lan.g("FormClaimPrint","Claim from ")+claim.DateService.ToShortDateString();
				bool doUseLastPrinterSettingsIfAvailable=(doUsePrinterSettingsForAll && i>0);
				if(!formClaimPrint.PrintImmediate(stringAuditDescription,PrintSituation.Claim,0,doUseLastPrinterSettingsIfAvailable)) {
					if(MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g("FormClaimPrint","Check the printer. Click OK to try printing again or Cancel to stop."))) {
						i--;
						continue;
					}
					break;
				}
				Etranss.SetClaimSentOrPrinted(claim.ClaimNum,claim.ClaimStatus,claim.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				//This call may receive the claim, but we don't want to prompt for secondary claims because we are already in the process of doing that.
				Claims.ReceiveAsNoPaymentIfNeeded(claim.ClaimNum);
			}
		}
		
		///<summary>Validates and sends each of the ClaimSendQueueItems passed in. Returns a list of the ClaimSendQueueItem that were sent.</summary>
		internal static List<ClaimSendQueueItem> SendClaimSendQueueItems(List<ClaimSendQueueItem> listClaimSendQueueItems,long hqClearinghouseNum) {
			List<ClaimSendQueueItem> listClaimSendQueueItemsRetVal=new List<ClaimSendQueueItem>();//a list of queue items to send
			if(listClaimSendQueueItems.Count==0) {
				return listClaimSendQueueItemsRetVal;
			}
			if(PrefC.HasClinicsEnabled) {//Clinics is in use
				long clinicNum0=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum).ClinicNum;
				for(int i=1;i<listClaimSendQueueItems.Count;i++) {
					long clinicNum=Claims.GetClaim(listClaimSendQueueItems[i].ClaimNum).ClinicNum;
					if(clinicNum0!=clinicNum) {
						MsgBox.Show("ContrAccount","All claims must be for the same clinic.  You can use the combobox at the top to filter.");//TODO: Wording.
						return listClaimSendQueueItemsRetVal;
					}
				}
			}
			long clearinghouseNum0=listClaimSendQueueItems[0].ClearinghouseNum;
			EnumClaimMedType claimMedType0=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum).MedType;
			for(int i=0;i<listClaimSendQueueItems.Count;i++){//we start with 0 so that we can check medtype match on the first claim
				long clearinghouseNumI=listClaimSendQueueItems[i].ClearinghouseNum;
				if(clearinghouseNum0!=clearinghouseNumI) {
					MsgBox.Show("ContrAccount","All claims must be for the same clearinghouse.");
					return listClaimSendQueueItemsRetVal;
				}
				EnumClaimMedType claimMedTypeI=Claims.GetClaim(listClaimSendQueueItems[i].ClaimNum).MedType;
				if(claimMedType0!=claimMedTypeI) {
					MsgBox.Show("ContrAccount","All claims must have the same MedType.");
					return listClaimSendQueueItemsRetVal;
				}
				Clearinghouse clearinghouse=Clearinghouses.GetClearinghouse(clearinghouseNumI);
				if(clearinghouse.Eformat==ElectronicClaimFormat.x837D_4010 || clearinghouse.Eformat==ElectronicClaimFormat.x837D_5010_dental) {
					if(claimMedTypeI!=EnumClaimMedType.Dental) {
						MsgBox.Show("ContrAccount","On claim "+POut.Int(i)+", the MedType does not match the clearinghouse e-format.");
						return listClaimSendQueueItemsRetVal;
					}
				}
				if(clearinghouse.Eformat==ElectronicClaimFormat.x837_5010_med_inst) {
					if(claimMedTypeI!=EnumClaimMedType.Medical && claimMedTypeI!=EnumClaimMedType.Institutional) {
						MsgBox.Show("ContrAccount","On claim "+POut.Int(i)+", the MedType does not match the clearinghouse e-format.");
						return listClaimSendQueueItemsRetVal;
					}
				}
				if(listClaimSendQueueItems[i].HasIcd9) {
					string msgText=Lan.g("ContrAccount","There are ICD-9 codes attached to a procedure.  Would you like to send the claim without the ICD-9 codes? ");
					if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
						return listClaimSendQueueItemsRetVal;
					}
				}
				//This is done for PromptForSecondaryClaim(...).
				//SendEclaimsToClearinghouse(...) already validates items in listClaimSendQueueItems, SetClaimItemIsValid(...) will just return in this case.
				listClaimSendQueueItems[i]=SetClaimItemIsValid(listClaimSendQueueItems[i],clearinghouse);
				if(!listClaimSendQueueItems[i].IsValid && listClaimSendQueueItems[i].CanSendElect) {
					MsgBox.Show("ContrAccount","Not allowed to send e-claims with missing information.");
					return listClaimSendQueueItemsRetVal;
				}
				if(listClaimSendQueueItems[i].NoSendElect==NoSendElectType.NoSendElect) {
					MsgBox.Show("ContrAccount","Not allowed to send e-claims.");
					return listClaimSendQueueItemsRetVal;
				}
				if(listClaimSendQueueItems[i].NoSendElect==NoSendElectType.NoSendSecondaryElect && listClaimSendQueueItems[i].Ordinal!=1) {
					MsgBox.Show("ContrAccount","Only allowed to send primary insurance e-claims.");
					return listClaimSendQueueItemsRetVal;
				}
			}
			for(int i=0;i<listClaimSendQueueItems.Count;i++){
				ClaimSendQueueItem claimSendQueueitem=listClaimSendQueueItems[i].Copy();
				if(hqClearinghouseNum!=0) {
					claimSendQueueitem.ClearinghouseNum=hqClearinghouseNum;
				}
				listClaimSendQueueItemsRetVal.Add(claimSendQueueitem);
			}
			Claim claim=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum);
			long claimClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				claimClinicNum=claim.ClinicNum;//All claims for the queueItems have same clinic, due to validation above.
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listClaimSendQueueItemsRetVal[0].ClearinghouseNum);
			Clearinghouse clearinghouseClinic=Clearinghouses.OverrideFields(clearinghouseHq,claimClinicNum);
			EnumClaimMedType medType=claim.MedType;
			//Already validated that all claims are for the same clearinghouse, clinic, and medType.
			//Validated that medtype matches clearinghouse e-format
			using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
			Eclaims.SendBatch(clearinghouseClinic,listClaimSendQueueItemsRetVal,medType,formClaimFormItemEdit,
				FormClaimPrint.FillRenaissance,new FormTerminalConnection());//this also calls SetClaimSentOrPrinted which creates the etrans entry.
			return listClaimSendQueueItemsRetVal;
		}

		///<summary>Sets the ClaimSendQueueItem.IsValid flag. Checks if the ClaimSendQueueItem passed in has any missing data.</summary>
		public static ClaimSendQueueItem SetClaimItemIsValid(ClaimSendQueueItem claimSendQueueItem,Clearinghouse clearinghouseClin) {
			if(claimSendQueueItem.IsValid) {
				return claimSendQueueItem;//no need to check. ClaimItem is valid already.
			}
			claimSendQueueItem=Eclaims.GetMissingData(clearinghouseClin,claimSendQueueItem);
			if(claimSendQueueItem.MissingData=="") {
				claimSendQueueItem.IsValid=true;
			}
			return claimSendQueueItem;
		}

		///<summary>Returns ClaimIsValidState.True if given claim is valid.
		///Does NOT check for Canadian warnings.</summary>
		public static ClaimIsValidState ClaimIsValid(Claim claim,List<ClaimProc> listClaimProcsForClaim) {
			return ClaimIsValid(claim.DateService.ToShortDateString(),claim.ClaimType, claim.ClaimStatus.In("S","R"),claim.DateSent.ToShortDateString(),listClaimProcsForClaim,
				claim.PlanNum,null,claim.ClaimNote,claim.UniformBillType,claim.CorrectionType
			);
		}

		///<summary>Returns ClaimIsValidState.True if given claim is valid.
		///Does NOT check for Canadian warnings.
		///This should be called when there is a UI that the user can make changes to that might not be saved in the claim object.</summary>
		public static ClaimIsValidState ClaimIsValid(string dateService,string claimType,bool isSentOrReceived,string claimDateSent,List<ClaimProc> listClaimProcsForClaim
			,long claimPlanNum,List<InsPlan> listInsPlans,string claimNote,string claimUniformBillType,ClaimCorrectionType claimCorrectionType)
		{
			if(dateService=="" && claimType!="PreAuth"){
				MsgBox.Show("Claims","Please enter a date of service");
				return ClaimIsValidState.False;
			}
			if(isSentOrReceived && claimDateSent=="") {
				MsgBox.Show("Claims","Please enter date sent.");
				return ClaimIsValidState.False;
			}
			if(claimType=="PreAuth") {
				bool hasStatusChanged=false;
				for(int i=0;i<listClaimProcsForClaim.Count;i++){
					if(listClaimProcsForClaim[i].Status!=ClaimProcStatus.Preauth){
						listClaimProcsForClaim[i].Status=ClaimProcStatus.Preauth;
						ClaimProcs.Update(listClaimProcsForClaim[i]);
						hasStatusChanged=true;
					}
				}
				if(hasStatusChanged) {
					InsBlueBooks.DeleteByClaimNums(listClaimProcsForClaim.Select(x => x.ClaimNum).ToArray());//We don't keep insbluebook entries for PreAuths.
					MsgBox.Show("Claims","Status of procedures was changed back to preauth to match status of claim.");
					return ClaimIsValidState.FalseClaimProcsChanged;
				}
			}
			if(PrefC.GetBool(PrefName.ClaimsValidateACN)) {
				InsPlan insPlan=InsPlans.GetPlan(claimPlanNum,listInsPlans);//Does a query if listInsPlans is null or if claimPlanNum is not in list.
				if(insPlan!=null && insPlan.GroupName.Contains("ADDP")) {
					if(!Regex.IsMatch(claimNote,"ACN[0-9]{5,}")) {//ACN with at least 5 digits following
						MsgBox.Show("Claims","For an ADDP claim, there must be an ACN number in the note.  Example format: ACN12345");
						return ClaimIsValidState.False;
					}
				}
			}
			if(claimUniformBillType!="" && claimCorrectionType!=ClaimCorrectionType.Original) {
				MsgBox.Show("Claims","Correction type must be original when type of bill is not blank.");
				return ClaimIsValidState.False;
			}
			return ClaimIsValidState.True;
		}

		///<summary>Returns true if a procedure is over-credited in a way that violates one of the preferences that blocks insurance overpayment.
		///This is in ClaimL because the methods called will show message boxes.</summary>
		public static bool AreCreditsGreaterThanProcFee(List<ClaimProc> listClaimProcsHypothetical) {
			if(IsInitialPrimaryInsGreaterThanProcFees(listClaimProcsHypothetical)){
				return true;
			}
			if(AreWriteOffsGreaterThanProcFees(listClaimProcsHypothetical)){
				return true;
			}
			return AreAllCreditsGreaterThanProcFees(listClaimProcsHypothetical);
		}

		///<summary>Returns true if the sum of all credits are not allowed to exceed adjusted procedure fees and one or more procedures are over-credited.
		///Displays a message detailing how the procedures are over-credited, and what the remaining balances would be.</summary>
		public static bool AreAllCreditsGreaterThanProcFees(List<ClaimProc> listClaimProcsHypothetical) {
			ClaimProcCreditsGreaterThanProcFee claimProcCreditsGreaterThanProcFee=PrefC.GetEnum<ClaimProcCreditsGreaterThanProcFee>(PrefName.ClaimProcAllowCreditsGreaterThanProcFee);
			if(claimProcCreditsGreaterThanProcFee==ClaimProcCreditsGreaterThanProcFee.Allow) {
				return false;
			}
			List<string> listProcDescripts=Claims.GetAllCreditsGreaterThanProcFees(listClaimProcsHypothetical);
			//list will be empty if there are no claimprocs greater than procedure fee, or if the procedure
			if(listProcDescripts.IsNullOrEmpty()) {
				return false;
			}
			if(claimProcCreditsGreaterThanProcFee==ClaimProcCreditsGreaterThanProcFee.Block) {
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g("FormClaimPayTotal","Remaining amount is negative for the following procedures")+":\r\n"
					+string.Join("\r\n",listProcDescripts)+"\r\n"+Lan.g("FormClaimPayTotal","Not allowed to continue."));
				msgBoxCopyPaste.Text=Lan.g("FormClaimPayTotal","Overpaid Procedure Warning");
				msgBoxCopyPaste.ShowDialog();
				return true;
			}
			if(claimProcCreditsGreaterThanProcFee==ClaimProcCreditsGreaterThanProcFee.Warn) {
				return MessageBox.Show(Lan.g("FormClaimPayTotal","Remaining amount is negative for the following procedures")+":\r\n"
					+string.Join("\r\n",listProcDescripts.Take(10))+"\r\n"+(listProcDescripts.Count>10?"...\r\n":"")+Lan.g("ClaimL","Continue?")
					,Lan.g("FormClaimPayTotal","Overpaid Procedure Warning"),MessageBoxButtons.OKCancel)==DialogResult.Cancel;
			} 
			return true;//should never get to this line, only possible if another enum value is added to allow, warn, and block
		}

		///<summary>Returns true if write-offs are not allowed to exceed adjusted procedure fees, and one or more procedures are over-credited by write-offs.
		///Displays a message detailing how the procedures are over-credited, and what the remaining balances would be.</summary>
		public static bool AreWriteOffsGreaterThanProcFees(List<ClaimProc> listClaimProcsHypothetical) {
			if(!PrefC.GetBool(PrefName.InsPayNoWriteoffMoreThanProc)) {
				return false;//InsPayNoWriteoffMoreThanProc preference is off. No need to check.
			}
			List<string> listProcDescripts=Claims.GetWriteOffsGreaterThanProcFees(listClaimProcsHypothetical);
			if(!listProcDescripts.IsNullOrEmpty()) {
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g("FormClaimPayTotal","Write-off amount is greater than the adjusted procedure fee for the following "
					+"procedure(s)")+":\r\n"+string.Join("\r\n",listProcDescripts)+"\r\n"+Lan.g("FormClaimPayTotal","Not allowed to continue."));
				msgBoxCopyPaste.Text=Lan.g("FormClaimPayTotal","Excessive Write-off");
				msgBoxCopyPaste.ShowDialog();
			}
			return !listProcDescripts.IsNullOrEmpty();
		}

		///<summary>Returns true if the sum of the pay amount and write-off from the initial primary insurance payment are not allowed to exceed adjusted procedure fees,
		///and one or more procedures are over-credited in this way. Displays a message detailing how the procedures are over-credited, and what the remaining balances would be.</summary>
		public static bool IsInitialPrimaryInsGreaterThanProcFees(List<ClaimProc> listClaimProcsHypothetical) {
			if(!PrefC.GetBool(PrefName.InsPayNoInitialPrimaryMoreThanProc)) {
				return false;//NoInitialPrimaryInsMoreThanProc preference is off. No need to check.
			}
			List<string> listProcDescripts=Claims.GetInitialPrimaryInsGreaterThanProcFees(listClaimProcsHypothetical);
			if(!listProcDescripts.IsNullOrEmpty()) {
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g("FormClaimPayTotal","The sum of the initial primary insurance payment and write-off is greater "
				+"than the adjusted procedure fee for the following procedure(s)")
				+":\r\n"+string.Join("\r\n",listProcDescripts)+"\r\n"+Lan.g("FormClaimPayTotal","Not allowed to continue."));
				msgBoxCopyPaste.Text=Lan.g("FormClaimPayTotal","Overpaid Procedure");
				msgBoxCopyPaste.ShowDialog();
			}
			return !listProcDescripts.IsNullOrEmpty();
		}
	}

	///<summary>Helper class for passing around data required to create a claim. Also contains informational variables for consuming methods.
	///This class helps so that we no longer have to pass around DataTables and ODGrids but instead can have a strongly typed object.</summary>
	public class CreateClaimDataWrapper {
		///<summary>The currently selected patient that is having a claim created.</summary>
		public Patient Patient_;
		///<summary>The family of Pat.</summary>
		public Family Family_;
		///<summary>Pertinent insurance information for the corresponding patient and family.</summary>
		public AccountModules.CreateClaimData CreateClaimData_;
		///<summary>Technically this is just a list of account items in the sense that it is almost always comprised from account grids.
		///These account items can represent anything selected in a grid that was showing to the user (typically an account grid from the Account module).
		///Methods that utilize this class will know how to filter through these items in order to find the ones they care about.
		///The main purpose of this list is to represent which grid items were selected (or not selected) and to what procedures they are associated to.
		///This list allows us to stop passing around an ODGrid / DataTable combination which was causing concurrency bugs to get submitted.</summary>
		public List<CreateClaimItem> ListCreateClaimItems;
		///<summary>A count of how many claims were created. This variable is handled by helper methods and should not be set manually.</summary>
		public int CountClaimsCreated;
		///<summary>An indicator to the consuming method so that they know if they need to refresh their UI or not.
		///Old comment: True if the Account module needs to be refreshed (old comment from ContrAccount.toolBarButIns_Click()).
		///This variable is handled by helper methods and should not be set manually.</summary>
		public bool ShouldRefresh;
		///<summary>Set to true if any errors occurred when creating a claim; Otherwise, false.
		///ErrorMessage should be the only other value trusted when this is set to true, no other information should be trusted.
		///This variable is handled by helper methods and should not be set manually.</summary>
		public bool HasError;
		///<summary>Additional information to help determine what errors happened while trying to create claims.
		///Will typically be set to a detailed error to display to the user when HasError is true.
		///However, it can still be empty even when an error occurred so HasError should be the indicator that an error occurred.
		///This variable is handled by helper methods and should not be set manually.</summary>
		public string ErrorMessage;
	}

	///<summary>Represents a selected item (or all items) from a grid. Typically an item from the account grid in the Account module.
	///Methods that utilize this class will know how to filter through these items in order to find the ones they care about.
	///Therefore any value is acceptable for any of the variables within this class. E.g. an item with a ProcNum of 0 represents a non-procedure item.
	///This object allows us to stop passing around an ODGrid / DataTable combination which was causing concurrency bugs to get submitted.</summary>
	public class CreateClaimItem {
		///<summary>A value greater than 0 will indicate that this item represents a procedure.</summary>
		public long ProcNum;
		///<summary>A value greater than 0 will indicate that this item represents a lab. Currently only used by Canadians.</summary>
		public long ProcNumLab;
		///<summary>The charge associated to this item. Typically represents a procedure fee.</summary>
		public double ChargesDouble;
		///<summary>Set to true if claim creation logic should consider this item for the claim that is being created; Otherwise, false.
		///This variable is typically set by helper methods unless this item needs to be treated as if it were manually selected.
		///Canadians have a hard limit of 7 items selected per claim so this value may change regardless of how it was instantiated.</summary>
		public bool IsSelected;
	}

	public enum ClaimIsValidState {
		False,
		FalseClaimProcsChanged,
		True,
	}

	public enum BoolOverride {
		Undefined,
		False,
		True
	}
}