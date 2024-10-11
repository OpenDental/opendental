using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	public class PaymentEdit {

		#region FormPayment
		///<summary>Gets most all the data needed to load FormPayment.</summary>
		public static LoadData GetLoadData(Patient patCur,Payment paymentCur,bool isNew,bool isIncomeTxfr) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),patCur,paymentCur,isNew,isIncomeTxfr);
			}
			LoadData data=new LoadData();
			data.PatCur=patCur;
			data.Fam=Patients.GetFamily(patCur.PatNum);
			data.SuperFam=new Family(Patients.GetBySuperFamily(patCur.SuperFamily));
			data.ListCreditCards=CreditCards.Refresh(patCur.PatNum);
			data.XWebResponse=XWebResponses.GetOneByPaymentNum(paymentCur.PayNum);
			data.PayConnectResponseWeb=PayConnectResponseWebs.GetOneByPayNum(paymentCur.PayNum);
			data.CareCreditWebResponse=CareCreditWebResponses.GetOneByPayNum(paymentCur.PayNum);
			data.PaymentCur=paymentCur;
			data.TableBalances=Patients.GetPaymentStartingBalances(patCur.Guarantor,paymentCur.PayNum);
			data.TableBalances.TableName="TableBalances";
			data.ListSplits=PaySplits.GetForPayment(paymentCur.PayNum);
			if(!isNew) {
				data.Transaction=Transactions.GetAttachedToPayment(paymentCur.PayNum);
			}
			data.ListValidPayPlans=PayPlans.GetValidPlansNoIns(patCur.PatNum);
			List<long> listFamilyPatNums=data.Fam.GetPatNums();
			if(patCur.SuperFamily > 0) {
				//Add all of the super family members to listFamilyPatNums if there are any splits for super family members outside of the direct family.
				if(data.ListSplits.Any(x => !listFamilyPatNums.Contains(x.PatNum) && data.SuperFam.GetPatNums().Contains(x.PatNum))) {
					listFamilyPatNums.AddRange(data.SuperFam.ListPats.Select(x => x.PatNum));
				}
			}
			//if there were no payment plans found where this patient is the guarantor, find payment plans in the family.
			if(data.ListValidPayPlans.Count == 0) {
				//Do not include insurance payment plans.
				data.ListValidPayPlans=PayPlans.GetForPats(listFamilyPatNums,patCur.PatNum).FindAll(x => !x.IsClosed && x.PlanNum==0);
			}
			data.ListAssociatedPatients=Patients.GetAssociatedPatients(patCur.PatNum);
			data.ListProcsForSplits=Procedures.GetManyProc(data.ListSplits.Select(x => x.ProcNum).ToList(),false);
			data.ConstructChargesData=GetConstructChargesData(listFamilyPatNums,patCur.PatNum,data.ListSplits,paymentCur.PayNum,isIncomeTxfr);
			return data;
		}

		///<summary>Performs explicit linking, imiplicit linking, and auto split logic depending on the parameters provided.</summary>
		public static InitData Init(LoadData loadData,List<AccountEntry> listPayFirstAcctEntries=null,Dictionary<long,Patient> dictPatients=null,
			bool isIncomeTxfr=false,bool isPatPrefer=false,bool doAutoSplit=true,bool doIncludeExplicitCreditsOnly=false)
		{
			//No remoting role check; no call to db
			InitData initData=new InitData();
			//get patients who have this patient's guarantor as their payplan's guarantor
			List<Patient> listPatients=loadData.ListAssociatedPatients;
			listPatients.AddRange(loadData.Fam.ListPats);
			if(loadData.SuperFam.ListPats!=null) {
				listPatients.AddRange(loadData.SuperFam.ListPats);
			}
			List<long> listPatNums=listPatients.Select(x => x.PatNum).ToList();
			//Add patients with paysplits on this payment
			List<long> listUnknownPatNums=loadData.ListSplits.Select(x => x.PatNum)
				.Where(x => !listPatNums.Contains(x))
				.Distinct()
				.ToList();
			//Add patients that are guarantors of payment plan charges for this patient just in case they are outside of the family.
			listUnknownPatNums.AddRange(loadData.ConstructChargesData.ListPayPlanCharges.Select(x => x.Guarantor)
				.Where(x => !listPatNums.Contains(x))
				.Distinct()
				.ToList());
			listPatients.AddRange(Patients.GetLimForPats(listUnknownPatNums.Distinct().ToList()));
			if(dictPatients==null) {
				initData.DictPats=listPatients.GroupBy(x => x.PatNum).ToDictionary(x => x.Key,x => x.First());
			}
			else {
				//Preserve any patients already present in the dictionary.
				initData.DictPats=dictPatients;
				//But overwrite or add to the dictionary for any patients that it might not already know about.
				foreach(Patient patient in listPatients) {
					initData.DictPats[patient.PatNum]=patient;
				}
			}
			initData.SplitTotal=(decimal)loadData.ListSplits.Sum(x => x.SplitAmt);
			loadData.PaymentCur.PayAmt=Math.Round(loadData.PaymentCur.PayAmt-(double)initData.SplitTotal,3);
			if(listPayFirstAcctEntries==null) {
				listPayFirstAcctEntries=new List<AccountEntry>();
			}
			initData.AutoSplitData=AutoSplitForPayment(loadData.PatCur.PatNum,
				listPatNums,
				loadData.ListSplits,
				loadData.PaymentCur,
				listPayFirstAcctEntries,
				isIncomeTxfr,
				isPatPrefer,
				constructChargesData: loadData.ConstructChargesData,
				doAutoSplit: doAutoSplit,
				doIncludeExplicitCreditsOnly: doIncludeExplicitCreditsOnly);
			loadData.ListSplits.AddRange(initData.AutoSplitData.ListPaySplitsSuggested);
			initData.AutoSplitData.ListPaySplitsForPayment=loadData.ListSplits.Union(initData.AutoSplitData.ListPaySplitsSuggested).ToList();
			return initData;
		}
		#endregion

		#region constructing and linking charges and credits
		public static ConstructChargesData GetConstructChargesData(long patNum,List<long> listPatNums=null,bool isIncomeTransfer=false) {
			//No remoting role check; no call to db
			if(listPatNums==null) {
				Family family=Patients.GetFamily(patNum);
				listPatNums=family.GetPatNums();
			}
			//Get all of the account data from the database so that we can invoke auto split logic with all of the required information.
			return GetConstructChargesData(listPatNums,patNum,new List<PaySplit>(),0,isIncomeTransfer);
		}

		///<summary>Gets the data needed to construct a list of charges on FormPayment.</summary>
		public static ConstructChargesData GetConstructChargesData(List<long> listPatNums,long patNum,List<PaySplit> listPaySplitsForPayment,long payNum,
			bool isIncomeTransfer,bool doIncludeTreatmentPlanned=false)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ConstructChargesData>(MethodBase.GetCurrentMethod(),listPatNums,patNum,listPaySplitsForPayment,payNum,isIncomeTransfer,doIncludeTreatmentPlanned);
			}
			ConstructChargesData data=new ConstructChargesData();
			data.ListPaySplits=PaySplits.GetForPats(listPatNums);//Might contain payplan payments.
			data.ListProcs=Procedures.GetCompleteForPats(listPatNums);//will also contain TP procs if pref is set to ON
			if((PrefC.GetYN(PrefName.PrePayAllowedForTpProcs) && !isIncomeTransfer) || doIncludeTreatmentPlanned) {
				data.ListProcs.AddRange(Procedures.GetTpForPats(listPatNums));
			}
			if(isIncomeTransfer) {
				//do not show pre-payment splits that are attached to TP procedures, they should not be transferred since they are already reserved money
				//only TP unearned should have an unearned type and a procNum. 
				data.ListPaySplits.RemoveAll(x => x.UnearnedType!=0 && x.ProcNum!=0 && !data.ListProcs.Select(y => y.ProcNum).Contains(x.ProcNum));
			}
			else {
				//sum ins pay as totals by claims to include the value for the ones that have already been transferred and taken care of. 
				data.ListInsPayAsTotal=ClaimProcs.GetOutstandingClaimPayByTotal(listPatNums);
			}
			data.ListPayPlans=PayPlans.GetForPats(listPatNums,patNum);//Used to figure out how much we need to pay off procs with, also contains ins payplans
			List<long> listSplitNums=data.ListPaySplits.Select(x => x.SplitNum).ToList();
			if(data.ListPayPlans.Count>0) {
				List<long> listPayPlanNums=data.ListPayPlans.Select(x => x.PayPlanNum).ToList();
				//get list where payplan guar is not in the fam)
				data.ListPayPlanSplits=PaySplits.GetForPayPlans(listPayPlanNums);
				data.ListPayPlanCharges=PayPlanCharges.GetForPayPlans(listPayPlanNums,listPatNums);
				data.ListPayPlanLinks=PayPlanLinks.GetForPayPlans(listPayPlanNums);
				List<long> listPayPlanSplitNums=data.ListPayPlanSplits.Select(x => x.SplitNum).ToList();
				List<long> listMissingSplitNums=listPayPlanSplitNums.Except(listSplitNums).ToList();
				data.ListPaySplits.AddRange(data.ListPayPlanSplits.FindAll(x => listMissingSplitNums.Contains(x.SplitNum)));
				listSplitNums.AddRange(listMissingSplitNums);
			}
			if(payNum > 0) {
				//Look for splits that are in the database yet have been deleted from the pay splits grid.
				//If we have a split that's not found in the passed-in list of splits for the payment
				//and the split we got from the DB is on this payment, remove it because the user must have deleted the split from the payment window.
				//The payment window won't update the DB with the change until it's closed.
				List<long> listSplitNumsForPaymentShowing=listPaySplitsForPayment.FindAll(x => x.PayNum==payNum && x.SplitNum > 0).Select(x => x.SplitNum).ToList();
				List<long> listSplitNumsForPaymentDatabase=data.ListPaySplits.FindAll(x => x.PayNum==payNum && x.SplitNum > 0).Select(x => x.SplitNum).ToList();
				List<long> listSplitNumsDeleted=listSplitNumsForPaymentDatabase.Except(listSplitNumsForPaymentShowing).ToList();
				data.ListPaySplits.RemoveAll(x => listSplitNumsDeleted.Contains(x.SplitNum));
			}
			//Get all adjustments for the family members
			data.ListAdjustments=Adjustments.GetAdjustForPats(listPatNums);
			data.ListClaimProcs=ClaimProcs.Refresh(listPatNums);
			//Get all claimprocs that are not PayAsTotals (which are present in ListInsPayAsTotal when it matters).
			data.ListClaimProcsFiltered=data.ListClaimProcs.FindAll(x => x.ProcNum!=0);
			//In the rare case that a Payment Plan has a Guarantor outside of the Patient's family, we want to make sure to collect more
			//data for Adjustments and ClaimProcs so that debits and credits balance properly.
			if(data.ListPayPlans.Any(x => x.Guarantor!=x.PatNum && !listPatNums.Contains(x.PatNum))) {
				//add potentially missing procedures attached to payplans with patient's outside the family
				data.ListProcs.AddRange(
					Procedures.GetManyProc(data.ListPayPlanLinks
						.Where(x => x.LinkType == PayPlanLinkType.Procedure)
						.Select(x => x.FKey).ToList(),
					includeNote: false)
				);
				data.ListProcs=data.ListProcs.DistinctBy(x => x.ProcNum).ToList(); // remove potential duplicates from the list
				//add missing adjustments for newly added procs and attached to payplans for patients outside the family
				data.ListAdjustments.AddRange(Adjustments.GetForProcs(data.ListProcs.Select(x => x.ProcNum).ToList()));
				data.ListAdjustments.AddRange(
					Adjustments.GetMany(data.ListPayPlanLinks
						.Where(x => x.LinkType==PayPlanLinkType.Adjustment)
						.Select(x => x.FKey).ToList())
				);
				data.ListAdjustments=data.ListAdjustments.DistinctBy(x => x.AdjNum).ToList(); //remove potential duplicates from the list
				//finally get all claimprocs for the newly added procedures
				data.ListClaimProcsFiltered.AddRange(
					ClaimProcs.GetForProcs(data.ListProcs
						.FindAll(x => x.ProcNum!=0)
						.Select(x => x.ProcNum).ToList())
				);
				data.ListClaimProcsFiltered=data.ListClaimProcsFiltered.DistinctBy(x => x.ClaimProcNum).ToList(); // remove potenial duplicates
			}
			return data;
		}

		///<summary>Gets the charges and links credits for the patient. Includes family account entries if listPatNums left null.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(long patNum,List<long> listPatNums=null,bool isIncomeTxfr=false,LoadData loadData=null,
			bool doIncludeTreatmentPlanned=false,bool doIncludeExplicitCreditsOnly=false,Payment payment=null,List<AccountEntry> listAccountEntriesPayFirst=null,
			DateTime dateAsOf=default,bool hasInsOverpay=false,bool hasOffsettingAdjustmets=true)
		{
			//No remoting role check; no call to db
			return ConstructAndLinkChargeCredits(listPatNums,
				patNum,
				new List<PaySplit>(),
				payment,
				listAccountEntriesPayFirst,
				isIncomeTxfr:isIncomeTxfr,
				loadData:loadData,
				doIncludeExplicitCreditsOnly:doIncludeExplicitCreditsOnly,
				dateAsOf:dateAsOf,
				doIncludeTreatmentPlanned:doIncludeTreatmentPlanned,
				hasInsOverpay:hasInsOverpay,
				hasOffsettingAdjustmets:hasOffsettingAdjustmets);
		}

		///<summary>Gets the charges and links credits for the patient. Includes family account entries if listPatNums left null.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(List<long> listPatNums,long patNum,List<PaySplit> listPaySplitsForPayment,Payment payment,
			List<AccountEntry> listAccountEntriesPayFirst,bool isIncomeTxfr=false,bool isPreferCurPat=false,LoadData loadData=null,bool doIncludeExplicitCreditsOnly=false,
			bool isAllocateUnearned=false,DateTime dateAsOf=default,bool doIncludeTreatmentPlanned=false,bool hasInsOverpay=false,bool hasOffsettingAdjustmets=true)
		{
			return ConstructAndLinkChargeCredits(patNum,
				listPatNums,
				listPaySplitsForPayment,
				payment?.PayNum??0,
				listAccountEntriesPayFirst,
				isIncomeTxfr,
				isPreferCurPat,
				loadData?.ConstructChargesData,
				doIncludeExplicitCreditsOnly,
				isAllocateUnearned,
				dateAsOf,
				doIncludeTreatmentPlanned,
				payment?.ClinicNum??0,
				payment?.PayAmt??0,
				payment?.PayDate??DateTime.MinValue,
				hasInsOverpay,
				hasOffsettingAdjustmets);
		}

		///<summary>Gets the charges and links credits for the patient. Includes family account entries if listPatNums left null.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(long patNum,List<long> listPatNums,List<PaySplit> listPaySplitsForPayment,long payNum,
			List<AccountEntry> listAccountEntriesPayFirst,bool isIncomeTxfr,bool isPreferCurPat,ConstructChargesData constructChargesData,bool doIncludeExplicitCreditsOnly,
			bool isAllocateUnearned,DateTime dateAsOf,bool doIncludeTreatmentPlanned,long clinicNum,double payAmt,DateTime payDate,bool hasInsOverpay,bool hasOffsettingAdjustmets)
		{
			//No remoting role check; no call to db
			if(listPatNums==null) {
				listPatNums=Patients.GetFamily(patNum).GetPatNums();
			}
			if(listAccountEntriesPayFirst==null) {
				listAccountEntriesPayFirst=new List<AccountEntry>();
			}
			//Get the lists of items we'll be using to calculate with.
			if(constructChargesData==null) {
				constructChargesData=GetConstructChargesData(listPatNums,patNum,listPaySplitsForPayment,payNum,isIncomeTxfr,doIncludeTreatmentPlanned);
			}
			ConstructResults constructResults=GetConstructResults(constructChargesData,patNum,listPatNums,payNum,isIncomeTxfr,clinicNum,payAmt,payDate,dateAsOf);
			ExplicitAndImplicitLinkingForConstructResults(ref constructResults,isIncomeTxfr,listPaySplitsForPayment,constructChargesData,doIncludeExplicitCreditsOnly,isAllocateUnearned,
				listAccountEntriesPayFirst,patNum,isPreferCurPat,payNum,hasInsOverpay,hasOffsettingAdjustmets);
			return constructResults;
		}

		public static ConstructResults GetConstructResults(ConstructChargesData constructChargesData,long patNum,List<long> listPatNums,long payNum,bool isIncomeTxfr,long clinicNum,
			double payAmt,DateTime payDate,DateTime dateAsOf)
		{
			//No remoting role check; no call to db
			ConstructResults constructResults=new ConstructResults(patNum,clinicNum,payAmt,payNum,payDate);
			constructResults.ListAccountEntries=ConstructListCharges(listPatNums,
				constructChargesData.ListProcs,
				constructChargesData.ListAdjustments,
				constructChargesData.ListPaySplits,
				constructChargesData.ListInsPayAsTotal,
				constructChargesData.ListPayPlanCharges,
				constructChargesData.ListPayPlanLinks,
				isIncomeTxfr,
				constructChargesData.ListClaimProcsFiltered,
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum > 0),
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum==0));
			constructResults.ListAccountEntries.Sort(AccountEntrySort);
			if(dateAsOf.Year > 1880) {
				//Remove all account entries that fall after the 'as of date' passed in.
				constructResults.ListAccountEntries.RemoveAll(x => x.Date > dateAsOf);
			}
			return constructResults;
		}

		public static List<AccountEntry> ConstructListCharges(List<long> listPatNums,List<Procedure> listProcs,List<Adjustment> listAdjustments,List<PaySplit> listPaySplits,
			List<PayAsTotal> listInsPayAsTotal,List<PayPlanCharge> listPayPlanCharges,List<PayPlanLink> listPayPlanLinks,bool isIncomeTxfr,
			List<ClaimProc> listClaimProcs,List<PayPlan> listInsPayPlans=null,List<PayPlan> listPayPlans=null)
		{
			//No remoting role check; no call to db
			List<AccountEntry> listCharges=new List<AccountEntry>();
			#region Procedures
			listCharges.AddRange(listProcs.Select(x => new AccountEntry(x)));
			bool includeEstimates=!PrefC.GetBool(PrefName.BalancesDontSubtractIns);
			//Set AmountEnd
			foreach(AccountEntry accountEntryProc in listCharges) {
				Procedure proc=(Procedure)accountEntryProc.Tag;
				accountEntryProc.AmountEnd=GetPatPortion(accountEntryProc,listClaimProcs,includeEstimates);
				if(proc.ProcStatus==ProcStat.TP) {
					accountEntryProc.AmountEnd-=(decimal)proc.DiscountPlanAmt;
					//Apply the value set in the Discount field at the procedure level. A user could discount a discount plan?
					accountEntryProc.AmountEnd-=(decimal)proc.Discount;
				}
			}
			#endregion
			#region Adjustments
			listCharges.AddRange(listAdjustments.Select(x => new AccountEntry(x)));
			#endregion
			#region Payment Plans
			//=================================================Patient and Dynamic Payment Plans===========================================================
			if(!listPayPlanCharges.IsNullOrEmpty() || !listPayPlanLinks.IsNullOrEmpty()) {
				#region Production Outside the Family
				//The guarantor of the family passed in may in fact be the guarantor on a payment plan for a patient outside of the family.
				//The AmountEnd of the production that any credits are pointing to need to be directly manipulated to match the value attached to the plan(s).
				//This is necessary because the entire value of said production does not need to be associated to the payment plan (partial credits).
				//However, the production AccountEntry object itself should NOT include any amount remaining after payment plans have been considered.
				//Procedures=================================================================================================================================
				List<AccountEntry> listAccountEntryProcsOutsideFam=listCharges.FindAll(x => x.ProcNum > 0
					&& !listPatNums.Contains(x.PatNum)
					&& x.GetType()==typeof(Procedure));
				//Only manipulate procedures from outside of the family that are associated to a patient or dynamic payment plan.
				List<long> listPatientPayPlanProcNums=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Credit && x.ProcNum > 0).Select(x => x.ProcNum).ToList();
				List<long> listDynamicPayPlanProcNums=listPayPlanLinks.FindAll(x => x.LinkType==PayPlanLinkType.Procedure).Select(x => x.FKey).ToList();
				List<AccountEntry> listAccountEntryPayPlanProcsOutsideFam=listAccountEntryProcsOutsideFam
					.FindAll(x => listPatientPayPlanProcNums.Contains(x.ProcNum) || listDynamicPayPlanProcNums.Contains(x.ProcNum));
				//Set the AmountEnd for every single entry based on how much value is associated to payment plans.
				for(int i=0;i<listAccountEntryPayPlanProcsOutsideFam.Count;i++) {
					//Get the sum of patient payment plans credits for the procedure.
					decimal totalCredit=0;
					//Figure out if the procedure is attached to a dynamic pay plan or not.
					//Procedures are not allowed to be attached to more than one payment plan type at the same time.
					PayPlanLink payPlanLinkForProc=listPayPlanLinks.FirstOrDefault(x => x.LinkType==PayPlanLinkType.Procedure && x.FKey==listAccountEntryPayPlanProcsOutsideFam[i].ProcNum);
					if(payPlanLinkForProc==null) {//Patient Payment Plan
						totalCredit=listPayPlanCharges
							.Where(x => x.ChargeType==PayPlanChargeType.Credit && x.ProcNum==listAccountEntryPayPlanProcsOutsideFam[i].ProcNum)
							.Sum(x => (decimal)x.Principal);
					}
					else {//Dynamic Payment Plan
						//The DPP on the account contains the entire value or the remaining value of the procedure when AmountOverride is 0.
						//Completely leave the AmountEnd alone for procedures that fall into this scenario since their entire value needs to be considered.
						if(payPlanLinkForProc.AmountOverride==0) {
							totalCredit=listAccountEntryPayPlanProcsOutsideFam[i].AmountEnd;
						}
						else {//Use AmountOverride value if one was set.
							totalCredit=(decimal)payPlanLinkForProc.AmountOverride;
						}
					}
					//Do not inflate AmountEnd for the production based off of how much is associated to payment plans (overrides could be way more than production worth).
					listAccountEntryPayPlanProcsOutsideFam[i].AmountEnd=Math.Min(listAccountEntryPayPlanProcsOutsideFam[i].AmountEnd,totalCredit);
				}
				//Adjustments================================================================================================================================
				//There is no such thing as making an adjustment credit for patient payment plans so only execute the following code for DPPs.
				List<PayPlanLink> listAdjPayPlanLinks=listPayPlanLinks.FindAll(x => x.LinkType==PayPlanLinkType.Adjustment);
				//Get all of the adjustments that are attached to a DPP but are associated to a patient outside of the current family.
				List<AccountEntry> listAccountEntryAdjsOutsideFam=listCharges.FindAll(x => x.AdjNum > 0
					&& !listPatNums.Contains(x.PatNum)
					&& x.GetType()==typeof(Adjustment)
					&& listAdjPayPlanLinks.Select(y => y.FKey).Contains(x.AdjNum));
				//Set the AmountEnd for every single entry based on how much value is associated to payment plans.
				for(int i=0;i<listAccountEntryAdjsOutsideFam.Count;i++) {
					decimal totalCredit=0;
					PayPlanLink payPlanLinkAdj=listAdjPayPlanLinks.First(x => x.FKey==listAccountEntryAdjsOutsideFam[i].AdjNum);
					//Check to see if there are any DPPs that do NOT have an override value set (consumes entire adjustment value).
					if(payPlanLinkAdj.AmountOverride==0) {
						//Completely leave the AmountEnd for adjustments that fall into this scenario since their entire value needs to be considered.
						totalCredit=listAccountEntryAdjsOutsideFam[i].AmountEnd;
					}
					else {//Use AmountOverride value if one was set.
						totalCredit=(decimal)payPlanLinkAdj.AmountOverride;
					}
					//Do not inflate AmountEnd for the production based off of how much is associated to payment plans (overrides could be way more than production worth).
					listAccountEntryAdjsOutsideFam[i].AmountEnd=Math.Min(listAccountEntryAdjsOutsideFam[i].AmountEnd,totalCredit);
				}
				#endregion
				List<long> listInsPayPlanNums=new List<long>();
				if(!listInsPayPlans.IsNullOrEmpty()) {
					listInsPayPlanNums=listInsPayPlans.Select(x => x.PayPlanNum).ToList();
				}
				if(listPayPlans==null) {
					long[] payPlanNumArray=listPayPlanCharges.Select(x=>x.PayPlanNum).ToArray();
					listPayPlans=PayPlans.GetMany(payPlanNumArray).FindAll(x=>x.PlanNum==0);
				}
				listCharges.AddRange(
					GetFauxEntriesForPayPlans(listPayPlanCharges.FindAll(x => !listInsPayPlanNums.Contains(x.PayPlanNum)),listPayPlanLinks,listCharges,listPayPlans)
				);
			}
			//======================================================Insurance Payment Plans================================================================
			//Only consider active plans because closed plans will have claimprocs that should have already been taken into account above.
			if(!listInsPayPlans.IsNullOrEmpty() && listInsPayPlans.Any(x => !x.IsClosed)) {
				//Ignore all insurance payment plans that do not have any claimprocs associated (no received payments thus no known ins estimates).
				//It is in our online manual to create the insurance payment plan first prior to receiving the insurance payment.
				List<PayPlan> listFilteredInsPayPlans=listInsPayPlans.FindAll(x => !x.IsClosed 
					&& listClaimProcs.Any(y => y.PayPlanNum==x.PayPlanNum));
				//Get all of the payment plan charge debits for the insurance payment plans. Credits are completely ignored.
				Dictionary<long,List<PayPlanCharge>> dictPayPlanNumDebits=PayPlanCharges.GetChargesForPayPlanChargeType(
						listFilteredInsPayPlans.Select(x => x.PayPlanNum).ToList(),PayPlanChargeType.Debit)
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				foreach(PayPlan payPlan in listFilteredInsPayPlans) {
					//Skip any plans that do not have any debits in the database which is the only way we know how much the plan is worth.
					if(!dictPayPlanNumDebits.ContainsKey(payPlan.PayPlanNum)) {
						continue;
					}
					//PayPlanCharge credits are typically used to figure out the total value of a payment plan. Insurance payment plans are different.
					//There is only one payment plan charge credit for each insurance payment plan and it simply matches the Completed Tx Amount field.
					//This isn't helpful because the user can add random debits to the plan whenever they like and for however much they desire.
					//Then to top it off, they are lightly slapped on the wrist with a warning message suggesting that the total amt and tx amt should match.
					//When the popup is inevitably ignored, the payment plan will display in the Account module as if the plan is worth the total of debits.
					//Therefore, this section of code will also tally up all debits attached to the plan in order to know the total value of the plan.
					double amtToDistribute=dictPayPlanNumDebits[payPlan.PayPlanNum].Sum(x => x.Principal);
					//Move onto the next plan if there is no value to distribute.
					if(amtToDistribute<=0) {
						continue;
					}
					List<ClaimProc> listClaimProcsForPlan=listClaimProcs.FindAll(x => x.PayPlanNum==payPlan.PayPlanNum && x.InsPayEst>=0);
					//There is some value on the plan that should be applied to procedures that are vicariously associated to this plan FIFO style.
					List<AccountEntry> listProcEntries=listCharges.FindAll(x => x.GetType()==typeof(Procedure)
						&& x.AmountEnd > 0
						&& listClaimProcsForPlan.Any(y => y.ProcNum==x.ProcNum));
					foreach(AccountEntry procEntry in listProcEntries) {
						if(amtToDistribute<=0) {
							break;
						}
						List<ClaimProc> listProcClaimProcs=listClaimProcsForPlan.FindAll(x => x.ProcNum==procEntry.ProcNum);
						//There should be at least one received claimproc associated to the procedure based on the official steps that we provide in the manual.
						//This claimproc is where we will get the official insurance estimate for the procedure from.
						//Only distribute up to the insurance estimate and do not apply so much value as to make the account entry exceed the original amount.
						ClaimProc claimProcEst=listProcClaimProcs.FirstOrDefault(x => x.Status==ClaimProcStatus.Received);
						if(claimProcEst==null) {
							continue;//We don't know what the official estimate for this procedure is. Move to the next procedure if available.
						}
						//Start with the insurance estimate as the amount of value that can be distributed to this procedure.
						double amtProcCanTake=claimProcEst.InsPayEst;
						//Subtract all insurance payments attached to this insurance payment plan that have already been paid.
						amtProcCanTake-=listProcClaimProcs.Sum(x => x.InsPayAmt);
						//Never distribute more value to a procedure than what it needs.
						amtProcCanTake=Math.Min((double)procEntry.AmountEnd,amtProcCanTake);
						if(amtProcCanTake<=0) {
							continue;//The insurance payment plan has already paid the estimated value for this procedure. Move to the next procedure.
						}
						double amtToRemove=Math.Min(amtProcCanTake,amtToDistribute);
						procEntry.AmountEnd-=(decimal)amtToRemove;
						amtToDistribute-=amtToRemove;
					}
				}
			}
			#endregion
			#region Unearned
			if(isIncomeTxfr) {
				List<long> listHiddenUnearnedTypes=PaySplits.GetHiddenUnearnedDefNums();
				for(int i=listPaySplits.Count-1;i>=0;i--) {
					//Hidden unearned splits that are attached to procedures or payment plans are not transferrable.
					if(listHiddenUnearnedTypes.Contains(listPaySplits[i].UnearnedType) && (listPaySplits[i].ProcNum > 0 || listPaySplits[i].PayPlanNum > 0)) {
						continue;
					}
					//Add all other payment splits when performing income transfers since previously transferred splits will balance out (negative and positive offsetting amounts).
					listCharges.Add(new AccountEntry(listPaySplits[i]));
				}
				foreach(PayAsTotal totalPmt in listInsPayAsTotal) {//Ins pay totals need to be added to the sum total for income transfers
					listCharges.Add(new AccountEntry(totalPmt));
				}
			}
			#endregion
			return listCharges;
		}

		///<summary>Manipulates the AmountEnd of the account entries within the ConstructResults passed in based off of how money in the database links to production. 
		///The ListPaySplits field on the ConstructResults passed in can be manipulated when account entries to be 'paid first' are passed in.</summary>
		public static void ExplicitAndImplicitLinkingForConstructResults(ref ConstructResults constructResults,bool isIncomeTxfr,List<PaySplit> listPaySplitsForPayment,
			ConstructChargesData constructChargesData,bool doIncludeExplicitCreditsOnly,bool isAllocateUnearned,List<AccountEntry> listAccountEntriesPayFirst,long patNum,
			bool isPreferCurPat,long payNum,bool hasInsOverpay,bool hasOffsettingAdjustmets)
		{
			#region Explicit Linking
			//When executing an income transfer from within the payment window listSplitsCur can be filled with new splits.
			//These splits need to be present in the list of outstanding charges so that their value can be considered within the pat/prov/clinic bucket.
			//This is because the Outstanding Charges grid is forced to be grouped by pat/prov/clinic when in transfer mode.
			//The AmountEnd column for the grouping could show incorrectly without these Account Entries after a transfer has been made.
			if(isIncomeTxfr) {
				constructResults.ListAccountEntries.AddRange(listPaySplitsForPayment.Where(x => x.SplitNum==0).Select(x => new AccountEntry(x)));
			}
			//Make deep copies of the current splits that are attached to the payment because the SplitAmt field will be manipulated below.
			List<PaySplit> listSplitsCurrent=listPaySplitsForPayment.Where(x => x.SplitNum==0).Select(y => y.Copy()).ToList();
			List<PaySplit> listSplitsHistoric=constructChargesData.ListPaySplits.Select(x => x.Copy()).ToList();
			List<PaySplit> listSplitsCurrentAndHistoric=listSplitsCurrent;
			listSplitsCurrentAndHistoric.AddRange(listSplitsHistoric);
			//This ordering is necessary so parents come before their children when explicitly linking credits.
			listSplitsCurrentAndHistoric=listSplitsCurrentAndHistoric.OrderBy(x => x.SplitNum > 0)
				.ThenBy(x => x.DatePay)
				.ToList();
			constructResults.ListAccountEntries=ExplicitlyLinkCredits(constructResults.ListAccountEntries,
				listSplitsCurrentAndHistoric,hasInsOverpay:hasInsOverpay,hasOffsettingAdjustmets:hasOffsettingAdjustmets);
			#endregion
			//If this payment is an income transfer, do NOT use unallocated income to pay off charges.
			//However, allow partial implicit linking when running AllocateUnearned logic.
			if((!isIncomeTxfr && !doIncludeExplicitCreditsOnly)
				|| (isIncomeTxfr && isAllocateUnearned))
			{
				#region Pay Selected Entries First
				if(!listAccountEntriesPayFirst.IsNullOrEmpty()) {
					//Find all account entries that are related to the selected entries.
					List<AccountEntry> listAccountEntriesRelated=new List<AccountEntry>();
					//Related PayPlanCharges==========================================================================================================
					List<long> listPayPlanNums=listAccountEntriesPayFirst.Where(x => x.PayPlanNum > 0).Select(x => x.PayPlanNum).ToList();
					listAccountEntriesRelated.AddRange(constructResults.ListAccountEntries.Where(x => x.GetType()==typeof(FauxAccountEntry)
							&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
							&& listPayPlanNums.Contains(x.PayPlanNum))
						.Cast<FauxAccountEntry>()
						.OrderBy(x => CompareDecimal.IsGreaterThanZero(x.Interest)));//Pay interest first.
					//Related Procedures==============================================================================================================
					List<long> listProcNums=listAccountEntriesPayFirst.Where(x => x.GetType()==typeof(Procedure)).Select(x => x.ProcNum).ToList();
					listAccountEntriesRelated.AddRange(constructResults.ListAccountEntries.FindAll(x => x.GetType()==typeof(Procedure)
							&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
							&& listProcNums.Contains(x.ProcNum)));
					//Related Adjustments=============================================================================================================
					List<long> listAdjNums=listAccountEntriesPayFirst.Where(x => x.GetType()==typeof(Adjustment)).Select(x => x.AdjNum).ToList();
					listAccountEntriesRelated.AddRange(constructResults.ListAccountEntries.FindAll(x => x.GetType()==typeof(Adjustment)
							&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
							&& listAdjNums.Contains(x.AdjNum)));
					listAccountEntriesRelated.Sort(AccountEntrySort);
					//Create a variable to keep track of the money that can be allocated for this payment.
					double amtToAllocate=constructResults.PayAmt;
					//Create as many splits as possible for account entries with positive AmountEnd values.
					for(int i=0;i<listAccountEntriesRelated.Count;i++) {
						if(CompareDouble.IsLessThanOrEqualToZero(amtToAllocate)) {
							break;//No more value to allocate.
						}
						if(CompareDecimal.IsLessThanOrEqualToZero(listAccountEntriesRelated[i].AmountEnd)) {
							continue;
						}
						double splitAmt=Math.Min(amtToAllocate,(double)listAccountEntriesRelated[i].AmountEnd);
						//Make a new split that will apply as much value as possible from the account entry.
						PaySplit split=CreatePaySplitHelper(listAccountEntriesRelated[i],splitAmt,constructResults.PayDate,payNum:constructResults.PayNum,isNew:true);
						//Remove the value from the account entry
						listAccountEntriesRelated[i].AmountEnd-=(decimal)splitAmt;
						amtToAllocate-=splitAmt;
						listAccountEntriesRelated[i].SplitCollection.Add(split);
						constructResults.ListPaySplits.Add(split);
					}
				}
				#endregion
				#region Implicit Linking
				PayResults implicitResult=ImplicitlyLinkCredits(ref listSplitsHistoric,
					constructChargesData.ListInsPayAsTotal,
					constructResults.ListAccountEntries,
					listPaySplitsForPayment,
					listAccountEntriesPayFirst,
					patNum,
					isPreferCurPat,
					isAllocateUnearned);
				constructResults.ListAccountEntries=implicitResult.ListAccountCharges;
				#endregion
			}
			#region Set AmountAvailable
			//Set the AmountAvailable field on each account entry to the sum of all PaySplits that are associated to other payments.
			foreach(AccountEntry accountEntry in constructResults.ListAccountEntries.Where(x => x.PayPlanChargeNum==0)) {
				double amtUsed=accountEntry.SplitCollection.Where(x => x.SplitNum > 0 && x.PayNum!=payNum && x!=accountEntry.Tag)
					.Sum(x => x.SplitAmt);
				accountEntry.AmountAvailable=(accountEntry.AmountOriginal + accountEntry.AdjustedAmt)-(decimal)amtUsed;
				if(accountEntry.Tag!=null && accountEntry.Tag.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
					//Apply the value set in the Discount field at the procedure level only when the procedure is of status TP.
					accountEntry.AmountAvailable-=(decimal)((Procedure)accountEntry.Tag).Discount;
				}
			}
			//Payment plan account entries are handled differently because they can have multiple account entries that represent one payment plan charge.
			Dictionary<long,List<AccountEntry>> dictPayPlanChargeEntries=constructResults.ListAccountEntries.Where(x => x.PayPlanChargeNum > 0)
				.GroupBy(x => x.PayPlanChargeNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(long payPlanChargeNum in dictPayPlanChargeEntries.Keys) {
				List<PaySplit> listPaySplits=new List<PaySplit>();
				//Get all unique PaySplits that are associated to this charge.
				foreach(AccountEntry accountEntry in dictPayPlanChargeEntries[payPlanChargeNum]) {
					foreach(PaySplit paySplit in accountEntry.SplitCollection.Where(x => x.SplitNum > 0 && x.PayNum!=payNum && x!=accountEntry.Tag)) {
						if(!listPaySplits.Any(x => x.SplitNum==paySplit.SplitNum)) {
							listPaySplits.Add(paySplit);
						}
					}
				}
				double amtToAllocate=listPaySplits.Sum(x => x.SplitAmt);
				//Only apply as much as possible to each charge (might be a tiny amount of interest followed by a large principal amount).
				foreach(AccountEntry accountEntry in dictPayPlanChargeEntries[payPlanChargeNum]) {
					double amtUsed=Math.Min(amtToAllocate,(double)accountEntry.AmountOriginal);
					accountEntry.AmountAvailable=accountEntry.AmountOriginal-(decimal)amtUsed;
					amtToAllocate-=amtUsed;
				}
			}
			#endregion
			#region Set Transferrable Income Amount
			//Set the IncomeAmt field on every account entry to the sum of all associated paysplits.
			//This is the maximum value that can be transferred away from negative production entries when we can't treat negative production as income.
			constructResults.ListAccountEntries.ForEach(x => x.IncomeAmt=(decimal)x.AmountPaid);
			#endregion
			#region Combine Unallocated and Unearned
			//The unallocated and unearned buckets are unique in the fact that they do not care about explicit linking in order to be taken from.
			//This means that money can be taken from unallocated or unearned for providerA to pay for an outstanding procedure for providerB.
			//However, there could be a plethora of individual account entries for individual providers which can cause many tiny splits when transferring.
			//All unallocated and unearned buckets should be combined in order to cut back on the number of splits that income transfers suggest.
			if(constructResults.ListAccountEntries.Any(x => x.PayPlanNum==0 && (x.IsUnallocated || x.IsUnearned))) {
				//Identify the unallocated and unearned entries that have value to transfer.
				List<AccountEntry> listUnallocatedUnearnedEntries=constructResults.ListAccountEntries.FindAll(x => (x.IsUnallocated || x.IsUnearned)
					&& x.PayPlanNum==0
					&& !CompareDecimal.IsZero(x.AmountEnd));
				//Remove these entries from the return value. They will be grouped up and put back in later.
				constructResults.ListAccountEntries.RemoveAll(x => (x.IsUnallocated || x.IsUnearned)
					&& x.PayPlanNum==0
					&& !CompareDecimal.IsZero(x.AmountEnd));
				//Separate the unallocated and unearned entries.
				List<AccountEntry> listUnallocatedEntries=listUnallocatedUnearnedEntries.FindAll(x => x.IsUnallocated);
				List<AccountEntry> listUnearnedEntries=listUnallocatedUnearnedEntries.FindAll(x => x.IsUnearned);
				Func<List<AccountEntry>,AccountEntry> funcGetCombinedEntry=(listAccountEntries) => {
					//The list of account entries passed in will all be unallocated or unearned entries for the sam pat/prov/clinic.
					//These entries are safe to combine and treat as one large PaySplit.
					AccountEntry accountEntryFirst=listAccountEntries.First();
					return new AccountEntry() {
						AmountAvailable=listAccountEntries.Sum(x => x.AmountAvailable),
						AmountEnd=listAccountEntries.Sum(x => x.AmountEnd),
						AmountOriginal=listAccountEntries.Sum(x => x.AmountOriginal),
						ClinicNum=accountEntryFirst.ClinicNum,
						Date=listAccountEntries.Min(x => x.Date),
						PatNum=accountEntryFirst.PatNum,
						ProvNum=accountEntryFirst.ProvNum,
						Tag=new PaySplit() { UnearnedType=accountEntryFirst.UnearnedType, },
						UnearnedType=accountEntryFirst.UnearnedType,
					};
				};
				List<AccountEntry> listAccountEntriesCombined=new List<AccountEntry>();
				listUnallocatedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
					.ToDictionary(x => x.Key,x => x.ToList())
					.ForEach(x => listAccountEntriesCombined.Add(funcGetCombinedEntry(x.Value)));
				if(PrefC.IsODHQ) {
					//HQ wants to have each unearned type considered separately.
					//This will allow for payment splits to be suggested that will technically transfer from one unearned type to another unearned type.
					listUnearnedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum,x.UnearnedType })
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listAccountEntriesCombined.Add(funcGetCombinedEntry(x.Value)));
				}
				else {
					//Treat all unearned types the same, as if there is only one big unearned type.
					listUnearnedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => listAccountEntriesCombined.Add(funcGetCombinedEntry(x.Value)));
				}
				constructResults.ListAccountEntries.AddRange(listAccountEntriesCombined);
			}
			#endregion
		}

		///<summary>Helper method that will eventually invoke ClaimProcs.GetPatPortion().
		///This method will completely ignore the list of ClaimProcs passed in if all related to the proc have NotBillIns set.</summary>
		private static decimal GetPatPortion(AccountEntry accountEntryProc,List<ClaimProc> listClaimProcs,bool includeEstimates=true) {
			//No remoting role check; no call to db
			if(accountEntryProc.Tag==null || accountEntryProc.Tag.GetType()!=typeof(Procedure)) {
				return 0;
			}
			Procedure proc=(Procedure)accountEntryProc.Tag;
			List<ClaimProc> listProcClaimProcs=listClaimProcs.FindAll(x => x.ProcNum==proc.ProcNum);
			//There is an extremely rare scenario where a completed procedure can be flagged as "Do Not Bill to Ins" but will still have ins estimates.
			//Act like there are no ClaimProcs for the procedure in question when all ClaimProcs are flagged as NoBillIns.
			if(listProcClaimProcs.All(x => x.NoBillIns)) {
				return (decimal)proc.ProcFeeTotal;
			}
			//One or more ClaimProcs are flagged to be billed to insurance, take their value into consideration.
			accountEntryProc.InsPayAmt=ClaimProcs.GetInsPay(listProcClaimProcs,includeEstimates:includeEstimates);
			return ClaimProcs.GetPatPortion(proc,listProcClaimProcs,includeEstimates:includeEstimates);
		}

		///<summary>Returns a list of FauxAccountEntry objects that represent the current state of affairs based on the information provided.
		///Typical AccountEntry objects represent objects from the database in a 1 to 1 fashion. Payment plans utilize FauxAccountEntries because they are much more complex.
		///A single PayPlanCharge debit could technically cover the remaining value on several different procedures (credits).
		///Therefore, multiple FauxAccountEntry objects can be returned for a singular PayPlanCharge. This is most common when a payment plan has APR set.</summary>
		private static List<FauxAccountEntry> GetFauxEntriesForPayPlans(List<PayPlanCharge> listPayPlanCharges,List<PayPlanLink> listPayPlanLinks,
			List<AccountEntry> listAccountEntries,List<PayPlan> listPayPlans)
		{
			if(listPayPlans==null) {
				listPayPlans=new List<PayPlan>();
			}
			List<FauxAccountEntry> listFauxAccountEntries=new List<FauxAccountEntry>();
			List<PayPlanCharge> listPayPlanChargeCredits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Credit);
			List<PayPlanCharge> listPayPlanChargeDebits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit);
			List<PayPlanProductionEntry> listPayPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			#region Patient Payment Plan Credits
			//Create faux account entries for all credits associated to a payment plan.
			for(int i=0;i<listPayPlanChargeCredits.Count;i++) {
				FauxAccountEntry fauxAccountEntry=new FauxAccountEntry(listPayPlanChargeCredits[i],true);
				//Prefer the patient, provider, and clinic combo from the procedure if present.
				if(listPayPlanChargeCredits[i].ProcNum > 0) {
					AccountEntry accountEntryProc=listAccountEntries.FirstOrDefault(x => x.ProcNum > 0 && x.ProcNum==listPayPlanChargeCredits[i].ProcNum);
					if(accountEntryProc!=null) {
						//Payment plans need to know exactly how much this procedure is worth. This includes any explicitly linked adjustments and insurance.
						ExplicitlyLinkPositiveAdjustmentsToProcedure(accountEntryProc,listAccountEntries);
						//This faux payment plan account entry should always honor the pat / prov / clinic of the procedure and not for the payment plan.
						fauxAccountEntry.AccountEntryProc=accountEntryProc;
						fauxAccountEntry.PatNum=accountEntryProc.PatNum;
						fauxAccountEntry.ProvNum=accountEntryProc.ProvNum;
						fauxAccountEntry.ClinicNum=accountEntryProc.ClinicNum;
					}
				}
				listFauxAccountEntries.Add(fauxAccountEntry);
			}
			#endregion
			#region Dynamic Payment Plan Credits
			//Create faux account entries for PayPlanProductionEntry procedures and adjustments (dynamic payment plan credits).
			List<PayPlanProductionEntry> listPayPlanProductionEntriesCredits=listPayPlanProductionEntries.FindAll(x => x.LinkType.In(PayPlanLinkType.Procedure,PayPlanLinkType.Adjustment));
			for(int i=0;i<listPayPlanProductionEntriesCredits.Count;i++) {
				FauxAccountEntry fauxAccountEntryCredit=new FauxAccountEntry(listPayPlanProductionEntriesCredits[i]);
				if(fauxAccountEntryCredit.IsAdjustment) {
					AccountEntry accountEntryAdj=listAccountEntries.FirstOrDefault(x => x.GetType()==typeof(Adjustment) && x.AdjNum > 0 && x.AdjNum==fauxAccountEntryCredit.AdjNum);
					if(accountEntryAdj==null) {
						continue;
					}
					//Dynamic payment plans don't create multiple PayPlanCharge entries for adjustments (like the old payment plan system does).
					//There will be a single PayPlanLink entry that is attached to a specific adjustment.
					//The strange part is that the PayPlanCharge entries that are created for this dynamic payment plan have already factored in adjustments.
					//So take value away from the adjustment if AmountEnd is positive because a payplan might only cover part of an adjustment.
					if(CompareDecimal.IsGreaterThanZero(fauxAccountEntryCredit.AmountEnd)) {
						accountEntryAdj.AmountEnd-=Math.Min(accountEntryAdj.AmountEnd,fauxAccountEntryCredit.AmountEnd);
					}
					else if(CompareDecimal.IsLessThanZero(fauxAccountEntryCredit.AmountEnd)) {
						accountEntryAdj.AmountEnd-=Math.Max(accountEntryAdj.AmountEnd,fauxAccountEntryCredit.AmountEnd);
					}
				}
				else {//Procedure
					AccountEntry accountEntryProc=listAccountEntries.FirstOrDefault(x => x.ProcNum > 0 && x.ProcNum==fauxAccountEntryCredit.ProcNum);
					if(accountEntryProc==null) {
						continue;//Do NOT add this FauxAccountEntry to the list of payment plan account entries because the associated proc was not found.
					}
					ExplicitlyLinkPositiveAdjustmentsToProcedure(accountEntryProc,listAccountEntries);
					fauxAccountEntryCredit.AccountEntryProc=accountEntryProc;
					//Take as much value away from the procedure as possible if AmountEnd is positive because a payplan might only cover part of a procedure.
					//Only do this for positive credits because negative procedure credits should not give value back to the procedure.
					if(CompareDecimal.IsGreaterThanZero(fauxAccountEntryCredit.AmountEnd)) {
						accountEntryProc.AmountEnd-=Math.Min(accountEntryProc.AmountEnd,fauxAccountEntryCredit.AmountEnd);
					}
				}
				listFauxAccountEntries.Add(fauxAccountEntryCredit);
			}
			#endregion
			#region All Payment Plan Debits
			for(int i=0;i<listPayPlanChargeDebits.Count;i++) {
				if(!CompareDouble.IsZero(listPayPlanChargeDebits[i].Principal)) {
					listFauxAccountEntries.Add(new FauxAccountEntry(listPayPlanChargeDebits[i],true));
				}
				if(!CompareDouble.IsZero(listPayPlanChargeDebits[i].Interest)) {
					listFauxAccountEntries.Add(new FauxAccountEntry(listPayPlanChargeDebits[i],false));
				}
			}
			#endregion
			#region Payment Plan Offsetting Debits
			//Dynamic payment plans can get into this rare scenario where a debit charge has been inserted into the database for too much value.
			//There is a 'fix' that users can apply from within the dynamic payment plan overcharge report which will create offsetting negative debits.
			//These offsetting charges need to remove value from corresponding debits that are linked to the same production entry (proc, adj, etc).
			//The following code is explicitly written to work for all payment plan types just in case we need to introduce this paradigm to others.
			List<FauxAccountEntry> listFauxAccountEntriesOffsetDebits=listFauxAccountEntries.FindAll(x => x.IsOffset);
			for(int i=0;i<listFauxAccountEntriesOffsetDebits.Count;i++) {
				List<FauxAccountEntry> listFauxAccountEntriesRelatedDebits=listFauxAccountEntries.FindAll(x => !x.IsOffset
					&& x.IsAdjustment==listFauxAccountEntriesOffsetDebits[i].IsAdjustment
					&& x.ChargeType==listFauxAccountEntriesOffsetDebits[i].ChargeType
					&& x.ProcNum==listFauxAccountEntriesOffsetDebits[i].ProcNum
					&& x.AdjNum==listFauxAccountEntriesOffsetDebits[i].AdjNum
					&& x.PayPlanNum==listFauxAccountEntriesOffsetDebits[i].PayPlanNum);
				for(int j=0;j<listFauxAccountEntriesRelatedDebits.Count;j++) {
					if(listFauxAccountEntriesOffsetDebits[i].AmountEnd>=0) {
						break;
					}
					if(listFauxAccountEntriesRelatedDebits[j].AmountEnd<=0) {
						continue;
					}
					//The offsetting debit still has a negative value that needs to be removed from related debits.
					decimal amountRemove=Math.Min(Math.Abs(listFauxAccountEntriesOffsetDebits[i].AmountEnd),Math.Abs(listFauxAccountEntriesRelatedDebits[j].AmountEnd));
					listFauxAccountEntriesOffsetDebits[i].AmountEnd+=amountRemove;
					listFauxAccountEntriesRelatedDebits[j].AmountEnd-=amountRemove;
				}
			}
			#endregion
			//All payment plan charge credits and debits passed in have been turned into FauxAccountEntry objects at this point.
			//Some entries have been massaged but for the most part these entries represent the database.
			//E.g. Dynamic payment plans have already taken value from production at this point.
			//Calling methods need to have these faux entries broken down even further so that they have more accurate entries to allocate income to.
			//E.g. A single PayPlanCharge with principal and interest should be broken up into two FauxAccountEntry objects to make sure we do not overpay the production.
			//This will also allow us to allocate income correctly to the production that is attached to payment plans.
			//E.g. A payment plan can have production from multiple providers associated with it and we need to pay those providers and not simply pay the provider on the plan.
			List<FauxAccountEntry> listFauxAccountEntriesAllocateds=new List<FauxAccountEntry>();
			#region Payment Plan Debit Allocation
			List<PayPlanNumFauxAccountEntriesGroup> listPayPlanNumFauxAccountEntriesGroups=listFauxAccountEntries
				.GroupBy(x => x.PayPlanNum)
				.ToDictionary(x => x.Key,x => x.ToList())
				.Select(x => new PayPlanNumFauxAccountEntriesGroup() { PayPlanNum=x.Key,ListFauxAccountEntries=x.Value })
				.ToList();
			for(int i=0;i<listPayPlanNumFauxAccountEntriesGroups.Count;i++) {
				PayPlan payPlan=listPayPlans.First(x => x.PayPlanNum==listPayPlanNumFauxAccountEntriesGroups[i].PayPlanNum);
				List<FauxAccountEntry> listFauxAccountEntriesCredits=listPayPlanNumFauxAccountEntriesGroups[i].ListFauxAccountEntries.FindAll(x => x.ChargeType==PayPlanChargeType.Credit);
				List<FauxAccountEntry> listFauxAccountEntriesDebits=listPayPlanNumFauxAccountEntriesGroups[i].ListFauxAccountEntries.FindAll(x => x.ChargeType==PayPlanChargeType.Debit);
				if(payPlan.IsDynamic) {
					listFauxAccountEntriesAllocateds.AddRange(AllocateDynamicPayPlanDebitsToCredits(listFauxAccountEntriesCredits,listFauxAccountEntriesDebits,payPlan.DynamicPayPlanTPOption));
				}
				else {
					listFauxAccountEntriesAllocateds.AddRange(AllocatePatientPayPlanDebitsToCredits(listFauxAccountEntriesCredits,listFauxAccountEntriesDebits));
				}
			}
			#endregion
			//Remove all value from debits due in the future so that calling entities don't think these charges are due right now.
			List<FauxAccountEntry> listFauxAccountEntriesFutureAllocateds=listFauxAccountEntriesAllocateds.FindAll(x => x.Date > DateTime.Today);
			for(int i=0;i<listFauxAccountEntriesFutureAllocateds.Count;i++) {
				listFauxAccountEntriesFutureAllocateds[i].AmountEnd=0;
			}
			return listFauxAccountEntriesAllocateds;
		}

		private static void ExplicitlyLinkPositiveAdjustmentsToProcedure(AccountEntry accountEntryProcedure,List<AccountEntry> listAccountEntries) {
			if(accountEntryProcedure==null || accountEntryProcedure.GetType()!=typeof(Procedure) || listAccountEntries==null || listAccountEntries.Count==0) {
				return;
			}
			//Blindly move all positive adjustment value into explicitly linked procedures.
			List<AccountEntry> listAccountEntriesExplicitAdjustments=listAccountEntries.FindAll(x => x.GetType()==typeof(Adjustment)
				&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
				&& x.ProcNum==accountEntryProcedure.ProcNum
				&& x.PatNum==accountEntryProcedure.PatNum
				&& x.ProvNum==accountEntryProcedure.ProvNum
				&& x.ClinicNum==accountEntryProcedure.ClinicNum);
			for(int i=0;i<listAccountEntriesExplicitAdjustments.Count;i++) {
				accountEntryProcedure.AmountEnd+=listAccountEntriesExplicitAdjustments[i].AmountEnd;
				listAccountEntriesExplicitAdjustments[i].AmountEnd=0;
			}
		}

		private static List<FauxAccountEntry> AllocateDynamicPayPlanDebitsToCredits(List<FauxAccountEntry> listFauxAccountEntriesCredits,List<FauxAccountEntry> listFauxAccountEntriesDebits,DynamicPayPlanTPOptions dynamicPayPlanTPOption) {
			List<FauxAccountEntry> listFauxAccountEntriesAllocatedDebits=new List<FauxAccountEntry>();
			//Remove treatment planned procedure credits if they are not treated as complete.
			if(dynamicPayPlanTPOption!=DynamicPayPlanTPOptions.TreatAsComplete) {
				listFauxAccountEntriesCredits.RemoveAll(x => x.AccountEntryProc!=null && ((Procedure)x.AccountEntryProc.Tag).ProcStatus==ProcStat.TP);
			}
			//Dynamic payment plans utilize the PayPlanCharge FKey column to directly link debits to production.
			//There is a report that breaks down 'overpaid payment plans' to a procedure level to help users know exactly which procedures are wrong.
			//Therefore, loop through each faux credit entry one at a time and apply any matching due debits (via FKey) first. Starting with adjustments.
			#region Adjustments
			//Loop through each faux credit adjustment and apply as many adjustment debits as possible.
			List<FauxAccountEntry> listFauxAccountEntriesAdjCredits=listFauxAccountEntriesCredits.FindAll(x => x.IsAdjustment);
			//It is safe to use AmountEnd because Principal was the only thing used to populate it when this faux entry was created (no interest).
			List<FauxAccountEntry> listFauxAccountEntriesPosAdjDebits=listFauxAccountEntriesDebits.FindAll(x => x.IsAdjustment && CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			for(int i=0;i<listFauxAccountEntriesAdjCredits.Count;i++) {
				List<FauxAccountEntry> listFauxAccountEntriesAdjDebits=listFauxAccountEntriesPosAdjDebits.FindAll(x => x.AdjNum==listFauxAccountEntriesAdjCredits[i].AdjNum);
				for(int j=0;j<listFauxAccountEntriesAdjDebits.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesAdjCredits[i].AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesAdjDebits[j].AmountEnd)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(listFauxAccountEntriesAdjCredits[i].AmountEnd,listFauxAccountEntriesAdjDebits[j].AmountEnd);
					listFauxAccountEntriesAdjDebits[j].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesAdjCredits[i].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesAdjCredits[i].PrincipalAdjusted-=amountToAllocate;
					FauxAccountEntry fauxAccountEntryAllocatedDebit=GetAllocatedDebit(amountToAllocate,listFauxAccountEntriesAdjCredits[i],listFauxAccountEntriesAdjDebits[j]);
					listFauxAccountEntriesAllocatedDebits.Add(fauxAccountEntryAllocatedDebit);
				}
			}
			#endregion
			#region Procedures
			//Loop through each procedure credit and apply as many debits as possible.
			List<FauxAccountEntry> listFauxAccountEntriesProcCredits=listFauxAccountEntriesCredits.FindAll(x => !x.IsAdjustment);
			List<FauxAccountEntry> listFauxAccountEntriesPosProcDebits=listFauxAccountEntriesDebits.FindAll(x => !x.IsAdjustment && CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			for(int i=0;i<listFauxAccountEntriesProcCredits.Count;i++) {
				List<FauxAccountEntry> listFauxAccountEntriesProcDebits=listFauxAccountEntriesPosProcDebits.FindAll(x => x.ProcNum==listFauxAccountEntriesProcCredits[i].ProcNum);
				for(int j=0;j<listFauxAccountEntriesProcDebits.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesProcCredits[i].PrincipalAdjusted)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesProcDebits[j].AmountEnd)) {
						continue;
					}
					//Use PrincipalAdjusted instead of AmountEnd (which could include interest) instead or Principal (has not been adjusted).
					decimal amountToAllocate=Math.Min(listFauxAccountEntriesProcCredits[i].PrincipalAdjusted,listFauxAccountEntriesProcDebits[j].AmountEnd);
					listFauxAccountEntriesProcDebits[j].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesProcCredits[i].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesProcCredits[i].PrincipalAdjusted-=amountToAllocate;
					FauxAccountEntry fauxAccountEntryAllocatedDebit=GetAllocatedDebit(amountToAllocate,listFauxAccountEntriesProcCredits[i],listFauxAccountEntriesProcDebits[j]);
					listFauxAccountEntriesAllocatedDebits.Add(fauxAccountEntryAllocatedDebit);
				}
			}
			#endregion
			#region Remaining Debits
			List<FauxAccountEntry> listFauxAccountEntriesRemainingDebits=listFauxAccountEntriesDebits.FindAll(x => x.IsOffset || CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			for(int i=0;i<listFauxAccountEntriesRemainingDebits.Count;i++) {
				if(CompareDecimal.IsGreaterThanZero(listFauxAccountEntriesRemainingDebits[i].Interest)) {
					//Blindly add interest FauxAccountEntry objects as 'allocated' faux debits.
					listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesRemainingDebits[i]);
					continue;
				}
				if(listFauxAccountEntriesRemainingDebits[i].IsOffset) {
					//Blindly add IsOffset FauxAccountEntry objects as 'allocated' faux debits.
					listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesRemainingDebits[i]);
					continue;
				}
				//We need to make sure and add at least one 'allocated' FauxAccountEntry object for every single debit.
				bool hasAddedDebit=false;
				//Smash any remaining non-interest debits into any credits that can take value (preserves old behavior).
				//Use PrincipalAdjusted instead of AmountEnd (which could include interest) instead or Principal (has not been adjusted).
				List<FauxAccountEntry> listFauxAccountEntriesRemainingCredits=listFauxAccountEntriesCredits.FindAll(x => x.AmountEnd!=x.PrincipalAdjusted);
				for(int j=0;j<listFauxAccountEntriesRemainingCredits.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesRemainingCredits[j].PrincipalAdjusted)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesRemainingDebits[i].AmountEnd)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(listFauxAccountEntriesRemainingCredits[j].PrincipalAdjusted,listFauxAccountEntriesRemainingDebits[i].AmountEnd);
					listFauxAccountEntriesRemainingDebits[i].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesRemainingCredits[j].AmountEnd-=amountToAllocate;
					listFauxAccountEntriesRemainingCredits[j].PrincipalAdjusted-=amountToAllocate;
					FauxAccountEntry fauxAccountEntryAllocatedDebit=GetAllocatedDebit(amountToAllocate,listFauxAccountEntriesRemainingCredits[j],listFauxAccountEntriesRemainingDebits[i]);
					listFauxAccountEntriesAllocatedDebits.Add(fauxAccountEntryAllocatedDebit);
					hasAddedDebit=true;
				}
				if(hasAddedDebit && CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesRemainingDebits[i].AmountEnd)) {
					continue;
				}
				//There are no credits for this debit to apply to... so just preserve the fact that this debit exists.
				//Most likely scenario is that this payment plan has been overcharged (rare).
				listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesRemainingDebits[i]);
			}
			#endregion
			return listFauxAccountEntriesAllocatedDebits;
		}

		private static List<FauxAccountEntry> AllocatePatientPayPlanDebitsToCredits(List<FauxAccountEntry> listFauxAccountEntriesCredits,List<FauxAccountEntry> listFauxAccountEntriesDebits) {
			List<FauxAccountEntry> listFauxAccountEntriesAllocatedDebits=new List<FauxAccountEntry>();
			#region Procedure Credits (consume procedure value)
			//Reduce the AmountEnd on procedure AccountEntry objects based on the value associated with procedure credits.
			//This is so that procedures do not look like they have 'outstanding' value when associated with a payment plan.
			List<FauxAccountEntry> listFauxAccountEntryProcCredits=listFauxAccountEntriesCredits.FindAll(x => x.AccountEntryProc!=null && !x.IsAdjustment && CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			for(int i=0;i<listFauxAccountEntryProcCredits.Count;i++) {
				//Figure out how much value can be removed from the associated procedure.
				//Ignore anything that insurance is going to pay in order to allow payment plans to 'overpay' the procedure before insurance does.
				decimal amountForProcedure=listFauxAccountEntryProcCredits[i].AccountEntryProc.AmountEnd + listFauxAccountEntryProcCredits[i].AccountEntryProc.InsPayAmt;
				decimal amountToRemove=Math.Min(amountForProcedure,listFauxAccountEntryProcCredits[i].AmountEnd);
				listFauxAccountEntryProcCredits[i].AmountEnd-=amountToRemove;
				listFauxAccountEntryProcCredits[i].AccountEntryProc.AmountEnd-=amountToRemove;
				listFauxAccountEntryProcCredits[i].AccountEntryProc.ListPayPlanPrincipalApplieds.Add(
					new PayPlanPrincipalApplied(listFauxAccountEntryProcCredits[i].PayPlanNum,amountToRemove)
				);
			}
			#endregion
			#region Adjustment Credits (associated with procedures)
			//Adjustment credits that are associated with procedures will immediately give the amount of value back to the procedure.
			List<FauxAccountEntry> listFauxAccountEntryProcAdjCredits=listFauxAccountEntriesCredits.FindAll(x => x.AccountEntryProc!=null && x.IsAdjustment);
			for(int i=0;i<listFauxAccountEntryProcAdjCredits.Count;i++) {
				//Blindly adjust PrincipalAdjusted and AmountEnd for this procedure credit by any adjustment credits associated with the same procedure even if it doesn't make sense.
				List<FauxAccountEntry> listFauxAccountEntriesCreditsForProc=listFauxAccountEntryProcCredits.FindAll(x => x.AccountEntryProc==listFauxAccountEntryProcAdjCredits[i].AccountEntryProc);
				for(int j=0;j<listFauxAccountEntriesCreditsForProc.Count;j++) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(listFauxAccountEntryProcAdjCredits[i].AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntriesCreditsForProc[j].PrincipalAdjusted)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(listFauxAccountEntriesCreditsForProc[j].PrincipalAdjusted,Math.Abs(listFauxAccountEntryProcAdjCredits[i].AmountEnd));
					listFauxAccountEntryProcAdjCredits[i].AmountEnd+=amountToAllocate;
					listFauxAccountEntriesCreditsForProc[j].PrincipalAdjusted-=amountToAllocate;
					listFauxAccountEntriesCreditsForProc[j].AccountEntryProc.AmountEnd+=amountToAllocate;
					listFauxAccountEntriesCreditsForProc[j].AccountEntryProc.ListPayPlanPrincipalApplieds.Add(
						new PayPlanPrincipalApplied(listFauxAccountEntryProcAdjCredits[i].PayPlanNum,(amountToAllocate * -1))//Negative because this principal is being removed.
					);
				}
			}
			#endregion
			#region Adjustment Credits (remaining)
			//Smash any remaining adjustment credits into any debits that still have value (preserves old behavior).
			List<FauxAccountEntry> listFauxAccountEntryAdjCredits=listFauxAccountEntriesCredits.FindAll(x => x.IsAdjustment && CompareDecimal.IsLessThanZero(x.AmountEnd));
			for(int i=0;i<listFauxAccountEntryAdjCredits.Count;i++) {
				//Prefer to remove from credits that are not associated with procedures first.
				List<FauxAccountEntry> listFauxAccountEntryNonProcCredits=listFauxAccountEntriesCredits.FindAll(x => !x.IsAdjustment && x.AccountEntryProc==null && CompareDecimal.IsGreaterThanZero(x.PrincipalAdjusted));
				for(int j=0;j<listFauxAccountEntryNonProcCredits.Count;j++) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(listFauxAccountEntryAdjCredits[i].AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntryNonProcCredits[j].PrincipalAdjusted)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(listFauxAccountEntryNonProcCredits[j].PrincipalAdjusted,Math.Abs(listFauxAccountEntryAdjCredits[i].AmountEnd));
					listFauxAccountEntryNonProcCredits[j].PrincipalAdjusted-=amountToAllocate;
					listFauxAccountEntryAdjCredits[i].AmountEnd+=amountToAllocate;
				}
				//Check to see if the entire value has been distributed.
				if(CompareDecimal.IsGreaterThanOrEqualToZero(listFauxAccountEntryAdjCredits[i].AmountEnd)) {
					continue;
				}
				//Distribute any remaining value to procedure credits since it has to go somewhere.
				List<FauxAccountEntry> listFauxAccountEntryRemainingProcCredits=listFauxAccountEntriesCredits.FindAll(x => !x.IsAdjustment && x.AccountEntryProc!=null && CompareDecimal.IsGreaterThanZero(x.PrincipalAdjusted));
				for(int j=0;j<listFauxAccountEntryRemainingProcCredits.Count;j++) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(listFauxAccountEntryAdjCredits[i].AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(listFauxAccountEntryRemainingProcCredits[j].PrincipalAdjusted)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(listFauxAccountEntryRemainingProcCredits[j].PrincipalAdjusted,Math.Abs(listFauxAccountEntryAdjCredits[i].AmountEnd));
					listFauxAccountEntryAdjCredits[i].AmountEnd+=amountToAllocate;
					//Give this amount back to the procedure I guess; The user did some strange nonsense if they got here.
					listFauxAccountEntryRemainingProcCredits[j].PrincipalAdjusted-=amountToAllocate;
					listFauxAccountEntryRemainingProcCredits[j].AccountEntryProc.AmountEnd+=amountToAllocate;
					listFauxAccountEntryRemainingProcCredits[j].AccountEntryProc.ListPayPlanPrincipalApplieds.Add(
						new PayPlanPrincipalApplied(listFauxAccountEntryAdjCredits[i].PayPlanNum,(amountToAllocate * -1))//Negative because this principal is being removed.
					);
				}
			}
			#endregion
			#region Credits (remove AmountEnd value)
			//We don't know how much of this credit is due right now until we consider outstanding debits.
			//Therefore, we need to clear out the AmountEnd variable which holds the value for what the patient owes right now.
			//Also, all adjustment credits should have been considered by now so if there are any with an AmountEnd then their value will get blasted away here because what else are we going to do?
			for(int i=0;i<listFauxAccountEntriesCredits.Count;i++) {
				listFauxAccountEntriesCredits[i].AmountEnd=0;
			}
			#endregion
			#region Debits
			//Patient payment plan debits are never directly associated with production.
			//Therefore, blindly sum up all of the debits (including negative adjustment debits).
			//The sum of all the debits will be the amount that needs to be used in order to 'fill up' the credits up to their PrincipalAdjusted value.
			decimal amountTotalDebits=listFauxAccountEntriesDebits.Sum(x => x.AmountEnd);//Purposefully include negative debits (adjustments).
			//Debits are the AccountEntries that the user interacts with. We are nice to the user and create FauxAccountEntry objects for production (even though they aren't technically associated).
			//Therefore, we need to make at least one FauxAccountEntry object for each debit in the database.
			for(int i=0;i<listFauxAccountEntriesDebits.Count;i++) {
				if(listFauxAccountEntriesDebits[i].IsAdjustment) {
					//Zero out all of the adjustments since their value was just considered within the sum.
					listFauxAccountEntriesDebits[i].AmountEnd=0;
					listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesDebits[i]);
					continue;
				}
				if(CompareDecimal.IsGreaterThanZero(listFauxAccountEntriesDebits[i].Interest)) {
					//Blindly include interest FauxAccountEntries and preserve their AmountEnd value.
					listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesDebits[i]);
					continue;
				}
				//We need to make sure and add at least one 'allocated' FauxAccountEntry object for every single debit.
				bool hasAddedDebit=false;
				decimal amountAvilableForDebit=Math.Min(amountTotalDebits,listFauxAccountEntriesDebits[i].AmountEnd);
				if(CompareDecimal.IsLessThanZero(amountAvilableForDebit)) {
					amountAvilableForDebit=0;
				}
				//Attempt to associate the debit with a credit that has yet to be filled (PrincipalAdjusted is still greater than AmountEnd).
				List<FauxAccountEntry> listFauxAccountEntriesAvailableCredits=listFauxAccountEntriesCredits.FindAll(x => !x.IsAdjustment && CompareDecimal.IsGreaterThan(x.PrincipalAdjusted,x.AmountEnd));
				for(int j=0;j<listFauxAccountEntriesAvailableCredits.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(amountAvilableForDebit)) {
						break;
					}
					decimal amountCreditCanTake=Math.Min(listFauxAccountEntriesAvailableCredits[j].PrincipalAdjusted - listFauxAccountEntriesAvailableCredits[j].AmountEnd,amountAvilableForDebit);
					if(CompareDecimal.IsLessThanOrEqualToZero(amountCreditCanTake)) {
						continue;
					}
					decimal amountToAllocate=Math.Min(amountCreditCanTake,amountTotalDebits);
					listFauxAccountEntriesAvailableCredits[j].AmountEnd+=amountToAllocate;
					amountTotalDebits-=amountToAllocate;
					amountAvilableForDebit-=amountToAllocate;
					listFauxAccountEntriesDebits[i].AmountEnd-=amountToAllocate;
					FauxAccountEntry fauxAccountEntryAllocatedDebit=GetAllocatedDebit(amountToAllocate,listFauxAccountEntriesAvailableCredits[j],listFauxAccountEntriesDebits[i]);
					//Do not suggest paying TP procedures at this time (or procedures that were originally set but not found).
					if((listFauxAccountEntriesAvailableCredits[j].AccountEntryProc==null && listFauxAccountEntriesAvailableCredits[j].ProcNum > 0)
						|| (listFauxAccountEntriesAvailableCredits[j].AccountEntryProc!=null && ((Procedure)listFauxAccountEntriesAvailableCredits[j].AccountEntryProc.Tag).ProcStatus==ProcStat.TP))
					{
						//Detach the procedure and set the pat / prov / clinic to the payment plan values (on the debit).
						fauxAccountEntryAllocatedDebit.ProcNum=0;
						fauxAccountEntryAllocatedDebit.PatNum=listFauxAccountEntriesDebits[i].PatNum;
						fauxAccountEntryAllocatedDebit.ProvNum=listFauxAccountEntriesDebits[i].ProvNum;
						fauxAccountEntryAllocatedDebit.ClinicNum=listFauxAccountEntriesDebits[i].ClinicNum;
					}
					listFauxAccountEntriesAllocatedDebits.Add(fauxAccountEntryAllocatedDebit);
					hasAddedDebit=true;
				}
				if(hasAddedDebit && CompareDecimal.IsLessThanOrEqualToZero(amountAvilableForDebit)) {
					continue;
				}
				if(listFauxAccountEntriesCredits.IsNullOrEmpty()) {
					//Blindly add the debit as it is to the list of allocated debits if there are no credit to apply it to.
					//Patient payment plans do not have to have credits at all... even though that makes no sense.
					amountTotalDebits-=amountAvilableForDebit;
					listFauxAccountEntriesDebits[i].AmountEnd=amountAvilableForDebit;
					listFauxAccountEntriesAllocatedDebits.Add(listFauxAccountEntriesDebits[i]);
				}
				else {
					//YOLO; use the first credit in the list cause it has to go somewhere.
					amountTotalDebits-=amountAvilableForDebit;
					listFauxAccountEntriesDebits[i].AmountEnd-=amountAvilableForDebit;
					listFauxAccountEntriesCredits.First().AmountEnd+=amountAvilableForDebit;
					FauxAccountEntry fauxAccountEntryAllocatedDebit=GetAllocatedDebit(amountAvilableForDebit,listFauxAccountEntriesCredits.First(),listFauxAccountEntriesDebits[i]);
					listFauxAccountEntriesAllocatedDebits.Add(fauxAccountEntryAllocatedDebit);
				}
			}
			#endregion
			return listFauxAccountEntriesAllocatedDebits;
		}

		private static FauxAccountEntry GetAllocatedDebit(decimal amtToAllocate,FauxAccountEntry fauxCredit,FauxAccountEntry fauxDebit) {
			//Create a new faux account entry from the debit but only for the amount that to allocate.
			FauxAccountEntry allocatedDebit=fauxDebit.Copy();
			allocatedDebit.AccountEntryProc=fauxCredit.AccountEntryProc;
			allocatedDebit.AmountEnd=amtToAllocate;
			allocatedDebit.PrincipalAdjusted=amtToAllocate;
			allocatedDebit.PatNum=fauxDebit.PatNum;
			allocatedDebit.Guarantor=fauxDebit.Guarantor;
			//Change specific information on this allocated debit to match the faux credit passed in.
			allocatedDebit.ProvNum=fauxCredit.ProvNum;
			allocatedDebit.ClinicNum=fauxCredit.ClinicNum;
			allocatedDebit.AdjNum=fauxCredit.AdjNum;
			allocatedDebit.ProcNum=fauxCredit.ProcNum;
			allocatedDebit.IsAdjustment=fauxCredit.IsAdjustment;
			return allocatedDebit;
		}

		///<summary>Returns a list of AccountEntries with manipulated amounts due to entities that are explicitly linked to them.
		///An explicit link is a match between an entity itself (procedure, adjustment, etc) along with matching patient, provider, and clinic.</summary>
		private static List<AccountEntry> ExplicitlyLinkCredits(List<AccountEntry> listAccountEntries,List<PaySplit> listSplitsCurrentAndHistoric,bool hasInsOverpay=false,bool hasOffsettingAdjustmets=true) {
			//No remoting role check; no call to db and private method
			List<AccountEntry> listExplicitAccountCharges=listAccountEntries
				.FindAll(x => x.GetType().In(typeof(Procedure),typeof(FauxAccountEntry),typeof(Adjustment)));
			//Create a dictionary that can easily find a corresponding AccountEntry for a specific PaySplit.
			//Old logic did not consider the fact that the same PaySplit could be in multiple AccountEntries so maybe that scenario isn't possible.
			Dictionary<string,AccountEntry> dictPaySplitAccountEntries=new Dictionary<string,AccountEntry>();
			foreach(AccountEntry splitEntry in listAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit))) {
				foreach(PaySplit paySplit in splitEntry.SplitCollection) {
					dictPaySplitAccountEntries[(string)paySplit.TagOD]=splitEntry;
				}
			}
			#region Adjustments
			foreach(AccountEntry accountEntryProc in listExplicitAccountCharges.Where(x => x.GetType()==typeof(Procedure))) {
				//Find every adjustment entry that is explicitly linked to the current procedure and directly manipulate the AmountEnd for both.
				List<AccountEntry> listAdjEntries=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment)
					&& x.ProcNum==accountEntryProc.ProcNum
					&& x.PatNum==accountEntryProc.PatNum
					&& x.ProvNum==accountEntryProc.ProvNum
					&& x.ClinicNum==accountEntryProc.ClinicNum);
				if(listAdjEntries.IsNullOrEmpty()) {
					continue;
				}
				//Remove the entire amount of the negative adjustment (even if the procedure is sent into the negative).
				//This is so that we do not accidentally implicitly pay off anything associated to the same pat/prov/clinic later (if implicit linking).
				decimal sumAdjs=listAdjEntries.Sum(x => x.AmountEnd);
				accountEntryProc.AdjustedAmt=sumAdjs;
				accountEntryProc.AmountEnd+=sumAdjs;
				listAdjEntries.ForEach(x => x.AmountEnd=0);
			}
			//Allow positive and negative adjustments that are incorrectly linked to the same procedure to offset each other if they have the same pat/prov/clinic.
			List<AccountEntry> listAdjProcNegEntries=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment)
				&& x.ProcNum > 0
				&& x.PayPlanNum==0
				&& CompareDecimal.IsLessThanZero(x.AmountEnd));
			List<AccountEntry> listAdjProcPosEntries=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment)
				&& x.ProcNum > 0
				&& x.PayPlanNum==0
				&& CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			for(int i=0;i<listAdjProcNegEntries.Count;i++) {
				//Create a new list of account entries that will hold all of the negative and positive adjustments that match pat/prov/clinic along with the procedure.
				List<AccountEntry> listAdjProcPosNegEntries=new List<AccountEntry>() { listAdjProcNegEntries[i] };
				listAdjProcPosNegEntries.AddRange(listAdjProcPosEntries.FindAll(x => x.ProcNum==listAdjProcNegEntries[i].ProcNum
					&& x.PatNum==listAdjProcNegEntries[i].PatNum
					&& x.ProvNum==listAdjProcNegEntries[i].ProvNum
					&& x.ClinicNum==listAdjProcNegEntries[i].ClinicNum));
				//Have any adjustments found balance each other out as much as possible.
				BalanceAccountEntries(ref listAdjProcPosNegEntries);
			}
			#endregion
			//Group up all current and historical splits by Pat/Prov/Clinic for explicit linking.
			var dictPatProvClinicSplits=listSplitsCurrentAndHistoric.Where(x => !CompareDouble.IsZero(x.SplitAmt))
				.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(var kvpPatProvClinicSplits in dictPatProvClinicSplits) {
				List<PaySplit> listPatProvClinicSplits=kvpPatProvClinicSplits.Value;
				//Get a subset of account entries that can be explicitly linked to these splits.
				List<AccountEntry> listPatProvClinicAccountCharges=listExplicitAccountCharges.FindAll(x => x.PatNum==kvpPatProvClinicSplits.Key.PatNum
					&& x.ProvNum==kvpPatProvClinicSplits.Key.ProvNum
					&& x.ClinicNum==kvpPatProvClinicSplits.Key.ClinicNum);
				//Prefer explicit links to procedures, faux account entries, and then adjustments.
				//This is because splits can be vicariously attached to adjustments via the procedure but the split should prefer the procedure first.
				List<AccountEntry> listProcEntries=listPatProvClinicAccountCharges.FindAll(x => x.GetType()==typeof(Procedure));
				List<AccountEntry> listAdjEntries=listPatProvClinicAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment));
				//NOTE: Any explicitly linked paysplit needs to be used on what it's attached to in its entirety (even if it's overpaid).
				#region Procedures
				foreach(AccountEntry procEntry in listProcEntries) {
					foreach(PaySplit procSplit in listPatProvClinicSplits.FindAll(x => x.ProcNum==procEntry.ProcNum && x.PayPlanNum==0)) {
						decimal splitAmt=(decimal)procSplit.SplitAmt;//Overpayment on procedures is handled later
						procEntry.AmountEnd-=splitAmt;
						procEntry.SplitCollection.Add(procSplit.Copy());//take copy so we can get amtPaid without overwriting.
						procSplit.SplitAmt-=(double)splitAmt;
						if(dictPaySplitAccountEntries.TryGetValue((string)procSplit.TagOD,out AccountEntry splitEntry)) {
							splitEntry.AmountEnd+=splitAmt;
						}
					}
				}
				#endregion
				#region FauxAccountEntry
				//Get a subset of account entries that can be explicitly linked to these splits based off of guarantor if the payment plan version calls for it.
				//The guarantor on the payment plan is almost always in charge of paying for the payment plan.
				//Therefore, payment splits should be considered explicitly linked to the payment plan charge even when the PatNum does not match, but the guarantor does.
				bool useGuar=(PrefC.GetEnum<PayPlanVersions>(PrefName.PayPlansVersion)!=PayPlanVersions.NoCharges);
				List<FauxAccountEntry> listPayPlanEntries=listExplicitAccountCharges
					.FindAll(x => x.ProvNum==kvpPatProvClinicSplits.Key.ProvNum
						&& x.ClinicNum==kvpPatProvClinicSplits.Key.ClinicNum
						&& x.GetType()==typeof(FauxAccountEntry))
					.Cast<FauxAccountEntry>()
					.Where(x => (useGuar ? x.Guarantor==kvpPatProvClinicSplits.Key.PatNum : x.PatNum==kvpPatProvClinicSplits.Key.PatNum))
					.ToList();
				//Negative payment splits are created for payment plan charges when money is transferred away from them. There should be offsetting splits when this scenario has happened.
				//Apply any negative splits to positive splits that are explicitly linked to exact same production entry.
				List<PaySplit> listPayPlanChargeSplitsNegative=listPatProvClinicSplits.FindAll(x => CompareDouble.IsLessThanZero(x.SplitAmt) && x.UnearnedType==0 && x.PayPlanNum > 0);
				for(int i=0;i<listPayPlanChargeSplitsNegative.Count;i++) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(listPayPlanChargeSplitsNegative[i].SplitAmt)) {
						continue;
					}
					//Find any offsetting positive payment splits (linked to exactly the same production entry).
					List<PaySplit> listPayPlanChargeSplitsPositive=listPatProvClinicSplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt)
							&& x.UnearnedType==listPayPlanChargeSplitsNegative[i].UnearnedType
							&& x.PayPlanNum==listPayPlanChargeSplitsNegative[i].PayPlanNum
							&& x.AdjNum==listPayPlanChargeSplitsNegative[i].AdjNum
							&& x.ProcNum==listPayPlanChargeSplitsNegative[i].ProcNum)
						.OrderByDescending(x => x.PayPlanChargeNum==listPayPlanChargeSplitsNegative[i].PayPlanChargeNum)
						.ThenByDescending(x => x.PayPlanDebitType==listPayPlanChargeSplitsNegative[i].PayPlanDebitType)
						.ThenByDescending(x => x.PayPlanDebitType==PayPlanDebitTypes.Principal)
						.ThenByDescending(x => x.DatePay)
						.ThenByDescending(x => Math.Abs(x.SplitAmt)==Math.Abs(listPayPlanChargeSplitsNegative[i].SplitAmt))
						.ToList();
					//Offset the payment splits as much as possible.
					for(int j=0;j<listPayPlanChargeSplitsPositive.Count;j++) {
						if(CompareDouble.IsLessThanOrEqualToZero(listPayPlanChargeSplitsPositive[j].SplitAmt)) {
							break;
						}
						if(CompareDecimal.IsGreaterThanOrEqualToZero(listPayPlanChargeSplitsNegative[i].SplitAmt)) {
							continue;
						}
						double splitAmountBeingApplied=Math.Min(Math.Abs(listPayPlanChargeSplitsNegative[i].SplitAmt),listPayPlanChargeSplitsPositive[j].SplitAmt);
						listPayPlanChargeSplitsNegative[i].SplitAmt+=splitAmountBeingApplied;
						if(dictPaySplitAccountEntries.TryGetValue((string)listPayPlanChargeSplitsNegative[i].TagOD,out AccountEntry accountEntrySplitNegative)) {
							accountEntrySplitNegative.AmountEnd-=(decimal)splitAmountBeingApplied;
						}
						listPayPlanChargeSplitsPositive[j].SplitAmt-=splitAmountBeingApplied;
						if(dictPaySplitAccountEntries.TryGetValue((string)listPayPlanChargeSplitsPositive[j].TagOD,out AccountEntry accountEntrySplitPositive)) {
							accountEntrySplitPositive.AmountEnd+=(decimal)splitAmountBeingApplied;
						}
					}
				}
				//Apply all positive splits that are explicitly linked to the PayPlanCharge. Generic payment plan splits will be applied to PayPlanCharges if there is anything left over.
				foreach(AccountEntry payPlanChargeEntry in listPayPlanEntries) {
					List<PaySplit> listPayPlanChargeSplits=listPatProvClinicSplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt)
							&& x.UnearnedType==0
							&& x.PayPlanNum==payPlanChargeEntry.PayPlanNum
							&& x.PayPlanChargeNum==payPlanChargeEntry.PayPlanChargeNum)
						.OrderByDescending(x => x.ProcNum==payPlanChargeEntry.ProcNum && x.AdjNum==payPlanChargeEntry.AdjNum)
						.ToList();
					foreach(PaySplit payPlanChargeSplit in listPayPlanChargeSplits) {
						//Production that is associated to the split must match the production on the charge to be considered explicitly linked.
						if(((payPlanChargeSplit.ProcNum > 0 || payPlanChargeSplit.AdjNum > 0) 
								&& (payPlanChargeSplit.ProcNum!=payPlanChargeEntry.ProcNum || payPlanChargeSplit.AdjNum!=payPlanChargeEntry.AdjNum))
							|| (payPlanChargeSplit.ProcNum==0 && payPlanChargeSplit.AdjNum==0
								&& (payPlanChargeSplit.ProcNum!=payPlanChargeEntry.ProcNum || payPlanChargeSplit.AdjNum!=payPlanChargeEntry.AdjNum)))
						{
							continue;
						}
						decimal splitAmt=Math.Min((decimal)payPlanChargeSplit.SplitAmt,payPlanChargeEntry.AmountEnd);
						payPlanChargeEntry.AmountEnd-=splitAmt;
						payPlanChargeEntry.SplitCollection.Add(payPlanChargeSplit.Copy());//take copy so we can get amtPaid without overwriting.
						payPlanChargeSplit.SplitAmt-=(double)splitAmt;
						if(dictPaySplitAccountEntries.TryGetValue((string)payPlanChargeSplit.TagOD,out AccountEntry splitEntry)) {
							splitEntry.AmountEnd+=splitAmt;
						}
					}
				}
				//Do the same thing over but this time do it on a payment plan level (old splits won't always be explicitly linked to a PayPlanCharge).
				foreach(AccountEntry payPlanChargeEntry in listPayPlanEntries.Where(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd))) {
					List<PaySplit> listPayPlanSplits=listPatProvClinicSplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt)
						&& x.UnearnedType==0
						&& x.PayPlanNum==payPlanChargeEntry.PayPlanNum);
					foreach(PaySplit payPlanSplit in listPayPlanSplits) {
						decimal splitAmt=Math.Min((decimal)payPlanSplit.SplitAmt,payPlanChargeEntry.AmountEnd);
						if(CompareDecimal.IsZero(splitAmt)) {
							break;
						}
						//Production that is associated to the split must match the production on the charge to be considered explicitly linked.
						if((payPlanSplit.ProcNum > 0 || payPlanSplit.AdjNum > 0)
							&& (payPlanSplit.ProcNum!=payPlanChargeEntry.ProcNum || payPlanSplit.AdjNum!=payPlanChargeEntry.AdjNum))
						{
							continue;
						}
						payPlanChargeEntry.AmountEnd-=splitAmt;
						payPlanChargeEntry.SplitCollection.Add(payPlanSplit.Copy());//take copy so we can get amtPaid without overwriting.
						payPlanSplit.SplitAmt-=(double)splitAmt;
						if(dictPaySplitAccountEntries.TryGetValue((string)payPlanSplit.TagOD,out AccountEntry splitEntry)) {
							splitEntry.AmountEnd+=splitAmt;
						}
					}
				}
				#endregion
				#region Adjustment
				List<PaySplit> listPaySplits=listPatProvClinicSplits.FindAll(x => x.PayPlanNum==0 && !CompareDouble.IsZero(x.SplitAmt));
				List<AdjustmentSplitGroup> listAdjustmentSplitGroups=listPaySplits.FindAll(x => x.AdjNum > 0)
					.GroupBy(x => x.AdjNum)
					.ToDictionary(x => x.Key,x => x.ToList())
					.Select(x => new AdjustmentSplitGroup() { AdjNum=x.Key, ListPaySplits=x.Value })
					.ToList();
				List<ProcedureSplitGroup> listProcedureSplitGroups=listPaySplits.FindAll(x => x.ProcNum > 0)
					.GroupBy(x => x.ProcNum)
					.ToDictionary(x => x.Key,x => x.ToList())
					.Select(x => new ProcedureSplitGroup() { ProcNum=x.Key, ListPaySplits=x.Value })
					.ToList();
				foreach(AccountEntry adjEntry in listAdjEntries) {
					List<PaySplit> listAdjSplits=new List<PaySplit>();
					AdjustmentSplitGroup adjustmentSplitGroup=listAdjustmentSplitGroups.FirstOrDefault(x => x.AdjNum==adjEntry.AdjNum);
					if(adjustmentSplitGroup!=null) {
						listAdjSplits.AddRange(adjustmentSplitGroup.ListPaySplits);
					}
					ProcedureSplitGroup procedureSplitGroup=listProcedureSplitGroups.FirstOrDefault(x => x.ProcNum==adjEntry.ProcNum);
					if(procedureSplitGroup!=null) {
						listAdjSplits.AddRange(procedureSplitGroup.ListPaySplits);
					}
					foreach(PaySplit adjSplit in listAdjSplits.Distinct()) {
						decimal splitAmt=(decimal)adjSplit.SplitAmt;//Overpayment on procedures is handled later
						adjEntry.AmountEnd-=splitAmt;
						adjEntry.SplitCollection.Add(adjSplit.Copy());//take copy so we can get amtPaid without overwriting.
						adjSplit.SplitAmt-=(double)splitAmt;
						if(dictPaySplitAccountEntries.TryGetValue((string)adjSplit.TagOD,out AccountEntry splitEntry)) {
							splitEntry.AmountEnd+=splitAmt;
						}
					}
				}
				#endregion
			}
			#region Insurance Overpayments
			//Insurance overpayments should not be transferred around. The user needs to be warned to manually handle this scenario themselves.
			List<AccountEntry> listInsProcEntries=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Procedure) && x.InsPayAmt > 0);
			//However, allow ZZZFIX procedures to have insurance payments transferred around since they are conversion related.
			ProcedureCode codeZZZFIX=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode=="ZZZFIX");
			if(codeZZZFIX!=null) {
				listInsProcEntries.RemoveAll(x => ((Procedure)x.Tag).CodeNum==codeZZZFIX.CodeNum);
			}
			//Allow all procedures to include insurance overpayments if the calling method desired it.
			if(hasInsOverpay) {
				listInsProcEntries.Clear();//Don't do anything about insurance overpayments and leave the full overpayment value within AmountEnd.
			}
			//Find adjustments linked to a procedure that are not associated with a payment plan and have a non-zero amount.
			List<AccountEntry> listAccountEntriesImplicitAdj=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Adjustment) && x.ProcNum!=0 && x.PayPlanNum==0 && x.AmountEnd!=0);
			foreach(AccountEntry accountEntryInsProc in listInsProcEntries) {
				decimal amountAfterIns=AccountEntry.GetExplicitlyLinkedProcAmt(accountEntryInsProc);
				if(amountAfterIns>=0) {
					//Insurance has not overpaid the procedure fee itself so no need to check explicitly linked amounts.
					continue;
				}
				if(accountEntryInsProc.AmountEnd>=0 || (accountEntryInsProc.AmountEnd + amountAfterIns)>=0) {
					//Something that was explicitly linked to the procedure was enough to offset the insurance overpayment (e.g. an adjustment).
					continue;//The procedure itself was overpaid but the procedure + explicitly linked entries caused insurance to not 'overpay'.
				}
				//Warn the user that insurance overpayment has been detected and provide as much detail as possible so that they can go investigate and manually fix the problem.
				accountEntryInsProc.WarningMsg.AppendLine($"ProcNum #{accountEntryInsProc.ProcNum} for PatNum #{accountEntryInsProc.PatNum} has ignored insurance overpayment:");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^{accountEntryInsProc.DescriptionForGrid} on {accountEntryInsProc.Date.ToShortDateString()}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^ProcFeeTotal: {((Procedure)accountEntryInsProc.Tag).ProcFeeTotal:C}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^AdjustedAmt: {accountEntryInsProc.AdjustedAmt:C}");
				decimal sumPrincipalApplied=0;
				if(!accountEntryInsProc.ListPayPlanPrincipalApplieds.IsNullOrEmpty()) {
					sumPrincipalApplied=accountEntryInsProc.ListPayPlanPrincipalApplieds.Sum(x => x.PrincipalApplied);
				}
				accountEntryInsProc.WarningMsg.AppendLine($"  ^PayPlan Credits: {sumPrincipalApplied:C}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^InsPayAmt: {accountEntryInsProc.InsPayAmt:C}");
				decimal sumSplitAmt=accountEntryInsProc.SplitCollection.Sum(x => (decimal)x.SplitAmt);
				accountEntryInsProc.WarningMsg.AppendLine($"  ^PatPayAmt: {sumSplitAmt:C}");
				//Only allow up to the patient payment amount to be transferred around.
				decimal amountOverpaid=Math.Abs(accountEntryInsProc.AmountEnd);
				decimal amountTransferable=Math.Min(sumSplitAmt,amountOverpaid);
				accountEntryInsProc.WarningMsg.AppendLine($"  ^Overpayment: {amountOverpaid:C}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^Transferable PatPayAmt: {amountTransferable:C}");
				//Spell out the real problem; the following amount of income will be ignored.
				decimal sumIgnoredInsuranceOverpayment=(amountOverpaid - amountTransferable);
				accountEntryInsProc.WarningMsg.AppendLine($"  ^Ignored insurance overpayment: {sumIgnoredInsuranceOverpayment:C}");
				//Sum up any mismatched adjustments that don't offset the procedure fee.
				decimal sumImplicitAdjustmentEntries=listAccountEntriesImplicitAdj
					.FindAll(x => x.ProcNum==accountEntryInsProc.ProcNum)
					.Sum(x => x.AmountEnd);
				if(sumImplicitAdjustmentEntries!=0) {
					accountEntryInsProc.WarningMsg.AppendLine($"  ^Attached Adjustment Total: {sumImplicitAdjustmentEntries:C}");
					accountEntryInsProc.WarningMsg.AppendLine($"    (Not offsetting the procedure value due to a mismatch in Patient, Provider, or Clinic)");
				}
				//Only allow the procedure to be as negative as the amount of associated patient payments.
				//The ITM will be given the opportunity to transfer this amount to other production.
				accountEntryInsProc.AmountEnd=(amountTransferable * -1);
			}
			#endregion
			#region Adjustments - Offset Unattached
			//Positive and negative unattached adjustments should offset each other if the 'AdjustmentsOffsetEachOther' preference says so.
			if(hasOffsettingAdjustmets && PrefC.GetBool(PrefName.AdjustmentsOffsetEachOther)) {
				List<AccountEntry> listUnattachedAdjustmentEntries=listExplicitAccountCharges.FindAll(x => x.AdjNum > 0
					&& x.PayPlanNum==0
					&& x.ProcNum==0
					&& x.GetType()==typeof(Adjustment));
				List<AccountEntry> listPositiveUnattachedAdjustmentEntries=listUnattachedAdjustmentEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
				List<AccountEntry> listNegativeUnattachedAdjustmentEntries=listUnattachedAdjustmentEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
				ExplicitlyLinkPositiveNegativeEntries(ref listPositiveUnattachedAdjustmentEntries,ref listNegativeUnattachedAdjustmentEntries);
			}
			#endregion
			return ExplicitlyLinkUnearnedTogether(listAccountEntries);
		}

		///<summary>This method applies positive and negative unearned to each other so the sum is correct before making any transfers.
		///This method assumes that explicit linking has been done before it is called so the unallocated splits no longer have any value (if any)
		///Without this method when a procedure, postive unearned, and negative unearned are on an account the postive would be applied to the procedure,
		///leaving a negative on the account. We want the positive and negative to cancel each other out before any transfers are made.</summary>
		private static List<AccountEntry> ExplicitlyLinkUnearnedTogether(List<AccountEntry> listAccountCharges) {
			//No remoting role check; no call to db and private method
			List<AccountEntry> listUnearned=listAccountCharges.FindAll(x => x.IsUnearned);
			//Prefer to link unearned that is attached to the same payment plan together first.
			Dictionary<long,List<AccountEntry>> dictPayPlanEntries=listUnearned.GroupBy(x => x.PayPlanNum).ToDictionary(x => x.Key,x => x.ToList());
			foreach(long payPlanNum in dictPayPlanEntries.Keys) {
				List<AccountEntry> listPositiveUnearnedPP=dictPayPlanEntries[payPlanNum].FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
				List<AccountEntry> listNegativeUnearnedPP=dictPayPlanEntries[payPlanNum].FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
				ExplicitlyLinkPositiveNegativeEntries(ref listPositiveUnearnedPP,ref listNegativeUnearnedPP);
			}
			//After both regular and payment plan unearned splits have been considered separately, lump them all together.
			List<AccountEntry> listPositiveUnearned=listUnearned.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			List<AccountEntry> listNegativeUnearned=listUnearned.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
			ExplicitlyLinkPositiveNegativeEntries(ref listPositiveUnearned,ref listNegativeUnearned);
			return listAccountCharges;
		}

		private static void ExplicitlyLinkPositiveNegativeEntries(ref List<AccountEntry> listPositiveEntries,ref List<AccountEntry> listNegativeEntries) {
			//No remoting role check; no call to db and private method
			foreach(AccountEntry positiveEntry in listPositiveEntries) {
				foreach(AccountEntry negativeEntry in listNegativeEntries) {
					if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
						continue;//no more money to apply.
					}
					if(positiveEntry.ProvNum!=negativeEntry.ProvNum
						|| positiveEntry.PatNum!=negativeEntry.PatNum
						|| positiveEntry.ClinicNum!=negativeEntry.ClinicNum
						|| positiveEntry.UnearnedType!=negativeEntry.UnearnedType)
					{
						continue;
					}
					decimal amount=Math.Min(Math.Abs(positiveEntry.AmountEnd),Math.Abs(negativeEntry.AmountEnd));
					positiveEntry.AmountEnd-=amount;
					negativeEntry.AmountEnd+=amount;
				}
			}
		}

		///<summary>Attempts to implicitly link old unattached payments to past production that has not explicitly has payments attached to them.
		///This will give the patient a better idea on what they need to pay off next.
		///It is important to invoke ExplicitlyLinkCredits() prior to this method.</summary>
		///<param name="listPaySplits">All payment splits for the family.</param>
		///<param name="listInsPayAsTotal">All claimprocs paid as total for the family, might contain ins payplan payments.
		///Adds claimprocs for the completed procedures if in income xfer mode.</param>
		///<param name="listAccountCharges">All account entries generated by ConstructListCharges() and ExplicitlyLinkCredits().
		///Can include account charges for the family.</param>
		///<param name="listSplitsCur">All splits associated to payCur.  Empty list for a new payment.</param>
		///<param name="listPayFirstAcctEntries">All account entries that payCur should be linked to first.
		///If payCur.PayAmt is greater than the sum of these account entries then any leftover amount will be implicitly linked to other entries.</param>
		///<param name="patNum">The PatNum of the currently selected patient.</param>
		///<param name="isPatPrefer">Set to true if account entries for patNum should be prioritized before other entries.</param>
		///<returns>A helper class that represents the implicit credits that this method made.</returns>
		private static PayResults ImplicitlyLinkCredits(ref List<PaySplit> listPaySplits,List<PayAsTotal> listInsPayAsTotal,
			List<AccountEntry> listAccountCharges,List<PaySplit> listSplitsCur,List<AccountEntry> listPayFirstAcctEntries,long patNum,
			bool isPatPrefer,bool isAllocateUnearned=false)
		{
			//No remoting role check; no call to db and private method
			if(isPatPrefer) {
				//Shove all account entries associated to patNum passed in to the bottom of the list so that they are implicitly linked to last.
				listAccountCharges=listAccountCharges.OrderByDescending(x => x.PatNum!=patNum).ThenBy(x => x.Date).ToList();
			}
			//Make a deep copy of all splits because the SplitAmt will get directly manipulated within implicit linking processing.
			List<PaySplit> listSplitsCopied=listPaySplits.Select(x => x.Copy()).ToList();
			//Create a list of account entries that ignore TP procs as they should never be implicitly paid.
			List<AccountEntry> listImplicitCharges=new List<AccountEntry>(listAccountCharges);
			//Never auto split to treatment planned entries
			listImplicitCharges.RemoveAll(x => x.GetType()==typeof(Procedure) && ((Procedure)x.Tag).ProcStatus==ProcStat.TP);
			//Remove every unearned and unallocated split and entry when executing implicit linking for the AllocateUnearned system.
			if(isAllocateUnearned) {
				listSplitsCopied.RemoveAll(x => x.UnearnedType > 0 || x.IsUnallocated);
				listImplicitCharges.RemoveAll(x => x.IsUnearned || x.IsUnallocated);
			}
			//Patient payment plans can have credits attached to treatment planned procedures, ignore those as well.
			listImplicitCharges.RemoveAll(x => x.GetType()==typeof(FauxAccountEntry)
				&& ((FauxAccountEntry)x.Tag).AccountEntryProc!=null
				&& ((FauxAccountEntry)x.Tag).AccountEntryProc.GetType()==typeof(Procedure)
				&& ((Procedure)((FauxAccountEntry)x.Tag).AccountEntryProc.Tag).ProcStatus==ProcStat.TP);
			//Push account entries that correspond to the account entries that should be 'paid first' to the end of the implicit charge list.
			//This will cause unallocated account entries to prefer other account entries first in order to leave as much value as possible on the 'paid first' entries.
			if(!listPayFirstAcctEntries.IsNullOrEmpty()) {
				//Shove all of the selected account entries to the bottom of listImplicitCharges so that they are implicitly linked to last.
				listImplicitCharges=listImplicitCharges.OrderBy(x => listPayFirstAcctEntries.Any(y => y.PriKey==x.PriKey && y.GetType()==x.GetType()))
					.ThenBy(x => listPayFirstAcctEntries.Any(y => y.PayPlanNum > 0 && y.PayPlanNum==x.PayPlanNum))
					.ToList();
			}
			//Users can manually create payment splits that are not explicitly linked to production.
			//These splits are often times the result of the user attempting to create a manual transfer that is related to the payment plan.
			//All negative and positive splits need to offset each other.
			BalancePaymentPlanSplits(ref listSplitsCopied);
			foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
				if(layer==AccountBalancingLayers.Unearned) {
					continue;
				}
				List<ImplicitLinkBucket> listBuckets=CreateImplicitLinkBucketsForLayer(layer,listInsPayAsTotal,listSplitsCopied,listImplicitCharges);
				foreach(ImplicitLinkBucket bucket in listBuckets) {
					ProcessImplicitLinkBucket(bucket);
				}
			}
			//At this point implicit linking has been performed as accurately as possible (in regards to the bucket system).
			//Now is the point in the process where ANY negative account entry can be applied towards ANY positive entry FIFO style.
			//E.g. Some chain of events could have taken place to cause a procedure to get overpaid (AmountEnd of -$20).
			//This negative production needs to be applied to another piece of production that has a positive value.
			//Ergo, it can be applied towards another procedure with an AmountEnd of $40 (just as long as it is positive).
			BalanceAccountEntries(ref listImplicitCharges);
			if(isPatPrefer) {
				//The list of account entries were sorted at the beginning of this method to make charges associated to the patNum passed in 'unpaid'.
				//Shove all account entries associated to patNum to the top of the list so that calling methods prefer these entries first.
				listAccountCharges=listAccountCharges.OrderBy(x => x.PatNum!=patNum).ThenBy(x => x.Date).ToList();
			}
			PayResults implicitCredits=new PayResults();
			implicitCredits.ListAccountCharges=listAccountCharges;
			implicitCredits.ListSplitsCur=listSplitsCur;
			return implicitCredits;
		}

		///<summary>A helper method that will move as much SplitAmt as possible from the negative payment plan splits and apply them to the positive ones.
		///Returns true if account entries changed.  Otherwise; false.</summary>
		private static bool BalancePaymentPlanSplits(ref List<PaySplit> listPaySplits) {
			//No remoting role check; private method
			bool hasChanges=false;
			List<PayPlanSplitGroup> listPayPlanSplitGroups=listPaySplits.Where(x => x.PayPlanNum > 0)
				.GroupBy(x => x.PayPlanNum)
				.Select(x => new PayPlanSplitGroup() { PayPlanNum=x.Key, ListPaySplits=x.ToList() })
				.ToList();
			for(int i=0;i<listPayPlanSplitGroups.Count;i++) {
				List<PaySplit> listPaySplitsPositive=listPayPlanSplitGroups[i].ListPaySplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt))
					//The below OrderBys are designed to ensure that credits of a given type are applied to debits of the same type.
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest)//We should always apply Interest first
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal)//Principal should be handled before Unknown
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Unknown)
					.ToList();
				List<PaySplit> listPaySplitsNegative=listPayPlanSplitGroups[i].ListPaySplits.FindAll(x => CompareDouble.IsLessThanZero(x.SplitAmt))
					//The below OrderBys are designed to ensure that credits of a given type are applied to debits of the same type.
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest)//We should always apply Interest first
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal)//Principal should be handled before Unknown
					.OrderBy(x=>x.PayPlanDebitType==PayPlanDebitTypes.Unknown)
					.ToList();
				for(int j=0;j<listPaySplitsPositive.Count;j++) {
					if(CompareDouble.IsLessThanOrEqualToZero(listPaySplitsPositive[j].SplitAmt)) {
						continue;
					}
					for(int k=0;k<listPaySplitsNegative.Count;k++) {
						if(CompareDouble.IsLessThanOrEqualToZero(listPaySplitsPositive[j].SplitAmt)) {
							break;
						}
						if(CompareDecimal.IsGreaterThanOrEqualToZero(listPaySplitsNegative[k].SplitAmt)) {
							continue;
						}
						hasChanges=true;
						double amountTxfr=Math.Min(Math.Abs(listPaySplitsPositive[j].SplitAmt),Math.Abs(listPaySplitsNegative[k].SplitAmt));
						listPaySplitsPositive[j].SplitAmt-=amountTxfr;
						listPaySplitsNegative[k].SplitAmt+=amountTxfr;
					}
				}
			}
			return hasChanges;
		}

		///<summary>Groups up the account entries passed in into buckets based on the layer passed in.</summary>
		private static List<ImplicitLinkBucket> CreateImplicitLinkBucketsForLayer(AccountBalancingLayers layer,List<PayAsTotal> listInsPayAsTotal,
			List<PaySplit> listPaySplits,List<AccountEntry> listAccountEntries)
		{
			//No remoting role check; private method
			switch(layer) {
				case AccountBalancingLayers.ProvPatClinic:
					List<ImplicitLinkBucket> listProvPatClinicBuckets=listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketProvPatClinic in listProvPatClinicBuckets) {
						long patNum=bucketProvPatClinic.ListAccountEntries.First().PatNum;
						long provNum=bucketProvPatClinic.ListAccountEntries.First().ProvNum;
						long clinicNum=bucketProvPatClinic.ListAccountEntries.First().ClinicNum;
						bucketProvPatClinic.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.PatNum==patNum && x.ProvNum==provNum && x.ClinicNum==clinicNum);
						bucketProvPatClinic.ListPaySplits=listPaySplits.FindAll(x => x.PatNum==patNum && x.ProvNum==provNum && x.ClinicNum==clinicNum);
					}
					return listProvPatClinicBuckets;
				case AccountBalancingLayers.ProvPat:
					List<ImplicitLinkBucket> listProvPatBuckets=listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketProvPat in listProvPatBuckets) {
						long patNum=bucketProvPat.ListAccountEntries.First().PatNum;
						long provNum=bucketProvPat.ListAccountEntries.First().ProvNum;
						bucketProvPat.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.PatNum==patNum && x.ProvNum==provNum);
						bucketProvPat.ListPaySplits=listPaySplits.FindAll(x => x.PatNum==patNum && x.ProvNum==provNum);
					}
					return listProvPatBuckets;
				case AccountBalancingLayers.ProvClinic:
					List<ImplicitLinkBucket> listProvClinicBuckets=listAccountEntries.GroupBy(x => new { x.ProvNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketProvClinic in listProvClinicBuckets) {
						long provNum=bucketProvClinic.ListAccountEntries.First().ProvNum;
						long clinicNum=bucketProvClinic.ListAccountEntries.First().ClinicNum;
						bucketProvClinic.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.ProvNum==provNum && x.ClinicNum==clinicNum);
						bucketProvClinic.ListPaySplits=listPaySplits.FindAll(x => x.ProvNum==provNum && x.ClinicNum==clinicNum);
					}
					return listProvClinicBuckets;
				case AccountBalancingLayers.PatClinic:
					List<ImplicitLinkBucket> listPatClinicBuckets=listAccountEntries.GroupBy(x => new { x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketPatClinic in listPatClinicBuckets) {
						long patNum=bucketPatClinic.ListAccountEntries.First().PatNum;
						long clinicNum=bucketPatClinic.ListAccountEntries.First().ClinicNum;
						bucketPatClinic.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.PatNum==patNum && x.ClinicNum==clinicNum);
						bucketPatClinic.ListPaySplits=listPaySplits.FindAll(x => x.PatNum==patNum && x.ClinicNum==clinicNum);
					}
					return listPatClinicBuckets;
				case AccountBalancingLayers.Prov:
					List<ImplicitLinkBucket> listProvBuckets=listAccountEntries.GroupBy(x => new { x.ProvNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketProv in listProvBuckets) {
						long provNum=bucketProv.ListAccountEntries.First().ProvNum;
						bucketProv.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.ProvNum==provNum);
						bucketProv.ListPaySplits=listPaySplits.FindAll(x => x.ProvNum==provNum);
					}
					return listProvBuckets;
				case AccountBalancingLayers.Pat:
					List<ImplicitLinkBucket> listPatBuckets=listAccountEntries.GroupBy(x => new { x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketPat in listPatBuckets) {
						long patNum=bucketPat.ListAccountEntries.First().PatNum;
						bucketPat.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.PatNum==patNum);
						bucketPat.ListPaySplits=listPaySplits.FindAll(x => x.PatNum==patNum);
					}
					return listPatBuckets;
				case AccountBalancingLayers.Clinic:
					List<ImplicitLinkBucket> listClinicBuckets=listAccountEntries.GroupBy(x => new { x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ImplicitLinkBucket(x.Value))
						.ToList();
					foreach(ImplicitLinkBucket bucketClinic in listClinicBuckets) {
						long clinicNum=bucketClinic.ListAccountEntries.First().ClinicNum;
						bucketClinic.ListInsPayAsTotal=listInsPayAsTotal.FindAll(x => x.ClinicNum==clinicNum);
						bucketClinic.ListPaySplits=listPaySplits.FindAll(x => x.ClinicNum==clinicNum);
					}
					return listClinicBuckets;
				case AccountBalancingLayers.Nothing:
					//Create a single bucket to hold all entities:
					ImplicitLinkBucket bucket=new ImplicitLinkBucket(listAccountEntries);
					bucket.ListInsPayAsTotal=listInsPayAsTotal;
					bucket.ListPaySplits=listPaySplits;
					return new List<ImplicitLinkBucket>() {
						bucket
					};
				case AccountBalancingLayers.Unearned:
				default:
					throw new ODException($"Income transfer buckets cannot be created for unsupported layer: {layer}");
			}
		}

		///<summary></summary>
		public static void ProcessImplicitLinkBucket(ImplicitLinkBucket bucket) {
			//No remoting role check; no call to db
			#region PayAsTotal
			foreach(PayAsTotal payAsTotal in bucket.ListInsPayAsTotal) {//Use claim payments by total to pay off procedures for that specific patient.
				if(payAsTotal.SummedInsPayAmt==0) {
					continue;
				}
				foreach(AccountEntry accountEntry in bucket.ListAccountEntries) {
					if(payAsTotal.SummedInsPayAmt==0) {
						break;
					}
					if(accountEntry.AmountEnd==0) {
						continue;
					}
					if(accountEntry.GetType().In(typeof(PayPlanCharge),typeof(FauxAccountEntry))) {
						continue;
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,payAsTotal.SummedInsPayAmt);
					accountEntry.AmountEnd-=(decimal)amt;
					payAsTotal.SummedInsPayAmt-=amt;
				}
			}
			#endregion
			#region PaySplits
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => !string.IsNullOrEmpty(x.ItemValue))//If ItemValue is not blank, it means "do not show on account"
				.Select(x => x.DefNum).ToList();
			List<PaySplit> listLinkableSplits=bucket.ListPaySplits.FindAll(x => !listHiddenUnearnedDefNums.Contains(x.UnearnedType));
			List<PaySplit> listLinkablePosSplits=listLinkableSplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt));
			List<PaySplit> listLinkableNegSplits=listLinkableSplits.FindAll(x => CompareDouble.IsLessThanZero(x.SplitAmt));
			#region Payment Plans
			foreach(PaySplit split in listLinkablePosSplits.FindAll(x => x.PayPlanNum > 0)) {
				foreach(AccountEntry accountEntry in bucket.ListAccountEntries.FindAll(x => x.PayPlanNum==split.PayPlanNum)) {
					if(CompareDouble.IsZero(split.SplitAmt)) {
						break;//Split's amount has been used by previous charges.
					}
					if(CompareDecimal.IsZero(accountEntry.AmountEnd)) {
						continue;
					}
					if(accountEntry.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
						continue;//we do not implicitly link to TP procedures
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,split.SplitAmt);
					//Manipulate the amounts but do not officially link this split to the accountEntry (via SplitCollection) since it is not explicitly linked.
					accountEntry.AmountEnd-=(decimal)amt;
					split.SplitAmt-=amt;
				}
			}
			#endregion
			#region Non-Payment Plans
			//Loop through any negative pay splits and offset their value with any other positive pay split within this bucket.
			foreach(PaySplit positiveSplit in listLinkablePosSplits.FindAll(x => x.PayPlanNum==0)) {
				if(CompareDouble.IsLessThanOrEqualToZero(positiveSplit.SplitAmt)) {
					continue;
				}
				foreach(PaySplit negativeSplit in listLinkableNegSplits.FindAll(x => x.PayPlanNum==0)) {
					if(CompareDouble.IsLessThanOrEqualToZero(positiveSplit.SplitAmt)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(negativeSplit.SplitAmt)) {
						continue;
					}
					double amountTxfr=Math.Min(Math.Abs(positiveSplit.SplitAmt),Math.Abs(negativeSplit.SplitAmt));
					positiveSplit.SplitAmt-=amountTxfr;
					negativeSplit.SplitAmt+=amountTxfr;
				}
			}
			//Distribute any splits that still have a positive SplitAmt remaining (after negating the negative splits above).
			foreach(PaySplit split in listLinkablePosSplits) {
				if(CompareDouble.IsLessThanOrEqualToZero(split.SplitAmt)) {
					continue;
				}
				foreach(AccountEntry accountEntry in bucket.ListAccountEntries) {
					if(CompareDouble.IsLessThanOrEqualToZero(split.SplitAmt)) {
						break;//Split's amount has been used by previous charges.
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(accountEntry.AmountEnd)) {
						continue;
					}
					if(accountEntry.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
						continue;//we do not implicitly link to TP procedures
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,split.SplitAmt);
					//Manipulate the amounts but do not officially link this split to the accountEntry (via SplitCollection) since it is not explicitly linked.
					accountEntry.AmountEnd-=(decimal)amt;
					split.SplitAmt-=amt;
				}
			}
			//Negative non-procedure adjustments can implicitly link to negative paysplits.
			//Negative non-procedure adjustments are basically bookkeeping errors, courtesy discounts, or some sort of donation to the patient.
			//Negative paysplits are money going from the doctor/office back to the patient for similar reasons (usually done to correct errors).
			//It is completely acceptable to have these donations/corrections offset each other.
			List<AccountEntry> listNegNonPayPlanAdjEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(Adjustment)
				&& x.ProcNum==0
				&& x.PayPlanNum==0
				&& CompareDecimal.IsLessThanZero(x.AmountEnd));
			foreach(PaySplit splitNeg in listLinkableNegSplits.FindAll(x => x.PayPlanNum==0)) {
				if(CompareDecimal.IsGreaterThanOrEqualToZero(splitNeg.SplitAmt)) {
					continue;
				}
				foreach(AccountEntry accountEntryNeg in listNegNonPayPlanAdjEntries) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(splitNeg.SplitAmt)) {
						break;//Split amount has been used by previous charges.
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(accountEntryNeg.AmountEnd)) {
						continue;
					}
					double amt=Math.Max((double)accountEntryNeg.AmountEnd,splitNeg.SplitAmt);
					//Manipulate the amounts but do not officially link this split to the accountEntry (via SplitCollection) since it is not explicitly linked.
					accountEntryNeg.AmountEnd-=(decimal)amt;
					splitNeg.SplitAmt-=amt;
				}
			}
			#endregion
			#endregion
			#region Adjustments
			//Negative non-procedure adjustments need to remove value from positive procedures as accurately as possible.
			List<AccountEntry> listNegAdjEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(Adjustment)
				&& x.ProcNum==0
				&& CompareDecimal.IsLessThanZero(x.AmountEnd));
			List<AccountEntry> listPosProcEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(Procedure)
				&& CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			List<AccountEntry> listAdjProcEntries=new List<AccountEntry>();
			listAdjProcEntries.AddRange(listNegAdjEntries);
			listAdjProcEntries.AddRange(listPosProcEntries);
			BalanceAccountEntries(ref listAdjProcEntries);
			#region Payment Plan Adjustments
			//Negative non-procedure faux account entries (pay plan adjustments) need to remove value from positive faux account entries FIFO style.
			//Only consider ones that are not associated to an unearned type.  Those faux entries are designed for the transfer system, not linking system.
			Dictionary<long,List<AccountEntry>> dictPayPlanNumNegAdjEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry)
					&& ((FauxAccountEntry)x.Tag).IsAdjustment
					&& CompareDecimal.IsLessThanZero(x.AmountEnd)
					&& !x.IsUnearned)
				.GroupBy(x => x.PayPlanNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(long payPlanNum in dictPayPlanNumNegAdjEntries.Keys) {
				List<AccountEntry> listNegAdjFauxEntries=dictPayPlanNumNegAdjEntries[payPlanNum];
				List<AccountEntry> listPosFauxEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry)
					&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
					&& x.PayPlanNum==payPlanNum
					&& !x.IsUnearned);
				List<AccountEntry> listNegAdjPosFauxEntries=new List<AccountEntry>();
				listNegAdjPosFauxEntries.AddRange(listNegAdjFauxEntries);
				listNegAdjPosFauxEntries.AddRange(listPosFauxEntries);
				BalanceAccountEntries(ref listNegAdjPosFauxEntries);
			}
			#endregion
			#endregion
		}
		#endregion

		#region Misc
		///<summary>Distributes available unearned money to the account entries up to the amount passed in.</summary>
		public static List<PaySplit> AllocateUnearned(long payNum,double amountUnearned,List<AccountEntry> listAccountEntries,Family fam=null, bool excludeHiddenUnearned=false) {
			//No need to check MiddleTierRole; no call to db.
			if(CompareDouble.IsLessThanOrEqualToZero(amountUnearned) || listAccountEntries.IsNullOrEmpty()) {
				return new List<PaySplit>();
			}
			if(fam==null) {
				fam=Patients.GetFamily(listAccountEntries.First().PatNum);
			}
			List<PaySplit> listPaySplits=new List<PaySplit>();
			double amountRemaining=amountUnearned;
			//Perform explicit and implicit linking on the entire account and get the actual account entries that make up the current unearned bucket.
			ConstructResults constructResults=ConstructAndLinkChargeCredits(fam.GetPatNums(),fam.Guarantor.PatNum,new List<PaySplit>(),new Payment(),
				listAccountEntries,isIncomeTxfr:true,isAllocateUnearned:true);
			//The account entries passed in may not have had explicit linking executed on them so find the same entries from our results.
			//Allow allocating to account entries that are related by proxy (e.g. payment plan debits that are linked to procedures via credits).
			List<AccountEntry> listAllocateEntries=new List<AccountEntry>();
			for(int i = 0;i<listAccountEntries.Count;i++) {
				if(listAccountEntries[i].ProcNum > 0) {
					listAllocateEntries.AddRange(constructResults.ListAccountEntries.FindAll(x => x.ProcNum==listAccountEntries[i].ProcNum));
				}
				if(listAccountEntries[i].AdjNum > 0) {
					listAllocateEntries.AddRange(constructResults.ListAccountEntries.FindAll(x => x.AdjNum==listAccountEntries[i].AdjNum));
				}
				if(listAccountEntries[i].PayPlanChargeNum > 0) {
					listAllocateEntries.AddRange(constructResults.ListAccountEntries.FindAll(x => x.PayPlanChargeNum==listAccountEntries[i].PayPlanChargeNum));
				}
			}
			//Suggest splits that explicitly take from the unearned bucket first. Any value left over will get transferred from the 0 provider later.
			List<AccountEntry> listUnearnedEntries;
			if(excludeHiddenUnearned) {
				List<long> listDefNumsHiddenUnearnedTypes = new List<long>();
				//get list of unearned types marked as "Do Not Show On Account" so that we can avoid grabbing them for allocation
				Defs.GetCatList((int)DefCat.PaySplitUnearnedType).Where(x => x.ItemValue=="X").ForEach(x => listDefNumsHiddenUnearnedTypes.Add(x.DefNum));
				listUnearnedEntries=constructResults.ListAccountEntries.FindAll(x => x.IsUnearned && CompareDecimal.IsLessThanOrEqualToZero(x.AmountEnd) && !listDefNumsHiddenUnearnedTypes.Contains(x.UnearnedType));
			}
			else {
				listUnearnedEntries=constructResults.ListAccountEntries.FindAll(x => x.IsUnearned && CompareDecimal.IsLessThanOrEqualToZero(x.AmountEnd));
			}
			foreach(AccountEntry accountEntry in listAllocateEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd))) {
				if(CompareDouble.IsLessThanOrEqualToZero(amountRemaining)) {
					break;
				}
				//Prefer to pay account entries off via unearned from the corresponding provider prior to taking FIFO style.
				foreach(AccountEntry unearnedEntry in listUnearnedEntries.OrderByDescending(x => x.ProvNum==accountEntry.ProvNum)) {
					if(CompareDouble.IsLessThanOrEqualToZero(amountRemaining) || CompareDecimal.IsLessThanOrEqualToZero(accountEntry.AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(unearnedEntry.AmountEnd)) {
						continue;
					}
					double amountToAllocate=Math.Min((double)Math.Abs(unearnedEntry.AmountEnd),(double)accountEntry.AmountEnd);
					amountToAllocate=Math.Min(amountToAllocate,amountRemaining);
					//Make a split that will offset a legitimate unearned account entry.
					listPaySplits.Add(CreatePaySplitHelper(unearnedEntry,0-amountToAllocate,DateTime.Today,payNum:payNum,unearnedType:unearnedEntry.UnearnedType));
					//Blindly apply the amount that was taken from unearned to one of the account entries that the user selected.
					listPaySplits.Add(CreatePaySplitHelper(accountEntry,amountToAllocate,DateTime.Today,payNum:payNum));
					unearnedEntry.AmountEnd+=(decimal)amountToAllocate;
					accountEntry.AmountEnd-=(decimal)amountToAllocate;
					amountRemaining-=amountToAllocate;
				}
			}
			//Get the default unearned types and make as many splits as necessary in order to move amountRemaining from the 0 provider.
			long unearnedTypePrepayment=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
			long unearnedTypeTP=PrefC.GetLong(PrefName.TpUnearnedType);
			foreach(AccountEntry accountEntry in listAllocateEntries) {
				if(CompareDouble.IsLessThanOrEqualToZero(amountRemaining)) {
					break;
				}
				if(CompareDecimal.IsLessThanOrEqualToZero(accountEntry.AmountEnd)) {
					continue;
				}
				long unearnedType=unearnedTypePrepayment;
				if(accountEntry.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
					unearnedType=unearnedTypeTP;
				}
				double amountToAllocate=Math.Min(amountRemaining,(double)accountEntry.AmountEnd);
				//Always take from the default unearned payment type and the 0 / 'None' provider. The income transfer system will correct this later.
				//They simply want to see a singlular negative entry (or as few as possible) and an offsetting positive to wherever they chose.
				listPaySplits.Add(new PaySplit() {
					AdjNum=0,
					ClinicNum=0,
					DatePay=DateTime.Today,
					PatNum=accountEntry.PatNum,
					PayPlanNum=0,
					PayNum=payNum,
					ProcNum=0,
					ProvNum=0,
					SplitAmt=0-amountToAllocate,
					UnearnedType=unearnedType,
				});
				//Blindly apply the amount that was taken from unearned to one of the account entries that the user selected.
				listPaySplits.Add(CreatePaySplitHelper(accountEntry,amountToAllocate,DateTime.Today,payNum: payNum));
				amountRemaining-=amountToAllocate;
				accountEntry.AmountEnd-=(decimal)amountToAllocate;
			}
			return listPaySplits;
		}

		///<summary>Makes a payment from a passed in list of charges.</summary>
		public static PayResults MakePayment(List<List<AccountEntry>> listSelectedCharges,Payment payCur,decimal textAmount,
			List<AccountEntry> listAllCharges)
		{
			//No remoting role check; no call to db.
			PayResults splitData=null;
			List<PaySplit> listPaySplits=new List<PaySplit>();
			bool isPayAmtZeroUponEntering=CompareDouble.IsZero(payCur.PayAmt);
			foreach(List<AccountEntry> listCharges in listSelectedCharges) {
				if(!isPayAmtZeroUponEntering && CompareDouble.IsZero(payCur.PayAmt)) {
					break;
				}
				foreach(AccountEntry charge in listCharges.FindAll(x => !CompareDecimal.IsZero(x.AmountEnd))) {
					decimal splitAmt=(isPayAmtZeroUponEntering ? charge.AmountEnd : (decimal)payCur.PayAmt);
					if(!isPayAmtZeroUponEntering && CompareDecimal.IsLessThanOrEqualToZero(splitAmt)) {
						break;
					}
					splitData=CreatePaySplit(charge,splitAmt,payCur,textAmount,listAllCharges);
					listPaySplits.AddRange(splitData.ListSplitsCur);
					listAllCharges=splitData.ListAccountCharges;
				}
			}
			if(splitData==null) {
				splitData=new PayResults { ListAccountCharges=listAllCharges,ListSplitsCur=listPaySplits };
			}
			else {
				splitData.ListSplitsCur=listPaySplits;
			}
			return splitData;
		}

		public static PayResults CreatePaySplit(AccountEntry charge,decimal payAmt,Payment payCur,decimal textAmount,List<AccountEntry> listCharges,
			bool isManual=false)
		{
			//No remoting role check; no call to db.
			PayResults createdSplit=new PayResults();
			createdSplit.ListSplitsCur=new List<PaySplit>();
			createdSplit.ListAccountCharges=listCharges;
			createdSplit.Payment=payCur;
			double amount;
			if(!isManual && (Math.Abs(charge.AmountEnd)<Math.Abs(payAmt) || textAmount==0)) {
				//Not a manual charge and user wants to make a split for the full charge amount.
				amount=(double)charge.AmountEnd;
				charge.AmountEnd=0;
			}
			else {//Either a manual charge or a partial payment.
				amount=(double)payAmt;
				charge.AmountEnd-=payAmt;
			}
			long unearnedType = charge.UnearnedType;
			if(charge.GetType()==typeof(Procedure) && ((Procedure)charge.Tag).ProcStatus==ProcStat.TP) {
				unearnedType=PrefC.GetLong(PrefName.TpUnearnedType);
			}
			PaySplit split=CreatePaySplitHelper(charge,amount,DateTime.Today,payNum:payCur.PayNum,unearnedType:unearnedType);
			//PaySplits for TP procedures should always set the UnearnedType to the TpUnearnedType preference.
			payCur.PayAmt-=split.SplitAmt;
			charge.SplitCollection.Add(split);
			createdSplit.ListSplitsCur.Add(split);
			createdSplit.Payment=payCur;
			return createdSplit;
		}

		///<summary>Checks if the amtEntered will result in the AccountEntry.Tag procedure to be overpaid. Returns false if AccountEntry.Tag is not a procedure.</summary>
		public static bool IsProcOverPaid(decimal amtEntered,AccountEntry accountEntry) {
			if(accountEntry.GetType()!=typeof(Procedure)) {
				return false;//AccountEntry is not a procedure. Return false.
			}
			//Only look for explicitly linked paysplits when calculating amount remaining. 
			decimal amtOverpay=accountEntry.AmountOriginal-(decimal)accountEntry.SplitCollection.Sum(x=>x.SplitAmt)-amtEntered;
			return CompareDecimal.IsLessThanZero(amtOverpay);
		}

		/// <summary>
		/// Returns a new paysplit that is explicitly linked to the AccountEntry for the amount passed in. This helper method is designed to centralize core logic for creating paysplits associated to account entries. E.g. PatNum will be set correctly when the entry passed in is associated to a payment plan.
		/// </summary>
		public static PaySplit CreatePaySplitHelper(AccountEntry entry,double splitAmt,DateTime datePay,long payNum = 0,long unearnedType = 0,bool isNew = false) {
			PaySplit split=new PaySplit();
			//set baseline values first
			split.IsNew=isNew;
			//Payment splits should not be associated to an adjustment and a procedure at the same time.
			if(entry.GetType()==typeof(Adjustment)) {
				split.AdjNum=entry.AdjNum;
				split.ProcNum=0;
			}
			else if(entry.GetType()==typeof(Procedure)) {
				split.AdjNum=0;
				split.ProcNum=entry.ProcNum;
			}
			else {//Payment plan entry or something else, just preserve the values set on the AccountEntry passed in.
				split.AdjNum=entry.AdjNum;
				split.ProcNum=entry.ProcNum;
			}
			split.ClinicNum=entry.ClinicNum;
			split.DatePay=datePay;
			split.PatNum=entry.PatNum;
			split.PayPlanChargeNum=entry.PayPlanChargeNum;
			split.PayPlanDebitType=entry.PayPlanDebitType;
			split.PayPlanNum=entry.PayPlanNum;
			split.PayNum=payNum;
			split.ProvNum=entry.ProvNum;
			split.SplitAmt=splitAmt;
			split.UnearnedType=unearnedType;
			//if entry is for a payplan charge (FauxAccountEntry) and we are not using PayPlanVersion.NoCharge, set the paysplit patnum to the payplancharges Guarantor.
			if(entry is FauxAccountEntry payPlanEntry && PrefC.GetEnum<PayPlanVersions>(PrefName.PayPlansVersion)!=PayPlanVersions.NoCharges) {
				split.PatNum=payPlanEntry.Guarantor;
			}
			return split;
		}
		#endregion

		#region AutoSplit
		public static AutoSplit AutoSplitForPayment(long patNum,Payment payment,bool isIncomeTxfr=false,bool isPatPrefer=false,long payPlanNum=0,
			List<AccountEntry> listAccountEntriesPayFirst=null,ConstructChargesData constructChargesData=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			return AutoSplitForPayment(patNum,
				Patients.GetFamily(patNum).GetPatNums(),
				new List<PaySplit>(),
				payment,
				listAccountEntriesPayFirst,
				isIncomeTxfr,
				isPatPrefer,
				constructChargesData,
				payPlanNum:payPlanNum);
		}

		///<summary>Leave loadData blank for doRefreshData to be true and get a new copy of the objects.</summary>
		public static AutoSplit AutoSplitForPayment(long patCurNum,List<long> listPatNums,List<PaySplit> listPaySplitsForPayment,Payment payment,
			List<AccountEntry> listAccountEntriesPayFirst,bool isIncomeTxfr,bool isPatPrefer,ConstructChargesData constructChargesData=null,bool doAutoSplit=true,
			bool doIncludeExplicitCreditsOnly=false,long payPlanNum=0)
		{
			ConstructResults constructResults=ConstructAndLinkChargeCredits(patCurNum,listPatNums,listPaySplitsForPayment,payment?.PayNum??0,listAccountEntriesPayFirst,isIncomeTxfr:isIncomeTxfr,
				isPreferCurPat:isPatPrefer,constructChargesData:constructChargesData,doIncludeExplicitCreditsOnly:doIncludeExplicitCreditsOnly,false,DateTime.MinValue,false,
				payment?.ClinicNum??0,payment?.PayAmt??0,payment?.PayDate??DateTime.MinValue,false,true);
			AutoSplit autoSplit=AutoSplitForPayment(constructResults,doAutoSplit,payPlanNum:payPlanNum,listAccountEntriesPayFirst:listAccountEntriesPayFirst);
			return autoSplit;
		}

		public static AutoSplit AutoSplitForPayment(ConstructResults constructResults,bool doAutoSplit=true,long payPlanNum=0,List<AccountEntry> listAccountEntriesPayFirst=null,
			int rigorousAccounting=-1)
		{
			AutoSplit autoSplitData=new AutoSplit(constructResults);
			if(rigorousAccounting==-1) {
				rigorousAccounting=PrefC.GetInt(PrefName.RigorousAccounting);
			}
			if(!doAutoSplit || rigorousAccounting==(int)RigorousAccounting.DontEnforce) {
				return autoSplitData;
			}
			//Get a subset of the account charges that can have value auto split to them.
			List<AccountEntry> listAutoSplitAccountEntries=autoSplitData.ListAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			//Never auto split to treatment planned entries
			listAutoSplitAccountEntries.RemoveAll(x => x.GetType()==typeof(Procedure) && ((Procedure)x.Tag).ProcStatus==ProcStat.TP);
			//Patient payment plans can have credits attached to treatment planned procedures, ignore those as well.
			listAutoSplitAccountEntries.RemoveAll(x => x.GetType()==typeof(FauxAccountEntry)
				&& ((FauxAccountEntry)x.Tag).AccountEntryProc!=null
				&& ((FauxAccountEntry)x.Tag).AccountEntryProc.GetType()==typeof(Procedure)
				&& ((Procedure)((FauxAccountEntry)x.Tag).AccountEntryProc.Tag).ProcStatus==ProcStat.TP);
			if(payPlanNum > 0) {
				if(PrefC.IsODHQ) {
					listAutoSplitAccountEntries=listAutoSplitAccountEntries.OrderByDescending(x => x.PayPlanNum==payPlanNum).ToList();
				}
				else {
					listAutoSplitAccountEntries.RemoveAll(x => x.PayPlanNum!=payPlanNum);
				}
			}
			if(!listAccountEntriesPayFirst.IsNullOrEmpty()) {
				//Shove all of the selected account entries to the top of listAutoSplitAccountEntries so that they are paid first.
				listAutoSplitAccountEntries=listAutoSplitAccountEntries
					.OrderByDescending(x => listAccountEntriesPayFirst.Any(y => y.PriKey==x.PriKey && y.GetType()==x.GetType()))
					.ThenByDescending(x => listAccountEntriesPayFirst.Any(y => y.PayPlanNum > 0 && y.PayPlanNum==x.PayPlanNum))
					.ToList();
			}
			//Create a variable to keep track of the money that can be allocated for this payment.
			double amtToAllocate=(autoSplitData.PayAmt - autoSplitData.ListPaySplitsSuggested.Sum(x => x.SplitAmt));
			//Create as many auto splits as possible for account entries with positive AmountEnd values.
			foreach(AccountEntry charge in listAutoSplitAccountEntries) {
				if(CompareDouble.IsZero(amtToAllocate)) {
					break;//No more value to allocate.
				}
				if(CompareDouble.IsLessThanZero(amtToAllocate)) {
					return autoSplitData;//Negative payments should not make any auto splits so return here.
				}
				if(CompareDecimal.IsLessThanOrEqualToZero(charge.AmountEnd)) {
					continue;
				}
				double splitAmt=Math.Min(amtToAllocate,(double)charge.AmountEnd);
				//Make a new split that will apply as much value as possible from the account entry.
				PaySplit split=CreatePaySplitHelper(charge,splitAmt,autoSplitData.PayDate,payNum:autoSplitData.PayNum,isNew:true);
				//Remove the value from the account entry
				charge.AmountEnd-=(decimal)splitAmt;
				amtToAllocate-=splitAmt;
				charge.SplitCollection.Add(split);
				autoSplitData.ListPaySplitsSuggested.Add(split);
			}
			//Create an unearned split if there is any remaining money to allocate.
			//this is a special case, creating a paysplit without an account entry object tied to it. This is why it is not using CreatePaySplitHelper(). 
			if(!CompareDouble.IsZero(amtToAllocate)) {
				PaySplit split=new PaySplit();
				split.SplitAmt=amtToAllocate;
				amtToAllocate=0;
				split.DatePay=autoSplitData.PayDate;
				split.PatNum=autoSplitData.PatNum;
				split.ProvNum=(PrefC.IsODHQ) ? 7 : 0;//Jordan's ProvNum for HQ.
				split.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
				if(PrefC.HasClinicsEnabled) {
					split.ClinicNum=autoSplitData.ClinicNum;
				}
				split.PayNum=autoSplitData.PayNum;
				autoSplitData.ListPaySplitsSuggested.Add(split);
			}
			return autoSplitData;
		}

		///<summary>Recreates every payment in the database by replacing all payment splits with new payment splits suggested by the auto split system.</summary>
		public static void ReAutoSplitAllPaymentsForFamily(long patNum,DateTime dateAsOf=default) {
			//Keep track of a list of new payment splits that will replace all of the splits that are currently in the database.
			List<PaySplit> listPaySplitsSuggested=new List<PaySplit>();
			//Keep track of offsetting negative payments separately. These splits will be added to the generic list of suggested splits after auto-split logic is ran.
			List<PaySplit> listPaySplitsNegativeOffset=new List<PaySplit>();
			//Keep track of a list of PaySplitNums to delete.
			List<long> listSplitNumsToDelete=new List<long>();
			//Get all of the account data from the database so that we can invoke auto split logic with all of the required information.
			Family family=Patients.GetFamily(patNum);
			ConstructChargesData constructChargesData=GetConstructChargesData(patNum,listPatNums:family.GetPatNums());
			//Ignore any payment splits that fall after dateAsOf.
			if(dateAsOf.Year > 1880) {
				constructChargesData.ListPaySplits.RemoveAll(x => x.DatePay > dateAsOf);
			}
			//Keep track of all of the payment splits for the family that are currently in the database.
			List<PaySplit> listPaySplitsForFamily=new List<PaySplit>(constructChargesData.ListPaySplits);
			//Get the actual payment objects from the database.
			List<Payment> listPayments=Payments.GetPayments(listPaySplitsForFamily.Select(x => x.PayNum).Distinct().ToList());
			//Clear out the list of splits from the constructChargesData object to act like there are no payment splits in the database at this time (we will be creating new ones).
			constructChargesData.ListPaySplits.Clear();
			//Clear out the list of payment plan splits just to be safe. This list is only used within GetConstructChargesData() but it's better to play it safe.
			constructChargesData.ListPayPlanSplits.Clear();
			//Group up the payment splits by PayNum.
			List<PayNumPaySplitsGroup> listPayNumPaySplitsGroups=listPaySplitsForFamily.GroupBy(x => x.PayNum)
				.ToDictionary(x => x.Key,x => x.ToList())
				.Select(x => new PayNumPaySplitsGroup(payNum:x.Key,payment:listPayments.FirstOrDefault(y => y.PayNum==x.Key),listPaySplits:x.Value))
				.ToList();
			if(listPayNumPaySplitsGroups.Any(x => x.Payment==null)) {
				throw new ODException("Payment splits associated to an invalid payment detected. Run Database Maintenance before recreating payment splits for the family.");
			}
			List<Def> listDefsForPaymentTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes);
			#region Negative Payments
			List<PayNumPaySplitsGroup> listPayNumPaySplitsGroupsNegative=listPayNumPaySplitsGroups.FindAll(x => CompareDouble.IsLessThanZero(x.PayAmount));
			for(int i=0;i<listPayNumPaySplitsGroupsNegative.Count;i++) {
				//Look for offsetting entities that most likely offset this negative payment (PayAmounts must exactly match and must be dated on or before the negative payment).
				//Consider negative adjustments first.
				Adjustment adjustment=constructChargesData.ListAdjustments.FirstOrDefault(x => CompareDouble.IsEqual(x.AdjAmt,listPayNumPaySplitsGroupsNegative[i].PayAmount));
				if(adjustment!=null) {
					//Suggest a split from the payment directly to the adjustment.
					listPaySplitsNegativeOffset.Add(CreatePaySplitHelper(new AccountEntry(adjustment),
						listPayNumPaySplitsGroupsNegative[i].PayAmount,
						listPayNumPaySplitsGroupsNegative[i].Payment.PayDate,
						payNum:listPayNumPaySplitsGroupsNegative[i].Payment.PayNum));
					//Add to the list of PaySplitNums that need to be deleted from the database because they are going to get replaced by the offsetting split above.
					listSplitNumsToDelete.AddRange(listPayNumPaySplitsGroupsNegative[i].ListPaySplits.Select(x => x.SplitNum));
					//The payment and negative adjustment have been taken care of so remove them from their respective lists.
					listPayNumPaySplitsGroups.RemoveAll(x => x.PayNum.In(listPayNumPaySplitsGroupsNegative[i].PayNum));
					constructChargesData.ListAdjustments.Remove(adjustment);
					continue;
				}
				//Consider positive payments second. 
				PayNumPaySplitsGroup payNumPaySplitsGroupOffset=listPayNumPaySplitsGroups
					.Where(x => x.PayNum!=listPayNumPaySplitsGroupsNegative[i].PayNum && x.PayAmount==Math.Abs(listPayNumPaySplitsGroupsNegative[i].PayAmount))
					.OrderByDescending(x => x.Payment.PayDate<=listPayNumPaySplitsGroupsNegative[i].Payment.PayDate)
					.ThenByDescending(x => x.Payment.PatNum==listPayNumPaySplitsGroupsNegative[i].Payment.PatNum)
					.ThenByDescending(x => x.Payment.ClinicNum==listPayNumPaySplitsGroupsNegative[i].Payment.ClinicNum)
					.FirstOrDefault();
				if(payNumPaySplitsGroupOffset!=null) {
					//Recreate the payment splits for each payment so that they truly offset each other and are not associated to other production.
					//The fact that we erase whatever production these payments may have been associated to may seem strange to the user.
					//Especially if the payments have notes on them and we just associated two payments together that have nothing to do with each other.
					//This is fine since there probably isn't an easy way to programmatically guess which payments should (or even if any are) associated together.
					//This method is being ran to completely rewrite history so this is the history we are going with. These positive and negative payments offset each other.
					listPaySplitsNegativeOffset.Add(new PaySplit() { 
						AdjNum=0,
						ClinicNum=0,
						DatePay=payNumPaySplitsGroupOffset.Payment.PayDate,
						PatNum=payNumPaySplitsGroupOffset.Payment.PatNum,
						PayNum=payNumPaySplitsGroupOffset.Payment.PayNum,
						PayPlanChargeNum=0,
						PayPlanNum=0,
						ProcNum=0,
						ProvNum=0,
						SplitAmt=payNumPaySplitsGroupOffset.PayAmount,
						UnearnedType=0
					});
					listPaySplitsNegativeOffset.Add(new PaySplit() {
						AdjNum=0,
						ClinicNum=0,
						DatePay=listPayNumPaySplitsGroupsNegative[i].Payment.PayDate,
						PatNum=listPayNumPaySplitsGroupsNegative[i].Payment.PatNum,
						PayNum=listPayNumPaySplitsGroupsNegative[i].Payment.PayNum,
						PayPlanChargeNum=0,
						PayPlanNum=0,
						ProcNum=0,
						ProvNum=0,
						SplitAmt=listPayNumPaySplitsGroupsNegative[i].PayAmount,
						UnearnedType=0
					});
					//Add to the list of PaySplitNums that need to be deleted from the database because they are going to get replaced by the offsetting splits above.
					listSplitNumsToDelete.AddRange(payNumPaySplitsGroupOffset.ListPaySplits.Select(x => x.SplitNum));
					listSplitNumsToDelete.AddRange(listPayNumPaySplitsGroupsNegative[i].ListPaySplits.Select(x => x.SplitNum));
					//Remove the payments from the list of splits that need to go through the ReAuto-Split region.
					listPayNumPaySplitsGroups.RemoveAll(x => x.PayNum.In(listPayNumPaySplitsGroupsNegative[i].PayNum,payNumPaySplitsGroupOffset.PayNum));
					continue;
				}
				throw new ODException($"Negative payment without an offsetting entity detected.  See PayNum {listPayNumPaySplitsGroupsNegative[i].PayNum}");
			}
			#endregion
			#region ReAuto-Split
			//Loop through each payment and execute auto split logic as if no payment splits exist in the database (other than the ones we have previously suggested).
			for(int i=0;i<listPayNumPaySplitsGroups.Count;i++) {
				if(listPayNumPaySplitsGroups[i].ListPaySplits.Any(x => !family.GetPatNums().Contains(x.PatNum))) {
					throw new ODException($"There are payment splits associated to patients outside of the family. See PayNum {listPayNumPaySplitsGroups[i].PayNum}");
				}
				if(CompareDouble.IsLessThanOrEqualToZero(listPayNumPaySplitsGroups[i].PayAmount)) {
					//Don't do anything with income transfers or negative payments (which should have been handled above.
					//Users should delete income transfers if they don't want them around or the conversions department tech should have deleted them.
					continue;
				}
				//Manipulate a ConstructData object that was instantiated prior to this loop and simply override the list of payment splits as desired.
				constructChargesData.ListPaySplits=listPaySplitsSuggested;
				//Create a ConstructResults with the newly updated list of payment splits.
				ConstructResults constructResults=GetConstructResults(constructChargesData,patNum,family.GetPatNums(),listPayNumPaySplitsGroups[i].Payment.PayNum,false,
					listPayNumPaySplitsGroups[i].Payment.ClinicNum,listPayNumPaySplitsGroups[i].PayAmount,listPayNumPaySplitsGroups[i].Payment.PayDate,dateAsOf);
				//Execute explicit and implicit linking logic so that the Account Entries have correct AmountEnd values.
				ExplicitAndImplicitLinkingForConstructResults(ref constructResults,false,new List<PaySplit>(),constructChargesData,false,false,new List<AccountEntry>(),patNum,false,
					listPayNumPaySplitsGroups[i].PayNum,false,true);
				//Execute auto-split logic for the amount of the current payment and act like the office has EnforceFully mode enabled so that everything is perfectly linked.
				AutoSplit autoSplit=AutoSplitForPayment(constructResults,rigorousAccounting:(int)RigorousAccounting.EnforceFully);
				if(autoSplit.ListPaySplitsSuggested.IsNullOrEmpty()) {
					throw new ODException($"AutoSplitForPayment did not suggest any payment splits. See PayNum {listPayNumPaySplitsGroups[i].Payment.PayNum}");
				}
				//Make sure that the splits suggested equate to the payment amount otherwise fail out.
				double paymentAmountSuggested=autoSplit.ListPaySplitsSuggested.Sum(x => x.SplitAmt);
				if(!CompareDouble.IsEqual(listPayNumPaySplitsGroups[i].PayAmount,paymentAmountSuggested)) {
					//Any payments that cannot be recreated with 100% accuracy should cause the entire process to fail since this is such a dangerous method.
					throw new ODException($"Suggested payment amount does not match original payment amount. See PayNum {listPayNumPaySplitsGroups[i].Payment.PayNum}"
						+$"\r\npaymentAmountSuggested: {paymentAmountSuggested:C}"
						+$"\r\npaymentAmount: {listPayNumPaySplitsGroups[i].PayAmount:C}");
				}
				//Add to the list of payment splits for future auto split logic and for eventual insertion into the database.
				listPaySplitsSuggested.AddRange(autoSplit.ListPaySplitsSuggested);
				//Add to the list of PaySplitNums that need to be deleted from the database because they are going to get replaced by the list of new auto splits.
				listSplitNumsToDelete.AddRange(listPayNumPaySplitsGroups[i].ListPaySplits.Select(x => x.SplitNum));
			}
			#endregion
			//Add any negative offsetting payment splits to the generic list of suggested splits.
			if(!listPaySplitsNegativeOffset.IsNullOrEmpty()) {
				listPaySplitsSuggested.AddRange(listPaySplitsNegativeOffset);
			}
			//Return if there is nothing to do.
			if(listPaySplitsSuggested.IsNullOrEmpty()) {
				return;
			}
			//Make sure that the family account doesn't have negative unearned. Fail the family if there is negative unearned (even if it is legitimate).
			double amountUnearned=listPaySplitsSuggested.Where(x => x.UnearnedType > 0).Sum(x => x.SplitAmt);
			if(CompareDouble.IsLessThanZero(amountUnearned)) {
				throw new ODException($"Suggested payment splits would cause unearned to be negative.");
			}
			#region Delete and Insert Splits
			//It is safe to delete old payment splits since they are not linked to anything important and instead the Payment object is the entity that is linked to things like deposits.
			//Another reason that this should be safe is that we are technically recreating the payment splits but simply pointing them at different production entities.
			//Delete all of the payment splits that we are replacing.
			PaySplits.DeleteMany(listSplitNumsToDelete.ToArray());
			//Insert all of the new explicitly linked payment splits.
			PaySplits.InsertMany(listPaySplitsSuggested);
			//Make a single security log that indicates every single payment for the entire family has been rectreated.
			SecurityLogs.MakeLogEntry(EnumPermType.PaymentEdit,patNum,Lans.g("FormFamilyBalancer","Every single payment split was recreated for this family by the Family Balancer."));
			#endregion
		}
		#endregion

		#region AccountEntry
		///<summary>Sorts similar to account module (AccountModules.cs, AccountLineComparer).Groups procedures together, then Adjustments, then everything else.</summary>
		private static int AccountEntrySort(AccountEntry x,AccountEntry y) {
			if(x.Date==y.Date) {
				if(x.GetType()==typeof(Procedure) && y.GetType()!=typeof(Procedure)) {
					return -1;
				}
				if(x.GetType()!=typeof(Procedure) && y.GetType()==typeof(Procedure)) {
					return 1;
				}
				if(x.GetType()==typeof(Adjustment) && y.GetType()!=typeof(Adjustment)) {
					return -1;
				}
				if(x.GetType()!=typeof(Adjustment) && y.GetType()==typeof(Adjustment)) {
					return 1;
				}
				if(x.GetType()==typeof(FauxAccountEntry) && y.GetType()==typeof(FauxAccountEntry)) {
					//PayPlanCharge entries should prefer to show interest charges above principal (we suggest paying interest first when auto-splitting).
					if(CompareDecimal.IsGreaterThanZero(((FauxAccountEntry)x).Interest) && CompareDecimal.IsZero(((FauxAccountEntry)y).Interest)) {
						return -1;
					}
					if(CompareDecimal.IsZero(((FauxAccountEntry)x).Interest) && CompareDecimal.IsGreaterThanZero(((FauxAccountEntry)y).Interest)) {
						return 1;
					}
					//If both have interest set then order predictably every time.
					if(CompareDecimal.IsGreaterThanZero(((FauxAccountEntry)x).Interest) && CompareDecimal.IsGreaterThanZero(((FauxAccountEntry)y).Interest)) {
						return ((FauxAccountEntry)x).Interest.CompareTo(((FauxAccountEntry)y).Interest);
					}
					//Simply sort by principal due if no interest is being used.
					return ((FauxAccountEntry)x).Principal.CompareTo(((FauxAccountEntry)y).Principal);
				}
			}
			return x.Date.CompareTo(y.Date);
		}

		public static List<Procedure> GetProcsForAccountEntries(List<AccountEntry> listAccountEntry) {
			List<Procedure> listProcs=new List<Procedure>();
			List<AccountEntry> listAccountEntryProcs=listAccountEntry.FindAll(x => x.GetType()==typeof(Procedure));
			foreach(AccountEntry entry in listAccountEntryProcs) {
				listProcs.Add((Procedure)entry.Tag);
			}
			return listProcs;
		}

		public static List<AccountEntry> CreateAccountEntries(List<Procedure> listProcs) {
			//No remoting role check; no call to db
			List<AccountEntry> listAccountEntries=new List<AccountEntry>();
			foreach(Procedure proc in listProcs) {
				listAccountEntries.Add(new AccountEntry(proc));
			}
			return listAccountEntries;
		}
		#endregion

		#region Income Transfer

		///<summary>Attempts to delete income transfer payments for the PatNums passed in. 
		///Payments with hidden unearned or splits associated to patients not present within listPatNums will not be deleted.
		///Set isPayTypeIgnored to true to ignore payment.PayType and instead delete all payments that sum to $0.
		///Set isPayTypeIgnored to false to only delete payments where the PayType is 0.</summary>
		public static void DeleteTransfersForFamily(List<long> listPatNums,bool isPayTypeIgnored=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPatNums,isPayTypeIgnored);
				return;
			}
			//Get all income transfer payments for the family.
			List<long> listPayNumsToDelete=Payments.GetPayNumsForTransfers(isPayTypeIgnored,listPatNums.ToArray());
			if(listPayNumsToDelete.IsNullOrEmpty()) {
				return;//No transfers to even consider deleting.
			}
			string command;
			List<long> listPreservePayNums=new List<long>();
			List<long> listHiddenUnearnedPayTypes=PaySplits.GetHiddenUnearnedDefNums();
			if(listHiddenUnearnedPayTypes.Count > 0) {
				//Some income transfers may have splits that are associated to a hidden type. These transfers must not be deleted (via TaskNum 2806662).
				command=$@"SELECT DISTINCT PayNum FROM paysplit
				WHERE PayNum IN({string.Join(",",listPayNumsToDelete.Select(x => POut.Long(x)))})
				AND UnearnedType IN({string.Join(",",listHiddenUnearnedPayTypes.Select(x => POut.Long(x)))})";
				listPreservePayNums.AddRange(Db.GetListLong(command));
			}
			//Some income transfers may have splits that are associated to patients in different families. These transfers must not be deleted.
			command=$@"SELECT DISTINCT PayNum FROM paysplit
				WHERE PayNum IN({string.Join(",",listPayNumsToDelete.Select(x => POut.Long(x)))})
				AND PatNum NOT IN({string.Join(",",listPatNums.Select(x => POut.Long(x)))})";
			listPreservePayNums.AddRange(Db.GetListLong(command));
			//Remove any income transfers that need to be preserved from our list of payments to delete.
			listPayNumsToDelete.RemoveAll(x => listPreservePayNums.Contains(x));
			//Delete all income transfers that are left in the list of transfers that are 'safe' to delete.
			for(int i=0;i<listPayNumsToDelete.Count;i++) {
				//Some income transfers will not be able to be deleted. Do not let one failure spoil the entire batch.
				//Users will be able to manually try and delete these income transfers and will get a warning message as to why it can't be deleted.
				ODException.SwallowAnyException(() => Payments.Delete(listPayNumsToDelete[i]));
			}
		}

		///<summary>Throws exceptions.</summary>
		public static ClaimTransferResult TransferClaimsPayAsTotal(long patNum,List<long> listFamPatNums,string logText) {
			bool didFix=ClaimProcs.FixClaimsNoProcedures(listFamPatNums);
			if(didFix && !ProcedureCodes.GetContainsKey("ZZZFIX")) {
				Cache.Refresh(InvalidType.ProcCodes);//Refresh local cache only because middle tier has already inserted the signal.
			}
			ClaimTransferResult claimTransferResult=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamPatNums);
			if(claimTransferResult!=null && claimTransferResult.ListClaimProcsInserted.Count > 0) {//valid and items were created
				SecurityLogs.MakeLogEntry(EnumPermType.ClaimProcReceivedEdit,patNum,logText);
			}
			return claimTransferResult;
		}

		///<summary></summary>
		public static bool TryCreateIncomeTransfer(List<AccountEntry> listAllAccountEntries,DateTime datePay,out IncomeTransferData incomeTransferData,
			List<PayPlan> listPayPlans=null)
		{
			//No remoting role check; no call to db
			incomeTransferData=new IncomeTransferData();
			if(listAllAccountEntries.IsNullOrEmpty()) {
				return true;
			}
			if(listPayPlans==null) {
				Family famCur=Patients.GetFamily(listAllAccountEntries.First().PatNum);
				listPayPlans=PayPlans.GetForPats(famCur.GetPatNums(),famCur.Guarantor.Guarantor);
			}
			List<PayPlan> listPayPlansPatient=listPayPlans.FindAll(x => !x.IsDynamic && x.PlanNum==0);
			List<PayPlan> listPayPlansDynamic=listPayPlans.FindAll(x => x.IsDynamic && x.PlanNum==0);
			if(!listPayPlansPatient.IsNullOrEmpty()) {
				//PayPlanCharge Credits are not made when the PaymentPlanVersion is set to NoCharges.
				//For now, do not allow income transfers to be made when the version is set to NoCharges because we don't know what has value to transfer.
				if(PrefC.GetEnum<PayPlanVersions>(PrefName.PayPlansVersion)==PayPlanVersions.NoCharges) {
					incomeTransferData.StringBuilderErrors.AppendLine(Lans.g("PaymentEdit","Transfers cannot be made while 'Pay Plan charge logic' is set to "+
						$"'{PayPlanVersions.NoCharges.GetDescription()}'."));
					return false;
				}
				//Do not allow transfers if there is a payment plan associated to the family that is for an amount that does not equal the Tx amount.
				//E.g. A "Total Tx Amt" not equal to the "Total Amount" means the user is using a patient payment plan and didn't attach Tx Credits.
				//This is a requirement for the transfer system because it needs to know what to take value from and what to give it to (pat/prov/clinic).
				List<long> listInvalidTotalPayPlanNums=new List<long>();
				Dictionary<long,List<PayPlanCharge>> dictPayPlanCharges=PayPlanCharges.GetForPayPlans(listPayPlansPatient.Select(x => x.PayPlanNum).ToList())
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				foreach(long payPlanNum in dictPayPlanCharges.Keys) {
					//The total principal of all credits must equate to the total Principal of all debits.
					double txTotalAmt=PayPlans.GetTxTotalAmt(dictPayPlanCharges[payPlanNum]);//credits
					double totalCost=PayPlans.GetTotalPrinc(payPlanNum,dictPayPlanCharges[payPlanNum]);//debits
					if(!CompareDouble.IsEqual(txTotalAmt,totalCost)) {
						listInvalidTotalPayPlanNums.Add(payPlanNum);
					}
				}
				if(listInvalidTotalPayPlanNums.Count > 0) {
					incomeTransferData.StringBuilderErrors.AppendLine(Lans.g("PaymentEdit","Transfers cannot be made for this family at this time."));
					string errorMsgStart=Lans.g("PaymentEdit","The following payment plans have a 'Total Tx Amt' that does not match the 'Total Amount':");
					List<PayPlan> listInvalidPayPlans=listPayPlansPatient.FindAll(x => listInvalidTotalPayPlanNums.Contains(x.PayPlanNum));
					incomeTransferData.StringBuilderErrors.AppendLine(GetInvalidPayPlanDescription(errorMsgStart,listInvalidPayPlans,dictPayPlanCharges));
					return false;
				}
			}
			if(!listPayPlansDynamic.IsNullOrEmpty()) {
				//Do not allow income transfers when there is negative production associated to a dynamic payment plan.
				List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(listPayPlansDynamic.Select(x => x.PayPlanNum).ToList());
				for(int i=0;i<listPayPlansDynamic.Count;i++) {
					List<PayPlanLink> listPayPlanLinksForPlan=listPayPlanLinks.FindAll(x => x.PayPlanNum==listPayPlansDynamic[i].PayPlanNum);
					List<PayPlanProductionEntry> listPayPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinksForPlan);
					if(listPayPlanLinksForPlan.IsNullOrEmpty() || listPayPlanProductionEntries.IsNullOrEmpty()) {
						incomeTransferData.StringBuilderErrors.AppendLine(
							Lans.g("PaymentEdit","Transfers cannot be made for this family due to a payment plan with no production attached."));
						return false;
					}
					if(listPayPlanProductionEntries.Any(x => CompareDecimal.IsLessThanZero(x.AmountOriginal))) {
						incomeTransferData.StringBuilderErrors.AppendLine(
							Lans.g("PaymentEdit","Transfers cannot be made for this family due to a payment plan with negative production attached."));
						return false;
					}
					List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(listPayPlansDynamic[i].PayPlanNum);
					var dictProductionPayPlanCharges=listPayPlanCharges.GroupBy(x => new { x.FKey,x.LinkType })
						.ToDictionary(x => x.Key,x => x.ToList());
					foreach(var kvp in dictProductionPayPlanCharges) {
						PayPlanProductionEntry payPlanProductionEntry=listPayPlanProductionEntries.FirstOrDefault(x => x.LinkType==kvp.Key.LinkType && x.PriKey==kvp.Key.FKey);
						if(payPlanProductionEntry==null) {
							incomeTransferData.StringBuilderErrors.AppendLine(
								Lans.g("PaymentEdit","Transfers cannot be made for this family due to a payment plan with a charge for production no longer linked to the plan."));
							return false;
						}
						if(CompareDouble.IsGreaterThan(kvp.Value.Sum(x => x.Principal),(double)payPlanProductionEntry.AmountOriginal)) {
							incomeTransferData.StringBuilderErrors.AppendLine(
								Lans.g("PaymentEdit","Transfers cannot be made for this family due to a payment plan with overcharged production."));
							return false;
						}
					}
				}
			}
			listAllAccountEntries.Sort(AccountEntrySort);
			//Move incorrectly allocated splits from payment plans to unearned before going through the AccountBalancingLayers.
			PreprocessPayPlanSplits(ref listAllAccountEntries,ref incomeTransferData,datePay);
			#region Preprocess Unearned/Unallocated
			//Users can choose to make strange decisions like taking from a provider's unearned bucket to pay off a procedure for a different provider
			//even when the original provider has no unearned to take from (see unit tests associated to this commit).
			//These providers that were wrongly taken from need to be credited back so that the income transfer system can correctly balance the account.
			//Without this preprocessing, accounts could end up with a negative unearned bucket (rare, but easy to duplicate).
			List<AccountEntry> listUnearnedUnallocated=listAllAccountEntries.FindAll(x => x.IsUnearned || x.IsUnallocated);
			//Go through each bucket for all unearned and unallocated account entries to balance them out prior to looking at production.
			foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
				if(layer==AccountBalancingLayers.Unearned) {
					continue;//Unearned is special and gets handled after this loop.
				}
				incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,datePay,ref listUnearnedUnallocated,ref listAllAccountEntries));
			}
			#endregion
			#region Payment Plans
			//Find entries with PayPlanNums and perform income transfers for each individual payment plan.
			//Payment plan entries should prefer to transfer within themselves and any excess money (overpaid plan) should move to unearned.
			Dictionary<long,List<AccountEntry>> dictPayPlanEntries=listAllAccountEntries
				.Where(x => x!=null && x.Tag!=null && x.PayPlanNum > 0)
				.GroupBy(x => x.PayPlanNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			List<AccountEntry> listUnearnedEntries;
			foreach(long payPlanNum in dictPayPlanEntries.Keys) {
				List<AccountEntry> listPayPlanEntries=dictPayPlanEntries[payPlanNum];
				//There is a posibility that this payment plan was overpaid.  The overpayment will have been transferred to unearned.
				//This money is now available to go towards other payment plans (if present).
				listUnearnedEntries=listAllAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit)
					&& x.PayPlanNum==0
					&& x.IsUnearned
					&& CompareDecimal.IsLessThanZero(x.AmountEnd));
				//Always consider transferring unearned that is outside of the payment plan into the payment plan.
				listPayPlanEntries.AddRange(listUnearnedEntries);
				//Loop through all of the income transfer layers except the Unearned layer which will be handled manually afterwards.
				foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
					if(layer==AccountBalancingLayers.Unearned) {
						continue;//Unearned is special and gets handled after this loop.
					}
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
				}
			}
			foreach(long payPlanNum in dictPayPlanEntries.Keys) {
				List<AccountEntry> listPayPlanEntries=dictPayPlanEntries[payPlanNum];
				//Go through the list of payment plans again now that all overpayments have been detected. 
				//This allows overpayments from payment plans to flow into other payment plans.
				listUnearnedEntries=listAllAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit)
					&& x.PayPlanNum==0
					&& x.IsUnearned
					&& CompareDecimal.IsLessThanZero(x.AmountEnd));
				//Always consider transferring unearned that is outside of the payment plan into the payment plan.
				listPayPlanEntries.AddRange(listUnearnedEntries);
				//Loop through all of the income transfer layers except the Unearned layer which will be handled manually afterwards.
				foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
					if(layer==AccountBalancingLayers.Unearned) {
						continue;//Unearned is special and gets handled after this loop.
					}
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
				}
			}
			#endregion
			#region Non-Payment Plan Account Entries
			//Loop through all of the income transfer layers except the Unearned layer which will be handled manually afterwards.
			foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
				if(layer==AccountBalancingLayers.Unearned) {
					continue;//Unearned is special and gets handled after this loop.
				}
				//Get all non-payment plan account entries along with any leftover unearned payment plan PaySplits that still have value (can be transferred).
				//There is a posibility that account entries were overpaid.  The overpayments will have been transferred to unearned.
				//This money is now available to go towards payment plans (if present).
				List<AccountEntry> listAccountEntriesNoPP=listAllAccountEntries.FindAll(x => x.PayPlanNum==0
					|| (x.GetType()==typeof(PaySplit) && CompareDecimal.IsLessThanZero(x.AmountEnd)));
				incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,datePay,ref listAccountEntriesNoPP,ref listAllAccountEntries));
			}
			#endregion
			#region Payment Plans Yet Again
			//There is a chance that a payment plan needs money transferred to it and a non-payment plan account entry was overpaid.
			//Perform payment plan income transfer logic one more time to allow the non-payment plan overpayments to flow into payment plan entries.
			foreach(long payPlanNum in dictPayPlanEntries.Keys) {
				List<AccountEntry> listPayPlanEntries=dictPayPlanEntries[payPlanNum];
				//Always consider transferring unearned that is outside of the payment plan into the payment plan.
				//Some account entries could have been overpaid. These overpayments need to be available to transfer into payment plan entries.
				listUnearnedEntries=listAllAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit)
					&& x.PayPlanNum==0
					&& (x.IsUnearned || x.IsUnallocated)
					&& CompareDecimal.IsLessThanZero(x.AmountEnd));
				listPayPlanEntries.AddRange(listUnearnedEntries);
				//Loop through all of the income transfer layers except the Unearned layer which will be handled manually afterwards.
				foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
					if(layer==AccountBalancingLayers.Unearned) {
						continue;//Unearned is special and gets handled after this loop.
					}
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
				}
			}
			#endregion
			#region Unearned
			incomeTransferData.AppendLine($"Processing for layer: {AccountBalancingLayers.Unearned.ToString()}...");
			//Transfer all remaining excess production to unearned keeping the same pat/prov/clinic.
			//Only consider procedures and adjustments because payment plan entries should not be allowed to go into the negative.
			//The scenarios where that would be possible should have been blocked or transferred to unearned above.
			List<AccountEntry> listNegativeProduction=listAllAccountEntries.FindAll(x => x.GetType().In(typeof(Procedure),typeof(Adjustment))
				&& CompareDecimal.IsLessThanZero(x.AmountEnd));
			foreach(AccountEntry negativeProduction in listNegativeProduction) {
				if(negativeProduction.GetType()==typeof(Procedure) && CompareDouble.IsLessThanZero(((Procedure)negativeProduction.Tag).ProcFee)) {
					continue;//do not use negative procedures as a souce of income. 
				}
				incomeTransferData.AppendLine($"  Moving excess production for {negativeProduction.Description} to unearned.");
				negativeProduction.SplitCollection.Add(CreateUnearnedTransfer(negativeProduction,ref incomeTransferData,datePay)[0]);
				negativeProduction.AmountEnd+=Math.Abs(negativeProduction.AmountEnd);
			}
			//Transfer all remaining income to unearned keeping the same pat/prov/clinic.
			List<AccountEntry> listRemainingIncome=listAllAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit)
				&& !x.IsUnearned
				&& !CompareDecimal.IsZero(x.AmountEnd));
			#region Remaining Adjustments
			List<AccountEntry> listRemainingAdjIncome=listRemainingIncome.FindAll(x => ((PaySplit)x.Tag).AdjNum > 0);
			foreach(Bucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingAdjIncome)) {
				foreach(var adjGroup in buckets.ListAccountEntries.GroupBy(x => x.AdjNum).ToDictionary(x => x.Key,x => x.ToList())) {
					decimal amountEndSum=adjGroup.Value.Sum(x => x.AmountEnd);
					if(CompareDecimal.IsZero(amountEndSum)) {
						continue;
					}
					AccountEntry entryFirst=adjGroup.Value.First();
					incomeTransferData.AppendLine($"  Moving excess income for AdjNum #{entryFirst.AdjNum} to unearned.");
					CreateUnearnedTransfer(amountEndSum,entryFirst.PatNum,entryFirst.ProvNum,entryFirst.ClinicNum,ref incomeTransferData,
						adjNum: entryFirst.AdjNum,payPlanNum: entryFirst.PayPlanNum,datePay: datePay);
					adjGroup.Value.ForEach(x => x.AmountEnd=0);
				}
			}
			#endregion
			#region Remaining Procedures
			List<AccountEntry> listRemainingProcIncome=listRemainingIncome.FindAll(x => ((PaySplit)x.Tag).ProcNum > 0);
			foreach(Bucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingProcIncome)) {
				foreach(var procGroup in buckets.ListAccountEntries.GroupBy(x => x.ProcNum).ToDictionary(x => x.Key,x => x.ToList())) {
					decimal amountEndSum=procGroup.Value.Sum(x => x.AmountEnd);
					if(CompareDecimal.IsZero(amountEndSum)) {
						continue;
					}
					AccountEntry entryFirst=procGroup.Value.First();
					incomeTransferData.AppendLine($"  Moving excess income for ProcNum #{entryFirst.ProcNum} to unearned.");
					CreateUnearnedTransfer(amountEndSum,entryFirst.PatNum,entryFirst.ProvNum,entryFirst.ClinicNum,ref incomeTransferData,
						procNum: entryFirst.ProcNum,payPlanNum: entryFirst.PayPlanNum,datePay: datePay);
					procGroup.Value.ForEach(x => x.AmountEnd=0);
				}
			}
			#endregion
			#region Remaining Unallocated
			List<AccountEntry> listRemainingUnallocatedIncome=listAllAccountEntries.FindAll(x => x.IsUnallocated);
			foreach(Bucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingUnallocatedIncome)) {
				decimal amountEndSum=buckets.ListAccountEntries.Sum(x => x.AmountEnd);
				if(CompareDecimal.IsZero(amountEndSum)) {
					continue;
				}
				AccountEntry entryFirst=buckets.ListAccountEntries.First();
				incomeTransferData.AppendLine($"  Moving excess unallocated income for PatNum #{entryFirst.PatNum}" +
					$", ProvNum #{entryFirst.ProvNum}, ClinicNum #{entryFirst.ClinicNum} to unearned.");
				CreateUnearnedTransfer(amountEndSum,entryFirst.PatNum,entryFirst.ProvNum,entryFirst.ClinicNum,ref incomeTransferData,
					payPlanNum: entryFirst.PayPlanNum,datePay: datePay);
				buckets.ListAccountEntries.ForEach(x => x.AmountEnd=0);
			}
			#endregion
			#endregion
			if(incomeTransferData.HasInvalidNegProd) {
				incomeTransferData.StringBuilderErrors.AppendLine(
					Lans.g("PaymentEdit","Negative production needs to be manually allocated before making an income transfer:")
				);
			}
			//Look for any error messages that need to be displayed to the user after the income transfer 
			foreach(AccountEntry accountEntryError in listAllAccountEntries.FindAll(x => x.ErrorMsg.Length > 0)) {
				incomeTransferData.StringBuilderErrors.AppendLine(accountEntryError.ErrorMsg.ToString().Trim());
			}
			//Look for any warning messages that need to be displayed to the user after the income transfer 
			foreach(AccountEntry accountEntryWarning in listAllAccountEntries.FindAll(x => x.WarningMsg.Length > 0)) {
				incomeTransferData.StringBuilderWarnings.AppendLine(accountEntryWarning.WarningMsg.ToString().Trim());
			}
			if(incomeTransferData.StringBuilderErrors.Length > 0) {
				return false;//Indicate to the calling method that the income transfer data should not be used.
			}
			return true;
		}

		private static string GetInvalidPayPlanDescription(string errorMsgStart,List<PayPlan> listInvalidPayPlans,
			Dictionary<long,List<PayPlanCharge>> dictPayPlanCharges)
		{
			List<long> listInvalidPatNums=listInvalidPayPlans.Select(x => x.PatNum).ToList();
			listInvalidPatNums.AddRange(listInvalidPayPlans.Select(x => x.Guarantor));
			Dictionary<long,Patient> dictPatients=Patients.GetLimForPats(listInvalidPatNums.Distinct().ToList()).ToDictionary(x => x.PatNum);
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine(errorMsgStart);
			foreach(PayPlan payPlan in listInvalidPayPlans) {
				string ppType;
				if(payPlan.IsDynamic) {
					ppType="DPP";
				}
				else if(payPlan.PlanNum==0) {
					ppType="PP";
				}
				else {
					ppType="Ins";
				}
				string planCategory=Lans.g("ContrAccount","None");
				if(payPlan.PlanCategory > 0) {
					planCategory=Defs.GetDef(DefCat.PayPlanCategories,payPlan.PlanCategory).ItemName;
				}
				double principal=PayPlans.GetTotalPrinc(payPlan.PayPlanNum,dictPayPlanCharges[payPlan.PayPlanNum]);
				stringBuilder.AppendLine($"Date: {payPlan.PayPlanDate.ToShortDateString()}");
				stringBuilder.AppendLine($"  Guarantor: {dictPatients[payPlan.Guarantor].GetNameLF()}");
				stringBuilder.AppendLine($"  Patient: {dictPatients[payPlan.PatNum].GetNameLF()}");
				stringBuilder.AppendLine($"  Type: {ppType}");
				stringBuilder.AppendLine($"  Category: {planCategory}");
				stringBuilder.AppendLine($"  Principal: {principal.ToString("C")}");
			}
			return stringBuilder.ToString().Trim();
		}

		///<summary>Creates income transfer buckets out of the account entries passed in and then processes the buckets for the layer.
		///Preprocessing will be performed for the ProvPatClinic layer only.  See PreprocessProvPatClinicBuckets() for details.</summary>
		private static IncomeTransferData TransferForLayer(AccountBalancingLayers layer,DateTime datePay,
			ref List<AccountEntry> listProcessAccountEntries,ref List<AccountEntry> listAllAccountEntries)
		{
			//No remoting role check; private method
			IncomeTransferData incomeTransferData=new IncomeTransferData();
			List<Bucket> listBuckets=CreateTransferBucketsForLayer(layer,listProcessAccountEntries);
			//Preprocess the production explicitly linked to procedures within each bucket on layer ProvPatClinic.
			if(layer==AccountBalancingLayers.ProvPatClinic) {
				PreprocessProvPatClinicBuckets(ref listBuckets,ref listProcessAccountEntries,ref incomeTransferData,datePay,ref listAllAccountEntries);
			}
			incomeTransferData.AppendLine($"Processing buckets for layer: {layer.ToString()}...");
			//Process each bucket and make any necessary income transfers for this layer.
			//Create 'account entries' for any transfers that were created so that subsequent layers know about the transfers from previous layers.
			foreach(Bucket bucket in listBuckets) {
				incomeTransferData.MergeIncomeTransferData(TransferLoopHelper(bucket,datePay));
			}
			return incomeTransferData;
		}

		///<summary>Groups up the account entries passed in into buckets based on the layer passed in.</summary>
		private static List<Bucket> CreateTransferBucketsForLayer(AccountBalancingLayers layer,List<AccountEntry> listAccountEntries) {
			//No remoting role check; private method
			switch(layer) {
				case AccountBalancingLayers.ProvPatClinic:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.ProvPat:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.ProvClinic:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.PatClinic:
					return listAccountEntries.GroupBy(x => new { x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Prov:
					return listAccountEntries.GroupBy(x => new { x.ProvNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Pat:
					return listAccountEntries.GroupBy(x => new { x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Clinic:
					return listAccountEntries.GroupBy(x => new { x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new Bucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Nothing:
					return new List<Bucket>() {
						new Bucket(listAccountEntries)
					};
				case AccountBalancingLayers.Unearned:
				default:
					throw new ODException($"Income transfer buckets cannot be created for unsupported layer: {layer}");
			}
		}

		///<summary>Only call after explicit linking has been ran against the account entries passed in. Moves incorrectly linked payment splits from payment plans to unearned. E.g. when a payment split is linked to a payment plan and a procedure but the procedure is not part of the payment plan.</summary>
		private static void PreprocessPayPlanSplits(ref List<AccountEntry> listAccountEntries,ref IncomeTransferData incomeTransferData,DateTime datePay) {
			#region Unearned Splits
			//There is no such thing as unearned money in payment plan land. Any money associated to a payment plan is 'earned' due to the debits.
			//Transfer all unearned payment splits that are also attached to a payment plan out into the generic unearned bucket.
			List<AccountEntry> listUnearnedPayPlanSplits=listAccountEntries.FindAll(x => x.PayPlanNum > 0 && x.IsUnearned);
			var dictUnearnedPayPlanSplits=listUnearnedPayPlanSplits
				.GroupBy(x => new { x.PayPlanNum,x.PatNum,x.ProvNum,x.ClinicNum,x.ProcNum,x.AdjNum,x.UnearnedType })
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(var kvp in dictUnearnedPayPlanSplits) {
				List<AccountEntry> listPayPlanEntries=kvp.Value;
				decimal offsetAmt=listPayPlanEntries.Sum(x => x.AmountEnd);
				if(CompareDecimal.IsGreaterThanOrEqualToZero(offsetAmt)) {
					continue;
				}
				//These unearned splits are incorrectly associated to the payment plan. Move them to the non-payment plan unearned bucket.
				List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,
					listPayPlanEntries.First().PatNum,
					listPayPlanEntries.First().ProvNum,
					listPayPlanEntries.First().ClinicNum,
					ref incomeTransferData,
					procNum:listPayPlanEntries.First().ProcNum,
					adjNum:listPayPlanEntries.First().AdjNum,
					unearnedType:listPayPlanEntries.First().UnearnedType,
					payPlanNum:listPayPlanEntries.First().PayPlanNum,
					datePay:datePay);
				AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
				AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
				//Zero out the AmountEnd field for every negative account entry so that they are not transferred again.
				listPayPlanEntries.ForEach(x => x.AmountEnd=0);
				//Also, the offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
				accountEntryOffset.AmountEnd=0;
				listAccountEntries.AddRange(new List<AccountEntry>() {
					accountEntryOffset,
					accountEntryUnearned,
				});
			}
			#endregion
			#region Incorrectly Linked Splits
			//Explicit linking should have taken care of payment splits that are correctly linked to credits / production by this point.
			//Therefore, blindly make transfers away from payment plans when account entries are incorrectly linked to production.
			List<AccountEntry> listLinkedPayPlanSplits=listAccountEntries.FindAll(x => x.PayPlanNum > 0 && x.GetType()==typeof(PaySplit));
			var dictLinkedPayPlanSplits=listLinkedPayPlanSplits
				.GroupBy(x => new { x.PayPlanNum,x.PatNum,x.ProvNum,x.ClinicNum,x.ProcNum,x.AdjNum,x.UnearnedType,x.PayPlanDebitType })
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(var kvp in dictLinkedPayPlanSplits) {
				List<AccountEntry> listPayPlanEntries=kvp.Value;
				decimal offsetAmt=listPayPlanEntries.Sum(x => x.AmountEnd);
				if(CompareDecimal.IsZero(offsetAmt)) {
					continue;
				}
				//These splits are incorrectly allocated to production that are not part of this payment plan. Move them to unearned.
				List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,
					listPayPlanEntries.First().PatNum,
					listPayPlanEntries.First().ProvNum,
					listPayPlanEntries.First().ClinicNum,
					ref incomeTransferData,
					procNum:listPayPlanEntries.First().ProcNum,
					adjNum:listPayPlanEntries.First().AdjNum,
					unearnedType:listPayPlanEntries.First().UnearnedType,
					payPlanNum:listPayPlanEntries.First().PayPlanNum,
					datePay:datePay,
					payPlanDebitType:listPayPlanEntries.First().PayPlanDebitType);
				AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
				AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
				//Zero out the AmountEnd field for every account entry so that they are not transferred again.
				listPayPlanEntries.ForEach(x => x.AmountEnd=0);
				//Also, the offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
				accountEntryOffset.AmountEnd=0;
				listAccountEntries.AddRange(new List<AccountEntry>() {
					accountEntryOffset,
					accountEntryUnearned,
				});
			}
			#endregion
		}

		///<summary>Breaks up the individual buckets passed in into sub-buckets that are grouped by procedures.
		///Loops through each sub-bucket and removes as much production as possible (allocating AmountEnd).
		///Creates a transfer if there is a net negative after all production has been allocated.
		///There shouldn't be such a thing as negative production and if there is then it should be transferred to unearned for later transfers.
		///Also, balances any unallocated / unearned income that is not explicitly linked to a procedure for all buckets passed in.
		///This last step is so that we don't get stuck in infinite loops transferring income between unallocated and unearned.</summary>
		private static void PreprocessProvPatClinicBuckets(ref List<Bucket> listBuckets,ref List<AccountEntry> listProcessAccountEntries,
			ref IncomeTransferData incomeTransferData,DateTime datePay,ref List<AccountEntry> listAllAccountEntries)
		{
			//No remoting role check; private method
			incomeTransferData.AppendLine($"Preprocessing buckets for ProvPatClinic...");
			foreach(Bucket bucket in listBuckets) {
				List<AccountEntry> listUnallocatedEntries=new List<AccountEntry>();
				List<AccountEntry> listUnearnedEntries=new List<AccountEntry>();
				List<AccountEntry> listNonPayPlanEntries=bucket.ListAccountEntries.FindAll(x => x.PayPlanNum==0 && x.GetType()!=typeof(FauxAccountEntry));
				List<AccountEntry> listPayPlanEntries=bucket.ListAccountEntries.FindAll(x => x.PayPlanNum > 0);
				foreach(var procGroup in listNonPayPlanEntries.GroupBy(x => x.ProcNum).ToDictionary(x => x.Key,x => x.ToList())) {
					List<AccountEntry> listBucketEntries=procGroup.Value;
					if(procGroup.Key==0) {//Account entries are not associated to any procedures.
						listUnallocatedEntries=listBucketEntries.FindAll(x => x.IsUnallocated);
						listUnearnedEntries=listBucketEntries.FindAll(x => x.IsUnearned);
						continue;
					}
					decimal total=listBucketEntries.Sum(x => x.AmountEnd);
					if(CompareDecimal.IsZero(total)) {
						listBucketEntries.ForEach(x => x.AmountEnd=0);
						continue;
					}
					else if(CompareDecimal.IsGreaterThanZero(total)) {
						continue;
					}
					long clinicNum=listBucketEntries.First().ClinicNum;
					long patNum=listBucketEntries.First().PatNum;
					long procNum=procGroup.Key;
					long provNum=listBucketEntries.First().ProvNum;
					//This procedure specific bucket does not balance out.
					//Try to balance the procedure specific bucket FIFO style (strictly consider negative and positive amounts).
					string results=BalanceAccountEntries(ref listBucketEntries);
					if(!string.IsNullOrWhiteSpace(results)) {
						incomeTransferData.AppendLine($"  Procedure #{procNum} PatNum #{patNum}" +
							$", ProvNum #{provNum} ({Providers.GetAbbr(provNum)})" +
							$"{((clinicNum > 0) ? $", ClinicNum #{clinicNum} ({Clinics.GetAbbr(clinicNum)})" : "")}" +
							$" is over allocated");
						incomeTransferData.AppendLine(results);
					}
					//Any negative AmountEnd values need to be transferred to unearned so that other buckets can utilize the overpayments.
					//PaySplits will be created to offset any production or income that left this bucket in the negative.
					decimal offsetAmt=listBucketEntries.Sum(x => x.AmountEnd);
					if(!PrefC.GetBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome)) {
						offsetAmt=GetTransferableIncomeAmount(offsetAmt,ref listBucketEntries,ref incomeTransferData);
					}
					//Create AccountEntries out of the PaySplits that were just created and add them to listAccountEntries for allocation.
					List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,patNum,provNum,clinicNum,ref incomeTransferData,procNum:procNum,
						datePay:datePay);
					AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
					AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
					//The PaySplits that were just created will technically offset any production or income that left this bucket in the negative.
					//Zero out the AmountEnd field for every negative account entry so that they are not transferred.
					listBucketEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd)).ForEach(x => x.AmountEnd=0);
					//Also, the offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
					accountEntryOffset.AmountEnd=0;
					listProcessAccountEntries.AddRange(new List<AccountEntry>() {
						accountEntryOffset,
						accountEntryUnearned,
					});
					listAllAccountEntries.AddRange(new List<AccountEntry>() {
						accountEntryOffset,
						accountEntryUnearned,
					});
					listUnearnedEntries.Add(accountEntryUnearned);
				}
				foreach(var adjGroup in listNonPayPlanEntries.GroupBy(x => x.AdjNum).ToDictionary(x => x.Key,x => x.ToList())) {
					if(adjGroup.Key==0) {
						continue;//Unearned and unallocated have already been considered within the procedure grouping loop.
					}
					List<AccountEntry> listBucketEntries=adjGroup.Value;
					decimal total=listBucketEntries.Sum(x => x.AmountEnd);
					if(CompareDecimal.IsZero(total)) {
						listBucketEntries.ForEach(x => x.AmountEnd=0);
						continue;
					}
					else if(CompareDecimal.IsGreaterThanZero(total)) {
						continue;
					}
					long adjNum=adjGroup.Key;
					long clinicNum=listBucketEntries.First().ClinicNum;
					long patNum=listBucketEntries.First().PatNum;
					long provNum=listBucketEntries.First().ProvNum;
					//This adjustment specific bucket does not balance out.
					//Try to balance the adjustment specific bucket FIFO style (strictly consider negative and positive amounts).
					string results=BalanceAccountEntries(ref listBucketEntries);
					if(!string.IsNullOrWhiteSpace(results)) {
						incomeTransferData.AppendLine($"  Adjustment #{adjNum} PatNum #{patNum}" +
							$", ProvNum #{provNum} ({Providers.GetAbbr(provNum)})" +
							$"{((clinicNum > 0) ? $", ClinicNum #{clinicNum} ({Clinics.GetAbbr(clinicNum)})" : "")}" +
							$" is over allocated");
						incomeTransferData.AppendLine(results);
					}
					//Any negative AmountEnd values need to be transferred to unearned so that other buckets can utilize the overpayments.
					//PaySplits will be created to offset any production or income that left this bucket in the negative.
					decimal offsetAmt=listBucketEntries.Sum(x => x.AmountEnd);
					if(!PrefC.GetBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome)) {
						offsetAmt=GetTransferableIncomeAmount(offsetAmt,ref listBucketEntries,ref incomeTransferData);
					}
					//Any negative balance left over shall get transferred to unearned so that it can be balanced later.
					//Create an AccountEntry out of the unearned PaySplit that was just created and add it to listPendingAccountEntries for allocation.
					List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,patNum,provNum,clinicNum,ref incomeTransferData,adjNum:adjNum,
						datePay:datePay);
					AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
					AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
					//The PaySplits that were just created will technically offset any production or income that left this bucket in the negative.
					//Zero out the AmountEnd field for every negative account entry so that they are not transferred.
					listBucketEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd)).ForEach(x => x.AmountEnd=0);
					//Also, the offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
					accountEntryOffset.AmountEnd=0;
					listProcessAccountEntries.AddRange(new List<AccountEntry>() {
						accountEntryOffset,
						accountEntryUnearned,
					});
					listAllAccountEntries.AddRange(new List<AccountEntry>() {
						accountEntryOffset,
						accountEntryUnearned,
					});
					listUnearnedEntries.Add(accountEntryUnearned);
				}
				foreach(var payPlanGroup in listPayPlanEntries.GroupBy(x => x.PayPlanNum).ToDictionary(x => x.Key,x => x.ToList())) {
					if(payPlanGroup.Key==0) {//Account entries are not associated to any payment plans.
						continue;
					}
					List<AccountEntry> listBucketEntries=payPlanGroup.Value;
					//Skip adjustments because they directly manipulate the value of production and should not be transferred to unearned at this point.
					listBucketEntries.RemoveAll(x => x.GetType()==typeof(FauxAccountEntry) && ((FauxAccountEntry)x).IsAdjustment);
					decimal total=listBucketEntries.Sum(x => x.AmountEnd);
					if(CompareDecimal.IsZero(total)) {
						listBucketEntries.ForEach(x => x.AmountEnd=0);
						continue;
					}
					else if(CompareDecimal.IsGreaterThanZero(total)) {
						continue;
					}
					long clinicNum=listBucketEntries.First().ClinicNum;
					long patNum=listBucketEntries.First().PatNum;
					long payPlanNum=payPlanGroup.Key;
					long provNum=listBucketEntries.First().ProvNum;
					//This payment plan specific bucket does not balance out.
					//Try to balance the payment plan specific bucket FIFO style (strictly consider negative and positive amounts).
					string results=BalanceAccountEntries(ref listBucketEntries);
					if(!string.IsNullOrWhiteSpace(results)) {
						incomeTransferData.AppendLine($"  Payment Plan #{payPlanNum} PatNum #{patNum}" +
							$", ProvNum #{provNum} ({Providers.GetAbbr(provNum)})" +
							$"{((clinicNum > 0) ? $", ClinicNum #{clinicNum} ({Clinics.GetAbbr(clinicNum)})" : "")}");
						incomeTransferData.AppendLine(results);
					}
					//Payment plans are unique in that they can have unearned payments attached that might need to be moved out of the payment plan.
					//Preserve the UnearnedType in order to not create an infinite loop of transferring the same unearned out of a payment plan over and over.
					//Payment plan payments (regardless if they are associated to unearned or not) will typically specify if they apply towards principal or interest.
					//All offsetting payment splits should also preserve where the split applies to in order to help dynamic payment plans calculate charges.
					var dictionaryUnearnedTypeDebitTypeGroupEntries=listBucketEntries
						.GroupBy(x => new { x.UnearnedType,x.PayPlanDebitType })
						.ToDictionary(x => x.Key,x => x.ToList());
					foreach(var keyValuePairUnearnedTypeDebitTypeEntries in dictionaryUnearnedTypeDebitTypeGroupEntries) {
						//The Value of the dictionary is a list of payment plan account entries that all share the same UnearnedType and PayPlanDebitType.
						List<AccountEntry> listUnearnedTypeDebitTypeEntries=keyValuePairUnearnedTypeDebitTypeEntries.Value;
						//Create helper variables for the UnearnedType and PayPlanDebitType values since they are the same for all account entries in this group.
						long unearnedType=listUnearnedTypeDebitTypeEntries.First().UnearnedType;
						PayPlanDebitTypes payPlanDebitType=listUnearnedTypeDebitTypeEntries.First().PayPlanDebitType;
						//Sum up AmountEnd for all of the account entries to see if this group has a net negative value that should be transferred.
						decimal offsetAmt=listUnearnedTypeDebitTypeEntries.Sum(x => x.AmountEnd);
						if(CompareDecimal.IsGreaterThanOrEqualToZero(offsetAmt)) {
							//There should never be a scenario where a positive offsetAmt exists (due to BalanceAccountEntries above).
							//However, there will be valid scenarios where a particular grouping sums up to $0 which need to be ignored.
							//E.g. When only Principal or only Interest is overpaid.
							continue;
						}
						List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,patNum,provNum,clinicNum,ref incomeTransferData,unearnedType:unearnedType,
							payPlanNum:payPlanNum,datePay:datePay,payPlanDebitType:payPlanDebitType);
						AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
						AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
						//The PaySplits that were just created will technically offset any production or income that left this bucket in the negative.
						//Zero out the AmountEnd field for every negative account entry so that they are not transferred.
						listUnearnedTypeDebitTypeEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd)).ForEach(x => x.AmountEnd=0);
						//Also, the offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
						accountEntryOffset.AmountEnd=0;
						listProcessAccountEntries.AddRange(new List<AccountEntry>() {
							accountEntryOffset,
							accountEntryUnearned,
						});
						listAllAccountEntries.AddRange(new List<AccountEntry>() {
							accountEntryOffset,
							accountEntryUnearned,
						});
						listUnearnedEntries.Add(accountEntryUnearned);
					}
				}
				//Balance both unallocated and unearned lists (manipulate their AmountEnd to balance themselves out FIFO style).
				if(listUnallocatedEntries.Count > 1) {
					string results=BalanceAccountEntries(ref listUnallocatedEntries);
					if(!string.IsNullOrWhiteSpace(results)) {
						incomeTransferData.AppendLine($"  Balancing {listUnallocatedEntries.Count} unallocated PaySplits not associated to any procedures...");
						incomeTransferData.AppendLine(results);
						incomeTransferData.AppendLine($"  Done");
					}
				}
				if(listUnearnedEntries.Count > 1) {
					string results=null;
					if(PrefC.IsODHQ) {
						//Invoke the helper method that will balance individual uneared types and will suggest new splits.
						results=BalanceAccountEntriesByUnearnedType(ref listUnearnedEntries,ref incomeTransferData,datePay);
					}
					else {
						//Invoke the helper method that will simply balance positive and negative account entries and will not suggest new splits.
						results=BalanceAccountEntries(ref listUnearnedEntries);
					}
					if(!string.IsNullOrWhiteSpace(results)) {
						incomeTransferData.AppendLine($"  Balancing {listUnearnedEntries.Count} unearned PaySplits not associated to any procedures...");
						incomeTransferData.AppendLine(results);
						incomeTransferData.AppendLine($"  Done");
					}
				}
			}
		}

		///<summary>Loops through all negative account entries from the list of entries passed in and looks for transferable income.
		///Income will be removed from each entry (e.g. manipulates AmountEnd and IncomeAmt) as it finds transferable income.
		///Returns the maximum possible amount of transferable income stopping at the offsetAmt passed in.</summary>
		private static decimal GetTransferableIncomeAmount(decimal offsetAmt,ref List<AccountEntry> listAccountEntries,
			ref IncomeTransferData incomeTransferData)
		{
			//Any negative AmountEnd values need to be transferred to unearned so that other buckets can utilize the overpayments.
			List<AccountEntry> listNegEntries=listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
			//Figure out how much transferable income is available because the office does not allow treating negative production as income.
			decimal transferableIncomeTotal=0;
			foreach(AccountEntry accountEntryNeg in listNegEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.IncomeAmt))) {
				if(CompareDecimal.IsEqual(offsetAmt,transferableIncomeTotal)) {
					break;//Enough transferable income has been found in order to offset this bucket.
				}
				//This account entry has been overpaid somehow (hence the AmountEnd is in the negative).
				//Therefore, it is always safe to transfer income away from this account entry.
				//Don't take too much value away from the account entry which would give the entry a positive value and only transfer as much income allots.
				decimal transferableIncome=Math.Min(Math.Abs(accountEntryNeg.AmountEnd),Math.Abs(accountEntryNeg.IncomeAmt));
				//Subtract the transferable income from the account entry so that we do not do this again in another bucket.
				accountEntryNeg.IncomeAmt-=transferableIncome;
				//Subtract the transferable income from the total of income to be transferred (simply because offsetAmt is stored as a negative).
				transferableIncomeTotal-=transferableIncome;
				//Add the transferable income to AmountEnd in case only a partial amount was removed.
				accountEntryNeg.AmountEnd+=transferableIncome;
			}
			//There may have been enough income spread out on the negative account entries.
			//However, if the required offset amount does not equate to the amount of transferable income then there is negative production held up.
			//Notify the user of all account entries that are not allowed to be transferred into unearned.
			if(!CompareDecimal.IsEqual(offsetAmt,transferableIncomeTotal)) {
				incomeTransferData.HasInvalidNegProd=true;
				Family family=Patients.GetFamily(listAccountEntries.First().PatNum);
				//Notify the user about all of the account entries that have negative AmountEnd values since they should be negative production.
				foreach(AccountEntry accountEntryNeg in listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd))) {
					string desc=$"{accountEntryNeg.Date.ToShortDateString()}  {accountEntryNeg.GetType().Name}";
					if(accountEntryNeg.GetType()==typeof(Adjustment)) {
						desc+=$": '{Defs.GetName(DefCat.AdjTypes,((Adjustment)accountEntryNeg.Tag).AdjType)}'";
					}
					else if(accountEntryNeg.GetType()==typeof(Procedure)) {
						Procedure proc=(Procedure)accountEntryNeg.Tag;
						desc+=$": '{Procedures.ConvertProcToString(proc.CodeNum,proc.Surf,proc.ToothNum,true)}'";
					}
					desc+=$"  {accountEntryNeg.AmountOriginal:C}"
						+$"\r\n  Patient: '{family.GetNameInFamFL(accountEntryNeg.PatNum)}'";
					if(accountEntryNeg.ProvNum > 0) {
						desc+=$"\r\n  Provider: {Providers.GetAbbr(accountEntryNeg.ProvNum)}";
					}
					if(accountEntryNeg.ClinicNum > 0) {
						desc+=$"\r\n  Clinic: {Clinics.GetAbbr(accountEntryNeg.ClinicNum)}";
					}
					accountEntryNeg.ErrorMsg.AppendLine(desc);
				}
			}
			return transferableIncomeTotal;
		}

		///<summary>Loops through all of the positive entries in the bucket and creates as many transfers as necessary from the negative entries
		///until the entire bucket has been balanced as much as possible.  Preprocessing should be done prior to calling this method.</summary>
		private static IncomeTransferData TransferLoopHelper(Bucket bucket,DateTime datePay) {
			//No remoting role check; private method
			IncomeTransferData transferData=new IncomeTransferData();
			foreach(AccountEntry posCharge in bucket.ListPositiveEntries) {
				if(CompareDecimal.IsLessThanOrEqualToZero(posCharge.AmountEnd)) {
					continue;
				}
				bool hasTransfer=false;
				foreach(AccountEntry negCharge in bucket.ListNegativeEntries) {
					if(CompareDecimal.IsLessThanOrEqualToZero(posCharge.AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(negCharge.AmountEnd)) {
						continue;
					}
					//Check if both positive and negative charges are misallocated PaySplits and skip them if they are.
					//This is to prevent transferring between misallocated income which would just create more misallocated income.
					if(posCharge.IsPaySplitAttachedToProd && negCharge.IsPaySplitAttachedToProd) {
						continue;
					}
					if(!hasTransfer) {
						transferData.AppendLine($"  Balancing {posCharge.Description}");
						hasTransfer=true;
					}
					transferData.AppendLine($"    Moving excess to {negCharge.Description}");
					transferData.MergeIncomeTransferData(CreateTransferHelper(posCharge,negCharge,datePay));
				}
				if(hasTransfer) {
					transferData.AppendLine($"  Done - {posCharge.Description}");
				}
			}
			return transferData;
		}

		///<summary>Creates and links paysplits with micro-allocations based on the charges passed in. Constructs the paysplits with all necessary information depending on the type of the charges.</summary>
		private static IncomeTransferData CreateTransferHelper(AccountEntry posCharge,AccountEntry negCharge,DateTime datePay) {
			//No remoting role check; private method
			IncomeTransferData transferSplits=new IncomeTransferData();
			if(negCharge.GetType()==typeof(Procedure) && CompareDouble.IsLessThanZero(((Procedure)negCharge.Tag).ProcFee)) {
				transferSplits.AppendLine($"  Negative procedure cannot be used as source of income:\r\n      {negCharge.Description}");
				return transferSplits;//do not use negative procedures as sources of income. 
			}
			decimal amt=Math.Min(Math.Abs(posCharge.AmountEnd),Math.Abs(negCharge.AmountEnd));
			if(CompareDecimal.IsEqual(amt,0)) {
				return transferSplits;//there is no income to transfer
			}
			PaySplit posSplit=CreatePaySplitHelper(posCharge,(double)amt,datePay,unearnedType:posCharge.UnearnedType);
			PaySplit negSplit=CreatePaySplitHelper(negCharge,0-(double)amt,datePay,unearnedType:negCharge.UnearnedType);
			//Never allow unearned to be transferred towards a provider (posSplit) when AllowPrepayProvider is off.
			//However, it is acceptable for unearned to be transferred away from a provider (negSplit).
			if(!PrefC.GetBool(PrefName.AllowPrepayProvider) && posSplit.UnearnedType!=0 && posSplit.ProvNum!=0) {
				transferSplits.HasInvalidSplits=true;
				return transferSplits;
			}
			transferSplits.ListSplitsCur.AddRange(new List<PaySplit>() { posSplit,negSplit });
			negCharge.SplitCollection.Add(negSplit);
			posCharge.SplitCollection.Add(posSplit);
			posCharge.AmountEnd-=amt;
			negCharge.AmountEnd+=amt;
			transferSplits.AppendLine($"      ^PaySplit created to move {amt.ToString("c")} " +
				$"from {posCharge.TagTypeName} [AmtEnd: {posCharge.AmountEnd.ToString("c")}]");
			transferSplits.AppendLine($"      ^PaySplit created to move {amt.ToString("c")} " +
				$"to {negCharge.TagTypeName} [AmtEnd: {negCharge.AmountEnd.ToString("c")}]");
			return transferSplits;
		}

		private static List<PaySplit> CreateUnearnedTransfer(AccountEntry accountEntry,ref IncomeTransferData incomeTransferData,DateTime datePay) {
			//No remoting role check; private method
			long unearnedType=0;
			if(accountEntry.GetType()==typeof(PaySplit)) {
				unearnedType=((PaySplit)accountEntry.Tag).UnearnedType;
			}
			long offsetUnearnedType=0;
			//Payment plans are the only entities that need the ability to transfer unearned to unearned.
			if(accountEntry.PayPlanNum > 0) {
				offsetUnearnedType=unearnedType;
			}
			if(datePay.Year < 1880) {
				datePay=DateTime.Today;
			}
			PaySplit offsetSplit=CreatePaySplitHelper(accountEntry,
				(double)accountEntry.AmountEnd,
				datePay,
				unearnedType:offsetUnearnedType,
				isNew:true);
			PaySplit unearnedSplit=new PaySplit() {
				AdjNum=0,
				ClinicNum=offsetSplit.ClinicNum,
				DatePay=datePay,
				IsNew=true,
				PatNum=offsetSplit.PatNum,
				PayPlanNum=0,
				ProcNum=0,
				ProvNum=PrefC.GetBool(PrefName.AllowPrepayProvider) ? offsetSplit.ProvNum : 0,
				SplitAmt=0-offsetSplit.SplitAmt,
				UnearnedType=(unearnedType==0? PrefC.GetLong(PrefName.PrepaymentUnearnedType) : unearnedType),
			};
			return new List<PaySplit>() { 
				offsetSplit,
				unearnedSplit
			};
		}

		///<summary>Makes a split to take splitAmount away from unearned and then makes an offsetting split of splitAmount using the values passed in.
		///Always returns exactly two PaySplits. The first will be the 'offset' split and the second will be the split taking from unearned.</summary>
		public static List<PaySplit> CreateUnearnedTransfer(decimal splitAmount,long patNum,long provNum,long clinicNum,
			ref IncomeTransferData incomeTransferData,long procNum=0,long adjNum=0,long unearnedType=0,long payPlanNum=0,DateTime datePay=default,
			long payPlanChargeNum=0,PayPlanDebitTypes payPlanDebitType=PayPlanDebitTypes.Unknown,bool isForDynamicPaymentPlanBalancing=false,
			bool isMovingFromHiddenUnearnedToUnearned=false)
		{
			//No remoting role check; private method
			long offsetUnearnedType=0;
			//Payment plans are the only entities that need the ability to transfer unearned to unearned.
			if(payPlanNum > 0) {
				offsetUnearnedType=unearnedType;
			}
			long unearnedPayPlanNum=0;
			if(isForDynamicPaymentPlanBalancing) {
				offsetUnearnedType=0;
				unearnedPayPlanNum=payPlanNum;
			}
			if(isMovingFromHiddenUnearnedToUnearned) {
				offsetUnearnedType=unearnedType;
				unearnedType=0;
				unearnedPayPlanNum=0;
				splitAmount=-splitAmount;
			}
			if(datePay.Year < 1880) {
				datePay=DateTime.Today;
			}
			PaySplit offsetSplit=new PaySplit() {
				AdjNum=adjNum,
				ClinicNum=clinicNum,
				DatePay=datePay,
				IsNew=true,
				PatNum=patNum,
				PayPlanChargeNum=payPlanChargeNum,
				PayPlanDebitType=payPlanDebitType,
				PayPlanNum=payPlanNum,
				ProcNum=procNum,
				ProvNum=provNum,
				SplitAmt=(double)splitAmount,
				UnearnedType=offsetUnearnedType,
			};
			PaySplit unearnedSplit=new PaySplit() {
				AdjNum=0,
				ClinicNum=clinicNum,
				DatePay=datePay,
				IsNew=true,
				PatNum=patNum,
				PayPlanNum=unearnedPayPlanNum,
				ProcNum=0,
				ProvNum=PrefC.GetBool(PrefName.AllowPrepayProvider) ? provNum : 0,
				SplitAmt=0-(double)splitAmount,
				UnearnedType=(unearnedType==0? PrefC.GetLong(PrefName.PrepaymentUnearnedType) : unearnedType),
			};
			List<PaySplit> listPaySplits=new List<PaySplit>() { offsetSplit,unearnedSplit };
			incomeTransferData.ListSplitsCur.AddRange(listPaySplits);
			incomeTransferData.AppendLine($"    ^PaySplit created to move {offsetSplit.SplitAmt.ToString("c")} from {GetPaySplitTypeDesc(offsetSplit)}");
			incomeTransferData.AppendLine($"    ^PaySplit created to move {unearnedSplit.SplitAmt.ToString("c")} to {GetPaySplitTypeDesc(unearnedSplit)}");
			return listPaySplits;
		}

		///<summary>A helper method that will move as much AmountEnd as possible from the negative entries and apply them to the positive ones.
		///Returns true if account entries changed.  Otherwise; false.</summary>
		private static string BalanceAccountEntries(ref List<AccountEntry> listAccountEntries) {
			//No remoting role check; private method
			StringBuilder strBuilderSummary=new StringBuilder();
			List<AccountEntry> listAccountEntriesPositive=listAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			List<AccountEntry> listAccountEntriesNegative=listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
			foreach(AccountEntry positiveEntry in listAccountEntriesPositive) {
				if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
					continue;
				}
				foreach(AccountEntry negativeEntry in listAccountEntriesNegative) {
					if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(negativeEntry.AmountEnd)) {
						continue;
					}
					decimal amountTxfr=Math.Min(Math.Abs(positiveEntry.AmountEnd),Math.Abs(negativeEntry.AmountEnd));
					positiveEntry.AmountEnd-=amountTxfr;
					negativeEntry.AmountEnd+=amountTxfr;
					strBuilderSummary.AppendLine($"    Removed {amountTxfr.ToString("c")} from {positiveEntry.Description}");
					strBuilderSummary.AppendLine($"    Added {amountTxfr.ToString("c")} to {negativeEntry.Description}");
				}
			}
			return strBuilderSummary.ToString().TrimEnd();
		}

		///<summary>A helper method that will suggest payment splits from one unearned type to another unearned type.</summary>
		private static string BalanceAccountEntriesByUnearnedType(ref List<AccountEntry> listAccountEntries,ref IncomeTransferData incomeTransferData,
			DateTime datePay)
		{
			//No remoting role check; private method
			StringBuilder strBuilderSummary=new StringBuilder();
			//Balance the negative and positive entries from different unearned types by suggesting payment splits from one to the other.
			foreach(AccountEntry positiveEntry in listAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd) && x.IsUnearned)) {
				if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
					continue;
				}
				List<AccountEntry> listOtherUnearnedEntries=listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd) 
					&& x.IsUnearned
					&& x.UnearnedType!=positiveEntry.UnearnedType);
				foreach(AccountEntry negativeEntry in listOtherUnearnedEntries) {
					if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(negativeEntry.AmountEnd)) {
						continue;
					}
					decimal amountTxfr=Math.Min(Math.Abs(positiveEntry.AmountEnd),Math.Abs(negativeEntry.AmountEnd));
					#region Offsetting Unearned Payment Splits
					//Create two splits in order to take from the positive entry and give to the negative entry.
					if(datePay.Year < 1880) {
						datePay=DateTime.Today;
					}
					PaySplit splitOffset=CreatePaySplitHelper(positiveEntry,(double)amountTxfr,datePay,unearnedType:positiveEntry.UnearnedType);
					PaySplit splitAllocate=CreatePaySplitHelper(negativeEntry,0-(double)amountTxfr,datePay,unearnedType:negativeEntry.UnearnedType);
					List<PaySplit> listPaySplits=new List<PaySplit>() { splitOffset,splitAllocate };
					incomeTransferData.ListSplitsCur.AddRange(listPaySplits);
					incomeTransferData.AppendLine($"    ^PaySplit created to move {splitOffset.SplitAmt.ToString("c")} from {GetPaySplitTypeDesc(splitOffset)}");
					incomeTransferData.AppendLine($"    ^PaySplit created to move {splitAllocate.SplitAmt.ToString("c")} to {GetPaySplitTypeDesc(splitAllocate)}");
					#endregion
					positiveEntry.AmountEnd-=amountTxfr;
					negativeEntry.AmountEnd+=amountTxfr;
					strBuilderSummary.AppendLine($"    Removed {amountTxfr.ToString("c")} from {positiveEntry.Description}");
					strBuilderSummary.AppendLine($"    Added {amountTxfr.ToString("c")} to {negativeEntry.Description}");
				}
			}
			return strBuilderSummary.ToString().TrimEnd();
		}

		///<summary>Method to encapsulate the creation of a new payment that is specifically meant to store payment information for the unallocated
		///payment transfer.</summary>
		public static long CreateAndInsertUnallocatedPayment(Patient patCur) {
			//No remoting role check; no call to db
			//user clicked ok and has permisson to save splits to the database. 
			Payment unallocatedTransferPayment=new Payment();
			unallocatedTransferPayment.PatNum=patCur.PatNum;
			unallocatedTransferPayment.PayDate=DateTime.Today;
			unallocatedTransferPayment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				unallocatedTransferPayment.ClinicNum=Clinics.ClinicNum;
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					unallocatedTransferPayment.ClinicNum=patCur.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					unallocatedTransferPayment.ClinicNum=(Clinics.ClinicNum==0 ? patCur.ClinicNum : Clinics.ClinicNum);
				}
			}
			unallocatedTransferPayment.DateEntry=DateTime.Today;
			unallocatedTransferPayment.PaymentSource=CreditCardSource.None;
			unallocatedTransferPayment.PayAmt=0;
			unallocatedTransferPayment.PayType=0;
			long payNum=Payments.Insert(unallocatedTransferPayment);
			return payNum;
		}

		///<summary>A helper method to get a description for the type of pay split that was passed in.  E.g. returns "Adjustment" if AdjNum > 0</summary>
		private static string GetPaySplitTypeDesc(PaySplit paySplit) {
			string offsetTypeName="Unallocated";
			if(paySplit.UnearnedType > 0) {
				offsetTypeName="Unearned";
			}
			else if(paySplit.ProcNum > 0) {
				offsetTypeName="Procedure";
			}
			else if(paySplit.AdjNum > 0) {
				offsetTypeName="Adjustment";
			}
			else if(paySplit.PayPlanChargeNum > 0) {
				offsetTypeName="PayPlanCharge";
			}
			else if(paySplit.PayPlanNum > 0) {
				offsetTypeName="PayPlan";
			}
			return offsetTypeName;
		}

		///<summary>Makes a "partial" income transfer that will only transfer patient payments between procedures associated to the claimprocs passed in and unearned.
		///Returns null if no payment was inserted into the database otherwise returns an object containing both the payment and the splits inserted.
		///Only attempts to make an income transfer if the IncomeTransfersMadeUponClaimReceived preference allows it.</summary>
		public static PayNumPaySplitsGroup MakeIncomeTransferForClaimProcs(long patNum,List<ClaimProc> listClaimProcs) {
			//No remoting role check; no call to db
			if(listClaimProcs.IsNullOrEmpty()) {
				return null;
			}
			if(!PrefC.MakeIncomeTransferOnClaimReceived()) {
				return null;
			}
			return MakeIncomeTransferForProcs(patNum,listClaimProcs.Select(x => x.ProcNum).ToList());
		}

		///<summary>Makes a "partial" income transfer that will only transfer money between procedures associated to the ProcNums passed in and unearned.
		///Returns null if no payment was inserted into the database otherwise returns an object containing both the payment and the splits inserted.</summary>
		public static PayNumPaySplitsGroup MakeIncomeTransferForProcs(long patNum,List<long> listProcNums) {
			//No remoting role check; no call to db
			if(listProcNums.IsNullOrEmpty()) {
				return null;
			}
			Patient patient=Patients.GetLim(patNum);
			//Get up to date data
			ConstructResults constructResults=ConstructAndLinkChargeCredits(patient.PatNum,isIncomeTxfr:true);
			//Get List of AccountEntries associated with the current claim
			List<AccountEntry> listAccountEntryProcs=constructResults.ListAccountEntries.FindAll(x => x.GetType()==typeof(Procedure) && listProcNums.Contains(x.ProcNum));
			//Create income transfer
			DateTime datePay=DateTime.Today;
			TryCreateIncomeTransfer(listAccountEntryProcs,datePay,out IncomeTransferData incomeTransferData);
			//Get the payment splits suggested by income transfer
			if(incomeTransferData.ListSplitsCur.IsNullOrEmpty()) {
				return null;
			}
			//Create a new payment associated to the suggested payment splits.
			Payment payment=new Payment();
			payment.PayDate=datePay;
			payment.PatNum=patient.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			payment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				payment.ClinicNum=Clinics.ClinicNum;
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					payment.ClinicNum=patient.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					payment.ClinicNum=(Clinics.ClinicNum==0 ? patient.ClinicNum : Clinics.ClinicNum);
				}
			}
			payment.DateEntry=DateTime.Today;
			payment.PaymentSource=CreditCardSource.None;
			payment.ProcessStatus=ProcessStat.OfficeProcessed;
			payment.PayAmt=0;
			payment.PayType=0;
			Payments.Insert(payment,incomeTransferData.ListSplitsCur);
			SecurityLogs.MakeLogEntry(EnumPermType.PaymentCreate,patient.PatNum,$"Income transfer automatically created upon claim received.");
			Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(patient.PatNum,incomeTransferData.ListSplitsCur);
			return new PayNumPaySplitsGroup(payment,incomeTransferData.ListSplitsCur);
		}
		#endregion

		#region Income Transfer FIFO

		///<summary>Returns a list of splits that should be inserted into the database in order to balance the patient's account using FIFO logic while ignoring patient, provider, and clinic mismatches.</summary>
		public static IncomeTransferData GetIncomeTransferDataFIFO(long patNum,DateTime datePay,DateTime dateAsOf=default) {
			List<PaySplit> listPaySplits=new List<PaySplit>();
			//Get the family account as it stands.
			Family fam=Patients.GetFamily(patNum);
			ConstructChargesData constructChargesData=GetConstructChargesData(patNum,listPatNums:fam.GetPatNums(),isIncomeTransfer:true);
			List<AccountEntry> listAccountEntries=ConstructListCharges(fam.GetPatNums(),
				constructChargesData.ListProcs,
				constructChargesData.ListAdjustments,
				constructChargesData.ListPaySplits,
				constructChargesData.ListInsPayAsTotal,
				constructChargesData.ListPayPlanCharges,
				constructChargesData.ListPayPlanLinks,
				true,
				constructChargesData.ListClaimProcsFiltered,
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum > 0),
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum==0));
			listAccountEntries.Sort(AccountEntrySort);
			if(dateAsOf.Year > 1880) {
				//Remove all account entries that fall after the 'as of date' passed in.
				listAccountEntries.RemoveAll(x => x.Date > dateAsOf);
			}
			//Run explicit linking logic so that account entries have a starting point for their AmountEnd values.
			listAccountEntries=ExplicitlyLinkCredits(listAccountEntries,constructChargesData.ListPaySplits);
			//Blindly apply the value for adjustments that have a ProcNum set to the corresponding procedure's value.
			List<AccountEntry> listAccountEntriesAdjsWithProcNum=listAccountEntries.FindAll(x => !CompareDecimal.IsZero(x.AmountEnd)
				&& x.ProcNum > 0 && x.GetType()==typeof(Adjustment));
			for(int i=0;i<listAccountEntriesAdjsWithProcNum.Count;i++) {
				AccountEntry accountEntryProc=listAccountEntries.FirstOrDefault(x => x.ProcNum==listAccountEntriesAdjsWithProcNum[i].ProcNum && x.GetType()==typeof(Procedure));
				if(accountEntryProc==null) {
					continue;
				}
				accountEntryProc.AmountEnd+=listAccountEntriesAdjsWithProcNum[i].AmountEnd;
				listAccountEntriesAdjsWithProcNum[i].AmountEnd=0;
			}
			//Allow implicitly linked payment splits to manipulate AmountEnd values of adjustments and procedures based on the corresponding FK columns.
			listAccountEntries=ApplyAssociatedSplits(listAccountEntries);
			//Transfer overpayments to unearned or zero out the value if negative production is not allowed to be treated as income.
			List<AccountEntry> listAccountEntriesOverpaid=listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd)
				&& ((x.ProcNum > 0 && x.GetType()==typeof(Procedure)) || (x.AdjNum > 0 && x.GetType()==typeof(Adjustment))));
			IncomeTransferData incomeTransferData=new IncomeTransferData();
			for(int i=0;i<listAccountEntriesOverpaid.Count;i++) {
				//Only move overpayments when negative production is allowed to be treated as income.
				if(PrefC.GetBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome)) {
					List<PaySplit> listPaySplitsOverpaid=CreateUnearnedTransfer(listAccountEntriesOverpaid[i],ref incomeTransferData,datePay);
					AccountEntry accountEntryOffset=new AccountEntry(listPaySplitsOverpaid[0]);
					AccountEntry accountEntryUnearned=new AccountEntry(listPaySplitsOverpaid[1]);
					//The offsetting PaySplit needs to have no value (untransferrable, a.k.a. AmountEnd set to zero).
					accountEntryOffset.AmountEnd=0;
					listAccountEntries.AddRange(new List<AccountEntry>() {
						accountEntryOffset,
						accountEntryUnearned,
					});
					listPaySplits.AddRange(listPaySplitsOverpaid);
				}
				//Either splits were added to move the overpayment to unearned or negative production cannot be treated as income. Regardless, zero out the production.
				listAccountEntriesOverpaid[i].AmountEnd=0;
			}
			//Separate the account entries into production and outstanding unearned/unallocated income.
			List<AccountEntry> listAccountEntriesOutstandingProduction=listAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd) && x.GetType()!=typeof(PaySplit));
			List<AccountEntry> listAccountEntriesUnallocated=listAccountEntries.FindAll(x => x.IsUnallocated);
			List<AccountEntry> listAccountEntriesUnearned=listAccountEntries.FindAll(x => x.IsUnearned);
			//Apply all negative and positive unearned/unallocated together in order to get an accurate account of what unearned/unallocated is right now.
			//Balance both unallocated and unearned lists (manipulate their AmountEnd to balance themselves out FIFO style).
			if(listAccountEntriesUnallocated.Count > 1) {
				BalanceAccountEntries(ref listAccountEntriesUnallocated);
			}
			if(listAccountEntriesUnearned.Count > 1) {
				if(PrefC.IsODHQ) {
					//Invoke the helper method that will balance individual uneared types and will suggest new splits.
					BalanceAccountEntriesByUnearnedType(ref listAccountEntriesUnearned,ref incomeTransferData,datePay);
				}
				else {
					//Invoke the helper method that will simply balance positive and negative account entries and will not suggest new splits.
					BalanceAccountEntries(ref listAccountEntriesUnearned);
				}
			}
			List<AccountEntry> listAccountEntriesUnallocatedUnearned=new List<AccountEntry>(listAccountEntriesUnallocated);
			listAccountEntriesUnallocatedUnearned.AddRange(listAccountEntriesUnearned);
			//Loop through the entire list of production account entries for the family FIFO style.
			for(int i=0;i<listAccountEntriesOutstandingProduction.Count;i++) {
				//Take from the entire list of unearned account entries FIFO style when a production entry needs money allocated to it.
				for(int j=0;j<listAccountEntriesUnallocatedUnearned.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(listAccountEntriesOutstandingProduction[i].AmountEnd)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(listAccountEntriesUnallocatedUnearned[j].AmountEnd)) {
						continue;
					}
					incomeTransferData=CreateTransferHelper(listAccountEntriesOutstandingProduction[i],listAccountEntriesUnallocatedUnearned[j],datePay);
					listPaySplits.AddRange(incomeTransferData.ListSplitsCur);
				}
			}
			IncomeTransferData retVal=new IncomeTransferData();
			retVal.ListSplitsCur=listPaySplits;
			//Look for any error messages that need to be displayed to the user after the income transfer.
			List<AccountEntry> listAccountEntryErrors=listAccountEntries.FindAll(x => x.ErrorMsg.Length > 0);
			for(int i=0;i<listAccountEntryErrors.Count;i++) {
				retVal.StringBuilderErrors.AppendLine(listAccountEntryErrors[i].ErrorMsg.ToString().Trim());
			}
			//Look for any warning messages that need to be displayed to the user after the income transfer.
			List<AccountEntry> listAccountEntryWarnings=listAccountEntries.FindAll(x => x.WarningMsg.Length > 0);
			for(int i=0;i<listAccountEntryWarnings.Count;i++) {
				retVal.StringBuilderWarnings.AppendLine(listAccountEntryWarnings[i].WarningMsg.ToString().Trim());
			}
			return retVal;
		}

		///<summary>Manipulates the AmountEnd of account entries that split entries are linked to while ignoring patient, provider, and clinic mismatches.</summary>
		private static List<AccountEntry> ApplyAssociatedSplits(List<AccountEntry> listAccountEntries) {
			//Separate the account entries into production and income (sans payment plans).
			List<AccountEntry> listAccountEntriesProduction=listAccountEntries.FindAll(x => (x.ProcNum > 0 && x.GetType()==typeof(Procedure)) || (x.AdjNum > 0 && x.GetType()==typeof(Adjustment)));
			List<AccountEntry> listAccountEntriesIncome=listAccountEntries.FindAll(x => x.IsPaySplitAttachedToProd);
			//Loop through the production and blindly apply all of the associated income.
			for(int i=0;i<listAccountEntriesProduction.Count;i++) {
				List<AccountEntry> listAccountEntriesIncomeForProd;
				if(listAccountEntriesProduction[i].GetType()==typeof(Procedure)) {
					listAccountEntriesIncomeForProd=listAccountEntriesIncome.FindAll(x => x.ProcNum==listAccountEntriesProduction[i].ProcNum);
				}
				else {//Adjustment
					listAccountEntriesIncomeForProd=listAccountEntriesIncome.FindAll(x => x.AdjNum==listAccountEntriesProduction[i].AdjNum);
				}
				for(int j=0;j<listAccountEntriesIncomeForProd.Count;j++) {
					listAccountEntriesProduction[i].AmountEnd+=listAccountEntriesIncomeForProd[j].AmountEnd;
					listAccountEntriesIncomeForProd[j].AmountEnd=0;
				}
			}
			//Payment plan charges are technically production but they need to be handled separately.
			//Older versions of Open Dental wouldn't always link splits to the charges themselves but instead to the payment plan as a whole.
			List<AccountEntry> listAccountEntriesPayPlanProd=listAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry));
			List<AccountEntry> listAccountEntriesPayPlanIncome=listAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit) && (x.PayPlanNum > 0 || x.PayPlanChargeNum > 0));
			//Loop through the payment plan production and apply all of the associated income.
			for(int i=0;i<listAccountEntriesPayPlanProd.Count;i++) {
				//Blindly apply all income that specifies a specific PayPlanChargeNum.
				List<AccountEntry> listAccountEntriesChargeIncome=listAccountEntriesPayPlanIncome.FindAll(x => x.PayPlanChargeNum==listAccountEntriesPayPlanProd[i].PayPlanChargeNum);
				for(int j=0;j<listAccountEntriesChargeIncome.Count;j++) {
					listAccountEntriesPayPlanProd[i].AmountEnd+=listAccountEntriesChargeIncome[j].AmountEnd;
					listAccountEntriesChargeIncome[j].AmountEnd=0;
				}
				//Apply leftover income to any charge that still needs income when the PayPlanNum matches up to the amount of oustanding production.
				List<AccountEntry> listAccountEntriesPlanIncome=listAccountEntriesPayPlanIncome.FindAll(x => x.PayPlanNum==listAccountEntriesPayPlanProd[i].PayPlanNum);
				for(int j=0;j<listAccountEntriesPlanIncome.Count;j++) {
					if(CompareDecimal.IsLessThanOrEqualToZero(listAccountEntriesPayPlanProd[i].AmountEnd)) {
						break;
					}
					decimal amtToAllocate=Math.Min(Math.Abs(listAccountEntriesPayPlanProd[i].AmountEnd),Math.Abs(listAccountEntriesPlanIncome[j].AmountEnd));
					listAccountEntriesPayPlanProd[i].AmountEnd+=amtToAllocate;
					listAccountEntriesPlanIncome[j].AmountEnd-=amtToAllocate;
				}
				//Leave the AmountEnd value of any payment plan payments that do not have any outstanding charges to apply to.
			}
			return listAccountEntries;
		}

		#endregion

		#region Insurance Overpayments

		///<summary>Combs through every claim associated to the family of the guarantor passed in and transfers any overpayments into patient payments.
		///Returns a helper object that contains details about what happened to the family during this process for logging and unit testing.</summary>
		public static InsOverpayResult TransferInsuranceOverpaymentsForFamily(Family family,DateTime payDate=default,long defNumPayType=0) {
			long guarantor=family.Guarantor.PatNum;
			long defNumUnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
			//This method is designed to mimic the Insurance Overpaid Report in the sense that it only looks at the values of claimprocs and procedures and nothing else.
			//The ultimate goal is to act like an insurance version of the re-auto split all payments tool.
			//Meaning that we will treat every single claim on the family as if insurance has paid nothing and then redistribute what insurance actually paid FIFO style to claimprocs.
			//Insurance estimates will be filled up first, followed by associated procedure fees, and finally any left over value will be transferred to unearned.
			//Negative supplemental claimprocs will be created and will be associated to the same procedure that the original claimproc is having value removed.
			InsOverpayResult insOverpayResult=new InsOverpayResult();
			//Get all of the account information for the family passed in.
			ConstructChargesData constructChargesData=GetConstructChargesData(guarantor,listPatNums:family.GetPatNums(),isIncomeTransfer:false);
			//Create a grouping of claims and their respective information.
			List<ClaimGroup> listClaimGroups=constructChargesData.ListClaimProcs
				.Where(x => x.ClaimNum > 0 && ClaimProcs.GetInsPaidStatuses().Contains(x.Status))
				.GroupBy(x => x.ClaimNum)
				.Select(x => new ClaimGroup(x.Key,x.ToList(),constructChargesData.ListProcs))
				.ToList();
			if(listClaimGroups.IsNullOrEmpty()) {
				return insOverpayResult;//Nothing to do.
			}
			//Make sure that there is at least one group of claimprocs that are associated to a procedure.
			//We cannot do anything for claims that don't have at least one procedure since we won't know if it's overpaid or not.
			List<long> listClaimNumsInvalid=listClaimGroups.Where(x => x.ListClaimProcGroups.All(y => y.ProcNum==0))
				.Select(x => x.ClaimNum)
				.ToList();
			if(!listClaimNumsInvalid.IsNullOrEmpty()) {
				//This is an error. There should never be such a thing as a claim without a procedure...
				insOverpayResult.StringBuilderErrors.AppendLine($"Error, the following claims are missing procedures or have none and are considered invalid:");
				insOverpayResult.StringBuilderErrors.AppendLine(string.Join("\r\n  ^ClaimNum: ",listClaimNumsInvalid));
				return insOverpayResult;
			}
			//Loop through each ClaimGroup and move insurance payment around as needed. Convert completely overpaid claims into patient payments.
			for(int i=0;i<listClaimGroups.Count;i++) {
				//Get the total amount of insurance payments and write offs to process.
				double totalInsPay=listClaimGroups[i].ListClaimProcGroups.Sum(x => x.InsPayAmt);
				double totalWriteOff=listClaimGroups[i].ListClaimProcGroups.Sum(x => x.WriteOff);
				#region Create Fake ClaimProcs
				//Create fake claimprocs for every ClaimProcGroup because we're about to recreate them all with "perfect FIFO style logic".
				//Meaning, we're going to act like each procedure has exactly one claimproc and that it was paid exactly as it was intended all at once.
				//These fake claimprocs will then be used to compare against the groupings and any differences will be used to suggest new supplemental claimprocs.
				for(int k=0;k<listClaimGroups[i].ListClaimProcGroups.Count;k++) {
					listClaimGroups[i].ListClaimProcsFake.Add(new ClaimProc() {
						ClaimNum=listClaimGroups[i].ClaimNum,
						ProcNum=listClaimGroups[i].ListClaimProcGroups[k].ProcNum,
						InsPayEst=listClaimGroups[i].ListClaimProcGroups[k].InsPayEst,
						WriteOffEst=listClaimGroups[i].ListClaimProcGroups[k].WriteOffEstimate
					});
				}
				#endregion
				#region Fill InsPayEst Amount
				//Fill InsPayAmt up to the InsPayEst amount.
				for(int k=0;k<listClaimGroups[i].ListClaimProcsFake.Count;k++) {
					if(CompareDouble.IsLessThanOrEqualToZero(totalInsPay)) {
						break;
					}
					double amountPaid=Math.Min(totalInsPay,listClaimGroups[i].ListClaimProcsFake[k].InsPayEst);
					if(CompareDouble.IsLessThanOrEqualToZero(amountPaid)) {
						continue;
					}
					listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt+=amountPaid;
					totalInsPay-=amountPaid;
				}
				#endregion
				#region Fill WriteOff Amount
				//Fill WriteOff up to the WriteOffEst / WriteOffEstOverride amount.
				for(int k=0;k<listClaimGroups[i].ListClaimProcsFake.Count;k++) {
					if(CompareDouble.IsLessThanOrEqualToZero(totalWriteOff)) {
						break;
					}
					double amountWriteOff=Math.Min(totalWriteOff,ClaimProcs.GetWriteOffEstimate(listClaimGroups[i].ListClaimProcsFake[k]));
					if(CompareDouble.IsLessThanOrEqualToZero(amountWriteOff)) {
						continue;
					}
					listClaimGroups[i].ListClaimProcsFake[k].WriteOff+=amountWriteOff;
					totalWriteOff-=amountWriteOff;
				}
				#endregion
				#region Fill ProcFee Amount
				//Fill InsPayAmt up to the associated procedure value.
				for(int k=0;k<listClaimGroups[i].ListClaimProcsFake.Count;k++) {
					if(CompareDouble.IsLessThanOrEqualToZero(totalInsPay)) {
						break;
					}
					ClaimProcGroup claimProcGroup=listClaimGroups[i].ListClaimProcGroups.FirstOrDefault(x => x.ProcNum==listClaimGroups[i].ListClaimProcsFake[k].ProcNum);
					Procedure procedure=claimProcGroup.Procedure;
					if(procedure==null || CompareDouble.IsLessThanOrEqualToZero(procedure.ProcFeeTotal)) {
						continue;
					}
					//Figure out how much of the totalInsPay amount can be applied to this claim procedure.
					double amountAlreadyPaid=listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt + listClaimGroups[i].ListClaimProcsFake[k].WriteOff;
					double amountAvailable=procedure.ProcFeeTotal - amountAlreadyPaid;
					if(CompareDouble.IsLessThanOrEqualToZero(amountAvailable)) {
						continue;
					}
					double amountToAdd=Math.Min(totalInsPay,amountAvailable);
					listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt+=amountToAdd;
					totalInsPay-=amountToAdd;
				}
				//Fill WriteOff up to the associated procedure value.
				for(int k=0;k<listClaimGroups[i].ListClaimProcsFake.Count;k++) {
					if(CompareDouble.IsLessThanOrEqualToZero(totalWriteOff)) {
						break;
					}
					ClaimProcGroup claimProcGroup=listClaimGroups[i].ListClaimProcGroups.FirstOrDefault(x => x.ProcNum==listClaimGroups[i].ListClaimProcsFake[k].ProcNum);
					Procedure procedure=claimProcGroup.Procedure;
					if(procedure==null || CompareDouble.IsLessThanOrEqualToZero(procedure.ProcFeeTotal)) {
						continue;
					}
					//Figure out how much of the totalWriteOff amount can be applied to this claim procedure.
					double amountAlreadyPaid=listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt + listClaimGroups[i].ListClaimProcsFake[k].WriteOff;
					double amountAvailable=procedure.ProcFeeTotal - amountAlreadyPaid;
					if(CompareDouble.IsLessThanOrEqualToZero(amountAvailable)) {
						continue;
					}
					double amountToAdd=Math.Min(totalWriteOff,amountAvailable);
					listClaimGroups[i].ListClaimProcsFake[k].WriteOff+=amountToAdd;
					totalWriteOff-=amountToAdd;
				}
				#endregion
				#region Create Supplemental ClaimProcs
				List<ClaimProc> listClaimProcSupplementals=new List<ClaimProc>();
				for(int k=0;k<listClaimGroups[i].ListClaimProcsFake.Count;k++) {
					ClaimProcGroup claimProcGroup=listClaimGroups[i].ListClaimProcGroups.First(x => x.ProcNum==listClaimGroups[i].ListClaimProcsFake[k].ProcNum);
					if(listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt!=claimProcGroup.InsPayAmt || listClaimGroups[i].ListClaimProcsFake[k].WriteOff!=claimProcGroup.WriteOff) {
						//Create a supplemental claimproc that negates the difference between the new and old claimprocs.
						//Simply use the first claimproc within the grouping since they are all associated to the same procedure and claim.
						ClaimProc claimProcSupplemental=ClaimProcs.CreateSuppClaimProcForTransfer(claimProcGroup.ListClaimProcs.First());
						//Purposefully toggle off IsTransfer since these are technically not transfer claimprocs (no new claimproc is going to be made to offset it).
						claimProcSupplemental.IsTransfer=false;
						//This new claimproc will show up in the Account module since IsTransfer is turned off so we need to set a date that makes sense.
						//Simply use maximum DateCP within this group of claimprocs.
						claimProcSupplemental.DateCP=claimProcGroup.ListClaimProcs.Max(x => x.DateCP);
						//Set the InsPayAmt and WriteOff accordingly.
						claimProcSupplemental.InsPayAmt=listClaimGroups[i].ListClaimProcsFake[k].InsPayAmt - claimProcGroup.InsPayAmt;
						claimProcSupplemental.WriteOff=listClaimGroups[i].ListClaimProcsFake[k].WriteOff - claimProcGroup.WriteOff;
						claimProcSupplemental.Remarks="Conv Ins Overpayment";
						listClaimProcSupplementals.Add(claimProcSupplemental);
					}
				}
				#endregion
				#region Validation
				//Move onto the next claim if no supplemental claimprocs were suggested. We CANNOT make unearned payments if no counteracting supplementals are suggested.
				if(listClaimProcSupplementals.IsNullOrEmpty()) {
					continue;
				}
				//Fail if there is any write off value left over because this means that the office wrote off more than they should have.
				//The office will have to manually fix the claim(s) for this family before overpayments can be transferred over to patient payments.
				if(CompareDecimal.IsGreaterThanZero(totalWriteOff)) {
					insOverpayResult.StringBuilderErrors.AppendLine($"Error processing ClaimNum {listClaimGroups[i].ClaimNum}; Too much has been written off.");
					continue;
				}
				//Assert that the value of the supplementals suggested offsets the value of insurance overpayment.
				double totalSupplementals=listClaimProcSupplementals.Sum(x => x.InsPayAmt + x.WriteOff);
				double totalSupplementalsNegated=totalSupplementals * -1;
				if(!CompareDouble.IsEqual(totalSupplementalsNegated,totalInsPay)) {
					insOverpayResult.StringBuilderErrors.AppendLine($"Error processing ClaimNum {listClaimGroups[i].ClaimNum}; Unexpected total for suggested supplementals.");
					insOverpayResult.StringBuilderErrors.AppendLine($"  ^Expected {totalInsPay} but suggested {totalSupplementalsNegated}");//Do not format the numbers on purpose.
					continue;
				}
				#endregion
				#region Patient Payment to Offset Supplementals
				//Make a payment to unearned for the overpayment value (what remains in totalInsPay).
				if(CompareDecimal.IsGreaterThanZero(totalInsPay)) {
					if(payDate.Year < 1880) {
						payDate=DateTime.Now;
					}
					Payment payment=new Payment() {
						PatNum=guarantor,
						PayAmt=totalInsPay,
						PaymentSource=CreditCardSource.None,
						PayDate=payDate,
						PaymentStatus=PaymentStatus.None,
						PayType=defNumPayType,
						PayNote=$"Created via Family Balancer Tool in order to convert insurance overpayment from ClaimNum {listClaimGroups[i].ClaimNum} into patient payment."
					};
					//Do not copy this pattern of manually making a payment split. You should be invoking CreatePaySplitHelper() instead.
					//However, this payment split is unique in that it is not counteracting a specific account entry and is instead a standalone unearned payment.
					List<PaySplit> listPaySplits=new List<PaySplit>() {
						new PaySplit() {
							AdjNum=0,
							ClinicNum=0,
							DatePay=payDate,
							IsNew=true,
							PatNum=guarantor,
							PayNum=0,//This will be set by consuming methods if they decide to insert these suggested payments into the database.
							ProcNum=0,
							ProvNum=0,
							SplitAmt=totalInsPay,
							UnearnedType=defNumUnearnedType,
						}
					};
					insOverpayResult.StringBuilderVerbose.AppendLine($"ClaimNum {listClaimGroups[i].ClaimNum} has been overpaid by {totalInsPay:C}");
					insOverpayResult.StringBuilderVerbose.AppendLine($"  ^Created {listClaimProcSupplementals.Count} new supplemental claimproc(s).");
					insOverpayResult.StringBuilderVerbose.AppendLine($"  ^Created {listPaySplits.Count} unearned payment split(s).");
					insOverpayResult.ListPayNumPaySplitsGroups.Add(new PayNumPaySplitsGroup(payment,listPaySplits));
					insOverpayResult.ListClaimProcSupplementals.AddRange(listClaimProcSupplementals);
				}
				#endregion
			}
			return insOverpayResult;
		}

		///<summary>Makes the InsOverpayResults official by inserting all suggested supplemental claimprocs and patient payments into the database.</summary>
		public static void InsertInsOverpayResult(InsOverpayResult insOverpayResult,string logText="") {
			//No need to check MiddleTierRole; no call to db.
			if(insOverpayResult.ListClaimProcSupplementals.IsNullOrEmpty()) {
				return;
			}
			//Insert the supplemental claimprocs. This InsertMany method invokes the Insert method for each claimproc instead of invoking the CRUD.InsertMany method.
			//This means that the list that is returned will have the PKs set correctly.
			insOverpayResult.ListClaimProcSupplementals=ClaimProcs.InsertMany(insOverpayResult.ListClaimProcSupplementals);
			//Group up the claimprocs by ClaimNum in order to make an insurance check per claim.
			List<ClaimGroup> listClaimGroups=insOverpayResult.ListClaimProcSupplementals
				.GroupBy(x => x.ClaimNum)
				.ToDictionary(x => x.Key,x => x.ToList())
				.Select(x => new ClaimGroup(x.Key,x.Value))
				.ToList();
			//Create and insert insurance checks for the supplemental claimprocs that were just inserted.
			for(int i=0;i<listClaimGroups.Count;i++) {
				ClaimPayment claimPayment=new ClaimPayment();
				claimPayment.CheckDate=listClaimGroups[i].ListClaimProcs.First().DateCP;
				claimPayment.CheckAmt=listClaimGroups[i].ListClaimProcs.Sum(x => x.InsPayAmt);
				claimPayment.ClinicNum=listClaimGroups[i].ListClaimProcs.First().ClinicNum;
				//Use a dummy carrier name so that it doesn't look like the carrier actually refunded the overpayment.
				claimPayment.CarrierName="Family Balancer";
				claimPayment.PayType=Defs.GetFirstForCategory(DefCat.InsurancePaymentType,true).DefNum;
				claimPayment.Note="Conv Ins Overpayment";
				ClaimPayments.Insert(claimPayment);
				for(int j=0;j<listClaimGroups[i].ListClaimProcs.Count;j++) {
					listClaimGroups[i].ListClaimProcs[j].ClaimPaymentNum=claimPayment.ClaimPaymentNum;
					ClaimProcs.Update(listClaimGroups[i].ListClaimProcs[j]);
				}
			}
			//Insert the unearned payments.
			for(int i=0;i<insOverpayResult.ListPayNumPaySplitsGroups.Count;i++) {
				Payments.Insert(insOverpayResult.ListPayNumPaySplitsGroups[i].Payment,insOverpayResult.ListPayNumPaySplitsGroups[i].ListPaySplits);
				SecurityLogs.MakeLogEntry(EnumPermType.PaymentCreate,insOverpayResult.ListPayNumPaySplitsGroups[i].Payment.PatNum,logText);
			}
		}

		#endregion

		#region Data Classes
		///<summary>The data needed to load FormPayment.</summary>
		[Serializable]
		public class LoadData {
			public Patient PatCur;
			public Family Fam;
			public Family SuperFam;
			public Payment PaymentCur;
			public List<CreditCard> ListCreditCards;
			public XWebResponse XWebResponse;
			public PayConnectResponseWeb PayConnectResponseWeb;
			public CareCreditWebResponse CareCreditWebResponse;
			[XmlIgnore]
			public DataTable TableBalances;
			///<summary>List of splits associated to this payment</summary>
			public List<PaySplit> ListSplits;
			public Transaction Transaction;
			public List<PayPlan> ListValidPayPlans;
			public List<Patient> ListAssociatedPatients;
			public ConstructChargesData ConstructChargesData;
			public List<Procedure> ListProcsForSplits;

			[XmlElement(nameof(TableBalances))]
			public string TableBalancesXml {
				get {
					if(TableBalances==null) {
						return null;
					}
					return XmlConverter.TableToXml(TableBalances);
				}
				set {
					if(value==null) {
						TableBalances=null;
						return;
					}
					TableBalances=XmlConverter.XmlToTable(value);
				}
			}

			public LoadData Copy() {
				LoadData loadData=(LoadData)this.MemberwiseClone();
				loadData.ConstructChargesData=this.ConstructChargesData.Copy();
				loadData.ListAssociatedPatients=this.ListAssociatedPatients.Select(x => x.Copy()).ToList();
				loadData.ListCreditCards=this.ListCreditCards.Select(x => x.Copy()).ToList();
				loadData.ListProcsForSplits=this.ListProcsForSplits.Select(x => x.Copy()).ToList();
				loadData.ListSplits=this.ListSplits.Select(x => x.Copy()).ToList();
				loadData.ListValidPayPlans=this.ListValidPayPlans.Select(x => x.Copy()).ToList();
				return loadData;
			}
		}

		///<summary>The data needed to construct a list of charges for FormPayment.</summary>
		[Serializable]
		public class ConstructChargesData {
			///<summary>List from the db, completed for pat. Not list of pre-selected procs from acct. Contains TP procs if pref is set to ON.</summary>
			public List<Procedure> ListProcs=new List<Procedure>();
			public List<Adjustment> ListAdjustments=new List<Adjustment>();
			///<summary>Current list of all splits from database</summary>
			public List<PaySplit> ListPaySplits=new List<PaySplit>();
			///<summary>Stores the summed outstanding ins pay as totals (amounts and write-offs) for the list of patnums</summary>
			public List<PayAsTotal> ListInsPayAsTotal=new List<PayAsTotal>();
			public List<PayPlan> ListPayPlans=new List<PayPlan>();
			public List<PaySplit> ListPayPlanSplits=new List<PaySplit>();
			///<summary>List of all pay plan charges (including future charges).  Does not include insurance pay plan charges.</summary>
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
			///<summary>All of the claimprocs (including ins pay as totals) for the list of pat nums</summary>
			public List<ClaimProc> ListClaimProcs=new List<ClaimProc>();
			///<summary>Stores the list of claimprocs (not ins pay as totals) for the list of pat nums</summary>
			public List<ClaimProc> ListClaimProcsFiltered=new List<ClaimProc>();
			///<summary>List of all pay plan links for the pay plans.</summary>
			public List<PayPlanLink> ListPayPlanLinks=new List<PayPlanLink>();

			public ConstructChargesData Copy() {
				ConstructChargesData constructChargesData=new ConstructChargesData();
				constructChargesData.ListAdjustments=this.ListAdjustments.Select(x => x.Clone()).ToList();
				constructChargesData.ListClaimProcsFiltered=this.ListClaimProcsFiltered.Select(x => x.Copy()).ToList();
				constructChargesData.ListInsPayAsTotal=this.ListInsPayAsTotal.Select(x => x.Copy()).ToList();
				constructChargesData.ListPayPlanCharges=this.ListPayPlanCharges.Select(x => x.Copy()).ToList();
				constructChargesData.ListPayPlanLinks=this.ListPayPlanLinks.Select(x => x.Copy()).ToList();
				constructChargesData.ListPayPlans=this.ListPayPlans.Select(x => x.Copy()).ToList();
				constructChargesData.ListPayPlanSplits=this.ListPayPlanSplits.Select(x => x.Copy()).ToList();
				constructChargesData.ListPaySplits=this.ListPaySplits.Select(x => x.Copy()).ToList();
				constructChargesData.ListProcs=this.ListProcs.Select(x => x.Copy()).ToList();
				return constructChargesData;
			}
		}

		///<summary>Data retrieved upon initialization. AutpSplit stores data retireved from going through list of charges, linking,and autosplitting.</summary>
		[Serializable]
		public class InitData {
			public AutoSplit AutoSplitData;
			public Dictionary<long,Patient> DictPats=new Dictionary<long,Patient>();
			public decimal SplitTotal;
		}

		///<summary>Data resulting after making a payment.</summary>
		[Serializable]
		public class PayResults {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
		}

		///<summary>Data results after constructing list of charges and linking credits to them.</summary>
		[Serializable]
		public class ConstructResults {
			public long PatNum;
			public long ClinicNum;
			public double PayAmt;
			public long PayNum;
			public DateTime PayDate;
			public List<PaySplit> ListPaySplits=new List<PaySplit>();
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();

			public ConstructResults(long patNum,long clinicNum,double payAmt,long payNum,DateTime payDate,List<PaySplit> listPaySplitsForPayment=null,
				List<AccountEntry> listAccountEntries=null)
			{
				PayAmt=payAmt;
				PayDate=payDate;
				PatNum=patNum;
				ClinicNum=clinicNum;
				PayNum=payNum;
				if(listPaySplitsForPayment!=null) {
					ListPaySplits=listPaySplitsForPayment;
				}
				if(listAccountEntries!=null) {
					ListAccountEntries=listAccountEntries;
				}
			}
		}

		///<summary>Data used for automatically suggesting payment splits.</summary>
		[Serializable]
		public class AutoSplit {
			public long PatNum;
			public long ClinicNum;
			public double PayAmt;
			public long PayNum;
			public DateTime PayDate;
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();
			public List<PaySplit> ListPaySplitsForPayment=new List<PaySplit>();
			///<summary>A list of payment splits designed to get filled after invoking auto-split logic.</summary>
			public List<PaySplit> ListPaySplitsSuggested=new List<PaySplit>();

			public AutoSplit(ConstructResults constructResults) 
				: this(constructResults.PatNum,
						constructResults.ClinicNum,
						constructResults.PayAmt,
						constructResults.PayNum,
						constructResults.PayDate,
						constructResults.ListPaySplits,
						constructResults.ListAccountEntries)
			{
			}

			public AutoSplit(long patNum,long clinicNum,double payAmt,long payNum,DateTime payDate,List<PaySplit> listPaySplitsForPayment=null,
				List<AccountEntry> listAccountEntries=null)
			{
				PayAmt=payAmt;
				PayDate=payDate;
				PatNum=patNum;
				ClinicNum=clinicNum;
				PayNum=payNum;
				if(listPaySplitsForPayment!=null) {
					ListPaySplitsForPayment=listPaySplitsForPayment;
					//Users can select production from within the Account module to indicate what should be "paid first".
					//Some payment splits will have already been suggested just prior to executing the implicit linking logic.
					ListPaySplitsSuggested=new List<PaySplit>(ListPaySplitsForPayment);
				}
				if(listAccountEntries!=null) {
					ListAccountEntries=listAccountEntries;
				}
			}
		}

		///<summary>Keeps the list of splits that are being transferred, and a bool telling if one or more that was attempted are invalid.</summary>
		[Serializable]
		public class IncomeTransferData {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			///<summary>Set to true when an income transfer tried to move unearned to a provider and the pref AllowPrepayProvider is off.</summary>
			public bool HasInvalidSplits=false;
			///<summary>Set to true when the income transfer tried to move negative production to unearned and the office does not want to allow treating negative production as income.</summary>
			public bool HasInvalidNegProd=false;
			public StringBuilder StringBuilderErrors=new StringBuilder();
			public StringBuilder StringBuilderWarnings=new StringBuilder();
			private StringBuilder _stringBuilderSummary=new StringBuilder();
			
			///<summary>This is a detailed log of what happened during the transfer process.  This text is designed to help explain the transfer.</summary>
			public string SummaryText {
				get {
					return _stringBuilderSummary.ToString().TrimEnd();
				}
			}

			///<summary>Appends text along with a new line at the end of it to Summary.  Does nothing if the text passed in is null or empty.</summary>
			public void AppendLine(string text) {
				if(string.IsNullOrEmpty(text)) {
					return;
				}
				_stringBuilderSummary.AppendLine(text);
			}

			///<summary>Merges given data list into this objects assocaited list.</summary>
			public void MergeIncomeTransferData(IncomeTransferData data) {
				this.ListSplitsCur.AddRange(data.ListSplitsCur);
				this.HasInvalidSplits|=data.HasInvalidSplits;
				this.HasInvalidNegProd|=data.HasInvalidNegProd;
				AppendLine(data.SummaryText);
			}

			///<summary>Creates the negative and positive splits from a given parentSplit and a payment's payNum. When isTransferToUnearned is true, these
			///splits will transfer to the user's unearned type based on preference values. If transferAmtOverride is given a value, it will be used for the
			///split amounts in place of the parentSplit.SplitAmt.</summary>
			public static IncomeTransferData CreateTransfer(PaySplit parentSplit,long payNum,bool isTransferToUnearned=false
				,double transferAmtOverride=0)
			{
				IncomeTransferData transferReturning=new IncomeTransferData();
				AccountEntry accountEntrySplit=new AccountEntry(parentSplit);
				double offsetSplitAmt=(transferAmtOverride==0 ? parentSplit.SplitAmt : transferAmtOverride)*-1;
				double allocationSplitAmt=transferAmtOverride==0 ? parentSplit.SplitAmt : transferAmtOverride;
				long allocationUnearnedType=isTransferToUnearned ? PrefC.GetLong(PrefName.PrepaymentUnearnedType) : parentSplit.UnearnedType;
				PaySplit splitOffset=CreatePaySplitHelper(accountEntrySplit,offsetSplitAmt,DateTime.Today,unearnedType:parentSplit.UnearnedType);
				PaySplit splitAllocate=CreatePaySplitHelper(accountEntrySplit,allocationSplitAmt,DateTime.Today,unearnedType:allocationUnearnedType);
				transferReturning.ListSplitsCur.AddRange(new List<PaySplit>() {
					splitOffset,
					splitAllocate,
				});
				return transferReturning;
			}
		}

		///<summary>Storage class for organizing account entries when processing income transfer layers.</summary>
		public class Bucket {
			///<summary>All account entries for the current bucket.</summary>
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();

			public Bucket(List<AccountEntry> listAccountEntries) {
				ListAccountEntries=listAccountEntries;
			}

			///<summary>All account entries that have an AmountEnd greater than zero.</summary>
			public List<AccountEntry> ListPositiveEntries {
				get {
					return ListAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
				}
			}

			///<summary>All account entries that have an AmountEnd less than zero.</summary>
			public List<AccountEntry> ListNegativeEntries {
				get {
					return ListAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
				}
			}

		}

		///<summary>Helper class for organizing entities involved with implicitly linking account entries.</summary>
		public class ImplicitLinkBucket {
			///<summary>All account entries for the current bucket.</summary>
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();
			///<summary>All PayAsTotals for the current bucket.</summary>
			public List<PayAsTotal> ListInsPayAsTotal=new List<PayAsTotal>();
			///<summary>All PaySplits for the current bucket.</summary>
			public List<PaySplit> ListPaySplits=new List<PaySplit>();

			public ImplicitLinkBucket(List<AccountEntry> listAccountEntries) {
				ListAccountEntries=listAccountEntries;
			}
		}

		///<summary>Represents ways of grouping up account entities.  Each layer represents how account entries should be grouped for balancing.
		///It is critical that each layer be considered in the EXACT order that this enumeration is constructed when balancing an account.</summary>
		private enum AccountBalancingLayers {
			///<summary>0 - Creates buckets grouped by provider, patient, and clinic for balancing.  Arguably the most important layer.
			///This is the only layer that is explicit enough to transfer money to unearned.
			///Transferring money to unearned should happen prior to processing the income transfer buckets that this layer creates.</summary>
			ProvPatClinic,
			///<summary>1 - Creates buckets grouped by provider and patient for balancing.</summary>
			ProvPat,
			///<summary>2 - Creates buckets grouped by provider and clinic for balancing.</summary>
			ProvClinic,
			///<summary>3 - Creates buckets grouped by patient and clinic for balancing.</summary>
			PatClinic,
			///<summary>4 - Creates buckets grouped by provider for balancing.</summary>
			Prov,
			///<summary>5 - Creates buckets grouped by patient for balancing.</summary>
			Pat,
			///<summary>6 - Creates buckets grouped by clinic for balancing.</summary>
			Clinic,
			///<summary>7 - Creates buckets grouped by nothing for balancing.</summary>
			Nothing,
			///<summary>8 - Does not create any bucket for balancing.  This is another special layer and it should always be considered last.
			///It will move any leftover money that the previous layers could not address and will move it over to unearned for balancing.</summary>
			Unearned,
		}

		private class AdjustmentSplitGroup {
			public long AdjNum;
			public List<PaySplit> ListPaySplits;
		}

		private class PayPlanSplitGroup {
			public long PayPlanNum;
			public List<PaySplit> ListPaySplits;
		}

		private class ProcedureSplitGroup {
			public long ProcNum;
			public List<PaySplit> ListPaySplits;
		}

		///<summary>A claim and its associated claimprocs.</summary>
		private class ClaimGroup {
			public long ClaimNum;
			///<summary>A shallow, read only list of all claim procedures associated to this claim. Not designed to be manipulated.</summary>
			public List<ClaimProc> ListClaimProcs;
			///<summary>A shallow, read only list of all claim procedures associated to this claim grouped up by ProcNum. Not designed to be manipulated.</summary>
			public List<ClaimProcGroup> ListClaimProcGroups;
			///<summary>A list of fake claimprocs created by filling up the claim in a specific FIFO manner.</summary>
			public List<ClaimProc> ListClaimProcsFake=new List<ClaimProc>();

			public ClaimGroup(long claimNum,List<ClaimProc> listClaimProcs,List<Procedure> listProcedures=null) {
				ClaimNum=claimNum;
				ListClaimProcs=new List<ClaimProc>(listClaimProcs);
				if(listProcedures==null) {
					listProcedures=new List<Procedure>();
				}
				ListClaimProcGroups=listClaimProcs.OrderBy(y => y.LineNumber)
					.GroupBy(x => x.ProcNum)
					.Select(x => new ClaimProcGroup(x.ToList(),listProcedures.FirstOrDefault(y => y.ProcNum==x.Key)))
					.ToList();
			}
		}

		///<summary>A group of claimprocs that are all associated to the same claim and the same procedure.</summary>
		private class ClaimProcGroup {
			///<summary>A group of claimprocs that are all associated to the same procedure (or no procedure).</summary>
			public List<ClaimProc> ListClaimProcs;
			///<summary>The procedure that all of the claimprocs are associated to. Null when the claimprocs are not associated to a procedure.</summary>
			public Procedure Procedure;
			///<summary>The ProcNum of the procedure that this grouping of claimprocs is associated to. 0 when not associated to any procedure.</summary>
			public long ProcNum => Procedure?.ProcNum??0;
			///<summary>The sum of InsPayAmt for all claimprocs in this grouping.</summary>
			public double InsPayAmt => ListClaimProcs.Sum(x => x.InsPayAmt);
			///<summary>The sum of InsPayEst for all claimprocs in this grouping.</summary>
			public double InsPayEst => ListClaimProcs.Sum(x => x.InsPayEst);
			///<summary>The sum of write-off for all claimprocs in this grouping.</summary>
			public double WriteOff => ListClaimProcs.Sum(x => x.WriteOff);
			///<summary>The sum of write-off estimate for all claimprocs in this grouping.</summary>
			public double WriteOffEstimate => ListClaimProcs.Sum(x => ClaimProcs.GetWriteOffEstimate(x));

			public ClaimProcGroup(List<ClaimProc> listClaimProcs,Procedure procedure) {
				ListClaimProcs=listClaimProcs;
				Procedure=procedure;
			}
		}

		///<summary>A group of FauxAccountEntry objects that are all associated with the same payment plan.</summary>
		private class PayPlanNumFauxAccountEntriesGroup {
			public long PayPlanNum;
			public List<FauxAccountEntry> ListFauxAccountEntries;
		}

		#endregion
	}

	///<summary>Holds information in regards to what happened during the process of transferring insurance overpayments into patient payments.</summary>
	public class InsOverpayResult {
		public List<ClaimProc> ListClaimProcSupplementals=new List<ClaimProc>();
		public List<PayNumPaySplitsGroup> ListPayNumPaySplitsGroups=new List<PayNumPaySplitsGroup>();
		///<summary>A user friendly description of errors that happened during the process of transferring insurance overpayments into patient payments.</summary>
		public StringBuilder StringBuilderErrors=new StringBuilder();
		///<summary>A user friendly description of what happened during the process of transferring insurance overpayments into patient payments.</summary>
		public StringBuilder StringBuilderVerbose=new StringBuilder();
	}

	///<summary>Holds three dictionaries containing the procedures, adjustments not attached to procedures, and payplancharge debits on a family's account. The dictionary keys are the Fkeys to the production items, and the dictionary values are the remaining balance of the production items. Unearned, adjustments, and insurance estimates for created claims are taken into account.</summary>
	public class FamilyProdBalances {
		///<summary>Holds all of the account entries for a family</summary>
		private List<AccountEntry> _listAccountEntries=new List<AccountEntry>();
		public long GuarantorPatNum;

		///<summary></summary>
		public FamilyProdBalances(long guarantorPatNum) {
			_listAccountEntries=PaymentEdit.ConstructAndLinkChargeCredits(guarantorPatNum).ListAccountEntries;
			GuarantorPatNum=guarantorPatNum;
		}

		///<summary>Returns decimal value of the found statmentProd for the matching type of statment production, Procedure, Adjustment, or PayPlanCharge. Returns zero if no match is found. Removes matched AccountEntries to ensure we don't make multiple late charges for a single entry.</summary>
		public decimal GetAmountForStatementProdIfExists(StatementProd statementProd) {
			//No need to check MiddleTierRole; no call to db.
			decimal amount=0;
			for(int i=0;i<_listAccountEntries.Count;i++){
				switch(statementProd.ProdType){
					case ProductionType.Procedure:
						if(_listAccountEntries[i].GetType()==typeof(Procedure) && _listAccountEntries[i].ProcNum==statementProd.FKey){
							amount=_listAccountEntries[i].AmountEnd;
							// remove AccountEntry once found
							_listAccountEntries.Remove(_listAccountEntries[i]);
							return amount;
						}
						break;
					case ProductionType.Adjustment:
						if(_listAccountEntries[i].GetType()==typeof(Adjustment) && _listAccountEntries[i].ProcNum==statementProd.FKey){
							amount=_listAccountEntries[i].AmountEnd;
							// remove AccountEntry once found
							_listAccountEntries.Remove(_listAccountEntries[i]);
							return amount;
						}
						break;
					case ProductionType.PayPlanCharge:
						if(_listAccountEntries[i].GetType()==typeof(FauxAccountEntry) 
							&& ((FauxAccountEntry)_listAccountEntries[i]).ChargeType==PayPlanChargeType.Debit
							&& _listAccountEntries[i].PayPlanChargeNum==statementProd.FKey)
						{//There can be multiple FauxAccountEntries for a single PayPlanCharge, so we sum the AmountEnds as we iterate over the list.
							amount+=_listAccountEntries[i].AmountEnd;
							_listAccountEntries.Remove(_listAccountEntries[i]);
						}
						break;
				}
			}
			return amount;
		}

	}

	[Serializable]
	public class PayNumPaySplitsGroup {
		public long PayNum;
		public Payment Payment;
		public List<PaySplit> ListPaySplits;
		///<summary>The sum of all payment splits instead of Payment.PayAmt.</summary>
		public double PayAmount;

		///<summary>Only for serialization purposes.</summary>
		public PayNumPaySplitsGroup() {
		}

		///<summary>Payment object cannot be null.</summary>
		public PayNumPaySplitsGroup(Payment payment,List<PaySplit> listPaySplits) : this(payment.PayNum,payment,listPaySplits) {
		}

		public PayNumPaySplitsGroup(long payNum,Payment payment,List<PaySplit> listPaySplits) {
			PayNum=payNum;
			Payment=payment;
			ListPaySplits=listPaySplits;
			PayAmount=ListPaySplits.Sum(x => x.SplitAmt);
		}
	}

}
