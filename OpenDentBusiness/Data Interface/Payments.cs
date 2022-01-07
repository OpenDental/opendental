using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Payments {
		#region Get Methods
		///<summary>Gets all payments for the specified patient. This has NOTHING to do with pay splits.  Must use pay splits for accounting.  This is only for display in Account module.</summary>
		public static List<Payment> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <Payment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * from payment"
				+" WHERE PatNum="+patNum.ToString();
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Get one specific payment from db.</summary>
		public static Payment GetPayment(long payNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Payment>(MethodBase.GetCurrentMethod(),payNum);
			}
			string command=
				"SELECT * from payment"
				+" WHERE PayNum = '"+payNum+"'";
			return Crud.PaymentCrud.SelectOne(command);
		}

		///<summary>Get all specified payments.</summary>
		public static List<Payment> GetPayments(List<long> payNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <Payment>>(MethodBase.GetCurrentMethod(),payNums);
			}
			if(payNums.Count==0) {
				return new List <Payment>();
			}
			string command=
				"SELECT * from payment"
				+" WHERE";
			for(int i=0;i<payNums.Count;i++) {
				if(i>0) {
					command+=" OR";
				}
				command+=" PayNum="+payNums[i].ToString();
			}
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Gets all payments flagged as a transfer.  Optionally pass in PatNums to only get transfers for specific patients.</summary>
		public static List<Payment> GetTransfers(params long[] arrayPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <Payment>>(MethodBase.GetCurrentMethod(),arrayPatNums);
			}
			string command="SELECT * FROM payment WHERE PayType=0";
			if(!arrayPatNums.IsNullOrEmpty()) {
				command+=$" AND PatNum IN({string.Join(",",arrayPatNums.Select(x => POut.Long(x)))})";
			}
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Gets all payments for a family.</summary>
		public static List<Payment> GetNonSplitForPats(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Payment>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM payment "
				+"LEFT JOIN paysplit ON paysplit.PayNum=payment.PayNum "
				+"WHERE payment.PatNum IN("+String.Join(", ",listPatNums)+") "
				+"AND paysplit.SplitNum IS NULL";//Getting all payments with no splits
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Gets all payments attached to a single deposit.</summary>
		public static List<Payment> GetForDeposit(long depositNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <Payment>>(MethodBase.GetCurrentMethod(),depositNum);
			}
			string command=
				"SELECT * FROM payment "
				+"WHERE DepositNum = "+POut.Long(depositNum)+" "
				//Order by the date on the payment, and then the incremental order of the creation of each payment (doesn't affect random primary keys).
				//It was an internal complaint that checks on the same date show up in a 'random' order.
				//The real fix for this issue would be to add a time column and order by it by that instead of the PK.
				+"ORDER BY PayDate,PayNum";//Not usual pattern to order by PK
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Gets all unattached payments for a new deposit slip.  Excludes payments before dateStart.  There is a chance payTypes might be of length 1 or even 0.</summary>
		public static List<Payment> GetForDeposit(DateTime dateStart,long clinicNum,List<long> payTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List <Payment>>(MethodBase.GetCurrentMethod(),dateStart,clinicNum,payTypes);
			}
			string command=
				"SELECT * FROM payment "
				+"WHERE DepositNum = 0 "
				+"AND PayDate >= "+POut.Date(dateStart)+" ";
			if(clinicNum!=0){
				command+="AND ClinicNum="+POut.Long(clinicNum);
			}
			for(int i=0;i<payTypes.Count;i++) {
				if(i==0) {
					command+=" AND (";
				}
				else {
					command+=" OR ";
				}
				command+="PayType="+POut.Long(payTypes[i]);
				if(i==payTypes.Count-1) {
					command+=")";
				}
			}
			//Order by the date on the payment, and then the incremental order of the creation of each payment (doesn't affect random primary keys).
			//It was an internal complaint that checks on the same date show up in a 'random' order.
			//The real fix for this issue would be to add a time column and order by it by that instead of the PK.
			command+=" ORDER BY PayDate,PayNum";//Not usual pattern to order by PK
			object[] parameters={command,payTypes};
			Plugins.HookAddCode(null, "Payments.GetForDeposit_beforequeryrun", parameters);
			command=(string)parameters[0];
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Gets all payments that have a ProcessStatus of OnlinePending. Pass in an empty list to get payments for all clinics.</summary>
		public static List<Payment> GetNeedingProcessed(List<long> clinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Payment>>(MethodBase.GetCurrentMethod(),clinicNums);
			}
			string command="SELECT * FROM payment WHERE ProcessStatus="+POut.Int((int)ProcessStat.OnlinePending)+" ";
			if(clinicNums.Count>0) {
				command+="AND payment.ClinicNum IN ("+string.Join(",",clinicNums)+") ";
			}
			return Crud.PaymentCrud.SelectMany(command);
		}

		///<summary>Used in OpenDentalWebApps to get the payment based off of paysimple's payment ID.</summary>
		public static Payment GetForExternalId(string externalId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Payment>(MethodBase.GetCurrentMethod(),externalId);
			}
			string command="SELECT * FROM payment WHERE ExternalId='"+POut.String(externalId)+"'";
			return Crud.PaymentCrud.SelectOne(command);
		}

		///<summary>Gets all payments that have a ProcessStatus of OnlinePending for the clinic. Pass in a clinicNum of 0 to see all payments.</summary>
		public static int CountNeedingProcessed(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT COUNT(*) FROM payment WHERE ProcessStatus="+POut.Int((int)ProcessStat.OnlinePending)+" ";
			if(clinicNum!=0) {
				command+="AND payment.ClinicNum="+POut.Long(clinicNum);
			}
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Used for display in ProcEdit. List MUST include the requested payment. Use GetPayments to get the list.</summary>
		public static Payment GetFromList(long payNum,List<Payment> List) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<List.Count;i++){
				if(List[i].PayNum==payNum){
					return List[i];
				}
			}
			return null;//should never happen
		}

		///<summary>Returns the number of payments from the passed in paynums that are attached to a deposit other than IgnoreDepositNum.</summary>
		public static int GetCountAttachedToDeposit(List<long> listPayNums,long ignoreDepositNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),listPayNums,ignoreDepositNum);
			}
			if(listPayNums.Count==0) {
				return 0;
			}
			string command="";
			command="SELECT COUNT(*) FROM payment WHERE PayNum IN("+string.Join(",",listPayNums)+") AND DepositNum!=0";
			if(ignoreDepositNum!=0) {
				command+=" AND DepositNum!="+POut.Long(ignoreDepositNum);
			}
			return PIn.Int(Db.GetCount(command));
		}

		/*
		///<summary></summary>
		public static string GetInfo(int payNum){
			string retStr;
			Payment Cur=GetPayment(payNum);
			retStr=DefB.GetName(DefCat.PaymentTypes,Cur.PayType);
			if(Cur.IsSplit) retStr=retStr
				+"  "+Cur.PayAmt.ToString("c")
				+"  "+Cur.PayDate.ToString("d")
				+" "+Lans.g("Payments","split between patients");
			return retStr;
		}*/
		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(Payment pay) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pay.PayNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pay);
				return pay.PayNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			pay.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.PaymentCrud.Insert(pay);
		}

		///<summary>There's only one place in the program where this is called from.  Date is today, so no need to validate the date.</summary>
		public static long Insert(Payment pay,bool useExistingPK) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pay.PayNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pay,useExistingPK);
				return pay.PayNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			pay.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.PaymentCrud.Insert(pay,useExistingPK);
		}

		///<summary>Inserts the payment passed in as long as at least one payment split is passed in.  Returns 0 if no payment was inserted.</summary>
		public static long Insert(Payment pay,List<PaySplit> listPaySplits) {
			if(listPaySplits.IsNullOrEmpty()) {
				return 0;//Never insert a payment without any payment splits.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pay.PayNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pay,listPaySplits);
				return pay.PayNum;
			}
			Insert(pay);//The CRUD will set pay.PayNum accordingly.
			PaySplits.InsertMany(pay.PayNum,listPaySplits);
			return pay.PayNum;
		}

		///<summary></summary>
		public static void InsertMany(List<Payment> listPayments) {
			if(listPayments.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPayments);
				return;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listPayments.ForEach(x => x.SecUserNumEntry=Security.CurUser.UserNum);
			Crud.PaymentCrud.InsertMany(listPayments);
		}

		///<summary>Insert Payment and PaySplit. Returns newly inserted Payment.PayNum.  Throws exceptions if XWeb Program Properties are invalid.</summary>
		public static long InsertFromXWeb(long patNum,long provNum,long clinicNum,double amount,string payNote,string receipt,CreditCardSource ccSource) {
			//No need to check RemotingRole;no call to db.
			OpenDentBusiness.WebTypes.Shared.XWeb.WebPaymentProperties xwebProperties;
			OpenDentBusiness.ProgramProperties.GetXWebCreds(clinicNum,out xwebProperties);
			long ret=Payments.Insert(new Payment() {
				ClinicNum=clinicNum,
				IsRecurringCC=false,
				IsSplit=false,
				PatNum=patNum,
				PayAmt=amount,
				PayDate=DateTime.Now,
				PaymentSource=ccSource,
				PayType=xwebProperties.PaymentTypeDefNum,
				ProcessStatus=ProcessStat.OnlinePending,
				Receipt=receipt,
				PayNote=payNote,
			});
			PaySplits.Insert(new PaySplit() {
				ClinicNum=clinicNum,
				DatePay=DateTime.Now,
				PatNum=patNum,
				PayNum=ret,
				ProvNum=provNum,
				SplitAmt=amount,
			});
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,patNum,Lans.g("Payments.InsertFromXWeb","XWeb payment by")+" "
				+OpenDentBusiness.Patients.GetLim(patNum).GetNameLF()+", "+amount.ToString("c"),LogSources.PatientPortal);
			EServiceLogs.MakeLogEntry(eServiceAction.PPMadePayment,eServiceType.PatientPortal,FKeyType.PayNum,patNum:patNum,FKey:ret,clinicNum:clinicNum);
			return ret;
		}

		///<summary>Insert Payment and PaySplit. Returns newly inserted Payment.PayNum.  Throws exceptions if PayConnect Program Properties are invalid.</summary>
		public static long InsertFromPayConnect(long patNum,long provNum,long clinicNum,double amount,string payNote,string receipt,CreditCardSource ccSource) {
			//No need to check RemotingRole;no call to db.
			long ret=Payments.Insert(new Payment() {
				ClinicNum=clinicNum,
				IsRecurringCC=false,
				IsSplit=false,
				PatNum=patNum,
				PayAmt=amount,
				PayDate=DateTime.Now,
				PaymentSource=ccSource,
				PayType=PIn.Long(ProgramProperties.GetPropVal(Programs.GetCur(ProgramName.PayConnect).ProgramNum,"PaymentType",clinicNum)),
				ProcessStatus=ProcessStat.OnlinePending,
				Receipt=receipt,
				PayNote=payNote,
			});
			PaySplits.Insert(new PaySplit() {
				ClinicNum=clinicNum,
				DatePay=DateTime.Now,
				PatNum=patNum,
				PayNum=ret,
				ProvNum=provNum,
				SplitAmt=amount,
			});
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,patNum,Lans.g("Payments.InsertFromPayConnect","PayConnect payment by")+" "
				+OpenDentBusiness.Patients.GetLim(patNum).GetNameLF()+", "+amount.ToString("c"),LogSources.PatientPortal);
			EServiceLogs.MakeLogEntry(eServiceAction.PPMadePayment,eServiceType.PatientPortal,FKeyType.PayNum,patNum:patNum,clinicNum:clinicNum,FKey:ret);
			return ret;
		}

		public static long InsertFromCareCredit(long patNum,long provNum,long clinicNum,double amount,string payNote,CreditCardSource ccSource) {
			long payNum=Insert(new Payment() {
				ClinicNum=clinicNum,
				IsRecurringCC=false,
				IsSplit=false,
				PatNum=patNum,
				PayAmt=amount,
				PayDate=MiscData.GetNowDateTime(),
				PaymentSource=ccSource,
				PayType=PIn.Long(ProgramProperties.GetPropVal(Programs.GetCur(ProgramName.CareCredit).ProgramNum,
					ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType,clinicNum)),ProcessStatus=ProcessStat.OnlinePending,
				PayNote=payNote,
			});;
			PaySplits.Insert(new PaySplit() {
				ClinicNum=clinicNum,
				DatePay=DateTime.Now,
				PatNum=patNum,
				PayNum=payNum,
				ProvNum=provNum,
				SplitAmt=amount,
			});
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,patNum,Lans.g("Payments.InsertFromCareCredit","CareCredit payment by")+" "
				+Patients.GetLim(patNum).GetNameLF()+", "+amount.ToString("c"),LogSources.CareCredit);
			return payNum;
		}

		/// <summary>Populates a new payment with a negative paysplit for each paysplit in the payment passed in. Returns a list of negative paysplits and updates paymentRefund. Used to make refunds.</summary>
		public static Payment MakeNegativePaymentsRefund(Payment paymentExisting) {
			#region Make Payment
			List<PaySplit> listPaySplitsExisting=PaySplits.GetForPayment(paymentExisting.PayNum);
			Payment paymentRefund=new Payment();
			listPaySplitsExisting=listPaySplitsExisting.FindAll(x=> x.PayNum==paymentExisting.PayNum);
			//Create the refund payment with negative amount
			//Give the paytime of the original, the user will be able to select a different type on the payment window if they like
			paymentRefund.PayType=paymentExisting.PayType;
			paymentRefund.PayDate=DateTime.Today;
			paymentRefund.PayAmt=-paymentExisting.PayAmt;
			paymentRefund.IsSplit=paymentExisting.IsSplit;
			paymentRefund.PatNum=paymentExisting.PatNum;
			paymentRefund.ClinicNum=paymentExisting.ClinicNum;
			paymentRefund.DateEntry=DateTime.Today;
			#endregion
			#region Make Pay Splits
			//Create a negative payment split for each paysplit attached to the existing payment.
			List<PaySplit> listPaySplitsRefund= new List<PaySplit>();
			for(int i = 0;i<listPaySplitsExisting.Count;i++) {
				PaySplit paySplit=listPaySplitsExisting[i].Copy();
				paySplit.SplitNum=0;
				paySplit.PayNum=paymentRefund.PayNum;
				paySplit.DatePay=paymentRefund.PayDate;
				paySplit.DateEntry=paymentRefund.DateEntry;
				paySplit.SplitAmt=-listPaySplitsExisting[i].SplitAmt;
				listPaySplitsRefund.Add(paySplit);
			}
			#endregion
			Payments.Insert(paymentRefund,listPaySplitsRefund);
			return paymentRefund;
		}
		#endregion

		#region Update
		///<summary>Updates this payment.  Must make sure to update the datePay of all attached paysplits so that they are always in synch.  Also need to manually set IsSplit before here.  Will throw an exception if bad date, so surround by try-catch.  Set excludeDepositNum to true from FormPayment to prevent collision from another worksation that just deleted a deposit.</summary>
		public static void Update(Payment pay,bool excludeDepositNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pay,excludeDepositNum);
				return;
			}
			if(!PrefC.GetBool(PrefName.AccountAllowFutureDebits) && !PrefC.GetBool(PrefName.FutureTransDatesAllowed) && pay.PayDate.Date>DateTime.Today.Date) {
				throw new ApplicationException(Lans.g("Payments","Payment Date must not be a future date."));
			}
			if(pay.PayDate.Year<1880) {
				throw new ApplicationException(Lans.g("Payments","Invalid Payment Date"));
			}
			//the functionality below needs to be taken care of before calling the function:
			/*string command="SELECT DepositNum,PayAmt FROM payment "
					+"WHERE PayNum="+POut.PInt(PayNum);
			DataConnection dcon=new DataConnection();
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return;
			}
			if(table.Rows[0][0].ToString()!="0"//if payment is already attached to a deposit
					&& PIn.PDouble(table.Rows[0][1].ToString())!=PayAmt) {//and PayAmt changes
				throw new ApplicationException(Lans.g("Payments","Not allowed to change the amount on payments attached to deposits."));
			}*/
			Crud.PaymentCrud.Update(pay);
			if(!excludeDepositNum) {
				string command="UPDATE payment SET DepositNum="+POut.Long(pay.DepositNum)+" WHERE PayNum = "+POut.Long(pay.PayNum);
				Db.NonQ(command);
			}
		}

		///<summary>Updates a payment based upon the comparison of an old one versus a new one, ensuring less modification.</summary>
		public static void Update(Payment payNew,Payment payOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payNew,payOld);
				return;
			}
			Crud.PaymentCrud.Update(payNew,payOld);
		}
		#endregion

		#region Delete
		///<summary>Deletes the payment as well as all splits.
		///Surround with try catch, throws an exception if trying to delete a payment attached to a deposit.</summary>
		public static void Delete(Payment pay) {
			Delete(pay.PayNum);
		}

		///<summary>Deletes the payment as well as all splits.
		///Surround with try catch, throws an exception if trying to delete a payment attached to a deposit.</summary>
		public static void Delete(long payNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payNum);
				return;
			}
			string command="SELECT DepositNum,PayAmt FROM payment WHERE PayNum="+POut.Long(payNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return;
			}
			if(table.Rows[0]["DepositNum"].ToString()!="0"//if payment is already attached to a deposit
				&& PIn.Double(table.Rows[0]["PayAmt"].ToString())!=0)//and it's not new
			{
				throw new ApplicationException(Lans.g("Payments","Not allowed to delete a payment attached to a deposit."));
			}
			command= "DELETE from payment WHERE PayNum = "+POut.Long(payNum);
 			Db.NonQ(command);
			//this needs to be improved to handle EstBal
			command= "DELETE from paysplit WHERE PayNum = "+POut.Long(payNum);
			Db.NonQ(command);
			command="UPDATE recurringcharge SET PayNum=0 WHERE PayNum="+POut.Long(payNum);
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary>Called just before Allocate in FormPayment.butOK click.  If true, then it will prompt the user before allocating.</summary>
		public static bool AllocationRequired(double payAmt,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),payAmt,patNum);
			}
			string command="SELECT EstBalance FROM patient "
				+"WHERE PatNum = "+POut.Long(patNum);
			DataTable table=Db.GetTable(command);
			double estBal=0;
			if(table.Rows.Count>0){
				estBal=PIn.Double(table.Rows[0][0].ToString());
			}
			if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)){
				command=@"SELECT SUM(InsPayEst)+SUM(Writeoff) 
					FROM claimproc
					WHERE PatNum="+POut.Long(patNum)+" "
					+"AND Status=0"; //NotReceived
				table=Db.GetTable(command);
				if(table.Rows.Count>0){
					estBal-=PIn.Double(table.Rows[0][0].ToString());
				}
			}
			if(payAmt>estBal){
				return true;
			}
			return false;
		}

		/// <summary>Only Called only from FormPayment.butOK click.  Only called if the user did not enter any splits.  Usually just adds one split for the current patient.  But if that would take the balance negative, then it loops through all other family members and creates splits for them.  It might still take the current patient negative once all other family members are zeroed out.</summary>
		public static List<PaySplit> Allocate(Payment pay){//double amtTot,int patNum,Payment payNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PaySplit>>(MethodBase.GetCurrentMethod(),pay);
			}
			string command= 
				"SELECT Guarantor FROM patient "
				+"WHERE PatNum = "+POut.Long(pay.PatNum);
 			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return new List<PaySplit>();
			}
			command= 
				"SELECT patient.PatNum,EstBalance,PriProv,SUM(InsPayEst)+SUM(Writeoff) insEst_ "
				+"FROM patient "
				+"LEFT JOIN claimproc ON patient.PatNum=claimproc.PatNum "
				+"AND Status=0 "//NotReceived
				+"WHERE Guarantor = "+table.Rows[0][0].ToString()+" "
				+"GROUP BY  patient.PatNum,EstBalance,PriProv";
				//+" ORDER BY PatNum!="+POut.PInt(pay.PatNum);//puts current patient in position 0 //Oracle does not allow
 			table=Db.GetTable(command);
			List<Patient> pats=new List<Patient>();
			Patient pat;
			//first, put the current patient at position 0.
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["PatNum"].ToString()==pay.PatNum.ToString()){
					pat=new Patient();
					pat.PatNum    = PIn.Long(table.Rows[i][0].ToString());
					pat.EstBalance= PIn.Double(table.Rows[i][1].ToString());
					if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)){
						pat.EstBalance-=PIn.Double(table.Rows[i]["insEst_"].ToString());
					}
					pat.PriProv   = PIn.Long(table.Rows[i][2].ToString());
					pats.Add(pat.Copy());
				}
			}
			//then, do all the rest of the patients.
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["PatNum"].ToString()==pay.PatNum.ToString()){
					continue;
				}
				pat=new Patient();
				pat.PatNum    = PIn.Long   (table.Rows[i][0].ToString());
				pat.EstBalance= PIn.Double(table.Rows[i][1].ToString());
				if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)){
					pat.EstBalance-=PIn.Double(table.Rows[i]["insEst_"].ToString());
				}
				pat.PriProv   = PIn.Long   (table.Rows[i][2].ToString());
				pats.Add(pat.Copy());
			}
			//first calculate all the amounts
			double amtRemain=pay.PayAmt;//start off with the full amount
			double[] amtSplits=new double[pats.Count];
			//loop through each family member, starting with current
			for(int i=0;i<pats.Count;i++){
				if(pats[i].EstBalance==0 || pats[i].EstBalance<0){
					continue;//don't apply paysplits to anyone with a negative balance
				}
				if(amtRemain<pats[i].EstBalance){//entire remainder can be allocated to this patient
					amtSplits[i]=amtRemain;
					amtRemain=0;
					break;
				}
				else{//amount remaining is more than or equal to the estBal for this family member
					amtSplits[i]=pats[i].EstBalance;
					amtRemain-=pats[i].EstBalance;
				}
			}
			//add any remainder to the split for this patient
			amtSplits[0]+=amtRemain;
			//now create a split for each non-zero amount
			PaySplit PaySplitCur;
			List<PaySplit> retVal=new List<PaySplit>();
			for(int i=0;i<pats.Count;i++){
				if(amtSplits[i]==0){
					continue;
				}
				PaySplitCur=new PaySplit();
				PaySplitCur.PatNum=pats[i].PatNum;
				PaySplitCur.PayNum=pay.PayNum;
				PaySplitCur.DatePay=pay.PayDate;
				PaySplitCur.ClinicNum=pay.ClinicNum;
				PaySplitCur.ProvNum=Patients.GetProvNum(pats[i]);
				PaySplitCur.SplitAmt=Math.Round(amtSplits[i],CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
				//PaySplitCur.InsertOrUpdate(true);
				retVal.Add(PaySplitCur);
			}
			//finally, adjust each EstBalance, but no need to do current patient
			//This no longer works here.  Must do it when closing payment window somehow
			/*for(int i=1;i<pats.Length;i++){
				if(amtSplits[i]==0){
					continue;
				}
				command="UPDATE patient SET EstBalance=EstBalance-"+POut.PDouble(amtSplits[i])
					+" WHERE PatNum="+POut.PInt(pats[i].PatNum);
				Db.NonQ(command);
			}*/
			return retVal;
		}

		///<summary>This does all the validation before calling AlterLinkedEntries.  It had to be separated like this because of the complexity of saving a payment.  Surround with try-catch.  Will throw an exception if user is trying to change, but not allowed.  Will return false if no synch with accounting is needed.  Use -1 for newAcct to indicate no change.</summary>
		public static bool ValidateLinkedEntries(double oldAmt, double newAmt, bool isNew, long payNum, long newAcct){
			//No need to check RemotingRole; no call to db.
			if(!Accounts.PaymentsLinked()){
				return false;//user has not even set up accounting links, so no need to check any of this.
			}
			bool amtChanged=false;
			if(oldAmt!=newAmt) {
				amtChanged=true;
			}
			Transaction trans=Transactions.GetAttachedToPayment(payNum);//this gives us the oldAcctNum
			if(trans==null && (newAcct==0 || newAcct==-1)) {//if there was no previous link, and there is no attempt to create a link
				return false;//no synch needed
			}
			if(trans==null){//no previous link, but user is trying to create one. newAcct>0.
				return true;//new transaction will be required
			}
			//at this point, we have established that there is a previous transaction.
			//If payment is attached to a transaction which is more than 48 hours old, then not allowed to change.
			if(amtChanged && trans.DateTimeEntry < MiscData.GetNowDateTime().AddDays(-2)) {
				throw new ApplicationException(Lans.g("Payments","Not allowed to change amount that is more than 48 hours old.  This payment is already attached to an accounting transaction.  You will need to detach it from within the accounting section of the program."));
			}
			if(amtChanged && Transactions.IsReconciled(trans)) {
				throw new ApplicationException(Lans.g("Payments","Not allowed to change amount.  This payment is attached to an accounting transaction that has been reconciled.  You will need to detach it from within the accounting section of the program."));
			}
			List<JournalEntry> jeL=JournalEntries.GetForTrans(trans.TransactionNum);
			long oldAcct=0;
			JournalEntry jeDebit=null;
			JournalEntry jeCredit=null;
			double absOld=oldAmt;//the absolute value of the old amount
			if(oldAmt<0) {
				absOld=-oldAmt;
			}
			for(int i=0;i<jeL.Count;i++) {//we make sure down below that this count is exactly 2.
				if(Accounts.GetAccount(jeL[i].AccountNum).AcctType==AccountType.Asset) {
					oldAcct=jeL[i].AccountNum;
				}
				if(jeL[i].DebitAmt==absOld) {
					jeDebit=jeL[i];
				}
				//old credit entry
				if(jeL[i].CreditAmt==absOld) {
					jeCredit=jeL[i];
				}
			}
			if(jeCredit==null || jeDebit==null) {
				throw new ApplicationException(Lans.g("Payments","Not able to automatically make changes in the accounting section to match the change made here.  You will need to detach it from within the accounting section."));
			}
			if(oldAcct==0){//something must have gone wrong.  But this should never happen
				throw new ApplicationException(Lans.g("Payments","Could not locate linked transaction.  You will need to detach it manually from within the accounting section of the program."));
			}
			if(newAcct==0){//detaching it from a linked transaction.
				//We will delete the transaction
				return true;
			}
			bool acctChanged=false;
			if(newAcct!=-1 && oldAcct!=newAcct){
				acctChanged=true;//changing linked acctNum
			}
			if(!amtChanged && !acctChanged){
				return false;//no changes being made to amount or account, so no synch required.
			}
			if(jeL.Count!=2) {
				throw new ApplicationException(Lans.g("Payments","Not able to automatically change the amount in the accounting section to match the change made here.  You will need to detach it from within the accounting section."));
			}
			//Amount or account changed on an existing linked transaction.
			return true;
		}

		///<summary>Only called once from FormPayment when trying to change an amount or an account on a payment that's already linked to the Accounting section or when trying to create a new link.  This automates updating the Accounting section.  Do not surround with try-catch, because it was already validated in ValidateLinkedEntries above.  Use -1 for newAcct to indicate no changed. The name is required to give descriptions to new entries.</summary>
		public static void AlterLinkedEntries(double oldAmt, double newAmt, bool isNew, long payNum, long newAcct,DateTime payDate,
			string patName)
		{
			//No need to check RemotingRole; no call to db.
			if(!Accounts.PaymentsLinked()) {
				return;//user has not even set up accounting links.
			}
			bool amtChanged=false;
			if(oldAmt!=newAmt) {
				amtChanged=true;
			}
			Transaction trans=Transactions.GetAttachedToPayment(payNum);//this gives us the oldAcctNum
			double absNew=newAmt;//absolute value of the new amount
			if(newAmt<0) {
				absNew=-newAmt;
			}
			//if(trans==null && (newAcct==0 || newAcct==-1)) {//then this method will not even be called
			if(trans==null) {//no previous link, but user is trying to create one.
				//this is the only case where a new trans is required.
				trans=new Transaction();
				trans.PayNum=payNum;
				trans.UserNum=Security.CurUser.UserNum;
				Transactions.Insert(trans);//sets entry date
				//first the deposit entry
				JournalEntry je=new JournalEntry();
				je.AccountNum=newAcct;//DepositAccounts[comboDepositAccount.SelectedIndex];
				je.CheckNumber=Lans.g("Payments","DEP");
				je.DateDisplayed=payDate;//it would be nice to add security here.
				if(absNew==newAmt){//amount is positive
					je.DebitAmt=newAmt;
				}
				else{
					je.CreditAmt=absNew;
				}
				je.Memo=Lans.g("Payments","Payment -")+" "+patName;
				je.Splits=Accounts.GetDescript(PrefC.GetLong(PrefName.AccountingCashIncomeAccount));
				je.TransactionNum=trans.TransactionNum;
				JournalEntries.Insert(je);
				//then, the income entry
				je=new JournalEntry();
				je.AccountNum=PrefC.GetLong(PrefName.AccountingCashIncomeAccount);
				//je.CheckNumber=;
				je.DateDisplayed=payDate;//it would be nice to add security here.
				if(absNew==newAmt) {//amount is positive
					je.CreditAmt=newAmt;
				}
				else {
					je.DebitAmt=absNew;
				}
				je.Memo=Lans.g("Payments","Payment -")+" "+patName;
				je.Splits=Accounts.GetDescript(newAcct);
				je.TransactionNum=trans.TransactionNum;
				JournalEntries.Insert(je);
				return;
			}
			//at this point, we have established that there is a previous transaction.
			List<JournalEntry> jeL=JournalEntries.GetForTrans(trans.TransactionNum);
			long oldAcct=0;
			JournalEntry jeDebit=null;
			JournalEntry jeCredit=null;
			bool signChanged=false;
			double absOld=oldAmt;//the absolute value of the old amount
			if(oldAmt<0){
				absOld=-oldAmt;
			}
			if(oldAmt<0 && newAmt>0){
				signChanged=true;
			}
			if(oldAmt>0 && newAmt<0){
				signChanged=true;
			}
			for(int i=0;i<2;i++){
				if(Accounts.GetAccount(jeL[i].AccountNum).AcctType==AccountType.Asset) {
					oldAcct=jeL[i].AccountNum;
				}
				if(jeL[i].DebitAmt==absOld) {
					jeDebit=jeL[i];
				}
				//old credit entry
				if(jeL[i].CreditAmt==absOld) {
					jeCredit=jeL[i];
				}
			}
			//Already validated that both je's are not null, and that oldAcct is not 0.
			if(newAcct==0){//detaching it from a linked transaction. We will delete the transaction
				//we don't care about the amount
				Transactions.Delete(trans);//we need to make sure this doesn't throw any exceptions by carefully checking all
					//possibilities in the validation routine above.
				return;
			}
			//Either the amount or the account changed on an existing linked transaction.
			bool acctChanged=false;
			if(newAcct!=-1 && oldAcct!=newAcct) {
				acctChanged=true;//changing linked acctNum
			}
			if(amtChanged){
				if(signChanged) {
					jeDebit.DebitAmt=0;
					jeDebit.CreditAmt=absNew;
					jeCredit.DebitAmt=absNew;
					jeCredit.CreditAmt=0;
				}
				else {
					jeDebit.DebitAmt=absNew;
					jeCredit.CreditAmt=absNew;
				}
			}
			if(acctChanged){
				if(jeDebit.AccountNum==oldAcct){
					jeDebit.AccountNum=newAcct;
				}
				if(jeCredit.AccountNum==oldAcct) {
					jeCredit.AccountNum=newAcct;
				}
			}
			JournalEntries.Update(jeDebit);
			JournalEntries.Update(jeCredit);
		}

		public static DataTable GetFamilyBalancePayDatesCounts() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SELECT DateEntry,COUNT(*) FROM payment WHERE PayNote LIKE '"+"Auto-created by Family Balancer tool%"+"' GROUP BY DateEntry";
			DataTable table=Db.GetTable(command);
			return table;
		}

		public static DataTable GetFamilyBalanceTransferForDate(DateTime dateEntry) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateEntry);
			}
			string command="SELECT PayNum,PatNum FROM payment WHERE DateEntry="+POut.Date(dateEntry)+" AND PayNote LIKE '"+"Auto-created by Family Balancer tool%"+"'";
			return Db.GetTable(command);
		}

		///<summary>Returns a string that can be used for securitylog entries.</summary>
		public static string GetSecuritylogEntryText(Payment paymentNew,Payment paymentOld,bool isNew,List<Def> listPayTypes=null) {
			string secLogText;
			if(listPayTypes==null) {
				listPayTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes);
			}
			string clinicAbbrNew=Clinics.GetAbbr(paymentNew.ClinicNum);
			if(isNew) {
				secLogText=$"Payment created for {Patients.GetLim(paymentNew.PatNum).GetNameLF()} with payment type '"+
					GetPaymentTypeDesc(paymentNew,listPayTypes)+"'";
				if(!string.IsNullOrEmpty(clinicAbbrNew)) {
					secLogText+=$", clinic '{clinicAbbrNew}'";
				}
				secLogText+=$", amount '{paymentNew.PayAmt.ToString("c")}'";
			}
			else {
				secLogText=$"Payment edited for {Patients.GetLim(paymentNew.PatNum).GetNameLF()}";
				secLogText+=SecurityLogEntryTextHelper(paymentNew.PayAmt.ToString("c"),paymentOld.PayAmt.ToString("c"),"amount");
				secLogText+=SecurityLogEntryTextHelper(clinicAbbrNew,Clinics.GetAbbr(paymentOld.ClinicNum),"clinic");
				secLogText+=SecurityLogEntryTextHelper(paymentNew.PayDate.ToShortDateString(),paymentOld.PayDate.ToShortDateString(),"payment date");
				secLogText+=SecurityLogEntryTextHelper(GetPaymentTypeDesc(paymentNew,listPayTypes),GetPaymentTypeDesc(paymentOld,listPayTypes),"payment type");
			}
			return secLogText;
		}

		///<summary>Returns the payment type string for the payment.</summary>
		public static string GetPaymentTypeDesc(Payment payment,List<Def> listPayTypes=null) {
			if(listPayTypes==null) {
				listPayTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes);
			}
			return payment.PayType==0 ? "Income Transfer" : Defs.GetName(DefCat.PaymentTypes,payment.PayType,listPayTypes);
		}

		///<summary>Securitylog text helper that returns a string of value changes passed in. Returns empty string if no changes.</summary>
		private static string SecurityLogEntryTextHelper(string newVal,string oldVal,string textInLog) {
			return (newVal!=oldVal ? $"\r\n {textInLog} changed from '{oldVal}' to '{newVal}'" : "");
		}

		///<summary>Creates a transfer originating from the prepayment containing the procOriginal, back to the procOriginal. 
		///Used to transfer money from TP unearned back onto the procedure as an allocated non pre-pay split. 
		///Optionally pass in procNumAttaching when wanting to attach to a procedure other than the procOriginal. 
		///Optionally pass in transferAmountOverride when transferring an amount that is not the split amount, as in the case for broken procs.</summary>
		public static void CreateTransferForTpProcs(Procedure procOriginal,List<PaySplit> listSplitsForProc,Procedure procAttaching=null,
			double transferAmountOverride=0) 
		{
			if(listSplitsForProc.IsNullOrEmpty() || listSplitsForProc.Sum(x => x.SplitAmt)==0) {
				return;
			}
			//Remove all TP that are associated to DPP/PP
			listSplitsForProc.RemoveAll(x => x.PayPlanChargeNum!=0);
			if(listSplitsForProc.IsNullOrEmpty()) {
				return;
			}
			procAttaching=procAttaching??procOriginal;
			Payment transferPayment=new Payment();
			transferPayment.PayDate=DateTime.Today;
			transferPayment.ClinicNum=procOriginal.ClinicNum;
			transferPayment.PayNote="Automatic transfer from treatment planned procedure prepayment.";
			transferPayment.PatNum=procAttaching.PatNum;//ultimately where the payment ends up.
			transferPayment.PayType=0;
			Insert(transferPayment);
			foreach(PaySplit prepaySplit in listSplitsForProc) {
				//make negative split to remove the 'tp prepayment'
				PaySplit negSplitForTxfr=new PaySplit {
					ClinicNum=procOriginal.ClinicNum,
					DatePay=DateTime.Today,
					ProcNum=0,//either the procedure is being set complete, or pref for Non-Refundable TP prepay is set and transferring to procAttaching.
					//If non-refundable the procedure needs to be disassociated as well as we will not have a way to determine when procOriginal eventually
					//gets set complete and unearned cannot exist with a completed procedure attached. 
					PatNum=procOriginal.PatNum,
					PayNum=transferPayment.PayNum,
					SplitAmt=(transferAmountOverride==0?prepaySplit.SplitAmt:transferAmountOverride)*-1,
					UnearnedType=prepaySplit.UnearnedType,
					ProcDate=procOriginal.ProcDate,
					ProvNum=prepaySplit.ProvNum,
				};
				PaySplits.Insert(negSplitForTxfr);
				//Update original pre-payment split to disassociate the procedure now that the procedure is complete (Splits cannot have unaerned and C proc)
				prepaySplit.ProcNum=0;
				PaySplits.Update(prepaySplit);
				PaySplit positiveSplit=new PaySplit{
					ClinicNum=procAttaching.ClinicNum,
					DatePay=DateTime.Today,
					ProcNum=procAttaching.ProcNum,
					PatNum=procAttaching.PatNum,
					PayNum=transferPayment.PayNum,
					SplitAmt=transferAmountOverride==0?prepaySplit.SplitAmt:transferAmountOverride,
					ProcDate=procAttaching.ProcDate,
					ProvNum=procAttaching.ProvNum,
					UnearnedType=0//necessary for when broken appointments do not get a procedure created for them to transfer to. 
				};
				PaySplits.Insert(positiveSplit);
			}
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,transferPayment.PatNum,"Automatic transfer of funds for treatment plan procedure pre-payments.");
		}
		#endregion
	}

}










