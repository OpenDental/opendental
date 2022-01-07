using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PaySplits {
		#region Get Methods
		///<summary>Returns all paySplits for the given patNum, organized by DatePay.  WARNING! Also includes related paysplits that aren't actually attached to patient.  Includes any split where payment is for this patient.</summary>
		public static PaySplit[] Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PaySplit[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			/*This query was too slow
			string command=
				"SELECT DISTINCT paysplit.* FROM paysplit,payment "
				+"WHERE paysplit.PayNum=payment.PayNum "
				+"AND (paysplit.PatNum = '"+POut.Long(patNum)+"' OR payment.PatNum = '"+POut.Long(patNum)+"') "
				+"ORDER BY DatePay";*/
			//this query goes 10 times faster for very large databases
			string command=@"select DISTINCT paysplitunion.* FROM "
				+"(SELECT DISTINCT paysplit.* FROM paysplit,payment "
				+"WHERE paysplit.PayNum=payment.PayNum and payment.PatNum='"+POut.Long(patNum)+"' "
				+"UNION "
				+"SELECT DISTINCT paysplit.* FROM paysplit,payment "
				+"WHERE paysplit.PayNum = payment.PayNum AND paysplit.PatNum='"+POut.Long(patNum)+"') paysplitunion "
				+"ORDER BY paysplitunion.DatePay";
			return Crud.PaySplitCrud.SelectMany(command).ToArray();
		}

		///<summary>Returns a list of paysplits that have AdjNum of any of the passed in adjustments.</summary>
		public static List<PaySplit> GetForAdjustments(List<long> listAdjustNums) {
			if(listAdjustNums==null || listAdjustNums.Count==0) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listAdjustNums);
			}
			string command="SELECT * FROM paysplit WHERE AdjNum IN ("+string.Join(",",listAdjustNums)+")";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		public static List<PaySplit> GetForPayPlanCharges(List<long> listPayPlanChargeNums) {
			if(listPayPlanChargeNums.IsNullOrEmpty()) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPayPlanChargeNums);
			}
			string command=$"SELECT * FROM paysplit WHERE PayPlanChargeNum IN ({string.Join(",",listPayPlanChargeNums)})";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		///<summary>Used from payment window to get all paysplits for the payment.</summary>
		public static List<PaySplit> GetForPayment(long payNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),payNum);
			}
			string command=
				"SELECT * FROM paysplit "
				+"WHERE PayNum="+POut.Long(payNum);
			return Crud.PaySplitCrud.SelectMany(command);
		}

		///<summary>Gets the splits for all the payments passed in.</summary>
		public static List<PaySplit> GetForPayments(List<long> listPayNums) {
			if(listPayNums.IsNullOrEmpty()) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPayNums);
			}
			string command=
				"SELECT * FROM paysplit "
				+"WHERE PayNum IN("+string.Join(",",listPayNums.Select( x=> POut.Long(x)))+")";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		public static List<PaySplit> GetForProcs(List<long> listProcNums) {
			if(listProcNums.IsNullOrEmpty()) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command=$"SELECT * FROM paysplit WHERE paysplit.ProcNum IN ({string.Join(",",listProcNums)}) ";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		///<summary>Inserts all paysplits with the provided payNum. All paysplits should be for the same payment. </summary>
		public static void InsertMany(long payNum,List<PaySplit> listSplits) {
			//No need to check RemotingRole; no call to db.
			foreach(PaySplit split in listSplits) {
				split.PayNum=payNum;
			}
			InsertMany(listSplits);
		}

		public static void InsertMany(List<PaySplit> listSplits) {
			if(listSplits.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSplits);
				return;
			}
			foreach(PaySplit split in listSplits) {
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				split.SecUserNumEntry=Security.CurUser.UserNum;
			}
			Crud.PaySplitCrud.InsertMany(listSplits);
		}

		///<summary>Gets one paysplit using the specified SplitNum.</summary>
		public static PaySplit GetOne(long splitNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PaySplit>(MethodBase.GetCurrentMethod(),splitNum);
			}
			string command="SELECT * FROM paysplit WHERE SplitNum="+POut.Long(splitNum);
			return Crud.PaySplitCrud.SelectOne(command);
		}

		///<summary>Used from FormPayment to return the total payments for a procedure without requiring a supplied list.</summary>
		public static string GetTotForProc(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT SUM(paysplit.SplitAmt) FROM paysplit "
				+"WHERE paysplit.ProcNum="+POut.Long(procNum);
			return Db.GetScalar(command);

		}

		///<summary>Returns all paySplits for the given procNum. Must supply a list of all paysplits for the patient.</summary>
		public static ArrayList GetForProc(long procNum,PaySplit[] List) {
			//No need to check RemotingRole; no call to db.
			ArrayList retVal=new ArrayList();
			for(int i=0;i<List.Length;i++){
				if(List[i].ProcNum==procNum){
					retVal.Add(List[i]);
				}
			}
			return retVal;
		}

		///<summary>Used from FormAdjust to display and calculate payments for procs attached to adjustments.</summary>
		public static double GetTotForProc(Procedure procCur) {
			//No need to check RemotingRole; no call to db.
			return GetForProcs(ListTools.FromSingle(procCur.ProcNum)).Sum(x => x.SplitAmt);
		}

		///<summary>Used from FormPaySplitEdit.  Returns total payments for a procedure for all paysplits other than the supplied excluded paysplit.</summary>
		public static double GetTotForProc(long procNum,PaySplit[] List,PaySplit paySplitToExclude,out int countSplitsAttached) {
			//No need to check RemotingRole; no call to db.
			double retVal=0;
			countSplitsAttached=0;
			for(int i=0;i<List.Length;i++){
				if(List[i].IsSame(paySplitToExclude)) {
					continue;
				}
				if(List[i].ProcNum==procNum){
					countSplitsAttached++;
					retVal+=List[i].SplitAmt;
				}
			}
			return retVal;
		}

		///<summary>Used once in ContrAccount.  WARNING!  The returned list of 'paysplits' are not real paysplits.  They are actually grouped by patient and date.  Only the DateEntry, SplitAmt, PatNum, and ProcNum(one of many) are filled. Must supply a list which would include all paysplits for this payment.</summary>
		public static ArrayList GetGroupedForPayment(long payNum,PaySplit[] List) {
			//No need to check RemotingRole; no call to db.
			ArrayList retVal=new ArrayList();
			int matchI;
			for(int i=0;i<List.Length;i++){
				if(List[i].PayNum==payNum){
					//find a 'paysplit' with matching DateEntry and patnum
					matchI=-1;
					for(int j=0;j<retVal.Count;j++){
						if(((PaySplit)retVal[j]).DateEntry==List[i].DateEntry && ((PaySplit)retVal[j]).PatNum==List[i].PatNum){
							matchI=j;
							break;
						}
					}
					if(matchI==-1){
						retVal.Add(new PaySplit());
						matchI=retVal.Count-1;
						((PaySplit)retVal[matchI]).DateEntry=List[i].DateEntry;
						((PaySplit)retVal[matchI]).PatNum=List[i].PatNum;
					}
					if(((PaySplit)retVal[matchI]).ProcNum==0 && List[i].ProcNum!=0){
						((PaySplit)retVal[matchI]).ProcNum=List[i].ProcNum;
					}
					((PaySplit)retVal[matchI]).SplitAmt+=List[i].SplitAmt;
				}
			}
			return retVal;
		}

		///<summary>Used in Payment window to get all paysplits for a single patient without using a supplied list.</summary>
		public static List<PaySplit> GetForPats(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM paysplit "
				+"WHERE PatNum IN("+String.Join(", ",listPatNums)+")";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		///<summary>Used once in ContrAccount to just get the splits for a single patient.  The supplied list also contains splits that are not necessarily for this one patient.</summary>
		public static PaySplit[] GetForPatient(long patNum,PaySplit[] List) {
			//No need to check RemotingRole; no call to db.
			ArrayList retVal=new ArrayList();
			for(int i=0;i<List.Length;i++){
				if(List[i].PatNum==patNum){
					retVal.Add(List[i]);
				}
			}
			PaySplit[] retList=new PaySplit[retVal.Count];
			retVal.CopyTo(retList);
			return retList;
		}

		///<summary>For a given PayPlan, returns a table of PaySplits with additional payment information.
		///The additional information from the payment table will be columns titled "CheckNum", "PayAmt", and "PayType"</summary>
		public static DataTable GetForPayPlan(long payPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),payPlanNum);
			}
			string command="SELECT paysplit.*,payment.CheckNum,payment.PayAmt,payment.PayType "
					+"FROM paysplit "
					+"LEFT JOIN payment ON paysplit.PayNum=payment.PayNum "
					+"WHERE paysplit.PayPlanNum="+POut.Long(payPlanNum)+" "
					+"ORDER BY DatePay";
			DataTable tableSplits=Db.GetTable(command);
			return tableSplits;
		}

		///<summary>For a given PayPlan, returns a list of PaySplits associated to that PayPlan.</summary>
		public static List<PaySplit> GetForPayPlans(List<long> listPayPlanNums) {
			if(listPayPlanNums.Count==0) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPayPlanNums);
			}
			string command="SELECT paysplit.* "
					+"FROM paysplit "
					+"WHERE paysplit.PayPlanNum IN ("+POut.String(String.Join(",",listPayPlanNums))+") "
					+"ORDER BY DatePay";
			List<PaySplit> listSplits=Crud.PaySplitCrud.SelectMany(command);
			return listSplits;
		}

		///<summary>Gets paysplits from a provided datatable.  This was originally part of GetForPayPlan but can't be because it's passed through the Middle Tier.</summary>
		public static List<PaySplit> GetFromBundled(DataTable dataTable) {
			//No need to check RemotingRole; no call to db.
			return Crud.PaySplitCrud.TableToList(dataTable);
		}

		///<summary>Used once in ContrAccount.  Usually returns 0 unless there is a payplan for this payment and patient.</summary>
		public static long GetPayPlanNum(long payNum,long patNum,PaySplit[] List) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<List.Length;i++){
				if(List[i].PayNum==payNum && List[i].PatNum==patNum && List[i].PayPlanNum!=0){
					return List[i].PayPlanNum;
				}
			}
			return 0;
		}

		///<summary>Returns every unearned PaySplit associated to the patients passed in, including TP prepayments.
		///This method can include some PaySplits that aren't directly associated to the patients passed in.
		///These PaySplits will be splits that are indirectly associated to the patients passed in via the payment.
		///E.g. a PatNum passed in is the PatNum on a payment but the payment has no PaySplits associated to the PatNum on the payment.
		///These are payments made to another family / patient in the database. The Account module needs to know about these splits.</summary>
		public static List<PaySplit> GetUnearnedForAccount(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM paysplit WHERE PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND UnearnedType!=0 "
			  +"UNION ALL "
				+"SELECT paysplit.* FROM payment "//We use payment here so that we can filter the results based on payment.PatNum
				+"INNER JOIN paysplit ON paysplit.PayNum=payment.PayNum " 
				+$"WHERE payment.PatNum IN ({string.Join(",",listPatNums)}) "
				+$"AND paysplit.PatNum NOT IN ({string.Join(",",listPatNums)}) "
				+"AND UnearnedType!=0 "
				+"ORDER BY DatePay";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		///<summary>Returns unearned PaySplits associated to the patients that contribute to the unearned bucket.
		///Hidden unearned types are ignored as well as any unearned PaySplit attached to a procedure (TP prepayments).</summary>
		public static List<PaySplit> GetUnearnedForPats(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			List<long> listHiddenUnearnedTypes=GetHiddenUnearnedDefNums();
			string strHiddenUnearned="";
			if(listHiddenUnearnedTypes.Count > 0) {
				strHiddenUnearned=$"AND UnearnedType NOT IN({string.Join(",",listHiddenUnearnedTypes)})";
			}
			string command=$@"SELECT * FROM paysplit
				WHERE PatNum IN ({string.Join(",",listPatNums)})
				AND UnearnedType!=0
				AND ProcNum=0
				{strHiddenUnearned}
				ORDER BY DatePay";
			return Crud.PaySplitCrud.SelectMany(command);
		}

		/// <summary>Gets a list of all unearned types that are marked as hidden on account.</summary>
		public static List<long> GetHiddenUnearnedDefNums() {
			//No need to check RemotingRole; no call to db.
			return Defs.GetHiddenUnearnedDefs().Select(x => x.DefNum).ToList();
		}

		///<summary>Returns the total amount of unearned for the patients. Provide a payNumExcluded to ignore a specific payment.</summary>
		public static decimal GetTotalAmountOfUnearnedForPats(List<long> listPatNums,long payNumExcluded=0) {
			//No need to check RemotingRole; no call to db.
			List<PaySplit> listUnearnedSplits=GetUnearnedForPats(listPatNums);
			//Remove any splits attached to the payment to exclude if one was passed in.
			if(payNumExcluded > 0) {
				listUnearnedSplits.RemoveAll(x => x.PayNum==payNumExcluded);
			}
			//At this point we know that the list of unearned splits contains all splits (negative and positive) that make up the unearned bucket.
			return (decimal)listUnearnedSplits.Sum(x => x.SplitAmt);
		}

		///<summary>Takes a procNum and returns a list of all paysplits associated to the procedure.Returns an empty list if there are none.</summary>
		public static List<PaySplit> GetPaySplitsFromProc(long procNum,bool onlyUnearned=false) {
			//No need to check RemotingRole; no call to db.
			return GetPaySplitsFromProcs(new List<long>() { procNum },onlyUnearned);
		}

		///<summary>Takes a list of procNums and returns a list of all paysplits associated to the procedures.  Returns an empty list if there are none.</summary>
		public static List<PaySplit> GetPaySplitsFromProcs(List<long> listProcNums,bool onlyUnearned=false) {
			if(listProcNums==null || listProcNums.Count<1) {
				return new List<PaySplit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),listProcNums,onlyUnearned);
			}
			string command="SELECT * FROM paysplit WHERE ProcNum IN("+string.Join(",",listProcNums)+")";
			if(onlyUnearned) {
				command+=$" AND UnearnedType > 0";
			}
			return Crud.PaySplitCrud.SelectMany(command);
		}
		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(PaySplit split) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				split.SplitNum=Meth.GetLong(MethodBase.GetCurrentMethod(),split);
				return split.SplitNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			split.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.PaySplitCrud.Insert(split);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(PaySplit split){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),split);
				return;
			}
			Crud.PaySplitCrud.Update(split);
		}

		///<summary></summary>
		public static void Update(PaySplit paySplit,PaySplit oldPaySplit) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),paySplit,oldPaySplit);
				return;
			}
			Crud.PaySplitCrud.Update(paySplit,oldPaySplit);
		}

		///<summary>Takes a procedure and updates the provnum of each of the paysplits attached.
		///Does nothing if there are no paysplits attached to the passed-in procedure.</summary>
		public static void UpdateAttachedPaySplits(Procedure proc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			Db.NonQ($@"UPDATE paysplit SET ProvNum = {POut.Long(proc.ProvNum)} WHERE ProcNum = {POut.Long(proc.ProcNum)}");
		}

		///<summary>Unlinks all paysplits that are currently linked to the passed-in adjustment. (Sets paysplit.AdjNum to 0)</summary>
		public static void UnlinkForAdjust(Adjustment adj) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adj);
				return;
			}
			Db.NonQ($@"UPDATE paysplit SET AdjNum = 0 WHERE AdjNum = {POut.Long(adj.AdjNum)}");
		}

		///<summary>Updates the provnum of all paysplits for a supplied adjustment.  Supply a list of splits to use that instead of querying the database.</summary>
		public static void UpdateProvForAdjust(Adjustment adj,List<PaySplit> listSplits=null) {
			if(listSplits!=null && listSplits.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adj,listSplits);
				return;
			}
			if(listSplits==null) {
				Db.NonQ($@"UPDATE paysplit SET ProvNum = {POut.Long(adj.ProvNum)} WHERE AdjNum = {POut.Long(adj.AdjNum)}");
			}
			else {
				Db.NonQ($@"UPDATE paysplit SET ProvNum = {POut.Long(adj.ProvNum)}
					WHERE SplitNum IN({string.Join(",",listSplits.Select(x => POut.Long(x.SplitNum)))})");
			}
		}

		///<summary>Inserts, updates, or deletes db rows to match listNew.</summary>
		public static bool Sync(List<PaySplit> listNew,List<PaySplit> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			return Crud.PaySplitCrud.Sync(listNew,listOld,Security.CurUser.UserNum);
		}
		#endregion

		#region Delete
		///<summary>Deletes the paysplit.</summary>
		public static void Delete(PaySplit split){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),split);
				return;
			}
			string command= "DELETE from paysplit WHERE SplitNum = "+POut.Long(split.SplitNum);
 			Db.NonQ(command);
		}

		///<summary>Used from payment window AutoSplit button to delete paysplits when clicking AutoSplit more than once.</summary>
		public static void DeleteForPayment(long payNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payNum);
				return;
			}
			string command="DELETE FROM paysplit"
				+" WHERE PayNum="+POut.Long(payNum);
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary>Returns true if a paysplit is attached to the associated procnum. Returns false otherwise.</summary>
		public static bool IsPaySplitAttached(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT COUNT(*) FROM paysplit WHERE ProcNum="+POut.Long(procNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		public static string GetSecurityLogMsgDelete(PaySplit paySplit,Payment payment=null) {
			return $"Paysplit deleted for: {Patients.GetLim(paySplit.PatNum).GetNameLF()}, {paySplit.SplitAmt.ToString("c")}, with payment type "
				+$"'{Payments.GetPaymentTypeDesc(payment??Payments.GetPayment(paySplit.PayNum))}'";
		}
		#endregion
	}
}










