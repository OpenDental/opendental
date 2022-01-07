using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimPayments {
		///<summary></summary>
		public static DataTable GetForClaim(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),claimNum);
			}
			DataTable table=new DataTable();
			DataRow row;
			table.Columns.Add("amount");
			table.Columns.Add("payType");
			table.Columns.Add("BankBranch");
			table.Columns.Add("ClaimPaymentNum");
			table.Columns.Add("checkDate");
			table.Columns.Add("CheckNum");
			table.Columns.Add("Note");
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT BankBranch,claimpayment.ClaimPaymentNum,CheckNum,CheckDate,"
				+"SUM(claimproc.InsPayAmt) amount,Note,PayType "
				+"FROM claimpayment,claimproc "
				+"WHERE claimpayment.ClaimPaymentNum = claimproc.ClaimPaymentNum "
				+"AND claimproc.ClaimNum = '"+POut.Long(claimNum)+"' "
				+"GROUP BY claimpayment.ClaimPaymentNum, BankBranch, CheckDate, CheckNum, Note, PayType";
			DataTable rawT=Db.GetTable(command);
			DateTime date;
			for(int i=0;i<rawT.Rows.Count;i++) {
				row=table.NewRow();
				row["amount"]=PIn.Double(rawT.Rows[i]["amount"].ToString()).ToString("F");
				row["payType"]=Defs.GetName(DefCat.InsurancePaymentType,PIn.Long(rawT.Rows[i]["PayType"].ToString()));
				row["BankBranch"]=rawT.Rows[i]["BankBranch"].ToString();
				row["ClaimPaymentNum"]=rawT.Rows[i]["ClaimPaymentNum"].ToString();
				date=PIn.Date(rawT.Rows[i]["CheckDate"].ToString());
				row["checkDate"]=date.ToShortDateString();
				row["CheckNum"]=rawT.Rows[i]["CheckNum"].ToString();
				row["Note"]=rawT.Rows[i]["Note"].ToString();
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Returns dictionary such that every given claim num is a key and the value is a bool indicating if their is a claimpayment associated to it.</summary>
		public static SerializableDictionary<long,bool> HasClaimPayment(List<long> listClaimNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,bool>>(MethodBase.GetCurrentMethod(),listClaimNums);
			}
			SerializableDictionary<long,bool> retVal=new SerializableDictionary<long,bool>();
			listClaimNums=listClaimNums.Distinct().ToList();
			if(listClaimNums==null || listClaimNums.Count==0) {
				return retVal;
			}
			string command=@"SELECT claimproc.ClaimNum
											FROM claimproc
											INNER JOIN claimpayment ON claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum
											WHERE claimproc.ClaimNum IN ("+string.Join(",",listClaimNums)+") "+
											"GROUP BY claimproc.ClaimNum";
			List<long> listRetClaimNums=Db.GetListLong(command);
			foreach(long claimNum in listClaimNums) {
				retVal.Add(claimNum,listRetClaimNums.Contains(claimNum));
			}
			return retVal;
		}
		
		///<summary>Gets all claimpayments of the specified claimpayment type, within the specified date range and from the specified clinic. 
		///0 for clinics means all clinics, 0 for claimpaytype means all types.</summary>
		public static DataTable GetForDateRange(DateTime dateFrom,DateTime dateTo,long clinicNum,long claimpayGroup) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,clinicNum,claimpayGroup);
			}
			string command="SELECT claimpayment.*,"
				+"(CASE WHEN (SELECT COUNT(*) FROM eobattach WHERE eobattach.ClaimPaymentNum=claimpayment.ClaimPaymentNum)>0 THEN 1 ELSE 0 END) hasEobAttach "
				+"FROM claimpayment "
				+"WHERE CheckDate >= "+POut.Date(dateFrom)+" "
				+"AND CheckDate <= "+POut.Date(dateTo)+" ";
			if(clinicNum!=0) {
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(claimpayGroup!=0) {
				command+="AND PayGroup="+POut.Long(claimpayGroup)+" ";
			}
			command+="ORDER BY CheckDate";
			return Db.GetTable(command);
		}

		public static List<ClaimPayment> GetClaimPaymentsWithEobAttach(List<long> listClaimPaymentNums) {
			if(listClaimPaymentNums.Count==0) {
				return new List<ClaimPayment>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPayment>>(MethodBase.GetCurrentMethod(),listClaimPaymentNums);
			}
			string command="SELECT * "
				+"FROM claimpayment "
				+"INNER JOIN eobattach ON eobattach.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
				+"WHERE claimpayment.ClaimPaymentNum IN("+string.Join(",",listClaimPaymentNums.Select(x => POut.Long(x)))+") "
				+"GROUP BY claimpayment.ClaimPaymentNum";
			return Crud.ClaimPaymentCrud.SelectMany(command);
		}

		public static List<ClaimPayment> GetByClaimPaymentNums(List<long> listClaimPaymentNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPayment>>(MethodBase.GetCurrentMethod(),listClaimPaymentNums);
			}
			if(listClaimPaymentNums.Count==0) {
				return new List<ClaimPayment>();
			}
			string command="SELECT * "
				+"FROM claimpayment "
				+"WHERE ClaimPaymentNum IN("+string.Join(",",listClaimPaymentNums.Select(x => POut.Long(x)))+")";
			return Crud.ClaimPaymentCrud.SelectMany(command);
		}

		///<summary>Gets all unattached claimpayments for display in a new deposit.  Excludes payments before dateStart and partials.</summary>
		public static List<ClaimPayment> GetForDeposit(DateTime dateStart,long clinicNum,List<long> insPayTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPayment>>(MethodBase.GetCurrentMethod(),dateStart,clinicNum,insPayTypes);
			}
			string command=
				"SELECT * FROM claimpayment "
				+"INNER JOIN definition ON claimpayment.PayType=definition.DefNum "
				+"WHERE DepositNum = 0 "
				+"AND definition.ItemValue='' "//Check if payment type should show in the deposit slip.  'N'=not show, empty string means should show.
				+"AND CheckDate >= "+POut.Date(dateStart);
			if(clinicNum!=0){
				command+=" AND ClinicNum="+POut.Long(clinicNum);
			}
			for(int i=0;i<insPayTypes.Count;i++) {
				if(i==0) {
					command+=" AND PayType IN ("+insPayTypes[0];
				}
				else {
					command+=","+insPayTypes[i];
				}
				if(i==insPayTypes.Count-1) {
					command+=")";
				}
			}
			command+=" AND IsPartial=0"//Don't let users attach partial insurance payments to deposits.
				+" AND CheckAmt!=0"//Users kept complaining about zero dollar insurance payments showing up, so don't show them.
				//Order by the date on the check, and then the incremental order of the creation of each payment (doesn't affect random primary keys).
				//It was an internal complaint that checks on the same date show up in a 'random' order.
				//The real fix for this issue would be to add a time column and order by it by that instead of the PK.
				+" ORDER BY CheckDate,ClaimPaymentNum";//Not usual pattern to order by PK
			return Crud.ClaimPaymentCrud.SelectMany(command);
		}

		///<summary>Gets all claimpayments for one specific deposit.</summary>
		public static ClaimPayment[] GetForDeposit(long depositNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClaimPayment[]>(MethodBase.GetCurrentMethod(),depositNum);
			}
			string command=
				"SELECT * FROM claimpayment "
				+"INNER JOIN definition ON claimpayment.PayType=definition.DefNum "
				+"WHERE DepositNum = "+POut.Long(depositNum)
				+" AND definition.ItemValue=''"//Check if payment type should show in the deposit slip.  'N'=not show, empty string means should show.
				//Order by the date on the check, and then the incremental order of the creation of each payment (doesn't affect random primary keys).
				//It was an internal complaint that checks on the same date show up in a 'random' order.
				//The real fix for this issue would be to add a time column and order by it by that instead of the PK.
				+" ORDER BY CheckDate,ClaimPaymentNum";//Not usual pattern to order by PK
			return Crud.ClaimPaymentCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets one claimpayment directly from database.</summary>
		public static ClaimPayment GetOne(long claimPaymentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClaimPayment>(MethodBase.GetCurrentMethod(),claimPaymentNum);
			}
			string command=
				"SELECT * FROM claimpayment "
				+"WHERE ClaimPaymentNum = "+POut.Long(claimPaymentNum);
			return Crud.ClaimPaymentCrud.SelectOne(command);
		}

		///<summary>Returns the number of partial batch insurance payments or if there are any claimprocs with status Received that have InsPayAmts but 
		///are not associated to a claim payment. Used to warn users that reports will be inaccurate until insurance payments are finalized.</summary>
		public static int GetPartialPaymentsCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			//Union together two queries that look for incomplete insurance payments.
			//The first query will look for any partial batch insurance payments.
			//The second query will look for any claim payments (InsPayAmt > 0) that do not have a claim payment (no check / not finalized).
			string command=@"
				SELECT COUNT(*) 
				FROM ((SELECT claimpayment.ClaimPaymentNum 
					FROM claimpayment 
					WHERE claimpayment.IsPartial = 1 
					AND claimpayment.CheckDate <= "+POut.Date(DateTime.Now)+@" 
					AND claimpayment.CheckDate >= "+POut.Date(DateTime.Now.AddMonths(-1))+@")
						UNION ALL
					(SELECT claimproc.ClaimProcNum 
					FROM claimproc
					WHERE claimproc.ClaimPaymentNum = 0
					AND claimproc.InsPayAmt != 0 
					AND claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapClaim)+@") 
					AND claimproc.DateEntry <= "+POut.Date(DateTime.Now)+@" 
					AND claimproc.DateEntry >= "+POut.Date(DateTime.Now.AddMonths(-1))+@"
					AND claimproc.IsTransfer=0
					GROUP BY claimproc.ClaimNum)
				) partialpayments";//claimproc.DateEntry is updated when payment is received.
			return PIn.Int(Db.GetCount(command),false);
		}

		///<summary></summary>
		public static long Insert(ClaimPayment cp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				cp.ClaimPaymentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cp);
				return cp.ClaimPaymentNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			cp.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.ClaimPaymentCrud.Insert(cp);
		}

		///<summary>If trying to change the amount and attached to a deposit, it will throw an error, so surround with try catch.</summary>
		public static void Update(ClaimPayment cp,bool isDepNew=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cp,isDepNew);
				return;
			}
			if(!isDepNew && cp.DepositNum!=0 && PrefC.GetBool(PrefName.ShowAutoDeposit)) {
				string cmd="SELECT deposit.Amount,SUM(COALESCE(claimpayment.CheckAmt,0))+SUM(COALESCE(payment.PayAmt,0)) depAmtOthers "
					+"FROM deposit "
					+"LEFT JOIN payment ON payment.DepositNum=deposit.DepositNum "
					+"LEFT JOIN claimpayment ON claimpayment.DepositNum=deposit.DepositNum AND claimpayment.ClaimPaymentNum!="+POut.Long(cp.ClaimPaymentNum)+" "
					+"WHERE deposit.DepositNum="+POut.Long(cp.DepositNum);
				DataTable tble=Db.GetTable(cmd);
				if(tble.Rows.Count==0) {
					cp.DepositNum=0;
				}
				else if(PIn.Double(tble.Rows[0]["depAmtOthers"].ToString())+cp.CheckAmt!=PIn.Double(tble.Rows[0]["Amount"].ToString())) {
					throw new ApplicationException(Lans.g("ClaimPayments","Not allowed to change the amount on checks attached to deposits."));
				}
			}
			else {
				string command="SELECT DepositNum,CheckAmt FROM claimpayment "
				+"WHERE ClaimPaymentNum="+POut.Long(cp.ClaimPaymentNum);
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count==0){
					return;
				}
				if(table.Rows[0][0].ToString()!="0"//if claimpayment is already attached to a deposit
					&& PIn.Double(table.Rows[0][1].ToString())!=cp.CheckAmt)//and checkAmt changes
				{
					throw new ApplicationException(Lans.g("ClaimPayments","Not allowed to change the amount on checks attached to deposits."));
				}
			}
			Crud.ClaimPaymentCrud.Update(cp);
		}

		///<summary>Surround by try catch, because it will throw an exception if trying to delete a claimpayment attached to a deposit or if there are eobs attached.</summary>
		public static void Delete(ClaimPayment cp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cp);
				return;
			}
			//validate deposits
			string command="SELECT DepositNum FROM claimpayment "
				+"WHERE ClaimPaymentNum="+POut.Long(cp.ClaimPaymentNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return;
			}
			if(table.Rows[0][0].ToString()!="0" && !HasAutoDeposit(cp)){//if claimpayment is already attached to a deposit and was not created automatically
				if(!ODBuild.IsDebug()) {
					throw new ApplicationException(Lans.g("ClaimPayments","Not allowed to delete a payment attached to a deposit."));
				}
			}
			//validate eobs
			command="SELECT COUNT(*) FROM eobattach WHERE ClaimPaymentNum="+POut.Long(cp.ClaimPaymentNum);
			if(Db.GetScalar(command)!="0") {
				throw new ApplicationException(Lans.g("ClaimPayments","Not allowed to delete this payment because EOBs are attached."));
			}
			if(table.Rows[0][0].ToString()!="0") {//deposit was created automatically. Delete deposit.
				Deposit deposit=Deposits.GetOne(cp.DepositNum);
				if(deposit!=null) {
					Deposits.Delete(deposit);
				}
			}
			command="UPDATE claimproc SET "
				+"DateInsFinalized='0001-01-01' "
				+"WHERE ClaimPaymentNum="+POut.Long(cp.ClaimPaymentNum)+" "
				+"AND (SELECT SecDateEntry FROM claimpayment WHERE ClaimPaymentNum="+POut.Long(cp.ClaimPaymentNum)+")=CURDATE()";
			Db.NonQ(command);
			command= "UPDATE claimproc SET "
				+"ClaimPaymentNum=0 "
				+"WHERE claimpaymentNum="+POut.Long(cp.ClaimPaymentNum);
			//MessageBox.Show(string command);
			Db.NonQ(command);
			command= "DELETE FROM claimpayment "
				+"WHERE ClaimPaymentnum ="+POut.Long(cp.ClaimPaymentNum);
			//MessageBox.Show(string command);
 			Db.NonQ(command);
		}

		///<summary>Returns the number of payments from the passed in claimpaymentnums that are attached to a deposit other than IgnoreDepositNum.</summary>
		public static int GetCountAttachedToDeposit(List<long> listClaimPaymentNums,long ignoreDepositNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),listClaimPaymentNums,ignoreDepositNum);
			}
			if(listClaimPaymentNums.Count==0) {
				return 0;
			}
			string command="";
			command="SELECT COUNT(*) FROM claimpayment WHERE ClaimPaymentNum IN("+string.Join(",",listClaimPaymentNums)+") AND DepositNum!=0";
			if(ignoreDepositNum!=0) {
				command+=" AND DepositNum!="+POut.Long(ignoreDepositNum);
			}
			return PIn.Int(Db.GetCount(command));
		}

		public static bool HasAutoDeposit(ClaimPayment cp) {
			if(cp==null || cp.DepositNum==0 || !PrefC.GetBool(PrefName.ShowAutoDeposit)) {
				return false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),cp);
			}
			//Per Mark on 07/16/2018
			//A deposit is consided an "Auto Deposit" if the ShowAutoDeposit preference is turned on
			//and only one claimpayment is attached to the deposit passed in. 
			string command="SELECT COUNT(*) FROM claimpayment where DepositNum="+POut.Long(cp.DepositNum);
			return PIn.Int(Db.GetCount(command))==1;
		}
	}

	

	


}









