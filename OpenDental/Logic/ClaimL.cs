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
		///Shows the message passed in to the user if isVerbose is true.  Be sure to translate message before calling this function.</summary>
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
		///<param name="table">This table must have three columns at minimum: ProcNum, ProcNumLab, and chargesDouble</param>
		///<param name="arraySelectedIndices">Any selected rows in the corresponding table.  An empty array is acceptable.</param>
		///<returns>List of objects that can be used for creating claims instead of directly utilizing DataTable\ODGrid objects directly.</returns>
		public static List<CreateClaimItem> GetCreateClaimItems(DataTable table,int[] arraySelectedIndices) {
			List<DataRow> listRows=table.Select().ToList();
			//If the user manually selected items in the grid then we want to only include the selected rows.
			bool hasSelections=(arraySelectedIndices.Length > 0);
			if(hasSelections) {
				listRows=listRows.Where((x,index) => ListTools.In(index,arraySelectedIndices)).ToList();
			}
			//Create CreateClaimItem objects from the data rows so that we have deep copied and strongly typed objects to work with in other methods.
			return listRows.Select(x => new CreateClaimItem() {
				ProcNum=PIn.Long(x["ProcNum"].ToString()),
				ProcNumLab=PIn.Long(x["ProcNumLab"].ToString()),
				ChargesDouble=PIn.Double(x["chargesDouble"].ToString()),
				IsSelected=hasSelections,
			}).ToList();
		}

		///<summary>Returns a CreateClaimDataWrapper object that is specifically designed for the claim creation process from within the UI.
		///It contains strongly typed variables which help indicate to the claim creation method how to correctly create the claim.
		///It also contains variables that indicate to consuming methods what happened during the claim creation process.
		///It is a requirement to have listCreateClaimItems filled with at least one item.
		///Optionally set isSelectionRequired to true if the user must have already selected indices within the grid passed in.
		///This method throws exceptions (specifcally for developers), shows messages, and other UI related actions during claim creation.</summary>
		public static CreateClaimDataWrapper GetCreateClaimDataWrapper(Patient pat,Family fam,List<CreateClaimItem> listCreateClaimItems,bool isVerbose
			,bool isSelectionRequired=false)
		{
			CreateClaimDataWrapper createClaimDataWrapper=new CreateClaimDataWrapper();
			if(listCreateClaimItems==null) {//An engineer incorrectly called this method.
				throw new ArgumentException("Invalid argument passed in.",nameof(listCreateClaimItems));
			}
			createClaimDataWrapper.ListCreateClaimItems=listCreateClaimItems;
			if(!Security.IsAuthorized(Permissions.ClaimView,true)) {
				LogClaimError(createClaimDataWrapper,Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(Permissions.ClaimView),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			if(listCreateClaimItems.Count < 1) {
				//There is nothing to do because at least one item is required in order to create a claim...
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			if(!CheckClearinghouseDefaults()) {
				createClaimDataWrapper.DoRefresh=false;
				createClaimDataWrapper.HasError=true;
				return createClaimDataWrapper;
			}
			createClaimDataWrapper.Pat=pat;
			createClaimDataWrapper.Fam=fam;
			createClaimDataWrapper.ClaimData=AccountModules.GetCreateClaimData(pat,fam);
			if(createClaimDataWrapper.ClaimData.ListPatPlans.Count==0) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Patient does not have insurance."),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			int countSelected=0;
			InsSub sub;
			if(listCreateClaimItems.All(x => !x.IsSelected)) {
				if(isSelectionRequired) {
					LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
					createClaimDataWrapper.DoRefresh=false;
					return createClaimDataWrapper;
				}
				//autoselect procedures
				foreach(CreateClaimItem item in listCreateClaimItems) {
					if(item.ProcNum==0) {
						continue;//ignore non-procedures
					}
					if(item.ChargesDouble==0) {
						continue;//ignore zero fee procedures, but user can explicitly select them
					}
					//payment rows skipped
					Procedure proc=Procedures.GetProcFromList(createClaimDataWrapper.ClaimData.ListProcs,item.ProcNum);
					ProcedureCode procCode=ProcedureCodes.GetFirstOrDefault(x => x.CodeNum==proc.CodeNum)??new ProcedureCode();
					if(procCode.IsCanadianLab) {
						continue;
					}
					int ordinal=PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.ClaimData.ListPatPlans
						,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs);
					int ordinalSec=PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.ClaimData.ListPatPlans
						,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs);
					if(ordinal==0) { //No primary dental plan. Must be a medical plan.  Use the first medical plan instead.
						ordinal=1;
					}
					sub=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.ClaimData.ListPatPlans,ordinal),createClaimDataWrapper.ClaimData.ListInsSubs);
					InsSub subSec=null;
					if(ordinalSec>0) {
						subSec=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.ClaimData.ListPatPlans,ordinalSec),createClaimDataWrapper.ClaimData.ListInsSubs);
					}
					//Select the item if any procedure needs primary or secondary insurance sent.
					if(Procedures.NeedsSent(proc.ProcNum,sub.InsSubNum,createClaimDataWrapper.ClaimData.ListClaimProcs) 
						|| (subSec!=null && Procedures.NeedsSent(proc.ProcNum,subSec.InsSubNum,createClaimDataWrapper.ClaimData.ListClaimProcs)))
					{
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && countSelected==7) {//Canadian. en-CA or fr-CA
							if(isVerbose) {//This is an informational message, not an error.
								MsgBox.Show("ContrAccount","Only the first 7 procedures will be automatically selected.\r\n"
								+"You will need to create another claim for the remaining procedures.");
							}
							break;//Continue creating the claim, but stop selecting procedures so that only 7 will be added to the claim.
						}
						countSelected++;
						item.IsSelected=true;
					}
				}
				if(listCreateClaimItems.All(x => !x.IsSelected)) {//if still none selected
					LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
					createClaimDataWrapper.DoRefresh=false;
					return createClaimDataWrapper;
				}
			}
			if(listCreateClaimItems.Any(x => x.IsSelected && x.ProcNum==0)) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","You can only select procedures."),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			//At this point, all selected items are procedures.  In Canada, the selections may also include labs.
			InsCanadaValidateProcs(createClaimDataWrapper,isVerbose);
			if(createClaimDataWrapper.ListCreateClaimItems.All(x => !x.IsSelected)) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Please select procedures first."),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			return createClaimDataWrapper;
		}

		///<summary>Returns the createClaimDataWrapper object that was passed in after manipulating it.
		///This object will contain information about what happened during the claim creation process (e.g. error messages, refresh indicator, etc).
		///E.g. CreateClaimDataWrapper.HasError will be true if any errors occurred.  ErrorMessage might contain additional information about the error.
		///This method assumes that createClaimDataWrapper was set up correctly (refer to GetCreateClaimDataWrapper() for how to set up this object).
		///Set flag hasPrimaryClaim to true if the primary claim has already been created and should not be recreated.
		///Set flag hasSecondaryClaim to true if the secondary claim has already been created and should not be recreated.</summary>
		public static CreateClaimDataWrapper CreateClaimFromWrapper(bool isVerbose,CreateClaimDataWrapper createClaimDataWrapper
			,bool hasPrimaryClaim=false,bool hasSecondaryClaim=false)
		{
			createClaimDataWrapper.ErrorMessage="";
			if(!Security.IsAuthorized(Permissions.ClaimView,true)) {
				LogClaimError(createClaimDataWrapper,Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(Permissions.ClaimView),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			createClaimDataWrapper.ClaimCreatedCount=0;
			if(!CheckClearinghouseDefaults()) {
				createClaimDataWrapper.DoRefresh=false;
				createClaimDataWrapper.HasError=true;
				return createClaimDataWrapper;
			}
			if(createClaimDataWrapper.ClaimData.ListPatPlans.Count==0) {
				LogClaimError(createClaimDataWrapper,Lan.g("ContrAccount","Patient does not have insurance."),isVerbose);
				createClaimDataWrapper.DoRefresh=false;
				return createClaimDataWrapper;
			}
			bool doCreateSecondaryClaim=!hasSecondaryClaim && PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.ClaimData.ListPatPlans,
				createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs)>0 //if there exists a secondary plan
				&& !CultureInfo.CurrentCulture.Name.EndsWith("CA");//And not Canada (don't create secondary claim for Canada)
			if(!hasPrimaryClaim) {
				string claimType="P";
				//If they have medical insurance and no dental, make the claim type Medical.  This is to avoid the scenario of multiple med ins and no dental.
				if(PatPlans.GetOrdinal(PriSecMed.Medical,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs)>0
					&& PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs)==0
					&& PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListInsSubs)==0) 
				{
					claimType="Med";
				}
				Claim claimCur=new Claim();
				claimCur.DateSent=DateTime.Today;
				claimCur.DateSentOrig=DateTime.MinValue;
				claimCur.ClaimStatus="W";
				//Set claimCur to what CreateClaim returns because the reference to claimCur gets broken when inserting.
				claimCur=CreateClaim(claimCur,claimType,isVerbose,createClaimDataWrapper,"Primary Claim Error");
				if(claimCur.ClaimNum==0) {
					createClaimDataWrapper.DoRefresh=true;
					return createClaimDataWrapper; 
				}
				if(isVerbose && claimCur.ClaimNum!=0) {//Only provide the user with the option to cancel the claim if attempting to create a single claim manually.
					using FormClaimEdit FormCE=new FormClaimEdit(claimCur,createClaimDataWrapper.Pat,createClaimDataWrapper.Fam);
					FormCE.IsNew=true;//this causes it to delete the claim if cancelling.
					FormCE.ShowDialog();
					if(FormCE.DialogResult!=DialogResult.OK) {
						createClaimDataWrapper.DoRefresh=true;//will have already been deleted
						return createClaimDataWrapper;
					}
					double unearnedAmount=(double)PaySplits.GetTotalAmountOfUnearnedForPats(createClaimDataWrapper.Fam.GetPatNums());
					//If there's unallocated amounts, we want to redistribute the money to other procedures.
					if(unearnedAmount>0) {
						AllocateUnearnedPayment(createClaimDataWrapper.Pat,createClaimDataWrapper.Fam,unearnedAmount,claimCur);
					}
				}
				else if(claimCur.ClaimNum!=0) {//isVerbose is false, still need to log.
					Patient patCur=createClaimDataWrapper.Pat;
					SecurityLogs.MakeLogEntry(Permissions.ClaimEdit,patCur.PatNum,"New claim created for "+patCur.LName+","+patCur.FName,
						claimCur.ClaimNum,claimCur.SecDateTEdit);
				}
			}
			if(doCreateSecondaryClaim) {
				//ClaimL.CalculateAndUpdate() could have added new claimprocs for additional insurance plans if they were added after the proc was completed
				createClaimDataWrapper.ClaimData.ListClaimProcs=ClaimProcs.Refresh(createClaimDataWrapper.Pat.PatNum);
				Claim claimCur=new Claim();
				claimCur.ClaimStatus="H";
				//Set ClaimCur to CreateClaim because the reference to ClaimCur gets broken when inserting.
				claimCur=CreateClaim(claimCur,"S",isVerbose,createClaimDataWrapper,"Secondary Claim Error");
				if(claimCur.ClaimNum==0) {
					createClaimDataWrapper.DoRefresh=true;
					return createClaimDataWrapper;
				}
				else {
					Patient patCur=createClaimDataWrapper.Pat;
					SecurityLogs.MakeLogEntry(Permissions.ClaimEdit,patCur.PatNum,"New claim created for "+patCur.LName+","+patCur.FName,
						claimCur.ClaimNum,claimCur.SecDateTEdit);
				}
			}
			createClaimDataWrapper.DoRefresh=true;
			return createClaimDataWrapper;
		}

		public static void AllocateUnearnedPayment(Patient patcur,Family famcur,double unearnedAmt,Claim ClaimCur) {
			//do not try to allocate payment if preference is disabled or if there isn't a payment to allocate
			if(!PrefC.GetBool(PrefName.ShowAllocateUnearnedPaymentPrompt) || ClaimProcs.GetPatPortionForClaim(ClaimCur)<=0) { 
				return;
			}
			using FormProcSelect FormPS=new FormProcSelect(patcur.PatNum,false,true,true);
			if(FormPS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			List<AccountEntry> listProcAccountEntries=PaymentEdit.CreateAccountEntries(FormPS.ListSelectedProcs);
			Payment paymentCur=new Payment();
			paymentCur.PayDate=DateTime.Today;
			paymentCur.PatNum=patcur.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			paymentCur.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					paymentCur.ClinicNum=patcur.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					paymentCur.ClinicNum=(Clinics.ClinicNum==0)?patcur.ClinicNum:Clinics.ClinicNum;
				}
				else {
					paymentCur.ClinicNum=Clinics.ClinicNum;
				}
			}
			paymentCur.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				paymentCur.PayType=listDefs[0].DefNum;
			}
			paymentCur.PaymentSource=CreditCardSource.None;
			paymentCur.ProcessStatus=ProcessStat.OfficeProcessed;
			paymentCur.PayAmt=0;
			Payments.Insert(paymentCur);
			using FormPayment FormP=new FormPayment(patcur,famcur,paymentCur,false);
			FormP.IsNew=true;
			FormP.UnearnedAmt=unearnedAmt;
			FormP.ListEntriesPayFirst=listProcAccountEntries;
			FormP.IsIncomeTransfer=true;
			FormP.ShowDialog();
		}

		///<summary>Only allows up to 7 CreateClaimItems to be selected within createClaimDataWrapper when Canadian.
		///Shows a message to the user stating this fact if enforced; Otherwise does nothing.</summary>
		private static void InsCanadaValidateProcs(CreateClaimDataWrapper createClaimDataWrapper,bool isVerbose) {
			List<CreateClaimItem> listToolBarInsSelectedItems=createClaimDataWrapper.ListCreateClaimItems.FindAll(x => x.IsSelected);
			int procCount=listToolBarInsSelectedItems.Count(x => x.ProcNumLab==0);//Not a lab
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && procCount > 7) {//Canadian. en-CA or fr-CA
				//Unselect all but the first 7 procedures with the smallest index numbers.
				int procSelectedCount=0;
				foreach(CreateClaimItem item in listToolBarInsSelectedItems) {
					item.IsSelected=(procSelectedCount < 7);
					if(item.ProcNumLab==0) {//Not a lab
						procSelectedCount++;
					}
				}
				string message=Lan.g("ContrAccount","Only the first 7 procedures will be selected.  "
					+"You will need to create another claim for the remaining procedures.");
				LogClaimError(createClaimDataWrapper,message,isVerbose);
			}
		}

		///<summary>All validation on the procedures is done here.  Creates and saves the claim initially, attaching all selected procedures.
		///But it does not refresh any data, does not do a final update of the new claim, and does not enter fee amounts.
		///claimType=P,S,Med,or Other
		///This method assumes that createClaimDataWrapper was set up correctly (refer to GetCreateClaimDataWrapper() for how to set up this object).
		///Returns a 'new' claim object (ClaimNum=0) to indicate that the user does not want to create the claim or there are validation issues..</summary>
		public static Claim CreateClaim(Claim claimCur,string claimType,bool isVerbose,CreateClaimDataWrapper createClaimDataWrapper,string msgBoxHeader="") {
			Patient pat=createClaimDataWrapper.Pat;
			InsPlan PlanCur=new InsPlan();
			InsSub SubCur=new InsSub();
			Relat relatOther=Relat.Self;
			string claimTypeDesc="";
			switch(claimType){
				case "P":
					SubCur=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.ClaimData.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans
						,createClaimDataWrapper.ClaimData.ListInsSubs)),createClaimDataWrapper.ClaimData.ListInsSubs);
					PlanCur=InsPlans.GetPlan(SubCur.PlanNum,createClaimDataWrapper.ClaimData.ListInsPlans);
					claimTypeDesc="primary";
					break;
				case "S":
					SubCur=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.ClaimData.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans
						,createClaimDataWrapper.ClaimData.ListInsSubs)),createClaimDataWrapper.ClaimData.ListInsSubs);
					PlanCur=InsPlans.GetPlan(SubCur.PlanNum,createClaimDataWrapper.ClaimData.ListInsPlans);
					claimTypeDesc="secondary";
					break;
				case "Med":
					//It's already been verified that a med plan exists
					SubCur=InsSubs.GetSub(PatPlans.GetInsSubNum(createClaimDataWrapper.ClaimData.ListPatPlans
						,PatPlans.GetOrdinal(PriSecMed.Medical,createClaimDataWrapper.ClaimData.ListPatPlans,createClaimDataWrapper.ClaimData.ListInsPlans
						,createClaimDataWrapper.ClaimData.ListInsSubs)),createClaimDataWrapper.ClaimData.ListInsSubs);
					PlanCur=InsPlans.GetPlan(SubCur.PlanNum,createClaimDataWrapper.ClaimData.ListInsPlans);
					claimTypeDesc="medical";
					break;
				case "Other":
					using(FormClaimCreate FormCC=new FormClaimCreate(pat.PatNum)) {
						FormCC.ShowDialog();
						if(FormCC.DialogResult!=DialogResult.OK){
							return new Claim();
						}
						PlanCur=FormCC.SelectedPlan;
						SubCur=FormCC.SelectedSub;
						relatOther=FormCC.PatRelat;
					}
					break;
			}
			List<long> listSelectedProcNums=createClaimDataWrapper.ListCreateClaimItems
				.Where(x => x.IsSelected)
				.Select(x => x.ProcNum).ToList();
			List<Procedure> listSelectedProcs=createClaimDataWrapper.ClaimData.ListProcs.FindAll(x => ListTools.In(x.ProcNum,listSelectedProcNums));
			List<Procedure> listBillInsProcs=listSelectedProcs.FindAll(x => !Procedures.NoBillIns(x,createClaimDataWrapper.ClaimData.ListClaimProcs,PlanCur.PlanNum));
			List<Procedure> listNoBillInsProcs=listSelectedProcs.FindAll(x => !ListTools.In(x.ProcNum,listBillInsProcs.Select(y => y.ProcNum)));
			//If all the procedures are marked as NoBillIns, then warn the user that the primary/secondary claim will not get created and then return a new Claim.
			if(listSelectedProcs.Count>0 && listBillInsProcs.Count==0) {
				//No claim to create since we will be creating a secondary. Continue on to the secondary. Nothing to log.
				MsgBox.Show($"No {claimTypeDesc} claim will be created due to all procedures being marked as NoBillIns.");
				return new Claim();
			}
			if(listNoBillInsProcs.Count > 0) {
				StringBuilder sbMsg=new StringBuilder($"The following procedures were marked as NoBillIns. They will be excluded from the {claimTypeDesc} claim.\r\n");
				foreach(Procedure proc in listNoBillInsProcs) {
					ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
					sbMsg.AppendLine($"{procCode.ProcCode} {(string.IsNullOrEmpty(proc.ToothNum) ? "" : ($"Tth {proc.ToothNum} "))} - {procCode.Descript}");
				}
				MsgBox.Show(sbMsg.ToString());
			}
			//If we are going to block based on a preference, do that before figuring out other claim validation.
			//Ignore "No Bill Ins" here, because we want "No Bill Ins" to be the more important block for backwards compatability.
			switch(PIn.Enum<ClaimZeroDollarProcBehavior>(PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior))) {
				case ClaimZeroDollarProcBehavior.Warn:
					if(listBillInsProcs.Any(x => CompareDouble.IsZero(x.ProcFee))
						&& MessageBox.Show(Lan.g("ContrAccount","You are about to make a")+" "+(claimTypeDesc=="" ? "" : (claimTypeDesc+" "))
							+Lan.g("ContrAccount","claim that will include a $0 procedure.  Continue?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
						//Nothing to log.  The user hit Cancel.
						return new Claim();
					}
					break;
				case ClaimZeroDollarProcBehavior.Block:
					if(listBillInsProcs.Any(x => CompareDouble.IsZero(x.ProcFee))) {
						MsgBox.Show("ContrAccount","You can't make a claim for a $0 procedure.");
						//Nothing to log.  The user was just notified.
						return new Claim();
					}
					break;
				case ClaimZeroDollarProcBehavior.Allow:
				default:
					break;
			}
			ODTuple<bool,Claim,string> result=AccountModules.CreateClaim(claimCur,claimType,createClaimDataWrapper.ClaimData.ListPatPlans
				,createClaimDataWrapper.ClaimData.ListInsPlans,createClaimDataWrapper.ClaimData.ListClaimProcs,createClaimDataWrapper.ClaimData.ListProcs
				,createClaimDataWrapper.ClaimData.ListInsSubs,pat,createClaimDataWrapper.ClaimData.PatNote,listBillInsProcs
				,createClaimDataWrapper.ErrorMessage,PlanCur,SubCur,relatOther);
			bool isSuccess=result.Item1;
			claimCur=result.Item2;
			string claimError=result.Item3;
			if(isSuccess) {
				createClaimDataWrapper.ClaimCreatedCount++;
			}
			else if(!string.IsNullOrEmpty(claimError)) {
				LogClaimError(createClaimDataWrapper,claimError,isVerbose,msgBoxHeader);
			}
			return claimCur;
		}

		public static void PromptForSecondaryClaim(List<ClaimProc> _listClaimProcsForClaim) {
			//Get a list of ProcNums from the primary claim.
			List<long> listProcNumsOnPriClaim=_listClaimProcsForClaim.Where(x => x.ProcNum>0).Select(x => x.ProcNum).ToList();
			//Get list of ClaimProcs for the list of ProcNums on Primary claim. Make sure the claim procs are not received and are attached to a claim.
			List<ClaimProc> listClaimProcsForPriProcsNotReceived=ClaimProcs.GetForProcs(listProcNumsOnPriClaim)
				.Where(x=>x.Status==ClaimProcStatus.NotReceived && x.ClaimNum!=0).ToList();
			if(listClaimProcsForPriProcsNotReceived.Count==0) {
				return;//No unreceived claimprocs for procs on pri claim.
			}
			//We have unreceived claimprocs for the ProcNums that are attached to the primary claim. We need to find out if the claim procs 
			//are attached to a secondary claim with status of 'Sent' or 'Hold Until Pri Received'
			List<Claim> listSecondaryClaims=Claims.GetClaimsFromClaimNums(listClaimProcsForPriProcsNotReceived.Select(x => x.ClaimNum).ToList())
				.Where(x=>ListTools.In(x.ClaimStatus,"U","H") && x.ClaimType=="S").ToList();
			if(listSecondaryClaims.Count==0) {
				return;//No secondary claims for the procedures attached to the primary.
			}
			string msg=Lan.g("ContrAccount","There is at least one unsent secondary claim for the received procedures.");
			msg+="\r\n";
			msg+=Lan.g("ContrAccount","Would you like to:");
			using InputBox inputBox=new InputBox(new List<InputBoxParam>()
			{
				new InputBoxParam(InputBoxType.RadioButton,msg,Lan.g("ContrAccount","Change the claim status to 'Waiting to send'"),Size.Empty),
				new InputBoxParam(InputBoxType.RadioButton,"",Lan.g("ContrAccount","Send secondary claim(s) now"),Size.Empty),
				new InputBoxParam(InputBoxType.RadioButton,"",Lan.g("ContrAccount","Do nothing"),Size.Empty)
			});
			inputBox.setTitle(Lan.g("ContrAccount","Outstanding secondary claims"));
			inputBox.SizeInitial=new Size(450,200);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			RadioButton selectedRadioButton=inputBox.PanelClient.Controls.OfType<RadioButton>().Where(x=>x.Checked).First();
			if(selectedRadioButton.Text.Contains("Do nothing")) {
				return;
			}
			//We need to update claims status to 'Waiting to Send' regardless of what option they check. See Claims.GetQueueList(...) below.
			foreach(Claim claim in listSecondaryClaims) {
				claim.ClaimStatus="W";
				Claims.Update(claim);
			}
			if(selectedRadioButton.Text.Contains("Send secondary claim")) {
				//Most likely all of the procedures on the primary claim will have all of the procedures on 1 secondary claim. Expecially since most of time the 
				//claim will be created automatically. The only time they don't get created automatically is when the patient doesn't have a secondary claim 
				//at the time the primary claim gets created. Even if the user created the claim manually, the chances that the procedures on the primary have more than
				//one claim are low.
				ClaimSendQueueItem[] listQueue=Claims.GetQueueList(listSecondaryClaims.Select(x=>x.ClaimNum).ToList(),0,0);
				SendClaimSendQueueItems(listQueue.ToList(),0);//Use clearinghouseNum of 0 to indicate automatic calculation of clearinghouse
			}
		}
		
		///<summary>Validates and sends each of the ClaimSendQueueItems passed in. Returns a list of the ClaimSendQueueItem that were sent.</summary>
		internal static List<ClaimSendQueueItem> SendClaimSendQueueItems(List<ClaimSendQueueItem> listClaimSendQueueItems,long hqClearinghouseNum) {
			List<ClaimSendQueueItem> retVal=new List<ClaimSendQueueItem>();//a list of queue items to send
			if(listClaimSendQueueItems.Count==0) {
				return retVal;
			}
			if(PrefC.HasClinicsEnabled) {//Clinics is in use
				long clinicNum0=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum).ClinicNum;
				for(int i=1;i<listClaimSendQueueItems.Count;i++) {
					long clinicNum=Claims.GetClaim(listClaimSendQueueItems[i].ClaimNum).ClinicNum;
					if(clinicNum0!=clinicNum) {
						MsgBox.Show("ContrAccount","All claims must be for the same clinic.  You can use the combobox at the top to filter.");//TODO: Wording.
						return retVal;
					}
				}
			}
			long clearinghouseNum0=listClaimSendQueueItems[0].ClearinghouseNum;
			EnumClaimMedType medType0=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum).MedType;
			int index=0;
			foreach(ClaimSendQueueItem claimSendItem in listClaimSendQueueItems) {//we start with 0 so that we can check medtype match on the first claim
				long clearinghouseNumI=claimSendItem.ClearinghouseNum;
				if(clearinghouseNum0!=clearinghouseNumI) {
					MsgBox.Show("ContrAccount","All claims must be for the same clearinghouse.");
					return retVal;
				}
				EnumClaimMedType medTypeI=Claims.GetClaim(claimSendItem.ClaimNum).MedType;
				if(medType0!=medTypeI) {
					MsgBox.Show("ContrAccount","All claims must have the same MedType.");
					return retVal;
				}
				Clearinghouse clearh=Clearinghouses.GetClearinghouse(clearinghouseNumI);
				if(clearh.Eformat==ElectronicClaimFormat.x837D_4010 || clearh.Eformat==ElectronicClaimFormat.x837D_5010_dental) {
					if(medTypeI!=EnumClaimMedType.Dental) {
						MsgBox.Show("ContrAccount","On claim "+POut.Int(index)+", the MedType does not match the clearinghouse e-format.");
						return retVal;
					}
				}
				if(clearh.Eformat==ElectronicClaimFormat.x837_5010_med_inst) {
					if(medTypeI!=EnumClaimMedType.Medical && medTypeI!=EnumClaimMedType.Institutional) {
						MsgBox.Show("ContrAccount","On claim "+POut.Int(index)+", the MedType does not match the clearinghouse e-format.");
						return retVal;
					}
				}
				if(claimSendItem.HasIcd9) {
					string msgText=Lan.g("ContrAccount","There are ICD-9 codes attached to a procedure.  Would you like to send the claim without the ICD-9 codes? ");
					if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
						return retVal;
					}
				}
				//This is done for PromptForSecondaryClaim(...).
				//SendEclaimsToClearinghouse(...) already validates items in listClaimSendQueueItems, SetClaimItemIsValid(...) will just return in this case.
				SetClaimItemIsValid(claimSendItem,clearh);
				if(!claimSendItem.IsValid && claimSendItem.CanSendElect) {
					MsgBox.Show("ContrAccount","Not allowed to send e-claims with missing information.");
					return retVal;
				}
				if(claimSendItem.NoSendElect==NoSendElectType.NoSendElect) {
					MsgBox.Show("ContrAccount","Not allowed to send e-claims.");
					return retVal;
				}
				if(claimSendItem.NoSendElect==NoSendElectType.NoSendSecondaryElect && claimSendItem.Ordinal!=1) {
					MsgBox.Show("ContrAccount","Only allowed to send primary insurance e-claims.");
					return retVal;
				}
				index++;
			}
			foreach(ClaimSendQueueItem claimSendItem in listClaimSendQueueItems) {
				ClaimSendQueueItem queueitem=claimSendItem.Copy();
				if(hqClearinghouseNum!=0) {
					queueitem.ClearinghouseNum=hqClearinghouseNum;
				}
				retVal.Add(queueitem);
			}
			Claim claim0=Claims.GetClaim(listClaimSendQueueItems[0].ClaimNum);
			long claimClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				claimClinicNum=claim0.ClinicNum;//All claims for the queueItems have same clinic, due to validation above.
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(retVal[0].ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,claimClinicNum);
			EnumClaimMedType medType=claim0.MedType;
			//Already validated that all claims are for the same clearinghouse, clinic, and medType.
			//Validated that medtype matches clearinghouse e-format
			using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
			Eclaims.SendBatch(clearinghouseClin,retVal,medType,formClaimFormItemEdit,
				FormClaimPrint.FillRenaissance,new FormTerminalConnection());//this also calls SetClaimSentOrPrinted which creates the etrans entry.
			return retVal;
		}

		///<summary>Sets the ClaimSendQueueItem.IsValid flag. Checks if the ClaimSendQueueItem passed in has any missing data.</summary>
		public static void SetClaimItemIsValid(ClaimSendQueueItem claimSendQueueItem,Clearinghouse clearinghouseClin) {
			if(claimSendQueueItem.IsValid) {
				return;//no need to check. ClaimItem is valid already.
			}
			claimSendQueueItem=Eclaims.GetMissingData(clearinghouseClin,claimSendQueueItem);
			if(claimSendQueueItem.MissingData=="") {
				claimSendQueueItem.IsValid=true;
			}
		}

		///<summary>Returns ClaimIsValidState.True if given claim is valid.
		///Does NOT check for Canadian warnings.</summary>
		public static ClaimIsValidState ClaimIsValid(Claim claim,List<ClaimProc> listClaimProcsForClaim) {
			return ClaimIsValid(claim.DateService.ToShortDateString(),claim.ClaimType,ListTools.In(claim.ClaimStatus,"S","R"),claim.DateSent.ToShortDateString(),listClaimProcsForClaim,
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
				foreach(ClaimProc claimProc in listClaimProcsForClaim){
					if(claimProc.Status!=ClaimProcStatus.Preauth){
						claimProc.Status=ClaimProcStatus.Preauth;
						ClaimProcs.Update(claimProc);
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
				InsPlan plan=InsPlans.GetPlan(claimPlanNum,listInsPlans);//Does a query if listInsPlans is null or if claimPlanNum is not in list.
				if(plan!=null && plan.GroupName.Contains("ADDP")) {
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
	}

	///<summary>Helper class for passing around data required to create a claim.  Also contains informational variables for consuming methods.
	///This class helps so that we no longer have to pass around DataTables and ODGrids but instead can have a strongly typed object.</summary>
	public class CreateClaimDataWrapper {
		///<summary>The currently selected patient that is having a claim created.</summary>
		public Patient Pat;
		///<summary>The family of Pat.</summary>
		public Family Fam;
		///<summary>Pertinent insurance information for the corresponding patient and family.</summary>
		public AccountModules.CreateClaimData ClaimData;
		///<summary>Technically this is just a list of account items in the sense that it is almost always comprised from account grids.
		///These account items can represent anything selected in a grid that was showing to the user (typically an account grid from the Account module).
		///Methods that utilize this class will know how to filter through these items in order to find the ones they care about.
		///The main purpose of this list is to represent which grid items were selected (or not selected) and to what procedures they are associated to.
		///This list allows us to stop passing around an ODGrid / DataTable combination which was causing concurrency bugs to get submitted.</summary>
		public List<CreateClaimItem> ListCreateClaimItems;
		///<summary>A count of how many claims were created.  This variable is handled by helper methods and should not be set manually.</summary>
		public int ClaimCreatedCount;
		///<summary>An indicator to the consuming method so that they know if they need to refresh their UI or not.
		///Old comment:  True if the Account module needs to be refreshed (old comment from ContrAccount.toolBarButIns_Click()).
		///This variable is handled by helper methods and should not be set manually.</summary>
		public bool DoRefresh;
		///<summary>Set to true if any errors occurred when creating a claim;  Otherwise, false.
		///ErrorMessage should be the only other value trusted when this is set to true, no other information should be trusted.
		///This variable is handled by helper methods and should not be set manually.</summary>
		public bool HasError;
		///<summary>Additional information to help determine what errors happened while trying to create claims.
		///Will typically be set to a detailed error to display to the user when HasError is true.
		///However, it can still be empty even when an error occurred so HasError should be the indicator that an error occurred.
		///This variable is handled by helper methods and should not be set manually.</summary>
		public string ErrorMessage;
	}

	///<summary>Represents a selected item (or all items) from a grid.  Typically an item from the account grid in the Account module.
	///Methods that utilize this class will know how to filter through these items in order to find the ones they care about.
	///Therefore any value is acceptable for any of the variables within this class.  E.g. an item with a ProcNum of 0 represents a non-procedure item.
	///This object allows us to stop passing around an ODGrid / DataTable combination which was causing concurrency bugs to get submitted.</summary>
	public class CreateClaimItem {
		///<summary>A value greater than 0 will indicate that this item represents a procedure.</summary>
		public long ProcNum;
		///<summary>A value greater than 0 will indicate that this item represents a lab.  Currently only used by Canadians.</summary>
		public long ProcNumLab;
		///<summary>The charge associated to this item.  Typically represents a procedure fee.</summary>
		public double ChargesDouble;
		///<summary>Set to true if claim creation logic should consider this item for the claim that is being created;  Otherwise, false.
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
