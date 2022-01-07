using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayPlanCharges {
		#region Get Methods
		///<summary></summary>
		public static List<PayPlanCharge> GetForDownPayment(PayPlanTerms terms,Family family,List<PayPlanLink> listPayPlanLinks,PayPlan payplan) {
			//No remoting role check; no call to db
			//Create a temporary variable to keep track of the original PeriodPayment.
			decimal periodPaymentTemp=terms.PeriodPayment;
			double aprTemp=terms.APR;
			//Set the PeriodPayment to the current DownPayment so that the full amount of the down payment gets generated.
			//E.g. there are several procedures attached to the payment plan and the down payment only covers one and a half (partial proc).
			terms.PeriodPayment=(decimal)terms.DownPayment;
			terms.APR=0;//downpayments should pay on principal only
			List<PayPlanCharge> listDownPayments=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,family,listPayPlanLinks,payplan,true
				,true,new List<PaySplit>());
			listDownPayments.ForEach(x => {
				x.Note="Down Payment";
				x.ChargeDate=DateTime.Today.Date;
				x.Interest=0;
			});
			//Put the PeriodPayment back to the way it was upon entry.
			terms.PeriodPayment=periodPaymentTemp;
			terms.APR=aprTemp;
			return listDownPayments;
		}

		///<summary>Gets all payplancharges for a specific payment plan.</summary>
		public static List<PayPlanCharge> GetForPayPlan(long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			string command=
				"SELECT * FROM payplancharge "
				+"WHERE PayPlanNum="+POut.Long(payPlanNum)
				+" ORDER BY ChargeDate";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Returns a list of payplancharges associated to the passed in payplannums.  Will return a blank list if none.</summary>
		public static List<PayPlanCharge> GetForPayPlans(List<long> listPayPlanNums) {
			if(listPayPlanNums==null || listPayPlanNums.Count<1) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listPayPlanNums);
			}
			string command=
				"SELECT * FROM payplancharge "
				+"WHERE PayPlanNum IN ("+POut.String(String.Join(",",listPayPlanNums)) +") "
				+"ORDER BY ChargeDate";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Gets all payplan charges for the payplans passed in where the specified patient is the Guarantor.  Based on today's date.  
		///Will return both credits and debits.  Does not return insurance payment plan charges.</summary>
		public static List<PayPlanCharge> GetDueForPayPlan(PayPlan payPlan,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),payPlan,patNum);
			}			
			return GetDueForPayPlans(new List<PayPlan> { payPlan },patNum);
		}

		///<summary>Gets all payplan charges for the payplans passed in where the specified patient is the Guarantor.  Based on today's date.  
		///Will return both credits and debits.  Does not return insurance payment plan charges.</summary>
		public static List<PayPlanCharge> GetDueForPayPlans(List<PayPlan> listPayPlans,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listPayPlans,patNum);
			}
			if(listPayPlans.Count < 1) {
				return new List<PayPlanCharge>();
			}
			string command="SELECT payplancharge.* FROM payplan "
				+"INNER JOIN payplancharge ON payplancharge.PayPlanNum = payplan.PayPlanNum "
					+"AND payplancharge.ChargeDate <= "+DbHelper.Curdate()+" "
				+"WHERE payplan.Guarantor="+POut.Long(patNum)+" "
				+"AND payplan.PayPlanNum IN("+String.Join(", ",listPayPlans.Select(x => x.PayPlanNum).ToList())+") "
				+"AND payplan.PlanNum = 0 "; //do not return insurance payment plan charges.
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Gets all payplan charges for the payplans passed in where the any of the patients in the list are the Guarantor or the patient on the
		///payment plan.  Will return both credits and debits.  Does not return insurance payment plan charges.</summary>
		public static List<PayPlanCharge> GetForPayPlans(List<long> listPayPlans,List<long> listPatNums) {
			if(listPayPlans.IsNullOrEmpty() || listPatNums.IsNullOrEmpty()) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listPayPlans,listPatNums);
			}
			string command="SELECT payplancharge.* FROM payplan "
				+"INNER JOIN payplancharge ON payplancharge.PayPlanNum = payplan.PayPlanNum "
				+"WHERE (payplan.PatNum IN("+string.Join(",",listPatNums)+") OR payplan.Guarantor IN("+string.Join(",",listPatNums)+")) "
				+"AND payplan.PayPlanNum IN("+string.Join(", ",listPayPlans)+") "
				+"AND payplan.PlanNum = 0 "; //do not return insurance payment plan charges.
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<PayPlanCharge> GetChargesForPayPlanChargeType(long payPlanNum,PayPlanChargeType chargeType) {
			//No need to check RemotingRole; no call to db.
			return GetChargesForPayPlanChargeType(new List<long>() { payPlanNum },chargeType);
		}

		///<summary></summary>
		public static List<PayPlanCharge> GetChargesForPayPlanChargeType(List<long> listPayPlanNums,PayPlanChargeType chargeType) {
			if(listPayPlanNums.IsNullOrEmpty()) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listPayPlanNums,chargeType);
			}
			string command="SELECT * FROM payplancharge "
				+"WHERE PayPlanNum IN("+string.Join(",",listPayPlanNums.Select(x => POut.Long(x)))+") "
				+"AND ChargeType="+POut.Int((int)chargeType);
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Gets all charges of the credit type that don't have a procnum of 0 and belong to any of the pats in listPatNums</summary>
		public static List<PayPlanCharge> GetAllProcCreditsForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$"SELECT * FROM payplancharge WHERE payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)} " +
				$"AND payplancharge.ProcNum!=0 AND payplancharge.PatNum IN ({string.Join(",",listPatNums)})";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Takes a procNum and returns a list of all payment plan charges associated to the procedure.
		///Returns an empty list if there are none.</summary>
		public static List<PayPlanCharge> GetFromProc(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command=$"SELECT * FROM payplancharge WHERE payplancharge.ProcNum={POut.Long(procNum)} OR (payplancharge.LinkType=" +
				$"{POut.Int((int)PayPlanLinkType.Procedure)} AND payplancharge.FKey={POut.Long(procNum)})";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary>Gets a list of all payment plan charges of type Credit associated to the procedures for patient payment plans.</summary>
		public static List<PayPlanCharge> GetPatientPayPlanCreditsForProcs(List<long> listProcNums) {
			if(listProcNums.Count==0) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command=$"SELECT * FROM payplancharge WHERE payplancharge.ProcNum IN({string.Join(",",listProcNums.Select(x => POut.Long(x)))})" +
				$" AND payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)}";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static PayPlanCharge GetOne(long payPlanChargeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PayPlanCharge>(MethodBase.GetCurrentMethod(),payPlanChargeNum);
			}
			string command=
				"SELECT * FROM payplancharge "
				+"WHERE PayPlanChargeNum="+POut.Long(payPlanChargeNum);
			return Crud.PayPlanChargeCrud.SelectOne(command);
		}

		///<summary>Gets a list of charges for the passed in fkey and link type (i.e. adjustment, procedure...)</summary>
		public static List<PayPlanCharge> GetForLinkTypeAndFKeys(PayPlanLinkType linkType,params long[] arrayFKeys) {
			if(arrayFKeys.IsNullOrEmpty()) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),linkType,arrayFKeys);
			}
			string command=$"SELECT * FROM payplancharge "+
				$"WHERE payplancharge.FKey IN({string.Join(",",arrayFKeys.Select(x => POut.Long(x)))}) "+
				$"AND payplancharge.LinkType={POut.Int((int)linkType)}";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}

		public static List<PayPlanCharge> GetForProcs(List<long> listProcNums) {
			if(listProcNums.IsNullOrEmpty()) {
				return new List<PayPlanCharge>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlanCharge>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command=$"SELECT * FROM payplancharge WHERE payplancharge.ProcNum IN ({string.Join(",",listProcNums)}) ";
			return Crud.PayPlanChargeCrud.SelectMany(command);
		}
		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(PayPlanCharge charge) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				charge.PayPlanChargeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),charge);
				return charge.PayPlanChargeNum;
			}
			return Crud.PayPlanChargeCrud.Insert(charge);
		}
		
		///<summary></summary>
		public static void InsertMany(List<PayPlanCharge> listPayPlanCharges) {
			if(listPayPlanCharges.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlanCharges);
				return;
			}
			Crud.PayPlanChargeCrud.InsertMany(listPayPlanCharges);
		}
		#endregion

		#region Update
		///<summary>Takes a procNum and updates all of the dates of the payment plan charge credits associated to it.
		///If a completed procedure is passed in, it will update all of the payment plan charges associated to it to the ProcDate. 
		///If a non-complete procedure is passed in, it will update the charges associated to MaxValue.
		///Does nothing if there are no charges attached to the passed-in procedure.</summary>
		public static void UpdateAttachedPayPlanCharges(Procedure proc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			#region PayPlanCharge.ChargeDate
			List<PayPlanCharge> listCharges=GetFromProc(proc.ProcNum);
			List<PayPlan> listPayPlans=PayPlans.GetAllForCharges(listCharges);
			foreach(PayPlanCharge chargeCur in listCharges) {
				PayPlan planForCharge=listPayPlans.FirstOrDefault(x=>x.PayPlanNum==chargeCur.PayPlanNum);
				if(planForCharge.IsDynamic) { //Dynamic payment plan charges only get issued when they're due, thus should not have their dates changed.
					continue;
				}
				chargeCur.ChargeDate=DateTime.MaxValue;
				//Since Dynamic PaymentPlans only create charge debits that are explicitly linked to procs,
				//we need to check for Procs going back to treatment planned, otherwise the debit will be removed.
				if(proc.ProcStatus==ProcStat.C) 
				{
					chargeCur.ChargeDate=proc.ProcDate;
				}
				Update(chargeCur); //one update statement for each payplancharge.
			}
			#endregion
			#region PayPlan.CompletedAmt
			//The list of payment plans is guaranteed to have every patient payment plan that is associated to the procedure at this point.
			//However, it is not guaranteed to have every dynamic payment plan associated to the procedure (only debits are stored in the payplancharge table).
			PayPlans.UpdateTreatmentCompletedAmt(listPayPlans);
			//Refresh the list of payment plans so that every dynamic payment plan associated to the procedure is present.
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForFKeyAndLinkType(proc.ProcNum,PayPlanLinkType.Procedure);
			long[] listPayPlanNums=listPayPlanLinks.Select(x => x.PayPlanNum).ToArray();
			listPayPlans=PayPlans.GetMany(listPayPlanNums);
			PayPlans.UpdateTreatmentCompletedAmtsDynamicPaymentPlan(listPayPlans);
			#endregion
		}

		///<summary></summary>
		public static void Update(PayPlanCharge charge){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),charge);
				return;
			}
			Crud.PayPlanChargeCrud.Update(charge);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Must always pass in payPlanNum.</summary>
		public static void Sync(List<PayPlanCharge> listPayPlanCharges,long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlanCharges,payPlanNum);
				return;
			}
			List<PayPlanCharge> listDB=PayPlanCharges.GetForPayPlan(payPlanNum);
			Crud.PayPlanChargeCrud.Sync(listPayPlanCharges,listDB);
		}
		#endregion

		#region Delete
		///<summary>Will delete all PayPlanCharges associated to the passed-in procNum from the database.  Does nothing if the procNum = 0.</summary>
		public static void DeleteForProc(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum);
				return;
			}
			if(procNum==0) {
				return;
			}
			List<PayPlan> listPayPlans=PayPlans.GetAllForCharges(GetFromProc(procNum));
			string command="DELETE FROM payplancharge WHERE ProcNum="+POut.Long(procNum);
			Db.NonQ(command);
			PayPlans.UpdateTreatmentCompletedAmt(listPayPlans);
		}

		///<summary></summary>
		public static void Delete(PayPlanCharge charge){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),charge);
				return;
			}
			string command= "DELETE from payplancharge WHERE PayPlanChargeNum = '"
				+POut.Long(charge.PayPlanChargeNum)+"'";
 			Db.NonQ(command);
		}	

		public static void DeleteMany(List<long> listCharges) {
			if(listCharges.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCharges);
				return;
			}
			string command=$"DELETE from payplancharge WHERE PayPlanChargeNum IN ({string.Join(",",listCharges.Select(x => POut.Long(x)))})";
			Db.NonQ(command);
		}
		#endregion	
	}
}