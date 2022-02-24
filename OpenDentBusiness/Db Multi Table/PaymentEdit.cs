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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
				if(data.ListSplits.Any(x => !ListTools.In(x.PatNum,listFamilyPatNums) && ListTools.In(x.PatNum,data.SuperFam.GetPatNums()))) {
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
				.Where(x => !ListTools.In(x,listPatNums))
				.Distinct()
				.ToList();
			//Add patients that are guarantors of payment plan charges for this patient just in case they are outside of the family.
			listUnknownPatNums.AddRange(loadData.ConstructChargesData.ListPayPlanCharges.Select(x => x.Guarantor)
				.Where(x => !ListTools.In(x,listPatNums))
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
			initData.AutoSplitData=AutoSplitForPayment(listPatNums,loadData.PatCur.PatNum,loadData.ListSplits,loadData.PaymentCur,listPayFirstAcctEntries,
				isIncomeTxfr,isPatPrefer,loadData,doAutoSplit,doIncludeExplicitCreditsOnly);
			loadData.ListSplits.AddRange(initData.AutoSplitData.ListAutoSplits);
			initData.AutoSplitData.ListSplitsCur=loadData.ListSplits.Union(initData.AutoSplitData.ListAutoSplits).ToList();
			return initData;
		}
		#endregion

		#region constructing and linking charges and credits
		///<summary>Gets the data needed to construct a list of charges on FormPayment.</summary>
		public static ConstructChargesData GetConstructChargesData(List<long> listPatNums,long patNum,List<PaySplit> listSplitsCur,long payCurNum
			,bool isIncomeTransfer,bool doIncludeTreatmentPlanned=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ConstructChargesData>(MethodBase.GetCurrentMethod(),listPatNums,patNum,listSplitsCur,payCurNum,isIncomeTransfer,doIncludeTreatmentPlanned);
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
				data.ListPaySplits.RemoveAll(x => x.UnearnedType!=0 && x.ProcNum!=0 && !ListTools.In(x.ProcNum,data.ListProcs.Select(y => y.ProcNum)));
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
				data.ListPaySplits.AddRange(data.ListPayPlanSplits.FindAll(x => ListTools.In(x.SplitNum,listMissingSplitNums)));
				listSplitNums.AddRange(listMissingSplitNums);
			}
			//Look for splits that are in the database yet have been deleted from the pay splits grid.
			//If we have a split that's not found in the passed-in list of splits for the payment
			//and the split we got from the DB is on this payment, remove it because the user must have deleted the split from the payment window.
			//The payment window won't update the DB with the change until it's closed.
			List<long> listRemoveSplitNums=listSplitsCur.FindAll(x => x.PayNum==payCurNum && !ListTools.In(x.SplitNum,listSplitNums))
				.Select(x => x.SplitNum).ToList();
			data.ListPaySplits.RemoveAll(x => ListTools.In(x.SplitNum,listRemoveSplitNums));
			//In the rare case that a Payment Plan has a Guarantor outside of the Patient's family, we want to make sure to collect more 
			//data for Adjustments and ClaimProcs so that debits and credits balance properly.
			if(data.ListPayPlans.Any(x => x.Guarantor!=x.PatNum && !ListTools.In(x.PatNum,listPatNums))) {
				data.ListAdjustments=Adjustments.GetForProcs(data.ListProcs.Select(x => x.ProcNum).ToList());
				//still does not contain PayAsTotals
				data.ListClaimProcsFiltered=ClaimProcs.GetForProcs(data.ListProcs.FindAll(x => x.ProcNum!=0).Select(x => x.ProcNum).ToList());
			}
			else {//Otherwise get the smaller dataset as it will suffice for our needs.
				data.ListAdjustments=Adjustments.GetAdjustForPats(listPatNums);
				data.ListClaimProcsFiltered=ClaimProcs.Refresh(listPatNums).Where(x => x.ProcNum!=0).ToList();//does not contain PayAsTotals
			}
			return data;
		}

		///<summary>Gets the charges and links credits for the patient. Includes family account entries if listPatNums left null.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(long patNum,List<long> listPatNums=null,bool isIncomeTxfr=false) {
			//No remoting role check; no call to db
			if(listPatNums==null) {
				listPatNums=Patients.GetFamily(patNum).GetPatNums();
			}
			return ConstructAndLinkChargeCredits(listPatNums,patNum,new List<PaySplit>(),new Payment(),new List<AccountEntry>(),
				isIncomeTxfr:isIncomeTxfr);
		}

		/// <summary>Helper method that does the entire original auto split for payment. Gets the charges, and links the credits. Optionally set doIncludeTreatmentPlanned to true to 
		/// force include TP procs in the logic.</summary>
		public static ConstructResults ConstructAndLinkChargeCredits(List<long> listPatNums,long patCurNum,List<PaySplit> listSplitsCur,Payment payCur
			,List<AccountEntry> listPayFirstAcctEntries,bool isIncomeTxfr=false,bool isPreferCurPat=false,LoadData loadData=null
			,bool doIncludeExplicitCreditsOnly=false,bool isAllocateUnearned=false,DateTime dateAsOf=default,bool doIncludeTreatmentPlanned=false)
		{
			//No remoting role check; no call to db
			ConstructResults retVal=new ConstructResults();
			retVal.Payment=payCur;
			#region Get data
			//Get the lists of items we'll be using to calculate with.
			ConstructChargesData constructChargesData=loadData?.ConstructChargesData
				??GetConstructChargesData(listPatNums,patCurNum,listSplitsCur,retVal.Payment.PayNum,isIncomeTxfr,doIncludeTreatmentPlanned);
			#endregion
			#region Construct List of Charges
			retVal.ListAccountCharges=ConstructListCharges(constructChargesData.ListProcs,
				constructChargesData.ListAdjustments,
				constructChargesData.ListPaySplits,
				constructChargesData.ListInsPayAsTotal,
				constructChargesData.ListPayPlanCharges,
				constructChargesData.ListPayPlanLinks,
				isIncomeTxfr,
				constructChargesData.ListClaimProcsFiltered,
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum > 0));
			retVal.ListAccountCharges.Sort(AccountEntrySort);
			if(dateAsOf.Year > 1880) {
				//Remove all account entries that fall after the 'as of date' passed in.
				retVal.ListAccountCharges.RemoveAll(x => x.Date > dateAsOf);
			}
			#endregion
			#region Explicitly Link Credits
			//When executing an income transfer from within the payment window listSplitsCur can be filled with new splits.
			//These splits need to be present in the list of outstanding charges so that their value can be considered within the pat/prov/clinic bucket.
			//This is because the Outstanding Charges grid is forced to be grouped by pat/prov/clinic when in transfer mode.
			//The AmountEnd column for the grouping could show incorrectly without these Account Entries after a transfer has been made.
			if(isIncomeTxfr) {
				retVal.ListAccountCharges.AddRange(listSplitsCur.Where(x => x.SplitNum==0).Select(x => new AccountEntry(x)));
			}
			//Make deep copies of the current splits that are attached to the payment because the SplitAmt field will be manipulated below.
			List<PaySplit> listSplitsCurrent=listSplitsCur.Where(x => x.SplitNum==0).Select(y => y.Copy()).ToList();
			List<PaySplit> listSplitsHistoric=constructChargesData.ListPaySplits.Select(x => x.Copy()).ToList();
			List<PaySplit> listSplitsCurrentAndHistoric=listSplitsCurrent;
			listSplitsCurrentAndHistoric.AddRange(listSplitsHistoric);
			//This ordering is necessary so parents come before their children when explicitly linking credits.
			listSplitsCurrentAndHistoric=listSplitsCurrentAndHistoric.OrderBy(x => x.SplitNum > 0)
				.ThenBy(x => x.DatePay)
				.ToList();
			retVal.ListAccountCharges=ExplicitlyLinkCredits(retVal.ListAccountCharges,
				listSplitsCurrentAndHistoric);
			#endregion
			#region Implicitly Link Credits
			//If this payment is an income transfer, do NOT use unallocated income to pay off charges.
			//However, allow partial implicit linking when running AllocateUnearned logic.
			if((!isIncomeTxfr && !doIncludeExplicitCreditsOnly)
				|| (isIncomeTxfr && isAllocateUnearned))
			{
				PayResults implicitResult=ImplicitlyLinkCredits(listSplitsHistoric,
					constructChargesData.ListInsPayAsTotal,
					retVal.ListAccountCharges,
					listSplitsCur,
					listPayFirstAcctEntries,
					retVal.Payment,
					patCurNum,
					isPreferCurPat,
					isAllocateUnearned);
				retVal.ListAccountCharges=implicitResult.ListAccountCharges;
				retVal.ListSplitsCur=implicitResult.ListSplitsCur;
			}
			#endregion
			#region Set AmountAvailable
			//Set the AmountAvailable field on each account entry to the sum of all PaySplits that are associated to other payments.
			foreach(AccountEntry accountEntry in retVal.ListAccountCharges.Where(x => x.PayPlanChargeNum==0)) {
				double amtUsed=accountEntry.SplitCollection.Where(x => x.SplitNum > 0 && x.PayNum!=payCur.PayNum && x!=accountEntry.Tag)
					.Sum(x => x.SplitAmt);
				accountEntry.AmountAvailable=(accountEntry.AmountOriginal + accountEntry.AdjustedAmt)-(decimal)amtUsed;
				if(accountEntry.Tag!=null && accountEntry.Tag.GetType()==typeof(Procedure) && ((Procedure)accountEntry.Tag).ProcStatus==ProcStat.TP) {
					//Apply the value set in the Discount field at the procedure level only when the procedure is of status TP.
					accountEntry.AmountAvailable-=(decimal)((Procedure)accountEntry.Tag).Discount;
				}
			}
			//Payment plan account entries are handled differently because they can have multiple account entries that represent one payment plan charge.
			Dictionary<long,List<AccountEntry>> dictPayPlanChargeEntries=retVal.ListAccountCharges.Where(x => x.PayPlanChargeNum > 0)
				.GroupBy(x => x.PayPlanChargeNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(long payPlanChargeNum in dictPayPlanChargeEntries.Keys) {
				List<PaySplit> listPaySplits=new List<PaySplit>();
				//Get all unique PaySplits that are associated to this charge.
				foreach(AccountEntry accountEntry in dictPayPlanChargeEntries[payPlanChargeNum]) {
					foreach(PaySplit paySplit in accountEntry.SplitCollection.Where(x => x.SplitNum > 0 && x.PayNum!=payCur.PayNum && x!=accountEntry.Tag)) {
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
			retVal.ListAccountCharges.ForEach(x => x.IncomeAmt=(decimal)x.AmountPaid);
			#endregion
			#region Combine Unallocated and Unearned
			//The unallocated and unearned buckets are unique in the fact that they do not care about explicit linking in order to be taken from.
			//This means that money can be taken from unallocated or unearned for providerA to pay for an outstanding procedure for providerB.
			//However, there could be a plethora of individual account entries for individual providers which can cause many tiny splits when transferring.
			//All unallocated and unearned buckets should be combined in order to cut back on the number of splits that income transfers suggest.
			if(retVal.ListAccountCharges.Any(x => x.PayPlanNum==0 && (x.IsUnallocated || x.IsUnearned))) {
				//Identify the unallocated and unearned entries that have value to transfer.
				List<AccountEntry> listUnallocatedUnearnedEntries=retVal.ListAccountCharges.FindAll(x => (x.IsUnallocated || x.IsUnearned)
					&& x.PayPlanNum==0
					&& !CompareDecimal.IsZero(x.AmountEnd));
				//Remove these entries from the return value. They will be grouped up and put back in later.
				retVal.ListAccountCharges.RemoveAll(x => (x.IsUnallocated || x.IsUnearned)
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
				listUnallocatedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
					.ToDictionary(x => x.Key,x => x.ToList())
					.ForEach(x => retVal.ListAccountCharges.Add(funcGetCombinedEntry(x.Value)));
				if(PrefC.IsODHQ) {
					//HQ wants to have each unearned type considered separately.
					//This will allow for payment splits to be suggested that will technically transfer from one unearned type to another unearned type.
					listUnearnedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum,x.UnearnedType })
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => retVal.ListAccountCharges.Add(funcGetCombinedEntry(x.Value)));
				}
				else {
					//Treat all unearned types the same, as if there is only one big unearned type.
					listUnearnedEntries.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.ForEach(x => retVal.ListAccountCharges.Add(funcGetCombinedEntry(x.Value)));
				}
			}
			#endregion
			return retVal;
		}

		public static List<AccountEntry> ConstructListCharges(List<Procedure> listProcs,List<Adjustment> listAdjustments,List<PaySplit> listPaySplits,
			List<PayAsTotal> listInsPayAsTotal,List<PayPlanCharge> listPayPlanCharges,List<PayPlanLink> listPayPlanLinks,bool isIncomeTxfr,
			List<ClaimProc> listClaimProcs,List<PayPlan> listInsPayPlans=null)
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
				List<long> listInsPayPlanNums=new List<long>();
				if(!listInsPayPlans.IsNullOrEmpty()) {
					listInsPayPlanNums=listInsPayPlans.Select(x => x.PayPlanNum).ToList();
				}
				listCharges.AddRange(
					GetFauxEntriesForPayPlans(listPayPlanCharges.FindAll(x => !listInsPayPlanNums.Contains(x.PayPlanNum)),listPayPlanLinks,listCharges)
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
					//If we are hiding hidden splits and current split doesn't have hidden type OR we are not hiding hidden splits, add split to listCharges
					if(listHiddenUnearnedTypes.Contains(listPaySplits[i].UnearnedType) && listPaySplits[i].ProcNum > 0) {
						continue;
					}
					//In Income Transfer mode, add all paysplits to the buckets.  
					//The income transfers made previously should balance out any adjustments/inspaytotals that were transferred previously.
					listCharges.Add(new AccountEntry(listPaySplits[i]));
				}
				foreach(PayAsTotal totalPmt in listInsPayAsTotal) {//Ins pay totals need to be added to the sum total for income transfers
					listCharges.Add(new AccountEntry(totalPmt));
				}
			}
			#endregion
			return listCharges;
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

		private static List<FauxAccountEntry> GetFauxEntriesForPayPlans(List<PayPlanCharge> listPayPlanCharges,List<PayPlanLink> listPayPlanLinks,
			List<AccountEntry> listCharges)
		{
			List<FauxAccountEntry> listPayPlanAccountEntries=new List<FauxAccountEntry>();
			List<PayPlanCharge> listPayPlanChargeCredits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Credit);
			List<PayPlanCharge> listPayPlanChargeDebits=listPayPlanCharges.FindAll(x => x.ChargeType==PayPlanChargeType.Debit);
			List<PayPlanProductionEntry> listPayPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			#region Patient Payment Plan Credits
			//Create faux account entries for all credits associated to a payment plan.
			foreach(PayPlanCharge payPlanChargeCredit in listPayPlanChargeCredits) {
				FauxAccountEntry fauxAccountEntry=new FauxAccountEntry(payPlanChargeCredit,true);
				if(!fauxAccountEntry.IsAdjustment) {
					//We don't technically know how much this faux procedure account entry is worth until we consider the debits that are due.  That is later.
					fauxAccountEntry.AmountEnd=0;
					//Prefer the patient, provider, and clinic combo from the procedure if present.
					if(payPlanChargeCredit.ProcNum > 0) {
						AccountEntry accountEntryProc=listCharges.FirstOrDefault(x => x.ProcNum > 0 && x.ProcNum==payPlanChargeCredit.ProcNum);
						if(accountEntryProc==null) {
							continue;//Do NOT add this FauxAccountEntry to the list of payment plan account entries because the associated proc was not found.
						}
						ExplicitlyLinkPositiveAdjustmentsToProcedure(accountEntryProc,listCharges);
						fauxAccountEntry.AccountEntryProc=accountEntryProc;
						fauxAccountEntry.PatNum=accountEntryProc.PatNum;
						fauxAccountEntry.ProvNum=accountEntryProc.ProvNum;
						fauxAccountEntry.ClinicNum=accountEntryProc.ClinicNum;
						//Take as much value away from the procedure as possible if principal is positive because a payplan might only cover part of a procedure.
						//Only do this for positive credits because negative procedure credits should not give value back to the procedure.
						if(CompareDecimal.IsGreaterThanZero(fauxAccountEntry.Principal)) {
							accountEntryProc.AmountEnd-=Math.Min(accountEntryProc.AmountEnd,fauxAccountEntry.Principal);
						}
					}
				}
				listPayPlanAccountEntries.Add(fauxAccountEntry);
			}
			#endregion
			#region Dynamic Payment Plan Credits
			//Create faux account entries for PayPlanProductionEntry procedures and adjustments (dynamic payment plan credits).
			foreach(PayPlanProductionEntry payPlanProdEntry in listPayPlanProductionEntries
				.FindAll(x => ListTools.In(x.LinkType,PayPlanLinkType.Procedure,PayPlanLinkType.Adjustment)))
			{
				FauxAccountEntry fauxCreditEntry=new FauxAccountEntry(payPlanProdEntry);
				if(fauxCreditEntry.IsAdjustment) {
					AccountEntry accountEntryAdj=listCharges.FirstOrDefault(x => x.GetType()==typeof(Adjustment) && x.AdjNum > 0 && x.AdjNum==fauxCreditEntry.AdjNum);
					if(accountEntryAdj==null) {
						continue;
					}
					//Dynamic payment plans don't create multiple PayPlanCharge entries for adjustments (like the old payment plan system does).
					//There will be a single PayPlanLink entry that is attached to a specific adjustment.
					//The strange part is that the PayPlanCharge entries that are created for this dynamic payment plan have already factored in adjustments.
					//So take value away from the adjustment if AmountEnd is positive because a payplan might only cover part of an adjustment.
					if(CompareDecimal.IsGreaterThanZero(fauxCreditEntry.AmountEnd)) {
						accountEntryAdj.AmountEnd-=Math.Min(accountEntryAdj.AmountEnd,fauxCreditEntry.AmountEnd);
					}
					else if(CompareDecimal.IsLessThanZero(fauxCreditEntry.AmountEnd)) {
						accountEntryAdj.AmountEnd-=Math.Max(accountEntryAdj.AmountEnd,fauxCreditEntry.AmountEnd);
					}
				}
				else {//Procedure
					AccountEntry accountEntryProc=listCharges.FirstOrDefault(x => x.ProcNum > 0 && x.ProcNum==fauxCreditEntry.ProcNum);
					if(accountEntryProc==null) {
						continue;//Do NOT add this FauxAccountEntry to the list of payment plan account entries because the associated proc was not found.
					}
					ExplicitlyLinkPositiveAdjustmentsToProcedure(accountEntryProc,listCharges);
					fauxCreditEntry.AccountEntryProc=accountEntryProc;
					//Take as much value away from the procedure as possible if AmountEnd is positive because a payplan might only cover part of a procedure.
					//Only do this for positive credits because negative procedure credits should not give value back to the procedure.
					if(CompareDecimal.IsGreaterThanZero(fauxCreditEntry.AmountEnd)) {
						accountEntryProc.AmountEnd-=Math.Min(accountEntryProc.AmountEnd,fauxCreditEntry.AmountEnd);
					}
				}
				listPayPlanAccountEntries.Add(fauxCreditEntry);
			}
			#endregion
			#region All Payment Plan Debits
			foreach(PayPlanCharge payPlanDebit in listPayPlanChargeDebits) {
				if(!CompareDouble.IsZero(payPlanDebit.Principal)) {
					listPayPlanAccountEntries.Add(new FauxAccountEntry(payPlanDebit,true));
				}
				if(!CompareDouble.IsZero(payPlanDebit.Interest)) {
					listPayPlanAccountEntries.Add(new FauxAccountEntry(payPlanDebit,false));
				}
			}
			#endregion
			#region Manipulate Procedure AmountEnd
			//Now that all of the account entries have been created, manipulate the AmountEnd in preparation for explicit and implicit linking.
			//We need to figure out how much of the payment plan is actually due right now (debits due) and give that value to the faux entries.
			Dictionary<long,List<FauxAccountEntry>> dictPayPlanEntries=listPayPlanAccountEntries
				.GroupBy(x => x.PayPlanNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			List<FauxAccountEntry> listAllocatedDebits=new List<FauxAccountEntry>();
			foreach(long payPlanNum in dictPayPlanEntries.Keys) {
				List<FauxAccountEntry> listDebits=dictPayPlanEntries[payPlanNum].FindAll(x => x.ChargeType==PayPlanChargeType.Debit);
				//Dynamic payment plans are strange in the sense that they utilize the PayPlanCharge FKey column to directly link DEBITS to procedures.
				//There is a report that breaks down 'overpaid payment plans' to a procedure level to help the user know exactly which procedures are wrong.
				//Therefore, loop through each faux credit entry one at a time and apply any matching due debits (via FKey) first. Starting with adjustments.
				foreach(FauxAccountEntry creditEntry in dictPayPlanEntries[payPlanNum].Where(x => x.AdjNum > 0)) {
					List<FauxAccountEntry> listAdjDebits=listDebits.FindAll(x => x.IsAdjustment && x.AdjNum==creditEntry.AdjNum);
					listAllocatedDebits.AddRange(AllocatePayPlanDebitsToCredit(creditEntry,listAdjDebits));
				}
				foreach(FauxAccountEntry fauxEntry in dictPayPlanEntries[payPlanNum].Where(x => x.ProcNum > 0)) {
					List<FauxAccountEntry> listProcDebits=listDebits.FindAll(x => !x.IsAdjustment && x.ProcNum==fauxEntry.ProcNum);
					listAllocatedDebits.AddRange(AllocatePayPlanDebitsToCredit(fauxEntry,listProcDebits));
				}
				//Same goes for adjustments.
				//Now that the dynamic payment plan credits have been handled to the best of our ability, blindly apply any leftover debits to credits.
				listAllocatedDebits.AddRange(AllocatePayPlanDebitsToCredits(dictPayPlanEntries[payPlanNum],listDebits));
			}
			#endregion
			//Only return debit faux account entries because the credits were only used to figure out where value was distributed.
			//Patients should never pay on credits and should only pay on debits (negative credits aren't supported ATM).
			listPayPlanAccountEntries.RemoveAll(x => x.ChargeType==PayPlanChargeType.Credit);
			//Remove any debits that have no value because they have been allocated to credits.
			listPayPlanAccountEntries.RemoveAll(x => x.ChargeType==PayPlanChargeType.Debit
				&& CompareDecimal.IsZero(x.AmountEnd)
				&& ListTools.In(x.PayPlanChargeNum,listAllocatedDebits.Select(y => y.PayPlanChargeNum)));
			//Add every distributed debit to the return value.
			listPayPlanAccountEntries.AddRange(listAllocatedDebits);
			//Remove all value from debits due in the future so that calling entities don't think these charges are due right now.
			listPayPlanAccountEntries.FindAll(x => x.Date > DateTime.Today).ForEach(x => x.AmountEnd=0);
			return listPayPlanAccountEntries;
		}

		private static void ExplicitlyLinkPositiveAdjustmentsToProcedure(AccountEntry procEntry,List<AccountEntry> listAdjEntries) {
			if(procEntry==null || procEntry.GetType()!=typeof(Procedure) || listAdjEntries==null || listAdjEntries.Count==0) {
				return;
			}
			List<AccountEntry> listExplicitAdjEntries=listAdjEntries.FindAll(x => x.GetType()==typeof(Adjustment)
				&& x.AmountEnd > 0
				&& x.ProcNum==procEntry.ProcNum
				&& x.PatNum==procEntry.PatNum
				&& x.ProvNum==procEntry.ProvNum
				&& x.ClinicNum==procEntry.ClinicNum);
			foreach(AccountEntry adjEntry in listExplicitAdjEntries) {
				procEntry.AmountEnd+=adjEntry.AmountEnd;
				adjEntry.AmountEnd=0;
			}
		}

		private static List<FauxAccountEntry> AllocatePayPlanDebitsToCredit(FauxAccountEntry fauxCredit,List<FauxAccountEntry> listDebits) {
			return AllocatePayPlanDebitsToCredits(new List<FauxAccountEntry>() { fauxCredit },listDebits);
		}

		private static List<FauxAccountEntry> AllocatePayPlanDebitsToCredits(List<FauxAccountEntry> listFauxEntries,List<FauxAccountEntry> listDebits) {
			List<FauxAccountEntry> listAllocatedDebits=new List<FauxAccountEntry>();
			#region Adjustments (for non-dynamic payment plans)
			//Adjustments for patient payment plans will have a negative debit AND a negative credit.
			//These adjustments need to offset each other and push value back to the credits that are attached to procedures (if any).
			foreach(FauxAccountEntry fauxAdjCredit in listFauxEntries.FindAll(x => x.IsAdjustment && x.ChargeType==PayPlanChargeType.Credit && !x.IsDynamic)) {
				decimal amtToOffset=fauxAdjCredit.AmountEnd;//Used when offsetting adjustments.
				decimal amtToRemove=fauxAdjCredit.AmountEnd;//Used when removing value from due debits.
				decimal amtToAllocate=fauxAdjCredit.AmountEnd;//Used when allocating value back to procedure credits.
				#region Offset Adjustments
				//There should always be a credit adjustment entry with a corresponding debit adjustment for patient payment plans.
				//Since we are about to 'adjust' the entire value of the payment plan (remove value from due debits) we can offset the adjustment entries.
				foreach(FauxAccountEntry fauxAdjDebit in listFauxEntries.FindAll(x => x.IsAdjustment && x.ChargeType==PayPlanChargeType.Debit)) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(amtToOffset)) {
						break;
					}
					if(CompareDecimal.IsGreaterThanOrEqualToZero(fauxAdjDebit.AmountEnd)) {
						continue;
					}
					decimal amt=Math.Min(Math.Abs(amtToOffset),Math.Abs(fauxAdjDebit.AmountEnd));
					fauxAdjCredit.AmountEnd+=amt;
					fauxAdjDebit.AmountEnd+=amt;
					amtToOffset+=amt;
				}
				#endregion
				#region Remove Due Debits Value
				//This is where the real magic of payment plan adjustments takes place.
				//Removing value from debits that are due is how the payment plan is worth less money overall.
				//Patient payment plan debits are never explicitly linked to anything so it doesn't matter which debits we take from.
				List<FauxAccountEntry> listPosDebits=listDebits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd)
					&& x.ChargeType==PayPlanChargeType.Debit
					&& !CompareDecimal.IsZero(x.Principal));
				foreach(FauxAccountEntry positiveDebit in listPosDebits) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(amtToRemove)) {
						break;
					}
					decimal amtDebitRemaining=positiveDebit.AmountEnd;
					if(CompareDecimal.IsLessThanOrEqualToZero(amtDebitRemaining)) {
						continue;
					}
					decimal amt=Math.Min(Math.Abs(amtToRemove),amtDebitRemaining);
					positiveDebit.AmountEnd-=amt;
					amtToRemove+=amt;
				}
				#endregion
				#region Allocate to Procedures
				//This is where the ludicrous magic of payment plan adjustments takes place.
				//Feed the adjustment value back into entities that have FauxAccountEntry procedure credits attached.
				//If there are no faux credits attached to procedures then this value won't go anywhere and is just 'written off'.
				//We need to give value BACK to the procedure credits just in case there was an invalid pat/prov/clinic attached (for income transfers).
				foreach(FauxAccountEntry fauxProcEntry in listFauxEntries.FindAll(x => !x.IsAdjustment && x.AccountEntryProc!=null)) {
					if(CompareDecimal.IsGreaterThanOrEqualToZero(amtToAllocate)) {
						break;
					}
					decimal amtProcCanTake=fauxProcEntry.AccountEntryProc.AmountOriginal-fauxProcEntry.AccountEntryProc.AmountEnd;
					if(CompareDecimal.IsLessThanOrEqualToZero(amtProcCanTake)) {
						continue;
					}
					decimal amtToGiveBack=Math.Min(Math.Abs(amtToAllocate),amtProcCanTake);
					fauxProcEntry.AccountEntryProc.AmountEnd+=amtToGiveBack;
					//The overall value of this faux entry needs to be adjusted just in case there are other procedures / credits on the payment plan.
					//E.g. if this procedure was overcharged to begin with, it shouldn't give value back to the procedure AND continue to be worth full value.
					fauxProcEntry.PrincipalAdjusted-=amtToGiveBack;
					amtToAllocate+=amtToGiveBack;
				}
				#endregion
			}
			#endregion
			#region Adjustments (dynamic payment plans)
			//Loop through each faux credit adjustment and apply as many adjustment debits as possible.
			//That's right, dynamic payment plans explicitly dictate if a debit is designed for a procedure or an adjustment.
			foreach(FauxAccountEntry fauxAdjCredit in listFauxEntries.FindAll(x => x.IsAdjustment && x.IsDynamic)) {
				//It is safe to use AmountEnd because Principal was the only thing used to populate it when this faux entry was created (no interest).
				List<FauxAccountEntry> listPosDebits=listDebits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd)
					&& x.ChargeType==PayPlanChargeType.Debit
					&& x.IsAdjustment
					&& !CompareDecimal.IsZero(x.Principal));
				foreach(FauxAccountEntry positiveDebit in listPosDebits) {
					if(CompareDecimal.IsLessThanOrEqualToZero(fauxAdjCredit.AmountEnd)) {
						break;
					}
					decimal amtDebitRemaining=positiveDebit.AmountEnd;
					if(CompareDecimal.IsLessThanOrEqualToZero(amtDebitRemaining)) {
						continue;
					}
					decimal amtToAllocate=Math.Min(fauxAdjCredit.AmountEnd,amtDebitRemaining);
					positiveDebit.AmountEnd-=amtToAllocate;
					fauxAdjCredit.AmountEnd-=amtToAllocate;
					listAllocatedDebits.Add(GetAllocatedDebit(amtToAllocate,fauxAdjCredit,positiveDebit));
				}
			}
			#endregion
			#region Credits (non-adjustments)
			//Loop through each faux credit and apply as many debits as possible.
			List<FauxAccountEntry> listFauxNonAdjCredits=listFauxEntries.FindAll(x => !x.IsAdjustment);
			//Never allow FauxAccountEntries associated to TP procedures to get value.
			listFauxNonAdjCredits.RemoveAll(x => x.AccountEntryProc!=null && ((Procedure)x.AccountEntryProc.Tag).ProcStatus==ProcStat.TP);
			foreach(FauxAccountEntry fauxNonAdjCredit in listFauxNonAdjCredits) {
				//Use PrincipalAdjusted instead of AmountEnd (which could include interest) or Principal (has not been adjusted).
				decimal amtCreditRemaining=fauxNonAdjCredit.PrincipalAdjusted;
				List<FauxAccountEntry> listPosDebits=listDebits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd)
					&& x.ChargeType==PayPlanChargeType.Debit
					&& !CompareDecimal.IsZero(x.Principal));
				foreach(FauxAccountEntry positiveDebit in listPosDebits) {
					if(CompareDecimal.IsLessThanOrEqualToZero(amtCreditRemaining)) {
						break;
					}
					decimal amtDebitRemaining=positiveDebit.AmountEnd;
					if(CompareDecimal.IsLessThanOrEqualToZero(amtDebitRemaining)) {
						continue;
					}
					decimal amtToAllocate=Math.Min(amtCreditRemaining,amtDebitRemaining);
					positiveDebit.AmountEnd-=amtToAllocate;
					amtCreditRemaining-=amtToAllocate;
					listAllocatedDebits.Add(GetAllocatedDebit(amtToAllocate,fauxNonAdjCredit,positiveDebit));
				}
			}
			#endregion
			return listAllocatedDebits;
		}

		private static FauxAccountEntry GetAllocatedDebit(decimal amtToAllocate,FauxAccountEntry fauxCredit,FauxAccountEntry fauxDebit) {
			//Create a new faux account entry from the debit but only for the amount that to allocate.
			FauxAccountEntry allocatedDebit=fauxDebit.Copy();
			allocatedDebit.AccountEntryProc=fauxCredit.AccountEntryProc;
			allocatedDebit.AmountEnd=amtToAllocate;
			allocatedDebit.Principal=amtToAllocate;
			allocatedDebit.PrincipalAdjusted=amtToAllocate;
			if(PrefC.GetEnum<PayPlanVersions>(PrefName.PayPlansVersion)==PayPlanVersions.NoCharges) {
				//There are no charges shown in patient Account modules in NoCharges mode so do not require the guarantor to pay for the charges.
				//PatNum needs to ignore the Guarantor value that is on the debit which is technically the person responsible for payment.
				allocatedDebit.PatNum=fauxDebit.PatNum;
			}
			else {
				//PatNum need to be set to the Guarantor value that is on the debit.
				//This is the patient (could be in any family) that is responsible for paying off this debit.
				allocatedDebit.PatNum=fauxDebit.Guarantor;
			}
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
		private static List<AccountEntry> ExplicitlyLinkCredits(List<AccountEntry> listAccountEntries,List<PaySplit> listSplitsCurrentAndHistoric) {
			//No remoting role check; no call to db and private method
			List<AccountEntry> listExplicitAccountCharges=listAccountEntries
				.FindAll(x => ListTools.In(x.GetType(),typeof(Procedure),typeof(FauxAccountEntry),typeof(Adjustment)));
			//Create a dictionary that can easily find a corresponding AccountEntry for a specific PaySplit.
			//Old logic did not consider the fact that the same PaySplit could be in multiple AccountEntries so maybe that scenario isn't possible.
			Dictionary<string,AccountEntry> dictPaySplitAccountEntries=new Dictionary<string,AccountEntry>();
			foreach(AccountEntry splitEntry in listAccountEntries.FindAll(x => x.GetType()==typeof(PaySplit))) {
				foreach(PaySplit paySplit in splitEntry.SplitCollection) {
					dictPaySplitAccountEntries[(string)paySplit.TagOD]=splitEntry;
				}
			}
			#region Adjustments (payment plans / procedures)
			Dictionary<long,List<AccountEntry>> dictProcNumEntries=listExplicitAccountCharges.GroupBy(x => x.ProcNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			#region Payment Plans
			foreach(long procNum in dictProcNumEntries.Keys) {
				//Adjustments can be associated to procedures that are associated to payment plans.
				//Value should be removed from positive adjustments that have already been considered within the principal of payment plan credits.
				if(dictProcNumEntries[procNum].Count(x => x.GetType()==typeof(FauxAccountEntry))==0
					|| dictProcNumEntries[procNum].Count(x => x.GetType()==typeof(Adjustment))==0
					|| dictProcNumEntries[procNum].Count(x => x.GetType()==typeof(Procedure))==0)
				{
					continue;//No payment plan entries, adjustments, or procedures.  Nothing to do in this loop which requires all three be present.
				}
				//Find the procedure that all of these entries are associated to in order to get access to the ProcFee.
				AccountEntry procAccountEntry=dictProcNumEntries[procNum].First(x => x.GetType()==typeof(Procedure));
				//Sum the principal for the faux entries and subtract the ProcFee from that value.
				//Any amount remaining can be directly removed from the adjustment so it's not double counted.
				decimal principalTotal=dictProcNumEntries[procNum].Where(x => x.GetType()==typeof(FauxAccountEntry))
					.Cast<FauxAccountEntry>()
					.Sum(x => x.Principal);
				decimal amountRemaining=(principalTotal-procAccountEntry.AmountOriginal);
				//Find all of the adjustments that are directly associated to this pat/prov/clinic combo.
				List<AccountEntry> listAdjAccountEntries=dictProcNumEntries[procNum].FindAll(x => x.GetType()==typeof(Adjustment)
					&& x.PatNum==procAccountEntry.PatNum
					&& x.ProvNum==procAccountEntry.ProvNum
					&& x.ClinicNum==procAccountEntry.ClinicNum);
				foreach(AccountEntry adjAccountEntry in listAdjAccountEntries) {
					if(CompareDecimal.IsLessThanOrEqualToZero(amountRemaining)) {
						break;
					}
					decimal amountToRemove=Math.Min(adjAccountEntry.AmountEnd,amountRemaining);
					adjAccountEntry.AmountEnd-=amountToRemove;
					amountRemaining-=amountToRemove;
				}
			}
			#endregion
			#region Procedures
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
			#region Insurance Overpayments
			//Insurance overpayments should not be transferred around. The user needs to be warned to manually handle this scenario themselves.
			List<AccountEntry> listInsProcEntries=listExplicitAccountCharges.FindAll(x => x.GetType()==typeof(Procedure) && x.InsPayAmt > 0);
			//However, allow ZZZFIX procedures to have insurance payments transferred around since they are conversion related.
			ProcedureCode codeZZZFIX=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode=="ZZZFIX");
			if(codeZZZFIX!=null) {
				listInsProcEntries.RemoveAll(x => ((Procedure)x.Tag).CodeNum==codeZZZFIX.CodeNum);
			}
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
				//Overpayment. The procedure should be completely ignored when there is any insurance overpayment whatsoever.
				accountEntryInsProc.WarningMsg.AppendLine($"Procedure #{accountEntryInsProc.ProcNum} "
					+$"for PatNum #{accountEntryInsProc.PatNum} was ignored due to insurance overpayment:");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^{accountEntryInsProc.DescriptionForGrid} on {accountEntryInsProc.Date.ToShortDateString()}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^ProcFeeTotal: {((Procedure)accountEntryInsProc.Tag).ProcFeeTotal:C}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^Proc AmountEnd: {accountEntryInsProc.AmountEnd:C}");
				accountEntryInsProc.WarningMsg.AppendLine($"  ^InsPayAmt: {accountEntryInsProc.InsPayAmt:C}");
				accountEntryInsProc.AmountEnd=0;//set to 0 so the negative doesn't get transferred to anything else. 
			}
			#endregion
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
				List<AccountEntry> listPayPlanEntries=listPatProvClinicAccountCharges.FindAll(x => x.GetType()==typeof(FauxAccountEntry));
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
				//Explicitly attach all PayPlanCharge splits first. Then the payment plan splits will be attached to anything that is left over.
				foreach(AccountEntry payPlanChargeEntry in listPayPlanEntries) {
					List<PaySplit> listPayPlanChargeSplits=listPatProvClinicSplits.FindAll(x => CompareDecimal.IsGreaterThanZero(x.SplitAmt)
						&& x.UnearnedType==0
						&& x.PayPlanNum==payPlanChargeEntry.PayPlanNum
						&& x.PayPlanChargeNum==payPlanChargeEntry.PayPlanChargeNum);
					foreach(PaySplit payPlanChargeSplit in listPayPlanChargeSplits) {
						//Production that is associated to the split must match the production on the charge to be considered explicitly linked.
						if((payPlanChargeSplit.ProcNum > 0 || payPlanChargeSplit.AdjNum > 0)
							&& (payPlanChargeSplit.ProcNum!=payPlanChargeEntry.ProcNum || payPlanChargeSplit.AdjNum!=payPlanChargeEntry.AdjNum))
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
				foreach(AccountEntry adjEntry in listAdjEntries) {
					List<PaySplit> listAdjSplits=listPatProvClinicSplits.FindAll(x => !CompareDouble.IsZero(x.SplitAmt)
						&& (x.AdjNum==adjEntry.AdjNum || (x.ProcNum!=0 && x.ProcNum==((Adjustment)adjEntry.Tag).ProcNum))
						&& x.PayPlanNum==0);
					foreach(PaySplit adjSplit in listAdjSplits) {
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
		///<param name="payCur">The payment that is wanting to be linked to other account entries.  Could have entites linked already.</param>
		///<param name="patNum">The PatNum of the currently selected patient.</param>
		///<param name="isPatPrefer">Set to true if account entries for patNum should be prioritized before other entries.</param>
		///<returns>A helper class that represents the implicit credits that this method made.</returns>
		private static PayResults ImplicitlyLinkCredits(List<PaySplit> listPaySplits,List<PayAsTotal> listInsPayAsTotal,
			List<AccountEntry> listAccountCharges,List<PaySplit> listSplitsCur,List<AccountEntry> listPayFirstAcctEntries,Payment payCur,long patNum,
			bool isPatPrefer,bool isAllocateUnearned=false)
		{
			//No remoting role check; no call to db and private method
			if(isPatPrefer) {
				//Shove all account entries associated to patNum passed in to the bottom of the list so that they are implicitly linked to last.
				listAccountCharges=listAccountCharges.OrderByDescending(x => x.PatNum!=patNum).ThenBy(x => x.Date).ToList();
			}
			if(!listPayFirstAcctEntries.IsNullOrEmpty()) {//User has specific procs/payplancharges/adjustments selected prior to entering the Payment window.  
				//They wish these be paid by this payment specifically.
				//To accomplish this, we need to auto-split to the selected procedures prior to implicit linking.
				foreach(AccountEntry entry in listPayFirstAcctEntries) {
					if(payCur.PayAmt<=0) {
						break;//Will be empty
					}
					if(entry.GetType()==typeof(PayPlanCharge)) {
						//Handle payment plan splits in a special way. Continue to the next entry after we add to the list. 
						listSplitsCur.AddRange(CreatePaySplitsForPayPlanCharge(payCur,(PayPlanCharge)entry.Tag,listAccountCharges));
						continue;
					}
					AccountEntry charge=listAccountCharges.Find(x => x.PriKey==entry.PriKey);
					if(charge==null) {
						continue;//likely only for the event of selecting payplan line items when the plan is closed and on version 2. 
					}
					PaySplit split=new PaySplit();
					if(charge.GetType()==typeof(Procedure)) {
						split.ProcNum=charge.PriKey;
					}
					else if(charge.GetType()==typeof(Adjustment) && ((Adjustment)charge.Tag).ProcNum==0) {
						//should already be verified to have no procedure and positive amount
						split.AdjNum=charge.PriKey;
					}
					if(PrefC.HasClinicsEnabled) {//Clinics
						split.ClinicNum=charge.ClinicNum;
					}
					double amt=Math.Min((double)charge.AmountEnd,payCur.PayAmt);
					payCur.PayAmt=Math.Round(payCur.PayAmt-amt,3);
					split.SplitAmt=amt;
					charge.AmountEnd-=(decimal)amt;
					split.DatePay=DateTime.Today;
					split.PatNum=charge.PatNum;
					split.ProvNum=charge.ProvNum;
					split.PayNum=payCur.PayNum;
					split.PayPlanNum=charge.PayPlanNum;
					split.PayPlanChargeNum=charge.PayPlanChargeNum;
					charge.SplitCollection.Add(split);
					listSplitsCur.Add(split);
				}
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
				listImplicitCharges=listImplicitCharges.OrderBy(x => listPayFirstAcctEntries.Any(y => y.PriKey==x.PriKey && y.GetType()==x.GetType())).ToList();
			}
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
			implicitCredits.Payment=payCur;
			return implicitCredits;
		}

		private static List<PaySplit> CreatePaySplitsForPayPlanCharge(Payment payCur,PayPlanCharge payPlanCharge,List<AccountEntry> listAccountEntries) {
			List<PaySplit> listSplitsToAdd=new List<PaySplit>();
			//Find all of the account entries that are associated to the PayPlanCharge.
			//There can be multiple; one for principal and one for interest.
			List<FauxAccountEntry> listFauxAccountEntries=listAccountEntries.Where(x => x.GetType()==typeof(FauxAccountEntry)
					&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
					&& x.PayPlanChargeNum==payPlanCharge.PayPlanChargeNum)
				.Cast<FauxAccountEntry>()
				.OrderBy(x => CompareDecimal.IsGreaterThanZero(x.Interest))//Pay interest first.
				.ToList();
			foreach(FauxAccountEntry fauxAccountEntry in listFauxAccountEntries) {
				if(CompareDouble.IsLessThanOrEqualToZero(payCur.PayAmt)) {
					break;
				}
				double amountToSplit=Math.Min((double)fauxAccountEntry.AmountEnd,payCur.PayAmt);
				PaySplit paySplit=new PaySplit();
				paySplit.AdjNum=fauxAccountEntry.AdjNum;
				paySplit.DatePay=DateTime.Today;
				//the split should always go to the payplancharge's guarantor.
				paySplit.PatNum=payPlanCharge.Guarantor;
				paySplit.ProcNum=fauxAccountEntry.ProcNum;
				paySplit.ProvNum=fauxAccountEntry.ProvNum;
				if(PrefC.HasClinicsEnabled) {//Clinics
					paySplit.ClinicNum=fauxAccountEntry.ClinicNum;
				}
				paySplit.PayPlanNum=fauxAccountEntry.PayPlanNum;
				paySplit.PayPlanChargeNum=fauxAccountEntry.PayPlanChargeNum;
				paySplit.PayNum=payCur.PayNum;
				paySplit.SplitAmt=amountToSplit;
				fauxAccountEntry.AmountEnd-=(decimal)amountToSplit;
				fauxAccountEntry.SplitCollection.Add(paySplit);
				listSplitsToAdd.Add(paySplit);
				payCur.PayAmt-=amountToSplit;
			}
			return listSplitsToAdd;
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
				if(payAsTotal.SummedInsPayAmt==0 && payAsTotal.SummedWriteOff==0) {
					continue;
				}
				foreach(AccountEntry accountEntry in bucket.ListAccountEntries) {
					if(payAsTotal.SummedInsPayAmt==0 && payAsTotal.SummedWriteOff==0) {
						break;
					}
					if(accountEntry.AmountEnd==0) {
						continue;
					}
					if(ListTools.In(accountEntry.GetType(),typeof(PayPlanCharge),typeof(FauxAccountEntry))) {
						continue;
					}
					double amt=Math.Min((double)accountEntry.AmountEnd,payAsTotal.SummedInsPayAmt);
					accountEntry.AmountEnd-=(decimal)amt;
					payAsTotal.SummedInsPayAmt-=amt;
					double amtWriteOff=Math.Min((double)accountEntry.AmountEnd,payAsTotal.SummedWriteOff);
					accountEntry.AmountEnd-=(decimal)amtWriteOff;
					payAsTotal.SummedWriteOff-=amtWriteOff;
				}
			}
			#endregion
			#region PaySplits
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => !string.IsNullOrEmpty(x.ItemValue))//If ItemValue is not blank, it means "do not show on account"
				.Select(x => x.DefNum).ToList();
			List<PaySplit> listLinkableSplits=bucket.ListPaySplits.FindAll(x => !ListTools.In(x.UnearnedType,listHiddenUnearnedDefNums));
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
					PaySplit splitCopy=split.Copy();
					splitCopy.SplitAmt=amt;
					accountEntry.SplitCollection.Add(splitCopy);
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
			foreach(PaySplit split in listLinkablePosSplits.FindAll(x => x.PayPlanNum==0)) {
				if(CompareDouble.IsLessThanOrEqualToZero(split.SplitAmt)) {
					continue;
				}
				foreach(AccountEntry accountEntry in bucket.ListAccountEntries.FindAll(x => x.PayPlanNum==0)) {
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
					PaySplit splitCopy=split.Copy();
					splitCopy.SplitAmt=amt;
					accountEntry.SplitCollection.Add(splitCopy);
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
					PaySplit splitNegCopy=splitNeg.Copy();
					splitNegCopy.SplitAmt=amt;
					accountEntryNeg.SplitCollection.Add(splitNegCopy);
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
			List<AccountEntry> listNegAdjFauxEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry)
				&& ((FauxAccountEntry)x.Tag).IsAdjustment
				&& CompareDecimal.IsLessThanZero(x.AmountEnd)
				&& !x.IsUnearned);
			List<AccountEntry> listPosFauxEntries=bucket.ListAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry)
				&& CompareDecimal.IsGreaterThanZero(x.AmountEnd)
				&& !x.IsUnearned);
			List<AccountEntry> listNegAdjPosFauxEntries=new List<AccountEntry>();
			listNegAdjPosFauxEntries.AddRange(listNegAdjFauxEntries);
			listNegAdjPosFauxEntries.AddRange(listPosFauxEntries);
			BalanceAccountEntries(ref listNegAdjPosFauxEntries);
			#endregion
			#endregion
		}
		#endregion

		#region Misc
		///<summary>Distributes available unearned money to the account entries up to the amount passed in.</summary>
		public static List<PaySplit> AllocateUnearned(long payNum,double amountUnearned,List<AccountEntry> listAccountEntries,Family fam=null, bool excludeHiddenUnearned=false) {
			//No need to check RemotingRole; no call to db.
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
					listAllocateEntries.AddRange(constructResults.ListAccountCharges.FindAll(x => x.ProcNum==listAccountEntries[i].ProcNum));
				}
				if(listAccountEntries[i].AdjNum > 0) {
					listAllocateEntries.AddRange(constructResults.ListAccountCharges.FindAll(x => x.AdjNum==listAccountEntries[i].AdjNum));
				}
				if(listAccountEntries[i].PayPlanChargeNum > 0) {
					listAllocateEntries.AddRange(constructResults.ListAccountCharges.FindAll(x => x.PayPlanChargeNum==listAccountEntries[i].PayPlanChargeNum));
				}
			}
			//Suggest splits that explicitly take from the unearned bucket first. Any value left over will get transferred from the 0 provider later.
			List<AccountEntry> listUnearnedEntries;
			if(excludeHiddenUnearned) {
				List<long> listDefNumsHiddenUnearnedTypes = new List<long>();
				//get list of unearned types marked as "Do Not Show On Account" so that we can avoid grabbing them for allocation
				Defs.GetCatList((int)DefCat.PaySplitUnearnedType).Where(x => x.ItemValue=="X").ForEach(x => listDefNumsHiddenUnearnedTypes.Add(x.DefNum));
				listUnearnedEntries=constructResults.ListAccountCharges.FindAll(x => x.IsUnearned && CompareDecimal.IsLessThanOrEqualToZero(x.AmountEnd) && !listDefNumsHiddenUnearnedTypes.Contains(x.UnearnedType));
			}
			else {
				listUnearnedEntries=constructResults.ListAccountCharges.FindAll(x => x.IsUnearned && CompareDecimal.IsLessThanOrEqualToZero(x.AmountEnd));
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
					listPaySplits.Add(new PaySplit() {
						AdjNum=unearnedEntry.AdjNum,
						ClinicNum=unearnedEntry.ClinicNum,
						DatePay=DateTime.Today,
						PatNum=unearnedEntry.PatNum,
						PayPlanNum=unearnedEntry.PayPlanNum,
						PayNum=payNum,
						ProcNum=unearnedEntry.ProcNum,
						ProvNum=unearnedEntry.ProvNum,
						SplitAmt=0-amountToAllocate,
						UnearnedType=unearnedEntry.UnearnedType,
					});
					//Blindly apply the amount that was taken from unearned to one of the account entries that the user selected.
					listPaySplits.Add(new PaySplit() {
						AdjNum=accountEntry.AdjNum,
						ClinicNum=accountEntry.ClinicNum,
						DatePay=DateTime.Today,
						PatNum=accountEntry.PatNum,
						PayPlanNum=accountEntry.PayPlanNum,
						PayNum=payNum,
						ProcNum=accountEntry.ProcNum,
						ProvNum=accountEntry.ProvNum,
						SplitAmt=amountToAllocate,
						UnearnedType=0,
					});
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
				listPaySplits.Add(new PaySplit() {
					AdjNum=accountEntry.AdjNum,
					ClinicNum=accountEntry.ClinicNum,
					DatePay=DateTime.Today,
					PatNum=accountEntry.PatNum,
					PayPlanNum=accountEntry.PayPlanNum,
					PayNum=payNum,
					ProcNum=accountEntry.ProcNum,
					ProvNum=accountEntry.ProvNum,
					SplitAmt=amountToAllocate,
					UnearnedType=0,
				});
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
					payCur=splitData.Payment;
				}
			}
			if(splitData==null) {
				splitData=new PayResults { ListAccountCharges=listAllCharges,ListSplitsCur=listPaySplits,Payment=payCur };
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
			PaySplit split=new PaySplit();
			split.DatePay=DateTime.Today;
			split.PayNum=payCur.PayNum;
			split.PatNum=charge.PatNum;
			split.ProvNum=charge.ProvNum;
			split.PayPlanChargeNum=charge.PayPlanChargeNum;
			split.PayPlanNum=charge.PayPlanNum;
			split.ClinicNum=charge.ClinicNum;
			split.ProcNum=charge.ProcNum;
			split.AdjNum=charge.AdjNum;
			//Pay splits should never be associated to both a procedure and an adjustment at the same time.
			if(split.ProcNum > 0 && split.AdjNum > 0) {
				//Always prefer the procedure over the adjustment unless the account entry passed in is an adjustment.
				if(charge.GetType()==typeof(Adjustment)) {
					split.ProcNum=0;
				}
				else {
					split.AdjNum=0;
				}
			}
			split.UnearnedType=charge.UnearnedType;
			//PaySplits for TP procedures should always set the UnearnedType to the TpUnearnedType preference.
			if(charge.GetType()==typeof(Procedure) && ((Procedure)charge.Tag).ProcStatus==ProcStat.TP) {
				split.UnearnedType=PrefC.GetLong(PrefName.TpUnearnedType);
			}
			if(!isManual && (Math.Abs(charge.AmountEnd)<Math.Abs(payAmt) || textAmount==0)) {
				//Not a manual charge and user wants to make a split for the full charge amount.
				split.SplitAmt=(double)charge.AmountEnd;
				charge.AmountEnd=0;
			}
			else {//Either a manual charge or a partial payment.
				split.SplitAmt=(double)payAmt;
				charge.AmountEnd-=payAmt;
			}
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
		#endregion

		#region AutoSplit
		public static AutoSplit AutoSplitForPayment(long patNum,Payment paymentCur,bool isIncomeTxfr=false,bool isPatPrefer=false,long payPlanNum=0) {
			//No need to check RemotingRole; no call to db.
			return AutoSplitForPayment(Patients.GetFamily(patNum).GetPatNums(),
				patNum,
				new List<PaySplit>(),
				paymentCur,
				new List<AccountEntry>(),
				isIncomeTxfr,
				isPatPrefer,
				null,
				payPlanNum:payPlanNum);
		}

		/// <summary>Leave loadData blank for doRefreshData to be true and get a new copy of the objects.</summary>
		public static AutoSplit AutoSplitForPayment(List<long> listPatNums,long patCurNum,List<PaySplit> listSplitsCur,Payment payCur,
			List<AccountEntry> listPayFirstAcctEntries,bool isIncomeTxfr,bool isPatPrefer,LoadData loadData,bool doAutoSplit=true,
			bool doIncludeExplicitCreditsOnly=false,long payPlanNum=0)
		{
			ConstructResults constructResults=ConstructAndLinkChargeCredits(listPatNums,patCurNum,listSplitsCur,payCur
				,listPayFirstAcctEntries,isIncomeTxfr,isPatPrefer,loadData,doIncludeExplicitCreditsOnly);
			AutoSplit autoSplit=AutoSplitForPayment(constructResults,doAutoSplit,payPlanNum:payPlanNum);
			return autoSplit;
		}

		public static AutoSplit AutoSplitForPayment(ConstructResults constructResults,bool doAutoSplit=true,long payPlanNum=0) {
			AutoSplit autoSplitData=new AutoSplit();
			autoSplitData.ListAccountCharges=constructResults.ListAccountCharges;
			autoSplitData.ListSplitsCur=constructResults.ListSplitsCur;
			autoSplitData.Payment=constructResults.Payment;
			//Create Auto-splits for the current payment to any remaining non-zero charges FIFO by date.
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.DontEnforce) {
				return autoSplitData;
			}
			if(!doAutoSplit) {
				return autoSplitData;
			}
			//Get a subset of the account charges that can have value auto split to them.
			List<AccountEntry> listAutoSplitAccountEntries=autoSplitData.ListAccountCharges.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
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
			//Create a variable to keep track of the money that can be allocated for this payment.
			double amtToAllocate=autoSplitData.Payment.PayAmt;
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
				PaySplit split=new PaySplit();
				split.IsNew=true;
				split.DatePay=autoSplitData.Payment.PayDate;
				split.PatNum=charge.PatNum;
				split.ProvNum=charge.ProvNum;
				split.ClinicNum=charge.ClinicNum;
				//A split should never be attached to both a procedure and an adjustment at the same time.
				//Therefore, the split being created for an adjustment account entry should ignore any attached procedure so that the adjustment is paid.
				split.ProcNum=(charge.GetType()==typeof(Adjustment) ? 0 : charge.ProcNum);
				split.PayPlanChargeNum=charge.PayPlanChargeNum;
				split.PayPlanNum=charge.PayPlanNum;
				split.AdjNum=charge.AdjNum;
				split.PayNum=autoSplitData.Payment.PayNum;
				split.UnearnedType=charge.UnearnedType;
				split.SplitAmt=splitAmt;
				//Remove the value from the account entry
				charge.AmountEnd-=(decimal)splitAmt;
				amtToAllocate-=splitAmt;
				charge.SplitCollection.Add(split);
				autoSplitData.ListAutoSplits.Add(split);
			}
			//Create an unearned split if there is any remaining money to allocate.
			if(!CompareDouble.IsZero(amtToAllocate)) {
				PaySplit split=new PaySplit();
				split.SplitAmt=amtToAllocate;
				amtToAllocate=0;
				split.DatePay=autoSplitData.Payment.PayDate;
				split.PatNum=autoSplitData.Payment.PatNum;
				split.ProvNum=(PrefC.IsODHQ) ? 7 : 0;//Jordan's ProvNum for HQ.
				split.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
				if(PrefC.HasClinicsEnabled) {
					split.ClinicNum=autoSplitData.Payment.ClinicNum;
				}
				split.PayNum=autoSplitData.Payment.PayNum;
				autoSplitData.ListAutoSplits.Add(split);
			}
			autoSplitData.Payment.PayAmt=amtToAllocate;
			return autoSplitData;
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

		public static void DeleteTransfersForFamily(Family fam) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fam);
				return;
			}
			//Get all income transfer payments for the family prior to the date specified.
			List<Payment> listDeleteIncomeTransfers=Payments.GetTransfers(fam.GetPatNums().ToArray());
			if(listDeleteIncomeTransfers.Count==0) {
				return;//No transfers to even consider deleting.
			}
			string command;
			List<long> listPreservePayNums=new List<long>();
			List<long> listHiddenUnearnedPayTypes=PaySplits.GetHiddenUnearnedDefNums();
			if(listHiddenUnearnedPayTypes.Count > 0) {
				//Some income transfers may have splits that are associated to a hidden type. These transfers must not be deleted (via TaskNum 2806662).
				command=$@"SELECT DISTINCT PayNum FROM paysplit
				WHERE PayNum IN({string.Join(",",listDeleteIncomeTransfers.Select(x => POut.Long(x.PayNum)))})
				AND UnearnedType IN({string.Join(",",listHiddenUnearnedPayTypes.Select(x => POut.Long(x)))})";
				listPreservePayNums.AddRange(Db.GetListLong(command));
			}
			//Some income transfers may have splits that are associated to patients in different families. These transfers must not be deleted.
			command=$@"SELECT DISTINCT PayNum FROM paysplit
				WHERE PayNum IN({string.Join(",",listDeleteIncomeTransfers.Select(x => POut.Long(x.PayNum)))})
				AND PatNum NOT IN({string.Join(",",fam.GetPatNums().Select(x => POut.Long(x)))})";
			listPreservePayNums.AddRange(Db.GetListLong(command));
			//Remove any income transfers that need to be preserved from our list of payments to delete.
			listDeleteIncomeTransfers.RemoveAll(x => ListTools.In(x.PayNum,listPreservePayNums));
			//Delete all income transfers that are left in the list of transfers that are 'safe' to delete.
			for(int i=0;i<listDeleteIncomeTransfers.Count;i++) {
				//Some income transfers will not be able to be deleted. Do not let one failure spoil the entire batch.
				//Users will be able to manually try and delete these income transfers and will get a warning message as to why it can't be deleted.
				ODException.SwallowAnyException(() => Payments.Delete(listDeleteIncomeTransfers[i].PayNum));
			}
		}

		///<summary>Throws exceptions.</summary>
		public static void TransferClaimsPayAsTotal(long patNum,List<long> listFamPatNums,string logText) {
			ClaimProcs.FixClaimsNoProcedures(listFamPatNums);
			if(!ProcedureCodes.GetContainsKey("ZZZFIX")) {
				Cache.Refresh(InvalidType.ProcCodes);//Refresh local cache only because middle tier has already inserted the signal.
			}
			ClaimTransferResult claimTransferResult=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamPatNums);
			if(claimTransferResult!=null && claimTransferResult.ListInsertedClaimProcs.Count > 0) {//valid and items were created
				SecurityLogs.MakeLogEntry(Permissions.ClaimProcReceivedEdit,patNum,logText);
			}
		}

		///<summary></summary>
		public static bool TryCreateIncomeTransfer(List<AccountEntry> listAllAccountEntries,DateTime datePay,out IncomeTransferData incomeTransferData,
			long guarNum=0,List<PayPlan> listPayPlans=null)
		{
			//No remoting role check; no call to db
			incomeTransferData=new IncomeTransferData();
			if(listAllAccountEntries.IsNullOrEmpty()) {
				return true;
			}
			//Do not allow transfers if there is a payment plan associated to the family that is for an amount that does not equal the Tx amount.
			//E.g. A "Total Tx Amt" not equal to the "Total Amount" means the user is using a patient payment plan and didn't attach Tx Credits.
			//This is a requirement for the transfer system because it needs to know what to take value from and what to give it to (pat/prov/clinic).
			//Side note, dynamic payment plans can always be used in the imcome transfer manager.
			if(listPayPlans==null) {
				Family famCur=Patients.GetFamily(listAllAccountEntries.First().PatNum);
				listPayPlans=PayPlans.GetForPats(famCur.GetPatNums(),famCur.Guarantor.Guarantor).FindAll(x => !x.IsDynamic);
			}
			if(!listPayPlans.IsNullOrEmpty()) {
				//PayPlanCharge Credits are not made when the PaymentPlanVersion is set to NoCharges.
				//For now, do not allow income transfers to be made when the version is set to NoCharges because we don't know what has value to transfer.
				if(PrefC.GetEnum<PayPlanVersions>(PrefName.PayPlansVersion)==PayPlanVersions.NoCharges) {
					incomeTransferData.StringBuilderErrors.AppendLine(Lans.g("PaymentEdit","Transfers cannot be made while 'Pay Plan charge logic' is set to "+
						$"'{PayPlanVersions.NoCharges.GetDescription()}'."));
					return false;
				}
				List<long> listInvalidTotalPayPlanNums=new List<long>();
				List<long> listInvalidNegPayPlanNums=new List<long>();
				Dictionary<long,List<PayPlanCharge>> dictPayPlanCharges=PayPlanCharges.GetForPayPlans(listPayPlans.Select(x => x.PayPlanNum).ToList())
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				foreach(long payPlanNum in dictPayPlanCharges.Keys) {
					//The total Principal of all credits must equate to the total Principal of all debits.
					double txTotalAmt=PayPlans.GetTxTotalAmt(dictPayPlanCharges[payPlanNum]);//credits
					double totalCost=PayPlans.GetTotalPrinc(payPlanNum,dictPayPlanCharges[payPlanNum]);//debits
					if(!CompareDouble.IsEqual(txTotalAmt,totalCost)) {
						listInvalidTotalPayPlanNums.Add(payPlanNum);
					}
					//There isn't a reliable way to discern negative credits from adjustment credits.
					//Negative non-adjustment credits don't make sense (at least to me) so we are not going to allow income transfer to be made if these exist.
					//However, it is still allowed to have negative adjustment credits.
					//The key to negative adjustment credits is that they have corresponding negative adjustment debits (to bring the value of the plan down).
					//Therefore, there must be an equal ratio of negative debits and negative credits present.
					double totalNegDebits=dictPayPlanCharges[payPlanNum]
						.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && CompareDouble.IsLessThanZero(x.Principal))
						.Sum(x => x.Principal);
					double totalNegCredits=dictPayPlanCharges[payPlanNum]
						.FindAll(x => x.ChargeType==PayPlanChargeType.Credit && CompareDouble.IsLessThanZero(x.Principal))
						.Sum(x => x.Principal);
					if(!CompareDouble.IsEqual(totalNegDebits,totalNegCredits)) {
						listInvalidNegPayPlanNums.Add(payPlanNum);
					}
				}
				if(listInvalidTotalPayPlanNums.Count > 0 || listInvalidNegPayPlanNums.Count > 0) {
					incomeTransferData.StringBuilderErrors.AppendLine(Lans.g("PaymentEdit","Transfers cannot be made for this family at this time."));
				}
				if(listInvalidTotalPayPlanNums.Count > 0) {
					string errorMsgStart=Lans.g("PaymentEdit","The following payment plans have a 'Total Tx Amt' that does not match the 'Total Amount':");
					List<PayPlan> listInvalidPayPlans=listPayPlans.FindAll(x => ListTools.In(x.PayPlanNum,listInvalidTotalPayPlanNums));
					incomeTransferData.StringBuilderErrors.AppendLine(GetInvalidPayPlanDescription(errorMsgStart,listInvalidPayPlans,dictPayPlanCharges));
					return false;
				}
				if(listInvalidNegPayPlanNums.Count > 0) {
					string errorMsgStart=Lans.g("PaymentEdit","The following payment plans have an imbalance between negative debits and negative credits:");
					List<PayPlan> listInvalidPayPlans=listPayPlans.FindAll(x => ListTools.In(x.PayPlanNum,listInvalidNegPayPlanNums));
					incomeTransferData.StringBuilderErrors.AppendLine(GetInvalidPayPlanDescription(errorMsgStart,listInvalidPayPlans,dictPayPlanCharges));
					return false;
				}
			}
			listAllAccountEntries.Sort(AccountEntrySort);
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
				incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,guarNum,datePay,ref listUnearnedUnallocated,ref listAllAccountEntries));
			}
			#endregion
			#region Payment Plans
			//Move incorrectly allocated splits from payment plans to unearned before going through the AccountBalancingLayers.
			PreprocessPayPlanSplits(ref listAllAccountEntries,ref incomeTransferData,datePay);
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
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,guarNum,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
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
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,guarNum,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
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
				incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,guarNum,datePay,ref listAccountEntriesNoPP,ref listAllAccountEntries));
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
					&& x.IsUnearned
					&& CompareDecimal.IsLessThanZero(x.AmountEnd));
				listPayPlanEntries.AddRange(listUnearnedEntries);
				//Loop through all of the income transfer layers except the Unearned layer which will be handled manually afterwards.
				foreach(AccountBalancingLayers layer in Enum.GetValues(typeof(AccountBalancingLayers))) {
					if(layer==AccountBalancingLayers.Unearned) {
						continue;//Unearned is special and gets handled after this loop.
					}
					incomeTransferData.MergeIncomeTransferData(TransferForLayer(layer,guarNum,datePay,ref listPayPlanEntries,ref listAllAccountEntries));
				}
			}
			#endregion
			#region Unearned
			incomeTransferData.AppendLine($"Processing for layer: {AccountBalancingLayers.Unearned.ToString()}...");
			//Transfer all remaining excess production to unearned keeping the same pat/prov/clinic.
			//Only consider procedures and adjustments because payment plan entries should not be allowed to go into the negative.
			//The scenarios where that would be possible should have been blocked or transferred to unearned above.
			List<AccountEntry> listNegativeProduction=listAllAccountEntries.FindAll(x => ListTools.In(x.GetType(),typeof(Procedure),typeof(Adjustment))
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
			foreach(IncomeTransferBucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingAdjIncome)) {
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
			foreach(IncomeTransferBucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingProcIncome)) {
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
			foreach(IncomeTransferBucket buckets in CreateTransferBucketsForLayer(AccountBalancingLayers.ProvPatClinic,listRemainingUnallocatedIncome)) {
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
		private static IncomeTransferData TransferForLayer(AccountBalancingLayers layer,long guarNum,DateTime datePay,
			ref List<AccountEntry> listProcessAccountEntries,ref List<AccountEntry> listAllAccountEntries)
		{
			//No remoting role check; private method
			IncomeTransferData incomeTransferData=new IncomeTransferData();
			List<IncomeTransferBucket> listBuckets=CreateTransferBucketsForLayer(layer,listProcessAccountEntries);
			//Preprocess the production explicitly linked to procedures within each bucket on layer ProvPatClinic.
			if(layer==AccountBalancingLayers.ProvPatClinic) {
				PreprocessProvPatClinicBuckets(ref listBuckets,ref listProcessAccountEntries,ref incomeTransferData,datePay,ref listAllAccountEntries);
			}
			incomeTransferData.AppendLine($"Processing buckets for layer: {layer.ToString()}...");
			//Process each bucket and make any necessary income transfers for this layer.
			//Create 'account entries' for any transfers that were created so that subsequent layers know about the transfers from previous layers.
			foreach(IncomeTransferBucket bucket in listBuckets) {
				incomeTransferData.MergeIncomeTransferData(TransferLoopHelper(bucket,guarNum,datePay));
			}
			return incomeTransferData;
		}

		///<summary>Groups up the account entries passed in into buckets based on the layer passed in.</summary>
		private static List<IncomeTransferBucket> CreateTransferBucketsForLayer(AccountBalancingLayers layer,List<AccountEntry> listAccountEntries) {
			//No remoting role check; private method
			switch(layer) {
				case AccountBalancingLayers.ProvPatClinic:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.ProvPat:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.ProvClinic:
					return listAccountEntries.GroupBy(x => new { x.ProvNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.PatClinic:
					return listAccountEntries.GroupBy(x => new { x.PatNum,x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Prov:
					return listAccountEntries.GroupBy(x => new { x.ProvNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Pat:
					return listAccountEntries.GroupBy(x => new { x.PatNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Clinic:
					return listAccountEntries.GroupBy(x => new { x.ClinicNum })
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new IncomeTransferBucket(x.Value))
						.ToList();
				case AccountBalancingLayers.Nothing:
					return new List<IncomeTransferBucket>() {
						new IncomeTransferBucket(listAccountEntries)
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
			List<AccountEntry> listLinkedPayPlanSplits=listAccountEntries.FindAll(x => x.PayPlanNum > 0
				&& x.GetType()==typeof(PaySplit)
				&& (x.AdjNum > 0 || x.ProcNum > 0));
			var dictLinkedPayPlanSplits=listLinkedPayPlanSplits
				.GroupBy(x => new { x.PayPlanNum,x.PatNum,x.ProvNum,x.ClinicNum,x.ProcNum,x.AdjNum,x.UnearnedType })
				.ToDictionary(x => x.Key,x => x.ToList());
			foreach(var kvp in dictLinkedPayPlanSplits) {
				List<AccountEntry> listPayPlanEntries=kvp.Value;
				decimal offsetAmt=listPayPlanEntries.Sum(x => x.AmountEnd);
				if(CompareDecimal.IsGreaterThanOrEqualToZero(offsetAmt)) {
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
		}

		///<summary>Breaks up the individual buckets passed in into sub-buckets that are grouped by procedures.
		///Loops through each sub-bucket and removes as much production as possible (allocating AmountEnd).
		///Creates a transfer if there is a net negative after all production has been allocated.
		///There shouldn't be such a thing as negative production and if there is then it should be transferred to unearned for later transfers.
		///Also, balances any unallocated / unearned income that is not explicitly linked to a procedure for all buckets passed in.
		///This last step is so that we don't get stuck in infinite loops transferring income between unallocated and unearned.</summary>
		private static void PreprocessProvPatClinicBuckets(ref List<IncomeTransferBucket> listBuckets,ref List<AccountEntry> listProcessAccountEntries,
			ref IncomeTransferData incomeTransferData,DateTime datePay,ref List<AccountEntry> listAllAccountEntries)
		{
			//No remoting role check; private method
			incomeTransferData.AppendLine($"Preprocessing buckets for ProvPatClinic...");
			foreach(IncomeTransferBucket bucket in listBuckets) {
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
					List<AccountEntry> listBucketEntries=payPlanGroup.Value;
					listBucketEntries.AddRange(listPayPlanEntries.FindAll(x => !ListTools.In(x,listBucketEntries)));
					//Skip adjustments because they directly manipulate the value of production and should not be transferred to unearned at this point.
					listBucketEntries.RemoveAll(x => x.GetType()==typeof(FauxAccountEntry) && ((FauxAccountEntry)x).IsAdjustment);
					if(payPlanGroup.Key==0) {//Account entries are not associated to any payment plans.
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
					Dictionary<long,List<AccountEntry>> dictUnearnedEntries=listBucketEntries.GroupBy(x => x.UnearnedType).ToDictionary(x => x.Key,x => x.ToList());
					foreach(long unearnedType in dictUnearnedEntries.Keys) {
						decimal offsetAmt=dictUnearnedEntries[unearnedType].Sum(x => x.AmountEnd);
						List<PaySplit> listPaySplits=CreateUnearnedTransfer(offsetAmt,patNum,provNum,clinicNum,ref incomeTransferData,unearnedType:unearnedType,
							payPlanNum:payPlanNum,datePay:datePay);
						AccountEntry accountEntryOffset=new AccountEntry(listPaySplits[0]);
						AccountEntry accountEntryUnearned=new AccountEntry(listPaySplits[1]);
						//The PaySplits that were just created will technically offset any production or income that left this bucket in the negative.
						//Zero out the AmountEnd field for every negative account entry so that they are not transferred.
						dictUnearnedEntries[unearnedType].FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd)).ForEach(x => x.AmountEnd=0);
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
		private static IncomeTransferData TransferLoopHelper(IncomeTransferBucket bucket,long guarNum,DateTime datePay) {
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
					transferData.MergeIncomeTransferData(CreateTransferHelper(posCharge,negCharge,guarNum,datePay));
				}
				if(hasTransfer) {
					transferData.AppendLine($"  Done - {posCharge.Description}");
				}
			}
			return transferData;
		}

		///<summary>Creates and links paysplits with micro-allocations based on the charges passed in. Constructs the paysplits with all necessary information depending on the type of the charges.</summary>
		private static IncomeTransferData CreateTransferHelper(AccountEntry posCharge,AccountEntry negCharge,long guarNum,DateTime datePay) {
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
			#region Positive Split
			PaySplit posSplit=new PaySplit();
			posSplit.DatePay=datePay;
			posSplit.ClinicNum=posCharge.ClinicNum;
			posSplit.PatNum=posCharge.PatNum;
			posSplit.PayPlanNum=posCharge.PayPlanNum;
			posSplit.AdjNum=posCharge.AdjNum;
			//A split should never be attached to both a procedure and an adjustment at the same time.
			//Therefore, the split being created for an adjustment account entry should ignore any attached procedure so that the adjustment is paid.
			posSplit.ProcNum=(posCharge.GetType()==typeof(Adjustment) ? 0 : posCharge.ProcNum);
			posSplit.ProvNum=posCharge.ProvNum;
			posSplit.SplitAmt=(double)amt;
			posSplit.UnearnedType=posCharge.UnearnedType;
			#endregion
			#region Negative Split
			PaySplit negSplit=new PaySplit();
			negSplit.DatePay=datePay;
			negSplit.ClinicNum=negCharge.ClinicNum;
			negSplit.PatNum=negCharge.PatNum;
			negSplit.PayPlanNum=negCharge.PayPlanNum;
			negSplit.AdjNum=negCharge.AdjNum;
			//A split should never be attached to both a procedure and an adjustment at the same time.
			//Therefore, the split being created for an adjustment account entry should ignore any attached procedure so that the adjustment is paid.
			negSplit.ProcNum=(negCharge.GetType()==typeof(Adjustment) ? 0 : negCharge.ProcNum);
			negSplit.ProvNum=negCharge.ProvNum;
			negSplit.SplitAmt=0-(double)amt;
			negSplit.UnearnedType=negCharge.UnearnedType;
			#endregion
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
			long procNum=0;
			long adjNum=0;
			long unearnedType=0;
			if(accountEntry.GetType()==typeof(PaySplit)) {
				procNum=((PaySplit)accountEntry.Tag).ProcNum;
				unearnedType=((PaySplit)accountEntry.Tag).UnearnedType;
			}
			else if(accountEntry.GetType()==typeof(Procedure)) {
				procNum=((Procedure)accountEntry.Tag).ProcNum;
			}
			else if(accountEntry.GetType()==typeof(Adjustment)) {
				adjNum=((Adjustment)accountEntry.Tag).AdjNum;
			}
			return CreateUnearnedTransfer(accountEntry.AmountEnd,accountEntry.PatNum,accountEntry.ProvNum,accountEntry.ClinicNum,ref incomeTransferData,
				procNum:procNum,adjNum:adjNum,unearnedType:unearnedType,payPlanNum:accountEntry.PayPlanNum,datePay:datePay);
		}

		///<summary>Makes a split to take splitAmount away from unearned and then makes an offsetting split of splitAmount using the values passed in.
		///Always returns exactly two PaySplits. The first will be the 'offset' split and the second will be the split taking from unearned.</summary>
		private static List<PaySplit> CreateUnearnedTransfer(decimal splitAmount,long patNum,long provNum,long clinicNum,
			ref IncomeTransferData incomeTransferData,long procNum=0,long adjNum=0,long unearnedType=0,long payPlanNum=0,DateTime datePay=default)
		{
			//No remoting role check; private method
			long offsetUnearnedType=0;
			//Payment plans are the only entities that need the ability to transfer unearned to unearned.
			if(payPlanNum > 0) {
				offsetUnearnedType=unearnedType;
			}
			if(datePay.Year < 1880) {
				datePay=DateTime.Today;
			}
			PaySplit offsetSplit=new PaySplit() {
				AdjNum=adjNum,
				ClinicNum=clinicNum,
				DatePay=datePay,
				PatNum=patNum,
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
				PatNum=patNum,
				PayPlanNum=0,
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
			foreach(AccountEntry positiveEntry in listAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd))) {
				if(CompareDecimal.IsLessThanOrEqualToZero(positiveEntry.AmountEnd)) {
					continue;
				}
				foreach(AccountEntry negativeEntry in listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd))) {
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
					PaySplit splitOffset=new PaySplit() {
						AdjNum=positiveEntry.AdjNum,
						ClinicNum=positiveEntry.ClinicNum,
						DatePay=datePay,
						PatNum=positiveEntry.PatNum,
						PayPlanNum=positiveEntry.PayPlanNum,
						ProcNum=positiveEntry.ProcNum,
						ProvNum=positiveEntry.ProvNum,
						SplitAmt=(double)amountTxfr,//Positive payment splits technically remove value from unearned buckets.
						UnearnedType=positiveEntry.UnearnedType,
					};
					PaySplit splitAllocate=new PaySplit() {
						AdjNum=negativeEntry.AdjNum,
						ClinicNum=negativeEntry.ClinicNum,
						DatePay=datePay,
						PatNum=negativeEntry.PatNum,
						PayPlanNum=negativeEntry.PayPlanNum,
						ProcNum=negativeEntry.ProcNum,
						ProvNum=negativeEntry.ProvNum,
						SplitAmt=0-(double)amountTxfr,//Negative payment splits technically give value to unearned buckets.
						UnearnedType=negativeEntry.UnearnedType,
					};
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

		#endregion

		#region Income Transfer FIFO

		///<summary>Returns a list of splits that should be inserted into the database in order to balance the patient's account using FIFO logic while ignoring patient, provider, and clinic mismatches.</summary>
		public static IncomeTransferData GetIncomeTransferDataFIFO(long patNum,DateTime datePay,DateTime dateAsOf=default) {
			List<PaySplit> listPaySplits=new List<PaySplit>();
			//Get the family account as it stands.
			Family fam=Patients.GetFamily(patNum);
			ConstructChargesData constructChargesData=GetConstructChargesData(fam.GetPatNums(),patNum,new List<PaySplit>(),0,true);
			List<AccountEntry> listAccountEntries=ConstructListCharges(constructChargesData.ListProcs,
				constructChargesData.ListAdjustments,
				constructChargesData.ListPaySplits,
				constructChargesData.ListInsPayAsTotal,
				constructChargesData.ListPayPlanCharges,
				constructChargesData.ListPayPlanLinks,
				true,
				constructChargesData.ListClaimProcsFiltered,
				constructChargesData.ListPayPlans.FindAll(x => x.PlanNum > 0));
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
					incomeTransferData=CreateTransferHelper(listAccountEntriesOutstandingProduction[i],listAccountEntriesUnallocatedUnearned[j],0,datePay);
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
		}

		///<summary>The data needed to construct a list of charges for FormPayment.</summary>
		[Serializable]
		public class ConstructChargesData {
			///<summary>List from the db, completed for pat. Not list of pre-selected procs from acct. Contains TP procs if pref is set to ON.</summary>
			public List<Procedure> ListProcs=new List<Procedure>();
			public List<Adjustment> ListAdjustments=new List<Adjustment>();
			///<summary>Current list of all splits from database</summary>
			public List<PaySplit> ListPaySplits=new List<PaySplit>();
			///<summary>Stores the summed outstanding ins pay as totals (amounts and write offs) for the list of patnums</summary>
			public List<PayAsTotal> ListInsPayAsTotal=new List<PayAsTotal>();
			public List<PayPlan> ListPayPlans=new List<PayPlan>();
			public List<PaySplit> ListPayPlanSplits=new List<PaySplit>();
			///<summary>List of all pay plan charges (including future charges).  Does not include insurance pay plan charges.</summary>
			public List<PayPlanCharge> ListPayPlanCharges=new List<PayPlanCharge>();
			///<summary>Stores the list of claimprocs (not ins pay as totals) for the list of pat nums</summary>
			public List<ClaimProc> ListClaimProcsFiltered=new List<ClaimProc>();
			///<summary>List of all pay plan links for the pay plans.</summary>
			public List<PayPlanLink> ListPayPlanLinks=new List<PayPlanLink>();
		}

		///<summary>Data retrieved upon initialization. AutpSplit stores data retireved from going through list of charges, linking,and autosplitting.</summary>
		[Serializable]
		public class InitData {
			public AutoSplit AutoSplitData;
			public Dictionary<long,Patient> DictPats=new Dictionary<long,Patient>();
			public decimal SplitTotal;
		}

		/// <summary>Data resulting after making a payment.</summary>
		[Serializable]
		public class PayResults {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
		}

		/// <summary>Data results after constructing list of charges and linking credits to them.</summary>
		[Serializable]
		public class ConstructResults {
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
		}

		/// <summary>Data after autosplitting. ListAutoSplits is separate from ListSplitsCur./// </summary>
		[Serializable]
		public class AutoSplit {
			public List<PaySplit> ListAutoSplits=new List<PaySplit>();
			public Payment Payment;
			public List<AccountEntry> ListAccountCharges=new List<AccountEntry>();
			public List<PaySplit> ListSplitsCur=new List<PaySplit>();
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
				PaySplit offsetSplit=new PaySplit();
				offsetSplit.DatePay=DateTime.Today;
				offsetSplit.PatNum=parentSplit.PatNum;
				offsetSplit.PayNum=payNum;
				offsetSplit.ProvNum=parentSplit.ProvNum;
				offsetSplit.ClinicNum=parentSplit.ClinicNum;
				offsetSplit.UnearnedType=parentSplit.UnearnedType;
				offsetSplit.ProcNum=parentSplit.ProcNum;
				offsetSplit.AdjNum=parentSplit.AdjNum;
				offsetSplit.SplitAmt=(transferAmtOverride==0 ? parentSplit.SplitAmt : transferAmtOverride)*-1;
				PaySplit allocationSplit=new PaySplit();
				allocationSplit.DatePay=DateTime.Today;
				allocationSplit.PatNum=parentSplit.PatNum;
				allocationSplit.PayNum=payNum;
				allocationSplit.ProvNum=parentSplit.ProvNum;
				allocationSplit.ClinicNum=parentSplit.ClinicNum;
				allocationSplit.ProcNum=parentSplit.ProcNum;
				allocationSplit.AdjNum=parentSplit.AdjNum;
				allocationSplit.SplitAmt=transferAmtOverride==0 ? parentSplit.SplitAmt : transferAmtOverride;
				allocationSplit.UnearnedType=parentSplit.UnearnedType;
				if(isTransferToUnearned) {
					allocationSplit.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
				}
				transferReturning.ListSplitsCur.AddRange(new List<PaySplit> { offsetSplit,allocationSplit });
				return transferReturning;
			}
		}

		///<summary>Helper class for organizing account entries when processing income transfer layers.</summary>
		public class IncomeTransferBucket {
			///<summary>All account entries for the current bucket.</summary>
			public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();

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

			public IncomeTransferBucket(List<AccountEntry> listAccountEntries) {
				ListAccountEntries=listAccountEntries;
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

		#endregion
	}

	///<summary>Holds three dictionaries containing the procedures, adjustments not attached to procedures, and payplancharge debits on a family's account. The dictionary keys are the Fkeys to the production items, and the dictionary values are the remaining balance of the production items. Unearned, adjustments, and insurance estimates for created claims are taken into account.</summary>
	public class FamilyProdBalances {
		///<summary>Holds the remaining balance of all procedures on a family's account.</summary>
		private Dictionary<long,decimal> _dictProcedureAmounts=new Dictionary<long,decimal>();
		///<summary>Holds the remaining balance of all adjustments not attached to procedures on a family's account.</summary>
		private Dictionary<long,decimal> _dictAdjustmentAmounts=new Dictionary<long,decimal>();
		///<summary>Holds the remaining balance of all payplancharges on a family's account.</summary>
		private Dictionary<long,decimal> _dictPayPlanChargeAmounts=new Dictionary<long,decimal>();

		///<summary></summary>
		public FamilyProdBalances(long guarantorPatNum) {
			List<AccountEntry> listAccountEntries=PaymentEdit.ConstructAndLinkChargeCredits(guarantorPatNum).ListAccountCharges;
			GetAmountsFromAccountEntries(listAccountEntries);
		}

		///<summary>Makes dictionary entries for each of the procedures, adjustments not attached to procedures, and payplancharges in the list.</summary>
		private void GetAmountsFromAccountEntries(List<AccountEntry> listAccountEntries) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<listAccountEntries.Count;i++) {
				AccountEntry accountEntryCur=listAccountEntries[i];
				Type type=accountEntryCur.GetType();
				if(type==typeof(Procedure)) {
					_dictProcedureAmounts.Add(accountEntryCur.ProcNum,accountEntryCur.AmountEnd);
				}
				else if(type==typeof(Adjustment)) {
					_dictAdjustmentAmounts.Add(accountEntryCur.AdjNum,accountEntryCur.AmountEnd);
				}
				else if(type==typeof(FauxAccountEntry)) {
					if(((FauxAccountEntry)accountEntryCur).ChargeType!=PayPlanChargeType.Debit) {
						continue;
					}
					//There can be multiple FauxAccountEntries for a single PayPlanCharge, so we sum the AmountEnds as we iterate over the list.
					if(_dictPayPlanChargeAmounts.ContainsKey(accountEntryCur.PayPlanChargeNum)) {
						_dictPayPlanChargeAmounts[accountEntryCur.PayPlanChargeNum]+=accountEntryCur.AmountEnd;
					}
					else {
						_dictPayPlanChargeAmounts.Add(accountEntryCur.PayPlanChargeNum,accountEntryCur.AmountEnd);
					}
				}
			}
		}

		///<summary>Returns true if any of the three dictionaries contain an entry for the production item represented by the StatementProd passed in. Assigns the balance of the production item to the out peram.</summary>
		public bool GetAmountForStatementProdIfExists(StatementProd statementProd,out decimal amount) {
			//No need to check RemotingRole; no call to db.
			bool doesExist=false;
			amount=0;
			switch(statementProd.ProdType) {
				case ProductionType.Procedure:
					doesExist=_dictProcedureAmounts.TryGetValue(statementProd.FKey,out decimal procAmount);
					amount=procAmount;
					break;
				case ProductionType.Adjustment:
					doesExist=_dictAdjustmentAmounts.TryGetValue(statementProd.FKey,out decimal adjAmount);
					amount=adjAmount;
					break;
				case ProductionType.PayPlanCharge:
					doesExist=_dictPayPlanChargeAmounts.TryGetValue(statementProd.FKey,out decimal payPlanChargeAmount);
					amount=payPlanChargeAmount;
					break;
			}
			return doesExist;
		}

		///<summary>Removes an entry from one of the three dictionaries for the production item represented by the StatementProd.</summary>
		public void RemoveEntryForStatementProd(StatementProd statementProd) {
			//No need to check RemotingRole; no call to db.
			switch(statementProd.ProdType) {
				case ProductionType.Procedure:
					_dictProcedureAmounts.Remove(statementProd.FKey);
					break;
				case ProductionType.Adjustment:
					_dictAdjustmentAmounts.Remove(statementProd.FKey);
					break;
				case ProductionType.PayPlanCharge:
					_dictPayPlanChargeAmounts.Remove(statementProd.FKey);
					break;
			}
		}
	}
}
