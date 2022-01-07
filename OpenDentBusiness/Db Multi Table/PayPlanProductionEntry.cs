using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Helper class for keeping track of information on payment plans, linked production </summary>
	public class PayPlanProductionEntry {
		public PayPlanLink LinkedCredit;
		public decimal AmountOriginal;
		public decimal AmountOverride;
		///<summary>Amount that still needs to be made into payplan charges (debited). If overrides are used, this will remain at the override value.</summary>
		public decimal AmountRemaining;
		[XmlIgnore]
		public object ProductionTag;
		//The following fields are accessors to fields within the ProductionTag object.
		public DateTime ProductionDate;
		///<summary>Contains the primary key for the production entry (ProcNum,AdjNum...etc)</summary>
		public long PriKey;
		public long ProvNum;
		public long ClinicNum;
		public long PatNum;
		///<summary>credit.SecDateTEntry.  Will be MinValue if this credit is new (not inserted into the db yet).</summary>
		public DateTime CreditDate;
		///<summary>A short description of the production entry.  Currently supports procedures and adjustments.
		///Procedures description will be the proc code followed by the layman term or full description.
		///Adjustments description will be 'Adjustment' followed by the textual representation of the adjustment type.</summary>
		public string Description;
		public PayPlanLinkType LinkType;

		[XmlIgnore]
		public long PayPlanNum {
			get {
				return LinkedCredit.PayPlanNum;
			}
		}

		[XmlElement(nameof(ProductionTag))]
		public DtoObject ProductionTagXml {
			get {
				if(ProductionTag==null) {
					return null;
				}
				return new DtoObject(ProductionTag,ProductionTag.GetType());
			}
			set {
				if(value==null) {
					ProductionTag=null;
					return;
				}
				ProductionTag=value.Obj;
			}
		}

		///<summary>Construct a payplanproductionentry item for a procedure. Calculates pat portion.</summary>
		public PayPlanProductionEntry(Procedure proc,PayPlanLink credit,List<ClaimProc> listClaimProcs,List<Adjustment> listAdjustments
			,List<PaySplit> listPaySplits,decimal amountOriginal=decimal.MinValue) 
		{
			ProductionTag=proc;
			LinkedCredit=credit;
			ProductionDate=proc.ProcDate;
			PriKey=proc.ProcNum;
			ProvNum=proc.ProvNum;
			ClinicNum=proc.ClinicNum;
			PatNum=proc.PatNum;
			if(amountOriginal==decimal.MinValue) {
				decimal patPortion=ClaimProcs.GetPatPortion(proc,listClaimProcs,listAdjustments);
				if(proc.ProcStatus==ProcStat.TP) {
					patPortion-=(decimal)proc.DiscountPlanAmt;
				}
				//Get the amount that was paid to the procedure prior to the procedure being attached to a payment plan.
				decimal patPaid=(decimal)listPaySplits.FindAll(x => x.ProcNum==proc.ProcNum && x.PayPlanNum==0 && x.PayPlanChargeNum==0).Sum(x => x.SplitAmt);
				AmountOriginal=patPortion-patPaid;
			}
			else {
				AmountOriginal=amountOriginal;
			}
			AmountOverride=(decimal)credit.AmountOverride;
			AmountRemaining=(AmountOverride==0)?AmountOriginal:AmountOverride;
			CreditDate=credit.SecDateTEntry;
			Description=$"{ProcedureCodes.GetStringProcCode(proc.CodeNum)} - {ProcedureCodes.GetLaymanTerm(proc.CodeNum)}";
			LinkType=PayPlanLinkType.Procedure;
		}

		///<summary>Construct a payplanproductionentry for an UNATTACHED adjustment (attached adjustments get treated as procedures).</summary>
		public PayPlanProductionEntry(Adjustment adj,PayPlanLink credit,List<PaySplit> listPaySplits,decimal amountOriginal=decimal.MinValue) {
			ProductionTag=adj;
			LinkedCredit=credit;
			ProductionDate=adj.AdjDate;
			PriKey=adj.AdjNum;
			ProvNum=adj.ProvNum;
			ClinicNum=adj.ClinicNum;
			PatNum=adj.PatNum;
			if(amountOriginal==decimal.MinValue) {
				//Get the amount that was paid to the adjustment prior to the adjustment being attached to a payment plan.
				decimal patPaid=(decimal)listPaySplits.FindAll(x => x.DatePay<=credit.SecDateTEntry && x.AdjNum==adj.AdjNum && x.PayPlanNum==0 && x.PayPlanChargeNum==0)
					.Sum(x => x.SplitAmt);
				AmountOriginal=(decimal)adj.AdjAmt-patPaid;
			}
			else {
				AmountOriginal=amountOriginal;
			}
			AmountOverride=(decimal)credit.AmountOverride;
			AmountRemaining=(AmountOverride==0)?AmountOriginal:AmountOverride;//Gets set when calculating
			CreditDate=credit.SecDateTEntry;
			Description=$"Adjustment - {Defs.GetName(DefCat.AdjTypes,adj.AdjType)}";
			LinkType=PayPlanLinkType.Adjustment;
		}

		public decimal GetAmountAttached() {
			if(!CompareDecimal.IsEqual(this.AmountOverride,0)) {
				return this.AmountOverride;
			}
			return this.AmountOriginal;
		}

		///<summary>Used as a short way to grab the procNum from a procedure. Returns 0 if not a procedure.</summary>
		public long GetProcNum() {
			if(this.LinkType==PayPlanLinkType.Procedure) {
				return this.PriKey;//just as safeguard in case is called with this link type
			}
			return 0;
		}

		///<summary>Used as a short way to grab the AdjNum from an adjustment. Returns 0 if not an adjustment.</summary>
		public long GetAdjNum() {
			if(this.LinkType==PayPlanLinkType.Adjustment) {
				return ((Adjustment)this.ProductionTag).AdjNum;
			}
			return 0;
		}

		public static List<PayPlanProductionEntry> GetWithAmountRemaining(List<PayPlanLink> listPayPlanLinks,List<PayPlanCharge> listChargesInDB) {
			//calculate remaining amounts for attached production
			List<PayPlanProductionEntry> listCreditsAndProduction=GetProductionForLinks(listPayPlanLinks);//will need to account for newly added
			foreach(PayPlanProductionEntry entry in listCreditsAndProduction) { 
			//find amount remaining for each credit/production object. This will be our basis for caculating estimated remaining charges. 
				if(CompareDecimal.IsEqual(entry.AmountRemaining,0)) {
					continue;
				}
				List<PayPlanCharge> listChargesInDbForEntry=listChargesInDB.FindAll(x => x.LinkType==entry.LinkType && x.FKey==entry.PriKey);
				foreach(PayPlanCharge chargeForEntry in listChargesInDbForEntry) {
					entry.AmountRemaining-=Math.Min((decimal)chargeForEntry.Principal,entry.AmountRemaining);
					if(CompareDecimal.IsEqual(entry.AmountRemaining,0)) {
						break;
					}
				}
			}
			listCreditsAndProduction.RemoveAll(x => CompareDecimal.IsEqual(x.AmountRemaining,0));//only keep the ones we still need to make charges for. 
			return listCreditsAndProduction.OrderBy(x => x.CreditDate).ToList();//It is important to consumers that the result be ordered by CreditDate.
		}

		public static List<PayPlanProductionEntry> GetProductionForLinks(List<PayPlanLink> listCredits) {
			//No remoting role check; no call to db
			List<long> listProcNums=listCredits.FindAll(x => x.LinkType==PayPlanLinkType.Procedure).Select(x => x.FKey).ToList();
			List<long> listAdjNumsForCredits=listCredits.FindAll(x => x.LinkType==PayPlanLinkType.Adjustment).Select(x => x.FKey).ToList();
			List<PayPlanProductionEntry> listPayPlanProductionEntries=new List<PayPlanProductionEntry>(); 
			List<Procedure> listProcedures=Procedures.GetManyProc(listProcNums,false);
			List<Adjustment> listCreditAdjustments=Adjustments.GetMany(listAdjNumsForCredits);
			List<Adjustment> listProcAdjustments=Adjustments.GetForProcs(listProcNums);
			List<ClaimProc> listClaimProcs=ClaimProcs.GetForProcs(listProcNums);//used for calculating patient porition
			List<PaySplit> listAdjPaySplits=PaySplits.GetForAdjustments(listAdjNumsForCredits);
			List<PaySplit> listProcPaySplits=PaySplits.GetPaySplitsFromProcs(listProcNums);
			foreach(PayPlanLink credit in listCredits){
				if(credit.LinkType==PayPlanLinkType.Procedure) {
					Procedure proc=listProcedures.FirstOrDefault(x => x.ProcNum==credit.FKey);
					if(proc!=null) {
						List<Adjustment> listExplicitAdjs=listProcAdjustments.FindAll(x => x.ProcNum==proc.ProcNum
							&& x.PatNum==proc.PatNum
							&& x.ProvNum==proc.ProvNum
							&& x.ClinicNum==proc.ClinicNum);
						listPayPlanProductionEntries.Add(new PayPlanProductionEntry(proc,credit,listClaimProcs,listExplicitAdjs,listProcPaySplits));
					}
				}
				else if(credit.LinkType==PayPlanLinkType.Adjustment) {
					Adjustment adj=listCreditAdjustments.FirstOrDefault(x => x.AdjNum==credit.FKey);
					if(adj!=null) {
						listPayPlanProductionEntries.Add(new PayPlanProductionEntry(adj,credit,listAdjPaySplits));
					}
				}
			}
			return listPayPlanProductionEntries;
		}

		///<summary>Iterates through all passed in PayPlanProductionEntries, and sums up the amounts. 
		///If the PayPlan has Await Complete set, skips over TP procs.</summary>
		public static double GetDynamicPayPlanCompletedAmount(PayPlan payPlan,List<PayPlanProductionEntry> listPayPlanProductionEntries) {
			double completedAmt=0;
			for(int i = 0;i<listPayPlanProductionEntries.Count;i++) {
				PayPlanProductionEntry entry=listPayPlanProductionEntries[i];
				if(entry.LinkType==PayPlanLinkType.Procedure) {
					Procedure procAssociated=(Procedure)entry.ProductionTag;
					if(procAssociated==null) {
						completedAmt+=0;
						continue;
					}
					if((ListTools.In(payPlan.DynamicPayPlanTPOption,DynamicPayPlanTPOptions.AwaitComplete,DynamicPayPlanTPOptions.None)
						&& procAssociated.ProcStatus==ProcStat.C) 
						|| payPlan.DynamicPayPlanTPOption==DynamicPayPlanTPOptions.TreatAsComplete) 
					{
						completedAmt+=(double)entry.GetAmountAttached();
					}
				}
				else { //Adjustments or OrthoCase
					completedAmt+=(double)entry.GetAmountAttached();
				} 
			}
			return completedAmt;
		}
	}
}
