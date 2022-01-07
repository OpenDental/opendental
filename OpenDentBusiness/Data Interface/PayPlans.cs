using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayPlans {
		#region Get Methods
		///<summary>Gets a list of all payplans for a given patient, whether they are the patient or the guarantor.  This is only used in one place, when deleting a patient to check dependencies.</summary>
		public static int GetDependencyCount(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM payplan"
				+" WHERE PatNum = "+POut.Long(patNum)
				+" OR Guarantor = "+POut.Long(patNum);
			return PIn.Int(Db.GetScalar(command));
		}

		public static PayPlan GetOne(long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PayPlan>(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			return Crud.PayPlanCrud.SelectOne(payPlanNum);
		}

		public static List<PayPlan> GetMany(params long[] arrayPayPlanNums) {
			if(arrayPayPlanNums.IsNullOrEmpty()) {
				return new List<PayPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),arrayPayPlanNums);
			}
			string command=$"SELECT * FROM payplan WHERE PayPlanNum IN ({string.Join(",",arrayPayPlanNums.Select(x => POut.Long(x)))})";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Returns a list of payment plans where the patient of the payment plan matches ANY in listPatNums OR the guarantor matches patNum.
		///patNum will typically be the current patient.</summary>
		public static List<PayPlan> GetForPats(List<long> listPatNums,long guarantor) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),listPatNums,guarantor);
			}
			//We have to check for guarantor separately in case the payment plan belongs to a patient in another family.
			string command="SELECT * FROM payplan WHERE Guarantor="+POut.Long(guarantor);
			if(!listPatNums.IsNullOrEmpty()) {
				command+=" OR PatNum IN("+String.Join(",",listPatNums)+")";
			}
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Gets All patient pay plans for each patient in listPatNums.</summary>
		public static List<PayPlan> GetAllPatPayPlansForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<PayPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$"SELECT * FROM payplan WHERE payplan.PatNum IN({string.Join(",",listPatNums)}) AND payplan.InsSubNum=0";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Gets all payment plans that this patient is associated to.  
		///Will return payment plans that this pat is the patient or guarantor of.</summary>
		public static List<PayPlan> GetForPatNum(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM payplan "
				+"WHERE PatNum = "+POut.Long(patNum)+ " "
				+"OR Guarantor = "+POut.Long(patNum);
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Returns a list of overcharged payplans from the listPayPlanNums. Only necessary for Dynamic Payment Plans. 
		///Returns an empty list if none are overcharged.</summary>
		public static List<PayPlan> GetOverChargedPayPlans(List<long> listPayPlanNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),listPayPlanNums);
			}
			#region Get Data
			List<PayPlanLink> listPayPlanLinksAll=PayPlanLinks.GetForPayPlans(listPayPlanNums);
			List<long> listProcedureLinkFKeys=listPayPlanLinksAll.Where(x => x.LinkType==PayPlanLinkType.Procedure).Select(x => x.FKey).ToList();
			List<long> listAdjustmentLinkFKeys=listPayPlanLinksAll.Where(x => x.LinkType==PayPlanLinkType.Adjustment).Select(x => x.FKey).ToList();
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlans(listPayPlanNums);
			List<Procedure> listProcsAttachedToPayPlan=Procedures.GetManyProc(listProcedureLinkFKeys,false);
			List<Adjustment> listAdjsAttachedToPayPlan=Adjustments.GetMany(listAdjustmentLinkFKeys);
			List<ClaimProc> listClaimProcsForProcs=ClaimProcs.GetForProcs(listProcedureLinkFKeys);
			List<Adjustment> listAdjForProcs=Adjustments.GetForProcs(listProcedureLinkFKeys);
			List<PaySplit> listSplitsForProcs=PaySplits.GetPaySplitsFromProcs(listProcedureLinkFKeys);
			List<PaySplit> listSplitsForAdjustments=PaySplits.GetForAdjustments(listAdjustmentLinkFKeys);
			#endregion Get Data
			List<long> listPayPlansOvercharged=new List<long>();
			foreach(long payPlanNum in listPayPlanNums) {
				List<PayPlanLink> listLinksForPayPlan=listPayPlanLinksAll.FindAll(x => x.PayPlanNum==payPlanNum);
				//Get total amount that has been debited for the current pay plan thus far.
				decimal amtDebitedTotal=listPayPlanCharges.FindAll(x => x.PayPlanNum==payPlanNum && x.ChargeType==PayPlanChargeType.Debit)
					.Sum(x => (decimal)x.Principal);
				#region Sum Linked Production
				decimal totalPrincipalForPayPlan=0;
				foreach(PayPlanLink payPlanLink in listLinksForPayPlan) {
					PayPlanProductionEntry productionEntry=null;
					if(payPlanLink.LinkType==PayPlanLinkType.Procedure) {
						Procedure proc=listProcsAttachedToPayPlan.FirstOrDefault(x => x.ProcNum==payPlanLink.FKey);
						if(proc!=null) {
							List<Adjustment> listExplicitAdjs=listAdjForProcs.FindAll(x => x.ProcNum==proc.ProcNum
								&& x.PatNum==proc.PatNum
								&& x.ProvNum==proc.ProvNum
								&& x.ClinicNum==proc.ClinicNum);
							productionEntry=new PayPlanProductionEntry(proc,payPlanLink,listClaimProcsForProcs,listExplicitAdjs,listSplitsForProcs);
						}
					}
					else if(payPlanLink.LinkType==PayPlanLinkType.Adjustment) {
						Adjustment adj=listAdjsAttachedToPayPlan.FirstOrDefault(x => x.AdjNum==payPlanLink.FKey);
						if(adj!=null) {
							productionEntry=new PayPlanProductionEntry(adj,payPlanLink,listSplitsForAdjustments);
						}
					}
					if(productionEntry!=null) {
						if(productionEntry.AmountOverride==0) {
							totalPrincipalForPayPlan+=productionEntry.AmountOriginal;
						}
						else {
							totalPrincipalForPayPlan+=productionEntry.AmountOverride;
						}
					}
				}
				#endregion Sum Linked Production
				//If the total that has been debited thus far exceeds the total principal for the pay plan, it is overcharged.
				if(CompareDecimal.IsGreaterThan(amtDebitedTotal,totalPrincipalForPayPlan)) {
					listPayPlansOvercharged.Add(payPlanNum);
				}
			}
			return GetMany(listPayPlansOvercharged.ToArray()).FindAll(x => x.IsDynamic);
		}

		///<summary>Determines if there are any valid plans with that patient as the guarantor.</summary>
		public static List<PayPlan> GetValidPlansNoIns(long guarNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),guarNum);
			}
			string command="SELECT * FROM payplan"
				+" WHERE Guarantor = "+POut.Long(guarNum)
				+" AND PlanNum = 0"
				+" AND IsClosed = 0"
				+" ORDER BY payplandate";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		public static List<PayPlan> GetAllOpenInsPayPlans() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM payplan WHERE payplan.PlanNum != 0 AND IsClosed = 0";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Gets all insurance payplans that aren't fully paid for patients associated to the claims passed in.
		///Only returns payplans that have no claimprocs linked to them, or those that have claimprocs linked to them 
		///that are also linked to one of the claims passed in.</summary>
		public static List<PayPlan> GetAllValidInsPayPlansForClaims(List<Claim> listClaims) {
			if(listClaims.IsNullOrEmpty()) {
				return new List<PayPlan>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),listClaims);
			}
			string command="SELECT payplan.* FROM payplan "
				+"LEFT JOIN claimproc ON claimproc.PayPlanNum=payplan.PayPlanNum	"
				//Only ins payplans
				+"WHERE payplan.PlanNum!=0 "
				//Only ones for patients from the list of claims.
				+$"AND payplan.PatNum IN ({string.Join(",",listClaims.Select(x => POut.Long(x.PatNum)))}) "
				//Only ones with no claimprocs attached or only claimprocs from the list of claims.
				+$"AND (claimproc.ClaimNum IS NULL OR claimproc.ClaimNum IN ({string.Join(",",listClaims.Select(x => POut.Long(x.ClaimNum)))})) "
				+"GROUP BY payplan.PayPlanNum "
				//Only ones that are not fully paid off.
				+"HAVING payplan.CompletedAmt>SUM(COALESCE(claimproc.InsPayAmt,0)) "
				+"ORDER BY payplan.PayPlanDate";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		///<summary>Get all payment plans for this patient with the insurance plan identified by PlanNum and InsSubNum attached (marked used for tracking expected insurance payments) that have not been paid in full.  Only returns plans with no claimprocs currently attached or claimprocs from the claim identified by the claimNum sent in attached.  If claimNum is 0 all payment plans with planNum, insSubNum, and patNum not paid in full will be returned.</summary>
		public static List<PayPlan> GetValidInsPayPlans(long patNum,long planNum,long insSubNum,long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),patNum,planNum,insSubNum,claimNum);
			}
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="SELECT payplan.*,MAX(claimproc.ClaimNum) ClaimNum";
			}
			else {
				command+="SELECT payplan.PayPlanNum,payplan.PatNum,payplan.Guarantor,payplan.PayPlanDate,"
					+"payplan.APR,payplan.Note,payplan.PlanNum,payplan.CompletedAmt,payplan.InsSubNum,MAX(claimproc.ClaimNum) ClaimNum";
			}
			command+=" FROM payplan"
				+" LEFT JOIN claimproc ON claimproc.PayPlanNum=payplan.PayPlanNum"
				+" WHERE payplan.PatNum="+POut.Long(patNum)
				+" AND payplan.PlanNum="+POut.Long(planNum)
				+" AND payplan.InsSubNum="+POut.Long(insSubNum);
			if(claimNum>0) {
				command+=" AND (claimproc.ClaimNum IS NULL OR claimproc.ClaimNum="+POut.Long(claimNum)+")";//payplans with no claimprocs attached or only claimprocs from the same claim
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+=" GROUP BY payplan.PayPlanNum";
			}
			else {
				command+=" GROUP BY payplan.PayPlanNum,payplan.PatNum,payplan.Guarantor,payplan.PayPlanDate,"
					+"payplan.APR,payplan.Note,payplan.PlanNum,payplan.CompletedAmt,payplan.InsSubNum";
			}
			command+=" HAVING payplan.CompletedAmt>SUM(COALESCE(claimproc.InsPayAmt,0))";//has not been paid in full yet
			if(claimNum==0) {//if current claimproc is not attached to a claim, do not return payplans with claimprocs from existing claims already attached
				command+=" AND (MAX(claimproc.ClaimNum) IS NULL OR MAX(claimproc.ClaimNum)=0)";
			}
			command+=" ORDER BY payplan.PayPlanDate";
			DataTable payPlansWithClaimNum=Db.GetTable(command);
			List<PayPlan> retval=new List<PayPlan>();
			for(int i=0;i<payPlansWithClaimNum.Rows.Count;i++) {
				PayPlan planCur=new PayPlan();
				planCur.PayPlanNum=PIn.Long(payPlansWithClaimNum.Rows[i]["PayPlanNum"].ToString());
				planCur.PatNum=PIn.Long(payPlansWithClaimNum.Rows[i]["PatNum"].ToString());
				planCur.Guarantor=PIn.Long(payPlansWithClaimNum.Rows[i]["Guarantor"].ToString());
				planCur.PayPlanDate=PIn.Date(payPlansWithClaimNum.Rows[i]["PayPlanDate"].ToString());
				planCur.APR=PIn.Double(payPlansWithClaimNum.Rows[i]["APR"].ToString());
				planCur.Note=payPlansWithClaimNum.Rows[i]["Note"].ToString();
				planCur.PlanNum=PIn.Long(payPlansWithClaimNum.Rows[i]["PlanNum"].ToString());
				planCur.CompletedAmt=PIn.Double(payPlansWithClaimNum.Rows[i]["CompletedAmt"].ToString());
				planCur.InsSubNum=PIn.Long(payPlansWithClaimNum.Rows[i]["InsSubNum"].ToString());
				if(claimNum>0 && payPlansWithClaimNum.Rows[i]["ClaimNum"].ToString()==claimNum.ToString()) {
					//if a payplan exists with claimprocs from the same claim as the current claimproc attached, always only return that one payplan
					//claimprocs from one claim are not allowed to be attached to different payplans
					retval.Clear();
					retval.Add(planCur);
					break;
				}
				retval.Add(planCur);
			}
			return retval;
		}

		///<summary>Executes a LINQ statement that returns the total amount of tx that is both completed and planned for the passed in payment plan.
		///Only used for payplans v2.  Different from the TxCompletedAmt, which looks ONLY at PayPlanCharge credits that have already occurred. 
		///Does not update or make any calls to the database, as TxTotalAmt is not a db column.</summary>
		public static double GetTxTotalAmt(List<PayPlanCharge> listCharges) {
			//no need to check RemotingRole; no call to db.
			if(listCharges.IsNullOrEmpty()) {
				return 0;
			}
			return listCharges.Where(x => x.ChargeType==PayPlanChargeType.Credit)
				.Sum(x => x.Principal);
		}

		/// <summary>Gets info directly from database. Used from PayPlan and Account windows to get the amount paid so far on one payment plan.</summary>
		public static double GetAmtPaid(PayPlan payPlan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),payPlan);
			}
			string command;
			if(payPlan.PlanNum==0) {//Patient payment plan
				command="SELECT SUM(paysplit.SplitAmt) FROM paysplit "
					+"WHERE paysplit.PayPlanNum = "+POut.Long(payPlan.PayPlanNum)+" "
					+"GROUP BY paysplit.PayPlanNum";
			}
			else {//Insurance payment plan
				command="SELECT SUM(claimproc.InsPayAmt) "
					+"FROM claimproc "
					+"WHERE claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+","
						+POut.Int((int)ClaimProcStatus.CapClaim)+") "
					+"AND claimproc.PayPlanNum="+POut.Long(payPlan.PayPlanNum);
			}
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return 0;
			}
			return PIn.Double(table.Rows[0][0].ToString());
		}

		///<summary>Used from FormPayPlan and the Account to get the accumulated amount due for a payment plan based on today's date.  
		///Includes interest, but does not include payments made so far.  The chargelist must include all charges for this payplan, 
		///but it can include more as well.</summary>
		public static double GetAccumDue(long payPlanNum, List<PayPlanCharge> chargeList){
			//No need to check RemotingRole; no call to db.
			double retVal=0;
			for(int i=0;i<chargeList.Count;i++){
				if(chargeList[i].PayPlanNum!=payPlanNum){
					continue;
				}
				if(chargeList[i].ChargeDate>DateTime.Today){//not due yet
					continue;
				}
				if(chargeList[i].ChargeType!=PayPlanChargeType.Debit) { //for v1, debits are the only ChargeType.
					continue;
				}
				retVal+=chargeList[i].Principal+chargeList[i].Interest;
			}
			return retVal;
		}

		///<summary>Gets the amount due now of the passed in payment plan num. 
		///Optionally pass in the list of PayPlanCharges and list of PaySplits to avoid unneccesary database calls.
		///Will filter out paysplits and charges associated to different payplans as well as payplan charges that are for the future or have a charge type of debit. </summary>
		public static double GetDueNow(long payPlanNum,List<PayPlanCharge> listPayPlanCharges = null,List<PaySplit> listPaySplits = null) {
			//No need to check RemotingRole; no call to db.
			double amtDue=0;
			if(listPayPlanCharges==null) {
				listPayPlanCharges=PayPlanCharges.GetForPayPlan(payPlanNum);
			}
			if(listPaySplits==null) {
				listPaySplits=PaySplits.GetFromBundled(PaySplits.GetForPayPlan(payPlanNum));
			}
			foreach(PayPlanCharge chargeCur in listPayPlanCharges) {
				if(chargeCur.PayPlanNum == payPlanNum
					&& chargeCur.ChargeType == PayPlanChargeType.Debit
					&& chargeCur.ChargeDate <= DateTime.Today) 
				{
					amtDue+=chargeCur.Principal + chargeCur.Interest;
				}
			}
			foreach(PaySplit splitCur in listPaySplits) {
				if(splitCur.PayPlanNum == payPlanNum) {
					amtDue-=splitCur.SplitAmt;
				}
			}
			return amtDue;
		}

		///<summary>Gets the current balance of the passed in payment plan num. 
		///Performs the same calculation as the "balance" column in the payment plans grid in ContrAccount.
		///Optionally pass in the list of PayPlanCharges and list of PaySplits to avoid unneccesary database calls.
		///Will filter out paysplits and charges associated to different payplans as well as payplan charges that are for the future or have a charge type of debit. </summary>
		public static double GetBalance(long payPlanNum,List<PayPlanCharge> listPayPlanCharges = null,List<PaySplit> listPaySplits = null) {
			//No need to check RemotingRole; no call to db.
			double amtBal=0;
			if(listPayPlanCharges==null) {
				listPayPlanCharges=PayPlanCharges.GetForPayPlan(payPlanNum);
			}
			if(listPaySplits==null) {
				listPaySplits=PaySplits.GetFromBundled(PaySplits.GetForPayPlan(payPlanNum));
			}
			foreach(PayPlanCharge chargeCur in listPayPlanCharges) {
				if(chargeCur.PayPlanNum == payPlanNum
					&& chargeCur.ChargeType == PayPlanChargeType.Debit) 
				{
					amtBal+=chargeCur.Principal;
					if(chargeCur.ChargeDate<=DateTime.Today) {
						amtBal+=chargeCur.Interest;
					}
				}
			}
			foreach(PaySplit splitCur in listPaySplits) {
				if(splitCur.PayPlanNum == payPlanNum) {
					amtBal-=splitCur.SplitAmt;
				}
			}
			return amtBal;
		}

		///<summary>Gets the total cost now of the passed in payment plan num. 
		///Optionally pass in the list of PayPlanCharges to avoid unneccesary database calls.
		///Will filter out charges associated to different payplans as well as payplan charges that are for the future or have a charge type of debit. </summary>
		public static double GetTotalCost(long payPlanNum,List<PayPlanCharge> listPayPlanCharges = null) {
			//No need to check RemotingRole; no call to db.
			double amtTotal=0;
			if(listPayPlanCharges==null) {
				listPayPlanCharges=PayPlanCharges.GetForPayPlan(payPlanNum);
			}
			foreach(PayPlanCharge chargeCur in listPayPlanCharges) {
				if(chargeCur.PayPlanNum == payPlanNum
					&& chargeCur.ChargeType == PayPlanChargeType.Debit) {
					amtTotal+=chargeCur.Principal + chargeCur.Interest;
				}
			}
			return amtTotal;
		}

		public static List<PayPlan> GetAllForCharges(List<PayPlanCharge> listCharge) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod(),listCharge);
			}
			if(listCharge.Count==0) {
				return new List<PayPlan>();
			}
			string command="SELECT * FROM payplan "
				+"WHERE PayPlanNum IN ("+String.Join(",",listCharge.Select(x => x.PayPlanNum))+")";
			return Crud.PayPlanCrud.SelectMany(command);
		}

		/// <summary>Used from Account window to get the amount paid so far on one payment plan.  
		/// Must pass in the total amount paid and the returned value will not be more than this.  
		/// The chargelist must include all charges for this payplan, but it can include more as well.  
		/// It will loop sequentially through the charges to get just the principal portion.</summary>
		public static double GetPrincPaid(double amtPaid,long payPlanNum,List<PayPlanCharge> chargeList) {
			//No need to check RemotingRole; no call to db.
			//amtPaid gets reduced to 0 throughout this loop.
			double retVal=0;
			for(int i=0;i<chargeList.Count;i++){
				if(chargeList[i].PayPlanNum!=payPlanNum){
					continue;
				}
				if(chargeList[i].ChargeType!=PayPlanChargeType.Debit) { //for v1, debits are the only ChargeType.
					continue;
				}
				//For this charge, first apply payment to interest
				if(amtPaid>chargeList[i].Interest){
					amtPaid-=chargeList[i].Interest;
				}
				else{//interest will eat up the remainder of the payment
					amtPaid=0;
					break;
				}
				//Then, apply payment to principal
				if(amtPaid>chargeList[i].Principal){
					retVal+=chargeList[i].Principal;
					amtPaid-=chargeList[i].Principal;
				}
				else{//principal will eat up the remainder of the payment
					retVal+=amtPaid;
					amtPaid=0;
					break;
				}
			}
			return retVal;
		}

		/// <summary>Used from Account and ComputeBal to get the total amount of the original principal for one payment plan.
		/// Does not include any interest. The chargelist must include all charges for this payplan, but it can include more as well.</summary>
		public static double GetTotalPrinc(long payPlanNum,List<PayPlanCharge> chargeList) {
			//No need to check RemotingRole; no call to db.
			double retVal=0;
			for(int i=0;i<chargeList.Count;i++){
				if(chargeList[i].PayPlanNum!=payPlanNum){
					continue;
				}
				if(chargeList[i].ChargeType!=PayPlanChargeType.Debit) { //for v1, debits are the only ChargeType.
					continue;
				}
				retVal+=chargeList[i].Principal;
			}
			return retVal;
		}

		///<summary>Gets the hashstring from the provided string that is typically generated from GetStringForSignatureHash().
		///This is done seperate of building the string so that new line replacements can be done when validating signatures before hashing.</summary>
		public static string GetHashStringForSignature(string str) {
			//No need to check RemotingRole; no call to db.
			return Encoding.ASCII.GetString(ODCrypt.MD5.Hash(Encoding.UTF8.GetBytes(str)));
		}

		///<summary>Get all open, dynamic payment plans.</summary>
		public static List<PayPlan> GetDynamic() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PayPlan>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM payplan WHERE payplan.IsDynamic=1 AND payplan.IsClosed=0";
			return Crud.PayPlanCrud.SelectMany(command);
		}
		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(PayPlan payPlan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				payPlan.PayPlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),payPlan);
				return payPlan.PayPlanNum;
			}
			return Crud.PayPlanCrud.Insert(payPlan);
		}

		public static void InsertMany(List<PayPlan> listPayPlans) {
			if(listPayPlans.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlans);
				return;
			}
			Crud.PayPlanCrud.InsertMany(listPayPlans);
		}

		#endregion

		#region Update
		///<summary>Updates the TreatmentCompletedAmt field of the passed in payplans in the database.  
		///Used when a procedure attached to a payment plan charge is set complete or deleted.
		///The treatment completed amount only takes into account payplancharge credits that have already occurred
		///(no charges attached to TP'd procs).</summary>
		public static void UpdateTreatmentCompletedAmt(List<PayPlan> listPayPlans) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlans);
				return;
			}
			foreach(PayPlan payPlanCur in listPayPlans) {
				double completedAmt=0;
				List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payPlanCur.PayPlanNum);
				completedAmt=listCharges.Where(x => x.ChargeType==PayPlanChargeType.Credit)
					.Where(x => x.ChargeDate.Date <= DateTime.Today.Date)
					.Select(x => x.Principal)
					.Sum();
				payPlanCur.CompletedAmt=completedAmt;
				Update(payPlanCur);
			}
		}

		///<summary>Updates the TreatmentCompletedAmt field of the passed in payplans in the database.  
		///Used when a procedure attached to a payment plan charge is set complete or deleted.
		///The treatment completed amount only takes into account payplancharge credits that have already occurred
		///(no charges attached to TP'd procs).</summary>
		public static void UpdateTreatmentCompletedAmtsDynamicPaymentPlan(List<PayPlan> listPayPlans) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayPlans);
				return;
			}
			List<PayPlan> listPayPlansUnique=listPayPlans.FindAll(x=>x.IsDynamic).DistinctBy(x=>x.PayPlanNum).ToList();
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(listPayPlans.Select(x=>x.PayPlanNum).ToList());
			foreach(PayPlan payPlanCur in listPayPlansUnique) {
				List<PayPlanLink> listPayPlanLinksForPlan=listPayPlanLinks.FindAll(x=>x.PayPlanNum==payPlanCur.PayPlanNum);
				List<PayPlanProductionEntry> payPlanProductionEntry=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinksForPlan);
				double completedAmt=0;
				completedAmt=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(payPlanCur,payPlanProductionEntry);
				payPlanCur.CompletedAmt=completedAmt;
				Update(payPlanCur);
			}
		}

		///<summary></summary>
		public static void Update(PayPlan payPlan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payPlan);
				return;
			}
			Crud.PayPlanCrud.Update(payPlan);
		}
		#endregion

		#region Delete
		///<summary>Called from FormPayPlan.  Also deletes all attached payplancharges.  Throws exception if there are any paysplits attached.</summary>
		public static void Delete(PayPlan plan){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),plan);
				return;
			}
			string command;
			if(plan.PlanNum==0 || plan.IsDynamic) {  //Patient payment plan
				command="SELECT COUNT(*) FROM paysplit WHERE PayPlanNum="+POut.Long(plan.PayPlanNum);
				if(Db.GetCount(command)!="0") {
					throw new ApplicationException
						(Lans.g("PayPlans","You cannot delete a payment plan with patient payments attached.  Unattach the payments first."));
				}
			}
			else {  //Insurance payment plan
				command="SELECT COUNT(*) FROM claimproc WHERE PayPlanNum="+POut.Long(plan.PayPlanNum)+" AND claimproc.Status IN ("
					+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+")";
				if(Db.GetCount(command)!="0") {
					throw new ApplicationException
						(Lans.g("PayPlans","You cannot delete a payment plan with insurance payments attached.  Unattach the payments first."));
				}
				//if there are any unreceived items, detach them here, then proceed deleting
				List<ClaimProc> listClaimProcs=ClaimProcs.GetForPayPlans(new List<long> {plan.PayPlanNum});
				foreach(ClaimProc claimProc in listClaimProcs) {
					claimProc.PayPlanNum=0;
					ClaimProcs.Update(claimProc);
				}
			}
			command="DELETE FROM payplancharge WHERE PayPlanNum="+POut.Long(plan.PayPlanNum);
			Db.NonQ(command);
			command=$"DELETE FROM payplanlink WHERE PayPlanNum={POut.Long(plan.PayPlanNum)}";
			Db.NonQ(command);
			command="DELETE FROM payplan WHERE PayPlanNum ="+POut.Long(plan.PayPlanNum);
			Db.NonQ(command);
			command=$"DELETE FROM orthoplanlink WHERE orthoplanlink.FKey={POut.Long(plan.PayPlanNum)} " +
				$"AND orthoplanlink.LinkType IN ({POut.Enum<OrthoPlanLinkType>(OrthoPlanLinkType.PatPayPlan)}," +
				$"{POut.Enum<OrthoPlanLinkType>(OrthoPlanLinkType.InsPayPlan)})";
			Db.NonQ(command);
			CreditCards.RemoveRecurringCharges(plan.PayPlanNum);
		}
		#endregion

		#region Misc Methods
		///<summary>Returns true if the patient passed in has any outstanding non-ins payment plans with them as the guarantor.</summary>
		public static bool HasOutstandingPayPlansNoIns(long guarNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),guarNum);
			}
			string command="SELECT SUM(paysplit.SplitAmt) FROM paysplit "
				+"INNER JOIN payplan ON paysplit.PayPlanNum=payplan.PayPlanNum "
				+"WHERE payplan.PlanNum=0 "
				+"AND payplan.Guarantor="+POut.Long(guarNum);
			double amtPaid=PIn.Double(Db.GetScalar(command));
			command="SELECT SUM(payplancharge.Principal+payplancharge.Interest) FROM payplancharge "
				+"INNER JOIN payplan ON payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"WHERE payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" AND payplan.PlanNum=0 "
				+"AND payplan.Guarantor="+POut.Long(guarNum);
			double totalCost=PIn.Double(Db.GetScalar(command));
			if(totalCost-amtPaid < .01) {
				return false;
			}
			return true;
		}

		/// <summary>Gets info directly from database. Used when adding a payment.</summary>
		public static bool PlanIsPaidOff(long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			string command="SELECT SUM(paysplit.SplitAmt) FROM paysplit "
				+"WHERE PayPlanNum = "+POut.Long(payPlanNum);// +"' "
				//+" GROUP BY paysplit.PayPlanNum";
			double amtPaid=PIn.Double(Db.GetScalar(command));
			command="SELECT SUM(Principal+Interest) FROM payplancharge "
				+"WHERE ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" AND PayPlanNum="+POut.Long(payPlanNum);
			double totalCost=PIn.Double(Db.GetScalar(command));
			if(totalCost-amtPaid < .01) {
				return true;
			}
			return false;
		}

		///<summary>Automatically closes all payment plans that have no future charges and that are paid off.
		///Not really a problem if it fails because the UPDATE statement happens all at once, so at worst, no changes are made to their database.
		///Returns the number of payment plans that were closed.</summary>
		public static long AutoClose() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="";
			DataTable table;
			command="SELECT payplan.PayPlanNum,SUM(payplancharge.Principal) AS Princ,SUM(payplancharge.Interest) AS Interest,"
				+"COALESCE(ps.TotPayments,0) AS TotPay,COALESCE(cp.InsPayments,0) AS InsPay,"
				+"MAX(payplancharge.ChargeDate) AS LastDate "
				+"FROM payplan "
				+"LEFT JOIN payplancharge ON payplancharge.PayPlanNum=payplan.PayPlanNum "
					+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
				+"LEFT JOIN ("
					+"SELECT paysplit.PayPlanNum, SUM(paysplit.SplitAmt) AS TotPayments "
					+"FROM paysplit "
					+"GROUP BY paysplit.PayPlanNum "
				+")ps ON ps.PayPlanNum = payplan.PayPlanNum "
				+"LEFT JOIN ( "
					+"SELECT claimproc.PayPlanNum, SUM(claimproc.InsPayAmt) AS InsPayments "
					+"FROM claimproc "
					+"GROUP BY claimproc.PayPlanNum "
				+")cp ON cp.PayPlanNum = payplan.PayPlanNum "
				+"WHERE payplan.IsClosed = 0 "
				+"GROUP BY payplan.PayPlanNum "
				+"HAVING Princ+Interest <= (TotPay + InsPay) AND LastDate <="+DbHelper.Curdate();
			table=Db.GetTable(command);
			string payPlanNums="";
			for(int i=0;i < table.Rows.Count;i++) {
				if(i!=0) {
					payPlanNums+=",";
				}
				payPlanNums+=table.Rows[i]["PayPlanNum"];
			}
			if(payPlanNums=="") {
				return 0; //no plans to close.
			}
			command="UPDATE payplan SET IsClosed=1 WHERE PayPlanNum IN ("+payPlanNums+")";
			return Db.NonQ(command);
		}
		#endregion
	}

	
}










